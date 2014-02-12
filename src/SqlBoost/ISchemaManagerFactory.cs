using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlBoost.Core.SchemaManager;

namespace SqlBoost
{
	public interface ISchemaManagerFactory
	{
		bool MatchSchemaManager(IDbConnection connection, string connectionString);
		void SetupConnectionString(IDbConnection connection, string connectionString);
		string TryGetProviderName(IDbConnection connection, string connectionString);
		IEntitySchemaManager CreateSchemaManager(Type dbType, string connectionString);
	}
}
