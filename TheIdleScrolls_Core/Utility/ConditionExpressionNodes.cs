using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Utility
{
    public interface IConditionExpressionNode
    {
        public double Evaluate(Entity target, World world);
    }

    public class NumericNode : IConditionExpressionNode
    {
        private double m_value;

        public NumericNode(double value)
        {
            m_value = value;
        }

        public double Evaluate(Entity target, World world)
        {
            return m_value;
        }
    }

    public class VariableNode : IConditionExpressionNode
    {
        private string m_fieldId;

        public VariableNode(string fieldId)
        {
            m_fieldId = fieldId;
        }

        public double Evaluate(Entity target, World world)
        {
            if (m_fieldId == "Level")
            {
                return target.GetComponent<LevelComponent>()?.Level ?? 0;
            }
            else if (m_fieldId == "Kills")
            {
                return target.GetComponent<PlayerProgressComponent>()?.Data.Kills ?? 0;
            }
            else if (m_fieldId == "Losses")
            {
                return target.GetComponent<PlayerProgressComponent>()?.Data.Losses ?? 0;
            }
            else if (m_fieldId == "Wilderness")
            {
                return target.GetComponent<PlayerProgressComponent>()?.Data.HighestWildernessKill ?? 0;
            }
            else if (m_fieldId == "Playtime")
            {
                return (double)(target.GetComponent<PlayerProgressComponent>()?.Data.Playtime ?? 0.0);
            }
            else if (m_fieldId == "MaxCoins")
            {
                return (double)(target.GetComponent<PlayerProgressComponent>()?.Data.MaxCoins ?? 0.0);
            }
            else if (m_fieldId == "TotalCoins")
            {
                return (double)(target.GetComponent<PlayerProgressComponent>()?.Data.TotalCoins ?? 0.0);
            }
            else if (m_fieldId == "BestRefine")
            {
                return (double)(target.GetComponent<PlayerProgressComponent>()?.Data.BestRefine ?? 0.0);
            }
            else if (m_fieldId == "BestG0Craft")
            {
                return (double)(target.GetComponent<PlayerProgressComponent>()?.Data.BestG0Refine ?? 0.0);
            }
            else if (m_fieldId.StartsWith("dng_open:"))
            {
                var dungeon = m_fieldId.Split(':')[1];
                return (target.GetComponent<TravellerComponent>()?.AvailableDungeons?.Keys?.Contains(dungeon!) ?? false) ? 1.0 : 0.0;
            }
            else if (m_fieldId.StartsWith("dng:"))
            {
                var dungeon = m_fieldId[4..];
                int level = 0;
                if (dungeon.Contains('@'))
                {
                    dungeon = dungeon.Split('@')[0];
                    level = int.Parse(dungeon.Split('@')[1]);
                }
                var timesForDungeon = target.GetComponent<PlayerProgressComponent>()?.Data?.DungeonTimes?.GetValueOrDefault(dungeon, []);
                if (timesForDungeon == null)
                {
                    return -1.0;
                }

                return (level > 0) 
                    ? timesForDungeon.GetValueOrDefault(level, -1.0) 
                    : timesForDungeon.Values.FirstOrDefault(-1.0);
            }
            else if (m_fieldId.StartsWith("abl:"))
            {
                var ability = m_fieldId[4..];
                return (double)(target.GetComponent<AbilitiesComponent>()?.GetAbility(ability)?.Level ?? -1.0);
            }
            else
            {
                // CornerCut(?): If no prefix or id is provided, check if m_fieldId is the ID of another achievement
                var achieveComp = world.GlobalEntity.GetComponent<AchievementsComponent>();
                if (achieveComp != null)
                {
                    var achievement = achieveComp.Achievements.FirstOrDefault(a => a.Id == m_fieldId);
                    if (achievement != null)
                    {
                        return (achievement.Status == Achievements.AchievementStatus.Awarded) ? 1.0 : 0.0;
                    }
                }
                throw new ArgumentException($"Invalid field condition: {m_fieldId}");
            }
        }
    }

    public class ComparisonNode : IConditionExpressionNode
    {
        public enum Comparator { LT, LEQ, EQ, NEQ, GEQ, GT }

        private Comparator m_comparator;

        private IConditionExpressionNode m_left;

        private IConditionExpressionNode m_right;

        public ComparisonNode(Comparator comparator, IConditionExpressionNode left, IConditionExpressionNode right)
        {
            m_comparator = comparator;
            m_left = left;
            m_right = right;
        }

        public double Evaluate(Entity target, World world)
        {
            double left = m_left.Evaluate(target, world);
            double right = m_right.Evaluate(target, world);
            bool result = m_comparator switch
            {
                Comparator.LT => left < right,
                Comparator.LEQ => left <= right,
                Comparator.EQ => left == right,
                Comparator.NEQ => left != right,
                Comparator.GEQ => left >= right,
                Comparator.GT => left > right,
                _ => left == right
            };
            return result ? 1.0 : 0.0;
        }
    }

    public class AndNode : IConditionExpressionNode
    {
        private List<IConditionExpressionNode> m_nodes;

        public AndNode(List<IConditionExpressionNode> nodes)
        {
            m_nodes = nodes;
        }

        public double Evaluate(Entity target, World world)
        {
            var results = m_nodes.Select(n => n.Evaluate(target, world));
            return Math.Max(results.Select(d => Math.Min(d, 1.0)).Average(), 0.0);
        }
    }

    public class OrNode : IConditionExpressionNode
    {
        private List<IConditionExpressionNode> m_nodes;

        public OrNode(List<IConditionExpressionNode> nodes)
        {
            m_nodes = nodes;
        }

        public double Evaluate(Entity target, World world)
        {
            var results = m_nodes.Select(n => n.Evaluate(target, world));
            return Math.Max(Math.Min(results.Max(), 1.0), 0.0);
        }
    }
}
