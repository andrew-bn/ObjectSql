using System.Data;

namespace ObjectSql.Core.Bo.EntitySchema
{
	public class StorageParameter:StorageField
	{
		public ParameterDirection Direction { get; private set; }
		public bool IsOut { get { return Direction == ParameterDirection.Output || Direction == ParameterDirection.InputOutput; } }
		public bool IsIn { get { return Direction == ParameterDirection.Input; } }

		public StorageParameter(string name,ParameterDirection direction)
			:base(name)
		{
			Direction = direction;
		}
		public StorageParameter(string name, IStorageFieldType dbType,ParameterDirection direction)
			:base(name,dbType)
		{
			Direction = direction;
		}
	}
}
