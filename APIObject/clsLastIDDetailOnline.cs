using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class LastIDData
    {
        private int _LastInsertID;
        [JsonProperty("LastInsertID")]
        public int LastInsertID
        {
            get { return _LastInsertID; }
            set { _LastInsertID = value; }
        }

        private string _LastTableName;
        [JsonProperty("LastTableName")]
        public string LastTableName
        {
            get { return _LastTableName; }
            set { _LastTableName = value; }
        }
    }
    public class LastIDDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<LastIDData> data { get; set; }
    }
}
