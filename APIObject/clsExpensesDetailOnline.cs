using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class ExpensesData
    {
        private int _ExpensesID;
        [JsonProperty("ExpensesID")]
        public int ExpensesID
        {
            get { return _ExpensesID; }
            set { _ExpensesID = value; }
        }

        private string _Description;
        [JsonProperty("Description")]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
    }
    public class ExpensesDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<ExpensesData> data { get; set; }
    }
}
