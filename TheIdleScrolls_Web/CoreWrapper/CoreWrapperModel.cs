using Microsoft.JSInterop;
using MiniECS;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.DataAccess;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Storage;
using TheIdleScrolls_Storage;

namespace TheIdleScrolls_Web.CoreWrapper
{
    public delegate void CharacterListChangeHandler(List<string> characters);
    public delegate void CharacterLoadedHandler();
    public delegate void StateChangedHandler();

    public class CoreWrapperModel : IApplicationModel
    {
        DataAccessHandler dataHandler;
        GameRunner gameRunner;

        bool gameLoopRunning = false;

        public event CharacterListChangeHandler? CharacterListChanged;
        public event CharacterLoadedHandler? CharacterLoaded;
        public event StateChangedHandler? StateChanged;

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
        public HashSet<GameFeature> AvailableFeatures { get; } = new();
        public Equipment Equipment { get; private set; } = new();
        public List<ItemRepresentation> Inventory { get; private set; } = new();
        public CharacterStats CharacterStats { get; private set; } = new();
        public List<AchievementRepresentation> Achievements { get; private set; } = new();
        public List<AbilityRepresentation> Abilities { get; private set; } = new();
        public int AchievementCount { get; private set; } = 0;


        public bool IsFeatureAvailable(GameFeature feature) => AvailableFeatures.Contains(feature);

        public CoreWrapperModel(IJSRuntime js)
        {
            dataHandler = new DataAccessHandler(new EntityJsonConverter(new ItemFactory()), new LocalBrowserStorageHandler(js));
            gameRunner = new GameRunner(dataHandler);
            gameRunner.SetAppInterface(this);
            ConnectEvents();
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

        public async Task StartGameLoop()
        {
            const int frameTime = 50;
            gameLoopRunning = true;
            while (gameLoopRunning)
            {
                gameRunner.ExecuteTick(frameTime / 1000.0);
                StateChanged?.Invoke();
                await Task.Delay(frameTime);
            }
        }

        public void StopGameLoop()
        {
            gameLoopRunning = false;
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
            emitter.FeatureAvailabilityChanged += (GameFeature feature, bool available) =>
            {
                if (available)
                    AvailableFeatures.Add(feature);
                else
                    AvailableFeatures.Remove(feature);
            };
            emitter.PlayerEquipmentChanged += (List<ItemRepresentation> items) => Equipment.SetItems(items);
            emitter.PlayerInventoryChanged += (List<ItemRepresentation> items) => Inventory = items;
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
        }
    }
}
