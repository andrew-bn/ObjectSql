using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using ObjectSql.Core.Misc;
using ObjectSql.Core.QueryBuilder;
using ObjectSql.Core.QueryParts;
using ObjectSql.Core.SchemaManager;

namespace ObjectSql.Core.Bo
{
	public class QueryContext
	{
		private static readonly ConcurrentDictionary<QueryContext, QueryPreparationData> _queryCache = new ConcurrentDictionary<QueryContext, QueryPreparationData>();

		public const int PRIME = 397;

		public QueryEnvironment QueryEnvironment { get; set; }
		public SqlPart SqlPart { get; set; }
		
		public Delegate MaterializationDelegate { get; set; }
		public QueryPreparationData PreparationData { get; set; }
		public bool Prepared { get; set; }
		public bool ConnectionOpened { get; set; }

		internal QueryContext(QueryEnvironment queryEnvironment)
		{
			ConnectionOpened = false;
			QueryEnvironment = queryEnvironment;
			SqlPart = new SqlPart(queryEnvironment);
		}
		internal QueryContext CopyWith(IDbCommand command)
		{
			var result = new QueryContext(QueryEnvironment)
				{
					SqlPart = this.SqlPart,
					MaterializationDelegate = MaterializationDelegate
				};
			return result;
		}

		#region Equals
		public override bool Equals(object obj)
		{
			var b = obj as QueryContext;
			if (b == null) return false;

			return ExpressionCompareBasedEquals(b);
		}

		private bool ExpressionCompareBasedEquals(QueryContext obj)
		{
			var cmd = QueryEnvironment.Command;
			var objCmd = obj.QueryEnvironment.Command;

			return SqlPart.Equals(obj.SqlPart) &
			       cmd.GetType() == objCmd.GetType() &&
			       QueryEnvironment.InitialConnectionString == obj.QueryEnvironment.InitialConnectionString;

		}
		#endregion
		#region GetHashCode

		private int? _hashCode = null;
		public override int GetHashCode()
		{
			if (!_hashCode.HasValue)
				_hashCode = CalculateDbCommandHash(SqlPart.GetHashCode());

			return _hashCode.Value;
		}

		private int CalculateDbCommandHash(int hash)
		{
			hash *= PRIME;
			hash ^= QueryEnvironment.Command.GetType().GetHashCode();
			hash *= PRIME;
			hash ^= QueryEnvironment.Command.Connection.ConnectionString.GetHashCode();

			return hash;
		}
		#endregion

		public IDbCommand PrepareQuery()
		{
			if (!Prepared)
			{
				PreparationData = _queryCache.GetOrAdd(this, c => GeneratePreparationData());

				PreProcessQuery();

				Prepared = true;
			}
			return QueryEnvironment.Command;
		}

		public QueryPreparationData GeneratePreparationData()
		{
			return SqlPart.BuildPart();
		}
		internal void PreProcessQuery()
		{
			var preparationData = PreparationData;
			MaterializationDelegate = preparationData.DataMaterializer;
			var dbCommand = QueryEnvironment.Command;

			if (string.IsNullOrEmpty(dbCommand.CommandText))
				dbCommand.CommandText = preparationData.CommandText;
			else
				dbCommand.CommandText += preparationData.CommandText;

			for (int i = 0; i < preparationData.PreProcessors.Length; i++)
			{
				if (!preparationData.PreProcessors[i].RootDemanding)
					preparationData.PreProcessors[i].CommandPreparationAction(dbCommand, null);
				else
				{
					foreach (var root in this.SqlPart.QueryRoots.Roots)
					{
						if ((root.Value & preparationData.PreProcessors[i].RootMap) != 0)
							preparationData.PreProcessors[i].CommandPreparationAction(dbCommand, root.Key);
					}
				}
			}
		}
	}
}
