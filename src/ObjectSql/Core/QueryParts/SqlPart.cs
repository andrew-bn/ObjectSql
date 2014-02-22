﻿using System;
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
	public class SqlPart : QueryPartBase
	{
		public QueryContext Context { get; private set; }
		private int? _hashCode = null;
		private List<IQueryPart> _queryParts;
		public IList<IQueryPart> QueryParts { get { return _queryParts; } }
		public QueryRoots _queryRoots;
		public QueryRoots QueryRoots { get { return _queryRoots; } }
		public BuilderContext BuilderContext { get; private set; }
		public SqlPart(QueryContext context)
		{
			_queryRoots = new QueryRoots();
			Context = context;
			var env = Context.QueryEnvironment;
			_queryParts = new List<IQueryPart>();
			BuilderContext = new BuilderContext(env.DatabaseManager, env.SchemaManager, env.SqlWriter,
				new ExpressionAnalizer(env.SchemaManager, env.DelegatesBuilder, env.SqlWriter),
				env.DelegatesBuilder, new MaterializationInfoExtractor(env.SchemaManager),
				new InsertionInfoExtractor(env.SchemaManager));
		}

		public void AddQueryPart(IQueryPart part)
		{
			_queryParts.Add(part);
		}

		public override QueryPartType PartType
		{
			get { return QueryPartType.Sql; }
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

		public void BuildPart()
		{
			BuildPart(BuilderContext);
		}

		public override void BuildPart(BuilderContext context)
		{
			foreach (var part in _queryParts)
				part.BuildPart(context);

			Context.PreparationData = new QueryPreparationData(context.Text.ToString(),
																context.Preparators.PreProcessors.ToArray(),
																context.Preparators.PostProcessors.ToArray(),
																context.MaterializationDelegate);
		}
	}
}