using System.Linq.Expressions;
using ObjectSql.Core.Bo;

namespace ObjectSql.Core.QueryParts
{
	public class WherePart: LambdaBasedQueryPart
	{
		public bool UseAliases { get; private set; }
		public WherePart(bool useAliases, LambdaExpression expression)
			:base(expression)
		{
			UseAliases = useAliases;
		}

		public override void BuildPart(BuilderContext context)
		{
			var index = context.Parts.IndexOf(this);
			var groupByGenerated = false;
			for (int i = index - 1; i >= 0; i--)
			{
				if (context.Parts[i] is NextQueryPart)
					break;
				if (context.Parts[i] is GroupByPart)
				{
					groupByGenerated = true;
					break;
				}
			}

			var sql = context.ExpressionAnalizer.AnalizeExpression(context.Preparators, Expression.Body, ExpressionAnalizerType.Expression, UseAliases);
			if (groupByGenerated)
				context.SqlWriter.WriteHaving(context.Text, sql);
			else
				context.SqlWriter.WriteWhere(context.Text, sql);
		}
		public override void CalculateQueryExpressionParameters(ref QueryRoots parameters)
		{
			base.CalculateQueryExpressionParameters(ref parameters);
			parameters.Hash *= PRIME;
			parameters.Hash ^= UseAliases ? 1 : 0;
		}
		public override bool IsEqualTo(QueryPart part, ref QueryRoots rootsA, ref QueryRoots rootsB)
		{
			return base.IsEqualTo(part, ref rootsA, ref rootsB) && UseAliases == ((WherePart)part).UseAliases;
		}
	}
}
