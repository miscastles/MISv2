using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class SIMData
    {
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

        private string _SIMStatusDescription;
        [JsonProperty("SIMStatusDescription")]
        public string SIMStatusDescription
        {
            get { return _SIMStatusDescription; }
            set { _SIMStatusDescription = value; }
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

        private string _IRNo;
        [JsonProperty("IRNo")]
        public string IRNo
        {
            get { return _IRNo; }
            set { _IRNo = value; }
        }

        private string _Remarks;
        [JsonProperty("Remarks")]
        public string Remarks
        {
            get { return _Remarks; }
            set { _Remarks = value; }
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
    public class SIMDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<SIMData> data { get; set; }
    }
}
