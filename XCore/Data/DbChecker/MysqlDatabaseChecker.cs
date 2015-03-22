//------------------------------------------------------------------------------
//	文件名称：System\Data\DbChecker\MysqlDatabaseChecker.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ORM;
namespace System.Data {
    internal class MysqlDatabaseChecker : IDatabaseChecker {
        private static readonly ILog logger = LogManager.GetLogger( typeof( MysqlDatabaseChecker ) );
        private List<String> existTables = new List<String>();
        private String _connectionString;
        public String ConnectionString {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        public DatabaseType DatabaseType {
            get { return DatabaseType.MySql; }
            set { }
        }
        public void CheckDatabase() {
            if (strUtil.IsNullOrEmpty( this._connectionString )) {
                throw new Exception("[MySQL] 数据库连接字符串未设置");
            }
            IDatabaseDialect dialect = DataFactory.GetDialect( DatabaseType.MySql );
            if (strUtil.IsNullOrEmpty( dialect.GetConnectionItem( this._connectionString, ConnectionItemType.Server ) )) {
                throw new Exception( "[MySQL] 未指定目标数据库服务器地址" );
            }
            if (strUtil.IsNullOrEmpty( dialect.GetConnectionItem( _connectionString, ConnectionItemType.Database ) )) {
                throw new Exception("[MySQL] 未指定目标数据库名称");
            }
        }
        public void CheckTable( MappingClass mapping, String db ) {
            logger.Info( "[MySQL] 加载数据库已有表" );
            IDbConnection connection = DataFactory.GetConnection(_connectionString, this.DatabaseType);
            connection.Open();
            OrmHelper.initCount++;
            LogManager.GetLogger("Class:System.Data.MysqlDatabaseChecker Method:CheckTable").Info("数据库连接已开启【" + OrmHelper.initCount + "】");
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = "show tables";
            IDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) {
                existTables.Add( reader[0].ToString() );
                logger.Info( "加载表：" + reader[0].ToString() );
            }
            reader.Close();
            existTables = new MySqlTableBuilder().CheckMappingTableIsExist( cmd, db, existTables, mapping );
            connection.Close();
            OrmHelper.clostCount++;
            LogManager.GetLogger("Class:System.Data.MysqlDatabaseChecker Method:CheckTable").Info("数据库连接关闭成功【" + OrmHelper.clostCount + "】");
        }
        public List<String> GetTables() {
            return existTables;
        }
    }
}