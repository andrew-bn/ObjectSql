﻿using System.Linq;
using System.Linq.Expressions;
using ObjectSql.Core.Bo;

namespace ObjectSql.Core.QueryParts
{
	public class UpdatePart: LambdaBasedQueryPart
	{
		public UpdatePart(LambdaExpression expression)
			:base(expression)
		{
			
		}


		public override void BuildPart(Bo.BuilderContext context)
		{
			var entity = Expression.ReturnType;
			var sql = context.AnalizeExpression(Expression.Parameters.ToArray(), Expression.Body, ExpressionAnalizerType.FieldsUpdate);
			context.SqlWriter.WriteUpdate(context.Text, GetSchema(entity,context), sql);
		}
	}
}
