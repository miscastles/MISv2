using DocumentFormat.OpenXml.Office2010.PowerPoint;
using iText.Forms.Form.Element;
using MIS.Controller;
using MIS.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MIS.Function.AppUtilities;

namespace MIS
{
    public partial class frmServiceExpensesFSR : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        private clsFile dbFile;
        private clsReceiptImageProcessor dbReceiptImageProcessor;

        // Controller
        private ServicingDetailController _mServicingDetailController;
        private IRDetailController _mIRDetailController;
        private ZoningController _mZoningController;
        private ExpensesController _mExpensesController;

        private bool fEdit = false;

        private string formName = "EXPENSES - FSR";

        public frmServiceExpensesFSR()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwExpenseList, true);
            dbFunction.setDoubleBuffer(lvwServiceList, true);
            dbFunction.setDoubleBuffer(lvwReceiptList, true);

            // Initialize the controller object
            _mServicingDetailController = new ServicingDetailController();
            _mIRDetailController = new IRDetailController();
            _mZoningController = new ZoningController();
            _mExpensesController = new ExpensesController();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void frmServiceExpensesFSR_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void frmServiceExpensesFSR_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            lblHeader.Text = dbFunction.getSystemEnvironmentLabel($"{formName}");

            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);

            InitDate();
            initAmount();

            fEdit = false;
            InitButton();

            btnSearchService.Enabled = false;
            dbFunction.SetButtonIconImage(btnSearchService);

            btnSearchExpensesReferenceNo.Enabled = true;
            dbFunction.SetButtonIconImage(btnSearchExpensesReferenceNo);

            dbAPI.FillComboBoxServiceType(cboSearchServiceType);
            dbAPI.FillComboBoxExpenseType(cboExpenseType);

            initExpensesListView(lvwExpenseList);
            initServiceListView(lvwServiceList);
            initReceiptListView(lvwReceiptList);

            Cursor.Current = Cursors.Default;
        }

        private void initAmount()
        {
            txtExpenseAmount.Text = txtTotalExpenses.Text = "0.00";
        }

        private void InitDate()
        {   
            dtExpenseDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dtExpenseDate, clsFunction.sDateDefaultFormat);

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            dbAPI.GenerateID(true, txtExpenseReferenceNo, txtExpensesID, "Expenses-FSR", clsDefines.CONTROLID_PREFIX_EXPENSES);            

            lblHeader.Text = dbFunction.getSystemEnvironmentLabel($"CREATE {formName}");

            fEdit = false;            
            btnNew.Enabled = false;
            btnSave.Enabled = true;

            btnSearchService.Enabled = true;
            dbFunction.SetButtonIconImage(btnSearchService);

            btnSearchExpensesReferenceNo.Enabled = false;
            dbFunction.SetButtonIconImage(btnSearchExpensesReferenceNo);

            btnSearchServiceNos.Enabled = false;
            dbFunction.SetButtonIconImage(btnSearchServiceNos);

            txtCreatedBy.Text = txtUpdatedBy.Text = clsSearch.ClassCurrentParticularName;
            txtCreatedDate.Text = txtUpdatedDate.Text = dbFunction.getCurrentDateTime();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string sSQL = "";
            string sRowSQL = "";
            
            if (!ValidateFields()) return;

            if (!dbFunction.fSavingConfirm(true)) return;

            try
            {
                // ----------------------------------------------------------------------------------
                // API call to save tblexpensestransmaster
                // ----------------------------------------------------------------------------------
                var master = new
                {
                    ServiceDate = dbFunction.getCurrentDate(),
                    ExpensesDate = dbFunction.getCurrentDate(),                    
                    ReferenceNo = txtExpenseReferenceNo.Text,
                    ServiceNo = dbFunction.CheckAndSetNumericValue(txtServiceNo.Text),
                    IRIDNo = dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text),
                    MerchantID = int.Parse(dbFunction.CheckAndSetNumericValue(txtMerchantID.Text)),
                    ClientID = int.Parse(dbFunction.CheckAndSetNumericValue(txtClientID.Text)),
                    Location = txtMerchantCity.Text,
                    TotalAmount = decimal.Parse(txtTotalExpenses.Text),
                    CreatedBy = txtCreatedBy.Text,
                    CreatedDate = dbFunction.getCurrentDateTime(),
                    UpdatedBy = txtCreatedBy.Text,
                    UpdatedDate = dbFunction.getCurrentDateTime(),
                    ServiceNoList = txtServiceNoList.Text,
                    IRIDNoList = txtIRIDNoList.Text,
                    IRNoList = txtIRNoList.Text,
                    ReceiptList = "",
                    FEID = int.Parse(dbFunction.CheckAndSetNumericValue(txtFEID.Text)),
                    Remarks = txtRemarks.Text
                };

                sSQL = IFormat.Insert(master);

                Debug.WriteLine("--ExpensesTransMaster--");
                Debug.WriteLine($"sSQL={sSQL}");
                dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 0);

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "Expenses Trans Master", sSQL, "InsertCollectionMaster");

                Debug.WriteLine($"Last inserted ID = {clsLastID.ClassLastInsertedID}");
                txtExpnesesNo.Text = dbFunction.CheckAndSetNumericValue(clsLastID.ClassLastInsertedID.ToString());

                // ----------------------------------------------------------------------------------
                // API call to save tblexpensestransdetail
                // ----------------------------------------------------------------------------------
                foreach (ListViewItem item in lvwExpenseList.Items)
                {
                    string pExpensesID = item.SubItems[1].Text;
                    string pExpensesType = item.SubItems[2].Text;
                    string pExpensesDate = item.SubItems[3].Text;
                    string pExpensesAmount = item.SubItems[4].Text;
                    string pExpensesRemarks = item.SubItems[5].Text;

                    var detail = new
                    {
                        ExpensesNo = dbFunction.CheckAndSetNumericValue(txtExpnesesNo.Text),
                        ExpensesID = int.Parse(pExpensesID),
                        ServiceNo = dbFunction.CheckAndSetNumericValue(txtServiceNo.Text),
                        IRIDNo = dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text),
                        ExpensesReferenceNo = txtExpenseReferenceNo.Text,
                        ExpensesDate = pExpensesDate,
                        Amount = decimal.Parse(pExpensesAmount),
                        Remarks = pExpensesRemarks
                    };
                    
                    sSQL = IFormat.Insert(detail);

                    sRowSQL += sSQL + ",";
                    
                    Debug.WriteLine("--ExpensesTransDetail--");
                    dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 0);

                }

                // Remove extra comma at the end                
                if (sRowSQL.EndsWith(","))                
                    sRowSQL = sRowSQL.Substring(0, sRowSQL.Length - 1);                

                Debug.WriteLine("--ExpensesTransDetail--");
                Debug.WriteLine($"sRowSQL={sRowSQL}");

                if (dbFunction.isValidDescription(sRowSQL))
                {
                    dbAPI.ExecuteAPI("POST", "Insert", "", "", "Expenses Trans Detail", sRowSQL, "InsertCollectionDetail");
                }
                
                // Display messagebox completiion
                if (!fEdit)
                    dbFunction.SetMessageBox("Expenses successfully saved.", clsDefines.CONFIRMATION_MSG, clsFunction.IconType.iInformation);
                else
                    dbFunction.SetMessageBox("Expenses successfully updated.", clsDefines.CONFIRMATION_MSG, clsFunction.IconType.iInformation);

                btnClear_Click(this, e);

            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox(
                        "An error occurred while saving the expense information.\n\n" +
                        "Error: " + ex.Message,
                        "Saving failed",
                        clsFunction.IconType.iError);
            }
            

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dbFunction.ClearTextBox(this);
            dbFunction.ClearListViewItems(lvwExpenseList);
            dbFunction.ClearListViewItems(lvwServiceList);
            dbFunction.ClearListViewItems(lvwReceiptList);

            InitDate();
            initAmount();

            fEdit = false;
            InitButton();

            btnSearchService.Enabled = false;
            dbFunction.SetButtonIconImage(btnSearchService);

            btnSearchExpensesReferenceNo.Enabled = true;
            dbFunction.SetButtonIconImage(btnSearchExpensesReferenceNo);

            btnSearchServiceNos.Enabled = false;
            dbFunction.SetButtonIconImage(btnSearchServiceNos);

        }

        private void btnSearchExpensesReferenceNo_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iExpense;
            frmSearchField.sHeader = "EXPENSE REFERENCE";
            frmSearchField.isPreview = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                try
                {
                    dbFunction.ClearTextBox(this);                    
                    dbFunction.ClearListViewItems(lvwExpenseList);
                    dbFunction.ClearListViewItems(lvwServiceList);
                    dbFunction.ClearListViewItems(lvwReceiptList);

                    txtServiceNo.Text = $"{clsSearch.ClassServiceNo}";
                    txtIRIDNo.Text = $"{clsSearch.ClassIRIDNo}";
                    txtMerchantID.Text = $"{clsSearch.ClassMerchantID}";
                    txtExpenseReferenceNo.Text = $"{clsSearch.ClassExpenseReferenceNo}";

                    FillMerchantTextBox();
                    getZoningInfo();

                    getExpensesMasterInfo();
                    FillExpenseList();

                    fEdit = true;
                    InitButton();
                }
                catch (Exception ex)
                {
                        dbFunction.SetMessageBox(
                        "An error occurred while loading the expense information.\n\n" +
                        "Error: " + ex.Message,
                        "Loading failed",
                        clsFunction.IconType.iError
                    );
                }                
            }
        }

        private void btnSearchService_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iMerchant;
            frmSearchField.sHeader = "MERCHANT";
            frmSearchField.isPreview = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                btnClear_Click(this, e);

                txtIRIDNo.Text = $"{clsSearch.ClassIRIDNo}";
                txtMerchantID.Text = $"{clsSearch.ClassParticularID}";
                txtClientID.Text = $"{clsSearch.ClassClientID}";

                fEdit = false;
                btnNew.Enabled = false;
                btnSave.Enabled = true;

                btnSearchService.Enabled = true;
                dbFunction.SetButtonIconImage(btnSearchService);

                btnSearchExpensesReferenceNo.Enabled = false;
                dbFunction.SetButtonIconImage(btnSearchExpensesReferenceNo);

                btnSearchServiceNos.Enabled = true;
                dbFunction.SetButtonIconImage(btnSearchServiceNos);

                dbAPI.GenerateID(true, txtExpenseReferenceNo, txtExpensesID, "Expenses-FSR", clsDefines.CONTROLID_PREFIX_EXPENSES);

                txtCreatedBy.Text = txtUpdatedBy.Text = clsSearch.ClassCurrentParticularName;
                txtCreatedDate.Text = txtUpdatedDate.Text = dbFunction.getCurrentDateTime();

                FillMerchantTextBox();

                getZoningInfo();
            }
        }

        private void InitButton()
        {
            if (fEdit)
            {
                btnNew.Enabled = false;
                btnSave.Enabled = true;
            }
            else
            {
                btnNew.Enabled = true;
                btnSave.Enabled = false;
            }
        }

        private void FillMerchantTextBox()
        {
            if (dbFunction.isValidID(txtIRIDNo.Text) && dbFunction.isValidID(txtMerchantID.Text))
            {
                _mIRDetailController = _mIRDetailController.getMerchantInfo($"{txtMerchantID.Text}{clsDefines.gPipe}{txtIRIDNo.Text}");

                if (_mIRDetailController != null)
                {
                    txtClientID.Text = $"{_mIRDetailController.ClientID}";
                    txtClientName.Text = _mIRDetailController.ClientName;
                    txtMerchant.Text = _mIRDetailController.MerchantName;
                    txtTID.Text = _mIRDetailController.TID;
                    txtMID.Text = _mIRDetailController.MID;
                    txtMerchantAddress.Text = _mIRDetailController.Address;
                    txtMerchantCity.Text = _mIRDetailController.Province;
                    txtMerchantRegion.Text = _mIRDetailController.Region;
                    txtZoneID.Text = $"{_mIRDetailController.ZoneID}";
                }

            }
        }

        private void getZoningInfo()
        {
            txtZZone.Text = txtZZone.Text = txtZRegion.Text = clsDefines.gNull;

            if (dbFunction.isValidID(txtZoneID.Text))
            {
                modelZoning model = _mZoningController.getInfo(int.Parse(txtZoneID.Text));

                if (model != null)
                {
                    txtZZone.Text = model.Zone;
                    txtZRegion.Text = model.Region;
                }                
            }
        }

        private void getExpensesMasterInfo()
        {
            txtExpensesID.Text = "1";

            if (dbFunction.isValidID(txtExpensesID.Text))
            {
                modelExpensesMaster model = _mExpensesController.geMastertInfo(int.Parse(txtExpensesID.Text));

                if (model != null)
                {
                    txtExpenseReferenceNo.Text = model.ReferenceNo;
                    txtCreatedDate.Text = model.CreatedBy;
                    txtCreatedBy.Text = $"{model.CreatedDate}";
                    txtUpdatedDate.Text = model.UpdatedBy;
                    txtUpdatedBy.Text = $"{model.UpdatedDate}";
                    txtExpenseAmount.Text = $"{model.TotalAmount}";

                    txtServiceNoList.Text = $"{model.ServiceNoList}";                    
                    txtIRNoList.Text = $"{model.IRNoList}";
                }
            }
        }

        private void btnSearchServiceNos_Click(object sender, EventArgs e)
        {
            int i = 0;

            frmSearchField.iSearchType = frmSearchField.SearchType.iFSR;
            frmSearchField.sHeader = "SEARCH COMPLETED SERVICE";
            frmSearchField.sSearchChar = dbFunction.CheckAndSetStringValue(txtTID.Text);
            frmSearchField.isCheckBoxes = false;

            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                lblTServiceNos.Text = $"{clsDefines.gZero}";

                if (clsArray.ID.Length > 0)
                {
                    while (clsArray.ID.Length > i)
                    {
                        // ServiceNo
                        Debug.WriteLine($"ID = {clsArray.ID[i]}");

                        string pServiceNo = $"{clsArray.ID[i]}";
                        string pIRIDNo = $"{clsDefines.gZero}";

                        addServiceToServiceList(pServiceNo, pIRIDNo);

                        i++;
                    }
                }

                if (!frmSearchField.isCheckBoxes)
                {
                    txtServiceNo.Text = $"{clsSearch.ClassServiceNo}";
                    txtIRIDNo.Text = $"{clsSearch.ClassIRIDNo}";
                    txtFEID.Text = $"{clsSearch.ClassFEID}";
                }

                lblTServiceNos.Text = $"{clsArray.ID.Length}";

                displayList();
            }
        }

        private void initExpensesListView(ListView lvw)
        {
            string outField = "";
            int outWidth = 0;
            string outTitle = "";
            HorizontalAlignment outAlign = 0;
            bool outVisible = false;
            bool outAutoWidth = false;
            string outFormat = "";

            dbFunction = new clsFunction();

            lvw.Clear();
            lvw.View = View.Details;

            dbFunction.GetListViewHeaderColumnFromFile("", "Line#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ExpensesID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ExpensesType", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ExpensesDate", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ExpensesAmount", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ExpensesRemarks", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);            

        }

        private void initServiceListView(ListView lvw)
        {
            string outField = "";
            int outWidth = 0;
            string outTitle = "";
            HorizontalAlignment outAlign = 0;
            bool outVisible = false;
            bool outAutoWidth = false;
            string outFormat = "";

            dbFunction = new clsFunction();

            lvw.Clear();
            lvw.View = View.Details;

            dbFunction.GetListViewHeaderColumnFromFile("", "Line#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ServiceNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "IRIDNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Service Type", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Request No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Merchant", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "TID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "MID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

        }

        private void initReceiptListView(ListView lvw)
        {
            string outField = "";
            int outWidth = 0;
            string outTitle = "";
            HorizontalAlignment outAlign = 0;
            bool outVisible = false;
            bool outAutoWidth = false;
            string outFormat = "";

            dbFunction = new clsFunction();

            lvw.Clear();
            lvw.View = View.Details;

            dbFunction.GetListViewHeaderColumnFromFile("", "Line#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Receipt", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ReceiptAmount", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

        }

        private void addServiceToServiceList(string pServiceNo, string pIRIDNo)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                // -------------------------------------------------------------
                // Fill additional info
                // -------------------------------------------------------------
                _mServicingDetailController =
                    _mServicingDetailController.getServicingInfo(
                        $"{pServiceNo}{clsDefines.gPipe}{pIRIDNo}"
                    );

                string ServiceNo = $"{_mServicingDetailController.ServiceNo}";
                string JobType = $"{_mServicingDetailController.JobType}";
                string IRIDNo = $"{_mServicingDetailController.IRIDNo}";
                string RequestID = $"{_mServicingDetailController.IRNo}";
                string Merchant = $"{_mServicingDetailController.MerchantName}";
                string TID = $"{_mServicingDetailController.TID}";
                string MID = $"{_mServicingDetailController.MID}";
                string JobTypeDescription =
                    $"{_mServicingDetailController.ServiceJobTypeDescription}";
                string ActionMade =
                    $"{_mServicingDetailController.ActionMade}".Trim();


                // -------------------------------------------------------------
                // Validate Action Made
                // Only SUCCESS or NEGATIVE can be added
                // -------------------------------------------------------------
                if (!dbFunction.isValidDescription(ActionMade))
                {
                    dbFunction.SetMessageBox(
                        $"Service #{ServiceNo} cannot be added.\n\n" +
                        $"Action Made: {ActionMade}\n\n" +
                        "Only SUCCESS or NEGATIVE services can be added.",
                        "Invalid Service",
                        clsFunction.IconType.iWarning
                    );

                    return;
                }


                // -------------------------------------------------------------
                // Prevent duplicate Service #
                // -------------------------------------------------------------
                foreach (ListViewItem item in lvwServiceList.Items)
                {
                    string existingServiceNo = item.SubItems[1].Text;
                    string existingIRIDNo = item.SubItems[2].Text;

                    if (existingServiceNo.Equals(
                            ServiceNo,
                            StringComparison.OrdinalIgnoreCase) &&
                        existingIRIDNo.Equals(
                            IRIDNo,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
                }


                // -------------------------------------------------------------
                // Add Service to ListView
                // -------------------------------------------------------------
                int lineNo = lvwServiceList.Items.Count + 1;

                ListViewItem lvi =
                    new ListViewItem(lineNo.ToString());

                lvi.SubItems.Add(ServiceNo);
                lvi.SubItems.Add(IRIDNo);
                lvi.SubItems.Add(JobTypeDescription);
                lvi.SubItems.Add(RequestID);
                lvi.SubItems.Add(Merchant);
                lvi.SubItems.Add(TID);
                lvi.SubItems.Add(MID);

                lvwServiceList.Items.Add(lvi);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void displayList()
        {
            string pServiceNos = getSelectedListView(lvwServiceList, 1);
            string pIRIDNos = getSelectedListView(lvwServiceList, 2);
            string pIRNos = getSelectedListView(lvwServiceList, 4);

            txtServiceNoList.Text = pServiceNos;
            txtIRIDNoList.Text = pIRIDNos;
            txtIRNoList.Text = pIRNos;

        }

        private string getSelectedListView(ListView lvw, int colIndex)
        {
            List<string> selectedList = new List<string>();

            foreach (ListViewItem item in lvw.Items)
            {
                string selected = item.SubItems[colIndex].Text;

                if (!string.IsNullOrWhiteSpace(selected))
                {
                    selectedList.Add(selected.Trim());
                }
            }

            return JsonConvert.SerializeObject(selectedList);
        }

        private void FillExpenseList()
        {
            int i = 0;
            int iLineNo = 0;

            lvwExpenseList.Items.Clear();
            txtTotalExpenses.Text = "0.00";

            dbAPI.ExecuteAPI("GET", "View", "Expenses Transaction Detail", txtServiceNo.Text + clsDefines.gPipe + txtIRIDNo.Text, "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound()) return;

            if (clsArray.ID == null || clsArray.detail_info == null) return;

            while (clsArray.ID.Length > i)
            {
                iLineNo++;

                string pExpenseID = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ExpensesID");
                string pExpenseReferenceNo =dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ExpensesReferenceNo");
                string pExpenseType = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ExpenseType");
                string pAmount = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ExpensesAmount");
                string pRemarks = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ExpensesDescription");
                string pExpenseDate = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ExpensesDate");

                decimal dExpenseAmount;

                if (!decimal.TryParse(pAmount, out dExpenseAmount))
                {
                    dExpenseAmount = 0;
                }

                ListViewItem item = new ListViewItem(pExpenseID);

                item.SubItems.Add(iLineNo.ToString());
                item.SubItems.Add(pExpenseReferenceNo);
                item.SubItems.Add(pExpenseType);
                item.SubItems.Add(dExpenseAmount.ToString("N2"));
                item.SubItems.Add(pRemarks);
                item.SubItems.Add(pExpenseDate);

                item.Tag = clsArray.detail_info[i];

                lvwExpenseList.Items.Add(item);

                i++;
            }

            dbFunction.ListViewAlternateBackColor(lvwExpenseList);

            txtTotalExpenses.Text = $"{ComputeTotalExpenses()}";
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidID(txtServiceNo.Text))
            {
                dbFunction.SetMessageBox(
                    "Please select a valid service first.",
                    "Generate expense report",
                    clsFunction.IconType.iWarning
                );

                return;
            }

            if (clsSearch.ClassMerchantID <= 0)
            {
                dbFunction.SetMessageBox(
                    "The selected service has no valid merchant.",
                    "Generate expense report",
                    clsFunction.IconType.iWarning
                );

                return;
            }

            clsReport.ClassReportDesc = "OPERATIONS REIMBURSEMENT REPORT";

            clsSearch.ClassReportID = 61;
            clsSearch.ClassReportDescription = clsReport.ClassReportDesc;

            clsSearch.ClassStatementType = "View";
            clsSearch.ClassSearchBy = "Expenses-Report";

            clsSearch.ClassSearchValue = dbFunction.CheckAndSetNumericValue(txtServiceNo.Text) +
                clsDefines.gPipe +
                dbFunction.CheckAndSetNumericValue(clsSearch.ClassMerchantID.ToString());

            clsSearch.ClassStoredProcedureName = "spViewReport";

            dbFunction.ProcessReport(clsSearch.ClassReportID);
        }

        private void cboExpenseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtExpenseAmount.Text = "0.00";
            txtExpenseAmount.SelectAll();
            txtExpenseAmount.Focus();
        }

        private void cboSearchServiceType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAddExpense_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidDescriptionEntry(cboExpenseType.Text, "Expenses Type." + clsDefines.MUST_NOT_BLANK_MESSAGE))
            {
                cboExpenseType.Focus();
                return;
            }

            if (!dbFunction.isValidAmount(txtExpenseAmount.Text))
            {
                dbFunction.SetMessageBox("Please enter a valid expense amount.", "Expense Amount", clsFunction.IconType.iWarning);

                txtExpenseAmount.SelectAll();
                txtExpenseAmount.Focus();
                return;
            }

            dbFunction.GetIDFromFile("Expense List", cboExpenseType.Text);
            clsSearch.ClassExpensesID = clsSearch.ClassOutFileID;

            // populate model
            modelExpensesDetail model = new modelExpensesDetail();
            model.ExpensesID = clsSearch.ClassExpensesID;
            model.ExpensesType = cboExpenseType.Text;
            model.ExpensesDate = dbFunction.CheckAndSetDatePickerValueToDate(dtExpenseDate);
            model.Amount = decimal.Parse(txtExpenseAmount.Text);
            model.Remarks = txtExpensesRemarks.Text;

            addExpensesToExpensesList(model);

            txtTotalExpenses.Text = $"{ComputeTotalExpenses()}";
        }

        private void addExpensesToExpensesList(modelExpensesDetail pModel)
        {
            if (pModel == null)
                return;


            // -------------------------------------------------------------
            // Prevent exact duplicate expense entry
            // -------------------------------------------------------------
            foreach (ListViewItem item in lvwExpenseList.Items)
            {
                if (item.SubItems.Count <= 5)
                    continue;

                string existingExpensesID = item.SubItems[1].Text.Trim();
                string existingExpensesDate = item.SubItems[3].Text.Trim();
                string existingAmount = item.SubItems[4].Text.Trim();
                string existingRemarks = item.SubItems[5].Text.Trim();

                // ---------------------------------------------------------
                // Compare values
                // ---------------------------------------------------------
                bool sameExpensesID =
                    existingExpensesID.Equals(
                        pModel.ExpensesID.ToString(),
                        StringComparison.OrdinalIgnoreCase
                    );

                bool sameExpensesDate =
                    existingExpensesDate.Equals(
                        pModel.ExpensesDate?.Trim(),
                        StringComparison.OrdinalIgnoreCase
                    );

                decimal existingAmountValue = 0M;

                decimal.TryParse(
                    existingAmount.Replace(",", ""),
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out existingAmountValue
                );

                bool sameAmount =
                    existingAmountValue == pModel.Amount;

                bool sameRemarks =
                    existingRemarks.Equals(
                        pModel.Remarks?.Trim() ?? "",
                        StringComparison.OrdinalIgnoreCase
                    );


                // ---------------------------------------------------------
                // Exact duplicate
                // ---------------------------------------------------------
                if (sameExpensesID &&
                    sameExpensesDate &&
                    sameAmount &&
                    sameRemarks)
                {
                    dbFunction.SetMessageBox(
                        "This expense entry already exists in the list.\n\n" +
                        "Expense Type: " + pModel.ExpensesType + "\n" +
                        "Amount: ₱" + pModel.Amount + "\n" +
                        "Date: " + pModel.ExpensesDate,
                        "Duplicate Expense",
                        clsFunction.IconType.iWarning
                    );

                    return;
                }
            }


            // -------------------------------------------------------------
            // Add Expenses to ListView
            // -------------------------------------------------------------
            int iLineNo = lvwExpenseList.Items.Count + 1;

            ListViewItem lvi = new ListViewItem(iLineNo.ToString());


            // ExpensesID
            lvi.SubItems.Add(pModel.ExpensesID.ToString());

            // Expenses Type
            lvi.SubItems.Add(pModel.ExpensesType ?? "");

            // Expenses Date
            lvi.SubItems.Add(pModel.ExpensesDate ?? "");

            // Amount
            lvi.SubItems.Add(pModel.Amount.ToString());

            // Remarks
            lvi.SubItems.Add(pModel.Remarks ?? "");

            // -------------------------------------------------------------
            // Store model for Edit / Save / Delete
            // -------------------------------------------------------------
            lvi.Tag = pModel;

            lvwExpenseList.Items.Add(lvi);
        }

        private void btnServiceRemove_Click(object sender, EventArgs e)
        {
            dbFunction.removeItemListView(lvwServiceList, false);
        }

        private void btnServiceClearAll_Click(object sender, EventArgs e)
        {
            dbFunction.ClearListViewItems(lvwServiceList);

            txtServiceNoList.Text = txtIRIDNoList.Text = txtIRNoList.Text = "";
            btnSearchServiceNos.Focus();
        }

        private void btnExpenseRemove_Click(object sender, EventArgs e)
        {
            dbFunction.removeItemListView(lvwExpenseList, false);

            txtTotalExpenses.Text = $"{ComputeTotalExpenses()}";
        }

        private void btnExpenseClearAll_Click(object sender, EventArgs e)
        {
            dbFunction.ClearListViewItems(lvwExpenseList);

            txtTotalExpenses.Text = $"{ComputeTotalExpenses()}";
            txtTotalExpenses.Text = "0.00";
            cboExpenseType.Focus();
        }

        private void lvwExpenseList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwExpenseList.SelectedItems.Count == 0)
                return;

            ListViewItem item = lvwExpenseList.SelectedItems[0];

            if (item.Tag == null)
                return;

            modelExpensesMaster model = item.Tag as modelExpensesMaster;

            if (model == null)
                return;


            // -------------------------------------------------------------
            // Load selected expense to entry fields
            // -------------------------------------------------------------

            // Expense Type
            cboExpenseType.Text = model.ExpensesType ?? "";

            // Expense Amount
            txtExpenseAmount.Text = model.TotalAmount.ToString("N2");

            // Remarks
            txtExpensesRemarks.Text = model.Remarks ?? "";
        }

        private bool ValidateFields()
        {
            bool isValid = true;

            // Service List
            if (!dbFunction.isValidCount(lvwServiceList.Items.Count))
            {
                dbFunction.SetMessageBox("Please select at least one service.",clsDefines.FIELD_CHECK_MSG,clsFunction.IconType.iWarning);

                isValid = false;
            }

            // Receipt List
            /*
            if (!dbFunction.isValidCount(lvwReceiptList.Items.Count))
            {
                dbFunction.SetMessageBox("Please select at least one receipt.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);

                isValid = false;
            }
            */

            // Expenses List
            if (!dbFunction.isValidCount(lvwExpenseList.Items.Count))
            {
                dbFunction.SetMessageBox("Please select at least one expense.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);

                isValid = false;
            }

            // Reference No
            if (!dbFunction.isValidDescription(txtExpenseReferenceNo.Text))
            {
                dbFunction.SetMessageBox("Reference number must not be blank.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);

                isValid = false;
            }

            // Merchant
            if (!dbFunction.isValidDescription(txtMerchant.Text) || !dbFunction.isValidID(txtMerchantID.Text))
            {
                dbFunction.SetMessageBox("Merchant must not be blank.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);

                isValid = false;
            }

            // Client
            if (!dbFunction.isValidDescription(txtClientName.Text) || !dbFunction.isValidID(txtClientID.Text))
            {
                dbFunction.SetMessageBox("Client must not be blank.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);

                isValid = false;
            }

            // Expenses Amount
            if (!dbFunction.isValidAmount(txtTotalExpenses.Text))
            {
                dbFunction.SetMessageBox("Expenses total amount must not be blank.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);

                isValid = false;
            }

            return isValid;
        }

        private decimal ComputeTotalExpenses()
        {
            decimal dTotalExpenses = 0M;

            foreach (ListViewItem item in lvwExpenseList.Items)
            {
                if (item.SubItems.Count <= 4)
                    continue;

                decimal dAmount = 0M;

                decimal.TryParse(
                    item.SubItems[4].Text.Trim(),
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out dAmount
                );

                dTotalExpenses += dAmount;
            }

            return dTotalExpenses;
        }

        private void txtExpenseAmount_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    txtExpensesRemarks.Focus();
                    break;
            }
        }

        private void txtRemarks_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    btnAddExpense.Focus();
                    break;
            }
        }

        private void btnClearExpense_Click(object sender, EventArgs e)
        {
            cboExpenseType.SelectedIndex = 0;
            txtExpenseAmount.Text = "0.00";
            txtExpensesRemarks.Text = "";
            InitDate();
        }

        private void btnReceiptAdd_Click(object sender, EventArgs e)
        {

        }

        private void btnReceiptEdit_Click(object sender, EventArgs e)
        {

        }

        private void btnReceiptDelete_Click(object sender, EventArgs e)
        {

        }

        private void btnReceiptDownload_Click(object sender, EventArgs e)
        {

        }
    }
}
