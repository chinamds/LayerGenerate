using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace LayerGen.DatabasePlugins
{
    public class SqlServer : IDatabasePlugin
    {
        private delegate void SetTextCallback(int percentage);

        private int _progressNdx;

        public Languages Language { get; set; }
        public string DatabaseName { get; set; }

        public DatabaseTypes DatabaseType
        {
            get { return DatabaseTypes.SqlServer; }
        }

        public string OutputDirectory { get; set; }
        public bool HasDynamicDataRetrieval { get; set; }
        public bool AutoRightTrimStrings { get; set; }
        public bool AllowSerialization { get; set; }
        public string DatabaseServer { get; set; }
        public int DatabasePort { get; set; }
        public string Objects { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool TrustedConnection { get; set; }
        public bool HasCustomConnectionString { get; set; }
        public string CustomConnectionString { get; set; }
        public string DefaultSchema { get; set; }
        public bool IncludeComments { get; set; }
        public ProgressBar ProgressBar { get; set; }
        public string DataNamespaceName { get; set; }
        public string BusinessNamespaceName { get; set; }
        public string EventLogNamespaceName { get; set; }
        public string CustomComments { get; set; }
        public string DataObjectSuffix { get; set; }
        public string DatabaseContenxt { get; set; }

        public string ModelPackageName { get; set; }
        public string DaoPackageName { get; set; }
        public string ServicePackageName { get; set; }
        public string ControllerPackageName { get; set; }

        public string ModelExtends { get; set; }
        public string DaoExtends { get; set; }
        public string ServiceExtends { get; set; }
        public string ControllerExtends { get; set; }

        public bool GenerateTestData { get; set; }
        public bool AllowJsonSerialization { get; set; }

        private string ConnectionString
        {
            get
            {
                if (HasCustomConnectionString)
                    return CustomConnectionString;

                var builder = new SqlConnectionStringBuilder();
                builder["Data Source"] = DatabaseServer + "," + DatabasePort;
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

        public void CreateLayers()
        {
            _progressNdx = 0;
            UpdateProgress(0, 1);            

            if (Language == Languages.CSharp)
            {
                CreateCsBILayers();
                CreateCsICLayers();
                CreateCsCollectionLayers();
                CreateCsRepoLayers();
                CreateCsDtoLayers();
                CreateCsControllerLayers();
                CreateCsInitDataLayers();
                CreateCsDataLayers();
                CreateCsBusinessLayers();
                CreateCsUniversalFile();
            }
            if (Language == Languages.VbNet)
            {
                CreateVbDataLayers();
                CreateVbBusinessLayers();
                CreateVbUniversalFile();
            }
            if (Language == Languages.Java)
            {
                CreateJavaEntityLayers();
                //GetIndexs("Settings", "");
                //CreateSQLScript();
            }
        }

        public void CreateCsUniversalFile()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var universal1Template = new StringBuilder();
            var universal2Template = new StringBuilder();

            using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.Universal1SqlServerCs.txt"))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        universal1Template.Append(reader.ReadToEnd());
                    }
                }
            }

            using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.Universal2SqlServerCs.txt"))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        universal2Template.Append(reader.ReadToEnd());
                    }
                }
            }

            universal2Template.Replace("{0}", DataNamespaceName);

            using (StreamWriter sw = File.CreateText(OutputDirectory + "\\Universal.cs"))
            {
                if (HasDynamicDataRetrieval)
                {
                    sw.WriteLine("using System;");
                    sw.WriteLine("using System.Collections.Generic;");
                    sw.WriteLine("using System.Data;");
                    sw.WriteLine("using System.Data.SqlClient;");
                    sw.WriteLine("using System.Dynamic;");
                    sw.WriteLine("using System.Linq;");
                    sw.WriteLine("using System.Reflection;");
                    sw.WriteLine();
                }
                sw.WriteLine("namespace " + DataNamespaceName);
                sw.WriteLine("{");
                sw.WriteLine("    internal static class Universal");
                sw.WriteLine("    {");
                sw.WriteLine("        /// <summary>");
                sw.WriteLine("        /// Gets the connection string to connect to the database");
                sw.WriteLine("        /// </summary>");
                sw.WriteLine("        /// <returns>A string containing the connection string</returns>");
                sw.WriteLine("        internal static string GetConnectionString()");
                sw.WriteLine("        {");
                sw.WriteLine("            // If this is an ASP.NET application, you can use a line like the following to pull");
                sw.WriteLine("            // the connection string from the Web.Config:");
                sw.WriteLine("            // return System.Configuration.ConfigurationManager.ConnectionStrings[\"MyConnectionString\"].ConnectionString;");
                sw.WriteLine("");
                sw.WriteLine("            return \"" + ConnectionString.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\";");
                sw.WriteLine("        }");
                if (HasDynamicDataRetrieval)
                {
                    sw.WriteLine();
                    sw.WriteLine(universal1Template.ToString());
                    sw.WriteLine();
                }
                sw.WriteLine("    }");
                sw.WriteLine("}");
                sw.WriteLine();
                sw.WriteLine("namespace " + BusinessNamespaceName);
                sw.WriteLine("{");
                if (AllowSerialization)
                {
                    sw.WriteLine();
                    sw.WriteLine("    /// <summary>");
                    sw.WriteLine("    /// Enumeration of various serialization formats");
                    sw.WriteLine("    /// </summary>");
                    sw.WriteLine("    public enum SerializationFormats");
                    sw.WriteLine("    {");
                    sw.WriteLine("        /// <summary>");
                    sw.WriteLine("        /// JSON format");
                    sw.WriteLine("        /// </summary>");
                    sw.WriteLine("        Json = 1,");
                    sw.WriteLine("        /// <summary>");
                    sw.WriteLine("        /// XML format");
                    sw.WriteLine("        /// </summary>");
                    sw.WriteLine("        Xml = 2,");
                    sw.WriteLine("        /// <summary>");
                    sw.WriteLine("        /// Base 64 encoded BSON format");
                    sw.WriteLine("        /// </summary>");
                    sw.WriteLine("        BsonBase64 = 3");
                    sw.WriteLine("    }");
                }
                if (HasDynamicDataRetrieval)
                {
                    sw.WriteLine();
                    sw.WriteLine(universal2Template.ToString());
                    sw.WriteLine();
                }
                sw.WriteLine("    /// <summary>");
                sw.WriteLine("    /// The exception that is thrown when data in memory is out of sync with data in the database.");
                sw.WriteLine("    /// </summary>");
                sw.WriteLine("    public class OutOfSyncException : System.Exception");
                sw.WriteLine("    {");
                sw.WriteLine("        public OutOfSyncException()");
                sw.WriteLine("        {");
                sw.WriteLine("        }");
                sw.WriteLine();
                sw.WriteLine("        public OutOfSyncException(string message)");
                sw.WriteLine("            : base(message)");
                sw.WriteLine("        {");
                sw.WriteLine("        }");
                sw.WriteLine();
                sw.WriteLine("        public OutOfSyncException(string message, System.Exception inner)");
                sw.WriteLine("            : base(message, inner)");
                sw.WriteLine("        {");
                sw.WriteLine("        }");
                sw.WriteLine("    }");
                sw.WriteLine();
                sw.WriteLine("    /// <summary>");
                sw.WriteLine("    /// The exception that is thrown when data in memory is in a read only state.");
                sw.WriteLine("    /// </summary>");
                sw.WriteLine("    public class ReadOnlyException : System.Exception");
                sw.WriteLine("    {");
                sw.WriteLine("        public ReadOnlyException()");
                sw.WriteLine("        {");
                sw.WriteLine("        }");
                sw.WriteLine();
                sw.WriteLine("        public ReadOnlyException(string message)");
                sw.WriteLine("            : base(message)");
                sw.WriteLine("        {");
                sw.WriteLine("        }");
                sw.WriteLine();
                sw.WriteLine("        public ReadOnlyException(string message, System.Exception inner)");
                sw.WriteLine("            : base(message, inner)");
                sw.WriteLine("        {");
                sw.WriteLine("        }");
                sw.WriteLine("    }");
                sw.WriteLine();
                sw.WriteLine("    /// <summary>");
                sw.WriteLine("    /// The exception that is thrown when trying to read from a row that doesn't exist.");
                sw.WriteLine("    /// </summary>");
                sw.WriteLine("    public class RowNotFoundException : System.Exception");
                sw.WriteLine("    {");
                sw.WriteLine("        public RowNotFoundException()");
                sw.WriteLine("        {");
                sw.WriteLine("        }");
                sw.WriteLine();
                sw.WriteLine("        public RowNotFoundException(string message)");
                sw.WriteLine("            : base(message)");
                sw.WriteLine("        {");
                sw.WriteLine("        }");
                sw.WriteLine();
                sw.WriteLine("        public RowNotFoundException(string message, System.Exception inner)");
                sw.WriteLine("            : base(message, inner)");
                sw.WriteLine("        {");
                sw.WriteLine("        }");
                sw.WriteLine("    }");
                sw.WriteLine("}");
            }
        }

        public void CreateVbUniversalFile()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var universal1Template = new StringBuilder();
            var universal2Template = new StringBuilder();

            using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.Universal1SqlServerVb.txt"))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        universal1Template.Append(reader.ReadToEnd());
                    }
                }
            }

            using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.Universal2SqlServerVb.txt"))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        universal2Template.Append(reader.ReadToEnd());
                    }
                }
            }

            universal2Template.Replace("{0}", DataNamespaceName);

            using (StreamWriter sw = File.CreateText(OutputDirectory + "\\Universal.vb"))
            {
                sw.WriteLine("Option Strict On");
                sw.WriteLine("Option Explicit On");
                sw.WriteLine();
                if (HasDynamicDataRetrieval)
                {
                    sw.WriteLine("Imports System.Collections.Generic");
                    sw.WriteLine("Imports System.Data");
                    sw.WriteLine("Imports System.Data.SqlClient");
                    sw.WriteLine("Imports System.Dynamic");
                    sw.WriteLine("Imports System.Linq");
                    sw.WriteLine("Imports System.Reflection");
                    sw.WriteLine();
                }
                sw.WriteLine("Namespace " + DataNamespaceName);
                sw.WriteLine("    Friend NotInheritable Class Universal");
                sw.WriteLine("        Private Sub New()");
                sw.WriteLine("        End Sub");
                sw.WriteLine("        ''' <summary>");
                sw.WriteLine("        ''' Gets the connection string to connect to the database");
                sw.WriteLine("        ''' </summary>");
                sw.WriteLine("        ''' <returns>A string containing the connection string</returns>");
                sw.WriteLine("        Friend Shared Function GetConnectionString() As String");
                sw.WriteLine("            ' If this is an ASP.NET application, you can use a line like the following to pull");
                sw.WriteLine("            ' the connection string from the Web.Config:");
                sw.WriteLine("            ' Return System.Configuration.ConfigurationManager.ConnectionStrings(\"MyConnectionString\").ConnectionString");
                sw.WriteLine("");
                sw.WriteLine("            Return \"" + ConnectionString.Replace("\"", "\"\"") + "\"");
                sw.WriteLine("        End Function");
                if (HasDynamicDataRetrieval)
                {
                    sw.WriteLine();
                    sw.WriteLine(universal1Template.ToString());
                    sw.WriteLine();
                }
                sw.WriteLine("    End Class");
                
                sw.WriteLine("End Namespace");
                sw.WriteLine();
                sw.WriteLine("Namespace " + BusinessNamespaceName);
                if (AllowSerialization)
                {
                    sw.WriteLine();
                    sw.WriteLine("    ''' <summary>");
                    sw.WriteLine("    ''' Enumeration of various serialization formats");
                    sw.WriteLine("    ''' </summary>");
                    sw.WriteLine("    Public Enum SerializationFormats");
                    sw.WriteLine("        ''' <summary>");
                    sw.WriteLine("        ''' JSON format");
                    sw.WriteLine("        ''' </summary>");
                    sw.WriteLine("        Json = 1");
                    sw.WriteLine("        ''' <summary>");
                    sw.WriteLine("        ''' XML format");
                    sw.WriteLine("        ''' </summary>");
                    sw.WriteLine("        Xml = 2");
                    sw.WriteLine("        ''' <summary>");
                    sw.WriteLine("        ''' Base 64 encoded BSON format");
                    sw.WriteLine("        ''' </summary>");
                    sw.WriteLine("        BsonBase64 = 3");
                    sw.WriteLine("    End Enum");
                }
                if (HasDynamicDataRetrieval)
                {
                    sw.WriteLine();
                    sw.WriteLine(universal2Template.ToString());
                    sw.WriteLine();
                }
                sw.WriteLine("    ''' <summary>");
                sw.WriteLine("    ''' The exception that is thrown when data in memory is out of sync with data in the database.");
                sw.WriteLine("    ''' </summary>");
                sw.WriteLine("    Public Class OutOfSyncException");
                sw.WriteLine("        Inherits System.Exception");
                sw.WriteLine();
                sw.WriteLine("        Public Sub New()");
                sw.WriteLine("        End Sub");
                sw.WriteLine();
                sw.WriteLine("        Public Sub New(message As String)");
                sw.WriteLine("            MyBase.New(message)");
                sw.WriteLine("        End Sub");
                sw.WriteLine();
                sw.WriteLine("        Public Sub New(message As String, inner As System.Exception)");
                sw.WriteLine("            MyBase.New(message, inner)");
                sw.WriteLine("        End Sub");
                sw.WriteLine("    End Class");
                sw.WriteLine();
                sw.WriteLine("    ''' <summary>");
                sw.WriteLine("    ''' The exception that is thrown when data in memory is in a read only state.");
                sw.WriteLine("    ''' </summary>");
                sw.WriteLine("    Public Class ReadOnlyException");
                sw.WriteLine("        Inherits System.Exception");
                sw.WriteLine();
                sw.WriteLine("        Public Sub New()");
                sw.WriteLine("        End Sub");
                sw.WriteLine();
                sw.WriteLine("        Public Sub New(message As String)");
                sw.WriteLine("            MyBase.New(message)");
                sw.WriteLine("        End Sub");
                sw.WriteLine();
                sw.WriteLine("        Public Sub New(message As String, inner As System.Exception)");
                sw.WriteLine("            MyBase.New(message, inner)");
                sw.WriteLine("        End Sub");
                sw.WriteLine("    End Class");
                sw.WriteLine();
                sw.WriteLine("    ''' <summary>");
                sw.WriteLine("    ''' The exception that is thrown when trying to read from a row that doesn't exist.");
                sw.WriteLine("    ''' </summary>");
                sw.WriteLine("    Public Class RowNotFoundException");
                sw.WriteLine("        Inherits System.Exception");
                sw.WriteLine();
                sw.WriteLine("        Public Sub New()");
                sw.WriteLine("        End Sub");
                sw.WriteLine();
                sw.WriteLine("        Public Sub New(message As String)");
                sw.WriteLine("            MyBase.New(message)");
                sw.WriteLine("        End Sub");
                sw.WriteLine();
                sw.WriteLine("        Public Sub New(message As String, inner As System.Exception)");
                sw.WriteLine("            MyBase.New(message, inner)");
                sw.WriteLine("        End Sub");
                sw.WriteLine("    End Class");
                sw.WriteLine("End Namespace");
            }
        }

        private void UpdateProgress(int ndx, int total)
        {
            int percentage = (int) ((double) ndx/total*100);

            if (ProgressBar.InvokeRequired)
            {
                SetTextCallback updateProgressDelegate = UpdateProgressBar;
                ProgressBar.Invoke(updateProgressDelegate, percentage);
            }
            else
            {
                UpdateProgressBar(percentage);
            }
        }

        private void UpdateProgressBar(int percentage)
        {
            ProgressBar.Value = percentage;
        }

        private void CreateCsBusinessLayers()
        {
            foreach (string objectName in Objects.Split(';'))
            {
                UpdateProgress(_progressNdx, Objects.Split(';').Length * 2);
                _progressNdx++;

                var assembly = Assembly.GetExecutingAssembly();
                var businessLayerTemplate = new StringBuilder();

                using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.BusinessLayer.SqlServerCSharp.txt"))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            businessLayerTemplate.Append(reader.ReadToEnd());
                        }
                    }
                }

                string objName = objectName.Trim();
                List<Field> fields = MapFields(objName);
                bool isView = IsView(objName);

                if (!HasPrimaryKey(fields) && !isView)
                {
                    continue;
                }

                using (StreamWriter sw = File.CreateText(OutputDirectory + "\\" + objName.ToProperFileName() + "Business.cs"))
                {
                    var enumsPart = new StringBuilder();
                    foreach (Field field in fields)
                    {
                        if (!string.IsNullOrEmpty(field.Description))
                        {
                            enumsPart.Append("            /// <summary>" + Environment.NewLine);
                            enumsPart.Append("            /// " + field.Description + Environment.NewLine);
                            enumsPart.Append("            /// </summary>" + Environment.NewLine);
                        }
                        enumsPart.Append("            " + field.SafeCsPropertyName + "," + Environment.NewLine);
                    }
                    businessLayerTemplate.Replace("{3}", enumsPart.ToString().TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd(','));

                    businessLayerTemplate.Replace("{0}", Common.GetSafeCsName(Common.GetCsPropertyName(objName)));
                    businessLayerTemplate.Replace("{99}", objName);
                    if (!isView)
                    {
                        businessLayerTemplate.Replace("{1}",fields.First(z => z.IsPrimaryKey).CsDataType);
                    }

                    if (isView)
                    {
                        RemoveTemplateComments(ref businessLayerTemplate);
                    }
                    else
                    {
                        businessLayerTemplate.Replace("{/*}", "");
                        businessLayerTemplate.Replace("{*/}", "");
                    }

                    var fkBusinessGetBy = new StringBuilder();
                    if (!isView)
                    {
                        List<ForeignKey> keys = GetForeignKeys(objName);
                        
                        foreach (ForeignKey key in keys)
                        {
                            fkBusinessGetBy.Append("        public void GetBy" + Common.GetSafeCsName(Common.GetCsPropertyName(key.ForeignColumnName)) + "(");
                            foreach (Field f in fields)
                            {
                                if (String.Equals(f.FieldName, key.ForeignColumnName, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    fkBusinessGetBy.Append(f.CsDataType + " fkId)" + Environment.NewLine);
                                    break;
                                }
                            }
                            fkBusinessGetBy.Append("        {" + Environment.NewLine);
                            fkBusinessGetBy.Append("            DataTable dt = " + DataNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".GetBy" + Common.GetSafeCsName(Common.GetCsPropertyName(key.ForeignColumnName)) + "(fkId);" + Environment.NewLine);
                            fkBusinessGetBy.Append("            if (dt != null)" + Environment.NewLine);
                            fkBusinessGetBy.Append("            {" + Environment.NewLine);
                            fkBusinessGetBy.Append("                Load(dt.Rows);" + Environment.NewLine);
                            fkBusinessGetBy.Append("            }" + Environment.NewLine);
                            fkBusinessGetBy.Append("        }" + Environment.NewLine);
                        }
                    }
                    businessLayerTemplate.Replace("{2}", fkBusinessGetBy.ToString());

                    businessLayerTemplate.Replace("{26}", DataNamespaceName);
                    businessLayerTemplate.Replace("{27}", BusinessNamespaceName);

                    var serializationCode = new StringBuilder();

                    if (AllowSerialization)
                    {
                        serializationCode.AppendLine("        /// <summary>");
                        serializationCode.AppendLine("        /// Creates an instance of " + Common.GetCsPropertyName(objName) + "s from a base64 encoded BSON string");
                        serializationCode.AppendLine("        /// </summary>");
                        serializationCode.AppendLine("        /// <param name=\"bson\">The base64 encoded BSON string</param>");
                        serializationCode.AppendLine("        /// <returns>A " + Common.GetCsPropertyName(objName) + "s object instance</returns>");
                        serializationCode.AppendLine("        public static " + Common.GetCsPropertyName(objName) + "s FromBson(string bson)");
                        serializationCode.AppendLine("        {");
                        serializationCode.AppendLine("            List<" + DataNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Serializable" + Common.GetCsPropertyName(objName) + "> zc;");
                        serializationCode.AppendLine("            byte[] data = Convert.FromBase64String(bson);");
                        serializationCode.AppendLine("            var tmp = new " + Common.GetCsPropertyName(objName) + "s();");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            using (var ms = new System.IO.MemoryStream(data))");
                        serializationCode.AppendLine("            {");
                        serializationCode.AppendLine("                using (var reader = new Newtonsoft.Json.Bson.BsonReader(ms))");
                        serializationCode.AppendLine("                {");
                        serializationCode.AppendLine("                    reader.ReadRootValueAsArray = true;");
                        serializationCode.AppendLine("                    var serializer = new Newtonsoft.Json.JsonSerializer();");
                        serializationCode.AppendLine("                    zc = serializer.Deserialize<List<" + DataNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Serializable" + Common.GetCsPropertyName(objName) + ">>(reader);");
                        serializationCode.AppendLine("                }");
                        serializationCode.AppendLine("            }");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            foreach (" + DataNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Serializable" + Common.GetCsPropertyName(objName) + " z in zc)");
                        serializationCode.AppendLine("            {");
                        serializationCode.AppendLine("                tmp.Add(" + Common.GetCsPropertyName(objName) + ".FromJson(Newtonsoft.Json.JsonConvert.SerializeObject(z)));");
                        serializationCode.AppendLine("            }");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            return tmp;");
                        serializationCode.AppendLine("        }");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("        /// <summary>");
                        serializationCode.AppendLine("        /// Creates an instance of " + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + "s from an XML string");
                        serializationCode.AppendLine("        /// </summary>");
                        serializationCode.AppendLine("        /// <param name=\"xml\">The XML string</param>");
                        serializationCode.AppendLine("        /// <returns>A " + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + "s object instance</returns>");
                        serializationCode.AppendLine("        public static " + Common.GetCsPropertyName(objName) + "s FromXml(string xml)");
                        serializationCode.AppendLine("        {");
                        serializationCode.AppendLine("            var xType = new System.Xml.Serialization.XmlSerializer(typeof(List<" + DataNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Serializable" + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ">));");
                        serializationCode.AppendLine("            List<" + DataNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Serializable" + Common.GetCsPropertyName(objName) + "> zc;");
                        serializationCode.AppendLine("            var tmp = new " + Common.GetCsPropertyName(objName) + "s();");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            using (var sr = new System.IO.StringReader(xml))");
                        serializationCode.AppendLine("            {");
                        serializationCode.AppendLine("                zc = (List<" + DataNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Serializable" + Common.GetCsPropertyName(objName) + ">)xType.Deserialize(sr);");
                        serializationCode.AppendLine("            }");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            foreach (" + DataNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Serializable" + Common.GetCsPropertyName(objName) + " z in zc)");
                        serializationCode.AppendLine("            {");
                        serializationCode.AppendLine("                tmp.Add(" + Common.GetCsPropertyName(objName) + ".FromJson(Newtonsoft.Json.JsonConvert.SerializeObject(z)));");
                        serializationCode.AppendLine("            }");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            return tmp;");
                        serializationCode.AppendLine("        }");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("        /// <summary>");
                        serializationCode.AppendLine("        /// Creates an instance of " + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + "s from a JSON string");
                        serializationCode.AppendLine("        /// </summary>");
                        serializationCode.AppendLine("        /// <param name=\"json\">The JSON string</param>");
                        serializationCode.AppendLine("        /// <returns>A " + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + "s object instance</returns>");
                        serializationCode.AppendLine("        public static " + Common.GetCsPropertyName(objName) + "s FromJson(string json)");
                        serializationCode.AppendLine("        {");
                        serializationCode.AppendLine("            List<" + DataNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Serializable" + Common.GetCsPropertyName(objName) + "> zs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<" + DataNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Serializable" + Common.GetCsPropertyName(objName) + ">>(json);");
                        serializationCode.AppendLine("            var tmp = new " + Common.GetCsPropertyName(objName) + "s();");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            foreach (" + DataNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Serializable" + Common.GetCsPropertyName(objName) + " z in zs)");
                        serializationCode.AppendLine("            {");
                        serializationCode.AppendLine("                tmp.Add(" + Common.GetCsPropertyName(objName) + ".FromJson(Newtonsoft.Json.JsonConvert.SerializeObject(z)));");
                        serializationCode.AppendLine("            }");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            return tmp;");
                        serializationCode.AppendLine("        }");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("        /// <summary>");
                        serializationCode.AppendLine("        /// Converts an instance of an object to a string format");
                        serializationCode.AppendLine("        /// </summary>");
                        serializationCode.AppendLine("        /// <param name=\"format\">Specifies if it should convert to XML, BSON or JSON</param>");
                        serializationCode.AppendLine("        /// <returns>The object, converted to a string representation</returns>");
                        serializationCode.AppendLine("        public string ToString(SerializationFormats format)");
                        serializationCode.AppendLine("        {");
                        serializationCode.AppendLine("            var zs = new List<" + DataNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + "." + Common.GetSafeCsName("Serializable" + Common.GetCsPropertyName(objName)) + ">();");
                        serializationCode.AppendLine("            foreach (" + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + " z in this)");
                        serializationCode.AppendLine("            {");
                        serializationCode.AppendLine("                var " + Common.GetSafeCsName("serializable" + Common.GetCsPropertyName(objName)) + " = new " + DataNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + "." + Common.GetSafeCsName("Serializable" + Common.GetCsPropertyName(objName)) + "();");
                        foreach (var field in fields.OrderByDescending(z => z.IsPrimaryKey))
                        {
                            if (field.IsValueType)
                            {
                                serializationCode.AppendLine("            " + Common.GetSafeCsName("serializable" + Common.GetCsPropertyName(objName)) + "." + field.SafeCsPropertyName + " = z.IsNull(" + Common.GetCsPropertyName(objName) + ".Fields." + field.SafeCsPropertyName + ")");
                                serializationCode.AppendLine("                ? (" + field.CsDataType + "?) null : z." + field.SafeCsPropertyName + ";");
                            }
                            else if (field.CsDataType == "string")
                            {
                                serializationCode.AppendLine("            " + Common.GetSafeCsName("serializable" + Common.GetCsPropertyName(objName)) + "." + field.SafeCsPropertyName + " = z.IsNull(" + Common.GetCsPropertyName(objName) + ".Fields." + field.SafeCsPropertyName + ")");
                                serializationCode.AppendLine("                ? null : z." + field.SafeCsPropertyName + ";");
                            }
                            else
                            {
                                serializationCode.AppendLine("            " + Common.GetSafeCsName("serializable" + Common.GetCsPropertyName(objName)) + "." + field.SafeCsPropertyName + " = z." + field.SafeCsPropertyName + ";");
                            }
                        }
                        serializationCode.AppendLine("            " + Common.GetSafeCsName("serializable" + Common.GetCsPropertyName(objName)) + ".SerializationIsUpdate = z.IsUpdate();");
                        serializationCode.AppendLine("                zs.Add(" + Common.GetSafeCsName("serializable" + Common.GetCsPropertyName(objName)) + ");");
                        serializationCode.AppendLine("            }");
                        serializationCode.AppendLine("            ");
                        serializationCode.AppendLine("            if (format == SerializationFormats.Json)");
                        serializationCode.AppendLine("            {");
                        serializationCode.AppendLine("                return Newtonsoft.Json.JsonConvert.SerializeObject(zs);");
                        serializationCode.AppendLine("            }");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            if (format == SerializationFormats.Xml)");
                        serializationCode.AppendLine("            {");
                        serializationCode.AppendLine("                var xType = new System.Xml.Serialization.XmlSerializer(zs.GetType());");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("                using (var sw = new System.IO.StringWriter())");
                        serializationCode.AppendLine("                {");
                        serializationCode.AppendLine("                    xType.Serialize(sw, zs);");
                        serializationCode.AppendLine("                    return sw.ToString();");
                        serializationCode.AppendLine("                }");
                        serializationCode.AppendLine("            }");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            if (format == SerializationFormats.BsonBase64)");
                        serializationCode.AppendLine("            {");
                        serializationCode.AppendLine("                using (var ms = new System.IO.MemoryStream())");
                        serializationCode.AppendLine("                {");
                        serializationCode.AppendLine("                    using (var writer = new Newtonsoft.Json.Bson.BsonWriter(ms))");
                        serializationCode.AppendLine("                    {");
                        serializationCode.AppendLine("                        var serializer = new Newtonsoft.Json.JsonSerializer();");
                        serializationCode.AppendLine("                        serializer.Serialize(writer, zs);");
                        serializationCode.AppendLine("                    }");
                        serializationCode.AppendLine("                    return Convert.ToBase64String(ms.ToArray());");
                        serializationCode.AppendLine("                }");
                        serializationCode.AppendLine("            }");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            return \"\";");
                        serializationCode.AppendLine("        }");
                    }

                    businessLayerTemplate.Replace("{33}", serializationCode.ToString());

                    serializationCode.Clear();

                    if (AllowSerialization)
                    {
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("        /// <summary>");
                        serializationCode.AppendLine("        /// Creates an instance of " + Common.GetCsPropertyName(objName) + " from a JSON string");
                        serializationCode.AppendLine("        /// </summary>");
                        serializationCode.AppendLine("        /// <param name=\"json\">The JSON string</param>");
                        serializationCode.AppendLine("        /// <returns>A " + Common.GetCsPropertyName(objName) + " object instance</returns>");
                        serializationCode.AppendLine("        public static " + Common.GetCsPropertyName(objName) + " FromJson(string json)");
                        serializationCode.AppendLine("        {");
                        serializationCode.AppendLine("            return JsonTo" + Common.GetCsPropertyName(objName) + "(json);");
                        serializationCode.AppendLine("        }");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("        /// <summary>");
                        serializationCode.AppendLine("        /// Creates an instance of " + Common.GetCsPropertyName(objName) + " from an XML string");
                        serializationCode.AppendLine("        /// </summary>");
                        serializationCode.AppendLine("        /// <param name=\"xml\">The XML string</param>");
                        serializationCode.AppendLine("        /// <returns>A " + Common.GetCsPropertyName(objName) + " object instance</returns>");
                        serializationCode.AppendLine("        public static " + Common.GetCsPropertyName(objName) + " FromXml(string xml)");
                        serializationCode.AppendLine("        {");
                        serializationCode.AppendLine("            return XmlTo" + Common.GetCsPropertyName(objName) + "(xml);");
                        serializationCode.AppendLine("        }");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("        /// <summary>");
                        serializationCode.AppendLine("        /// Creates an instance of " + Common.GetCsPropertyName(objName) + " from a base64 encoded BSON string");
                        serializationCode.AppendLine("        /// </summary>");
                        serializationCode.AppendLine("        /// <param name=\"bson\">The base64 encoded BSON string</param>");
                        serializationCode.AppendLine("        /// <returns>A " + Common.GetCsPropertyName(objName) + " object instance</returns>");
                        serializationCode.AppendLine("        public static " + Common.GetCsPropertyName(objName) + " FromBson(string bson)");
                        serializationCode.AppendLine("        {");
                        serializationCode.AppendLine("            return BsonTo" + Common.GetCsPropertyName(objName) + "(bson);");
                        serializationCode.AppendLine("        }");
                    }

                    businessLayerTemplate.Replace("{34}", serializationCode.ToString());

                    Common.DoComments(ref businessLayerTemplate, "//", IncludeComments);
                    sw.Write(businessLayerTemplate.ToString());
                }
            }
        }

        private void CreateVbBusinessLayers()
        {
            foreach (string objectName in Objects.Split(';'))
            {
                UpdateProgress(_progressNdx, Objects.Split(';').Length * 2);
                _progressNdx++;

                var assembly = Assembly.GetExecutingAssembly();
                var businessLayerTemplate = new StringBuilder();

                using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.BusinessLayer.SqlServerVbNet.txt"))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            businessLayerTemplate.Append(reader.ReadToEnd());
                        }
                    }
                }

                string objName = objectName.Trim();
                List<Field> fields = MapFields(objName);
                bool isView = IsView(objName);
                if (!HasPrimaryKey(fields) && !isView)
                {
                    continue;
                }

                using (StreamWriter sw = File.CreateText(OutputDirectory + "\\" + objName.ToProperFileName() + "Business.vb"))
                {
                    var enumsPart = new StringBuilder();
                    foreach (Field field in fields)
                    {
                        if (!string.IsNullOrEmpty(field.Description))
                        {
                            enumsPart.Append("            ''' <summary>" + Environment.NewLine);
                            enumsPart.Append("            ''' " + field.Description + Environment.NewLine);
                            enumsPart.Append("            ''' </summary>" + Environment.NewLine);
                        }
                        enumsPart.Append("            " + field.SafeVbPropertyName + Environment.NewLine);
                    }
                    businessLayerTemplate.Replace("{3}", enumsPart.ToString());

                    businessLayerTemplate.Replace("{0}", Common.GetSafeVbName(Common.GetVbPropertyName(objName)));
                    businessLayerTemplate.Replace("{99}", objName);
                    if (!isView)
                    {
                        businessLayerTemplate.Replace("{1}", fields.First(z => z.IsPrimaryKey).VbDataType);
                    }

                    if (isView)
                    {
                        RemoveTemplateComments(ref businessLayerTemplate);
                    }
                    else
                    {
                        businessLayerTemplate.Replace("{/*}", "");
                        businessLayerTemplate.Replace("{*/}", "");
                    }

                    var fkBusinessGetBy = new StringBuilder();
                    if (!isView)
                    {
                        List<ForeignKey> keys = GetForeignKeys(objName);

                        foreach (ForeignKey key in keys)
                        {
                            fkBusinessGetBy.Append("        Public Sub GetBy" + Common.GetSafeVbName(Common.GetVbPropertyName(key.ForeignColumnName)) + "(fkId As ");
                            foreach (Field f in fields)
                            {
                                if (String.Equals(f.FieldName, key.ForeignColumnName, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    fkBusinessGetBy.Append(f.VbDataType + ")" + Environment.NewLine);
                                    break;
                                }
                            }

                            fkBusinessGetBy.Append("            Dim dt As DataTable = " + DataNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".GetBy" + Common.GetSafeVbName(Common.GetVbPropertyName(key.ForeignColumnName)) + "(fkId)" + Environment.NewLine);
                            fkBusinessGetBy.Append("            If dt IsNot Nothing Then" + Environment.NewLine);
                            fkBusinessGetBy.Append("                Load(dt.Rows)" + Environment.NewLine);
                            fkBusinessGetBy.Append("            End If" + Environment.NewLine);
                            fkBusinessGetBy.Append("        End Sub" + Environment.NewLine);
                        }
                    }

                    businessLayerTemplate.Replace("{2}", fkBusinessGetBy.ToString());

                    businessLayerTemplate.Replace("{26}", DataNamespaceName);
                    businessLayerTemplate.Replace("{27}", BusinessNamespaceName);
                    businessLayerTemplate.Replace("{100}", DataNamespaceName);
                    businessLayerTemplate.Replace("{101}", BusinessNamespaceName);
                    businessLayerTemplate.Replace("{102}", EventLogNamespaceName);

                    var serializationCode = new StringBuilder();

                    if (AllowSerialization)
                    {
                        serializationCode.AppendLine("        ''' <summary>");
                        serializationCode.AppendLine("        ''' Creates an instance of " + Common.GetVbPropertyName(objName) + "s from a base64 encoded BSON string");
                        serializationCode.AppendLine("        ''' </summary>");
                        serializationCode.AppendLine("        ''' <param name=\"bson\">The base64 encoded BSON string</param>");
                        serializationCode.AppendLine("        ''' <returns>A " + Common.GetVbPropertyName(objName) + "s object instance</returns>");
                        serializationCode.AppendLine("        Public Shared Function FromBson(bson As String) As " + Common.GetVbPropertyName(objName) + "s");
                        serializationCode.AppendLine("            Dim zc As List(Of " + DataNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Serializable" + Common.GetVbPropertyName(objName) + ")");
                        serializationCode.AppendLine("            Dim data As Byte() = Convert.FromBase64String(bson)");
                        serializationCode.AppendLine("            Dim tmp As New " + Common.GetVbPropertyName(objName) + "s()");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            Using ms As New System.IO.MemoryStream(data)");
                        serializationCode.AppendLine("                Using reader As New Newtonsoft.Json.Bson.BsonReader(ms)");
                        serializationCode.AppendLine("                    reader.ReadRootValueAsArray = True");
                        serializationCode.AppendLine("                    Dim serializer As New Newtonsoft.Json.JsonSerializer()");
                        serializationCode.AppendLine("                    zc = serializer.Deserialize(Of List(Of " + DataNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Serializable" + Common.GetVbPropertyName(objName) + "))(reader)");
                        serializationCode.AppendLine("                End Using");
                        serializationCode.AppendLine("            End Using");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            For Each z As " + DataNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Serializable" + Common.GetVbPropertyName(objName) + " In zc");
                        serializationCode.AppendLine("                tmp.Add(" + Common.GetVbPropertyName(objName) + ".FromJson(Newtonsoft.Json.JsonConvert.SerializeObject(z)))");
                        serializationCode.AppendLine("            Next");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            Return tmp");
                        serializationCode.AppendLine("        End Function");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("        ''' <summary>");
                        serializationCode.AppendLine("        ''' Creates an instance of " + Common.GetVbPropertyName(objName) + "s from an XML string");
                        serializationCode.AppendLine("        ''' </summary>");
                        serializationCode.AppendLine("        ''' <param name=\"xml\">The XML string</param>");
                        serializationCode.AppendLine("        ''' <returns>A " + Common.GetVbPropertyName(objName) + "s object instance</returns>");
                        serializationCode.AppendLine("        Public Shared Function FromXml(xml As String) As " + Common.GetVbPropertyName(objName) + "s");
                        serializationCode.AppendLine("            Dim xType As New System.Xml.Serialization.XmlSerializer(GetType(List(Of " + DataNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Serializable" + Common.GetVbPropertyName(objName) + ")))");
                        serializationCode.AppendLine("            Dim zc As List(Of " + DataNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Serializable" + Common.GetVbPropertyName(objName) + ")");
                        serializationCode.AppendLine("            Dim tmp As New " + Common.GetVbPropertyName(objName) + "s()");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            Using sr As New System.IO.StringReader(xml)");
                        serializationCode.AppendLine("                zc = DirectCast(xType.Deserialize(sr), List(Of " + DataNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Serializable" + Common.GetVbPropertyName(objName) + "))");
                        serializationCode.AppendLine("            End Using");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            For Each z As " + DataNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Serializable" + Common.GetVbPropertyName(objName) + " In zc");
                        serializationCode.AppendLine("                tmp.Add(" + Common.GetVbPropertyName(objName) + ".FromJson(Newtonsoft.Json.JsonConvert.SerializeObject(z)))");
                        serializationCode.AppendLine("            Next");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            Return tmp");
                        serializationCode.AppendLine("        End Function");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("        ''' <summary>");
                        serializationCode.AppendLine("        ''' Creates an instance of " + Common.GetVbPropertyName(objName) + "s from a JSON string");
                        serializationCode.AppendLine("        ''' </summary>");
                        serializationCode.AppendLine("        ''' <param name=\"json\">The JSON string</param>");
                        serializationCode.AppendLine("        ''' <returns>A " + Common.GetVbPropertyName(objName) + "s object instance</returns>");
                        serializationCode.AppendLine("        Public Shared Function FromJson(json As String) As " + Common.GetVbPropertyName(objName) + "s");
                        serializationCode.AppendLine("            Dim zs As List(Of " + DataNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Serializable" + Common.GetVbPropertyName(objName) + ") = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of " + DataNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + ".Serializable" + Common.GetVbPropertyName(objName) + "))(json)");
                        serializationCode.AppendLine("            Dim tmp As New " + Common.GetVbPropertyName(objName) + "s()");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            For Each z As " + DataNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Serializable" + Common.GetVbPropertyName(objName) + " In zs");
                        serializationCode.AppendLine("                tmp.Add(" + Common.GetVbPropertyName(objName) + ".FromJson(Newtonsoft.Json.JsonConvert.SerializeObject(z)))");
                        serializationCode.AppendLine("            Next");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            Return tmp");
                        serializationCode.AppendLine("        End Function");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("        ''' <summary>");
                        serializationCode.AppendLine("        ''' Converts an instance of an object to a string format");
                        serializationCode.AppendLine("        ''' </summary>");
                        serializationCode.AppendLine("        ''' <param name=\"format\">Specifies if it should convert to XML or JSON</param>");
                        serializationCode.AppendLine("        ''' <returns>The object, converted to a string representation</returns>");
                        serializationCode.AppendLine("        Public Function ToString(format As SerializationFormats) As String");
                        serializationCode.AppendLine("            Dim zs As New List(Of " + DataNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + "." + Common.GetSafeVbName("Serializable" + Common.GetVbPropertyName(objName)) + ")()");
                        serializationCode.AppendLine("            For Each z As " + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + " In Me");
                        serializationCode.AppendLine("                Dim " + Common.GetSafeVbName("serializable" + Common.GetVbPropertyName(objName)) + " As new " + DataNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + "." + Common.GetSafeVbName("Serializable" + Common.GetVbPropertyName(objName)) + "()");
                        foreach (var field in fields.OrderByDescending(z => z.IsPrimaryKey))
                        {
                            if (field.IsValueType)
                            {
                                serializationCode.AppendLine("            " + Common.GetSafeVbName("serializable" + Common.GetVbPropertyName(objName)) + "." + field.SafeVbPropertyName + " = If(z.IsNull(" + Common.GetSafeVbName(objName) + ".Fields." + field.SafeVbPropertyName + "), DirectCast(Nothing, System.Nullable(Of " + field.VbDataType + ")), z." + field.SafeVbPropertyName + ")");
                            }
                            else if (field.CsDataType == "string")
                            {
                                serializationCode.AppendLine("            " + Common.GetSafeVbName("serializable" + Common.GetVbPropertyName(objName)) + "." + field.SafeVbPropertyName + " = If(z.IsNull(" + Common.GetSafeVbName(objName) + ".Fields." + field.SafeVbPropertyName + "), Nothing, z." + field.SafeVbPropertyName + ")");
                            }
                            else
                            {
                                serializationCode.AppendLine("            " + Common.GetSafeVbName("serializable" + Common.GetVbPropertyName(objName)) + "." + field.SafeVbPropertyName + " = z." + field.SafeVbPropertyName);
                            }
                        }
                        serializationCode.AppendLine("            " + Common.GetSafeCsName("serializable" + Common.GetCsPropertyName(objName)) + ".SerializationIsUpdate = z.IsUpdate()");
                        serializationCode.AppendLine("                zs.Add(" + Common.GetSafeVbName("serializable" + Common.GetVbPropertyName(objName)) + ")");
                        serializationCode.AppendLine("            Next");
                        serializationCode.AppendLine("            ");
                        serializationCode.AppendLine("            If format = SerializationFormats.Json Then");
                        serializationCode.AppendLine("                Return Newtonsoft.Json.JsonConvert.SerializeObject(zs)");
                        serializationCode.AppendLine("            End If");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            If format = SerializationFormats.Xml Then");
                        serializationCode.AppendLine("                Dim xType As New System.Xml.Serialization.XmlSerializer(zs.GetType())");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("                Using sw As New System.IO.StringWriter()");
                        serializationCode.AppendLine("                    xType.Serialize(sw, zs)");
                        serializationCode.AppendLine("                    Return sw.ToString()");
                        serializationCode.AppendLine("                End Using");
                        serializationCode.AppendLine("            End If");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            If format = SerializationFormats.BsonBase64 Then");
                        serializationCode.AppendLine("                Using ms As New System.IO.MemoryStream");
                        serializationCode.AppendLine("                    Using writer As New Newtonsoft.Json.Bson.BsonWriter(ms)");
                        serializationCode.AppendLine("                        Dim serializer As New Newtonsoft.Json.JsonSerializer()");
                        serializationCode.AppendLine("                        serializer.Serialize(writer, zs)");
                        serializationCode.AppendLine("                    End Using");
                        serializationCode.AppendLine("                    Return Convert.ToBase64String(ms.ToArray())");
                        serializationCode.AppendLine("                End Using");
                        serializationCode.AppendLine("            End If");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            Return \"\"");
                        serializationCode.AppendLine("        End Function");
                    }

                    businessLayerTemplate.Replace("{33}", serializationCode.ToString());

                    serializationCode.Clear();

                    if (AllowSerialization)
                    {
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("        ''' <summary>");
                        serializationCode.AppendLine("        ''' Creates an instance of " + Common.GetVbPropertyName(objName) + " from a JSON string");
                        serializationCode.AppendLine("        ''' </summary>");
                        serializationCode.AppendLine("        ''' <param name=\"json\">The JSON string</param>");
                        serializationCode.AppendLine("        ''' <returns>A " + Common.GetVbPropertyName(objName) + " object instance</returns>");
                        serializationCode.AppendLine("        Public Shared Function FromJson(json As String) As " + Common.GetVbPropertyName(objName));
                        serializationCode.AppendLine("            Return JsonTo" + Common.GetVbPropertyName(objName) + "(json)");
                        serializationCode.AppendLine("        End Function");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("        ''' <summary>");
                        serializationCode.AppendLine("        ''' Creates an instance of " + Common.GetVbPropertyName(objName) + " from an XML string");
                        serializationCode.AppendLine("        ''' </summary>");
                        serializationCode.AppendLine("        ''' <param name=\"xml\">The XML string</param>");
                        serializationCode.AppendLine("        ''' <returns>A " + Common.GetVbPropertyName(objName) + " object instance</returns>");
                        serializationCode.AppendLine("        Public Shared Function FromXml(xml As String) As " + Common.GetVbPropertyName(objName));
                        serializationCode.AppendLine("            Return XmlTo" + Common.GetVbPropertyName(objName) + "(xml)");
                        serializationCode.AppendLine("        End Function");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("        ''' <summary>");
                        serializationCode.AppendLine("        ''' Creates an instance of " + Common.GetVbPropertyName(objName) + " from a Base64 Encoded BSON string");
                        serializationCode.AppendLine("        ''' </summary>");
                        serializationCode.AppendLine("        ''' <param name=\"bson\">The Base64 Encoded BSON string</param>");
                        serializationCode.AppendLine("        ''' <returns>A " + Common.GetVbPropertyName(objName) + " object instance</returns>");
                        serializationCode.AppendLine("        Public Shared Function FromBson(bson As String) As " + Common.GetVbPropertyName(objName));
                        serializationCode.AppendLine("            Return BsonTo" + Common.GetVbPropertyName(objName) + "(bson)");
                        serializationCode.AppendLine("        End Function");
                    }

                    businessLayerTemplate.Replace("{34}", serializationCode.ToString());

                    Common.DoComments(ref businessLayerTemplate, "'", IncludeComments);
                    sw.Write(businessLayerTemplate.ToString());
                }
            }
        }

        private void CreateCsDataLayers()
        {
            if (!Directory.Exists(OutputDirectory + "\\Business"))
            {
                Directory.CreateDirectory(OutputDirectory + "\\Business");
            }
            var storedProcedures = new StringBuilder();

            foreach (string objectName in Objects.Split(';'))
            {
                UpdateProgress(_progressNdx, Objects.Split(';').Length*2);
                _progressNdx++;
                var assembly = Assembly.GetExecutingAssembly();
                StringBuilder dataLayerTemplate = new StringBuilder();
                
                using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.DataLayer.SqlServerCSharp.txt"))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            dataLayerTemplate.Append(reader.ReadToEnd());
                        }
                    }
                }

                string objName = objectName.Trim();
                bool isView = IsView(objName);

                List<Field> fields = MapFields(objName);

                if (!HasPrimaryKey(fields) && !isView)
                {
                    continue;
                }
                string objNameWithSuffix = Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + DataObjectSuffix;//"Obj";
                using (StreamWriter sw = File.CreateText(OutputDirectory + "\\Business\\" + objNameWithSuffix + ".cs"))
                {
                    storedProcedures.Append(CreateStoredProcedures(Common.GetSafeCsName(Common.GetCsPropertyName(objName)), fields, isView));

                    dataLayerTemplate.Replace("{1}", Common.GetSafeCsName(Common.GetCsPropertyName(objName)));
                    dataLayerTemplate.Replace("{5}", objNameWithSuffix);

                    var fieldsPart = new StringBuilder();
                    foreach (var field in fields)
                    {
                        if (field.CanBeNull && field.CsDataType != "string" && !field.CsDataType.Contains("[]"))
                        {
                            fieldsPart.AppendLine("        private Nullable<" + field.CsDataType + "> " + field.SafeCsFieldName + ";");
                        }
                        else
                        {
                            fieldsPart.AppendLine("        private " + field.CsDataType + " " + field.SafeCsFieldName + ";");
                        }
                    }
                    dataLayerTemplate.Replace("{2}", fieldsPart.ToString());

                    var propertiesPart = new StringBuilder();
                    string variablesPart = "";
                    var initDatasPart = new StringBuilder();
                    foreach (Field field in fields.OrderByDescending(z => z.IsPrimaryKey))
                    {
                        if (!string.IsNullOrEmpty(field.Description))
                        {
                            propertiesPart.Append("        /// <summary>" + Environment.NewLine);
                            propertiesPart.Append("        /// " + field.Description + Environment.NewLine);
                            propertiesPart.Append("        /// </summary>" + Environment.NewLine);
                        }
                        if (field.CanBeNull && field.CsDataType != "string" && !field.CsDataType.Contains("[]"))
                        {
                            propertiesPart.Append("        public virtual " + field.CsDataType + " ? " + field.SafeCsPropertyName + Environment.NewLine);
                        }
                        else
                        {
                            propertiesPart.Append("        public virtual " + field.CsDataType + " " + field.SafeCsPropertyName + Environment.NewLine);
                        }
                        propertiesPart.Append("        {" + Environment.NewLine);
                        propertiesPart.Append("            get { return " + field.SafeCsFieldName + "; }" + Environment.NewLine);

                        if (field.IsIdentity && (!isView))
                        {
                            propertiesPart.Append("            set { " + field.SafeCsFieldName + " = value; }" + Environment.NewLine);
                        } else if ((!field.IsComputedField) && (!isView))
                        {
                            if (field.CsDataType == "DateTime")
                                propertiesPart.Append("            set { " + field.SafeCsFieldName + " = value; }" + Environment.NewLine);//if(value == DateTime.MinValue) SetNull(" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + "); else UnsetNull(" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + "); }
                            else if (field.CsDataType == "string")
                                propertiesPart.Append("            set { " + field.SafeCsFieldName + " = value; }" + Environment.NewLine); //if(value == null) SetNull(" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + "); else UnsetNull(" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + "); }
                            else propertiesPart.Append("            set { " + field.SafeCsFieldName + " = value; }" + Environment.NewLine); // UnsetNull(" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + "); }
                        }

                        if (field.IsComputedField || isView)
                        {
                            propertiesPart.Append("            set { " + field.SafeCsFieldName + " = value; }" + Environment.NewLine);
                        }
                        propertiesPart.Append("        }" + Environment.NewLine + Environment.NewLine);

                        string variableName = field.SafeCsFieldName.Substring(1);
                        if (string.Compare(variableName, "uiID", true) == 0)
                        {
                            variableName = "id";
                        }
                        if (!string.IsNullOrEmpty(variablesPart))
                            variablesPart += ", ";
                        variablesPart += field.CsDataType;
                        variablesPart += " ";
                        variablesPart += variableName;
                        initDatasPart.AppendLine("			" + field.SafeCsFieldName + " = " + variableName + ";");
                    }

                    dataLayerTemplate.Replace("{7}", propertiesPart.ToString());
                    dataLayerTemplate.Replace("{15}", variablesPart);
                    dataLayerTemplate.Replace("{16}", initDatasPart.ToString());

                    string pkCsName = fields.First(z => z.IsPrimaryKey).SafeCsFieldName;
                    string strKeyFieldName = fields.First(z => z.IsPrimaryKey).SafeCsPropertyName;
                    Field PrimaryKeyField = fields.FirstOrDefault(z => z.IsPrimaryKey);
                    if (PrimaryKeyField != null && PrimaryKeyField.IsIdentity)
                    {
                        if (PrimaryKeyField.CsDataType == "string")
                        {
                            dataLayerTemplate.Replace("{10}", string.Format("{0}==\"\"", pkCsName));
                            dataLayerTemplate.Replace("{11}", string.Format("{0}=\"\";", pkCsName));
                        }
                        else
                        {
                            dataLayerTemplate.Replace("{10}", string.Format("{0} == int.MinValue", pkCsName));
                            dataLayerTemplate.Replace("{11}", string.Format("{0} = int.MinValue;", pkCsName));
                        }
                    }
                    else
                    {
                        dataLayerTemplate.Replace("{10}", "false");
                        dataLayerTemplate.Replace("{11}", "");
                    }

                    dataLayerTemplate.Replace("{9}", strKeyFieldName);
                    dataLayerTemplate.Replace("{26}", BusinessNamespaceName);
                    dataLayerTemplate.Replace("{100}", DataNamespaceName);
                    dataLayerTemplate.Replace("{101}", BusinessNamespaceName);
                    dataLayerTemplate.Replace("{102}", EventLogNamespaceName);

                    var objcopyPart = new StringBuilder();
                    string strObjName = Common.GetSafeCsName(Common.GetCsPropertyName(objName));
                    string strObjNameL = strObjName.Substring(0, 1).ToLower() + strObjName.Substring(1, strObjName.Length - 1);
                    string strObjNameCopy = strObjNameL + "Copy";
                    objcopyPart.AppendLine(string.Format("I{0} {1} = new {2}();", strObjName, strObjNameCopy, objNameWithSuffix));
                    foreach (var field in fields)
                    {
                        objcopyPart.AppendLine("            " + strObjNameCopy + "." + field.SafeCsPropertyName + " = this." + field.SafeCsPropertyName + ";"); //field.SafeCsPropertyName
                    }
                    objcopyPart.AppendLine(string.Format(Environment.NewLine + "            return {0};", strObjNameCopy));

                    dataLayerTemplate.Replace("{3}", objcopyPart.ToString());
                    dataLayerTemplate.Replace("{8}", strObjNameL);

                    string strObjNameDto = strObjNameL + "Dto";
                    var objSavePart = new StringBuilder();
                    if (PrimaryKeyField != null && PrimaryKeyField.IsIdentity)
                    {
                        objSavePart.AppendLine("            bool isNew = IsNew;");
                        objSavePart.AppendLine(string.Format("            using (var repo = new {0}Repository())", Common.GetSafeCsName(Common.GetCsPropertyName(objName))));
                        objSavePart.AppendLine("            {");
                        objSavePart.AppendLine("                if (IsNew)");
                        objSavePart.AppendLine("                {");
                        objSavePart.AppendLine(string.Format("                    var {1} = new {0}Dto()", strObjName, strObjNameDto));
                        objSavePart.AppendLine("                    {");
                        foreach (var field in fields)
                        {
                            objSavePart.AppendLine("                        " + field.SafeCsPropertyName + " = this." + field.SafeCsPropertyName + ","); //field.SafeCsPropertyName
                        }
                        objSavePart.AppendLine("                    };");
                        objSavePart.AppendLine(string.Format("                    repo.Add({0});", strObjNameDto));
                        objSavePart.AppendLine("                    repo.Save();");
                        objSavePart.AppendLine("                }");
                        objSavePart.AppendLine("                else");
                        objSavePart.AppendLine("                {");
                        objSavePart.AppendLine(string.Format("                    var {1} = repo.Find({0});", strKeyFieldName, strObjNameDto));
                        objSavePart.AppendLine(string.Format("                    if ({0} != null)", strObjNameDto));
                        objSavePart.AppendLine("                    {");
                        foreach (var field in fields)
                        {
                            if (!field.IsPrimaryKey)
                            {
                                objSavePart.AppendLine("                        " + strObjNameDto + "." + field.SafeCsPropertyName + " = this." + field.SafeCsPropertyName + ";"); //field.SafeCsPropertyName
                            }
                        }
                        objSavePart.AppendLine("						repo.Save();");
                        objSavePart.AppendLine("					}");
                        objSavePart.AppendLine("					else");
                        objSavePart.AppendLine("					{");
                        objSavePart.AppendLine(string.Format("						throw new BusinessException(String.Format(CultureInfo.CurrentCulture, \"Cannot save {0}: No existing {0} with {1} ", strObjNameL, strKeyFieldName) + "{0} was found in the database.\", " + strKeyFieldName + "));");
                        objSavePart.AppendLine("					}");
                        objSavePart.AppendLine("                }");
                        objSavePart.AppendLine("            }");
                        dataLayerTemplate.Replace("{4}", objSavePart.ToString());
                    }
                    else
                    {
                        objSavePart.AppendLine(string.Format("            using (var repo = new {0}Repository())", Common.GetSafeCsName(Common.GetCsPropertyName(objName))));
                        objSavePart.AppendLine("            {");
                        objSavePart.AppendLine(string.Format("                var {1} = repo.Find({0});", strKeyFieldName, strObjNameDto));
                        objSavePart.AppendLine(string.Format("                if ({0} != null)", strObjNameDto));
                        objSavePart.AppendLine("                {");
                        foreach (var field in fields)
                        {
                            if (!field.IsPrimaryKey)
                            {
                                objSavePart.AppendLine("                    " + strObjNameDto + "." + field.SafeCsPropertyName + " = this." + field.SafeCsPropertyName + ";"); //field.SafeCsPropertyName
                            }
                        }
                        objSavePart.AppendLine("                    repo.Save();");
                        objSavePart.AppendLine("                }");
                        objSavePart.AppendLine("                else");
                        objSavePart.AppendLine("                {");
                        objSavePart.AppendLine(string.Format("                    {1} = new {0}Dto()", strObjName, strObjNameDto));
                        objSavePart.AppendLine("                    {");
                        foreach (var field in fields)
                        {
                            objSavePart.AppendLine("                        " + field.SafeCsPropertyName + " = this." + field.SafeCsPropertyName + ","); //field.SafeCsPropertyName
                        }
                        objSavePart.AppendLine("                    };");
                        objSavePart.AppendLine(string.Format("                    repo.Add({0});", strObjNameDto));
                        objSavePart.AppendLine("                    repo.Save();");
                        objSavePart.AppendLine("                }");
                        objSavePart.AppendLine("            }");

                        dataLayerTemplate.Replace("{4}", objSavePart.ToString());
                    }

                    var objDeletePart = new StringBuilder();
                    objDeletePart.AppendLine(string.Format("var {1} = repo.Find({0});", strKeyFieldName, strObjNameDto));
                    objDeletePart.AppendLine(string.Format("				if ({0} != null)", strObjNameDto));
                    objDeletePart.AppendLine("				{");
                    objDeletePart.AppendLine(string.Format("					repo.Delete({0});", strObjNameDto));
                    objDeletePart.AppendLine("					repo.Save();");
                    objDeletePart.AppendLine("				}");
                    dataLayerTemplate.Replace("{6}", objDeletePart.ToString());

                    var objCopyFromPart = new StringBuilder();
                    foreach (var field in fields)
                    {
                        objCopyFromPart.AppendLine("				this." + field.SafeCsPropertyName + " = " + strObjNameDto + "." + field.SafeCsPropertyName + ";"); //field.SafeCsPropertyName
                    }
                    dataLayerTemplate.Replace("{13}", objCopyFromPart.ToString());

                    var objCopyToPart = new StringBuilder();
                    foreach (var field in fields)
                    {
                        objCopyToPart.AppendLine("				" + strObjNameDto + "." + field.SafeCsPropertyName + " = this." + field.SafeCsPropertyName + ";"); //field.SafeCsPropertyName
                    }
                    dataLayerTemplate.Replace("{14}", objCopyToPart.ToString());

                    Common.DoComments(ref dataLayerTemplate, "//", IncludeComments);

                    sw.Write(dataLayerTemplate);
                }
            }
            /*using (StreamWriter sw = File.CreateText(OutputDirectory + "\\StoredProcedureScripts.SQL"))
            {
                sw.Write(storedProcedures.ToString());
            }*/
        }

        private void CreateCsControllerLayers()
        {
            if (!Directory.Exists(OutputDirectory + "\\Controller"))
            {
                Directory.CreateDirectory(OutputDirectory + "\\Controller");
            }
            var storedProcedures = new StringBuilder();

            foreach (string objectName in Objects.Split(';'))
            {
                //UpdateProgress(_progressNdx, Objects.Split(';').Length * 2);
                //_progressNdx++;
                var assembly = Assembly.GetExecutingAssembly();
                StringBuilder dataLayerTemplate = new StringBuilder();

                using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.DataLayer.SqlServerCSharpController.txt"))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            dataLayerTemplate.Append(reader.ReadToEnd());
                        }
                    }
                }

                string objName = objectName.Trim();
                bool isView = IsView(objName);

                List<Field> fields = MapFields(objName);

                if (!HasPrimaryKey(fields) && !isView)
                {
                    continue;
                }
                using (StreamWriter sw = File.CreateText(OutputDirectory + "\\Controller\\" + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + "DDX.cs"))
                {
                    storedProcedures.Append(CreateStoredProcedures(Common.GetSafeCsName(Common.GetCsPropertyName(objName)), fields, isView));

                    dataLayerTemplate.Replace("{1}", Common.GetSafeCsName(Common.GetCsPropertyName(objName)));

                    string pkCsName = fields.First(z => z.IsPrimaryKey).SafeCsFieldName;
                    string strKeyFieldName = fields.First(z => z.IsPrimaryKey).SafeCsPropertyName;
                    dataLayerTemplate.Replace("{26}", BusinessNamespaceName);
                    dataLayerTemplate.Replace("{100}", DataNamespaceName);
                    dataLayerTemplate.Replace("{101}", BusinessNamespaceName);
                    dataLayerTemplate.Replace("{102}", EventLogNamespaceName);

                    string strObjName = Common.GetSafeCsName(Common.GetCsPropertyName(objName));
                    string strObjNameL = strObjName.Substring(0, 1).ToLower() + strObjName.Substring(1, strObjName.Length - 1);
                   
                    dataLayerTemplate.Replace("{8}", strObjNameL);

                    var initDatasPart = new StringBuilder();
                    var initDatasPart1 = new StringBuilder();
                    var initDatasPart2 = new StringBuilder();
                    var initDatasPart3 = new StringBuilder();
                    var initDatasPart4 = new StringBuilder();
                    var initDatasPart5 = new StringBuilder();
                    var initDatasPart6 = new StringBuilder();
                    var initDatasPart7 = new StringBuilder();
                    string preObjName = strObjNameL.Substring(0, 1);
                    foreach (Field field in fields.OrderByDescending(z => z.IsPrimaryKey))
                    {
                        initDatasPart.AppendLine("			" + strObjNameL + "." + field.SafeCsPropertyName + " = " + strObjNameL + "Data." + field.SafeCsPropertyName + ";");
                        initDatasPart1.AppendLine("			" + strObjNameL + "Data." + field.SafeCsPropertyName + " = " + strObjNameL + "." + field.SafeCsPropertyName + ";");
                        initDatasPart2.AppendLine("			" + strObjNameL + "Data." + field.SafeCsPropertyName + " = " + strObjNameL + "Dto." + field.SafeCsPropertyName + ";");
                        initDatasPart3.AppendLine("			" + strObjNameL + "." + field.SafeCsPropertyName + " = " + strObjNameL + "Dto." + field.SafeCsPropertyName + ";");
                        initDatasPart4.AppendLine("			" + strObjNameL + "Dto." + field.SafeCsPropertyName + " = " + strObjNameL + "." + field.SafeCsPropertyName + ";");
                        initDatasPart5.AppendLine("			" + strObjNameL + "Dto." + field.SafeCsPropertyName + " = " + strObjNameL + "Data" + "." + field.SafeCsPropertyName + ";");
                        initDatasPart6.AppendLine("			        " + field.SafeCsPropertyName + " = " + preObjName + "." + field.SafeCsPropertyName + ",");
                        if (!field.IsPrimaryKey && field.SafeCsPropertyName != "InputBy" && field.SafeCsPropertyName != "DateAdded" && field.SafeCsPropertyName != "LastModifiedBy" && field.SafeCsPropertyName != "DateLastModified")
                        {
                            if (initDatasPart7.Length == 0)
                            {
                                initDatasPart7.AppendLine("			if (Original." + field.SafeCsPropertyName + " != Present." + field.SafeCsPropertyName);
                            }
                            else
                            {
                                initDatasPart7.AppendLine("				|| Original." + field.SafeCsPropertyName + " != Present." + field.SafeCsPropertyName);
                            }
                        }
                    }
                    if (initDatasPart7.Length > 0)
                    {
                        initDatasPart7.Insert(initDatasPart7.Length - 2, ")");
                        initDatasPart7.Append("				return true;");
                    }
                    dataLayerTemplate.Replace("{2}", initDatasPart.ToString());
                    dataLayerTemplate.Replace("{3}", initDatasPart1.ToString());
                    dataLayerTemplate.Replace("{6}", initDatasPart2.ToString());
                    dataLayerTemplate.Replace("{4}", initDatasPart3.ToString());
                    dataLayerTemplate.Replace("{5}", initDatasPart4.ToString());
                    dataLayerTemplate.Replace("{7}", initDatasPart5.ToString());
                    dataLayerTemplate.Replace("{9}", initDatasPart6.ToString());
                    dataLayerTemplate.Replace("{10}", preObjName);
                    dataLayerTemplate.Replace("{11}", initDatasPart7.ToString());

                    Common.DoComments(ref dataLayerTemplate, "//", IncludeComments);

                    sw.Write(dataLayerTemplate);
                }
            }
            /*using (StreamWriter sw = File.CreateText(OutputDirectory + "\\StoredProcedureScripts.SQL"))
            {
                sw.Write(storedProcedures.ToString());
            }*/
        }

        private void CreateCsBILayers()
        {
            if (!Directory.Exists(OutputDirectory + "\\Interfaces"))
            {
                Directory.CreateDirectory(OutputDirectory + "\\Interfaces");
            }
            foreach (string objectName in Objects.Split(';'))
            {
                //UpdateProgress(_progressNdx, Objects.Split(';').Length * 2);
                //_progressNdx++;
                var assembly = Assembly.GetExecutingAssembly();
                StringBuilder dataLayerTemplate = new StringBuilder();

                using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.DataLayer.SqlServerCSharpI.txt"))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            dataLayerTemplate.Append(reader.ReadToEnd());
                        }
                    }
                }

                string objName = objectName.Trim();
                bool isView = IsView(objName);

                List<Field> fields = MapFields(objName);

                if (!HasPrimaryKey(fields) && !isView)
                {
                    continue;
                }
                using (StreamWriter sw = File.CreateText(OutputDirectory + "\\Interfaces\\I" + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".cs"))
                {
                    dataLayerTemplate.Replace("{1}", Common.GetSafeCsName(Common.GetCsPropertyName(objName)));

                    var fieldsPart = new StringBuilder();
                    foreach (var field in fields)
                    {
                        if (field.CanBeNull && field.CsDataType != "string" && !field.CsDataType.Contains("[]"))
                        {
                            fieldsPart.AppendLine("        Nullable<" + field.CsDataType + "> " + field.SafeCsPropertyName + " { get; set; }");
                        }
                        else
                        {
                            fieldsPart.AppendLine("        " + field.CsDataType + " " + field.SafeCsPropertyName + " { get; set; }");
                        }
                    }
                    dataLayerTemplate.Replace("{2}", fieldsPart.ToString());

                    string strObjName = Common.GetSafeCsName(Common.GetCsPropertyName(objName));
                    string strObjNameL = strObjName.Substring(0, 1).ToLower() + strObjName.Substring(1, strObjName.Length - 1);
                    dataLayerTemplate.Replace("{8}", strObjNameL);                  

                    string pkCsName = fields.First(z => z.IsPrimaryKey).SafeCsFieldName;
                    string strKeyFieldName = fields.First(z => z.IsPrimaryKey).SafeCsPropertyName;
                    
                    //dataLayerTemplate.Replace("{9}", strKeyFieldName);
                    dataLayerTemplate.Replace("{26}", BusinessNamespaceName + ".Interfaces");
                    dataLayerTemplate.Replace("{100}", DataNamespaceName);
                    dataLayerTemplate.Replace("{101}", BusinessNamespaceName);
                    dataLayerTemplate.Replace("{102}", EventLogNamespaceName);

                    Common.DoComments(ref dataLayerTemplate, "//", IncludeComments);

                    sw.Write(dataLayerTemplate);
                }
            }
        }

        private void CreateCsCollectionLayers()
        {
            if (!Directory.Exists(OutputDirectory + "\\Business"))
            {
                Directory.CreateDirectory(OutputDirectory + "\\Business");
            }
            foreach (string objectName in Objects.Split(';'))
            {
                //UpdateProgress(_progressNdx, Objects.Split(';').Length * 2);
                //_progressNdx++;
                var assembly = Assembly.GetExecutingAssembly();
                StringBuilder dataLayerTemplate = new StringBuilder();

                using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.DataLayer.SqlServerCSharpCollection.txt"))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            dataLayerTemplate.Append(reader.ReadToEnd());
                        }
                    }
                }

                string objName = objectName.Trim();
                bool isView = IsView(objName);

                List<Field> fields = MapFields(objName);

                if (!HasPrimaryKey(fields) && !isView)
                {
                    continue;
                }
                using (StreamWriter sw = File.CreateText(OutputDirectory + "\\Business\\" + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + "Collection.cs"))
                {
                    dataLayerTemplate.Replace("{1}", Common.GetSafeCsName(Common.GetCsPropertyName(objName)));
                    string objNameWithSuffix = Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + DataObjectSuffix;//"Obj";
                    dataLayerTemplate.Replace("{5}", objNameWithSuffix);

                    
                    string strObjName = Common.GetSafeCsName(Common.GetCsPropertyName(objName));
                    string strObjNameL = strObjName.Substring(0, 1).ToLower() + strObjName.Substring(1, strObjName.Length - 1);
                    dataLayerTemplate.Replace("{8}", strObjNameL);

                    string pkCsName = fields.First(z => z.IsPrimaryKey).SafeCsFieldName;
                    string strKeyFieldName = fields.First(z => z.IsPrimaryKey).SafeCsPropertyName;
                    if (fields.First(z => z.IsPrimaryKey).CsDataType == "string")
                    {
                        dataLayerTemplate.Replace("{12}", "string");
                    }
                    else
                    {
                        dataLayerTemplate.Replace("{12}", "int");
                    }
                    dataLayerTemplate.Replace("{9}", strKeyFieldName);

                    //dataLayerTemplate.Replace("{9}", strKeyFieldName);
                    dataLayerTemplate.Replace("{26}", BusinessNamespaceName);
                    dataLayerTemplate.Replace("{100}", DataNamespaceName);
                    dataLayerTemplate.Replace("{101}", BusinessNamespaceName);
                    dataLayerTemplate.Replace("{102}", EventLogNamespaceName);

                    Common.DoComments(ref dataLayerTemplate, "//", IncludeComments);

                    sw.Write(dataLayerTemplate);
                }
            }
        }

        private void CreateCsICLayers()
        {
            if (!Directory.Exists(OutputDirectory + "\\Interfaces"))
            {
                Directory.CreateDirectory(OutputDirectory + "\\Interfaces");
            }
            foreach (string objectName in Objects.Split(';'))
            {
                //UpdateProgress(_progressNdx, Objects.Split(';').Length * 2);
                //_progressNdx++;
                var assembly = Assembly.GetExecutingAssembly();
                StringBuilder dataLayerTemplate = new StringBuilder();

                using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.DataLayer.SqlServerCSharpIC.txt"))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            dataLayerTemplate.Append(reader.ReadToEnd());
                        }
                    }
                }

                string objName = objectName.Trim();
                bool isView = IsView(objName);

                List<Field> fields = MapFields(objName);

                if (!HasPrimaryKey(fields) && !isView)
                {
                    continue;
                }
                using (StreamWriter sw = File.CreateText(OutputDirectory + "\\Interfaces\\I" + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + "Collection.cs"))
                {
                    dataLayerTemplate.Replace("{1}", Common.GetSafeCsName(Common.GetCsPropertyName(objName)));


                    string strObjName = Common.GetSafeCsName(Common.GetCsPropertyName(objName));
                    string strObjNameL = strObjName.Substring(0, 1).ToLower() + strObjName.Substring(1, strObjName.Length - 1);
                    dataLayerTemplate.Replace("{8}", strObjNameL);

                    string pkCsName = fields.First(z => z.IsPrimaryKey).SafeCsFieldName;
                    string strKeyFieldName = fields.First(z => z.IsPrimaryKey).SafeCsPropertyName;
                    if (fields.First(z => z.IsPrimaryKey).CsDataType == "string")
                    {
                        dataLayerTemplate.Replace("{12}", "string");
                    }
                    else
                    {
                        dataLayerTemplate.Replace("{12}", "int");
                    }
                    dataLayerTemplate.Replace("{9}", strKeyFieldName);

                    //dataLayerTemplate.Replace("{9}", strKeyFieldName);
                    dataLayerTemplate.Replace("{26}", BusinessNamespaceName + ".Interfaces");
                    dataLayerTemplate.Replace("{100}", DataNamespaceName);
                    dataLayerTemplate.Replace("{101}", BusinessNamespaceName);
                    dataLayerTemplate.Replace("{102}", EventLogNamespaceName);

                    Common.DoComments(ref dataLayerTemplate, "//", IncludeComments);

                    sw.Write(dataLayerTemplate);
                }
            }
        }

        private void CreateCsRepoLayers()
        {
            if (!Directory.Exists(OutputDirectory + "\\Repository"))
            {
                Directory.CreateDirectory(OutputDirectory + "\\Repository");
            }
            foreach (string objectName in Objects.Split(';'))
            {
                //UpdateProgress(_progressNdx, Objects.Split(';').Length * 2);
                //_progressNdx++;
                var assembly = Assembly.GetExecutingAssembly();
                StringBuilder dataLayerTemplate = new StringBuilder();

                using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.DataLayer.SqlServerCSharpRepo.txt"))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            dataLayerTemplate.Append(reader.ReadToEnd());
                        }
                    }
                }

                string objName = objectName.Trim();
                bool isView = IsView(objName);

                List<Field> fields = MapFields(objName);

                if (!HasPrimaryKey(fields) && !isView)
                {
                    continue;
                }
                using (StreamWriter sw = File.CreateText(OutputDirectory + "\\Repository\\" + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + "Repository.cs"))
                {
                    dataLayerTemplate.Replace("{1}", Common.GetSafeCsName(Common.GetCsPropertyName(objName)));
                    string strObjName = Common.GetSafeCsName(Common.GetCsPropertyName(objName));
                    string strObjNameL = strObjName.Substring(0, 1).ToLower() + strObjName.Substring(1, strObjName.Length - 1);
                    dataLayerTemplate.Replace("{8}", strObjNameL);

                    dataLayerTemplate.Replace("{26}", DataNamespaceName);
                    dataLayerTemplate.Replace("{13}", DatabaseContenxt);
                    dataLayerTemplate.Replace("{100}", DataNamespaceName);
                    dataLayerTemplate.Replace("{101}", BusinessNamespaceName);
                    dataLayerTemplate.Replace("{102}", EventLogNamespaceName);

                    Common.DoComments(ref dataLayerTemplate, "//", IncludeComments);

                    sw.Write(dataLayerTemplate);
                }
            }
        }

        private void CreateCsDtoLayers()
        {
            if (!Directory.Exists(OutputDirectory + "\\Entity"))
            {
                Directory.CreateDirectory(OutputDirectory + "\\Entity");
            }
            foreach (string objectName in Objects.Split(';'))
            {
                //UpdateProgress(_progressNdx, Objects.Split(';').Length * 2);
                //_progressNdx++;
                var assembly = Assembly.GetExecutingAssembly();
                StringBuilder dataLayerTemplate = new StringBuilder();

                using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.DataLayer.SqlServerCSharpDto.txt"))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            dataLayerTemplate.Append(reader.ReadToEnd());
                        }
                    }
                }

                string objName = objectName.Trim();
                bool isView = IsView(objName);

                List<Field> fields = MapFields(objName);

                if (!HasPrimaryKey(fields) && !isView)
                {
                    continue;
                }
                using (StreamWriter sw = File.CreateText(OutputDirectory + "\\Entity\\" + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + "Dto.cs"))
                {
                    dataLayerTemplate.Replace("{1}", Common.GetSafeCsName(Common.GetCsPropertyName(objName)));
                    dataLayerTemplate.Replace("{2}", DefaultSchema);

                    var fieldsPart = new StringBuilder();
                    foreach (var field in fields)
                    {
                        if (field.IsPrimaryKey)
                            fieldsPart.AppendLine("        [Key]");


                        if (field.CsDataType == "string")
                        {
                            if (field.SqlLength > 0 && field.SqlLength / sizeof(char) < int.MaxValue / sizeof(char))
                                fieldsPart.AppendLine(string.Format("        [StringLength({0})]", field.SqlLength / sizeof(char)));
                            else
                                fieldsPart.AppendLine("        [MaxLength]");
                        }

                        if (field.CanBeNull && field.CsDataType != "string" && !field.CsDataType.Contains("[]"))
                        {
                            fieldsPart.AppendLine("        public Nullable<" + field.CsDataType + "> " + field.SafeCsPropertyName + " { get; set; }");
                        }
                        else
                        {
                            fieldsPart.AppendLine("        public " + field.CsDataType + " " + field.SafeCsPropertyName + " { get; set; }");
                        }
                    }
                    dataLayerTemplate.Replace("{3}", fieldsPart.ToString());

                    //dataLayerTemplate.Replace("{9}", strKeyFieldName);
                    dataLayerTemplate.Replace("{26}", DataNamespaceName);
                    dataLayerTemplate.Replace("{100}", DataNamespaceName);
                    dataLayerTemplate.Replace("{101}", BusinessNamespaceName);
                    dataLayerTemplate.Replace("{102}", EventLogNamespaceName);

                    Common.DoComments(ref dataLayerTemplate, "//", IncludeComments);

                    sw.Write(dataLayerTemplate);
                }
            }
        }

        private void CreateCsInitDataLayers()
        {
            if (!Directory.Exists(OutputDirectory + "\\DBSeed"))
            {
                Directory.CreateDirectory(OutputDirectory + "\\DBSeed");
            }
            foreach (string objectName in Objects.Split(';'))
            {
                //UpdateProgress(_progressNdx, Objects.Split(';').Length * 2);
                //_progressNdx++;
                var assembly = Assembly.GetExecutingAssembly();
                StringBuilder dataLayerTemplate = new StringBuilder();

                using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.DataLayer.SqlServerCSharpInit.txt"))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            dataLayerTemplate.Append(reader.ReadToEnd());
                        }
                    }
                }

                string objName = objectName.Trim();
                bool isView = IsView(objName);

                List<Field> fields = MapFields(objName);

                if (!HasPrimaryKey(fields) && !isView)
                {
                    continue;
                }
                DataTable dt = GetInitDatas(objName);
                if (dt == null)
                    continue;

                using (StreamWriter sw = File.CreateText(OutputDirectory + "\\DBSeed\\" + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".cs"))
                {
                    dataLayerTemplate.Replace("{1}", Common.GetSafeCsName(Common.GetCsPropertyName(objName)));

                    var fieldsPart = new StringBuilder();
                    foreach (DataRow dr in dt.Rows)
                    {
                        string record = "						new " + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + "Dto {";
                        foreach (var field in fields)
                        {
                            if (!field.IsIdentity)
                            {
                                record += field.SafeCsPropertyName;
                                if (dr[field.FieldName] == DBNull.Value)
                                {
                                    record += " = null, ";
                                }
                                else
                                {
                                    if (field.CsDataType == "string")
                                    {
                                        record += " = \"";
                                    }
                                    else
                                    {
                                        record += " = ";
                                    }

                                    if (field.CsDataType == "bool")
                                    {
                                        record += (dr[field.FieldName].Equals(true) ? "true" : "false");
                                    }
                                    else
                                        record += dr[field.FieldName].ToString();
                                    if (field.CsDataType == "string")
                                    {
                                        record += "\", ";
                                    }
                                    else
                                        record += ", ";
                                }
                            }
                        }
                        record += "},";
                        fieldsPart.AppendLine(record);
                    }
                    if (objName == "MenuFunc")
                    {
                        fieldsPart.AppendLine(" ");
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr["FuncURL"] == DBNull.Value || string.IsNullOrEmpty(dr["FuncURL"].ToString()))
                                continue;

                            string strFunc = dr["FuncURL"].ToString();
                            if (strFunc.IndexOf('.') > 0)
                                strFunc = strFunc.Substring(0, strFunc.IndexOf('.'));
                            fieldsPart.AppendLine(strFunc + ",");
                        }

                        fieldsPart.AppendLine(" ");
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr["FuncURL"] == DBNull.Value || string.IsNullOrEmpty(dr["FuncURL"].ToString()))
                                continue;

                            string strFunc = dr["FuncURL"].ToString();
                            if (strFunc.IndexOf('.') > 0)
                                strFunc = strFunc.Substring(0, strFunc.IndexOf('.'));
                            fieldsPart.AppendLine("case PageId." + strFunc + ":");
                            fieldsPart.AppendLine("  return \"" + dr["ID"] + "\";");
                        }
                    }
                    dataLayerTemplate.Replace("{14}", fieldsPart.ToString());

                    string strObjName = Common.GetSafeCsName(Common.GetCsPropertyName(objName));
                    string strObjNameL = strObjName.Substring(0, 1).ToLower() + strObjName.Substring(1, strObjName.Length - 1);
                    dataLayerTemplate.Replace("{8}", strObjNameL);

                    string pkCsName = fields.First(z => z.IsPrimaryKey).SafeCsFieldName;
                    string strKeyFieldName = fields.First(z => z.IsPrimaryKey).SafeCsPropertyName;

                    dataLayerTemplate.Replace("{9}", strKeyFieldName);
                    dataLayerTemplate.Replace("{26}", BusinessNamespaceName + ".Interfaces");
                    dataLayerTemplate.Replace("{13}", DatabaseContenxt);
                    dataLayerTemplate.Replace("{100}", DataNamespaceName);
                    dataLayerTemplate.Replace("{101}", BusinessNamespaceName);
                    dataLayerTemplate.Replace("{102}", EventLogNamespaceName);

                    Common.DoComments(ref dataLayerTemplate, "//", IncludeComments);

                    sw.Write(dataLayerTemplate);
                }
            }
        }

        private void CreateJavaEntityLayers()
        {
            if (!Directory.Exists(OutputDirectory + "\\Entity"))
            {
                Directory.CreateDirectory(OutputDirectory + "\\Entity");
            }

            if (AllowSerialization)
            {
                var assembly = Assembly.GetExecutingAssembly();
                StringBuilder jsonDateSerializerTemplate = new StringBuilder();

                using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.DataLayer.JsonDateSerializer.txt"))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            jsonDateSerializerTemplate.Append(reader.ReadToEnd());
                        }
                    }
                }
                using (StreamWriter sw = File.CreateText(OutputDirectory + "\\Entity\\JsonDateSerializer.java"))
                {
                    jsonDateSerializerTemplate.Replace("{26}", ModelPackageName);
                    jsonDateSerializerTemplate.Replace("{100}", DataNamespaceName);
                    jsonDateSerializerTemplate.Replace("{101}", BusinessNamespaceName);
                    jsonDateSerializerTemplate.Replace("{102}", EventLogNamespaceName);

                    Common.DoComments(ref jsonDateSerializerTemplate, "//", IncludeComments);

                    sw.Write(jsonDateSerializerTemplate);
                }
            }

            foreach (string objectName in Objects.Split(';'))
            {
                //UpdateProgress(_progressNdx, Objects.Split(';').Length * 2);
                //_progressNdx++;
                var assembly = Assembly.GetExecutingAssembly();
                StringBuilder dataLayerTemplate = new StringBuilder();

                using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.DataLayer.SqlServerJavaEntity.txt"))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            dataLayerTemplate.Append(reader.ReadToEnd());
                        }
                    }
                }

                string objName = objectName.Trim();
                bool isView = IsView(objName);

                List<Field> fields = MapFields(objName);

                if (!HasPrimaryKey(fields) && !isView)
                {
                    continue;
                }

                IIndexes _indexes = GetIndexs(objName, "");
                using (StreamWriter sw = File.CreateText(OutputDirectory + "\\Entity\\" + Common.GetSafeJavaName(Common.GetJavaPropertyName(objName, "Table")) + ".java"))
                {
                    string uniqueIndex = ", uniqueConstraints = ";
                    List<string> uniqueIndexes = new List<string>();
                    foreach (IIndex index in _indexes.Where(i => i.Unique == true))
                    {
                        string cols="";
                        foreach(var colName in ((Index)index).ColumnNames)
                        {
                            if (string.IsNullOrEmpty(cols))
                            {
                                cols += "{\"";
                                cols += colName;
                                cols += "\"";
                            }
                            else
                            {
                                cols += ", \"";
                                cols += colName;
                                cols += "\"";
                            }
                        }
                        cols += "}";
                        uniqueIndexes.Add(string.Format("@UniqueConstraint(columnNames={0})", cols));
                    }
                    if (uniqueIndexes.Count > 1)
                    {
                        uniqueIndex += "{";
                        foreach(var indexexp in uniqueIndexes)
                        {
                            uniqueIndex += indexexp;
                            uniqueIndex += ", ";
                        }
                        uniqueIndex += "}";
                    }
                    else if (uniqueIndexes.Count > 0)
                    {
                        uniqueIndex += uniqueIndexes[0];
                    }
                    else
                    {
                        uniqueIndex = "";
                    }
                    dataLayerTemplate.Replace("{9}", objName); //Common.GetSafeJavaName(Common.GetJavaPropertyName(objName, "Table"))
                    dataLayerTemplate.Replace("{1}", Common.GetSafeJavaName(Common.GetJavaPropertyName(objName, "Table"))); 
                    dataLayerTemplate.Replace("{2}", DefaultSchema);
                    dataLayerTemplate.Replace("{4}", uniqueIndex);
                    dataLayerTemplate.Replace("{8}", DatabaseName);//@Table(name="{9}",catalog="{8}"{4} ) 
                    string baseClass = ModelExtends.Substring(ModelExtends.LastIndexOf('.') + 1);
                    dataLayerTemplate.Replace("{10}", baseClass);
                    dataLayerTemplate.Replace("{11}", ModelExtends);

                    var fieldsPart = new StringBuilder();
                    foreach (var field in fields)
                    {
                        fieldsPart.AppendLine("    private " + field.JavaDataType + " " + field.SafeJavaFieldName + ";");
                    }
                    fieldsPart.AppendLine(" ");

                    List<string> flds = new List<string>();
                    foreach (var field in fields)
                    {
                        if (AllowSerialization)
                        {
                            fieldsPart.AppendLine(string.Format("    @JsonProperty(value = \"{0}\")", field.FieldName));
                            if (field.JavaDataType == "Date")
                                fieldsPart.AppendLine("    @JsonSerialize(using = JsonDateSerializer.class)");
                        }
                        if (field.IsPrimaryKey)
                        {
                            fieldsPart.AppendLine("    @Id");
                            fieldsPart.AppendLine("    @GeneratedValue(strategy = GenerationType.AUTO)");
                            fieldsPart.AppendLine("    @DocumentId");
                        }
                        else
                        {
                            string columnDef = "";
                            string nullable = "";
                            if (!field.CanBeNull)
                                nullable = ", nullable=false";
                            //@Column(name="Price", columnDefinition="Decimal(10,2) default '100.00'")
                            //if (field.DefaultValue != null)
                             //   nullable += ", nullable=false";

                            if (field.JavaDataType == "String")
                            {
                                if (field.SqlLength > 0 && field.SqlLength / sizeof(char) < int.MaxValue / sizeof(char))
                                {
                                    int fldLength = field.UnicodeText ? field.SqlLength / sizeof(char) : field.SqlLength;
                                    columnDef = string.Format("    @Column(name=\"{0}\"{1}, length={2})", field.FieldName, nullable, fldLength);
                                }
                                else
                                {
                                    field.JavaMappingType = "text";
                                    columnDef = string.Format("    @Column(name=\"{0}\"{1})", field.FieldName, nullable);
                                }
                            }
                            else if (field.JavaDataType == "Date")
                            {
                                if (field.JavaMappingType == "time")
                                {
                                    fieldsPart.AppendLine("    @Temporal(TemporalType.TIME)");
                                    columnDef = string.Format("    @Column(name=\"{0}\"{1}, length=8)", field.FieldName, nullable);
                                }
                                else
                                {
                                    fieldsPart.AppendLine("    @Temporal(TemporalType.TIMESTAMP)");
                                    columnDef = string.Format("    @Column(name=\"{0}\"{1}, length=19)", field.FieldName, nullable);
                                }
                            }
                            else if (string.Compare(field.JavaDataType, "BigDecimal", true) == 0) //string.Compare(field.JavaDataType, "float", true) == 0 || string.Compare(field.JavaDataType, "double", true) == 0
                            {
                                string fieldps = "";
                                if (field.SqlPrecision > 0)
                                    fieldps = string.Format("precision={0}", field.SqlPrecision);
                                if (field.SqlScale > 0)
                                {
                                    if (!string.IsNullOrEmpty(fieldps))
                                    {
                                        fieldps+=", ";
                                    }
                                    fieldps += string.Format("scale={0}", field.SqlScale);
                                }
                                columnDef = string.Format("    @Column(name=\"{0}\"{1}", field.FieldName, nullable);
                                if (!string.IsNullOrEmpty(fieldps))
                                {
                                    columnDef += ", ";
                                }
                                columnDef += fieldps;
                                columnDef += ")";
                            }
                            else
                            {
                                if (field.JavaMappingType == "org.hibernate.type.WrappedMaterializedBlobType")
                                {
                                    fieldsPart.AppendLine("    @Lob");
                                    fieldsPart.AppendLine("    @Basic(fetch = FetchType.LAZY )");
                                    columnDef = string.Format("    @Column(name=\"{0}\", columnDefinition = \"BLOB\"{1})", field.FieldName, nullable);
                                }
                                else
                                    columnDef = string.Format("    @Column(name=\"{0}\"{1})", field.FieldName, nullable);
                            }

                            fieldsPart.AppendLine(columnDef);
                            if (field.JavaMappingType == "text" || field.JavaMappingType == "org.hibernate.type.ByteType" || field.JavaMappingType == "org.hibernate.type.WrappedMaterializedBlobType")
                                fieldsPart.AppendLine(string.Format("    @Type(type=\"{0}\")", field.JavaMappingType));
                            if (field.JavaMappingType == "org.hibernate.type.ByteType")
                                fieldsPart.AppendLine("    @FieldBridge(impl = IntegerBridge.class)");
                            if (field.JavaMappingType != "org.hibernate.type.WrappedMaterializedBlobType")
                                fieldsPart.AppendLine("    @Field");
                        }
                        if (flds.Contains(field.SafeJavaPropertyName))
                        {
                            field.SafeJavaPropertyName += Char.ToUpperInvariant(field.JavaDataType[0]) + field.JavaDataType.Substring(1); ;
                        }
                        flds.Add(field.SafeJavaPropertyName);

                        if (field.JavaDataType == "boolean")
                        {
                            fieldsPart.AppendLine("    public " + field.JavaDataType + " is" + field.SafeJavaPropertyName + "(){");
                        }
                        else
                            fieldsPart.AppendLine("    public " + field.JavaDataType + " get" + field.SafeJavaPropertyName + "(){");
                        fieldsPart.AppendLine("        return this." + field.SafeJavaFieldName + ";");
                        fieldsPart.AppendLine("    }");
                        fieldsPart.AppendLine("    ");
                        fieldsPart.AppendLine(string.Format(@"    public void set{0}({1} {2})", field.SafeJavaPropertyName, field.JavaDataType, field.SafeJavaFieldName) + "{");
                        fieldsPart.AppendLine(string.Format("        this.{0} = {0};", field.SafeJavaFieldName));
                        fieldsPart.AppendLine("    }");
                        fieldsPart.AppendLine("    ");     
                    }
                    dataLayerTemplate.Replace("{3}", fieldsPart.ToString());

                    //Implementing equals()
                    var equalsPart = new StringBuilder();
                    equalsPart.AppendLine("    /**");
                    equalsPart.AppendLine("    * {@inheritDoc}");
                    equalsPart.AppendLine("    */");
                    equalsPart.AppendLine("    public boolean equals(Object o) {");
                    equalsPart.AppendLine("        if (this == o) return true;");
                    equalsPart.AppendLine("        if (o == null || getClass() != o.getClass()) return false;");
                    equalsPart.AppendLine("");

                    equalsPart.AppendLine(string.Format("        {0} pojo = ({0})o;", Common.GetSafeJavaName(Common.GetJavaPropertyName(objName, "Table"))));
                    equalsPart.AppendLine("        return (new EqualsBuilder()");
                    foreach (var field in fields)
                    {
                        if (field.IsPrimaryKey)
                            continue;

                        equalsPart.AppendLine(string.Format("             .append({0}, pojo.{0})", field.SafeJavaFieldName));
                    }
                    equalsPart.AppendLine("             ).isEquals();");
                    equalsPart.AppendLine("    }");
                    dataLayerTemplate.Replace("{5}", equalsPart.ToString());

                    //Implementing hashCode()
                    var hashCodePart = new StringBuilder();
                    hashCodePart.AppendLine("    /**");
                    hashCodePart.AppendLine("    * {@inheritDoc}");
                    hashCodePart.AppendLine("    */");
                    hashCodePart.AppendLine("     public int hashCode() {");
                    hashCodePart.AppendLine("        return   new  HashCodeBuilder( 17 ,  37 )");
                    foreach (var field in fields)
                    {
                        if (field.IsPrimaryKey)
                            continue;

                        hashCodePart.AppendLine(string.Format("             .append({0})", field.SafeJavaFieldName));
                    }
                    hashCodePart.AppendLine("             .toHashCode();");
                    hashCodePart.AppendLine("    }");
                    dataLayerTemplate.Replace("{6}", hashCodePart.ToString());

                    //Implementing toString()
                    var toStringPart = new StringBuilder();
                    toStringPart.AppendLine("    /**");
                    toStringPart.AppendLine("    * {@inheritDoc}");
                    toStringPart.AppendLine("    */");
                    toStringPart.AppendLine("     public String toString() {");
                    toStringPart.AppendLine("        StringBuffer sb = new StringBuffer(getClass().getSimpleName());");
                    toStringPart.AppendLine("        sb.append(\" [\");");
                    foreach (var field in fields)
                    {
                        if (field.JavaDataType == "boolean")
                        {
                            toStringPart.AppendLine(string.Format("        sb.append(\"{0}\").append(\"='\").append({1}).append(\"', \");", field.SafeJavaFieldName, "is" + field.SafeJavaPropertyName + "()"));
                        }
                        else
                            toStringPart.AppendLine(string.Format("        sb.append(\"{0}\").append(\"='\").append({1}).append(\"', \");", field.SafeJavaFieldName, "get" + field.SafeJavaPropertyName + "()"));
                    }
                    toStringPart.AppendLine("        sb.append(\"]\");");
                    toStringPart.AppendLine("        ");
                    toStringPart.AppendLine("        return sb.toString();");
                    toStringPart.AppendLine("    }");
                    dataLayerTemplate.Replace("{7}", toStringPart.ToString());

                    //dataLayerTemplate.Replace("{9}", strKeyFieldName);
                    dataLayerTemplate.Replace("{26}", ModelPackageName);
                    dataLayerTemplate.Replace("{100}", DataNamespaceName);
                    dataLayerTemplate.Replace("{101}", BusinessNamespaceName);
                    dataLayerTemplate.Replace("{102}", EventLogNamespaceName);

                    Common.DoComments(ref dataLayerTemplate, "//", IncludeComments, CustomComments);

                    sw.Write(dataLayerTemplate);
                }
            }
        }

        private void CreateVbDataLayers()
        {
            var storedProcedures = new StringBuilder();

            foreach (string objectName in Objects.Split(';'))
            {
                UpdateProgress(_progressNdx, Objects.Split(';').Length * 2);
                _progressNdx++;

                var assembly = Assembly.GetExecutingAssembly();
                var dataLayerTemplate = new StringBuilder();

                using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.DataLayer.SqlServerVbNet.txt"))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            dataLayerTemplate.Append(reader.ReadToEnd());
                        }
                    }
                }

                string objName = objectName.Trim();
                bool isView = IsView(objName);

                List<Field> fields = MapFields(objName);
                if (!HasPrimaryKey(fields) && !isView)
                {
                    continue;
                }

                using (StreamWriter sw = File.CreateText(OutputDirectory + "\\" + objName.ToProperFileName() + "Data.vb"))
                {
                    storedProcedures.Append(CreateStoredProcedures(Common.GetSafeCsName(Common.GetCsPropertyName(objName)), fields, isView));

                    dataLayerTemplate = dataLayerTemplate.Replace("{1}", Common.GetSafeVbName(Common.GetVbPropertyName(objName)));

                    var fieldsPart = new StringBuilder();
                    foreach (var field in fields)
                    {
                        fieldsPart.AppendLine("        Private " + field.SafeVbFieldName + " As " + field.VbDataType);
                    }
                    dataLayerTemplate = dataLayerTemplate.Replace("{2}", fieldsPart.ToString());

                    dataLayerTemplate = dataLayerTemplate.Replace("{3}", objName);

                    var fieldListPart = new StringBuilder();
                    foreach (var field in fields.Where(z => (!z.IsIdentity) && (!z.IsComputedField)).OrderBy(z => z.FieldName))
                    {
                        fieldListPart.Append("[" + field.FieldName + "],");
                    }
                    dataLayerTemplate = dataLayerTemplate.Replace("{4}", fieldListPart.ToString().TrimEnd(','));

                    var valPart = new StringBuilder();
                    int i = 1;
                    foreach (Field field in fields.Where(z => (!z.IsIdentity) && (!z.IsComputedField)).OrderBy(z => z.FieldName))
                    {
                        valPart.Append("@val" + i + ",");
                        i++;
                    }
                    dataLayerTemplate = dataLayerTemplate.Replace("{5}", valPart.ToString().TrimEnd(','));
                    if (!isView)
                    {
                        dataLayerTemplate = dataLayerTemplate.Replace("{6}", fields.First(z => z.IsPrimaryKey).FieldName);
                    }

                    var propertiesPart = new StringBuilder();
                    foreach (Field field in fields.OrderByDescending(z => z.IsPrimaryKey))
                    {
                        if (!string.IsNullOrEmpty(field.Description))
                        {
                            propertiesPart.Append("        ''' <summary>" + Environment.NewLine);
                            propertiesPart.Append("        ''' " + field.Description + Environment.NewLine);
                            propertiesPart.Append("        ''' </summary>" + Environment.NewLine);
                        }
                        propertiesPart.Append("        Public Overridable Property " + field.SafeVbPropertyName + "() As " + field.VbDataType + Environment.NewLine);
                        propertiesPart.Append("            Get" + Environment.NewLine);
                        propertiesPart.Append("                Return " + field.SafeVbFieldName + Environment.NewLine);
                        propertiesPart.Append("            End Get" + Environment.NewLine);

                        if (field.IsIdentity && (!isView))
                        {
                            propertiesPart.Append("            Protected Set(value As " + field.VbDataType + ")" + Environment.NewLine);
                            propertiesPart.Append("                " + field.SafeVbFieldName + " = value" + Environment.NewLine);
                            propertiesPart.Append("                _isDirty = True" + Environment.NewLine);
                            propertiesPart.Append("            End Set" + Environment.NewLine);
                        }
                        else if ((!field.IsComputedField) && (!isView))
                        {
                            propertiesPart.Append("            Set(value As " + field.VbDataType + ")" + Environment.NewLine);
                            propertiesPart.Append("                " + field.SafeVbFieldName + " = value" + Environment.NewLine);
                            propertiesPart.Append("                _isDirty = True" + Environment.NewLine);
                            if (field.VbDataType == "DateTime")
                                propertiesPart.Append("                If value = DateTime.MinValue Then SetNull(" + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + ".Fields." + field.SafeVbPropertyName + ") Else UnsetNull(" + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + ".Fields." + field.SafeVbPropertyName + ")" + Environment.NewLine);
                            else if (field.CsDataType == "string")
                            {
                                propertiesPart.AppendLine("                If value Is Nothing Then SetNull(" +
                                                      BusinessNamespaceName + "." +
                                                      Common.GetSafeVbName(Common.GetVbPropertyName(objName)) +
                                                      ".Fields." + field.SafeVbPropertyName + ") Else UnsetNull(" +
                                                      BusinessNamespaceName + "." +
                                                      Common.GetSafeVbName(Common.GetVbPropertyName(objName)) +
                                                      ".Fields." + field.SafeVbPropertyName + ")");
                            }
                            else
                            {
                                propertiesPart.AppendLine("                UnsetNull(" + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + ".Fields." + field.SafeVbPropertyName + ")");
                            }
                            propertiesPart.Append("            End Set" + Environment.NewLine);
                        }

                        if (field.IsComputedField || isView)
                        {
                            propertiesPart.Append("            Protected Set(value As " + field.VbDataType + ")" + Environment.NewLine);
                            propertiesPart.Append("                " + field.SafeVbFieldName + " = value" + Environment.NewLine);
                            propertiesPart.Append("                _isDirty = True" + Environment.NewLine);
                            propertiesPart.Append("            End Set" + Environment.NewLine);
                        }
                        propertiesPart.Append("        End Property" + Environment.NewLine + Environment.NewLine);
                    }

                    dataLayerTemplate = dataLayerTemplate.Replace("{7}", propertiesPart.ToString());

                    if (!isView)
                    {
                        string pkVbName = fields.First(z => z.IsPrimaryKey).SafeVbFieldName;
                        dataLayerTemplate.Replace("{8}", pkVbName);
                        if (fields.First(z => z.IsPrimaryKey).TextBased)
                        {
                            dataLayerTemplate.Replace("{88}", "'\" & " + pkVbName + " & \"'\"");
                        }
                        else
                        {
                            dataLayerTemplate.Replace("{88}", "\" & " + pkVbName);
                        }
                    }

                    var nullDictionaryPart = new StringBuilder("            _nullDictionary = New Dictionary(Of " + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + ".Fields, Boolean)() From { _" + Environment.NewLine);
                    foreach (var field in fields)
                    {
                        nullDictionaryPart.Append("                {" + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + ".Fields." + field.SafeVbPropertyName + ", True}, _" + Environment.NewLine);
                    }

                    dataLayerTemplate = dataLayerTemplate.Replace("{9}", nullDictionaryPart.ToString().TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd('_').TrimEnd(' ').TrimEnd(',') + " _" + Environment.NewLine + "            }");

                    var internalNamePart = new StringBuilder("            _internalNameDictionary = New Dictionary(Of " + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + ".Fields, String)() From { _" + Environment.NewLine);
                    foreach (var field in fields)
                    {
                        internalNamePart.Append("                {" + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + ".Fields." + field.SafeVbPropertyName + ", \"" + field.FieldName + "\"}, _" + Environment.NewLine);
                    }
                    dataLayerTemplate = dataLayerTemplate.Replace("{10}", internalNamePart.ToString().TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd('_').TrimEnd(' ').TrimEnd(',') + " _" + Environment.NewLine + "            }");

                    var fillPart = new StringBuilder();
                    foreach (Field field in fields)
                    {
                        fillPart.Append("            If HasField(_internalNameDictionary(" + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + ".Fields." + field.SafeVbPropertyName + "), dr) Then" + Environment.NewLine);
                        fillPart.Append("                If IsDBNull(dr(_internalNameDictionary(" + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + ".Fields." + field.SafeVbPropertyName + "))) Then" + Environment.NewLine);
                        fillPart.Append("                    SetNull(" + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + ".Fields." + field.SafeVbPropertyName + ")" + Environment.NewLine);
                        fillPart.Append("                Else" + Environment.NewLine);
                        fillPart.Append("                    " + field.SafeVbPropertyName + " = DirectCast(dr(_internalNameDictionary(" + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + ".Fields." + field.SafeVbPropertyName + ")), " + field.VbDataType + ")" + Environment.NewLine);
                        if (field.TextBased && AutoRightTrimStrings)
                        {
                            fillPart.AppendLine("                    " + field.SafeVbPropertyName + " = " + field.SafeVbPropertyName + ".TrimEnd()");
                        }
                        fillPart.Append("                    UnsetNull(" + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + ".Fields." + field.SafeVbPropertyName + ")" + Environment.NewLine);
                        fillPart.Append("                End If" + Environment.NewLine);
                        fillPart.Append("            Else" + Environment.NewLine);
                        fillPart.Append("                _isReadOnly = True" + Environment.NewLine);
                        fillPart.Append("                SetNull(" + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + ".Fields." + field.SafeVbPropertyName + ")" + Environment.NewLine);
                        fillPart.Append("            End If" + Environment.NewLine + Environment.NewLine);
                    }

                    dataLayerTemplate = dataLayerTemplate.Replace("{11}", fillPart.ToString());

                    var getPart = new StringBuilder();
                    foreach (Field field in fields.OrderByDescending(z => z.IsPrimaryKey))
                    {
                        if (field.IsPrimaryKey)
                        {
                            getPart.Append("                            " + field.SafeVbPropertyName + " = DirectCast(reader(\"" + field.FieldName + "\"), " + field.VbDataType + ")" + Environment.NewLine);
                            getPart.Append("                            UnsetNull(" + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + ".Fields." + field.SafeVbPropertyName + ")" + Environment.NewLine);
                        }
                        else
                        {
                            getPart.Append("                            If (Not HasField(\"" + field.FieldName + "\", reader)) OrElse reader.IsDBNull(reader.GetOrdinal(\"" + field.FieldName + "\")) Then" + Environment.NewLine);
                            getPart.Append("                                SetNull(" + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + ".Fields." + field.SafeVbPropertyName + ")" + Environment.NewLine);
                            getPart.Append("                            Else" + Environment.NewLine);
                            getPart.Append("                                " + field.SafeVbPropertyName + " = DirectCast(reader(\"" + field.FieldName + "\"), " + field.VbDataType + ")" + Environment.NewLine);
                            if (field.TextBased && AutoRightTrimStrings)
                            {
                                getPart.AppendLine("                                " + field.SafeVbPropertyName + " = " + field.SafeVbPropertyName + ".TrimEnd()");
                            }
                            getPart.Append("                                UnsetNull(" + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + ".Fields." + field.SafeVbPropertyName + ")" + Environment.NewLine);
                            getPart.Append("                            End If" + Environment.NewLine);
                        }
                    }

                    dataLayerTemplate = dataLayerTemplate.Replace("{12}", getPart.ToString());

                    var resetToDefaultPart = new StringBuilder();
                    foreach (var z in fields)
                    {
                        if (!z.IsIdentity) 
                            resetToDefaultPart.Append("            _nullDictionary(" + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + ".Fields." + z.SafeVbPropertyName + ") = True" + Environment.NewLine);
                    }
                    dataLayerTemplate = dataLayerTemplate.Replace("{13}", resetToDefaultPart.ToString());

                    var savePart = new StringBuilder();

                    i = 1;
                    foreach (Field field in fields.Where(z => (!z.IsIdentity) && (!z.IsComputedField)).OrderBy(z => z.FieldName))
                    {
                        savePart.Append("                            If IsNull(" + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(objName)) + ".Fields." + field.SafeVbPropertyName + ") Then" + Environment.NewLine);
                        savePart.Append("                                Dim param As New SqlParameter(\"@val" + i + "\", SqlDbType." + field.SqlDbType + ")" + Environment.NewLine);
                        if (field.CanBeNull)
                            savePart.Append("                                param.Value = DBNull.Value" + Environment.NewLine);
                        else savePart.Append("                                param.Value = " + GetDefaultVbValue(field) + Environment.NewLine);
                        savePart.Append("                                command.Parameters.Add(param)" + Environment.NewLine);
                        savePart.Append("                            Else" + Environment.NewLine);
                        savePart.Append("                                Dim param As New SqlParameter(\"@val" + i + "\", SqlDbType." + field.SqlDbType + ")" + Environment.NewLine);
                        savePart.Append("                                param.Value = " + field.SafeVbFieldName + Environment.NewLine);
                        savePart.Append("                                command.Parameters.Add(param)" + Environment.NewLine);
                        savePart.Append("                            End If" + Environment.NewLine);
                        i++;
                    }

                    dataLayerTemplate = dataLayerTemplate.Replace("{14}", savePart.ToString());

                    var str1 = new StringBuilder("                            ");

                    if (!isView)
                    {
                        Field f = fields.First(z => z.IsPrimaryKey);
                        if (f.IsIdentity)
                            str1.Append(f.SafeVbFieldName + " = " + GetVbConversionFunction(f.VbDataType) + "(obj)" + Environment.NewLine);
                    }

                    dataLayerTemplate = dataLayerTemplate.Replace("{15}", str1.ToString());

                    str1.Clear();
                    str1.Append("                    Const cmdString As String = \"UPDATE [" + DefaultSchema + "].[\" & LayerGenTableName & \"] SET ");
                    i = 1;
                    foreach (Field field in fields.Where(z => (!z.IsIdentity) && (!z.IsComputedField)).OrderBy(z => z.FieldName))
                    {
                        str1.Append("[" + field.FieldName + "]=@val" + i + ",");
                        i++;
                    }
                    dataLayerTemplate = dataLayerTemplate.Replace("{16}", str1.ToString().TrimEnd(',') + " WHERE \" & LayerGenPrimaryKey & \"=@val" + i + "\"" + Environment.NewLine);

                    dataLayerTemplate = dataLayerTemplate.Replace("{17}", "                            command.Parameters.AddWithValue(\"@val" + i + "\", _oldPrimaryKeyValue)" + Environment.NewLine);
                    if (!isView)
                    {
                        dataLayerTemplate = dataLayerTemplate.Replace("{18}", "        Private _oldPrimaryKeyValue As " + fields.First(z => z.IsPrimaryKey).VbDataType + Environment.NewLine);
                        dataLayerTemplate = dataLayerTemplate.Replace("{19}", fields.First(z => z.IsPrimaryKey).VbDataType);

                        if (fields.First(z => z.IsPrimaryKey).TextBased)
                        {
                            dataLayerTemplate = dataLayerTemplate.Replace("{20}", "            Dim sql As String = \"SELECT \" & strFields & \" FROM [" + DefaultSchema + "].[\" & LayerGenTableName & \"] WHERE \" & LayerGenPrimaryKey & \"=@val1\"" + Environment.NewLine);
                            dataLayerTemplate.Replace("{28}", "                    command.Parameters.AddWithValue(\"@val1\", id)");
                        }
                        else
                        {
                            dataLayerTemplate = dataLayerTemplate.Replace("{20}", "            Dim sql As String = \"SELECT \" & strFields & \" FROM [" + DefaultSchema + "].[\" & LayerGenTableName & \"] WHERE \" & LayerGenPrimaryKey & \"=\" & id" + Environment.NewLine);
                            dataLayerTemplate.Replace("{28}", "");
                        }
                    }

                    var fkDataGetBy = new StringBuilder();
                    if (!isView)
                    {
                        var fkProperties = new StringBuilder();
                        var fkFields = new StringBuilder();
                        string sqlDataType = "";

                        List<ForeignKey> keys = GetForeignKeys(objName);

                        foreach (ForeignKey key in keys)
                        {
                            fkFields.Append("        Private _my" + Common.GetSafeVbName(Common.GetVbPropertyName(key.ForeignColumnName)) + " As " + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(key.PrimaryTableName)) + Environment.NewLine);

                            fkProperties.Append("        Public ReadOnly Property F" + Common.GetSafeVbName(Common.GetVbPropertyName(key.ForeignColumnName)) + "() As " + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(key.PrimaryTableName)) + Environment.NewLine);
                            fkProperties.Append("            Get" + Environment.NewLine);
                            fkProperties.Append("                If _my" + Common.GetSafeVbName(Common.GetVbPropertyName(key.ForeignColumnName)) + " Is Nothing Then" + Environment.NewLine);
                            fkProperties.Append("                    _my" + Common.GetSafeVbName(Common.GetVbPropertyName(key.ForeignColumnName)) + " = New " + BusinessNamespaceName + "." + Common.GetSafeVbName(Common.GetVbPropertyName(key.PrimaryTableName)) + "(" + Common.GetVbFieldName(key.ForeignColumnName) + ")" + Environment.NewLine);
                            fkProperties.Append("                End If" + Environment.NewLine);
                            fkProperties.Append("                Return _my" + Common.GetSafeVbName(Common.GetVbPropertyName(key.ForeignColumnName)) + Environment.NewLine);
                            fkProperties.Append("            End Get" + Environment.NewLine);
                            fkProperties.Append("        End Property" + Environment.NewLine + Environment.NewLine);

                            fkDataGetBy.Append("        Friend Shared Function GetBy" + Common.GetSafeVbName(Common.GetVbPropertyName(key.ForeignColumnName)) + "(fkId As ");
                            foreach (Field f in fields)
                            {
                                if (String.Equals(f.FieldName, key.ForeignColumnName, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    sqlDataType = f.SqlDbType;
                                    fkDataGetBy.Append(f.VbDataType + ") As DataTable" + Environment.NewLine);
                                    break;
                                }
                            }
                            fkDataGetBy.Append("            Using connection As New SqlConnection()" + Environment.NewLine);
                            fkDataGetBy.Append("                connection.ConnectionString = Universal.GetConnectionString()" + Environment.NewLine);
                            fkDataGetBy.Append("                Using command As New SqlCommand()" + Environment.NewLine);
                            fkDataGetBy.Append("                    command.Connection = connection" + Environment.NewLine);
                            fkDataGetBy.Append("                    command.CommandType = CommandType.Text" + Environment.NewLine);
                            fkDataGetBy.Append("                    command.CommandText = \"SELECT * FROM [" + DefaultSchema + "].[" + key.ForeignTableName + "] WHERE [" + key.ForeignColumnName + "]=@val1\"" + Environment.NewLine);
                            fkDataGetBy.Append("                    command.Parameters.Add(New SqlParameter(\"@val1\", SqlDbType." + sqlDataType + ") With { _" + Environment.NewLine);
                            fkDataGetBy.Append("                        .Value = fkId _" + Environment.NewLine);
                            fkDataGetBy.Append("                    })" + Environment.NewLine);
                            fkDataGetBy.Append(Environment.NewLine);
                            fkDataGetBy.Append("                    connection.Open()" + Environment.NewLine);
                            fkDataGetBy.Append("                    Using adapter As New SqlDataAdapter()" + Environment.NewLine);
                            fkDataGetBy.Append("                        Using ds As New DataSet()" + Environment.NewLine);
                            fkDataGetBy.Append("                            adapter.SelectCommand = command" + Environment.NewLine);
                            fkDataGetBy.Append("                            adapter.Fill(ds)" + Environment.NewLine);
                            fkDataGetBy.Append(Environment.NewLine);
                            fkDataGetBy.Append("                            If ds.Tables.Count > 0 Then" + Environment.NewLine);
                            fkDataGetBy.Append("                                Return ds.Tables(0)" + Environment.NewLine);
                            fkDataGetBy.Append("                            End If" + Environment.NewLine);
                            fkDataGetBy.Append("                        End Using" + Environment.NewLine);
                            fkDataGetBy.Append("                    End Using" + Environment.NewLine);
                            fkDataGetBy.Append("                End Using" + Environment.NewLine);
                            fkDataGetBy.Append("            End Using" + Environment.NewLine);
                            fkDataGetBy.Append(Environment.NewLine);
                            fkDataGetBy.Append("            Return Nothing" + Environment.NewLine);
                            fkDataGetBy.Append("        End Function" + Environment.NewLine);
                        }

                        dataLayerTemplate = dataLayerTemplate.Replace("{22}", fkFields.ToString());
                        dataLayerTemplate = dataLayerTemplate.Replace("{21}", fkProperties.ToString());
                    }

                    dataLayerTemplate = dataLayerTemplate.Replace("{23}", fkDataGetBy.ToString());

                    if (isView)
                    {
                        dataLayerTemplate = dataLayerTemplate.Replace("{18}", "");
                        dataLayerTemplate = dataLayerTemplate.Replace("{21}", "");
                        dataLayerTemplate = dataLayerTemplate.Replace("{22}", "");
                        RemoveTemplateComments(ref dataLayerTemplate);
                    }
                    else
                    {
                        dataLayerTemplate = dataLayerTemplate.Replace("{/*}", "");
                        dataLayerTemplate = dataLayerTemplate.Replace("{*/}", "");
                    }

                    var equalPart = new StringBuilder();
                    if (!isView)
                    {
                        Field primaryKeyField = fields.First(z => z.IsPrimaryKey);
                        dataLayerTemplate.Replace("{25}", primaryKeyField.SafeVbPropertyName);
                    }
                    foreach (Field field in fields.OrderByDescending(z => z.IsPrimaryKey))
                    {
                        equalPart.Append("            Dim cls" + field.SafeVbFieldName.Remove(0,1) + " As Byte() = ObjectToByteArray(cls." + field.SafeVbPropertyName + ")" + Environment.NewLine);
                    }
                    equalPart.Append(Environment.NewLine);

                    var tmpEqual = new StringBuilder();

                    equalPart.Append("            Dim clsArray As Byte() = New Byte(");

                    foreach (Field field in fields.OrderByDescending(z => z.IsPrimaryKey))
                    {
                        tmpEqual.Append("cls" + field.SafeVbFieldName.Remove(0, 1) + ".Length + ");
                        if (tmpEqual.Length >= 110)
                        {
                            equalPart.Append(tmpEqual + " _ " + Environment.NewLine + "                       ");
                            tmpEqual.Clear();
                        }
                    }
                    equalPart.Append(tmpEqual);
                    equalPart.ReplaceAllText(equalPart.ToString().TrimEnd(' ', '\r', '\n', '+', '_'));
                    equalPart.Append(" - 1) {}" + Environment.NewLine);

                    tmpEqual.Clear();
                    tmpEqual.Append("0");

                    foreach (Field field in fields.OrderByDescending(z => z.IsPrimaryKey))
                    {
                        equalPart.Append("            Array.Copy(cls" + field.SafeVbFieldName.Remove(0, 1));
                        equalPart.Append(", 0, clsArray, " + tmpEqual + ", cls" + field.SafeVbFieldName.Remove(0, 1) + ".Length)" + Environment.NewLine);
                        tmpEqual.Append(" + cls" + field.SafeVbFieldName.Remove(0, 1) + ".Length");
                        int teLength = tmpEqual.ToString().Split(Environment.NewLine.ToCharArray()).Length;
                        if (teLength > 0)
                        {
                            string te = tmpEqual.ToString().Split(Environment.NewLine.ToCharArray())[teLength - 1];
                            if (te.Length >= 85)
                            {
                                tmpEqual.Append(" _ " + Environment.NewLine + "                        ");
                            }
                        }
                    }

                    equalPart.Append(Environment.NewLine);
                    equalPart.Append("            Return clsArray" + Environment.NewLine);

                    dataLayerTemplate.Replace("{24}", equalPart.ToString());
                    dataLayerTemplate.Replace("{26}", DataNamespaceName);
                    dataLayerTemplate.Replace("{27}", BusinessNamespaceName);
                    dataLayerTemplate.Replace("{30}", DefaultSchema);

                    var serializationCode = new StringBuilder();

                    if (AllowSerialization)
                    {
                        serializationCode.AppendLine("        <Serializable> _");
                        serializationCode.AppendLine("        Public Class " + Common.GetSafeVbName("Serializable" + Common.GetVbPropertyName(objName)));

                        foreach (Field field in fields)
                        {
                            if (field.IsValueType)
                                serializationCode.AppendLine("            Private " + field.SafeVbFieldName + " As System.Nullable(Of " + field.VbDataType + ")");
                            else
                                serializationCode.AppendLine("            Private " + field.SafeVbFieldName + " As " + field.VbDataType);
                        }
                        serializationCode.AppendLine("            Private _serializationIsUpdate As Boolean");
                        serializationCode.AppendLine();

                        foreach (Field field in fields.OrderByDescending(z => z.IsPrimaryKey))
                        {
                            if (field.IsValueType)
                                serializationCode.AppendLine("            Public Property " + field.SafeVbPropertyName + "() As System.Nullable(Of " + field.VbDataType + ")");
                            else
                                serializationCode.AppendLine("            Public Property " + field.SafeVbPropertyName + "() As " + field.VbDataType);
                            serializationCode.AppendLine("                Get");
                            serializationCode.AppendLine("                    Return " + field.SafeVbFieldName);
                            serializationCode.AppendLine("                End Get");
                            if (field.IsValueType)
                                serializationCode.AppendLine("                Set(value As System.Nullable(Of " + field.VbDataType + "))");
                            else
                                serializationCode.AppendLine("                Set(value As " + field.VbDataType + ")");
                            serializationCode.AppendLine("                    " + field.SafeVbFieldName + " = value");
                            serializationCode.AppendLine("                End Set");
                            serializationCode.AppendLine("            End Property");
                        }
                        serializationCode.AppendLine("            ''' <summary>");
                        serializationCode.AppendLine("            ''' Set this to true if <see cref=\"Save()\"></see> should do an update.");
                        serializationCode.AppendLine("            ''' Otherwise, set to false to force <see cref=\"Save()\"></see> to do an insert.");
                        serializationCode.AppendLine("            ''' </summary>");
                        serializationCode.AppendLine("            Public Property SerializationIsUpdate() As Boolean");
                        serializationCode.AppendLine("                Get");
                        serializationCode.AppendLine("                    Return _serializationIsUpdate");
                        serializationCode.AppendLine("                End Get");
                        serializationCode.AppendLine("                Set(value As Boolean)");
                        serializationCode.AppendLine("                    _serializationIsUpdate = value");
                        serializationCode.AppendLine("                End Set");
                        serializationCode.AppendLine("            End Property");
                        serializationCode.AppendLine("        End Class");
                    }

                    dataLayerTemplate.Replace("{32}", serializationCode.ToString());

                    serializationCode.Clear();

                    if (AllowSerialization)
                    {
                        serializationCode.AppendLine("        ''' <summary>");
                        serializationCode.AppendLine("        ''' Converts an instance of an object to a string format");
                        serializationCode.AppendLine("        ''' </summary>");
                        serializationCode.AppendLine("        ''' <param name=\"format\">Specifies if it should convert to XML, BSON or JSON</param>");
                        serializationCode.AppendLine("        ''' <returns>The object, converted to a string representation</returns>");
                        serializationCode.AppendLine("        Public Function ToString(format As " + BusinessNamespaceName + ".SerializationFormats) As String");
                        serializationCode.AppendLine("            Dim " + Common.GetSafeVbName("serializable" + Common.GetVbPropertyName(objName)) + " As New " + Common.GetSafeVbName("Serializable" + Common.GetVbPropertyName(objName)) + "()");
                        foreach (var field in fields.OrderByDescending(z => z.IsPrimaryKey))
                        {
                            if (field.IsValueType)
                            {
                                serializationCode.AppendLine("            " + Common.GetSafeVbName("serializable" + Common.GetVbPropertyName(objName)) + "." + field.SafeVbPropertyName + " = If(IsNull(" + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Fields." + field.SafeVbPropertyName + "), DirectCast(Nothing, System.Nullable(Of " + field.VbDataType + ")), " + field.SafeVbFieldName + ")");
                            } 
                            else if (field.CsDataType == "string")
                            {
                                serializationCode.AppendLine("            " + Common.GetSafeVbName("serializable" + Common.GetVbPropertyName(objName)) + "." + field.SafeVbPropertyName + " = If(IsNull(" + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Fields." + field.SafeVbPropertyName + "), Nothing, " + field.SafeVbFieldName + ")");
                            }
                            else 
                            { 
                                serializationCode.AppendLine("            " + Common.GetSafeVbName("serializable" + Common.GetVbPropertyName(objName)) + "." + field.SafeVbPropertyName + " = " + field.SafeVbFieldName);
                            }
                        }
                        serializationCode.AppendLine("            " + Common.GetSafeVbName("serializable" + Common.GetVbPropertyName(objName)) + ".SerializationIsUpdate = _isUpdate");
                        serializationCode.AppendLine("            If format = " + BusinessNamespaceName + ".SerializationFormats.Json Then");
                        serializationCode.AppendLine("                Return Newtonsoft.Json.JsonConvert.SerializeObject(" + Common.GetSafeVbName("serializable" + Common.GetVbPropertyName(objName)) + ")");
                        serializationCode.AppendLine("            End If");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            If format = " + BusinessNamespaceName + ".SerializationFormats.Xml Then");
                        serializationCode.AppendLine("                Dim xType As New System.Xml.Serialization.XmlSerializer(" + Common.GetSafeVbName("serializable" + Common.GetVbPropertyName(objName)) + ".GetType())");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("                Using sw As New StringWriter()");
                        serializationCode.AppendLine("                    xType.Serialize(sw, " + Common.GetSafeVbName("serializable" + Common.GetVbPropertyName(objName)) + ")");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("                    Return sw.ToString()");
                        serializationCode.AppendLine("                End Using");
                        serializationCode.AppendLine("            End If");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            If format = " + BusinessNamespaceName + ".SerializationFormats.BsonBase64 Then");
                        serializationCode.AppendLine("                Using ms As New System.IO.MemoryStream");
                        serializationCode.AppendLine("                    Using writer As New Newtonsoft.Json.Bson.BsonWriter(ms)");
                        serializationCode.AppendLine("                        Dim serializer As New Newtonsoft.Json.JsonSerializer()");
                        serializationCode.AppendLine("                        serializer.Serialize(writer, " + Common.GetSafeVbName("serializable" + Common.GetVbPropertyName(objName)) + ")");
                        serializationCode.AppendLine("                    End Using");
                        serializationCode.AppendLine("                    Return Convert.ToBase64String(ms.ToArray())");
                        serializationCode.AppendLine("                End Using");
                        serializationCode.AppendLine("            End If");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            Return \"\"");
                        serializationCode.AppendLine("        End Function");
                    }

                    dataLayerTemplate.Replace("{33}", serializationCode.ToString());

                    serializationCode.Clear();

                    if (AllowSerialization)
                    {
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("        Protected Shared Function BsonTo" + Common.GetVbPropertyName(objName) + "(bson As String) As " + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName));
                        serializationCode.AppendLine("            Dim z As " + Common.GetSafeVbName("Serializable" + Common.GetVbPropertyName(objName)));
                        serializationCode.AppendLine("            Dim data As Byte() = Convert.FromBase64String(bson)");
                        serializationCode.AppendLine("            Using ms As New System.IO.MemoryStream(data)");
                        serializationCode.AppendLine("                Using reader As New Newtonsoft.Json.Bson.BsonReader(ms)");
                        serializationCode.AppendLine("                    Dim serializer As New Newtonsoft.Json.JsonSerializer");
                        serializationCode.AppendLine("                    z = serializer.Deserialize(Of " + Common.GetSafeVbName("Serializable" + Common.GetVbPropertyName(objName)) + ")(reader)");
                        serializationCode.AppendLine("                End Using");
                        serializationCode.AppendLine("            End Using");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            Dim tmp As New " + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName) + "()");
                        serializationCode.AppendLine();
                        foreach (Field field in fields)
                        {
                            if (field.IsValueType)
                            {
                                serializationCode.AppendLine("            If z." + field.SafeVbPropertyName + ".HasValue Then");
                                serializationCode.AppendLine("                tmp." + field.SafeVbFieldName + " = z." + field.SafeVbPropertyName + ".Value");
                                serializationCode.AppendLine("                tmp.UnsetNull(" + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Fields." + field.SafeVbPropertyName + ")");
                                serializationCode.AppendLine("            Else ");
                                serializationCode.AppendLine("                tmp.SetNull(" + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Fields." + field.SafeVbPropertyName + ")");
                                serializationCode.AppendLine("            End If");
                            }
                            else
                            {
                                serializationCode.AppendLine("            If z." + field.SafeVbPropertyName + " Is Nothing Then");
                                serializationCode.AppendLine("                tmp.SetNull(" + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Fields." + field.SafeVbPropertyName + ")");
                                serializationCode.AppendLine("            Else ");
                                serializationCode.AppendLine("                tmp." + field.SafeVbFieldName + " = z." + field.SafeVbPropertyName);
                                serializationCode.AppendLine("                tmp.UnsetNull(" + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Fields." + field.SafeVbPropertyName + ")");
                                serializationCode.AppendLine("            End If");
                            }
                        }
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            tmp._isUpdate = z.SerializationIsUpdate");
                        serializationCode.AppendLine("            tmp._isDirty = True");
                        serializationCode.AppendLine("            Return tmp");

                        serializationCode.AppendLine("        End Function");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("        Protected Shared Function XmlTo" + Common.GetVbPropertyName(objName) + "(xml As String) As " + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName));
                        serializationCode.AppendLine("            Dim xType As New System.Xml.Serialization.XmlSerializer(GetType(" + Common.GetSafeVbName("Serializable" + Common.GetVbPropertyName(objName)) + "))");
                        serializationCode.AppendLine("            Dim z As " + Common.GetSafeVbName("Serializable" + Common.GetVbPropertyName(objName)));
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            Using sr As New StringReader(xml)");
                        serializationCode.AppendLine("                z = DirectCast(xType.Deserialize(sr), " + Common.GetSafeVbName("Serializable" + Common.GetVbPropertyName(objName)) + ")");
                        serializationCode.AppendLine("            End Using");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            Dim tmp As New " + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName) + "()");
                        serializationCode.AppendLine();
                        foreach (Field field in fields)
                        {
                            if (field.IsValueType)
                            {
                                serializationCode.AppendLine("            If z." + field.SafeVbPropertyName + ".HasValue Then");
                                serializationCode.AppendLine("                tmp." + field.SafeVbFieldName + " = z." + field.SafeVbPropertyName + ".Value");
                                serializationCode.AppendLine("                tmp.UnsetNull(" + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Fields." + field.SafeVbPropertyName + ")");
                                serializationCode.AppendLine("            Else ");
                                serializationCode.AppendLine("                tmp.SetNull(" + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Fields." + field.SafeVbPropertyName + ")");
                                serializationCode.AppendLine("            End If");
                            }
                            else
                            {
                                serializationCode.AppendLine("            If z." + field.SafeVbPropertyName + " Is Nothing Then");
                                serializationCode.AppendLine("                tmp.SetNull(" + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Fields." + field.SafeVbPropertyName + ")");
                                serializationCode.AppendLine("            Else ");
                                serializationCode.AppendLine("                tmp." + field.SafeVbFieldName + " = z." + field.SafeVbPropertyName);
                                serializationCode.AppendLine("                tmp.UnsetNull(" + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Fields." + field.SafeVbPropertyName + ")");
                                serializationCode.AppendLine("            End If");
                            }
                        }
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            tmp._isUpdate = z.SerializationIsUpdate");
                        serializationCode.AppendLine("            tmp._isDirty = True");
                        serializationCode.AppendLine("            Return tmp");

                        serializationCode.AppendLine("        End Function");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("        Protected Shared Function JsonTo" + Common.GetVbPropertyName(objName) + "(json As String) As " + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName));
                        serializationCode.AppendLine("            Dim z As " + Common.GetSafeVbName("Serializable" + Common.GetVbPropertyName(objName)) + " = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Serializable" + Common.GetVbPropertyName(objName) + ")(json)");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            Dim tmp As New " + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName) + "()");
                        serializationCode.AppendLine();
                        foreach (Field field in fields)
                        {
                            if (field.IsValueType)
                            {
                                serializationCode.AppendLine("            If z." + field.SafeVbPropertyName + ".HasValue Then");
                                serializationCode.AppendLine("                tmp." + field.SafeVbFieldName + " = z." + field.SafeVbPropertyName + ".Value");
                                serializationCode.AppendLine("                tmp.UnsetNull(" + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Fields." + field.SafeVbPropertyName + ")");
                                serializationCode.AppendLine("            Else ");
                                serializationCode.AppendLine("                tmp.SetNull(" + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Fields." + field.SafeVbPropertyName + ")");
                                serializationCode.AppendLine("            End If");
                            }
                            else
                            {
                                serializationCode.AppendLine("            If z." + field.SafeVbPropertyName + " Is Nothing Then");
                                serializationCode.AppendLine("                tmp.SetNull(" + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Fields." + field.SafeVbPropertyName + ")");
                                serializationCode.AppendLine("            Else ");
                                serializationCode.AppendLine("                tmp." + field.SafeVbFieldName + " = z." + field.SafeVbPropertyName);
                                serializationCode.AppendLine("                tmp.UnsetNull(" + BusinessNamespaceName + "." + Common.GetVbPropertyName(objName) + ".Fields." + field.SafeVbPropertyName + ")");
                                serializationCode.AppendLine("            End If");
                            }
                        }
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            tmp._isUpdate = z.SerializationIsUpdate");
                        serializationCode.AppendLine("            tmp._isDirty = True");
                        serializationCode.AppendLine("            Return tmp");

                        serializationCode.AppendLine("        End Function");
                    }

                    dataLayerTemplate.Replace("{34}", serializationCode.ToString());

                    Common.DoComments(ref dataLayerTemplate, "'", IncludeComments);
                    sw.Write(dataLayerTemplate);
                }
            }
            using (StreamWriter sw = File.CreateText(OutputDirectory + "\\StoredProcedureScripts.SQL"))
            {
                sw.Write(storedProcedures.ToString());
            }
        }

        private string GetDefaultCsValue(Field field)
        {
            string csType = field.CsDataType;

            if (csType == "bool")
                return "false";

            if (csType == "DateTime")
                return "DateTime.Now";

            if (csType == "short")
                return "0";

            if (csType == "int")
                return "0";

            if (csType == "long")
                return "0";

            if (csType == "char")
                return "'\\0'";

            if (csType == "string")
                return "\"\"";

            if (csType == "byte[]")
                return "new byte[] {0}";

            if (csType == "float")
                return "0f";

            if (csType == "decimal")
                return "0m";

            if (csType == "double")
                return "0";

            if (csType == "byte")
                return "0";

            return "";
        }

        private string GetDefaultVbValue(Field field)
        {
            string csType = field.CsDataType;

            if (csType == "bool")
                return "False";

            if (csType == "DateTime")
                return "DateTime.Now";

            if (csType == "short")
                return "0";

            if (csType == "int")
                return "0";

            if (csType == "long")
                return "0";

            if (csType == "char")
                return "'\\0'";

            if (csType == "string")
                return "\"\"";

            if (csType == "byte[]")
                return "new Byte() {0}";

            if (csType == "float")
                return "0";

            if (csType == "decimal")
                return "0";

            if (csType == "double")
                return "0";

            if (csType == "byte")
                return "0";

            return "";
        }

        private void RemoveTemplateComments(ref StringBuilder templateString)
        {
            int ndx1 = templateString.IndexOf("{/*}");
            int ndx2 = templateString.IndexOf("{*/}");

            while (ndx1 >= 0 && ndx2 >= 0)
            {
                templateString.Remove(ndx1, ndx2 - ndx1 + 4);

                ndx1 = templateString.IndexOf("{/*}");
                ndx2 = templateString.IndexOf("{*/}");
            }
        }

        private bool IsPrimaryKey(string tableName, string fieldName)
        {
            using (var connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "sp_pkeys";
                    command.Parameters.AddWithValue("@table_name", tableName);
                    command.Parameters.AddWithValue("@table_owner", DefaultSchema);

                    using (var adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = command;
                        using (var ds = new DataSet())
                        {
                            adapter.Fill(ds);
                            if (ds.Tables.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Cast<DataRow>().Any(row => ((string) row["COLUMN_NAME"]).ToLower() == fieldName.ToLower()))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private void CreateSQLScript()
        {
            if (!Directory.Exists(OutputDirectory + "\\InitData"))
            {
                Directory.CreateDirectory(OutputDirectory + "\\InitData");
            }

            var assembly = Assembly.GetExecutingAssembly();
            StringBuilder dataLayerTemplate = new StringBuilder();

            using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.DataLayer.SqlServerScript.txt"))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        dataLayerTemplate.Append(reader.ReadToEnd());
                    }
                }
            }
            using (StreamWriter sw = File.CreateText(OutputDirectory + "\\InitData\\" + DatabaseName + ".sql"))
            {
                var tablesPart = new StringBuilder();
                foreach (string objectName in Objects.Split(';'))
                {
                    string objName = objectName.Trim();
                    bool isView = IsView(objName);
                    if (isView)
                        continue;

                    tablesPart.AppendLine(string.Format("/****** Object:  Table [{0}].[{1}]    Script Date: 10/31/2016 14:54:14 ******/", DefaultSchema, objName));
                    tablesPart.AppendLine(string.Format("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].[{1}]') AND type in (N'U'))", DefaultSchema, objName));
                    tablesPart.AppendLine(string.Format("DROP TABLE [{0}].[{1}]", DefaultSchema, objName));
                    tablesPart.AppendLine("GO");
                    tablesPart.AppendLine(" ");
                }
                dataLayerTemplate.Replace("{1}", tablesPart.ToString());
                sw.Write(dataLayerTemplate);
            }
        }

        private IIndexes GetIndexs(string tableName, string fieldName) // sp_indexes @is_unique 
        {
            //using (var connection = new SqlConnection())
            //{
            //    connection.ConnectionString = ConnectionString;
            //    using (var command = new SqlCommand())
            //    {
            //        command.Connection = connection;
            //        command.CommandType = CommandType.Text;
            //        command.CommandText = "SELECT TABLE_NAME = t.name, INDEX_NAME = ind.name,[UNIQUE] = ind.is_unique,[TYPE] = ind.[type],";
            //        command.CommandText += "     FILL_FACTOR = ind.fill_factor ";
            //        command.CommandText += "FROM ";
            //        command.CommandText += "     sys.indexes ind ";
            //        command.CommandText += "INNER JOIN ";
            //        command.CommandText += "     sys.tables t ON ind.object_id = t.object_id ";
            //        command.CommandText += "WHERE ";
            //        command.CommandText += "     ind.is_primary_key = 0 ";
            //        command.CommandText += "     AND ind.is_unique_constraint = 0 ";
            //        command.CommandText += "     AND t.is_ms_shipped = 0 ";
            //        command.CommandText += "     AND t.name = '" + tableName + "'";
            //        command.CommandText += "ORDER BY t.name, ind.name, ind.index_id";

            //        using (var adapter = new SqlDataAdapter())
            //        {
            //            adapter.SelectCommand = command;
            //            using (var ds = new DataSet())
            //            {
            //                adapter.Fill(ds);
            //                Sql.SqlIndexes _indexes = new Sql.SqlIndexes();
            //                _indexes.PopulateArray(ds.Tables[0]);
            //            }
            //        }
            //    }
            //}
            var StaticMyMeta = new dbRoot();
            {
                if (StaticMyMeta.Connect("SQL", ConnectionString))
                {
                    IIndexes _indexes = new Sql.SqlIndexes(StaticMyMeta);
                    bool bLoad = _indexes.LoadIndexes(StaticMyMeta.DefaultDatabaseName, DefaultSchema, tableName);
                    //int nCount = _indexes.Count();
                    //foreach (var index in _indexes)
                    //{
                    //    string name = index.Name;
                    //    bool isUnique = index.Unique;
                    //    string[] colsName = ((Index)index).ColumnNames;
                    //}

                    return _indexes;
                    //_indexes.Table = this;
                    //_indexes.dbRoot = this.dbRoot;
                    //_indexes.LoadAll();
                }
            }

            //using (var connection = new SqlConnection())
            //{
            //    connection.ConnectionString = ConnectionString;
            //    using (var command = new SqlCommand())
            //    {
            //        command.Connection = connection;
            //        command.CommandType = CommandType.StoredProcedure;
            //        command.CommandText = "sp_indexes";
            //        command.Parameters.AddWithValue("@table_server", DatabaseServer);
            //        command.Parameters.AddWithValue("@table_name", tableName);
            //        command.Parameters.AddWithValue("@table_schema", DefaultSchema);
            //        command.Parameters.AddWithValue("@table_catalog", connection.Database);

            //        using (var adapter = new SqlDataAdapter())
            //        {
            //            adapter.SelectCommand = command;
            //            using (var ds = new DataSet())
            //            {
            //                adapter.Fill(ds);
            //                if (ds.Tables.Count > 0)
            //                {
            //                    Sql.SqlIndexes _indexes = new Sql.SqlIndexes();
            //                    _indexes.PopulateArray(ds.Tables[0]);
            //                    int nCount = _indexes.Count();
            //                }
            //            }
            //        }
            //    }
            //}

            return null;
        }

        private DataTable GetInitDatas(string tableName)
        {
            var fields = new List<Field>();

            try
            {
                using (var connection = new SqlConnection())
                {
                    connection.ConnectionString = ConnectionString;
                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "select * from " + DefaultSchema + "." + tableName;
                        if (tableName == "MenuFunc")
                        {
                            command.CommandText += " Order by FuncParentID, Funcdesc";
                        }

                        using (var adapter = new SqlDataAdapter())
                        {
                            adapter.SelectCommand = command;
                            using (var ds = new DataSet())
                            {
                                adapter.Fill(ds);
                                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                {
                                    return ds.Tables[0];
                                }
                            }
                        }
                    }
                }
            }
            catch// (Exception ex)
            {
                //ex;
            }

            return null;
        }

        private List<Field> MapFields(string tableName)
        {
            var fields = new List<Field>();

            using (var connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "sp_columns";
                    command.Parameters.AddWithValue("@table_name", tableName);
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
                                    var field = new Field();

                                    field.FieldName = (string) row["COLUMN_NAME"];
                                    field.IsPrimaryKey = IsPrimaryKey(tableName, field.FieldName);
                                    field.IsIdentity = ((string) row["TYPE_NAME"]).ToLower().Trim().EndsWith("identity");
                                    field.CsDataType = GetCsDataType(((string) row["TYPE_NAME"]).Trim());
                                    field.JavaMappingType = GetJavaMappingType(((string)row["TYPE_NAME"]).Trim());
                                    field.VbDataType = GetVbDataType(((string) row["TYPE_NAME"]).Trim());
                                    field.SafeCsFieldName = Common.GetSafeCsName(Common.GetCsFieldName(field.FieldName));
                                    field.SafeCsPropertyName = Common.GetSafeCsName(Common.GetCsPropertyName(field.FieldName, Common.GetSafeCsName(Common.GetCsPropertyName(tableName))));
                                    field.SafeVbFieldName = Common.GetSafeVbName(Common.GetVbFieldName(field.FieldName));
                                    field.SafeVbPropertyName = Common.GetSafeVbName(Common.GetVbPropertyName(field.FieldName, Common.GetSafeVbName(Common.GetVbPropertyName(tableName))));
                                    field.SqlDbType = GetSqlDbTypeFromSqlType(((string) row["TYPE_NAME"]).Trim());
                                    field.IntrinsicSqlDataType = ((string) row["TYPE_NAME"]).ToUpper().Trim();
                                    field.IntrinsicSqlDataType = field.IntrinsicSqlDataType.Replace(" IDENTITY", "").Trim();
                                    field.IsValueType = Common.IsValueType(field.CsDataType);
                                    field.UnicodeText = IsUnicodeText(((string)row["TYPE_NAME"]).Trim());

                                    try
                                    {
                                        field.SqlLength = row["LENGTH"]!= DBNull.Value ? (int)row["LENGTH"] : 0;
                                    }
                                    catch
                                    {
                                        field.SqlLength = 0;
                                    }

                                    try
                                    {
                                        field.SqlPrecision = row["PRECISION"]!= DBNull.Value ? (int) row["PRECISION"] : 0;
                                    }
                                    catch
                                    {
                                        field.SqlPrecision = 0;
                                    }
                                    try
                                    {
                                        field.SqlScale = row["SCALE"] != DBNull.Value ? (short) row["SCALE"] : (short)0;
                                    }
                                    catch
                                    {
                                        field.SqlScale = 0;
                                    }
                                    if (field.SqlScale == 0 && field.SqlPrecision > 0 && field.SqlLength - field.SqlPrecision > 0)
                                    {
                                        field.SqlScale = (short)(field.SqlLength - field.SqlPrecision);
                                    }
                                    field.TextBased = IsTextBased(GetCsDataType(((string)row["TYPE_NAME"]).Trim()));
                                    field.Description = GetDescription(tableName, field.FieldName);
                                    field.IsComputedField = IsFieldComputed(tableName, field.FieldName);
                                    field.CanBeNull = ((short) row["NULLABLE"]) == 1;

                                    field.JavaDataType = GetJavaDataType(((string)row["TYPE_NAME"]).Trim(), field.CanBeNull);
                                    field.SafeJavaFieldName = Common.GetSafeJavaName(Common.GetJavaFieldName(field.FieldName));
                                    field.SafeJavaPropertyName = Common.GetSafeJavaName(Common.GetJavaPropertyName(field.FieldName, Common.GetSafeJavaName(Common.GetJavaPropertyName(tableName, "Table")), field.JavaDataType));
                                    field.DefaultValue = row["COLUMN_DEF"] != DBNull.Value ? ((string)row["COLUMN_DEF"]).Trim() : null;

                                    fields.Add(field);
                                }
                            }
                        }
                    }
                }
            }

            return fields;
        }

        private bool IsFieldComputed(string tableName, string fieldName)
        {
            using (var connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                using (var command = new SqlCommand())
                {
                    string sql = "SELECT sysobjects.name AS TableName, syscolumns.name AS ColumnName FROM syscolumns INNER JOIN sysobjects ON syscolumns.id = sysobjects.id";
                    sql = sql + " AND sysobjects.xtype = 'U' WHERE syscolumns.iscomputed = 1 AND sysobjects.name = '" + tableName + "'";
                    sql = sql + " AND syscolumns.name = '" + fieldName + "'";

                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;

                    using (var adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = command;
                        using (var ds = new DataSet())
                        {
                            adapter.Fill(ds);
                            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                            {
                                if (ds.Tables[0].Rows[0]["ColumnName"] == null || ds.Tables[0].Rows[0]["ColumnName"] == DBNull.Value)
                                    return false;

                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool IsView(string objectName)
        {
            using (var connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                using (var command = new SqlCommand())
                {
                    string sql = "SELECT type_desc FROM sys.objects WHERE name= N'" + objectName + "'";

                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;

                    using (var adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = command;
                        using (var ds = new DataSet())
                        {
                            adapter.Fill(ds);
                            if (ds.Tables.Count > 0)
                            {
                                if (ds.Tables[0].Rows[0]["type_desc"] == null || ds.Tables[0].Rows[0]["type_desc"] == DBNull.Value)
                                    return false;
                                var view = (string)ds.Tables[0].Rows[0]["type_desc"];

                                return !string.IsNullOrEmpty(view) && view.ToLower() == "view";
                            }
                        }
                    }
                }
            }

            return false;
        }

        private string GetDescription(string tableName, string fieldName)
        {
            using (var connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                using (var command = new SqlCommand())
                {
                    string sql = "SELECT  st.name AS [Table] , sc.name AS [Column] ,sep.value AS [Description] FROM sys.tables st";
                    sql = sql + " INNER JOIN sys.columns sc ON st.object_id = sc.object_id LEFT JOIN sys.extended_properties sep ON st.object_id = sep.major_id AND sc.column_id = sep.minor_id AND sep.name = 'MS_Description'";
                    sql = sql + " WHERE st.name = '" + tableName + "' AND sc.name = '" + fieldName + "'";

                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;

                    using (var adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = command;
                        using (var ds = new DataSet())
                        {
                            adapter.Fill(ds);
                            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                            {
                                if (ds.Tables[0].Rows[0]["Description"] == null || ds.Tables[0].Rows[0]["Description"] == DBNull.Value)
                                    return "";
                                var description = (string)ds.Tables[0].Rows[0]["Description"];

                                return string.IsNullOrEmpty(description) ? "" : description;
                            }
                        }
                    }
                }
            }

            return "";
        }

        private List<ForeignKey> GetForeignKeys(string tableName)
        {
            var keys = new List<ForeignKey>();

            using (var connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "sp_fkeys";

                    command.Parameters.AddWithValue("@fktable_name", tableName);

                    using (var adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = command;
                        using (var ds = new DataSet())
                        {
                            adapter.Fill(ds);
                            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                            {
                                keys.AddRange(from DataRow row in ds.Tables[0].Rows
                                    select new ForeignKey
                                    {
                                        ForeignTableName = (string) row["FKTABLE_NAME"],
                                        ForeignColumnName = (string) row["FKCOLUMN_NAME"],
                                        PrimaryTableName = (string) row["PKTABLE_NAME"]
                                    });
                            }
                        }
                    }
                }
            }

            return keys;
        }

        private bool IsTextBased(string csType)
        {
            return csType.ToLower() == "string";
        }

        private bool IsUnicodeText(string sqlType)
        {
            sqlType = sqlType.Trim().ToLower();

            switch (sqlType)
            {
                case "char":
                case "text":
                case "varchar":
                    return false;
                case "nchar":
                case "ntext":
                case "nvarchar":
                    return true;
            }

            return false;
        }

        private string GetSqlDbTypeFromSqlType(string sqlType)
        {
            sqlType = sqlType.EndsWith("identity") ? sqlType.Substring(0, sqlType.Length - 8) : sqlType;
            sqlType = sqlType.Trim().ToLower();

            switch (sqlType)
            {
                case "bigint":
                    return "BigInt";
                case "binary":
                    return "Binary";
                case "bit":
                    return "Bit";
                case "char":
                    return "Char";
                case "date":
                    return "Date";
                case "datetime":
                    return "DateTime";
                case "datetime2":
                    return "DateTime2";
                case "datetimeoffset":
                    return "DateTimeOffset";
                case "decimal":
                    return "Decimal";
                case "float":
                    return "Float";
                case "image":
                    return "Image";
                case "int":
                    return "Int";
                case "money":
                    return "Money";
                case "nchar":
                    return "NChar";
                case "ntext":
                    return "NText";
                case "numeric":
                    return "Decimal";
                case "nvarchar":
                    return "NVarChar";
                case "real":
                    return "Real";
                case "smalldatetime":
                    return "SmallDateTime";
                case "smallint":
                    return "SmallInt";
                case "smallmoney":
                    return "SmallMoney";
                case "text":
                    return "Text";
                case "time":
                    return "Time";
                case "timestamp":
                    return "Timestamp";
                case "tinyint":
                    return "TinyInt";
                case "uniqueidentifier":
                    return "UniqueIdentifier";
                case "varbinary":
                    return "VarBinary";
                case "varchar":
                    return "VarChar";
                case "variant":
                    return "Variant";
                case "xml":
                    return "Xml";
            }

            return sqlType;
        }

        private string GetVbConversionFunction(string vbType)
        {
            switch (vbType)
            {
                case "Long":
                    return "CLng";
                case "Boolean":
                    return "CBool";
                case "String":
                    return "CStr";
                case "Decimal":
                    return "CDec";
                case "Double":
                    return "CDbl";
                case "Integer":
                    return "CInt";
                case "Short":
                    return "CShort";
                case "Byte":
                    return "CByte";
                case "Char":
                    return "CChar";
                case "Single":
                    return "CSng";
                case "Object":
                    return "CObj";
            }

            return "";
        }

        private string GetVbDataType(string sqlTypeName)
        {
            sqlTypeName = sqlTypeName.EndsWith("identity") ? sqlTypeName.Substring(0, sqlTypeName.Length - 8) : sqlTypeName;
            sqlTypeName = sqlTypeName.Trim().ToLower();

            switch (sqlTypeName)
            {
                case "bigint":
                    return "Long";
                case "binary":
                case "image":
                case "rowversion":
                case "timestamp":
                case "varbinary":
                    return "Byte()";
                case "bit":
                    return "Boolean";
                case "char":
                case "nchar":
                case "ntext":
                case "nvarchar":
                case "text":
                case "varchar":
                    return "String";
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    return "DateTime";
                case "datetimeoffset":
                    return "DateTimeOffset";
                case "decimal":
                case "money":
                case "numeric":
                case "smallmoney":
                    return "Decimal";
                case "float":
                    return "Double";
                case "real":
                    return "Single";
                case "int":
                    return "Integer";
                case "smallint":
                    return "Short";
                case "sql_variant":
                    return "Object";
                case "time":
                    return "TimeSpan";
                case "tinyint":
                    return "Byte";
                case "uniqueidentifier":
                    return "Guid";
                case "xml":
                    return "Xml";
            }

            return "object";
        }

        private string GetCsDataType(string sqlTypeName)
        {
            sqlTypeName = sqlTypeName.EndsWith("identity") ? sqlTypeName.Substring(0, sqlTypeName.Length - 8) : sqlTypeName;
            sqlTypeName = sqlTypeName.Trim().ToLower();

            switch (sqlTypeName)
            {
                case "bigint":
                    return "long";
                case "binary":
                case "image":
                case "rowversion":
                case "timestamp":
                case "varbinary":
                    return "byte[]";
                case "bit":
                    return "bool";
                case "char":
                case "nchar":
                case "ntext":
                case "nvarchar":
                case "text":
                case "varchar":
                    return "string";
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    return "DateTime";
                case "datetimeoffset":
                    return "DateTimeOffset";
                case "decimal":
                case "money":
                case "numeric":
                case "smallmoney":
                    return "decimal";
                case "float":
                    return "double";
                case "real":
                    return "float";
                case "int":
                    return "int";
                case "smallint":
                    return "short";
                case "sql_variant":
                    return "object";
                case "time":
                    return "TimeSpan";
                case "tinyint":
                    return "byte";
                case "uniqueidentifier":
                    return "Guid";
                case "xml":
                    return "SqlXml";
            }

            return "object";
        }

        private string GetJavaDataType(string sqlTypeName, bool canBeNull)
        {
            sqlTypeName = sqlTypeName.EndsWith("identity") ? sqlTypeName.Substring(0, sqlTypeName.Length - 8) : sqlTypeName;
            sqlTypeName = sqlTypeName.Trim().ToLower();

            switch (sqlTypeName)
            {
                case "bigint":
                    return "Long";
                case "binary":
                case "image":
                case "rowversion":
                case "timestamp":
                case "varbinary":
                    return "Byte[]";
                case "bit":
                    return "boolean";
                case "char":
                case "nchar":
                case "ntext":
                case "nvarchar":
                case "text":
                case "varchar":
                    return "String";
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    return "Date";
                case "datetimeoffset":
                    return "DateTimeOffset";
                case "decimal":
                case "money":
                case "numeric":
                case "smallmoney":
                    return "BigDecimal";
                case "float":
                    return canBeNull ? "Double" : "double";
                case "real":
                    return canBeNull ? "Float" : "float";
                case "int":
                    return canBeNull ? "Integer" : "int";
                case "smallint":
                    return canBeNull ? "Short" : "short";
                case "sql_variant":
                    return "object";
                case "time":
                    return "Date";
                case "tinyint":
                    return canBeNull ? "Byte" : "byte";
                case "uniqueidentifier":
                    return "Guid";
                case "xml":
                    return "SqlXml";
            }

            return "object";
        }

        private string GetJavaMappingType(string sqlTypeName)
        {
            sqlTypeName = sqlTypeName.EndsWith("identity") ? sqlTypeName.Substring(0, sqlTypeName.Length - 8) : sqlTypeName;
            sqlTypeName = sqlTypeName.Trim().ToLower();

            switch (sqlTypeName)
            {
                case "bigint":
                    return "long";
                case "binary":
                case "image":
                case "rowversion":
                case "timestamp":
                case "varbinary":
                    return "org.hibernate.type.WrappedMaterializedBlobType";
                case "bit":
                    return "boolean";
                case "char":
                case "nchar":
                case "nvarchar":
                case "varchar":
                    return "String";
                case "ntext":
                case "text":
                    return "text";
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    return "Date";
                case "datetimeoffset":
                    return "DateTimeOffset";
                case "decimal":
                case "money":
                case "numeric":
                case "smallmoney":
                    return "big_decimal";
                case "float":
                    return "double";
                case "real":
                    return "float";
                case "int":
                    return "integer";
                case "smallint":
                    return "short";
                case "sql_variant":
                    return "object";
                case "time":
                    return "time";
                case "tinyint":
                    return "org.hibernate.type.ByteType";
                case "uniqueidentifier":
                    return "Guid";
                case "xml":
                    return "SqlXml";
            }

            return "object";
        }
        private bool HasPrimaryKey(List<Field> fields)
        {
            return fields.Any(field => field.IsPrimaryKey);
        }

        private string CreateSelectStoredProcedure(string tableName, List<Field> fields)
        {
            var selectProcedure = new StringBuilder();

            selectProcedure.AppendLine("IF EXISTS ( SELECT  *");
            selectProcedure.AppendLine("            FROM    dbo.sysobjects");
            selectProcedure.AppendLine("            WHERE   id = OBJECT_ID(N'[" + DefaultSchema + "].[sp" + tableName + "_Select]')");
            selectProcedure.AppendLine("                    AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )");
            selectProcedure.AppendLine("    DROP PROCEDURE [" + DefaultSchema + "].[sp" + tableName + "_Select];");
            selectProcedure.AppendLine("GO");
            selectProcedure.AppendLine();
            selectProcedure.AppendLine("SET ANSI_NULLS OFF;");
            selectProcedure.AppendLine("GO");
            selectProcedure.AppendLine("SET QUOTED_IDENTIFIER OFF;");
            selectProcedure.AppendLine("GO");
            selectProcedure.AppendLine("CREATE    PROCEDURE [" + DefaultSchema + "].[sp" + tableName + "_Select]");
            selectProcedure.AppendLine("    (");
            selectProcedure.Append("     @id ");
            selectProcedure.Append(fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper());
            if (fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("DECIMAL") || fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("NUMERIC"))
            {
                selectProcedure.Append("(" + fields.First(z => z.IsPrimaryKey).SqlPrecision + ", " + fields.First(z => z.IsPrimaryKey).SqlScale + ")");
            }
            if (fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("VARCHAR") ||
                fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("NVARCHAR") ||
                fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("CHAR") ||
                fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("NCHAR"))
            {
                selectProcedure.Append("(" + fields.First(z => z.IsPrimaryKey).SqlPrecision + ")");
            }
            selectProcedure.AppendLine(" ,");
            selectProcedure.AppendLine("     @fields NVARCHAR(MAX)");
            selectProcedure.AppendLine("    )");
            selectProcedure.AppendLine("AS");
            selectProcedure.AppendLine("    DECLARE @sqlCommand NVARCHAR(MAX);");
            selectProcedure.AppendLine();
            selectProcedure.Append("    SET @sqlCommand = 'SELECT ' + @fields + ' FROM [" + DefaultSchema + "].[" + tableName + "] WHERE " + fields.First(z => z.IsPrimaryKey).FieldName + " = '");
            if (fields.First(z => z.IsPrimaryKey).TextBased)
            {
                selectProcedure.AppendLine(" + CHAR(39) + @id + CHAR(39);");
            }
            else
            {
                selectProcedure.AppendLine();
                selectProcedure.AppendLine("        + CAST(@id AS NVARCHAR(MAX));");
            }
            selectProcedure.AppendLine("    EXEC (@sqlCommand);");
            selectProcedure.AppendLine();
            selectProcedure.AppendLine("    SET QUOTED_IDENTIFIER ON;");
            selectProcedure.AppendLine("GO");
            selectProcedure.AppendLine();

            return selectProcedure.ToString();
        }

        private string CreateUpdateStoredProcedure(string tableName, List<Field> fields)
        {
            var updateProcedure = new StringBuilder();

            updateProcedure.AppendLine("IF EXISTS ( SELECT  *");
            updateProcedure.AppendLine("            FROM    dbo.sysobjects");
            updateProcedure.AppendLine("            WHERE   id = OBJECT_ID(N'[" + DefaultSchema + "].[sp" + tableName + "_Update]')");
            updateProcedure.AppendLine("                    AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )");
            updateProcedure.AppendLine("    DROP PROCEDURE [" + DefaultSchema + "].[sp" + tableName + "_Update];");
            updateProcedure.AppendLine("GO");
            updateProcedure.AppendLine();
            updateProcedure.AppendLine("SET ANSI_NULLS OFF;");
            updateProcedure.AppendLine("GO");
            updateProcedure.AppendLine("SET QUOTED_IDENTIFIER OFF;");
            updateProcedure.AppendLine("GO");
            updateProcedure.AppendLine("CREATE    PROCEDURE [" + DefaultSchema + "].[sp" + tableName + "_Update]");
            updateProcedure.AppendLine("    (");
            int count = 1;
            foreach (Field field in fields.Where(z => (!z.IsIdentity) && (!z.IsComputedField)).OrderBy(z => z.FieldName))
            {
                updateProcedure.Append("     @val" + count + " " + field.IntrinsicSqlDataType.ToUpper());
                if (field.IntrinsicSqlDataType.ToUpper().Equals("DECIMAL") || fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("NUMERIC"))
                {
                    updateProcedure.Append("(" + field.SqlPrecision + ", " + field.SqlScale + ")");
                }
                if (field.IntrinsicSqlDataType.ToUpper().Equals("VARCHAR") ||
                    field.IntrinsicSqlDataType.ToUpper().Equals("NVARCHAR") ||
                    field.IntrinsicSqlDataType.ToUpper().Equals("CHAR") ||
                    field.IntrinsicSqlDataType.ToUpper().Equals("NCHAR"))
                {
                    updateProcedure.Append("(" + field.SqlPrecision + ")");
                }
                updateProcedure.AppendLine(" ,");
                count++;
            }
            updateProcedure.Append("     @val" + count + " " + fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper());
            if (fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("DECIMAL") || fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("NUMERIC"))
            {
                updateProcedure.Append("(" + fields.First(z => z.IsPrimaryKey).SqlPrecision + ", " + fields.First(z => z.IsPrimaryKey).SqlScale + ")");
            }
            if (fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("VARCHAR") ||
                fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("NVARCHAR") ||
                fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("CHAR") ||
                fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("NCHAR"))
            {
                updateProcedure.Append("(" + fields.First(z => z.IsPrimaryKey).SqlPrecision + ")");
            }
            updateProcedure.AppendLine();
            updateProcedure.AppendLine("    )");
            updateProcedure.AppendLine("AS");
            updateProcedure.AppendLine("    UPDATE  [" + DefaultSchema + "].[" + tableName + "]");
            updateProcedure.Append("    SET");

            count = 1;

            Field first = fields.Where(z => (!z.IsIdentity) && (!z.IsComputedField)).OrderBy(z => z.FieldName).First();
            Field last = fields.Where(z => (!z.IsIdentity) && (!z.IsComputedField)).OrderBy(z => z.FieldName).Last();

            foreach (Field field in fields.Where(z => (!z.IsIdentity) && (!z.IsComputedField)).OrderBy(z => z.FieldName))
            {
                if (first.FieldName == field.FieldName)
                    updateProcedure.AppendLine("     [" + field.FieldName + "]=@val" + count + ( field.FieldName == last.FieldName ? "" : ","));
                else updateProcedure.AppendLine("            [" + field.FieldName + "]=@val" + count + (field.FieldName == last.FieldName ? "" : ","));
                count++;
            }
            updateProcedure.Remove(updateProcedure.Length - 1, 1);
            updateProcedure.AppendLine("    WHERE   [" + fields.First(z => z.IsPrimaryKey).FieldName + "]=@val" + count + ";");
            updateProcedure.AppendLine("    SET QUOTED_IDENTIFIER ON;");
            updateProcedure.AppendLine("GO");
            updateProcedure.AppendLine();

            return updateProcedure.ToString();
        }

        private string CreateInsertStoredProcedure(string tableName, List<Field> fields)
        {
            var insertProcedure = new StringBuilder();

            insertProcedure.AppendLine("IF EXISTS ( SELECT  *");
            insertProcedure.AppendLine("            FROM    dbo.sysobjects");
            insertProcedure.AppendLine("            WHERE   id = OBJECT_ID(N'[" + DefaultSchema + "].[sp" + tableName + "_Insert]')");
            insertProcedure.AppendLine("                    AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )");
            insertProcedure.AppendLine("    DROP PROCEDURE [" + DefaultSchema + "].[sp" + tableName + "_Insert];");
            insertProcedure.AppendLine("GO");
            insertProcedure.AppendLine();
            insertProcedure.AppendLine("SET ANSI_NULLS OFF;");
            insertProcedure.AppendLine("GO");
            insertProcedure.AppendLine("SET QUOTED_IDENTIFIER OFF;");
            insertProcedure.AppendLine("GO");
            insertProcedure.AppendLine("CREATE    PROCEDURE [" + DefaultSchema + "].[sp" + tableName + "_Insert]");
            insertProcedure.AppendLine("    (");
            int count = 1;
            Field first = fields.Where(z => (!z.IsIdentity) && (!z.IsComputedField)).OrderBy(z => z.FieldName).First();
            Field last = fields.Where(z => (!z.IsIdentity) && (!z.IsComputedField)).OrderBy(z => z.FieldName).Last();

            foreach (Field field in fields.Where(z => (!z.IsIdentity) && (!z.IsComputedField)).OrderBy(z => z.FieldName))
            {
                insertProcedure.Append("     @val" + count + " " + field.IntrinsicSqlDataType.ToUpper());
                if (field.IntrinsicSqlDataType.ToUpper().Equals("DECIMAL") || fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("NUMERIC"))
                {
                    insertProcedure.Append("(" + field.SqlPrecision + ", " + field.SqlScale + ")");
                }
                if (field.IntrinsicSqlDataType.ToUpper().Equals("VARCHAR") ||
                    field.IntrinsicSqlDataType.ToUpper().Equals("NVARCHAR") ||
                    field.IntrinsicSqlDataType.ToUpper().Equals("CHAR") ||
                    field.IntrinsicSqlDataType.ToUpper().Equals("NCHAR"))
                {
                    insertProcedure.Append("(" + field.SqlPrecision + ")");
                }
                if (field.FieldName == last.FieldName)
                {
                    insertProcedure.AppendLine();
                }
                else
                {
                    insertProcedure.AppendLine(" ,");    
                }
                
                count++;
            }
            
            insertProcedure.AppendLine("    )");
            insertProcedure.AppendLine("AS");
            insertProcedure.AppendLine("    INSERT  INTO  " + DefaultSchema + ".[" + tableName + "]");
            insertProcedure.Append("            (");

            foreach (Field field in fields.Where(z => (!z.IsIdentity) && (!z.IsComputedField)).OrderBy(z => z.FieldName))
            {
                if (field.FieldName == first.FieldName)
                {
                    insertProcedure.Append(" [" + field.FieldName + "]");
                }
                else
                {
                    insertProcedure.Append("              [" + field.FieldName + "]");
                }
                if (field.FieldName == last.FieldName)
                {
                    insertProcedure.AppendLine();
                }
                else
                {
                    insertProcedure.AppendLine(" ,");
                }
            }
            insertProcedure.AppendLine("            )");
            insertProcedure.Append("    VALUES  (");
            count = 1;
            foreach (Field field in fields.Where(z => (!z.IsIdentity) && (!z.IsComputedField)).OrderBy(z => z.FieldName))
            {
                if (field.FieldName == first.FieldName)
                {
                    insertProcedure.Append(" @val" + count);
                }
                else
                {
                    insertProcedure.Append("               @val" + count);
                }
                if (field.FieldName == last.FieldName)
                {
                    insertProcedure.AppendLine();
                }
                else
                {
                    insertProcedure.AppendLine(" ,");
                }
                count++;
            }
            insertProcedure.AppendLine("            );");
            insertProcedure.AppendLine("    SELECT  SCOPE_IDENTITY();");
            insertProcedure.AppendLine();
            
            insertProcedure.AppendLine("    SET QUOTED_IDENTIFIER ON;");
            insertProcedure.AppendLine("GO");
            insertProcedure.AppendLine();

            return insertProcedure.ToString();
        }

        private string CreateGetAllStoredProcedure(string tableName)
        {
            var getAllProcedure = new StringBuilder();

            getAllProcedure.AppendLine("IF EXISTS ( SELECT  *");
            getAllProcedure.AppendLine("            FROM    dbo.sysobjects");
            getAllProcedure.AppendLine("            WHERE   id = OBJECT_ID(N'[" + DefaultSchema + "].[sp" + tableName + "_GetAll]')");
            getAllProcedure.AppendLine("                    AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )");
            getAllProcedure.AppendLine("    DROP PROCEDURE [" + DefaultSchema + "].[sp" + tableName + "_GetAll];");
            getAllProcedure.AppendLine("GO");
            getAllProcedure.AppendLine();
            getAllProcedure.AppendLine("SET ANSI_NULLS OFF;");
            getAllProcedure.AppendLine("GO");
            getAllProcedure.AppendLine("SET QUOTED_IDENTIFIER OFF;");
            getAllProcedure.AppendLine("GO");
            getAllProcedure.AppendLine("CREATE    PROCEDURE [" + DefaultSchema + "].[sp" + tableName + "_GetAll]");
            getAllProcedure.AppendLine("AS");
            getAllProcedure.AppendLine("    SELECT *");
            getAllProcedure.AppendLine("    FROM [" + DefaultSchema + "].[" + tableName + "];");
            getAllProcedure.AppendLine();
            getAllProcedure.AppendLine("    SET QUOTED_IDENTIFIER ON;");
            getAllProcedure.AppendLine("GO");
            getAllProcedure.AppendLine();

            return getAllProcedure.ToString();
        }

        private string CreateDeleteStoredProcedure(string tableName, List<Field> fields)
        {
            var deleteProcedure = new StringBuilder();

            deleteProcedure.AppendLine("IF EXISTS ( SELECT  *");
            deleteProcedure.AppendLine("            FROM    dbo.sysobjects");
            deleteProcedure.AppendLine("            WHERE   id = OBJECT_ID(N'[" + DefaultSchema + "].[sp" + tableName + "_Delete]')");
            deleteProcedure.AppendLine("                    AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )");
            deleteProcedure.AppendLine("    DROP PROCEDURE [" + DefaultSchema + "].[sp" + tableName + "_Delete];");
            deleteProcedure.AppendLine("GO");
            deleteProcedure.AppendLine();
            deleteProcedure.AppendLine("SET ANSI_NULLS OFF;");
            deleteProcedure.AppendLine("GO");
            deleteProcedure.AppendLine("SET QUOTED_IDENTIFIER OFF;");
            deleteProcedure.AppendLine("GO");
            deleteProcedure.Append("CREATE    PROCEDURE [" + DefaultSchema + "].[sp" + tableName + "_Delete] ( @val1 ");
            deleteProcedure.Append(fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper());
            if (fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("DECIMAL") || fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("NUMERIC"))
            {
                deleteProcedure.Append("(" + fields.First(z => z.IsPrimaryKey).SqlPrecision + ", " + fields.First(z => z.IsPrimaryKey).SqlScale + ")");
            }
            if (fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("VARCHAR") ||
                fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("NVARCHAR") ||
                fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("CHAR") ||
                fields.First(z => z.IsPrimaryKey).IntrinsicSqlDataType.ToUpper().Equals("NCHAR"))
            {
                deleteProcedure.Append("(" + fields.First(z => z.IsPrimaryKey).SqlPrecision + ")");
            }
            
            deleteProcedure.AppendLine(" )");
            deleteProcedure.AppendLine("AS");
            deleteProcedure.AppendLine("    DELETE FROM [" + DefaultSchema + "].[" + tableName + "] WHERE [" + fields.First(z => z.IsPrimaryKey).FieldName + "]=@val1;");
            deleteProcedure.AppendLine();
            deleteProcedure.AppendLine("    SET QUOTED_IDENTIFIER ON;");
            deleteProcedure.AppendLine("GO");
            deleteProcedure.AppendLine();

            return deleteProcedure.ToString();
        }

        private string CreateStoredProcedures(string tableName, List<Field> fields, bool isView)
        {
            var procedures = new StringBuilder();

            if (!isView)
            {
                procedures.AppendLine(CreateSelectStoredProcedure(tableName, fields));
                procedures.AppendLine(CreateInsertStoredProcedure(tableName, fields));
                procedures.AppendLine(CreateUpdateStoredProcedure(tableName, fields));
                procedures.AppendLine(CreateDeleteStoredProcedure(tableName, fields));
            }
            procedures.AppendLine(CreateGetAllStoredProcedure(tableName));

            
            return procedures.ToString();
        }

        private class Field
        {
            public string FieldName { get; set; }
            public string CsDataType { get; set; }
            public string VbDataType { get; set; }
            public string JavaDataType { get; set; }
            public string JavaMappingType { get; set; }
            public bool IsPrimaryKey { get; set; }
            public bool IsIdentity { get; set; }
            public string SafeCsPropertyName { get; set; }
            public string SafeJavaPropertyName { get; set; }
            public string SafeVbPropertyName { get; set; }
            public string SafeCsFieldName { get; set; }
            public string SafeJavaFieldName { get; set; }
            public string SafeVbFieldName { get; set; }
            public string SqlDbType { get; set; }
            public string IntrinsicSqlDataType { get; set; }
            public int SqlLength { get; set; }
            public short SqlScale { get; set; }
            public int SqlPrecision { get; set; }
            public bool TextBased { get; set; }
            public string Description { get; set; }
            public bool IsComputedField { get; set; }
            public bool CanBeNull { get; set; }
            public bool IsValueType { get; set; }
            public bool UnicodeText{ get; set; }
            public string DefaultValue { get; set; }
        }

        private class ForeignKey
        {
            public string ForeignTableName { get; set; }
            public string PrimaryTableName { get; set; }
            public string ForeignColumnName { get; set; }
        }
    }
}
