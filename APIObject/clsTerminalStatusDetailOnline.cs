using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class TerminalStatusData
    {
        private int _TerminalStatusID;
        [JsonProperty("TerminalStatusID")]
        public int TerminalStatusID
        {
            get { return _TerminalStatusID; }
            set { _TerminalStatusID = value; }
        }

        private string _Description;
        [JsonProperty("Description")]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private int _TerminalStatusType;
        [JsonProperty("TerminalStatusType")]
        public int TerminalStatusType
        {
            get { return _TerminalStatusType; }
            set { _TerminalStatusType = value; }
        }
    }
    public class TerminalStatusDetailOnline
    {

        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<TerminalStatusData> data { get; set; }
    }
}
