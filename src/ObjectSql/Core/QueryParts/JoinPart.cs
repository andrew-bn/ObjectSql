using System.Linq;
using System.Linq.Expressions;
using ObjectSql.Core.Bo;

namespace ObjectSql.Core.QueryParts
{
	public class JoinPart: LambdaBasedQueryPart
	{
		private readonly JoinType _joinType;

		public JoinPart(LambdaExpression expression, JoinType joinType)
			:base(expression)
		{
			_joinType = joinType;
		}

		public override void CalculateQueryExpressionParameters(ref QueryRoots parameters)
		{
			base.CalculateQueryExpressionParameters(ref parameters);
			parameters.Hash *= PRIME;
			parameters.Hash ^= _joinType.GetHashCode();
		}

		public override bool IsEqualTo(QueryPart part, ref QueryRoots rootsA, ref QueryRoots rootsB)
		{
			return base.IsEqualTo(part, ref rootsA, ref rootsB) && _joinType == ((JoinPart)part)._joinType;
		}

		public override void BuildPart(BuilderContext context)
		{
			var joinToTable = Expression.Parameters.Last().Type;
			var sql = context.AnalizeExpression(Expression.Parameters.ToArray(), Expression.Body, ExpressionAnalizerType.Expression, true);

			context.SqlWriter.WriteJoin(context.Text, GetSchema(joinToTable, context), Expression.Parameters.Last().Name, sql, _joinType);
		}
	}
}
