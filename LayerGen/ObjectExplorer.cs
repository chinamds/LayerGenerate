using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace LayerGen
{
    public partial class ObjectExplorer : Form
    {
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public bool TrustedConnection { get; set; }
        public string DatabaseName { get; set; }
        public string SelectedObjects { get; set; }
        public string DefaultSchema { get; set; }
        public string CustomConnectionString { get; set; }
        public bool HasCustomConnectionString { get; set; }

        private string ConnectionString
        {
            get
            {
                if (HasCustomConnectionString)
                    return CustomConnectionString;

                var builder = new SqlConnectionStringBuilder();
                builder["Data Source"] = ServerName + "," + Port;
                builder["Integrated Security"] = TrustedConnection;
                builder["Initial Catalog"] = DatabaseName;
                if (!TrustedConnection)
                {
                    builder["User ID"] = UserName;
                    builder["Password"] = Password;
                }

                return builder.ConnectionString;
            }
        }

        public ObjectExplorer()
        {
            InitializeComponent();
        }

        private void ObjectExplorer_Shown(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedObjects))
                return;

            for (int i = 0; i < clbTables.Items.Count; i++)
            {
                string name = (string) clbTables.Items[i];
                if (SelectedObjects.Split(';').Any(s => name.ToLower() == s.ToLower().Trim()))
                {
                    clbTables.SetItemChecked(i, true);
                }
            }
            for (int i = 0; i < clbViews.Items.Count; i++)
            {
                string name = (string)clbViews.Items[i];
                if (SelectedObjects.Split(';').Any(s => name.ToLower() == s.ToLower().Trim()))
                {
                    clbViews.SetItemChecked(i, true);
                }
            }
        }

        private void ObjectExplorer_Load(object sender, EventArgs e)
        {
            List<LgObject> objectList = GetTablesAndViews();

            foreach (LgObject obj in objectList.OrderBy(z => z.ObjectName))
            {
                if (obj.IsView)
                {
                    clbViews.Items.Add(obj.ObjectName);
                }
                else
                {
                    clbTables.Items.Add(obj.ObjectName);
                }
            }

            for (int i = 0; i < clbTables.Items.Count; i++)
            {
                clbTables.SetItemChecked(i, false);
            }

            for (int i = 0; i < clbViews.Items.Count; i++)
            {
                clbViews.SetItemChecked(i, false);
            }
        }

        private List<LgObject> GetTablesAndViews()
        {
            var lgObjects = new List<LgObject>();

            using (var connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "sp_tables";
                    command.Parameters.AddWithValue("@table_qualifier", DatabaseName);
                    command.Parameters.AddWithValue("@table_owner", DefaultSchema);

                    using (var adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = command;
                        using (var ds = new DataSet())
                        {
                            adapter.Fill(ds);
                            if (ds.Tables.Count > 0)
                            {
                                foreach (DataRow row in ds.Tables[0].Rows)
                                {
                                    var lgObject = new LgObject();
                                    lgObject.ObjectName = (string)row["TABLE_NAME"];
                                    if (lgObject.ObjectName.ToLower() == "dtproperties" || lgObject.ObjectName.ToLower() == "syscolumns" ||
                                        lgObject.ObjectName.ToLower() == "sysdepends" || lgObject.ObjectName.ToLower() == "syscomments" ||
                                        lgObject.ObjectName.ToLower() == "sysfilegroups" || lgObject.ObjectName.ToLower() == "sysfiles" ||
                                        lgObject.ObjectName.ToLower() == "sysfiles1" || lgObject.ObjectName.ToLower() == "sysforeignkeys" ||
                                        lgObject.ObjectName.ToLower() == "sysproperties" || lgObject.ObjectName.ToLower() == "sysusers" ||
                                        lgObject.ObjectName.ToLower() == "sysconstraints" || lgObject.ObjectName.ToLower() == "syssegments" ||
                                        lgObject.ObjectName.ToLower() == "sysdiagrams")
                                    {
                                        continue;
                                    }
                                    
                                    lgObject.IsView = ((string)row["TABLE_TYPE"]).ToLower() == "view";

                                    lgObjects.Add(lgObject);
                                }
                            }
                        }
                    }
                }
            }
            return lgObjects;
        }

        private void btnTablesCheckAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbTables.Items.Count; i++)
            {
                clbTables.SetItemChecked(i, true);
            }
        }

        private void btnTablesDecheckAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbTables.Items.Count; i++)
            {
                clbTables.SetItemChecked(i, false);
            }
        }

        private void btnViewsCheckAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbViews.Items.Count; i++)
            {
                clbViews.SetItemChecked(i, true);
            }
        }

        private void btnViewsDecheckAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbViews.Items.Count; i++)
            {
                clbViews.SetItemChecked(i, false);
            }
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            SelectedObjects = "";

            for (int i = 0; i < clbTables.Items.Count; i++)
            {
                if (clbTables.GetItemChecked(i))
                    SelectedObjects = SelectedObjects + clbTables.Items[i] + ";";
            }
            for (int i = 0; i < clbViews.Items.Count; i++)
            {
                if (clbViews.GetItemChecked(i))
                    SelectedObjects = SelectedObjects + clbViews.Items[i] + ";";
            }
            SelectedObjects = SelectedObjects.TrimEnd(';');
            DialogResult = DialogResult.OK;
        }
    }
}
