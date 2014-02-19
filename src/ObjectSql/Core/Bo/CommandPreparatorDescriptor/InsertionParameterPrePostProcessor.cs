using System;
using System.Data;

namespace ObjectSql.Core.Bo.CommandPreparatorDescriptor
{
	public class InsertionParameterPrePostProcessor : CommandPrePostProcessor
	{
		public InsertionParameterPrePostProcessor(Action<IDbCommand, object> preparationAction) 
			: base(preparationAction)
		{
			RootMap = 1;
		}

		public override CommandPreparatorType PreparatorType
		{
			get
			{
				return CommandPreparatorType.InsertionParameter;
			}
		}

		public override bool RootDemanding
		{
			get { return true; }
		}
	}
}
