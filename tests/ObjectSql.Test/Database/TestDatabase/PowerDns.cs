﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from database.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using ObjectSql;
using ObjectSql.QueryInterfaces;

namespace TestDatabase.MySql
{
	[Table("__migratorVariables")]
	public partial class __migratorVariables
	{
		[Column("Name", TypeName = "varchar")] public string Name { get; set; }
		[Column("Value", TypeName = "varchar")] public string Value { get; set; }
	}

	[Table("__versionInfo")]
	public partial class __versionInfo
	{
		[Column("Version", TypeName = "bigint")] public long Version { get; set; }
		[Column("AppliedOn", TypeName = "datetime")] public DateTime AppliedOn { get; set; }
		[Column("Description", TypeName = "varchar")] public string Description { get; set; }
	}

	[Table("comments")]
	public partial class comments
	{
		[Column("id", TypeName = "int")] public int id { get; set; }
		[Column("domain_id", TypeName = "int")] public int domain_id { get; set; }
		[Column("name", TypeName = "varchar")] public string name { get; set; }
		[Column("type", TypeName = "varchar")] public string type { get; set; }
		[Column("modified_at", TypeName = "int")] public int modified_at { get; set; }
		[Column("account", TypeName = "varchar")] public string account { get; set; }
		[Column("comment", TypeName = "varchar")] public string comment { get; set; }
	}

	[Table("cryptokeys")]
	public partial class cryptokeys
	{
		[Column("id", TypeName = "int")] public int id { get; set; }
		[Column("domain_id", TypeName = "int")] public int domain_id { get; set; }
		[Column("flags", TypeName = "int")] public int flags { get; set; }
		[Column("active", TypeName = "tinyint")] public bool? active { get; set; }
		[Column("content", TypeName = "text")] public string content { get; set; }
	}

	[Table("domainmetadata")]
	public partial class domainmetadata
	{
		[Column("id", TypeName = "int")] public int id { get; set; }
		[Column("domain_id", TypeName = "int")] public int domain_id { get; set; }
		[Column("kind", TypeName = "varchar")] public string kind { get; set; }
		[Column("content", TypeName = "text")] public string content { get; set; }
	}

	[Table("domains")]
	public partial class domains
	{
		[Column("id", TypeName = "int")] public int id { get; set; }
		[Column("name", TypeName = "varchar")] public string name { get; set; }
		[Column("master", TypeName = "varchar")] public string master { get; set; }
		[Column("last_check", TypeName = "int")] public int? last_check { get; set; }
		[Column("type", TypeName = "varchar")] public string type { get; set; }
		[Column("notified_serial", TypeName = "int")] public int? notified_serial { get; set; }
		[Column("account", TypeName = "varchar")] public string account { get; set; }
	}

	[Table("records")]
	public partial class records
	{
		[Column("id", TypeName = "int")] public int id { get; set; }
		[Column("domain_id", TypeName = "int")] public int? domain_id { get; set; }
		[Column("name", TypeName = "varchar")] public string name { get; set; }
		[Column("type", TypeName = "varchar")] public string type { get; set; }
		[Column("content", TypeName = "varchar")] public string content { get; set; }
		[Column("ttl", TypeName = "int")] public int? ttl { get; set; }
		[Column("prio", TypeName = "int")] public int? prio { get; set; }
		[Column("change_date", TypeName = "int")] public int? change_date { get; set; }
		[Column("disabled", TypeName = "tinyint")] public bool? disabled { get; set; }
		[Column("ordername", TypeName = "varchar")] public string ordername { get; set; }
		[Column("auth", TypeName = "tinyint")] public bool? auth { get; set; }
		[Column("fingerPrint", TypeName = "char")] public string fingerPrint { get; set; }
	}

	[Table("supermasters")]
	public partial class supermasters
	{
		[Column("ip", TypeName = "varchar")] public string ip { get; set; }
		[Column("nameserver", TypeName = "varchar")] public string nameserver { get; set; }
		[Column("account", TypeName = "varchar")] public string account { get; set; }
	}

	[Table("tsigkeys")]
	public partial class tsigkeys
	{
		[Column("id", TypeName = "int")] public int id { get; set; }
		[Column("name", TypeName = "varchar")] public string name { get; set; }
		[Column("algorithm", TypeName = "varchar")] public string algorithm { get; set; }
		[Column("secret", TypeName = "varchar")] public string secret { get; set; }
	}


	public abstract class CsNameProcedures
	{
		[Procedure("__throwIfVersionExists")] public abstract void __throwIfVersionExists([Parameter("ver", "bigint", ParameterDirection.Input)] long? ver);
	}
}