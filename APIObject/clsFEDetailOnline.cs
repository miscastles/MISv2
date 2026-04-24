using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class FEData
    {
        private int _FEID;
        [JsonProperty("FEID")]
        public int FEID
        {
            get { return _FEID; }
            set { _FEID = value; }
        }

        private string _Description;
        [JsonProperty("Description")]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private string _Name;
        [JsonProperty("Name")]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Address;
        [JsonProperty("Address")]
        public string Address
        {
            get { return _Address; }
            set { _Address = value; }
        }

        private string _ContactNo;
        [JsonProperty("ContactNo")]
        public string ContactNo
        {
            get { return _ContactNo; }
            set { _ContactNo = value; }
        }
    }
    public class FEDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<FEData> data { get; set; }
    }
}
