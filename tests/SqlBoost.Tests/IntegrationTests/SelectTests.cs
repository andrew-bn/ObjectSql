using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlBoost;
using SqlBoost.QueryInterfaces;
using SqlBoost.SqlServer;
namespace SqlBoost.Tests.IntegrationTests
{
	[TestClass]
	public class SelectTests : TestBase
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
		public void Select_Constant_Integration()
		{
			var c = "cost";
			var scalar = EfQuery.Select(() => c).ExecuteScalar();
			Assert.AreEqual(c, scalar);
		}
		[TestMethod]
		public void Select_Constants_In_AnonimusType_Integration()
		{
			var c = "cost";
			var i = 43;
			var res = EfQuery.Select(() => new { c, i }).ExecuteQuery().ToArray();

			Assert.AreEqual(1, res.Length);
			Assert.AreEqual(c, res[0].c);
			Assert.AreEqual(i, res[0].i);
		}

		[TestMethod]
		public void Select_Constants_In_DtoType_InitByConstructor_Integration()
		{
			var c = "cost";
			var i = 43;
			var res = EfQuery.Select(() => new Dto1(c, i)).ExecuteQuery().ToArray();
			Assert.AreEqual(1, res.Length);
			Assert.AreEqual(c, res[0].Field1);
			Assert.AreEqual(i, res[0].Field2);
		}
		[TestMethod]
		public void Select_Constants_In_DtoType_InitByParams_Integration()
		{
			var c = "cost";
			var i = 43;
			var res = EfQuery.Select(() => new Dto1() { Field1 = c, Field2 = i }).ExecuteQuery().ToArray();

			Assert.AreEqual(1, res.Length);
			Assert.AreEqual(c, res[0].Field1);
			Assert.AreEqual(i, res[0].Field2);
		}
		[TestMethod]
		public void Select_AllDbFields_Integration()
		{
			var res = EfQuery.From<Product>().Select((p) => p).ExecuteQuery().ToArray();
			Assert.AreEqual(77, res.Length);
		}
		[TestMethod]
		public void Select_OneField_Integration()
		{
			var res = EfQuery.From<Product>()
							.Select((p) => p.ProductName).ExecuteQuery().ToArray();
			Assert.AreEqual(77, res.Length);
			Assert.AreEqual("Alice Mutton", res[0]);
		}
		[TestMethod]
		public void Select_FromTable_ButConstant_Integration()
		{
			var c = "constant";
			var res = EfQuery.From<Product>().Select((p) => c).ExecuteQuery().ToArray();
			Assert.AreEqual(77, res.Length);
			Assert.AreEqual(77, res.Count(r => r == c));
		}
		[TestMethod]
		public void Select_Anonimus_ConcreteFields_Integration()
		{
			var res = EfQuery.From<Product>()
			.Select((p) => new { Fld1 = p.CategoryID, Fld2 = p.Discontinued }).ExecuteQuery().ToArray();

			Assert.AreEqual(77, res.Length);
			Assert.AreEqual(1, res[0].Fld1);
			Assert.AreEqual(false, res[0].Fld2);
		}
		[TestMethod]
		public void Select_TSqlComplexFunctionCall()
		{
			var res = EfQuery.From<Product>().Select((p) => MsSql.Substring(p.ProductName, 1, 5)).ExecuteQuery().ToArray();
			Assert.AreEqual(77, res.Length);
			Assert.IsFalse(res.Any(v=>v.Length>5));
		}
		[TestMethod]
		public void Select_CountFunctionCall()
		{
			var res = EfQuery.From<Product>().Select((p) => Sql.Count(p.ProductID)).ExecuteScalar();
			Assert.AreEqual(77, res);
		}
		[TestMethod]
		public void Select_WithoutSource_TSqlFunctionResult()
		{
			var c = "cost";
			var res = EfQuery.Select(() => MsSql.Substring(c, 1, 3)).ExecuteScalar();

			Assert.AreEqual("cos", res);
		}
		[TestMethod]
		public void Select_WithoutSource_TSqlFunctionResult_AndConstant()
		{
			var c = "cost";
			var res = EfQuery.Select(() => new { res = MsSql.Substring(c, 1, 3), c }).ExecuteQuery().ToArray();

			Assert.AreEqual(1, res.Length);
			Assert.AreEqual("cos", res[0].res);
			Assert.AreEqual(c, res[0].c);

		}
		[TestMethod]
		public void Select_PassEntity_ButSelectConstant()
		{
			var c = "constant";
			var res = EfQuery.From<Product>()
				.Select((p) => MsSql.Substring(c, 1, 4)).ExecuteQuery().ToArray();
			Assert.AreEqual(77, res.Length);
			Assert.AreEqual(77, res.Count(r => r == "cons"));
		}
		[TestMethod]
		public void Select_GroupBy()
		{
			var res = EfQuery.From<Product>()
				.GroupBy((p) => new { p.CategoryID })
				.Where((p)=> p.CategoryID>2 && Sql.Avg(p.ReorderLevel)>15)
				.Select((p) => new { Fld1 = p.CategoryID, Fld2 = Sql.Avg(p.ReorderLevel)});
			var arr = res.ExecuteQuery().Single();
			Assert.AreEqual(5, arr.Fld1);
			Assert.AreEqual(22, arr.Fld2);
		}
		[TestMethod]
		public void Select_GroupBy_AvgByDecimal()
		{
			var res = EfQuery
				.From<Product>()
				.GroupBy((p) => new { p.CategoryID })
				.Select((p) => new { Fld1 = p.CategoryID, Fld2 = Sql.Avg(p.UnitPrice) });
			var arr = res.ExecuteQuery().ToArray();
			Assert.AreEqual(8, arr.Length);
		}

		[TestMethod]
		public void Select_CompiledQuery()
		{
			var mng = new SqlBoostManager<SqlConnection>(EfConnectionString);
			var q = mng.CompileQuery(
						(Tuple<int,int> args) =>
						mng.Query().From<Product>()
						.GroupBy((p) => new { p.CategoryID })
						.Where((p) => p.CategoryID > args.Item1 && Sql.Avg(p.ReorderLevel) > 15)
						.Select((p) => new { Fld1 = p.CategoryID, Fld2 = Sql.Avg(p.ReorderLevel) }));

			var arr = q(new Tuple<int, int>(2, 15)).ExecuteQuery().ToArray();

			var entity = arr[0];
			Assert.AreEqual(5, entity.Fld1);
			Assert.AreEqual(22, entity.Fld2);
		}
		[TestMethod]
		public void StoredProcedure()
		{
			var result = EfQuery.StoredProcedure((TestDatabaseEntities e) => e.CustOrderHist("VINET"))
			                    .ExecuteQuery().ToArray();

			Assert.AreEqual(9,result.Length);

		}
#if NET45
		[TestMethod]
		public void Select_GroupBy_Async()
		{
			var res = EfQuery.From<Product>()
				.GroupBy((p) => new { p.CategoryID })
				.Where((p) => p.CategoryID > 2 && Sql.Avg(p.ReorderLevel) > 15)
				.Select((p) => new { Fld1 = p.CategoryID, Fld2 = Sql.Avg(p.ReorderLevel) });
			var arr = res.ExecuteQueryAsync().Result.EntitiesToArrayAsync().Result;
			var entity = arr[0];
			Assert.AreEqual(5, entity.Fld1);
			Assert.AreEqual(22, entity.Fld2);
		}
#endif
	}
}
