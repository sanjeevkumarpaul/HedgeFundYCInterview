﻿using EvaluateExpression.Helpers;

namespace EvaluateExpression.Constant
{
    internal static class EvalConstants
    {        
        internal const string ConditionMinusSymbol = "~MINUS~";
        internal const string MinusPattern = @"[-]\d";

        internal const string AirthmaticOperators = "^/*+-";
        internal const string DecimalSeparator = ".";

        /// <summary>
        /// Readonly string since it is calculated with some ticks.
        /// </summary>
        internal static readonly string OperandPrameterPattern = $"[*+/-]|{"^".Regesc()}";       
        internal static readonly string OperandParameterNestedBrackests = $@"({"(".Regesc()}([^{"(".Regesc()}{")".Regesc()}]+){")".Regesc()})";   //$@"({Regex.Escape("(")}(.*?){Regex.Escape(")")})";          

    }

    
}
