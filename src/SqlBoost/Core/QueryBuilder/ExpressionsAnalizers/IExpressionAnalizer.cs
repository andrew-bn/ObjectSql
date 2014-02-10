using System.Linq.Expressions;
using SqlBoost.Core.Bo;

namespace SqlBoost.Core.QueryBuilder.ExpressionsAnalizers
{
	internal interface IExpressionAnalizer
	{
		string AnalizeExpression(ICommandPreparatorsHolder commandPreparators, Expression expression, ExpressionAnalizerType expressionType, bool useAliases);
	}
}
