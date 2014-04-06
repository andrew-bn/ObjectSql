using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ObjectSql.Core.Bo;
using ObjectSql.Core.QueryBuilder.ExpressionsAnalizers;

namespace ObjectSql.Core.QueryBuilder
{
	public class SqlWriterContext
	{
		public Expression InitialExpression { get; set; }
		public ISqlQueryBuilder QueryBuilder { get; set; }
		public BuilderContext Context { get; set; }
		public CommandText CommandText { get; set; }

		public SqlWriterContext(Expression initialExpression, ISqlQueryBuilder queryBuilder, BuilderContext context,
		                        CommandText commandText)
		{
			InitialExpression = initialExpression;
			QueryBuilder = queryBuilder;
			Context = context;
			CommandText = commandText;
		}

		public string BuildSql(Expression ex)
		{
			return QueryBuilder.BuildSql(ex);
		}
		public string BuildSql(string dbTypeName, Expression ex)
		{
			Context.DbTypeInContext = Context.SchemaManager.ParseDbType(dbTypeName);
			return QueryBuilder.BuildSql(ex);
		}
		public void UpdateTypeInContext(string dbTypeName)
		{
			Context.DbTypeInContext = Context.SchemaManager.ParseDbType(dbTypeName);
		}
	}
}
