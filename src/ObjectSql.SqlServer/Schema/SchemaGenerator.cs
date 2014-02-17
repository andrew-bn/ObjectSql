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
				var p = Procedures(connection).ToList();

				FillTableParameters(t, connection);
				FillProcedureParameters(p, connection);

				t.OrderBy(tbl => tbl.Schema + "." + tbl.Name);
				p.OrderBy(proc => proc.Schema + "." + proc.Name);

				foreach (var tbl in t)
					tbl.Columns = tbl.Columns.OrderBy(c => c.Position).ToList();

				foreach (var proc in p)
					proc.Parameters = proc.Parameters.OrderBy(par => par.Position).ToList();

				var schema = new DatabaseSchema() { Tables = t.ToArray(), Procedures = p.ToArray() };
				var template = new DatabaseSchemaTemplate(connStrName, schema, ns);
				return template.TransformText();
			}
		}

		private static void FillTableParameters(IEnumerable<Table> tables, SqlConnection connection)
		{
			var table = connection.GetSchema("Columns");
			foreach (DataRow row in table.Rows)
			{
				var column = new Column();
				foreach (DataColumn col in table.Columns)
				{
					if (Column(col, "table_schema"))
						column.Schema = row[col].ToString();
					if (Column(col, "table_name"))
						column.TableName = row[col].ToString();
					if (Column(col, "ordinal_position"))
						column.Position = int.Parse(row[col].ToString());
					if (Column(col, "is_nullable"))
						column.IsNullable = row[col].ToString().ToLower() == "yes";
					if (Column(col, "column_name"))
						column.Name = (row[col] is DBNull) ? null : row[col].ToString();
					if (Column(col, "data_type"))
						column.DataType = row[col].ToString();
				}
				column.NetType = MapToNetType(column.DataType, column.IsNullable);
				tables.First(p => p.Name == column.TableName && p.Schema == column.Schema).Columns.Add(column);
			}
		}

		private static void FillProcedureParameters(IEnumerable<Procedure> procedures, SqlConnection connection)
		{
			var table = connection.GetSchema("ProcedureParameters");
			foreach (DataRow row in table.Rows)
			{
				var param = new Parameter();
				foreach (DataColumn col in table.Columns)
				{
					if (Column(col, "specific_schema"))
						param.Schema = row[col].ToString();
					if (Column(col, "specific_name"))
						param.ProcedureName = row[col].ToString();
					if (Column(col, "parameter_mode"))
						param.ParameterType = (ParameterType)Enum.Parse(typeof(ParameterType), row[col].ToString(), true);
					if (Column(col, "ordinal_position"))
						param.Position = int.Parse(row[col].ToString());
					if (Column(col, "is_result"))
						param.IsResult = row[col].ToString().ToLower() == "yes";
					if (Column(col, "parameter_name"))
						param.Name = (row[col] is DBNull) ? null : row[col].ToString();
					if (Column(col, "data_type"))
					{
						param.DataType = row[col].ToString();
						param.NetType = MapToNetType(param.DataType, true);
					}
				}
				procedures.First(p => p.Name == param.ProcedureName && p.Schema == param.Schema).Parameters.Add(param);
			}
		}

		private static bool Column(DataColumn col, string name)
		{
			return col.ColumnName.Equals(name, StringComparison.OrdinalIgnoreCase);
		}
		private static IEnumerable<Table> Tables(SqlConnection connection)
		{
			var table = connection.GetSchema("Tables");
			foreach (DataRow row in table.Rows)
			{
				var p = new Table();
				foreach (DataColumn col in table.Columns)
				{
					if (col.ColumnName.ToLower() == "table_schema")
						p.Schema = row[col].ToString();
					if (col.ColumnName.ToLower() == "table_name")
						p.Name = row[col].ToString();
				}
				yield return p;
			}
		}
		private static IEnumerable<Procedure> Procedures(SqlConnection connection)
		{
			var table = connection.GetSchema("Procedures");
			foreach (DataRow row in table.Rows)
			{
				var p = new Procedure();
				foreach (DataColumn col in table.Columns)
				{
					if (col.ColumnName.ToLower() == "routine_schema")
						p.Schema = row[col].ToString();
					if (col.ColumnName.ToLower() == "routine_name")
						p.Name = row[col].ToString();
				}
				yield return p;
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