using EvaluateExpression.Helpers;

namespace EvaluateExpression
{
    public static class Evaluate
    {
        public static T Math<T>(string equation)
        {
            return new EvalMath().Calculate<T>(equation);
        }
    }
}
