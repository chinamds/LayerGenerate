using System;
using System.Data.SQLite;

namespace LayerGen.DatabasePlugins.SQLite
{
	/// <summary>
	/// Summary description for ConnectionHelper.
	/// </summary>
	public class ConnectionHelper
	{
		public ConnectionHelper()
		{

		}
//
        static public SQLiteConnection CreateConnection(LayerGen.DatabasePlugins.dbRoot dbRoot)
		{
			SQLiteConnection cn = new SQLiteConnection(dbRoot.ConnectionString);
			cn.Open();
			//cn.ChangeDatabase(database);
			return cn;
		}
	}
}
