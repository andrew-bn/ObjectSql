using System;

namespace ObjectSql.Core
{
	public class DeclaringTypeAttribute:Attribute
	{
		public Type Type { get; private set; }

		public DeclaringTypeAttribute(Type type)
		{
			Type = type;
		}
	}
}
