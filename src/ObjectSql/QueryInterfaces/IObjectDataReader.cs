using System;
using System.Collections.Generic;

namespace ObjectSql.QueryInterfaces
{
	public interface IObjectDataReader : IDisposable
	{
		IEnumerable<TData> MapResult<TData>();
		TReturn MapReturnValue<TReturn>();
	}
}