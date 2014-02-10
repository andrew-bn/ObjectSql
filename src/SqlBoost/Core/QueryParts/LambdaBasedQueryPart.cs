using SqlBoost.Core.Bo;
using SqlBoost.Core.Misc;
using System.Linq.Expressions;

namespace SqlBoost.Core.QueryParts
{
	internal abstract class LambdaBasedQueryPart: QueryPartBase
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

		public override bool IsEqualTo(IQueryPart part, ref QueryRoots rootsA, ref QueryRoots rootsB)
		{
			return base.IsEqualTo(part, ref rootsA, ref rootsB) && 
				ExpressionComparer.AreEqual(Expression, ref rootsA, ((LambdaBasedQueryPart)part).Expression, ref rootsB);
		}
	}
}
