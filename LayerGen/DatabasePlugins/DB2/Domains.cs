using System;
using System.Data;
using System.Data.OleDb;

namespace LayerGen.DatabasePlugins.DB2
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IDomains))]
#endif 
	public class DB2Domains : Domains
	{
		public DB2Domains()
		{

		}
	}
}
