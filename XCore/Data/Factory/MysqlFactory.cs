//------------------------------------------------------------------------------
//	文件名称：System\Data\Factory\MysqlFactory.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Reflection;
namespace System.Data {
    /// <summary>
    /// mysql 数据工厂，获取 Connection, Command, DataAdapter
    /// </summary>
    public partial class MysqlFactory : DbFactoryBase
    {
        public override IDbConnection GetConnection(String connectionString)
        {
            return getMySqlConnection(connectionString);
        }
        public override IDbCommand GetCommand(String CommandText)
        {
            IDbCommand cmd = getMySqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = CommandText;
            setTransaction(cmd);
            return cmd;
        }
        public override IDatabaseDialect GetDialect()
        {
            return new MysqlDialect();
        }
        public override Object SetParameter(IDbCommand cmd, String parameterName, Object parameterValue)
        {
            parameterValue = base.processValue(parameterValue);
            parameterName = new MysqlDialect().GetParameterAdder(parameterName);
            IDbDataParameter parameter = getMySqlParameter(parameterName, parameterValue);
            cmd.Parameters.Add(parameter);
            return parameterValue;
        }
        public override DbDataAdapter GetAdapter()
        {
            return getMySqlDataAdapter(cmd);
        }

        public override DbDataAdapter GetAdapter(String CommandText)
        {
            return getMySqlDataAdapter(GetCommand(CommandText));
        }

        //-----------------------------------

        private static IDbCommand getMySqlCommand()
        {
            return (rft.GetInstance(mySqlCommandType) as IDbCommand);
        }

        private static IDbConnection getMySqlConnection(String connectionString)
        {
            return (rft.GetInstance(mySqlConnectionType, new object[] { connectionString }) as IDbConnection);
        }

        private static DbDataAdapter getMySqlDataAdapter(Object cmd)
        {
            return (rft.GetInstance(mySqlDataAssembly.GetType("MySql.Data.MySqlClient.MySqlDataAdapter"), new object[] { cmd }) as DbDataAdapter);
        }

        private static IDbDataParameter getMySqlParameter(String parameterName, Object parameterValue)
        {
            return (rft.GetInstance(mySqlDataAssembly.GetType("MySql.Data.MySqlClient.MySqlParameter"), new object[] { parameterName, parameterValue }) as IDbDataParameter);
        }

        public static Type mySqlCommandType
        {
            get { return mySqlDataAssembly.GetType("MySql.Data.MySqlClient.MySqlCommand"); }
        }

        public static Type mySqlConnectionType
        {
            get { return mySqlDataAssembly.GetType("MySql.Data.MySqlClient.MySqlConnection"); }
        }

        public static Assembly mySqlDataAssembly
        {
            get { return Assembly.Load("MySql.Data"); }
        }

    }
}