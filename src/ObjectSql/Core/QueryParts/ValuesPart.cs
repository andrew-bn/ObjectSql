using System;
using ObjectSql.Core.Bo;

namespace ObjectSql.Core.QueryParts
{
	public class ValuesPart: QueryPartBase
	{
		public object[] Values { get; private set; }
		public Type Type { get; private set; }
		public ValuesPart(Type type, object[] values)
		{
			Values = values;
			Type = type;
		}
		public override QueryPartType PartType
		{
			get { return QueryPartType.Values; }
		}
		public override void CalculateQueryExpressionParameters(ref QueryRoots parameters)
		{
			base.CalculateQueryExpressionParameters(ref parameters);
			parameters.AddRoot(Values);
		}
	}
}
