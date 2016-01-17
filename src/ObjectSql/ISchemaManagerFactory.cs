using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectSql.Core.SchemaManager;

namespace ObjectSql
{
	public interface ISchemaManagerFactory
	{
		bool MatchSchemaManager(DbConnection connection, string connectionString);
		void SetupConnectionString(DbConnection connection, string connectionString);
		string TryGetProviderName(DbConnection connection, string connectionString);
		IEntitySchemaManager CreateSchemaManager(Type dbType, string connectionString);
	}
}
