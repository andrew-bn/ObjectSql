using ObjectSql.SqlServer.Schema;
using System;
using System.IO;

namespace dotnet_objsql
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var curDir = Directory.GetCurrentDirectory();
			var projectName = curDir.Substring(curDir.LastIndexOf(Path.DirectorySeparatorChar) + 1);

			if (!File.Exists(Path.Combine(curDir, "project.json")))
				throw new FileNotFoundException("It is expected that objsql-sqlgen utility runs at the root of project folder");

			var files = Directory.GetFiles(curDir, "*.sql.connstr", SearchOption.AllDirectories);
			foreach (var f in files)
			{
				var dbDir = Path.GetDirectoryName(f);
				var configFileName = Path.GetFileName(f);

				var ns = projectName + dbDir.Substring(curDir.Length).Replace(Path.DirectorySeparatorChar, '.');
				var csFileName = configFileName;
				csFileName = Path.Combine(dbDir, csFileName.Substring(0, csFileName.IndexOf(".sql.connstr")) + ".cs");
				var cs = File.ReadAllText(f).Trim().Replace("{projectDir}", curDir);
				var proceduresName = dbDir.Substring(dbDir.LastIndexOf(Path.DirectorySeparatorChar) + 1);


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
