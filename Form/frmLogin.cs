using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using MIS.Model;

namespace MIS
{
    public partial class frmLogin : Form
    {
        private clsAPI dbAPI;
        private clsINI dbSystem;        
        private clsInternet dbInternet;
        private clsFunction dbFunction;        
        private clsINI dbINI;
        private clsFile dbFile;

        public bool IsSwitchBankMode { get; set; } = false;

        protected override CreateParams CreateParams
        {
            // Override CreateParams to enable double-buffering for child controls
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED
                //cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        public frmLogin()
        {
            InitializeComponent();
            //AdminRelauncher();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("Form Load");

            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbSystem = new clsINI();
            dbFile = new clsFile();

            dbSystem.InitAPISetting();
            dbSystem.InitFTPSetting();
            dbSystem.InitBasicSystem();
            
            dbINI = new clsINI();
            dbFunction = new clsFunction();
            
            dbInternet = new clsInternet();

            InitTextBox();
                        
            //dbSystem.InitSystemSetting();
            dbSystem.InitVersionSetting();
            dbFunction.GetPublishVersion(lblPublishVersion);
            dbFunction.GetEnvironment(lblEnvironment);
            
            GetComputerLogInDetails();            

            lblAPIDetail.Text = dbFunction.GetURLDetail();
            lblComputerDetail.Text = dbFunction.GetComputerDetail();
            //CreateDSN();

            InitLogo();

            // Create app required folder
            dbFile.CreateAppRequiredFolder();

            dbFunction.updateSytemDateFormat();
            
            fillBankSelection(); // load bank list
            
            // Reload switch bank from frmMain
            if (AppSession.BankCode != null)
            {
                cboBank.Text = AppSession.BankName;
                btnOK.Focus();
            }
            
            Cursor.Current = Cursors.Default;
        }

        private void frmLogin_Activated(object sender, EventArgs e)
        {
            cboBank.Focus();
        }
        private void InitTextBox()
        {
            txtUserName.UseSystemPasswordChar = false;
            txtPassword.UseSystemPasswordChar = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Application.Exit();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string EncryptPassword = "";

            Cursor.Current = Cursors.WaitCursor;

            // check valid bank
            if (!dbFunction.isValidDescriptionEntry(cboBank.Text, "Bank" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

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

            // Check FTPServer Connection
            if (!dbInternet.CheckFTPServerSocket())
            {
                MessageBox.Show("Unable to connect to FTPServer." +
                                "\n\n" +
                                "Server IP: " + clsGlobalVariables.strFTPURL + "\n" +
                                "Server Port: " + clsGlobalVariables.strFTPPORT + "\n" +
                                "\n\n" +
                                "Please contact administrator.", "Connect failed.", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);

                return;
            }

            if (string.IsNullOrEmpty(txtUserName.Text))
            {
                MessageBox.Show("Username must not be blank", "Important Note",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                return;
            }

            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Password must not be blank", "Important Note",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                return;
            }

            // ---------------------------------------------------------------
            // set bank
            // ---------------------------------------------------------------
            getBankInfo();

            dbSystem.InitSystemSetting();

            EncryptPassword = dbFunction.EncryptString(txtPassword.Text, clsFunction.SystemPassword);                                   
            dbAPI.ExecuteAPI("GET", "Search", "UserName", txtUserName.Text, "User", "", "ViewUser");
            
            // User Found
            if (clsUser.ClassUserID > 0)
            {
                if (string.Compare(EncryptPassword, clsUser.ClassPassword) == 0)
                {
                    if (clsSystemSetting.ClassSystemLogInCheck > 0)
                    {
                        if (dbAPI.isAlreadyLogIn(clsUser.ClassUserID.ToString()))
                        {
                            MessageBox.Show("User " + clsUser.ClassUserFullName + " is logged on at another workstation." + "\n\n" +
                                            "Please contact administrator.", "Important Note",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1);
                            return;
                        }
                    }

                    Cursor.Current = Cursors.WaitCursor;

                    clsSearch.ClassCurrentUserID = clsUser.ClassUserID;
                    clsSearch.ClassCurrentParticularID = clsUser.ClassParticularID;
                    clsSearch.ClassCurrentParticularName = clsUser.ClassUserFullName;
                    clsSearch.ClassisAppVersion = clsUser.ClassisAppVersion;

                    // set Apps Session
                    AppSession.BankCode = clsSearch.ClassBankCode;
                    AppSession.BankName = clsSearch.ClassBankDisplayName;
                    AppSession.Username = txtUserName.Text;
                    AppSession.Password = txtPassword.Text;

                    string message = "Welcome to " + clsSystemSetting.ClassApplicationName
                                                     + "\n\n" + clsUser.ClassUserFullName.Trim()
                                                     + "\n\nSelected Bank: " + cboBank.Text;

                    dbFunction.SetMessageBox(message, "Login", clsFunction.IconType.iInformation);


                    // Check version
                    if (!dbAPI.isValidSystemVersion()) return;

                    initIRRemotePath(); // create ir remote path

                    initSerialRemotePath(); // create serial remote path

                    initErmRemotePath(); // create erm remote path

                    if (clsSystemSetting.ClassGenerateResponseFile > 0)
                    {
                        //dbFunction.SetMessageBox("Common files will be generated." + "\n\n" + clsDefines.TAKE_FEW_MINUTE_MSG, "Information", clsFunction.IconType.iInformation);
                        
                        dbAPI.GenerateResponseFile(ucStatus);
                    }
                    
                    dbAPI.SaveUserLog(clsAPI.UserActionType.iLogIn, lblPublishVersion.Text);

                    dbAPI.GetHeader(); // Load Report Header

                    Cursor.Current = Cursors.Default;

                    clsSearch.ClassStatus = clsGlobalVariables.LOGIN_STATUS;
                    clsSearch.ClassStatusDescription = clsGlobalVariables.LOGIN_STATUS_DESC;

                    if (IsSwitchBankMode)
                    {
                        // Just return to caller — let caller show new frmMain
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        // Normal login behavior
                        frmMain f2 = new frmMain();
                        this.Hide();
                        f2.ShowDialog();                        
                    }

                }
                else
                {
                    MessageBox.Show("Invalid password.", "Important Note",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                    return;
                }
            }
            else
            {
                MessageBox.Show("User does not exist.", "Important Note",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1);
                return;
            }

            Cursor.Current = Cursors.Default;
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
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
                    if (txtUserName.Text.Length > 0)
                        txtPassword.Focus();
                    break;
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtPassword.Text.Length > 0)
                    btnOK_Click(this, e);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
        private void GetComputerLogInDetails()
        {

            // Get Local IP
            clsGlobalVariables.strLocalIP = dbInternet.GetLocalIPAddress();

            // Get Public IP

            // Get Computer Name
            clsGlobalVariables.strComputerName = dbInternet.GetComputerName();

        }
        
        private void AdminRelauncher()
        {            
            if (!IsRunAsAdmin())
            {
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = Assembly.GetEntryAssembly().CodeBase;

                proc.Verb = "runas";

                try
                {
                    Process.Start(proc);
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("This program must be run as an administrator! \n\n" + ex.ToString());
                }
            }
        }

        private bool IsRunAsAdmin()
        {
            try
            {
                WindowsIdentity id = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(id);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void lblPublishVersion_Click(object sender, EventArgs e)
        {

        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            frmSetting frm = new frmSetting();
            frm.ShowDialog();
           
        }

        private void lblSetting_Click(object sender, EventArgs e)
        {
            frmSetting frm = new frmSetting();
            frm.ShowDialog();
        }

        private void txtUserName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }
        
        private void InitLogo()
        {
            Debug.WriteLine("--InitLogo--");

            imgLogo.Image = null;
            imgLogo.Enabled = false;
            imgLogo.Image = Image.FromFile(dbFile.sImagePath + clsGlobalVariables.IMAGE_LOGO);
        }

        private void lblChangePassword_Click(object sender, EventArgs e)
        {
            // check valid bank
            if (!dbFunction.isValidDescriptionEntry(cboBank.Text, "Bank" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            getBankInfo();

            frmChangePassword frm = new frmChangePassword();
            frm.ShowDialog();
        }
        
        private void cboBank_SelectedIndexChanged(object sender, EventArgs e)
        {
            imgBankLogo.Visible = false;

            txtUserName.Focus();

            if (dbFunction.isValidDescription(cboBank.Text))
            {
                getBankInfo();
                setBankThemes();

                imgBankLogo.Visible = true;
                dbFunction.InitBankLogo(imgBankLogo);

                lblAPIDetail.Text = dbFunction.GetURLDetail();
            }
                
        }

        private void getBankInfo()
        {
            clsBank selectedBank = (clsBank)cboBank.SelectedItem;
            if (selectedBank != null)
            {
                clsSearch.ClassBankID = selectedBank.ID;
                clsSearch.ClassBankCode = clsGlobalVariables.strAPIBank = selectedBank.Code;
                clsSearch.ClassIsBillType = selectedBank.IsBillType;
                clsSearch.ClassBankName = selectedBank.Bank;
                clsSearch.ClassBankDisplayName = selectedBank.DisplayName;
                clsSearch.ClassBankMainColor = selectedBank.mainColor;
                clsSearch.ClassBankPrimaryColor = selectedBank.primaryColor;
                clsSearch.ClassBankSecondaryColor = selectedBank.secondaryColor;

            }
        }

        private void setBankThemes()
        {
            pnlLeft.BackColor = ColorTranslator.FromHtml(clsSearch.ClassBankPrimaryColor);
            this.BackColor = ColorTranslator.FromHtml(clsSearch.ClassBankSecondaryColor);
            
        }

        private void fillBankSelection()
        {
            Debug.WriteLine("--fillBankSelection--");

            dbFile = new clsFile();
            dbFunction = new clsFunction();

            string filepath = dbFile.sSettingPath + clsDefines.RESP_BANKLIST_FILENAME;
            if (!dbFile.FileExist(filepath))
            {
                dbFunction.SetMessageBox("Bank list file does not exist.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return;
            }

            var banks = dbFunction.loadBankList(filepath);

            // debug
            foreach (var b in banks)
            {
                Debug.WriteLine($"Enabled: [{b.Enable}] | Code: [{b.Code}] | Display: [{b.DisplayName}]");
            }

            // filter only enabled banks
            var enabledBanks = banks.Where(b => b.Enable == 1).ToList();

            cboBank.DataSource = enabledBanks;
            cboBank.DisplayMember = "DisplayName"; // case-sensitive fix (optional)
            cboBank.ValueMember = "Code";

            imgBankLogo.BackgroundImage = null;
        }

        public void autoFillAppSession(string pBankCode, string pBankName, string pUserName, string pPassword)
        {
            Debug.WriteLine("--autoFillAppSession--");
            Debug.WriteLine($"Code: {pBankCode}, Display: {pBankName}, pUserName: {pUserName}, pPassword: {pPassword}");

            fillBankSelection(); // load bank list

            cboBank.SelectedValue = pBankName;
            
            txtUserName.Text = pUserName;
            txtPassword.Text = pPassword;

            btnOK.Focus();
        }

        // ir remote folder
        private void initIRRemotePath()
        {
            ucStatus.iState = 3;
            ucStatus.sMessage = "Creating ir remote path";
            ucStatus.AnimateStatus();

            List<clsBank> banks;
            string pRemotePath = clsGlobalVariables.strFTPRemoteIRPath;
            ftp ftpClient = null;

            // Create FTP client
            ftpClient = new ftp(
                clsGlobalVariables.strFTPURL,
                clsGlobalVariables.strFTPUserName,
                clsGlobalVariables.strFTPPassword
            );

            // Load bank list
            banks = loadBankList();

            if (banks != null)
            {
                foreach (var item in banks)
                {
                    // Build remote path                    
                    string pPath = $"{pRemotePath.TrimEnd('/')}/{item.Code}".TrimStart('/');

                    Console.WriteLine($"[{item.Code}] Enable: {item.Enable}");
                    Console.WriteLine($"[{item.Code}] Remote directory: {pPath}");

                    if (item.Enable > 0)
                    {
                        try
                        {
                            // Create remote directory
                            ftpClient.createDirectory(pPath);
                            Console.WriteLine($"[{item.Code}] Remote directory created: {pPath}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[{item.Code}] ERROR creating remote directory: {ex.Message}");
                        }
                    }

                }
            }
        }

        // serial remote folder
        private void initSerialRemotePath()
        {
            ucStatus.iState = 3;
            ucStatus.sMessage = "Creating serial remote path";
            ucStatus.AnimateStatus();

            List<clsBank> banks;
            string pRemotePath = clsGlobalVariables.strFTPRemoteSerialPath;
            ftp ftpClient = null;

            // Create FTP client
            ftpClient = new ftp(
                clsGlobalVariables.strFTPURL,
                clsGlobalVariables.strFTPUserName,
                clsGlobalVariables.strFTPPassword
            );

            // Load bank list
            banks = loadBankList();

            if (banks != null)
            {
                foreach (var item in banks)
                {
                    // Build remote path                   
                    string pPath = $"{pRemotePath.TrimEnd('/')}/{item.Code}".TrimStart('/');

                    Console.WriteLine($"[{item.Code}] Enable: {item.Enable}");
                    Console.WriteLine($"[{item.Code}] Remote directory: {pPath}");

                    if (item.Enable > 0)
                    {
                        try
                        {
                            // Create remote directory
                            ftpClient.createDirectory(pPath);
                            Console.WriteLine($"[{item.Code}] Remote directory created: {pPath}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[{item.Code}] ERROR creating remote directory: {ex.Message}");
                        }
                    }

                }
            }
        }

        // erm remote folder
        private void initErmRemotePath()
        {
            ucStatus.iState = 3;
            ucStatus.sMessage = "Creating erm remote path";
            ucStatus.AnimateStatus();

            List<clsBank> banks;
            string pRemotePath = clsGlobalVariables.strFTPRemoteErmPath;
            ftp ftpClient = null;

            // Create FTP client
            ftpClient = new ftp(
                clsGlobalVariables.strFTPURL,
                clsGlobalVariables.strFTPUserName,
                clsGlobalVariables.strFTPPassword
            );

            // Load bank list
            banks = loadBankList();

            if (banks != null)
            {
                foreach (var item in banks)
                {
                    // Build remote path                   
                    string pPath = $"{pRemotePath.TrimEnd('/')}/{item.Code}".TrimStart('/');

                    Console.WriteLine($"[{item.Code}] Enable: {item.Enable}");
                    Console.WriteLine($"[{item.Code}] Remote directory: {pPath}");

                    if (item.Enable > 0)
                    {
                        try
                        {
                            // Create remote directory
                            ftpClient.createDirectory(pPath);
                            Console.WriteLine($"[{item.Code}] Remote directory created: {pPath}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[{item.Code}] ERROR creating remote directory: {ex.Message}");
                        }
                    }

                }
            }
        }

        public List<clsBank> loadBankList()
        {
            string filepath = dbFile.sSettingPath + clsDefines.RESP_BANKLIST_FILENAME;
            string json = File.ReadAllText(filepath);
            BankResponse response = JsonConvert.DeserializeObject<BankResponse>(json);

            return response.Data;

        }

    }
}
