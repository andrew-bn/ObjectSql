﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ObjectSql.SqlServer.Schema;

namespace ObjectSql.SqlServer
{
    public static class Program
    {
		public static void Main(string[] args)
		{
			var ns = args[0];
			var filename = args[1];
			var cs = args[2];

			Console.Write($@"

ConnetionString: {cs}
DefaultNamespace: {ns}
FileName: {filename}

Script generation. Please wait...");
			File.WriteAllText(filename, SchemaGenerator.Generate(ns, "connstr", cs));
			Console.WriteLine(" Done");

		}
    }
}
