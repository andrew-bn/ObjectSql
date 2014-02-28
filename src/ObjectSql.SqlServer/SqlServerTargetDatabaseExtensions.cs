using System;
using ObjectSql.Core;
using ObjectSql.QueryInterfaces;

namespace ObjectSql.SqlServer
{
	public class MsSql : DatabaseExtension
	{
		private MsSql()
		{
		}

		public static long CountBig(params object[] columnSelector)
		{
			return default(long);
		}
		internal static string RenderCountBig(ICommandPreparatorsHolder commandPreparators, string[] parts)
		{
			return BuildSql("COUNT_BIG", parts);
		}
		public static string Upper(string characterExpression)
		{
			return default(string);
		}
		internal static string RenderUpper(ICommandPreparatorsHolder commandPreparators, string[] parts)
		{
			return BuildSql("UPPER", parts);
		}
		public static string Lower(string characterExpression)
		{
			return default(string);
		}
		internal static string RenderLower(ICommandPreparatorsHolder commandPreparators, string[] parts)
		{
			return BuildSql("LOWER", parts);
		}
		public static string Substring(string characterExpression, int start, int end)
		{
			return default(string);
		}
		internal static string RenderSubstring(ICommandPreparatorsHolder commandPreparators, string[] parts)
		{
			return BuildSql("SUBSTRING", parts);
		}
		public static string Replace(string target, string pattern, string replacement)
		{
			return default(string);
		}
		internal static string RenderReplace(ICommandPreparatorsHolder commandPreparators, string[] parts)
		{
			return BuildSql("REPLACE", parts);
		}

		public static Interval Day()
		{
			return default(Interval);
		}

		internal static string RenderDay(ICommandPreparatorsHolder commandPreparators, string[] parts)
		{
			return "day";
		}

		public static int DateDiff(Interval interval, DateTime? date1, DateTime? date2)
		{
			return default(int);
		}

		internal static string RenderDateDiff(ICommandPreparatorsHolder commandPreparators, string[] parts)
		{
			return BuildSql("DATEDIFF", parts);
		}

		public static DateTime GetDate()
		{
			return default(DateTime);
		}

		internal static string RenderGetDate(ICommandPreparatorsHolder commandPreparators, string[] parts)
		{
			return "GETDATE()";
		}

		private static string BuildSql(string method, string[] parts)
		{
			return string.Format(" {0}({1}) ", method, string.Join(", ", parts));
		}
	}
}
