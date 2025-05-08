using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core
{
    public class MobDescription
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public double HP { get; set; } = 1.0;
        public double Damage { get; set; } = 1.0;
        public Func<ZoneDescription, bool> CanSpawn { get; set; } = (zone) => true;

        public MobDescription() { }

        public MobDescription(string id, string name, Func<ZoneDescription, bool> spawnCondition, double hP = 1.0, double damage = 1.0)
        {
            Id = id;
            Name = name;
            HP = hP;
            Damage = damage;
            CanSpawn = spawnCondition;
        }

        public MobDescription(string id, string name, double hP = 1.0, double damage = 1.0)
        {
            Id = id;
            Name = name;
            HP = hP;
            Damage = damage;
        }
    }

    public class MobFactory
    {
        public static Entity MakeMob(MobDescription description, int level)
        {
            var mob = new Entity();
            mob.AddComponent(new MobComponent(description.Id));
            mob.AddComponent(new NameComponent(description.Name));
            mob.AddComponent(new LevelComponent { Level = level });
            mob.AddComponent(new LifePoolComponent(CalculateHP(description, level)));
            mob.AddComponent(new XpGiverComponent { Amount = CalculateXpValue(description, level) });
            mob.AddComponent(new AccuracyComponent(Functions.CalculateMobAccuracy(level)));

            double damage = CalculateDamage(description, level);
            if (damage > 0.0)
                mob.AddComponent(new MobDamageComponent(damage));

            return mob;
        }

        public static int CalculateHP(MobDescription description, int level)
        {
            return Functions.CalculateMobHp(level, description.HP);
        }

        public static int CalculateXpValue(MobDescription description, int level)
        {
            double dmgMulti = 0.5 + 0.5 * description.Damage;
            double hp = CalculateHP(description, level);
            double levelScaling = Math.Sqrt(level) * (1.0 + level / 100.0);
            double xp = Math.Ceiling(levelScaling * hp * dmgMulti / 10);
            return (int)Math.Min(xp, 2_500_000);
        }

        public static double CalculateDamage(MobDescription description, int level)
        {
            _ = level; // unused
            return description.Damage;
        }
    }
}
