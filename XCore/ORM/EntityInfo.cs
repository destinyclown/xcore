using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using System.Data;

namespace System.ORM {

    /// <summary>
    /// ʵ�����Ԫ������Ϣ
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
        /// ��Ӧ�����ݱ��е������е�����
        /// </summary>
        public String ColumnList {
            get { return _columnList; }
            set { _columnList = value; }
        }

        /// <summary>
        /// ֻ��ʵ�����ʵ����Ե��б����� BlogPost ��ĳ�������� BlogCategory ������ʵ������
        /// </summary>
        public List<EntityPropertyInfo> EntityPropertyList {
            get { return _entityPropertyList; }
            set { _entityPropertyList = value; }
        }

        /// <summary>
        /// �������Ե��б�(�����Ѿ���װ��EntityPropertyInfo)
        /// </summary>
        public List<EntityPropertyInfo> PropertyListAll {
            get { return _propertyListAll; }
            set { _propertyListAll = value; }
        }

        /// <summary>
        /// ������Ҫ���������
        /// </summary>
        public List<EntityPropertyInfo> SavedPropertyList {
            get { return _savedPropertyList; }
            set { _savedPropertyList = value; }
        }

        /// <summary>
        /// ʵ����ȫ�������� System.apps.BlogApp
        /// </summary>
        public String FullName {
            get { return _fullName; }
            set { _fullName = value; }
        }

        /// <summary>
        /// ʵ�����ڱ��е����ƣ����ڱ������Զ�����
        /// </summary>
        public String Label {
            get { return _label; }
            set { _label = value; }
        }

        /// <summary>
        /// ʵ�������ƣ���ͬ��type.Name������BlogApp
        /// </summary>
        public String Name {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// ��ǰʵ����ĸ��࣬������Ǽ̳���ĳ������Ļ�
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
        /// ʵ�����Ӧ�����ݱ�����
        /// </summary>
        public String TableName {
            get { return _tableName; }
            set { _tableName = value; }
        }

        /// <summary>
        /// ʵ�����Ӧ��Type
        /// </summary>
        public Type Type {
            get { return _type; }
            set { _type = value; }
        }



        /// <summary>
        /// ʵ����� dialect
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
        /// ���ݿ�����
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
        /// ��������Type����ʼ��EntityInfo��ע�⣺��Ϊ���Ǵӻ�����ȡ�������ٶȽ���
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
        /// ��ȡĳ�����������ݿ��ж�Ӧ������������
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
        /// ��ȡĳ�����Ե�Ԫ������Ϣ(�ѷ�װ��EntityPropertyInfo)
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public EntityPropertyInfo GetProperty( String propertyName ) {
            return (_propertyHashTable[propertyName] as EntityPropertyInfo);
        }

        /// <summary>
        /// ����column���ƣ���ȡ��ȡĳ�����Ե�Ԫ������Ϣ(�ѷ�װ��EntityPropertyInfo)
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
        /// �������Ե����ͣ�����BlogCategory����ȡ����Ҫ��ĵ�һ�����Ե�����
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

