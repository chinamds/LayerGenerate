using System;
using System.Data;
using System.Data.OleDb;

namespace LayerGen.DatabasePlugins.Plugin
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(ITable))]
#endif 
    public class PluginTable : Table
    {
        private IMyMetaPlugin plugin;

        public PluginTable(IMyMetaPlugin plugin)
        {
            this.plugin = plugin;
        }
    
		public override IColumns PrimaryKeys
		{
			get
			{
				if(null == _primaryKeys)
				{
                    System.Collections.Generic.List<string> metaData = this.plugin.GetPrimaryKeyColumns(this.Database.Name, this.Name);

					_primaryKeys = (Columns)this.dbRoot.ClassFactory.CreateColumns();
					_primaryKeys.Table = this;
					_primaryKeys.dbRoot = this.dbRoot;

					string colName = "";

					int count = metaData.Count;
					for(int i = 0; i < count; i++)
					{
						colName = metaData[i];
                        Column col = this.Columns[colName] as Column;

                        if (col != null) _primaryKeys.AddColumn(col);
					}
				}

				return _primaryKeys;
			}
        }

        public override object DatabaseSpecificMetaData(string key)
        {
            return this.plugin.GetDatabaseSpecificMetaData(this, key);
        }
    }
}
