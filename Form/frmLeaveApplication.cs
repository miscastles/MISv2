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

namespace MIS
{
    public partial class frmLeaveApplication : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        private clsReportFunc dbReportFunc;
        public static string sHeader;
        
        bool fEdit = false;

        public frmLeaveApplication()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtReasonDescription_TextChanged(object sender, EventArgs e)
        {

        }

        private void LoadDataHoliday()
        {
            int i = 0;            

            dbFunction.ClearListViewItems(lvwHoliday);

            clsSearch.ClassSearchValue = clsFunction.sZero + clsFunction.sPipe;
            dbAPI.ExecuteAPI("GET", "View", "Holiday List", clsSearch.ClassSearchValue, "Holiday", "", "ViewHoliday");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.HolidayID.Length > i)
                {
                    clsHoliday.ClassHolidayID = int.Parse(clsArray.HolidayID[i]);
                    clsHoliday.ClassDescription = clsArray.HolidayDesc[i];
                    clsHoliday.ClassHolidayDate = clsArray.HolidayDate[i];     
                    clsHoliday.ClassisActive = int.Parse(clsArray.HolidayisActive[i]);

                    i++;

                    AddItem(i);
                }
            }

            dbFunction.ListViewAlternateBackColor(lvwHoliday);

        }
        private void AddItem(int inLineNo)
        {
            string sDayOfWeek = "";

            // Add to List            
            ListViewItem item = new ListViewItem(inLineNo.ToString());
            item.SubItems.Add(clsHoliday.ClassHolidayID.ToString());
            item.SubItems.Add(clsHoliday.ClassDescription.ToString());
            item.SubItems.Add(clsHoliday.ClassHolidayDate.ToString());

            // Day of Week
            string sYear = DateTime.Now.Year.ToString();
            string sHolidayDate = clsHoliday.ClassHolidayDate + "-" + sYear;
            Debug.WriteLine("sHolidayDate=" + sHolidayDate);

            DateTime dteHoliday;
            DateTime.TryParse(sHolidayDate, out dteHoliday);
            Debug.WriteLine("clsHoliday.ClassHolidayDate=" + clsHoliday.ClassHolidayDate);
            Debug.WriteLine("DateTime.Now.Year=" + DateTime.Now.Year);
            Debug.WriteLine("dteHoliday=" + dteHoliday);

            if (isValidDate(dteHoliday.ToString()))
            {
                DayOfWeek dteDayOfWeek = dteHoliday.DayOfWeek;
                sDayOfWeek = dteDayOfWeek.ToString().ToUpper();                
            }
            else
            {
                sDayOfWeek = clsFunction.sDash;
            }

            item.SubItems.Add(sDayOfWeek);

            item.SubItems.Add(clsHoliday.ClassisActive.ToString());
            lvwHoliday.Items.Add(item);
        }

        private void frmLeaveApplication_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            dbReportFunc = new clsReportFunc();

            fEdit = false;
            dbFunction.ClearTextBox(this);            
            dbFunction.TextBoxUnLock(false, this);
            InitButton();
            LoadDataHoliday();
            InitDate();

            dbAPI.FillComboBoxLeaveType(cboLeaveType);
            dbAPI.FillComboBoxDateType(cboDateType);
            
            cboLeaveType.SelectedIndex = 0;
            cboDateType.SelectedIndex = 0;
            btnSearchLeave.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void InitButton()
        {
            if (fEdit)
            {
                btnAdd.Enabled = false;
                btnSave.Enabled = true;
            }
            else
            {
                btnAdd.Enabled = true;
                btnSave.Enabled = false;
            }
        }
        private bool ValidateFields()
        {
            bool fValid = false;

            if (dbFunction.isValidID(txtParticularID.Text) && dbFunction.isValidID(txtLeaveTypeID.Text))
                fValid = true;

            if (!fValid)
            {
                MessageBox.Show("Check the following field(s) listed below:\n\n" +
                                "*Name\n" +
                                "*Leave Type\n" +
                                "\n" +
                                "Field(s) with asterisk(*) must not be blank.", "Incomplete Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return fValid;
        }

        private void btnAddHoliday_Click(object sender, EventArgs e)
        {
            frmHoliday frm = new frmHoliday();
            frm.ShowDialog();
        }

        private void btnRefreshHoliday_Click(object sender, EventArgs e)
        {
            LoadDataHoliday();
        }

        private void btnSearchParticular_Click(object sender, EventArgs e)
        {
            btnSearchLeave.Enabled = false;
            btnDelete.Enabled = false;
            frmSearchField.iSearchType = frmSearchField.SearchType.iFE;
            frmSearchField.sHeader = "EMPLOYEE";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                dbFunction.ClearTextBox(this);

                txtParticularID.Text = clsSearch.ClassParticularID.ToString();
                txtName.Text = clsSearch.ClassParticularName;
                txtDepartment.Text = clsSearch.ClassDepartment;
                txtPosition.Text = clsSearch.ClassPosition;
                txtEmploymentStatus.Text = clsSearch.ClassEmploymentStatus;

                dbFunction.TextBoxUnLock(true, this);
                txtName.BackColor = clsFunction.MKBackColor;
                
                btnAdd.Enabled = false;
                btnSave.Enabled = true;
                btnSearchLeave.Enabled = true;
            }            
        }

        private bool ValidateHoliday(DateTime objFrom, DateTime objTo)
        {
            bool isValid = false;

            return isValid;
        }

        private bool ValidateWeekEnd(DateTimePicker objFrom, DateTimePicker objTo)
        {
            bool isValid = true;            
            DateTime dteDateFrom;
            DateTime dteDateTo;

            if (!chkWeekend.Checked) return isValid;

            dteDateFrom = DateTime.Parse(objFrom.Value.ToString());
            dteDateTo = DateTime.Parse(objTo.Value.ToString());

            foreach (DateTime day in EachDay(dteDateFrom, dteDateTo))
            {
                Debug.WriteLine("day="+ day);

                DayOfWeek today = day.DayOfWeek;

                if (today == DayOfWeek.Saturday || today == DayOfWeek.Sunday)
                {
                    isValid = false;
                    dbFunction.SetMessageBox("Weekend found from selected start/end date." + "\n\n" +
                                             "Selected date below:" + "\n" +
                                             "Start date " + dteDateFrom.ToString("ddd, MM-dd-yyyy") + "\n" +
                                             "End date " + dteDateTo.ToString("ddd, MM-dd-yyyy") + "\n"
                                             ,"Invalid date", clsFunction.IconType.iError);
                    break;
                }
            }
            
            return isValid;
        }

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
            {
                yield return day;
            }
        }

        private void InitDate()
        {
            dteDateFrom.Value = DateTime.Now.Date;
            //dbFunction.SetDateFormatWithWeekDay(dteDateFrom);
            dbFunction.SetDateFormat(dteDateFrom, clsFunction.sDateDefaultFormat);

            dteDateTo.Value = DateTime.Now;
            //dbFunction.SetDateFormatWithWeekDay(dteDateTo);
            dbFunction.SetDateFormat(dteDateTo, clsFunction.sDateDefaultFormat);

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            fEdit = false;
            dbFunction.ClearTextBox(this);            
            dbFunction.TextBoxUnLock(false, this);
            InitButton();            
            InitDate();
            
            cboLeaveType.SelectedIndex = 0;
            cboDateType.SelectedIndex = 0;
            btnSearchLeave.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Set DateFrom / DateTo
            //dbFunction.SetDateFormatWithWeekDay(dteDateFrom);            
            //dbFunction.SetDateFormatWithWeekDay(dteDateTo);
            dbFunction.SetDateFormat(dteDateFrom, clsFunction.sDateDefaultFormat);
            dbFunction.SetDateFormat(dteDateTo, clsFunction.sDateDefaultFormat);

            // Check Date Type
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iDateType, cboDateType.Text)) return;

            // Check Date Range            
            if (!CheckDateFromTo(dteDateFrom, dteDateTo)) return;

            // Check WeekEnd           
            //if (!ValidateWeekEnd(dteDateFrom, dteDateTo)) return;

            // Holiday            
            //if (!ValidateHoliday(dteDateFrom, dteDateTo)) return;
                        
            if (!ValidateFields()) return;

            // Check Leave Balance
            if (!isValidLeaveBalance(txtName.Text, double.Parse(txtLeaveBalance.Text))) return;

            if (!fEdit)
            {
                if (!ValidateDateLeave(dteDateFrom, dteDateTo)) return;
            }
            
            if (!fConfirmDetails()) return;

            // Compute Duration
            ComputeDuration();

            Cursor.Current = Cursors.WaitCursor;

            if (fEdit)
            {
                              
            }
            else
            {
                // check if already exist

                SaveLeaveApplication();
            }
            
            if (fEdit)
            {
                dbFunction.SetMessageBox("Leave application has been successfully modified.", "Updated", clsFunction.IconType.iExclamation);
            }
            else
            {
                dbFunction.SetMessageBox("Leave application has been successfully added.", "Saved", clsFunction.IconType.iInformation);
            }

            if (MessageBox.Show("Do you want another entry for " + txtName.Text + "?", "Confirm entry", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                btnClear_Click(this, e);
            }
            else
            {
                //Initialize object
                txtDuration.Text = clsFunction.sZero;
                cboDateType.SelectedIndex = 0;
                txtReasonID.Text = clsFunction.sZero;
                txtReasonDescription.Text = clsFunction.sNull;
                txtRemarks.Text = clsFunction.sNull;

                //LoadLeaveApplication(txtParticularID.Text);

                cboLeaveType_SelectedIndexChanged(sender, e);

            }            

            Cursor.Current = Cursors.Default;
        }

        private bool CheckDateFromTo(DateTimePicker objFrom, DateTimePicker objTo)
        {
            bool fValid = true;
            int iResult;

            iResult = DateTime.Compare(DateTime.Parse(objFrom.Value.ToShortDateString()), DateTime.Parse(objTo.Value.ToShortDateString()));

            if (iResult > 0)
                fValid = false;

            if (!fValid)
            {
                MessageBox.Show("[Date From] shouldn't be greater than [Date To]" +
                                        "\n\n" +
                                        "Date From: " + objFrom.Value.ToString("MM-dd-yyyy") +
                                        "\n" +
                                        "Date To:      " + objTo.Value.ToString("MM-dd-yyyy"), "Date Range", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

            }

            return fValid;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            fEdit = false;
            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(true, this);            
            InitButton();
            btnSave.Enabled = true;
            btnAdd.Enabled = false;            
            btnSearchParticular.Enabled = true;

            txtName.BackColor = clsFunction.MKBackColor;
            txtName.Focus();

        }

        private bool isValidDate(string pDate)
        {
            DateTime tempDate;
            return DateTime.TryParse(pDate, out tempDate);
        }

        
        private void SaveLeaveApplication()
        {
            string sRowSQL = "";
            string sSQL = "";

            Debug.WriteLine("--SaveLeaveApplication--");
            Debug.WriteLine("fEdit=" + fEdit);

            //string sDateFrom = dbFunction.GetDateFromParse(dteDateFrom.Text, "ddd, MM-dd-yyyy", "yyyy-MM-dd");
            //string sDateTo = dbFunction.GetDateFromParse(dteDateTo.Text, "ddd, MM-dd-yyyy", "yyyy-MM-dd");

            DateTime stDateFrom = dteDateFrom.Value;
            DateTime stDateTo = dteDateTo.Value;
            
            string sDateFrom = stDateFrom.ToString("yyyy-MM-dd");
            string sDateTo = stDateTo.ToString("yyyy-MM-dd");
            
            dbFunction.GetProcessedByAndDateTime(); // Get processedby and datetime

            dbFunction.GetModifiedByAndDateTime(); // Get modifiedby and datetime

            // Insert
            sRowSQL = "";
            sSQL = "";
            sRowSQL = " (" + dbFunction.CheckAndSetNumericValue(txtParticularID.Text) + ", " +
            sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(txtLeaveTypeID.Text) + ", " +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(txtLeaveCode.Text) + "', " +
            sRowSQL + sRowSQL + " '" + sDateFrom + "', " +
            sRowSQL + sRowSQL + " '" + sDateTo + "', " +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtDuration.Text) + "', " +
            sRowSQL + sRowSQL + "'" + clsUser.ClassProcessedBy + "'," +
            sRowSQL + sRowSQL + "'" + clsUser.ClassProcessedDateTime + "'," +
            sRowSQL + sRowSQL + "'" + clsUser.ClassModifiedBy + "'," +
            sRowSQL + sRowSQL + "'" + clsUser.ClassModifiedDateTime + "'," +
            sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(txtReasonID.Text) + ", " +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(cboDateType.Text) + "', " +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(txtRemarks.Text) + "'," +
            sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(chkIsActive.Checked ? clsFunction.sOne : clsFunction.sZero) + ") ";
            sSQL = sSQL + sRowSQL;

            Debug.WriteLine("sSQL=" + sSQL);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "Leave Movement", sSQL, "InsertCollectionDetail");
        }
        

        private void cboLeaveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int outLeaveTypeID = 0;
            string outCode = "";
            string outDescription = "";
            double outCreditLimit = 0.00;
            
            clsSearch.ClassLeaveTypeID = 0;
            if (!cboLeaveType.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetLeaveTypeInfoFromFile("LeaveType", cboLeaveType.Text, out outLeaveTypeID, out outCode, out outDescription, out outCreditLimit);
                clsSearch.ClassLeaveTypeID = outLeaveTypeID;
                txtLeaveTypeID.Text = clsSearch.ClassLeaveTypeID.ToString();
                txtLeaveCode.Text = outCode;
                txtCreditLimit.Text = txtLeaveCredit.Text = outCreditLimit.ToString("N");

                if (dbFunction.isValidID(txtParticularID.Text) && dbFunction.isValidID(txtLeaveTypeID.Text))               
                 LeaveAssignment(txtParticularID.Text, txtLeaveTypeID.Text); // Load Leave Credit

                ComputeLeave();

            }
        }

        private bool fConfirmDetails()
        {
            bool fConfirm = true;
            string sTemp = "";

            sTemp = (fEdit ? "Are you sure to update the following details below:\n\n" : "Are you to save the following details below:\n\n") +
                           clsFunction.sLineSeparator + "\n" +
                           "> Leave Application" + "\n" +
                           clsFunction.sLineSeparator + "\n" +
                           "Name: " + txtName.Text + "\n" +
                           "Department: " + txtDepartment.Text + "\n" +
                           "Position: " + txtPosition.Text + "\n" +
                           "Employment: " + txtEmploymentStatus.Text + "\n" +                           
                           clsFunction.sLineSeparator + "\n" +
                           "Leave Type: " + cboLeaveType.Text + "\n" +
                           "Start Date: " + dteDateFrom.Value.ToString("MM-dd-yyyy") + "\n" +
                           "End Date: " + dteDateTo.Value.ToString("MM-dd-yyyy") + "\n" +
                           "Date Type: " + cboDateType.Text + "\n" +
                           "Duration: " + txtDuration.Text + "\n" +
                           clsFunction.sLineSeparator + "\n" +
                           "Reason: " + txtReasonDescription.Text + "\n" +
                           "Remarks: " + txtRemarks.Text + "\n" +
                           clsFunction.sLineSeparator + "\n";

            if (MessageBox.Show(sTemp,(fEdit ? "Confirm Update" : "Confirm Saving"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fConfirm = false;
            }

            return fConfirm;
        }

        private void btnDuration_Click(object sender, EventArgs e)
        {
            // Set DateFrom / DateTo
            //dbFunction.SetDateFormatWithWeekDay(dteDateFrom);
            //dbFunction.SetDateFormatWithWeekDay(dteDateTo);
            dbFunction.SetDateFormat(dteDateFrom, clsFunction.sDateDefaultFormat);
            dbFunction.SetDateFormat(dteDateTo, clsFunction.sDateDefaultFormat);

            // Check Date Type
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iDateType, cboDateType.Text)) return;

            // Check Date Range            
            if (!CheckDateFromTo(dteDateFrom, dteDateTo)) return;

            // Check WeekEnd
            //if (!ValidateWeekEnd(dteDateFrom, dteDateTo)) return;

            // Check Holiday
            //if (!ValidateHoliday(dteDateFrom, dteDateTo)) return;

            ComputeDuration();            
        }

        private void ComputeDuration()
        {
            double dblInterval = 0.0;

            // Compute Duration          
            if (cboDateType.Text.Equals("WHOLEDAY"))
            {
                dblInterval = 1.0;
            }

            if (cboDateType.Text.Equals("HALFDAY"))
            {
                dblInterval = 0.5;
            }

            TimeSpan timeSpan = DateTime.Parse(dteDateTo.Value.ToString()) - DateTime.Parse(dteDateFrom.Value.ToString());
            int inDays = timeSpan.Days + 1;            
            double dblDuaration = inDays * dblInterval;
            txtDuration.Text = (dblDuaration > 0 ? dblDuaration.ToString("N") : "0.00");

            Debug.WriteLine("dblDuaration=" + dblDuaration);
        }

        private void btnSearchReason_Click(object sender, EventArgs e)
        {
            clsSearch._isWriteResponse = true;
            frmSearchField.iSearchType = frmSearchField.SearchType.iNegativeReason;
            frmSearchField.sHeader = "REASON";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();
            clsSearch._isWriteResponse = false;

            if (frmSearchField.fSelected)
            {
                txtReasonID.Text = clsSearch.ClassReasonID.ToString();                
                txtReasonDescription.Text = clsSearch.ClassReasonDescription;
            }
        }

        private void btnAddReason_Click(object sender, EventArgs e)
        {
            frmReason.sHeader = clsGlobalVariables.REASON_TYPE;
            clsSearch.ClassReasonType = clsGlobalVariables.REASON_TYPE;
            frmReason frm = new frmReason();
            frm.ShowDialog();
        }

        private void btnRemoveReason_Click(object sender, EventArgs e)
        {
            txtReasonID.Text = clsFunction.sZero;            
            txtReasonDescription.Text = clsFunction.sNull;
        }

        private void LeaveAssignment(string pParticularID, string pLeaveTypeID)
        {
            int i = 0;
            clsSearch.ClassSearchValue = pParticularID + clsFunction.sPipe + pLeaveTypeID;
            dbAPI.ExecuteAPI("GET", "Search", "Particular Leave Assignment", clsSearch.ClassSearchValue, "Particular Detail", "", "ViewParticularDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.LeaveTypeID.Length > i)
                {                    
                    clsLeaveType.ClassCreditLimit = double.Parse(clsArray.LeaveTypeCreditLimit[i]);
                    clsLeaveType.ClassLeaveCredit = double.Parse(clsArray.LeaveCredit[i]);

                    i++;
                    
                }

                txtCreditLimit.Text = (clsLeaveType.ClassCreditLimit > 0 ? clsLeaveType.ClassCreditLimit.ToString("N") : "0.00");
                txtLeaveCredit.Text = (clsLeaveType.ClassLeaveCredit > 0 ? clsLeaveType.ClassLeaveCredit.ToString("N") : "0.00");                
            }
        }
        
        private void ComputeLeave()
        {   
            double dblLeaveCredit = 0.00;
            double dblLeaveUsed = 0.00;
            double dblLeaveBalance = 0.00;

            if (dbFunction.isValidID(txtParticularID.Text) && dbFunction.isValidID(txtLeaveTypeID.Text))
            {
                clsSearch.ClassSearchValue = txtParticularID.Text + clsFunction.sPipe + txtLeaveTypeID.Text;
                dbAPI.GetViewTotal("Search", "Particular Leave Count", clsSearch.ClassSearchValue, "Get Total");
                if (dbAPI.isNoRecordFound() == false)
                    dblLeaveUsed = clsTerminal.ClassTerminalTotal;
            }

            dblLeaveCredit = double.Parse(txtLeaveCredit.Text);
            txtLeaveUsed.Text = (dblLeaveUsed > 0 ? dblLeaveUsed.ToString("N") : "0.00");
            dblLeaveBalance = dblLeaveCredit - dblLeaveUsed;
            txtLeaveBalance.Text = (dblLeaveBalance > 0 ? dblLeaveBalance.ToString("N") : "0.00");

        }

        private bool ValidateDateLeave(DateTimePicker objFrom, DateTimePicker objTo)
        {
            bool isValid = true;
            DateTime dteDateFrom;
            DateTime dteDateTo;
            string sLeaveDate = clsFunction.sNull;
            bool isExist = false;

            dteDateFrom = DateTime.Parse(objFrom.Value.ToString());
            dteDateTo = DateTime.Parse(objTo.Value.ToString());

            foreach (DateTime day in EachDay(dteDateFrom, dteDateTo))
            {
                sLeaveDate = dteDateFrom.ToString("yyyy-MM-dd");

                Debug.WriteLine("sLeaveDate=" + sLeaveDate);

                clsSearch.ClassSearchValue = txtParticularID.Text + clsFunction.sPipe + sLeaveDate;
                isExist = dbAPI.isRecordExist("Search", "Particular Leave Date", clsSearch.ClassSearchValue);
                if (isExist)
                    break;
            }

            if (isExist)
            {
                isValid = false;
                dbFunction.SetMessageBox("Leave date " + sLeaveDate + " already exist for employee\n" + txtName.Text, "Leave date exist", clsFunction.IconType.iExclamation);
            }

            return isValid;
        }

        private bool ValidateHoliday(DateTimePicker objFrom, DateTimePicker objTo)
        {
            bool isValid = true;
            DateTime dteDateFrom;
            DateTime dteDateTo;
            string sLeaveDate = clsFunction.sNull;
            string sLeaveMosDay = clsFunction.sNull;
            bool isExist = false;
            string sMonthDay = clsFunction.sNull;
            string sHoliday = clsFunction.sNull;

            if (!chkHoliday.Checked) return isValid;

            dteDateFrom = DateTime.Parse(objFrom.Value.ToString());
            dteDateTo = DateTime.Parse(objTo.Value.ToString());

            foreach (DateTime day in EachDay(dteDateFrom, dteDateTo))
            {
                sLeaveDate = dteDateFrom.ToString("yyyy-MM-dd");
                sLeaveMosDay = sLeaveDate.Substring(5, 5);

                Debug.WriteLine("sLeaveDate=" + sLeaveDate);
                Debug.WriteLine("sLeaveMosDay=" + sLeaveMosDay);

                // Loop here for holiday list
                foreach (ListViewItem i in lvwHoliday.Items)
                {
                    sHoliday = i.SubItems[2].Text;
                    sMonthDay = i.SubItems[3].Text;

                    if (sMonthDay.Equals(sLeaveMosDay))
                    {
                        isExist = true;
                        break;
                    }
                }
            }

            if (isExist)
            {
                isValid = false;
                dbFunction.SetMessageBox("Date " + sLeaveDate + " is " + sHoliday + " holiday." , "Holiday date exist", clsFunction.IconType.iExclamation);
            }

            return isValid;
        }

        private void btnPreviewHoliday_Click(object sender, EventArgs e)
        {
            if (lvwHoliday.Items.Count > 0)
                dbReportFunc.ViewHoliday();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (txtParticularID.TextLength > 0 && txtName.TextLength > 0)
            {
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                clsSearch.ClassReportID = 16;
                clsSearch.ClassParticularID = int.Parse(dbFunction.CheckAndSetNumericValue(txtParticularID.Text));
                clsSearch.ClassDepartmentID = clsFunction.iDefaultID;
                clsSearch.ClassDateFrom = clsFunction.sDateFormat;
                clsSearch.ClassDateTo = clsFunction.sDateFormat;

                dbReportFunc.ViewLeaveApplication();

                Cursor.Current = Cursors.Default; // Back to normal 
            }
            else
            {
                MessageBox.Show("Check the following field(s) listed below:\n\n" +
                                "*Name\n" +
                                "\n" +
                                "Field(s) with asterisk(*) must not be blank.", "Incomplete Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void frmLeaveApplication_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private bool isValidLeaveBalance(string pName, double dblLeaveBalance)
        {
            bool isValid = false;

            if (dblLeaveBalance > 0)
                isValid = true;
            else
                isValid = false;

            if (!isValid)
            {
                if (MessageBox.Show("Particular " + pName + " has no more leave balance." +
                                    "\n\n" +
                                    "Do you still want to continue?" +
                                    "\n\n",
                                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    isValid = false;
                }
                else
                {
                    isValid = true;
                }
            }

            return isValid;
        }

        private void btnSearchLeave_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iParticularID, txtParticularID.Text)) return;

            clsSearch.ClassParticularID = int.Parse(dbFunction.CheckAndSetNumericValue(txtParticularID.Text));
            frmSearchField.iSearchType = frmSearchField.SearchType.iLeaveDetail;
            frmSearchField.sTerminalType = clsFunction.sNull;
            frmSearchField.sHeader = "Particular Leave Movement";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                // Selected
                txtLeaveNo.Text = clsSearch.ClassLeaveNo.ToString();
                txtLeaveTypeID.Text = clsSearch.ClassLeaveTypeID.ToString();
                cboLeaveType.Text = clsSearch.ClassDescription;
                dteDateFrom.Value = DateTime.Parse(clsSearch.ClassDateFrom);
                dteDateTo.Value = DateTime.Parse(clsSearch.ClassDateTo);
                cboDateType.Text = clsSearch.ClassDateType;
                txtDuration.Text = clsSearch.ClassDuration;
                txtReasonID.Text = clsSearch.ClassReasonID.ToString();
                txtReasonDescription.Text = clsSearch.ClassReasonDescription;
                txtRemarks.Text = clsSearch.ClassRemarks;

                btnDelete.Enabled = true;
                btnAdd.Enabled = false;
                btnSave.Enabled = false;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            bool fConfirm = false;
            string sTemp = "";

            sTemp = "Are you sure to delete the following details below:\n\n" +
                    clsFunction.sLineSeparator + "\n" +
                    "> Delete Leave Application" + "\n" +
                    clsFunction.sLineSeparator + "\n" +
                    "Name: " + txtName.Text + "\n" +
                    "Department: " + txtDepartment.Text + "\n" +
                    "Position: " + txtPosition.Text + "\n" +
                    "Employment: " + txtEmploymentStatus.Text + "\n" +
                    clsFunction.sLineSeparator + "\n" +
                    "Leave ID: " + txtLeaveNo.Text + "\n" +
                    "Particular ID: " + txtParticularID.Text + "\n" +
                    clsFunction.sLineSeparator + "\n" +
                    "Leave Type: " + cboLeaveType.Text + "\n" +
                    "Start Date: " + dteDateFrom.Value.ToString("MM-dd-yyyy") + "\n" +
                    "End Date: " + dteDateTo.Value.ToString("MM-dd-yyyy") + "\n" +
                    "Date Type: " + cboDateType.Text + "\n" +
                    "Duration: " + txtDuration.Text + "\n" +
                    clsFunction.sLineSeparator + "\n" +
                    "Reason: " + txtReasonDescription.Text + "\n" +
                    "Remarks: " + txtRemarks.Text + "\n" +
                    clsFunction.sLineSeparator + "\n\n\n" +
                    "Note: Warning! Record will permanently deleted.";

            if (MessageBox.Show(sTemp, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                fConfirm = true;
            }

            if (fConfirm)
            {
                clsSearch.ClassAdvanceSearchValue = txtLeaveNo.Text + clsFunction.sPipe + txtParticularID.Text;
                dbAPI.ExecuteAPI("DELETE", "Delete", "Leave Movement", clsSearch.ClassAdvanceSearchValue, "Leave Movement", "", "DeleteCollectionDetail");

                dbFunction.SetMessageBox("Leave Application has been successfully deleted.", "Leave Application", clsFunction.IconType.iInformation);

                btnClear_Click(this, e);
            }
        }
    }
}
