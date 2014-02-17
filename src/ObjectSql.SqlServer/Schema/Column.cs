using System;

namespace ObjectSql.SqlServer.Schema
{
	public class Column
	{
		public string Schema { get; set; }
		public string TableName { get; set; }
		public int Position { get; set; }
		public bool IsNullable { get; set; }
		public string Name { get; set; }
		public string DataType { get; set; }
		public Type NetType { get; set; }

		public override string ToString()
		{
			if (IsNullable && NetType.IsValueType)
				return string.Format("Nullable<{0}> {1}",NetType, Name);
			return string.Format("{0} {1}", NetType, Name);
		}
	}
}