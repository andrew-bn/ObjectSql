using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
	class Program
	{
		static void Main(string[] args)
		{
			var rep = new PremiumDnsRepository();
			while (true)
			{
				var info = rep.SubscriptionInfoByDomainName("ab-3916-410.com ");
				var info2 = rep.SubscriptionInfoByDomainName("ab-3916-411.com ");
			}
		}
	}
}
