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
using System.Text;

namespace System.ORM.Operation {

    internal class ConditionInfo {
		
		private Type _Type;
		public Type Type { get{return _Type;} set{_Type=value;}  }		
		private String _SelectedItem;
		public String SelectedItem { get{return _SelectedItem;} set{_SelectedItem=value;}  }
		private String _WhereString;
		public String WhereString { get{return _WhereString;} set{_WhereString=value;} }
		private Dictionary<String,Object> _Parameters;
		public Dictionary<String,Object> Parameters { get{return _Parameters;} set{_Parameters=value;}  }
		private int _Count;
		public int Count { get{return _Count;} set{_Count=value;} }
		private String _Sql;
		public String Sql { get{return _Sql;} set{_Sql=value;} }
		private ObjectInfo _State;
		public ObjectInfo State { get{return _State;} set{_State=value;}  }

    }

}
