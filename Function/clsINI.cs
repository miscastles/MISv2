using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;

namespace MIS
{
    class clsINI
    {
        private string filePath;
        private clsFile dbFile = new clsFile();
        private clsFunction dbFunction = new clsFunction();

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);


        public void INIFile(string sfilePath)
        {
            filePath = sfilePath;
        }

        public string Read(string section, string key)
        {
            StringBuilder SB = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, "", SB, 255, filePath);
            return SB.ToString();
        }
        public long Write(string section, string key, string value)
        {
            long i = WritePrivateProfileString(section, key, value, filePath);
            return i;
        }

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        // --------------------------------------------------------------------------------------------------------------------------
        // Read
        // --------------------------------------------------------------------------------------------------------------------------
        public void InitDatabaseSetting()
        {
            string Path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string FullPath = "";
            string FileName = "";

            FileName = "DatabaseSetting.ini";
            FullPath = dbFile.sSettingPath + FileName;
            this.INIFile(FullPath);

            string filepath = dbFile.sSettingPath + clsDefines.RESP_DATABASELIST_FILENAME;
            clsDatabase dbInfo = GetDatabaseByCode(filepath, clsSearch.ClassBankCode);

            if (dbInfo != null)
            {
                Debug.WriteLine("Source=" + dbInfo.Source);
                Debug.WriteLine("Server=" + dbInfo.Server);
                Debug.WriteLine("Database=" + dbInfo.Database);
                Debug.WriteLine("Username=" + dbInfo.Username);
                Debug.WriteLine("Password=" + dbInfo.Password);

                clsGlobalVariables.strSource = dbInfo.Source;
                clsGlobalVariables.strServer = dbInfo.Server;
                clsGlobalVariables.strDatabase = dbInfo.Database;
                clsGlobalVariables.strUserName = dbInfo.Username;
                clsGlobalVariables.strPassword = dbInfo.Password;
                clsGlobalVariables.strSecurity = dbInfo.Security;
                clsGlobalVariables.strPort = dbInfo.Port.ToString();
                clsGlobalVariables.strTimeOut = dbInfo.ConnectionTimeout.ToString();
            }
            else
            {
                dbFunction.SetMessageBox("Database code " + clsSearch.ClassBankCode + " not found.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError); 
            }
            
            /*
            clsGlobalVariables.strSource = this.Read("_Source", "Source");
            clsGlobalVariables.strServer = this.Read("_Server", "Server");
            clsGlobalVariables.strDatabase = this.Read("_Database", "Database");
            clsGlobalVariables.strUserName = this.Read("_UserName", "UserName");
            clsGlobalVariables.strPassword = this.Read("_Password", "Password");
            clsGlobalVariables.strSecurity = this.Read("_Security", "Security");
            clsGlobalVariables.strPort = this.Read("_Port", "Port");
            clsGlobalVariables.strTimeOut = this.Read("_ConnectTimeout", "ConnectTimeout");
            */
        }

        public void InitAPISetting()
        {
            string Path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string FullPath = "";
            string FileName = "";

            FileName = "APISetting.ini";
            FullPath = dbFile.sSettingPath + FileName;
            this.INIFile(FullPath);

            clsGlobalVariables.strAPIURL = clsSearch.ClassAPIURL = this.Read("_APIURL", "APIURL");
            clsGlobalVariables.strAPIPath = clsSearch.ClassAPIPath = this.Read("_APIPath", "APIPath");
            clsGlobalVariables.strAPIKeys = clsSearch.ClassAPIKeys =  this.Read("_APIKeys", "APIKeys");
            clsGlobalVariables.strAPIContentType = clsSearch.ClassAPIContentType =  this.Read("_APIContentType", "APIContentType");
            clsGlobalVariables.strAPIAuthUser = clsSearch.ClassAPIAuthUser = this.Read("_APIAuthUser", "APIAuthUser");
            clsGlobalVariables.strAPIAuthPassword = clsSearch.ClassAPIAuthPassword = this.Read("_APIAuthPassword", "APIAuthPassword");
            clsGlobalVariables.strAPIDashboardURL = clsSearch.ClassAPIDashboardURL =  this.Read("_APIDashboardPath", "APIDashboardPath");
            clsGlobalVariables.strAPIServerIPAddress = clsSearch.ClassAPIServerIPAddress = this.Read("_APIIPAddress", "APIIPAddress");
            clsGlobalVariables.strAPIFolder = this.Read("_APIFolder", "APIFolder");
            clsGlobalVariables.strAPIImagePath = this.Read("_APIImagePath", "APIImagePath");
            //clsGlobalVariables.strAPIBank = this.Read("_APIBank", "APIBank");
            clsGlobalVariables.strAPIBank = clsSearch.ClassBankCode;
            clsGlobalVariables.strAPISSLEnable = int.Parse(this.Read("_APISSLEnable", "APISSLEnable"));

            // Split IP And Port
            string[] sIPPort = clsGlobalVariables.strAPIURL.Split(':');
            int iCount = sIPPort.Length;

            for (int x = 0; x < iCount; x++)
            {
                switch (x)
                {
                    case 0:
                        clsGlobalVariables.strAPIServerIP = sIPPort[0].ToString();
                        break;
                    case 1:
                        clsGlobalVariables.strAPIServerPort = sIPPort[1].ToString();
                        break;                    
                }
            }

        }

        public void InitSystemSetting()
        {
            string Path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string FullPath = "";
            string FileName = "";

            FileName = "SystemSetting.ini";
            FullPath = dbFile.sSettingPath + FileName;
            this.INIFile(FullPath);
            
            clsSystemSetting.ClassSystemSplashInterval = int.Parse(this.Read("_SplashInterval", "SplashInterval"));
            clsSystemSetting.ClassSystemSplashTimeOut = int.Parse(this.Read("_SplashTimeOut", "SplashTimeOut"));
            clsSystemSetting.ClassSystemCheckNetLink = int.Parse(this.Read("_CheckNetLink", "CheckNetLink"));
            clsSystemSetting.ClassSystemNetLink = this.Read("_NetLink", "NetLink");
            clsSystemSetting.ClassSystemCheckIPAddress = int.Parse(this.Read("_CheckIPAddress", "CheckIPAddress"));
            clsSystemSetting.ClassSystemIPAddress = this.Read("_IPAddress", "IPAddress");
            clsSystemSetting.ClassSystemConnectionType = this.Read("_ConnectionType", "ConnectionType");
            clsSystemSetting.ClassSystemPromptAPIRequest = int.Parse(this.Read("_PromptAPIRequest", "PromptAPIRequest"));
            clsSystemSetting.ClassSystemPromptAPIResponse = int.Parse(this.Read("_PromptAPIResponse", "PromptAPIResponse"));
            clsSystemSetting.ClassSystemPromptAPIError = int.Parse(this.Read("_PromptAPIError", "PromptAPIError"));
            clsSystemSetting.ClassSystemLogInCheck = int.Parse(this.Read("_LogInCheck", "LogInCheck"));
            clsSystemSetting.ClassSystemImportCheck = int.Parse(this.Read("_ImportCheck", "ImportCheck"));
            clsSystemSetting.ClassSystemMSOffice = this.Read("_MSOffice", "MSOffice");
            clsSystemSetting.ClassSystemShowTaskBar = int.Parse(this.Read("_ShowTaskBar", "ShowTaskBar"));
            clsSystemSetting.ClassSystemExntededMonitor = int.Parse(this.Read("_ExntededMonitor", "ExntededMonitor"));
            clsSystemSetting.ClassSystemAutoPulse = int.Parse(this.Read("_AutoPulse", "AutoPulse"));
            clsSystemSetting.ClassSystemPulseInterval = int.Parse(this.Read("_PulseInterval", "PulseInterval"));
            clsSystemSetting.ClassSystemAutoCheckServer = int.Parse(this.Read("_AutoCheckServer", "AutoCheckServer"));
            clsSystemSetting.ClassSystemCheckServerInterval = int.Parse(this.Read("_CheckServerInterval", "CheckServerInterval"));
            clsSystemSetting.ClassSystemReaderInterval = int.Parse(this.Read("_ReaderInterval", "ReaderInterval"));
            clsSystemSetting.ClassSystemReaderTimeOut = int.Parse(this.Read("_ReaderTimeOut", "ReaderTimeOut"));
            clsSystemSetting.ClassSystemSerialNoMaxLength = int.Parse(this.Read("_SerialNoMaxLength", "SerialNoMaxLength"));
            clsSystemSetting.ClassSystemSkinColor = this.Read("_SkinColor", "SkinColor");
            clsSystemSetting.ClassSystemImportIRFieldCheck = this.Read("_ImportIRFieldCheck", "ImportIRFieldCheck");
            clsSystemSetting.ClassSystemImportTerminalFieldCheck = this.Read("_ImportTerminalFieldCheck", "ImportTerminalFieldCheck");
            clsSystemSetting.ClassSystemImportSIMFieldCheck = this.Read("_ImportSIMFieldCheck", "ImportSIMFieldCheck");
            clsSystemSetting.ClassSystemImportFSRFieldCheck = this.Read("_ImportFSRFieldCheck", "ImportFSRFieldCheck");
            clsSystemSetting.ClassSystemCheckServiceRequest = int.Parse(this.Read("_CheckServiceRequest", "CheckServiceRequest"));
            clsSystemSetting.ClassSystemDeveloperMode = int.Parse(this.Read("_DeveloperMode", "DeveloperMode"));
            clsSystemSetting.ClassSystemPageLimit = int.Parse(this.Read("_PageLimit", "PageLimit"));
            clsSystemSetting.ClassSystemNoOfDayPending = int.Parse(this.Read("_NoOfDayPending", "NoOfDayPending"));
            clsSystemSetting.ClassSystemRecordMinLimit = int.Parse(this.Read("_RecordMinLimit", "RecordMinLimit"));
            clsSystemSetting.ClassGenerateResponseFile = int.Parse(this.Read("_GenerateResponseFile", "GenerateResponseFile"));
            clsSystemSetting.ClassSystemIRNoPrefix = this.Read("_IRNoPrefix", "IRNoPrefix");
            clsSystemSetting.ClassSystemSNLocationID = int.Parse(this.Read("_SNLocationID", "SNLocationID"));
            clsSystemSetting.ClassSystemSNLocation = this.Read("_SNLocation", "SNLocation");
            clsSystemSetting.ClassSystemSNStatusID = int.Parse(this.Read("_SNStatusID", "SNStatusID"));
            clsSystemSetting.ClassSystemSNStatus = this.Read("_SNStatus", "SNStatus");
            clsSystemSetting.ClassApplicationName = this.Read("_ApplicationName", "ApplicationName");
            clsSystemSetting.ClassSystemRequestIDMaxLimit = int.Parse(this.Read("_RequestIDMaxLimit", "RequestIDMaxLimit"));
            clsSystemSetting.ClassSystemComponentMaxLimit = int.Parse(this.Read("_ComponentMaxLimit", "ComponentMaxLimit"));

            //clsSystemSetting.ClassSystemClientID = int.Parse(this.Read("_ClientID", "ClientID"));
            //clsSystemSetting.ClassSystemClientName = this.Read("_ClientName", "ClientName");

            clsSystemSetting.ClassSystemClientID = clsSearch.ClassBankID;
            clsSystemSetting.ClassSystemClientName = clsSearch.ClassBankName.ToUpper();

            clsSystemSetting.ClassSystemJobOrderLimit = int.Parse(this.Read("_JobOrderLimit", "JobOrderLimit"));
            clsSystemSetting.ClassSystemValidRequestType = this.Read("_ValidRequestType", "ValidRequestType");

            clsSystemSetting.ClassSystemEnvironment = this.Read("_Environment", "Environment");

        }

        public void InitODBCSetting()
        {
            string Path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string FullPath = "";
            string FileName = "";

            FileName = "ODBCSetting.ini";
            FullPath = dbFile.sSettingPath + FileName;
            this.INIFile(FullPath);

            clsODBC.ClassODBCServerName = this.Read("_ServerName", "ServerName");
            clsODBC.ClassODBCDSNName = this.Read("_DSNName", "DSNName");
            clsODBC.ClassODBCDriverName = this.Read("_DriverName", "DriverName");
            clsODBC.ClassODBCDatabaseName = this.Read("_DatabaseName", "DatabaseName");
            clsODBC.ClassODBCDescription = this.Read("_Description", "Description");
            clsODBC.ClassODBCUser = this.Read("_User", "User");
            clsODBC.ClassODBCPassword = this.Read("_Password", "Password");
        }

        public void InitFTPSetting()
        {
            string Path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string FullPath = "";
            string FileName = "";

            FileName = "FTPSetting.ini";
            FullPath = dbFile.sSettingPath + FileName;
            this.INIFile(FullPath);

            clsGlobalVariables.strFTPURL = this.Read("_FTPURL", "FTPURL");
            clsGlobalVariables.strFTPPORT = this.Read("_FTPPORT", "FTPPORT");
            clsGlobalVariables.strFTPUploadPath = this.Read("_FTPUploadPath", "FTPUploadPath");
            clsGlobalVariables.strFTPDownloadPath = this.Read("_FTPDownloadPath", "FTPDownloadPath");            
            clsGlobalVariables.strFTPUserName = this.Read("_FTPUserName", "FTPUserName");
            clsGlobalVariables.strFTPPassword = this.Read("_FTPPassword", "FTPPassword");
            clsGlobalVariables.strFTPLocalPath = this.Read("_FTPLocalPath", "FTPLocalPath");
            clsGlobalVariables.strFTPLocalImportPath = this.Read("_FTPLocalImportPath", "FTPLocalImportPath");
            clsGlobalVariables.strFTPLocalExportPath = this.Read("_FTPLocalExportPath", "FTPLocalExportPath");
            clsGlobalVariables.strFTPRemoteFSRPath = this.Read("_FTPRemoteFSRPath", "FTPRemoteFSRPath");
            clsGlobalVariables.strFTPRemoteImagesPath = this.Read("_FTPRemoteImagesPath", "FTPRemoteImagesPath");

            clsGlobalVariables.strFTPRemoteIRPath = this.Read("_FTPRemoteIRPath", "FTPRemoteIRPath");
            clsGlobalVariables.strFTPRemoteSerialPath = this.Read("_FTPRemoteSerialPath", "FTPRemoteSerialPath");
            clsGlobalVariables.strFTPRemoteErmPath = this.Read("_FTPRemoteERMPath", "FTPRemoteERMPath");

            clsGlobalVariables.strLocalSignPath = this.Read("_LocalSignPath", "LocalSignPath");            

        }
        public void InitVersionSetting()
        {
            string Path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string FullPath = "";
            string FileName = "";

            FileName = "VersionSetting.ini";
            FullPath = dbFile.sSettingPath + FileName;
            this.INIFile(FullPath);

            clsGlobalVariables.strCheckUpdate = this.Read("_CheckUpdate", "CheckUpdate");
            clsGlobalVariables.strPublishDate = this.Read("_PublishDate", "PublishDate");
            clsGlobalVariables.strPublishVersion = this.Read("_PublishVersion", "PublishVersion");
            
        }
        // --------------------------------------------------------------------------------------------------------------------------
        // Write
        // --------------------------------------------------------------------------------------------------------------------------
        public void WriteAPISetting()
        {
            string Path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string FullPath = "";
            string FileName = "";

            FileName = "APISetting.ini";
            FullPath = dbFile.sSettingPath + FileName;
            this.INIFile(FullPath);

            this.Write("_APIURL", "APIURL", clsGlobalVariables.strAPIURL);
            this.Write("_APIPath", "APIPath", clsGlobalVariables.strAPIPath);
            this.Write("_APIKeys", "APIKeys", clsGlobalVariables.strAPIKeys);
            this.Write("_APIContentType", "APIContentType", clsGlobalVariables.strAPIContentType);
            this.Write("_APIAuthUser", "APIAuthUser", clsGlobalVariables.strAPIAuthUser);
            this.Write("_APIAuthPassword", "APIAuthPassword", clsGlobalVariables.strAPIAuthPassword);
        }
        public void WriteFTPSetting()
        {
            string Path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string FullPath = "";
            string FileName = "";

            FileName = "FTPSetting.ini";
            FullPath = dbFile.sSettingPath + FileName;
            this.INIFile(FullPath);

            this.Write("_FTPURL", "FTPURL", clsGlobalVariables.strFTPURL);
            this.Write("_FTPPORT", "FTPPORT", clsGlobalVariables.strFTPPORT);
            this.Write("_FTPUploadPath", "FTPUploadPath", clsGlobalVariables.strFTPUploadPath);
            this.Write("_FTPDownloadPath", "FTPDownloadPath", clsGlobalVariables.strFTPDownloadPath);
            this.Write("_FTPUserName", "FTPUserName", clsGlobalVariables.strFTPUserName);
            this.Write("_FTPPassword", "FTPPassword", clsGlobalVariables.strFTPPassword);
            this.Write("_FTPLocalPath", "FTPLocalPath", clsGlobalVariables.strFTPLocalPath);

            this.Write("_LocalSignPath", "LocalSignPath", clsGlobalVariables.strLocalSignPath);
            
        }
        public void WriteDatabaseSetting()
        {
            string Path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string FullPath = "";
            string FileName = "";

            FileName = "DatabaseSetting.ini";
            FullPath = dbFile.sSettingPath + FileName;
            this.INIFile(FullPath);

             this.Write("_Source", "Source", clsGlobalVariables.strSource);
             this.Write("_Server", "Server", clsGlobalVariables.strServer);
             this.Write("_Database", "Database", clsGlobalVariables.strDatabase);
             this.Write("_UserName", "UserName", clsGlobalVariables.strUserName);
             this.Write("_Password", "Password", clsGlobalVariables.strPassword);
             this.Write("_Security", "Security", clsGlobalVariables.strSecurity);
             this.Write("_Port", "Port", clsGlobalVariables.strPort);
        }
        public void WriteODBCSetting()
        {
            string Path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string FullPath = "";
            string FileName = "";

            FileName = "ODBCSetting.ini";
            FullPath = dbFile.sSettingPath + FileName;
            this.INIFile(FullPath);

            this.Write("_ServerName", "ServerName", clsODBC.ClassODBCServerName);
            this.Write("_DSNName", "DSNName", clsODBC.ClassODBCDSNName);
            this.Write("_DriverName", "DriverName", clsODBC.ClassODBCDriverName);
            this.Write("_DatabaseName", "DatabaseName", clsODBC.ClassODBCDatabaseName);
            this.Write("_Description", "Description", clsODBC.ClassODBCDescription);
            this.Write("_User", "User", clsODBC.ClassODBCUser);
            this.Write("_Password", "Password", clsODBC.ClassODBCPassword);
        }
        public void WriteSystemSetting()
        {
            string Path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string FullPath = "";
            string FileName = "";

            FileName = "SystemSetting.ini";
            FullPath = dbFile.sSettingPath + FileName;
            this.INIFile(FullPath);

             this.Write("_SplashInterval", "SplashInterval", clsSystemSetting.ClassSystemSplashInterval.ToString());
             this.Write("_SplashTimeOut", "SplashTimeOut", clsSystemSetting.ClassSystemSplashTimeOut.ToString());
             this.Write("_CheckNetLink", "CheckNetLink", clsSystemSetting.ClassSystemCheckNetLink.ToString());
             this.Write("_NetLink", "NetLink", clsSystemSetting.ClassSystemNetLink);
             this.Write("_CheckIPAddress", "CheckIPAddress", clsSystemSetting.ClassSystemCheckIPAddress.ToString());
             this.Write("_IPAddress", "IPAddress", clsSystemSetting.ClassSystemIPAddress);
             this.Write("_ConnectionType", "ConnectionType", clsSystemSetting.ClassSystemConnectionType);
             this.Write("_PromptAPIRequest", "PromptAPIRequest", clsSystemSetting.ClassSystemPromptAPIRequest.ToString());
             this.Write("_PromptAPIResponse", "PromptAPIResponse", clsSystemSetting.ClassSystemPromptAPIResponse.ToString());
             this.Write("_LogInCheck", "LogInCheck", clsSystemSetting.ClassSystemLogInCheck.ToString());
             this.Write("_ImportCheck", "ImportCheck", clsSystemSetting.ClassSystemImportCheck.ToString());
             this.Write("_ExntededMonitor", "ExntededMonitor", clsSystemSetting.ClassSystemShowTaskBar.ToString());
             this.Write("_MSOffice", "MSOffice", clsSystemSetting.ClassSystemMSOffice);
             this.Write("_SkinColor", "SkinColor", clsSystemSetting.ClassSystemSkinColor);
        }

        public void WriteVersionSetting()
        {
            string Path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string FullPath = "";
            string FileName = "";

            FileName = "VersionSetting.ini";
            FullPath = dbFile.sSettingPath + FileName;
            this.INIFile(FullPath);

            this.Write("_PublishVersion", "PublishVersion", clsGlobalVariables.strPublishVersion);
        }

        public clsDatabase GetDatabaseByCode(string path, string code)
        {
            string json = File.ReadAllText(path);
            DatabaseResponse response = JsonConvert.DeserializeObject<DatabaseResponse>(json);
            return response.Data.FirstOrDefault(d => d.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        private clsBank GetBankByCode(string path, string code)
        {
            string json = File.ReadAllText(path);
            BankResponse response = JsonConvert.DeserializeObject<BankResponse>(json);
            return response.Data.FirstOrDefault(d => d.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        public void InitBasicSystem()
        {
            string Path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string FullPath = "";
            string FileName = "";

            FileName = "SystemSetting.ini";
            FullPath = dbFile.sSettingPath + FileName;
            this.INIFile(FullPath);
            
            clsSystemSetting.ClassSystemEnvironment = this.Read("_Environment", "Environment");

        }
    }
}
