using System.Collections.Generic;
using System.Linq;
using ObjectSql;
using ObjectSql.Core;
using ObjectSql.Core.Bo;
using ObjectSql.Core.QueryBuilder.InfoExtractor;
using ObjectSql.QueryImplementation;
using ObjectSql.QueryInterfaces;
using System.Data;

namespace System
{
	public static class ObjectSqlExtensions
	{
		public static ISql Query(this IDbCommand command)
		{
			return command.Query(ResourcesTreatmentType.DisposeReader);
		}
		public static ISql Query(this IDbConnection connection)
		{
			return connection.CreateCommand().Query(ResourcesTreatmentType.DisposeCommand);
		}
		internal static ISql Query(this IDbCommand command, ResourcesTreatmentType treatType)
		{
			var objSqlCommand = command as ObjectSqlCommand;
			var dbCommand = objSqlCommand == null ? command : objSqlCommand.UnderlyingCommand;
			var initialCs = objSqlCommand == null ? command.Connection.ConnectionString : objSqlCommand.Connection.ConnectionString;
			
			var factory = ObjectSqlRegistry.FindSchemaManagerFactory(dbCommand.Connection, initialCs);
			var providerName = factory.TryGetProviderName(dbCommand.Connection, initialCs);
			var dbManager = ObjectSqlRegistry.FindDatabaseManager(dbCommand.Connection, providerName);
			
			var sm = factory.CreateSchemaManager(dbManager.DbType, initialCs);
			var queryBuilder = dbManager.CreateQueryBuilder(sm);

			var context = QueryManager.CreateQueryContext(dbCommand, queryBuilder, sm, command.Connection.ConnectionString, treatType);
			return new QueryRoot(context);
		}
	}
}
