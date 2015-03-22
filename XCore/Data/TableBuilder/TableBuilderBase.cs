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
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.ORM;
using System.Log;

namespace System.Data {

    internal class TableBuilderBase {


        private static readonly ILog logger = LogManager.GetLogger( typeof( TableBuilderBase ) );

        public List<String> CheckMappingTableIsExist( IDbCommand cmd, String db, List<String> existTables, MappingClass mapping ) {
            foreach (DictionaryEntry entry in mapping.ClassList) {
                EntityInfo entity = entry.Value as EntityInfo;
                if (entity.Database.Equals( db ) == false) continue;

                if (!isTableCreated( existTables, entity )) {
                    existTables = createTable( entity, cmd, existTables, mapping.ClassList );
                }
            }
            return existTables;
        }

        private List<String> createTable( EntityInfo entity, IDbCommand cmd, List<String> existTables, IDictionary clsList ) {

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat( "Create Table {0} (", getFullName( entity.TableName, entity ) );
            addColumn_PrimaryKey( entity, sb, clsList );

            addColumns( entity, sb );
            String str = sb.ToString().Trim().TrimEnd( new char[] { ',' } ) + " )";

            cmd.CommandText = str;
            logger.Info( "创建表:" + str );
            if (cmd.Connection == null) throw new Exception( "connection is null" );

            if (cmd.Connection.State == ConnectionState.Closed) {
                cmd.Connection.Open();
                OrmHelper.initCount++;
                LogManager.GetLogger("Class:System.Data.TableBuilderBase Method:createTable").Info("数据库连接已开启【" + OrmHelper.initCount + "】");
            }

            cmd.ExecuteNonQuery();

            existTables.Add( entity.TableName );
            logger.Info( LoggerUtil.SqlPrefix + String.Format( "create table {0} ({1})", entity.TableName, entity.FullName ) );

            return existTables;
        }

        private void addColumns( EntityInfo entity, StringBuilder sb ) {
            for (int i = 0; i < entity.SavedPropertyList.Count; i++)
            {
                EntityPropertyInfo ep = entity.SavedPropertyList[i];
                String columnName = getFullName(ep.ColumnName, entity);
                if (ep.Name == "Id")
                {
                    String message = String.Format("字段“Id”为系统内置自增长字段，请勿自定义此字段！");
                    logger.Info(message);
                }
                else if (ep.SaveToDB && !ep.IsList)
                {
                    addColumnSingle(entity, sb, ep, columnName);
                }
            }
        }

        private void addColumnSingle( EntityInfo entity, StringBuilder sb, EntityPropertyInfo ep, String columnName ) {
            if (ep.Type == typeof(int) || ep.Type == typeof(long))
            {
                addColumn_Int(sb, entity, ep, columnName);
            }
            else if (ep.Type == typeof(bool))
            {
                addColumn_Boolean(sb, ep, columnName);
            }
            else if (ep.Type == typeof(DateTime))
            {
                addColumn_Time(sb, columnName);
            }
            else if (ep.Type == typeof(decimal))
            {
                addColumn_Decimal(entity, sb, ep, columnName);
            }
            else if (ep.Type == typeof(double))
            {
                addColumn_Double(entity, sb, columnName);
            }
            else if (ep.Type == typeof(float))
            {
                addColumn_Single(entity, sb, columnName);
            }
            else if (ep.Type == typeof(String))
            {
                addColumn_String(entity, sb, ep, columnName);
            }
            else if (ep.IsEntity)
            {
                addColumn_entity(sb, columnName);
            }
        }



        //------------------------------------------------------------------------------------------------------------------------

        protected virtual void addColumn_PrimaryKey( EntityInfo entity, StringBuilder sb, IDictionary clsList ) {
            if (isAddIdentityKey( entity.Type ) == false) {
                sb.Append( " Id int primary key default 0, " );
            }
            else {
                sb.Append( " Id int identity(1,1) primary key, " );
            }
        }

        private Boolean isAddIdentityKey( Type t ) {
            if (OrmHelper.IsEntityBase( t.BaseType )) return true;
            if (t.BaseType.IsAbstract) return true;
            return false;
        }


