using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class TerminalTypeData
    {
        private int _TerminalTypeID;
        [JsonProperty("TerminalTypeID")]
        public int TerminalTypeID
        {
            get { return _TerminalTypeID; }
            set { _TerminalTypeID = value; }
        }

        private string _Description;
        [JsonProperty("Description")]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
    }
    public class TerminalTypeDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<TerminalTypeData> data { get; set; }
    }
}
