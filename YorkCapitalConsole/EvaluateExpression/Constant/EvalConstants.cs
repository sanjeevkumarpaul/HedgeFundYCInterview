using System.Text.RegularExpressions;
using EvaluateExpression.Helpers;

namespace EvaluateExpression.Constant
{
    internal static class EvalConstants
    {        
        internal const string ConditionMinusSymbol = "~MINUS~";
        internal const string MinusPattern = @"[-]\d";

        /// <summary>
        /// Readonly string since it is calculated with some ticks.
        /// </summary>
        internal static readonly string OperandPrameterPattern = $"[*+/-]|{"^".Regesc()}";
        //internal static readonly string OperandParamterBrackets = $"(?<={Regex.Escape("(")})[^{Regex.Escape(")")}]*(?={Regex.Escape(")")})";        
        internal static readonly string OperandParameterNestedBrackests = $@"({"(".Regesc()}([^{"(".Regesc()}{")".Regesc()}]+){")".Regesc()})";   //$@"({Regex.Escape("(")}(.*?){Regex.Escape(")")})";          

    }

    
}
