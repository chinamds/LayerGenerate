using System;
using System.Data;

namespace LayerGen.DatabasePlugins.MySql5
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IDomain))]
#endif 
	public class MySql5Domain : Domain
	{
		public MySql5Domain()
		{

		}
	}
}
