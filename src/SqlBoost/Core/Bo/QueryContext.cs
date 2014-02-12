using System;
using System.Collections.Generic;
using System.Data;
using SqlBoost.Core.Misc;
using SqlBoost.Core.QueryParts;

namespace SqlBoost.Core.Bo
{
	public class QueryContext
	{
		public const int PRIME = 397;
		public IDbCommand DbCommand { get; private set; }
		public IDatabaseManager DbManager { get; private set; }
		public ISchemaManagerFactory SchemaManagerFactory { get; private set; }
		public string ConnectionString { get; private set; }
		public IReadOnlyList<IQueryPart> QueryParts { get { return _queryParts; } }
		public QueryRoots QueryRoots { get { return QueryRootsStruct; } }
		public Delegate MaterializationDelegate { get; set; }
		public ResourcesTreatmentType ResourcesTreatmentType { get; private set; }
		public bool Prepared { get; set; }

		private List<IQueryPart> _queryParts;
		protected QueryRoots QueryRootsStruct;

		internal QueryContext(IDbCommand command, IDatabaseManager dbManager, ISchemaManagerFactory schemaManagerFactory, string connectionString, ResourcesTreatmentType resTreatment)
		{
			DbCommand = command;
			DbManager = dbManager;
			SchemaManagerFactory = schemaManagerFactory;
			ConnectionString = connectionString;
			_queryParts = new List<IQueryPart>();
			ResourcesTreatmentType = resTreatment;
		}
		internal QueryContext CopyWith(IDbCommand command)
		{
			var result = new QueryContext(command,DbManager, SchemaManagerFactory, ConnectionString,ResourcesTreatmentType)
				{
					QueryRootsStruct = QueryRootsStruct,
					_queryParts = _queryParts,
					MaterializationDelegate = MaterializationDelegate
				};
			return result;
		}
		internal void AddQueryPart(IQueryPart part)
		{
			_queryParts.Add(part);
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
			var cmd = DbCommand;
			var objCmd = obj.DbCommand;
			if (_queryParts.Count != obj._queryParts.Count ||
				!ExpressionComparer.AreEqual(ref QueryRootsStruct, ref obj.QueryRootsStruct) ||
				cmd.GetType() != objCmd.GetType() ||
				cmd.Connection.ConnectionString != objCmd.Connection.ConnectionString)
				return false;

			for (var i = 0; i < _queryParts.Count; i++)
			{
				if (!_queryParts[i].IsEqualTo(obj._queryParts[i], ref QueryRootsStruct, ref obj.QueryRootsStruct))
					return false;
			}

			return true;
		}
		#endregion
		#region GetHashCode
		public override int GetHashCode()
		{
			return ExpressionCompareBasedGetHashCode();
		}

		private int ExpressionCompareBasedGetHashCode()
		{
			if (!QueryRootsStruct.RootsGenerated)
			{
				CalculateDbCommandHash(ref QueryRootsStruct);

				foreach (var part in _queryParts)
					part.CalculateQueryExpressionParameters(ref QueryRootsStruct);
			}
			return QueryRootsStruct.Hash;
		}

		private void CalculateDbCommandHash(ref QueryRoots parameters)
		{
			parameters.Hash *= PRIME;
			parameters.Hash ^= DbCommand.GetType().GetHashCode();
			parameters.Hash *= PRIME;
			parameters.Hash ^= DbCommand.Connection.ConnectionString.GetHashCode();
		}
		#endregion
	}
}
