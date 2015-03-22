//------------------------------------------------------------------------------
//	文件名称：System\Lang\LanguageSetting.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Web;

namespace System {

    /// <summary>
    /// 某个语言包配置文件的内容，包括一个名称和一个语言包的 Dictionary
    /// </summary>
    public class LanguageSetting {

        private String name;
        private Dictionary<String, String> langMap;

        public LanguageSetting( String name, Dictionary<String, String> lang ) {
            this.name = name;
            this.langMap = lang;
        }

        /// <summary>
        /// 根据 key 获取语言值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String get( String key ) {
            return langMap[key];
        }

        /// <summary>
        /// 获取语言的键值对 Dictionary
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, String> getLangMap() {
            return this.langMap;
        }


    }
}
