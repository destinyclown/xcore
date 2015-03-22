//------------------------------------------------------------------------------
//	文件名称：System\ORM\Attribute\DecimalAttribute.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
namespace System.ORM {    
    /// <summary>
    /// 用于自定义精度数据，也可以存储自定义精度的货币数值。
    /// </summary>
    [Serializable, AttributeUsage( AttributeTargets.Property )]
    public class DecimalAttribute : Attribute {
		private int _Precision;
        /// <summary>
        /// 数值的精度，即小数点左右的总共位数，但不包括小数点。
        /// </summary>
		public int Precision { get{return _Precision;} set{_Precision=value;} }
		private int _Scale;
        /// <summary>
        /// 小数点右侧的位数
        /// </summary>
		public int Scale { get{return _Scale;} set{_Scale=value;}}
    }
}