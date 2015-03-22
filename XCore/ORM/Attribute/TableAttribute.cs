//------------------------------------------------------------------------------
//	文件名称：System\ORM\Attribute\TableAttribute.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
namespace System.ORM {
    /// <summary>
    /// 表名称批注，用于标识对象在数据库中对应的表名称
    /// </summary>
    [Serializable, AttributeUsage( AttributeTargets.Class )]
    public class TableAttribute : Attribute {
        private String _tableName;
        public TableAttribute( String tableName ) {
            _tableName = tableName;
        }
        public String TableName {
            get {
                return _tableName;
            }
            set {
                _tableName = value;
            }
        }
    }
}