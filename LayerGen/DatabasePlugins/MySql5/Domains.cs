using System;
using System.Data;

namespace LayerGen.DatabasePlugins.MySql5
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IDomains))]
#endif 
	public class MySql5Domains : Domains
	{
		public MySql5Domains()
		{

		}
	}
}
