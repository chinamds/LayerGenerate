using System;
using System.Data;
using System.Data.OleDb;

namespace LayerGen.DatabasePlugins.ISeries
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IParameter))]
#endif 
	public class ISeriesParameter : Parameter
	{
		public ISeriesParameter()
		{

		}
	}
}
