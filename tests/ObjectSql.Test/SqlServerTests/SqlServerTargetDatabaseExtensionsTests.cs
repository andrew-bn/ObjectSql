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
			Query.Select(()=> MsSql.ScopeIdentity())
				.Verify("SELECT SCOPE_IDENTITY()");
		}

		[Fact]
		public void Scope_Identity_with_alias()
		{
			Query.Select(() =>new { Id = MsSql.ScopeIdentity()})
				.Verify("SELECT SCOPE_IDENTITY() AS [Id]");
		}

		[Fact]
		public void Count_Big()
		{
			Query.From<Categories>().Select(c => MsSql.CountBig(c.CategoryID))
				 .Verify("SELECT COUNT_BIG([c].[CategoryID])FROM[dbo].[Categories]AS[c]");
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
