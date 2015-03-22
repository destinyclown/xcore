//------------------------------------------------------------------------------
//	文件名称：System\Data\Factory\DataFactory.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Data.Common;
namespace System.Data {
    /// <summary>
    /// 数据工厂，可以不用考虑数据库差异而获取 Connection, Command, DataAdapter
    /// </summary>
    public partial class DataFactory
    {
        public static IDbConnection GetConnection(String connectionString, DatabaseType dbtype)
        {
            return DbFactoryBase.Instance(dbtype).GetConnection(connectionString);
        }

        public static IDbCommand GetCommand(String CommandText, IDbConnection cn)
        {
            return DbFactoryBase.Instance(cn).GetCommand(CommandText);
        }

        internal static IDatabaseChecker GetDatabaseChecker(DatabaseType dbtype)
        {
            return DbFactoryBase.Instance(dbtype).GetDatabaseChecker();
        }

        public static IDatabaseDialect GetDialect(DatabaseType dbtype)
        {
            return DbFactoryBase.Instance(dbtype).GetDialect();
        }

        public static Object SetParameter(IDbCommand cmd, String parameterName, Object parameterValue)
        {
            return DbFactoryBase.Instance(cmd).SetParameter(cmd, parameterName, parameterValue);
        }

        public static DbDataAdapter GetAdapter(IDbCommand cmd)
        {
            return DbFactoryBase.Instance(cmd).GetAdapter();
        }

        public static DbDataAdapter GetAdapter(String CommandText, IDbConnection cn)
        {
            return DbFactoryBase.Instance(cn).GetAdapter(CommandText);
        }

    }


}
