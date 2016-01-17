using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
		bool MatchManager(DbConnection dbConnection, string providerName);
		bool MatchManager(DbDataReader dataReader);
		IDelegatesBuilder CreateDelegatesBuilder();
		string MapToDbType(Type netType);
		SqlWriter CreateSqlWriter();
	}
}
