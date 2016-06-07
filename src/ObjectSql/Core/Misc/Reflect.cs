using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectSql.Core.Misc
{
	public static class Reflect
	{
#if !NET40
		public static bool IsAssignableFro(this Type type, Type type2)
		{
			return type.GetTypeInfo().IsAssignableFrom(type2.GetTypeInfo());
		}
#endif
#if NET40
		public static object GetCustomAttr(this Type mi, Type attrType)
		{
			var attrs = mi.GetCustomAttributes(attrType, true);
			return attrs.FirstOrDefault();
		}
#else
		public static object GetCustomAttr(this Type mi, Type attrType)
		{
			var attrs = mi.GetTypeInfo().GetCustomAttributes(attrType, true);
			return attrs.FirstOrDefault();
		}
#endif

		public static PropertyInfo[] GetProps(this Type type)
		{
			return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase )
							.Where(p=>p.GetGetMethod().GetParameters().Length == 0)
							.ToArray();
		}

		public static PropertyInfo GetProp(this Type type, string name)
		{
			return type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
		}

 
		public static object GetCustomAttr(this MemberInfo mi, Type attrType)
		{
			var attrs = mi.GetCustomAttributes(attrType,true);
			return attrs.FirstOrDefault();
		}
		public static object GetCustomAttr(this MethodInfo mi, Type attrType)
		{
			var attrs = mi.GetCustomAttributes(attrType, true);
			return attrs.FirstOrDefault();
		}
		public static T GetCustomAttr<T>(this MethodInfo mi) where T: class
		{
			return mi.GetCustomAttr(typeof (T)) as T;
		}
		public static object GetCustomAttr(this ParameterInfo mi, Type attrType)
		{
			var attrs = mi.GetCustomAttributes(attrType, true);
			return attrs.FirstOrDefault();
		}

		public static bool IsEnum(this Type type)
		{
#if NET40
			return type.IsEnum;
#else
			return type.GetTypeInfo().IsEnum;
#endif

		}

		public static bool IsGenericType(this Type type)
		{
#if NET40
			return type.IsGenericType;
#else
			return type.GetTypeInfo().IsGenericType;
#endif
		}


		public static bool IsValueType(this Type type)
		{
#if NET40
			return type.IsValueType;
#else
			return type.GetTypeInfo().IsValueType;
#endif
		}

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
