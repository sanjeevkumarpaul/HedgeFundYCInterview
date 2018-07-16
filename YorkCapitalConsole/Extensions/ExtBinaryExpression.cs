using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Extensions
{
    public static partial class ExtBinaryExpression
    {
        public static List<string> UnderlyingFields(this ParameterExpression exp)
        {
            return exp.Type.UnderlyingSystemType.GetProperties().Select(p => p.Name).ToList();
        }

        public static string ClosestField(this List<string> propertyNames, string unAllocatedField)
        {
            unAllocatedField = unAllocatedField.ToLower();
            return propertyNames.Find(p => p.ToLower().Equals(unAllocatedField));
        }

        public static PropertyInfo Property(this ParameterExpression exp, string fieldName)
        {
            return exp.Type.UnderlyingSystemType.GetProperties().First(p => p.Name.Equals(fieldName));
        }

        public static object ConvertToFieldType(this ParameterExpression exp, string fieldName, string value)
        {
            var property = exp.Property(fieldName);

            object result = property.ConvertToPropertyType(value);

            return result;
        }

        public static Expression InjectStringMethod(this Expression propertyExp, string methodName, params object[] parameters)
        {
            MethodInfo method = typeof(string).GetMethod(methodName, new Type[0]);
            MethodCallExpression methodCallExp = Expression.Call(propertyExp, method);

            return methodCallExp;
        }

        public static List<T> Order<T>(this List<T> entities, string sortorder, string sortField)
        {
            //Since Expression Trees only work with IQueryable(s).
            var query = entities.AsQueryable();

            var type = typeof(T);
            var property = type.GetProperty(sortField);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            var typeArguments = new Type[] { type, property.PropertyType };
            var methodName = sortorder.ToLower().Equals("asc") ? "OrderBy" : "OrderByDescending";
            var resultExp = Expression.Call(typeof(Queryable), methodName, typeArguments, query.Expression, Expression.Quote(orderByExp));

            return query.Provider.CreateQuery<T>(resultExp).ToList();
        }


        public static List<T> Page<T>(this List<T> entities, int pageSize, int pageNo)
        {
            var usersCount = entities.Count();
            if (pageSize > 0)
            {
                entities = entities.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
            }

            return entities;
        }
    }
}
