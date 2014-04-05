using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql.Core
{
	public class ExpressionVisitorManager: ExpressionVisitor
	{
		private readonly Delegate[] _visitors;

		public ExpressionVisitorManager(params Delegate[] visitors)
		{
			_visitors = visitors;
		}

		public override Expression Visit(Expression node)
		{
			var visitor = FindVisitor(node.GetType());
			if (visitor == null)
				return base.Visit(node);

			return (Expression) visitor.DynamicInvoke(this, node);
		}

		private Delegate FindVisitor(Type expType)
		{
			return _visitors.FirstOrDefault(d => d.Method.GetParameters()[1].ParameterType.IsAssignableFrom(expType));
		}
	}
}
