using SqlBoost.Core;
using SqlBoost.QueryInterfaces;

namespace SqlBoost
{
	public static class TargetDatabaseExtensions
	{
		public static bool Like(this ITargetDatabase targetDatabase, string expression,string pattern) { return false; }
		internal static string RenderLike(ICommandPreparatorsHolder commandPreparators, string[] parts)
		{
			return string.Format(" ({0} LIKE {1})", parts[0], parts[1]);
		}
		public static bool NotLike(this ITargetDatabase targetDatabase, string expression, string pattern) { return false; }
		internal static string RenderNotLike(ICommandPreparatorsHolder commandPreparators, string[] parts)
		{
			return string.Format(" ({0} NOT LIKE {1})", parts[0], parts[1]);
		}
		public static int Avg(this ITargetDatabase targetDatabase, short columnSelector) { return default(int); }
		public static int Avg(this ITargetDatabase targetDatabase, short? columnSelector) { return default(int); }
		public static int Avg(this ITargetDatabase targetDatabase, byte columnSelector) { return default(int); }
		public static int Avg(this ITargetDatabase targetDatabase, byte? columnSelector) { return default(int); }
		public static int Avg(this ITargetDatabase targetDatabase, int columnSelector) { return default(int); }
		public static int Avg(this ITargetDatabase targetDatabase, int? columnSelector) { return default(int); }
		public static long Avg(this ITargetDatabase targetDatabase, long columnSelector) { return default(int); }
		public static long Avg(this ITargetDatabase targetDatabase, long? columnSelector) { return default(int); }
		public static decimal Avg(this ITargetDatabase targetDatabase, decimal columnSelector) { return default(int); }
		public static decimal Avg(this ITargetDatabase targetDatabase, decimal? columnSelector) { return default(int); }
		public static float Avg(this ITargetDatabase targetDatabase, float columnSelector) { return default(int); }
		public static float Avg(this ITargetDatabase targetDatabase, float? columnSelector) { return default(int); }
		public static float Avg(this ITargetDatabase targetDatabase, double columnSelector) { return default(int); }
		public static float Avg(this ITargetDatabase targetDatabase, double? columnSelector) { return default(int); }
		internal static string RenderAvg(ICommandPreparatorsHolder commandPreparators, string[] parts)
		{
			return BuildSql("AVG", parts);
		}
		public static T Max<T>(this ITargetDatabase targetDatabase, T columnSelector)
		{
			return default(T);
		}
		internal static string RenderMax(ICommandPreparatorsHolder commandPreparators, string[] parts)
		{
			return BuildSql("MAX", parts);
		}
		public static T Min<T>(this ITargetDatabase targetDatabase, T columnSelector)
		{
			return default(T);
		}
		internal static string RenderMin(ICommandPreparatorsHolder commandPreparators, string[] parts)
		{
			return BuildSql("MIN", parts);
		}
		public static int Count(this ITargetDatabase targetDatabase, params object[] columnSelector)
		{
			return default(int);
		}
		internal static string RenderCount(ICommandPreparatorsHolder commandPreparators, string[] parts)
		{
			return BuildSql("COUNT", parts);
		}
		private static string BuildSql(string method, string[] parts)
		{
			return string.Format(" {0}({1}) ",method, string.Join(", ", parts[0]));
		}
	}
}
