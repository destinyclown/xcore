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
using System.Collections.Generic;
using System.Text;

using System.ORM;
using System.Reflection;
using System.Serialization;
using System.Web;
using System.IO;
using System.Web;
using System.IO;

namespace System.Data {

    /// <summary>
    /// 数据库连接字符串内容的封装
    /// </summary>
    public class ConnectionString {
        private String _Name;
        public String Name { get{return _Name;} set{_Name=value;} }
        private String _StringContent;
        public String StringContent { get{return _StringContent;} set{_StringContent=value;} }
        private DatabaseType _DbType;
        public DatabaseType DbType { get{return _DbType;} set{_DbType=value;} }
    }

    /// <summary>
    /// ORM 的数据库配置
    /// </summary>
    public class DbConfig {
        internal static System.Web.Caching.CacheDependency dep = null;
        private static readonly ILog logger = LogManager.GetLogger(typeof(DbConfig));

        public DbConfig() {
            this.ConnectionStringTable = new Dictionary<String, object>();
            this.AssemblyList = new List<object>();
            this.DbType = new Dictionary<String, object>();
            this.Interceptor = new List<object>();
            this.IsCheckDatabase = true;
            this.ContextCache = true;
            this.Mapping = new List<object>();
        }
        public static void CreateDbConfig() {
            DbConfig config=new DbConfig();
            string[] ddlfiles = System.IO.Directory.GetFiles(PathHelper.Map("~/"), "*.dll");
            if (ddlfiles.Length == 0)
            {
                ddlfiles = System.IO.Directory.GetFiles(PathHelper.Map("~/bin"), "*.dll");
            }
            foreach (string dll in ddlfiles)
            {
                config.AssemblyList.Add(System.IO.Path.GetFileName(dll).Replace(".dll", ""));
            }
            //this.AssemblyList.Add("XCore");
            config.DbType.Add("default", "access");
            config.ConnectionStringTable.Add("default", DatabaseBuilder.ConnectionStringPrefix + DatabaseBuilder.BuildAccessDb4o());

            String cfgPath = getConfigPath();
            file.Write(cfgPath, JsonString.ConvertObject(config, true));
            loadMappingInfo(config);
            checkConnectionString(config);
            DbConfig.Instance = config;
            MappingClass.loadByReflection();
            MappingClass.checkMultiDB(MappingClass.Instance);
        }
        public static void ReLoad()
        {
            try
            {
                DbConfig.Instance = loadConfig(getConfigPath());
            }
            catch { }
            try
            {
                MappingClass.loadByReflection();
            }
            catch { }
        }

        /// <summary>
        /// 默认数据库名称(值为default)
        /// </summary>
        public static readonly String DefaultDbName = "default";

        /// <summary>
        /// 配置的缓存内容(单例模式缓存)
        /// </summary>
        public static DbConfig Instance = loadConfig( getConfigPath() );
		
		private Dictionary<String, object> _ConnectionStringTable; 
        /// <summary>
        /// 直接解析json的结果：多个数据库连接字符串(connectionString)的键值对
        /// </summary>
		public Dictionary<String, object> ConnectionStringTable { get{return _ConnectionStringTable;} set{_ConnectionStringTable=value;} }

        private Dictionary<String, ConnectionString> _connectionStringMap = new Dictionary<String, ConnectionString>();

        /// <summary>
        /// 多个数据库连接字符串对象的map，值是ConnectionString对象(包括Name/StringContent/DbType)
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, ConnectionString> GetConnectionStringMap() {
            return _connectionStringMap;
        }

        internal void SetConnectionStringMap( Dictionary<String, ConnectionString> cmap ) {
            _connectionStringMap = cmap;
        }
		
		private Dictionary<String, object> _DbType;
        /// <summary>
        /// 直接解析json的结果：数据库类型
        /// </summary>
		public Dictionary<String, object> DbType { get{return _DbType;} set{_DbType=value;} }
		
		private List<object> _AssemblyList;
        /// <summary>
        /// 直接解析json的结果：程序集列表
        /// </summary>
		public List<object> AssemblyList {  get{return _AssemblyList;} set{_AssemblyList=value;} }
		
		private Boolean _IsCheckDatabase;
        /// <summary>
        /// 是否检查数据库，如果检查，则会将尚未创建的数据表自动创建
        /// </summary>
		public Boolean IsCheckDatabase {  get{return _IsCheckDatabase;} set{_IsCheckDatabase=value;}}
		
		private String _TablePrefix;
        /// <summary>
        /// 数据表的前缀(默认没有前缀)
        /// </summary>
		public String TablePrefix {  get{return _TablePrefix;} set{_TablePrefix=value;} }
		
		private Boolean _ContextCache;
        /// <summary>
        /// 是否开启一级缓存，默认开启，并且建议开启
        /// </summary>
		public Boolean ContextCache { get{return _ContextCache;} set{_ContextCache=value;} }
		
		private Boolean _ApplicationCache;
        /// <summary>
        /// 是否开启二级缓存
        /// </summary>
		public Boolean ApplicationCache { get{return _ApplicationCache;} set{_ApplicationCache=value;} }
		
