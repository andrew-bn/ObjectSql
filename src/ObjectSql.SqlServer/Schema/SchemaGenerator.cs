using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ObjectSql.Exceptions;

namespace ObjectSql.SqlServer.Schema
{
	public static class SchemaGenerator
	{
		public static string Generate(string ns, string connStrName, string connectionString)
		{
			if (ns == null)
				ns = "";

			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				var t = Tables(connection).ToArray();
				var p = ProceduresAndFunctions(connection).ToArray();

				FillTableParameters(t, connection);
				FillProcedureParameters(p, connection);

				t = t.OrderBy(tbl => tbl.Schema + "." + tbl.Name).ToArray();
				p = p.OrderBy(proc => proc.Schema + "." + proc.Name).ToArray();

				foreach (var tbl in t)
					tbl.Columns = tbl.Columns.OrderBy(c => c.Position).ToList();

				foreach (var proc in p)
				{
					proc.Parameters = proc.Parameters.OrderBy(par => par.Position).ToList();
					
					if (p.Any(pr => pr!=proc && pr.Name.Equals(proc.Name, StringComparison.OrdinalIgnoreCase)))
						proc.UseSchema = true;
				}

				var schema = new DatabaseSchema()
					{
						Tables = t.ToArray(),
						Procedures = p.Where(prc => prc.RoutineType == RoutineType.Procedure).ToArray(),
						Functions = p.Where(f => f.RoutineType == RoutineType.Function).ToArray()
					};
				var template = new DatabaseSchemaTemplate(connStrName, schema, ns);
				return template.TransformText();
			}
		}

		private static void FillTableParameters(IEnumerable<Table> tables, SqlConnection connection)
		{
			using (var cmd = connection.CreateCommand())
			{
				cmd.CommandText = "SELECT * FROM information_schema.COLUMNS";
				using (var row = cmd.ExecuteReader())
				{
					while (row.Read())
					{
						var column = new Column();
						column.Schema = row["TABLE_SCHEMA"].ToString();
						column.TableName = row["TABLE_NAME"].ToString();
						column.Position = int.Parse(row["ORDINAL_POSITION"].ToString());
						column.IsNullable = row["IS_NULLABLE"].ToString().ToLower() == "yes";
						column.Name = (row["COLUMN_NAME"] is DBNull) ? null : row["COLUMN_NAME"].ToString();
						column.DataType = row["DATA_TYPE"].ToString();
						column.NetType = MapToNetType(column.DataType, column.IsNullable);
						tables.First(p => p.Name == column.TableName && p.Schema == column.Schema).Columns.Add(column);
					}
				}
			}
		}

		private static void FillProcedureParameters(IEnumerable<Procedure> procedures, SqlConnection connection)
		{
			using (var cmd = connection.CreateCommand())
			{
				cmd.CommandText = "SELECT * FROM information_schema.PARAMETERS";
				using (var row = cmd.ExecuteReader())
				{
					while (row.Read())
					{
						var param = new Parameter();

						param.Schema = row["SPECIFIC_SCHEMA"].ToString();
						param.ProcedureName = row["SPECIFIC_NAME"].ToString();
						param.Direction = ParseParameterMode(row["PARAMETER_MODE"].ToString());
						param.Position = int.Parse(row["ORDINAL_POSITION"].ToString());
						param.IsResult = row["IS_RESULT"].ToString().ToLower() == "yes";
						param.Name = (row["PARAMETER_NAME"] is DBNull) ? null : row["PARAMETER_NAME"].ToString();
						param.DataType = row["DATA_TYPE"].ToString();
						param.NetType = MapToNetType(param.DataType, true);

						if (param.Position == 0)
							param.Direction = ParameterDirection.ReturnValue;

						procedures.First(p => p.Name == param.ProcedureName && p.Schema == param.Schema).Parameters.Add(param);

					}
				}
			}
		}

		private static ParameterDirection ParseParameterMode(string value)
		{
			value = value.ToLower();
			if (value == "in")
				return ParameterDirection.Input;
			if (value == "out")
				return ParameterDirection.Output;
			if (value == "inout")
				return ParameterDirection.InputOutput;
			return ParameterDirection.ReturnValue;
		}

