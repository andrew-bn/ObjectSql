using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ObjectSql.Core;
using ObjectSql.Core.Bo;
using ObjectSql.QueryImplementation;
using ObjectSql.QueryInterfaces;
using ObjectSql.Core.Misc;
using System;
using System.Data;
using System.Data.Common;

namespace ObjectSql
{
	public class ObjectSqlManager<T> : IObjectSqlManager where T : DbConnection, new()
	{
		private readonly string _connectionString;
		public ObjectSqlManager(string connectionString)
		{
			_connectionString = connectionString;
		}

		public IQuery Query()
		{
			var connection = CreateConnection();
			return connection.CreateCommand().ObjectSql(ResourcesTreatmentType.DisposeConnection);
		}

		public DbConnection CreateConnection()
		{
			var connection = new T();

			var factory = ObjectSqlRegistry.FindSchemaManagerFactory(connection, _connectionString);
			factory.SetupConnectionString(connection, _connectionString);


			return new ObjectSqlConnection(_connectionString, connection);
		}
		public Func<TArgs, IQueryEnd<TEntity>> CompileQuery<TArgs, TEntity>(Expression<Func<TArgs, IQueryEnd<TEntity>>> query)
		{
			var func = query.Compile();
			var result = (Query)func(default(TArgs));
			result.Context.GetHashCode();
			result.Context.PreparationData = result.Context.GeneratePreparationData();
			int idx = 0;
			foreach (var root in result.Context.SqlPart.QueryRoots.Roots)
			{
				var rootType = root.GetType();
				if (rootType.IsGenericType() && rootType.GetGenericTypeDefinition() == typeof (StrongBox<>))
					break;
				idx++;
			}
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

					var env = new QueryEnvironment(sm,
												   dbManager,
					                               delBuilder,
					                               sqlWriter);

					var context = new CompiledQueryContext(result.Context.InitialConnectionString,cmd, result.Context.ResourcesTreatmentType, env, new StrongBox<TArgs>(arg1), idx, result.Context);

					context.PreProcessQuery();
					return new QueryEnd<TEntity>(context);
				};
		}

	}
}
