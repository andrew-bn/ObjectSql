using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ObjectSql.SqlServer;
using ObjectSql.Test.Database.TestDatabase.dbo;
using Xunit;

namespace ObjectSql.Test.CommandTextGenerationTests
{
    public class SelectTests: TestBase
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
		public void Select_Constant()
		{
			var c = "cost";
			Query.Select(() => c)
				.Verify(@"SELECT @p0", c.DbType(SqlDbType.NVarChar));
		}
		[Fact]
		public void Select_Constants_In_AnonimusType()
		{
			var c = "cost";
			var i = 43;
			Query.Select(() => new { c, i })
				.Verify(@"SELECT @p0 AS [c],  @p1 AS [i]",
				c.DbType(SqlDbType.NVarChar), i.DbType(SqlDbType.Int));
		}
		[Fact]
		public void Select_Constants_In_DtoType_InitByConstructor()
		{
			var c = "cost";
			var i = 43;
			Query.Select(() => new Dto1(c, i))
				.Verify(@"SELECT @p0 AS [fld1],  @p1 AS [fld2]",
				c.DbType(SqlDbType.NVarChar), i.DbType(SqlDbType.Int));
		}

		[Fact]
		public void Select_Constants_In_DtoType_InitByParams()
		{
			var c = "cost";
			var i = 43;
			Query.Select(() => new Dto1() { Field1 = c, Field2 = i })
				.Verify(@"SELECT @p0 AS [Field1],  @p1 AS [Field2]",
				c.DbType(SqlDbType.NVarChar), i.DbType(SqlDbType.Int));
		}

		[Fact]
		public void Select_AllDbFields()
		{
			Query.From<Products>().Select((p) => p)
			.Verify(
		@"SELECT [p].[ProductID], [p].[ProductName],[p].[SupplierID],[p].[CategoryID],
					[p].[QuantityPerUnit],[p].[UnitPrice],[p].[UnitsInStock],
					[p].[UnitsOnOrder],[p].[ReorderLevel],[p].[Discontinued]
			FROM [dbo].[Products] AS [p]");

		}

		[Fact]
		public void Select_OneField()
		{
			Query.From<Products>().Select((p) => p.ProductName)
							.Verify("SELECT [p].[ProductName] FROM [dbo].[Products] AS [p]");
		}
		[Fact]
		public void Select_FromTable_ButConstant()
		{
			var c = "constant";
			var res = Query.From<Products>()
				.Select(p => c)
				.Verify("SELECT @p0 FROM [dbo].[Products] AS [p]"
				, c.DbType(SqlDbType.NVarChar));
		}
		[Fact]
		public void Select_Anonimus_ConcreteFields()
		{
			var res = Query.From<Products>()
			.Select(p => new { Fld1 = p.CategoryID, Fld2 = p.Discontinued })
			.Verify("SELECT[p].[CategoryID]AS[Fld1],[p].[Discontinued]AS[Fld2]" +
						"FROM[dbo].[Products]AS[p]");
		}

		[Fact]
		public void Select_TSqlComplexFunctionCall()
		{
			Query
				.From<Products>()
				.Select((p) => MsSql.Substring(p.ProductName, 1, 5))
				.Verify("SELECTSUBSTRING([p].[ProductName],@p0,@p1)FROM[dbo].[Products]AS[p]",
						1.DbType(SqlDbType.Int), 5.DbType(SqlDbType.Int));
		}
		[Fact]
		public void Select_CountFunctionCall()
		{
			Query
				.From<Products>()
				.Select((p) => Sql.Count(p.ProductID))
				.Verify("SELECTCOUNT([p].[ProductID])FROM[dbo].[Products]AS[p]");
		}
		[Fact]
		public void Select_WithoutSource_TSqlFunctionResult()
		{
			var c = "cost";
			var res = Query.Select(() => MsSql.Substring(c, 1, 3))
				.Verify(@"SELECTSUBSTRING(@p0,@p1,@p2)",
				c.DbType(SqlDbType.NVarChar), 1.DbType(SqlDbType.Int), 3.DbType(SqlDbType.Int));
		}
		[Fact]
		public void Select_WithoutSource_TSqlFunctionResult_AndConstant()
		{
			var c = "cost";
			Query.Select(() => new { res = MsSql.Substring(c, 1, 3), c })
				.Verify(@"SELECT SUBSTRING(@p0,@p1,@p2) AS [res], @p0 AS [c]",
				c.DbType(SqlDbType.NVarChar), 1.DbType(SqlDbType.Int), 3.DbType(SqlDbType.Int));

		}
		[Fact]
		public void Select_PassEntity_ButSelectConstant()
		{
			var c = "constant";
			Query.From<Products>()
				.Select((p) => MsSql.Substring(c, 1, 4))
				.Verify("SELECT SUBSTRING(@p0,@p1,@p2) FROM [dbo].[Products] AS [p]"
				, c.DbType(SqlDbType.NVarChar), 1.DbType(SqlDbType.Int),
				4.DbType(SqlDbType.Int));
		}
		[Fact]
		public void Select_GroupBy()
		{
			Query.From<Products>()
				.GroupBy((p) => new { p.CategoryID })
				.Select((p) => new { Fld1 = p.CategoryID })
				.Verify("SELECT[p].[CategoryID]AS[Fld1]" +
							"FROM[dbo].[Products]AS[p]" +
							"GROUPBY[p].[CategoryID]");
		}
		[Fact]
		public void Select_GroupBy_Having()
		{
			Query.From<Products>()
				.GroupBy((p) => new { p.CategoryID })
				.Where((p) => p.CategoryID > 5)
				.Select((p) => new { Fld1 = p.CategoryID })
				.Verify("SELECT[p].[CategoryID]AS[Fld1]" +
							"FROM[dbo].[Products]AS[p]" +
							"GROUPBY[p].[CategoryID]" +
							"HAVING([p].[CategoryID]>@p0)",
						5.DbType(SqlDbType.Int));
		}
		[Fact]
		public void Select_GroupBy_Having_AggregateFunction()
		{
			Query.From<Products>()
				.GroupBy((p) => new { p.CategoryID })
				.Where((p) => p.CategoryID > 5 && Sql.Avg(p.UnitPrice) > 5)
				.Select((p) => new { Fld1 = p.CategoryID, Fld2 = Sql.Avg(p.UnitPrice) })
				.Verify("SELECT[p].[CategoryID]AS[Fld1],AVG([p].[UnitPrice])AS[Fld2]" +
							"FROM[dbo].[Products]AS[p]" +
							"GROUPBY[p].[CategoryID]" +
							"HAVING(([p].[CategoryID]>@p0)AND(AVG([p].[UnitPrice])>@p1))",
							5.DbType(SqlDbType.Int), 5M.DbType(SqlDbType.Money));
		}
	}
}
