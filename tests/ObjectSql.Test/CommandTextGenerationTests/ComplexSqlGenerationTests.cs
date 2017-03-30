using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectSql;
using System.Data.SqlClient;
using System.Reflection;
using System.Data;
using ObjectSql.SqlServer;
using ObjectSql.Test;
using ObjectSql.Test.Database.TestDatabase.dbo;
using Xunit;
using ObjectSql.Test.Database.TestDatabase;

namespace ObjectSql.Tests.CommandTextGenerationTests
{
	
	public class ComplexSqlGenerationTests : TestBase
	{
		[Fact]
		public void ComplexSelectSql_TypeSchema()
		{
			var val = "val";
			var val2 = "val";
			var result = Query
				.From<Products>()
				.Join<Categories>((p, c) => p.CategoryID == c.CategoryID)
				.Where((p, c) => p.ProductName != val && p.ReorderLevel == 2 &&
									 p.QuantityPerUnit != null || p.QuantityPerUnit != val)
				.GroupBy((p, c) => new { p.ProductName })
				.Where((p, c) => p.ProductName != val2)
				.Select((p, c) => new { p.ProductName, c.CategoryName })
				.Verify(
				@"SELECT [p].[ProductName] AS [ProductName],[c].[CategoryName] AS [CategoryName] 
				  FROM [dbo].[Products] AS [p] 
					JOIN [dbo].[Categories] AS [c] ON ([p].[CategoryID] = [c].[CategoryID])
				  WHERE (((([p].[ProductName] <> @p0) AND ([p].[ReorderLevel] = @p1)) AND 
							([p].[QuantityPerUnit] IS NOT NULL)) OR ([p].[QuantityPerUnit] <> @p0))
				  GROUP BY [p].[ProductName]
				  HAVING ([p].[ProductName] <> @p2)",
					val.DbType(SqlDbType.NVarChar), 2.DbType(SqlDbType.SmallInt),
					val2.DbType(SqlDbType.NVarChar));
		}
		[Fact]
		public void ComplexSelectSql_EfSchema()
		{
			var val = "val";
			var val2 = "val";
			var result = Query
				.From<Products>()
				.Join<Categories>((p, c) => p.CategoryID == c.CategoryID)
				.Where((p, c) => p.ProductName != val && p.ReorderLevel == 2 &&
									 p.QuantityPerUnit != null || p.QuantityPerUnit != val)
				.GroupBy((p, c) => new { p.ProductName })
				.Where((p, c) => p.ProductName != val2)
				.Select((p, c) => new { p.ProductName, c.CategoryName })
				.Verify(
				@"SELECT [p].[ProductName] AS [ProductName],[c].[CategoryName] AS [CategoryName] 
				  FROM [dbo].[Products] AS [p] 
					JOIN [dbo].[Categories] AS [c] ON ([p].[CategoryID] = [c].[CategoryID]) 
				  WHERE (((([p].[ProductName] <> @p0) AND ([p].[ReorderLevel] = @p1)) 
							AND ([p].[QuantityPerUnit] IS NOT NULL)) OR ([p].[QuantityPerUnit] <> @p0)) 
				  GROUP BY [p].[ProductName] 
				  HAVING ([p].[ProductName] <> @p2)",
				val.DbType(SqlDbType.NVarChar), 2.DbType(SqlDbType.SmallInt), val2.DbType(SqlDbType.NVarChar));
		}
		[Fact]
		public void ComplextInsert_EfSchema()
		{
			var result = Query
							.Insert<Products>(p => new { p.ProductName, p.QuantityPerUnit, p.SupplierID })
							.Values(new Products() { ProductName = "P1", QuantityPerUnit = "23", SupplierID = null },
									new Products() { ProductName = "P2", QuantityPerUnit = "223", SupplierID = 2 })
							.Verify(
							@"INSERT INTO [dbo].[Products] ([ProductName],[QuantityPerUnit],[SupplierID])
							  VALUES (@p0,@p1,NULL),(@p2,@p3,@p4)",
							"P1".DbType(SqlDbType.NVarChar), "23".DbType(SqlDbType.NVarChar),
							"P2".DbType(SqlDbType.NVarChar), "223".DbType(SqlDbType.NVarChar), 2.DbType(SqlDbType.Int));
		}
		[Fact]
		public void ComplextInsert_EfSchema_Null_Param()
		{
			var result = Query
							.Insert<Products>(p => new { p.ProductName, p.QuantityPerUnit, p.SupplierID })
							.Values(new Products() { ProductName = "P1", QuantityPerUnit = "23", SupplierID = null },
									new Products() { ProductName = "P2", QuantityPerUnit = "223", SupplierID = 2 })
							.Verify(
							@"INSERT INTO [dbo].[Products] ([ProductName],[QuantityPerUnit],[SupplierID])
							  VALUES (@p0,@p1,NULL),(@p2,@p3,@p4)",
							"P1".DbType(SqlDbType.NVarChar), "23".DbType(SqlDbType.NVarChar),
							"P2".DbType(SqlDbType.NVarChar), "223".DbType(SqlDbType.NVarChar), 2.DbType(SqlDbType.Int));
		}
		[Fact]
		public void ComplexDelete_EfSchema()
		{
			var val = "avsdf";
			var result = Query.
							Delete<Products>((p) => p.CategoryID != 23 &&
													p.ProductName != val ||
													p.ReorderLevel == null)
							.Verify(@"DELETE FROM [dbo].[Products]
									 WHERE ((([CategoryID] <> @p0) AND
												([ProductName] <> @p1)) OR
												([ReorderLevel] IS NULL))",
									23.DbType(SqlDbType.Int), val.DbType(SqlDbType.NVarChar));
		}
		[Fact]
		public void ComplexUpdate_EfSchema()
		{
			var val = "avsdf";
			var result = Query.
							Update(() => new Products() { ProductID = 23, QuantityPerUnit = val, Discontinued = false })
							.Where((p) => p.UnitPrice == 23M && p.UnitsInStock == null &&
											p.ProductName != val)
							.Verify(@"UPDATE [dbo].[Products]
									  SET [ProductID] = @p0, [QuantityPerUnit] = @p1, [Discontinued] = @p2
									 WHERE ((([UnitPrice] = @p3) AND ([UnitsInStock] IS NULL)) AND ([ProductName] <> @p1))",
									23.DbType(SqlDbType.Int), val.DbType(SqlDbType.NVarChar), false.DbType(SqlDbType.Bit),
									23M.DbType(SqlDbType.Money));

		}
		[Fact]
		public void ComplexUpdate_EfSchema_DateDiff()
		{
			var val = DateTime.Now;

			var result = Query.
							Update(() => new Orders() { CustomerID = "1" })
							.Where(o => MsSql.DateDiff(DatePart.Year, o.OrderDate, val) > 13)
							.Verify(@"UPDATE [dbo].[Orders] 
									  SET [CustomerID] = @p0  
									 WHERE  (DATEDIFF(year, [OrderDate], @p1) > @p2)",
										"1".DbType(SqlDbType.NChar), val.DbType(SqlDbType.DateTime2), 13.DbType(SqlDbType.Int));

		}
		//[Fact]
		//public void StoredProcedure_EfSchema()
		//{
		//	Query.Exec((TestDatabaseEntities e) => e.CustOrderHist("0"))
		//		.Verify("[dbo].[CustOrderHist]",
		//					"0".DbType(SqlDbType.NChar).Name("CustomerID"));
		//}
		//[Fact]
		//public void StoredProcedure_EfSchema_ParameterIsNull()
		//{
		//	Query.Exec((TestDatabaseEntities e) => e.CustOrderHist(null))
		//		.Verify("[dbo].[CustOrderHist]",
		//					DBNull.Value.DbType(SqlDbType.NChar).Name("CustomerID"));
		//}

