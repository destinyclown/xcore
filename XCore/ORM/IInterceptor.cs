//------------------------------------------------------------------------------
//	文件名称：System\ORM\IInterceptor.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
namespace System.ORM {
    /// <summary>
    /// 拦截器接口，用户可以通过自定义拦截器，在插入前或插入后等动作中注入自己的处理逻辑
    /// </summary>
    public interface IInterceptor {
        void BeforInsert( IEntity entity );
        void AfterInsert( IEntity entity );
        void BeforUpdate( IEntity entity );
        void AfterUpdate( IEntity entity );
        void BeforUpdateBatch( Type t, String action, String condition );
        void AfterUpdateBatch( Type t, String action, String condition );
        void BeforDelete( IEntity entity );
        void AfterDelete( IEntity entity );
        void BeforDeleteBatch( Type t, String condition );
        void AfterDeleteBatch( Type t, String condition );
    }
}