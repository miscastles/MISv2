using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using MIS.Model;
using MIS.AppMainActivity;

namespace MIS
{
    public partial class frmPrintOption : Form
    {
        private clsAPI dbAPI;
        private clsReportFunc dbReportFunc;
        private clsFunction dbFunction;

        public static int iParticularType;
        public static DateTime stDateFrom;
        public static string sDateFrom = "";
        public static DateTime stDateTo;
        public static string sDateTo = "";

        public static int iReportType;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED
                //cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        public frmPrintOption()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (!isValidReport()) return;

            if (txtReportID.TextLength > 0)
            {
                // Check Date
                if (!CheckDateFromTo(dteDateFrom, dteDateTo)) return;

                // Waiting / Hour Glass
                Cursor.Current = Cursors.WaitCursor;

                try
                {
                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                    PreviewReport(clsReport.ClassReportID);

                    Cursor.Current = Cursors.Default; // Back to normal 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                // Back to normal 
                Cursor.Current = Cursors.Default;
            }

        }

        private void InitDate()
        {
            dteDateFrom.Value = DateTime.Now.Date;
            //dbFunction.SetDateFormatWithWeekDay(dteDateFrom);
            dbFunction.SetDateFormat(dteDateFrom, clsFunction.sDateDefaultFormat);

            dteDateTo.Value = DateTime.Now.Date;
            //dbFunction.SetDateFormatWithWeekDay(dteDateTo);
            dbFunction.SetDateFormat(dteDateTo, clsFunction.sDateDefaultFormat);

        }

        private void frmPrintOption_Load(object sender, EventArgs e)
        {

            dbAPI = new clsAPI();
            dbReportFunc = new clsReportFunc();
            dbFunction = new clsFunction();

            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);
            dbFunction.ClearListViewItems(lvwList);

            // ROCKY - PRINT OPTION ISSUE: DISPLAYING CLIENT IF CLIENT SEARCH.
            if (iParticularType.Equals(clsGlobalVariables.iClient_Type))
            {
                lblParticularHeader.Text = "CLIENT NAME *";
            }

            InitDate();

            LoadReport("View", "Report Type", iReportType + clsFunction.sPipe);

            dbAPI.FillComboBoxDepartment(cboDepartment);

            ResetSearchValue();

            SetOption();
        }

