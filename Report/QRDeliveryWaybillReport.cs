using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Windows.Forms;
using MIS.Controller;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using QRCoder;

namespace MIS
{
    internal static class QRDeliveryWaybillReport
    {
        private const string ReportFileName = "rptQRDeliveryWaybill.rpt";

        public static void ShowPreview(IWin32Window owner, ServicingDetailController service,
            string internalQRContent)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (string.IsNullOrWhiteSpace(internalQRContent))
                throw new ArgumentException("The internal QR content is required.", "internalQRContent");

            string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Report", ReportFileName);
            if (!File.Exists(reportPath))
                throw new FileNotFoundException("The QR Delivery waybill template was not found.", reportPath);

            ReportDocument report = new ReportDocument();
            string qrImagePath = null;
            try
            {
                report.Load(reportPath);
                IDictionary<string, object> values = CreateValues(service);

                if (report.Database.Tables.Count > 0)
                    report.SetDataSource(CreateDataSource(values));

                BindParameters(report, values);
                BindTextObjects(report, values);
                // The report's current QR is an approved placeholder. The senior
                // developer will provide the data-bound QR object in the final RPT.

                QRDeliveryReportPreview preview = new QRDeliveryReportPreview(report);
                report = null; // the preview owns and disposes the report
                preview.ShowDialog(owner);
            }
            finally
            {
                if (report != null)
                {
                    report.Close();
                    report.Dispose();
                }
                if (!string.IsNullOrWhiteSpace(qrImagePath) && File.Exists(qrImagePath))
                    File.Delete(qrImagePath);
            }
        }

        private static string CreateQrImage(string content)
        {
            string path = Path.Combine(Path.GetTempPath(),
                "MIS_QR_DELIVERY_" + Guid.NewGuid().ToString("N") + ".bmp");
            using (QRCodeGenerator generator = new QRCodeGenerator())
            using (QRCodeData data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q))
            using (QRCode qrCode = new QRCode(data))
            using (Bitmap bitmap = qrCode.GetGraphic(12))
                bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
            return path;
        }

        private static void BindQrPicture(ReportDocument report, string imagePath)
        {
            CrystalDecisions.ReportAppServer.Controllers.ReportObjectController controller =
                report.ReportClientDocument.ReportDefController.ReportObjectController;
            CrystalDecisions.ReportAppServer.ReportDefModel.ReportObjects objects =
                controller.GetAllReportObjects();

            foreach (CrystalDecisions.ReportAppServer.ReportDefModel.ISCRReportObject item in objects)
            {
                if (!string.Equals(item.Name, "Picture5", StringComparison.OrdinalIgnoreCase))
                    continue;

                CrystalDecisions.ReportAppServer.ReportDefModel.ISCRPictureObject picture =
                    item as CrystalDecisions.ReportAppServer.ReportDefModel.ISCRPictureObject;
                if (picture == null) break;

                CrystalDecisions.ReportAppServer.CommonObjectModel.ByteArray imageData =
                    new CrystalDecisions.ReportAppServer.CommonObjectModel.ByteArrayClass();
                imageData.ByteArray = File.ReadAllBytes(imagePath);
                picture.PictureData = imageData;
                picture.PictureType =
                    CrystalDecisions.ReportAppServer.ReportDefModel.CrPictureTypeEnum.crPictureTypeBitmap;
                controller.Modify(item, picture);
                return;
            }

            throw new InvalidOperationException(
                "The QR picture placeholder (Picture5) was not found in the waybill report.");
        }

        private static IDictionary<string, object> CreateValues(
            ServicingDetailController service)
        {
            Dictionary<string, object> values = new Dictionary<string, object>(
                StringComparer.OrdinalIgnoreCase);

            Add(values, service.MerchantName,
                "MerchantName", "Merchant", "DBAName", "txtMerchantName");
            Add(values, service.Address,
                "MerchantAddress", "Address", "MerchantLocation", "txtAddress");
            Add(values, service.TerminalSN,
                "POSSN", "POSSerialNumber", "POSSerialNo", "TerminalSerialNumber", "TerminalSerialNo",
                "TerminalSN", "txtPOSSerialNumber", "txtTerminalSN");
            Add(values, service.SIMSN,
                "SIMSerialNumber", "SIMSerialNo", "SIMSN", "txtSIMSerialNumber", "txtSIMSN");
            Add(values, service.TID, "TID", "TerminalID", "txtTID");
            Add(values, service.MID, "MID", "MerchantIDNumber", "txtMID");
            Add(values, service.ServiceNo, "ServiceNo", "JobOrderNo");
            Add(values, service.IRIDNo, "IRIDNo", "IRNo");
            return values;
        }

        private static void Add(IDictionary<string, object> values, object value,
            params string[] names)
        {
            foreach (string name in names)
                values[Normalize(name)] = value ?? string.Empty;
        }

        private static DataTable CreateDataSource(IDictionary<string, object> values)
        {
            DataTable table = new DataTable("QRDeliveryWaybill");
            foreach (KeyValuePair<string, object> pair in values)
            {
                Type type = pair.Value == null ? typeof(string) : pair.Value.GetType();
                table.Columns.Add(pair.Key, type);
            }

            DataRow row = table.NewRow();
            foreach (KeyValuePair<string, object> pair in values)
                row[pair.Key] = pair.Value ?? string.Empty;
            table.Rows.Add(row);
            return table;
        }

        private static void BindParameters(ReportDocument report,
            IDictionary<string, object> values)
        {
            foreach (ParameterFieldDefinition parameter in report.DataDefinition.ParameterFields)
            {
                object value;
                if (values.TryGetValue(Normalize(parameter.Name), out value))
                    report.SetParameterValue(parameter.Name, value);
            }
        }

        private static void BindTextObjects(ReportDocument report,
            IDictionary<string, object> values)
        {
            foreach (Section section in report.ReportDefinition.Sections)
            {
                foreach (ReportObject reportObject in section.ReportObjects)
                {
                    TextObject text = reportObject as TextObject;
                    if (text == null) continue;

                    object value;
                    if (values.TryGetValue(Normalize(text.Name), out value))
                        text.Text = Convert.ToString(value);
                }
            }
        }

        private static string Normalize(string value)
        {
            StringBuilder normalized = new StringBuilder();
            foreach (char character in value ?? string.Empty)
                if (char.IsLetterOrDigit(character))
                    normalized.Append(char.ToLowerInvariant(character));
            return normalized.ToString();
        }

        private sealed class QRDeliveryReportPreview : Form
        {
            private readonly ReportDocument report;

            public QRDeliveryReportPreview(ReportDocument report)
            {
                this.report = report;
                Text = "QR Delivery Waybill Preview";
                WindowState = FormWindowState.Maximized;

                CrystalReportViewer viewer = new CrystalReportViewer
                {
                    Dock = DockStyle.Fill,
                    ToolPanelView = ToolPanelViewType.None,
                    ReportSource = report
                };
                Controls.Add(viewer);
                FormClosed += PreviewFormClosed;
            }

            private void PreviewFormClosed(object sender, FormClosedEventArgs e)
            {
                report.Close();
                report.Dispose();
            }
        }
    }
}
