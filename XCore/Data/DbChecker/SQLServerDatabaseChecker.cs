//------------------------------------------------------------------------------
//	文件名称：System\Data\DbChecker\SQLServerDatabaseChecker.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.ORM;
namespace System.Data {
    internal class SQLServerDatabaseChecker : IDatabaseChecker {
        private static readonly ILog logger = LogManager.GetLogger( typeof( SQLServerDatabaseChecker ) );
        private String _connectionString;
        private DatabaseType _databaseType;
        public String ConnectionString {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        public DatabaseType DatabaseType {
            get { return _databaseType; }
            set { _databaseType = value; }
        }
        private List<String> existTables = new List<String>();
        public void CheckDatabase() {
            if (strUtil.IsNullOrEmpty( _connectionString ))
                throw new Exception( "[sqlserver] connection String is not found" );
            IDatabaseDialect dialect = DataFactory.GetDialect( DatabaseType.SqlServer );
            if (strUtil.IsNullOrEmpty( dialect.GetConnectionItem( _connectionString, ConnectionItemType.Server ) ))
                throw new Exception( "[sqlserver] address is empty" );
            if (strUtil.IsNullOrEmpty( dialect.GetConnectionItem( _connectionString, ConnectionItemType.Database ) )) 
                throw new Exception( "[sqlserver] database is empty" );
        }
        public void CheckTable( MappingClass mapping, String db ) {
            logger.Info( "[sqlserver] begin check table" );
            SqlConnection connection = new SqlConnection( _connectionString );
            connection.Open();
            OrmHelper.initCount++;
            LogManager.GetLogger("Class:System.Data.SQLServerDatabaseChecker Method:CheckTable").Info("数据库连接已开启【" + OrmHelper.initCount + "】");
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT OBJECT_NAME(id) as name FROM sysobjects WHERE xtype='U' AND OBJECTPROPERTY(id, 'IsMSShipped') = 0";
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) {
                existTables.Add( reader["name"].ToString() );
                logger.Info( "table found：" + reader["name"].ToString() );
            }
            reader.Close();
            existTables = new SqlServerTableBuilder().CheckMappingTableIsExist( cmd, db, existTables, mapping );
            connection.Close();
            OrmHelper.clostCount++;
            LogManager.GetLogger("Class:System.Data.SQLServerDatabaseChecker Method:CheckTable").Info("数据库连接关闭成功【" + OrmHelper.clostCount + "】");
        }
        public List<String> GetTables() {
            return existTables;
        }
    }
}