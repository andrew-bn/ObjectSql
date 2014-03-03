using System.Linq;
using System.Linq.Expressions;
using ObjectSql.Core.Bo;

namespace ObjectSql.Core.QueryParts
{
	public class JoinPart: LambdaBasedQueryPart
	{
		public JoinPart(LambdaExpression expression)
			:base(expression)
		{
			
		}

		public override void BuildPart(BuilderContext context)
		{
			var joinToTable = Expression.Parameters.Last().Type;
			var sql = context.AnalizeExpression(Expression.Parameters.ToArray(), Expression.Body, ExpressionAnalizerType.Expression, true);
			context.SqlWriter.WriteJoin(context.Text, GetSchema(joinToTable,context), Expression.Parameters.Last().Name, sql);
		}
	}
}
