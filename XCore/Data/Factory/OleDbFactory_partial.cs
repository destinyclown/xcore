//------------------------------------------------------------------------------
//	文件名称：System\Data\Factory\AccessFactory_partial.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Data.Common;
namespace System.Data {
    /// <summary>
    /// access 数据工厂，获取 Connection, Command, DataAdapter
    /// </summary>
    public partial class AccessFactory : DbFactoryBase {
        internal override IDatabaseChecker GetDatabaseChecker() {
            return new AccessDatabaseChecker();
        }
    }
}