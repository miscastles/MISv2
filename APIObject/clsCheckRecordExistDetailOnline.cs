using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class CheckRecordExistData
    {
        private int _RecordID;
        [JsonProperty("RecordID")]
        public int RecordID
        {
            get { return _RecordID; }
            set { _RecordID = value; }
        }

        private int _Count;
        [JsonProperty("Count")]
        public int Count
        {
            get { return _Count; }
            set { _Count = value; }
        }
    }
    public class CheckRecordExistDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<CheckRecordExistData> data { get; set; }
    }
}
