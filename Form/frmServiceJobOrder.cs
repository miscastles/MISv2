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
using static MIS.Function.AppUtilities;
using Newtonsoft.Json;
using MIS.Enums;
using MIS.Controller;

namespace MIS
{

    public partial class frmServiceJobOrder : Form
    {

        public static bool fAutoLoadData = false;
        public static bool fModify = false;
        public static bool fCompleted = false;
        public static bool fSelected = false;
        private clsAPI dbAPI;
        private clsINI dbSetting;
        private clsFile dbFile;
        private clsFunction dbFunction;
        private clsReportFunc dbReportFunc;

        // Controller
        private ServicingDetailController _mServicingDetailController;
        private HelpDeskController _mHelpDeskController;

        private bool fEdit = false;
        public static string sHeader = "";
        public static clsAPI.JobType iSearchJobType;
        public static clsAPI.JobType iHoldJobType;
        public static clsFunction.CheckType iCheckType;

        private static bool isDispatch;
        private static int iStatus;
        private static string sStatusDesc;

        int iLimit = 1000;

        private string pHoldEntryRequestID = clsFunction.sNull;
        private string pHoldEntryReferenceNo = clsFunction.sNull;

        private enum searchType
        {
            iTerminal, iSIM, iDocker
        }

        class jsonObj
        {
            public object outParamValue { get; set; }
        }

        // Override CreateParams to enable double-buffering for child controls
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

        public frmServiceJobOrder()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwList, true);
            dbFunction.setDoubleBuffer(lvwChanges, true);
            dbFunction.setDoubleBuffer(lvwStockDetail, true);
            dbFunction.setDoubleBuffer(lvwMM, true);
            dbFunction.setDoubleBuffer(lvwProfile1, true);
            dbFunction.setDoubleBuffer(lvwProfile2, true);
            dbFunction.setDoubleBuffer(lvwRaw, true);

