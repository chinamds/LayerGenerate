using System;
using System.Xml;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace LayerGen35.DatabasePlugins
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(false), ClassInterface(ClassInterfaceType.AutoDual)]
#endif
	public class MetaObject
	{
		public MetaObject()
		{

		}

		virtual protected DataTable LoadData(Guid oleDbSchemaGuid, object[] filter)
		{
			try
			{
				OleDbConnection cn = dbRoot.TheConnection;
				DataTable metaData = cn.GetOleDbSchemaTable(oleDbSchemaGuid, filter);
				return metaData;
			}
			catch(Exception ex)
			{
				string s = ex.Message;
				return new DataTable();
			}
		}

        virtual protected DataTable LoadDataSqlClient(string collectionName, string[] filter)
        {
            try
            {
                SqlConnection cn = dbRoot.TheSqlConnection;
                DataTable metaData = cn.GetSchema(collectionName, filter);
                return metaData;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return new DataTable();
            }
        }

		#region XML User Data

#if ENTERPRISE
		[ComVisible(false)]
#endif
		virtual internal bool GetXmlNode(out XmlNode node, bool forceCreate)
		{
			node = null;
			return false;
		}

#if ENTERPRISE
		[ComVisible(false)]
#endif
		virtual public string UserDataXPath
		{
			get
			{
				return string.Empty;
			}
		}

#if ENTERPRISE
		[ComVisible(false)]
#endif
		virtual public string GlobalUserDataXPath
		{
			get
			{
				return string.Empty;
			}
		}

#if ENTERPRISE
		[ComVisible(false)]
#endif
		virtual public void CreateUserMetaData(XmlNode parentNode)
		{

		}

#if ENTERPRISE
		[ComVisible(false)]
#endif
		virtual public void CreateUserMetaData()
		{

		}

		protected bool GetUserData(string xPath, XmlNode startingNode, out XmlNodeList data)
		{
			data = null;
			bool success = false;

			XmlNodeList nodeList = null;
		
			if(null == startingNode)
			{
				nodeList = dbRoot.UserData.SelectNodes(xPath, null);
			}
			else
			{
				nodeList = startingNode.SelectNodes(xPath, null);
			}

			if(null != nodeList)
			{
				data = nodeList;
				success = true;
			}

			return success;
		}

		protected bool GetUserData(string xPath, XmlNode startingNode, out XmlNode data)
		{
			data = null;
			bool success = false;

			XmlNode node = null;

			if(null == startingNode)
			{
				node = dbRoot.UserData.SelectSingleNode(xPath, null);
			}
			else
			{
				node = startingNode.SelectSingleNode(xPath, null);
			}

			if(null != node)
			{
				data = node;
				success = true;
			}

			return success;
		}

		protected bool GetUserData(string xPath, XmlNode startingNode, string attribute, out string data)
		{
			data = string.Empty;
			bool success = false;

			XmlNode node = null;

			if(null == startingNode)
			{
				node = dbRoot.UserData.SelectSingleNode(xPath, null);
			}
			else
			{
				node = startingNode.SelectSingleNode(xPath, null);
			}

			if(null != node)
			{
				XmlAttributeCollection coll = node.Attributes;
				if(null != coll)
				{
					if(null != coll[attribute])
					{
						data = coll[attribute].Value;
						success = true;
					}
				}
			}

			return success;
		}

		protected bool GetUserData(XmlNode startingNode, string attribute, out string data)
		{
			data = string.Empty;
			bool success = false;

			if(null != startingNode)
			{
				XmlAttributeCollection coll = startingNode.Attributes;
				if(null != coll)
				{
					if(null != coll[attribute])
					{
						data = coll[attribute].Value;
						success = true;
					}
				}
			}

			return success;
		}
		
		protected void SetUserData(XmlNode node, string attribute, string data)
		{
			if(null != node)
			{
				node.Attributes[attribute].Value = data;
			}
		}


		#endregion

		internal dbRoot dbRoot
		{
			get
			{
				return _dbRoot;
			}

			set
			{
				_dbRoot = value;
			}
		}

		internal  dbRoot _dbRoot = null;
		protected XmlNode _xmlNode = null;
	}
}
