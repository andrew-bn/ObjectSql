using ObjectSql.Core.Bo;
using ObjectSql.QueryInterfaces;

namespace ObjectSql.QueryImplementation
{
	public class DatabaseContextHolder<T> : Query, IDatabaseContextHolder<T>
	{
		public DatabaseContextHolder(QueryContext context):base(context)
		{
		}
	}
}