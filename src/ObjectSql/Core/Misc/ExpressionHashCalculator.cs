using System;
using System.Linq.Expressions;
using ObjectSql.Core.Bo;

namespace ObjectSql.Core.Misc
{
	public static class ExpressionHashCalculator
	{
		const int PRIME = 397;

		public static void CalculateHashAndExtractConstantRoots(Expression expression, ref QueryRoots treeParameters)
		{
			foreach (var node in ExpressionEnumerator.Enumerate(expression))
			{
				var exp = node as Expression;
				if (exp != null)
					HashExpression(exp, ref treeParameters);
				else
				{
					HashBinding(node as MemberBinding, ref treeParameters);
				}
			}
		}
		private static void HashBinding(MemberBinding bind, ref QueryRoots treeParameters)
		{
			if (bind == null) return;
			treeParameters.Hash *= PRIME;
			treeParameters.Hash ^= bind.Member.GetHashCode();
		}
		private static void HashExpression(Expression node, ref QueryRoots parameters)
		{
			parameters.Hash *= PRIME;
			parameters.Hash ^= (int)node.NodeType;
			parameters.Hash *= PRIME;
			parameters.Hash ^= node.Type.GetHashCode();
			parameters.Hash *= PRIME;
			switch (node.NodeType)
			{
				
				case ExpressionType.MemberAccess:
					var memberNode = (MemberExpression) node;
					parameters.Hash ^= memberNode.Member.GetHashCode();
					break;
				case ExpressionType.Constant:
					var value = ((ConstantExpression)node).Value;
					if (value != null)
						parameters.AddRoot(value);
					else
						parameters.Hash ^= DBNull.Value.GetHashCode();
					break;
				case ExpressionType.Call:
					parameters.Hash ^= ((MethodCallExpression)node).Method.GetHashCode();
					break;
				case ExpressionType.New:
					var newExp = ((NewExpression)node);
					parameters.Hash ^= newExp.Constructor.GetHashCode();
					if (newExp.Members != null)
						for (int i = 0; i < newExp.Members.Count; i++)
						{
							parameters.Hash *= PRIME;
							parameters.Hash ^= newExp.Members[i].GetHashCode();
						}
					break;
			}
		}
	}
}