		[Fact]
		public void StoredProcedure_AutoGeneratedSp()
		{

			Query.Exec<TestDatabaseProcedures>(d => d.CustOrderHist(null))
				.Verify("[dbo].[CustOrderHist]",
							DBNull.Value.DbType(SqlDbType.NChar).Name("CustomerID"));
		}

		[Fact]
		public void StoredProcedure_AutoGeneratedSp_ComplexName()
		{
			Query.Exec<TestDatabaseProcedures>(d => d.Ten_Most_Expensive_Products())
				 .Verify("[dbo].[Ten Most Expensive Products]");
		}

		[Fact]
		public void ComplexSelectSql_ButchQuery()
		{
			var val = "val";
			var val2 = "val";
			var result = Query
				//select p.productname as productname, c.categoryname as categoryname
				.From<Products>()
				.Join<Categories>((p, c) => p.CategoryID == c.CategoryID)
				.Where((p, c) => p.ProductName != val && p.ReorderLevel == 2 &&
									 p.QuantityPerUnit != null || p.QuantityPerUnit != val)
				.GroupBy((p, c) => new { p.ProductName })
				.Where((p, c) => p.ProductName != val2)
				.Select((p, c) => new { p.ProductName, c.CategoryName })

				.From<Products>()
				.Join<Categories>((p, c) => p.CategoryID == c.CategoryID)
				.Where((p, c) => p.ProductName != val && p.ReorderLevel == 2 &&
									 p.QuantityPerUnit != null || p.QuantityPerUnit != val)
				.Select((p, c) => p.ProductName)

				.Update(() => new Products { ProductName = "ProductName" })
				.Where(p => p.ProductID == 22)

				.Verify(
				@"SELECT [p].[ProductName] AS [ProductName],[c].[CategoryName] AS [CategoryName] 
				  FROM [dbo].[Products] AS [p] 
					JOIN [dbo].[Categories] AS [c] ON ([p].[CategoryID] = [c].[CategoryID])
				  WHERE (((([p].[ProductName] <> @p0) AND ([p].[ReorderLevel] = @p1)) AND 
							([p].[QuantityPerUnit] IS NOT NULL)) OR ([p].[QuantityPerUnit] <> @p0))
				  GROUP BY [p].[ProductName]
				  HAVING ([p].[ProductName] <> @p2);

				  SELECT [p].[ProductName]
				  FROM [dbo].[Products] AS [p] 
					JOIN [dbo].[Categories] AS [c] ON ([p].[CategoryID] = [c].[CategoryID])
				  WHERE (((([p].[ProductName] <> @p0) AND ([p].[ReorderLevel] = @p1)) AND 
							([p].[QuantityPerUnit] IS NOT NULL)) OR ([p].[QuantityPerUnit] <> @p0));

				  UPDATE [dbo].[Products] SET [ProductName] = @p3 
				  WHERE([ProductID] = @p4 )",
					val, 2, val2, "ProductName", 22);
		}
		[Fact]
		public void select_with_in_array()
		{
			var p1 = "pn";
			var result = Query
				.From<Products>()
				.Where(p => p.ProductName == p1 && p.CategoryID.In(11, 22, 33))
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName] 
				FROM [dbo].[Products] AS [p] 
				WHERE (([p].[ProductName] = @p0) AND  
					   ([p].[CategoryID] IN (@p1, @p2, @p3)))",
				  p1, 11, 22, 33);

		}
		[Fact]
		public void select_with_in_array_of_strings()
		{
			var p1 = "pn";
			var result = Query
				.From<Products>()
				.Where(p => p.ProductName.In("name1", "name2"))
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName] 
				FROM [dbo].[Products] AS [p] 
				WHERE ([p].[ProductName] IN (@p0, @p1))",
				  "name1", "name2");
		}
		[Fact]
		public void select_with_not_in_array()
		{
			var p1 = "pn";
			var result = Query
				.From<Products>()
				.Where(p => p.ProductName == p1 && p.CategoryID.NotIn(11, 22, 33))
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName] 
				FROM [dbo].[Products] AS [p] 
				WHERE (([p].[ProductName] = @p0) AND  
					   ([p].[CategoryID] NOT IN (@p1, @p2, @p3)))",
				  p1, 11, 22, 33);
		}
		[Fact]
		public void select_with_in_array_variable()
		{
			var p1 = "pn";
			var array_variable = new[] { 11, 22, 33 };
			var result = Query
				.From<Products>()
				.Where(p => p.ProductName == p1 && p.CategoryID.In(array_variable) &&
					p.ProductID.In(array_variable))
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName] 
				FROM [dbo].[Products] AS [p] 
				WHERE ((([p].[ProductName] = @p0) AND  
					   ([p].[CategoryID] IN (@p1_0, @p1_1, @p1_2))) AND
                       ([p].[ProductID] IN (@p1_0, @p1_1, @p1_2)))",

				  p1, 11.Name("p1_0"), 22.Name("p1_1"), 33.Name("p1_2"));
		}
		[Fact]
		public void select_with_in_array_variable_complex()
		{
			var p1 = "pn";
			var array_variable = new[] { 11, 22, 33 };
			var result = Query
				.From<Products>()
				.Where(p => p.ProductName == p1 && p.CategoryID.In(array_variable) &&
					p.ProductID.In(array_variable))
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName] 
				FROM [dbo].[Products] AS [p] 
				WHERE ((([p].[ProductName] = @p0) AND  
					   ([p].[CategoryID] IN (@p1_0, @p1_1, @p1_2))) AND
                       ([p].[ProductID] IN (@p1_0, @p1_1, @p1_2)))",

				  p1, 11.Name("p1_0"), 22.Name("p1_1"), 33.Name("p1_2"));


			p1 = "pn2";
			array_variable = new[] { 11, 25, 33 };
			result = Query
				.From<Products>()
				.Where(p => p.ProductName == p1 && p.CategoryID.In(array_variable) &&
					p.ProductID.In(array_variable))
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName] 
				FROM [dbo].[Products] AS [p] 
				WHERE ((([p].[ProductName] = @p0) AND  
					   ([p].[CategoryID] IN (@p1_0, @p1_1, @p1_2))) AND
                       ([p].[ProductID] IN (@p1_0, @p1_1, @p1_2)))",

				  p1, 11.Name("p1_0"), 25.Name("p1_1"), 33.Name("p1_2"));

			p1 = "pn2";
			array_variable = new[] { 11, 25, 33, 45 };
			result = Query
				.From<Products>()
				.Where(p => p.ProductName == p1 && p.CategoryID.In(array_variable) &&
					p.ProductID.In(array_variable))
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName] 
				FROM [dbo].[Products] AS [p] 
				WHERE ((([p].[ProductName] = @p0) AND  
					   ([p].[CategoryID] IN (@p1_0, @p1_1, @p1_2, @p1_3))) AND
                       ([p].[ProductID] IN (@p1_0, @p1_1, @p1_2, @p1_3)))",

				  p1, 11.Name("p1_0"), 25.Name("p1_1"), 33.Name("p1_2"), 45.Name("p1_3"));
		}
		[Fact]
		public void select_with_subquery()
		{
			var p1 = "pn";
			var p2 = "p324";
			var result = Query
				.From<Products>()
				.Where(p => p.ProductName == p1 && p.CategoryID.In(Sql.Query.From<Categories>()
																			.Where(c => c.CategoryName == p1 && c.Description != p2)
																			.Select(c => c.CategoryID))
							&& p.ProductName != p2
					  )
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName]
				  FROM [dbo].[Products] AS [p] 
				  WHERE ((([p].[ProductName]=@p0) AND 
						 ([p].[CategoryID] IN (SELECT [c].[CategoryID]
												FROM [dbo].[Categories] AS [c]
												WHERE (([c].[CategoryName]=@p0) AND ([c].[Description]<>@p1))))) AND
						  ([p].[ProductName]<>@p2))",
				  p1, p2, p2);
		}

		[Fact]
		public void select_with_subquery_that_uses_main_querytable()
		{
			Query
				.From<Products>()
				.Where(p => p.CategoryID.In(Sql.Query.From<Categories>()
													 .Where(c => c.CategoryName == p.ProductName ||
																p.CategoryID == c.CategoryID)
													 .Select(c => c.CategoryID)))
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName] 
				  FROM [dbo].[Products] AS [p] 
				  WHERE  ([p].[CategoryID] IN (SELECT [c].[CategoryID] 
				  							   FROM [dbo].[Categories] AS [c] 
				  							   WHERE (([c].[CategoryName] = [p].[ProductName]) OR ([p].[CategoryID] = [c].[CategoryID]))))");
		}

		[Fact]
		public void select_with_nested_subquery_that_uses_main_querytable()
		{
			Query
				.From<Products>()
				.Where(p => p.CategoryID.In(Sql.Query.From<Categories>()
													 .Where(c => c.CategoryName == p.ProductName &&
																c.CategoryID.NotIn(Sql.Query.From<Products>()
																							 .Where(pr => pr.ProductID != p.ProductID &&
																									pr.CategoryID != c.CategoryID)
																							 .Select(pr => pr.CategoryID)))
													 .Select(c => c.CategoryID)))
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName] 
FROM [dbo].[Products] AS [p] 
WHERE  ([p].[CategoryID] IN (SELECT [c].[CategoryID] 
							 FROM [dbo].[Categories] AS [c] 
							 WHERE (([c].[CategoryName] = [p].[ProductName]) AND  
									([c].[CategoryID] NOT IN (SELECT [pr].[CategoryID] 
															  FROM [dbo].[Products] AS [pr] 
															  WHERE (([pr].[ProductID] <> [p].[ProductID]) AND 
																	([pr].[CategoryID] <> [c].[CategoryID])))
									 )))
		) ");
		}

		[Fact]
		public void ComplexSelectSql_LeftJoin()
		{
			Query
				.From<Products>()
				.LeftJoin<Categories>((p, c) => p.CategoryID == c.CategoryID)
				.Select((p, c) => new { p.ProductName, c.CategoryName })
				.Verify(
				@"SELECT [p].[ProductName] AS [ProductName],[c].[CategoryName] AS [CategoryName] 
				  FROM [dbo].[Products] AS [p] 
				  LEFT JOIN [dbo].[Categories] AS [c] ON ([p].[CategoryID] = [c].[CategoryID])");
		}
		[Fact]
		public void ComplexSelectSql_RightJoin()
		{
			Query
				.From<Products>()
				.RightJoin<Categories>((p, c) => p.CategoryID == c.CategoryID)
				.Select((p, c) => new { p.ProductName, c.CategoryName })
				.Verify(
				@"SELECT [p].[ProductName] AS [ProductName],[c].[CategoryName] AS [CategoryName] 
				  FROM [dbo].[Products] AS [p] 
				  RIGHT JOIN [dbo].[Categories] AS [c] ON ([p].[CategoryID] = [c].[CategoryID])");
		}

		[Fact]
		public void ComplexSelectSql_FullJoin()
		{
			Query
				.From<Products>()
				.FullJoin<Categories>((p, c) => p.CategoryID == c.CategoryID)
				.Select((p, c) => new { p.ProductName, c.CategoryName })
				.Verify(
				@"SELECT [p].[ProductName] AS [ProductName],[c].[CategoryName] AS [CategoryName] 
				  FROM [dbo].[Products] AS [p] 
				  FULL JOIN [dbo].[Categories] AS [c] ON ([p].[CategoryID] = [c].[CategoryID])");
		}
		[Fact]
		public void BuildSql_Insert_SelectedFieldsWithDto()
		{
			Query
				.Insert<Products>(p => new Products { ProductName = null, QuantityPerUnit = null, SupplierID = 1 })
				.Values(new Products() { ProductName = "P1", QuantityPerUnit = "23", SupplierID = null },
						new Products() { ProductName = "P2", QuantityPerUnit = "223", SupplierID = 2 })
				.Verify(
				@"INSERT INTO [dbo].[Products] ([ProductName],[QuantityPerUnit],[SupplierID])
				  VALUES (@p0,@p1,NULL),(@p2,@p3,@p4)",
				"P1".DbType(SqlDbType.NVarChar), "23".DbType(SqlDbType.NVarChar),
				"P2".DbType(SqlDbType.NVarChar), "223".DbType(SqlDbType.NVarChar), 2.DbType(SqlDbType.Int));
		}
		[Fact]
		public void BuildSql_OutputClause()
		{
			Query
				.Insert<Products>(p => new { p.ProductName, p.QuantityPerUnit, p.SupplierID })
				.Output((i, d) => i)
				.Values(new Products() { ProductName = "P1", QuantityPerUnit = "23", SupplierID = null },
						new Products() { ProductName = "P2", QuantityPerUnit = "223", SupplierID = 2 })
				.Verify(
				@"INSERT INTO [dbo].[Products] ([ProductName],[QuantityPerUnit],[SupplierID])
				  OUTPUT [INSERTED].[ProductID], [INSERTED].[ProductName], [INSERTED].[SupplierID], [INSERTED].[CategoryID], [INSERTED].[QuantityPerUnit], [INSERTED].[UnitPrice], [INSERTED].[UnitsInStock], [INSERTED].[UnitsOnOrder], [INSERTED].[ReorderLevel], [INSERTED].[Discontinued]
				  VALUES (@p0,@p1,NULL),(@p2,@p3,@p4)",
				"P1".DbType(SqlDbType.NVarChar), "23".DbType(SqlDbType.NVarChar),
				"P2".DbType(SqlDbType.NVarChar), "223".DbType(SqlDbType.NVarChar), 2.DbType(SqlDbType.Int));
		}
		[Fact]
		public void should_correctly_use_complex_parameters1()
		{
			int? param = 5;
			Query
				.From<Products>()
				.Where(p => p.ProductName == param.ToString())
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName] 
				  FROM [dbo].[Products] AS [p] 
				  WHERE ([p].[ProductName] = @p0)", "5");
		}

		[Fact]
		public void should_correctly_use_complex_parameters2()
		{
			int? param = 5;
			Query
				.From<Products>()
				.Where(p => p.ProductID == param.ToString().Length)
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName] 
				  FROM [dbo].[Products] AS [p] 
				  WHERE ([p].[ProductID] = @p0)", 1);
		}
		[Fact]
		public void should_correctly_use_complex_parameters3()
		{
			var param = new DateTime(2014, 2, 23);
			Query
				.From<Products>()
				.Where(p => p.ProductID == param.Day)
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName] 
				  FROM [dbo].[Products] AS [p] 
				  WHERE ([p].[ProductID] = @p0)", 23);
		}
		[Fact]
		public void should_correctly_use_complex_parameters4()
		{
			Query
				.From<Products>()
				.Where(p => p.ProductID == new DateTime(2014, 2, 23).Day)
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName] 
				  FROM [dbo].[Products] AS [p] 
				  WHERE ([p].[ProductID] = @p0)", 23);
		}

		[Fact]
		public void insert_select_identity()
		{
			var result = Query
				.Insert<Products>(p => new { p.ProductName, p.QuantityPerUnit, p.SupplierID })
							.Values(new Products() { ProductName = "P1", QuantityPerUnit = "23", SupplierID = null })
				.Select(() => new { Id = MsSql.ScopeIdentity() })
				.Verify(
				@"INSERT INTO [dbo].[Products] ([ProductName],[QuantityPerUnit],[SupplierID])
							  VALUES (@p0,@p1,NULL);

				  SELECT SCOPE_IDENTITY() AS [Id]",
				"P1".DbType(SqlDbType.NVarChar), "23".DbType(SqlDbType.NVarChar));
		}
	}
}
