//------------------------------------------------------------------------------
//	文件名称：System\Lang\Lang.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
namespace System {
    /// <summary>
    /// 语言包工具，用于加载多国语言。
    /// </summary>
    /// <remarks>
    /// 默认语言包文件存放在 /xcenter/lang/ 中，比如 /xcenter/lang/zh-cn/ 。只要在 /xcenter/lang/  中新增一个语言包文件夹，则系统将其作为语言包列表自动加载。可添加的语言包名称包括：en-us,en-gb,zh-cn,zh-tw,ja,ko,fr,de,it
    /// </remarks>
    public class lang {
        // 这个一定要放在第一行，以保证第一个加载
        private static Dictionary<String, Dictionary<String, LanguageSetting>> langLocaleAll = getLangLocale();
        /// <summary>
        /// 获取当前语言字符(比如 zh-cn，或 en-us)
        /// </summary>
        /// <returns></returns>
        public static String getLangString() {
            String defaultLang = "zh-cn";
            String langCookie = CurrentRequest.getLangCookie();
            if (strUtil.HasText( langCookie ) && langLocaleAll.ContainsKey( langCookie )) return langCookie;
            if (CurrentRequest.getUserLanguages() == null) return defaultLang;
            String[] reqLangs = CurrentRequest.getUserLanguages();
            if (reqLangs.Length == 0) return defaultLang;
            if (langLocaleAll.ContainsKey(reqLangs[0])) return reqLangs[0];   
            return defaultLang;
        }
        /// <summary>
        /// 获取某 key 的语言值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String get( String key ) {
            try {
                String langStr = getLangString();
                return getCoreLang( langStr ).getLangMap() [key];
            }
            catch (KeyNotFoundException) {
                return key;
            }
        }
        /// <summary>
        /// 获取某 key 的语言值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String getKey(String key)
        {
            try
            {
                String langStr = getLangString();
                return getCoreLang(langStr).getLangMap()[key];
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException("language's key not found: " + key);
            }
        }
        public static String getBySkin(String key)
        {
            string value = "";
            string local = getLangString();
            string _skinname = string.Empty;
            Dictionary<String, String> _core = null;
            Dictionary<String, String> _lang = null;
            try
            {
                _core = (Dictionary<String, String>)System.Caching.SysCache.Get("TemplateLanguageDic");
            }
            catch { }
            try
            {
                _lang = (Dictionary<String, String>)System.Caching.SysCache.Get("TemplateLanguageDic" + local);
            }
            catch { }

            try
            {
                if (_core == null)
                {
                    try
                    {
                        _core = cfgHelper.Read(PathHelper.Map(cfgHelper.FrameworkRoot + System.TemplateEngine.TeConfig.Instance.TemplateFolder + "/" + System.TemplateEngine.TeConfig.Instance.CurrentSkin + "/lang.config"), '=');
                        System.Caching.SysCache.Put("TemplateLanguageDic", _core);
                    }
                    catch
                    {
                        _core = new Dictionary<String, String>();
                        System.Caching.SysCache.Put("TemplateLanguageDic", _core);
                    }
                }
                if (_lang == null)
                {
                    try
                    {
                        _lang = cfgHelper.Read(PathHelper.Map(cfgHelper.FrameworkRoot + System.TemplateEngine.TeConfig.Instance.TemplateFolder + "/" + System.TemplateEngine.TeConfig.Instance.CurrentSkin + "/lang." + local + ".config"), '=');
                        System.Caching.SysCache.Put("TemplateLanguageDic" + local, _lang);
                    }
                    catch
                    {
                        _core = new Dictionary<String, String>();
                        System.Caching.SysCache.Put("TemplateLanguageDic" + local, _core);
                    }
                }
                if (string.IsNullOrEmpty(value))
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        try
                        {
                            value = _lang[key];
                        }
                        catch { }
                        if (string.IsNullOrEmpty(value))
                        {
                            try
                            {
                                value = _core[key];
                            }
                            catch { }
                            if (string.IsNullOrEmpty(value))
                            {
                                try
                                {
                                    value = getKey(key);
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch{}
            return value;
        }
        public static String getBySkin(String skinname, String key)
        {
            string value = "";
            string local = getLangString();
            Dictionary<String, String> _core = null;
            Dictionary<String, String> _lang = null;
            try
            {
                if (_core == null)
                {
                    try
                    {
                        _core = cfgHelper.Read(PathHelper.Map(cfgHelper.FrameworkRoot + System.TemplateEngine.TeConfig.Instance.TemplateFolder + "/" + skinname + "/lang.config"), '=');
                    }
                    catch
                    {
                        _core = new Dictionary<String, String>();
                    }
                }
                if (_lang == null)
                {
                    try
                    {
                        _lang = cfgHelper.Read(PathHelper.Map(cfgHelper.FrameworkRoot + System.TemplateEngine.TeConfig.Instance.TemplateFolder + "/" + skinname + "/lang." + local + ".config"), '=');
                    }
                    catch
                    {
                        _core = new Dictionary<String, String>();
                    }
                }
                if (string.IsNullOrEmpty(value))
                {
                    try
                    {
                        value = _lang[key];
                    }
                    catch { }
                    if (string.IsNullOrEmpty(value))
                    {
                        try
                        {
                            value = _core[key];
                        }
                        catch { }
                    }
                }
            }
            catch { }
            return value;
        }
        /// <summary>
        /// 获取在 core.config 中定义的核心语言包
        /// </summary>
        /// <returns></returns>
        public static LanguageSetting getCoreLang() {
            return getCoreLang( getLangString() );
        }
        private static LanguageSetting getCoreLang( String langStr ) {
            Dictionary<String, LanguageSetting> langlist = langLocaleAll[langStr];
            return langlist["core"];
        }
        /// <summary>
        /// 根据类型 t 获取语言列表
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static LanguageSetting getByApp( Type t ) {
            Dictionary<String, LanguageSetting> langlist = langLocaleAll[getLangString()];
            LanguageSetting result;
            langlist.TryGetValue( t.FullName, out result );
            return result;
        }
        private static Dictionary<String, Dictionary<String, LanguageSetting>> getLangLocale()        {
            System.IO.Directory.CreateDirectory(PathHelper.Map(getLangRootPath()));       
            String dirRoot = PathHelper.Map( getLangRootPath() );
            String[] dirPaths = Directory.GetDirectories( dirRoot );
            if (dirPaths.Length == 0)
            {
                System.IO.Directory.CreateDirectory(PathHelper.Map(getLangRootPath() + "/zh-cn"));
                dirPaths = Directory.GetDirectories(dirRoot);
            }
            Dictionary<String, Dictionary<String, LanguageSetting>> results = new Dictionary<String, Dictionary<String, LanguageSetting>>();
            foreach (String path in dirPaths) {
                String langName = Path.GetFileName( path );
                results.Add( langName.ToLower(), getLangList( path ) );
            }
            return results;
        }
        private static Dictionary<String, LanguageSetting> getLangList( String path ) {
            String[] files = Directory.GetFiles(path);
            if (files.Length == 0)
            {
                System.IO.FileEx.Write(path + "/core.config", "default=Hello World\n");
                files = Directory.GetFiles(path);
            }
            Dictionary<String, LanguageSetting> results = new Dictionary<String, LanguageSetting>();
            foreach (String file in files) {
                if (Path.GetExtension( file ) != ".config") continue;
                String fileName = Path.GetFileNameWithoutExtension( file );
                Dictionary<String, String> _lang = cfgHelper.Read( file, '=' );
                LanguageSetting lbl = new LanguageSetting( fileName, _lang );
                results.Add( fileName, lbl );
            }
            return results;
        }
        private static String getLangRootPath() {
            return cfgHelper.FrameworkRoot + "lang/";
        }
        /// <summary>
        /// 获取所有支持的语言包
        /// </summary>
        /// <returns></returns>
        public static List<Dictionary<String, String>> GetSupportedLang() {
            List<Dictionary<String, String>> list = new List<Dictionary<String, String>>();
            foreach (String key in langLocaleAll.Keys) {
                Dictionary<String, String> pair = new Dictionary<String, String>();
                pair.Add( "Name", GetLangInfo( key ) );
                pair.Add( "Value", key );
                list.Add( pair );
            }
            return list;
        }
        private static String GetLangInfo( String langStr ) {
            if (langMap.ContainsKey( langStr )) return langMap[langStr];
            return langStr;
        }
        private static Dictionary<String, String> langMap = getLangInfo();
        private static Dictionary<String, String> getLangInfo() {
            Dictionary<String, String> map = new Dictionary<String, String>();
            map.Add( "en-us", "English (US)" );
            map.Add( "en-gb", "English (British)" );
            map.Add( "zh-cn", "中文(简体)" ); // skipLang
            map.Add( "zh-tw", "正體中文(繁體)" ); // skipLang
            map.Add( "ja", "日本語" );// skipLang
            map.Add( "ko", "한국어" );
            map.Add( "fr", "Français" );
            map.Add( "de", "Deutsch" );
            map.Add( "it", "Italiano" );
            return map;
        }
    }
}