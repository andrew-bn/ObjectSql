using System;
using System.Data;

namespace ObjectSql.Core.Bo.CommandPreparatorDescriptor
{
	public class CommandPrePostProcessor
	{
		public CommandPrePostProcessor(Action<IDbCommand, object> preparationAction)
		{
			CommandPreparationAction = preparationAction;
			RootIndex = -1;
		}

		public Action<IDbCommand, object> CommandPreparationAction { get; private set; }

		public bool RootDemanding { get { return RootIndex > -1; } }
		public int RootIndex { get; set; }
	}
}
