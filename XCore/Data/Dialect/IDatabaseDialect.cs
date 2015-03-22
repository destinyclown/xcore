//------------------------------------------------------------------------------
//	文件名称：System\Data\Dialect\IDatabaseDialect.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
namespace System.Data {
    /// <summary>
    /// 各种数据库的特殊语法处理接口
    /// </summary>
    public interface IDatabaseDialect {
        String GetConnectionItem( String connectionString, ConnectionItemType connectionItem );
        String GetLimit( String sql, int limit );
        String GetLimit( String sql );
        String GetParameter( String parameterName );
        String GetParameterAdder( String parameterName );
        String GetTimeQuote();
        String GetLeftQuote();
        String GetRightQuote();
        String Top { get; }
    }
}