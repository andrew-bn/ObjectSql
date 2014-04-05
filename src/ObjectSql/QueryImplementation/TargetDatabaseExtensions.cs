using System;
using System.Linq;
using ObjectSql.Core;
using ObjectSql.Core.Bo;
using ObjectSql.QueryInterfaces;

namespace ObjectSql
{
	[DatabaseExtension]
	public static class Sql
	{
		public static IQuery Query { get { return null; } }

		public static bool Like(this string expression,string pattern) { return false; }
		public static bool NotLike(this string expression, string pattern) { return false; }
		public static int Avg(short columnSelector) { return default(int); }
		public static int Avg(short? columnSelector) { return default(int); }
		public static int Avg(byte columnSelector) { return default(int); }
		public static int Avg(byte? columnSelector) { return default(int); }
		public static int Avg(int columnSelector) { return default(int); }
		public static int Avg(int? columnSelector) { return default(int); }
		public static long Avg(long columnSelector) { return default(int); }
		public static long Avg(long? columnSelector) { return default(int); }
		public static decimal Avg(decimal columnSelector) { return default(int); }
		public static decimal Avg(decimal? columnSelector) { return default(int); }
		public static float Avg(float columnSelector) { return default(int); }
		public static float Avg(float? columnSelector) { return default(int); }
		public static float Avg(double columnSelector) { return default(int); }
		public static float Avg(double? columnSelector) { return default(int); }

		public static T Max<T>(T columnSelector){ return default(T); }
		public static T Min<T>(T columnSelector) { return default(T);}

		public static int Count(params object[] columnSelector) { return default(int);}

		public static bool NotIn<T, TEntity>(this T field, IQueryEnd<TEntity> query) { return false; }
		public static bool NotIn<T, TEntity>(this T field, params TEntity[] enumeration) { return false; }

		public static bool In<T,TEntity>(this T field, IQueryEnd<TEntity> query) { return false; }
		public static bool In<T, TEntity>(this T field, params TEntity[] enumeration) { return false; }
	}
}
