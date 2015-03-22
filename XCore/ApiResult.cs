using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    /// <summary>
    /// API返回结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T>
    {
        #region 结果是否有效
        private bool _success = false;
        /// <summary>
        /// 结果是否有效
        /// </summary>
        public bool success
        {
            get
            {
                return _success;
            }
            set
            {
                _success = value;
            }
        }
        #endregion

        #region 附带返回的消息
        /// <summary>
        /// 附带返回的消息
        /// </summary>
        private string _message = "暂未对结果赋值";
        public string message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }
        #endregion

        #region 附带返回的数据
        private T _data = default(T);
        /// <summary>
        /// 附带返回的数据
        /// </summary>
        public T data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
        #endregion

        public override string ToString()
        {
            return Json.ToStringEx(this);
        }
    }
}
