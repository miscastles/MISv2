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

namespace MIS
{
    public partial class frmTimeSheet : Form
    {
        private clsAPI dbAPI;
        private clsReportFunc dbReportFunc;
        private clsFunction dbFunction;
        private clsFile dbFile;
        public static string sHeader;
        private int iSelectedIndex = 0;
        private bool isPosted = false;

        bool fEdit = false;

        public frmTimeSheet()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void frmTimeSheet_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            dbReportFunc = new clsReportFunc();
            dbFile = new clsFile();

            fEdit = false;
            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ClearListViewItems(lvwTimeLog);
            InitButton();            
            InitDate();
            InitTime();
            isPosted = false;
            btnDeleteTimeSheet.Enabled = false;
            ChangeTimeLogButton(false);
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

        private void InitDate()
        {
            dteDate.Value = DateTime.Now.Date;         
            dbFunction.SetDateFormat(dteDate, clsFunction.sDateDefaultFormat);

            dteDateFrom.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteDateFrom, clsFunction.sDateDefaultFormat);

            dteDateTo.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteDateTo, clsFunction.sDateDefaultFormat);
           
        }

        private void InitTime()
        {
            dteTime.Value = DateTime.Now;                   
            dbFunction.SetTimeFormat(dteTime, clsFunction.sTimeDefaultFormat);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            fEdit = false;
            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ClearListViewItems(lvwTimeLog);
            InitButton();
            InitDate();
            InitTime();
            isPosted = false;
            btnDeleteTimeSheet.Enabled = false;
            ChangeTimeLogButton(false);
        }

        private void btnSearchParticular_Click(object sender, EventArgs e)
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
                txtEmploymentStatus.Text = clsSearch.ClassEmploymentStatus;

                dbFunction.ClearListViewItems(lvwTimeLog);                
                InitDate();
                InitTime();

                txtName.BackColor = clsFunction.MKBackColor;

                ChangeTimeLogButton(true);
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            clsSearch.ClassDateFrom = clsSearch.ClassDateTo = clsFunction.sDateFormat;
             
            // Check Date Range            
            if (!CheckDateFromTo(dteDateFrom, dteDateTo)) return;

            if (txtParticularID.TextLength > 0 && txtName.TextLength > 0)
            {
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                DateTime stDateFrom = dteDateFrom.Value;                
                string sDateFrom = stDateFrom.ToString("yyyy-MM-dd");

                DateTime stDateTo = dteDateTo.Value;
                string sDateTo = stDateTo.ToString("yyyy-MM-dd");

                clsSearch.ClassParticularID = int.Parse(dbFunction.CheckAndSetNumericValue(txtParticularID.Text));
                clsSearch.ClassMobileTerminalID = clsFunction.sZero;
                clsSearch.ClassDepartmentID = int.Parse(dbFunction.CheckAndSetNumericValue(txtDepartmentID.Text));
                clsSearch.ClassDateFrom = sDateFrom;
                clsSearch.ClassDateTo = sDateTo;
                clsSearch.ClassMissingTimeSheet = clsFunction.iZero;
                clsSearch.ClassDepartment = txtDepartment.Text;
                clsSearch.ClassReportType = clsFunction.sDefaultSelect;

                dbAPI.ProcessTimeSheet();

                dbReportFunc.ViewTimeSheet();

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

        private void frmTimeSheet_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void LoadTimeLogs()
        {
            int i = 0;
            int inLineNo = 0;

            Cursor.Current = Cursors.WaitCursor;

            DateTime stDateFrom = dteDate.Value;
            string sDateFrom = stDateFrom.ToString("yyyy-MM-dd");

            clsSearch.ClassParticularID = int.Parse(dbFunction.CheckAndSetNumericValue(txtParticularID.Text));
            clsSearch.ClassDate = sDateFrom;

            dbFunction.ClearListViewItems(lvwTimeLog);
            
            clsSearch.ClassSearchValue = clsSearch.ClassParticularID + clsFunction.sPipe + clsSearch.ClassDate;
            dbAPI.ExecuteAPI("GET", "View", "TimeSheet Log", clsSearch.ClassSearchValue, "TimeSheet", "", "ViewTimeSheet");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.TimeSheetID.Length > i)
                {
                    // Add To List  
                    inLineNo++;
                    ListViewItem item = new ListViewItem(inLineNo.ToString());
                    item.SubItems.Add(clsArray.ParticularID[i].ToString());
                    item.SubItems.Add(clsArray.TimeSheetID[i].ToString());
                    item.SubItems.Add(clsArray.TimeSheetDate[i]);
                    item.SubItems.Add(clsArray.TimeIn[i]);

                    string sDevice = GetDeviceName(clsArray.TerminalID[i]);

                    item.SubItems.Add(sDevice);
                    item.SubItems.Add(clsFunction.sPosted);
                    item.SubItems.Add(clsFunction.sDash);
                    item.SubItems.Add(clsFunction.sDash);
                    item.SubItems.Add(clsFunction.sDash);

                    lvwTimeLog.Items.Add(item);

                    i++;
                }

                btnDeleteTimeSheet.Enabled = true;
            }

            dbFunction.ListViewAlternateBackColor(lvwTimeLog);

            Cursor.Current = Cursors.Default;
        }

        private void btnSearchTimeSheet_Click(object sender, EventArgs e)
        {
            btnDeleteTimeSheet.Enabled = false;
            if (txtParticularID.TextLength > 0 && txtName.TextLength > 0)
            {
                LoadTimeLogs();
            }
            else
            {
                MessageBox.Show("Check the following field(s) listed below:\n\n" +
                                "*Name\n" +
                                "\n" +
                                "Field(s) with asterisk(*) must not be blank.", "Incomplete Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }                
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            fEdit = false;
            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            InitButton();
            btnSave.Enabled = true;
            btnAdd.Enabled = false;            
            btnSearchParticular.Enabled = true;

            txtRemarks.Enabled = true;
            txtRemarks.BackColor = Color.White;
            txtRemarks.ReadOnly = false;

            txtName.Focus();
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

        private void lvwTimeLog_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:

                    if (isPosted)
                    {
                        dbFunction.SetMessageBox("Unable to remove TimeSheet that already posted.", "TimeSheet posted", clsFunction.IconType.iError);
                    }
                    else
                    {
                        if (iSelectedIndex > 0)
                            lvwTimeLog.Items.RemoveAt(iSelectedIndex);
                    }
                    
                    break;
                case Keys.Enter:
                    break;
            }
        }

        private void lvwTimeLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwTimeLog.SelectedItems.Count > 0)
            {
                iSelectedIndex = 0;
                isPosted = false;
                string LineNo = lvwTimeLog.SelectedItems[0].Text;                
                txtLineNo.Text = LineNo;

                if (LineNo.Length > 0)
                {
                    iSelectedIndex = int.Parse(LineNo) - 1;
                    int pTimeSheetID = int.Parse(lvwTimeLog.SelectedItems[0].SubItems[2].Text);

                    if (pTimeSheetID > 0)
                        isPosted = true;
                    else
                        isPosted = false;
                }
            }
        }

        private void btnAddTimeSheet_Click(object sender, EventArgs e)
        {
            //if (!dbFunction.isValidEntry(clsFunction.CheckType.iRemarks, txtRemarks.Text)) return;

            //btnSearchTimeSheet_Click(sender, e);
            DateTime stDate = dteDate.Value;
            DateTime stTime = dteTime.Value;
            string pTimeSheetID = clsFunction.sZero;

            string pDate = stDate.ToString("ddd, MMM, dd, yyyy");
            string pTime = stTime.ToString("hh:mm tt");


            string pDateFormat = stDate.ToString("yyyy-MM-dd");            
            string pTimeFormat = stTime.ToString("HH:mm:ss");

            string pDevice = clsFunction.sEntry;
            string pStatus = clsFunction.sDash;
            
            if (dbFunction.isValidID(txtParticularID.Text))
                AddToList(txtParticularID.Text, pTimeSheetID, pDate.ToUpper(), pTime.ToUpper(), txtRemarks.Text, pDateFormat.ToUpper(), pTimeFormat.ToUpper(), pDevice, pStatus);
        }

        private void AddToList(string pParticularID, string pTimeSheetID, string pDate, string pTime, string pRemarks, string pDateFormat, string pTimeFormat, string pDevice, string pStatus)
        {
            Debug.WriteLine("--AddToList--");
            Debug.WriteLine("pParticularID=" + pParticularID);
            Debug.WriteLine("pTimeSheetID=" + pTimeSheetID);
            Debug.WriteLine("pDate=" + pDate);
            Debug.WriteLine("pTime=" + pTime);
            Debug.WriteLine("pRemarks=" + pRemarks);
            Debug.WriteLine("pDateFormat=" + pDateFormat);
            Debug.WriteLine("pTimeFormat=" + pTimeFormat);
            Debug.WriteLine("pDevice=" + pDevice);
            Debug.WriteLine("pStatus=" + pStatus);

            if (dbFunction.isValidID(pParticularID))
            {
                CheckItemList(1, pParticularID, pTimeSheetID, pDate, pTime, pRemarks, pDateFormat, pTimeFormat, pDevice, pStatus);
            }
            else
            {
                if (fEdit)
                {
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iEMPID, txtParticularID.Text)) return;                    
                }                
            }
        }

        private void CheckItemList(int pLineNo, string pParticularID, string pTimeSheetID, string pDate, string pTime, string pRemarks, string pDateFormat, string pTimeFormat, string pDevice, string pStatus)
        {
            bool fListed = false;
            int LineNo = 1;
            string sParticularID = clsFunction.sZero;
            string sTimeSheetID = clsFunction.sZero;
            string sDate = clsFunction.sDash;
            string sTime = clsFunction.sDash;
            string sRemarks = clsFunction.sDash;
            
            if (lvwTimeLog.Items.Count > 0)
            {
                foreach (ListViewItem i in lvwTimeLog.Items)
                {
                    if (i.SubItems[3].Text == pDate && i.SubItems[4].Text == pTime)
                    {
                        fListed = true;
                        break;
                    }
                }
            }

            // Check if already in the product list
            if (fListed)
            {
                // Update on the list
                foreach (ListViewItem i in lvwTimeLog.Items)
                {
                    if (i.SubItems[3].Text == pDate)
                    {
                        sParticularID = i.SubItems[1].Text;
                        sTimeSheetID = i.SubItems[2].Text;                        
                        sDate = i.SubItems[3].Text;
                        sTime = i.SubItems[4].Text;
                        sRemarks = i.SubItems[5].Text;

                        if (MessageBox.Show("TimeSheet detail you are trying to add is already on the list.\n" +
                            "------------------------------------------------------------- \n" +
                            "TimeSheet leave detail: \n" +
                            "Date: " + sDate + "\n" +
                            "Time: " + sTime + "\n" +
                            "Remarks: " + sRemarks + "\n" +
                            "------------------------------------------------------------- \n" +
                            "Overwrite with TimeSheet detail: \n" +
                            "Date: " + pDate + "\n" +
                            "Time: " + pTime + "\n" +
                            "Remarks: " + pRemarks + "\n" +
                            "\n\n" +
                            "Are you sure you want to overwrite the existing detail?",
                            "Item already listed.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            // Update existing....
                            i.SubItems[1].Text = pParticularID;
                            i.SubItems[2].Text = pTimeSheetID;
                            i.SubItems[3].Text = pDate;
                            i.SubItems[4].Text = pTime;
                            i.SubItems[5].Text = pDevice;
                            i.SubItems[6].Text = pStatus;
                            i.SubItems[7].Text = pRemarks;
                            i.SubItems[8].Text = pDateFormat;
                            i.SubItems[9].Text = pTimeFormat;
                        }

                        break;
                    }
                }
            }
            else
            {
                AddItem(LineNo, pParticularID, pTimeSheetID, pDate, pTime, pRemarks, pDateFormat, pTimeFormat, pDevice, pStatus);
            }

            // Update LineNo
            if (lvwTimeLog.Items.Count > 0)
            {
                foreach (ListViewItem i in lvwTimeLog.Items)
                {
                    if (lvwTimeLog.Items.Count > 0)
                    {
                        i.SubItems[0].Text = LineNo.ToString();
                        LineNo++;
                    }
                }
            }
        }

        private void AddItem(int pLineNo, string pParticularID, string pTimeSheetID, string pDate, string pTime, string pRemarks, string pDateFormat, string pTimeFormat, string pDevice, string pStatus)
        {

            ListViewItem item = new ListViewItem(pLineNo.ToString());

            item.SubItems.Add(pParticularID);
            item.SubItems.Add(pTimeSheetID);
            item.SubItems.Add(pDate);
            item.SubItems.Add(pTime);
            item.SubItems.Add(pDevice);
            item.SubItems.Add(pStatus);
            item.SubItems.Add(pRemarks);
            item.SubItems.Add(pDateFormat);
            item.SubItems.Add(pTimeFormat);

            lvwTimeLog.Items.Add(item);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateFields()) return;

            if (!fConfirmDetails()) return;

            Cursor.Current = Cursors.WaitCursor;

            SaveTimeSheet();

            if (fEdit)
                dbFunction.SetMessageBox("TimeSheet has been successfully modified.", "Updated", clsFunction.IconType.iExclamation);
            else
                dbFunction.SetMessageBox("TimeSheet has been successfully added.", "Saved", clsFunction.IconType.iInformation);

            if (MessageBox.Show("Do you want another entry for " + txtName.Text + "?", "Confirm entry", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                btnClear_Click(this, e);
            }
            else
            {
                //Initialize object
                dbFunction.ClearListViewItems(lvwTimeLog);                
                InitTime();
            }

            Cursor.Current = Cursors.Default;
        }

        private bool ValidateFields()
        {
            bool fValid = false;

            if (txtName.TextLength > 0 && dbFunction.isValidID(txtParticularID.Text))
                fValid = true;

            if (!fValid)
            {
                MessageBox.Show("Check the following field(s) listed below:\n\n" +
                                "*Name\n" +                                
                                "\n" +
                                "Field(s) with asterisk(*) must not be blank.", "Incomplete Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return fValid;
        }

        private bool fConfirmDetails()
        {
            bool fConfirm = true;
            string sTemp = "";

            sTemp = (fEdit ? "Are you sure to update the following details below:\n\n" : "Are you to save the following details below:\n\n") +
                           clsFunction.sLineSeparator + "\n" +
                           "> TimeSheet" + "\n" +
                           clsFunction.sLineSeparator + "\n" +
                           "Name: " + txtName.Text + "\n" +
                           "Department: " + txtDepartment.Text + "\n" +
                           "Position: " + txtPosition.Text + "\n" +
                           "Employment: " + txtEmploymentStatus.Text + "\n" +                           
                           clsFunction.sLineSeparator + "\n" +
                           "Remarks: " + txtRemarks.Text + "\n" +
                           clsFunction.sLineSeparator + "\n";

            if (MessageBox.Show(sTemp, (fEdit ? "Confirm Update" : "Confirm Saving"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fConfirm = false;
            }

            return fConfirm;
        }

        private void SaveTimeSheet()
        {
            string sRowSQL = "";
            string sSQL = "";
            
            Debug.WriteLine("--SaveTimeSheet--");
            Debug.WriteLine("fEdit=" + fEdit);

            dbFunction.GetProcessedByAndDateTime(); // Get processedby and datetime

            dbFunction.GetModifiedByAndDateTime(); // Get modifiedby and datetime

            foreach (ListViewItem i in lvwTimeLog.Items)
            {
                int pParticularID = int.Parse(i.SubItems[1].Text);
                int pTimeSheetID = int.Parse(i.SubItems[2].Text);
                string pActualDate = i.SubItems[3].Text;
                string pActualTime = i.SubItems[4].Text;
                string pDevice = i.SubItems[5].Text;
                string pStatus = i.SubItems[6].Text;
                string pRemarks = i.SubItems[7].Text;
                string pDate = i.SubItems[8].Text;
                string pTime = i.SubItems[9].Text;
                string pFullDate = pDate + " " + pTime;

                string pTerminalID = clsFunction.sNull;

                switch (pDevice)
                {
                    case clsFunction.sPC:
                        pTerminalID = clsFunction.sDevicePC;
                        break;
                    case clsFunction.sEntry:
                        pTerminalID = clsFunction.sDeviceEntry;
                        break;
                    default:
                        pTerminalID = clsFunction.sDeviceMobile;
                        break;
                }

                if (dbFunction.isValidID(pTimeSheetID.ToString()))
                {
                    Debug.WriteLine("pDate="+ pDate + ",pTime="+ pTime + "-->>pTimeSheetID=" + pTimeSheetID);
                }
                else
                {
                    // Insert
                    sRowSQL = "";
                    sSQL = "";
                    sRowSQL = " (" + dbFunction.CheckAndSetNumericValue(txtParticularID.Text) + ", " +
                    sRowSQL + sRowSQL + " '" + txtName.Text + "', " +
                    sRowSQL + sRowSQL + " '" + pDate + "', " +
                    sRowSQL + sRowSQL + " '" + pTime + "', " +
                    sRowSQL + sRowSQL + " '" + pTime + "', " +
                    sRowSQL + sRowSQL + "'" + pFullDate + "'," +
                    sRowSQL + sRowSQL + "'" + clsFunction.sNonePNG + "'," +
                    sRowSQL + sRowSQL + "'" + clsFunction.sNonePNG + "'," +
                    sRowSQL + sRowSQL + "'" + pFullDate + "', " +
                    sRowSQL + sRowSQL + "'" + clsGlobalVariables.strComputerName + "', " +
                    sRowSQL + sRowSQL + "'" + clsGlobalVariables.strLocalIP + "', " +
                    sRowSQL + sRowSQL + "'" + pTerminalID + "', " +
                    sRowSQL + sRowSQL + "'" + pRemarks + "', " +
                    sRowSQL + sRowSQL + "'" + clsUser.ClassProcessedBy + "'," +
                    sRowSQL + sRowSQL + "'" + clsUser.ClassProcessedDateTime + "'," +
                    sRowSQL + sRowSQL + "'" + clsUser.ClassModifiedBy + "'," +
                    sRowSQL + sRowSQL + "'" + clsUser.ClassModifiedDateTime + "'," +
                    sRowSQL + sRowSQL + "'" + clsFunction.sX + "'," +
                    sRowSQL + sRowSQL + "'" + clsFunction.sDash + "') ";
                    sSQL = sSQL + sRowSQL;

                    Debug.WriteLine("sSQL=" + sSQL);

                    dbAPI.ExecuteAPI("POST", "Insert", "", "", "Entry User Time Log", sSQL, "InsertMaintenanceMaster");
                }

            }
        }

        private void btnResetAM_Click(object sender, EventArgs e)
        {
            DateTime dateTime = DateTime.Parse("08:00:00");
            dteTime.Value = dateTime;
            dteTime.CustomFormat = "hh:mm tt";
            dteTime.Format = DateTimePickerFormat.Custom;
        }

        private void btnPM_Click(object sender, EventArgs e)
        {
            DateTime dateTime = DateTime.Parse("17:00:00");
            dteTime.Value = dateTime;
            dteTime.CustomFormat = "hh:mm tt";
            dteTime.Format = DateTimePickerFormat.Custom;
        }

        private void btnDeleteTimeSheet_Click(object sender, EventArgs e)
        {
            bool fConfirm = false;
            DateTime dteDateFrom;
            dteDateFrom = DateTime.Parse(dteDate.Value.ToString());

            if (MessageBox.Show("Are you sure you want to delete timesheet.\n\n" +
                "Name: " + txtName.Text + "\n" +
                "Date: " + dteDateFrom.ToString("ddd, MM-dd-yyyy") + "\n\n" +
                "Note: Warning! Record will permanently deleted.", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                fConfirm = true;
            }
            
            if (fConfirm)
            {
                clsSearch.ClassAdvanceSearchValue = txtParticularID.Text + clsFunction.sPipe + dteDateFrom.ToString("yyyy-MM-dd");
                dbAPI.ExecuteAPI("DELETE", "Delete", "TimeSheet", clsSearch.ClassAdvanceSearchValue, "TimeSheet", "", "DeleteCollectionDetail");

                dbFunction.SetMessageBox("Timesheet has been successfully deleted.", "TimeSheet", clsFunction.IconType.iInformation);

                dbFunction.ClearListViewItems(lvwTimeLog);
                InitTime();
            }
        }

        private string GetDeviceName(string pTerminalID)
        {
            string sTemp = clsFunction.sNull;

            switch (pTerminalID)
            {
                case clsFunction.sDevicePC:
                    sTemp = clsFunction.sPC;
                    break;
                case clsFunction.sDeviceEntry:
                    sTemp = clsFunction.sEntry;
                    break;
                default:
                    sTemp = clsFunction.sMobile;
                    break;
            }

            return sTemp;
        }

        private void dteDateTo_ValueChanged(object sender, EventArgs e)
        {            
            clsSearch.ClassDateTo = SetDatePickerValue(dteDateTo, "yyyy-MM-dd");
        }

        private void dteDateFrom_ValueChanged(object sender, EventArgs e)
        {            
            clsSearch.ClassDateFrom = SetDatePickerValue(dteDateFrom, "yyyy-MM-dd");
        }

        private void dteDate_ValueChanged(object sender, EventArgs e)
        {            
            clsSearch.ClassDate = SetDatePickerValue(dteDate, "yyyy-MM-dd");
        }
        
        private string SetDatePickerValue(DateTimePicker objDate, string pFormat)
        {
            string sTemp = clsFunction.sDateFormat;
            DateTime stDate = objDate.Value;
            string sDate = stDate.ToString(pFormat);

            sTemp = sDate;

            return sTemp;
        }

        private void SetTimePickerValue(DateTimePicker objDate, string pFormat)
        {            
            DateTime stDate = objDate.Value;
            string sDate = stDate.ToString(pFormat);

            clsSearch.ClassTime = sDate;            
        }

        private string GetTimePickerValue()
        {
            string sTemp = clsFunction.sNull;

            sTemp = clsSearch.ClassTime;

            return sTemp;
        }

        private void ChangeTimeLogButton(bool isEnable)
        {
            btnAddTimeSheet.Image = null;
            btnAddTimeSheet.Enabled = isEnable;

            if (isEnable)
            {
                btnAddTimeSheet.Image = Image.FromFile(dbFile.sImagePath + clsGlobalVariables.IMAGE_ALARM_ON);
            }
            else
            {
                btnAddTimeSheet.Image = Image.FromFile(dbFile.sImagePath + clsGlobalVariables.IMAGE_ALARM_OFF);
            }
        }

    }
}