		private int _ApplicationCacheMinutes;
        /// <summary>
        /// 二级缓存的时间(分钟)
        /// </summary>
		public int ApplicationCacheMinutes { get{return _ApplicationCacheMinutes;} set{_ApplicationCacheMinutes=value;} }
		
		private String _ApplicationCacheManager;
        /// <summary>
        /// 二级缓存管理程序，请填写类型(type)的全名(full name)，比如 System.somens.myCache；
        /// 如果不填写，则使用默认的System.Web.Caching
        /// </summary>
		public String ApplicationCacheManager { get{return _ApplicationCacheManager;} set{_ApplicationCacheManager=value;} }
		
		private String _MetaDLL;
        /// <summary>
        /// ORM 的元数据文件名称，一般不需填写(建议不要填写)。如果为了提高网站启动时候的速度，可以填写。
        /// 系统会根据文件名自动生成元数据文件，可以避免以后网站启动过程中的反射，能略微提高启动速度；
        /// 文件名不包括路径(必须放在bin目录中)，比如 System.meta.dll
        /// </summary>
		public String MetaDLL { get{return _MetaDLL;} set{_MetaDLL=value;} }
		
		private List<object> _Mapping;
        /// <summary>
        /// 直接解析json的结果：数据表映射
        /// </summary>
		public List<object> Mapping { get{return _Mapping;} set{_Mapping=value;} }
		
		private List<object> _Interceptor;
        /// <summary>
        /// 拦截器列表
        /// </summary>
		public List<object> Interceptor { get{return _Interceptor;} set{_Interceptor=value;} }

        /// <summary>
        /// 反射优化模式，目前只实现了 CodeDom 方式
        /// </summary>
        [NotSerialize]
        internal OptimizeMode OptimizeMode {
            get { return OptimizeMode.CodeDom; }
            set { }
        }

        /// <summary>
        /// 获取元数据库文件的绝对路径
        /// </summary>
        /// <returns></returns>
        public String GetMetaDllAbsPath() {

            if (strUtil.IsNullOrEmpty( this.MetaDLL )) return "";

            String dllPath = this.MetaDLL;
            if (dllPath.ToLower().EndsWith( ".dll" ) == false)
                dllPath = dllPath + ".dll";

            dllPath = Path.Combine( PathTool.GetBinDirectory(), dllPath );
            

            return dllPath;
        }

        private Dictionary<String, MappingInfo> _mappings = new Dictionary<String, MappingInfo>();

        private void addMapping( MappingInfo mi ) {
            _mappings.Add( mi.TypeName, mi );
        }

        /// <summary>
        /// 获取映射信息
        /// </summary>
        /// <returns></returns>
        internal Dictionary<String, MappingInfo> GetMappingInfo() {
            return _mappings;
        }

        //----------------------------------------------------------------------

        public static DbConfig loadConfig(String cfgPath)
        {
            try
            {
                if (!file.Exists(cfgPath))
                {
                    CreateDbConfig();
                }
                String str = file.Read(cfgPath, true);
                DbConfig dbc = JSON.ToObject<DbConfig>(str);
                Dictionary<string, object>.Enumerator en = dbc.ConnectionStringTable.GetEnumerator();
                List<System.Data.KeyValueData> kvlist = new List<KeyValueData>();
                while (en.MoveNext())
                {
                    string connstr = en.Current.Value.ToString();
                    if (connstr.IndexOf(':') <= 0 && dbc.DbType[en.Current.Key].ToString() == "access")
                    {
                        connstr = connstr.Replace(DatabaseBuilder.ConnectionStringPrefix, DatabaseBuilder.ConnectionStringPrefix + PathHelper.Map("/")).Replace('/', '\\');
                        kvlist.Add(new System.Data.KeyValueData(en.Current.Key, connstr.Replace("\\\\", "\\")));
                    }
                }
                foreach (System.Data.KeyValueData kv in kvlist)
                {
                    dbc.ConnectionStringTable[kv.KeyName] = kv.KeyValue;
                }

                loadMappingInfo(dbc);
                checkConnectionString(dbc);

                return dbc;
            }
            catch{}
            return null;
        }

        private static void loadMappingInfo( DbConfig dbc ) {
            if (dbc.Mapping.Count == 0) return;
            foreach (Dictionary<String, object> dic in dbc.Mapping) {

                MappingInfo mi = new MappingInfo();

                if (dic.ContainsKey( "name" )) mi.TypeName = dic["name"].ToString();
                if (dic.ContainsKey( "database" )) mi.Database = dic["database"].ToString();
                if (dic.ContainsKey( "table" )) mi.Table = dic["table"].ToString();

                dbc.addMapping( mi );
            }
        }

