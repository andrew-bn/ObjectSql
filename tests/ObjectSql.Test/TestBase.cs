using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ObjectSql.QueryInterfaces;
using ObjectSql.SqlServer;
using Xunit;

namespace ObjectSql.Test
{
	public class MsSqlParameterToCheck
	{
		public object ParameterValue { get; set; }
		public SqlDbType? DbType { get; set; }
		public string ParameterName { get; set; }
	}
	public abstract class TestBase
	{
		public static string ConnectionString = "data source=(LocalDB)\\v11.0;attachdbfilename=c:\\Users\\Andrew\\Source\\Repos\\ObjectSql\\tests\\ObjectSql.Test\\TestDatabase.mdf;integrated security=True;connect timeout=30;MultipleActiveResultSets=True;App=EntityFramework";
		static TestBase()
		{
			ObjectSqlSqlServerInitializer.Initialize();
		}
		protected IQuery Query
		{
			get
			{
				return new SqlConnection(ConnectionString).CreateCommand().ObjectSql();
			}
		}
		public static string PrepareResult(string result)
		{
			return result.Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");
		}
	}

	public static class SqlEndExtenstion
	{
		public static T Verify<T>(this T sqlEnd, string expectedSql)
			where T : IQueryEnd
		{
			return Verify(sqlEnd, expectedSql, new object[0]);
		}
		public static T Verify<T>(this T sqlEnd, string expectedSql,
			params object[] dbParameters)
			where T : IQueryEnd
		{
			return Verify(sqlEnd, expectedSql, dbParameters
					.Select(p => new MsSqlParameterToCheck()
					{
						DbType = (p is MsSqlParameterToCheck) ? ((MsSqlParameterToCheck)p).DbType : null,
						ParameterValue = (p is MsSqlParameterToCheck) ? ((MsSqlParameterToCheck)p).ParameterValue : p,
						ParameterName = (p is MsSqlParameterToCheck) ? ((MsSqlParameterToCheck)p).ParameterName : null,
					}).ToArray());
		}
		public static T Verify<T>(this T sqlEnd, string expectedSql,
			params MsSqlParameterToCheck[] dbParameters)
			where T : IQueryEnd
		{
			var cmd = sqlEnd.Command;
			Assert.Equal(TestBase.PrepareResult(expectedSql), TestBase.PrepareResult(cmd.CommandText));
			Assert.Equal(dbParameters.Length, cmd.Parameters.Count);

			for (int i = 0; i < dbParameters.Length; i++)
			{
				SqlParameter param = (SqlParameter)cmd.Parameters[i];
				Assert.Equal(dbParameters[i].ParameterValue, param.Value);
				if (string.IsNullOrEmpty(dbParameters[i].ParameterName))
					Assert.Equal("p" + i, param.ParameterName);
				else Assert.Equal(dbParameters[i].ParameterName, param.ParameterName);
				if (dbParameters[i].DbType.HasValue)
					Assert.Equal(dbParameters[i].DbType.Value, param.SqlDbType);
			}

			return sqlEnd;
		}
		public static MsSqlParameterToCheck DbType(this object value, SqlDbType dbType)
		{
			return new MsSqlParameterToCheck() { DbType = dbType, ParameterValue = value };
		}
		public static MsSqlParameterToCheck Name(this int value, string name)
		{
			return new MsSqlParameterToCheck() { ParameterValue = value, ParameterName = name };
		}
		public static MsSqlParameterToCheck Name(this MsSqlParameterToCheck value, string name)
		{
			value.ParameterName = name;
			return value;
		}
	}
}