using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class ProvinceData
    {
        private int _ProvinceID;
        [JsonProperty("ProvinceID")]
        public int ProvinceID
        {
            get { return _ProvinceID; }
            set { _ProvinceID = value; }
        }

        private string _Province;
        [JsonProperty("Name")]
        public string Province
        {
            get { return _Province; }
            set { _Province = value; }
        }
    }
    public class ProvinceDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<ProvinceData> data { get; set; }
    }
}
