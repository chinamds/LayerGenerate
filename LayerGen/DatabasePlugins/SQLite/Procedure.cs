using System;
using System.Data;
using System.Data.OleDb;

namespace LayerGen.DatabasePlugins.SQLite
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IProcedure))]
#endif 
	public class SQLiteProcedure : Procedure
	{
		public SQLiteProcedure()
		{

		}
	}
}
