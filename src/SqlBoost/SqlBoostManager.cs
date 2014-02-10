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
	public class SqlBoostManager<T> : ISqlBoostManager where T : IDbConnection, new()
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
			if (typeof(T) == typeof(EntityConnection))
				connection.ConnectionString = _connectionString;
			else
			{
				EntityConnectionStringBuilder entityStringBuilder;
				var isEf = ConnectionStringAnalizer.TryGetEfConnectionString(_connectionString, out entityStringBuilder);
				connection.ConnectionString = isEf 
												? entityStringBuilder.ProviderConnectionString 
												: _connectionString;
			}
			return new SqlBoostConnection(_connectionString, connection);
		}
		public Func<TArgs, IQueryEnd<TEntity>> CompileQuery<TArgs, TEntity>(Expression<Func<TArgs, IQueryEnd<TEntity>>> query)
		{
			var func = query.Compile();
			var result = (QueryEnd<TEntity>)func(default(TArgs));
			var preparationData = QueryManager.GetQueryPreparationData(result.Context);

			return (arg1) =>
				{
					var context = new CompiledQueryContext(
						CreateConnection().CreateCommand(),
						new StrongBox<TArgs>(arg1), result.Context);
					QueryManager.PrepareQuery(context, preparationData);
					return new QueryEnd<TEntity>(context);
				};
		}

	}
}
