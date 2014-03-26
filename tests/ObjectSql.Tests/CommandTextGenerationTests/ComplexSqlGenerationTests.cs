﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectSql;
using NUnit.Framework;
using System.Data.SqlClient;
using System.Reflection;
using System.Data;
using ObjectSql.SqlServer;

namespace ObjectSql.Tests.CommandTextGenerationTests
{
	[TestFixture]
	public class ComplexSqlGenerationTests : TestBase
	{
		[Test]
		public void ComplexSelectSql_TypeSchema()
		{
			var val = "val";
			var val2 = "val";
			var result = Query
				.From<Product>()
				.Join<Category>((p, c) => p.CategoryID == c.CategoryID)
				.Where((p, c) => p.ProductName != val && p.ReorderLevel == 2 &&
									 p.QuantityPerUnit != null || p.QuantityPerUnit != val)
				.GroupBy((p, c) => new { p.ProductName })
				.Where((p, c) => p.ProductName != val2)
				.Select((p, c) => new { p.ProductName, c.CategoryName })
				.Verify(
				@"SELECT [p].[ProductName] AS [ProductName],[c].[CategoryName] AS [CategoryName] 
				  FROM [Product] AS [p] 
					JOIN [Category] AS [c] ON ([p].[CategoryID] = [c].[CategoryID])
				  WHERE (((([p].[ProductName] <> @p0) AND ([p].[ReorderLevel] = @p1)) AND 
							([p].[QuantityPerUnit] IS NOT NULL)) OR ([p].[QuantityPerUnit] <> @p0))
				  GROUP BY [p].[ProductName]
				  HAVING ([p].[ProductName] <> @p2)",
					val, 2, val2);
		}
		[Test]
		public void ComplexSelectSql_TypeSchema2()
		{
			var val = "val";
			var val2 = "val";
			var result = Query
				.From<Product>()
				.Join<Category>((p, c) => p.CategoryID == c.CategoryID)
				.Where((p, c) => p.ProductName != val && p.ReorderLevel == 2 &&
									 p.QuantityPerUnit != null || p.QuantityPerUnit != val)
				.GroupBy((p, c) => new { p.ProductName })
				.Where((p, c) => p.ProductName != val2)
				.Select((p, c) => new { p, c })
				.Verify(
				@"SELECT [p].[ProductName] AS [ProductName],[c].[CategoryName] AS [CategoryName] 
				  FROM [Product] AS [p] 
					JOIN [Category] AS [c] ON ([p].[CategoryID] = [c].[CategoryID])
				  WHERE (((([p].[ProductName] <> @p0) AND ([p].[ReorderLevel] = @p1)) AND 
							([p].[QuantityPerUnit] IS NOT NULL)) OR ([p].[QuantityPerUnit] <> @p0))
				  GROUP BY [p].[ProductName]
				  HAVING ([p].[ProductName] <> @p2)",
					val, 2, val2);
		}
		[Test]
		public void ComplexSelectSql_EfSchema()
		{
			var val = "val";
			var val2 = "val";
			var result = EfQuery
				.From<Product>()
				.Join<Category>((p, c) => p.CategoryID == c.CategoryID)
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
		[Test]
		public void ComplextInsert_EfSchema()
		{
			var result = EfQuery
							.Insert<Product>(p => new { p.ProductName, p.QuantityPerUnit, p.SupplierID })
							.Values(new Product() { ProductName = "P1", QuantityPerUnit = "23", SupplierID = null },
									new Product() { ProductName = "P2", QuantityPerUnit = "223", SupplierID = 2 })
							.Verify(
							@"INSERT INTO [dbo].[Products] ([ProductName],[QuantityPerUnit],[SupplierID])
							  VALUES (@p0,@p1,NULL),(@p2,@p3,@p4)",
							"P1".DbType(SqlDbType.NVarChar), "23".DbType(SqlDbType.NVarChar),
							"P2".DbType(SqlDbType.NVarChar), "223".DbType(SqlDbType.NVarChar), 2.DbType(SqlDbType.Int));
		}
		[Test]
		public void ComplextInsert_EfSchema_Null_Param()
		{
			var result = EfQuery
							.Insert<Product>(p => new { p.ProductName, p.QuantityPerUnit, p.SupplierID })
							.Values(new Product() { ProductName = "P1", QuantityPerUnit = "23", SupplierID = null },
									new Product() { ProductName = "P2", QuantityPerUnit = "223", SupplierID = 2 })
							.Verify(
							@"INSERT INTO [dbo].[Products] ([ProductName],[QuantityPerUnit],[SupplierID])
							  VALUES (@p0,@p1,NULL),(@p2,@p3,@p4)",
							"P1".DbType(SqlDbType.NVarChar), "23".DbType(SqlDbType.NVarChar),
							"P2".DbType(SqlDbType.NVarChar), "223".DbType(SqlDbType.NVarChar), 2.DbType(SqlDbType.Int));
		}
		[Test]
		public void ComplexDelete_EfSchema()
		{
			var val = "avsdf";
			var result = EfQuery.
							Delete<Product>((p) => p.CategoryID != 23 &&
													p.ProductName != val ||
													p.ReorderLevel == null)
							.Verify(@"DELETE FROM [dbo].[Products]
									 WHERE ((([CategoryID] <> @p0) AND
												([ProductName] <> @p1)) OR
												([ReorderLevel] IS NULL))",
									23.DbType(SqlDbType.Int), val.DbType(SqlDbType.NVarChar));
		}
		[Test]
		public void ComplexUpdate_EfSchema()
		{
			var val = "avsdf";
			var result = EfQuery.
							Update(() => new Product() { ProductID = 23, QuantityPerUnit = val, Discontinued = false })
							.Where((p) => p.UnitPrice == 23M && p.UnitsInStock == null &&
											p.ProductName != val)
							.Verify(@"UPDATE [dbo].[Products]
									  SET [ProductID] = @p0, [QuantityPerUnit] = @p1, [Discontinued] = @p2
									 WHERE ((([UnitPrice] = @p3) AND ([UnitsInStock] IS NULL)) AND ([ProductName] <> @p1))",
									23.DbType(SqlDbType.Int), val.DbType(SqlDbType.NVarChar), false.DbType(SqlDbType.Bit),
									23M.DbType(SqlDbType.Money));

		}
		[Test]
		public void StoredProcedure_EfSchema()
		{
			EfQuery.Exec((TestDatabaseEntities e) => e.CustOrderHist("0"))
				.Verify("[dbo].[CustOrderHist]",
							"0".DbType(SqlDbType.NChar).Name("CustomerID"));
		}
		[Test]
		public void StoredProcedure_EfSchema_ParameterIsNull()
		{
			EfQuery.Exec((TestDatabaseEntities e) => e.CustOrderHist(null))
				.Verify("[dbo].[CustOrderHist]",
							DBNull.Value.DbType(SqlDbType.NChar).Name("CustomerID"));
		}

		[Test]
		public void StoredProcedure_AutoGeneratedSp()
		{

			Query.Exec<TestDatabaseProcedures>(d => d.CustOrderHist(null))
				.Verify("[dbo].[CustOrderHist]",
							DBNull.Value.DbType(SqlDbType.NChar).Name("CustomerID"));
		}

		[Test]
		public void StoredProcedure_AutoGeneratedSp_ComplexName()
		{
			Query.Exec<TestDatabaseProcedures>(d => d.Ten_Most_Expensive_Products())
				 .Verify("[dbo].[Ten Most Expensive Products]");
		}

		[Test]
		public void ComplexSelectSql_ButchQuery()
		{
			var val = "val";
			var val2 = "val";
			var result = Query
				//select p.productname as productname, c.categoryname as categoryname
				.From<Product>()
				.Join<Category>((p, c) => p.CategoryID == c.CategoryID)
				.Where((p, c) => p.ProductName != val && p.ReorderLevel == 2 &&
									 p.QuantityPerUnit != null || p.QuantityPerUnit != val)
				.GroupBy((p, c) => new { p.ProductName })
				.Where((p, c) => p.ProductName != val2)
				.Select((p, c) => new { p.ProductName, c.CategoryName })

				.From<Product>()
				.Join<Category>((p, c) => p.CategoryID == c.CategoryID)
				.Where((p, c) => p.ProductName != val && p.ReorderLevel == 2 &&
									 p.QuantityPerUnit != null || p.QuantityPerUnit != val)
				.Select((p, c) => p.ProductName)

				.Update(() => new Product { ProductName = "ProductName" })
				.Where(p => p.ProductID == 22)
				
				.Verify(
				@"SELECT [p].[ProductName] AS [ProductName],[c].[CategoryName] AS [CategoryName] 
				  FROM [Product] AS [p] 
					JOIN [Category] AS [c] ON ([p].[CategoryID] = [c].[CategoryID])
				  WHERE (((([p].[ProductName] <> @p0) AND ([p].[ReorderLevel] = @p1)) AND 
							([p].[QuantityPerUnit] IS NOT NULL)) OR ([p].[QuantityPerUnit] <> @p0))
				  GROUP BY [p].[ProductName]
				  HAVING ([p].[ProductName] <> @p2);

				  SELECT [p].[ProductName]
				  FROM [Product] AS [p] 
					JOIN [Category] AS [c] ON ([p].[CategoryID] = [c].[CategoryID])
				  WHERE (((([p].[ProductName] <> @p0) AND ([p].[ReorderLevel] = @p1)) AND 
							([p].[QuantityPerUnit] IS NOT NULL)) OR ([p].[QuantityPerUnit] <> @p0));

				  UPDATE [Product] SET [ProductName] = @p3 
				  WHERE([ProductID] = @p4 )",
					val, 2, val2, "ProductName", 22);
		}
		[Test]
		public void select_with_in_array()
		{
			var p1 = "pn";
			var result = Query
				.From<Product>()
				.Where(p => p.ProductName == p1 && p.CategoryID.In(11,22,33))
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName] 
				FROM [Product] AS [p] 
				WHERE (([p].[ProductName] = @p0) AND  
					   ([p].[CategoryID] IN (@p1, @p2, @p3)))",
				  p1, 11, 22,33);
		}
		[Test]
		public void select_with_in_array_of_strings()
		{
			var p1 = "pn";
			var result = Query
				.From<Product>()
				.Where(p => p.ProductName.In("name1","name2"))
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName] 
				FROM [Product] AS [p] 
				WHERE ([p].[ProductName] IN (@p0, @p1))",
				  "name1", "name2");
		}
		[Test]
		public void select_with_not_in_array()
		{
			var p1 = "pn";
			var result = Query
				.From<Product>()
				.Where(p => p.ProductName == p1 && p.CategoryID.NotIn(11, 22, 33))
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName] 
				FROM [Product] AS [p] 
				WHERE (([p].[ProductName] = @p0) AND  
					   ([p].[CategoryID] NOT IN (@p1, @p2, @p3)))",
				  p1, 11, 22, 33);
		}
		[Test]
		public void select_with_in_array_variable()
		{
			var p1 = "pn";
			var array_variable = new[] {11, 22, 33};
			var result = Query
				.From<Product>()
				.Where(p => p.ProductName == p1 && p.CategoryID.In(array_variable) &&
					p.ProductID.In(array_variable))
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName] 
				FROM [Product] AS [p] 
				WHERE ((([p].[ProductName] = @p0) AND  
					   ([p].[CategoryID] IN (@p1_0, @p1_1, @p1_2))) AND
                       ([p].[ProductID] IN (@p1_0, @p1_1, @p1_2)))",

				  p1, 11.Name("p1_0"), 22.Name("p1_1"), 33.Name("p1_2"));
		}
		[Test]
		public void select_with_subquery()
		{
			var p1 = "pn";
			var p2 = "p324";
			var result = Query
				.From<Product>()
				.Where(p=> p.ProductName == p1 && p.CategoryID.In(Sql.Query.From<Category>()
																			.Where(c=>c.CategoryName == p2 && c.Description != p1)
																			.Select(c=>c.CategoryID))
							&& p.ProductName != p2
					  )
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName]
				  FROM [Product] AS [p] 
				  WHERE ((([p].[ProductName]=@p0) AND 
						 ([p].[CategoryID] IN (SELECT [c].[CategoryID]
												FROM [Category] AS [c]
												WHERE (([c].[CategoryName]=@p1) AND ([c].[Description]<>@p0))))) AND
						  ([p].[ProductName]<>@p1))",
				  p1, p2);
		}

		[Test]
		public void select_with_subquery_that_uses_main_querytable()
		{
			Query
				.From<Product>()
				.Where(p => p.CategoryID.In(Sql.Query.From<Category>()
													 .Where(c => c.CategoryName == p.ProductName || 
																p.CategoryID == c.CategoryID)
													 .Select(c => c.CategoryID)))
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName] 
				  FROM [Product] AS [p] 
				  WHERE  ([p].[CategoryID] IN (SELECT [c].[CategoryID] 
				  							   FROM [Category] AS [c] 
				  							   WHERE (([c].[CategoryName] = [p].[ProductName]) OR ([p].[CategoryID] = [c].[CategoryID]))))");
		}

		[Test]
		public void select_with_nested_subquery_that_uses_main_querytable()
		{
			Query
				.From<Product>()
				.Where(p => p.CategoryID.In(Sql.Query.From<Category>()
													 .Where(c => c.CategoryName == p.ProductName &&
																c.CategoryID.NotIn(Sql.Query.From<Product>()
																							 .Where(pr=>pr.ProductID != p.ProductID &&
																									pr.CategoryID != c.CategoryID)
																							 .Select(pr=>pr.CategoryID)))
													 .Select(c => c.CategoryID)))
				.Select(p => p.ProductName)
				.Verify(
				@"SELECT [p].[ProductName] 
FROM [Product] AS [p] 
WHERE  ([p].[CategoryID] IN (SELECT [c].[CategoryID] 
							 FROM [Category] AS [c] 
							 WHERE (([c].[CategoryName] = [p].[ProductName]) AND  
									([c].[CategoryID] NOT IN (SELECT [pr].[CategoryID] 
															  FROM [Product] AS [pr] 
															  WHERE (([pr].[ProductID] <> [p].[ProductID]) AND 
																	([pr].[CategoryID] <> [c].[CategoryID])))
									 )))
		) ");
		}
		[Test]
		public void ComplexSelectSql_LeftJoin()
		{
			Query
				.From<Product>()
				.LeftJoin<Category>((p, c) => p.CategoryID == c.CategoryID)
				.Select((p, c) => new { p.ProductName, c.CategoryName })
				.Verify(
				@"SELECT [p].[ProductName] AS [ProductName],[c].[CategoryName] AS [CategoryName] 
				  FROM [Product] AS [p] 
				  LEFT JOIN [Category] AS [c] ON ([p].[CategoryID] = [c].[CategoryID])");
		}
		[Test]
		public void ComplexSelectSql_RightJoin()
		{
			Query
				.From<Product>()
				.RightJoin<Category>((p, c) => p.CategoryID == c.CategoryID)
				.Select((p, c) => new { p.ProductName, c.CategoryName })
				.Verify(
				@"SELECT [p].[ProductName] AS [ProductName],[c].[CategoryName] AS [CategoryName] 
				  FROM [Product] AS [p] 
				  RIGHT JOIN [Category] AS [c] ON ([p].[CategoryID] = [c].[CategoryID])");
		}

		[Test]
		public void ComplexSelectSql_FullJoin()
		{
			Query
				.From<Product>()
				.FullJoin<Category>((p, c) => p.CategoryID == c.CategoryID)
				.Select((p, c) => new { p.ProductName, c.CategoryName })
				.Verify(
				@"SELECT [p].[ProductName] AS [ProductName],[c].[CategoryName] AS [CategoryName] 
				  FROM [Product] AS [p] 
				  FULL JOIN [Category] AS [c] ON ([p].[CategoryID] = [c].[CategoryID])");
		}

		[Test]
		public void BuildSql_OutputClause()
		{
			EfQuery
				.Insert<Product>(p => new { p.ProductName, p.QuantityPerUnit, p.SupplierID })
				.Output((i,d) => i)
				.Values(new Product() { ProductName = "P1", QuantityPerUnit = "23", SupplierID = null },
						new Product() { ProductName = "P2", QuantityPerUnit = "223", SupplierID = 2 })
				.Verify(
				@"INSERT INTO [dbo].[Products] ([ProductName],[QuantityPerUnit],[SupplierID])
				  OUTPUT [INSERTED].[ProductID], [INSERTED].[ProductName], [INSERTED].[SupplierID], [INSERTED].[CategoryID], [INSERTED].[QuantityPerUnit], [INSERTED].[UnitPrice], [INSERTED].[UnitsInStock], [INSERTED].[UnitsOnOrder], [INSERTED].[ReorderLevel], [INSERTED].[Discontinued]
				  VALUES (@p0,@p1,NULL),(@p2,@p3,@p4)",
				"P1".DbType(SqlDbType.NVarChar), "23".DbType(SqlDbType.NVarChar),
				"P2".DbType(SqlDbType.NVarChar), "223".DbType(SqlDbType.NVarChar), 2.DbType(SqlDbType.Int));
		}
	}
}
