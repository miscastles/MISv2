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
    public partial class frmTerminalAllocation : Form
    {
        public static bool fAutoLoadData = false;
        public static bool fModify = false;
        private clsAPI dbAPI;
        private clsINI dbSetting;
        private clsFunction dbFunction;        
        public static clsAPI.JobType iJobType;
        string sNegative;

        private static bool isDispatch;
        private static int iStatus;
        private static string sStatusDesc;

        bool fEdit = false;
        int iLimit = 1000;

        // Hold Variable
        string sHoldSPID;
        string sHoldSPName;
        string sHoldClientID;
        string sHoldClientName;
        string sHoldTerminalID;
        string sHoldTerminalSN;
        string sHoldSIMID;
        string sHoldSIMSN;
        string sHoldDockID;
        string sHoldDockSN;
        string sHoldServiceNo;
        string sHoldRequestNo;

        string sOldTerminalID;
        string sOldSIMID;
        string sOldDockID;       

        public frmTerminalAllocation()
        {
            InitializeComponent();
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
        private void frmTerminalAllocation_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbSetting = new clsINI();
            dbFunction = new clsFunction();

            dbSetting.InitDatabaseSetting();

            btnClear_Click(this, e);
            InitMessageCountLimit();
            InitSerialStatusDescription();

            Debug.WriteLine("frmTerminalAllocation_Load::" + "\n" +
                        "iJobType=" + iJobType.ToString() + "|" + "\n" +
                        "ClassIRStatus=" + clsSearch.ClassIRStatus + "|" + "\n" +
                        "ClassIRStatusDescription=" + clsSearch.ClassIRStatusDescription + "|" + "\n" +
                        "ClassStatus=" + clsSearch.ClassStatus + "|" + "\n" +
                        "ClassStatusDescription=" + clsSearch.ClassStatusDescription + "|" + "\n" +
                        "ClassServiceNo=" + clsSearch.ClassServiceNo + "|" + "\n" +
                        "ClassRequestNo=" + clsSearch.ClassRequestNo + "|" + "\n" +
                        "ClassIRIDNo=" + clsSearch.ClassIRIDNo + "|" + "\n" +
                        "ClassIRNo=" + clsSearch.ClassIRNo + "|" + "\n" +
                        "ClassTAIDNo=" + clsSearch.ClassTAIDNo + "|" + "\n" +
                        "ClassFEID=" + clsSearch.ClassFEID + "|" + "\n" +
                        "ClassFEName=" + clsSearch.ClassFEName + "|" + "\n" +
                        "ClassClientID=" + clsSearch.ClassClientID + "|" + "\n" +
                        "ClassClientName=" + clsSearch.ClassClientName + "|" + "\n" +
                        "ClassMerchantID=" + clsSearch.ClassMerchantID + "|" + "\n" +
                        "ClassMerchantName=" + clsSearch.ClassMerchantName + "|" + "\n" +
                        "ClassServiceProviderID=" + clsSearch.ClassServiceProviderID + "|" + "\n" +
                        "ClassServiceProviderName=" + clsSearch.ClassServiceProviderName + "|" + "\n" +
                        "ClassTerminalID=" + clsSearch.ClassTerminalID + "|" + "\n" +
                        "ClassTerminalSN=" + clsSearch.ClassTerminalSN + "|" + "\n" +
                        "ClassSIMID=" + clsSearch.ClassSIMID + "|" +"\n" + "\n" +
                        "ClassSIMSerialNo=" + clsSearch.ClassSIMSerialNo + "|" + "\n" +
                        "ClassDockID=" + clsSearch.ClassDockID + "|" + "\n" +
                        "ClassDockSN=" + clsSearch.ClassDockSN + "|" + "\n" +
                        "\n");

            sNegative = "";

            if (fAutoLoadData)
            {
                if (fModify)
                {
                    LoadTA();
                }
                else
                {
                    if (clsSearch.ClassIRStatus == clsGlobalVariables.STATUS_NEGATIVE || clsSearch.ClassStatus == clsGlobalVariables.STATUS_NEGATIVE)
                    {                        
                        sNegative = "-NEGATIVE";

                        LoadTA();

                        sHoldServiceNo = txtServiceNo.Text;
                        sHoldRequestNo = txtRequestNo.Text;

                        // Get Service Request ID (Recreate)
                        int iControlNo = dbAPI.GetControlID("Servicing Detail");
                        string sServiceRequestID = iControlNo.ToString();
                        sServiceRequestID = dbFunction.GenerateControlNo(iControlNo, "SR");
                        clsSearch.ClassRequestNo = sServiceRequestID;
                        txtRequestNo.Text = sServiceRequestID;

                        fEdit = true;
                        btnDispatchIR.Enabled = true;
                        txtRequestNo.BackColor = clsFunction.DisableBackColor;

                    }
                    else
                    {
                        LoadIR();
                    }                    
                }
                    
                btnAdd.Enabled = false;
                btnSave.Enabled = true;

                InitSearchButton(true);
                InitStatusTitle(true);
            }

            InitCustomer(false);
            //CheckWindowState();

        }
        
        private void btnAdd_Click(object sender, EventArgs e)
        {   
            fEdit = false;
            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);
            
            InitButton();
            
            btnAdd.Enabled = false;
            btnSave.Enabled = true;

            btnIRSearch.Enabled = true;
            btnTASearch.Enabled = false;
            btnTSearch.Enabled = true;
            btnSIMSearch.Enabled = true;

            dbFunction.TextBoxUnLock(true, this);
            PKTextBoxBackColor(true);
            
            //btnSearch_Click(this, e);

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {            
            fEdit = false;
            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);

            dbFunction.ClearListView(lvwList);
            dbFunction.ClearListView(lvwDetail);

            InitButton();
            InitSearchButton(false);
            UpdateButton(true);
            btnIRSearch.Enabled = false;
            btnTASearch.Enabled = true;
            btnTSearch.Enabled = false;
            btnSIMSearch.Enabled = false;

            dbFunction.TextBoxUnLock(false, this);
            PKTextBoxBackColor(false);
            
            InitMessageCountLimit();
            InitSerialStatusDescription();
            InitStatusTitle(true);
            InitCustomer(false);

        }
        
        private void btnSearch_Click(object sender, EventArgs e)
        {            
            dbAPI.ResetAdvanceSearch();            
            frmFindCount.fMultiSelect = false;
            frmFindCount.iCountType = frmFindCount.CountType.iTInstallationReq;
            frmFindCount.sHeader = "NEW INSTALLATION REQUEST";
            frmFindCount.isPrimary = clsFunction.iZero;
            frmFindCount.sJobTypeDescription = clsFunction.sZero;
            frmFindCount.iIRStatus = clsGlobalVariables.STATUS_AVAILABLE;
            frmFindCount.sIRStatusDescription = clsGlobalVariables.STATUS_AVAILABLE_DESC;
            frmFindCount frm = new frmFindCount();
            frm.ShowDialog();
            
            if (frm.fSelected)
            {
                if (clsSearch.ClassIRStatus != clsGlobalVariables.STATUS_AVAILABLE)
                {
                    dbFunction.SetMessageBox("Unable to include IR. Check details below" +
                                    "\n\n" +
                                    "Status: " + clsSearch.ClassIRStatusDescription + "\n" +
                                    "Request ID: " + clsSearch.ClassIRNo + "\n" +
                                    "Merchant Name: " + clsSearch.ClassParticularName + "\n" +
                                    "\n\n" +
                                    "Reason: IR Status is not AVAILABLE.", "IR Status Check", clsFunction.IconType.iError);                    
                    return;


                }
                
                LoadIR();
                txtServiceJobTypeStatusDesc.Text = clsGlobalVariables.JOB_TYPE_STATUS_PREPARATION_DESC;
                UpdateButton(false);
                InitCustomer(true);
            }
        }

        private void LoadIR()
        {
            Cursor.Current = Cursors.WaitCursor;

            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);                       

            lblSubHeader.Text = "INSTALLATION SERVICE REQUEST" + sNegative;
            
            // Load IR Detail
            txtIRIDNo.Text = clsSearch.ClassIRIDNo.ToString();
            txtSearchIRNo.Text = clsSearch.ClassIRNo;
            txtIRRequestDate.Text = clsSearch.ClassIRRequestDate;
            txtIRInstallationDate.Text = clsSearch.ClassIRInstallationDate;
            txtIRTID.Text = clsSearch.ClassTID;
            txtIRMID.Text = clsSearch.ClassMID;
            txtRegionID.Text = clsSearch.ClassRegionID.ToString();
            txtClientID.Text = clsSearch.ClassClientID.ToString();
            txtIRProcessedDateTime.Text = clsSearch.ClassProcessedDateTime;
            txtIRProcessedBy.Text = clsSearch.ClassProcessedBy;
            txtProcessType.Text = clsSearch.ClassProcessType;

            if (!fEdit && !dbFunction.isValidID(txtTAID.Text))
                txtIRSVCStatus.Text = clsGlobalVariables.JOB_TYPE_STATUS_PREPARATION_DESC;

            // Merchant Detail
            clsSearch.ClassMerchantID = clsSearch.ClassMerchantID;
            clsSearch.ClassMerchantName = clsFunction.sZero;
            PopulateMerchantTextBox();

            // Load Client                
            clsSearch.ClassClientID = clsSearch.ClassClientID;
            clsSearch.ClassClientName = clsFunction.sZero;
            PopulateClientTextBox();

            // Load SP
            clsSearch.ClassServiceProviderID = clsFunction.iZero;
            clsSearch.ClassServiceProviderName = clsFunction.sZero;            
            PopulateSPTextBox();

            // Load FE
            clsSearch.ClassFEID = clsFunction.iZero;
            clsSearch.ClassFEName = clsFunction.sZero;
            PopulateFETextBox();

            // Load Terminal
            txtTerminalID.Text = clsFunction.sZero;
            txtTerminalSN.Text = clsFunction.sZero;
            PopulateTerminalTextBox(txtTerminalID.Text, txtTerminalSN.Text);

            // Load Dock            
            txtDockID.Text = clsFunction.sZero;
            txtDockSN.Text = clsFunction.sZero;
            PopulateDockTextBox(txtDockID.Text, txtDockSN.Text);

            // Load SIM
            txtSIMID.Text = clsFunction.sZero;
            txtSIMSN.Text = clsFunction.sZero;
            PopulateSIMTextBox(txtSIMID.Text, txtSIMSN.Text);
            
            // Load Service Type
            FillServiceTypeTextBox();

            // Load Servicing
            clsSearch.ClassServiceStatus = clsFunction.iZero;
            clsSearch.ClassServiceStatusDescription = clsFunction.sZero;
            FillServicingTextBox();

            SetHoldVariable();

            dbFunction.TextBoxUnLock(false, this);
            PKTextBoxBackColor(true);
            MKTextBoxBackColor(true);
            EntryTextBox(false);
            DKTextBoxBackColor();

            InitSearchButton(true);

            InitCreatedDateTime();
            InitCreatedBy();

            btnIRSearch.Enabled = true;

            UpdateButton(false);

            InitStatusTitle(false);
            
            Cursor.Current = Cursors.Default;
        }

        private void LoadTA()
        {
            Cursor.Current = Cursors.WaitCursor;

            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);

            lblSubHeader.Text = "MODIFY INSTALLATION SERVICE REQUEST" + sNegative;

            txtServiceNo.Text = clsSearch.ClassServiceNo.ToString();

            // Load IR Detail            
            PopulateIRTextBox(clsSearch.ClassIRIDNo.ToString());
            
            // Load TA            
            PopulateTATextBox(clsSearch.ClassTAIDNo.ToString());
            
            // Load Mechant
            txtMerchantID.Text = clsSearch.ClassMerchantID.ToString();
            clsSearch.ClassMerchantName = clsFunction.sZero;
            PopulateMerchantTextBox();

            // Load Client
            txtClientID.Text = clsSearch.ClassClientID.ToString();
            clsSearch.ClassClientName = clsFunction.sZero;
            PopulateClientTextBox();

            // Load SP
            txtSPID.Text = clsSearch.ClassServiceProviderID.ToString();
            clsSearch.ClassServiceProviderName = clsFunction.sZero;
            PopulateSPTextBox();

            // Load FE
            txtFEID.Text = clsSearch.ClassFEID.ToString();
            clsSearch.ClassFEName = clsFunction.sZero;
            PopulateFETextBox();

            // Load Terminal
            txtTerminalID.Text = sOldTerminalID = clsSearch.ClassTerminalID.ToString();
            txtTerminalSN.Text = txtOldTerminalSN.Text = clsSearch.ClassTerminalSN;            
            PopulateTerminalTextBox(txtTerminalID.Text, txtTerminalSN.Text);

            // Load SIM            
            txtSIMID.Text = sOldSIMID = clsSearch.ClassSIMID.ToString();
            txtSIMSN.Text =  txtOldSIMSN.Text = clsSearch.ClassSIMSerialNo;
            PopulateSIMTextBox(txtSIMID.Text, txtSIMSN.Text);

            // Load Dock
            txtDockID.Text = sOldDockID  = clsSearch.ClassDockID.ToString();
            txtDockSN.Text = txtOldDockSN.Text = clsSearch.ClassDockSN;
            PopulateDockTextBox(txtDockID.Text, txtDockSN.Text);

            // Load Service Type
            FillServiceTypeTextBox();

            // Load Servicing
            clsSearch.ClassServiceStatus = clsFunction.iZero;
            clsSearch.ClassServiceStatusDescription = clsFunction.sZero;
            txtServiceNo.Text = clsSearch.ClassServiceNo.ToString();
            FillServicingTextBox();

            SetHoldVariable();

            dbFunction.TextBoxUnLock(true, this);
            PKTextBoxBackColor(true);
            MKTextBoxBackColor(true);
            EntryTextBox(false);
            DKTextBoxBackColor();

            fEdit = true;
            InitButton();
            InitSearchButton(true);
            UpdateButton(false);
            
            // Load Service Detail
            if (dbFunction.isValidID(txtSearchTAIDNo.Text))
            {
                lvwList.Items.Clear();
                lvwDetail.Items.Clear();
                
                LoadServicingDetail();
            }
            
            // Get SerialNo Status
            SerialNoStatusDescription();
            InitStatusTitle(false);

            Cursor.Current = Cursors.Default;
        }
        private void btnTSearch_Click(object sender, EventArgs e)
        {
            
            frmSearchField.iSearchType = frmSearchField.SearchType.iTerminal;
            frmSearchField.iStatus = clsGlobalVariables.STATUS_AVAILABLE;
            frmSearchField.sTerminalType = "View Terminal";
            frmSearchField.sHeader = "TERMINAL";
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                // Check SN status must be available
                if (clsSearch.ClassTerminalStatus != clsGlobalVariables.STATUS_AVAILABLE)
                {
                    dbFunction.SetMessageBox("Terminal SN " + clsSearch.ClassTerminalSN + " status is not AVAILABLE.", "Unable to add", clsFunction.IconType.iError);
                    return;
                }

                txtTerminalID.Text = clsSearch.ClassTerminalID.ToString();
                txtTerminalSN.Text = clsSearch.ClassTerminalSN;
                PopulateTerminalTextBox(txtTerminalID.Text, txtTerminalSN.Text);
            }            
        }
        
        private void SaveTerminalAllocation()
        {
            string sRowSQL = "";
            string sSQL = "";
            
            DateTime TADateTime = DateTime.Now;
            string sTADateTime = "";

            DateTime ProcessDateTime = DateTime.Now;
            string sProcessDateTime = "";

            DateTime ModifiedDateTime = DateTime.Now;
            string sModifiedDateTime = "";
            
            sTADateTime = TADateTime.ToString("yyyy-MM-dd H:mm:ss");
            sProcessDateTime = ProcessDateTime.ToString("yyyy-MM-dd H:mm:ss");
            sModifiedDateTime = ModifiedDateTime.ToString("yyyy-MM-dd H:mm:ss");

            sSQL = "";
            sRowSQL = "";
            sRowSQL = " ('" + dbFunction.CheckAndSetNumericValue(txtTAID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtClientID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtSPID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtFEID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + "', " +
            sRowSQL + sRowSQL + " '" + (txtSearchIRNo.Text.Length > 0 ? txtSearchIRNo.Text : "0") + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtMerchantID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtTerminalID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + txtTerminalSN.Text + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtTerminalTypeID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtTerminalModelID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtTerminalBrandID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtServiceTypeID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtOtherServiceTypeID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + sTADateTime + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassUserFullName + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassUserFullName + "', " +
            sRowSQL + sRowSQL + " '" + sProcessDateTime + "', " +
            sRowSQL + sRowSQL + " '" + sModifiedDateTime + "', " +
            sRowSQL + sRowSQL + " '" + (txtRemarks.Text.Length > 0 ? txtRemarks.Text : clsFunction.sDash) + "', " +
            sRowSQL + sRowSQL + " '" + (txtComments.Text.Length > 0 ? txtRemarks.Text : clsFunction.sDash) + "', " +
            sRowSQL + sRowSQL + " '" + (txtIRTID.Text.Length > 0 ? txtIRTID.Text : "0") + "', " +
            sRowSQL + sRowSQL + " '" + (txtIRMID.Text.Length > 0 ? txtIRMID.Text : "0") + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtSIMID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + (txtSIMSN.Text.Length > 0 ? txtSIMSN.Text : "0") + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtRegionID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + txtMerchantRegion.Text + "', " +
            sRowSQL + sRowSQL + " '" + clsGlobalVariables.STATUS_ALLOCATED + "', " +
            sRowSQL + sRowSQL + " '" + clsGlobalVariables.STATUS_ALLOCATED_DESC + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtDockID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + txtDockSN.Text + "', " +
            sRowSQL + sRowSQL + " '" + clsGlobalVariables.JOB_TYPE_STATUS_PENDING + "', " +
            sRowSQL + sRowSQL + " '" + clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC + "', " +
            sRowSQL + sRowSQL + " '" + txtServiceTypeDescription.Text + "', " +
            sRowSQL + sRowSQL + " '" + clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtRegionType.Text) + "') ";
            sSQL = sSQL + sRowSQL;

            Debug.WriteLine("TA::sSQL="+ sSQL);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "Terminal Allocation", sSQL, "InsertCollectionDetail");

            txtSearchTAIDNo.Text = clsLastID.ClassLastInsertedID.ToString();
        }

        private void SaveNotification(int iNotifType)
        {
            string sRowSQL = "";
            string sSQL = "";

            DateTime dCurDateTime = DateTime.Now;
            string sCurDateTime = "";

            int xNotifType = 0;
            string xNotifDesc = "";
            string xMaintenanceType = "";

            sCurDateTime = dCurDateTime.ToString("yyyy-MM-dd H:mm:ss");

            if (iNotifType == clsGlobalVariables.EMAIL_NOTIF_TYPE)
            {
                xNotifType = clsGlobalVariables.EMAIL_NOTIF_TYPE;
                xNotifDesc = clsGlobalVariables.EMAIL_NOTIF_TYPE_DESC;
                xMaintenanceType = "Email Notif";
            }

            if (iNotifType == clsGlobalVariables.SMS_NOTIF_TYPE)
            {
                xNotifType = clsGlobalVariables.SMS_NOTIF_TYPE;
                xNotifDesc = clsGlobalVariables.SMS_NOTIF_TYPE_DESC;
                xMaintenanceType = "SMS Notif";
            }

            sSQL = "";
            sRowSQL = "";
            sRowSQL = " ('" + sCurDateTime + "', " +
            sRowSQL + sRowSQL + " '" + xNotifType + "', " +
            sRowSQL + sRowSQL + " '" + xNotifDesc + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(clsUser.ClassUserID.ToString()) + "', " +            
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtServiceNo.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtSearchTAIDNo.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtClientID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtMerchantID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtFEID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassUserFullName + "', " +
            sRowSQL + sRowSQL + " '" + txtClientName.Text + "', " +
            sRowSQL + sRowSQL + " '" + txtMerchantName.Text + "', " +
            sRowSQL + sRowSQL + " '" + txtFEName.Text + "', " +
            sRowSQL + sRowSQL + " '" + txtSearchIRNo.Text + "', " +
            sRowSQL + sRowSQL + " '" + txtRequestNo.Text + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtTerminalSN.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtSIMSN.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtDockSN.Text) + "', " +
            sRowSQL + sRowSQL + " '" + clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC + "', " +
            sRowSQL + sRowSQL + " '" + clsGlobalVariables.FSR_REPORT_TYPE + "', " +            
            sRowSQL + sRowSQL + " '" + clsGlobalVariables.FSR_REPORT_TYPE_DESC + "') ";
            sSQL = sSQL + sRowSQL;

            Debug.WriteLine("SaveEmailNotif::sSQL=" + sSQL);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", xMaintenanceType, sSQL, "InsertMaintenanceMaster");
            
        }

        private void UpdateTerminalAllocation(string sTAIDNo)
        {
            string sTADateTime = dbFunction.GetDateFromParse(txtTADateTime.Text, "MM-dd-yyyy hh:mm:ss tt", "yyyy-MM-dd H:mm:ss");
            string sProcessedDateTime = dbFunction.GetDateFromParse(txtProcessedDate.Text, "MM-dd-yyyy hh:mm:ss tt", "yyyy-MM-dd H:mm:ss");
            string sModifiedDateTime = dbFunction.GetDateFromParse(txtModifiedDate.Text, "MM-dd-yyyy hh:mm:ss tt", "yyyy-MM-dd H:mm:ss");

            clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(sTAIDNo) + clsFunction.sPipe +
                                                dbFunction.CheckAndSetNumericValue(txtClientID.Text) + clsFunction.sPipe +
                                                dbFunction.CheckAndSetNumericValue(txtSPID.Text) + clsFunction.sPipe +
                                                dbFunction.CheckAndSetNumericValue(txtFEID.Text) + clsFunction.sPipe +
                                                dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + clsFunction.sPipe +
                                                txtSearchIRNo.Text + clsFunction.sPipe +
                                                dbFunction.CheckAndSetNumericValue(txtMerchantID.Text) + clsFunction.sPipe +
                                                dbFunction.CheckAndSetNumericValue(txtTerminalID.Text) + clsFunction.sPipe +
                                                txtTerminalSN.Text + clsFunction.sPipe +
                                                dbFunction.CheckAndSetNumericValue(txtTerminalTypeID.Text) + clsFunction.sPipe +
                                                dbFunction.CheckAndSetNumericValue(txtTerminalModelID.Text) + clsFunction.sPipe +
                                                dbFunction.CheckAndSetNumericValue(txtTerminalBrandID.Text) + clsFunction.sPipe +
                                                dbFunction.CheckAndSetNumericValue(txtServiceTypeID.Text) + clsFunction.sPipe +
                                                dbFunction.CheckAndSetNumericValue(txtOtherServiceTypeID.Text) + clsFunction.sPipe +
                                                sTADateTime + clsFunction.sPipe +
                                                txtIRProcessedBy.Text + clsFunction.sPipe +
                                                txtModifiedBy.Text + clsFunction.sPipe +
                                                sProcessedDateTime + clsFunction.sPipe +
                                                sModifiedDateTime + clsFunction.sPipe +
                                                txtRemarks.Text + clsFunction.sPipe +
                                                txtComments.Text + clsFunction.sPipe +
                                                txtIRTID.Text + clsFunction.sPipe +
                                                txtIRMID.Text + clsFunction.sPipe +
                                                dbFunction.CheckAndSetNumericValue(txtSIMID.Text) + clsFunction.sPipe +
                                                txtSIMSN.Text + clsFunction.sPipe +
                                                dbFunction.CheckAndSetNumericValue(txtRegionID.Text) + clsFunction.sPipe +
                                                txtMerchantRegion.Text + clsFunction.sPipe +
                                                dbFunction.CheckAndSetNumericValue(txtDockID.Text) + clsFunction.sPipe +
                                                txtDockSN.Text;

            Debug.WriteLine("UpdateTA::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbAPI.ExecuteAPI("PUT", "Update", "TA Detail", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");            
        }
        private void btnSave_Click(object sender, EventArgs e)
        {            
            if (!fEdit)
            {
                if (!ValidateFieldsForNegative()) return;

                if (!ValidateFields(false)) return;

                if (!dbFunction.fSavingConfirm(false)) return;

                if (!ValidateSerialNo()) return;

                Cursor.Current = Cursors.WaitCursor;

                SaveTerminalAllocation();
                
                SaveServiceDetail(false); // Save Servicing

                clsSearch.ClassStatus = clsGlobalVariables.STATUS_ALLOCATED;
                clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_ALLOCATED_DESC;

                // Update Status
                if (dbFunction.isValidID(txtIRIDNo.Text))
                    dbAPI.UpdateIRDetailStatus(txtIRIDNo.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                if (dbFunction.isValidID(txtTerminalID.Text))
                    dbAPI.UpdateTerminalDetailStatus(txtTerminalID.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                if (dbFunction.isValidID(txtSIMID.Text))
                    dbAPI.UpdateSIMDetailStatus(txtSIMID.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                if (dbFunction.isValidID(txtDockID.Text))
                    dbAPI.UpdateTerminalDetailStatus(txtDockID.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                if (dbFunction.isValidID(txtServiceNo.Text))
                    dbAPI.UpdateServiceStatus(txtServiceNo.Text, txtRequestNo.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription, clsSearch.ClassIRIDNo.ToString());

                if (dbFunction.isValidID(txtServiceNo.Text))
                    dbAPI.UpdateServiceRemarks(txtServiceNo.Text, txtRemarks.Text);

                Cursor.Current = Cursors.Default;

                MessageBox.Show("New Terminal Allocation successfully saved", "Saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }
            else
            {
                if (!ValidateFieldsForNegative()) return;

                if (!ValidateFields(false)) return;

                // Update TA already saved.
                if (dbFunction.isValidID(txtSearchTAIDNo.Text) && dbFunction.isValidID(txtIRIDNo.Text))
                {
                    if (!fConfrimUpdateTA()) return;

                    dbFunction.GetModifiedByAndDateTime(); // Get modifiedby and datetime

                    Debug.WriteLine("->>ID Comparison");
                    Debug.WriteLine("sOldTerminalID=" + sOldTerminalID + " is equal to " + "txtTerminalID.Text=" + txtTerminalID.Text);
                    Debug.WriteLine("sOldSIMID=" + sOldSIMID + " is equal to " + "txtSIMID.Text=" + txtSIMID.Text);
                    Debug.WriteLine("sOldDockID=" + sOldDockID + " is equal to " + "txtDockID.Text=" + txtDockID.Text);

                    Debug.WriteLine("->>SN Comparison");
                    Debug.WriteLine("txtTerminalSN.Text="+ txtTerminalSN.Text+ " is equal to "+ "txtOldTerminalSN.Text="+ txtOldTerminalSN.Text);
                    Debug.WriteLine("txtSIMSN.Text=" + txtSIMSN.Text + " is equal to " + "txtOldSIMSN.Text=" + txtOldSIMSN.Text);
                    Debug.WriteLine("txtDockSN.Text=" + txtDockSN.Text + " is equal to " + "txtOldDockSN.Text=" + txtOldDockSN.Text);

                    Debug.WriteLine("GetModifiedByAndDateTime");
                    Debug.WriteLine("clsUser.ClassModifiedBy=" + clsUser.ClassModifiedBy);
                    Debug.WriteLine("clsUser.ClassModifiedDateTime=" + clsUser.ClassModifiedDateTime);

                    // Update Terminal Detail
                    if (!txtTerminalSN.Text.Equals(txtOldTerminalSN.Text))
                    {
                        dbAPI.UpdateTerminalDetailStatus(sOldTerminalID, clsGlobalVariables.STATUS_AVAILABLE, clsGlobalVariables.STATUS_AVAILABLE_DESC); // Update old terminal id
                        dbAPI.UpdateTerminalDetailStatus(txtTerminalID.Text, clsGlobalVariables.STATUS_ALLOCATED, clsGlobalVariables.STATUS_ALLOCATED_DESC); // Update new terminal id
                    }

                    // Update SIM Detail
                    if (!txtSIMSN.Text.Equals(txtOldSIMSN.Text))
                    {
                        dbAPI.UpdateSIMDetailStatus(sOldSIMID, clsGlobalVariables.STATUS_AVAILABLE, clsGlobalVariables.STATUS_AVAILABLE_DESC);  // Update old sim id
                        dbAPI.UpdateSIMDetailStatus(txtSIMID.Text, clsGlobalVariables.STATUS_ALLOCATED, clsGlobalVariables.STATUS_ALLOCATED_DESC);  // Update new sim id
                    }

                    // Update Dock Detail
                    if (!txtDockSN.Text.Equals(txtOldDockSN.Text))
                    {
                        dbAPI.UpdateTerminalDetailStatus(sOldDockID, clsGlobalVariables.STATUS_AVAILABLE, clsGlobalVariables.STATUS_AVAILABLE_DESC); // Update old dock id
                        dbAPI.UpdateTerminalDetailStatus(txtDockID.Text, clsGlobalVariables.STATUS_ALLOCATED, clsGlobalVariables.STATUS_ALLOCATED_DESC); // Update new dock id
                    }
                    
                    // Update Terminal Allocation
                    if (dbFunction.isValidID(txtSearchTAIDNo.Text))
                    {
                        dbAPI.UpdateTARemarks(txtSearchTAIDNo.Text, txtRemarks.Text, txtComments.Text); // Update TA Remarks/Comments
                        dbAPI.UpdateTAFESP(txtSearchTAIDNo.Text, txtFEID.Text, txtSPID.Text); // Update TA FE and SP
                    }

                    // Update Terminal Allocation
                    if ((!txtTerminalSN.Text.Equals(txtOldTerminalSN.Text)) || (!txtSIMSN.Text.Equals(txtOldSIMSN.Text)) || (dbFunction.isValidID(txtSearchTAIDNo.Text)))
                    {
                        dbAPI.UpdateTAReplacement(txtSearchTAIDNo.Text, txtTerminalID.Text, txtSIMID.Text, txtDockID.Text,
                                                  txtTerminalTypeID.Text, txtTerminalModelID.Text, txtTerminalBrandID.Text,
                                                  txtTerminalSN.Text, txtSIMSN.Text, txtDockSN.Text);
                    }

                    // Update servicing status
                    dbAPI.UpdateServiceStatus2(txtServiceNo.Text, txtRequestNo.Text, clsGlobalVariables.STATUS_ALLOCATED, clsGlobalVariables.STATUS_ALLOCATED_DESC, clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC);

                    // Update terminal allocation status
                    dbAPI.UpdateTADetailStatus2(txtSearchTAIDNo.Text, clsGlobalVariables.STATUS_ALLOCATED, clsGlobalVariables.STATUS_ALLOCATED_DESC, clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC);

                    // Update Modified By
                    dbAPI.UpdateModifiedBy("TA", txtSearchTAIDNo.Text, clsUser.ClassModifiedBy, clsUser.ClassModifiedDateTime);


                    dbFunction.SetMessageBox("Terminal Allocation has been successfully updated.", "Edited", clsFunction.IconType.iInformation);
                }
                else
                {
                    if (!dbFunction.fSavingConfirm(true)) return;

                    UpdateTerminalAllocation(txtSearchTAIDNo.Text);

                    if (dbFunction.isValidID(txtServiceNo.Text))
                        dbAPI.UpdateServiceRemarks(txtServiceNo.Text, txtRemarks.Text);

                    // Update Hold Variable
                    if (dbFunction.isValidID(sHoldTerminalID))
                        dbAPI.UpdateTerminalDetailStatus(sHoldTerminalID, clsGlobalVariables.STATUS_AVAILABLE, clsGlobalVariables.STATUS_AVAILABLE_DESC);

                    if (dbFunction.isValidID(sHoldSIMID))
                        dbAPI.UpdateSIMDetailStatus(sHoldSIMID, clsGlobalVariables.STATUS_AVAILABLE, clsGlobalVariables.STATUS_AVAILABLE_DESC);

                    if (dbFunction.isValidID(sHoldDockID))
                        dbAPI.UpdateTerminalDetailStatus(sHoldDockID, clsGlobalVariables.STATUS_AVAILABLE, clsGlobalVariables.STATUS_AVAILABLE_DESC);

                    if (clsSearch.ClassIRStatus == clsGlobalVariables.STATUS_NEGATIVE || clsSearch.ClassStatus == clsGlobalVariables.STATUS_NEGATIVE)
                    {
                        SaveServiceDetail(false); // Save Servicing

                        // Update Status
                        clsSearch.ClassStatus = clsGlobalVariables.STATUS_ALLOCATED;
                        clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_ALLOCATED_DESC;

                        if (dbFunction.isValidID(txtIRIDNo.Text))
                            dbAPI.UpdateIRDetailStatus(txtIRIDNo.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                        if (dbFunction.isValidID(txtTerminalID.Text))
                            dbAPI.UpdateTerminalDetailStatus(txtTerminalID.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                        if (dbFunction.isValidID(txtSIMID.Text))
                            dbAPI.UpdateSIMDetailStatus(txtSIMID.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                        if (dbFunction.isValidID(txtDockID.Text))
                            dbAPI.UpdateTerminalDetailStatus(txtDockID.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                        clsSearch.ClassStatus = clsGlobalVariables.STATUS_NEGATIVE;
                        clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_NEGATIVE_DESC;

                        if (dbFunction.isValidID(txtServiceNo.Text))
                            dbAPI.UpdateServiceStatus(txtServiceNo.Text, txtRequestNo.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription, clsSearch.ClassIRIDNo.ToString());

                        if (dbFunction.isValidID(sHoldServiceNo))
                            dbAPI.UpdateServiceJobType(sHoldServiceNo, sHoldRequestNo, clsGlobalVariables.JOB_TYPE_STATUS_PENDING, clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC, clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC, clsGlobalVariables.TA_STATUS_INSTALLED_SUB_DESC);

                        dbFunction.SetMessageBox("Negative Terminal Allocation has been successfully updated.", "Edited", clsFunction.IconType.iInformation);
                    }
                    else
                    {
                        dbFunction.SetMessageBox("Terminal Allocation has been successfully updated.", "Edited", clsFunction.IconType.iInformation);
                    }
                }
                               
            }
            
            btnClear_Click(this, e);
        }

        private bool ValidateFields(bool isDispatch)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iClientID, txtClientID.Text)) return false;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iSPID, txtSPID.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iSPID, txtSPName.Text)) return false;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iIRNo, txtSearchIRNo.Text)) return false;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantID, txtMerchantID.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantID, txtMerchantName.Text)) return false;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalID, txtTerminalID.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalID, txtTerminalSN.Text)) return false;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iServiceType, txtServiceTypeID.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iServiceType, txtServiceTypeDescription.Text)) return false;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRegion, txtRegionID.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRegion, txtMerchantRegion.Text)) return false;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iAppVersion, txtAppVersion.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iAppCRC, txtAppCRC.Text)) return false;

            if (isDispatch)
            {
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iFEID, txtFEID.Text)) return false;

                if (!dbFunction.isValidEntry(clsFunction.CheckType.iFEID, txtFEName.Text)) return false;

                if (!dbFunction.isValidEntry(clsFunction.CheckType.iSPID, txtSPID.Text)) return false;

                if (!dbFunction.isValidEntry(clsFunction.CheckType.iSPID, txtSPName.Text)) return false;
            }
                
            
            return true;
        }

        private bool ValidateFieldsForNegative()
        {
            if (clsSearch.ClassIRStatus == clsGlobalVariables.STATUS_NEGATIVE || clsSearch.ClassStatus == clsGlobalVariables.STATUS_NEGATIVE)
            {
                if (lblTerminalSNStatus.Text.CompareTo(clsFunction.sDash) != 0 && lblTerminalSNStatus.Text.CompareTo(clsGlobalVariables.STATUS_AVAILABLE_DESC) != 0)
                {                    
                    dbFunction.SetMessageBox("Terminal Serial Number must be available", "Check Field", clsFunction.IconType.iExclamation);
                }

                if (lblDockSNStatus.Text.CompareTo(clsFunction.sDash) != 0 && lblDockSNStatus.Text.CompareTo(clsGlobalVariables.STATUS_AVAILABLE_DESC) != 0)
                {
                    dbFunction.SetMessageBox("Dock Serial Number must be available", "Check Field", clsFunction.IconType.iExclamation);
                }

                if (lblSIMSNStatus.Text.CompareTo(clsFunction.sDash) != 0 && lblSIMSNStatus.Text.CompareTo(clsGlobalVariables.STATUS_AVAILABLE_DESC) != 0)
                {
                    dbFunction.SetMessageBox("SIM Serial Number must be available", "Check Field", clsFunction.IconType.iExclamation);
                }

            }

            return true;
        }

        private bool ValidateSerialNo()
        {
            if (dbFunction.isValidID(txtTerminalID.Text))
            {
                if (!dbAPI.isRecordExist("Search", "Terminal Available Check", txtTerminalSN.Text))
                {
                    dbFunction.SetMessageBox("Terminal Serial Number " + txtTerminalSN.Text + " already allocated.\nPlease select again.", "Already Allocated", clsFunction.IconType.iExclamation);

                    return false;
                }
            }
            
            if (dbFunction.isValidID(txtSIMSN.Text))
            {
                if (!dbAPI.isRecordExist("Search", "SIM Available Check", txtSIMSN.Text))
                {
                    dbFunction.SetMessageBox("SIM Serial Number " + txtSIMSN.Text + " already allocated.\nPlease select again.", "Already Allocated", clsFunction.IconType.iExclamation);

                    return false;
                }
            }

            if (dbFunction.isValidID(txtDockID.Text))
            {
                if (!dbAPI.isRecordExist("Search", "Terminal Available Check", txtDockSN.Text))
                {
                    dbFunction.SetMessageBox("Dock Serial Number " + txtDockSN.Text + " already allocated.\nPlease select again.", "Already Allocated", clsFunction.IconType.iExclamation);

                    return false;
                }
            }

            return true;
        }
        
        private void PopulateMerchantTextBox()
        {            
            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassMerchantID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassMerchantName;

            Debug.WriteLine("PopulateMerchantTextBox::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbAPI.ExecuteAPI("GET", "Search", "Particular Detail", clsSearch.ClassAdvanceSearchValue, "Particular Detail", "", "ViewParticularDetail");

            if ((clsSearch.ClassMerchantID.ToString().CompareTo(clsFunction.sZero) == 0) && (clsSearch.ClassMerchantName.CompareTo(clsFunction.sZero) == 0))
            {
                txtMerchantID.Text = clsFunction.sZero;
                txtRegionID.Text = "";
                txtRegionType.Text = "";
                txtMerchantName.Text = "";
                txtMerchantAddress1.Text = "";
                txtMerchantRegion.Text = "";
                txtMerchantProvince.Text = "";
                txtMerchantContactPerson.Text = "";
                txtMerchantTelNo.Text = "";
                txtMerchantMobile.Text = "";

                return;
            }

            if (dbAPI.isNoRecordFound() == false)
            {
                clsParticular.RecordFound = true;
                txtMerchantID.Text = clsParticular.ClassParticularID.ToString();
                txtRegionID.Text = clsParticular.ClassRegionID.ToString();
                txtRegionType.Text = clsParticular.ClassRegionType.ToString();
                txtMerchantName.Text = clsParticular.ClassParticularName;
                txtMerchantAddress1.Text = clsParticular.ClassAddress;
                txtMerchantRegion.Text = clsParticular.ClassRegion;
                txtMerchantProvince.Text = clsParticular.ClassProvince;
                txtMerchantContactPerson.Text = clsParticular.ClassContactPerson;
                txtMerchantTelNo.Text = clsParticular.ClassTelNo;
                txtMerchantMobile.Text = clsParticular.ClassMobile;
            }
            else
            {
                clsParticular.RecordFound = false;
            }
        }
       
        private void PopulateTerminalTextBox(string sTerminalID, string sTerminalSN)
        {
            if (sTerminalID.CompareTo(clsFunction.sZero) == 0)
            {
                txtTerminalID.Text = clsFunction.sZero;
                txtTerminalTypeID.Text = "";
                txtTerminalModelID.Text = "";
                txtTerminalBrandID.Text = "";
                txtTerminalSN.Text = "";
                txtTerminalType.Text = "";
                txtTerminalModel.Text = "";
                txtTerminalBrand.Text = "";

                return;
            }

            dbAPI.GetTerminalInfo(sTerminalID, sTerminalSN);

            txtTerminalID.Text = clsTerminal.ClassTerminalID.ToString();
            txtTerminalTypeID.Text = clsTerminal.ClassTerminalTypeID.ToString();
            txtTerminalModelID.Text = clsTerminal.ClassTerminalModelID.ToString();
            txtTerminalBrandID.Text = clsTerminal.ClassTerminalBrandID.ToString();
            txtTerminalSN.Text = clsTerminal.ClassTerminalSN;
            txtTerminalType.Text = clsTerminal.ClassTerminalType;
            txtTerminalModel.Text = clsTerminal.ClassTerminalModel;
            txtTerminalBrand.Text = clsTerminal.ClassTerminalBrand;
        }

        private void PopulateSIMTextBox(string sSIMID, string sSIMSN)
        {
            if (sSIMID.CompareTo(clsFunction.sZero) == 0)
            {
                txtSIMID.Text = clsFunction.sZero;
                txtSIMSN.Text = "";
                txtSIMCarrier.Text = "";

                return;
            }

            dbAPI.GetSIMInfo(sSIMID, sSIMSN);

            txtSIMID.Text = clsSIM.ClassSIMID.ToString();
            txtSIMSN.Text = clsSIM.ClassSIMSN;
            txtSIMCarrier.Text = clsSIM.ClassSIMCarrier;
        }

        private void PopulateDockTextBox(string sDockID, string sDockSN)
        {
            if (sDockID.ToString().CompareTo(clsFunction.sZero) == 0)
            {
                txtDockID.Text = clsFunction.sZero;
                txtDockSN.Text = "";

                return;
            }

            dbAPI.GetDockInfo(sDockID, sDockSN);

            txtDockID.Text = clsSearch.ClassDockID.ToString();
            txtDockSN.Text = clsSearch.ClassDockSN;            
        }

        private void PopulateClientTextBox()
        {            
            
            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassClientID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassClientName;

            Debug.WriteLine("PopulateClientTextBox::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            if ((clsSearch.ClassClientID.ToString().CompareTo(clsFunction.sZero) == 0) && (clsSearch.ClassClientName.CompareTo(clsFunction.sZero) == 0))
            {
                txtClientID.Text = clsFunction.sZero;
                txtClientName.Text = "";
                txtClientAddress.Text = "";
                txtClientContactPerson.Text = "";
                txtClientTelNo.Text = "";
                txtClientMobile.Text = "";

                return;
            }

            dbAPI.ExecuteAPI("GET", "Search", "Particular Detail", clsSearch.ClassAdvanceSearchValue, "Particular", "", "ViewParticularDetail");

            if (dbAPI.isNoRecordFound() == false)
            {
                clsParticular.RecordFound = true;

                txtClientID.Text = clsParticular.ClassParticularID.ToString();
                txtClientName.Text = clsParticular.ClassParticularName;
                txtClientAddress.Text = clsParticular.ClassAddress;
                txtClientContactPerson.Text = clsParticular.ClassContactPerson;
                txtClientTelNo.Text = clsParticular.ClassTelNo;
                txtClientMobile.Text = clsParticular.ClassMobile;
            }
            else
            {
                clsParticular.RecordFound = false;
            }
        }

        private void PopulateSPTextBox()
        {
            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassServiceProviderID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassServiceProviderName;

            Debug.WriteLine("FillSPTextBox::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            if (clsSearch.ClassServiceProviderID.ToString().CompareTo(clsFunction.sZero) == 0 && clsSearch.ClassServiceProviderName.CompareTo(clsFunction.sZero) == 0)
            {
                txtSPID.Text = clsFunction.sZero;
                txtSPName.Text = "";
                txtSPAddress.Text = "";
                txtSPContactPerson.Text = "";
                txtSPTelNo.Text = "";
                txtSPMobile.Text = "";

                return;
            }

            dbAPI.ExecuteAPI("GET", "Search", "Particular Detail", clsSearch.ClassAdvanceSearchValue, "Particular Detail", "", "ViewParticularDetail");

            if (dbAPI.isNoRecordFound() == false)
            {
                clsParticular.RecordFound = true;
                txtSPID.Text = clsParticular.ClassParticularID.ToString();
                txtSPName.Text = clsParticular.ClassParticularName;
                txtSPAddress.Text = clsParticular.ClassAddress;
                txtSPContactPerson.Text = clsParticular.ClassContactPerson;
                txtSPTelNo.Text = clsParticular.ClassTelNo;
                txtSPMobile.Text = clsParticular.ClassMobile;
            }
            else
            {
                clsParticular.RecordFound = false;
            }
           
        }
        private void PopulateFETextBox()
        {            
            
            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassFEID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassFEName;

            Debug.WriteLine("FillFETextBox::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            if (clsSearch.ClassFEID.ToString().CompareTo(clsFunction.sZero) == 0 && clsSearch.ClassFEName.CompareTo(clsFunction.sZero) == 0)
            {
                txtFEID.Text = clsFunction.sZero;
                txtFEName.Text = "";
                txtFEAddress.Text = "";
                txtFETelNo.Text = "";
                txtFEMobile.Text = "";

                return;
            }

            dbAPI.ExecuteAPI("GET", "Search", "Particular Detail", clsSearch.ClassAdvanceSearchValue, "Particular Detail", "", "ViewParticularDetail");

            if (dbAPI.isNoRecordFound() == false)
            {
                clsParticular.RecordFound = true;
                txtFEID.Text = clsParticular.ClassParticularID.ToString();
                txtFEName.Text = clsParticular.ClassParticularName;
                txtFEAddress.Text = clsParticular.ClassAddress;
                txtFETelNo.Text = clsParticular.ClassTelNo;
                txtFEMobile.Text = clsParticular.ClassMobile;
            }
            else
            {
                clsParticular.RecordFound = false;
            }                
        }
        private void FillServiceTypeTextBox()
        {
            clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsGlobalVariables.TA_STATUS_INSTALLED_SUB_DESC + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero;
                                                
            dbAPI.GetServiceTypeInfo();
            txtServiceTypeID.Text = clsSearch.ClassServiceTypeID.ToString();
            txtServiceTypeDescription.Text = clsSearch.ClassServiceTypeDescription;

        }

        private void FillServicingTextBox()
        {
            if (dbFunction.isValidID(txtServiceNo.Text))
                dbAPI.GetServicingInfo(txtServiceNo.Text);

            txtServiceNo.Text = clsServicingDetail.ClassServiceNo.ToString();
            txtRequestNo.Text = clsServicingDetail.ClassRequestNo;
            txtJobTypeDescription.Text = clsServicingDetail.ClassJobTypeDescription;
            txtServiceJobTypeStatusDesc.Text = clsServicingDetail.ClassJobTypeStatusDescription;

        }

        private void PopulateIRTextBox(string sIRIDNo)
        {
            int i = 0;
            clsSearch.ClassAdvanceSearchValue = sIRIDNo;

            Debug.WriteLine("LoadIR::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbFunction.GetRequestTime("Find IR");

            dbAPI.ExecuteAPI("GET", "View", "IR Detail", clsSearch.ClassAdvanceSearchValue, "IR", "", "ViewAdvanceIR");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.IRIDNo.Length > i)
                {
                    txtIRIDNo.Text = clsArray.IRIDNo[i];
                    txtSearchIRNo.Text = clsArray.IRNo[i];
                    txtIRRequestDate.Text = clsArray.IRDate[i];
                    txtIRInstallationDate.Text = clsArray.InstallationDate[i];
                    txtIRTID.Text = clsArray.TID[i];
                    txtIRMID.Text = clsArray.MID[i];
                    txtRegionID.Text = clsArray.RegionID[i];
                    txtClientID.Text = clsArray.ClientID[i];
                    txtIRProcessedDateTime.Text = clsArray.ProcessedDateTime[i];
                    txtIRProcessedBy.Text = clsArray.ProcessedBy[i];
                    txtProcessType.Text = clsArray.ProcessType[i];
                    txtAppVersion.Text = clsArray.AppVersion[i];
                    txtAppCRC.Text = clsArray.AppCRC[i];

                    i++;

                }                
            }
           
            dbFunction.GetResponseTime("Find IR");
            
        }

        private void PopulateTATextBox(string sTAIDNo)
        {
            int i = 0;
            clsSearch.ClassAdvanceSearchValue = sTAIDNo;

            Debug.WriteLine("LoadTA::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbFunction.GetRequestTime("Find TA");

            dbAPI.ExecuteAPI("GET", "View", "TA Detail", clsSearch.ClassAdvanceSearchValue, "TA", "", "ViewAdvanceTA");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.TAIDNo.Length > i)
                {
                    txtSPID.Text = clsArray.ServiceProviderID[i];
                    txtSearchTAIDNo.Text = clsArray.TAIDNo[i];
                    txtTADateTime.Text = clsArray.TADateTime[i];
                    txtTAModifiedDateTime.Text = clsArray.TAModifiedDateTime[i];
                    txtIRSVCStatus.Text = clsArray.IRStatusDescription[i];
                    txtProcessedBy.Text = clsArray.TAProcessedBy[i];
                    txtModifiedBy.Text = clsArray.TAModifiedBy[i];
                    txtProcessedDate.Text = clsArray.TAProcessedDateTime[i];
                    txtModifiedDate.Text = clsArray.TAModifiedDateTime[i];
                    txtRemarks.Text = clsArray.TARemarks[i];
                    txtComments.Text = clsArray.TAComments[i];

                    i++;

                }
            }

            dbFunction.GetResponseTime("Find TA");
            
        }

        private void bunifuCards1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnTASearch_Click(object sender, EventArgs e)
        {            
            frmFindCount.sHeader = "TERMINAL ALLOCATION";
            frmFindCount.iCountType = frmFindCount.CountType.iSearchTA;
            frmFindCount.isPrimary = clsGlobalVariables.SERVICE_PRIMARY;
            frmFindCount frm = new frmFindCount();
            frm.ShowDialog();

            if (frm.fSelected)
            {
                Cursor.Current = Cursors.WaitCursor;

                fEdit = true;

                LoadTA();
                InitCustomer(true);

                Cursor.Current = Cursors.Default;
            }
        }
        
        private void PKTextBoxBackColor(bool isLock)
        {
            if (isLock)
            {                
                txtMerchantName.BackColor = clsFunction.PKBackColor;                
                txtIRTID.BackColor = clsFunction.PKBackColor;
                txtIRMID.BackColor = clsFunction.PKBackColor;                                                
                txtClientName.BackColor = clsFunction.PKBackColor;                                                
            }
        }

        private void MKTextBoxBackColor(bool isLock)
        {
            if (isLock)
            {
                txtSearchTAIDNo.BackColor = clsFunction.SearchBackColor;
                txtServiceNo.BackColor = clsFunction.SearchBackColor;
                txtIRIDNo.BackColor = clsFunction.SearchBackColor;
                txtSearchIRNo.BackColor = clsFunction.MKBackColor;
                txtServiceTypeDescription.BackColor = clsFunction.MKBackColor;
                txtProcessType.BackColor = clsFunction.MKBackColor;
                txtRequestNo.BackColor = clsFunction.MKBackColor;
                txtJobTypeDescription.BackColor = clsFunction.MKBackColor;
                txtServiceTypeDescription.BackColor = clsFunction.MKBackColor;
                txtServiceJobTypeStatusDesc.BackColor = clsFunction.StatusBackColor;                
                txtIRSVCStatus.BackColor = clsFunction.MKBackColor;

                txtOldTerminalSN.BackColor = clsFunction.DisableBackColor;
                txtOldSIMSN.BackColor = clsFunction.DisableBackColor;
                txtOldDockSN.BackColor = clsFunction.DisableBackColor;
            }
        }

        private void EntryTextBox(bool isLock)
        {
            if (!isLock)
            {
                txtMerchantProvince.BackColor = clsFunction.EntryBackColor;
                txtSPName.BackColor = clsFunction.EntryBackColor;
                txtFEName.BackColor = clsFunction.EntryBackColor;
                txtRemarks.BackColor = clsFunction.EntryBackColor;
                txtComments.BackColor = clsFunction.EntryBackColor;
                txtTerminalSN.BackColor = clsFunction.EntryBackColor;
                txtSIMSN.BackColor = clsFunction.EntryBackColor;
                txtDockSN.BackColor = clsFunction.EntryBackColor;
                txtMerchantProvince.ReadOnly = isLock;
                txtSPName.ReadOnly = isLock;
                txtFEName.ReadOnly = isLock;
                txtRemarks.ReadOnly = isLock;
                txtComments.ReadOnly = isLock;
                txtTerminalSN.ReadOnly = true;
                txtSIMSN.ReadOnly = true;
                txtDockSN.ReadOnly = true;

                txtAppVersion.BackColor = clsFunction.EntryBackColor;
                txtAppVersion.ReadOnly = false;
                txtAppCRC.BackColor = clsFunction.EntryBackColor;
                txtAppCRC.ReadOnly = false;
            }
        }

        private void DKTextBoxBackColor()
        {
            txtProcessedDate.BackColor = clsFunction.DateBackColor;
            txtModifiedDate.BackColor = clsFunction.DateBackColor;
            txtIRRequestDate.BackColor = clsFunction.DateBackColor;
            txtIRInstallationDate.BackColor = clsFunction.DateBackColor;
            txtIRProcessedDateTime.BackColor = clsFunction.DateBackColor;
            txtTADateTime.BackColor = clsFunction.DateBackColor;
            txtTAModifiedDateTime.BackColor = clsFunction.DateBackColor;            
        }
        private void btnAddClient_Click(object sender, EventArgs e)
        {
            frmParticular.iParticularType = clsGlobalVariables.iClient_Type;
            frmParticular frm = new frmParticular();
            frm.ShowDialog();
            
        }

        private void btnAddSP_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iSP;
            frmSearchField.sHeader = "SERVICE PROVIDER";
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                txtSPID.Text = clsSearch.ClassParticularID.ToString();
                txtSPName.Text = clsSearch.ClassParticularName;
                txtSPAddress.Text = clsSearch.ClassParticularAddress;
                txtSPContactPerson.Text = clsSearch.ClassParticularContactPerson;
                txtSPTelNo.Text = clsSearch.ClassParticularTelNo;
                txtSPMobile.Text = clsSearch.ClassParticularMobileNo;
            }

        }

        private void btnFE_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iFE;
            frmSearchField.sHeader = "FIELD ENGINEER";
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                txtFEID.Text = clsSearch.ClassParticularID.ToString();
                txtFEName.Text = clsSearch.ClassParticularName;
                txtFEAddress.Text = clsSearch.ClassParticularAddress;
                txtFETelNo.Text = clsSearch.ClassParticularTelNo;
                txtFEMobile.Text = clsSearch.ClassParticularMobileNo;
            }
        }
        
        private void btnSIMSearch_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iSIM;
            frmSearchField.iStatus = clsGlobalVariables.STATUS_AVAILABLE;
            frmSearchField.sHeader = "SIM";
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                // Check SN status must be available
                if (clsSearch.ClassSIMStatus != clsGlobalVariables.STATUS_AVAILABLE)
                {
                    dbFunction.SetMessageBox("SIM SN " + clsSearch.ClassSIMSerialNo + " status is not AVAILABLE.", "Unable to add", clsFunction.IconType.iError);
                    return;
                }

                txtSIMID.Text = clsSearch.ClassSIMID.ToString();
                txtSIMSN.Text = clsSearch.ClassSIMSerialNo;
                PopulateSIMTextBox(txtSIMID.Text, txtSIMSN.Text);
            }
            
        }

        private void cboRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            //FillRegionTextBox();
        }

        private void btnRemoveSIM_Click(object sender, EventArgs e)
        {
            txtSIMID.Text = clsFunction.sZero;
            txtSIMSN.Text = "";
            txtSIMCarrier.Text = "";
        }

        private void btnAddSIM_Click(object sender, EventArgs e)
        {
            frmImportSIM frm = new frmImportSIM();
            frm.ShowDialog();
        }

        private void frmTerminalAllocation_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void btnPreviewFSR_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to preview FSR?", "Confirm?", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button1) == DialogResult.No)
                return;

            dbFunction.PreviewTA(txtSearchIRNo.Text, clsFunction.iZero, clsFunction.iZero);

        }

        private void btnRemoveFE_Click(object sender, EventArgs e)
        {
            txtFEID.Text = clsFunction.sZero;
            txtFEName.Text = "";
            txtFEAddress.Text = "";
            txtFEMobile.Text = "";
            txtFETelNo.Text = "";
        }

        private void txtIRNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnProvinceSearch_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iProvince;
            frmSearchField.sHeader = "CITY";
            frmSearchField.sSearchChar = txtMerchantProvince.Text;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                txtRegionID.Text = clsSearch.ClassRegionID.ToString();
                txtRegionType.Text = clsSearch.ClassRegionType.ToString();
                txtMerchantProvince.Text = clsSearch.ClassProvince;
                txtMerchantRegion.Text = clsSearch.ClassRegion;
            }
        }

        private void btnAddProvince_Click(object sender, EventArgs e)
        {
            frmRegionDetail frm = new frmRegionDetail();
            frm.ShowDialog();            
        }

        private void pnlHeader_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnDockSearch_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iTerminal;
            frmSearchField.iStatus = clsGlobalVariables.STATUS_AVAILABLE;
            frmSearchField.sTerminalType = "View Dock";
            frmSearchField.sHeader = "DOCK";
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                // Check SN status must be available
                if (clsSearch.ClassTerminalStatus != clsGlobalVariables.STATUS_AVAILABLE)
                {
                    dbFunction.SetMessageBox("Dock SN " + clsSearch.ClassTerminalSN + " status is not AVAILABLE.", "Unable to add", clsFunction.IconType.iError);
                    return;
                }

                txtDockID.Text = clsSearch.ClassTerminalID.ToString();                
                txtDockSN.Text = clsSearch.ClassTerminalSN;               
                PopulateDockTextBox(txtDockID.Text, txtDockSN.Text);
            }
        }

        private void btnRemoveDock_Click(object sender, EventArgs e)
        {
            txtDockID.Text = clsFunction.sZero;
            txtDockSN.Text = "";
        }

        private void btnAddDock_Click(object sender, EventArgs e)
        {
            frmImportTerminal.iTab = 0;
            frmImportTerminal.iTabSub = 1;
            frmImportTerminal frm = new frmImportTerminal();
            frm.ShowDialog();
        }

        private void groupBox8_Enter(object sender, EventArgs e)
        {

        }

        private void btnRemoveTerminal_Click(object sender, EventArgs e)
        {
            txtTerminalID.Text = clsFunction.sZero;
            txtTerminalSN.Text = "";
            txtTerminalType.Text = "";
            txtTerminalModel.Text = "";
            txtTerminalBrand.Text = "";
            txtTerminalPartNo.Text = "";            
        }

        private void txtSIMCarrier_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSIMSerialNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void InitSearchButton(bool fEnable)
        {            
            btnAddSP.Enabled = fEnable;
            btnAddFE.Enabled = fEnable;
            btnRemoveFE.Enabled = fEnable;
            btnProvinceSearch.Enabled = fEnable;
            btnTASearch.Enabled = fEnable;
            btnTSearch.Enabled = fEnable;
            btnTRemove.Enabled = fEnable;
            btnSIMSearch.Enabled = fEnable;
            btnSIMRemove.Enabled = fEnable;
            btnDockSearch.Enabled = fEnable;
            btnDockRemove.Enabled = fEnable;
        }

        private void txtTerminalSN_Click(object sender, EventArgs e)
        {
            //btnTSearch_Click(this, e);
        }

        private void txtDockSerialNo_Click(object sender, EventArgs e)
        {
            //btnDockSearch_Click(this, e);
        }

        private void txtSIMSerialNo_Click(object sender, EventArgs e)
        {
            //btnSIMSearch_Click(this, e);
        }
        private void InitMessageCountLimit()
        {
            lblCountRemarks.Text = txtRemarks.TextLength.ToString() + "/" + iLimit.ToString();
            lblCountComments.Text = txtComments.TextLength.ToString() + "/" + iLimit.ToString();

            txtRemarks.MaxLength = iLimit;
            txtComments.MaxLength = iLimit;
        }

        private void txtRemarks_TextChanged(object sender, EventArgs e)
        {
            lblCountRemarks.Text = txtRemarks.Text.Length.ToString() + "/" + iLimit.ToString();
        }

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            lblCountComments.Text = txtComments.Text.Length.ToString() + "/" + iLimit.ToString();
        }

        private void btnRemoveSP_Click(object sender, EventArgs e)
        {
            txtSPID.Text = clsFunction.sZero;
            txtSPName.Text = "";
            txtSPContactPerson.Text = "";
            txtSPTelNo.Text = "";
            txtSPMobile.Text = "";
        }

        private void InitCreatedDateTime()
        {
            DateTime dDateTime = DateTime.Now;
            string sDateTime = "";

            sDateTime = dDateTime.ToString("yyyy-MM-dd H:mm:ss");
            

            txtProcessedDate.Text = sDateTime;
            txtModifiedDate.Text = sDateTime;
            
        }

        private void InitCreatedBy()
        {
            txtProcessedBy.Text = clsUser.ClassUserFullName;
            txtModifiedBy.Text = clsUser.ClassUserFullName;
        }

        private void txtSPName_Click(object sender, EventArgs e)
        {
            btnAddSP_Click(this, e);
        }

        private void txtFEName_Click(object sender, EventArgs e)
        {
            btnFE_Click(this, e);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!fDetailConfirm(true)) return;

            if (dbFunction.isValidID(txtIRIDNo.Text))            
                dbAPI.DeleteIRDetail(txtIRIDNo.Text, txtSearchIRNo.Text); // Delete from (tblirdetail)

            if (dbFunction.isValidID(txtSearchTAIDNo.Text))
                dbAPI.DeleteTADetail(txtSearchTAIDNo.Text); // Delete from (tblterminalallocation)

            if (dbFunction.isValidID(txtServiceNo.Text))
                dbAPI.DeleteServicingDetail(txtServiceNo.Text); // Delete from (tblservicingdetail)

            clsSearch.ClassStatus = clsGlobalVariables.STATUS_AVAILABLE;
            clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_AVAILABLE_DESC;

            if (dbFunction.isValidID(txtTerminalID.Text))
                dbAPI.UpdateTerminalDetailStatus(txtTerminalID.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription); // Update Terminal Detail

            if (dbFunction.isValidID(txtDockID.Text))
                dbAPI.UpdateTerminalDetailStatus(txtDockID.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription); // Update Dock Detail

            if (dbFunction.isValidID(txtSIMID.Text))
                dbAPI.UpdateSIMDetailStatus(txtSIMID.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);         // Update SIM Detail 

            dbFunction.SetMessageBox("Installation request successfully deleted.", "Deleted", clsFunction.IconType.iInformation);

            btnClear_Click(this, e);
        }

        private void btnDispatch_Click(object sender, EventArgs e)
        {            
            if (!ValidateFieldsForNegative()) return;

            if (!ValidateFields(true)) return;

            // Update TA already saved.
            if (dbFunction.isValidID(txtSearchTAIDNo.Text) && dbFunction.isValidID(txtIRIDNo.Text))
            {
                if (!fConfrimUpdateTA()) return;

                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                dbFunction.GetProcessedByAndDateTime(); // Get modifiedby and datetime
                dbFunction.GetModifiedByAndDateTime(); // Get modifiedby and datetime

                Debug.WriteLine("->>ID Comparison");
                Debug.WriteLine("sOldTerminalID=" + sOldTerminalID + " is equal to " + "txtTerminalID.Text=" + txtTerminalID.Text);
                Debug.WriteLine("sOldSIMID=" + sOldSIMID + " is equal to " + "txtSIMID.Text=" + txtSIMID.Text);
                Debug.WriteLine("sOldDockID=" + sOldDockID + " is equal to " + "txtDockID.Text=" + txtDockID.Text);

                Debug.WriteLine("->>SN Comparison");
                Debug.WriteLine("txtTerminalSN.Text=" + txtTerminalSN.Text + " is equal to " + "txtOldTerminalSN.Text=" + txtOldTerminalSN.Text);
                Debug.WriteLine("txtSIMSN.Text=" + txtSIMSN.Text + " is equal to " + "txtOldSIMSN.Text=" + txtOldSIMSN.Text);
                Debug.WriteLine("txtDockSN.Text=" + txtDockSN.Text + " is equal to " + "txtOldDockSN.Text=" + txtOldDockSN.Text);

                Debug.WriteLine("GetModifiedByAndDateTime");
                Debug.WriteLine("clsUser.ClassModifiedBy=" + clsUser.ClassModifiedBy);
                Debug.WriteLine("clsUser.ClassModifiedDateTime=" + clsUser.ClassModifiedDateTime);

                // Update Terminal Detail
                if (!txtTerminalSN.Text.Equals(txtOldTerminalSN.Text))
                {
                    dbAPI.UpdateTerminalDetailStatus(sOldTerminalID, clsGlobalVariables.STATUS_AVAILABLE, clsGlobalVariables.STATUS_AVAILABLE_DESC); // Update old terminal id
                    dbAPI.UpdateTerminalDetailStatus(txtTerminalID.Text, clsGlobalVariables.STATUS_DISPATCH, clsGlobalVariables.STATUS_DISPATCH_DESC); // Update new terminal id
                }

                // Update SIM Detail
                if (!txtSIMSN.Text.Equals(txtOldSIMSN.Text))
                {
                    dbAPI.UpdateSIMDetailStatus(sOldSIMID, clsGlobalVariables.STATUS_AVAILABLE, clsGlobalVariables.STATUS_AVAILABLE_DESC);  // Update old sim id
                    dbAPI.UpdateSIMDetailStatus(txtSIMID.Text, clsGlobalVariables.STATUS_DISPATCH, clsGlobalVariables.STATUS_DISPATCH_DESC);  // Update new sim id
                }

                // Update Dock Detail
                if (!txtDockSN.Text.Equals(txtOldDockSN.Text))
                {
                    dbAPI.UpdateTerminalDetailStatus(sOldDockID, clsGlobalVariables.STATUS_AVAILABLE, clsGlobalVariables.STATUS_AVAILABLE_DESC); // Update old dock id
                    dbAPI.UpdateTerminalDetailStatus(txtDockID.Text, clsGlobalVariables.STATUS_DISPATCH, clsGlobalVariables.STATUS_DISPATCH_DESC); // Update new dock id
                }
                    
                // Update Terminal Allocation
                if (dbFunction.isValidID(txtSearchTAIDNo.Text))
                {
                    dbAPI.UpdateTARemarks(txtSearchTAIDNo.Text, txtRemarks.Text, txtComments.Text); // Update TA Remarks/Comments
                    dbAPI.UpdateTAFESP(txtSearchTAIDNo.Text, txtFEID.Text, txtSPID.Text); // Update TA FE and SP
                }

                // Update Terminal Allocation
                if ((!txtTerminalSN.Text.Equals(txtOldTerminalSN.Text)) || (!txtSIMSN.Text.Equals(txtOldSIMSN.Text)) || (dbFunction.isValidID(txtSearchTAIDNo.Text)))
                {
                    dbAPI.UpdateTAReplacement(txtSearchTAIDNo.Text, txtTerminalID.Text, txtSIMID.Text, txtDockID.Text,
                                              txtTerminalTypeID.Text, txtTerminalModelID.Text, txtTerminalBrandID.Text,
                                              txtTerminalSN.Text, txtSIMSN.Text, txtDockSN.Text);
                }
                    
                // Update servicing status
                dbAPI.UpdateServiceStatus2(txtServiceNo.Text, txtRequestNo.Text, clsGlobalVariables.STATUS_DISPATCH, clsGlobalVariables.STATUS_DISPATCH_DESC, clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING_DESC);

                // Update terminal allocation status
                dbAPI.UpdateTADetailStatus2(txtSearchTAIDNo.Text, clsGlobalVariables.STATUS_DISPATCH, clsGlobalVariables.STATUS_DISPATCH_DESC, clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING_DESC);

                // Update Modified By
                dbAPI.UpdateModifiedBy("TA", txtSearchTAIDNo.Text, clsUser.ClassModifiedBy, clsUser.ClassModifiedDateTime);

                // Update Servicing FE                
                if (dbFunction.isValidID(txtServiceNo.Text))
                {
                    clsSearch.ClassAdvanceSearchValue = txtServiceNo.Text + clsFunction.sPipe +
                                                    txtFEID.Text + clsFunction.sPipe +
                                                    txtFEName.Text;
                    
                    dbAPI.ExecuteAPI("PUT", "Update", "Update Servicing SP/FE", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
                }

                // Update Dispatch
                if (dbFunction.isValidID(txtServiceNo.Text))
                    dbAPI.UpdateServicingDispatch(txtServiceNo.Text, txtRequestNo.Text, clsUser.ClassProcessedDate, clsUser.ProcessedTime, clsUser.ClassProcessedBy);

                Cursor.Current = Cursors.Default; // Back to normal

                dbFunction.SetMessageBox("Terminal Allocation has been updated/dispatch successfully.", "Updated/Dispatch Terminal", clsFunction.IconType.iInformation);

                if (chkFSR.Checked)
                {
                    if (dbFunction.isValidID(txtServiceNo.Text))
                        dbAPI.PreviewFSR(txtSearchIRNo.Text, txtRequestNo.Text, txtMerchantName.Text, txtIRTID.Text, txtIRMID.Text, "INSTALLATION", int.Parse(txtServiceNo.Text), clsFunction.iZero); // Preview FSR Report
                }                
            }
            else
            {                
                dbFunction.GetProcessedByAndDateTime(); // Get modifiedby and datetime

                string sTemp =
                               clsFunction.sLineSeparator + "\n" +
                               "Request Date: " + txtIRRequestDate.Text + "\n" +
                               "Installation Date: " + txtIRInstallationDate.Text + "\n" +
                               "Request ID: " + txtSearchIRNo.Text + "\n" +
                               clsFunction.sLineSeparator + "\n" +
                               "Merchant Name: " + txtMerchantName.Text + "\n" +
                               "TID: " + txtIRTID.Text + "\n" +
                               "MID: " + txtIRMID.Text + "\n" +
                               "Client Name :" + txtClientName.Text + "\n" +
                               clsFunction.sLineSeparator + "\n" +
                               "Terminal Serial No.: " + txtTerminalSN.Text + "\n" +
                               "Dock Serial No.: " + txtDockSN.Text + "\n" +
                               "SIM Serial No.: " + txtSIMSN.Text + "\n" +
                               clsFunction.sLineSeparator + "\n";

                if (MessageBox.Show("Do you really want to dispatch selected record(s)?\n" +
                                   "\n\n" +
                                    sTemp +
                                   "\n\n" +
                                   "Warning:" +
                                   "\nService request and SN's will be set to dispatch." +
                                   "\n",
                                   "Confirm Dispatching", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    return;
                }

                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                string sTAIDNo = dbFunction.CheckAndSetNumericValue(txtSearchTAIDNo.Text);
                if (sTAIDNo.Length > 0 && sTAIDNo.CompareTo(clsFunction.sZero) != 0)
                {
                    UpdateTerminalAllocation(sTAIDNo);

                    if (clsSearch.ClassIRStatus == clsGlobalVariables.STATUS_NEGATIVE || clsSearch.ClassStatus == clsGlobalVariables.STATUS_NEGATIVE)
                    {
                        SaveServiceDetail(false); // Save Servicing   

                        if (dbFunction.isValidID(sHoldServiceNo))
                            dbAPI.UpdateServiceJobType(sHoldServiceNo, sHoldRequestNo, clsGlobalVariables.JOB_TYPE_STATUS_FAILED, clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC, clsGlobalVariables.JOB_TYPE_STATUS_FAILED_DESC, clsGlobalVariables.TA_STATUS_INSTALLED_SUB_DESC);
                    }

                    if (dbFunction.isValidID(txtServiceNo.Text))
                        dbAPI.UpdateServicingDispatch(txtServiceNo.Text, txtRequestNo.Text, clsUser.ClassProcessedDate, clsUser.ProcessedTime, clsUser.ClassProcessedBy);
                }
                else
                {

                    if (!ValidateSerialNo()) return;

                    SaveTerminalAllocation();

                    SaveServiceDetail(true); // Save Servicing

                    if (dbFunction.isValidID(txtServiceNo.Text))
                        dbAPI.UpdateServicingDispatch(txtServiceNo.Text, txtRequestNo.Text, clsUser.ClassProcessedDate, clsUser.ProcessedTime, clsUser.ClassProcessedBy);
                }

                //SaveNotification(clsGlobalVariables.EMAIL_NOTIF_TYPE); // For Email Notification
                //SaveNotification(clsGlobalVariables.SMS_NOTIF_TYPE); // For SMS Notification

                clsSearch.ClassStatus = clsGlobalVariables.STATUS_AVAILABLE;
                clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_AVAILABLE_DESC;

                // Update Hold Variable
                if (dbFunction.isValidID(sHoldTerminalID))
                {
                    if (sHoldTerminalID.CompareTo(txtTerminalID.Text) != 0)
                        dbAPI.UpdateTerminalDetailStatus(sHoldTerminalID, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);
                }

                if (dbFunction.isValidID(sHoldSIMID))
                {
                    if (sHoldSIMID.CompareTo(txtSIMID.Text) != 0)
                        dbAPI.UpdateSIMDetailStatus(sHoldSIMID, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);
                }

                if (dbFunction.isValidID(sHoldDockID))
                {
                    if (sHoldDockID.CompareTo(txtDockID.Text) != 0)
                        dbAPI.UpdateTerminalDetailStatus(sHoldDockID, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);
                }

                clsSearch.ClassStatus = clsGlobalVariables.STATUS_DISPATCH;
                clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_DISPATCH_DESC;

                // Update Status            
                if (dbFunction.isValidID(txtIRIDNo.Text))
                    dbAPI.UpdateIRDetailStatus(txtIRIDNo.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                if (dbFunction.isValidID(sTAIDNo))
                    dbAPI.UpdateTADetailStatus(sTAIDNo, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                if (dbFunction.isValidID(txtServiceNo.Text))
                    dbAPI.UpdateServiceStatus(txtServiceNo.Text, txtRequestNo.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription, clsSearch.ClassIRIDNo.ToString());

                if (dbFunction.isValidID(txtServiceNo.Text))
                    dbAPI.UpdateServiceJobType(txtServiceNo.Text, txtRequestNo.Text, clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING, clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC, clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING_DESC, clsGlobalVariables.TA_STATUS_INSTALLED_SUB_DESC);

                dbAPI.UpdateTerminalDetailStatus(txtTerminalID.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);
                dbAPI.UpdateTerminalDetailStatus(txtDockID.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);
                dbAPI.UpdateSIMDetailStatus(txtSIMID.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                // Update FEID and SPID
                if (dbFunction.isValidID(txtServiceNo.Text))
                {
                    clsSearch.ClassAdvanceSearchValue = txtServiceNo.Text + clsFunction.sPipe +
                                                        txtFEID.Text + clsFunction.sPipe +
                                                        txtFEName.Text;

                    Debug.WriteLine("UpdateFE::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                    dbAPI.ExecuteAPI("PUT", "Update", "Update Servicing SP/FE", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
                }

                Cursor.Current = Cursors.Default; // Back to normal 

                if (clsSearch.ClassIRStatus == clsGlobalVariables.STATUS_NEGATIVE || clsSearch.ClassStatus == clsGlobalVariables.STATUS_NEGATIVE)
                    dbFunction.SetMessageBox("Negative Terminal Allocation has been updated/dispatch successfully.", "Updated/Dispatch Terminal", clsFunction.IconType.iInformation);
                else
                    dbFunction.SetMessageBox("Terminal Allocation has been updated/dispatch successfully.", "Updated/Dispatch Terminal", clsFunction.IconType.iInformation);

                if (chkFSR.Checked)
                {
                    if (dbFunction.isValidID(txtServiceNo.Text))
                        dbAPI.PreviewFSR(txtSearchIRNo.Text, txtRequestNo.Text, txtMerchantName.Text, txtIRTID.Text, txtIRMID.Text, "INSTALLATION", int.Parse(txtServiceNo.Text), clsFunction.iZero); // Preview FSR Report
                }                
            }
            
            btnClear_Click(this, e);

        }

        private void SetHoldVariable()
        {
            sHoldSPID = dbFunction.CheckAndSetNumericValue(txtSPID.Text);
            sHoldSPName = dbFunction.CheckAndSetStringValue(txtSPName.Text);
            sHoldClientID = dbFunction.CheckAndSetNumericValue(txtClientID.Text);
            sHoldClientName = dbFunction.CheckAndSetStringValue(txtClientName.Text);
            sHoldTerminalID = dbFunction.CheckAndSetNumericValue(txtTerminalID.Text);
            sHoldTerminalSN = dbFunction.CheckAndSetStringValue(txtTerminalSN.Text);
            sHoldSIMID = dbFunction.CheckAndSetNumericValue(txtSIMID.Text);
            sHoldSIMSN = dbFunction.CheckAndSetStringValue(txtSIMSN.Text);
            sHoldDockID = dbFunction.CheckAndSetNumericValue(txtDockID.Text);
            sHoldDockSN = dbFunction.CheckAndSetStringValue(txtDockSN.Text);
        }

        public bool fDetailConfirm(bool fDelete)
        {
            bool fConfirm = true;

            string sTempSerial = "";

            if (txtSearchTAIDNo.Text.Length > 0 && txtSearchTAIDNo.Text.CompareTo(clsFunction.sZero) != 0)
            {
                if (!fAllowDelete("Terminal"))
                {
                    dbFunction.SetMessageBox("Unable to delete installation request. Terminal serial number has been changed.\n\n" +
                                             "Please reload the terminal allocation again.", "Warning! Terminal serial number changed.", clsFunction.IconType.iWarning);
                    fConfirm = false;
                }

                if (!fAllowDelete("SIM"))
                {
                    dbFunction.SetMessageBox("Unable to delete installation request. SIM serial number has been changed.\n\n" +
                                             "Please reload the terminal allocation again.", "Warning! SIM serial number changed.", clsFunction.IconType.iWarning);
                    fConfirm = false;
                }

                if (!fAllowDelete("Dock"))
                {
                    dbFunction.SetMessageBox("Unable to delete installation request. Dock serial number has been changed.\n\n" +
                                             "Please reload the terminal allocation again.", "Warning! Dock serial number changed.", clsFunction.IconType.iWarning);
                    fConfirm = false;
                }
            }

            string sTemp =
                           clsFunction.sLineSeparator + "\n" +
                           "Request Date: " + txtIRRequestDate.Text + "\n" +
                           "Installation Date: " + txtIRInstallationDate.Text + "\n" +
                           "Request ID: " + txtSearchIRNo.Text + "\n" +
                           clsFunction.sLineSeparator + "\n" +
                           "Merchant Name: " + txtMerchantName.Text + "\n" +
                           "TID: " + txtIRTID.Text + "\n" +
                           "MID: " + txtIRMID.Text + "\n" +
                           "Client Name :" + txtClientName.Text + "\n" +
                           clsFunction.sLineSeparator + "\n";

            if (txtSearchTAIDNo.Text.Length > 0 && txtSearchTAIDNo.Text.CompareTo(clsFunction.sZero) != 0)
            {
                sTempSerial = "Terminal Serial No.: " + txtTerminalSN.Text + "\n" +
                            "Dock Serial No.: " + txtDockSN.Text + "\n" +
                            "SIM Serial No.: " + txtSIMSN.Text + "\n" +
                            clsFunction.sLineSeparator + "\n";
            }

            sTemp = sTemp + sTempSerial;

            if (fDelete)
            {
                if (MessageBox.Show("Do you really want to delete selected record(s)?\n" +
                               "\n\n" +
                                sTemp +
                                "\n\n" +
                               "Warning:\nData will permanently deleted and included serial number used will be set to available." +
                               "\n",
                               "Confirm Delete Service Request?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    fConfirm = false;
                }
            }
            else
            {
                if (MessageBox.Show("Do you really want to cancel selected record(s)?\n" +
                               "\n\n" +
                                sTemp +
                                "\n\n" +
                               "Warning: Data will permanently cancel and included serial number used will be set to available." +
                               "\n",
                               "Confirm Cancel Service Request?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    fConfirm = false;
                }
            }
            

            return fConfirm;
        }

        /*
        public bool fModifyConfirm()
        {
            bool fConfirm = true;
            bool fModify = false;
            string sSPDetails = "";
            string sFEDetails = "";
            string sTerminalDetails = "";
            string sDockDetails = "";
            string sSIMDetails = "";

            if (sHoldSPID.CompareTo(txtSPID.Text) != 0)
            {
                fModify = true;
                sSPDetails = "*Service provider change" +
                              "\n" +
                              "  From: " + sHoldSPName + "\n" +
                              "  To  : " + txtSPName.Text + "\n" +
                              "----------------------------------------------------------" +
                              "\n";
            }

            if (sHoldClientID.CompareTo(txtClientID.Text) != 0)
            {
                fModify = true;
                sFEDetails = "*Client change" +
                              "\n" +
                              "  From: " + sHoldClientName + "\n" +
                              "  To  : " + txtClientName.Text + "\n" +
                              "----------------------------------------------------------" +
                              "\n";
            }

            if (sHoldTerminalID.CompareTo(txtTerminalID.Text) != 0)
            {
                fModify = true;
                sTerminalDetails = "*Terminal serial number change" +
                              "\n" +
                              "  From: " + sHoldTerminalSN + "\n" +
                              "  To  : " + txtTerminalSN.Text + "\n" +
                              "----------------------------------------------------------" +
                              "\n";
            }

            if (sHoldSIMID.CompareTo(txtSIMID.Text) != 0)
            {
                fModify = true;
                sSIMDetails = "*SIM serial number change" +
                              "\n" +
                              "  From: " + sHoldSIMSN + "\n" +
                              "  To  : " + txtSIMSN.Text + "\n" +
                              "----------------------------------------------------------" +
                              "\n";
            }

            if (sHoldDockID.CompareTo(txtDockID.Text) != 0)
            {
                fModify = true;
                sDockDetails = "*Terminal dock serial number change" +
                              "\n" +
                              "  From: " + sHoldDockSN + "\n" +
                              "  To  : " + txtDockSN.Text + "\n" +
                              "----------------------------------------------------------" +
                              "\n";
            }

            if (fModify)
            {
                if (MessageBox.Show("Do you really want to modify selected record(s)?\n" +
                                    "\n" +
                                    sSPDetails +
                                    sFEDetails +
                                    sTerminalDetails +
                                    sSIMDetails +
                                    sDockDetails +
                                    "\n\n",
                                    "Confirm?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    fConfirm = false;
                }
            }
            else
            {
                dbFunction.SetMessageBox("No modification made.", "Terminal Allocation", clsFunction.IconType.iInformation);
                fConfirm = false;
            }
            

            return fConfirm;
        }
        */

        private bool fAllowDelete(string sType)
        {
            bool fValid = true;

            switch (sType)
            {
                case "Terminal":
                    if (sHoldTerminalID.CompareTo(txtTerminalID.Text) != 0)
                        fValid = false;
                        break;
                case "Dock":
                    if (sHoldDockID.CompareTo(txtDockID.Text) != 0)
                        fValid = false;
                    break;
                case "SIM":
                    if (sHoldSIMID.CompareTo(txtSIMID.Text) != 0)
                        fValid = false;
                    break;
            }

            return fValid;
        }

        private void btnCancelIR_Click(object sender, EventArgs e)
        {
            dbFunction.SetMessageBox("Cancellation not allowed yet.", "Service cancel", clsFunction.IconType.iExclamation);
            return;

            if (dbFunction.isValidID(txtSearchTAIDNo.Text))
            {
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iReason, txtReasonID.Text)) return;

                if (!dbFunction.isValidEntry(clsFunction.CheckType.iRemarks, txtRemarks.Text)) return;
                
                if (txtServiceJobTypeStatusDesc.Text.Equals(clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC))
                {
                    dbFunction.SetMessageBox("Unable to cancel terminal allocation that already completed.", "Terminal allocation cancel", clsFunction.IconType.iExclamation);
                }
                else
                {
                    if (MessageBox.Show("Are you sure to cancel terminal allocation?" + "\n\n" + 
                        "Client:" + txtClientName.Text + "\n" +
                        "Merchant:" + txtMerchantName.Text + "\n" +
                        "Field Engineer:" + txtFEName.Text + "\n" +
                        clsFunction.sLineSeparator + "\n" +
                        "IR No:" + txtRequestNo.Text + "\n" +
                        "IR Date:" + txtIRInstallationDate.Text + "\n" +
                        "IR Processed By:" + txtIRProcessedBy.Text + "\n" +
                        clsFunction.sLineSeparator + "\n" +
                        "TA Date:" + txtTADateTime.Text + "\n" +
                        "TA Processed By:" + txtProcessedBy.Text + "\n" +
                        clsFunction.sLineSeparator + "\n" +
                        "Terminal SN:" + txtTerminalSN.Text + "\n" +
                        " > Type:" + txtTerminalType.Text + "\n" +
                        " > Model:" + txtTerminalModel.Text + "\n" +
                        " > Brand:" + txtTerminalBrand.Text + "\n" +
                        "SIM SN:" + txtSIMSN.Text + "\n" +
                        " > Carrier:" + txtSIMCarrier.Text + "\n" +
                        "Dock SN:" + txtDockSN.Text + "\n" +
                        clsFunction.sLineSeparator + "\n" +
                        "Reason:" + txtReasonDescription.Text + "\n" +
                        "Remarks:" + txtRemarks.Text,
                        "Terminal allocation cancellation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }

                    dbFunction.GetModifiedByAndDateTime(); // Get modifiedby and datetime

                    // Update TA Cancelled
                    clsSearch.ClassAdvanceSearchValue = txtSearchTAIDNo.Text + clsFunction.sPipe +
                                                        clsFunction.sOne + clsFunction.sPipe +
                                                        clsGlobalVariables.STATUS_CANCELLED + clsFunction.sPipe +
                                                        clsGlobalVariables.STATUS_CANCELLED_DESC + clsFunction.sPipe +
                                                        clsGlobalVariables.JOB_TYPE_STATUS_CANCELLED_DESC;

                    Debug.WriteLine("clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                    //dbAPI.ExecuteAPI("PUT", "Update", "Update TA Cancelled", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                    // Update TA FillUp
                    clsSearch.ClassAdvanceSearchValue = txtSearchTAIDNo.Text + clsFunction.sPipe +
                                                        txtRemarks.Text + clsFunction.sPipe +
                                                        txtComments.Text + clsFunction.sPipe +
                                                        txtReasonID.Text;

                    Debug.WriteLine("clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                    //dbAPI.ExecuteAPI("PUT", "Update", "Update TA FillUp", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
                    
                    // Update Modified By
                    //dbAPI.UpdateModifiedBy("TA", txtSearchTAIDNo.Text, clsUser.ClassModifiedBy, clsUser.ClassModifiedDateTime);

                    clsSearch.ClassStatus = clsGlobalVariables.STATUS_AVAILABLE;
                    clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_AVAILABLE_DESC;

                    // Update Terminal Detail
                    if (dbFunction.isValidID(txtTerminalID.Text))
                        //dbAPI.UpdateTerminalDetailStatus(txtTerminalID.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                    // Update SIM Detail
                    if (dbFunction.isValidID(txtSIMID.Text))
                        //dbAPI.UpdateSIMDetailStatus(txtSIMID.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                    // Update Dock Detail
                    if (dbFunction.isValidID(txtDockID.Text))
                        //dbAPI.UpdateTerminalDetailStatus(txtDockID.Text, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                    dbFunction.SetMessageBox("Terminal allocation cancelled complete.", "Terminal allocation cancel", clsFunction.IconType.iInformation);

                    btnClear_Click(this, e);
                }
            }
            else
            {
                dbFunction.SetMessageBox("No terminal allocation record to be cancelled.", "Terminal allocation cancel", clsFunction.IconType.iExclamation);
            }

            
        }
        private void SaveServiceDetail(bool isDispatch)
        {
            string sSQL = "";
            string sRowSQL = "";
            DateTime ProcessDateTime = DateTime.Now;
            string sDateTime = "";
            string sProcessDate = "";
            string sProcessTime = "";
            string sProcessedBy = clsUser.ClassUserFullName;            

            string sServiceReqDate = "";
            string sServiceReqTime = "";
            int isBillable = (chkBillable.Checked == true ? 1 : 0);

            sProcessDate = ProcessDateTime.ToString("yyyy-MM-dd");
            sProcessTime = ProcessDateTime.ToString("H:mm:ss");
            sDateTime = sProcessDate + " " + sProcessTime;

            sServiceReqDate = ProcessDateTime.ToString("yyyy-MM-dd");
            sServiceReqTime = ProcessDateTime.ToString("H:mm:ss");


            dbFunction.GetProcessedByAndDateTime(); // Get processedby and datetime

            dbFunction.GetModifiedByAndDateTime(); // Get modifiedby and datetime

            dbAPI.GetFSRStatus(clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC); // Get FSR Status
            
            if (isDispatch)
                clsSearch.ClassJobType = clsGlobalVariables.STATUS_DISPATCH;
            else
                clsSearch.ClassJobType = clsGlobalVariables.STATUS_ALLOCATED;

            clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC;
            clsSearch.ClassJobTypeSubDescription = clsGlobalVariables.TA_STATUS_INSTALLED_SUB_DESC;
            clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;

            // Get Service Request ID            
            int iControlNo = dbAPI.GetControlID("Servicing Detail");
            string sServiceRequestID = iControlNo.ToString();
            sServiceRequestID = dbFunction.GenerateControlNo(iControlNo, "SR");
            clsSearch.ClassRequestNo = sServiceRequestID;
            txtRequestNo.Text = sServiceRequestID;

            // Get Counter
            clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtSearchTAIDNo.Text) + clsFunction.sPipe;
            dbAPI.GetViewCount("Search", "Servicing Detail Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
            int iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            string sCounter = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, 3);
            string sServiceDate = dbFunction.getReformatDate(txtIRRequestDate.Text);

            sSQL = "";
            sRowSQL = "";
            sRowSQL = "('" + dbFunction.CheckAndSetNumericValue(txtSearchTAIDNo.Text) + "'," +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtClientID.Text) + "'," +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtMerchantID.Text) + "'," +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtFEID.Text) + "'," +
            sRowSQL + sRowSQL + "'" + txtClientName.Text + "'," +
            sRowSQL + sRowSQL + "'" + txtMerchantName.Text + "'," +
            sRowSQL + sRowSQL + "'" + txtFEName.Text + "'," +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + "'," +
            sRowSQL + sRowSQL + "'" + txtSearchIRNo.Text + "'," +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtTerminalID.Text) + "'," +
            sRowSQL + sRowSQL + "'" + txtTerminalSN.Text + "'," +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtSIMID.Text) + "'," +
            sRowSQL + sRowSQL + "'" + txtSIMSN.Text + "'," +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtDockID.Text) + "'," +
            sRowSQL + sRowSQL + "'" + txtDockSN.Text + "'," +            
            sRowSQL + sRowSQL + "'" + sCounter + "'," +
            sRowSQL + sRowSQL + "'" + clsSearch.ClassRequestNo + "'," +
            sRowSQL + sRowSQL + "'" + sDateTime + "'," +
            sRowSQL + sRowSQL + "'" + sServiceDate + "'," +
            sRowSQL + sRowSQL + "'" + sProcessTime + "'," +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(txtMerchantContactPerson.Text) + "'," +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(txtMerchantTelNo.Text) + "'," +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(txtMerchantPosition.Text) + "'," +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(txtRemarks.Text) + "'," +
            sRowSQL + sRowSQL + "'" + sServiceReqDate + "'," +
            sRowSQL + sRowSQL + "'" + sServiceReqTime + "'," +
            sRowSQL + sRowSQL + "'" + clsGlobalVariables.STATUS_INSTALLATION_DESC + "'," +
            sRowSQL + sRowSQL + "'" + clsFunction.sDash + "'," +
            sRowSQL + sRowSQL + "'" + clsFunction.sZero + "'," +
            sRowSQL + sRowSQL + "'" + clsFunction.sDash + "'," +
            sRowSQL + sRowSQL + "'" + clsFunction.sZero + "'," +
            sRowSQL + sRowSQL + "'" + clsFunction.sDash + "'," +
            sRowSQL + sRowSQL + "'" + clsFunction.sZero + "'," +
            sRowSQL + sRowSQL + "'" + clsFunction.sDash + "'," +            
            sRowSQL + sRowSQL + "'" + clsSearch.ClassJobType + "'," +
            sRowSQL + sRowSQL + "'" + clsSearch.ClassJobTypeDescription + "'," +
            sRowSQL + sRowSQL + "'" + clsSearch.ClassJobTypeSubDescription + "'," +
            sRowSQL + sRowSQL + "'" + clsSearch.ClassJobTypeStatusDescription + "'," +
            sRowSQL + sRowSQL + "'" + clsFunction.sDash + "'," +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtRegionID.Text) + "'," +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtRegionType.Text) + "'," +
            sRowSQL + sRowSQL + "" + isBillable + "," +
            sRowSQL + sRowSQL + "" + clsGlobalVariables.SERVICE_PRIMARY + "," +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(txtAppVersion.Text) + "'," +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(txtAppCRC.Text) + "'," +
            sRowSQL + sRowSQL + "'" + clsUser.ClassProcessedBy + "'," +
            sRowSQL + sRowSQL + "'" + clsUser.ClassModifiedBy + "'," +
            sRowSQL + sRowSQL + "'" + clsUser.ClassProcessedDateTime + "'," +
            sRowSQL + sRowSQL + "'" + clsUser.ClassModifiedDateTime + "') ";
            sSQL = sSQL + sRowSQL;           

            Debug.WriteLine("sSQL=" + sSQL);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "Servicing Detail", sSQL, "InsertCollectionDetail");
            
            txtServiceNo.Text = clsLastID.ClassLastInsertedID.ToString();

            // Update Job Type Detail (tblterminalallocation)
            clsSearch.ClassAdvanceSearchValue = txtSearchTAIDNo.Text + clsFunction.sPipe +
                                                txtIRIDNo.Text + clsFunction.sPipe +
                                                clsSearch.ClassJobType + clsFunction.sPipe +
                                                clsSearch.ClassJobTypeDescription + clsFunction.sPipe +
                                                clsSearch.ClassJobTypeSubDescription + clsFunction.sPipe +
                                                clsSearch.ClassJobTypeStatusDescription;

            dbAPI.ExecuteAPI("PUT", "Update", "Job Type Detail", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");


            clsSearch.ClassCurTerminalSNStatus = clsFunction.iZero;
            clsSearch.ClassCurSIMSNStatus = clsFunction.iZero;
            clsSearch.ClassCurDockSNStatus = clsFunction.iZero;
            clsSearch.ClassCurTerminalSNStatusDescription = clsFunction.sDefaultSelect;            
            clsSearch.ClassCurSIMSNStatusDescription = clsFunction.sDefaultSelect;            
            clsSearch.ClassCurDockSNStatusDescription = clsFunction.sDefaultSelect;

            dbAPI.UpdateServicingCurrentTerminalStatus(txtServiceNo.Text, txtRequestNo.Text);

            if (dbFunction.isValidID(txtServiceNo.Text))
            {                              
                string sServiceCode = dbAPI.GetServiceCodeByDescription(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC, false);
                dbAPI.UpdateServiceCode(txtServiceNo.Text, sServiceCode);
            }
        }

        private void LoadServicingDetail()
        {
            int i = 0;
            int iLineNo = 0;

            lvwList.Items.Clear();
           
            clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe +
                txtSearchTAIDNo.Text + clsFunction.sPipe +
                clsFunction.sZero + clsFunction.sPipe +
                clsFunction.sZero + clsFunction.sPipe +
                dbFunction.CheckAndSetNumericValue(txtMerchantID.Text) + clsFunction.sPipe +
                dbFunction.CheckAndSetNumericValue(txtClientID.Text) + clsFunction.sPipe +
                txtIRTID.Text + clsFunction.sPipe +
                txtIRMID.Text;

            dbAPI.ExecuteAPI("GET", "View", "Servicing Detail", clsSearch.ClassAdvanceSearchValue, "Servicing Detail", "", "ViewServicingDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.ServiceNo.Length > i)
                {
                    iLineNo++;

                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.ServiceNo[i].ToString());
                    item.SubItems.Add(clsArray.ServiceDateTime[i]);
                    item.SubItems.Add(clsArray.CounterNo[i]);
                    item.SubItems.Add(clsArray.IRNo[i]);
                    item.SubItems.Add(clsArray.RequestNo[i]);
                    item.SubItems.Add(clsArray.ServiceDate[i]);
                    item.SubItems.Add(clsArray.ServiceTime[i]);
                    item.SubItems.Add(clsArray.CustomerName[i]);
                    item.SubItems.Add(clsArray.CustomerContactNo[i]);
                    item.SubItems.Add(clsArray.Remarks[i]);
                    item.SubItems.Add(clsArray.ServiceReqDate[i]);
                    item.SubItems.Add(clsArray.ServiceReqTime[i]);
                    item.SubItems.Add(clsArray.LastServiceRequest[i]);
                    item.SubItems.Add(clsArray.NewServiceRequest[i]);
                    item.SubItems.Add(clsArray.ReplaceTerminalSN[i]);
                    item.SubItems.Add(clsArray.ReplaceSIMSN[i]);
                    item.SubItems.Add(clsArray.ReplaceDockSN[i]);
                    item.SubItems.Add(clsArray.JobType[i]);
                    item.SubItems.Add(clsArray.JobTypeDescription[i]);
                    item.SubItems.Add(clsArray.JobTypeSubDescription[i]);
                    item.SubItems.Add(clsArray.JobTypeStatusDescription[i]);
                    item.SubItems.Add(clsArray.ReasonID[i]);
                    item.SubItems.Add(clsArray.ReasonDescription[i]);
                    item.SubItems.Add(clsArray.ServiceStatus[i]);
                    item.SubItems.Add(clsArray.ServiceStatusDescription[i]);
                    item.SubItems.Add(clsArray.ExpensesID[i]);
                    item.SubItems.Add(clsArray.TExpenses[i]);
                    item.SubItems.Add(clsArray.ReferenceNo[i]);

                    lvwList.Items.Add(item);

                    i++;

                }

                dbFunction.ListViewAlternateBackColor(lvwList);                
            }
        }

        private void lvwList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwList.SelectedItems.Count > 0)
            {
                string LineNo = lvwList.SelectedItems[0].Text;
                txtLineNo.Text = LineNo;

                if (LineNo.Length > 0)
                {
                    clsSearch.ClassServiceNo = int.Parse(lvwList.SelectedItems[0].SubItems[1].Text);
                    clsSearch.ClassJobOrderDate = lvwList.SelectedItems[0].SubItems[2].Text;
                    clsSearch.ClassIRNo = lvwList.SelectedItems[0].SubItems[4].Text;
                    clsSearch.ClassServiceRequestID = lvwList.SelectedItems[0].SubItems[5].Text;
                    clsSearch.ClassJobTypeStatusDescription = lvwList.SelectedItems[0].SubItems[21].Text;

                    lvwList_DoubleClick(this, e);
                    
                }
            }
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.Items.Count > 0)
            {
                if (clsSearch.ClassServiceNo > 0)
                {
                    ListDetails(int.Parse(txtLineNo.Text));
                    dbFunction.ListViewAlternateBackColor(lvwDetail);
                }
            }
        }
        private void ListDetails(int iRow)
        {
            int iLineNo = 0;
            int iColCount = lvwList.Columns.Count;


            lvwDetail.Items.Clear();
            for (int i = 0; i < iColCount; i++)
            {
                iLineNo++;
                string cellParam = lvwList.Columns[i].Text; // Param
                string cellValue = lvwList.SelectedItems[0].SubItems[i].Text; // Value

                switch (i)
                {
                    case 0:
                    case 1:
                    case 3:
                    case 17:
                    case 18:
                    case 22:
                    case 24:
                    case 26:
                        break;
                    default:
                        ListViewItem item = new ListViewItem(iLineNo.ToString());
                        item.SubItems.Add(cellParam);
                        item.SubItems.Add(cellValue);
                        lvwDetail.Items.Add(item);
                        break;
                }
            }
        }

        private void txtRemarks_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtComments_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void SerialNoStatusDescription()
        {
            int i = 0;
            string sStatusDescription = clsFunction.sNull;

            InitSerialStatusDescription();

            // Terminal Status Description
            i = 0;
            sStatusDescription = clsFunction.sDash;
            clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtTerminalSN.Text);
            dbAPI.ExecuteAPI("GET", "View", "Terminal Status Description", clsSearch.ClassAdvanceSearchValue, "Terminal", "", "ViewAdvanceTerminal");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.TerminalID.Length > i)
                {
                    sStatusDescription = clsArray.TerminalStatusDescription[i];
                    i++;
                }                
            }

            lblTerminalSNStatus.Text = (dbFunction.isValidID(txtTerminalID.Text) ? sStatusDescription : clsFunction.sDash);

            // Dock Status Description
            i = 0;
            sStatusDescription = clsFunction.sDash;
            clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtDockSN.Text);
            dbAPI.ExecuteAPI("GET", "View", "Terminal Status Description", clsSearch.ClassAdvanceSearchValue, "Terminal", "", "ViewAdvanceTerminal");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.TerminalID.Length > i)
                {
                    sStatusDescription = clsArray.TerminalStatusDescription[i];
                    i++;
                }                
            }            
            lblDockSNStatus.Text = (dbFunction.isValidID(txtDockID.Text) ? sStatusDescription : clsFunction.sDash);

            // SIM Status Description
            i = 0;
            sStatusDescription = clsFunction.sDash;
            clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtSIMSN.Text);
            dbAPI.ExecuteAPI("GET", "View", "SIM Status Description", clsSearch.ClassAdvanceSearchValue, "Terminal", "", "ViewAdvanceTerminal");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.SIMID.Length > i)
                {
                    sStatusDescription = clsArray.SIMStatusDescription[i];
                    i++;
                }                
            }            
            lblSIMSNStatus.Text = (dbFunction.isValidID(txtSIMID.Text) ? sStatusDescription : clsFunction.sDash);

        }

        private void InitSerialStatusDescription()
        {
            lblTerminalSNStatus.Text = lblDockSNStatus.Text = lblSIMSNStatus.Text = clsFunction.sDash;
        }

        private void btnNewSP_Click(object sender, EventArgs e)
        {
            frmParticular.iParticularType = clsGlobalVariables.iMerchant_Type;
            frmParticular frm = new frmParticular();
            frm.ShowDialog();            
        }

        private void btnNewFE_Click(object sender, EventArgs e)
        {            
            frmParticular.iParticularType = clsGlobalVariables.iFE_Type;
            frmParticular frm = new frmParticular();
            frm.ShowDialog();
        }

        private void InitStatusTitle(bool isClear)
        {
            if (isClear)
            {
                lblFSRStatus.BackColor = Color.DarkGray;
                lblFSRStatus.Text = clsFunction.sDash;
            }
            else
            {
                if (dbFunction.isValidID(txtSearchTAIDNo.Text))
                {
                    lblFSRStatus.BackColor = Color.DarkRed;
                    lblFSRStatus.Text = "UPDATE SN's ALLOCATION";
                }
                else
                {
                    lblFSRStatus.BackColor = Color.DarkBlue;
                    lblFSRStatus.Text = "NEW SN's ALLOCATION";
                }
            }

        }

        public bool fConfrimUpdateTA()
        {
            bool fConfirm = true;           
            string sTemp = "";

            sTemp = "You are about to update the already saved terminal allocation.\n\n" +
                    "Previous SN's details:\n" +
                    "Terminal:" + txtOldTerminalSN.Text + "\n" +
                    "SIM: " + txtOldSIMSN.Text + "\n" +
                    "Dock: " + txtOldDockSN.Text + "\n\n" +
                    "Update SN's details:\n" +
                    "Terminal:" + txtTerminalSN.Text + "\n" +
                    "SIM: " + txtSIMSN.Text + "\n" +
                    "Dock: " + txtDockSN.Text + "\n\n" +
                    "Are you sure to continue?" + "\n\n" +
                    "Note: Previous SN's, status will be set to AVAILABLE.";

            if (MessageBox.Show(sTemp, "Confirm Updating", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                fConfirm = false;
            }

            return fConfirm;
        }

        private void UpdateButton(bool isClear)
        {
            btnSave.Enabled = false;
            btnDispatchIR.Enabled = false;
            btnCancelIR.Enabled = false;

            if (isClear)
            {
                btnSave.Enabled = false;
                btnDispatchIR.Enabled = false;
                btnCancelIR.Enabled = false;
            }
            else
            {
                if (txtServiceJobTypeStatusDesc.Text.Equals(clsGlobalVariables.STATUS_DISPATCH_DESC))
                {
                    btnCancelIR.Enabled = true;
                }
                else
                {
                    if (txtServiceJobTypeStatusDesc.Text.Equals(clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC))
                    {
                        btnSave.Enabled = true;
                        btnDispatchIR.Enabled = true;
                        btnCancelIR.Enabled = true;
                    }
                    else if (txtServiceJobTypeStatusDesc.Text.Equals(clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING_DESC))
                    {
                        btnCancelIR.Enabled = true;
                    }
                    else if (txtServiceJobTypeStatusDesc.Text.Equals(clsGlobalVariables.JOB_TYPE_STATUS_PREPARATION_DESC))
                    {
                        btnSave.Enabled = true;
                        btnDispatchIR.Enabled = true;
                        btnCancelIR.Enabled = true;
                    }
                }
            }
        }

        private void txtMerchantTelNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSearchReason_Click(object sender, EventArgs e)
        {
            clsSearch._isWriteResponse = true;
            frmSearchField.iSearchType = frmSearchField.SearchType.iReason;
            frmSearchField.sHeader = "REASON";
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

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void CheckWindowState()
        {            
            switch (this.WindowState)
            {
                case FormWindowState.Maximized:
                    this.WindowState = FormWindowState.Normal;
                    break;
                case FormWindowState.Normal:
                    this.WindowState = FormWindowState.Normal;
                    break;
                case FormWindowState.Minimized:
                    this.WindowState = FormWindowState.Normal;
                    break;
            }
        }

        private void InitCustomer(bool isEnabled)
        {
            if (isEnabled)
            {
                txtMerchantContactPerson.Enabled = txtMerchantPosition.Enabled = txtMerchantTelNo.Enabled = txtMerchantMobile.Enabled = true;
                txtMerchantContactPerson.ReadOnly = txtMerchantPosition.ReadOnly = txtMerchantTelNo.ReadOnly = txtMerchantMobile.ReadOnly = false;
                txtMerchantContactPerson.BackColor = txtMerchantPosition.BackColor = txtMerchantTelNo.BackColor = txtMerchantMobile.BackColor = clsFunction.EntryBackColor;
            }
            else
            {
                txtMerchantContactPerson.Enabled = txtMerchantPosition.Enabled = txtMerchantTelNo.Enabled = txtMerchantMobile.Enabled = false;
                txtMerchantContactPerson.ReadOnly = txtMerchantPosition.ReadOnly = txtMerchantTelNo.ReadOnly = txtMerchantMobile.ReadOnly = true;
                txtMerchantContactPerson.BackColor = txtMerchantPosition.BackColor = txtMerchantTelNo.BackColor = txtMerchantMobile.BackColor = clsFunction.DisableBackColor;
            }
        }
    }
}
