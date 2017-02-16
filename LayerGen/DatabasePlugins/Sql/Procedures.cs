using System;
using System.Data;
using System.Data.OleDb;

namespace LayerGen.DatabasePlugins.Sql
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IProcedures))]
#endif 
	public class SqlProcedures : Procedures
	{
		public SqlProcedures()
		{

		}

		override internal void LoadAll()
		{
			try
			{
				DataTable metaData = this.LoadData(OleDbSchemaGuid.Procedures, 
					new Object[] {this.Database.Name, null, null});

				PopulateArray(metaData);

				LoadDescriptions();
			}
			catch {}
		}

		override public IProcedure this[object name]
		{
			get
			{
				return base[name];
			}
		}

		private void LoadDescriptions()
		{
			try
			{
				string select = @"SELECT objName, value FROM ::fn_listextendedproperty ('MS_Description', 'user', 'dbo', 'procedure', null, null, null)";

				OleDbConnection cn = new OleDbConnection(dbRoot.ConnectionString);
				cn.Open();
				cn.ChangeDatabase("[" + this.Database.Name + "]");

				OleDbDataAdapter adapter = new OleDbDataAdapter(select, cn);
				DataTable dataTable = new DataTable();

				adapter.Fill(dataTable);

				cn.Close();

				Procedure p;

				foreach(DataRow row in dataTable.Rows)
				{
					p = this[row["objName"] as string] as Procedure;

					if(null != p)
					{
						p._row["DESCRIPTION"] = row["value"] as string;
					}
				}
			}
			catch
			{
			
			}
		}
	}
}
