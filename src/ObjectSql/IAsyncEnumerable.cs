namespace ObjectSql
{
	public interface IAsyncEnumerable<out T>
	{
		IAsyncEnumerator<T> GetAsyncEnumerator();
		
	}
}
