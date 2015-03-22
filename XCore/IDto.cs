//------------------------------------------------------------------------------
//	文件名称：System\IDto.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
namespace System {
    /// <summary>
    /// DTO(Data Transfer Object) 的接口
    /// </summary>
    public interface IDto {
        /// <summary>
        /// 将实体类赋值给DTO对象
        /// </summary>
        /// <param name="obj"></param>
        void Init( IEntity obj );
        /// <summary>
        /// 从DTO中获取实体类
        /// </summary>
        /// <returns></returns>
        IEntity GetEntity();
        /// <summary>
        /// 创建一个新对象
        /// </summary>
        /// <returns></returns>
        IDto New();
    }
    public interface IDtoFactory {
        IDto CreateDto( String entityTypeName );
        Dictionary<String, IDto> GetDtoMap();
    }
}