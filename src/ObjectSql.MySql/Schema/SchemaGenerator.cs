using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Xml;

namespace ObjectSql.MySql.Schema
{
	public class SchemaGenerator
	{
		public static string Generate(string ns, string connStrName, string connectionString)
		{
			if (ns == null)
				ns = "";

			var cs = new MySqlConnectionStringBuilder(connectionString);
			var schemaName = cs.Database;

			using (var connection = new MySqlConnection(connectionString))
			{
				connection.Open();
				var t = Tables(connection, schemaName).ToArray();
				var p = ProceduresAndFunctions(connection, schemaName).ToArray();

				FillTableParameters(t, connection, schemaName);
				FillProcedureParameters(p, connection, schemaName);

				t = t.OrderBy(tbl => tbl.Schema + "." + tbl.Name).ToArray();
				p = p.OrderBy(proc => proc.Schema + "." + proc.Name).ToArray();

				foreach (var tbl in t)
					tbl.Columns = tbl.Columns.OrderBy(c => c.Position).ToList();

				foreach (var proc in p)
				{
					proc.Parameters = proc.Parameters.OrderBy(par => par.Position).ToList();

					if (p.Any(pr => pr != proc && pr.Name.Equals(proc.Name, StringComparison.OrdinalIgnoreCase)))
						proc.UseSchema = true;
				}

				var schema = new DatabaseSchema()
				{
					Tables = t.ToArray(),
					Procedures = p.Where(prc => prc.RoutineType == RoutineType.Procedure).ToArray(),
					Functions = p.Where(f => f.RoutineType == RoutineType.Function).ToArray()
				};

				var template = new DatabaseSchemaTemplate(connStrName, schema, ns);
				return template.TransformText();
			}
		}
		private static void FillTableParameters(IEnumerable<Table> tables, MySqlConnection connection, string schemaName)
		{
			using (var cmd = connection.CreateCommand())
			{
				cmd.Parameters.AddWithValue("@schemaName", schemaName);
				cmd.CommandText = "SELECT * FROM information_schema.COLUMNS " +
								  "WHERE `TABLE_SCHEMA` = @schemaName";
				using (var row = cmd.ExecuteReader())
				{
					while (row.Read())
					{
						var column = new Column();
						column.Schema = row["TABLE_SCHEMA"].ToString();
						column.TableName = row["TABLE_NAME"].ToString();
						column.Position = int.Parse(row["ORDINAL_POSITION"].ToString());
						column.IsNullable = row["IS_NULLABLE"].ToString().ToLower() == "yes";
						column.Name = (row["COLUMN_NAME"] is DBNull) ? null : row["COLUMN_NAME"].ToString();
						column.DataType = row["DATA_TYPE"].ToString();
						var rawType = row["COLUMN_TYPE"].ToString().ToLower();
						column.NetType = MapToNetType(column.DataType, column.IsNullable, rawType.Contains("unsigned"), rawType.Contains("tinyint(1)"));
						tables.First(p => p.Name == column.TableName && p.Schema == column.Schema).Columns.Add(column);
					}
				}
			}
		}

		private static void FillProcedureParameters(IEnumerable<Procedure> procedures, MySqlConnection connection, string schemaName)
		{
			using (var cmd = connection.CreateCommand())
			{
				cmd.Parameters.AddWithValue("@schemaName", schemaName);
				cmd.CommandText = "SELECT * FROM information_schema.PARAMETERS " +
								  "WHERE `SPECIFIC_SCHEMA` = @schemaName";

				using (var row = cmd.ExecuteReader())
				{
					while (row.Read())
					{
						var param = new Parameter();

						param.Schema = row["SPECIFIC_SCHEMA"].ToString();
						param.ProcedureName = row["SPECIFIC_NAME"].ToString();
						param.Direction = ParseParameterMode(row["PARAMETER_MODE"].ToString());
						param.Position = int.Parse(row["ORDINAL_POSITION"].ToString());
						param.IsResult = false;
						param.Name = (row["PARAMETER_NAME"] is DBNull) ? null : row["PARAMETER_NAME"].ToString();
						param.DataType = row["DATA_TYPE"].ToString();
						var rawType = row["DTD_IDENTIFIER"].ToString().ToLower();
						param.NetType = MapToNetType(param.DataType, true, rawType.Contains("unsigned"), rawType.Contains("tinyint(1)"));

						if (param.Position == 0)
							param.Direction = ParameterDirection.ReturnValue;

						procedures.First(p => p.Name == param.ProcedureName && p.Schema == param.Schema).Parameters.Add(param);

					}
				}
			}
		}

