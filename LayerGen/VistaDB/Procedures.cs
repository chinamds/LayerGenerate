using System;
using System.Data;

namespace LayerGen35.DatabasePlugins.VistaDB
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IProcedures))]
#endif 
	public class VistaDBProcedures : Procedures
	{
		public VistaDBProcedures()
		{

		}

		override internal void LoadAll()
		{
			try
			{
//				DataTable metaData = this.LoadData(OleDbSchemaGuid.Procedures, null);
//
//				PopulateArray(metaData);
			}
			catch {}
		}
	}
}
