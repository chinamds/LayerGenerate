using System;
using System.Data;
using System.Data.OleDb;

namespace LayerGen35.DatabasePlugins.Access
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IView))]
#endif 
	public class AccessView : View
	{
		public AccessView()
		{

		}
	}
}
