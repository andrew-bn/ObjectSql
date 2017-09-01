using ObjectSql.SqlServer.Schema;
using System;
using System.IO;
using System.Linq;

namespace dotnet_objsql
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var curDir = Directory.GetCurrentDirectory();
			var projectName = curDir.Substring(curDir.LastIndexOf(Path.DirectorySeparatorChar) + 1);

			var mySqlFiles = Directory.GetFiles(curDir, "*.mysql.connstr", SearchOption.AllDirectories)
								.Select(f => new { file = f, sqlType = "mysql", ext = ".mysql.connstr" });

			var files = Directory.GetFiles(curDir, "*.sql.connstr", SearchOption.AllDirectories)
				.Select(f => new {file = f, sqlType="sql", ext = ".sql.connstr"})
				.Concat(mySqlFiles).ToArray();

			foreach (var f in files)
			{
				var dbDir = Path.GetDirectoryName(f.file);
				var configFileName = Path.GetFileName(f.file);

				var ns = projectName + dbDir.Substring(curDir.Length).Replace(Path.DirectorySeparatorChar, '.');
				var csFileName = configFileName;
				csFileName = Path.Combine(dbDir, csFileName.Substring(0, csFileName.IndexOf(f.ext, StringComparison.Ordinal)) + ".cs");
				var cs = File.ReadAllText(f.file).Trim().Replace("{projectDir}", curDir);	
				var proceduresName = dbDir.Substring(dbDir.LastIndexOf(Path.DirectorySeparatorChar) + 1);

				Console.Write($@"

ConnetionString: {cs}
FileName: {csFileName}

Script generation. Please wait...");

				switch (f.sqlType)
				{
					case "sql":
						File.WriteAllText(csFileName, SchemaGenerator.Generate(ns, proceduresName, cs));
						break;
					case "mysql":
						File.WriteAllText(csFileName, ObjectSql.MySql.Schema.SchemaGenerator.Generate(ns, proceduresName, cs));
						break;
				}

				Console.WriteLine(" Done");
			}

		}
	}
}
