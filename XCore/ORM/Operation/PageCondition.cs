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
using System.Data;
using System.Data;

namespace System.ORM.Operation {

    internal class PageCondition {

        private static readonly ILog logger = LogManager.GetLogger( typeof( PageCondition ) );
		
		private String _ConditionStr;
		public String ConditionStr { get{return _ConditionStr;} set{_ConditionStr=value;} }
		private String _Property;
		public String Property { get{return _Property;} set{_Property=value;} }
		private int _CurrentPage;
		public int CurrentPage { get{return _CurrentPage;} set{_CurrentPage=value;} }
		private int _Size;
		public int Size { get{return _Size;} set{_Size=value;} }
		private String _OrderStr;
		public String OrderStr { get{return _OrderStr;} set{_OrderStr=value;} }
		private ObjectPage _Pager;
		public ObjectPage Pager { get{return _Pager;} set{_Pager=value;} }

        public int beginCount( String countSql, ObjectPage pager, EntityInfo entityInfo ) {

            String commandText = countSql;
            logger.Info( "[Page Record Count] " + commandText );
            IDbCommand command = DataFactory.GetCommand( commandText, DbContext.getConnection( entityInfo ) );
            pager.RecordCount = cvt.ToInt( command.ExecuteScalar() );
            pager.computePageCount();
            pager.resetCurrent();

            return pager.getCurrent();
        }

    }

}
