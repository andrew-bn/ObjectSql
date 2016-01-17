using System;
using System.Data;
using System.Data.Common;

namespace ObjectSql.Core.Bo.CommandPreparatorDescriptor
{
	public class CommandPrePostProcessor
	{
		public CommandPrePostProcessor(Action<DbCommand, QueryRoots> preparationAction)
		{
			CommandPreparationAction = preparationAction;
		}

		public Action<DbCommand, QueryRoots> CommandPreparationAction { get; private set; }
	}
}
