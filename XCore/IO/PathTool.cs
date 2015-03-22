//------------------------------------------------------------------------------
//	文件名称：System\IO\PathTool.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
namespace System.IO
{
    /// <summary>
    /// 路径辅助工具
    /// </summary>
    internal abstract class PathTool
    {
        /// <summary>
        /// 获取字符串格式的路径（Windows为“\”，Linux为“/”）
        /// </summary>
        /// <param name="arrPath">分级的目录路径</param>
        /// <returns>完整的字符串</returns>
        public abstract String CombineAbs(String[] arrPath);
        /// <summary>
        /// 将路径转换为物理路径
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>物理路径</returns>
        public abstract String Map(String path);
        /// <summary>
        /// 自动根据系统获取相应的辅助实例
        /// </summary>
        /// <returns></returns>
        public static PathTool getInstance()
        {
            if (SystemInfo.IsWindows) return new WindowsPath();
            return new LinuxPath();
        }
        /// <summary>
        /// 自动根据系统获取相应的辅助实例
        /// </summary>
        /// <returns></returns>
        public static String ServerMap(String path)
        {
            try
            {
                if (SystemInfo.IsWindows)
                    return new WindowsPath().Map(path);
                else
                    return new LinuxPath().Map(path);
            }
            catch
            {
                return new WindowsPath().Map(path);
            }
        }
        /// <summary>
        /// 自动根据系统追加一级目录
        /// </summary>
        /// <returns></returns>
        public static String JoinPath(String path,String dir)
        {
            dir = dir.Replace("\\", "").Replace("/", "");
            try
            {
                if (path.EndsWith("\\") || path.EndsWith("/"))
                {
                    if (SystemInfo.IsWindows)
                        return path += dir + "\\";
                    else
                        return path += dir + "/";
                }
                else
                {
                    if (SystemInfo.IsWindows)
                        return path += "\\" + dir + "\\";
                    else
                        return path += "/" + dir + "/";
                }
            }
            catch
            {
                return path += "\\" + dir + "\\";
            }
        }
        /// <summary>
        /// bin 的绝对路径
        /// </summary>
        /// <returns></returns>
        public static String GetBinDirectory()
        {
            if (SystemInfo.IsWeb)
                return HttpRuntime.BinDirectory;
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}