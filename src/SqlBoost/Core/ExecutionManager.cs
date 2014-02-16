using System.Collections;
using System.Data.Common;
using SqlBoost.Core.Bo;
using SqlBoost.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SqlBoost.Core
{
	internal static class ExecutionManager
	{
		private static void PrepareQuery(QueryContext context)
		{
			QueryManager.PrepareQuery(context);
		}
		public static object ExecuteScalar(QueryContext context)
		{
			PrepareQuery(context);
			return ExecuteCommand(context, cmd => cmd.ExecuteScalar());
		}
		public static int ExecuteNonQuery(QueryContext context)
		{
			PrepareQuery(context);
			return ExecuteCommand(context, cmd => cmd.ExecuteNonQuery());
		}
		public static IEnumerable<T> ExecuteQuery<T>(QueryContext context)
		{
			PrepareQuery(context);
			var cmd = context.DbCommand;

			var connection = cmd.Connection;
			var connectionOpened = connection.State == ConnectionState.Closed;
			if (connectionOpened) connection.Open();

			var dataReader = cmd.ExecuteReader();
			return new EntityEnumerable<T>(context, dataReader, connectionOpened);
		}

		private static T ExecuteCommand<T>(QueryContext context, Func<IDbCommand, T> executor)
		{
			var cmd = context.DbCommand;
			var connectionOpened = OpenConnection(cmd.Connection);
			try
			{
				return executor(cmd);
			}
			finally
			{
				FreeResources(context, cmd, connectionOpened);
			}
		}

		private static void FreeResources(QueryContext context, IDbCommand cmd, bool connectionOpened)
		{
			if (connectionOpened)
				cmd.Connection.Close();
			if (context.ResourcesTreatmentType == ResourcesTreatmentType.DisposeCommand ||
				context.ResourcesTreatmentType == ResourcesTreatmentType.DisposeConnection)
				cmd.Dispose();
			if (context.ResourcesTreatmentType == ResourcesTreatmentType.DisposeConnection)
				cmd.Connection.Dispose();
		}

		private static bool OpenConnection(IDbConnection dbConnection)
		{
			bool openConnection = dbConnection.State == ConnectionState.Closed;
			if (openConnection)
				dbConnection.Open();
			return openConnection;
		}
#if NET45
		#region async
		public static async Task<IAsyncEnumerable<T>> ExecuteQueryAsync<T>(QueryContext context)
		{
			if (!(context.DbCommand is DbCommand))
				throw new SqlBoostException("Provider does not support async operations");

			PrepareQuery(context);

			var cmd = (DbCommand)context.DbCommand;
			var connection = cmd.Connection;
			var connectionOpened = connection.State == ConnectionState.Closed;
			if (connectionOpened) await connection.OpenAsync();

			var dataReader = await cmd.ExecuteReaderAsync();

			return new EntityEnumerable<T>(context, dataReader, connectionOpened);
		}

		
		#endregion async
#endif
		public class EntityEnumerable<T> : IEnumerable<T>, IEnumerator<T>
#if NET45
			, IAsyncEnumerable<T>
			, IAsyncEnumerator<T>
#endif
		{
			private readonly QueryContext _context;
			private readonly IDataReader _dataReader;
			private readonly bool _connectionOpened;
			private readonly Func<IDataReader, T> _materializer;
			private volatile bool _enumeratorReturned;

			public EntityEnumerable(QueryContext context, IDataReader dataReader, bool connectionOpened)
			{
				_context = context;
				_dataReader = dataReader;
				_connectionOpened = connectionOpened;
				_materializer = (Func<IDataReader, T>)_context.MaterializationDelegate;
			}

			public IEnumerator<T> GetEnumerator()
			{
				return GetValidEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetValidEnumerator();
			}
#if NET45
			public IAsyncEnumerator<T> GetAsyncEnumerator()
			{
				return GetValidEnumerator();
			}
#endif
			public T Current
			{
				get { return _materializer(_dataReader); }
			}

			object IEnumerator.Current
			{
				get { return Current; }
			}

			public bool MoveNext()
			{
				return _dataReader.Read();
			}
#if NET45
			public Task<bool> MoveNextAsync()
			{
				return ((DbDataReader)_dataReader).ReadAsync();
			}
#endif

			public void Reset()
			{

			}

			public void Dispose()
			{
				_dataReader.Dispose();
				FreeResources(_context, _context.DbCommand, _connectionOpened);
			}

			private EntityEnumerable<T> GetValidEnumerator()
			{
				if (_enumeratorReturned)
					throw new SqlBoostException("You can use only one instance of enumerator. You have already got one");
				_enumeratorReturned = true;

				return this;
			}
		}
	}
}
