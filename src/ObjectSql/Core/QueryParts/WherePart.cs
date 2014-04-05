using System.Linq;
using System.Linq.Expressions;
using ObjectSql.Core.Bo;
using ObjectSql.Core.Misc;

namespace ObjectSql.Core.QueryParts
{
	public class WherePart: LambdaBasedQueryPart
	{
		public WherePart(LambdaExpression expression)
			:base(expression)
		{
		}

		public override void BuildPart(BuilderContext context)
		{
			var groupByPart = context.Parts.MoveBackAndFind(this, p => p is GroupByPart) as GroupByPart;
			var groupByGenerated = groupByPart != null;

			var sql = context.AnalizeExpression(Expression.Parameters.ToArray(), Expression.Body, ExpressionAnalizerType.Expression);
			if (groupByGenerated)
				context.SqlWriter.WriteHaving(context.Text, sql);
			else
				context.SqlWriter.WriteWhere(context.Text, sql);
		}
	}
}
