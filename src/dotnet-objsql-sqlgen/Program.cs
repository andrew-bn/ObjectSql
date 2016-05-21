using ObjectSql.SqlServer.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace dotnet_objsql_sqlgen
{
    public class Program
    {
        public static void Main(string[] args)
        {
	        var curDir = Directory.GetCurrentDirectory();
			var projectName = curDir.Substring(curDir.LastIndexOf(Path.DirectorySeparatorChar)+1);

			if (!File.Exists(Path.Combine(curDir,"project.json")))
				throw new FileNotFoundException("It is expected that objsql-sqlgen utility runs at the root of project folder");

			var files = Directory.GetFiles(curDir, "*.objsql.*.json", SearchOption.AllDirectories);
	        foreach (var f in files)
	        {
		        var dbDir = Path.GetDirectoryName(f);
		        var configFileName = Path.GetFileName(f);

				var ns = projectName + dbDir.Substring(curDir.Length).Replace(Path.DirectorySeparatorChar,'.');
		        var csFileName = configFileName;
		        csFileName = Path.Combine(dbDir, csFileName.Substring(0, csFileName.IndexOf(".objsql"))+".cs");
		        var config = new ConfigurationBuilder()
											.SetBasePath(dbDir)
											.AddJsonFile(configFileName)
											.Build();

		        var cs = config["connectionstring"].Replace("{projectDir}", curDir);
		        var proceduresName = dbDir.Substring(dbDir.LastIndexOf(Path.DirectorySeparatorChar)+1);
		        

				Console.Write($@"

ConnetionString: {cs}
FileName: {csFileName}

Script generation. Please wait...");
		        File.WriteAllText(csFileName, SchemaGenerator.Generate(ns, proceduresName, cs));
				Console.WriteLine(" Done");
			}
	
        }
    }
}
