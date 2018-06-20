using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
                return Operation(Operator);
            }

            return "";
        }

        private void SearchOperands(string operand, bool isleft = true)
        {
            operand = operand.Replace(" ", "").Replace("(-)", "~MINUS~");

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

            var exprEquation = GetResultExpression ( GetMathematicExpression(operand) );
                        
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

        private Expression GetMathematicExpression(string operand)
        {
            var equation = operand;
            var _operator = "";
            Expression exprEquation = null;
            Regex.Matches(operand, "[*+/-]").Cast<Match>().All(m =>
            {
                _operator = m.Groups[0].Value;
                var _left = equation.Substring(0, equation.IndexOf(_operator));
                equation = equation.Substring(_left.Length + 1); //+1 is to avoid the operator just received.
                exprEquation = AddOperationExpression(_operator, exprEquation, _left.ToDouble());

                return true;
            });

            exprEquation = AddOperationExpression(_operator, exprEquation, equation.ToDouble());

            return exprEquation;
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
            }

            return equation;
        }


        private bool Operation(APIConditionOperator oper)
        {
            Expression<Func<string, string, bool>> expression = (left, right) => left.Equals(right);
            
            switch (oper)
            {
                case APIConditionOperator.EQ : expression = (left, right) => left.Equals(right); break;
                case APIConditionOperator.NE: expression = (left, right) => !left.Equals(right); break;
                case APIConditionOperator.GT: expression = (left, right) => left.ToDouble() > right.ToDouble(); break;
                case APIConditionOperator.LT: expression = (left, right) => left.ToDouble() < right.ToDouble(); break;
                case APIConditionOperator.GE: expression = (left, right) => left.ToDouble() >= right.ToDouble(); break;
                case APIConditionOperator.LE: expression = (left, right) => left.ToDouble() <= right.ToDouble(); break;
            }

            var delegateOperation = expression.Compile();
            var res = delegateOperation(Left, Right);

            return res;
        }

        private IEnumerable<OperandObject> GenerateEquation(string operand)
        {
            List<OperandObject> operands = new List<OperandObject>();
                       
            if (Regex.IsMatch(operand, APIConstants.ParamterPatter))
            {
                Regex.Matches(operand, APIConstants.ParamterPatter).Cast<Match>().All(m =>
                {
                    var _type = m.Groups[1].Value.SplitEx('.');
                    if (!OperandObject.FindAndReplace(operands, _type[0], _type[1]))
                    {
                        operands.Add(new OperandObject
                        {
                            Object = _type[0],
                            Properties = new List<string> { _type[1] }
                        });
                    }
                    return true;
                });
            }
            else operands.Add( new OperandObject { Constant = operand });

            return operands;
        }

        private bool IsOk()
        {            
            var _typeMatch = Regex.Match(Operand, APIConstants.ParamterPatter);
            if (_typeMatch == null) return false; //Invalid condition declaration;
            
            return true;
        }
    }

    internal class OperandObject
    {
        internal string Object { get; set; }
        internal List<string> Properties { get; set; } = new List<string>();
        internal string Constant { get; set; }

        //internal static Expression Equation = Expression

        internal static bool FindAndReplace(List<OperandObject> objects, string obj, string prop)
        {
            var _item = objects.FirstOrDefault(o => o.Object == obj);
            if (_item != null)
            {
                if (!_item.Properties.Any(p => p.Equals(prop))) //Check if property also exists.
                {
                    _item.Properties.Add(prop);
                    return true;
                }
            }
            return false;
        }
    }
}
