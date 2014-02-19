using System;
using System.Data;
using System.Linq.Expressions;
using ObjectSql.Core.Bo.EntitySchema;

namespace ObjectSql.Core.Bo.CommandPreparatorDescriptor
{
	public class DatabaseCommandParameterPrePostProcessor : SingleParameterPrePostProcessor
	{
		public DatabaseCommandParameterPrePostProcessor(string parameterName,
			IStorageFieldType parameterType, Expression valueAccessor, Action<IDbCommand, object> preparationAction) 
			: base(parameterName, parameterType,valueAccessor, preparationAction)
		{
		}

		public void ParameterWasEncountered(int place)
		{
			RootMap = RootMap ^ (1 << place);
		}
		

		public override CommandPreparatorType PreparatorType
		{
			get { return CommandPreparatorType.DatabaseCommandParameter; }
		}

		public override bool RootDemanding
		{
			get { return true; }
		}
	}
}
