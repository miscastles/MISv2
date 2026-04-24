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
using MIS.Controller;

namespace MIS
{
    public partial class frmUser : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        private TerminalController _mTerminalController = new TerminalController();
        private UserController _mUserController = new UserController();

        bool fEdit = false;

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

        public frmUser()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwUser, true);
            dbFunction.setDoubleBuffer(lvwPrivacy, true);

        }
        private void UserType()
        {
            cboType.Items.Clear();
            cboType.Items.Add(clsFunction.sDefaultSelect);            
            cboType.Items.Add("ADMIN");
            //cboType.Items.Add("EDITOR");
            cboType.Items.Add("USER");

            cboType.SelectedIndex = 0;

        }

        private void ClearListView()
        {
            lvwUser.Items.Clear();
            lvwPrivacy.Items.Clear();
        }
        private void ClearTextBox()
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is TextBox)
                        (control as TextBox).Clear();
                    else
                        func(control.Controls);
            };

            func(Controls);
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
            if (!dbFunction.isValidDescriptionEntry(txtUserName.Text, "UserName" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtPassword.Text, "Password" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtReTypePassword.Text, "Re-type password" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtMD5Password.Text, "MD5 generated" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtHashPassword.Text, "Hash generated" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtFullName.Text, "Name" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(cboType.Text, "User Type" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            
            return true;
        }
        
        private void frmUser_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);

            dbFunction.ClearComboBox(this);
            dbFunction.ComBoBoxUnLock(false, this);

            dbFunction.ClearListViewItems(lvwUser);
            dbFunction.ClearListViewItems(lvwPrivacy);

            UserType();            
            InitButton();            
            LoadUser();

            dbAPI.FillComboBoxMobileTerminal(cboMobile);
            
            chkIsView.Checked = chkIsAdd.Checked = chkIsUpdate.Checked = chkIsDelete.Checked = chkIsPrint.Checked = false;

            txtPassword.UseSystemPasswordChar = txtReTypePassword.UseSystemPasswordChar = true;

            cboType.Text = cboMobile.Text = clsFunction.sDefaultSelect;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            fEdit = false;

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(true, this);
            dbFunction.ComBoBoxUnLock(true, this);

            InitButton();
            btnAdd.Enabled = false;
            btnSave.Enabled = true;            
            
            InitPrivacyDetail();
            txtUserName.BackColor = clsFunction.MKBackColor;

            cboType.Text = cboMobile.Text = clsFunction.sDefaultSelect;

            txtUserName.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            fEdit = false;

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);

            dbFunction.ClearListViewItems(lvwUser);
            dbFunction.ClearListViewItems(lvwPrivacy);
            
            InitButton();
            UserType();

            LoadUser();
            
            chkIsView.Checked = chkIsAdd.Checked = chkIsUpdate.Checked = chkIsDelete.Checked = chkIsPrint.Checked = false;

            txtPassword.UseSystemPasswordChar = txtReTypePassword.UseSystemPasswordChar = true;

            cboType.Text = cboMobile.Text = clsFunction.sDefaultSelect;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateFields()) return;
            
            // Check UserName
            if (!fEdit)
            {
                if (CheckUserName()) return;
            }

            // Check Password
            if (string.Compare(txtPassword.Text, txtReTypePassword.Text) != 0)
            {
                MessageBox.Show("Password entry mismatch. Please check.", "Password Failed.",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1);

                return;
            }
            
            // Check if
            if (!isCheckPrivacyDetail())
            {
                if (MessageBox.Show("User  " + txtFullName.Text + " has no selected rights and privacy.", "Continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
            }

            if (!dbFunction.fPromptConfirmation("Are you sure to " + (fEdit ? "update" : "saved") + " user?")) return;

            // get
           
            Cursor.Current = Cursors.WaitCursor;

            // generate MD5 for password
            btnGenerateMD5_Click(this, e);
            btnGenerateHash_Click(this, e);

            SaveUser();

            SavePrivacyDetail();

            if (!fEdit)
            {
                MessageBox.Show("New User successfully saved", "Saved",
               MessageBoxButtons.OK,
               MessageBoxIcon.Information,
               MessageBoxDefaultButton.Button1);
            }
            else
            {
                MessageBox.Show("User has been successfully modified", "Edited",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }

            Cursor.Current = Cursors.Default;

            btnClear_Click(this, e);

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadUser()
        {
            int i = 0;

            ClearListView();            
            dbAPI.ExecuteAPI("GET", "View", "", "", "User", "", "ViewUser");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            while (clsArray.UserID.Length > i)
            {
                
                clsUser.ClassUserID = int.Parse(clsArray.UserID[i].ToString());
                clsUser.ClassUserName = clsArray.UserName[i];
                clsUser.ClassPassword = clsArray.Password[i];
                clsUser.ClassUserFullName = clsArray.FullName[i];
                clsUser.ClassUserType = clsArray.UserType[i];
                clsUser.ClassParticularID = int.Parse(clsArray.ParticularID[i]);
                clsUser.ClassMobileTerminalID = clsArray.MobileTerminalID[i];

                i++;

                AddItem(i);
            }

            dbFunction.ListViewAlternateBackColor(lvwUser);
        }
        private void AddItem(int inLineNo)
        {
            // Add to List            
            ListViewItem item = new ListViewItem(inLineNo.ToString());
            item.SubItems.Add(clsUser.ClassUserID.ToString());
            item.SubItems.Add(clsUser.ClassUserName.ToString());
            item.SubItems.Add(clsUser.ClassPassword.ToString());
            item.SubItems.Add(clsUser.ClassUserFullName.ToString());
            item.SubItems.Add(clsUser.ClassUserType.ToString());
            item.SubItems.Add(clsUser.ClassParticularID.ToString());
            item.SubItems.Add(clsUser.ClassMobileTerminalID.ToString());

            lvwUser.Items.Add(item);
        }

        private void lvwUser_DoubleClick(object sender, EventArgs e)
        {
            if (lvwUser.SelectedItems[0].SubItems[1].Text.Length > 0)
            {

                string ID = lvwUser.SelectedItems[0].SubItems[1].Text;
                string UserName = lvwUser.SelectedItems[0].SubItems[2].Text;
                string Password = lvwUser.SelectedItems[0].SubItems[3].Text;
                string FullName = lvwUser.SelectedItems[0].SubItems[4].Text;
                string UserType = lvwUser.SelectedItems[0].SubItems[5].Text;
                string ParticularID = lvwUser.SelectedItems[0].SubItems[6].Text;
                
                dbFunction.ClearTextBox(this);
                dbFunction.TextBoxUnLock(true, this);
                dbFunction.ComBoBoxUnLock(true, this);

                txtID.Text = ID;
                txtUserName.Text = UserName;

                string sDecryptPassword = dbFunction.DecryptString(Password, clsFunction.SystemPassword);
                txtPassword.Text = sDecryptPassword;
                txtReTypePassword.Text = sDecryptPassword;
                txtFullName.Text = FullName;
                cboType.Text = UserType;
                txtParticularID.Text = ParticularID;
                
                getUserInfo(int.Parse(dbFunction.CheckAndSetNumericValue(txtID.Text)));
                txtHashPassword.Text = dbFunction.EncryptString(txtPassword.Text, clsFunction.SystemPassword);

                if (txtID.Text.Length > 0 && txtUserName.Text.Length > 0)
                    fEdit = true;

                InitButton();

                TextBoxUnLock(true);
                ComBoBoxUnLock(true);

                txtUserName.BackColor = clsFunction.MKBackColor;
                txtFullName.BackColor = clsFunction.MKBackColor;

                // Load User Rights and Privacy
                dbAPI.FillListViewPrivacy(lvwPrivacy, "Privacy Detail", txtID.Text + clsFunction.sPipe + clsFunction.sZero, false);
            }
        }

        private void TextBoxUnLock(bool isLock)
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is TextBox)
                    {
                        (control as TextBox).Enabled = isLock;

                        if (isLock)
                            (control as TextBox).BackColor = Color.White;
                        else
                            (control as TextBox).BackColor = Color.LightGray;
                    }
                    else
                        func(control.Controls);
            };

            func(Controls);
        }

        private void ComBoBoxUnLock(bool isLock)
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is ComboBox)
                    {
                        (control as ComboBox).Enabled = isLock;
                    }
                    else
                    {
                        func(control.Controls);
                    }
            };

            func(Controls);
        }
        private bool CheckUserName()
        {
            bool fExist = false;

            fExist = dbAPI.CheckUser("Search", "Check UserName", txtUserName.Text);

            if (fExist)
            {
                dbFunction.SetMessageBox("Unable to save user." +
                            "\n\n" +
                            "Name: " + txtUserName.Text +
                            "\n", "Already exist.", clsFunction.IconType.iWarning);


            }

            return fExist;
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

                txtUserName.BackColor = clsFunction.MKBackColor;
                txtFullName.BackColor = clsFunction.MKBackColor;

                txtFullName.Text = clsSearch.ClassParticularName;

                //LoadUser();
            }
        }
        
        private void bunifuCards3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lvwPrivacy_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkIsAdd.Checked = chkIsAdd.Checked = chkIsUpdate.Checked = chkIsDelete.Checked = chkIsPrint.Checked = false;
            if (lvwPrivacy.SelectedItems.Count > 0)
            {
                string LineNo = lvwPrivacy.SelectedItems[0].Text;
                txtPrivacyLineNo.Text = LineNo;

                gbSelected.Text = "Selected: ";
                chkSelectedAll.Checked = false;
                chkSelectedAll.Text = "CHECK ALL";

                if (LineNo.Length > 0)
                {
                    txtPrivacyID.Text = lvwPrivacy.SelectedItems[0].SubItems[1].Text;

                    if (dbFunction.isMultipleListViewChecked(lvwPrivacy))
                        gbSelected.Text += "[ Multiple ]";
                    else
                        gbSelected.Text += "[ " + lvwPrivacy.SelectedItems[0].SubItems[2].Text + " ]";
                    
                    // Set checkbox
                    chkIsView.Checked = dbFunction.setYesNoToBoolean(lvwPrivacy.SelectedItems[0].SubItems[4].Text);
                    chkIsAdd.Checked = dbFunction.setYesNoToBoolean(lvwPrivacy.SelectedItems[0].SubItems[5].Text);
                    chkIsUpdate.Checked = dbFunction.setYesNoToBoolean(lvwPrivacy.SelectedItems[0].SubItems[6].Text);
                    chkIsDelete.Checked = dbFunction.setYesNoToBoolean(lvwPrivacy.SelectedItems[0].SubItems[7].Text);
                    chkIsPrint.Checked = dbFunction.setYesNoToBoolean(lvwPrivacy.SelectedItems[0].SubItems[8].Text);
                    
                }
            }
            
        }


        private void SaveUser()
        {         
            string EncryptPassword = "";
            string sRowSQL = "";
            string sSQL = "";
            
            EncryptPassword = dbFunction.EncryptString(txtPassword.Text, clsFunction.SystemPassword);
            Debug.WriteLine("EncryptPassword=" + EncryptPassword);

            if (!fEdit)
            {
                sRowSQL = "";
                sRowSQL = " ('" + txtUserName.Text + "', " +
                sRowSQL + sRowSQL + " '" + EncryptPassword + "', " +
                sRowSQL + sRowSQL + " '" + txtMD5Password.Text + "', " +
                sRowSQL + sRowSQL + " '" + txtFullName.Text + "', " +
                sRowSQL + sRowSQL + " '" + cboType.Text + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtParticularID.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtMobileID.Text) + "', " +
                sRowSQL + sRowSQL + " '" + clsGlobalVariables.LOGOUT_STATUS + "', " +
                sRowSQL + sRowSQL + " '" + clsGlobalVariables.LOGOUT_STATUS_DESC + "') ";
                sSQL = sSQL + sRowSQL;

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "User", sSQL, "InsertMaintenanceMaster");
               
            }
            else
            {
                if (dbFunction.isValidID(txtID.Text))
                {
                    clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtID.Text) + clsFunction.sPipe +
                                                    txtUserName.Text + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtParticularID.Text) + clsFunction.sPipe +
                                                    txtFullName.Text + clsFunction.sPipe +
                                                    EncryptPassword + clsFunction.sPipe +
                                                    cboType.Text + clsFunction.sPipe +
                                                    txtMD5Password.Text + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtMobileID.Text);

                    dbAPI.ExecuteAPI("PUT", "Update", "Update User", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                }
            }
        }

        private void SavePrivacyDetail()
        {
            string sRowSQL = "";
            string sSQL = "";
   
            if (lvwPrivacy.Items.Count > 0)
            {
                // Delete privacy detail per userid
                clsSearch.ClassAdvanceSearchValue = txtID.Text;
                dbAPI.ExecuteAPI("DELETE", "Delete", "Privacy Detail", clsSearch.ClassAdvanceSearchValue, "Privacy Detail", "", "DeleteCollectionDetail");

                foreach (ListViewItem i in lvwPrivacy.Items)
                {
                    int PrivacyID = int.Parse(i.SubItems[1].Text);
                    string sDescription = i.SubItems[2].Text;
                    string isView = i.SubItems[4].Text;
                    string isAdd = i.SubItems[5].Text;
                    string isUpdate = i.SubItems[6].Text;
                    string isDelete = i.SubItems[7].Text;
                    string isPrint = i.SubItems[8].Text;

                    // Insert
                    sRowSQL = "";
                    sRowSQL = " (" + dbFunction.CheckAndSetNumericValue(txtID.Text) + "," +
                    sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(txtFullName.Text) + "'," +
                    sRowSQL + sRowSQL + "" + dbFunction.CheckAndSetNumericValue(PrivacyID.ToString()) + "," +
                    sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(sDescription) + "'," +
                    sRowSQL + sRowSQL + "" + dbFunction.CheckAndSetYesNoValue(isView.ToString()) + "," +
                    sRowSQL + sRowSQL + "" + dbFunction.CheckAndSetYesNoValue(isAdd.ToString()) + "," +
                    sRowSQL + sRowSQL + "" + dbFunction.CheckAndSetYesNoValue(isUpdate.ToString()) + "," +
                    sRowSQL + sRowSQL + "" + dbFunction.CheckAndSetYesNoValue(isDelete.ToString()) + "," +
                    sRowSQL + sRowSQL + "" + dbFunction.CheckAndSetYesNoValue(isPrint.ToString()) + ")";

                    if (sSQL.Length > 0)
                        sSQL = sSQL + ", " + sRowSQL;
                    else
                        sSQL = sSQL + sRowSQL;

                }

                Debug.WriteLine("sSQL=" + sSQL);

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "Privacy Detail", sSQL, "InsertCollectionDetail");
            }
        }

        private bool isCheckPrivacyDetail()
        {
            bool fCheck = false;

            foreach (ListViewItem i in lvwPrivacy.Items)
            {
                if (i.Checked)
                {
                    fCheck = true;
                    break;
                }
            }

            return fCheck;
        }

        private bool fConfirmDetails()
        {
            bool fConfirm = true;
            string sTemp = "";

            sTemp = (fEdit ? "Are you sure to update the following details below:\n\n" : "Are you to save the following details below:\n\n") +
                           clsFunction.sLineSeparator + "\n" +
                           "> User" + "\n" +
                           clsFunction.sLineSeparator + "\n" +
                           "UserName: " + txtUserName.Text + "\n" +
                           "Name: " + txtFullName.Text + "\n" +
                           "User Type: " + cboType.Text + "\n" +                         
                           clsFunction.sLineSeparator + "\n";

            if (MessageBox.Show(sTemp, (fEdit ? "Confirm Update" : "Confirm Saving"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fConfirm = false;
            }

            return fConfirm;
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAll.Text.Equals("CHECK ALL"))
            {
                chkAll.Text = "UNCHECK ALL";

                // Check All
                foreach (ListViewItem i in lvwPrivacy.Items)
                {
                    i.Checked = true;
                }
            }
            else
            {
                chkAll.Text = "CHECK ALL";

                // UnCheck All
                foreach (ListViewItem i in lvwPrivacy.Items)
                {
                    i.Checked = false;
                }
            }
        }

        private void InitPrivacyDetail()
        {
            foreach (ListViewItem i in lvwPrivacy.Items)
            {
                i.Checked = false;
            }
        }

        private void ApplyPrivacy(int pRowIndex, int pPrivacyID, bool isSpecific)
        {
            foreach (ListViewItem item in lvwPrivacy.Items)
            {
                // ================================
                // APPLY CHECKED (btnApplyChecked)
                // ================================
                if (isSpecific)
                {
                    // Affect CHECKED rows only
                    if (!item.Checked)
                        continue;

                    item.SubItems[4].Text = "N"; // VIEW
                    item.SubItems[5].Text = "N"; // ADD
                    item.SubItems[6].Text = "N"; // UPDATE
                    item.SubItems[7].Text = "N"; // DELETE
                    item.SubItems[8].Text = "N"; // PRINT

                    // Optional UX: uncheck after clearing
                    item.Checked = false;

                    continue; // ⛔ never fall through
                }

                // =================================
                // APPLY SPECIFIC (btnApplySpecific)
                // =================================
                int privacyID = int.Parse(item.SubItems[1].Text);
                if (privacyID != pPrivacyID)
                    continue;

                bool hasAnyPermission =
                    chkIsView.Checked ||
                    chkIsAdd.Checked ||
                    chkIsUpdate.Checked ||
                    chkIsDelete.Checked ||
                    chkIsPrint.Checked;

                // Row checkbox reflects permission existence
                item.Checked = hasAnyPermission;

                item.SubItems[4].Text = dbFunction.setBooleanToYesNo(chkIsView.Checked);
                item.SubItems[5].Text = dbFunction.setBooleanToYesNo(chkIsAdd.Checked);
                item.SubItems[6].Text = dbFunction.setBooleanToYesNo(chkIsUpdate.Checked);
                item.SubItems[7].Text = dbFunction.setBooleanToYesNo(chkIsDelete.Checked);
                item.SubItems[8].Text = dbFunction.setBooleanToYesNo(chkIsPrint.Checked);

                break; // only ONE selected row
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (!dbFunction.isValidListViewChecked(lvwPrivacy)) return;

            if (dbFunction.isMultipleListViewChecked(lvwPrivacy))
            {
                if (!dbFunction.fPromptConfirmation("Multiple privacy detected. Are you sure to continue?"))
                {
                    Cursor.Current = Cursors.Default;
                    return;
                }

                // Apply to all checked items
                foreach (ListViewItem item in lvwPrivacy.Items)
                {
                    if (item.Checked)
                    {
                        ApplyPrivacyToItem(item);
                        item.Checked = false; // optionally uncheck after applying
                    }
                }
            }
            else
            {
                // Apply to specific privacy item
                if (dbFunction.isValidID(txtPrivacyLineNo.Text) && dbFunction.isValidID(txtPrivacyID.Text))
                {
                    int targetID = int.Parse(txtPrivacyID.Text);

                    foreach (ListViewItem item in lvwPrivacy.Items)
                    {
                        if (int.TryParse(item.SubItems[1].Text, out int PrivacyID) && PrivacyID == targetID)
                        {
                            ApplyPrivacyToItem(item);
                            break; // only one match
                        }
                    }
                }
            }

            Cursor.Current = Cursors.Default;
        }

        private void ApplyPrivacyToItem(ListViewItem item)
        {
            item.SubItems[4].Text = dbFunction.setBooleanToYesNo(chkIsView.Checked);
            item.SubItems[5].Text = dbFunction.setBooleanToYesNo(chkIsAdd.Checked);
            item.SubItems[6].Text = dbFunction.setBooleanToYesNo(chkIsUpdate.Checked);
            item.SubItems[7].Text = dbFunction.setBooleanToYesNo(chkIsDelete.Checked);
            item.SubItems[8].Text = dbFunction.setBooleanToYesNo(chkIsPrint.Checked);
        }


        private void chkSelectedAll_CheckedChanged(object sender, EventArgs e)
        {
            // Determine if we should proceed
            bool canToggle = dbFunction.isMultipleListViewChecked(lvwPrivacy) ||
                             (dbFunction.isValidID(txtPrivacyLineNo.Text) && dbFunction.isValidID(txtPrivacyID.Text));

            if (!canToggle) return; // Nothing to do

            // Determine the new state
            bool checkAll = chkSelectedAll.Text.Equals("CHECK ALL");

            // Set all checkboxes
            chkIsView.Checked = chkIsAdd.Checked = chkIsUpdate.Checked = chkIsDelete.Checked = chkIsPrint.Checked = checkAll;

            // Toggle button text
            chkSelectedAll.Text = checkAll ? "UNCHECK ALL" : "CHECK ALL";
        }

        private void chkShow_CheckedChanged(object sender, EventArgs e)
        {
            switch (chkShow.Checked)
            {
                case true:
                    txtPassword.UseSystemPasswordChar = txtReTypePassword.UseSystemPasswordChar = false;
                    chkShow.Text = "Hide";
                    break;
                case false:
                    txtPassword.UseSystemPasswordChar = txtReTypePassword.UseSystemPasswordChar = true;
                    chkShow.Text = "Show";
                    break;
            }
        }

        private void frmUser_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:                   
                    this.Close();
                    break;
            }
        }

        private void btnGenerateMD5_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iPassword, txtPassword.Text)) return;

            txtMD5Password.Text = dbFunction.EncryptMD5(txtPassword.Text);
        }

        private void getTerminalInfo(string pDescription)
        {   
            TerminalController data = _mTerminalController.getInfo(pDescription);

            if (data != null)
            {
                txtMobileID.Text = data.MobileID.ToString();
                txtMobileName.Text = data.MobileTerminalName;
            }
        }

        private void cboMobile_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtMobileID.Text = clsFunction.sZero;
            txtMobileName.Text = clsFunction.sNull;
            if (!cboMobile.Text.Equals(clsFunction.sDefaultSelect))
            {
                getTerminalInfo(cboMobile.Text);
            }
        }

        private void getUserInfo(int pID)
        {
            UserController data = _mUserController.getInfo(pID);

            if (data != null)
            {
                txtMobileID.Text = data.MobileID.ToString();
                cboMobile.Text = data.MobileTerminalID.Equals(clsFunction.sDash) ? clsFunction.sDefaultSelect : data.MobileTerminalID;
                txtMobileName.Text = data.MobileTerminalName;
                txtMD5Password.Text = data.MD5Password;
                txtHashPassword.Text = data.Password;
                cboType.Text = data.UserType;
            }
        }

        private void btnGenerateHash_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iPassword, txtMD5Password.Text)) return;

            txtHashPassword.Text = dbFunction.EncryptString(txtPassword.Text, clsFunction.SystemPassword);
        }

        private void btnResetPermissions_Click(object sender, EventArgs e)
        {
            if (!dbFunction.fPromptConfirmation("Are you sure to reset permissions?")) return;

            Cursor.Current = Cursors.WaitCursor;

            int[] permissionColumns = { 4, 5, 6, 7, 8 }; // columns for View, Add, Update, Delete, Print
            foreach (ListViewItem item in lvwPrivacy.Items)
            {
                foreach (int col in permissionColumns)
                {
                    item.SubItems[col].Text = "N";
                }

                item.Checked = false;
            }

            Cursor.Current = Cursors.Default;

        }
        
    }
}
