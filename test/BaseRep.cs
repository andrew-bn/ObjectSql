using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using ObjectSql.Core.Bo;
using ObjectSql.QueryInterfaces;
using ObjectSql.SqlServer;
using test.NcCore;

namespace test
{
	public enum PremiumDnsSubscriptionStatus
	{
		Active = 0,
		Expired = 1,
		Cancelled = 2,
		WaitingForBind = 3
	}
	public enum DNSProviderSubType
	{
		NONE = 0,
		FREE = 1,
		ENOM = 2,
		CUSTOM = 3,
		FREE2 = 4,
		PREMIUM = 5
	}
	public class SubscriptionsWithoutProviderQueryResult
	{
		public string User { get; set; }
		public string Host { get; set; }
		public DateTime ExpirationDate { get; set; }
		public bool IsTrial { get; set; }
		public long SubscriptionID { get; set; }
		public DateTime CreatedDate { get; set; }
		public bool AutoRenew { get; set; }
		public PremiumDnsSubscriptionStatus Status { get; set; }
		public int TotalRows { get; set; }
		public int OrderId { get; set; }
		public bool IsActive { get; set; }
		public bool IsHosted { get; set; }
	}
	public class SubscriptionsQueryResult : SubscriptionsWithoutProviderQueryResult
	{
		public DNSProviderSubType? ProviderSubType { get; set; }
	}
	public class PremiumDnsRepository : NamecheapCoreRepository
	{
		public SubscriptionsWithoutProviderQueryResult SubscriptionInfoByDomainName(string domainName)
		{
			//ToDo: check if orm code is safe to use under the high load
			//using (SqlConnection connection = new SqlConnection(base._connectionString))
			//{
			//	using (SqlCommand command = new SqlCommand("USP_PremiumDns_Info_ByDomainName", connection))
			//	{
			//		command.CommandType = CommandType.StoredProcedure;
			//		command.Parameters.AddWithValue("domainName", domainName);
			//		connection.Open();
			//		using (SqlDataReader reader = command.ExecuteReader())
			//		{
			//			if (reader.Read())
			//			{
			//				SubscriptionsQueryResult result = new SubscriptionsQueryResult();
			//				result.User = reader["User"].ToString();
			//				result.ExpirationDate = Convert.ToDateTime(reader["ExpirationDate"]);
			//				result.IsTrial = Convert.ToBoolean(reader["isTrial"]);
			//				result.SubscriptionID = Convert.ToInt64(reader["SubscriptionId"]);
			//				result.Status = (PremiumDnsSubscriptionStatus) Convert.ToInt32(reader["Status"]);
			//				result.AutoRenew = Convert.ToBoolean(reader["AutoRenew"]);
			//				result.IsActive = Convert.ToBoolean(reader["IsActive"]);
			//				result.OrderId = Convert.ToInt32(reader["OrderId"]);

			//				return result;
			//			}
			//		}
			//	}
			//}
			//return null;

			using (var reader = Sp(db => db.USP_PremiumDns_Info_ByDomainName(domainName)).ExecuteReader())
			{
				return reader.MapResult<SubscriptionsQueryResult>().FirstOrDefault(x => x.ExpirationDate > DateTime.Now);
			}
		}

	}

	public class NamecheapCoreRepository : SingleDatabaseRepository<NcCoreProcedures>
	{
		public static string ConnectionString
		{
			get
			{
				return ConfigurationManager.ConnectionStrings["NcCore"].ConnectionString;
			}
		}


		public NamecheapCoreRepository()
			: base(ConnectionString)
		{

		}
	}
	public class SingleDatabaseRepository<TSpHolder> : BaseRepository
	{
		protected string _connectionString;

		public SingleDatabaseRepository(string connectionString)
		{
			_connectionString = connectionString;
		}

		protected IQueryEnd Sp(string connectionString, Expression<Action<TSpHolder>> spExecutor)
		{
			return Sp<TSpHolder>(connectionString, spExecutor);
		}

		public IQueryEnd Sp(Expression<Action<TSpHolder>> spExecutor)
		{
			return Sp<TSpHolder>(_connectionString, spExecutor);
		}

		public IQuery Query()
		{
			return Query(_connectionString);
		}

		public ITransactionHolder CreateTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
		{
			return CreateTransaction(_connectionString, isolationLevel);
		}
	}
	public interface ITransactionHolder : IDisposable
	{
		void Commit();
		void Rollback();
	}

	public class BaseRepository
	{
		static BaseRepository()
		{
			ObjectSqlSqlServerInitializer.Initialize();
		}

		private static readonly ConcurrentDictionary<string, TransactionHolder> _transactionPerThread =
			new ConcurrentDictionary<string, TransactionHolder>();

		private class TransactionHolder : ITransactionHolder
		{
			private readonly string _key;
			private readonly SqlTransaction _transaction;
			private readonly SqlConnection _connection;

			public TransactionHolder(string key, SqlTransaction transaction)
			{
				_key = key;
				_transaction = transaction;
				_connection = _transaction.Connection;
			}

			public SqlCommand CreateCommand()
			{
				var cmd = _transaction.Connection.CreateCommand();
				cmd.Transaction = _transaction;
				return cmd;
			}

			public void Dispose()
			{
				_transaction.Dispose();
				_connection.Dispose();
				TransactionHolder @out;
				_transactionPerThread.TryRemove(_key, out @out);
			}

			public void Commit()
			{
				_transaction.Commit();
			}

			public void Rollback()
			{
				_transaction.Rollback();
			}
		}

		protected BaseRepository()
		{

		}

		protected ITransactionHolder CreateTransaction(string connectionString,
			IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
		{
			var transactionKey = GetTransactionKey(connectionString);
			return _transactionPerThread.GetOrAdd(transactionKey,
				id => CreateNewTransaction(connectionString, id, isolationLevel));
		}

		protected IQueryEnd Sp<TSpHolder>(string connectionString, Expression<Action<TSpHolder>> spExecutor)
		{
			return Query(connectionString).Exec(spExecutor);
		}

		protected IQuery Query(string connectionString)
		{
			TransactionHolder holder;
			var transactionKey = GetTransactionKey(connectionString);

			if (_transactionPerThread.TryGetValue(transactionKey, out holder))
				return holder.CreateCommand().ObjectSql(ResourcesTreatmentType.DisposeCommand);
			return CreateRawCommand(connectionString).ObjectSql(ResourcesTreatmentType.DisposeConnection);
		}

		private static SqlCommand CreateRawCommand(string connectionString)
		{
			var connection = new SqlConnection(connectionString);
			return connection.CreateCommand();
		}

		private static TransactionHolder CreateNewTransaction(string connectionString, string key,
			IsolationLevel isolationLevel)
		{
			var connection = new SqlConnection(connectionString);
			connection.Open();
			var transaction = connection.BeginTransaction(isolationLevel);
			var holder = new TransactionHolder(key, transaction);
			return holder;
		}

		private static string GetTransactionKey(string connectionString)
		{
			return string.Format("{0}.Thread{1}", connectionString, Thread.CurrentThread.ManagedThreadId);
		}
	}
}