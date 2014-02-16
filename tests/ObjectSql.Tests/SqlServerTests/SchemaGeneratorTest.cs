using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ObjectSql.Tests.SqlServerTests
{
	[TestClass]
	public class SchemaGeneratorTest:TestBase
	{
		[TestMethod]
		public void Foo()
		{
			var result = SqlServer.Schema.SchemaGenerator.Generate("adf","CsName", ConnectionString);

			Assert.Fail(result);
		}
	}
}
