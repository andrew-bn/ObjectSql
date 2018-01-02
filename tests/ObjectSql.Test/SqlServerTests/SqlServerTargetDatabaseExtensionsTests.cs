using ObjectSql.SqlServer;
using System.Data;
using ObjectSql.Test;
using Xunit;
using ObjectSql.Test.Database.TestDatabase.dbo;

namespace ObjectSql.Tests.SqlServerTests
{
	public class SqlServerTargetDatabaseExtensionsTests : TestBase
	{
		[Fact]
		public void Scope_Identity()
		{
			Query.Select(() => MsSql.ScopeIdentity())
				.Verify("SELECT SCOPE_IDENTITY()");
		}

		[Fact]
		public void Is_Null()
		{
			Query.Select(() => MsSql.IsNull(1))
				.Verify("SELECT (@p0 IS NULL)", 1.DbType(SqlDbType.Int));
		}

		[Fact]
		public void Is_Not_Null()
		{
			Query.Select(() => MsSql.IsNotNull(1))
				.Verify("SELECT (@p0 IS NOT NULL)", 1.DbType(SqlDbType.Int));
		}

		[Fact]
		public void Is_Null_Of_Null_Value()
		{
			Query.Select(() => MsSql.IsNull(null))
				.Verify("SELECT (NULL IS NULL)");
		}

		[Fact]
		public void Is_Not_Null_Of_Null_Value()
		{
			Query.Select(() => MsSql.IsNotNull(null))
				.Verify("SELECT (NULL IS NOT NULL)");
		}

		[Fact]
		public void Is_Null_Of_Empty_Array()
		{
			var emptyArray = new int[0];
			Query.Select(() => MsSql.IsNull(emptyArray))
				.Verify("SELECT (1=0)");
		}

		[Fact]
		public void Is_Not_Null_Of_Empty_Array()
		{
			var emptyArray = new int[0];
			Query.Select(() => MsSql.IsNotNull(emptyArray))
				.Verify("SELECT (1=1)");
		}

		[Fact]
		public void Is_Null_Of_Not_Empty_Array()
		{
			var emptyArray = new[] { 1 };
			Query.Select(() => MsSql.IsNull(emptyArray))
				.Verify("SELECT (1=0)", 1.Name("p0_0"));
		}

		[Fact]
		public void Is_Not_Null_Of_Not_Empty_Array()
		{
			var emptyArray = new[] { 1 };
			Query.Select(() => MsSql.IsNotNull(emptyArray))
				.Verify("SELECT (1=1)", 1.Name("p0_0"));
		}

		[Fact]
		public void Is_Null_Array_Item()
		{
			var emptyArray = new[] { 1 };
			Query.Select(() => MsSql.IsNull(emptyArray[0]))
				.Verify("SELECT (@p0 IS NULL)", 1.Name("p0"));
		}

		[Fact]
		public void Is_Not_Null_Array_Item()
		{
			var emptyArray = new[] { 1 };
			Query.Select(() => MsSql.IsNotNull(emptyArray[0]))
				.Verify("SELECT (@p0 IS NOT NULL)", 1.Name("p0"));
		}

		[Fact]
		public void Is_Null_Array_Item2()
		{
			var emptyArray = new[] { 1 , 2, 3};
			Query.Select(() => MsSql.IsNull(emptyArray) && MsSql.IsNull(emptyArray[0]))
				.Verify("SELECT ((1=0) AND (@p1 IS NULL))",
				1.Name("p0_0"), 2.Name("p0_1"), 3.Name("p0_2"), 1.Name("p1"));
		}

		[Fact]
		public void Is_Not_Null_Array_Item2()
		{
			var emptyArray = new[] { 1, 2, 3 };
			Query.Select(() => MsSql.IsNotNull(emptyArray) && MsSql.IsNotNull(emptyArray[0]))
				.Verify("SELECT ((1=1) AND (@p1 IS NOT NULL))",
					1.Name("p0_0"), 2.Name("p0_1"), 3.Name("p0_2"), 1.Name("p1"));
		}

		[Fact]
		public void Scope_Identity_with_alias()
		{
			Query.Select(() => new { Id = MsSql.ScopeIdentity() })
				.Verify("SELECT SCOPE_IDENTITY() AS [Id]");
		}

		[Fact]
		public void Count_Big()
		{
			Query.From<Categories>().Select(c => MsSql.CountBig(c.CategoryID))
				 .Verify("SELECT COUNT_BIG([c].[CategoryID])FROM[dbo].[Categories]AS[c]");
		}

		[Fact]
		public void Is_Null_ext()
		{
			var p1 = "value";
			var p2 = "value2";

			Query.From<Categories>()
				.Where(c => (MsSql.IsNull(p1) || c.CategoryName == p1) && (MsSql.IsNull(p2) || c.Description == p2))
				.Select(c => MsSql.CountBig(c.CategoryID))
				 .Verify("SELECT COUNT_BIG([c].[CategoryID])" +
						 "FROM [dbo].[Categories] AS [c]" +
						 "WHERE(((@p0 IS NULL) OR ([c].[CategoryName]=@p0))AND" +
						 "((@p1 IS NULL) OR ([c].[Description]=@p2)))",
						 p1.DbType(SqlDbType.NVarChar),
						 p2.DbType(SqlDbType.NVarChar),
						 p2.DbType(SqlDbType.NText));
		}

		[Fact]
		public void Lower()
		{
			Query.From<Categories>().Select(c => MsSql.Lower(c.CategoryName))
				 .Verify("SELECT LOWER([c].[CategoryName])FROM[dbo].[Categories]AS[c]");
		}

		[Fact]
		public void Lower2()
		{
			Query.From<Categories>().Select(c => c.CategoryName.ToLower())
				 .Verify("SELECT LOWER([c].[CategoryName])FROM[dbo].[Categories]AS[c]");
		}

		[Fact]
		public void Replace()
		{
			Query.From<Categories>().Select(c => MsSql.Replace(c.Description, "p", "c"))
				 .Verify("SELECT REPLACE([c].[Description],@p0,@p1)FROM[dbo].[Categories]AS[c]",
				 "p".DbType(SqlDbType.NVarChar), "c".DbType(SqlDbType.NVarChar));
		}

		[Fact]
		public void Substring()
		{
			Query.From<Categories>().Select(c => MsSql.Substring(c.Description, 1, 2))
				 .Verify("SELECT SUBSTRING([c].[Description],@p0,@p1)FROM[dbo].[Categories]AS[c]",
				 1.DbType(SqlDbType.Int), 2.DbType(SqlDbType.Int));
		}

		[Fact]
		public void Upper()
		{
			Query.From<Categories>().Select(c => MsSql.Upper(c.Description))
				 .Verify("SELECT UPPER([c].[Description])FROM[dbo].[Categories]AS[c]");
		}

		[Fact]
		public void Upper2()
		{
			Query.From<Categories>().Select(c => c.Description.ToUpper())
				 .Verify("SELECT UPPER([c].[Description])FROM[dbo].[Categories]AS[c]");
		}
		[Fact]
		public void DateDiff_GetDate()
		{
			Query.From<Employees>().Select(c => MsSql.DateDiff(DatePart.Day, c.BirthDate, MsSql.GetDate()))
				 .Verify("SELECT DATEDIFF(day,[c].[BirthDate],GETDATE())FROM[dbo].[Employees]AS[c]");
		}
	}
}
