using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EvaluateExpression.Helpers
{
    internal static class EvalResults
    {
        internal static T Evaluate<T>(Expression mathematicEquation)
        {
            ParameterExpression paramRes = Expression.Parameter(typeof(T), "result");
            BlockExpression block = Expression.Block
            (
                new[] { paramRes }, //crating local var.
                Expression.Assign(paramRes, mathematicEquation)
            );
            T exprResult = Expression.Lambda<Func<T>>(block).Compile()();

            return exprResult;
        }

    }
}

