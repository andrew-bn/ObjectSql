using ObjectSql.Core.Bo;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;
using ObjectSql.Core.Bo.EntitySchema;
using ObjectSql.Core.QueryBuilder.ExpressionsAnalizers;
using ObjectSql.Core.QueryBuilder.InfoExtractor;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.QueryParts;
using ObjectSql.Core.SchemaManager;
using ObjectSql.Exceptions;
using ObjectSql.QueryInterfaces;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace ObjectSql.Core.QueryBuilder
{
	public class ObjectQueryBuilder: IQueryBuilder
	{
		private readonly IDatabaseManager _databaseManager;
		private readonly IEntitySchemaManager _schemaManager;
		private readonly ISqlWriter _sqlWriter;
		private readonly IDelegatesBuilder _delegatesBuilder;
		private readonly IExpressionAnalizer _expressionAnalizer;
		private readonly IMaterializationInfoExtractor _materializationInfoExtrator;
		private readonly IInsertionInfoExtractor _insertionInfoExtractor;

		public ObjectQueryBuilder(QueryEnvironment env)
			: this(env.DatabaseManager, env.SchemaManager,env.SqlWriter,env.DelegatesBuilder,
					new ExpressionAnalizer(env.SchemaManager,env.DelegatesBuilder,env.SqlWriter),
					new MaterializationInfoExtractor(env.SchemaManager),
					new InsertionInfoExtractor(env.SchemaManager))
		{
		}

		private ObjectQueryBuilder(IDatabaseManager databaseManager,
							   IEntitySchemaManager schemaManager,
							   ISqlWriter sqlWriter,
							   IDelegatesBuilder expressionBuilder,
							   IExpressionAnalizer expressionAnalizer,
							   IMaterializationInfoExtractor materializationInfoExtrator, 
							   IInsertionInfoExtractor insertionInfoExtractor)
		{
			_databaseManager = databaseManager;
			_schemaManager = schemaManager;
			_sqlWriter = sqlWriter;
			_delegatesBuilder = expressionBuilder;
			_expressionAnalizer = expressionAnalizer;
			_materializationInfoExtrator = materializationInfoExtrator;
			_insertionInfoExtractor = insertionInfoExtractor;
		}

		public QueryPreparationData BuildQuery(IQueryPart[] parts)
		{
			var context = new BuilderContext(_databaseManager, _schemaManager, _sqlWriter,_expressionAnalizer,_delegatesBuilder, _materializationInfoExtrator, _insertionInfoExtractor);

			foreach (var part in parts)
				part.BuildPart(context);

			return new QueryPreparationData(context.Text.ToString(),
											context.Preparators.PreProcessors.ToArray(),
											context.Preparators.PostProcessors.ToArray(),
											context.MaterializationDelegate);
		}
	}
}