        protected virtual void addColumn_Int(StringBuilder sb, EntityInfo entity, EntityPropertyInfo ep, String columnName)
        {
            sb.Append( columnName );
            if (ep.Property.IsDefined( typeof( TinyIntAttribute ), false )) {
                sb.Append(" tinyint default 0, ");
            }
            else if (ep.Property.IsDefined(typeof(LongAttribute), false))
            {
                if (entity.DbType == DatabaseType.Access)
                {
                    sb.Append(" double default 0, ");
                }
                else
                {
                    sb.Append(" bigint default 0, ");
                }
            }
            else
            {
                sb.Append(" int default 0, ");
            }
        }
        protected virtual void addColumn_Boolean(StringBuilder sb, EntityPropertyInfo ep, String columnName)
        {
            sb.Append(columnName);
            sb.Append(" tinyint default 0, ");
        }


        protected virtual void addColumn_Time( StringBuilder sb, String columnName ) {
            sb.Append( columnName );
            sb.Append( " DateTime, " );
        }

        protected virtual void addColumn_Decimal( EntityInfo entity, StringBuilder sb, EntityPropertyInfo ep, String columnName ) {
            if (ep.MoneyAttribute != null) {
                sb.Append( columnName );
                sb.Append( " money default 0, " );
            }
            else {

                DecimalAttribute da = ep.DecimalAttribute;
                if (da == null) throw new Exception( "DecimalAttribute not found=" + entity.FullName + "_" + ep.Name );

                sb.Append( columnName );
                sb.Append( " decimal(" + da.Precision + "," + da.Scale + ") default 0, " );
            }
        }

        protected virtual void addColumn_Double( EntityInfo entity, StringBuilder sb, string columnName ) {
            sb.Append( columnName );
            sb.Append( " float default 0, " );
        }


        protected virtual void addColumn_Single( EntityInfo entity, StringBuilder sb, string columnName ) {
            sb.Append( columnName );
            sb.Append( " real default 0, " );
        }


        protected virtual void addColumn_String( EntityInfo entity, StringBuilder sb, EntityPropertyInfo ep, String columnName ) {
            if (ep.LongTextAttribute != null) {
                addColumn_LongText( entity, sb, columnName );
            }
            else if (ep.SaveAttribute != null) {
                addColumn_ByColumnAttribute( entity, sb, ep, columnName );
            }
            else {
                addColumn_ShortText( sb, columnName, 250 );
            }
        }


        protected virtual void addColumn_LongText( EntityInfo entity, StringBuilder sb, String columnName ) {
            sb.Append( columnName );
            sb.Append( " ntext, " );

        }

        protected virtual void addColumn_ShortText( StringBuilder sb, String columnName, int length ) {
            sb.Append( columnName );
            sb.Append( " nvarchar(" );
            sb.Append( length );
            sb.Append( "), " );
        }


        protected virtual void addColumn_ByColumnAttribute( EntityInfo entity, StringBuilder sb, EntityPropertyInfo ep, String columnName ) {
            if (ep.SaveAttribute.Length < 255) {
                addColumn_ShortText( sb, columnName, ep.SaveAttribute.Length );
            }
            else if ((ep.SaveAttribute.Length > 255) && (ep.SaveAttribute.Length < 4000)) {
                addColumn_MiddleText( entity, sb, ep, columnName );
            }
            else {
                addColumn_LongText( entity, sb, columnName );
            }
        }

        protected virtual void addColumn_MiddleText( EntityInfo entity, StringBuilder sb, EntityPropertyInfo ep, String columnName ) {

            addColumn_ShortText( sb, columnName, ep.SaveAttribute.Length );
        }


        protected virtual void addColumn_entity( StringBuilder sb, String columnName ) {
            sb.Append( columnName );
            sb.Append( " int default 0, " );
        }


        //------------------------------------------------------------------------------------------------------------------------

        private String getFullName( String name, EntityInfo entity ) {
            if (DbConst.SqlKeyWords.Contains( name.ToLower() )) {
                String message = String.Format("'{0}' is reserved word. Entity:{1}, Table:{2}\n您在实体类：'{1}'中使用的字段：'{0}'为系统保留字段，对应的表名为：'{2}'，请修改后重试！", name, entity.FullName, entity.TableName);
                logger.Info( message );
                throw new Exception( message );
            }
            return name;
        }

        private Boolean isTableCreated( IList existTables, EntityInfo entity ) {
            for (int i = 0; i < existTables.Count; i++) {
                if (string.Compare( existTables[i].ToString(), entity.TableName.Replace( "[", "" ).Replace( "]", "" ), true ) == 0) {
                    logger.Info( "映射类 : " + entity.FullName + " => " + existTables[i] );
                    return true;
                }
            }
            return false;
        }

    }

}
