//------------------------------------------------------------------------------
//	文件名称：WlniaoCMS\Mobirds.TemplateEngine\PageBase.cs
//	运 行 库：2.0.50727.1882
//	代码功能：页面基类
//	最后修改：2011年12月7日 23:35:52
//------------------------------------------------------------------------------
using System;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Security.Cryptography;
namespace System.TemplateEngine
{
    /// <summary>
    /// 模版引擎信息
    /// </summary>
    public static class TempInfo
    {
        /// <summary>
        /// 标签列表
        /// </summary>
        private static Dictionary<String, String> tags;
        /// <summary>
        /// 标签列表
        /// </summary>
        public static Dictionary<String, String> Tags
        {
            get
            {
                if (tags == null)
                {
                    UpdateTags();
                }
                return tags;
            }
            set
            {
                tags = value;
            }
        }
        public static void UpdateTags()
        {
            tags = new Dictionary<string, string>();
            String path = System.PathHelper.Map(cfgHelper.FrameworkRoot + "template/" + System.TemplateEngine.TeConfig.Instance.CurrentSkin + "/skin.xml");
            System.Data.DataTable dt = System.IO.XMLHelper.GetData(path, "Tag");
            if (dt != null && dt.Rows.Count > 0)
            {
                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            tags.Add(dt.Rows[i]["TagName"].ToString(), strUtil.HtmlDecode(dt.Rows[i]["Content"].ToString()));
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
                catch { }
            }
        }
    }
}