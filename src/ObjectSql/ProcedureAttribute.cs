using System;

namespace ObjectSql
{
	public class ProcedureAttribute : Attribute
	{
		public string Name { get; }
		public string Schema { get; }

		public ProcedureAttribute(string name)
			: this (name, string.Empty)
		{
		}

		public ProcedureAttribute(string name, string schema)
		{
			Name = name;
			Schema = schema;
		}
	}
}
