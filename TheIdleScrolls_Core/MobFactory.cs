using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Skills;
using TheIdleScrolls_Core.StatusEffects;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core
{
    public class MobDescription
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public double HP { get; set; } = 1.0;
        public double Damage { get; set; } = 1.0;
        public Func<ZoneDescription, bool> CanSpawn { get; set; } = (zone) => true;
        public List<ActiveSkillDefinition> ActiveSkills { get; set; } = [];
        public List<Perk> Perks { get; set; } = [];

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
            {
                AttackComponent attackComp = new();
                attackComp.AddAttackVector(new DamageCluster(Definitions.DamageType.Physical, damage), 1.0);
                mob.AddComponent(attackComp);
                var skillComp = new ActiveSkillComponent();
                var defaultAttack = new ActiveSkill(Skills.Skills.DefaultAttack.Skill);
                defaultAttack.SetupForUser(mob);
                skillComp.Add(defaultAttack);
                mob.AddComponent(skillComp);
            }

            if (description.ActiveSkills.Count > 0)
            {
                var skillComp = mob.GetComponent<ActiveSkillComponent>();
                if (skillComp is null)
                {
                    skillComp = new ActiveSkillComponent();
                    mob.AddComponent(skillComp);
                }
                foreach (var skill in description.ActiveSkills)
                {
                    skillComp.Add(new(skill));
                }
            }

            if (description.Perks.Count > 0)
            {
                var perkComp = new PerksComponent();
                foreach (var effect in description.Perks)
                {
                    perkComp.AddPerk(effect);
                }
                mob.AddComponent(perkComp);
                mob.AddComponent(new ModifierComponent());
            }

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
            return description.Damage * Functions.CalculateMobDamage(level, description.Damage);
        }
    }
}
