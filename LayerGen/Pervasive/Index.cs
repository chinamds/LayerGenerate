using System;
using System.Data;
using System.Data.OleDb;

namespace LayerGen35.DatabasePlugins.Pervasive
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IIndex))]
#endif 
	public class PervasiveIndex : Index
	{
		public PervasiveIndex()
		{

		}
	}
}