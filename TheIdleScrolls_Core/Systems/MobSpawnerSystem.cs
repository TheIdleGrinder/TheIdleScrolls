using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Systems
{
    public class MobSpawnerSystem : AbstractSystem
    {
        MobFactory m_factory = new();
        List<MobDescription> m_descriptions = new();

        public void SetMobList(List<MobDescription> mobs)
        {
            m_descriptions = mobs;
        }

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            foreach (Entity player in coordinator.GetEntities<PlayerComponent>())
            {
                var locationComp = player.GetComponent<LocationComponent>();
                if (locationComp == null)
                    continue;
                
                var zone = locationComp.GetCurrentZone(world.Map);
                if (zone == null)
                    throw new Exception($"Player {player.GetName()} is not in a valid zone");

                // Check if a new mob needs to be spawned
                // TODO: Give mobs LocationComponent and actually check if a mob is at the players location
                var mobCount = coordinator.GetEntities<MobComponent>().Count;
                if (mobCount > 0 || locationComp.RemainingEnemies <= 0)
                {
                    continue;
                }

                List<MobDescription> additionalMobs = (locationComp.InDungeon) 
                    ? world.AreaKingdom.GetLocalEnemies(locationComp.DungeonId) 
                    : new();
                var mob = CreateRandomMob(zone, additionalMobs);

                if (locationComp.InDungeon && zone.MobCount > 1)
                {
                    string name = mob.GetName();
                    int mobNo = zone.MobCount - locationComp.RemainingEnemies + 1;
                    name += $" [{mobNo}/{zone.MobCount}]";
                    mob.GetComponent<NameComponent>()!.Name = name;
                }
                coordinator.AddEntity(mob);
                coordinator.PostMessage(this, new MobSpawnMessage(mob));
            }
        }

        private Entity CreateRandomMob(ZoneDescription zone, List<MobDescription> additionalMobs)
        {
            int level = zone.Level;
            var allMobs = m_descriptions.Concat(additionalMobs);
            var validMobs = allMobs.Where(m => m.MinLevel <= level && m.MaxLevel >= level);
            if (zone.MobTypes.Any())
            {
                validMobs = validMobs.Where(m => zone.MobTypes.Contains(m.Name));
            }
            if (validMobs == null || !validMobs.Any())
                throw new Exception($"No valid mobs for area level {level}");
            int index = new Random().Next(validMobs.Count());
            return MobFactory.MakeMob(validMobs.ElementAt(index), level);
        }
    }

    public class MobSpawnMessage : IMessage
    {
        public Entity Mob;

        string IMessage.BuildMessage()
        {
            string name = Mob.GetComponent<NameComponent>()?.Name ?? "<unknown>";
            int level = Mob.GetComponent<LevelComponent>()?.Level ?? 0;
            int hp = Mob.GetComponent<LifePoolComponent>()?.Maximum ?? 0;
            return $"Spawned level {level} {name} with {hp} HP";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Low;
        }

        public MobSpawnMessage(Entity mob)
        {
            Mob = mob;
        }
    }
}
