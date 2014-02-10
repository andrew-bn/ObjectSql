using System.Collections.Generic;
using System.Linq.Expressions;

namespace SqlBoost.Core.Misc
{
	internal static class ExpressionEnumerator
	{
		public static IEnumerable<object> Enumerate(Expression exp)
		{
			var stack = new Stack<object>();
			object currentObj = exp;
			while (true)
			{
				yield return currentObj;
				var currentExpr = currentObj as Expression;
				if (currentExpr != null)
				{
					switch (currentExpr.NodeType)
					{
						case ExpressionType.Negate:
						case ExpressionType.NegateChecked:
						case ExpressionType.Not:
						case ExpressionType.Convert:
						case ExpressionType.ConvertChecked:
						case ExpressionType.ArrayLength:
						case ExpressionType.Quote:
						case ExpressionType.TypeAs:
							currentObj = ((UnaryExpression)currentObj).Operand;
							continue;
						case ExpressionType.Add:
						case ExpressionType.AddChecked:
						case ExpressionType.Subtract:
						case ExpressionType.SubtractChecked:
						case ExpressionType.Multiply:
						case ExpressionType.MultiplyChecked:
						case ExpressionType.Divide:
						case ExpressionType.Modulo:
						case ExpressionType.And:
						case ExpressionType.AndAlso:
						case ExpressionType.Or:
						case ExpressionType.OrElse:
						case ExpressionType.LessThan:
						case ExpressionType.LessThanOrEqual:
						case ExpressionType.GreaterThan:
						case ExpressionType.GreaterThanOrEqual:
						case ExpressionType.Equal:
						case ExpressionType.NotEqual:
						case ExpressionType.Coalesce:
						case ExpressionType.ArrayIndex:
						case ExpressionType.RightShift:
						case ExpressionType.LeftShift:
						case ExpressionType.ExclusiveOr:
							var binary = ((BinaryExpression)currentObj);
							stack.Push(binary.Right);
							currentObj = binary.Left;
							continue;
						case ExpressionType.TypeIs:
							var tis = ((TypeBinaryExpression)currentObj);
							currentObj = tis.Expression;
							continue;
						case ExpressionType.Conditional:
							var cond = ((ConditionalExpression)currentObj);
							currentObj = cond.Test;
							stack.Push(cond.IfFalse);
							stack.Push(cond.IfTrue);
							continue;
						case ExpressionType.Constant:
						case ExpressionType.Parameter:
							if (stack.Count == 0)
								yield break;
							currentObj = stack.Pop();
							continue;
						case ExpressionType.MemberAccess:
							var me = ((MemberExpression)currentObj);
							currentObj = me.Expression;
							continue;
						case ExpressionType.Call:
							var call = ((MethodCallExpression)currentObj);
							currentObj = call.Object;
							for (int i = call.Arguments.Count - 1; i >= 0; i--)
								stack.Push(call.Arguments[i]);
							continue;

						case ExpressionType.Lambda:
							var lambda = ((LambdaExpression)currentObj);
							currentObj = lambda.Body;
							for (int i = lambda.Parameters.Count - 1; i >= 0; i--)
								stack.Push(lambda.Parameters[i]);
							continue;

						case ExpressionType.New:
							var newExp = ((NewExpression)currentObj);

							if (newExp.Arguments.Count > 0)
							{
								currentObj = newExp.Arguments[0];
								for (int i = newExp.Arguments.Count - 1; i > 0; i--)
									stack.Push(newExp.Arguments[i]);
								continue;
							}
							if (stack.Count == 0)
							yield break;

							currentObj = stack.Pop();
							continue;
						case ExpressionType.NewArrayInit:
						case ExpressionType.NewArrayBounds:
							var newArr = ((NewArrayExpression)currentObj);
							if (newArr.Expressions.Count > 0)
							{
								currentObj = newArr.Expressions[0];
								for (int i = newArr.Expressions.Count - 1; i > 0; i--)
									stack.Push(newArr.Expressions[i]);
								continue;
							}
							if (stack.Count == 0)
							yield break;

							currentObj = stack.Pop();
							continue;
						case ExpressionType.Invoke:
							var inv = ((InvocationExpression)currentObj);
							currentObj = inv.Expression;

							for (int i = inv.Arguments.Count - 1; i >= 0; i--)
								stack.Push(inv.Arguments[i]);
							continue;


						case ExpressionType.MemberInit:
							var memInit = ((MemberInitExpression)currentObj);
							currentObj = memInit.NewExpression;

							for (int i = memInit.Bindings.Count - 1; i >= 0; i--)
								stack.Push(memInit.Bindings[i]);

							continue;
						case ExpressionType.ListInit:
							var listInit = ((ListInitExpression)currentObj);
							currentObj = listInit.NewExpression;
							for (int i = listInit.Initializers.Count - 1; i >= 0; i--)
								stack.Push(listInit.Initializers[i]);
							continue;
					}
				}

				var memBind = currentObj as MemberBinding;
				if (memBind != null)
				{
					switch (memBind.BindingType)
					{
						case MemberBindingType.Assignment:
							var memAss = (MemberAssignment)memBind;
							stack.Push(memAss.Expression);
							break;
						case MemberBindingType.ListBinding:
							var listBind = (MemberListBinding)memBind;
							for (int j = listBind.Initializers.Count - 1; j >= 0; j--)
								stack.Push(listBind.Initializers[j]);
							break;
						case MemberBindingType.MemberBinding:
							var memmemBind = (MemberMemberBinding)memBind;
							for (int i = memmemBind.Bindings.Count - 1; i >= 0; i--)
								stack.Push(memmemBind.Bindings[i]);
							break;
					}
				}
				var elemInit = currentObj as ElementInit;
				if (elemInit != null)
				{
					for (int i = elemInit.Arguments.Count - 1; i >= 0; i--)
						stack.Push(elemInit.Arguments[i]);
				}

				if (stack.Count == 0)
					yield break;

				currentObj = stack.Pop();
			}
		}
	}
}
