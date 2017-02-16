using System;
using System.Data;

namespace LayerGen35.DatabasePlugins.VistaDB
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IIndex))]
#endif 
	public class VistaDBIndex : Index
	{
		public VistaDBIndex()
		{

		}
	}
}
