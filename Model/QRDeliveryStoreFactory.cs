using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;

namespace MIS
{
    internal static class QRDeliveryStoreFactory
    {
        private const string AppUserName = "qr_delivery_app";

        public static bool TryCreateLocal(out IQRDeliveryHistoryStore store)
        {
            store = null;
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "CITAS", "MIS", "qr_delivery_local.dat");
            if (!File.Exists(path)) return false;

            byte[] protectedBytes = Convert.FromBase64String(File.ReadAllText(path).Trim());
            byte[] plainBytes = ProtectedData.Unprotect(
                protectedBytes, null, DataProtectionScope.CurrentUser);
            try
            {
                string password = Encoding.UTF8.GetString(plainBytes);
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
                {
                    Server = "127.0.0.1",
                    Port = 3306,
                    Database = "miscastlesdb_qr_local",
                    UserID = AppUserName,
                    Password = password,
                    SslMode = MySqlSslMode.None,
                    ConnectionTimeout = 5
                };
                builder["AllowPublicKeyRetrieval"] = true;
                store = new LocalMySqlQRDeliveryHistoryStore(builder.ConnectionString);
                return true;
            }
            finally
            {
                Array.Clear(plainBytes, 0, plainBytes.Length);
            }
        }
    }
}
