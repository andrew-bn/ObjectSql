using System.Linq.Expressions;

namespace SqlBoost.Core.QueryParts
{
	internal class UpdatePart: LambdaBasedQueryPart
	{
		public UpdatePart(LambdaExpression expression)
			:base(expression)
		{
			
		}

		public override QueryPartType PartType
		{
			get { return QueryPartType.Update; }
		}
	}
}
