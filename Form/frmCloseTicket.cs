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
    public partial class frmCloseTicket : Form
    {
        private clsAPI dbAPI;
        private clsINI dbSystem;
        private clsInternet dbInternet;
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

        public frmCloseTicket()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwList, true);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmCloseTicket_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F2:
                    btnSearchMerchant_Click(this, e);
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void frmCloseTicket_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            dbInternet = new clsInternet();
            dbSystem = new clsINI();

            dbSystem.InitAPISetting();

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ClearComboBox(this);
            dbFunction.ClearListViewItems(lvwList);
            
            dbAPI.FillComboBoxYesNo(cboBillable);
            dbAPI.FillComboBoxYesNo(cboStatus);

            dbAPI.FillComboBoxPositionType(cboFEName, clsDefines.FIELD_ENGINEER_POSITION_TYPE);
            dbAPI.FillComboBoxPositionType(cboDispatcher, clsDefines.DISPATCHER_POSITION_TYPE);
            dbAPI.FillComboBoxDepedency(cboDependency);
            dbAPI.FillComboBoxStatusReason(cboStatusReason);

            // dependency
            dbAPI.FillComboBoxDepedency(cboDependency);
            dbAPI.FillComboBoxStatusReason(cboStatusReason);

            btnOpenManualFSR.Enabled = btnApply.Enabled = false;

            // Init user control
            ucStatus.iState = 3;
            ucStatus.sMessage = "-";

            iniComboBoxSelection(true);

            btnClear.Focus();

            Cursor.Current = Cursors.Default;
        }

        private void loadData(ListView obj)
        {
            int i = 0;
            int iLineNo = 0;
            bool isDiagnostic = false;
            bool isExist = false;
            string pFileName = "";

            Cursor.Current = Cursors.WaitCursor;

            dbAPI.ExecuteAPI("GET", "View", "Merchant Service Status List", 
                dbFunction.CheckAndSetNumericValue(txtSearchMerchantID.Text) + clsDefines.gPipe + 
                dbFunction.CheckAndSetNumericValue(txtFEID.Text) + clsDefines.gPipe +
                dbFunction.CheckAndSetNumericValue(txtDispatcherID.Text), "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.ID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                    // Display detail info
                    string pFSRNo = dbFunction.CheckAndSetNumericValue(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FSRNO));
                    string pServiceNo = dbFunction.CheckAndSetNumericValue(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_SERVICENO));
                    string pIRIDNo = dbFunction.CheckAndSetNumericValue(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_IRIDNo));
                    string pIRNo = dbFunction.CheckAndSetNumericValue(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_IRNO));
                    string pMerchantID = dbFunction.CheckAndSetNumericValue(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MerchantID));

                    item.SubItems.Add(pFSRNo);
                    item.SubItems.Add(pServiceNo);
                    item.SubItems.Add(pIRIDNo);
                    item.SubItems.Add(pMerchantID);
                    item.SubItems.Add(pIRNo);
                    
                    string pJSONString = dbAPI.getInfoDetailJSON("Search", "Merchant Servicing Info", pServiceNo + clsDefines.gPipe + pIRIDNo);

                    dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                    string pMerchantName = dbFunction.CheckAndSetNumericValue(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MERCHANTNAME));

                    item.SubItems.Add(pMerchantName);
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TID));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MID));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ServiceJobTypeDescription));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FEName));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_DispatchBy));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ServiceCreatedDate));                    
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ScheduleDate));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_RequestDate));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FSRDate));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ActionMade));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Reason));

                    string pFSRMode = (dbFunction.isValidID(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MobileID)) ? clsDefines.DIGITAL_FSR : clsDefines.MANUAL_FSR) + clsDefines.gPipe + dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MobileVersion);
                    item.SubItems.Add(pFSRMode);

                    //string isDiagnostic = (dbFunction.isValidID(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_isDiagnostic)) ? clsDefines.gYes : clsDefines.gNo);
                    //item.SubItems.Add(isDiagnostic);
                    // Diagnostic
                    if (dbAPI.isRecordExist("Search", "Diagnostic Detail", pServiceNo + clsDefines.gPipe + pFSRNo))
                        isDiagnostic = true;
                    else
                        isDiagnostic = false;

                    item.SubItems.Add(dbFunction.setIntegerToYesNoString(isDiagnostic ? 1 : 0));

                    string isBillable = (dbFunction.isValidID(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_isBillable)) ? clsDefines.gYes : clsDefines.gNo);
                    item.SubItems.Add(isBillable);

                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_RequestNo));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_REFERENCENO));

                    // Merchant Signature
                    pFileName = pServiceNo + "_" + dbFunction.padLeftChar(clsDefines.MERCHANT_SIGNATURE_INDEX.ToString(), "0", 2) + clsDefines.FILE_EXT_PNG;
                    if (dbAPI.isFileExist("Search", "Check Upload File", pFileName))
                        isExist = true;
                    else
                        isExist = false;

                    item.SubItems.Add(dbFunction.setIntegerToYesNoString(isExist ? 1 : 0));

                    obj.Items.Add(item);

                    ucStatus.iState = 3;                    
                    ucStatus.sMessage = "Line#: " + iLineNo + ", Request ID: " + pIRNo + ", Mercahnt: " + pMerchantName;
                    ucStatus.AnimateStatus();

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }

            initCountHeader();

            ucStatus.sMessage = clsFunction.sDash;

            Cursor.Current = Cursors.Default;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {

            Cursor.Current = Cursors.WaitCursor;

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);            
            dbFunction.ClearListViewItems(lvwList);
            
            btnOpenManualFSR.Enabled = btnApply.Enabled = false;

            iniComboBoxSelection(true);

            btnSearchNow.Focus();

            Cursor.Current = Cursors.Default;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantName, txtMerchantName.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iClientID, txtClientID.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iFEName, txtVendorRepName.Text)) return;
            
            if (!dbFunction.fPromptConfirmation("Selected merchant information to Manual FSR Window" + "\n\n" +
                    " > Line# :" + dbFunction.GetSearchValue("Line#") + "\n" +
                    " > Request ID :" + dbFunction.GetSearchValue("RequestID") + "\n" +
                    " > Name :" + dbFunction.GetSearchValue("Merchant Name") + "\n" +
                    " > TID :" + dbFunction.GetSearchValue("TID") + "\n" +
                    " > MID :" + dbFunction.GetSearchValue("MID") + "\n" +
                    " > Job Type :" + dbFunction.GetSearchValue("Service Job Type") + "\n" +
                    " > Field Engineer :" + dbFunction.GetSearchValue("Field Engineer") + "\n" +
                    " > Dispatcher :" + dbFunction.GetSearchValue("Dispatcher") + "\n" +
                    " > Diagnostic :" + dbFunction.GetSearchValue("Diagnostic?") + "\n" +
                    " > Billable :" + dbFunction.GetSearchValue("Billable") + "\n" +
                    "\n\n" +
                    "Are you sure you want to continue?")) return;

            try
            {
                btnOpenManualFSR.Enabled = btnApply.Enabled = true;

                // set fields
                clsSearch.ClassServiceNo = int.Parse(txtServiceNo.Text);
                clsSearch.ClassFSRNo = int.Parse(txtFSRNo.Text);
                clsSearch.ClassParticularName = txtMerchantName.Text;
                clsSearch.ClassClientID = int.Parse(txtClientID.Text);
                clsSearch.ClassMerchantID = int.Parse(txtMerchantID.Text);
                clsSearch.ClassFEID = int.Parse(txtFEID.Text);
                clsSearch.ClassTID = txtIRTID.Text;
                clsSearch.ClassMID = txtIRMID.Text;
                clsSearch.ClassIRIDNo = int.Parse(txtIRIDNo.Text);
                clsSearch.ClassIRNo = txtIRNo.Text;

                //dbFunction.SetMessageBox("Opening FSR window with:" + "\n\n" +
                //    " > Service No. :" + dbFunction.AddBracketStartEnd(clsSearch.ClassServiceNo.ToString()) + "\n" +
                //    " > Request ID :"  + dbFunction.AddBracketStartEnd(clsSearch.ClassIRNo) + "\n"+
                //    " > Merchant Name :" + dbFunction.AddBracketStartEnd(clsSearch.ClassParticularName) + "\n" +
                //    " > TID :" + dbFunction.AddBracketStartEnd(clsSearch.ClassTID) + "\n" +
                //    " > MID :" + dbFunction.AddBracketStartEnd(clsSearch.ClassMID), "Open window", clsFunction.IconType.iInformation);

                frmTerminalFSR.fAutoLoadData = true;
                frmTerminalFSR frm = new frmTerminalFSR();
                frm.Text = "FSR";
                dbFunction.handleForm(frm);

            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox("Exception error " + ex.Message, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }
            
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.Items.Count > 0)
            {
                string pSelectedRow = clsSearch.ClassRowSelected = dbFunction.GetListViewSelectedRow(lvwList, 0);
                Debug.WriteLine("pSelectedRow=\n" + pSelectedRow);

                if (!dbFunction.GetSearchValue("Dispatcher").Equals(clsSearch.ClassCurrentParticularName))
                {
                    if (!dbFunction.fPromptConfirmation("This service is assigned by " + dbFunction.AddBracketStartEnd(dbFunction.GetSearchValue("Dispatcher")) + "." + "\n" +
                    "\n\n" +
                    "Are you sure to proceed?")) return;
                }
                
                if (!dbFunction.fPromptConfirmation("Selected merchant information" + "\n\n" +
                    " > Line# :" + dbFunction.GetSearchValue("Line#") + "\n" +
                    " > Name :" + dbFunction.GetSearchValue("Merchant Name") + "\n" +
                    " > TID :" + dbFunction.GetSearchValue("TID") + "\n" +
                    " > MID :" + dbFunction.GetSearchValue("MID") + "\n" +
                    " > Job Type :" + dbFunction.GetSearchValue("Service Job Type") + "\n" +
                    " > Field Engineer :" + dbFunction.GetSearchValue("Field Engineer") + "\n" +
                    " > Dispatcher :" + dbFunction.GetSearchValue("Dispatcher") + "\n" +
                    clsFunction.sLineSeparator + "\n" +
                    " > Unique ID :" + dbFunction.GetSearchValue("UniqueID") + "\n" +
                    " > Request ID :" + dbFunction.GetSearchValue("RequestID") + "\n" +
                    " > Reference No :" + dbFunction.GetSearchValue("ReferenceNo") + "\n" +
                    "\n\n" +
                    "Are you sure you want to continue?")) return;

                txtLineNo.Text = dbFunction.GetSearchValue("LINE#");
                txtFSRNo.Text = dbFunction.GetSearchValue("FSRNo");
                txtServiceNo.Text = dbFunction.GetSearchValue("ServiceNo");
                txtIRIDNo.Text = dbFunction.GetSearchValue("IRIDNo");
                txtMerchantID.Text = dbFunction.GetSearchValue("MerchantID");
                
                loadServiceInfo();

                loadTATDetail();

                cboStatus.Text = clsFunction.sDefaultSelect;

                btnOpenManualFSR.Enabled = btnApply.Enabled = true;

                btnClear.Focus();
            }
        }

        private void loadServiceInfo()
        {
            string pJSONString = dbAPI.getInfoDetailJSON("Search", "Merchant Servicing Info", txtServiceNo.Text + clsDefines.gPipe + txtIRIDNo.Text);

            dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

            txtServiceJobTypeDescription.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ServiceJobTypeDescription);
            txtMerchantName.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MERCHANTNAME);
            txtMerchantAddress.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Address);
            txtMerchantCity.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Province);
            txtMerchantRegion.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_REGION);
            txtAppVersion.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_AppVersion);
            txtAppCRC.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_AppCRC);
            txtIRTID.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TID);
            txtIRMID.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MID);
            txtFEID.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FEID);
           
            //txtCurTerminalSN.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalSN);
            //txtCurTerminalType.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalType);
            //txtCurTerminalModel.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalModel);
            //txtCurSIMSN.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SIMSN);
            //txtCurSIMCarrier.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SIMCarrier);

            //txtRepTerminalSN.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReplaceTerminalSN);
            //txtRepTerminalType.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReplaceTerminalType);
            //txtRepTerminalModel.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReplaceTerminalModel);
            //txtRepSIMSN.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReplaceSIMSN);
            //txtRepSIMCarrier.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReplaceSIMCarrier);

            txtReqInstallationDate.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_RequestDate);
            txtServiceReqDate.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ScheduleDate);
            txtCreatedDate.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ServiceCreatedDate);
            
            txtIRNo.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_IRNO);
            txtRequestNo.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_RequestNo);
            txtReferenceNo.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_REFERENCENO);
            txtServiceStatus.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_JobTypeStatusDescription);

            txtMFSRDate.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FSRDate);
            txtMReceiptTime.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FSRTime);
            txtMTimeArrived.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TimeArrived);
            txtMTimeStart.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TimeStart);
            txtMTimeEnd.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TimeEnd);           
            txtServiceResult.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ActionMade);

            if (txtServiceResult.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS))
                txtServiceResult.ForeColor = Color.Green;
            else if (txtServiceResult.Text.Equals(clsGlobalVariables.ACTION_MADE_NEGATIVE))
                txtServiceResult.ForeColor = Color.Red;
            else
                txtServiceResult.ForeColor = Color.Black;

            txtReason.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Reason);

            txtVendorRepName.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FEName);
            txtDispatcher.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_DispatchBy);
            //txtVendorRepPosition.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Position);
            txtVendorRepEmail.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Email);

            txtMerchRepName.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MerchantRepresentative);
            //txtMerchRepPosition.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MerchantRepresentativePosition);
            txtMerchRepEmail.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MerchantRepresentativeEmail);

            //txtMobileID.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MobileID);

            txtClientID.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ClientID);
            txtClientName.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ClientName);
            txtClientEmail.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ClientEmail);

            string isBillable = (dbFunction.isValidID(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_isBillable)) ? clsDefines.gYes : clsDefines.gNo);
            //cboBillable.Text = dbFunction.GetSearchValue("Billable");
            cboBillable.Text = isBillable;

            cboDependency.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Dependency);
            cboStatusReason.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_StatusReason);

        }
        
        private void initCountHeader()
        {
            string pHeader = "SERVICE INFORMATION";

            if (lvwList.Items.Count > 0)
                lblCountHeader.Text = pHeader + dbFunction.AddParenthesisStartEnd(lvwList.Items.Count.ToString());
            else
                lblCountHeader.Text = pHeader;
        }

        private void cboDispatcher_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassDispatcherID = 0;
            if (!cboDispatcher.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("FE List", cboDispatcher.Text);
                clsSearch.ClassDispatcherID = clsSearch.ClassOutFileID;               
            }

            txtDispatcherID.Text = clsSearch.ClassDispatcherID.ToString();
        }

        private void cboFEName_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassFEID = 0;
            if (!cboFEName.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("FE List", cboFEName.Text);
                clsSearch.ClassFEID = clsSearch.ClassOutFileID;
            }

            txtFEID.Text = clsSearch.ClassFEID.ToString();
        }

        private void btnSearchMerchant_Click(object sender, EventArgs e)
        {

            frmSearchField.iSearchType = frmSearchField.SearchType.iMerchant;
            frmSearchField.sHeader = "MERCHANT";
            frmSearchField.isCheckBoxes = false;
            frmSearchField.isPreview = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                txtSearchMerchantID.Text = clsSearch.ClassParticularID.ToString();
                txtSearchMerchantName.Text = clsSearch.ClassParticularName;
            }

        }

        private void btnSearchNow_Click(object sender, EventArgs e)
        {   
            dbFunction.ClearListViewItems(lvwList);

            cboDispatcher_SelectedIndexChanged(this, e);

            cboFEName_SelectedIndexChanged(this, e);

            // reaset fields
            clsSearch.ClassParticularID = int.Parse(dbFunction.CheckAndSetNumericValue(txtSearchMerchantID.Text));
            clsSearch.ClassParticularName = dbFunction.CheckAndSetStringValue(txtSearchMerchantName.Text);

            clsSearch.ClassFEID = int.Parse(dbFunction.CheckAndSetNumericValue(txtFEID.Text));
            clsSearch.ClassDispatcherID = int.Parse(dbFunction.CheckAndSetNumericValue(txtDispatcherID.Text));

            loadData(lvwList);
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            if (!dbFunction.isValidDescription(cboBillable.Text))
            {
                dbFunction.SetMessageBox("Select valid value for [BILLABLE]", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return;
            }

            if (!dbFunction.isValidDescription(cboStatus.Text))
            {
                dbFunction.SetMessageBox("Select valid value for [CLOSE SERVICE]", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return;
            }

            if (dbFunction.isValidID(txtFSRNo.Text) && dbFunction.isValidID(txtServiceNo.Text) && dbFunction.isValidID(txtIRIDNo.Text))
            {
                if (!dbFunction.fPromptConfirmation("Selected merchant information" + "\n\n" +
                    " > Line# :" + txtLineNo.Text + "\n" +
                    " > Name :" + txtMerchantName.Text + "\n" +
                    " > TID :" + txtIRTID.Text + "\n" +
                    " > MID :" + txtIRMID.Text + "\n" +
                    " > Job Type :" + txtServiceJobTypeDescription.Text + "\n" +
                    " > Field Engineer :" + txtVendorRepName.Text + "\n" +
                    " > Dispatcher :" + txtDispatcher.Text + "\n" +
                    clsFunction.sLineSeparator + "\n" +                 
                    " > Request ID :" + txtIRNo.Text + "\n" +
                    " > Reference No :" + txtReferenceNo.Text + "\n" +
                     clsFunction.sLineSeparator + "\n" +
                     " > Billable Status Selected :" + cboBillable.Text + "\n" +
                     " > Service Status Selected :" + cboStatus.Text + "\n" +
                     clsFunction.sLineSeparator + "\n" +
                     " > Dependency :" + cboDependency.Text + "\n" +
                     " > Status Reason :" + cboStatusReason.Text + "\n" +
                    "\n\n" +
                    "!!IMPORTANT!!" + "\n" +
                    "Are you sure you want to " + (cboStatus.Text.Equals(clsDefines.gYes) ? clsDefines.CLOSE_TICKET : clsDefines.OPEN_TICKET + "?"))) return;

                dbFunction.GetCurrentDateTime();

                int pBillable = (cboBillable.Text.Equals(clsDefines.gYes) ? 1 : 0);
                int pTicketStatus = (cboStatus.Text.Equals(clsDefines.gYes) ? 1 : 0);

                // ------------------------------------------------------------------------------------
                // Ticket Status
                // ------------------------------------------------------------------------------------
                if (dbFunction.isValidID(txtServiceNo.Text))
                {
                    clsSearch.ClassAdvanceSearchValue = txtIRIDNo.Text + clsDefines.gPipe +
                                                    txtServiceNo.Text + clsDefines.gPipe +
                                                    pTicketStatus + clsDefines.gPipe +
                                                    clsSearch.ClassCurrentParticularID + clsDefines.gPipe +
                                                    clsSearch.ClassCurrentDateTime + clsDefines.gPipe +
                                                    pBillable;

                    dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                    dbAPI.ExecuteAPI("PUT", "Update", "Ticket Status", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                    // ------------------------------------------------------------------------------------
                    // Dependency
                    // ------------------------------------------------------------------------------------
                    clsSearch.ClassAdvanceSearchValue = txtIRIDNo.Text + clsDefines.gPipe +
                        txtServiceNo.Text + clsDefines.gPipe +
                        dbFunction.getFileID(cboDependency, "All Type") + clsDefines.gPipe +
                        dbFunction.getFileID(cboStatusReason, "All Type");

                    dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                    dbAPI.ExecuteAPI("PUT", "Update", "Dependency", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                    dbFunction.SetMessageBox("Service status updated completed.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);

                    loadData(lvwList);
                }
            }
            else
            {
                dbFunction.SetMessageBox("No selected service to be update.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void loadTATDetail()
        {
            txtSLA.Text = txtNetworkDays.Text = txtDaysOverDue.Text = txtTATStatus.Text = clsFunction.sDash;

            dbAPI.getTATInfo(int.Parse(txtClientID.Text), int.Parse(txtIRIDNo.Text), int.Parse(txtServiceNo.Text));

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

        }

        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {   
            if (!cboStatus.Text.Equals(clsFunction.sDefaultSelect))
            {
                btnApply.Enabled = true;
            }
            
        }

        private void cboBillable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cboBillable.Text.Equals(clsFunction.sDefaultSelect))
            {
                btnApply.Enabled = true;
            }
        }

        private void iniComboBoxSelection(bool isEnable)
        {
            cboDispatcher.Text = cboFEName.Text = cboBillable.Text = cboStatus.Text = cboDependency.Text = cboStatusReason.Text = clsFunction.sDefaultSelect;
            cboDispatcher.Enabled = cboFEName.Enabled = cboBillable.Enabled = cboStatus.Enabled = cboDependency.Enabled = cboStatusReason.Enabled = isEnable;
        }
    }
}
