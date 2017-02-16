using System;
using System.Data;

namespace LayerGen.DatabasePlugins
{
	/// <summary>
	/// Summary description for Utility.
	/// </summary>
	public class Utility
	{
		public Utility()
		{

		}

		static public string GetDatabaseName(string catalog, string schema)
		{
			return (catalog != string.Empty) ? catalog : schema;
		}
	}
}
