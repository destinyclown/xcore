//------------------------------------------------------------------------------
//	�ļ����ƣ�System\Data\DbChecker\MysqlDatabaseChecker.cs
//	�� �� �⣺2.0.50727.1882
//	����޸ģ�2012��9��8�� 22:15:20
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
                throw new Exception("[MySQL] ���ݿ������ַ���δ����");
            }
            IDatabaseDialect dialect = DataFactory.GetDialect( DatabaseType.MySql );
            if (strUtil.IsNullOrEmpty( dialect.GetConnectionItem( this._connectionString, ConnectionItemType.Server ) )) {
                throw new Exception( "[MySQL] δָ��Ŀ�����ݿ��������ַ" );
            }
            if (strUtil.IsNullOrEmpty( dialect.GetConnectionItem( _connectionString, ConnectionItemType.Database ) )) {
                throw new Exception("[MySQL] δָ��Ŀ�����ݿ�����");
            }
        }
        public void CheckTable( MappingClass mapping, String db ) {
            logger.Info( "[MySQL] �������ݿ����б�" );
            IDbConnection connection = DataFactory.GetConnection(_connectionString, this.DatabaseType);
            connection.Open();
            OrmHelper.initCount++;
            LogManager.GetLogger("Class:System.Data.MysqlDatabaseChecker Method:CheckTable").Info("���ݿ������ѿ�����" + OrmHelper.initCount + "��");
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = "show tables";
            IDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) {
                existTables.Add( reader[0].ToString() );
                logger.Info( "���ر�" + reader[0].ToString() );
            }
            reader.Close();
            existTables = new MySqlTableBuilder().CheckMappingTableIsExist( cmd, db, existTables, mapping );
            connection.Close();
            OrmHelper.clostCount++;
            LogManager.GetLogger("Class:System.Data.MysqlDatabaseChecker Method:CheckTable").Info("���ݿ����ӹرճɹ���" + OrmHelper.clostCount + "��");
        }
        public List<String> GetTables() {
            return existTables;
        }
    }
}