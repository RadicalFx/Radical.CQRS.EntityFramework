﻿using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Data.Entity.Migrations.Model;

namespace System.Data.Entity.Migrations
{
	public static class CreateViewOperationExtensions
	{
		public static void CreateView( this DbMigration migration, string viewName, string viewqueryString )
		{
			( ( IDbMigration )migration ).AddOperation( new CreateViewOperation( viewName, viewqueryString ) );
		}		
	}

	public class CreateViewOperation : MigrationOperation
	{
		public CreateViewOperation( string viewName, string viewQueryString )
			: base( null )
		{
			ViewName = viewName;
			ViewString = viewQueryString;
		}
		public string ViewName { get; private set; }
		public string ViewString { get; private set; }
		public override bool IsDestructiveChange
		{
			get { return false; }
		}
	}
}
