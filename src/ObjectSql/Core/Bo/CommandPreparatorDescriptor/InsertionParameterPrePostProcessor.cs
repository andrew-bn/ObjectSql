using System;
using System.Data;
using System.Data.Common;

namespace ObjectSql.Core.Bo.CommandPreparatorDescriptor
{
	public class InsertionParameterPrePostProcessor : CommandPrePostProcessor
	{
		public InsertionParameterPrePostProcessor(Action<DbCommand, QueryRoots> preparationAction) 
			: base(preparationAction)
		{
		}
	}
}
