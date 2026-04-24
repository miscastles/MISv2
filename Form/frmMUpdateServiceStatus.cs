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
using static MIS.Function.AppUtilities;

namespace MIS
{
    public partial class frmMUpdateServiceStatus : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;

        // Controller
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

        private void btnSearchService_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iService;
            frmSearchField.sHeader = "SEARCH SERVICE";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                Cursor.Current = Cursors.WaitCursor;

                // fill additional info
                _mServicingDetailController = _mServicingDetailController.getServicingInfo($"{dbFunction.CheckAndSetNumericValue(clsSearch.ClassServiceNo.ToString())}{clsDefines.gPipe}{dbFunction.CheckAndSetNumericValue(clsSearch.ClassIRIDNo.ToString())}");

                if (_mServicingDetailController != null)
                {
                    // fill control information                
                    txtJobType.Text = _mServicingDetailController.JobType.ToString();
                    txtServiceNo.Text = _mServicingDetailController.ServiceNo.ToString();
                    txtFSRNo.Text = _mServicingDetailController.FSRNo.ToString();
                    txtIRIDNo.Text = _mServicingDetailController.IRIDNo.ToString();
                    txtMerchantID.Text = _mServicingDetailController.MerchantID.ToString();
                    txtJobTypeDesc.Text = _mServicingDetailController.ServiceJobTypeDescription;
                    txtServiceStatus.Text = _mServicingDetailController.ServiceStatus.ToString();

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

                    btnSave.Enabled = btnDelete.Enabled = true;
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
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);

            btnSearchMerchant.Enabled = false;
            btnSearchService.Enabled = true;

            dbFunction.SetButtonIconImage(btnSearchMerchant);
            dbFunction.SetButtonIconImage(btnSearchService);

            btnSave.Enabled = btnDelete.Enabled = false;
            cboServiceStatusCategory.Enabled = false;

