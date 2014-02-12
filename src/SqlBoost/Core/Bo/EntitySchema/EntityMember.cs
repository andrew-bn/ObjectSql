namespace SqlBoost.Core.Bo.EntitySchema
{
	public abstract class EntityMember
	{
		public StorageField StorageField { get; private set; }
		public string Name { get; private set; }
		public int Index { get; private set; }
		public bool Mapped { get { return StorageField != null; } }

		protected EntityMember(string name, int index, StorageField storageField)
		{
			StorageField = storageField;
			Name = name;
			Index = index;
		}
	}
}