		private static ParameterDirection ParseParameterMode(string value)
		{
			value = value.ToLower();
			if (value == "in")
				return ParameterDirection.Input;
			if (value == "out")
				return ParameterDirection.Output;
			if (value == "inout")
				return ParameterDirection.InputOutput;
			return ParameterDirection.ReturnValue;
		}

		private static IEnumerable<Table> Tables(MySqlConnection connection, string schemaName)
		{
			using (var cmd = connection.CreateCommand())
			{
				cmd.Parameters.AddWithValue("@schemaName", schemaName);
				cmd.CommandText = "SELECT * FROM information_schema.tables " +
								  "WHERE `TABLE_SCHEMA` = @schemaName";
				using (var r = cmd.ExecuteReader())
				{
					while (r.Read())
					{
						var p = new Table();
						p.Schema = r["TABLE_SCHEMA"].ToString();
						p.Name = r["TABLE_NAME"].ToString();
						yield return p;
					}
				}
			}
		}

		private static IEnumerable<Procedure> ProceduresAndFunctions(MySqlConnection connection, string schemaName)
		{
			using (var cmd = connection.CreateCommand())
			{
				cmd.Parameters.AddWithValue("@schemaName", schemaName);
				cmd.CommandText = "SELECT * FROM information_schema.ROUTINES " +
								  "WHERE `ROUTINE_SCHEMA` = @schemaName";
				using (var row = cmd.ExecuteReader())
				{
					while (row.Read())
					{
						var p = new Procedure();
						p.Schema = row["ROUTINE_SCHEMA"].ToString();
						p.Name = row["ROUTINE_NAME"].ToString();
						p.RoutineType = (row["ROUTINE_TYPE"].ToString().ToLower() == "procedure")
											? RoutineType.Procedure
											: RoutineType.Function;
						yield return p;
					}
				}
			}
		}

		private static Type MapToNetType(string dataType, bool nullable, bool unsigned, bool isTiniint1)
		{
			if (isTiniint1)
			{
				return typeof(bool);
			}

			dataType = dataType.ToLower();
			switch (dataType)
			{
				case "bigint":
					return unsigned ? typeof(ulong) : typeof(long);
				case "binary":
				case "blob":
				case "tinyblob":
				case "longblob":
				case "filestream":
				case "image":
				case "rowversion":
				case "varbinary":
					return typeof(byte[]);
				case "int":
				case "mediumint":
					return unsigned ? typeof(uint) : typeof(int);
				case "smallint":
					return unsigned ? typeof(ushort) : typeof(short);
				case "xml":
					return typeof(XmlReader);
				case "tinyint":
					return unsigned ? typeof(byte) : typeof(sbyte);
				case "bit":
					return typeof(bool);
				case "char":
				case "nchar":
				case "ntext":
				case "text":
				case "mediumtext":
				case "tinytext":
				case "longtext":
				case "json":
				case "nvarchar":
				case "varchar":
					return typeof(string);
				case "date":
				case "datetime":
				case "datetime2":
				case "smalldatetime":
				case "timestamp":
					return typeof(DateTime);
				case "datetimeoffset":
					return typeof(DateTimeOffset);
				case "decimal":
				case "money":
				case "numeric":
				case "smallmoney":
					return typeof(decimal);
				case "double":
					return typeof(double);
				case "float":
					return typeof(float);
				case "sql_variant":
					return typeof(object);
				case "time":
					return typeof(TimeSpan);
				default:
					return typeof(object);
			}
		}
	}
}
