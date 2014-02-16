using System;
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
		public const int PRIME = 397;

		public IDbCommand DbCommand { get; private set; }
		public IQueryBuilder QueryBuilder { get; set; }
		public IEntitySchemaManager SchemaManager { get; set; }
		public string ConnectionString { get; private set; }
		public IList<IQueryPart> QueryParts { get { return _queryParts; } }
		public QueryRoots QueryRoots { get { return QueryRootsStruct; } }
		public Delegate MaterializationDelegate { get; set; }
		public ResourcesTreatmentType ResourcesTreatmentType { get; private set; }
		public bool Prepared { get; set; }

		private List<IQueryPart> _queryParts;
		protected QueryRoots QueryRootsStruct;

		internal QueryContext(IDbCommand command, IQueryBuilder queryBuilder, IEntitySchemaManager schemaManager, string connectionString, ResourcesTreatmentType resTreatment)
		{
			DbCommand = command;
			QueryBuilder = queryBuilder;
			SchemaManager = schemaManager;
			ConnectionString = connectionString;
			_queryParts = new List<IQueryPart>();
			ResourcesTreatmentType = resTreatment;
		}
		internal QueryContext CopyWith(IDbCommand command)
		{
			var result = new QueryContext(command,QueryBuilder, SchemaManager, ConnectionString,ResourcesTreatmentType)
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

		private int? _hashCode = null;
		public override int GetHashCode()
		{
			if (!_hashCode.HasValue)
				_hashCode = ExpressionCompareBasedGetHashCode();

			return _hashCode.Value;
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
