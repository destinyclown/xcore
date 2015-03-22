//------------------------------------------------------------------------------
//	文件名称：System\Data\Factory\OracleFactory.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OracleClient;
using System.Data.Common;
namespace System.Data {
    /// <summary>
    /// oracle 数据工厂，获取 Connection, Command, DataAdapter
    /// </summary>
    public partial class OracleFactory : DbFactoryBase
    {
        // TODO
        internal override IDatabaseChecker GetDatabaseChecker() {
            throw new Exception( lang.get( "dbNotSupport" ) );
        }
    }
}