        public static String getConfigPath()
        {
            string _temp = PathHelper.Map(strUtil.Join(cfgHelper.ConfigRoot, "orm.config"));
            //try
            //{
            //    if (SystemInfo.IsWeb && SystemInfo.OrmConfigPath == null)
            //    {
            //        if (file.Exists(_temp))
            //        {
            //            SystemInfo.OrmConfigPath = _temp;
            //            dep = new System.Web.Caching.CacheDependency(SystemInfo.OrmConfigPath);
            //            //if (System.Data.DbConfig.dep != null && System.Data.DbConfig.dep.HasChanged)
            //            //{
            //            //    cfgHelper.SetAppSettings("LastStartTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //            //    //File.SetLastWriteTime(SystemInfo.OrmConfigPath, DateTime.Now);
            //            //}
            //        }
            //    }
            //}
            //catch { }
            return _temp;
        }

        private static void checkConnectionString( DbConfig result ) {

            logger.Info( "检查数据库连接设置..." );

            if (result.ConnectionStringTable == null) return;

            Dictionary<String, ConnectionString> connStringMap = new Dictionary<String, ConnectionString>();

            Dictionary<String, String> newString = new Dictionary<string, string>();
            foreach (KeyValuePair<String, object> kv in result.ConnectionStringTable) {

                String connectionString = kv.Value.ToString();
                DatabaseType dbtype = getDbType( kv.Key, connectionString, result );

				ConnectionString objConnString = new ConnectionString ();
				objConnString.Name = kv.Key;
				objConnString.StringContent = connectionString;
				objConnString.DbType = dbtype;

                connStringMap.Add( kv.Key, objConnString );

                logger.Info( "链接字符串:" + connectionString );

                IDatabaseDialect dialect = DataFactory.GetDialect( dbtype );

                if ((dbtype == DatabaseType.Access)) {
                    String connectionItem = dialect.GetConnectionItem( connectionString, ConnectionItemType.Database );
                    logger.Info( "database path original:" + connectionItem );

                    if (IsRelativePath( connectionItem )) {
                        connectionItem = strUtil.Join(SystemInfo.ApplicationPath, connectionItem);
                        //connectionItem = PathHelper.Map( strUtil.Join( SystemInfo.ApplicationPath, connectionItem ) );
                    }
                    logger.Info("database path now:" + connectionItem);
                    String newConnString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", connectionItem);
                    newString.Add(kv.Key, newConnString);
                    if (!file.Exists(connectionItem))
                    {
                        try
                        {
                            object catobj = null;
                            try
                            {
                                catobj = rft.GetInstance("Interop.ADOX", "ADOX.CatalogClass");
                            }
                            catch
                            {
                                logger.Error("Interop.ADOX.dll文件不存在，请检查bin目录！");
                            }
                            object connstr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + connectionItem + ";" +
                                             "Jet OLEDB:Engine Type=5";
                            rft.CallMethod(catobj, "Create", new[] {connstr});
                            //ADOX.CatalogClass cat = new ADOX.CatalogClass();
                            //cat.Create("Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + connectionItem + ";" + "Jet OLEDB:Engine Type=5");
                            //Console.WriteLine("Database Created Successfully");
                            //logger.Info("Access数据库已创建:" + connectionItem);
                            //cat = null;
                        }
                        catch
                        {
                            logger.Info("Access数据库创建失败:" + connectionItem);
                        }
                    }
                }
            }

            foreach (KeyValuePair<String, String> kv in newString) {
                result.ConnectionStringTable[kv.Key] = kv.Value;
                connStringMap[kv.Key].StringContent = kv.Value;
            }

            result.SetConnectionStringMap( connStringMap );
        }

        private static bool IsRelativePath( string connectionItem ) {
            return connectionItem.IndexOf( ":" ) < 0;
        }

        private static DatabaseType getDbType(String dbname, String connectionString, DbConfig result)
        {

            foreach (KeyValuePair<String, Object> kv in result.DbType)
            {
                if (kv.Key == dbname) return DbTypeChecker.GetFromString(kv.Value.ToString());
            }

            DatabaseType dbtype = DbTypeChecker.GetDatabaseType(connectionString);
            return dbtype;
        }

        //----------------------------------------------------------------------

        /// <summary>
        /// 根据命名，获取数据库连接字符串
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static String GetConnectionString(String db)
        {
            if (DbConfig.Instance.ConnectionStringTable.ContainsKey(db) == false)
                throw new Exception(lang.get("dbNotExist") + ": " + db);
            return (String)DbConfig.Instance.ConnectionStringTable[db];
        }


        internal static void SaveConnectionString( String connectionString ) {
            //if (connectionString.IndexOf(':') <= 0 && DbConfig.Instance.DbType[DefaultDbName].ToString() == "access")
            //{
            //    connectionString = connectionString.Replace(DatabaseBuilder.ConnectionStringPrefix, DatabaseBuilder.ConnectionStringPrefix + PathHelper.Map("/"));
            //}

            if (DbConfig.Instance.ConnectionStringTable == null)
                DbConfig.Instance.ConnectionStringTable = new Dictionary<String, object>();

            DbConfig.Instance.ConnectionStringTable[DefaultDbName] = connectionString;
            //String str = JsonString.ConvertObject(DbConfig.Instance, true);
            //String str = JsonString.ConvertObject(DbConfig.Instance, true).Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace("\b", "");

            //file.Write(getConfigPath(), str);
        }

    }

}
