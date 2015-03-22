/*
 * Copyright 2012 www.xcenter.cn
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;

using System.Web;
using System.ORM;
using System.ORM.Caching;

namespace System.Data {

    /// <summary>
    /// 数据库上下文，主要用于获取数据库连接
    /// </summary>
    public partial class DbContext {


        /// <summary>
        /// 关闭数据库连接。因为ORM支持多个数据库，所以所有可能的数据库连接都会一起关闭。
        /// </summary>
        public static void closeConnectionAll() {

            ContextCache.Clear();

            Dictionary<String, IDbConnection> dic = getConnectionAll();
            foreach (KeyValuePair<String, IDbConnection> kv in dic) {
                IDbConnection connection = kv.Value;
                if (connection == null || connection.State != ConnectionState.Open) continue;
                connection.Close();
                connection.Dispose();
                OrmHelper.clostCount++;
                LogManager.GetLogger("Class:System.Data.DbContext Method:closeConnectionAll").Info("数据库连接关闭成功【" + OrmHelper.clostCount + "】");
            }
            freeItem( _connectionKey );
        }

        //------------------------------------------------------------------------------

        /// <summary>
        /// 获取所有的数据库连接
        /// </summary>
        /// <returns></returns>
        public static Dictionary<String, IDbConnection> getConnectionAll() {
            Dictionary<String, IDbConnection> dic;
            dic = CurrentRequest.getItem( _connectionKey ) as Dictionary<String, IDbConnection>;
            if (dic == null) {
                dic = new Dictionary<String, IDbConnection>();
                CurrentRequest.setItem( _connectionKey, dic );
            }
            return dic;
        }

        private static Dictionary<String, IDbTransaction> getTransactionAll() {

            Dictionary<String, IDbTransaction> dic;
            dic = CurrentRequest.getItem( _transactionKey ) as Dictionary<String, IDbTransaction>;
            if (dic == null) {
                dic = new Dictionary<String, IDbTransaction>();
                CurrentRequest.setItem( _transactionKey, dic );
            }
            return dic;
        }

        private static void setConnection(String key, IDbConnection cn)
        {
            Dictionary<String, IDbConnection> dic;
            dic = CurrentRequest.getItem(_connectionKey) as Dictionary<String, IDbConnection>;
            if (dic == null)
            {
                dic = new Dictionary<String, IDbConnection>();
            }
            if (dic.ContainsKey(key))
            {
                dic[key] = cn;
            }
            else
            {
                dic.Add(key, cn);
            }
            CurrentRequest.setItem(_connectionKey, dic);
        }

        private static void setTransaction(String key, IDbTransaction trans)
        {
            Dictionary<String, IDbTransaction> dic;
            dic = CurrentRequest.getItem(_transactionKey) as Dictionary<String, IDbTransaction>;
            if (dic == null)
            {
                dic = new Dictionary<String, IDbTransaction>();
            }
            if (dic.ContainsKey(key))
            {
                dic[key] = trans;
            }
            else
            {
                dic.Add(key, trans);
            }
            CurrentRequest.setItem(_transactionKey, dic);
        }

        //------------------------------------------------------------------------------

        public static void beginAndMarkTransactionAll() {
            CurrentRequest.setItem( _beginTransactionAll, true ); // 如果当前没有打开的数据库连接，则打上标记，等真正打开的时候启用事务
            beginTransactionAll();
        }


        internal static bool shouldTransaction() {
            Object trans = CurrentRequest.getItem( _beginTransactionAll );
            if (trans == null) return false;
            return (Boolean)trans;
        }

        /// <summary>
        /// 针对所有数据库连接，开启数据库事务
        /// </summary>
        public static void beginTransactionAll() {
            Dictionary<String, IDbConnection> list = getConnectionAll();
            foreach (KeyValuePair<String, IDbConnection> kv in list)
            {
                if (kv.Value != null && kv.Value.State == ConnectionState.Closed)
                {
                    kv.Value.Open();
                    OrmHelper.initCount++;
                    LogManager.GetLogger("Class:System.Data.DbContext Method:beginTransactionAll").Info("数据库连接已开启【" + OrmHelper.initCount + "】");
                }
                IDbTransaction trans = kv.Value.BeginTransaction();
                setTransaction( kv.Key, trans );
            }
        }

        public static void setTransaction( IDbCommand cmd ) {
            Dictionary<String, IDbTransaction> transTable = getTransactionAll();
            foreach (KeyValuePair<String, IDbTransaction> kv in transTable) {
                IDbTransaction trans = kv.Value;

                if (cmd.Connection == trans.Connection) {
                    cmd.Transaction = trans;
                    return;
                }
            }
        }

        /// <summary>
        /// 获取数据库连接，返回的连接已经打开(open)；在 mvc 框架中不用关闭，框架会自动关闭连接。
        /// 之所以要传入 Type，因为 ORM 支持多个数据库，不同的类型有可能映射到不同的数据库。
        /// </summary>
        /// <param name="t">实体的类型</param>
        /// <returns></returns>
        public static IDbConnection getConnection(Type t)
        {
            return getConnection(Entity.GetInfo(t));
        }

        /// <summary>
        /// 获取数据库连接，返回的连接已经打开(open)；在 mvc 框架中不用关闭，框架会自动关闭连接。
        /// 之所以要传入 EntityInfo，因为 ORM 支持多个数据库，不同的类型有可能映射到不同的数据库。
        /// </summary>
        /// <param name="et"></param>
        /// <returns></returns>
        public static IDbConnection getConnection(EntityInfo et)
        {

            String db = et.Database;
            String connectionString = DbConfig.GetConnectionString(db);

            IDbConnection connection;
            Dictionary<String, IDbConnection> connections = getConnectionAll();
            connections.TryGetValue(db, out connection);
            if (connection == null)
            {
                try
                {
                    connection = DataFactory.GetConnection(connectionString, et.DbType);
                }
                catch(Exception ex)
                {
                    LogManager.GetLogger().Error("数据库连接字符串错误：" + connectionString);
                    throw ex;
                }
                connection.Open();
                OrmHelper.initCount++;
                LogManager.GetLogger("Class:System.Data.DbContext Method:getConnection").Info("数据库连接已开启【" + OrmHelper.initCount + "】");
                setConnection(db, connection);
                if (shouldTransaction())
                {
                    IDbTransaction trans = connection.BeginTransaction();
                    setTransaction(db, trans);
                }
                return connection;
            }
            if (connection.State == ConnectionState.Closed)
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                OrmHelper.initCount++;
                LogManager.GetLogger("Class:System.Data.DbContext Method:getConnection").Info("数据库连接已开启【" + OrmHelper.initCount + "】");
            }
            return connection;
        }


        /// <summary>
        /// 获取数据库连接，返回的连接已经打开(open)；在 mvc 框架中不用关闭，框架会自动关闭连接。
        /// 之所以要传入 EntityInfo，因为 ORM 支持多个数据库，不同的类型有可能映射到不同的数据库。
        /// </summary>
        /// <param name="et"></param>
        /// <returns></returns>
        public static IDbConnection getReadOnlyConnection()
        {
            String dbname = "readonly";
            DatabaseType dbType = DatabaseType.Access;
            foreach (KeyValuePair<String, Object> kv in DbConfig.Instance.DbType)
            {
                dbname = kv.Key;
                dbType = DbTypeChecker.GetFromString(kv.Value.ToString());
                if (kv.Key == "readonly")
                {
                    break;
                }
            }
            String connectionString = DbConfig.GetConnectionString(dbname);
            IDbConnection connection;
            Dictionary<String, IDbConnection> connections = getConnectionAll();
            connections.TryGetValue(dbname, out connection);
            if (connection == null)
            {
                try
                {
                    connection = DataFactory.GetConnection(connectionString, dbType);
                }
                catch (Exception ex)
                {
                    LogManager.GetLogger().Error("数据库连接字符串错误：" + connectionString);
                    throw ex;
                }
                connection.Open();
                OrmHelper.initCount++;
                LogManager.GetLogger("Class:System.Data.DbContext Method:getConnection").Info("数据库连接已开启【" + OrmHelper.initCount + "】");
                setConnection(dbname, connection);
                if (shouldTransaction())
                {
                    IDbTransaction trans = connection.BeginTransaction();
                    setTransaction(dbname, trans);
                }
                return connection;
            }
            if (connection.State == ConnectionState.Closed)
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                OrmHelper.initCount++;
                LogManager.GetLogger("Class:System.Data.DbContext Method:getConnection").Info("数据库连接已开启【" + OrmHelper.initCount + "】");
            }
            return connection;
        }

        private static void clearTransactionAll() {
            Dictionary<String, IDbTransaction> dic = CurrentRequest.getItem( _transactionKey ) as Dictionary<String, IDbTransaction>;
            if (dic == null) return;
            dic.Clear();
            CurrentRequest.setItem( _transactionKey, dic );
        }

        /// <summary>
        /// 提交全部的数据库事务
        /// </summary>
        public static void commitAll() {
            Dictionary<String, IDbTransaction> transTable = getTransactionAll();
            foreach (KeyValuePair<String, IDbTransaction> kv in transTable) {
                IDbTransaction trans = kv.Value;
                if (trans != null && trans.Connection != null ) trans.Commit();
            }
            clearTransactionAll();
            closeConnectionAll();
        }

        /// <summary>
        /// 回滚所有数据库事务
        /// </summary>
        public static void rollbackAll() {
            Dictionary<String, IDbTransaction> transTable = getTransactionAll();
            foreach (KeyValuePair<String, IDbTransaction> kv in transTable) {
                IDbTransaction trans = kv.Value;
                if (trans != null && trans.Connection != null) trans.Rollback();
            }
            clearTransactionAll();
            closeConnectionAll();
        }

        //------------------------------------------------------------------------------

        private static void freeItem( String key ) {
            if (SystemInfo.IsWeb)
            {
                if (HttpContext.Current.Items.Contains(key))
                {
                    HttpContext.Current.Items.Remove(key);
                }
            }
            else
            {
                Thread.FreeNamedDataSlot( key );
            }
        }


        internal static IDictionary getContextCache() {
            Object dic = null;
            try
            {
                dic = CurrentRequest.getItem(_contextCacheKey);
            }
            catch { }
            if (dic == null) {
                dic = new Hashtable();
                CurrentRequest.setItem( _contextCacheKey, dic );
            }
            return dic as IDictionary;
        }

        /// <summary>
        /// 获取存储在上下文中的 sql 执行次数
        /// </summary>
        /// <returns></returns>
        public static int getSqlCount() {

            Object count = CurrentRequest.getItem( "sqlcount" );
            if (count == null) return 0;
            return cvt.ToInt( count );
        }

        private static readonly String _beginTransactionAll = "__beginTransactionAll";
        private static readonly String _connectionKey = "__xcenterConnection";
        private static readonly String _transactionKey = "__xcenterDbTransaction";
        private static readonly String _contextCacheKey = "__contextCacheDictionary";
    }
}

