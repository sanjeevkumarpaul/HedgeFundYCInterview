using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace Extensions
{
    public static partial class ExtEntityFramework
    {
        public static string TableName<Entity>(this Entity T) where Entity : class
        {
            var attr = (TableAttribute)Attribute.GetCustomAttribute(T.GetType(), typeof(TableAttribute));
            if (attr.Null()) return "";

            return attr.Name;
        }

        public static IList<T> Query<T>(this DbSet<T> entity, Func<T, bool> condition) where T : class
        {
            return entity.ToList().Where(condition).ToList();
        }

        public static T Find<T>(this DbSet<T> entity, Func<T, bool> condition) where T : class, new()
        {
            return entity.Where(condition).ToList().FirstOrDefault().NullToEmpty();
        }

        public static IEnumerable<T> FindAll<T>(this DbSet<T> entity, Func<T, bool> condition) where T : class, new()
        {
            return entity.Where(condition);
        }

        public static void Update<T>(this DbContext context, T entity, bool attach = false) where T : class
        {
            if (attach)
            {
                var dbSet = context.Set<T>();
                dbSet.Attach(entity);
            }

            context.Entry(entity).State = EntityState.Modified;
        }

        public static void Add<T>(this DbContext context, T entity) where T : class
        {
            var dbSet = context.Set<T>();
            dbSet.Add(entity);
        }

        public static void RejectChanges(this DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged || e.State != EntityState.Detached))
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        {
                            entry.CurrentValues.SetValues(entry.OriginalValues);
                            entry.State = EntityState.Unchanged;
                            break;
                        }
                    case EntityState.Deleted:
                        {
                            entry.State = EntityState.Unchanged;
                            break;
                        }
                    case EntityState.Added:
                        {
                            entry.State = EntityState.Detached;
                            break;
                        }
                }
            }
        }

        public static int? Save(this DbContext context, bool rejectChangesIfException = true)
        {
            var oContext = ((IObjectContextAdapter)context).ObjectContext;

            if (oContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified | EntityState.Deleted).Count() <= 0)
                return null;

            try
            {
                return oContext.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
            }
            catch (Exception)
            {
                if (rejectChangesIfException) context.RejectChanges();
                throw;
            }
        }

        public static void DeleteAll(this DbContext context, params Type[] entities)
        {
            StringBuilder query = new StringBuilder();
            int entityNo = 1;

            query.AppendLine("BEGIN TRANSACTION;");
            foreach (var entity in entities)
            {
                query.AppendLine($"-- {entityNo++}. --");
                query.AppendLine($"DELETE FROM {entity.TableName()};");
                query.AppendLine();
            }
            query.AppendLine("COMMIT TRANSACTION;");

            if (!query.ToString().Empty()) context.Exec(query.ToString());
        }

        public static void Exec(this DbContext context, string query)
        {
            context.Database.ExecuteSqlCommand(query);
        }

    }


    public static partial class ExtEntityFramework
    {
        public static List<T> GetSet<T>(this DbContext context, Func<T, bool> condition) where T : class
        {
            DbSet<T> dbSetMetadata = context.Set<T>();

            var _set = dbSetMetadata.Where(condition).ToList();

            return _set;
        }

        public static string TableName(this ObjectStateEntry entry)
        {
            string _entity = entry.EntitySet.ToString();
            _entity = _entity.Substring(_entity.LastIndexOf(".") + 1);

            return _entity;
        }

        public static string PrimaryColumn(this ObjectStateEntry entry, List<string> ignoreList = null)
        {
            var primaryFields = entry.EntitySet.ElementType.KeyMembers;
            if (primaryFields.Count <= 0) return null;

            var primaryFieldName = (ignoreList == null) ? primaryFields[0].Name : string.Empty;

            if (ignoreList != null)
            {
                foreach (var primaryField in primaryFields)
                {
                    if (ignoreList.Any(i => i.EqualsIgnoreCase(primaryField.Name))) continue;

                    primaryFieldName = primaryField.Name;
                    break;
                }
            }

            return primaryFieldName;
        }

        public static Int32 PrimaryColumnValueFirstInt(this ObjectStateEntry entry, Int32 NoValue = 0)
        {
            List<string> ignoreFields = new List<string>();
            Int32 intValue = 0;

            do
            {
                var primaryFieldName = entry.PrimaryColumn(ignoreFields);
                if (string.IsNullOrWhiteSpace(primaryFieldName)) { intValue = 0; break; }

                try
                {
                    var primaryField = entry.EntityKey.EntityKeyValues.FirstOrDefault(e => e.Key.EqualsIgnoreCase(primaryFieldName));
                    if (primaryField != null)
                        intValue = primaryField.Value.ToString().ToInt();
                }
                catch { }

                if (entry.State == EntityState.Added) break;
                ignoreFields.Add(primaryFieldName);

            } while (intValue == NoValue);

            return intValue;

        }

        public static Int32 PrimaryColumnValue(this ObjectStateEntry entry, string auditableContextStore)
        {
            int value = entry.PrimaryColumnValueFirstInt();

            if (value == 0 && entry.State != EntityState.Added) value = entry.IdentityColumnValue(auditableContextStore);

            return value;
        }

        public static Int32 IdentityColumnValue(this ObjectStateEntry entry, string auditableContextStore)
        {
            try
            {
                string tableName = entry.TableName();
                string store = auditableContextStore ?? "";

                EntityType edmEntity = (EntityType)entry.ObjectStateManager.MetadataWorkspace.GetType(tableName, store, DataSpace.SSpace);

                var property = edmEntity.Properties.FirstOrDefault(p => p.TypeUsage.Facets.Any(f => f.Name.Equals("StoreGeneratedPattern") && f.Value.ToString().Equals("Identity")));

                return entry.GetColumnValue(property.Name, true).ToInt();
            }
            catch { }

            return 0;
        }

        public static string PrimaryKeyCondition(this ObjectStateEntry entry, bool excludeWhereClause = false)
        {
            List<string> values = new List<string>();
            if (entry.EntityKey.EntityKeyValues != null)
            {
                foreach (EntityKeyMember member in entry.EntityKey.EntityKeyValues)
                    values.Add(string.Format(" {0} = '{1}' ", member.Key.ToUpper(), member.Value));
            }

            string condition = values.JoinExt(" AND ");

            return !condition.Empty() && !excludeWhereClause ? string.Format(" WHERE {0} ", condition) : condition;
        }

        public static string PrimaryKeyCondition(this IEnumerable<ObjectStateEntry> entries, bool excludeWhereClause = false)
        {
            string condition = "";
            foreach (var entry in entries)
            {
                string innerCondition = "";
                if (!(innerCondition = entry.PrimaryKeyCondition(true)).Empty())
                {
                    condition += string.Format("|( {0} ) ", innerCondition);
                }
            }

            condition = condition.Trim().Trim(new char[] { '|' }).Replace("|", " OR ");
            return condition.Empty() ? "" : (excludeWhereClause ? "" : " Where ") + condition;
        }

        public static string GetColumnValue(this ObjectStateEntry entry, string field, bool returnNull = false)
        {
            try
            {
                CurrentValueRecord current = entry.CurrentValues;
                string value = current.GetValue(current.GetOrdinal(field)).ToStringExt().ToEmpty();

                return value;
            }
            catch
            {
                return returnNull ? null : string.Empty;
            }
        }
    }
}
