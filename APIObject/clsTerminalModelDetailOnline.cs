using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class TerminalModelData
    {
        private int _TerminalModelID;
        [JsonProperty("TerminalModelID")]
        public int TerminalModelID
        {
            get { return _TerminalModelID; }
            set { _TerminalModelID = value; }
        }

        private string _Description;
        [JsonProperty("Description")]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private int _TerminalTypeID;
        [JsonProperty("TerminalTypeID")]
        public int TerminalTypeID
        {
            get { return _TerminalTypeID; }
            set { _TerminalTypeID = value; }
        }

        private string _TerminalTypeDescription;
        [JsonProperty("TerminalTypeDescription")]
        public string TerminalTypeDescription
        {
            get { return _TerminalTypeDescription; }
            set { _TerminalTypeDescription = value; }
        }
    }
    public class TerminalModelDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<TerminalModelData> data { get; set; }
    }
}
