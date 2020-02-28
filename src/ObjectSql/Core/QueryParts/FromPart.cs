using System;
using ObjectSql.Core.Bo;

namespace ObjectSql.Core.QueryParts
{
	public class FromPart : QueryPart
	{
		public Type Entity { get; }

		public object[] Addition { get; }

		public FromPart(Type entity, params object[] addition)
		{
			Entity = entity;
			Addition = addition;
		}

		public override void BuildPart(BuilderContext context)
		{
			context.SqlWriter.Write(context, this);
		}

		public override void CalculateQueryExpressionParameters(ref QueryRoots parameters)
		{
			base.CalculateQueryExpressionParameters(ref parameters);
			parameters.Hash *= PRIME;
			parameters.Hash ^= Entity.GetHashCode();
		}
		public override bool IsEqualTo(QueryPart part, ref QueryRoots rootsA, ref QueryRoots rootsB)
		{
			if (!base.IsEqualTo(part, ref rootsA, ref rootsB))
				return false;
			var b = (FromPart)part;
			if (Entity != b.Entity)
				return false;
			return true;
		}
	}
}