        private void LoadReport(string StatementType, string SearchBy, string SearchValue)
        {
            int inLineNo = 0;
            int i = 0;

            lvwList.Items.Clear();
            dbAPI.ExecuteAPI("GET", StatementType, SearchBy, SearchValue, "Report", "", "ViewReport");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (clsReport.ClassRecordFound)
            {
                while (clsArray.ReportID.Length > i)
                {
                    inLineNo++;
                    ListViewItem item = new ListViewItem(inLineNo.ToString());
                    item.SubItems.Add(clsArray.ReportID[i]);
                    item.SubItems.Add(clsArray.ReportDesc[i]);
                    lvwList.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(lvwList);
            }
        }

        private void btnSearchParticular_Click(object sender, EventArgs e)
        {
            // ROCKY - PRINT OPTION ISSUE: DISPLAYING CLIENT IF CLIENT SEARCH.
            if (iParticularType == clsGlobalVariables.iClient_Type)
            {
                frmSearchField.iSearchType = frmSearchField.SearchType.iClient;
                frmSearchField.sHeader = "CLIENT";
                frmSearchField.isCheckBoxes = false;
                frmSearchField frm = new frmSearchField();
                frm.ShowDialog();

                if (frmSearchField.fSelected)
                {
                    txtParticularID.Text = clsSearch.ClassParticularID.ToString();
                    txtName.Text = clsSearch.ClassParticularName;
                }
            }
            else
            {
                frmSearchField.iSearchType = frmSearchField.SearchType.iFE;
                frmSearchField.sHeader = "EMPLOYEE";
                frmSearchField.isCheckBoxes = false;
                frmSearchField frm = new frmSearchField();
                frm.ShowDialog();

                if (frmSearchField.fSelected)
                {
                    txtParticularID.Text = clsSearch.ClassParticularID.ToString();
                    txtName.Text = clsSearch.ClassParticularName;
                    txtDepartment.Text = clsSearch.ClassDepartment;
                    txtPosition.Text = clsSearch.ClassPosition;

                    txtName.BackColor = clsFunction.MKBackColor;
                }
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            ResetSearchValue();
            InitDate();
            cboDepartment.Text = clsFunction.sDefaultSelect;
            chkMissing.Enabled = false;
            chkMissing.Checked = false;
            btnCreateInvoice.Enabled = false;
        }

        private void cboDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassDepartmentID = 0;
            clsSearch.ClassDepartment = clsFunction.sDash;

            if (!cboDepartment.Text.Equals(clsFunction.sDefaultSelect))
            {
                clsSearch.ClassSearchValue = cboDepartment.Text;
                dbAPI.ExecuteAPI("GET", "Search", "Department", clsSearch.ClassSearchValue, "Department", "", "ViewDepartment");
                clsSearch.ClassDepartmentID = clsDepartment.ClassDepartmentID;
                clsSearch.ClassDepartment = cboDepartment.Text;
            }

            txtDepartmentID.Text = clsSearch.ClassDepartmentID.ToString();

        }

        private void lvwList_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtReportID.Text = "";

            if (lvwList.SelectedItems.Count > 0)
            {
                string LineNo = lvwList.SelectedItems[0].Text;

                if (LineNo.Length > 0)
                {
                    // Clear
                    dbFunction.ClearTextBox(this);
                    clsSearch.ClassReportID = 0;
                    clsSearch.ClassDepartmentID = 0;
                    clsSearch.ClassParticularID = 0;
                    clsSearch.ClassDepartment = clsFunction.sDash;
                    clsSearch.ClassDateFrom = clsFunction.sDateDefault;
                    clsSearch.ClassDateTo = clsFunction.sDateDefault;

                    string ReportID = lvwList.SelectedItems[0].SubItems[1].Text;
                    string ReportDescription = lvwList.SelectedItems[0].SubItems[2].Text;

                    clsReport.ClassReportID = int.Parse(ReportID);
                    clsReport.ClassReportDesc = ReportDescription;
                    clsSearch.ClassReportID = int.Parse(ReportID);

                    txtReportID.Text = clsReport.ClassReportID.ToString();
                    lblReportDescription.Text = clsReport.ClassReportDesc;
                }
            }

            SetOption();
        }

