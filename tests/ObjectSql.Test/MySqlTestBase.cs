using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using ObjectSql.MySql;
using ObjectSql.QueryInterfaces;
using ObjectSql.SqlServer;
using Xunit;

namespace ObjectSql.Test
{
	public class MySqlParameterToCheck
	{
		public object ParameterValue { get; set; }
		public MySqlDbType? DbType { get; set; }
		public string ParameterName { get; set; }
	}

	public abstract class MySqlTestBase
	{
		public static string ConnectionString;
		static MySqlTestBase()
		{
			var path = Directory.GetCurrentDirectory();
			ConnectionString =
				$@"Server=127.0.0.1;Database=gg_PowerDns;Uid=root;Pwd=mariadb;";
				//$@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={path}\TestDatabase.mdf;Integrated Security=True;Connect Timeout=30";
			ObjectSqlMySqlInitializer.Initialize();
		}
		protected IQuery Query => new MySqlConnection(ConnectionString).CreateCommand().ObjectSql();

		public static string PrepareResult(string result)
		{
			return result.Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");
		}
	}

	public static class MySqlEndExtenstion
	{
		public static T MySqlVerify<T>(this T sqlEnd, string expectedSql)
			where T : IQueryEnd
		{
			return MySqlVerify(sqlEnd, expectedSql, new object[0]);
		}
		public static T MySqlVerify<T>(this T sqlEnd, string expectedSql,
			params object[] dbParameters)
			where T : IQueryEnd
		{
			return MySqlVerify(sqlEnd, expectedSql, dbParameters
					.Select(p => new MySqlParameterToCheck()
					{
						DbType = (p as MySqlParameterToCheck)?.DbType,
						ParameterValue = (p is MySqlParameterToCheck) ? ((MySqlParameterToCheck)p).ParameterValue : p,
						ParameterName = (p as MySqlParameterToCheck)?.ParameterName,
					}).ToArray());
		}
		public static T MySqlVerify<T>(this T sqlEnd, string expectedSql,
			params MySqlParameterToCheck[] dbParameters)
			where T : IQueryEnd
		{
			var cmd = sqlEnd.Command;
			Assert.Equal(TestBase.PrepareResult(expectedSql), TestBase.PrepareResult(cmd.CommandText));
			Assert.Equal(dbParameters.Length, cmd.Parameters.Count);

			for (int i = 0; i < dbParameters.Length; i++)
			{
				MySqlParameter param = (MySqlParameter)cmd.Parameters[i];
				Assert.Equal(dbParameters[i].ParameterValue, param.Value);
				if (string.IsNullOrEmpty(dbParameters[i].ParameterName))
					Assert.Equal("p" + i, param.ParameterName);
				else Assert.Equal(dbParameters[i].ParameterName, param.ParameterName);
				if (dbParameters[i].DbType.HasValue)
					Assert.Equal(dbParameters[i].DbType.Value, param.MySqlDbType);
			}

			return sqlEnd;
		}
		public static MySqlParameterToCheck MySqlDbType(this object value, MySqlDbType dbType)
		{
			return new MySqlParameterToCheck() { DbType = dbType, ParameterValue = value };
		}
		public static MySqlParameterToCheck MySqlName(this int value, string name)
		{
			return new MySqlParameterToCheck() { ParameterValue = value, ParameterName = name };
		}
		public static MySqlParameterToCheck MySqlName(this MySqlParameterToCheck value, string name)
		{
			value.ParameterName = name;
			return value;
		}
	}
}