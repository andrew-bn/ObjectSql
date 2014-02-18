using System;
using ObjectSql.Core.QueryBuilder;
using ObjectSql.Core.QueryBuilder.ExpressionsAnalizers;
using ObjectSql.Core.SchemaManager;

namespace ObjectSql.Core.Bo
{
	public class BuilderContext
	{
		public IEntitySchemaManager SchemaManager { get; private set; }
		public ISqlWriter SqlWriter { get; private set; }
		public IExpressionAnalizer ExpressionAnalizer { get; private set; }
		public CommandPreparatorsHolder Preparators { get; private set; }
		public BuilderState State { get; set; }
		public CommandText Text { get; set; }
		public EntityInsertionInformation InsertionInfo { get; set; }
		public EntityMaterializationInformation MaterializationInfo { get; set; }
		public Delegate MaterializationDelegate { get; set; }

		public BuilderContext(IEntitySchemaManager schemaManager, ISqlWriter sqlWriter, IExpressionAnalizer expressionAnalizer)
		{
			SchemaManager = schemaManager;
			SqlWriter = sqlWriter;
			ExpressionAnalizer = expressionAnalizer;
			Text = new CommandText();
			Preparators = new CommandPreparatorsHolder();
		}
	}
}
