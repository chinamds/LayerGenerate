using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Runtime.InteropServices;

namespace LayerGen.DatabasePlugins
{
	[ComVisible(false)]
    public interface IMyMetaPluginExt : IMyMetaPlugin
    {
        void ChangeDatabase(IDbConnection connection, string database);

        DataTable GetProviderTypes(string database);
    }
}