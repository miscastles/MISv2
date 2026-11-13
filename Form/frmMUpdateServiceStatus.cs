using MIS.Controller;
using MIS.Model;
using System;
using System.Windows.Forms;
using static MIS.Function.AppUtilities;

namespace MIS
{
    public partial class frmMUpdateServiceStatus : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;

        // Controller
        private ServicingDetailController _mServicingDetailController;

#if ENABLE_COMPOSITED
                protected override CreateParams CreateParams
                {
                    get
                    {
                        CreateParams cp = base.CreateParams;
                        cp.ExStyle |= 0x02000000;
                        return cp;
                    }
                }
#endif

        public frmMUpdateServiceStatus()
        {
            InitializeComponent();

            // Initialize the controller object
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

        private void frmMUpdateServiceStatus_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1: // Reload data
                    if (btnSearchService.Enabled)
                        btnSearchService_Click(this, e);
                    break;
                case Keys.F2: // Reload data
                    if (btnSearchService.Enabled)
                        btnSearchService_Click(this, e);
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private bool FetchServicingDetail()
        {
            bool isValid = false;

            modelSearch.HoldClear();

            _mServicingDetailController = _mServicingDetailController.getServicingInfo(
                $"{dbFunction.CheckAndSetNumericValue(clsSearch.ClassServiceNo.ToString())}{clsDefines.gPipe}" +
                $"{dbFunction.CheckAndSetNumericValue(clsSearch.ClassIRIDNo.ToString())}");

            if (_mServicingDetailController == null || _mServicingDetailController.ServiceNo <= 0)
            {
                dbFunction.SetMessageBox("Servicing data not found.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
                return isValid;
            }

            modelSearch.HoldData.Save(new modelSearch.HoldData
            {
                TerminalID = _mServicingDetailController.TerminalID,
                SIMID = _mServicingDetailController.SIMID,

                ReplaceTerminalID = _mServicingDetailController.ReplaceTerminalID,
                ReplaceSIMID = _mServicingDetailController.ReplaceSIMID
            });

            txtJobType.Text = dbFunction.toStringAndTrim(_mServicingDetailController.JobType);
            txtServiceNo.Text = dbFunction.toStringAndTrim(_mServicingDetailController.ServiceNo);
            txtFSRNo.Text = dbFunction.toStringAndTrim(_mServicingDetailController.FSRNo);
            txtIRIDNo.Text = dbFunction.toStringAndTrim(_mServicingDetailController.IRIDNo);
            txtMerchantID.Text = dbFunction.toStringAndTrim(_mServicingDetailController.MerchantID);
            txtJobTypeDesc.Text = dbFunction.toStringAndTrim(_mServicingDetailController.ServiceJobTypeDescription);
            txtServiceStatus.Text = dbFunction.toStringAndTrim(_mServicingDetailController.ServiceStatus);

            clsSearch.ClassJobType = _mServicingDetailController.JobType;
            clsSearch.ClassHoldServiceStatus = _mServicingDetailController.ServiceStatus;
            txtServiceStatusDescription.Text = clsSearch.ClassHoldServiceStatusDescription = _mServicingDetailController.ServiceStatusDescription;

            txtSearchIRNo.Text = _mServicingDetailController.IRNo;

            ucMerchantInfo.loadData(_mServicingDetailController);
            ucServiceInfo.loadData(_mServicingDetailController);
            ucFSRInfo.loadData(_mServicingDetailController);
            ucCurrentSNInfo.loadData(_mServicingDetailController);
            ucReplaceSNInfo.loadData(_mServicingDetailController);
            ucMerchantRepInfo.loadData(_mServicingDetailController);
            ucVendorRepInfo.loadData(_mServicingDetailController);

            txtRemarks.ReadOnly = false;
            txtRemarks.Text = _mServicingDetailController.Remarks;

            LoadRequestHistory();

            return isValid = true;
        }

        private void btnSearchService_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iService;
            frmSearchField.sHeader = "SEARCH SERVICE";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (!frmSearchField.fSelected) return;

            Cursor.Current = Cursors.WaitCursor;

            if (FetchServicingDetail())
            {
                cboServiceStatusCategory.Enabled = true;
                cboServiceStatusCategory_SelectedIndexChanged(this, e);
                cboServiceStatusCategory.Focus();
            }
            else
            {
                dbFunction.SetMessageBox("No record found.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
            }

            Cursor.Current = Cursors.Default;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);

            btnSearchMerchant.Enabled = false;
            btnSearchService.Enabled = true;

            dbFunction.SetButtonIconImage(btnSearchMerchant);
            dbFunction.SetButtonIconImage(btnSearchService);

            cboServiceStatusCategory.Enabled = false;

            chkAvailable.Enabled = true;
            chkAvailable.Checked = false;

            lvwRequestIDHistory.Items.Clear();

            cboServiceStatusCategory.Text = clsFunction.sDefaultSelect;
            btnSearchService.Focus();
        }

        private void frmMUpdateServiceStatus_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);

