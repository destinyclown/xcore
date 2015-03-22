//------------------------------------------------------------------------------
//	文件名称：System\Reflection\IPropertyAccessor.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月11日 22:15:20
//------------------------------------------------------------------------------
using System;
namespace System.Reflection {
    /// <summary>
    /// 属性操作工具
    /// </summary>
    public interface IPropertyAccessor {
        Object Get( Object target );
        void Set( Object target, Object value );
    }
}