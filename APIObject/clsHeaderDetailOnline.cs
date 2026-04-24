using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class HeaderData
    {
        private int _HeaderID;
        [JsonProperty("HeaderID")]
        public int HeaderID
        {
            get { return _HeaderID; }
            set { _HeaderID = value; }
        }

        private string _Name;
        [JsonProperty("Name")]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Header1;
        [JsonProperty("Header1")]
        public string Header1
        {
            get { return _Header1; }
            set { _Header1 = value; }
        }

        private string _Header2;
        [JsonProperty("Header2")]
        public string Header2
        {
            get { return _Header2; }
            set { _Header2 = value; }
        }

        private string _Header3;
        [JsonProperty("Header3")]
        public string Header3
        {
            get { return _Header3; }
            set { _Header3 = value; }
        }

        private string _Header4;
        [JsonProperty("Header4")]
        public string Header4
        {
            get { return _Header4; }
            set { _Header4 = value; }
        }

        private string _Header5;
        [JsonProperty("Header5")]
        public string Header5
        {
            get { return _Header5; }
            set { _Header5 = value; }
        }
    }
    public class HeaderDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<HeaderData> data { get; set; }
    }
}
