using System;
using System.Data;
using Npgsql;

namespace LayerGen.DatabasePlugins.PostgreSQL
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IDomains))]
#endif 
	public class PostgreSQLDomains : Domains
	{
		public PostgreSQLDomains()
		{

		}
	}
}
