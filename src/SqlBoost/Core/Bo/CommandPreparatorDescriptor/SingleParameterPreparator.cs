using System;
using System.Data;
using System.Linq.Expressions;
using SqlBoost.Core.Bo.EntitySchema;

namespace SqlBoost.Core.Bo.CommandPreparatorDescriptor
{
	public abstract class SingleParameterPreparator : CommandPreparator
	{
		protected SingleParameterPreparator(string name, 
			IStorageFieldType type, Expression accessor, Action<IDbCommand, object> preparationAction) 
			: base(preparationAction)
		{
			Name = name;
			DbType = type;
			ValueAccessorExp = accessor;
		}
		public string Name { get; private set; }
		public Expression ValueAccessorExp { get; private set; }
		public IStorageFieldType DbType { get; private set; }
		
	}
}
