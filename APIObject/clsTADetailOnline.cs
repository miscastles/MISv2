using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class TAData
    {
        private string _SerialNo;
        [JsonProperty("SerialNo")]
        public string SerialNo
        {
            get { return _SerialNo; }
            set { _SerialNo = value; }
        }

        private string _DeliveryDate;
        [JsonProperty("DeliveryDate")]
        public string DeliveryDate
        {
            get { return _DeliveryDate; }
            set { _DeliveryDate = value; }
        }

        private string _ReceiveDate;
        [JsonProperty("ReceiveDate")]
        public string ReceiveDate
        {
            get { return _ReceiveDate; }
            set { _ReceiveDate = value; }
        }

        private int _TerminalStatus;
        [JsonProperty("TerminalStatus")]
        public int TerminalStatus
        {
            get { return _TerminalStatus; }
            set { _TerminalStatus = value; }
        }

        private string _TerminalStatusDescription;
        [JsonProperty("TerminalStatusDescription")]
        public string TerminalStatusDescription
        {
            get { return _TerminalStatusDescription; }
            set { _TerminalStatusDescription = value; }
        }

        private int _TAIDNo;
        [JsonProperty("TAIDNo")]
        public int TAIDNo
        {
            get { return _TAIDNo; }
            set { _TAIDNo = value; }
        }

        private int _TAID;
        [JsonProperty("TAID")]
        public int TAID
        {
            get { return _TAID; }
            set { _TAID = value; }
        }

        private int _ClientID;
        [JsonProperty("ClientID")]
        public int ClientID
        {
            get { return _ClientID; }
            set { _ClientID = value; }
        }

        private int _ServiceProviderID;
        [JsonProperty("ServiceProviderID")]
        public int ServiceProviderID
        {
            get { return _ServiceProviderID; }
            set { _ServiceProviderID = value; }
        }

        private int _IRID;
        [JsonProperty("IRID")]
        public int IRID
        {
            get { return _IRID; }
            set { _IRID = value; }
        }

        private int _IRIDNo;
        [JsonProperty("IRIDNo")]
        public int IRIDNo
        {
            get { return _IRIDNo; }
            set { _IRIDNo = value; }
        }

        private int _MerchantID;
        [JsonProperty("MerchantID")]
        public int MerchantID
        {
            get { return _MerchantID; }
            set { _MerchantID = value; }
        }

        private int _FEID;
        [JsonProperty("FEID")]
        public int FEID
        {
            get { return _FEID; }
            set { _FEID = value; }
        }

        private int _TerminalID;
        [JsonProperty("TerminalID")]
        public int TerminalID
        {
            get { return _TerminalID; }
            set { _TerminalID = value; }
        }

        private int _TerminalTypeID;
        [JsonProperty("TerminalTypeID")]
        public int TerminalTypeID
        {
            get { return _TerminalTypeID; }
            set { _TerminalTypeID = value; }
        }

        private int _TerminalModelID;
        [JsonProperty("TerminalModelID")]
        public int TerminalModelID
        {
            get { return _TerminalModelID; }
            set { _TerminalModelID = value; }
        }

        private int _TerminalBrandID;
        [JsonProperty("TerminalBrandID")]
        public int TerminalBrandID
        {
            get { return _TerminalBrandID; }
            set { _TerminalBrandID = value; }
        }

        private int _ServiceTypeID;
        [JsonProperty("ServiceTypeID")]
        public int ServiceTypeID
        {
            get { return _ServiceTypeID; }
            set { _ServiceTypeID = value; }
        }

        private int _OtherServiceTypeID;
        [JsonProperty("OtherServiceTypeID")]
        public int OtherServiceTypeID
        {
            get { return _OtherServiceTypeID; }
            set { _OtherServiceTypeID = value; }
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

        private string _SIMCarrier;
        [JsonProperty("SIMCarrier")]
        public string SIMCarrier
        {
            get { return _SIMCarrier; }
            set { _SIMCarrier = value; }
        }

        private int _SIMStatus;
        [JsonProperty("SIMStatus")]
        public int SIMStatus
        {
            get { return _SIMStatus; }
            set { _SIMStatus = value; }
        }

        private string _IRNo;
        [JsonProperty("IRNo")]
        public string IRNo
        {
            get { return _IRNo; }
            set { _IRNo = value; }
        }

        private string _TerminalSN;
        [JsonProperty("TerminalSN")]
        public string TerminalSN
        {
            get { return _TerminalSN; }
            set { _TerminalSN = value; }
        }


        private string _MerchantName;
        [JsonProperty("MerchantName")]
        public string MerchantName
        {
            get { return _MerchantName; }
            set { _MerchantName = value; }
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

        private string _ClientName;
        [JsonProperty("ClientName")]
        public string ClientName
        {
            get { return _ClientName; }
            set { _ClientName = value; }
        }

        private string _ServiceProviderName;
        [JsonProperty("ServiceProviderName")]
        public string ServiceProviderName
        {
            get { return _ServiceProviderName; }
            set { _ServiceProviderName = value; }
        }

        private string _FEName;
        [JsonProperty("FEName")]
        public string FEName
        {
            get { return _FEName; }
            set { _FEName = value; }
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

        private string _BrandDescription;
        [JsonProperty("BrandDescription")]
        public string BrandDescription
        {
            get { return _BrandDescription; }
            set { _BrandDescription = value; }
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

        private string _FSRDate;
        [JsonProperty("FSRDate")]
        public string FSRDate
        {
            get { return _FSRDate; }
            set { _FSRDate = value; }
        }

        private string _TADateTime;
        [JsonProperty("TADateTime")]
        public string TADateTime
        {
            get { return _TADateTime; }
            set { _TADateTime = value; }
        }

        private string _TAProcessedBy;
        [JsonProperty("TAProcessedBy")]
        public string TAProcessedBy
        {
            get { return _TAProcessedBy; }
            set { _TAProcessedBy = value; }
        }

        private string _TAModifiedBy;
        [JsonProperty("TAModifiedBy")]
        public string TAModifiedBy
        {
            get { return _TAModifiedBy; }
            set { _TAModifiedBy = value; }
        }

        private string _TAProcessedDateTime;
        [JsonProperty("TAProcessedDateTime")]
        public string TAProcessedDateTime
        {
            get { return _TAProcessedDateTime; }
            set { _TAProcessedDateTime = value; }
        }

        private string _TAModifiedDateTime;
        [JsonProperty("TAModifiedDateTime")]
        public string TAModifiedDateTime
        {
            get { return _TAModifiedDateTime; }
            set { _TAModifiedDateTime = value; }
        }

        private string _TARemarks;
        [JsonProperty("TARemarks")]
        public string TARemarks
        {
            get { return _TARemarks; }
            set { _TARemarks = value; }
        }

        private string _TAComments;
        [JsonProperty("TAComments")]
        public string TAComments
        {
            get { return _TAComments; }
            set { _TAComments = value; }
        }

        private string _ServiceTypeDescription;
        [JsonProperty("ServiceTypeDescription")]
        public string ServiceTypeDescription
        {
            get { return _ServiceTypeDescription; }
            set { _ServiceTypeDescription = value; }
        }

        private string _OtherServiceTypeDescription;
        [JsonProperty("OtherServiceTypeDescription")]
        public string OtherServiceTypeDescription
        {
            get { return _OtherServiceTypeDescription; }
            set { _OtherServiceTypeDescription = value; }
        }

        private string _BatchNo;
        [JsonProperty("BatchNo")]
        public string BatchNo
        {
            get { return _BatchNo; }
            set { _BatchNo = value; }
        }

        private string _Carrier;
        [JsonProperty("Carrier")]
        public string Carrier
        {
            get { return _Carrier; }
            set { _Carrier = value; }
        }

        private string _AssignedTo;
        [JsonProperty("AssignedTo")]
        public string AssignedTo
        {
            get { return _AssignedTo; }
            set { _AssignedTo = value; }
        }

        private string _Remarks;
        [JsonProperty("Remarks")]
        public string Remarks
        {
            get { return _Remarks; }
            set { _Remarks = value; }
        }

        private string _SIMStatusDescription;
        [JsonProperty("SIMStatusDescription")]
        public string SIMStatusDescription
        {
            get { return _SIMStatusDescription; }
            set { _SIMStatusDescription = value; }
        }

        private string _IRImportDateTime;
        [JsonProperty("IRImportDateTime")]
        public string IRImportDateTime
        {
            get { return _IRImportDateTime; }
            set { _IRImportDateTime = value; }
        }

        private string _RegionID;
        [JsonProperty("RegionID")]
        public string RegionID
        {
            get { return _RegionID; }
            set { _RegionID = value; }
        }

        private string _RegionType;
        [JsonProperty("RegionType")]
        public string RegionType
        {
            get { return _RegionType; }
            set { _RegionType = value; }
        }

        private string _Region;
        [JsonProperty("Region")]
        public string Region
        {
            get { return _Region; }
            set { _Region = value; }
        }

        private int _ServiceTypeStatus;
        [JsonProperty("ServiceTypeStatus")]
        public int ServiceTypeStatus
        {
            get { return _ServiceTypeStatus; }
            set { _ServiceTypeStatus = value; }
        }

        private string _ServiceTypeStatusDescription;
        [JsonProperty("ServiceTypeStatusDescription")]
        public string ServiceTypeStatusDescription
        {
            get { return _ServiceTypeStatusDescription; }
            set { _ServiceTypeStatusDescription = value; }
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

        private string _ReferenceNo;
        [JsonProperty("ReferenceNo")]
        public string ReferenceNo
        {
            get { return _ReferenceNo; }
            set { _ReferenceNo = value; }
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
    }
    public class TADetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<TAData> data { get; set; }
    }
}
