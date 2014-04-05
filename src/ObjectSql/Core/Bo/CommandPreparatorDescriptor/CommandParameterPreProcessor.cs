using System;
using System.Data;
using System.Linq.Expressions;
using ObjectSql.Core.Bo.EntitySchema;

namespace ObjectSql.Core.Bo.CommandPreparatorDescriptor
{
	public class CommandParameterPreProcessor : CommandPrePostProcessor
	{
		public CommandParameterPreProcessor(string name, 
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
