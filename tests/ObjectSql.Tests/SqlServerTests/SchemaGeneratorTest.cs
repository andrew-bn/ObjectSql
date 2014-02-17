using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ObjectSql.Tests.SqlServerTests
{
	[TestFixture]
	public class SchemaGeneratorTest:TestBase
	{
		[Test]
		public void Foo()
		{
			var result = SqlServer.Schema.SchemaGenerator.Generate("adf","CsName", ConnectionString);

			//Assert.Fail(result);
		}
	}
}
