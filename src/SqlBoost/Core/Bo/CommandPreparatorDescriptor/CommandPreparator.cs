using System;
using System.Data;

namespace SqlBoost.Core.Bo.CommandPreparatorDescriptor
{
	internal abstract class CommandPreparator
	{
		protected CommandPreparator(Action<IDbCommand, object> preparationAction)
		{
			CommandPreparationAction = preparationAction;
		}

		public Action<IDbCommand, object> CommandPreparationAction { get; private set; }

		public abstract CommandPreparatorType PreparatorType { get; }
		public abstract bool RootDemanding { get; }
		public int RootMap { get; protected set; }
	}
}
