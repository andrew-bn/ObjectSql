using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectSql.Core.Bo;

namespace ObjectSql.QueryInterfaces
{
	public interface IQueryContextHolder
	{
		QueryContext Context { get; }
	}
}
