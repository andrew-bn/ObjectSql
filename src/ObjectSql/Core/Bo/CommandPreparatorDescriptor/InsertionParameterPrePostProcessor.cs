using System;
using System.Data;

namespace ObjectSql.Core.Bo.CommandPreparatorDescriptor
{
	public class InsertionParameterPrePostProcessor : CommandPrePostProcessor
	{
		public InsertionParameterPrePostProcessor(Action<IDbCommand, object> preparationAction) 
			: base(preparationAction)
		{
			RootIndex = 0;
		}
	}
}
