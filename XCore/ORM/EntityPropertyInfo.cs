//------------------------------------------------------------------------------
//	文件名称：System\ORM\EntityPropertyInfo.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
namespace System.ORM {
    /// <summary>
    /// 实体类某个属性的元数据信息
    /// </summary>
    [Serializable]
    public partial class EntityPropertyInfo {
        public EntityPropertyInfo() {
            this.IsList = false;
            this.IsAbstractEntity = false;
            this.IsEntity = false;
            this.ValidationAttributes = new List<ValidationAttribute>();
        }
        private String _Name;
        /// <summary>
        /// 属性名称
        /// </summary>
        public String Name {get{return _Name;} set{_Name=value;} }
        private String _ColumnName;
        /// <summary>
        /// 对应的数据列名
        /// </summary>
        public String ColumnName {get{return _ColumnName;} set{_ColumnName=value;} }
        private Type _Type;
        /// <summary>
        /// 属性的类型，比如是int，还是string等等
        /// </summary>
        public Type Type {get{return _Type;} set{_Type=value;} }
        private PropertyInfo _Property;
        /// <summary>
        /// 属性信息(系统自带的元数据)
        /// </summary>
        public PropertyInfo Property {get{return _Property;} set{_Property=value;} }
        private EntityInfo _ParentEntityInfo;
        /// <summary>
        /// 本属性所属的实体类信息，比如Blog有一个属性Title，则Title这个属性的ParentEntityInfo就是Blog
        /// </summary>
        public EntityInfo ParentEntityInfo {get{return _ParentEntityInfo;} set{_ParentEntityInfo=value;} }
        private EntityInfo _EntityInfo;
        /// <summary>
        /// 当本属性是实体属性时，此实体属性的信息。比如Blog的实体属性Category的EntityInfo。如果不是实体属性，则为null
        /// </summary>
        public EntityInfo EntityInfo {get{return _EntityInfo;} set{_EntityInfo=value;} }
        private Boolean _SaveToDB;
        /// <summary>
        /// 是否保存到数据库(是否打上了NotSave批注)
        /// </summary>
        public Boolean SaveToDB {get{return _SaveToDB;} set{_SaveToDB=value;} }
        private Boolean _IsList;
        /// <summary>
        /// 是否是列表类型，列表类型不会保存到数据库
        /// </summary>
        public Boolean IsList {get{return _IsList;} set{_IsList=value;} }
        private Boolean _IsEntity;
        /// <summary>
        /// 是否是实体类属性
        /// </summary>
        public Boolean IsEntity {get{return _IsEntity;} set{_IsEntity=value;} }
        private Boolean _IsAbstractEntity;
        /// <summary>
        /// 是否是抽象类型实体
        /// </summary>
        public Boolean IsAbstractEntity {get{return _IsAbstractEntity;} set{_IsAbstractEntity=value;} }
        private ColumnAttribute _SaveAttribute;
        /// <summary>
        /// 当前属性的 ColumnAttribute
        /// </summary>
        public ColumnAttribute SaveAttribute {get{return _SaveAttribute;} set{_SaveAttribute=value;} }
        private LongTextAttribute _LongTextAttribute;
        /// <summary>
        /// 当前属性的 LongTextAttribute
        /// </summary>
        public LongTextAttribute LongTextAttribute {get{return _LongTextAttribute;} set{_LongTextAttribute=value;} }
        private DateTimeAttribute _DateTimeAttribute;
        /// <summary>
        /// 当前属性的 DateTimeAttribute
        /// </summary>
        public DateTimeAttribute DateTimeAttribute {get{return _DateTimeAttribute;} set{_DateTimeAttribute=value;} }
        private MoneyAttribute _MoneyAttribute;
        /// <summary>
        /// 当前属性的 MoneyAttribute
        /// </summary>
        public MoneyAttribute MoneyAttribute {get{return _MoneyAttribute;} set{_MoneyAttribute=value;} }
        private DecimalAttribute _DecimalAttribute;
        /// <summary>
        /// 当前属性的 DecimalAttribute
        /// </summary>
        public DecimalAttribute DecimalAttribute {get{return _DecimalAttribute;} set{_DecimalAttribute=value;} }
        private DefaultAttribute _DefaultAttribute;
        /// <summary>
        /// 当前属性的 DefaultAttribute
        /// </summary>
        public DefaultAttribute DefaultAttribute {get{return _DefaultAttribute;} set{_DefaultAttribute=value;} }
        private List<ValidationAttribute> _ValidationAttributes;
        /// <summary>
        /// 当前属性的 ValidationAttribute 的列表
        /// </summary>
        public List<ValidationAttribute> ValidationAttributes {get{return _ValidationAttributes;} set{_ValidationAttributes=value;} }
        
		private IPropertyAccessor _PropertyAccessor;
		/// <summary>
        /// 当前属性的赋值/取值器，可以避免反射的低效
        /// </summary>
		internal IPropertyAccessor PropertyAccessor {get{return _PropertyAccessor;} set{_PropertyAccessor=value;} }
        internal static EntityPropertyInfo Get( PropertyInfo property ) {
            EntityPropertyInfo ep = new EntityPropertyInfo();
            object[] arrAttr = property.GetCustomAttributes( typeof( ValidationAttribute ), true );
            foreach (Object at in arrAttr) {
                ep.ValidationAttributes.Add( at as ValidationAttribute );
            }
            ep.SaveAttribute = ReflectionUtil.GetAttribute( property, typeof( ColumnAttribute ) ) as ColumnAttribute;
            ep.LongTextAttribute = ReflectionUtil.GetAttribute( property, typeof( LongTextAttribute ) ) as LongTextAttribute;
            ep.DateTimeAttribute = ReflectionUtil.GetAttribute(property, typeof(DateTimeAttribute)) as DateTimeAttribute;
            ep.MoneyAttribute = ReflectionUtil.GetAttribute( property, typeof( MoneyAttribute ) ) as MoneyAttribute;
            ep.DecimalAttribute = ReflectionUtil.GetAttribute( property, typeof( DecimalAttribute ) ) as DecimalAttribute;
            ep.DefaultAttribute = ReflectionUtil.GetAttribute( property, typeof( DefaultAttribute ) ) as DefaultAttribute;
            ep.Property = property;
            ep.Name = property.Name;
            ep.Type = property.PropertyType;
            ep.SaveToDB = !property.IsDefined( typeof( NotSaveAttribute ), false );
            if (property.PropertyType is IList) {
                ep.IsList = true;
                ep.SaveToDB = false;
            }
            return ep;
        }
        /// <summary>
        /// 获取obj的当前属性的值
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Object GetValue( Object target ) {
            return PropertyAccessor.Get( target );
        }
        /// <summary>
        /// 给obj的当前属性赋值
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        public void SetValue( Object target, Object value ) {
            PropertyAccessor.Set( target, value );
        }
        /// <summary>
        /// 是否是长文本
        /// </summary>
        public Boolean IsLongText {
            get {
                if (Type != typeof( String )) return false;
                return LongTextAttribute != null || ((SaveAttribute != null) && (SaveAttribute.Length > 255));
            }
        }
        /// <summary>
        /// 获取属性的label(用在表单中)
        /// </summary>
        public String Label {
            get {
                if (SaveAttribute == null) return Name;
                if (strUtil.IsNullOrEmpty( SaveAttribute.Label )) return Name;
                return SaveAttribute.Label;
            }
        }
    }
}