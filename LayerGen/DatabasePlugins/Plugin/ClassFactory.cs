using System;

using LayerGen.DatabasePlugins;

namespace LayerGen.DatabasePlugins.Plugin
{
#if ENTERPRISE
	using System.EnterpriseServices;
	using System.Runtime.InteropServices;
	[ComVisible(false)]
#endif
	public class ClassFactory : IClassFactory
	{
        private IMyMetaPlugin plugin;

		public ClassFactory(IMyMetaPlugin plugin)
		{
            this.plugin = plugin;
		}

		public ITables CreateTables()
		{
            return new Plugin.PluginTables(plugin);
		}

		public ITable CreateTable()
		{
            return new Plugin.PluginTable(plugin);
		}

		public IColumn CreateColumn()
		{
            return new Plugin.PluginColumn(plugin);
		}

		public IColumns CreateColumns()
		{
            return new Plugin.PluginColumns(plugin);
		}

		public IDatabase CreateDatabase()
		{
            return new Plugin.PluginDatabase(plugin);
		}

		public IDatabases CreateDatabases()
		{
            return new Plugin.PluginDatabases(plugin);
		}

		public IProcedure CreateProcedure()
		{
            return new Plugin.PluginProcedure(plugin);
		}

		public IProcedures CreateProcedures()
		{
            return new Plugin.PluginProcedures(plugin);
		}

		public IView CreateView()
		{
            return new Plugin.PluginView(plugin);
		}

		public IViews CreateViews()
		{
            return new Plugin.PluginViews(plugin);
		}

		public IParameter CreateParameter()
		{
            return new Plugin.PluginParameter(plugin);
		}

		public IParameters CreateParameters()
		{
            return new Plugin.PluginParameters(plugin);
		}

		public IForeignKey  CreateForeignKey()
		{
            return new Plugin.PluginForeignKey(plugin);
		}

		public IForeignKeys CreateForeignKeys()
		{
            return new Plugin.PluginForeignKeys(plugin);
		}

		public IIndex CreateIndex()
		{
            return new Plugin.PluginIndex(plugin);
		}

		public IIndexes CreateIndexes()
		{
            return new Plugin.PluginIndexes(plugin);
		}

		public IResultColumn CreateResultColumn()
		{
            return new Plugin.PluginResultColumn(plugin);
		}

		public IResultColumns CreateResultColumns()
		{
            return new Plugin.PluginResultColumns(plugin);
		}

		public IDomain CreateDomain()
		{
            return new Plugin.PluginDomain(plugin);
		}

		public IDomains CreateDomains()
		{
            return new Plugin.PluginDomains(plugin);
		}

        public IProviderType CreateProviderType()
        {
            /*if (plugin is IMyMetaPluginExt)
            {
            }
            else
            {*/
                return new ProviderType();
            //}
        }

		public IProviderTypes CreateProviderTypes()
		{
            /*if (plugin is IMyMetaPluginExt)
            {
            }
            else
            {*/
                return new ProviderTypes();
           // }
		}

        public System.Data.IDbConnection CreateConnection()
        {
            return plugin.NewConnection;
        }

        public void ChangeDatabase(System.Data.IDbConnection connection, string database)
        {
            if (plugin is IMyMetaPluginExt)
            {
                IMyMetaPluginExt pluginext = plugin as IMyMetaPluginExt;
                pluginext.ChangeDatabase(connection, database);
            }
            else
            {
                connection.ChangeDatabase(database);
            }
        }
	}
}
