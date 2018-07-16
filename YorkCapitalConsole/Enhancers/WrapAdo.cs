using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Wrappers
{
    using Extensions;


    public sealed class WrapAdo
    {
        private WrapAdo()
        {

        }

        /// <summary>
        /// Retrieves the semicolon separated queries as a batch into a DataSet
        /// </summary>
        /// <param name="queries">Semicolor separated Select Statements
        /// E.g., Select * from sometable; [select * from someothertable where somecolumn = 'somevalue'; [...]]
        /// </param>
        /// <param name="connectionStr">Connection string for a database to retieve from</param>
        /// <param name="tables">List of Tables which can be identified via query, so that TableNames within Dataset is set to appropriate name.
        /// E.g., IEnumerable of String => "sometable"[, "someothertable" [...]]
        /// </param>
        /// <returns>DataSet with table results or an Empty Dataset</returns>
        public static DataSet RetreiveInBatch(string queries, string connectionStr, IEnumerable<string> tables = null)
        {
            DataSet dSet = new DataSet();

            if (!queries.Empty())
            {
                using (SqlDataAdapter dbAdapter = new SqlDataAdapter(queries, connectionStr))
                {
                    if (tables != null)
                    {
                        int index = 0;
                        foreach (var table in tables)
                        {
                            string tableName = string.Format("Table{0}", index <= 0 ? "" : index.ToString());

                            dbAdapter.TableMappings.Add(tableName, table);
                            index++;
                        }
                    }

                    dbAdapter.Fill(dSet);
                }
            }

            return dSet;
        }

        /// <summary>
        /// Filters a record based on condition within Dataset and Return a Dictionary based on Column Name and Values.
        /// </summary>
        /// <param name="dSet">Dataset towards which filter has to applied.</param>
        /// <param name="tableName">Filter to be applied towards the table within DataSet. If Table is not found, empty Dictionary is returned.</param>
        /// <param name="condition">Filter condition if required.</param>
        /// <param name="forceCondition">If Set to false, and Table consists of only 1 record, that will be called, Otherwise condition will be applied, if found non-empty.</param>
        /// <param name="returnNullIfNoCount">When set True, returns null when there are no items found, else returns Dictionary</param>
        /// <returns>Dictionary{string, string}, comprising of each field and field value as key pair. </returns>
        public static Dictionary<string, string> FilterInDataSet(DataSet dSet,
                                                                string tableName,
                                                                string condition = null,
                                                                bool forceCondition = false,
                                                                bool returnNullIfNoCount = true)
        {
            Dictionary<string, string> filterSet = new Dictionary<string, string>();
            DataTable dbTable = null;

            if ((dbTable = dSet.Tables[tableName]) != null)
            {
                var row = ((forceCondition && !condition.Empty()) || dbTable.Rows.Count > 1) ? dbTable.Select(condition).ElementAtOrDefault(0) : dbTable.Rows[0];
                if (row != null)
                {
                    foreach (DataColumn column in dbTable.Columns)
                        filterSet.Add(column.ColumnName, row[column].ToStringExt(true));
                }
            }

            return (returnNullIfNoCount && filterSet.Count <= 0) ? null : filterSet;
        }

    }
}
