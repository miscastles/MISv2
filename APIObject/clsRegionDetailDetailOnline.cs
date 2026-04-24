using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class RegionDetailDData
    {
        private int _RegionID;
        [JsonProperty("RegionID")]
        public int RegionID
        {
            get { return _RegionID; }
            set { _RegionID = value; }
        }

        private int _RegionType;
        [JsonProperty("RegionType")]
        public int RegionType
        {
            get { return _RegionType; }
            set { _RegionType = value; }
        }

        private string _Province;
        [JsonProperty("Province")]
        public string Province
        {
            get { return _Province; }
            set { _Province = value; }
        }

        private string _Region;
        [JsonProperty("Region")]
        public string Region
        {
            get { return _Region; }
            set { _Region = value; }
        }
    }
    public class RegionDetailDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<RegionDetailDData> data { get; set; }
    }
}
