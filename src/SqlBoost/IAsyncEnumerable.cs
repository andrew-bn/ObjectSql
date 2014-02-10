namespace SqlBoost
{
	public interface IAsyncEnumerable<out T>
	{
		IAsyncEnumerator<T> GetAsyncEnumerator();
		
	}
}
