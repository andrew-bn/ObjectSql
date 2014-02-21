using System;
using ObjectSql.Core.QueryBuilder;
using ObjectSql.Core.QueryBuilder.ExpressionsAnalizers;
using ObjectSql.Core.QueryBuilder.InfoExtractor;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.SchemaManager;

namespace ObjectSql.Core.Bo
{
	public class BuilderContext
	{
		public IDatabaseManager DatabaseManager { get; private set; }
		public IEntitySchemaManager SchemaManager { get; private set; }
		public ISqlWriter SqlWriter { get; private set; }
		public IExpressionAnalizer ExpressionAnalizer { get; private set; }
		public IDelegatesBuilder DelegatesBuilder { get; private set; }
		public IMaterializationInfoExtractor MaterializationInfoExtractor { get; private set; }
		public IInsertionInfoExtractor InsertionInfoExtractor { get; private set; }
		public CommandPreparatorsHolder Preparators { get; private set; }
		public BuilderState State { get; set; }
		public CommandText Text { get; set; }
		public EntityInsertionInformation InsertionInfo { get; set; }
		public EntityMaterializationInformation MaterializationInfo { get; set; }
		public Delegate MaterializationDelegate { get; set; }

		public BuilderContext(IDatabaseManager databaseManager, IEntitySchemaManager schemaManager, ISqlWriter sqlWriter, IExpressionAnalizer expressionAnalizer, IDelegatesBuilder delegatesBuilder,
			IMaterializationInfoExtractor materializationInfoExtractor, IInsertionInfoExtractor insertionInfoExtractor)
		{
			DatabaseManager = databaseManager;
			SchemaManager = schemaManager;
			SqlWriter = sqlWriter;
			ExpressionAnalizer = expressionAnalizer;
			DelegatesBuilder = delegatesBuilder;
			MaterializationInfoExtractor = materializationInfoExtractor;
			InsertionInfoExtractor = insertionInfoExtractor;
			Text = new CommandText();
			Preparators = new CommandPreparatorsHolder();
		}
	}
}
