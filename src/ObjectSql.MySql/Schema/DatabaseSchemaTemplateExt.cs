using System;
using System.Data;
using System.Xml;
using ObjectSql.Core.Misc;

namespace ObjectSql.MySql.Schema
{
	public partial class DatabaseSchemaTemplate
	{
		public string ConnStrName { get; }
		public DatabaseSchema Schema { get; }
		public string Namespace { get; }

		public DatabaseSchemaTemplate(string connStrName, DatabaseSchema schema, string @namespace)
		{
			ConnStrName = connStrName;
			Schema = schema;
			Namespace = @namespace;
		}
		public bool IsOut(Parameter param)
		{
			return param.Direction == ParameterDirection.Output;
		}
		public bool IsInOut(Parameter param)
		{
			return param.Direction == ParameterDirection.InputOutput;
		}
		public string ToValidName(NameHolder name)
		{
			var value = name.Name;

			if (value.Length > 0 && char.IsDigit(value[0]))
				value = "_" + value;

			if (value.Length > 1 && value[0] == '@' && char.IsDigit(value[1]))
				value = "@_" + value.TrimStart('@');

			var result = value.Replace(" ", "_")
						.Replace("-", "_")
						.Replace("'", "")
						.Replace(".", "_")
						.Replace("$", "");

			return name.UseSchema ? name.Schema + "_" + result : result;
		}
		
		public string ToTypeName(Type netType, bool nullable)
		{
			var result = "object";
			
			if (netType == typeof(long))
				result = "long";
			else if (netType == typeof(ulong))
				result = "ulong";
			else if (netType == typeof (byte[]))
				result = "byte[]";
			else if (netType == typeof(Guid))
				result = "Guid";
			else if (netType == typeof(int))
				result = "int";
			else if (netType == typeof(uint))
				result = "uint";
			else if (netType == typeof(short))
				result = "short";
			else if (netType == typeof(ushort))
				result = "ushort";
			else if (netType == typeof(XmlReader))
				result = "string";
			else if (netType == typeof(byte))
				result = "byte";
			else if (netType == typeof(sbyte))
				result = "sbyte";
			else if (netType == typeof(bool))
				result = "bool";
			else if (netType == typeof(string))
				result = "string";
			else if (netType == typeof(DateTime))
				result = "DateTime";
			else if (netType == typeof(DateTimeOffset))
				result = "DateTimeOffset";
			else if (netType == typeof(decimal))
				result = "decimal";
			else if (netType == typeof(double))
				result = "double";
			else if (netType == typeof(float))
				result = "float";
			else if (netType == typeof(TimeSpan))
				result = "TimeSpan";

			return (nullable && netType.IsValueType()) ? result + "?" : result;
		}
	}
}