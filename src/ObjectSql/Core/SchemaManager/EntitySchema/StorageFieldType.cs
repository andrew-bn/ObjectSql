namespace ObjectSql.Core.SchemaManager.EntitySchema
{
	public class StorageFieldType<TTypeEnum>: IStorageFieldType
		where TTypeEnum:struct
	{
		public StorageFieldType(TTypeEnum value)
		{
			Value = value;
		}
		public TTypeEnum Value { get; }

		public override bool Equals(object obj)
		{
			if (!(obj is StorageFieldType<TTypeEnum> b))
			{
				return false;
			}

			return Equals(b.Value, Value);

		}
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}
}
