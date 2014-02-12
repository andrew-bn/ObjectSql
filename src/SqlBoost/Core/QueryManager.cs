using SqlBoost.Core.Bo;
using SqlBoost.Core.Misc;
using SqlBoost.Core.QueryBuilder;
using SqlBoost.Core.SchemaManager;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.EntityClient;
using System.Linq;

namespace SqlBoost.Core
{
	public class QueryManager
	{
		private static readonly ConcurrentDictionary<QueryContext, QueryPreparationData> _queryCache = new ConcurrentDictionary<QueryContext, QueryPreparationData>();

		internal static QueryPreparationData GetQueryPreparationData(QueryContext context)
		{
			return _queryCache.GetOrAdd(context, GeneratePreparationData);
		}
		public static void PrepareQuery(QueryContext context)
		{
			if (!context.Prepared)
			{
				var preparationData = _queryCache.GetOrAdd(context, GeneratePreparationData);
				PrepareQuery(context, preparationData);
				context.Prepared = true;
			}
		}

		internal static void PrepareQuery(QueryContext context, QueryPreparationData preparationData)
		{
			context.MaterializationDelegate = preparationData.DataMaterializer;
			var dbCommand = context.DbCommand;

			if (string.IsNullOrEmpty(dbCommand.CommandText))
				dbCommand.CommandText = preparationData.CommandText;
			else
				dbCommand.CommandText += preparationData.CommandText;

			for (int i = 0; i < preparationData.Parameters.Length; i++)
			{
				if (!preparationData.Parameters[i].RootDemanding)
					preparationData.Parameters[i].CommandPreparationAction(dbCommand, null);
				else
				{
					foreach (var root in context.QueryRoots.Roots)
					{
						if ((root.Value & preparationData.Parameters[i].RootMap) != 0)
							preparationData.Parameters[i].CommandPreparationAction(dbCommand, root.Key);
					}
				}
			}
		}

		private static QueryPreparationData GeneratePreparationData(QueryContext context)
		{
			return CreateQueryBuilder(context).BuildQuery(context.QueryParts.ToArray());
		}

		#region query builder factory
		private static IQueryBuilder CreateQueryBuilder(QueryContext context)
		{
			var sm = GetSchemaManager(context.SchemaManagerFactory, context.DbManager, context.ConnectionString);
			return context.DbManager.CreateQueryBuilder(sm);
		}

		#endregion
		#region schema manager factory
		private static readonly ConcurrentDictionary<ISchemaManagerFactory, ConcurrentDictionary<string, IEntitySchemaManager>> _schemaManagersCache = new ConcurrentDictionary<ISchemaManagerFactory, ConcurrentDictionary<string, IEntitySchemaManager>>();

		private static IEntitySchemaManager GetSchemaManager(ISchemaManagerFactory schemaManager, IDatabaseManager dbManager, string connectionString)
		{
			var cache = _schemaManagersCache.GetOrAdd(schemaManager, sm => new ConcurrentDictionary<string, IEntitySchemaManager>());
			return cache.GetOrAdd(connectionString, cs => schemaManager.CreateSchemaManager(dbManager.DbType, cs));
		}

		#endregion

		internal static QueryContext CreateQueryContext(IDbCommand command, IDatabaseManager dbManager, ISchemaManagerFactory providerManagerFactory, string connectionString, ResourcesTreatmentType resTreatment)
		{
			return new QueryContext(command, dbManager, providerManagerFactory, connectionString, resTreatment);
		}
	}
}
