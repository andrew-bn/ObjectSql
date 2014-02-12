using SqlBoost.Core.QueryBuilder;
using SqlBoost.Core.QueryBuilder.ExpressionsAnalizers;
using SqlBoost.Core.SchemaManager;
using SqlBoost.Core.QueryBuilder.InfoExtractor;

namespace SqlBoost.SqlServer
{
	internal class SqlServerQueryBuilder: QueryBuilder
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
