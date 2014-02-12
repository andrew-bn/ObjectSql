namespace SqlBoost.Core.Bo.EntitySchema
{
	public class StorageField
	{
		public string Name { get; private set; }
		public IStorageFieldType DbType { get; private set; }
		public StorageField(string name)
			:this(name,null)
		{
		}
		public StorageField(string name, IStorageFieldType dbType)
		{
			Name = name;
			DbType = dbType;
		}
	}
}
