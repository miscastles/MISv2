using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class ReportData
    {
        private int _ReportID;
        [JsonProperty("ReportID")]
        public int ReportID
        {
            get { return _ReportID; }
            set { _ReportID = value; }
        }

        private string _ReportDesc;
        [JsonProperty("ReportDesc")]
        public string ReportDesc
        {
            get { return _ReportDesc; }
            set { _ReportDesc = value; }
        }

        private string _ReportType;
        [JsonProperty("ReportType")]
        public string ReportType
        {
            get { return _ReportType; }
            set { _ReportType = value; }
        }

        private string _ReportOrderDisplay;
        [JsonProperty("ReportOrderDisplay")]
        public string ReportOrderDisplay
        {
            get { return _ReportOrderDisplay; }
            set { _ReportOrderDisplay = value; }
        }
    }
    public class ReportDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<ReportData> data { get; set; }
    }
}
