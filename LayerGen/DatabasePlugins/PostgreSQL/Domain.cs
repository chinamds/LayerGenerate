using System;
using System.Data;
using Npgsql;

namespace LayerGen.DatabasePlugins.PostgreSQL
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IDomain))]
#endif 
	public class PostgreSQLDomain : Domain
	{
		public PostgreSQLDomain()
		{

		}
	}
}
