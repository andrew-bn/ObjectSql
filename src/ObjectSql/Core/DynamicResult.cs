using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql.Core
{
	public class DynamicResult : DynamicObject
	{
		private readonly ConcurrentDictionary<string, object> _values = new ConcurrentDictionary<string, object>();
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			result = GetValue(binder.Name);
			return true;
		}
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			SetValue(binder.Name, value);
			return true;
		}
		public object this[string name]
		{
			get { return GetValue(name); }
			set { SetValue(name, value); }
		}
		private object GetValue(string name)
		{
			return _values.GetOrAdd(name, key => null);
		}
		private void SetValue(string name, object value)
		{
			_values.AddOrUpdate(name, s => value, (s, o) => value);
		}
	}

}

