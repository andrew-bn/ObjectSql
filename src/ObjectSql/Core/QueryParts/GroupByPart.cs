using System.Linq.Expressions;
using ObjectSql.Core.Bo;

namespace ObjectSql.Core.QueryParts
{
	public class GroupByPart:LambdaBasedQueryPart
	{
		public GroupByPart(LambdaExpression expression)
			:base(expression)
		{
			
		}

		public override void BuildPart(BuilderContext context)
		{
			var sql = context.AnalizeExpression(Expression.Body, ExpressionAnalizerType.FieldsSequence, true);
			context.SqlWriter.WriteGroupBy(context.Text, sql);
		}
	}
}
