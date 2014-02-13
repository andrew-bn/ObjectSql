using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBoost.Tests.CommandTextGenerationTests
{
	[TestClass]
	public class MiscTests : TestBase
	{
		public class Dto1
		{
			public Dto1() { }
			public Dto1(string fld1, int fld2)
			{
				Field1 = fld1;
				Field2 = fld2;
			}
			public Dto1(string fld1, Dto1 fld2) { }
			public Dto1 Dto { get; set; }
			public DateTime Fld0 { get; set; }
			public string Field1 { get; set; }
			public int Fld1_5 { get; set; }
			public int Field2 { get; set; }
			public string Field3 { get; set; }
		}
		[TestMethod]
		public void Select_WithoutSource_BinaryOperation_SelectResult()
		{
			var c = "const";
			var c2 = "ant";
			EfQuery.Select(() => c + c2)
				.Verify(@"SELECT (@p0 + @p1)",
				c.DbType(SqlDbType.NVarChar), c2.DbType(SqlDbType.NVarChar));
		}
		[TestMethod]
		public void Select_Anonimus_ConcatFieldWithConstant()
		{
			var c = "_const";
			EfQuery.From<Product>()
			.Select((p) => new { Fld1 = p.ProductName + c })
			.Verify("SELECT([p].[ProductName]+@p0)AS[Fld1]" +
						"FROM[dbo].[Products]AS[p]",
						c.DbType(SqlDbType.NVarChar));

		}
		[TestMethod]
		public void Select_Anonimus_ConcatFieldWithField()
		{
			var c = "_const";
			EfQuery.From<Product>()
			.Select((p) => new { Fld1 = p.ProductName + p.ProductName })
			.Verify("SELECT([p].[ProductName]+[p].[ProductName])AS[Fld1]" +
						"FROM[dbo].[Products]AS[p]");
		}
	}
}
