using System.Data.SqlClient;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectSql.SqlServer;
using ObjectSql;
using ObjectSql.QueryInterfaces;
using ObjectSql.Test;
using Xunit;
using ObjectSql.Test.Database.TestDatabase.dbo;

namespace ObjectSql.Tests.IntegrationTests
{
	
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
		[Fact]
		public void Select_Constant_Integration()
		{
			var c = "cost";
			var scalar = Query.Select(() => c).ExecuteScalar();
			Assert.Equal(c, scalar.ScalarResult);
		}
		[Fact]
		public void Select_Constants_In_AnonimusType_Integration()
		{
			var c = "cost";
			var i = 43;
			var res = Query.Select(() => new { c, i }).ExecuteQuery().ToArray();

			Assert.Equal(1, res.Length);
			Assert.Equal(c, res[0].c);
			Assert.Equal(i, res[0].i);
		}

		[Fact]
		public void Select_Constants_In_DtoType_InitByConstructor_Integration()
		{
			var c = "cost";
			var i = 43;
			var res = Query.Select(() => new Dto1(c, i)).ExecuteQuery().ToArray();
			Assert.Equal(1, res.Length);
			Assert.Equal(c, res[0].Field1);
			Assert.Equal(i, res[0].Field2);
		}
		[Fact]
		public void Select_Constants_In_DtoType_InitByParams_Integration()
		{
			var c = "cost";
			var i = 43;
			var res = Query.Select(() => new Dto1() { Field1 = c, Field2 = i }).ExecuteQuery().ToArray();

			Assert.Equal(1, res.Length);
			Assert.Equal(c, res[0].Field1);
			Assert.Equal(i, res[0].Field2);
		}
		[Fact]
		public void Select_AllDbFields_Integration()
		{
			var res = Query.From<Products>().Select((p) => p).ExecuteQuery().ToArray();
			Assert.Equal(77, res.Length);
		}
		[Fact]
		public void Select_OneField_Integration()
		{
			var res = Query.From<Products>()
							.Select((p) => p.ProductName).ExecuteQuery().ToArray();
			Assert.Equal(77, res.Length);
			Assert.Equal("Alice Mutton", res[0]);
		}
		[Fact]
		public void Select_FromTable_ButConstant_Integration()
		{
			var c = "constant";
			var res = Query.From<Products>().Select((p) => c).ExecuteQuery().ToArray();
			Assert.Equal(77, res.Length);
			Assert.Equal(77, res.Count(r => r == c));
		}
		[Fact]
		public void Select_Anonimus_ConcreteFields_Integration()
		{
			var res = Query.From<Products>()
			.Select((p) => new { Fld1 = p.CategoryID, Fld2 = p.Discontinued }).ExecuteQuery().ToArray();

			Assert.Equal(77, res.Length);
			Assert.Equal(1, res[0].Fld1);
			Assert.Equal(false, res[0].Fld2);
		}
		[Fact]
		public void Select_TSqlComplexFunctionCall()
		{
			var res = Query.From<Products>().Select((p) => MsSql.Substring(p.ProductName, 1, 5)).ExecuteQuery().ToArray();
			Assert.Equal(77, res.Length);
			Assert.False(res.Any(v => v.Length > 5));
		}
		[Fact]
		public void Select_CountFunctionCall()
		{
			var res = Query.From<Products>().Select((p) => Sql.Count(p.ProductID)).ExecuteScalar();
			Assert.Equal(77, res.ScalarResult);
		}
		[Fact]
		public void Select_WithoutSource_TSqlFunctionResult()
		{
			var c = "cost";
			var res = Query.Select(() => MsSql.Substring(c, 1, 3)).ExecuteScalar();

			Assert.Equal("cos", res.ScalarResult);
		}
		[Fact]
		public void Select_WithoutSource_TSqlFunctionResult_AndConstant()
		{
			var c = "cost";
			var res = Query.Select(() => new { res = MsSql.Substring(c, 1, 3), c }).ExecuteQuery().ToArray();

			Assert.Equal(1, res.Length);
			Assert.Equal("cos", res[0].res);
			Assert.Equal(c, res[0].c);

		}
		[Fact]
		public void Select_PassEntity_ButSelectConstant()
		{
			var c = "constant";
			var res = Query.From<Products>()
				.Select((p) => MsSql.Substring(c, 1, 4)).ExecuteQuery().ToArray();
			Assert.Equal(77, res.Length);
			Assert.Equal(77, res.Count(r => r == "cons"));
		}
		[Fact]
		public void Select_GroupBy()
		{
			var res = Query.From<Products>()
				.GroupBy((p) => new { p.CategoryID })
				.Where((p) => p.CategoryID > 2 && Sql.Avg(p.ReorderLevel) > 15)
				.Select((p) => new { Fld1 = p.CategoryID, Fld2 = Sql.Avg(p.ReorderLevel) });
			var arr = res.ExecuteQuery().Single();
			Assert.Equal(5, arr.Fld1);
			Assert.Equal(22, arr.Fld2);
		}
		[Fact]
		public void Select_GroupBy_AvgByDecimal()
		{
			var res = Query
				.From<Products>()
				.GroupBy((p) => new { p.CategoryID })
				.Select((p) => new { Fld1 = p.CategoryID, Fld2 = Sql.Avg(p.UnitPrice) });
			var arr = res.ExecuteQuery().ToArray();
			Assert.Equal(8, arr.Length);
		}

		[Fact]
		public void Select_CompiledQuery()
		{
			var mng = new ObjectSqlManager<SqlConnection>(ConnectionString);
			var q = mng.CompileQuery(
						(Tuple<int, int> args) =>
						mng.Query().From<Products>()
						.GroupBy((p) => new { p.CategoryID })
						.Where((p) => Sql.Avg(p.ReorderLevel) > 15 && p.CategoryID > args.Item1)
						.Select((p) => new { Fld1 = p.CategoryID, Fld2 = Sql.Avg(p.ReorderLevel) }));

			var arr = q(new Tuple<int, int>(2, 15)).ExecuteQuery().ToArray();

			var entity = arr[0];
			Assert.Equal(5, entity.Fld1);
			Assert.Equal(22, entity.Fld2);
		}
	}
}
