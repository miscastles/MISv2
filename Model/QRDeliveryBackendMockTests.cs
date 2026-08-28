using System;
using System.Collections.Generic;

namespace MIS
{
    internal static class QRDeliveryBackendMockTests
    {
        public static void Run()
        {
            InMemoryQRDeliveryHistoryStore store = new InMemoryQRDeliveryHistoryStore();
            QRDeliveryBackendService service = new QRDeliveryBackendService(store);
            service.SaveValidated(new QRDeliverySaveRequest
            {
                ServiceNo = 68902,
                IRIDNo = 49876,
                MerchantID = 544175,
                QRContent = "{\"tid\":\"TEST\"}",
                InventoryStatus = "MATCH",
                TerminalPrepStatus = string.Empty,
                DispatcherStatus = string.Empty,
                ProcessedBy = "MOCK USER",
                CreatedDate = new DateTime(2026, 8, 17, 12, 0, 0)
            });

            IList<QRDeliveryHistoryItem> history = service.GetRecentHistory(68902, 20);
            if (history.Count != 1 || history[0].ServiceNo != 68902)
                throw new InvalidOperationException("QR backend save/history mock check failed.");

            service.SaveValidation(new QRDeliverySaveRequest
            {
                ServiceNo = 0,
                IRIDNo = 0,
                MerchantID = 0,
                QRContent = "{}",
                InventoryStatus = "MISMATCH",
                TerminalPrepStatus = string.Empty,
                DispatcherStatus = string.Empty,
                ProcessedBy = "MOCK USER",
                CreatedDate = DateTime.Now
            });
            history = service.GetRecentHistory(0, 20);
            if (history.Count != 2 || history[0].InventoryStatus != "MISMATCH")
                throw new InvalidOperationException("Failed QR validation audit save check failed.");

            service.SaveValidation(new QRDeliverySaveRequest
            {
                ServiceNo = 0,
                IRIDNo = 0,
                MerchantID = 0,
                QRContent = "not-json",
                InventoryStatus = "MISMATCH",
                TerminalPrepStatus = string.Empty,
                DispatcherStatus = string.Empty,
                ProcessedBy = "MOCK USER",
                CreatedDate = DateTime.Now
            });
            history = service.GetRecentHistory(0, 20);
            if (history.Count != 3 || history[0].InventoryStatus != "MISMATCH")
                throw new InvalidOperationException("Invalid QR audit save check failed.");
        }
    }
}
