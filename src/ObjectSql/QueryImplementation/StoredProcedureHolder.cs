using ObjectSql.QueryInterfaces;

namespace ObjectSql.QueryImplementation
{
	public class StoredProcedureHolder<T>:IStoredProcedureHolder<T>
	{
		public ISql Root { get; private set; }

		public StoredProcedureHolder(QueryRoot root)
		{
			Root = root;
		}
	}
}