using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class CityData
    {
        private int _CityID;
        [JsonProperty("CityID")]
        public int CityID
        {
            get { return _CityID; }
            set { _CityID = value; }
        }

        private string _City;
        [JsonProperty("City")]
        public string City
        {
            get { return _City; }
            set { _City = value; }
        }
    }
    public class CityDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<CityData> data { get; set; }
    }
}
