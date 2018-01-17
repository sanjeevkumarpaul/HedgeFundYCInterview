using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
