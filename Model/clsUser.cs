using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace MIS
{
    public class clsUser
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _UserID;
        public static int _LogID;
        public static string _UserName;
        public static string _Password;
        public static string _FullName;
        public static string _UserType;       
        
        public static int ClassUserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }

        public static int ClassLogID
        {
            get { return _LogID; }
            set { _LogID = value; }
        }

        public static string ClassUserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        public static string ClassPassword
        {
            get { return _Password; }
            set { _Password = value; }
        }

        public static string ClassUserFullName
        {
            get { return _FullName; }
            set { _FullName = value; }
        }
        public static string ClassUserType
        {
            get { return _UserType; }
            set { _UserType = value; }
        }

        public static string ProcessedBy;
        public static string ClassProcessedBy
        {
            get { return ProcessedBy; }
            set { ProcessedBy = value; }
        }

        public static string ModifiedBy;
        public static string ClassModifiedBy
        {
            get { return ModifiedBy; }
            set { ModifiedBy = value; }
        }

        public static string ProcessedDateTime;
        public static string ClassProcessedDateTime
        {
            get { return ProcessedDateTime; }
            set { ProcessedDateTime = value; }
        }

        public static string ProcessedDate;
        public static string ClassProcessedDate
        {
            get { return ProcessedDate; }
            set { ProcessedDate = value; }
        }

        public static string ProcessedTime;
        public static string ClassProcessedTime
        {
            get { return ProcessedTime; }
            set { ProcessedTime = value; }
        }

        public static string ProcessedContactNo;
        public static string ClassProcessedContactNo
        {
            get { return ProcessedContactNo; }
            set { ProcessedContactNo = value; }
        }

        public static string ProcessedEmail;
        public static string ClassProcessedEmail
        {
            get { return ProcessedEmail; }
            set { ProcessedEmail = value; }
        }

        public static string ModifiedDateTime;
        public static string ClassModifiedDateTime
        {
            get { return ModifiedDateTime; }
            set { ModifiedDateTime = value; }
        }

        public static string ModifiedDate;
        public static string ClassModifiedDate
        {
            get { return ModifiedDate; }
            set { ModifiedDate = value; }
        }

        public static string ModifiedTime;
        public static string ClassModifiedTime
        {
            get { return ModifiedTime; }
            set { ModifiedTime = value; }
        }

        public static string ModifiedContactNo;
        public static string ClassModifiedContactNo
        {
            get { return ModifiedContactNo; }
            set { ModifiedContactNo = value; }
        }

        public static string ModifiedEmail;
        public static string ClassModifiedEmail
        {
            get { return ModifiedEmail; }
            set { ModifiedEmail = value; }
        }

        public static int _ParticularID;
        public static int ClassParticularID
        {
            get { return _ParticularID; }
            set { _ParticularID = value; }
        }

        public static bool _isActive;
        public static bool ClassisActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        public static bool _isAppVersion;
        public static bool ClassisAppVersion
        {
            get { return _isAppVersion; }
            set { _isAppVersion = value; }
        }

        public static string _MD5Password;
        public static string ClassMD5Password
        {
            get { return _MD5Password; }
            set { _MD5Password = value; }
        }
        public static string _MobileTerminalID;
        public static string ClassMobileTerminalID
        {
            get { return _MobileTerminalID; }
            set { _MobileTerminalID = value; }
        }

    }
}
