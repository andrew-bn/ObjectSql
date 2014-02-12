using SqlBoost.Core.Bo;
using SqlBoost.Core.QueryBuilder.LambdaBuilder;
using SqlBoost.Core.SchemaManager;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SqlBoost.Core.QueryBuilder.ExpressionsAnalizers
{
	public class ExpressionAnalizer : IExpressionAnalizer
	{
		private readonly ISqlWriter _sqlWriter;
		private readonly Dictionary<ExpressionAnalizerType, ISqlQueryBuilder> _analizers = new Dictionary<ExpressionAnalizerType, ISqlQueryBuilder>();

		public ExpressionAnalizer(IEntitySchemaManager schemaManager, IDelegatesBuilder expressionBuilder, ISqlWriter sqlWriter)
		{
			_sqlWriter = sqlWriter;
			_analizers.Add(ExpressionAnalizerType.Expression, new QueryExpressionBuilder(schemaManager, expressionBuilder, _sqlWriter));
			_analizers.Add(ExpressionAnalizerType.FieldsSelect, new QuerySelectBuilder(schemaManager, expressionBuilder, _sqlWriter));
			_analizers.Add(ExpressionAnalizerType.FieldsSequence, new QueryFieldsSequenceBuilder(schemaManager, expressionBuilder, _sqlWriter));
			_analizers.Add(ExpressionAnalizerType.FieldsUpdate, new QueryUpdateBuilder(schemaManager, expressionBuilder, _sqlWriter));
			_analizers.Add(ExpressionAnalizerType.FuncCall, new QueryFuncCallBuilder(schemaManager, expressionBuilder));
		}
		public string AnalizeExpression(ICommandPreparatorsHolder commandPreparators, Expression expression, ExpressionAnalizerType expressionType, bool useAliases)
		{
			return _analizers[expressionType].BuildSql(commandPreparators, expression, useAliases);
		}
	}
}
