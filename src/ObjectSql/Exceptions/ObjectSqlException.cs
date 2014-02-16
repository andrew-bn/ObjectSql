using System;
using System.Runtime.Serialization;

namespace ObjectSql.Exceptions
{
	[Serializable]
	public class ObjectSqlException : Exception
	{

		public ObjectSqlException()
		{

		}

		public ObjectSqlException(String msg)
			: base(msg)
		{

		}
		
		protected ObjectSqlException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}

		public ObjectSqlException(String msg, Exception innerException)
			: base(msg, innerException)
		{
		}

	}
}
