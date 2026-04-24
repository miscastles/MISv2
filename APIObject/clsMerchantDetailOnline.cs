using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class MerchantData
    {
        private int _MerchantID;
        [JsonProperty("MerchantID")]
        public int MerchantID
        {
            get { return _MerchantID; }
            set { _MerchantID = value; }
        }

        private int _CityID;
        [JsonProperty("CityID")]
        public int CityID
        {
            get { return _CityID; }
            set { _CityID = value; }
        }

        private int _ProvinceID;
        [JsonProperty("ProvinceID")]
        public int ProvinceID
        {
            get { return _ProvinceID; }
            set { _ProvinceID = value; }
        }

        private string _MerchantName;
        [JsonProperty("MerchantName")]
        public string MerchantName
        {
            get { return _MerchantName; }
            set { _MerchantName = value; }
        }

        private string _Address;
        [JsonProperty("Address")]
        public string Address
        {
            get { return _Address; }
            set
            { _Address = value; }
        }

        private string _City;
        [JsonProperty("City")]
        public string City
        {
            get { return _City; }
            set { _City = value; }
        }

        private string _Province;
        [JsonProperty("Province")]
        public string Province
        {
            get { return _Province; }
            set { _Province = value; }
        }

        private string _ContactPerson;
        [JsonProperty("ContactPerson")]
        public string ContactPerson
        {
            get { return _ContactPerson; }
            set { _ContactPerson = value; }
        }

        private string _ContactNo;
        [JsonProperty("ContactNo")]
        public string ContactNo
        {
            get { return _ContactNo; }
            set { _ContactNo = value; }
        }
    }
    public class MerchantDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<MerchantData> data { get; set; }
    }
}
