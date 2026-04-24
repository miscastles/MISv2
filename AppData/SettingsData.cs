using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.AppData
{
    class SettingsData
    {
        public class ApiSettings
        {
            public string Host { get; set; }
            public string Path { get; set; }
            public string Keys { get; set; }
            public string ContentType { get; set; }
            public string AuthUser { get; set; }
            public string AuthPassword { get; set; }
            public string IPAddress { get; set; }
            public string ImagesPath { get; set; }
        }

        public class DatabaseSettings
        {
            public string Security { get; set; }
            public string Server { get; set; }
            public string Port { get; set; }
            public string Database { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string SSLMode { get; set; }
            public string Source { get; set; }
        }

        public class FtpSettings
        {
            public int Enabled { get; set; }
            public string Url { get; set; }
            public string Port { get; set; }
            public string UploadPath { get; set; }
            public string DownloadPath { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string LocalPath { get; set; }
            public string RemotePath { get; set; }
            public string LocalImportPath { get; set; }
            public string LocalExportPath { get; set; }
            public string RemoteDirectoryPath { get; set; }
            public string LocalDirectoryPath { get; set; }
        }

        public class SystemSettings
        {
            public int PromptApiRequest { get; set; }
            public int PromptApiResponse { get; set; }
            public int GenResponseFile { get; set; }

            public int CheckNetLink { get; set; }
            public string NetLink { get; set; }

            public string DatabaseServer { get; set; }
            public string ConnType { get; set; }

            public int CheckIpAddress { get; set; }
            public string IpAddress { get; set; }

            public int SplashInterval { get; set; }
            public int SplashTimeOut { get; set; }

            public int LogInCheck { get; set; }
            public int ImportCheck { get; set; }
            public string MSOffice { get; set; }
            public int ShowTaskBar { get; set; }
            public int ExntededMonitor { get; set; }
            public int AutoPulse { get; set; }
            public int PulseInterval { get; set; }
            public int AutoCheckServer { get; set; }
            public int CheckServerInterval { get; set; }
            public int ReaderInterval { get; set; }
            public int ReaderTimeOut { get; set; }
            public int SerialNoMaxLength { get; set; }
            public string SkinColor { get; set; }
            public int SystemID { get; set; }
            public int CheckUpdate { get; set; }
            public string PublishDate { get; set; }
            public string LocalPublishVersion { get; set; }
            public string PublishVersion { get; set; }

            public int CheckServiceRequest { get; set; }
            public int DeveloperMode { get; set; }
            public int NoOfDayPending { get; set; }

            public int PageLimit { get; set; }
            public int RecordMinLimit { get; set; }

            public int RecordDisplayLimit { get; set; }

        }
    }
}
