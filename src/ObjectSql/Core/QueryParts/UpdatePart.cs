using System.Linq.Expressions;

namespace ObjectSql.Core.QueryParts
{
	public class UpdatePart: LambdaBasedQueryPart
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
