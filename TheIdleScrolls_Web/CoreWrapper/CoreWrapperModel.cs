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
        public MobRepresentation Mob { get; private set; } = new(0, "", "", 0, 0, 0);
        public AccessibleAreas Accessible { get; } = new();
        public List<IItemEntity> CraftingRecipes { get; private set; } = new();
        public CraftingBenchRepresentation CraftingBench { get; private set; } = new(0, 0, 0, new());
        public bool AutoProceedActive { get; private set; } = false;
        public HashSet<GameFeature> AvailableFeatures { get; } = new();
        public Equipment Equipment { get; private set; } = new();
        public List<IItemEntity> Inventory { get; private set; } = new();
        public int Coins { get; private set; } = 0;
        public CharacterStats CharacterStats { get; private set; } = new();
        public List<AchievementRepresentation> Achievements { get; private set; } = new();
        public List<AbilityRepresentation> Abilities { get; private set; } = new();
        public List<PerkRepresentation> Perks { get; private set; } = new();
        public int AchievementCount { get; private set; } = 0;
        public string StatisticsReport { get; private set; } = String.Empty;
        public BountyStateRepresentation BountyState { get; private set; } = new(0, 0, 0, 0, 0);
        public List<DialogueMessage> DialogueMessages { get; private set; } = new();
        public List<ExpiringMessage> ExpiringMessages { get; private set; } = new();


        public DataAccessHandler DataAccessHandler => dataHandler;
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
            const int frameTime = 50;
            gameLoopRunning = true;
            GameLoopRunStateChanged?.Invoke(gameLoopRunning);
            long owedTime = 0;
            while (gameLoopRunning)
            {
                try
                {
                    // Somehow this runs the smoothest. Only using Task.Delay if frame time was not completely used loses ~3-4 secs per minute
                    System.Diagnostics.Stopwatch sw = new();
                    sw.Start();

                    var delay = Task.Delay(frameTime);

                    gameRunner.ExecuteTick((frameTime + owedTime) / 1000.0);
                    StateChanged?.Invoke();
					owedTime = 0;
					await delay;

					sw.Stop();

                    if (sw.ElapsedMilliseconds >= frameTime)
                    {
                        owedTime = sw.ElapsedMilliseconds - frameTime;
                    }
                    //Console.WriteLine($"Frame time: {sw.ElapsedMilliseconds}ms");
				}
                catch (Exception e)
                {
                    Console.WriteLine($"Exception caught: {e.Message}");
                    StopGameLoop();
                }
            }
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
            emitter.AvailableCraftingRecipesChanged += (List<IItemEntity> recipes) => CraftingRecipes = recipes;
            emitter.CraftingBenchChanged += (CraftingBenchRepresentation bench) => CraftingBench = bench;
            emitter.PlayerAutoProceedStateChanged += (bool active) => AutoProceedActive = active;
            emitter.FeatureAvailabilityChanged += (GameFeature feature, bool available) =>
            {
                if (available)
                    AvailableFeatures.Add(feature);
                else
                    AvailableFeatures.Remove(feature);
            };
            emitter.PlayerEquipmentChanged += (List<IItemEntity> items) => Equipment.SetItems(items);
            emitter.PlayerInventoryChanged += (List<IItemEntity> items) => Inventory = items;
            emitter.PlayerCoinsChanged += (int coins) => Coins = coins;
            emitter.PlayerOffenseChanged += (double dmg, double cdMax, double cd) =>
            {
                CharacterStats.Damage = dmg;
                CharacterStats.Cooldown = cdMax;
                CharacterStats.CooldownRemaining = cd;
            };
            emitter.PlayerDefenseChanged += (double armor, double evasion, double defenseRating) =>
            {
                CharacterStats.Armor = armor;
                CharacterStats.Evasion = evasion;
                CharacterStats.DefenseRating = defenseRating;
            };
            emitter.PlayerEncumbranceChanged += (double encumbrance) => CharacterStats.Encumbrance = encumbrance;
            emitter.AchievementsChanged += (List<AchievementRepresentation> achievements, int count) =>
            {
                Achievements = achievements;
                AchievementCount = count;
            };
            emitter.PlayerAbilitiesChanged += (List<AbilityRepresentation> abilities) => Abilities = abilities;
            emitter.PlayerPerksChanged += (List<PerkRepresentation> perks) => Perks = perks;
            emitter.StatReportChanged += (string report) => StatisticsReport = report;
            emitter.BountyStateChanged += (BountyStateRepresentation bounty) => BountyState = bounty;
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
                    AddExpiringMessage(message);
                }
            };
        }

        [MemberNotNull(nameof(dataHandler))]
        [MemberNotNull(nameof(gameRunner))]
        public void Reset()
        {
            StopGameLoop();
            dataHandler = new DataAccessHandler(
                new EntityJsonConverter(), 
                new LocalBrowserStorageHandler(jSRuntime),
                new Base64ConversionDecorator<string>(
                    new InputToByteArrayConversionDecorator<byte[]>(
                        new NopDataEncryptor<byte[]>()
                    )
                ));
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

        public void AddExpiringMessage(string message)
        {
			ExpiringMessages.Add(new(message, 5.0));
			ExpiringMessages = ExpiringMessages.Where(m => !m.Expired).ToList();
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

        public IItemEntity? GetItem(uint itemId)
        {
            var result = Equipment.Items.FirstOrDefault(item => item.Id == itemId) 
                ?? Inventory.FirstOrDefault(item => item.Id == itemId)
                ?? CraftingRecipes.FirstOrDefault(item => item.Id == itemId);
            return result;
        }

        public double CalculateReforgingSuccessRate(int rarity)
        {
            int level = Abilities.FirstOrDefault(a => a.Key == TheIdleScrolls_Core.Definitions.Abilities.Crafting)?.Level ?? 0;
            return Functions.CalculateReforgingSuccessRate(level, rarity);
        }

        public async Task ExportAll()
        {
            await gameRunner.DataAccessHandler.ExportAllCharacters();
        }
    }
}
