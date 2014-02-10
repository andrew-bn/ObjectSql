﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlBoost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Reflection;
using System.Data;
namespace SqlBoost.Tests.CommandTextGenerationTests
{
	[TestClass]
	public class ComplexSqlGenerationTests: TestBase
	{
		[TestMethod]
		public void ComplexSelectSql_TypeSchema()
		{
			var val = "val";
			var val2 = "val";
			var result = Query
				.From<Product>()
				.Join<Category>((p, c) => p.CategoryID == c.CategoryID)
				.Where((db, p, c) => p.ProductName != val && p.ReorderLevel == 2 &&
									 p.QuantityPerUnit != null || p.QuantityPerUnit != val)
				.GroupBy((db, p, c) => new { p.ProductName })
				.Where((db, p, c) => p.ProductName != val2)
				.Select((db, p, c) => new { p.ProductName, c.CategoryName })
				.Verify(
				@"SELECT [p].[ProductName] AS [ProductName],[c].[CategoryName] AS [CategoryName] 
				  FROM [Product] AS [p] 
					JOIN [Category] AS [c] ON ([p].[CategoryID] = [c].[CategoryID])
				  WHERE (((([p].[ProductName] <> @p0) AND ([p].[ReorderLevel] = @p1)) AND 
							([p].[QuantityPerUnit] IS NOT NULL)) OR ([p].[QuantityPerUnit] <> @p0))
				  GROUP BY [p].[ProductName]
				  HAVING ([p].[ProductName] <> @p2)",
					val, 2,val2);
		}
		[TestMethod]
		public void ComplexSelectSql_EfSchema()
		{
			var val = "val";
			var val2 = "val";
			var result = EfQuery
				.From<Product>()
				.Join<Category>((p, c) => p.CategoryID == c.CategoryID)
				.Where((db, p, c) => p.ProductName != val && p.ReorderLevel == 2 &&
									 p.QuantityPerUnit != null || p.QuantityPerUnit != val)
				.GroupBy((db, p, c) => new { p.ProductName })
				.Where((db, p, c) => p.ProductName != val2)
				.Select((db, p, c) => new { p.ProductName, c.CategoryName })
				.Verify(
				@"SELECT [p].[ProductName] AS [ProductName],[c].[CategoryName] AS [CategoryName] 
				  FROM [dbo].[Products] AS [p] 
					JOIN [dbo].[Categories] AS [c] ON ([p].[CategoryID] = [c].[CategoryID]) 
				  WHERE (((([p].[ProductName] <> @p0) AND ([p].[ReorderLevel] = @p1)) 
							AND ([p].[QuantityPerUnit] IS NOT NULL)) OR ([p].[QuantityPerUnit] <> @p0)) 
				  GROUP BY [p].[ProductName] 
				  HAVING ([p].[ProductName] <> @p2)",
				val.DbType(SqlDbType.NVarChar),2.DbType(SqlDbType.SmallInt),val2.DbType(SqlDbType.NVarChar));
		}
		[TestMethod]
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
		[TestMethod]
		public void ComplexDelete_EfSchema()
		{
			var val = "avsdf";
			var result = EfQuery.
							Delete<Product>((db, p) => p.CategoryID != 23 &&
													p.ProductName != val ||
													p.ReorderLevel == null)
							.Verify(@"DELETE FROM [dbo].[Products]
									 WHERE ((([CategoryID] <> @p0) AND
												([ProductName] <> @p1)) OR
												([ReorderLevel] IS NULL))",
									23.DbType(SqlDbType.Int), val.DbType(SqlDbType.NVarChar));
		}
		[TestMethod]
		public void ComplexUpdate_EfSchema()
		{
			var val = "avsdf";
			var result = EfQuery.
							Update<Product>(_ => new Product() { ProductID = 23, QuantityPerUnit = val, Discontinued = false })
							.Where((db, p) => p.UnitPrice == 23M && p.UnitsInStock == null &&
											p.ProductName != val)
							.Verify(@"UPDATE [dbo].[Products]
									  SET [ProductID] = @p0, [QuantityPerUnit] = @p1, [Discontinued] = @p2
									 WHERE ((([UnitPrice] = @p3) AND ([UnitsInStock] IS NULL)) AND ([ProductName] <> @p1))",
									23.DbType(SqlDbType.Int), val.DbType(SqlDbType.NVarChar), false.DbType(SqlDbType.Bit),
									23M.DbType(SqlDbType.Money));

		}
		[TestMethod]
		public void StoredProcedure_EfSchema()
		{
			EfQuery.StoredProcedure((TestDatabaseEntities e) => e.CustOrderHist("0"))
				.Verify("[dbo].[CustOrderHist]", 
							"0".DbType(SqlDbType.NChar).Name("CustomerID"));
		}
		[TestMethod]
		public void StoredProcedure_EfSchema_ParameterIsNull()
		{
			EfQuery.StoredProcedure((TestDatabaseEntities e) => e.CustOrderHist(null))
				.Verify("[dbo].[CustOrderHist]",
							DBNull.Value.DbType(SqlDbType.NChar).Name("CustomerID"));
		}
	}
}
