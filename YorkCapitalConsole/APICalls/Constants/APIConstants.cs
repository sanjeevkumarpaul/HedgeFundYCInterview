using System.Text.RegularExpressions;

namespace APICalls.Constants
{
    internal static class APIConstants
    {
        internal const string ParamterPatter = "{(.*)}";
        internal const string ConditionMinusSymbol = "~MINUS~";
        internal const string ConditionMinusUserSymbol = "(-)";

        /// <summary>
        /// Readonly string since it is calculated with some ticks.
        /// </summary>
        internal static readonly string OperandPrameterPattern = $"[*+/-]|{Regex.Escape("^")}";
        //internal static readonly string OperandParamterBrackets = $"(?<={Regex.Escape("(")})[^{Regex.Escape(")")}]*(?={Regex.Escape(")")})";
        internal static readonly string OperandParameterNestedBrackests = $@"({Regex.Escape("(")}(.*?){Regex.Escape(")")})";
    }
}
