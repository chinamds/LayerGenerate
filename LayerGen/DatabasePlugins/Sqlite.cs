using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace LayerGen.DatabasePlugins
{
    public class Sqlite : IDatabasePlugin
    {
        private delegate void SetTextCallback(int percentage);

        private int _progressNdx;

        public DatabaseTypes DatabaseType
        {
            get { return DatabaseTypes.Sqlite; }
        }
        public Languages Language { get; set; }
        public bool HasDynamicDataRetrieval { get; set; }
        public bool AutoRightTrimStrings { get; set; }
        public bool AllowSerialization { get; set; }
        public string DatabaseName { get; set; }
        public string OutputDirectory { get; set; }
        public string DatabaseServer { get; set; }
        public int DatabasePort { get; set; }
        public bool HasCustomConnectionString { get; set; }
        public string Objects { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool TrustedConnection { get; set; }
        public bool IncludeComments { get; set; }
        public ProgressBar ProgressBar { get; set; }
        public string DataNamespaceName { get; set; }
        public string BusinessNamespaceName { get; set; }
        public string CustomConnectionString { get; set; }

        private string ConnectionString
        {
            get
            {
                if (HasCustomConnectionString)
                    return CustomConnectionString;

                var builder = new SQLiteConnectionStringBuilder
                {
                    DataSource = DatabaseName,
                    JournalMode = SQLiteJournalModeEnum.Memory,
                    Password = string.IsNullOrEmpty(Password) ? null : Password.Trim()
                };

                return builder.ConnectionString;
            }
        }

        public void CreateLayers()
        {
            _progressNdx = 0;
            UpdateProgress(0, 1);

            if (Language == Languages.CSharp)
            {
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
        }

        public void CreateCsUniversalFile()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var universal1Template = new StringBuilder();
            var universal2Template = new StringBuilder();

            using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.Universal1SqliteCs.txt"))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        universal1Template.Append(reader.ReadToEnd());
                    }
                }
            }

            using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.Universal2SqliteCs.txt"))
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
                    sw.WriteLine("using System.Data.SQLite;");
                    sw.WriteLine("using System.Dynamic;");
                    sw.WriteLine("using System.Linq;");
                    sw.WriteLine("using System.Reflection;");
                    sw.WriteLine();
                }
                sw.WriteLine("namespace " + DataNamespaceName);
                sw.WriteLine("{");
                sw.WriteLine("    internal static class Universal");
                sw.WriteLine("    {");
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

            using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.Universal1SqliteVb.txt"))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        universal1Template.Append(reader.ReadToEnd());
                    }
                }
            }

            using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.Universal2SqliteVb.txt"))
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
                sw.WriteLine("Option Explicit On");
                sw.WriteLine("Option Strict On");
                sw.WriteLine();
                if (HasDynamicDataRetrieval)
                {
                    sw.WriteLine("Imports System.Collections.Generic");
                    sw.WriteLine("Imports System.Data");
                    sw.WriteLine("Imports System.Data.SQLite");
                    sw.WriteLine("Imports System.Dynamic");
                    sw.WriteLine("Imports System.Linq");
                    sw.WriteLine("Imports System.Reflection");
                    sw.WriteLine();
                }
                sw.WriteLine("Namespace " + DataNamespaceName);
                sw.WriteLine("    Friend NotInheritable Class Universal");
                sw.WriteLine("        Private Sub New()");
                sw.WriteLine("        End Sub");
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
                    sw.WriteLine("        ''' Base 64 encoded BSON");
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
            int percentage = (int)((double)ndx / total * 100);

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

                using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.BusinessLayer.SQLiteCSharp.txt"))
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
                        businessLayerTemplate.Replace("{1}", fields.First(z => z.IsPrimaryKey).CsDataType);
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

                using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.BusinessLayer.SQLiteVbNet.txt"))
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
            foreach (string objectName in Objects.Split(';'))
            {
                UpdateProgress(_progressNdx, Objects.Split(';').Length * 2);
                _progressNdx++;
                var assembly = Assembly.GetExecutingAssembly();
                StringBuilder dataLayerTemplate = new StringBuilder();

                using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.DataLayer.SQLiteCSharp.txt"))
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
                using (StreamWriter sw = File.CreateText(OutputDirectory + "\\" + objName.ToProperFileName() + "Data.cs"))
                {
                    dataLayerTemplate.Replace("{1}", Common.GetSafeCsName(Common.GetCsPropertyName(objName)));

                    var fieldsPart = new StringBuilder();
                    foreach (var field in fields)
                    {
                        fieldsPart.Append("        private " + field.CsDataType + " " + field.SafeCsFieldName + ";" + Environment.NewLine);
                    }
                    dataLayerTemplate.Replace("{2}", fieldsPart.ToString());

                    dataLayerTemplate.Replace("{3}", objName);

                    var fieldListPart = new StringBuilder();
                    foreach (var field in fields.Where(z => (!z.IsIdentity) && (!z.IsComputedField)).OrderBy(z => z.FieldName))
                    {
                        fieldListPart.Append("[" + field.FieldName + "],");
                    }
                    dataLayerTemplate.Replace("{4}", fieldListPart.ToString().TrimEnd(','));

                    var valPart = new StringBuilder();
                    int i = 1;
                    foreach (Field field in fields.Where(z => (!z.IsIdentity) && (!z.IsComputedField)).OrderBy(z => z.FieldName))
                    {
                        valPart.Append("@val" + i + ",");
                        i++;
                    }
                    dataLayerTemplate.Replace("{5}", valPart.ToString().TrimEnd(','));
                    if (!isView)
                    {
                        dataLayerTemplate.Replace("{6}", fields.First(z => z.IsPrimaryKey).FieldName);
                    }

                    var propertiesPart = new StringBuilder();
                    foreach (Field field in fields.OrderByDescending(z => z.IsPrimaryKey))
                    {
                        if (!string.IsNullOrEmpty(field.Description))
                        {
                            propertiesPart.Append("        /// <summary>" + Environment.NewLine);
                            propertiesPart.Append("        /// " + field.Description + Environment.NewLine);
                            propertiesPart.Append("        /// </summary>" + Environment.NewLine);
                        }
                        propertiesPart.Append("        public virtual " + field.CsDataType + " " + field.SafeCsPropertyName + Environment.NewLine);
                        propertiesPart.Append("        {" + Environment.NewLine);
                        propertiesPart.Append("            get { return " + field.SafeCsFieldName + "; }" + Environment.NewLine);

                        if (field.IsIdentity && (!isView))
                        {
                            propertiesPart.Append("            protected set { " + field.SafeCsFieldName + " = value; _isDirty = true; }" + Environment.NewLine);
                        }
                        else if ((!field.IsComputedField) && (!isView))
                        {
                            if (field.CsDataType == "DateTime")
                                propertiesPart.Append("            set { " + field.SafeCsFieldName + " = value; _isDirty = true; if(value == DateTime.MinValue) SetNull(" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + "); else UnsetNull(" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + "); }" + Environment.NewLine);
                            else if (field.CsDataType == "string")
                                propertiesPart.Append("            set { " + field.SafeCsFieldName + " = value; _isDirty = true; if(value == null) SetNull(" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + "); else UnsetNull(" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + "); }" + Environment.NewLine);
                            else propertiesPart.Append("            set { " + field.SafeCsFieldName + " = value; _isDirty = true; UnsetNull(" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + "); }" + Environment.NewLine);
                        }

                        if (field.IsComputedField || isView)
                        {
                            propertiesPart.Append("            protected set { " + field.SafeCsFieldName + " = value; _isDirty = true; }" + Environment.NewLine);
                        }
                        propertiesPart.Append("        }" + Environment.NewLine + Environment.NewLine);
                    }

                    dataLayerTemplate.Replace("{7}", propertiesPart.ToString());

                    if (!isView)
                    {
                        string pkCsName = fields.First(z => z.IsPrimaryKey).SafeCsFieldName;
                        dataLayerTemplate.Replace("{8}", pkCsName);
                        if (fields.First(z => z.IsPrimaryKey).TextBased)
                        {
                            dataLayerTemplate.Replace("{88}", "'\" + " + pkCsName + " + \"'\";");
                        }
                        else
                        {
                            dataLayerTemplate.Replace("{88}", "\" + " + pkCsName + ";");
                        }
                    }

                    var nullDictionaryPart = new StringBuilder("            _nullDictionary = new Dictionary<" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields, bool>" + Environment.NewLine);
                    nullDictionaryPart.Append("            {" + Environment.NewLine);
                    foreach (var field in fields)
                        nullDictionaryPart.Append("                {" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + ", true}," + Environment.NewLine);
                    dataLayerTemplate.Replace("{9}", nullDictionaryPart.ToString().TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd(',') + Environment.NewLine + "            };");

                    string internalNamePart = "            _internalNameDictionary = new Dictionary<" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields, string>" + Environment.NewLine;
                    internalNamePart = internalNamePart + "            {" + Environment.NewLine;
                    internalNamePart = fields.Aggregate(internalNamePart, (current, field) => current + "                {" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + ", \"" + field.FieldName + "\"}," + Environment.NewLine);
                    internalNamePart = internalNamePart.TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd(',') + "";
                    internalNamePart = internalNamePart + Environment.NewLine + "            };";
                    dataLayerTemplate.Replace("{10}", internalNamePart);

                    var fillPart = new StringBuilder();
                    foreach (Field field in fields)
                    {
                        fillPart.Append("            if (HasField(_internalNameDictionary[" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + "], dr))" + Environment.NewLine);
                        fillPart.Append("            {" + Environment.NewLine);
                        fillPart.Append("                if (dr[_internalNameDictionary[" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + "]] == DBNull.Value)" + Environment.NewLine);
                        fillPart.Append("                {" + Environment.NewLine);
                        fillPart.Append("                    SetNull(" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + ");" + Environment.NewLine);
                        fillPart.Append("                }" + Environment.NewLine);
                        fillPart.Append("                else" + Environment.NewLine);
                        fillPart.Append("                {" + Environment.NewLine);
                        fillPart.Append("                    " + field.SafeCsPropertyName + " = (" + field.CsDataType + ") dr[_internalNameDictionary[" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + "]];" + Environment.NewLine);
                        if (field.TextBased && AutoRightTrimStrings)
                        {
                            fillPart.AppendLine("                    " + field.SafeCsPropertyName + " = " + field.SafeCsPropertyName + ".TrimEnd();");
                        }
                        fillPart.Append("                    UnsetNull(" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + ");" + Environment.NewLine);
                        fillPart.Append("                }" + Environment.NewLine);
                        fillPart.Append("            }" + Environment.NewLine);
                        fillPart.Append("            else" + Environment.NewLine);
                        fillPart.Append("            {" + Environment.NewLine);
                        fillPart.Append("                _isReadOnly = true;" + Environment.NewLine);
                        fillPart.Append("                SetNull(" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + ");" + Environment.NewLine);
                        fillPart.Append("            }" + Environment.NewLine + Environment.NewLine);
                    }

                    dataLayerTemplate.Replace("{11}", fillPart.ToString());

                    var getPart = new StringBuilder();
                    foreach (Field field in fields.OrderByDescending(z => z.IsPrimaryKey))
                    {
                        if (field.IsPrimaryKey)
                        {
                            getPart.Append("                            " + field.SafeCsPropertyName + " = (" + field.CsDataType + ") reader[\"" + field.FieldName + "\"];" + Environment.NewLine);
                            getPart.Append("                            UnsetNull(" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + ");" + Environment.NewLine);
                        }
                        else
                        {
                            getPart.Append("                            if ((!HasField(\"" + field.FieldName + "\", reader)) || reader.IsDBNull(reader.GetOrdinal(\"" + field.FieldName + "\")))" + Environment.NewLine);
                            getPart.Append("                            {" + Environment.NewLine);
                            getPart.Append("                                SetNull(" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + ");" + Environment.NewLine);
                            getPart.Append("                            }" + Environment.NewLine);
                            getPart.Append("                            else" + Environment.NewLine);
                            getPart.Append("                            {" + Environment.NewLine);
                            getPart.Append("                                " + field.SafeCsPropertyName + " = (" + field.CsDataType + ") reader[\"" + field.FieldName + "\"];" + Environment.NewLine);
                            if (field.TextBased && AutoRightTrimStrings)
                            {
                                getPart.AppendLine("                    " + field.SafeCsPropertyName + " = " + field.SafeCsPropertyName + ".TrimEnd();");
                            }
                            getPart.Append("                                UnsetNull(" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + ");" + Environment.NewLine);
                            getPart.Append("                            }" + Environment.NewLine);
                        }
                    }

                    dataLayerTemplate.Replace("{12}", getPart.ToString());

                    var resetToDefaultPart = new StringBuilder();
                    foreach (var z in fields)
                    {
                        if (!z.IsIdentity)
                            resetToDefaultPart.Append("            _nullDictionary[" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + z.SafeCsPropertyName + "] = true;" + Environment.NewLine);
                    }
                    dataLayerTemplate.Replace("{13}", resetToDefaultPart.ToString());

                    var savePart = new StringBuilder();

                    i = 1;
                    foreach (Field field in fields.Where(z => (!z.IsIdentity) && (!z.IsComputedField)).OrderBy(z => z.FieldName))
                    {
                        savePart.Append("                            command.Parameters.Add(IsNull(" + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(objName)) + ".Fields." + field.SafeCsPropertyName + ")" + Environment.NewLine);
                        if (field.CanBeNull)
                            savePart.Append("                                ? new SQLiteParameter(\"@val" + i + "\", DbType." + field.SqlDbType + ") {Value = DBNull.Value}" + Environment.NewLine);
                        else savePart.Append("                                ? new SQLiteParameter(\"@val" + i + "\", DbType." + field.SqlDbType + ") {Value = " + GetDefaultCsValue(field) + "}" + Environment.NewLine);
                        savePart.Append("                                : new SQLiteParameter(\"@val" + i + "\", DbType." + field.SqlDbType + ") {Value = " + field.SafeCsFieldName + "});" + Environment.NewLine);
                        i++;
                    }

                    dataLayerTemplate.Replace("{14}", savePart.ToString());

                    var str1 = new StringBuilder("                            ");

                    if (!isView)
                    {
                        Field f = fields.First(z => z.IsPrimaryKey);
                        if (f.IsIdentity)
                            str1.Append(f.SafeCsFieldName + " = (" + f.CsDataType + ") obj;" + Environment.NewLine);
                    }

                    dataLayerTemplate.Replace("{15}", str1.ToString());

                    str1.Clear();
                    str1.Append("                    const string cmdString = \"UPDATE [\" + LayerGenTableName + \"] SET ");
                    i = 1;
                    foreach (Field field in fields.Where(z => (!z.IsIdentity) && (!z.IsComputedField)).OrderBy(z => z.FieldName))
                    {
                        str1.Append("[" + field.FieldName + "]=@val" + i + ",");
                        i++;
                    }
                    dataLayerTemplate.Replace("{16}", str1.ToString().TrimEnd(',') + " WHERE \" + LayerGenPrimaryKey + \"=@val" + i + "\";" + Environment.NewLine);

                    dataLayerTemplate.Replace("{17}", "                            command.Parameters.AddWithValue(\"@val" + i + "\", _oldPrimaryKeyValue);" + Environment.NewLine);
                    if (!isView)
                    {
                        dataLayerTemplate.Replace("{18}", "        private " + fields.First(z => z.IsPrimaryKey).CsDataType + " _oldPrimaryKeyValue;" + Environment.NewLine);
                        dataLayerTemplate.Replace("{19}", fields.First(z => z.IsPrimaryKey).CsDataType);

                        if (fields.First(z => z.IsPrimaryKey).TextBased)
                        {
                            dataLayerTemplate.Replace("{20}", "            string sql = \"SELECT \" + strFields + \" FROM [\" + LayerGenTableName + \"] WHERE \" + LayerGenPrimaryKey + \"=@val1\";" + Environment.NewLine);
                            dataLayerTemplate.Replace("{28}", "                command.Parameters.AddWithValue(\"@val1\", id);");
                        }
                        else
                        {
                            dataLayerTemplate.Replace("{20}", "            string sql = \"SELECT \" + strFields + \" FROM [\" + LayerGenTableName + \"] WHERE \" + LayerGenPrimaryKey + \"=\" + id;" + Environment.NewLine);
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
                            fkFields.Append("        private " + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(key.PrimaryTableName)) + " _my" + Common.GetSafeCsName(Common.GetCsPropertyName(key.ForeignColumnName)) + ";" + Environment.NewLine);

                            fkProperties.Append("        public " + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(key.PrimaryTableName)) + " F" + Common.GetSafeCsName(Common.GetCsPropertyName(key.ForeignColumnName)) + " {" + Environment.NewLine);
                            fkProperties.Append("            get {" + Environment.NewLine);
                            fkProperties.Append("                if (_my" + Common.GetSafeCsName(Common.GetCsPropertyName(key.ForeignColumnName)) + " == null) {" + Environment.NewLine);
                            fkProperties.Append("                    _my" + Common.GetSafeCsName(Common.GetCsPropertyName(key.ForeignColumnName)) + " = new " + BusinessNamespaceName + "." + Common.GetSafeCsName(Common.GetCsPropertyName(key.PrimaryTableName)) + "(" + Common.GetCsFieldName(key.ForeignColumnName) + ");" + Environment.NewLine);
                            fkProperties.Append("                }" + Environment.NewLine);
                            fkProperties.Append("                return _my" + Common.GetSafeCsName(Common.GetCsPropertyName(key.ForeignColumnName)) + ";" + Environment.NewLine);
                            fkProperties.Append("            }" + Environment.NewLine);
                            fkProperties.Append("        }" + Environment.NewLine + Environment.NewLine);

                            fkDataGetBy.Append("        internal static DataTable GetBy" + Common.GetSafeCsName(Common.GetCsPropertyName(key.ForeignColumnName)) + "(");
                            foreach (Field f in fields)
                            {
                                if (String.Equals(f.FieldName, key.ForeignColumnName, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    sqlDataType = f.SqlDbType;
                                    fkDataGetBy.Append(f.CsDataType + " fkId)" + Environment.NewLine);
                                    break;
                                }
                            }
                            fkDataGetBy.Append("        {" + Environment.NewLine);
                            fkDataGetBy.Append("            using (var connection = new SQLiteConnection(Universal.GetConnectionString()))" + Environment.NewLine);
                            fkDataGetBy.Append("            {" + Environment.NewLine);
                            fkDataGetBy.Append("                using (var command = new SQLiteCommand())" + Environment.NewLine);
                            fkDataGetBy.Append("                {" + Environment.NewLine);
                            fkDataGetBy.Append("                    command.Connection = connection;" + Environment.NewLine);
                            fkDataGetBy.Append("                    command.CommandType = CommandType.Text;" + Environment.NewLine);
                            fkDataGetBy.Append("                    command.CommandText = \"SELECT * FROM [" + key.ForeignTableName + "] WHERE [" + key.ForeignColumnName + "]=@val1\";" + Environment.NewLine);
                            fkDataGetBy.Append("                    command.Parameters.Add(new SQLiteParameter(\"@val1\", DbType." + sqlDataType + ") {Value = fkId});" + Environment.NewLine);
                            fkDataGetBy.Append(Environment.NewLine);
                            fkDataGetBy.Append("                    connection.Open();" + Environment.NewLine);
                            fkDataGetBy.Append("                    using (var adapter = new SQLiteDataAdapter())" + Environment.NewLine);
                            fkDataGetBy.Append("                    {" + Environment.NewLine);
                            fkDataGetBy.Append("                        using (var ds = new DataSet())" + Environment.NewLine);
                            fkDataGetBy.Append("                        {" + Environment.NewLine);
                            fkDataGetBy.Append("                            adapter.SelectCommand = command;" + Environment.NewLine);
                            fkDataGetBy.Append("                            adapter.Fill(ds);" + Environment.NewLine);
                            fkDataGetBy.Append(Environment.NewLine);
                            fkDataGetBy.Append("                            if (ds.Tables.Count > 0)" + Environment.NewLine);
                            fkDataGetBy.Append("                            {" + Environment.NewLine);
                            fkDataGetBy.Append("                                return ds.Tables[0];" + Environment.NewLine);
                            fkDataGetBy.Append("                            }" + Environment.NewLine);
                            fkDataGetBy.Append("                        }" + Environment.NewLine);
                            fkDataGetBy.Append("                    }" + Environment.NewLine);
                            fkDataGetBy.Append("                }" + Environment.NewLine);
                            fkDataGetBy.Append("            }" + Environment.NewLine);
                            fkDataGetBy.Append(Environment.NewLine);
                            fkDataGetBy.Append("            return null;" + Environment.NewLine);
                            fkDataGetBy.Append("        }" + Environment.NewLine);
                        }

                        dataLayerTemplate.Replace("{22}", fkFields.ToString());
                        dataLayerTemplate.Replace("{21}", fkProperties.ToString());
                    }

                    dataLayerTemplate.Replace("{23}", fkDataGetBy.ToString());
                    if (isView)
                    {
                        dataLayerTemplate.Replace("{18}", "");
                        dataLayerTemplate.Replace("{21}", "");
                        dataLayerTemplate.Replace("{22}", "");
                        RemoveTemplateComments(ref dataLayerTemplate);
                    }
                    else
                    {
                        dataLayerTemplate.Replace("{/*}", "");
                        dataLayerTemplate.Replace("{*/}", "");
                    }

                    var equalPart = new StringBuilder();

                    foreach (Field field in fields.OrderByDescending(z => z.IsPrimaryKey))
                    {
                        equalPart.Append("            byte[] cls" + field.SafeCsFieldName.Remove(0, 1) + " = ObjectToByteArray(cls." + field.SafeCsPropertyName + ");" + Environment.NewLine);
                    }
                    equalPart.Append(Environment.NewLine);

                    var tmpEqual = new StringBuilder();
                    if (!isView)
                    {
                        Field primaryKeyField = fields.First(z => z.IsPrimaryKey);
                        dataLayerTemplate.Replace("{25}", primaryKeyField.SafeCsPropertyName);
                    }

                    equalPart.Append("            byte[] clsArray = new byte[");
                    foreach (Field field in fields.OrderByDescending(z => z.IsPrimaryKey))
                    {
                        tmpEqual.Append("cls" + field.SafeCsFieldName.Remove(0, 1) + ".Length + ");
                        if (tmpEqual.Length >= 110)
                        {
                            equalPart.Append(tmpEqual + Environment.NewLine + "                       ");
                            tmpEqual.Clear();
                        }
                    }
                    equalPart.Append(tmpEqual);
                    equalPart.ReplaceAllText(equalPart.ToString().TrimEnd(' ', '\r', '\n', '+'));
                    equalPart.Append("];" + Environment.NewLine);

                    tmpEqual.Clear();
                    tmpEqual.Append("0");

                    foreach (Field field in fields.OrderByDescending(z => z.IsPrimaryKey))
                    {
                        equalPart.Append("            Array.Copy(cls" + field.SafeCsFieldName.Remove(0, 1));
                        equalPart.Append(", 0, clsArray, " + tmpEqual + ", cls" + field.SafeCsFieldName.Remove(0, 1) + ".Length);" + Environment.NewLine);
                        tmpEqual.Append(" + cls" + field.SafeCsFieldName.Remove(0, 1) + ".Length");
                        int teLength = tmpEqual.ToString().Split(Environment.NewLine.ToCharArray()).Length;
                        if (teLength > 0)
                        {
                            string te = tmpEqual.ToString().Split(Environment.NewLine.ToCharArray())[teLength - 1];
                            if (te.Length >= 85)
                            {
                                tmpEqual.Append(Environment.NewLine + "                        ");
                            }
                        }
                    }

                    equalPart.Append(Environment.NewLine);
                    equalPart.Append("            return clsArray;" + Environment.NewLine);

                    dataLayerTemplate.Replace("{24}", equalPart.ToString());

                    dataLayerTemplate.Replace("{26}", DataNamespaceName);
                    dataLayerTemplate.Replace("{27}", BusinessNamespaceName);

                    var serializationCode = new StringBuilder();

                    if (AllowSerialization)
                    {
                        serializationCode.AppendLine("        [Serializable]");
                        serializationCode.AppendLine("        public class " + Common.GetSafeCsName("Serializable" + Common.GetCsPropertyName(objName)));
                        serializationCode.AppendLine("        {");

                        foreach (var field in fields)
                        {
                            if (field.IsValueType)
                                serializationCode.AppendLine("            private " + field.CsDataType + "? " + field.SafeCsFieldName + ";");
                            else
                                serializationCode.AppendLine("            private " + field.CsDataType + " " + field.SafeCsFieldName + ";");
                        }
                        serializationCode.AppendLine("            private bool _serializationIsUpdate;");
                        serializationCode.AppendLine();

                        foreach (var field in fields.OrderByDescending(z => z.IsPrimaryKey))
                        {
                            if (field.IsValueType)
                                serializationCode.AppendLine("            public " + field.CsDataType + "? " + field.SafeCsPropertyName);
                            else
                                serializationCode.AppendLine("            public " + field.CsDataType + " " + field.SafeCsPropertyName);
                            serializationCode.AppendLine("            {");
                            serializationCode.AppendLine("                get { return " + field.SafeCsFieldName + "; }");
                            serializationCode.AppendLine("                set { " + field.SafeCsFieldName + " = value; }");
                            serializationCode.AppendLine("            }");
                        }
                        serializationCode.AppendLine("            /// <summary>");
                        serializationCode.AppendLine("            /// Set this to true if <see cref=\"Save()\"></see> should do an update.");
                        serializationCode.AppendLine("            /// Otherwise, set to false to force <see cref=\"Save()\"></see> to do an insert.");
                        serializationCode.AppendLine("            /// </summary>");
                        serializationCode.AppendLine("            public bool SerializationIsUpdate");
                        serializationCode.AppendLine("            {");
                        serializationCode.AppendLine("                get { return _serializationIsUpdate; }");
                        serializationCode.AppendLine("                set { _serializationIsUpdate = value; }");
                        serializationCode.AppendLine("            }");
                        serializationCode.AppendLine("        }");
                    }

                    dataLayerTemplate.Replace("{32}", serializationCode.ToString());

                    serializationCode.Clear();

                    if (AllowSerialization)
                    {
                        serializationCode.AppendLine("        /// <summary>");
                        serializationCode.AppendLine("        /// Converts an instance of an object to a string format");
                        serializationCode.AppendLine("        /// </summary>");
                        serializationCode.AppendLine("        /// <param name=\"format\">Specifies if it should convert to XML, BSON, or JSON</param>");
                        serializationCode.AppendLine("        /// <returns>The object, converted to a string representation</returns>");
                        serializationCode.AppendLine("        public string ToString(" + BusinessNamespaceName + ".SerializationFormats format)");
                        serializationCode.AppendLine("        {");
                        serializationCode.AppendLine("            var " + Common.GetSafeCsName("serializable" + Common.GetCsPropertyName(objName)) + " = new " + Common.GetSafeCsName("Serializable" + Common.GetCsPropertyName(objName)) + "();");
                        foreach (var field in fields.OrderByDescending(z => z.IsPrimaryKey))
                        {
                            if (field.IsValueType)
                            {
                                serializationCode.AppendLine("            " + Common.GetSafeCsName("serializable" + Common.GetCsPropertyName(objName)) + "." + field.SafeCsPropertyName + " = IsNull(" + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Fields." + field.SafeCsPropertyName + ")");
                                serializationCode.AppendLine("                ? (" + field.CsDataType + "?) null : " + field.SafeCsFieldName + ";");
                            }
                            else if (field.CsDataType == "string")
                            {
                                serializationCode.AppendLine("            " + Common.GetSafeCsName("serializable" + Common.GetCsPropertyName(objName)) + "." + field.SafeCsPropertyName + " = IsNull(" + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Fields." + field.SafeCsPropertyName + ")");
                                serializationCode.AppendLine("                ? null : " + field.SafeCsFieldName + ";");
                            }
                            else
                            {
                                serializationCode.AppendLine("            " + Common.GetSafeCsName("serializable" + Common.GetCsPropertyName(objName)) + "." + field.SafeCsPropertyName + " = " + field.SafeCsFieldName + ";");
                            }
                        }
                        serializationCode.AppendLine("            " + Common.GetSafeCsName("serializable" + Common.GetCsPropertyName(objName)) + ".SerializationIsUpdate = _isUpdate;");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            if (format == " + BusinessNamespaceName + ".SerializationFormats.Json)");
                        serializationCode.AppendLine("            {");
                        serializationCode.AppendLine("                return Newtonsoft.Json.JsonConvert.SerializeObject(" + Common.GetSafeCsName("serializable" + Common.GetCsPropertyName(objName)) + ");");
                        serializationCode.AppendLine("            }");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            if (format == " + BusinessNamespaceName + ".SerializationFormats.Xml)");
                        serializationCode.AppendLine("            {");
                        serializationCode.AppendLine("                var xType = new System.Xml.Serialization.XmlSerializer(" + Common.GetSafeCsName("serializable" + Common.GetCsPropertyName(objName)) + ".GetType());");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("                using (var sw = new StringWriter())");
                        serializationCode.AppendLine("                {");
                        serializationCode.AppendLine("                    xType.Serialize(sw, " + Common.GetSafeCsName("serializable" + Common.GetCsPropertyName(objName)) + ");");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("                    return sw.ToString();");
                        serializationCode.AppendLine("                }");
                        serializationCode.AppendLine("            }");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            if (format == " + BusinessNamespaceName + ".SerializationFormats.BsonBase64)");
                        serializationCode.AppendLine("            {");
                        serializationCode.AppendLine("                using (var ms = new System.IO.MemoryStream())");
                        serializationCode.AppendLine("                {");
                        serializationCode.AppendLine("                    using (var writer = new Newtonsoft.Json.Bson.BsonWriter(ms))");
                        serializationCode.AppendLine("                    {");
                        serializationCode.AppendLine("                        var serializer = new Newtonsoft.Json.JsonSerializer();");
                        serializationCode.AppendLine("                        serializer.Serialize(writer, " + Common.GetSafeCsName("serializable" + Common.GetCsPropertyName(objName)) + ");");
                        serializationCode.AppendLine("                    }");
                        serializationCode.AppendLine("                    return Convert.ToBase64String(ms.ToArray());");
                        serializationCode.AppendLine("                }");
                        serializationCode.AppendLine("            }");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            return \"\";");
                        serializationCode.AppendLine("        }");
                    }

                    dataLayerTemplate.Replace("{33}", serializationCode.ToString());

                    serializationCode.Clear();

                    if (AllowSerialization)
                    {
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("        protected static " + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + " BsonTo" + Common.GetCsPropertyName(objName) + "(string bson)");
                        serializationCode.AppendLine("        {");
                        serializationCode.AppendLine("            " + Common.GetSafeCsName("Serializable" + Common.GetCsPropertyName(objName)) + " z;");
                        serializationCode.AppendLine("            byte[] data = Convert.FromBase64String(bson);");
                        serializationCode.AppendLine("            using (var ms = new System.IO.MemoryStream(data))");
                        serializationCode.AppendLine("            {");
                        serializationCode.AppendLine("                using (var reader = new Newtonsoft.Json.Bson.BsonReader(ms))");
                        serializationCode.AppendLine("                {");
                        serializationCode.AppendLine("                    var serializer = new Newtonsoft.Json.JsonSerializer();");
                        serializationCode.AppendLine("                    z = serializer.Deserialize<" + Common.GetSafeCsName("Serializable" + Common.GetCsPropertyName(objName)) + ">(reader);");
                        serializationCode.AppendLine("                }");
                        serializationCode.AppendLine("            }");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            var tmp = new " + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + "();");
                        serializationCode.AppendLine();
                        foreach (Field field in fields)
                        {
                            if (field.IsValueType)
                            {
                                serializationCode.AppendLine("            if (z." + field.SafeCsPropertyName + ".HasValue)");
                                serializationCode.AppendLine("            {");
                                serializationCode.AppendLine("                tmp." + field.SafeCsFieldName + " = z." + field.SafeCsPropertyName + ".Value;");
                                serializationCode.AppendLine("                tmp.UnsetNull(" + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Fields." + field.SafeCsPropertyName + ");");
                                serializationCode.AppendLine("            } else {");
                                serializationCode.AppendLine("                tmp.SetNull(" + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Fields." + field.SafeCsPropertyName + ");");
                                serializationCode.AppendLine("            }");
                            }
                            else
                            {
                                serializationCode.AppendLine("            if (z." + field.SafeCsPropertyName + " == null)");
                                serializationCode.AppendLine("            {");
                                serializationCode.AppendLine("                tmp.SetNull(" + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Fields." + field.SafeCsPropertyName + ");");
                                serializationCode.AppendLine("            } else {");
                                serializationCode.AppendLine("                tmp." + field.SafeCsFieldName + " = z." + field.SafeCsPropertyName + ";");
                                serializationCode.AppendLine("                tmp.UnsetNull(" + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Fields." + field.SafeCsPropertyName + ");");
                                serializationCode.AppendLine("            }");
                            }
                        }
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            tmp._isUpdate = z.SerializationIsUpdate;");
                        serializationCode.AppendLine("            tmp._isDirty = true;");
                        serializationCode.AppendLine("            return tmp;");

                        serializationCode.AppendLine("        }");
                        serializationCode.AppendLine("        protected static " + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + " XmlTo" + Common.GetCsPropertyName(objName) + "(string xml)");
                        serializationCode.AppendLine("        {");
                        serializationCode.AppendLine("            var xType = new System.Xml.Serialization.XmlSerializer(typeof(" + Common.GetSafeCsName("Serializable" + Common.GetCsPropertyName(objName)) + "));");
                        serializationCode.AppendLine("            " + Common.GetSafeCsName("Serializable" + Common.GetCsPropertyName(objName)) + " z;");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            using (var sr = new StringReader(xml))");
                        serializationCode.AppendLine("            {");
                        serializationCode.AppendLine("                z = (" + Common.GetSafeCsName("Serializable" + Common.GetCsPropertyName(objName)) + ") xType.Deserialize(sr);");
                        serializationCode.AppendLine("            }");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            var tmp = new " + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + "();");
                        serializationCode.AppendLine();
                        foreach (Field field in fields)
                        {
                            if (field.IsValueType)
                            {
                                serializationCode.AppendLine("            if (z." + field.SafeCsPropertyName + ".HasValue)");
                                serializationCode.AppendLine("            {");
                                serializationCode.AppendLine("                tmp." + field.SafeCsFieldName + " = z." + field.SafeCsPropertyName + ".Value;");
                                serializationCode.AppendLine("                tmp.UnsetNull(" + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Fields." + field.SafeCsPropertyName + ");");
                                serializationCode.AppendLine("            } else {");
                                serializationCode.AppendLine("                tmp.SetNull(" + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Fields." + field.SafeCsPropertyName + ");");
                                serializationCode.AppendLine("            }");
                            }
                            else
                            {
                                serializationCode.AppendLine("            if (z." + field.SafeCsPropertyName + " == null)");
                                serializationCode.AppendLine("            {");
                                serializationCode.AppendLine("                tmp.SetNull(" + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Fields." + field.SafeCsPropertyName + ");");
                                serializationCode.AppendLine("            } else {");
                                serializationCode.AppendLine("                tmp." + field.SafeCsFieldName + " = z." + field.SafeCsPropertyName + ";");
                                serializationCode.AppendLine("                tmp.UnsetNull(" + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Fields." + field.SafeCsPropertyName + ");");
                                serializationCode.AppendLine("            }");
                            }
                        }
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            tmp._isUpdate = z.SerializationIsUpdate;");
                        serializationCode.AppendLine("            tmp._isDirty = true;");
                        serializationCode.AppendLine("            return tmp;");

                        serializationCode.AppendLine("        }");

                        serializationCode.AppendLine();
                        serializationCode.AppendLine("        protected static " + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + " JsonTo" + Common.GetCsPropertyName(objName) + "(string json)");
                        serializationCode.AppendLine("        {");
                        serializationCode.AppendLine("            " + Common.GetSafeCsName("Serializable" + Common.GetCsPropertyName(objName)) + " z = Newtonsoft.Json.JsonConvert.DeserializeObject<Serializable" + Common.GetCsPropertyName(objName) + ">(json);");
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            var tmp = new " + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + "();");
                        serializationCode.AppendLine();
                        foreach (Field field in fields)
                        {
                            if (field.IsValueType)
                            {
                                serializationCode.AppendLine("            if (z." + field.SafeCsPropertyName + ".HasValue)");
                                serializationCode.AppendLine("            {");
                                serializationCode.AppendLine("                tmp." + field.SafeCsFieldName + " = z." + field.SafeCsPropertyName + ".Value;");
                                serializationCode.AppendLine("                tmp.UnsetNull(" + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Fields." + field.SafeCsPropertyName + ");");
                                serializationCode.AppendLine("            } else {");
                                serializationCode.AppendLine("                tmp.SetNull(" + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Fields." + field.SafeCsPropertyName + ");");
                                serializationCode.AppendLine("            }");
                            }
                            else
                            {
                                serializationCode.AppendLine("            if (z." + field.SafeCsPropertyName + " == null)");
                                serializationCode.AppendLine("            {");
                                serializationCode.AppendLine("                tmp.SetNull(" + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Fields." + field.SafeCsPropertyName + ");");
                                serializationCode.AppendLine("            } else {");
                                serializationCode.AppendLine("                tmp." + field.SafeCsFieldName + " = z." + field.SafeCsPropertyName + ";");
                                serializationCode.AppendLine("                tmp.UnsetNull(" + BusinessNamespaceName + "." + Common.GetCsPropertyName(objName) + ".Fields." + field.SafeCsPropertyName + ");");
                                serializationCode.AppendLine("            }");
                            }
                        }
                        serializationCode.AppendLine();
                        serializationCode.AppendLine("            tmp._isUpdate = z.SerializationIsUpdate;");
                        serializationCode.AppendLine("            tmp._isDirty = true;");
                        serializationCode.AppendLine("            return tmp;");

                        serializationCode.AppendLine("        }");
                    }

                    dataLayerTemplate.Replace("{34}", serializationCode.ToString());

                    Common.DoComments(ref dataLayerTemplate, "//", IncludeComments);

                    sw.Write(dataLayerTemplate);
                }
            }
        }

        private void CreateVbDataLayers()
        {
            foreach (string objectName in Objects.Split(';'))
            {
                UpdateProgress(_progressNdx, Objects.Split(';').Length * 2);
                _progressNdx++;

                var assembly = Assembly.GetExecutingAssembly();
                var dataLayerTemplate = new StringBuilder();

                using (Stream stream = assembly.GetManifestResourceStream("LayerGen.Templates.DataLayer.SQLiteVbNet.txt"))
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
                    dataLayerTemplate = dataLayerTemplate.Replace("{1}", Common.GetSafeVbName(Common.GetVbPropertyName(objName)));

                    var fieldsPart = new StringBuilder();
                    foreach (var field in fields)
                    {
                        fieldsPart.Append("        Private " + field.SafeVbFieldName + " As " + field.VbDataType + Environment.NewLine);
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
                                getPart.AppendLine("                    " + field.SafeVbPropertyName + " = " + field.SafeVbPropertyName + ".TrimEnd()");
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
                        savePart.Append("                                Dim param As New SQLiteParameter(\"@val" + i + "\", DbType." + field.SqlDbType + ")" + Environment.NewLine);
                        savePart.Append("                                param.Value = DBNull.Value" + Environment.NewLine);
                        savePart.Append("                                command.Parameters.Add(param)" + Environment.NewLine);
                        savePart.Append("                            Else" + Environment.NewLine);
                        savePart.Append("                                Dim param As New SQLiteParameter(\"@val" + i + "\", DbType." + field.SqlDbType + ")" + Environment.NewLine);
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
                    str1.Append("                    Const cmdString As String = \"UPDATE [\" & LayerGenTableName & \"] SET ");
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
                            dataLayerTemplate = dataLayerTemplate.Replace("{20}", "            Dim sql As String = \"SELECT \" & strFields & \" FROM [\" & LayerGenTableName & \"] WHERE \" & LayerGenPrimaryKey & \"=@val1\"" + Environment.NewLine);
                            dataLayerTemplate.Replace("{28}", "                    command.Parameters.AddWithValue(\"@val1\", id)");
                        }
                        else
                        {
                            dataLayerTemplate = dataLayerTemplate.Replace("{20}", "            Dim sql As String = \"SELECT \" & strFields & \" FROM [\" & LayerGenTableName & \"] WHERE \" & LayerGenPrimaryKey & \"=\" & id" + Environment.NewLine);
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
                            fkDataGetBy.Append("            Using connection As New SQLiteConnection(Universal.GetConnectionString())" + Environment.NewLine);
                            fkDataGetBy.Append("                Using command As New SQLiteCommand()" + Environment.NewLine);
                            fkDataGetBy.Append("                    command.Connection = connection" + Environment.NewLine);
                            fkDataGetBy.Append("                    command.CommandType = CommandType.Text" + Environment.NewLine);
                            fkDataGetBy.Append("                    command.CommandText = \"SELECT * FROM [" + key.ForeignTableName + "] WHERE [" + key.ForeignColumnName + "]=@val1\"" + Environment.NewLine);
                            fkDataGetBy.Append("                    command.Parameters.Add(New SQLiteParameter(\"@val1\", DbType." + sqlDataType + ") With { _" + Environment.NewLine);
                            fkDataGetBy.Append("                        .Value = fkId _" + Environment.NewLine);
                            fkDataGetBy.Append("                    })" + Environment.NewLine);
                            fkDataGetBy.Append(Environment.NewLine);
                            fkDataGetBy.Append("                    connection.Open()" + Environment.NewLine);
                            fkDataGetBy.Append("                    Using adapter As New SQLiteDataAdapter()" + Environment.NewLine);
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
                        equalPart.Append("            Dim cls" + field.SafeVbFieldName.Remove(0, 1) + " As Byte() = ObjectToByteArray(cls." + field.SafeVbPropertyName + ")" + Environment.NewLine);
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

            if (csType == "Guid")
                return "Guid.Empty";

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

        private List<Field> MapFields(string tableName)
        {
            var fields = new List<Field>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (DataTable columns = connection.GetSchema("Columns"))
                {
                    foreach (DataRow row in columns.Rows)
                    {
                        if (((string) row["TABLE_NAME"]).ToLower() != tableName.ToLower())
                            continue;

                        var field = new Field();
                        field.FieldName = (string)row["COLUMN_NAME"];
                        field.IsPrimaryKey = (bool)row["PRIMARY_KEY"];
                        field.IsIdentity = (bool)row["AUTOINCREMENT"];
                        field.CsDataType = GetCsDataType(((string)row["DATA_TYPE"]).Trim(), field.IsIdentity);
                        field.VbDataType = GetVbDataType(((string)row["DATA_TYPE"]).Trim(), field.IsIdentity);
                        field.TextBased = IsTextBased(GetCsDataType(((string)row["DATA_TYPE"]).Trim(), field.IsIdentity));
                        field.SafeCsFieldName = Common.GetSafeCsName(Common.GetCsFieldName(field.FieldName));
                        field.SafeCsPropertyName = Common.GetSafeCsName(Common.GetCsPropertyName(field.FieldName));
                        field.SafeVbFieldName = Common.GetSafeVbName(Common.GetVbFieldName(field.FieldName));
                        field.SafeVbPropertyName = Common.GetSafeVbName(Common.GetVbPropertyName(field.FieldName));
                        field.SqlDbType = GetSqlDbTypeFromSqlType(((string)row["DATA_TYPE"]).Trim());
                        field.IntrinsicSqlDataType = ((string) row["DATA_TYPE"]).Trim();
                        field.IsValueType = Common.IsValueType(field.CsDataType);
                        // A column declared INTEGER PRIMARY KEY will autoincrement: http://www.sqlite.org/faq.html#q1
                        if (field.IntrinsicSqlDataType.ToLower() == "integer" && field.IsPrimaryKey)
                        {
                            field.IsIdentity = true;
                        }
                        fields.Add(field);
                    }
                }
            }

            return fields;
        }

        private bool IsView(string tableName)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (DataTable views = connection.GetSchema("Views"))
                {
                    if (views.Rows.Cast<DataRow>().Any(row => ((string)row["TABLE_NAME"]).ToLower() == tableName.ToLower()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private List<ForeignKey> GetForeignKeys(string tableName)
        {
            var keys = new List<ForeignKey>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (DataTable columns = connection.GetSchema("ForeignKeys"))
                {
                    foreach (DataRow row in columns.Rows)
                    {
                        if (!String.Equals(((string)row["TABLE_NAME"]), tableName, StringComparison.CurrentCultureIgnoreCase))
                            continue;
                        var fk = new ForeignKey
                        {
                            ForeignColumnName = (string)row["FKEY_FROM_COLUMN"],
                            ForeignTableName = (string)row["TABLE_NAME"],
                            PrimaryTableName = (string)row["FKEY_TO_TABLE"]
                        };

                        keys.Add(fk);
                    }
                }
            }

            return keys;
        }

        private bool IsTextBased(string csType)
        {
            return csType.ToLower() == "string";
        }

        private string GetSqlDbTypeFromSqlType(string sqlType)
        {
            sqlType = sqlType.Trim().ToLower();

            switch (sqlType)
            {
                case "tinyint":
                    return "Byte";
                case "integer":
                case "bigint":
                case "int64":
                case "largeint":
                case "word":
                    return "Int64";
                case "int":
                    return "Int32";
                case "nvarchar":
                case "nvarchar2":
                case "varchar":
                case "varchar2":
                case "text":
                case "char":
                case "nchar":
                case "ntext":
                case "datetext":
                case "clob":
                case "memo":
                case "xml":
                case "string":
                    return "String";
                case "float":
                case "double precision":
                case "double":
                case "smallmoney":
                case "dec":
                    return "Double";
                case "real":
                    return "Double";
                case "decimal":
                case "numeric":
                case "money":
                case "number":
                case "currency":
                    return "Currency";
                case "boolean":
                case "bool":
                case "bit":
                    return "Boolean";
                case "time":
                case "date":
                case "datetime":
                case "timestamp":
                    return "DateTime";
                case "blob":
                case "binary":
                case "blob_text":
                case "image":
                case "graphic":
                case "raw":
                case "photo":
                case "picture":
                case "varbinary":
                    return "Binary";
                case "smallint":
                    return "Int16";
                case "guid":
                case "uniqueidentifier":
                    return "Guid";
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

        private string GetVbDataType(string sqliteDataType, bool isIdentity)
        {
            if (isIdentity)
                return "Long";

            string sqlLiteDataTypeLower = sqliteDataType.ToLower();

            if (sqlLiteDataTypeLower == "tinyint")
                return "Byte";
            if (sqlLiteDataTypeLower == "integer" || sqlLiteDataTypeLower == "bigint" || sqlLiteDataTypeLower == "int64" || sqlLiteDataTypeLower == "largeint" ||
                sqlLiteDataTypeLower == "word")
                return "Long";
            if (sqlLiteDataTypeLower == "int" || sqlLiteDataTypeLower == "tinyint")
                return "Integer";
            if (sqlLiteDataTypeLower == "nvarchar" || sqlLiteDataTypeLower == "varchar" || sqlLiteDataTypeLower == "text" || sqlLiteDataTypeLower == "datetext" ||
                sqlLiteDataTypeLower == "char" || sqlLiteDataTypeLower == "nchar" || sqlLiteDataTypeLower == "clob" || sqlLiteDataTypeLower == "xml" ||
                sqlLiteDataTypeLower == "memo" || sqlLiteDataTypeLower == "ntext" || sqlLiteDataTypeLower == "nvarchar2" || sqlLiteDataTypeLower == "varchar2" ||
                sqlLiteDataTypeLower == "string")
                return "String";
            if (sqlLiteDataTypeLower == "float" || sqlLiteDataTypeLower == "double precision" || sqlLiteDataTypeLower == "double" || sqlLiteDataTypeLower == "dec" ||
                sqlLiteDataTypeLower == "smallmoney")
                return "Double";
            if (sqlLiteDataTypeLower == "real")
                return "Double";
            if (sqlLiteDataTypeLower == "numeric" || sqlLiteDataTypeLower == "currency" || sqlLiteDataTypeLower == "decimal" || sqlLiteDataTypeLower == "money" ||
                sqlLiteDataTypeLower == "number")
                return "Decimal";
            if (sqlLiteDataTypeLower == "boolean" || sqlLiteDataTypeLower == "bit" || sqlLiteDataTypeLower == "bool")
                return "Boolean";
            if (sqlLiteDataTypeLower == "time" || sqlLiteDataTypeLower == "date" || sqlLiteDataTypeLower == "timestamp" || sqlLiteDataTypeLower == "datetime")
                return "DateTime";
            if (sqlLiteDataTypeLower == "blob" || sqlLiteDataTypeLower == "blob_text" || sqlLiteDataTypeLower == "binary" ||
                sqlLiteDataTypeLower == "graphic" || sqlLiteDataTypeLower == "image" || sqlLiteDataTypeLower == "picture" || sqlLiteDataTypeLower == "photo" ||
                sqlLiteDataTypeLower == "raw" || sqlLiteDataTypeLower == "varbinary")
                return "Byte()";
            if (sqlLiteDataTypeLower == "smallint")
                return "Short";
            if (sqlLiteDataTypeLower == "guid" || sqlLiteDataTypeLower == "uniqueidentifier")
                return "Guid";

            return sqliteDataType.ToUpper();
        }

        private string GetCsDataType(string sqliteDataType, bool isIdentity)
        {
            if (isIdentity)
                return "long";

            string sqlLiteDataTypeLower = sqliteDataType.ToLower();

            if (sqlLiteDataTypeLower == "tinyint")
                return "byte";
            if (sqlLiteDataTypeLower == "integer" || sqlLiteDataTypeLower == "bigint" || sqlLiteDataTypeLower == "int64" || sqlLiteDataTypeLower == "largeint" ||
                sqlLiteDataTypeLower == "word")
                return "long";
            if (sqlLiteDataTypeLower == "int" || sqlLiteDataTypeLower == "tinyint")
                return "int";
            if (sqlLiteDataTypeLower == "nvarchar" || sqlLiteDataTypeLower == "varchar" || sqlLiteDataTypeLower == "text" || sqlLiteDataTypeLower == "datetext" ||
                sqlLiteDataTypeLower == "char" || sqlLiteDataTypeLower == "nchar" || sqlLiteDataTypeLower == "clob" || sqlLiteDataTypeLower == "xml" ||
                sqlLiteDataTypeLower == "memo" || sqlLiteDataTypeLower == "ntext" || sqlLiteDataTypeLower == "nvarchar2" || sqlLiteDataTypeLower == "varchar2" ||
                sqlLiteDataTypeLower == "string")
                return "string";
            if (sqlLiteDataTypeLower == "float" || sqlLiteDataTypeLower == "double precision" || sqlLiteDataTypeLower == "double" || sqlLiteDataTypeLower == "dec" ||
                sqlLiteDataTypeLower == "smallmoney")
                return "double";
            if (sqlLiteDataTypeLower == "real")
                return "double";
            if (sqlLiteDataTypeLower == "numeric" || sqlLiteDataTypeLower == "currency" || sqlLiteDataTypeLower == "decimal" || sqlLiteDataTypeLower == "money" ||
                sqlLiteDataTypeLower == "number")
                return "decimal";
            if (sqlLiteDataTypeLower == "boolean" || sqlLiteDataTypeLower == "bit" || sqlLiteDataTypeLower == "bool")
                return "bool";
            if (sqlLiteDataTypeLower == "time" || sqlLiteDataTypeLower == "date" || sqlLiteDataTypeLower == "timestamp" || sqlLiteDataTypeLower == "datetime" ||
                sqlLiteDataTypeLower == "datetext")
                return "DateTime";
            if (sqlLiteDataTypeLower == "blob" || sqlLiteDataTypeLower == "blob_text" || sqlLiteDataTypeLower == "binary" || 
                sqlLiteDataTypeLower == "graphic" || sqlLiteDataTypeLower == "image" || sqlLiteDataTypeLower == "picture" || sqlLiteDataTypeLower == "photo" ||
                sqlLiteDataTypeLower == "raw" || sqlLiteDataTypeLower == "varbinary")
                return "byte[]";
            if (sqlLiteDataTypeLower == "smallint")
                return "short";
            if (sqlLiteDataTypeLower == "guid" || sqlLiteDataTypeLower == "uniqueidentifier")
                return "Guid";

            return sqliteDataType.ToUpper();
        }

        private bool HasPrimaryKey(List<Field> fields)
        {
            return fields.Any(field => field.IsPrimaryKey);
        }

        private class Field
        {
            public string FieldName { get; set; }
            public string CsDataType { get; set; }
            public string VbDataType { get; set; }
            public bool IsPrimaryKey { get; set; }
            public bool IsIdentity { get; set; }
            public string SafeCsPropertyName { get; set; }
            public string SafeVbPropertyName { get; set; }
            public string SafeCsFieldName { get; set; }
            public string SafeVbFieldName { get; set; }
            public string SqlDbType { get; set; }
            public string IntrinsicSqlDataType { get; set; }
            public short SqlScale { get; set; }
            public int SqlPrecision { get; set; }
            public bool TextBased { get; set; }
            public string Description { get; set; }
            public bool IsComputedField { get; set; }
            public bool CanBeNull { get; set; }
            public bool IsValueType { get; set; }
        }

        private class ForeignKey
        {
            public string ForeignTableName { get; set; }
            public string PrimaryTableName { get; set; }
            public string ForeignColumnName { get; set; }
        }
    }
}
