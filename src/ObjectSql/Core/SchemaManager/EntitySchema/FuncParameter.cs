namespace ObjectSql.Core.SchemaManager.EntitySchema
{
	public class FuncParameter : EntityMember
	{
		public StorageParameter StorageParameter { get { return (StorageParameter)StorageField; } }
		public FuncParameter(string name, int index, StorageParameter storageField)
			: base(name, index, storageField)
		{
		}
	}

}
