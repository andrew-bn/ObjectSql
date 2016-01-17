using System.Data;
using System.Data.Common;
using ObjectSql.Exceptions;

namespace ObjectSql.Core
{
	internal sealed class ObjectSqlConnection : DbConnection, IObjectSqlConnection
	{
		private readonly string _connectionString;
		public DbConnection UnderlyingConnection { get; }

		internal ObjectSqlConnection(string connectionString, DbConnection connection)
		{
			UnderlyingConnection = connection;
			_connectionString = connectionString;
		}

		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			return UnderlyingConnection.BeginTransaction(isolationLevel);
		}


		public override void ChangeDatabase(string databaseName)
		{
			UnderlyingConnection.ChangeDatabase(databaseName);
		}

		public override void Close()
		{
			UnderlyingConnection.Close();
		}

		public override string ConnectionString
		{
			get
			{
				return _connectionString;
			}
			set
			{
				throw new ObjectSqlException("Invalid operation");
			}
		}
		protected override DbCommand CreateDbCommand()
		{
			return new ObjectSqlCommand(this, UnderlyingConnection.CreateCommand());
		}

		public override int ConnectionTimeout => UnderlyingConnection.ConnectionTimeout;
		public override string Database => UnderlyingConnection.Database;
		public override string DataSource => UnderlyingConnection.DataSource;
		public override string ServerVersion => UnderlyingConnection.ServerVersion;
		public override void Open() => UnderlyingConnection.Open();
		public override ConnectionState State => UnderlyingConnection.State;

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				UnderlyingConnection.Dispose();
		}
	}
}
