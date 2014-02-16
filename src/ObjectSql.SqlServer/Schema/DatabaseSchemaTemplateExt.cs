using System;
using System.Xml;

namespace ObjectSql.SqlServer.Schema
{
	public partial class DatabaseSchemaTemplate
	{
		public string ConnStrName { get; private set; }
		public DatabaseSchema Schema { get; private set; }
		public string Namespace { get; private set; }

		public DatabaseSchemaTemplate(string connStrName, DatabaseSchema schema, string @namespace)
		{
			ConnStrName = connStrName;
			Schema = schema;
			Namespace = @namespace;
		}
		public string ToValidName(string value)
		{
			return value.Replace(" ", "_");
		}
		public string ToTypeName(Type netType, bool nullable)
		{
			var result = "object";
			
			if (netType == typeof(long))
				result = "long";
			else if (netType == typeof (byte[]))
				result = "byte[]";
			else if (netType == typeof(Guid))
				result = "Guid";
			else if (netType == typeof(int))
				result = "int";
			else if (netType == typeof(short))
				result = "short";
			else if (netType == typeof(XmlReader))
				result = "string";
			else if (netType == typeof(byte))
				result = "byte";
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

			return (nullable && netType.IsValueType) ? result + "?" : result;
		}
	}
}