using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ObjectSql.Core.Bo;
using ObjectSql.Core.SchemaManager.EntitySchema;
using ObjectSql.QueryInterfaces;

namespace ObjectSql.Core.QueryParts
{
	public abstract class QueryPart
	{
		public const int PRIME = 397;

		public virtual void CalculateQueryExpressionParameters(ref QueryRoots parameters)
		{
			parameters.Hash *= PRIME;
			parameters.Hash ^= GetType().GetHashCode();
		}

		public virtual bool IsEqualTo(QueryPart part, ref QueryRoots rootsA, ref QueryRoots rootsB)
		{
			return GetType() == part.GetType();
		}
		public virtual bool SortParts(BuilderContext parts)
		{
			return false;
		}
		public abstract void BuildPart(BuilderContext context);

		protected EntitySchema GetSchema(Type entityType,BuilderContext context)
		{
			return context.SchemaManager.GetSchema(entityType);
		}
	}
}
