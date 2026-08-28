using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MIS
{
    public sealed class QRDeliveryData
    {
        public string TID { get; set; }
        public string MID { get; set; }
        public string MerchantName { get; set; }
        public string MerchantAddress { get; set; }
        public string TerminalSerialNo { get; set; }
        public string SimSerialNo { get; set; }
    }

    public sealed class QRDeliveryFieldResult
    {
        public string Field { get; set; }
        public string ScannedValue { get; set; }
        public string ExpectedValue { get; set; }
        public bool IsMatch { get; set; }
    }

    public sealed class QRDeliveryValidationResult
    {
        public QRDeliveryValidationResult()
        {
            Fields = new List<QRDeliveryFieldResult>();
            MissingFields = new List<string>();
        }

        public IList<QRDeliveryFieldResult> Fields { get; private set; }
        public IList<string> MissingFields { get; private set; }
        public bool IsMatch { get; set; }
    }

    public sealed class QRDeliveryValidationException : Exception
    {
        public QRDeliveryValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public interface IQRDeliveryHistoryStore
    {
        void Save(QRDeliverySaveRequest request);
        IList<QRDeliveryHistoryItem> GetRecent(int serviceNo, int limit);
    }

    public interface IQRDeliveryLookupStore
    {
        QRDeliveryLookupResult FindJobOrder(string tid, string mid);
    }

    public sealed class QRDeliveryLookupResult
    {
        public bool Found { get; set; }
        public int ServiceNo { get; set; }
        public int IRIDNo { get; set; }
        public int MerchantID { get; set; }
        public int JobType { get; set; }
        public string JobTypeDescription { get; set; }
        public QRDeliveryData Expected { get; set; }
    }

    public sealed class QRDeliverySaveRequest
    {
        public int ServiceNo { get; set; }
        public int IRIDNo { get; set; }
        public int MerchantID { get; set; }
        public string QRContent { get; set; }
        public string InventoryStatus { get; set; }
        public string TerminalPrepStatus { get; set; }
        public string DispatcherStatus { get; set; }
        public string ProcessedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public sealed class QRDeliveryHistoryItem
    {
        [JsonProperty("QRID")]
        public int QRID { get; set; }
        [JsonProperty("ServiceNo")]
        public int ServiceNo { get; set; }
        [JsonProperty("IRIDNo")]
        public int IRIDNo { get; set; }
        [JsonProperty("MerchantID")]
        public int MerchantID { get; set; }
        [JsonProperty("InventoryStatus")]
        public string InventoryStatus { get; set; }
        [JsonProperty("TerminalPrepStatus")]
        public string TerminalPrepStatus { get; set; }
        [JsonProperty("DispatcherStatus")]
        public string DispatcherStatus { get; set; }
        [JsonProperty("ProcessedBy")]
        public string ProcessedBy { get; set; }
        [JsonProperty("QRDate")]
        public DateTime QRDate { get; set; }
        [JsonProperty("DateTimeStamp")]
        public DateTime DateTimeStamp { get; set; }
    }

    public sealed class QRDeliveryValidator
    {
        private static readonly string[] RequiredFields =
        {
            "tid", "mid", "merchantName", "terminalSerialNo", "simSerialNo"
        };

        public QRDeliveryValidationResult Validate(string json, QRDeliveryData expected)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new QRDeliveryValidationException("The QR content is empty.", null);
            if (expected == null)
                throw new ArgumentNullException("expected");

            JObject source;
            try
            {
                source = JObject.Parse(json.Trim());
            }
            catch (JsonException ex)
            {
                throw new QRDeliveryValidationException(
                    "The scanned QR content is not valid JSON. Please scan the terminal QR code again.", ex);
            }

            QRDeliveryValidationResult result = new QRDeliveryValidationResult();
            foreach (string field in RequiredFields)
            {
                JToken token = source.GetValue(field, StringComparison.OrdinalIgnoreCase);
                if (token == null || string.IsNullOrWhiteSpace(token.ToString()))
                    result.MissingFields.Add(DisplayName(field));
            }

            AddResult(result, "TID", Value(source, "tid"), expected.TID);
            AddResult(result, "MID", Value(source, "mid"), expected.MID);
            AddResult(result, "Merchant Name", Value(source, "merchantName"), expected.MerchantName);
            AddResult(result, "Terminal Serial No.", Value(source, "terminalSerialNo"), expected.TerminalSerialNo);
            AddResult(result, "SIM Serial No.", Value(source, "simSerialNo"), expected.SimSerialNo);

            result.IsMatch = result.MissingFields.Count == 0;
            foreach (QRDeliveryFieldResult field in result.Fields)
                result.IsMatch &= field.IsMatch;
            return result;
        }

        public QRDeliveryData Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new QRDeliveryValidationException("The QR content is empty.", null);

            JObject source;
            try
            {
                source = JObject.Parse(json.Trim());
            }
            catch (JsonException ex)
            {
                throw new QRDeliveryValidationException(
                    "The scanned QR content is not valid JSON. Please scan the terminal QR code again.", ex);
            }

            return new QRDeliveryData
            {
                TID = Value(source, "tid"),
                MID = Value(source, "mid"),
                MerchantName = Value(source, "merchantName"),
                TerminalSerialNo = Value(source, "terminalSerialNo"),
                SimSerialNo = Value(source, "simSerialNo")
            };
        }

        private static void AddResult(QRDeliveryValidationResult result, string field,
            string scanned, string expected)
        {
            scanned = Normalize(scanned);
            expected = Normalize(expected);
            result.Fields.Add(new QRDeliveryFieldResult
            {
                Field = field,
                ScannedValue = scanned,
                ExpectedValue = expected,
                IsMatch = string.Equals(scanned, expected, StringComparison.OrdinalIgnoreCase)
            });
        }

        private static string Value(JObject source, string name)
        {
            JToken token = source.GetValue(name, StringComparison.OrdinalIgnoreCase);
            return token == null ? string.Empty : token.ToString();
        }

        private static string Normalize(string value)
        {
            return (value ?? string.Empty).Trim();
        }

        private static string DisplayName(string field)
        {
            switch (field)
            {
                case "merchantName": return "Merchant Name";
                case "terminalSerialNo": return "Terminal Serial No.";
                case "simSerialNo": return "SIM Serial No.";
                default: return field.ToUpperInvariant();
            }
        }
    }
}
