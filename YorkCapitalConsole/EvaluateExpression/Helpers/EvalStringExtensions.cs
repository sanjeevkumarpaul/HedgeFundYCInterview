using System;
using System.Linq;
using System.Text.RegularExpressions;
using EvaluateExpression.Constant;
using Extensions;

namespace EvaluateExpression.Helpers
{
    internal static class EvalStringExtensions
    {

        internal static string ReplaceMinus(this string value)
        {
            foreach (var match in Regex.Matches(value, EvalConstants.MinusPattern).Cast<Match>())
            {
                var _match = match.Groups[0].Value;
                value = value.Replace(_match, $"{EvalConstants.ConditionMinusSymbol}{_match.Substring(1)}");
            }

            return value;
        }

        internal static T ParseNumber<T>(this string value)
        {
            var res = Convert.ChangeType(ParseDoubleForEquation(value), typeof(T));

            return (T)res;
        }

        internal static string Regesc(this string value)
        {
            return Regex.Escape(value.ToString());
        }

        private static double ParseDoubleForEquation(string value)
        {
            var minusflag = value.StartsWith(EvalConstants.ConditionMinusSymbol);
            if (minusflag) value = value.Replace(EvalConstants.ConditionMinusSymbol, "");

            return (value.ToDouble()) * (minusflag ? -1 : 1);
        }                
    }
}
