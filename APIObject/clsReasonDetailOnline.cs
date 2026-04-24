using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class ReasonData
    {
        private int _ReasonID;
        [JsonProperty("ReasonID")]
        public int ReasonID
        {
            get { return _ReasonID; }
            set { _ReasonID = value; }
        }

        private string _Code;
        [JsonProperty("Code")]
        public string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }

        private string _Description;
        [JsonProperty("Description")]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private string _Type;
        [JsonProperty("Type")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        private int _IsInput;
        [JsonProperty("isInput")]
        public int IsInput
        {
            get { return _IsInput; }
            set { _IsInput = value; }
        }
    }
    public class ReasonDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<ReasonData> data { get; set; }
    }
}
