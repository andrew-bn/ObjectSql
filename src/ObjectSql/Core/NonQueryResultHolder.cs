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
	public class NonQueryResultHolder: INonQueryResultHolder
	{
		private readonly QueryContext _context;
		private readonly int _result;

		public NonQueryResultHolder(QueryContext context, int result)
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

		public int NonQueryResult { get { return _result; } }
	}
}
