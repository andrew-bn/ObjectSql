using System;
using System.Data;
using System.Data.Common;

namespace ObjectSql.Core
{
	internal sealed class ObjectSqlCommand : DbCommand, IObjectSqlCommand
	{
		private readonly ObjectSqlConnection _connection;
		public DbCommand UnderlyingCommand { get; }

		internal ObjectSqlCommand(ObjectSqlConnection connection, DbCommand command)
		{
			UnderlyingCommand = command;
			_connection = connection;
		}

		public override void Cancel() => UnderlyingCommand.Cancel();

		public override string CommandText
		{
			get
			{
				return UnderlyingCommand.CommandText;
			}
			set
			{
				UnderlyingCommand.CommandText = value;
			}
		}

		public override int CommandTimeout
		{
			get
			{
				return UnderlyingCommand.CommandTimeout;
			}
			set
			{
				UnderlyingCommand.CommandTimeout = value;
			}
		}

		public override CommandType CommandType
		{
			get
			{
				return UnderlyingCommand.CommandType;
			}
			set
			{
				UnderlyingCommand.CommandType = value;
			}
		}

		protected override DbConnection DbConnection
		{
			get
			{
				return _connection;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		protected override DbParameter CreateDbParameter()
		{
			return UnderlyingCommand.CreateParameter();
		}

		public override int ExecuteNonQuery()
		{
			return UnderlyingCommand.ExecuteNonQuery();
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			return UnderlyingCommand.ExecuteReader(behavior);
		}

		public override object ExecuteScalar()
		{
			return UnderlyingCommand.ExecuteScalar();
		}

		protected override DbParameterCollection DbParameterCollection => UnderlyingCommand.Parameters;

		public override void Prepare()
		{
			UnderlyingCommand.Prepare();
		}

		protected override DbTransaction DbTransaction
		{
			get
			{
				return UnderlyingCommand.Transaction;
			}
			set
			{
				UnderlyingCommand.Transaction = value;
			}
		}

		public override bool DesignTimeVisible
		{
			get { return UnderlyingCommand.DesignTimeVisible; }
			set { UnderlyingCommand.DesignTimeVisible = value; }
		}

	public override UpdateRowSource UpdatedRowSource
	{
		get
		{
			return UnderlyingCommand.UpdatedRowSource;
		}
		set
		{
			UnderlyingCommand.UpdatedRowSource = value;
		}
	}
	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		return obj is ObjectSqlCommand && Equals((ObjectSqlCommand)obj);
	}

	private bool Equals(ObjectSqlCommand other)
	{
		return Equals(UnderlyingCommand, other.UnderlyingCommand);
	}

	public override int GetHashCode()
	{
		return (UnderlyingCommand != null ? UnderlyingCommand.GetHashCode() : 0);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
			UnderlyingCommand.Dispose();
	}
}
}
