using System;
using System.Data;
using System.Data.OleDb;

namespace LayerGen.DatabasePlugins.ISeries
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IView))]
#endif 
	public class ISeriesView : View
	{
		public ISeriesView()
		{

		}
	}
}
