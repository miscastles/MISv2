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
using MIS.Enums;

namespace MIS
{
    public partial class frmMerchantValidation : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        public static string sHeader;
        bool fEdit = false;

        // Controller
        private TypeController _mTypeController;
        private MSPMasterController _mMSPMasterController;
        private MSPDetailController _mMSPDetailController;

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

        public frmMerchantValidation()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwList, true);

            // Initialize the controller object
            _mTypeController = new TypeController();
            _mMSPMasterController = new MSPMasterController();
            _mMSPDetailController = new MSPDetailController();
            
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmMerchantValidation_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
                case Keys.F2:
                    if (btnSearchMSP.Enabled)
                        btnSearchMSP_Click(this, e);
                    break;
            }
        }

        private void FillMerchantTextBox()
        {
            dbAPI.ExecuteAPI("GET", "Search", "Particular Info", dbFunction.CheckAndSetNumericValue(txtMerchantID.Text), "Get Info Detail", "", "GetInfoDetail");

            if (dbAPI.isNoRecordFound() == false)
            {
                txtMerchantName.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                txtMerchantAddress.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                txtRegionType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                txtMerchantRegion.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                txtRegionID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5);
                txtMerchantProvince.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                txtContactPerson.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 11);
                txtContactNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);
                txtContactEmail.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 14);
                txtContactPosition.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 22);

            }
        }

        private void FillMSPMasterBox()
        {
            MSPMasterController model = new MSPMasterController();
            MSPMasterController data = model.getInfo(int.Parse(dbFunction.CheckAndSetNumericValue(txtMSPNo.Text)));
            if (data != null)
            {
                txtMSPNo.Text = data.MSPNo.ToString();
                txtReferenceNo.Text = data.ReferenceNo;
                txtRegisteredName.Text = data.RegisteredName;
                txtTradeName.Text = data.TradeName;
                txtTINNo.Text = data.TINNo;

                txtAcqType.Text = data.AcquirerType;
                txtCategoryType.Text = data.Category;
                txtNoBType.Text = data.NoBType;
                txtBusType.Text = data.BusType;
                txtReferralType.Text = data.ReferralType;
                txtSchemeType.Text = data.SchemeType;
                txtClientName.Text = data.ClientName;
                cboMSPStatus.Text = data.MSPStatusDesc;
                
                txtMDRCreditType.Text = data.MDRCreditType;
                txtMDRDebitType.Text = data.MDRDebitType;
                txtMDRInstType.Text = data.MDRInstType;

                txtSalesName.Text = data.SubmitBy;
                txtNoOfPOS.Text = data.NoOfPOS;
                txtNoOfYear.Text = data.NoOfYear;
                txtRentalAmt.Text = data.RentalAmt;

                txtCreatedDate.Text = data.CreatedAt;
                txtSubmitDate.Text = data.SubmitAt;
                txtCreatedBy.Text = data.CreatedBy;
                
                txtRemarks.Text = data.Remarks;

                // Bank
                txtBankAcntName.Text = data.BankAcntName;
                txtBankAcntNo.Text = data.BankAcntNo;
                txtBankSettlement.Text = data.BankSettlement;
                txtBankReferring.Text = data.BankReferring;

                txtBankAcntName.ReadOnly = txtBankAcntNo.ReadOnly = txtBankSettlement.ReadOnly = txtBankReferring.ReadOnly = txtResultRemarks.ReadOnly = false;

                txtCheckedID.Text = data.CheckedID.ToString();
                txtCheckedName.Text = data.CheckedBy;

                if (dbFunction.isValidID(txtCheckedID.Text))
                    dteCheckedDate.Value = DateTime.Parse(data.CheckedAt);

                txtNoOfDoc.Text = data.NoOfDocument.ToString();
                initDocumentButton(data.NoOfDocument);
            }
        }

        private void btnSearchMSP_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iMSPMasterList;
            frmSearchField.sHeader = "SEARCH MSP";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                Cursor.Current = Cursors.WaitCursor;

                fEdit = true;
                dbFunction.ClearTextBox(this);
                dbFunction.TextBoxUnLock(true, this);

                txtMSPNo.Text = clsSearch.ClassMSPNo.ToString();
                txtClientID.Text = clsSearch.ClassClientID.ToString();
                txtMerchantID.Text = clsSearch.ClassMerchantID.ToString();

                FillMSPMasterBox();

                FillMerchantTextBox();

                loadMSPDetailList();

                btnSave.Enabled = true;

                PKTextBoxBackColor();

                Cursor.Current = Cursors.Default;
            }
        }

        private void frmMerchantValidation_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            fEdit = false;
            
            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);

            FillComboBoxStatusType(cboMSPStatus);
            FillComboBoxResultStatusType(cboResultStatus);

            initDate();

            iniComboBoxSelection();

            btnSave.Enabled = false;

            PKTextBoxBackColor();

            initDocumentButton(0);

            Cursor.Current = Cursors.Default;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            fEdit = false;
            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ClearListViewItems(lvwList);

            initDate();

            iniComboBoxSelection();

            chkResultInfo.Checked = false;

            btnSave.Enabled = false;

            initDocumentButton(0);

        }

        private void initDate()
        {
            dteCheckedDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteCheckedDate, clsFunction.sStandardDateDefault);

            dteResultSubmitDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteResultSubmitDate, clsFunction.sStandardDateDefault);

            dteResultCreatedDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteResultCreatedDate, clsFunction.sStandardDateDefault);
            
        }

        private void FillComboBoxStatusType(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;
            List<TypeController> mList = null;

            obj.Items.Clear();
            mList = _mTypeController.getDetailList(clsFunction.sZero + clsFunction.sPipe + (int)OtherType.StatusType);

            if (mList != null)
            {
                if (mList != null)
                {
                    foreach (var itemData in mList)
                    {
                        if (!fSelect)
                        {
                            obj.Items.Add(clsFunction.sDefaultSelect);
                            fSelect = true;
                        }

                        obj.Items.Add(itemData.Description);

                    }

                    if (i > 0)
                        obj.SelectedIndex = 0;

                }
            }
        }

        void FillComboBoxResultStatusType(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;
            List<TypeController> mList = null;

            obj.Items.Clear();
            mList = _mTypeController.getDetailList(clsFunction.sZero + clsFunction.sPipe + (int)OtherType.ResultStatusType);

            if (mList != null)
            {
                if (mList != null)
                {
                    foreach (var itemData in mList)
                    {
                        if (!fSelect)
                        {
                            obj.Items.Add(clsFunction.sDefaultSelect);
                            fSelect = true;
                        }

                        obj.Items.Add(itemData.Description);

                    }

                    if (i > 0)
                        obj.SelectedIndex = 0;

                }
            }
        }

        private void btnUploadDocument_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidDescriptionEntry(txtClientID.Text, "Client" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtMSPNo.Text, "MSP No" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtMerchantName.Text, "Merchant" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtSalesName.Text, "Sales/MSE" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            clsSearch.ClassReferenceNo = txtReferenceNo.Text;

            frmMerchantAttachment.sHeader = "MSP - UPLOAD SUPPORT DOCUMENT";
            frmMerchantAttachment frm = new frmMerchantAttachment();
            frm.ShowDialog();
        }

        private void btnSearchAdmin_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iFE;
            frmSearchField.sHeader = "EMPLOYEE";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsSearch.ClassCheckedID = clsSearch.ClassParticularID;
                txtCheckedID.Text = clsSearch.ClassCheckedID.ToString();
                txtCheckedName.Text = clsSearch.ClassParticularName;
            }
        }

        private void iniComboBoxSelection()
        {   
            cboMSPStatus.Text = cboResultStatus.Text = clsFunction.sDefaultSelect;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateFields()) return;

            if (!dbFunction.fPromptConfirmation("Are you sure to saved Merchant validation." + "\n\n" +
                "[MSP Information]" + "\n" +
                ">Merchant Name: " + txtMerchantName.Text + "\n" +
                ">Scheme: " + txtSchemeType.Text + "\n" +
                ">Status: " + cboMSPStatus.Text)) return;

            updateMSPMasterStatus();

            updateBankDetail();

            saveMSPDetail();

            dbFunction.SetMessageBox("Merchant validation successfully saved.", "Saved", clsFunction.IconType.iInformation);

            btnClear_Click(this, e);
        }

        private void saveMSPDetail()
        {
            string sSQL = "";

            var data = new
            {
                MSPNo = dbFunction.CheckAndSetNumericValue(txtMSPNo.Text),
                ClientID = dbFunction.CheckAndSetNumericValue(txtClientID.Text),
                MeerchantID = dbFunction.CheckAndSetNumericValue(txtMerchantID.Text),
                CreatedID = clsSearch.ClassCurrentParticularID,
                CreatedAt = dbFunction.getCurrentDateTime(),
                SubmitAt = dbFunction.CheckAndSetDatePickerValueToDate(dteResultSubmitDate),
                ResultStatus = dbFunction.getFileID(cboResultStatus, "All Type"),
                Remarks = dbFunction.CheckAndSetStringValue(txtResultRemarks.Text)
                
            };

            sSQL = IFormat.Insert(data);

            Debug.WriteLine("saveMSPDetail" + sSQL);

            dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 0);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "MSP Detail", sSQL, "InsertCollectionDetail");

            clsSearch.ClassLastInsertedID = clsLastID.ClassLastInsertedID;
            txtControlNo.Text = clsSearch.ClassLastInsertedID.ToString();
        }

        private void updateBankDetail()
        {
            clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtMSPNo.Text) + clsFunction.sPipe +
                                                dbFunction.CheckAndSetStringValue(txtBankAcntName.Text) + clsFunction.sPipe +
                                                dbFunction.CheckAndSetStringValue(txtBankAcntNo.Text) + clsFunction.sPipe +
                                                dbFunction.CheckAndSetStringValue(txtBankSettlement.Text) + clsFunction.sPipe +
                                                dbFunction.CheckAndSetStringValue(txtBankReferring.Text) + clsFunction.sPipe +
                                                dbFunction.CheckAndSetNumericValue(txtCheckedID.Text) + clsFunction.sPipe +
                                                dbFunction.CheckAndSetDatePickerValueToDate(dteCheckedDate);

            dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

            dbAPI.ExecuteAPI("PUT", "Update", "MSP Master-Bank", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
        }

        private bool ValidateFields()
        {
            if (!dbFunction.isValidDescriptionEntry(txtMSPNo.Text, "MSP No." + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtMerchantName.Text, "Merchant name" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtSalesName.Text, "MSE/Sales name" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtCheckedName.Text, "Checked by" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;

            if (!dbFunction.isValidDescriptionEntry(txtBankAcntName.Text, "Bank account name" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtBankAcntNo.Text, "Bank account number" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtBankSettlement.Text, "Bank settlement" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtBankReferring.Text, "Bank referring" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;

            if (chkResultInfo.Checked)
                if (!dbFunction.isValidDescriptionEntry(cboResultStatus.Text, "Result status" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;

            return true;
        }

        private void loadMSPDetailList()
        {
            int lineno = 0;
            List<MSPDetailController> mList = null;
            MSPDetailController mData = null;
            int pControlNo = 0;

            dbFunction.ClearListViewItems(lvwList);

            mList = _mMSPDetailController.getDetailList(dbFunction.CheckAndSetNumericValue(txtMSPNo.Text));

            if (mList != null)
            {
                foreach (var itemData in mList)
                {
                    lineno++;
                    ListViewItem item = new ListViewItem(lineno.ToString());
                    item.SubItems.Add(itemData.ControlNo.ToString());
                    item.SubItems.Add(itemData.CreatedAt);
                    item.SubItems.Add(itemData.SubmitAt);
                    item.SubItems.Add(itemData.ResultStatusDesc);
                    item.SubItems.Add(itemData.Remarks);
                    
                    lvwList.Items.Add(item);

                    pControlNo = itemData.ControlNo;
                }

                dbFunction.ListViewAlternateBackColor(lvwList);
                
            }

            if (pControlNo > 0)
            {
                mData = _mMSPDetailController.getInfo(pControlNo);
                if (mData != null)
                {
                    dteResultCreatedDate.Value = DateTime.Parse(mData.CreatedAt);
                    dteResultSubmitDate.Value = DateTime.Parse(mData.SubmitAt);
                    cboResultStatus.Text = mData.ResultStatusDesc;
                    txtResultRemarks.Text = mData.Remarks;
                }
            }
        }

        private void PKTextBoxBackColor()
        {
            txtMSPNo.BackColor = clsFunction.PKBackColor;
            txtReferenceNo.BackColor = clsFunction.MKBackColor;
        }

        private void updateMSPMasterStatus()
        {
            clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtMSPNo.Text) + clsFunction.sPipe +
                                                dbFunction.getFileID(cboMSPStatus, "All Type");

            dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

            dbAPI.ExecuteAPI("PUT", "Update", "MSP Master-Status", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
        }

        private void initDocumentButton(int pDocCount)
        {
            btnUploadDocument.Text = "UPLOAD DOCUMENT" + " " + dbFunction.AddParenthesisStartEnd(pDocCount.ToString());            
        }

        private void btnEmailMSE_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidDescriptionEntry(txtMSPNo.Text, "MSP No." + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtMerchantName.Text, "Merchant name" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            if (!dbFunction.fPromptConfirmation("Are you sure to notify email?")) return;

            Cursor.Current = Cursors.WaitCursor;

            var tagValueMap = new Dictionary<string, string>
            {
                { clsDefines.TAG_MERCHANTNAME, txtMerchantName.Text },
                { clsDefines.TAG_Address, txtMerchantAddress.Text },
                { clsDefines.TAG_REGION, txtMerchantRegion.Text },
                { clsDefines.TAG_Province, txtMerchantProvince.Text },
                { clsDefines.TAG_REFERENCENO, txtReferenceNo.Text },
                { clsDefines.TAG_ClientName, txtClientName.Text },
                { clsDefines.TAG_RegisteredName, txtRegisteredName.Text },
                { clsDefines.TAG_TradeName, txtTradeName.Text },
                { clsDefines.TAG_TINNo, txtTINNo.Text },
                { clsDefines.TAG_AcquirerType, txtAcqType.Text },
                { clsDefines.TAG_BusType, txtBusType.Text },
                { clsDefines.TAG_NoBType, txtNoBType.Text },
                { clsDefines.TAG_SchemeType, txtSchemeType.Text },
                { clsDefines.TAG_ReferralType, txtReferralType.Text },
                { clsDefines.TAG_Category, txtCategoryType.Text },
                { clsDefines.TAG_MDRCreditType, txtMDRCreditType.Text },
                { clsDefines.TAG_MDRDebitType, txtMDRDebitType.Text },
                { clsDefines.TAG_MDRInstType, txtMDRInstType.Text },

                { clsDefines.TAG_CreatedAt, txtCreatedDate.Text },
                { clsDefines.TAG_CreatedBy, txtCreatedBy.Text },
                { clsDefines.TAG_SubmitAt, txtSubmitDate.Text },
                //{ clsDefines.TAG_UpdatedBy, txtUpdatedBy.Text },

                { clsDefines.TAG_CheckedAt, dbFunction.CheckAndSetDate(dteCheckedDate.Value.ToShortDateString()) },
                { clsDefines.TAG_CheckedBy, txtCheckedName.Text },

                { clsDefines.TAG_BankAcntName, txtBankAcntName.Text },
                { clsDefines.TAG_BankAcntNo, txtBankAcntNo.Text },
                { clsDefines.TAG_BankSettlement, txtBankSettlement.Text },
                { clsDefines.TAG_BankReferring, txtBankReferring.Text },

                { clsDefines.TAG_MSPStatusDesc, cboMSPStatus.Text },
                { clsDefines.TAG_ResultCreatedAt, dbFunction.CheckAndSetDate(dteResultCreatedDate.Value.ToShortDateString()) },
                { clsDefines.TAG_ResultSubmitAt, dbFunction.CheckAndSetDate(dteResultSubmitDate.Value.ToShortDateString()) },
                { clsDefines.TAG_ResultStatusDesc, cboResultStatus.Text },
                { clsDefines.TAG_ResultRemarks, txtResultRemarks.Text },

                { clsDefines.TAG_SubmitBy, txtSalesName.Text },
                { clsDefines.TAG_ContactPerson, txtContactPerson.Text },
                { clsDefines.TAG_ContactPosition, txtContactPosition.Text },
                { clsDefines.TAG_ContactNumber, txtContactNo.Text },
                { clsDefines.TAG_NoOfYear, txtNoOfYear.Text },
                { clsDefines.TAG_NoOfPOS, txtNoOfPOS.Text },
                { clsDefines.TAG_RentalAmt, txtRentalAmt.Text },
                { clsDefines.TAG_NoOfDocument, txtNoOfDoc.Text }
            };

            string jsonString = dbFunction.convertToJson(tagValueMap);
            Debug.WriteLine(jsonString);

            dbFunction.parseDelimitedString(jsonString, clsDefines.gComma, 1);

            dbAPI.ExecuteAPI("POST", "Notify", "MSP Validation", jsonString, "Email Notification", "", "EmailNotification");

            dbFunction.SetMessageBox("MSP email notification complete.", "Email Notification - Merchant Validation", clsFunction.IconType.iInformation);

            Cursor.Current = Cursors.Default;
        }
    }
}
