//------------------------------------------------------------------------------
//	文件名称：System\ORM\Attribute\ColumnAttribute.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
namespace System.ORM {

    /// <summary>
    /// 数据列批注，用于标识属性在数据库中对应的列名称和长度
    /// </summary>
    [Serializable, AttributeUsage( AttributeTargets.Property )]
    public class ColumnAttribute : Attribute {
        private String _columnName;
        private String _label;
        private int _length;
        public ColumnAttribute() {
            _length = 250;
        }
        public String Name {
            get {
                return _columnName;
            }
            set {
                _columnName = value;
            }
        }
        public String Label {
            get {
                return _label;
            }
            set {
                _label = value;
            }
        }
        public int Length {
            get {
                return _length;
            }
            set {
                _length = value;
            }
        }
        public Boolean LengthSetted() {
            return _length > 0 && _length != 250;
        }
    }
}