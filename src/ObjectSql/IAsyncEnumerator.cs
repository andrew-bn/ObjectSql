using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql
{
	public interface IAsyncEnumerator<out T> : IDisposable
	{
		Task<bool> MoveNextAsync();
		T Current { get; }
	}
}
