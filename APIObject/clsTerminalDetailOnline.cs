using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class TerminalData
    {
        private int _TerminalID;
        [JsonProperty("TerminalID")]
        public int TerminalID
        {
            get { return _TerminalID; }
            set { _TerminalID = value; }
        }

        private int _TIID;
        [JsonProperty("TIID")]
        public int TIID
        {
            get { return _TIID; }
            set { _TIID = value; }
        }

        private int _SIMID;
        [JsonProperty("SIMID")]
        public int SIMID
        {
            get { return _SIMID; }
            set { _SIMID = value; }
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
        private string _No;
        [JsonProperty("No")]
        public string No
        {
            get { return _No; }
            set { _No = value; }
        }        

        private string _SerialNo;
        [JsonProperty("SerialNo")]
        public string SerialNo
        {
            get { return _SerialNo; }
            set { _SerialNo = value; }
        }

        private string _TerminalType;
        [JsonProperty("TerminalType")]
        public string TerminalType
        {
            get { return _TerminalType; }
            set { _TerminalType = value; }
        }

        private string _TerminalModel;
        [JsonProperty("TerminalModel")]
        public string TerminalModel
        {
            get { return _TerminalModel; }
            set { _TerminalModel = value; }
        }

        private string _TerminalBrand;
        [JsonProperty("TerminalBrand")]
        public string TerminalBrand
        {
            get { return _TerminalBrand; }
            set { _TerminalBrand = value; }
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

        private string _TerminalStatus;
        [JsonProperty("TerminalStatus")]
        public string TerminalStatus
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

        private string _TerminalCount;
        [JsonProperty("TerminalCount")]
        public string TerminalCount
        {
            get { return _TerminalCount; }
            set { _TerminalCount = value; }
        }

        private string _PartNo;
        [JsonProperty("PartNo")]
        public string PartNo
        {
            get { return _PartNo; }
            set { _PartNo = value; }
        }

        private string _PONo;
        [JsonProperty("PONo")]
        public string PONo
        {
            get { return _PONo; }
            set { _PONo = value; }
        }

        private string _InvNo;
        [JsonProperty("InvNo")]
        public string InvNo
        {
            get { return _InvNo; }
            set { _InvNo = value; }
        }

        private string _Carrier;
        [JsonProperty("Carrier")]
        public string Carrier
        {
            get { return _Carrier; }
            set { _Carrier = value; }
        }

        private int _SIMStatus;
        [JsonProperty("SIMStatus")]
        public int SIMStatus
        {
            get { return _SIMStatus; }
            set { _SIMStatus = value; }
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

        private string _Location;
        [JsonProperty("Location")]
        public string Location
        {
            get { return _Location; }
            set { _Location = value; }
        }

        private string _Allocation;
        [JsonProperty("Allocation")]
        public string Allocation
        {
            get { return _Allocation; }
            set { _Allocation = value; }
        }

        private string _AssetType;
        [JsonProperty("AssetType")]
        public string AssetType
        {
            get { return _AssetType; }
            set { _AssetType = value; }
        }

        private string _ServiceStatusDescription;
        [JsonProperty("ServiceStatusDescription")]
        public string ServiceStatusDescription
        {
            get { return _ServiceStatusDescription; }
            set { _ServiceStatusDescription = value; }
        }

        private string _ParticularName;
        [JsonProperty("ParticularName")]
        public string ParticularName
        {
            get { return _ParticularName; }
            set { _ParticularName = value; }
        }

        private string _Date;
        [JsonProperty("Date")]
        public string Date
        {
            get { return _Date; }
            set { _Date = value; }
        }

        private string _Time;
        [JsonProperty("Time")]
        public string Time
        {
            get { return _Time; }
            set { _Time = value; }
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

        private int _TransNo;
        [JsonProperty("TransNo")]
        public int TransNo
        {
            get { return _TransNo; }
            set { _TransNo = value; }
        }

        private string _TransDate;
        [JsonProperty("TransDate")]
        public string TransDate
        {
            get { return _TransDate; }
            set { _TransDate = value; }
        }

        private string _TransTime;
        [JsonProperty("TransTime")]
        public string TransTime
        {
            get { return _TransTime; }
            set { _TransTime = value; }
        }

        private string _ReleaseDate;
        [JsonProperty("ReleaseDate")]
        public string ReleaseDate
        {
            get { return _ReleaseDate; }
            set { _ReleaseDate = value; }
        }

        private string _RequestNo;
        [JsonProperty("RequestNo")]
        public string RequestNo
        {
            get { return _RequestNo; }
            set { _RequestNo = value; }
        }

        private string _ReferenceNo;
        [JsonProperty("ReferenceNo")]
        public string ReferenceNo
        {
            get { return _ReferenceNo; }
            set { _ReferenceNo = value; }
        }

        private int _UserID;
        [JsonProperty("UserID")]
        public int UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }

        private int _FromLocationID;
        [JsonProperty("FromLocationID")]
        public int FromLocationID
        {
            get { return _FromLocationID; }
            set { _FromLocationID = value; }
        }

        private string _FromLocation;
        [JsonProperty("FromLocation")]
        public string FromLocation
        {
            get { return _FromLocation; }
            set { _FromLocation = value; }
        }

        private int _ToLocationID;
        [JsonProperty("ToLocationID")]
        public int ToLocationID
        {
            get { return _ToLocationID; }
            set { _ToLocationID = value; }
        }

        private string _ToLocation;
        [JsonProperty("ToLocation")]
        public string ToLocation
        {
            get { return _ToLocation; }
            set { _ToLocation = value; }
        }

    }
    public class TerminalDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<TerminalData> data { get; set; }
    }
}
