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
    public partial class frmSetting : Form
    {
        private clsINI dbINI;
        private clsAPI dbAPI;
        private clsFunction dbFunction;

        private int iTab;
        public static int iType;
        public frmSetting()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveSetting()
        {            
            switch (iTab)
            {
                case 0: // API                   
                    clsGlobalVariables.strAPIAuthUser = txtAPIUserName.Text;
                    clsGlobalVariables.strAPIAuthPassword = txtAPIPassword.Text;
                    clsGlobalVariables.strAPIURL = txtAPIURL.Text;
                    clsGlobalVariables.strAPIPath = txtAPIPath.Text;
                    clsGlobalVariables.strAPIKeys = txtAPIKey.Text;
                    clsGlobalVariables.strAPIContentType = txtAPIContentType.Text;
                    dbINI.WriteAPISetting();
                    break;
                case 1: // FTP
                    clsGlobalVariables.strFTPURL = txtFTPURL.Text;
                    clsGlobalVariables.strFTPPORT = txtFTPPORT.Text;
                    clsGlobalVariables.strFTPUploadPath = txtFTPUploadPath.Text;
                    clsGlobalVariables.strFTPDownloadPath = txtFTPDownloadPath.Text;
                    clsGlobalVariables.strFTPUserName = txtFTPUserName.Text;
                    clsGlobalVariables.strFTPPassword = txtFTPUPassword.Text;
                    clsGlobalVariables.strFTPLocalPath = txtFTPLocalPath.Text;
                    dbINI.WriteFTPSetting();
                    break;
                case 2: // Database
                    clsGlobalVariables.strSource = txtDatabaseSource.Text;
                    clsGlobalVariables.strServer = txtDatabaseServer.Text;
                    clsGlobalVariables.strDatabase = txtDatabaseName.Text;
                    clsGlobalVariables.strUserName = txtDatabaseUserName.Text;
                    clsGlobalVariables.strPassword = txtDatabasePassword.Text;
                    clsGlobalVariables.strSecurity = txtDatabaseSecurity.Text;
                    clsGlobalVariables.strPort = txtDatabasePort.Text;
                    dbINI.WriteDatabaseSetting();
                    break;
                case 3: // System
                    clsSystemSetting.ClassSystemSplashInterval = dbFunction.SetValueToZero(txtSystemSI.Text);
                    clsSystemSetting.ClassSystemSplashTimeOut = dbFunction.SetValueToZero(txtSystemST.Text);
                    clsSystemSetting.ClassSystemCheckNetLink = dbFunction.SetValueToZero(txtSystemCheckNetLink.Text);
                    clsSystemSetting.ClassSystemNetLink = txtSystemNetLink.Text;
                    clsSystemSetting.ClassSystemCheckIPAddress = dbFunction.SetValueToZero(txtSystemCheckIPAddress.Text);
                    clsSystemSetting.ClassSystemIPAddress = txtSystemIPAddress.Text;
                    clsSystemSetting.ClassSystemConnectionType = txtSystemConnType.Text;
                    clsSystemSetting.ClassSystemPromptAPIRequest = dbFunction.SetValueToZero(txtSystemAPIRequest.Text);
                    clsSystemSetting.ClassSystemPromptAPIResponse = dbFunction.SetValueToZero(txtSystemAPIResponse.Text);
                    clsSystemSetting.ClassSystemLogInCheck = dbFunction.SetValueToZero(txtSystemCheckLogIn.Text);
                    clsSystemSetting.ClassSystemImportCheck = dbFunction.SetValueToZero(txtSystemCheckImport.Text);
                    clsSystemSetting.ClassSystemMSOffice = txtSystemMSOffice.Text;
                    clsSystemSetting.ClassSystemSkinColor = cboSkinColor.Text;
                    dbINI.WriteSystemSetting();
                    break;
                case 4: // ODBC
                     clsODBC.ClassODBCServerName = txtODBCServerName.Text;
                     clsODBC.ClassODBCDSNName = txtODBCDNSName.Text;
                     clsODBC.ClassODBCDriverName = txtODBCDriverName.Text;
                     clsODBC.ClassODBCDatabaseName = txtODBCDatabaseName.Text;
                     clsODBC.ClassODBCDescription = txtODBCDescription.Text;
                     clsODBC.ClassODBCUser = txtODBCUserName.Text;
                     clsODBC.ClassODBCPassword = txtODBCPassword.Text;
                    dbINI.WriteODBCSetting();
                    break;
                default:
                    break;
            }

            MessageBox.Show(tabSetting.SelectedTab.Text + " successfully saved", "Saved",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1);
        }

        private void ReadSetting()
        {            
            switch (iTab)
            {
                case 0: // API  
                    dbINI.InitAPISetting();
                    txtAPIUserName.Text = clsGlobalVariables.strAPIAuthUser;
                    txtAPIPassword.Text = clsGlobalVariables.strAPIAuthPassword;
                    txtAPIURL.Text = clsGlobalVariables.strAPIURL;
                    txtAPIPath.Text = clsGlobalVariables.strAPIPath;
                    txtAPIKey.Text = clsGlobalVariables.strAPIKeys;
                    txtAPIContentType.Text = clsGlobalVariables.strAPIContentType;
                    break;
                case 1: // FTP
                    dbINI.InitFTPSetting();
                    txtFTPURL.Text = clsGlobalVariables.strFTPURL;
                    txtFTPPORT.Text = clsGlobalVariables.strFTPPORT;
                    txtFTPUploadPath.Text = clsGlobalVariables.strFTPUploadPath;
                    txtFTPDownloadPath.Text = clsGlobalVariables.strFTPDownloadPath;
                    txtFTPUserName.Text = clsGlobalVariables.strFTPUserName;
                    txtFTPUPassword.Text = clsGlobalVariables.strFTPPassword;
                    txtFTPLocalPath.Text = clsGlobalVariables.strFTPLocalPath;
                    break;
                case 2: // Database
                    dbINI.InitDatabaseSetting();
                    txtDatabaseSource.Text = clsGlobalVariables.strSource;
                    txtDatabaseServer.Text = clsGlobalVariables.strServer;
                    txtDatabaseName.Text = clsGlobalVariables.strDatabase;
                    txtDatabaseUserName.Text = clsGlobalVariables.strUserName;
                    txtDatabasePassword.Text = clsGlobalVariables.strPassword;
                    txtDatabaseSecurity.Text = clsGlobalVariables.strSecurity;
                    txtDatabasePort.Text = clsGlobalVariables.strPort;
                    break;
                case 3: // System
                    dbINI.InitSystemSetting();
                    txtSystemSI.Text = clsSystemSetting.ClassSystemSplashInterval.ToString();
                    txtSystemST.Text = clsSystemSetting.ClassSystemSplashTimeOut.ToString();
                    txtSystemCheckNetLink.Text = clsSystemSetting.ClassSystemCheckNetLink.ToString();
                    txtSystemNetLink.Text = clsSystemSetting.ClassSystemNetLink;
                    txtSystemCheckIPAddress.Text = clsSystemSetting.ClassSystemCheckIPAddress.ToString();
                    txtSystemIPAddress.Text = clsSystemSetting.ClassSystemIPAddress;
                    txtSystemConnType.Text = clsSystemSetting.ClassSystemConnectionType;
                    txtSystemAPIRequest.Text = clsSystemSetting.ClassSystemPromptAPIRequest.ToString();
                    txtSystemAPIResponse.Text = clsSystemSetting.ClassSystemPromptAPIResponse.ToString();
                    txtSystemCheckLogIn.Text = clsSystemSetting.ClassSystemLogInCheck.ToString();
                    txtSystemCheckImport.Text = clsSystemSetting.ClassSystemImportCheck.ToString();
                    txtSystemMSOffice.Text = clsSystemSetting.ClassSystemMSOffice;

                    string sSkinColor = clsSystemSetting.ClassSystemSkinColor;
                    dbFunction.FillComboBoxSkinColor(cboSkinColor);
                    cboSkinColor.Text = sSkinColor;
                    InitSkinColor(sSkinColor);
                    break;
                case 4: // ODBC
                    dbINI.InitODBCSetting();
                    txtODBCServerName.Text = clsODBC.ClassODBCServerName;
                    txtODBCDNSName.Text = clsODBC.ClassODBCDSNName;
                    txtODBCDriverName.Text = clsODBC.ClassODBCDriverName;
                    txtODBCDatabaseName.Text = clsODBC.ClassODBCDatabaseName;
                    txtODBCDescription.Text = clsODBC.ClassODBCDescription;
                    txtODBCUserName.Text = clsODBC.ClassODBCUser;
                    txtODBCPassword.Text = clsODBC.ClassODBCPassword;
                    break;
                case 5: // Version
                    dbINI.InitVersionSetting();
                    dbAPI.GetSystemInfo();
                    txtVersionCheckUpdate.Text = clsGlobalVariables.strCheckUpdate;                    
                    txtPublishVersionLocal.Text = clsGlobalVariables.strPublishVersion;
                    txtPublishVersionServer.Text = clsSystemSetting.ClassSystemPublishVersion;

                    lblVersionStatus.Text = "";
                    if (txtPublishVersionLocal.Text.CompareTo(txtPublishVersionServer.Text) == 0)
                    {
                        lblVersionStatus.Text = "Application version is updated.";
                    }
                    else
                    {
                        lblVersionStatus.Text = "Application version is outdated. Update version now.";
                    }

                    break;
                default:
                    break;
            }

        }

        private void frmSetting_Load(object sender, EventArgs e)
        {
            dbINI = new clsINI();
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            dbINI.InitAPISetting();
            dbINI.InitFTPSetting();

            // Write Version
            dbFunction.GetPublishVersion(lblVersion);
            clsGlobalVariables.strPublishVersion = clsSystemSetting.ClassSystemLocalPublishVersion;
            dbINI.WriteVersionSetting();

            InitType();
            dbFunction.ClearTextBox(this);
            tabSetting.SelectedIndex = 0;
            ReadSetting();            
        }
        

        private void InitType()
        {
            btnContinue.Enabled = false;

            switch (iType)
            {
                case 0:
                    btnContinue.Enabled = true;
                    break;
                case 1:
                    btnContinue.Enabled = false;
                    break;
            }
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {            
            //frmLogin frm = new frmLogin();
            //this.Hide();
            //frm.ShowDialog();
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!fContinueConfirm()) return;

            SaveSetting();
        }
        bool fContinueConfirm()
        {
            bool fContinue = true;
            string sMessage = tabSetting.SelectedTab.Text;
            
            if (MessageBox.Show("Are you sure to save " + sMessage + 
                                    "\n\n",
                                    "Continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fContinue = false;
            }

            return fContinue;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (!fResetConfirm()) return;

            dbFunction.ClearTextBox(this);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ReadSetting();
        }

        private void tabSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            iTab = tabSetting.SelectedIndex;
            ReadSetting();
        }

        private void frmSetting_Activated(object sender, EventArgs e)
        {
            btnContinue.Focus();
        }

        private void frmSetting_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void txtFTPPORT_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtDatabasePort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSystemSI_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSystemCheckNetLink_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSystemCheckLogIn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSystemMSOffice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSystemST_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSystemAPIRequest_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSystemAPIResponse_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSystemCheckImport_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        bool fResetConfirm()
        {
            bool fContinue = true;

            if (MessageBox.Show("This will clear field(s)." +
                                    "\n\n",
                                    "Continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fContinue = false;
            }

            return fContinue;
        }

        private void InitSkinColor(string sSkinColor)
        {
            Color c = Color.FromName(sSkinColor);

            if (c.IsKnownColor && sSkinColor.Length > 0)
            {
                pnlSkinColor.BackColor = Color.FromName(sSkinColor);                
            }
            else
            {
                dbFunction.SetMessageBox("Color: " + "[" + sSkinColor + "]" + " is unknown color.", "Invalid color", clsFunction.IconType.iExclamation);
                pnlSkinColor.BackColor = Color.Maroon;                
            }
        }

        private void cboSkinColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSkinColor.Text.CompareTo(clsFunction.sDefaultSelect) != 0)
                InitSkinColor(cboSkinColor.Text);
        }
    }
}
