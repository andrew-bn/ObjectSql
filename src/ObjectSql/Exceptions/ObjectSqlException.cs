using System;
#if NET40 || NET45 || NET451
using System.Runtime.Serialization;
#endif
namespace ObjectSql.Exceptions
{
#if NET40 || NET45 || NET451
	[Serializable]
#endif
	public class ObjectSqlException : Exception
	{

		public ObjectSqlException()
		{

		}

		public ObjectSqlException(String msg)
			: base(msg)
		{

		}
#if NET40 || NET45 || NET451
		protected ObjectSqlException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
#endif
		public ObjectSqlException(String msg, Exception innerException)
			: base(msg, innerException)
		{
		}

	}
}
