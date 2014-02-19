using System.Data;

namespace ObjectSql.Core.Bo.EntitySchema
{
	public class StorageParameter:StorageField
	{
		public ParameterDirection Direction { get; private set; }
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