            chkAvailable.Enabled = true;
            chkAvailable.Checked = false;

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

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidDescriptionEntry(txtMerchantID.Text, "Merchant" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtServiceNo.Text, "Service No." + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            if (dbFunction.isAlreadyCompleted(_mServicingDetailController.JobTypeStatusDescription)) return;

            //if (!cboServiceStatusCategory.Text.Equals(clsSearch.ClassHoldServiceStatusDescription))
            //{
            //    if (!dbFunction.fPromptConfirmation("Changes on service status type detected:" + "\n\n" +
            //                                        "From: " + clsSearch.ClassHoldServiceStatusDescription + "\n" +
            //                                        "To: " + (cboServiceStatusCategory.Text.Equals(clsFunction.sDefaultSelect) ? clsGlobalVariables.STATUS_RESUME_DESC : cboServiceStatusCategory.Text) + "\n\n" +
            //                                        "Do you still want to continue?")) return;
                
            //    }

            if (clsSearch.ClassHoldServiceStatus.Equals(clsGlobalVariables.JOB_TYPE_CANCEL))
            {
                dbFunction.SetMessageBox("The requested service has already been canceled and cannot be processed again.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return;
            }
            
            string pTerminalSN = "";
            string pSIMSN = "";
            if (dbFunction.isValidID(_mServicingDetailController.TerminalID.ToString()) && dbFunction.isValidID(_mServicingDetailController.ReplaceTerminalID.ToString()))
            {
                pTerminalSN = $" >SN: {_mServicingDetailController.ReplaceTerminalSN}\n >Type: {_mServicingDetailController.ReplaceTerminalType}\n >Model: {_mServicingDetailController.ReplaceTerminalModel}";
            }
            else
            {
                pTerminalSN = $" >SN: {_mServicingDetailController.TerminalSN}\n >Type: {_mServicingDetailController.TerminalType}\n >Model: {_mServicingDetailController.TerminalModel}";
            }

            if (dbFunction.isValidID(_mServicingDetailController.SIMID.ToString()) && dbFunction.isValidID(_mServicingDetailController.ReplaceSIMID.ToString()))
            {
                pSIMSN = $" >SN: {_mServicingDetailController.ReplaceSIMSN}\n >Carrier: {_mServicingDetailController.ReplaceSIMCarrier}";
            }
            else
            {
                pSIMSN = $" >SN: {_mServicingDetailController.SIMSN}\n >Carrier: {_mServicingDetailController.SIMCarrier}";
            }

            if (!dbFunction.fPromptConfirmation("Are you sure you want to update service status for job type " + txtJobTypeDesc.Text + "?" + "\n\n" +
                                                   "Status details: " + "\n" +
                                                   " >From: " + clsSearch.ClassHoldServiceStatusDescription + "\n" +
                                                   " >To: " + (cboServiceStatusCategory.Text.Equals(clsFunction.sDefaultSelect) ? clsGlobalVariables.STATUS_RESUME_DESC : cboServiceStatusCategory.Text) + "\n\n" +
                                                   "Terminal details:" + "\n" + pTerminalSN + "\n\n" +
                                                   "SIM details:" + "\n" + pSIMSN + "\n\n" +                                                   
                                                   $"*Warning: The serial number listed below will be marked as {(int.Parse(txtJobType.Text) == (int)Enums.JobType.Pullout ? "INSTALLED" : "AVAILABLE")}")) return;


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
                    if (clsSearch.ClassJobType.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION))
                    {
                        clsSearch.ClassAdvanceSearchValue = $"{clsSearch.ClassIRIDNo}{clsDefines.gPipe}{clsSearch.ClassServiceStatus}{clsDefines.gPipe}{clsSearch.ClassServiceStatusDescription}";
                        dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                        dbAPI.ExecuteAPI("PUT", "Update", "Update IR Detail Status", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                        clsSearch.ClassAdvanceSearchValue = $"{clsSearch.ClassIRIDNo}{clsDefines.gPipe}{clsSearch.ClassTerminalID}{clsDefines.gPipe}{clsSearch.ClassTerminalSN}{clsDefines.gPipe}{clsSearch.ClassSIMID}{clsDefines.gPipe}{clsSearch.ClassSIMSerialNo}";
                        dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                        dbAPI.ExecuteAPI("PUT", "Update", "Update IR Detail Terminal/SIM", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
                    }
                    
                    // set SN to available
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

                    // IR Status
                }

                if (clsSearch.ClassServiceNo > 0)
                {
                    // update service status
                    clsSearch.ClassAdvanceSearchValue = $"{_mServicingDetailController.ServiceNo}{clsDefines.gPipe}{_mServicingDetailController.IRIDNo}{clsDefines.gPipe}{clsSearch.ClassServiceStatus}{clsDefines.gPipe}{clsSearch.ClassServiceStatusDescription}";
                    dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                    dbAPI.ExecuteAPI("PUT", "Update", "Service Type Status Category", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                    // update service remarks
                    clsSearch.ClassAdvanceSearchValue = $"{_mServicingDetailController.ServiceNo}{clsDefines.gPipe}{StrClean(txtRemarks.Text)}";
                    dbAPI.ExecuteAPI("PUT", "Update", "Update Service Remarks", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
                }
                
                dbFunction.SetMessageBox("Service status update completed", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);

                btnClear_Click(this, e);
            }
            

        }

        private void cboServiceStatusCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboServiceStatusCategory.Text.Equals(clsGlobalVariables.STATUS_HOLD_DESC))
            {
                chkAvailable.Enabled = true;
                chkAvailable.Checked = true;
            }
            else if (cboServiceStatusCategory.Text.Equals(clsGlobalVariables.STATUS_CANCEL_DESC))
            {
                chkAvailable.Enabled = false;
                chkAvailable.Checked = true;
            }
            else
            {
                chkAvailable.Enabled = false;
                chkAvailable.Checked = false;
            }
            
        }

        private void btnUpdateRemarks_Click(object sender, EventArgs e)
        {
            if (!dbFunction.fPromptConfirmation("Are you sure to update job order remarks?")) return;

            if (clsSearch.ClassServiceNo > 0)
            {
                // update service remarks
                clsSearch.ClassAdvanceSearchValue = $"{_mServicingDetailController.ServiceNo}{clsDefines.gPipe}{StrClean(txtRemarks.Text)}";
                dbAPI.ExecuteAPI("PUT", "Update", "Update Service Remarks", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                dbFunction.SetMessageBox("Job order remarks update completed", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
            }

            
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

        }
    }
}
