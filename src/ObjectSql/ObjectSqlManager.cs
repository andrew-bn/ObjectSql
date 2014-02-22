using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ObjectSql.Core;
using ObjectSql.Core.Bo;
using ObjectSql.QueryImplementation;
using ObjectSql.QueryInterfaces;
using ObjectSql.Core.Misc;
using System;
using System.Data;
using System.Data.EntityClient;

namespace ObjectSql
{

	public class ObjectSqlManager<T> : IObjectSqlManager where T : IDbConnection, new()
	{
		private readonly string _connectionString;
		public ObjectSqlManager(string connectionString)
		{
			_connectionString = connectionString;
		}
		public ISql Query()
		{
			var connection = CreateConnection();
			return connection.CreateCommand().ObjectSql(ResourcesTreatmentType.DisposeConnection);
		}

		public IDbConnection CreateConnection()
		{
			var connection = new T();

			var factory = ObjectSqlRegistry.FindSchemaManagerFactory(connection, _connectionString);
			factory.SetupConnectionString(connection, _connectionString);


			return new ObjectSqlConnection(_connectionString, connection);
		}
		public Func<TArgs, IQueryEnd<TEntity>> CompileQuery<TArgs, TEntity>(Expression<Func<TArgs, IQueryEnd<TEntity>>> query)
		{
			var func = query.Compile();
			var result = (QueryEnd<TEntity>)func(default(TArgs));
			result.Context.PreparationData = result.Context.GeneratePreparationData();

			return (arg1) =>
				{
					var conn = CreateConnection();
					var dbConnection = conn is ObjectSqlConnection
						                   ? ((ObjectSqlConnection) conn).UnderlyingConnection
						                   : conn;

					var factory = ObjectSqlRegistry.FindSchemaManagerFactory(dbConnection, _connectionString);
					var dbManager = ObjectSqlRegistry.FindDatabaseManager(dbConnection, factory.TryGetProviderName(conn, _connectionString));
					var sm = factory.CreateSchemaManager(dbManager.DbType, _connectionString);
					var cmd = conn.CreateCommand();
					var delBuilder = dbManager.CreateDelegatesBuilder();
					var sqlWriter = dbManager.CreateSqlWriter();

					var env = new QueryEnvironment(result.Context.QueryEnvironment.InitialConnectionString,
					                               cmd,
					                               result.Context.QueryEnvironment.ResourcesTreatmentType,
					                               sm,
												   dbManager,
					                               delBuilder,
					                               sqlWriter);

					var context = new CompiledQueryContext(env, new StrongBox<TArgs>(arg1), result.Context);

					context.PreProcessQuery();
					return new QueryEnd<TEntity>(context);
				};
		}

	}
}
