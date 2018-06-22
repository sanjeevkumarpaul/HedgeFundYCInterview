using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using APICalls.Configurations.Filters;
using APICalls.Constants;
using APICalls.Enum;
using Extensions;
 

namespace APICalls.Dependents
{
    internal class APIExpression
    {
        private string Operand;
        private APIConditionOperator Operator;
        private string Comparer;
        private List<object> Objects;

        private string Left;
        private string Right;
        
        internal APIExpression(List<object> objects, string operand, APIConditionOperator oper, string comparer)
        {
            Objects = objects;
            Operand = operand;
            Operator = oper;
            Comparer = comparer;
        }

        internal object GetVal()
        {
            if (IsOk())
            {
                SearchOperands(Operand, true);
                SearchOperands(Comparer, false);
                return CalculateExpression(Operator);
            }

            return "";
        }

        private bool CalculateExpression(APIConditionOperator oper)
        {
            Expression<Func<string, string, bool>> expression = null;

            switch (oper)
            {
                case APIConditionOperator.EQ: expression = (left, right) => left.Equals(right); break;
                case APIConditionOperator.NE: expression = (left, right) => !left.Equals(right); break;
                case APIConditionOperator.GT: expression = (left, right) => left.ToDouble() > right.ToDouble(); break;
                case APIConditionOperator.LT: expression = (left, right) => left.ToDouble() < right.ToDouble(); break;
                case APIConditionOperator.GE: expression = (left, right) => left.ToDouble() >= right.ToDouble(); break;
                case APIConditionOperator.LE: expression = (left, right) => left.ToDouble() <= right.ToDouble(); break;
            }

            if (expression != null)
            {
                var delegateOperation = expression.Compile();
                var res = delegateOperation(Left, Right);

                return res;
            }
            else return false;
        }

        private void SearchOperands(string operand, bool isleft = true)
        {
            operand = GetMathematicalEquation(operand.Replace(" ", "").Replace(APIConstants.ConditionMinusUserSymbol, APIConstants.ConditionMinusSymbol));

            var exprEquation = GetResultExpression ( GetMathematicalExpression(operand) );
                        
            if (isleft) Left = exprEquation.ToString(); else Right = exprEquation.ToString();
        }

        private double GetResultExpression(Expression mathematicEquation)
        {
            ParameterExpression paramRes = Expression.Parameter(typeof(double), "result");
            BlockExpression block = Expression.Block
            (
                new[] { paramRes }, //crating local var.
                Expression.Assign(paramRes, mathematicEquation)
            );
            double exprResult = Expression.Lambda<Func<double>>(block).Compile()();

            return exprResult;
        }

        private Expression GetMathematicalExpression(string equation)
        {
            return GenerateEvalutionExpression(SearchAndCalcualteBracketGroupEquations(equation));

            //Local function to recurssively get the Brackets done.
            string SearchAndCalcualteBracketGroupEquations(string bracketOperand)
            {
                var matches = Regex.Matches(bracketOperand, APIConstants.OperandParameterNestedBrackests).Cast<Match>();
                matches.All(m =>
                {
                    var _val = m.Groups[0].Value.Substring(1);
                    bracketOperand = bracketOperand.Replace($"({_val}", SearchAndCalcualteBracketGroupEquations(_val)); //Recursive Call

                    return true;
                });
                return CalculateEquation(bracketOperand); //Returning Calculated values for the outer bracket equation.
            }

            //Local function.
            string CalculateEquation(string operandEquation)
            {
                if (operandEquation.EndsWith(")")) operandEquation = operandEquation.Remove(operandEquation.Length - 1);
                var expression = GenerateEvalutionExpression(operandEquation);
                var result = GetResultExpression(expression).ToString();
                return result;
            }

            //Local function once again to calcualte the values of each group.
            Expression GenerateEvalutionExpression(string groupOperand)
            {
                var _equation = groupOperand;
                var _operator = "";
                var _prevOperator = "";
                Expression _exprEquation = null;

                Regex.Matches(groupOperand, APIConstants.OperandPrameterPattern).Cast<Match>().All(m =>
                {
                    _operator = m.Groups[0].Value;
                    var _left = _equation.Substring(0, _equation.IndexOf(_operator));
                    _equation = _equation.Substring(_left.Length + 1); //+1 is to avoid the operator just received.
                    _exprEquation = AddOperationExpression(_prevOperator, _exprEquation, ParseDoubleForEquation(_left));
                    _prevOperator = _operator;
                    
                    return true;
                });

                _exprEquation = AddOperationExpression(_operator, _exprEquation, ParseDoubleForEquation(_equation));
                return _exprEquation;
            }                                   
        }

        private double ParseDoubleForEquation(string value)
        {
            var minusflag = value.StartsWith(APIConstants.ConditionMinusSymbol);
            if (minusflag) value = value.Replace(APIConstants.ConditionMinusSymbol, "");

            return (value.ToDouble()) * ( minusflag ? -1 : 1 );
        }

        private Expression AddOperationExpression(string operation, Expression equation, double value)
        {
            if (equation == null) return Expression.Constant(value, typeof(double));

            switch (operation)
            {
                case "+": return Expression.Add(equation, Expression.Constant(value, typeof(double))); 
                case "-": return Expression.Subtract(equation, Expression.Constant(value, typeof(double))); 
                case "*": return Expression.Multiply(equation, Expression.Constant(value, typeof(double))); 
                case "/": return Expression.Divide(equation, Expression.Constant(value, typeof(double)));
                case "^": return Expression.Power(equation, Expression.Constant(value, typeof(double)));
            }

            return equation;
        }

        private string GetMathematicalEquation(string operand)
        {
            foreach (var oper in GenerateEquation(operand))
            {
                if (!oper.Object.Empty() && oper.Properties.Exists(p => !p.Empty()))
                {
                    var obj = Objects.Find(o => o.GetType().Name.Equals(oper.Object, StringComparison.CurrentCultureIgnoreCase));
                    foreach (var prop in oper.Properties)
                    {
                        var itenary = $"{{{oper.Object}.{prop}}}";
                        var res = obj?.GetVal(prop);

                        operand = operand.Replace(itenary, $"{res}");
                    }
                }
            }

            return operand;
        }
        
        private IEnumerable<APIFilterOperand> GenerateEquation(string operand)
        {
            List<APIFilterOperand> operands = new List<APIFilterOperand>();
                       
            if (Regex.IsMatch(operand, APIConstants.ParamterPatter))
            {
                Regex.Matches(operand, APIConstants.ParamterPatter).Cast<Match>().All(m =>
                {
                    var _type = m.Groups[1].Value.SplitEx('.');
                    if (!APIFilterOperand.FindAndReplace(operands, _type[0], _type[1]))
                    {
                        operands.Add(new APIFilterOperand
                        {
                            Object = _type[0],
                            Properties = new List<string> { _type[1] }
                        });
                    }
                    return true;
                });
            }
            else operands.Add( new APIFilterOperand { Constant = operand });

            return operands;
        }

        private bool IsOk()
        {            
            var _typeMatch = Regex.Match(Operand, APIConstants.ParamterPatter);
            if (_typeMatch == null) return false; //Invalid condition declaration;
            
            return true;
        }
    }

    
}
