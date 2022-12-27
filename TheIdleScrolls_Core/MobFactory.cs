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
        static double DifficultyScaling = 1.2;

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
            return (int)(Math.Pow(level, DifficultyScaling) * description.HP * BaseHpMultiplier);
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
