using System;
using System.Data;
using System.Reflection;

namespace LayerGen35.DatabasePlugins.VistaDB3x
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(ITable))]
#endif 
	public class VistaDBTable : Table
	{
		public VistaDBTable()
		{

		}

		public override IColumns PrimaryKeys
		{
			get
			{
				if(null == _primaryKeys)
				{
					_primaryKeys = (Columns)this.dbRoot.ClassFactory.CreateColumns();
					_primaryKeys.Table = this;
					_primaryKeys.dbRoot = this.dbRoot;

					foreach(IColumn col in this.Columns)
					{
						if(col.IsInPrimaryKey)
						{
							_primaryKeys.AddColumn((Column)this.Columns[col.Name]);
						}
					}
				}
				return _primaryKeys;
			}
		}
	}
}
