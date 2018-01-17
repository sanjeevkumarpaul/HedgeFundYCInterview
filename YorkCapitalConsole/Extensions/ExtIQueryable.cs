using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
