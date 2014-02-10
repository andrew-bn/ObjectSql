using System;
using System.Runtime.Serialization;

namespace SqlBoost.Exceptions
{
	[Serializable]
	public class SqlBoostException : Exception
	{

		public SqlBoostException()
		{

		}

		public SqlBoostException(String msg)
			: base(msg)
		{

		}
		
		protected SqlBoostException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}

		public SqlBoostException(String msg, Exception innerException)
			: base(msg, innerException)
		{
		}

	}
}
