using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class IRData
    {
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

        private int _IRID;
        [JsonProperty("IRID")]
        public int IRID
        {
            get { return _IRID; }
            set { _IRID = value; }
        }

        private string _IRNo;
        [JsonProperty("IRNo")]
        public string IRNo
        {
            get { return _IRNo; }
            set { _IRNo = value; }
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

        private int _MerchantID;
        [JsonProperty("MerchantID")]
        public int MerchantID
        {
            get { return _MerchantID; }
            set { _MerchantID = value; }
        }

        private int _ParticularID;
        [JsonProperty("ParticularID")]
        public int ParticularID
        {
            get { return _ParticularID; }
            set { _ParticularID = value; }
        }

        private string _ParticularName;
        [JsonProperty("ParticularName")]
        public string ParticularName
        {
            get { return _ParticularName; }
            set { _ParticularName = value; }
        }

        private string _Address;
        [JsonProperty("Address")]
        public string Address
        {
            get { return _Address; }
            set { _Address = value; }
        }

        private string _Address2;
        [JsonProperty("Address2")]
        public string Address2
        {
            get { return _Address2; }
            set { _Address2 = value; }
        }

        private string _Address3;
        [JsonProperty("Address3")]
        public string Address3
        {
            get { return _Address3; }
            set { _Address3 = value; }
        }

        private string _Address4;
        [JsonProperty("Address4")]
        public string Address4
        {
            get { return _Address4; }
            set { _Address4 = value; }
        }

        private string _ContactPerson;
        [JsonProperty("ContactPerson")]
        public string ContactPerson
        {
            get { return _ContactPerson; }
            set { _ContactPerson = value; }
        }

        private string _TelNo;
        [JsonProperty("TelNo")]
        public string TelNo
        {
            get { return _TelNo; }
            set { _TelNo = value; }
        }

        private string _Mobile;
        [JsonProperty("Mobile")]
        public string Mobile
        {
            get { return _Mobile; }
            set { _Mobile = value; }
        }

        private string _Email;
        [JsonProperty("Email")]
        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }

        private string _City;
        [JsonProperty("City")]
        public string City
        {
            get { return _City; }
            set { _City = value; }
        }

        private string _Province;
        [JsonProperty("Province")]
        public string Province
        {
            get { return _Province; }
            set { _Province = value; }
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

        private string _IRStatus;
        [JsonProperty("IRStatus")]
        public string IRStatus
        {
            get { return _IRStatus; }
            set { _IRStatus = value; }
        }

        private string _IRStatusDescription;
        [JsonProperty("IRStatusDescription")]
        public string IRStatusDescription
        {
            get { return _IRStatusDescription; }
            set { _IRStatusDescription = value; }
        }

        private string _ServiceTypeDescription;
        [JsonProperty("ServiceTypeDescription")]
        public string ServiceTypeDescription
        {
            get { return _ServiceTypeDescription; }
            set { _ServiceTypeDescription = value; }
        }

        private string _ImportDateTime;
        [JsonProperty("ImportDateTime")]
        public string ImportDateTime
        {
            get { return _ImportDateTime; }
            set { _ImportDateTime = value; }
        }

        private int _RegionID;
        [JsonProperty("RegionID")]
        public int RegionID
        {
            get { return _RegionID; }
            set { _RegionID = value; }
        }

        private int _TerminalID;
        [JsonProperty("TerminalID")]
        public int TerminalID
        {
            get { return _TerminalID; }
            set { _TerminalID = value; }
        }

        private string _TerminalSN;
        [JsonProperty("TerminalSN")]
        public string TerminalSN
        {
            get { return _TerminalSN; }
            set { _TerminalSN = value; }
        }

        private string _ReplaceTerminalSN;
        [JsonProperty("ReplaceTerminalSN")]
        public string ReplaceTerminalSN
        {
            get { return _ReplaceTerminalSN; }
            set { _ReplaceTerminalSN = value; }
        }

        private int _SIMID;
        [JsonProperty("SIMID")]
        public int SIMID
        {
            get { return _SIMID; }
            set { _SIMID = value; }
        }

        private string _SIMSerialNo;
        [JsonProperty("SIMSerialNo")]
        public string SIMSerialNo
        {
            get { return _SIMSerialNo; }
            set { _SIMSerialNo = value; }
        }

        private string _ReplaceSIMSN;
        [JsonProperty("ReplaceSIMSN")]
        public string ReplaceSIMSN
        {
            get { return _ReplaceSIMSN; }
            set { _ReplaceSIMSN = value; }
        }

        private int _DockID;
        [JsonProperty("DockID")]
        public int DockID
        {
            get { return _DockID; }
            set { _DockID = value; }
        }

        private string _DockSN;
        [JsonProperty("DockSN")]
        public string DockSN
        {
            get { return _DockSN; }
            set { _DockSN = value; }
        }

        private int _ClientID;
        [JsonProperty("ClientID")]
        public int ClientID
        {
            get { return _ClientID; }
            set { _ClientID = value; }
        }

        private string _ClientName;
        [JsonProperty("ClientName")]
        public string ClientName
        {
            get { return _ClientName; }
            set { _ClientName = value; }
        }

        private int _ServiceProviderID;
        [JsonProperty("ServiceProviderID")]
        public int ServiceProviderID
        {
            get { return _ServiceProviderID; }
            set { _ServiceProviderID = value; }
        }

        private string _ServiceProviderName;
        [JsonProperty("ServiceProviderName")]
        public string ServiceProviderName
        {
            get { return _ServiceProviderName; }
            set { _ServiceProviderName = value; }
        }

        private int _FEID;
        [JsonProperty("FEID")]
        public int FEID
        {
            get { return _FEID; }
            set { _FEID = value; }
        }

        private string _FEName;
        [JsonProperty("FEName")]
        public string FEName
        {
            get { return _FEName; }
            set { _FEName = value; }
        }

        private string _JobTypeDescription;
        [JsonProperty("JobTypeDescription")]
        public string JobTypeDescription
        {
            get { return _JobTypeDescription; }
            set { _JobTypeDescription = value; }
        }

        private string _JobTypeStatusDescription;
        [JsonProperty("JobTypeStatusDescription")]
        public string JobTypeStatusDescription
        {
            get { return _JobTypeStatusDescription; }
            set { _JobTypeStatusDescription = value; }
        }

        private string _ProcessedDateTime;
        [JsonProperty("ProcessedDateTime")]
        public string ProcessedDateTime
        {
            get { return _ProcessedDateTime; }
            set { _ProcessedDateTime = value; }
        }

        private string _ProcessedBy;
        [JsonProperty("ProcessedBy")]
        public string ProcessedBy
        {
            get { return _ProcessedBy; }
            set { _ProcessedBy = value; }
        }

        private string _ProcessType;
        [JsonProperty("ProcessType")]
        public string ProcessType
        {
            get { return _ProcessType; }
            set { _ProcessType = value; }
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

        private string _ServiceReqDate;
        [JsonProperty("ServiceReqDate")]
        public string ServiceReqDate
        {
            get { return _ServiceReqDate; }
            set { _ServiceReqDate = value; }
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

        private int _FSRNo;
        [JsonProperty("FSRNo")]
        public int FSRNo
        {
            get { return _FSRNo; }
            set { _FSRNo = value; }
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

        private string _SerialNoList;
        [JsonProperty("SerialNoList")]
        public string SerialNoList
        {
            get { return _SerialNoList; }
            set { _SerialNoList = value; }
        }

        private string _ServiceCode;
        [JsonProperty("ServiceCode")]
        public string ServiceCode
        {
            get { return _ServiceCode; }
            set { _ServiceCode = value; }
        }

        private string _MerchantName;
        [JsonProperty("MerchantName")]
        public string MerchantName
        {
            get { return _MerchantName; }
            set { _MerchantName = value; }
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

        private string _ActionMade;
        [JsonProperty("ActionMade")]
        public string ActionMade
        {
            get { return _ActionMade; }
            set { _ActionMade = value; }
        }

        private string _ReferenceNo;
        [JsonProperty("ReferenceNo")]
        public string ReferenceNo
        {
            get { return _ReferenceNo; }
            set { _ReferenceNo = value; }
        }

        private string _Description;
        [JsonProperty("Description")]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private double _RentalFee;
        [JsonProperty("RentalFee")]
        public double RentalFee
        {
            get { return _RentalFee; }
            set { _RentalFee = value; }
        }


    }

    public class IRDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<IRData> data { get; set; }
    }
}
