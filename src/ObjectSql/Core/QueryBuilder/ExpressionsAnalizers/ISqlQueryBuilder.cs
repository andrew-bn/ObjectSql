using System.Linq.Expressions;

namespace ObjectSql.Core.QueryBuilder.ExpressionsAnalizers
{
	public interface ISqlQueryBuilder
	{
		string BuildSql(ICommandPreparatorsHolder commandPreparators, Expression expression,bool useAliases);
	}
}
