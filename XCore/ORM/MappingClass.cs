//------------------------------------------------------------------------------
//	文件名称：System\ORM\MappingClass.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.DI;
using System.Web;
using System.IO;
using System.Web.Hosting;
namespace System.ORM {
    /// <summary>
    /// 实体类和数据表关联映射的信息
    /// </summary>
    [Serializable]
    public partial class MappingClass {
        private static readonly ILog logger = LogManager.GetLogger( typeof( MappingClass ) );
        private IDictionary _assemblyList = new Hashtable();
        private IDictionary _classList = new Hashtable();
        private IDictionary _typeList = new Hashtable();
        private IDictionary _factoryList;
        private List<IInterceptor> _interceptorList = new List<IInterceptor>();
        public static MappingClass Instance = loadInstance();
        private List<String> _tableList = new List<String>();
        /// <summary>
        /// ORM需要加载的所有程序集
        /// </summary>
        public IDictionary AssemblyList {
            get { return _assemblyList; }
            set { _assemblyList = value; }
        }
        /// <summary>
        /// 所有需要持久化的实体的 EntityInfo(每个EntityInfo包括类型、映射的表名等信息)
        /// </summary>
        public IDictionary ClassList {
            get { return _classList; }
            set { _classList = value; }
        }
        /// <summary>
        /// 所有需要持久化的实体的类型(type)
        /// </summary>
        public IDictionary TypeList {
            get { return _typeList; }
            set { _typeList = value; }
        }
        /// <summary>
        /// 所有需要持久化的实体的创建工厂
        /// </summary>
        public IDictionary FactoryList {
            get { return _factoryList; }
            set { _factoryList = value; }
        }
        /// <summary>
        /// 所有拦截器
        /// </summary>
        public List<IInterceptor> InterceptorList {
            get { return _interceptorList; }
            set { _interceptorList = value; }
        }
        /// <summary>
        /// 所有表名
        /// </summary>
        public List<String> TableList {
            get { return _tableList; }
            set { _tableList = value; }
        }
        private MappingClass() {}
        private static String getColumnName( PropertyInfo property, IDictionary mapClassList ) {
            ColumnAttribute attribute = ReflectionUtil.GetAttribute( property, typeof( ColumnAttribute ) ) as ColumnAttribute;
            if ((attribute != null) && strUtil.HasText( attribute.Name )) {
                return attribute.Name;
            }
            if (mapClassList.Contains( property.PropertyType.FullName )) {
                return (property.Name + "Id");
            }
            if (property.PropertyType.IsInterface) {
                return String.Format( "{0}Id", property.PropertyType.Name.TrimStart( new char[] { 'I' } ) );
            }
            if (property.PropertyType.Name == "IList") {
                throw new Exception( "property data type can not be IList" );
            }
            return property.Name;
        }
        internal Boolean ContainsTable( String tableName ) {
            for (int i = 0; i < TableList.Count; i++) {
                if (strUtil.EqualsIgnoreCase( TableList[i], tableName )) {
                    return true;
                }
            }
            return false;
        }
        private static MappingClass loadInstance()
        {
            logger.Info("[Orm]领域类分析开始...");
            MappingClass result;
            if (strUtil.HasText(DbConfig.Instance.MetaDLL))
            {
                result = loadByCacheDLL();
            }
            else
            {
                result = loadByReflection();
            }
            logger.Info("[Orm]领域类分析完成...");
            return result;
        }
        public static MappingClass loadByReflection()
        {
            logger.Info("通过反射机制加载程序集...");
            IList asmList = DbConfig.Instance.AssemblyList;
            MappingClass map = new MappingClass();
            try
            {
                for (int i = 0; i < asmList.Count; i++)
                {
                    Assembly assembly = ObjectContext.LoadAssembly(asmList[i].ToString());
                    logger.Info("程序集: " + assembly.FullName);
                    map.AssemblyList[asmList[i].ToString()] = assembly;
                    Type[] typeArray = ObjectContext.FindTypes(asmList[i].ToString());
                    foreach (Type type in typeArray)
                    {
                        ResolveOneType(map, type);
                    }
                }
                logger.Info("FinishPropertyInfo");
                FinishPropertyInfo(map.ClassList);
                logger.Info("cacheInterceptor");
                cacheInterceptor(map);
                logger.Info("AccessorUtil.Init");
                MetaList list = new MetaList(map.AssemblyList, map.ClassList);
                map.FactoryList = AccessorUtil.Init(list);
                try
                {
                    checkMultiDB(map);
                }
                catch (Exception ex)
                {
                    logger.Info("初始化数据库出现错误：" + ex.Message);
                }
            }
            catch (Exception ex)
            {
                logger.Info("初始化Orm对象出现错误：" + ex.Message);
            }
            return map;
        }
        public static MappingClass loadByCacheDLL()
        {
            String dllPath = DbConfig.Instance.GetMetaDllAbsPath();
            MappingClass map = null;
            try
            {
                map = EasyDB.LoadFromFile(dllPath) as MappingClass;
            }
            catch (Exception exception)
            {
                logger.Error("[MappingClass.loadInstance=>db.LoadFromFile( DbConfig.Instance.MetaDLL=" + DbConfig.Instance.MetaDLL + ")]:" + exception.Message);
            }
            if (map == null)
            {
                map = loadByReflection();
                EasyDB.SaveToFile(map, dllPath);
            }
            else
                logger.Info("load meta from cache");
            checkMultiDB(map);
            return map;
        }
        private static void ResolveOneType(MappingClass map, Type t)
        {
            map.TypeList.Add(t.FullName, t);
            //if (!t.IsAbstract && ( t.IsSubclassOf( typeof( ObjectBase ) ) || t is IEntity )) {
            //    map.ClassList[t.FullName] = EntityInfo.GetByType( t );
            //    logger.Info( "Loading Type : " + t.FullName );
            //}
            // 1029
            logger.Info("加载类 : " + t.FullName);
            if (rft.IsInterface(t, typeof(IEntity)) && !OrmHelper.IsEntityBase(t))
            {
                map.ClassList.Add(t.FullName, EntityInfo.GetByType(t));
            }
        }
        private static IDictionary FinishPropertyInfo(IDictionary mapClassList)
        {
            foreach (DictionaryEntry entry in mapClassList)
            {
                EntityInfo info = entry.Value as EntityInfo;
                foreach (EntityPropertyInfo ep in info.SavedPropertyList)
                {
                    //if (mapClassList.Contains( ep.Type.FullName )) {
                    //    ep.EntityInfo = mapClassList[ep.Type.FullName] as EntityInfo;
                    //}
                    if (mapClassList.Contains(ep.Type.FullName) && ep.SaveToDB)
                    {
                        ep.EntityInfo = mapClassList[ep.Type.FullName] as EntityInfo;
                        ep.IsEntity = true;
                        info.EntityPropertyList.Add(ep);
                    }
                    ep.ColumnName = getColumnName(ep.Property, mapClassList);
                    //if (!(!ep.SaveToDB || ep.IsList)) {
                    //    info.ColumnList = info.ColumnList + ep.ColumnName + ",";
                    //}
                    if (isSavedProperty(ep)) info.ColumnList = info.ColumnList + ep.ColumnName + ",";
                }
                foreach (EntityPropertyInfo ep in info.PropertyListAll)
                {
                    info.AddPropertyToHashtable(ep);
                }
                info.ColumnList = info.ColumnList.Trim().TrimEnd(new char[] { ',' });
                //if (!((info.Type.BaseType == typeof( ObjectBase )) || info.Type.BaseType.IsAbstract)) {
                //    EntityInfo info4 = mapClassList[info.Type.BaseType.FullName] as EntityInfo;
                //    info4.ChildEntityList.Add( info );
                //}
                // 找到父对象，然后给父对象的ChildEntityList加上当前对象
                if (updateParentInfo(info))
                {
                    EntityInfo parentEi = mapClassList[info.Type.BaseType.FullName] as EntityInfo;
                    parentEi.ChildEntityList.Add(info);
                }
            }
            return mapClassList;
        }
        //1029
        private static Boolean updateParentInfo(EntityInfo info)
        {
            if (info.Type.BaseType == typeof(Object)) return false;
            if (OrmHelper.IsEntityBase(info.Type.BaseType)) return false;
            if (info.Type.BaseType.IsAbstract) return false;
            return true;
        }
        //1029
        private static Boolean isSavedProperty(EntityPropertyInfo ep)
        {
            if (ep.SaveToDB) return true;
            return false;
        }
        private static void cacheInterceptor(MappingClass map)
        {
            List<object> interceptor = DbConfig.Instance.Interceptor;
            for (int i = 0; i < interceptor.Count; i++)
            {
                Dictionary<String, object> info = interceptor[i] as Dictionary<String, object>;
                String asmName = info["AssemblyName"].ToString();
                String typeName = info["TypeFullName"].ToString();
                IInterceptor obj = rft.GetInstance(asmName, typeName) as IInterceptor;
                if (obj == null)
                {
                    throw new Exception("load ORM interceptor error( Assembly:" + asmName + ", Type:" + typeName + ")");
                }
                map.InterceptorList.Add(obj);
            }
        }
        public static void checkMultiDB(MappingClass map)
        {
            if (DbConfig.Instance.IsCheckDatabase == false)
            {
                logger.Info("跳过检查数据库...");
                return;
            }
            if (map.TableList == null) map.TableList = new List<String>();
            if (DbConfig.Instance.ConnectionStringTable == null || DbConfig.Instance.ConnectionStringTable.Count == 0)
            {
                createDB(map);
                return;
            }
            logger.Info("开始检查数据库...");
            foreach (KeyValuePair<String, ConnectionString> kv in DbConfig.Instance.GetConnectionStringMap())
            {
                String connectionString = kv.Value.StringContent;
                if (strUtil.IsNullOrEmpty(connectionString))
                    throw new NotImplementedException(lang.get("exConnectionString") + ":" + kv.Key);
                DatabaseType dbtype = kv.Value.DbType;
                IDatabaseChecker databaseChecker = DataFactory.GetDatabaseChecker(dbtype);
                databaseChecker.ConnectionString = connectionString;
                databaseChecker.DatabaseType = dbtype;
                logger.Info("CheckDatabase");
                databaseChecker.CheckDatabase();
                logger.Info("CheckTable");
                databaseChecker.CheckTable(map, kv.Key);
                logger.Info("GetTables");
                map.TableList.AddRange(databaseChecker.GetTables());
            }
            logger.Info("end check database...");
        }
        private static void createDB(MappingClass map)
        {
            logger.Info("begin create database ...");
            DatabaseType dbtype = DatabaseType.Access;
            IDatabaseChecker databaseChecker = DataFactory.GetDatabaseChecker(dbtype);
            databaseChecker.DatabaseType = dbtype;
            logger.Info("CheckDatabase");
            databaseChecker.CheckDatabase();
            logger.Info("CheckTable");
            databaseChecker.CheckTable(map, DbConfig.DefaultDbName);
            logger.Info("GetTables");
            map.TableList = databaseChecker.GetTables();
        }
    }
}