using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class ServiceProviderData
    {
        private int _ProviderID;
        [JsonProperty("ProviderID")]
        public int ProviderID
        {
            get { return _ProviderID; }
            set { _ProviderID = value; }
        }

        private string _Name;
        [JsonProperty("Name")]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Address;
        [JsonProperty("Address")]
        public string Address
        {
            get { return _Address; }
            set { _Address = value; }
        }

        private string _TelNo;
        [JsonProperty("TelNo")]
        public string TelNo
        {
            get { return _TelNo; }
            set { _TelNo = value; }
        }

        private string _Mobile;
        [JsonProperty("Mobile")]
        public string Mobile
        {
            get { return _Mobile; }
            set { _Mobile = value; }
        }

        private string _Fax;
        [JsonProperty("Fax")]
        public string Fax
        {
            get { return _Fax; }
            set { _Fax = value; }
        }

        private string _Email;
        [JsonProperty("Email")]
        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }

        private string _ContractTerms;
        [JsonProperty("ContractTerms")]
        public string ContractTerms
        {
            get { return _ContractTerms; }
            set { _ContractTerms = value; }
        }       
    }

    public class ServiceProviderDetailOnline
    {

        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<ServiceProviderData> data { get; set; }
    }
}
