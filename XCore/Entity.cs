//------------------------------------------------------------------------------
//	文件名称：System\Entity.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ORM;

namespace System {

    /// <summary>
    /// 实体类常用方法
    /// </summary>
    public class Entity {

        /// <summary>
        /// 根据类型全名，创建持久化对象
        /// </summary>
        /// <param name="typeFullName">类型的全名</param>
        /// <returns>返回一个 IEntity 持久化对象</returns>
        public static IEntity New( String typeFullName ) {
            IConcreteFactory factory = (IConcreteFactory)MappingClass.Instance.FactoryList[typeFullName];
            return factory == null ? null : factory.New();
        }

        /// <summary>
        /// 获取类型 t 的元数据信息
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static EntityInfo GetInfo(Type t)
        {
            try
            {
                if (MappingClass.Instance == null)
                {
                    MappingClass.loadByReflection();
                }
                return MappingClass.Instance.ClassList[t.FullName] as EntityInfo;
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(typeof(Entity)).Error(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 获取类型 typeFullName 的元数据信息
        /// </summary>
        /// <param name="typeFullName">类型全名，包括namespace，但不包括程序集</param>
        /// <returns></returns>
        public static EntityInfo GetInfo( String typeFullName ) {
            return MappingClass.Instance.ClassList[typeFullName] as EntityInfo;
        }

        /// <summary>
        /// 获取对象 obj 的元数据信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static EntityInfo GetInfo( Object obj ) {
            return GetInfo( obj.GetType() );
        }

        /// <summary>
        /// 根据全名获取类型
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <returns></returns>
        public static Type GetType( String typeFullName ) {
            return MappingClass.Instance.TypeList[typeFullName] as Type;
        }


    }

}
