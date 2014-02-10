using System.Data.EntityClient;
using System.Text.RegularExpressions;
using SqlBoost.Core.Bo;

namespace SqlBoost.Core.Misc
{
	internal static class ConnectionStringAnalizer
	{
		private static readonly Regex _isEfConnectionString = new Regex("metadata=[^;]+;provider=[^;]+;provider connection string=\"[^\"]+\"");
		public static bool TryGetEfConnectionString(string connectionString, out EntityConnectionStringBuilder stringBuilder)
		{
			stringBuilder = null;

			if (_isEfConnectionString.IsMatch(connectionString))
				stringBuilder = new EntityConnectionStringBuilder(connectionString);

			return stringBuilder != null;
		}
		public static bool TryDiscoverStorageType(string connectionString, out StorageType storageType)
		{
			storageType = StorageType.SqlServer;
			return true;
		}
	}
}
