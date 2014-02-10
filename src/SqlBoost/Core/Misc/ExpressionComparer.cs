using System.Linq.Expressions;
using SqlBoost.Core.Bo;

namespace SqlBoost.Core.Misc
{
	internal static class ExpressionComparer
	{
		/// <summary>
		/// Compare two expressions. All constants are compared too
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool AreEqual(Expression a, Expression b)
		{
			if ((a == null) != (b == null))
				return false;
			if (a == null) return true;

			return AreEqual(a, b, false, ref QueryRoots.Empty, ref QueryRoots.Empty);
		}
		/// <summary>
		/// Compare two expressions. Only not root constants are compared
		/// </summary>
		/// <param name="a"></param>
		/// <param name="pa"></param>
		/// <param name="b"></param>
		/// <param name="pb"></param>
		/// <returns></returns>
		public static bool AreEqual(Expression a, ref QueryRoots pa, Expression b, ref QueryRoots pb)
		{
			if ((a == null) != (b == null))
				return false;
			if (a == null) return true;

			return AreEqual(ref pa, ref pb) && AreEqual(a, b, false, ref pa, ref pb);
		}

		public static bool AreEqual(ref QueryRoots pa, ref QueryRoots pb)
		{
			if (pa.Hash != pb.Hash)
				return false;
			if (pa.Roots.Count != pb.Roots.Count)
				return false;

			using (var aEnumerator = pa.Roots.GetEnumerator())
			{
				using (var bEnumerator = pb.Roots.GetEnumerator())
				{
					while (aEnumerator.MoveNext())
					{
						if (!bEnumerator.MoveNext())
							return false;
						if (aEnumerator.Current.Value != bEnumerator.Current.Value)
							return false;
						if (aEnumerator.Current.Key.GetType() != bEnumerator.Current.Key.GetType())
							return false;
					}
					return true;
				}
			}
		}

		private static bool AreEqual(Expression a, Expression b, bool compareAllConstants, ref QueryRoots pa, ref QueryRoots pb)
		{
			using (var aEnumerator = ExpressionEnumerator.Enumerate(a).GetEnumerator())
			{
				using (var bEnumerator = ExpressionEnumerator.Enumerate(b).GetEnumerator())
				{

					while (true)
					{
						var moveNext = aEnumerator.MoveNext();
						if (moveNext != bEnumerator.MoveNext())
							return false;
						if (!moveNext)
							break;

						var expA = aEnumerator.Current as Expression;
						var expB = bEnumerator.Current as Expression;
						if ((expA == null) != (expB == null))
							return false;
						if (expA != null)
						{
							if (!ExpressionsAreEqual(expA, expB, compareAllConstants, ref pa, ref pb))
								return false;
						}
					}
					return true;
				}
			}
		}
		private static bool ExpressionsAreEqual(Expression a, Expression b, bool compareAllConstants, ref QueryRoots pa, ref QueryRoots pb)
		{
			if (a.NodeType != b.NodeType)
				return false;
			if (a.Type != b.Type)
				return false;

			if (a.NodeType == ExpressionType.Constant)
			{
				var ca = (ConstantExpression)a;
				var cb = (ConstantExpression)b;

				if (compareAllConstants && !Equals(ca.Value, cb.Value))
					return false;


				var isRoot = pa.ContainsRoot(ca.Value);
				if (isRoot != pb.ContainsRoot(cb.Value))
					return false;
				if (!isRoot && !Equals(ca.Value, cb.Value))
					return false;

			}
			else if (a.NodeType == ExpressionType.MemberAccess && ((MemberExpression)a).Member != ((MemberExpression)b).Member)
				return false;
			else if (a.NodeType == ExpressionType.Call && ((MethodCallExpression)a).Method != ((MethodCallExpression)b).Method)
				return false;
			else if (a.NodeType == ExpressionType.New)
			{
				var newA = (NewExpression)a;
				var newB = (NewExpression)b;
				if (newA.Constructor != newB.Constructor)
					return false;

				if ((newA.Members == null) != (newB.Members == null))
					return false;

				if (newA.Members != null)
				{
					if (newA.Members.Count != newB.Members.Count)
						return false;

					for (int i = 0; i < newA.Members.Count; i++)
						if (newA.Members[i] != newB.Members[i]) return false;
				}
			}
			return true;
		}


	}
}
