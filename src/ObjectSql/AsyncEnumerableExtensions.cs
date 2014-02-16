using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ObjectSql
{
#if NET45
	public static class AsyncEnumerableExtensions
	{
		public static Task ForEachEntityAsync<T>(this IAsyncEnumerable<T> enumerable, Action<T> action)
		{
			return ForEachEntityAsync(enumerable, action, CancellationToken.None);
		}
		public static Task TakeEntitiesWhileAsync<T>(this IAsyncEnumerable<T> enumerable, Func<T, bool> predicate)
		{
			var cancellationTokenSource = new CancellationTokenSource();

			return ForEachEntityAsync(enumerable,
									  e => { if (!predicate(e)) cancellationTokenSource.Cancel(); },
									  cancellationTokenSource.Token);
		}

		private async static Task ForEachEntityAsync<T>(this IAsyncEnumerable<T> enumerable, Action<T> action, CancellationToken cancellationToken)
		{
			using (var enumerator = enumerable.GetAsyncEnumerator())
			{
				if (!cancellationToken.IsCancellationRequested)
				{
					while (await enumerator.MoveNextAsync())
					{
						action(enumerator.Current);
						if (cancellationToken.IsCancellationRequested)
							break;
					}
				}
			}
		}
		public async static Task<IEnumerable<T>> EntitiesAsEnumerableAsync<T>(this IAsyncEnumerable<T> enumerable)
		{
			var result = new List<T>();
			await ForEachEntityAsync(enumerable, result.Add);
			return result;
		}
		public async static Task<T[]> EntitiesToArrayAsync<T>(this IAsyncEnumerable<T> enumerable)
		{
			var result = new List<T>();
			await ForEachEntityAsync(enumerable, result.Add);
			return result.ToArray();
		}
		public async static Task<List<T>> EntitiesToListAsync<T>(this IAsyncEnumerable<T> enumerable)
		{
			var result = new List<T>();
			await ForEachEntityAsync(enumerable, result.Add);
			return result;
		}
		public async static Task<T> FirstOrDefaultEntityAsync<T>(this IAsyncEnumerable<T> enumerable)
		{
			var result = default(T);
			await TakeEntitiesWhileAsync(enumerable, e =>
			{
				result = e;
				return false;
			});
			return result;
		}
	}
#endif
}
