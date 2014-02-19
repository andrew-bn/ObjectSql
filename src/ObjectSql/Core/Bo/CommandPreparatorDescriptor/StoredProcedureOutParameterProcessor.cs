using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql.Core.Bo.CommandPreparatorDescriptor
{
	public class StoredProcedureOutParameterProcessor : CommandPrePostProcessor
	{
		public StoredProcedureOutParameterProcessor(Action<IDbCommand, object> preparationAction) 
			: base(preparationAction)
		{
		}
		public void ParameterWasEncountered(int place)
		{
			RootMap = RootMap ^ (1 << place);
		}
		public override CommandPreparatorType PreparatorType
		{
			get { return CommandPreparatorType.StoredProcedureOutParameterReader; }
		}

		public override bool RootDemanding
		{
			get { return true; }
		}
	}
}
