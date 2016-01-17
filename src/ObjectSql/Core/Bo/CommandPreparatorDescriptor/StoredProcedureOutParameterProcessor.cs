using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql.Core.Bo.CommandPreparatorDescriptor
{
	public class StoredProcedureOutParameterProcessor : CommandPrePostProcessor
	{
		public StoredProcedureOutParameterProcessor(Action<DbCommand, QueryRoots> preparationAction) 
			: base(preparationAction)
		{
		}
	}
}
