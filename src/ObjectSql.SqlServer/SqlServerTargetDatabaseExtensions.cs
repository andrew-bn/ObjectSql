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

		public static string HoldLock { get; } = "HOLDLOCK";
		public static string NoLock { get; } = "NOLOCK";
		public static string NoWait { get; } = "NOWAIT";
		public static string PagLock { get; } = "PAGLOCK";
		public static string ReadCommitted { get; } = "READCOMMITTED";
		public static string ReadCommittedLock { get; } = "READCOMMITTEDLOCK";
		public static string ReadPast { get; } = "READPAST";
		public static string RepeatableRead { get; } = "REPEATABLEREAD";
		public static string RowLock { get; } = "ROWLOCK";
		public static string Serializable { get; } = "SERIALIZABLE";
		public static string Snapshot { get; } = "SNAPSHOT";
		public static string TabLock { get; } = "TABLOCK";
		public static string TabLockX { get; } = "TABLOCKX";
		public static string UpdLock { get; } = "UPDLOCK";
		public static string XLock { get; } = "XLOCK";
		internal const string WithStatement = "WITH ";

		public static string WithHint(params string[] hints)
		{
			return $"WITH ({string.Join(", ", hints)})";
		}

		[Sql("OBJECT_ID({0})")]
		public static int ObjectId(string objectName) { return 0; }
		public static long CountBig(params object[] columnSelector) { return default(long); }
		[Sql("UPPER({0})")]
		public static string Upper(string characterExpression) { return default(string); }
		[Sql("LOWER({0})")]
		public static string Lower(string characterExpression) { return default(string); }
		[Sql("SUBSTRING({0}, {1}, {2})")]
		public static string Substring(string characterExpression, int start, int end) { return default(string); }
		[Sql("REPLACE({0}, {1}, {2})")]
		public static string Replace(string target, string pattern, string replacement){ return default(string); }
		[Sql("DATEDIFF({0}, {1}, {2})")]
		public static int DateDiff(DatePart datePart, DateTime? date1, DateTime? date2) { return default(int); }
		[Sql("GETDATE()")]
		public static DateTime GetDate() { return default(DateTime); }
		[Sql("SCOPE_IDENTITY()")]
		public static decimal ScopeIdentity() { return default(decimal); }
		[Sql("{0} IS NULL")]
		public static bool IsNull(object target) { return default(bool); }
		[Sql("{0} IS NOT NULL")]
		public static bool IsNotNull(object target) { return default(bool); }
	}
}
