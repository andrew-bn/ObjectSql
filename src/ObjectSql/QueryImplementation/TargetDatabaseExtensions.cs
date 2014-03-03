using System;
using ObjectSql.Core;
using ObjectSql.Core.Bo;
using ObjectSql.QueryInterfaces;

namespace ObjectSql
{
	[DatabaseExtension]
	public static class Sql
	{
		public static IQuery Query
		{
			get { return null; }
		}

		public static bool Like(this string expression,string pattern) { return false; }
		internal static string RenderLike(BuilderContext commandPreparators, string[] parts)
		{
			return string.Format(" ({0} LIKE {1})", parts[0], parts[1]);
		}
		public static bool NotLike(this string expression, string pattern) { return false; }
		internal static string RenderNotLike(BuilderContext commandPreparators, string[] parts)
		{
			return string.Format(" ({0} NOT LIKE {1})", parts[0], parts[1]);
		}
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
		internal static string RenderAvg(BuilderContext commandPreparators, string[] parts)
		{
			return BuildSql("AVG", parts);
		}
		public static T Max<T>(T columnSelector)
		{
			return default(T);
		}
		internal static string RenderMax(BuilderContext commandPreparators, string[] parts)
		{
			return BuildSql("MAX", parts);
		}
		public static T Min<T>(T columnSelector)
		{
			return default(T);
		}
		internal static string RenderMin(BuilderContext commandPreparators, string[] parts)
		{
			return BuildSql("MIN", parts);
		}
		public static int Count(params object[] columnSelector)
		{
			return default(int);
		}
		internal static string RenderCount(BuilderContext commandPreparators, string[] parts)
		{
			return BuildSql("COUNT", parts);
		}
		public static bool NotIn<T, TEntity>(this T field, IQueryEnd<TEntity> query) { return false; }
		internal static string RenderNotIn(BuilderContext commandPreparators, string[] parts)
		{
			return string.Format(" ({0} NOT IN ({1})) ", parts[0], parts[1]);
		}
		public static bool In<T,TEntity>(this T field, IQueryEnd<TEntity> query) { return false; }
		internal static string RenderIn(BuilderContext commandPreparators, string[] parts)
		{
			return string.Format(" ({0} IN ({1})) ", parts[0],parts[1]);
		}

		private static string BuildSql(string method, string[] parts)
		{
			return string.Format(" {0}({1}) ",method, string.Join(", ", parts[0]));
		}
	}
}
