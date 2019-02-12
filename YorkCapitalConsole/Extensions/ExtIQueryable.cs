using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Extensions
{
    public static partial class ExtIQueryable
    {
        #region ^Chunk Reading
        /// <summary>
        /// Gets data in Chunks from Entity frame work for huge amount of data. .... 
        /// USAGE: 
        /// foreach (var client in clientList.OrderBy(c => c.Id).QueryInChunksOf(100))
        /// {
        ///     foreach (var client in chunk) {  /*Do work with chunk data and then save the changes at the end .*/  }
        ///     context.SaveChanges();
        ///  }
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public static IEnumerable<T> QueryInChunksOf<T>(this IQueryable<T> queryable, int chunkSize)
        {
            return queryable.QueryChunksOfSize(chunkSize).SelectMany(chunk => chunk);
        }

        public static IEnumerable<T[]> QueryChunksOfSize<T>(this IQueryable<T> queryable, int chunkSize)
        {
            int chunkNumber = 0;
            while (true)
            {
                var query = (chunkNumber == 0)
                    ? queryable
                    : queryable.Skip(chunkNumber * chunkSize);
                var chunk = query.Take(chunkSize).ToArray();
                if (chunk.Length == 0)
                    yield break;
                yield return chunk;
                chunkNumber++;
            }
        }
        #endregion ~Chunk Reading
    }

    partial class ExtIQueryable
    {
        public static IQueryable<T> Order<T>(this IQueryable<T> query, string sortorder, string sortField, params object[] values)
        {
            var type = typeof(T);
            var property = type.GetProperty(sortField);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            var typeArguments = new Type[] { type, property.PropertyType };
            var methodName = sortorder.ToLower().Equals("asc") ? "OrderBy" : "OrderByDescending";
            var resultExp = Expression.Call(typeof(Queryable), methodName, typeArguments, query.Expression, Expression.Quote(orderByExp));

            return query.Provider.CreateQuery<T>(resultExp);
        }
    }
}
