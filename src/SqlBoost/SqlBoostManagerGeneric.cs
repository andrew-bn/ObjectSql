using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using SqlBoost.Core;
using SqlBoost.Core.Bo;
using SqlBoost.Core.Misc;
using SqlBoost.QueryImplementation;
using SqlBoost.QueryInterfaces;
using System;
using System.Data;
using System.Data.EntityClient;

namespace SqlBoost
{

	public class SqlBoostManager<T> : SqlBoostManager, ISqlBoostManager where T : IDbConnection, new()
	{
		private readonly string _connectionString;
		public SqlBoostManager(string connectionString)
		{
			_connectionString = connectionString;
		}
		public ISql Query()
		{
			var connection = CreateConnection();
			return connection.CreateCommand().Query(ResourcesTreatmentType.DisposeConnection);
		}

		public IDbConnection CreateConnection()
		{
			var connection = new T();

			var factory = FindSchemaManagerFactory(connection, _connectionString);
			factory.SetupConnectionString(connection, _connectionString);


			return new SqlBoostConnection(_connectionString, connection);
		}
		public Func<TArgs, IQueryEnd<TEntity>> CompileQuery<TArgs, TEntity>(Expression<Func<TArgs, IQueryEnd<TEntity>>> query)
		{
			var func = query.Compile();
			var result = (QueryEnd<TEntity>)func(default(TArgs));
			var preparationData = QueryManager.GetQueryPreparationData(result.Context);

			return (arg1) =>
				{
					var conn = CreateConnection();
					var factory = FindSchemaManagerFactory(conn, _connectionString);
					var dbManager= FindDatabaseManager(conn, factory.TryGetProviderName(conn, _connectionString));
					var context = new CompiledQueryContext(
						conn.CreateCommand(), dbManager, factory,
						new StrongBox<TArgs>(arg1), result.Context);
					QueryManager.PrepareQuery(context, preparationData);
					return new QueryEnd<TEntity>(context);
				};
		}

	}
}