		private static IEnumerable<Table> Tables(SqlConnection connection)
		{
			using (var cmd = connection.CreateCommand())
			{
				cmd.CommandText = "SELECT * FROM information_schema.tables";
				using (var r = cmd.ExecuteReader())
				{
					while(r.Read())
					{
						var p = new Table();
						p.Schema = r["TABLE_SCHEMA"].ToString();
						p.Name = r["TABLE_NAME"].ToString();
						yield return p;
					}
				}
			}
		}
		private static IEnumerable<Procedure> ProceduresAndFunctions(SqlConnection connection)
		{
			using (var cmd = connection.CreateCommand())
			{
				cmd.CommandText = "SELECT * FROM information_schema.ROUTINES";
				using (var row = cmd.ExecuteReader())
				{
					while (row.Read())
					{
						var p = new Procedure();
						p.Schema = row["ROUTINE_SCHEMA"].ToString();
						p.Name = row["ROUTINE_NAME"].ToString();
						p.RoutineType = (row["ROUTINE_TYPE"].ToString().ToLower() == "procedure")
											? RoutineType.Procedure
											: RoutineType.Function;
						yield return p;
					}
				}
			}
		}

		private static Type MapToNetType(string dataType, bool nullable)
		{
			dataType = dataType.ToLower();
			switch (dataType)
			{
				case "bigint":
					return typeof(long);
				case "binary":
				case "filestream":
				case "image":
				case "rowversion":
				case "timestamp":
				case "varbinary":
					return typeof(byte[]);
				case "uniqueidentifier":
					return typeof(Guid);
				case "int":
					return typeof(int);
				case "smallint":
					return typeof(short);
				case "xml":
					return typeof(XmlReader);
				case "tinyint":
					return typeof(byte);
				case "bit":
					return typeof(bool);
				case "char":
				case "nchar":
				case "ntext":
				case "text":
				case "nvarchar":
				case "varchar":
					return typeof(string);
				case "date":
				case "datetime":
				case "datetime2":
				case "smalldatetime":
					return typeof(DateTime);
				case "datetimeoffset":
					return typeof(DateTimeOffset);
				case "decimal":
				case "money":
				case "numeric":
				case "smallmoney":
					return typeof(decimal);
				case "float":
					return typeof(double);
				case "real":
					return typeof(float);
				case "sql_variant":
					return typeof(object);
				case "time":
					return typeof(TimeSpan);
				default:
					return typeof(object);
			}
		}
	}
}
/*

<#@ template debug="false" hostspecific="true" language="C#" #>
<#@ assembly name="System.Core" #>
<#@ assembly name="System.Configuration" #>
<#@ assembly name="EnvDTE" #> 
<#@ assembly name="$(ProjectDir)bin\$(ConfigurationName)\ObjectSql.Dll" #>
<#@ assembly name="$(ProjectDir)bin\$(ConfigurationName)\ObjectSql.SqlServer.Dll" #>
<#@ import namespace="System.Linq" #>
<#@ import namespace="System.Text" #>
<#@ import namespace="System.Collections.Generic" #>
<#@ import namespace="ObjectSql.SqlServer" #>

<#@ import namespace="System.Configuration" #>
<#@ import namespace="System.Text.RegularExpressions" #>
<#@ output extension=".txt" #>

<#  


var config = new ConfigurationAccessor((IServiceProvider)this.Host);
this.WriteLine("NS = "+ config.FindNs(this.Host.TemplateFile,this,(IServiceProvider)this.Host));


this.WriteLine("+!=+!@+#12=4321=34=213");
Action<string> header=new Action<string>((s)=>   { 
    this.WriteLine(""); 
    this.WriteLine("--------------------------------------------------------------"); 
    this.WriteLine(s); 
    this.WriteLine("--------------------------------------------------------------"); 
  });   
  header("ConfigurationAccessor.Project");   
  this.WriteLine("Project.FileName = {0}",config.Project.FileName); 
  this.WriteLine("Project.FullName = {0}",config.Project.FullName); 
  this.WriteLine("Project.Name = {0}",config.Project.Name);  
  this.WriteLine("TemplateFile = {0}",this.Host.TemplateFile);  
   this.WriteLine("Document.Name = {0}",config.Document.Name); 
  header("ConfigurationAccessor.Properties"); 
    var en = config.Properties.GetEnumerator();  
	 while(en.MoveNext())   { 
    var property = (EnvDTE.Property)en.Current; 
    object propertyValue = null;     try     { 
      propertyValue = property.Value;     } 
    catch (Exception ex)     {       propertyValue = ex.Message; 
    } 
    this.WriteLine("{0} = {1}",property.Name,propertyValue.ToString()); 
  }     
  header("ConfigurationAccessor.Configuration");  

  this.WriteLine("Enumerator access\r\n"); 
  en = config.AppSettings.GetEnumerator();  
   while(en.MoveNext())   { 
    var kv = (KeyValueConfigurationElement)en.Current; 
    this.WriteLine("{0} = {1}",kv.Key,kv.Value);   } 
	
  header("ConfigurationAccessor.ConnectionStrings"); 
 
   this.WriteLine("Enumerator access\r\n"); 
  en = config.ConnectionStrings.GetEnumerator();   while(en.MoveNext()) 
  {     var cs = (ConnectionStringSettings)en.Current; 
    this.WriteLine("{0}, {1}, {2}",cs.Name,cs.ProviderName,cs.ConnectionString); 
  } 


  #>


  <#+
public class ConfigurationAccessor
{
	public string FindNs(string fileName,Microsoft.VisualStudio.TextTemplating.TextTransformation tt,IServiceProvider host)
	{
		EnvDTE.DTE env = (EnvDTE.DTE)host.GetService(typeof(EnvDTE.DTE));
		var proj = env.Solution.Projects.GetEnumerator();

		while(proj.MoveNext())
		{
		    var p = (EnvDTE.Project)proj.Current; 
		    tt.WriteLine(p.Name);
			
			var res = FindNs(fileName,p.Name, tt,"    ",p.ProjectItems);
			if (res != null)
				return res;
		}
		return null;
	}

	public string FindNs(string fileName, string ns, Microsoft.VisualStudio.TextTemplating.TextTransformation tt, string tab,EnvDTE.ProjectItems items)
	{
		var itemsEn = items.GetEnumerator();

		while(itemsEn.MoveNext())
		{
		    var i = (EnvDTE.ProjectItem)itemsEn.Current;

			for(short j = 0;j<i.FileCount;j++)
			{
				if (fileName.ToLower() == i.FileNames[j].ToLower())
					return ns;
			}

			var res = FindNs(fileName, ns+"."+i.Name, tt,tab+"    ",i.ProjectItems);
			if (res != null)
				return res;
		}
		return null;
	}
	public ConfigurationAccessor(IServiceProvider host)
	{
		EnvDTE.DTE env = (EnvDTE.DTE)host.GetService(typeof(EnvDTE.DTE));
		_project = (EnvDTE.Project)((Array)env.ActiveSolutionProjects).GetValue(0);
		_document = (EnvDTE.Document)env.ActiveDocument;
		_documents = (EnvDTE.Documents)env.Documents;
		string configurationFilename=null;	
		
		// examine each project item's filename looking for app.config or web.config
		foreach (EnvDTE.ProjectItem item in _project.ProjectItems)
		{
			if (Regex.IsMatch(item.Name,"(app|web).config",RegexOptions.IgnoreCase))
			{
				// TODO: try this with linked files. is the filename pointing to the source?
				configurationFilename=item.get_FileNames(0);
				break;
			}
		}

		if(!string.IsNullOrEmpty(configurationFilename))
		{
			ExeConfigurationFileMap configFile = null;
			configFile = new ExeConfigurationFileMap();
			configFile.ExeConfigFilename=configurationFilename;
			_configuration = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);
 		}
	}
	
	private EnvDTE.Project _project;
	private Configuration _configuration;
	private EnvDTE.Document _document;
	public EnvDTE.Document Document
	{
		get { return _document; }
	}
	private EnvDTE.Documents _documents;
	public EnvDTE.Documents Documents
	{
		get { return _documents; }
	}
	public EnvDTE.Project Project
	{
		get { return _project; }
	}
	public EnvDTE.Properties Properties 
	{
		get { return _project.Properties;}
	}
	public Configuration Configuration
	{
		get { return _configuration; }
	}	
	public KeyValueConfigurationCollection AppSettings
	{
		get { return _configuration.AppSettings.Settings;}
	}
	public ConnectionStringSettingsCollection ConnectionStrings
	{
		get { return _configuration.ConnectionStrings.ConnectionStrings;}
	}

}
#>
*/