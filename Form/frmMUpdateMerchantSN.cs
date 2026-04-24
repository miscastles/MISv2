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
using MIS.Controller;

namespace MIS
{
    public partial class frmMUpdateMerchantSN : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;

        // Controller
        private IRDetailController _mIRDetailController;
        private ServicingDetailController _mServicingDetailController;

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

        public frmMUpdateMerchantSN()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwHistory, true);
            dbFunction.setDoubleBuffer(lvwTerminalSN, true);
            dbFunction.setDoubleBuffer(lvwSIMSN, true);

            // Initialize the controller object
            _mIRDetailController = new IRDetailController();
            _mServicingDetailController = new ServicingDetailController();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
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
                Cursor.Current = Cursors.WaitCursor;

                txtMerchantID.Text = clsSearch.ClassParticularID.ToString();
                txtSearchMerchantName.Text = clsSearch.ClassParticularName;                
                txtIRIDNo.Text = clsSearch.ClassIRIDNo.ToString();

                if (dbFunction.isValidID(txtMerchantID.Text) && dbFunction.isValidID(txtIRIDNo.Text))
                {
                    _mIRDetailController = _mIRDetailController.getMerchantInfo($"{txtMerchantID.Text}{clsDefines.gPipe}{txtIRIDNo.Text}");

                    if (_mIRDetailController != null)
                    {
                        ucMerchantInfo.loadMerchant(_mIRDetailController);
                    }

                    _mIRDetailController = _mIRDetailController.getInfo($"{txtIRIDNo.Text}");
                    if (_mIRDetailController != null)
                    {
                        ucIRInfo.loadData(_mIRDetailController);

                        txtClientID.Text = _mIRDetailController.ClientID.ToString();
                        txtTerminalID.Text = _mIRDetailController.TerminalID.ToString();
                        txtSIMID.Text = _mIRDetailController.SIMID.ToString();
                    }

                    // fill service history
                    clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        _mIRDetailController.TID + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();

                    dbAPI.FillListViewAServiceDispatch(lvwHistory, "View", "Dispatch Servicing 2", clsSearch.ClassAdvanceSearchValue);

                    dbAPI.viewTerminalHistory(lvwTerminalSN, _mIRDetailController.TerminalID.ToString());

                    dbAPI.viewSIMHistory(lvwSIMSN, _mIRDetailController.SIMID.ToString());

                    btnSave.Enabled = btnRemoveSNByStatus.Enabled = false;
                    btnRemoveSNFromMerchant.Enabled = true;
                    tabService.SelectedIndex = 0;
                }
                
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ClearListViewItems(lvwHistory);
            dbFunction.ClearListViewItems(lvwTerminalSN);
            dbFunction.ClearListViewItems(lvwSIMSN);

            btnSearchMerchant.Enabled = true;            
            dbFunction.SetButtonIconImage(btnSearchMerchant);
           
            btnSave.Enabled = btnRemoveSNByStatus.Enabled = btnRemoveSNFromMerchant.Enabled = false;

            cboLocation.Text = cboStatus.Text = clsFunction.sDefaultSelect;

            btnSearchMerchant.Focus();
        }

        private void frmMUpdateMerchantSN_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {   
                case Keys.F2: // Search                  
                        btnSearchMerchant_Click(this, e);
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void frmMUpdateMerchantSN_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            
            chckDeployed_CheckedChanged(this, e);

            InitListView();

            btnSave.Enabled = btnRemoveSNFromMerchant.Enabled = false;

            Cursor.Current = Cursors.Default;
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
                txtNewTerminalID.Text = clsSearch.ClassTerminalID.ToString();
                txtTerminalSN.Text = clsSearch.ClassTerminalSN;

                if (dbFunction.isValidID(txtNewTerminalID.Text))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "Terminal SN Info", txtNewTerminalID.Text, "Get Info Detail", "", "GetInfoDetail");

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);
                        
                        txtTerminalType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                        txtTerminalModel.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                        txtTerminalStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5);
                        txtTerminalLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);
                        
                    }
                }
                
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
                txtNewSIMID.Text = clsSearch.ClassSIMID.ToString();
                txtSIMSN.Text = clsSearch.ClassSIMSerialNo;

                if (dbFunction.isValidID(txtNewSIMID.Text))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "SIM SN Info", txtNewSIMID.Text, "Get Info Detail", "", "GetInfoDetail");

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);
                        
                        txtSIMCarrier.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                        txtSIMStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                        txtSIMLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);                        
                    }
                }

            }
        }

        private void InitListView()
        {
            string outField = "";
            int outWidth = 0;
            string outTitle = "";
            HorizontalAlignment outAlign = 0;
            bool outVisible = false;
            bool outAutoWidth = false;
            string outFormat = "";
            int iFormWidth = 0;

            dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "ServiceNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "ClientID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "MerchantID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "FEID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "Name", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "TID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "MID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "Client Name", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "FE Name", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "IRIDNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "IR No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "Service No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "Request Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "Job Type", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "Reference No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "Status", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "Service Result", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "FSRNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            outWidth = 0;
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "ServiceNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            outWidth = 0;
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "TerminalSN", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "SIMSN", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "ReplaceTerminalSN", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "ReplaceSIMSN", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "Schedule Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

            dbFunction.GetListViewHeaderColumnFromFile("", "Serviced Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwHistory.Columns.Add(outTitle, outWidth, outAlign);
            iFormWidth += outWidth;

        }

        private void lvwHistory_DoubleClick(object sender, EventArgs e)
        {
            if (lvwHistory.Items.Count > 0)
            {
                string pSelectedRow = dbFunction.GetListViewSelectedRow(lvwHistory, 0);
                Debug.WriteLine("pSelectedRow=\n" + pSelectedRow);

                // Fill update service control#
                txtServiceNo.Text = lvwHistory.SelectedItems[0].SubItems[1].Text;
                txtIRNo.Text = lvwHistory.SelectedItems[0].SubItems[11].Text;
                txtSvcTerminalSN.Text = lvwHistory.SelectedItems[0].SubItems[22].Text;
                txtSvcSIMSN.Text = lvwHistory.SelectedItems[0].SubItems[23].Text;

                txtJobType.Text = lvwHistory.SelectedItems[0].SubItems[14].Text;
                txtServiceStatus.Text = lvwHistory.SelectedItems[0].SubItems[18].Text;
                txtServiceResult.Text = lvwHistory.SelectedItems[0].SubItems[19].Text;

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string pTemp = "";

            // check to override SN
            if (dbFunction.isValidID(txtTerminalID.Text))
            {
                pTemp = $"Terminal SN:{_mIRDetailController.TerminalSN}\nSIM SN:{_mIRDetailController.SIMSN}\n\nare marked as an [OVERRIDE].\n\nDo you want to continue?";
                if (!dbFunction.fPromptConfirmation(pTemp)) return;
            }
            else // check to assigned SN
            {
                pTemp = $"Terminal SN:{_mIRDetailController.TerminalSN}\nSIM SN:{_mIRDetailController.SIMSN}\n\nare not yet [ASSIGNED].\n\nDo you want to [ASSIGNED] it now?";
                if (!dbFunction.fPromptConfirmation(pTemp)) return;
            }
            
            if (!ValidateFields()) return;

            pTemp = $"Are you sure to update the following:\n\n[Service]\nJobType: {txtJobType.Text}\nRequest ID: {txtIRNo.Text}\n\n" +
                $"[Merchant]\nName: {txtSearchMerchantName.Text}\nTID: {_mIRDetailController.TID}\nMID: {_mIRDetailController.MID}\n\n" +
                $"[Terminal]\nSN: {txtTerminalSN.Text}\nType: {txtTerminalType.Text}\nModel: {txtTerminalModel.Text}\n\n" +
                $"[SIM]\nSIM SN: {txtSIMSN.Text}\nCarrier: {txtSIMCarrier.Text}\n\nLocation: {cboLocation.Text}\nStatus: {cboStatus.Text}\n\n" +
                $"Merchant SN will be {(chckDeployed.Checked ? "deployed" : "returned")};";

            if (!dbFunction.fPromptConfirmation(pTemp)) return;

            string pSearchValue =  $"{clsDefines.Mode_Type_Deploy}{clsDefines.gPipe}" +
                                    $"{dbFunction.CheckAndSetNumericValue(txtClientID.Text)}{clsDefines.gPipe}" +
                                    $"{dbFunction.CheckAndSetNumericValue(txtServiceNo.Text)}{clsDefines.gPipe}" +
                                    $"{dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text)}{clsDefines.gPipe}" +
                                    $"{dbFunction.CheckAndSetNumericValue(txtMerchantID.Text)}{clsDefines.gPipe}" +
                                    $"{dbFunction.CheckAndSetNumericValue(txtNewTerminalID.Text)}{clsDefines.gPipe}" +
                                    $"{dbFunction.CheckAndSetNumericValue(txtNewSIMID.Text)}{clsDefines.gPipe}" +
                                    $"{dbFunction.CheckAndSetNumericValue(txtLocationID.Text)}{clsDefines.gPipe}" +                               
                                    $"{dbFunction.CheckAndSetNumericValue(txtStatusID.Text)}";

            dbFunction.parseDelimitedString(pSearchValue, clsDefines.gPipe, 0);

            dbAPI.ExecuteAPI("PUT", "Update", "Merchant SN-Mode Type", pSearchValue, "", "", "UpdateCollectionDetail");

            dbFunction.SetMessageBox("Update merchant SN complete.", "Update", clsFunction.IconType.iInformation);

            btnClear_Click(this, e);
        }

        private void cboLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtLocationID.Text = clsFunction.sZero;
            if (!cboLocation.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Location", cboLocation.Text);
                txtLocationID.Text = clsSearch.ClassOutFileID.ToString();

            }            
        }

        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtStatusID.Text = clsFunction.sZero;
            if (!cboStatus.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Status List", cboStatus.Text);
                txtStatusID.Text = clsSearch.ClassOutFileID.ToString();
            }
        }

        private bool ValidateFields()
        {
            if (!dbFunction.isValidDescriptionEntry(txtClientID.Text, "Client ID" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtServiceNo.Text, "Service No." + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtIRIDNo.Text, "IRID No." + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtSearchMerchantName.Text, "Merchant name" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtLocationID.Text, "SN Location" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtStatusID.Text, "SN Status" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtServiceResult.Text, "Service Result" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtServiceStatus.Text, "Service Status" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;

            if (dbFunction.isValidDescription(txtSvcTerminalSN.Text))
            {
                if (!dbFunction.isValidDescriptionEntry(txtNewTerminalID.Text, "Update terminal SN" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            }

            if (dbFunction.isValidDescription(txtSvcSIMSN.Text))
            {
                if (!dbFunction.isValidDescriptionEntry(txtNewSIMID.Text, "Update SIM SN" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            }

            if (!txtServiceStatus.Text.Equals(clsGlobalVariables.STATUS_INSTALLED_DESC))
            {
                dbFunction.SetMessageBox($"Service status must be [{clsGlobalVariables.STATUS_INSTALLED_DESC}]", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return false;
            }

            if (!txtServiceResult.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS))
            {
                dbFunction.SetMessageBox($"Service result must be [{clsGlobalVariables.ACTION_MADE_SUCCESS}]", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return false;
            }

            if (dbFunction.isValidDescription(txtTerminalSN.Text) && dbFunction.isValidDescription(txtSvcTerminalSN.Text))
            {
                if (txtTerminalSN.Text.Equals(txtSvcTerminalSN.Text))
                {
                    dbFunction.SetMessageBox("Unable to update, Terminal SN are the same.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return false;
                }
            }

            /*
            if (dbFunction.isValidDescription(txtSIMSN.Text) && dbFunction.isValidDescription(txtSvcSIMSN.Text))
            {
                if (txtSIMSN.Text.Equals(txtSvcSIMSN.Text))
                {
                    dbFunction.SetMessageBox("Unable to update, SIM SN are the same.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return false;
                }
            }
            */

            if (txtJobType.Text.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
            {
                dbFunction.SetMessageBox($"Update is applicable for [{clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC}]", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return false;
            }

            return true;
        }

        private void chckDeployed_CheckedChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = btnRemoveSNByStatus.Enabled = false;

            if (dbFunction.isValidID(txtIRIDNo.Text))
            {
                if (chckDeployed.Checked)
                {
                    btnSave.Enabled = true;
                    dbAPI.LoadDeployedLocation(cboLocation);
                    dbAPI.LoadDeployedStatus(cboStatus);
                }
                else
                {
                    if (dbFunction.isValidID(txtTerminalID.Text))
                        btnRemoveSNByStatus.Enabled = true;

                    dbAPI.LoadReturnedLocation(cboLocation);
                    dbAPI.LoadReturnedStatus(cboStatus);
                }
            }
            
        }

        private void btnRemoveSNByStatus_Click(object sender, EventArgs e)
        {
            string pTemp = "";

            if (!dbFunction.isValidDescriptionEntry(txtClientID.Text, "Client ID" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtIRIDNo.Text, "IRID No." + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtSearchMerchantName.Text, "Merchant name" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtLocationID.Text, "SN Location" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtStatusID.Text, "SN Status" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            
            if (dbAPI.isRecordExist("Search", "Merchant Assign SN", dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text)))
            {
                dbFunction.SetMessageBox("This merchant has an active SN assigned.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);
                return;
            }

            pTemp = $"This will remove the serial number assigned from the merchant [{txtSearchMerchantName.Text}]";
            if (!dbFunction.fPromptConfirmation(pTemp)) return;

            pTemp = $"Are you sure to update the following:\n\n[Installation]\nRequest ID: {_mIRDetailController.IRNo}\n\n" +
                $"[Merchant]\nName: {txtSearchMerchantName.Text}\nTID: {_mIRDetailController.TID}\nMID: {_mIRDetailController.MID}\n\n" +
                $"[Terminal]\nSN: {_mIRDetailController.TerminalSN}\nType: {_mIRDetailController.TerminalType}\nModel: {_mIRDetailController.TerminalModel}\n\n" +
                $"[SIM]\nSIM SN: {_mIRDetailController.SIMSN}\nCarrier: {_mIRDetailController.SIMCarrier}\n\nLocation: {cboLocation.Text}\nStatus: {cboStatus.Text}\n\n" +
                $"Merchant SN will be {(chckDeployed.Checked ? "deployed" : "returned")}";

            if (!dbFunction.fPromptConfirmation(pTemp)) return;

            string pSearchValue = $"{clsDefines.Mode_Type_Return}{clsDefines.gPipe}" +
                                    $"{dbFunction.CheckAndSetNumericValue(txtClientID.Text)}{clsDefines.gPipe}" +
                                    $"{clsDefines.gZero}{clsDefines.gPipe}" +
                                    $"{dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text)}{clsDefines.gPipe}" +
                                    $"{dbFunction.CheckAndSetNumericValue(txtMerchantID.Text)}{clsDefines.gPipe}" +
                                    $"{dbFunction.CheckAndSetNumericValue(txtTerminalID.Text)}{clsDefines.gPipe}" +
                                    $"{dbFunction.CheckAndSetNumericValue(txtSIMID.Text)}{clsDefines.gPipe}" +
                                    $"{dbFunction.CheckAndSetNumericValue(txtLocationID.Text)}{clsDefines.gPipe}" +
                                    $"{dbFunction.CheckAndSetNumericValue(txtStatusID.Text)}";

            dbFunction.parseDelimitedString(pSearchValue, clsDefines.gPipe, 0);

            dbAPI.ExecuteAPI("PUT", "Update", "Merchant SN-Mode Type", pSearchValue, "", "", "UpdateCollectionDetail");

            dbFunction.SetMessageBox("Remove merchant SN assignment complete.", "Update", clsFunction.IconType.iInformation);

            btnClear_Click(this, e);


        }

        private void btnRemoveSNFromMerchant_Click(object sender, EventArgs e)
        {
            string pTemp = "";

            if (!dbFunction.isValidDescriptionEntry(txtClientID.Text, "Client ID" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtIRIDNo.Text, "IRID No." + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtSearchMerchantName.Text, "Merchant name" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            
            pTemp = $"This will remove the serial number assigned from the merchant [{txtSearchMerchantName.Text}]";
            if (!dbFunction.fPromptConfirmation(pTemp)) return;

            string pSearchValue = 
                                    $"{dbFunction.CheckAndSetNumericValue(txtClientID.Text)}{clsDefines.gPipe}" +                                  
                                    $"{dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text)}{clsDefines.gPipe}" +
                                    $"{dbFunction.CheckAndSetNumericValue(txtMerchantID.Text)}{clsDefines.gPipe}" +
                                    $"{dbFunction.CheckAndSetNumericValue(txtTerminalID.Text)}{clsDefines.gPipe}" +
                                    $"{dbFunction.CheckAndSetNumericValue(txtSIMID.Text)}";

            dbFunction.parseDelimitedString(pSearchValue, clsDefines.gPipe, 0);

            dbAPI.ExecuteAPI("PUT", "Update", "Remove Merchant SN Assigned", pSearchValue, "", "", "UpdateCollectionDetail");

            dbFunction.SetMessageBox("Remove merchant SN assignment complete.", "Update", clsFunction.IconType.iInformation);

            btnClear_Click(this, e);

        }
    }
}
