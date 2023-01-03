using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.Achievements
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
                return (double)(target.GetComponent<LevelComponent>()?.Level ?? 0);
            }
            else if (m_fieldId == "Kills")
            {
                return (double)(target.GetComponent<PlayerProgressComponent>()?.Data.Kills ?? 0);
            }
            else if (m_fieldId == "Losses")
            {
                return (double)(target.GetComponent<PlayerProgressComponent>()?.Data.Losses ?? 0);
            }
            else if (m_fieldId == "Playtime")
            {
                return (double)(target.GetComponent<PlayerProgressComponent>()?.Data.Playtime ?? 0.0);
            }
            else if (m_fieldId.StartsWith("dng:"))
            {
                var dungeon = m_fieldId[4..];
                return target.GetComponent<PlayerProgressComponent>()?.Data.DungeonTimes.GetValueOrDefault(dungeon, -1.0) ?? -1.0;
            }
            else if (m_fieldId.StartsWith("abl:"))
            {
                var ability = m_fieldId[4..];
                return (double)(target.GetComponent<AbilitiesComponent>()?.GetAbility(ability)?.Level ?? -1.0);
            }
            else
            {
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
            this.m_comparator = comparator;
            this.m_left = left;
            this.m_right = right;
        }

        public double Evaluate(Entity target, World world)
        {
            double left = m_left.Evaluate(target, world);
            double right = m_right.Evaluate(target, world);
            bool result = m_comparator switch
            {
                Comparator.LT  => left < right,
                Comparator.LEQ => left <= right,
                Comparator.EQ  => left == right,
                Comparator.NEQ => left != right,
                Comparator.GEQ => left >= right,
                Comparator.GT  => left > right,
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
