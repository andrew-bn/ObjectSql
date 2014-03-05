using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectSql.Core.Bo;
using ObjectSql.Exceptions;
using ObjectSql.QueryInterfaces;

namespace ObjectSql.Core
{
	public class ScalarResultHolder: IScalarResultHolder
	{
		private readonly QueryContext _context;
		private readonly object _result;

		public ScalarResultHolder(QueryContext context, object result)
		{
			_context = context;
			_result = result;
		}

		public TReturn MapReturnValue<TReturn>()
		{
			if (_context.PreparationData.ReturnParameterReader==null)
				throw new ObjectSqlException("Return mapping is not set. Use IQueryEnd.Returns<TResult>(object sqlDbType) to set mapping");

			return (TReturn) _context.PreparationData.ReturnParameterReader(_context.Command);
		}

		public object ScalarResult { get { return _result; } }
	}
}
