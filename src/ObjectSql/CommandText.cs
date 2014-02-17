using System.Text;

namespace ObjectSql
{
	public class CommandText
	{
		private readonly StringBuilder _sb = new StringBuilder();

		public CommandText Append(string format)
		{
			Append(format, new object[] {});
			return this;
		}

		public CommandText Append(string format, params object[] args)
		{
			_sb.AppendFormat(format, args);
			return this;
		}
		public override string ToString()
		{
			return _sb.ToString();
		}
	}

}
