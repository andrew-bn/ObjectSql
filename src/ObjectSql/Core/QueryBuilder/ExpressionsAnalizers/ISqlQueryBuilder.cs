using System.Linq.Expressions;
using ObjectSql.Core.Bo;
using ObjectSql.Core.SchemaManager.EntitySchema;

namespace ObjectSql.Core.QueryBuilder.ExpressionsAnalizers
{
	public interface ISqlQueryBuilder
	{
		IStorageFieldType DbTypeInContext { get; set; }
		string BuildSql(BuilderContext builderContext,ParameterExpression[] parameters, Expression expression);
		string BuildSql(IStorageFieldType dbTypeInContext, Expression expression);
		string BuildSql(Expression expression);
	}
}
