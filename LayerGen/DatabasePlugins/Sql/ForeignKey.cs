using System;
using System.Data;
using System.Data.OleDb;

namespace LayerGen.DatabasePlugins.Sql
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IForeignKey))]
#endif 
	public class SqlForeignKey : ForeignKey
	{
		public SqlForeignKey()
		{

		}
	}
}
