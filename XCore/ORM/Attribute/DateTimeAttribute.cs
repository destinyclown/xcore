//------------------------------------------------------------------------------
//	文件名称：System\ORM\Attribute\DateTimeAttribute.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
namespace System.ORM {
    /// <summary>
    /// 时间批注，可以保存日期类型数据。
    /// 数据库存储的时候，使用日期类型进行存储
    /// </summary>
    [Serializable, AttributeUsage( AttributeTargets.Property )]
    public class DateTimeAttribute : Attribute{ }
}