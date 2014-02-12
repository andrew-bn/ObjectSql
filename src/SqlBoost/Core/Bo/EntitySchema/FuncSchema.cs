namespace SqlBoost.Core.Bo.EntitySchema
{
	public class FuncSchema
	{
		public FuncSchema(StorageName storageName, FuncParameter[] funcParameters)
		{
			StorageName = storageName;
			FuncParameters = funcParameters;
		}

		public StorageName StorageName { get; private set; }
		public FuncParameter[] FuncParameters { get; private set; }
	}
}