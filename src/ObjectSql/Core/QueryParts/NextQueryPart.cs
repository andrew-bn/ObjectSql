using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectSql.Core.Bo;

namespace ObjectSql.Core.QueryParts
{
	public class NextQueryPart: QueryPart
	{
		public override void BuildPart(BuilderContext context)
		{
			context.SqlWriter.WriteSqlEnd(context.Text);
		}
	}
}
