﻿using System.Linq.Expressions;
using ObjectSql.Core.Bo;

namespace ObjectSql.Core.QueryBuilder.ExpressionsAnalizers
{
	public interface IExpressionAnalizer
	{
		string AnalizeExpression(Expression expression, ExpressionAnalizerType expressionType, bool useAliases);
	}
}
