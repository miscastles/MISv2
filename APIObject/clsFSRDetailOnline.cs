using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class FSRData
    {
        private int _FSRID;
        [JsonProperty("FSRID")]
        public int FSRID
        {
            get { return _FSRID; }
            set { _FSRID = value; }
        }

        private int _FSRNo;
        [JsonProperty("FSRNo")]
        public int FSRNo
        {
            get { return _FSRNo; }
            set { _FSRNo = value; }
        }

        private string _No;
        [JsonProperty("No")]
        public string No
        {
            get { return _No; }
            set { _No = value; }
        }

        private string _Merchant;
        [JsonProperty("Merchant")]
        public string Merchant
        {
            get { return _Merchant; }
            set { _Merchant = value; }
        }

        private string _MID;
        [JsonProperty("MID")]
        public string MID
        {
            get { return _MID; }
            set { _MID = value; }
        }

        private string _TID;
        [JsonProperty("TID")]
        public string TID
        {
            get { return _TID; }
            set { _TID = value; }
        }

        private string _InvoiceNo;
        [JsonProperty("InvoiceNo")]
        public string InvoiceNo
        {
            get { return _InvoiceNo; }
            set { _InvoiceNo = value; }
        }

        private string _BatchNo;
        [JsonProperty("BatchNo")]
        public string BatchNo
        {
            get { return _BatchNo; }
            set { _BatchNo = value; }
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

        private string _FSR;
        [JsonProperty("FSR")]
        public string FSR
        {
            get { return _FSR; }
            set { _FSR = value; }
        }

        private string _ServiceTypeDescription;
        [JsonProperty("ServiceTypeDescription")]
        public string ServiceTypeDescription
        {
            get { return _ServiceTypeDescription; }
            set { _ServiceTypeDescription = value; }
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

        private string _TxnAmt;
        [JsonProperty("TxnAmt")]
        public string TxnAmt
        {
            get { return _TxnAmt; }
            set { _TxnAmt = value; }
        }

        private string _TimeEnd;
        [JsonProperty("TimeEnd")]
        public string TimeEnd
        {
            get { return _TimeEnd; }
            set { _TimeEnd = value; }
        }

        private string _TerminalSN;
        [JsonProperty("TerminalSN")]
        public string TerminalSN
        {
            get { return _TerminalSN; }
            set { _TerminalSN = value; }
        }

        private string _MerchantContactNo;
        [JsonProperty("MerchantContactNo")]
        public string MerchantContactNo
        {
            get { return _MerchantContactNo; }
            set { _MerchantContactNo = value; }
        }

        private string _MerchantRepresentative;
        [JsonProperty("MerchantRepresentative")]
        public string MerchantRepresentative
        {
            get { return _MerchantRepresentative; }
            set { _MerchantRepresentative = value; }
        }

        private string _FEName;
        [JsonProperty("FEName")]
        public string FEName
        {
            get { return _FEName; }
            set { _FEName = value; }
        }

        private string _SerialNo;
        [JsonProperty("SerialNo")]
        public string SerialNo
        {
            get { return _SerialNo; }
            set { _SerialNo = value; }
        }

        private string _FSRStatusDescription;
        [JsonProperty("FSRStatusDescription")]
        public string FSRStatusDescription
        {
            get { return _FSRStatusDescription; }
            set { _FSRStatusDescription = value; }
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

        private string _Remarks;
        [JsonProperty("Remarks")]
        public string Remarks
        {
            get { return _Remarks; }
            set { _Remarks = value; }
        }

        private string _ProcessType;
        [JsonProperty("ProcessType")]
        public string ProcessType
        {
            get { return _ProcessType; }
            set { _ProcessType = value; }
        }

        private int _IRIDNo;
        [JsonProperty("IRIDNo")]
        public int IRIDNo
        {
            get { return _IRIDNo; }
            set { _IRIDNo = value; }
        }

        private int _TAIDNo;
        [JsonProperty("TAIDNo")]
        public int TAIDNo
        {
            get { return _TAIDNo; }
            set { _TAIDNo = value; }
        }

        private int _ClientID;
        [JsonProperty("ClientID")]
        public int ClientID
        {
            get { return _ClientID; }
            set { _ClientID = value; }
        }

        private string _IRNo;
        [JsonProperty("IRNo")]
        public string IRNo
        {
            get { return _IRNo; }
            set { _IRNo = value; }
        }

        private string _ClientName;
        [JsonProperty("ClientName")]
        public string ClientName
        {
            get { return _ClientName; }
            set { _ClientName = value; }
        }

        private string _SIMSN;
        [JsonProperty("SIMSN")]
        public string SIMSN
        {
            get { return _SIMSN; }
            set { _SIMSN = value; }
        }

        private string _PowerSN;
        [JsonProperty("PowerSN")]
        public string PowerSN
        {
            get { return _PowerSN; }
            set { _PowerSN = value; }
        }

        private string _DockSN;
        [JsonProperty("DockSN")]
        public string DockSN
        {
            get { return _DockSN; }
            set { _DockSN = value; }
        }

        private string _TypeDescription;
        [JsonProperty("TypeDescription")]
        public string TypeDescription
        {
            get { return _TypeDescription; }
            set { _TypeDescription = value; }
        }

        private string _ModelDescription;
        [JsonProperty("ModelDescription")]
        public string ModelDescription
        {
            get { return _ModelDescription; }
            set { _ModelDescription = value; }
        }

        private string _DateFrom;
        [JsonProperty("DateFrom")]
        public string DateFrom
        {
            get { return _DateFrom; }
            set { _DateFrom = value; }
        }

        private string _DateTo;
        [JsonProperty("DateTo")]
        public string DateTo
        {
            get { return _DateTo; }
            set { _DateTo = value; }
        }

        private int _ServiceNo;
        [JsonProperty("ServiceNo")]
        public int ServiceNo
        {
            get { return _ServiceNo; }
            set { _ServiceNo = value; }
        }

        private string _RequestNo;
        [JsonProperty("RequestNo")]
        public string RequestNo
        {
            get { return _RequestNo; }
            set { _RequestNo = value; }
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

        private string _ActionMade;
        [JsonProperty("ActionMade")]
        public string ActionMade
        {
            get { return _ActionMade; }
            set { _ActionMade = value; }
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

        private string _AuthCode;
        [JsonProperty("AuthCode")]
        public string AuthCode
        {
            get { return _AuthCode; }
            set { _AuthCode = value; }
        }

        private string _RefNo;
        [JsonProperty("RefNo")]
        public string RefNo
        {
            get { return _RefNo; }
            set { _RefNo = value; }
        }

        private string _NRIC;
        [JsonProperty("NRIC")]
        public string NRIC
        {
            get { return _NRIC; }
            set { _NRIC = value; }
        }

        private string _AdditionalInformation;
        [JsonProperty("AddInfo")]
        public string AdditionalInformation
        {
            get { return _AdditionalInformation; }
            set { _AdditionalInformation = value; }
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

        private string _JobTypeDescription;
        [JsonProperty("JobTypeDescription")]
        public string JobTypeDescription
        {
            get { return _JobTypeDescription; }
            set { _JobTypeDescription = value; }
        }

        private string _ServiceJobTypeDescription;
        [JsonProperty("ServiceJobTypeDescription")]
        public string ServiceJobTypeDescription
        {
            get { return _ServiceJobTypeDescription; }
            set { _ServiceJobTypeDescription = value; }
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
    }

    public class FSRDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<FSRData> data { get; set; }
    }
}
