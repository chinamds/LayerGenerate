using System;
using System.Data;

namespace LayerGen35.DatabasePlugins.VistaDB3x
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IDomains))]
#endif 
	public class VistaDBDomains : Domains
	{
		public VistaDBDomains()
		{

		}
	}
}
