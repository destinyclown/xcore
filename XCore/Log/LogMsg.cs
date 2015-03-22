//------------------------------------------------------------------------------
//	文件名称：System\Log\LogMsg.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
namespace System.Log {
    /// <summary>
    /// 日志信息(尚未实现，请勿使用)
    /// </summary>
    internal partial class LogMsg : ILogMsg
    {
        private int _id;
        private String _level;
        private DateTime _logTime;
        private String _message;
        private String _typeName;
        public void Insert() {}
        /// <summary>
        /// 日志的编号
        /// </summary>
        public int Id {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// 日志的等级
        /// </summary>
        public String LogLevel {
            get { return _level; }
            set { _level = value; }
        }
        /// <summary>
        /// 日志的时间
        /// </summary>
        public DateTime LogTime {
            get { return _logTime; }
            set { _logTime = value; }
        }
        /// <summary>
        /// 日志的内容
        /// </summary>
        public String Message {
            get { return _message; }
            set { _message = value; }
        }
        /// <summary>
        /// 输出日志的类型名称
        /// </summary>
        public String TypeName {
            get { return _typeName; }
            set { _typeName = value; }
        }
    }
}