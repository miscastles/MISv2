using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace MIS
{
    /// <summary>
    /// Development-only persistence adapter. It deliberately refuses remote hosts
    /// and any schema other than the isolated QR Delivery local database.
    /// Production must use the approved MIS API adapter.
    /// </summary>
    public sealed class LocalMySqlQRDeliveryHistoryStore : IQRDeliveryHistoryStore, IQRDeliveryLookupStore
    {
        private const string ExpectedDatabase = "miscastlesdb_qr_local";
        private readonly string connectionString;

        public LocalMySqlQRDeliveryHistoryStore(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("A local QR database connection string is required.", "connectionString");

            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(connectionString);
            if (!IsLocalHost(builder.Server))
                throw new InvalidOperationException("QR local adapter refuses non-local database hosts.");
            if (!string.Equals(builder.Database, ExpectedDatabase, StringComparison.Ordinal))
                throw new InvalidOperationException("QR local adapter requires database " + ExpectedDatabase + ".");

            builder.Pooling = true;
            builder.AllowUserVariables = false;
            builder["AllowPublicKeyRetrieval"] = true;
            this.connectionString = builder.ConnectionString;
        }

        public QRDeliveryLookupResult FindJobOrder(string tid, string mid)
        {
            if (string.IsNullOrWhiteSpace(tid) || string.IsNullOrWhiteSpace(mid))
                return new QRDeliveryLookupResult { Found = false };

            const string sql = "CALL spGetInfoDetail('Search', 'QR Delivery', @SearchValue, @OutResult);";
            using (MySqlConnection connection = OpenVerifiedConnection())
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.Add("@SearchValue", MySqlDbType.VarChar, 128).Value =
                    tid.Trim() + "|" + mid.Trim();
                MySqlParameter outResult = command.Parameters.Add("@OutResult", MySqlDbType.LongText);
                outResult.Direction = System.Data.ParameterDirection.Output;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read()) return new QRDeliveryLookupResult { Found = false };
                    string json = reader.GetString("detail_info");
                    LocalLookupJson data = Newtonsoft.Json.JsonConvert.DeserializeObject<LocalLookupJson>(json);
                    if (data == null) return new QRDeliveryLookupResult { Found = false };
                    return new QRDeliveryLookupResult
                    {
                        Found = true,
                        ServiceNo = data.ServiceNo,
                        IRIDNo = data.IRIDNo,
                        MerchantID = data.MerchantID,
                        JobType = data.JobType,
                        JobTypeDescription = data.JobTypeDescription,
                        Expected = new QRDeliveryData
                        {
                            TID = data.TID,
                            MID = data.MID,
                            MerchantName = data.MerchantName,
                            MerchantAddress = data.MerchantAddress,
                            TerminalSerialNo = data.TerminalSN,
                            SimSerialNo = data.SIMSerialNo
                        }
                    };
                }
            }
        }

        public void Save(QRDeliverySaveRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");

            const string sql = @"
INSERT INTO tblservicingqrdetail
    (ServiceNo, IRIDNo, MerchantID, QRDate, QRContent, ProcessedBy,
     InventoryStatus, TerminalPrepStatus, DispatcherStatus)
VALUES
    (@ServiceNo, @IRIDNo, @MerchantID, @QRDate, @QRContent, @ProcessedBy,
     @InventoryStatus, @TerminalPrepStatus, @DispatcherStatus);";

            using (MySqlConnection connection = OpenVerifiedConnection())
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.Add("@ServiceNo", MySqlDbType.Int32).Value = request.ServiceNo;
                command.Parameters.Add("@IRIDNo", MySqlDbType.Int32).Value = request.IRIDNo;
                command.Parameters.Add("@MerchantID", MySqlDbType.Int32).Value = request.MerchantID;
                command.Parameters.Add("@QRDate", MySqlDbType.Date).Value = request.CreatedDate.Date;
                command.Parameters.Add("@QRContent", MySqlDbType.Text).Value = request.QRContent;
                command.Parameters.Add("@ProcessedBy", MySqlDbType.VarChar, 255).Value = request.ProcessedBy;
                command.Parameters.Add("@InventoryStatus", MySqlDbType.VarChar, 45).Value = request.InventoryStatus;
                command.Parameters.Add("@TerminalPrepStatus", MySqlDbType.VarChar, 45).Value = request.TerminalPrepStatus;
                command.Parameters.Add("@DispatcherStatus", MySqlDbType.VarChar, 45).Value = request.DispatcherStatus;

                if (command.ExecuteNonQuery() != 1)
                    throw new InvalidOperationException("QR Delivery save did not insert exactly one record.");
            }
        }

        public IList<QRDeliveryHistoryItem> GetRecent(int serviceNo, int limit)
        {
            const string sql = @"
SELECT QRID, ServiceNo, IRIDNo, MerchantID, InventoryStatus,
       TerminalPrepStatus, DispatcherStatus, ProcessedBy, QRDate, DateTimeStamp
FROM tblservicingqrdetail
WHERE (@ServiceNo = 0 OR ServiceNo = @ServiceNo)
ORDER BY DateTimeStamp DESC, QRID DESC
LIMIT @Limit;";

            List<QRDeliveryHistoryItem> items = new List<QRDeliveryHistoryItem>();
            using (MySqlConnection connection = OpenVerifiedConnection())
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.Add("@ServiceNo", MySqlDbType.Int32).Value = serviceNo;
                command.Parameters.Add("@Limit", MySqlDbType.Int32).Value = limit;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new QRDeliveryHistoryItem
                        {
                            QRID = reader.GetInt32("QRID"),
                            ServiceNo = reader.GetInt32("ServiceNo"),
                            IRIDNo = reader.GetInt32("IRIDNo"),
                            MerchantID = reader.GetInt32("MerchantID"),
                            InventoryStatus = reader.GetString("InventoryStatus"),
                            TerminalPrepStatus = reader.GetString("TerminalPrepStatus"),
                            DispatcherStatus = reader.GetString("DispatcherStatus"),
                            ProcessedBy = reader.GetString("ProcessedBy"),
                            QRDate = reader.GetDateTime("QRDate"),
                            DateTimeStamp = reader.GetDateTime("DateTimeStamp")
                        });
                    }
                }
            }
            return items;
        }

        private MySqlConnection OpenVerifiedConnection()
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            using (MySqlCommand command = new MySqlCommand(
                "SELECT DATABASE(), @@global.event_scheduler;", connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (!reader.Read() ||
                    !string.Equals(reader.GetString(0), ExpectedDatabase, StringComparison.Ordinal) ||
                    !string.Equals(reader.GetString(1), "OFF", StringComparison.OrdinalIgnoreCase))
                {
                    connection.Dispose();
                    throw new InvalidOperationException("QR local database safety verification failed.");
                }
            }
            return connection;
        }

        private static bool IsLocalHost(string host)
        {
            return string.Equals(host, "127.0.0.1", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(host, "::1", StringComparison.OrdinalIgnoreCase);
        }

        private sealed class LocalLookupJson
        {
            public int ServiceNo { get; set; }
            public int IRIDNo { get; set; }
            public int MerchantID { get; set; }
            public int JobType { get; set; }
            public string JobTypeDescription { get; set; }
            public string MerchantName { get; set; }
            public string MerchantAddress { get; set; }
            public string TID { get; set; }
            public string MID { get; set; }
            public string TerminalSN { get; set; }
            public string SIMSerialNo { get; set; }
        }
    }
}
