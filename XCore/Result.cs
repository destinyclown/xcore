//------------------------------------------------------------------------------
//	文件名称：System\Result.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Text;
namespace System {
    /// <summary>
    /// 对结果信息的封装(有效或错误)，在ORM和MVC中经常被使用
    /// </summary>
    public class Result {
        private Boolean _autoShow;
        private List<String> _errors;
        private String _errorsHtml;
        private String _errorsText;
        private String _errorsJson;
        private Object _info;
        /// <summary>
        /// 
        /// </summary>
        public Result() {
            _autoShow = true;
            _errors = new List<String>();
        }
        /// <summary>
        /// 根据错误信息构建 result
        /// </summary>
        /// <param name="errorMsg"></param>
        public Result( String errorMsg ) {
            //_autoShow = true;
            _errors = new List<String>();
            _errors.Add( errorMsg );
        }
        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="errorMsg"></param>
        public void Add( String errorMsg ) {
            _errors.Add( errorMsg );
        }
        /// <summary>
        /// 合并结果信息
        /// </summary>
        /// <param name="result"></param>
        public void Join( Result result ) {
            foreach (String str in result.Errors) {
                Add( str );
            }
            if (_info == null)
            {
                _info = result.Info;
            }
        }
        /// <summary>
        /// 是否自动显示(默认都是自动显示的，比如表单验证发生错误会自动显示在表单上方)
        /// </summary>
        [ORM.NotSerialize]
        public Boolean AutoShow {
            get { return _autoShow; }
            set { _autoShow = value; }
        }
        /// <summary>
        /// 获取所有错误信息的列表
        /// </summary>
        public List<String> Errors {
            get { return _errors; }
            set { _errors = value; }
        }
        /// <summary>
        /// html 格式的错误信息(封装在一个class=XCenterValidationResultList的无序列表ul中)
        /// </summary>
        [ORM.NotSerialize]
        public String ErrorsHtml {
            get {
                if (IsValid) return null;

                if (_errorsHtml == null) {
                    StringBuilder builder = new StringBuilder( "<ul class=\"XCenterValidationResultList\">" );
                    for (int i = 0; i < Errors.Count; i++) {
                        builder.Append( "<li>" );
                        builder.Append( Errors[i] );
                        builder.Append( "</li>" );
                    }
                    builder.Append( "</ul>" );
                    _errorsHtml = builder.ToString();
                }
                return _errorsHtml;
            }
        }
        /// <summary>
        /// 纯文本格式的错误信息，包括换行符。
        /// </summary>
        [ORM.NotSerialize]
        public String ErrorsText {
            get {
                if (IsValid) return null;

                if (_errorsText == null) {
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < Errors.Count; i++) {
                        builder.Append( Errors[i] );
                        builder.Append( Environment.NewLine );
                    }
                    _errorsText = builder.ToString();
                }
                return _errorsText;
            }
        }
        /// <summary>
        /// Json 格式的错误信息。格式 {"IsValid":false, "Msg":"请填写作者名称,请填写评论内容,验证码错误"}
        /// </summary>
        [ORM.NotSerialize]
        public String ErrorsJson {
            get {
                if (IsValid) return "{}";
                if (_errorsJson == null) {
                    // 格式 {"IsValid":false, "Msg":"请填写作者名称,请填写评论内容,验证码错误"}
                    StringBuilder builder = new StringBuilder( );
                    builder.Append( "{" );
                    builder.Append( "\"IsValid\":false, \"Msg\":\"" );
                    for (int i = 0; i < Errors.Count; i++) {
                        builder.Append( Errors[i].Replace( "\"", "'" ) );
                        if (i < Errors.Count - 1) builder.Append( "," );
                    }
                    builder.Append( "\"}" );
                    _errorsJson = builder.ToString();
                }
                return _errorsJson;
            }
        }
        /// <summary>
        /// 附带的对象
        /// </summary>
        public Object Info {
            get { return _info; }
            set { _info = value; }
        }
        /// <summary>
        /// 结果是否包含错误
        /// </summary>
        [ORM.NotSerialize]
        public Boolean HasErrors {
            get { return (Errors.Count > 0); }
        }
        /// <summary>
        /// 结果是否全部正确有效
        /// </summary>
        [ORM.NotSerialize]
        public Boolean IsValid {
            get { return !HasErrors; }
        }
    }
}

