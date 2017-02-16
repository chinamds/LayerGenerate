using System;
using System.Xml;
using System.Data;
using System.Data.OleDb;

namespace LayerGen35.DatabasePlugins
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(false), ClassInterface(ClassInterfaceType.AutoDual)]
#endif 
	public class Index : Single, IIndex
	{
		public Index()
		{
			
		}

		

		#region Properties

#if ENTERPRISE
		[DispId(0)]
#endif		
		override public string Alias
		{
			get
			{
				

				// There was no nice name
				return this.Name;
			}

			set
			{
				
			}
		}

		override public string Name
		{
			get
			{
				return this.GetString(Indexes.f_IndexName);
			}
		}

		virtual public string Schema
		{
			get
			{
				return this.GetString(Indexes.f_IndexSchema);
			}
		}

		virtual public System.Boolean Unique
		{
			get
			{
				return this.GetBool(Indexes.f_Unique);
			}
		}

		virtual public System.Boolean Clustered
		{
			get
			{
				return this.GetBool(Indexes.f_Clustered);
			}
		}

		virtual public string Type
		{
			get
			{
				System.Int32 i = this.GetInt32(Indexes.f_Type);

				switch(i)
				{
					case 1:
						return "BTREE";
					case 2:
						return "HASH";
					case 3:
						return "CONTENT";
					case 4:
						return "OTHER";
					default:
						return "OTHER";
				}
			}
		}

		virtual public System.Int32 FillFactor
		{
			get
			{
				return this.GetInt32(Indexes.f_FillFactor);
			}
		}

		virtual public System.Int32 InitialSize
		{
			get
			{
				return this.GetInt32(Indexes.f_InitializeSize);
			}
		}

		virtual public System.Boolean SortBookmarks
		{
			get
			{
				return this.GetBool(Indexes.f_SortBookmarks);
			}
		}

		virtual public System.Boolean AutoUpdate
		{
			get
			{
				return this.GetBool(Indexes.f_AutoUpdate);
			}
		}

		virtual public string NullCollation
		{
			get
			{
				System.Int32 i = this.GetInt32(Indexes.f_NullCollation);

				switch(i)
				{
					case 1:
						return "END";
					case 2:
						return "HIGH";
					case 4:
						return "LOW";
					case 8:
						return "START";
					default:
						return "UNKNOWN";
				}
			}
		}

		virtual public string Collation
		{
			get
			{
				System.Int32 i = this.GetInt16(Indexes.f_Collation);

				switch(i)
				{
					case 1:
						return "ASCENDING";
					case 2:
						return "DECENDING";
					default:
						return "UNKNOWN";
				}
			}
		}

		virtual public Decimal Cardinality
		{
			get
			{
				return this.GetDecimal(Indexes.f_Cardinality);
			}
		}

		virtual public System.Int32 Pages
		{
			get
			{
				return this.GetInt32(Indexes.f_Pages);
			}
		}

		virtual public string FilterCondition
		{
			get
			{
				return this.GetString(Indexes.f_FilterCondition);
			}
		}

		virtual public System.Boolean Integrated
		{
			get
			{
				return this.GetBool(Indexes.f_Integrated);
			}
		}
		#endregion

		

		#region INameValueCollection Members

		public string ItemName
		{
			get
			{
				return this.Name;
			}
		}

		public string ItemValue
		{
			get
			{
				return this.Name;
			}
		}

		#endregion

		internal Indexes Indexes = null;
	}
}
