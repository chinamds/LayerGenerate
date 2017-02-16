using System;
using System.Data;

namespace LayerGen35.DatabasePlugins.VistaDB
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IProcedure))]
#endif 
	public class VistaDBProcedure : Procedure
	{
		public VistaDBProcedure()
		{

		}
	}
}
