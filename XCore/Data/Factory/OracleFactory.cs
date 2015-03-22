//------------------------------------------------------------------------------
//	文件名称：System\Data\Factory\OracleFactory.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OracleClient;
using System.Data.Common;
namespace System.Data {
    /// <summary>
    /// oracle 数据工厂，获取 Connection, Command, DataAdapter
    /// </summary>
    public partial class OracleFactory : DbFactoryBase
    {
        public override IDbConnection GetConnection( String connectionString ) {
            return new OracleConnection( connectionString );
        }
        public override IDbCommand GetCommand( String CommandText ) {
            IDbCommand cmd = new OracleCommand();
            cmd.Connection = cn;
            cmd.CommandText = CommandText;
            setTransaction( cmd );
            return cmd;
        }
        // TODO
        public override IDatabaseDialect GetDialect() {
            throw new Exception( lang.get( "dbNotSupport" ) );
        }
        public override Object SetParameter( IDbCommand cmd, String parameterName, Object parameterValue ) {
            parameterValue = base.processValue( parameterValue );
            // TODO
            //parameterName = new SQLServerDialect().GetParameterAdder( parameterName );
            IDbDataParameter parameter = new OracleParameter( parameterName, parameterValue );
            cmd.Parameters.Add( parameter );
            return parameterValue;
        }
        public override DbDataAdapter GetAdapter() {
            return new OracleDataAdapter( (OracleCommand)cmd );
        }
        public override DbDataAdapter GetAdapter( String CommandText ) {
            return new OracleDataAdapter( (OracleCommand)GetCommand( CommandText ) );
        }
    }
}