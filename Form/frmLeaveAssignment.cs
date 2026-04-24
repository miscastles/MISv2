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
    public partial class frmLeaveAssignment : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        private clsReportFunc dbReportFunc;
        public static string sHeader;
        private int iSelectedIndex = 0;

        bool fEdit = false;

        public frmLeaveAssignment()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmLeaveAssignment_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            dbReportFunc = new clsReportFunc();

            fEdit = false;
            dbFunction.ClearTextBox(this);
            dbFunction.ClearListViewItems(lvwList);
            dbFunction.TextBoxUnLock(false, this);
            InitButton();

            dbAPI.FillComboBoxLeaveType(cboLeaveType);
            
            btnAddLeave.Enabled = false;
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

            if (txtName.TextLength > 0 && lvwList.Items.Count > 0 && dbFunction.isValidID(txtParticularID.Text))
                fValid = true;

            if (!fValid)
            {
                MessageBox.Show("Check the following field(s) listed below:\n\n" +
                                "*Name\n" +
                                "*Leave assignment list\n" +
                                "\n" +
                                "Field(s) with asterisk(*) must not be blank.", "Incomplete Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            iSelectedIndex = 0;

            txtName.BackColor = clsFunction.MKBackColor;
            txtName.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            fEdit = false;
            dbFunction.ClearTextBox(this);
            dbFunction.ClearListViewItems(lvwList);
            dbFunction.TextBoxUnLock(false, this);
            InitButton();

            btnSearchParticular.Enabled = true;
            btnAddLeave.Enabled = false;
            iSelectedIndex = 0;

            cboLeaveType.SelectedIndex = 0;
        }

        private void LoadLeaveAssignment(string pParticularID)
        {
            int i = 0;

            dbFunction.ClearListViewItems(lvwList);

            clsSearch.ClassSearchValue = pParticularID + clsFunction.sPipe + clsFunction.sZero;
            dbAPI.ExecuteAPI("GET", "Search", "Particular Leave Assignment", clsSearch.ClassSearchValue, "Particular Detail", "", "ViewParticularDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.LeaveTypeID.Length > i)
                {
                    clsParticular.ClassParticularID = int.Parse(clsArray.ParticularID[i]);
                    clsLeaveType.ClassLeaveTypeID = int.Parse(clsArray.LeaveTypeID[i]);
                    clsLeaveType.ClassCode = clsArray.LeaveTypeCode[i];
                    clsLeaveType.ClassDescription = clsArray.LeaveTypeDesc[i];

                    double dblCreditLimit = double.Parse(clsArray.LeaveTypeCreditLimit[i]);
                    string sCreditLimit = (dblCreditLimit > 0 ? dblCreditLimit.ToString("N") : "0.00");

                    double dblLeaveCredit = double.Parse(clsArray.LeaveCredit[i]);
                    string sLeaveCredit = (dblLeaveCredit > 0 ? dblLeaveCredit.ToString("N") : "0.00");

                    clsLeaveType.ClassCreditLimitString = sCreditLimit;
                    clsLeaveType.ClassLeaveCreditString = sLeaveCredit;

                    clsLeaveType.ClassRemarks = clsArray.Remarks[i];

                    i++;

                    AddItem(i);
                }
            }

            dbFunction.ListViewAlternateBackColor(lvwList);

        }
        private void AddItem(int inLineNo)
        {
            // Add to List            
            ListViewItem item = new ListViewItem(inLineNo.ToString());
            item.SubItems.Add(clsParticular.ClassParticularID.ToString());
            item.SubItems.Add(clsLeaveType.ClassLeaveTypeID.ToString());
            item.SubItems.Add(clsLeaveType.ClassCode.ToString());
            item.SubItems.Add(clsLeaveType.ClassDescription.ToString());
            item.SubItems.Add(clsLeaveType.ClassCreditLimitString);
            item.SubItems.Add(clsLeaveType.ClassLeaveCreditString);            
            item.SubItems.Add(clsLeaveType.ClassRemarks.ToString());
            lvwList.Items.Add(item);
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

                if (dbFunction.isValidID(txtParticularID.Text))
                {
                    fEdit = true;
                    LoadLeaveAssignment(txtParticularID.Text); 

                    dbFunction.TextBoxUnLock(true, this);
                    txtName.BackColor = clsFunction.MKBackColor;
                }

                InitButton();
                btnAddLeave.Enabled = true;

            }
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

                LeaveAssignment(txtParticularID.Text, txtLeaveTypeID.Text); // Load Leave Credit

                ComputeLeave();
            }
        }

        private void btnAddLeave_Click(object sender, EventArgs e)
        {            
            AddLeave(txtParticularID.Text, txtLeaveTypeID.Text, txtLeaveCode.Text, cboLeaveType.Text, txtCreditLimit.Text, txtLeaveCredit.Text, txtRemarks.Text);
        }

        private void AddLeave(string pParticularID, string pLeaveTypeID, string pCode, string pDescription, string pCreditLimit, string pLeaveCredit, string pRemarks)
        {
            Debug.WriteLine("--AddLeave--");
            Debug.WriteLine("fEdit=" + fEdit);
            Debug.WriteLine("pParticularID=" + pParticularID);
            Debug.WriteLine("pLeaveTypeID=" + pLeaveTypeID);
            Debug.WriteLine("pCode=" + pCode);
            Debug.WriteLine("pDescription=" + pDescription);
            Debug.WriteLine("pCreditLimit=" + pCreditLimit);
            Debug.WriteLine("pRemarks=" + pRemarks);

            if (dbFunction.isValidID(pParticularID) && dbFunction.isValidID(pLeaveTypeID))
            {
                CheckItemList(1, pParticularID, pLeaveTypeID, pCode, pDescription, pCreditLimit, pLeaveCredit, pRemarks);
            }
            else
            {
                if (fEdit)
                {
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iEMPID, txtParticularID.Text)) return;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iLeaveType, pLeaveTypeID)) return;
                }

                if (!dbFunction.isValidEntry(clsFunction.CheckType.iLeaveType, pLeaveTypeID)) return;
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iCreditLimit, pCreditLimit)) return;                
            }
        }

        private void CheckItemList(int pLineNo, string pParticularID, string pLeaveTypeID, string pCode, string pDescription, string pCreditLimit, string pLeaveCredit, string pRemarks)
        {
            bool fListed = false;
            int LineNo = 1;
            string sParticularID = clsFunction.sZero;
            string sLeaveTypeID = clsFunction.sZero;
            string sCode = clsFunction.sNull;
            string sDescription = clsFunction.sNull;
            string sCreditLimit = clsFunction.sZero;
            string sLeaveCredit = clsFunction.sZero;
            string sRemarks = clsFunction.sDash;

            if (lvwList.Items.Count > 0)
            {
                foreach (ListViewItem i in lvwList.Items)
                {
                    if (i.SubItems[2].Text == pLeaveTypeID)
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
                foreach (ListViewItem i in lvwList.Items)
                {
                    if (i.SubItems[2].Text == pLeaveTypeID)
                    {
                        sParticularID = i.SubItems[1].Text;
                        sLeaveTypeID = i.SubItems[2].Text;
                        sCode = i.SubItems[3].Text;
                        sDescription = i.SubItems[4].Text;
                        sCreditLimit = i.SubItems[5].Text;
                        sLeaveCredit = i.SubItems[6].Text;
                        sRemarks = i.SubItems[7].Text;

                        if (MessageBox.Show("Leave detail you are trying to add is already on the list.\n" +
                            "------------------------------------------------------------- \n" +
                            "Current leave detail: \n" +
                            "Leave Code: " + sCode + "\n" +
                            "Leave Type: " + sDescription + "\n" +
                            "Credit Limit: " + sCreditLimit + "\n" +
                            "Leave Credit: " + sLeaveCredit + "\n" +
                            "Remarks: " + sRemarks + "\n" +
                            "------------------------------------------------------------- \n" +
                            "Overwrite with leave detail: \n" +
                            "Leave Code: " + pCode + "\n" +
                            "Leave Type: " + pDescription + "\n" +
                            "Credit Limit: " + pCreditLimit + "\n" +
                            "Leave Credit: " + pLeaveCredit + "\n" +
                            "Remarks: " + pRemarks + "\n" +
                            "\n\n" +
                            "Are you sure you want to overwrite the existing detail?",
                            "Item already listed.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            // Update existing....
                            i.SubItems[1].Text = pParticularID;
                            i.SubItems[2].Text = pLeaveTypeID;
                            i.SubItems[3].Text = pCode;
                            i.SubItems[4].Text = pDescription;
                            i.SubItems[5].Text = pCreditLimit;
                            i.SubItems[6].Text = pLeaveCredit;
                            i.SubItems[7].Text = pRemarks;
                        }

                        break;
                    }
                }
            }
            else
            {                
                AddItem(LineNo, pParticularID, pLeaveTypeID, pCode, pDescription, pCreditLimit, pLeaveCredit, pRemarks);
            }

            // Update LineNo
            if (lvwList.Items.Count > 0)
            {
                foreach (ListViewItem i in lvwList.Items)
                {
                    if (lvwList.Items.Count > 0)
                    {
                        i.SubItems[0].Text = LineNo.ToString();
                        LineNo++;
                    }
                }
            }
        }

        private void AddItem(int pLineNo, string pParticularID, string pLeaveTypeID, string pCode, string pDescription, string pCreditLimit, string pLeaveCredit, string pRemarks)
        {

            ListViewItem item = new ListViewItem(pLineNo.ToString());

            item.SubItems.Add(pParticularID);
            item.SubItems.Add(pLeaveTypeID);
            item.SubItems.Add(pCode);
            item.SubItems.Add(pDescription);
            item.SubItems.Add(pCreditLimit);
            item.SubItems.Add(pLeaveCredit);
            item.SubItems.Add(pRemarks);

            lvwList.Items.Add(item);
        }

        private void lvwList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwList.SelectedItems.Count > 0)
            {
                iSelectedIndex = 0;
                string LineNo = lvwList.SelectedItems[0].Text;
                txtLineNo.Text = LineNo;

                if (LineNo.Length > 0)
                {
                    iSelectedIndex = int.Parse(LineNo) - 1;
                    txtLeaveTypeID.Text = lvwList.SelectedItems[0].SubItems[2].Text;
                    txtLeaveCode.Text = lvwList.SelectedItems[0].SubItems[3].Text;
                    cboLeaveType.Text = lvwList.SelectedItems[0].SubItems[4].Text;
                    txtCreditLimit.Text = lvwList.SelectedItems[0].SubItems[5].Text;
                    txtLeaveCredit.Text = lvwList.SelectedItems[0].SubItems[6].Text;
                    txtRemarks.Text = lvwList.SelectedItems[0].SubItems[7].Text;

                }
            }
        }

        private void lvwList_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:                    
                    lvwList.Items.RemoveAt(iSelectedIndex);

                    break;
                case Keys.Enter:
                    break;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateFields()) return;

            if (!fConfirmDetails()) return;

            Cursor.Current = Cursors.WaitCursor;

            if (fEdit)
            {
                // Delete
                clsSearch.ClassSearchValue = txtParticularID.Text + clsFunction.sPipe + clsFunction.sNull;
                dbAPI.ExecuteAPI("DELETE", "Delete", "Leave Assignment", clsSearch.ClassSearchValue, "Leave Assignment", "", "DeleteCollectionDetail");

                SaveLeaveAssignment();
            }
            else
            {
                SaveLeaveAssignment();
            }

            if (fEdit)
                dbFunction.SetMessageBox("Leave assignment has been successfully modified.", "Updated", clsFunction.IconType.iExclamation);
            else
                dbFunction.SetMessageBox("Leave assingment has been successfully added.", "Saved", clsFunction.IconType.iInformation);


            btnClear_Click(this, e);

            Cursor.Current = Cursors.Default;

        }

        private bool fConfirmDetails()
        {
            bool fConfirm = true;
            string sTemp = "";
            string sDescription = "";
            
            // Get Leave Type
            foreach (ListViewItem i in lvwList.Items)
            {   
                sDescription = sDescription + i.SubItems[4].Text + "\n";                
            }

            sTemp = (fEdit ? "Are you sure to update the following details below:\n\n" : "Are you to save the following details below:\n\n") +
                           clsFunction.sLineSeparator + "\n" +
                           "> Leave Assignment" + "\n" +
                           clsFunction.sLineSeparator + "\n" +
                           "Name: " + txtName.Text + "\n" +
                           "Department: " + txtDepartment.Text + "\n" +
                           "Position: " + txtPosition.Text + "\n" +
                           "Employment: " + txtEmploymentStatus.Text + "\n" +
                           clsFunction.sLineSeparator + "\n" +
                           "Leave Type Selected: " + "\n" +
                           sDescription + "\n" +
                           clsFunction.sLineSeparator;

            if (MessageBox.Show(sTemp, (fEdit ? "Confirm Update" : "Confirm Saving"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fConfirm = false;
            }

            return fConfirm;
        }

        private void SaveLeaveAssignment()
        {
            string sRowSQL = "";
            string sSQL = "";

            Debug.WriteLine("--SaveLeaveAssignment--");
            Debug.WriteLine("fEdit=" + fEdit);

            dbFunction.GetProcessedByAndDateTime(); // Get processedby and datetime

            dbFunction.GetModifiedByAndDateTime(); // Get modifiedby and datetime

            foreach (ListViewItem i in lvwList.Items)
            {
                int ParticularID = int.Parse(i.SubItems[1].Text);               
                int LeaveTypeID = int.Parse(i.SubItems[2].Text);
                string sCode = i.SubItems[3].Text;
                string sDescription = i.SubItems[4].Text;
                double dblCreditLimit = double.Parse(i.SubItems[5].Text);
                double dblLeaveLimit = double.Parse(i.SubItems[6].Text);
                string sRemarks = i.SubItems[7].Text;

                // Insert
                sRowSQL = "";
                sSQL = "";
                sRowSQL = " (" + dbFunction.CheckAndSetNumericValue(ParticularID.ToString()) + ", " +
                sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(LeaveTypeID.ToString()) + ", " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(sCode) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(sRemarks) + "', " +
                sRowSQL + sRowSQL + "'" + clsUser.ClassProcessedBy + "'," +
                sRowSQL + sRowSQL + "'" + clsUser.ClassProcessedDateTime + "'," +
                sRowSQL + sRowSQL + "'" + clsUser.ClassModifiedBy + "'," +
                sRowSQL + sRowSQL + "'" + clsUser.ClassModifiedDateTime + "'," +
                sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(dblCreditLimit.ToString()) + ", " +
                sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(dblLeaveLimit.ToString()) + ") ";
                sSQL = sSQL + sRowSQL;

                Debug.WriteLine("sSQL=" + sSQL);

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "Leave Assignment", sSQL, "InsertCollectionDetail");

            }
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

        private void panel13_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void frmLeaveAssignment_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (txtParticularID.TextLength > 0 && txtName.TextLength > 0)
            {
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                clsSearch.ClassReportID = 17;
                clsSearch.ClassParticularID = int.Parse(dbFunction.CheckAndSetNumericValue(txtParticularID.Text));
                clsSearch.ClassDepartmentID = clsFunction.iDefaultID;
                clsSearch.ClassDateFrom = clsFunction.sDateFormat;
                clsSearch.ClassDateTo = clsFunction.sDateFormat;

                dbReportFunc.ViewLeaveAssignment();

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
    }
}
