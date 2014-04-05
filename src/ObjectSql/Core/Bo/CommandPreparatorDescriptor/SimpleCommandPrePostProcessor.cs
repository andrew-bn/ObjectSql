using System;
using System.Data;

namespace ObjectSql.Core.Bo.CommandPreparatorDescriptor
{
	public class SimpleCommandPrePostProcessor: CommandPrePostProcessor
	{
		public SimpleCommandPrePostProcessor(Action<IDbCommand, object> preparationAction)
			: base(preparationAction)
		{
		}
	}
}
