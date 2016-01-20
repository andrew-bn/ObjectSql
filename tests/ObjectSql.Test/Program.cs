using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObjectSql.Test
{
    public static class Program
    {
		public static void Main(string[] args)
		{
			if (args[0] == "load")
			{
				LoadTestHolder.Run();
			}
				
		}
    }
}
