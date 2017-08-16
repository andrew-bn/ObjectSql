using System.Data.SqlClient;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectSql.Test;
using Xunit;
using ObjectSql.Test.Database.TestDatabase.dbo;
using ObjectSql.Test.Database.TestDatabase;

namespace ObjectSql.Tests.IntegrationTests
{
	
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
		[Fact]
		public void Select_WithoutSource_BinaryOperation_SelectResult()
		{
			var c = "const";
			var c2 = "ant";
			var res = Query.Select(() => c + c2)
				.ExecuteScalar();

			Assert.Equal("constant", res.ScalarResult);
		}
		[Fact]
		public void Select_Anonimus_ConcatFieldWithConstant()
		{
			var c = "_const";
			var res = Query.From<Products>()
			.Select((p) => new { Fld1 = p.ProductName + c }).ExecuteQuery().ToArray();

			Assert.Equal(77, res.Length);
			Assert.Equal("Alice Mutton_const", res[0].Fld1);

		}
		[Fact]
		public void Select_Anonimus_ConcatFieldWithField()
		{
			var res = Query.From<Products>()
			.Select((p) => new { Fld1 = p.ProductName + p.ProductName }).ExecuteQuery().ToArray();

			Assert.Equal(77, res.Length);
			Assert.Equal("Alice MuttonAlice Mutton", res[0].Fld1);
		}
		public class Fo

		{
			public int? b;
		}
		public class ResutDto
		{
			public int param1 { get; set; }
			public int outParam { get; set; }
		}
		// [Fact]
		public void StoredProcedure_ObjectReader_ResultMapping()
		{
			int outP = 12;
			using (var rdr = Query.Exec<TestDatabaseProcedures>(db => db.MyProcedure(23, outP)).Returns<int>(SqlDbType.Int).ExecuteReader())
			{
				Assert.Equal(12, outP);

				var res = rdr.MapResult<ResutDto>().ToArray();
				var returnValue = rdr.MapReturnValue<int>();

				Assert.Equal(23, res[0].param1);
				Assert.Equal(12, res[0].outParam);
				Assert.Equal(43, returnValue);
				Assert.Equal(64, outP);
			}
		}
		// [Fact]
		public void StoredProcedure_NonQueryReturnMapping()
		{
			int outP = 12;
			var res = Query.Exec<TestDatabaseProcedures>(db => db.MyProcedure(23, outP)).Returns<int>(SqlDbType.Int).ExecuteNonQuery();
			Assert.Equal(43, res.MapReturnValue<int>());
			Assert.Equal(64, outP);
		}

		public enum RegionType
		{
			Wa
		}
		public class EmployeeResultWithNonNullable
		{
			public RegionType Region { get; set; }
			public string LastName { get; set; }
			public string FirstName { get; set; }
		}
		public class EmployeeResultWithNullable
		{
			public RegionType? Region { get; set; }
			public string LastName { get; set; }
			public string FirstName { get; set; }
		}
		public class SelectProductResult
		{
			public int ProductId { get; set; }
			public string ProductName { get; set; }
		}

		public enum CustomEnum
		{
			Konbu
		}

		public class SelectProductResultToEnum
		{
			public int ProductId { get; set; }
			public CustomEnum ProductName { get; set; }
		}
		public class SelectProductResultInvalid
		{
			public double ProductId { get; set; }
			public string ProductName { get; set; }
		}
		[Fact]
		public void Select_MapResult()
		{
			var res = Query.From<Products>()
							 .Select(p => new { p.ProductID, p.ProductName }).ExecuteReader();

			var data = res.MapResult(dr => new SelectProductResult
			{
				ProductId = (int)dr["ProductId"],
				ProductName = dr["ProductName"].ToString()
			}).ToArray();
			Assert.Equal(77, data.Length);
			Assert.Equal("Alice Mutton", data[0].ProductName);
		}

