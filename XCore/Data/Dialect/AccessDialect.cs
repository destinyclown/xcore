//------------------------------------------------------------------------------
//	文件名称：System\Data\Dialect\AccessDialect.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Web;
namespace System.Data {
    /// <summary>
    /// access 特殊语法处理器
    /// </summary>
    [Serializable]
    public class AccessDialect : IDatabaseDialect {
        public String GetConnectionItem( String connectionString, ConnectionItemType connectionItem ) {
            String str = connectionItem.ToString().ToLower().Replace( "database", "data source" ).Replace( "userid", "user id" );
            String[] arrItem = connectionString.ToLower().Split( new char[] { ';' } );
            foreach (String item in arrItem) {
                if (item.Trim().ToLower().StartsWith( str ))
                    return item.Replace( str, "" ).Replace( "=", "" ).Replace( " ", "" );
            }
            return null;
        }
        public String GetLimit( String sql, int limit ) {
            return sql.ToLower().Replace( "select ", "select top " + limit + " " );
        }
        public String GetLimit( String sql) {
            return sql;
        }
        public String GetTimeQuote() {
            return "#";
        }
        public String GetParameter( String parameterName ) {
            return "?";
        }
        public String GetParameterAdder( String parameterName ) {
            return ("@" + parameterName);
        }
        public static String MapPath( String connectionString ) {
            if (SystemInfo.IsWeb==false)
                return connectionString;
            String connectionItem = new AccessDialect().GetConnectionItem( connectionString, ConnectionItemType.Database );
            String newValue = PathHelper.Map( connectionItem );
            return connectionString.Replace( connectionItem, newValue );
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