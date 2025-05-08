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
        List<MobDescription> m_descriptions = new();

        public void SetMobList(List<MobDescription> mobs)
        {
            m_descriptions = mobs;
        }

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            foreach (Entity player in coordinator.GetEntities<PlayerComponent, BattlerComponent>())
            {
                var battle = player.GetComponent<BattlerComponent>()!.Battle;
                if (battle.NeedsMob)
                {
                    var locationComp = player.GetComponent<LocationComponent>() 
                        ?? throw new Exception($"{player.GetName()} is not in a valid location");
                    
                    var zone = locationComp.GetCurrentZone(world.Map)
                        ?? throw new Exception($"Player {player.GetName()} is not in a valid zone");

                    List<MobDescription> additionalMobs = (locationComp.InDungeon)
                        ? world.AreaKingdom.GetLocalEnemies(locationComp.DungeonId)
                        : new();
                    var mob = CreateRandomMob(zone, additionalMobs);

                    battle.Mob = mob;
                    battle.MobsRemaining--;
                    mob.AddComponent(new BattlerComponent(battle));

                    if (locationComp.InDungeon && zone.MobCount > 1)
                    {
                        string name = mob.GetName();
                        int mobNo = zone.MobCount - battle.MobsRemaining;
                        name += $" [{mobNo}/{zone.MobCount}]";
                        mob.GetComponent<NameComponent>()!.Name = name;
                    }
                    coordinator.AddEntity(mob);
                    coordinator.PostMessage(this, new MobSpawnMessage(mob));
                }
            }
        }

        private Entity CreateRandomMob(ZoneDescription zone, List<MobDescription> additionalMobs)
        {
            int level = zone.Level;
            var allMobs = m_descriptions.Concat(additionalMobs);
            var validMobs = allMobs.Where(m => m.CanSpawn(zone));
            if (zone.MobTypes.Count > 0)
            {
                validMobs = validMobs.Where(m => zone.MobTypes.Contains(m.Id));
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
