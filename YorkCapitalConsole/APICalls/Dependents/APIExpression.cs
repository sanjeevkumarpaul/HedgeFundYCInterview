using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using APICalls.Configurations.Filters;
using APICalls.Constants;
using APICalls.Enum;
using EvaluateExpression;
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
            operand = GetMathematicalEquation(operand);

            var exprEquation = Evaluate.Math<double>(operand);
                        
            if (isleft) Left = exprEquation.ToString(); else Right = exprEquation.ToString();        
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
