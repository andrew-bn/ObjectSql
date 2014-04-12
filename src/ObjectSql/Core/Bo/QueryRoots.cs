using System;
using System.Collections.Generic;
using ObjectSql.Exceptions;

namespace ObjectSql.Core.Bo
{
	public class QueryRoots
	{
		public static QueryRoots Empty = new QueryRoots();
		private List<object> _roots = new List<object>();
		public IList<object> Roots { get { return _roots; } }

		public bool ContainsRoot(object value)
		{
			return value != null && _roots.Contains(value);
		}
		public object Get(int index)
		{
			return _roots[index];
		}
		public int IndexOf(object value)
		{
			return _roots.IndexOf(value);
		}
		public int IndexOf(Func<object ,bool> predicate)
		{
			foreach (var o in _roots)
			{
				if (predicate(o))
					return _roots.IndexOf(o);
			}
			return -1;
		}
		public void AddRoot(object value)
		{
			if (!_roots.Contains(value))
				_roots.Add(value);
		 }
		public int Hash;

		internal void ClearRoots()
		{
			_roots.Clear();
		}


		internal void ReplaceRoot(int rootIndex, object newRootValue)
		{
			_roots.RemoveAt(rootIndex);
			_roots.Insert(rootIndex,newRootValue);
		}
	}
}
