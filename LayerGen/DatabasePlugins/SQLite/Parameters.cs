using System;
using System.Data;
using System.Data.OleDb;

namespace LayerGen.DatabasePlugins.SQLite
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IParameters))]
#endif 
	public class SQLiteParameters : Parameters
	{
		public SQLiteParameters()
		{

		}
	}
}
