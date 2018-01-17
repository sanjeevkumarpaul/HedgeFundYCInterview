using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data.Entity.Core.Objects;

namespace Wrappers
{
    using InterfaceArchitecture;
    

    public class WrapEntityFramework
    {
        private ObjectContext Context;
        private ILogger Log;

        public WrapEntityFramework(DbContext context, ILogger log)
        {
            Context = ((IObjectContextAdapter)context).ObjectContext;
            Context.SavingChanges += new EventHandler(SavingChanges);

            Log = log;
        }
        
        /*
         *      USAGE
         *      =======================================================
         *      Context = ((IObjectContextAdapter) DBase).ObjectContext;

                Context.SavingChanges += new EventHandler(SavingChanges);
                DBase.Database.Log = DBLogger;
         */
        public void SavingChanges(object sender, EventArgs e)
        {
            var commandText = new StringBuilder("Executing BATCH INSERT/UPDATE Query - " + Environment.NewLine);

            var conn = sender.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.Name == "Connection")
                .Select(p => p.GetValue(sender, null))
                .SingleOrDefault();
            var entityConn = (EntityConnection)conn;

            var objStateManager = (System.Data.Entity.Core.Objects.ObjectStateManager)sender.GetType()
                .GetProperty("ObjectStateManager", BindingFlags.Instance | BindingFlags.Public)
                .GetValue(sender, null);

            var workspace = entityConn.GetMetadataWorkspace();

            var translatorT =
                sender.GetType().Assembly.GetType("System.Data.Entity.Core.Mapping.Update.Internal.UpdateTranslator");

            var entityAdapterT =
                sender.GetType().Assembly.GetType("System.Data.Entity.Core.EntityClient.Internal.EntityAdapter");
            var entityAdapter = Activator.CreateInstance(entityAdapterT, BindingFlags.Instance |
                                                                         BindingFlags.NonPublic | BindingFlags.Public,
                null, new object[] { sender }, System.Globalization.CultureInfo.InvariantCulture);

            entityAdapterT.GetProperty("Connection").SetValue(entityAdapter, entityConn);

            var translator = Activator.CreateInstance(translatorT, BindingFlags.Instance |
                                                                   BindingFlags.NonPublic | BindingFlags.Public, null,
                new object[] { entityAdapter }, System.Globalization.CultureInfo.InvariantCulture);

            var produceCommands = translator.GetType()
                .GetMethod("ProduceCommands", BindingFlags.NonPublic | BindingFlags.Instance);

            var commands = (IEnumerable<object>)produceCommands.Invoke(translator, null);

            foreach (var cmd in commands)
            {
                var identifierValues = new Dictionary<int, object>();
                var dcmd =
                    (System.Data.Common.DbCommand)cmd.GetType()
                        .GetMethod("CreateCommand", BindingFlags.Instance | BindingFlags.NonPublic)
                        .Invoke(cmd, new[] { identifierValues });

                foreach (System.Data.Common.DbParameter param in dcmd.Parameters)
                {
                    var sqlParam = (SqlParameter)param;

                    commandText.AppendLine(
                        $"DECLARE {sqlParam.ParameterName} {sqlParam.SqlDbType} {(sqlParam.Size > 0 ? "(" + sqlParam.Size + ")" : string.Empty)}");

                    commandText.AppendLine($"SET {sqlParam.ParameterName} = '{sqlParam.SqlValue}';");
                }

                commandText.AppendLine();
                commandText.AppendLine(dcmd.CommandText);
                commandText.AppendLine("GO");
                commandText.AppendLine();
            }

            Log.Info(commandText.ToString());
        }

    }
}
