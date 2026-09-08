using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Windows.Forms;
using MIS.Controller;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace MIS
{
    internal static class QRDeliveryWaybillReport
    {
        private const string ReportPath = @"C:\CASTLESTECH_MIS\REPORTS\";
        private const string ReportFileName = "rptQRDeliveryWaybill.rpt";

        public static void ShowPreview(IWin32Window owner, ServicingDetailController service,
            string internalQRContent, DateTime qrDate)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (string.IsNullOrWhiteSpace(internalQRContent))
                throw new ArgumentException("The internal QR content is required.", "internalQRContent");

            string reportPath = ResolveReportPath();
            if (!File.Exists(reportPath))
                throw new FileNotFoundException("The QR Delivery waybill template was not found.", reportPath);

            ReportDocument report = new ReportDocument();
            try
            {
                report.Load(reportPath);
                IDictionary<string, object> values = CreateValues(service, internalQRContent);
                AddBankReportText(values, qrDate);

                if (report.Database.Tables.Count > 0)
                    report.SetDataSource(CreateDataSource(values));

                BindParameters(report, values);
                BindTextObjects(report, values);

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
            }
        }

        private static string ResolveReportPath()
        {
            string deployedPath = Path.Combine(ReportPath, ReportFileName);
            if (File.Exists(deployedPath))
                return deployedPath;

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Report", ReportFileName);
        }

        private static IDictionary<string, object> CreateValues(
            ServicingDetailController service, string internalQRContent)
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
            Add(values, internalQRContent, "internalqrcontent");
            return values;
        }

        private static void AddBankReportText(IDictionary<string, object> values, DateTime qrDate)
        {
            clsBank bank = GetCurrentBankSettings();
            if (bank == null)
                return;

            AddIfConfigured(values, bank.Hotline1, "txtHotLine1");
            AddIfConfigured(values, bank.Hotline2, "txtHotLine2");
            AddIfConfigured(values, bank.Hotline3, "txtHotLine3");
            AddIfConfigured(values, ReplaceQRDate(bank.Warranty, qrDate), "txtWarranty");
        }

        private static string ReplaceQRDate(string template, DateTime qrDate)
        {
            if (string.IsNullOrWhiteSpace(template))
                return template;

            return template.Replace("<QRDate>",
                qrDate.ToString("MMM. d, yyyy", CultureInfo.InvariantCulture));
        }

        private static void AddIfConfigured(IDictionary<string, object> values, string value,
            string reportObjectName)
        {
            if (!string.IsNullOrWhiteSpace(value))
                Add(values, value, reportObjectName);
        }

        private static clsBank GetCurrentBankSettings()
        {
            string settingsFile = Path.Combine(new clsFile().sSettingPath,
                clsDefines.RESP_BANKLIST_FILENAME);
            if (!File.Exists(settingsFile))
                return null;

            IList<clsBank> banks = new clsFunction().loadBankList(settingsFile);
            foreach (clsBank bank in banks)
            {
                if (string.Equals(bank.Code, clsSearch.ClassBankCode,
                    StringComparison.OrdinalIgnoreCase))
                    return bank;
            }

            return null;
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
