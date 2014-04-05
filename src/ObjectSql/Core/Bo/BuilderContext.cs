using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using ObjectSql.Core.QueryBuilder;
using ObjectSql.Core.QueryBuilder.ExpressionsAnalizers;
using ObjectSql.Core.QueryBuilder.InfoExtractor;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.QueryParts;
using ObjectSql.Core.SchemaManager;

namespace ObjectSql.Core.Bo
{
	public class BuilderContext : IExpressionAnalizer
	{
		public QueryContext Context { get; private set; }
		public IDatabaseManager DatabaseManager { get; private set; }
		public IEntitySchemaManager SchemaManager { get; private set; }
		public SqlWriter SqlWriter { get; private set; }

		public IDelegatesBuilder DelegatesBuilder { get; private set; }
		public IMaterializationInfoExtractor MaterializationInfoExtractor { get; private set; }
		public IInsertionInfoExtractor InsertionInfoExtractor { get; private set; }
		public List<QueryPart> Parts { get; private set; }
		public ICommandPreparatorsHolder Preparators { get; set; }
		public CommandText Text { get; set; }
		public EntityInsertionInformation InsertionInfo { get; set; }
		public EntityMaterializationInformation MaterializationInfo { get; set; }
		public Delegate MaterializationDelegate { get; set; }
		public QueryPart CurrentPart { get; set; }
		public Func<IDbCommand, object> ReturnParameterReader { get; set; }
		private readonly Dictionary<ExpressionAnalizerType, ISqlQueryBuilder> _analizers = new Dictionary<ExpressionAnalizerType, ISqlQueryBuilder>();

		public BuilderContext(QueryContext context, IDatabaseManager databaseManager, IEntitySchemaManager schemaManager, SqlWriter sqlWriter, IDelegatesBuilder delegatesBuilder,
			IMaterializationInfoExtractor materializationInfoExtractor, IInsertionInfoExtractor insertionInfoExtractor, List<QueryPart> parts)
		{
			Context = context;
			DatabaseManager = databaseManager;
			SchemaManager = schemaManager;
			SqlWriter = sqlWriter;
			DelegatesBuilder = delegatesBuilder;
			MaterializationInfoExtractor = materializationInfoExtractor;
			InsertionInfoExtractor = insertionInfoExtractor;
			Parts = parts;
			Text = new CommandText();
			Preparators = new CommandPreparatorsHolder();

			_analizers.Add(ExpressionAnalizerType.Expression, new QueryExpressionBuilder(schemaManager, DelegatesBuilder, SqlWriter));
			_analizers.Add(ExpressionAnalizerType.FieldsSelect, new QuerySelectBuilder(schemaManager, DelegatesBuilder, SqlWriter));
			_analizers.Add(ExpressionAnalizerType.FieldsSequence, new QueryFieldsSequenceBuilder(schemaManager, DelegatesBuilder, SqlWriter));
			_analizers.Add(ExpressionAnalizerType.FieldsUpdate, new QueryUpdateBuilder(schemaManager, DelegatesBuilder, SqlWriter));
			_analizers.Add(ExpressionAnalizerType.FuncCall, new QueryFuncCallBuilder(schemaManager, DelegatesBuilder));

		}

		public string AnalizeExpression(ParameterExpression[] parameters,System.Linq.Expressions.Expression expression, ExpressionAnalizerType expressionType)
		{
			return _analizers[expressionType].BuildSql(this,parameters, expression);
		}
	}
}
