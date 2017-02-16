using System;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LayerGen
{
    public partial class MainForm : Form
    {
        const int GwlStyle = -16;
        const int WsDisabled = 0x08000000;

        public MainForm()
        {
            InitializeComponent();

            linkLabel1.Links.Add(new LinkLabel.Link {LinkData = "http://www.newtonsoft.com/json"});
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string outputFolder = GetRegistryValue("OutputFolder");
            string includeComments = GetRegistryValue("IncludeComments");
            string language = GetRegistryValue("Language");
            string sqliteFileName = GetRegistryValue("SqliteFileName");
            string sqlServer = GetRegistryValue("SqlServer");
            string sqlServerName = GetRegistryValue("SqlServerName");
            string sqlServerPort = GetRegistryValue("SqlServerPort");
            string sqlServerDatabaseName = GetRegistryValue("SqlDatabaseName");
            string sqlServerDefaultSchema = GetRegistryValue("SqlDefaultSchema");
            string sqlServerTrustedConnection = GetRegistryValue("SqlTrustedConnection");
            string sqlServerUserName = GetRegistryValue("SqlUserName");
            string sqlServerPassword = GetRegistryValue("SqlPassword");
            string mysqlServerName = GetRegistryValue("MySqlServerName");
            string mysqlServerPort = GetRegistryValue("MySqlServerPort");
            string mysqlServerDatabaseName = GetRegistryValue("MySqlDatabaseName");
            string mysqlServerUserName = GetRegistryValue("MySqlUserName");
            string mysqlServerPassword = GetRegistryValue("MySqlPassword");
            string customNamespaceNames = GetRegistryValue("CustomNamespaceNames");
            string dataNamespaceName = GetRegistryValue("DataNamespaceName");
            string businessNamespaceName = GetRegistryValue("BusinessNamespaceName");
            string eventLogNamespaceName = GetRegistryValue("EventLogNamespaceName");
            string customSqliteConnectString = GetRegistryValue("CustomSqliteConnectString");
            string customSqliteConnectionString = GetRegistryValue("CustomSqliteConnectionString");
            string sqlitePassword = GetRegistryValue("SqlitePassword");
            string customMySqlConnectString = GetRegistryValue("CustomMySqlConnectString");
            string customMySqlConnectionString = GetRegistryValue("CustomMySqlConnectionString");
            string customSqlServerConnectString = GetRegistryValue("CustomSqlServerConnectString");
            string customSqlServerConnectionString = GetRegistryValue("CustomSqlServerConnectionString");
            string dynamicDataRetrieval = GetRegistryValue("DynamicDataRetrieval");
            string automaticallyRightTrimStrings = GetRegistryValue("AutoRightTrimStrings");
            string allowSerialization = GetRegistryValue("AllowSerialization");
            string customComments = GetRegistryValue("CustomComments");
            string dataSuffix = GetRegistryValue("DataSuffix");
            string databaseContext = GetRegistryValue("DatabaseContext");

            txtEventLogNamespace.Text = string.IsNullOrEmpty(eventLogNamespaceName) ? "" : eventLogNamespaceName;
            txtBusinessNamespace.Text = string.IsNullOrEmpty(businessNamespaceName) ? "" : businessNamespaceName;
            txtDataNamespace.Text = string.IsNullOrEmpty(dataNamespaceName) ? "" : dataNamespaceName;
            txtDataSuffix.Text = string.IsNullOrEmpty(dataSuffix) ? "" : dataSuffix;
            txtDbContext.Text = string.IsNullOrEmpty(databaseContext) ? "" : databaseContext;
            txtOutput.Text = string.IsNullOrEmpty(outputFolder) ? "" : outputFolder;
            txtSqliteFileName.Text = string.IsNullOrEmpty(sqliteFileName) ? "" : sqliteFileName;
            txtSqlitePassword.Text = string.IsNullOrEmpty(sqlitePassword) ? "" : sqlitePassword;
            txtMySqlServerName.Text = string.IsNullOrEmpty(mysqlServerName) ? "" : mysqlServerName;
            txtMySqlPort.Text = string.IsNullOrEmpty(mysqlServerPort) ? "3306" : mysqlServerPort;
            txtMySqlDatabaseName.Text = string.IsNullOrEmpty(mysqlServerDatabaseName) ? "" : mysqlServerDatabaseName;
            txtMySqlUserName.Text = string.IsNullOrEmpty(mysqlServerUserName) ? "" : mysqlServerUserName;
            txtMySqlPassword.Text = string.IsNullOrEmpty(mysqlServerPassword) ? "" : mysqlServerPassword;

            if (string.IsNullOrEmpty(allowSerialization))
            {
                chkAllowSerialization.Checked = false;
            }
            else
            {
                try
                {
                    chkAllowSerialization.Checked = bool.Parse(allowSerialization);
                }
                catch
                {
                    chkAllowSerialization.Checked = false;
                }
            }

            if (string.IsNullOrEmpty(automaticallyRightTrimStrings))
            {
                chkAutomaticallyTrimStrings.Checked = true;
            }
            else
            {
                try
                {
                    chkAutomaticallyTrimStrings.Checked = bool.Parse(automaticallyRightTrimStrings);
                }
                catch
                {
                    chkAutomaticallyTrimStrings.Checked = true;
                }
            }

            if (string.IsNullOrEmpty(dynamicDataRetrieval))
            {
                chkEnableDynamicData.Checked = false;
            }
            else
            {
                try
                {
                    chkEnableDynamicData.Checked = bool.Parse(dynamicDataRetrieval);
                }
                catch
                {
                    chkEnableDynamicData.Checked = false;
                }
            }
            if (string.IsNullOrEmpty(customSqlServerConnectString))
            {
                chkSqlServerCustomConnectionString.Checked = false;
            }
            else
            {
                try
                {
                    chkSqlServerCustomConnectionString.Checked = bool.Parse(customSqlServerConnectString);
                }
                catch
                {
                    chkSqlServerCustomConnectionString.Checked = false;
                }
            }
            if (string.IsNullOrEmpty(customMySqlConnectString))
            {
                chkMySqlCustomConnectionString.Checked = false;
            }
            else
            {
                try
                {
                    chkMySqlCustomConnectionString.Checked = bool.Parse(customMySqlConnectString);
                }
                catch
                {
                    chkMySqlCustomConnectionString.Checked = false;
                }
            }
            if (string.IsNullOrEmpty(customSqliteConnectString))
            {
                chkSqliteCustomConnectionString.Checked = false;
            }
            else
            {
                try
                {
                    chkSqliteCustomConnectionString.Checked = bool.Parse(customSqliteConnectString);
                }
                catch
                {
                    chkSqliteCustomConnectionString.Checked = false;
                }
            }
            if (string.IsNullOrEmpty(customNamespaceNames))
            {
                chkCustomNamespaceNames.Checked = false;
            }
            else
            {
                try
                {
                    chkCustomNamespaceNames.Checked = bool.Parse(customNamespaceNames);
                }
                catch
                {
                    chkCustomNamespaceNames.Checked = false;
                }
            }
            if (string.IsNullOrEmpty(includeComments))
            {
                chkIncludeComments.Checked = true;
            }
            else
            {
                try
                {
                    chkIncludeComments.Checked = bool.Parse(includeComments);
                }
                catch
                {
                    chkIncludeComments.Checked = true;
                }
            }
            if (chkIncludeComments.Checked)
            {
                txtCustomComments.Text = customComments;
            }

            if (string.IsNullOrEmpty(language))
            {
                ddlLanguage.SelectedIndex = 0;
            }
            else
            {
                try
                {
                    ddlLanguage.SelectedIndex = int.Parse(language);
                }
                catch
                {
                    ddlLanguage.SelectedIndex = 0;
                }
            }
            if (string.IsNullOrEmpty(sqlServer))
            {
                ddlSqlServer.SelectedIndex = 0;
            }
            else
            {
                try
                {
                    ddlSqlServer.SelectedIndex = int.Parse(sqlServer);
                }
                catch
                {
                    ddlSqlServer.SelectedIndex = 0;
                }
            }

            txtSqlServerCustomConnectionString.Text = string.IsNullOrEmpty(customSqlServerConnectionString) ? "" : customSqlServerConnectionString;
            txtMySqlCustomConnectionString.Text = string.IsNullOrEmpty(customMySqlConnectionString) ? "" : customMySqlConnectionString;
            txtSqliteCustomConnectionString.Text = string.IsNullOrEmpty(customSqliteConnectionString) ? "" : customSqliteConnectionString;
            txtSqlServerName.Text = string.IsNullOrEmpty(sqlServerName) ? "" : sqlServerName;
            txtSqlServerPort.Text = string.IsNullOrEmpty(sqlServerPort) ? "1433" : sqlServerPort;
            txtDatabaseName.Text = string.IsNullOrEmpty(sqlServerDatabaseName) ? "" : sqlServerDatabaseName;
            txtSqlDefaultSchema.Text = string.IsNullOrEmpty(sqlServerDefaultSchema) ? "dbo" : sqlServerDefaultSchema;
            
            if (string.IsNullOrEmpty(sqlServerTrustedConnection))
            {
                chkSqlTrustedConnection.Checked = false;
            }
            else
            {
                try
                {
                    chkSqlTrustedConnection.Checked = bool.Parse(sqlServerTrustedConnection);
                }
                catch
                {
                    chkSqlTrustedConnection.Checked = false;
                }
            }

            txtSqlUserName.Text = string.IsNullOrEmpty(sqlServerUserName) ? "" : sqlServerUserName;
            txtSqlPassword.Text = string.IsNullOrEmpty(sqlServerPassword) ? "" : sqlServerPassword;

            txtSqlServerName.Focus();

            txtSqliteObjects.MaxLength = int.MaxValue;
            txtSqlServerObjects.MaxLength = int.MaxValue;
            txtMySqlObjects.MaxLength = int.MaxValue;

            RefreshNamespaces();
            ConfigureSqliteConnectionString();
            ConfigureMySqlConnectionString();
            ConfigureSqlServerConnectionString();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "SqlServer", ddlSqlServer.SelectedIndex.ToString(CultureInfo.InvariantCulture));
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "OutputFolder", txtOutput.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "IncludeComments", chkIncludeComments.Checked.ToString());
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "Language", ddlLanguage.SelectedIndex.ToString(CultureInfo.InvariantCulture));
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "SqliteFileName", txtSqliteFileName.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "SqlServerName", txtSqlServerName.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "SqlServerPort", txtSqlServerPort.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "SqlDatabaseName", txtDatabaseName.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "SqlDefaultSchema", txtSqlDefaultSchema.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "SqlTrustedConnection", chkSqlTrustedConnection.Checked.ToString());
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "SqlUserName", txtSqlUserName.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "SqlPassword", txtSqlPassword.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "MySqlServerName", txtMySqlServerName.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "MySqlServerPort", txtMySqlPort.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "MySqlDatabaseName", txtMySqlDatabaseName.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "MySqlUserName", txtMySqlUserName.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "MySqlPassword", txtMySqlPassword.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "CustomNamespaceNames", chkCustomNamespaceNames.Checked.ToString());
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "DataNamespaceName", txtDataNamespace.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "BusinessNamespaceName", txtBusinessNamespace.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "EventLogNamespaceName", txtEventLogNamespace.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "SqlitePassword", txtSqlitePassword.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "CustomSqliteConnectString", chkSqliteCustomConnectionString.Checked.ToString());
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "CustomSqliteConnectionString", txtSqliteCustomConnectionString.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "CustomMySqlConnectString", chkMySqlCustomConnectionString.Checked.ToString());
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "CustomMySqlConnectionString", txtMySqlCustomConnectionString.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "CustomSqlServerConnectString", chkSqlServerCustomConnectionString.Checked.ToString());
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "CustomSqlServerConnectionString", txtSqlServerCustomConnectionString.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "DynamicDataRetrieval", chkEnableDynamicData.Checked.ToString());
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "AutoRightTrimStrings", chkAutomaticallyTrimStrings.Checked.ToString());
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "AllowSerialization", chkAllowSerialization.Checked.ToString());
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "CustomComments", txtCustomComments.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "DataSuffixs", txtDataSuffix.Text);
            RegistryFunctions.WriteToRegistry(Microsoft.Win32.RegistryHive.CurrentUser, "Software\\Icemanind\\LayerGen", "DatabaseContext", txtDbContext.Text);
        }

        private void chkSqlTrustedConnection_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSqlTrustedConnection.Checked)
            {
                txtSqlUserName.Enabled = false;
                txtSqlPassword.Enabled = false;
                pbHelpSqlServerUserName.Visible = false;
                pbHelpSqlServerPassword.Visible = false;
            }
            else
            {
                txtSqlUserName.Enabled = true;
                txtSqlPassword.Enabled = true;
                pbHelpSqlServerUserName.Visible = true;
                pbHelpSqlServerPassword.Visible = true;
            }

            ConfigureSqlServerConnectionString();
        }

        private void chkIncludeComments_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIncludeComments.Checked)
            {
                txtCustomComments.Enabled = true;
                txtCustomComments.Text = GetRegistryValue("CustomComments");
            }
            else
            {
                txtCustomComments.Enabled = false;
                txtCustomComments.Text = "";
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult dr = folderBrowserDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                txtOutput.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void SqlServerThread(object plugin)
        {
            var form = (DatabasePlugins.SqlServer) plugin;

            form.CreateLayers();

            if (((Form)form.ProgressBar.Parent).InvokeRequired)
            {
                Invoke((Action)(() => { CloseProgressForm(form); }));
            }
        }

        private void MySqlServerThread(object plugin)
        {
            var form = (DatabasePlugins.MySql)plugin;

            form.CreateLayers();

            if (((Form)form.ProgressBar.Parent).InvokeRequired)
            {
                Invoke((Action)(() => { CloseProgressForm(form); }));
            }
        }

        private void SqliteThread(object plugin)
        {
            var form = (DatabasePlugins.Sqlite)plugin;

            form.CreateLayers();

            if (((Form) form.ProgressBar.Parent).InvokeRequired)
            {
                Invoke((Action)(() => { CloseProgressForm(form); }));
            }
        }

        private void CloseProgressForm(DatabasePlugins.IDatabasePlugin plugin)
        {
            ((Form) plugin.ProgressBar.Parent).Close();
            SetNativeEnabled(true);
            MessageBox.Show(this, @"LayerGen is Complete!");
        }

        private void btnCreateLayers_Click(object sender, EventArgs e)
        {
            if (ddlSqlServer.SelectedIndex == 0)
            {
                if (string.IsNullOrEmpty(txtSqlServerObjects.Text) ||
                    string.IsNullOrEmpty(txtSqlServerObjects.Text.Trim()) || txtSqlServerObjects.Text.Trim() == ";")
                {
                    MessageBox.Show(@"You must select at least one object (table or view)!", @"Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if (ddlSqlServer.SelectedIndex == 1)
            {
                if (string.IsNullOrEmpty(txtSqliteObjects.Text) ||
                    string.IsNullOrEmpty(txtSqliteObjects.Text.Trim()) || txtSqliteObjects.Text.Trim() == ";")
                {
                    MessageBox.Show(@"You must select at least one object (table or view)!", @"Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if (ddlSqlServer.SelectedIndex == 2)
            {
                if (string.IsNullOrEmpty(txtMySqlObjects.Text) ||
                    string.IsNullOrEmpty(txtMySqlObjects.Text.Trim()) || txtMySqlObjects.Text.Trim() == ";")
                {
                    MessageBox.Show(@"You must select at least one object (table or view)!", @"Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            try
            {
                new FileInfo(txtOutput.Text);
            }
            catch
            {
                MessageBox.Show(@"Invalid Output Path!", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(txtOutput.Text))
            {
                DialogResult dr = MessageBox.Show(@"Directory does not exist. Create?", @"Directory not found", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.No)
                {
                    return;
                }
                Directory.CreateDirectory(txtOutput.Text);
            }

            var pw = new PleaseWaitForm {StartPosition = FormStartPosition.Manual};
            pw.Location = new Point(Location.X + (Width - pw.Width)/2, Location.Y + (Height - pw.Height)/2);
            pw.Show(this);
            SetNativeEnabled(false);

            pw.ProgressBar.Minimum = 0;
            pw.ProgressBar.Value = 0;
            pw.ProgressBar.Maximum = 100;

            if (ddlSqlServer.SelectedIndex == 0)
            {
                var databasePlugin = new DatabasePlugins.SqlServer
                {
                    DatabaseName = txtDatabaseName.Text,
                    DatabasePort = int.Parse(txtSqlServerPort.Text),
                    DatabaseServer = txtSqlServerName.Text,
                    OutputDirectory = txtOutput.Text,
                    Password = txtSqlPassword.Text,
                    TrustedConnection = chkSqlTrustedConnection.Checked,
                    UserName = txtSqlUserName.Text,
                    Objects = txtSqlServerObjects.Text,
                    DefaultSchema = txtSqlDefaultSchema.Text,
                    IncludeComments = chkIncludeComments.Checked,
                    ProgressBar = pw.ProgressBar,
                    DataNamespaceName = txtDataNamespace.Text,
                    BusinessNamespaceName = txtBusinessNamespace.Text,
                    EventLogNamespaceName = txtEventLogNamespace.Text,
                    DataObjectSuffix = txtDataSuffix.Text,
                    DatabaseContenxt = string.IsNullOrEmpty(txtDbContext.Text) ? txtDatabaseName.Text :  txtDbContext.Text,
                    AutoRightTrimStrings = chkAutomaticallyTrimStrings.Checked,
                    HasCustomConnectionString = chkSqlServerCustomConnectionString.Checked,
                    CustomConnectionString = txtSqlServerCustomConnectionString.Text.Replace('\r', ' ').Replace('\n', ' '),
                    HasDynamicDataRetrieval = chkEnableDynamicData.Checked,
                    AllowSerialization = chkAllowSerialization.Checked,
                    CustomComments  = txtCustomComments.Text
                };

                if ((string) ddlLanguage.SelectedItem == "VB.NET")
                {
                    databasePlugin.Language = DatabasePlugins.Languages.VbNet;
                }
                if ((string) ddlLanguage.SelectedItem == "C#")
                {
                    databasePlugin.Language = DatabasePlugins.Languages.CSharp;
                }
                if ((string)ddlLanguage.SelectedItem == "Java appfuse")
                {
                    databasePlugin.Language = DatabasePlugins.Languages.Java;
                }

                var thread = new Thread(SqlServerThread);
                thread.Start(databasePlugin);
            }

            if (ddlSqlServer.SelectedIndex == 1)
            {
                var databasePlugin = new DatabasePlugins.Sqlite
                {
                    DatabaseName = txtSqliteFileName.Text,
                    Password = string.IsNullOrEmpty(txtSqlitePassword.Text.Trim()) ? null : txtSqlitePassword.Text.Trim(),
                    Objects = txtSqliteObjects.Text,
                    OutputDirectory = txtOutput.Text,
                    IncludeComments = chkIncludeComments.Checked,
                    ProgressBar = pw.ProgressBar,
                    DataNamespaceName = txtDataNamespace.Text,
                    AutoRightTrimStrings = chkAutomaticallyTrimStrings.Checked,
                    BusinessNamespaceName = txtBusinessNamespace.Text,
                    HasCustomConnectionString = chkSqliteCustomConnectionString.Checked,
                    CustomConnectionString = txtSqliteCustomConnectionString.Text.Replace('\r', ' ').Replace('\n', ' '),
                    HasDynamicDataRetrieval = chkEnableDynamicData.Checked,
                    AllowSerialization = chkAllowSerialization.Checked
                };

                if ((string)ddlLanguage.SelectedItem == "VB.NET")
                {
                    databasePlugin.Language = DatabasePlugins.Languages.VbNet;
                }
                if ((string)ddlLanguage.SelectedItem == "C#")
                {
                    databasePlugin.Language = DatabasePlugins.Languages.CSharp;
                }
                if ((string)ddlLanguage.SelectedItem == "Java appfuse")
                {
                    databasePlugin.Language = DatabasePlugins.Languages.Java;
                }

                var thread = new Thread(SqliteThread);
                thread.Start(databasePlugin);
            }

            if (ddlSqlServer.SelectedIndex == 2)
            {
                var databasePlugin = new DatabasePlugins.MySql
                {
                    DatabaseName = txtMySqlDatabaseName.Text,
                    Objects = txtMySqlObjects.Text,
                    OutputDirectory = txtOutput.Text,
                    IncludeComments = chkIncludeComments.Checked,
                    DatabaseServer = txtMySqlServerName.Text,
                    UserName = txtMySqlUserName.Text,
                    Password = txtMySqlPassword.Text,
                    AutoRightTrimStrings = chkAutomaticallyTrimStrings.Checked,
                    DatabasePort = int.Parse(txtMySqlPort.Text),
                    ProgressBar = pw.ProgressBar,
                    DataNamespaceName = txtDataNamespace.Text,
                    BusinessNamespaceName = txtBusinessNamespace.Text,
                    HasCustomConnectionString = chkMySqlCustomConnectionString.Checked,
                    CustomConnectionString = txtMySqlCustomConnectionString.Text.Replace('\r', ' ').Replace('\n', ' '),
                    HasDynamicDataRetrieval = chkEnableDynamicData.Checked,
                    AllowSerialization = chkAllowSerialization.Checked,
                    CustomComments  = txtCustomComments.Text
                };

                if ((string)ddlLanguage.SelectedItem == "VB.NET")
                {
                    databasePlugin.Language = DatabasePlugins.Languages.VbNet;
                }
                if ((string)ddlLanguage.SelectedItem == "C#")
                {
                    databasePlugin.Language = DatabasePlugins.Languages.CSharp;
                }
                if ((string)ddlLanguage.SelectedItem == "Java appfuse")
                {
                    databasePlugin.Language = DatabasePlugins.Languages.Java;
                }

                var thread = new Thread(MySqlServerThread);
                thread.Start(databasePlugin);
            }
        }

        private void ddlSqlServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSqlServer.SelectedIndex == 0)
            {
                pnlSqlServer.Visible = true;
                pnlSqlite.Visible = false;
                pnlMySql.Visible = false;
            }
            else if (ddlSqlServer.SelectedIndex == 1)
            {
                pnlSqlServer.Visible = false;
                pnlSqlite.Visible = true;
                pnlMySql.Visible = false;
            }
            else if (ddlSqlServer.SelectedIndex == 2)
            {
                pnlSqlServer.Visible = false;
                pnlSqlite.Visible = false;
                pnlMySql.Visible = true;
                pnlMySql.BringToFront();
            }
            RefreshNamespaces();
        }

        private void btnTablesBrowse_Click(object sender, EventArgs e)
        {
            var objectExplorer = new ObjectExplorer
            {
                DatabaseName = txtDatabaseName.Text,
                Port = int.Parse(txtSqlServerPort.Text),
                ServerName = txtSqlServerName.Text,
                Password = txtSqlPassword.Text,
                UserName = txtSqlUserName.Text,
                TrustedConnection = chkSqlTrustedConnection.Checked,
                DefaultSchema = txtSqlDefaultSchema.Text,
                SelectedObjects = txtSqlServerObjects.Text,
                HasCustomConnectionString = chkSqlServerCustomConnectionString.Checked,
                CustomConnectionString = txtSqlServerCustomConnectionString.Text.Replace('\r', ' ').Replace('\n', ' ')
            };

            DialogResult dr = objectExplorer.ShowDialog();

            if (dr == DialogResult.OK)
            {
                txtSqlServerObjects.Text = objectExplorer.SelectedObjects;
            }
        }

        private void btnSqliteObjectsBrowse_Click(object sender, EventArgs e)
        {
            var objectExplorer = new ObjectExplorerSqlite
            {
                Filename = txtSqliteFileName.Text,
                Password = string.IsNullOrEmpty(txtSqlitePassword.Text.Trim()) ? null : txtSqlitePassword.Text.Trim(),
                HasCustomConnectionString = chkSqliteCustomConnectionString.Checked,
                CustomConnectionString = txtSqliteCustomConnectionString.Text.Replace('\r', ' ').Replace('\n', ' ')
            };

            DialogResult dr = objectExplorer.ShowDialog();

            if (dr == DialogResult.OK)
            {
                txtSqliteObjects.Text = objectExplorer.SelectedObjects;
            }
        }

        private void btnMySqlObjectsBrowse_Click(object sender, EventArgs e)
        {
            var objectExplorer = new ObjectExplorerMySql
            {
                DatabaseName = txtMySqlDatabaseName.Text,
                UserName = txtMySqlUserName.Text,
                Password = txtMySqlPassword.Text,
                ServerName = txtMySqlServerName.Text,
                ServerPort = uint.Parse(txtMySqlPort.Text),
                HasCustomConnectionString = chkMySqlCustomConnectionString.Checked,
                CustomConnectionString = txtMySqlCustomConnectionString.Text.Replace('\r', ' ').Replace('\n', ' ')
            };

            DialogResult dr = objectExplorer.ShowDialog();

            if (dr == DialogResult.OK)
            {
                txtMySqlObjects.Text = objectExplorer.SelectedObjects;
            }
        }

        private void btnSqliteFileNameBrowse_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();

            if (dr == DialogResult.OK)
            {
                txtSqliteFileName.Text = openFileDialog1.FileName;
            }
        }

        private void RefreshNamespaces()
        {
            if (chkCustomNamespaceNames.Checked)
            {
                label20.ForeColor = Color.White;
                label21.ForeColor = Color.White;
                txtDataNamespace.Enabled = true;
                txtBusinessNamespace.Enabled = true;
                pbHelpBusinessNamespace.Visible = true;
                pbHelpDataNamespace.Visible = true;

                return;
            }
            pbHelpBusinessNamespace.Visible = false;
            pbHelpDataNamespace.Visible = false;

            label20.ForeColor = Color.FromArgb(165, 165, 165);
            label21.ForeColor = Color.FromArgb(165, 165, 165);
            txtDataNamespace.Enabled = false;
            txtBusinessNamespace.Enabled = false;

            bool isCs = ddlLanguage.SelectedIndex == 0;

            if (ddlSqlServer.SelectedIndex == 0)
            {
                txtDataNamespace.Text = @"DataLayer." + txtDatabaseName.Text.FirstCharToUpper();
                txtDataNamespace.Text = isCs ? txtDataNamespace.Text.ConvertToValidCSharpNamespace() : txtDataNamespace.Text.ConvertToValidVbNamespace();
                txtBusinessNamespace.Text = @"BusinessLayer." + txtDatabaseName.Text.FirstCharToUpper();
                txtBusinessNamespace.Text = isCs ? txtBusinessNamespace.Text.ConvertToValidCSharpNamespace() : txtBusinessNamespace.Text.ConvertToValidVbNamespace();
            }
            if (ddlSqlServer.SelectedIndex == 1)
            {
                string file;
                try
                {
                    file = Path.GetFileNameWithoutExtension(txtSqliteFileName.Text);
                }
                catch
                {
                    file = "";
                }
                txtDataNamespace.Text = @"DataLayer." + file.FirstCharToUpper();
                txtDataNamespace.Text = isCs ? txtDataNamespace.Text.ConvertToValidCSharpNamespace() : txtDataNamespace.Text.ConvertToValidVbNamespace();
                txtBusinessNamespace.Text = @"BusinessLayer." + file.FirstCharToUpper();
                txtBusinessNamespace.Text = isCs ? txtBusinessNamespace.Text.ConvertToValidCSharpNamespace() : txtBusinessNamespace.Text.ConvertToValidVbNamespace();
            }
            if (ddlSqlServer.SelectedIndex == 2)
            {
                txtDataNamespace.Text = @"DataLayer." + txtMySqlDatabaseName.Text.FirstCharToUpper();
                txtDataNamespace.Text = isCs ? txtDataNamespace.Text.ConvertToValidCSharpNamespace() : txtDataNamespace.Text.ConvertToValidVbNamespace();
                txtBusinessNamespace.Text = @"BusinessLayer." + txtMySqlDatabaseName.Text.FirstCharToUpper();
                txtBusinessNamespace.Text = isCs ? txtBusinessNamespace.Text.ConvertToValidCSharpNamespace() : txtBusinessNamespace.Text.ConvertToValidVbNamespace();
            }
        }

        private void SetNativeEnabled(bool enabled)
        {
            NativeMethods.SetWindowLong(Handle, GwlStyle, NativeMethods.GetWindowLong(Handle, GwlStyle) &
                ~WsDisabled | (enabled ? 0 : WsDisabled));
        }

        private string GetRegistryValue(string valueName)
        {
            string error = "";
            return RegistryFunctions.RegValue(Microsoft.Win32.RegistryHive.CurrentUser,
                "Software\\Icemanind\\LayerGen", valueName, ref error);
        }

        private void chkCustomNamespaceNames_CheckedChanged(object sender, EventArgs e)
        {
            RefreshNamespaces();
        }

        private void txtMySqlDatabaseName_TextChanged(object sender, EventArgs e)
        {
            RefreshNamespaces();
            ConfigureMySqlConnectionString();
        }

        private void txtDatabaseName_TextChanged(object sender, EventArgs e)
        {
            RefreshNamespaces();
            ConfigureSqlServerConnectionString();
        }

        private void txtSqliteFileName_TextChanged(object sender, EventArgs e)
        {
            RefreshNamespaces();
            ConfigureSqliteConnectionString();
        }
        
        private void txtSqlitePassword_TextChanged(object sender, EventArgs e)
        {
            RefreshNamespaces();
            ConfigureSqliteConnectionString();
        }

        private void ddlLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshNamespaces();
        }

        private void chkSqliteCustomConnectionString_CheckedChanged(object sender, EventArgs e)
        {
            ConfigureSqliteConnectionString();
        }

        private void chkSqlServerCustomConnectionString_CheckedChanged(object sender, EventArgs e)
        {
            ConfigureSqlServerConnectionString();
        }

        private void chkMySqlCustomConnectionString_CheckedChanged(object sender, EventArgs e)
        {
            ConfigureMySqlConnectionString();
        }

        private void txtMySqlServerName_TextChanged(object sender, EventArgs e)
        {
            ConfigureMySqlConnectionString();
        }

        private void txtMySqlPort_TextChanged(object sender, EventArgs e)
        {
            ConfigureMySqlConnectionString();
        }

        private void txtMySqlUserName_TextChanged(object sender, EventArgs e)
        {
            ConfigureMySqlConnectionString();
        }

        private void txtMySqlPassword_TextChanged(object sender, EventArgs e)
        {
            ConfigureMySqlConnectionString();
        }

        private void txtSqlPassword_TextChanged(object sender, EventArgs e)
        {
            ConfigureSqlServerConnectionString();
        }

        private void txtSqlServerName_TextChanged(object sender, EventArgs e)
        {
            ConfigureSqlServerConnectionString();
        }

        private void txtSqlUserName_TextChanged(object sender, EventArgs e)
        {
            ConfigureSqlServerConnectionString();
        }

        private void txtSqlServerPort_TextChanged(object sender, EventArgs e)
        {
            ConfigureSqlServerConnectionString();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData as string ?? "");
        }

        private void ConfigureSqliteConnectionString()
        {
            if (chkSqliteCustomConnectionString.Checked)
            {
                txtSqliteCustomConnectionString.Enabled = true;
                return;
            }

            txtSqliteCustomConnectionString.Enabled = false;

            try
            {
                var builder = new SQLiteConnectionStringBuilder
                {
                    DataSource = txtSqliteFileName.Text,
                    JournalMode = SQLiteJournalModeEnum.Memory,
                    Password = string.IsNullOrEmpty(txtSqlitePassword.Text.Trim()) ? null : txtSqlitePassword.Text.Trim()
                };

                txtSqliteCustomConnectionString.Text = builder.ConnectionString;
            }
            catch
            {
                txtSqliteCustomConnectionString.Text = "";
            }
        }

        private void ConfigureMySqlConnectionString()
        {
            if (chkMySqlCustomConnectionString.Checked)
            {
                txtMySqlCustomConnectionString.Enabled = true;
                return;
            }

            txtMySqlCustomConnectionString.Enabled = false;

            try
            {
                var builder = new MySqlConnectionStringBuilder
                {
                    UserID = txtMySqlUserName.Text,
                    Password = txtMySqlPassword.Text,
                    Database = txtMySqlDatabaseName.Text,
                    Server = txtMySqlServerName.Text,
                    ConvertZeroDateTime = true,
                    Port = uint.Parse(txtMySqlPort.Text)
                };

                txtMySqlCustomConnectionString.Text = builder.ConnectionString;
            }
            catch
            {
                txtMySqlCustomConnectionString.Text = "";
            }
        }

        private void ConfigureSqlServerConnectionString()
        {
            if (chkSqlServerCustomConnectionString.Checked)
            {
                txtSqlServerCustomConnectionString.Enabled = true;
                return;
            }

            txtSqlServerCustomConnectionString.Enabled = false;

            try
            {
                var builder = new SqlConnectionStringBuilder();
                builder["Data Source"] = txtSqlServerName.Text + "," + txtSqlServerPort.Text; 
                builder["Integrated Security"] = chkSqlTrustedConnection.Checked;
                builder["Initial Catalog"] = txtDatabaseName.Text;
                if (!chkSqlTrustedConnection.Checked)
                {
                    builder["User ID"] = txtSqlUserName.Text;
                    builder["Password"] = txtSqlPassword.Text;
                }

                txtSqlServerCustomConnectionString.Text = builder.ConnectionString;
            }
            catch
            {
                txtSqlServerCustomConnectionString.Text = "";
            }
        }
    }
}
