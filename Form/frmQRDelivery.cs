using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using MIS.Controller;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QRCoder;

namespace MIS
{
    public partial class frmQRDelivery : Form
    {
        private readonly clsFunction dbFunction;
        private readonly ServicingDetailController servicingController;
        private readonly QRDeliveryBackendService qrBackend;
        private readonly IQRDeliveryLookupStore qrLookup;
        private readonly PrintDocument printDocument;
        private readonly QRDeliveryValidator qrValidator;
        private ServicingDetailController selectedService;
        private string validatedQRContent;
        private string internalQRContent;
        private string inventoryStatus;
        private string terminalPrepStatus;
        private string dispatcherStatus;
        private DateTime? validatedQRDate;
        private Bitmap generatedQrImage;
        private readonly List<QRDeliveryHistoryItem> sessionHistory =
            new List<QRDeliveryHistoryItem>();
        private bool validationInProgress;

        public frmQRDelivery()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            servicingController = new ServicingDetailController();
            // Lookup and save use the same authoritative MIS API.
            QRDeliveryController apiStore = new QRDeliveryController();
            qrBackend = new QRDeliveryBackendService(apiStore);
            qrLookup = apiStore;
            printDocument = new PrintDocument();
            qrValidator = new QRDeliveryValidator();
            sessionHistory.AddRange(QRDeliveryHistoryCache.Load());
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
            btnSave.Visible = false;
            btnClear.Click += btnClear_Click;
            btnPrintQR.Click += btnPrint_Click;            
            printDocument.PrintPage += printDocument_PrintPage;
            rtbQRContent.KeyDown += rtbQRContent_KeyDown;
            txtServiceNo.ReadOnly = true;
            btnSearchService.Enabled = true;
            btnSearchService.Click += btnSearchService_Click;

            KeyPreview = true;
            KeyDown += frmQRDeliveryPrototype_KeyDown;
            Shown += frmQRDeliveryPrototype_Shown;
        }

