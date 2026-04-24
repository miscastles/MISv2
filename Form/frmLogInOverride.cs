using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIS
{
    public partial class frmLogInOverride : Form
    {
        public static bool fSelected = false;
        private clsAPI dbAPI;
        private clsFunction dbFunction;

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

        public frmLogInOverride()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            fSelected = false;
            this.Close();
        }

        private void frmLogInOverride_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    fSelected = false;
                    this.Close();
                    break;                
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            fSelected = false;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            fSelected = false;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iUserName, txtUserName.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iPassword, txtPassword.Text)) return;

            string pEncryptPassword = dbFunction.EncryptString(txtPassword.Text, clsFunction.SystemPassword);
            string pJSONString = dbAPI.getInfoDetailJSON("Search", "User LogIn Info", txtUserName.Text + clsDefines.gPipe + pEncryptPassword);
            dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

            if (!dbFunction.isValidDescription(pJSONString))
            {
                dbFunction.SetMessageBox("Invalid username/password.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return;
            }

            string pName = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Name);
            string pUserType = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_UserType);

            if (!clsDefines.USERTYPE_ADMIN.Equals(pUserType))
            {
                dbFunction.SetMessageBox("For adminnistrator account login only.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);
                return;
            }

            fSelected = true;
            this.Close();
        }

        private void frmLogInOverride_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            
            Cursor.Current = Cursors.Default;
        }

        private void frmLogInOverride_Activated(object sender, EventArgs e)
        {
            txtUserName.Focus();
        }

        private void chkShow_CheckedChanged(object sender, EventArgs e)
        {
            switch (chkShow.Checked)
            {
                case true:
                    txtPassword.UseSystemPasswordChar = false;
                    chkShow.Text = "Hide";
                    break;
                case false:
                    txtPassword.UseSystemPasswordChar = true;
                    chkShow.Text = "Show";
                    break;
            }
        }

        private void txtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    txtPassword.Focus();
                    break;
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    btnOK.Focus();
                    break;
            }
        }
    }
}
