using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectSql.Core.QueryBuilder;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.SchemaManager;

namespace ObjectSql.Core.Bo
{
	public class QueryEnvironment
	{
		public IEntitySchemaManager SchemaManager { get; private set; }
		public IDatabaseManager DatabaseManager { get; private set; }
		public IDelegatesBuilder DelegatesBuilder { get; private set; }
		public SqlWriter SqlWriter { get; private set; }

		public QueryEnvironment(IEntitySchemaManager schemaManager,
								IDatabaseManager databaseManager,
								IDelegatesBuilder delegatesBuilder,
								SqlWriter sqlWriter)
		{
			SchemaManager = schemaManager;
			DatabaseManager = databaseManager;
			DelegatesBuilder = delegatesBuilder;
			SqlWriter = sqlWriter;
		}
	}
}
