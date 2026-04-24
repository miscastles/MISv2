using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class TypeData
    {
        private int _TypeID;
        [JsonProperty("TypeID")]
        public int TypeID
        {
            get { return _TypeID; }
            set { _TypeID = value; }
        }

        private string _Description;
        [JsonProperty("Description")]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private string _Remarks;
        [JsonProperty("Remarks")]
        public string Remarks
        {
            get { return _Remarks; }
            set { _Remarks = value; }
        }

        private string _QueryString;
        [JsonProperty("QueryString")]
        public string QueryString
        {
            get { return _QueryString; }
            set { _QueryString = value; }
        }

        // WorkType
        private int _WorkTypeID;
        [JsonProperty("WorkTypeID")]
        public int WorkTypeID
        {
            get { return _WorkTypeID; }
            set { _WorkTypeID = value; }
        }

        private string _Code;
        [JsonProperty("Code")]
        public string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }

        private double _TypeValue;
        [JsonProperty("TypeValue")]
        public double TypeValue
        {
            get { return _TypeValue; }
            set { _TypeValue = value; }
        }
        
    }

    public class TypeDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<TypeData> data { get; set; }
    }
}
