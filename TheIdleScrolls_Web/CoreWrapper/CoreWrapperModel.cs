﻿using Microsoft.JSInterop;
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

    public class TimeLimit
    {
        public double Remaining { get; set; } = 0.0;
        public double Maximum { get; set; } = 0.0;
    }

    public class AccessibleAreas
    {
        public int MaxWilderness { get; set; } = 0;
        public List<DungeonRepresentation> Dungeons { get; set; } = new();
    }

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
        public TimeLimit TimeLimit { get; set; } = new();
        public AreaRepresentation Area { get; set; } = new("", 0, false);
        public MobRepresentation Mob { get; set; } = new(0, "", 0, 0, 0);
        public AccessibleAreas Accessible { get; } = new();
        public HashSet<GameFeature> AvailableFeatures { get; } = new();


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
        }
    }
}
