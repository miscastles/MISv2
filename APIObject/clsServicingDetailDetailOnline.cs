using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class ServicingDetailData
    {
        private int _ServiceNo;
        [JsonProperty("ServiceNo")]
        public int ServiceNo
        {
            get { return _ServiceNo; }
            set { _ServiceNo = value; }
        }

        private int _TAIDNo;
        [JsonProperty("TAIDNo")]
        public int TAIDNo
        {
            get { return _TAIDNo; }
            set { _TAIDNo = value; }
        }

        private int _IRIDNo;
        [JsonProperty("IRIDNo")]
        public int IRIDNo
        {
            get { return _IRIDNo; }
            set { _IRIDNo = value; }
        }

        private int _ClientID;
        [JsonProperty("ClientID")]
        public int ClientID
        {
            get { return _ClientID; }
            set { _ClientID = value; }
        }

        private int _FEID;
        [JsonProperty("FEID")]
        public int FEID
        {
            get { return _FEID; }
            set { _FEID = value; }
        }        

        private int _MerchantID;
        [JsonProperty("MerchantID")]
        public int MerchantID
        {
            get { return _MerchantID; }
            set { _MerchantID = value; }
        }

        private int _TerminalID;
        [JsonProperty("TerminalID")]
        public int TerminalID
        {
            get { return _TerminalID; }
            set { _TerminalID = value; }
        }

        private int _SIMID;
        [JsonProperty("SIMID")]
        public int SIMID
        {
            get { return _SIMID; }
            set { _SIMID = value; }
        }

        private int _DockID;
        [JsonProperty("DockID")]
        public int DockID
        {
            get { return _DockID; }
            set { _DockID = value; }
        }

        private string _TerminalSN;
        [JsonProperty("TerminalSN")]
        public string TerminalSN
        {
            get { return _TerminalSN; }
            set { _TerminalSN = value; }
        }

        private string _SIMSN;
        [JsonProperty("SIMSN")]
        public string SIMSN
        {
            get { return _SIMSN; }
            set { _SIMSN = value; }
        }

        private string _DockSN;
        [JsonProperty("DockSN")]
        public string DockSN
        {
            get { return _DockSN; }
            set { _DockSN = value; }
        }

        private int _CurTerminalSNStatus;
        [JsonProperty("CurTerminalSNStatus")]
        public int CurTerminalSNStatus
        {
            get { return _CurTerminalSNStatus; }
            set { _CurTerminalSNStatus = value; }
        }

        private int _CurSIMSNStatus;
        [JsonProperty("CurSIMSNStatus")]
        public int CurSIMSNStatus
        {
            get { return _CurSIMSNStatus; }
            set { _CurSIMSNStatus = value; }
        }

        private int _CurDockSNStatus;
        [JsonProperty("CurDockSNStatus")]
        public int CurDockSNStatus
        {
            get { return _CurDockSNStatus; }
            set { _CurDockSNStatus = value; }
        }

        private string _CurTerminalSNStatusDescription;
        [JsonProperty("CurTerminalSNStatusDescription")]
        public string CurTerminalSNStatusDescription
        {
            get { return _CurTerminalSNStatusDescription; }
            set { _CurTerminalSNStatusDescription = value; }
        }

        private string _CurSIMSNStatusDescription;
        [JsonProperty("CurSIMSNStatusDescription")]
        public string CurSIMSNStatusDescription
        {
            get { return _CurSIMSNStatusDescription; }
            set { _CurSIMSNStatusDescription = value; }
        }

        private string _CurDockSNStatusDescription;
        [JsonProperty("CurDockSNStatusDescription")]
        public string CurDockSNStatusDescription
        {
            get { return _CurDockSNStatusDescription; }
            set { _CurDockSNStatusDescription = value; }
        }
        
        private int _ReplaceTerminalID;
        [JsonProperty("ReplaceTerminalID")]
        public int ReplaceTerminalID
        {
            get { return _ReplaceTerminalID; }
            set { _ReplaceTerminalID = value; }
        }

        private int _ReplaceSIMID;
        [JsonProperty("ReplaceSIMID")]
        public int ReplaceSIMID
        {
            get { return _ReplaceSIMID; }
            set { _ReplaceSIMID = value; }
        }

        private int _ReplaceDockID;
        [JsonProperty("ReplaceDockID")]
        public int ReplaceDockID
        {
            get { return _ReplaceDockID; }
            set { _ReplaceDockID = value; }
        }
        
        private int _RepTerminalSNStatus;
        [JsonProperty("RepTerminalSNStatus")]
        public int RepTerminalSNStatus
        {
            get { return _RepTerminalSNStatus; }
            set { _RepTerminalSNStatus = value; }
        }

        private int _RepSIMSNStatus;
        [JsonProperty("RepSIMSNStatus")]
        public int RepSIMSNStatus
        {
            get { return _RepSIMSNStatus; }
            set { _RepSIMSNStatus = value; }
        }

        private int _RepDockSNStatus;
        [JsonProperty("RepDockSNStatus")]
        public int RepDockSNStatus
        {
            get { return _RepDockSNStatus; }
            set { _RepDockSNStatus = value; }
        }

        private string _RepTerminalSNStatusDescription;
        [JsonProperty("RepTerminalSNStatusDescription")]
        public string RepTerminalSNStatusDescription
        {
            get { return _RepTerminalSNStatusDescription; }
            set { _RepTerminalSNStatusDescription = value; }
        }

        private string _RepSIMSNStatusDescription;
        [JsonProperty("RepSIMSNStatusDescription")]
        public string RepSIMSNStatusDescription
        {
            get { return _RepSIMSNStatusDescription; }
            set { _RepSIMSNStatusDescription = value; }
        }

        private string _RepDockSNStatusDescription;
        [JsonProperty("RepDockSNStatusDescription")]
        public string RepDockSNStatusDescription
        {
            get { return _RepDockSNStatusDescription; }
            set { _RepDockSNStatusDescription = value; }
        }

        private string _CounterNo;
        [JsonProperty("CounterNo")]
        public string CounterNo
        {
            get { return _CounterNo; }
            set { _CounterNo = value; }
        }

        private string _IRNo;
        [JsonProperty("IRNo")]
        public string IRNo
        {
            get { return _IRNo; }
            set { _IRNo = value; }
        }

        private string _RequestNo;
        [JsonProperty("RequestNo")]
        public string RequestNo
        {
            get { return _RequestNo; }
            set { _RequestNo = value; }
        }

        private string _ServiceDateTime;
        [JsonProperty("ServiceDateTime")]
        public string ServiceDateTime
        {
            get { return _ServiceDateTime; }
            set { _ServiceDateTime = value; }
        }
        
        private string _ServiceDate;
        [JsonProperty("ServiceDate")]
        public string ServiceDate
        {
            get { return _ServiceDate; }
            set { _ServiceDate = value; }
        }
        
        private string _ServiceTime;
        [JsonProperty("ServiceTime")]
        public string ServiceTime
        {
            get { return _ServiceTime; }
            set { _ServiceTime = value; }
        }

        private string _CustomerName;
        [JsonProperty("CustomerName")]
        public string CustomerName
        {
            get { return _CustomerName; }
            set { _CustomerName = value; }
        }

        private string _CustomerContactNo;
        [JsonProperty("CustomerContactNo")]
        public string CustomerContactNo
        {
            get { return _CustomerContactNo; }
            set { _CustomerContactNo = value; }
        }

        private string _Remarks;
        [JsonProperty("Remarks")]
        public string Remarks
        {
            get { return _Remarks; }
            set { _Remarks = value; }
        }

        private string _ServiceReqDate;
        [JsonProperty("ServiceReqDate")]
        public string ServiceReqDate
        {
            get { return _ServiceReqDate; }
            set { _ServiceReqDate = value; }
        }

        private string _ServiceReqTime;
        [JsonProperty("ServiceReqTime")]
        public string ServiceReqTime
        {
            get { return _ServiceReqTime; }
            set { _ServiceReqTime = value; }
        }

        private string _LastServiceRequest;
        [JsonProperty("LastServiceRequest")]
        public string LastServiceRequest
        {
            get { return _LastServiceRequest; }
            set { _LastServiceRequest = value; }
        }

        private string _NewServiceRequest;
        [JsonProperty("NewServiceRequest")]
        public string NewServiceRequest
        {
            get { return _NewServiceRequest; }
            set { _NewServiceRequest = value; }
        }

        private string _ReplaceTerminalSN;
        [JsonProperty("ReplaceTerminalSN")]
        public string ReplaceTerminalSN
        {
            get { return _ReplaceTerminalSN; }
            set { _ReplaceTerminalSN = value; }
        }

        private string _ReplaceSIMSN;
        [JsonProperty("ReplaceSIMSN")]
        public string ReplaceSIMSN
        {
            get { return _ReplaceSIMSN; }
            set { _ReplaceSIMSN = value; }
        }

        private string _ReplaceDockSN;
        [JsonProperty("ReplaceDockSN")]
        public string ReplaceDockSN
        {
            get { return _ReplaceDockSN; }
            set { _ReplaceDockSN = value; }
        }

        private int _JobType;
        [JsonProperty("JobType")]
        public int JobType
        {
            get { return _JobType; }
            set { _JobType = value; }
        }

        private string _JobTypeDescription;
        [JsonProperty("JobTypeDescription")]
        public string JobTypeDescription
        {
            get { return _JobTypeDescription; }
            set { _JobTypeDescription = value; }
        }

        private string _JobTypeSubDescription;
        [JsonProperty("JobTypeSubDescription")]
        public string JobTypeSubDescription
        {
            get { return _JobTypeSubDescription; }
            set { _JobTypeSubDescription = value; }
        }

        private string _JobTypeStatusDescription;
        [JsonProperty("JobTypeStatusDescription")]
        public string JobTypeStatusDescription
        {
            get { return _JobTypeStatusDescription; }
            set { _JobTypeStatusDescription = value; }
        }

        private int _ReasonID;
        [JsonProperty("ReasonID")]
        public int ReasonID
        {
            get { return _ReasonID; }
            set { _ReasonID = value; }
        }

        private string _ReasonCode;
        [JsonProperty("ReasonCode")]
        public string ReasonCode
        {
            get { return _ReasonCode; }
            set { _ReasonCode = value; }
        }

        private string _ReasonDescription;
        [JsonProperty("ReasonDescription")]
        public string ReasonDescription
        {
            get { return _ReasonDescription; }
            set { _ReasonDescription = value; }
        }

        private int _ServiceStatus;
        [JsonProperty("ServiceStatus")]
        public int ServiceStatus
        {
            get { return _ServiceStatus; }
            set { _ServiceStatus = value; }
        }

        private string _ServiceStatusDescription;
        [JsonProperty("ServiceStatusDescription")]
        public string ServiceStatusDescription
        {
            get { return _ServiceStatusDescription; }
            set { _ServiceStatusDescription = value; }
        }

        private int _ExpensesID;
        [JsonProperty("ExpensesID")]
        public int ExpensesID
        {
            get { return _ExpensesID; }
            set { _ExpensesID = value; }
        }

        private double _TExpenses;
        [JsonProperty("TExpenses")]
        public double TExpenses
        {
            get { return _TExpenses; }
            set { _TExpenses = value; }
        }

        private string _ReferenceNo;
        [JsonProperty("ReferenceNo")]
        public string ReferenceNo
        {
            get { return _ReferenceNo; }
            set { _ReferenceNo = value; }
        }

        private string _TID;
        [JsonProperty("TID")]
        public string TID
        {
            get { return _TID; }
            set { _TID = value; }
        }

        private string _MID;
        [JsonProperty("MID")]
        public string MID
        {
            get { return _MID; }
            set { _MID = value; }
        }

        private string _Region;
        [JsonProperty("Region")]
        public string Region
        {
            get { return _Region; }
            set { _Region = value; }
        }

        private string _Province;
        [JsonProperty("Province")]
        public string Province
        {
            get { return _Province; }
            set { _Province = value; }
        }

        private string _ServiceCode;
        [JsonProperty("ServiceCode")]
        public string ServiceCode
        {
            get { return _ServiceCode; }
            set { _ServiceCode = value; }
        }

        private string _TerminalStatusDescription;
        [JsonProperty("TerminalStatusDescription")]
        public string TerminalStatusDescription
        {
            get { return _TerminalStatusDescription; }
            set { _TerminalStatusDescription = value; }
        }

        private string _Type;
        [JsonProperty("Type")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        private string _Model;
        [JsonProperty("Model")]
        public string Model
        {
            get { return _Model; }
            set { _Model = value; }
        }

        private string _Brand;
        [JsonProperty("Brand")]
        public string Brand
        {
            get { return _Brand; }
            set { _Brand = value; }
        }

        private string _IRDate;
        [JsonProperty("IRDate")]
        public string IRDate
        {
            get { return _IRDate; }
            set { _IRDate = value; }
        }

        private string _InstallationDate;
        [JsonProperty("InstallationDate")]
        public string InstallationDate
        {
            get { return _InstallationDate; }
            set { _InstallationDate = value; }
        }

        private string _MerchantName;
        [JsonProperty("MerchantName")]
        public string MerchantName
        {
            get { return _MerchantName; }
            set { _MerchantName = value; }
        }

        private string _IRStatusDescription;
        [JsonProperty("IRStatusDescription")]
        public string IRStatusDescription
        {
            get { return _IRStatusDescription; }
            set { _IRStatusDescription = value; }
        }

        private string _ClientName;
        [JsonProperty("ClientName")]
        public string ClientName
        {
            get { return _ClientName; }
            set { _ClientName = value; }
        }

        private string _FEName;
        [JsonProperty("FEName")]
        public string FEName
        {
            get { return _FEName; }
            set { _FEName = value; }
        }

        private string _ActionMade;
        [JsonProperty("ActionMade")]
        public string ActionMade
        {
            get { return _ActionMade; }
            set { _ActionMade = value; }
        }

        private string _TimeArrived;
        [JsonProperty("TimeArrived")]
        public string TimeArrived
        {
            get { return _TimeArrived; }
            set { _TimeArrived = value; }
        }

        private string _TimeStart;
        [JsonProperty("TimeStart")]
        public string TimeStart
        {
            get { return _TimeStart; }
            set { _TimeStart = value; }
        }

        private string _FSRDate;
        [JsonProperty("FSRDate")]
        public string FSRDate
        {
            get { return _FSRDate; }
            set { _FSRDate = value; }
        }

        private string _FSRTime;
        [JsonProperty("FSRTime")]
        public string FSRTime
        {
            get { return _FSRTime; }
            set { _FSRTime = value; }
        }

        private string _TimeEnd;
        [JsonProperty("TimeEnd")]
        public string TimeEnd
        {
            get { return _TimeEnd; }
            set { _TimeEnd = value; }
        }

        private string _SIMCarrier;
        [JsonProperty("SIMCarrier")]
        public string SIMCarrier
        {
            get { return _SIMCarrier; }
            set { _SIMCarrier = value; }
        }

        private string _ProblemReported;
        [JsonProperty("ProblemReported")]
        public string ProblemReported
        {
            get { return _ProblemReported; }
            set { _ProblemReported = value; }
        }

        private string _ActualProblemReported;
        [JsonProperty("ActualProblemReported")]
        public string ActualProblemReported
        {
            get { return _ActualProblemReported; }
            set { _ActualProblemReported = value; }
        }

        private string _ActionTaken;
        [JsonProperty("ActionTaken")]
        public string ActionTaken
        {
            get { return _ActionTaken; }
            set { _ActionTaken = value; }
        }

        private string _AnyComments;
        [JsonProperty("AnyComments")]
        public string AnyComments
        {
            get { return _AnyComments; }
            set { _AnyComments = value; }
        }

        private string _MerchantRepresentative;
        [JsonProperty("MerchantRepresentative")]
        public string MerchantRepresentative
        {
            get { return _MerchantRepresentative; }
            set { _MerchantRepresentative = value; }
        }

        private string _MerchantContactNo;
        [JsonProperty("MerchantContactNo")]
        public string MerchantContactNo
        {
            get { return _MerchantContactNo; }
            set { _MerchantContactNo = value; }
        }

        private int _RegionID;
        [JsonProperty("RegionID")]
        public int RegionID
        {
            get { return _RegionID; }
            set { _RegionID = value; }
        }

        private int _RegionType;
        [JsonProperty("RegionType")]
        public int RegionType
        {
            get { return _RegionType; }
            set { _RegionType = value; }
        }

        private string _AppVersion;
        [JsonProperty("AppVersion")]
        public string AppVersion
        {
            get { return _AppVersion; }
            set { _AppVersion = value; }
        }

        private string _AppCRC;
        [JsonProperty("AppCRC")]
        public string AppCRC
        {
            get { return _AppCRC; }
            set { _AppCRC = value; }
        }

        private int _FSRNo;
        [JsonProperty("FSRNo")]
        public int FSRNo
        {
            get { return _FSRNo; }
            set { _FSRNo = value; }
        }

        private string _PrimaryNum;
        [JsonProperty("PrimaryNum")]
        public string PrimaryNum
        {
            get { return _PrimaryNum; }
            set { _PrimaryNum = value; }
        }

        private string _SecondaryNum;
        [JsonProperty("SecondaryNum")]
        public string SecondaryNum
        {
            get { return _SecondaryNum; }
            set { _SecondaryNum = value; }
        }

        private string _ProcessedBy;
        [JsonProperty("ProcessedBy")]
        public string ProcessedBy
        {
            get { return _ProcessedBy; }
            set { _ProcessedBy = value; }
        }

        private string _ProcessedDateTime;
        [JsonProperty("ProcessedDateTime")]
        public string ProcessedDateTime
        {
            get { return _ProcessedDateTime; }
            set { _ProcessedDateTime = value; }
        }

        private string _ModifiedBy;
        [JsonProperty("ModifiedBy")]
        public string ModifiedBy
        {
            get { return _ModifiedBy; }
            set { _ModifiedBy = value; }
        }

        private string _ModifiedDateTime;
        [JsonProperty("ModifiedDateTime")]
        public string ModifiedDateTime
        {
            get { return _ModifiedDateTime; }
            set { _ModifiedDateTime = value; }
        }

        private string _ServiceJobTypeDescription;
        [JsonProperty("ServiceJobTypeDescription")]
        public string ServiceJobTypeDescription
        {
            get { return _ServiceJobTypeDescription; }
            set { _ServiceJobTypeDescription = value; }
        }

    }
    public class ServicingDetailOnline
    {

        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<ServicingDetailData> data { get; set; }
    }
}
