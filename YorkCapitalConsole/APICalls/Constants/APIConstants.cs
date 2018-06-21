using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        internal static readonly string OperandPrameterPattern = $"[*+/-]|{System.Text.RegularExpressions.Regex.Escape("^")}";
    }
}
