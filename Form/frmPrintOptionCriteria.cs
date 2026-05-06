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
    public partial class frmPrintOptionCriteria : Form
    {        
        public static string sServiceTypeDetails = "";
        public static string sMerchantDetails = "";
        public static string sTerminalDetails = "";
        public static string sDateDetails = "";
        public static string sFSRDetails = "";
        public static string sSearchOptionDetails;
        public static string sHeader = "";
        public static bool fSelected = false;

        private clsAPI dbAPI;
        private clsReportFunc dbReportFunc;
        private clsFunction dbFunction;
        private clsFile dbFile;
        
        // Defaut Value
        public static string sDefaultServiceType;
        public static string sDefaultTerminalStatus;
        public static string sDefaultSIMStatus;
        public static string sDefaultIRStatus;

        public static SearchType iSearchType;

        public static DateTime stDateFrom;
        public static string sDateFrom = "";
        public static DateTime stDateTo;
        public static string sDateTo = "";

        // Detail
        public static DateTime stDetailDateFrom;
        public static string sDetailDateFrom = "";
        public static DateTime stDetailDateTo;
        public static string sDetailDateTo = "";

        public static int iReportType;

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

        public frmPrintOptionCriteria()
        {
            InitializeComponent();
        }

        public enum SearchType
        {
            iTerminal, iSIM, iMerchant, iReason, iFSRAttempt, iIR, iFSR, iTA
        }
        
        private void btnSearchClear_Click(object sender, EventArgs e)
        {
            dbFunction.ClearTextBox(this);           
            dbFunction.TextBoxUnLock(false, this);

            InitDateRange();
            
            InitCheckBox(false);
            
            DefaultSelectedComboBoxValue();

            fSelected = false;

            ResetID();

            rbAll.Checked = rbToday.Checked = rbRange.Checked = false;
            rbDetailAll.Checked = rbDetailToday.Checked = rbDetailRange.Checked = false;

            initDateFilter(false);

            dteDateFrom.Enabled = dteDateTo.Enabled = false;
            dteDetailDateFrom.Enabled = dteDetailDateTo.Enabled = false;

        }

        private void ResetID()
        {         
            clsSearch.ClassAdvanceSearchValue = clsFunction.sNull;
            clsSearch.ClassClientID = clsSearch.ClassMerchantID = clsSearch.ClassFEID = clsSearch.ClassRegionID = clsSearch.ClassProvinceID = clsSearch.ClassServiceTypeID = clsSearch.ClassServiceStatus = 0;
            clsSearch.ClassTerminalTypeID = clsSearch.ClassTerminalModelID = clsSearch.ClassTerminalStatus = clsSearch.ClassTerminalStatus = clsSearch.ClassSIMStatus = clsSearch.ClassSIMID = 0;
            clsSearch.ClassTerminalSN = clsSearch.ClassSIMSerialNo = clsFunction.sNull;
            clsSearch.ClassFSRStatus = clsSearch.ClassIRIDNo = clsSearch.ClassJobType = clsSearch.ClassRegionType = clsSearch.ClassRegionID = 0;
            clsSearch.ClassIRIDNo = clsSearch.ClassStatus = 0;
            clsSearch.ClassIRNo = clsFunction.sNull;

            clsSearch.ClassMobileID = clsFunction.iZero;
            clsSearch.ClassMobileTerminalID = clsSearch.ClassMobileTerminalName = clsSearch.ClassMobileAssignedTo = clsFunction.sNull;

            clsSearch.ClassDispatcherID = 0;
            clsSearch.ClassDispatcherName = clsFunction.sNull;

            clsSearch.ClassIncludeSummaryTab = 0;
            clsSearch.ClassIncludeDetailTab = 0;

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            sDefaultServiceType = "";
            sDefaultTerminalStatus = "";
            sDefaultSIMStatus = "";
            sDefaultIRStatus = "";

            this.Close();
        }

        private void txtSearchTID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSearchMID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSearchTerminalSN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }    

        private void InitDateRange()
        {
            dteDateFrom.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteDateFrom, clsFunction.sDateDefaultFormat);
           
            dteDateTo.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteDateTo, clsFunction.sDateDefaultFormat);

            if (!chkDetailTab.Checked)
            {
                dteDetailDateFrom.Value = DateTime.Now.Date;
                dbFunction.SetDateFormat(dteDetailDateFrom, clsFunction.sDateDefaultFormat);

                dteDetailDateTo.Value = DateTime.Now.Date;
                dbFunction.SetDateFormat(dteDetailDateTo, clsFunction.sDateDefaultFormat);

            }
        }
        
        
        private void txtSearchInvoiceNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSearchBatchNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSearchSIMSerialNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnSearchTerminalSN_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iTerminal;
            frmSearchField.iStatus = 0;
            frmSearchField.sHeader = "VIEW TERMINAL";
            frmSearchField.isCheckBoxes = false;
            frmSearchField.iLocationID = clsFunction.iZero;
            frmSearchField.sLocation = clsFunction.sDefaultSelect;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsTerminal.ClassTerminalID = clsSearch.ClassTerminalID;
                clsTerminal.ClassTerminalSN = clsSearch.ClassTerminalSN;

                txtSearchTerminalID.Text = clsTerminal.ClassTerminalID.ToString();
                txtSearchTerminalSN.Text = clsTerminal.ClassTerminalSN;
            }
        }

        private void btnSearchSIMSN_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iSIM;
            frmSearchField.iStatus = 0;
            frmSearchField.sHeader = "VIEW SIM";
            frmSearchField.isCheckBoxes = false;
            frmSearchField.iLocationID = clsFunction.iZero;
            frmSearchField.sLocation = clsFunction.sDefaultSelect;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsTerminal.ClassSIMID = clsSearch.ClassSIMID;
                clsTerminal.ClassSIMSerialNo = clsSearch.ClassSIMSerialNo;

                txtSearchSIMID.Text = clsTerminal.ClassSIMID.ToString();
                txtSearchSIMSerialNo.Text = clsTerminal.ClassSIMSerialNo;
            }
        }
        
        private bool CheckDateFromTo(DateTimePicker objFrom, DateTimePicker objTo, int iIndex)
        {
            bool fValid = true;
            int iResult;

            iResult = DateTime.Compare(DateTime.Parse(objFrom.Value.ToShortDateString()), DateTime.Parse(objTo.Value.ToShortDateString()));

            if (iResult > 0)
                fValid = false;

            if (!fValid)
            {
                switch (iIndex)
                {
                    case 0: // Req Date
                        MessageBox.Show("[Date From] shouldn't be greater than [Date To]" +
                                        "\n\n" +
                                        "Date From: " + objFrom.Value.ToString("MM-dd-yyyy") +
                                        "\n"+
                                        "Date To:      " + objTo.Value.ToString("MM-dd-yyyy"), "Request Date", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;
                    case 1: // Inst Date
                        MessageBox.Show("[Date From] shouldn't be greater than [Date To]" +
                                        "\n\n" +
                                        "Date From: " + objFrom.Value.ToString("MM-dd-yyyy") +
                                        "\n" +
                                        "Date To:      " + objTo.Value.ToString("MM-dd-yyyy"), "Installation Date", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;
                    case 2: // TA Date
                        MessageBox.Show("[Date From] shouldn't be greater than [Date To]" +
                                        "\n\n" +
                                        "Date From: " + objFrom.Value.ToString("MM-dd-yyyy") +
                                        "\n" +
                                        "Date To:      " + objTo.Value.ToString("MM-dd-yyyy"), "TA Date", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;
                    case 3: // FSR Date
                        MessageBox.Show("[Date From] shouldn't be greater than [Date To]" +
                                        "\n\n" +
                                        "Date From: " + objFrom.Value.ToString("MM-dd-yyyy") +
                                        "\n" +
                                        "Date To:      " + objTo.Value.ToString("MM-dd-yyyy"), "FSR Date", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;
                    case 4: // IR Import Date
                        MessageBox.Show("[Date From] shouldn't be greater than [Date To]" +
                                        "\n\n" +
                                        "Date From: " + objFrom.Value.ToString("MM-dd-yyyy") +
                                        "\n" +
                                        "Date To:      " + objTo.Value.ToString("MM-dd-yyyy"), "IR Import Date", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;

                    case 5: // Created Date
                        MessageBox.Show("[Date From] shouldn't be greater than [Date To]" +
                                        "\n\n" +
                                        "Date From: " + objFrom.Value.ToString("MM-dd-yyyy") +
                                        "\n" +
                                        "Date To:      " + objTo.Value.ToString("MM-dd-yyyy"), "IR Import Date", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;
                    case 6: // Date Range
                        MessageBox.Show("[Date From] shouldn't be greater than [Date To]" +
                                        "\n\n" +
                                        "Date From: " + objFrom.Value.ToString("MM-dd-yyyy") +
                                        "\n" +
                                        "Date To:      " + objTo.Value.ToString("MM-dd-yyyy"), "Date Range", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;

                }
                
            }

            return fValid;
        }

        private void btnSearchMerchant_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iMerchant;
            frmSearchField.sHeader = "MERCHANT";
            frmSearchField.sSearchChar = txtSearchMerchant.Text;
            frmSearchField.isCheckBoxes = false;
            frmSearchField.isPreview = true;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsParticular.ClassParticularID = clsSearch.ClassMerchantID = clsSearch.ClassParticularID;
                clsParticular.ClassParticularName = clsSearch.ClassMerchantName = clsSearch.ClassParticularName;

                txtClientID.Text = clsSearch.ClassClientID.ToString();
                txtMerchantID.Text = clsSearch.ClassParticularID.ToString();             
                txtSearchMerchant.Text = clsParticular.ClassParticularName;
                txtSearchTID.Text = clsSearch.ClassTID;
                txtSearchMID.Text = clsSearch.ClassMID;

                txtIRIDNo.Text = clsSearch.ClassIRIDNo.ToString();
                txtIRNo.Text = clsSearch.ClassIRNo;

                //// get client info
                //cboSearchClient.Text = clsFunction.sDefaultSelect;
                //if (dbFunction.isValidID(txtClientID.Text))
                //{
                //    dbAPI.ExecuteAPI("GET", "Search", "Client Info", txtClientID.Text, "Get Info Detail", "", "GetInfoDetail");
                //    if (dbAPI.isNoRecordFound() == false)
                //    {
                //        cboSearchClient.Text = clsSearch.ClassClientName = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                //    }
                //}
            }
        }       

        private void DefaultSelectedComboBoxValue()
        {           
            cboSearchServiceType.Text = clsFunction.sDefaultSelect;
     
            cboSearchClient.Text = clsFunction.sDefaultSelect;
            cboSearchSP.Text = clsFunction.sDefaultSelect;            
            cboSearchType.Text = clsFunction.sDefaultSelect;
            cboSearchModel.Text = clsFunction.sDefaultSelect;
            cboSearchBrand.Text = clsFunction.sDefaultSelect;
           
            cboSearchActionMade.Text = clsFunction.sDefaultSelect;
            cboSearchBillable.Text = clsFunction.sDefaultSelect;
            cboSearchRegion.Text = clsFunction.sDefaultSelect;
            cboSearchProvince.Text = clsFunction.sDefaultSelect;
          
            cboSearchTerminalAssetType.Text = clsFunction.sDefaultSelect;
           
            cboSearchTerminalAssetType.Text = clsFunction.sDefaultSelect;
            cboSearchLocation.Text = clsFunction.sDefaultSelect;
            cboStatus.Text = clsFunction.sDefaultSelect;
            cboSearchServiceStatus.Text = clsFunction.sDefaultSelect;

            cboFSRMode.Text = clsFunction.sDefaultSelect;

            cboSearchTerminalStatus.Text = clsFunction.sDefaultSelect;

            cboSearchReportStatus.Text = clsFunction.sDefaultSelect;
        }

           
        private void PopulateComboBoxFSRStatus(ComboBox obj)
        {
            int i = 0;            

            obj.Items.Clear();
            dbAPI.GetFSRStatusList();

            while (dbAPI.GetFSRStatus().Length > i)
            {
                obj.Items.Add(dbAPI.GetFSRStatus()[i]);
                i++;               
            }            
        }

        private void InitCheckBox(bool fEnable)
        {
            chkInstDate.Checked = fEnable;
            chkTADate.Checked = fEnable;
            chkFSRDate.Checked = fEnable;
            chkReqImportDate.Checked = fEnable;
            chkReqDate.Checked = fEnable;
            chkReleasedDate.Checked = fEnable;
            chkPullout.Checked = fEnable;
            chkMobile.Checked = fEnable;
        }

        private void InitSesrchTextBox(bool fReadOnly)
        {
            txtSearchMerchant.ReadOnly = fReadOnly;
            txtSearchTerminalSN.ReadOnly = fReadOnly;
            txtSearchSIMSerialNo.ReadOnly = fReadOnly;
            txtSearchSetup.ReadOnly = fReadOnly;
            
        }

        private void txtSearchMerchant_Click(object sender, EventArgs e)
        {
            //btnSearchMerchant_Click(this, e);
        }

        private void txtSearchTerminalSN_Click(object sender, EventArgs e)
        {
            //btnSearchTerminalSN_Click(this, e);
        }

        private void txtSearchSIMSerialNo_Click(object sender, EventArgs e)
        {
            //btnSearchSIMSN_Click(this, e);
        }

        private void txtSearchIRNo_Click(object sender, EventArgs e)
        {
            //btnSearchIRNo_Click(this, e);
        }

        private void txtSearchMerchant_KeyPress(object sender, KeyPressEventArgs e)
        {
                
        }

        private void LoadSearchForm(EventArgs e)
        {
            switch (iSearchType)
            {
                case SearchType.iMerchant:
                    txtSearchMerchant_Click(this, e);
                    break;
                case SearchType.iIR:
                    txtSearchIRNo_Click(this, e);
                    break;
                case SearchType.iTerminal:
                    txtSearchTerminalSN_Click(this, e);
                    break;
                case SearchType.iSIM:
                    txtSearchSIMSerialNo_Click(this, e);
                    break;
            }
        }

        private void txtSearchIRNo_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void btnSearchFE_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iFE;
            frmSearchField.sHeader = "FIELD ENGINEER";
            frmSearchField.sSearchChar = txtSearchFE.Text;
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {   
                txtFEID.Text = clsSearch.ClassParticularID.ToString();            
                txtSearchFE.Text = clsSearch.ClassFEName = clsSearch.ClassParticularName;               
            }
        }

        private void InitGroupBox(SearchType searchType)
        {
            
            gbParticular.Enabled = false;
            gbRegion.Enabled = false;
            gbServiceType.Enabled = false;
            gbServiceStatus.Enabled = false;
            gbMerchant.Enabled = false;
            gbTerminal.Enabled = false;
            gbSIM.Enabled = false;
          
            gbServiceResult.Enabled = false;

            gbDateFlag.Enabled = false;
            gbDateFilter.Enabled = false;
            gbDetailDateFilter.Enabled = false;

            gbInventoryStatus.Enabled = false;

            switch (searchType)
            {
                case SearchType.iIR:
                    gbParticular.Enabled = true;
                    gbRegion.Enabled = true;
                    gbServiceType.Enabled = true;
                
                    break;
                case SearchType.iFSR:
                    gbParticular.Enabled = true;
                    gbRegion.Enabled = true;
                    gbServiceType.Enabled = true;
                    gbMerchant.Enabled = true;
                    gbTerminal.Enabled = true;
                    gbSIM.Enabled = true;
                    gbServiceResult.Enabled = true;
                    break;
                case SearchType.iTA:
                    gbParticular.Enabled = true;
                    gbRegion.Enabled = true;
                    gbMerchant.Enabled = true;
                    gbTerminal.Enabled = true;
                    gbSIM.Enabled = true;
                 
                    break;
                default:
                    gbParticular.Enabled = true;
                    gbRegion.Enabled = true;
                    gbServiceType.Enabled = true;
                    gbMerchant.Enabled = true;
                    gbTerminal.Enabled = true;
                    gbSIM.Enabled = true;
                 
                    gbServiceResult.Enabled = true;

                    gbDateFlag.Enabled = true;
                    gbDateFilter.Enabled = true;
                    gbDetailDateFilter.Enabled = true;

                    gbInventoryStatus.Enabled = true;
                    break;
            }

            // Set forecolor
            gbParticular.ForeColor = Color.Blue;
            gbRegion.ForeColor = Color.Blue;
            gbServiceType.ForeColor = Color.Blue;
            gbMerchant.ForeColor = Color.Blue;
            gbTerminal.ForeColor = Color.Blue;
            gbSIM.ForeColor = Color.Blue;
          
            gbServiceResult.ForeColor = Color.Blue;
            gbDateFlag.ForeColor = Color.Blue;
            gbStatus.ForeColor = Color.Blue;
            gbLocation.ForeColor = Color.Blue;
            gbDataFilter.ForeColor = Color.Blue;

            gbInventoryStatus.ForeColor = Color.Blue;
        }

        private void InitLabel()
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is Label)
                        if (control.Name.CompareTo("lblHeader") == 0)
                            (control as Label).ForeColor = Color.White;
                        else if (control.Name.CompareTo("lblReportDescription") == 0)
                            (control as Label).ForeColor = Color.Yellow;
                        else
                            (control as Label).ForeColor = Color.Black;
                    else
                        func(control.Controls);
            };

            func(Controls);
        }

        private void InitCheckBoxLabel()
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is CheckBox)
                        (control as CheckBox).ForeColor = Color.Black;
                    else
                        func(control.Controls);
            };

            func(Controls);
        }
        
        private void cboSearchClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassClientID = 0;
            if (!cboSearchClient.Text.Equals(clsFunction.sDefaultSelect))
            {                
                dbFunction.GetIDFromFile("Client List", cboSearchClient.Text);
                clsSearch.ClassClientID = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassClientID="+ clsSearch.ClassClientID);
            }

            txtClientID.Text = clsSearch.ClassClientID.ToString();
        }

        private void cboSearchRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassRegionType = 0;
            if (!cboSearchRegion.Text.Equals(clsFunction.sDefaultSelect))
            {                
                dbFunction.GetIDFromFile("Region", cboSearchRegion.Text);
                clsSearch.ClassRegionType = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassRegionType=" + clsSearch.ClassRegionType);
                txtRegionType.Text = clsSearch.ClassRegionType.ToString();

                clsSearch._isWriteResponse = true;
                dbAPI.FillComboBoxRegionDetail(cboSearchProvince, "View", "RegionDetail", clsSearch.ClassRegionType.ToString(), "Region"); // Load RegionDetail (Province)
                clsSearch._isWriteResponse = false;
            }
            txtRegionType.Text = clsSearch.ClassRegionType.ToString();
        }

        private void cboSearchSP_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassServiceProviderID = 0;
            if (!cboSearchSP.Text.Equals(clsFunction.sDefaultSelect))
            {                
                dbFunction.GetIDFromFile("SP List", cboSearchSP.Text);
                clsSearch.ClassServiceProviderID = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassServiceProviderID=" + clsSearch.ClassServiceProviderID);
            }

            txtSPID.Text = clsSearch.ClassServiceProviderID.ToString();
        }

        private void cboSearchServiceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassServiceTypeID = 0;
            clsSearch.ClassJobType = 0;
            if (!cboSearchServiceType.Text.Equals(clsFunction.sDefaultSelect))
            {
                // Get Info
                dbAPI.ExecuteAPI("GET", "Search", "Service Type Info", cboSearchServiceType.Text, "Get Info Detail", "", "GetInfoDetail");

                if (dbAPI.isNoRecordFound() == false)
                {
                    clsSearch.ClassServiceTypeID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0));                   
                    clsSearch.ClassJobType = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5));
                    
                }
            }

            txtServiceTypeID.Text = clsSearch.ClassServiceTypeID.ToString();
            txtJobType.Text = clsSearch.ClassJobType.ToString();

            chkPullout.Checked  = true;          
            if (cboSearchServiceType.Text.Equals(clsGlobalVariables.STATUS_PULLED_OUT_DESC))
            {
                chkPullout.Checked = false;
            }
        }

        

        private void cboSearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassTerminalTypeID = 0;
            if (!cboSearchType.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Terminal Type", cboSearchType.Text);
                clsSearch.ClassTerminalTypeID = clsSearch.ClassOutFileID;


                dbAPI.FillComboBoxTerminalModelByTerminalType(cboSearchModel, clsSearch.ClassTerminalTypeID.ToString());
            }
        }

        private void cboSearchModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassTerminalModelID = 0;
            if (!cboSearchModel.Text.Equals(clsFunction.sDefaultSelect))
            {                
                dbFunction.GetIDFromFile("Terminal Model", cboSearchModel.Text);
                clsSearch.ClassTerminalModelID = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassTerminalModelID=" + clsSearch.ClassTerminalModelID);
            }
        }

        private void cboSearchBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassTerminalBrandID = 0;
            if (!cboSearchBrand.Text.Equals(clsFunction.sDefaultSelect))
            {                
                dbFunction.GetIDFromFile("Terminal Brand", cboSearchBrand.Text);
                clsSearch.ClassTerminalBrandID = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassTerminalBrandID=" + clsSearch.ClassTerminalBrandID);
            }
        }
       

        private void cboSearchProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassRegionID = 0;
            if (!cboSearchProvince.Text.Equals(clsFunction.sDefaultSelect))
            {                
                dbFunction.GetIDFromFile("RegionDetail", cboSearchProvince.Text);
                clsSearch.ClassRegionID = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassRegionID=" + clsSearch.ClassRegionID);
                txtRegionID.Text = clsSearch.ClassRegionID.ToString();
            }
        }

        private void frmFindCriteria_FormClosing(object sender, FormClosingEventArgs e)
        {
            btnPrint.Focus();
        }

        private void frmFindCriteria_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    fSelected = false;
                    this.Close();
                    break;
            }
        }

        private void frmFindCriteria_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("--frmFindCriteria_Load--");

            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            dbFile = new clsFile();
            dbReportFunc = new clsReportFunc();

            //lblHeader.Text = lblHeader.Text + " " + "[ " + sHeader + " ]";
            
            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);
            dbFunction.TextBoxUnLock(false, this);

            InitDateRange();
            
            InitCheckBox(false);
            InitSesrchTextBox(true);

            clsParticular.ClassParticularID = 0;
            clsTerminal.ClassSIMID = 0;

            clsSearch._isWriteResponse = true;
            dbAPI.FillComboBoxServiceType(cboSearchServiceType);
            
            dbAPI.FillComboBoxClient(cboSearchClient);
            //dbAPI.FillComboBoxSP(cboSearchSP);
            dbAPI.FillComboBoxTerminalType(cboSearchType);
            //dbAPI.FillComboBoxTerminalModel(cboSearchModel);
            dbAPI.FillComboBoxTerminalBrand(cboSearchBrand);
          
            dbAPI.FillComboBoxCarrier(cboSearchSIMCarrier);
          
            dbAPI.FillComboBoxActionMade(cboSearchActionMade); // Load Action Made
            dbAPI.FillComboBoxBillable(cboSearchBillable); // Load Action Made
            dbAPI.FillComboBoxRegion(cboSearchRegion, "View", "Region", "", "Region"); // Load Region
            dbAPI.FillComboBoxServiceStatus(cboSearchServiceStatus); // Load Service Status
            
            dbAPI.FillComboBoxAssetType(cboSearchTerminalAssetType);
            dbAPI.FillComboBoxLocation(cboSearchLocation);

            dbAPI.FillComboBoxFSRMode(cboFSRMode);

            dbAPI.FillComboBoxTerminalStatus(cboSearchTerminalStatus);

            dbAPI.FillComboBoxReportStatus(cboSearchReportStatus);

            DefaultSelectedComboBoxValue();

            clsSearch._isWriteResponse = false;

            //InitDefaultCriteriaValue();
            InitGroupBox(iSearchType);
            InitLabel();
            InitCheckBoxLabel();

            fSelected = false;

            dbFunction.ClearListViewItems(lvwList);
            LoadReport("View", "Report Type", iReportType + clsFunction.sPipe);

            SetOption();

            rbAll.Checked = true;
            rbToday.Checked = false;
            rbRange.Checked = false;
            initDateFilter(false);

            Cursor.Current = Cursors.Default;
        }

        private void frmFindCriteria_Activated(object sender, EventArgs e)
        {
            
        }
        
        
        private void LoadReport(string StatementType, string SearchBy, string SearchValue)
        {
            int inLineNo = 0;
            int i = 0;

            lvwList.Items.Clear();
            dbAPI.ExecuteAPI("GET", StatementType, SearchBy, SearchValue, "Report", "", "ViewReport");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (clsReport.ClassRecordFound)
            {
                while (clsArray.ReportID.Length > i)
                {
                    inLineNo++;
                    ListViewItem item = new ListViewItem(inLineNo.ToString());
                    item.SubItems.Add(clsArray.ReportID[i]);
                    item.SubItems.Add(clsArray.ReportDesc[i]);
                    lvwList.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(lvwList);
            }
        }

        private void lvwList_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtReportID.Text = "";

            if (lvwList.SelectedItems.Count > 0)
            {
                string LineNo = lvwList.SelectedItems[0].Text;

                if (LineNo.Length > 0)
                {
                    // Clear
                    dbFunction = new clsFunction();

                    dbFunction.ClearTextBox(this);
                    clsSearch.ClassReportID = 0;
                    clsSearch.ClassDepartmentID = 0;
                    clsSearch.ClassParticularID = 0;
                    clsSearch.ClassClientID = 0;
                    clsSearch.ClassMerchantID = 0;
                    clsSearch.ClassDepartment = clsFunction.sDash;
                    clsSearch.ClassDateFrom = clsSearch.ClassDetailDateFrom = clsFunction.sDateDefault;
                    clsSearch.ClassDateTo = clsSearch.ClassDetailDateTo = clsFunction.sDateDefault;

                    string ReportID = lvwList.SelectedItems[0].SubItems[1].Text;
                    string ReportDescription = lvwList.SelectedItems[0].SubItems[2].Text;

                    clsSearch.ClassReportID = clsReport.ClassReportID = int.Parse(ReportID);
                    clsReport.ClassReportDesc = clsSearch.ClassReportType = ReportDescription;
                    
                    txtReportID.Text = clsReport.ClassReportID.ToString();
                    lblReportDescription.Text = clsReport.ClassReportDesc + clsFunction.sPipe + clsReport.ClassReportID;

                }
            }

            SetOption();

            SetMKTextBoxBackColor();
            DefaultSelectedComboBoxValue();

            dbAPI.FillSNStatusList(cboStatus, "View", "Status By Report ID", clsSearch.ClassReportID.ToString());
        }

        private void ComposeSearchValue(int ReportID)
        {
            Debug.WriteLine("--ComposeSearchValue--");
            Debug.WriteLine("ReportID="+ReportID);

            switch (ReportID)
            {
                case 1: // Terminal Inventory Detail Report
                   
                    clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(clsSearch.ClassTerminalID.ToString()) + clsFunction.sPipe +
                                                        clsSearch.ClassTerminalSN + clsFunction.sPipe +
                                                        clsSearch.ClassTerminalTypeID + clsFunction.sPipe +
                                                        clsSearch.ClassTerminalModelID + clsFunction.sPipe +
                                                        clsSearch.ClassTerminalBrandID + clsFunction.sPipe +
                                                        clsSearch.ClassTerminalStatus + clsFunction.sPipe +
                                                        clsSearch.ClassLocation + clsFunction.sPipe +
                                                        clsSearch.ClassAssetType + clsFunction.sPipe +
                                                        clsSearch.ClassDateFrom + clsFunction.sPipe +
                                                        clsSearch.ClassDateTo + clsFunction.sPipe +
                                                        getDateFilter();
                    break;
                case 3: // SIM Inventory Detail Report
                    
                    clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(clsSearch.ClassSIMID.ToString()) + clsFunction.sPipe +
                                                        clsSearch.ClassSIMSerialNo + clsFunction.sPipe +
                                                        clsSearch.ClassSIMCarrier + clsFunction.sPipe +
                                                        clsSearch.ClassTerminalStatus + clsFunction.sPipe +
                                                        clsSearch.ClassLocation + clsFunction.sPipe +
                                                        clsSearch.ClassDateFrom + clsFunction.sPipe +
                                                        clsSearch.ClassDateTo + clsFunction.sPipe +
                                                        getDateFilter();
                    break;
                case 4: // FSR Servicing Report
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassServiceTypeID + clsFunction.sPipe +
                                             dbFunction.CheckAndSetNumericValue(clsSearch.ClassParticularID.ToString()) + clsFunction.sPipe +
                                             dbFunction.CheckAndSetNumericValue(clsSearch.ClassTerminalTypeID.ToString()) + clsFunction.sPipe +
                                             dbFunction.CheckAndSetNumericValue(clsSearch.ClassTerminalModelID.ToString()) + clsFunction.sPipe +
                                             dbFunction.CheckAndSetNumericValue(clsSearch.ClassTerminalBrandID.ToString()) + clsFunction.sPipe +
                                             clsSearch.ClassTerminalSN + clsFunction.sPipe +
                                             clsSearch.ClassIRNo + clsFunction.sPipe +
                                             dbFunction.CheckAndSetNumericValue(clsSearch.ClassClientID.ToString()) + clsFunction.sPipe +
                                             dbFunction.CheckAndSetNumericValue(clsSearch.ClassServiceProviderID.ToString()) + clsFunction.sPipe +
                                             dbFunction.CheckAndSetNumericValue(clsSearch.ClassFEID.ToString()) + clsFunction.sPipe +
                                             clsSearch.ClassTID + clsFunction.sPipe +
                                             clsSearch.ClassMID + clsFunction.sPipe +
                                             clsFunction.sZero + clsFunction.sPipe +
                                             clsFunction.sZero + clsFunction.sPipe +
                                             dbFunction.CheckAndSetNumericValue(clsSearch.ClassTerminalStatus.ToString()) + clsFunction.sPipe +
                                             dbFunction.CheckAndSetNumericValue(clsSearch.ClassFSRStatus.ToString()) + clsFunction.sPipe +
                                             dbFunction.CheckAndSetNumericValue(clsSearch.ClassServiceStatus.ToString()) + clsFunction.sPipe +
                                             clsSearch.ClassFSRDateFrom + clsFunction.sPipe +
                                             clsSearch.ClassFSRDateTo + clsFunction.sPipe +
                                             clsSearch.ClassActionMade + clsFunction.sPipe +
                                             clsSearch.ClassJobTypeDescription + clsFunction.sPipe +
                                             clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                             clsSearch.ClassTotalPage + clsFunction.sPipe +
                                             dbFunction.CheckAndSetNumericValue(clsSearch.ClassRegionID.ToString()) + clsFunction.sPipe + // Region
                                             dbFunction.CheckAndSetNumericValue(clsSearch.ClassProvinceID.ToString()) + clsFunction.sPipe +
                                             getDateFilter();  // Location
                    break;
                case 5: // FSR (Engineer) Report
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassIRNo + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(clsSearch.ClassServiceNo.ToString()) + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(clsSearch.ClassFSRNo.ToString()) + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(clsSearch.ClassIRIDNo.ToString());
                    break;
                case 8: // INSTALLATION REQUEST SUMMARY REPORT
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassClientID + clsFunction.sPipe +
                                                        clsSearch.ClassRegionType + clsFunction.sPipe +
                                                        clsSearch.ClassJobType + clsFunction.sPipe +
                                                        clsSearch.ClassTerminalTypeID + clsFunction.sPipe +
                                                        clsSearch.ClassTerminalModelID + clsFunction.sPipe +
                                                        clsSearch.ClassDateTo + clsFunction.sPipe +
                                                        clsSearch.ClassID + clsFunction.sPipe +
                                                        getDateFilter();
                    break;
                case 9: // INSTALLATION REQUEST DETAIL REPORT
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassClientID + clsFunction.sPipe +
                                                        clsSearch.ClassIRIDNo + clsFunction.sPipe +
                                                        clsSearch.ClassStatus + clsFunction.sPipe +
                                                        clsSearch.ClassDateFrom + clsFunction.sPipe +
                                                        clsSearch.ClassDateTo + clsFunction.sPipe +
                                                        clsSearch.ClassID + clsFunction.sPipe +
                                                        clsSearch.ClassRegionType + clsFunction.sPipe +
                                                        clsSearch.ClassRegionID + clsFunction.sPipe +
                                                        getDateFilter();
                    break;
                case 18: // FSR REPORT (CLIENT)
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassClientID + clsFunction.sPipe +
                                                        clsSearch.ClassMerchantID + clsFunction.sPipe +
                                                        clsSearch.ClassJobType + clsFunction.sPipe +
                                                        clsSearch.ClassRegionType + clsFunction.sPipe +
                                                        clsSearch.ClassRegionID + clsFunction.sPipe +
                                                        clsSearch.ClassIRIDNo + clsFunction.sPipe +
                                                        clsSearch.ClassActionMade + clsFunction.sPipe +
                                                        clsSearch.ClassDateFrom + clsFunction.sPipe +
                                                        clsSearch.ClassDateTo + clsFunction.sPipe +
                                                        clsSearch.ClassStatus + clsFunction.sPipe +
                                                        clsSearch.ClassTerminalSN + clsFunction.sPipe +
                                                        clsSearch.ClassSIMSerialNo + clsFunction.sPipe +
                                                        clsSearch.ClassIsPullOut + clsFunction.sPipe +
                                                        clsSearch.ClassIncludeBillable + clsFunction.sPipe +
                                                        clsSearch.ClassServiceStatusDesc + clsFunction.sPipe +
                                                        clsSearch.ClassServiceTypeDesc + clsFunction.sPipe +
                                                        clsSearch.ClassServiceResultDesc + clsFunction.sPipe +
                                                        getDateFilter();
                    break;

                case 11: // MERCHANT SERVICE HISTORY REPORT
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassClientID + clsFunction.sPipe + clsSearch.ClassIRIDNo;
                    break;
                

                case 19: // Terminal Inventory Summary Report
                    clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(clsSearch.ClassLocationID.ToString()) + clsFunction.sPipe +                                                 
                                                        clsSearch.ClassTerminalTypeID + clsFunction.sPipe +
                                                        clsSearch.ClassTerminalModelID + clsFunction.sPipe +
                                                        clsSearch.ClassClientID;
                    break;
                case 20: // SIM Inventory Summary Report
                    clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(clsSearch.ClassLocationID.ToString()) + clsFunction.sPipe +
                                                        clsSearch.ClassSIMCarrier;
                    break;

                case 23: // OPERATION FSR REPORT
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassClientID + clsFunction.sPipe +
                                                        clsSearch.ClassMerchantID + clsFunction.sPipe +
                                                        clsSearch.ClassJobType + clsFunction.sPipe +
                                                        clsSearch.ClassRegionType + clsFunction.sPipe +
                                                        clsSearch.ClassRegionID + clsFunction.sPipe +
                                                        clsSearch.ClassIRIDNo + clsFunction.sPipe +
                                                        clsSearch.ClassActionMade + clsFunction.sPipe +
                                                        clsSearch.ClassDateFrom + clsFunction.sPipe +
                                                        clsSearch.ClassDateTo + clsFunction.sPipe +
                                                        clsSearch.ClassStatus + clsFunction.sPipe +
                                                        clsSearch.ClassTerminalSN + clsFunction.sPipe +
                                                        clsSearch.ClassSIMSerialNo + clsFunction.sPipe +
                                                        clsSearch.ClassIsPullOut + clsFunction.sPipe +
                                                        clsSearch.ClassIncludeBillable + clsFunction.sPipe +
                                                        clsSearch.ClassMobileID + clsFunction.sPipe +
                                                        clsSearch.ClassMobileTerminalID + clsFunction.sPipe +
                                                        clsSearch.ClassIsMobile + clsFunction.sPipe +                                                       
                                                        clsSearch.ClassServiceStatusDesc + clsFunction.sPipe +
                                                        clsSearch.ClassServiceTypeDesc + clsFunction.sPipe +
                                                        clsSearch.ClassServiceResultDesc + clsFunction.sPipe +
                                                        getDateFilter();
                    break;

                case 25: // RELEASED TERMINAL SUMMARY REPORT
                    clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(clsSearch.ClassTerminalTypeID.ToString()) + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(clsSearch.ClassTerminalModelID.ToString()) + clsFunction.sPipe +
                                                        clsSearch.ClassDateFrom + clsFunction.sPipe +
                                                        clsSearch.ClassDateTo + clsFunction.sPipe +
                                                        getDateFilter();

                    break;
                case 26: // RELEASED TERMINAL DETAIL REPORT
                    clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(clsSearch.ClassTerminalTypeID.ToString()) + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(clsSearch.ClassTerminalModelID.ToString()) + clsFunction.sPipe +
                                                        clsSearch.ClassDateFrom + clsFunction.sPipe +
                                                        clsSearch.ClassDateTo + clsFunction.sPipe +
                                                        getDateFilter();

                    break;
                case 27: // RELEASED SIM DETAIL REPORT
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassSIMCarrier + clsFunction.sPipe +
                                                        clsSearch.ClassDateFrom + clsFunction.sPipe +
                                                        clsSearch.ClassDateTo + clsFunction.sPipe +
                                                        getDateFilter();
                    break;
                case 28: // RELEASED SIM SUMMARY REPORT
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassSIMCarrier + clsFunction.sPipe +                                                        
                                                        clsSearch.ClassDateFrom + clsFunction.sPipe +
                                                        clsSearch.ClassDateTo + clsFunction.sPipe +
                                                        getDateFilter();
                    break;
                case 32: // TERMINAL SUMMARY(TYPE/MODEL) INVENTORY REPORT
                    clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(clsSearch.ClassLocationID.ToString()) + clsFunction.sPipe +
                                                        clsSearch.ClassTerminalTypeID + clsFunction.sPipe +
                                                        clsSearch.ClassTerminalModelID + clsFunction.sPipe +
                                                        clsSearch.ClassClientID;
                    break;
                case 33: // SIM SUMMARY(TELCO) INVENTORY REPORT
                    clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(clsSearch.ClassLocationID.ToString()) + clsFunction.sPipe +
                                                        clsSearch.ClassSIMCarrier;
                    break;
                    
                case 42: // DIAGSNOTC (Engineer) Report
                    clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(clsSearch.ClassFSRNo.ToString()) + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(clsSearch.ClassServiceNo.ToString());
                    break;

                /*
                case 43: // SERVICE SUMMARY REPORT
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassServiceStatusDesc + clsFunction.sPipe + clsSearch.ClassJobType + clsFunction.sPipe +
                                                        clsSearch.ClassDateFrom + clsFunction.sPipe + clsSearch.ClassDateTo + clsFunction.sPipe +
                                                        clsSearch.ClassServiceTypeDesc + clsFunction.sPipe +
                                                        clsSearch.ClassServiceResultDesc + clsFunction.sPipe +
                                                        getDateFilter() + clsFunction.sPipe +
                                                        clsSearch.ClassIncludeBillable + clsFunction.sPipe +
                                                        clsSearch.ClassFSRMode + clsFunction.sPipe +
                                                        clsSearch.ClassDispatcherID + clsFunction.sPipe +
                                                        clsSearch.ClassRegionType;
                    break;
                */
                case 43: // SERVICE SUMMARY REPORT
                case 44: // SERVICE DETAIL REPORT
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassServiceStatusDesc + clsFunction.sPipe + clsSearch.ClassJobType + clsFunction.sPipe +
                                                        clsSearch.ClassDateFrom + clsFunction.sPipe + clsSearch.ClassDateTo + clsFunction.sPipe +
                                                        clsSearch.ClassServiceTypeDesc + clsFunction.sPipe +
                                                        clsSearch.ClassServiceResultDesc + clsFunction.sPipe +
                                                        getDateFilter() + clsFunction.sPipe +
                                                        clsSearch.ClassClientID + clsFunction.sPipe +
                                                        clsSearch.ClassIRIDNo + clsFunction.sPipe +
                                                        clsSearch.ClassFEID + clsFunction.sPipe +
                                                        clsSearch.ClassDispatcherID + clsFunction.sPipe +
                                                        clsSearch.ClassIncludeBillable + clsFunction.sPipe +
                                                        clsSearch.ClassFSRMode + clsFunction.sPipe +
                                                        clsSearch.ClassRegionType + clsFunction.sPipe +
                                                        clsSearch.ClassReasonID;
                    break;
                case 45: // INSTALLATION SUMMARY REPORT
                case 46: // INSTALLATION DETAIL REPORT
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassRegionType + clsFunction.sPipe + 
                        clsSearch.ClassRegionID + clsFunction.sPipe + 
                        clsSearch.ClassDateFrom + clsFunction.sPipe +
                        clsSearch.ClassDateTo;
                    break;

                case 47: // ACTIVE TERMINAL REPORT
                case 48: // ACTIVE SIM REPORT
                case 49: // ACTIVE SIM REPORT
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassClientID + clsFunction.sPipe + 
                        clsSearch.ClassLocationID + clsFunction.sPipe + 
                        clsSearch.ClassTerminalStatus + clsFunction.sPipe + 
                        clsSearch.ClassTerminalTypeID + clsFunction.sPipe + 
                        clsSearch.ClassTerminalModelID + clsFunction.sPipe +
                        clsSearch.ClassDateFrom + clsFunction.sPipe +
                        clsSearch.ClassDateTo;
                    break;
                case 52: // MONTHLY COMPLETED SERVICE REPORT
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassClientID.ToString() + clsFunction.sPipe + clsSearch.ClassServiceResultDesc + clsFunction.sPipe + clsSearch.ClassIncludeBillable + clsFunction.sPipe + clsSearch.ClassFSRMode;
                    break;

                case 53: // SERVICE INSTALLATION REPORT                    
                    clsSearch.ClassJobTypeList = $"{clsGlobalVariables.JOB_TYPE_INSTALLATION}{clsFunction.sComma}{clsGlobalVariables.JOB_TYPE_REPROGRAMMING}";
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassClientID + clsFunction.sPipe +
                                                        clsSearch.ClassJobTypeList + clsFunction.sPipe +
                                                        clsSearch.ClassIRIDNo + clsFunction.sPipe + 
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsSearch.ClassDateFrom + clsFunction.sPipe +
                                                        clsSearch.ClassDateTo + clsFunction.sPipe +
                                                        clsSearch.ClassIsExcludePending + clsFunction.sPipe +
                                                        clsSearch.ClassReasonID + clsFunction.sPipe +
                                                        clsSearch.ClassReportStatus;
                    break;
                case 54: // SERVICE MAINTENANCE REPORT
                    clsSearch.ClassJobTypeList = $"{clsGlobalVariables.JOB_TYPE_SERVICING}{clsFunction.sComma}{clsGlobalVariables.JOB_TYPE_REPLACEMENT}";
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassClientID + clsFunction.sPipe +                                                        
                                                        $"{clsGlobalVariables.JOB_TYPE_SERVICING}{clsFunction.sComma}{clsGlobalVariables.JOB_TYPE_REPLACEMENT}" + clsFunction.sPipe +
                                                        clsSearch.ClassIRIDNo + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsSearch.ClassDateFrom + clsFunction.sPipe +
                                                        clsSearch.ClassDateTo + clsFunction.sPipe +
                                                        clsSearch.ClassIsExcludePending + clsFunction.sPipe +
                                                        clsSearch.ClassReasonID + clsFunction.sPipe +
                                                        clsSearch.ClassReportStatus;
                    break;
                case 55: // SERVICE PULLOUT REPORT
                    clsSearch.ClassJobTypeList = $"{clsGlobalVariables.JOB_TYPE_PULLOUT}";
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassClientID + clsFunction.sPipe +                                                        
                                                        clsSearch.ClassJobTypeList + clsFunction.sPipe +
                                                        clsSearch.ClassIRIDNo + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsSearch.ClassDateFrom + clsFunction.sPipe +
                                                        clsSearch.ClassDateTo + clsFunction.sPipe +
                                                        clsSearch.ClassIsExcludePending + clsFunction.sPipe +
                                                        clsSearch.ClassReasonID + clsFunction.sPipe +
                                                        clsSearch.ClassReportStatus;
                    break;
                case 56: // UNCLOSED TICKET REPORT
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassClientID + clsFunction.sPipe +
                                                        clsSearch.ClassServiceNo + clsFunction.sPipe +
                                                        clsSearch.ClassIRIDNo + clsFunction.sPipe +
                                                        clsSearch.ClassDispatcherID + clsFunction.sPipe +
                                                        clsSearch.ClassDateFrom + clsFunction.sPipe +
                                                        clsSearch.ClassDateTo;
                    break;

                case 57: // HELPDESK DETAILS

                    if (rbRange.Checked)
                    {
                        clsSearch.ClassDateFrom = dteDateFrom.Value.ToString("yyyy-MM-dd");
                        clsSearch.ClassDateTo = dteDateTo.Value.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        clsSearch.ClassDateFrom = clsSearch.ClassDateTo = "";
                    }

                    clsSearch.ClassAdvanceSearchValue = getDateFilter() + clsFunction.sPipe +
                                                         clsSearch.ClassDateFrom + clsFunction.sPipe +
                                                         clsSearch.ClassDateTo + clsFunction.sPipe +
                                                         clsSearch.ClassClientID;
                    break;

                case 58: // HELPDESK KPI

                    if (rbRange.Checked)
                    {
                        clsSearch.ClassDateFrom = dteDateFrom.Value.ToString("yyyy-MM-dd");
                        clsSearch.ClassDateTo = dteDateTo.Value.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        clsSearch.ClassDateFrom = clsSearch.ClassDateTo = "";
                    }

                    clsSearch.ClassAdvanceSearchValue = getDateFilter() + clsFunction.sPipe +
                                                         clsSearch.ClassDateFrom + clsFunction.sPipe +
                                                         clsSearch.ClassDateTo + clsFunction.sPipe +
                                                         clsSearch.ClassClientID;
                    break;

            }
            
            //Debug.WriteLine("ComposeSearchValue, clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

        }

        private void PreviewReport(int ReportID)
        {
           
            Debug.WriteLine("--PreviewReport--");
            Debug.WriteLine("ReportID=" + ReportID);
            Debug.WriteLine("clsReport.ClassReportDesc=" + clsReport.ClassReportDesc);

            ResetID();

            clsSearch.ClassCurrentPage = clsSearch.ClassTotalPage = clsFunction.iZero;
            clsSearch.ClassDateFrom = clsSearch.ClassDateTo = clsSearch.ClassDetailDateFrom = clsSearch.ClassDetailDateTo = clsFunction.sDateFormat;
            clsSearch.ClassJobTypeDescription = clsFunction.sZero;

            // Variable Date
            stDateFrom = dteDateFrom.Value;
            sDateFrom = stDateFrom.ToString("yyyy-MM-dd");

            stDateTo = dteDateTo.Value;
            sDateTo = stDateTo.ToString("yyyy-MM-dd");

            // Detail
            stDetailDateFrom = dteDetailDateFrom.Value;
            sDetailDateFrom = stDetailDateFrom.ToString("yyyy-MM-dd");

            stDetailDateTo = dteDetailDateTo.Value;
            sDetailDateTo = stDetailDateTo.ToString("yyyy-MM-dd");

            // Set Field Value
            clsSearch.ClassServiceTypeID = dbAPI.GetServiceTypeFromList(cboSearchServiceType.Text);
           
            clsSearch.ClassParticularID = clsParticular.ClassParticularID;         
          
            clsSearch.ClassTerminalTypeID = dbAPI.GetTerminalTypeFromList(cboSearchType.Text);
            clsSearch.ClassTerminalModelID = dbAPI.GetTerminalModelFromList(cboSearchModel.Text);
            clsSearch.ClassTerminalBrandID = dbAPI.GetTerminalBrandFromList(cboSearchBrand.Text);
            clsSearch.ClassTID = (txtSearchTID.TextLength > 0 ? txtSearchTID.Text : "0");
            clsSearch.ClassMID = (txtSearchMID.TextLength > 0 ? txtSearchMID.Text : "0");
            clsSearch.ClassInvoiceNo = (txtSearchInvoiceNo.TextLength > 0 ? txtSearchInvoiceNo.Text : "0");
            clsSearch.ClassBatchNo = (txtSearchBatchNo.TextLength > 0 ? txtSearchBatchNo.Text : "0");

            clsSearch.ClassTerminalID = int.Parse(dbFunction.CheckAndSetNumericValue(txtSearchTerminalID.Text));
            clsSearch.ClassTerminalSN = (txtSearchTerminalSN.TextLength > 0 ? txtSearchTerminalSN.Text : "0");
            
            clsSearch.ClassLocation = dbFunction.CheckDefaultSelectValue(cboSearchLocation.Text);
            clsSearch.ClassAssetType = dbFunction.CheckDefaultSelectValue(cboSearchTerminalAssetType.Text);

            clsSearch.ClassServiceTypeID = int.Parse(dbFunction.CheckAndSetNumericValue(txtServiceTypeID.Text));
            clsSearch.ClassJobType = int.Parse(dbFunction.CheckAndSetNumericValue(txtJobType.Text));
            
            clsSearch.ClassIRIDNo = int.Parse(dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text));
            clsSearch.ClassIRNo = txtIRNo.Text;

            // Get ClientID
            //dbAPI.GetParticularInfo(clsFunction.sZero, cboSearchClient.Text);
            clsSearch.ClassClientID = int.Parse(dbFunction.CheckAndSetNumericValue(txtClientID.Text));

            // Get MerchantID
            clsSearch.ClassMerchantID = int.Parse(dbFunction.CheckAndSetNumericValue(txtMerchantID.Text));

            // Get FEID
            clsSearch.ClassFEID = int.Parse(dbFunction.CheckAndSetNumericValue(txtFEID.Text));

            // Get SPID
            clsSearch.ClassServiceProviderID = int.Parse(dbFunction.CheckAndSetNumericValue(txtSPID.Text));

            clsSearch.ClassSIMID = int.Parse(dbFunction.CheckAndSetNumericValue(txtSearchSIMID.Text));
           
            clsSearch.ClassSIMSerialNo = (txtSearchSIMSerialNo.TextLength > 0 ? txtSearchSIMSerialNo.Text : "0");
            clsSearch.ClassSIMCarrier = dbFunction.CheckDefaultSelectValue(cboSearchSIMCarrier.Text);
            
            clsSearch.ClassActionMade = cboSearchActionMade.Text;
            clsSearch.ClassIncludeBillable = cboSearchBillable.Text;

            clsSearch.ClassServiceTypeDescription = cboSearchServiceType.Text;
           
            clsSearch.ClassClientName = cboSearchClient.Text;

            clsSearch.ClassRegionType = int.Parse(dbFunction.CheckAndSetNumericValue(txtRegionType.Text));
            clsSearch.ClassRegionID = int.Parse(dbFunction.CheckAndSetNumericValue(txtRegionID.Text));

            clsSearch.ClassIRStatus = int.Parse(dbFunction.CheckAndSetNumericValue(txtStatus.Text));

            clsSearch.ClassStatus = int.Parse(dbFunction.CheckAndSetNumericValue(txtStatus.Text));
            clsSearch.ClassStatusDescription = clsSearch.ClassServiceStatusDescription = dbFunction.CheckAndSetStringValue(cboStatus.Text);

            clsSearch.ClassID = int.Parse(dbFunction.CheckAndSetNumericValue(txtSetupID.Text));

            clsSearch.ClassLocationID = int.Parse(dbFunction.CheckAndSetNumericValue(txtLocationID.Text));

            clsSearch.ClassIsPullOut = (chkPullout.Checked ? 1 : 0);
            clsSearch.ClassIsMobile = (chkMobile.Checked ? 1 : 0);
            clsSearch.ClassIsExcludePending = (chkPending.Checked ? 1 : 0);

            if (chkReqDate.Checked || chkCreatedDate.Checked || chkFSRDate.Checked || chkDeliveryDate.Checked || chkReleasedDate.Checked)
            {
                clsSearch.ClassDateFrom = sDateFrom;
                clsSearch.ClassDateTo = sDateTo;

                clsSearch.ClassDetailDateFrom = sDetailDateFrom;
                clsSearch.ClassDetailDateTo = sDetailDateTo;
            }

            if (!gbDateFilter.Enabled || rbAll.Checked)
            {
                clsSearch.ClassDateFrom = clsFunction.sDateFormat; ;
                clsSearch.ClassDateTo = clsFunction.sDateFormat;
            }

            // Detail
            if (!gbDetailDateFilter.Enabled || rbDetailAll.Checked)
            {
                clsSearch.ClassDetailDateFrom = clsFunction.sDateFormat; ;
                clsSearch.ClassDetailDateTo = clsFunction.sDateFormat;
            }


            clsSearch.ClassMobileID = int.Parse(dbFunction.CheckAndSetNumericValue(txtMobileID.Text));
            clsSearch.ClassMobileTerminalID = txtSearchMobile.Text;
            clsSearch.ClassMobileTerminalName = txtMobileName.Text;
            clsSearch.ClassMobileAssignedTo = txtMobileAssignedTo.Text;

            clsSearch.ClassServiceTypeDesc = cboSearchServiceType.Text;
            clsSearch.ClassServiceStatusDesc = cboSearchServiceStatus.Text;
            clsSearch.ClassServiceResultDesc = cboSearchActionMade.Text;

            clsSearch.ClassDispatcherID = int.Parse(dbFunction.CheckAndSetNumericValue(txtDispatcherID.Text));
            clsSearch.ClassDispatcherName = txtSearchDispatcher.Text;

            clsSearch.ClassFSRMode = cboFSRMode.Text;

            clsSearch.ClassLocation = cboSearchLocation.Text;
            clsSearch.ClassTerminalStatus = int.Parse(dbFunction.CheckAndSetNumericValue(txtTerminalStatus.Text));
            clsSearch.ClassTerminalStatusDescription = cboSearchTerminalStatus.Text;

            clsSearch.ClassServiceNo = int.Parse(dbFunction.CheckAndSetNumericValue(txtServiceNo.Text));
            clsSearch.ClassFSRNo = int.Parse(dbFunction.CheckAndSetNumericValue(txtFSRNo.Text));

            clsSearch.ClassTerminalType = cboSearchType.Text;
            clsSearch.ClassTerminalModel = cboSearchModel.Text;

            clsSearch.ClassReasonID = int.Parse(dbFunction.CheckAndSetNumericValue(txtReasonID.Text));

            clsSearch.ClassIncludeSummaryTab = (chkSummaryTab.Checked ? 0 : 1);
            clsSearch.ClassIncludeDetailTab = (chkDetailTab.Checked ? 0 : 1);

            clsSearch.ClassReportStatus = cboSearchReportStatus.Text;

            Debug.WriteLine("clsSearch.ClassReportID=" + clsSearch.ClassReportID);
            Debug.WriteLine("clsSearch.ClassReportDescription=" + clsSearch.ClassReportDescription);
            Debug.WriteLine("clsSearch.ClassClientID=" + clsSearch.ClassClientID);
            Debug.WriteLine("clsSearch.ClassServiceProviderID=" + clsSearch.ClassServiceProviderID);
            Debug.WriteLine("clsSearch.ClassParticularID=" + clsSearch.ClassParticularID);
            Debug.WriteLine("clsSearch.ClassDepartmentID=" + clsSearch.ClassDepartmentID);
            Debug.WriteLine("clsSearch.ClassServiceTypeID=" + clsSearch.ClassServiceTypeID);
            Debug.WriteLine("clsSearch.ClassTerminalTypeID=" + clsSearch.ClassTerminalTypeID);
            Debug.WriteLine("clsSearch.ClassTerminalModelID=" + clsSearch.ClassTerminalModelID);
            Debug.WriteLine("clsSearch.ClassTerminalBrandID=" + clsSearch.ClassTerminalBrandID);
            Debug.WriteLine("clsSearch.ClassTerminalSN=" + clsSearch.ClassTerminalSN);
            Debug.WriteLine("clsSearch.ClassSIMID=" + clsSearch.ClassSIMID);
            Debug.WriteLine("clsSearch.ClassSIMSerialNo=" + clsSearch.ClassSIMSerialNo);
            Debug.WriteLine("clsSearch.ClassSIMCarrier=" + clsSearch.ClassSIMCarrier);
            Debug.WriteLine("clsSearch.ClassIRNo=" + clsSearch.ClassIRNo);
            Debug.WriteLine("clsSearch.ClassServiceNo=" + clsSearch.ClassServiceNo);
            Debug.WriteLine("clsSearch.ClassFSRNo=" + clsSearch.ClassFSRNo);
            Debug.WriteLine("clsSearch.ClassFEID=" + clsSearch.ClassFEID);
            Debug.WriteLine("clsSearch.ClassTID=" + clsSearch.ClassTID);
            Debug.WriteLine("clsSearch.ClassMID=" + clsSearch.ClassMID);
            Debug.WriteLine("clsSearch.ClassTerminalStatus=" + clsSearch.ClassTerminalStatus);
            Debug.WriteLine("clsSearch.ClassFSRStatus=" + clsSearch.ClassFSRStatus);
            Debug.WriteLine("clsSearch.ClassServiceStatus=" + clsSearch.ClassServiceStatus);
            Debug.WriteLine("clsSearch.ClassFSRDateFrom=" + clsSearch.ClassFSRDateFrom);
            Debug.WriteLine("clsSearch.ClassFSRDateTo=" + clsSearch.ClassFSRDateTo);
            Debug.WriteLine("clsSearch.ClassActionMade=" + clsSearch.ClassActionMade);
            Debug.WriteLine("clsSearch.ClassJobTypeDescription=" + clsSearch.ClassJobTypeDescription);
            Debug.WriteLine("clsSearch.ClassCurrentPage=" + clsSearch.ClassCurrentPage);
            Debug.WriteLine("clsSearch.ClassTotalPage=" + clsSearch.ClassTotalPage);
            Debug.WriteLine("clsSearch.ClassRegionID=" + clsSearch.ClassRegionID);
            Debug.WriteLine("clsSearch.ClassProvinceID=" + clsSearch.ClassProvinceID);           
            Debug.WriteLine("clsSearch.ClassLocation=" + clsSearch.ClassLocation);
            Debug.WriteLine("clsSearch.ClassAssetType=" + clsSearch.ClassAssetType);
            Debug.WriteLine("clsSearch.ClassIRIDNo=" + clsSearch.ClassIRIDNo);
            Debug.WriteLine("clsSearch.ClassIRNo=" + clsSearch.ClassIRNo);
            Debug.WriteLine("clsSearch.ClassRegionType=" + clsSearch.ClassRegionType);
            Debug.WriteLine("clsSearch.ClassRegionID=" + clsSearch.ClassRegionID);
            Debug.WriteLine("clsSearch.ClassDateFrom=" + clsSearch.ClassDateFrom);
            Debug.WriteLine("clsSearch.ClassDateTo=" + clsSearch.ClassDateTo);
            Debug.WriteLine("clsSearch.ClassID=" + clsSearch.ClassID);
            Debug.WriteLine("clsSearch.ClassLocationID=" + clsSearch.ClassLocationID);
            Debug.WriteLine("clsSearch.ClassIsPullOut=" + clsSearch.ClassIsPullOut);
            Debug.WriteLine("clsSearch.ClassIncludeBillable=" + clsSearch.ClassIncludeBillable);

            Debug.WriteLine("clsSearch.ClassIsMobile=" + clsSearch.ClassIsMobile);
            Debug.WriteLine("clsSearch.ClassMobileTerminalID=" + clsSearch.ClassMobileTerminalID);
            Debug.WriteLine("clsSearch.ClassMobileTerminalName=" + clsSearch.ClassMobileTerminalName);
            Debug.WriteLine("clsSearch.ClassMobileAssignedTo=" + clsSearch.ClassMobileAssignedTo);

            Debug.WriteLine("clsSearch.ClassServiceTypeDesc=" + clsSearch.ClassServiceTypeDesc);
            Debug.WriteLine("clsSearch.ClassServiceStatusDesc=" + clsSearch.ClassServiceStatusDesc);
            Debug.WriteLine("clsSearch.ClassServiceResultDesc=" + clsSearch.ClassServiceResultDesc);

            Debug.WriteLine("clsSearch.ClassDispatcherID=" + clsSearch.ClassDispatcherID);
            Debug.WriteLine("clsSearch.ClassDispatcherName=" + clsSearch.ClassDispatcherName);

            Debug.WriteLine("clsSearch.ClassFSRMode=" + clsSearch.ClassFSRMode);

            Debug.WriteLine("clsSearch.ClassTerminalStatus=" + clsSearch.ClassTerminalStatus);
            Debug.WriteLine("clsSearch.ClassTerminalStatusDescription=" + clsSearch.ClassTerminalStatusDescription);

            Debug.WriteLine("clsSearch.ClassServiceNo=" + clsSearch.ClassServiceNo);
            Debug.WriteLine("clsSearch.ClassFSRNo=" + clsSearch.ClassFSRNo);
            Debug.WriteLine("clsSearch.ClassIsExcludePending=" + clsSearch.ClassIsExcludePending);

            Debug.WriteLine("clsSearch.ClassReasonID=" + clsSearch.ClassReasonID);

            Debug.WriteLine("clsSearch.ClassIncludeSummaryTab=" + clsSearch.ClassIncludeSummaryTab);
            Debug.WriteLine("clsSearch.ClassIncludeDetailTab=" + clsSearch.ClassIncludeDetailTab);

            Debug.WriteLine("clsSearch.ClassReportStatus=" + clsSearch.ClassReportStatus);

            ComposeSearchValue(ReportID);

            //Debug.WriteLine("clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

            Debug.WriteLine("ReportID=" + ReportID);

            switch (ReportID)
            {
                case 4: // FSR (SERVICING) REPORT
                    dbReportFunc.ViewFSRServicing();
                    break;
                case 1: // TERMINAL(INVENTORY) DETAIL REPORT
                    dbReportFunc.ViewTerminalInventory();
                    break;
                case 3: // SIM(INVENTORY) DETAIL REPORT
                    dbReportFunc.ViewSIMInventory();
                    break;
                case 5: // FSR (ENGINEER) REPORT
                    clsSearch.ClassIsExportToPDF = false;
                    dbReportFunc.ViewFSR(5);
                    break;

                case 8: // INSTALLATION REQUEST SUMMARY REPORT
                    dbAPI.GenerateInstallationSummary();

                    dbReportFunc.ViewInstallationSummaryReport();

                    break;
                case 9: // INSTALLATION REQUEST DETAIL REPORT
                    dbReportFunc.ViewInstallationReport();
                    break;
                case 18:
                    dbReportFunc.ViewFSRServicingV2();
                    break;
                case 11: // MERCHANT SERVICE HISTORY REPORT
                    dbReportFunc.ViewServiceHistoryDetail(11);
                    break;
                case 19: // TERMINAL(INVENTORY) SUMMARY REPORT

                    dbAPI.GenerateTerminalSummary();

                    dbReportFunc.ViewTerminalSummaryInventory();
                    break;
                case 20: // SIM(INVENTORY) SUMMARY REPORT

                    dbAPI.GenerateSIMSummary();

                    dbReportFunc.ViewSIMSummaryInventory();
                    break;
                case 23:
                    dbReportFunc.ViewFSROperation();
                    break;

                case 25: // RELEASED TERMINAL SUMMARY REPORT

                    dbReportFunc.ViewReleaseTerminalSummaryInventory();

                    break;
                case 26: // RELEASED TERMINAL DETAIL REPORT
                    dbReportFunc.ViewReleaseTerminalDetailInventory();

                    break;
                case 27: // RELEASED SIM DETAIL REPORT
                    dbReportFunc.ViewReleaseSIMDetailInventory();

                    break;
                case 28: // RELEASED SIM DETAIL REPORT
                    dbReportFunc.ViewReleaseSIMSummaryInventory();

                    break;
                case 32: // TERMINAL SUMMARY(TYPE/MODEL) INVENTORY REPORT
                    dbAPI.GenerateTerminalSummary();

                    dbReportFunc.ViewTerminalTypeModelSummaryInventory();
                    break;
                case 33: // SIM SUMMARY(TELCO) INVENTORY REPORT

                    dbAPI.GenerateSIMSummary();

                    dbReportFunc.ViewSIMTelcoSummaryInventory();
                    break;

                case 42: // DIAGNOSTIC (ENGINEER) REPORT
                    clsSearch.ClassIsExportToPDF = false;
                    dbReportFunc.ViewDiagnosticReport(42);
                    break;

                case 43: // SERVICE SUMMARY REPORT(TYPE)                   
                    dbReportFunc.ViewServiceSummaryReport();
                    break;
                   
                case 44: // SERVICE DETAIL REPORT(TYPE)
                    dbReportFunc.ViewServiceDetailReport();
                    break;

                case 45: // SERVICE INSTALLATION SUMMARY REPORT               
                    dbReportFunc.ViewServiceInstallationSummaryReport();
                    break;

                case 46: // SERVICE INSTALLATION DETAIL REPORT
                    dbReportFunc.ViewServiceInstallationDetailReport();
                    break;

                case 47: // ACTIVE POS SUMMARY REPORT
                    dbReportFunc.ViewActiveTerminalReport();
                    break;

                case 48: // ACTIVE SIM REPORT
                    dbReportFunc.ViewActiveSIMReport();
                    break;

                case 49: // ACTIVE POS DETAIL REPORT
                    dbReportFunc.ViewActiveTerminalDetailReport();
                    break;
                case 50: // FSR AND DIAGNOSTIC REPROT
                    clsSearch.ClassIsExportToPDF = false;
                    dbFunction.SetMessageBox("FSR report will be generated." + "\n\n" + "Select [OK] to continue.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
                    ComposeSearchValue(5);
                    dbReportFunc.ViewFSR(5);

                    dbFunction.SetMessageBox("Diagnostic report will be generated." + "\n\n" + "Select [OK] to continue.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
                    ComposeSearchValue(42);
                    dbReportFunc.ViewDiagnosticReport(42);
                    break;

                case 52: // MONTHLY COMPLETED SERVICE REPORT
                    dbReportFunc.ViewMonthlyServiceCountReport();
                    break;
                case 53: // SERVICE INSTALLATION REPORT
                case 54: // SERVICE MAINTENANCE REPORT
                case 55: // SERVICE PULLOUT REPORT

                    if (!dbAPI.isRecordExist("Search", "Report Status", clsSearch.ClassReportType))
                    {
                        dbAPI.insertReportStatus(clsSearch.ClassReportType, (int)Enums.ReportProcessType.Processing, clsDefines.REPORT_STATUS_PROCESSING, dbFunction.getCurrentDateTime(), clsUser.ClassUserFullName);
                    }

                    // get info of last process
                    string pJSONString = dbAPI.getInfoDetailJSON("Search", "Report Status Info", clsSearch.ClassReportType);
                    if (dbFunction.isValidDescription(pJSONString))
                    {
                        dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                        string message = "";
                        string pSearchValue = "";
                        string processedBy = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ProcessedBy);
                        string processedAt = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ProcessedAt);
                        int statusID = int.Parse(dbFunction.CheckAndSetNumericValue(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_StatusID)));
                        string status = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Status);

                        int isReset = int.Parse(dbFunction.CheckAndSetNumericValue(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_isReset)));
                        
                        switch (statusID)
                        {
                            case (int)Enums.ReportProcessType.Idle: // Idle

                                pSearchValue = $"{clsSearch.ClassReportType}{clsDefines.gPipe}{(int)Enums.ReportProcessType.Processing}{clsDefines.gPipe}" +
                                        $"{clsDefines.REPORT_STATUS_PROCESSING}{clsDefines.gPipe}" +
                                        $"{dbFunction.getCurrentDateTime()}{clsDefines.gPipe}" +
                                        $"{clsUser.ClassUserFullName}";

                                dbAPI.ExecuteAPI("PUT", "Update", "Report Status", pSearchValue, "", "", "UpdateCollectionDetail");

                                break;
                            case (int)Enums.ReportProcessType.Processing: // Processing

                                if (isReset > 0)
                                {
                                    message =
                                                $"This report was last processed by \"{processedBy}\" on {processedAt:yyyy-MM-dd HH:mm:ss}.\n\n" +
                                                "It appears to be stuck in processing.\n\n" +
                                                "Do you want to force reset and proceed with a new generation?";

                                    if (!dbFunction.fPromptConfirmation(message)) return;

                                    // update
                                    pSearchValue = $"{clsSearch.ClassReportType}{clsDefines.gPipe}{(int)Enums.ReportProcessType.Processing}{clsDefines.gPipe}" +
                                        $"{clsDefines.REPORT_STATUS_PROCESSING}{clsDefines.gPipe}" +
                                        $"{dbFunction.getCurrentDateTime()}{clsDefines.gPipe}" +
                                        $"{clsUser.ClassUserFullName}";

                                    dbAPI.ExecuteAPI("PUT", "Update", "Report Status", pSearchValue, "", "", "UpdateCollectionDetail");
                                }
                                else
                                {
                                    message =
                                                $"This report is currently being processed by \"{processedBy}\" since {processedAt:yyyy -MM-dd HH:mm:ss}.\n\n" +
                                                "Please wait until the current process completes.";

                                    dbFunction.SetMessageBox(message, "Report", clsFunction.IconType.iInformation);
                                    return;
                                }
                                break;
                        }                        
                    }
                    
                    dbReportFunc.ViewServiceReport();

                    break;
                case 56: // UNCLOSED TICKET REPORT                    
                    dbReportFunc.ViewUnclosedServiceTicketReport();
                    break;
                case 57: // HELPDESK SUMMARY REPORT
                    dbReportFunc.ViewHelpdeskDetailReport();
                    break;
                case 58: // HELPDESK KPI REPORT
                    dbReportFunc.ViewHelpdeskDetailReport();
                    break;
                default:
                    dbFunction.SetMessageBox("No report to be generated.", "Report failed", clsFunction.IconType.iExclamation);
                    break;
            }


        }

        private bool isValidReport()
        {
            bool isValid = false;
            int ReportID = 0;

            ReportID = (txtReportID.Text.Length > 0 ? int.Parse(txtReportID.Text) : 0);

            if (ReportID > 0)
            {
                return true;
            }

            if (!isValid)
            {
                MessageBox.Show("Please select report to view first.", "Invalid Report", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
        
        private void SetOption()
        {
            gbParticular.Enabled = false;
            gbRegion.Enabled = false;
            gbServiceType.Enabled = false;
            gbServiceStatus.Enabled = false;
            gbMerchant.Enabled = false;
            gbTerminal.Enabled = false;
            gbSIM.Enabled = false;
          
            gbServiceResult.Enabled = false;
            gbDateFlag.Enabled = false;
            gbDataFilter.Enabled = false;
            gbLocation.Enabled = false;
            gbStatus.Enabled = false;
            gbSetup.Enabled = false;

            gbMobile.Enabled = false;
            gbDateFilter.Enabled = false;
            gbDetailDateFilter.Enabled = false;
            gbReason.Enabled = false;

            gbInventoryStatus.Enabled = false;

            chkReqDate.Enabled = chkCreatedDate.Enabled = chkFSRDate.Enabled = chkDeliveryDate.Enabled = chkReleasedDate.Enabled = chkPullout.Enabled = chkMobile.Enabled = false;
            chkReqDate.Checked = chkCreatedDate.Checked = chkFSRDate.Checked  = chkDeliveryDate.Checked = chkReleasedDate.Checked = chkPullout.Checked = chkMobile.Checked = false;
            
            chkPending.Enabled = chkSummaryTab.Enabled = chkDetailTab.Enabled = false;
            chkPending.Checked = chkSummaryTab.Checked = chkDetailTab.Checked = true;

            rbAll.Checked = rbToday.Checked = rbRange.Checked = false;

            btnSearchService.Enabled = false;

            btnSearchFE.Enabled = btnSearchDispatcher.Enabled = false;
            btnSearchReason.Enabled = false;

            gbReportStatus.Enabled = false;

            switch (clsReport.ClassReportID)
            {
                case 1: // TERMINAL DETAIL INVENTORY REPORT
                    //gbParticular.Enabled = true;
                    gbTerminal.Enabled = true;
                    gbLocation.Enabled = true;
                    gbStatus.Enabled = true;
                    gbDateFlag.Enabled = true;
                    gbDateFilter.Enabled = true;
                    gbDataFilter.Enabled = true;
                    gbInventoryStatus.Enabled = true;

                    chkDeliveryDate.Enabled = false;
                    chkDeliveryDate.Checked = false;

                    rbAll.Checked = true;

                    break;

                case 5: // FSR REPORT                    
                    gbParticular.Enabled = true;
                    cboSearchClient.Enabled = false;
                    btnSearchFE.Enabled = btnSearchDispatcher.Enabled = false;

                    gbMerchant.Enabled = true;
                    btnSearchMerchant.Enabled = false;
                    btnSearchService.Enabled = true;

                    gbReason.Enabled = true;
                    btnSearchReason.Enabled = true;
                    break;

                case 3: // SIM DETAIL INVENTORY REPORT
                    //gbParticular.Enabled = true;
                    gbSIM.Enabled = true;
                    gbLocation.Enabled = true;
                    gbStatus.Enabled = true;
                    gbDateFlag.Enabled = true;
                    gbDateFilter.Enabled = true;
                    gbDataFilter.Enabled = true;
                    gbInventoryStatus.Enabled = true;

                    chkDeliveryDate.Enabled = false;
                    chkDeliveryDate.Checked = false;

                    rbAll.Checked = true;
                    break;
                case 8: // INSTALLATION REQUEST SUMMARY REPORT
                    gbParticular.Enabled = true;
                    gbRegion.Enabled = true;
                    gbServiceType.Enabled = true;

                    cboSearchProvince.Enabled = false;

                    break;
                case 9: // INSTALLATION REQUEST DETAIL REPORT
                    gbParticular.Enabled = true;
                    gbRegion.Enabled = true;
                    
                    gbMerchant.Enabled = true; 
                    gbDateFlag.Enabled = true;
                    gbDateFilter.Enabled = true;
                    gbStatus.Enabled = true;
                    gbSetup.Enabled = true;

                    chkReqDate.Enabled = true;
                    chkReqDate.Checked = true;

                    cboSearchProvince.Enabled = true;
                    
                    break;
                case 18: // CLIENT SERVICING REPORT
                    gbTerminal.Enabled = true;
                    gbSIM.Enabled = true;
                    gbParticular.Enabled = true;
                    gbRegion.Enabled = true;
                    gbServiceType.Enabled = true;
                    gbMerchant.Enabled = true;
                  
                    gbServiceResult.Enabled = true;
                    gbServiceStatus.Enabled = true;
                    gbStatus.Enabled = true;
                    gbDateFlag.Enabled = true;
                    gbDateFilter.Enabled = true;
                    gbDataFilter.Enabled = true;
                    gbInventoryStatus.Enabled = true;

                    gbMobile.Enabled = true;

                    chkFSRDate.Enabled = true;
                    chkFSRDate.Checked = true;

                    chkPullout.Enabled = true;
                    chkPullout.Checked = true;

                    chkMobile.Enabled = false;
                    chkMobile.Checked = false;

                    gbReason.Enabled = true;
                    btnSearchReason.Enabled = true;

                    break;
                case 11: // MERCHANT SERVICE HISTORY REPORT
                    gbParticular.Enabled = true;
                    cboSearchClient.Enabled = false;
                    gbMerchant.Enabled = true;
                    btnSearchMerchant.Enabled = true;

                    gbReason.Enabled = true;
                    btnSearchReason.Enabled = true;

                    break;
                case 19: // TERMINAL SUMMARY INVENTORY REPORT
                    //gbParticular.Enabled = true;
                    gbTerminal.Enabled = true;
                    gbLocation.Enabled = true;
                    
                    break;
                case 20: // SIM SUMMARY INVENTORY REPORT
                    gbParticular.Enabled = true;
                    gbSIM.Enabled = true;
                    gbLocation.Enabled = true;
                    
                    break;

                case 23: // OPERATION FSR REPORT
                    gbTerminal.Enabled = true;
                    gbSIM.Enabled = true;
                    gbParticular.Enabled = true;
                    gbRegion.Enabled = true;
                    gbServiceType.Enabled = true;
                    gbMerchant.Enabled = true;

                    gbServiceResult.Enabled = true;
                    gbServiceStatus.Enabled = true;
                    gbStatus.Enabled = true;
                    gbDateFlag.Enabled = true;
                    gbDateFilter.Enabled = true;
                    gbDataFilter.Enabled = true;
                    gbInventoryStatus.Enabled = true;

                    gbMobile.Enabled = true;

                    chkFSRDate.Enabled = true;
                    chkFSRDate.Checked = true;

                    chkPullout.Enabled = true;
                    chkPullout.Checked = true;

                    chkMobile.Enabled = false;
                    chkMobile.Checked = false;

                    gbReason.Enabled = true;
                    btnSearchReason.Enabled = true;

                    break;

                case 25: // RELEASED TERMINAL SUMMARY REPORT
                    gbTerminal.Enabled = true;
                    gbLocation.Enabled = true;

                    gbDateFlag.Enabled = true;
                    gbDateFilter.Enabled = true;
                    gbInventoryStatus.Enabled = true;

                    chkReleasedDate.Enabled = true;
                    chkReleasedDate.Checked = true;

                    break;
                case 28: // RELEASED SIM SUMMARY REPORT
                    gbSIM.Enabled = true;
                    gbLocation.Enabled = true;

                    gbDateFlag.Enabled = true;
                    gbDateFilter.Enabled = true;
                    gbInventoryStatus.Enabled = true;

                    chkReleasedDate.Enabled = true;
                    chkReleasedDate.Checked = true;

                    break;

                case 26: // RELEASED TERMINAL DETAIL REPORT
                    gbTerminal.Enabled = true;
                    gbLocation.Enabled = true;

                    gbDateFlag.Enabled = true;
                    gbDateFilter.Enabled = true;
                    gbInventoryStatus.Enabled = true;

                    chkReleasedDate.Enabled = true;
                    chkReleasedDate.Checked = true;

                    break;

                case 27: // RELEASED SIM DETAIL REPORT
                    gbSIM.Enabled = true;
                    gbLocation.Enabled = true;

                    gbDateFlag.Enabled = true;
                    gbDateFilter.Enabled = true;
                    gbInventoryStatus.Enabled = true;

                    chkReleasedDate.Enabled = true;
                    chkReleasedDate.Checked = true;

                    break;
                case 32: // TERMINAL SUMMARY(TYPE/MODEL) INVENTORY REPORT
                    gbParticular.Enabled = true;
                    gbTerminal.Enabled = true;
                    gbInventoryStatus.Enabled = true;

                    break;
                case 33: // SIM SUMMARY(TELCO) INVENTORY REPORT       
                    gbParticular.Enabled = true;
                    gbSIM.Enabled = true;
                    gbInventoryStatus.Enabled = true;

                    break;
                case 42: // DIAGNOSTIC REPORT
                    gbParticular.Enabled = true;
                    cboSearchClient.Enabled = false;
                    btnSearchFE.Enabled = btnSearchDispatcher.Enabled = false;

                    gbMerchant.Enabled = true;
                    btnSearchMerchant.Enabled = false;
                    btnSearchService.Enabled = true;
                    break;
                case 43: // SERVICE SUMMARY REPORT
                case 44: // SERVICE DETAIL REPORT

                    if (clsReport.ClassReportID == 43)
                        cboFSRMode.Enabled = false;
                    else
                        cboFSRMode.Enabled = true;

                    cboSearchClient.Enabled = true;
                    gbParticular.Enabled = true;
                    gbRegion.Enabled = true;
                    gbServiceType.Enabled = true;
                    gbServiceStatus.Enabled = true;
                    gbServiceResult.Enabled = true;
                    gbDateFlag.Enabled = true;
                    gbDateFilter.Enabled = true;

                    gbDataFilter.Enabled = true;
                    gbInventoryStatus.Enabled = true;

                    chkFSRDate.Enabled = true;
                    chkFSRDate.Checked = true;

                    rbAll.Enabled = true;
                    rbToday.Enabled = true;
                    rbRange.Enabled = true;

                    rbToday.Checked = true;

                    cboSearchServiceStatus.Enabled = true;
                    cboSearchTerminalStatus.Enabled = false;

                    gbReason.Enabled = true;
                    btnSearchReason.Enabled = true;

                    break;
                case 45: // INSTALLATION SUMMARY REPORT
                case 46: // INSTALLATION DETAIL REPORT
                    gbParticular.Enabled = true;
                    cboSearchClient.Enabled = true;
                    gbRegion.Enabled = true;
                    gbDateFilter.Enabled = true;
                    gbMerchant.Enabled = true;

                    chkReqDate.Enabled = true;
                    chkReqDate.Checked = true;

                    rbAll.Checked = true;

                    btnSearchMerchant.Enabled = true;
                    btnSearchService.Enabled = false;

                    break;
                case 47: // ACTIVE TERMINAL REPORT
                case 48: // ACTIVE SIM REPORT
                case 49: // ACTIVE SIM REPORT
                    gbLocation.Enabled = true;
                    gbParticular.Enabled = true;
                    gbRegion.Enabled = true;
                    cboSearchClient.Enabled = true;
                    gbServiceStatus.Enabled = true;
                    gbTerminal.Enabled = true;
                    cboSearchServiceStatus.Enabled = false;
                    cboSearchTerminalStatus.Enabled = true;

                    gbDateFlag.Enabled = true;
                    gbDateFilter.Enabled = true;
                    gbInventoryStatus.Enabled = true;

                    rbAll.Checked = true;

                    chkFSRDate.Enabled = true;
                    chkFSRDate.Checked = true;
                    
                    break;
                case 50: // FSR AND DIAGNOSTIC REPORT
                    gbParticular.Enabled = true;
                    cboSearchClient.Enabled = false;
                    btnSearchFE.Enabled = btnSearchDispatcher.Enabled = false;

                    gbMerchant.Enabled = true;
                    btnSearchMerchant.Enabled = false;
                    btnSearchService.Enabled = true;
                    break;

                case 52: // MONTHLY COMPLETED SERVICE REPORT                    
                    gbParticular.Enabled = true;
                    cboSearchClient.Enabled = true;
                    btnSearchFE.Enabled = btnSearchDispatcher.Enabled = false;
                    gbServiceResult.Enabled = true;
                    cboSearchActionMade.Enabled = cboSearchBillable.Enabled = cboFSRMode.Enabled = true;
                    
                    break;
                case 53: // SERVICE INSTALLATION REPORT   
                case 54: // SERVICE MAINTENANCE REPORT
                case 55: // SERVICE PULLOUT REPORT
                    gbParticular.Enabled = true;
                    gbDateFlag.Enabled = true;
                    gbDateFilter.Enabled = true;
                    gbDataFilter.Enabled = true;
                    gbMerchant.Enabled = true;
                    gbReportStatus.Enabled = true;
                    
                    chkFSRDate.Enabled = true;
                    chkFSRDate.Checked = true;

                    chkPullout.Enabled = chkPullout.Checked = false;
                    chkMobile.Enabled = chkMobile.Checked = false;

                    //chkPending.Enabled = true;
                    //chkPending.Checked = true;

                    chkSummaryTab.Enabled = chkDetailTab.Enabled = true;
                    chkSummaryTab.Checked = chkDetailTab.Checked = true;

                    rbToday.Checked = true;

                    gbReason.Enabled = true;

                    btnSearchReason.Enabled = true;
                    btnSearchMerchant.Enabled = true;
                    btnSearchService.Enabled = false;

                    break;
                case 56: // UNCLOSED TICKET REPORT
                    gbParticular.Enabled = true;
                    btnSearchDispatcher.Enabled = true;
                    gbDateFlag.Enabled = true;

                    chkFSRDate.Enabled = true;
                    chkFSRDate.Checked = true;

                    gbDateFilter.Enabled = true;
                    rbAll.Checked = true;

                    break;

                case 57: // HELPDESK DETAIL REPORT
                    gbParticular.Enabled = true;
                    cboSearchClient.Enabled = true;
                    btnSearchFE.Enabled = btnSearchDispatcher.Enabled = false;

                    gbDateFilter.Enabled = true;
                    rbAll.Checked = true;
                    rbAll.Enabled = true;
                    rbToday.Enabled = true;
                    rbRange.Enabled = true;
                    break;

                case 58: // HELPDESK KPI
                    gbParticular.Enabled = true;
                    cboSearchClient.Enabled = true;
                    btnSearchFE.Enabled = btnSearchDispatcher.Enabled = false;

                    gbDateFilter.Enabled = true;
                    rbAll.Checked = true;
                    rbAll.Enabled = true;
                    rbToday.Enabled = true;
                    rbRange.Enabled = true;
                    break;

                default:
                    break;
            }

            // set button
            dbFunction.SetButtonIconImage(btnSearchFE);
            dbFunction.SetButtonIconImage(btnSearchDispatcher);
            dbFunction.SetButtonIconImage(btnSearchMobile);
            dbFunction.SetButtonIconImage(btnSearchMerchant);
            dbFunction.SetButtonIconImage(btnSearchService);
            dbFunction.SetButtonIconImage(btnSearchTerminalSN);
            dbFunction.SetButtonIconImage(btnSearchSIMSN);
            dbFunction.SetButtonIconImage(btnSearchReason);
        }

        private void SetMKTextBoxBackColor()
        {
            txtSearchFE.BackColor = txtSearchMerchant.BackColor = txtSearchMerchant.BackColor = txtSearchTerminalSN.BackColor = txtSearchSIMSerialNo.BackColor = txtSearchSetup.BackColor = txtSearchMobile.BackColor = txtSearchDispatcher.BackColor = txtReason.BackColor = clsFunction.MKBackColor;         
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void bunifuCards2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label32_Click(object sender, EventArgs e)
        {

        }

        private void SetCheckBoxDate(int pIndex)
        {
            switch (pIndex)
            {
                case 1:
                    if (chkReqDate.Checked)
                    {
                        chkCreatedDate.Checked = chkFSRDate.Checked = false;
                    }
                    break;
                case 2:
                    if (chkCreatedDate.Checked)
                    {
                        chkReqDate.Checked = chkFSRDate.Checked = false;
                    }
                    break;
                case 3:
                    if (chkFSRDate.Checked)
                    {
                        chkReqDate.Checked = chkCreatedDate.Checked = false;
                    }
                    break;
            }
        }
        
        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassStatus = 0;
            if (!cboStatus.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Status List", cboStatus.Text);
                clsSearch.ClassStatus = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassStatus=" + clsSearch.ClassStatus);
            }

            txtStatus.Text = clsSearch.ClassStatus.ToString();
        }

        private void cboSearchLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassLocationID = 0;
            if (!cboSearchLocation.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Location", cboSearchLocation.Text);
                clsSearch.ClassLocationID = clsSearch.ClassOutFileID;            
            }

            txtLocationID.Text = clsSearch.ClassLocationID.ToString();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (!isValidReport()) return;

            if (txtReportID.TextLength > 0)
            {
                if (gbMerchant.Enabled)
                {
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iClientID, txtClientID.Text)) return;
                }
                
                // Check Date               
                if (!dbFunction.checkDateFromTo(DateTime.Parse(dteDateFrom.Value.ToShortDateString()), DateTime.Parse(dteDateTo.Value.ToShortDateString())))
                {
                    dbFunction.SetMessageBox("[Date From] must not greater than [Date To]", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return;
                }

                if (gbParticular.Enabled)
                {
                    if (btnSearchFE.Enabled)
                    {   
                        if (!dbFunction.isValidEntry(clsFunction.CheckType.iFEName, txtSearchFE.Text)) return;
                    }

                    if (btnSearchDispatcher.Enabled)
                    {
                        if (!dbFunction.isValidEntry(clsFunction.CheckType.iiDispatcher, txtSearchDispatcher.Text)) return;
                    }
                }
                

                if (!dbFunction.fPromptConfirmation("Are you sure to preview report?" + "\n\n" + "Report Type: " + "\n" + clsSearch.ClassReportType)) return;

                // Waiting / Hour Glass
                Cursor.Current = Cursors.WaitCursor;

                try
                {
                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                    ResetID();

                    PreviewReport(clsReport.ClassReportID);
                    
                    Cursor.Current = Cursors.Default; // Back to normal 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                // Back to normal 
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnSearchSetup_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iSetup;
            frmSearchField.iStatus = 0;
            frmSearchField.sHeader = "VIEW SETUP";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                txtSetupID.Text = clsSearch.ClassID.ToString();
                txtSearchSetup.Text = clsSearch.ClassDescription;
            }
        }
        
        private void btnSearchMobile_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iMobile;
            frmSearchField.sHeader = "TERMINAL/MOBILE";
            frmSearchField.sSearchChar = txtSearchMobile.Text;
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                Debug.WriteLine("clsSearch.ClassMobileID=" + clsSearch.ClassMobileID);
                
                txtMobileID.Text = clsSearch.ClassMobileID.ToString();

                dbAPI.ExecuteAPI("GET", "Search", "Mobile Info", txtMobileID.Text, "Get Info Detail", "", "GetInfoDetail");

                chkMobile.Checked = false;
                chkPullout.Checked = true;

                txtSearchMobile.Text = txtMobileName.Text = txtMobileAssignedTo.Text = clsFunction.sNull;
                if (dbAPI.isNoRecordFound() == false)
                {   
                    chkMobile.Checked = true;
                    chkPullout.Checked = false;

                    txtSearchMobile.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_MobileTerminalID);
                    txtMobileName.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_MobileTerminalName);
                    txtMobileAssignedTo.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_FullName);
                }
                
            }
        }

        private void cboSearchServiceStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            gbDateFlag.Enabled = gbDateFilter.Enabled = true;
            rbAll.Enabled = rbToday.Enabled = rbRange.Enabled = true;
            cboSearchActionMade.Enabled = true;
            cboSearchBillable.Enabled = true;

            if (cboSearchServiceStatus.Text.Equals(clsDefines.SERVICE_STATUS_PROCESSING) || cboSearchServiceStatus.Text.Equals(clsDefines.SERVICE_STATUS_PENDING) || cboSearchServiceStatus.Text.Equals(clsDefines.SERVICE_STATUS_OVERALL_PENDING))
            {
                cboSearchActionMade.Enabled = false;
                cboSearchBillable.Enabled = false;
                cboFSRMode.Enabled = false;
                cboSearchActionMade.Text = cboSearchBillable.Text = clsFunction.sDefaultSelect;
                gbDateFlag.Enabled = gbDateFilter.Enabled = false;
                rbAll.Enabled = rbToday.Enabled = rbRange.Enabled = false;
            }
            else if (cboSearchServiceStatus.Text.Equals(clsDefines.SERVICE_STATUS_COMPLETED))
            {
                rbAll.Enabled = false;
            }
           
            initDateFilter(false);
            
        }

        private void rbToday_CheckedChanged(object sender, EventArgs e)
        {
            initDateFilter(false);
            InitDateRange();
        }

        private void rbRange_CheckedChanged(object sender, EventArgs e)
        {
            initDateFilter(false);
        }

        private string getDateFilter()
        {
            string pOutput = "";

            if (rbAll.Checked)
                pOutput = "ALL";

            if (rbToday.Checked)
                pOutput = "TODAY";

            if (rbRange.Checked)
                pOutput = "RANGE";

            return pOutput;
        }

        private void initDateFilter(bool isDetail)
        {
            if (isDetail)
            {
                dteDetailDateFrom.Enabled = dteDetailDateTo.Enabled = true;
                if (rbDetailAll.Checked || rbDetailToday.Checked)
                {
                    dteDetailDateFrom.Enabled = dteDetailDateTo.Enabled = false;
                }
            }
            else
            {
                if (!gbDetailDateFilter.Enabled)
                {
                    dteDateFrom.Enabled = dteDateTo.Enabled = true;
                    if (rbAll.Checked || rbToday.Checked)
                    {
                        dteDateFrom.Enabled = dteDateTo.Enabled = false;
                    }
                }
                
            }
            
        }

        private void btnSearchDispatcher_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iDispatcher;
            frmSearchField.sHeader = "DISPATCHER";
            frmSearchField.sSearchChar = txtSearchDispatcher.Text;
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                txtDispatcherID.Text = clsSearch.ClassParticularID.ToString();
                txtSearchDispatcher.Text = clsSearch.ClassParticularName;               
            }
        }

        private void rbAll_CheckedChanged(object sender, EventArgs e)
        {
            initDateFilter(false);
        }

        private void cboSearchTerminalStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassTerminalStatus = 0;
            if (!cboSearchTerminalStatus.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Status List", cboSearchTerminalStatus.Text);
                clsSearch.ClassTerminalStatus = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassTerminalStatus=" + clsSearch.ClassTerminalStatus);
            }

            txtTerminalStatus.Text = clsSearch.ClassTerminalStatus.ToString();
        }

        private void btnSearchService_Click(object sender, EventArgs e)
        {   
            frmSearchField.iSearchType = frmSearchField.SearchType.iService;
            frmSearchField.sHeader = "SEARCH SERVICE";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsParticular.ClassParticularID = clsSearch.ClassMerchantID = clsSearch.ClassParticularID;
                clsParticular.ClassParticularName = clsSearch.ClassMerchantName = clsSearch.ClassParticularName;

                txtClientID.Text = clsSearch.ClassClientID.ToString();
                txtMerchantID.Text = clsSearch.ClassParticularID.ToString();
                txtSearchMerchant.Text = clsParticular.ClassParticularName;
                txtSearchTID.Text = clsSearch.ClassTID;
                txtSearchMID.Text = clsSearch.ClassMID;

                txtIRIDNo.Text = clsSearch.ClassIRIDNo.ToString();
                txtIRNo.Text = clsSearch.ClassIRNo;
                txtServiceNo.Text = clsSearch.ClassServiceNo.ToString();
                txtFSRNo.Text = clsSearch.ClassFSRNo.ToString();

                //// get client info
                ////cboSearchClient.Text = clsFunction.sDefaultSelect;
                //if (dbFunction.isValidID(txtClientID.Text))
                //{
                //    dbAPI.ExecuteAPI("GET", "Search", "Client Info", txtClientID.Text, "Get Info Detail", "", "GetInfoDetail");
                //    if (dbAPI.isNoRecordFound() == false)
                //    {
                //        cboSearchClient.Text = clsSearch.ClassClientName = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                //    }
                //}                
            }
        }

        private void btnSearchReason_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iAllReason;
            frmSearchField.sHeader = "REASON";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                txtReasonID.Text = clsSearch.ClassReasonID.ToString();
                txtReason.Text = txtReason.Text = clsSearch.ClassReasonDescription;                
            }
        }

        private void chkDetailTab_CheckedChanged(object sender, EventArgs e)
        {
            gbDetailDateFilter.Enabled = true;

            if (chkDetailTab.Checked)
                gbDetailDateFilter.Enabled = false;
        }

        private void rbDetailAll_CheckedChanged(object sender, EventArgs e)
        {
            initDateFilter(true);
        }

        private void rbDetailToday_CheckedChanged(object sender, EventArgs e)
        {
            initDateFilter(true);
            InitDateRange();
        }

        private void rbDetailRange_CheckedChanged(object sender, EventArgs e)
        {
            initDateFilter(true);
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void chkSummaryTab_CheckedChanged(object sender, EventArgs e)
        {
            gbDetailDateFilter.Enabled = true;

            if (chkSummaryTab.Checked)
                gbDetailDateFilter.Enabled = false;
        }
    }
}
