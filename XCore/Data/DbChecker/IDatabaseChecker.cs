//------------------------------------------------------------------------------
//	文件名称：System\Data\DbChecker\IDatabaseChecker.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ORM;
namespace System.Data {
    internal interface IDatabaseChecker {
        String ConnectionString { get; set; }
        DatabaseType DatabaseType { get; set; }
        void CheckDatabase();
        void CheckTable( MappingClass mapping, String db );
        List<String> GetTables();
    }
}