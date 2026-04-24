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
    public partial class frmChangePassword : Form
    {
        private clsAPI dbAPI;
        private clsINI dbSystem;
        private clsInternet dbInternet;
        private clsFunction dbFunction;
        private clsINI dbINI;
        private clsFile dbFile;

        public frmChangePassword()
        {
            InitializeComponent();
        }

        private void frmChangePassword_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbSystem = new clsINI();
            dbFile = new clsFile();

            dbSystem.InitAPISetting();
            dbSystem.InitFTPSetting();
       
            dbINI = new clsINI();
            dbFunction = new clsFunction();

            dbInternet = new clsInternet();

            txtOldPassword.UseSystemPasswordChar = txtNewPassword.UseSystemPasswordChar = txtConfirmPassword.UseSystemPasswordChar = true;


        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string EncryptPassword = "";

            // Check fields
            if (!ValidateFields()) return;

            // Check new/confirm password
            if (!txtNewPassword.Text.Equals(txtConfirmPassword.Text))
            {
                dbFunction.SetMessageBox("Confirm password does not match.", "Check field", clsFunction.IconType.iError);
                return;
            }

            if (clsSystemSetting.ClassSystemCheckNetLink > 0)
            {
                // Check internet connection            
                if (!dbInternet.CheckInternetConnection(clsSystemSetting.ClassSystemNetLink))
                {
                    MessageBox.Show("Please check internet connection.", "Internet unavailable.", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1);
                    return;
                }
            }

            // Check WebServer Connection
            if (!dbInternet.CheckServerSocket())
            {
                MessageBox.Show("Unable to connect to WebServer." +
                                "\n\n" +
                                "Server IP: " + clsGlobalVariables.strAPIServerIP + "\n" +
                                "Server Port: " + clsGlobalVariables.strAPIServerPort + "\n" +
                                "\n\n" +
                                "Please contact administrator.", "Connect failed.", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);

                return;
            }

            if (MessageBox.Show("Are you sure you want to change password.\n\n" +
               "UserName: " + txtUserName.Text + "\n\n" +              
               "Note:\nPlease contact administrator if old password is forgotten.", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                return;
            }

            EncryptPassword = dbFunction.EncryptString(txtOldPassword.Text, clsFunction.SystemPassword);
            dbAPI.ExecuteAPI("GET", "Search", "User Time Logger", txtUserName.Text, "User", "", "ViewUser");

            //if (!dbAPI.isValidComputerName(clsUser.ClassParticularID.ToString(), clsUser.ClassUserFullName, clsGlobalVariables.strComputerName)) return;

            // Check Old Password
            if (string.Compare(EncryptPassword, clsUser.ClassPassword) == 0)
            {
                EncryptPassword = dbFunction.EncryptString(txtNewPassword.Text, clsFunction.SystemPassword);

                // Check system password
                if (string.Compare(txtNewPassword.Text, clsFunction.MasterKey.ToUpper()) == 0)
                {
                    dbFunction.SetMessageBox("Please use different password.", "Check field", clsFunction.IconType.iWarning);
                    return;
                }

                // Check if same password from old
                if (string.Compare(EncryptPassword, clsUser.ClassPassword) == 0)
                {
                    dbFunction.SetMessageBox("New password still the same with old password.", "Check field", clsFunction.IconType.iWarning);
                    return;
                }

                Cursor.Current = Cursors.WaitCursor;

                // Update Password              
                clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(clsUser.ClassUserID.ToString()) + clsFunction.sPipe +
                                                    txtUserName.Text + clsFunction.sPipe +                                              
                                                    EncryptPassword;

                dbAPI.ExecuteAPI("PUT", "Update", "Update User Password", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                Cursor.Current = Cursors.Default;

                dbFunction.SetMessageBox(clsUser.ClassUserFullName + " your password has been successfullly changed.", "Change password", clsFunction.IconType.iInformation);

                btnClear_Click(this, e);

            }
            else
            {
                dbFunction.SetMessageBox("Invalid username and old password.", "Check field", clsFunction.IconType.iError);
                return;
            }


        }

        private bool ValidateFields()
        {   
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iUserName, txtUserName.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iOldPassword, txtOldPassword.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iNewPassword, txtNewPassword.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iConfirmPassword, txtConfirmPassword.Text)) return false;
            
            return true;

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dbFunction.ClearTextBox(this);
            txtOldPassword.UseSystemPasswordChar = txtNewPassword.UseSystemPasswordChar = txtConfirmPassword.UseSystemPasswordChar = true;

            txtUserName.Focus();
        }

        private void frmChangePassword_Activated(object sender, EventArgs e)
        {
            txtUserName.Focus();
        }

        private void txtUserName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtOldPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtNewPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtConfirmPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void chkShow_CheckedChanged(object sender, EventArgs e)
        {
            switch (chkShow.Checked)
            {
                case true:
                    txtOldPassword.UseSystemPasswordChar = txtNewPassword.UseSystemPasswordChar = txtConfirmPassword.UseSystemPasswordChar = false;
                    chkShow.Text = "Hide";
                    break;
                case false:
                    txtOldPassword.UseSystemPasswordChar = txtNewPassword.UseSystemPasswordChar = txtConfirmPassword.UseSystemPasswordChar = true;
                    chkShow.Text = "Show";
                    break;
            }
        }

        private void txtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (txtUserName.Text.Length > 0)
                        txtOldPassword.Focus();
                    break;
            }
        }

        private void txtNewPassword_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (txtUserName.Text.Length > 0)
                        txtConfirmPassword.Focus();
                    break;
            }
        }

        private void txtConfirmPassword_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (txtUserName.Text.Length > 0)
                        btnOK.Focus();
                    break;
            }
        }
    }
}
