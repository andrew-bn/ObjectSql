using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using ObjectSql.Exceptions;

namespace ObjectSql.Core
{
	public class EntityEnumerable<T> : IEnumerable<T>, IEnumerator<T>
#if NET45
	                                   , IAsyncEnumerable<T>
	                                   , IAsyncEnumerator<T>
#endif
	{
		private readonly IDataReader _dataReader;
		private readonly Action _disposing;
		private readonly Func<IDataReader, T> _materializer;
		private volatile bool _enumeratorReturned;

		public EntityEnumerable(Delegate materializationDelegate, IDataReader dataReader, Action disposing)
		{
			_dataReader = dataReader;

			_disposing = disposing;
			_materializer = (Func<IDataReader, T>)materializationDelegate;
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
			get
			{
				return _materializer(_dataReader);
			}
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
			if (_disposing != null)
				_disposing();
		}

		private EntityEnumerable<T> GetValidEnumerator()
		{
			if (_enumeratorReturned)
				throw new ObjectSqlException("You can use only one instance of enumerator. You have already got one");
			_enumeratorReturned = true;

			return this;
		}
	}
}