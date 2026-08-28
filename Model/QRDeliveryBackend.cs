using System;
using System.Collections.Generic;
using System.Linq;

namespace MIS
{
    public sealed class QRDeliveryBackendService
    {
        private readonly IQRDeliveryHistoryStore store;

        public QRDeliveryBackendService(IQRDeliveryHistoryStore store)
        {
            if (store == null) throw new ArgumentNullException("store");
            this.store = store;
        }

        public void SaveValidation(QRDeliverySaveRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (string.IsNullOrWhiteSpace(request.QRContent)) throw new InvalidOperationException("QR content is required.");
            if (request.InventoryStatus != "MATCH" && request.InventoryStatus != "MISMATCH")
                throw new InvalidOperationException("Inventory status must be MATCH or MISMATCH.");
            if (string.IsNullOrWhiteSpace(request.ProcessedBy)) throw new InvalidOperationException("Processed By is required.");
            if (request.CreatedDate == DateTime.MinValue) request.CreatedDate = DateTime.Now;

            store.Save(request);
        }

        public void SaveValidated(QRDeliverySaveRequest request)
        {
            SaveValidation(request);
        }

        public IList<QRDeliveryHistoryItem> GetRecentHistory(int serviceNo, int limit)
        {
            if (serviceNo < 0) throw new ArgumentOutOfRangeException("serviceNo");
            if (limit <= 0 || limit > 200) throw new ArgumentOutOfRangeException("limit");
            return store.GetRecent(serviceNo, limit);
        }
    }

    public sealed class InMemoryQRDeliveryHistoryStore : IQRDeliveryHistoryStore
    {
        private readonly List<QRDeliveryHistoryItem> items = new List<QRDeliveryHistoryItem>();
        private int nextId = 1;

        public void Save(QRDeliverySaveRequest request)
        {
            items.Add(new QRDeliveryHistoryItem
            {
                QRID = nextId++,
                ServiceNo = request.ServiceNo,
                IRIDNo = request.IRIDNo,
                MerchantID = request.MerchantID,
                InventoryStatus = request.InventoryStatus,
                TerminalPrepStatus = request.TerminalPrepStatus,
                DispatcherStatus = request.DispatcherStatus,
                ProcessedBy = request.ProcessedBy,
                QRDate = request.CreatedDate.Date,
                DateTimeStamp = request.CreatedDate
            });
        }

        public IList<QRDeliveryHistoryItem> GetRecent(int serviceNo, int limit)
        {
            IEnumerable<QRDeliveryHistoryItem> query = items;
            if (serviceNo > 0) query = query.Where(item => item.ServiceNo == serviceNo);
            return query.OrderByDescending(item => item.DateTimeStamp).Take(limit).ToList();
        }
    }
}
