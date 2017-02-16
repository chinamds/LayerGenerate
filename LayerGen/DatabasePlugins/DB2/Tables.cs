using System;
using System.Data;
using System.Data.OleDb;

namespace LayerGen.DatabasePlugins.DB2
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(ITables))]
#endif 
	public class DB2Tables : Tables
	{
		public DB2Tables()
		{

		}

		override internal void LoadAll()
		{
			try
			{
				string type = this.dbRoot.ShowSystemData ? "SYSTEM TABLE" : "TABLE";
				DataTable metaData = this.LoadData(OleDbSchemaGuid.Tables, new Object[] {null, null, null, type});

				PopulateArray(metaData);
			}
			catch {}
		}
	}
}
