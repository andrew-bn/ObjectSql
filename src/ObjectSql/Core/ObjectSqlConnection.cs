using System.Data;
using ObjectSql.Exceptions;

namespace ObjectSql.Core
{
	internal sealed class ObjectSqlConnection : IObjectSqlConnection
	{
		private readonly IDbConnection _connection;
		private readonly string _connectionString;
		public IDbConnection UnderlyingConnection { get { return _connection; } }
		internal ObjectSqlConnection(string connectionString, IDbConnection connection)
		{
			_connection = connection;
			_connectionString = connectionString;
		}

		public IDbTransaction BeginTransaction(IsolationLevel il)
		{
			return _connection.BeginTransaction(il);
		}

		public IDbTransaction BeginTransaction()
		{
			return _connection.BeginTransaction();
		}

		public void ChangeDatabase(string databaseName)
		{
			_connection.ChangeDatabase(databaseName);
		}

		public void Close()
		{
			_connection.Close();
		}

		public string ConnectionString
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

		public int ConnectionTimeout
		{
			get { return _connection.ConnectionTimeout; }
		}

		public IDbCommand CreateCommand()
		{
			return new ObjectSqlCommand(this, _connection.CreateCommand());
		}

		public string Database
		{
			get { return _connection.Database; }
		}

		public void Open()
		{
			_connection.Open();
		}

		public ConnectionState State
		{
			get { return _connection.State; }
		}

		public void Dispose()
		{
			_connection.Dispose();
		}
	}
}
