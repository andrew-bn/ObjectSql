using System;
using System.Data;
using ObjectSql.Core;
using ObjectSql.Core.Bo;
using ObjectSql.QueryInterfaces;

namespace ObjectSql.SqlServer
{

	[DatabaseExtension]
	public class MsSql
	{
		
		private MsSql()
		{
		}

		public static long CountBig(params object[] columnSelector)
		{
			return default(long);
		}
		internal static string RenderCountBig(BuilderContext commandPreparators, string[] parts)
		{
			return BuildSql("COUNT_BIG", parts);
		}
		public static string Upper(string characterExpression)
		{
			return default(string);
		}
		internal static string RenderUpper(BuilderContext commandPreparators, string[] parts)
		{
			return BuildSql("UPPER", parts);
		}
		public static string Lower(string characterExpression)
		{
			return default(string);
		}
		internal static string RenderLower(BuilderContext commandPreparators, string[] parts)
		{
			return BuildSql("LOWER", parts);
		}
		public static string Substring(string characterExpression, int start, int end)
		{
			return default(string);
		}
		internal static string RenderSubstring(BuilderContext commandPreparators, string[] parts)
		{
			return BuildSql("SUBSTRING", parts);
		}
		public static string Replace(string target, string pattern, string replacement)
		{
			return default(string);
		}
		internal static string RenderReplace(BuilderContext commandPreparators, string[] parts)
		{
			return BuildSql("REPLACE", parts);
		}

		[DatabaseTypes("","DateTime2","DateTime2", "Int")]
		public static int DateDiff(DatePart datePart, DateTime? date1, DateTime? date2)
		{
			return default(int);
		}

		internal static string RenderDateDiff(BuilderContext commandPreparators, string[] parts)
		{
			return BuildSql("DATEDIFF", parts);
		}

		public static DateTime GetDate()
		{
			return default(DateTime);
		}

		internal static string RenderGetDate(BuilderContext commandPreparators, string[] parts)
		{
			return "GETDATE()";
		}

		private static string BuildSql(string method, string[] parts)
		{
			return string.Format(" {0}({1}) ", method, string.Join(", ", parts));
		}
	}
}
