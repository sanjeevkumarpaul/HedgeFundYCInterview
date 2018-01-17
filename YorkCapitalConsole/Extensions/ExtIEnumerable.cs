using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static partial class ExtIEnumerable
    {
        public static string JoinExt<T>(this IEnumerable<T> objs, string seprator = " ", bool donotPutEmptyElements = false)
        {
            string joinstr = "";
            if (!objs.Null())
                foreach (T obj in objs)
                    joinstr = string.Format("{0}{1}{2}", joinstr, (donotPutEmptyElements) ? (joinstr.Empty() ? "" : seprator) : seprator, obj);

            return joinstr.Trim().TrimEx(seprator);
        }

        public static IEnumerable<T> WhereWildCharacterMatch<T>(this IEnumerable<T> source, Func<T, string> nameSelector, string nameToCompare, int numberOfChars)
        {
            var formatName = nameToCompare.Replace(" ", "").RemoveSpecialCharacters();

            //get first 5 wild charac match on nameToCompare
            var wildCharac = formatName
                .Substring(0, Math.Min(formatName.Length, numberOfChars))
                .ToLower();

            return source.Where(x =>
            {
                string lowercaseStr = nameSelector(x).Replace(" ", "").RemoveSpecialCharacters().ToLower();
                string dWildChar = lowercaseStr.Substring(0, Math.Min(lowercaseStr.Length, wildCharac.Length));
                return (dWildChar == wildCharac.Substring(0, dWildChar.Length));
            });
        }

        //Order By and Pagination
        public static List<T> Pagination<T>(this IEnumerable<T> source, string orderBy, string sortOrder, int pageNo, int pageSize)
        {
            return source.AsQueryable()
                           .OrderIt(orderBy, sortOrder)
                           .Skip((pageNo - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();
        }

        public static IEnumerable<T> OrderIt<T>(this IQueryable<T> source, string orderColumn, string sortOrder = "ASC", params object[] values)
        {
            if (orderColumn == null) return source;


            bool isAscending = !sortOrder.ToLower().Equals("desc");

            var type = typeof(T);
            var property = type.GetProperty(orderColumn);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), isAscending ? "OrderBy" : "OrderByDescending", new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));
            return source.Provider.CreateQuery<T>(resultExp);
        }

    }
}
