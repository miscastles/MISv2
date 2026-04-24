using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.AccessControl;

namespace MIS
{
    public class clsODBCManager
    {        
        public const string ODBC_INI_REG_PATH = "SOFTWARE\\ODBC\\ODBC.INI\\";
        public const string ODBCINST_INI_REG_PATH = "SOFTWARE\\WOW6432Node\\ODBC\\ODBCINST.INI\\";

        //Computer\HKEY_USERS\S-1-5-21-2900863311-3183097920-2942051195-1001\Software\ODBC\ODBC.INI\ODBC Data Sources

        public void CreateSystemDSN(string dsnName, string description, string server, string driverName, bool trustedConnection, string database, string User, string Password)
        {

            // Lookup driver path from driver name
            RegistryKey driverKey = Registry.LocalMachine.CreateSubKey(ODBCINST_INI_REG_PATH + driverName);
            if (driverKey == null) throw new Exception(string.Format("ODBC Registry key for driver '{0}' does not exist", driverName));
            string driverPath = driverKey.GetValue("Driver").ToString();

            // Add value to odbc data sources
            RegistryKey datasourcesKey = Registry.LocalMachine.CreateSubKey(ODBC_INI_REG_PATH + "ODBC Data Sources");
            if (datasourcesKey == null) throw new Exception("ODBC Registry key for datasources does not exist");
            datasourcesKey.SetValue(dsnName, driverName);

            // Create new key in odbc.ini with dsn name and add values
            RegistryKey dsnKey = Registry.LocalMachine.CreateSubKey(ODBC_INI_REG_PATH + dsnName);
            if (dsnKey == null) throw new Exception("ODBC Registry key for DSN was not created");
            dsnKey.SetValue("Database", database);
            dsnKey.SetValue("Description", description);
            dsnKey.SetValue("Driver", driverPath);
            dsnKey.SetValue("Server", server);
            dsnKey.SetValue("Database", database);
            dsnKey.SetValue("User", User);
            dsnKey.SetValue("Password", Password);

            dsnKey.SetValue("Trusted_Connection", trustedConnection ? "Yes" : "No");

        }


        public void CreateUSERDSN(string dsnName, string description, string server, string driverName, bool trustedConnection, string database, string User, string Password)
        {
            RegistrySecurity userSecurity = new RegistrySecurity();
            RegistryAccessRule userRule = new RegistryAccessRule("Everyone", RegistryRights.FullControl, AccessControlType.Allow);
            userSecurity.AddAccessRule(userRule);

            // Lookup driver path from driver name for HKEY_CURRENT_USER
            RegistryKey driverKey = Registry.LocalMachine.CreateSubKey(ODBCINST_INI_REG_PATH + "ODBC Drivers\\" + driverName, RegistryKeyPermissionCheck.ReadWriteSubTree, userSecurity);

            if (driverKey == null) throw new Exception(string.Format("ODBC Registry key for driver '{0}' does not exist", driverName));
            string driverPath = driverKey.GetValue("Driver").ToString();

            // Add value to odbc data sources
            RegistryKey datasourcesKey = Registry.CurrentUser.CreateSubKey(ODBC_INI_REG_PATH + "ODBC Data Sources");
            if (datasourcesKey == null) throw new Exception("ODBC Registry key for datasources does not exist");
            datasourcesKey.SetValue(dsnName, driverName);

            // Create new key in odbc.ini with dsn name and add values
            RegistryKey dsnKey = Registry.CurrentUser.CreateSubKey(ODBC_INI_REG_PATH + dsnName);
            if (dsnKey == null) throw new Exception("ODBC Registry key for DSN was not created");
            dsnKey.SetValue("Database", database);
            dsnKey.SetValue("Description", description);
            dsnKey.SetValue("Driver", driverPath);
            dsnKey.SetValue("Server", server);
            dsnKey.SetValue("Database", database);
            dsnKey.SetValue("User", User);
            dsnKey.SetValue("Password", Password);

            dsnKey.SetValue("Trusted_Connection", trustedConnection ? "Yes" : "No");

        }

        ///
        /// Removes a DSN entry
        ///
        /// Name of the DSN to remove.
        public void RemoveDSN(string dsnName)
        {

            // Remove DSN key
            Registry.LocalMachine.DeleteSubKeyTree(ODBC_INI_REG_PATH + dsnName);

            // Remove DSN name from values list in ODBC Data Sources key
            RegistryKey datasourcesKey = Registry.LocalMachine.CreateSubKey(ODBC_INI_REG_PATH + "ODBC Data Sources");
            if (datasourcesKey == null) throw new Exception("ODBC Registry key for datasources does not exist");
            datasourcesKey.DeleteValue(dsnName);

        }

        ///
        /// Checks the registry to see if a DSN exists with the specified name
        ///
        public bool DSNExists(string dsnName)
        {

            bool retval = false;
            string DSNPath = "";
            try
            {
                RegistryKey dsnKey = Registry.CurrentUser.CreateSubKey(ODBC_INI_REG_PATH, RegistryKeyPermissionCheck.ReadSubTree);
                if (dsnKey.OpenSubKey(dsnName) != null)
                {
                    DSNPath = dsnKey.OpenSubKey(dsnName).Name;
                    if (DSNPath.Equals(dsnKey.Name + @"\" + dsnName))
                    {
                        retval = true;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
                //MessageBox.Show(ex.ToString());
            }
            return retval;

        }

        ///
        /// Returns an array of driver names installed on the system
        ///


    }
}

