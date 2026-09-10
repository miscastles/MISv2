using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MIS
{
    /// <summary>
    /// Durable client-side copy of the QR Delivery audit index. The authoritative
    /// record is still saved through the MIS API; this copy keeps history visible
    /// across application restarts when the live history endpoint returns only a
    /// filtered subset.
    /// </summary>
    public static class QRDeliveryHistoryCache
    {
        private static readonly object SyncRoot = new object();

        private static string CachePath
        {
            get
            {
                return Path.Combine(ApplicationDataPath(), "qr-delivery-history.json");
            }
        }

        public static IList<QRDeliveryHistoryItem> Load()
        {
            lock (SyncRoot)
            {
                try
                {
                    if (!File.Exists(CachePath))
                        return new List<QRDeliveryHistoryItem>();

                    List<QRDeliveryHistoryItem> items =
                        JsonConvert.DeserializeObject<List<QRDeliveryHistoryItem>>(
                            File.ReadAllText(CachePath));
                    return items ?? new List<QRDeliveryHistoryItem>();
                }
                catch
                {
                    // A bad local cache must never prevent QR validation.
                    return new List<QRDeliveryHistoryItem>();
                }
            }
        }

        public static void Append(QRDeliveryHistoryItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            lock (SyncRoot)
            {
                List<QRDeliveryHistoryItem> items = new List<QRDeliveryHistoryItem>(Load());
                items.Insert(0, item);

                string directory = Path.GetDirectoryName(CachePath);
                Directory.CreateDirectory(directory);
                File.WriteAllText(CachePath, JsonConvert.SerializeObject(items));
            }
        }

        private static string ApplicationDataPath()
        {
            string root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(root, "Castles Technology", "MIS", "QR Delivery");
        }
    }
}
