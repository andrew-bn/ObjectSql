using System.Collections.Generic;
using System.Linq;
using ObjectSql;
using ObjectSql.Core;
using ObjectSql.Core.Bo;
using ObjectSql.Core.QueryBuilder.InfoExtractor;
using ObjectSql.QueryImplementation;
using ObjectSql.QueryInterfaces;
using System.Data;
using System.Data.Common;

namespace System
{
	public static class ObjectSqlExtensions
	{
		public static IQuery ObjectSql(this DbCommand command)
		{
			return command.ObjectSql(ResourcesTreatmentType.DisposeReader);
		}
		public static IQuery ObjectSql(this DbConnection connection)
		{
			return connection.CreateCommand().ObjectSql(ResourcesTreatmentType.DisposeCommand);
		}
		public static IQuery ObjectSql(this DbCommand command, ResourcesTreatmentType treatType)
		{
			var objSqlCommand = command as ObjectSqlCommand;
			var dbCommand = objSqlCommand == null ? command : objSqlCommand.UnderlyingCommand;
			var initialCs = objSqlCommand == null ? command.Connection.ConnectionString : objSqlCommand.Connection.ConnectionString;
			
			var factory = ObjectSqlRegistry.FindSchemaManagerFactory(dbCommand.Connection, initialCs);
			var providerName = factory.TryGetProviderName(dbCommand.Connection, initialCs);
			
			var dbManager = ObjectSqlRegistry.FindDatabaseManager(dbCommand.Connection, providerName);
			var sm = factory.CreateSchemaManager(dbManager.DbType, initialCs);

			var env = new QueryEnvironment(
						sm,
						dbManager,
						dbManager.CreateDelegatesBuilder(),
						dbManager.CreateSqlWriter());

			var context = new QueryContext(initialCs,dbCommand,treatType, env);

			return new Query(context);
		}
	}
}
