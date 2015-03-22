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

using System;
using System.Text;
using System.Web;
using System.Collections.Generic;
using System.XCenter.Web;
using System.XCenter.Web.Mvc;
using System.XCenter.Web.Mvc.Routes;

namespace System.XCenter.ORM {

    /// <summary>
    /// 分页查询的结果集的处理，确定当前页、生成分页栏 html 等 
    /// </summary>
    /// <example>
    /// 只要提供三个参数，就可以获取分页bar<br/>
    /// 1）recordCount (记录总数)  <br/>
    /// 2）pageSize (每页数量)  <br/>
    /// 3）currentPage (当前页码)  <br/>
    /// 然后通过构造函数给 ObjectPage 赋值，然后通过它的 PageBar 属性就可以得到分页栏了。
    /// int recordCount = 302; 
    /// int pageSize = 15;
    /// int currentPage = ctx.route.page;
    /// System.XCenter.ORM.ObjectPage op = new System.XCenter.ORM.ObjectPage( recordCount, pageSize, currentPage );
    /// set( "page", op.PageBar );
    /// </example>
    public partial class ObjectPage
    {

        private void appendLink(StringBuilder sb, int pageNo)
        {
            String path = this.getPath();
            sb.Append(Link.AppendPage(path, pageNo));
        }

        //----------------------------------------------------------

        private String getPath()
        {

            if (_path != null) return _path;

            String routePath = Route.getRoutePath();
            String path = (routePath == null ? CurrentRequest.getRawUrl() : routePath);
            _path = path;
            return path;
        }

    }
}

