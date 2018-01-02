using System;
using System.Collections.Generic;
using System.Text;
using ObjectSql.QueryInterfaces;

namespace ObjectSql.MySql
{
	[DatabaseExtension]
	public class MySql
	{
		private MySql() { }
		[Sql("UPPER({0})")]
		public static string Upper(string characterExpression) { return default(string); }
		[Sql("LOWER({0})")]
		public static string Lower(string characterExpression) { return default(string); }
		[Sql("MID({0}, {1}, {2})")]
		public static string Substring(string characterExpression, int start, int end) { return default(string); }
		[Sql("REPLACE({0}, {1}, {2})")]
		public static string Replace(string target, string pattern, string replacement) { return default(string); }
		[Sql("NOW()")]
		public static DateTime GetDate() { return default(DateTime); }
		[Sql("{0} IS NULL")]
		public static bool IsNull(object target) { return default(bool); }
		[Sql("{0} IS NOT NULL")]
		public static bool IsNotNull(object target) { return default(bool); }
	}
}
