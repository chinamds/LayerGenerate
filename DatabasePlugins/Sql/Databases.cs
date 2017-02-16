using System;
using System.Data;
using System.Data.OleDb;

namespace LayerGen35.DatabasePlugins.Sql
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IDatabases))]
#endif 
	public class SqlDatabases : Databases
	{
		public SqlDatabases()
		{

		}

		override internal void LoadAll()
		{
			//DataTable metaData  = this.LoadData(OleDbSchemaGuid.Catalogs, null);
            DataTable metaData = this.LoadDataSqlClient("Databases", null);
		
			PopulateArray(metaData);
		}
	}
}
