using System;
using System.Data;
using System.Data.OleDb;

namespace LayerGen.DatabasePlugins.Oracle
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IProcedure))]
#endif 
	public class OracleProcedure : Procedure
	{
		public OracleProcedure()
		{

		}
	}
}
