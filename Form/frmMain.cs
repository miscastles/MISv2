using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Security.Principal;
using System.Diagnostics;
using System.Timers;
using Timer = System.Timers.Timer;
using MIS.Controller;
using MIS.Model;
using MIS.Enums;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace MIS
{
    public partial class frmMain : Form
    {
        private clsFunction dbFunction;
        private clsAPI dbAPI;
        private clsINI dbINI;
        private clsInternet dbInternet;
        private clsReportFunc dbReportFunc;
        private clsFile dbFile;
        private int iMenu = 0;

        // Count
        private int iSecondPulse = 20;
        private int iCountPulse = 0;
        private int iDecrement = 0;
        private int iCount = 0;

        // Connection
        private int iSecondConn = 10;
        private int iCountConn = 0;
        private int iDecrementConn = 0;

        private bool fWebServerConn = false;
        private bool fFtpServerConn = false;
        
        private int iPaddedLength = 5;
        private string sPaddedCount = "00000";
        string sInput = "";

        private static Timer _timer;

        // Controller        
        private ServicingDetailController _mServicingDetailController;

        private List<ServicingDetailController> mList = null;
        
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

        public frmMain()
        {
            InitializeComponent();

            dbFunction = new clsFunction();

            // Subscribe to the Move event
            if (clsSystemSetting.ClassSystemExntededMonitor > 0)
                Move += new EventHandler(dbFunction.onFormMove);

            _mServicingDetailController = new ServicingDetailController();

        }

        private void btnLogoff_Click(object sender, EventArgs e)
        {
            bool fLogOut = true;

            iMenu = 13;            
            InitMenu(iMenu, true);

            if (MessageBox.Show(clsSearch.ClassCurrentParticularName + "\n\n" +
                                " Are you sure you want to logout " + clsSystemSetting.ClassApplicationName + "." +                                 
                                "\n\n",
                                "Confirm?", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fLogOut = false;
            }
            
            if (fLogOut)
            {                
                clsSearch.ClassStatus = clsGlobalVariables.LOGOUT_STATUS;
                clsSearch.ClassStatusDescription = clsGlobalVariables.LOGOUT_STATUS_DESC;

                dbAPI.UpdateStatus("Update", "UserID", clsSearch.ClassCurrentUserID.ToString(), "User Log");

                dbAPI.SaveUserLog(clsAPI.UserActionType.iLogOut, lblPublishVersion.Text);

                //closeAllOpenForms(); // close open form before logout

                MessageBox.Show("You're successfully logout " + clsSearch.ClassCurrentParticularName.Trim() + " !", "User LogOut.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);


                // restart application
                new Thread(() =>
                {
                    Thread.Sleep(500);
                    Application.Restart();
                }).Start();

                Application.Exit(); 
                
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (!fCloseFormConfirm()) return;
        }
        private bool fCloseFormConfirm()
        {
            bool fClose = true;
            
            if (MessageBox.Show(clsSearch.ClassCurrentParticularName + "\n\n" +
                                "Are you sure you want to exit " + clsSystemSetting.ClassApplicationName + "." +
                                    "\n\n",
                                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fClose = false;
            }

            if (fClose)
            {
                clsSearch.ClassStatus = clsGlobalVariables.LOGOUT_STATUS;
                clsSearch.ClassStatusDescription = clsGlobalVariables.LOGOUT_STATUS_DESC;

                dbAPI.UpdateStatus("Update", "UserID", clsSearch.ClassCurrentUserID.ToString(), "User Log");

                dbAPI.SaveUserLog(clsAPI.UserActionType.iLogOut, lblPublishVersion.Text);
                
                Application.Exit();
            }

            return fClose;
        }

        private void btnExitApplication_Click(object sender, EventArgs e)
        {
            iMenu = 14;
            InitMenu(iMenu, true);

            if (!fCloseFormConfirm()) return;
        }

        private void tmrTime_Tick(object sender, EventArgs e)
        {
            GetCurrentDateTime();
        }
        private void InitCurrentTimer()
        {
            lblDate.Text = "";
            tmrTime.Enabled = true;
            tmrTime.Interval = 1000;
        }
        private void GetCurrentDateTime()
        {
            DateTime now = DateTime.Now;

            // Format hour correctly (12-hour format)
            int strHour = now.Hour % 12 == 0 ? 12 : now.Hour % 12;
            string period = now.Hour >= 12 ? "PM" : "AM";

            // Format the time string
            string strTime = $"{strHour:00}:{now.Minute:00}:{now.Second:00} {period}";
            string dateDisplay = $"{now:ddd}, {now:MMMM d, yyyy}";

            // Update UI labels safely
            UpdateLabelSafe(lblDate, dateDisplay);
            UpdateLabelSafe(lblTime, strTime);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbFunction = new clsFunction();
            dbAPI = new clsAPI();
            dbINI = new clsINI();
            dbInternet = new clsInternet();
            dbReportFunc = new clsReportFunc();
            dbFile = new clsFile();

            dbINI.InitBasicSystem();

            // init
            lblBank.Text = "";
            imgBankLogo.BackgroundImage = null;

            // Skin Color
            InitSkinColor();
            
            InitCurrentTimer();            

            dbFunction.GetPublishVersion(lblPublishVersion);
            dbFunction.GetEnvironment(lblEnvironment);
            lblAPIDetail.Text = dbFunction.GetURLDetail();
            lblComputerDetail.Text = dbFunction.GetComputerDetail();

            lblServer.Text = $"WebServer Ip/Port: {clsGlobalVariables.strAPIURL} | FTP Ip/Port: {clsGlobalVariables.strFTPURL}:{clsGlobalVariables.strFTPPORT}";
            dbFunction.applyOuterColorOverlay(lblServer, Color.Cyan, 1);
            
            lblWhoIsOnline.Text = "";
            lblWhoIsOnlineFSR.Text = "";
            lblInstallationReqDaysPending.Text = "";
            lblInstallationReqDaysPending.Text = " -:- " + clsSystemSetting.ClassSystemNoOfDayPending.ToString() + clsFunction.sPadSpace + "DAY(S) PENDING";            

            InitMenu(0, false);
            InitCount();
            CheckPulse();

            // Server Status            
            ServerConnStatus();

            InitConn(true);
            InitConnTimer();

            iSecondPulse = clsSystemSetting.ClassSystemPulseInterval;
            iSecondConn = clsSystemSetting.ClassSystemCheckServerInterval;
            iCountPulse = iSecondPulse;
            iCountConn = iSecondConn;

            InitPulse(true);
            InitPulseTimer();

            InitPanelMenuList();            

            GetCurrentDateTime();
            lblUser.Text = "USER:" + clsUser.ClassUserFullName + Environment.NewLine +
                           "LEVEL:" + clsUser.ClassUserType + Environment.NewLine +
                           "DATE:" + lblDate.Text + Environment.NewLine +
                           "TIME:" + lblTime.Text;

            // Default to maximize window
            this.WindowState = FormWindowState.Normal;
            btnMaximize_Click(this, e);

            EnableApplication(true);

            loadWhosOnline();

            loadWhosOnlineFSR();

            loadWhosOnlineDashboard();
            
            loadUnclosedTicketList();

            lblFailedService.ForeColor = lblPendingFSR.ForeColor = lblTInstallation.ForeColor = lblTReprogramming.ForeColor = lblTServicing.ForeColor = lblTReplacement.ForeColor = lblTPullout.ForeColor = Color.Gray;
            
            dbFunction.InitLogo(imgLogo);

            // bank name
            lblBank.Text = clsSearch.ClassBankDisplayName;          
            dbFunction.applyOuterColorOverlay(lblBank, Color.FromArgb(0, 153, 255), 4);

            dbFunction.InitBankLogo(imgBankLogo);

            initShortCutKeyboard();

            // init subAppsName
            lblSubAppsName.Text = $"[ {clsSearch.ClassBankDisplayName} | {clsSystemSetting.ClassSystemEnvironment} ]";
            
            Cursor.Current = Cursors.Default;
        }

        private void btnMaintenance_Click(object sender, EventArgs e)
        {
            
        }

        private void btnTerminal_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            iMenu = 2;
            InitMenu(iMenu, true);
        }

        private void btnEnrollment_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            iMenu = 7;
            InitMenu(iMenu, true);
        }

        private void btnInstallation_Click(object sender, EventArgs e)
        {
            iMenu = 3;            
            InitMenu(iMenu, true);
        }

        private void btnServicing_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            iMenu = 4;
            InitMenu(iMenu, true);
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        
        private void btnReports_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            iMenu = 6;            
            InitMenu(iMenu, true);
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            iMenu = 0;
            InitMenu(iMenu, false);

            //dbFunction.SetMessageBox("Ongoing development...", "Oooops", clsFunction.IconType.iInformation);
            return;
            
            //iMenu = 12;
            //InitMenu(iMenu, true);
            //frmFindField frm = new frmFindField();
            //frm.ShowDialog();   

            //frmLoading frm = new frmLoading();
            //frm.ShowDialog();
        }

        private void btnTools_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            InitMenu(0, false);

            /*
            frmTools frm = new frmTools();
            frm.Text = "DEVELOPMENT TOOLS";
            frm.Show();
            */

            frmToolSelections frm = new frmToolSelections();
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            iMenu = 10;
            InitMenu(0, false);            
            frmSetting.iType = 1;
            frmSetting frm = new frmSetting();
            frm.Text = "SETTING";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnLogs_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            iMenu = 0;
            InitMenu(iMenu, false);

            //dbFunction.SetMessageBox("Ongoing development...", "Oooops", clsFunction.IconType.iInformation);
            return;
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {           
            Form frm = frmMain.ActiveForm;
            MaximizeMain();
        }

        private void MaximizeMain()
        {
            switch (this.WindowState)
            {
                case FormWindowState.Maximized:                    
                    this.WindowState = FormWindowState.Normal;
                    break;
                case FormWindowState.Normal:

                    if (clsSystemSetting.ClassSystemShowTaskBar > 0)
                    {
                        this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
                    }
                        
                    this.WindowState = FormWindowState.Maximized;
                    break;
            }

        }

        private void frmMain_Activated(object sender, EventArgs e)
        {
            Debug.WriteLine("Form Activate");
            setBankThemes();
        }

        private void btnTerminalTracker_Click(object sender, EventArgs e)
        {
            iMenu = 5;
            InitMenu(iMenu, true);
        }

        private void InitMenu(int iMenu, bool fVisible)
        {
            int iLeft = 15;
            //int iTop = 4;
            int yAxis = 60;
            var gbAdministrativeLocation = gbAdministrative.Location;
            var gbOperationLocation = gbOperation.Location;
            var gbHelpdeskLocation = gbHelpdesk.Location;
            var gbManagementLocation = gbManagement.Location;
            var gbOtherLocation = gbOther.Location;

            pnlSubMenuAdministrative.Visible = false;
            pnlSubMenuFinance.Visible = false;
            pnlSubMenuInventoryEntry.Visible = false;
            pnlSubMenuInventoryEntry.Visible = false;            
            pnlSubMenuEnrollment.Visible = false;
            pnlSubMenuAccount.Visible = false;
            pnlSubMenuReports.Visible = false;            
            pnlSubMenuServicing.Visible = false;
            pnlSubMenuLog.Visible = false;
            pnlSubMenuCustomerService.Visible = false;
            pnlSubMenuTools.Visible = false;
            pnlSubMenuPOSRental.Visible = false;
            pnlSubMenuMSP.Visible = false;
            pnlSubMenuSwitchBank.Visible = false;

            switch (iMenu)
            {
                case 1: // Administrative
                    pnlSubMenuAdministrative.Visible = fVisible;
                    pnlSubMenuAdministrative.Left = pnlMenu.Width - btnAdministrative.Left + iLeft;
                    pnlSubMenuAdministrative.Top = gbAdministrativeLocation.Y + yAxis;
                    break;
                case 2: // Terminal                    
                    pnlSubMenuInventoryEntry.Visible = fVisible;
                    pnlSubMenuInventoryEntry.Left = pnlMenu.Width - btnInventoryEntry.Left + iLeft;
                    pnlSubMenuInventoryEntry.Top = gbOperationLocation.Y + yAxis;
                    break;
                case 3: // Installation                    
                    break;
                case 4: // FSR                    
                    pnlSubMenuServicing.Visible = fVisible;
                    pnlSubMenuServicing.Left = pnlMenu.Width - btnServicing.Left + iLeft;
                    pnlSubMenuServicing.Top =  gbOperationLocation.Y + yAxis;
                    break;
                case 5: // Search   
                    pnlSubMenuReports.Visible = fVisible;
                    pnlSubMenuReports.Left = pnlMenu.Width - btnSearch.Left + iLeft;
                    pnlSubMenuReports.Top = pnlMenu.Top + btnSearch.Top;
                    break;
                case 6: // Report
                    pnlSubMenuReports.Visible = fVisible;
                    pnlSubMenuReports.Left = pnlMenu.Width - btnReports.Left + iLeft;
                    pnlSubMenuReports.Top = gbHelpdeskLocation.Y + yAxis;
                    break;
                case 7: // Enrollment
                    pnlSubMenuEnrollment.Visible = fVisible;
                    pnlSubMenuEnrollment.Left = pnlMenu.Width - btnEnrollment.Left + iLeft;
                    pnlSubMenuEnrollment.Top = gbOperationLocation.Y + yAxis;
                    break;
                case 8: // Accounts
                    pnlSubMenuAccount.Visible = fVisible;
                    pnlSubMenuAccount.Left = pnlMenu.Width - btnUserAcnt.Left + iLeft;
                    pnlSubMenuAccount.Top = gbManagementLocation.Y + yAxis;
                    break;
                case 9: // Logs
                    pnlSubMenuLog.Visible = fVisible;
                    pnlSubMenuLog.Left = pnlMenu.Width - btnLogs.Left + iLeft;
                    pnlSubMenuLog.Top = gbManagementLocation.Y + yAxis;
                    break;
                case 10: // Setting
                    break;
                case 11: // Tools
                    pnlSubMenuTools.Visible = fVisible;
                    pnlSubMenuTools.Left = pnlMenu.Width - btnToolsERMBilling.Left + iLeft;
                    pnlSubMenuTools.Top = gbManagementLocation.Y + yAxis;
                    break;
                case 12: // Help                    
                    break;
                case 13: // LogOff
                    break; 
                case 14: // Exit
                    break;
                case 15: // Enrollment - Particular                                        
                    break;
                case 16: // Enrollment - Terminal                    
                    break;
                case 17: // Enrollment - Service                    
                    break;
                case 18: // Enrollment - Others                    
                    break;
                case 19:                    
                    break;
                case 20:
                    pnlSubMenuFinance.Visible = fVisible;
                    pnlSubMenuFinance.Left = pnlMenu.Width - btnFinance.Left + iLeft;
                    pnlSubMenuFinance.Top = gbAdministrativeLocation.Y + yAxis;
                    break;
                case 21: // HelpDesk
                    pnlSubMenuCustomerService.Visible = fVisible;
                    pnlSubMenuCustomerService.Left = pnlMenu.Width + iLeft;
                    pnlSubMenuCustomerService.Top = gbHelpdeskLocation.Y + yAxis;
                    break;
                //case 22:
                //    pnlSubMenuCustomerService.Visible = fVisible;
                //    pnlSubMenuCustomerService.Left = pnlMenu.Width - btnJobOrderServicing.Left + iLeft;
                //    pnlSubMenuCustomerService.Top = gbHelpdeskLocation.Y + yAxis;
                //    break;
                case 22: // POS Rental
                    pnlSubMenuMSP.Visible = fVisible;
                    pnlSubMenuMSP.Left = pnlMenu.Width - btnMSP.Left + iLeft;
                    pnlSubMenuMSP.Top = gbAdministrativeLocation.Y + yAxis;
                    break;
                case 23: // Switch Bank
                    pnlSubMenuSwitchBank.Visible = fVisible;
                    pnlSubMenuSwitchBank.Left = pnlMenu.Width - btnSwitchBankCode.Left + iLeft;
                    pnlSubMenuSwitchBank.Top = gbManagementLocation.Y + yAxis;
                    break;
                default:
                    break;
            }
        }
        
        private void btnAdministrative_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            iMenu = 1;            
            InitMenu(iMenu, true);
        }

        private void btnAccounts_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            iMenu = 8;
            InitMenu(iMenu, true);
        }

        private void btnEnrollmentParticular_Click(object sender, EventArgs e)
        {
            iMenu = 15;
            InitMenu(iMenu, true);
        }

        private void btnEnrollmentTerminal_Click(object sender, EventArgs e)
        {
            iMenu = 16;
            InitMenu(iMenu, true);
        }

        private void btnEnrollmentService_Click(object sender, EventArgs e)
        {
            iMenu = 17;
            InitMenu(iMenu, true);
        }

        private void btnEnrollmentOther_Click(object sender, EventArgs e)
        {
            iMenu = 18;
            InitMenu(iMenu, true);
        }
        
        private void pnlMenuList_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void pnlMenuList_Click(object sender, EventArgs e)
        {
            
        }

        private void btnImportExport_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 31)) return;

            InitMenu(0, false);            
            frmImportTerminal.iTab = 0;
            frmImportTerminal.iTabSub = 0;
            frmImportTerminal frm = new frmImportTerminal();
            frm.Text = "INVENTORY-TERMINAL";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();            
        }

        private void btnAutoGenerate_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);
            frmImportTerminal.iTab = 0;
            frmImportTerminal.iTabSub = 1;
            frmImportTerminal frm = new frmImportTerminal();
            frm.Text = "INVENTORY-TERMINAL";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();            
        }       
        
        private void btnMerchant_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 6)) return;

            InitMenu(0, false);
            frmParticular.iParticularType = clsGlobalVariables.iMerchant_Type;
            frmParticular frm = new frmParticular();
            frm.Text = "ENROLLMENT-MERCHANT";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnClient_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 7)) return;

            InitMenu(0, false);
            frmParticular.iParticularType = clsGlobalVariables.iClient_Type;
            frmParticular frm = new frmParticular();
            frm.Text = "ENROLLMENT-CLIENT";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();            
        }

        private void btnFE_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 8)) return;

            InitMenu(0, false);
            frmParticular.iParticularType = clsGlobalVariables.iFE_Type;
            frmParticular frm = new frmParticular();
            frm.Text = "ENROLLMENT-VENDOR REPRESENTATIVE";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();            
        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);
            frmParticular.iParticularType = clsGlobalVariables.iSupplier_Type;
            frmParticular frm = new frmParticular();
            frm.Text = "ENROLLMENT-SUPPLIER";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();            
        }   

        private void btnTerminalType_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 9)) return;

            InitMenu(0, false);
            frmTerminalType frm = new frmTerminalType();
            frm.Text = "ENROLLMENT-TERMINAL TYPE";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();            
        }

        private void btnTerminalModel_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 10)) return;

            InitMenu(0, false);
            frmTerminalModel frm = new frmTerminalModel();
            frm.Text = "ENROLLMENT-TERMINAL MODEL";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();            
        }

        private void btnTerminalBrand_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);
            frmTerminalBrand frm = new frmTerminalBrand();
            frm.Text = "ENROLLMENT-TERMINAL BRAND";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();            
        }

        private void btnTerminalStatus_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);
            frmTerminalStatus frm = new frmTerminalStatus();
            frm.Text = "ENROLLMENT-TERMINAL STATUS";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();            
        }

        private void btnSP_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);
            frmParticular.iParticularType = clsGlobalVariables.iSP_Type;
            frmParticular frm = new frmParticular();
            frm.Text = "ENROLLMENT-PARTICULAR";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();            
        }

        private void btnServiceType_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);
            frmServiceType frm = new frmServiceType();
            frm.Text = "ENROLLMENT-SERVICE TYPE";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();            
        }

        private void btnOtherServiceType_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);
            frmOtherServiceType frm = new frmOtherServiceType();
            frm.Text = "ENROLLMENT-OTHER SERVICE TYPE";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();            
        }

        private void btnProvince_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 16)) return;

            InitMenu(0, false);
            frmRegion frm = new frmRegion();
            frm.Text = "ENROLLMENT-REGION";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();            
        }

        private void btnCity_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 15)) return;

            InitMenu(0, false);
            frmRegionDetail frm = new frmRegionDetail();
            frm.Text = "ENROLLMENT-PROVINCE";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();            
        }

        private void frmMain_Click(object sender, EventArgs e)
        {
            //var pnlConfigLoc = pnlConfig.Location;
            //MessageBox.Show("Form Width="+this.Width+"-"+"Form Height="+this.Height+"\n"+
            //               "Panel Config Width=" + pnlConfig.Width + " - " + "Panel Config Height=" + pnlConfig.Height+"\n"+
            //              "Panel Config X=" + pnlConfigLoc.X + " - " + "Panel Config Y=" + pnlConfigLoc.Y);     
            InitMenu(0, false);
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            
        }

        private void frmMain_Leave(object sender, EventArgs e)
        {
            
        }

        private void btnSIM_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 34)) return;

            InitMenu(0, false);            
            frmImportSIM.iTab = 0;
            frmImportSIM frm = new frmImportSIM();
            frm.Text = "INVENTORY-SIM";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();            
        }

        private void pnlHeader_DoubleClick(object sender, EventArgs e)
        {
            btnMaximize_Click(this, e);
        }

        private void pnlMenuList_Paint_1(object sender, PaintEventArgs e)
        {

        }       
        
        private void frmMain_Deactivate(object sender, EventArgs e)
        {
            Debug.WriteLine("Deactivate");
            //InitPulse(false);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Debug.WriteLine("Form Closing");
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Debug.WriteLine("Form Closed");
        }

        private void btnPulse_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            iMenu = 0;
            InitMenu(iMenu, false);

            //dbFunction.SetMessageBox("Ongoing development...", "Oooops", clsFunction.IconType.iInformation);
            return;
            
            //dbAPI.ResetAdvanceSearch();
            //frmSearchField.iSearchType = frmSearchField.SearchType.iDashboard;
            //frmSearchField.sHeader = "CLIENT FOR DASHBOARD";
            //frmSearchField.sSearchChar = clsFunction.sZero;
            //frmSearchField.isCheckBoxes = false;
            //frmSearchField frm = new frmSearchField();
            //frm.ShowDialog();

            //if (frmSearchField.fSelected)
            //{
            //    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

            //    clsSearch.ClassClientID = clsSearch.ClassParticularID;

            //    // Get Client Dashboard Detail
            //    dbAPI.ExecuteAPI("GET", "Search", "Client Dashboard Detail", clsSearch.ClassClientID.ToString(), "Particular Detail", "", "ViewParticularDetail");

            //    if (!clsGlobalVariables.isAPIResponseOK) return;

            //    if (dbAPI.isNoRecordFound() == false)
            //        clsParticular.RecordFound = true;
            //    else
            //        clsParticular.RecordFound = false;

            //    //if (!dbFunction.isValidID(clsSearch.ClassClientID.ToString())) clsSearch.ClassParticularName = "CASTLES";

            //    OpenWebDashboard(clsParticular.ClassParticularID.ToString(), clsSearch.ClassParticularName, clsParticular.ClassParticularUserKey);

            //    Cursor.Current = Cursors.Default; // Normal
            //}


        }

        private void btnReason_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 17)) return;

            dbAPI.ResetAdvanceSearch();
            clsSearch.ClassReasonType = clsGlobalVariables.REASON_TYPE;
            InitMenu(0, false);
            frmReason.sHeader = clsGlobalVariables.REASON_TYPE;
            frmReason frm = new frmReason();
            frm.Text = "ENROLLMENT-REASON";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }
       
        private void pnlSubMenuTerminal_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnGenerateTerminal_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 32)) return;

            InitMenu(0, false);            
            frmImportTerminal.iTab = 1;
            frmImportTerminal.iTabSub = 0;
            frmImportTerminal frm = new frmImportTerminal();
            frm.Text = "INVENTORY-TERMINAL";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnManualTerminal_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 33)) return;

            InitMenu(0, false);            
            frmImportTerminal.iTab = 2;
            frmImportTerminal.iTabSub = 0;
            frmImportTerminal frm = new frmImportTerminal();
            frm.Text = "INVENTORY-TERMINAL";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnGenerateSIM_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 35)) return;

            InitMenu(0, false);            
            frmImportSIM.iTab = 1;
            frmImportSIM frm = new frmImportSIM();
            frm.Text = "INVENTORY-SIM";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnManualSIM_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 36)) return;

            InitMenu(0, false);           
            frmImportSIM.iTab = 2;
            frmImportSIM frm = new frmImportSIM();
            frm.Text = "INVENTORY-SIM";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }
       
        private void btnInventoryEntryTerminal_Click(object sender, EventArgs e)
        {
            
        }

        private void btnInventoryEntrySIM_Click(object sender, EventArgs e)
        {
            
        }

        private void btnInventoryEntryOther_Click(object sender, EventArgs e)
        {
            
        }

        private void btnFinance_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            iMenu = 20;
            InitMenu(iMenu, true);
        }

        private void panel15_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnServicingImportIR_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 23)) return;

            InitMenu(0, false);
            frmImportIR.iTab = 0;
            frmImportIR frm = new frmImportIR();
            frm.Text = "SERVICING-IR";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnServicingManualEntryIR_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 24)) return;

            InitMenu(0, false);
            frmImportIR.iTab = 1;
            frmImportIR frm = new frmImportIR();
            frm.Text = "SERVICING-IR";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }
 
        private void btnServicingManualEntryFSR_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 26)) return;

            InitMenu(0, false);
            dbAPI.ResetAdvanceSearch();
            frmTerminalFSR.fAutoLoadData = false;
            frmTerminalFSR frm = new frmTerminalFSR();
            frm.Text = "FSR";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
            
        }

        
        

        private void btnSearch_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            iMenu = 0;
            InitMenu(iMenu, false);

            //dbFunction.SetMessageBox("Ongoing development...", "Oooops", clsFunction.IconType.iInformation);
            return;
            
        }

        private void InitPanelMenuList()
        {
            pnlMenuList.Top = 980;
            pnlMenuList.Left = 41;
            pnlMenuList.Width = 447;
            pnlMenuList.Height = 727;
        }
      
        private void lvwSearch_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label50_Click(object sender, EventArgs e)
        {

        }

        private void btnCustomerService_Click(object sender, EventArgs e)
        {
            iMenu = 21;
            InitMenu(iMenu, true);
        }

        private void btnCustomerServiceNewCall_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);
            frmServiceCall.sHeader = "NEW CALL";
            frmServiceCall frm = new frmServiceCall();
            frm.Text = "SERVICING-NEW CALL";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnAcntAdd_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 30)) return;

            InitMenu(0, false);            
            frmUser frm = new frmUser();
            frm.Text = "USER";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }
        
        private void OpenWebDashboard(modelDashboard model)
        {   
            string sURL = dbAPI.getAPISSLEnable() + model.IP + clsFunction.sColon + model.Port + model.Folder + clsFunction.sBackSlash + "service_detail.php" + clsFunction.sQuestionMark + 
                        "username=" + "eFSR Management Solution" + clsFunction.sAnd +
                        "searchby=" + model.SearchBy + clsFunction.sAnd +
                        "searchvalue=" + "UNCLOSED TICKET|0|0000-00-00|0000-00-00|[NOT%20SPECIFIED]|SUCCESS|RANGE|"+model.ClientID+"|0|0|"+model.DispatchID+"|[NOT SPECIFIED]|[NOT SPECIFIED]|0|[NOT SPECIFIED]&department=[NOT SPECIFIED]&jobtype=Unclosed Ticket&category=COMPLETED&title=Dispatcher&tcount=" + model.TCount + clsFunction.sAnd +
                        "userkey="+model.UserKey + clsFunction.sAnd +
                        "client="+model.ClientName + clsFunction.sAnd +
                        "bank="+clsGlobalVariables.strAPIBank + clsFunction.sAnd +
                        "searchmode="+clsFunction.sNull;

            Debug.WriteLine("sURL="+ sURL);

            Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

            try
            {
                Process.Start(sURL);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "MIS DashBoard", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            Cursor.Current = Cursors.Default; // Back to normal

        }
        
        private void btnImportOthers_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 37)) return;

            InitMenu(0, false);
            frmImportTerminal.iTab = 0;
            frmImportTerminal.iTabSub = 0;
            frmImportTerminal frm = new frmImportTerminal();
            frm.Text = "IMPORT TERMINAL";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnGnerateOthers_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 38)) return;

            InitMenu(0, false);
            frmImportTerminal.iTab = 1;
            frmImportTerminal.iTabSub = 0;
            frmImportTerminal frm = new frmImportTerminal();
            frm.Text = "IMPORT TERMINAL";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnManualOthers_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 39)) return;

            InitMenu(0, false);
            frmImportTerminal.iTab = 2;
            frmImportTerminal.iTabSub = 0;
            frmImportTerminal frm = new frmImportTerminal();
            frm.Text = "IMPORT TERMINAL";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }
        
        private void btnCustomerServiceReceiveCall_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);
            frmServiceCall.sHeader = "RECEIVE CALL";
            frmServiceCall frm = new frmServiceCall();
            frm.Text = "SERVICING-RECEIVE CALL";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
        
        private void btnCustomerServiceCallHistory_Click(object sender, EventArgs e)
        {
            dbFunction.SetMessageBox("It's under construction.", "Oops", clsFunction.IconType.iInformation);
            return;
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    //if (!fCloseFormConfirm()) return;
                    break;
                case Keys.T: // Terminal Inventory
                    if (e.Control)
                        btnInventoryTerminal_Click(this, e);
                    break;
                case Keys.S: // SIM Inventory
                    if (e.Control)
                        btnInventorySIM_Click(this, e);
                    break;
                case Keys.C: // Component
                    if (e.Control)
                        btnStockEntry_Click(this, e);
                    break;
                case Keys.I: // Installation
                    if (e.Control)
                        btnServicingIR_Click(this, e);
                    break;
                case Keys.J: // Servicing (JO)
                    if (e.Control)
                        btnJobOrderService_Click(this, e);
                    break;
                case Keys.M: // Manual FSR
                    if (e.Control)                   
                        btnServicingManualEntryFSR_Click(this, e);                    
                    break;
                case Keys.L: // Close Ticket
                    if (e.Control)
                        btnMaintenanceUpdateCloseTicket_Click(this, e);
                    break;
                case Keys.P: // Pending eFSR
                    if (e.Control)
                        btnMaintenancePendingeFSRGenerator_Click(this, e);
                    break;
                case Keys.F: // Failed Service
                    if (e.Control)
                        btnMaintenanceFailedService_Click(this, e);
                    break;
                case Keys.H: // Helpdesk
                    if (e.Control)
                        btnServiceMaintenance_Click(this, e);
                    break;

            }
        }

        private void InitPulse(bool fVisible)
        {
            if (clsSystemSetting.ClassSystemAutoPulse > 0)
            {
                iDecrement = iSecondPulse;                

                if (!fVisible)
                    tmrPulse.Enabled = false;
                else
                    tmrPulse.Enabled = true;
            }
            else
            {                
                tmrPulse.Enabled = false;
            }            
        }
        private void InitPulseTimer()
        {
            tmrPulse.Enabled = true;
            tmrPulse.Interval = 1000;
        }

        private void InitConn(bool fVisible)
        {
            iDecrementConn = iSecondConn;

            if (!fVisible)
                tmrConn.Enabled = false;
            else
                tmrConn.Enabled = true;
        }
        private void InitConnTimer()
        {
            tmrConn.Enabled = true;
            tmrConn.Interval = 1000;
        }

        private void GetWaitingForAssignmentCount()
        {
            // New Installation Request
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.STATUS_AVAILABLE + clsFunction.sPipe + clsGlobalVariables.STATUS_AVAILABLE_DESC;            
            dbAPI.GetViewCount("Search", "IR Status Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTInstallationReqCount.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);
            //---------------------------------------------------------------------------------------------------------

            // Days Pending Installation Request
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.STATUS_AVAILABLE + clsFunction.sPipe + clsGlobalVariables.STATUS_AVAILABLE_DESC + clsFunction.sPipe + clsSystemSetting.ClassSystemNoOfDayPending;
            dbAPI.GetViewCount("Search", "IR Days Pending Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTInstallationReqDaysPendingCount.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);
            //---------------------------------------------------------------------------------------------------------

            // Overdue Installation Request
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.STATUS_AVAILABLE + clsFunction.sPipe + clsGlobalVariables.STATUS_AVAILABLE_DESC;
            dbAPI.GetViewCount("Search", "IR OverDue Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTInstallationReqOverDueCount.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);
            //---------------------------------------------------------------------------------------------------------

        }

        private void ServiceRequestForDispatch()
        {            
            // Installation
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.STATUS_ALLOCATED + clsFunction.sPipe + clsGlobalVariables.STATUS_ALLOCATED_DESC + clsFunction.sPipe + clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
            dbAPI.GetViewCount("Search", "IR Status Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTInstallationCount.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);

            // Servicing
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.STATUS_SERVICING + clsFunction.sPipe + clsGlobalVariables.STATUS_SERVICING_DESC + clsFunction.sPipe + clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
            dbAPI.GetViewCount("Search", "Servicing Status Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTServicingCount.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);

            // Pull Out
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.STATUS_PULLED_OUT + clsFunction.sPipe + clsGlobalVariables.STATUS_PULLED_OUT_DESC + clsFunction.sPipe + clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
            dbAPI.GetViewCount("Search", "Servicing Status Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTPulloutCount.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);

            // Replacement
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.STATUS_REPLACEMENT + clsFunction.sPipe + clsGlobalVariables.STATUS_REPLACEMENT_DESC + clsFunction.sPipe + clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
            dbAPI.GetViewCount("Search", "Servicing Status Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTReplacementCount.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);

            // Reprogramming
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.STATUS_REPROGRAMMED + clsFunction.sPipe + clsGlobalVariables.STATUS_REPROGRAMMED_DESC + clsFunction.sPipe + clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
            dbAPI.GetViewCount("Search", "Servicing Status Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTReprogrammingCount.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);

        }

        private void NegativeServicing()
        {
            // Installation
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC + clsFunction.sPipe +
                                                clsGlobalVariables.ACTION_MADE_NEGATIVE + clsFunction.sPipe +
                                                clsGlobalVariables.SVC_REQ_OPEN + clsFunction.sPipe +
                                                clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;

            dbAPI.GetViewCount("Search", "Negative Servicing Status Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTNegativeInstallationCount.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);
            //---------------------------------------------------------------------------------------------------------

            // Servicing
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.JOB_TYPE_SERVICING_DESC + clsFunction.sPipe +
                                                clsGlobalVariables.ACTION_MADE_NEGATIVE + clsFunction.sPipe +
                                                clsGlobalVariables.SVC_REQ_OPEN + clsFunction.sPipe +
                                                clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;

            dbAPI.GetViewCount("Search", "Negative Servicing Status Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTNegativeServicingCount.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);
            //---------------------------------------------------------------------------------------------------------

            // Pullout
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.JOB_TYPE_PULLOUT_DESC + clsFunction.sPipe +
                                                clsGlobalVariables.ACTION_MADE_NEGATIVE + clsFunction.sPipe +
                                                clsGlobalVariables.SVC_REQ_OPEN + clsFunction.sPipe +
                                                clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;

            dbAPI.GetViewCount("Search", "Negative Servicing Status Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTNegativePulloutCount.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);
            //---------------------------------------------------------------------------------------------------------

            // Replacement
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC + clsFunction.sPipe +
                                                clsGlobalVariables.ACTION_MADE_NEGATIVE + clsFunction.sPipe +
                                                clsGlobalVariables.SVC_REQ_OPEN + clsFunction.sPipe +
                                                clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;

            dbAPI.GetViewCount("Search", "Negative Servicing Status Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTNegativeReplacementCount.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);
            //---------------------------------------------------------------------------------------------------------

            // Reprogramming
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC + clsFunction.sPipe +
                                                clsGlobalVariables.ACTION_MADE_NEGATIVE + clsFunction.sPipe +
                                                clsGlobalVariables.SVC_REQ_OPEN + clsFunction.sPipe +
                                                clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;

            dbAPI.GetViewCount("Search", "Negative Servicing Status Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTNegativeReprogrammingCount.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);
            //---------------------------------------------------------------------------------------------------------
        }

        private void ServiceRequestForFSR()
        {
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.STATUS_DISPATCH + clsFunction.sPipe + clsGlobalVariables.STATUS_DISPATCH_DESC + clsFunction.sPipe + clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING_DESC;
            dbAPI.GetViewCount("Search", "Servicing Status Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTDispatchCount.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);
        }

        private void ServiceRequestCompleted()
        {
            // Installed
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC + clsFunction.sPipe +
                                                clsGlobalVariables.ACTION_MADE_SUCCESS + clsFunction.sPipe +
                                                clsGlobalVariables.SVC_REQ_CLOSE;

            dbAPI.GetViewCount("Search", "Success Servicing Status Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTInstalledCountComplete.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);
            // ----------------------------------------------------------------------------------------------------

            // PulledOut
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.JOB_TYPE_PULLOUT_DESC + clsFunction.sPipe +
                                                clsGlobalVariables.ACTION_MADE_SUCCESS + clsFunction.sPipe +
                                                clsGlobalVariables.SVC_REQ_CLOSE;

            dbAPI.GetViewCount("Search", "Success Servicing Status Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTPulledOutCountComplete.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);
            // ----------------------------------------------------------------------------------------------------

            // Replacement
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC + clsFunction.sPipe +
                                                clsGlobalVariables.ACTION_MADE_SUCCESS + clsFunction.sPipe +
                                                clsGlobalVariables.SVC_REQ_CLOSE;

            dbAPI.GetViewCount("Search", "Success Servicing Status Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTReplacementCountComplete.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);
            // ----------------------------------------------------------------------------------------------------

            // Reprogramming
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC + clsFunction.sPipe +
                                                clsGlobalVariables.ACTION_MADE_SUCCESS + clsFunction.sPipe +
                                                clsGlobalVariables.SVC_REQ_CLOSE;

            dbAPI.GetViewCount("Search", "Success Servicing Status Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTReprogrammingCountComplete.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);
            // ----------------------------------------------------------------------------------------------------

            // Servicing
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.JOB_TYPE_SERVICING_DESC + clsFunction.sPipe +
                                                clsGlobalVariables.ACTION_MADE_SUCCESS + clsFunction.sPipe +
                                                clsGlobalVariables.SVC_REQ_CLOSE;

            dbAPI.GetViewCount("Search", "Success Servicing Status Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTServiceCountComplete.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);
            // ----------------------------------------------------------------------------------------------------

            // Cancelled
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.STATUS_CANCELLED + clsFunction.sPipe + clsGlobalVariables.STATUS_CANCELLED_DESC + clsFunction.sPipe + clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC;
            dbAPI.GetViewCount("Search", "IR Status Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            lblTCancelledCountComplete.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, iPaddedLength);
            // ----------------------------------------------------------------------------------------------------
        }


        private void tmrPulse_Tick(object sender, EventArgs e)
        {
            if (clsSystemSetting.ClassSystemAutoPulse > 0)
            {
                if (!fWebServerConn || !fFtpServerConn)
                {
                    //InitCount();
                    iCountPulse = 0;
                    iDecrement = iSecondPulse;
                    return;
                }
                    
                iCountPulse++;
                iDecrement--;

                if (iCountPulse > iSecondPulse)
                {
                    // Get Pulse
                    //Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
                    lblPulse.Text = "Please wait...";

                    GetWaitingForAssignmentCount();

                    ServiceRequestForDispatch();
                    
                    ServiceRequestForFSR();

                    ServiceRequestCompleted();

                    NegativeServicing();

                    CheckPulse();
                    
                    lblPulse.Text = "Complete";
                    //Cursor.Current = Cursors.Default; // Back to normal

                    iCountPulse = 0;
                    iDecrement = iSecondPulse;
                }

                lblPulse.Text = "Count: Update in " + iDecrement.ToString() + " " + "second(s)";
            }
        }

        private void tmrConn_Tick(object sender, EventArgs e)
        {
            if (clsSystemSetting.ClassSystemAutoCheckServer > 0)
            {
                iCountConn++;
                iDecrementConn--;

                if (iCountConn > iSecondConn)
                {
                    lblConn.Text = "Please wait...";
                    ServerConnStatus();                    
                    lblConn.Text = "Complete";

                    iCountConn = 0;
                    iDecrementConn = iSecondConn;
                }

                lblConn.Text = "Connection: checking in " + iDecrementConn.ToString() + " " + "second(s)";
            }                
        }

        private void ServerConnStatus()
        {
            fWebServerConn = false;
            fFtpServerConn = false;

            // Check WebServer
            if (clsSystemSetting.ClassSystemAutoPulse > 0)
            {
                if (!dbInternet.CheckServerSocket())
                {
                    lblWEBServer.Text = "OFFLINE";
                    lblWEBServer.ForeColor = Color.Red;
                    fWebServerConn = false;
                }
                else
                {
                    lblWEBServer.Text = "ONLINE";
                    lblWEBServer.ForeColor = Color.Lime;
                    fWebServerConn = true;
                }
            }
            else
            {
                lblWEBServer.Text = "DISABLED";
                lblWEBServer.ForeColor = Color.Gray;
                fWebServerConn = false;
            }


            // Check FtpServer
            if (clsSystemSetting.ClassSystemAutoPulse > 0)
            {
                if (!dbInternet.CheckFTPServerSocket())
                {
                    lblFTPServer.Text = "OFFLINE";
                    lblFTPServer.ForeColor = Color.Red;
                    fFtpServerConn = false;
                }
                else
                {
                    lblFTPServer.Text = "ONLINE";
                    lblFTPServer.ForeColor = Color.Lime;
                    fFtpServerConn = true;
                }
            }
            else
            {
                lblFTPServer.Text = "DISABLED";
                lblFTPServer.ForeColor = Color.Gray;
                fFtpServerConn = false;
            }

            // Check Internet
            if (clsSystemSetting.ClassSystemCheckNetLink > 0)
            {
                if (!dbInternet.CheckInternetConnection(clsSystemSetting.ClassSystemNetLink))
                {
                    lblInternetServer.Text = "OFFLINE";
                    lblInternetServer.ForeColor = Color.Red;                    
                }
                else
                {
                    lblInternetServer.Text = "ONLINE";
                    lblInternetServer.ForeColor = Color.Lime;                 
                }
            }
            else
            {
                lblInternetServer.Text = "DISABLED";
                lblInternetServer.ForeColor = Color.Gray;             
            }
        }

        private void InitCount()
        {
            lblTInstallationReqCount.Text = sPaddedCount;
            lblTInstallationReqDaysPendingCount.Text = sPaddedCount;
            lblTInstallationReqOverDueCount.Text = sPaddedCount;
            lblTInstallationCount.Text = sPaddedCount;
            lblTDispatchCount.Text = sPaddedCount;
        }

        private void CheckPulse()
        {
            lblTInstallationReqCount.Enabled = false;
            lblTInstallationReqDaysPendingCount.Enabled = false;
            lblTInstallationReqOverDueCount.Enabled = false;
            lblTInstallationCount.Enabled = false;
            lblTServicingCount.Enabled = false;
            lblTPulloutCount.Enabled = false;
            lblTReplacementCount.Enabled = false;
            lblTReprogrammingCount.Enabled = false;
            lblTDispatchCount.Enabled = false;

            lblTNegativeInstallationCount.Enabled = false;
            lblTNegativeServicingCount.Enabled = false;
            lblTNegativePulloutCount.Enabled = false;
            lblTNegativeReplacementCount.Enabled = false;
            lblTNegativeReprogrammingCount.Enabled = false;

            lblCOResolvedByPhone.Enabled = false;
            lblCOForServicing.Enabled = false;
            lblCONegative.Enabled = false;
            lblCONoAnswer.Enabled = false;

            lblRCResolvedByPhone.Enabled = false;
            lblRCForServicing.Enabled = false;
            lblRCNegative.Enabled = false;
            lblRCNoAnswer.Enabled = false;

            lblTInstalledCountComplete.Enabled = false;
            lblTPulledOutCountComplete.Enabled = false;
            lblTReplacementCountComplete.Enabled = false;
            lblTReprogrammingCountComplete.Enabled = false;
            lblTServiceCountComplete.Enabled = false;
            lblTCancelledCountComplete.Enabled = false;

            if (lblTInstallationReqCount.Text.CompareTo(sPaddedCount) != 0) lblTInstallationReqCount.Enabled = true;
            if (lblTInstallationReqDaysPendingCount.Text.CompareTo(sPaddedCount) != 0) lblTInstallationReqDaysPendingCount.Enabled = true;
            if (lblTInstallationReqOverDueCount.Text.CompareTo(sPaddedCount) != 0) lblTInstallationReqOverDueCount.Enabled = true;
            if (lblTInstallationCount.Text.CompareTo(sPaddedCount) != 0) lblTInstallationCount.Enabled = true;
            if (lblTServicingCount.Text.CompareTo(sPaddedCount) != 0) lblTServicingCount.Enabled = true;
            if (lblTPulloutCount.Text.CompareTo(sPaddedCount) != 0) lblTPulloutCount.Enabled = true;
            if (lblTReplacementCount.Text.CompareTo(sPaddedCount) != 0) lblTReplacementCount.Enabled = true;
            if (lblTReprogrammingCount.Text.CompareTo(sPaddedCount) != 0) lblTReprogrammingCount.Enabled = true;
            if (lblTDispatchCount.Text.CompareTo(sPaddedCount) != 0) lblTDispatchCount.Enabled = true;

            if (lblTNegativeInstallationCount.Text.CompareTo(sPaddedCount) != 0) lblTNegativeInstallationCount.Enabled = true;
            if (lblTNegativeServicingCount.Text.CompareTo(sPaddedCount) != 0) lblTNegativeServicingCount.Enabled = true;
            if (lblTNegativePulloutCount.Text.CompareTo(sPaddedCount) != 0) lblTNegativePulloutCount.Enabled = true;
            if (lblTNegativeReplacementCount.Text.CompareTo(sPaddedCount) != 0) lblTNegativeReplacementCount.Enabled = true;
            if (lblTNegativeReprogrammingCount.Text.CompareTo(sPaddedCount) != 0) lblTNegativeReprogrammingCount.Enabled = true;            

            if (lblCOResolvedByPhone.Text.CompareTo(sPaddedCount) != 0) lblCOResolvedByPhone.Enabled = true;
            if (lblCOForServicing.Text.CompareTo(sPaddedCount) != 0) lblCOForServicing.Enabled = true;
            if (lblCONegative.Text.CompareTo(sPaddedCount) != 0) lblCONegative.Enabled = true;
            if (lblCONoAnswer.Text.CompareTo(sPaddedCount) != 0) lblCONoAnswer.Enabled = true;

            if (lblRCResolvedByPhone.Text.CompareTo(sPaddedCount) != 0) lblRCResolvedByPhone.Enabled = true;
            if (lblRCForServicing.Text.CompareTo(sPaddedCount) != 0) lblRCForServicing.Enabled = true;
            if (lblRCNegative.Text.CompareTo(sPaddedCount) != 0) lblRCNegative.Enabled = true;
            if (lblRCNoAnswer.Text.CompareTo(sPaddedCount) != 0) lblRCNoAnswer.Enabled = true;

            if (lblTInstalledCountComplete.Text.CompareTo(sPaddedCount) != 0) lblTInstalledCountComplete.Enabled = true;
            if (lblTPulledOutCountComplete.Text.CompareTo(sPaddedCount) != 0) lblTPulledOutCountComplete.Enabled = true;
            if (lblTReplacementCountComplete.Text.CompareTo(sPaddedCount) != 0) lblTReplacementCountComplete.Enabled = true;
            if (lblTReprogrammingCountComplete.Text.CompareTo(sPaddedCount) != 0) lblTReprogrammingCountComplete.Enabled = true;
            if (lblTServiceCountComplete.Text.CompareTo(sPaddedCount) != 0) lblTServiceCountComplete.Enabled = true;
            if (lblTCancelledCountComplete.Text.CompareTo(sPaddedCount) != 0) lblTCancelledCountComplete.Enabled = true;
        }
        
        private void InitSkinColor()
        {
            try
            {
                if (!string.IsNullOrEmpty(clsSearch.ClassBankPrimaryColor))
                {
                    // Color bgcolor = ColorTranslator.FromHtml(clsSystemSetting.ClassSystemSkinColor);
                    Color bgprimary = ColorTranslator.FromHtml(clsSearch.ClassBankPrimaryColor);
                    Color bgmain = ColorTranslator.FromHtml(clsSearch.ClassBankMainColor);

                    pnlHeader.BackColor = bgprimary;
                    pnlFooter.BackColor = bgprimary;

                    pnlMenu.BackColor = bgmain;
                    gbAdministrative.BackColor = gbOperation.BackColor = gbHelpdesk.BackColor = gbManagement.BackColor = gbOther.BackColor = bgmain;
                }
            }
            catch
            {
                dbFunction.SetMessageBox(
                    "Color: " + dbFunction.AddBracketStartEnd(clsSearch.ClassBankPrimaryColor) + " is invalid.",
                    "Invalid color",
                    clsFunction.IconType.iExclamation
                );

                pnlHeader.BackColor = Color.DarkGray;
                pnlFooter.BackColor = Color.DarkGray;
                pnlMenu.BackColor = Color.DarkGray;

                gbAdministrative.BackColor = gbOperation.BackColor = gbHelpdesk.BackColor = gbManagement.BackColor = gbOther.BackColor = Color.Black;
            }
        }
        
        private void btnExpenses_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 19)) return;

            InitMenu(0, false);
            frmExpenses frm = new frmExpenses();
            frm.ShowDialog();
        }

        private void btnToolsERMBilling_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);            
            frmImportERM frmERM = new frmImportERM();
            frmERM.ShowDialog();

        }

        private void label24_Click(object sender, EventArgs e)
        {

        }

        private void CheckTrialVersion()
        {
            DateTime dCurrentDateTime = DateTime.Now;
            string sCurrentDateTime = "";
            string sInstalledDate = "";
            string sLastAccessDate = "";
            bool fValid = false;
            int iNoOfDays = 15;
            string sDays = "";
            DateTime dExpiredDate;
            string sExpiredDate = "";
            //string sEncMasterKey = dbFunction.EncryptString(clsFunction.initVector, clsFunction.initVector);

            Debug.WriteLine("--CheckTrialVersion--");

            sCurrentDateTime = dCurrentDateTime.ToString("yyyy-MM-dd");
            Debug.WriteLine("sCurrentDateTime=" + sCurrentDateTime);

            fValid = dbFunction.ReadFromRegistry("InstalledDate", ref sInstalledDate);
            Debug.WriteLine("fValid=" + fValid);
            Debug.WriteLine("sInstalledDate=" + sInstalledDate);

            if (fValid)
            {
                // Read NoOfDays
                fValid = dbFunction.ReadFromRegistry("NoOfDays", ref sDays);
                int iDays = int.Parse(sDays);

                if (iDays > 0)
                {
                    // Read Last AccessDate 
                    fValid = false;
                    fValid = dbFunction.ReadFromRegistry("LastAccessDate", ref sLastAccessDate);
                    Debug.WriteLine("fValid=" + fValid);
                    Debug.WriteLine("sLastAccessDate=" + sLastAccessDate);

                    if (fValid)
                    {
                        // Check expiry date
                        dExpiredDate = DateTime.Parse(sInstalledDate).AddDays(iNoOfDays);
                        sExpiredDate = dExpiredDate.ToString("yyyy-MM-dd");
                        Debug.WriteLine("sExpiredDate=" + sExpiredDate);

                        // Check Back Current Date
                        if (DateTime.Parse(sCurrentDateTime) < DateTime.Parse(sLastAccessDate))
                        {
                            MessageBox.Show("Back date detected. " + Environment.NewLine + Environment.NewLine +
                                "Please correct your current date.", "Customized ECR Simulator", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            EnableApplication(false);
                        }
                        else
                        {
                            if (DateTime.Parse(sCurrentDateTime) > DateTime.Parse(sExpiredDate))
                            {
                                MessageBox.Show("Your trial version has been expired." + Environment.NewLine + Environment.NewLine +
                                                "You need to email for the license key to continue using it." + Environment.NewLine + Environment.NewLine +
                                                "Developer: Stephen I. Dumili" + Environment.NewLine +
                                                "Email: stephendumili@castech.asia / tagok0911@yahoo.com.ph" + Environment.NewLine +
                                                "Mobile: 09278644531", "MIS: Click CASTLES LOGO to enter license key", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                EnableApplication(false);
                            }
                            else
                            {
                                dbFunction.WriteToRegistry("LastAccessDate", sCurrentDateTime);
                            }
                        }
                    }
                }
                else
                {
                    // Do nothing.... No checking of expiry
                }
            }
            else
            {
                dbFunction.WriteToRegistry("InstalledDate", sCurrentDateTime);
                dbFunction.WriteToRegistry("LastAccessDate", sCurrentDateTime);

                dExpiredDate = DateTime.Parse(sCurrentDateTime).AddDays(iNoOfDays);
                sExpiredDate = dExpiredDate.ToString("yyyy-MM-dd");

                dbFunction.WriteToRegistry("ExpiredDate", sExpiredDate);
                dbFunction.WriteToRegistry("NoOfDays", iNoOfDays.ToString());
                dbFunction.WriteToRegistry("MasterKey", clsFunction.MasterKey);
            }

            string sRemaining = "";
            string sNoOfDays = "";

            GetDaysRemaining(ref sRemaining, ref sNoOfDays);
            lblRemainingDays.Text = "Remaining/License: " + sRemaining + " of " + sNoOfDays + " Days(s)" + " " + "Trial";
        }

        private void EnableApplication(bool isEnable)
        {
            InitGroupMenu(isEnable);

            if (isEnable)
            {
                lblRemainingDays.Text = "";
            }
            else
            {
                lblRemainingDays.Text = "CLICK TO UNLOCK EXPIRY";
            }

        }
        private void GetDaysRemaining(ref string sRemaining, ref string sNoOfDays)
        {
            string sTemp = "";
            DateTime dCurrentDateTime = DateTime.Now;
            string sCurrentDateTime = "";
            string sDecInstalledDate = "";
            string sDecExpiryDate = "";
            string sDecNoOfDays = "";
            int iRemaining = 0;

            dbFunction.ReadFromRegistry("InstalledDate", ref sDecInstalledDate);
            dbFunction.ReadFromRegistry("ExpiredDate", ref sDecExpiryDate);
            dbFunction.ReadFromRegistry("NoOfDays", ref sDecNoOfDays);

            sCurrentDateTime = dCurrentDateTime.ToString("yyyy-MM-dd");
            Debug.WriteLine("sCurrentDateTime=" + sCurrentDateTime);

            Debug.WriteLine("sDecInstalledDate=" + sDecInstalledDate);
            Debug.WriteLine("sDecExpiryDate=" + sDecExpiryDate);
            Debug.WriteLine("sDecNoOfDays=" + sDecNoOfDays);

            iRemaining = ((TimeSpan)(DateTime.Parse(sDecExpiryDate) - DateTime.Parse(sCurrentDateTime))).Days;
            Debug.WriteLine("iRemaining=" + iRemaining);

            iRemaining = (iRemaining > 0 ? iRemaining : 0);

            sTemp = iRemaining.ToString();

            sRemaining = sTemp;
            sNoOfDays = sDecNoOfDays;

        }

        private void imgLogo_Click(object sender, EventArgs e)
        {
            if (!ShowMenuInputBox()) return;

            Debug.WriteLine("sInput" + sInput);            

            ParseAndWriteToRegistryExpiryParameter(sInput);
        }
        private bool ShowMenuInputBox()
        {
            string DefaultAmmount = "";

            InputBox.iInputType = clsFunction.AlphaNumeric_Input;
            InputBox.iInputLimitSize = 500;
            InputBoxResult MenuNum = InputBox.Show("Enter your license key: " + "\n", "License Key", DefaultAmmount, 100, 0, 255, (int)Enums.OptionType.Others);

            if (MenuNum.ReturnCode == DialogResult.OK)
            {
                sInput = MenuNum.Text;
                return true;
            }

            if (MenuNum.ReturnCode == DialogResult.No || MenuNum.ReturnCode == DialogResult.Cancel)
            {
                sInput = "";
                return false;
            }

            return false;
        }

        private void ParseAndWriteToRegistryExpiryParameter(string sEncGeneratedKey)
        {
            string sEncInstalledDate = "";
            string sEncLastAccessDate = "";
            string sEncExpiryDate = "";
            string sEncNoOfDays = "";
            string sEncMasterKey = "";

            string sDecInstalledDate = "";
            string sDecLastAccessDate = "";
            string sDecExpiryDate = "";
            string sDecNoOfDays = "";
            string sDecMasterKey = "";

            string[] sTemp = sEncGeneratedKey.Split('|');
            int iCount = sTemp.Length;
            try
            {
                sEncInstalledDate = sTemp[0].ToString();
                sEncLastAccessDate = sTemp[1].ToString();
                sEncExpiryDate = sTemp[2].ToString();
                sEncNoOfDays = sTemp[3].ToString();
                sEncMasterKey = sTemp[4].ToString();

                Debug.WriteLine("--Encrypted--");
                Debug.WriteLine("sInstalledDate=" + sEncInstalledDate);
                Debug.WriteLine("sLastAccessDate=" + sEncLastAccessDate);
                Debug.WriteLine("sExpiryDate=" + sEncExpiryDate);
                Debug.WriteLine("sNoOfDays=" + sEncNoOfDays);
                Debug.WriteLine("sMasterKey=" + sEncMasterKey);


                Debug.WriteLine("--Decrypted--");
                sDecInstalledDate = dbFunction.Decrypt(sEncInstalledDate, clsFunction.MasterKey);
                sDecLastAccessDate = dbFunction.Decrypt(sEncLastAccessDate, clsFunction.MasterKey);
                sDecExpiryDate = dbFunction.Decrypt(sEncExpiryDate, clsFunction.MasterKey);
                sDecNoOfDays = dbFunction.Decrypt(sEncNoOfDays, clsFunction.MasterKey);
                sDecMasterKey = dbFunction.Decrypt(sEncMasterKey, clsFunction.MasterKey);

                Debug.WriteLine("sDecInstalledDate=" + sDecInstalledDate);
                Debug.WriteLine("sDecLastAccessDate=" + sDecLastAccessDate);
                Debug.WriteLine("sDecExpiryDate=" + sDecExpiryDate);
                Debug.WriteLine("sDecNoOfDays=" + sDecNoOfDays);
                Debug.WriteLine("sDecMasterKey=" + sDecMasterKey);

                // Check MasterKey
                if (sDecMasterKey.CompareTo(clsFunction.MasterKey) != 0)
                {
                    EnableApplication(false);
                    MessageBox.Show("Invalid master key.", "MIS-Castles", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                if (!dbFunction.IsDateTime(sDecInstalledDate))
                {
                    EnableApplication(false);
                    MessageBox.Show("Invalid install date.", "MIS-Castles", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                if (!dbFunction.IsDateTime(sDecLastAccessDate))
                {
                    EnableApplication(false);
                    MessageBox.Show("Invalid last access date.", "MIS-Castles", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                if (!dbFunction.IsDateTime(sDecExpiryDate))
                {
                    EnableApplication(false);
                    MessageBox.Show("Invalid expiry date date.", "MIS-Castles", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                // Write To Registry (Descrypted)
                dbFunction.WriteToRegistry("InstalledDate", sDecInstalledDate);
                dbFunction.WriteToRegistry("LastAccessDate", sDecLastAccessDate);
                dbFunction.WriteToRegistry("ExpiredDate", sDecExpiryDate);
                dbFunction.WriteToRegistry("NoOfDays", sDecNoOfDays);
                dbFunction.WriteToRegistry("MasterKey", sDecMasterKey);

                // Write To Registry (Encrypted)
                //WriteToRegistry("InstalledDate", sEncInstalledDate);
                //WriteToRegistry("LastAccessDate", sEncLastAccessDate);
                //WriteToRegistry("ExpiredDate", sEncExpiryDate);
                //WriteToRegistry("NoOfDays", sEncNoOfDays);
                //WriteToRegistry("MasterKey", sEncMasterKey);

                MessageBox.Show("License key have been successfully applied. " + Environment.NewLine + Environment.NewLine +
                                "Application will be close and you may run it again.", "MIS-Castles", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString() + Environment.NewLine + Environment.NewLine +
                                "Please enter or ask another key value.", "Error: Invalid key value", MessageBoxButtons.OK, MessageBoxIcon.Error);

                EnableApplication(false);
            }

        }

        private void lblLogo_Click(object sender, EventArgs e)
        {
            //imgLogo_Click(this, e);
        }
        
        private void btnResolution_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 18)) return;

            dbAPI.ResetAdvanceSearch();
            clsSearch.ClassReasonType = clsGlobalVariables.RESOLUTION_TYPE;
            InitMenu(0, false);
            frmReason.sHeader = clsGlobalVariables.RESOLUTION_TYPE;
            frmReason frm = new frmReason();
            frm.ShowDialog();
        }

        private void InitGroupMenu(bool isVisible)
        {
            gbAdministrative.Visible = isVisible;
            gbOperation.Visible = isVisible;
            gbHelpdesk.Visible = isVisible;
            gbManagement.Visible = isVisible;            
        }

        private void btnEmployee_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);
            frmParticular.iParticularType = clsGlobalVariables.iEMP_Type;
            frmParticular frm = new frmParticular();
            frm.Text = "ENROLMENT-PARTICULAR";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }
        
        private void btnToolsLeave_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);
            frmLeaveType frm = new frmLeaveType();
            frm.Text = "ENROLLMENT-LEAVE TYPE";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnFinanceBilling_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 5)) return;

            InitMenu(0, false);
            frmImportERM frm = new frmImportERM();
            frm.Text = "ERM BILLING";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void bunifuFlatButton17_Click(object sender, EventArgs e)
        {

        }

        private void bunifuFlatButton22_Click(object sender, EventArgs e)
        {

        }

        private void btnLeaveType_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 20)) return;

            InitMenu(0, false);
            frmLeaveType frm = new frmLeaveType();
            frm.Text = "ENROLLMENT-LEAVE TYPE";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnAdminLeaveAssignment_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 2)) return;

            InitMenu(0, false);
            frmLeaveAssignment frm = new frmLeaveAssignment();
            frm.Text = "LEAVE ASSIGNMENT";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnAdminLeaveApplication_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 3)) return;

            InitMenu(0, false);
            frmLeaveApplication frm = new frmLeaveApplication();
            frm.Text = "LEAVE APPLICATION";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnHoliday_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 21)) return;

            InitMenu(0, false);
            frmHoliday frm = new frmHoliday();
            frm.Text = "ENROLMENT-HOLIDAY";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        
        private void bunifuFlatButton34_Click(object sender, EventArgs e)
        {

        }

        private void btnViewTimeSheet_Click(object sender, EventArgs e)
        {
            
        }

        private void pnlSubMenuAdministrative_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnAdminLeaveAdjustment_Click(object sender, EventArgs e)
        {

        }

        private void btnAdminWorkArrangement_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 1)) return;

            InitMenu(0, false);
            frmWorkArrangement frm = new frmWorkArrangement();
            frm.Text = "WORK ARRANGEMENT";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnViewNonWorkingDay_Click(object sender, EventArgs e)
        {

        }

        private void btnViewLeaveDetail_Click(object sender, EventArgs e)
        {

        }

        private void btnViewReportOption_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);
            frmPrintOption frm = new frmPrintOption();
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void pnlSubMenuReports_Paint(object sender, PaintEventArgs e)
        {

        }

        public void AddToMinizeForm(Form objFrm)
        {
            objFrm = new Form { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            pnlFooter.Controls.Add(objFrm);
            objFrm.Show();
        }

        private void btnAdminTimeSheet_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 4)) return;

            InitMenu(0, false);
            frmTimeSheet frm = new frmTimeSheet();
            frm.Text = "TIMESHEET";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnWorkType_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 22)) return;

            InitMenu(0, false);
            frmWorkType frm = new frmWorkType();
            frm.Text = "ENROLLMENT-WORK TYPE";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }
        
        private void btnViewReport_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 27)) return;

            InitMenu(0, false);
            frmPrintOption.iReportType = clsGlobalVariables.REPORT_TYPE_ADMINISTRATIVE;

            // ROCKY - PRINT OPTION ISSUE: DISPLAYING CLIENT IF CLIENT SEARCH.
            frmPrintOption.iParticularType = 0;

            frmPrintOption frm = new frmPrintOption();
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnOperationViewReport_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 29)) return;

            InitMenu(0, false);
            frmPrintOptionCriteria.iReportType = clsGlobalVariables.REPORT_TYPE_OPERATION;
            frmPrintOptionCriteria frm = new frmPrintOptionCriteria();
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnFinanceViewReport_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 28)) return;

            InitMenu(0, false);
            frmPrintOption.iReportType = clsGlobalVariables.REPORT_TYPE_FINANCE;

            // ROCKY - PRINT OPTION ISSUE: DISPLAYING CLIENT IF CLIENT SEARCH.
            frmPrintOption.iParticularType = 2;

            frmPrintOption frm = new frmPrintOption();
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnJobOrderService_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 25)) return;

            InitMenu(0, false);
            dbAPI.ResetAdvanceSearch();     
            frmServiceJobOrder.sHeader = "JOB ORDER";
            frmServiceJobOrder.fAutoLoadData = false;
            frmServiceJobOrder.fModify = false;    
            frmServiceJobOrder frm = new frmServiceJobOrder();
            frm.Text = "JOB ORDER";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnInventoryViewReport_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 27)) return;

            InitMenu(0, false);
            frmPrintOptionCriteria.iReportType = clsGlobalVariables.REPORT_TYPE_INVENTORY;
            frmPrintOptionCriteria frm = new frmPrintOptionCriteria();
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnInventoryTerminal_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 31)) return;

            InitMenu(0, false);           
            frmImportTerminal frm = new frmImportTerminal();
            frm.Text = "INVENTORY-TERMINAL";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnInventorySIM_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 34)) return;

            InitMenu(0, false);
            frmImportSIM frm = new frmImportSIM();
            frm.Text = "INVENTORY-SIM";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnServicingIR_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 23)) return;

            InitMenu(0, false);
            frmImportIR.iTab = 0;
            frmImportIR frm = new frmImportIR();
            frm.Text = "INSTALLATION REQUEST";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }
        
        private void btnPOSRentalCreateInvoice_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);          
            frmPRCreateInvoice frm = new frmPRCreateInvoice();
            frm.Text = "POS RENTAL - CREATE INVOICE";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnPOSRentalSendInvoice_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);
            frmPRSendInvoice frm = new frmPRSendInvoice();
            frm.Text = "POS RENTAL - SEND INVOICE";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnMaintenanceUpdateIRNo_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);
            frmMRequestID frm = new frmMRequestID();
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnMaintenanceUpdateAppsInfo_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 42)) return;

            InitMenu(0, false);
            frmAppsInfo frm = new frmAppsInfo();
            frm.Text = "APPLICATION INFORMATION";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnMaintenanceUpdateCloseTicket_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 40)) return;

            InitMenu(0, false);
            frmCloseTicket frm = new frmCloseTicket();
            frm.Text = "CLOSE TICKET";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnServiceBilling_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 1)) return;

            InitMenu(0, false);
            frmServicesBilling frm = new frmServicesBilling();
            frm.Text = "BILLING";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnMaintenanceUpdateDiagnostic_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 43)) return;

            InitMenu(0, false);
            frmDiagnostic frm = new frmDiagnostic();
            frm.Text = "UPDATE DIAGNOSTIC";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void loadWhosOnline()
        {
            int iLineNo = 0;
            int i = 0;

            Cursor.Current = Cursors.WaitCursor;

            lblWhoIsOnline.Text = clsFunction.sNull;

            dbAPI.ExecuteAPI("GET", "View", "Whos Online", "", "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                lblWhoIsOnline.Text = clsFunction.sNull;
                while (clsArray.ID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    string pTemp = dbFunction.AddBracketStartEnd(iLineNo.ToString()) + dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FullName) + clsDefines.gPipe +
                                   dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_PublishVersion) + Environment.NewLine; // + clsDefines.gPipe +
                                   //dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_LogTime) + Environment.NewLine;
                    
                    lblWhoIsOnline.Text += pTemp;
                    
                    i++;
                }
                
            }

            Cursor.Current = Cursors.Default;
        }

        private void lblWhosOnline_Click(object sender, EventArgs e)
        {
            loadWhosOnline();
        }

        private void loadWhosOnlineFSR()
        {
            int iLineNo = 0;
            int i = 0;

            Cursor.Current = Cursors.WaitCursor;

            lblWhoIsOnlineFSR.Text = clsFunction.sNull;

            dbAPI.ExecuteAPI("GET", "View", "Whos eFSR Online", "", "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                lblWhoIsOnlineFSR.Text = clsFunction.sNull;
                while (clsArray.ID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    string pTemp = dbFunction.AddBracketStartEnd(iLineNo.ToString()) + dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FullName) + clsDefines.gPipe +
                                   dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MobileVersion) + Environment.NewLine; // + clsDefines.gPipe +
                                   //dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FSRTime) + Environment.NewLine;

                    lblWhoIsOnlineFSR.Text += pTemp;

                    i++;
                }

            }

            Cursor.Current = Cursors.Default;
        }

        private void lblWhosOnlineFSR_Click(object sender, EventArgs e)
        {
            loadWhosOnlineFSR();
        }

        private void btnMaintenancePendingeFSRGenerator_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);
            frmPendingFSRGenerator frm = new frmPendingFSRGenerator();
            frm.Text = "PENDING eFSR GENERATOR";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnStockEntry_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 41)) return;

            InitMenu(0, false);
            frmStockEntry frm = new frmStockEntry();
            frm.Text = "CONPONENTS ENTRY";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnStockAdjustment_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);
            frmStockAdjustment frm = new frmStockAdjustment();
            frm.Text = "COMPONENTS ADJUSTMENT";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnStockTransfer_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);
            frmStockTransfer frm = new frmStockTransfer();
            frm.Text = "COMPONENTS TRANSFER";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnMaintenanceServiceReverse_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 53)) return;

            InitMenu(0, false);
            frmServiceOveride frm = new frmServiceOveride();
            frm.Text = "OVERRIDE SERVICE";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnMaintenanceFailedService_Click(object sender, EventArgs e)
        {
            InitMenu(0, false);
            frmPendingFailedService frm = new frmPendingFailedService();
            frm.Text = "FAILED SERVICE";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnType_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 44)) return;

            InitMenu(0, false);
            frmMType.sHeader = "CUSTOM TYPE";
            frmMType frm = new frmMType();
            frm.ShowDialog();
        }

        private void btnAutomate_Click(object sender, EventArgs e)
        {
                                 
        }

        private void TimerDashboardElapsed(object sender, ElapsedEventArgs e)
        {
            string TInstallation = "";
            string TReprogramming = "";
            string TServicing = "";
            string TReplacement = "";
            string TPullout = "";

            if (_timer.Enabled)
            {
                // get count pending fsr
                UpdateLabel(lblPendingFSR, clsFunction.sZero);                
                dbAPI.GetViewCount("Search", "Generate FSR Count", "", "Get Count");                
                if (dbAPI.isNoRecordFound() == false)
                    UpdateLabel(lblPendingFSR, clsTerminal.ClassTerminalCount.ToString());
                
                // get count failed service
                UpdateLabel(lblFailedService, clsFunction.sZero);
                dbAPI.GetViewCount("Search", "Failed Service Count", "", "Get Count");
                if (dbAPI.isNoRecordFound() == false)
                    UpdateLabel(lblFailedService, clsTerminal.ClassTerminalCount.ToString());

                // get region pending
                UpdateLabel(lblTInstallation, clsFunction.sZero);
                UpdateLabel(lblTReprogramming, clsFunction.sZero);
                UpdateLabel(lblTServicing, clsFunction.sZero);
                UpdateLabel(lblTReplacement, clsFunction.sZero);
                UpdateLabel(lblTPullout, clsFunction.sZero);
                loadRegionSummary(ref TInstallation, ref TReprogramming, ref TServicing, ref TReplacement, ref TPullout);
                UpdateLabel(lblTInstallation, TInstallation);
                UpdateLabel(lblTReprogramming, TReprogramming);
                UpdateLabel(lblTServicing, TServicing);
                UpdateLabel(lblTReplacement, TReplacement);
                UpdateLabel(lblTPullout, TPullout);
                
            }            
        }

        private void UpdateLabel(Label lbl, string message)
        {
            if (lbl.InvokeRequired)
            {
                // InvokeRequired is true if the calling thread is not the UI thread
                lbl.Invoke(new Action(() => lbl.Text = message));
            }
            else
            {
                // Direct update if already on the UI thread
                lbl.Text = message;
            }
           
        }

        private void loadRegionSummary(ref string TInstallation, ref string TReprogramming, ref string TServicing, ref string TReplacement, ref string TPullout)
        {
            int i  = 0;
            string firstdate = "";
            string lastdate = "";

            dbFunction.getCurrentFirstAndLastDate(ref firstdate, ref lastdate);            
            dbAPI.ExecuteAPI("GET", "View", "Region Service Summary",  clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC + clsFunction.sPipe + firstdate + clsFunction.sPipe + lastdate, "Advance Detail", "", "ViewAdvanceDetail");
            if (!clsGlobalVariables.isAPIResponseOK) return;
            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.ID.Length > i)
                {
                    string pRegion = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_REGION) + " : ";
                    TInstallation += pRegion + dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TInstallation) + "\n";
                    TReprogramming += pRegion + dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TReprogramming) + "\n";
                    TServicing += pRegion + dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TServicing) + "\n";
                    TReplacement += pRegion + dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TReplacement) + "\n";
                    TPullout += pRegion + dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TPullOut) + "\n";

                    i++;

                }                
            }
        }

        private void lblAutomate_Click(object sender, EventArgs e)
        {
            double refresh = clsSystemSetting.ClassSystemPulseInterval * 10000; // 30secs

            lblFailedService.Text = lblPendingFSR.Text = clsFunction.sZero;

            if (clsSystemSetting.ClassSystemPulseInterval > 0)
            {
                _timer = new Timer(refresh);
                _timer.Elapsed += TimerDashboardElapsed;

                if (lblAutomate.Text.Equals("REALTIME MONITORING - OFF"))
                {
                    lblAutomate.Text = "REALTIME MONITORING - ON";
                    _timer.Enabled = false;
                    lblPendingFSR.Text = lblFailedService.Text = lblTInstallation.Text = lblTReprogramming.Text = lblTServicing.Text = lblTReplacement.Text = lblTPullout.Text = clsFunction.sZero;
                    lblFailedService.ForeColor = lblPendingFSR.ForeColor = lblTInstallation.ForeColor = lblTReprogramming.ForeColor = lblTServicing.ForeColor = lblTReplacement.ForeColor = lblTPullout.ForeColor = Color.Gray;
                }
                else
                {
                    lblAutomate.Text = "REALTIME MONITORING - OFF";
                    _timer.Enabled = true;
                    lblFailedService.ForeColor = lblPendingFSR.ForeColor = lblTInstallation.ForeColor = lblTReprogramming.ForeColor = lblTServicing.ForeColor = lblTReplacement.ForeColor = lblTPullout.ForeColor = Color.Yellow;

                    TimerDashboardElapsed(this, null);
                }
            }
        }

        private void btnMSP_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            iMenu = 22;
            InitMenu(iMenu, true);
        }

        private void btnMSPEnrollment_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 45)) return;

            InitMenu(0, false);
            frmParticular.iParticularType = clsGlobalVariables.iMerchant_Type;
            frmParticular frm = new frmParticular();
            frm.Text = "MSP-MERCHANT";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnMSPOnboarding_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 46)) return;

            InitMenu(0, false);
            frmMerchantOnboarding frm = new frmMerchantOnboarding();
            frm.Text = "MSP-MERCHANT ONBOARDING";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnMSPValidation_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 47)) return;

            InitMenu(0, false);
            frmMerchantValidation frm = new frmMerchantValidation();
            frm.Text = "MSP-MERCHANT VALIDATION";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void lblPendingFSR_Click(object sender, EventArgs e)
        {
            btnMaintenancePendingeFSRGenerator_Click(this, e);
        }

        private void lblFailedService_Click(object sender, EventArgs e)
        {
            btnMaintenanceFailedService_Click(this, e);
        }

        private void loadUnclosedTicketList()
        {
            int lineno = 0;
            
            lvwList.Items.Clear();

            mList = _mServicingDetailController.getDetailList("Unclosed Ticket List", "");

            if (mList != null)
            {
                foreach (var itemData in mList)
                {
                    lineno++;
                    ListViewItem item = new ListViewItem(lineno.ToString());

                    if (clsSearch.ClassCurrentParticularID.Equals(itemData.DispatchID))
                        item.ForeColor = Color.Cyan;
                    else
                        item.ForeColor = Color.LightGray;

                    item.SubItems.Add(itemData.DispatchID.ToString());
                    item.SubItems.Add(itemData.Dispatcher);
                    item.SubItems.Add(itemData.TCount.ToString());

                    lvwList.Items.Add(item);
                }

                resizePanelTicket(mList.Count);
            }
        }

        private void resizePanelTicket(int count)
        {
            // Auto-size Panel height based on the number of records
            int itemHeight = 20; // Approximate height of each ListView item
            int maxHeight = 400; // Maximum panel height
            int minHeight = 50; // Minimum panel height (if no records)
            int newHeight = Math.Min(Math.Max((count * itemHeight) + 80, minHeight), maxHeight);

            if (count > 0)
                pnlTicket.Height = newHeight; // Resize the Panel
        }


        private void btnRefresh_Click(object sender, EventArgs e)
        {
            loadUnclosedTicketList();
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            if (btnShow.Tag?.ToString() == "up")
            {
                btnShow.Image = Properties.Resources.ic_down;
                btnShow.Tag = "down";
                pnlTicket.Size = new Size(371, 26);

            }
            else
            {
                btnShow.Image = Properties.Resources.ic_up;
                btnShow.Tag = "up";

                resizePanelTicket(mList.Count);
            }
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            string pMessage = "";

            if (lvwList.Items.Count > 0)
            {
                string pSelectedRow = dbFunction.GetListViewSelectedRow(lvwList, 0);
                Debug.WriteLine("pSelectedRow=\n" + pSelectedRow);

                pMessage = "Are you sure to open browser for details below:\n" +
                                   clsFunction.sLineSeparator + "\n" +
                                   "> Dispatch ID: " + dbFunction.GetSearchValue("ID") + "\n" +
                                   "> Dispatcher: " + dbFunction.GetSearchValue("Dispatcher");

                if (MessageBox.Show(pMessage, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    return;
                }
                else
                {
                    modelDashboard model = new modelDashboard();

                    model.IP = clsGlobalVariables.strAPIServerIPAddress;
                    model.Port = clsGlobalVariables.strAPIServerPort;
                    model.UserName = clsGlobalVariables.strAPIAuthUser;
                    model.Password = clsGlobalVariables.strAPIAuthPassword;
                    model.UserKey = clsGlobalVariables.strAPIKeys;
                    model.Folder = clsGlobalVariables.strAPIFolder;

                    model.ClientID = clsSystemSetting.ClassSystemClientID;
                    model.ClientName = clsSystemSetting.ClassSystemClientName;
                    model.SearchBy = "Service Detail List";
                    model.SearchValue = dbFunction.GetSearchValue("ID");
                    model.DispatchID = int.Parse(dbFunction.CheckAndSetNumericValue(dbFunction.GetSearchValue("ID")));
                    model.TCount = int.Parse(dbFunction.CheckAndSetNumericValue(dbFunction.GetSearchValue("Count")));

                    OpenWebDashboard(model);
                }
            }            
        }

        private void btnServiceMaintenance_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 48)) return;

            InitMenu(0, false);
            dbAPI.ResetAdvanceSearch();
            //frmHelpDesk.sHeader = "HELPDESK - MAINTENANCE";
            //frmHelpDesk.fAutoLoadData = false;
            //frmHelpDesk.fModify = false;
            frmHelpDesk frm = new frmHelpDesk();
            frm.Text = "HELPDESK";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnMaintenanceUpdateRequsetID_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 49)) return;

            InitMenu(0, false);
            frmMUpdateRequestID frm = new frmMUpdateRequestID();
            frm.Text = "UPDATE REQUEST ID";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnMaintenanceUpdateServiceStatus_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 50)) return;

            InitMenu(0, false);
            frmMUpdateServiceStatus frm = new frmMUpdateServiceStatus();
            frm.Text = "UPDATE SERVICE STATUS";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }
        
        private void loadWhosOnlineDashboard()
        {
            int iLineNo = 0;
            int i = 0;

            Cursor.Current = Cursors.WaitCursor;

            lblWhoIsOnlineDashboard.Text = clsFunction.sNull;

            dbAPI.ExecuteAPI("GET", "View", "Whos Dashboard Online", "", "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                lblWhoIsOnlineDashboard.Text = clsFunction.sNull;
                while (clsArray.ID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    string pTemp = dbFunction.AddBracketStartEnd(iLineNo.ToString()) + dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FullName) + Environment.NewLine; 
                               
                    lblWhoIsOnlineDashboard.Text += pTemp;

                    i++;
                }

            }

            Cursor.Current = Cursors.Default;
        }

        private void lblWhosOnlineDashboard_Click(object sender, EventArgs e)
        {
            loadWhosOnlineDashboard();
        }

        private void btnMSPDocumentGenerator_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 53)) return;

            InitMenu(0, false);
            frmMerchantDocumentGenerator frm = new frmMerchantDocumentGenerator();
            frm.Text = "MSP-MERCHANT DOCUMENNT GENERATOR";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void setBankThemes()
        {
            pnlHeader.BackColor = pnlFooter.BackColor = ColorTranslator.FromHtml(clsSearch.ClassBankPrimaryColor);
            this.BackColor = lvwList.BackColor = ColorTranslator.FromHtml(clsSearch.ClassBankSecondaryColor);

        }

        private void closeAllOpenForms(params Form[] excludeForms)
        {
            Debug.WriteLine("--closeAllOpenForms--");

            var excludeSet = new HashSet<Form>(excludeForms);

            // Create a stable copy first to avoid collection-modification issues
            List<Form> openForms = Application.OpenForms.Cast<Form>().ToList();

            foreach (Form form in openForms)
            {
                Debug.WriteLine($"Open: {form.Name}, Visible: {form.Visible}, Disposed: {form.IsDisposed}");
                
                if (!excludeSet.Contains(form))
                {
                    if (form.InvokeRequired)
                    {
                        form.Invoke(new Action(() =>
                        {
                            ForceCloseForm(form);
                        }));
                    }
                    else
                    {
                        ForceCloseForm(form);
                    }
                }
            }
        }

        private void hideAllForms(Form exceptForm)
        {
            Debug.WriteLine("--hideAllFormsExcept--");

            foreach (Form form in Application.OpenForms.Cast<Form>().ToList())
            {
                if (form != exceptForm && form.Visible)
                {
                    if (form.InvokeRequired)
                    {
                        form.Invoke(new Action(() => form.Hide()));
                    }
                    else
                    {
                        form.Hide();
                    }
                }
            }
        }


        private void ForceCloseForm(Form form)
        {
            try
            {
                if (!form.IsDisposed && form.Visible)
                {
                    Debug.WriteLine($"Closing form: {form.Name}");
                    form.Close();

                    if (!form.IsDisposed)
                        form.Dispose();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error closing form {form.Name}: {ex.Message}");
            }
        }
        
        private void initShortCutKeyboard()
        {
            lblShortCutKeyboard.Text =
                                    "CTRL+T > Terminal Inventory\n" +
                                    "CTRL+S > SIM Inventory\n" +
                                    "CTRL+C > Component Inventory\n" +
                                    "CTRL+I > Installation\n" +
                                    "CTRL+J > Job Order\n" +
                                    "CTRL+M > Manual FSR\n" +
                                    "CTRL+L > Close Ticket\n" +
                                    "CTRL+P > Pending eFSR\n" +
                                    "CTRL+F > Failed Service\n" +
                                    "CTRL+H > Helpdesk\n" +
                                    "ESC    > Close Window / Form";

        }
        
        private void btnSwitchBankCode_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            iMenu = 23;
            InitMenu(iMenu, true);

            dbFunction.ClearListViewItems(lvwBank);
            fillListViewBankSelection(lvwBank);
        }

        private void fillListViewBankSelection(ListView lvw)
        {
            Debug.WriteLine("--fillListViewBankSelection--");

            dbFile = new clsFile();
            dbFunction = new clsFunction();

            string filepath = dbFile.sSettingPath + clsDefines.RESP_BANKLIST_FILENAME;
            if (!dbFile.FileExist(filepath))
            {
                dbFunction.SetMessageBox("Bank list file does not exist.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return;
            }

            var banks = dbFunction.loadBankList(filepath);

            // Populate list
            int lineNo = 1;
            foreach (var b in banks)
            {
                if (!b.DisplayName.Equals(clsFunction.sDefaultSelect))
                {
                    var item = new ListViewItem(lineNo.ToString());
                    item.SubItems.Add(lineNo.ToString());
                    item.SubItems.Add(b.Code);
                    item.SubItems.Add(b.DisplayName);
                    lvw.Items.Add(item);

                    lineNo++;
                }                
            }
            
        }

        private void lvwBank_DoubleClick(object sender, EventArgs e)
        {
            if (lvwBank.SelectedItems.Count == 0)
                return;

            // Get the first selected item
            ListViewItem selectedItem = lvwBank.SelectedItems[0];

            // The DisplayName is in SubItem[3]
            string displayName = selectedItem.SubItems[3].Text;

            if (displayName == AppSession.BankName)
            {
                dbFunction.SetMessageBox("You are already logged in to the selected bank.\nPlease choose a different bank to switch.", "Switch Bank", clsFunction.IconType.iInformation);
                return;
            }

            string promptMessage = $"Switch to the selected bank: {displayName}?";
            if (!dbFunction.fPromptConfirmation(promptMessage))
                return;

            var loginForm = new frmLogin();
            hideAllForms(loginForm);
            loginForm.IsSwitchBankMode = true;

            AppSession.BankName = displayName;
            loginForm.autoFillAppSession(AppSession.BankCode, AppSession.BankName, AppSession.Username, AppSession.Password);

            this.Hide(); // Hide the current form (frmMain or wherever called)

            DialogResult result = loginForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                // Close all *existing* forms including the old frmMain, AFTER login is done
                closeAllOpenForms(); // Don't pass anything here unless absolutely necessary

                // Now start a brand new frmMain (safe)
                frmMain newMain = new frmMain();
                newMain.ShowDialog(); // or ShowDialog if it's the main window
            }
            else
            {
                this.Show(); // Login was cancelled
            }

        }

        void UpdateLabelSafe(Label label, string text)
        {
            if (label.InvokeRequired)
            {
                label.Invoke(new Action(() =>
                {
                    label.Text = text;
                    label.Refresh(); // Forces UI to update instantly
                }));
            }
            else
            {
                label.Text = text;
                label.Refresh();
            }
        }

        private void btnMaintenanceUpdateMerchantSN_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            //if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 49)) return;

            InitMenu(0, false);
            frmMUpdateMerchantSN frm = new frmMUpdateMerchantSN();
            frm.Text = "UPDATE MERCHANT SN";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnFinanceSettlement_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 54)) return;

            InitMenu(0, false);
            frmImportSettlement frm = new frmImportSettlement();
            frm.Text = "ERM SETTLEMENT REPORT";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }

        private void btnToolsInventoryDeletion_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 31)) return;
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 34)) return;
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 41)) return;

            InitMenu(0, false);
            frmMInventoryDeletion frm = new frmMInventoryDeletion();
            frm.Text = "TOOLS-INVENTORY DELETION";
            frm.WindowState = FormWindowState.Normal;
            frm.Show();
        }
    }
}
