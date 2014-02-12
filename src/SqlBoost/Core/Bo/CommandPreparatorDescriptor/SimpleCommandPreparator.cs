using System;
using System.Data;

namespace SqlBoost.Core.Bo.CommandPreparatorDescriptor
{
	public class SimpleCommandPreparator: CommandPreparator
	{
		public SimpleCommandPreparator(Action<IDbCommand, object> preparationAction)
			: base(preparationAction)
		{
		}

		public override CommandPreparatorType PreparatorType
		{
			get { return CommandPreparatorType.CommandPreparator; }
		}

		public override bool RootDemanding
		{
			get { return false; }
		}
	}
}
