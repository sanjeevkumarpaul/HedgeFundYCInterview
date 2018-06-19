﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using APICalls.Enum;
using Extensions;
 

namespace APICalls.Dependents
{
    internal class APIExpression
    {
        private string Operand;
        private APIOperator Operator;
        private string Comparer;
        private List<object> Objects;

        private string Left;
        private string Right;
        
        internal APIExpression(List<object> objects, string operand, APIOperator oper, string comparer)
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
                SearchOperands(Operand, false);
                return Operation(Operator);
            }

            return "";
        }

        private void SearchOperands(string operand, bool isleft = true)
        {
            string res = string.Empty;
            var oper = new Operand(operand);
            if (!oper.Type.Empty())
            {
                var obj = Objects.Find(o => o.GetType().Name.Equals(oper.Type, StringComparison.CurrentCultureIgnoreCase));
                res = obj?.GetVal(oper.Property);
            }
            else res = oper.Constant;
            if (isleft) Left = res; else Right = res;
            
        }


        private bool Operation(APIOperator oper)
        {
            Expression<Func<string, string, bool>> expression = (left, right) => left.Equals(right);
            
            switch (oper)
            {
                case APIOperator.EQ : expression = (left, right) => left.Equals(right); break;
                case APIOperator.NE: expression = (left, right) => !left.Equals(right); break;
                case APIOperator.GT: expression = (left, right) => left.ToDouble() > right.ToDouble(); break;
                case APIOperator.LT: expression = (left, right) => left.ToDouble() < right.ToDouble(); break;
                case APIOperator.GE: expression = (left, right) => left.ToDouble() >= right.ToDouble(); break;
                case APIOperator.LE: expression = (left, right) => left.ToDouble() <= right.ToDouble(); break;
            }

            var delegateOperation = expression.Compile();
            var res = delegateOperation(Left, Right);

            return res;
        }



        private bool IsOk()
        {            
            var _typeMatch = Regex.Match(Operand, "{(.*)}");
            if (_typeMatch == null) return false; //Invalid condition declaration;
            
            return true;
        }
    }

    internal class Operand
    {
        internal string Type { get; set; }
        internal string Property { get; set; }
        internal string Constant { get; set; }

        internal Operand(string operand)
        {
            if (Regex.IsMatch(operand, "{(.*)}"))
            {
                var _type = operand.TrimEx("{").TrimEx("}").SplitEx(".");
                Type = _type[0];
                Property = _type[1];
            }
            else Constant = operand;
        }
    }
}
