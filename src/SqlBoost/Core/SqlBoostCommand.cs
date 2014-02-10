using System;
using System.Data;

namespace SqlBoost.Core
{
	internal sealed class SqlBoostCommand : ISqlBoostCommand
	{
		private readonly IDbCommand _command;
		private readonly SqlBoostConnection _connection;
		public IDbCommand UnderlyingCommand { get { return _command; } }
		internal SqlBoostCommand(SqlBoostConnection connection, IDbCommand command)
		{
			_command = command;
			_connection = connection;
		}

		public void Cancel()
		{
			_command.Cancel();
		}

		public string CommandText
		{
			get
			{
				return _command.CommandText;
			}
			set
			{
				_command.CommandText = value;
			}
		}

		public int CommandTimeout
		{
			get
			{
				return _command.CommandTimeout;
			}
			set
			{
				_command.CommandTimeout = value;
			}
		}

		public CommandType CommandType
		{
			get
			{
				return _command.CommandType;
			}
			set
			{
				_command.CommandType = value;
			}
		}

		public IDbConnection Connection
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

		public IDbDataParameter CreateParameter()
		{
			return _command.CreateParameter();
		}

		public int ExecuteNonQuery()
		{
			return _command.ExecuteNonQuery();
		}

		public IDataReader ExecuteReader(CommandBehavior behavior)
		{
			return _command.ExecuteReader(behavior);
		}

		public IDataReader ExecuteReader()
		{
			return _command.ExecuteReader();
		}

		public object ExecuteScalar()
		{
			return _command.ExecuteScalar();
		}

		public IDataParameterCollection Parameters
		{
			get { return _command.Parameters; }
		}

		public void Prepare()
		{
			_command.Prepare();
		}

		public IDbTransaction Transaction
		{
			get
			{
				return _command.Transaction;
			}
			set
			{
				_command.Transaction = value;
			}
		}

		public UpdateRowSource UpdatedRowSource
		{
			get
			{
				return _command.UpdatedRowSource;
			}
			set
			{
				_command.UpdatedRowSource = value;
			}
		}
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj is SqlBoostCommand && Equals((SqlBoostCommand) obj);
		}

		private bool Equals(SqlBoostCommand other)
		{
			return Equals(_command, other._command);
		}

		public override int GetHashCode()
		{
			return (_command != null ? _command.GetHashCode() : 0);
		}
		public void Dispose()
		{
			_command.Dispose();
		}
	}
}
