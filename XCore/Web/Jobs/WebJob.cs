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
using System.Data;

namespace System.Web.Jobs {

    /// <summary>
    /// 计划任务对象
    /// </summary>
    public class WebJob : CacheObject {

        public WebJob() {
            this.IsRunning = true;
        }
		
		private String _Type;
        /// <summary>
        /// 类的完整名称，比如System.XCenter.Common.Jobs.RefreshServerJob
        /// </summary>
		public String Type { get{return _Type;} set{_Type=value;} }
		
		private int _Interval;
        /// <summary>
        /// 间隔时间。单位:ms
        /// </summary>
		public int Interval { get{return _Interval;} set{_Interval=value;} } 
		
		private Boolean _PageSuffix;
        /// <summary>
        /// 是否运行
        /// </summary>
		public Boolean IsRunning { get{return _PageSuffix;} set{_PageSuffix=value;} }

    }

}
