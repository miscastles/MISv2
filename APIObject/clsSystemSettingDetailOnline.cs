using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class SystemSettingData
    {
        private int _SysID;
        [JsonProperty("SysID")]
        public int SysID
        {
            get { return _SysID; }
            set { _SysID = value; }
        }

        private string _PublishDate;
        [JsonProperty("PublishDate")]
        public string PublishDate
        {
            get { return _PublishDate; }
            set { _PublishDate = value; }
        }

        private string _PublishVersion;
        [JsonProperty("PublishVersion")]
        public string PublishVersion
        {
            get { return _PublishVersion; }
            set { _PublishVersion = value; }
        }
    }
    public class SystemSettingDetailOnline
    {

        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<SystemSettingData> data { get; set; }
    }
}
