using System;
using System.Data;

namespace ObjectSql.Core.Bo.CommandPreparatorDescriptor
{
	public abstract class CommandPrePostProcessor
	{
		protected CommandPrePostProcessor(Action<IDbCommand, object> preparationAction)
		{
			CommandPreparationAction = preparationAction;
		}

		public Action<IDbCommand, object> CommandPreparationAction { get; private set; }

		public abstract CommandPreparatorType PreparatorType { get; }
		public abstract bool RootDemanding { get; }
		public int RootMap { get; protected set; }
	}
}
