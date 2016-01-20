using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ObjectSql.QueryInterfaces;
using ObjectSql.SqlServer;

namespace ObjectSql.Test
{
	public class LoadTestHolder
	{
		static int _res;
		public class Foo
		{
			public Foo() { }
			public Foo(int param) { }
			public string Param1 { get; set; }
			public int Method()
			{
				return 0;
			}
			public bool Param3 { get; set; }
			public Foo FooParam { get; set; }
		}

		private static ObjectSqlManager<SqlConnection> _manager;
		public class ParamsHolder
		{
			public string Val { get; set; }
			public string Val2 { get; set; }
		}
		public class Result
		{
			public string P1 { get; set; }
			public string P2 { get; set; }
		}

		public static Func<ParamsHolder, IQueryEnd<Result>> _compiledQuery;
		public static void Run()
		{
			ObjectSqlSqlServerInitializer.Initialize();
			_manager = new ObjectSqlManager<SqlConnection>("");
			_compiledQuery = _manager.CompileQuery(
				(ParamsHolder param) =>
				_manager.Query()
						.From<Product>()
						.Join<Category>((p, c) => p.CategoryID == c.CategoryID)
						.Where((p, c) => p.ProductName != param.Val && p.ReorderLevel == 2 &&
											 p.QuantityPerUnit != null || p.QuantityPerUnit != param.Val)
						.GroupBy((p, c) => new { p.ProductName })
						.Where((p, c) => p.ProductName != param.Val2)
						.Select((p, c) => new Result { P1 = p.ProductName, P2 = c.CategoryName })
				);


			Console.WriteLine("Warming up...");
			TestMethod();
			Console.WriteLine("Start");
			Stopwatch sw = new Stopwatch();
			sw.Start();
			LoadTest();
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			Console.WriteLine(_res);
		}
		public static void LoadTest()
		{
			for (int i = 0; i < 300000; i++)
				TestMethod2();
		}
		public static void TestMethod2()
		{
			var val = "val";
			var val2 = "val2";

			var result = _compiledQuery(new ParamsHolder() { Val = val, Val2 = val2 }).Command;
		}
		public static void TestMethod()
		{
			var val = "val";
			var val2 = "val2";

			var result = new SqlConnection().CreateCommand().ObjectSql()
				.From<Product>()
				.Join<Category>((p, c) => p.CategoryID == c.CategoryID)
				.Where((p, c) => p.ProductName != val && p.ReorderLevel == 2 &&
									 p.QuantityPerUnit != null || p.QuantityPerUnit != val)
				.GroupBy((p, c) => new { p.ProductName })
				.Where((p, c) => p.ProductName != val2)
				.Select((p, c) => new { p.ProductName, c.CategoryName })
				.Command;
		}
		public partial class Category
		{
			public int CategoryID { get; set; }
			public string CategoryName { get; set; }
			public string Description { get; set; }
			public byte[] Picture { get; set; }
		}
		public partial class Product
		{
			public int ProductID { get; set; }
			public string ProductName { get; set; }
			public Nullable<int> SupplierID { get; set; }
			public Nullable<int> CategoryID { get; set; }
			public string QuantityPerUnit { get; set; }
			public Nullable<decimal> UnitPrice { get; set; }
			public Nullable<short> UnitsInStock { get; set; }
			public Nullable<short> UnitsOnOrder { get; set; }
			public Nullable<short> ReorderLevel { get; set; }
			public bool Discontinued { get; set; }
		}
	}
}
