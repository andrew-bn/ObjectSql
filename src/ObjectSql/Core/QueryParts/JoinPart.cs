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

		public override QueryPartType PartType
		{
			get { return QueryPartType.Join; }
		}

		public override void BuildPart(BuilderContext context)
		{
			AppendAlias(Expression, context);
			var joinToTable = Expression.Parameters.Last().Type;
			var sql = context.ExpressionAnalizer.AnalizeExpression(context.Preparators, Expression.Body, ExpressionAnalizerType.Expression, true);
			context.SqlWriter.WriteJoin(context.Text, GetSchema(joinToTable,context), Expression.Parameters.Last().Name, sql);
		}
	}
}
