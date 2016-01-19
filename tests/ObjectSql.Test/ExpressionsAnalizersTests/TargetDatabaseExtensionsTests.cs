using System.Data;
using ObjectSql.Test;
using ObjectSql.Test.Database.TestDatabase.dbo;
using Xunit;

namespace ObjectSql.Tests.ExpressionsAnalizersTests
{
	public class TargetDatabaseExtensionsTests : TestBase
	{
		[Fact]
		public void Avg()
		{
			Query.From<Categories>().Select(c => Sql.Avg(c.CategoryID))
				 .Verify("SELECT AVG([c].[CategoryID])FROM[dbo].[Categories]AS[c]");
		}
		[Fact]
		public void Count()
		{
			Query.From<Categories>().Select(c => Sql.Count(c.CategoryID))
				 .Verify("SELECT COUNT([c].[CategoryID])FROM[dbo].[Categories]AS[c]");
		}
		[Fact]
		public void Min()
		{
			Query.From<Categories>().Select(c => Sql.Min(c.CategoryID))
				 .Verify("SELECT MIN([c].[CategoryID])FROM[dbo].[Categories]AS[c]");
		}
		[Fact]
		public void Max()
		{
			Query.From<Categories>().Select(c => Sql.Max(c.CategoryID))
				 .Verify("SELECT MAX([c].[CategoryID])FROM[dbo].[Categories]AS[c]");
		}
		[Fact]
		public void Like()
		{
			Query.From<Categories>().Select(c => c.Description.Like("Descr"))
				 .Verify("SELECT ([c].[Description] LIKE @p0) FROM[dbo].[Categories]AS[c]",
				 "Descr".DbType(SqlDbType.NVarChar));
		}
		[Fact]
		public void NotLike()
		{
			Query.From<Categories>().Select(c => c.Description.NotLike("Descr"))
				 .Verify("SELECT ([c].[Description] NOT LIKE @p0) FROM[dbo].[Categories]AS[c]",
				 "Descr".DbType(SqlDbType.NVarChar));
		}
	}
}
