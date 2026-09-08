using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MIS
{
    public sealed partial class frmQRDeliveryHistory : Form
    {
        public frmQRDeliveryHistory()
        {
            InitializeComponent();

            // Keep behavior in the code-behind so Designer-only changes do not
            // remove the window controls' event wiring.
            btnMinimize.Click += btnMinimize_Click;
            btnExit.Click += btnExit_Click;
        }

        public frmQRDeliveryHistory(IList<QRDeliveryHistoryItem> items, bool sessionOnly)
            : this()
        {
            lblDescription.Text = sessionOnly
                ? "VALIDATION HISTORY (LOCAL AUDIT COPY)"
                : "VALIDATION HISTORY";

            LoadRows(items);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        // Kept for the Click event assigned to the title label in the
        // WinForms Designer. Dragging is handled by bunifuDragControl2.
        private void lblHeader_Click(object sender, EventArgs e)
        {
        }

        private void LoadRows(IList<QRDeliveryHistoryItem> items)
        {
            dgvHistory.Rows.Clear();

            if (items != null)
                foreach (QRDeliveryHistoryItem item in items)
                {
                    // Hide legacy/unusable scans that were saved without any
                    // service, IR, or merchant reference.
                    if (item.ServiceNo == 0 && item.IRIDNo == 0 && item.MerchantID == 0)
                        continue;

                    int rowIndex = dgvHistory.Rows.Add();
                    DataGridViewRow row = dgvHistory.Rows[rowIndex];

                    // Assign by the Designer column names so the functionality
                    // remains correct when the visual column order is changed.
                    row.Cells["DateTime"].Value = item.DateTimeStamp.ToString("yyyy-MM-dd HH:mm:ss");
                    row.Cells["Result"].Value = OverallResult(item);
                    row.Cells["ServiceNo"].Value = item.ServiceNo;
                    row.Cells["IRIDNo"].Value = item.IRIDNo;
                    row.Cells["MerchantID"].Value = item.MerchantID;
                    row.Cells["InventoryStatus"].Value = item.InventoryStatus;
                    row.Cells["TerminalPrepStatus"].Value = item.TerminalPrepStatus;
                    row.Cells["DispatcherStatus"].Value = item.DispatcherStatus;
                    row.Cells["ProcessedBy"].Value = item.ProcessedBy;

                    ColorStatusCell(row.Cells["InventoryStatus"]);
                    ColorStatusCell(row.Cells["TerminalPrepStatus"]);
                    ColorStatusCell(row.Cells["DispatcherStatus"]);
                    ColorStatusCell(row.Cells["Result"]);
                }

            lblRecordCount.Text = dgvHistory.Rows.Count == 0
                ? "NO HISTORY RECORDS FOUND"
                : string.Format("{0} RECORD(S) SHOWN", dgvHistory.Rows.Count);
        }

        private static string OverallResult(QRDeliveryHistoryItem item)
        {
            if (string.Equals(item.DispatcherStatus, "NO JO", StringComparison.OrdinalIgnoreCase))
                return "NO JO";
            if (string.Equals(item.DispatcherStatus, "INVALID QR", StringComparison.OrdinalIgnoreCase))
                return "INVALID QR";
            if (!string.Equals(item.InventoryStatus, "VALID", StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(item.TerminalPrepStatus, "VALID", StringComparison.OrdinalIgnoreCase))
                return "MISMATCH";
            if (!string.Equals(item.DispatcherStatus, "VALID", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(item.DispatcherStatus, "DISPATCH", StringComparison.OrdinalIgnoreCase))
                return "NOT YET DISPATCH";
            return "MATCH";
        }

        private static void ColorStatusCell(DataGridViewCell cell)
        {
            string value = Convert.ToString(cell.Value);
            bool valid = string.Equals(value, "VALID", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(value, "DISPATCH", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(value, "MATCH", StringComparison.OrdinalIgnoreCase);
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cell.Style.BackColor = valid ? Color.Green : Color.Firebrick;
            cell.Style.ForeColor = Color.White;
            cell.Style.SelectionBackColor = cell.Style.BackColor;
            cell.Style.SelectionForeColor = Color.White;
        }
    }
}
