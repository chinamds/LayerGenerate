using System;
using System.Data;
using System.Collections;
using System.Data.OleDb;

namespace LayerGen.DatabasePlugins.Plugin
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IColumns))]
#endif 
	public class PluginColumns : Columns
	{
        private IMyMetaPlugin plugin;

		#region DataColumn Binding Stuff

		// Added for 3rd party providers
		internal DataColumn f_extTypeName	      = null;	
		internal DataColumn f_extTypeNameComplete = null;

		private void BindToColumns(DataTable metaData)
		{
			if(false == _fieldsBound)
			{
				if(metaData.Columns.Contains("TYPE_NAME"))
					f_extTypeName = metaData.Columns["TYPE_NAME"];

				if(metaData.Columns.Contains("TYPE_NAME_COMPLETE"))
					f_extTypeNameComplete = metaData.Columns["TYPE_NAME_COMPLETE"];
			}																		
		}
		#endregion

        public PluginColumns(IMyMetaPlugin plugin)
        {
            this.plugin = plugin;
        }

		override internal void LoadForTable()
        {
            DataTable metaData = this.plugin.GetTableColumns(this.Table.Database.Name, this.Table.Name);
			BindToColumns(metaData);
            PopulateArray(metaData);
	    }

		override internal void LoadForView()
        {
            DataTable metaData = this.plugin.GetViewColumns(this.View.Database.Name, this.View.Name);
			BindToColumns(metaData);
            PopulateArray(metaData);
		}
	}
}
