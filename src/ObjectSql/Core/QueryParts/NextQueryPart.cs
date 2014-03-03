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
			if (context.Parts.IndexOf(this) > 0)
				context.SqlWriter.WriteSqlEnd(context.Text);
		}
	}
}
