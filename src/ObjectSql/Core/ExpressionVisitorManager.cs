using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace ObjectSql.Core
{
	public class ExpressionVisitorManager<T1>: ExpressionVisitor where T1 : Expression
	{
		private readonly Func<ExpressionVisitor, T1, Expression> _visitor;

		public ExpressionVisitorManager(Func<ExpressionVisitor, T1, Expression> visitor)
		{
			_visitor = visitor;
		}

		public override Expression Visit(Expression node)
		{
			if (node == null)
			{
				return node;
			}
			else if (!typeof (T1).IsAssignableFrom(node.GetType()))
			{
				return base.Visit(node);
			}

			return _visitor(this, (T1)node);
		}
	}
}
