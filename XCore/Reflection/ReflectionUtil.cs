//------------------------------------------------------------------------------
//	文件名称：System\Reflection\ReflectionUtil.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
namespace System.Reflection {
    /// <summary>
    /// 封装了反射的常用操作方法
    /// </summary>
    public class ReflectionUtil {
        private static readonly ILog logger = LogManager.GetLogger( typeof( ReflectionUtil ) );
        /// <summary>
        /// 通过反射创建对象(Activator.CreateInstance)
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Object GetInstance( Type t ) {
            return Activator.CreateInstance( t );
        }
        /// <summary>
        /// 通过反射创建对象(Activator.CreateInstance)，并提供构造函数
        /// </summary>
        /// <param name="t"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Object GetInstance( Type t, params object[] args ) {
            return Activator.CreateInstance( t, args );
        }
        /// <summary>
        /// 为类型创建对象(通过加载指定程序集中的类型)
        /// </summary>
        /// <param name="asmName">不需要后缀名</param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Object GetInstance( String asmName, String typeName ) {
            // Load不需要ext，LoadFrom需要
            Assembly asm = Assembly.Load( asmName );
            return asm.CreateInstance( typeName );
        }
        /// <summary>
        /// 为类型创建对象(直接指定类型的完全限定名称)
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <returns></returns>
        private static Object GetInstance(String typeFullName)
        {
            if (typeFullName.IndexOf(',') <= 0)
                typeFullName = typeFullName + "," + Assembly.GetEntryAssembly().GetName().Name;
            return rft.GetInstance(Type.GetType(typeFullName));
        }
        public static Object GetInstanceFromProgId( String progId ) {
            return rft.GetInstance( Type.GetTypeFromProgID( progId ) );
        }
        public static IList GetPropertyList( Type t ) {
            PropertyInfo[] properties = t.GetProperties( BindingFlags.Public | BindingFlags.Instance );
            IList list = new ArrayList();
            foreach (PropertyInfo info in properties) {
                list.Add( info );
            }
            return list;
        }
        public static Object GetPropertyValue( Object currentObject, String propertyName ) {
            if (currentObject == null) return null;
            if (strUtil.IsNullOrEmpty( propertyName )) return null;
            PropertyInfo p = currentObject.GetType().GetProperty( propertyName );
            if (p == null) return null;
            return p.GetValue( currentObject, null );
        }
        public static void SetPropertyValue(Object currentObject, String propertyName, Object propertyValue)
        {
            if (currentObject == null)
                throw new NullReferenceException(String.Format("propertyName={0}, propertyValue={1}", propertyName, propertyValue));
            try
            {
                propertyValue = Convert.ChangeType(propertyValue, currentObject.GetType().GetProperty(propertyName).PropertyType);
                currentObject.GetType().GetProperty(propertyName).SetValue(currentObject, propertyValue, null);
            }
            catch (Exception exception)
            {
                //throw new Exception(exception.Message + "(propertyName=" + propertyName + ")");
            }
        }
        /// <summary>
        /// 获取属性的类型的fullName(对泛型名称做了特殊处理)
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static String getPropertyTypeName( PropertyInfo p ) {
            if (p.PropertyType.IsGenericType == false)
                return p.PropertyType.FullName;
            Type pGenericType = p.PropertyType.GetGenericTypeDefinition();
            String genericTypeName = pGenericType.FullName.Split( '`' )[0];
            Type[] ts = p.PropertyType.GetGenericArguments();
            String args = null;
            foreach (Type at in ts) {
                if (args != null) args += ", ";
                args += at.FullName;
            }
            return genericTypeName + "<" + args + ">";
        }
        public static Object CallMethod( Object obj, String methodName ) {
            return CallMethod( obj, methodName, null );
        }
        public static Object CallMethod( Type currentType, String methodName ) {
            return CallMethod( currentType, methodName, null );
        }
        public static Object CallMethod( Object obj, String methodName, object[] args ) {
            return obj.GetType().InvokeMember( methodName, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, obj, args );
        }
        public static Object CallMethod( Type currentType, String methodName, object[] args ) {
            return CallMethod( rft.GetInstance( currentType ), methodName, args );
        }
        /// <summary>
        /// 获取 public 实例方法，不包括继承的方法
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static MethodInfo[] GetMethods( Type t ) {
            return t.GetMethods( BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly );
        }
        /// <summary>
        /// 获取 public 实例方法，包括继承的方法
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static MethodInfo[] GetMethodsWithInheritance( Type t ) {
            return t.GetMethods( BindingFlags.Public | BindingFlags.Instance );
        }
        public static Attribute GetAttribute( MemberInfo memberInfo, Type attributeType ) {

            object[] customAttributes = memberInfo.GetCustomAttributes( attributeType, false );
            if (customAttributes.Length == 0) return null;
            return customAttributes[0] as Attribute;
        }
        public static object[] GetAttributes( MemberInfo memberInfo ) {
            return memberInfo.GetCustomAttributes( false );
        }
        public static object[] GetAttributes( MemberInfo memberInfo, Type attributeType ) {
            return memberInfo.GetCustomAttributes( attributeType, false );
        }
        public static Boolean IsBaseType( Type type ) {
            return type == typeof(int) ||
                type == typeof(long) ||
                type == typeof(short) ||
                type == typeof( String ) ||
                type == typeof( DateTime ) ||
                type == typeof(bool) ||
                type == typeof(double) ||
                type == typeof(float) ||
                type == typeof(object) ||
                type == typeof( decimal );
        }
        /// <summary>
        /// 判断 t 是否实现了某种接口
        /// </summary>
        /// <param name="t">需要判断的类型</param>
        /// <param name="interfaceType">是否实现的接口</param>
        /// <returns></returns>
        public static Boolean IsInterface( Type t, Type interfaceType ) {
            Type[] interfaces = t.GetInterfaces();
            foreach (Type type in interfaces) {
                if (interfaceType.FullName.Equals(type.FullName)) return true;
            }
            return false;
        }
    }
}