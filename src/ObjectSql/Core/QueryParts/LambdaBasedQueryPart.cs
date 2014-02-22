using ObjectSql.Core.Bo;
using ObjectSql.Core.Misc;
using System.Linq.Expressions;

namespace ObjectSql.Core.QueryParts
{
	public abstract class LambdaBasedQueryPart: QueryPart
	{
		public LambdaExpression Expression { get; private set; }
		protected LambdaBasedQueryPart(LambdaExpression expression)
		{
			Expression = expression;
		}

		public override void CalculateQueryExpressionParameters(ref QueryRoots parameters)
		{
			base.CalculateQueryExpressionParameters(ref parameters);
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(Expression, ref parameters);
		}

		public override bool IsEqualTo(QueryPart part, ref QueryRoots rootsA, ref QueryRoots rootsB)
		{
			return base.IsEqualTo(part, ref rootsA, ref rootsB) && 
				ExpressionComparer.AreEqual(Expression, ref rootsA, ((LambdaBasedQueryPart)part).Expression, ref rootsB);
		}
	}
}
