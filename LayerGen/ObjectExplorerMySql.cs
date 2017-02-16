using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LayerGen
{
    public partial class ObjectExplorerMySql : Form
    {
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
        public uint ServerPort { get; set; }
        public string CustomConnectionString { get; set; }
        public bool HasCustomConnectionString { get; set; }


        public string SelectedObjects { get; private set; }

        private string ConnectionString
        {
            get
            {
                if (HasCustomConnectionString)
                    return CustomConnectionString;

                var builder = new MySqlConnectionStringBuilder
                {
                    UserID = UserName,
                    Password = Password,
                    Database = DatabaseName,
                    Server = ServerName,
                    Port = ServerPort
                };

                return builder.ConnectionString;
            }
        }

        public ObjectExplorerMySql()
        {
            InitializeComponent();
        }

        private void ObjectExplorerMySql_Load(object sender, EventArgs e)
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

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (DataTable tables = connection.GetSchema("Tables"))
                {
                    foreach (DataRow row in tables.Rows.Cast<DataRow>())
                    {
                        if (((string)row["TABLE_SCHEMA"]) != DatabaseName)
                            continue;
                        var obj = new LgObject();
                        obj.IsView = false;
                        obj.ObjectName = (string)row["TABLE_NAME"];

                        lgObjects.Add(obj);
                    }
                }

                using (DataTable views = connection.GetSchema("Views"))
                {
                    foreach (DataRow row in views.Rows)
                    {
                        if (((string)row["TABLE_SCHEMA"]) != DatabaseName)
                            continue;
                        var obj = new LgObject();
                        obj.IsView = true;
                        obj.ObjectName = (string)row["TABLE_NAME"];

                        lgObjects.Add(obj);
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

        private void btnViewsCheckAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbViews.Items.Count; i++)
            {
                clbViews.SetItemChecked(i, true);
            }
        }

        private void btnTablesDecheckAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbTables.Items.Count; i++)
            {
                clbTables.SetItemChecked(i, false);
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
