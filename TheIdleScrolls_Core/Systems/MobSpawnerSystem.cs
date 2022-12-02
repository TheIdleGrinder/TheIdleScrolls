using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TheIdleScrolls_Core.Components;

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
            int mobCount = 0;
            foreach (var entity in coordinator.GetEntities())
            {
                if (entity.HasComponent<MobComponent>())
                    mobCount++;
            }

            if (mobCount < 1)
            {
                var mob = CreateRandomMob(world);
                coordinator.AddEntity(mob);
                coordinator.PostMessage(this, new MobSpawnMessage(mob));
            }
        }

        private Entity CreateRandomMob(World world)
        {
            int level = world.AreaLevel;
            var validMobs = m_descriptions.Where(m => m.MinLevel <= level && m.MaxLevel >= level);
            if (validMobs == null || !validMobs.Any())
                throw new Exception($"No valid mobs for area level {level}");
            int index = new Random().Next(validMobs.Count());
            return m_factory.MakeMob(validMobs.ElementAt(index), level);
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
            return $"Spawned level {level} {name} with {hp} HP (#{Mob.Id})";
        }

        public MobSpawnMessage(Entity mob)
        {
            Mob = mob;
        }
    }
}
