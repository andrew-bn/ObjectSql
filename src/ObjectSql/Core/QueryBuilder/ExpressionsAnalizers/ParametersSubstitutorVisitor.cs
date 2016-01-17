using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ObjectSql.Core.Misc;

namespace ObjectSql.Core.QueryBuilder.ExpressionsAnalizers
{
	public class ParametersSubstitutorVisitor: ExpressionVisitor
	{
		private readonly ParameterExpression[] _parameters;

		public ParametersSubstitutorVisitor(ParameterExpression[] parameters)
		{
			_parameters = parameters;
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			if (_parameters.Contains(node))
			{
				var s = (IParameterSubstitutor)Activator.CreateInstance(typeof (ParametersSubstitutor<>).MakeGenericType(node.Type));
				s.Name = node.Name;
				return Expression.MakeMemberAccess(Expression.Constant(s,s.GetType()), s.GetType().GetProp("Table"));
			}
			return node;
		}
	}
}
