using ObjectSql.Core.Bo;
using ObjectSql.QueryInterfaces;

namespace ObjectSql.QueryImplementation
{
	public class DatabaseContextHolder<T> : QueryRoot, IDatabaseContextHolder<T>
	{
		public DatabaseContextHolder(QueryContext context):base(context)
		{
		}
	}
}