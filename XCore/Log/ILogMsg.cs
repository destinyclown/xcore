//------------------------------------------------------------------------------
//	文件名称：System\Log\ILogMsg.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
namespace System.Log {
    /// <summary>
    /// 日志信息接口
    /// </summary>
    public interface ILogMsg {
        /// <summary>
        /// 日志的等级
        /// </summary>
        String LogLevel { get; set; }
        /// <summary>
        /// 日志的时间
        /// </summary>
        DateTime LogTime { get; set; }
        /// <summary>
        /// 日志的内容
        /// </summary>
        String Message { get; set; }
        /// <summary>
        /// 输出日志的类型名称
        /// </summary>
        String TypeName { get; set; }
    }
}