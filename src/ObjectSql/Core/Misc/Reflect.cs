using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectSql.Core.Misc
{
	public static class Reflect
	{
#if NET40
		public static object GetCustomAttribute(this MemberInfo mi, Type attrType)
		{

			var attrs = mi.GetCustomAttributes(attrType,true);
			return attrs.Length > 0 ? attrs[0] : null;
		}
		public static object GetCustomAttribute(this MethodInfo mi, Type attrType)
		{
			var attrs = mi.GetCustomAttributes(attrType, true);
			return attrs.Length > 0 ? attrs[0] : null;
		}
		public static T GetCustomAttribute<T>(this MethodInfo mi) where T: class
		{
			return mi.GetCustomAttribute(typeof (T)) as T;
		}
		public static object GetCustomAttribute(this ParameterInfo mi, Type attrType)
		{
			var attrs = mi.GetCustomAttributes(attrType, true);
			return attrs.Length > 0 ? attrs[0] : null;
		}
#endif
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
