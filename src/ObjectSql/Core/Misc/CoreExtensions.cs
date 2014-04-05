using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ObjectSql.Core.Bo;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;

namespace ObjectSql.Core.Misc
{
	public static class CoreExtensions
	{
		public static InsertionParameterPrePostProcessor AsInsertion(this CommandPrePostProcessor descriptor)
		{
			return (InsertionParameterPrePostProcessor)descriptor;
		}
		public static CommandParameterPreProcessor AsDatabaseParameter(this CommandPrePostProcessor descriptor)
		{
			return (CommandParameterPreProcessor)descriptor;
		}
		public static StoredProcedureOutParameterProcessor AsStoredProcedureOutParameterProcessor(this CommandPrePostProcessor descriptor)
		{
			return (StoredProcedureOutParameterProcessor)descriptor;
		}

		public static CommandParameterPreProcessor AsSingleParameter(this CommandPrePostProcessor descriptor)
		{
			return (CommandParameterPreProcessor) descriptor;
		}
		public static Expression StripConvert(this Expression exp)
		{
			if (exp.NodeType == ExpressionType.Convert)
				return ((UnaryExpression) exp).Operand;
			return exp;
		}

		public static T MoveBackAndFind<T>(this List<T> enumerable, T obj, Func<T,bool> predicate)
		{
			var indexOfObj = enumerable.IndexOf(obj);
			for (int i = indexOfObj - 1; i >= 0; i--)
			{
				if (predicate(enumerable[i]))
					return enumerable[i];
			}
			return default(T);
		}

		public static Expression Visit<T1>(this Expression exp, Func<ExpressionVisitor, T1, Expression> visitor1) where T1:Expression
		{
			return new ExpressionVisitorManager(visitor1).Visit(exp);
		}
		public static int IndexOfRoot(this Expression exp, QueryRoots roots)
		{
			int index = -1;
			exp.Visit<ConstantExpression>((v, e) => { index = roots.IndexOf(e.Value); return e; });
			return index;
		}
	}
}
