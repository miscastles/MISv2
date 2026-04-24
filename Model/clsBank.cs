using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class clsBank
    {
        public int ID { get; set; }
        public int Enable { get; set; }
        public int IsBillType { get; set; }
        public string Code { get; set; }
        public string Bank { get; set; }
        public string DisplayName { get; set; }
        public string mainColor { get; set; }
        public string primaryColor { get; set; }
        public string secondaryColor { get; set; }

        public override string ToString() => DisplayName;  // for ComboBox display
    }

    public class BankResponse
    {
        [JsonProperty("resp_code")]
        public string RespCode { get; set; }

        public string Message { get; set; }

        [JsonProperty("recordcount")]
        public string RecordCount { get; set; }

        public List<clsBank> Data { get; set; }
    }

}
