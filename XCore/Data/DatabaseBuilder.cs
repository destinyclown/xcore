using System;
using System.Data;
using System.Reflection;
using System.IO;
using System.Web;

namespace System.Data {


    internal class DatabaseBuilder {

        public static String ConnectionStringPrefix = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
        private static readonly ILog logger = LogManager.GetLogger( typeof( DatabaseBuilder ) );

        public static String BuildAccessDb4o() {
            String dbPath = "xcore.mdb";
            BuildAccessDb4o( dbPath );
            return dbPath;
        }
        public static String BuildAccessDb4o(String dbPath)
        {
            dbPath = PathHelper.Map(dbPath);
            String str = dbPath;
            logger.Info("creating database : " + str);
            Object instanceFromProgId = ReflectionUtil.GetInstanceFromProgId("ADOX.Catalog");
            try
            {
                ReflectionUtil.CallMethod(instanceFromProgId, "Create", new object[] { ConnectionStringPrefix + str });
            }
            catch (Exception exception) {
                logger.Info( "creating database error : " + exception.Message );
                LogManager.Flush();
                throw exception;
            }
            logger.Info( "create database ok" );
            return str;
        }
        

    }
}

