using NUnit.Framework;
using System.Data;

namespace ObjectSql.Tests.ExpressionsAnalizersTests.TardetDbExtensionsTests
{
	[TestFixture]
	public class TargetDatabaseExtensionsTests : TestBase
	{
		[Test]
		public void Avg()
		{
			EfQuery.From<Category>().Select(c => Sql.Avg(c.CategoryID))
				 .Verify("SELECT AVG([c].[CategoryID])FROM[dbo].[Categories]AS[c]");
		}
		[Test]
		public void Count()
		{
			EfQuery.From<Category>().Select(c => Sql.Count(c.CategoryID))
				 .Verify("SELECT COUNT([c].[CategoryID])FROM[dbo].[Categories]AS[c]");
		}
		[Test]
		public void Min()
		{
			EfQuery.From<Category>().Select(c => Sql.Min(c.CategoryID))
				 .Verify("SELECT MIN([c].[CategoryID])FROM[dbo].[Categories]AS[c]");
		}
		[Test]
		public void Max()
		{
			EfQuery.From<Category>().Select(c => Sql.Max(c.CategoryID))
				 .Verify("SELECT MAX([c].[CategoryID])FROM[dbo].[Categories]AS[c]");
		}
		[Test]
		public void Like()
		{
			EfQuery.From<Category>().Select(c => c.Description.Like("Descr"))
				 .Verify("SELECT ([c].[Description] LIKE @p0) FROM[dbo].[Categories]AS[c]",
				 "Descr".DbType(SqlDbType.NVarChar));
		}
		[Test]
		public void NotLike()
		{
			EfQuery.From<Category>().Select(c => c.Description.NotLike("Descr"))
				 .Verify("SELECT ([c].[Description] NOT LIKE @p0) FROM[dbo].[Categories]AS[c]",
				 "Descr".DbType(SqlDbType.NVarChar));
		}
	}
}
