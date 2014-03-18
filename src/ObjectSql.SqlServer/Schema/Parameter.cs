using System;
using System.Data;

namespace ObjectSql.SqlServer.Schema
{
	public class Parameter : NameHolder
	{
		public ParameterDirection Direction { get; set; }
		public string ProcedureName { get; set; }
		public int Position { get; set; }
		public bool IsResult { get; set; }
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