using System;
using System.Data;
using System.Data.OleDb;

namespace LayerGen.DatabasePlugins.Advantage
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IViews))]
#endif 
	public class AdvantageViews : Views
	{
		public AdvantageViews()
		{

		}

		override internal void LoadAll()
		{
			try
			{
				string type = this.dbRoot.ShowSystemData ? "SYSTEM VIEW" : "VIEW";
				DataTable metaData = this.LoadData(OleDbSchemaGuid.Views, new Object[] {null});

				PopulateArray(metaData);

				LoadDescriptions();
			}
			catch {}
		}

		private void LoadDescriptions()
		{
			try
			{
				string select = @"SELECT objName, value FROM ::fn_listextendedproperty ('MS_Description', 'user', 'dbo', 'view', null, null, null)";

				OleDbConnection cn = new OleDbConnection(dbRoot.ConnectionString);
				cn.Open();
				cn.ChangeDatabase("[" + this.Database.Name + "]");

				OleDbDataAdapter adapter = new OleDbDataAdapter(select, cn);
				DataTable dataTable = new DataTable();

				adapter.Fill(dataTable);

				cn.Close();

				View v;

				foreach(DataRow row in dataTable.Rows)
				{
					v = this[row["objName"] as string] as View;

					if(null != v)
					{
						v._row["DESCRIPTION"] = row["value"] as string;
					}
				}
			}
			catch
			{
			
			}
		}
	}
}
