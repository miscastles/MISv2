using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class ServiceTypeData
    {
        private int _ServiceTypeID;
        [JsonProperty("ServiceTypeID")]
        public int ServiceTypeID
        {
            get { return _ServiceTypeID; }
            set { _ServiceTypeID = value; }
        }

        private string _Description;
        [JsonProperty("Description")]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private string _Code;
        [JsonProperty("Code")]
        public string Code
        {
            get { return _Code; }
            set { _Code = value; }
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
    }
    public class ServiceTypeDetailOnline
    {

        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<ServiceTypeData> data { get; set; }
    }
}
