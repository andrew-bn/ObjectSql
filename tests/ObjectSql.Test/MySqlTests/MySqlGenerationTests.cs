using ObjectSql.SqlServer;
using System.Data;
using MySql.Data.MySqlClient;
using ObjectSql.Test;
using Xunit;
using ObjectSql.Test.Database.TestDatabase.dbo;
using TestDatabase.MySql;

namespace ObjectSql.Tests.MySqlTests
{
	public class MySqlGenerationTests : MySqlTestBase
	{
		[Fact]
		public void select()
		{
			Query.From<records>().Where(r=>r.id > 1).Select(r=>r)
				.MySqlVerify("SELECT `r`.`id`, `r`.`domain_id`, `r`.`name`, `r`.`type`, `r`.`content`, `r`.`ttl`, " +
				        "`r`.`prio`, `r`.`change_date`, `r`.`disabled`, `r`.`ordername`, `r`.`auth`, `r`.`fingerPrint` " +
				        "FROM `records` AS `r` " +
				        "WHERE  (`r`.`id` > @p0)", 1.MySqlDbType(MySqlDbType.Int32));
		}

		[Fact]
		public void select_specific()
		{
			Query.From<records>().Where(r => r.id > 1).Select(r => new{SomeId = r.id, SomeParam = r.change_date })
				.MySqlVerify("SELECT `r`.`id` AS `SomeId`, `r`.`change_date` AS `SomeParam` " +
				             "FROM `records` AS `r` WHERE  (`r`.`id` > @p0)", 1.MySqlDbType(MySqlDbType.Int32));
		}
	}
}
