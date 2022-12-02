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
        public string Name { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public double HP { get; set; }

        public MobDescription()
        {
            Name = "";
            MinLevel = 0;
            MaxLevel = 100;
            HP = 1.0;
        }
    }

    public class MobFactory
    {
        List<MobDescription> m_mobs = new();

        public Entity MakeMob(MobDescription description, int level)
        {
            if (level < description.MinLevel || level > description.MaxLevel)
                throw new Exception($"Invalid level for {description.Name}: {level} (valid: {description.MinLevel} - {description.MaxLevel})");
            var mob = new Entity();
            mob.AddComponent(new MobComponent());
            mob.AddComponent(new NameComponent(description.Name));
            mob.AddComponent(new LevelComponent { Level = level });
            mob.AddComponent(new LifePoolComponent((int)(level * description.HP * 10)));
            mob.AddComponent(new XpGiverComponent { Amount = CalculateXpValue(description, level) });
            return mob;
        }

        int CalculateXpValue(MobDescription description, int level)
        {
            return (int)Math.Round(
                5.0 * Math.Pow(level, 2) * description.HP
            );
        }
    }
}
