using DocumentFormat.OpenXml.Office2010.PowerPoint;
using iText.Forms.Form.Element;
using MIS.Controller;
using MIS.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Windows.Forms;
using System.Globalization;
using static MIS.AppData.ConstData.Api;
using static MIS.Function.AppUtilities;

namespace MIS
{
    public partial class frmServiceExpensesFSR : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        private clsFile dbFile;
        private clsReceiptImageProcessor dbReceiptImageProcessor;
        private string pExpenseFTPHost = $"{clsGlobalVariables.strFTPURL}/{clsGlobalVariables.strFTPUploadPath}/expenses/{clsSearch.ClassBankCode}";

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
            dbReceiptImageProcessor = new clsReceiptImageProcessor();
            dbFile = new clsFile();

            lblHeader.Text = dbFunction.getSystemEnvironmentLabel($"{formName}");

            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);

            InitDate();
            initAmount();

            fEdit = false;
            InitButton();

            btnSearchExpensesReferenceNo.Enabled = true;
            dbFunction.SetButtonIconImage(btnSearchExpensesReferenceNo);

            btnSearchFieldEngineer.Enabled = false;
            dbFunction.SetButtonIconImage(btnSearchFieldEngineer);

            dbAPI.FillComboBoxServiceType(cboSearchServiceType);
            dbAPI.FillComboBoxExpenseType(cboExpenseType);

            initExpensesListView(lvwExpenseList);
            initServiceListView(lvwServiceList);
            initReceiptListView(lvwReceiptList);

            initReportButton(false);

            Cursor.Current = Cursors.Default;
        }

        private void initAmount()
        {
            txtExpenseAmount.Text = txtTotalExpenses.Text = txtTotalReceptAmount.Text = "0.00";
        }

        private void InitDate()
        {
            dtExpenseDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dtExpenseDate, clsFunction.sStandardDateDefault);

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            dbAPI.GenerateID(true, txtExpenseReferenceNo, txtExpensesNo, "Expenses-FSR", clsDefines.CONTROLID_PREFIX_EXPENSES);

            lblHeader.Text = dbFunction.getSystemEnvironmentLabel($"CREATE {formName}");

            fEdit = false;
            btnNew.Enabled = false;
            btnSave.Enabled = true;

            btnSearchExpensesReferenceNo.Enabled = false;
            dbFunction.SetButtonIconImage(btnSearchExpensesReferenceNo);

            btnSearchFieldEngineer.Enabled = true;
            dbFunction.SetButtonIconImage(btnSearchFieldEngineer);

            btnSearchServiceNos.Enabled = false;
            dbFunction.SetButtonIconImage(btnSearchServiceNos);

            txtCreatedBy.Text = txtUpdatedBy.Text = clsSearch.ClassCurrentParticularName;
            txtCreatedDate.Text = txtUpdatedDate.Text = dbFunction.getCurrentDateTime();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string sSQL = "";
            string sRowSQL = "";
            string pSearchBy = "";
            string pSearchValue = "";

            if (!ValidateFields()) return;

            if (!dbFunction.fSavingConfirm(true)) return;

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                displayList();

                // ----------------------------------------------------------------------------------
                // API call to save tblexpensestransmaster
                // ----------------------------------------------------------------------------------
                var master = new
                {
                    ServiceDate = dbFunction.getCurrentDate(),
                    ExpensesDate = dbFunction.getCurrentDate(),
                    ReferenceNo = txtExpenseReferenceNo.Text,
                    ServiceNo = 0,
                    IRIDNo = 0,
                    MerchantID = 0,
                    ClientID = 0,
                    Location = "",
                    TotalAmount = decimal.Parse(txtTotalExpenses.Text),
                    CreatedBy = txtCreatedBy.Text,
                    CreatedDate = dbFunction.getCurrentDateTime(),
                    UpdatedBy = txtCreatedBy.Text,
                    UpdatedDate = dbFunction.getCurrentDateTime(),
                    ServiceNoList = txtServiceNoList.Text,
                    IRIDNoList = txtIRIDNoList.Text,
                    MerchantIDList = txtMerchantIDList.Text,
                    IRNoList = txtIRNoList.Text,
                    ReceiptList = txtReceiptList.Text,
                    ReceiptDateList = txtReceiptDateList.Text,
                    ReceiptAmountList = txtReceiptAmountList.Text,
                    FEID = int.Parse(dbFunction.CheckAndSetNumericValue(txtFEID.Text)),
                    Remarks = txtRemarks.Text
                };

                sSQL = IFormat.Insert(master, true);

                Debug.WriteLine("--ExpensesTransMaster--");
                Debug.WriteLine($"sSQL={sSQL}");
                dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 0);

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "Expenses Trans Master", sSQL, "InsertCollectionMaster");

                Debug.WriteLine($"Last inserted ID = {clsLastID.ClassLastInsertedID}");
                txtExpensesNo.Text = dbFunction.CheckAndSetNumericValue(clsLastID.ClassLastInsertedID.ToString());

                // ----------------------------------------------------------------------------------
                // API call to save tblexpensestransdetail
                // ----------------------------------------------------------------------------------
                foreach (ListViewItem item in lvwExpenseList.Items)
                {
                    string pExpensesID = item.SubItems[1].Text;
                    string pExpensesType = item.SubItems[2].Text;
                    string pExpensesDate = item.SubItems[3].Text;
                    string pExpensesLocatonFrom = item.SubItems[4].Text;
                    string pExpensesLocationTo = item.SubItems[5].Text;
                    string pExpensesAmount = item.SubItems[6].Text;
                    string pExpensesRemarks = item.SubItems[7].Text;
                    string pMerchantName = item.SubItems[8].Text;
                    string pReceiptFileName = item.SubItems[9].Text;

                    var detail = new
                    {
                        ExpensesNo = dbFunction.CheckAndSetNumericValue(txtExpensesNo.Text),
                        ExpensesID = int.Parse(pExpensesID),
                        ServiceNo = 0,
                        IRIDNo = 0,
                        ExpensesReferenceNo = txtExpenseReferenceNo.Text,
                        ExpensesDate = pExpensesDate,
                        Amount = decimal.Parse(pExpensesAmount),
                        Remarks = pExpensesRemarks,
                        LocationFrom = pExpensesLocatonFrom,
                        LocationTo = pExpensesLocationTo,
                        MerchantName = pMerchantName,
                        ReceiptFileName = pReceiptFileName
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

                // -------------------------------------------------
                // update syncID -> Expenses Trans Master
                // -------------------------------------------------
                pSearchBy = "Expenses Trans Master";
                pSearchValue = $"{pSearchBy}{clsDefines.gPipe}{txtExpensesNo.Text}{clsDefines.gPipe}{dbFunction.generateSyncID(master, Enums.SyncEntity.Expenses_Master)}";

                Debug.WriteLine("--Update SyncID ExpensesTransMaster--");
                dbFunction.parseDelimitedString(pSearchValue, clsDefines.gPipe, 0);

                dbAPI.ExecuteAPI("PUT", "Update", "SyncID", pSearchValue, "", "", "UpdateCollectionDetail");

                // -------------------------------------------------
                // update syncID -> Expenses Trans Detail
                // -------------------------------------------------               
                List<modelExpensesDetail> detailList = _mExpensesController.getDetailList<modelExpensesDetail>("Expense Reference List", txtExpenseReferenceNo.Text);
                if (detailList != null)
                {
                    foreach (modelExpensesDetail _mDetail in detailList)
                    {
                        Debug.WriteLine($"DetailID = {_mDetail.DetailID}");
                        Debug.WriteLine($"ExpensesID = {_mDetail.ExpensesID}");

                        pSearchBy = "Expenses Trans Detail";
                        pSearchValue = $"{pSearchBy}{clsDefines.gPipe}{_mDetail.DetailID}{clsDefines.gPipe}{dbFunction.generateSyncID(_mDetail, Enums.SyncEntity.Expenses_Detail)}";

                        Debug.WriteLine("--Update SyncID ExpensesTransDetail--");
                        dbFunction.parseDelimitedString(pSearchValue, clsDefines.gPipe, 0);

                        dbAPI.ExecuteAPI("PUT", "Update", "SyncID", pSearchValue, "", "", "UpdateCollectionDetail");
                    }
                }

                // ----------------------------------------------------------------------------------
                // Upload receipt images to ftp /upload/expenses/bank/
                // ----------------------------------------------------------------------------------
                uploadReceiptFTP(txtExpenseReferenceNo.Text);

                // Display messagebox completiion
                if (fEdit)                    
                    dbFunction.SetMessageBox("Expenses successfully updated.", clsDefines.CONFIRMATION_MSG, clsFunction.IconType.iInformation);
                else
                    dbFunction.SetMessageBox("Expenses successfully saved.", clsDefines.CONFIRMATION_MSG, clsFunction.IconType.iInformation);


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

            Cursor.Current = Cursors.Default;
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

            btnSearchExpensesReferenceNo.Enabled = true;
            dbFunction.SetButtonIconImage(btnSearchExpensesReferenceNo);

            btnSearchServiceNos.Enabled = false;
            dbFunction.SetButtonIconImage(btnSearchServiceNos);

            btnSearchFieldEngineer.Enabled = false;
            dbFunction.SetButtonIconImage(btnSearchFieldEngineer);

            initExpensesEntry();

            pbReceiptPreview.Image = null;

            initReportButton(false);

        }

        private void btnSearchExpensesReferenceNo_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iExpensesMaster;
            frmSearchField.sHeader = "EXPENSE REFERENCE";
            frmSearchField.isPreview = false;
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    dbFunction.ClearTextBox(this);
                    dbFunction.ClearListViewItems(lvwExpenseList);
                    dbFunction.ClearListViewItems(lvwServiceList);
                    dbFunction.ClearListViewItems(lvwReceiptList);

                    // search expenses master info
                    modelExpensesMaster model = _mExpensesController.geMastertInfo(clsSearch.ClassExpensesNo);

                    if (model != null)
                    {
                        clsSearch.ClassServiceNo = model.ServiceNo;
                        clsSearch.ClassIRIDNo = model.IRIDNo;
                        clsSearch.ClassMerchantID = model.MerchantID;
                        clsSearch.ClassExpenseReferenceNo = model.ReferenceNo;
                        clsSearch.ClassFEID = model.FEID;
                        clsSearch.ClassFEName = model.FEName;

                        txtExpensesNo.Text = $"{clsSearch.ClassExpensesNo}";                        ;
                        txtExpenseReferenceNo.Text = $"{clsSearch.ClassExpenseReferenceNo}";
                        txtFEID.Text = $"{clsSearch.ClassFEID}";
                        txtFEName.Text = $"{clsSearch.ClassFEName}";

                        getExpensesMasterInfo();
                        FillExpenseList();

                        // ----------------------------------------------------------------
                        // Fill Service
                        // ----------------------------------------------------------------
                        dbFunction.ClearListViewItems(lvwServiceList);
                        
                        List<string> pServiceList = dbFunction.ParseCSVtoArray(model.ServiceNoList);

                        foreach (string serviceNo in pServiceList)
                        {
                            string pServiceNo = serviceNo.Trim();
                            string pIRIDNo = $"{clsDefines.gZero}";

                            addServiceToServiceList(pServiceNo, pIRIDNo);
                        }
                        // ----------------------------------------------------------------
                        // Fill Service
                        // ----------------------------------------------------------------

                        // ----------------------------------------------------------------
                        // Fill Receipt
                        // ----------------------------------------------------------------
                        dbFunction.ClearListViewItems(lvwReceiptList);
                        
                        List<string> pReceiptList = dbFunction.ParseCSVtoArray(model.ReceiptList);                        
                        List<string> pReceiptDateList = dbFunction.ParseCSVtoArray(model.ReceiptDateList);                        
                        List<string> pReceiptAmountList = dbFunction.ParseCSVtoArray(model.ReceiptAmountList);

                        for (int i = 0; i < pReceiptList.Count; i++)
                        {
                            string pFileName = pReceiptList[i].Trim();
                            string pDate = pReceiptDateList[i].Trim();
                            string pAmount = pReceiptAmountList[i].Trim();

                            modelExpensesReceipt expensesReceipt = new modelExpensesReceipt();

                            expensesReceipt.ExpensesFileName = pFileName;
                            expensesReceipt.ExpensesDate = pDate;

                            decimal pAmountValue = 0;

                            decimal.TryParse(pReceiptAmountList[i].Trim(),out pAmountValue);

                            expensesReceipt.ExpensesAmount = pAmountValue;

                            addReceiptToReceiptList(expensesReceipt);
                        }
                        // ----------------------------------------------------------------
                        // Fill Receipt
                        // ----------------------------------------------------------------

                        txtRemarks.Text = model.Remarks;

                        txtTServiceCount.Text = $"{lvwServiceList.Items.Count}";
                        
                        txtTotalReceptAmount.Text = $"{ComputeTotalAmount(lvwReceiptList, 2)}";

                        txtTotalExpenses.Text = $"{ComputeTotalAmount(lvwExpenseList, 6)}";

                        dbFunction.RefreshCountListView(txtTReceiptCount, lvwReceiptList);

                        displayList();

                        fEdit = true;

                        InitButton();

                        initReportButton(true);

                        btnSave.Enabled = false; // to be remove

                        Cursor.Current = Cursors.Default;
                    }

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

        private void getExpensesMasterInfo()
        {
            if (dbFunction.isValidID(txtExpensesNo.Text))
            {
                modelExpensesMaster model = _mExpensesController.geMastertInfo(int.Parse(txtExpensesNo.Text));

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

            if (!dbFunction.isValidDescriptionEntry(txtFEName.Text, "Field Engineer" + clsDefines.MUST_NOT_BLANK_MESSAGE))
            {
                btnSearchFieldEngineer.Focus();
                return;
            }

            // FEID
            clsSearch.ClassFEID = int.Parse(dbFunction.CheckAndSetNumericValue(txtFEID.Text));

            // -------------------------------------------------------------
            // set JobTypeDescription
            // -------------------------------------------------------------
            clsSearch.ClassJobType = 0;
            clsSearch.ClassJobTypeDescription = "";
            if (dbFunction.isValidDescription(cboSearchServiceType.Text))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Service Type Info", cboSearchServiceType.Text, "Get Info Detail", "", "GetInfoDetail");

                if (dbAPI.isNoRecordFound() == false)
                {
                    clsSearch.ClassJobType = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5));
                    clsSearch.ClassJobTypeDescription = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);

                }
            }
            // -------------------------------------------------------------
            // -------------------------------------------------------------

            frmSearchField.iSearchType = frmSearchField.SearchType.iFSR;
            frmSearchField.sHeader = "SEARCH COMPLETED SERVICE";
            //frmSearchField.sSearchChar = dbFunction.CheckAndSetStringValue(txtFEName.Text);
            frmSearchField.isCheckBoxes = true;

            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                txtTServiceCount.Text = $"{clsDefines.gZero}";

                if (clsArray.ID.Length > 0)
                {
                    while (clsArray.ID.Length > i)
                    {
                        // ServiceNo
                        Debug.WriteLine($"ID = {clsArray.ID[i]}");

                        string pServiceNo = $"{clsArray.ID[i]}";
                        string pIRIDNo = $"{clsDefines.gZero}";

                        Debug.WriteLine($"pServiceNo={pServiceNo}, pIRIDNo={pIRIDNo}");

                        addServiceToServiceList(pServiceNo, pIRIDNo);

                        i++;
                    }
                }

                txtTServiceCount.Text = $"{clsArray.ID.Length}";

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

            dbFunction.GetListViewHeaderColumnFromFile("", "Location From", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Location To", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ExpensesAmount", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ExpensesRemarks", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Merchant", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Receipt", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
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

            dbFunction.GetListViewHeaderColumnFromFile("", "MerchantID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
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

            dbFunction.GetListViewHeaderColumnFromFile("", "FieldEngineer", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
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

            dbFunction.GetListViewHeaderColumnFromFile("", "ReceiptDate", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);
        }

        private void addServiceToServiceList(string pServiceNo, string pIRIDNo)
        {
            string ServiceNo = "";
            string JobType = "";
            string IRIDNo = "";
            string RequestID = "";
            string Merchant = "";
            string TID = "";
            string MID = "";
            string JobTypeDescription = "";
            string ActionMade = "";
            string FEName = "";
            string MerchantID = "";

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                // -------------------------------------------------------------
                // Fill additional info
                // -------------------------------------------------------------
                _mServicingDetailController = _mServicingDetailController.getServicingInfo($"{pServiceNo}{clsDefines.gPipe}{pIRIDNo}");

                ServiceNo = $"{_mServicingDetailController.ServiceNo}";
                JobType = $"{_mServicingDetailController.JobType}";
                IRIDNo = $"{_mServicingDetailController.IRIDNo}";
                RequestID = $"{_mServicingDetailController.IRNo}";
                Merchant = $"{_mServicingDetailController.MerchantName}";
                TID = $"{_mServicingDetailController.TID}";
                MID = $"{_mServicingDetailController.MID}";
                JobTypeDescription = $"{_mServicingDetailController.ServiceJobTypeDescription}";
                ActionMade = $"{_mServicingDetailController.ActionMade}".Trim();
                FEName = $"{_mServicingDetailController.FEName}";
                MerchantID = $"{_mServicingDetailController.MerchantID}";

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
                // Validate Field Engineer
                // -------------------------------------------------------------
                if (!dbFunction.isValidDescription(FEName))
                {
                    dbFunction.SetMessageBox(
                        $"Service #{ServiceNo} cannot be added.\n\n" +
                        "Field Engineer is not assigned to this service.",
                        "Invalid Field Engineer",
                        clsFunction.IconType.iWarning
                    );

                    return;
                }

                if (!dbFunction.isValidDescription(txtFEName.Text))
                {
                    dbFunction.SetMessageBox(
                        $"Service #{ServiceNo} cannot be added.\n\n" +
                        "Please enter/select the Field Engineer.",
                        "Invalid Field Engineer",
                        clsFunction.IconType.iWarning
                    );

                    txtFEName.Focus();
                    return;
                }

                if (!txtFEName.Text.Trim().Equals(
                        FEName.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                {
                    dbFunction.SetMessageBox(
                        $"Service #{ServiceNo} cannot be added.\n\n" +
                        $"Assigned Field Engineer: {FEName}\n" +
                        $"Selected Field Engineer: {txtFEName.Text}\n\n" +
                        "The selected Field Engineer does not match the assigned Field Engineer.",
                        "Invalid Field Engineer",
                        clsFunction.IconType.iWarning
                    );

                    txtFEName.Focus();
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
                lvi.SubItems.Add(MerchantID);
                lvi.SubItems.Add(JobTypeDescription);
                lvi.SubItems.Add(RequestID);
                lvi.SubItems.Add(Merchant);
                lvi.SubItems.Add(TID);
                lvi.SubItems.Add(MID);
                lvi.SubItems.Add(FEName);                

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
            string pMerchantIDs = getSelectedListView(lvwServiceList, 3);
            string pIRNos = getSelectedListView(lvwServiceList, 5);
            string pReceipts = getSelectedListView(lvwReceiptList, 1);
            string pReceiptsAmount = getSelectedListView(lvwReceiptList, 2);
            string pReceiptsDate = getSelectedListView(lvwReceiptList, 3);            

            txtServiceNoList.Text = pServiceNos;
            txtIRIDNoList.Text = pIRIDNos;
            txtIRNoList.Text = pIRNos;
            txtMerchantIDList.Text = pMerchantIDs;

            // for receipt
            if (dbFunction.isValidCount(lvwReceiptList.Items.Count))
            {
                int index = 1;
                List<string> pReceiptList = new List<string>();

                foreach (ListViewItem item in lvwReceiptList.Items)
                {
                    string pFileName = genFileName(txtExpenseReferenceNo.Text, dbFunction.getCurrentDate(), index);

                    pReceiptList.Add(pFileName);

                    index++;
                }

                txtReceiptList.Text = string.Join(",", pReceiptList);
                txtReceiptDateList.Text = pReceiptsDate;
                txtReceiptAmountList.Text = pReceiptsAmount;
            }
            else
            {
                txtReceiptList.Clear();
            }

            cboMerchant.Items.Clear();            
            if (dbFunction.isValidCount(lvwServiceList.Items.Count))
            {
                fillMerchant();
            }
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

            return string.Join(",", selectedList);
        }

        private void FillExpenseList()
        {
            int i = 0;
            int iLineNo = 0;

            lvwExpenseList.Items.Clear();
            txtTotalExpenses.Text = "0.00";

            dbAPI.ExecuteAPI("GET", "View", "Expenses Transaction Detail", dbFunction.CheckAndSetNumericValue(txtExpensesNo.Text), "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound()) return;

            if (clsArray.ID == null || clsArray.detail_info == null) return;

            while (clsArray.ID.Length > i)
            {
                iLineNo++;

                string pExpenseID = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ExpensesID");
                
                string pExpenseType = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ExpenseType");
                string pExpenseDate = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ExpensesDate");
                string pLocationFrom = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "LocationFrom");
                string pLocationTo = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "LocationTo");
                string pAmount = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ExpensesAmount");
                string pRemarks = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ExpensesDescription");
                string pExpenseReferenceNo = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ExpensesReferenceNo");
                string pMerchantName = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "MerchantName");
                string pReceiptFileName = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ReceiptFileName");

                decimal dExpenseAmount;

                if (!decimal.TryParse(pAmount, out dExpenseAmount))
                {
                    dExpenseAmount = 0;
                }

                ListViewItem item = new ListViewItem(iLineNo.ToString());

                item.SubItems.Add(pExpenseID);
                item.SubItems.Add(pExpenseType);
                item.SubItems.Add(pExpenseDate);
                item.SubItems.Add(pLocationFrom);
                item.SubItems.Add(pLocationTo);
                item.SubItems.Add(dExpenseAmount.ToString());
                item.SubItems.Add(pRemarks);                
                item.SubItems.Add(pMerchantName);
                item.SubItems.Add(pReceiptFileName);

                item.Tag = clsArray.detail_info[i];

                lvwExpenseList.Items.Add(item);

                i++;
            }

            txtTotalExpenses.Text = $"{ComputeTotalAmount(lvwExpenseList, 6)}";
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            // Reference No
            if (!dbFunction.isValidDescription(txtExpenseReferenceNo.Text))
            {
                dbFunction.SetMessageBox("Reference number must not be blank.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);
                return;
            }

            if (!dbFunction.fPromptConfirmation("Are you sure want to preview expenses report?"))
            {
                return;
            }

            if (!dbFunction.isValidID(txtExpensesNo.Text))
            {
                dbFunction.SetMessageBox(
                    "Please select a valid service first.",
                    "Generate expense report",
                    clsFunction.IconType.iWarning
                );

                return;
            }

            clsReport.ClassReportDesc = "EXPENSES-FSR REPORT";

            clsSearch.ClassReportID = 61;
            clsSearch.ClassReportDescription = clsReport.ClassReportDesc;

            clsSearch.ClassStatementType = "View";
            clsSearch.ClassSearchBy = "Expenses-Trans-Report";

            clsSearch.ClassSearchValue = dbFunction.CheckAndSetNumericValue(txtExpensesNo.Text);

            clsSearch.ClassStoredProcedureName = "spViewReport";

            dbFunction.ProcessReport(clsSearch.ClassReportID);
        }

        private void cboExpenseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtLocationFrom.Text = txtLocationTo.Text = "";

            if (dbFunction.isValidCount(lvwServiceList.Items.Count))
            {
                cboMerchant.SelectedIndex = 0;
                cboMerchant.Focus();
            }            
        }

        private void cboSearchServiceType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAddExpense_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidDescriptionEntry(cboExpenseType.Text, "Expenses Type" + clsDefines.MUST_NOT_BLANK_MESSAGE))
            {
                cboExpenseType.Focus();
                return;
            }

            if (!dbFunction.isValidDescriptionEntry(txtExpensesRemarks.Text, "Expenses Remarks" + clsDefines.MUST_NOT_BLANK_MESSAGE))
            {
                txtExpensesRemarks.Focus();
                return;
            }

            if (!dbFunction.isValidDescriptionEntry(cboMerchant.Text, "Merchant" + clsDefines.MUST_NOT_BLANK_MESSAGE))
            {
                cboMerchant.Focus();
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
            model.LocationFrom = txtLocationFrom.Text;
            model.LocationTo = txtLocationTo.Text;

            model.Merchant = cboMerchant.Text;
            model.ReceiptFileName = txtReceiptFileName.Text;

            addExpensesToExpensesList(model);

            txtTotalExpenses.Text = $"{ComputeTotalAmount(lvwExpenseList, 6)}";

            // clear entry
            initExpensesEntry();

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
                string existingExpensesID = item.SubItems[1].Text.Trim();
                string existingExpensesType = item.SubItems[2].Text.Trim();
                string existingExpensesDate = item.SubItems[3].Text.Trim();
                string existingLocationFrom = item.SubItems[4].Text.Trim();
                string existingLocationTo = item.SubItems[5].Text.Trim();
                string existingAmount = item.SubItems[6].Text.Trim();
                string existingRemarks = item.SubItems[7].Text.Trim();
                string existingMerchant = item.SubItems[8].Text.Trim();
                string existingReceiptFileName = item.SubItems[9].Text.Trim();

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

                bool sameMerchant =
                    existingMerchant.Equals(
                        pModel.Merchant?.Trim() ?? "",
                        StringComparison.OrdinalIgnoreCase
                    );

                bool sameReceiptFileName =
                    existingReceiptFileName.Equals(
                        pModel.Merchant?.Trim() ?? "",
                        StringComparison.OrdinalIgnoreCase
                    );


                // ---------------------------------------------------------
                // Exact duplicate
                // ---------------------------------------------------------
                if (sameExpensesID &&
                    sameExpensesDate &&
                    sameAmount &&
                    sameRemarks &&
                    sameMerchant &&
                    sameReceiptFileName)
                {
                    dbFunction.SetMessageBox(
                        "This expense entry already exists in the list.\n\n" +
                        "Expense Type: " + pModel.ExpensesType + "\n" +
                        "Merchant: " + pModel.Merchant + "\n" +
                        "Amount: ₱" + pModel.Amount + "\n" +
                        "Date: " + pModel.ExpensesDate + "\n" +
                        "Receipt: " + pModel.ReceiptFileName,
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

            // Location From
            lvi.SubItems.Add(pModel.LocationFrom ?? "");

            // Location To
            lvi.SubItems.Add(pModel.LocationTo ?? "");

            // Amount
            lvi.SubItems.Add(pModel.Amount.ToString());

            // Remarks
            lvi.SubItems.Add(pModel.Remarks ?? "");

            // Merchant
            lvi.SubItems.Add(pModel.Merchant ?? "");

            // Receipt FileName
            lvi.SubItems.Add(pModel.ReceiptFileName ?? "");

            // -------------------------------------------------------------
            // Store model for Edit / Save / Delete
            // -------------------------------------------------------------
            lvi.Tag = pModel;

            lvwExpenseList.Items.Add(lvi);
        }

        private void btnServiceRemove_Click(object sender, EventArgs e)
        {
            dbFunction.removeItemListView(lvwServiceList, false);

            displayList();
        }

        private void btnServiceClearAll_Click(object sender, EventArgs e)
        {
            dbFunction.ClearListViewItems(lvwServiceList);

            displayList();

            txtServiceNoList.Text = txtIRIDNoList.Text = txtIRNoList.Text = "";
            btnSearchServiceNos.Focus();
        }

        private void btnExpenseRemove_Click(object sender, EventArgs e)
        {
            dbFunction.removeItemListView(lvwExpenseList, false);

            txtTotalExpenses.Text = $"{ComputeTotalAmount(lvwExpenseList, 6)}";
        }

        private void btnExpenseClearAll_Click(object sender, EventArgs e)
        {
            dbFunction.ClearListViewItems(lvwExpenseList);

            initExpensesEntry();

            txtTotalExpenses.Text = $"{ComputeTotalAmount(lvwExpenseList, 6)}";
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

            modelExpensesDetail modelDetail = item.Tag as modelExpensesDetail;

            if (modelDetail == null)
                return;


            // -------------------------------------------------------------
            // Load selected expense to entry fields
            // -------------------------------------------------------------

            // Expense Type
            cboExpenseType.Text = modelDetail.ExpensesType ?? "";

            // Expense Amount
            txtExpenseAmount.Text = modelDetail.Amount.ToString();

            // Remarks
            txtExpensesRemarks.Text = modelDetail.Remarks ?? "";

            // Location From
            txtLocationFrom.Text = modelDetail.LocationFrom;

            // Location To
            txtLocationTo.Text = modelDetail.LocationTo;
        }

        private bool ValidateFields()
        {
            bool isValid = true;

            // Service List
            if (!dbFunction.isValidCount(lvwServiceList.Items.Count))
            {
                dbFunction.SetMessageBox("Please select at least one service.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);

                isValid = false;
            }

            // Receipt List            
            if (!dbFunction.isValidCount(lvwReceiptList.Items.Count))
            {
                dbFunction.SetMessageBox("Please select at least one receipt.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);

                isValid = false;
            }

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

            // Expenses Amount
            if (!dbFunction.isValidAmount(txtTotalExpenses.Text))
            {
                dbFunction.SetMessageBox("Expenses total amount must not be blank.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);

                isValid = false;
            }

            return isValid;
        }

        private decimal ComputeTotalAmount(ListView lvw, int colIndex)
        {
            decimal dTotal = 0M;
            int amtColIndex = 6;

            foreach (ListViewItem item in lvw.Items)
            {
                if (item.SubItems.Count <= colIndex)
                    continue;

                decimal dAmount = 0M;

                decimal.TryParse(
                    item.SubItems[colIndex].Text.Trim(),
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out dAmount
                );

                dTotal += dAmount;
            }

            return dTotal;
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
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
                openFile.Title = "Select Receipt Images";

                openFile.Filter =
                    "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|" +
                    "PNG Images (*.png)|*.png|" +
                    "JPEG Images (*.jpg;*.jpeg)|*.jpg;*.jpeg";

                openFile.Multiselect = true;

                if (openFile.ShowDialog(this) != DialogResult.OK) return;

                Cursor.Current = Cursors.WaitCursor;

                try
                {
                    foreach (string pSelectedReceipt in openFile.FileNames)
                    {
                        if (!File.Exists(pSelectedReceipt))
                        {
                            dbFunction.SetMessageBox(
                                "Receipt is not found.",
                                clsDefines.FIELD_CHECK_MSG,
                                clsFunction.IconType.iWarning
                            );

                            continue;
                        }

                        FileInfo fileInfo = new FileInfo(pSelectedReceipt);

                        int iLineNo = lvwReceiptList.Items.Count + 1;

                        ListViewItem item = new ListViewItem(iLineNo.ToString());

                        item.SubItems.Add(fileInfo.Name);

                        // init
                        item.SubItems.Add(clsDefines.gNull);
                        item.SubItems.Add(clsDefines.gNull);

                        item.Tag = fileInfo.FullName;

                        lvwReceiptList.Items.Add(item);

                        dbFunction.RefreshCountListView(txtTReceiptCount, lvwReceiptList);

                        try
                        {
                            JObject pOCRResult = receiptOCR(fileInfo.FullName);

                            decimal? dReceiptAmount = pOCRResult.Value<decimal?>("Amount");
                            string pReceiptDate = pOCRResult.Value<string>("ReceiptDate");

                            item.SubItems[2].Text = dReceiptAmount.Value.ToString();

                            DateTime dReceiptDate;

                            if (DateTime.TryParse(pReceiptDate, out dReceiptDate))
                            {
                                item.SubItems[3].Text = dReceiptDate.ToString("MM-dd-yyyy");
                            }
                            else
                            {
                                item.SubItems[3].Text = "0000-00-00";
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(
                                "Receipt OCR failed: " +
                                fileInfo.Name + "\n" + ex
                            );
                        }

                        // Select the last added receipt - preview
                        lvwReceiptList.SelectedItems.Clear();
                        item.Selected = true;
                        item.Focused = true;
                        item.EnsureVisible();
                        lvwReceiptList_SelectedIndexChanged(this, e);

                    }
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }

                // Compute Amount
                txtTotalReceptAmount.Text = $"{ComputeTotalAmount(lvwReceiptList, 2)}";

                dbFunction.RefreshCountListView(txtTReceiptCount, lvwReceiptList);                
            }

            displayList();
        }

        private void btnReceiptEdit_Click(object sender, EventArgs e)
        {

        }

        private void btnReceiptDelete_Click(object sender, EventArgs e)
        {
            dbFunction.removeItemListView(lvwReceiptList, false);
            dbFunction.RefreshCountListView(txtTReceiptCount, lvwReceiptList);

            // Compute Amount
            txtTotalReceptAmount.Text = $"{ComputeTotalAmount(lvwReceiptList, 2)}";
            pbReceiptPreview.Image = null;
        }

        private void initExpensesEntry()
        {
            txtLocationFrom.Text = txtLocationTo.Text = "";
            cboExpenseType.SelectedIndex = 0;
            txtExpenseAmount.Text = "0.00";
            txtExpensesRemarks.Text = "";
            chkExpensensRemarks.Checked = false;

            InitDate();

        }

        private void btnSearchFE_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iFE;
            frmSearchField.sHeader = "FIELD ENGINEER";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                txtFEID.Text = clsSearch.ClassParticularID.ToString();
                txtFEName.Text = clsSearch.ClassParticularName;

                clsSearch.ClassFEID = int.Parse(dbFunction.CheckAndSetNumericValue(txtFEID.Text));

                btnSearchServiceNos.Enabled = true;
                dbFunction.SetButtonIconImage(btnSearchServiceNos);

            }
        }

        private void chkExpensensRemarks_CheckedChanged(object sender, EventArgs e)
        {
            if (chkExpensensRemarks.Checked)
            {
                txtExpensesRemarks.Text = "N/A";
            }
            else
            {
                txtExpensesRemarks.Text = "";
                txtExpensesRemarks.Focus();
            }
        }

        private void txtLocationFrom_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    txtLocationTo.Focus();
                    break;
            }
        }

        private void txtLocationTo_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    txtExpenseAmount.Text = "0.00";
                    txtExpenseAmount.SelectAll();
                    txtExpenseAmount.Focus();
                    break;
            }
        }

        private JObject receiptOCR(string pImagePath)
        {
            JObject pOCRResult = new JObject
            {
                ["Amount"] = 0,
                ["ReceiptDate"] = "0000-00-00"
            };

            string pOCRText = dbReceiptImageProcessor.ExtractText(pImagePath);

            decimal? dReceiptAmount = dbReceiptImageProcessor.ExtractTransactionAmount(pOCRText);

            string pReceiptDate = dbReceiptImageProcessor.ExtractTransactionDate(pOCRText);

            // Default Amount = 0 if OCR cannot extract amount
            pOCRResult["Amount"] = dReceiptAmount ?? 0;

            // Default Date = 0000-00-00 if OCR cannot extract date
            pOCRResult["ReceiptDate"] = string.IsNullOrWhiteSpace(pReceiptDate) ? "0000-00-00" : pReceiptDate;

            return pOCRResult;
        }

        private void lvwReceiptList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwReceiptList.SelectedItems.Count == 0)
                return;

            ListViewItem item = lvwReceiptList.SelectedItems[0];

            if (item.Tag == null)
                return;

            Image pPreviousImage = pbReceiptPreview.Image;

            try
            {
                pbReceiptPreview.Image = dbReceiptImageProcessor.CreatePreview(
                    item.Tag.ToString(),
                    ReceiptPreviewMode.Original
                );

                pbReceiptPreview.SizeMode = PictureBoxSizeMode.Zoom;
            }
            finally
            {
                if (pPreviousImage != null)
                    pPreviousImage.Dispose();
            }
        }

        private void uploadReceiptFTP(string pExpensesReferenceNo)
        {
            ftp ftpClient = new ftp(pExpenseFTPHost, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);

            int ImageCount = 1;

            try
            {
                int iTotalImages = lvwReceiptList.Items.Count;
                JArray pReceiptList = new JArray();

                foreach (ListViewItem item in lvwReceiptList.Items)
                {
                    if (item.Tag == null)
                    {
                        dbFunction.SetMessageBox(
                            "The selected receipt contains invalid image information.",
                            "Receipt upload failed",
                            clsFunction.IconType.iError
                        );

                        return;
                    }

                    string pImageSource = item.Tag.ToString();

                    if (!File.Exists(pImageSource))
                    {
                        dbFunction.SetMessageBox(
                            "The receipt image could not be found.\n\n" +
                            "File: " + pImageSource,
                            "Receipt upload failed",
                            clsFunction.IconType.iError
                        );

                        return;
                    }

                    string pFileName = genFileName(txtExpenseReferenceNo.Text, dbFunction.getCurrentDate(), ImageCount);
                    pReceiptList.Add(pFileName);

                    Debug.WriteLine("FTP host: " + pExpenseFTPHost);
                    Debug.WriteLine("FTP filename: " + pFileName);

                    ftpClient.upload(pFileName, pImageSource);

                    long pLocalFileSize = new FileInfo(pImageSource).Length;
                    long pUploadedFileSize = ftpClient.getFileSize(pFileName);

                    if (pUploadedFileSize <= 0 || pUploadedFileSize != pLocalFileSize)
                    {
                        dbFunction.SetMessageBox(
                            "The receipt image could not be verified on FTP.\n\n" +
                            "File: " + pFileName,
                            "Receipt upload failed",
                            clsFunction.IconType.iError
                        );

                        return;
                    }

                    ImageCount++;
                }

                txtReceiptList.Text = pReceiptList.ToString(Formatting.None);
                return;
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox(
                    "The expense was saved, but its receipt images could not be uploaded.\n\n" +
                    ex.Message,
                    "Receipt upload failed",
                    clsFunction.IconType.iError
                );

                return;
            }
            finally
            {
                ftpClient.disconnect();
            }
        }

        private string genFileName(string pExpensesReferenceNo, string pExpenseDate, int pImageCount)
        {
            return pExpensesReferenceNo + "_" +
                   pExpenseDate.Replace("-", "") + "_" +
                   pImageCount.ToString("00") +
                   clsDefines.FILE_EXT_PNG;
        }

        private void btnReceiptClearAll_Click(object sender, EventArgs e)
        {
            dbFunction.ClearListViewItems(lvwReceiptList);
            dbFunction.RefreshCountListView(txtTReceiptCount, lvwReceiptList);

            // Compute Amount
            txtTotalReceptAmount.Text = $"{ComputeTotalAmount(lvwReceiptList, 2)}";
            pbReceiptPreview.Image = null;
        }

        private void lvwServiceList_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    btnServiceRemove_Click(this, e);
                    break;
            }
        }

        private void lvwReceiptList_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    btnReceiptDelete_Click(this, e);
                    break;
            }
        }

        private void lvwExpenseList_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    btnExpenseRemove_Click(this, e);
                    break;
            }
        }

        private void btnGenerateReceiptReport_Click(object sender, EventArgs e)
        {
            // Reference No
            if (!dbFunction.isValidDescription(txtExpenseReferenceNo.Text))
            {
                dbFunction.SetMessageBox("Reference number must not be blank.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);
                return;
            }

            if (!dbFunction.fPromptConfirmation("Are you sure want to preview expenses receipt?"))
            {
                return;
            }

            // download receipt
            downloadReceipt();

            clsReport.ClassReportDesc = "OPERATIONS RECEIPT REPORT";

            clsSearch.ClassReportID = 62;
            clsSearch.ClassReportDescription = clsReport.ClassReportDesc;

            clsSearch.ClassStatementType = "View";
            clsSearch.ClassSearchBy = "Expenses-Receipt-Report";

            clsSearch.ClassSearchValue = dbFunction.CheckAndSetNumericValue(txtExpensesNo.Text);

            clsSearch.ClassStoredProcedureName = "spViewReport";

            dbFunction.ProcessReport(clsSearch.ClassReportID);

        }

        private void addReceiptToReceiptList(modelExpensesReceipt pModel)
        {
            if (pModel == null)
                return;

            int lineNo = lvwReceiptList.Items.Count + 1;

            ListViewItem lvw = new ListViewItem(lineNo.ToString());

            lvw.Tag = pModel.ExpensesFileName;

            lvw.SubItems.Add(pModel.ExpensesFileName);
            lvw.SubItems.Add(pModel.ExpensesAmount.ToString("0.00"));
            lvw.SubItems.Add(pModel.ExpensesDate);

            lvwReceiptList.Items.Add(lvw);
        }

        private void fillReceiptList()
        { 

        }

        private void btnGenerateFSRReport_Click(object sender, EventArgs e)
        {
            // Reference No
            if (!dbFunction.isValidDescription(txtExpenseReferenceNo.Text))
            {
                dbFunction.SetMessageBox("Reference number must not be blank.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);
                return;
            }

            if (!dbFunction.fPromptConfirmation("Are you sure want to download all FSR related to this expense."))
            {
                return;
            }

            Cursor.Current = Cursors.WaitCursor;

            int pCount = 0;

            string pExportPath = getExportPath();

            dbFile.CheckFolder(pExportPath);

            ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL +
                "/fsr/" +
                clsSearch.ClassBankCode,
                clsGlobalVariables.strFTPUserName,
                clsGlobalVariables.strFTPPassword
            );

            try
            {
                List<string> pServiceNoList = dbFunction.ParseCSVtoArray(txtServiceNoList.Text);
                foreach (string pServiceNo in pServiceNoList)
                {
                    string pFileName = pServiceNo +
                        clsDefines.FSR_FILENAME_PREFIX +
                        clsDefines.FILE_EXT_PDF;

                    string pLocalPath = Path.Combine(pExportPath, pFileName);

                    ftpClient.download(pFileName, pLocalPath);

                    if (File.Exists(pLocalPath) && new FileInfo(pLocalPath).Length > 0)
                    {
                        pCount++;
                    }
                    else
                    {
                        Debug.WriteLine(
                            "FSR PDF not found: " +
                            pFileName
                        );
                    }
                }

                dbFunction.SetMessageBox(
                    pCount +
                    " FSR report(s) downloaded.\n\n" +
                    "Location: " +
                    pExportPath,
                    "Download FSR reports",
                    clsFunction.IconType.iInformation
                );
            }
            finally
            {
                ftpClient.disconnect();
                Cursor.Current = Cursors.Default;
            }
        }

        private void initReportButton(bool isEnable)
        {
            btnGenerateReport.Enabled = isEnable;
            btnGenerateReceiptReport.Enabled = isEnable;
            btnGenerateFSRReport.Enabled = isEnable;
        }

        private void btnAddToExpenses_Click(object sender, EventArgs e)
        {
            if (lvwReceiptList.SelectedItems.Count == 0)
            {
                dbFunction.SetMessageBox(
                    "Please select a receipt first.",
                    "Receipt",
                    clsFunction.IconType.iWarning
                );

                return;
            }

            ListViewItem item = lvwReceiptList.SelectedItems[0];

            // =========================================
            // RECEIPT DATE
            // =========================================
            string pDate = item.SubItems[3].Text.Trim();

            DateTime dDate;

            if (DateTime.TryParse(pDate, out dDate))
            {
                dtExpenseDate.Value = dDate;
            }

            // =========================================
            // RECEIPT AMOUNT
            // =========================================
            string pAmount = item.SubItems[2].Text.Trim();

            decimal dAmount;

            if (decimal.TryParse(
                pAmount,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out dAmount))
            {
                txtExpenseAmount.Text = dAmount.ToString("0.00");
            }
            else
            {
                txtExpenseAmount.Text = "0.00";
            }

            // generate filename
            int selectedIndex = lvwReceiptList.SelectedIndices[0] + 1;

            txtReceiptFileName.Text = genFileName(txtExpenseReferenceNo.Text,dbFunction.getCurrentDate(),selectedIndex);
        }

        private void fillMerchant()
        {
            cboMerchant.Items.Clear();

            // First value
            cboMerchant.Items.Add(clsDefines.NOT_SPECIFIED);

            foreach (ListViewItem item in lvwServiceList.Items)
            {
                if (item.SubItems.Count <= 6)
                    continue;

                string pMerchant = item.SubItems[6].Text.Trim();

                if (string.IsNullOrEmpty(pMerchant))
                    continue;

                if (!cboMerchant.Items.Contains(pMerchant))
                {
                    cboMerchant.Items.Add(pMerchant);
                }
            }

            cboMerchant.SelectedIndex = 0;
        }

        private string getExportPath()
        {
            string pExportPath = dbFile.sExportPath + "EXPENSES\\" + txtExpenseReferenceNo.Text;

            return pExportPath;
        }

        private void downloadReceipt()
        {
            int pCount = 0;

            Cursor.Current = Cursors.WaitCursor;

            string pExportPath = getExportPath();

            dbFile.CheckFolder(pExportPath);

            try
            {
                ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL +
                                    clsGlobalVariables.strFTPUploadPath + "/expenses/" +
                                    clsSearch.ClassBankCode,
                                    clsGlobalVariables.strFTPUserName,
                                    clsGlobalVariables.strFTPPassword
                                );

                List<string> pReceiptList = dbFunction.ParseCSVtoArray(txtReceiptList.Text);
                foreach (string pReceipt in pReceiptList)
                {
                    string pFileName = pReceipt;

                    string pLocalPath = Path.Combine(pExportPath, pFileName);

                    ftpClient.download(pFileName, pLocalPath);

                    if (File.Exists(pLocalPath) && new FileInfo(pLocalPath).Length > 0)
                    {
                        pCount++;
                    }
                    else
                    {
                        Debug.WriteLine(
                            $"Receipt [{pFileName}] not found: " +
                            pFileName
                        );
                    }
                }

                ftpClient.disconnect();

            }
            catch (Exception ex)
            {
                dbFunction.SetExceptionMessageBox(ex);
            }

            Cursor.Current = Cursors.Default;
        }
    }
}
