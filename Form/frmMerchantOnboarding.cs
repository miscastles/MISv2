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
    public partial class frmMerchantOnboarding : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        public static string sHeader;
        bool fEdit = false;

        // Controller
        private TypeController _mTypeController;

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

        public frmMerchantOnboarding()
        {
            InitializeComponent();

            // Initialize the controller object
            _mTypeController = new TypeController();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmMerchantOnboarding_Load(object sender, EventArgs e)
        {

            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            fEdit = false;
            InitButton();

            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);

            dbAPI.FillComboBoxClient(cboClient);
            FillComboBoxMerchantCategoryType(cboCategoryType);
            FillComboBoxNatureOfBusinessType(cboNoBType);
            FillComboBoxBusinessType(cboBusType);
            FillComboBoxReferralType(cboReferralType);
            FillComboBoxSchemeType(cboSchemeType);
            FillComboBoxStatusType(cboMSPStatus);
            FillComboBoxAcquirerType(cboAcqType);
            FillComboBoxMDRCreditType(cboMDRCreditType);
            FillComboBoxMDRDebitType(cboMDRDebitType);
            FillComboBoxMDRInstType(cboMDRInstType);

            iniComboBoxSelection();

            initDate();

            PKTextBoxBackColor();

            initDocumentButton(0);

            Cursor.Current = Cursors.Default;

            
        }

        private void label31_Click(object sender, EventArgs e)
        {

        }

        private void FillComboBoxMerchantCategoryType(ComboBox obj)
        {   
            int i = 0;
            bool fSelect = false;
            List<TypeController> mList = null;

            obj.Items.Clear();
            mList = _mTypeController.getDetailList(clsFunction.sZero + clsFunction.sPipe + (int)OtherType.CategoryType);

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

        private void FillComboBoxNatureOfBusinessType(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;
            List<TypeController> mList = null;

            obj.Items.Clear();
            mList = _mTypeController.getDetailList(clsFunction.sZero + clsFunction.sPipe + (int)OtherType.NoBType);

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

        private void FillComboBoxBusinessType(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;
            List<TypeController> mList = null;

            obj.Items.Clear();
            mList = _mTypeController.getDetailList(clsFunction.sZero + clsFunction.sPipe + (int)OtherType.BusType);

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

        private void FillComboBoxReferralType(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;
            List<TypeController> mList = null;

            obj.Items.Clear();
            mList = _mTypeController.getDetailList(clsFunction.sZero + clsFunction.sPipe + (int)OtherType.ReferralType);

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

        private void FillComboBoxSchemeType(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;
            List<TypeController> mList = null;

            obj.Items.Clear();
            mList = _mTypeController.getDetailList(clsFunction.sZero + clsFunction.sPipe + (int)OtherType.SchemeType);

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

        private void FillComboBoxAcquirerType(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;
            List<TypeController> mList = null;

            obj.Items.Clear();
            mList = _mTypeController.getDetailList(clsFunction.sZero + clsFunction.sPipe + (int)OtherType.AcqType);

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

        private void FillComboBoxMDRCreditType(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;
            List<TypeController> mList = null;

            obj.Items.Clear();
            mList = _mTypeController.getDetailList(clsFunction.sZero + clsFunction.sPipe + (int)OtherType.MDRCreditType);

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

        private void FillComboBoxMDRDebitType(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;
            List<TypeController> mList = null;

            obj.Items.Clear();
            mList = _mTypeController.getDetailList(clsFunction.sZero + clsFunction.sPipe + (int)OtherType.MDRDebitType);

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

        private void FillComboBoxMDRInstType(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;
            List<TypeController> mList = null;

            obj.Items.Clear();
            mList = _mTypeController.getDetailList(clsFunction.sZero + clsFunction.sPipe + (int)OtherType.MDRInstType);

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

        private void label50_Click(object sender, EventArgs e)
        {

        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void iniComboBoxSelection()
        {
            cboClient.Text = cboCategoryType.Text = cboNoBType.Text = cboBusType.Text = cboReferralType.Text = cboSchemeType.Text = cboMSPStatus.Text = cboAcqType.Text = clsFunction.sDefaultSelect;
            cboMDRCreditType.Text = cboMDRDebitType.Text = cboMDRInstType.Text = clsFunction.sDefaultSelect;
        }

        private void btnSearchMerchant_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iIRMerchantList;
            frmSearchField.sHeader = "MERCHANT";
            frmSearchField.isCheckBoxes = false;
            frmSearchField.isPreview = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsSearch.ClassMerchantID = clsSearch.ClassParticularID;
                txtMerchantID.Text = clsSearch.ClassMerchantID.ToString();
                txtMerchantName.Text = txtRegisteredName.Text = txtTradeName.Text = clsSearch.ClassParticularName;

                FillMerchantTextBox();
            }
        }

        private void btnSearchSales_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iFE;
            frmSearchField.sHeader = "EMPLOYEE";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsSearch.ClassSubmitID = clsSearch.ClassParticularID;
                txtSalesName.Text = clsSearch.ClassParticularName;
            }
        }

        private void FillMerchantTextBox()
        {
            dbAPI.ExecuteAPI("GET", "Search", "Particular Info", dbFunction.CheckAndSetNumericValue(txtMerchantID.Text), "Get Info Detail", "", "GetInfoDetail");

            dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 1);

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

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (!fEdit)
            {
                if (dbAPI.isRecordExist("Search", "MSP Merchant", dbFunction.CheckAndSetStringValue(txtMerchantName.Text)))
                {
                    dbFunction.SetMessageBox("Merchant " + dbFunction.AddBracketStartEnd(txtMerchantName.Text) + " is already onboard.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return;
                }
            }

            if (!ValidateFields()) return;
            
            if (!dbFunction.fPromptConfirmation("Are you sure to " + (fEdit ? "update" : "save" + " " + "Merchant Onboarding" + "\n\n" +
                "[MSP Information]" + "\n" +
                ">Merchant Name: " + txtMerchantName.Text + "\n" +
                ">Scheme: " + cboSchemeType.Text + "\n" +
                ">Status" + cboMSPStatus.Text))) return;

            if (!fEdit)
            {
                saveParticular();

                saveMSPMaster();

                dbFunction.SetMessageBox("Merchant onboarding successfully saved.", "Saved", clsFunction.IconType.iInformation);
            }
            else
            {
                // update tblparticular
                clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtMerchantID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtMerchantName.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtMerchantAddress.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtContactPerson.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtContactPosition.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtContactNo.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtContactEmail.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtContactNo.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtContactNo.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtRegionID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtMerchantProvince.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtRegionType.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtMerchantRegion.Text);


                dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                dbAPI.ExecuteAPI("PUT", "Update", "Particular-MSP", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                // update tblmspmaster
                clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtMSPNo.Text) + clsFunction.sPipe +
                                             dbFunction.CheckAndSetNumericValue(txtClientID.Text) + clsFunction.sPipe +
                                             dbFunction.CheckAndSetNumericValue(txtMerchantID.Text) + clsFunction.sPipe +
                                             clsSearch.ClassCurrentParticularID + clsFunction.sPipe +
                                             dbFunction.getCurrentDateTime() + clsFunction.sPipe +
                                             clsSearch.ClassSubmitID + clsFunction.sPipe +
                                             dbFunction.CheckAndSetDatePickerValueToDate(dteSubmitDate) + clsFunction.sPipe +
                                             dbFunction.CheckAndSetStringValue(txtRegisteredName.Text) + clsFunction.sPipe +
                                             dbFunction.CheckAndSetStringValue(txtTradeName.Text) + clsFunction.sPipe +
                                             dbFunction.getFileID(cboNoBType, "All Type") + clsFunction.sPipe +
                                             dbFunction.getFileID(cboBusType, "All Type") + clsFunction.sPipe +
                                             dbFunction.getFileID(cboCategoryType, "All Type") + clsFunction.sPipe +
                                             dbFunction.CheckAndSetStringValue(txtTINNo.Text) + clsFunction.sPipe +
                                             dbFunction.getFileID(cboAcqType, "All Type") + clsFunction.sPipe +
                                             dbFunction.getFileID(cboReferralType, "All Type") + clsFunction.sPipe +
                                             dbFunction.CheckAndSetNumericValue(txtNoOfPOS.Text) + clsFunction.sPipe +
                                             dbFunction.getFileID(cboMDRCreditType, "All Type") + clsFunction.sPipe +
                                             dbFunction.getFileID(cboMDRDebitType, "All Type") + clsFunction.sPipe +
                                             dbFunction.getFileID(cboMDRInstType, "All Type") + clsFunction.sPipe +
                                             dbFunction.getFileID(cboSchemeType, "All Type") + clsFunction.sPipe +
                                             dbFunction.CheckAndSetNumericValue(txtNoOfYear.Text) + clsFunction.sPipe +
                                             dbFunction.CheckAndSetNumericValue(txtRentalAmt.Text) + clsFunction.sPipe +
                                             dbFunction.getFileID(cboMSPStatus, "All Type") + clsFunction.sPipe +
                                             dbFunction.CheckAndSetStringValue(txtRemarks.Text);


                dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                dbAPI.ExecuteAPI("PUT", "Update", "MSP Master", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                dbFunction.SetMessageBox("Merchant onboarding successfully updated.", "Updated", clsFunction.IconType.iInformation);
            }
            
            // check if has uploaded document
            if (!dbAPI.isRecordExist("Search", "Import Master", txtMSPNo.Text + clsDefines.gPipe + (int)FileType.MSP))
            {
                if (!dbFunction.fPromptConfirmation("Support document has not been uploaded." + "\n\n" + "Do you want to upload?"))
                {
                    btnClear_Click(this, e);
                    return;
                }
                    
                btnUploadDocument_Click(this, e);
                btnClear_Click(this, e);

            }

            btnClear_Click(this, e);
        }

        private void saveMSPMaster()
        {
            string sSQL = "";
            
            var data = new
            {
                ReferenceNo = dbFunction.CheckAndSetStringValue(txtReferenceNo.Text),
                ClientID = dbFunction.getFileID(cboClient, "Client List"),
                MerchantID = dbFunction.CheckAndSetNumericValue(txtMerchantID.Text),                
                CreatedID = clsSearch.ClassCurrentParticularID,
                CreatedAt = dbFunction.CheckAndSetDatePickerValueToDate(dteCreatedDate),
                UpdatedID = clsSearch.ClassCurrentParticularID,
                UpdatedAt = dbFunction.getCurrentDateTime(),
                SubmitID = clsSearch.ClassSubmitID,
                SubmitAt = dbFunction.CheckAndSetDatePickerValueToDate(dteSubmitDate),                
                RegisteredName = dbFunction.CheckAndSetStringValue(txtRegisteredName.Text),
                TradeName = dbFunction.CheckAndSetStringValue(txtTradeName.Text),
                NoBTypeID = dbFunction.getFileID(cboNoBType, "All Type"),
                BusTypeID = dbFunction.getFileID(cboBusType, "All Type"),
                CategoryTypeID = dbFunction.getFileID(cboCategoryType, "All Type"),
                TINNo = dbFunction.CheckAndSetStringValue(txtTINNo.Text),
                AcqTypeID = dbFunction.getFileID(cboAcqType, "All Type"),
                ReferralTypeID = dbFunction.getFileID(cboReferralType, "All Type"),
                NoOfPOS = dbFunction.CheckAndSetNumericValue(txtNoOfPOS.Text),
                MDRCreditTypeID = dbFunction.getFileID(cboMDRCreditType, "All Type"),
                MDRDebitTypeID = dbFunction.getFileID(cboMDRDebitType, "All Type"),
                MDRInstTypeID = dbFunction.getFileID(cboMDRInstType, "All Type"),
                SchemeTypeID = dbFunction.getFileID(cboSchemeType, "All Type"),
                NoOfYear = dbFunction.CheckAndSetNumericValue(txtNoOfYear.Text),
                RentalAmt = dbFunction.CheckAndSetNumericValue(txtRentalAmt.Text),                
                MSPStatus = dbFunction.getFileID(cboMSPStatus, "All Type"),
                ProcessedID = clsSearch.ClassCurrentUserID,
                ProcessedDateTime = dbFunction.getCurrentDateTime(),
                Remarks = dbFunction.CheckAndSetStringValue(txtRemarks.Text)
            };

            sSQL = IFormat.Insert(data);

            Debug.WriteLine("saveMSPMaster" + sSQL);

            dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 0);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "MSP Master", sSQL, "InsertCollectionMaster");

            clsSearch.ClassLastInsertedID = clsLastID.ClassLastInsertedID;
            txtMSPNo.Text = clsSearch.ClassLastInsertedID.ToString();
        }

        private void initDate()
        {
            dteCreatedDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteCreatedDate, clsFunction.sStandardDateDefault);

            dteSubmitDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteSubmitDate, clsFunction.sStandardDateDefault);
            
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            btnClear_Click(this, e);

            int MSPNo = dbAPI.GetControlID("MSP Master");
            txtMSPNo.Text = dbFunction.CheckAndSetNumericValue(MSPNo.ToString());
            clsSearch.ClassMSPNo = MSPNo;

            dbAPI.GenerateID(true, txtReferenceNo, txtMSPNo, "MSP Master", clsDefines.CONTROLID_PREFIX_MSP);
            clsSearch.ClassReferenceNo = txtReferenceNo.Text;

            dbFunction.TextBoxUnLock(true, this);

            fEdit = false;
            btnAdd.Enabled = false;
            btnSave.Enabled = true;


            btnSearchMSP.Enabled = false;
            btnProvinceSearch.Enabled = true;
            btnSearchSales.Enabled = true;

            dbFunction.SetButtonIconImage(btnSearchMSP);
            dbFunction.SetButtonIconImage(btnProvinceSearch);
            dbFunction.SetButtonIconImage(btnSearchSales);

            PKTextBoxBackColor();

            txtMerchantName.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            fEdit = false;
            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            iniComboBoxSelection();
            initDate();

            InitButton();

            btnSearchMSP.Enabled = true;
            btnProvinceSearch.Enabled = false;
            btnSearchSales.Enabled = false;

            dbFunction.SetButtonIconImage(btnSearchMSP);
            dbFunction.SetButtonIconImage(btnProvinceSearch);
            dbFunction.SetButtonIconImage(btnSearchSales);

            initDocumentButton(0);

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

        private bool ValidateFields()
        {
            if (!dbFunction.isValidDescriptionEntry(txtMSPNo.Text, "MSP No." + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtMerchantName.Text, "Merchant name" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtMerchantAddress.Text, "Merchant address" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtMerchantRegion.Text, "Region" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtMerchantProvince.Text, "Province" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtContactPerson.Text, "Contact person" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtContactNo.Text, "Contact Number" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(cboClient.Text, "Client" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(cboMSPStatus.Text, "Status" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(cboMDRCreditType.Text, "MDR credit" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(cboMDRDebitType.Text, "MDR debit" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(cboMDRInstType.Text, "MDR installment" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(cboSchemeType.Text, "Scheme" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(cboCategoryType.Text, "Category" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(cboNoBType.Text, "Nature of business" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(cboBusType.Text, "Business type" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(cboAcqType.Text, "Acquirer" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(cboReferralType.Text, "Referral type" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtNoOfPOS.Text, "Number of POS" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtNoOfYear.Text, "Number of Year(s)" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtRentalAmt.Text, "Rental Amount" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtTINNo.Text, "TIN No." + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;

            return true;
        }

        private void btnUploadDocument_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidDescriptionEntry(cboClient.Text, "Client" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtMSPNo.Text, "MSP No" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtMerchantName.Text, "Merchant" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtSalesName.Text, "Sales/MSE" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            clsSearch.ClassReferenceNo = txtReferenceNo.Text;

            frmMerchantAttachment.sHeader = "MSP - UPLOAD SUPPORT DOCUMENT";
            frmMerchantAttachment frm = new frmMerchantAttachment();
            frm.ShowDialog();
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
                txtSubmitID.Text = clsSearch.ClassSubmitID.ToString();

                FillMSPMasterBox();

                FillMerchantTextBox();

                btnAdd.Enabled = false;
                btnSave.Enabled = true;

                PKTextBoxBackColor();

                Cursor.Current = Cursors.Default;
            }
        }

        private void frmMerchantOnboarding_KeyDown(object sender, KeyEventArgs e)
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

                cboAcqType.Text = data.AcquirerType;
                cboCategoryType.Text = data.Category;
                cboNoBType.Text = data.NoBType;
                cboBusType.Text = data.BusType;
                cboReferralType.Text = data.ReferralType;
                cboSchemeType.Text = data.SchemeType;
                cboMSPStatus.Text = data.MSPStatusDesc;
                cboClient.Text = data.ClientName;

                cboMDRCreditType.Text = data.MDRCreditType;
                cboMDRDebitType.Text = data.MDRDebitType;
                cboMDRInstType.Text = data.MDRInstType;

                txtSalesName.Text = data.SubmitBy;
                txtNoOfPOS.Text = data.NoOfPOS;
                txtNoOfYear.Text = data.NoOfYear;
                txtRentalAmt.Text = data.RentalAmt;

                txtRemarks.Text = data.Remarks;

                txtCreatedAt.Text = data.CreatedAt;
                txtCreatedBy.Text = data.CreatedBy;
                txtUpdatedAt.Text = data.UpdatedAt;
                txtUpdatedBy.Text = data.UpdatedBy;

                dteCreatedDate.Value = DateTime.Parse(data.CreatedAt);
                dteSubmitDate.Value = DateTime.Parse(data.SubmitAt);

                txtNoOfDoc.Text = data.NoOfDocument.ToString();
                initDocumentButton(data.NoOfDocument);

            }
        }

        private void btnProvinceSearch_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iProvince;
            frmSearchField.sHeader = "REGION/PROVINCE";            
            frmSearchField.isCheckBoxes = false;
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

        private void saveParticular()
        {
            string sSQL = "";

            var data = new
            {
                Name = dbFunction.CheckAndSetStringValue(txtMerchantName.Text),
                Address = dbFunction.CheckAndSetStringValue(txtMerchantAddress.Text),
                ContactPerson = dbFunction.CheckAndSetStringValue(txtContactPerson.Text),
                ContactPosition = dbFunction.CheckAndSetStringValue(txtContactPosition.Text),
                ContactNumber = dbFunction.CheckAndSetStringValue(txtContactNo.Text),
                Email = dbFunction.CheckAndSetStringValue(txtContactEmail.Text),
                TelNo = dbFunction.CheckAndSetStringValue(txtContactNo.Text),
                Mobile = dbFunction.CheckAndSetStringValue(txtContactNo.Text),
                RegionID = dbFunction.CheckAndSetNumericValue(txtRegionID.Text),
                Province = dbFunction.CheckAndSetStringValue(txtMerchantProvince.Text),
                RegionType = dbFunction.CheckAndSetNumericValue(txtRegionType.Text),
                Region = dbFunction.CheckAndSetStringValue(txtMerchantRegion.Text),
                ParticularTypeID = clsGlobalVariables.iMerchant_Type,
                ParticularDescription = clsGlobalVariables.sMerchant_Type

            };

            sSQL = IFormat.Insert(data);

            Debug.WriteLine("saveParticular" + sSQL);

            dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 0);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "Particular-MSP", sSQL, "InsertMaintenanceMaster");

            clsSearch.ClassLastInsertedID = clsLastID.ClassLastInsertedID;
            txtMerchantID.Text = clsSearch.ClassLastInsertedID.ToString();
        }

        private void txtMerchantName_TextChanged(object sender, EventArgs e)
        {
            txtRegisteredName.Text = txtTradeName.Text = txtMerchantName.Text;
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
                { clsDefines.TAG_ClientName, cboClient.Text },
                { clsDefines.TAG_RegisteredName, txtRegisteredName.Text },
                { clsDefines.TAG_TradeName, txtTradeName.Text },
                { clsDefines.TAG_TINNo, txtTINNo.Text },
                { clsDefines.TAG_AcquirerType, cboAcqType.Text },
                { clsDefines.TAG_BusType, cboBusType.Text },
                { clsDefines.TAG_NoBType, cboNoBType.Text },
                { clsDefines.TAG_SchemeType, cboSchemeType.Text },
                { clsDefines.TAG_ReferralType, cboReferralType.Text },
                { clsDefines.TAG_Category, cboCategoryType.Text },
                { clsDefines.TAG_MDRCreditType, cboMDRCreditType.Text },
                { clsDefines.TAG_MDRDebitType, cboMDRDebitType.Text },
                { clsDefines.TAG_MDRInstType, cboMDRInstType.Text },
                { clsDefines.TAG_CreatedAt, txtCreatedAt.Text },
                { clsDefines.TAG_CreatedBy, txtCreatedBy.Text },
                { clsDefines.TAG_UpdatedAt, txtUpdatedAt.Text },
                { clsDefines.TAG_UpdatedBy, txtUpdatedBy.Text },
                { clsDefines.TAG_MSPStatusDesc, cboMSPStatus.Text },
                { clsDefines.TAG_SubmitAt, dbFunction.CheckAndSetDate(dteSubmitDate.Value.ToShortDateString()) },
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

            dbAPI.ExecuteAPI("POST", "Notify", "MSP Onboarding", jsonString, "Email Notification", "", "EmailNotification");

            dbFunction.SetMessageBox("MSP email notification complete.", "Email Notification - Merchant Onboarding", clsFunction.IconType.iInformation);

            Cursor.Current = Cursors.Default;
        }

        private void PKTextBoxBackColor()
        {
            txtMSPNo.BackColor = clsFunction.PKBackColor;
            txtReferenceNo.BackColor = clsFunction.MKBackColor;
        }

        private void initDocumentButton(int pDocCount)
        {
            btnUploadDocument.Text = "UPLOAD DOCUMENT" + " " + dbFunction.AddParenthesisStartEnd(pDocCount.ToString());
        }
    }
}
