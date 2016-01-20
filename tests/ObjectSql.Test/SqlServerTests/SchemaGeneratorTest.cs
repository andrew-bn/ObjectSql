using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectSql.Test;
using Xunit;

namespace ObjectSql.Tests.SqlServerTests
{
	public class SchemaGeneratorTest : TestBase
	{
		[Fact]
		public void Foo()
		{
			var result = SqlServer.Schema.SchemaGenerator.Generate("adf", "CsName", ConnectionString);

			//Assert.Fail(result);
		}
	}
}
