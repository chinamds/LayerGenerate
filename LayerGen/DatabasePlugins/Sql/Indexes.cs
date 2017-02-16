using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace LayerGen.DatabasePlugins.Sql
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IIndexes))]
#endif 
	public class SqlIndexes : Indexes
	{
		public SqlIndexes()
		{

		}

        public SqlIndexes(dbRoot db)
        {
            dbRoot = db;
        }

		override internal void LoadAll()
		{
			try
			{
                //DataTable metaData = this.LoadData(OleDbSchemaGuid.Indexes, 
                //    new object[]{this.Table.Database.Name, this.Table.Schema, null, null, this.Table.Name});
                DataTable metaData = this.LoadDataSqlClient("Indexes",
                    new string[] { this.Table.Database.Name, this.Table.Schema, null, null, this.Table.Name });

				PopulateArray(metaData);
			}
			catch {}
		}

        override public bool LoadIndexes(string dbName, string schema, string table)
        {
            try
            {
                //DataTable metaData = this.LoadData(OleDbSchemaGuid.Indexes, 
                //    new object[]{this.Table.Database.Name, this.Table.Schema, null, null, this.Table.Name});
                //String[] indexesRestrictions = new String[3];
                //indexesRestrictions[0] = dbName;
                //indexesRestrictions[1] = schema;
                //indexesRestrictions[2] = table;
                ////indexColumnsRestrictions[4] = "CourseID";

                //DataTable metaData = this.LoadDataSqlClient("Indexes", indexesRestrictions);

                //PopulateArray(metaData);
                using (var command = new SqlCommand())
                {
                    command.Connection = dbRoot.TheSqlConnection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT table_catalog='"+dbName+"', table_schema='"+schema+"', TABLE_NAME = t.name, INDEX_NAME = ind.name,[UNIQUE] = ind.is_unique,[TYPE] = ind.[type],";
                    command.CommandText += "     FILL_FACTOR = ind.fill_factor ";
                    command.CommandText += "FROM ";
                    command.CommandText += "     sys.indexes ind ";
                    command.CommandText += "INNER JOIN ";
                    command.CommandText += "     sys.tables t ON ind.object_id = t.object_id ";
                    command.CommandText += "WHERE ";
                    command.CommandText += "     ind.is_primary_key = 0 ";
                    //command.CommandText += "     AND ind.is_unique_constraint = 0 ";
                    command.CommandText += "     AND t.is_ms_shipped = 0 ";
                    command.CommandText += "     AND t.name = '" + table + "' ";
                    command.CommandText += "ORDER BY t.name, ind.name, ind.index_id";

                    using (var adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = command;
                        using (var ds = new DataSet())
                        {
                            adapter.Fill(ds);
                            PopulateArray(ds.Tables[0]);
                        }
                    }
                }

                return true;
            }
            catch { }

            return false;
        }
	}
}
