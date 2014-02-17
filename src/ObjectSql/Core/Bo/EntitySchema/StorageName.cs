namespace ObjectSql.Core.Bo.EntitySchema
{
	public class StorageName
	{

		public StorageName(bool nameOnly, string name,string schema)
		{
			NameOnly = nameOnly;
			Name = name;
			Schema = schema;
			NameOnly = false;
		}

		public bool NameOnly { get; private set; }
		public string Name { get; private set; }
		public string Schema { get; private set; }
	}
}
