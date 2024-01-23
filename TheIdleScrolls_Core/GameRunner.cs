using MiniECS;

using TheIdleScrolls_Core.Systems;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.DataAccess;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Utility;
using TheIdleScrollsApp;

namespace TheIdleScrolls_Core
{
    public class GameRunner
    {
        ulong m_ticks = 0;

        readonly World m_world = new() { XpMultiplier = 5.0, RarityMultiplier = 2.0 };

        readonly Coordinator m_coordinator = new();

        readonly List<AbstractSystem> m_systems = new();

        readonly DataAccessHandler m_dataHandler;

        readonly IUserInputHandler m_userInputHandler;

        readonly ApplicationUpdateSystem m_appUpdateSystem;

        double m_prevSpeedMulti = -1.0;

        public bool IsPaused { get { return m_prevSpeedMulti > 0.0; } }

        public ulong Ticks { get { return m_ticks; } }

        public DataAccessHandler DataAccessHandler { get { return m_dataHandler; } }
        public WorldMap WorldMap { get { return m_world.Map; } }

        public GameRunner(DataAccessHandler dataHandler)
        {
            m_ticks = 0;
            m_dataHandler = dataHandler;
            m_userInputHandler = new UserInputSystem();
            m_appUpdateSystem = new ApplicationUpdateSystem();

            m_systems.Add(m_userInputHandler as dynamic);
            m_systems.Add(new TimeLimitSystem());
            m_systems.Add(new TravelSystem());
            m_systems.Add(new MobSpawnerSystem());
            m_systems.Add(new TargetSelectorSystem());
            m_systems.Add(new AttackProcessingSystem());
            m_systems.Add(new KillProcessingSystem());
            m_systems.Add(new LevelUpSystem());
            m_systems.Add(new QuestSystem());
            m_systems.Add(new TutorialSystem());
            m_systems.Add(new EquipmentManagementSystem());
            m_systems.Add(new AbilitiesSystem());
            m_systems.Add(new ModifierSystem());
            m_systems.Add(new StatUpdateSystem());
            m_systems.Add(new DungeonSystem());
            m_systems.Add(new LootSystem());
            m_systems.Add(new CraftingSystem());
            m_systems.Add(new PlayerProgressSystem());
            m_systems.Add(new AchievementsSystem());
            m_systems.Add(new SaveSystem(dataHandler));
            m_systems.Add(m_appUpdateSystem);
        }

        public async Task Initialize(string playerName = "Leeroy")
        {
            const string globalEntityName = "_perpetual";
            Entity? globalEntity = await m_dataHandler.LoadEntity(globalEntityName);
            if (globalEntity == null)
            {
                globalEntity = new Entity();
                globalEntity.AddComponent(new NameComponent(globalEntityName));
                globalEntity.AddComponent(new AchievementsComponent());
            }
            if (globalEntity.GetComponent<PlayerProgressComponent>() == null)
            {
                globalEntity.AddComponent(new PlayerProgressComponent()); // so far only used for tutorial progress
            }
            m_coordinator.AddEntity(globalEntity);
            m_world.GlobalEntity = globalEntity;

            var player = await PlayerFactory.MakeOrLoadPlayer(playerName, m_dataHandler);
            AddPlayerToCoordinator(player);

            Logger.LogMessage($"Player '{player.GetName()}' (Level {player.GetComponent<LevelComponent>()?.Level ?? 0}) spawned (#{player.Id})");

            try
            {
                var mobs = ReadResourceFile<List<MobDescription>>("Mobs.json");
                GetSystem<MobSpawnerSystem>()?.SetMobList(mobs);

                m_world.ItemKingdom = ItemFactory.ItemKingdom;              
            }
            catch (Exception e)
            {
                Logger.LogMessage(e.Message);
            }
        }

        public IUserInputHandler GetUserInputHandler()
        {
            return m_userInputHandler;
        }

        public IGameEventEmitter GetEventEmitter()
        {
            return m_appUpdateSystem;
        }

        public void SetAppInterface(IApplicationModel model)
        {
            m_appUpdateSystem.SetApplicationInterface(model);
        }

        void AddPlayerToCoordinator(Entity player)
        {
            m_coordinator.AddEntity(player);

            player.GetComponent<InventoryComponent>()?.GetItems()?.ForEach(i => m_coordinator.AddEntity(i));
            player.GetComponent<EquipmentComponent>()?.GetItems()?.ForEach(i => m_coordinator.AddEntity(i));
        }

        public void ExecuteTick(double dt)
        {
/*            var tickStart = DateTime.Now;*/

            dt *= m_world.SpeedMultiplier;
            m_ticks++;

            foreach (var system in m_systems)
            {
                m_coordinator.DeleteMessagesFromSender(system);
                system.Update(m_world, m_coordinator, dt);
            }

/*            var tickDuration = DateTime.Now - tickStart;
            Console.WriteLine($"Tick duration: {tickDuration.TotalMilliseconds}");*/
        }

        public void AddSystem(AbstractSystem system)
        {
            m_systems.Add(system);
        }

        public void Pause()
        {
            if (!IsPaused)
            {
                m_prevSpeedMulti = m_world.SpeedMultiplier;
                m_world.SpeedMultiplier = 0.0;
            }
        }

        public void Unpause()
        {
            if (IsPaused)
            {
                m_world.SpeedMultiplier = m_prevSpeedMulti;
                m_prevSpeedMulti = -1.0;
            }
        }

        public T? GetSystem<T>() where T : AbstractSystem
        {
            foreach (var system in m_systems)
            {
                if (system.GetType() == typeof(T))
                    return (T)system;
            }
            return null;
        }

        static T ReadResourceFile<T>(string file)
        {
            return ResourceAccess.ParseResourceFile<T>("TheIdleScrolls_Core", file);
        }

        public bool IsGameOver()
        {
            return m_world.GameOver;
        }
    }
}