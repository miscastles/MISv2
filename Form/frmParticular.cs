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
using System.IO;
using System.Drawing.Imaging;
using System.Net;
using static MIS.Function.AppUtilities;

namespace MIS
{
    public partial class frmParticular : Form
    {
        public static int iParticularType;
        private string sParticularDescription;
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        private clsFile dbFile;
        string sParticularType = "";

        bool fEdit = false;
        private string reqNBI { get; set; }
        private string reqSchool { get; set; }
        private string reqAppForm { get; set; }
        private string reqWaiver { get; set; }


        protected override CreateParams CreateParams
        {
            // Override CreateParams to enable double-buffering for child controls
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED
                //cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        public frmParticular()
        {
            InitializeComponent();
        }
        private void ClearListView()
        {
            lvwList.Items.Clear();
        }
        private void ClearTextBox()
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is TextBox)
                        (control as TextBox).Clear();
                    else
                        func(control.Controls);
            };

            func(Controls);
        }

        private void ClearComboBox()
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is ComboBox)
                    {
                        (control as ComboBox).Items.Clear();
                        (control as ComboBox).Text = "";
                    }
                    else
                    {
                        func(control.Controls);
                    }
            };

            func(Controls);
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
            /*
            bool fValid = false;

            if (txtName.Text.Length > 0 &&
                //txtProvince.Text.Length > 0 &&
                //txtRegion.Text.Length > 0 &&
                dbFunction.isValidID(txtCountryID.Text) &&
                (iParticularType == clsGlobalVariables.iFE_Type ? (txtEmail.Text.Length > 0 ? true : false) : true) &&
                (iParticularType == clsGlobalVariables.iFE_Type ? (txtMobile.Text.Length > 0 ? true : false) : true))
                fValid = true;

            if (!fValid)
            {
                MessageBox.Show("Check the following field(s) listed below:\n\n" +
                                "*Name\n" +
                                //"*Country\n" +
                                //"*City\n" +
                                //"*Province\n" +
                                (iParticularType == clsGlobalVariables.iFE_Type ? "*Email\n*Mobile\n" : "") +                                
                                "\n" +
                                "Field(s) with asterisk(*) must not be blank.", "Incomplete Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            */

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iName, txtName.Text)) return false;
            //if (!dbFunction.isValidEntry(clsFunction.CheckType.iCity, txtProvince.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRegionType, txtRegionType.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRegionID, txtRegionID.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iProvince, txtProvince.Text)) return false;
            //if (!dbFunction.isValidEntry(clsFunction.CheckType.iMobileNo, txtMobile.Text)) return false;
            //if (!dbFunction.isValidEntry(clsFunction.CheckType.iEmail, txtEmail.Text)) return false;

            if (iParticularType == clsGlobalVariables.iFE_Type)
            {
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iDepartment, cboDepartment.Text)) return false;
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iPosition, cboPosition.Text)) return false;
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iEmploymentStatus, cboEmploymentStatus.Text)) return false;
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iMobileNo, txtMobile.Text)) return false;
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iEmail, txtEmail.Text)) return false;
            }

            return true;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            fEdit = false;
            ClearTextBox();
            InitButton();
            btnSave.Enabled = true;
            btnAdd.Enabled = false;
            SetParticularName();

            dbFunction.TextBoxUnLock(true, this);
            dbFunction.ComBoBoxUnLock(true, this);

            btnProvinceSearch.Enabled = true;
            btnAddProvince.Enabled = true;

            // ROCKY - PARTICULAR: ADDITIONAL PARTICULAR INFO - ADD GENDER COMBOBOX
            FillGenderComboBox(cboGender);
            FillGenderComboBox(cboEducLevel);
            dtpDateResign.Value = new DateTime(2000, 1, 1);

            btnUploadAppForm.Enabled = true;
            btnUploadSchoolReq.Enabled = true;
            btnUploadWaiver.Enabled = true;
            btnUploadNBI.Enabled = true;

            btnPrevAppForm.Enabled = true;
            btnPrevNbi.Enabled = true;
            btnPrevWaiver.Enabled = true;
            btnPrevSchoolReq.Enabled = true;

            //dbAPI.FillComboBoxEmploymentStatus(cboEmploymentStatus);

            txtName.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            fEdit = false;
            ClearTextBox();
            //ClearComboBox();
            InitButton();
            SetParticularName();
            InitCheckBox();
            //LoadParticular("View", sParticularType, "");            

            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);

            btnProvinceSearch.Enabled = false;
            btnAddProvince.Enabled = false;

            //dbAPI.FillComboBoxEmploymentStatus(cboEmploymentStatus);

            // set compbobox selected index
            cboDepartment.Text = clsFunction.sDefaultSelect;
            cboPosition.Text = clsFunction.sDefaultSelect;
            cboEmploymentStatus.Text = clsFunction.sDefaultSelect;

            cboRentalType.Text = clsFunction.sDefaultSelect;
            cboRentalTerms.Text = clsFunction.sDefaultSelect;

            PKTextBoxBackColor(true);

            btnLoadImage.Enabled = btnResetImage.Enabled = false;
            btnResetImage_Click(this, e);

            // ROCKY - PARTICULAR: ADDITIONAL PARTICULAR INFO - CLEAR GENDER COMBOBOX
            FillGenderComboBox(cboGender);
            FillGenderComboBox(cboEducLevel);
            dtpDateResign.Value = new DateTime(2000, 1, 1);

            // ROCKY - PARTICULAR: ADDITIONAL PARTICULAR REQUIREMENTS INFO - CLEAR
            picRequirements.BackgroundImage = null;

            btnUploadAppForm.Enabled = false;
            btnUploadSchoolReq.Enabled = false;
            btnUploadWaiver.Enabled = false;
            btnUploadNBI.Enabled = false;

            btnPrevAppForm.Enabled = false;
            btnPrevNbi.Enabled = false;
            btnPrevWaiver.Enabled = false;
            btnPrevSchoolReq.Enabled = false;

            txtParticularID.ReadOnly = true;

            Cursor.Current = Cursors.Default;
        }

        private void LoadParticular(string StatementType, string SearchBy, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            ClearListView();

            clsSearch.ClassParticularID = 0;
            clsSearch.ClassProvinceID = 0;
            clsSearch.ClassCityID = 0;
            clsSearch.ClassParticularTypeID = iParticularType;
            clsSearch.ClassParticularTypeDescription = sParticularDescription;
            clsSearch.ClassParticularName = "0";

            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassParticularID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassProvinceID + clsFunction.sPipe +
                                                clsSearch.ClassCityID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassParticularTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassParticularTypeDescription + clsFunction.sPipe +
                                                clsSearch.ClassParticularName.ToString();

            Debug.WriteLine("LoadParticular::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbAPI.ExecuteAPI("GET", "View", "Advance Particular", clsSearch.ClassAdvanceSearchValue, "Particular", "", "ViewAdvanceParticular");


            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.ParticularID.Length > i)
                {

                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.ParticularID[i].ToString());
                    item.SubItems.Add(clsArray.RegionID[i].ToString());
                    item.SubItems.Add(clsArray.RegionType[i].ToString());
                    item.SubItems.Add(clsArray.ParticularTypeID[i].ToString());
                    item.SubItems.Add(clsArray.ParticularDescription[i]);
                    item.SubItems.Add(clsArray.ParticularName[i]);
                    item.SubItems.Add(clsArray.Address[i]);
                    item.SubItems.Add(clsArray.Address2[i]);
                    item.SubItems.Add(clsArray.Address3[i]);
                    item.SubItems.Add(clsArray.Address4[i]);
                    item.SubItems.Add(clsArray.Region[i]);
                    item.SubItems.Add(clsArray.Province[i]);
                    item.SubItems.Add(clsArray.ContactPerson[i]);
                    item.SubItems.Add(clsArray.TelNo[i]);
                    item.SubItems.Add(clsArray.MobileNo[i]);
                    item.SubItems.Add(clsArray.Fax[i]);
                    item.SubItems.Add(clsArray.Email[i]);
                    item.SubItems.Add(clsArray.ContractTerms[i]);
                    lvwList.Items.Add(item);

                    i++;
                }
            }

            dbFunction.ListViewAlternateBackColor(lvwList);

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int ID = 0;
            string sRowSQL = "";
            string sSQL = "";

            if (!ValidateFields()) return;

            if (!fEdit)
            {
                if (CheckParticular()) return; // Check Name No already exist

                if (!dbFunction.fSavingConfirm(false)) return;
            }
            else
            {
                if (!dbFunction.fSavingConfirm(true)) return;

                ID = int.Parse(txtParticularID.Text);
            }

            Cursor.Current = Cursors.WaitCursor;

            if (!fEdit)
            {
                sRowSQL = "";
                sRowSQL = " ('" + dbFunction.CheckAndSetStringValue(StrClean(txtName.Text)) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtCode.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(StrClean(txtAddress.Text)) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(StrClean(txtAddress2.Text)) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(StrClean(txtAddress3.Text)) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(StrClean(txtAddress4.Text)) + "', " +
                sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(txtProvinceID.Text) + ", " +
                sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(txtCityID.Text) + ", " +
                sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(txtParticularTypeID.Text) + ", " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(sParticularDescription).ToUpper() + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(StrClean(txtContactPerson.Text)) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtTelNo.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtMobile.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtFax.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtEmail.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtContractTerms.Text) + "', " +
                sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(txtRegionID.Text) + ", " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtProvince.Text) + "', " +
                sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(txtRegionType.Text) + ", " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtRegion.Text) + "', " +
                sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(txtDepartmentID.Text) + ", " +
                sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(txtPositionID.Text) + ", " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(cboEmploymentStatus.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtCode.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtComputerName.Text) + "', " +
                sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(txtRentalType.Text) + ", " +
                sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(txtRentalTerms.Text) + ", " +
                sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetBooleanValue(chkActive.Checked) + ", " +
                sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetBooleanValue(chkWorkArrangement.Checked) + ", " +
                sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetBooleanValue(chkTimeSheet.Checked) + ", " +
                sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetBooleanValue(chkVersionApp.Checked) + ", " +

                // ROCKY - PARTICULAR: ADDITIONAL PARTICULAR INFO - SAVE
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtJobDesc.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetDatePickerValueToDate(dtpDateHired) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtTINNo.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtPhilHealthNo.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtSSSNo.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtHDMFNo.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(cboEducLevel.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtPresentID.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetDatePickerValueToDate(dtpBirthDate) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(cboGender.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetDatePickerValueToDate(dtpDateResign) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtPresIDNo.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtAccountInfo.Text) + "', " +

                // ROCKY - PARTICULAR: ADDITIONAL PARTICULAR REQUIREMENTS INFO - SAVE
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtNbiClearance.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtSchoolCred.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtApplicationForm.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtWaiverForm.Text) + "') ";

                sSQL = sSQL + sRowSQL;
                //sSQL = sSQL.Replace("&", "AND");

                Debug.WriteLine("sSQL=" + sSQL);

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "Particular", sSQL, "InsertMaintenanceMaster");

                txtParticularID.Text = clsLastID.ClassLastInsertedID.ToString();

                MessageBox.Show("New Particular successfully saved", "Saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }
            else
            {
                clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtParticularID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(StrClean(txtName.Text)) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(StrClean(txtAddress.Text)) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(StrClean(txtAddress2.Text)) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(StrClean(txtAddress3.Text)) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(StrClean(txtAddress4.Text)) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtProvinceID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtCityID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtParticularTypeID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtParticularDescription.Text.ToUpper()) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(StrClean(txtContactPerson.Text)) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtTelNo.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtMobile.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtFax.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtEmail.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtContractTerms.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtRegionID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtProvince.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtRegionType.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtRegion.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtDepartmentID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtPositionID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(cboEmploymentStatus.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtCode.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtComputerName.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetBooleanValue(chkActive.Checked) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetBooleanValue(chkWorkArrangement.Checked) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetBooleanValue(chkTimeSheet.Checked) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetBooleanValue(chkVersionApp.Checked) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtRentalType.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtRentalTerms.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtAcntNo.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtCustomerNo.Text) + clsFunction.sPipe +

                                                    // ROCKY - PARTICULAR: ADDITIONAL PARTICULAR INFO - GET
                                                    dbFunction.CheckAndSetStringValue(txtJobDesc.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetDatePickerValueToDate(dtpDateHired) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtTINNo.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtPhilHealthNo.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtSSSNo.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtHDMFNo.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(cboEducLevel.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtPresentID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(cboGender.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetDatePickerValueToDate(dtpDateResign) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtPresIDNo.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtAccountInfo.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetDatePickerValueToDate(dtpBirthDate) + clsFunction.sPipe +

                                                    // ROCKY - PARTICULAR: ADDITIONAL PARTICULAR REQUIREMENTS INFO - GET
                                                    dbFunction.CheckAndSetStringValue(txtNbiClearance.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtSchoolCred.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtApplicationForm.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtWaiverForm.Text);

                //clsSearch.ClassAdvanceSearchValue = clsSearch.ClassAdvanceSearchValue.Replace("&", "AND");

                Debug.WriteLine("clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                dbAPI.ExecuteAPI("PUT", "Update", "Particular", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                MessageBox.Show("Particular has been successfully modified", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }

            // for employee
            if (iParticularType == clsGlobalVariables.iFE_Type)
            {
                //SaveBitmap();

                //UploadImage();

                UploadRequirementToServer(txtNbiClearance.Text);
                UploadRequirementToServer(txtSchoolCred.Text);
                UploadRequirementToServer(txtApplicationForm.Text);
                UploadRequirementToServer(txtWaiverForm.Text);
            }

            Cursor.Current = Cursors.Default;

            btnClear_Click(this, e);
        }

        private void frmParticular_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            dbFile = new clsFile();

            ClearTextBox();
            ClearComboBox();
            InitButton();
            InitCheckBox();
            SetParticularName();
            //LoadParticular("View", "", "");                        

            dbAPI.FillComboBoxDepartment(cboDepartment);
            dbAPI.FillComboBoxEmploymentStatus(cboEmploymentStatus);

            dbAPI.FillComboBoxRentalType(cboRentalType);
            dbAPI.FillComboBoxRentalTerm(cboRentalTerms);

            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);

            btnProvinceSearch.Enabled = false;
            btnAddProvince.Enabled = false;

            grpParticular.Enabled = false;

            if (iParticularType == clsGlobalVariables.iFE_Type)
            {
                grpParticular.Enabled = true;
                grpEmployee.Enabled = true;
                tcEmployee.Enabled = true;
            }

            grpMerchant.Enabled = false;

            if (iParticularType == clsGlobalVariables.iMerchant_Type_List)
            {
                grpMerchant.Enabled = true;
                tabParticular.TabPages.Remove(tabPage2);
            }

            // ROCKY - PARTICULAR: REMOVE PARTICULAR TAB  
            if (iParticularType == clsGlobalVariables.iMerchant_Type)
            {
                grpMerchant.Enabled = true;
                tabParticular.TabPages.Remove(tabPage2);
            }

            // ROCKY - PARTICULAR: REMOVE PARTICULAR TAB 
            if (iParticularType == clsGlobalVariables.iClient_Type)
            {
                grpMerchant.Enabled = true;
                tabParticular.TabPages.Remove(tabPage2);
            }

            cboDepartment.Text = clsFunction.sDefaultSelect;
            cboPosition.Text = clsFunction.sDefaultSelect;
            cboEmploymentStatus.Text = clsFunction.sDefaultSelect;

            // For merchant
            cboRentalType.Text = clsFunction.sDefaultSelect;
            cboRentalTerms.Text = clsFunction.sDefaultSelect;

            PKTextBoxBackColor(true);

            btnLoadImage.Enabled = btnResetImage.Enabled = false;
            btnResetImage_Click(this, e);

            // ROCKY - PARTICULAR: ADDITIONAL PARTICULAR INFO - GENDER COMBOBOX
            FillGenderComboBox(cboGender);
            FillGenderComboBox(cboEducLevel);
            dtpDateResign.Value = new DateTime(2000, 1, 1);

            txtParticularID.ReadOnly = true;
        }

        private void SetParticularName()
        {
            string sPrefix = "ENROLLMENT";

            if (iParticularType == clsGlobalVariables.iMerchant_Type)
            {
                sParticularType = clsGlobalVariables.sMerchant_Type;
            }

            if (iParticularType == clsGlobalVariables.iClient_Type)
            {
                sParticularType = clsGlobalVariables.sClient_Type;
            }

            if (iParticularType == clsGlobalVariables.iFE_Type)
            {
                // sParticularType = clsGlobalVariables.sFE_Type;
                sParticularType = clsGlobalVariables.sEMP_Type;
            }

            if (iParticularType == clsGlobalVariables.iSupplier_Type)
            {
                sParticularType = clsGlobalVariables.sSupplier_Type;
            }

            if (iParticularType == clsGlobalVariables.iSP_Type)
            {
                sParticularType = clsGlobalVariables.sSP_Type;
            }

            if (iParticularType == clsGlobalVariables.iEMP_Type)
            {
                sParticularType = clsGlobalVariables.sEMP_Type;
            }

            if (iParticularType == clsGlobalVariables.iMerchant_Type_List)
            {
                sParticularType = clsGlobalVariables.sMerchant_Type;
            }

            lblParticular.Text = sPrefix + " - " + sParticularType;
            sParticularDescription = sParticularType;
            txtParticularTypeID.Text = iParticularType.ToString();
            txtParticularDescription.Text = sParticularType;
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.SelectedItems[0].SubItems[1].Text.Length > 0)
            {
                string sParticularID = lvwList.SelectedItems[0].SubItems[1].Text;

                if (dbFunction.isValidID(sParticularID))
                {
                    clsSearch.ClassParticularID = int.Parse(sParticularID);
                    PopulateParticularTextBox();

                    if (txtParticularID.Text.Length > 0 && txtName.Text.Length > 0)
                        fEdit = true;

                    InitButton();

                    btnProvinceSearch.Enabled = true;
                    btnAddProvince.Enabled = true;

                    dbFunction.TextBoxUnLock(true, this);
                    dbFunction.ComBoBoxUnLock(true, this);
                }
            }
        }

        private void lvwList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwList.Items.Count > 0)
            {

            }
        }

        private void btnAddProvince_Click(object sender, EventArgs e)
        {
            frmRegionDetail frm = new frmRegionDetail();
            frm.ShowDialog();
        }

        private void frmParticular_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1: // New
                    btnAdd_Click(this, e);
                    break;
                case Keys.F2: // Sesrch               
                    btnSearchParticular_Click(this, e);
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void btnProvinceSearch_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iProvince;
            frmSearchField.sHeader = "VIEW CITY";
            frmSearchField.sSearchChar = txtRegion.Text;
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                txtRegionID.Text = clsSearch.ClassRegionID.ToString();
                txtRegionType.Text = clsSearch.ClassRegionType.ToString();
                txtProvince.Text = clsSearch.ClassProvince;
                txtRegion.Text = clsSearch.ClassRegion;
            }
        }

        private void txtProvince_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtProvince_Click(object sender, EventArgs e)
        {
            //btnProvinceSearch_Click(this, e);
        }

        private bool CheckParticular()
        {
            bool fExist = false;
            
            fExist = dbAPI.isRecordExist("Search", "Particular Name", txtName.Text + clsDefines.gPipe + txtRegionType.Text + clsDefines.gPipe + txtRegionID.Text + clsDefines.gPipe + txtAddress.Text);

            if (fExist)
            {
                dbFunction.SetMessageBox("Unable to save particular." +
                            "\n\n" +
                            "Name: " + txtName.Text + "\n" +
                            "Region: " + txtRegion.Text + "\n" +
                            "City: " + txtProvince.Text + "\n" +
                            "Address: " + txtAddress.Text + "\n" +
                            "\n", "Already exist.", clsFunction.IconType.iWarning);


            }

            return fExist;
        }

        private void txtTelNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtMobile_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtFax_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void PopulateParticularTextBox()
        {
            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassParticularID.ToString() + clsFunction.sPipe +
                                                clsFunction.sZero;

            Debug.WriteLine("PopulateParticularTextBox::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbAPI.ExecuteAPI("GET", "Search", "Particular Detail", clsSearch.ClassAdvanceSearchValue, "Particular Detail", "", "ViewParticularDetail");

            // ROCKY - PARTICULAR ISSUE: ADD FIX FOR SELECTION IN GETTING PARTICULAR INFO REMOVES EMP RETRIEVAL FOR CLIENT OR MERCHANT
            if (iParticularType == clsGlobalVariables.iClient_Type || iParticularType == clsGlobalVariables.iMerchant_Type_List || iParticularType == clsGlobalVariables.iMerchant_Type )
            {
                if (dbAPI.isNoRecordFound() == false)
                {
                    clsParticular.RecordFound = true;
                    txtParticularID.Text = clsParticular.ClassParticularID.ToString();
                    txtProvinceID.Text = clsParticular.ClassProvinceID.ToString();
                    txtCityID.Text = clsParticular.ClassCityID.ToString();
                    txtParticularTypeID.Text = clsParticular.ClassParticularTypeID.ToString();
                    txtName.Text = clsParticular.ClassParticularName;
                    txtAddress.Text = clsParticular.ClassAddress;
                    txtAddress2.Text = clsParticular.ClassAddress2;
                    txtAddress3.Text = clsParticular.ClassAddress3;
                    txtAddress4.Text = clsParticular.ClassAddress4;
                    txtContactPerson.Text = clsParticular.ClassContactPerson;
                    txtTelNo.Text = clsParticular.ClassTelNo;
                    txtMobile.Text = clsParticular.ClassMobile;
                    txtFax.Text = clsParticular.ClassFax;
                    txtEmail.Text = clsParticular.ClassEmail;
                    txtContractTerms.Text = clsParticular.ClassContractTerms;
                    txtRegionID.Text = clsParticular.ClassRegionID.ToString();
                    txtProvince.Text = clsParticular.ClassProvince;
                    txtRegionType.Text = clsParticular.ClassRegionType.ToString();
                    txtRegion.Text = clsParticular.ClassRegion;

                    // POS Rental
                    txtRentalType.Text = clsParticular.ClassRentalType.ToString();
                    txtRentalTerms.Text = clsParticular.ClassRentalTerms.ToString();

                    if (iParticularType == clsGlobalVariables.iMerchant_Type_List)
                    {
                        // Get Type Info
                        if (dbFunction.isValidID(txtRentalType.Text))
                        {
                            dbAPI.ExecuteAPI("GET", "Search", "Type Info", txtRentalType.Text, "Get Info Detail", "", "GetInfoDetail");
                            if (dbAPI.isNoRecordFound() == false)
                            {
                                cboRentalType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                            }
                        }

                        // Get Type Info
                        if (dbFunction.isValidID(txtRentalTerms.Text))
                        {
                            dbAPI.ExecuteAPI("GET", "Search", "Type Info", txtRentalTerms.Text, "Get Info Detail", "", "GetInfoDetail");
                            if (dbAPI.isNoRecordFound() == false)
                            {
                                cboRentalTerms.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                            }
                        }
                    }

                    txtAcntNo.Text = clsParticular.ClassAccountNo;
                    txtCustomerNo.Text = clsParticular.ClassCustomerNo;
                }
            }
            else
            {
                if (dbAPI.isNoRecordFound() == false)
                {

                    clsParticular.RecordFound = true;
                    txtParticularID.Text = clsParticular.ClassParticularID.ToString();
                    txtProvinceID.Text = clsParticular.ClassProvinceID.ToString();
                    txtCityID.Text = clsParticular.ClassCityID.ToString();
                    txtParticularTypeID.Text = clsParticular.ClassParticularTypeID.ToString();
                    txtName.Text = clsParticular.ClassParticularName;
                    txtAddress.Text = clsParticular.ClassAddress;
                    txtAddress2.Text = clsParticular.ClassAddress2;
                    txtAddress3.Text = clsParticular.ClassAddress3;
                    txtAddress4.Text = clsParticular.ClassAddress4;
                    txtContactPerson.Text = clsParticular.ClassContactPerson;
                    txtTelNo.Text = clsParticular.ClassTelNo;
                    txtMobile.Text = clsParticular.ClassMobile;
                    txtFax.Text = clsParticular.ClassFax;
                    txtEmail.Text = clsParticular.ClassEmail;
                    txtContractTerms.Text = clsParticular.ClassContractTerms;
                    txtRegionID.Text = clsParticular.ClassRegionID.ToString();
                    txtProvince.Text = clsParticular.ClassProvince;
                    txtRegionType.Text = clsParticular.ClassRegionType.ToString();
                    txtRegion.Text = clsParticular.ClassRegion;

                    txtDepartmentID.Text = clsParticular.ClassDepartmentID.ToString();
                    cboDepartment.Text = clsParticular.ClassDepartment;
                    txtPositionID.Text = clsParticular.ClassPositionID.ToString();
                    cboPosition.Text = clsParticular.ClassPosition;
                    cboEmploymentStatus.Text = clsParticular.ClassEmploymentStatus;
                    txtCode.Text = clsParticular.ClassCode;

                    txtComputerName.Text = clsParticular.ClassComputerName;
                    chkActive.Checked = clsParticular.ClassisActive;
                    chkWorkArrangement.Checked = clsParticular.ClassisWorkArrangement;
                    chkTimeSheet.Checked = clsParticular.ClassisTimeSheet;
                    chkVersionApp.Checked = clsParticular.ClassisAppVersion;

                    // ROCKY - PARTICULAR: ADDITIONAL PARTICULAR INFO
                    dbAPI.ExecuteAPI("GET", "Search", "Staff Info Detail", clsSearch.ClassAdvanceSearchValue, "Get Info Detail", "", "GetInfoDetail");

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtJobDesc.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "JobDescription");
                        dtpDateHired.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "OnBoardDate");
                        txtTINNo.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "TINNo");
                        txtPhilHealthNo.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "PhilhealthNo");
                        txtSSSNo.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "SSSNo");
                        txtHDMFNo.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "HDMFNo");
                        cboEducLevel.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "EducLevel");
                        txtPresentID.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "PresentIDNo");
                        txtPresIDNo.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "PresentID");
                        dtpBirthDate.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "BirthDate");
                        cboGender.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "Gender");
                        dtpDateResign.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "ResignDate");
                        txtAccountInfo.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "AccountInfo");

                        txtNbiClearance.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "NbiReq");
                        txtWaiverForm.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "WaiverReq");
                        txtApplicationForm.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "AppFormReq");
                        txtSchoolCred.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "SchoolReq");
                    }

                }
                else
                {
                    clsParticular.RecordFound = false;
                }
            }
            
        }

        private void SearchParticular()
        {
            Debug.WriteLine("SearchParticular");
            Debug.WriteLine("iParticularType=" + iParticularType);

            if (iParticularType == clsGlobalVariables.iMerchant_Type)
            {
                frmSearchField.iSearchType = frmSearchField.SearchType.iMerchant;
                frmSearchField.sHeader = "MERCHANT";
                frmSearchField.isCheckBoxes = false;
                frmSearchField.isPreview = false;
                frmSearchField frm = new frmSearchField();
                frm.ShowDialog();

                if (frmSearchField.fSelected)
                {
                    txtParticularID.Text = clsSearch.ClassParticularID.ToString();
                    txtName.Text = clsSearch.ClassParticularName;
                }
            }

            if (iParticularType == clsGlobalVariables.iMerchant_Type_List)
            {
                frmSearchField.iSearchType = frmSearchField.SearchType.iMerchantList;
                frmSearchField.sHeader = "MERCHANT";
                frmSearchField frm = new frmSearchField();
                frm.ShowDialog();

                if (frmSearchField.fSelected)
                {
                    txtParticularID.Text = clsSearch.ClassParticularID.ToString();
                    txtName.Text = clsSearch.ClassParticularName;
                }
            }

            if (iParticularType == clsGlobalVariables.iClient_Type)
            {
                frmSearchField.iSearchType = frmSearchField.SearchType.iClient;
                frmSearchField.sHeader = "CLIENT";
                frmSearchField.isCheckBoxes = false;
                frmSearchField frm = new frmSearchField();
                frm.ShowDialog();

                if (frmSearchField.fSelected)
                {
                    txtParticularID.Text = clsSearch.ClassParticularID.ToString();
                    txtName.Text = clsSearch.ClassParticularName;
                }
            }

            if (iParticularType == clsGlobalVariables.iFE_Type)
            {
                frmSearchField.iSearchType = frmSearchField.SearchType.iFE;
                frmSearchField.sHeader = "EMPLOYEE";
                frmSearchField.isCheckBoxes = false;
                frmSearchField frm = new frmSearchField();
                frm.ShowDialog();

                if (frmSearchField.fSelected)
                {
                    txtParticularID.Text = clsSearch.ClassParticularID.ToString();
                    txtName.Text = clsSearch.ClassParticularName;
                }
            }

            if (iParticularType == clsGlobalVariables.iSupplier_Type)
            {
                frmSearchField.iSearchType = frmSearchField.SearchType.iSupplier;
                frmSearchField.sHeader = "SUPPLIER";
                frmSearchField.isCheckBoxes = false;
                frmSearchField frm = new frmSearchField();
                frm.ShowDialog();

                if (frmSearchField.fSelected)
                {
                    txtParticularID.Text = clsSearch.ClassParticularID.ToString();
                    txtName.Text = clsSearch.ClassParticularName;
                }
            }

            if (iParticularType == clsGlobalVariables.iSP_Type)
            {
                frmSearchField.iSearchType = frmSearchField.SearchType.iSP;
                frmSearchField.sHeader = "SERVICE PROVIDER";
                frmSearchField.isCheckBoxes = false;
                frmSearchField frm = new frmSearchField();
                frm.ShowDialog();

                if (frmSearchField.fSelected)
                {
                    txtParticularID.Text = clsSearch.ClassParticularID.ToString();
                    txtName.Text = clsSearch.ClassParticularName;
                }
            }

            if (iParticularType == clsGlobalVariables.iEMP_Type)
            {
                frmSearchField.iSearchType = frmSearchField.SearchType.iEMP;
                frmSearchField.sHeader = "EMPLOYEE";
                frmSearchField.isCheckBoxes = false;
                frmSearchField frm = new frmSearchField();
                frm.ShowDialog();

                if (frmSearchField.fSelected)
                {
                    txtParticularID.Text = clsSearch.ClassParticularID.ToString();
                    txtName.Text = clsSearch.ClassParticularName;
                }
            }
        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtAddress2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtAddress3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtAddress4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtContactPerson_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtEmail_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtContractTerms_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void bunifuCards2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnSearchParticular_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            SearchParticular();

            if (dbFunction.isValidID(txtParticularID.Text))
            {
                dbFunction.TextBoxUnLock(true, this);
                dbFunction.ComBoBoxUnLock(true, this);

                clsSearch.ClassParticularID = int.Parse(txtParticularID.Text);

                PopulateParticularTextBox();
                fEdit = true;

                InitButton();

                btnProvinceSearch.Enabled = true;
                btnAddProvince.Enabled = true;

                PKTextBoxBackColor(false);

                txtName.ReadOnly = false;
                
                if (iParticularType == clsGlobalVariables.iFE_Type)
                {
                    btnLoadImage.Enabled = btnResetImage.Enabled = true;

                    txtFileName.Text = dbFunction.padLeftChar(txtParticularID.Text, clsFunction.sZero, 6) + ".bmp";

                    DownloadImage(); // download from ftp

                    LoadProfileImage(); // load image

                    // ROCKY - PARTICULAR: ADDITIONAL PARTICULAR REQUIREMENTS INFO - UPLOAD IMAGES
                    DownloadRequirementFromServer($"NBI_{txtCode.Text}");
                    DownloadRequirementFromServer($"SCRED_{txtCode.Text}");
                    DownloadRequirementFromServer($"APPFORM_{txtCode.Text}");
                    DownloadRequirementFromServer($"WAIVER_{txtCode.Text}");
                }

                btnPrevNbi.Enabled = true;
                btnPrevSchoolReq.Enabled = true;
                btnPrevAppForm.Enabled = true;
                btnPrevWaiver.Enabled = true;

                btnUploadNBI.Enabled = true;
                btnUploadAppForm.Enabled = true;
                btnUploadWaiver.Enabled = true;
                btnUploadSchoolReq.Enabled = true;
            }

            Cursor.Current = Cursors.Default;
        }

        private void cboDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassDepartmentID = 0;
            if (!cboDepartment.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Department", cboDepartment.Text);
                clsSearch.ClassDepartmentID = clsSearch.ClassOutFileID;

                Debug.WriteLine("clsSearch.ClassDepartmentID=" + clsSearch.ClassDepartmentID);

                dbAPI.FillComboBoxPosition(cboPosition, "Position By Department");

            }

            txtDepartmentID.Text = clsSearch.ClassDepartmentID.ToString();
        }

        private void cboPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassPositionID = 0;
            if (!cboPosition.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Position", cboPosition.Text);
                clsSearch.ClassPositionID = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassPositionID=" + clsSearch.ClassPositionID);
            }

            txtPositionID.Text = clsSearch.ClassPositionID.ToString();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void InitCheckBox()
        {
            chkActive.Checked = true;
            chkTimeSheet.Checked = false;
            chkVersionApp.Checked = true;
            chkWorkArrangement.Checked = false;
        }

        private void PKTextBoxBackColor(bool isLock)
        {
            if (isLock)
            {
                txtName.BackColor = clsFunction.DisableBackColor;
                txtName.ReadOnly = true;
            }
            else
            {
                if (fEdit)
                {
                    txtName.BackColor = clsFunction.MKBackColor;
                    txtName.ReadOnly = true;
                }
                else
                {
                    txtName.BackColor = clsFunction.EntryBackColor;
                    txtName.ReadOnly = false;
                }
            }
        }

        private void cboRentalType_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassTypeID = 0;
            if (!cboRentalType.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Type List", cboRentalType.Text);
                clsSearch.ClassTypeID = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassTypeID=" + clsSearch.ClassTypeID);

            }

            txtRentalType.Text = clsSearch.ClassTypeID.ToString();
        }

        private void cboRentalTerm_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassTypeID = 0;
            if (!cboRentalTerms.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Type List", cboRentalTerms.Text);
                clsSearch.ClassTypeID = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassTypeID=" + clsSearch.ClassTypeID);

            }

            txtRentalTerms.Text = clsSearch.ClassTypeID.ToString();
        }

        private void grpMerchant_Enter(object sender, EventArgs e)
        {

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string sFileName = "";
            string sFullPath = "";

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "BMP Files|*.bmp";
            openFile.InitialDirectory = @"C:\CASTLESTECH_MIS\IMAGE\";
            openFile.Title = "Select bmp image file";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                // selected image
                sFullPath = openFile.FileName;
                sFileName = Path.GetFileName(sFullPath);
                txtFileName.Text = sFullPath;

                picProfile.BackgroundImage = Bitmap.FromFile(sFullPath);

            }

            openFile.RestoreDirectory = true;
            openFile.Dispose();
            openFile = null;

        }

        private void btnResetImage_Click(object sender, EventArgs e)
        {
            string sFullPath = @"C:\CASTLESTECH_MIS\IMAGE\profile.bmp";

            try
            {
                picProfile.BackgroundImage = new Bitmap(sFullPath);
            }
            catch (Exception ex)
            {
                // ROCKY - PARTICULAR ISSUE: FIX ISSUE ERROR MESSAGE - ERROR MESSAGE
                /*
                dbFunction.SetMessageBox("Error: " + ex.Message, "Reset image", clsFunction.IconType.iError);
                */
                Debug.WriteLine("Error " + ex.Message);
            }
        }

        private void LoadProfileImage()
        {
            string sFileName = txtFileName.Text;
            string sFullPath = @"C:\CASTLESTECH_MIS\IMAGE\" + sFileName;

            try
            {
                picProfile.BackgroundImage = new Bitmap(sFullPath);
            }
            catch (Exception ex)
            {
                //dbFunction.SetMessageBox("Error: " + ex.Message, "Load image", clsFunction.IconType.iError);

                sFullPath = @"C:\CASTLESTECH_MIS\IMAGE\profile.bmp";
                picProfile.BackgroundImage = new Bitmap(sFullPath);
                Debug.WriteLine("Error " + ex.Message);

            }
        }

        private void DownloadImage()
        {
            Debug.WriteLine("--DownloadImage--");

            string sFile = txtFileName.Text;
            string sRemotePath = "\\download\\";
            string sLocaPath = "C:\\CASTLESTECH_MIS\\IMAGE\\";

            ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
            ftpClient.download(sRemotePath + sFile, sLocaPath + sFile);
            ftpClient.disconnect();           
        }

        private void UploadImage()
        {
            string sFile = txtFileName.Text;
            string sRemotePath = "\\download\\";
            string sLocaPath = "C:\\CASTLESTECH_MIS\\IMAGE\\";

            ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
            ftpClient.delete(sRemotePath + sFile);
            ftpClient.upload(sRemotePath + sFile, sLocaPath + sFile);
            ftpClient.disconnect(); // ftp disconnect
        }

        private void SaveBitmap()
        {
            string sFileName = txtFileName.Text = "x_" + dbFunction.padLeftChar(txtParticularID.Text, clsFunction.sZero, 6) + ".bmp";
            string sFullPath = @"C:\CASTLESTECH_MIS\IMAGE\" + sFileName;

            // delete file if exist         
            dbFile.DeleteFile(sFullPath);

            // save file
            Bitmap bmp = new Bitmap((int)picProfile.Width, (int)picProfile.Height);
            picProfile.DrawToBitmap(bmp, new Rectangle(0, 0, picProfile.Width, picProfile.Height));

            FileStream saveStream = new FileStream(sFullPath, FileMode.CreateNew);
            bmp.Save(saveStream, ImageFormat.Bmp);

            clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtParticularID.Text) + clsFunction.sPipe + dbFunction.streamToByteArray(saveStream);
            dbAPI.ExecuteAPI("PUT", "Update", "Update Particular Image", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

            bmp = null;
            saveStream = null;
            saveStream.Flush();
            saveStream.Close();

        }

        private Image FromFile(string pFileName)
        {
            var bytes = File.ReadAllBytes(pFileName);
            var ms = new MemoryStream(bytes);
            var img = Image.FromStream(ms);
            return img;
        }
        
        // ROCKY - PARTICULAR: ADD PARTICULAR DETAILS REPORT  - GENDER COMBO BOX 
        private void FillGenderComboBox(ComboBox obj)
        {
            switch (obj.Name)
            {
                case "cboGender":
                    obj.Items.Clear();
                    string[] Gender = { "[NOT SPECIFIED]", "MALE", "FEMALE" };
                    obj.Items.AddRange(Gender);
                    break;

                case "cboEducLevel":
                    obj.Items.Clear();
                    string[] Education = { "[NOT SPECIFIED]", "PHD", "MASTER", "DEGREE HOLDER", "DIPLOMA", "SECONDARY" };
                    obj.Items.AddRange(Education);
                    break;
            }

        }

        // ROCKY - PARTICULAR: ADD IMAGE CONVERTER TO BMP
        public string ConvertToBmp(string empCode, string reqName)
        {
            string DestinationPath = "C:\\CASTLESTECH_MIS\\IMAGE\\REQUIREMENTS";

            // Check Picture Box to Dispose Image
            if (picRequirements.BackgroundImage != null)
            {
                picRequirements.BackgroundImage.Dispose();
            }

            try
            {
                // Check if empCode is empty or null
                if (string.IsNullOrEmpty(empCode))
                {
                    MessageBox.Show("Employee No. is Required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }

                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All Files (*.*)|*.*";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Get the selected image path
                        string originalImagePath = openFileDialog.FileName;

                        // Remove invalid Characters
                        string empCodeText = RemoveInvalidFileNameChars(empCode);
                        string reqNameText = RemoveInvalidFileNameChars(reqName);

                        // Generate a new file name with .bmp
                        string uniqueFileName = $"{reqNameText}_{empCodeText}.bmp";

                        // Combine the destination folder and the new file name
                        string newImagePath = Path.Combine(DestinationPath, uniqueFileName);

                        // Convert the image to .bmp format and save it to the destination path
                        Image originalImage = Image.FromFile(originalImagePath);
                        originalImage.Save(newImagePath, ImageFormat.Bmp);

                        Debug.WriteLine($"New File Added: {newImagePath}");

                        // Return the new image name
                        return uniqueFileName;
                    }
                    else
                    {
                        // canceled selected
                        Debug.WriteLine("File selection was canceled.");
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"File Image Error: {ex}", "Image File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private string RemoveInvalidFileNameChars(string fileName)
        {
            // Remove any invalid characters from the file name
            char[] invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        }

        private void UploadRequirementToServer(string fileName)
        {
            if (picRequirements.BackgroundImage != null)
            {
                picRequirements.BackgroundImage.Dispose();
            }

            // Check fileName if not null or empty
            if (!string.IsNullOrEmpty(fileName) && fileName != "-" &&
                !fileName.Contains("NBI") && !fileName.Contains("SCRED") &&
                !fileName.Contains("APPFORM") && !fileName.Contains("WAIVER"))
            {
                string sFile = fileName;
                string sRemotePath = "\\download\\REQUIREMENTS\\";
                string sLocalPath = "C:\\CASTLESTECH_MIS\\IMAGE\\REQUIREMENTS\\";

                ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                ftpClient.upload(sRemotePath + sFile, sLocalPath + sFile);
                ftpClient.disconnect(); // ftp disconnect
            }
        }

        private void DownloadRequirementFromServer(string fileName)
        {
            // Check fileName if not null or empty
            if (!string.IsNullOrEmpty(fileName) && fileName != "-")
            {
                string sFile = fileName + ".BMP";
                string sRemotePath = "\\download\\REQUIREMENTS\\";
                string sLocalPath = "C:\\CASTLESTECH_MIS\\IMAGE\\REQUIREMENTS\\";
                string remoteFilePath = sRemotePath + sFile;
                string localFilePath = sLocalPath + sFile;

                ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                ftpClient.download(remoteFilePath, localFilePath);
                ftpClient.disconnect();
            }
        }

        private void PreviewRequirements(string filename)
        {
            if (!string.IsNullOrEmpty(filename) && filename != "-")
            {
                string imagePath = Path.Combine("C:\\CASTLESTECH_MIS\\IMAGE\\REQUIREMENTS\\", filename);

                if (File.Exists(imagePath))
                {
                    // Dispose of the previous image before setting the new one
                    if (picRequirements.BackgroundImage != null)
                    {
                        picRequirements.BackgroundImage.Dispose();
                    }

                    picRequirements.BackgroundImage = Image.FromFile(imagePath);
                }
                else
                {
                    // If the file does not exist, display an error message or handle it accordingly
                    MessageBox.Show("Image file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnNbiUpload_Click(object sender, EventArgs e)
        {
            txtNbiClearance.Text = ConvertToBmp(txtCode.Text, "NBI");
            PreviewRequirements(txtNbiClearance.Text);
        }

        private void btnUploadSchoolReq_Click(object sender, EventArgs e)
        {
            txtSchoolCred.Text = ConvertToBmp(txtCode.Text, "SCRED");
            PreviewRequirements(txtSchoolCred.Text);
        }

        private void btnUploadAppForm_Click(object sender, EventArgs e)
        {
            txtApplicationForm.Text = ConvertToBmp(txtCode.Text, "APPFORM");
            PreviewRequirements(txtApplicationForm.Text);
        }

        private void btnUploadWaiver_Click(object sender, EventArgs e)
        {
            txtWaiverForm.Text = ConvertToBmp(txtCode.Text, "WAIVER");
            PreviewRequirements(txtWaiverForm.Text);
        }

        private void btnPrevNbi_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNbiClearance.Text) && txtNbiClearance.Text != "-" || !txtNbiClearance.Text.Contains("NBI_"))
            {
                PreviewRequirements(txtNbiClearance.Text);
            }
            else
            {
                PreviewRequirements($"NBI_{txtCode.Text}");
            }

        }

        private void btnPrevSchoolReq_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSchoolCred.Text) && txtNbiClearance.Text != "-" || !txtNbiClearance.Text.Contains("SCRED_"))
            {
                PreviewRequirements(txtSchoolCred.Text);
            }
            else
            {
                PreviewRequirements($"SCRED_{txtCode.Text}");
            }
        }

        private void btnPrevAppForm_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtApplicationForm.Text) && txtNbiClearance.Text != "-" || !txtNbiClearance.Text.Contains("APPFORM_"))
            {
                PreviewRequirements(txtApplicationForm.Text);
            }
            else
            {
                PreviewRequirements($"APPFORM_{txtCode.Text}");
            }
        }

        private void btnPrevWaiver_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtApplicationForm.Text) && txtNbiClearance.Text != "-" || !txtNbiClearance.Text.Contains("WAIVER_"))
            {
                PreviewRequirements(txtWaiverForm.Text);
            }
            else
            {
                PreviewRequirements($"WAIVER_{txtCode.Text}");
            }

        }
    }
}
