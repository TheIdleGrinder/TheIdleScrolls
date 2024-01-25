using Microsoft.JSInterop;
using MiniECS;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.DataAccess;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Messages;
using TheIdleScrolls_Core.Storage;
using TheIdleScrolls_Storage;

namespace TheIdleScrolls_Web.CoreWrapper
{
    public delegate void CharacterListChangeHandler(List<string> characters);
    public delegate void CharacterLoadedHandler();
    public delegate void StateChangedHandler();
    public delegate void GameLoopRunStateChangedHandler(bool running);

    public record TitledMessage(string Title, string Message);

    public class CoreWrapperModel : IApplicationModel
    {
        readonly IJSRuntime jSRuntime;
        DataAccessHandler dataHandler;
        GameRunner gameRunner;

        bool gameLoopRunning = false;

        public event CharacterListChangeHandler? CharacterListChanged;
        public event CharacterLoadedHandler? CharacterLoaded;
        public event StateChangedHandler? StateChanged;
        public event GameLoopRunStateChangedHandler? GameLoopRunStateChanged;

        public IUserInputHandler InputHandler { get => gameRunner.GetUserInputHandler(); }

        public bool GameLoopRunning => gameLoopRunning;
        public List<string> StoredCharacters { get; set; } = new();
        public CharacterRepresentation Character { get; set; } = new(0, "", "", 0);
        public int XpCurrent { get; set; } = 0;
        public int XpTarget { get; set; } = 0;
        public TimeLimit TimeLimit { get; private set; } = new();
        public AreaRepresentation Area { get; private set; } = new("", 0, false);
        public MobRepresentation Mob { get; private set; } = new(0, "", 0, 0, 0);
        public AccessibleAreas Accessible { get; } = new();
        public bool AutoProceedActive { get; private set; } = false;
        public HashSet<GameFeature> AvailableFeatures { get; } = new();
        public Equipment Equipment { get; private set; } = new();
        public List<ItemRepresentation> Inventory { get; private set; } = new();
        public int Coins { get; private set; } = 0;
        public CharacterStats CharacterStats { get; private set; } = new();
        public List<AchievementRepresentation> Achievements { get; private set; } = new();
        public List<AbilityRepresentation> Abilities { get; private set; } = new();
        public int AchievementCount { get; private set; } = 0;
        public string StatisticsReport { get; private set; } = String.Empty;
        public List<DialogueMessage> DialogueMessages { get; private set; } = new();
        public List<ExpiringMessage> ExpiringMessages { get; private set; } = new();


        public WorldMap WorldMap => gameRunner.WorldMap;
        // (Ab)use wrapper to store the currently hightlighted item used in InventoryDisplay and EquipmentDisplay
        public uint HighlightedItem { get; private set; } = uint.MaxValue;
        
        public bool IsFeatureAvailable(GameFeature feature) => AvailableFeatures.Contains(feature);

        public CoreWrapperModel(IJSRuntime js)
        {
            jSRuntime = js;
            Reset();
        }

        public HashSet<IMessage.PriorityLevel> GetRelevantMessagePriorties()
        {
            return new() { IMessage.PriorityLevel.High, IMessage.PriorityLevel.VeryHigh };
        }

        public async Task UpdateSavedCharacters()
        {
            StoredCharacters = await dataHandler.ListStoredEntities();
            CharacterListChanged?.Invoke(StoredCharacters);
        }

        public async Task LoadCharacter(string name)
        {
            await gameRunner.Initialize(name);
            CharacterLoaded?.Invoke();
        }

        public async Task<CharacterMetaData?> GetCharacterMetaData(string name)
        {
            return await dataHandler.GetCharacterMetaData(name);
        }

        public async Task StartGameLoop()
        {
            Console.WriteLine("Starting game loop");
            const int frameTime = 50;
            gameLoopRunning = true;
            GameLoopRunStateChanged?.Invoke(gameLoopRunning);
            while (gameLoopRunning)
            {
                try
                {
                    gameRunner.ExecuteTick(frameTime / 1000.0);
                    StateChanged?.Invoke();
                    await Task.Delay(frameTime);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception caught: {e.Message}");
                    StopGameLoop();
                }
            }
            Console.WriteLine("Game loop finished");
        }

        public void StopGameLoop()
        {
            gameLoopRunning = false;
            GameLoopRunStateChanged?.Invoke(gameLoopRunning);
            StateChanged?.Invoke();
        }

