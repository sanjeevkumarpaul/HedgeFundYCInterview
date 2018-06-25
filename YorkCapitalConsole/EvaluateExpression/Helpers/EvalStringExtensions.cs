using System;
using EvaluateExpression.Constant;
using Extensions;

namespace EvaluateExpression.Helpers
{
    internal static class EvalStringExtensions
    {

        internal static T ParseNumber<T>(this string value)
        {
            var res = Convert.ChangeType(ParseDoubleForEquation(value), typeof(T));

            return (T)res;
        }


        private static double ParseDoubleForEquation(string value)
        {
            var minusflag = value.StartsWith(EvalConstants.ConditionMinusSymbol);
            if (minusflag) value = value.Replace(EvalConstants.ConditionMinusSymbol, "");

            return (value.ToDouble()) * (minusflag ? -1 : 1);
        }                
    }
}
