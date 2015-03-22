//------------------------------------------------------------------------------
//	文件名称：System\ORM\Attribute\DefaultAttribute.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
namespace System.ORM {
    /// <summary>
    /// 默认值批注，当属性没有被赋值的时候，系统使用此默认值存入数据库
    /// </summary>
    [Serializable, AttributeUsage( AttributeTargets.Property )]
    public class DefaultAttribute : Attribute {
        private Object _value;
        public DefaultAttribute( Object val ) {
            _value = val;
        }
        public Object Value {
            get { return _value; }
            set { _value = value; }
        }
    }
}