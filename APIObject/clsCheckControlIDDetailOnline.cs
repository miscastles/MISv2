using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class CheckControlIDData
    {
        private int _ControlID;
        [JsonProperty("ControlID")]
        public int ControlID
        {
            get { return _ControlID; }
            set { _ControlID = value; }
        }
    }
    public class CheckControlIDDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<CheckControlIDData> data { get; set; }
    }
}
