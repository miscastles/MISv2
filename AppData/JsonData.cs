using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MIS.AppData
{
    class JsonData
    {
        // Response Message Format
        [JsonObject]
        public class RespMsg
        {
            //[JsonProperty("resp_code")] - Set Property
            public string Code { get; set; }
            public string Message { get; set; }
            public string Record { get; set; }
            public JArray Data { get; set; }
        }
    }
}
