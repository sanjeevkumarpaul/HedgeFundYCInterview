using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Extensions;
 

namespace APICalls.Dependents
{
    internal class APIExpression
    {
        private string Operand;
        private string Operator;
        private string Comparer;
        private List<object> Objects;

        private string Left;
        private string Right;
        
        internal APIExpression(List<object> objects, string operand, string oper, string comparer)
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
                Search(Operand, true);
                Constant(Comparer);
                return Operation(Operator);
            }

            return "";
        }

        private void Search(string operand, bool create = false)
        {
            var oper = new Operand(operand);
            var obj = Objects.Find(o => o.GetType().Name.Equals( oper.Type, StringComparison.CurrentCultureIgnoreCase));

            if (obj != null)                
            {
                Left = obj.GetVal(oper.Property);                               
            }
        }

        private void Constant(string value)
        {
            Right = value;
        }

        private bool Operation(string oper)
        {
            Expression<Func<string, string, bool>> expression = (left, right) => left.Equals(right);
            
            switch (oper)
            {
                case "==": expression = (left, right) => left.Equals(right); break;
                case "!=": expression = (left, right) => !left.Equals(right); break;
                case ">": expression = (left, right) => left.ToDouble() > right.ToDouble(); break;
                case "<": expression = (left, right) => left.ToDouble() < right.ToDouble(); break;
                case ">=": expression = (left, right) => left.ToDouble() >= right.ToDouble(); break;
                case "<=": expression = (left, right) => left.ToDouble() <= right.ToDouble(); break;
            }

            var delegateOperation = expression.Compile();
            var res = delegateOperation(Left, Right);

            return res;
        }



        private bool IsOk()
        {            
            var _typeMatch = Regex.Match(Operand, "{(.*)}");
            if (_typeMatch == null) return false; //Invalid condition declaration;
            var _oprMatch = Regex.Match(Operator, @"[><=!]{1,2}");
            if (_oprMatch == null) return false; ; //Invalid condition declaration;

            return true;
        }
    }

    internal class Operand
    {
        internal string Type { get; set; }
        internal string Property { get; set; }

        internal Operand(string operand)
        {
            var _type = operand.TrimEx("{").TrimEx("}").SplitEx(".");
            Type = _type[0];
            Property = _type[1];
        }
    }
}
