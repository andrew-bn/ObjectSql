namespace SqlBoost.Core.Bo.EntitySchema
{
	public class StorageFieldType<TTypeEnum>: IStorageFieldType
		where TTypeEnum:struct
	{
		public StorageFieldType(TTypeEnum value)
		{
			Value = value;
		}
		public TTypeEnum Value { get; private set; }

		public override bool Equals(object obj)
		{
			var b = obj as StorageFieldType<TTypeEnum>;
			if (b == null)
				return false;

			return Equals(b.Value, Value);

		}
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}
}
