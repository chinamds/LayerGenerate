using System;
using System.Data;
using System.Reflection;


namespace LayerGen35.DatabasePlugins.VistaDB3x
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(ITables))]
#endif 
	public class VistaDBTables : Tables
	{
		public VistaDBTables()
		{

		}

		override internal void LoadAll()
		{
			try
			{
				VistaDBDatabase db = (VistaDBDatabase)this.Database;

				DataTable metaData = db._mh.LoadTables(this.dbRoot.ConnectionString);

				PopulateArray(metaData);
			}
			catch {}
		}
	}
}
