using NUnit.Framework;
using ObjectSql.SqlServer;
using System.Data;

namespace ObjectSql.Tests.SqlServerTests
{
	[TestFixture]
	public class SqlServerTargetDatabaseExtensionsTests : TestBase
	{
		[Test]
		public void Count_Big()
		{
			EfQuery.From<Category>().Select(c => MsSql.CountBig(c.CategoryID))
				 .Verify("SELECT COUNT_BIG([c].[CategoryID])FROM[dbo].[Categories]AS[c]");

		}
		[Test]
		public void Lower()
		{
			EfQuery.From<Category>().Select(c => MsSql.Lower(c.CategoryName))
				 .Verify("SELECT LOWER([c].[CategoryName])FROM[dbo].[Categories]AS[c]");
		}
		[Test]
		public void Lower2()
		{
			EfQuery.From<Category>().Select(c => c.CategoryName.ToLower())
				 .Verify("SELECT LOWER([c].[CategoryName])FROM[dbo].[Categories]AS[c]");
		}
		[Test]
		public void Replace()
		{
			EfQuery.From<Category>().Select(c => MsSql.Replace(c.Description, "p", "c"))
				 .Verify("SELECT REPLACE([c].[Description],@p0,@p1)FROM[dbo].[Categories]AS[c]",
				 "p".DbType(SqlDbType.NVarChar), "c".DbType(SqlDbType.NVarChar));
		}
		[Test]
		public void Substring()
		{
			EfQuery.From<Category>().Select(c => MsSql.Substring(c.Description, 1, 2))
				 .Verify("SELECT SUBSTRING([c].[Description],@p0,@p1)FROM[dbo].[Categories]AS[c]",
				 1.DbType(SqlDbType.Int), 2.DbType(SqlDbType.Int));
		}
		[Test]
		public void Upper()
		{
			EfQuery.From<Category>().Select(c => MsSql.Upper(c.Description))
				 .Verify("SELECT UPPER([c].[Description])FROM[dbo].[Categories]AS[c]");
		}
		[Test]
		public void Upper2()
		{
			EfQuery.From<Category>().Select(c => c.Description.ToUpper())
				 .Verify("SELECT UPPER([c].[Description])FROM[dbo].[Categories]AS[c]");
		}
		[Test]
		public void DateDiff_GetDate()
		{
			EfQuery.From<Employee>().Select(c => MsSql.DateDiff(DatePart.Day, c.BirthDate, MsSql.GetDate()))
				 .Verify("SELECT DATEDIFF(day,[c].[BirthDate],GETDATE())FROM[dbo].[Employees]AS[c]");
		}
	}
}
