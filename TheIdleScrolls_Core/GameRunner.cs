using MiniECS;

using TheIdleScrolls_Core.Systems;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.DataAccess;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Utility;
using TheIdleScrollsApp;

namespace TheIdleScrolls_Core
{
    public class GameRunner
    {
        ulong m_ticks = 0;

        World m_world = new World { XpMultiplier = 10.0 };

        Coordinator m_coordinator = new();

        List<AbstractSystem> m_systems = new();

        DataAccessHandler m_dataHandler;

        IUserInputHandler m_userInputHandler;

        ApplicationUpdateSystem m_appUpdateSystem;

        public ulong Ticks { get { return m_ticks; } }

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
            m_systems.Add(new TutorialSystem());
            m_systems.Add(new EquipmentManagementSystem());
            m_systems.Add(new AbilitiesSystem());
            m_systems.Add(new StatUpdateSystem());
            m_systems.Add(new PlayerProgressSystem());
            m_systems.Add(new SaveSystem(dataHandler));
            m_systems.Add(m_appUpdateSystem);
        }

        public void Initialize(string playerName = "Leeroy")
        {

            var player = PlayerFactory.MakeOrLoadPlayer(playerName, m_dataHandler);
            AddPlayerToCoordinator(player);

            Logger.LogMessage($"Player '{player.GetName()}' (Level {player.GetComponent<LevelComponent>()?.Level ?? 0}) spawned (#{player.Id})");

            try
            {
                var mobs = ReadResourceFile<List<MobDescription>>("Mobs.json");
                GetSystem<MobSpawnerSystem>()?.SetMobList(mobs);

                var areas = ReadResourceFile<AreaKingdomDescription>("Dungeons.json");
                m_world.AreaKingdowm = areas;

                var items = ReadResourceFile<ItemKingdomDescription>("Items.json");
                m_world.ItemKingdom = items;

                var inventoryComp = player.GetComponent<InventoryComponent>();
                if (inventoryComp != null && inventoryComp.ItemCount == 0)
                {
                    foreach (var item in ItemFactory.GetAllItemDescriptions(items))
                    {
                        var weapon = ItemFactory.MakeItem(item);
                        inventoryComp.AddItem(weapon);
                        m_coordinator.AddEntity(weapon);
                    }
                }                
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

        public T? GetSystem<T>() where T : AbstractSystem
        {
            foreach (var system in m_systems)
            {
                if (system.GetType() == typeof(T))
                    return (T)system;
            }
            return null;
        }

        T ReadResourceFile<T>(string file)
        {
            return ResourceAccess.ParseResourceFile<T>("TheIdleScrolls_Core", file);
        }
    }
}