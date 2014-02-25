using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectSql.Core.Bo;
using ObjectSql.Core.Misc;
using ObjectSql.Core.QueryBuilder.ExpressionsAnalizers;
using ObjectSql.Core.QueryBuilder.InfoExtractor;

namespace ObjectSql.Core.QueryParts
{
	public class SqlPart : QueryPart
	{
		private int? _hashCode = null;
		private List<QueryPart> _queryParts;
		public IList<QueryPart> QueryParts { get { return _queryParts; } }
		public QueryRoots _queryRoots;
		public QueryRoots QueryRoots { get { return _queryRoots; } }
		public BuilderContext BuilderContext { get; private set; }
		public SqlPart(QueryEnvironment env)
		{
			_queryRoots = new QueryRoots();
			
			_queryParts = new List<QueryPart>();
			
			BuilderContext = new BuilderContext(env.DatabaseManager, env.SchemaManager, env.SqlWriter,
				new ExpressionAnalizer(env.SchemaManager, env.DelegatesBuilder, env.SqlWriter),
				env.DelegatesBuilder, new MaterializationInfoExtractor(env.SchemaManager),
				new InsertionInfoExtractor(env.SchemaManager),_queryParts);
		}

		public void AddQueryPart(QueryPart part)
		{
			_queryParts.Add(part);
		}

		public override int GetHashCode()
		{

			if (!_hashCode.HasValue)
				_hashCode = ExpressionCompareBasedGetHashCode();

			return _hashCode.Value;
		}

		private int ExpressionCompareBasedGetHashCode()
		{
			foreach (var part in _queryParts)
				part.CalculateQueryExpressionParameters(ref _queryRoots);

			return _queryRoots.Hash;
		}

		public override bool Equals(object obj)
		{
			var b = obj as SqlPart;
			if (b == null) return false;

			return ExpressionCompareBasedEquals(b);
		}

		private bool ExpressionCompareBasedEquals(SqlPart obj)
		{
			if (_queryParts.Count != obj._queryParts.Count ||
				!ExpressionComparer.AreEqual(ref _queryRoots, ref obj._queryRoots))
				return false;

			for (var i = 0; i < _queryParts.Count; i++)
			{
				if (!_queryParts[i].IsEqualTo(obj._queryParts[i], ref _queryRoots, ref obj._queryRoots))
					return false;
			}

			return true;
		}

		public QueryPreparationData BuildPart()
		{
			BuildPart(BuilderContext);
			return new QueryPreparationData(BuilderContext.Text.ToString(),
											BuilderContext.Preparators.PreProcessors.ToArray(),
											BuilderContext.Preparators.PostProcessors.ToArray(),
											BuilderContext.MaterializationDelegate);
		}

		public override void BuildPart(BuilderContext context)
		{
			
			while (true)
			{
				foreach (var part in _queryParts)
				{
					if (part.SortParts(context))
						goto NextIteration;
				}
				break;
				NextIteration:;
			}

			foreach (var part in _queryParts)
				part.BuildPart(context);
		}
	}
}
