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
using System.Threading;
using MIS.Controller;
using MIS.Enums;
using System.IO;

namespace MIS
{
    public partial class frmTerminalFSR : Form
    {
        public static bool fAutoLoadData = false;
        public static bool fModify = false;
        public static bool fCompleted = false;
        public static string sHeader = "";
        public static int iTab;
        private clsAPI dbAPI;
        private clsINI dbSetting;
        private clsFile dbFile;
        private clsFunction dbFunction;
        private clsReportFunc dbReportFunc;

        // Controller
        private ServicingDetailController _mServicingDetailController;        
        private TypeController _mTypeController;
        private HelpDeskController _mHelpDeskController;

        public string CurRep;

        private bool fEdit = false;
        int iLimit = 1000;
        private static clsAPI.JobType iSearchJobType;

        private System.Timers.Timer timer;
        private int remainingSeconds;

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

        public frmTerminalFSR()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwList, true);
            dbFunction.setDoubleBuffer(lvwChanges, true);
            dbFunction.setDoubleBuffer(lvwMM, true);
            dbFunction.setDoubleBuffer(lvwProfile1, true);
            dbFunction.setDoubleBuffer(lvwProfile2, true);
            dbFunction.setDoubleBuffer(lvwRaw, true);

            // Initialize the controller object
            _mServicingDetailController = new ServicingDetailController();
            _mTypeController = new TypeController();
            _mHelpDeskController = new HelpDeskController();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (timer != null)
            {
                timer.Enabled = false;
                timer = null;
            }
            
