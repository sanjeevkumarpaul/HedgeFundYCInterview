﻿using EvaluateExpression.Constant;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace EvaluateExpression.Helpers
{
    internal sealed class EvalMath
    {
        /// <summary>
        /// Main method to Calcuate Mathematical Results.
        /// </summary>
        /// <param name="equation"></param>
        /// <returns></returns>
        internal T Calculate<T>(string equation)
        {
            equation = equation.Replace(" ", "");//.ReplaceMinus();

            return EvalResults.Evaluate<T>( GetMathematicalExpression<T>(equation) );
        }
        
        
        /// <summary>
        /// As we pass on the Mathematical equation with all kinds of precendence from brackets,
        /// this method will calcualte all values inside all nested bracket and form an single liner non-nested bracket equation and send it to caller.
        /// </summary>
        /// <param name="equation">strign with brackets or no brackets</param>
        /// <returns>Expression with simplified mathematical equation.</returns>
        private Expression GetMathematicalExpression<T>(string equation)
        {
            return GenerateEvalutionExpression(SearchAndCalcualteBracketGroupEquations(equation));

            //Local function to Recurssively get all equations from Brackets and process them to return single line equation.
            string SearchAndCalcualteBracketGroupEquations(string bracketOperand)
            {
                var matches = Regex.Matches(bracketOperand, EvalConstants.OperandParameterNestedBrackests).Cast<Match>();
                matches.All(m =>
                {
                    var _val = m.Groups[0].Value.Substring(1);
                    bracketOperand = bracketOperand.Replace($"({_val}", SearchAndCalcualteBracketGroupEquations(_val)); //Recursive Call

                    return true;
                });
                return CalculateEquation(bracketOperand); //Returning Calculated values for the outer bracket equation.
            }

            //Local function to get Expression and calculation based on the parsed equation string passed on by caller.
            string CalculateEquation(string operandEquation)
            {
                if (operandEquation.EndsWith(")")) operandEquation = operandEquation.Remove(operandEquation.Length - 1);
                var expression = GenerateEvalutionExpression(operandEquation);
                var result = EvalResults.Evaluate<T>(expression).ToString();
                return result; //.ReplaceMinus();
            }

            //Local function once again to calcualte the values of each group.
            Expression GenerateEvalutionExpression(string groupOperand)
            {
                var _equation = groupOperand;
                var _operator = "";
                var _prevOperator = "";
                Expression _exprEquation = null;

                if (_equation.StartsWith("-")) _equation = $"0{_equation}";

                //Matches all Operators(*,+,/,-,^) and Reverses them to take Right to Left advantage in mathematical calculation.
                Regex.Matches(groupOperand, EvalConstants.OperandPrameterPattern).Cast<Match>().Reverse().All(m =>
                {
                    _operator = m.Groups[0].Value;
                    var _right = _equation.Substring(_equation.LastIndexOf(_operator) + 1);
                    _equation = _equation.Substring(0, _equation.LastIndexOf(_operator));
                    _exprEquation = AddOperationExpression(_prevOperator, _exprEquation, _right.ParseNumber<T>());
                    _prevOperator = _operator;

                    return true;
                });

                _exprEquation = AddOperationExpression(_operator, _exprEquation, _equation.ParseNumber<T>()); //for the last one out of loop.
                return _exprEquation;
            }
        }
        
        /// <summary>
        /// Expression generation towards mathematical equation based on operation string passed.
        /// Creates equation and appends to equation Expression passed
        /// as it is mathematical, Value deals with numbers only.        
        /// </summary>
        /// <param name="operation">Mathematical operation passed as string</param>
        /// <param name="equation">Expression which needs Generated Expression to be appened too</param>
        /// <param name="value">double value represting number</param>
        /// <param name="rightToLeft">As to mention, calcualtion to proceed from left to right or right to left (Precendence)</param>
        /// <returns>Expression with Numbers</returns>
        private Expression AddOperationExpression<T>(string operation, Expression equation, T value, bool rightToLeft = true)
        {
            if (equation != null)
            {
                var secondExpression = Expression.Constant(value, typeof(T));

                var _left = SwitchExpressions(equation, secondExpression);      //Gets the appropriate Left or Right
                var _right = SwitchExpressions(secondExpression, equation);    //Here too.

                switch (operation)
                {
                    case "+": return Expression.Add(_left, _right);
                    case "-": return Expression.Subtract(_left, _right);
                    case "*": return Expression.Multiply(_left, _right);
                    case "/": return Expression.Divide(_left, _right);
                    case "^": return Expression.Power(_left, _right);
                }
            }
            else equation = Expression.Constant(value, typeof(T));

            return equation;

            //Local function to check on hte switch.
            Expression SwitchExpressions(Expression left, Expression right)
            {
                return rightToLeft ? right : left;
            }
        }
    }
}
