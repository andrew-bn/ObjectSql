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
		private MsSql() { }

		public static long CountBig(params object[] columnSelector) { return default(long); }
		public static string Upper(string characterExpression) { return default(string); }
		public static string Lower(string characterExpression) { return default(string); }
		public static string Substring(string characterExpression, int start, int end) { return default(string); }
		public static string Replace(string target, string pattern, string replacement){ return default(string); }
		public static int DateDiff(DatePart datePart, DateTime? date1, DateTime? date2) { return default(int); }
		public static DateTime GetDate() { return default(DateTime); }
	}
}
