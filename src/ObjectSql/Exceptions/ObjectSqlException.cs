using System;
#if !COREFX
using System.Runtime.Serialization;
#endif
namespace ObjectSql.Exceptions
{
#if !COREFX
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
#if !COREFX
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
