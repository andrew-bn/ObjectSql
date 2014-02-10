using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlBoost;
using SqlBoost.Core.Misc;
using SqlBoost.QueryInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlBoost.Tests
{
	public class MsSqlParameterToCheck
	{
		public object ParameterValue { get; set; }
		public SqlDbType? DbType { get; set; }
		public string ParameterName { get; set; }
	}
	public static class SqlEndExtenstion
	{
		public static T Verify<T>(this T sqlEnd, string expectedSql)
			where T : ISqlEnd
		{
			return Verify(sqlEnd, expectedSql, new object[0]);
		}
		public static T Verify<T>(this T sqlEnd, string expectedSql,
			params object[] dbParameters)
			where T : ISqlEnd
		{
			return Verify(sqlEnd, expectedSql, dbParameters.Select(p => new MsSqlParameterToCheck() { DbType = null, ParameterValue = p }).ToArray());
		}
		public static string Prepare(this string str)
		{
			return TestBase.PrepareResult(str);
		}
		public static bool AreEqual(this Expression a, Expression b)
		{
			return ExpressionComparer.AreEqual(a, b);
		}
		public static bool AreEqual<T>(this Expression a, Expression<Func<T>> b)
		{
			return ExpressionComparer.AreEqual(a, b.Body);

		}
		public static T Verify<T>(this T sqlEnd, string expectedSql,
			params MsSqlParameterToCheck[] dbParameters)
			where T : ISqlEnd
		{
			var cmd = sqlEnd.Command;
			Assert.AreEqual(TestBase.PrepareResult(expectedSql), TestBase.PrepareResult(cmd.CommandText));
			Assert.AreEqual(dbParameters.Length, cmd.Parameters.Count);

			for (int i = 0; i < dbParameters.Length; i++)
			{
				SqlParameter param = (SqlParameter)cmd.Parameters[i];
				Assert.AreEqual(dbParameters[i].ParameterValue, param.Value);
				if (string.IsNullOrEmpty(dbParameters[i].ParameterName))
					Assert.AreEqual("p" + i, param.ParameterName);
				else Assert.AreEqual(dbParameters[i].ParameterName,param.ParameterName);
				if (dbParameters[i].DbType.HasValue)
					Assert.AreEqual(dbParameters[i].DbType.Value, param.SqlDbType);
			}

			return sqlEnd;
		}
		public static MsSqlParameterToCheck DbType(this object value, SqlDbType dbType)
		{
			return new MsSqlParameterToCheck() { DbType = dbType, ParameterValue = value };
		}
		public static MsSqlParameterToCheck Name(this MsSqlParameterToCheck value, string name)
		{
			value.ParameterName = name;
			return value;
		}
	}
	[TestClass]
	public abstract class TestBase
	{
		protected static string EfConnectionString
		{
			get
			{
				return System.Configuration.ConfigurationManager.ConnectionStrings["TestDatabaseEntities"].ConnectionString;
			}
		}
		static SqlBoostManager<SqlConnection> _sqlManager = new SqlBoostManager<SqlConnection>(EfConnectionString);
		
		protected ISql EfQuery
		{
			get
			{
				return _sqlManager.Query();
			}
		}
		protected ISql Query
		{
			get
			{
				return new SqlConnection().CreateCommand().Query();
			}
		}
		public static string PrepareResult(string result)
		{
			return result.Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");
		}
	}
}
