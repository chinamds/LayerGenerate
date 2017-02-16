using System.Windows.Forms;

namespace LayerGen.DatabasePlugins
{
    public enum Languages
    {
        CSharp,
        VbNet,
        Java
    }

    public enum DatabaseTypes
    {
        Unknown = 0,
        SqlServer = 1,
        Sqlite = 2,
        MySql = 3
    }

    public interface IDatabasePlugin
    {
        bool AllowSerialization { get; set; }
        bool HasDynamicDataRetrieval { get; set; }
        bool HasCustomConnectionString { get; set; }
        bool AutoRightTrimStrings { get; set; }
        string CustomConnectionString { get; set; }
        DatabaseTypes DatabaseType { get; }
        string OutputDirectory { get; set; }
        bool IncludeComments { get; set; }
        string DatabaseServer { get; set; }
        int DatabasePort { get; set; }
        string DatabaseName { get; set; }
        string Objects { get; set; }
        void CreateLayers();
        Languages Language { get; set; }
        string DataNamespaceName { get; set; }
        string BusinessNamespaceName { get; set; }
        ProgressBar ProgressBar { get; set; }
    }
}