            dbAPI.FillComboBoxServiceStatusCategory(cboServiceStatusCategory);

            btnClear_Click(this, e);
        }

        private void btnSearchMerchant_Click(object sender, EventArgs e)
        {

        }

        private void LoadRequestHistory()
        {
            int i = 0;
            int iLineNo = 0;

            lvwRequestIDHistory.View = View.Details;
            lvwRequestIDHistory.FullRowSelect = true;

            lvwRequestIDHistory.Columns.Clear();
            lvwRequestIDHistory.Columns.Add("LINE#", 50);
            lvwRequestIDHistory.Columns.Add("SERVICE NO", 80);
            lvwRequestIDHistory.Columns.Add("FSR NO", 70);
            lvwRequestIDHistory.Columns.Add("JOB TYPE", 140);
            lvwRequestIDHistory.Columns.Add("REQUEST ID", 130);
            lvwRequestIDHistory.Columns.Add("JOB STATUS", 100);
            lvwRequestIDHistory.Columns.Add("SERVICE STATUS", 120);
            lvwRequestIDHistory.Columns.Add("RESULT", 80);
            lvwRequestIDHistory.Columns.Add("SERVICE DATE", 90);
            lvwRequestIDHistory.Columns.Add("SERVICE REQ DATE", 110);
            lvwRequestIDHistory.Columns.Add("TERMINAL SN", 120);
            lvwRequestIDHistory.Columns.Add("SIM SN", 150);
            lvwRequestIDHistory.Columns.Add("MODIFIED BY", 140);
            lvwRequestIDHistory.Columns.Add("MODIFIED DATE", 130);

            lvwRequestIDHistory.Items.Clear();

            if (!dbFunction.isValidDescription(_mServicingDetailController.IRNo)) return;

            string pSearchValue = dbFunction.CheckAndSetStringValue(_mServicingDetailController.IRNo) + clsFunction.sPipe +
                                  clsFunction.sNull + clsFunction.sPipe +
                                  clsFunction.sZero + clsFunction.sPipe +
                                  clsFunction.sZero + clsFunction.sPipe +
                                  clsFunction.sZero + clsFunction.sPipe +
                                  dbFunction.CheckAndSetStringValue(_mServicingDetailController.TID) + clsFunction.sPipe +
                                  dbFunction.CheckAndSetStringValue(_mServicingDetailController.MID);

            dbAPI.ExecuteAPI("GET", "View", "Servicing List", pSearchValue, "Servicing Detail", "", "ViewServicingDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;
            if (dbAPI.isNoRecordFound()) return;

            while (clsArray.ServiceNo.Length > i)
            {
                if (clsArray.IRNo[i].Equals(_mServicingDetailController.IRNo) &&
                    clsArray.IRIDNo[i].Equals(_mServicingDetailController.IRIDNo.ToString()))
                {
                    iLineNo++;

                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.ForeColor = dbFunction.GetColorByStatus(int.Parse(clsArray.ServiceStatus[i]), clsArray.ActionMade[i]);

                    item.SubItems.Add(clsArray.ServiceNo[i]);
                    item.SubItems.Add(clsArray.FSRNo[i]);
                    item.SubItems.Add(clsArray.ServiceJobTypeDescription[i]);
                    item.SubItems.Add(clsArray.IRNo[i]);
                    item.SubItems.Add(clsArray.JobTypeStatusDescription[i]);
                    item.SubItems.Add(clsArray.ServiceStatusDescription[i]);
                    item.SubItems.Add(clsArray.ActionMade[i]);
                    item.SubItems.Add(clsArray.ServiceDate[i]);
                    item.SubItems.Add(clsArray.ServiceReqDate[i]);
                    item.SubItems.Add(clsArray.TerminalSN[i]);
                    item.SubItems.Add(clsArray.SIMSerialNo[i]);
                    item.SubItems.Add(clsArray.ModifiedBy[i]);
                    item.SubItems.Add(clsArray.ModifiedDateTime[i]);

                    lvwRequestIDHistory.Items.Add(item);
                }

                i++;
            }

            dbFunction.ListViewAlternateBackColor(lvwRequestIDHistory);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string pTerminalSN = "";
            string pSIMSN = "";

            if (!dbFunction.isValidDescriptionEntry(txtMerchantID.Text, "Merchant" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtServiceNo.Text, "Service No." + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            if (cboServiceStatusCategory.Text.Equals(clsFunction.sDefaultSelect) &&
                !clsSearch.ClassHoldServiceStatus.Equals(clsGlobalVariables.TA_STATUS_HOLD))
            {
                dbFunction.SetMessageBox("Please select a service status.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return;
            }

            // RESULT is column 7 of lvwRequestIDHistory; rows are ordered oldest to newest
            string pLastResult = lvwRequestIDHistory.Items.Count > 0
                ? lvwRequestIDHistory.Items[lvwRequestIDHistory.Items.Count - 1].SubItems[7].Text
                : "";

            if (cboServiceStatusCategory.Text.Equals(clsGlobalVariables.STATUS_CANCEL_DESC) &&
                int.Parse(txtJobType.Text) == (int)Enums.JobType.Pullout &&
                clsDefines.SERVICE_STATUS_COMPLETED.Equals(_mServicingDetailController.JobTypeStatusDescription))
            {
                dbFunction.SetMessageBox("Completed pull-out services cannot be cancelled.", "Action not allowed", clsFunction.IconType.iError);
                return;
            }
            else if (pLastResult.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS))
            {
                dbFunction.SetMessageBox($"Request ID [{_mServicingDetailController.IRNo}] already has a SUCCESS service." + "\n\n" +
                                         "Unable to proceed.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return;
            }
            else if (!pLastResult.Equals(clsGlobalVariables.ACTION_MADE_NEGATIVE) &&
                     dbFunction.isAlreadyCompleted(_mServicingDetailController.JobTypeStatusDescription)) return;

            bool fShowSN = !(clsSearch.ClassJobType.Equals(clsGlobalVariables.JOB_TYPE_SERVICING) ||
                             clsSearch.ClassJobType.Equals(clsGlobalVariables.JOB_TYPE_REPROGRAMMING) ||
                             clsSearch.ClassJobType.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT));

            pTerminalSN = $" >SN: {_mServicingDetailController.TerminalSN}\n >Type: {_mServicingDetailController.TerminalType}\n >Model: {_mServicingDetailController.TerminalModel}";
            pSIMSN = $" >SN: {_mServicingDetailController.SIMSN}\n >Carrier: {_mServicingDetailController.SIMCarrier}";

            if (!dbFunction.fConfirmPopup("Are you sure you want to update service status for job type " + txtJobTypeDesc.Text + "?" + "\n\n" +
                                          "Status details: " + "\n" +
                                          " >From: " + clsSearch.ClassHoldServiceStatusDescription + "\n" +
                                          " >To: " + (cboServiceStatusCategory.Text.Equals(clsFunction.sDefaultSelect) ? clsGlobalVariables.STATUS_RESUME_DESC : cboServiceStatusCategory.Text) + "\n\n" +
                                          (fShowSN
                                              ? "Terminal details:" + "\n" + pTerminalSN + "\n\n" +
                                                "SIM details:" + "\n" + pSIMSN + "\n\n" +
                                            $"*Warning: The serial number listed above will be marked as {(int.Parse(txtJobType.Text) == (int)Enums.JobType.Pullout ? "INSTALLED" : chkAvailable.Checked ? "AVAILABLE" : "DISPATCH/ALLOCATED")}" + "\n\n" : "") +
                                          (pLastResult.Equals(clsGlobalVariables.ACTION_MADE_NEGATIVE)
                                              ? $"*Note: The timeline of Request ID [{_mServicingDetailController.IRNo}] is NEGATIVE. No successful service was found." + "\n\n"
                                              : "") +
                                          "*Note: This action will [ NOT ] permanently delete the record.")) return;

            if (clsSearch.ClassServiceNo > 0)
            {
                clsSearch.ClassTerminalID = clsSearch.ClassSIMID = 0;
                clsSearch.ClassTerminalSN = clsSearch.ClassSIMSerialNo = clsFunction.sZero;

                if (clsSearch.ClassJobType.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION))
                {
                    clsSearch.ClassTerminalID = _mServicingDetailController.TerminalID;
                    clsSearch.ClassTerminalSN = _mServicingDetailController.TerminalSN;

                    clsSearch.ClassSIMID = _mServicingDetailController.SIMID;
                    clsSearch.ClassSIMSerialNo = _mServicingDetailController.SIMSN;
                }
                else if (clsSearch.ClassJobType.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT))
                {
                    clsSearch.ClassTerminalID = _mServicingDetailController.ReplaceTerminalID;
                    clsSearch.ClassTerminalSN = _mServicingDetailController.ReplaceTerminalSN;

                    clsSearch.ClassSIMID = _mServicingDetailController.ReplaceSIMID;
                    clsSearch.ClassSIMSerialNo = _mServicingDetailController.ReplaceSIMSN;
                }

                clsSearch.ClassServiceStatus = dbFunction.getFileID(cboServiceStatusCategory, "Service Status Active");
                clsSearch.ClassServiceStatusDescription = cboServiceStatusCategory.Text;

                int TerminalID = clsSearch.ClassTerminalID;
                int SIMID = clsSearch.ClassSIMID;

                if (clsSearch.ClassServiceStatus.Equals(clsGlobalVariables.JOB_TYPE_CANCEL))
                {
                    clsSearch.ClassTerminalID = clsSearch.ClassSIMID = 0;
                    clsSearch.ClassTerminalSN = clsSearch.ClassSIMSerialNo = clsFunction.sZero;
                }

                // SN Status
                if (chkAvailable.Checked)
                {
                    clsSearch.ClassTerminalStatus = clsSearch.ClassSIMStatus = clsGlobalVariables.STATUS_AVAILABLE;
                    clsSearch.ClassTerminalStatusDescription = clsSearch.ClassSIMStatusDescription = clsGlobalVariables.STATUS_AVAILABLE_DESC;
                }
                else
                {
                    if (_mServicingDetailController.DispatchID > 0)
                    {
                        clsSearch.ClassServiceStatus = clsSearch.ClassTerminalStatus = clsSearch.ClassSIMStatus = clsGlobalVariables.STATUS_DISPATCH;
                        clsSearch.ClassServiceStatusDescription = clsSearch.ClassTerminalStatusDescription = clsSearch.ClassSIMStatusDescription = clsGlobalVariables.STATUS_DISPATCH_DESC;
                    }
                    else
                    {
                        clsSearch.ClassServiceStatus = clsSearch.ClassTerminalStatus = clsSearch.ClassSIMStatus = clsGlobalVariables.STATUS_ALLOCATED;
                        clsSearch.ClassServiceStatusDescription = clsSearch.ClassTerminalStatusDescription = clsSearch.ClassSIMStatusDescription = clsGlobalVariables.STATUS_ALLOCATED_DESC;
                    }
                }

                if (clsSearch.ClassIRIDNo > 0)
                {
                    if (TerminalID > 0)
                    {
                        clsSearch.ClassAdvanceSearchValue = $"{TerminalID}{clsDefines.gPipe}{clsSearch.ClassTerminalStatus}{clsDefines.gPipe}{clsSearch.ClassTerminalStatusDescription}";
                        dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                        dbAPI.ExecuteAPI("PUT", "Update", "Update Terminal Detail Status", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
                    }

                    if (SIMID > 0)
                    {
                        clsSearch.ClassAdvanceSearchValue = $"{SIMID}{clsDefines.gPipe}{clsSearch.ClassSIMStatus}{clsDefines.gPipe}{clsSearch.ClassSIMStatusDescription}";
                        dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                        dbAPI.ExecuteAPI("PUT", "Update", "Update SIM Detail Status", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
                    }

                    if (clsSearch.ClassTerminalID > 0)
                    {
                        clsSearch.ClassAdvanceSearchValue = $"{clsSearch.ClassTerminalID}{clsDefines.gPipe}{clsSearch.ClassTerminalStatus}{clsDefines.gPipe}{clsSearch.ClassTerminalStatusDescription}";
                        dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                        dbAPI.ExecuteAPI("PUT", "Update", "Update Terminal Detail Status", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
                    }

                    if (clsSearch.ClassSIMID > 0)
                    {
                        clsSearch.ClassAdvanceSearchValue = $"{clsSearch.ClassSIMID}{clsDefines.gPipe}{clsSearch.ClassSIMStatus}{clsDefines.gPipe}{clsSearch.ClassSIMStatusDescription}";
                        dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                        dbAPI.ExecuteAPI("PUT", "Update", "Update SIM Detail Status", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
                    }
                }

                if (clsSearch.ClassServiceNo > 0)
                {
                    clsSearch.ClassAdvanceSearchValue = $"{_mServicingDetailController.ServiceNo}{clsDefines.gPipe}{_mServicingDetailController.IRIDNo}{clsDefines.gPipe}{clsSearch.ClassServiceStatus}{clsDefines.gPipe}{clsSearch.ClassServiceStatusDescription}";
                    dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                    dbAPI.ExecuteAPI("PUT", "Update", "Service Type Status Category", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                    clsSearch.ClassAdvanceSearchValue = $"{_mServicingDetailController.ServiceNo}{clsDefines.gPipe}{StrClean(txtRemarks.Text)}";
                    dbAPI.ExecuteAPI("PUT", "Update", "Update Service Remarks", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
                }

                dbFunction.SetMessageBox("Service status update completed", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);

                btnClear_Click(this, e);
            }
        }

        private void cboServiceStatusCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkAvailable.Enabled = false;
            chkAvailable.Checked = cboServiceStatusCategory.Text.Equals(clsGlobalVariables.STATUS_HOLD_DESC) ||
                                   cboServiceStatusCategory.Text.Equals(clsGlobalVariables.STATUS_CANCEL_DESC);
        }

        private void btnUpdateRemarks_Click(object sender, EventArgs e)
        {
            if (!dbFunction.fConfirmPopup("Are you sure to update job order remarks?")) return;

            if (clsSearch.ClassServiceNo > 0)
            {
                // update service remarks
                clsSearch.ClassAdvanceSearchValue = $"{_mServicingDetailController.ServiceNo}{clsDefines.gPipe}{StrClean(txtRemarks.Text)}";
                dbAPI.ExecuteAPI("PUT", "Update", "Update Service Remarks", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                dbFunction.SetMessageBox("Job order remarks update completed", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
            }
        }

        private void lvwRequestIDHistory_DoubleClick(object sender, EventArgs e)
        {
            if (lvwRequestIDHistory.SelectedIndices.Count > 0)
            {
                string jsonResult = dbFunction.genJSONFormat(lvwRequestIDHistory, lvwRequestIDHistory.SelectedIndices[0], "", "");

                // Pass JSON to popup window
                frmPopUpInfo frm = new frmPopUpInfo(jsonResult);
                frm.ShowDialog();
            }
        }
    }
}
