using System;
using System.Data;
using System.Data.OleDb;

namespace LayerGen35.DatabasePlugins.Access
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IColumn))]
#endif 
	public class AccessColumn : Column
	{
		public AccessColumn()
		{

		}

		override internal Column Clone()
		{
			Column c = base.Clone();

			return c;
		}

		override public System.Boolean IsInPrimaryKey
		{
			get
			{
				if(null == this.Columns.Table) return false;

				IColumn col = this.Columns.Table.PrimaryKeys[this.Name];

				return (null == col) ? false : true;
			}
		}

		override public System.Boolean IsAutoKey
		{
			get
			{
				AccessColumns cols = Columns as AccessColumns;
				return this.GetBool(cols.f_AutoKey);
			}
		}

		public override Int32 AutoKeyIncrement
		{
			get
			{
				AccessColumns cols = Columns as AccessColumns;
				return this.GetInt32(cols.f_AutoKeyIncrement);
			}
		}

		public override Int32 AutoKeySeed
		{
			get
			{
				AccessColumns cols = Columns as AccessColumns;
				return this.GetInt32(cols.f_AutoKeySeed);
			}
		}

		override public string DataTypeName
		{
			get
			{
				AccessColumns cols = Columns as AccessColumns;
				string type = this.GetString(cols.f_TypeName);

				switch(type)
				{
					case "adWChar":
					case "adVarWChar":
						return "Text";
					case "adLongVarWChar":
						return "Memo";
					case "adUnsignedTinyInt":
						return "Byte";
					case "adCurrency":
						return "Currency";
					case "adDate":
						return "DateTime";
					case "adBoolean":
						return @"Yes/No";
					case "adLongVarBinary":
						return "OLE Object";
					case "adInteger":
						return "Long";
					case "adDouble":
						return "Double";
					case "adGUID":
						return "Replication ID";
					case "adSingle":
						return "Single";
					case "adNumeric":
						return "Decimal";
					case "adSmallInt":
						return "Integer";
					case "adVarBinary":
						return "Binary";
					case "Hyperlink":
						return "Hyperlink";
					default:
						return type;
				}
			}
		}

		override public string DataTypeNameComplete
		{
			get
			{
				AccessColumns cols = Columns as AccessColumns;
				string type = this.GetString(cols.f_TypeName);

				switch(type)
				{
					case "adWChar":
					case "adVarWChar":
						return "Text";
					case "adLongVarWChar":
						return "Memo";
					case "adUnsignedTinyInt":
						return "Byte";
					case "adCurrency":
						return "Currency";
					case "adDate":
						return "DateTime";
					case "adBoolean":
						//return @"Yes/No";
						return "Bit";
					case "adLongVarBinary":
						//return "OLE Object";
						return "LongBinary";
					case "adInteger":
						return "Long";
					case "adDouble":
						return "IEEEDouble";
					case "adGUID":
						//return "Replication ID";
						return "Guid";
					case "adSingle":
						return "IEEESingle";
					case "adNumeric":
						return "Decimal";
					case "adSmallInt":
						return "Integer";
					case "adVarBinary":
						return "Binary";
					case "Hyperlink":
						return "Text (255)";
					default:
						return type;
				}
			}
		}
	}
}
