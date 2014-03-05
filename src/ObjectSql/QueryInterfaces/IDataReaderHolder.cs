using System;
using System.Collections.Generic;
using System.Data;

namespace ObjectSql.QueryInterfaces
{
	public interface IDataReaderHolder : IReturnValueHolder, IDisposable
	{
		IEnumerable<IDictionary<string, object>> MapResultToDictionary();
		IEnumerable<T> MapResult<T>(Func<IDataReader, T> materializer);
		IEnumerable<TData> MapResult<TData>();
	}
}