        private void frmQRDeliveryPrototype_Shown(object sender, EventArgs e)
        {
            FocusQRInput();
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            if (validationInProgress)
                return;

            validationInProgress = true;
            ClearValidation();
            selectedService = null;

            lblAction.Text = $"PROCESSING";

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
                    txtServiceNo.Clear();
                    validatedQRContent = rtbQRContent.Text.Trim();
                    inventoryStatus = "INVALID";
                    terminalPrepStatus = "INVALID";
                    dispatcherStatus = "INVALID";
                    lblQRStatus.Text = "NO JO";
                    lblQRStatus.ForeColor = Color.Red;
                    btnPrintQR.Enabled = false;
                    lblServiceDetails.Text = string.Format(
                        "NO INSTALLATION OR REPLACEMENT J.O. FOR TID: {0} / MID: {1}       PROCESSED BY: {2}",
                        scanned.TID, scanned.MID,
                        string.IsNullOrWhiteSpace(clsUser.ClassUserName) ? "CURRENT USER" : clsUser.ClassUserName);
                    MessageBox.Show("No Installation or Replacement J.O. was found for the scanned TID and MID. " +
                        "Printing remains disabled until a valid service record is selected.", "QR Delivery",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    TrySaveValidationAttempt();
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
                    TerminalID = lookup.Expected.TerminalID,
                    TerminalSN = lookup.Expected.TerminalSerialNo,
                    SIMID = lookup.Expected.SimID,
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
                bool allStatusesValid = AddStatusRows(lookup, result);

                validatedQRContent = rtbQRContent.Text.Trim();
                lblQRStatus.Text = result.IsMatch ? "MATCH" : "MISMATCH";
                lblQRStatus.ForeColor = result.IsMatch ? Color.Green : Color.Red;
                btnPrintQR.Enabled = result.IsMatch && allStatusesValid;
                if (result.IsMatch && allStatusesValid)
                    internalQRContent = qrValidator.CreateInternalContent(lookup);

                TrySaveValidationAttempt();

                if (result.MissingFields.Count > 0)
                    MessageBox.Show("The QR code is missing required information:\n\n- " +
                        string.Join("\n- ", new List<string>(result.MissingFields).ToArray()) +
                        "\n\nPlease scan the terminal QR code again.", "QR Delivery",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (QRDeliveryValidationException ex)
            {
                validatedQRContent = rtbQRContent.Text.Trim();
                inventoryStatus = "INVALID";
                terminalPrepStatus = "INVALID";
                dispatcherStatus = "INVALID";
                lblQRStatus.Text = "INVALID QR";
                lblQRStatus.ForeColor = Color.Red;
                btnPrintQR.Enabled = false;
                MessageBox.Show(ex.Message,
                    "QR Delivery", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TrySaveValidationAttempt();
            }
            catch (Exception ex)
            {
                btnPrintQR.Enabled = false;
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
            ApplyResultCellStyle(dgvValidation.Rows[row].Cells[3], result.IsMatch);
        }

        private bool AddStatusRows(QRDeliveryLookupResult lookup,
            QRDeliveryValidationResult validation)
        {
            bool terminalInventoryValid = lookup.Expected.TerminalID > 0 &&
                IsFieldMatch(validation, "Terminal Serial No.");
            bool hasSim = lookup.Expected.SimID > 0;
            bool simInventoryValid = !hasSim ||
                IsFieldMatch(validation, "SIM Serial No.");
            inventoryStatus = terminalInventoryValid && simInventoryValid && validation.IsMatch
                ? "VALID" : "INVALID";
            terminalPrepStatus = terminalInventoryValid &&
                QRDeliveryStatusRules.TerminalPrepStatus(lookup.Expected) == "VALID"
                ? "VALID" : "INVALID";
            dispatcherStatus = QRDeliveryStatusRules.DispatcherStatus(
                lookup.JobTypeStatusDescription);

            AddStatusRow("Inventory Terminal Status",
                lookup.Expected.TerminalID.ToString(),
                terminalInventoryValid ? "VALID" : "INVALID");
            if (hasSim)
                AddStatusRow("Inventory SIM Status",
                    lookup.Expected.SimID.ToString(),
                    simInventoryValid ? "VALID" : "INVALID");
            AddStatusRow("Terminal Prep Status", lookup.Expected.TerminalID.ToString(),
                terminalPrepStatus);
            AddStatusRow("Dispatcher Status", lookup.JobTypeStatusDescription,
                dispatcherStatus);
            return inventoryStatus == "VALID" && terminalPrepStatus == "VALID" &&
                   dispatcherStatus == "VALID";
        }

        private static bool IsFieldMatch(QRDeliveryValidationResult validation, string fieldName)
        {
            foreach (QRDeliveryFieldResult field in validation.Fields)
                if (string.Equals(field.Field, fieldName, StringComparison.OrdinalIgnoreCase))
                    return field.IsMatch;
            return false;
        }

        private void AddStatusRow(string name, string sourceValue, string result)
        {
            int row = dgvValidation.Rows.Add(name, sourceValue, string.Empty, result);
            bool valid = result == "VALID";
            ApplyResultCellStyle(dgvValidation.Rows[row].Cells[3], valid);
        }

        private static void ApplyResultCellStyle(DataGridViewCell cell, bool valid)
        {
            Color background = valid ? Color.Green : Color.Red;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cell.Style.BackColor = background;
            cell.Style.ForeColor = Color.White;
            cell.Style.SelectionBackColor = background;
            cell.Style.SelectionForeColor = Color.White;
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

        private void SaveValidationAttempt()
        {
            DateTime savedAt = DateTime.Now;
            string processedBy = string.IsNullOrWhiteSpace(clsUser.ClassUserName)
                ? "CURRENT USER" : clsUser.ClassUserName;
            QRDeliveryHistoryItem sessionItem = new QRDeliveryHistoryItem
            {
                ServiceNo = selectedService == null ? 0 : selectedService.ServiceNo,
                IRIDNo = selectedService == null ? 0 : selectedService.IRIDNo,
                MerchantID = selectedService == null ? 0 : selectedService.MerchantID,
                MerchantName = selectedService == null ? string.Empty : selectedService.MerchantName,
                TID = selectedService == null ? string.Empty : selectedService.TID,
                MID = selectedService == null ? string.Empty : selectedService.MID,
                TerminalSN = selectedService == null ? string.Empty : selectedService.TerminalSN,
                SIMSN = selectedService == null ? string.Empty : selectedService.SIMSN,
                QRContent = validatedQRContent,
                InventoryStatus = inventoryStatus,
                TerminalPrepStatus = terminalPrepStatus,
                DispatcherStatus = dispatcherStatus,
                QRResult = lblQRStatus.Text,
                ProcessedBy = processedBy,
                QRDate = savedAt.Date,
                DateTimeStamp = savedAt
            };

            sessionHistory.Insert(0, sessionItem);
            if (string.Equals(sessionItem.QRResult, "READY TO DISPATCH",
                StringComparison.OrdinalIgnoreCase))
                validatedQRDate = sessionItem.QRDate;
            try
            {
                QRDeliveryHistoryCache.Append(sessionItem);
            }
            catch
            {
                // Continue with the authoritative API save when the local cache
                // is temporarily unavailable.
            }

            qrBackend.SaveValidation(new QRDeliverySaveRequest
            {
                ServiceNo = sessionItem.ServiceNo,
                IRIDNo = sessionItem.IRIDNo,
                MerchantID = sessionItem.MerchantID,
                QRContent = validatedQRContent,
                InternalQRContent = internalQRContent,
                InventoryStatus = inventoryStatus,
                TerminalPrepStatus = terminalPrepStatus,
                DispatcherStatus = dispatcherStatus,
                QRResult = sessionItem.QRResult,
                ProcessedBy = processedBy,
                CreatedDate = savedAt
            });
        }

        private void TrySaveValidationAttempt()
        {
            // Calculate the final QR result before creating the backoffice payload.
            if (dgvValidation.Rows.Count > 0)
                setOverallQRStatus();
            else
            {
                lblQRStatus.Text = "NOT READY TO DISPATCH";
                lblQRStatus.ForeColor = Color.Red;
            }

            try
            {
                SaveValidationAttempt();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("The scan result was displayed, but its audit record could not be saved.\n\n" +
                    ex.Message, "QR Delivery Audit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            try
            {
                IList<QRDeliveryHistoryItem> items = LoadSearchHistory();
                using (frmQRDeliveryHistory history = new frmQRDeliveryHistory(items, false))
                    history.ShowDialog(this);
            }
            catch (Exception)
            {
                if (sessionHistory.Count > 0)
                {
                    using (frmQRDeliveryHistory history =
                        new frmQRDeliveryHistory(sessionHistory, true))
                        history.ShowDialog(this);
                    return;
                }

                MessageBox.Show("No QR delivery validations have been saved in this session.",
                    "QR Delivery History", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private static IList<QRDeliveryHistoryItem> MergeHistory(
            IList<QRDeliveryHistoryItem> persisted,
            IList<QRDeliveryHistoryItem> currentSession,
            int limit)
        {
            List<QRDeliveryHistoryItem> merged = persisted == null
                ? new List<QRDeliveryHistoryItem>()
                : new List<QRDeliveryHistoryItem>(persisted);

            if (currentSession != null)
                foreach (QRDeliveryHistoryItem sessionItem in currentSession)
                {
                    bool duplicate = false;
                    foreach (QRDeliveryHistoryItem item in merged)
                        if (item.ServiceNo == sessionItem.ServiceNo &&
                            item.IRIDNo == sessionItem.IRIDNo &&
                            item.MerchantID == sessionItem.MerchantID &&
                            string.Equals(item.QRResult, sessionItem.QRResult,
                                StringComparison.OrdinalIgnoreCase) &&
                            Math.Abs((item.DateTimeStamp - sessionItem.DateTimeStamp).TotalSeconds) < 2)
                        {
                            duplicate = true;
                            break;
                        }

                    if (!duplicate)
                        merged.Add(sessionItem);
                }

            merged.Sort(delegate(QRDeliveryHistoryItem left, QRDeliveryHistoryItem right)
            {
                return right.DateTimeStamp.CompareTo(left.DateTimeStamp);
            });
            if (merged.Count > limit)
                merged.RemoveRange(limit, merged.Count - limit);
            return merged;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (selectedService == null || string.IsNullOrWhiteSpace(internalQRContent) ||
                !btnPrintQR.Enabled)
            {
                MessageBox.Show("Only a successfully validated QR code can be printed.", "QR Delivery",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (!validatedQRDate.HasValue)
                    throw new InvalidOperationException("The validated QR date is unavailable.");

                QRDeliveryWaybillReport.ShowPreview(this, selectedService, internalQRContent,
                    validatedQRDate.Value);
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
            const int qrSize = 320;
            int qrX = e.MarginBounds.Left + (e.MarginBounds.Width - qrSize) / 2;
            int qrY = e.MarginBounds.Top + 35;

            using (Font titleFont = new Font("Arial", 14F, FontStyle.Bold))
            using (Font contentFont = new Font("Courier New", 9F))
            {
                e.Graphics.DrawString("QR DELIVERY", titleFont, Brushes.Black,
                    e.MarginBounds.Left, e.MarginBounds.Top);
                if (generatedQrImage != null)
                    e.Graphics.DrawImage(generatedQrImage,
                        new Rectangle(qrX, qrY, qrSize, qrSize));

                RectangleF contentBounds = new RectangleF(
                    e.MarginBounds.Left,
                    qrY + qrSize + 20,
                    e.MarginBounds.Width,
                    e.MarginBounds.Bottom - (qrY + qrSize + 20));
                e.Graphics.DrawString(internalQRContent, contentFont, Brushes.Black,
                    contentBounds);
            }

            e.HasMorePages = false;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            selectedService = null;
            if (generatedQrImage != null)
            {
                generatedQrImage.Dispose();
                generatedQrImage = null;
            }
            validatedQRContent = string.Empty;
            internalQRContent = string.Empty;
            inventoryStatus = string.Empty;
            terminalPrepStatus = string.Empty;
            dispatcherStatus = string.Empty;
            txtServiceNo.Clear();
            rtbQRContent.Clear();
            lblServiceDetails.Text = string.Format(
                "IR ID NO.: -       MERCHANT ID: -       PROCESSED BY: {0}",
                string.IsNullOrWhiteSpace(clsUser.ClassUserName) ? "CURRENT USER" : clsUser.ClassUserName);
            ClearValidation();
            FocusQRInput();

            lblAction.Text = $"NEXT SCAN";
            rtbQRContent.Focus();
        }

        private void FocusQRInput()
        {
            if (!IsDisposed && rtbQRContent.CanFocus)
            {
                lblAction.Text = $"NEXT SCAN";

                rtbQRContent.Focus();
                rtbQRContent.SelectionStart = rtbQRContent.TextLength;
            }
        }

        private void ClearValidation()
        {
            dgvValidation.Rows.Clear();
            validatedQRContent = string.Empty;
            internalQRContent = string.Empty;
            inventoryStatus = string.Empty;
            terminalPrepStatus = string.Empty;
            dispatcherStatus = string.Empty;
            validatedQRDate = null;
            lblQRStatus.Text = "NOT VALIDATED";
            lblQRStatus.ForeColor = Color.Silver;
            btnSave.Enabled = false;
            btnPrintQR.Enabled = false;
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
            using (Font font = new Font("Courier New", 9.25F, FontStyle.Bold))
                e.Graphics.DrawString(Convert.ToString(section.Tag), font, textBrush, 7, 5);
        }

        private void lblHeader_Click(object sender, EventArgs e) { }
        private void pnlHeader_Paint(object sender, PaintEventArgs e) { }
        private void dgvValidation_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void btnSearchService_Click(object sender, EventArgs e)
        {
            try
            {
                IList<QRDeliveryHistoryItem> items = MergeHistory(
                    qrBackend.GetRecentHistory(0, 50), sessionHistory, 50);
                using (frmSearchField search = new frmSearchField(items))
                    if (search.ShowDialog(this) == DialogResult.OK &&
                        search.SelectedQRDeliveryRecord != null)
                    {
                        DisplayHistoryRecord(search.SelectedQRDeliveryRecord);
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Saved QR scans could not be loaded.\n\n" + ex.Message,
                    "QR Delivery Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private IList<QRDeliveryHistoryItem> LoadSearchHistory()
        {
            IList<QRDeliveryHistoryItem> persisted;
            try
            {
                persisted = qrBackend.GetRecentHistory(0, 50);
            }
            catch
            {
                // A malformed value in an older API row must not block search.
                persisted = new List<QRDeliveryHistoryItem>();
            }

            IList<QRDeliveryHistoryItem> combined = MergeHistory(
                persisted, QRDeliveryHistoryCache.Load(), 200);
            return MergeHistory(combined, sessionHistory, 200);
        }

        private void DisplayHistoryRecord(QRDeliveryHistoryItem history)
        {
            ClearValidation();
            selectedService = null;
            txtServiceNo.Text = history.ServiceNo > 0
                ? history.ServiceNo.ToString() : string.Empty;
            lblServiceDetails.Text = string.Format(
                "IR ID NO.: {0}       MERCHANT ID: {1}       MERCHANT: {2}       PROCESSED BY: {3}",
                history.IRIDNo, history.MerchantID, history.MerchantName,
                history.ProcessedBy);

            string historyQRContent = history.QRContent;
            if (string.IsNullOrWhiteSpace(historyQRContent) &&
                string.Equals(history.QRResult, "READY TO DISPATCH",
                    StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(history.TID) &&
                !string.IsNullOrWhiteSpace(history.MID))
            {
                JObject content = new JObject
                {
                    ["tid"] = history.TID,
                    ["mid"] = history.MID,
                    ["merchantName"] = history.MerchantName,
                    ["terminalSerialNo"] = history.TerminalSN
                };
                if (!string.IsNullOrWhiteSpace(history.SIMSN))
                    content["simSerialNo"] = history.SIMSN;
                historyQRContent = content.ToString(Formatting.None);
            }

            if (!string.IsNullOrWhiteSpace(historyQRContent))
            {
                rtbQRContent.Text = historyQRContent;
                QRDeliveryData scanned = qrValidator.Parse(historyQRContent);
                QRDeliveryLookupResult lookup = qrLookup.FindJobOrder(scanned.TID, scanned.MID);
                if (lookup.Found && lookup.Expected != null)
                {
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
                        TerminalID = lookup.Expected.TerminalID,
                        TerminalSN = lookup.Expected.TerminalSerialNo,
                        SIMID = lookup.Expected.SimID,
                        SIMSN = lookup.Expected.SimSerialNo
                    };
                    lblServiceDetails.Text = string.Format(
                        "IR ID NO.: {0}       MERCHANT ID: {1}       JOB TYPE: {2}       PROCESSED BY: {3}",
                        lookup.IRIDNo, lookup.MerchantID, lookup.JobTypeDescription,
                        history.ProcessedBy);

                    QRDeliveryValidationResult comparison =
                        qrValidator.Validate(historyQRContent, lookup.Expected);
                    foreach (QRDeliveryFieldResult field in comparison.Fields)
                        AddResult(field);
                    bool statusesValid = AddStatusRows(lookup, comparison);
                    bool ready = comparison.IsMatch && statusesValid;
                    lblQRStatus.Text = ready ? "READY TO DISPATCH" : "NOT READY TO DISPATCH";
                    lblQRStatus.ForeColor = ready ? Color.Green : Color.Red;
                    validatedQRContent = historyQRContent;
                    validatedQRDate = history.QRDate;
                    btnPrintQR.Enabled = ready;
                    if (ready)
                        internalQRContent = qrValidator.CreateInternalContent(lookup);
                    return;
                }
            }

            AddStatusRow("Inventory Terminal Status", string.Empty,
                NormalizeStoredStatus(history.InventoryStatus));
            AddStatusRow("Inventory SIM Status", string.Empty,
                NormalizeStoredStatus(history.InventoryStatus));
            AddStatusRow("Terminal Prep Status", string.Empty,
                NormalizeStoredStatus(history.TerminalPrepStatus));
            AddStatusRow("Dispatcher Status", string.Empty,
                NormalizeStoredStatus(history.DispatcherStatus));
            bool storedReady = string.Equals(history.QRResult, "READY TO DISPATCH",
                StringComparison.OrdinalIgnoreCase);
            lblQRStatus.Text = storedReady ? "READY TO DISPATCH" : "NOT READY TO DISPATCH";
            lblQRStatus.ForeColor = storedReady ? Color.Green : Color.Red;
            validatedQRDate = history.QRDate;
            btnPrintQR.Enabled = false;
        }

        private static string NormalizeStoredStatus(string status)
        {
            return string.Equals(status, "VALID", StringComparison.OrdinalIgnoreCase)
                ? "VALID" : "INVALID";
        }

        private void setOverallQRStatus()
        {
            bool hasError = false;

            foreach (DataGridViewRow row in dgvValidation.Rows)
            {
                if (row.IsNewRow)
                    continue;

                DataGridViewCell resultCell = row.Cells[3];

                string result = resultCell.Value?.ToString().Trim();

                // Check result value
                bool invalidResult =
                    result == "NOT MATCH" ||
                    result == "NOT YET DISPATCH" ||
                    result == "INVALID";

                // Check result cell color
                bool redResult =
                    resultCell.InheritedStyle.BackColor == Color.Red ||
                    resultCell.Style.BackColor == Color.Red;

                if (invalidResult || redResult)
                {
                    hasError = true;
                    break;
                }
            }

            if (hasError)
            {
                lblQRStatus.Text = "NOT READY TO DISPATCH";
                lblQRStatus.ForeColor = Color.Red;
            }
            else
            {
                lblQRStatus.Text = "READY TO DISPATCH";
                lblQRStatus.ForeColor = Color.Green;
            }
        }

        private void btnPrintQR_Click(object sender, EventArgs e)
        {

        }
    }
}
