using ObjectSql.Core.QueryBuilder;
using ObjectSql.Core.QueryBuilder.ExpressionsAnalizers;
using ObjectSql.Core.QueryBuilder.InfoExtractor;
using ObjectSql.Core.SchemaManager;

namespace ObjectSql.SqlServer
{
	public class SqlServerQueryBuilder: QueryBuilder
	{
		private readonly static SqlServerSqlWriter _sqlWriter = new SqlServerSqlWriter();
		private static readonly SqlServerDelegatesBuilder _delegatesBuilder = new SqlServerDelegatesBuilder();
		public SqlServerQueryBuilder(IEntitySchemaManager schemaManager)
			: base(schemaManager,
					_sqlWriter,
					_delegatesBuilder,
					new ExpressionAnalizer(schemaManager, _delegatesBuilder, _sqlWriter),
					new MaterializationInfoExtractor(schemaManager),
					new InsertionInfoExtractor(schemaManager))
		{
		}
	}

}
