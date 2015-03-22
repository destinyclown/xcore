//------------------------------------------------------------------------------
//	文件名称：System\ORM\Attribute\LabelAttribute.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
namespace System.ORM {

    /// <summary>
    /// label 批注，用于表单代码的自动生成
    /// </summary>
    [Serializable, AttributeUsage( AttributeTargets.Class )]
    public class LabelAttribute : Attribute {

        private String _label;

        public LabelAttribute( String lbl ) {
            _label = lbl;
        }

        public String Label {
            get {
                return _label;
            }
            set {
                _label = value;
            }
        }
    }
}

