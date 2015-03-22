/*
 * Copyright 2012 www.xcenter.cn
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections;
using System.Collections.Generic;
using System.ORM.Caching;

namespace System.ORM {

    /// <summary>
    /// 绕过缓存，直接访问数据库
    /// </summary>
    public class NoCacheDbFinder {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
// ReSharper disable once InconsistentNaming
        public List<T> findAll<T>() {
            var state = new ObjectInfo( typeof( T ) );
            state.includeAll();
            IList list = ObjectDb.FindAll( state );
            return db.getResults<T>( list );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T findById<T>( int id ) {
            if (id < 0) return default( T );
            Object obj = ObjectDb.FindById(id, new ObjectInfo(typeof(T)));
            ObjectPool.Update((IEntity)obj);
            return (T)obj;
        }


    }

}
