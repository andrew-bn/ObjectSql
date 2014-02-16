using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql.Tests.IntegrationTests
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
			var res = EfQuery.Select(() => c + c2)
				.ExecuteScalar();

			Assert.AreEqual("constant", res);
		}
		[TestMethod]
		public void Select_Anonimus_ConcatFieldWithConstant()
		{
			var c = "_const";
			var res = EfQuery.From<Product>()
			.Select((p) => new { Fld1 = p.ProductName + c }).ExecuteQuery().ToArray();

			Assert.AreEqual(77, res.Length);
			Assert.AreEqual("Alice Mutton_const", res[0].Fld1);

		}
		[TestMethod]
		public void Select_Anonimus_ConcatFieldWithField()
		{
			var c = "_const";
			var res = EfQuery.From<Product>()
			.Select((p) => new { Fld1 = p.ProductName + p.ProductName }).ExecuteQuery().ToArray();

			Assert.AreEqual(77, res.Length);
			Assert.AreEqual("Alice MuttonAlice Mutton", res[0].Fld1);

		}
	}
}
