using System;
using System.Data;
using FirebirdSql.Data.FirebirdClient;

namespace LayerGen.DatabasePlugins.Firebird
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IForeignKey))]
#endif 
	public class FirebirdForeignKey : ForeignKey
	{
		public FirebirdForeignKey()
		{

		}

		override public ITable ForeignTable
		{
			get
			{
				string catalog = this.ForeignKeys.Table.Database.Name;
				string schema  = this.GetString(ForeignKeys.f_FKTableSchema);

				return this.dbRoot.Databases[0].Tables[this.GetString(ForeignKeys.f_FKTableName)];
			}
		}
	}
}
