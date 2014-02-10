using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlBoost.SqlServer;
using SqlBoost;
namespace SqlBoost.Tests.CommandTextGenerationTests
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
		public void Select_Constant()
		{
			var c = "cost";
			EfQuery.Select((db) => c)
				.Verify(@"SELECT @p0", c.DbType(SqlDbType.NVarChar));
		}
		[TestMethod]
		public void Select_Constants_In_AnonimusType()
		{
			var c = "cost";
			var i = 43;
			EfQuery.Select((db) => new { c, i })
				.Verify(@"SELECT @p0 AS [c],  @p1 AS [i]",
				c.DbType(SqlDbType.NVarChar), i.DbType(SqlDbType.Int));
		}
		[TestMethod]
		public void Select_Constants_In_DtoType_InitByConstructor()
		{
			var c = "cost";
			var i = 43;
			EfQuery.Select((db) => new Dto1(c, i))
				.Verify(@"SELECT @p0 AS [fld1],  @p1 AS [fld2]",
				c.DbType(SqlDbType.NVarChar), i.DbType(SqlDbType.Int));
		}
		[TestMethod]
		public void Select_Constants_In_DtoType_InitByParams()
		{
			var c = "cost";
			var i = 43;
			EfQuery.Select((db) => new Dto1() { Field1 = c, Field2 = i })
				.Verify(@"SELECT @p0 AS [Field1],  @p1 AS [Field2]",
				c.DbType(SqlDbType.NVarChar), i.DbType(SqlDbType.Int));
		}
		[TestMethod]
		public void Select_AllDbFields()
		{
			EfQuery.From<Product>().Select((db, p) => p)
			.Verify(
		@"SELECT [p].[ProductID], [p].[ProductName],[p].[SupplierID],[p].[CategoryID],
					[p].[QuantityPerUnit],[p].[UnitPrice],[p].[UnitsInStock],
					[p].[UnitsOnOrder],[p].[ReorderLevel],[p].[Discontinued]
			FROM [dbo].[Products] AS [p]");

		}
		[TestMethod]
		public void Select_OneField()
		{
			EfQuery.From<Product>().Select((db,p) => p.ProductName)
							.Verify("SELECT [p].[ProductName] FROM [dbo].[Products] AS [p]");
		}
		[TestMethod]
		public void Select_FromTable_ButConstant()
		{
			var c = "constant";
			var res = EfQuery.From<Product>()
				.Select((db,p) => c)
				.Verify("SELECT @p0 FROM [dbo].[Products] AS [p]"
				, c.DbType(SqlDbType.NVarChar));
		}
		[TestMethod]
		public void Select_Anonimus_ConcreteFields()
		{
			var res = EfQuery.From<Product>()
			.Select((db,p) => new { Fld1 = p.CategoryID, Fld2 = p.Discontinued })
			.Verify("SELECT[p].[CategoryID]AS[Fld1],[p].[Discontinued]AS[Fld2]" +
						"FROM[dbo].[Products]AS[p]");
		}
		[TestMethod]
		public void Select_TSqlComplexFunctionCall()
		{
			EfQuery
				.From<Product>()
				.Select((db, p) => db.Substring(p.ProductName, 1, 5))
				.Verify("SELECTSUBSTRING([p].[ProductName],@p0,@p1)FROM[dbo].[Products]AS[p]",
						1.DbType(SqlDbType.Int), 5.DbType(SqlDbType.Int));
		}
		[TestMethod]
		public void Select_CountFunctionCall()
		{
			EfQuery
				.From<Product>()
				.Select((db, p) => db.Count(p.ProductID))
				.Verify("SELECTCOUNT([p].[ProductID])FROM[dbo].[Products]AS[p]");
		}
		[TestMethod]
		public void Select_WithoutSource_TSqlFunctionResult()
		{
			var c = "cost";
			var res = EfQuery.Select(db => db.Substring(c, 1, 3))
				.Verify(@"SELECTSUBSTRING(@p0,@p1,@p2)",
				c.DbType(SqlDbType.NVarChar), 1.DbType(SqlDbType.Int), 3.DbType(SqlDbType.Int));
		}
		[TestMethod]
		public void Select_WithoutSource_TSqlFunctionResult_AndConstant()
		{
			var c = "cost";
			EfQuery.Select(db => new { res = db.Substring(c, 1, 3), c })
				.Verify(@"SELECT SUBSTRING(@p0,@p1,@p2) AS [res], @p0 AS [c]",
				c.DbType(SqlDbType.NVarChar), 1.DbType(SqlDbType.Int), 3.DbType(SqlDbType.Int));

		}
		[TestMethod]
		public void Select_PassEntity_ButSelectConstant()
		{
			var c = "constant";
			EfQuery.From<Product>()
				.Select((db, p) => db.Substring(c, 1, 4))
				.Verify("SELECT SUBSTRING(@p0,@p1,@p2) FROM [dbo].[Products] AS [p]"
				, c.DbType(SqlDbType.NVarChar), 1.DbType(SqlDbType.Int),
				4.DbType(SqlDbType.Int));
		}
		[TestMethod]
		public void Select_GroupBy()
		{
			EfQuery.From<Product>()
				.GroupBy((db,p) => new { p.CategoryID })
				.Select((db,p) => new { Fld1 = p.CategoryID })
				.Verify("SELECT[p].[CategoryID]AS[Fld1]" +
							"FROM[dbo].[Products]AS[p]" +
							"GROUPBY[p].[CategoryID]");
		}
		[TestMethod]
		public void Select_GroupBy_Having()
		{
			EfQuery.From<Product>()
				.GroupBy((db,p) => new { p.CategoryID })
				.Where((db, p) => p.CategoryID > 5)
				.Select((db, p) => new { Fld1 = p.CategoryID })
				.Verify("SELECT[p].[CategoryID]AS[Fld1]" +
							"FROM[dbo].[Products]AS[p]" +
							"GROUPBY[p].[CategoryID]" +
							"HAVING([p].[CategoryID]>@p0)",
						5.DbType(SqlDbType.Int));
		}
		[TestMethod]
		public void Select_GroupBy_Having_AggregateFunction()
		{
			EfQuery.From<Product>()
				.GroupBy((db, p) => new { p.CategoryID })
				.Where((db, p) => p.CategoryID > 5 && db.Avg(p.UnitPrice) > 5)
				.Select((db, p) => new { Fld1 = p.CategoryID, Fld2 = db.Avg(p.UnitPrice) })
				.Verify("SELECT[p].[CategoryID]AS[Fld1],AVG([p].[UnitPrice])AS[Fld2]" +
							"FROM[dbo].[Products]AS[p]" +
							"GROUPBY[p].[CategoryID]" +
							"HAVING(([p].[CategoryID]>@p0)AND(AVG([p].[UnitPrice])>@p1))",
							5.DbType(SqlDbType.Int), 5M.DbType(SqlDbType.Money));
		}
	}
}
