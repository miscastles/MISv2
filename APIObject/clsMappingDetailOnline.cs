using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class DescriptionMappingData
    {
        private int _MapID;
        [JsonProperty("MapID")]
        public int MapID
        {
            get { return _MapID; }
            set { _MapID = value; }
        }

        private string _MapFrom;
        [JsonProperty("MapFrom")]
        public string MapFrom
        {
            get { return _MapFrom; }
            set { _MapFrom = value; }
        }

        private string _MapTo;
        [JsonProperty("MapTo")]
        public string MapTo
        {
            get { return _MapTo; }
            set { _MapTo = value; }
        }

        private string _Delimeter;
        [JsonProperty("Delimeter")]
        public string Delimeter
        {
            get { return _Delimeter; }
            set { _Delimeter = value; }
        }

        private string _ColumnIndex;
        [JsonProperty("ColumnIndex")]
        public string ColumnIndex
        {
            get { return _ColumnIndex; }
            set { _ColumnIndex = value; }
        }

        private int _isMust;
        [JsonProperty("isMust")]
        public int isMust
        {
            get { return _isMust; }
            set { _isMust = value; }
        }

        private string _Format;
        [JsonProperty("Format")]
        public string Format
        {
            get { return _Format; }
            set { _Format = value; }
        }
    }
    public class MappingDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<DescriptionMappingData> data { get; set; }
    }
}
