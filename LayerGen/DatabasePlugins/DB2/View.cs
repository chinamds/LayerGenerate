using System;
using System.Data;
using System.Data.OleDb;

namespace LayerGen.DatabasePlugins.DB2
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IView))]
#endif 
	public class DB2View : View
	{
		public DB2View()
		{

		}
	}
}