using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class UserData
    {

        private int _UserID;
        [JsonProperty("UserID")]
        public int UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }

        private string _UserName;
        [JsonProperty("UserName")]
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        private string _FullName;
        [JsonProperty("FullName")]
        public string FullName
        {
            get { return _FullName; }
            set { _FullName = value; }
        }

        private string _Password;
        [JsonProperty("Password")]
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        private string _UserType;
        [JsonProperty("UserType")]
        public string UserType
        {
            get { return _UserType; }
            set { _UserType = value; }
        }

        private int _LogID;
        [JsonProperty("LogID")]
        public int LogID
        {
            get { return _LogID; }
            set { _LogID = value; }
        }

        private int _SessionStatus;
        [JsonProperty("SessionStatus")]
        public int SessionStatus
        {
            get { return _SessionStatus; }
            set { _SessionStatus = value; }
        }

        private string _ComputerIP;
        [JsonProperty("ComputerIP")]
        public string ComputerIP
        {
            get { return _ComputerIP; }
            set { _ComputerIP = value; }
        }

        private string _ComputerName;
        [JsonProperty("ComputerName")]
        public string ComputerName
        {
            get { return _ComputerName; }
            set { _ComputerName = value; }
        }

        private string _SessionStatusDescription;
        [JsonProperty("SessionStatusDescription")]
        public string SessionStatusDescription
        {
            get { return _SessionStatusDescription; }
            set { _SessionStatusDescription = value; }
        }

        private string _LogInDate;
        [JsonProperty("LogInDate")]
        public string LogInDate
        {
            get { return _LogInDate; }
            set { _LogInDate = value; }
        }

        private string _LogInTime;
        [JsonProperty("LogInTime")]
        public string LogInTime
        {
            get { return _LogInTime; }
            set { _LogInTime = value; }
        }

        private string _LogOutDate;
        [JsonProperty("LogOutDate")]
        public string LogOutDate
        {
            get { return _LogOutDate; }
            set { _LogOutDate = value; }
        }

        private string _LogOutTime;
        [JsonProperty("LogOutTime")]
        public string LogOutTime
        {
            get { return _LogOutTime; }
            set { _LogOutTime = value; }
        }

        private string _PublishVersion;
        [JsonProperty("PublishVersion")]
        public string PublishVersion
        {
            get { return _PublishVersion; }
            set { _PublishVersion = value; }
        }

        private int _isActive;
        [JsonProperty("isActive")]
        public int isActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        private int _isAppVersion;
        [JsonProperty("isAppVersion")]
        public int isAppVersion
        {
            get { return _isAppVersion; }
            set { _isAppVersion = value; }
        }

        private int _ParticularID;
        [JsonProperty("ParticularID")]
        public int ParticularID
        {
            get { return _ParticularID; }
            set { _ParticularID = value; }
        }

        private string _MD5Password;
        [JsonProperty("MD5Password")]
        public string MD5Password
        {
            get { return _MD5Password; }
            set { _MD5Password = value; }
        }

        private string _MobileID;
        [JsonProperty("MobileID")]
        public string MobileID
        {
            get { return _MobileID; }
            set { _MobileID = value; }
        }

        private string _MobileTerminalID;
        [JsonProperty("MobileTerminalID")]
        public string MobileTerminalID
        {
            get { return _MobileTerminalID; }
            set { _MobileTerminalID = value; }
        }

        private string _MobileTerminalName;
        [JsonProperty("MobileTerminalName")]
        public string MobileTerminalName
        {
            get { return _MobileTerminalName; }
            set { _MobileTerminalName = value; }
        }

    }

    public class UserDetailOnline
        {

            [JsonProperty("resp_code")]
            public string error_code { get; set; }

            [JsonProperty("message")]
            public string message { get; set; }

            [JsonProperty("data")]
            public List<UserData> data { get; set; }
        }
    
}