		[Fact]
		public void Select_MapResult_CustomDto()
		{
			var res = Query.From<Products>()
							 .Select(p => new { p.ProductID, p.ProductName }).ExecuteReader();
			var data = res.MapResult<SelectProductResult>().ToArray();
			Assert.Equal(77, data.Length);
			Assert.Equal("Alice Mutton", data[0].ProductName);
		}

		[Fact]
		public void Select_MapResult_Error_If_Map_DBNull_To_NullableEnum()
		{
			var res = Query.From<Employees>()
				.Select(p => p).ExecuteReader();

			var data = res.MapResult<EmployeeResultWithNullable>().ToArray();
			Assert.Equal(9, data.Length);
			Assert.True(data.Any(d => d.Region == null));
			Assert.True(data.Any(d => d.Region == RegionType.Wa));
		}

		[Fact]
		public void Select_MapResult_Error_If_Map_DBNull_To_NonNullable()
		{
			var res = Query.From<Employees>()
				.Select(p => p).ExecuteReader();

			var ex = Assert.Throws<InvalidCastException>(() => res.MapResult<EmployeeResultWithNonNullable>().ToArray());
			Assert.Equal("Unable to cast result set value to Field 'Region'. Possible cast error of DBNull and non nullable type", ex.Message);
		}

		[Fact]
		//[ExpectedException(typeof(InvalidCastException), ExpectedMessage = "Unable to cast result set value to Field 'ProductId'. Possible cast error of DBNull and non nullable type")]
		public void Select_MapResult_InvalidMapping()
		{
			var res = Query.From<Products>()
							 .Select(p => new { p.ProductID, p.ProductName }).ExecuteReader();
			var ex = Assert.Throws<InvalidCastException>(() => res.MapResult<SelectProductResultInvalid>().ToArray());
			Assert.Equal("Unable to cast result set value to Field 'ProductId'. Possible cast error of DBNull and non nullable type", ex.Message);

		}
		[Fact]
		public void Select_MapResult_EnumMapping()
		{
			var res = Query.From<Products>()
							.Where(p => p.ProductID == 13)
							 .Select(p => new { p.ProductID, p.ProductName }).ExecuteReader();
			var data = res.MapResult<SelectProductResultToEnum>().ToArray();
			Assert.Equal(1, data.Length);
			Assert.Equal(CustomEnum.Konbu, data[0].ProductName);
		}
		[Fact]
		public void Select_MapResult_Entity_Only_Two_Fields_Selected()
		{
			var res = Query.From<Products>()
							 .Select(p => new { p.ProductID, p.ProductName }).ExecuteReader();
			var data = res.MapResult<Products>().ToArray();
			Assert.Equal(77, data.Length);
			Assert.Equal("Alice Mutton", data[0].ProductName);
			Assert.Equal(17, data[0].ProductID);
		}
		[Fact]
		public void Select_MapResult_Entity()
		{
			var res = Query.From<Products>()
							 .Select(p => p).ExecuteReader();

			var data = res.MapResult<Products>().ToArray();
			Assert.Equal(77, data.Length);
			Assert.Equal("Alice Mutton", data[16].ProductName);
			Assert.Equal(17, data[16].ProductID);
		}

		[Fact]
		public void Select_MapResult_To_Dictionary()
		{
			var res = Query.From<Products>()
							 .Select(p => p).ExecuteReader();

			var data = res.MapResultToDictionary().ToArray();
			Assert.Equal(77, data.Length);
			Assert.Equal("Alice Mutton", data[16]["ProductName"]);
			Assert.Equal(17, data[16]["ProductID"]);
		}
		[Fact]
		public void Select_MapResult_To_Dynamic()
		{
			var res = Query.From<Products>()
							 .Select(p => p).ExecuteReader();

			var data = res.MapResultToDynamic().ToArray();
			Assert.Equal(77, data.Length);
			Assert.Equal("Alice Mutton", data[16].ProductName);
			Assert.Equal(17, data[16].ProductID);
		}

	}
}
