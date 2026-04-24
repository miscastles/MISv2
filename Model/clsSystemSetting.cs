using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    class clsSystemSetting
    {
        public clsINI dbSystem;

        public static bool RecordFound;
        public static int _SplashInterval;
        public static int _SplashTimeOut;
        public static int _CheckNetLink;
        public static string _NetLink;
        public static int _CheckIPAddress;
        public static string _IPAddress;
        public static string _ConnectionType;
        public static int _PromptAPIRequest;
        public static int _PromptAPIResponse;
        public static int _LogInCheck;
        public static int _ImportCheck;
        public static string _MSOffice;
        public static int _ShowTaskBar;
        public static int _ExntededMonitor;
        public static int _AutoPulse;
        public static int _PulseInterval;
        public static int _AutoCheckServer;
        public static int _CheckServerInterval;
        public static int _ReaderInterval;
        public static int _ReaderTimeOut;
        public static int _SerialNoMaxLength;
        public static string _SkinColor;

        public static int _SysID;
        public static int _CheckUpdate;
        public static string _PublishDate;
        public static string _LocalPublishVersion;
        public static string _PublishVersion;
        public static string _ImportIRFieldCheck;
        public static string _ImportTerminalFieldCheck;
        public static string _ImportSIMFieldCheck;
        public static string _ImportFSRFieldCheck;
        public static int _CheckServiceRequest;

        public static int _GenerateResponseFile;

        public void GetSystemSetting()
        {
            dbSystem = new clsINI();
            dbSystem.InitSystemSetting();
        }
        public static int ClassSystemSplashInterval
        {
            get { return _SplashInterval; }
            set { _SplashInterval = value; }
        }

        public static int ClassSystemSplashTimeOut
        {
            get { return _SplashTimeOut; }
            set { _SplashTimeOut = value; }
        }

        public static int ClassSystemCheckNetLink
        {
            get { return _CheckNetLink; }
            set { _CheckNetLink = value; }
        }
        public static string ClassSystemNetLink
        {
            get { return _NetLink; }
            set { _NetLink = value; }
        }
        public static int ClassSystemCheckIPAddress
        {
            get { return _CheckIPAddress; }
            set { _CheckIPAddress = value; }
        }
        public static string ClassSystemIPAddress
        {
            get { return _IPAddress; }
            set { _IPAddress = value; }
        }

        public static string ClassSystemConnectionType
        {
            get { return _ConnectionType; }
            set { _ConnectionType = value; }
        }

        public static int ClassSystemPromptAPIRequest
        {
            get { return _PromptAPIRequest; }
            set { _PromptAPIRequest = value; }
        }

        public static int ClassSystemPromptAPIResponse
        {
            get { return _PromptAPIResponse; }
            set { _PromptAPIResponse = value; }
        }
        public static int ClassSystemLogInCheck
        {
            get { return _LogInCheck; }
            set { _LogInCheck = value; }
        }
        public static int ClassSystemImportCheck
        {
            get { return _ImportCheck; }
            set { _ImportCheck = value; }
        }
        public static string ClassSystemMSOffice
        {
            get { return _MSOffice; }
            set { _MSOffice = value; }
        }
        public static int ClassSystemShowTaskBar
        {
            get { return _ShowTaskBar; }
            set { _ShowTaskBar = value; }
        }
        public static int ClassSystemExntededMonitor
        {
            get { return _ExntededMonitor; }
            set { _ExntededMonitor = value; }
        }
        public static int ClassSystemAutoPulse
        {
            get { return _AutoPulse; }
            set { _AutoPulse = value; }
        }
        public static int ClassSystemPulseInterval
        {
            get { return _PulseInterval; }
            set { _PulseInterval = value; }
        }
        public static int ClassSystemAutoCheckServer
        {
            get { return _AutoCheckServer; }
            set { _AutoCheckServer = value; }
        }
        public static int ClassSystemCheckServerInterval
        {
            get { return _CheckServerInterval; }
            set { _CheckServerInterval = value; }
        }
        public static int ClassSystemReaderInterval
        {
            get { return _ReaderInterval; }
            set { _ReaderInterval = value; }
        }
        public static int ClassSystemReaderTimeOut
        {
            get { return _ReaderTimeOut; }
            set { _ReaderTimeOut = value; }
        }
        public static int ClassSystemSerialNoMaxLength
        {
            get { return _SerialNoMaxLength; }
            set { _SerialNoMaxLength = value; }
        }
        public static string ClassSystemSkinColor
        {
            get { return _SkinColor; }
            set { _SkinColor = value; }
        }
        public static int ClassSystemID
        {
            get { return _SysID; }
            set { _SysID = value; }
        }
        public static int ClassSystemCheckUpdate
        {
            get { return _CheckUpdate; }
            set { _CheckUpdate = value; }
        }
        public static string ClassSystemPublishDate
        {
            get { return _PublishDate; }
            set { _PublishDate = value; }
        }
        public static string ClassSystemLocalPublishVersion
        {
            get { return _LocalPublishVersion; }
            set { _LocalPublishVersion = value; }
        }
        public static string ClassSystemPublishVersion
        {
            get { return _PublishVersion; }
            set { _PublishVersion = value; }
        }
        public static string ClassSystemImportIRFieldCheck
        {
            get { return _ImportIRFieldCheck; }
            set { _ImportIRFieldCheck = value; }
        }
        public static string ClassSystemImportTerminalFieldCheck
        {
            get { return _ImportTerminalFieldCheck; }
            set { _ImportTerminalFieldCheck = value; }
        }
        public static string ClassSystemImportSIMFieldCheck
        {
            get { return _ImportSIMFieldCheck; }
            set { _ImportSIMFieldCheck = value; }
        }
        public static string ClassSystemImportFSRFieldCheck
        {
            get { return _ImportFSRFieldCheck; }
            set { _ImportFSRFieldCheck = value; }
        }
        public static int ClassSystemCheckServiceRequest
        {
            get { return _CheckServiceRequest; }
            set { _CheckServiceRequest = value; }
        }
        public static int _DeveloperMode;
        public static int ClassSystemDeveloperMode
        {
            get { return _DeveloperMode; }
            set { _DeveloperMode = value; }
        }

        public static int _PageLimit;
        public static int ClassSystemPageLimit
        {
            get { return _PageLimit; }
            set { _PageLimit = value; }
        }

        public static int _NoOfDayPending;
        public static int ClassSystemNoOfDayPending
        {
            get { return _NoOfDayPending; }
            set { _NoOfDayPending = value; }
        }

        public static int _RecordMinLimit;
        public static int ClassSystemRecordMinLimit
        {
            get { return _RecordMinLimit; }
            set { _RecordMinLimit = value; }
        }
        public static int ClassGenerateResponseFile
        {
            get { return _GenerateResponseFile; }
            set { _GenerateResponseFile = value; }
        }

        public static string _IRNoPrefix;
        public static string ClassSystemIRNoPrefix
        {
            get { return _IRNoPrefix; }
            set { _IRNoPrefix = value; }
        }

        public static int _SNLocationID;
        public static int ClassSystemSNLocationID
        {
            get { return _SNLocationID; }
            set { _SNLocationID = value; }
        }

        public static string _SNLocation;
        public static string ClassSystemSNLocation
        {
            get { return _SNLocation; }
            set { _SNLocation = value; }
        }

        public static int _SNStatusID;
        public static int ClassSystemSNStatusID
        {
            get { return _SNStatusID; }
            set { _SNStatusID = value; }
        }

        public static string _SNStatus;
        public static string ClassSystemSNStatus
        {
            get { return _SNStatus; }
            set { _SNStatus = value; }
        }
        public static string _ApplicationName;
        public static string ClassApplicationName
        {
            get { return _ApplicationName; }
            set { _ApplicationName = value; }
        }
        public static int _RequestIDMaxLimit;
        public static int ClassSystemRequestIDMaxLimit
        {
            get { return _RequestIDMaxLimit; }
            set { _RequestIDMaxLimit = value; }
        }
        public static int _ComponentMaxLimit;
        public static int ClassSystemComponentMaxLimit
        {
            get { return _ComponentMaxLimit; }
            set { _ComponentMaxLimit = value; }
        }
        
        public static int _ClientID;
        public static int ClassSystemClientID
        {
            get { return _ClientID; }
            set { _ClientID = value; }
        }
        public static string _ClientName;
        public static string ClassSystemClientName
        {
            get { return _ClientName; }
            set { _ClientName = value; }
        }
        
        public static int _JobOrderLimit;
        public static int ClassSystemJobOrderLimit
        {
            get { return _JobOrderLimit; }
            set { _JobOrderLimit = value; }
        }
        public static string _ValidRequestType;
        public static string ClassSystemValidRequestType
        {
            get { return _ValidRequestType; }
            set { _ValidRequestType = value; }
        }
        public static int _PromptAPIError;
        public static int ClassSystemPromptAPIError
        {
            get { return _PromptAPIError; }
            set { _PromptAPIError = value; }
        }
        public static string _Environment;
        public static string ClassSystemEnvironment
        {
            get { return _Environment; }
            set { _Environment = value; }
        }

    }
}
