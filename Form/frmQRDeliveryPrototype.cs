using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using MIS.Controller;

namespace MIS
{
    public partial class frmQRDeliveryPrototype : Form
    {
        private readonly clsFunction dbFunction;
        private readonly ServicingDetailController servicingController;
        private readonly QRDeliveryBackendService qrBackend;
        private readonly IQRDeliveryLookupStore qrLookup;
        private readonly PrintDocument printDocument;
        private readonly QRDeliveryValidator qrValidator;
        private ServicingDetailController selectedService;
        private string validatedQRContent;
        private bool validationInProgress;

        public frmQRDeliveryPrototype()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            servicingController = new ServicingDetailController();
            IQRDeliveryHistoryStore historyStore;
            bool isLocalDatabase = QRDeliveryStoreFactory.TryCreateLocal(out historyStore);
            if (!isLocalDatabase)
                historyStore = new InMemoryQRDeliveryHistoryStore();
            qrBackend = new QRDeliveryBackendService(historyStore);
            qrLookup = isLocalDatabase
                ? (IQRDeliveryLookupStore)historyStore
                : new QRDeliveryController();
            printDocument = new PrintDocument();
            qrValidator = new QRDeliveryValidator();

            WireEvents();
            ResetForm();
        }

        private void WireEvents()
        {
            // The form layout and colors are controlled by the WinForms Designer.
            grpService.Paint -= sectionGroup_Paint;
            grpScan.Paint -= sectionGroup_Paint;
            grpResult.Paint -= sectionGroup_Paint;

            btnMinimize.Click += btnMinimize_Click;
            btnExit.Click += btnClose_Click;
            button4.Click += btnClose_Click;
            button1.Click += btnHistory_Click;
            btnSave.Visible = false;
            button2.Click += btnClear_Click;
            button3.Click += btnPrint_Click;
            btnSearchMerchant.Click += btnSearchMerchant_Click;
            printDocument.PrintPage += printDocument_PrintPage;
            rtbQRContent.KeyDown += rtbQRContent_KeyDown;
            txtServiceNo.ReadOnly = true;
            btnSearchMerchant.Enabled = false;
            btnSearchMerchant.Visible = false;

            KeyPreview = true;
            KeyDown += frmQRDeliveryPrototype_KeyDown;
            Shown += frmQRDeliveryPrototype_Shown;
        }

        private void frmQRDeliveryPrototype_Shown(object sender, EventArgs e)
        {
            FocusQRInput();
        }

