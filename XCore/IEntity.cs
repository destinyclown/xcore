//------------------------------------------------------------------------------
//	文件名称：System\IEntity.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
namespace System {
    /// <summary>
    /// 可以被 ORM 持久化的对象，都自动实现了本接口
    /// </summary>
    public interface IEntity {
        /// <summary>
        /// 每一个持久化对象，都具有一个 Id 属性
        /// </summary>
        int Id { get; set; }
        /// <summary>
        /// 获取属性的值(并非通过反射，速度较快)
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        Object get( String propertyName );
        /// <summary>
        /// 设置属性的值(并非通过反射，速度较快)
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="propertyValue">属性的值</param>
        void set( String propertyName, Object propertyValue );
        /// <summary>
        /// 包括对象的元数据，以及在对象查询的时候需要的额外信息，不常用
        /// </summary>
        //ObjectInfo state { get; set; }
    }
}