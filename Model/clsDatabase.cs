using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class clsDatabase
    {
        public string Code { get; set; }
        public string Source { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Security { get; set; }
        public int Port { get; set; }
        public int ConnectionTimeout { get; set; }
    }

    public class DatabaseResponse
    {
        [JsonProperty("resp_code")]
        public string RespCode { get; set; }

        public string Message { get; set; }

        [JsonProperty("recordcount")]
        public string RecordCount { get; set; }

        public List<clsDatabase> Data { get; set; }
    }
}
