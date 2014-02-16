using System.Collections.Generic;
using SqlBoost.Exceptions;

namespace SqlBoost.Core.Bo
{
	public struct QueryRoots
	{
		public static QueryRoots Empty = new QueryRoots();

		const int MAX_PARAMETERS_COUNT = sizeof(int)*8;
		private int _rootsCount;
		private Dictionary<object, int> _roots;
		public ICollection<KeyValuePair<object, int>> Roots { get { return _roots != null ? _roots : new Dictionary<object, int>(); } }
		public bool RootsGenerated
		{
			get { return _roots != null; }
		
		}
		public bool ContainsRoot(object value)
		{
			return value!=null && _roots != null && _roots.ContainsKey(value);
		}
		public void AddRoot(object value)
		{
			if (_roots == null)
			{
				_roots = new Dictionary<object, int>();
				_rootsCount = -1;
			}
			++_rootsCount;

			if (_rootsCount >= MAX_PARAMETERS_COUNT)
				throw new SqlBoostException("Maximum parameters count acceded");

			int currentIndex;
			if (_roots.TryGetValue(value, out currentIndex))
				_roots[value] = currentIndex | (1 << _rootsCount);
			else
				_roots.Add(value, 1 << _rootsCount);
		 }
		public int Hash;

		internal void ClearRoots()
		{
			if (_roots!=null)
				_roots.Clear();
		}

		public void AddRoot(object value,int rootMap)
		{
			if (value == null)
				return;

			if (_roots == null)
			{
				_roots = new Dictionary<object, int>();
				_rootsCount = -1;
			}
			_roots.Add(value,rootMap);
		}
	}
}
