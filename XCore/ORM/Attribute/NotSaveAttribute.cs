//------------------------------------------------------------------------------
//	文件名称：System\ORM\Attribute\NotSaveAttribute.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
namespace System.ORM {
    /// <summary>
    /// ORM在保存数据的时候，会忽略打上 NotSave 批注的属性
    /// </summary>
    [Serializable, AttributeUsage( AttributeTargets.Property | AttributeTargets.Class )]
    public class NotSaveAttribute : Attribute {}
}