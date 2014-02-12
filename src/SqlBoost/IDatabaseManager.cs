using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlBoost.Core.QueryBuilder;
using SqlBoost.Core.SchemaManager;

namespace SqlBoost
{
	public interface IDatabaseManager
	{
		Type DbType { get; }
		bool MatchManager(IDbConnection dbConnection, string providerName);
		IQueryBuilder CreateQueryBuilder(IEntitySchemaManager schemaManager);
	}
}
