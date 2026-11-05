
using System;
using Newtonsoft.Json;

namespace MIS
{
    public sealed class QRDeliveryRecord
    {
        [JsonProperty("QRID")]
        public int QRID { get; set; }

        [JsonProperty("ServiceNo")]
        public int ServiceNo { get; set; }

        [JsonProperty("IRIDNo")]
        public int IRIDNo { get; set; }

        [JsonProperty("MerchantID")]
        public int MerchantID { get; set; }

        [JsonProperty("MerchantName")]
        public string MerchantName { get; set; }

        [JsonProperty("MerchantAddress")]
        public string MerchantAddress { get; set; }

        [JsonProperty("TID")]
        public string TID { get; set; }

        [JsonProperty("MID")]
        public string MID { get; set; }

        [JsonProperty("TerminalSN")]
        public string TerminalSN { get; set; }

        [JsonProperty("SIMSN")]
        public string SIMSN { get; set; }

        [JsonProperty("JobType")]
        public int JobType { get; set; }

        [JsonProperty("QRContent")]
        public string QRContent { get; set; }

        [JsonProperty("InternalQRContent")]
        public string InternalQRContent { get; set; }

        [JsonProperty("InventoryStatus")]
        public string InventoryStatus { get; set; }

        [JsonProperty("TerminalPrepStatus")]
        public string TerminalPrepStatus { get; set; }

        [JsonProperty("DispatcherStatus")]
        public string DispatcherStatus { get; set; }

        [JsonProperty("QRResult")]
        public string QRResult { get; set; }

        [JsonProperty("ProcessedBy")]
        public string ProcessedBy { get; set; }

        [JsonProperty("QRDate")]
        public DateTime QRDate { get; set; }

        [JsonProperty("DateTimeStamp")]
        public DateTime DateTimeStamp { get; set; }
    }
}
