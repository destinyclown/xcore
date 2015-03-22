using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using System.Data;

namespace System.ORM {

    /// <summary>
    /// 实体类的元数据信息
    /// </summary>
    [Serializable]
    public class EntityInfo {
        private Assembly _assembly;

        private List<EntityInfo> _childEntityList = new List<EntityInfo>();
        private List<EntityPropertyInfo> _entityPropertyList = new List<EntityPropertyInfo>();
        private List<EntityPropertyInfo> _savedPropertyList = new List<EntityPropertyInfo>();
        private List<EntityPropertyInfo> _propertyListAll = new List<EntityPropertyInfo>();

        private readonly Hashtable _propertyHashTable = new Hashtable();
        private readonly Hashtable _propertyHashTableByColumn = new Hashtable();

        private String _columnList;

        private String _tableName;
        private Type _type;
        private String _fullName;
        private String _label;
        private String _name;
        private EntityPropertyInfo _relationProperty;

        private DatabaseType _dbtype = DatabaseType.Other;
        private IDatabaseDialect _dialect;
		
		private String _database;
        /// <summary>
        /// 
        /// </summary>
		public String Database { get{return _database;} set{_database=value;} }

        /// <summary>
        /// 
        /// </summary>
        public Assembly Assembly {
            get { return _assembly; }
            set { _assembly = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<EntityInfo> ChildEntityList {
            get { return _childEntityList; }
            set { _childEntityList = value; }
        }

        /// <summary>
        /// 对应的数据表中的所有列的名称
        /// </summary>
        public String ColumnList {
            get { return _columnList; }
            set { _columnList = value; }
        }

        /// <summary>
        /// 只是实体性质的属性的列表，比如 BlogPost 的某个属性是 BlogCategory 这样的实体类型
        /// </summary>
        public List<EntityPropertyInfo> EntityPropertyList {
            get { return _entityPropertyList; }
            set { _entityPropertyList = value; }
        }

        /// <summary>
        /// 所有属性的列表(属性已经封装成EntityPropertyInfo)
        /// </summary>
        public List<EntityPropertyInfo> PropertyListAll {
            get { return _propertyListAll; }
            set { _propertyListAll = value; }
        }

        /// <summary>
        /// 所有需要保存的属性
        /// </summary>
        public List<EntityPropertyInfo> SavedPropertyList {
            get { return _savedPropertyList; }
            set { _savedPropertyList = value; }
        }

        /// <summary>
        /// 实体类全名，比如 System.apps.BlogApp
        /// </summary>
        public String FullName {
            get { return _fullName; }
            set { _fullName = value; }
        }

        /// <summary>
        /// 实体类在表单中的名称，用于表单代码自动生成
        /// </summary>
        public String Label {
            get { return _label; }
            set { _label = value; }
        }

        /// <summary>
        /// 实体类名称，等同于type.Name，比如BlogApp
        /// </summary>
        public String Name {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// 当前实体类的父类，如果它是继承于某个基类的话
        /// </summary>
        public EntityInfo Parent {
            get {
                if (Type.BaseType != null && (Type.BaseType.IsAbstract || OrmHelper.IsEntityBase( Type.BaseType ))) {// 1029
                    return null;
                }
                //return (MappingClass.Instance.ClassList[Type.BaseType.FullName] as EntityInfo); //Mobirds
                return null;
            }
        }

        /// <summary>
        /// 实体类对应的数据表名称
        /// </summary>
        public String TableName {
            get { return _tableName; }
            set { _tableName = value; }
        }

        /// <summary>
        /// 实体类对应的Type
        /// </summary>
        public Type Type {
            get { return _type; }
            set { _type = value; }
        }



        /// <summary>
        /// 实体类的 dialect
        /// </summary>
        public IDatabaseDialect Dialect
        {
            get { return _dialect ?? (_dialect = DataFactory.GetDialect(DbType)); }
        }
        private DatabaseType GetDbType()
        {
            DatabaseType dbtype = GetDbTypeFromConfig();
            if (dbtype != DatabaseType.Other) return dbtype;
            return DbTypeChecker.GetDatabaseType(DbConfig.GetConnectionString(Database));
        }
        private DatabaseType GetDbTypeFromConfig()
        {
            if (DbConfig.Instance.DbType.ContainsKey(Database))
            {
                return DbConfig.Instance.GetConnectionStringMap()[Database].DbType;
            }
            return DatabaseType.Other;
        }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseType DbType
        {
            get
            {
                if (_dbtype == DatabaseType.Other)
                {
                    _dbtype = GetDbType();
                }
                return _dbtype;
            }
        }
        private static String AddPrefixToTableName(String tableName)
        {
            if (strUtil.HasText(DbConfig.Instance.TablePrefix))
            {
                tableName = tableName.Replace("[", "").Replace("]", "");
                tableName = DbConfig.Instance.TablePrefix + tableName;
            }
            return tableName;
        }
        private static void CheckCustomMapping(EntityInfo info)
        {
            Dictionary<String, MappingInfo> map = DbConfig.Instance.GetMappingInfo();
            if (map.ContainsKey(info.Type.FullName))
            {
                MappingInfo mi = map[info.Type.FullName];
                if (strUtil.HasText(mi.Table)) info.TableName = mi.Table;
                if (strUtil.HasText(mi.Database)) info.Database = mi.Database;
            }
        }
        private static String GetDatabase(Type t)
        {
            DatabaseAttribute attribute = ReflectionUtil.GetAttribute(t, typeof(DatabaseAttribute)) as DatabaseAttribute;
            if (attribute == null)
            {
                return DbConfig.DefaultDbName;
            }
            return attribute.Database;
        }
        /// <summary>
        /// 根据类型Type，初始化EntityInfo；注意：因为不是从缓存中取，所以速度较慢
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        internal static EntityInfo GetByType(Type t)
        {

            EntityInfo info = new EntityInfo();

            info.Type = t;
            info.Name = t.Name;
            info.FullName = t.FullName;

            info.TableName = AddPrefixToTableName(GetTableName(t));
            info.Database = GetDatabase(t);

            CheckCustomMapping(info);

            info.Label = GetTypeLabel(t);

            IList propertyList = ReflectionUtil.GetPropertyList(t);
            for (int i = 0; i < propertyList.Count; i++)
            {
                PropertyInfo property = propertyList[i] as PropertyInfo;
                EntityPropertyInfo ep = EntityPropertyInfo.Get(property);
                ep.ParentEntityInfo = info;

                if (!(!ep.SaveToDB || ep.IsList))
                {
                    info.SavedPropertyList.Add(ep);
                }
                info.PropertyListAll.Add(ep);
            }

            if (info.SavedPropertyList.Count == 1)
            {
                throw new Exception("class's properties have not been setted '[save]' attribute.");
            }

            return info;
        }

        internal void AddPropertyToHashtable( EntityPropertyInfo p ) {
            _propertyHashTable[p.Name] = p;
            if (strUtil.HasText( p.ColumnName )) {
                _propertyHashTableByColumn[p.ColumnName.ToLower()] = p;
            }
        }

        internal EntityPropertyInfo FindRelationProperty( Type t ) {
            if (_relationProperty == null) {
                for (int i = 0; i < EntityPropertyList.Count; i++) {
                    EntityPropertyInfo info = EntityPropertyList[i];
                    if (info.Type != t) {
                        _relationProperty = info;
                    }
                }
            }
            return _relationProperty;
        }


        /// <summary>
        /// 获取某个属性在数据库中对应的数据列名称
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public String GetColumnName( String propertyName ) {
            for (var i = 0; i < _savedPropertyList.Count; i++) {
                var ep = _savedPropertyList[i];
                if (ep.Name == propertyName) {
                    return ep.ColumnName;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取某个属性的元数据信息(已封装成EntityPropertyInfo)
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public EntityPropertyInfo GetProperty( String propertyName ) {
            return (_propertyHashTable[propertyName] as EntityPropertyInfo);
        }

        /// <summary>
        /// 根据column名称，获取获取某个属性的元数据信息(已封装成EntityPropertyInfo)
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public EntityPropertyInfo GetPropertyByColumn( String columnName ) {
            //if (columnName == "OrderRemarksNum")
            //{
            //    columnName = "OrderRemarksNum";
            //}
            return (_propertyHashTable[columnName] as EntityPropertyInfo);
        }

        /// <summary>
        /// 根据属性的类型，比如BlogCategory，获取符合要求的第一个属性的名称
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public String GetPropertyName( Type propertyType ) {
            for (int i = 0; i < _savedPropertyList.Count; i++) {
                var ep = _savedPropertyList[i];
                if (ep.Type.FullName == propertyType.FullName) {
                    return ep.Name;
                }
            }
            return null;
        }

        internal String GetRelationPropertyName( Type propertyType ) {
            String name = null;
            for (int i = 0; i < _savedPropertyList.Count; i++) {
                var ep = _savedPropertyList[i];
                if (ep.Type.FullName == propertyType.FullName) {
                    return ep.Name;
                }
                if (propertyType.IsSubclassOf( ep.Type )) {
                    name = ep.Name;
                }
            }
            return name;
        }

        private static String GetTableName( Type t ) {
            var attribute = ReflectionUtil.GetAttribute( t, typeof( TableAttribute ) ) as TableAttribute;
            if (attribute == null) {
                return t.Name;
            }
            return attribute.TableName;
        }



        private static String GetTypeLabel( Type t ) {
            var attribute = ReflectionUtil.GetAttribute( t, typeof( LabelAttribute ) ) as LabelAttribute;
            if (attribute == null) {
                return t.Name;
            }
            return attribute.Label;
        }


    }
}

