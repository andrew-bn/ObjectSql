using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace ObjectSql.QueryInterfaces
{
	public interface IDataReaderHolder : IReturnValueHolder, IDisposable
	{
		IEnumerable<IDictionary<string, object>> MapResultToDictionary();
		IEnumerable<dynamic> MapResultToDynamic();
		IEnumerable<T> MapResult<T>(Func<DbDataReader, T> materializer);
		IEnumerable<TData> MapResult<TData>();
	}
}