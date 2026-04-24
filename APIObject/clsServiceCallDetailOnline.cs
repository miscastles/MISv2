using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class ServiceCallData
    {
        private int _SCNo;
        [JsonProperty("SCNo")]
        public int SCNo
        {
            get { return _SCNo; }
            set { _SCNo = value; }
        }

        private string _SCDateTime;
        [JsonProperty("SCDateTime")]
        public string SCDateTime
        {
            get { return _SCDateTime; }
            set { _SCDateTime = value; }
        }

        private string _ReferralID;
        [JsonProperty("ReferralID")]
        public string ReferralID
        {
            get { return _ReferralID; }
            set { _ReferralID = value; }
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

        private string _ReportedProblem;
        [JsonProperty("ReportedProblem")]
        public string ReportedProblem
        {
            get { return _ReportedProblem; }
            set { _ReportedProblem = value; }
        }

        private string _ArrangementMade;
        [JsonProperty("ArrangementMade")]
        public string ArrangementMade
        {
            get { return _ArrangementMade; }
            set { _ArrangementMade = value; }
        }

        private string _SCReqDate;
        [JsonProperty("SCReqDate")]
        public string SCReqDate
        {
            get { return _SCReqDate; }
            set { _SCReqDate = value; }
        }

        private string _SCReqTime;
        [JsonProperty("SCReqTime")]
        public string SCReqTime
        {
            get { return _SCReqTime; }
            set { _SCReqTime = value; }
        }

        private string _SCShipDate;
        [JsonProperty("SCShipDate")]
        public string SCShipDate
        {
            get { return _SCShipDate; }
            set { _SCShipDate = value; }
        }

        private string _SCShipTime;
        [JsonProperty("SCShipTime")]
        public string SCShipTime
        {
            get { return _SCShipTime; }
            set { _SCShipTime = value; }
        }

        private string _TrackingNo;
        [JsonProperty("TrackingNo")]
        public string TrackingNo
        {
            get { return _TrackingNo; }
            set { _TrackingNo = value; }
        }

        private string _SCStatus;
        [JsonProperty("SCStatus")]
        public string SCStatus
        {
            get { return _SCStatus; }
            set { _SCStatus = value; }
        }
    }
    public class ServiceCallDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<ServiceCallData> data { get; set; }
    }
}
