using System.Linq.Expressions;

namespace SqlBoost.Core.QueryBuilder.ExpressionsAnalizers
{
	public interface ISqlQueryBuilder
	{
		string BuildSql(ICommandPreparatorsHolder commandPreparators, Expression expression,bool useAliases);
	}
}
