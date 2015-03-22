//------------------------------------------------------------------------------
//	文件名称：System\ORM\Attribute\DateTimeAttribute.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
namespace System.ORM {
    /// <summary>
    /// 长整型批注，可以保存长整型数据。
    /// 数据库存储的时候，使用长整型进行存储
    /// </summary>
    [Serializable, AttributeUsage( AttributeTargets.Property )]
    public class LongAttribute : Attribute{ }
}