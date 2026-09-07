using System;
using System.Collections.Generic;
using System.Globalization;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace MIS.Controller
{
    /// <summary>
    /// MIS API adapter for QR Delivery lookups, saves, and history retrieval.
    /// </summary>
    public sealed class QRDeliveryController : IQRDeliveryHistoryStore, IQRDeliveryLookupStore
    {
        private readonly clsAPI api = new clsAPI();

        public QRDeliveryLookupResult FindJobOrder(string tid, string mid)
        {
            if (string.IsNullOrWhiteSpace(tid) || string.IsNullOrWhiteSpace(mid))
                return new QRDeliveryLookupResult { Found = false };

            string json = api.getInfoDetailJSON("Search", "QR Delivery",
                tid.Trim() + "|" + mid.Trim());
            if (string.IsNullOrWhiteSpace(json))
                return new QRDeliveryLookupResult { Found = false };

            LookupJson data = JsonConvert.DeserializeObject<LookupJson>(json);
            if (data == null) return new QRDeliveryLookupResult { Found = false };
            return new QRDeliveryLookupResult
            {
                Found = true,
                ServiceNo = data.ServiceNo,
                IRIDNo = data.IRIDNo,
                MerchantID = data.MerchantID,
                JobType = data.JobType,
                JobTypeDescription = string.IsNullOrWhiteSpace(data.JobTypeDescription)
                    ? data.pJobTypeDescription : data.JobTypeDescription,
                JobTypeStatusDescription = data.JobTypeStatusDescription,
                TerminalInventoryStatus = data.EffectiveTerminalStatus,
                SimInventoryStatus = data.EffectiveSimStatus,
                Expected = new QRDeliveryData
                {
                    ServiceNo = data.ServiceNo,
                    IRIDNo = data.IRIDNo,
                    TID = data.TID,
                    MID = data.MID,
                    MerchantName = data.MerchantName,
                    MerchantAddress = data.MerchantAddress,
                    TerminalID = data.EffectiveTerminalID,
                    TerminalSerialNo = data.EffectiveTerminalSN,
                    SimID = data.EffectiveSIMID,
                    SimSerialNo = data.EffectiveSIMSN
                }
            };
        }

        public void Save(QRDeliverySaveRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");

            string values = string.Format(CultureInfo.InvariantCulture,
                "({0},{1},{2},{3},{4},{5},{6},{7},{8})",
                request.ServiceNo,
                request.IRIDNo,
                request.MerchantID,
                Quote(request.CreatedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                Quote(request.QRContent),
                Quote(request.ProcessedBy),
                Quote(request.InventoryStatus),
                Quote(request.TerminalPrepStatus),
                Quote(request.DispatcherStatus));

            api.ExecuteAPI(
                "POST",
                "Insert",
                "",
                "",
                "QR Delivery Detail",
                values,
                "InsertCollectionDetail");

            EnsureSuccess("saving the QR Delivery record");

            QRDeliverySaveApiResponse response =
                JsonConvert.DeserializeObject<QRDeliverySaveApiResponse>(
                    clsGlobalVariables.strJSONResponse);
            if (response == null || response.Data == null || response.Data.Count == 0 ||
                !response.Data[0].LastInsertID.HasValue || response.Data[0].LastInsertID.Value <= 0)
            {
                throw new InvalidOperationException(
                    "The MIS API responded successfully, but no QR Delivery audit record was inserted.");
            }
        }

        public IList<QRDeliveryHistoryItem> GetRecent(int serviceNo, int limit)
        {
            if (serviceNo < 0) throw new ArgumentOutOfRangeException("serviceNo");
            if (limit <= 0 || limit > 50) throw new ArgumentOutOfRangeException("limit");

            api.ExecuteAPI(
                "GET",
                "View",
                "QR Delivery History",
                serviceNo.ToString(CultureInfo.InvariantCulture),
                "Advance Detail",
                "",
                "ViewAdvanceDetail");

            EnsureSuccess("loading QR Delivery history");
            QRDeliveryApiResponse response =
                JsonConvert.DeserializeObject<QRDeliveryApiResponse>(clsGlobalVariables.strJSONResponse);
            return response == null || response.Data == null
                ? new List<QRDeliveryHistoryItem>()
                : response.Data;
        }

        private static string Quote(string value)
        {
            return "'" + MySqlHelper.EscapeString(value ?? string.Empty) + "'";
        }

        private static void EnsureSuccess(string operation)
        {
            if (!clsGlobalVariables.isAPIResponseOK ||
                !string.Equals(clsGlobalVariables.sAPIResponseCode,
                    clsGlobalVariables.SUCCESS_RESPONSE, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("MIS API failed while " + operation + ".");
            }
        }

        private sealed class QRDeliveryApiResponse
        {
            [JsonProperty("resp_code")]
            public string ResponseCode { get; set; }

            [JsonProperty("data")]
            public List<QRDeliveryHistoryItem> Data { get; set; }
        }

        private sealed class QRDeliverySaveApiResponse
        {
            [JsonProperty("data")]
            public List<QRDeliverySaveApiData> Data { get; set; }
        }

        private sealed class QRDeliverySaveApiData
        {
            [JsonProperty("LastInsertID")]
            public int? LastInsertID { get; set; }
        }

        private sealed class LookupJson
        {
            public int ServiceNo { get; set; }
            public int IRIDNo { get; set; }
            public int MerchantID { get; set; }
            public int JobType { get; set; }
            public string JobTypeDescription { get; set; }
            public string pJobTypeDescription { get; set; }
            public string JobTypeStatusDescription { get; set; }
            public string MerchantName { get; set; }
            public string MerchantAddress { get; set; }
            public string TID { get; set; }
            public string MID { get; set; }
            public string TerminalSN { get; set; }
            public int TerminalID { get; set; }
            public string TerminalStatus { get; set; }
            public string pTerminalStatus { get; set; }
            public int ReplaceTerminalID { get; set; }
            public string ReplaceTerminalSN { get; set; }
            public string ReplaceTerminalStatus { get; set; }
            public string pReplaceTerminalStatus { get; set; }
            public int SIMID { get; set; }
            public string SIMSerialNo { get; set; }
            public string SIMStatus { get; set; }
            public string pSIMStatus { get; set; }
            public int ReplaceSIMID { get; set; }
            public string ReplaceSIMSN { get; set; }
            public string ReplaceSIMStatus { get; set; }
            public string pReplaceSIMStatus { get; set; }

            public bool IsReplacement { get { return JobType == 7; } }
            public int EffectiveTerminalID { get { return IsReplacement ? ReplaceTerminalID : TerminalID; } }
            public string EffectiveTerminalSN { get { return IsReplacement ? ReplaceTerminalSN : TerminalSN; } }
            public string EffectiveTerminalStatus
            {
                get
                {
                    return IsReplacement
                ? First(ReplaceTerminalStatus, pReplaceTerminalStatus)
                : First(TerminalStatus, pTerminalStatus);
                }
            }
            public int EffectiveSIMID { get { return IsReplacement ? ReplaceSIMID : SIMID; } }
            public string EffectiveSIMSN { get { return IsReplacement ? ReplaceSIMSN : SIMSerialNo; } }
            public string EffectiveSimStatus
            {
                get
                {
                    return IsReplacement
                ? First(ReplaceSIMStatus, pReplaceSIMStatus)
                : First(SIMStatus, pSIMStatus);
                }
            }
            private static string First(string first, string second)
            {
                return string.IsNullOrWhiteSpace(first) ? second : first;
            }
        }
    }
}
