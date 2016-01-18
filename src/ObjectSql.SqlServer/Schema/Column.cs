using System;
using ObjectSql.Core.Misc;

namespace ObjectSql.SqlServer.Schema
{
	public class Column : NameHolder
	{
		public string TableName { get; set; }
		public int Position { get; set; }
		public bool IsNullable { get; set; }
		public string DataType { get; set; }
		public Type NetType { get; set; }

		public override string ToString()
		{
			if (IsNullable && NetType.IsValueType())
				return string.Format("Nullable<{0}> {1}",NetType, Name);
			return string.Format("{0} {1}", NetType, Name);
		}
	}
}