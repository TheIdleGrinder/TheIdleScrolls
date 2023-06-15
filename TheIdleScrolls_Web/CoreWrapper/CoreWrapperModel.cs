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

        public bool GameLoopRunning => gameLoopRunning;
        public List<string> StoredCharacters { get; set; } = new();
        public CharacterRepresentation Character { get; set; } = new(0, "Leeroy", "Adventurer", 0);
        public int XpCurrent { get; set; } = 0;
        public int XpTarget { get; set; } = 0;

        public CoreWrapperModel(IJSRuntime js)
        {
            dataHandler = new DataAccessHandler(new EntityJsonConverter(new ItemFactory()), new LocalBrowserStorageHandler(js));
            gameRunner = new GameRunner(dataHandler);
            gameRunner.SetAppInterface(this);

            var emitter = gameRunner.GetEventEmitter();
            emitter.PlayerCharacterChanged += (CharacterRepresentation cRep) => Character = cRep;
            emitter.PlayerXpChanged += UpdateXp;
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

        private void UpdateXp(int current, int target)
        {
            XpCurrent = current;
            XpTarget = target;
            Console.WriteLine($"XP: {XpCurrent}/{XpTarget}");
        }
    }
}
