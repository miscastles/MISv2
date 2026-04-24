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
    public partial class frmFindMECriteria : Form
    {
        public static string sServiceTypeDetails = "";
        public static string sMerchantDetails = "";
        public static string sParticularDetails = "";
        public static string sRequestDetails = "";
        public static string sTerminalDetails = "";
        public static string sDateDetails = "";
        public static string sSearchOptionDetails;
        public static string sHeader = "";

        public static bool fSelected = false;
        private clsAPI dbAPI;
        private clsFunction dbFunction;        

        public frmFindMECriteria()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            fSelected = false;
            this.Close();
        }

        private void frmFindMECriteria_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);

            //dbFunction.ClearComboBox(this);
            //dbFunction.ComBoBoxUnLock(false, this);
            PKTextBoxBackColor(true);
            EntryTextBoxBackColor(true);
            InitServiceDate();

            //dbFunction.ComBoBoxUnLock(true, this);
            //dbAPI.FillComboBoxJobTypeDescription(cboSearchJobTypeDescription);
            //dbAPI.FillComboBoxJobTypeStatusDescription(cboSearchJobTypeStatusDescription);
            FillComboBoxJobTypeDescription();
            FillComboBoxJobTypeStatusDescription();

            txtSearchMerchant.Focus();

            chkServiceDate.Checked = false;
            CheckCheckBox(0);

        }

        private void btnSearchClear_Click(object sender, EventArgs e)
        {
            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);

            //dbFunction.ClearComboBox(this);
            //dbFunction.ComBoBoxUnLock(false, this);

            PKTextBoxBackColor(true);
            EntryTextBoxBackColor(true);
            InitServiceDate();

            //dbFunction.ComBoBoxUnLock(true, this);
            //dbAPI.FillComboBoxJobTypeDescription(cboSearchJobTypeDescription);

            FillComboBoxJobTypeDescription();
            FillComboBoxJobTypeStatusDescription();

            chkServiceDate.Checked = false;
            CheckCheckBox(0);

            dbAPI.ResetAdvanceSearch();
        }

        private void InitServiceDate()
        {
            dteServiceDateSearchFrom.Value = DateTime.Now;
            dbFunction.SetDateCustomFormat(dteServiceDateSearchFrom);

            dteServiceDateSearchTo.Value = DateTime.Now;
            dbFunction.SetDateCustomFormat(dteServiceDateSearchTo);
        }

        private void btnSearchMerchant_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iMerchant;
            frmSearchField.sHeader = "MERCHANT";
            frmSearchField.sSearchChar = txtSearchMerchant.Text;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {     
                clsSearch.ClassMerchantID = clsSearch.ClassParticularID;
                txtSearchMerchant.Text = clsSearch.ClassParticularName;
            }
        }

        private void btnSearchClient_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iClient;
            frmSearchField.sHeader = "CLIENT";
            frmSearchField.sSearchChar = txtSearchClient.Text;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsSearch.ClassClientID = clsSearch.ClassParticularID;                
                txtSearchClient.Text = clsSearch.ClassParticularName;
            }
        }

        private void btnSearchSP_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iSP;
            frmSearchField.sHeader = "SERVICE PROVIDER";
            frmSearchField.sSearchChar = txtSearchSP.Text;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsSearch.ClassServiceProviderID = clsSearch.ClassParticularID;
                txtSearchSP.Text = clsSearch.ClassParticularName;
            }
        }

        private void btnSearchFE_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iFE;
            frmSearchField.sHeader = "FIELD ENGINEER";
            frmSearchField.sSearchChar = txtSearchFE.Text;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsSearch.ClassFEID = clsSearch.ClassParticularID;
                txtSearchFE.Text = clsSearch.ClassParticularName;
            }
        }

        private void btnSearchIRNo_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iIR;
            frmSearchField.sHeader = "REQUEST ID";
            frmSearchField.sSearchChar = txtSearchIRNo.Text;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsSearch.ClassIRIDNo = clsSearch.ClassIRIDNo;
                txtSearchIRNo.Text = clsSearch.ClassIRNo;
            }
        }

        private void btnSearchTerminalSN_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iTerminal;
            frmSearchField.sHeader = "TERMINAL SERIAL NO.";
            frmSearchField.sSearchChar = txtSearchTerminalSN.Text;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsSearch.ClassTerminalID = clsSearch.ClassTerminalID;
                txtSearchTerminalSN.Text = clsSearch.ClassTerminalSN;
            }
        }

        private void btnSearchSIMSN_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iSIM;
            frmSearchField.sHeader = "SIM SERIAL NO.";
            frmSearchField.sSearchChar = txtSearchSIMSN.Text;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsSearch.ClassSIMID = clsSearch.ClassSIMID;
                txtSearchSIMSN.Text = clsSearch.ClassSIMSerialNo;
            }
        }

        private void btnSearchDockSN_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iTerminal;
            frmSearchField.sHeader = "DOCK SERIAL NO.";
            frmSearchField.sSearchChar = txtSearchDockSN.Text;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsSearch.ClassDockID = clsSearch.ClassTerminalID;
                txtSearchDockSN.Text = clsSearch.ClassTerminalSN;
            }
        }

        private void PKTextBoxBackColor(bool isLock)
        {
            if (isLock)
            {
                txtSearchRegion.BackColor = clsFunction.PKBackColor;
                txtSearchCity.BackColor = clsFunction.PKBackColor;
                txtSearchMerchant.BackColor = clsFunction.PKBackColor;                
                txtSearchClient.BackColor = clsFunction.PKBackColor;
                txtSearchSP.BackColor = clsFunction.PKBackColor;
                txtSearchFE.BackColor = clsFunction.PKBackColor;
                txtSearchIRNo.BackColor = clsFunction.PKBackColor;
                txtSearchTerminalSN.BackColor = clsFunction.PKBackColor;
                txtSearchSIMSN.BackColor = clsFunction.PKBackColor;
                txtSearchDockSN.BackColor = clsFunction.PKBackColor;
            }
        }

        private void EntryTextBoxBackColor(bool isLock)
        {
            if (isLock)
            {
                txtSearchTID.BackColor = clsFunction.EntryBackColor;
                txtSearchTID.ReadOnly = false;
                txtSearchTID.Enabled = true;

                txtSearchMID.BackColor = clsFunction.EntryBackColor;
                txtSearchMID.ReadOnly = false;
                txtSearchMID.Enabled = true;
            }
        }

        private void frmFindMECriteria_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    fSelected = false;
                    this.Close();
                    break;
            }
        }

        private void btnSearchNow_Click(object sender, EventArgs e)
        {
            string sServiceDateFrom = "";
            string sServiceDateTo = "";            
            
            DateTime stServiceDateFrom = dteServiceDateSearchFrom.Value;
            DateTime stServiceDateTo = dteServiceDateSearchTo.Value;

            clsSearch.ClassTID = (txtSearchTID.TextLength > 0 ? txtSearchTID.Text : clsFunction.sZero);
            clsSearch.ClassMID = (txtSearchMID.TextLength > 0 ? txtSearchMID.Text : clsFunction.sZero);
            clsSearch.ClassIRNo = (txtSearchIRNo.Text.Length > 0 ? txtSearchIRNo.Text : clsFunction.sZero);
            clsSearch.ClassCurrentPage = clsFunction.iZero;

            //clsSearch.ClassJobTypeDescription = cboSearchJobTypeDescription.Text;
            //clsSearch.ClassJobTypeStatusDescription = cboSearchJobTypeStatusDescription.Text;

            // Date            
            sServiceDateFrom = sServiceDateTo = clsFunction.sDateFormat;

            if (chkServiceDate.CheckState == CheckState.Checked)
            {
                sServiceDateFrom = stServiceDateFrom.ToString("MM-dd-yyyy");
                sServiceDateTo = stServiceDateTo.ToString("MM-dd-yyyy");

                clsSearch.ClassServiceDateFrom = sServiceDateFrom;
                clsSearch.ClassServiceDateTo = sServiceDateTo;
                
                if (!CheckDateFromTo(dteServiceDateSearchFrom, dteServiceDateSearchTo, 0)) return;
                
            }

            clsSearch.ClassJobTypeDescriptionList  = GetListOfJobTypeDescription();
            clsSearch.ClassJobTypeStatusDescriptionList = GetListOfJobTypeStatusDescription();
            clsSearch.ClassJobTypeList = GetJobTypeList();

            Debug.WriteLine("clsSearch.ClassMerchantID=" + clsSearch.ClassMerchantID.ToString());
            Debug.WriteLine("clsSearch.ClassTID=" + clsSearch.ClassTID);
            Debug.WriteLine("clsSearch.ClassMID=" + clsSearch.ClassMID);
            Debug.WriteLine("clsSearch.ClassClientID=" + clsSearch.ClassClientID.ToString());
            Debug.WriteLine("clsSearch.ClassServiceProviderID=" + clsSearch.ClassServiceProviderID.ToString());
            Debug.WriteLine("clsSearch.ClassFEID=" + clsSearch.ClassFEID.ToString());
            Debug.WriteLine("clsSearch.ClassIRNo=" + clsSearch.ClassIRNo);
            Debug.WriteLine("clsSearch.ClassTerminalID=" + clsSearch.ClassTerminalID.ToString());
            Debug.WriteLine("clsSearch.ClassSIMID=" + clsSearch.ClassSIMID.ToString());
            Debug.WriteLine("clsSearch.ClassDockID=" + clsSearch.ClassDockID.ToString());
            Debug.WriteLine("clsSearch.ClassServiceDateFrom=" + clsSearch.ClassServiceDateFrom);
            Debug.WriteLine("clsSearch.ClassServiceDateTo=" + clsSearch.ClassServiceDateTo);
            Debug.WriteLine("clsSearch.ClassJobTypeDescription=" + clsSearch.ClassJobTypeDescription);
            Debug.WriteLine("clsSearch.ClassJobTypeStatusDescription=" + clsSearch.ClassJobTypeStatusDescription);
            Debug.WriteLine("clsSearch.ClassJobTypeDescriptionList=" + clsSearch.ClassJobTypeDescriptionList);
            Debug.WriteLine("clsSearch.ClassJobTypeStatusDescriptionList=" + clsSearch.ClassJobTypeStatusDescriptionList);
            Debug.WriteLine("clsSearch.ClassJobTypeList=" + clsSearch.ClassJobTypeList);
            Debug.WriteLine("clsSearch.ClassCurrentPage=" + clsSearch.ClassCurrentPage);

            fSelected = true;
            
            sMerchantDetails = "";
            sParticularDetails = "";
            sRequestDetails = "";
            sTerminalDetails = "";
            sDateDetails = "";

            sRequestDetails = "*Job Type Search Details" + "\n" +
                                  "   Job Type                      : " + clsSearch.ClassJobTypeList + "\n" +
                                   "   Job Type Status               : " + clsFunction.sDash + 
                                   "\n";

            sMerchantDetails = "*Merchant Search Details" + "\n" +
                                   "   Name                          : " + txtSearchMerchant.Text + "\n" +
                                   "   TID                           : " + txtSearchTID.Text + "\n" +
                                   "   MID                           : " + txtSearchMID.Text + "\n" +
                                   "   Request ID                    : " + txtSearchIRNo.Text +
                                   "\n";

            sParticularDetails = "*Particular Search Details" + "\n" +
                                   "   Client                          : " + txtSearchClient.Text + "\n" +
                                   "   Service Provider                : " + txtSearchSP.Text + "\n" +
                                   "   Field Engineer                  : " + txtSearchFE.Text + 
                                   "\n";
           
            sTerminalDetails = "*Serial Number Search Details" + "\n" +                                   
                                   "   Terminal                    : " + txtSearchTerminalSN.Text + "\n" +
                                   "   SIM                         : " + txtSearchSIMSN.Text + "\n" +
                                   "   Dock                        : " + txtSearchDockSN.Text + 
                                   "\n";

            sDateDetails = "*Date Search Details" + "\n" +
                                   "   Service Date From/To   : " + sServiceDateFrom + " / " + sServiceDateTo + 
                                   "\n";

            sSearchOptionDetails = sRequestDetails + sMerchantDetails + sParticularDetails + sTerminalDetails + sDateDetails;

            this.Close();

        }
        private bool CheckDateFromTo(DateTimePicker objFrom, DateTimePicker objTo, int iIndex)
        {
            bool fValid = true;
            int iResult;

            iResult = DateTime.Compare(DateTime.Parse(objFrom.Value.ToShortDateString()), DateTime.Parse(objTo.Value.ToShortDateString()));

            if (iResult > 0)
                fValid = false;

            if (!fValid)
            {
                switch (iIndex)
                {
                    case 0: // Service Date
                        MessageBox.Show("[Date From] shouldn't be greater than [Date To]" +
                                        "\n\n" +
                                        "Date From: " + objFrom.Value.ToString("MM-dd-yyyy") +
                                        "\n" +
                                        "Date To:      " + objTo.Value.ToString("MM-dd-yyyy"), "Service Date", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;                    
                }

            }

            return fValid;
        }

        private void frmFindMECriteria_Activated(object sender, EventArgs e)
        {
            txtSearchMerchant.Focus();
        }

        private void txtSearchTID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSearchMID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void CheckCheckBox(int iIndex)
        {
            InitDate(false);

            switch (iIndex)
            {
                case 0:
                    if (chkServiceDate.CheckState == CheckState.Checked)
                    {
                        dteServiceDateSearchFrom.Enabled = true;
                        dteServiceDateSearchTo.Enabled = true;
                    }
                    break;                
                default:
                    chkServiceDate.Checked = false;                    
                    break;
            }
        }
        private void InitDate(bool isEnable)
        {
            dteServiceDateSearchFrom.Enabled = isEnable;
            dteServiceDateSearchTo.Enabled = isEnable;            
        }

        private void chkServiceDate_CheckedChanged(object sender, EventArgs e)
        {
            CheckCheckBox(0);
        }

        private void btnSearchRegion_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iRegion;
            frmSearchField.sHeader = "REGION";
            frmSearchField.sSearchChar = txtSearchRegion.Text;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsSearch.ClassRegionType = clsSearch.ClassRegionType;
                txtSearchRegion.Text = clsSearch.ClassRegion;
            }
        }

        private void btnSearchCity_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iProvince;
            frmSearchField.sHeader = "CITY";
            frmSearchField.sSearchChar = txtSearchCity.Text;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsSearch.ClassRegionID = clsSearch.ClassRegionID;
                txtSearchCity.Text = clsSearch.ClassProvince;
            }
        }

        private void FillComboBoxJobTypeDescription()
        {
            int i = 0;

            ucCheckComboBoxJobType.Items.Clear();
            while (dbAPI.GetJobTypeDescription().Length > i)
            {                
                string sJobTypeDescription = dbAPI.GetJobTypeDescription()[i];
                ucCheckComboBoxJobType.Items.Add(new ucCheckComboBox.ComboboxData(sJobTypeDescription, false));

                i++;
            }

            if (i > 0)
                ucCheckComboBoxJobType.SelectedIndex = 0;

        }

        private void FillComboBoxJobTypeStatusDescription()
        {
            int i = 0;

            ucCheckComboBoxJobTypeStatus.Items.Clear();
            while (dbAPI.GetJobTypeStatusDescription().Length > i)
            {
                string sJobTypeStatusDescription = dbAPI.GetJobTypeStatusDescription()[i];
                ucCheckComboBoxJobTypeStatus.Items.Add(new ucCheckComboBox.ComboboxData(sJobTypeStatusDescription, false));

                i++;
            }

            if (i > 0)
                ucCheckComboBoxJobTypeStatus.SelectedIndex = 0;

        }

        private string GetListOfJobTypeDescription()
        {
            string sTemp = clsFunction.sZero;
            int i = 0;                        
            int iSelectedCount = 0;

            Debug.WriteLine("-GetListOfJobTypeDescription--");

            clsSearch.ClassJobTypeDescriptionList = clsFunction.sNull;

            // Hold To Temporary Array
            List<string> ServiceStatusCol = new List<String>();

            ServiceStatusCol.Clear();
            foreach (ucCheckComboBox.ComboboxData item in ucCheckComboBoxJobType.CheckItems)
            {
                sTemp = clsFunction.sZero;
                sTemp = dbAPI.GetServiceStatus(item.Data.ToString());
                if (dbFunction.isValidID(sTemp))
                    ServiceStatusCol.Add(sTemp);
            }
            clsArray.ServiceStatus = ServiceStatusCol.ToArray();

            // Check How Many Check
            iSelectedCount = 0;
            foreach (ucCheckComboBox.ComboboxData item in ucCheckComboBoxJobType.CheckItems)
            {
                if (item.Checked)
                {
                    iSelectedCount++;
                }
            }

            switch (iSelectedCount)
            {
                case 0: // [DEFAULT SELECT]
                    foreach (ucCheckComboBox.ComboboxData item in ucCheckComboBoxJobType.CheckItems)
                    {
                        sTemp = dbAPI.GetServiceStatus(item.Data.ToString());
                        clsSearch.ClassJobTypeDescriptionList = clsSearch.ClassJobTypeDescriptionList + sTemp + clsFunction.sComma;
                    }
                    break;
                case 1:

                    foreach (ucCheckComboBox.ComboboxData item in ucCheckComboBoxJobType.CheckItems)
                    {
                        if (item.Checked)
                        {
                            if (item.Data.ToString().CompareTo(clsFunction.sDefaultSelect) == 0)
                            {
                                foreach (ucCheckComboBox.ComboboxData itemx in ucCheckComboBoxJobType.CheckItems)
                                {
                                    sTemp = dbAPI.GetServiceStatus(itemx.Data.ToString());
                                    clsSearch.ClassJobTypeDescriptionList = clsSearch.ClassJobTypeDescriptionList + sTemp + clsFunction.sComma;
                                }                                
                            }
                            else
                            {
                                sTemp = dbAPI.GetServiceStatus(item.Data.ToString());
                                clsSearch.ClassJobTypeDescriptionList = clsSearch.ClassJobTypeDescriptionList + sTemp + clsFunction.sComma;
                            }

                            break;
                        }
                    }                        
                    break;
                default:
                    foreach (ucCheckComboBox.ComboboxData item in ucCheckComboBoxJobType.CheckItems)
                    {
                        if (item.Checked)
                        {
                            sTemp = dbAPI.GetServiceStatus(item.Data.ToString());
                            clsSearch.ClassJobTypeDescriptionList = clsSearch.ClassJobTypeDescriptionList + sTemp + clsFunction.sComma;
                        }
                    }
                    break;
            }            
            
            // Remove Last Pipe
            sTemp = clsSearch.ClassJobTypeDescriptionList.Remove(clsSearch.ClassJobTypeDescriptionList.Length - 1);

            Debug.WriteLine("sTemp=" + sTemp);

            return sTemp;
        }

        private string GetListOfJobTypeStatusDescription()
        {
            string sTemp = clsFunction.sZero;
            int i = 0;
            bool isCheck = false;

            Debug.WriteLine("-GetListOfJobTypeStatusDescription--");
            
            clsSearch.ClassJobTypeStatusDescriptionList = clsFunction.sNull;

            foreach (ucCheckComboBox.ComboboxData item in ucCheckComboBoxJobTypeStatus.CheckItems)
            {
                Debug.WriteLine("i=" + i.ToString() + "|item.Text=" + item.Checked);

                if (item.Checked)
                {
                    isCheck = true;
                    Debug.WriteLine("itemText=" + item.Data.ToString());
                    sTemp = clsFunction.sZero;
                    sTemp = dbAPI.GetJobTypeStatus(item.Data.ToString());
                    clsSearch.ClassJobTypeStatusDescriptionList = clsSearch.ClassJobTypeStatusDescriptionList + sTemp + clsFunction.sComma;
                }

                i++;
            }

            sTemp = clsFunction.sZero;
            if (isCheck)
            {
                // Remove Last Pipe
                sTemp = clsSearch.ClassJobTypeStatusDescriptionList.Remove(clsSearch.ClassJobTypeStatusDescriptionList.Length - 1);                
            }
            else
            {
                sTemp = clsFunction.sDefaultSelect;
            }
            
            Debug.WriteLine("sTemp="+ sTemp);

            return sTemp;
        }

        private string GetJobTypeList()
        {
            string sTemp = "";
            string sJobTypeList = "";

            foreach (ucCheckComboBox.ComboboxData item in ucCheckComboBoxJobType.CheckItems)
            {
                if (item.Checked)
                {
                    sTemp = item.Data.ToString();
                    sJobTypeList = sJobTypeList + sTemp + clsFunction.sPipe;
                }
            }

            return sJobTypeList;
        }
    }
}
