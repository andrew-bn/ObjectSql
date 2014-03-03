using System.Linq.Expressions;
using ObjectSql.Core.Bo;

namespace ObjectSql.Core.QueryBuilder.ExpressionsAnalizers
{
	public interface ISqlQueryBuilder
	{
		string BuildSql(BuilderContext builderContext,ParameterExpression[] parameters, Expression expression, bool useAliases);
	}
}
