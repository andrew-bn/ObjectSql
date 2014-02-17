using ObjectSql.Core.Bo;
using ObjectSql.Core.QueryBuilder;
using ObjectSql.Core.SchemaManager;
using ObjectSql.Core.Misc;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.EntityClient;
using System.Linq;

namespace ObjectSql.Core
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
			var dbCommand = context.QueryEnvironment.Command;

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
			return new ObjectQueryBuilder(context.QueryEnvironment).BuildQuery(context.QueryParts.ToArray());
		}

		internal static QueryContext CreateQueryContext(QueryEnvironment environment)
		{
			return new QueryContext(environment);
		}
	}
}
