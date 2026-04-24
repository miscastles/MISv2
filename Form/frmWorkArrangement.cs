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
    public partial class frmWorkArrangement : Form
    {
        private clsAPI dbAPI;
        private clsReportFunc dbReportFunc;
        private clsFunction dbFunction;
        public static string sHeader;
        
        bool fEdit = false;

        public frmWorkArrangement()
        {
            InitializeComponent();
        }

        private void frmWorkArrangement_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            dbReportFunc = new clsReportFunc();

            fEdit = false;
            dbFunction.ClearTextBox(this);            
            dbFunction.TextBoxUnLock(false, this);
            InitButton();
            LoadHoliday();
            InitDate();

            dbAPI.FillComboBoxWorkType(cboWorkType);            
            
            cboWorkType.SelectedIndex = 0;
            btnSearchWorkArrangement.Enabled = false;
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

            if (txtName.TextLength > 0 && dbFunction.isValidID(txtWorkTypeID.Text) && dbFunction.isValidID(txtParticularID.Text) && cboDateType.Text.Length > 0 && cboWorkType.Text.Length > 0)
                fValid = true;

            if (!fValid)
            {
                MessageBox.Show("Check the following field(s) listed below:\n\n" +
                                "*Name\n" +
                                "*Work Type\n" +
                                "*Date Type\n" +
                                "\n" +
                                "Field(s) with asterisk(*) must not be blank.", "Incomplete Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return fValid;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadHoliday()
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
                sDayOfWeek = dteDayOfWeek.ToString();
            }
            else
            {
                sDayOfWeek = clsFunction.sDash;
            }

            item.SubItems.Add(sDayOfWeek);

            item.SubItems.Add(clsHoliday.ClassisActive.ToString());
            lvwHoliday.Items.Add(item);
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

        private bool isValidDate(string pDate)
        {
            DateTime tempDate;
            return DateTime.TryParse(pDate, out tempDate);
        }

        private void btnSearchParticular_Click(object sender, EventArgs e)
        {
            btnSearchWorkArrangement.Enabled = false;
            btnDelete.Enabled = false;
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
                txtEmploymentStatus.Text = clsSearch.ClassEmploymentStatus;

                txtName.BackColor = clsFunction.MKBackColor;
                btnSearchWorkArrangement.Enabled = true;
            }
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

            txtRemarks.Enabled = true;
            txtRemarks.BackColor = Color.White;

            txtName.BackColor = clsFunction.MKBackColor;
            txtName.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            fEdit = false;
            dbFunction.ClearTextBox(this);            
            dbFunction.TextBoxUnLock(false, this);
            InitButton();
            InitDate();            

            cboWorkType.SelectedIndex = 0;
            btnSearchWorkArrangement.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Set DateFrom / DateTo
            //dbFunction.SetDateFormatWithWeekDay(dteDateFrom);
            //dbFunction.SetDateFormatWithWeekDay(dteDateTo);
            dbFunction.SetDateFormat(dteDateFrom, clsFunction.sDateDefaultFormat);
            dbFunction.SetDateFormat(dteDateTo, clsFunction.sDateDefaultFormat);

            // Check Date Range            
            if (!CheckDateFromTo(dteDateFrom, dteDateTo)) return;

            // Check WeekEnd
            if (!ValidateWeekEnd(dteDateFrom, dteDateTo)) return;

            // Holiday
            if (!ValidateHoliday(dteDateFrom, dteDateTo)) return;

            if (!ValidateFields()) return;

            if (!fEdit)
            {
                if (!ValidateDateWorkArrangement(dteDateFrom, dteDateTo)) return;
            }

            if (!fConfirmDetails()) return;

            ComputeDuration();

            Cursor.Current = Cursors.WaitCursor;

            if (fEdit)
            {
                // Update already saved
                if (dbFunction.isValidID(txtID.Text) && dbFunction.isValidID(txtParticularID.Text))
                {

                }
            }
            else
            {
                SaveWorkArrangement();
            }

            if (fEdit)
            {
                dbFunction.SetMessageBox("Work arrangement has been successfully modified.", "Updated", clsFunction.IconType.iExclamation);
            }
            else
            {
                dbFunction.SetMessageBox("Work arrangement has been successfully added.", "Saved", clsFunction.IconType.iInformation);
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
                                        Environment.NewLine +
                                        "Date To:      " + objTo.Value.ToString("MM-dd-yyyy"), "Date Range", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

            }

            return fValid;
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
                Debug.WriteLine("day=" + day);

                DayOfWeek today = day.DayOfWeek;

                if (today == DayOfWeek.Saturday || today == DayOfWeek.Sunday)
                {
                    isValid = false;
                    dbFunction.SetMessageBox("Weekend found from selected start/end date." + "\n\n" +
                                             "Selected date below:" + "\n" +
                                             "Start date " + dteDateFrom.ToString("ddd, MM-dd-yyyy") + "\n" +
                                             "End date " + dteDateTo.ToString("ddd, MM-dd-yyyy") + "\n"
                                             , "Invalid date", clsFunction.IconType.iError);
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
                dbFunction.SetMessageBox("Date " + sLeaveDate + " is " + sHoliday + " holiday.", "Holiday date exist", clsFunction.IconType.iExclamation);
            }

            return isValid;
        }

        private bool fConfirmDetails()
        {
            bool fConfirm = true;
            string sTemp = "";

            sTemp = (fEdit ? "Are you sure to update the following details below:\n\n" : "Are you to save the following details below:\n\n") +
                           clsFunction.sLineSeparator + "\n" +
                           "> Work Arrangement" + "\n" +
                           clsFunction.sLineSeparator + "\n" +
                           "Name: " + txtName.Text + "\n" +
                           "Department: " + txtDepartment.Text + "\n" +
                           "Position: " + txtPosition.Text + "\n" +
                           "Employment: " + txtEmploymentStatus.Text + "\n" +
                           clsFunction.sLineSeparator + "\n" +
                           "Work Type: " + cboWorkType.Text + "\n" +
                           "Start Date: " + dteDateFrom.Value.ToString("MM-dd-yyyy") + "\n" +
                           "End Date: " + dteDateTo.Value.ToString("MM-dd-yyyy") + "\n" +
                           "Date Type: " + cboDateType.Text + "\n" +
                           "Duration: " + txtDuration.Text + "\n" +
                           clsFunction.sLineSeparator + "\n" +                           
                           "Remarks: " + txtRemarks.Text + "\n" +
                           clsFunction.sLineSeparator + "\n";

            if (MessageBox.Show(sTemp, (fEdit ? "Confirm Update" : "Confirm Saving"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fConfirm = false;
            }

            return fConfirm;
        }

        private bool ValidateDateWorkArrangement(DateTimePicker objFrom, DateTimePicker objTo)
        {
            bool isValid = true;
            DateTime dteDateFrom;
            DateTime dteDateTo;
            string sDate = clsFunction.sNull;
            bool isExist = false;

            dteDateFrom = DateTime.Parse(objFrom.Value.ToString());
            dteDateTo = DateTime.Parse(objTo.Value.ToString());

            foreach (DateTime day in EachDay(dteDateFrom, dteDateTo))
            {
                sDate = dteDateFrom.ToString("yyyy-MM-dd");

                Debug.WriteLine("sDate=" + sDate);

                clsSearch.ClassSearchValue = txtParticularID.Text + clsFunction.sPipe + sDate;
                isExist = dbAPI.isRecordExist("Search", "Particular Work Arrangement Date", clsSearch.ClassSearchValue);
                if (isExist)
                    break;
            }

            if (isExist)
            {
                isValid = false;
                dbFunction.SetMessageBox("Work arrangement date " + sDate + " already exist for employee\n" + txtName.Text, "Work arrangment date exist", clsFunction.IconType.iExclamation);
            }

            return isValid;
        }

        private void SaveWorkArrangement()
        {
            string sRowSQL = "";
            string sSQL = "";

            Debug.WriteLine("--SaveWorkArrangement--");
            Debug.WriteLine("fEdit=" + fEdit);

            DateTime stDateFrom = dteDateFrom.Value;
            DateTime stDateTo = dteDateTo.Value;

            string pDateFrom = stDateFrom.ToString("yyyy-MM-dd");
            string pDateTo = stDateTo.ToString("yyyy-MM-dd");

            dbFunction.GetProcessedByAndDateTime(); // Get processedby and datetime

            dbFunction.GetModifiedByAndDateTime(); // Get modifiedby and datetime

            // Insert
            sRowSQL = "";
            sSQL = "";
            sRowSQL = " (" + dbFunction.CheckAndSetNumericValue(txtParticularID.Text) + ", " +
            sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(txtWorkTypeID.Text) + ", " +            
            sRowSQL + sRowSQL + " '" + pDateFrom + "', " +
            sRowSQL + sRowSQL + " '" + pDateTo + "', " +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtDuration.Text) + "', " +
            sRowSQL + sRowSQL + "'" + clsUser.ClassProcessedBy + "'," +
            sRowSQL + sRowSQL + "'" + clsUser.ClassProcessedDateTime + "'," +
            sRowSQL + sRowSQL + "'" + clsUser.ClassModifiedBy + "'," +
            sRowSQL + sRowSQL + "'" + clsUser.ClassModifiedDateTime + "'," +                        
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(cboDateType.Text) + "'," +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(txtRemarks.Text) + "') ";
            sSQL = sSQL + sRowSQL;

            Debug.WriteLine("sSQL=" + sSQL);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "Work Arrangement", sSQL, "InsertMaintenanceMaster");
        }

        private void cboWorkType_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassWorkTypeID = 0;
            if (!cboWorkType.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("WorkType", cboWorkType.Text);
                clsSearch.ClassWorkTypeID = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassWorkTypeID=" + clsSearch.ClassWorkTypeID);

            }

            txtWorkTypeID.Text = clsSearch.ClassWorkTypeID.ToString();

        }

        private void cboDateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            if (cboDateType.Text.Length > 0)
            {
                ComputeDuration();
            }
            */
        }

        private void btnDuration_Click(object sender, EventArgs e)
        {
            ComputeDuration();
        }

        private void btnAddHoliday_Click(object sender, EventArgs e)
        {
            frmHoliday frm = new frmHoliday();
            frm.ShowDialog();
        }

        private void btnRefreshHoliday_Click(object sender, EventArgs e)
        {
            LoadHoliday();
        }

        private void btnPreviewHoliday_Click(object sender, EventArgs e)
        {
            if (lvwHoliday.Items.Count > 0)
                dbReportFunc.ViewHoliday();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (txtParticularID.TextLength > 0 && txtName.TextLength > 0)
            {
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                clsSearch.ClassReportID = 14;
                clsSearch.ClassParticularID = int.Parse(dbFunction.CheckAndSetNumericValue(txtParticularID.Text));
                clsSearch.ClassDepartmentID = clsFunction.iDefaultID;
                clsSearch.ClassDateFrom = clsFunction.sDateFormat;
                clsSearch.ClassDateTo = clsFunction.sDateFormat;
                
                dbReportFunc.ViewWorkArrangement();

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

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void frmWorkArrangement_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void btnSearchWorkArrangement_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iParticularID, txtParticularID.Text)) return;

            clsSearch.ClassParticularID = int.Parse(dbFunction.CheckAndSetNumericValue(txtParticularID.Text));
            frmSearchField.iSearchType = frmSearchField.SearchType.iWorkArrangement;            
            frmSearchField.sTerminalType = clsFunction.sNull;
            frmSearchField.sHeader = "Particular Work Arrangement";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                // Selected
                txtWorkArrangementID.Text = clsSearch.ClassWorkArrangementID.ToString();
                cboWorkType.Text = clsSearch.ClassDescription;
                dteDateFrom.Value = DateTime.Parse(clsSearch.ClassDateFrom);
                dteDateTo.Value = DateTime.Parse(clsSearch.ClassDateTo);
                cboDateType.Text = clsSearch.ClassDateType;
                txtDuration.Text = clsSearch.ClassDuration;
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
                    "> Delete Work Arrangement" + "\n" +
                    clsFunction.sLineSeparator + "\n" +
                    "Name: " + txtName.Text + "\n" +
                    "Department: " + txtDepartment.Text + "\n" +
                    "Position: " + txtPosition.Text + "\n" +
                    "Employment: " + txtEmploymentStatus.Text + "\n" +
                    clsFunction.sLineSeparator + "\n" +
                    "Work Arrangement ID: " + txtWorkArrangementID.Text + "\n" +
                    "Particular ID: " + txtParticularID.Text + "\n" +
                    clsFunction.sLineSeparator + "\n" +
                    "Work Type: " + cboWorkType.Text + "\n" +
                    "Start Date: " + dteDateFrom.Value.ToString("MM-dd-yyyy") + "\n" +
                    "End Date: " + dteDateTo.Value.ToString("MM-dd-yyyy") + "\n" +
                    "Date Type: " + cboDateType.Text + "\n" +
                    "Duration: " + txtDuration.Text + "\n" +
                    clsFunction.sLineSeparator + "\n" +
                    "Remarks: " + txtRemarks.Text + "\n" +
                    clsFunction.sLineSeparator + "\n\n\n" +
                    "Note: Warning! Record will permanently deleted.";

            if (MessageBox.Show(sTemp, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                fConfirm = true;
            }

            if (fConfirm)
            {
                clsSearch.ClassAdvanceSearchValue = txtWorkArrangementID.Text + clsFunction.sPipe + txtParticularID.Text;
                dbAPI.ExecuteAPI("DELETE", "Delete", "Work Arrangement", clsSearch.ClassAdvanceSearchValue, "Work Arrangement", "", "DeleteCollectionDetail");

                dbFunction.SetMessageBox("Work Arrangement has been successfully deleted.", "Work Arrangement", clsFunction.IconType.iInformation);

                btnClear_Click(this, e);
            }
        }
    }
}
