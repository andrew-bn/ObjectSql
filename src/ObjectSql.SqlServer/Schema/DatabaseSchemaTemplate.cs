﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 11.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace ObjectSql.SqlServer.Schema
{
    using System.Linq;
    using System.Data;
    using System.Text;
    using System.Collections.Generic;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "11.0.0.0")]
    public partial class DatabaseSchemaTemplate : DatabaseSchemaTemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write(@"//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from database.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

");
            this.Write("using System;\r\nusing System.Configuration;\r\nusing System.Data.SqlClient;\r\nusing S" +
                    "ystem.Collections.Generic;\r\nusing ObjectSql;\r\nusing ObjectSql.QueryInterfaces;\r\n" +
                    "\r\nnamespace ");
            
            #line 23 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n");
            
            #line 25 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
 
	string currentSchema = null;
	foreach(var tbl in Schema.Tables) 
	{
		if (currentSchema != tbl.Schema) 
		{
			if (currentSchema != null)
			{

            
            #line default
            #line hidden
            this.Write("\t}\r\n");
            
            #line 34 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
			}

            
            #line default
            #line hidden
            this.Write("\tnamespace ");
            
            #line 35 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tbl.Schema));
            
            #line default
            #line hidden
            this.Write("\r\n\t{\r\n");
            
            #line 37 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"

			currentSchema = tbl.Schema;
		}


            
            #line default
            #line hidden
            this.Write("\t\t[Table(\"");
            
            #line 41 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tbl.Name));
            
            #line default
            #line hidden
            this.Write("\",Schema=\"");
            
            #line 41 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tbl.Schema));
            
            #line default
            #line hidden
            this.Write("\")]\r\n\t\tpublic partial class ");
            
            #line 42 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ToValidName(tbl.Name)));
            
            #line default
            #line hidden
            this.Write("\r\n\t\t{\r\n");
            
            #line 44 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"

			foreach(var col in tbl.Columns)
			{

            
            #line default
            #line hidden
            this.Write("\t\t\t[Column(\"");
            
            #line 48 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(col.Name));
            
            #line default
            #line hidden
            this.Write("\",TypeName=\"");
            
            #line 48 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(col.DataType));
            
            #line default
            #line hidden
            this.Write("\")] public ");
            
            #line 48 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ToTypeName(col.NetType,col.IsNullable)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 48 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ToValidName(col.Name)));
            
            #line default
            #line hidden
            this.Write(" {get; set;}\r\n");
            
            #line 49 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"

			}

            
            #line default
            #line hidden
            this.Write("\t\t}\r\n\r\n");
            
            #line 54 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"


	}

            
            #line default
            #line hidden
            this.Write("\t}\r\n\r\n\tpublic partial class ");
            
            #line 60 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ConnStrName));
            
            #line default
            #line hidden
            this.Write("Context\r\n\t{\r\n\t\tprivate string _connectionString;\r\n\t\tprivate ObjectSqlManager<SqlC" +
                    "onnection> _sqlServerManager;\r\n\r\n\t\tpublic ");
            
            #line 65 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ConnStrName));
            
            #line default
            #line hidden
            this.Write("Context()\r\n\t\t{\r\n\t\t\t_connectionString = ConfigurationManager.ConnectionStrings[\"");
            
            #line 67 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ConnStrName));
            
            #line default
            #line hidden
            this.Write("\"].ConnectionString;\r\n\t\t\t_sqlServerManager = new ObjectSqlManager<SqlConnection>(" +
                    "_connectionString);\r\n\t\t}\r\n\r\n\t\tpublic IDatabaseContextHolder<");
            
            #line 71 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ConnStrName));
            
            #line default
            #line hidden
            this.Write("Context> Query()\r\n\t\t{\r\n\t\t\treturn _sqlServerManager.Query().WithContext<");
            
            #line 73 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ConnStrName));
            
            #line default
            #line hidden
            this.Write("Context>();\r\n\t\t}\r\n\t}\r\n\r\n\tpublic abstract class ");
            
            #line 77 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ConnStrName));
            
            #line default
            #line hidden
            this.Write("ProceduresHolder\r\n\t{\r\n");
            
            #line 79 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
 
		foreach(var proc in Schema.Procedures)
		{

            
            #line default
            #line hidden
            this.Write("\t\t[Procedure(\"");
            
            #line 83 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(proc.Name));
            
            #line default
            #line hidden
            this.Write("\", \"");
            
            #line 83 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(proc.Schema));
            
            #line default
            #line hidden
            this.Write("\")] public abstract void ");
            
            #line 83 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ToValidName(proc.Name)));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 83 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"

			foreach(var p in proc.Parameters){
            
            #line default
            #line hidden
            
            #line 84 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(p.Position>1?", ":""));
            
            #line default
            #line hidden
            this.Write("[Parameter(\"");
            
            #line 84 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(p.Name.Replace("@","")));
            
            #line default
            #line hidden
            this.Write("\", \"");
            
            #line 84 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(p.DataType));
            
            #line default
            #line hidden
            this.Write("\")] ");
            
            #line 84 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ToTypeName(p.NetType,true)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 84 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ToValidName(p.Name)));
            
            #line default
            #line hidden
            
            #line 84 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
}
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 85 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"

		}

            
            #line default
            #line hidden
            this.Write("\t}\r\n\r\n\tpublic static partial class ");
            
            #line 90 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ConnStrName));
            
            #line default
            #line hidden
            this.Write("ProceduresExtension\r\n\t{\r\n");
            
            #line 92 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
 
		foreach(var proc in Schema.Procedures)
		{

            
            #line default
            #line hidden
            this.Write("\t\tpublic static IQueryEnd ");
            
            #line 96 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ToValidName(proc.Name)));
            
            #line default
            #line hidden
            this.Write("(this IDatabaseContextHolder<");
            
            #line 96 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ConnStrName));
            
            #line default
            #line hidden
            this.Write("Context> holder");
            
            #line 96 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"

			foreach(var p in proc.Parameters){
            
            #line default
            #line hidden
            this.Write(",");
            
            #line 97 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
	if(IsOut(p)){
            
            #line default
            #line hidden
            this.Write("out ");
            
            #line 97 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
}if(IsInOut(p)){
            
            #line default
            #line hidden
            this.Write("ref ");
            
            #line 97 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
}
            
            #line default
            #line hidden
            
            #line 97 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ToTypeName(p.NetType,true)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 97 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ToValidName(p.Name)));
            
            #line default
            #line hidden
            
            #line 97 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
}
            
            #line default
            #line hidden
            this.Write(")\r\n\t\t{\r\n");
            
            #line 99 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"

			foreach(var p in proc.Parameters.Where(IsOut))
			{

            
            #line default
            #line hidden
            this.Write("\t\t\tvar ");
            
            #line 103 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ToValidName(p.Name)));
            
            #line default
            #line hidden
            this.Write("_out = default(");
            
            #line 103 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ToTypeName(p.NetType,true)));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 104 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"

			}

            
            #line default
            #line hidden
            
            #line 107 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"

			foreach(var p in proc.Parameters.Where(IsInOut))
			{

            
            #line default
            #line hidden
            this.Write("\t\t\tvar ");
            
            #line 111 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ToValidName(p.Name)));
            
            #line default
            #line hidden
            this.Write("_ref = ");
            
            #line 111 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ToValidName(p.Name)));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 112 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"

			}

            
            #line default
            #line hidden
            this.Write("\t\t\tvar ___lresl___ = holder.Exec<");
            
            #line 115 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ConnStrName));
            
            #line default
            #line hidden
            this.Write("ProceduresHolder>(h=>h.");
            
            #line 115 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ToValidName(proc.Name)));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 115 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"

				foreach(var p in proc.Parameters){
            
            #line default
            #line hidden
            
            #line 116 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(p.Position>1?", ":""));
            
            #line default
            #line hidden
            
            #line 116 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ToValidName(p.Name)));
            
            #line default
            #line hidden
            
            #line 116 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
if(IsOut(p)){
            
            #line default
            #line hidden
            this.Write("_out");
            
            #line 116 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
} if(IsInOut(p)){
            
            #line default
            #line hidden
            this.Write("_ref");
            
            #line 116 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
}
            
            #line default
            #line hidden
            
            #line 116 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
}
            
            #line default
            #line hidden
            this.Write("));\r\n");
            
            #line 117 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"

			foreach(var p in proc.Parameters.Where(IsInOut))
			{

            
            #line default
            #line hidden
            this.Write("\t\t\t");
            
            #line 121 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ToValidName(p.Name)));
            
            #line default
            #line hidden
            this.Write(" = ");
            
            #line 121 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ToValidName(p.Name)));
            
            #line default
            #line hidden
            this.Write("_ref;\r\n");
            
            #line 122 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"

			}
			foreach(var p in proc.Parameters.Where(IsOut))
			{

            
            #line default
            #line hidden
            this.Write("\t\t\t");
            
            #line 127 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ToValidName(p.Name)));
            
            #line default
            #line hidden
            this.Write(" = ");
            
            #line 127 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ToValidName(p.Name)));
            
            #line default
            #line hidden
            this.Write("_out;\r\n");
            
            #line 128 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"

			}

            
            #line default
            #line hidden
            this.Write("\t\t\treturn ___lresl___;\r\n\t\t}\r\n");
            
            #line 133 "D:\Work\Git\ObjectSql\src\ObjectSql.SqlServer\Schema\DatabaseSchemaTemplate.tt"

		}

            
            #line default
            #line hidden
            this.Write("\t}\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "11.0.0.0")]
    public class DatabaseSchemaTemplateBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
