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
    public partial class frmServiceCall : Form
    {
        public static string sHeader;
        private clsAPI dbAPI;
        private clsINI dbSetting;
        private clsFile dbFile;
        private clsFunction dbFunction;        
        private bool fEdit = false;
        int iLimit = 255;

        public frmServiceCall()
        {
            InitializeComponent();
        }
        
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmCustomerService_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbSetting = new clsINI();
            dbFile = new clsFile();
            dbFunction = new clsFunction();
            dbSetting.InitDatabaseSetting();

            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);
            
            dbFunction.ClearListView(lvwList);
            
            btnSave.Enabled = false;

            fEdit = false;
            InitButton();
            InitDate();
            lblHeader.Text = lblHeader.Text + " " + "[ " + sHeader + " ]";
            InitMessageCountLimit();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {                       
            btnAdd.Enabled = false;
            btnSave.Enabled = true;
            
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

        private void btnClear_Click(object sender, EventArgs e)
        {            
            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);

            dbFunction.ClearListView(lvwList);
            dbFunction.ClearListView(lvwDetail);

            fEdit = false;
            InitButton();
            InitMessageCountLimit();
        }

        private void PopulateMerchantTextBox()
        {
            int i = 0;
            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassParticularID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassProvinceID + clsFunction.sPipe +
                                                clsSearch.ClassCityID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassParticularTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassParticularTypeDescription + clsFunction.sPipe +
                                                clsSearch.ClassParticularName.ToString();

            Debug.WriteLine("PopulateMerchantTextBox::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbAPI.ExecuteAPI("GET", "View", "Advance Particular", clsSearch.ClassAdvanceSearchValue, "Particular", "", "ViewAdvanceParticular");

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.ParticularID.Length > i)
                {
                    txtMerchantID.Text = clsArray.ParticularID[i].ToString();
                    txtMerchantName.Text = clsArray.ParticularName[i];
                    txtMerchantAddress.Text = clsArray.Address[i];
                    txtMerchantProvince.Text = clsArray.Province[i];
                    txtMerchantCity.Text = clsArray.City[i];
                    txtMerchantContactPerson.Text = clsArray.ContactPerson[i];
                    txtMerchantTelNo.Text = clsArray.TelNo[i];
                    txtMerchantMobile.Text = clsArray.MobileNo[i];

                    i++;

                }
            }
        }
        private void FillClientTextBox()
        {
            int i = 0;

            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassParticularID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassProvinceID + clsFunction.sPipe +
                                                clsSearch.ClassCityID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassParticularTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassParticularTypeDescription + clsFunction.sPipe +
                                                clsSearch.ClassParticularName.ToString();

            Debug.WriteLine("PopulateClientTextBox::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbAPI.ExecuteAPI("GET", "View", "Advance Particular", clsSearch.ClassAdvanceSearchValue, "Particular", "", "ViewAdvanceParticular");

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.ParticularID.Length > i)
                {
                    txtClientID.Text = clsArray.ParticularID[i].ToString();
                    txtClientName.Text = clsArray.ParticularName[i];
                    txtClientAddress.Text = clsArray.Address[i];

                    i++;

                }
            }
        }
        private void PopulateTerminalTextBox()
        {
            int i = 0;

            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassTerminalID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalModelID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalBrandID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalSN + clsFunction.sPipe +
                                                clsSearch.ClassTerminalStatus.ToString();

            Debug.WriteLine("PopulateTerminalTextBox::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbAPI.ExecuteAPI("GET", "View", "Advance Terminal", clsSearch.ClassAdvanceSearchValue, "Terminal", "", "ViewAdvanceTerminal");

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.TerminalID.Length > i)
                {
                    txtTerminalID.Text = clsArray.TerminalID[i];
                    txtTerminalTypeID.Text = clsArray.TerminalTypeID[i];
                    txtTerminalModelID.Text = clsArray.TerminalModelID[i];
                    txtTerminalBrandID.Text = clsArray.TerminalBrandID[i];
                    txtTerminalSN.Text = clsArray.TerminalSN[i];
                    txtTerminalType.Text = clsArray.TerminalType[i];
                    txtTerminalModel.Text = clsArray.TerminalModel[i];
                    txtTerminalBrand.Text = clsArray.TerminalBrand[i];
                    i++;
                }

            }
        }
        private void PopulateSIMTextBox()
        {
            txtSIMID.Text = clsSearch.ClassSIMID.ToString();
            txtSIMSerialNo.Text = clsSearch.ClassSIMSerialNo;
            txtSIMCarrier.Text = clsSearch.ClassSIMCarrier;
        }
        private void PKTextBoxBackColor(bool isLock)
        {
            if (isLock)
            {
                txtTAIDNo.BackColor = Color.PaleGreen;
                txtIRNo.BackColor = Color.LightCyan;
                txtMerchantName.BackColor = Color.LightCyan;
                txtTerminalSN.BackColor = Color.LightCyan;
                txtIRTID.BackColor = Color.LightCyan;
                txtIRMID.BackColor = Color.LightCyan;
                txtSIMSerialNo.BackColor = Color.LightCyan;
                txtClientName.BackColor = Color.LightCyan;
                txtCurrentStatus.BackColor = Color.LightCyan;
            }
        }

        private void InitCallRequestDateTime()
        {
            DateTime ProcessDateTime = DateTime.Now;
            string sProcessDate = "";
            string sProcessTime = "";

            sProcessDate = ProcessDateTime.ToString("MM-dd-yyyy");
            sProcessTime = ProcessDateTime.ToString("hh:mm:ss tt");

            // Call Date/Time
            txtCallDate.Text = sProcessDate;
            txtCallTime.Text = sProcessTime;

            // Request Date/TIme
            dteReqDate.Value = DateTime.Now;            
            dbFunction.SetDateCustomFormat(dteReqDate);                       
            dteReqTime.Value = DateTime.Parse(sProcessTime);
        }
        
        private void PKTextBoxReadOnly(bool fReadOnly)
        {
            txtReferralID.ReadOnly = fReadOnly;
            txtCustomerName.ReadOnly = fReadOnly;
            txtCustomerContactNo.ReadOnly = fReadOnly;
            txtProblemReported.ReadOnly = fReadOnly;
            txtArrangementMade.ReadOnly = fReadOnly;
            txtTrackinNo.ReadOnly = fReadOnly;

            if (!fReadOnly)
            {
                txtReferralID.Enabled = true;
                txtCustomerName.Enabled = true;
                txtCustomerContactNo.Enabled = true;
                txtProblemReported.Enabled = true;
                txtArrangementMade.Enabled = true;
                txtTrackinNo.Enabled = true;

                txtReferralID.BackColor = Color.White;
                txtCustomerName.BackColor = Color.White;
                txtCustomerContactNo.BackColor = Color.White;
                txtProblemReported.BackColor = Color.White;
                txtArrangementMade.BackColor = Color.White;
                txtTrackinNo.BackColor = Color.White;
            }
        }

        private void frmCustomerService_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateFields()) return;

            if (!dbFunction.fSavingConfirm(false)) return;

            SaveSCDetail();

            LoadTAWithSC();

        }

        private bool ValidateFields()
        {
            bool fValid = false;
            string sReqTime = dbFunction.GetDateFromParse(dteReqTime.Text, "h:mm:ss tt", "HH:mm:ss");

            if (txtClientID.Text.Length > 0 && txtMerchantID.Text.Length > 0 && txtIRNo.Text.Length > 0 && txtIRTID.Text.Length > 0 && txtIRMID.Text.Length > 0 &&
                txtCounter.Text.Length > 0 && txtReferralID.Text.Length > 0 &&
                dbFunction.isValidComboBoxValue(cboSCStatus.Text) &&
                sReqTime.CompareTo(clsFunction.sInvalidTime) != 0)
                fValid = true;

            if (!fValid)
            {
                MessageBox.Show("Check the following field(s) listed below:\n\n" +
                                "*Referral ID\n" +
                                "*Call Received By\n" +
                                "*Contact No.\n" +
                                "*Problem Reported\n" +
                                "*Remarks(Arrange During Call)\n" +
                                "*Request Time\n" +
                                "*Status\n" +
                                "\n" +
                                "Field(s) with asterisk(*) must not be blank.", "Incomplete Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return fValid;
        }
        private void SaveSCDetail()
        {
            string sSQL = "";
            string sRowSQL = "";            
            DateTime SCDateTime = DateTime.Now;
            string sSCDateTime = "";            
            string sSCReqDate = dbFunction.GetDateFromParse(dteReqDate.Text, "MM-dd-yyyy H:mm:ss tt", "yyyy-MM-dd");
            string sSCReqTime = dbFunction.GetDateFromParse(dteReqTime.Text, "h:mm:ss tt", "HH:mm:ss");
            string sSCShipDate = dbFunction.GetDateFromParse(dteShipDate.Text, "MM-dd-yyyy H:mm:ss tt", "yyyy-MM-dd");
            string sSCShipTime = dbFunction.GetDateFromParse(dteShipTime.Text, "h:mm:ss tt", "HH:mm:ss");

            sSCDateTime = SCDateTime.ToString("yyyy-MM-dd H:mm:ss");
            

            sSQL = "";
            sRowSQL = "";
            sRowSQL = "('" + (txtTAIDNo.Text.Length > 0 ? txtTAIDNo.Text : "0") + "'," +
            sRowSQL + sRowSQL + "'" + (txtClientID.Text.Length > 0 ? txtClientID.Text : "0") + "'," +
            sRowSQL + sRowSQL + "'" + (txtMerchantID.Text.Length > 0 ? txtMerchantID.Text : "0") + "'," +
            sRowSQL + sRowSQL + "'" + txtClientName.Text + "'," +
            sRowSQL + sRowSQL + "'" + txtMerchantName.Text + "'," +
            sRowSQL + sRowSQL + "'" + (txtIRIDNo.Text.Length > 0 ? txtIRIDNo.Text : "0") + "'," +
            sRowSQL + sRowSQL + "'" + txtIRNo.Text + "'," +
            sRowSQL + sRowSQL + "'" + (txtTerminalID.Text.Length > 0 ? txtTerminalID.Text : "0") + "'," +
            sRowSQL + sRowSQL + "'" + txtTerminalSN.Text + "'," +
            sRowSQL + sRowSQL + "'" + (txtSIMID.Text.Length > 0 ? txtSIMID.Text : "0") + "'," +
            sRowSQL + sRowSQL + "'" + txtSIMSerialNo.Text + "'," +
            sRowSQL + sRowSQL + "'" + (txtCounter.Text.Length > 0 ? txtCounter.Text : "0") + "'," +
            sRowSQL + sRowSQL + "'" + (txtReferralID.Text.Length > 0 ? txtReferralID.Text : "0") + "'," +
            sRowSQL + sRowSQL + "'" + sSCDateTime + "'," +
            sRowSQL + sRowSQL + "'" + txtCallDate.Text + "'," +
            sRowSQL + sRowSQL + "'" + txtCallTime.Text + "'," +
            sRowSQL + sRowSQL + "'" + sSCReqDate + "'," +
            sRowSQL + sRowSQL + "'" + sSCReqTime + "'," +
            sRowSQL + sRowSQL + "'" + sSCShipDate + "'," +
            sRowSQL + sRowSQL + "'" + sSCShipTime + "'," +
            sRowSQL + sRowSQL + "'" + txtCustomerName.Text + "'," +
            sRowSQL + sRowSQL + "'" + txtCustomerContactNo.Text + "'," +
            sRowSQL + sRowSQL + "'" + txtProblemReported.Text + "'," +
            sRowSQL + sRowSQL + "'" + txtArrangementMade.Text + "'," +
            sRowSQL + sRowSQL + "'" + txtTrackinNo.Text + "'," +
            sRowSQL + sRowSQL + "'" + cboSCStatus.Text + "') ";
            sSQL = sSQL + sRowSQL;

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "Service Call Detail", sSQL, "InsertCollectionDetail");

            MessageBox.Show("New Service Call has been successfully saved.", "Saved",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1);
        }
        
        private void LoadServiceCall()
        {
            int i = 0;
            int iLineNo = 0;

            lvwList.Items.Clear();
            txtCustomerName.Text = clsFunction.sNull;
            txtCustomerContactNo.Text = clsFunction.sNull;

            clsSearch.ClassAdvanceSearchValue = txtTAIDNo.Text + clsFunction.sPipe;
            dbAPI.ExecuteAPI("GET", "View", "Service Call Detail", clsSearch.ClassAdvanceSearchValue, "Service Call", "", "ViewServiceCall");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.SCNo.Length > i)
                {
                    iLineNo++;

                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.SCNo[i].ToString());
                    item.SubItems.Add(clsArray.SCDateTime[i]);
                    item.SubItems.Add(clsArray.ReferralID[i]);
                    item.SubItems.Add(clsArray.CustomerName[i]);
                    item.SubItems.Add(clsArray.CustomerContactNo[i]);
                    item.SubItems.Add(clsArray.ReportedProblem[i]);
                    item.SubItems.Add(clsArray.ArrangementMade[i]);
                    item.SubItems.Add(clsArray.SCReqDate[i]);
                    item.SubItems.Add(clsArray.SCReqTime[i]);
                    item.SubItems.Add(clsArray.SCShipDate[i]);
                    item.SubItems.Add(clsArray.SCShipTime[i]);
                    item.SubItems.Add(clsArray.TrackingNo[i]);
                    item.SubItems.Add(clsArray.SCStatus[i]);
                    lvwList.Items.Add(item);

                    i++;

                }

                dbFunction.ListViewAlternateBackColor(lvwList);                
            }
        }

        private void ListDetails(int iRow)
        {
            int iLineNo = 0;
            int iColCount = lvwList.Columns.Count;
            

            lvwDetail.Items.Clear();
            for (int i = 0; i < iColCount; i++)
            {
                iLineNo++;
                string cellParam = lvwList.Columns[i].Text; // Param
                string cellValue = lvwList.SelectedItems[0].SubItems[i].Text; // Value

                switch (i)
                {
                    case 0:
                    case 1:                    
                        break;
                    default:
                        ListViewItem item = new ListViewItem(iLineNo.ToString());
                        item.SubItems.Add(cellParam);
                        item.SubItems.Add(cellValue);
                        lvwDetail.Items.Add(item);
                        break;
                }

            }
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.Items.Count > 0)
            {
                if (clsSearch.ClassSCNo > 0)
                {
                    ListDetails(int.Parse(txtLineNo.Text));
                    dbFunction.ListViewAlternateBackColor(lvwDetail);
                }
            }
        }

        private void lvwList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwList.SelectedItems.Count > 0)
            {
                string LineNo = lvwList.SelectedItems[0].Text;
                txtLineNo.Text = LineNo;

                if (LineNo.Length > 0)
                {
                    clsSearch.ClassSCNo = int.Parse(lvwList.SelectedItems[0].SubItems[1].Text);
                    lvwList_DoubleClick(this, e);
                }
            }
        }

        private void LoadTAWithSC()
        {
            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);

            InitCallRequestDateTime();
            
            // Load IR
            txtIRIDNo.Text = clsSearch.ClassIRIDNo.ToString();
            txtIRNo.Text = clsSearch.ClassIRNo;
            txtIRRequestDate.Text = clsSearch.ClassIRDate;
            txtIRInstallationDate.Text = clsSearch.ClassInstallationDate;
            txtIRProcessedBy.Text = clsSearch.ClassTAProcessedBy;
            txtIRTID.Text = clsSearch.ClassTID;
            txtIRMID.Text = clsSearch.ClassMID;

            // Load TA
            txtServiceProviderID.Text = clsSearch.ClassServiceProviderID.ToString();
            txtTAIDNo.Text = clsSearch.ClassTAIDNo.ToString();
            txtTAServiceTypeStatus.Text = clsSearch.ClassServiceTypeStatus.ToString();
            txtCurrentStatus.Text = clsSearch.ClassServiceTypeStatusDescription;

            // Load Region                
            txtRegionID.Text = clsSearch.ClassRegionID.ToString();
            txtMerchantRegion.Text = clsSearch.ClassRegion;

            // Load Mechant
            txtMerchantID.Text = clsSearch.ClassMerchantID.ToString();
            clsSearch.ClassParticularID = clsSearch.ClassMerchantID;
            clsSearch.ClassProvinceID = 0;
            clsSearch.ClassCityID = 0;
            clsSearch.ClassParticularTypeID = clsGlobalVariables.iMerchant_Type;
            clsSearch.ClassParticularTypeDescription = clsGlobalVariables.sMerchant_Type;
            clsSearch.ClassParticularName = "0";
            PopulateMerchantTextBox();

            // Load Client
            txtClientID.Text = clsSearch.ClassClientID.ToString();
            clsSearch.ClassParticularID = clsSearch.ClassClientID;
            clsSearch.ClassProvinceID = 0;
            clsSearch.ClassCityID = 0;
            clsSearch.ClassParticularTypeID = clsGlobalVariables.iClient_Type;
            clsSearch.ClassParticularTypeDescription = clsGlobalVariables.sClient_Type;
            clsSearch.ClassParticularName = "0";
            FillClientTextBox();

            // Load Terminal
            clsSearch.ClassTerminalID = 0;
            clsSearch.ClassTerminalTypeID = 0;
            clsSearch.ClassTerminalModelID = 0;
            clsSearch.ClassTerminalBrandID = 0;
            clsSearch.ClassTerminalSN = clsSearch.ClassTerminalSN;
            clsSearch.ClassTerminalStatus = 0;
            PopulateTerminalTextBox();

            // Load SIM
            PopulateSIMTextBox();

            clsSearch.ClassAdvanceSearchValue = txtTAIDNo.Text + clsFunction.sPipe;
            dbAPI.GetViewCount("Search", "Service Call Count", clsSearch.ClassAdvanceSearchValue, "Get Count");

            int iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            txtCounter.Text = dbFunction.padLeftChar(iCount.ToString(), clsFunction.sZero, 3);

            // Load Service Call
            if (txtTAIDNo.Text.Length > 0)
            {
                lvwList.Items.Clear();
                lvwDetail.Items.Clear();

                LoadServiceCall();
            }
                

            dbFunction.ClearComboBox(this);
            dbAPI.FillComboBoxSCStatus(cboSCStatus);
            DefaultSelectedComboBoxValue();
            dbFunction.ComBoBoxUnLock(true, this);

            PKTextBoxBackColor(true);
            PKTextBoxReadOnly(false);

            fEdit = true;
            InitButton();            
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadTAWithSC();
        }

        private void InitDate()
        {
            dteReqDate.Value = DateTime.Now;
            dteShipDate.Value = DateTime.Now;
            dbFunction.SetDateCustomFormat(dteReqDate);
            dbFunction.SetDateCustomFormat(dteShipDate);

            dteReqTime.Value = DateTime.Now.Date;
            dteShipTime.Value = DateTime.Now.Date;
            dbFunction.SetTimeCustomFormat(dteReqTime);
            dbFunction.SetTimeCustomFormat(dteShipTime);

        }

        private void btnClearField_Click(object sender, EventArgs e)
        {
            txtReferralID.Text = "";
            txtCustomerName.Text = "";
            txtCustomerContactNo.Text = "";
            txtProblemReported.Text = "";
            txtArrangementMade.Text = "";

            dbFunction.ClearComboBox(this);
            dbAPI.FillComboBoxSCStatus(cboSCStatus);
            dbFunction.ComBoBoxUnLock(true, this);
            InitMessageCountLimit();

        }

        private void btnGenerateID_Click(object sender, EventArgs e)
        {
            int iControlNo = 0;

            iControlNo = dbAPI.GetControlID("SC Detail");
            txtReferralID.Text = iControlNo.ToString();
            txtReferralID.Text = dbFunction.GenerateControlNo(iControlNo, "SC", true);
        }
        private void DefaultSelectedComboBoxValue()
        {
            cboSCStatus.SelectedIndex = 0;            
        }

        private void bunifuCards13_Paint(object sender, PaintEventArgs e)
        {

        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }
        private void InitMessageCountLimit()
        {
            lblCountProblemReported.Text = txtProblemReported.TextLength.ToString() + "/" + iLimit.ToString();
            lblCountRemarks.Text = txtArrangementMade.TextLength.ToString() + "/" + iLimit.ToString();

            txtProblemReported.MaxLength = iLimit;
            txtArrangementMade.MaxLength = iLimit;
        }

        private void txtProblemReported_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtArrangementMade_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtProblemReported_TextChanged(object sender, EventArgs e)
        {
            lblCountProblemReported.Text = txtProblemReported.Text.Length.ToString() + "/" + iLimit.ToString();            
        }

        private void txtArrangementMade_TextChanged(object sender, EventArgs e)
        {
            lblCountRemarks.Text = txtArrangementMade.Text.Length.ToString() + "/" + iLimit.ToString();
        }

        private void txtCustomerContactNo_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtCustomerName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtTrackinNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }
    }
}
