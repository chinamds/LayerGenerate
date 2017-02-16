using System;
using System.Data;
using System.Data.OleDb;

namespace LayerGen.DatabasePlugins.Oracle
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IResultColumn))]
#endif 
	public class OracleResultColumn : ResultColumn
	{
		public OracleResultColumn()
		{

		}
	}
}
