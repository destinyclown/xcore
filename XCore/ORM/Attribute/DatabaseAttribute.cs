//------------------------------------------------------------------------------
//	文件名称：System\ORM\Attribute\DatabaseAttribute.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
namespace System.ORM {
    /// <summary>
    /// 数据库批注
    /// </summary>
    [Serializable, AttributeUsage( AttributeTargets.Class )]
    public class DatabaseAttribute : Attribute {
        private String _dbName;
        public DatabaseAttribute( String dbName ) {
            _dbName = dbName;
        }
        public String Database {
            get {
                return _dbName;
            }
            set {
                _dbName = value;
            }
        }
    }
}