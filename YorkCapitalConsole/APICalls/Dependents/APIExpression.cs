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
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="objects">Objects to get equation place holder values.</param>
        /// <param name="operand">Template mathematical equation for left side operand</param>
        /// <param name="oper">Comparison operation as of now.</param>
        /// <param name="comparer">Template mathematical equation for right side operand</param>
        internal APIExpression(List<object> objects, string operand, APIConditionOperator oper, string comparer)
        {
            Objects = objects;
            Operand = operand;
            Operator = oper;
            Comparer = comparer;
        }

        /// <summary>
        /// Main method to be called from outer program to get outfome of Equation.
        /// Here we cover for Comparison.
        /// </summary>
        /// <returns></returns>
        internal object GetVal()
        {
            if (IsOk())
            {
                SearchOperands(Operand, true);
                SearchOperands(Comparer, false);
                return CompareOperands(Operator);
            }

            return "";
        }

        /// <summary>
        /// Compiles equations between left and right and based on Operation specified, determines the mathematical binary expression and sends the output back to caller.
        /// </summary>
        /// <param name="oper">Comparison Operation (APIConditionOperator)</param>
        /// <returns></returns>
        private bool CompareOperands(APIConditionOperator oper)
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

        /// <summary>
        /// Searches through the template WHERE/Condition to identify equations and get the results of the calcualation and 
        /// assign the same result to either LEFT or RIGHT equation which is then used to get calcuation at caller.
        /// This is called two times, once for each side (left/right) operands. 
        /// At templates the XML Atrributes are - Operand and Value from <Condition> template.
        /// </summary>
        /// <param name="operand">Equation as template</param>
        /// <param name="isleft">Boolean to set it for Left or Right Operand</param>
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

        /// <summary>
        /// As we pass on the Mathematical equation with all kinds of precendence from brackets,
        /// this method will calcualte all values inside all nested bracket and form an single liner non-nested bracket equation and send it to caller.
        /// </summary>
        /// <param name="equation">strign with brackets or no brackets</param>
        /// <returns>Expression with simplified mathematical equation.</returns>
        private Expression GetMathematicalExpression(string equation)
        {
            return GenerateEvalutionExpression(SearchAndCalcualteBracketGroupEquations(equation));

            //Local function to Recurssively get all equations from Brackets and process them to return single line equation.
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

            //Local function to get Expression and calculation based on the parsed equation string passed on by caller.
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

                //Matches all Operators(*,+,/,-,^) and Reverses them to take Right to Left advantage in mathematical calculation.
                Regex.Matches(groupOperand, APIConstants.OperandPrameterPattern).Cast<Match>().Reverse().All(m =>
                {
                    _operator = m.Groups[0].Value;                   
                    var _right = _equation.Substring(_equation.LastIndexOf(_operator)+1);
                    _equation = _equation.Substring(0, _equation.IndexOf(_operator));
                    _exprEquation = AddOperationExpression(_prevOperator, _exprEquation, ParseDoubleForEquation(_right));
                    _prevOperator = _operator;

                    return true;
                });

                _exprEquation = AddOperationExpression(_operator, _exprEquation, ParseDoubleForEquation(_equation)); //for the last one out of loop.
                return _exprEquation;
            }                                   
        }

        /// <summary>
        /// Parses a string value passed from Template and tries to convert it into double.
        /// From template there comes sometiems with a MINUS(-) notation, decodes it and if found to be all right multiples with -1
        /// </summary>
        /// <param name="value">String value represting a number from Template</param>
        /// <returns>Double value</returns>
        private double ParseDoubleForEquation(string value)
        {
            var minusflag = value.StartsWith(APIConstants.ConditionMinusSymbol);
            if (minusflag) value = value.Replace(APIConstants.ConditionMinusSymbol, "");

            return (value.ToDouble()) * ( minusflag ? -1 : 1 );
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
        private Expression AddOperationExpression(string operation, Expression equation, double value, bool rightToLeft = true)
        {
            if (equation != null)
            {
                var secondExpression = Expression.Constant(value, typeof(double));

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
            else equation = Expression.Constant(value, typeof(double));

            return equation;

            //Local function to check on hte switch.
            Expression SwitchExpressions(Expression left, Expression right)
            {
                return rightToLeft ? right : left;
            }
        }

        /// <summary>
        /// Creates a mathematicl equation with numbers from the Phrase at the WHERE Template.
        /// </summary>
        /// <param name="operand">WHERE -> Conidtional Template.</param>
        /// <returns>string representing an proper mathematical calculation. (right now its only for numbers)</returns>
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
        
        /// <summary>
        /// Creates APIFilterOperand object figuring out all required object values from WHERE/Condition template.
        /// </summary>
        /// <param name="operand">String Operation.</param>
        /// <returns></returns>
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

        /// <summary>
        /// To check if the Left Operand is good enough with parameter(s).
        /// </summary>
        /// <returns>Boolean</returns>
        private bool IsOk()
        {            
            var _typeMatch = Regex.Match(Operand, APIConstants.ParamterPatter);
            if (_typeMatch == null) return false; //Invalid condition declaration;
            
            return true;
        }
    }

    
}
