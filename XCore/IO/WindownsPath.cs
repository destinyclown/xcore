//------------------------------------------------------------------------------
//	文件名称：System\IO\WindownsPath.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
namespace System.IO {
    /// <summary>
    /// Windows系统下的文件路径
    /// </summary>
    internal class WindowsPath : PathTool
    {
        /// <summary>
        /// 获取字符串格式的路径（Windows为“\”）
        /// </summary>
        /// <param name="arrPath">分级的目录路径</param>
        /// <returns>完整的字符串</returns>
        public override String CombineAbs( String[] arrPath ) {
            if (arrPath.Length == 0) return "";
            String result = arrPath[0];
            for (int i = 1; i < arrPath.Length; i++) {
                if (strUtil.IsNullOrEmpty( arrPath[i] )) continue;
                result = strUtil.Join( result, arrPath[i].Replace( "/", "\\" ), "\\" );
            }
            return result;
        }
        /// <summary>
        /// 将路径转换为物理路径（Windows为“\”）
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>物理路径</returns>
        public override String Map( String path ) {
            if (strUtil.IsNullOrEmpty( path )) return "";
            if (SystemInfo.IsWeb == false)
            {
                return strUtil.Join(AppDomain.CurrentDomain.BaseDirectory, path.Replace("~", "").Replace("/", "\\"), "\\");
            }
            else
            {
                return strUtil.Join(AppDomain.CurrentDomain.BaseDirectory, path.Replace("~", "").Replace("/", "\\"), "\\");
                String str = path.Replace("~", "");
                if (str.IndexOf(':') < 0)
                {
                    if (str.ToLower().StartsWith(SystemInfo.ApplicationPath) == false)
                        str = strUtil.Join(SystemInfo.ApplicationPath, str);
                    return HttpContext.Current.Server.MapPath(str);
                }
                else
                    return str;
            }
        }
    }
}