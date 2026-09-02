using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MIS
{
    public sealed class frmQRDeliveryHistory : Form
    {
        private readonly DataGridView dgvHistory;
        private readonly Label lblRecordCount;

        public frmQRDeliveryHistory(IList<QRDeliveryHistoryItem> items, bool sessionOnly)
        {
            Text = "QR Delivery History";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(920, 500);
            BackColor = Color.FromArgb(247, 247, 247);
            Font = new Font("Courier New", 9.25F);

            Panel header = new Panel
            {
                BackColor = Color.DodgerBlue,
                Dock = DockStyle.Top,
                Height = 34
            };
            Label title = new Label
            {
                AutoSize = true,
                Font = new Font("Century Gothic", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 7),
                Text = "QR DELIVERY HISTORY"
            };
            header.Controls.Add(title);

            Label description = new Label
            {
                AutoSize = true,
                ForeColor = Color.Black,
                Location = new Point(15, 48),
                Text = sessionOnly
                    ? "RECENT VALIDATION HISTORY (CURRENT SESSION)"
                    : "RECENT VALIDATION HISTORY"
            };

            dgvHistory = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersHeight = 30,
                EnableHeadersVisualStyles = false,
                Location = new Point(15, 72),
                MultiSelect = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                RowTemplate = { Height = 25 },
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Size = new Size(890, 370)
            };
            dgvHistory.ColumnHeadersDefaultCellStyle.BackColor = Color.DodgerBlue;
            dgvHistory.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvHistory.ColumnHeadersDefaultCellStyle.Font =
                new Font("Courier New", 9.25F, FontStyle.Bold);
            dgvHistory.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgvHistory.DefaultCellStyle.SelectionBackColor = Color.FromArgb(205, 225, 245);
            dgvHistory.DefaultCellStyle.SelectionForeColor = Color.Navy;

            AddColumn("DateTime", "DATE / TIME", 135);
            AddColumn("Result", "RESULT", 95);
            AddColumn("ServiceNo", "SERVICE NO.", 80);
            AddColumn("IRIDNo", "IR ID NO.", 75);
            AddColumn("MerchantID", "MERCHANT ID", 80);
            AddColumn("InventoryStatus", "INVENTORY", 85);
            AddColumn("TerminalPrepStatus", "TERMINAL PREP", 95);
            AddColumn("DispatcherStatus", "DISPATCHER", 105);
            AddColumn("ProcessedBy", "PROCESSED BY", 105);

            lblRecordCount = new Label
            {
                AutoSize = true,
                ForeColor = Color.Navy,
                Location = new Point(15, 458)
            };

            Button btnClose = new Button
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Font = new Font("Arial Narrow", 9.25F, FontStyle.Bold),
                Location = new Point(805, 452),
                Size = new Size(100, 30),
                Text = "CLOSE",
                UseVisualStyleBackColor = true
            };
            btnClose.Click += delegate { Close(); };
            AcceptButton = btnClose;
            CancelButton = btnClose;

            Controls.Add(btnClose);
            Controls.Add(lblRecordCount);
            Controls.Add(dgvHistory);
            Controls.Add(description);
            Controls.Add(header);

            LoadRows(items);
        }

        private void AddColumn(string name, string headerText, float fillWeight)
        {
            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = headerText,
                FillWeight = fillWeight,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.Automatic
            });
        }

        private void LoadRows(IList<QRDeliveryHistoryItem> items)
        {
            if (items != null)
                foreach (QRDeliveryHistoryItem item in items)
                {
                    int rowIndex = dgvHistory.Rows.Add(
                        item.DateTimeStamp.ToString("yyyy-MM-dd HH:mm:ss"),
                        OverallResult(item),
                        item.ServiceNo,
                        item.IRIDNo,
                        item.MerchantID,
                        item.InventoryStatus,
                        item.TerminalPrepStatus,
                        item.DispatcherStatus,
                        item.ProcessedBy);
                    ColorStatusCell(dgvHistory.Rows[rowIndex].Cells[5]);
                    ColorStatusCell(dgvHistory.Rows[rowIndex].Cells[6]);
                    ColorStatusCell(dgvHistory.Rows[rowIndex].Cells[7]);
                    ColorStatusCell(dgvHistory.Rows[rowIndex].Cells[1]);
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
            if (!string.Equals(item.DispatcherStatus, "DISPATCH", StringComparison.OrdinalIgnoreCase))
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
