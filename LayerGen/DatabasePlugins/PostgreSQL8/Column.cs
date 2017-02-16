using System;
using System.Data;
using System.Data.OleDb;

namespace LayerGen.DatabasePlugins.PostgreSQL8
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IColumn))]
#endif 
	public class PostgreSQL8Column : Column
	{
		internal bool _isAutoKey = false;
		internal int _autoInc   = 0;
		internal int _autoSeed  = 0;

		public PostgreSQL8Column()
		{

		}

		override internal Column Clone()
		{
			Column c = base.Clone();

			return c;
		}

		override public System.Boolean IsNullable
		{
			get
			{
				string s = this.GetString(Columns.f_IsNullable);

				if(s == "YES") 
					return true;
				else
					return false;
			}
		}

		override public System.Boolean HasDefault
		{
			get
			{
				object o = this._row[Columns.f_Default];

				if(o == DBNull.Value)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}


		public override System.Boolean IsAutoKey
		{
			get
			{
				return this._isAutoKey;
			}
		}

		public override Int32 AutoKeyIncrement
		{
			get
			{
				return this._autoInc;
			}
		}

		public override Int32 AutoKeySeed
		{
			get
			{
				return this._autoSeed;
			}
		}

		override public System.Boolean IsComputed
		{
			get
			{
				return this.GetBool(Columns.f_IsComputed);
			}
		}


		override public string DataTypeName
		{
			get
			{
				PostgreSQL8Columns cols = Columns as PostgreSQL8Columns;
				return this.GetString(cols.f_TypeName);
			}
		}

		override public string DataTypeNameComplete
		{
			get
			{
				PostgreSQL8Columns cols = Columns as PostgreSQL8Columns;
				return this.GetString(cols.f_TypeNameComplete);
			}
		}
	}
}
