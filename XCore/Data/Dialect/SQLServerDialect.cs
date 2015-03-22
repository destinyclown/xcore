//------------------------------------------------------------------------------
//	文件名称：System\Data\Dialect\SQLServerDialect.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
namespace System.Data {
    /// <summary>
    /// sqlserver 特殊语法处理器
    /// </summary>
    public class SQLServerDialect : IDatabaseDialect {
        public String GetConnectionItem( String connectionString, ConnectionItemType connectionItem ) {
            String str = connectionItem.ToString().ToLower().Replace( "userid", "uid" ).Replace( "password", "pwd" );
            String[] strArray = connectionString.ToLower().Split( new char[] { ';' } );
            foreach (String item in strArray) {
                if (item.Trim().ToLower().StartsWith( str ))
                    return item.Replace( str, "" ).Replace( "=", "" ).Replace( " ", "" );
            }
            return null;
        }
        public String GetTimeQuote() {
            return "'";
        }
        public String GetLimit( String sql, int limit ) {
            return sql.ToLower().Replace( "select ", "select top " + limit + " " );
        }
        public String GetLimit( String sql ) {
            return sql;
        }
        public String GetParameter( String parameterName ) {
            return ("@" + parameterName);
        }
        public String GetParameterAdder( String parameterName ) {
            return ("@" + parameterName);
        }
        public String Top {
            get { return "top"; }
        }
        public string GetLeftQuote() {
            return "[";
        }
        public string GetRightQuote() {
            return "]";
        }
    }
}