using System;
using System.Data;
using System.Linq.Expressions;
using ObjectSql.Core.Bo.EntitySchema;

namespace ObjectSql.Core.Bo.CommandPreparatorDescriptor
{
	public class DatabaseCommandConstantPreparator: SingleParameterPreparator
	{
		public DatabaseCommandConstantPreparator(string parameterName,
			IStorageFieldType parameterType, Expression valueAccessor, Action<IDbCommand, object> preparationAction) 
			: base(parameterName, parameterType,valueAccessor, preparationAction)
		{
		}

		public override CommandPreparatorType PreparatorType
		{
			get { return CommandPreparatorType.DatabaseCommandConstant; }
		}

		public override bool RootDemanding
		{
			get { return false; }
		}
	}
}
