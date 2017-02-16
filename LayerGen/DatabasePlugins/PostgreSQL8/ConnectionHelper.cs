using System;
using Npgsql;

namespace LayerGen.DatabasePlugins.PostgreSQL8
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
//		static public NpgsqlConnection CreateConnection(MyMeta.dbRoot dbRoot, string database)
//		{
//			string cnstr = dbRoot.ConnectionString + "Database=" + database + ";";
//			NpgsqlConnection cn = new Npgsql.NpgsqlConnection(cnstr);
//			return cn;
//		}

        static public NpgsqlConnection CreateConnection(LayerGen.DatabasePlugins.dbRoot dbRoot, string database)
		{
			NpgsqlConnection cn = new Npgsql.NpgsqlConnection(dbRoot.ConnectionString);
			cn.Open();
			cn.ChangeDatabase(database);
			return cn;
		}
	}
}
