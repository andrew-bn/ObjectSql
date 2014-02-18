﻿using System;
using System.Linq.Expressions;
using ObjectSql.Core.Bo;
using ObjectSql.Core.Bo.EntitySchema;
using ObjectSql.QueryInterfaces;

namespace ObjectSql.Core.QueryParts
{
	public abstract class QueryPartBase: IQueryPart
	{
		public const int PRIME = 397;
		public abstract QueryPartType PartType {get;}

		public virtual void CalculateQueryExpressionParameters(ref QueryRoots parameters)
		{
			parameters.Hash *= PRIME;
			parameters.Hash ^= (int)PartType;
		}

		public virtual bool IsEqualTo(IQueryPart part, ref QueryRoots rootsA, ref QueryRoots rootsB)
		{
			return PartType == part.PartType;
		}

		public virtual void BuildPart(BuilderContext context)
		{
		}
		protected EntitySchema GetSchema(Type entityType,BuilderContext context)
		{
			return context.SchemaManager.GetSchema(entityType);
		}
		protected void AppendAlias(LambdaExpression expression, BuilderContext context)
		{
			if (context.State == BuilderState.FromAliasNeeded &&
				context.State != BuilderState.FromAliasGenerated &&
				context.State != BuilderState.GroupByGenerated)
			{
				if (expression.Parameters[0].Type != typeof(DatabaseExtension))
					context.SqlWriter.WriteAlias(context.Text, expression.Parameters[0].Name);
				else context.SqlWriter.WriteAlias(context.Text, expression.Parameters[1].Name);
				
				context.State = BuilderState.FromAliasGenerated;
			}
		}
	}
}
