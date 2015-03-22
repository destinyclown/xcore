//------------------------------------------------------------------------------
//	文件名称：System\Data\Factory\MysqlFactory_partial.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Reflection;
namespace System.Data {
    /// <summary>
    /// mysql 数据工厂，获取 Connection, Command, DataAdapter
    /// </summary>
    public partial class MysqlFactory : DbFactoryBase
    {
        internal override IDatabaseChecker GetDatabaseChecker()
        {
            return new MysqlDatabaseChecker();
        }
    }
}