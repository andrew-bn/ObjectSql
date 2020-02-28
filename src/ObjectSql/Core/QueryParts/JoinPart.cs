using System.Linq;
using System.Linq.Expressions;
using ObjectSql.Core.Bo;

namespace ObjectSql.Core.QueryParts
{
	public class JoinPart: LambdaBasedQueryPart
	{
		public JoinType JoinType { get; }

		public object[] Additions { get; }


		public JoinPart(LambdaExpression expression, JoinType joinType, params object[] additions)
			:base(expression)
		{
			JoinType = joinType;
			Additions = additions;
		}

		public override void CalculateQueryExpressionParameters(ref QueryRoots parameters)
		{
			base.CalculateQueryExpressionParameters(ref parameters);
			parameters.Hash *= PRIME;
			parameters.Hash ^= JoinType.GetHashCode();
		}

		public override bool IsEqualTo(QueryPart part, ref QueryRoots rootsA, ref QueryRoots rootsB)
		{
			return base.IsEqualTo(part, ref rootsA, ref rootsB) && JoinType == ((JoinPart)part).JoinType;
		}

		public override void BuildPart(BuilderContext context)
		{
			context.SqlWriter.Write(context, this);
		}
	}
}
