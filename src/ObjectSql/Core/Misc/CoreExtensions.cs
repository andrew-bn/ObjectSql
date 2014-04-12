using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ObjectSql.Core.Bo;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;
using ObjectSql.Core.QueryBuilder.ExpressionsAnalizers;
using ObjectSql.Core.QueryParts;
using ObjectSql.QueryInterfaces;

namespace ObjectSql.Core.Misc
{
	public static class CoreExtensions
	{
		public static Expression StripConvert(this Expression exp)
		{
			if (exp.NodeType == ExpressionType.Convert)
				return ((UnaryExpression) exp).Operand;
			return exp;
		}

		public static T MoveBackAndFind<T>(this List<T> enumerable, T obj, Func<T,bool> predicate)
		{
			if (enumerable == null)
				return default(T);
			var indexOfObj = enumerable.IndexOf(obj);
			for (int i = indexOfObj - 1; i >= 0; i--)
			{
				if (enumerable[i] is NextQueryPart)
					return default(T);
				if (predicate(enumerable[i]))
					return enumerable[i];
			}
			return default(T);
		}

		public static Expression Visit<T1>(this Expression exp, Func<ExpressionVisitor, T1, Expression> visitor1) where T1:Expression
		{
			return new ExpressionVisitorManager(visitor1).Visit(exp);
		}

		public static bool ContainsSql(this Expression exp)
		{
			bool hasSql = false;
			exp.Visit<ParameterExpression>((v, e) => { hasSql = true; return e; });

			if (!hasSql)
				exp.Visit<MemberExpression>((v, e) => { hasSql = e.Member.DeclaringType.GetCustomAttribute(typeof(DatabaseExtensionAttribute)) != null; return e; });

			if (!hasSql)
				exp.Visit<MethodCallExpression>((v, e) => { hasSql = e.Method.DeclaringType.GetCustomAttribute(typeof(DatabaseExtensionAttribute)) != null; return e; });

			if (!hasSql)
				exp.Visit<ConstantExpression>((v, e) => { hasSql = e.Type.GetCustomAttribute(typeof (DatabaseExtensionAttribute)) != null; return e; });

			return hasSql;
		}
	}
}
