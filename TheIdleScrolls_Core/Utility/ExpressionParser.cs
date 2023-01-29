using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Utility
{
    public class ExpressionParser
    {
        public static IConditionExpressionNode Parse(string conditionString)
        {
            // split at || first => && has higher precedence than || 
            if (conditionString.Contains("||"))
            {
                var fields = conditionString.Split("||").Select(s => s.Trim());
                return new OrNode(fields.Select(f => Parse(f)).ToList());
            }
            else if (conditionString.Contains("&&"))
            {
                var fields = conditionString.Split("&&").Select(s => s.Trim());
                return new AndNode(fields.Select(f => Parse(f)).ToList());
            }
            else
            {
                // Check if string contains comparison => split further
                string[] ops = { "<=", "<", "==", "!=", ">=", ">" };
                foreach (var op in ops)
                {
                    if (conditionString.Contains(op))
                    {
                        var fields = conditionString.Split(op).Select(s => s.Trim()).ToList();
                        if (fields.Count() != 2)
                        {
                            throw new ArgumentException($"Invalid condition expression: {conditionString}");
                        }
                        var left = Parse(fields[0].Trim());
                        var right = Parse(fields[1].Trim());
                        var eOp = ParseComparator(op);

                        return new ComparisonNode(eOp, left, right);
                    }
                }

                // Empty condition should always be fulfilled
                if (conditionString == String.Empty)
                {
                    return new NumericNode(1.0);
                }

                // Check if the remaining string is a numeric value
                if (double.TryParse(conditionString.Trim(), out double value))
                {
                    return new NumericNode(value);
                }

                // Interpret string as a variable description
                return new VariableNode(conditionString.Trim());
            }
        }

        private static ComparisonNode.Comparator ParseComparator(string comparatorString)
        {
            return comparatorString switch
            {
                "<=" => ComparisonNode.Comparator.LEQ,
                "<" => ComparisonNode.Comparator.LT,
                "==" => ComparisonNode.Comparator.EQ,
                "!=" => ComparisonNode.Comparator.NEQ,
                ">=" => ComparisonNode.Comparator.GEQ,
                ">" => ComparisonNode.Comparator.GT,
                _ => throw new ArgumentException($"Invalid comparator: {comparatorString}")
            };
        }
    }
}
