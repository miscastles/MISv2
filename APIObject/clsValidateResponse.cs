using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class ValidateResponse
    {
        [JsonProperty("resp_code")]
        public string resp_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }       
    }
}
