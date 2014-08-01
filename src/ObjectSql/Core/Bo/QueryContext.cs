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
		public string InitialConnectionString { get; private set; }
		public IDbCommand Command { get; private set; }
		public ResourcesTreatmentType ResourcesTreatmentType { get; set; }
		
		public QueryEnvironment QueryEnvironment { get; set; }
		public SqlPart SqlPart { get; set; }
		
		public Delegate MaterializationDelegate { get; set; }
		public QueryPreparationData PreparationData { get; set; }
		public bool Prepared { get; set; }
		public bool ConnectionOpened { get; set; }

		internal QueryContext(string initialConnectionString,
								IDbCommand command,
								ResourcesTreatmentType resourcesTreatmentType, 
								QueryEnvironment queryEnvironment)
		{
			ConnectionOpened = false;
			InitialConnectionString = initialConnectionString;
			Command = command;
			ResourcesTreatmentType = resourcesTreatmentType;
			QueryEnvironment = queryEnvironment;
			SqlPart = new SqlPart(this);
		}
		internal QueryContext CopyWith(IDbCommand command)
		{
			var result = new QueryContext(InitialConnectionString,Command,ResourcesTreatmentType, QueryEnvironment)
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
			var cmd = Command;
			var objCmd = obj.Command;

			return SqlPart.Equals(obj.SqlPart) &
			       cmd.GetType() == objCmd.GetType() &&
			       InitialConnectionString == obj.InitialConnectionString;

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
			hash ^= Command.GetType().GetHashCode();
			hash *= PRIME;
			hash ^= Command.Connection.ConnectionString.GetHashCode();

			return hash;
		}
		#endregion

		public IDbCommand PrepareQuery()
		{
			if (!Prepared)
			{
				SqlPart.SortParts();
				PreparationData = _queryCache.GetOrAdd(this, c => GeneratePreparationData());

				PreProcessQuery();

				Prepared = true;
			}
			return Command;
		}

		public QueryPreparationData GeneratePreparationData()
		{
			return SqlPart.BuildPart();
		}
		internal void PreProcessQuery()
		{
			MaterializationDelegate = PreparationData.DataMaterializer;

			Command.CommandText = PreparationData.CommandText;

			foreach (var p in PreparationData.PreProcessors)
				p.CommandPreparationAction(Command, SqlPart.QueryRoots);
		}
	}
}
