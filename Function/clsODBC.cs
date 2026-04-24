using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsODBC
    {
        static string ServerName;
        static string DSNName;
        static string DriverName;
        static string DatabaseName;
        static string Description;
        static string User;
        static string Password;

        public static string ClassODBCServerName
        {
            get { return ServerName; }
            set { ServerName = value; }
        }
        public static string ClassODBCDSNName
        {
            get { return DSNName; }
            set { DSNName = value; }
        }
        public static string ClassODBCDriverName
        {
            get { return DriverName; }
            set { DriverName = value; }
        }
        public static string ClassODBCDatabaseName
        {
            get { return DatabaseName; }
            set { DatabaseName = value; }
        }
        public static string ClassODBCDescription
        {
            get { return Description; }
            set { Description = value; }
        }
        public static string ClassODBCUser
        {
            get { return User; }
            set { User = value; }
        }
        public static string ClassODBCPassword
        {
            get { return Password; }
            set { Password = value; }
        }
    }
}
