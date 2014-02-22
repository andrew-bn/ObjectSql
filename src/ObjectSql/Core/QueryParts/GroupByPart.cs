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
			AppendAlias(Expression, context);
			var sql = context.ExpressionAnalizer.AnalizeExpression(context.Preparators, Expression.Body, ExpressionAnalizerType.FieldsSequence, true);
			context.SqlWriter.WriteGroupBy(context.Text, sql);
			context.State = BuilderState.GroupByGenerated;
		}
	}
}
