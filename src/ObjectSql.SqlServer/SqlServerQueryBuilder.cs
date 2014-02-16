using ObjectSql.Core.QueryBuilder;
using ObjectSql.Core.QueryBuilder.ExpressionsAnalizers;
using ObjectSql.Core.QueryBuilder.InfoExtractor;
using ObjectSql.Core.SchemaManager;

namespace ObjectSql.SqlServer
{
	public class SqlServerQueryBuilder: QueryBuilder
	{
		private readonly static SqlServerSqlWriter _sqlWriter = new SqlServerSqlWriter();
		
		public SqlServerQueryBuilder(IEntitySchemaManager schemaManager)
			: base(schemaManager,
					_sqlWriter,
					SqlServerDelegatesBuilder.Instance,
					new ExpressionAnalizer(schemaManager, SqlServerDelegatesBuilder.Instance, _sqlWriter),
					new MaterializationInfoExtractor(schemaManager),
					new InsertionInfoExtractor(schemaManager))
		{
		}
	}

}
