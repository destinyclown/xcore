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
using System.Collections.Generic;
using System.Text;

using System.ORM;
using System.Reflection;
using System.Serialization;
using System.Web;
using System.IO;
using System.Web;
using System.IO;

namespace System.TemplateEngine
{

    /// <summary>
    /// 模板引擎的配置
    /// </summary>
    public class TeConfig {

        private static readonly ILog logger = LogManager.GetLogger(typeof(TeConfig));

        public TeConfig()
        {
            this.PageSuffix = ".aspx";
            this.TemplatePageSuffix = ".html";
            this.TemplateFolder = "template";
            this.CurrentSkin = "default";
            this.ClassPage = "list;page";
            this.ArticlePage = "content";
        }

        /// <summary>
        /// 配置的缓存内容(单例模式缓存)
        /// </summary>
        public static TeConfig Instance = loadConfig( getConfigPath() );
		
		private String _PageSuffix;
        /// <summary>
        /// 需要接管的后缀名(默认为.aspx)
        /// </summary>
		public String PageSuffix { get{return _PageSuffix;} set{_PageSuffix=value;} }
		
		private String _TemplatePageSuffix;
        /// <summary>
        /// 模板文件的后缀名
        /// </summary>
		public String TemplatePageSuffix { get{return _TemplatePageSuffix;} set{_TemplatePageSuffix=value;} }
		
		private String _TemplateFolder;
        /// <summary>
        /// 模板文件所在目录
        /// </summary>
		public String TemplateFolder { get{return _TemplateFolder;} set{_TemplateFolder=value;} }
		
		private String _CurrentSkin;
        /// <summary>
        /// 默认模版目录名称(值为default)
        /// </summary>
		public String CurrentSkin { get{return _CurrentSkin;} set{_CurrentSkin=value;} }
		
		private String _ClassPage;
        /// <summary>
        /// 父级页面列表
        /// </summary>
		public String ClassPage { get{return _ClassPage;} set{_ClassPage=value;} }
		
		private String _ArticlePage;
        /// <summary>
        /// 内容页面列表
        /// </summary>
		public String ArticlePage { get{return _ArticlePage;} set{_ArticlePage=value;} }

        /// <summary>
        /// 反射优化模式，目前只实现了 CodeDom 方式
        /// </summary>
        [NotSerialize]
        internal OptimizeMode OptimizeMode {
            get { return OptimizeMode.CodeDom; }
            set { }
        }

        //----------------------------------------------------------------------

        private static TeConfig loadConfig( String cfgPath ) {

            String str = string.Empty;
            TeConfig tec = null;
            try
            {
                str = file.Read(cfgPath, true);
                tec = JSON.ToObject<TeConfig>(str);
            }
            catch (FileNotFoundException ex)
            {
                tec = new TeConfig();
                file.Write(cfgPath, Json.ToStringEx(tec));
            }
            return tec;
        }
        public static void SetTemplate(String CurrentSkin, String PageSuffix)
        {
            string cfgPath = getConfigPath();
            String str = string.Empty;
            TeConfig tec = null;
            try
            {
                str = file.Read(cfgPath, true);
                tec = JSON.ToObject<TeConfig>(str);
            }
            catch (FileNotFoundException ex)
            {
                tec = new TeConfig();
            }
            tec.PageSuffix = PageSuffix;
            tec.CurrentSkin = CurrentSkin;
            Instance = tec;
            file.Write(cfgPath, Json.ToStringEx(tec));
        }

        private static String getConfigPath() {
            return PathHelper.Map( strUtil.Join( cfgHelper.ConfigRoot, "template.config" ) );
        }


        private static bool IsRelativePath( string connectionItem ) {
            return connectionItem.IndexOf( ":" ) < 0;
        }
        
    }

}