            // Initialize the controller object
            _mServicingDetailController = new ServicingDetailController();
            _mHelpDeskController = new HelpDeskController();
        }

        private void frmServicing_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbSetting = new clsINI();
            dbFile = new clsFile();
            dbFunction = new clsFunction();
            dbReportFunc = new clsReportFunc();
            
            dbSetting.InitDatabaseSetting();

            dbFunction.ClearTextBox(this);
            dbFunction.ClearRichTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);

            dbFunction.ClearListViewItems(lvwList);
            dbFunction.ClearListViewItems(lvwChanges);
            dbFunction.ClearListViewItems(lvwStockDetail);
            dbFunction.ClearListViewItems(lvwRepStockDetail);
            dbFunction.ClearListViewItems(lvwMM);
            dbFunction.ClearListViewItems(lvwProfile1);
            dbFunction.ClearListViewItems(lvwProfile2);
            dbFunction.ClearListViewItems(lvwRaw);

            InitServiceHistoryListView();
            InitChangesListView();

            fEdit = false;
            InitButton();          
            InitDate();

            lblHeader.Text = lblHeader.Text + " " + "[ " + sHeader + " ]";
        
            dbFunction.ComBoBoxUnLock(false, this);

            InitStatusTitle(true);
            
            UpdateButton(true);

            btnSearchMerchant.Enabled = false;
            btnSearchService.Enabled = true;
            btnUpdateServiceType.Enabled = false;
            InitSearchRemoveButton(true);

            InitCount();

            dbAPI.ExecuteAPI("GET", "View", "All Type", "", "Type", "", "ViewType"); // re-load all type

            dbAPI.FillComboBoxServiceType(cboSearchServiceType);

            if (clsSearch.ClassIsBillType > 0)
            {
                cboBillingType.Enabled = true;
                dbAPI.FillComboBoxTypeByGroup(cboBillingType, (int)GroupType.BillingTypeID);                
            }
            else
            {
                cboBillingType.Enabled = false;
                cboBillingType.Text = clsFunction.sDefaultSelect;                
            }

            dbAPI.FillComboBoxTypeByGroup(cboSource, (int)GroupType.SourceType);
            dbAPI.FillComboBoxTypeByGroup(cboCategory, (int)GroupType.CategoryType);
            dbAPI.FillComboBoxTypeByGroup(cboSubCategory, (int)GroupType.SubCategoryType);

            ComboBoxDefaultSelect();
            AdditionalComBoBoxUnlock(false);

            // chkDispatch.Enabled = true;
            // chkDispatch.Checked = true;
            btnSendEmail.Enabled = false;

            btnPreviewSvcHistory.Enabled = btnPreviewFSR.Enabled = btnViewDiagnostic.Enabled = btnUpdateServiceDate.Enabled = btnUpdateMerchRep.Enabled = false;

            initDispatch(false);

            clsSearch.ClassIsExportToPDF = false;

            //Init Search Button (Search Merchant)
            btnSearchMerchant.Enabled = false;
            dbFunction.SetButtonIconImage(btnSearchMerchant);

            //Init Search Button (Search Service)
            btnSearchService.Enabled = true;
            dbFunction.SetButtonIconImage(btnSearchService);

            //Init Search Button (Search Helpdesk Ticket)
            btnSearchAssistNo.Enabled = false;
            dbFunction.SetButtonIconImage(btnSearchAssistNo);

            //setMainTab();

            btnCancelJO.Enabled = btnRefreshSN.Enabled = false;

            if (fAutoLoadData)
            {
                btnSearchService_Click(this, e);
                fAutoLoadData = false;
            }

            tabFillUp.TabIndex = 0;
            
            Cursor.Current = Cursors.Default;

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void FillMerchantTextBox()
        {
            string profile_info = "";
            string rawdata_info = "";
            string profile_config_info = "";

            Debug.WriteLine("--FillMerchantTextBox--");
            Debug.WriteLine("fEdit="+ fEdit);
            Debug.WriteLine("txtMerchantID.Text="+ txtMerchantID.Text);
            Debug.WriteLine("txtIRIDNo.Text=" + txtIRIDNo.Text);
            Debug.WriteLine("txtSearchServiceNo.Text=" + txtSearchServiceNo.Text);

            txtSearchMerchantName.Text =
            txtMerchantName.Text =
            txtMerchantAddress.Text =
            txtMerchantProvince.Text =
            txtMerchantRegion.Text =
            txtMerchantCity.Text =
            txtMerchantContactPerson.Text =
            txtMerchantTelNo.Text =
            txtMerchantMobileNo.Text =
            txtMerchantEmail.Text =
            txtIRTID.Text =
            txtIRMID.Text =
            txtMerchantPrimaryNum.Text =
            txtMerchantSecondaryNum.Text =
            txtAppVersion.Text =
            txtAppCRC.Text =
            //txtIRIDNo.Text =
            txtSearchIRNo.Text =
            txtIRRequestDate.Text =
            txtIRInstallationDate.Text =
            txtCustomerName.Text =
            txtCustomerPosition.Text =
            txtFUAppVersion.Text =
            txtFUAppCRC.Text =
            txtCustomerContactNo.Text =
            txtCustomerEmail.Text = 
            txtIRStatusDescription.Text =
            txtRequestor.Text =
            clsFunction.sNull;

            if (dbFunction.isValidID(txtMerchantID.Text))
            {
                if (!fEdit)
                    dbAPI.ExecuteAPI("GET", "Search", "Merchant Info", txtMerchantID.Text + clsFunction.sPipe + txtIRIDNo.Text, "Get Info Detail", "", "GetInfoDetail");
                else
                    dbAPI.ExecuteAPI("GET", "Search", "Service Merchant Info", txtSearchServiceNo.Text, "Get Info Detail", "", "GetInfoDetail");

                // parse delimited
                dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                if (dbAPI.isNoRecordFound() == false)
                {
                    txtMerchantID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                    txtMerchantName.Text = txtSearchMerchantName.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtMerchantAddress.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                    txtMerchantProvince.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                    txtMerchantRegion.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                    txtMerchantCity.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                    txtMerchantContactPerson.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                    txtMerchantTelNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                    txtMerchantMobileNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                    txtMerchantEmail.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                    txtIRTID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                    txtIRMID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 11);
                    txtMerchantPrimaryNum.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 12);
                    txtMerchantSecondaryNum.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);
                    txtAppVersion.Text = txtFUAppVersion.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 14);
                    txtAppCRC.Text = txtFUAppCRC.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 15);
                    txtIRIDNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 16);
                    txtSearchIRNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 17);
                    txtIRRequestDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 18);
                    txtIRInstallationDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 19);

                    //txtRMInstruction.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 21);
                    
                    txtCustomerName.Text = txtMerchantContactPerson.Text;                  
                    txtCustomerContactNo.Text = txtMerchantMobileNo.Text;
                    txtCustomerEmail.Text = txtMerchantEmail.Text;

                    // get json data
                    if (!fEdit)
                    {
                        profile_info = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 29);
                        rawdata_info = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 30);
                        profile_config_info = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 34);
                    }
                    else
                    {
                        profile_info = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 26);
                        rawdata_info = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 27);
                        profile_config_info = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 28);
                    }
                    
                    if (!fEdit)
                    {
                        if (cboSearchServiceType.SelectedItem.ToString().Equals(clsGlobalVariables.STATUS_INSTALLATION_DESC))
                            txtRMInstruction.Text = dbFunction.getJSONTagValue(rawdata_info, clsDefines.IR_RM_INSTRUCTION, clsDefines.ROOTKEY_RAWDATA_INFO, clsDefines.NESTED_OBJECT_VALUES);
                        else
                            txtRMInstruction.Text = "";
                    }
                    else
                    {
                        txtRMInstruction.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 21);
                    }
                    
                    if (!fEdit)
                    {
                        txtRemarks.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 28);
                        txtIRStatusDescription.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 32);
                        
                        txtRequestor.Text = dbFunction.getJSONTagValue(rawdata_info, clsDefines.IR_REQUESTOR, clsDefines.ROOTKEY_RAWDATA_INFO, clsDefines.NESTED_OBJECT_VALUES);
                        txtVendor.Text = dbFunction.getJSONTagValue(rawdata_info, clsDefines.IR_VENDOR, clsDefines.ROOTKEY_RAWDATA_INFO, clsDefines.NESTED_OBJECT_VALUES);

                    }
                    else
                    {
                        txtIRStatusDescription.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 23);
                        txtVendor.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 24);
                        txtRequestor.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 25);
                        
                    }
                    
                    // rawdata_info
                    if (dbFunction.isValidDescription(rawdata_info))
                        dbFunction.populateListViewFromJsonString(lvwRaw, rawdata_info, clsDefines.ROOTKEY_RAWDATA_INFO, clsDefines.NESTED_OBJECT_VALUES);

                    // profile_info
                    if (dbFunction.isValidDescription(profile_info))
                        dbFunction.populateListViewFromJsonString(lvwProfile1, profile_info, clsDefines.ROOTKEY_PROFILE_INFO, clsDefines.gNull);

                    // profile_config_info
                    if (dbFunction.isValidDescription(profile_config_info))
                        dbFunction.populateListViewFromJsonString(lvwProfile2, profile_config_info, clsDefines.ROOTKEY_PROFILE_CONFIG_INFO, clsDefines.NESTED_OBJECT_VALUES);

                    if (dbFunction.isValidDescription(rawdata_info))
                    {
                        txtPOSType.Text = dbFunction.getJSONTagValue(rawdata_info, clsDefines.IR_POS_TYPE, clsDefines.ROOTKEY_RAWDATA_INFO, clsDefines.NESTED_OBJECT_VALUES);
                        txtRequestType.Text = dbFunction.getJSONTagValue(rawdata_info, clsDefines.IR_REQUEST_TYPE, clsDefines.ROOTKEY_RAWDATA_INFO, clsDefines.NESTED_OBJECT_VALUES);
                    }
                    // set dteServiceReqDate / Sechedule date
                    if (!txtIRInstallationDate.Text.Equals(clsDefines.DEV_DATE) && cboSearchServiceType.SelectedItem.ToString().Equals(clsGlobalVariables.STATUS_INSTALLATION_DESC))
                    {
                        dteReqInstallationDate.Value = DateTime.Parse(txtIRRequestDate.Text);
                        dteServiceReqDate.Value = DateTime.Parse(txtIRInstallationDate.Text);
                    }

                    // Assigned SN
                    dbAPI.ExecuteAPI("GET", "Search", "Merchant Current Terminal SN", dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text), "Get Info Detail", "", "GetInfoDetail");
                    txtAssignedTerminalID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtAssignedTerminalSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);

                    dbAPI.ExecuteAPI("GET", "Search", "Merchant Current SIM SN", dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text), "Get Info Detail", "", "GetInfoDetail");
                    txtAssignedSIMID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtAssignedSIMSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                }
            }
          
        }
        private void FillClientTextBox()
        {
            Debug.WriteLine("--FillClientTextBox--");
            Debug.WriteLine("txtMerchantID.Text=" + txtMerchantID.Text);
            Debug.WriteLine("txtClientID.Text=" + txtClientID.Text);

            txtClientName.Text =
            txtClientAddress.Text =         
            txtClientContactPerson.Text =
            txtClientTelNo.Text =
            txtClientMobileNo.Text =
            txtClientEmail.Text = clsFunction.sNull;

            if (dbFunction.isValidID(txtMerchantID.Text))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Client Info", txtClientID.Text, "Get Info Detail", "", "GetInfoDetail");

                if (dbAPI.isNoRecordFound() == false)
                {
                    txtClientID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                    txtClientName.Text = clsSearch.ClassClientName = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtClientAddress.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                    txtClientContactPerson.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                    txtClientTelNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                    txtClientMobileNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                    txtClientEmail.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                    txtClientCode.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 11);

                }
            }
        }

        private void FillFETextBox()
        {
            Debug.WriteLine("--FillFETextBox--");
            Debug.WriteLine("txtMerchantID.Text="+ txtMerchantID.Text);
            Debug.WriteLine("txtFEID.Text=" + txtFEID.Text);
            
            txtFEName.Text =
            txtFEAddress.Text =
            txtFEContactPerson.Text =
            txtFETelNo.Text =
            txtFEMobileNo.Text =
            txtFEEmail.Text = clsFunction.sNull;

            if (dbFunction.isValidID(txtMerchantID.Text) && dbFunction.isValidID(txtFEID.Text))
            {
                //if (!fEdit)
                    dbAPI.ExecuteAPI("GET", "Search", "FE Info", txtFEID.Text, "Get Info Detail", "", "GetInfoDetail");
                //else
                //    dbAPI.ExecuteAPI("GET", "Search", "Service FE Info", txtSearchServiceNo.Text, "Get Info Detail", "", "GetInfoDetail");

                if (dbAPI.isNoRecordFound() == false)
                {
                    txtFEID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                    txtFEName.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtFEAddress.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                    txtFEContactPerson.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                    txtFETelNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                    txtFEMobileNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                    txtFEEmail.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);

                }
            }


        }
        private void PopulateTerminalTextBox(string sTerminalID, string sTerminalSN, bool isCurrent)
        {
            Debug.WriteLine("--PopulateTerminalTextBox--");
            Debug.WriteLine("sTerminalID="+ sTerminalID+ ",sTerminalSN="+ sTerminalSN+ ",isCurrent="+ isCurrent);

            if (isCurrent)
            {                           
                txtCurTerminalStatus.Text =
                txtCurTerminalSN.Text =
                txtCurTerminalCode.Text =
                txtCurTerminalType.Text =
                txtCurTerminalModel.Text =
                txtCurTerminalBrand.Text =
                txtCurTerminalLocation.Text =
                txtCurTerminalAssetType.Text = clsFunction.sNull;
                lblCurTerminalDetail.Text = clsDefines.HEADER_CURRENT_TERMINAL;

                if (dbFunction.isValidID(txtMerchantID.Text) && dbFunction.isValidID(sTerminalID))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "Terminal SN Info", sTerminalID, "Get Info Detail", "", "GetInfoDetail");

                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtCurTerminalID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                        txtCurTerminalStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                        txtCurTerminalSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                        txtCurTerminalCode.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                        txtCurTerminalType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                        txtCurTerminalModel.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                        txtCurTerminalBrand.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                        txtCurTerminalLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);
                        txtCurTerminalAssetType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 15);

                        txtCurTerminalLocationID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 33);
                        txtCurTerminalStatusDesc.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 34);

                        //lblCurTerminalDetail.Text = clsDefines.HEADER_CURRENT_TERMINAL + clsDefines.gPipe + dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 30);

                        clsSearch.ClassIsReleased = int.Parse(dbFunction.CheckAndSetNumericValue(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 26)));
                    }
                }
            }
            else
            {
                txtRepTerminalSN.Text =
                txtRepTerminalCode.Text =
                txtRepTerminalType.Text =
                txtRepTerminalModel.Text =
                txtRepTerminalBrand.Text =
                txtRepTerminalLocation.Text =
                txtRepTerminalAssetType.Text = clsFunction.sNull;
                lblRepTerminalDetail.Text = clsDefines.HEADER_REPLACE_TERMINAL;

                if (dbFunction.isValidID(txtMerchantID.Text) && dbFunction.isValidID(sTerminalID))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "Terminal SN Info", sTerminalID, "Get Info Detail", "", "GetInfoDetail");

                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtRepTerminalID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                        txtRepTerminalStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                        txtRepTerminalSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                        txtRepTerminalCode.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                        txtRepTerminalType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                        txtRepTerminalModel.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                        txtRepTerminalBrand.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                        txtRepTerminalLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);
                        txtRepTerminalAssetType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 15);

                        txtRepTerminalLocationID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 33);
                        txtRepTerminalStatusDesc.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 34);

                        //lblRepTerminalDetail.Text = clsDefines.HEADER_REPLACE_TERMINAL + clsDefines.gPipe + dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 30);

                        clsSearch.ClassIsReleased = int.Parse(dbFunction.CheckAndSetNumericValue(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 26)));
                    }
                }
            }
            
        }
        private void PopulateSIMTextBox(string sSIMID, string sSIMSN, bool isCurrent)
        {
            Debug.WriteLine("--PopulateSIMTextBox--");
            Debug.WriteLine("sSIMID=" + sSIMID + ",sSIMSN=" + sSIMSN + ",isCurrent=" + isCurrent);

            if (isCurrent)
            {
                txtCurSIMStatus.Text =
                txtCurSIMSN.Text =
                txtCurSIMCarrier.Text =
                txtCurSIMLocation.Text = clsFunction.sNull;
                lblCurSIMDetail.Text = clsDefines.HEADER_CURRENT_SIM;

                if (dbFunction.isValidID(txtMerchantID.Text) && dbFunction.isValidID(sSIMID))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "SIM SN Info", sSIMID, "Get Info Detail", "", "GetInfoDetail");

                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtCurSIMID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                        txtCurSIMStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                        txtCurSIMSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                        txtCurSIMCarrier.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);                      
                        txtCurSIMLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                        txtCurSIMLocationID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 19);
                        txtCurSIMStatusDesc.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 18);

                        //lblCurSIMDetail.Text = clsDefines.HEADER_CURRENT_SIM + clsDefines.gPipe + dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 18);

                        clsSearch.ClassIsReleased = int.Parse(dbFunction.CheckAndSetNumericValue(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 16)));
                    }
                }
            }
            else
            {
                txtRepSIMStatus.Text =
                txtRepSIMSN.Text =
                txtRepSIMCarrier.Text =
                txtRepSIMLocation.Text = clsFunction.sNull;
                lblRepSIMDetail.Text = clsDefines.HEADER_REPLACE_SIM;

                if (dbFunction.isValidID(txtMerchantID.Text) && dbFunction.isValidID(sSIMID))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "SIM SN Info", sSIMID, "Get Info Detail", "", "GetInfoDetail");

                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtRepSIMID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                        txtRepSIMStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                        txtRepSIMSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                        txtRepSIMCarrier.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                        txtRepSIMLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                        txtRepSIMLocationID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 19);
                        txtRepSIMStatusDesc.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 18);

                        //lblRepSIMDetail.Text = clsDefines.HEADER_REPLACE_SIM + clsDefines.gPipe + dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 18);

                        // ROCKY - SERVICING ISSUE: FIXED FOR INVALID CHECKING IF TERMINAL IS RELEASED ADD CHECKING IN CLASSISRELEASED VALUE.
                        clsSearch.ClassIsReleased = int.Parse(dbFunction.CheckAndSetNumericValue(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 16)));
                    }
                }
            }

        }
        private void PopulateDockTextBox(string sDockID, string sDockSN, bool isCurrent)
        {
            Debug.WriteLine("--PopulateDockTextBox--");
            Debug.WriteLine("isCurrent=" + isCurrent);
            Debug.WriteLine("sDockID=" + sDockID);
            Debug.WriteLine("sDockSN=" + sDockSN);

            txtCurDockStatus.Text = clsFunction.sZero;
            cboSearchDockStatus.Text = clsFunction.sDefaultSelect;

            if (sDockSN.CompareTo(clsFunction.sDash) == 0 || sDockSN.CompareTo(clsFunction.sZero) == 0)
            {
                if (isCurrent)
                {
                    txtCurDockID.Text = clsFunction.sZero;
                    txtCurDockSN.Text = clsFunction.sNull;
                    txtCurDockSN.BackColor = clsFunction.DisableBackColor;
                }
                else
                {
                    txtRepDockID.Text = clsFunction.sZero;
                    txtRepDockSN.Text = clsFunction.sNull;
                    txtRepDockSN.BackColor = clsFunction.DisableBackColor;
                }

                return;
            }

            dbAPI.GetTerminalInfo(sDockID, sDockSN);

            if (isCurrent)
            {
                txtCurDockID.Text = dbFunction.CheckAndSetNumericValue(clsTerminal.ClassTerminalID.ToString());
                txtCurDockSN.Text = dbFunction.CheckAndSetStringValue(clsTerminal.ClassTerminalSN);
                //txtCurDockSN.BackColor = clsFunction.MKBackColor;

                // Set Dock Status
                txtCurDockStatus.Text = clsTerminal.ClassTerminalStatus.ToString();
                cboSearchDockStatus.Text = clsTerminal.ClassTerminalStatusDescription;
            }
            else
            {
                txtRepDockID.Text = dbFunction.CheckAndSetNumericValue(clsTerminal.ClassTerminalID.ToString());
                txtRepDockSN.Text = dbFunction.CheckAndSetStringValue(clsTerminal.ClassTerminalSN);
                //txtRepDockSN.BackColor = clsFunction.MKBackColor;
            }

        }
        private void PKTextBoxBackColor(bool isLock)
        {
            if (isLock)
            {
                txtMerchantName.BackColor = clsFunction.MKBackColor;
                //txtCurTerminalSN.BackColor = clsFunction.PKBackColor;
                //txtIRTID.BackColor = clsFunction.PKBackColor;
                //txtIRMID.BackColor = clsFunction.PKBackColor;
                //txtCurSIMSN.BackColor = clsFunction.PKBackColor;
                txtClientName.BackColor = clsFunction.MKBackColor;
                txtFEName.BackColor = clsFunction.MKBackColor;
                //txtCurDockSN.BackColor = clsFunction.PKBackColor;

                //txtAppVersion.BackColor = clsFunction.PKBackColor;
                //txtAppCRC.BackColor = clsFunction.PKBackColor;

            }
        }
        /*
        private void MKTextBoxBackColor(bool isLock)
        {
            if (isLock)
            {
                //txtSearchTAIDNo.BackColor = clsFunction.MKBackColor;
                //txtSearchIRNo.BackColor = clsFunction.MKBackColor;
                //txtCurrentStatus.BackColor = clsFunction.MKBackColor;
                //txtLastServiceRequest.BackColor = clsFunction.MKBackColor;
                //txtLastJobTypeStatus.BackColor = clsFunction.MKBackColor;
                //txtServiceRequestID.BackColor = clsFunction.MKBackColor;
                //txtServiceReferenceNo.BackColor = clsFunction.MKBackColor;
                //txtLastJobTypeDescription.BackColor = clsFunction.MKBackColor;
                //txtLastReasonDescription.BackColor = clsFunction.MKBackColor;
                //txtServiceStatusDescription.BackColor = clsFunction.MKBackColor;
                txtServiceJobTypeStatusDesc.BackColor = clsFunction.StatusBackColor;

                //txtServiceJobType.BackColor = clsFunction.MKBackColor;
                //txtServiceJobTypeDesc.BackColor = clsFunction.MKBackColor;

                // FSR
                //txtSearchFSRTimeArrived.BackColor = clsFunction.MKBackColor;
                //txtSearchFSRReceiptTime.BackColor = clsFunction.MKBackColor;
                //txtSearchFSRTimeStart.BackColor = clsFunction.MKBackColor;
                //txtSearchFSRTimeEnd.BackColor = clsFunction.MKBackColor;
                txtSearchFSRServiceResult.BackColor = clsFunction.StatusBackColor;
            }
        }
        */
        
        private void PKTextBoxReadOnly(bool fReadOnly)
        {          
            txtCustomerName.ReadOnly = fReadOnly;
            txtCustomerPosition.ReadOnly = fReadOnly;
            txtCustomerContactNo.ReadOnly = fReadOnly;
            txtCustomerEmail.ReadOnly = fReadOnly;

            txtFUAppVersion.ReadOnly = fReadOnly;
            txtFUAppCRC.ReadOnly = fReadOnly;

            txtRemarks.ReadOnly = !fReadOnly;
            //txtEntryRequestID.ReadOnly = txtEntryReferenceNo.ReadOnly = fReadOnly;
            
            if (!fReadOnly)
            {              
                txtCustomerName.BackColor = clsFunction.EntryBackColor;
                txtCustomerPosition.BackColor = clsFunction.EntryBackColor;
                txtCustomerContactNo.BackColor = clsFunction.EntryBackColor;
                txtCustomerEmail.BackColor = clsFunction.EntryBackColor;

                txtFUAppVersion.BackColor = clsFunction.EntryBackColor;
                txtFUAppCRC.BackColor = clsFunction.EntryBackColor;

                txtRemarks.BackColor = clsFunction.EntryBackColor;              
                //txtEntryRequestID.BackColor = txtEntryReferenceNo.BackColor = clsFunction.EntryBackColor;             
            }
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

        private void btnClearField_Click(object sender, EventArgs e)
        {
            txtRequestNo.Text = "";
            txtCustomerName.Text = "";
            txtCustomerPosition.Text = "";
            txtCustomerContactNo.Text = "";
            txtCustomerEmail.Text = "";
            txtRemarks.Text = "";
            txtRepTerminalSN.Text = "";
            txtRepSIMSN.Text = "";
            txtRepDockSN.Text = "";

            txtFUAppVersion.Text = "";
            txtFUAppCRC.Text = "";
            
            if (iSearchJobType == clsAPI.JobType.iReplacement)
            {
                //dbFunction.ClearComboBox(this);
                dbAPI.FillComboBoxTerminalStatus(cboSearchTerminalStatus);
                dbAPI.FillComboBoxTerminalStatus(cboSearchSIMStatus);
                dbAPI.FillComboBoxTerminalStatus(cboSearchDockStatus);
                dbFunction.ComBoBoxUnLock(true, this);
                DefaultSelectedComboBoxValue();
            }

        }
        private void InitDate()
        {
            //dteReqDate.Value = DateTime.Now.Date;
            //dbFunction.SetDateCustomFormat(dteReqDate);

            dteReqInstallationDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteReqInstallationDate, clsFunction.sDateDefaultFormat);

            dteServiceReqDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteServiceReqDate, clsFunction.sDateDefaultFormat);

            //dteReqTime.Value = DateTime.Now;
            //dbFunction.SetTimeCustomFormat(dteReqTime);

            dteReqTime.Value = DateTime.Now;
            dbFunction.SetTimeFormat(dteReqTime, clsFunction.sTimeDefaultFormat);


        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbFunction.ClearTextBox(this);
            dbFunction.ClearRichTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);

            dbFunction.ClearListViewItems(lvwList);
            dbFunction.ClearListViewItems(lvwChanges);
            dbFunction.ClearListViewItems(lvwStockDetail);
            dbFunction.ClearListViewItems(lvwRepStockDetail);
            dbFunction.ClearListViewItems(lvwMM);
            dbFunction.ClearListViewItems(lvwProfile1);
            dbFunction.ClearListViewItems(lvwProfile2);
            dbFunction.ClearListViewItems(lvwRaw);

            //InitServiceHistoryListView();
            //InitChangesListView();

            fAutoLoadData = false;
            fEdit = false;
            fModify = false;
            fSelected = false;
            InitButton();        
            //gbReplacement.Enabled = false;
            
            btnDispatchJO.Enabled = false;
            btnDeleteJO.Enabled = false;
            btnServiceSearch.Enabled = false;            
            btnTASearch.Enabled = false;
            btnRefreshService.Enabled = false;
            btnUpdateServiceDate.Enabled = false;
            btnUpdateMerchRep.Enabled = false;

            btnUpdateServiceType.Enabled = false;
            btnUpdateServiceType.Text = "EDIT SERVICE TYPE";

            InitStatusTitle(true);
            UpdateButton(true);
        
            lblHeader.Text = "JOB ORDER";
            lblSubHeader.Text = clsFunction.sDash;

            //InitSearchRemoveButton(true);
            //InitSearchButton(true);

            cboSearchServiceType.Text = clsFunction.sDefaultSelect;
            
            InitDate();
            InitCreatedDateTime();
            InitCount();

            initDispatch(false);

            btnSendEmail.Enabled = false;
            
            btnPreviewFSR.Enabled = btnViewDiagnostic.Enabled = false;

            // SN header
            lblCurTerminalDetail.Text = clsDefines.HEADER_CURRENT_TERMINAL;
            lblCurSIMDetail.Text = clsDefines.HEADER_CURRENT_SIM;
            lblRepTerminalDetail.Text = clsDefines.HEADER_REPLACE_TERMINAL;
            lblRepSIMDetail.Text = clsDefines.HEADER_REPLACE_SIM;

            lblCreatedDate.Text = clsFunction.sNull;

            //Init Search Button (Search Merchant)
            btnSearchMerchant.Enabled = false;
            dbFunction.SetButtonIconImage(btnSearchMerchant);

            //Init Search Button (Search Service)
            btnSearchService.Enabled = true;
            dbFunction.SetButtonIconImage(btnSearchService);

            //Init Search Button (Search Helpdesk Ticket)
            btnSearchAssistNo.Enabled = false;
            dbFunction.SetButtonIconImage(btnSearchAssistNo);

            //setMainTab();

            btnPreviewSvcHistory.Enabled = btnCancelJO.Enabled = btnRefreshSN.Enabled = false;

            ComboBoxDefaultSelect();
            AdditionalComBoBoxUnlock(false);

            tabFillUp.TabIndex = 0;

            Cursor.Current = Cursors.Default;


        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isAdd, clsUser.ClassUserID, 25)) return;
            
            dbAPI.GenerateID(true, txtRequestNo, txtSearchServiceNo, "Servicing Detail", clsDefines.CONTROLID_PREFIX_SERVICE);
            lblSubHeader.Text = txtRequestNo.Text;

            fAutoLoadData = false;
            fEdit = false;
            fModify = false;
            btnAdd.Enabled = false;
            btnSave.Enabled = true;
            btnServiceSearch.Enabled = false;            
            cboSearchServiceType.Enabled = true;
            lblHeader.Text = "CREATE JOB ORDER";    
            
            SetMKTextBoxBackColor();
            SetPKTextBoxBackColor();
            
            //txtSearchServiceNo.Text = clsFunction.sZero;
            InitStatusTitle(false);

            //InitSearchRemoveButton(false);

            //Init Search Button (Search Merchant)
            btnSearchMerchant.Enabled = true;
            dbFunction.SetButtonIconImage(btnSearchMerchant);

            //Init Search Button (Search Service)
            btnSearchService.Enabled = false;
            dbFunction.SetButtonIconImage(btnSearchService);

            //Init Search Button (Search Helpdesk Ticket)
            btnSearchAssistNo.Enabled = true;
            dbFunction.SetButtonIconImage(btnSearchAssistNo);

            ComboBoxDefaultSelect();

        }
        
        private bool fConfirmDetails(bool fDispatch)
        {
            bool fConfirm = true;
            bool isTerminalChanged = false;
            bool isSIMChanged = false;
            int pOldID = 0;
            string pOldSN = clsFunction.sNull;
            int pNewID = 0;
            string pNewSN = clsFunction.sNull;

            if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC))
            {
                // Current Terminal
                pOldID = int.Parse(dbFunction.CheckAndSetNumericValue(txtOldTerminalID.Text));
                pOldSN = dbFunction.CheckAndSetStringValue(txtOldTerminalSN.Text);

                pNewID = int.Parse(dbFunction.CheckAndSetNumericValue(txtCurTerminalID.Text));
                pNewSN = dbFunction.CheckAndSetStringValue(txtCurTerminalSN.Text);
                isTerminalChanged = dbFunction.isSNChanged(pOldID, pOldSN, pNewID, pNewSN);

                // Current SIM
                pOldID = int.Parse(dbFunction.CheckAndSetNumericValue(txtOldSIMID.Text));
                pOldSN = dbFunction.CheckAndSetStringValue(txtOldSIMSN.Text);

                pNewID = int.Parse(dbFunction.CheckAndSetNumericValue(txtCurSIMID.Text));
                pNewSN = dbFunction.CheckAndSetStringValue(txtCurSIMSN.Text);
                isSIMChanged = dbFunction.isSNChanged(pOldID, pOldSN, pNewID, pNewSN);

            }

            if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
            {
                // Replace Terminal
                pOldID = int.Parse(dbFunction.CheckAndSetNumericValue(txtOldTerminalID.Text));
                pOldSN = dbFunction.CheckAndSetStringValue(txtOldTerminalSN.Text);

                pNewID = int.Parse(dbFunction.CheckAndSetNumericValue(txtRepTerminalID.Text));
                pNewSN = dbFunction.CheckAndSetStringValue(txtRepTerminalSN.Text);
                isTerminalChanged = dbFunction.isSNChanged(pOldID, pOldSN, pNewID, pNewSN);

                // Replace SIM
                pOldID = int.Parse(dbFunction.CheckAndSetNumericValue(txtOldSIMID.Text));
                pOldSN = dbFunction.CheckAndSetStringValue(txtOldSIMSN.Text);

                pNewID = int.Parse(dbFunction.CheckAndSetNumericValue(txtRepSIMID.Text));
                pNewSN = dbFunction.CheckAndSetStringValue(txtRepSIMSN.Text);
                isSIMChanged = dbFunction.isSNChanged(pOldID, pOldSN, pNewID, pNewSN);
            }

            Debug.WriteLine("isTerminalChanged=" + isTerminalChanged + ",isSIMChanged=" + isSIMChanged);

            string sTemp = (fDispatch ? "Are you sure to dispatch the following details below:\n\n" : "Are you to save the following details below:\n\n") +
                           clsFunction.sLineSeparator + "\n" +
                           "CLIENT: " + txtClientName.Text + "\n" +
                           "PRIMARY REQUEST ID: " + txtRequestID1.Text + "\n" +
                           "REQUEST ID: " + txtEntryRequestID.Text + "\n" +
                           "REFERENCE NO: " + txtEntryReferenceNo.Text + "\n" +
                           clsFunction.sLineSeparator + "\n" +
                           "JOB TYPE: " + cboSearchServiceType.Text + "\n" +
                           "    >Job Type Description: " + txtSearchSTJobTypeDescription.Text + "\n" +
                           "    >Job Order Created Date/Time: " + txtCreatedDate.Text + " " + txtCreatedTime.Text + "\n" +
                           "    >Service Request ID: " + txtSearchIRNo.Text + "\n" +
                           "    >Request No: " + txtRequestNo.Text + "\n" +
                           "    >Schedule Date: " + dteReqInstallationDate.Value.ToString("MM-dd-yyyy") + "\n" +                        
                           clsFunction.sLineSeparator + "\n" +
                           "Apps Version : " + txtFUAppVersion.Text + "\n" +
                           "Apps CRC. : " + txtFUAppCRC.Text + "\n" +
                           clsFunction.sLineSeparator + "\n" +
                           "MERCHANT: " + txtMerchantName.Text + "\n" +
                           "    >TID: " + txtIRTID.Text + "\n" +
                           "    >MID: " + txtIRMID.Text + "\n" +
                           "    >Representative: " + txtCustomerName.Text + "\n" +
                           "    >Position: " + txtCustomerPosition.Text + "\n" +
                           "    >Contact No: " + txtCustomerContactNo.Text + "\n" +
                           clsFunction.sLineSeparator + "\n" +
                           "Vendor Representative: " + txtFEName.Text + "\n" +
                           "Vendor Mobile No.: " + txtFEMobileNo.Text + "\n" +
                           clsFunction.sLineSeparator + "\n";

            string sCurrent =
                       (isTerminalChanged ?
                                   "TERMINAL SN SWITCH: " + "\n" +
                                   "    >Previous terminal SN: " + txtOldTerminalSN.Text + "\n" +
                                   "    >New terminal SN: " + txtCurTerminalSN.Text + "\n" +
                                                clsFunction.sLineSeparator + "\n" :
                                   "CURRENT TERMINAL: " + dbFunction.CheckAndSetStringValue(txtCurTerminalSN.Text) + "\n" +
                                   "    >Type: " + dbFunction.CheckAndSetStringValue(txtCurTerminalType.Text) + "\n" +
                                   "    >Model: " + dbFunction.CheckAndSetStringValue(txtCurTerminalModel.Text) + "\n" +
                                   "    >Brand: " + dbFunction.CheckAndSetStringValue(txtCurTerminalBrand.Text) + "\n" +                                  
                                   clsFunction.sLineSeparator + "\n") +
                       (isSIMChanged ?
                                   "SIM SN SWITCH: " + "\n" +
                                   "    >Previous SIM SN: " + txtOldSIMSN.Text + "\n" +
                                   "    >New SIM SN: " + txtCurSIMSN.Text + "\n" +
                                                clsFunction.sLineSeparator + "\n" :
                                   "CURRENT SIM: " + dbFunction.CheckAndSetStringValue(txtCurSIMSN.Text) + "\n" +
                                   "    >Carrier: " + dbFunction.CheckAndSetStringValue(txtCurSIMCarrier.Text) + "\n" +                                
                                   clsFunction.sLineSeparator + "\n");

            if (!dbFunction.isValidID(txtCurTerminalID.Text)) sCurrent = clsFunction.sNull;

            string sReplace =
                       (isTerminalChanged ?
                                   "TERMINAL SN SWITCH: " + "\n" +
                                   "    >Previous terminal SN: " + txtOldTerminalSN.Text + "\n" +
                                   "    >New terminal SN: " + txtRepTerminalSN.Text + "\n" +
                                                clsFunction.sLineSeparator + "\n" :
                                   "REPLACE TERMINAL: " + dbFunction.CheckAndSetStringValue(txtRepTerminalSN.Text) + "\n" +
                                   "    >Type: " + dbFunction.CheckAndSetStringValue(txtRepTerminalType.Text) + "\n" +
                                   "    >Model: " + dbFunction.CheckAndSetStringValue(txtRepTerminalModel.Text) + "\n" +
                                   "    >Brand: " + dbFunction.CheckAndSetStringValue(txtRepTerminalBrand.Text) + "\n" +  
                                   "    >Brand: " + dbFunction.CheckAndSetStringValue(txtRepTerminalBrand.Text) + "\n" +    
                                   clsFunction.sLineSeparator + "\n") +
                       (isSIMChanged ?
                                   "SIM SN SWITCH: " + "\n" +
                                   "    >Previous SIM SN: " + txtOldSIMSN.Text + "\n" +
                                   "    >New SIM SN: " + txtRepSIMSN.Text + "\n" +
                                                clsFunction.sLineSeparator + "\n" :
                                   "REPLACE SIM: " + dbFunction.CheckAndSetStringValue(txtRepSIMSN.Text) + "\n" +
                                   "    >Carrier: " + dbFunction.CheckAndSetStringValue(txtRepSIMCarrier.Text) + "\n" +                                 
                                   clsFunction.sLineSeparator + "\n");

            if (!dbFunction.isValidID(txtRepTerminalID.Text)) sReplace = clsFunction.sNull;

            // Clear message
            if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
                sCurrent = clsFunction.sNull;
            else
                sReplace = clsFunction.sNull;

            if (MessageBox.Show(sTemp +
                                sCurrent +
                                sReplace +
                                "\n" +
                                "Warning:" +
                                 (isTerminalChanged ? "\nPrevious terminal SN status will change to AVAILABLE." : "") +
                                (isSIMChanged ? "\nPrevious SIM SN status will change to AVAILABLE." : "") +
                                (chkDispatch.Checked ? "\nService request and SN's will be DISPATCH." : "\nService request and SN's will be set to ALLOCATED.") +
                                (chkEmail.Checked && chkDispatch.Checked ? "\nVendor representative will be notified via EMAIL." : "")
                                ,
                                (fDispatch ? "Confirm dispatch" : "Confirm save"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fConfirm = false;
            }

            return fConfirm;
        }

        private bool ValidateFields(bool fDispatch)
        {
            bool isBypassSNChecking = false;
            
            string sReqTime = dbFunction.GetDateFromParse(dteReqTime.Text, "h:mm:ss tt", "HH:mm:ss");

            // Current Terminal SN
            if (isDispatch)
            {
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalID, txtCurTerminalID.Text)) return false;
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iCurTerminalSN, txtCurTerminalSN.Text)) return false;
            }

            // Service Result
            if (!dbFunction.isValidComboBoxValue(cboSearchServiceType.Text))
            {
                dbFunction.SetMessageBox("Please choose a value for service type.", "Requeired field", clsFunction.IconType.iExclamation);
                return false;
            }

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantName, txtMerchantName.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantAddress, txtMerchantAddress.Text)) return false;
            

            //if (!dbFunction.isValidEntry(clsFunction.CheckType.iServiceType, txtSearchSTID.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iClientID, txtClientID.Text)) return false;
            //if (!dbFunction.isValidEntry(clsFunction.CheckType.iFEID, txtFEID.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantID, txtMerchantID.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iIRNo, txtSearchIRNo.Text)) return false;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTID, txtIRTID.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMID, txtIRMID.Text)) return false;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTIDLength, txtIRTID.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMIDLength, txtIRMID.Text)) return false;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRequestIDLength, txtEntryRequestID.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iReferenceNoLength, txtEntryReferenceNo.Text)) return false;

            //if (!dbFunction.isValidEntry(clsFunction.CheckType.iReferenceNo, txtReferenceNo.Text)) return false;
            //if (!dbFunction.isValidEntry(clsFunction.CheckType.iRequestBy, txtCustomerName.Text)) return false;
            //if (!dbFunction.isValidEntry(clsFunction.CheckType.iContactNo, txtCustomerContactNo.Text)) return false;
            //if (!dbFunction.isValidEntry(clsFunction.CheckType.iRemarks, txtRemarks.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTime, sReqTime)) return false;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRequestNo, txtRequestNo.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iReferenceNo, txtEntryReferenceNo.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRequestID, txtEntryRequestID.Text)) return false;

            // Region check
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRegion, txtMerchantRegion.Text)) return false;

            // Province check
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iProvince, txtMerchantCity.Text)) return false;

            // POS_Type
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iPOSType, txtPOSType.Text)) return false;

            // Vendor
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iVendor, txtVendor.Text)) return false;

            // Requestor
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRequestor, txtRequestor.Text)) return false;

            if (!dbAPI.isRecordExist("Search", "Region", txtMerchantRegion.Text)) {
                dbFunction.SetMessageBox("Region " + dbFunction.AddBracketStartEnd(txtMerchantRegion.Text) + " does not exist." + "\n\n" + 
                    "Update merchant information to continue.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return false;
            }
            // Region check

            // Validate Request ID
            if (!dbFunction.isValidIRNoPrefix(txtSearchIRNo.Text))
            {
                dbFunction.SetMessageBox("Invalid prefix for Request ID " + dbFunction.AddBracketStartEnd(txtSearchIRNo.Text) + "\n\n" + "Valid prefix " + dbFunction.AddBracketStartEnd(clsSystemSetting.ClassSystemIRNoPrefix), clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return false;
            }

            // Validate Request No
            if (!dbFunction.isValidIRNoPrefix(txtRequestNo.Text))
            {
                dbFunction.SetMessageBox("Invalid prefix for Request No " + dbFunction.AddBracketStartEnd(txtRequestNo.Text) + "\n\n" + "Valid prefix " + dbFunction.AddBracketStartEnd(clsSystemSetting.ClassSystemIRNoPrefix), clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return false;
            }

            // Validate Entry Requset ID
            if (!dbFunction.isValidIRNoPrefix(txtEntryRequestID.Text))
            {
                dbFunction.SetMessageBox("Invalid prefix for Request ID " + dbFunction.AddBracketStartEnd(txtEntryRequestID.Text) + "\n\n" + "Valid prefix " + dbFunction.AddBracketStartEnd(clsSystemSetting.ClassSystemIRNoPrefix), clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return false;
            }

            // Validate Ebtry Reference No
            if (!dbFunction.isValidIRNoPrefix(txtEntryReferenceNo.Text))
            {
                dbFunction.SetMessageBox("Invalid prefix for Reference No " + dbFunction.AddBracketStartEnd(txtEntryReferenceNo.Text) + "\n\n" + "Valid prefix " + dbFunction.AddBracketStartEnd(clsSystemSetting.ClassSystemIRNoPrefix), clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return false;
            }

            // check Request ID limit exceed
            string pJSONString = dbAPI.getInfoDetailJSON("Search", "Service RequestID Count", txtServiceJobType.Text + clsFunction.sPipe + txtIRIDNo.Text + clsFunction.sPipe + txtEntryRequestID.Text);
            int TCount = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TCount));
            if (TCount >= clsSystemSetting.ClassSystemJobOrderLimit)
            {
                dbFunction.SetMessageBox("Request ID " + dbFunction.AddBracketStartEnd(txtEntryRequestID.Text) + 
                                        " have reached its maximum usage limit of " + clsSystemSetting.ClassSystemJobOrderLimit + 
                                         ".", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return false;
            }
            

            // Check change on entry Request ID
            if (fEdit)
            {
                if (!pHoldEntryRequestID.Equals(txtEntryRequestID.Text))
                {
                    if (!dbFunction.fPromptConfirmation("You're about to change the value of Request ID" + "\n\n" +
                        " >From " + dbFunction.AddBracketStartEnd(pHoldEntryRequestID) + "\n" +
                        " >To " + dbFunction.AddBracketStartEnd(txtEntryRequestID.Text) + "\n\n" +
                        "Do you still want to continue?")) return false;
                }
            }

            // Check change on entry Reference No
            if (fEdit)
            {
                if (!pHoldEntryReferenceNo.Equals(txtEntryReferenceNo.Text))
                {
                    if (!dbFunction.fPromptConfirmation("You're about to change the value of Reference No" + "\n\n" +
                        " >From " + dbFunction.AddBracketStartEnd(pHoldEntryReferenceNo) + "\n" +
                        " >To " + dbFunction.AddBracketStartEnd(txtEntryReferenceNo.Text) + "\n\n" +
                        "Do you still want to continue?")) return false;
                }
            }
            
            if (fDispatch)
            {
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iDispatchID, dbFunction.CheckAndSetNumericValue(txtDispatchID.Text))) return false;
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iFEID, dbFunction.CheckAndSetNumericValue(txtFEID.Text))) return false;
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iAppVersion, txtFUAppVersion.Text)) return false;
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iAppCRC, txtFUAppCRC.Text)) return false;

                if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
                {
                    if (!dbFunction.isValidID(txtRepTerminalID.Text) && (!dbFunction.isValidID(txtRepSIMID.Text)))
                    {   
                        dbFunction.SetMessageBox("Replace Terminal or SIM should have a value.", "Warning", clsFunction.IconType.iExclamation);
                        return false;
                    }
                }
                else
                {
                    if (!dbFunction.isValidID(txtCurTerminalID.Text))
                    {
                        dbFunction.SetMessageBox("Current terminal must not be blank.", "Warning", clsFunction.IconType.iExclamation);
                        return false;
                    }
                }           
            }
            
            // check same current / replace terminal
            if (dbFunction.isValidID(txtCurTerminalID.Text) && dbFunction.isValidID(txtRepTerminalID.Text))
            {
                if (txtCurTerminalID.Text.Equals(txtRepTerminalID.Text))
                {
                    dbFunction.SetMessageBox("Current and replace terminal must not the same.", "Warning", clsFunction.IconType.iExclamation);
                    return false;
                }
            }

            // check same current / replace sim
            if (dbFunction.isValidID(txtCurSIMID.Text) && dbFunction.isValidID(txtRepSIMID.Text))
            {
                if (txtCurSIMID.Text.Equals(txtRepSIMID.Text))
                {
                    dbFunction.SetMessageBox("Current and replace SIM must not the same.", "Warning", clsFunction.IconType.iExclamation);
                    return false;
                }
            }

            if (!fEdit)
            {
                if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC))
                {
                    isBypassSNChecking = false;
                    if (!fDispatch)
                    {
                        if (!dbFunction.isValidID(txtCurTerminalID.Text) && !dbFunction.isValidID(txtCurSIMID.Text))
                        {
                            if (!dbFunction.fPromptConfirmation("Merchant information details: " + "\n\n" +
                            " > Request ID: " + txtEntryRequestID.Text + "\n" +
                            " > Reference No: " + txtEntryReferenceNo.Text + "\n" +
                            " > Name: " + txtMerchantName.Text + "\n" +
                            " > TID: " + txtIRTID.Text + "\n" +
                            " > MID: " + txtIRMID.Text + "\n" +
                            "\n" + "Terminal/SIM SN has not been set." + "\n\n" + "Do you still want to continue?"))
                            {
                                return false;
                            }
                        }

                        isBypassSNChecking = true;
                    }

                    if (!isBypassSNChecking)
                    {
                        // check terminal status
                        if (dbFunction.isValidID(txtCurTerminalStatus.Text))
                        {
                            if (int.Parse(txtCurTerminalStatus.Text) != clsGlobalVariables.STATUS_AVAILABLE)
                            {
                                dbFunction.SetMessageBox(clsDefines.HEADER_Current_Terminal + " SN must be available", "Warning", clsFunction.IconType.iError);
                                return false;
                            }
                        }

                        // check sim status
                        if (dbFunction.isValidID(txtCurSIMStatus.Text))
                        {
                            if (int.Parse(txtCurSIMStatus.Text) != clsGlobalVariables.STATUS_AVAILABLE)
                            {
                                dbFunction.SetMessageBox(clsDefines.HEADER_Current_SIM + " SN must be available", "Warning", clsFunction.IconType.iError);
                                return false;
                            }
                        }

                        // check terminal location
                        if (dbFunction.isValidID(txtCurTerminalLocationID.Text))
                        {
                            if (!dbAPI.isRecordExist("Search", "Valid SN Location", txtCurTerminalLocationID.Text))
                            {
                                dbFunction.SetMessageBox("Invalid " + clsDefines.HEADER_Current_Terminal + " location.", "Warning", clsFunction.IconType.iError);
                                return false;
                            }
                        }

                        // check sim location
                        if (dbFunction.isValidID(txtCurSIMLocationID.Text))
                        {
                            if (!dbAPI.isRecordExist("Search", "Valid SN Location", txtCurSIMLocationID.Text))
                            {
                                dbFunction.SetMessageBox("Invalid " + clsDefines.HEADER_Current_SIM + " location.", "Warning", clsFunction.IconType.iError);
                                return false;
                            }
                        }
                    }
                    
                }
                else if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
                {
                    // check terminal status
                    if (dbFunction.isValidID(txtRepTerminalStatus.Text))
                    {
                        if (int.Parse(txtRepTerminalStatus.Text) != clsGlobalVariables.STATUS_AVAILABLE)
                        {
                            dbFunction.SetMessageBox(clsDefines.HEADER_Replace_Terminal + " SN must be available", "Warning", clsFunction.IconType.iError);
                            return false;
                        }
                    }

                    // check sim status
                    if (dbFunction.isValidID(txtRepSIMStatus.Text))
                    {
                        if (int.Parse(txtRepSIMStatus.Text) != clsGlobalVariables.STATUS_AVAILABLE)
                        {
                            dbFunction.SetMessageBox(clsDefines.HEADER_Replace_SIM + " SN must be available", "Warning", clsFunction.IconType.iError);
                            return false;
                        }
                    }

                    // check terminal location
                    if (dbFunction.isValidID(txtRepTerminalLocationID.Text))
                    {
                        if (!dbAPI.isRecordExist("Search", "Valid SN Location", txtRepTerminalLocationID.Text))
                        {
                            dbFunction.SetMessageBox("Invalid " + clsDefines.HEADER_Replace_Terminal + " location.", "Warning", clsFunction.IconType.iError);
                            return false;
                        }
                    }

                    // check sim location
                    if (dbFunction.isValidID(txtRepSIMLocationID.Text))
                    {
                        if (!dbAPI.isRecordExist("Search", "Valid SN Location", txtRepSIMLocationID.Text))
                        {
                            dbFunction.SetMessageBox("Invalid " + clsDefines.HEADER_Replace_SIM + " location.", "Warning", clsFunction.IconType.iError);
                            return false;
                        }
                    }
                }
                else
                {
                    // check terminal location
                    if (dbFunction.isValidID(txtCurTerminalLocationID.Text))
                    {
                        if (!dbAPI.isRecordExist("Search", "Valid SN Location", txtCurTerminalLocationID.Text))
                        {
                            dbFunction.SetMessageBox("Invalid " + clsDefines.HEADER_Current_Terminal + " location.", "Warning", clsFunction.IconType.iError);
                            return false;
                        }
                    }

                    // check sim location
                    if (dbFunction.isValidID(txtCurSIMLocationID.Text))
                    {
                        if (!dbAPI.isRecordExist("Search", "Valid SN Location", txtCurSIMLocationID.Text))
                        {
                            dbFunction.SetMessageBox("Invalid " + clsDefines.HEADER_Current_SIM + " location.", "Warning", clsFunction.IconType.iError);
                            return false;
                        }
                    }
                }

            }

            // ----------------------------------------------------------------------------------------
            // check for duplicate SN
            // ----------------------------------------------------------------------------------------
            if (dbFunction.isValidID(txtCurTerminalID.Text))
            {
                if (!dbAPI.isDuplicateSN(int.Parse(txtCurTerminalID.Text), txtCurTerminalSN.Text, int.Parse(txtIRIDNo.Text), clsDefines.SEARCH_TERMINAL))
                    return false;
            }

            if (dbFunction.isValidID(txtCurSIMID.Text))
            {
                if (!dbAPI.isDuplicateSN(int.Parse(txtCurSIMID.Text), txtCurSIMSN.Text, int.Parse(txtIRIDNo.Text), clsDefines.SEARCH_SIM))
                    return false;
            }

            if (dbFunction.isValidID(txtRepTerminalID.Text))
            {
                if (!dbAPI.isDuplicateSN(int.Parse(txtRepTerminalID.Text), txtRepTerminalSN.Text, int.Parse(txtIRIDNo.Text), clsDefines.SEARCH_TERMINAL))
                    return false;
            }

            if (dbFunction.isValidID(txtRepSIMID.Text))
            {
                if (!dbAPI.isDuplicateSN(int.Parse(txtRepSIMID.Text), txtRepSIMSN.Text, int.Parse(txtIRIDNo.Text), clsDefines.SEARCH_SIM))
                    return false;
            }

            if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC) && !fEdit)
            {
                if (dbFunction.isValidID(txtCurTerminalID.Text))
                {
                    if (dbAPI.isRecordExist("Search", "Duplicate Assign TerminalSN", txtCurTerminalID.Text))
                    {
                        dbFunction.SetMessageBox("Terminal SN " + dbFunction.AddBracketStartEnd(txtCurTerminalSN.Text) + " already used." + "\n\n" + "Service Type: " + dbFunction.AddBracketStartEnd(txtSearchSTJobTypeDescription.Text) + "\n\n" + clsDefines.CONTACT_ADMIN_MESSAGE, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                        return false;
                    }
                        
                }

                if (dbFunction.isValidID(txtCurSIMID.Text))
                {
                    if (dbAPI.isRecordExist("Search", "Duplicate Assign SIMSN", txtCurSIMID.Text))
                    {
                        dbFunction.SetMessageBox("SIM SN " + dbFunction.AddBracketStartEnd(txtCurSIMSN.Text) + " already used." + "\n\n" + "Service Type: " + dbFunction.AddBracketStartEnd(txtSearchSTJobTypeDescription.Text) + "\n\n" + clsDefines.CONTACT_ADMIN_MESSAGE, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                        return false;
                    }

                }
                
            }

            if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC) && !fEdit)
            {   
                if (dbFunction.isValidID(txtRepTerminalID.Text))
                {
                    if (dbAPI.isRecordExist("Search", "Duplicate Assign TerminalSN", txtRepTerminalID.Text))
                    {
                        dbFunction.SetMessageBox("Terminal SN " + dbFunction.AddBracketStartEnd(txtRepTerminalSN.Text) + " already used." + "\n\n" + "Service Type: " + dbFunction.AddBracketStartEnd(txtSearchSTJobTypeDescription.Text) + "\n\n" + clsDefines.CONTACT_ADMIN_MESSAGE, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                        return false;
                    }

                }

                if (dbFunction.isValidID(txtRepSIMID.Text))
                {
                    if (dbAPI.isRecordExist("Search", "Duplicate Assign SIMSN", txtRepSIMID.Text))
                    {
                        dbFunction.SetMessageBox("SIM SN " + dbFunction.AddBracketStartEnd(txtRepSIMSN.Text) + " already used." + "\n\n" + "Service Type: " + dbFunction.AddBracketStartEnd(txtSearchSTJobTypeDescription.Text) + "\n\n" + clsDefines.CONTACT_ADMIN_MESSAGE, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                        return false;
                    }

                }

            }

            // ----------------------------------------------------------------------------------------
            // check for duplicate SN
            // ----------------------------------------------------------------------------------------

            // confirmation for target installation and schedule date
            if ((txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC)) &&
                !fEdit)
            {
                if (!isConfirmTargetInstDate()) return false;
            }

            // check if merchant is installed
            if ((txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC)) &&
                !fEdit)
            {
                if (!isMerchantInstalled()) return false;
            }

            // check problem reported
            if ((txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_SERVICING_DESC) ||
                txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC)) &&
                !dbFunction.isValidDescription(txtProbReported.Text))
            {
                tabFillUp.TabIndex = 0;
                dbFunction.SetMessageBox("Problem reported must not be blank", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);
                
                return false;
            }

            // check components
            if ((txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC) && !dbFunction.isValidCount(lvwStockDetail.Items.Count)) ||
                    txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC) && !dbFunction.isValidCount(lvwRepStockDetail.Items.Count))
            {
                tabFillUp.TabIndex = 2;
                if (!dbFunction.fPromptConfirmation("Job order " + dbFunction.AddBracketStartEnd(cboSearchServiceType.Text) + "\n\n" + "Additional components has not been set in " + "\n" +
                    (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC) ? "Current component tab" : "Replaced component tab") +
                    "\n\n" + "Do you still want to continue?"))
                {
                    return false;
                }
            }

            // check component max limit
            if (dbFunction.isValidCount(lvwStockDetail.Items.Count) || dbFunction.isValidCount(lvwRepStockDetail.Items.Count))
            {
                if (lvwStockDetail.Items.Count > clsSystemSetting.ClassSystemComponentMaxLimit)
                {
                    dbFunction.SetMessageBox("Current component limit reach." + "\n" + "Component limit is " + clsSystemSetting.ClassSystemComponentMaxLimit, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return false;
                }

                if (lvwRepStockDetail.Items.Count > clsSystemSetting.ClassSystemComponentMaxLimit)
                {
                    dbFunction.SetMessageBox("Replace component limit reach." + "\n" + "Component limit is " + clsSystemSetting.ClassSystemComponentMaxLimit, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return false;
                }
            }

            // check service chaanges
            if (dbFunction.isValidCount(lvwChanges.Items.Count))
            {
                if (!dbFunction.isValidCount(lvwProfile1.Items.Count))
                {
                    dbFunction.SetMessageBox("Unable to process service changes information." + "\n" + "Profile 1 must not be blank", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return false;
                }

                if (!dbFunction.isValidCount(lvwProfile2.Items.Count))
                {
                    dbFunction.SetMessageBox("Unable to process service changes information."+"\n"+"Profile 2 must not be blank", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return false;
                }

                if (!dbFunction.isValidCount(lvwRaw.Items.Count))
                {
                    dbFunction.SetMessageBox("Unable to process service changes information." + "\n" + "Raw data must not be blank", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return false;
                }
            }

            // check billing type
            if (clsSearch.ClassIsBillType > 0)
            {
                if (!dbFunction.isValidDescriptionEntry(cboBillingType.Text, "Billing Type" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            }

            // check replacement SN's
            if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
            {
                if ((!dbFunction.isValidID(txtRepTerminalID.Text)) && (!dbFunction.isValidID(txtRepSIMID.Text)))
                {
                    dbFunction.SetMessageBox(
                        $"Unable to save Job Order [{cboSearchServiceType.Text}]. Please provide at least one for SN replacement: Terminal or SIM.",
                        clsDefines.FIELD_CHECK_MSG,
                        clsFunction.IconType.iError
                    );
                    return false;
                }
            }

            return true;

        }

        private void SaveServiceDetail()
        {
            Debug.WriteLine("--SaveServiceDetail--");

            string sSQL = "";

            //DateTime SCDateTime = DateTime.Now;
            //string sDateTime = "";

            //string sReqDate = dbFunction.GetDateFromParse(dteReqInstallationDate.Text, "MM-dd-yyyy H:mm:ss tt", "yyyy-MM-dd");
            //string sReqTime = dbFunction.GetDateFromParse(dteReqTime.Text, "h:mm:ss tt", "HH:mm:ss");
            //string sServiceReqTime = dbFunction.GetDateFromParse(txtCreatedTime.Text, "HH:mm:ss", "HH:mm:ss");
            int isBillable = (chkBillable.Checked == true ? 1 : 0);
            int isPrimary = (lvwList.Items.Count > 0 ? 0 : 1);

            // dteReqInstallationDate
            DateTime stReqInstallationDate = dteReqInstallationDate.Value;
            DateTime stReqInstallationTime = dteReqTime.Value;
            string pReqInstallationDate = stReqInstallationDate.ToString("yyyy-MM-dd");
            string pReqInstallationTime = stReqInstallationTime.ToString("HH:mm:ss");

            // dteServiceReqDate
            DateTime stServiceReqDate = dteServiceReqDate.Value;
            DateTime stServiceReqTime = dteReqTime.Value;
            string pServiceReqDate = stServiceReqDate.ToString("yyyy-MM-dd");
            string pServiceReqTime = stServiceReqTime.ToString("HH:mm:ss");
            
            bool isDispatch = (chkDispatch.Checked ? true : false);

            dbFunction.GetProcessedByAndDateTime(); // Get processedby and datetime

            dbFunction.GetModifiedByAndDateTime(); // Get modifiedby and datetime

            InitCreatedDateTime();

            // Service Created
            //string pServiceDateTime = dbFunction.getCurrentDateTime();
            //string pServiceDate = dbFunction.getCurrentDate();
            //string pServiceTime = dbFunction.getCurrentTime();

            clsSearch.ClassJobType = int.Parse(txtSearchSTJobType.Text);
            clsSearch.ClassJobTypeDescription = txtSearchSTJobTypeDescription.Text;
            clsSearch.ClassJobTypeSubDescription = txtSearchSTDescription.Text;

            //sDateTime = SCDateTime.ToString("yyyy-MM-dd H:mm:ss");
            
            // Create Group Details - ROCKY BANTOLO
            var data = new
            {
                TAIDNo = dbFunction.CheckAndSetNumericValue(txtSearchTAIDNo.Text),
                ClientID = dbFunction.CheckAndSetNumericValue(txtClientID.Text),
                MerchantID = dbFunction.CheckAndSetNumericValue(txtMerchantID.Text),
                FEID = dbFunction.CheckAndSetNumericValue(txtFEID.Text),
                ClientName = dbFunction.CheckAndSetStringValue(txtClientName.Text),
                MerchantName = dbFunction.CheckAndSetStringValue(txtMerchantName.Text),
                FEName = dbFunction.CheckAndSetStringValue(txtFEName.Text),
                IRIDNo = dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text),
                SearchIRNo = dbFunction.CheckAndSetNumericValue(txtEntryRequestID.Text),
                CurTerminalID = dbFunction.CheckAndSetNumericValue(txtCurTerminalID.Text),
                CurTerminalSN = dbFunction.CheckAndSetStringValue(txtCurTerminalSN.Text),
                CurSIMID = dbFunction.CheckAndSetNumericValue(txtCurSIMID.Text),
                CurSIMSN = dbFunction.CheckAndSetStringValue(txtCurSIMSN.Text),
                CurDockID = dbFunction.CheckAndSetNumericValue(txtCurDockID.Text),
                CurDockSN = dbFunction.CheckAndSetStringValue(txtCurDockSN.Text),
                Counter = dbFunction.CheckAndSetStringValue(txtCounter.Text),
                RequestNo = dbFunction.CheckAndSetStringValue(txtRequestNo.Text),
                ServiceDateTime = dbFunction.CheckAndSetStringValue(pReqInstallationDate + " " + pReqInstallationTime),
                ServiceDate = dbFunction.CheckAndSetStringValue(pReqInstallationDate),
                ServiceTime = dbFunction.CheckAndSetStringValue(pReqInstallationTime),
                CustomerName = dbFunction.CheckAndSetStringValue(txtCustomerName.Text),
                CustomerNo = dbFunction.CheckAndSetStringValue(txtCustomerContactNo.Text),
                CustomerPosition = dbFunction.CheckAndSetStringValue(txtCustomerPosition.Text),
                CustomerEmail = dbFunction.CheckAndSetStringValue(txtCustomerEmail.Text),
                Remarks = dbFunction.CheckAndSetStringValue(txtRemarks.Text),
                ServiceReqDate = dbFunction.CheckAndSetStringValue(pServiceReqDate),
                ServiceReqTime = dbFunction.CheckAndSetStringValue(pServiceReqTime),
                LastServiceRequest = dbFunction.CheckAndSetStringValue(clsSearch.ClassLastServiceRequest),
                NewServiceRequest = dbFunction.CheckAndSetStringValue(txtNewServiceRequest.Text),
                RepTerminalID = dbFunction.CheckAndSetNumericValue(txtRepTerminalID.Text),
                RepTerminalSN = dbFunction.CheckAndSetStringValue(txtRepTerminalSN.Text),
                RepSIMID = dbFunction.CheckAndSetNumericValue(txtRepSIMID.Text),
                RepSIMSN = dbFunction.CheckAndSetStringValue(txtRepSIMSN.Text),
                RepDockID = dbFunction.CheckAndSetNumericValue(txtRepDockID.Text),
                RepDockSN = dbFunction.CheckAndSetStringValue(txtRepDockSN.Text),
                JobType = dbFunction.CheckAndSetStringValue(clsSearch.ClassJobType.ToString()),
                JobTypeDescription = dbFunction.CheckAndSetStringValue(clsSearch.ClassJobTypeDescription),
                JobTypeSubDescription = dbFunction.CheckAndSetStringValue(clsSearch.ClassJobTypeSubDescription),
                Dispatch = dbFunction.CheckAndSetStringValue(chkDispatch.Checked ? clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING_DESC : clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC),
                ReferenceNo = dbFunction.CheckAndSetStringValue(txtEntryReferenceNo.Text),
                RegionID = dbFunction.CheckAndSetNumericValue(txtRegionID.Text),
                RegionType = dbFunction.CheckAndSetNumericValue(txtRegionType.Text),
                Billable = dbFunction.CheckAndSetNumericValue(chkBillable.Checked ? "1" : "0"),
                Primary = dbFunction.CheckAndSetNumericValue(lvwList.Items.Count > 0 ? "0" : "1"),
                AppVersion = dbFunction.CheckAndSetStringValue(txtFUAppVersion.Text),
                AppCRC = dbFunction.CheckAndSetStringValue(txtFUAppCRC.Text),
                DispatchID = dbFunction.CheckAndSetNumericValue(txtDispatchID.Text),
                DispatchBy = dbFunction.CheckAndSetStringValue(txtDispatcher.Text),
                DispatchDateTime = dbFunction.CheckAndSetStringValue(chkDispatch.Checked ? dbFunction.getCurrentDateTime() : clsFunction.sDash),
                DispatchDate = dbFunction.CheckAndSetStringValue(chkDispatch.Checked ? dbFunction.getCurrentDate() : clsFunction.sDash),
                DispatchTime = dbFunction.CheckAndSetStringValue(chkDispatch.Checked ? dbFunction.getCurrentTime() : clsFunction.sDash),
                ProcessedBy = dbFunction.CheckAndSetStringValue(clsUser.ClassProcessedBy),
                ModifiedBy = dbFunction.CheckAndSetStringValue(clsUser.ClassModifiedBy),
                ProcessedDateTime = dbFunction.CheckAndSetStringValue(clsUser.ClassProcessedDateTime),
                ModifiedDateTime = dbFunction.CheckAndSetStringValue(clsUser.ClassModifiedDateTime),
                ReqInstallationDate = dbFunction.CheckAndSetStringValue(pReqInstallationDate),
                ProblemReported = dbFunction.CheckAndSetStringValue(txtProbReported.Text),
                RMInstruction = dbFunction.CheckAndSetStringValue(txtRMInstruction.Text),
                SourceID = clsSearch.ClassSourceID,
                CategoryID = clsSearch.ClassCategoryID,
                SubCategoryID = clsSearch.ClassSubCategoryID,
                Vendor = dbFunction.CheckAndSetStringValue(txtVendor.Text),
                Requestor = dbFunction.CheckAndSetStringValue(txtRequestor.Text),
                BillimgTypeID = clsSearch.ClassBillingTypeID,
            };

            sSQL = IFormat.Insert(data);
            
            Debug.WriteLine("SaveServiceDetail" + sSQL);

            dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 0);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "Servicing Detail", sSQL, "InsertCollectionDetail");

            clsSearch.ClassLastInsertedID = clsLastID.ClassLastInsertedID;
            txtSearchServiceNo.Text = clsSearch.ClassLastInsertedID.ToString();
            
        }
        
        private void InitCreatedDateTime()
        {
            DateTime ProcessDateTime = DateTime.Now;
            string sProcessDate = "";
            string sProcessTime = "";

            sProcessDate = ProcessDateTime.ToString("yyyy-MM-dd");
            sProcessTime = ProcessDateTime.ToString("HH:mm:ss");

            txtCreatedDate.Text = sProcessDate;
            txtCreatedTime.Text = sProcessTime;
        }

        private void btnSearchTerminalSN_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iTerminal;
            frmSearchField.iStatus = clsGlobalVariables.STATUS_AVAILABLE;
            frmSearchField.sTerminalType = "View Terminal";
            frmSearchField.sHeader = "VIEW TERMINAL";
            frmSearchField.isCheckBoxes = false;
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

                txtRepTerminalID.Text = clsSearch.ClassTerminalID.ToString();
                txtRepTerminalSN.Text = clsSearch.ClassTerminalSN;
                txtRepTerminalType.Text = clsSearch.ClassTerminalType;
                txtRepTerminalModel.Text = clsSearch.ClassTerminalModel;
                txtRepTerminalBrand.Text = clsSearch.ClassTerminalBrand;

                txtRepTerminalStatus.Text = clsSearch.ClassTerminalStatus.ToString();

            }
        }
        
        private void btnSearchSIMSN_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iSIM;
            frmSearchField.iStatus = clsGlobalVariables.STATUS_AVAILABLE;
            frmSearchField.sHeader = "VIEW SIM";
            frmSearchField.isCheckBoxes = false;
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

                txtRepSIMID.Text = clsSearch.ClassSIMID.ToString();
                txtRepSIMSN.Text = clsSearch.ClassSIMSerialNo;
                txtRepSIMCarrier.Text = clsSearch.ClassSIMCarrier;

                txtRepSIMStatus.Text = clsSearch.ClassSIMStatus.ToString();
            }
        }

        private void btnSearchDockSN_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iTerminal;
            frmSearchField.iStatus = clsGlobalVariables.STATUS_AVAILABLE;
            frmSearchField.sTerminalType = "View Dock";
            frmSearchField.sHeader = "VIEW DOCK";
            frmSearchField.isCheckBoxes = false;
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

                txtRepDockID.Text = clsSearch.ClassTerminalID.ToString();
                txtRepDockSN.Text = clsSearch.ClassTerminalSN;

                txtRepDockStatus.Text = clsSearch.ClassTerminalStatus.ToString();
            }
        }
        
        private void InitCurrentAndReplaceField()
        {
            txtRepTerminalSN.BackColor = clsFunction.DisableBackColor;
            txtRepSIMSN.BackColor = clsFunction.DisableBackColor;
            txtRepDockSN.BackColor = clsFunction.DisableBackColor;

            if (iSearchJobType == clsAPI.JobType.iReplacement)
            {
                if (dbFunction.isValidID(txtCurTerminalID.Text))
                    txtRepTerminalSN.BackColor = clsFunction.EntryBackColor;

                if (dbFunction.isValidID(txtCurSIMID.Text))
                    txtRepSIMSN.BackColor = clsFunction.EntryBackColor;

                if (dbFunction.isValidID(txtCurDockID.Text))
                    txtRepDockSN.BackColor = clsFunction.EntryBackColor;
            }
        }

        private void DefaultSelectedComboBoxValue()
        {
            cboSearchTerminalStatus.SelectedIndex = 0;
            cboSearchSIMStatus.SelectedIndex = 0;
            cboSearchDockStatus.SelectedIndex = 0;
        }

        private void frmServiceJobOrder_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
                case Keys.F1: // Select merchant
                    if (btnSearchMerchant.Enabled)
                        btnSearchMerchant_Click(this, e);
                    break;
                case Keys.F2: // Reload data
                    if (btnSearchService.Enabled)
                        btnSearchService_Click(this, e);
                    break;
                case Keys.F3: // Preview FSR
                    if (btnPreviewFSR.Enabled)
                        btnPreviewFSR_Click(this, e);
                    break;
                case Keys.F4: // Preview services
                    if (btnPreviewSvcHistory.Enabled)
                        btnPreviewSvcHistory_Click(this, e);
                    break;
                case Keys.F5: // Clear
                    btnClear_Click(this, e);
                    break;

                case Keys.F6: // frmImportTerminal
                    frmImportTerminal frm6 = new frmImportTerminal();
                    frm6.ShowDialog();
                    break;

                case Keys.F7: // frmImportSIM
                    frmImportSIM frm7 = new frmImportSIM();
                    frm7.ShowDialog();
                    break;
            }
        }

        private void txtRemarks_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }
        
        private void LoadServicingDetail()
        {
            int i = 0;
            int iLineNo = 0;

            Debug.WriteLine("--LoadServicingDetail--");

            lvwList.Enabled = true;
            lvwList.Items.Clear();

            clsSearch.ClassSearchValue = dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + clsFunction.sPipe +
                                                       clsFunction.sNull + clsFunction.sPipe +
                                                       clsFunction.sZero + clsFunction.sPipe +
                                                       clsFunction.sZero + clsFunction.sPipe +
                                                       clsFunction.sZero + clsFunction.sPipe +
                                                       dbFunction.CheckAndSetStringValue(clsSearch.ClassTID) + clsFunction.sPipe +
                                                       dbFunction.CheckAndSetStringValue(clsSearch.ClassMID);

            Debug.WriteLine("clsSearch.ClassSearchValue=" + clsSearch.ClassSearchValue);

            dbAPI.ExecuteAPI("GET", "View", "Servicing List", clsSearch.ClassSearchValue, "Servicing Detail", "", "ViewServicingDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.ServiceNo.Length > i)
                {
                    iLineNo++;

                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.ServiceNo[i].ToString());
                    item.SubItems.Add(clsArray.IRIDNo[i].ToString());
                    item.SubItems.Add(clsArray.TAIDNo[i].ToString());
                   
                    item.SubItems.Add(clsArray.ServiceJobTypeDescription[i].ToString());
                    item.SubItems.Add(clsArray.RequestNo[i].ToString());
                    item.SubItems.Add(clsArray.ReferenceNo[i].ToString());

                    // Servicing Date Info
                    string pJSONString = dbAPI.getInfoDetailJSON("Search", "Servicing Date Info", clsArray.ServiceNo[i].ToString());
                    dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);
                    
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_RequestDate));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_CreatedDate));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ScheduleDate));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_DispatchDate));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ServiceDate));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TicketDate));

                    item.SubItems.Add(clsArray.JobTypeStatusDescription[i].ToString());
                    item.SubItems.Add(clsArray.ServiceStatus[i].ToString());
                    
                    item.SubItems.Add(clsArray.ActionMade[i].ToString());
                    item.SubItems.Add(clsArray.FSRNo[i].ToString());

                    clsSearch.ClassLastServiceNo = int.Parse(clsArray.ServiceNo[i]);

                    item.SubItems.Add(clsArray.ProcessedBy[i]);
                    item.SubItems.Add(clsArray.ProcessedDateTime[i]);
                    item.SubItems.Add(clsArray.ModifiedBy[i]);
                    item.SubItems.Add(clsArray.ModifiedDateTime[i]);

                    item.SubItems.Add(clsArray.TerminalSN[i]);
                    item.SubItems.Add(clsArray.SIMSerialNo[i]);
                    item.SubItems.Add(clsArray.ReplaceTerminalSN[i]);
                    item.SubItems.Add(clsArray.ReplaceSIMSN[i]);

                    lvwList.Items.Add(item);

                    i++;

                }

                dbFunction.ListViewAlternateBackColor(lvwList);

                btnPreviewSvcHistory.Enabled = btnPreviewFSR.Enabled = btnViewDiagnostic.Enabled = true;

            }
        }
        
        //private void lvwList_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    return;

        //    if (lvwList.SelectedItems.Count > 0)
        //    {
        //        string LineNo = lvwList.SelectedItems[0].Text;
        //        txtLineNo.Text = LineNo;

        //        if (LineNo.Length > 0)
        //        {
        //            fSelected = true;
        //            clsSearch.ClassServiceNo = int.Parse(lvwList.SelectedItems[0].SubItems[1].Text);

        //            txtSearchServiceNo.Text = lvwList.SelectedItems[0].SubItems[1].Text;
        //            txtSearchServiceDate.Text = lvwList.SelectedItems[0].SubItems[4].Text;

        //            string sJobType = clsFunction.sDash;
        //            if (lvwList.SelectedItems[0].SubItems[5].Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC))
        //                sJobType = clsGlobalVariables.STATUS_INSTALLATION_DESC;

        //            if ((lvwList.SelectedItems[0].SubItems[5].Text.ToString().Equals(clsGlobalVariables.JOB_TYPE_SERVICING_DESC)) || (lvwList.SelectedItems[0].SubItems[5].Text.ToString().Equals(clsGlobalVariables.JOB_TYPE_DIAGNOSTIC_DESC)))
        //                sJobType = clsGlobalVariables.STATUS_SERVICING_DESC;

        //            if (lvwList.SelectedItems[0].SubItems[5].Text.Equals(clsGlobalVariables.JOB_TYPE_PULLOUT_DESC))
        //                sJobType = clsGlobalVariables.STATUS_PULLED_OUT_DESC;

        //            if (lvwList.SelectedItems[0].SubItems[5].Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
        //                sJobType = clsGlobalVariables.STATUS_REPLACEMENT_DESC;

        //            if (lvwList.SelectedItems[0].SubItems[5].Text.Equals(clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC))
        //                sJobType = clsGlobalVariables.STATUS_REPROGRAMMED_DESC;

        //            txtServiceJobType.Text = sJobType;
        //            txtServiceJobTypeDesc.Text = lvwList.SelectedItems[0].SubItems[5].Text;

        //            txtServiceRequestNo.Text = lvwList.SelectedItems[0].SubItems[6].Text;
        //            txtServiceReferenceNo.Text = lvwList.SelectedItems[0].SubItems[7].Text;
        //            txtServiceJobTypeStatusDesc.Text = lvwList.SelectedItems[0].SubItems[8].Text;
        //            txtServiceStatus.Text = lvwList.SelectedItems[0].SubItems[9].Text;
        //            txtServiceStatusDescription.Text = lvwList.SelectedItems[0].SubItems[10].Text;

        //            txtSearchFSRNo.Text = lvwList.SelectedItems[0].SubItems[11].Text;
        //            LoadFSRInfo();

        //            //MKTextBoxBackColor(true);
        //            PKTextBoxReadOnly(false);

        //            lvwList_DoubleClick(this, e);

                 
        //            //blSubHeader.Text = "SERVICE REQUEST - " + "[ " + txtServiceJobType.Text + " ]";

        //            //if (clsSearch.ClassJobTypeStatusDescription.CompareTo(clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC) == 0)
        //            //    btnDeleteJO.Enabled = true;
        //            //else
        //            //    btnDeleteJO.Enabled = false;
        //        }
        //    }
        //}
        
        private void txtCustomerContactNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void CheckReplacementTerminalDetails()
        {
            txtRepTerminalSN.BackColor = clsFunction.EntryBackColor;
            txtRepTerminalSN.ReadOnly = true;
            txtRepSIMSN.BackColor = clsFunction.EntryBackColor;
            txtRepSIMSN.ReadOnly = true;
            txtRepDockSN.BackColor = clsFunction.EntryBackColor;
            txtRepDockSN.ReadOnly = true;
        }

        

        private void CheckSerialComBoBox()
        {
            int iStatus = 0;

            InitComboBox(false);
            cboSearchTerminalStatus.Items.Clear();
            cboSearchSIMStatus.Items.Clear();
            cboSearchDockStatus.Items.Clear();

            if (dbFunction.isValidID(txtCurTerminalID.Text))
            {
                cboSearchTerminalStatus.Enabled = true;
                dbAPI.FillComboBoxTerminalStatus(cboSearchTerminalStatus);
                iStatus = int.Parse(txtCurTerminalID.Text);
                cboSearchTerminalStatus.Text = dbAPI.GetStatusDescription(iStatus);
            }

            if (dbFunction.isValidID(txtCurSIMID.Text))
            {
                cboSearchSIMStatus.Enabled = true;
                dbAPI.FillComboBoxTerminalStatus(cboSearchSIMStatus);
                iStatus = int.Parse(txtCurSIMID.Text);
                cboSearchSIMStatus.Text = dbAPI.GetStatusDescription(iStatus);
            }

            if (dbFunction.isValidID(txtCurDockID.Text))
            {
                cboSearchDockStatus.Enabled = true;
                dbAPI.FillComboBoxTerminalStatus(cboSearchDockStatus);
                iStatus = int.Parse(txtCurDockID.Text);
                cboSearchDockStatus.Text = dbAPI.GetStatusDescription(iStatus);
            }
        }

        private void InitComboBox(bool fEnable)
        {
            cboSearchTerminalStatus.Enabled = fEnable;
            cboSearchSIMStatus.Enabled = fEnable;
            cboSearchDockStatus.Enabled = fEnable;
        }

        
        private void txtCustomerName_KeyDown(object sender, KeyEventArgs e)
        {
            //switch (e.KeyCode)
            //{
            //    case Keys.Enter:
            //        if (txtCustomerName.Text.Length > 0)
            //            txtCustomerContactEmail.Focus();
            //        break;
            //}
        }

        private void txtCustomerContactNo_KeyDown(object sender, KeyEventArgs e)
        {
            //switch (e.KeyCode)
            //{
            //    case Keys.Enter:
            //        if (txtCustomerContactEmail.Text.Length > 0)
            //            txtRemarks.Focus();
            //        break;
            //}
        }

        private void btnDispatch_Click(object sender, EventArgs e)
        {
            bool isUpdateDispatch = false;            
            Debug.WriteLine("--btnDispatch_Click--");
            Debug.WriteLine("fEdit="+fEdit);

            try
            {
                // Check Application Version
                if (!dbAPI.isValidSystemVersion()) return;

                // Check User Access Rights
                if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isUpdate, clsUser.ClassUserID, 25)) return;

                // check service status
                if (dbFunction.isValidID(txtServiceStatus.Text))
                {
                    if (int.Parse(txtServiceStatus.Text).Equals(clsGlobalVariables.JOB_TYPE_HOLD))
                    {
                        dbFunction.SetMessageBox("Unable to process job order with [Hold Service] status." +
                                                "\n\n" +
                                                "Update the status under Menu->Service->Maintenance->Update Service Status", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);
                        return;
                    }
                    else if (int.Parse(txtServiceStatus.Text).Equals(clsGlobalVariables.JOB_TYPE_CANCEL))
                    {
                        dbFunction.SetMessageBox("Service already canceled. Processing is not allowed.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                        return;
                    }
                }

                string sTemp = clsFunction.sNull;

                isDispatch = (chkDispatch.Checked ? true : false);

                // Check Date Range (Schedule Date must be greater than or equal with request date(IR))
                if (!txtIRRequestDate.Text.Equals(clsFunction.sDateFormat))
                {
                    //DateTime dteRequest = DateTime.Parse(dbFunction.GetDateFromParse(txtIRRequestDate.Text, "yyyy-MM-dd", "yyyy-MM-dd"));
                    //DateTime dteSchedule = DateTime.Parse(dbFunction.GetDateFromParse(dteReqInstallationDate.Value.ToString(), "ddd, MM-dd-yyyy", "yyyy-MM-dd"));

                    //if (!CheckDateFromTo(dteRequest, dteSchedule)) return;

                    if (!dbFunction.checkDateFromTo(DateTime.Parse(dteReqInstallationDate.Value.ToShortDateString()), DateTime.Parse(dteServiceReqDate.Value.ToShortDateString())))
                    {
                        dbFunction.SetMessageBox("Request date must not greater than Schedule date", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                        return;
                    }

                    // eFSR Date Compare
                    //if (dteReqInstallationDate.Value > dteServiceReqDate.Value)
                    //{
                    //    MessageBox.Show("Request date should not be greater than the Schedule date.", clsDefines.FIELD_CHECK_MSG, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    return;
                    //}
                }

                if (!fEdit)
                {
                    // ----------------------------------------------------
                    // check schedule checking
                    // ----------------------------------------------------
                    if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC))
                    {
                        if (dteServiceReqDate.Value.Date < DateTime.Today)
                        {
                            if (!dbFunction.fPromptConfirmation($"The selected schedule date is in the past.\n\nAre you sure you want to continue?\n\n*Note:\nScheduling in the past can cause unexpected behavior, missed notification, inaccurate reports or incorrect data processing.")) return;
                        }
                    }
                    else
                    {
                        if (dteReqInstallationDate.Value.Date < DateTime.Today)
                        {
                            if (!dbFunction.fPromptConfirmation($"The selected request date is in the past.\n\nAre you sure you want to continue?\n\n*Note:\nScheduling in the past can cause unexpected behavior, missed notification, inaccurate reports or incorrect data processing.")) return;
                        }
                    }
                }

                if (!fEdit)
                {
                    if (!ValidateInstallationCount()) return;

                    // check valid reference no if not already exist
                    if (dbAPI.isRecordExist("Search", "Service RequestID", txtIRIDNo.Text + clsFunction.sPipe + txtSearchSTJobType.Text + clsFunction.sPipe + txtEntryRequestID.Text))
                    {
                        dbFunction.SetMessageBox("REQ ID " + dbFunction.AddBracketStartEnd(txtEntryRequestID.Text) + " already exist." + "\n\n" +
                                                 "Unable to proceed creation of job order", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                        return;
                    }

                    // check Merchant SN
                    if (int.Parse(txtSearchSTJobType.Text) != clsGlobalVariables.JOB_TYPE_INSTALLATION)

                        if (!isValidMerchantSN()) return;

                }
                else
                {
                    //if (dbFunction.isValidID(txtSearchFSRNo.Text))
                    //{

                    //    if (!dbFunction.fPromptConfirmation("Service is already completed." + "\n\n" + "Do you to stil want continue the update?")) 
                    //    return;

                    //}
                }

                // check service already completed
                if (txtServiceJobTypeStatusDesc.Text.Equals(clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC))
                {
                    dbFunction.SetMessageBox("Service already completed. Update not allowed." + "\n\n" + clsDefines.CONTACT_ADMIN_MESSAGE, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return;
                }

                if (!ValidateFields(isDispatch)) return;

                clsSearch.ClassServiceTypeDesc = cboSearchServiceType.Text;
                clsSearch.ClassRequestID = StrClean(txtEntryRequestID.Text);
                clsSearch.ClassRequestDate = dbFunction.CheckAndSetDatePickerValueToDate(dteReqInstallationDate);
                clsSearch.ClassScheduleDate = dbFunction.CheckAndSetDatePickerValueToDate(dteServiceReqDate);
                clsSearch.ClassServicedDate = txtSearchFSRDate.Text;
                clsSearch.ClassMerchantName = txtMerchantName.Text;
                clsSearch.ClassTID = txtIRTID.Text;
                clsSearch.ClassMID = txtIRMID.Text;
                clsSearch.ClassFEName = txtFEName.Text;
                clsSearch.ClassDispatcherName = txtDispatcher.Text;
                clsSearch.ClassRequestor = txtRequestor.Text;
                clsSearch.ClassIsDispatch = isDispatch;
                clsSearch.ClassTerminalSN = txtCurTerminalSN.Text;
                clsSearch.ClassSIMSerialNo = txtCurSIMSN.Text;
                clsSearch.ClassRepTerminalSN = txtRepTerminalSN.Text;
                clsSearch.ClassRepSIMSN = txtRepSIMSN.Text;
                clsSearch.ClassComponents = getComponentsDetail(lvwStockDetail);
                clsSearch.ClassRepComponents = getComponentsDetail(lvwRepStockDetail);
                clsSearch.ClassBillable = chkBillable.Checked ? true : false;

                dbFunction.GetIDFromFile("All Type", cboBillingType.Text);
                clsSearch.ClassBillingTypeID = clsSearch.ClassOutFileID;

                dbFunction.GetIDFromFile("All Type", cboSource.Text);
                clsSearch.ClassSourceID = clsSearch.ClassOutFileID;

                dbFunction.GetIDFromFile("All Type", cboCategory.Text);
                clsSearch.ClassCategoryID = clsSearch.ClassOutFileID;

                dbFunction.GetIDFromFile("All Type", cboSubCategory.Text);
                clsSearch.ClassSubCategoryID = clsSearch.ClassOutFileID;

                clsSearch.ClassJobType = int.Parse(dbFunction.CheckAndSetNumericValue(txtSearchSTJobType.Text));

                // ---------------------------------------------------------------
                // confirmation for fillup dispatcher/unchecked dispatch checkbox
                // ---------------------------------------------------------------
                if (dbFunction.isValidID(txtDispatchID.Text) &&
                    !dbFunction.isValidID(txtSearchFSRNo.Text) &&
                    !chkDispatch.Checked)
                {
                    if (!dbFunction.fPromptConfirmation(
                        $"Warning: You entered a Dispatcher [{txtDispatcher.Text}] but did not check the Dispatch option. Do you want to proceed with saving the Job Order?"
                    )) return;
                }

                // Service confirmation window
                frmServiceConfirmation frmServiceConfirmation = new frmServiceConfirmation();
                frmServiceConfirmation.gHeader = "JOB ORDER" + " " + clsDefines.gPipe + " " + lblMainStatus.Text;
                frmServiceConfirmation.ShowDialog();
                if (!frmServiceConfirmation.fSelected) return;

                Cursor.Current = Cursors.WaitCursor;

                if (chkDispatch.Checked)
                {
                    clsSearch.ClassStatus = clsGlobalVariables.STATUS_DISPATCH;
                    clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_DISPATCH_DESC;
                    clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING_DESC;
                }
                else
                {
                    clsSearch.ClassStatus = clsGlobalVariables.STATUS_ALLOCATED;
                    clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_ALLOCATED_DESC;
                    clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
                }

                // already serviced
                if (txtServiceJobTypeStatusDesc.Text.Equals(clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC) && dbFunction.isValidID(txtSearchFSRNo.Text))
                {
                    clsSearch.ClassStatus = clsGlobalVariables.STATUS_INSTALLED;
                    clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_INSTALLED_DESC;
                    clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC;
                }

                Debug.WriteLine("fEdit=" + fEdit);
                Debug.WriteLine("clsSearch.ClassStatus=" + clsSearch.ClassStatus);
                Debug.WriteLine("clsSearch.ClassStatusDescription=" + clsSearch.ClassStatusDescription);
                Debug.WriteLine("clsSearch.ClassJobTypeStatusDescription=" + clsSearch.ClassJobTypeStatusDescription);

                if (!fEdit)
                {
                    SaveServiceDetail();

                    // ---------------------------------------------------------------------------------------------
                    // Batch Update
                    // ---------------------------------------------------------------------------------------------     
                    clsSearch.ClassAdvanceSearchValue = txtSearchSTJobTypeDescription.Text + clsFunction.sPipe +
                                                        txtSearchServiceNo.Text + clsFunction.sPipe +
                                                        txtRequestNo.Text + clsFunction.sPipe +
                                                        clsSearch.ClassStatus + clsFunction.sPipe +
                                                        clsSearch.ClassStatusDescription + clsFunction.sPipe +
                                                        txtIRIDNo.Text + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtSearchServiceNo.Text) + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtFEID.Text) + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetStringValue(txtFEName.Text) + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtCurTerminalID.Text) + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtCurSIMID.Text) + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtRepTerminalID.Text) + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtRepSIMID.Text) + clsFunction.sPipe +
                                                        txtSearchSTCode.Text + clsFunction.sPipe;

                    Debug.WriteLine("Multiple Update->Value=" + clsSearch.ClassAdvanceSearchValue);

                    dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                    if (clsGlobalVariables.isAPIResponseOK)
                        dbAPI.ExecuteAPI("PUT", "Update", "Multiple Save Service", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                }
                else
                {
                    // dteReqInstallationDate
                    DateTime stReqInstallationDate = dteReqInstallationDate.Value;
                    DateTime stReqInstallationTime = dteReqTime.Value;
                    string pReqInstallationDate = stReqInstallationDate.ToString("yyyy-MM-dd");
                    string pReqInstallationTime = stReqInstallationTime.ToString("HH:mm:ss");

                    // dteServiceReqDate
                    DateTime stServiceReqDate = dteServiceReqDate.Value;
                    DateTime stServiceReqTime = dteReqTime.Value;
                    string pServiceReqDate = stServiceReqDate.ToString("yyyy-MM-dd");
                    string pServiceReqTime = stServiceReqTime.ToString("HH:mm:ss");

                    dbFunction.GetModifiedByAndDateTime(); // Get modifiedby and datetime

                    clsSearch.ClassAdvanceSearchValue = txtSearchSTJobTypeDescription.Text + clsFunction.sPipe +
                                                        txtSearchServiceNo.Text + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetStringValue(pReqInstallationDate) + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetStringValue(pReqInstallationTime) + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtFEID.Text) + clsFunction.sPipe +
                                                        txtFEName.Text + clsFunction.sPipe +
                                                        StrClean(txtEntryReferenceNo.Text) + clsFunction.sPipe +
                                                        txtCustomerName.Text + clsFunction.sPipe +
                                                        txtCustomerContactNo.Text + clsFunction.sPipe +
                                                        txtCustomerPosition.Text + clsFunction.sPipe +
                                                        StrClean(txtRemarks.Text) + clsFunction.sPipe +
                                                        txtFUAppVersion.Text + clsFunction.sPipe +
                                                        txtFUAppCRC.Text + clsFunction.sPipe +
                                                        clsUser.ModifiedBy + clsFunction.sPipe +
                                                        clsUser.ModifiedDateTime + clsFunction.sPipe +
                                                        (int.Parse(dbFunction.CheckAndSetNumericValue(txtSearchFSRNo.Text)) > 0 ? txtServiceJobTypeStatusDesc.Text : clsSearch.ClassJobTypeStatusDescription) + clsFunction.sPipe +
                                                        (int.Parse(dbFunction.CheckAndSetNumericValue(txtSearchFSRNo.Text)) > 0 ? txtServiceStatus.Text : clsSearch.ClassStatus.ToString()) + clsFunction.sPipe +
                                                        (int.Parse(dbFunction.CheckAndSetNumericValue(txtSearchFSRNo.Text)) > 0 ? txtServiceStatusDescription.Text : clsSearch.ClassStatusDescription) + clsFunction.sPipe;


                    if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC))
                    {
                        sTemp = dbFunction.CheckAndSetNumericValue(txtCurTerminalID.Text) + clsFunction.sPipe +
                                dbFunction.CheckAndSetNumericValue(txtOldTerminalID.Text) + clsFunction.sPipe +
                                clsSearch.ClassStatus + clsFunction.sPipe +
                                dbFunction.CheckAndSetNumericValue(txtCurSIMID.Text) + clsFunction.sPipe +
                                dbFunction.CheckAndSetNumericValue(txtOldSIMID.Text) + clsFunction.sPipe +
                                clsSearch.ClassStatus + clsFunction.sPipe;
                    }

                    if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
                    {
                        sTemp = dbFunction.CheckAndSetNumericValue(txtRepTerminalID.Text) + clsFunction.sPipe +
                                dbFunction.CheckAndSetNumericValue(txtOldTerminalID.Text) + clsFunction.sPipe +
                                clsSearch.ClassStatus + clsFunction.sPipe +
                                dbFunction.CheckAndSetNumericValue(txtRepSIMID.Text) + clsFunction.sPipe +
                                dbFunction.CheckAndSetNumericValue(txtOldSIMID.Text) + clsFunction.sPipe +
                                clsSearch.ClassStatus + clsFunction.sPipe;
                    }
                    else
                    {
                        sTemp = dbFunction.CheckAndSetNumericValue(txtCurTerminalID.Text) + clsFunction.sPipe +
                                dbFunction.CheckAndSetNumericValue(txtOldTerminalID.Text) + clsFunction.sPipe +
                                clsSearch.ClassStatus + clsFunction.sPipe +
                                dbFunction.CheckAndSetNumericValue(txtCurSIMID.Text) + clsFunction.sPipe +
                                dbFunction.CheckAndSetNumericValue(txtOldSIMID.Text) + clsFunction.sPipe +
                                clsSearch.ClassStatus + clsFunction.sPipe;
                    }

                    sTemp = sTemp + dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text);

                    Debug.WriteLine("sTemp=" + sTemp);
                    Debug.WriteLine("clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);
                    Debug.WriteLine("clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                    clsSearch.ClassAdvanceSearchValue =
                        clsSearch.ClassAdvanceSearchValue + sTemp + clsFunction.sPipe +
                        StrClean(txtProbReported.Text) + clsFunction.sPipe +
                        dbFunction.CheckAndSetStringValue(pReqInstallationDate) + clsFunction.sPipe +
                        StrClean(txtRMInstruction.Text) + clsFunction.sPipe +
                        txtCustomerEmail.Text + clsFunction.sPipe +
                        StrClean(txtEntryRequestID.Text) + clsFunction.sPipe +
                        dbFunction.CheckAndSetStringValue(pServiceReqDate) + clsFunction.sPipe +
                        dbFunction.CheckAndSetStringValue(pServiceReqTime) + clsFunction.sPipe +
                        clsSearch.ClassSourceID + clsFunction.sPipe +
                        clsSearch.ClassCategoryID + clsFunction.sPipe +
                        clsSearch.ClassSubCategoryID + clsFunction.sPipe +
                        dbFunction.CheckAndSetNumericValue(txtDispatchID.Text) + clsFunction.sPipe +
                        dbFunction.CheckAndSetStringValue(txtDispatcher.Text) + clsFunction.sPipe +
                        dbFunction.CheckAndSetStringValue(txtVendor.Text + clsFunction.sPipe +
                        dbFunction.CheckAndSetStringValue(txtRequestor.Text));

                    Debug.WriteLine("Final ->clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                    Debug.WriteLine("Multiple Update->Value=" + clsSearch.ClassAdvanceSearchValue);

                    dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                    dbAPI.ExecuteAPI("PUT", "Update", "Multiple Update Service", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                }

                Cursor.Current = Cursors.Default;

                if (chkEmail.Checked && chkDispatch.Checked)
                {
                    EmailNotification((fEdit ? "UPDATED: " : ""));
                }

                if (chkFSR.Checked)
                {
                    if (dbFunction.isValidID(txtSearchServiceNo.Text))
                        dbAPI.PreviewFSR(txtSearchIRNo.Text, txtRequestNo.Text, txtMerchantName.Text, txtIRTID.Text, txtIRMID.Text, sHeader,
                            int.Parse(dbFunction.CheckAndSetNumericValue(txtSearchServiceNo.Text)),
                            int.Parse(dbFunction.CheckAndSetNumericValue(txtSearchFSRNo.Text)),
                            int.Parse(dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text)), true); // Preview FSR Report
                }

                // save changes
                saveChangesDetail();

                // save stock movement detail
                deleteStockMovementDetail();
                saveStockMovementDetail(lvwStockDetail, true);
                saveStockMovementDetail(lvwRepStockDetail, false);

                dbAPI.BulkUpdateStockMovementDetail(lvwStockDetail, int.Parse(dbFunction.CheckAndSetNumericValue(txtClientID.Text)), isDispatch);
                dbAPI.BulkUpdateStockMovementDetail(lvwRepStockDetail, int.Parse(dbFunction.CheckAndSetNumericValue(txtClientID.Text)), isDispatch);

                // check if already dispatch
                if (!txtDispatchBy.Text.Equals(clsSearch.ClassCurrentParticularName) && dbFunction.isValidDescription(txtDispatchBy.Text) && fEdit)
                {
                    if (!dbFunction.fPromptConfirmation("You are about to update dispatcher " + dbFunction.AddBracketStartEnd(txtDispatchBy.Text) + "." + "\n" +
                                                         "Do you want to overwrite?"))
                    {
                        // do nothing
                        isUpdateDispatch = false;
                    }
                    else
                    {
                        isUpdateDispatch = true;
                    }
                }
                else
                {
                    isUpdateDispatch = true;
                }

                if (isUpdateDispatch)
                {
                    // update dispatch if edited
                    if (chkDispatch.Enabled && chkDispatch.Checked)
                    {
                        if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtIRIDNo.Text))
                        {
                            clsSearch.ClassAdvanceSearchValue = txtSearchServiceNo.Text + clsFunction.sPipe +
                                                                txtIRIDNo.Text + clsFunction.sPipe +
                                                                dbFunction.getCurrentDate() + clsFunction.sPipe +
                                                                dbFunction.getCurrentTime() + clsFunction.sPipe +
                                                                dbFunction.CheckAndSetStringValue(txtDispatcher.Text) + clsFunction.sPipe +
                                                                dbFunction.getCurrentDateTime() + clsFunction.sPipe +
                                                                dbFunction.CheckAndSetNumericValue(txtDispatchID.Text);

                            dbAPI.ExecuteAPI("PUT", "Update", "Update Servicing Dispatch", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
                        }
                    }
                }

                // Update AssistNo / ProblemNo by ServiceNo
                if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtAssistNo.Text) && dbFunction.isValidID(txtProblemNo.Text))
                {
                    dbAPI.ExecuteAPI("PUT", "Update", "Service Helpdesk", $"{txtSearchServiceNo.Text}{clsDefines.gPipe}{txtAssistNo.Text}{clsDefines.gPipe}{txtProblemNo.Text}", "", "", "UpdateCollectionDetail");
                }

                dbFunction.SetMessageBox(txtSearchSTServiceJobTypeDescription.Text + " service has been " + (fEdit ? "updated" : "saved") + " for" +
                    "\n\n" +
                    //"Primary Request ID >" + StrClean(txtRequestID1.Text) + "\n" +
                    "Request ID >" + StrClean(txtEntryRequestID.Text) + "\n" +
                    "Reference No. >" + StrClean(txtEntryReferenceNo.Text) + "\n" +
                    "Merchant Name >" + txtMerchantName.Text + "\n" +
                    "TID >" + txtIRTID.Text + "\n" +
                    "MID >" + txtIRMID.Text + "\n" +
                    "Region >" + txtMerchantRegion.Text + "\n" +
                    "City >" + txtMerchantCity.Text +
                    "\n\n" +
                   (chkEmail.Checked && chkDispatch.Checked ? "Service request emailed to vendor representative." : "")
                  , (fEdit ? "Job order updated" : "Job order saved") + (chkDispatch.Checked ? " & Dispatched" : ""), clsFunction.IconType.iInformation);

                btnClear_Click(this, e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceptional error message {ex.Message}");
                dbFunction.SetMessageBox($"Exceptional error message {ex.Message}", "Save: Job Order", clsFunction.IconType.iError);
            }            
        }

        private void btnAddFE_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantID, txtMerchantID.Text)) return;
            //if (!dbFunction.isValidEntry(clsFunction.CheckType.iClientID, txtClientID.Text)) return;

            frmSearchField.iSearchType = frmSearchField.SearchType.iFE;
            frmSearchField.sHeader = "VENDOR FIELD ENGINEER";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                // Load Information
                txtFEID.Text = clsSearch.ClassParticularID.ToString();
                txtFEName.Text = clsSearch.ClassParticularName;

                FillFETextBox();

                FillFEContactInfoTextBox();
                
            }
        }

        private void btnRemoveFE_Click(object sender, EventArgs e)
        {
            txtFEID.Text =
            txtFEName.Text =
            txtFEAddress.Text =
            txtFEEmail.Text = clsFunction.sNull;

            //initDispatch(false);
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {

        }

        //private void btnDelete_Click(object sender, EventArgs e)
        //{
        //    if (clsSearch.ClassServiceNo > 0)
        //    {

        //        string sTemp =
        //                   clsFunction.sLineSeparator + "\n" +
        //                   "Job Order Date/Time: " + clsSearch.ClassJobOrderDate + "\n" +
        //                   "Service Request ID: " + clsSearch.ClassServiceRequestID + "\n" +
        //                   "Request No: " + clsSearch.ClassIRNo + "\n" +
        //                   "Job Order Status: " + clsSearch.ClassJobTypeStatusDescription + "\n" +
        //                   clsFunction.sLineSeparator + "\n" +
        //                   "Merchant Name: " + txtMerchantName.Text + "\n" +
        //                   "TID: " + txtIRTID.Text + "\n" +
        //                   "MID: " + txtIRMID.Text + "\n" +
        //                   "Client Name :" + txtClientName.Text + "\n" +
        //                   clsFunction.sLineSeparator + "\n";

        //        if (MessageBox.Show("Do you really want to delete selected record(s)?\n" +
        //                           "\n\n" +
        //                            sTemp +
        //                            "\n\n" +
        //                           "Warning:\nData will permanently deleted." +
        //                           "\n",
        //                           "Confirm Delete " + lblHeader.Text.ToLower() + "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
        //        {
        //            return;
        //        }

        //        dbAPI.DeleteServicingDetail(clsSearch.ClassServiceNo.ToString());

        //        dbFunction.SetMessageBox("Job Order successfully deleted.", "Deleted", clsFunction.IconType.iInformation);

        //        LoadTAWithService();

        //        btnDeleteJO.Enabled = false;
        //    }
        //}

        private void GetServiceStatusAndDescription()
        {
            switch (iSearchJobType)
            {
                case clsAPI.JobType.iInstallation:
                    clsSearch.ClassServiceStatus = clsGlobalVariables.STATUS_INSTALLATION;
                    clsSearch.ClassServiceStatusDescription = clsGlobalVariables.STATUS_INSTALLATION_DESC;
                    break;
                case clsAPI.JobType.iPullOut:
                    clsSearch.ClassServiceStatus = clsGlobalVariables.STATUS_PULLED_OUT;
                    clsSearch.ClassServiceStatusDescription = clsGlobalVariables.STATUS_PULLED_OUT_DESC;
                    break;
                case clsAPI.JobType.iReplacement:
                    clsSearch.ClassServiceStatus = clsGlobalVariables.STATUS_REPLACEMENT;
                    clsSearch.ClassServiceStatusDescription = clsGlobalVariables.STATUS_REPLACEMENT_DESC;
                    break;
                case clsAPI.JobType.iReprogramming:
                    clsSearch.ClassServiceStatus = clsGlobalVariables.STATUS_REPROGRAMMED;
                    clsSearch.ClassServiceStatusDescription = clsGlobalVariables.STATUS_REPROGRAMMED_DESC;
                    break;
                case clsAPI.JobType.iServicing:
                    clsSearch.ClassServiceStatus = clsGlobalVariables.STATUS_SERVICING;
                    clsSearch.ClassServiceStatusDescription = clsGlobalVariables.STATUS_SERVICING_DESC;
                    break;
            }
        }

        private void txtRequestID_TextChanged(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void dteReqDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void gbCurrent_Enter(object sender, EventArgs e)
        {

        }

        private void txtReferenceNo_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (txtEntryReferenceNo.Text.Length > 0)
                        txtCustomerName.Focus();
                    break;
            }
        }

        private void txtReferenceNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtCustomerName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
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
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtSearchServiceNo.Text) + "', " +
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
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtCurTerminalSN.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtCurSIMSN.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtCurDockSN.Text) + "', " +
            sRowSQL + sRowSQL + " '" + clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC + "', " +
            sRowSQL + sRowSQL + " '" + clsGlobalVariables.FSR_REPORT_TYPE + "', " +
            sRowSQL + sRowSQL + " '" + clsGlobalVariables.FSR_REPORT_TYPE_DESC + "') ";
            sSQL = sSQL + sRowSQL;

            Debug.WriteLine("SaveEmailNotif::sSQL=" + sSQL);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", xMaintenanceType, sSQL, "InsertMaintenanceMaster");

        }

        private void ComposeSearchValue()
        {
            Debug.WriteLine("--ComposeSearchValue--");

            clsSearch.ClassSearchValue = clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sDateFormat + clsFunction.sPipe +
                                                clsFunction.sDateFormat + clsFunction.sPipe +
                                                clsFunction.sDateFormat + clsFunction.sPipe +
                                                clsFunction.sDateFormat + clsFunction.sPipe +
                                                clsFunction.sDateFormat + clsFunction.sPipe +
                                                clsFunction.sDateFormat + clsFunction.sPipe +
                                                clsFunction.sDateFormat + clsFunction.sPipe +
                                                clsFunction.sDateFormat + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsSearch.ClassTAIDNo + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe + // Region
                                                clsFunction.sZero + clsFunction.sPipe + // Province
                                                clsFunction.sZero + clsFunction.sPipe; // ServceNo

            Debug.WriteLine("ComposeSearchValue, clsSearch.ClassSearchValue=" + clsSearch.ClassSearchValue);
        }

        private void InitStatusTitle(bool isClear)
        {
            if (isClear)
            {
                //lblMainStatus.ForeColor = Color.Gray;
                lblMainStatus.Text = clsFunction.sDash;
            }
            else
            {
                if (dbFunction.isValidID(txtSearchServiceNo.Text) && fEdit)
                {
                    //lblMainStatus.ForeColor = Color.Cyan;
                    lblMainStatus.Text = "UPDATE JO";
                    lblHeader.Text = "UPDATE JOB ORDER " + cboSearchServiceType.Text;
                }
                else
                {
                    //lblMainStatus.ForeColor = Color.Yellow;
                    lblMainStatus.Text = "NEW JO";
                    lblHeader.Text = "CREATE JOB ORDER " + cboSearchServiceType.Text;
                }
            }

        }
       
        private void btnCancelJO_Click(object sender, EventArgs e)
        {
            bool isConfirm = false;

            // Check Application Version
            //if (!dbAPI.isValidSystemVersion()) return;

            //if (!dbAPI.isValidCancelService(txtSearchServiceNo.Text, txtSearchFSRNo.Text)) return;

            // Admin Login requirement
            if (!dbAPI.isPromptAdminLogIn()) return;

            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isDelete, clsUser.ClassUserID, 25)) return;

            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isDelete, clsUser.ClassUserID, 26)) return;

            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtIRIDNo.Text))
            {
                if (txtServiceJobTypeStatusDesc.Text.Equals(clsDefines.SERVICE_STATUS_COMPLETED))
                {
                    if ((txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC)) ||
                        (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC)) ||
                        (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_PULLOUT_DESC)))
                    {
                        dbFunction.SetMessageBox("Unable to delete completed service for\n[INSTALLATION/REPLACEMENT/PULL-OUT]" + "\n\n" + clsDefines.CONTACT_ADMIN_MESSAGE, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                        return;
                    }
                    else
                    {
                        if (!dbFunction.fPromptConfirmation("!!IMPORTANT!!" + "\n" +
                        "This service is already COMPLETED" + "\n\n" +
                        "Do you still want to cancel service?"))
                            return;
                    }
                }
                
                if (MessageBox.Show("Are you sure to cancel " + cboSearchServiceType.Text + " service" + " for \n" + txtMerchantName.Text + "." + 
                    "\n\n" +                   
                    "Warning:\nData will permanently deleted.", "Confirm?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    isConfirm = false;
                else
                    isConfirm = true;

                if (isConfirm)
                {
                    Cursor.Current = Cursors.WaitCursor;

                    if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC))
                    {
                        clsSearch.ClassAdvanceSearchValue = txtSearchSTJobTypeDescription.Text + clsFunction.sPipe +
                                                    txtSearchServiceNo.Text + clsFunction.sPipe +
                                                    txtIRIDNo.Text + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtClientID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtCurTerminalID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtCurSIMID.Text);
                    }
                    else if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
                    {
                        clsSearch.ClassAdvanceSearchValue = txtSearchSTJobTypeDescription.Text + clsFunction.sPipe +
                                                    txtSearchServiceNo.Text + clsFunction.sPipe +
                                                    txtIRIDNo.Text + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtClientID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtRepTerminalID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtRepSIMID.Text);
                    }
                    else
                    {
                        clsSearch.ClassAdvanceSearchValue = txtSearchSTJobTypeDescription.Text + clsFunction.sPipe +
                                                    txtSearchServiceNo.Text + clsFunction.sPipe +
                                                    txtIRIDNo.Text + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtClientID.Text) + clsFunction.sPipe +
                                                    clsFunction.sZero + clsFunction.sPipe +
                                                    clsFunction.sZero;
                    }

                    Debug.WriteLine("Cancel Service->Value=" + clsSearch.ClassAdvanceSearchValue);
                    dbAPI.ExecuteAPI("PUT", "Update", "Cancel Service", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                    Cursor.Current = Cursors.Default;

                    dbFunction.SetMessageBox("Service cancelled complete.", "Service cancelled", clsFunction.IconType.iInformation);

                    btnClear_Click(this, e);
                }
            
            }

            /*
            if (dbFunction.isValidID(txtSearchServiceNo.Text))
            {
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iReason, txtReasonID.Text)) return;
                
                if (txtServiceJobTypeStatusDesc.Text.Equals(clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC))
                {
                    dbFunction.SetMessageBox("Unable to cancel service that already completed.", "Service cancel", clsFunction.IconType.iExclamation);
                }
                else
                {
                    if (MessageBox.Show("Are you sure to cancel service?" + "\n\n" +
                        "Client:" + txtClientName.Text + "\n" +
                        "Merchant:" + txtMerchantName.Text + "\n" +
                        "Field Engineer:" + txtFEName.Text + "\n" +
                        clsFunction.sLineSeparator + "\n" +
                        "IR No:" + txtRequestNo.Text + "\n" +
                        "IR Reference No.:" + txtReferenceNo.Text + "\n" +
                        "IR Date:" + txtIRInstallationDate.Text + "\n" +
                        "IR Processed By:" + txtIRProcessedBy.Text + "\n" +
                        clsFunction.sLineSeparator + "\n" +
                        "Service Job Type:" + txtServiceJobType.Text + "\n" +
                        "Service Date:" + txtServiceDate.Text + "\n" +                        
                        "Service Request No.:" + txtServiceRequestID.Text + "\n" +                        
                        clsFunction.sLineSeparator + "\n" +
                        "Current SN's Details" + "\n" +
                        clsFunction.sLineSeparator + "\n" +
                        "Terminal SN:" + txtCurTerminalSN.Text + "\n" +
                        " > Type:" + txtCurTerminalType.Text + "\n" +
                        " > Model:" + txtCurTerminalModel.Text + "\n" +
                        " > Brand:" + txtCurTerminalBrand.Text + "\n" +
                        "SIM SN:" + txtCurSIMSN.Text + "\n" +
                        " > Carrier:" + txtCurSIMCarrier.Text + "\n" +
                        "Dock SN:" + txtCurDockSN.Text + "\n" +
                        clsFunction.sLineSeparator + "\n" +
                        "Replace With SN's Details" + "\n" +
                        clsFunction.sLineSeparator + "\n" +
                        "Terminal SN:" + txtRepTerminalSN.Text + "\n" +
                        " > Type:" + txtRepTerminalType.Text + "\n" +
                        " > Model:" + txtRepTerminalModel.Text + "\n" +
                        " > Brand:" + txtRepTerminalBrand.Text + "\n" +
                        "SIM SN:" + txtRepSIMSN.Text + "\n" +
                        " > Carrier:" + txtRepSIMCarrier.Text + "\n" +
                        "Dock SN:" + txtRepDockSN.Text + "\n" +
                        clsFunction.sLineSeparator + "\n" +
                        "Reason:" + txtReasonDescription.Text + "\n" +
                        "Remarks:" + txtRemarks.Text,
                        "Service cancellation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }

                    // Servicing Cancelled
                    clsSearch.ClassAdvanceSearchValue = txtSearchServiceNo.Text + clsFunction.sPipe +
                                                        clsFunction.sOne + clsFunction.sPipe +
                                                        clsGlobalVariables.STATUS_CANCELLED + clsFunction.sPipe +
                                                        clsGlobalVariables.STATUS_CANCELLED_DESC + clsFunction.sPipe +
                                                        clsGlobalVariables.JOB_TYPE_STATUS_CANCELLED_DESC;
                    Debug.WriteLine("clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                    dbAPI.ExecuteAPI("PUT", "Update", "Update Servicing Cancelled", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");


                    // Update FillUp
                    clsSearch.ClassSearchValue = txtSearchServiceNo.Text + clsFunction.sPipe +
                                                 txtCustomerName.Text + clsFunction.sPipe +
                                                 txtCustomerContactNo.Text + clsFunction.sPipe +
                                                 txtRemarks.Text + clsFunction.sPipe +
                                                 txtReferenceNo.Text + clsFunction.sPipe +
                                                 dbFunction.CheckAndSetNumericValue(txtReasonID.Text) + clsFunction.sPipe +
                                                 dbFunction.CheckAndSetNumericValue(txtFEID.Text) + clsFunction.sPipe +
                                                 txtFEName.Text;

                    Debug.WriteLine("clsSearch.ClassSearchValue=" + clsSearch.ClassSearchValue);

                    dbAPI.ExecuteAPI("PUT", "Update", "Update Servicing FillUp", clsSearch.ClassSearchValue, "", "", "UpdateCollectionDetail");

                    // Update Modified By
                    clsSearch.ClassSearchValue = "Servicing" + clsFunction.sPipe +
                                                    txtSearchServiceNo.Text + clsFunction.sPipe +
                                                    clsUser.ClassModifiedBy + clsFunction.sPipe +
                                                    clsUser.ClassModifiedDateTime;

                    Debug.WriteLine("UpdateModifiedBy::clsSearch.ClassSearchValue=" + clsSearch.ClassSearchValue);

                    dbAPI.ExecuteAPI("PUT", "Update", "Update Modified By", clsSearch.ClassSearchValue, "", "", "UpdateCollectionDetail");

                    dbFunction.SetMessageBox("Service cancelled complete.", "Service cancel", clsFunction.IconType.iInformation);

                    btnClear_Click(this, e);

                }
            }
            else
            {
                dbFunction.SetMessageBox("No service record to be cancelled.", "Service cancel", clsFunction.IconType.iExclamation);
            }
            */
        }

        private void UpdateButton(bool isClear)
        {
            if (isClear)
            {
                btnAdd.Enabled = true;
                btnDispatchJO.Enabled = false;               
                btnCancelJO.Enabled = false;
                btnSendEmail.Enabled = false;
            }
            else
            {
                if (fEdit)
                {
                    btnAdd.Enabled = false;
                    btnDispatchJO.Enabled = true;
                    
                }
                else
                {
                    btnAdd.Enabled = true;
                    btnDispatchJO.Enabled = false;
                }
            }
        }
        
        private void btnPreviewFSR_Click(object sender, EventArgs e)
        {
            if (!dbFunction.fPromptConfirmation("Are you sure to preview FSR report?")) return;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantName, txtMerchantName.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iService, cboSearchServiceType.Text)) return;

            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidDescription(txtSearchIRNo.Text) && dbFunction.isValidDescription(txtServiceJobType.Text))
            {
                // update signature
                dbAPI.updateSignature(int.Parse(txtSearchServiceNo.Text), int.Parse(txtSearchFSRNo.Text));

                // download signature
                dbFunction.downloadSignature(clsDefines.MERCHANT_SIGNATURE_INDEX, int.Parse(txtSearchServiceNo.Text));
                dbFunction.downloadSignature(clsDefines.VENDOR_SIGNATURE_INDEX, int.Parse(txtSearchServiceNo.Text));

                // Preview report
                clsSearch.ClassIsExportToPDF = false;                
                dbReportFunc.ViewFSR(5);
            }
            else
            {
                dbFunction.SetMessageBox("No selected service.", "Service", clsFunction.IconType.iExclamation);
            }
        }

        private void btnIRSearch_Click(object sender, EventArgs e)
        {

        }

        private void btnPreviewSvcHistory_Click(object sender, EventArgs e)
        {
            if (!dbFunction.fPromptConfirmation("Are you sure to preview Service History report?")) return;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iClientName, txtClientName.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantName, txtMerchantName.Text)) return;
            
            dbReportFunc.ViewServiceHistoryDetail(11);
        }

        private void btnClearService_Click(object sender, EventArgs e)
        {
            lvwList.Enabled = true;
            fSelected = false;
          
            //lblSubHeader.Text = "SERVICE REQUEST";
            UpdateButton(true);
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void cboSearchTerminalStatus_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboSearchSIMStatus_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        
        private void InitServiceHistoryListView()
        {
            string outField = "";
            int outWidth = 0;
            string outTitle = "";
            HorizontalAlignment outAlign = 0;
            bool outVisible = false;
            bool outAutoWidth = false;
            string outFormat = "";

            lvwList.Clear();
            lvwList.View = View.Details;

            dbFunction.GetListViewHeaderColumnFromFile("", "Line#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ServiceNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "IRIDNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "TAIDNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);
            
            dbFunction.GetListViewHeaderColumnFromFile("", "Service Type", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Request No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Reference No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);
            
            dbFunction.GetListViewHeaderColumnFromFile("", "Request Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Created Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Schedule Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Dispatch Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Service Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Ticket Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Stage", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "SVC Status", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Service Result", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "FSRNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Process By", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Process Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Modify By", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Modify Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "TerminalSN", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "SIMSN", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ReplaceTerminalSN", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ReplaceSIMSN", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

        }
        
        private bool isValidServiceRequest()
        {
            bool isValid = true;

            if (!lblMainStatus.Text.Equals(clsFunction.sDash) && lblMainStatus.Text.Substring(0, 3).Equals("NEW"))
            {
                clsSearch.ClassSearchValue = txtIRIDNo.Text + clsFunction.sPipe;
                if (dbAPI.isRecordExist("Search", "Incomplete Service", clsSearch.ClassSearchValue))
                {
                    if (MessageBox.Show("A pending request found on this merchant.\n\nDo you still want to continue?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        isValid = false;
                    }                        
                }
            }
            else
            {
                isValid = true;
            }
            
            return isValid;
        }

        private void UpdateCurrentSNStatus()
        {
            Debug.WriteLine("--UpdateCurrentSNStatus--");
            Debug.WriteLine("isDispatch=" + isDispatch);

            if (isDispatch)
            {
                iStatus = clsGlobalVariables.STATUS_DISPATCH;
                sStatusDesc = clsGlobalVariables.STATUS_DISPATCH_DESC;
            }
            else
            {
                iStatus = clsGlobalVariables.TA_STATUS_INSTALLED;
                sStatusDesc = clsGlobalVariables.STATUS_INSTALLED_DESC;
            }

            Debug.WriteLine("iStatus=" + iStatus);
            Debug.WriteLine("sStatusDesc=" + sStatusDesc);

            if (dbFunction.isValidID(txtCurTerminalID.Text))
            {
                clsSearch.ClassSearchValue = txtCurTerminalID.Text + clsFunction.sPipe +
                                             iStatus + clsFunction.sPipe +
                                             sStatusDesc;

                dbAPI.ExecuteAPI("PUT", "Update", "Update Terminal Detail Status", clsSearch.ClassSearchValue, "", "", "UpdateCollectionDetail");
            }

            if (dbFunction.isValidID(txtCurSIMID.Text))
            {
                clsSearch.ClassSearchValue = txtCurSIMID.Text + clsFunction.sPipe +
                                             iStatus + clsFunction.sPipe +
                                             sStatusDesc;

                dbAPI.ExecuteAPI("PUT", "Update", "Update SIM Detail Status", clsSearch.ClassSearchValue, "", "", "UpdateCollectionDetail");
            }

            if (dbFunction.isValidID(txtCurDockID.Text))
            {
                clsSearch.ClassSearchValue = txtCurDockID.Text + clsFunction.sPipe +
                                             iStatus + clsFunction.sPipe +
                                             sStatusDesc;

                dbAPI.ExecuteAPI("PUT", "Update", "Update Dock Detail Status", clsSearch.ClassSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        private void UpdateReplaceSNStatus()
        {
            Debug.WriteLine("--UpdateReplaceSNStatus--");
            Debug.WriteLine("isDispatch=" + isDispatch);

            if (isDispatch)
            {
                iStatus = clsGlobalVariables.STATUS_DISPATCH;
                sStatusDesc = clsGlobalVariables.STATUS_DISPATCH_DESC;
            }
            else
            {
                iStatus = clsGlobalVariables.STATUS_ALLOCATED;
                sStatusDesc = clsGlobalVariables.STATUS_ALLOCATED_DESC;
            }

            Debug.WriteLine("iStatus=" + iStatus);
            Debug.WriteLine("sStatusDesc=" + sStatusDesc);

            if (dbFunction.isValidID(txtRepTerminalID.Text))
            {
                clsSearch.ClassSearchValue = txtRepTerminalID.Text + clsFunction.sPipe +
                                             iStatus + clsFunction.sPipe +
                                             sStatusDesc;

                dbAPI.ExecuteAPI("PUT", "Update", "Update Terminal Detail Status", clsSearch.ClassSearchValue, "", "", "UpdateCollectionDetail");
            }

            if (dbFunction.isValidID(txtRepSIMID.Text))
            {
                clsSearch.ClassSearchValue = txtRepSIMID.Text + clsFunction.sPipe +
                                             iStatus + clsFunction.sPipe +
                                             sStatusDesc;

                dbAPI.ExecuteAPI("PUT", "Update", "Update SIM Detail Status", clsSearch.ClassSearchValue, "", "", "UpdateCollectionDetail");
            }

            if (dbFunction.isValidID(txtRepDockID.Text))
            {
                clsSearch.ClassSearchValue = txtRepDockID.Text + clsFunction.sPipe +
                                             iStatus + clsFunction.sPipe +
                                             sStatusDesc;

                dbAPI.ExecuteAPI("PUT", "Update", "Update Dock Detail Status", clsSearch.ClassSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        private void LoadFSRInfo()
        {
            // Clear
            txtSearchFSRDate.Text = clsFunction.sDateDefault;
            txtSearchFSRTimeArrived.Text = clsFunction.sInvalidTime;
            txtSearchFSRReceiptTime.Text = clsFunction.sInvalidTime;
            txtSearchFSRTimeStart.Text = clsFunction.sInvalidTime;
            txtSearchFSRTimeEnd.Text = clsFunction.sInvalidTime;
            txtSearchFSRServiceResult.Text = clsFunction.sDash;

            clsSearch.ClassFSRNo = int.Parse(txtSearchFSRNo.Text);
            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassFSRNo + clsFunction.sPipe;
            
            txtSearchFSRNo.Text = dbFunction.CheckAndSetNumericValue(clsSearch.ClassFSRNo.ToString());

            if (dbFunction.isValidID(txtSearchFSRNo.Text))
            {
                dbAPI.GetFSRInfo();

                txtSearchFSRDate.Text = clsFSR.ClassFSRDate;
                txtSearchFSRTimeArrived.Text = clsFSR.ClassTimeArrived;
                txtSearchFSRReceiptTime.Text = clsFSR.ClassFSRTime;
                txtSearchFSRTimeStart.Text = clsFSR.ClassTimeStart;
                txtSearchFSRTimeEnd.Text = clsFSR.ClassTimeEnd;
                txtSearchFSRServiceResult.Text = clsFSR.ClassActionMade;
                txtSearchTAIDNo.Text = clsFSR.ClassTAIDNo.ToString();
            }            
        }

        private void ClearCurretTextBox()
        {
            // Terminal
            txtCurTerminalID.Text = clsFunction.sZero;
            txtTerminalTypeID.Text = clsFunction.sZero;
            txtTerminalModelID.Text = clsFunction.sZero;
            txtTerminalBrandID.Text = clsFunction.sZero;
            txtCurTerminalSN.Text = clsFunction.sNull;
            txtCurTerminalType.Text = clsFunction.sNull;
            txtCurTerminalModel.Text = clsFunction.sNull;
            txtCurTerminalBrand.Text = clsFunction.sNull;

            // SIM
            txtCurSIMID.Text = clsFunction.sZero;
            txtCurSIMSN.Text = clsFunction.sNull;
            txtCurSIMCarrier.Text = clsFunction.sNull;

            // Dock
            txtCurDockID.Text = clsFunction.sZero;
            txtCurDockSN.Text = clsFunction.sNull;
        }

        private void ClearReplaceTextBox()
        {
            // Terminal
            txtRepTerminalID.Text = clsFunction.sZero;            
            txtRepTerminalSN.Text = clsFunction.sNull;
            txtRepTerminalType.Text = clsFunction.sNull;
            txtRepTerminalModel.Text = clsFunction.sNull;
            txtRepTerminalBrand.Text = clsFunction.sNull;

            // SIM
            txtRepSIMID.Text = clsFunction.sZero;
            txtRepSIMSN.Text = clsFunction.sNull;
            txtRepSIMCarrier.Text = clsFunction.sNull;

            // Dock
            txtRepDockID.Text = clsFunction.sZero;
            txtRepDockSN.Text = clsFunction.sNull;
        }

        
        private void GetClientInfoFromFile()
        {            
            dbFunction.GetIDFromFile("Client List", txtClientID.Text);
            txtClientID.Text = clsSearch.ClassOutFileID.ToString();
            txtClientName.Text = clsSearch.ClassOutFileDescription;
            //txtClientAddress.Text = clsSearch.ClassOutFileDescription2;
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnTRemove_Click(object sender, EventArgs e)
        {
            txtRepTerminalSN.Text = clsFunction.sZero;
            txtRepTerminalSN.Text = "";
            txtRepTerminalType.Text = "";
            txtRepTerminalModel.Text = "";
            txtRepTerminalBrand.Text = "";
            txtRepTerminalStatus.Text = clsFunction.sZero;
        }

        private void btnSIMRemove_Click(object sender, EventArgs e)
        {
            txtRepSIMID.Text = clsFunction.sZero;
            txtRepSIMSN.Text = "";
            txtRepSIMCarrier.Text = "";
            txtRepSIMStatus.Text = clsFunction.sZero;
        }

        private void btnDockRemove_Click(object sender, EventArgs e)
        {
            txtRepDockID.Text = clsFunction.sZero;
            txtRepDockSN.Text = "";
            txtRepDockStatus.Text = clsFunction.sZero;
        }

        private bool ValidateInstallationCount()
        {
            bool isValid = false;
            string sServiceDate = clsFunction.sNull;
            string sServiceJobTypeDescription = clsFunction.sNull;
            string sProcessedBy = clsFunction.sNull;

            if (dbFunction.isValidID(txtMerchantID.Text))
            {
                // Check pending service
                clsSearch.ClassSearchValue = dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtSearchSTJobType.Text);
                isValid = dbAPI.isRecordExist("Search", "Merchant Pending Service Count", clsSearch.ClassSearchValue);
                if (isValid)
                {
                    dbFunction.SetMessageBox("Unable to create service.\nPending " + cboSearchServiceType.Text + ".", "Create service failed", clsFunction.IconType.iExclamation);
                    isValid = false;
                    return isValid;
                }
                else
                {
                    if (!txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC))
                    {
                        clsSearch.ClassSearchValue = dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text);
                        isValid = dbAPI.isRecordExist("Search", "Merchant Installation Success Count", clsSearch.ClassSearchValue);

                        if (!isValid)
                        {
                            isValid = false;
                            dbFunction.SetMessageBox("Unable to create service.\n Merchant has no installed terminal.", "Create service failed", clsFunction.IconType.iExclamation);
                            return isValid;
                        }
                        else
                        {
                            isValid = true;
                        }
                    }
                    else
                    {
                        clsSearch.ClassSearchValue = dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text);
                        isValid = dbAPI.isRecordExist("Search", "Merchant Installation Success Count", clsSearch.ClassSearchValue);

                        if (isValid)
                        {
                            isValid = false;
                            dbFunction.SetMessageBox("This service has been completed", "Create service failed", clsFunction.IconType.iExclamation);
                            return isValid;
                        }
                        else
                        {
                            isValid = true;
                        }
                    }
                }

                // Check pullout status
                if (clsSearch.ClassServiceStatusDescription.Equals(clsGlobalVariables.STATUS_PULLEDOUT_DESC))
                {
                    dbFunction.SetMessageBox("Unable to create service.\n Request ID is no longer available.", "Create service failed", clsFunction.IconType.iExclamation);
                    isValid = false;
                    return isValid;
                }

                // Check has pending service
                clsSearch.ClassSearchValue = txtIRIDNo.Text + clsFunction.sPipe;
                if (dbAPI.isRecordExist("Search", "Incomplete Service", clsSearch.ClassSearchValue))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "Pending Service Info", clsSearch.ClassSearchValue, "Get Info Detail", "", "GetInfoDetail");

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                        sServiceDate = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                        sProcessedBy = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                        sServiceJobTypeDescription = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);

                    }

                    dbFunction.SetMessageBox("Pending service " + sServiceJobTypeDescription + " found.\n\nDate: " + sServiceDate + "\nProcessed By: " + sProcessedBy, "Create service failed", clsFunction.IconType.iExclamation);
                    isValid = false;
                    return isValid;
                }

            }
                 
            return isValid;
        }

        private bool ValidateDeleteCount()
        {
            bool isValid = false;

            if (dbFunction.isValidID(txtMerchantID.Text))
            {
                clsSearch.ClassSearchValue = dbFunction.CheckAndSetNumericValue(txtSearchServiceNo.Text) + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text);
                isValid = dbAPI.isRecordExist("Search", "Merchant Delete Service Count", clsSearch.ClassSearchValue);           
            }

            if (!isValid)
            {
                btnCancelJO.Enabled = true;
            }
            else
            {
                btnCancelJO.Enabled = false;
            }
               
            return isValid;
        }

        private void panel59_Paint(object sender, PaintEventArgs e)
        {

        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

        private void label46_Click(object sender, EventArgs e)
        {

        }

        private void cboSearchServiceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsServiceType.ClassServiceTypeID = 0;
            if (!cboSearchServiceType.Text.Equals(clsFunction.sDefaultSelect))
            {
                txtSearchSTID.Text =
                txtSearchSTDescription.Text =
                txtSearchSTCode.Text =
                txtSearchSTStatus.Text =
                txtSearchSTStatusDescription.Text =
                txtSearchSTJobType.Text =
                txtSearchSTJobTypeDescription.Text = clsFunction.sNull;
                
                //lblSubHeader.Text = clsFunction.sDash;

                // Get Info
                dbAPI.ExecuteAPI("GET", "Search", "Service Type Info", cboSearchServiceType.Text, "Get Info Detail", "", "GetInfoDetail");

                if (dbAPI.isNoRecordFound() == false)
                {
                    clsServiceType.ClassServiceTypeID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0));
                    clsServiceType.ClassDescription = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    clsServiceType.ClassCode = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                    clsServiceType.ClassServiceStatus = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3));
                    clsServiceType.ClassStatusDescription = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                    clsServiceType.ClassJobType = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5));
                    clsServiceType.ClassJobTypeDescrition = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                    clsServiceType.ClassServiceJobTypeDescrition = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);

                    txtSearchSTDescription.Text = clsServiceType.ClassDescription;
                    txtSearchSTCode.Text = clsServiceType.ClassCode;
                    txtSearchSTStatus.Text = clsServiceType.ClassServiceStatus.ToString();
                    txtSearchSTStatusDescription.Text = clsServiceType.ClassStatusDescription;
                    txtSearchSTJobType.Text = clsServiceType.ClassJobType.ToString();
                    txtSearchSTJobTypeDescription.Text = clsServiceType.ClassJobTypeDescrition;
                    txtSearchSTServiceJobTypeDescription.Text = clsServiceType.ClassServiceJobTypeDescrition;

                    Debug.WriteLine("Selected Service Type.....................................................................");
                    Debug.WriteLine("txtSearchSTDescription.Text="+ txtSearchSTDescription.Text);
                    Debug.WriteLine("txtSearchSTCode.Text=" + txtSearchSTCode.Text);
                    Debug.WriteLine("txtSearchSTStatus.Text=" + txtSearchSTStatus.Text);
                    Debug.WriteLine("txtSearchSTStatusDescription.Text=" + txtSearchSTStatusDescription.Text);
                    Debug.WriteLine("txtSearchSTJobType.Text=" + txtSearchSTJobType.Text);
                    Debug.WriteLine("txtSearchSTJobTypeDescription.Text=" + txtSearchSTJobTypeDescription.Text);
                    Debug.WriteLine("txtSearchSTServiceJobTypeDescription.Text=" + txtSearchSTServiceJobTypeDescription.Text);
                    Debug.WriteLine("..........................................................................................");


                    btnSearchRepTerminal.Enabled = btnRemoveRepTerminal.Enabled = btnSearchRepSIM.Enabled = btnRemoveRepSIM.Enabled = true;
                    if (!txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
                    {
                        btnSearchRepTerminal.Enabled = btnRemoveRepTerminal.Enabled = btnSearchRepSIM.Enabled = btnRemoveRepSIM.Enabled = false;
                    }

                    SetMKTextBoxBackColor();                
                    InitStatusTitle(false);
                }
                
            }

            txtSearchSTID.Text = clsServiceType.ClassServiceTypeID.ToString();

            if (!lblMainStatus.Text.Equals(clsFunction.sDash))
            {
                if (lblMainStatus.Text.Substring(0, 3).Equals("NEW"))
                {
                    if (!ValidateInstallationCount()) return;
                }
            }
               
        }

        private void btnSearchCurTerminal_Click(object sender, EventArgs e)
        {
            
            frmSearchField.iSearchType  = frmSearchField.SearchType.iTerminal;
            frmSearchField.iStatus      = clsGlobalVariables.STATUS_AVAILABLE;
            frmSearchField.sTerminalType = "View Terminal";
            frmSearchField.sHeader      = "TERMINAL";
            frmSearchField.isCheckBoxes = false;
            frmSearchField.iLocationID  = clsFunction.iZero;
            frmSearchField.sLocation    = clsFunction.sDefaultSelect;
            frmSearchField frm          = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsSearch.ClassIsReleased = 0;
                
                // Check SN status must be available
                if (clsSearch.ClassTerminalStatus != clsGlobalVariables.STATUS_AVAILABLE)
                {
                    dbFunction.SetMessageBox("Terminal SN " + "[" + clsSearch.ClassTerminalSN + "]" + " status is not AVAILABLE.", "Unable to add", clsFunction.IconType.iError);
                    return;
                }
                
                txtCurTerminalID.Text = clsSearch.ClassTerminalID.ToString();
                txtCurTerminalSN.Text = txtNewTerminalSN.Text = clsSearch.ClassTerminalSN;
                PopulateTerminalTextBox(txtCurTerminalID.Text, txtCurTerminalSN.Text, true);

                // check SN value
                if (!dbFunction.isValidDescription(txtCurTerminalSN.Text) || 
                    !dbFunction.isValidDescription(txtCurTerminalType.Text) ||
                    !dbFunction.isValidDescription(txtCurTerminalModel.Text) ||
                    !dbFunction.isValidDescription(txtCurTerminalBrand.Text) ||
                    !dbFunction.isValidDescription(txtCurTerminalLocation.Text) ||
                    !dbFunction.isValidDescription(txtCurTerminalAssetType.Text) ||
                    !dbFunction.isValidDescription(txtCurTerminalStatus.Text))
                {
                    dbFunction.SetMessageBox("Invalid current TERMINAL information selected." + "\n\n" +
                                              "Current TERMINAL information" + "\n" +
                                              " >SN : " + txtCurTerminalSN.Text + "\n" +
                                              " >Type : " + txtCurTerminalType.Text + "\n" +
                                              " >Model : " + txtCurTerminalModel.Text + "\n" +
                                              " >Brand : " + txtCurTerminalBrand.Text + "\n" +
                                              " >Location : " + txtCurTerminalLocation.Text + "\n" +
                                              " >Assert Type : " + txtCurTerminalAssetType.Text + "\n" +
                                              " >Status : " + txtCurTerminalStatus.Text + "\n\n" +
                                              "Kind check the selected record.", "Field checking", clsFunction.IconType.iError);                   
                    return;
                }
                
                // Handle terminal SN has not been released
                if (clsSearch.ClassIsReleased <= 0)
                {
                    dbFunction.SetMessageBox("Terminal SN " + "[" + txtCurTerminalSN.Text +"]" + " has not been released. ", "Unable to add", clsFunction.IconType.iError);

                    txtCurTerminalID.Text =
                    txtCurTerminalStatus.Text =
                    txtCurTerminalSN.Text =
                    txtCurTerminalCode.Text =
                    txtCurTerminalType.Text =
                    txtCurTerminalModel.Text =
                    txtCurTerminalBrand.Text =
                    txtCurTerminalLocation.Text =
                    txtCurTerminalAssetType.Text = clsFunction.sNull;

                    return;
                }

                getApplicationInfo(); // get version/crc from master data (tblterminalmodel)

                checkAndSetDispatch();

            }
        }

        private void btnSearchCurSIM_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iSIM;
            frmSearchField.iStatus = clsGlobalVariables.STATUS_AVAILABLE;
            frmSearchField.sHeader = "SIM";
            frmSearchField.isCheckBoxes = false;
            frmSearchField.iLocationID = clsFunction.iZero;
            frmSearchField.sLocation = clsFunction.sDefaultSelect;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsSearch.ClassIsReleased = 0;

                // Check SN status must be available
                if (clsSearch.ClassSIMStatus != clsGlobalVariables.STATUS_AVAILABLE)
                {
                    dbFunction.SetMessageBox("SIM SN " + clsSearch.ClassSIMSerialNo + " status is not AVAILABLE.", "Unable to add", clsFunction.IconType.iError);
                    return;
                }

                txtCurSIMID.Text = clsSearch.ClassSIMID.ToString();
                txtCurSIMSN.Text = txtNewSIMSN.Text = clsSearch.ClassSIMSerialNo;
                PopulateSIMTextBox(txtCurSIMID.Text, txtCurSIMSN.Text, true);

                // check SN value
                if (!dbFunction.isValidDescription(txtCurSIMSN.Text) ||
                    !dbFunction.isValidDescription(txtCurSIMCarrier.Text) ||
                    !dbFunction.isValidDescription(txtCurSIMLocation.Text) ||                   
                    !dbFunction.isValidDescription(txtCurSIMStatus.Text))
                {
                    dbFunction.SetMessageBox("Invalid current SIM information selected." + "\n\n" +
                                              "Current SIM information" + "\n" +
                                              " >SN : " + txtCurSIMSN.Text + "\n" +
                                              " >Carrier : " + txtCurSIMCarrier.Text + "\n" +
                                              " >Location : " + txtCurSIMLocation.Text + "\n" +
                                              " >Status : " + txtCurSIMStatus.Text + "\n\n" +
                                              "Kind check the selected record.", "Field checking", clsFunction.IconType.iError);
                    return;
                }

                // Handle terminal SN has not been released
                if (clsSearch.ClassIsReleased <= 0)
                {
                    dbFunction.SetMessageBox("SIM SN " + "[" + txtCurSIMSN.Text + "]" + " has not been released. ", "Unable to add", clsFunction.IconType.iError);

                    txtCurSIMID.Text =
                    txtCurSIMStatus.Text =
                    txtCurSIMSN.Text =
                    txtCurSIMCarrier.Text =
                    txtCurSIMLocation.Text = clsFunction.sNull;
                    
                    return;
                }

                checkAndSetDispatch();

            }
        }

        private void btnSearchRepTerminal_Click(object sender, EventArgs e)
        {
            
            frmSearchField.iSearchType = frmSearchField.SearchType.iTerminal;
            frmSearchField.iStatus = clsGlobalVariables.STATUS_AVAILABLE;
            frmSearchField.sTerminalType = "View Terminal";
            frmSearchField.sHeader = "REPLACE TERMINAL";
            frmSearchField.isCheckBoxes = false;
            frmSearchField.iLocationID = clsFunction.iZero;
            frmSearchField.sLocation = clsFunction.sDefaultSelect;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsSearch.ClassIsReleased = 0;

                // Check SN status must be available
                if (clsSearch.ClassTerminalStatus != clsGlobalVariables.STATUS_AVAILABLE)
                {
                    dbFunction.SetMessageBox("Terminal SN " + "[" + clsSearch.ClassTerminalSN + "]" + " status is not AVAILABLE.", "Unable to add", clsFunction.IconType.iError);
                    return;
                }

                txtRepTerminalID.Text = clsSearch.ClassTerminalID.ToString();
                txtRepTerminalSN.Text = txtNewTerminalSN.Text = clsSearch.ClassTerminalSN;

                PopulateTerminalTextBox(txtRepTerminalID.Text, txtRepTerminalSN.Text, false);

                // check SN value
                if (!dbFunction.isValidDescription(txtRepTerminalSN.Text) ||
                    !dbFunction.isValidDescription(txtRepTerminalType.Text) ||
                    !dbFunction.isValidDescription(txtRepTerminalModel.Text) ||
                    !dbFunction.isValidDescription(txtRepTerminalBrand.Text) ||
                    !dbFunction.isValidDescription(txtRepTerminalLocation.Text) ||
                    !dbFunction.isValidDescription(txtRepTerminalAssetType.Text) ||
                    !dbFunction.isValidDescription(txtRepTerminalStatus.Text))
                {
                    dbFunction.SetMessageBox("Invalid replace TERMINAL information selected." + "\n\n" +
                                  "Replace TERMINAL information" + "\n" +
                                  " >SN : " + txtRepTerminalSN.Text + "\n" +
                                  " >Type : " + txtRepTerminalType.Text + "\n" +
                                  " >Model : " + txtRepTerminalModel.Text + "\n" +
                                  " >Brand : " + txtRepTerminalBrand.Text + "\n" +
                                  " >Location : " + txtRepTerminalLocation.Text + "\n" +
                                  " >Assert Type : " + txtRepTerminalAssetType.Text + "\n" +
                                  " >Status : " + txtRepTerminalStatus.Text + "\n\n" +
                                  "Kind check the selected record.", "Field checking", clsFunction.IconType.iError);
                    return;
                }

                // Handle terminal SN has not been released
                if (clsSearch.ClassIsReleased <= 0)
                {
                    dbFunction.SetMessageBox("Terminal SN " + "[" + txtRepTerminalSN.Text + "]" + " has not been released. ", "Unable to add", clsFunction.IconType.iError);

                    txtRepTerminalID.Text =
                    txtRepTerminalStatus.Text =
                    txtRepTerminalSN.Text =
                    txtRepTerminalCode.Text =
                    txtRepTerminalType.Text =
                    txtRepTerminalModel.Text =
                    txtRepTerminalBrand.Text =
                    txtRepTerminalLocation.Text =
                    txtRepTerminalAssetType.Text = clsFunction.sNull;

                    return;
                }

                getApplicationInfo(); // get version/crc from master data (tblterminalmodel)

                checkAndSetDispatch();

            }
        }

        private void btnSearchRepSIM_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iSIM;
            frmSearchField.iStatus = clsGlobalVariables.STATUS_AVAILABLE;
            frmSearchField.sHeader = "REPLACE SIM";
            frmSearchField.isCheckBoxes = false;
            frmSearchField.iLocationID = clsFunction.iZero;
            frmSearchField.sLocation = clsFunction.sDefaultSelect;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsSearch.ClassIsReleased = 0;

                // Check SN status must be available
                if (clsSearch.ClassSIMStatus != clsGlobalVariables.STATUS_AVAILABLE)
                {
                    dbFunction.SetMessageBox("SIM SN " + "[" + clsSearch.ClassSIMSerialNo + "]" + " status is not AVAILABLE.", "Unable to add", clsFunction.IconType.iError);
                    return;
                }

                txtRepSIMID.Text = clsSearch.ClassSIMID.ToString();
                txtRepSIMSN.Text = txtNewSIMSN.Text = clsSearch.ClassSIMSerialNo;
                PopulateSIMTextBox(txtRepSIMID.Text, txtRepSIMSN.Text, false);

                // check SN value
                if (!dbFunction.isValidDescription(txtRepSIMSN.Text) ||
                    !dbFunction.isValidDescription(txtRepSIMCarrier.Text) ||
                    !dbFunction.isValidDescription(txtRepSIMLocation.Text) ||
                    !dbFunction.isValidDescription(txtRepSIMStatus.Text))
                {
                    dbFunction.SetMessageBox("Invalid replace SIM information selected." + "\n\n" +
                                  "Replace SIM information" + "\n" +
                                  " >SN : " + txtRepSIMSN.Text + "\n" +
                                  " >Carrier : " + txtRepSIMCarrier.Text + "\n" +
                                  " >Location : " + txtRepSIMLocation.Text + "\n" +
                                  " >Status : " + txtRepSIMStatus.Text + "\n\n" +
                                  "Kind check the selected record.", "Field checking", clsFunction.IconType.iError);
                    return;
                }

                // Handle terminal SN has not been released
                if (clsSearch.ClassIsReleased <= 0)
                {
                    dbFunction.SetMessageBox("SIM SN " + "[" + txtRepSIMSN.Text + "]" + " has not been released. ", "Unable to add", clsFunction.IconType.iError);

                    txtRepSIMID.Text =
                    txtRepSIMStatus.Text =
                    txtRepSIMSN.Text =
                    txtRepSIMCarrier.Text =
                    txtRepSIMLocation.Text = clsFunction.sNull;

                    return;
                }

                checkAndSetDispatch();

            }
        }

        private void btnSearchMerchant_Click(object sender, EventArgs e)
        {
            //if (!dbFunction.isValidEntry(clsFunction.CheckType.iServiceType, txtSearchSTID.Text)) return;

            if (fAutoLoadData)
            {
                frmSearchField.fSelected = true;
            }
            else
            {
                frmSearchField.iSearchType = frmSearchField.SearchType.iMerchant;
                frmSearchField.sHeader = "MERCHANT";
                frmSearchField.isCheckBoxes = false;
                frmSearchField.isPreview = false;
                frmSearchField frm = new frmSearchField();
                frm.ShowDialog();

            }

            if (frmSearchField.fSelected)
            {
                // ROCKY -- HANDLE NULL TID/MID: SERVICING JOB ORDER ISSUE
                if(!dbFunction.isValidID(clsSearch.ClassTID) || !dbFunction.isValidID(clsSearch.ClassMID))
                {
                    dbFunction.SetMessageBox("Invalid merchant information selected for job order " + cboSearchServiceType.Text + "." + "\n\n" +
                                              "Merchant information" + "\n" +
                                              " >Name : " + clsSearch.ClassParticularName + "\n" +
                                              " >TID : " + clsSearch.ClassTID + "\n" +
                                              " >MID : " + clsSearch.ClassMID + "\n\n" +
                                              "Kind check the selected record.", "Field checking", clsFunction.IconType.iError);                    
                    return;
                }

                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    dbFunction.ClearListViewItems(lvwMM);
                    dbFunction.ClearListViewItems(lvwProfile1);
                    dbFunction.ClearListViewItems(lvwProfile2);
                    dbFunction.ClearListViewItems(lvwRaw);
                    dbFunction.ClearListViewItems(lvwChanges);

                    ComboBoxDefaultSelect();

                    InitCount();

                    btnAdd.Enabled = false;
                    btnSave.Enabled = true;

                    // Load Information
                    txtMerchantID.Text = clsSearch.ClassParticularID.ToString();
                    txtSearchMerchantName.Text = txtMerchantName.Text = clsSearch.ClassParticularName;
                    txtIRTID.Text = clsSearch.ClassTID;
                    txtIRMID.Text = clsSearch.ClassMID;
                    txtSearchIRNo.Text = clsSearch.ClassIRNo;
                    txtIRIDNo.Text = clsSearch.ClassIRIDNo.ToString();
                    txtClientID.Text = clsSearch.ClassClientID.ToString();

                    txtServiceStatusDescription.Text = clsSearch.ClassServiceStatusDescription.ToString();

                    FillMerchantTextBox();

                    PKTextBoxReadOnly(false);

                    InitCreatedDateTime();

                    SetMKTextBoxBackColor();

                    btnDispatchJO.Enabled = true;

                    InitSearchRemoveButton(false);

                    LoadServicingDetail();

                    loadStockMovementDetail(lvwStockDetail, true);
                    loadStockMovementDetail(lvwRepStockDetail, false);

                    dbAPI.loadMultiMerchantInfo(lvwMM, int.Parse(txtIRIDNo.Text));

                    loadMerchantLastControlNo();

                    // Load data                
                    txtFEID.Text = (!fEdit ? clsFunction.sZero : clsSearch.ClassFEID.ToString());
                    txtCurTerminalID.Text = clsSearch.ClassTerminalID.ToString();
                    txtCurSIMID.Text = clsSearch.ClassSIMID.ToString();
                    txtRepTerminalID.Text = (!fEdit ? clsFunction.sZero : clsSearch.ClassRepTerminalID.ToString());
                    txtRepSIMID.Text = (!fEdit ? clsFunction.sZero : clsSearch.ClassRepSIMID.ToString());

                    FillClientTextBox();

                    FillFETextBox();

                    FillMerchRepTextBox();

                    FillMerchantContactInfoTextBox();

                    loadCurrentSN();

                    // Terminal
                    PopulateTerminalTextBox(txtCurTerminalID.Text, txtCurTerminalSN.Text, true); // current
                    PopulateTerminalTextBox(txtRepTerminalID.Text, txtRepTerminalSN.Text, false); // replace

                    // SIM
                    PopulateSIMTextBox(txtCurSIMID.Text, txtCurSIMSN.Text, true); // current
                    PopulateSIMTextBox(txtRepSIMID.Text, txtRepSIMSN.Text, false); // false

                    if (!txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
                    {
                        txtRepTerminalID.Text =
                        txtRepTerminalSN.Text =
                        txtRepSIMID.Text =
                        txtRepSIMSN.Text = clsFunction.sNull;

                        txtRepTerminalCode.Text =
                        txtRepTerminalType.Text =
                        txtRepTerminalModel.Text =
                        txtRepTerminalBrand.Text =
                        txtRepTerminalLocation.Text =
                        txtRepTerminalAssetType.Text = clsFunction.sNull;

                        txtRepSIMCarrier.Text =
                        txtRepSIMLocation.Text = clsFunction.sNull;
                    }

                    if (!ValidateInstallationCount())
                    {
                        btnDispatchJO.Enabled = false;
                        btnAdd.Enabled = false;
                        btnSave.Enabled = false;
                        return;
                    }

                    SetCount();

                    txtServiceType1.Text = cboSearchServiceType.Text;

                    cboSearchServiceType.Enabled = false;

                    //txtEntryRequestID.Focus();

                    btnDispatchJO.Enabled = true;

                    entryTextBox(true);

                    // check IRNo prefix
                    if (!dbFunction.isValidIRNoPrefix(txtSearchIRNo.Text))
                    {
                        dbFunction.SetMessageBox(dbFunction.AddBracketStartEnd(txtSearchIRNo.Text) + " is invalid Request ID." + "\n\n" +
                            "Modify Request ID to able to continue." + "\n\n" +
                            "Valid prefix " + dbFunction.AddBracketStartEnd(clsSystemSetting.ClassSystemIRNoPrefix), "Invalid Requet ID", clsFunction.IconType.iError);

                        btnDispatchJO.Enabled = false;
                    }

                    // FillChangesListView
                    clsSearch.ClassMerchantID = int.Parse(dbFunction.CheckAndSetNumericValue(txtMerchantID.Text));
                    clsSearch.ClassIRIDNo = int.Parse(dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text));
                    dbAPI.FillListViewChangesMapping(lvwChanges, "", "");

                    AdditionalComBoBoxUnlock(true);

                    if (!fEdit)
                        getApplicationInfo();

                    txtRequestID1.Text = dbAPI.getPrimaryIRNo(int.Parse(txtIRIDNo.Text));

                    // Disable readonly
                    txtEntryRequestID.ReadOnly = txtEntryReferenceNo.ReadOnly = false;

                    // Enable entry
                    txtEntryRequestID.Enabled = txtEntryReferenceNo.Enabled = true;
                    txtEntryRequestID.BackColor = txtEntryReferenceNo.BackColor = clsFunction.EntryBackColor;

                    // Enable auto gen button
                    btnNoRequestID.Enabled = true;
                    dbFunction.SetButtonIconImage(btnNoRequestID);

                    btnNoReferenceNo.Enabled = true;
                    dbFunction.SetButtonIconImage(btnNoReferenceNo);

                    txtServiceJobType.Text = dbAPI.getServiceJobType(txtSearchSTJobTypeDescription.Text).ToString();

                    txtTicketStatus.Text = clsFunction.sNull;

                    // Init header
                    lblHeader.Text = "CREATE JOB ORDER" + " " + dbFunction.AddBracketStartEnd(cboSearchServiceType.Text) + " " + dbFunction.AddBracketStartEnd(txtIRTID.Text) + " " + dbFunction.AddBracketStartEnd(txtIRMID.Text);
                    lblSubHeader.Text = txtSearchSTJobTypeDescription.Text + " - " + txtRequestNo.Text;

                    tabFillUp.TabIndex = 0;

                    btnPreviewFSR.Enabled = btnViewDiagnostic.Enabled = btnUpdateServiceDate.Enabled = btnUpdateMerchRep.Enabled = btnUpdateServiceType.Enabled = false;

                    btnClear.Focus();

                    Cursor.Current = Cursors.Default;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exceptional error message {ex.Message}");
                    dbFunction.SetMessageBox($"Exceptional error message {ex.Message}", "New: Job Order", clsFunction.IconType.iError);
                }
                
            }
        }

        private void btnSearchClient_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantID, txtMerchantID.Text)) return;

            frmSearchField.iSearchType = frmSearchField.SearchType.iClient;
            frmSearchField.sHeader = "CLIENT";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                // Load Information
                txtClientID.Text = clsSearch.ClassParticularID.ToString();
                txtClientName.Text = clsSearch.ClassParticularName;

                FillClientTextBox();

                //txtClientName.BackColor = clsFunction.MKBackColor;

                txtEntryReferenceNo.Focus();
            }
        }

        private void SetMKTextBoxBackColor()
        {
            txtMerchantName.BackColor = txtClientName.BackColor = txtFEName.BackColor = txtCurTerminalSN.BackColor = txtCurSIMSN.BackColor = txtRepTerminalSN.BackColor = txtRepSIMSN.BackColor = txtRequestID1.BackColor = txtServiceType1.BackColor = lblMainStatus.BackColor = txtDispatcher.BackColor = clsFunction.MKBackColor;
            if (!txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
            {
                txtRepTerminalSN.BackColor = txtRepSIMSN.BackColor = clsFunction.DisableBackColor;
            }

            // Header
            //txtRequestID1.BackColor = txtServiceType1.BackColor = lblMainStatus.BackColor = Color.Navy;

            // status backcolor
            txtServiceJobTypeStatusDesc.BackColor = txtSearchFSRDesc.BackColor = txtSearchFSRServiceResult.BackColor = txtTicketStatus.BackColor = txtBillable.BackColor = txtDiagnostic.BackColor = txtMerchantSign.BackColor = txtIRStatusDescription.BackColor = clsFunction.StatusBackColor;
            
        }

        private void SetPKTextBoxBackColor()
        {
            txtSearchMerchantName.BackColor = txtSearchIRNo.BackColor = clsFunction.PKBackColor;
        }
        
        private void FillServicingID()
        {
            Debug.WriteLine("--FillServicingID--");
            Debug.WriteLine("txtLastServiceNo.Text="+ txtLastServiceNo.Text);

            clsSearch.ClassClientID =
            clsSearch.ClassFEID =
            clsSearch.ClassTerminalID =
            clsSearch.ClassSIMID =
            clsSearch.ClassRepTerminalID =
            clsSearch.ClassRepSIMID = clsFunction.iZero;

            if (dbFunction.isValidID(txtLastServiceNo.Text))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Servicing ID Info", txtLastServiceNo.Text, "Get Info Detail", "", "GetInfoDetail");

                if (dbAPI.isNoRecordFound() == false)
                {
                    clsSearch.ClassClientID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1));
                    clsSearch.ClassFEID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2));
                    clsSearch.ClassTerminalID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3));
                    clsSearch.ClassSIMID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4));
                    clsSearch.ClassRepTerminalID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5));
                    clsSearch.ClassRepSIMID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6));
                }
            }
        }

        private void InitSearchRemoveButton(bool isClear)
        {
            bool isEdit = false;
            Debug.WriteLine("--InitSearchRemoveButton--");
            Debug.WriteLine("isClear="+ isClear);
            Debug.WriteLine("Global fEdit=" + fEdit);
            Debug.WriteLine("txtSearchSTJobTypeDescription.Text=" + txtSearchSTJobTypeDescription.Text);
            Debug.WriteLine("txtServiceStatusDescription.Text=" + txtServiceStatusDescription.Text);

            if (isClear)
            {
                btnSearchMerchant.Enabled = false;
                btnSearchService.Enabled = true;
                
                btnSearchClient.Enabled = btnRemoveClient.Enabled = btnSearchFE.Enabled = btnRemoveFE.Enabled = 
                    btnSearchCurTerminal.Enabled = btnRemoveCurTerminal.Enabled = btnSearchCurSIM.Enabled = btnRemoveCurSIM.Enabled =
                    btnSearchRepTerminal.Enabled = btnSearchRepSIM.Enabled = btnRemoveRepTerminal.Enabled = btnRemoveRepSIM.Enabled = 
                    btnSearchStock.Enabled = btnRemoveStock.Enabled = btnSearchDispatcher.Enabled = btnRemoveDispatcher.Enabled =  false;

                btnNoRequestID.Enabled = btnNoReferenceNo.Enabled = false;
            }
            else
            {
                btnSearchClient.Enabled = btnRemoveClient.Enabled = btnSearchFE.Enabled = btnRemoveFE.Enabled = btnSearchDispatcher.Enabled = btnRemoveDispatcher.Enabled = true;
                
                btnSearchCurTerminal.Enabled = 
                btnRemoveCurTerminal.Enabled = btnSearchCurSIM.Enabled = btnRemoveCurSIM.Enabled =
                btnSearchRepTerminal.Enabled = btnSearchRepSIM.Enabled = btnRemoveRepTerminal.Enabled = btnRemoveRepSIM.Enabled = 
                btnSearchStock.Enabled = btnRemoveStock.Enabled = false;

                if (((txtServiceStatusDescription.Text.Equals(clsGlobalVariables.STATUS_ALLOCATED_DESC)) || 
                    (txtServiceStatusDescription.Text.Equals(clsGlobalVariables.STATUS_DISPATCH_DESC)) || 
                    (txtServiceStatusDescription.Text.Equals(clsGlobalVariables.STATUS_AVAILABLE_DESC))) && (!txtServiceStatusDescription.Text.Equals(clsGlobalVariables.STATUS_NEGATIVE_DESC)))
                    isEdit = true;
                else
                    isEdit = false;



                Debug.WriteLine("Local isEdit=" + isEdit);

                if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
                {
                    if (txtServiceStatusDescription.Text.Equals(clsGlobalVariables.STATUS_INSTALLED_DESC))
                        isEdit = true;

                    btnSearchRepTerminal.Enabled = btnSearchRepSIM.Enabled = btnRemoveRepTerminal.Enabled = btnRemoveRepSIM.Enabled = 
                    btnSearchStock.Enabled = btnRemoveStock.Enabled = isEdit;                   
                }
                else if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC))
                {                
                    btnSearchCurTerminal.Enabled = btnRemoveCurTerminal.Enabled = btnSearchCurSIM.Enabled = btnRemoveCurSIM.Enabled =
                    btnSearchStock.Enabled = btnRemoveStock.Enabled = isEdit;

                    btnNoRequestID.Enabled = btnNoReferenceNo.Enabled = isEdit;
                }
                else
                {
                    //btnSearchCurTerminal.Enabled = btnRemoveCurTerminal.Enabled = btnSearchCurSIM.Enabled = btnRemoveCurSIM.Enabled = (!isEdit ? false : true);
                    btnSearchCurTerminal.Enabled = btnRemoveCurTerminal.Enabled = btnSearchCurSIM.Enabled = btnRemoveCurSIM.Enabled = 
                    btnSearchStock.Enabled = btnRemoveStock.Enabled = false;
                }

                if (txtServiceJobTypeStatusDesc.Text.Equals(clsDefines.SERVICE_STATUS_COMPLETED))
                {
                    btnSearchClient.Enabled = btnRemoveClient.Enabled = btnSearchFE.Enabled = btnRemoveFE.Enabled =
                    btnSearchCurTerminal.Enabled = btnRemoveCurTerminal.Enabled = btnSearchCurSIM.Enabled = btnRemoveCurSIM.Enabled =
                    btnSearchRepTerminal.Enabled = btnSearchRepSIM.Enabled = btnRemoveRepTerminal.Enabled = btnRemoveRepSIM.Enabled =
                    btnSearchStock.Enabled = btnRemoveStock.Enabled = btnSearchDispatcher.Enabled = btnRemoveDispatcher.Enabled = false;
                }
            }

            Debug.WriteLine("->>>Local isEdit=" + isEdit);

            // Search button
            dbFunction.SetButtonIconImage(btnSearchClient);
            dbFunction.SetButtonIconImage(btnSearchFE);
            dbFunction.SetButtonIconImage(btnSearchDispatcher);
            dbFunction.SetButtonIconImage(btnSearchCurTerminal);
            dbFunction.SetButtonIconImage(btnSearchCurSIM);
            dbFunction.SetButtonIconImage(btnSearchRepTerminal);
            dbFunction.SetButtonIconImage(btnSearchRepSIM);
            dbFunction.SetButtonIconImage(btnSearchStock);

            // Remove button
            dbFunction.SetButtonIconImage(btnRemoveClient);
            dbFunction.SetButtonIconImage(btnRemoveFE);
            dbFunction.SetButtonIconImage(btnRemoveDispatcher);
            dbFunction.SetButtonIconImage(btnRemoveCurTerminal);
            dbFunction.SetButtonIconImage(btnRemoveCurSIM);
            dbFunction.SetButtonIconImage(btnRemoveRepTerminal);
            dbFunction.SetButtonIconImage(btnRemoveRepSIM);
            dbFunction.SetButtonIconImage(btnRemoveStock);
            
            dbFunction.SetButtonIconImage(btnNoRequestID);
            dbFunction.SetButtonIconImage(btnNoReferenceNo);
            

            // Find
            //dbFunction.SetButtonIconImage(btnSearchMerchant);
            //dbFunction.SetButtonIconImage(btnSearchService);
        }

        private void btnRemoveCurTerminal_Click(object sender, EventArgs e)
        {
            txtCurTerminalID.Text =
            txtCurTerminalStatus.Text =
                txtCurTerminalSN.Text =
                txtCurTerminalCode.Text =
                txtCurTerminalType.Text =
                txtCurTerminalModel.Text =
                txtCurTerminalBrand.Text =
                txtCurTerminalLocation.Text =
                txtCurTerminalAssetType.Text = clsFunction.sNull;

        }

        private void btnRemoveCurSIM_Click(object sender, EventArgs e)
        {
            txtCurSIMID.Text =
            txtCurSIMStatus.Text =
                txtCurSIMSN.Text =
                txtCurSIMCarrier.Text =
                txtCurSIMLocation.Text = clsFunction.sNull;
        }

        private void btnRemoveRepTerminal_Click(object sender, EventArgs e)
        {
            txtRepTerminalID.Text =
            txtRepTerminalSN.Text =
                txtRepTerminalCode.Text =
                txtRepTerminalType.Text =
                txtRepTerminalModel.Text =
                txtRepTerminalBrand.Text =
                txtRepTerminalLocation.Text =
                txtRepTerminalAssetType.Text = clsFunction.sNull;

        }

        private void btnRemoveRepSIM_Click(object sender, EventArgs e)
        {
            txtRepSIMID.Text =
            txtRepSIMStatus.Text =
                txtRepSIMSN.Text =
                txtRepSIMCarrier.Text =
                txtRepSIMLocation.Text = clsFunction.sNull;
        }

        private void btnSearchService_Click(object sender, EventArgs e)
        {
            if (fAutoLoadData)
            {
                frmSearchField.fSelected = true;
            }
            else
            {
                frmSearchField.iSearchType = frmSearchField.SearchType.iService;
                frmSearchField.sHeader = "SEARCH SERVICE";
                frmSearchField.isCheckBoxes = false;
                frmSearchField frm = new frmSearchField();
                frm.ShowDialog();
            }
            
            if (frmSearchField.fSelected)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    dbFunction.ClearTextBox(this);

                    dbFunction.ClearListViewItems(lvwMM);
                    dbFunction.ClearListViewItems(lvwProfile1);
                    dbFunction.ClearListViewItems(lvwProfile2);
                    dbFunction.ClearListViewItems(lvwRaw);
                    dbFunction.ClearListViewItems(lvwChanges);

                    ComboBoxDefaultSelect();

                    fEdit = true;
                    InitCount();

                    txtSearchServiceNo.Text = dbFunction.CheckAndSetNumericValue(clsSearch.ClassServiceNo.ToString());
                    txtSearchFSRNo.Text = dbFunction.CheckAndSetNumericValue(clsSearch.ClassFSRNo.ToString());
                    txtMerchantName.Text = clsSearch.ClassParticularName;
                    txtClientID.Text = dbFunction.CheckAndSetNumericValue(clsSearch.ClassClientID.ToString());
                    txtMerchantID.Text = dbFunction.CheckAndSetNumericValue(clsSearch.ClassMerchantID.ToString());
                    txtFEID.Text = dbFunction.CheckAndSetNumericValue(clsSearch.ClassFEID.ToString());
                    txtIRTID.Text = clsSearch.ClassTID;
                    txtIRMID.Text = clsSearch.ClassMID;

                    txtIRIDNo.Text = dbFunction.CheckAndSetNumericValue(clsSearch.ClassIRIDNo.ToString());
                    txtSearchIRNo.Text = clsSearch.ClassIRNo;

                    txtServiceStatusDescription.Text = clsSearch.ClassServiceStatusDescription;

                    FillMerchantTextBox();

                    FillClientTextBox();

                    FillFETextBox();

                    FiillServicingInfo();

                    // Helpdesk
                    clsSearch.ProblemNo = int.Parse(dbFunction.CheckAndSetNumericValue(txtProblemNo.Text));
                    HelpDeskController data = _mHelpDeskController.getInfo(clsSearch.ProblemNo);

                    if (data != null && dbFunction.isValidID(txtProblemNo.Text))
                        FilldHelpdeskInfo(int.Parse(txtAssistNo.Text), int.Parse(txtProblemNo.Text), data);

                    FillFEContactInfoTextBox();

                    FillDispatcherContactInfoTextBox();

                    FillServicingSNInfo();

                    FillMerchRepTextBox();

                    PKTextBoxReadOnly(false);

                    SetMKTextBoxBackColor();
                    SetPKTextBoxBackColor();

                    LoadServicingDetail();

                    FiillFSRInfo();

                    getServiceGeoLocation();

                    clsSearch.ClassMerchantID = int.Parse(dbFunction.CheckAndSetNumericValue(txtMerchantID.Text));
                    clsSearch.ClassIRIDNo = int.Parse(dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text));
                    if (dbAPI.isRecordExist("Search", "Changes Service", $"{txtSearchServiceNo.Text}{clsDefines.gPipe}{txtIRIDNo.Text}"))
                        loadServiceChangesDetail();
                    else
                        dbAPI.FillListViewChangesMapping(lvwChanges, "", "");

                    loadTATDetail();

                    loadStockMovementDetail(lvwStockDetail, true);
                    loadStockMovementDetail(lvwRepStockDetail, false);

                    dbAPI.loadMultiMerchantInfo(lvwMM, int.Parse(txtIRIDNo.Text));

                    InitStatusTitle(false);

                    cboSearchServiceType.Text = txtServiceJobTypeDesc.Text;

                    //ValidateDeleteCount();

                    InitSearchRemoveButton(false);

                    UpdateButton(false);

                    chkDispatch.Enabled = true;
                    if (dbFunction.isValidID(txtSearchFSRNo.Text))
                    {
                        chkDispatch.Checked = true;
                        chkDispatch.Enabled = false;
                    }

                    txtServiceType1.Text = cboSearchServiceType.Text;

                    SetCount();

                    btnSendEmail.Enabled = true;

                    btnSearchMerchant.Enabled = false;

                    entryTextBox(true);

                    btnCancelJO.Enabled = true;
                    btnRefreshService.Enabled = true;

                    // Disable entry
                    txtEntryRequestID.Enabled = txtEntryReferenceNo.Enabled = false;
                    txtEntryRequestID.BackColor = txtEntryReferenceNo.BackColor = clsFunction.DisableBackColor;

                    // Disable auto gen button
                    btnNoRequestID.Enabled = false;
                    dbFunction.SetButtonIconImage(btnNoRequestID);

                    btnNoReferenceNo.Enabled = false;
                    dbFunction.SetButtonIconImage(btnNoReferenceNo);

                    txtServiceJobType.Text = dbAPI.getServiceJobType(txtSearchSTJobTypeDescription.Text).ToString();

                    txtRequestID1.Text = dbAPI.getPrimaryIRNo(int.Parse(txtIRIDNo.Text));
                   
                    // Hold value for Request ID / Reference No
                    pHoldEntryRequestID = StrClean(txtEntryRequestID.Text);
                    pHoldEntryReferenceNo = StrClean(txtEntryReferenceNo.Text);

                    setFSRMode();

                    //checkCancelJOButton();

                    btnCancelJO.Enabled = true;

                    btnUpdateServiceDate.Enabled = true;

                    btnUpdateMerchRep.Enabled = true;

                    btnUpdateServiceType.Enabled = true;

                    AdditionalComBoBoxUnlock(true);

                    // Init header
                    lblHeader.Text = "UPDATE JOB ORDER" + " " + dbFunction.AddBracketStartEnd(cboSearchServiceType.Text) + " " + dbFunction.AddBracketStartEnd(txtIRTID.Text) + " " + dbFunction.AddBracketStartEnd(txtIRMID.Text);
                    lblSubHeader.Text = txtSearchSTJobTypeDescription.Text + " - " + txtRequestNo.Text;

                    tabFillUp.TabIndex = 0;

                    if (dbFunction.isValidID(txtSearchFSRNo.Text))
                        getSignAndImageCount();

                    btnClear.Focus();

                    Cursor.Current = Cursors.Default;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exceptional error message {ex.Message}");
                    dbFunction.SetMessageBox($"Exceptional error message {ex.Message}", "Search: Job Order", clsFunction.IconType.iError);
                }
                
               
            }
        }

        /*
        private void InitSearchButton(bool isClear)
        {
            if (isClear)
            {
                btnSearchMerchant.Enabled = false;
                btnSearchService.Enabled = true;
            }
            else
            {
                if (!lblMainStatus.Text.Equals(clsFunction.sDash) && lblMainStatus.Text.Substring(0, 3).Equals("NEW"))
                {
                    btnSearchMerchant.Enabled = true;
                    btnSearchService.Enabled = false;
                }
                else
                {
                    btnSearchMerchant.Enabled = false;
                    btnSearchService.Enabled = true;
                }
            }          
        }
        */

        private void FiillServicingInfo()
        {
            Debug.WriteLine("--FiillServicingInfo--");
            Debug.WriteLine("fEdit="+fEdit);
            Debug.WriteLine("txtMerchantID.Text=" + txtMerchantID.Text);
            Debug.WriteLine("txtIRIDNo.Text=" + txtIRIDNo.Text);
            Debug.WriteLine("txtSearchServiceNo.Text=" + txtSearchServiceNo.Text);

            txtServiceReferenceNo.Text =
            //txtServiceCode.Text =
            //txtSearchServiceDate.Text =
            //txtServiceRequestDate.Text =
            //txtProcessedBy.Text =
            //txtProcessedDate.Text =
            //txtModifiedBy.Text =
            //txtModifiedDate.Text =
            //txtDispatchBy.Text =
            //txtDispatchDate.Text =
            //txtJobType.Text =
            //txtJobTypeDescription.Text =
            //txtServiceJobTypeStatusDesc.Text =
            //txtMMerchRemarks.Text =
            //txtMAppvesion.Text =
            //txtMAppCRC.Text =
            txtServiceRequestNo.Text =
            txtServiceReferenceNo.Text =
            txtEntryRequestID.Text =
            txtEntryReferenceNo.Text =
            txtServiceRequestDate.Text =
            txtServiceJobType.Text = 
            txtServiceJobTypeStatusDesc.Text =
            txtRemarks.Text =
            txtFUAppVersion.Text =
            txtFUAppCRC.Text =
            txtServiceJobTypeDesc.Text = 
            txtServiceStatus.Text =
            txtServiceStatusDescription.Text =
            txtCreatedDate.Text =
            txtCreatedTime.Text =
            txtDispatchDateTime.Text =
            txtDispatchBy.Text =           
            txtDispatchDate.Text =
            txtDispatchTime.Text =
            lblCreatedDate.Text = 
            txtTicketStatus.Text =
            clsFunction.sNull;

            if (dbFunction.isValidID(txtSearchServiceNo.Text))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Servicing Info", txtSearchServiceNo.Text, "Get Info Detail", "", "GetInfoDetail");

                // parse delimited
                dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                if (dbAPI.isNoRecordFound() == false)
                {
                    //txtServiceNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                    //txtServiceCode.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtServiceRequestNo.Text = txtRequestNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                    txtServiceReferenceNo.Text = txtEntryReferenceNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);

                    txtProcessedBy.Text = txtDispatcher.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);                    
                    txtProcessedDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);

                    lblCreatedDate.Text = "CREATED DATE: " + dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                    txtServiceRequestDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 21);
                    txtSearchServiceDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 23);
                    //txtServiceRequestDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5);
                    //txtProcessedBy.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                    //txtProcessedDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                    //txtModifiedDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                    //txtModifiedDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                    //txtDispatchBy.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                    //txtDispatchDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 11);

                    txtServiceJobType.Text = txtSearchSTJobType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 12);
                    
                    txtSearchSTJobTypeDescription.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);

                    txtServiceJobTypeStatusDesc.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 14);

                    txtRemarks.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 15);
                    txtFUAppVersion.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 16);
                    txtFUAppCRC.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 17);

                    //txtServiceStatusDescription.Text = txtSearchSTStatusDescription.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 14);

                    txtServiceJobTypeDesc.Text = txtSearchSTServiceJobTypeDescription.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 18);

                    
                    //txtSearchSTStatusDescription.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 18);

                    txtServiceStatus.Text = txtSearchSTStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 19);
                    txtServiceStatusDescription.Text = txtSearchSTStatusDescription.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 35);

                    //txtServiceJobTypeStatusDesc.Text = txtSearchSTServiceJobTypeDescription.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 20);

                    dteReqInstallationDate.Value = DateTime.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 26));
                    dteReqTime.Value = DateTime.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 22));

                    // Created
                    txtCreatedDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 39);
                    txtCreatedTime.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 40);

                    // Check dispatch
                    if (int.Parse(dbFunction.CheckAndSetNumericValue(txtServiceStatus.Text)).Equals(clsGlobalVariables.STATUS_ALLOCATED))
                        chkDispatch.Checked = false;
                    else
                        chkDispatch.Checked = true;

                    txtSearchFSRNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 25);

                    dteServiceReqDate.Value = DateTime.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 21));

                    txtProbReported.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 27);

                    txtRMInstruction.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 28);

                    // Dispatch -- sidumili
                    txtDispatchBy.Text = txtDispatcher.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 29);
                    txtDispatchDateTime.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 30);
                    txtDispatchDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 31);
                    txtDispatchTime.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 32);

                    txtEntryRequestID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 33);

                    setTicketStatus(int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 36)));

                    txtDispatchID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 41);

                    // Helpdesk
                    txtAssistNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 45);
                    txtProblemNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 46);

                }

                // fill additional info
                ServicingDetailController data = _mServicingDetailController.getInfo(dbFunction.CheckAndSetNumericValue(txtSearchServiceNo.Text) + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text));
                if (data != null)
                {
                    cboBillingType.Text = data.BillingType;
                    cboSource.Text = data.Source;
                    cboCategory.Text = data.Category;
                    cboSubCategory.Text = data.SubCategory;
                }
            }
        }

        private void FillServicingSNInfo()
        {
            txtCurTerminalID.Text =
            txtCurTerminalSN.Text =
            txtCurSIMID.Text =
            txtCurSIMSN.Text =
            txtRepTerminalID.Text =
            txtRepTerminalSN.Text =
            txtRepSIMID.Text =
            txtRepSIMSN.Text = clsFunction.sNull;

            if (dbFunction.isValidID(txtSearchServiceNo.Text))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Servicing SN Info", txtSearchServiceNo.Text, "Get Info Detail", "", "GetInfoDetail");

                dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);
                
                if (dbAPI.isNoRecordFound() == false)
                {
                    //txtServiceNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                    txtCurTerminalID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtCurTerminalSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                    txtCurSIMID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                    txtCurSIMSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                    txtRepTerminalID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5);
                    txtRepTerminalSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                    txtRepSIMID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                    txtRepSIMSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                }

                //clsSearch.ClassTerminalID = int.Parse(dbFunction.CheckAndSetNumericValue(txtCurTerminalID.Text));
                //clsSearch.ClassSIMID = int.Parse(dbFunction.CheckAndSetNumericValue(txtCurSIMID.Text));
                //clsSearch.ClassRepTerminalID = int.Parse(dbFunction.CheckAndSetNumericValue(txtRepTerminalID.Text));
                //clsSearch.ClassRepSIMID = int.Parse(dbFunction.CheckAndSetNumericValue(txtRepSIMID.Text));

                // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // [CURRENT]
                // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // --------------------------------------------------------------------------------------------------------------------
                // Get Terminal SN Info (Current)
                // --------------------------------------------------------------------------------------------------------------------
                txtCurTerminalCode.Text =
                txtCurTerminalType.Text =
                txtCurTerminalModel.Text =
                txtCurTerminalBrand.Text =
                txtCurTerminalLocation.Text =
                txtCurTerminalAssetType.Text = clsFunction.sNull;

                if (dbFunction.isValidID(txtCurTerminalID.Text))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "Terminal SN Info", txtCurTerminalID.Text, "Get Info Detail", "", "GetInfoDetail");

                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtCurTerminalCode.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                        txtCurTerminalType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                        txtCurTerminalModel.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                        txtCurTerminalBrand.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                        txtCurTerminalLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);
                        txtCurTerminalAssetType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 15);
                        txtCurTerminalStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 29);
                        txtCurTerminalStatusDesc.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 30);
                    }                    
                }

                // --------------------------------------------------------------------------------------------------------------------
                // Get SIM SN Info (Current)
                // --------------------------------------------------------------------------------------------------------------------
                txtCurSIMCarrier.Text =
                txtCurSIMLocation.Text = clsFunction.sNull;

                if (dbFunction.isValidID(txtCurSIMID.Text))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "SIM SN Info", txtCurSIMID.Text, "Get Info Detail", "", "GetInfoDetail");

                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtCurSIMCarrier.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                        txtCurSIMLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                        txtCurSIMStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 17);
                        txtCurSIMStatusDesc.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 18);
                    }
                }
                // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // [CURRENT]
                // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // [REPLACE]
                // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // --------------------------------------------------------------------------------------------------------------------
                // Get Terminaal SN Info (Replace)
                // --------------------------------------------------------------------------------------------------------------------
                txtRepTerminalCode.Text =
                txtRepTerminalType.Text =
                txtRepTerminalModel.Text =
                txtRepTerminalBrand.Text =
                txtRepTerminalLocation.Text =
                txtRepTerminalAssetType.Text = clsFunction.sNull;

                if (dbFunction.isValidID(txtRepTerminalID.Text))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "Terminal SN Info", txtRepTerminalID.Text, "Get Info Detail", "", "GetInfoDetail");

                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtRepTerminalCode.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                        txtRepTerminalType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                        txtRepTerminalModel.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                        txtRepTerminalBrand.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                        txtRepTerminalLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);
                        txtRepTerminalAssetType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 15);
                        txtRepTerminalStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 29);
                        txtRepTerminalStatusDesc.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 30);
                    }
                }


                // --------------------------------------------------------------------------------------------------------------------
                // Get SIM SN Info (Replace)    
                // --------------------------------------------------------------------------------------------------------------------
                txtRepSIMCarrier.Text =
                txtRepSIMLocation.Text = clsFunction.sNull;

                if (dbFunction.isValidID(txtRepSIMID.Text))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "SIM SN Info", txtRepSIMID.Text, "Get Info Detail", "", "GetInfoDetail");

                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtRepSIMCarrier.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                        txtRepSIMLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                        txtRepSIMStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 17);
                        txtRepSIMStatusDesc.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 18);
                    }
                }
                
                // ----------------------------------------------------------------------------------------------
                // Set old Terminal, current
                // ----------------------------------------------------------------------------------------------
                if (dbFunction.isValidID(txtCurTerminalID.Text))
                {
                    txtOldTerminalID.Text = txtCurTerminalID.Text;
                    txtOldTerminalSN.Text = txtCurTerminalSN.Text;
                }

                // ----------------------------------------------------------------------------------------------
                // Set old SIM, current
                // ----------------------------------------------------------------------------------------------
                if (dbFunction.isValidID(txtCurSIMID.Text))
                {
                    txtOldSIMID.Text = txtCurSIMID.Text;
                    txtOldSIMSN.Text = txtCurSIMSN.Text;
                }

                // ----------------------------------------------------------------------------------------------
                // Set old Terminal, replace
                // ----------------------------------------------------------------------------------------------
                if (dbFunction.isValidID(txtRepTerminalID.Text))
                {
                    txtOldTerminalID.Text = txtRepTerminalID.Text;
                    txtOldTerminalSN.Text = txtRepTerminalSN.Text;
                }

                // ----------------------------------------------------------------------------------------------
                // Set old SIM, replace
                // ----------------------------------------------------------------------------------------------
                if (dbFunction.isValidID(txtRepSIMID.Text))
                {
                    txtOldSIMID.Text = txtRepSIMID.Text;
                    txtOldSIMSN.Text = txtRepSIMSN.Text;
                }
               
                // Do not display replace SN for non installation/replacement for not update
                if (!fEdit)
                {                
                    if (!txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
                    {
                        txtRepTerminalID.Text =
                        txtRepTerminalSN.Text =
                        txtRepSIMID.Text =
                        txtRepSIMSN.Text = clsFunction.sNull;

                        txtRepTerminalCode.Text =
                        txtRepTerminalType.Text =
                        txtRepTerminalModel.Text =
                        txtRepTerminalBrand.Text =
                        txtRepTerminalLocation.Text =
                        txtRepTerminalAssetType.Text = clsFunction.sNull;

                        txtRepSIMCarrier.Text =
                        txtRepSIMLocation.Text = clsFunction.sNull;
                    }
                }          

            }
        }

        private void FillMerchRepTextBox()
        {

            txtCustomerName.Text =
            txtCustomerPosition.Text =
            txtCustomerContactNo.Text = 
            txtCustomerEmail.Text = clsFunction.sNull;

            if (dbFunction.isValidID(txtSearchServiceNo.Text))          
                dbAPI.ExecuteAPI("GET", "Search", "Servicing Merch Rep Info", txtSearchServiceNo.Text, "Get Info Detail", "", "GetInfoDetail");
            else
                dbAPI.ExecuteAPI("GET", "Search", "Merch Rep Info", txtMerchantID.Text, "Get Info Detail", "", "GetInfoDetail");
            
            if (dbAPI.isNoRecordFound() == false)
            {
                txtCustomerName.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                txtCustomerPosition.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                txtCustomerContactNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                txtCustomerEmail.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);

            }
        }

        private void FiillFSRInfo()
        {   
            txtSearchFSRDate.Text =
            txtSearchFSRTimeArrived.Text =
            txtSearchFSRReceiptTime.Text =
            txtSearchFSRTimeStart.Text =
            txtSearchFSRTimeEnd.Text =
            txtSearchFSRServiceResult.Text =
            txtFSRRequestNo.Text =
            txtBillable.Text =
            txtMobileID.Text =
            txtMobileTerminalID.Text =
            txtMobileVersion.Text =
            clsFunction.sNull;

            if (dbFunction.isValidID(txtSearchServiceNo.Text))
            {
                dbAPI.ExecuteAPI("GET", "Search", "FSR Info", txtSearchServiceNo.Text + clsFunction.sPipe + txtSearchFSRNo.Text, "Get Info Detail", "", "GetInfoDetail");

                dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                if (dbAPI.isNoRecordFound() == false)
                {                
                    txtSearchFSRNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtSearchFSRDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                    txtSearchFSRTimeArrived.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                    txtSearchFSRReceiptTime.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                    txtSearchFSRTimeStart.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5);
                    txtSearchFSRTimeEnd.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                    txtSearchFSRServiceResult.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                    txtFSRRequestNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 22);
                    txtAttemptCnt.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 25);
                    txtBillable.Text = (dbFunction.isValidID(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 23)) ? clsDefines.gYes : clsDefines.gNo);

                    txtMobileID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 26);
                    txtMobileTerminalID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 27);
                    txtMobileVersion.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 28);

                    txtDiagnostic.Text = (dbAPI.isRecordExist("Search", "Diagnostic Detail", txtSearchServiceNo.Text + clsDefines.gPipe + txtSearchFSRNo.Text) ? clsDefines.gYes : clsDefines.gNo);
                    string pFileName = txtSearchServiceNo.Text + "_" + dbFunction.padLeftChar(clsDefines.MERCHANT_SIGNATURE_INDEX.ToString(), "0", 2) + clsDefines.FILE_EXT_PNG;
                    txtMerchantSign.Text = (dbAPI.isFileExist("Search", "Check Upload File", pFileName) ? clsDefines.gYes : clsDefines.gNo);

                }
            }
        }

        private bool isValidTerminalStatus()
        {
            bool isValid = false;
            int iStatus = 0;
            int iLastServiceNo = 0;
            int iLastIRIDNo = 0;
            string pID = clsFunction.sZero;
            string pSN = clsFunction.sZero;

            Debug.WriteLine("--isValidTerminalStatus--");
            Debug.WriteLine("txtSearchSTJobTypeDescription.Text="+ txtSearchSTJobTypeDescription.Text);

            if (dbFunction.isValidID(txtCurTerminalID.Text) || dbFunction.isValidID(txtRepTerminalID.Text))
            {
                if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC))
                {
                    pID = dbFunction.CheckAndSetNumericValue(txtCurTerminalID.Text);
                    pSN = txtCurTerminalSN.Text;
                }
                /*
                else if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
                {
                    pID = dbFunction.CheckAndSetNumericValue(txtRepTerminalID.Text);
                    pSN = txtRepTerminalSN.Text;
                }
                */
                else
                {
                    return true;
                }

                Debug.WriteLine("pID=" + pID);
                Debug.WriteLine("pSN=" + pSN);

                if (dbFunction.isValidID(txtMerchantID.Text) && dbFunction.isValidID(pID) && dbFunction.isValidID(txtIRIDNo.Text))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "Terminal SN Info", pID, "Get Info Detail", "", "GetInfoDetail");

                    if (dbAPI.isNoRecordFound() == false)
                    {

                        iStatus = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4));
                        iLastServiceNo = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 20));
                        iLastIRIDNo = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 21));
                    }
                }

                if (iStatus > 0)
                {
                    if (iStatus.Equals(clsGlobalVariables.STATUS_ALLOCATED))
                    {
                        if (iLastServiceNo > 0)
                        {
                            if (int.Parse(txtSearchServiceNo.Text).Equals(iLastServiceNo))
                            {
                                isValid = true;
                            }
                            else
                            {
                                isValid = false;
                            }
                        }
                        else
                        {
                            isValid = true;
                        }
                    }
                    else if (iStatus.Equals(clsGlobalVariables.STATUS_AVAILABLE))
                    {
                        isValid = true;
                    }
                    else
                    {
                        isValid = false;
                    }
                }
            }
            else
            {
                isValid = true;
            }
            
            if (!isValid)
            {
                dbFunction.SetMessageBox("Terminal SN " + pSN + " is no longer available.", "Required field.", clsFunction.IconType.iExclamation);
            }

            return isValid;
        }

        private bool isValidSIMStatus()
        {
            bool isValid = false;
            int iStatus = 0;
            int iLastServiceNo = 0;
            int iLastIRIDNo = 0;
            string pID = clsFunction.sZero;
            string pSN = clsFunction.sZero;

            Debug.WriteLine("--isValidSIMStatus--");
            Debug.WriteLine("txtSearchSTJobTypeDescription.Text=" + txtSearchSTJobTypeDescription.Text);

            if (dbFunction.isValidID(txtCurSIMID.Text) || dbFunction.isValidID(txtRepSIMID.Text))
            {
                if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC))
                {
                    pID = dbFunction.CheckAndSetNumericValue(txtCurSIMID.Text);
                    pSN = txtCurSIMSN.Text;
                }
                /*
                else if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
                {
                    pID = dbFunction.CheckAndSetNumericValue(txtRepSIMID.Text);
                    pSN = txtRepSIMSN.Text;
                }
                */
                else
                {
                    return true;
                }

                Debug.WriteLine("pID=" + pID);
                Debug.WriteLine("pSN=" + pSN);

                if (dbFunction.isValidID(txtMerchantID.Text) && dbFunction.isValidID(pID) && dbFunction.isValidID(txtIRIDNo.Text))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "SIM SN Info", pID, "Get Info Detail", "", "GetInfoDetail");

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        iStatus = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1));
                        iLastServiceNo = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10));
                        iLastIRIDNo = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 11));
                    }
                }

                if (iStatus > 0)
                {
                    if (iStatus.Equals(clsGlobalVariables.STATUS_ALLOCATED))
                    {
                        if (iLastServiceNo > 0)
                        {
                            if (int.Parse(txtSearchServiceNo.Text).Equals(iLastServiceNo))
                            {
                                isValid = true;
                            }
                            else
                            {
                                isValid = false;
                            }
                        }
                        else
                        {
                            isValid = true;
                        }
                    }
                    else if (iStatus.Equals(clsGlobalVariables.STATUS_AVAILABLE))
                    {
                        isValid = true;
                    }
                    else
                    {
                        isValid = false;
                    }
                }
            }
            else
            {
                isValid = true;
            }
            
            if (!isValid)
            {
                dbFunction.SetMessageBox("SIM SN " + pSN + " is no longer available.", "Required field.", clsFunction.IconType.iExclamation);
            }

            return isValid;
        }

        private void EmailNotification(string pPrefix)
        {
            Debug.WriteLine("--EmailNotification--");
            
            dbFunction.GetProcessedByAndDateTime();

            // Get User Mobile/Email
            clsUser.ClassProcessedContactNo = clsUser.ClassProcessedEmail = clsFunction.sDash;
            dbAPI.ExecuteAPI("GET", "Search", "User Info", clsUser.ClassUserID.ToString(), "Get Info Detail", "", "GetInfoDetail");         
            if (dbAPI.isNoRecordFound() == false)
            {
                clsUser.ClassProcessedContactNo = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                clsUser.ClassProcessedEmail = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
            }

            clsSearch.ClassAdvanceSearchValue = cboSearchServiceType.Text + clsFunction.sCaret + dteReqInstallationDate.Text + clsFunction.sCaret + txtRequestNo.Text + clsFunction.sCaret + txtSearchIRNo.Text + clsFunction.sCaret + txtEntryReferenceNo.Text + clsFunction.sCaret + txtRemarks.Text + clsFunction.sCaret + txtRMInstruction.Text + clsFunction.sCaret +
                                                txtFUAppVersion.Text + clsFunction.sCaret + txtFUAppCRC.Text + clsFunction.sCaret +
                                                txtClientName.Text + clsFunction.sCaret + txtClientAddress.Text + clsFunction.sCaret + txtClientContactPerson.Text + clsFunction.sCaret + txtClientMobileNo.Text + "/" + txtClientTelNo.Text + clsFunction.sCaret +
                                                txtMerchantName.Text + clsFunction.sCaret + txtMerchantAddress.Text + clsFunction.sCaret + txtIRTID.Text + clsFunction.sCaret + txtIRMID.Text + clsFunction.sCaret + txtMerchantContactPerson.Text + clsFunction.sCaret + txtMerchantMobileNo.Text + "/" + txtMerchantTelNo.Text + clsFunction.sCaret +
                                                txtFEName.Text + clsFunction.sCaret + txtFEMobileNo.Text + clsFunction.sCaret + txtFEEmail.Text + clsFunction.sCaret +
                                                txtCurTerminalSN.Text + clsFunction.sCaret + txtCurTerminalType.Text + clsFunction.sCaret + txtCurTerminalModel.Text + clsFunction.sCaret + txtCurTerminalBrand.Text + clsFunction.sCaret +
                                                txtRepTerminalSN.Text + clsFunction.sCaret + txtRepTerminalType.Text + clsFunction.sCaret + txtRepTerminalModel.Text + clsFunction.sCaret + txtRepTerminalBrand.Text + clsFunction.sCaret +
                                                txtCurSIMSN.Text + clsFunction.sCaret + txtCurSIMCarrier.Text + clsFunction.sCaret +
                                                txtRepSIMSN.Text + clsFunction.sCaret + txtRepSIMCarrier.Text + clsFunction.sCaret +
                                                clsUser.ClassProcessedBy + clsFunction.sCaret + clsUser.ClassProcessedDateTime + clsFunction.sCaret + clsUser.ClassProcessedContactNo + clsFunction.sCaret + clsUser.ClassProcessedEmail + clsFunction.sCaret +
                                                pPrefix;

            Debug.WriteLine("clsSearch.ClassAdvanceSearchValue="+ clsSearch.ClassAdvanceSearchValue);
            dbAPI.ExecuteAPI("POST", "Notify", "Job Order", clsSearch.ClassAdvanceSearchValue, "Email Notification", "", "EmailNotification");
        }

        private void SetCount()
        {
            Debug.WriteLine("--SetCount--");
            Debug.WriteLine("fEdit="+fEdit);

            int iCount = 0;
            
            // Success
            iCount = clsTerminal.ClassTerminalCount = 0;
            clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtSearchSTJobType.Text) + clsFunction.sPipe + clsGlobalVariables.ACTION_MADE_SUCCESS;
            dbAPI.GetViewCount("Search", "Action Made Counter", clsSearch.ClassAdvanceSearchValue, "Get Count");
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            txtSuccessCnt.Text = iCount.ToString();

            // Negative
            iCount = clsTerminal.ClassTerminalCount = 0;
            clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtSearchSTJobType.Text) + clsFunction.sPipe + clsGlobalVariables.ACTION_MADE_NEGATIVE;
            dbAPI.GetViewCount("Search", "Action Made Counter", clsSearch.ClassAdvanceSearchValue, "Get Count");
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            txtNegativeCnt.Text = iCount.ToString();

            if (!fEdit)
            {
                // Attempt
                iCount = clsTerminal.ClassTerminalCount = 0;
                clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtSearchSTJobType.Text);
                dbAPI.GetViewCount("Search", "Attempt Counter", clsSearch.ClassAdvanceSearchValue, "Get Count");
                if (dbAPI.isNoRecordFound() == false)
                    iCount = clsTerminal.ClassTerminalCount;

                txtAttemptCnt.Text = iCount.ToString();
            }
           
        }

        private void InitCount()
        {
            txtSuccessCnt.Text = txtNegativeCnt.Text = txtAttemptCnt.Text = clsFunction.sZero;
        }

        private bool CheckDateFromTo(DateTime objFrom, DateTime objTo)
        {
            bool fValid = true;
            int iResult;

            Debug.WriteLine("--CheckDateFromTo--");
            Debug.WriteLine("objFrom="+ objFrom.ToString());
            Debug.WriteLine("objTo=" + objTo.ToString());

            iResult = DateTime.Compare(DateTime.Parse(objFrom.ToShortDateString()), DateTime.Parse(objTo.ToShortDateString()));

            Debug.WriteLine("iResult="+ iResult);

            if (iResult > 0)
                fValid = false;

            if (!fValid)
            {
                MessageBox.Show("[Schedule Date] should be greater or equal [Request Date]" +
                                        "\n\n" +
                                        "Schedule Date:  " + objTo.ToString("MM-dd-yyyy") +
                                        Environment.NewLine +
                                        "Request Date:   " + objFrom.ToString("MM-dd-yyyy"), "Date Range", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

            }

            return fValid;
        }

        private void btnSendEmail_Click(object sender, EventArgs e)
        {
            if (dbFunction.isValidID(txtMerchantID.Text) && dbFunction.isValidID(txtIRIDNo.Text) && dbFunction.isValidID(txtSearchServiceNo.Text))
            {
                // Check email for FSR
                if (dbFunction.isValidID(txtSearchFSRNo.Text))
                {
                    dbFunction.SetMessageBox("Service is already completed.\nGo to FSR and search this record inorder to send email.", "Email failed.", clsFunction.IconType.iWarning);
                    return;
                }

                if (MessageBox.Show("Are you sure you want to send email notification.\n\n" +
               "Job Order: " + cboSearchServiceType.Text + "\n" +
               "Vendor Representative: " + txtFEName.Text + "\n" +
               "Email: " + txtFEEmail.Text + "\n\n" +
               "", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    return;
                }

                Cursor.Current = Cursors.WaitCursor;

                EmailNotification("RE: ");

                if (clsGlobalVariables.isAPIResponseOK)
                    dbFunction.SetMessageBox("Email notification sent.", "Email notification", clsFunction.IconType.iInformation);
                else
                    dbFunction.SetMessageBox("Email notification failed.", "Email notification", clsFunction.IconType.iWarning);

                Cursor.Current = Cursors.Default;
            }
            else
            {
                dbFunction.SetMessageBox("No selected job order.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);
            }
            
        }

        private void initDispatch(bool isEnable)
        {
            chkDispatch.Enabled = chkDispatch.Checked = false;
            if (isEnable)
                chkDispatch.Enabled = chkDispatch.Checked = true;

        }

        private void panel134_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dtpReqDateVendor_ValueChanged(object sender, EventArgs e)
        {

        }

        private void frmServiceJobOrder_Activated(object sender, EventArgs e)
        {
            btnClear.Focus();
        }

        private void InitChangesListView()
        {
            string outField = "";
            int outWidth = 0;
            string outTitle = "";
            HorizontalAlignment outAlign = 0;
            bool outVisible = false;
            bool outAutoWidth = false;
            string outFormat = "";

            lvwChanges.Clear();
            lvwChanges.View = View.Details;

            dbFunction.GetListViewHeaderColumnFromFile("", "Line#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwChanges.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "MapID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwChanges.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ChangeDescription", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwChanges.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ChangeFrom", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwChanges.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ChangeTo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwChanges.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "MaxLimit", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwChanges.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "OptionType", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwChanges.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "FieldName", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwChanges.Columns.Add(outTitle, outWidth, outAlign);

        }

        private void lvwChanges_DoubleClick(object sender, EventArgs e)
        {
            string pOutput = "";
            if (lvwChanges.Items.Count > 0)
            {
                string pSelectedRow = clsSearch.ClassRowSelected = dbFunction.GetListViewSelectedRow(lvwChanges, 0);
                Debug.WriteLine("pSelectedRow=\n" + pSelectedRow);

                if (int.Parse(dbFunction.GetSearchValue("MapID")) > 0)
                {
                    string pDescription = dbFunction.GetSearchValue("Description");
                    string pChangeFrom = dbFunction.GetSearchValue("Change From");
                    string pLineNo = dbFunction.GetSearchValue("LINE#");
                    string pMaxLimit = dbFunction.GetSearchValue("MaxLimit");
                    int pOptionType = int.Parse(dbFunction.CheckAndSetNumericValue(dbFunction.GetSearchValue("OptionType")));

                    bool isValid = dbFunction.ShowMenuInputBox("Change entry for " + dbFunction.AddBracketStartEnd(pDescription) + "-" + dbFunction.AddBracketStartEnd($"Entry limit: {pMaxLimit}"), pChangeFrom, pDescription, int.Parse(pMaxLimit), pOptionType, ref pOutput);

                    Debug.WriteLine("isValid="+ isValid + ",pOutput="+ pOutput);

                    if (isValid)
                    {
                        dbFunction.updateListViewByColRow(lvwChanges, 4, int.Parse(pLineNo), pOutput);
                    }
                }             
            }            
        }

        private void btnRemoveChanges_Click(object sender, EventArgs e)
        {   
            dbAPI.FillListViewChangesMapping(lvwChanges, "", "");
        }

        private void saveChangesDetail()
        {
            string sRowSQL = "";
            string sSQL = "";

            if (lvwChanges.Items.Count > 0)
            {
                foreach (ListViewItem i in lvwChanges.Items)
                {
                    string pMapID = i.SubItems[1].Text;
                    string pChangeFrom = i.SubItems[3].Text;
                    string pChangeTo = i.SubItems[4].Text;

                    if (dbFunction.isValidID((pMapID)) && dbFunction.isValidDescription(pChangeTo))
                    {

                        // Insert                
                        sRowSQL = "";
                        sRowSQL = " (" + dbFunction.CheckAndSetNumericValue(txtSearchServiceNo.Text) + ", " +
                        sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + ", " +
                        sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(txtMerchantID.Text) + ", " +
                        sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(pMapID) + ", " +
                        sRowSQL + sRowSQL + " '" + pChangeFrom + "', " +
                        sRowSQL + sRowSQL + " '" + pChangeTo + "') ";

                        if (sSQL.Length > 0)
                            sSQL = sSQL + ", " + sRowSQL;
                        else
                            sSQL = sSQL + sRowSQL;
                    }                    
                }

                Debug.WriteLine("sSQL=" + sSQL);
                if (sSQL.Length > 0)
                {
                    dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 1);
                    dbAPI.ExecuteAPI("POST", "Insert", "", 
                        dbFunction.CheckAndSetNumericValue(txtSearchServiceNo.Text) + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text), 
                        "Service Changes Detail", sSQL, "InsertCollectionDetail");
                }
                
            }
        }

        private void loadServiceChangesDetail()
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            lvwChanges.Items.Clear();

            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtIRIDNo.Text))
            {
                dbAPI.ExecuteAPI("GET", "View", "eFSR Changes Service", dbFunction.CheckAndSetNumericValue(txtSearchServiceNo.Text) + clsFunction.sPipe +
                                                                    dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text)
                , "Advance Detail", "", "ViewAdvanceDetail");

                if (!clsGlobalVariables.isAPIResponseOK) return;

                if (dbAPI.isNoRecordFound() == false)
                {
                    lvwChanges.Items.Clear();
                    while (clsArray.MapID.Length > i)
                    {
                        // Add to List
                        iLineNo++;
                        ListViewItem item = new ListViewItem(iLineNo.ToString());
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MapID));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_Description));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ChangeFrom));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ChangeTo));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MaxLimit));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FieldType));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FieldName));

                        lvwChanges.Items.Add(item);

                        i++;
                    }

                    dbFunction.ListViewAlternateBackColor(lvwChanges);
                }
                
            }
            
        }

        private bool isValidSNStatus(searchType searchType)
        {
            bool isValid = false;

            if (searchType == searchType.iTerminal)
            {
                if (dbFunction.isValidID(txtCurTerminalStatus.Text))
                {
                    if (!txtCurTerminalStatus.Text.Equals(clsSystemSetting.ClassSystemSNStatusID))
                        isValid = false;
                    else
                        isValid = true;
                }
                else
                {
                    dbFunction.SetMessageBox("Current terminal status must not be blank.", "Required field.", clsFunction.IconType.iError);
                }

                if (!isValid)
                {
                    dbFunction.SetMessageBox("Invalid current terminal status." +
                            "\n\n" +
                            " > SN : " + txtCurTerminalSN.Text, "Required field.", clsFunction.IconType.iExclamation);
                }
            }

            if (searchType == searchType.iSIM)
            {
                if (dbFunction.isValidID(txtCurSIMStatus.Text))
                {
                    if (!txtCurSIMStatus.Text.Equals(clsSystemSetting.ClassSystemSNStatusID))
                        isValid = false;
                    else
                        isValid = true;
                }
                else
                {
                    dbFunction.SetMessageBox("Current SIM status must not be blank.", "Required field.", clsFunction.IconType.iError);
                }

                if (!isValid)
                {
                    dbFunction.SetMessageBox("Invalid current terminal status." +
                            "\n\n" +
                            " > SN : " + txtCurSIMSN.Text, "Required field.", clsFunction.IconType.iExclamation);
                }
            }

            return isValid;
        }
        
        private void btnCheck_Click(object sender, EventArgs e)
        {
            frmAppsInfo frm = new frmAppsInfo();
            frm.ShowDialog();
        }

        private void getApplicationInfo()
        {
            txtFUAppVersion.Text = txtFUAppCRC.Text = clsFunction.sNull;

            if (dbFunction.isValidDescription(txtCurTerminalModel.Text) || dbFunction.isValidDescription(txtRepTerminalModel.Text))
            {
                if (dbFunction.isValidID(txtCurTerminalID.Text))
                    dbAPI.ExecuteAPI("GET", "Search", "Application Version/CRC Info", txtCurTerminalModel.Text, "Get Info Detail", "", "GetInfoDetail");

                if (dbFunction.isValidID(txtRepTerminalID.Text))
                    dbAPI.ExecuteAPI("GET", "Search", "Application Version/CRC Info", txtRepTerminalModel.Text, "Get Info Detail", "", "GetInfoDetail");
                
                Debug.WriteLine("clsSearch.ClassOutParamValue=" + clsSearch.ClassOutParamValue);

                if (clsSearch.ClassOutParamValue.Length > 0)
                {
                    jsonObj obj = JsonConvert.DeserializeObject<jsonObj>(clsSearch.ClassOutParamValue);

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtFUAppVersion.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_AppVersion);
                        txtFUAppCRC.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_AppCRC);                        
                    }
                }
            }
        }

        private void btnRefreshSN_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            // current terminal
            if (dbFunction.isValidID(txtCurTerminalID.Text))
            {
                PopulateTerminalTextBox(txtCurTerminalID.Text, txtCurTerminalSN.Text, true);               
            }

            // current sim
            if (dbFunction.isValidID(txtCurSIMID.Text))
            {
                PopulateSIMTextBox(txtCurSIMID.Text, txtCurSIMSN.Text, true);              
            }

            // replace terminal
            if (dbFunction.isValidID(txtRepTerminalID.Text))
            {
                PopulateTerminalTextBox(txtRepTerminalID.Text, txtRepTerminalSN.Text, false);               
            }

            // replace sim
            if (dbFunction.isValidID(txtRepSIMID.Text))
            {
                PopulateSIMTextBox(txtRepSIMID.Text, txtRepSIMSN.Text, false);           
            }

            btnClear.Focus();

            Cursor.Current = Cursors.Default;
        }

        private bool isValidMerchantSN()
        {
            bool isValid = true;
            int pTerminalID = clsFunction.iZero;
            string pTerminalSN = clsFunction.sZero;
            int pTerminalStatus = clsFunction.iZero;
            string pTerminalStatusDesc = clsFunction.sZero;

            int pSIMID = clsFunction.iZero;
            string pSIMSN = clsFunction.sZero;
            int pSIMStatus = clsFunction.iZero;
            string pSIMStatusDesc = clsFunction.sZero;


            dbAPI.ExecuteAPI("GET", "Search", "Merchant Current SN", txtIRIDNo.Text, "Get Info Detail", "", "GetInfoDetail");
            Debug.WriteLine("clsSearch.ClassOutParamValue=" + clsSearch.ClassOutParamValue);
            if (clsSearch.ClassOutParamValue.Length > 0)
            {   
                jsonObj obj = JsonConvert.DeserializeObject<jsonObj>(clsSearch.ClassOutParamValue);

                if (dbAPI.isNoRecordFound() == false)
                {
                    pTerminalID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_TerminalID));
                    pTerminalSN = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_TerminalSN);
                    pTerminalStatus = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_TerminalStatus));
                    pTerminalStatusDesc = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_TerminalStatusDescription);

                    pSIMID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_SIMID));
                    pSIMSN = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_SIMSN);
                    pSIMStatus = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_SIMStatus));
                    pSIMStatusDesc = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_SIMStatusDescription);
                }
            }

            if ((pTerminalID > 0 && !pTerminalSN.Equals(txtCurTerminalSN.Text)) || 
                (pSIMID > 0 && !pSIMSN.Equals(txtCurSIMSN.Text)) ||
                (pTerminalStatus > 0 && pTerminalStatus != clsGlobalVariables.STATUS_INSTALLED) || 
                (pSIMStatus > 0 && pSIMStatus != clsGlobalVariables.STATUS_INSTALLED))
                isValid = false;

            if (!isValid)
            {
                dbFunction.SetMessageBox("Merchant installed/deployed SN" + "\n" +
                    " [Service information]" + "\n" +
                    " > Type : " + cboSearchServiceType.Text + "\n" +
                    " > Primary Request ID : " + txtRequestID1.Text + "\n" +
                    " > Reqiest ID : " + txtEntryRequestID.Text + "\n" +
                    " > Reference No : " + txtEntryReferenceNo.Text + "\n\n" +
                    " [Merchant information]" + "\n" +
                    " > Name : " + txtMerchantName.Text + "\n" +
                    " > TID : " + txtIRTID.Text + "\n" +
                    " > MID : " + txtIRMID.Text + "\n\n" +
                    " [Terminal information]" + "\n" +
                    " > SN : " + pTerminalSN + "\n" +
                    " > Status : " + pTerminalStatusDesc + "\n\n" +
                    " [SIM information]" + "\n" +
                    " > SN : " + pSIMSN + "\n" +
                    " > Status : " + pSIMStatusDesc + "\n\n" +                   
                    "Conflict on installed and current SN." + "\n" +
                    clsDefines.CONTACT_ADMIN_MESSAGE,
                    "Terminal/SIM SN not sync.", clsFunction.IconType.iError);
            }

            return isValid;
        }

        private void btnNoRequestID_Click(object sender, EventArgs e)
        {
            bool isAutoGen = true;
            string pIRNo = "";
            string pServiceResult = "";

            // check here if for re-use request id
            string pJSONString = dbAPI.getInfoDetailJSON("Search", "Merchant Last Service", $"{txtIRIDNo.Text}{clsDefines.gPipe}{clsGlobalVariables.ACTION_MADE_NEGATIVE}");
            if (dbFunction.isValidDescription(pJSONString))
            {
                pServiceResult = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ActionMade);
                if (pServiceResult.Equals(clsGlobalVariables.ACTION_MADE_NEGATIVE))
                {
                    pIRNo = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_IRNO);
                    dbFunction.SetMessageBox($"Please use Request ID [{pIRNo}] instead of creating new one.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
                    txtEntryRequestID.Text = pIRNo;
                    return;
                }
                
            }

            if (!dbFunction.fPromptConfirmation("Are you sure to let the system generate REQUEST ID?")) return;
            
            if (fEdit)
                isAutoGen = false;

            dbAPI.GenerateID(isAutoGen, txtEntryRequestID, txtSearchServiceNo, "Servicing Detail", clsDefines.CONTROLID_PREFIX_NO_REQUESTID);
        }

        private void btnNoReferenceNo_Click(object sender, EventArgs e)
        {
            bool isAutoGen = true;

            if (!dbFunction.fPromptConfirmation("Are you sure to let the system generate REFERENCE NO.?")) return;

            //if (!dbFunction.isValidID(txtSearchFSRNo.Text)) txtSearchServiceNo.Text = clsFunction.sZero;
            if (fEdit) isAutoGen = false;

            dbAPI.GenerateID(isAutoGen, txtEntryReferenceNo, txtSearchServiceNo, "Servicing Detail", clsDefines.CONTROLID_PREFIX_REFNO);
            
        }

        private void setFSRMode()
        {
            if (dbFunction.isValidID(txtSearchFSRNo.Text))            
                txtSearchFSRDesc.Text = (dbFunction.isValidID(txtMobileID.Text) ? clsDefines.DIGITAL_FSR : clsDefines.MANUAL_FSR);            
            else
                 txtSearchFSRDesc.Text = clsFunction.sDash;                        
        }

        private void FillMerchantContactInfoTextBox()
        {

            txtCustomerName.Text =
            txtCustomerPosition.Text =
            txtCustomerContactNo.Text =
            txtCustomerEmail.Text = clsFunction.sNull;

            if (dbFunction.isValidID(txtMerchantID.Text))
            {
                string pJSONString = dbAPI.getInfoDetailJSON("Search", "Merchant Contact Info", txtMerchantID.Text);

                dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                txtCustomerName.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ContactPerson);
                txtCustomerPosition.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ContactPosition);
                txtCustomerContactNo.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ContactNumber);
                txtCustomerEmail.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ContactEmail);

            }
        }

        private void btnViewDiagnostic_Click(object sender, EventArgs e)
        {
            if (!dbFunction.fPromptConfirmation("Are you sure to preview Diagnostic report?")) return;

            if (!dbAPI.isValidDiagnotic(txtSearchServiceNo.Text, txtSearchFSRNo.Text)) return;

            // update signature
            dbAPI.updateSignature(int.Parse(txtSearchServiceNo.Text), int.Parse(txtSearchFSRNo.Text));

            // download signature
            dbFunction.downloadSignature(clsDefines.MERCHANT_SIGNATURE_INDEX, int.Parse(txtSearchServiceNo.Text));
            dbFunction.downloadSignature(clsDefines.VENDOR_SIGNATURE_INDEX, int.Parse(txtSearchServiceNo.Text));

            // Preview report
            clsSearch.ClassIsExportToPDF = false;
            
            dbReportFunc.ViewDiagnosticReport(42);
        }

        private void setTicketStatus(int pStatus)
        {
            string pTemp = clsFunction.sNull;
            switch (pStatus)
            {
                case 0:
                    pTemp = clsDefines.OPEN_TICKET;
                    break;
                case 1:
                    pTemp = clsDefines.CLOSE_TICKET;
                    break;
            }

            txtTicketStatus.Text = pTemp;
        }

        private void btnResetServiceStatus_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtIRIDNo.Text))
            {
                if (txtServiceJobTypeStatusDesc.Text.Equals(clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC) && !dbFunction.isValidDescription(txtSearchFSRServiceResult.Text))
                {
                    if (!dbFunction.fPromptConfirmation("Are sure to reset service status?" + "\n\n" +
                        " > From: " + txtServiceJobTypeStatusDesc.Text + "\n" +
                        " > To:" + clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING_DESC)) return;

                    clsSearch.ClassSearchValue = dbFunction.CheckAndSetNumericValue(txtSearchServiceNo.Text) + clsDefines.gPipe +
                        dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + clsDefines.gPipe +
                        dbFunction.CheckAndSetNumericValue(txtServiceJobType.Text) + clsDefines.gPipe +
                        dbFunction.CheckAndSetStringValue(txtSearchSTJobTypeDescription.Text) + clsDefines.gPipe +
                        dbFunction.CheckAndSetNumericValue(txtServiceStatus.Text) + clsDefines.gPipe +
                        dbFunction.CheckAndSetStringValue(txtServiceStatusDescription.Text) + clsDefines.gPipe +
                        dbFunction.CheckAndSetStringValue(txtServiceJobTypeStatusDesc.Text) + clsDefines.gPipe +
                        dbFunction.CheckAndSetStringValue(txtSearchFSRServiceResult.Text) + clsDefines.gPipe +
                        dbFunction.CheckAndSetNumericValue(txtClientID.Text) + clsDefines.gPipe +
                        dbFunction.CheckAndSetStringValue(txtClientName.Text);

                    // parse delimited
                    dbFunction.parseDelimitedString(clsSearch.ClassSearchValue, clsDefines.gPipe, 0);

                    dbAPI.ExecuteAPI("PUT", "Update", "Service Status", clsSearch.ClassSearchValue, "", "", "UpdateCollectionDetail");

                    dbFunction.SetMessageBox("Reset service status complete.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);

                    txtServiceJobTypeStatusDesc.Text = clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING_DESC;

                    btnRefreshService_Click(this, e);

                }
                else
                {
                    dbFunction.SetMessageBox("Unable to reset service.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                }
            }
        }

        private void btnUpdateMerchRep_Click(object sender, EventArgs e)
        {
            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtMerchantID.Text))
            {
                if (!dbFunction.fPromptConfirmation("Merchant representative information" +
                "\n\n" +
                " > Merchant: " + txtMerchantName.Text + "\n" +
                " > Name: " + txtCustomerName.Text + "\n" +
                " > Position: " + txtCustomerPosition.Text + "\n" +
                " > Contact No.: " + txtCustomerContactNo.Text + "\n" +
                " > Email: " + txtCustomerEmail.Text +
                "\n\n" +
                "Are you sure to continue update?"
                )) return;

                string pSearchValue = $"{txtSearchServiceNo.Text}{clsDefines.gPipe}{txtMerchantID.Text}{clsDefines.gPipe}{txtCustomerName.Text}{clsDefines.gPipe}{txtCustomerPosition.Text}{clsDefines.gPipe}{txtCustomerContactNo.Text}{clsDefines.gPipe}{txtCustomerEmail.Text}";
                dbFunction.parseDelimitedString(pSearchValue, clsDefines.gPipe, 0);

                dbAPI.ExecuteAPI("PUT", "Update", "Merchant Representative Info",pSearchValue, "", "", "UpdateCollectionDetail");

                dbFunction.SetMessageBox("Merchant representative information updated.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
            }
        }

        private void loadTATDetail()
        {
            txtSLA.Text = txtNetworkDays.Text = txtDaysOverDue.Text = txtTATStatus.Text = clsFunction.sDash;

            dbAPI.getTATInfo(int.Parse(txtClientID.Text), int.Parse(txtIRIDNo.Text), int.Parse(txtSearchServiceNo.Text));

            txtSLA.Text = clsSearch.ClassSLA;
            txtNetworkDays.Text = clsSearch.ClassNetworkDays;
            txtDaysOverDue.Text = clsSearch.ClassDaysOverDue;
            txtTATStatus.Text = clsSearch.ClassTATStatus;

            if (txtTATStatus.Text.Equals(clsDefines.WITHIN_TAT))
                txtTATStatus.ForeColor = Color.Blue;
            else if (txtTATStatus.Text.Equals(clsDefines.BEYOND_TAT))
                txtTATStatus.ForeColor = Color.Red;
            else
                txtTATStatus.ForeColor = Color.Black;

            // compute TagetSLADate
            if (dteReqInstallationDate.Value != null)
            {
                DateTime requestDate = DateTime.Parse(dteReqInstallationDate.Value.ToString());
                DateTime slaDueDate = requestDate.AddDays(int.Parse(txtSLA.Text));

                txtTargetSLADate.Text = slaDueDate.ToString("ddd, MM-dd-yyyy");
            }

        }

        private void setMainTab()
        {
            tabMain.SelectedIndex = 0;
        }

        private void getServiceGeoLocation()
        {
            txtGeoLatitude.Text = txtGeoLongitude.Text = txtGeoCountry.Text = txtGeoLocality.Text = txtGeoAddress.Text = clsFunction.sDash;

            if (dbFunction.isValidID(txtSearchFSRNo.Text) && dbFunction.isValidID(txtSearchServiceNo.Text))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Service Geometric Location Info", txtSearchFSRNo.Text + clsDefines.gPipe + txtSearchServiceNo.Text, "Get Info Detail", "", "GetInfoDetail");

                Debug.WriteLine("clsSearch.ClassOutParamValue=" + clsSearch.ClassOutParamValue);

                if (clsSearch.ClassOutParamValue.Length > 0)
                {
                    jsonObj obj = JsonConvert.DeserializeObject<jsonObj>(clsSearch.ClassOutParamValue);

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtGeoLatitude.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_geoLatitude);
                        txtGeoLongitude.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_geoLongitude);
                        txtGeoCountry.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_geoCountry);
                        txtGeoLocality.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_geoLocality);
                        txtGeoAddress.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_geoAddress);
                    }
                }

            }
        }

        private bool isConfirmTargetInstDate()
        {
            bool isValid = true;
            DateTime stServiceReqDate = dteServiceReqDate.Value;            
            string pServiceReqDate = stServiceReqDate.ToString("MM-dd-yyy");

            if (!pServiceReqDate.Equals(txtIRInstallationDate.Text))
                isValid = false;

            if (!isValid)
            {
                isValid = dbFunction.fPromptConfirmation("Target installation date and schedule date are not equal." +
                    "\n\n" +
                    "Target installation date: " + txtIRInstallationDate.Text + "\n" +                  
                    "Schedule date: " + pServiceReqDate + "\n\n" +                                    
                    "Are you sure to continue?");

                
            }

            return isValid;
        }

        private bool isMerchantInstalled()
        {
            bool isValid = true;

            if (dbFunction.isValidID(txtIRIDNo.Text))
            {
                if (dbAPI.isRecordExist("Search", "Merchant Installed", txtIRIDNo.Text))              
                    isValid = false;
                
                if (!isValid)
                {
                    dbFunction.SetMessageBox("Unable to create service, merchant is installed", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                }
            }

            return isValid;
        }

        private void btnAppsInfo_Click(object sender, EventArgs e)
        {
            frmAppsInfo frm = new frmAppsInfo();
            frm.ShowDialog();
        }

        private void btnSearchStock_Click(object sender, EventArgs e)
        {
            // check
            if (!txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
            {
                dbFunction.SetMessageBox("Unable to add component." + "\n" + "Service created is not " + dbFunction.AddBracketStartEnd(clsGlobalVariables.STATUS_REPLACEMENT_DESC), clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return;
            }

            frmSearchField.iSearchType = frmSearchField.SearchType.iStockDetail;
            frmSearchField.iStatus = clsGlobalVariables.STATUS_AVAILABLE;
            frmSearchField.sTerminalType = "View Stock Detail";
            frmSearchField.sHeader = "COMPONENTS";
            frmSearchField.iLocationID = clsFunction.iZero;
            frmSearchField.sLocation = clsFunction.sDefaultSelect;
            frmSearchField.isCheckBoxes = false;            
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                Cursor.Current = Cursors.WaitCursor;

                switch (tabComponent.SelectedIndex)
                {
                    case 0: // Current
                        LoadSelected(lvwStockDetail);
                        break;
                    case 1: // Replace
                        LoadSelected(lvwRepStockDetail);
                        break;
                }
                
                checkAndSetDispatch();

                Cursor.Current = Cursors.Default;

            }
        }

        void LoadSelected(ListView lvw)
        {
            int i = 0;
            int iLineNo = 0;

            if (clsArray.ID.Length > 0)
            {
                while (clsArray.ID.Length > i)
                {

                    // Check if item already exist
                    if (!isItemOnList(clsArray.ID[i], clsArray.Description[i]))
                    {
                        // Add to List
                        iLineNo++;
                        ListViewItem item = new ListViewItem(iLineNo.ToString());

                        item.SubItems.Add(clsArray.ID[i].ToString());

                        string pJSONString = dbAPI.getInfoDetailJSON("Search", "Stock Detail Info", clsArray.ID[i]);
                        dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);
                        
                        if (dbFunction.isValidID(clsArray.ID[i]))
                        {
                            item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalTypeID));
                            item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalModelID));
                            item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalType));
                            item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalModel));
                            item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SerialNo));
                            item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Location));
                            item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_StockStatusDescription));

                        }

                        lvw.Items.Add(item);
                    }

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(lvw);

                dbFunction.UpdateListViewLineNo(lvw); // Update ListView LineNo

                // Focus first item
                if (lvw.Items.Count > 0)
                {
                    lvw.FocusedItem = lvw.Items[0];
                    lvw.Items[0].Selected = true;
                    lvw.Select();
                }
            }
        }

        private bool isItemOnList(string pID, string pDescrption)
        {
            bool isListed = false;
            if (lvwStockDetail.Items.Count > 0)
            {
                foreach (ListViewItem i in lvwStockDetail.Items)
                {
                    Debug.WriteLine("i=" + i + ">>" + i.SubItems[1].Text + " is equal with " + pID);
                    //Debug.WriteLine("i=" + i + ">>" + i.SubItems[3].Text + " is equal with " + pDescrption);

                    if ((i.SubItems[1].Text.Equals(pID)))
                    {
                        isListed = true;
                    }
                }
            }

            return isListed;
        }

        private void btnRemoveStock_Click(object sender, EventArgs e)
        {
            dbFunction.removeItemListView(lvwStockDetail, false);
        }

        private void lvwStockDetail_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {

                case Keys.Delete:

                    if (lvwStockDetail.Items.Count > 0)
                    {
                        dbFunction.removeItemListView(lvwStockDetail, false);
                    }
                    break;
            }
        }

        private void checkCancelJOButton()
        {
            btnCancelJO.Enabled = true;
            if (txtServiceJobTypeStatusDesc.Text.Equals(clsDefines.SERVICE_STATUS_COMPLETED))
                btnCancelJO.Enabled = false;
        }

        private void saveStockMovementDetail(ListView lvw, bool isCurrent)
        {
            string sRowSQL = "";
            string sSQL = "";
            

            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtIRIDNo.Text))
            {
                if (dbFunction.isValidCount(lvw.Items.Count))
                {
                    foreach (ListViewItem x in lvw.Items)
                    {
                        int pReplaceID = 0;
                        int pID = int.Parse(x.SubItems[1].Text);
                        string pSN = x.SubItems[6].Text;

                        if (isCurrent)
                        {
                            pReplaceID = 0;
                        }
                        else
                        {
                            pReplaceID = pID;
                            pID = 0;
                        }
                        
                        // Insert                
                        sRowSQL = "";
                        sRowSQL = " (" + dbFunction.CheckAndSetNumericValue(txtSearchServiceNo.Text) + ", " +
                        sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(txtServiceJobType.Text) + ", " +
                        sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + ", " +
                        sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(txtClientID.Text) + ", " +
                        sRowSQL + sRowSQL + " '" + pID + "', " +
                        sRowSQL + sRowSQL + " '" + pReplaceID + "') ";

                        if (sSQL.Length > 0)
                            sSQL = sSQL + ", " + sRowSQL;
                        else
                            sSQL = sSQL + sRowSQL;
                    }

                    Debug.WriteLine("sSQL=" + sSQL);

                    dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 1);

                    if (sSQL.Length > 0)
                    {
                        dbAPI.ExecuteAPI("POST", "Insert", "", "", "Stock Movement Detail", sSQL, "InsertCollectionDetail");
                    }
                }                
            }
            
        }

        private void deleteStockMovementDetail()
        {
            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtIRIDNo.Text))
            {
                dbAPI.ExecuteAPI("DELETE", "Delete", "Stock Movement Detail", txtSearchServiceNo.Text + clsDefines.gPipe + txtIRIDNo.Text, "Stock Movement Detail", "", "DeleteCollectionDetail");
            }                
        }

        private void loadStockMovementDetail(ListView lvw, bool isCurrent)
        {
            dbFunction.ClearListViewItems(lvw);
            if (dbFunction.isValidID(txtIRIDNo.Text))
            {
                dbAPI.FillListViewStockMovementDetail(lvw, "Stock Movement Detail List", txtSearchServiceNo.Text + clsDefines.gPipe + txtIRIDNo.Text, isCurrent);
            }
        }

        private string getComponentsDetail(ListView obj)
        {
            string pOutput = "";

            if (obj.Items.Count > 0)
            {
                foreach (ListViewItem i in obj.Items)
                {
                    string pType = i.SubItems[4].Text;
                    string pModel = i.SubItems[5].Text;
                    string pSerialNo = i.SubItems[6].Text;

                    pOutput += pModel + "-" + pSerialNo + Environment.NewLine;
                }
            }
            
            return pOutput;
        }

        private void checkAndSetDispatch()
        {
            Debug.WriteLine("--checkAndSetDispatch--");

            bool isDispatch = (chkDispatch.Checked ? true : false);
            
            string pStatus = clsGlobalVariables.STATUS_AVAILABLE_DESC;
            if (isDispatch)
                pStatus = clsGlobalVariables.STATUS_DISPATCH_DESC;
            else
                pStatus = clsGlobalVariables.STATUS_ALLOCATED_DESC;

            if (txtServiceJobTypeStatusDesc.Text.Equals(clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC) && dbFunction.isValidID(txtSearchFSRNo.Text))
                pStatus = clsGlobalVariables.STATUS_INSTALLED_DESC;

            if (dbFunction.isValidID(txtSuccessCnt.Text))
            {
                if (txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC) ||
                    txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_SERVICING_DESC) ||
                    txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC) ||
                    txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_PULLOUT_DESC))
                    pStatus = clsGlobalVariables.STATUS_INSTALLED_DESC;
                
            }    

            Debug.WriteLine("txtSearchSTJobTypeDescription.Text=" + txtSearchSTJobTypeDescription.Text);
            Debug.WriteLine("isDispatch=" + isDispatch);
            Debug.WriteLine("pStatus=" + pStatus);

            // --------------------------------------------------
            // For current
            // --------------------------------------------------
            if (dbFunction.isValidID(txtCurTerminalID.Text))
                txtCurTerminalStatusDesc.Text = txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC) ? clsGlobalVariables.STATUS_INSTALLED_DESC : pStatus;

            if (dbFunction.isValidID(txtCurSIMID.Text))
                txtCurSIMStatusDesc.Text = txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC) ? clsGlobalVariables.STATUS_INSTALLED_DESC : pStatus;

            // --------------------------------------------------
            // For replacement
            // --------------------------------------------------
            if (dbFunction.isValidID(txtRepTerminalID.Text))
                txtRepTerminalStatusDesc.Text = pStatus;

            if (dbFunction.isValidID(txtRepSIMID.Text))
                txtRepSIMStatusDesc.Text = pStatus;

            // --------------------------------------------------
            // For components
            // --------------------------------------------------
            switch (tabComponent.SelectedIndex)
            {
                case 0: // Current
                    dbFunction.updateListView(lvwStockDetail, 8, txtSearchSTJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC) ? clsGlobalVariables.STATUS_INSTALLED_DESC :  pStatus, false); // status
                    break;
                case 1: // Replaced                        
                    dbFunction.updateListView(lvwRepStockDetail, 8, pStatus, false); // status
                    break;
            }
        }
        
        private void chkDispatch_CheckedChanged(object sender, EventArgs e)
        {
            checkAndSetDispatch();
        }

        private void btnOpenCurTerminal_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iCurTerminalID, txtCurTerminalID.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iCurTerminalSN, txtCurTerminalSN.Text)) return;

            clsSearch.ClassTerminalID = int.Parse(txtCurTerminalID.Text);
            clsSearch.ClassTerminalSN = txtCurTerminalSN.Text;
            frmImportTerminal.fAutoLoadData = true;

            dbFunction.SetMessageBox("Opening TERMINAL window with SN" + dbFunction.AddBracketStartEnd(clsSearch.ClassTerminalSN), "Open window", clsFunction.IconType.iInformation);

            frmImportTerminal frm = new frmImportTerminal();
            dbFunction.handleForm(frm);
        }

        private void btnOpenCurSIM_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iCurSIMID, txtCurSIMID.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iCurSIMSN, txtCurSIMSN.Text)) return;

            clsSearch.ClassSIMID = int.Parse(txtCurSIMID.Text);
            clsSearch.ClassSIMSerialNo = txtCurSIMSN.Text;
            frmImportSIM.fAutoLoadData = true;

            dbFunction.SetMessageBox("Opening SIM window with SN" + dbFunction.AddBracketStartEnd(clsSearch.ClassSIMSerialNo), "Open window", clsFunction.IconType.iInformation);
            
            frmImportSIM frm = new frmImportSIM();
            dbFunction.handleForm(frm);
        }

        private void btnOpenRepTerminal_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRepTerminalID, txtRepTerminalID.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRepTerminalSN, txtRepTerminalSN.Text)) return;

            clsSearch.ClassTerminalID = int.Parse(txtRepTerminalID.Text);
            clsSearch.ClassTerminalSN = txtRepTerminalSN.Text;
            frmImportTerminal.fAutoLoadData = true;

            dbFunction.SetMessageBox("Opening TERMINAL window with SN" + dbFunction.AddBracketStartEnd(clsSearch.ClassTerminalSN), "Open window", clsFunction.IconType.iInformation);
            
            frmImportTerminal frm = new frmImportTerminal();
            dbFunction.handleForm(frm);
        }

        private void btnOpenRepSIM_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRepSIMID, txtRepSIMID.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRepSIMSN, txtRepSIMSN.Text)) return;

            clsSearch.ClassSIMID = int.Parse(txtRepSIMID.Text);
            clsSearch.ClassSIMSerialNo = txtRepSIMSN.Text;
            frmImportSIM.fAutoLoadData = true;

            dbFunction.SetMessageBox("Opening SIM window with SN" + dbFunction.AddBracketStartEnd(clsSearch.ClassSIMSerialNo), "Open window", clsFunction.IconType.iInformation);
            
            frmImportSIM frm = new frmImportSIM();
            dbFunction.handleForm(frm);
        }

        private void btnRefreshService_Click(object sender, EventArgs e)
        {
            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtIRIDNo.Text) && fEdit)
            {
                fAutoLoadData = true;
                clsSearch.ClassServiceNo = int.Parse(dbFunction.CheckAndSetNumericValue(txtSearchServiceNo.Text));
                clsSearch.ClassParticularName = txtMerchantName.Text;
                clsSearch.ClassClientID = int.Parse(dbFunction.CheckAndSetNumericValue(txtClientID.Text));
                clsSearch.ClassMerchantID = int.Parse(dbFunction.CheckAndSetNumericValue(txtMerchantID.Text));
                clsSearch.ClassFEID = int.Parse(dbFunction.CheckAndSetNumericValue(txtFEID.Text));
                clsSearch.ClassTID = txtIRTID.Text;
                clsSearch.ClassMID = txtIRMID.Text;
                clsSearch.ClassIRIDNo = int.Parse(dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text));
                clsSearch.ClassIRNo = txtSearchIRNo.Text;
                clsSearch.ClassServiceStatusDescription = txtServiceStatusDescription.Text;
                
                btnSearchService_Click(this, e);

            }
            else
            {
                dbFunction.SetMessageBox("No service selected.\n\nUnable to refresh service.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }
        }

        private void lvwStockDetail_DoubleClick(object sender, EventArgs e)
        {
            string pSelectedRow = dbFunction.GetListViewSelectedRow(lvwStockDetail, 0);
            Debug.WriteLine("pSelectedRow=\n" + pSelectedRow);

            if (lvwStockDetail.Items.Count > 0)
            {
                txtLineNo.Text = dbFunction.GetSearchValue("LINE#");
                txtItemID.Text = dbFunction.GetSearchValue("ID");
                txtItemSN.Text = dbFunction.GetSearchValue("SERIAL NO.");
                

            }
        }

        private void lvwRepStockDetail_DoubleClick(object sender, EventArgs e)
        {
            string pSelectedRow = dbFunction.GetListViewSelectedRow(lvwRepStockDetail, 0);
            Debug.WriteLine("pSelectedRow=\n" + pSelectedRow);

            if (lvwRepStockDetail.Items.Count > 0)
            {
                txtLineNo.Text = dbFunction.GetSearchValue("LINE#");
                txtItemID.Text = dbFunction.GetSearchValue("ID");
                txtItemSN.Text = dbFunction.GetSearchValue("SERIAL NO.");


            }
        }

        private void tabComponent_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {

                case Keys.Delete:

                    if (lvwRepStockDetail.Items.Count > 0)
                    {
                        dbFunction.removeItemListView(lvwRepStockDetail, false);
                    }
                    break;
            }
        }

        private void btnOpenComponent_Click(object sender, EventArgs e)
        {
            switch (tabComponent.SelectedIndex)
            {
                case 0:                  
                    break;
                case 1:
                    break;

            }

            if (dbFunction.isValidID(txtItemID.Text))
            {
                clsSearch.ClassTerminalID = int.Parse(txtItemID.Text);
                clsSearch.ClassTerminalSN = txtItemSN.Text;
                frmStockEntry.fAutoLoadData = true;

                dbFunction.SetMessageBox("Opening COMPONENT window with SN" + dbFunction.AddBracketStartEnd(clsSearch.ClassTerminalSN), "Open window", clsFunction.IconType.iInformation);

                frmStockEntry frm = new frmStockEntry();
                dbFunction.handleForm(frm);
            }
            
        }

        private void AdditionalComBoBoxUnlock(bool isLock)
        {
            if (isLock)
            {
                cboBillingType.Enabled = true;
                cboSource.Enabled = true;
                cboCategory.Enabled = true;
                cboSubCategory.Enabled = true;
            }
            else
            {
                cboBillingType.Enabled = false;
                cboSource.Enabled = false;
                cboCategory.Enabled = false;
                cboSubCategory.Enabled = false;
            }
        }

        private void ComboBoxDefaultSelect()
        {
            cboSource.Text = cboCategory.Text = cboSubCategory.Text = cboBillingType.Text = clsFunction.sDefaultSelect;
        }

        private void btnSearchDispatcher_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantID, txtMerchantID.Text)) return;
            //if (!dbFunction.isValidEntry(clsFunction.CheckType.iClientID, txtClientID.Text)) return;

            frmSearchField.iSearchType = frmSearchField.SearchType.iFE;
            frmSearchField.sHeader = "VENDOR DISPATCHER";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                // Load Information
                txtDispatchID.Text = clsSearch.ClassParticularID.ToString();
                txtDispatcher.Text = clsSearch.ClassParticularName;
                
                FillDispatcherContactInfoTextBox();

                chkDispatch.Enabled = true;
                chkDispatch.Checked = false;

                if (dbFunction.isValidID(txtSearchFSRNo.Text))
                    initDispatch(false);

            }
        }

        private void FillFEContactInfoTextBox()
        {

            txtFEMobileNo.Text =
            txtFEEmail.Text =
            clsFunction.sNull;

            if (dbFunction.isValidID(txtFEID.Text))
            {
                string pJSONString = dbAPI.getInfoDetailJSON("Search", "Merchant Contact Info", txtFEID.Text);

                dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                txtFEMobileNo.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Mobile);
                txtFEEmail.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Email);
                

            }
        }

        private void FillDispatcherContactInfoTextBox()
        {

            txtDispatcherMobileNo.Text =
            txtDispatcherEmail.Text =
            clsFunction.sNull;

            if (dbFunction.isValidID(txtDispatchID.Text))
            {
                string pJSONString = dbAPI.getInfoDetailJSON("Search", "Merchant Contact Info", txtDispatchID.Text);

                dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                txtDispatcherMobileNo.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Mobile);
                txtDispatcherEmail.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Email);


            }
        }

        private void btnRemoveDispatcher_Click(object sender, EventArgs e)
        {
            txtDispatchID.Text =
            txtDispatcher.Text =
            txtDispatcherMobileNo.Text =
            txtDispatcherEmail.Text = clsFunction.sNull;

            initDispatch(false);
        }

        private void entryTextBox(bool isEnable)
        {
            txtRemarks.ReadOnly = txtRMInstruction.ReadOnly = txtProbReported.ReadOnly = txtVendor.ReadOnly = txtRequestor.ReadOnly = (isEnable ? false : true);
            txtRemarks.Enabled = txtRMInstruction.Enabled = txtProbReported.Enabled = txtVendor.Enabled = txtRequestor.Enabled = (isEnable ? true : false);

            txtRemarks.BackColor = txtRMInstruction.BackColor = txtProbReported.BackColor = txtVendor.BackColor = txtRequestor.BackColor = (isEnable ? clsFunction.EntryBackColor : clsFunction.DisableBackColor);
            
        }

        private void btnSearchAssistNo_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iHelpDeskProblem;
            frmSearchField.sHeader = "HELPDESK TICKET";
            frmSearchField.isCheckBoxes = false;
            frmSearchField.isPreview = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    fAutoLoadData = true;
                    fEdit = false;

                    HelpDeskController data = _mHelpDeskController.getInfo(clsSearch.ProblemNo);
                    txtAssistNo.Text = clsSearch.AssistNo.ToString();
                    txtProblemNo.Text = clsSearch.ProblemNo.ToString();

                    clsSearch.ClassParticularID = clsSearch.ClassMerchantID = data.MerchantID;
                    clsSearch.ClassIRIDNo = data.IRIDNo;
                    txtMerchantID.Text = clsSearch.ClassParticularID.ToString();
                    txtIRIDNo.Text = clsSearch.ClassIRIDNo.ToString();
                    FillMerchantTextBox();

                    clsSearch.ClassIRIDNo = data.IRIDNo;
                    clsSearch.ClassClientID = data.ClientID;
                    clsSearch.ClassParticularName = txtMerchantName.Text;
                    clsSearch.ClassTID = txtIRTID.Text;
                    clsSearch.ClassMID = txtIRMID.Text;
                    clsSearch.ClassIRNo = txtSearchIRNo.Text;
                    clsSearch.ClassServiceStatusDescription = clsGlobalVariables.STATUS_INSTALLED_DESC;

                    dbAPI.GenerateID(true, txtRequestNo, txtSearchServiceNo, "Servicing Detail", clsDefines.CONTROLID_PREFIX_SERVICE);
                    lblSubHeader.Text = txtRequestNo.Text;

                    clsSearch.ClassJobType = data.JobType;
                    cboSearchServiceType.Enabled = true;
                    cboSearchServiceType.Text = clsSearch.ClassJobTypeDescription;

                    btnSearchMerchant_Click(this, e);

                    if (data != null && dbFunction.isValidID(txtProblemNo.Text))
                        FilldHelpdeskInfo(int.Parse(txtAssistNo.Text), int.Parse(txtProblemNo.Text), data);

                    txtEntryRequestID.Text = txtSearchIRNo.Text = data.RequestID;
                    txtProbReported.Text = data.ProblemReported;

                    fAutoLoadData = false;

                    Cursor.Current = Cursors.Default;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exceptional error message {ex.Message}");
                    dbFunction.SetMessageBox($"Exceptional error message {ex.Message}", "Search: HelpDesk", clsFunction.IconType.iError);
                }
                
            }
            
        }

        private void loadMerchantLastControlNo()
        {
            if (lvwList.Items.Count > 0)
            {
                // get Merchant Last Control No
                dbAPI.ExecuteAPI("GET", "Search", "Merchant Last ControlNo", txtIRIDNo.Text, "Get Info Detail", "", "GetInfoDetail");
                Debug.WriteLine("clsSearch.ClassOutParamValue=" + clsSearch.ClassOutParamValue);
                if (clsSearch.ClassOutParamValue.Length > 0)
                {
                    string pServiceNo = clsFunction.sZero;
                    jsonObj obj = JsonConvert.DeserializeObject<jsonObj>(clsSearch.ClassOutParamValue);

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        if (int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_JobType)) == clsGlobalVariables.JOB_TYPE_INSTALLATION)
                            pServiceNo = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_InstallationServiceNo);
                        else if (int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_JobType)) == clsGlobalVariables.JOB_TYPE_REPLACEMENT)
                            pServiceNo = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_ReplacementServiceNo);

                    }
                }
                else
                {
                    dbFunction.SetMessageBox("Unable to get merchant last control number." + "\n\n" +
                        clsDefines.CONTACT_ADMIN_MESSAGE, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);

                    btnSave.Enabled = false;
                    return;
                }

            }
        }

        private void loadCurrentSN()
        {
            // Check for current terminal/SIM SN
            if (dbFunction.isValidID(txtIRIDNo.Text))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Merchant Current Terminal SN", dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text), "Get Info Detail", "", "GetInfoDetail");
                if (dbAPI.isNoRecordFound() == false)
                {
                    txtCurTerminalID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtCurTerminalSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);

                }

                dbAPI.ExecuteAPI("GET", "Search", "Merchant Current SIM SN", dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text), "Get Info Detail", "", "GetInfoDetail");
                if (dbAPI.isNoRecordFound() == false)
                {
                    txtCurSIMID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtCurSIMSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);

                }
            }
        }

        private void FilldHelpdeskInfo(int pAssisNo, int pProblemNo, HelpDeskController data)
        {
            Debug.WriteLine("--FilldHelpdeskInfo--");

            if (pProblemNo > 0)
            {
                ucHelpDeskServiceInfo.loadData(data);
                ucHelpDeskEntryInfo.loadData(data);
                ucVendorHelpDeskRepInfo.loadData(data);
                ucVendorHelpDeskTeamLeadInfo.loadData(data);

                cboSource.Text = data.Source;
                cboCategory.Text = data.Category;
                cboSubCategory.Text = data.SubCategory;

                cboSource.Enabled = cboCategory.Enabled = cboSubCategory.Enabled = false;

                txtEntryRequestID.ReadOnly = true;
                txtEntryRequestID.Enabled = false;
                txtEntryRequestID.BackColor = clsFunction.DisableBackColor;

                // Disable/Read-Only
                btnSearchMerchant.Enabled = false;
                dbFunction.SetButtonIconImage(btnSearchMerchant);

                btnSearchService.Enabled = false;
                dbFunction.SetButtonIconImage(btnSearchService);
                
                btnNoRequestID.Enabled = false;
                dbFunction.SetButtonIconImage(btnNoRequestID);

                txtHelpdeskRequestID.Text = txtSearchIRNo.Text = data.RequestID;
                txtProbReported.Text = data.ProblemReported;
                txtHelpDeskActualProblemFound.Text = data.ActualProblem;
                txtHelpDeskActionMade.Text = data.ActionTaken;
                txtHelpDeskRemarks.Text = data.Remarks;

                txtHDName.Text = ucVendorHelpDeskRepInfo.VendorName;
                txtHDEmail.Text = ucVendorHelpDeskRepInfo.VendorEmail;
                txtHDContactNo.Text = ucVendorHelpDeskRepInfo.VendorMobileNo;

                txtHDTeamLead.Text = ucVendorHelpDeskTeamLeadInfo.VendorName;
                txtHDTLEmail.Text = ucVendorHelpDeskTeamLeadInfo.VendorEmail;
                txtHDTLContactNo.Text = ucVendorHelpDeskTeamLeadInfo.VendorMobileNo;
                
                txtHelpDeskActualProblemFound.ReadOnly = txtHelpDeskActionMade.ReadOnly = txtHelpDeskRemarks.ReadOnly = true;
            }            
        }

        private void btnUpdateServiceDate_Click(object sender, EventArgs e)
        {
            string pSearchValue = "";

            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isUpdate, clsUser.ClassUserID, 25)) return;

            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtMerchantID.Text) && dbFunction.isValidID(txtIRIDNo.Text))
            {
                if (!dbFunction.checkDateFromTo(DateTime.Parse(dteReqInstallationDate.Value.ToShortDateString()), DateTime.Parse(dteServiceReqDate.Value.ToShortDateString())))
                {
                    dbFunction.SetMessageBox("Request date must not greater than Schedule date", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return;
                }

                // Admin Login requirement
                if (!dbAPI.isPromptAdminLogIn()) return;

                // Update
                if (!dbFunction.fPromptConfirmation("Job Order date update information:" +
                    "\n\n" +
                    " > Job Type: " + cboSearchServiceType.Text + "\n" +
                    " > Service Status: " + txtServiceJobTypeStatusDesc.Text + "\n\n" +
                    " > Merchant: " + txtMerchantName.Text + "\n" +
                    " > TID: " + txtIRTID.Text + "\n" +
                    " > MID: " + txtIRMID.Text + "\n\n" +
                    " > Request Date: " + dteReqInstallationDate.Text + "\n" +
                    " > Schedule Date: " + dteServiceReqDate.Text +
                    "\n\n" +
                    "Are you sure to continue update?"
                    )) return;

                string pDateTime = dbFunction.getCurrentDateTime();
                string pDate = dbFunction.getCurrentDate();
                string pTime = dbFunction.getCurrentTime();
                string pRequestDate = dbFunction.CheckAndSetDatePickerValueToDate(dteReqInstallationDate);
                string pScheduleDate = dbFunction.CheckAndSetDatePickerValueToDate(dteServiceReqDate);

                pSearchValue = $"{txtSearchServiceNo.Text}{clsDefines.gPipe}{txtIRIDNo.Text}{clsDefines.gPipe}" +
                                      $"{pDate}{clsDefines.gPipe}{pTime}{clsDefines.gPipe}{pRequestDate}{clsDefines.gPipe}{pScheduleDate}{clsDefines.gPipe}" +
                                      $"{StrClean(txtProbReported.Text)}{clsDefines.gPipe}" +
                                      $"{StrClean(txtRMInstruction.Text)}{clsDefines.gPipe}" +
                                      $"{StrClean(txtRemarks.Text)}";

                dbFunction.parseDelimitedString(pSearchValue, clsDefines.gPipe, 0);

                // update
                dbAPI.ExecuteAPI("PUT", "Update", "Request-Schedule Date", pSearchValue, "", "", "UpdateCollectionDetail");

                dbFunction.SetMessageBox("Job Order date information updated.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
            }
            else
            {
                dbFunction.SetMessageBox("Service information must not be blank.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);                
              }            
        }

        private void btnUpdateServiceType_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> ServiceTypeButtonLabels = new Dictionary<string, string>
            {
                { "edit", "EDIT SERVICE TYPE" },
                { "update", "UPDATE SERVICE TYPE" }
            };

            HashSet<string> AllowedServiceTypes = new HashSet<string>
            {
                clsGlobalVariables.STATUS_SERVICING_DESC,
                clsGlobalVariables.STATUS_REPROGRAMMED_DESC
            };
            
            if (btnUpdateServiceType.Text == ServiceTypeButtonLabels["edit"])
            {
                // Enable editing
                btnDispatchJO.Enabled = false;
                cboSearchServiceType.Enabled = true;
                btnUpdateServiceType.Text = ServiceTypeButtonLabels["update"];
            }
            else if (btnUpdateServiceType.Text == ServiceTypeButtonLabels["update"])
            {
                string selectedType = cboSearchServiceType.SelectedItem?.ToString();
                string currentType = txtServiceType1.Text;

                if (string.IsNullOrEmpty(selectedType))
                {   
                    dbFunction.SetMessageBox("Please select a service type.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return;
                }

                // Check if both current and selected types are allowed
                if (!AllowedServiceTypes.Contains(currentType) || !AllowedServiceTypes.Contains(selectedType))
                {
                    dbFunction.SetMessageBox("Selected service type change is not allowed.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);
                    return;
                }

                if (selectedType == currentType)
                {   
                    dbFunction.SetMessageBox("Service type remains unchanged.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
                    return;
                }
                else
                {
                    // Admin Login requirement
                    if (!dbAPI.isPromptAdminLogIn()) return;

                    string pMessage = $"Are you sure to update the service type from [{txtServiceType1.Text}] to [{cboSearchServiceType.Text}]";
                    if (!dbFunction.fPromptConfirmation(pMessage)) return;
                }

                // call api
                string pSearchValue = $"{txtSearchServiceNo.Text}{clsDefines.gPipe}{txtEntryRequestID.Text}{clsDefines.gPipe}{txtSearchSTJobType.Text}";

                // parse delimited
                dbFunction.parseDelimitedString(pSearchValue, clsDefines.gPipe, 0);

                dbAPI.ExecuteAPI("PUT", "Update", "Servicing-FSR Job Type", pSearchValue, "", "", "UpdateCollectionDetail");

                dbFunction.SetMessageBox($"Service type has been successfully updated to {selectedType}.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);

                btnClear_Click(this, e);
            }
            
        }

        private void getSignAndImageCount()
        {
            string pJSONString = dbAPI.checkFileInfo("View", "File Count", txtSearchServiceNo.Text);
            dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

            if (dbFunction.isValidDescription(pJSONString))
            {
                txtSignCnt.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_PngCount);
                txtImageCnt.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_JpgCount);
            }

        }

        private void btnUpdateAppsInfo_Click(object sender, EventArgs e)
        {
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isUpdate, clsUser.ClassUserID, 25)) return;

            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtIRIDNo.Text))
            {
                if (!dbFunction.fPromptConfirmation("Service application information:" +
                "\n\n" +
                " > Merchant: " + txtMerchantName.Text + "\n" +
                " > TID: " + txtIRTID.Text + "\n" +
                " > MID: " + txtIRMID.Text + "\n" +
                " > Request ID: " + txtEntryRequestID.Text + "\n\n" +
                " > Version: " + txtFUAppVersion.Text + "\n" +
                " > CRC: " + txtFUAppCRC.Text +
                "\n\n" +
                "Are you sure to continue update?"
                )) return;

                dbAPI.ExecuteAPI("PUT", "Update", "Service Apps Info", $"{txtSearchServiceNo.Text}{clsFunction.sPipe}{txtIRIDNo.Text}{clsFunction.sPipe}{txtFUAppVersion.Text}{clsFunction.sPipe}{txtFUAppCRC.Text}", "", "", "UpdateCollectionDetail");

                dbFunction.SetMessageBox("Service application info updated.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
            }
        }
    }
}
