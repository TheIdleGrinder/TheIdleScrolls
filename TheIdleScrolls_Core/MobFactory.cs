using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core
{
    public class MobDescription
    {
        public string Name { get; set; } = "";
        public int MinLevel { get; set; } = 1;
        public int MaxLevel { get; set; } = Int32.MaxValue;
        public double HP { get; set; } = 1.0;
        public double Damage { get; set; } = 1.0;

        public MobDescription() { }

        public MobDescription(string name, int minLevel = 1, int maxLevel = Int32.MaxValue, double hP = 1.0, double damage = 1.0)
        {
            Name = name;
            MinLevel = minLevel;
            MaxLevel = maxLevel;
            HP = hP;
            Damage = damage;
        }
    }

    public class MobFactory
    {
        public static Entity MakeMob(MobDescription description, int level)
        {
            if (level < description.MinLevel || level > description.MaxLevel)
                throw new Exception($"Invalid level for {description.Name.Localize()}: {level} (valid: {description.MinLevel} - {description.MaxLevel})");
            var mob = new Entity();
            mob.AddComponent(new MobComponent());
            mob.AddComponent(new NameComponent(description.Name.Localize()));
            mob.AddComponent(new LevelComponent { Level = level });
            mob.AddComponent(new LifePoolComponent(CalculateHP(description, level)));
            mob.AddComponent(new XpGiverComponent { Amount = CalculateXpValue(description, level) });
            mob.AddComponent(new AccuracyComponent(Functions.CalculateMobAccuracy(level)));

            double damage = CalculateDamage(description, level);
            if (damage > 0.0)
                mob.AddComponent(new MobDamageComponent(damage));

            return mob;
        }

        static int CalculateHP(MobDescription description, int level)
        {
            /*double hp = Math.Pow(level, DifficultyScaling) * description.HP * BaseHpMultiplier;
            hp *= Math.Pow(1.1, Math.Pow(level, 0.75)); // Sub-exponential scaling for HP
            return (int)Math.Min(Math.Round(hp), 1_000_000_000); // Limit HP to 1B*/
            return Functions.CalculateMobHp(level, description.HP);
        }

        static int CalculateXpValue(MobDescription description, int level)
        {
            double dmgMulti = 0.5 + 0.5 * description.Damage;
            double hp = CalculateHP(description, level);
            double xp = Math.Ceiling(Math.Sqrt(level) * hp * dmgMulti / 10);
            return (int)Math.Min(xp, 2_500_000);
        }

        static double CalculateDamage(MobDescription description, int level)
        {
            return Functions.CalculateMobDamage(level, description.Damage);
        }
    }
}
