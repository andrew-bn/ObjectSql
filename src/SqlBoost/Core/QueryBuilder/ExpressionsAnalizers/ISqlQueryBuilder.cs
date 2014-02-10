using System.Linq.Expressions;

namespace SqlBoost.Core.QueryBuilder.ExpressionsAnalizers
{
	internal interface ISqlQueryBuilder
	{
		string BuildSql(ICommandPreparatorsHolder commandPreparators, Expression expression,bool useAliases);
	}
}
