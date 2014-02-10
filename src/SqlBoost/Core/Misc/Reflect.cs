using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SqlBoost.Core.Misc
{
	internal static class Reflect
	{

		public static ConstructorInfo FindCtor<T>(Expression<Func<T>> ctor)
		{
			return ((NewExpression)ctor.Body).Constructor;
		}
		public static MethodInfo FindMethod(Expression<Func<object>> func)
		{
			return ((MethodCallExpression)RemoveConvert(func.Body)).Method;
		}
		public static MethodInfo FindMethod(Expression<Action> func)
		{
			return ((MethodCallExpression)RemoveConvert(func.Body)).Method;
		}
		public static MethodInfo FindMethod<T>(Expression<Func<T, object>> func)
		{
			return ((MethodCallExpression)RemoveConvert(func.Body)).Method;
		}
		public static MethodInfo FindMethod<T>(Expression<Action<T>> func)
		{
			return ((MethodCallExpression)RemoveConvert(func.Body)).Method;
		}
		public static MemberInfo FindProperty<T>(Expression<Func<T, object>> prop)
		{
			return ((MemberExpression)RemoveConvert(prop.Body)).Member;
		}
		public static MemberInfo FindProperty(Expression<Func<object>> prop)
		{
			return ((MemberExpression)RemoveConvert(prop.Body)).Member;
		}
		private static Expression RemoveConvert(Expression exp)
		{
			return exp.NodeType == ExpressionType.Convert
								 ? ((UnaryExpression)exp).Operand
								 : exp;
		}
	}
}