        private void ConnectEvents()
        {
            var emitter = gameRunner.GetEventEmitter();
            emitter.PlayerCharacterChanged += (CharacterRepresentation cRep) => Character = cRep;
            emitter.PlayerXpChanged += (int current, int target) =>
            {
                XpCurrent = current;
                XpTarget = target;
            };
            emitter.TimeLimitChanged += (double remaining, double max) => 
            {
                TimeLimit.Remaining = remaining;
                TimeLimit.Maximum = max;
            };
            emitter.MobChanged += (MobRepresentation mob) => Mob = mob;
            emitter.PlayerAreaChanged += (string name, int level, bool isDungeon) => Area = new(name, level, isDungeon);
            emitter.AccessibleAreasChanged += (int maxWild, List<DungeonRepresentation> dungeons) => 
            {
                Accessible.MaxWilderness = maxWild;
                Accessible.Dungeons = dungeons;
            };
            emitter.PlayerAutoProceedStateChanged += (bool active) => AutoProceedActive = active;
            emitter.FeatureAvailabilityChanged += (GameFeature feature, bool available) =>
            {
                if (available)
                    AvailableFeatures.Add(feature);
                else
                    AvailableFeatures.Remove(feature);
            };
            emitter.PlayerEquipmentChanged += (List<ItemRepresentation> items) => Equipment.SetItems(items);
            emitter.PlayerInventoryChanged += (List<ItemRepresentation> items) => Inventory = items;
            emitter.PlayerCoinsChanged += (int coins) => Coins = coins;
            emitter.PlayerOffenseChanged += (double dmg, double cdMax, double cd) =>
            {
                CharacterStats.Damage = dmg;
                CharacterStats.Cooldown = cdMax;
                CharacterStats.CooldownRemaining = cd;
            };
            emitter.PlayerDefenseChanged += (double armor, double evasion) =>
            {
                CharacterStats.Armor = armor;
                CharacterStats.Evasion = evasion;
            };
            emitter.PlayerEncumbranceChanged += (double encumbrance) => CharacterStats.Encumbrance = encumbrance;
            emitter.AchievementsChanged += (List<AchievementRepresentation> achievements, int count) =>
            {
                Achievements = achievements;
                AchievementCount = count;
            };
            emitter.PlayerAbilitiesChanged += (List<AbilityRepresentation> abilities) => Abilities = abilities;
            
            emitter.StatReportChanged += (string report) => StatisticsReport = report;
            emitter.DisplayMessageReceived += (string title, string message) =>
            {
                DialogueMessages.Add(new(string.Empty, string.Empty, title, message, new()));
                gameRunner.Pause();
            };
            emitter.DialogueMessageReceived += (DialogueMessage message) => DialogueMessages.Add(message);
            emitter.NewLogMessages += (List<string> messages) =>
            {
                foreach (var message in messages)
                {
                    ExpiringMessages.Add(new(message, 5.0));
                }
                ExpiringMessages = ExpiringMessages.Where(m => !m.Expired).ToList();
            };
        }

        [MemberNotNull(nameof(dataHandler))]
        [MemberNotNull(nameof(gameRunner))]
        public void Reset()
        {
            StopGameLoop();
            dataHandler = new DataAccessHandler(new EntityJsonConverter(new ItemFactory()), new LocalBrowserStorageHandler(jSRuntime));
            gameRunner = new GameRunner(dataHandler);
            gameRunner.SetAppInterface(this);
            ConnectEvents();
        }

        public void MarkTopMessageAsRead()
        {
            if (DialogueMessages.Count > 0)
            {
                DialogueMessages.RemoveAt(0);
            }

            if (DialogueMessages.Count <= 0)
            {
                gameRunner.Unpause();
            }

            if (gameRunner.IsGameOver())
            {
                StopGameLoop();
            }
        }

        public void SendResponse(string response)
        {
            string id = DialogueMessages[0].ResponseId;
            MarkTopMessageAsRead();
            gameRunner.GetUserInputHandler().SendDialogueResponse(id, response);
        }

        public bool ToggleItemHighlight(uint itemId)
        {
            if (HighlightedItem != itemId)
            {
                HighlightedItem = itemId;
                return true;
            }
            else
            {
                HighlightedItem = uint.MaxValue;
                return false;
            }
        }
        public bool IsItemHighlighted(uint itemId)
        {
            return HighlightedItem == itemId;
        }

        public ItemRepresentation? GetOwnedItem(uint itemId)
        {
            var result = Equipment.Items.FirstOrDefault(item => item.Id == itemId);
            if (result == null)
            {
                result = Inventory.FirstOrDefault(item => item.Id == itemId);
            }
            return result;
        }
    }
}
