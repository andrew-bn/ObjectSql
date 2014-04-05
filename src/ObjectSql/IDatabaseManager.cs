using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectSql.Core.QueryBuilder;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.SchemaManager;

namespace ObjectSql
{
	public interface IDatabaseManager
	{
		Type DbType { get; }
		bool MatchManager(IDbConnection dbConnection, string providerName);
		bool MatchManager(IDataReader dataReader);
		IDelegatesBuilder CreateDelegatesBuilder();

		SqlWriter CreateSqlWriter();
	}
}
