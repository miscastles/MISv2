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
    public partial class frmMRequestID : Form
    {
        private clsAPI dbAPI;
        private clsINI dbSetting;
        private clsFile dbFile;
        private clsFunction dbFunction;

        public frmMRequestID()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (dbFunction.isValidID(txtIRIDNo.Text))
            {
                dbAPI.GenerateID(false, txtNewIRNo, txtIRIDNo, "IR Detail", clsDefines.CONTROLID_PREFIX_IR);

                if (chkUse.Checked)
                {
                    DateTime pRequestDate = DateTime.Parse(txtRequestDate.Text);
                    string pTemp = dbFunction.padLeftChar(pRequestDate.Year.ToString().Substring(2, 2), clsDefines.gZero.ToString(), 2)   + 
                                   dbFunction.padLeftChar(pRequestDate.Month.ToString(), clsDefines.gZero.ToString(), 2)   +
                                    dbFunction.padLeftChar(pRequestDate.Day.ToString(), clsDefines.gZero.ToString(), 2);
                    txtNewIRNo.Text = clsDefines.CONTROLID_PREFIX_IR + pTemp + clsDefines.gDash + dbFunction.padLeftChar(txtIRIDNo.Text, clsDefines.gZero.ToString(), 6);
                }
            }
            else
            {
                dbFunction.SetMessageBox("Unable to create new Request ID", "Request ID", clsFunction.IconType.iError);
            }
        }

        private void frmMRequestID_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbSetting = new clsINI();
            dbFile = new clsFile();
            dbFunction = new clsFunction();

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(true, this);

            btnSave.Enabled = false;
            btnGenerate.Enabled = false;

            SetMKTextBoxBackColor();
            txtNewIRNo.ReadOnly = true;
            chkUse.Checked = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iMerchant;
            frmSearchField.sHeader = "MERCHANT";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {

                Cursor.Current = Cursors.WaitCursor;

                dbFunction.TextBoxUnLock(true, this);
              
                txtMerchantName.Text = clsSearch.ClassParticularName;             
                txtMerchantID.Text = clsSearch.ClassParticularID.ToString();        
                txtIRTID.Text = clsSearch.ClassTID;
                txtIRMID.Text = clsSearch.ClassMID;

                txtIRIDNo.Text = clsSearch.ClassIRIDNo.ToString();
                txtSearchIRNo.Text = txtIRNo.Text = clsSearch.ClassIRNo;

                FillMerchantTextBox();

                btnSave.Enabled = true;
                btnGenerate.Enabled = true;

                txtNewIRNo.Text = "";
                chkUse.Checked = true;

                Cursor.Current = Cursors.Default;
            }
        }

        private void FillMerchantTextBox()
        {
            Debug.WriteLine("--FillMerchantTextBox--");         
            Debug.WriteLine("txtMerchantID.Text=" + txtMerchantID.Text);
            Debug.WriteLine("txtIRIDNo.Text=" + txtIRIDNo.Text);
           
            
            txtMerchantName.Text =
            txtMerchantAddress.Text =            
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
            txtAppCRC.Text = clsFunction.sNull;


            if (dbFunction.isValidID(txtMerchantID.Text))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Merchant Info", txtMerchantID.Text + clsFunction.sPipe + txtIRIDNo.Text, "Get Info Detail", "", "GetInfoDetail");

                dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                if (dbAPI.isNoRecordFound() == false)
                {
                    txtMerchantID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                    txtMerchantName.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtMerchantAddress.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                    
                    txtMerchantRegion.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                    txtMerchantCity.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);

                    txtMerchantContactPerson.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                    txtMerchantTelNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                    txtMerchantMobileNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                    txtMerchantEmail.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                    txtIRTID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                    txtIRMID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 11);
                    txtMerchantPrimaryNum.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 12);
                    txtMerchantSecondaryNum.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);
                    txtAppVersion.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 14);
                    txtAppCRC.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 15);

                    txtRequestDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 18);
                    txtInstallationDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 19);
                    txtRequestFor.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 26);
                    txtSpecialInstruction.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 21);
                    txtSetup.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 25);

                }
            }

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(true, this);

            btnSave.Enabled = false;
            btnGenerate.Enabled = false;

            SetMKTextBoxBackColor();
            txtNewIRNo.ReadOnly = true;
            chkUse.Checked = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRequestIDFrom, txtSearchIRNo.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRequestIDTo, txtNewIRNo.Text)) return;

            if (!isValidRequetID()) return;
            
            if (!fConfirmDetails()) return;

            clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + clsFunction.sPipe + dbFunction.CheckAndSetStringValue(txtNewIRNo.Text);

            dbAPI.ExecuteAPI("PUT", "Update", "Update RequestID", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

            dbFunction.SetMessageBox("Request ID has been successfully updated.", "Update", clsFunction.IconType.iInformation);

            btnClear_Click(this, e);
        }

        bool fConfirmDetails()
        {
            bool fConfirm = true;
            string sTemp = "";

            sTemp = ("Are you sure to update the request ID:\n\n") +                                       
                           "From: " + txtSearchIRNo.Text + "\n" +
                           "To: " + txtNewIRNo.Text + "\n\n" +
                           "Warning: All request ID will be update to all usage.";

            if (MessageBox.Show(sTemp, "Confirm Update?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fConfirm = false;
            }

            return fConfirm;
        }

        bool isValidRequetID()
        {
            bool isValid = false;

            clsSearch.ClassSearchValue = dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + clsFunction.sPipe + dbFunction.CheckAndSetStringValue(txtNewIRNo.Text);

            dbAPI.ExecuteAPI("GET", "Search", "RequestID", clsSearch.ClassSearchValue, "CheckRecordExist", "", "CheckRecordExist");
            if (clsGlobalVariables.sAPIResponseCode.CompareTo(clsGlobalVariables.NO_RECORD_FOUND) == 0)
                isValid = true;
            
            if (!isValid)
            {
                dbFunction.SetMessageBox("Request ID " + txtNewIRNo.Text + " is already taken.", "Request ID exist", clsFunction.IconType.iError);
            }

            return isValid;
        }

        private void frmMRequestID_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void SetMKTextBoxBackColor()
        {
            txtSearchIRNo.BackColor = txtMerchantName.BackColor = clsFunction.MKBackColor;
            txtNewIRNo.BackColor = clsFunction.SearchBackColor;
        }
    }
}
