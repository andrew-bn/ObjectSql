namespace SqlBoost.Core.Bo.EntitySchema
{
	internal class StorageName
	{
		public StorageName(string name, string schema)
		{
			Name = name;
			Schema = schema;
		}

		public string Name { get; private set; }
		public string Schema { get; private set; }
	}
}
