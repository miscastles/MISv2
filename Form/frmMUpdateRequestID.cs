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
    public partial class frmMUpdateRequestID : Form
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

        public frmMUpdateRequestID()
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

        private void frmMUpdateRequestID_KeyDown(object sender, KeyEventArgs e)
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
        
        private void frmMUpdateRequestID_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);

            btnClear_Click(this, e);

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

            btnSave.Enabled = false;

            btnSearchService.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidDescriptionEntry(txtMerchantID.Text, "Merchant" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtServiceNo.Text, "Service No." + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtCurrentRequestID.Text, "Current Request ID" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtNewRequestID.Text, "New Request ID" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            // check valid reference no if not already exist
            if (dbAPI.isRecordExist("Search", "Service RequestID", txtIRIDNo.Text + clsFunction.sPipe + txtJobType.Text + clsFunction.sPipe + txtNewRequestID.Text))
            {
                dbFunction.SetMessageBox("Request ID " + dbFunction.AddBracketStartEnd(txtNewRequestID.Text) + " already exist." + "\n\n" +
                                         "Unable to proceed.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                return;
            }

            if (!dbFunction.fPromptConfirmation("Are you sure to update the following below: " + "\n\n"  +
                         "[Merchant Information]" + "\n" +
                         " > Name: " + _mServicingDetailController.MerchantName + "\n" +
                         " > TID: " + _mServicingDetailController.TID + "\n" +
                         " > MID: " + _mServicingDetailController.MID + "\n" +
                         "[Service Information]" + "\n" +
                         " > Job Type: " + txtJobTypeDesc.Text + "\n" +
                         " > Current Request ID: " + txtCurrentRequestID.Text + "\n" +
                         "[Update Information]" + "\n" +
                         " > New Request ID: " + txtNewRequestID.Text                       
                         )) return;

            // update
            clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtServiceNo.Text) + clsFunction.sPipe +
                dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + clsFunction.sPipe +
                dbFunction.CheckAndSetStringValue(txtNewRequestID.Text);

            dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

            dbAPI.ExecuteAPI("PUT", "Update", "Update Service RequestID", clsSearch.ClassAdvanceSearchValue , "", "", "UpdateCollectionDetail");

            dbFunction.SetMessageBox("Request ID update completed", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);

            btnClear_Click(this, e);

        }

        private void btnSearchMerchant_Click(object sender, EventArgs e)
        {

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

                    txtSearchIRNo.Text = txtCurrentRequestID.Text = _mServicingDetailController.IRNo;

                    ucMerchantInfo.loadData(_mServicingDetailController);
                    ucServiceInfo.loadData(_mServicingDetailController);
                    ucFSRInfo.loadData(_mServicingDetailController);
                    ucCurrentSNInfo.loadData(_mServicingDetailController);
                    ucReplaceSNInfo.loadData(_mServicingDetailController);
                    ucMerchantRepInfo.loadData(_mServicingDetailController);
                    ucVendorRepInfo.loadData(_mServicingDetailController);

                    btnSave.Enabled = true;

                    txtCurrentRequestID.ReadOnly = true;
                    txtCurrentRequestID.BackColor = Color.FromArgb(192, 255, 255);

                    txtNewRequestID.ReadOnly = false;
                    txtNewRequestID.BackColor = Color.White;

                    dbFunction.isValidRequestID(_mServicingDetailController.IRNo, _mServicingDetailController.IRNo_fsr);

                    txtNewRequestID.Focus();
                }
                else
                {
                    dbFunction.SetMessageBox("No record found.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
                }
                
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnNoRequestID_Click(object sender, EventArgs e)
        {   
            if (!dbFunction.fPromptConfirmation("Are you sure to let the system generate REQUEST ID?")) return;
            
            if (txtJobTypeDesc.Text.Equals(clsGlobalVariables.STATUS_INSTALLATION_DESC))
                dbAPI.GenerateID(true, txtNewRequestID, txtIRIDNo, "IR Detail", clsDefines.CONTROLID_PREFIX_IR);
            else
                dbAPI.GenerateID(true, txtNewRequestID, txtServiceNo, "Servicing Detail", clsDefines.CONTROLID_PREFIX_NO_REQUESTID);

        }
    }
}