        private bool isValidReport()
        {
            bool isValid = false;
            int ReportID = 0;

            ReportID = (txtReportID.Text.Length > 0 ? int.Parse(txtReportID.Text) : 0);

            if (ReportID > 0)
            {
                return true;
            }

            if (!isValid)
            {
                MessageBox.Show("Please select report to view first.", "Invalid Report", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool CheckDateFromTo(DateTimePicker objFrom, DateTimePicker objTo)
        {
            bool fValid = true;
            int iResult;

            Debug.WriteLine("--CheckDateFromTo--");
            Debug.WriteLine("objFrom=" + objFrom.ToString());
            Debug.WriteLine("objTo=" + objTo.ToString());

            if (!dteDateFrom.Enabled && !dteDateTo.Enabled)
                return true;

            iResult = DateTime.Compare(DateTime.Parse(dteDateFrom.Value.ToShortDateString()), DateTime.Parse(dteDateTo.Value.ToShortDateString()));

            Debug.WriteLine("iResult=" + iResult);

            if (iResult > 0)
                fValid = false;

            if (!fValid)
            {
                MessageBox.Show("[Date From] shouldn't be greater than [Date To]" +
                                         "\n\n" +
                                         "Date From: " + objFrom.Value.ToString("MM-dd-yyyy") +
                                         "\n" +
                                         "Date To:      " + objTo.Value.ToString("MM-dd-yyyy"),
                                         "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }

            return fValid;
        }

        private void PreviewReport(int ReportID)
        {
            Debug.WriteLine("--PreviewReport--");
            Debug.WriteLine("ReportID=" + ReportID);

            clsSearch.ClassDateFrom = clsSearch.ClassDateTo = clsFunction.sDateFormat;

            // Variable Date
            stDateFrom = dteDateFrom.Value;
            sDateFrom = stDateFrom.ToString("yyyy-MM-dd");

            stDateTo = dteDateTo.Value;
            sDateTo = stDateTo.ToString("yyyy-MM-dd");

            clsSearch.ClassParticularID = int.Parse(dbFunction.CheckAndSetNumericValue(txtParticularID.Text));
            clsSearch.ClassDepartmentID = int.Parse(dbFunction.CheckAndSetNumericValue(txtDepartmentID.Text));
            clsSearch.ClassDepartment = cboDepartment.Text;
            clsSearch.ClassMobileTerminalID = clsSearch.ClassTSTerminalID = dbFunction.CheckAndSetNumericValue(txtTerminalID.Text);
            clsSearch.ClassTSTerminalName = dbFunction.CheckAndSetNumericValue(txtTerminalName.Text);
            clsSearch.ClassDateFrom = sDateFrom;
            clsSearch.ClassDateTo = sDateTo;
            clsSearch.ClassReportID = ReportID;
            clsSearch.ClassMissingTimeSheet = (chkMissing.Checked ? 1 : 0);
            clsSearch.ClassReportType = (chkMissing.Checked ? "MISSING AND INCOMPLETE TIMESHEET" : "-");

            // ROCKY - BILLING: ADD SEARCH FUNTION FOR CLIENT - PREVIEW REPORT
            clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetStringValue(txtName.Text);
            
            Debug.WriteLine("clsSearch.ClassReportID=" + clsSearch.ClassReportID);
            Debug.WriteLine("clsSearch.ClassParticularID=" + clsSearch.ClassParticularID);
            Debug.WriteLine("clsSearch.ClassDepartmentID=" + clsSearch.ClassDepartmentID);
            Debug.WriteLine("clsSearch.ClassMobileTerminalID=" + clsSearch.ClassMobileTerminalID);
            Debug.WriteLine("clsSearch.ClassTSTerminalID=" + clsSearch.ClassTSTerminalID);
            Debug.WriteLine("clsSearch.ClassTSTerminalName=" + clsSearch.ClassTSTerminalName);
            Debug.WriteLine("clsSearch.ClassDateFrom=" + clsSearch.ClassDateFrom);
            Debug.WriteLine("clsSearch.ClassDateTo=" + clsSearch.ClassDateTo);
            Debug.WriteLine("clsSearch.ClassMissingTimeSheet=" + clsSearch.ClassMissingTimeSheet);
            Debug.WriteLine("clsSearch.ClassReportType=" + clsSearch.ClassReportType);
            
            switch (ReportID)
            {
                case 12: // QRCode Report                    
                    dbReportFunc.ViewQRCode();
                    break;
                case 13: // TimeSheet Report
                case 21:

                    dbAPI.ProcessTimeSheet();

                    dbReportFunc.ViewTimeSheet();
                    break;
                case 14: // Work Arrangement Report
                    dbReportFunc.ViewWorkArrangement();
                    break;
                case 15: // Holiday Report
                    dbReportFunc.ViewHoliday();
                    break;
                case 16: // Leave Application Report
                    dbReportFunc.ViewLeaveApplication();
                    break;
                case 17: // Leave Assignment Report
                    dbReportFunc.ViewLeaveAssignment();
                    break;

                case 34: // ROCKY - PARTICULAR: ADD EMPLOYEE DETAILS - PREVIEW REPORT
                    dbReportFunc.ViewParticularDetailReport();
                    break;
                case 35: // ROCKY - PARTICULAR: ADD EMPLOYEE REQUIREMENTS DETAILS - PREVIEW REPORT
                    dbReportFunc.ViewParticularRequirementsReport();
                    break;

                case 36: // ROCKY - BILLING: ADD SECURITY BANK BILLING DETAILS - PREVIEW REPORT
                    dbReportFunc.ViewBillingSecurityBankDetails();
                    break;

                case 37: // ROCKY - BILLING: ADD METROBANK SERVICING DETAILS - PREVIEW REPORT
                    dbReportFunc.ViewMetrobankServicingDetails();
                    break;

                case 38:// ROCKY - BILLING: ADD METROBANK NET MATRIX INVOICE - PREVIEW REPORT
                    dbReportFunc.ViewMetrobankNetMatrixDetails();
                    break;

                case 39:
                    dbReportFunc.WeePayInventoryReport();
                    break;

                case 40:
                    dbReportFunc.WeePaySimSummaryReport();
                    break;


                default:
                    dbFunction.SetMessageBox("No report to be generated.", "Report failed", clsFunction.IconType.iExclamation);
                    break;
            }
        }

        private void lvwList_Click(object sender, EventArgs e)
        {
            SetOption();
        }

        private void SetOption()
        {
            cboDepartment.Enabled = false;
            dteDateFrom.Enabled = false;
            dteDateTo.Enabled = false;
            btnSearchParticular.Enabled = false;
            btnFindTerminal.Enabled = false;
            chkMissing.Enabled = false;
            btnCreateInvoice.Enabled = false;
            btnGenExcel.Enabled = false;

            txtName.BackColor = txtTerminalID.BackColor = txtTerminalName.BackColor = clsFunction.DisableBackColor;

            switch (clsReport.ClassReportID)
            {
                case 12: // QRCode Rwport
                    btnSearchParticular.Enabled = true;
                    cboDepartment.Enabled = true;

                    txtName.BackColor = clsFunction.MKBackColor;
                    break;
                case 13: // TimeSheet Report
                    btnSearchParticular.Enabled = true;
                    btnFindTerminal.Enabled = true;
                    cboDepartment.Enabled = true;
                    dteDateFrom.Enabled = true;
                    dteDateTo.Enabled = true;
                    chkMissing.Enabled = true;
                    chkMissing.Checked = false;

                    txtName.BackColor = txtTerminalID.BackColor = txtTerminalName.BackColor = clsFunction.MKBackColor;
                    break;
                case 14: // Work Arrangement Report
                    btnSearchParticular.Enabled = true;
                    cboDepartment.Enabled = true;

                    txtName.BackColor = clsFunction.MKBackColor;
                    break;
                case 15: // Holiday Report
                    break;
                case 16: // Leave Application Report
                    btnSearchParticular.Enabled = true;
                    cboDepartment.Enabled = true;

                    txtName.BackColor = clsFunction.MKBackColor;
                    break;
                case 17: // Leave Assignment Report
                    btnSearchParticular.Enabled = true;
                    cboDepartment.Enabled = true;

                    txtName.BackColor = clsFunction.MKBackColor;
                    break;
                case 21: // Missing/Incomplete TimeSheet Report
                    btnSearchParticular.Enabled = true;
                    btnFindTerminal.Enabled = true;
                    cboDepartment.Enabled = true;
                    dteDateFrom.Enabled = true;
                    dteDateTo.Enabled = true;
                    chkMissing.Enabled = true;
                    chkMissing.Checked = true;

                    txtName.BackColor = txtTerminalID.BackColor = txtTerminalName.BackColor = clsFunction.MKBackColor;
                    break;


                case 36: // ROCKY - SECURITY BANK BILLING: ENABLED CREATE INVOIC - PRINT OPTION

                    dteDateFrom.Enabled = true;
                    dteDateTo.Enabled = true;
                    break;

                case 37: // ROCKY - METROBANK SERVICING: ENABLED CREATE INVOICE - PRINT OPTION

                    btnCreateInvoice.Enabled = true;
                    dteDateFrom.Value = new DateTime(2000, 1, 1);
                    dteDateFrom.Enabled = true;
                    dteDateTo.Enabled = true;
                    break;

                case 38: // ROCKY - METROBANK NET MATRIX: ENABLED CREATE INVOICE - PRINT OPTION

                    btnCreateInvoice.Enabled = true;
                    dteDateFrom.Value = new DateTime(2000, 1, 1);
                    dteDateFrom.Enabled = true;
                    dteDateTo.Enabled = true;
                    btnGenExcel.Enabled = true;

                    break;

                case 40:
                    btnCreateInvoice.Enabled = true;
                    break;

                default:
                    break;
            }
        }

        private void ResetSearchValue()
        {
            clsSearch.ClassReportID = 0;
            clsSearch.ClassParticularID = 0;
            clsSearch.ClassDepartmentID = 0;
            clsSearch.ClassDepartment = clsFunction.sDash;
            clsSearch.ClassMobileTerminalID = clsFunction.sNull;
            clsSearch.ClassDateFrom = clsFunction.sDateDefault;
            clsSearch.ClassDateTo = clsFunction.sDateDefault;
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void frmPrintOption_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private string SetDatePickerValue(DateTimePicker objDate, string pFormat)
        {
            string sTemp = clsFunction.sDateFormat;
            DateTime stDate = objDate.Value;
            string sDate = stDate.ToString(pFormat);

            sTemp = sDate;

            return sTemp;
        }

        private void dteDateFrom_ValueChanged(object sender, EventArgs e)
        {
            clsSearch.ClassDateFrom = SetDatePickerValue(dteDateFrom, "yyyy-MM-dd");
        }

        private void dteDateTo_ValueChanged(object sender, EventArgs e)
        {
            clsSearch.ClassDateTo = SetDatePickerValue(dteDateTo, "yyyy-MM-dd");
        }

        private void btnFindTerminal_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iTimeSheetTerminal;
            frmSearchField.sHeader = "TIMESHEET TERMINAL";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                txtTID.Text = clsSearch.ClassTSID.ToString();
                txtTerminalID.Text = clsSearch.ClassTSTerminalID;
                txtTerminalName.Text = clsSearch.ClassTSTerminalName;

                txtTerminalID.BackColor = clsFunction.MKBackColor;
            }
        }

        // ROCKY - BILLING: ADD SERVICING INVOICE - PRINT OPTIONS
        private void btnCreateInvoice_Click(object sender, EventArgs e)
        {
            switch (clsReport.ClassReportID)
            {
                case 37:

                    clsSearch.ClassDateFrom = dteDateFrom.Value.ToString("yyyy-MM-dd");
                    clsSearch.ClassDateTo = dteDateTo.Value.ToString("yyyy-MM-dd");
                    clsSearch.ClassSearchValue = $"{dteDateFrom.Value.ToString("yyyy-MM-dd")}|{dteDateTo.Value.ToString("yyyy-MM-dd")}";

                    AppReports.ReportPreview(1001);
                    break;

                case 38:

                    clsSearch.ClassDateFrom = dteDateFrom.Value.ToString("yyyy-MM-dd");
                    clsSearch.ClassDateTo   = dteDateTo.Value.ToString("yyyy-MM-dd");               
                    clsSearch.ClassSearchValue = $"{dteDateFrom.Value.ToString("yyyy-MM-dd")}|{dteDateTo.Value.ToString("yyyy-MM-dd")}";

                    AppReports.ReportPreview(1002);

                    break;

                case 40:

                    clsSearch.ClassDateFrom = dteDateFrom.Value.ToString("yyyy-MM-dd");
                    clsSearch.ClassDateTo   = dteDateTo.Value.ToString("yyyy-MM-dd");
                    clsSearch.ClassSearchValue = $"{dteDateFrom.Value.ToString("yyyy-MM-dd")}|{dteDateTo.Value.ToString("yyyy-MM-dd")}";

                    AppReports.ReportPreview(1003);

                    break;
            }
        }

        private void btnGenExcel_Click(object sender, EventArgs e)
        {
            switch (clsReport.ClassReportID)
            {
                case 37:

                    break;

                case 38:
                    AppReports.ExportExcel.MccNetMatrixStr($"{dteDateFrom.Value.ToString("yyyy-MM-dd")}|{dteDateTo.Value.ToString("yyyy-MM-dd")}");
                    break;

                default:
                    MessageBox.Show("No Report Found ..","Exporting Data",MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }
    }
}
