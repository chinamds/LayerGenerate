using System;

using LayerGen35.DatabasePlugins;

namespace LayerGen35.DatabasePlugins.MySql
{
#if ENTERPRISE
	using System.EnterpriseServices;
	using System.Runtime.InteropServices;
	[ComVisible(false)]
#endif
	public class ClassFactory : IClassFactory
	{
        public static void Register()
        {
            InternalDriver drv = new InternalDriver
                (typeof(ClassFactory)
                , "Provider=MySQLProv;Persist Security Info=True;Data Source=test;UID=myUser;PWD=myPassword;PORT=3306"
                , true);
            drv.StripTrailingNulls = true;
            drv.RequiredDatabaseName = true;
            InternalDriver.Register("MYSQL",drv);
        }
        public ClassFactory()
		{

		}

		public ITables CreateTables()
		{
			return new MySql.MySqlTables();
		}

		public ITable CreateTable()
		{
			return new MySql.MySqlTable();
		}

		public IColumn CreateColumn()
		{
			return new MySql.MySqlColumn();
		}

		public IColumns CreateColumns()
		{
			return new MySql.MySqlColumns();
		}

		public IDatabase CreateDatabase()
		{
			return new MySql.MySqlDatabase();
		}

		public IDatabases CreateDatabases()
		{
			return new MySql.MySqlDatabases();
		}

		public IProcedure CreateProcedure()
		{
			return new MySql.MySqlProcedure();
		}

		public IProcedures CreateProcedures()
		{
			return new MySql.MySqlProcedures();
		}

		public IView CreateView()
		{
			return new MySql.MySqlView();
		}

		public IViews CreateViews()
		{
			return new MySql.MySqlViews();
		}

		public IParameter CreateParameter()
		{
			return new MySql.MySqlParameter();
		}

		public IParameters CreateParameters()
		{
			return new MySql.MySqlParameters();
		}

		public IForeignKey CreateForeignKey()
		{
			return new MySql.MySqlForeignKey();
		}

		public IForeignKeys CreateForeignKeys()
		{
			return new MySql.MySqlForeignKeys();
		}

		public IIndex CreateIndex()
		{
			return new MySql.MySqlIndex();
		}

		public IIndexes CreateIndexes()
		{
			return new MySql.MySqlIndexes();
		}

		public IResultColumn CreateResultColumn()
		{
			return new MySql.MySqlResultColumn();
		}

		public IResultColumns CreateResultColumns()
		{
			return new MySql.MySqlResultColumns();
		}

		public IDomain CreateDomain()
		{
			return new MySqlDomain();
		}

		public IDomains CreateDomains()
		{
			return new MySqlDomains();
		}

		public IProviderType CreateProviderType()
		{
			return new ProviderType();
		}

		public IProviderTypes CreateProviderTypes()
		{
			return new ProviderTypes();
		}
        public System.Data.IDbConnection CreateConnection()
        {
            return new System.Data.OleDb.OleDbConnection();
        }

        public void ChangeDatabase(System.Data.IDbConnection connection, string database)
        {
            connection.ChangeDatabase(database);
        }
    }
}
