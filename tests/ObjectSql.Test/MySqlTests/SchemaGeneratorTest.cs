using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectSql.Test;
using Xunit;

namespace ObjectSql.Tests.MySqlTests
{
	public class SchemaGeneratorTest : TestBase
	{
		[Fact]
		public void Foo()
		{
			var result = MySql.Schema.SchemaGenerator.Generate("adf", "CsName", "Server=127.0.0.1;Database=rg_UserMessages;Uid=root;Pwd=mariadb;");

			//Assert.Fail(result);
		}
	}
}
