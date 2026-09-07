using System;

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
            if (request.InventoryStatus != "VALID" && request.InventoryStatus != "INVALID")
                throw new InvalidOperationException("Inventory status must be VALID or INVALID.");
            if (request.DispatcherStatus != "VALID" && request.DispatcherStatus != "INVALID")
                throw new InvalidOperationException(
                    "Dispatcher status must be VALID or INVALID.");
            if (request.QRResult != "READY TO DISPATCH" &&
                request.QRResult != "NOT READY TO DISPATCH")
                throw new InvalidOperationException(
                    "QR result must be READY TO DISPATCH or NOT READY TO DISPATCH.");
            if (string.IsNullOrWhiteSpace(request.ProcessedBy)) throw new InvalidOperationException("Processed By is required.");
            if (request.CreatedDate == DateTime.MinValue) request.CreatedDate = DateTime.Now;

            store.Save(request);
        }

        public void SaveValidated(QRDeliverySaveRequest request)
        {
            SaveValidation(request);
        }

    }
}
