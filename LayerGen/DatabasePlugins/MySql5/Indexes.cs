using System;
using System.Data;
using System.Data.Common;

namespace LayerGen.DatabasePlugins.MySql5
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IIndexes))]
#endif 
	public class MySql5Indexes : Indexes
	{
		public MySql5Indexes()
		{

		}

		override internal void LoadAll()
		{
			try
			{
				string query = @"SHOW INDEX FROM `" + this.Table.Name + "`";

				DataTable metaData = new DataTable();
				DbDataAdapter adapter = MySql5Databases.CreateAdapter(query, this.dbRoot.ConnectionString);

				adapter.Fill(metaData);

				metaData.Columns["Key_name"].ColumnName		= "INDEX_NAME";
				metaData.Columns["Index_type"].ColumnName	= "TYPE";
				metaData.Columns["Non_unique"].ColumnName   = "UNIQUE";
			
				PopulateArray(metaData);


			}
			catch {}
		}
	}
}