            this.Close();
        }

        private void frmTerminalFSR_Load(object sender, EventArgs e)
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

            dbFunction.ClearListViewItems(lvwList);
            dbFunction.ClearListViewItems(lvwChanges);
            dbFunction.ClearListViewItems(lvwStockDetail);
            dbFunction.ClearListViewItems(lvwRepStockDetail);
            dbFunction.ClearListViewItems(lvwMM);
            dbFunction.ClearListViewItems(lvwProfile1);
            dbFunction.ClearListViewItems(lvwProfile2);
            dbFunction.ClearListViewItems(lvwRaw);

            InitManualDate();
            InitManualTime();
            InitMessageCountLimit();

            // Load Service Type
            dbAPI.ExecuteAPI("GET", "View", "", "", "Service Type", "", "ViewServiceType");

            fEdit = false;
            InitButton();

            // Init search button
            btnSearchMerchant.Enabled = false;
            btnTASearch.Enabled = true;

            InitRemarkCountLimit();

            //btnFSRSearch.Enabled = true;

            InitTime();
            InitStatusTitle(true);
            UpdateButton(true);
            //InitSearchReason();
            InitSearchRemoveButton(true);

            InitCount();

            btnFSRSearch.Enabled = true;
            btnCancelJO.Enabled = false;
            btnOverride.Enabled = false;
            btnUpdateServiceDate.Enabled = false;
            btnIncludeInReport.Enabled = false;

            chkBillable.Enabled = true;
            chkBillable.Checked = true;

            chkIncludeInReport.Enabled = true;
            chkIncludeInReport.Checked = false;

            btnPreviewSvcHistory.Enabled = btnPreviewFSR.Enabled = btnResetEmail.Enabled = btnSendFSRAndDiagEmail.Enabled = btnViewDiagnostic.Enabled = false;

            lblReason.Text = "RESOLVED";

            EditableServiceDateTime(true);

            clsSearch.ClassIsExportToPDF = false;

            // location current
            dbAPI.FillComboBoxLocation(cboCurTerminalLocation);
            dbAPI.FillComboBoxLocation(cboCurSIMLocation);

            // location replace
            dbAPI.FillComboBoxLocation(cboRepTerminalLocation);
            dbAPI.FillComboBoxLocation(cboRepSIMLocation);

            // components
            dbAPI.FillComboBoxLocation(cboItemLocation);
            dbAPI.FillComboBoxTerminalStatus(cboItemStatus);

            // dependency
            dbAPI.FillComboBoxDepedency(cboDependency);
            dbAPI.FillComboBoxStatusReason(cboStatusReason);

            iniComboBoxSelection(false);

            btnRefreshSN.Enabled = false;

            InitChangesListView();

            InitServiceHistoryListView();

            //Init Search Button (Search Merchant)
            btnSearchMerchant.Enabled = false;
            dbFunction.SetButtonIconImage(btnSearchMerchant);

            //Init Search Button (Search FSR)
            btnFSRSearch.Enabled = true;
            dbFunction.SetButtonIconImage(btnFSRSearch);

            btnUpdateMerchRep.Enabled = false;
            dbFunction.SetButtonIconImage(btnUpdateMerchRep);

            //setMainTab();

            if (fAutoLoadData)
            {
                btnFSRSearch_Click(this, e);
                fAutoLoadData = false;
            }

            InitCurrentComBoBox(false);
            InitReplacementComBoBox(false);

            tabFillUp.TabIndex = 0;

            Cursor.Current = Cursors.Default;
        }
        private void InitManualDate()
        {
            //dteMFSRDate.Value = DateTime.Now;
            //dbFunction.SetDateCustomFormat(dteMFSRDate);

            dteMFSRDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteMFSRDate, clsFunction.sDateDefaultFormat);
        }

        private void InitManualTime()
        {
            //dteMReceiptTime.Value = DateTime.Now;
            //dbFunction.SetTimeCustomFormat(dteMReceiptTime);

            //dteMTimeArrived.Value = DateTime.Now.Date;
            //dbFunction.SetTimeCustomFormat(dteMTimeArrived);

            //dteMTimeStart.Value = DateTime.Now.Date;
            //dbFunction.SetTimeCustomFormat(dteMTimeStart);

            //dteMTimeEnd.Value = DateTime.Now.Date;
            //dbFunction.SetTimeCustomFormat(dteMTimeEnd);

            dteMReceiptTime.Value = DateTime.Now;
            dbFunction.SetTimeFormat(dteMReceiptTime, clsFunction.sTimeDefaultFormat);

            dteMTimeArrived.Value = DateTime.Now;
            dbFunction.SetTimeFormat(dteMTimeArrived, clsFunction.sTimeDefaultFormat);

            dteMTimeStart.Value = DateTime.Now;
            dbFunction.SetTimeFormat(dteMTimeStart, clsFunction.sTimeDefaultFormat);

            dteMTimeEnd.Value = DateTime.Now;
            dbFunction.SetTimeFormat(dteMTimeEnd, clsFunction.sTimeDefaultFormat);
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

        private void InitRemarkCountLimit()
        {
            lblCountRemarks.Text = txtMAnyComments.TextLength.ToString() + "/" + iLimit.ToString();
            txtMAnyComments.MaxLength = iLimit;
        }

        /*
        private void LoadTA()
        {
            frmServiceExpenses.ResetHoldExpenses();

            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);


            LoadData();

            LoadLastRequestedBy();

            MKTextBoxBackColor(true);

            txtServiceJobTypeStatusDesc.Text = clsGlobalVariables.JOB_TYPE_STATUS_PREPARATION_DESC;
            UpdateButton(false);

            // Init ComboBox            
            InitCurrentComBoBox(true);
            InitReplacementComBoBox(true);

            SetComboBoxDefaultByService();

        }
        */

        private void FillMerchantTextBox()
        {
            string profile_info = "";
            string rawdata_info = "";
            string profile_config_info = "";

            txtMerchantName.Text =
            txtSearchMerchantName.Text =
            txtMerchantAddress.Text =
            txtMerchantCity.Text =
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
            txtMAppVesion.Text =
            txtMAppCRC.Text =
            txtIRInstallationDate.Text =
            txtRequestor.Text =
            clsFunction.sNull;

            if (dbFunction.isValidID(txtMerchantID.Text))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Merchant Info", txtMerchantID.Text + clsFunction.sPipe + txtIRIDNo.Text, "Get Info Detail", "", "GetInfoDetail");

                // parse delimited
                dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                if (dbAPI.isNoRecordFound() == false)
                {
                    txtMerchantID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                    txtMerchantName.Text = txtSearchMerchantName.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtMerchantAddress.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                    txtMerchantCity.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
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
                    txtAppVersion.Text = txtMAppVesion.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 14);
                    txtAppCRC.Text = txtMAppCRC.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 15);
                    txtIRIDNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 16);
                    txtSearchIRNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 17);
                    txtIRRequestDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 18);
                    txtIRInstallationDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 19);                    
                    txtRMInstruction.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 21);
                    txtIRStatusDescription.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 32);

                    profile_info = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 29);                    
                    rawdata_info = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 30);
                    profile_config_info = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 34);

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

                    // Assigned SN
                    dbAPI.ExecuteAPI("GET", "Search", "Merchant Current Terminal SN", txtIRIDNo.Text, "Get Info Detail", "", "GetInfoDetail");
                    txtAssignedTerminalID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtAssignedTerminalSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);

                    dbAPI.ExecuteAPI("GET", "Search", "Merchant Current SIM SN", txtIRIDNo.Text, "Get Info Detail", "", "GetInfoDetail");
                    txtAssignedSIMID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtAssignedSIMSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);

                }
            }

            if (!dbFunction.isValidDescriptionEntry(rawdata_info, "MSP No." + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

        }
        private void FillClientTextBox()
        {
            /*
            int i = 0;

            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassParticularID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassProvinceID + clsFunction.sPipe +
                                                clsSearch.ClassCityID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassParticularTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassParticularTypeDescription + clsFunction.sPipe +
                                                clsSearch.ClassParticularName.ToString();

            Debug.WriteLine("PopulateClientTextBox::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbAPI.ExecuteAPI("GET", "View", "Advance Particular", clsSearch.ClassAdvanceSearchValue, "Particular", "", "ViewAdvanceParticular");

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.ParticularID.Length > i)
                {
                    txtClientID.Text = clsArray.ParticularID[i].ToString();
                    txtClientName.Text = clsArray.ParticularName[i];
                    txtClientAddress.Text = clsArray.Address[i];
                    txtMMClientName.Text = clsArray.ParticularName[i];

                    i++;

                }
            }
            */

            txtClientName.Text =
            txtMMClientName.Text =
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
                    txtClientName.Text = clsSearch.ClassClientName = txtMMClientName.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtClientAddress.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                    txtClientContactPerson.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                    txtClientTelNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                    txtClientMobileNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                    txtClientEmail.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                    txtClientCode.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 11);

                }
            }
        }
        private void FillSPTextBox()
        {

            int i = 0;

            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassParticularID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassProvinceID + clsFunction.sPipe +
                                                clsSearch.ClassCityID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassParticularTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassParticularTypeDescription + clsFunction.sPipe +
                                                clsSearch.ClassParticularName.ToString();

            Debug.WriteLine("FillSPTextBox::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbAPI.ExecuteAPI("GET", "View", "Advance Particular", clsSearch.ClassAdvanceSearchValue, "Particular", "", "ViewAdvanceParticular");

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.ParticularID.Length > i)
                {
                    txtServiceProviderID.Text = clsArray.ParticularID[i].ToString();
                    txtSPName.Text = clsArray.ParticularName[i];
                    txtSPAddress.Text = clsArray.Address[i];

                    i++;

                }
            }
        }
        private void FillFETextBox()
        {
            /*
            int i = 0;

            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassParticularID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassProvinceID + clsFunction.sPipe +
                                                clsSearch.ClassCityID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassParticularTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassParticularTypeDescription + clsFunction.sPipe +
                                                clsSearch.ClassParticularName.ToString();

            Debug.WriteLine("FillFETextBox::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbAPI.ExecuteAPI("GET", "View", "Advance Particular", clsSearch.ClassAdvanceSearchValue, "Particular", "", "ViewAdvanceParticular");

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.ParticularID.Length > i)
                {
                    txtFEID.Text = clsArray.ParticularID[i].ToString();
                    txtFEName.Text = clsArray.ParticularName[i];
                    txtFEAddress.Text = clsArray.Address[i];

                    i++;

                }
            }
            */

            txtFEName.Text =
            txtFEAddress.Text =
            txtFEContactPerson.Text =
            txtFETelNo.Text =
            txtFEMobileNo.Text =
            txtFEEmail.Text = clsFunction.sNull;

            if (dbFunction.isValidID(txtMerchantID.Text))
            {
                dbAPI.ExecuteAPI("GET", "Search", "FE Info", txtFEID.Text, "Get Info Detail", "", "GetInfoDetail");

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

        private void PopulateCurrentTerminalTextBox(string sTerminalID, string sTerminalSN)
        {
            if ((sTerminalID.CompareTo(clsFunction.sZero) == 0) && (sTerminalSN.CompareTo(clsFunction.sZero) == 0 || sTerminalSN.CompareTo(clsFunction.sDash) == 0))
            {
                txtCurTerminalID.Text = clsFunction.sZero;
                txtCurTerminalTypeID.Text = clsFunction.sNull;
                txtCurTerminalModelID.Text = clsFunction.sNull;
                txtCurTerminalBrandID.Text = clsFunction.sNull;
                txtCurTerminalSN.Text = clsFunction.sNull;
                txtCurTerminalType.Text = clsFunction.sNull;
                txtCurTerminalModel.Text = clsFunction.sNull;
                txtCurTerminalBrand.Text = clsFunction.sNull;

                txtCurTerminalSN.BackColor = clsFunction.DisableBackColor;

                cboSearchCurTerminalStatus.Text = "";
                cboSearchCurTerminalStatus.Enabled = false;

                return;
            }

            dbAPI.GetTerminalInfo(sTerminalID, sTerminalSN);

            txtCurTerminalID.Text = clsTerminal.ClassTerminalID.ToString();
            txtCurTerminalTypeID.Text = clsTerminal.ClassTerminalTypeID.ToString();
            txtCurTerminalModelID.Text = clsTerminal.ClassTerminalModelID.ToString();
            txtCurTerminalBrandID.Text = clsTerminal.ClassTerminalBrandID.ToString();
            txtCurTerminalSN.Text = clsTerminal.ClassTerminalSN;
            txtCurTerminalType.Text = clsTerminal.ClassTerminalType;
            txtCurTerminalModel.Text = clsTerminal.ClassTerminalModel;
            txtCurTerminalBrand.Text = clsTerminal.ClassTerminalBrand;

            txtCurTerminalSN.BackColor = clsFunction.MKBackColor;
            txtSearchCurTerminalStatus.BackColor = clsFunction.BackColorAzure;

            if (dbFunction.isValidID(txtCurTerminalSNStatus.Text))
            {
                // Current SN Status
                string sHoldStatus = txtCurTerminalSNStatus.Text;
                cboSearchCurTerminalStatus.Text = dbAPI.GetStatusDescription(clsTerminal.ClassTerminalStatus);
                txtCurTerminalSNStatus.Text = clsTerminal.ClassTerminalStatus.ToString();

                //Service SN Status
                txtSearchCurTerminalStatus.Text = dbAPI.GetStatusDescription(int.Parse(sHoldStatus));
            }

        }

        private void PopulateCurrentDockTextBox(string sDockID, string sDockSN)
        {
            if ((sDockID.CompareTo(clsFunction.sZero) == 0) && (sDockSN.CompareTo(clsFunction.sZero) == 0 || sDockSN.CompareTo(clsFunction.sDash) == 0))
            {
                txtCurDockID.Text = clsFunction.sZero;
                txtCurDockSN.Text = clsFunction.sNull;

                txtCurDockSN.BackColor = clsFunction.DisableBackColor;

                cboSearchCurDockStatus.Text = "";
                cboSearchCurDockStatus.Enabled = false;

                return;
            }

            dbAPI.GetTerminalInfo(sDockID, sDockSN);

            txtCurDockID.Text = sDockID;
            txtCurDockSN.Text = sDockSN;
            txtMMDockSN.Text = clsServicingDetail.ClassDockSN;

            txtCurDockSN.BackColor = clsFunction.MKBackColor;

            if (dbFunction.isValidID(txtCurDockSNStatus.Text))
            {
                // Current SN Status
                string sHoldStatus = txtCurDockSNStatus.Text;
                cboSearchCurDockStatus.Text = dbAPI.GetStatusDescription(clsTerminal.ClassTerminalStatus);
                txtCurDockSNStatus.Text = clsTerminal.ClassTerminalStatus.ToString();

                //Service SN Status
                //txtSearchCurDockStatus.Text = dbAPI.GetStatusDescription(int.Parse(sHoldStatus));

            }
        }

        private void PopulateCurrentSIMTextBox(string sSIMID, string sSIMSN)
        {
            if ((sSIMID.CompareTo(clsFunction.sZero) == 0) && (sSIMSN.CompareTo(clsFunction.sZero) == 0 || sSIMSN.CompareTo(clsFunction.sDash) == 0))
            {
                txtCurSIMID.Text = clsFunction.sZero;
                txtCurSIMSN.Text = clsFunction.sNull;
                txtCurSIMCarrier.Text = clsFunction.sNull;

                txtCurSIMSN.BackColor = clsFunction.DisableBackColor;

                cboSearchCurSIMStatus.Text = "";
                cboSearchCurSIMStatus.Enabled = cboCurSIMLocation.Enabled = false;

                return;
            }

            dbAPI.GetSIMInfo(sSIMID, sSIMSN);

            txtCurSIMID.Text = sSIMID;
            txtCurSIMSN.Text = sSIMSN;
            txtCurSIMCarrier.Text = clsSearch.ClassSIMCarrier;

            txtCurSIMSN.BackColor = clsFunction.MKBackColor;
            txtSearchCurSIMStatus.BackColor = clsFunction.BackColorAzure;

            if (dbFunction.isValidID(txtCurSIMSNStatus.Text))
            {
                // Current SN Status
                string sHoldStatus = txtCurSIMSNStatus.Text;
                cboSearchCurSIMStatus.Text = dbAPI.GetStatusDescription(clsSIM.ClassSIMStatus);
                txtCurSIMSNStatus.Text = clsSIM.ClassSIMStatus.ToString();

                //Service SN Status
                txtSearchCurSIMStatus.Text = dbAPI.GetStatusDescription(int.Parse(sHoldStatus));
            }


        }


        private void PopulateReplaceTerminalTextBox(string sTerminalID, string sTerminalSN)
        {
            if ((sTerminalID.CompareTo(clsFunction.sZero) == 0) && (sTerminalSN.CompareTo(clsFunction.sZero) == 0 || sTerminalSN.CompareTo(clsFunction.sDash) == 0))
            {
                txtRepTerminalID.Text = clsFunction.sZero;
                txtRepTerminalTypeID.Text = clsFunction.sNull;
                txtRepTerminalModelID.Text = clsFunction.sNull;
                txtRepTerminalBrandID.Text = clsFunction.sNull;
                txtRepTerminalSN.Text = clsFunction.sNull;
                txtRepTerminalType.Text = clsFunction.sNull;
                txtRepTerminalModel.Text = clsFunction.sNull;
                txtRepTerminalBrand.Text = clsFunction.sNull;

                txtRepTerminalSN.BackColor = clsFunction.DisableBackColor;

                cboSearchRepTerminalStatus.Text = "";
                cboSearchRepTerminalStatus.Enabled = false;

                return;
            }

            dbAPI.GetTerminalInfo(sTerminalID, sTerminalSN);

            txtRepTerminalID.Text = clsTerminal.ClassTerminalID.ToString();
            txtRepTerminalTypeID.Text = clsTerminal.ClassTerminalTypeID.ToString();
            txtRepTerminalModelID.Text = clsTerminal.ClassTerminalModelID.ToString();
            txtRepTerminalBrandID.Text = clsTerminal.ClassTerminalBrandID.ToString();
            txtRepTerminalSN.Text = clsTerminal.ClassTerminalSN;
            txtRepTerminalType.Text = clsTerminal.ClassTerminalType;
            txtRepTerminalModel.Text = clsTerminal.ClassTerminalModel;
            txtRepTerminalBrand.Text = clsTerminal.ClassTerminalBrand;

            txtRepTerminalSN.BackColor = clsFunction.MKBackColor;
            txtSearchRepTerminalStatus.BackColor = clsFunction.BackColorAzure;

            if (dbFunction.isValidID(txtRepTerminalSNStatus.Text))
            {
                cboSearchRepTerminalStatus.Text = dbAPI.GetStatusDescription(int.Parse(txtRepTerminalSNStatus.Text));
                txtSearchRepTerminalStatus.Text = dbAPI.GetStatusDescription(int.Parse(txtRepTerminalSNStatus.Text));
            }

        }

        private void PopulateReplaceDockTextBox(string sDockID, string sDockSN)
        {
            if ((sDockID.CompareTo(clsFunction.sZero) == 0) && (sDockSN.CompareTo(clsFunction.sZero) == 0 || sDockSN.CompareTo(clsFunction.sDash) == 0))
            {
                txtRepDockID.Text = clsFunction.sZero;
                txtRepDockSN.Text = clsFunction.sNull;

                txtRepDockSN.BackColor = clsFunction.DisableBackColor;

                cboSearchRepDockStatus.Text = "";
                cboSearchRepDockStatus.Enabled = false;

                return;
            }

            txtRepDockID.Text = clsServicingDetail.ClassDockID.ToString();
            txtRepDockSN.Text = clsServicingDetail.ClassDockSN;

            txtRepDockSN.BackColor = clsFunction.MKBackColor;

            if (dbFunction.isValidID(txtRepDockSNStatus.Text))
            {
                cboSearchRepDockStatus.Text = dbAPI.GetStatusDescription(int.Parse(txtRepDockSNStatus.Text));
            }


        }

        private void PopulateReplaceSIMTextBox(string sSIMID, string sSIMSN)
        {
            if ((sSIMID.CompareTo(clsFunction.sZero) == 0) && (sSIMSN.CompareTo(clsFunction.sZero) == 0 || sSIMSN.CompareTo(clsFunction.sDash) == 0))
            {
                txtRepSIMID.Text = clsFunction.sZero;
                txtRepSIMSN.Text = clsFunction.sNull;
                txtRepSIMCarrier.Text = clsFunction.sNull;

                txtRepSIMSN.BackColor = clsFunction.DisableBackColor;

                cboSearchRepSIMStatus.Text = "";
                cboSearchRepSIMStatus.Enabled = false;

                return;
            }

            txtRepSIMID.Text = clsServicingDetail.ClassSIMID.ToString();
            txtRepSIMSN.Text = clsServicingDetail.ClassSIMSN;
            txtRepSIMCarrier.Text = clsSearch.ClassSIMCarrier;

            txtRepSIMSN.BackColor = clsFunction.MKBackColor;
            txtSearchRepSIMStatus.BackColor = clsFunction.BackColorAzure;

            if (dbFunction.isValidID(txtRepSIMSNStatus.Text))
            {
                cboSearchRepSIMStatus.Text = dbAPI.GetStatusDescription(int.Parse(txtRepSIMSNStatus.Text));
                txtSearchRepSIMStatus.Text = dbAPI.GetStatusDescription(int.Parse(txtRepSIMSNStatus.Text));
            }


        }
        
        private void PKTextBoxBackColor(bool isLock)
        {
            if (isLock)
            {
                txtMerchantName.BackColor = clsFunction.PKBackColor;
                //txtCurTerminalSN.BackColor = clsFunction.PKBackColor;
                //txtCurDockSN.BackColor = clsFunction.PKBackColor;                  
                txtIRTID.BackColor = clsFunction.PKBackColor;
                txtIRMID.BackColor = clsFunction.PKBackColor;
                //txtCurSIMSN.BackColor = clsFunction.PKBackColor;
                txtClientName.BackColor = clsFunction.PKBackColor;
                txtFEName.BackColor = clsFunction.PKBackColor;
                txtSPName.BackColor = clsFunction.PKBackColor;

                // Status Description
                txtSearchTAIDNo.BackColor = clsFunction.MKBackColor;
                txtSearchIRNo.BackColor = clsFunction.MKBackColor;

                txtTADateTime.BackColor = clsFunction.DateBackColor;
                txtTAModifiedDateTime.BackColor = clsFunction.DateBackColor;

                txtIRRequestDate.BackColor = clsFunction.DateBackColor;
                txtIRInstallationDate.BackColor = clsFunction.DateBackColor;

                txtProcessedDate.BackColor = clsFunction.DateBackColor;
                txtModifiedDate.BackColor = clsFunction.DateBackColor;

                txtTAStatus.BackColor = clsFunction.MKBackColor;                
                txtServiceScheduleDate.BackColor = clsFunction.DateBackColor;
                txtServiceRequestNo.BackColor = clsFunction.MKBackColor;
                txtSearchMerchantName.BackColor = clsFunction.MKBackColor;
                txtJobTypeDescription.BackColor = clsFunction.MKBackColor;
                txtFSRStatus.BackColor = clsFunction.MKBackColor;

                txtAppVersion.BackColor = clsFunction.PKBackColor;
                txtAppCRC.BackColor = clsFunction.PKBackColor;

            }
        }

        private void PKTextBoxReadOnly(bool fReadOnly)
        {
            txtMMerchRep.ReadOnly = fReadOnly;
            txtMMerchPosition.ReadOnly = fReadOnly;
            txtMMerchContactNo.ReadOnly = fReadOnly;
            txtMMerchEmail.ReadOnly = fReadOnly;

            txtMMPowerSN.ReadOnly = fReadOnly;
            txtMMDockSN.ReadOnly = fReadOnly;

            txtMProblemReported.ReadOnly = fReadOnly;
            txtMActualProblemReported.ReadOnly = fReadOnly;
            txtMActionTaken.ReadOnly = fReadOnly;
            txtMAnyComments.ReadOnly = fReadOnly;
            txtMMerchRemarks.ReadOnly = fReadOnly;
            //txtRMInstruction.ReadOnly = fReadOnly;

            txtMAmount.Text = clsFunction.sDefaultAmount;
            txtMAmount.ReadOnly = fReadOnly;

            txtMAppVesion.ReadOnly = fReadOnly;
            txtMAppCRC.ReadOnly = fReadOnly;
            txtBeyondReason.ReadOnly = fReadOnly;
            
            if (!fReadOnly)
            {  
                txtMMerchRep.BackColor = clsFunction.EntryBackColor;
                txtMMerchPosition.BackColor = clsFunction.EntryBackColor;
                txtMMerchContactNo.BackColor = clsFunction.EntryBackColor;
                txtMMerchEmail.BackColor = clsFunction.EntryBackColor;

                txtMMPowerSN.BackColor = clsFunction.EntryBackColor;
                txtMMDockSN.BackColor = clsFunction.EntryBackColor;

                txtMProblemReported.BackColor = clsFunction.EntryBackColor;
                txtMActualProblemReported.BackColor = clsFunction.EntryBackColor;
                txtMActionTaken.BackColor = clsFunction.EntryBackColor;
                txtMAnyComments.BackColor = clsFunction.EntryBackColor;
                txtMMerchRemarks.BackColor = clsFunction.EntryBackColor;
                //txtRMInstruction.BackColor = clsFunction.EntryBackColor;             
                txtMAppVesion.BackColor = clsFunction.EntryBackColor;
                txtMAppCRC.BackColor = clsFunction.EntryBackColor;
                txtBeyondReason.BackColor = clsFunction.EntryBackColor;
            }

        }
        private void DKTextBoxBackColor()
        {
            txtProcessedDate.BackColor = clsFunction.DateBackColor;
            txtModifiedDate.BackColor = clsFunction.DateBackColor;
            txtIRRequestDate.BackColor = clsFunction.DateBackColor;
            txtIRInstallationDate.BackColor = clsFunction.DateBackColor;
            txtServiceScheduleDate.BackColor = clsFunction.DateBackColor;
            txtTADateTime.BackColor = clsFunction.DateBackColor;
            txtTAModifiedDateTime.BackColor = clsFunction.DateBackColor;
        }

        private void btnMClear_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            fAutoLoadData = false;
            fEdit = false;
            fCompleted = false;
            dbFunction.ClearTextBox(this);
            dbFunction.ClearRichTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            //dbFunction.ComBoBoxUnLock(false, this);
            dbFunction.DatePickerUnlock(false, this);

            InitButton();
            InitManualDate();
            InitManualTime();
            InitTime();
            InitStatusTitle(true);

            // Init search button           
            btnTASearch.Enabled = true;
            //btnFSRSearch.Enabled = true;

            PKTextBoxBackColor(false);
            btnAttempt.Enabled = false;
            //btnSearchMerchant.Enabled = false;
            InitRemarkCountLimit();
            //lblSubHeader.Text = "";
            InitMessageCountLimit();
            UpdateButton(true);
            InitCount();
            chkCloseTicket.CheckState = CheckState.Checked;

            //InitSearchReason();

            btnSearchMerchant.Enabled = false;
            btnFSRSearch.Enabled = true;
            //InitSearchRemoveButton(true);

            lblSubHeader.Text = clsFunction.sDash;
            lblHeader.Text = "FSR";
            lblReason.Text = "RESOLVED";
            
            chkBillable.Enabled = true;
            chkBillable.Checked = true;

            chkIncludeInReport.Enabled = true;
            chkIncludeInReport.Checked = false;

            btnPreviewSvcHistory.Enabled = btnPreviewFSR.Enabled = btnResetEmail.Enabled = btnSendFSRAndDiagEmail.Enabled = btnViewDiagnostic.Enabled = false;

            EditableServiceDateTime(true);

            // location, current
            //dbAPI.FillComboBoxLocation(cboCurTerminalLocation);
            //dbAPI.FillComboBoxLocation(cboCurSIMLocation);

            // location, replace
            //dbAPI.FillComboBoxLocation(cboRepTerminalLocation);
            //dbAPI.FillComboBoxLocation(cboRepSIMLocation);

            btnCancelJO.Enabled = false;
            btnRefreshSN.Enabled = false;
            btnOverride.Enabled = false;
            btnRefreshService.Enabled = false;
            btnUpdateServiceDate.Enabled = false;
            btnIncludeInReport.Enabled = false;

            dbFunction.ClearListViewItems(lvwList);
            dbFunction.ClearListViewItems(lvwChanges);
            dbFunction.ClearListViewItems(lvwStockDetail);
            dbFunction.ClearListViewItems(lvwRepStockDetail);
            dbFunction.ClearListViewItems(lvwMM);
            dbFunction.ClearListViewItems(lvwProfile1);
            dbFunction.ClearListViewItems(lvwProfile2);
            dbFunction.ClearListViewItems(lvwRaw);

            //InitChangesListView();
            //InitServiceHistoryListView();

            lblCreatedDate.Text = clsFunction.sNull;

            //Init Search Button (Search Merchant)
            btnSearchMerchant.Enabled = false;
            dbFunction.SetButtonIconImage(btnSearchMerchant);

            //Init Search Button (Search FSR)
            btnFSRSearch.Enabled = true;
            dbFunction.SetButtonIconImage(btnFSRSearch);

            btnUpdateMerchRep.Enabled = false;
            dbFunction.SetButtonIconImage(btnUpdateMerchRep);

            btnUpdateMerchRep.Enabled = false;
            dbFunction.SetButtonIconImage(btnUpdateMerchRep);

            txtTicketStatus.Text = clsFunction.sNull;

            //setMainTab();

            InitCurrentComBoBox(false);
            InitReplacementComBoBox(false);

            tabFillUp.TabIndex = 0;

            iniComboBoxSelection(false);

            Cursor.Current = Cursors.Default;

        }

        private void btnAttempt_Click(object sender, EventArgs e)
        {
            clsSearch.ClassAdvanceSearchValue = txtSearchTAIDNo.Text + clsFunction.sPipe +
                                                txtClientID.Text + clsFunction.sPipe +
                                                txtIRTID.Text + clsFunction.sPipe +
                                                txtIRMID.Text + clsFunction.sPipe +
                                                txtSearchIRNo.Text;

            frmSearchField.iSearchType = frmSearchField.SearchType.iFSRAttempt;
            frmSearchField.sHeader = "ATTEMPT(S) FOR REQUEST ID: " + txtSearchIRNo.Text;
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                btnSave.Enabled = false;
            }
        }

        private void btnMAdd_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isAdd, clsUser.ClassUserID, 26)) return;

            dbAPI.GenerateID(true, txtFSRRequestNo, txtSearchFSRNo, "FSR Detail", clsDefines.CONTROLID_PREFIX_FSR);
            lblSubHeader.Text = txtFSRRequestNo.Text;

            fAutoLoadData = false;
            fEdit = false;
            fCompleted = false;
            InitButton();
            InitManualDate();

            btnAdd.Enabled = false;
            btnSave.Enabled = true;

            //dbFunction.TextBoxUnLock(true, this);
            //dbFunction.ComBoBoxUnLock(true, this);

            // Init search button           
            btnTASearch.Enabled = false;
            //btnFSRSearch.Enabled = false;

            //btnTASearch_Click(this, e);

            SetMKTextBoxBackColor();
            SetPKTextBoxBackColor();
            SetStatusTextBoxBackColor();

            InitCreatedDateTime();

            txtSearchFSRNo.Text = clsFunction.sNull;

            InitStatusTitle(false);

            //Init Search Button (Search Merchant)
            btnSearchMerchant.Enabled = true;
            dbFunction.SetButtonIconImage(btnSearchMerchant);

            //Init Search Button (Search FSR)
            btnFSRSearch.Enabled = false;
            dbFunction.SetButtonIconImage(btnFSRSearch);
            
        }

        private void btnMSave_Click(object sender, EventArgs e)
        {
            string pSearchValue = "";

            try
            {
                // Check Application Version
                if (!dbAPI.isValidSystemVersion()) return;

                // Check User Access Rights
                if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isUpdate, clsUser.ClassUserID, 26)) return;

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

                // check service already completed
                if (txtServiceJobTypeStatusDesc.Text.Equals(clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC))
                {
                    dbFunction.SetMessageBox("Service already completed. Update not allowed." + "\n\n" + clsDefines.CONTACT_ADMIN_MESSAGE, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return;
                }

                iSearchJobType = dbAPI.GetJobType(txtJobTypeDescription.Text);
                Debug.WriteLine("iSearchJobType=" + iSearchJobType);

                int isBillable = (chkBillable.Checked ? 1 : 0);

                Debug.WriteLine("isBillable=" + isBillable);

                // Check Required Fields
                if (!ValidateFields()) return;

                if (!dbFunction.CheckTimeFromTo(dteMTimeStart, dteMTimeEnd, "Time Start/End", true)) return;

                if (!dbFunction.isValidDigitalFSRMode(txtSearchFSRDesc.Text, false))
                {
                    if (!dbFunction.CheckTimeFromTo(dteMTimeArrived, dteMReceiptTime, "Time Arrived/Receipt", false)) return;
                }

                clsSearch.ClassServiceTypeDesc = txtServiceJobTypeDescription.Text;
                clsSearch.ClassRequestID = txtRequestID.Text;
                clsSearch.ClassRequestDate = txtServiceRequestDate.Text;
                clsSearch.ClassScheduleDate = txtServiceScheduleDate.Text;
                clsSearch.ClassServicedDate = dbFunction.CheckAndSetDatePickerValueToDate(dteMFSRDate);
                clsSearch.ClassMerchantName = txtMerchantName.Text;
                clsSearch.ClassTID = txtIRTID.Text;
                clsSearch.ClassMID = txtIRMID.Text;
                clsSearch.ClassFEName = txtFEName.Text;
                clsSearch.ClassDispatcherName = txtDispatchBy.Text;
                clsSearch.ClassRequestor = txtRequestor.Text;
                clsSearch.ClassIsDispatch = true;
                clsSearch.ClassTerminalSN = txtCurTerminalSN.Text;
                clsSearch.ClassSIMSerialNo = txtCurSIMSN.Text;
                clsSearch.ClassRepTerminalSN = txtRepTerminalSN.Text;
                clsSearch.ClassRepSIMSN = txtRepSIMSN.Text;
                clsSearch.ClassComponents = getComponentsDetail(lvwStockDetail);
                clsSearch.ClassRepComponents = getComponentsDetail(lvwRepStockDetail);
                clsSearch.ClassBillable = chkBillable.Checked ? true : false;
                clsSearch.ClassDependency = cboDependency.Text;
                clsSearch.ClassStatusReason = cboStatusReason.Text;

                // service confirmation window
                frmServiceConfirmation frmServiceConfirmation = new frmServiceConfirmation();
                frmServiceConfirmation.gHeader = "FSR" + " " + clsDefines.gPipe + " " + lblMainStatus.Text + " " + clsDefines.gPipe + " " + cboSearchActionMade.Text;
                frmServiceConfirmation.ShowDialog();
                if (!frmServiceConfirmation.fSelected) return;

                if (!fEdit)
                {
                    clsFSR.ClassRemarks = clsFunction.sDash;
                    clsFSR.ClassFileName = clsFunction.sDash;

                    dbFunction.GetProcessedByAndDateTime(); // Get processedby and datetime

                    dbFunction.GetModifiedByAndDateTime(); // Get modifiedby and datetime

                    Cursor.Current = Cursors.WaitCursor;

                    SaveFSRMaster();

                    SaveManualFSRDetail();

                    // ---------------------------------------------------------------------------------------------
                    // Save Multiple Data
                    // ---------------------------------------------------------------------------------------------       
                    clsSearch.ClassAdvanceSearchValue =
                                                    txtJobTypeDescription.Text + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtSearchFSRNo.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtSearchServiceNo.Text) + clsFunction.sPipe +
                                                    txtServiceRequestNo.Text + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtClientID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtClientName.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtMerchantID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtMerchantName.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtFEID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtFEName.Text) + clsFunction.sPipe +

                                                    dbFunction.CheckAndSetNumericValue(txtCurTerminalID.Text) + clsFunction.sPipe +
                                                    clsFunction.sZero + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtCurTerminalSNStatus.Text) + clsFunction.sPipe +

                                                    dbFunction.CheckAndSetNumericValue(txtCurSIMID.Text) + clsFunction.sPipe +
                                                    clsFunction.sZero + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtCurSIMSNStatus.Text) + clsFunction.sPipe +

                                                    dbFunction.CheckAndSetNumericValue(txtRepTerminalID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtRepTerminalSNStatus.Text) + clsFunction.sPipe +

                                                    dbFunction.CheckAndSetNumericValue(txtRepSIMID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtRepSIMSNStatus.Text) + clsFunction.sPipe +

                                                    dbFunction.CheckAndSetStringValue(txtServiceCode.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(cboSearchActionMade.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtMAppVesion.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtMAppCRC.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(isBillable.ToString()) + clsFunction.sPipe +

                                                    dbFunction.CheckAndSetStringValue(txtMMerchRep.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtMMerchPosition.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtMMerchContactNo.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtMerchantAddress.Text) + clsFunction.sPipe +

                                                    dbFunction.CheckAndSetNumericValue(txtAttemptCnt.Text) + clsFunction.sPipe +
                                                    (chkServiced.Checked ? 1 : 0) + clsFunction.sPipe +

                                                    dbFunction.CheckAndSetStringValue(cboCurTerminalLocation.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(cboCurSIMLocation.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(cboRepTerminalLocation.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(cboRepSIMLocation.Text) + clsFunction.sPipe +

                                                    dbFunction.CheckAndSetStringValue(txtMMerchEmail.Text);

                    Debug.WriteLine("Multiple Update->Value=" + clsSearch.ClassAdvanceSearchValue);

                    dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                    dbAPI.ExecuteAPI("PUT", "Update", "Multiple Save FSR", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                    Cursor.Current = Cursors.Default;

                }
                else
                {

                    if (dbFunction.isValidID(txtSearchFSRNo.Text) && dbFunction.isValidID(txtServiceNo.Text))
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        DateTime stFSRDate = dteMFSRDate.Value;
                        string sFSRDate = stFSRDate.ToString("yyyy-MM-dd");
                        string sMReceiptTime = dbFunction.GetDateFromParse(dteMReceiptTime.Text, "hh:mm tt", "HH:mm:ss");
                        string sMTimeArrived = dbFunction.GetDateFromParse(dteMTimeArrived.Text, "hh:mm tt", "HH:mm:ss");
                        string sMTimeStart = dbFunction.GetDateFromParse(dteMTimeStart.Text, "hh:mm tt", "HH:mm:ss");
                        string sMTimeEnd = dbFunction.GetDateFromParse(dteMTimeEnd.Text, "hh:mm tt", "HH:mm:ss");

                        dbFunction.GetModifiedByAndDateTime(); // Get modifiedby and datetime

                        clsSearch.ClassAdvanceSearchValue = txtJobTypeDescription.Text + clsFunction.sPipe +
                                                            dbFunction.CheckAndSetNumericValue(txtSearchServiceNo.Text) + clsFunction.sPipe +
                                                            dbFunction.CheckAndSetNumericValue(txtSearchFSRNo.Text) + clsFunction.sPipe +
                                                            dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + clsFunction.sPipe +
                                                            sFSRDate + clsFunction.sPipe +
                                                            sMReceiptTime + clsFunction.sPipe +
                                                            sMTimeArrived + clsFunction.sPipe +
                                                            sMTimeStart + clsFunction.sPipe +
                                                            sMTimeEnd + clsFunction.sPipe +
                                                            dbFunction.CheckAndSetNumericValue(txtFEID.Text) + clsFunction.sPipe +
                                                            txtFEName.Text + clsFunction.sPipe +
                                                            txtMMerchRep.Text + clsFunction.sPipe +
                                                            txtMMerchPosition.Text + clsFunction.sPipe +
                                                            txtMMerchContactNo.Text + clsFunction.sPipe +
                                                            txtMMPowerSN.Text + clsFunction.sPipe +
                                                            txtMMDockSN.Text + clsFunction.sPipe +
                                                            StrClean(txtMProblemReported.Text) + clsFunction.sPipe +
                                                            StrClean(txtMActionTaken.Text) + clsFunction.sPipe +
                                                            StrClean(txtMMerchRemarks.Text) + clsFunction.sPipe +
                                                            StrClean(txtMActualProblemReported.Text) + clsFunction.sPipe +
                                                            StrClean(txtMAnyComments.Text) + clsFunction.sPipe +
                                                            txtReasonID.Text + clsFunction.sPipe +
                                                            txtReasonDesc.Text + clsFunction.sPipe +
                                                            isBillable.ToString() + clsFunction.sPipe +
                                                            txtMAppVesion.Text + clsFunction.sPipe +
                                                            txtMAppCRC.Text + clsFunction.sPipe +
                                                            clsUser.ClassModifiedBy + clsFunction.sPipe +
                                                            clsUser.ClassModifiedDateTime + clsFunction.sPipe +

                                                            // ROCKY - FSR ENHANCEMENT: ADD ADDITIONAL LOCATION UPDATE
                                                            dbFunction.CheckAndSetStringValue(cboCurTerminalLocation.Text) + clsFunction.sPipe +
                                                            dbFunction.CheckAndSetStringValue(cboCurSIMLocation.Text) + clsFunction.sPipe +

                                                            // ROCKY - FSR ENHANCEMENT: GET TERMINAL & SIM SN FOR UPDATE
                                                            dbFunction.CheckAndSetNumericValue(txtCurTerminalSN.Text) + clsFunction.sPipe +
                                                            dbFunction.CheckAndSetNumericValue(txtCurSIMSN.Text) + clsFunction.sPipe +

                                                            dbFunction.CheckAndSetStringValue(txtVendor.Text) + clsFunction.sPipe +
                                                            dbFunction.CheckAndSetStringValue(txtRequestor.Text);

                        Debug.WriteLine("Multiple Update->Value=" + clsSearch.ClassAdvanceSearchValue);

                        dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                        dbAPI.ExecuteAPI("PUT", "Update", "Multiple Update FSR", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                        Cursor.Current = Cursors.Default;

                    }
                    else
                    {
                        dbFunction.SetMessageBox("FSR No. and Service No. must not be blank.", "Manual FSR", clsFunction.IconType.iError);
                        return;
                    }

                }

                if (chkEmail.Checked)
                {
                    EmailNotification((fEdit ? "UPDATED: " : ""));
                }

                // save service changes information
                if (dbFunction.isValidID(txtSearchServiceNo.Text))
                {
                    if (cboSearchActionMade.Text.CompareTo(dbAPI.GetActionMade()[1]) == 0) // SUCCESS
                    {
                        // Update service changes detail
                        if (lvwChanges.Items.Count > 0)
                        {
                            if (dbAPI.isRecordExist("Search", "Dynamic Search", clsDefines.Table_ServiceChangesDetail + clsFunction.sPipe + clsDefines.FieldName_ServiceNo + clsFunction.sPipe + txtSearchServiceNo.Text))
                            {
                                dbAPI.ExecuteAPI("PUT", "Update", "Service Changes Detail", txtSearchServiceNo.Text, "", "", "UpdateBulkCollectionDetail");
                            }

                        }

                        if (lvwStockDetail.Items.Count > 0)
                        {
                            dbAPI.BulkUpdateStockMovementDetail(lvwStockDetail, int.Parse(dbFunction.CheckAndSetNumericValue(txtClientID.Text)), false);
                        }
                    }

                    // update dispatcher
                    if (dbFunction.isValidID(txtDispatchID.Text))
                    {
                        clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtSearchServiceNo.Text) + clsFunction.sPipe +
                                                            dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + clsFunction.sPipe +
                                                            dbFunction.CheckAndSetNumericValue(txtDispatchID.Text) + clsFunction.sPipe +
                                                            dbFunction.CheckAndSetStringValue(txtDispatcher.Text);

                        dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                        dbAPI.ExecuteAPI("PUT", "Update", "DispatchID", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
                    }


                    // update dependency
                    clsSearch.ClassAdvanceSearchValue = txtIRIDNo.Text + clsDefines.gPipe +
                                                        txtSearchServiceNo.Text + clsDefines.gPipe +
                                                        dbFunction.getFileID(cboDependency, "All Type") + clsDefines.gPipe +
                                                        dbFunction.getFileID(cboStatusReason, "All Type");

                    dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                    dbAPI.ExecuteAPI("PUT", "Update", "Dependency", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                    // update beyond reason
                    clsSearch.ClassAdvanceSearchValue = $"{txtJobType.Text}{clsDefines.gPipe}{txtSearchFSRNo.Text}{clsDefines.gPipe}{txtServiceNo.Text}{clsDefines.gPipe}{txtIRIDNo.Text}{clsDefines.gPipe}{dbFunction.CheckAndSetStringValue(StrClean(txtBeyondReason.Text))}";

                    dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                    dbAPI.ExecuteAPI("PUT", "Update", "Beyond Reason", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                    // update isReport
                    clsSearch.ClassAdvanceSearchValue = $"{txtClientID.Text}{clsDefines.gPipe}{txtSearchServiceNo.Text}{clsDefines.gPipe}{txtIRIDNo.Text}{clsDefines.gPipe}" + dbFunction.CheckAndSetBooleanValue(chkIncludeInReport.Checked);

                    dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                    dbAPI.ExecuteAPI("PUT", "Update", "Servicing-FSR Include In Report", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                }

                // Save Terminal Activity
                if (clsGlobalVariables.isAPIResponseOK)
                {
                    if (dbFunction.isValidID(txtCurTerminalID.Text) || dbFunction.isValidID(txtRepTerminalID.Text))
                        SaveTerminalActivity();
                }


                // Save SIM Activity
                if (clsGlobalVariables.isAPIResponseOK)
                {
                    if (dbFunction.isValidID(txtCurSIMID.Text) || dbFunction.isValidID(txtRepSIMID.Text))
                        SaveSIMActivity();
                }

                // Save Ticket -> Update In Report
                if (chkIncludeInReport.Checked)
                {
                    int pBillable = dbFunction.CheckAndSetBooleanValue(chkBillable.Checked);
                    int pTicketStatus = int.Parse(clsFunction.sOne);

                    pSearchValue = dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + clsDefines.gPipe +
                                   dbFunction.CheckAndSetNumericValue(txtSearchServiceNo.Text) + clsDefines.gPipe +
                                   pTicketStatus + clsDefines.gPipe +
                                   clsSearch.ClassCurrentParticularID + clsDefines.gPipe +
                                   dbFunction.getCurrentDateTime() + clsDefines.gPipe +
                                   pBillable;

                    dbFunction.parseDelimitedString(pSearchValue, clsDefines.gPipe, 1);

                    dbAPI.ExecuteAPI("PUT", "Update", "Ticket Status", pSearchValue, "", "", "UpdateCollectionDetail");
                }

                dbFunction.SetMessageBox(txtServiceJobTypeDescription.Text + " service has been " + (fEdit ? "updated" : "saved") + " for" +
                   "\n\n" +
                    //"Primary Request ID >" + txtRequestID1.Text + "\n" +
                    "Request ID >" + txtRequestID.Text + "\n" +
                    "Reference No. >" + txtServiceReferenceNo.Text + "\n" +
                    "Merchant Name >" + txtMerchantName.Text + "\n" +
                    "TID >" + txtIRTID.Text + "\n" +
                    "MID >" + txtIRMID.Text + "\n" +
                    "Region >" + txtMerchantRegion.Text + "\n" +
                    "City >" + txtMerchantCity.Text +
                    "\n\n" +
                   (chkEmail.Checked ? "Service request emailed to vendor representative." : "")
                   , (fEdit ? "FSR updated" : "FSR saved") + (chkBillable.Checked ? " & billable" : ""), clsFunction.IconType.iInformation);

                btnMClear_Click(this, e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceptional error message {ex.Message}");
                dbFunction.SetMessageBox($"Exceptional error message {ex.Message}", "Save: Job Order", clsFunction.IconType.iError);
            }           
        }
        private bool ValidateFields()
        {
            string sMTimeArrived = dbFunction.GetDateFromParse(dteMTimeArrived.Text, "h:mm:ss tt", "HH:mm:ss");
            string sMReceiptTime = dbFunction.GetDateFromParse(dteMReceiptTime.Text, "h:mm:ss tt", "HH:mm:ss");
            string sMTimeStart = dbFunction.GetDateFromParse(dteMTimeStart.Text, "h:mm:ss tt", "HH:mm:ss");
            string sMTimeEnd = dbFunction.GetDateFromParse(dteMTimeEnd.Text, "h:mm:ss tt", "HH:mm:ss");

            if (!fEdit)
            {
                // -------------------------------------------------------------------------------------------
                // Current 
                // -------------------------------------------------------------------------------------------
                
                if (dbFunction.isValidID(txtCurTerminalID.Text))
                {
                    // status
                    if (dbFunction.isValidComboBoxValue(cboSearchCurTerminalStatus.Text))
                    {
                        if (!dbFunction.isValidEntry(clsFunction.CheckType.iCurTerminalStatus, cboSearchCurTerminalStatus.Text)) return false; 
                    }
                    else
                    {
                        dbFunction.SetMessageBox("Current terminal status must not be blank.", "Warning", clsFunction.IconType.iExclamation);
                        return false;
                    }

                    // location
                    if (dbFunction.isValidComboBoxValue(cboCurTerminalLocation.Text))
                    {
                        if (!dbFunction.isValidEntry(clsFunction.CheckType.iCurTerminalLocation, cboCurTerminalLocation.Text)) return false;
                    }
                    else
                    {
                        dbFunction.SetMessageBox("Current terminal location must not be blank.", "Warning", clsFunction.IconType.iExclamation);
                        return false;
                    }
                }

                if (dbFunction.isValidID(txtCurSIMID.Text))
                {
                    // status
                    if (dbFunction.isValidComboBoxValue(cboSearchCurSIMStatus.Text))
                    {
                        if (!dbFunction.isValidEntry(clsFunction.CheckType.iCurSIMStatus, cboSearchCurSIMStatus.Text)) return false;
                    }
                    else
                    {
                        dbFunction.SetMessageBox("Current SIM status must not be blank.", "Warning", clsFunction.IconType.iExclamation);
                        return false;
                    }

                    // location
                    if (dbFunction.isValidComboBoxValue(cboCurSIMLocation.Text))
                    {
                        if (!dbFunction.isValidEntry(clsFunction.CheckType.iCurSIMLocation, cboCurSIMLocation.Text)) return false;
                    }
                    else
                    {
                        dbFunction.SetMessageBox("Current SIM location must not be blank.", "Warning", clsFunction.IconType.iExclamation);
                        return false;
                    }

                }

                if (dbFunction.isValidID(txtCurDockID.Text))
                {
                    if (dbFunction.isValidComboBoxValue(cboSearchCurDockStatus.Text))
                    {
                        if (!dbFunction.isValidEntry(clsFunction.CheckType.iCurDockStatus, cboSearchCurDockStatus.Text)) return false;
                    }
                    else
                    {
                        dbFunction.SetMessageBox("Current dock status must not be blank.", "Warning", clsFunction.IconType.iExclamation);
                        return false;
                    }
                }


                // -------------------------------------------------------------------------------------------
                // Replace
                // -------------------------------------------------------------------------------------------
                if (dbFunction.isValidID(txtRepTerminalID.Text))
                {
                    // status
                    if (dbFunction.isValidComboBoxValue(cboSearchRepTerminalStatus.Text))
                    {
                        if (!dbFunction.isValidEntry(clsFunction.CheckType.iRepTerminalStatus, cboSearchRepTerminalStatus.Text)) return false;
                    }
                    else
                    {
                        dbFunction.SetMessageBox("Replace terminal status must not be blank.", "Warning", clsFunction.IconType.iExclamation);
                        return false;
                    }

                    // location
                    if (dbFunction.isValidComboBoxValue(cboRepTerminalLocation.Text))
                    {
                        if (!dbFunction.isValidEntry(clsFunction.CheckType.iRepTerminalLocation, cboRepTerminalLocation.Text)) return false;
                    }
                    else
                    {
                        dbFunction.SetMessageBox("Replace terminal location must not be blank.", "Warning", clsFunction.IconType.iExclamation);
                        return false;
                    }
                }

                if (dbFunction.isValidID(txtRepSIMID.Text))
                {
                    // status
                    if (dbFunction.isValidComboBoxValue(cboSearchRepSIMStatus.Text))
                    {
                        if (!dbFunction.isValidEntry(clsFunction.CheckType.iRepSIMStatus, cboSearchRepSIMStatus.Text)) return false;
                    }
                    else
                    {
                        dbFunction.SetMessageBox("Replace SIM status must not be blank.", "Warning", clsFunction.IconType.iExclamation);
                        return false;
                    }

                    // location                  
                    if (dbFunction.isValidComboBoxValue(cboRepSIMLocation.Text))
                    {
                        if (!dbFunction.isValidEntry(clsFunction.CheckType.iRepSIMLocation, cboRepSIMLocation.Text)) return false;
                    }
                    else
                    {
                        dbFunction.SetMessageBox("Replace SIM location must not be blank.", "Warning", clsFunction.IconType.iExclamation);
                        return false;
                    }
                }

                if (dbFunction.isValidID(txtRepDockID.Text))
                {
                    if (dbFunction.isValidComboBoxValue(cboSearchRepDockStatus.Text))
                    {
                        if (!dbFunction.isValidEntry(clsFunction.CheckType.iRepDockStatus, cboSearchRepDockStatus.Text)) return false;
                    }
                    else
                    {
                        dbFunction.SetMessageBox("Current dock status must not be blank.", "Warning", clsFunction.IconType.iExclamation);
                        return false;
                    }
                }

                // Check current and replace terminal status
                if (dbFunction.isValidID(txtCurTerminalSNStatus.Text) && dbFunction.isValidID(txtRepTerminalSNStatus.Text))
                {
                    if (cboSearchCurTerminalStatus.Text.Equals(cboSearchRepTerminalStatus.Text))
                    {
                        dbFunction.SetMessageBox("Current and replace terminal status must not same.", "Warning", clsFunction.IconType.iExclamation);
                        return false;
                    }
                }

                // Check current and replace sim status
                if (dbFunction.isValidID(txtCurSIMSNStatus.Text) && dbFunction.isValidID(txtRepSIMSNStatus.Text))
                {
                    if (cboSearchCurSIMStatus.Text.Equals(cboSearchRepSIMStatus.Text))
                    {
                        dbFunction.SetMessageBox("Current and replace SIM status must not same.", "Warning", clsFunction.IconType.iExclamation);
                        return false;
                    }
                }

                // Check current and replace terminal location
                if (dbFunction.isValidID(txtRepTerminalID.Text))
                {
                    if (cboCurTerminalLocation.Text.Equals(cboRepTerminalLocation.Text))
                    {
                        dbFunction.SetMessageBox("Current and replace terminal location must not same.", "Warning", clsFunction.IconType.iExclamation);
                        return false;
                    }
                }
                
                // Check current and replace sim location
                if (dbFunction.isValidID(txtRepSIMID.Text))
                {
                    if (cboCurSIMLocation.Text.Equals(cboRepSIMLocation.Text))
                    {
                        dbFunction.SetMessageBox("Current and replace SIM location must not same.", "Warning", clsFunction.IconType.iExclamation);
                        return false;
                    }
                }

                // check same current / replace terminal
                if (dbFunction.isValidID(txtCurTerminalID.Text) && dbFunction.isValidID(txtRepTerminalID.Text))
                {
                    if (txtCurTerminalID.Text.Equals(txtRepTerminalID.Text))
                    {
                        dbFunction.SetMessageBox("Current and replace terminal must not the same.", "Warning", clsFunction.IconType.iExclamation);
                    }
                }

                // check same current / replace sim
                if (dbFunction.isValidID(txtCurSIMID.Text) && dbFunction.isValidID(txtRepSIMID.Text))
                {
                    if (txtCurSIMID.Text.Equals(txtRepSIMID.Text))
                    {
                        dbFunction.SetMessageBox("Current and replace SIM must not the same.", "Warning", clsFunction.IconType.iExclamation);
                    }
                }

            }

            // check terminal
            if (!fEdit)
            {
                if (!txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
                {
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iCurTerminalID, txtCurTerminalID.Text)) return false;
                }
                else
                {
                    //if (!dbFunction.isValidEntry(clsFunction.CheckType.iRepTerminalID, txtRepTerminalID.Text)) return false;

                    // ROCKY - FSR ISSUE: FIX FOR REPLACEMENT BLANK TERMINAL & SIM SN 
                    if (!dbFunction.isValidID(txtRepTerminalID.Text) && (!dbFunction.isValidID(txtRepSIMID.Text)))
                    {
                        dbFunction.SetMessageBox("Replace Terminal or SIM should have a value.", "Warning", clsFunction.IconType.iExclamation);
                        return false;
                    }
                }

            }

            // Service Result
            if (!dbFunction.isValidComboBoxValue(cboSearchActionMade.Text))
            {
                dbFunction.SetMessageBox("Please choose a value for service result.", "Warning", clsFunction.IconType.iExclamation);
                return false;
            }

            // Service Result (Negative)
            if (cboSearchActionMade.Text.CompareTo(dbAPI.GetActionMade()[2]) == 0) // NEGATIVE
            {
                if (!dbFunction.isValidID(txtReasonID.Text))
                {
                    dbFunction.SetMessageBox("You have selected action made " + cboSearchActionMade.Text +
                                              "\n\n" +
                                              "Reason must not be blank.", "Select reason", clsFunction.IconType.iExclamation);
                    return false;
                }
            }

            // Check Field  
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalID, txtCurTerminalID.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iCurTerminalSN, txtCurTerminalSN.Text)) return false;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantName, txtMerchantName.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantAddress, txtMerchantAddress.Text)) return false;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iClientID, txtClientID.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantID, txtMerchantID.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iIRNo, txtSearchIRNo.Text)) return false;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTID, txtIRTID.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMID, txtIRMID.Text)) return false;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTIDLength, txtIRTID.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMIDLength, txtIRMID.Text)) return false;

            if (!dbFunction.checkDateFromTo(DateTime.Parse(dteMFSRDate.Value.ToShortDateString()), DateTime.Parse(dbFunction.getCurrentDate())))
            {
                dbFunction.SetMessageBox("[Service Date] must not greater than current date.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return false;
            }

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTime, sMTimeArrived)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTime, sMReceiptTime)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTime, sMTimeStart)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTime, sMTimeEnd)) return false;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iServiceNo, txtServiceNo.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRequestNo, txtServiceRequestNo.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iReason, txtReasonID.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iActionMade, cboSearchActionMade.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iActionTaken, txtMActionTaken.Text)) return false;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iAppVersion, txtMAppVesion.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iAppCRC, txtMAppCRC.Text)) return false;

            // Region check
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRegion, txtMerchantRegion.Text)) return false;

            // Province check
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iProvince, txtMerchantCity.Text)) return false;

            // POS_Type
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iPOSType, txtPOSType.Text)) return false;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iFEID, dbFunction.CheckAndSetNumericValue(txtFEID.Text))) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iDispatchID, dbFunction.CheckAndSetNumericValue(txtDispatchID.Text))) return false;

            // Vendor
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iVendor, txtVendor.Text)) return false;

            // Requestor
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRequestor, txtRequestor.Text)) return false;

            if (!dbAPI.isRecordExist("Search", "Region", txtMerchantRegion.Text))
            {
                dbFunction.SetMessageBox("Region " + dbFunction.AddBracketStartEnd(txtMerchantRegion.Text) + " does not exist." + "\n\n" +
                    "Update merchant information to continue.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return false;
            }
            // Region check

            if (dbFunction.isValidComboBoxValue(cboSearchActionMade.Text))
            {
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantRepresentative, txtMMerchRep.Text)) return false;
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantRepPosition, txtMMerchPosition.Text)) return false;
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantContactNo, txtMMerchContactNo.Text)) return false;
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantEmail, txtMMerchEmail.Text)) return false;
            }

            // check service is still active
            if (!dbAPI.isRecordExist("Search", "Merchant Delete Service Count", txtSearchServiceNo.Text + clsDefines.gPipe + txtIRIDNo.Text))
            {
                dbFunction.SetMessageBox("Service is no longer active or cancelled." + "\n\n" + clsDefines.CONTACT_ADMIN_MESSAGE, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return false;
            }

            // check for RequestID's mismatch
            if (!dbFunction.isValidRequestID(txtRequestID.Text, txtFSRRequestID.Text)) return false;

            // check dependency
            //if (!dbFunction.isValidDescriptionEntry(dbFunction.getFileID(cboDependency, "All Type").ToString(), "Dependency" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;

            // check status reason
            //if (!dbFunction.isValidDescriptionEntry(dbFunction.getFileID(cboStatusReason, "All Type").ToString(), "Status Reason" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;

            return true;
        }

        private bool ValidateExpenses()
        {
            bool fValid = false;

            if (dbFunction.isValidAmount(txtTExpenses.Text))
            {
                fValid = true;
            }

            if (!fValid)
            {
            }

            return fValid;
        }


        private void SaveFSRMaster()
        {
            Debug.WriteLine("--SaveFSRMaster--");

            string sRowSQL = "";
            string sSQL = "";

            DateTime ImportDateTime = DateTime.Now;
            string sImportDateTime = "";

            DateTime DateTimeModified = DateTime.Now;
            string sDateTimeModified = "";

            string sFileName = clsFSR.ClassFileName;
            string sRemarks = clsFSR.ClassRemarks;

            string sProcessedBy = clsUser.ClassUserFullName;
            string sModifiedBy = clsUser.ClassUserFullName;

            sImportDateTime = ImportDateTime.ToString("yyyy-MM-dd H:mm:ss");
            sDateTimeModified = DateTimeModified.ToString("yyyy-MM-dd H:mm:ss");

            sRowSQL = "";
            sRowSQL = " ('" + sImportDateTime + "', " +
            sRowSQL + sRowSQL + " '" + sFileName + "', " +
            sRowSQL + sRowSQL + " '" + sFileName + "', " +
            sRowSQL + sRowSQL + " '" + sRemarks + "', " +
            sRowSQL + sRowSQL + " '" + sDateTimeModified + "', " +
            sRowSQL + sRowSQL + " '" + sProcessedBy + "', " +
            sRowSQL + sRowSQL + " '" + sModifiedBy + "') ";
            sSQL = sSQL + sRowSQL;

            Debug.WriteLine("SaveFSRMaster::" + "\n" + "sSQL=" + sSQL);

            dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 0);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "FSR Import Master", sSQL, "InsertCollectionMaster");
            
        }

        private void SaveManualFSRDetail()
        {
            Debug.WriteLine("--SaveManualFSRDetail--");

            string sSQL = "";
            int LineNo = 0;
            string sSerialNo = dbFunction.CheckAndSetNumericValue(txtCurSIMSN.Text) + clsFunction.sPipe +
                               dbFunction.CheckAndSetNumericValue(txtMMPowerSN.Text) + clsFunction.sPipe +
                               dbFunction.CheckAndSetNumericValue(txtMMDockSN.Text) + clsFunction.sPipe;

            DateTime stFSRDate = dteMFSRDate.Value;
            string sMTimeArrived = dbFunction.GetDateFromParse(dteMTimeArrived.Text, "hh:mm tt", "HH:mm:ss");
            string sMTimeStart = dbFunction.GetDateFromParse(dteMTimeStart.Text, "hh:mm tt", "HH:mm:ss");
            string sMTimeEnd = dbFunction.GetDateFromParse(dteMTimeEnd.Text, "hh:mm tt", "HH:mm:ss");
            string sReasonID = (txtReasonID.Text.Length > 0 ? txtReasonID.Text : "0");
            string sProcessType = clsGlobalVariables.PROCESS_TYPE_MANUAL_DESC;

            int pTerminalID = 0;
            string pTerminalSN = clsFunction.sNull;
            int pSIMID = 0;
            string pSIMSN = clsFunction.sNull;

            pTerminalID = int.Parse(dbFunction.CheckAndSetNumericValue(txtCurTerminalID.Text));
            pTerminalSN = txtCurTerminalSN.Text;
            pSIMID = int.Parse(dbFunction.CheckAndSetNumericValue(txtCurSIMID.Text));
            pSIMSN = txtCurSIMSN.Text;

            if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
            {
                if (cboSearchActionMade.Text.CompareTo(dbAPI.GetActionMade()[1]) == 0) // SUCCESS
                {
                    pTerminalID = int.Parse(dbFunction.CheckAndSetNumericValue(txtRepTerminalID.Text));
                    pTerminalSN = txtRepTerminalSN.Text;

                    pSIMID = int.Parse(dbFunction.CheckAndSetNumericValue(txtRepSIMID.Text));
                    pSIMSN = txtRepSIMSN.Text;
                }
            }

            dbAPI.GetFSRStatus(cboSearchActionMade.Text); // Get FSR Status

            // Create Group Details - ROCKY BANTOLO
            var FsrDetails = new
            {
                LastInsertID = clsLastID.ClassLastInsertedID.ToString(),
                LineNo       = LineNo.ToString(),
                MerchantID   = dbFunction.CheckAndSetNumericValue(txtMerchantID.Text),
                MerchantName = (txtSearchMerchantName.Text.Length > 0 ? txtSearchMerchantName.Text : clsFunction.sDash),
                IRMID        = dbFunction.CheckAndSetStringValue(txtIRMID.Text),
                IRTID        = dbFunction.CheckAndSetStringValue(txtIRTID.Text),
                MobileID = clsFunction.sZero,
                MobileTerminalID = clsFunction.sDash,
                MobileVersion = clsFunction.sZero,
                MTimeArrived = dbFunction.GetDateFromParse(dteMTimeArrived.Text, "hh:mm tt", "HH:mm:ss"),
                MTimeStart   = sMTimeStart,
                MTimeEndStart = sMTimeStart,
                MTimeEnd     = sMTimeEnd,
                FSR  = txtServiceCode.Text,
                FSRDate      = stFSRDate.ToString("yyyy-MM-dd"),
                MReceiptTime = dbFunction.GetDateFromParse(dteMReceiptTime.Text, "hh:mm tt", "HH:mm:ss"),
                MAmount      = dbFunction.CheckAndSetNumericValue(txtMAmount.Text),
                MTimeEndAgain = sMTimeEnd,
                TerminalID   = pTerminalID,
                TerminalSN   = pTerminalSN,
                SIMID        = pSIMID,
                SIMSN        = pSIMSN,
                MMerchContactNo = (txtMMerchContactNo.Text.Length > 0 ? txtMMerchContactNo.Text : clsFunction.sDash),
                MMerchRep       = (txtMMerchRep.Text.Length > 0 ? txtMMerchRep.Text : clsFunction.sDash),
                MMerchPosition  = (txtMMerchPosition.Text.Length > 0 ? txtMMerchPosition.Text : clsFunction.sDash),
                FEID            = dbFunction.CheckAndSetNumericValue(txtFEID.Text),
                FEName          = (txtFEName.Text.Length > 0 ? txtFEName.Text : clsFunction.sDash),
                MMerchRemarks   = dbFunction.CheckAndSetStringValue(txtMMerchRemarks.Text),
                SerialNo        = sSerialNo,
                FSRValidStatus  = clsGlobalVariables.FSR_VALID_STATUS,
                FSRValidStatusDesc = clsGlobalVariables.FSR_VALID_STATUS_DESC,
                ServiceStatus = clsSearch.ClassStatus,
                ServiceStatusDescription = clsSearch.ClassStatusDescription,
                ReasonID        = dbFunction.CheckAndSetNumericValue(txtReasonID.Text),
                Reason          = dbFunction.CheckAndSetStringValue(txtReason.Text),
                ClientID        = dbFunction.CheckAndSetNumericValue(txtClientID.Text),
                ClientName      = txtClientName.Text,
                ProcessType     = clsGlobalVariables.PROCESS_TYPE_MANUAL_DESC,
                SearchTAIDNo    = dbFunction.CheckAndSetNumericValue(txtSearchTAIDNo.Text),
                IRIDNo          = dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text),                
                IRNo      = dbFunction.CheckAndSetNumericValue(txtRequestID.Text),
                ServiceNo = dbFunction.CheckAndSetNumericValue(txtSearchServiceNo.Text),
                ServiceCode = txtServiceCode.Text,
                FSRRequestNo    = txtFSRRequestNo.Text,
                SearchActionMade = cboSearchActionMade.Text,
                JobType          = dbFunction.CheckAndSetNumericValue(txtJobType.Text),
                JobTypeDescription = txtJobTypeDescription.Text,
                MProblemReported = dbFunction.CheckAndSetStringValue(txtMProblemReported.Text),
                MActualProblemReported = dbFunction.CheckAndSetStringValue(txtMActualProblemReported.Text),
                MActionTaken     = dbFunction.CheckAndSetStringValue(txtMActionTaken.Text),
                MAnyComments     = dbFunction.CheckAndSetStringValue(txtMAnyComments.Text),
                ProcessedBy      = clsUser.ClassProcessedBy,
                ModifiedBy       = clsUser.ClassModifiedBy,
                ProcessedDateTime = clsUser.ClassProcessedDateTime,
                ModifiedDateTime  = clsUser.ClassModifiedDateTime,
                geoLatitude = clsFunction.sNull,
                geoLongitude = clsFunction.sNull,
                geoCountry = clsFunction.sNull,
                geoLocality = clsFunction.sNull,
                geoAddress = clsFunction.sNull
            };


            // Convert to Insert Format - ROCKY BANTOLO
            // Fix for Special Characters
            sSQL = IFormat.Insert(FsrDetails);
            
            Debug.WriteLine("SaveManualFSRDetail" + sSQL);

            dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 0);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "FSR Manual Detail", sSQL, "InsertCollectionDetail");

            txtSearchFSRNo.Text = clsLastID.ClassLastInsertedID.ToString(); // Get Last Save FSRNo

        }
       
        private void frmTerminalFSR_KeyDown(object sender, KeyEventArgs e)
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
                    if (btnFSRSearch.Enabled)
                        btnFSRSearch_Click(this, e);
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
                    btnMClear_Click(this, e);
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
        
        private void InitCurrentComBoBox(bool fEnable)
        {
            cboSearchCurTerminalStatus.Enabled = cboCurTerminalLocation.Enabled = false;
            cboSearchCurSIMStatus.Enabled = cboCurSIMLocation.Enabled = false;
            cboSearchCurDockStatus.Enabled = false;

            if (dbFunction.isValidID(txtCurTerminalID.Text))
                cboSearchCurTerminalStatus.Enabled = cboCurTerminalLocation.Enabled = fEnable;

            if (dbFunction.isValidID(txtCurSIMID.Text))
                cboSearchCurSIMStatus.Enabled = cboCurSIMLocation.Enabled = fEnable;

            if (dbFunction.isValidID(txtCurDockID.Text))
                cboSearchCurDockStatus.Enabled = fEnable;

        }

        private void InitReplacementComBoBox(bool fEnable)
        {
            cboSearchRepTerminalStatus.Enabled = cboRepTerminalLocation.Enabled = false;
            cboSearchRepSIMStatus.Enabled = cboRepSIMLocation.Enabled = false;
            cboSearchRepDockStatus.Enabled = false;

            if (dbFunction.isValidID(txtRepTerminalID.Text))
                cboSearchRepTerminalStatus.Enabled = cboRepTerminalLocation.Enabled = fEnable;

            if (dbFunction.isValidID(txtRepSIMID.Text))
                cboSearchRepSIMStatus.Enabled = cboRepSIMLocation.Enabled = fEnable;

            if (dbFunction.isValidID(txtRepDockID.Text))
                cboSearchRepDockStatus.Enabled = fEnable;
        }
        
        private void txtTerminalTypeID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtMMPowerSN_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtMMerchRep_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (txtMMerchRep.Text.Length > 0)
                        txtMProblemReported.Focus();
                    break;
            }
        }

        private void txtMRemarks_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (txtMAnyComments.Text.Length > 0)
                        cboSearchActionMade.Focus();
                    break;
            }
        }

        private void cboSearchActionMade_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cboSearchActionMade.Text.Equals(clsFunction.sDefaultSelect))
            {
                if (cboSearchActionMade.Text.CompareTo(dbAPI.GetActionMade()[1]) == 0) // SUCCESS
                {
                    chkCloseTicket.CheckState = CheckState.Checked;
                    chkCloseTicket.Enabled = false;
                    lblReason.Text = "RESOLVED";

                }

                if (cboSearchActionMade.Text.CompareTo(dbAPI.GetActionMade()[2]) == 0) // NEGATIVE
                {
                    chkCloseTicket.CheckState = CheckState.Checked;
                    chkCloseTicket.Enabled = false;
                    lblReason.Text = "NEGATIVE";                   
                }

                cboSearchCurTerminalStatus.Items.Clear();
                cboSearchCurSIMStatus.Items.Clear();
                cboSearchCurDockStatus.Items.Clear();
                
                cboSearchCurTerminalStatus.Text = cboSearchCurSIMStatus.Text = cboSearchCurDockStatus.Text = clsFunction.sDefaultSelect;
                cboItemLocation.Text = cboItemStatus.Text = clsFunction.sDefaultSelect;

                cboSearchRepTerminalStatus.Items.Clear();
                cboSearchRepSIMStatus.Items.Clear();
                cboSearchRepDockStatus.Items.Clear();
                
                cboSearchRepTerminalStatus.Text = cboSearchRepSIMStatus.Text = cboSearchRepDockStatus.Text = clsFunction.sDefaultSelect;

                cboSearchCurTerminalStatus.Enabled = cboCurTerminalLocation.Enabled = cboSearchCurSIMStatus.Enabled = cboCurSIMLocation.Enabled = cboSearchCurDockStatus.Enabled = cboSearchRepTerminalStatus.Enabled = cboSearchRepSIMStatus.Enabled = cboSearchRepDockStatus.Enabled = cboRepTerminalLocation.Enabled = cboRepSIMLocation.Enabled = false;
                cboItemLocation.Enabled = cboItemStatus.Enabled = true;

                bool isReplacement = false;
                if (txtJobTypeDescription.Text.CompareTo(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC) == 0)
                    isReplacement = true;

                // Current
                if (dbFunction.isValidID(txtCurTerminalID.Text))
                {
                    cboSearchCurTerminalStatus.Enabled = cboCurTerminalLocation.Enabled = true;
                    dbAPI.FillSNStatusList(cboSearchCurTerminalStatus, "View", "FSR SN Status By Job Type", txtJobTypeDescription.Text + clsFunction.sPipe + cboSearchActionMade.Text + clsFunction.sPipe + (isReplacement ? 1 : 0));

                    // set status description
                    dbAPI.ExecuteAPI("GET", "Search", "Terminal SN Info", txtCurTerminalID.Text, "Get Info Detail", "", "GetInfoDetail");              
                    if (dbAPI.isNoRecordFound() == false)
                        cboSearchCurTerminalStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 30);
                }

                if (dbFunction.isValidID(txtCurSIMID.Text))
                {
                    cboSearchCurSIMStatus.Enabled = cboCurSIMLocation.Enabled = true;
                    dbAPI.FillSNStatusList(cboSearchCurSIMStatus, "View", "FSR SN Status By Job Type", txtJobTypeDescription.Text + clsFunction.sPipe + cboSearchActionMade.Text + clsFunction.sPipe + (isReplacement ? 1 : 0));

                    // set status description
                    dbAPI.ExecuteAPI("GET", "Search", "SIM SN Info", txtCurSIMID.Text, "Get Info Detail", "", "GetInfoDetail");
                    if (dbAPI.isNoRecordFound() == false)
                        cboSearchCurSIMStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 18);
                }

                if (dbFunction.isValidID(txtCurDockID.Text))
                {
                    cboSearchCurDockStatus.Enabled = true;
                    dbAPI.FillSNStatusList(cboSearchCurDockStatus, "View", "FSR SN Status By Job Type", txtJobTypeDescription.Text + clsFunction.sPipe + cboSearchActionMade.Text + clsFunction.sPipe + (isReplacement ? 1 : 0));
                }

                // Replace
                if (dbFunction.isValidID(txtRepTerminalID.Text))
                {
                    cboSearchRepTerminalStatus.Enabled = cboRepTerminalLocation.Enabled = true;
                    dbAPI.FillSNStatusList(cboSearchRepTerminalStatus, "View", "FSR SN Status By Job Type", txtJobTypeDescription.Text + clsFunction.sPipe + cboSearchActionMade.Text + clsFunction.sPipe + (isReplacement ? 0 : 0));

                    // set status description
                    dbAPI.ExecuteAPI("GET", "Search", "Terminal SN Info", txtRepTerminalID.Text, "Get Info Detail", "", "GetInfoDetail");
                    if (dbAPI.isNoRecordFound() == false)
                        cboSearchRepTerminalStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 30);
                }

                if (dbFunction.isValidID(txtRepSIMID.Text))
                {
                    cboSearchRepSIMStatus.Enabled = cboRepSIMLocation.Enabled = true;
                    dbAPI.FillSNStatusList(cboSearchRepSIMStatus, "View", "FSR SN Status By Job Type", txtJobTypeDescription.Text + clsFunction.sPipe + cboSearchActionMade.Text + clsFunction.sPipe + (isReplacement ? 0 : 0));

                    // set status description
                    dbAPI.ExecuteAPI("GET", "Search", "SIM SN Info", txtRepSIMID.Text, "Get Info Detail", "", "GetInfoDetail");
                    if (dbAPI.isNoRecordFound() == false)
                        cboSearchRepSIMStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 18);
                }


                if (dbFunction.isValidID(txtRepDockID.Text))
                {
                    cboSearchRepDockStatus.Enabled = true;
                    dbAPI.FillSNStatusList(cboSearchRepDockStatus, "View", "FSR SN Status By Job Type", txtJobTypeDescription.Text + clsFunction.sPipe + cboSearchActionMade.Text + clsFunction.sPipe + (isReplacement ? 0 : 0));
                }

                // check components status
                if (dbFunction.isValidCount(lvwStockDetail.Items.Count) ||
                    dbFunction.isValidCount(lvwRepStockDetail.Items.Count))
                {
                    dbAPI.FillSNStatusList(cboItemStatus, "View", "FSR SN Status By Job Type", txtJobTypeDescription.Text + clsFunction.sPipe + cboSearchActionMade.Text + clsFunction.sPipe + (isReplacement ? 0 : 0));
                }
                
                InitSearchReason();

                InitSearchRemoveButton(false);

                if (!fEdit)
                {
                    // --------------------------------------------------------
                    // Fill Service Result Location 
                    // --------------------------------------------------------
                    string pSearchValue = "";
                    if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC))
                    {
                        if (cboSearchActionMade.Text.CompareTo(dbAPI.GetActionMade()[1]) == 0) // SUCCESS                  
                            pSearchValue = clsDefines.Mode_Type_Deploy;
                        else if (cboSearchActionMade.Text.CompareTo(dbAPI.GetActionMade()[2]) == 0) // NEGATIVE                   
                            pSearchValue = clsDefines.Mode_Type_Return;

                        dbAPI.FillComboBoxServiceResultLocation(cboCurTerminalLocation, pSearchValue);
                        dbAPI.FillComboBoxServiceResultLocation(cboCurSIMLocation, pSearchValue);

                        // components
                        dbAPI.FillComboBoxServiceResultLocation(cboItemLocation, pSearchValue);

                    }

                    else if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_PULLOUT_DESC))
                    {
                        if (cboSearchActionMade.Text.CompareTo(dbAPI.GetActionMade()[1]) == 0) // SUCCESS                  
                            pSearchValue = clsDefines.Mode_Type_Return;
                        else if (cboSearchActionMade.Text.CompareTo(dbAPI.GetActionMade()[2]) == 0) // NEGATIVE                   
                            pSearchValue = clsDefines.Mode_Type_Deploy;

                        dbAPI.FillComboBoxServiceResultLocation(cboCurTerminalLocation, pSearchValue);
                        dbAPI.FillComboBoxServiceResultLocation(cboCurSIMLocation, pSearchValue);

                        // components
                        dbAPI.FillComboBoxServiceResultLocation(cboItemLocation, pSearchValue);
                    }

                    else if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
                    {
                        if (cboSearchActionMade.Text.CompareTo(dbAPI.GetActionMade()[1]) == 0) // SUCCESS
                        {
                            pSearchValue = clsDefines.Mode_Type_Deploy;

                            // Terminal
                            if (dbFunction.isValidID(txtRepTerminalID.Text))
                            {
                                dbAPI.FillComboBoxServiceResultLocation(cboCurTerminalLocation, clsDefines.Mode_Type_Return);
                                dbAPI.FillComboBoxServiceResultLocation(cboRepTerminalLocation, pSearchValue);
                            }
                            else
                            {
                                dbAPI.FillComboBoxServiceResultLocation(cboCurTerminalLocation, clsDefines.Mode_Type_Deploy);
                            }

                            // SIM
                            if (dbFunction.isValidID(txtRepSIMID.Text))
                            {
                                dbAPI.FillComboBoxServiceResultLocation(cboCurSIMLocation, clsDefines.Mode_Type_Return);
                                dbAPI.FillComboBoxServiceResultLocation(cboRepSIMLocation, pSearchValue);
                            }
                            else
                            {
                                dbAPI.FillComboBoxServiceResultLocation(cboCurSIMLocation, pSearchValue);
                            }

                            // components
                            if (dbFunction.isValidCount(lvwRepStockDetail.Items.Count))
                            {
                                dbAPI.FillComboBoxServiceResultLocation(cboItemLocation, clsDefines.Mode_Type_Return);
                            }
                            else
                            {
                                dbAPI.FillComboBoxServiceResultLocation(cboItemLocation, pSearchValue);
                            }

                        }

                        else if (cboSearchActionMade.Text.CompareTo(dbAPI.GetActionMade()[2]) == 0) // NEGATIVE    
                        {
                            pSearchValue = clsDefines.Mode_Type_Return;

                            // Terminal
                            if (dbFunction.isValidID(txtRepTerminalID.Text))
                            {
                                dbAPI.FillComboBoxServiceResultLocation(cboCurTerminalLocation, clsDefines.Mode_Type_Deploy);
                                dbAPI.FillComboBoxServiceResultLocation(cboRepTerminalLocation, pSearchValue);
                            }
                            else
                            {
                                dbAPI.FillComboBoxServiceResultLocation(cboCurTerminalLocation, clsDefines.Mode_Type_Deploy);
                            }

                            // SIM
                            if (dbFunction.isValidID(txtRepSIMID.Text))
                            {
                                dbAPI.FillComboBoxServiceResultLocation(cboCurSIMLocation, clsDefines.Mode_Type_Deploy);
                                dbAPI.FillComboBoxServiceResultLocation(cboRepSIMLocation, pSearchValue);
                            }
                            else
                            {
                                dbAPI.FillComboBoxServiceResultLocation(cboCurSIMLocation, clsDefines.Mode_Type_Deploy);
                            }

                            // components
                            if (dbFunction.isValidCount(lvwRepStockDetail.Items.Count))
                            {
                                dbAPI.FillComboBoxServiceResultLocation(cboItemLocation, clsDefines.Mode_Type_Deploy);
                            }
                            else
                            {
                                dbAPI.FillComboBoxServiceResultLocation(cboItemLocation, pSearchValue);
                            }
                            
                        }
                    }
                    // --------------------------------------------------------
                    // Fill Service Result Location 
                    // --------------------------------------------------------
                }

                if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_SERVICING_DESC) ||
                    txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC))
                {
                    InitCurrentComBoBox(false);
                    InitReplacementComBoBox(false);
                }

                txtMMerchRep.Focus();

            }

        }

        private void btnAddReason_Click(object sender, EventArgs e)
        {
            frmReason.sHeader = clsGlobalVariables.REASON_TYPE;
            clsSearch.ClassReasonType = clsGlobalVariables.REASON_TYPE;
            frmReason frm = new frmReason();
            frm.ShowDialog();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblSubHeader_Click(object sender, EventArgs e)
        {

        }

        private void groupBox7_Enter(object sender, EventArgs e)
        {

        }

        private void btnRemoveExpenses_Click(object sender, EventArgs e)
        {
            if (dbFunction.isValidAmount(txtTExpenses.Text))
            {
                if (MessageBox.Show("Expenses will be removed.\n" +
                                    "\n\n",
                                    "Continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    return;
                }

                frmServiceExpenses.ResetHoldExpenses();
                txtTExpenses.Text = clsFunction.sDefaultAmount;
            }

        }

        private void btnAddExpenses_Click(object sender, EventArgs e)
        {
            frmExpenses frm = new frmExpenses();
            frm.ShowDialog();
        }

        private void btnSearchExpenses_Click(object sender, EventArgs e)
        {
            frmServiceExpenses frm = new frmServiceExpenses();
            frm.ShowDialog();

            if (frm.fSelected)
            {
                txtTExpenses.Text = frm.sTExpenses;
            }
        }

        //private void SaveServiceExpenses()
        //{
        //    int i = 0;
        //    string sRowSQL = "";
        //    string sSQL = "";

        //    if (dbFunction.isValidAmount(txtTExpenses.Text))
        //    {
        //        while (clsArray.HoldExpensesID.Length > i)
        //        {
        //            if (dbFunction.isValidAmount(clsArray.HoldExpensesAmount[i]))
        //            {
        //                sSQL = "";
        //                sRowSQL = "";
        //                sRowSQL = "('" + txtServiceNo.Text + "'," +
        //                sRowSQL + sRowSQL + "'" + txtSearchTAIDNo.Text + "'," +
        //                sRowSQL + sRowSQL + "'" + txtIRIDNo.Text + "'," +
        //                sRowSQL + sRowSQL + "'" + clsArray.HoldExpensesID[i] + "'," +
        //                sRowSQL + sRowSQL + "'" + clsArray.HoldExpensesDescription[i] + "'," +
        //                sRowSQL + sRowSQL + " " + clsArray.HoldExpensesAmount[i] + ")";
        //                sSQL = sSQL + sRowSQL;

        //                Debug.WriteLine("SaveServiceExpenses::sSQL=" + sSQL);

        //                dbAPI.ExecuteAPI("POST", "Insert", "", "", "Service Expenses Detail", sSQL, "InsertCollectionDetail");

        //            }

        //            i++;
        //        }
        //    }
        //}

        private void txtMProblemReported_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (txtMProblemReported.Text.Length > 0)
                        txtMActualProblemReported.Focus();
                    break;
            }
        }

        private void txtMActualProblemReported_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (txtMActualProblemReported.Text.Length > 0)
                        txtMActionTaken.Focus();
                    break;
            }
        }

        private void txtMActionTaken_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (txtMActionTaken.Text.Length > 0)
                        txtMAnyComments.Focus();
                    break;
            }
        }
        
        private void txtMMPowerSN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtMMDockSN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtMMerchRep_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtMProblemReported_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtMActualProblemReported_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtMActionTaken_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtMAnyComments_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtMMerchContactNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void btnAddResolution_Click(object sender, EventArgs e)
        {
            frmReason.sHeader = clsGlobalVariables.RESOLUTION_TYPE;
            clsSearch.ClassReasonType = clsGlobalVariables.RESOLUTION_TYPE;
            frmReason frm = new frmReason();
            frm.ShowDialog();
        }

        private void btnSearchMerchant_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iDispatch;
            frmSearchField.sHeader = "SERVICE DISPATCH";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    fEdit = false;
                    InitCount();
                    iniComboBoxSelection(true);

                    dbFunction.ClearListViewItems(lvwMM);
                    dbFunction.ClearListViewItems(lvwProfile1);
                    dbFunction.ClearListViewItems(lvwProfile2);
                    dbFunction.ClearListViewItems(lvwRaw);

                    txtSearchServiceNo.Text = txtServiceNo.Text = clsSearch.ClassServiceNo.ToString();
                    txtMerchantName.Text = clsSearch.ClassParticularName;
                    txtClientID.Text = clsSearch.ClassClientID.ToString();
                    txtMerchantID.Text = clsSearch.ClassMerchantID.ToString();
                    txtFEID.Text = clsSearch.ClassFEID.ToString();
                    txtIRTID.Text = clsSearch.ClassTID;
                    txtIRMID.Text = clsSearch.ClassMID;

                    txtIRIDNo.Text = clsSearch.ClassIRIDNo.ToString();
                    txtSearchIRNo.Text = clsSearch.ClassIRNo;

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

                    loadServiceChangesDetail();

                    LoadServicingDetail();

                    loadStockMovementDetail(lvwStockDetail, true);
                    loadStockMovementDetail(lvwRepStockDetail, false);

                    dbAPI.loadMultiMerchantInfo(lvwMM, int.Parse(txtIRIDNo.Text));

                    cboSearchActionMade.Items.Clear();
                    dbAPI.FillComboBoxActionMade(cboSearchActionMade); // Load Action Made     

                    // SN Status/Location default select
                    cboSearchCurTerminalStatus.Text = cboSearchCurSIMStatus.Text = cboSearchCurDockStatus.Text = clsFunction.sDefaultSelect;
                    cboSearchRepTerminalStatus.Text = cboSearchRepSIMStatus.Text = cboSearchRepDockStatus.Text = clsFunction.sDefaultSelect;

                    // SN Status/Location Disable
                    cboCurTerminalLocation.Enabled = cboSearchCurTerminalStatus.Enabled = cboCurSIMLocation.Enabled = cboSearchCurSIMStatus.Enabled =
                    cboRepTerminalLocation.Enabled = cboSearchRepTerminalStatus.Enabled = cboRepSIMLocation.Enabled = cboSearchRepSIMStatus.Enabled = false;

                    SetMKTextBoxBackColor();

                    InitStatusTitle(false);
                    //lblHeader.Text = "CREATE FSR" + " " + "[ " + txtServiceJobTypeDescription.Text + " ]";

                    EnableDateTimeEntry(true);

                    PKTextBoxReadOnly(false);

                    cboSearchActionMade.Enabled = true;

                    InitSearchRemoveButton(false);

                    lblSubHeader.Text = txtJobTypeDescription.Text + " - " + txtFSRRequestNo.Text;

                    chkBillable.Checked = true;

                    EditableTextBox();

                    SetCount();

                    EditableServiceDateTime(true);

                    txtSearchFSRServiceResult.Text = cboSearchActionMade.Text;

                    btnCancelJO.Enabled = true;

                    btnRefreshSN.Enabled = true;

                    btnUpdateServiceDate.Enabled = false;

                    txtSearchFSRDesc.Text = clsFunction.sDash;

                    txtRequestID1.Text = dbAPI.getPrimaryIRNo(int.Parse(txtIRIDNo.Text));

                    txtTicketStatus.Text = clsFunction.sNull;

                    btnUpdateMerchRep.Enabled = true;
                    dbFunction.SetButtonIconImage(btnUpdateMerchRep);

                    btnUpdateMerchRep.Enabled = true;
                    dbFunction.SetButtonIconImage(btnUpdateMerchRep);

                    if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_SERVICING_DESC) ||
                        txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC))
                    {
                        InitCurrentComBoBox(false);
                        InitReplacementComBoBox(false);
                    }

                    btnCancelJO.Enabled = true;
                    btnRefreshService.Enabled = true;
                    btnIncludeInReport.Enabled = true;

                    // Init header
                    lblHeader.Text = "CREATE FSR" + " " + dbFunction.AddBracketStartEnd(txtServiceJobTypeDescription.Text) + " " + dbFunction.AddBracketStartEnd(txtIRTID.Text) + " " + dbFunction.AddBracketStartEnd(txtIRMID.Text);

                    tabFillUp.TabIndex = 0;

                    entryTextBox(true);

                    btnClear.Focus();

                    // check already serviced selected
                    if (dbFunction.isValidID(txtFSRNo.Text))
                    {
                        dbFunction.SetMessageBox("Merchant " + dbFunction.AddBracketStartEnd(txtMerchantName.Text) + " with job order " +
                            dbFunction.AddBracketStartEnd(txtServiceType1.Text) + " was already serviced on " + dbFunction.AddBracketStartEnd(txtFSRDate.Text) + "." + "\n\n" +
                            "Unable to process New Manual FSR.", "Manual FSR", clsFunction.IconType.iError);
                        btnSave.Enabled = false;
                        return;
                    }

                    Cursor.Current = Cursors.Default;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exceptional error message {ex.Message}");
                    dbFunction.SetMessageBox($"Exceptional error message {ex.Message}", "New: FSR", clsFunction.IconType.iError);
                }
              
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            InitTime();
        }

        private void InitTime()
        {
            DateTime dateTime = DateTime.Parse("09:00:00");

            dteMTimeArrived.Value = dateTime;
            dteMTimeArrived.CustomFormat = "hh:mm tt";
            dteMTimeArrived.Format = DateTimePickerFormat.Custom;

            dteMReceiptTime.Value = dateTime;
            dteMReceiptTime.CustomFormat = "hh:mm tt";
            dteMReceiptTime.Format = DateTimePickerFormat.Custom;

            dteMTimeStart.Value = dateTime;
            dteMTimeStart.CustomFormat = "hh:mm tt";
            dteMTimeStart.Format = DateTimePickerFormat.Custom;

            dteMTimeEnd.Value = dateTime;
            dteMTimeEnd.CustomFormat = "hh:mm tt";
            dteMTimeEnd.Format = DateTimePickerFormat.Custom;

        }

        private void btnFSRSearch_Click(object sender, EventArgs e)
        {
            if (fAutoLoadData)
            {
                frmSearchField.fSelected = true;
            }
            else
            {
                frmSearchField.iSearchType = frmSearchField.SearchType.iFSR;
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

                    fEdit = true;
                    InitCount();
                    iniComboBoxSelection(true);

                    txtSearchServiceNo.Text = txtServiceNo.Text = clsSearch.ClassServiceNo.ToString();
                    txtSearchFSRNo.Text = txtFSRNo.Text = clsSearch.ClassFSRNo.ToString();
                    txtMerchantName.Text = clsSearch.ClassParticularName;
                    txtClientID.Text = clsSearch.ClassClientID.ToString();
                    txtMerchantID.Text = clsSearch.ClassMerchantID.ToString();
                    txtFEID.Text = clsSearch.ClassFEID.ToString();
                    txtIRTID.Text = clsSearch.ClassTID;
                    txtIRMID.Text = clsSearch.ClassMID;

                    txtIRIDNo.Text = clsSearch.ClassIRIDNo.ToString();
                    txtSearchIRNo.Text = clsSearch.ClassIRNo;

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

                    FillMerchRepTextBox();

                    cboSearchCurTerminalStatus.Text = cboSearchCurSIMStatus.Text = cboSearchCurDockStatus.Text = clsFunction.sDefaultSelect;
                    cboSearchRepTerminalStatus.Text = cboSearchRepSIMStatus.Text = cboSearchRepDockStatus.Text = clsFunction.sDefaultSelect;

                    cboSearchActionMade.Items.Clear();
                    dbAPI.FillComboBoxActionMade(cboSearchActionMade); // Load Action Made     

                    FillServicingSNInfo();
                    FiillFSRInfo();
                    getServiceGeoLocation();

                    loadServiceChangesDetail();
                    LoadServicingDetail();

                    loadTATDetail();

                    loadStockMovementDetail(lvwStockDetail, true);
                    loadStockMovementDetail(lvwRepStockDetail, false);

                    dbAPI.loadMultiMerchantInfo(lvwMM, int.Parse(txtIRIDNo.Text));

                    SetMKTextBoxBackColor();

                    InitStatusTitle(false);

                    //lblHeader.Text = "UPDATE FSR" + " " + "[ " + txtServiceJobTypeDescription.Text + " ]";
                    lblSubHeader.Text = txtJobTypeDescription.Text + " - " + txtFSRRequestNo.Text;

                    EnableDateTimeEntry(true);

                    PKTextBoxReadOnly(false);

                    cboSearchActionMade.Enabled = false;
                    cboSearchRepTerminalStatus.Enabled = cboSearchCurTerminalStatus.Enabled = cboSearchCurSIMStatus.Enabled = cboSearchRepSIMStatus.Enabled = false;

                    InitSearchRemoveButton(false);

                    UpdateButton(false);

                    if (dbFunction.isValidID(txtSearchServiceNo.Text))
                    {
                        btnPreviewSvcHistory.Enabled = btnPreviewFSR.Enabled = true;
                    }

                    SetCount();

                    EditableServiceDateTime(false);

                    btnResetEmail.Enabled = btnSendFSRAndDiagEmail.Enabled = true;
                    btnViewDiagnostic.Enabled = true;
                    btnViewImages.Enabled = true;

                    txtSearchFSRServiceResult.Text = cboSearchActionMade.Text;

                    //InitCurrentComBoBox(false);
                    //InitReplacementComBoBox(false);

                    btnCancelJO.Enabled = true;
                    btnRefreshSN.Enabled = true;
                    btnOverride.Enabled = true;
                    btnRefreshService.Enabled = true;
                    btnUpdateServiceDate.Enabled = true;
                    btnIncludeInReport.Enabled = true;

                    setFSRMode();

                    txtRequestID1.Text = dbAPI.getPrimaryIRNo(int.Parse(txtIRIDNo.Text));

                    btnUpdateMerchRep.Enabled = true;
                    dbFunction.SetButtonIconImage(btnUpdateMerchRep);

                    btnUpdateMerchRep.Enabled = true;
                    dbFunction.SetButtonIconImage(btnUpdateMerchRep);

                    //checkCancelJOButton();

                    if (txtServiceJobTypeStatusDesc.Text.Equals(clsDefines.SERVICE_STATUS_COMPLETED))
                    {
                        InitCurrentComBoBox(false);
                        InitReplacementComBoBox(false);
                    }

                    btnCancelJO.Enabled = true;

                    // Init header
                    lblHeader.Text = "UPDATE FSR" + " " + dbFunction.AddBracketStartEnd(txtServiceJobTypeDescription.Text) + " " + dbFunction.AddBracketStartEnd(txtIRTID.Text) + " " + dbFunction.AddBracketStartEnd(txtIRMID.Text);

                    tabFillUp.TabIndex = 0;

                    entryTextBox(true);

                    if (dbFunction.isValidID(txtSearchFSRNo.Text))
                        getSignAndImageCount();

                    btnClear.Focus();

                    Cursor.Current = Cursors.Default;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exceptional error message {ex.Message}");
                    dbFunction.SetMessageBox($"Exceptional error message {ex.Message}", "Search: FSR", clsFunction.IconType.iError);
                }
                

            }
        }
        
        /*
        private void LoadData()
        {

            Cursor.Current = Cursors.WaitCursor;

            // Load TA Info            
            ComposeSearchValue();
            dbAPI.GetTAInfo();

            Debug.WriteLine("clsSearch.ClassFSRNo=" + clsSearch.ClassFSRNo);
            Debug.WriteLine("clsSearch.ClassServiceNo=" + clsSearch.ClassServiceNo);
            Debug.WriteLine("clsSearch.ClassIRIDNo=" + clsSearch.ClassIRIDNo);
            Debug.WriteLine("clsSearch.ClassTAIDNo=" + clsSearch.ClassTAIDNo);
            Debug.WriteLine("clsFSR.ClassTerminalSN=" + clsFSR.ClassTerminalSN);
            Debug.WriteLine("clsFSR.ClassSIMSN=" + clsFSR.ClassSIMSN);
            Debug.WriteLine("clsFSR.ClassPowerSN=" + clsFSR.ClassPowerSN);
            Debug.WriteLine("clsFSR.ClassDockSN=" + clsFSR.ClassDockSN);

            Debug.WriteLine("clsFSR.ClassFSRDate=" + clsFSR.ClassFSRDate);
            Debug.WriteLine("clsFSR.ClassFSRTime=" + clsFSR.ClassFSRTime);
            Debug.WriteLine("clsFSR.ClassTimeStart=" + clsFSR.ClassTimeStart);
            Debug.WriteLine("clsFSR.ClassTimeEnd=" + clsFSR.ClassTimeEnd);

            // Load IR
            txtIRIDNo.Text = clsSearch.ClassIRIDNo.ToString();
            txtSearchIRNo.Text = clsSearch.ClassIRNo;
            txtIRRequestDate.Text = clsSearch.ClassIRRequestDate;
            txtIRInstallationDate.Text = clsSearch.ClassIRInstallationDate;
            txtIRTID.Text = clsSearch.ClassTID;
            txtIRMID.Text = clsSearch.ClassMID;
            txtAppVersion.Text = clsSearch.ClassAppVersion;
            txtAppCRC.Text = clsSearch.ClassAppCRC;
            txtMerchantPrimaryNum.Text = clsSearch.ClassPrimaryNum;
            txtMerchantSecondaryNum.Text = clsSearch.ClassSecondaryNum;

            // Load TA
            txtServiceProviderID.Text = clsSearch.ClassServiceProviderID.ToString();
            txtSearchTAIDNo.Text = clsSearch.ClassTAIDNo.ToString();
            txtTADateTime.Text = clsSearch.ClassTADateTime;
            txtTAModifiedDateTime.Text = clsSearch.ClassTADateTime;
            txtIRSVCStatus.Text = clsSearch.ClassIRStatusDescription;
            txtProcessedBy.Text = clsSearch.ClassTAProcessedBy;
            txtModifiedBy.Text = clsSearch.ClassTAModifiedBy;
            txtProcessedDate.Text = clsSearch.ClassTAProcessedDateTime;
            txtModifiedDate.Text = clsSearch.ClassTAModifiedDateTime;
            txtTARemarks.Text = clsSearch.ClassTARemarks;
            txtTAComments.Text = clsSearch.ClassTAComments;
            txtTAServiceTypeStatus.Text = clsSearch.ClassServiceTypeStatus.ToString();
            txtTAStatus.Text = clsSearch.ClassServiceTypeStatusDescription;

            txtServiceNo.Text = txtSearchServiceNo.Text = clsSearch.ClassServiceNo.ToString();
            if (dbFunction.isValidID(txtServiceNo.Text))
                FillServicingTextBox();

            SetSearchJobType();

            // Load Region                
            txtRegionID.Text = clsSearch.ClassRegionID.ToString();
            txtMerchantRegion.Text = clsSearch.ClassRegion;

            // Load Mechant
            txtMerchantID.Text = clsSearch.ClassMerchantID.ToString();
            clsSearch.ClassParticularID = clsSearch.ClassMerchantID;
            clsSearch.ClassProvinceID = 0;
            clsSearch.ClassCityID = 0;
            clsSearch.ClassParticularTypeID = clsGlobalVariables.iMerchant_Type;
            clsSearch.ClassParticularTypeDescription = clsGlobalVariables.sMerchant_Type;
            clsSearch.ClassParticularName = clsFunction.sZero;
            if (dbFunction.isValidID(txtMerchantID.Text))
                FillMerchantTextBox();

            // Load Client
            txtClientID.Text = clsSearch.ClassClientID.ToString();
            clsSearch.ClassParticularID = clsSearch.ClassClientID;
            clsSearch.ClassProvinceID = 0;
            clsSearch.ClassCityID = 0;
            clsSearch.ClassParticularTypeID = clsGlobalVariables.iClient_Type;
            clsSearch.ClassParticularTypeDescription = clsGlobalVariables.sClient_Type;
            clsSearch.ClassParticularName = clsFunction.sZero;
            if (dbFunction.isValidID(txtClientID.Text))
                FillClientTextBox();

            // Load SP
            txtServiceProviderID.Text = clsSearch.ClassServiceProviderID.ToString();
            clsSearch.ClassParticularID = clsSearch.ClassServiceProviderID;
            clsSearch.ClassProvinceID = 0;
            clsSearch.ClassCityID = 0;
            clsSearch.ClassParticularTypeID = clsGlobalVariables.iSP_Type;
            clsSearch.ClassParticularTypeDescription = clsGlobalVariables.sSP_Type;
            clsSearch.ClassParticularName = clsFunction.sZero;
            if (dbFunction.isValidID(txtServiceProviderID.Text))
                FillSPTextBox();

            // Load FE
            txtFEID.Text = clsSearch.ClassFEID.ToString();
            clsSearch.ClassParticularID = clsSearch.ClassFEID;
            clsSearch.ClassProvinceID = 0;
            clsSearch.ClassCityID = 0;
            clsSearch.ClassParticularTypeID = clsGlobalVariables.iFE_Type;
            clsSearch.ClassParticularTypeDescription = clsGlobalVariables.sFE_Type;
            clsSearch.ClassParticularName = clsFunction.sZero;
            if (dbFunction.isValidID(txtFEID.Text))
                FillFETextBox();

            // Load Date/Time
            if (dbFunction.isValidID(txtSearchFSRNo.Text))
            {
                dbFunction.SetDate(dteMFSRDate, clsFSR.ClassFSRDate);
                dbFunction.SetTime(dteMTimeArrived, clsFSR.ClassTimeArrived);
                dbFunction.SetTime(dteMReceiptTime, clsFSR.ClassFSRTime);
                dbFunction.SetTime(dteMTimeStart, clsFSR.ClassTimeStart);
                dbFunction.SetTime(dteMTimeEnd, clsFSR.ClassTimeEnd);
            }

            Debug.WriteLine("txtFSRNo.Text=" + txtSearchFSRNo.Text);
            Debug.WriteLine("txtServiceNo.Text=" + txtServiceNo.Text);

            if (dbFunction.isValidID(txtSearchFSRNo.Text) && dbFunction.isValidID(txtServiceNo.Text))
            {
                LoadFSRFillUp(txtSearchFSRNo.Text, txtServiceNo.Text);
            }

            //FillSearchComBoBox(); // Load ComboBox Status

            // Load Current Detail
            if (dbFunction.isValidID(txtServiceNo.Text))
            {
                dbAPI.GetServicingCurrentTerminalInfo(txtServiceNo.Text, txtServiceRequestNo.Text);

                // Current Terminal
                txtCurTerminalID.Text = clsServicingDetail.ClassTerminalID.ToString();
                if (dbFunction.isValidID(txtCurTerminalID.Text))
                {
                    txtCurTerminalSN.Text = dbFunction.CheckAndSetNumericValue(clsServicingDetail.ClassTerminalSN);
                    txtCurTerminalSNStatus.Text = clsServicingDetail.ClassCurTerminalSNStatus.ToString();
                    PopulateCurrentTerminalTextBox(txtCurTerminalID.Text, txtCurTerminalSN.Text);
                }
                else
                {
                    cboSearchCurTerminalStatus.SelectedIndex = cboCurTerminalLocation.SelectedIndex = 0;
                    cboSearchCurTerminalStatus.Enabled = cboCurTerminalLocation.Enabled = false;
                }

                // Current SIM
                txtCurSIMID.Text = clsServicingDetail.ClassSIMID.ToString();
                if (dbFunction.isValidID(txtCurSIMID.Text))
                {
                    txtCurSIMSN.Text = dbFunction.CheckAndSetNumericValue(clsServicingDetail.SIMSN);
                    txtCurSIMSNStatus.Text = clsServicingDetail.CurSIMSNStatus.ToString();
                    PopulateCurrentSIMTextBox(txtCurSIMID.Text, txtCurSIMSN.Text);
                }
                else
                {
                    cboSearchCurSIMStatus.SelectedIndex = cboCurSIMLocation.SelectedIndex = 0;
                    cboSearchCurSIMStatus.Enabled = cboCurSIMLocation.Enabled =  false;
                }

                // Current Dock
                txtCurDockID.Text = clsServicingDetail.ClassDockID.ToString();
                if (dbFunction.isValidID(txtCurDockID.Text))
                {
                    txtCurDockSN.Text = dbFunction.CheckAndSetNumericValue(clsServicingDetail.ClassDockSN);
                    txtCurDockSNStatus.Text = clsServicingDetail.CurDockSNStatus.ToString();
                    PopulateCurrentDockTextBox(txtCurDockID.Text, txtCurDockSN.Text);
                }
                else
                {
                    //cboSearchCurDockStatus.SelectedIndex = 0;
                    //cboSearchCurDockStatus.Enabled = false;
                }

                if (txtJobTypeDescription.Text.CompareTo(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC) == 0)
                {
                    dbAPI.GetServicingReplaceTerminalInfo(txtServiceNo.Text, txtServiceRequestNo.Text);

                    // Replace Terminal
                    txtRepTerminalID.Text = clsServicingDetail.ClassTerminalID.ToString();
                    if (dbFunction.isValidID(txtRepTerminalID.Text))
                    {
                        txtRepTerminalSN.Text = dbFunction.CheckAndSetNumericValue(clsServicingDetail.ClassTerminalSN);
                        txtRepTerminalSNStatus.Text = clsServicingDetail.ClassRepTerminalSNStatus.ToString();
                        PopulateReplaceTerminalTextBox(txtRepTerminalID.Text, txtRepTerminalSN.Text);
                    }
                    else
                    {
                        cboSearchRepTerminalStatus.SelectedIndex = cboRepTerminalLocation.SelectedIndex = 0;
                        cboSearchRepTerminalStatus.Enabled = cboRepTerminalLocation.Enabled = false;
                    }

                    // Replace SIM
                    txtRepSIMID.Text = clsServicingDetail.ClassSIMID.ToString();
                    if (dbFunction.isValidID(txtRepSIMID.Text))
                    {
                        txtRepSIMSN.Text = dbFunction.CheckAndSetNumericValue(clsServicingDetail.SIMSN);
                        txtRepSIMSNStatus.Text = clsServicingDetail.RepSIMSNStatus.ToString();
                        PopulateReplaceSIMTextBox(txtRepSIMID.Text, txtRepSIMSN.Text);
                    }
                    else
                    {
                        cboSearchRepSIMStatus.SelectedIndex = cboRepSIMLocation.SelectedIndex = 0;
                        cboSearchRepSIMStatus.Enabled = cboRepSIMLocation.Enabled = false;
                    }

                    // Replace Dock
                    txtRepDockID.Text = clsServicingDetail.ClassDockID.ToString();
                    if (dbFunction.isValidID(txtRepDockID.Text))
                    {
                        txtRepDockSN.Text = dbFunction.CheckAndSetNumericValue(clsServicingDetail.ClassDockSN);
                        txtRepDockSNStatus.Text = clsServicingDetail.RepDockSNStatus.ToString();
                        PopulateReplaceDockTextBox(txtRepDockID.Text, txtRepDockSN.Text);
                    }
                    else
                    {
                        //cboSearchRepDockStatus.SelectedIndex = 0;
                        //cboSearchRepDockStatus.Enabled = false;
                    }
                }
            }
            else // ServiceNo = 0
            {
                // Current Terminal
                if (dbFunction.isValidID(txtCurTerminalID.Text) || dbFunction.isValidDescription(txtCurTerminalSN.Text))
                {
                    dbAPI.GetTerminalInfo(txtCurTerminalID.Text, txtCurTerminalSN.Text);
                    txtCurTerminalID.Text = clsTerminal.ClassTerminalID.ToString();
                    txtCurTerminalSN.Text = dbFunction.CheckAndSetNumericValue(clsTerminal.ClassTerminalSN);
                    txtCurTerminalSNStatus.Text = clsTerminal.ClassTerminalStatus.ToString();
                    PopulateCurrentTerminalTextBox(txtCurTerminalID.Text, txtCurTerminalSN.Text);
                }

                // Current SIM
                if (dbFunction.isValidID(txtCurSIMID.Text) || dbFunction.isValidDescription(txtCurSIMSN.Text))
                {
                    dbAPI.GetSIMInfo(txtCurSIMID.Text, txtCurSIMSN.Text);
                    txtCurSIMID.Text = clsSIM.ClassSIMID.ToString();
                    txtCurSIMSN.Text = dbFunction.CheckAndSetNumericValue(clsSIM.ClassSIMSN);
                    txtCurSIMSNStatus.Text = clsSIM.ClassSIMStatus.ToString();
                    PopulateCurrentSIMTextBox(txtCurSIMID.Text, txtCurSIMSN.Text);
                }

                // Current Dock
                if (dbFunction.isValidID(txtCurDockID.Text) || dbFunction.isValidDescription(txtCurDockSN.Text))
                {
                    dbAPI.GetTerminalInfo(txtCurDockID.Text, txtCurDockSN.Text);
                    txtCurDockID.Text = clsTerminal.ClassTerminalID.ToString();
                    txtCurDockSN.Text = dbFunction.CheckAndSetNumericValue(clsTerminal.ClassTerminalSN);
                    txtCurDockSNStatus.Text = clsTerminal.ClassTerminalStatus.ToString();
                    PopulateCurrentDockTextBox(txtCurDockID.Text, txtCurDockSN.Text);
                }
            }

            // Service Result
            if (dbFunction.isValidID(txtSearchFSRNo.Text))
            {
                if (clsFSR.ClassActionMade.Length > 0)
                    cboSearchActionMade.Text = clsFSR.ClassActionMade;
            }

            InitSubHeader();
            InitStatusTitle(false);

            //CheckComboBox();

            // Load Attempt                
            clsSearch.ClassClientID = int.Parse(txtClientID.Text);
            clsSearch.ClassClientName = txtClientName.Text;
            clsSearch.ClassServiceTypeStatus = int.Parse(txtTAServiceTypeStatus.Text);
            clsSearch.ClassServiceTypeStatusDescription = txtTAStatus.Text;

            clsSearch.ClassAdvanceSearchValue = txtIRTID.Text + clsFunction.sPipe + txtIRMID.Text + clsFunction.sPipe +
                                                txtCurTerminalSN.Text + clsFunction.sPipe;

            dbAPI.GetViewCount("Search", "FSR Service Count", clsSearch.ClassAdvanceSearchValue, "Get Count");

            int iCount = 0;
            string sFSRStatus = "";
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            txtAttemptCnt.Text = iCount.ToString();
            txtFSRStatus.Text = (iCount > 0 ? sFSRStatus : clsFunction.sDash);

            PKTextBoxBackColor(true);
            PKTextBoxReadOnly(false);
            DKTextBoxBackColor();
            dbFunction.DatePickerUnlock(true, this);

            btnAttempt.Enabled = true;
            chkCloseTicket.CheckState = CheckState.Checked;

            if (clsSearch.ClassServiceTypeStatus == clsGlobalVariables.TA_STATUS_INSTALLED)
            {
                btnAdd.Enabled = false;
                btnSave.Enabled = false;
            }
            else
            {
                fEdit = true;
                InitButton();
            }

            // Set search combobox status when NEW FSR Mode
            //if (!dbFunction.isValidID(txtFSRNo.Text))
            //    SetSearchComboBoxStatus();

            Cursor.Current = Cursors.Default;
        }
        */

        private void txtCurSIMCarrier_TextChanged(object sender, EventArgs e)
        {

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
                if (dbFunction.isValidID(txtSearchFSRNo.Text))
                {
                    //lblMainStatus.ForeColor = Color.Cyan;
                    lblMainStatus.Text = "UPDATE FSR";
                }
                else
                {
                    //lblMainStatus.ForeColor = Color.Yellow;
                    lblMainStatus.Text = "NEW FSR";
                }
            }

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
                                                clsSearch.ClassClientID + clsFunction.sPipe +
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
                                                clsSearch.ClassServiceNo + clsFunction.sPipe; // ServiceNo

            Debug.WriteLine("ComposeSearchValue, clsSearch.ClassSearchValue=" + clsSearch.ClassSearchValue);
        }

        /*
        private void MKTextBoxBackColor(bool isLock)
        {
            if (isLock)
            {
                txtSearchServiceNo.BackColor = clsFunction.SearchBackColor;
                txtSearchFSRNo.BackColor = clsFunction.SearchBackColor;
                txtSearchTAIDNo.BackColor = clsFunction.SearchBackColor;
                txtServiceNo.BackColor = clsFunction.SearchBackColor;
                txtSearchFSRNo.BackColor = clsFunction.SearchBackColor;
                txtJobTypeDescription.BackColor = clsFunction.MKBackColor;
                txtServiceReferenceNo.BackColor = clsFunction.MKBackColor;
                txtServiceJobTypeStatusDesc.BackColor = Color.Black;
                txtSearchFSRDesc.BackColor = Color.Purple;
            }
        }
        */

        private void label15_Click(object sender, EventArgs e)
        {

        }
        
        private void InitMessageCountLimit()
        {
            txtMActualProblemReported.MaxLength = iLimit;
            txtMAnyComments.MaxLength = iLimit;
            txtMProblemReported.MaxLength = iLimit;
            txtMActionTaken.MaxLength = iLimit;
            txtMMerchRemarks.MaxLength = iLimit;
            txtRMInstruction.MaxLength = iLimit;
        }

        private bool fConfirmDetails(bool fUpdate)
        {
            bool fConfirm = true;
            bool isTerminalChanged = false;
            bool isSIMChanged = false;
            int pOldID = 0;
            string pOldSN = clsFunction.sNull;
            int pNewID = 0;
            string pNewSN = clsFunction.sNull;
            string sCurrent = clsFunction.sNull;
            string sReplace = clsFunction.sNull;

            if (((txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC)) ||
                 (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_SERVICING_DESC)) ||
                 (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC))) && cboSearchActionMade.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS))
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

            if ((txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC)) && cboSearchActionMade.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS))
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

            // Warning message for switching SN
            if (isTerminalChanged || isSIMChanged)
            {
                if (!(txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC)))
                {
                    sCurrent = clsFunction.sNull +
                       (isTerminalChanged ?
                                   "TERMINAL SN SWITCH: " + "\n" +
                                   "    >Previous terminal SN: " + txtOldTerminalSN.Text + "\n" +
                                   "    >New terminal SN: " + txtCurTerminalSN.Text + "\n" +
                                                clsFunction.sLineSeparator + "\n" : clsFunction.sNull) +
                       (isSIMChanged ?
                                   "SIM SN SWITCH: " + "\n" +
                                   "    >Previous SIM SN: " + txtOldSIMSN.Text + "\n" +
                                   "    >New SIM SN: " + txtCurSIMSN.Text + "\n" +
                                                clsFunction.sLineSeparator + "\n" : clsFunction.sNull);

                    if (MessageBox.Show("Switching SN detected. See details below:" +
                        "\n\n" +
                        sCurrent +
                        "\n" +
                        "Do you still want to continue?", "SN switching", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                    {
                        return false;
                    }
                }
            }


            Debug.WriteLine("isTerminalChanged=" + isTerminalChanged + ",isSIMChanged=" + isSIMChanged);

            string sTemp = (fUpdate ? "Are you sure to update the following details below:\n\n" : "Are you to save the following details below:\n\n") +
                           clsFunction.sLineSeparator + "\n" +
                            "CLIENT :" + txtClientName.Text + "\n" +
                            "PRIMARY REQUEST ID: " + txtRequestID1.Text + "\n" +
                            "REQUEST ID: " + txtRequestID.Text + "\n" +
                            "REFERENCE NO: " + txtServiceReferenceNo.Text + "\n" +
                            "BILLABLE: " + (chkBillable.Checked ? clsFunction.sYes : clsFunction.sNo) + "\n" +
                           clsFunction.sLineSeparator + "\n" +
                           "JOB TYPE: " + txtJobTypeDescription.Text + "\n" +
                           "    >Job Order Date: " + txtServiceScheduleDate.Text + "\n" +
                           "    >Service Request ID: " + txtSearchIRNo.Text + "\n" +
                           "    >Request No: " + txtServiceRequestNo.Text + "\n" +
                           "    >Reference No: " + txtServiceReferenceNo.Text + "\n" +
                           "    >Created Date: " + txtServiceRequestDate.Text + "\n" +
                           "    >Schedule Date: " + txtServiceScheduleDate.Text + "\n" +
                           clsFunction.sLineSeparator + "\n" +
                           "    >Created Date: " + txtFSRDateTime.Text + "\n" +
                           "    >Serviced Date: " + dteMFSRDate.Value.ToString("MM-dd-yyyy") + "\n" +
                           "    >Serviced Time: " + dteMReceiptTime.Value.ToString("hh:mm:ss") + "\n" +
                           clsFunction.sLineSeparator + "\n" +
                           "MERCHANT: " + txtMerchantName.Text + "\n" +
                           "    >TID: " + txtIRTID.Text + "\n" +
                           "    >MID: " + txtIRMID.Text + "\n" +
                            clsFunction.sLineSeparator + "\n" +
                           "MERCHANT REPRESENTATIVE: " + txtMMerchRep.Text + "\n" +
                           "    >Position: " + txtMMerchPosition.Text + "\n" +
                           "    >Contact No.: " + txtMMerchContactNo.Text + "\n" +
                           "VENDOR REPRESENTATIVE: " + txtFEName.Text + "\n" +
                           "    >Contact No.: " + txtFEMobileNo.Text + "\n" +
                           clsFunction.sLineSeparator + "\n";

            sCurrent = clsFunction.sNull +
                       (isTerminalChanged ?
                                   "TERMINAL SN SWITCH: " + "\n" +
                                   "    >Previous terminal SN: " + txtOldTerminalSN.Text + "\n" +
                                   "    >New terminal SN: " + txtCurTerminalSN.Text + "\n" +
                                                clsFunction.sLineSeparator + "\n" :
                                   "CURRENT TERMINAL: " + dbFunction.CheckAndSetStringValue(txtCurTerminalSN.Text) + "\n" +
                                   "    >Type: " + dbFunction.CheckAndSetStringValue(txtCurTerminalType.Text) + "\n" +
                                   "    >Model: " + dbFunction.CheckAndSetStringValue(txtCurTerminalModel.Text) + "\n" +
                                   "    >Brand: " + dbFunction.CheckAndSetStringValue(txtCurTerminalBrand.Text) + "\n" +
                                   "    >Status: " + cboSearchCurTerminalStatus.Text + "\n" +
                                   "    >Location: " + cboCurTerminalLocation.Text + "\n" +
                                   clsFunction.sLineSeparator + "\n") +
                       (isSIMChanged ?
                                   "SIM SN SWITCH: " + "\n" +
                                   "    >Previous SIM SN: " + txtOldSIMSN.Text + "\n" +
                                   "    >New SIM SN: " + txtCurSIMSN.Text + "\n" +
                                                clsFunction.sLineSeparator + "\n" :
                                   "CURRENT SIM: " + dbFunction.CheckAndSetStringValue(txtCurSIMSN.Text) + "\n" +
                                   "    >Carrier: " + dbFunction.CheckAndSetStringValue(txtCurSIMCarrier.Text) + "\n" +
                                   "    >Status: " + cboSearchCurSIMStatus.Text + "\n" +
                                   "    >Location: " + cboCurSIMLocation.Text + "\n" +
                                   clsFunction.sLineSeparator + "\n");

            if (!dbFunction.isValidID(txtCurTerminalID.Text)) sCurrent = clsFunction.sNull;

            sReplace = clsFunction.sNull +
                       (isTerminalChanged ?
                                   "TERMINAL SN SWITCH: " + "\n" +
                                   "    >Previous terminal SN: " + txtOldTerminalSN.Text + "\n" +
                                   "    >New terminal SN: " + txtRepTerminalSN.Text + "\n" +
                                                clsFunction.sLineSeparator + "\n" :
                                   "REPLACE TERMINAL: " + dbFunction.CheckAndSetStringValue(txtRepTerminalSN.Text) + "\n" +
                                   "    >Type: " + dbFunction.CheckAndSetStringValue(txtRepTerminalType.Text) + "\n" +
                                   "    >Model: " + dbFunction.CheckAndSetStringValue(txtRepTerminalModel.Text) + "\n" +
                                   "    >Brand: " + dbFunction.CheckAndSetStringValue(txtRepTerminalBrand.Text) + "\n" +
                                   "    >Status: " + cboSearchRepTerminalStatus.Text + "\n" +
                                   "    >Location: " + cboRepTerminalLocation.Text + "\n" +
                                   clsFunction.sLineSeparator + "\n") +
                       (isSIMChanged ?
                                   "SIM SN SWITCH: " + "\n" +
                                   "    >Previous SIM SN: " + txtOldSIMSN.Text + "\n" +
                                   "    >New SIM SN: " + txtRepSIMSN.Text + "\n" +
                                                clsFunction.sLineSeparator + "\n" :
                                   "REPLACE SIM: " + dbFunction.CheckAndSetStringValue(txtRepSIMSN.Text) + "\n" +
                                   "    >Carrier: " + dbFunction.CheckAndSetStringValue(txtRepSIMCarrier.Text) + "\n" +
                                   "    >Status: " + cboSearchRepSIMStatus.Text + "\n" +
                                   "    >Location: " + cboRepSIMLocation.Text + "\n" +
                                   clsFunction.sLineSeparator + "\n");

            if (!dbFunction.isValidID(txtRepTerminalID.Text)) sReplace = clsFunction.sNull;

            // Clear message
            if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
                sCurrent = clsFunction.sNull;
            else
                sReplace = clsFunction.sNull;

            if (MessageBox.Show(sTemp +
                                sCurrent +
                                sReplace +
                                "\n" +
                                "Warning:" +
                                "\nSN's selected status will be applied." +
                                (isTerminalChanged ? "\nPrevious terminal SN status will change to AVAILABLE." : "") +
                                (isSIMChanged ? "\nPrevious SIM SN status will change to AVAILABLE." : "") +
                                (chkEmail.Checked ? "\nVendor representative will be notified via EMAIL." : "")
                                ,
                                (fUpdate ? "Confirm update" : "Confirm save") + (chkBillable.Checked ? " & billable" : ""), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fConfirm = false;
            }
            else
            {
                if (!isTerminalChanged)
                {
                    txtOldTerminalID.Text = txtOldTerminalSN.Text = clsFunction.sNull;
                }

                if (!isSIMChanged)
                {
                    txtOldSIMID.Text = txtOldSIMSN.Text = clsFunction.sNull;
                }
            }

            return fConfirm;
        }

        //private void LoadLastRequestedBy()
        //{
        //    int i = 0;
        //    clsSearch.ClassSearchValue = "FSR" + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtServiceNo.Text);

        //    Debug.WriteLine("clsSearch.ClassSearchValue=" + clsSearch.ClassSearchValue);

        //    dbAPI.ExecuteAPI("GET", "View", "Last Servicing Requested By", clsSearch.ClassSearchValue, "Servicing Detail", "", "ViewServicingDetail");

        //    if (!clsGlobalVariables.isAPIResponseOK) return;

        //    if (!dbAPI.isNoRecordFound())
        //    {
        //        while (clsArray.ServiceNo.Length > i)
        //        {

        //            txtMMerchRep.Text = clsArray.CustomerName[i].ToString();
        //            txtMMerchContactNo.Text = clsArray.CustomerContactNo[i].ToString();
        //            txtMMerchRemarks.Text = clsArray.Remarks[i].ToString();

        //            i++;
        //        }
        //    }
        //}
        
        //private void UpdateLastSNAllocated()
        //{
        //    Debug.WriteLine("--UpdateLastSNAllocated--");
        //    Debug.WriteLine("ServiceNo=" + txtServiceNo.Text);
        //    Debug.WriteLine("IRIDNo=" + txtIRIDNo.Text);
        //    Debug.WriteLine("JobTypeDescription=" + txtJobTypeDescription.Text);

        //    if (dbFunction.isValidID(txtServiceNo.Text) && dbFunction.isValidID(txtIRIDNo.Text))
        //    {
        //        // Installation
        //        if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC))
        //        {
        //            dbAPI.UpdateLastSNAllocated("Update Last Terminal Allocated", txtCurTerminalID.Text, txtMerchantID.Text, txtIRIDNo.Text, txtClientID.Text); // Current TerminalID
        //            dbAPI.UpdateLastSNAllocated("Update Last SIM Allocated", txtCurSIMID.Text, txtMerchantID.Text, txtIRIDNo.Text, txtClientID.Text); // Current SIMID
        //            dbAPI.UpdateLastSNAllocated("Update Last Terminal Allocated", txtCurDockID.Text, txtMerchantID.Text, txtIRIDNo.Text, txtClientID.Text); // Current DockID
        //        }

        //        // Replacement
        //        if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
        //        {
        //            dbAPI.UpdateLastSNAllocated("Update Last Terminal Allocated", txtCurTerminalID.Text, clsFunction.sZero, clsFunction.sZero, clsFunction.sZero); // Current TerminalID
        //            dbAPI.UpdateLastSNAllocated("Update Last SIM Allocated", txtCurSIMID.Text, clsFunction.sZero, clsFunction.sZero, clsFunction.sZero); // Current SIMID
        //            dbAPI.UpdateLastSNAllocated("Update Last Terminal Allocated", txtCurDockID.Text, clsFunction.sZero, clsFunction.sZero, clsFunction.sZero); // Current DockID

        //            dbAPI.UpdateLastSNAllocated("Update Last Terminal Allocated", txtRepTerminalID.Text, txtMerchantID.Text, txtIRIDNo.Text, txtClientID.Text); // Replace TerminalID
        //            dbAPI.UpdateLastSNAllocated("Update Last SIM Allocated", txtRepSIMID.Text, txtMerchantID.Text, txtIRIDNo.Text, txtClientID.Text); // Replace SIMID
        //            dbAPI.UpdateLastSNAllocated("Update Last Terminal Allocated", txtRepDockID.Text, txtMerchantID.Text, txtIRIDNo.Text, txtClientID.Text); // Replace DockID
        //        }

        //        // PullOut
        //        if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_PULLOUT_DESC))
        //        {
        //            dbAPI.UpdateLastSNAllocated("Update Last Terminal Allocated", txtCurTerminalID.Text, clsFunction.sZero, clsFunction.sZero, clsFunction.sZero); // Current TerminalID
        //            dbAPI.UpdateLastSNAllocated("Update Last SIM Allocated", txtCurSIMID.Text, clsFunction.sZero, clsFunction.sZero, clsFunction.sZero); // Current SIMID
        //            dbAPI.UpdateLastSNAllocated("Update Last Terminal Allocated", txtCurDockID.Text, clsFunction.sZero, clsFunction.sZero, clsFunction.sZero); // Current DockID
        //        }
        //    }
        //}
        
        private void UpdateButton(bool isClear)
        {
            if (isClear)
            {
                btnAdd.Enabled = true;
                btnSave.Enabled = false;
            }
            else
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
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnSearchSuccessReason_Click(object sender, EventArgs e)
        {
            clsSearch._isWriteResponse = true;
            frmSearchField.iSearchType = frmSearchField.SearchType.iReason;
            frmSearchField.sHeader = "SUCCESS REASON";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();
            clsSearch._isWriteResponse = false;

            if (frmSearchField.fSelected)
            {
                txtReasonID.Text = clsSearch.ClassReasonID.ToString();
                txtReasonCode.Text = clsSearch.ClassReasonCode;
                txtReasonDesc.Text = clsSearch.ClassReasonDescription;
            }
        }

        private void btnSearchResolutionReason_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = (cboSearchActionMade.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS) ? frmSearchField.SearchType.iReason : frmSearchField.SearchType.iNegativeReason);
            frmSearchField.sHeader = (cboSearchActionMade.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS) ? "RESOLVED" : "NEGATIVE") + " REASON";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                txtReasonID.Text = clsSearch.ClassReasonID.ToString();
                txtReasonDesc.Text = txtReason.Text = clsSearch.ClassReasonDescription;
                txtReasonDesc.BackColor = clsFunction.MKBackColor;
            }
        }

        private void InitSearchReason()
        {
            txtReasonDesc.BackColor = clsFunction.DisableBackColor;
            btnSearchReason.Enabled = false;
            btnRemoveReason.Enabled = false;

            if (!cboSearchActionMade.Text.Equals(clsFunction.sDefaultSelect))
            {
                txtReasonDesc.BackColor = clsFunction.MKBackColor;
                btnSearchReason.Enabled = true;
                btnRemoveReason.Enabled = true;
            }
        }

        private void btnRemoveResolutionReason_Click(object sender, EventArgs e)
        {
            txtResolutionID.Text = clsFunction.sZero;
            txtReasonDesc.Text = clsFunction.sNull;
            txtReasonDesc.BackColor = clsFunction.DisableBackColor;
        }

        private void btnRemoveSuccessReason_Click(object sender, EventArgs e)
        {

        }

        private void btnAddFE_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantID, txtMerchantID.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iClientID, txtClientID.Text)) return;

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
            txtFEID.Text = "";
            txtFEName.Text = "";
            txtFEAddress.Text = "";
        }

        private void SetComboBoxDefaultByService()
        {
            cboSearchCurTerminalStatus.Text = cboSearchCurSIMStatus.Text = cboSearchCurDockStatus.Text = clsFunction.sDefaultSelect;
            cboSearchRepTerminalStatus.Text = cboSearchRepSIMStatus.Text = cboSearchRepDockStatus.Text = clsFunction.sDefaultSelect;

            txtCurTerminalSNStatus.Text = txtCurSIMSNStatus.Text = txtCurDockSNStatus.Text = clsFunction.sZero;
            txtRepTerminalSNStatus.Text = txtRepSIMSNStatus.Text = txtRepDockSNStatus.Text = clsFunction.sZero;

            switch (iSearchJobType)
            {
                case clsAPI.JobType.iInstallation:

                    if (dbFunction.isValidID(txtCurTerminalID.Text))
                        cboSearchCurTerminalStatus.Text = clsGlobalVariables.STATUS_INSTALLED_DESC;

                    if (dbFunction.isValidID(txtCurSIMID.Text))
                        cboSearchCurSIMStatus.Text = clsGlobalVariables.STATUS_INSTALLED_DESC;

                    if (dbFunction.isValidID(txtCurDockID.Text))
                        cboSearchCurDockStatus.Text = clsGlobalVariables.STATUS_INSTALLED_DESC;

                    txtCurTerminalSNStatus.Text = txtCurSIMSNStatus.Text = txtCurDockSNStatus.Text = clsGlobalVariables.STATUS_INSTALLED.ToString();

                    break;

                case clsAPI.JobType.iReplacement:

                    // Current
                    if (dbFunction.isValidID(txtCurTerminalID.Text))
                        cboSearchCurTerminalStatus.Text = clsGlobalVariables.STATUS_AVAILABLE_DESC;

                    if (dbFunction.isValidID(txtCurSIMID.Text))
                        cboSearchCurSIMStatus.Text = clsGlobalVariables.STATUS_AVAILABLE_DESC;

                    if (dbFunction.isValidID(txtCurDockID.Text))
                        cboSearchCurDockStatus.Text = clsGlobalVariables.STATUS_AVAILABLE_DESC;

                    txtCurTerminalSNStatus.Text = txtCurSIMSNStatus.Text = txtCurDockSNStatus.Text = clsGlobalVariables.STATUS_AVAILABLE.ToString();

                    // Replace
                    if (dbFunction.isValidID(txtRepTerminalID.Text))
                        cboSearchRepTerminalStatus.Text = clsGlobalVariables.STATUS_INSTALLED_DESC;

                    if (dbFunction.isValidID(txtRepSIMID.Text))
                        cboSearchRepSIMStatus.Text = clsGlobalVariables.STATUS_INSTALLED_DESC;

                    if (dbFunction.isValidID(txtRepDockID.Text))
                        cboSearchRepDockStatus.Text = clsGlobalVariables.STATUS_INSTALLED_DESC;

                    txtRepTerminalSNStatus.Text = txtRepSIMSNStatus.Text = txtRepDockSNStatus.Text = clsGlobalVariables.STATUS_INSTALLED.ToString();

                    break;

                case clsAPI.JobType.iPullOut:

                    if (dbFunction.isValidID(txtCurTerminalID.Text))
                        cboSearchCurTerminalStatus.Text = clsGlobalVariables.STATUS_AVAILABLE_DESC;

                    if (dbFunction.isValidID(txtCurSIMID.Text))
                        cboSearchCurSIMStatus.Text = clsGlobalVariables.STATUS_AVAILABLE_DESC;

                    if (dbFunction.isValidID(txtCurDockID.Text))
                        cboSearchCurDockStatus.Text = clsGlobalVariables.STATUS_AVAILABLE_DESC;

                    txtCurTerminalSNStatus.Text = txtCurSIMSNStatus.Text = txtCurDockSNStatus.Text = clsGlobalVariables.STATUS_AVAILABLE.ToString();

                    break;


                default:

                    if (dbFunction.isValidID(txtCurTerminalID.Text))
                        cboSearchCurTerminalStatus.Text = clsGlobalVariables.STATUS_INSTALLED_DESC;

                    if (dbFunction.isValidID(txtCurSIMID.Text))
                        cboSearchCurSIMStatus.Text = clsGlobalVariables.STATUS_INSTALLED_DESC;

                    if (dbFunction.isValidID(txtCurDockID.Text))
                        cboSearchCurDockStatus.Text = clsGlobalVariables.STATUS_INSTALLED_DESC;

                    txtCurTerminalSNStatus.Text = txtCurSIMSNStatus.Text = txtCurDockSNStatus.Text = clsGlobalVariables.STATUS_INSTALLED.ToString();

                    break;
            }
        }

        private void SetSearchJobType()
        {
            Debug.WriteLine("--SearchJobType--");

            if (txtJobTypeDescription.Text.CompareTo(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC) == 0)
                iSearchJobType = clsAPI.JobType.iInstallation;
            else if (txtJobTypeDescription.Text.CompareTo(clsGlobalVariables.JOB_TYPE_SERVICING_DESC) == 0)
                iSearchJobType = clsAPI.JobType.iServicing;
            else if (txtJobTypeDescription.Text.CompareTo(clsGlobalVariables.JOB_TYPE_PULLOUT_DESC) == 0)
                iSearchJobType = clsAPI.JobType.iPullOut;
            else if (txtJobTypeDescription.Text.CompareTo(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC) == 0)
                iSearchJobType = clsAPI.JobType.iReplacement;
            else if (txtJobTypeDescription.Text.CompareTo(clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC) == 0)
                iSearchJobType = clsAPI.JobType.iReprogramming;

            Debug.WriteLine("iSearchJobType=" + iSearchJobType);
        }

        private void SetMKTextBoxBackColor()
        {
            txtMerchantName.BackColor = txtClientName.BackColor = txtFEName.BackColor = txtCurTerminalSN.BackColor = txtCurSIMSN.BackColor = txtRepTerminalSN.BackColor = txtRepSIMSN.BackColor = txtFSRDateTime.BackColor = txtRequestID1.BackColor = txtServiceType1.BackColor = lblMainStatus.BackColor = clsFunction.MKBackColor;
            txtReasonDesc.BackColor = clsFunction.MKBackColor;

            if (!txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
            {
                txtRepTerminalSN.BackColor = txtRepSIMSN.BackColor = clsFunction.DisableBackColor;
            }

            cboCurTerminalLocation.ForeColor = cboSearchCurTerminalStatus.ForeColor = cboCurSIMLocation.ForeColor = cboSearchCurSIMStatus.ForeColor = Color.Blue;
            cboRepTerminalLocation.ForeColor = cboSearchRepTerminalStatus.ForeColor = cboRepSIMLocation.ForeColor = cboSearchRepSIMStatus.ForeColor = Color.Blue;

            // Header
            //txtRequestID1.BackColor = txtServiceType1.BackColor = lblMainStatus.BackColor = Color.Navy;

            // status backcolor
            txtServiceJobTypeStatusDesc.BackColor = txtSearchFSRDesc.BackColor = txtSearchFSRServiceResult.BackColor = txtTicketStatus.BackColor = txtBillable.BackColor = txtDiagnostic.BackColor = txtMerchantSign.BackColor = txtIRStatusDescription.BackColor = clsFunction.StatusBackColor;
        }

        private void SetPKTextBoxBackColor()
        {
            txtSearchServiceNo.BackColor = txtSearchFSRNo.BackColor = txtSearchIRNo.BackColor = txtServiceRequestNo.BackColor = txtFSRRequestNo.BackColor = clsFunction.PKBackColor;
        }

        private void SetStatusTextBoxBackColor()
        {   
            txtFSRStatus.BackColor = clsFunction.StatusBackColor;
            
        }

        private void FillServicingSNInfo()
        {
            string sTerminalID = clsFunction.sNull;
            string sSIMID = clsFunction.sNull;

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

                if (dbAPI.isNoRecordFound() == false)
                {
                    txtServiceNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);

                    // current terminal
                    sTerminalID = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    if (dbFunction.isValidID(sTerminalID))
                    {
                        txtCurTerminalID.Text = txtOldTerminalID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                        txtCurTerminalSN.Text = txtOldTerminalSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                    }

                    // current sim
                    sSIMID = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                    if (dbFunction.isValidID(sSIMID))
                    {
                        txtCurSIMID.Text = txtOldSIMID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                        txtCurSIMSN.Text = txtOldSIMSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                    }

                    // replace terminal
                    sTerminalID = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5);
                    if (dbFunction.isValidID(sTerminalID))
                    {
                        txtRepTerminalID.Text = txtOldTerminalID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5);
                        txtRepTerminalSN.Text = txtOldTerminalSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                    }

                    // replace sim
                    sSIMID = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                    if (dbFunction.isValidID(sSIMID))
                    {
                        txtRepSIMID.Text = txtOldSIMID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                        txtRepSIMSN.Text = txtOldSIMSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                    }
                }

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

                    if (dbAPI.isNoRecordFound() == false)
                    {   
                        txtCurTerminalCode.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                        txtCurTerminalType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                        txtCurTerminalModel.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                        txtCurTerminalBrand.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                        txtCurTerminalLocation.Text = cboCurTerminalLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);
                        txtCurTerminalAssetType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 15);

                        txtCurTerminalSNStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 29);

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

                    if (dbAPI.isNoRecordFound() == false)
                    {   
                        txtCurSIMCarrier.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                        txtCurSIMLocation.Text = cboCurSIMLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);

                        txtCurSIMSNStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 17);

                    }
                }

                // --------------------------------------------------------------------------------------------------------------------
                // Get Terminal SN Info (Replace)
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

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtRepTerminalCode.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                        txtRepTerminalType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                        txtRepTerminalModel.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                        txtRepTerminalBrand.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                        txtRepTerminalLocation.Text = cboRepTerminalLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);
                        txtRepTerminalAssetType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 15);

                        txtRepTerminalSNStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 29);
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

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtRepSIMCarrier.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                        txtRepSIMLocation.Text = cboRepSIMLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);

                        txtRepSIMSNStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 17);
                    }
                }
                
                // Blank/null value for replace SN when job type is not REPLACEMENT
                if (!txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
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

                    cboRepTerminalLocation.Text = cboRepSIMLocation.Text = clsFunction.sDefaultSelect;
                }

            }
        }

        private void FiillServicingInfo()
        {
            txtServiceReferenceNo.Text =
            txtServiceCode.Text =
            txtServiceRequestDate.Text =
            txtServiceScheduleDate.Text =
            txtProcessedBy.Text =
            txtProcessedDate.Text =
            txtModifiedBy.Text =
            txtModifiedDate.Text =
            txtDispatchBy.Text =
            txtDispatchDate.Text =
            txtJobType.Text =
            txtJobTypeDescription.Text =
            txtServiceJobTypeStatusDesc.Text =
            txtMMerchRemarks.Text =
            txtMAppVesion.Text =
            txtMAppCRC.Text =
            txtFSRNo.Text =
            lblCreatedDate.Text =
            txtServiceStatusDescription.Text =
            txtServiceStatus.Text =
            txtTicketStatus.Text =
            txtTicketBy.Text =
            txtTicketDate.Text =
            txtSource.Text =
            txtCategory.Text =
            txtSubCategory.Text =
            clsFunction.sNull;

            if (dbFunction.isValidID(txtSearchServiceNo.Text))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Servicing Info", txtSearchServiceNo.Text, "Get Info Detail", "", "GetInfoDetail");

                // parse delimited
                dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                if (dbAPI.isNoRecordFound() == false)
                {
                    txtServiceNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                    txtServiceCode.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtServiceRequestNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                    txtServiceReferenceNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                    //txtSearchServiceDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                    //txtServiceRequestDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5);
                    txtProcessedBy.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                    txtProcessedDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                    lblCreatedDate.Text = "CREATED DATE: " + dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                    txtModifiedBy.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                    txtModifiedDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                    txtDispatchBy.Text = txtDispatcher.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                    txtDispatchDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 11);
                    txtJobType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 12);
                    txtJobTypeDescription.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);
                    txtServiceJobTypeStatusDesc.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 14);
                    txtMMerchRemarks.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 15);
                    txtMAppVesion.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 16);
                    txtMAppCRC.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 17);
                    txtServiceJobTypeDescription.Text = txtServiceType1.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 18);
                    
                    txtServiceScheduleDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 21);
                    txtServiceRequestDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 23);
                    txtFSRNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 25);
                    txtMProblemReported.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 27);
                    txtRMInstruction.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 28);

                    txtRequestID.Text = txtFSRRequestID.Text =  dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 33);

                    txtServiceStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe,34);
                    txtServiceStatusDescription.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 35);

                    // Ticket
                    setTicketStatus(int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 36)));
                    txtTicketBy.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 37);
                    txtTicketDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 38);

                    txtDispatchID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 41);

                    txtVendor.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 42);
                    txtRequestor.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 43);

                    txtReportedDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 44);

                    // Helpdesk
                    txtAssistNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 45);
                    txtProblemNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 46);

                }

                // fill additional info
                ServicingDetailController data = _mServicingDetailController.getInfo(txtSearchServiceNo.Text + clsFunction.sPipe + txtIRIDNo.Text);
                if (data != null)
                {
                    txtSource.Text = data.Source;
                    txtCategory.Text = data.Category;
                    txtSubCategory.Text = data.SubCategory;
                }
            }
        }

        private void InitCreatedDateTime()
        {
            DateTime ProcessDateTime = DateTime.Now;
            string sProcessDate = "";
            string sProcessTime = "";

            sProcessDate = ProcessDateTime.ToString("yyyy-MM-dd");
            sProcessTime = ProcessDateTime.ToString("HH:mm tt");

            txtFSRDateTime.Text = sProcessDate + " " + sProcessTime;
            txtFSRDate.Text = sProcessDate;
        }

        private void panel28_Paint(object sender, PaintEventArgs e)
        {

        }

        private void EnableDateTimeEntry(bool isEnable)
        {
            dteMFSRDate.Enabled = dteMTimeArrived.Enabled = dteMTimeArrived.Enabled = dteMReceiptTime.Enabled = dteMTimeStart.Enabled = dteMTimeEnd.Enabled = isEnable;

        }

        private void cboSearchCurTerminalStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassCurTerminalSNStatus = 0;
            if (!cboSearchCurTerminalStatus.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Status List", cboSearchCurTerminalStatus.Text);
                clsSearch.ClassCurTerminalSNStatus = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassCurTerminalSNStatus=" + clsSearch.ClassCurTerminalSNStatus);
            }
            txtCurTerminalSNStatus.Text = clsSearch.ClassCurTerminalSNStatus.ToString();
        }

        private void cboSearchCurSIMStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassCurSIMSNStatus = 0;
            if (!cboSearchCurSIMStatus.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Status List", cboSearchCurSIMStatus.Text);
                clsSearch.ClassCurSIMSNStatus = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassCurSIMSNStatus=" + clsSearch.ClassCurSIMSNStatus);
            }
            txtCurSIMSNStatus.Text = clsSearch.ClassCurSIMSNStatus.ToString();
        }

        private void cboSearchRepTerminalStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassRepTerminalSNStatus = 0;
            if (!cboSearchRepTerminalStatus.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Status List", cboSearchRepTerminalStatus.Text);
                clsSearch.ClassRepTerminalSNStatus = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassRepTerminalSNStatus=" + clsSearch.ClassRepTerminalSNStatus);
            }
            txtRepTerminalSNStatus.Text = clsSearch.ClassRepTerminalSNStatus.ToString();
        }

        private void cboSearchRepSIMStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassRepSIMSNStatus = 0;
            if (!cboSearchRepSIMStatus.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Status List", cboSearchRepSIMStatus.Text);
                clsSearch.ClassRepSIMSNStatus = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassRepSIMSNStatus=" + clsSearch.ClassRepSIMSNStatus);
            }
            txtRepSIMSNStatus.Text = clsSearch.ClassRepSIMSNStatus.ToString();
        }


        private void FillMerchRepTextBox()
        {

            txtMMerchRep.Text =
            txtMMerchPosition.Text =
            txtMMerchContactNo.Text = 
            txtMMerchEmail.Text = clsFunction.sNull;

            if (dbFunction.isValidID(txtSearchServiceNo.Text))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Servicing Merch Rep Info", txtSearchServiceNo.Text, "Get Info Detail", "", "GetInfoDetail");

                if (dbAPI.isNoRecordFound() == false)
                {
                    txtMMerchRep.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtMMerchPosition.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                    txtMMerchContactNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                    txtMMerchEmail.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);

                }
            }
        }

        private void InitSearchRemoveButton(bool isClear)
        {
            Debug.WriteLine("--InitSearchRemoveButton--");
            Debug.WriteLine("isClear=" + isClear);
            Debug.WriteLine("fEdit=" + fEdit);
            Debug.WriteLine("txtJobTypeDescription.Text=" + txtJobTypeDescription.Text);

            if (isClear)
            {
                btnSearchClient.Enabled = btnRemoveClient.Enabled = btnAddFE.Enabled = btnRemoveFE.Enabled =
                btnSearchCurTerminal.Enabled = btnRemoveCurTerminal.Enabled = btnSearchCurSIM.Enabled = btnRemoveCurSIM.Enabled =
                btnSearchRepTerminal.Enabled = btnSearchRepSIM.Enabled = btnRemoveRepTerminal.Enabled = btnRemoveRepSIM.Enabled = false;
            }
            else
            {
                btnSearchClient.Enabled = btnRemoveClient.Enabled = false;
                btnAddFE.Enabled = btnRemoveFE.Enabled = true;
                btnSearchCurTerminal.Enabled = btnRemoveCurTerminal.Enabled = btnSearchCurSIM.Enabled = btnRemoveCurSIM.Enabled =
                btnSearchRepTerminal.Enabled = btnSearchRepSIM.Enabled = btnRemoveRepTerminal.Enabled = btnRemoveRepSIM.Enabled = false;

                if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC) && cboSearchActionMade.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS))
                {
                    btnAddFE.Enabled = btnRemoveFE.Enabled =
                    //btnSearchCurTerminal.Enabled = btnRemoveCurTerminal.Enabled = btnSearchCurSIM.Enabled = btnRemoveCurSIM.Enabled =
                    btnSearchRepTerminal.Enabled = btnSearchRepSIM.Enabled = btnRemoveRepTerminal.Enabled = btnRemoveRepSIM.Enabled = (!fEdit ? true : false); ;
                }
                else if (((txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC) ||
                           txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_SERVICING_DESC) ||
                           txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC)) && cboSearchActionMade.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS)))
                {
                     btnRemoveFE.Enabled =
                     btnSearchCurTerminal.Enabled = btnRemoveCurTerminal.Enabled = btnSearchCurSIM.Enabled = btnRemoveCurSIM.Enabled = (!fEdit ? true : false); ;
                }

                if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_SERVICING_DESC) ||
                    txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC))
                {
                    btnSearchCurTerminal.Enabled = btnRemoveCurTerminal.Enabled = btnSearchCurSIM.Enabled = btnRemoveCurSIM.Enabled =
                    btnSearchRepTerminal.Enabled = btnSearchRepSIM.Enabled = btnRemoveRepTerminal.Enabled = btnRemoveRepSIM.Enabled = false;
                }
            }

            // Search button
            dbFunction.SetButtonIconImage(btnSearchCurTerminal);
            dbFunction.SetButtonIconImage(btnSearchCurSIM);
            dbFunction.SetButtonIconImage(btnSearchRepTerminal);
            dbFunction.SetButtonIconImage(btnSearchRepSIM);

            // Remove button
            dbFunction.SetButtonIconImage(btnRemoveCurTerminal);
            dbFunction.SetButtonIconImage(btnRemoveCurSIM);
            dbFunction.SetButtonIconImage(btnRemoveRepTerminal);
            dbFunction.SetButtonIconImage(btnRemoveRepSIM);

            // Find
            //dbFunction.SetButtonIconImage(btnSearchMerchant);
            //dbFunction.SetButtonIconImage(btnFSRSearch);
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


            }
        }

        private void btnRemoveClient_Click(object sender, EventArgs e)
        {
            txtClientID.Text = txtClientName.Text = clsFunction.sNull;
        }

        private void btnRemoveFE_Click_1(object sender, EventArgs e)
        {
            txtFEID.Text = 
            txtFEName.Text = 
            txtFEMobileNo.Text = 
            txtFEEmail.Text = clsFunction.sNull;
        }

        private void btnRemoveCurTerminal_Click(object sender, EventArgs e)
        {
            txtCurTerminalID.Text =
                txtCurTerminalSN.Text =
                txtCurTerminalCode.Text =
                txtCurTerminalType.Text =
                txtCurTerminalModel.Text =
                txtCurTerminalBrand.Text =
                txtCurTerminalLocation.Text =
                txtCurTerminalAssetType.Text = clsFunction.sNull;

            cboSearchCurTerminalStatus.Text = clsFunction.sDefaultSelect;
        }

        private void dteMFSRDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtCurTerminalSN_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSearchCurTerminal_Click(object sender, EventArgs e)
        {

            frmSearchField.iSearchType = frmSearchField.SearchType.iTerminal;
            frmSearchField.iStatus = clsGlobalVariables.STATUS_AVAILABLE;
            frmSearchField.sTerminalType = "View Terminal";
            frmSearchField.sHeader = "TERMINAL";
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

                txtCurTerminalID.Text = clsSearch.ClassTerminalID.ToString();
                txtCurTerminalSN.Text = txtNewTerminalSN.Text = clsSearch.ClassTerminalSN;
                PopulateTerminalTextBox(txtCurTerminalID.Text, txtCurTerminalSN.Text, true);

                // check SN value
                if (!dbFunction.isValidDescription(txtCurTerminalSN.Text) ||
                    !dbFunction.isValidDescription(txtCurTerminalType.Text) ||
                    !dbFunction.isValidDescription(txtCurTerminalModel.Text) ||
                    !dbFunction.isValidDescription(txtCurTerminalBrand.Text) ||
                    !dbFunction.isValidDescription(txtCurTerminalLocation.Text) ||
                    !dbFunction.isValidDescription(txtCurTerminalAssetType.Text))
                {
                    dbFunction.SetMessageBox("Invalid current TERMINAL information selected." + "\n\n" +
                                              "Current TERMINAL information" + "\n" +
                                              " >SN : " + txtCurTerminalSN.Text + "\n" +
                                              " >Type : " + txtCurTerminalType.Text + "\n" +
                                              " >Model : " + txtCurTerminalModel.Text + "\n" +
                                              " >Brand : " + txtCurTerminalBrand.Text + "\n" +
                                              " >Location : " + txtCurTerminalLocation.Text + "\n" +
                                              " >Assert Type : " + txtCurTerminalAssetType.Text + "\n\n" +                                          
                                              "Kind check the selected record.", "Field checking", clsFunction.IconType.iError);
                    return;
                }

                // Handle terminal SN has not been released
                if (clsSearch.ClassIsReleased <= 0)
                {
                    dbFunction.SetMessageBox("Terminal SN " + "[" + txtCurTerminalSN.Text + "]" + " has not been released. ", "Unable to add", clsFunction.IconType.iError);

                    txtCurTerminalID.Text =
                    txtCurTerminalSN.Text =
                    txtCurTerminalCode.Text =
                    txtCurTerminalType.Text =
                    txtCurTerminalModel.Text =
                    txtCurTerminalBrand.Text =
                    txtCurTerminalLocation.Text =
                    txtCurTerminalAssetType.Text = clsFunction.sNull;

                    cboSearchCurTerminalStatus.Text = clsFunction.sDefaultSelect;

                    return;
                }

            }
        }

        private void btnSearchCurSIM_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iSIM;
            frmSearchField.iStatus = clsGlobalVariables.STATUS_AVAILABLE;
            frmSearchField.sTerminalType = "View SIM";
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
                    dbFunction.SetMessageBox("SIM SN " + "[" + clsSearch.ClassSIMSerialNo + "]" + " status is not AVAILABLE.", "Unable to add", clsFunction.IconType.iError);
                    return;
                }

                txtCurSIMID.Text = clsSearch.ClassSIMID.ToString();
                txtCurSIMSN.Text = txtNewSIMSN.Text = clsSearch.ClassSIMSerialNo;
                PopulateSIMTextBox(txtCurSIMID.Text, txtCurSIMSN.Text, true);

                // check SN value
                if (!dbFunction.isValidDescription(txtCurSIMSN.Text) ||
                    !dbFunction.isValidDescription(txtCurSIMCarrier.Text) ||
                    !dbFunction.isValidDescription(txtCurSIMLocation.Text))
                {
                    dbFunction.SetMessageBox("Invalid current SIM information selected." + "\n\n" +
                                              "Current SIM information" + "\n" +
                                              " >SN : " + txtCurSIMSN.Text + "\n" +
                                              " >Carrier : " + txtCurSIMCarrier.Text + "\n" +
                                              " >Location : " + txtCurSIMLocation.Text + "\n\n" +                                              
                                              "Kind check the selected record.", "Field checking", clsFunction.IconType.iError);
                    return;
                }

                // Handle terminal SN has not been released
                if (clsSearch.ClassIsReleased <= 0)
                {
                    dbFunction.SetMessageBox("SIM SN " + "[" + txtCurSIMSN.Text + "]" + " has not been released. ", "Unable to add", clsFunction.IconType.iError);

                    txtCurSIMID.Text =
                    txtCurSIMSNStatus.Text =
                    txtCurSIMSN.Text =
                    txtCurSIMCarrier.Text =
                    txtCurSIMLocation.Text = clsFunction.sNull;

                    return;
                }
            }
        }

        private void PopulateTerminalTextBox(string sTerminalID, string sTerminalSN, bool isCurrent)
        {
            if (isCurrent)
            {
                txtCurTerminalSN.Text =
                txtCurTerminalCode.Text =
                txtCurTerminalType.Text =
                txtCurTerminalModel.Text =
                txtCurTerminalBrand.Text =
                txtCurTerminalLocation.Text =
                txtCurTerminalAssetType.Text = clsFunction.sNull;

                if (dbFunction.isValidID(txtMerchantID.Text))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "Terminal SN Info", txtCurTerminalID.Text, "Get Info Detail", "", "GetInfoDetail");

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtCurTerminalID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                        txtCurTerminalSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                        txtCurTerminalCode.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                        txtCurTerminalType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                        txtCurTerminalModel.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                        txtCurTerminalBrand.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                        txtCurTerminalLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);
                        txtCurTerminalAssetType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 15);

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

                if (dbFunction.isValidID(txtMerchantID.Text))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "Terminal SN Info", txtRepTerminalID.Text, "Get Info Detail", "", "GetInfoDetail");

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtRepTerminalID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);

                        txtRepTerminalSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                        txtRepTerminalCode.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                        txtRepTerminalType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                        txtRepTerminalModel.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                        txtRepTerminalBrand.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                        txtRepTerminalLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);
                        txtRepTerminalAssetType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 15);

                        clsSearch.ClassIsReleased = int.Parse(dbFunction.CheckAndSetNumericValue(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 26)));
                    }
                }
            }

        }
        private void PopulateSIMTextBox(string sSIMID, string sSIMSN, bool isCurrent)
        {
            if (isCurrent)
            {
                txtCurSIMSN.Text =
                txtCurSIMCarrier.Text =
                txtCurSIMLocation.Text = clsFunction.sNull;

                if (dbFunction.isValidID(txtMerchantID.Text))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "SIM SN Info", txtCurSIMID.Text, "Get Info Detail", "", "GetInfoDetail");

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtCurSIMID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                        txtCurSIMSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                        txtCurSIMCarrier.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                        txtCurSIMLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);

                        clsSearch.ClassIsReleased = int.Parse(dbFunction.CheckAndSetNumericValue(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 16)));
                    }
                }
            }
            else
            {
                txtRepSIMSN.Text =
                txtRepSIMCarrier.Text =
                txtRepSIMLocation.Text = clsFunction.sNull;

                if (dbFunction.isValidID(txtMerchantID.Text))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "SIM SN Info", txtRepSIMID.Text, "Get Info Detail", "", "GetInfoDetail");

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtRepSIMID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                        txtRepSIMSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                        txtRepSIMCarrier.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                        txtRepSIMLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                    }
                }
            }

        }

        private void btnPreviewSvcHistory_Click(object sender, EventArgs e)
        {
            if (!dbFunction.fPromptConfirmation("Are you sure to preview Service History report?")) return;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iClientName, txtClientName.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantName, txtMerchantName.Text)) return;
            
            dbReportFunc.ViewServiceHistoryDetail(11);
            
        }

        private void FiillFSRInfo()
        {
            bool isBillable = false;
            bool isReport = false;

            txtSearchFSRNo.Text = clsFunction.sNull;

            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtFSRNo.Text))
            {
                dbAPI.ExecuteAPI("GET", "Search", "FSR Info", txtSearchServiceNo.Text, "Get Info Detail", "", "GetInfoDetail");

                dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                try
                {
                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtSearchFSRNo.Text = txtFSRNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                        dteMFSRDate.Value = DateTime.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2));
                    
                        txtFSRDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);

                        dteMTimeArrived.Value = DateTime.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3));
                        dteMReceiptTime.Value = DateTime.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4));
                        dteMTimeStart.Value = DateTime.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5));
                        dteMTimeEnd.Value = DateTime.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6));
                        clsSearch.ClassActionMade = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);

                        txtFEID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                        txtFEName.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);

                        txtMMerchRep.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                        txtMMerchPosition.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 11);
                        txtMMerchContactNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 12);

                        txtMMPowerSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);
                        txtMMDockSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 14);

                        txtMProblemReported.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 15);
                        txtMActionTaken.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 16);
                        txtMMerchRemarks.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 17);
                        txtMActualProblemReported.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 18);
                        txtMAnyComments.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 19);

                        txtReasonID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 20);

                        txtReasonDesc.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 21);

                        txtFSRRequestNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 22);

                        // check billable
                        string sBillable = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 23);
                        isBillable = (sBillable.Equals(clsFunction.sOne) ? true : false);
                        chkBillable.Checked = isBillable;

                        txtBillable.Text = (dbFunction.isValidID(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 23)) ? clsDefines.gYes : clsDefines.gNo);

                        txtFSRDateTime.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 24);

                        txtAttemptCnt.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 25);

                        txtMobileID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 26);
                        txtMobileTerminalID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 27);
                        txtMobileVersion.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 28);
                        cboDependency.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 29);
                        cboStatusReason.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 30);

                        txtBeyondReason.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 31);
                        
                        if (dbFunction.isValidID(txtFSRNo.Text))
                            txtFSRRequestID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 32);

                        // check isReport
                        string sReport = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 33);
                        isReport = (sReport.Equals(clsFunction.sOne) ? true : false);
                        chkIncludeInReport.Checked = isReport;

                        cboSearchActionMade.Text = clsSearch.ClassActionMade;
                        
                        txtDiagnostic.Text = (dbAPI.isRecordExist("Search", "Diagnostic Detail", txtSearchServiceNo.Text + clsDefines.gPipe + txtSearchFSRNo.Text) ? clsDefines.gYes : clsDefines.gNo);
                        string pFileName = txtSearchServiceNo.Text + "_" + dbFunction.padLeftChar(clsDefines.MERCHANT_SIGNATURE_INDEX.ToString(), "0", 2) + clsDefines.FILE_EXT_PNG;
                        txtMerchantSign.Text = (dbAPI.isFileExist("Search", "Check Upload File", pFileName) ? clsDefines.gYes : clsDefines.gNo);

                        txtGeneralFReference.Text = getGeneralActionTaken();
                        
                    }

                }
                catch (Exception ex)
                {
                    dbFunction.SetMessageBox("Exception handle error." + "\n\n" + ex.Message + "\n\n" + "FSR No: " + txtSearchServiceNo.Text + "\n\n" + clsDefines.CONTACT_ADMIN_MESSAGE, "FSR", clsFunction.IconType.iError);
                }
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
                    !dbFunction.isValidDescription(txtRepTerminalAssetType.Text))
                {
                    dbFunction.SetMessageBox("Invalid replace TERMINAL information selected." + "\n\n" +
                                  "Replace TERMINAL information" + "\n" +
                                  " >SN : " + txtRepTerminalSN.Text + "\n" +
                                  " >Type : " + txtRepTerminalType.Text + "\n" +
                                  " >Model : " + txtRepTerminalModel.Text + "\n" +
                                  " >Brand : " + txtRepTerminalBrand.Text + "\n" +
                                  " >Location : " + txtRepTerminalLocation.Text + "\n" +
                                  " >Assert Type : " + txtRepTerminalAssetType.Text + "\n\n" +                                
                                  "Kind check the selected record.", "Field checking", clsFunction.IconType.iError);
                    return;
                }

                // Handle terminal SN has not been released
                if (clsSearch.ClassIsReleased <= 0)
                {
                    dbFunction.SetMessageBox("Terminal SN " + "[" + txtRepTerminalSN.Text + "]" + " has not been released. ", "Unable to add", clsFunction.IconType.iError);

                    txtRepTerminalID.Text =
                    txtRepTerminalSN.Text =
                    txtRepTerminalCode.Text =
                    txtRepTerminalType.Text =
                    txtRepTerminalModel.Text =
                    txtRepTerminalBrand.Text =
                    txtRepTerminalLocation.Text =
                    txtRepTerminalAssetType.Text = clsFunction.sNull;

                    cboSearchCurTerminalStatus.Text = clsFunction.sDefaultSelect;

                    return;
                }

            }
        }

        private void btnSearchRepSIM_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iSIM;
            frmSearchField.iStatus = clsGlobalVariables.STATUS_AVAILABLE;
            frmSearchField.sTerminalType = "View SIM";
            frmSearchField.sHeader = "REPLACE SIM";
            frmSearchField.isCheckBoxes = false;
            frmSearchField.iLocationID = clsFunction.iZero;
            frmSearchField.sLocation = clsFunction.sDefaultSelect;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            Debug.WriteLine("Is Released --- " + clsSearch.ClassIsReleased);

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
                    !dbFunction.isValidDescription(txtRepSIMLocation.Text))
                {
                    dbFunction.SetMessageBox("Invalid replace SIM information selected." + "\n\n" +
                                  "Replace SIM information" + "\n" +
                                  " >SN : " + txtRepSIMSN.Text + "\n" +
                                  " >Carrier : " + txtRepSIMCarrier.Text + "\n" +
                                  " >Location : " + txtRepSIMLocation.Text + "\n\n" +                               
                                  "Kind check the selected record.", "Field checking", clsFunction.IconType.iError);
                    return;
                }

                // Handle terminal SN has not been released
                if (clsSearch.ClassIsReleased <= 0)
                {
                    dbFunction.SetMessageBox("SIM SN " + "[" + txtRepSIMSN.Text + "]" + " has not been released. ", "Unable to add", clsFunction.IconType.iError);

                    txtRepSIMID.Text =
                    txtRepSIMSNStatus.Text =
                    txtRepSIMSN.Text =
                    txtRepSIMCarrier.Text =
                    txtRepSIMLocation.Text = clsFunction.sNull;

                    return;
                }

            }
        }

        private void SaveTerminalActivity()
        {
            string sRowSQL = "";
            string sSQL = "";
            string sTerminalID = "";
            string sTerminalSN = "";

            DateTime stFSRDate = dteMFSRDate.Value;
            string sFSRDate = stFSRDate.ToString("yyyy-MM-dd");
            string sMReceiptTime = dbFunction.GetDateFromParse(dteMReceiptTime.Text, "hh:mm tt", "HH:mm:ss");

            Debug.WriteLine("--SaveTerminalActivity--");

            Cursor.Current = Cursors.WaitCursor;

            dbFunction.GetProcessedByAndDateTime(); // Get processedby and datetime

            dbFunction.GetModifiedByAndDateTime(); // Get modifiedby and datetime

            if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC) && cboSearchActionMade.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS))
            {
                clsSearch.ClassStatus = clsGlobalVariables.STATUS_INSTALLATION;
                clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_INSTALLATION_DESC;

                clsSearch.ClassOperation = clsGlobalVariables.STATUS_INSTALLED;
                clsSearch.ClassOperationDescription = clsGlobalVariables.STATUS_INSTALLED_DESC;

                sTerminalID = txtCurTerminalID.Text;
                sTerminalSN = txtCurTerminalSN.Text;
            }
            else if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC) && cboSearchActionMade.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS))
            {
                clsSearch.ClassStatus = clsGlobalVariables.STATUS_REPLACEMENT;
                clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_REPLACEMENT_DESC;

                clsSearch.ClassOperation = clsGlobalVariables.STATUS_REPLACED;
                clsSearch.ClassOperationDescription = clsGlobalVariables.STATUS_REPLACED_DESC;


                sTerminalID = txtRepTerminalID.Text;
                sTerminalSN = txtRepTerminalSN.Text;

            }
            else if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_PULLOUT_DESC) && cboSearchActionMade.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS))
            {
                clsSearch.ClassStatus = clsGlobalVariables.STATUS_PULLED_OUT;
                clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_PULLED_OUT_DESC;

                clsSearch.ClassOperation = clsGlobalVariables.STATUS_PULLEDOUT;
                clsSearch.ClassOperationDescription = clsGlobalVariables.STATUS_PULLEDOUT_DESC;

                sTerminalID = txtCurTerminalID.Text;
                sTerminalSN = txtCurTerminalSN.Text;
            }

            sSQL = "";
            sRowSQL = "";
            sRowSQL = " ('" + dbFunction.CheckAndSetNumericValue(sTerminalID) + "', " +
            sRowSQL + sRowSQL + " '" + sTerminalSN + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtMerchantID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + txtMerchantName.Text + "', " +
            sRowSQL + sRowSQL + " '" + clsSearch.ClassStatus + "', " +
            sRowSQL + sRowSQL + " '" + clsSearch.ClassStatusDescription + "', " +
            sRowSQL + sRowSQL + " '" + clsSearch.ClassOperation + "', " +
            sRowSQL + sRowSQL + " '" + clsSearch.ClassOperationDescription + "', " +
            sRowSQL + sRowSQL + " '" + sFSRDate + "', " +
            sRowSQL + sRowSQL + " '" + sMReceiptTime + "', " +
            sRowSQL + sRowSQL + " '" + txtMMerchRemarks.Text + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassProcessedBy + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassProcessedDateTime + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassModifiedBy + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassModifiedDateTime + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtServiceNo.Text) + "', " +
            sRowSQL + sRowSQL + " '" + txtSearchIRNo.Text + "') ";
            sSQL = sSQL + sRowSQL;

            Debug.WriteLine("sSQL=" + sSQL);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "Terminal Activity", sSQL, "InsertCollectionDetail");

            Cursor.Current = Cursors.Default;

        }

        private void SaveSIMActivity()
        {
            string sRowSQL = "";
            string sSQL = "";
            string sSIMID = "";
            string sSIMSN = "";

            DateTime stFSRDate = dteMFSRDate.Value;
            string sFSRDate = stFSRDate.ToString("yyyy-MM-dd");
            string sMReceiptTime = dbFunction.GetDateFromParse(dteMReceiptTime.Text, "hh:mm tt", "HH:mm:ss");

            Debug.WriteLine("--SaveSIMActivity--");

            Cursor.Current = Cursors.WaitCursor;

            dbFunction.GetProcessedByAndDateTime(); // Get processedby and datetime

            dbFunction.GetModifiedByAndDateTime(); // Get modifiedby and datetime

            if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC) && cboSearchActionMade.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS))
            {
                clsSearch.ClassStatus = clsGlobalVariables.STATUS_INSTALLATION;
                clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_INSTALLATION_DESC;

                clsSearch.ClassOperation = clsGlobalVariables.STATUS_INSTALLED;
                clsSearch.ClassOperationDescription = clsGlobalVariables.STATUS_INSTALLED_DESC;

                sSIMID = txtCurSIMID.Text;
                sSIMSN = txtCurSIMSN.Text;
            }
            else if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC) && cboSearchActionMade.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS))
            {
                clsSearch.ClassStatus = clsGlobalVariables.STATUS_REPLACEMENT;
                clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_REPLACEMENT_DESC;

                clsSearch.ClassOperation = clsGlobalVariables.STATUS_REPLACED;
                clsSearch.ClassOperationDescription = clsGlobalVariables.STATUS_REPLACED_DESC;

                sSIMID = txtRepSIMID.Text;
                sSIMSN = txtRepSIMSN.Text;
            }
            else if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_PULLOUT_DESC) && cboSearchActionMade.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS))
            {
                clsSearch.ClassStatus = clsGlobalVariables.STATUS_PULLED_OUT;
                clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_PULLED_OUT_DESC;

                clsSearch.ClassOperation = clsGlobalVariables.STATUS_PULLEDOUT;
                clsSearch.ClassOperationDescription = clsGlobalVariables.STATUS_PULLEDOUT_DESC;

                sSIMID = txtCurSIMID.Text;
                sSIMSN = txtCurSIMSN.Text;
            }

            sSQL = "";
            sRowSQL = "";
            sRowSQL = " ('" + dbFunction.CheckAndSetNumericValue(sSIMID) + "', " +
            sRowSQL + sRowSQL + " '" + sSIMSN + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtMerchantID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + txtMerchantName.Text + "', " +
            sRowSQL + sRowSQL + " '" + clsSearch.ClassStatus + "', " +
            sRowSQL + sRowSQL + " '" + clsSearch.ClassStatusDescription + "', " +
            sRowSQL + sRowSQL + " '" + clsSearch.ClassOperation + "', " +
            sRowSQL + sRowSQL + " '" + clsSearch.ClassOperationDescription + "', " +
            sRowSQL + sRowSQL + " '" + sFSRDate + "', " +
            sRowSQL + sRowSQL + " '" + sMReceiptTime + "', " +
            sRowSQL + sRowSQL + " '" + txtMMerchRemarks.Text + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassProcessedBy + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassProcessedDateTime + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassModifiedBy + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassModifiedDateTime + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtServiceNo.Text) + "', " +
            sRowSQL + sRowSQL + " '" + txtSearchIRNo.Text + "') ";
            sSQL = sSQL + sRowSQL;

            Debug.WriteLine("sSQL=" + sSQL);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "SIM Activity", sSQL, "InsertCollectionDetail");

            Cursor.Current = Cursors.Default;

        }

        private void EditableTextBox()
        {
            if ((txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC)) ||
               (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC)) ||
               (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_SERVICING_DESC)) ||
               (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC)))
            {
                txtMerchantName.ReadOnly = txtMerchantAddress.ReadOnly = false;
                txtMerchantName.BackColor = txtMerchantAddress.BackColor = clsFunction.EntryBackColor;
            }

        }

        private void panel13_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnRemoveCurSIM_Click(object sender, EventArgs e)
        {
            txtCurSIMID.Text =
                txtCurSIMSN.Text =
                txtCurSIMCarrier.Text =
                txtCurSIMLocation.Text = clsFunction.sNull;

            cboSearchCurSIMStatus.Text = clsFunction.sDefaultSelect;
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

            cboSearchRepTerminalStatus.Text = clsFunction.sDefaultSelect;
        }

        private void btnRemoveRepSIM_Click(object sender, EventArgs e)
        {
            txtRepSIMID.Text =
                txtRepSIMSN.Text =
                txtRepSIMCarrier.Text =
                txtRepSIMLocation.Text = clsFunction.sNull;

            cboSearchRepSIMStatus.Text = clsFunction.sDefaultSelect;
        }

        private void btnPreviewFSR_Click(object sender, EventArgs e)
        {   
            Cursor.Current = Cursors.WaitCursor;

            if (!dbFunction.fPromptConfirmation("Are you sure to preview FSR report?")) return;

            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtSearchFSRNo.Text))
            {
                if (isFTPFile())
                {
                    // update signature
                    dbAPI.updateSignature(int.Parse(txtSearchServiceNo.Text), int.Parse(txtSearchFSRNo.Text));

                    // download signature
                    dbFunction.downloadSignature(clsDefines.MERCHANT_SIGNATURE_INDEX, int.Parse(txtSearchServiceNo.Text));
                    dbFunction.downloadSignature(clsDefines.VENDOR_SIGNATURE_INDEX, int.Parse(txtSearchServiceNo.Text));
                }

                // Preview report
                clsSearch.ClassComponents = dbAPI.getStockkMovementDetail("Stock Movement Detail List", txtSearchServiceNo.Text + clsDefines.gPipe + txtIRIDNo.Text);
                clsSearch.ClassIsExportToPDF = false;                
                dbReportFunc.ViewFSR(5);
            }
            else
            {
                dbFunction.SetMessageBox("Service not yet completed. Umable to process report.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }

            Cursor.Current = Cursors.Default;
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

            clsSearch.ClassAdvanceSearchValue = txtServiceJobTypeDescription.Text + clsFunction.sCaret + txtServiceScheduleDate.Text + clsFunction.sCaret + txtServiceRequestNo.Text + clsFunction.sCaret + txtSearchIRNo.Text + clsFunction.sCaret + txtServiceReferenceNo.Text + clsFunction.sCaret + txtMMerchRemarks.Text + clsFunction.sCaret + txtRMInstruction.Text + clsFunction.sCaret +
                                                txtMAppVesion.Text + clsFunction.sCaret + txtMAppCRC.Text + clsFunction.sCaret +
                                                txtClientName.Text + clsFunction.sCaret + txtClientAddress.Text + clsFunction.sCaret + txtClientContactPerson.Text + clsFunction.sCaret + txtClientMobileNo.Text + "/" + txtClientTelNo.Text + clsFunction.sCaret +
                                                txtMerchantName.Text + clsFunction.sCaret + txtMerchantAddress.Text + clsFunction.sCaret + txtIRTID.Text + clsFunction.sCaret + txtIRMID.Text + clsFunction.sCaret + txtMerchantContactPerson.Text + clsFunction.sCaret + txtMerchantMobileNo.Text + "/" + txtMerchantTelNo.Text + clsFunction.sCaret +
                                                txtFEName.Text + clsFunction.sCaret + txtFEMobileNo.Text + clsFunction.sCaret + txtFEEmail.Text + clsFunction.sCaret +
                                                txtCurTerminalSN.Text + clsFunction.sCaret + txtCurTerminalType.Text + clsFunction.sCaret + txtCurTerminalModel.Text + clsFunction.sCaret + txtCurTerminalBrand.Text + clsFunction.sCaret +
                                                txtRepTerminalSN.Text + clsFunction.sCaret + txtRepTerminalType.Text + clsFunction.sCaret + txtRepTerminalModel.Text + clsFunction.sCaret + txtRepTerminalBrand.Text + clsFunction.sCaret +
                                                txtCurSIMSN.Text + clsFunction.sCaret + txtCurSIMCarrier.Text + clsFunction.sCaret +
                                                txtRepSIMSN.Text + clsFunction.sCaret + txtRepSIMCarrier.Text + clsFunction.sCaret +
                                                clsUser.ClassProcessedBy + clsFunction.sCaret + clsUser.ClassProcessedDateTime + clsFunction.sCaret + clsUser.ClassProcessedContactNo + clsFunction.sCaret + clsUser.ClassProcessedEmail + clsFunction.sCaret +
                                                cboSearchActionMade.Text + clsFunction.sCaret + txtFSRRequestNo.Text + clsFunction.sCaret +
                                                dteMFSRDate.Text + clsFunction.sCaret + dteMTimeArrived.Text + clsFunction.sCaret + dteMReceiptTime.Text + clsFunction.sCaret + dteMTimeStart.Text + clsFunction.sCaret + dteMTimeEnd.Text + clsFunction.sCaret +
                                                txtMProblemReported.Text + clsFunction.sCaret + txtMActionTaken.Text + clsFunction.sCaret + txtMActualProblemReported.Text + clsFunction.sCaret + txtMAnyComments.Text + clsFunction.sCaret +
                                                txtMMerchRep.Text + clsFunction.sCaret + txtMMerchPosition.Text + clsFunction.sCaret + txtMMerchContactNo.Text + clsFunction.sCaret +
                                                txtReasonDesc.Text + clsFunction.sCaret +
                                                pPrefix;

            Debug.WriteLine("clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);
            dbAPI.ExecuteAPI("POST", "Notify", "FSR", clsSearch.ClassAdvanceSearchValue, "Email Notification", "", "EmailNotification");
        }

        private void SetCount()
        {
            Debug.WriteLine("--SetCount--");
            Debug.WriteLine("fEdit=" + fEdit);

            int iCount = 0;

            // Success
            iCount = clsTerminal.ClassTerminalCount = 0;
            clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtJobType.Text) + clsFunction.sPipe + clsGlobalVariables.ACTION_MADE_SUCCESS;
            dbAPI.GetViewCount("Search", "Action Made Counter", clsSearch.ClassAdvanceSearchValue, "Get Count");
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            txtSuccessCnt.Text = iCount.ToString();

            // Negative
            iCount = clsTerminal.ClassTerminalCount = 0;
            clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtJobType.Text) + clsFunction.sPipe + clsGlobalVariables.ACTION_MADE_NEGATIVE;
            dbAPI.GetViewCount("Search", "Action Made Counter", clsSearch.ClassAdvanceSearchValue, "Get Count");
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            txtNegativeCnt.Text = iCount.ToString();

            if (!fEdit)
            {
                // Attempt
                iCount = clsTerminal.ClassTerminalCount = 0;
                clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtJobType.Text);
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
        
        private void EditableServiceDateTime(bool isClear)
        {
            if (isClear)
            {
                dteMFSRDate.Enabled = dteMTimeArrived.Enabled = dteMReceiptTime.Enabled = dteMTimeStart.Enabled = dteMTimeEnd.Enabled = true;
            }
            else
            {
                dteMFSRDate.Enabled = dteMTimeArrived.Enabled = dteMReceiptTime.Enabled = dteMTimeStart.Enabled = dteMTimeEnd.Enabled = true;

                if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isUpdate, clsUser.ClassUserID, 26))
                {
                    dteMFSRDate.Enabled = dteMTimeArrived.Enabled = dteMReceiptTime.Enabled = dteMTimeStart.Enabled = dteMTimeEnd.Enabled = false;
                }
            }

        }
        
        private void btnResetEmail_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidDigitalFSRMode(txtSearchFSRDesc.Text, true)) return;

            //if (!dbAPI.isValidDiagnotic(txtSearchServiceNo.Text, txtSearchFSRNo.Text)) return;
            
            Cursor.Current = Cursors.WaitCursor;

            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtSearchFSRNo.Text))
            {
                if (!dbFunction.fPromptConfirmation("Are you sure to reset eFSR email?" + "\n\n" +
                    " > ServiceNo " + txtSearchServiceNo.Text + "\n" +
                    " > FSRNo " + txtSearchFSRNo.Text + "\n" +
                    " > Service Type " + txtServiceType1.Text + "\n" +
                    " > Merchant " + txtSearchMerchantName.Text)) return;
                
                dbAPI.ExecuteAPI("PUT", "Update", "Reset eFSR Generator", txtSearchServiceNo.Text + clsFunction.sPipe + txtSearchFSRNo.Text + clsFunction.sPipe + clsDefines.gZero, "", "", "UpdateCollectionDetail");

                startCountdown(clsDefines.FSR_GENERATE_TIMEOUT);

                //dbFunction.SetMessageBox("Reset email complete." + "\n" + "You may re-send email" + "\n" + "or advise vendor representative to send email on their end.", "Reset email notification", clsFunction.IconType.iInformation);
            }
            else
            {
                dbFunction.SetMessageBox("Unable to process. No selected service.", "Reset email notification", clsFunction.IconType.iError);
            }

            Cursor.Current = Cursors.Default;
        }
        
        private void panel139_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chkServiced_CheckedChanged(object sender, EventArgs e)
        {
            // ROCKY - FSR ENHANCEMENT: ADD CHECK BOX FOR SERVICED MERCHANTS
            if (!chkServiced.Checked)
            {
                if(cboSearchActionMade.SelectedItem == null)
                {
                    MessageBox.Show("Please Select Service Result");
                    chkServiced.Checked = true;
                    return;
                }
                else
                {
                    cboSearchActionMade.Text = dbAPI.GetActionMade()[1]; // SUCCESS
                    cboSearchActionMade.Enabled = false;
                    cboSearchCurTerminalStatus.Enabled = cboCurTerminalLocation.Enabled = false;
                    cboSearchCurSIMStatus.Enabled = cboCurSIMLocation.Enabled = false;
                    cboSearchCurTerminalStatus.SelectedIndex = 1;
                    cboSearchCurSIMStatus.SelectedIndex = 1;
                }
            }
            else
            {
                cboSearchActionMade.Enabled = true;
                cboSearchCurTerminalStatus.Enabled = cboCurTerminalLocation.Enabled = true;
                cboSearchCurSIMStatus.Enabled = cboCurSIMLocation.Enabled = true;
                cboSearchActionMade.SelectedItem = null;
                cboSearchCurTerminalStatus.SelectedItem = null;
                cboSearchCurSIMStatus.SelectedItem = null;

            }
        }

        private void btnViewImages_Click(object sender, EventArgs e)
        {
            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtSearchFSRNo.Text))
            {
                clsSearch.ClassMerchantName = txtMerchantName.Text;
                clsSearch.ClassTID = txtIRTID.Text;
                clsSearch.ClassMID = txtIRMID.Text;
                clsSearch.ClassServiceNo = int.Parse(txtSearchServiceNo.Text);
                clsSearch.ClassFSRNo = int.Parse(txtSearchFSRNo.Text);
                
                // merchand representative
                clsParticular.ClassParticularName = dbFunction.CheckAndSetStringValue(txtMMerchRep.Text);
                clsParticular.ClassPosition = dbFunction.CheckAndSetStringValue(txtMMerchPosition.Text);
                clsParticular.ClassContactNumber = dbFunction.CheckAndSetStringValue(txtMMerchContactNo.Text);
                clsParticular.ClassEmail = dbFunction.CheckAndSetStringValue(txtMMerchEmail.Text);

                // vendor representative
                clsVendor.ClassParticularName = dbFunction.CheckAndSetStringValue(txtFEName.Text);
                clsVendor.ClassPosition = dbFunction.CheckAndSetStringValue(txtFEPosition.Text);
                clsVendor.ClassContactNumber = dbFunction.CheckAndSetStringValue(txtFEMobileNo.Text);
                clsVendor.ClassEmail = dbFunction.CheckAndSetStringValue(txtFEEmail.Text);

                // SN               
                if (dbFunction.isValidID(txtRepTerminalID.Text))
                    clsSearch.ClassTerminalSN = txtRepTerminalSN.Text;
                else
                    clsSearch.ClassTerminalSN = txtCurTerminalSN.Text;

                if (dbFunction.isValidID(txtRepSIMID.Text))
                    clsSearch.ClassSIMSerialNo = txtRepSIMSN.Text;
                else
                    clsSearch.ClassSIMSerialNo = txtCurSIMSN.Text;


                clsSearch.ClassJobTypeDescription = txtServiceJobTypeDescription.Text;
                
                frmImageTaken frm = new frmImageTaken();
                frm.ShowDialog();
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

        private void btnCancelJO_Click(object sender, EventArgs e)
        {
            bool isConfirm = false;

            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

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
                    if ((txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC)) ||
                        (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC)) ||
                        (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_PULLOUT_DESC)))
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

                if (MessageBox.Show("Are you sure to cancel " + txtServiceType1.Text + " service" + " for \n" + txtMerchantName.Text + "." +
                    "\n\n" +
                    "Warning:\nData will permanently deleted.", "Confirm?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    isConfirm = false;
                else
                    isConfirm = true;

                if (isConfirm)
                {
                    Cursor.Current = Cursors.WaitCursor;

                    if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC) ||
                        txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_PULLOUT_DESC))
                    {
                        clsSearch.ClassAdvanceSearchValue = txtJobTypeDescription.Text + clsFunction.sPipe +
                                                    txtSearchServiceNo.Text + clsFunction.sPipe +
                                                    txtIRIDNo.Text + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtClientID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtCurTerminalID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtCurSIMID.Text);
                    }
                    else if (txtJobTypeDescription.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
                    {
                        clsSearch.ClassAdvanceSearchValue = txtJobTypeDescription.Text + clsFunction.sPipe +
                                                    txtSearchServiceNo.Text + clsFunction.sPipe +
                                                    txtIRIDNo.Text + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtClientID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtRepTerminalID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtRepSIMID.Text);
                    }
                    else
                    {
                        clsSearch.ClassAdvanceSearchValue = txtJobTypeDescription.Text + clsFunction.sPipe +
                                                    txtSearchServiceNo.Text + clsFunction.sPipe +
                                                    txtIRIDNo.Text + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtClientID.Text) + clsFunction.sPipe +
                                                    clsFunction.sZero + clsFunction.sPipe +
                                                    clsFunction.sZero;
                    }

                    Debug.WriteLine("Cancel Service->Value=" + clsSearch.ClassAdvanceSearchValue);
                    dbAPI.ExecuteAPI("PUT", "Update", "Cancel Service", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                    Cursor.Current = Cursors.Default;

                    dbFunction.SetMessageBox("Service cancelled complete.", "Job order cancelled", clsFunction.IconType.iInformation);

                    btnMClear_Click(this, e);
                }
            }
        }

        private void frmTerminalFSR_Activated(object sender, EventArgs e)
        {
            btnClear.Focus();
        }

        private void btnRefreshSN_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            cboCurSIMLocation.Text = cboCurTerminalLocation.Text = cboSearchCurTerminalStatus.Text = cboSearchCurSIMStatus.Text = cboSearchCurDockStatus.Text = clsFunction.sDefaultSelect;
            cboRepTerminalLocation.Text = cboRepSIMLocation.Text = cboSearchRepTerminalStatus.Text = cboSearchRepSIMStatus.Text = cboSearchRepDockStatus.Text = clsFunction.sDefaultSelect;
            
            FillServicingSNInfo();
            FiillFSRInfo();

            cboSearchActionMade_SelectedIndexChanged(this, e);

            // current terminal
            if (dbFunction.isValidID(txtCurTerminalID.Text))
            {
                PopulateTerminalTextBox(txtCurTerminalID.Text, txtCurTerminalSN.Text, true);
                cboCurTerminalLocation.Text = txtCurTerminalLocation.Text;
            }

            // current sim
            if (dbFunction.isValidID(txtCurSIMID.Text))
            {
                PopulateSIMTextBox(txtCurSIMID.Text, txtCurSIMSN.Text, true);
                cboCurSIMLocation.Text = txtCurSIMLocation.Text;
            }

            // replace terminal
            if (dbFunction.isValidID(txtRepTerminalID.Text))
            {
                PopulateTerminalTextBox(txtRepTerminalID.Text, txtRepTerminalSN.Text, false);
                cboRepTerminalLocation.Text = txtRepTerminalLocation.Text;
            }

            // replace sim
            if (dbFunction.isValidID(txtRepSIMID.Text))
            {
                PopulateSIMTextBox(txtRepSIMID.Text, txtRepSIMSN.Text, false);
                cboRepSIMLocation.Text = txtRepSIMLocation.Text;
            }

            btnClear.Focus();

            Cursor.Current = Cursors.Default;
        }

        private void btnResendEmail_Click(object sender, EventArgs e)
        {
            string pFileName = "";

            if (!dbFunction.isValidDescriptionEntry(txtClientCode.Text, "Client code" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            if (!dbFunction.fPromptConfirmation("Are you sure to send FSR and Diagnostic Report?")) return;

            if (!dbAPI.isValidDiagnotic(txtSearchServiceNo.Text, txtSearchFSRNo.Text)) return;

            if (!isFTPFile()) return;

            Cursor.Current = Cursors.WaitCursor;
            
            if (dbFunction.isValidID(txtSearchFSRNo.Text) && dbFunction.isValidID(txtSearchServiceNo.Text))
            {
                pFileName = txtSearchServiceNo.Text + clsDefines.FSR_FILENAME_PREFIX + clsDefines.FILE_EXT_PDF;
                if (!dbAPI.isFileExist("Search", "Check Attach File", pFileName))
                {
                    dbFunction.SetMessageBox("FSR report not yet ready.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return;
                }

                pFileName = txtSearchServiceNo.Text + clsDefines.DIAGNOSTIC_FILENAME_PREFIX + clsDefines.FILE_EXT_PDF;
                if (!dbAPI.isFileExist("Search", "Check Attach File", pFileName))
                {
                    dbFunction.SetMessageBox("Diagnostic report not yet ready.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return;
                }

                //// check if pdf and diagnostic complete
                //if (!dbAPI.isRecordExist("Search", "eFSR Email/Diagnostic", txtSearchServiceNo.Text + clsDefines.gPipe + txtSearchFSRNo.Text))
                //{
                //    dbFunction.SetMessageBox("Email not yet ready. Please try again.", "Resend email notification", clsFunction.IconType.iWarning);
                //    return;
                //}

                if (!dbFunction.fPromptConfirmation("Send FSR & Diagnostic report." + "\n\n" +
                        " > Bank " + txtClientName.Text + "\n" +
                        " > ServiceNo " + txtSearchServiceNo.Text + "\n" +
                        " > FSRNo " + txtSearchFSRNo.Text + "\n" +
                        " > Service Type " + txtServiceType1.Text + "\n" +
                        " > Merchant " + txtSearchMerchantName.Text +
                        "\n\n" +
                        "Are you sure to send email to merchant?")) return;


                clsSearch.ClassAdvanceSearchValue = txtSearchFSRNo.Text + clsDefines.gCaret +
                                                    txtSearchServiceNo.Text + clsDefines.gCaret +
                                                    txtMerchantName.Text + clsDefines.gCaret +
                                                    txtFEName.Text + clsDefines.gCaret +
                                                    txtFEEmail.Text + clsDefines.gCaret +
                                                    txtMMerchRep.Text + clsDefines.gCaret +
                                                    txtMMerchEmail.Text + clsDefines.gCaret +
                                                    txtServiceType1.Text + clsDefines.gCaret +
                                                    cboSearchActionMade.Text + clsDefines.gCaret +
                                                    txtIRTID.Text + clsDefines.gCaret +
                                                    txtIRMID.Text + clsDefines.gCaret +
                                                    txtClientCode.Text + clsDefines.gCaret +
                                                    txtReasonDesc.Text + clsDefines.gCaret +
                                                    txtDispatcher.Text + clsDefines.gCaret +
                                                    txtFSRDateTime.Text;

                dbAPI.ExecuteAPI("POST", "Notify", "Mobile eFSR", clsSearch.ClassAdvanceSearchValue, "Email Notification", "", "EmailNotification");

                dbFunction.SetMessageBox("Send email complete.", "Resend email", clsFunction.IconType.iInformation);
            }
            else
            {
                dbFunction.SetMessageBox("Unable to process. No selected service.", "Resend email notification", clsFunction.IconType.iError);
            }

            Cursor.Current = Cursors.Default;

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

        private void btnUseMerchLoc_Click(object sender, EventArgs e)
        {
            txtGeoLatitude.Text =
            txtGeoLongitude.Text =
            txtGeoCountry.Text =
            txtGeoLocality.Text =
            txtGeoAddress.Text = clsFunction.sDash;

            if (btnUseMerchLoc.Text.Equals("USE MERCH LOCATION"))
            {   
                txtGeoLatitude.Text = clsDefines.DEFAULT_GEO_LATITUDE;
                txtGeoLongitude.Text = clsDefines.DEFAULT_GEO_LONGITUDE;
                txtGeoCountry.Text = clsDefines.DEFAULT_GEO_COUNTRY;
                txtGeoLocality.Text = txtMerchantCity.Text;
                txtGeoAddress.Text = txtMerchantAddress.Text;
                btnUseMerchLoc.Text = "REVERT MERCH LOCATION";
            }
            else
            {   
                getServiceGeoLocation();
                btnUseMerchLoc.Text = "USE MERCH LOCATION";

            }
        }

        private void btnUpdateMerchLoc_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtSearchFSRNo.Text))
            {
                if (!dbFunction.fPromptConfirmation("Are you sure to update geometric location?" + "\n\n" +
                    " > ServiceNo " + txtSearchServiceNo.Text + "\n" +
                    " > FSRNo " + txtSearchFSRNo.Text + "\n" +
                    " > Service Type " + txtServiceType1.Text + "\n" +
                    " > Merchant " + txtSearchMerchantName.Text + "\n" +
                    " > Latitude " + txtGeoLatitude.Text + "\n" +
                    " > Longitude " + txtGeoLongitude.Text)) return;

                dbAPI.ExecuteAPI("PUT", "Update", "Update GeoLocation", 
                    txtSearchFSRNo.Text + clsFunction.sPipe + 
                    txtSearchServiceNo.Text + clsFunction.sPipe +
                    txtGeoLatitude.Text + clsFunction.sPipe +
                    txtGeoLongitude.Text + clsFunction.sPipe +
                    txtGeoCountry.Text + clsFunction.sPipe +
                    txtGeoLocality.Text + clsFunction.sPipe +
                    txtGeoAddress.Text, "", "", "UpdateCollectionDetail");
                
                dbFunction.SetMessageBox("Geolocation update complete.", "Manual FSR", clsFunction.IconType.iInformation);
            }
            else
            {
                dbFunction.SetMessageBox("Unable to process. No selected service.", "Manual FSR", clsFunction.IconType.iError);
            }

            Cursor.Current = Cursors.Default;
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

                btnPreviewSvcHistory.Enabled = true;

            }
        }

        private void setFSRMode()
        {
            if (dbFunction.isValidID(txtSearchFSRNo.Text))
                txtSearchFSRDesc.Text = (dbFunction.isValidID(txtMobileID.Text) ? clsDefines.DIGITAL_FSR : clsDefines.MANUAL_FSR);
            else
                txtSearchFSRDesc.Text = clsFunction.sDash;
        }

        private void btnCreateDiagnostic_Click(object sender, EventArgs e)
        {
            frmDiagnostic frm = new frmDiagnostic();
            frm.Show();
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

        private void btnSendFSREmail_Click(object sender, EventArgs e)
        {
            string sFileName = "";
            ftp ftpClient = null;

            Cursor.Current = Cursors.WaitCursor;

            if (!dbFunction.isValidDescriptionEntry(txtClientCode.Text, "Client code" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            if (!dbFunction.fPromptConfirmation("Are you sure to send FSR report?")) return;

            if (dbFunction.isValidID(txtSearchFSRNo.Text) && dbFunction.isValidID(txtSearchServiceNo.Text))
            {
                // -----------------------------------------------------------------------------------------------------------------------
                // prepare fsr report
                // -----------------------------------------------------------------------------------------------------------------------
                // update signature
                dbAPI.updateSignature(int.Parse(txtSearchServiceNo.Text), int.Parse(txtSearchFSRNo.Text));

                // download signature
                dbFunction.downloadSignature(clsDefines.MERCHANT_SIGNATURE_INDEX, int.Parse(txtSearchServiceNo.Text));
                dbFunction.downloadSignature(clsDefines.VENDOR_SIGNATURE_INDEX, int.Parse(txtSearchServiceNo.Text));
                
                clsSearch.ClassServiceRequestID = txtSearchServiceNo.Text + "_fsr";
                clsSearch.ClassIsExportToPDF = true;
                dbFunction.eFSRReport(5);

                // Upload File to FTP      
                sFileName = txtSearchServiceNo.Text + "_fsr.pdf";
                ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                ftpClient.delete(clsGlobalVariables.strFTPRemoteFSRPath + sFileName);
                ftpClient.upload(clsGlobalVariables.strFTPRemoteFSRPath + sFileName, @clsGlobalVariables.strFTPLocalExportPath + sFileName);
                ftpClient.disconnect(); // ftp disconnect

                // -----------------------------------------------------------------------------------------------------------------------
                // -----------------------------------------------------------------------------------------------------------------------

                if (!dbFunction.fPromptConfirmation("Send FSR report." + "\n\n" +
                        " > ServiceNo " + txtSearchServiceNo.Text + "\n" +
                        " > FSRNo " + txtSearchFSRNo.Text + "\n" +
                        " > Service Type " + txtServiceType1.Text + "\n" +
                        " > Merchant " + txtSearchMerchantName.Text +
                        "\n\n" +
                        "Are you sure to send email to merchant?")) return;


                clsSearch.ClassAdvanceSearchValue = txtSearchFSRNo.Text + clsDefines.gCaret +
                                                    txtSearchServiceNo.Text + clsDefines.gCaret +
                                                    txtMerchantName.Text + clsDefines.gCaret +
                                                    txtFEName.Text + clsDefines.gCaret +
                                                    txtFEEmail.Text + clsDefines.gCaret +
                                                    txtMMerchRep.Text + clsDefines.gCaret +
                                                    txtMMerchEmail.Text + clsDefines.gCaret +
                                                    txtServiceType1.Text + clsDefines.gCaret +
                                                    cboSearchActionMade.Text + clsDefines.gCaret +
                                                    txtIRTID.Text + clsDefines.gCaret +
                                                    txtIRMID.Text + clsDefines.gCaret +
                                                    txtClientCode.Text + clsDefines.gCaret +
                                                    txtReasonDesc.Text + clsDefines.gCaret +
                                                    txtDispatcher.Text + clsDefines.gCaret +
                                                    txtFSRDateTime.Text;

                dbAPI.ExecuteAPI("POST", "Notify", "Mobile eFSR", clsSearch.ClassAdvanceSearchValue, "Email Notification", "", "EmailNotification");

                dbFunction.SetMessageBox("Send email complete.", "Resend email", clsFunction.IconType.iInformation);
            }
            else
            {
                dbFunction.SetMessageBox("Unable to process. No selected service.", "Resend email notification", clsFunction.IconType.iError);
            }

            Cursor.Current = Cursors.Default;
        }

        private bool isFTPFile()
        {
            bool isValid = false;
            string pSignature1 = "";
            string pSignature2 = "";
            long pFileSize1 = 0;
            long pFileSize2 = 0;
            
            pSignature1 = dbFunction.CheckAndSetNumericValue(txtSearchServiceNo.Text) + "_05.png"; // Merchant signature
            pSignature2 = dbFunction.CheckAndSetNumericValue(txtSearchServiceNo.Text) + "_06.png"; // Vendor signature
            
            ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
            string pRemotePath = $"{clsGlobalVariables.strFTPRemoteImagesPath}{clsSearch.ClassBankCode}/";
            pFileSize1 = ftpClient.getFileSize(pRemotePath + pSignature1);
            pFileSize2 = ftpClient.getFileSize(pRemotePath + pSignature2);

            Debug.WriteLine("pSignature1=" + pSignature1 + ",pFileSize1=" + pFileSize1);
            Debug.WriteLine("pSignature2=" + pSignature1 + ",pFileSize2=" + pFileSize2);
            
            if (pFileSize1 > 0 && pFileSize2 > 0)
                isValid = true;

            if (!isValid)
            {
                dbFunction.SetMessageBox("Merchant or vendor signature not found.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);
            }

            return isValid;
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
                        dbFunction.CheckAndSetNumericValue(txtJobType.Text) + clsDefines.gPipe +
                        dbFunction.CheckAndSetStringValue(txtJobTypeDescription.Text) + clsDefines.gPipe +
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
                " > Name: " + txtMMerchRep.Text + "\n" +
                " > Position: " + txtMMerchPosition.Text + "\n" +
                " > Contact No.: " + txtMMerchContactNo.Text + "\n" +
                " > Email: " + txtMMerchEmail.Text +
                "\n\n" +
                "Are you sure to continue update?"
                )) return;



                dbAPI.ExecuteAPI("PUT", "Update", "Merchant Representative Info",
                    txtSearchServiceNo.Text + clsDefines.gPipe +
                    txtMerchantID.Text + clsDefines.gPipe +
                    txtMMerchRep.Text + clsDefines.gPipe +
                    txtMMerchPosition.Text + clsDefines.gPipe +
                    txtMMerchContactNo.Text + clsDefines.gPipe +
                    txtMMerchEmail.Text
                    , "", "", "UpdateCollectionDetail");

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
            if (txtServiceRequestDate.Text != null)
            {
                DateTime requestDate = DateTime.Parse(txtServiceRequestDate.Text);
                DateTime slaDueDate = requestDate.AddDays(int.Parse(txtSLA.Text));

                txtDueDate.Text = slaDueDate.ToString("MM-dd-yyyy");
            }

        }

        private void setMainTab()
        {
            tabMain.SelectedIndex = 0;
        }

        private void checkCancelJOButton()
        {
            btnCancelJO.Enabled = true;
            if (txtServiceJobTypeStatusDesc.Text.Equals(clsDefines.SERVICE_STATUS_COMPLETED))
                btnCancelJO.Enabled = false;
        }

        private void loadStockMovementDetail(ListView lvw, bool isCurrent)
        {
            dbFunction.ClearListViewItems(lvw);
            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtIRIDNo.Text))
            {
                dbAPI.FillListViewStockMovementDetail(lvw, "Stock Movement Detail List", txtSearchServiceNo.Text + clsDefines.gPipe + txtIRIDNo.Text, isCurrent);
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
                cboItemLocation.Text = dbFunction.GetSearchValue("LOCATION");
                cboItemStatus.Text = dbFunction.GetSearchValue("STATUS");

            }
        }

        private void btnItemApply_Click(object sender, EventArgs e)
        {
            // confirmation
            if (dbFunction.isValidCount(lvwStockDetail.Items.Count) ||
                dbFunction.isValidCount(lvwRepStockDetail.Items.Count))
            {
                if (!dbFunction.fPromptConfirmation("Additional components " + dbFunction.AddBracketStartEnd(tabComponent.SelectedTab.Text) +
                    "\n\n" +
                    "Are you sure to update the following" +
                    "\n\n" +
                    "> Location:" + cboItemLocation.Text +
                    "\n" +
                    "> Status: " + cboItemStatus.Text))
                {
                    return;
                }

            }

            switch (tabComponent.SelectedIndex)
            {
                case 0: // Current
                    dbFunction.updateListView(lvwStockDetail, 7, cboItemLocation.Text, false); // location
                    dbFunction.updateListView(lvwStockDetail, 8, cboItemStatus.Text, false); // status
                    break;
                case 1: // Replaced
                    dbFunction.updateListView(lvwRepStockDetail, 7, cboItemLocation.Text, false); // location
                    dbFunction.updateListView(lvwRepStockDetail, 8, cboItemStatus.Text, false); // status
                    break;                    
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

        private void btnOverride_Click(object sender, EventArgs e)
        {
            // Admin Login requirement
            if (!dbAPI.isPromptAdminLogIn()) return;

            cboSearchActionMade.Enabled = true;

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

        // Override the OnLoad method to include custom logic
        protected override void OnLoad(EventArgs e)
        {   
            base.OnLoad(e);

            // Your custom logic that should run during the Load event
            //MessageBox.Show("Form2 Load event triggered!");
        }

        private void btnRefreshService_Click(object sender, EventArgs e)
        {
            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtIRIDNo.Text))
            {
                fAutoLoadData = true;
                clsSearch.ClassServiceNo = int.Parse(dbFunction.CheckAndSetNumericValue(txtSearchServiceNo.Text));
                clsSearch.ClassFSRNo = int.Parse(dbFunction.CheckAndSetNumericValue(txtSearchFSRNo.Text));
                clsSearch.ClassParticularName = txtMerchantName.Text;
                clsSearch.ClassClientID = int.Parse(dbFunction.CheckAndSetNumericValue(txtClientID.Text));
                clsSearch.ClassMerchantID = int.Parse(dbFunction.CheckAndSetNumericValue(txtMerchantID.Text));
                clsSearch.ClassFEID = int.Parse(dbFunction.CheckAndSetNumericValue(txtFEID.Text));
                clsSearch.ClassTID = txtIRTID.Text;
                clsSearch.ClassMID = txtIRMID.Text;
                clsSearch.ClassIRIDNo = int.Parse(dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text));
                clsSearch.ClassIRNo = txtSearchIRNo.Text;

                btnFSRSearch_Click(this, e);

            }
            else
            {
                dbFunction.SetMessageBox("No service selected.\n\nUnable to refresh service.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
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

        private void lvwRepStockDetail_DoubleClick(object sender, EventArgs e)
        {
            string pSelectedRow = dbFunction.GetListViewSelectedRow(lvwRepStockDetail, 0);
            Debug.WriteLine("pSelectedRow=\n" + pSelectedRow);

            if (lvwRepStockDetail.Items.Count > 0)
            {
                txtLineNo.Text = dbFunction.GetSearchValue("LINE#");
                txtItemID.Text = dbFunction.GetSearchValue("ID");
                txtItemSN.Text = dbFunction.GetSearchValue("SERIAL NO.");
                cboItemLocation.Text = dbFunction.GetSearchValue("LOCATION");
                cboItemStatus.Text = dbFunction.GetSearchValue("STATUS");

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

        private void btnSearchDispatcher_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantID, txtMerchantID.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iClientID, txtClientID.Text)) return;

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
                
            }
        }

        private void btnRemoveDispatcher_Click(object sender, EventArgs e)
        {
            txtDispatchID.Text = 
            txtDispatcher.Text = 
            txtDispatcherMobileNo.Text = 
            txtDispatcherEmail.Text = clsFunction.sNull;
        }

        private void entryTextBox(bool isEnable)
        {
            txtVendor.ReadOnly = txtRequestor.ReadOnly = (isEnable ? false : true);
            txtVendor.Enabled = txtRequestor.Enabled = (isEnable ? true : false);

            txtVendor.BackColor = txtRequestor.BackColor = (isEnable ? clsFunction.EntryBackColor : clsFunction.DisableBackColor);

        }

        private void iniComboBoxSelection(bool isEnable)
        {
            cboSearchActionMade.Text = cboDependency.Text = cboStatusReason.Text = 
            cboCurTerminalLocation.Text = cboSearchCurTerminalStatus.Text = cboCurSIMLocation.Text = cboSearchCurSIMStatus.Text =
            cboRepTerminalLocation.Text = cboSearchRepTerminalStatus.Text = cboRepSIMLocation.Text = cboSearchRepSIMStatus.Text = clsFunction.sDefaultSelect;

            cboSearchActionMade.Enabled = cboDependency.Enabled = cboStatusReason.Enabled =
            cboCurTerminalLocation.Enabled = cboSearchCurTerminalStatus.Enabled = cboCurSIMLocation.Enabled = cboSearchCurSIMStatus.Enabled =
            cboRepTerminalLocation.Enabled = cboSearchRepTerminalStatus.Enabled = cboRepSIMLocation.Enabled = cboSearchRepSIMStatus.Enabled = isEnable;
        }

        private string getGeneralActionTaken()
        {
            string pOutput = "";
            
            pOutput = "Date: " + dteMFSRDate.Value.ToString("MM-dd-yyyy") + Environment.NewLine +
                      "AT: " + dteMTimeArrived.Value.ToString("hh:mm tt") + Environment.NewLine +
                      "ST: " + dteMTimeStart.Value.ToString("hh:mm tt") + Environment.NewLine +
                      "ET: " + dteMTimeEnd.Value.ToString("hh:mm tt") + Environment.NewLine +
                      "Assigned Engineer: " + txtFEName.Text + Environment.NewLine +
                      "Work Log: " + txtMActionTaken.Text + Environment.NewLine +
                      "TID: " + txtIRTID.Text + Environment.NewLine +
                      "MID: " + txtIRMID.Text + Environment.NewLine +
                      "PULLOUT: " + txtMMerchRemarks.Text + Environment.NewLine +
                      "Received by: " + txtMMerchRep.Text;

            return pOutput;
        }

        public void startCountdown(int seconds)
        {
            if (seconds <= 0)
            {   
                return;
            }

            remainingSeconds = seconds;
            timer = new System.Timers.Timer(1000); // 1-second interval
            timer.Elapsed += TimerTick;
            timer.AutoReset = true;
            timer.Start();
            
            lblCountDown.Invoke((MethodInvoker)delegate
            {
                lblCountDown.Visible = true;
                btnPreviewFSR.Enabled = btnViewDiagnostic.Enabled = btnSendFSRAndDiagEmail.Enabled = false;
            });


            lblCountDown.Text = $"{remainingSeconds}";
        }

        private void TimerTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (remainingSeconds > 0)
            {
                remainingSeconds--;

                lblCountDown.Invoke((MethodInvoker)delegate
                {
                    lblCountDown.Text = $"{remainingSeconds}";
                });
            }
            else
            {
                timer.Stop();
                timer.Dispose();
                
                lblCountDown.Invoke((MethodInvoker)delegate
                {
                    lblCountDown.Text = "0";
                    lblCountDown.Visible = false;
                    btnPreviewFSR.Enabled = btnViewDiagnostic.Enabled = btnSendFSRAndDiagEmail.Enabled = true;
                });
            }
        }

        private void btnUpdateReason_Click(object sender, EventArgs e)
        {
            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtReasonID.Text))
            {
                if (!dbFunction.fPromptConfirmation("Are you sure to update service reason?")) return;

                dbAPI.ExecuteAPI("PUT", "Update", "Update Servicing Reason", $"{txtSearchServiceNo.Text}{clsDefines.gPipe}{txtRequestID.Text}{clsDefines.gPipe}{txtReasonID.Text}{clsDefines.gPipe}{txtReasonDesc.Text}", "", "", "UpdateCollectionDetail");

                dbFunction.SetMessageBox("Service reason updated.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
                
            }
        }

        private void btnUpdateServiceDate_Click(object sender, EventArgs e)
        {
            string pSearchValue = "";

            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isUpdate, clsUser.ClassUserID, 26)) return;

            if (!dbFunction.isValidComboBoxValue(cboSearchActionMade.Text))
            {
                dbFunction.SetMessageBox("Please choose a value for service result.", "Warning", clsFunction.IconType.iExclamation);
                return;
            }

            if (dbFunction.isValidID(txtSearchFSRNo.Text) && dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtMerchantID.Text) && dbFunction.isValidID(txtIRIDNo.Text))
            {
                if (!dbFunction.CheckTimeFromTo(dteMTimeStart, dteMTimeEnd, "Time Start/End", true)) return;

                if (!dbFunction.CheckTimeFromTo(dteMTimeArrived, dteMReceiptTime, "Time Arrived/Receipt", false)) return;

                if (!dbFunction.isValidEntry(clsFunction.CheckType.iActionTaken, txtMActionTaken.Text)) return;

                // Admin Login requirement
                if (!dbAPI.isPromptAdminLogIn()) return;

                // Update
                if (!dbFunction.fPromptConfirmation("Manual FSR date/time/entry update information:" +
                    "\n\n" +
                    " > Job Type: " + txtJobTypeDescription.Text + "\n" +
                    " > FSR Mode: " + txtSearchFSRDesc.Text + "\n" +
                    " > Service Result: " + cboSearchActionMade.Text + "\n\n" +
                    " > Service Status: " + txtServiceJobTypeStatusDesc.Text + "\n\n" +
                    " > Merchant: " + txtMerchantName.Text + "\n" +
                    " > TID: " + txtIRTID.Text + "\n" +
                    " > MID: " + txtIRMID.Text + "\n\n" +
                    " > Serviced Date: " + dteMFSRDate.Text + "\n" +
                    " > Time Arrived: " + dteMTimeArrived.Text + "\n" +
                    " > Receipt Time: " + dteMReceiptTime.Text + "\n" +
                    " > Time Start: " + dteMTimeStart.Text + "\n" +
                    " > Time End: " + dteMTimeEnd.Text + "\n\n" +
                    " > Update In Report: " + dbFunction.setBooleanToYesNo(chkIncludeInReport.Checked) + "\n" +
                    " > Billable: " + dbFunction.setBooleanToYesNo(chkBillable.Checked) + "\n" +
                    "\n\n" +
                    "Are you sure to continue update?"
                    )) return;

                string pDateTime = dbFunction.getCurrentDateTime();
                string pDate = dbFunction.getCurrentDate();
                string pTime = dbFunction.getCurrentTime();
                string pFSRDate = dbFunction.CheckAndSetDatePickerValueToDate(dteMFSRDate);
                string pTimeArrived = dbFunction.CheckAndSetDatePickerValueToTime(dteMTimeArrived);
                string pReceiptTime = dbFunction.CheckAndSetDatePickerValueToTime(dteMReceiptTime);
                string pTimeStart = dbFunction.CheckAndSetDatePickerValueToTime(dteMTimeStart);
                string pTimeEnd = dbFunction.CheckAndSetDatePickerValueToTime(dteMTimeEnd);

                pSearchValue = $"{txtFSRNo.Text}{clsDefines.gPipe}{txtSearchServiceNo.Text}{clsDefines.gPipe}{txtIRIDNo.Text}{clsDefines.gPipe}" +
                                    $"{pDate}{clsDefines.gPipe}{pTime}{clsDefines.gPipe}" +
                                    $"{pFSRDate}{clsDefines.gPipe}" +
                                    $"{pTimeArrived}{clsDefines.gPipe}{pReceiptTime}{clsDefines.gPipe}" +
                                    $"{pTimeStart}{clsDefines.gPipe}{pTimeEnd}{clsDefines.gPipe}" +
                                    $"{StrClean(txtMProblemReported.Text)}{clsDefines.gPipe}" +
                                    $"{StrClean(txtMActualProblemReported.Text)}{clsDefines.gPipe}" +
                                    $"{StrClean(txtMActionTaken.Text)}{clsDefines.gPipe}" +
                                    $"{StrClean(txtMAnyComments.Text)}{clsDefines.gPipe}" +
                                    $"{StrClean(txtMMerchRemarks.Text)}{clsDefines.gPipe}" +
                                    $"{dbFunction.CheckAndSetBooleanValue(chkIncludeInReport.Checked)}{clsDefines.gPipe}" +
                                    $"{dbFunction.CheckAndSetBooleanValue(chkBillable.Checked)}{clsDefines.gPipe}" +
                                    $"{cboSearchActionMade.Text}";

                dbFunction.parseDelimitedString(pSearchValue, clsDefines.gPipe, 0);

                // update
                dbAPI.ExecuteAPI("PUT", "Update", "Serviced Date-Time", pSearchValue, "", "", "UpdateCollectionDetail");
                
                dbFunction.SetMessageBox("Manual FSR date/time/entry information updated.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);

            }
            else
            {
                dbFunction.SetMessageBox("Service information must not be blank.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);
            }            
        }
        
        private void btnIncludeInReport_Click(object sender, EventArgs e)
        {
            bool isConfirm = false;

            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtSearchFSRNo.Text))
            {
                // Admin Login requirement
                if (!dbAPI.isPromptAdminLogIn()) return;

                if (MessageBox.Show($"Are you sure you want to {(chkIncludeInReport.Checked ? "include" : "exclude")} " +
                                     $"{txtServiceType1.Text} service for {txtMerchantName.Text} in report?\n\n" +
                                     "Info:\nThis record can still be modified later.",
                                     "Confirm?",
                                     MessageBoxButtons.YesNo,
                                     MessageBoxIcon.Question) == DialogResult.No)
                {
                    isConfirm = false;
                }
                else
                {
                    isConfirm = true;
                }

                if (isConfirm)
                {

                    clsSearch.ClassAdvanceSearchValue = $"{txtClientID.Text}{clsDefines.gPipe}{txtSearchServiceNo.Text}{clsDefines.gPipe}{txtIRIDNo.Text}{clsDefines.gPipe}" + dbFunction.CheckAndSetBooleanValue(chkIncludeInReport.Checked);

                    dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 0);

                    // update isReport
                    dbAPI.ExecuteAPI("PUT", "Update", "Servicing-FSR Include In Report", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                    dbFunction.SetMessageBox($"{(chkIncludeInReport.Checked ? "Include" : "Exclude")} in report update complete.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);

                    btnMClear_Click(this, e);

                }
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

        private void FilldHelpdeskInfo(int pAssisNo, int pProblemNo, HelpDeskController data)
        {
            Debug.WriteLine("--FilldHelpdeskInfo--");

            if (pProblemNo > 0)
            {
                ucHelpDeskServiceInfo.loadData(data);
                ucHelpDeskEntryInfo.loadData(data);
                ucVendorHelpDeskRepInfo.loadData(data);
                ucVendorHelpDeskTeamLeadInfo.loadData(data);
                
            }
        }

        private void btnUpdateAppsInfo_Click(object sender, EventArgs e)
        {
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isUpdate, clsUser.ClassUserID, 26)) return;

            if (dbFunction.isValidID(txtSearchServiceNo.Text) && dbFunction.isValidID(txtIRIDNo.Text))
            {
                if (!dbFunction.fPromptConfirmation("Service application information:" +
                "\n\n" +
                " > Merchant: " + txtMerchantName.Text + "\n" +
                " > TID: " + txtIRTID.Text + "\n" +
                " > MID: " + txtIRMID.Text + "\n" +
                " > Request ID: " + txtRequestID.Text + "\n\n" +
                " > Version: " + txtMAppVesion.Text + "\n" +
                " > CRC: " + txtMAppCRC.Text +
                "\n\n" +
                "Are you sure to continue update?"
                )) return;

                dbAPI.ExecuteAPI("PUT", "Update", "Service Apps Info", $"{txtSearchServiceNo.Text}{clsFunction.sPipe}{txtIRIDNo.Text}{clsFunction.sPipe}{txtMAppVesion.Text}{clsFunction.sPipe}{txtMAppCRC.Text}", "", "", "UpdateCollectionDetail");

                dbFunction.SetMessageBox("Service application info updated.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
            }
        }
    }
}