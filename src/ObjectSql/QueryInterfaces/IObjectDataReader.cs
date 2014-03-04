using System;
using System.Collections.Generic;
using System.Data;

namespace ObjectSql.QueryInterfaces
{
	public interface IObjectDataReader : IDisposable
	{
		IEnumerable<IDictionary<string, object>> MapResultToDictionary();
		IEnumerable<T> MapResult<T>(Func<IDataReader, T> materializer);
		IEnumerable<TData> MapResult<TData>();
		TReturn MapReturnValue<TReturn>();
	}
}