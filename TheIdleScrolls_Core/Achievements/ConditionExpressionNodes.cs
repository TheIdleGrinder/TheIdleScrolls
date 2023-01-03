using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            throw new NotImplementedException($"Can't handle condition: {m_fieldId}");
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
            return results.Count(d => d >= 1.0) / (double)results.Count();
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
