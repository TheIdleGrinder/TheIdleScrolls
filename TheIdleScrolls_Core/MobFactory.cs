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
    }

    public class MobFactory
    {
        static double BaseHpMultiplier = 10.0;
        static double DifficultyScaling = 1.0;

        List<MobDescription> m_mobs = new();

        public Entity MakeMob(MobDescription description, int level)
        {
            if (level < description.MinLevel || level > description.MaxLevel)
                throw new Exception($"Invalid level for {description.Name}: {level} (valid: {description.MinLevel} - {description.MaxLevel})");
            var mob = new Entity();
            mob.AddComponent(new MobComponent());
            mob.AddComponent(new NameComponent(description.Name));
            mob.AddComponent(new LevelComponent { Level = level });
            mob.AddComponent(new LifePoolComponent(CalculateHP(description, level)));
            mob.AddComponent(new XpGiverComponent { Amount = CalculateXpValue(description, level) });

            if (description.Damage > 0.0)
            {
                mob.AddComponent(new MobDamageComponent(description.Damage));
            }

            return mob;
        }

        int CalculateHP(MobDescription description, int level)
        {
            double hp = Math.Pow(level, DifficultyScaling) * description.HP * BaseHpMultiplier;
            //hp *= Math.Pow(1.04, Math.Min(level, 100))      // HP increase exponentially by 5% per level before level 100,
            //    * Math.Pow(1.02, Math.Clamp(level - 100, 0, 100))   // 3% for every level between 100 and 200
            //    * Math.Pow(1.01, Math.Max(level - 200, 100));       // and 1% for each level above 200
            hp *= Math.Pow(1.1, Math.Pow(level, 0.75)); // Sub-exponential scaling for HP
            return (int)Math.Min(hp, Math.Pow(1000, 3)); // Limit HP to 1B
        }

        int CalculateXpValue(MobDescription description, int level)
        {
            double dmgMulti = 0.5 + 0.5 * description.Damage;
            return (int)Math.Round(
                5.0 * Math.Pow(level, 2) * description.HP * dmgMulti
            );
        }
    }
}