        private void btnSearchMerchant_Click(object sender, EventArgs e)
        {
            btnHistory_Click(sender, e);
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            if (validationInProgress)
                return;

            validationInProgress = true;
            ClearValidation();
            selectedService = null;

            if (string.IsNullOrWhiteSpace(rtbQRContent.Text))
            {
                MessageBox.Show("Scan or paste the terminal QR content first.", "QR Delivery",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rtbQRContent.Focus();
                validationInProgress = false;
                return;
            }

            try
            {
                QRDeliveryData scanned = qrValidator.Parse(rtbQRContent.Text);
                if (string.IsNullOrWhiteSpace(scanned.TID) || string.IsNullOrWhiteSpace(scanned.MID))
                    throw new QRDeliveryValidationException(
                        "The QR code must contain both TID and MID before MIS lookup can run.", null);

                QRDeliveryLookupResult lookup = qrLookup.FindJobOrder(scanned.TID, scanned.MID);
                if (!lookup.Found || lookup.Expected == null)
                {
                    selectedService = null;
                    QRDeliveryValidationResult missingJo = qrValidator.Validate(
                        rtbQRContent.Text, new QRDeliveryData());
                    foreach (QRDeliveryFieldResult field in missingJo.Fields) AddResult(field);
                    AddStatusRows("MISMATCH");
                    validatedQRContent = rtbQRContent.Text.Trim();
                    lblQRStatus.Text = "NO JO";
                    lblQRStatus.ForeColor = Color.Red;
                    button3.Enabled = false;
                    lblServiceDetails.Text = string.Format(
                        "NO INSTALLATION OR REPLACEMENT J.O. FOR TID: {0} / MID: {1}       PROCESSED BY: {2}",
                        scanned.TID, scanned.MID,
                        string.IsNullOrWhiteSpace(clsUser.ClassUserName) ? "CURRENT USER" : clsUser.ClassUserName);
                    AutoSaveValidationAttempt();
                    MessageBox.Show("No Installation or Replacement J.O. was found for the scanned TID and MID. " +
                        "This failed validation was saved automatically for audit/history.", "QR Delivery",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                selectedService = new ServicingDetailController
                {
                    ServiceNo = lookup.ServiceNo,
                    IRIDNo = lookup.IRIDNo,
                    MerchantID = lookup.MerchantID,
                    JobType = lookup.JobType,
                    TID = lookup.Expected.TID,
                    MID = lookup.Expected.MID,
                    MerchantName = lookup.Expected.MerchantName,
                    Address = lookup.Expected.MerchantAddress,
                    TerminalSN = lookup.Expected.TerminalSerialNo,
                    SIMSN = lookup.Expected.SimSerialNo
                };
                txtServiceNo.Text = selectedService.ServiceNo.ToString();
                lblServiceDetails.Text = string.Format(
                    "IR ID NO.: {0}       MERCHANT ID: {1}       JOB TYPE: {2}       PROCESSED BY: {3}",
                    selectedService.IRIDNo, selectedService.MerchantID, lookup.JobTypeDescription,
                    string.IsNullOrWhiteSpace(clsUser.ClassUserName) ? "CURRENT USER" : clsUser.ClassUserName);

                QRDeliveryValidationResult result = qrValidator.Validate(
                    rtbQRContent.Text,
                    lookup.Expected);

                foreach (QRDeliveryFieldResult field in result.Fields)
                    AddResult(field);
                AddStatusRows(result.IsMatch ? "MATCH" : "MISMATCH");

                validatedQRContent = rtbQRContent.Text.Trim();
                lblQRStatus.Text = result.IsMatch ? "MATCH" : "MISMATCH";
                lblQRStatus.ForeColor = result.IsMatch ? Color.Green : Color.Red;
                button3.Enabled = result.IsMatch;
                AutoSaveValidationAttempt();

                if (result.MissingFields.Count > 0)
                    MessageBox.Show("The QR code is missing required information:\n\n- " +
                        string.Join("\n- ", new List<string>(result.MissingFields).ToArray()) +
                        "\n\nPlease scan the terminal QR code again.", "QR Delivery",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (QRDeliveryValidationException ex)
            {
                validatedQRContent = rtbQRContent.Text.Trim();
                lblQRStatus.Text = "INVALID QR";
                lblQRStatus.ForeColor = Color.Red;
                button3.Enabled = false;
                AddStatusRows("MISMATCH");
                AutoSaveValidationAttempt();
                MessageBox.Show(ex.Message,
                    "QR Delivery", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                button3.Enabled = false;
                MessageBox.Show("The validation attempt could not be completed or saved.\n\n" + ex.Message,
                    "QR Delivery", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                validationInProgress = false;
                FocusQRInput();
            }
        }

        private void AddResult(QRDeliveryFieldResult result)
        {
            int row = dgvValidation.Rows.Add(result.Field, result.ScannedValue,
                result.ExpectedValue, result.IsMatch ? "MATCH" : "MISMATCH");
            dgvValidation.Rows[row].Cells[3].Style.ForeColor = result.IsMatch ? Color.Green : Color.Red;
        }

        private void AddStatusRows(string inventoryStatus)
        {
            int row = dgvValidation.Rows.Add("Inventory Status", string.Empty,
                string.Empty, inventoryStatus);
            dgvValidation.Rows[row].Cells[3].Style.ForeColor =
                inventoryStatus == "MATCH" ? Color.Green : Color.Red;
            dgvValidation.Rows.Add("Terminal Prep Status", string.Empty, string.Empty, string.Empty);
            dgvValidation.Rows.Add("Dispatcher Status", string.Empty, string.Empty, string.Empty);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Validation attempts are saved automatically after validation.",
                "QR Delivery", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AutoSaveValidationAttempt()
        {
            qrBackend.SaveValidation(new QRDeliverySaveRequest
            {
                ServiceNo = selectedService == null ? 0 : selectedService.ServiceNo,
                IRIDNo = selectedService == null ? 0 : selectedService.IRIDNo,
                MerchantID = selectedService == null ? 0 : selectedService.MerchantID,
                QRContent = validatedQRContent,
                InventoryStatus = lblQRStatus.Text == "MATCH" ? "MATCH" : "MISMATCH",
                TerminalPrepStatus = string.Empty,
                DispatcherStatus = string.Empty,
                ProcessedBy = string.IsNullOrWhiteSpace(clsUser.ClassUserName) ? "CURRENT USER" : clsUser.ClassUserName,
                CreatedDate = DateTime.Now
            });
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            int serviceNo = selectedService == null ? 0 : selectedService.ServiceNo;
            IList<QRDeliveryHistoryItem> items = qrBackend.GetRecentHistory(serviceNo, 50);
            List<string> lines = new List<string>();
            foreach (QRDeliveryHistoryItem item in items)
                lines.Add(string.Format("{0:yyyy-MM-dd HH:mm:ss} | Service {1} | {2} | {3}",
                    item.DateTimeStamp, item.ServiceNo, item.InventoryStatus, item.ProcessedBy));
            string history = lines.Count == 0
                ? "No QR delivery validations have been saved."
                : string.Join(Environment.NewLine, lines.ToArray());
            MessageBox.Show(history, "QR Delivery History", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (selectedService == null || string.IsNullOrWhiteSpace(validatedQRContent))
            {
                MessageBox.Show("Only a successfully validated QR code can be printed.", "QR Delivery",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                QRDeliveryWaybillReport.ShowPreview(this, selectedService);
            }
            catch (Exception ex)
            {
                Exception detail = ex;
                while (detail.InnerException != null)
                    detail = detail.InnerException;

                MessageBox.Show("The QR Delivery waybill could not be created.\n\n" +
                    ex.Message + "\n\nRoot cause:\n" + detail.Message,
                    "QR Delivery", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            StringBuilder content = new StringBuilder();
            content.AppendLine("QR DELIVERY VALIDATION");
            content.AppendLine();
            content.AppendLine("Service No.: " + selectedService.ServiceNo);
            content.AppendLine("IR ID No.: " + selectedService.IRIDNo);
            content.AppendLine("Merchant: " + selectedService.MerchantName);
            content.AppendLine("TID: " + selectedService.TID);
            content.AppendLine("MID: " + selectedService.MID);
            content.AppendLine("Terminal SN: " + selectedService.TerminalSN);
            content.AppendLine("SIM SN: " + selectedService.SIMSN);
            content.AppendLine("Status: " + lblQRStatus.Text);
            content.AppendLine();
            content.AppendLine("QR CONTENT:");
            content.AppendLine(validatedQRContent);

            using (Font font = new Font("Courier New", 10F))
                e.Graphics.DrawString(content.ToString(), font, Brushes.Black, e.MarginBounds);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            selectedService = null;
            validatedQRContent = string.Empty;
            txtServiceNo.Clear();
            rtbQRContent.Clear();
            lblServiceDetails.Text = string.Format(
                "IR ID NO.: -       MERCHANT ID: -       PROCESSED BY: {0}",
                string.IsNullOrWhiteSpace(clsUser.ClassUserName) ? "CURRENT USER" : clsUser.ClassUserName);
            ClearValidation();
            FocusQRInput();
        }

        private void FocusQRInput()
        {
            if (!IsDisposed && rtbQRContent.CanFocus)
            {
                rtbQRContent.Focus();
                rtbQRContent.SelectionStart = rtbQRContent.TextLength;
            }
        }

        private void ClearValidation()
        {
            dgvValidation.Rows.Clear();
            validatedQRContent = string.Empty;
            lblQRStatus.Text = "NOT VALIDATED";
            lblQRStatus.ForeColor = Color.Silver;
            btnSave.Enabled = false;
            button3.Enabled = false;
        }

        private void rtbQRContent_TextChanged(object sender, EventArgs e)
        {
            if (dgvValidation.Rows.Count > 0 || !string.IsNullOrWhiteSpace(validatedQRContent))
                ClearValidation();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void frmQRDeliveryPrototype_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
            else if (e.KeyCode == Keys.F2)
                btnSearchMerchant_Click(sender, e);
        }

        private void rtbQRContent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter || e.Modifiers != Keys.None)
                return;

            e.SuppressKeyPress = true;
            e.Handled = true;
            btnValidate_Click(btnValidate, EventArgs.Empty);
        }

        private void sectionGroup_Paint(object sender, PaintEventArgs e)
        {
            GroupBox section = sender as GroupBox;
            if (section == null) return;

            Color headerColor = Color.FromArgb(205, 153, 255);
            Color borderColor = Color.Gray;
            e.Graphics.Clear(Color.FromArgb(247, 247, 247));
            using (SolidBrush headerBrush = new SolidBrush(headerColor))
                e.Graphics.FillRectangle(headerBrush, 0, 0, section.Width - 1, 24);
            using (Pen borderPen = new Pen(borderColor))
                e.Graphics.DrawRectangle(borderPen, 0, 0, section.Width - 1, section.Height - 1);
            using (SolidBrush textBrush = new SolidBrush(Color.Navy))
            using (Font font = new Font("Courier New", 8.25F, FontStyle.Bold))
                e.Graphics.DrawString(Convert.ToString(section.Tag), font, textBrush, 7, 5);
        }

        private void lblHeader_Click(object sender, EventArgs e) { }
        private void pnlHeader_Paint(object sender, PaintEventArgs e) { }
        private void dgvValidation_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void txtServiceNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void W(object sender, EventArgs e)
        {

        }
    }
}
