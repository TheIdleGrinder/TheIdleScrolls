using MiniECS;

using TheIdleScrolls_Core.Systems;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.DataAccess;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Utility;
using TheIdleScrollsApp;
using TheIdleScrolls_Core.Resources;
using TheIdleScrolls_Core.ContentPacks;

namespace TheIdleScrolls_Core
{
    public class GameRunner
    {
        ulong m_ticks = 0;

        public readonly World GameWorld = new() { XpMultiplier = 5.0, RarityMultiplier = 2.0 };

        readonly Coordinator m_coordinator = new();

        readonly List<AbstractSystem> m_systems = [];

        public List<string> ActiveContentPacks = [];

        readonly DataAccessHandler m_dataHandler;

        readonly IUserInputHandler m_userInputHandler;

        readonly ApplicationUpdateSystem m_appUpdateSystem;

        double m_prevSpeedMulti = -1.0;

        public bool IsPaused { get { return m_prevSpeedMulti > 0.0; } }

        public ulong Ticks { get { return m_ticks; } }

        public DataAccessHandler DataAccessHandler { get { return m_dataHandler; } }
        public WorldMap WorldMap { get { return GameWorld.Map; } }

        public GameRunner(DataAccessHandler dataHandler)
        {
            m_ticks = 0;
            m_dataHandler = dataHandler;
            m_userInputHandler = new UserInputSystem();
            m_appUpdateSystem = new ApplicationUpdateSystem();

            m_systems.Add(m_userInputHandler as dynamic);
            m_systems.Add(new AchievementsSystem());
            m_systems.Add(new TravelSystem());
            m_systems.Add(new BattleSystem());
            m_systems.Add(new MobSpawnerSystem());
            m_systems.Add(new BountySystem());
            m_systems.Add(new KillProcessingSystem());
            m_systems.Add(new LevelUpSystem());
            m_systems.Add(new QuestSystem());
            m_systems.Add(new TutorialSystem());
            m_systems.Add(new EquipmentManagementSystem());
            m_systems.Add(new AbilitiesSystem());
            m_systems.Add(new PerksSystem());
            m_systems.Add(new StatUpdateSystem());
            m_systems.Add(new EvasionSystem());
            m_systems.Add(new DungeonSystem());
            m_systems.Add(new LootSystem());
            m_systems.Add(new CraftingSystem());
            m_systems.Add(new PlayerProgressSystem());
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
            GameWorld.GlobalEntity = globalEntity;

            var player = await PlayerFactory.MakeOrLoadPlayer(playerName, m_dataHandler);
            AddPlayerToCoordinator(player);

            Logger.LogMessage($"Player '{player.GetName()}' (Level {player.GetComponent<LevelComponent>()?.Level ?? 0}) spawned (#{player.Id})");
            
            GetSystem<MobSpawnerSystem>()?.SetMobList(MobList.Mobs);

            ResetContent();
			ActivateContentPack(new FightingStylesContentPack());
			ActivateContentPack(new CraftingContentPack());
			ActivateContentPack(new WarriorGettingStartedContentPack());
			ActivateContentPack(new WarriorCampaignContentPack());
			ActivateContentPack(new WarriorEndgameContentPack());
            ActivateContentPack(new CoinAchievementPack());
            ActivateContentPack(new AdditionalAchievementPack());
            ActivateContentPack(new EquipmentAchievementPack());
			ActivateContentPack(new UnderequippedAchievementPack());
			ActivateContentPack(new SpeedrunAchievementPack());
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
            player.GetComponent<CraftingBenchComponent>()?.ActiveCrafts?.ForEach(c => m_coordinator.AddEntity(c.TargetItem));
        }

        public void ExecuteTick(double dt)
        {
            var tickStart = DateTime.Now;
            dt *= GameWorld.SpeedMultiplier;
            m_ticks++;

            System.Diagnostics.Stopwatch sw = new();

            foreach (var system in m_systems)
            {
                try
                {
                    sw.Restart();

                    m_coordinator.DeleteMessagesFromSender(system);
                    system.Update(GameWorld, m_coordinator, dt);

                    sw.Stop();
                //if (sw.ElapsedMilliseconds > 5)
                //    Console.WriteLine($"{system.GetType().Name} took {sw.ElapsedMilliseconds} ms");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error in {system.GetType().Name}: {e.Message}");
                }
            }

            var tickDuration = DateTime.Now - tickStart;
            //if (tickDuration.Milliseconds > 50)
            //    Console.WriteLine($"Tick duration: {tickDuration.TotalMilliseconds}");
            //else
            //    Console.WriteLine("Tick duration ok");
        }

        public void AddSystem(AbstractSystem system)
        {
            m_systems.Add(system);
        }

        public void Pause()
        {
            if (!IsPaused)
            {
                m_prevSpeedMulti = GameWorld.SpeedMultiplier;
                GameWorld.SpeedMultiplier = 0.0;
            }
        }

        public void Unpause()
        {
            if (IsPaused)
            {
                GameWorld.SpeedMultiplier = m_prevSpeedMulti;
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

        private void ActivateContentPack(IContentPack contentPack)
		{
			if (ActiveContentPacks.Contains(contentPack.Id))
				return;

			IContentPack.Activate(contentPack);
			ActiveContentPacks.Add(contentPack.Id);
		}

		void ResetContent()
		{
			ActiveContentPacks = [];
			AbilityList.Reset();
			AchievementList.Reset();
			DungeonList.Reset();
			QuestList.Reset();
		}

        public bool IsGameOver()
        {
            return GameWorld.GameOver;
        }
    }
}