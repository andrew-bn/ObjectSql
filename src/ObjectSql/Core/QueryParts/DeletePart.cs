using System;
using ObjectSql.Core.Bo;

namespace ObjectSql.Core.QueryParts
{
	public class DeletePart : QueryPart
	{
		public Type Entity { get; private set; }
		public DeletePart(Type entity)
		{
			Entity = entity;
		}

		public override void CalculateQueryExpressionParameters(ref QueryRoots parameters)
		{
			base.CalculateQueryExpressionParameters(ref parameters);
			parameters.Hash *= PRIME;
			parameters.Hash ^= Entity.GetHashCode();
		}
		public override bool IsEqualTo(QueryPart part, ref QueryRoots rootsA, ref QueryRoots rootsB)
		{
			return base.IsEqualTo(part, ref rootsA, ref rootsB) && Entity == ((DeletePart)part).Entity;
		}
		public override void BuildPart(BuilderContext context)
		{
			context.SqlWriter.WriteDelete(context.Text, GetSchema(Entity,context));
		}
	}
}
