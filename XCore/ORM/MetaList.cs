//------------------------------------------------------------------------------
//	文件名称：System\ORM\MetaList.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Text;
namespace System.ORM {
    /// <summary>
    /// 元数据列表
    /// </summary>
    public class MetaList {
        public MetaList( IDictionary asmList, IDictionary clsList ) {
            this.AssemblyList = asmList;
            this.ClassList = clsList;
        }
		private IDictionary _AssemblyList;
		public IDictionary AssemblyList { get{return _AssemblyList;} set{_AssemblyList=value;}  }
		private IDictionary _ClassList;
		public IDictionary ClassList { get{return _ClassList;} set{_ClassList=value;}  }
    }
}