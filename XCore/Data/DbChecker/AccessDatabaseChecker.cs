//------------------------------------------------------------------------------
//	文件名称：System\Data\DbChecker\AccessDatabaseChecker.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.ORM;
namespace System.Data {
    internal class AccessDatabaseChecker : IDatabaseChecker {
        private static readonly ILog logger = LogManager.GetLogger( typeof( AccessDatabaseChecker ) );
        private String _connectionString;
        private List<String> existTables = new List<String>();
        public String ConnectionString {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        public DatabaseType DatabaseType {
            get { return DatabaseType.Access; }
            set { }
        }
        public void CheckDatabase() {
            logger.Info( "begin check database" );
            if (strUtil.IsNullOrEmpty( _connectionString )) {
                logger.Info( "connection String is empty. begin to create access database and set connection string" );
                createDatabaseAndSaveConnectionString();
            }
            else {
                String connectionItem = DataFactory.GetDialect( DatabaseType.Access ).GetConnectionItem( _connectionString, ConnectionItemType.Database );
                if (strUtil.IsNullOrEmpty( connectionItem )) {
                    logger.Info( "connection String is found, but database is empty. begin to create access database and set connection string" );
                    createDatabaseAndSaveConnectionString();
                }
                else if (!File.Exists( connectionItem )) {
                    logger.Info( "ConnectionString:" + _connectionString );
                    logger.Info( "the database [" + connectionItem + "] is not found. begin to create access database and set connection string" );
                    DatabaseBuilder.BuildAccessDb4o( connectionItem );
                }
            }
        }
        public void CheckTable( MappingClass mapping, String db ) {
            logger.Info( "[access] begin check table" );
            OleDbConnection connection = DataFactory.GetConnection( _connectionString, this.DatabaseType ) as OleDbConnection;
            connection.Open();
            OrmHelper.initCount++;
            LogManager.GetLogger("Class:System.Data.AccessDatabaseChecker Method:CheckTable").Info("数据库连接已开启【" + OrmHelper.initCount + "】");
            IDbCommand cmd = new OleDbCommand();
            cmd.Connection = connection;
            object[] restrictions = new object[4];
            restrictions[3] = "TABLE";
            DataTable oleDbSchemaTable = connection.GetOleDbSchemaTable( OleDbSchemaGuid.Tables, restrictions );
            foreach (DataRow row in oleDbSchemaTable.Rows) {
                existTables.Add( row["TABLE_NAME"].ToString() );
                logger.Info( "table found：" + row["TABLE_NAME"].ToString() );
            }
            existTables = new AccessTableBuilder().CheckMappingTableIsExist( cmd, db, existTables, mapping );
            connection.Close();
            OrmHelper.clostCount++;
            LogManager.GetLogger("Class:System.Data.AccessDatabaseChecker Method:CheckTable").Info("数据库连接关闭成功【" + OrmHelper.clostCount + "】");
        }
        private void createDatabaseAndSaveConnectionString() {
            _connectionString = DatabaseBuilder.ConnectionStringPrefix + DatabaseBuilder.BuildAccessDb4o();
            logger.Info( "connection String : " + _connectionString );
            DbConfig.SaveConnectionString( _connectionString );
            logger.Info( "the connection String is resetted" );
        }
        public List<String> GetTables() {
            return existTables;
        }
    }
}