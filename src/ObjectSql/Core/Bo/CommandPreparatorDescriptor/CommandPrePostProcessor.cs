using System;
using System.Data;

namespace ObjectSql.Core.Bo.CommandPreparatorDescriptor
{
	public class CommandPrePostProcessor
	{
		public CommandPrePostProcessor(Action<IDbCommand, QueryRoots> preparationAction)
		{
			CommandPreparationAction = preparationAction;
		}

		public Action<IDbCommand, QueryRoots> CommandPreparationAction { get; private set; }
	}
}
