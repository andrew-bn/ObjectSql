using System;

namespace ObjectSql.SqlServer.Schema
{
	public class Parameter
	{
		public string Schema { get; set; }
		public ParameterType ParameterType { get; set; }
		public string ProcedureName { get; set; }
		public int Position { get; set; }
		public bool IsResult { get; set; }
		public string Name { get; set; }
		public string DataType { get; set; }
		public Type NetType { get; set; }

		public override string ToString()
		{
			if (NetType.IsValueType)
				return string.Format("Nullable<{0}> {1}", NetType, Name);
			return string.Format("{0} {1}", NetType, Name);
		}
	}
}