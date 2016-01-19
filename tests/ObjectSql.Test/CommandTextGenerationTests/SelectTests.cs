using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
	}
}
