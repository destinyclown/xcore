//------------------------------------------------------------------------------
//	文件名称：System\Data\Factory\MsSqlDbFactory.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
namespace System.Data {
    /// <summary>
    /// sqlserver 数据工厂，获取 Connection, Command, DataAdapter
    /// </summary>
    public partial class MsSqlDbFactory : DbFactoryBase {
        public override IDbConnection GetConnection( String connectionString ) {
            return new SqlConnection( connectionString );
        }
        public override IDbCommand GetCommand( String CommandText ) {
            IDbCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = CommandText;
            setTransaction( cmd );
            return cmd;
        }
        internal override IDatabaseChecker GetDatabaseChecker()
        {
            return new SQLServerDatabaseChecker();
        }
        public override IDatabaseDialect GetDialect() {
            return new SQLServerDialect();
        }
        public override Object SetParameter( IDbCommand cmd, String parameterName, Object parameterValue ) {
            parameterValue = base.processValue( parameterValue );
            parameterName = new SQLServerDialect().GetParameterAdder( parameterName );
            IDbDataParameter parameter = new SqlParameter( parameterName, parameterValue );
            cmd.Parameters.Add( parameter );
            return parameterValue;
        }
        public override DbDataAdapter GetAdapter() {
            return new SqlDataAdapter( (SqlCommand)cmd );
        }
        public override DbDataAdapter GetAdapter( String CommandText ) {
            return new SqlDataAdapter( (SqlCommand)GetCommand( CommandText ) );
        }
    }
}