using MIS.Enums;
using MIS.Function;
using MIS.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using static MIS.Function.AppUtilities;

namespace MIS
{
    public partial class frmImportIR : Form
    {
        private int iHeaderRowIndex = 1;
        private int iDataRowIndex = 0;

        public static int iTab;
        private clsAPI dbAPI;
        private clsINI dbSetting;
        private clsFile dbFile;
        private clsFunction dbFunction;
        private static string ExcelFilePath = @"";
        private string sExcelFileName = "";
        private string sSheet = "";
        bool fEdit = false;        
        int iLimit = 255;
        bool fAddTerminal = false;

        bool isUpdate = true;
        int delay = 5; // 500 default
        int batchSize = 1000; // 500 default

        private modelParticular modelParticular;

        public class jsonObj
        {
            public object profile_info { get; set; }
            public object rawdata_info { get; set; }
        }

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

        public frmImportIR()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwDetail, true);
            dbFunction.setDoubleBuffer(lvwMerchant, true);            
            dbFunction.setDoubleBuffer(lvwSearch, true);
            
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isAdd, clsUser.ClassUserID, 23)) return;

            OpenFileDialog ExcelDialog = new OpenFileDialog();
            ExcelDialog.Filter = "Excel Files|*.xls;*.xlsx";
            ExcelDialog.InitialDirectory = @"C:\CASTLESTECH_MIS\IMPORT\";
            ExcelDialog.Title = "Select file to import...";

            if (ExcelDialog.ShowDialog() == DialogResult.OK)
            {
                ExcelFilePath = ExcelDialog.FileName;
                txtPathFileName.Text = ExcelDialog.FileName;
                txtPathFileName.ReadOnly = true;
                //txtPathFileName.Click -= btnImport_Click;
                btnLoadFile.Enabled = false;
                sExcelFileName = Path.GetFileName(txtPathFileName.Text);
                
                if (clsSystemSetting.ClassSystemImportCheck > 0)
                {

                    if (dbAPI.isImportFileName("Search", "IR Import", sExcelFileName))
                    {
                        dbFunction.SetMessageBox("Import failure:" + "\n" +
                                                 sExcelFileName + " was already processed.", "Import IR", clsFunction.IconType.iWarning);

                        btnCancel_Click(this, e);
                        return;
                    }
                }

                try
                {
                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                    ucStatusDisplay.SetStatus("Preparing import file" , Enums.StatusType.Processing);
                    Task.Delay(delay); // Asynchronously wait without blocking UI

                    InitImportDate();
                    InitProcessedBy();

                    sSheet = dbFunction.getSheetName(txtPathFileName.Text);

                    if (!dbFunction.isValidSheetName(sSheet)) return;
                    
                    txtSheetName.Text = "`" + sSheet + "$" + "`";
                    txtFileName.Text = sExcelFileName;

                    // Create Temporary database                
                    dbAPI.ExecuteAPI("POST", "Create", "", "", txtSheetName.Text, "", "CreateTempTable");

                    dbFunction.ClearDataGrid(grdDummy);
                    dbFunction.ImportToDummyDataGrid(grdDummy, sSheet, txtPathFileName.Text);

                    Cursor.Current = Cursors.Default; // Back to normal 
                    btnLoadFile.Enabled = false;

                    ucStatusDisplay.SetStatus("Preparing import file complete.", Enums.StatusType.Processing);
                    Task.Delay(delay); // Asynchronously wait without blocking UI

                    // Import
                    if (!fContinueConfirm())
                    {
                        btnCancel_Click(this, e);
                        return;
                    }

                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                    ucStatusDisplay.SetStatus("Processing import file", Enums.StatusType.Processing);
                    Task.Delay(delay); // Asynchronously wait without blocking UI

                    // Drop table create from sheetname
                    if (txtSheetName.Text.Length > 0)
                        dbAPI.ExecuteAPI("DELETE", "Delete", "", txtSheetName.Text.Replace("`", ""), "Drop Temp Table", "", "DeleteCollectionDetail");

                    //GetHeaderList();

                    ucStatusDisplay.SetStatus("Validating import file", Enums.StatusType.Processing);
                    Task.Delay(delay); // Asynchronously wait without blocking UI

                    btnSave.Enabled = false;
                    btnUpdateRawData.Enabled = false;
                    btnUpdateListRawData.Enabled = false;

                    if (!isValidHeader()) return; // Check Header                    
                    
                    if (!ImportToDataGrid()) return;

                    // Pupulate grdBulk
                    populateBulkDataGrid(grdList, grdBulk);
                    DataGridViewHelper.AddCheckBoxColumn(grdBulk);

                    // Display total
                    TotalCount();

                    btnSave.Enabled = true;
                    btnValidate.Enabled = true;
                    btnUpdateListRawData.Enabled = true;

                    Cursor.Current = Cursors.Default; // Back to normal 

                    dbFunction.SetMessageBox("Import of file " +
                                             "\n" +
                                             "[ " + txtFileName.Text + " ]" + " " + "is complete." +
                                             "\n\n" +
                                             "You may save import now."
                                             , "Import IR", clsFunction.IconType.iInformation);

                    //dbFunction.ClearComboBox(this);
                    dbAPI.FillComboBoxClient(cboSearchClient);
                    dbFunction.ComBoBoxUnLock(true, this);
                    dbFunction.TextBoxUnLock(true, this);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception error " + ex.Message);
                    MessageBox.Show(ex.Message);
                    btnLoadFile.Enabled = true;
                    txtFileName.Text = "";
                    txtPathFileName.Text = "";
                    return;
                }
            }
        }
        private string GetExcelWorkSheetName()
        {
            string sWorkSheetName = "";

            // Create Excel WorkSheet
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(txtPathFileName.Text);
            Worksheet worksheet = workbook.Worksheets[0];

            sWorkSheetName = worksheet.Name;

            if (sWorkSheetName.Length <= 0)
                sWorkSheetName = "Sheet1";

            return sWorkSheetName;
        }

        bool fContinueConfirm()
        {
            bool fContinue = true;

            if (MessageBox.Show(clsDefines.TAKE_FEW_MINUTE_MSG +
                                    "\n\n",
                                    "Continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fContinue = false;
            }

            return fContinue;
        }

        private void ImportToDummyDataGrid()
        {
            string pathName = txtPathFileName.Text;
            DataTable tbContainer = new DataTable();
            string strConn = string.Empty;
            string sheetName = sSheet;

            try
            {
                FileInfo file = new FileInfo(txtPathFileName.Text);
                if (!file.Exists) { throw new Exception("Error, file doesn't exists!"); }
                string extension = file.Extension;
                switch (extension.ToLower())
                {
                    case ".xls":
                        if (clsSystemSetting.ClassSystemMSOffice.CompareTo("2013") == 0)
                            strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathName + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
                        else
                            strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Jet OLEDB:Engine Type=5;Extended Properties=\"Excel 8.0;\"";
                        break;
                    case ".xlsx":
                        if (clsSystemSetting.ClassSystemMSOffice.CompareTo("2013") == 0)
                            strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathName + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
                        else
                            strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Jet OLEDB:Engine Type=5;Extended Properties=\"Excel 8.0;\"";
                        break;
                    default:
                        strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                        break;
                }

                OleDbConnection cnnxls = new OleDbConnection(strConn);
                OleDbDataAdapter oda = new OleDbDataAdapter(string.Format("select * from [{0}$]", sheetName), cnnxls);
                oda.Fill(tbContainer);

                grdDummy.DataSource = tbContainer;
                //MessageBox.Show("Excel file import complete.");
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox("Office Version: " +
                                         clsSystemSetting.ClassSystemMSOffice + "\n\n" +
                                         ex.ToString(), ex.Message, clsFunction.IconType.iError);
            }
        }
        private bool ImportToDataGrid()
        {
            int iRowCount = grdDummy.RowCount;
            int iColCount = grdDummy.ColumnCount;
            bool isValid = true;

            Debug.WriteLine("--Optimized ImportToDataGrid--");

            dbFunction.InitDataGridView(grdList);
            grdList.SuspendLayout(); // Suspend UI updates

            // Prepare DataTable for bulk insertion
            DataTable dt = new DataTable();

            // Define columns
            for (int i = 0; i < iColCount; i++)
            {
                string sHeader = grdDummy.Columns[i].Name.Replace("_", " ").Replace("\n", " ").Trim();
                dt.Columns.Add(sHeader);
            }

            // Pre-calculate column indexes to avoid redundant lookups
            int iIRNo = dbFunction.GetMapColumnIndex(clsDefines.IR_REQUEST_ID);
            int iPRIME_TID_01 = dbFunction.GetMapColumnIndex(clsDefines.IR_TID);
            int iPRIME_MID_01 = dbFunction.GetMapColumnIndex(clsDefines.IR_MID);
            int iRequestDate = dbFunction.GetMapColumnIndex(clsDefines.IR_REQUEST_DATE);
            int iInstDdate = dbFunction.GetMapColumnIndex(clsDefines.IR_TARGET_INSTALLATION_DATE);
            int iDBAName = dbFunction.GetMapColumnIndex(clsDefines.IR_MERCHANT_NAME);
            int iRegion = dbFunction.GetMapColumnIndex(clsDefines.IR_AREA1);
            int iProvince = dbFunction.GetMapColumnIndex(clsDefines.IR_CITY);
            int iAddress = dbFunction.GetMapColumnIndex(clsDefines.IR_ADDRESS);


            // Bulk processing
            for (int i = iDataRowIndex; i < iRowCount; i++)
            {
                DataRow row = dt.NewRow();

                string sIRNo = grdDummy.Rows[i].Cells[dbFunction.GetMapColumnIndex(clsDefines.IR_REQUEST_ID)].Value.ToString();
                string sMerchantName = StrClean(grdDummy.Rows[i].Cells[dbFunction.GetMapColumnIndex(clsDefines.IR_MERCHANT_NAME)].Value.ToString());
                string sTID = grdDummy.Rows[i].Cells[dbFunction.GetMapColumnIndex(clsDefines.IR_TID)].Value.ToString();
                string sMID = grdDummy.Rows[i].Cells[dbFunction.GetMapColumnIndex(clsDefines.IR_MID)].Value.ToString();
                string sRequestDate = grdDummy.Rows[i].Cells[dbFunction.GetMapColumnIndex(clsDefines.IR_REQUEST_DATE)].Value.ToString();
                string sInstallationDate = grdDummy.Rows[i].Cells[dbFunction.GetMapColumnIndex(clsDefines.IR_TARGET_INSTALLATION_DATE)].Value.ToString();
                string sRegion = grdDummy.Rows[i].Cells[dbFunction.GetMapColumnIndex(clsDefines.IR_AREA1)].Value.ToString();
                string sProvince = grdDummy.Rows[i].Cells[dbFunction.GetMapColumnIndex(clsDefines.IR_CITY)].Value.ToString();
                string sAddress = StrClean(grdDummy.Rows[i].Cells[dbFunction.GetMapColumnIndex(clsDefines.IR_ADDRESS)].Value.ToString());

                sRequestDate = (dbFunction.isValidDescription(sRequestDate) ? sRequestDate : clsFunction.sDateFormat);
                sInstallationDate = (dbFunction.isValidDescription(sInstallationDate) ? sInstallationDate : clsFunction.sDateFormat);

                Debug.WriteLine("------------------------------------");
                Debug.WriteLine($"Row:[{i}]->ReqquestID:[{sIRNo}],MerchantName:[{sMerchantName}],TID:[{sTID}],MID:[{sMID}],RequestDate:[{sRequestDate}],InstallationDate:[{sInstallationDate}],Region:[{sRegion}],Province:[{sProvince}],Address:[{sAddress}]");
                Debug.WriteLine("------------------------------------");

                for (int x = 0; x < iColCount; x++)
                {
                    string sCellValue = StrClean(grdDummy.Rows[i].Cells[x].Value?.ToString().Trim() ?? "");
                    
                    // Apply transformations only if necessary
                    if (x == iPRIME_TID_01 && !string.IsNullOrEmpty(sCellValue))
                        sCellValue = dbFunction.padLeftChar(sTID, clsFunction.sPadZero, 8);

                    if (x == iPRIME_MID_01 && !string.IsNullOrEmpty(sCellValue))
                        sCellValue = dbFunction.padLeftChar(sMID, clsFunction.sPadZero, 15);

                    if (x == iRequestDate && !string.IsNullOrEmpty(sRequestDate))
                    {
                        sCellValue = dbFunction.GetDateFromParse(sRequestDate.Substring(0, 10), "dd/MM/yyyy", "yyyy-MM-dd");
                        if (!dbFunction.isValidDescription(sCellValue))
                            sCellValue = clsDefines.DEV_DATE;                      
                    }

                    if (x == iInstDdate && !string.IsNullOrEmpty(sInstallationDate))
                    {
                        sCellValue = dbFunction.GetDateFromParse(sInstallationDate.Substring(0, 10), "dd/MM/yyyy", "yyyy-MM-dd");
                        if (!dbFunction.isValidDescription(sCellValue))
                            sCellValue = clsDefines.DEV_DATE;
                    }

                    if (x == iDBAName)
                        sCellValue = sMerchantName.Replace("(", "").Replace(")", "");
                    //sCellValue = sCellValue.Replace("&", "AND").Replace("(", "").Replace(")", "");

                    if (x == iRegion)
                        sCellValue = string.IsNullOrEmpty(sRegion) ? clsFunction.sNone : sRegion;

                    if (x == iProvince)
                        sCellValue = string.IsNullOrEmpty(sProvince) ? clsFunction.sNone : sProvince;

                    if (x == iAddress)
                        sCellValue = string.IsNullOrEmpty(sAddress) ? clsFunction.sNone : sAddress;

                    // Remove special characters
                    //sCellValue = sCellValue.Replace("&", "AND").Replace("|", "").Replace("~", "").Replace("^", "").Replace("%", "");
                    //sCellValue = sCellValue.Replace("|", "").Replace("~", "").Replace("^", "").Replace("%", "").Replace("'", "").Replace("#", "");

                    // Remove unwanted chars (including | automatically)                   
                    sCellValue = StrClean(sCellValue);

                    row[x] = !string.IsNullOrEmpty(sCellValue) ? sCellValue.ToUpper() : clsFunction.sDash;
                }

                dt.Rows.Add(row);

                // Update status every 500 rows to avoid UI lag
                if (i % batchSize == 0 || i == iRowCount - 1)
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        ucStatusDisplay.SetStatus($"Group# {i+1} of {iRowCount} to gridview.", Enums.StatusType.Processing);
                        Task.Delay(delay); // Asynchronously wait without blocking UI

                    }));
                }
            }

            // Set DataSource for DataGridView
            grdList.DataSource = dt;

            // Apply alternating row colors
            //dbFunction.DataGridViewAlternateBackColor(grdList);

            grdList.ResumeLayout(); // Resume UI updates

            return isValid;
        }
        
        /*
        private bool ImportToDataGrid()
        {
            int iRowCount = grdDummy.RowCount;
            int iColCount = grdDummy.ColumnCount;
            bool isValid = true;

            Debug.WriteLine("===== START ImportToDataGrid =====");

            var swTotal = System.Diagnostics.Stopwatch.StartNew();

            dbFunction.InitDataGridView(grdList);
            grdList.SuspendLayout();

            DataTable dt = new DataTable();

            // Columns
            for (int i = 0; i < iColCount; i++)
            {
                dt.Columns.Add(grdDummy.Columns[i].Name);
            }

            // Cache column indexes
            int iIRNo = dbFunction.GetMapColumnIndex(clsDefines.IR_REQUEST_ID);
            int iPRIME_TID_01 = dbFunction.GetMapColumnIndex(clsDefines.IR_TID);
            int iPRIME_MID_01 = dbFunction.GetMapColumnIndex(clsDefines.IR_MID);
            int iRequestDate = dbFunction.GetMapColumnIndex(clsDefines.IR_REQUEST_DATE);
            int iInstDdate = dbFunction.GetMapColumnIndex(clsDefines.IR_TARGET_INSTALLATION_DATE);
            int iDBAName = dbFunction.GetMapColumnIndex(clsDefines.IR_MERCHANT_NAME);
            int iRegion = dbFunction.GetMapColumnIndex(clsDefines.IR_AREA1);
            int iProvince = dbFunction.GetMapColumnIndex(clsDefines.IR_CITY);
            int iAddress = dbFunction.GetMapColumnIndex(clsDefines.IR_ADDRESS);

            dt.BeginLoadData();

            try
            {
                for (int i = iDataRowIndex; i < iRowCount; i++)
                {
                    var swRow = System.Diagnostics.Stopwatch.StartNew();

                    DataRow row = dt.NewRow();

                    string sIRNo = grdDummy.Rows[i].Cells[iIRNo].Value?.ToString() ?? "";
                    string sMerchantName = StrClean(grdDummy.Rows[i].Cells[iDBAName].Value?.ToString() ?? "");
                    string sTID = grdDummy.Rows[i].Cells[iPRIME_TID_01].Value?.ToString() ?? "";
                    string sMID = grdDummy.Rows[i].Cells[iPRIME_MID_01].Value?.ToString() ?? "";
                    string sRequestDate = grdDummy.Rows[i].Cells[iRequestDate].Value?.ToString() ?? "";
                    string sInstallationDate = grdDummy.Rows[i].Cells[iInstDdate].Value?.ToString() ?? "";
                    string sRegionVal = grdDummy.Rows[i].Cells[iRegion].Value?.ToString() ?? "";
                    string sProvinceVal = grdDummy.Rows[i].Cells[iProvince].Value?.ToString() ?? "";
                    string sAddressVal = StrClean(grdDummy.Rows[i].Cells[iAddress].Value?.ToString() ?? "");

                    if (sInstallationDate == "0000-00-00") sInstallationDate = "";

                    sRequestDate = dbFunction.isValidDescription(sRequestDate) ? sRequestDate : "";
                    sInstallationDate = dbFunction.isValidDescription(sInstallationDate) ? sInstallationDate : "";

                    for (int x = 0; x < iColCount; x++)
                    {
                        string sCellValue = grdDummy.Rows[i].Cells[x].Value?.ToString() ?? "";

                        // ⚠️ TEMP: comment StrClean for testing performance
                        // sCellValue = StrClean(sCellValue);

                        if (x == iPRIME_TID_01 && !string.IsNullOrEmpty(sTID))
                            sCellValue = dbFunction.padLeftChar(sTID, clsFunction.sPadZero, 8);

                        if (x == iPRIME_MID_01 && !string.IsNullOrEmpty(sMID))
                            sCellValue = dbFunction.padLeftChar(sMID, clsFunction.sPadZero, 15);

                        if (x == iRequestDate && sRequestDate.Length >= 10)
                            sCellValue = dbFunction.GetDateFromParse(
                                sRequestDate.Substring(0, 10),
                                "dd/MM/yyyy",
                                "yyyy-MM-dd"
                            );

                        if (x == iInstDdate && sInstallationDate.Length >= 10)
                            sCellValue = dbFunction.GetDateFromParse(
                                sInstallationDate.Substring(0, 10),
                                "dd/MM/yyyy",
                                "yyyy-MM-dd"
                            );

                        if (x == iDBAName)
                            sCellValue = sMerchantName.Replace("(", "").Replace(")", "");

                        if (x == iRegion)
                            sCellValue = string.IsNullOrEmpty(sRegionVal) ? clsFunction.sNone : sRegionVal;

                        if (x == iProvince)
                            sCellValue = string.IsNullOrEmpty(sProvinceVal) ? clsFunction.sNone : sProvinceVal;

                        if (x == iAddress)
                            sCellValue = string.IsNullOrEmpty(sAddressVal) ? clsFunction.sNone : sAddressVal;

                        // ⚠️ TEMP: reduce heavy operations for testing
                        // sCellValue = sCellValue.ToUpper();

                        row[x] = !string.IsNullOrEmpty(sCellValue)
                            ? sCellValue
                            : clsFunction.sDash;
                    }

                    dt.Rows.Add(row);

                    swRow.Stop();

                    // 🔍 DEBUG: detect slow rows
                    if (swRow.ElapsedMilliseconds > 50)
                    {
                        Debug.WriteLine($"⚠️ Slow row {i}: {swRow.ElapsedMilliseconds} ms");
                    }

                    // 🔍 Progress log
                    if (i % 500 == 0)
                    {
                        Debug.WriteLine($"Processing row {i}/{iRowCount}...");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
                Debug.WriteLine("ERROR FULL: " + ex.ToString());
                return false;
            }

            dt.EndLoadData();

            swTotal.Stop();
            Debug.WriteLine($"===== LOOP DONE in {swTotal.ElapsedMilliseconds} ms =====");

            // Bind
            grdList.DataSource = null;
            grdList.AutoGenerateColumns = true;
            grdList.DataSource = dt;

            grdList.ResumeLayout();
            grdList.Refresh();

            Debug.WriteLine($"===== GRID BOUND: {dt.Rows.Count} rows =====");

            ucStatusDisplay.SetStatus($"Loaded {dt.Rows.Count} rows.", Enums.StatusType.Success);

            return isValid;
        }
        */

        private void ListDetails(int iRow)
        {
            int inLineNo = 0;
            int inGridColCount = grdList.ColumnCount;

            lvwDetail.Items.Clear();
            for (int i = 0; i < inGridColCount; i++)
            {
                inLineNo++;
                string cellParam = grdList.Columns[i].Name; // Param
                string cellValue = grdList.Rows[iRow].Cells[i].Value.ToString(); // Value
                ListViewItem item = new ListViewItem(inLineNo.ToString());
                item.SubItems.Add(cellParam);
                item.SubItems.Add(cellValue);
                lvwDetail.Items.Add(item);

                Debug.WriteLine("ListDetails::" + i.ToString() + "-" + cellParam + "-" + cellValue + "\n");
            }
        }

        private void grdList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var dataIndexNo = grdList.Rows[e.RowIndex].Index.ToString();
                string cellValue = grdList.Rows[e.RowIndex].Cells[1].Value.ToString();

                if (int.Parse(dataIndexNo) >= 0)
                {
                    int pLineNo = int.Parse(dataIndexNo);
                    lblSelectedRow.Text = $"Selected line# {pLineNo+1}/{grdList.Rows.Count}";

                    string rawdata_info = dbFunction.genJSONFormat(grdList, pLineNo, "", clsDefines.NESTED_OBJECT_VALUES);                    
                    dbFunction.populateListViewFromJsonString(dgvRaw, rawdata_info, "", clsDefines.NESTED_OBJECT_VALUES);

                    string profile_info = getProfile_InfoFromJson(rawdata_info, "", clsDefines.NESTED_OBJECT_VALUES);
                    dbFunction.populateListViewFromJsonString(dgvProfile, profile_info, "", "");

                    clsSearch.ClassRawDataInfo = rawdata_info;
                    clsSearch.ClassProfileDataInfo = profile_info;

                    btnUpdateRawData.Enabled = true;

                }
            }
        }
        private void ListViewAlternateBackColor()
        {
            if (lvwDetail.Items.Count > 0)
            {
                for (int i = 0; i <= lvwDetail.Items.Count - 1; i++)
                {
                    if (i % 2 == 0)
                        lvwDetail.Items[i].BackColor = Color.GhostWhite;
                    else
                        lvwDetail.Items[i].BackColor = Color.Azure;
                }
            }
        }
        private void frmInstallationImport_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbSetting = new clsINI();
            dbFile = new clsFile();
            dbFunction = new clsFunction();
            modelParticular = new modelParticular();

            dbSetting.InitDatabaseSetting();

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);

            dbAPI.FillComboBoxTypeByGroup(cboRequestType, (int)GroupType.RequestType);
            dbAPI.FillComboBoxPOSType(cboPOSType);
            
            dbFunction.ClearDataGrid(grdDummy);
            dbFunction.ClearDataGrid(grdList);
            dbFunction.ClearDataGrid(dgvProfile);
            dbFunction.ClearDataGrid(dgvRaw);
            dbFunction.ClearDataGrid(grdBulk);

            InitImportDate();
            InitProcessedBy();
            InitDate();

            btnSave.Enabled = false;
            btnValidate.Enabled = false;
            btnUpdateListRawData.Enabled = false;

            InitTab();
            InitMessageCountLimit();
            InitTextBoxLength();
            InitListView();

            InitStatusTitle(true);

            fEdit = false;
            fAddTerminal = false;

            // Load Mapping
            dbAPI.ExecuteAPI("GET", "View", "Type", "IR", "Mapping", "", "ViewMapping");

            btnMerchantSearch.Enabled = true;
            btnMerchantListSearch.Enabled = false;
            btnClientSearch.Enabled = false;
            btnAddTerminal.Enabled = false;
            btnUpdateRawData.Enabled = false;

            dbFunction.SetButtonIconImage(btnMerchantSearch);
            dbFunction.SetButtonIconImage(btnMerchantListSearch);
            dbFunction.SetButtonIconImage(btnClientSearch);

            dbFunction.initTabSelection(tabTerminal, 1);

            ucStatusDisplay.SetStatus("", Enums.StatusType.Init);
            Task.Delay(delay); // Asynchronously wait without blocking UI
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!dbAPI.isValidSystemVersion()) return;

            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isAdd, clsUser.ClassUserID, 23)) return;

            // check file in use
            if (!dbFunction.checkFileInUse(txtPathFileName.Text)) return;

            // check client
            if (!dbFunction.isValidDescriptionEntry(cboSearchClient.Text, "Client" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            // check ready
            //if (!dbFunction.isValidID(txtReady.Text))
            //{
            //    dbFunction.SetMessageBox(
            //        "No valid records found to import. Please validate your list.",
            //        lblHeader.Text,
            //        clsFunction.IconType.iError
            //    );
            //    return;
            //}

            //// check restricted
            //if (dbFunction.isValidID(txtRestricted.Text))
            //{
            //    dbFunction.SetMessageBox(
            //        "Import blocked: Some items are currently restricted or in use.",
            //        lblHeader.Text,
            //        clsFunction.IconType.iError
            //    );
            //    return;
            //}

            dbFunction.SetMessageBox("This may take a few minutes to validate records on the list.", lblHeader.Text, clsFunction.IconType.iInformation);

            Cursor.Current = Cursors.WaitCursor;

            InitImportDate();
            InitProcessedBy();

            // check for mandatory fields
            ucStatusDisplay.SetStatus($"Checking mandatory fields...", Enums.StatusType.Processing);
            if (!dbFunction.isValidDataGridValue(clsFunction.ImportType.iIRImportDetail, dbAPI, grdList, txtFileName.Text, 0, false)) return;

            // check field column is required
            ucStatusDisplay.SetStatus($"Checking required fields...", Enums.StatusType.Processing);
            if (!dbFunction.isValidDataGridValue(clsFunction.ImportType.iIRImportDetail, dbAPI, grdList, txtFileName.Text, 1, false)) return;

            // check region
            ucStatusDisplay.SetStatus($"Checking required fields...", Enums.StatusType.Processing);
            if (!dbFunction.isValidDataGridValue(clsFunction.ImportType.iIRImportDetail, dbAPI, grdList, txtFileName.Text, 4, false)) return;

            // check valid request id prefix
            ucStatusDisplay.SetStatus($"Checking request id prefix...", Enums.StatusType.Processing);
            if (!dbFunction.isValidDataGridValue(clsFunction.ImportType.iIRImportDetail, dbAPI, grdList, txtFileName.Text, 5, false)) return;

            // check valid request type
            ucStatusDisplay.SetStatus($"Checking request type...", Enums.StatusType.Processing);
            if (!dbFunction.isValidDataGridValue(clsFunction.ImportType.iIRImportDetail, dbAPI, grdList, txtFileName.Text, 8, false)) return;

            // check merchant name/region/province
            //ucStatusDisplay.SetStatus($"Checking record existing...", Enums.StatusType.Processing);
            //if (!dbFunction.isValidDataGridValue(clsFunction.ImportType.iIRImportDetail, dbAPI, grdList, txtFileName.Text, 6, false)) return; // to be uncomment

            // check merchant name/address/region/province are match
            //ucStatusDisplay.SetStatus($"Matching record to server...", Enums.StatusType.Processing);
            //if (!dbFunction.isValidDataGridValue(clsFunction.ImportType.iIRImportDetail, dbAPI, grdList, txtFileName.Text, 7, false)) return; // to be uncomment

            // check request date
            ucStatusDisplay.SetStatus($"Checking request date...", Enums.StatusType.Processing);
            if (!dbFunction.isValidDataGridValue(clsFunction.ImportType.iIRImportDetail, dbAPI, grdList, txtFileName.Text, 2, true)) return;

            // check request id
            ucStatusDisplay.SetStatus($"Checking request id...", Enums.StatusType.Processing);
            if (!dbFunction.isValidDataGridValue(clsFunction.ImportType.iIRImportDetail, dbAPI, grdList, txtFileName.Text, 3, false)) return;

            Cursor.Current = Cursors.Default;

            if (!ValidateFields(2)) return;

            if (!dbFunction.fPromptConfirmation("Cross-check complete.\nProceed with saving the imported records?"))
                return;
            
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                ucStatusDisplay.SetStatus($"Processing region...", Enums.StatusType.Processing);
                InsertRegion();

                ucStatusDisplay.SetStatus($"Processing city...", Enums.StatusType.Processing);
                InsertCity();

                ucStatusDisplay.SetStatus($"Processing province...", Enums.StatusType.Processing);
                InsertProvince();

                ucStatusDisplay.SetStatus($"Processing region province detail...", Enums.StatusType.Processing);
                InsertRegionDetail();

                // ----------------------------------
                // re-create file
                // ----------------------------------
                clsSearch._isWriteResponse = true;
                ucStatusDisplay.SetStatus($"Updating file...", Enums.StatusType.Processing);
                dbAPI.ExecuteAPI("GET", "View", "Region", "", "Region", "", "ViewRegion");
                string pSearchValue = clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero;
                dbAPI.ExecuteAPI("GET", "View", "Province", pSearchValue, "Province", "", "ViewRegionDetail");
                ucStatusDisplay.SetStatus($"Updating file...completed", Enums.StatusType.Processing);
                clsSearch._isWriteResponse = false;
                // ----------------------------------
                // re-create file
                // ----------------------------------

                ucStatusDisplay.SetStatus($"Processing merchant...", Enums.StatusType.Processing);
                InsertMerchantParticular();

                ucStatusDisplay.SetStatus($"Processing ir master...", Enums.StatusType.Processing);
                SaveImportIRMaster();

                ucStatusDisplay.SetStatus($"Processing ir detail...", Enums.StatusType.Processing);
                SaveInstallationRequestDetail();

                FillClientTextBox();

                ucStatusDisplay.SetStatus($"Processing installation request...", Enums.StatusType.Processing);
                SaveIRDetail();

                ucStatusDisplay.SetStatus($"Processing json data...", Enums.StatusType.Processing);
                processDataGridView(grdList);

                // ------------------------------------------------------------------------------------------
                // Upload physical attach file
                // ------------------------------------------------------------------------------------------                               
                ucStatusDisplay.SetStatus($"Uploading physical file of {txtFileName.Text}", Enums.StatusType.Upload);

                string pLocalPath = $"{txtPathFileName.Text.Replace(txtFileName.Text, "")}";
                string pRemotePath = $"{clsGlobalVariables.strFTPRemoteIRPath}{clsGlobalVariables.strAPIBank}{clsFunction.sBackSlash}";
                string pFileName = $"{txtFileName.Text}";

                Debug.WriteLine("Uploading import ir...");
                Debug.WriteLine($"pLocalPath=[{pLocalPath}]");
                Debug.WriteLine($"pRemotePath=[{pRemotePath}]");
                Debug.WriteLine($"pFileName=[{pFileName}]");

                ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                ftpClient.delete(pRemotePath + pFileName);
                ftpClient.upload(pRemotePath + pFileName, pLocalPath + pFileName);
                ftpClient.disconnect();

                Debug.WriteLine("Uploading import ir...complete");

                ucStatusDisplay.SetStatus($"Importing cmppleted.", Enums.StatusType.Success);

                dbFunction.SetMessageBox("Import installation request successfully saved.", "Saved", clsFunction.IconType.iInformation);

                btnCancel_Click(this, e);
                
                Cursor.Current = Cursors.Default;

            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox("Message " + ex.Message, "IR import failed", clsFunction.IconType.iError);
            }
            
        }

        void FillClientTextBox()
        {
            txtClientName.Text =
            txtClientAddress.Text =
            txtClientContactPerson.Text =
            txtClientTelNo.Text =
            txtClientMobile.Text =
            txtClientEmail.Text = clsFunction.sNull;

            if (dbFunction.isValidID(txtClientID.Text))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Client Info", txtClientID.Text, "Get Info Detail", "", "GetInfoDetail");

                dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                if (dbAPI.isNoRecordFound() == false)
                {
                    txtClientID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                    txtClientName.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtClientAddress.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                    txtClientContactPerson.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                    txtClientTelNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                    txtClientMobile.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                    txtClientEmail.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                    txtProfileConfigInfo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);

                }
            }
        }
        
        private void SaveImportIRMaster()
        {
            string sRowSQL = "";
            string sSQL = "";

            //string FilePath = txtPathFileName.Text.Replace(@"\", clsFunction.sAsterisk); ;
            string FileName = txtFileName.Text;
            string Remarks = txtRemarks.Text;

            string ProcessedBy = clsUser.ClassUserFullName;
            string ModifiedBy = clsUser.ClassUserFullName;

            sRowSQL = "";
            sRowSQL = " ('" + txtImportDate.Text + "', " +
            sRowSQL + sRowSQL + " '" + FileName + "', " +
            sRowSQL + sRowSQL + " '" + FileName + "', " +
            sRowSQL + sRowSQL + " '" + Remarks + "', " +
            sRowSQL + sRowSQL + " '" + txtImportDate.Text + "', " +
            sRowSQL + sRowSQL + " '" + ProcessedBy + "', " +
            sRowSQL + sRowSQL + " '" + ModifiedBy + "') ";
            sSQL = sSQL + sRowSQL;

            //Debug.WriteLine("POST: IR Import Master [" + sSQL + "]" + "\n");

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "IR Import Master", sSQL, "InsertCollectionMaster");

            clsSearch.ClassLastInsertedID = clsLastID.ClassLastInsertedID;
        }

        private void SaveInstallationRequestDetail()
        {
            string sTempSQL = "";
            string sRowSQL = "";
            string sSQL = "";
            int iRowCount = grdList.RowCount;
            int iColCount = grdList.ColumnCount; // exclude Result column
            int iRowIndex = 0;
            string sLastInsertID = clsSearch.ClassLastInsertedID.ToString();
            int iPRIME_TID_01 = 0;
            int iPRIME_MID_01 = 0;
            int iPRIME_TID_02 = 0;
            int iPRIME_MID_02 = 0;
            string sRowCSV = "";
            //string sCSV = "";
            int ii = 0;

            int i = 0;
            List<string> TempArrayDataCol = new List<String>();
            int iRecordMinLimit = clsSystemSetting.ClassSystemRecordMinLimit;
            int iStartIndex = 0;
            int iEndIndex = 0;
            int iFileNameIndex = 0;
            int x;
            StringBuilder columnbind = new StringBuilder();

            // TID
            iPRIME_TID_01 = dbFunction.GetMapColumnIndex(clsDefines.IR_TID);
            iPRIME_TID_02 = dbFunction.GetMapColumnIndex(clsDefines.IR_TID);

            // MID
            iPRIME_MID_01 = dbFunction.GetMapColumnIndex(clsDefines.IR_MID);
            iPRIME_MID_02 = dbFunction.GetMapColumnIndex(clsDefines.IR_MID);


            if (iRowCount > 0)
            {
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                // Delete File            
                //string sFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iIRImportDetail, 0);
                //dbFile.DeleteCSV(sFileName);

                for (i = iRowIndex; i < iRowCount; i++)
                {
                    ii++;
                    sRowSQL = "";
                    sSQL = "";
                    sTempSQL = "('" + sLastInsertID + "',";
                    for (x = 0; x < iColCount; x++)
                    {
                        string sCellValue = grdList.Rows[i].Cells[x].Value.ToString().Trim();

                        if (sCellValue.Length > 0)
                        {
                            // Check for iPRIME_TID_01
                            if (iPRIME_TID_01 == x && sCellValue.Trim().Length > 0)
                            {
                                string sPRIME_TID_01 = dbFunction.padLeftChar(sCellValue, clsFunction.sPadZero, 8);
                                sCellValue = sPRIME_TID_01;
                            }

                            // Check for iPRIME_TID_02
                            if (iPRIME_TID_02 == x && sCellValue.Trim().Length > 0)
                            {
                                string sPRIME_TID_02 = dbFunction.padLeftChar(sCellValue, clsFunction.sPadZero, 8);
                                sCellValue = sPRIME_TID_02;
                            }

                            // Check for iPRIME_MID_01
                            if (iPRIME_MID_01 == x && sCellValue.Trim().Length > 0)
                            {
                                string sPRIME_MID_01 = dbFunction.padLeftChar(sCellValue, clsFunction.sPadZero, 15);
                                sCellValue = sPRIME_MID_01;
                            }

                            // Check for iPRIME_MID_02
                            if (iPRIME_MID_02 == x && sCellValue.Trim().Length > 0)
                            {
                                string sPRIME_MID_02 = dbFunction.padLeftChar(sCellValue, clsFunction.sPadZero, 15);
                                sCellValue = sPRIME_MID_02;
                            }

                            if (x == iColCount - 1)
                                sTempSQL = sTempSQL + "'" + (sCellValue.Length > 0 ? sCellValue : clsFunction.sDash) + "'";
                            else
                                sTempSQL = sTempSQL + "'" + (sCellValue.Length > 0 ? sCellValue : clsFunction.sDash) + "',";
                        }
                    }

                    sRowSQL = sTempSQL + ")";

                    if (sSQL.Length > 0)
                        sSQL = sSQL + "," + sRowSQL;
                    else
                        sSQL = sSQL + sRowSQL;

                    sRowCSV = "";
                    sRowCSV = sSQL.Replace("(", "");
                    sRowCSV = sRowCSV.Replace(")", "");
                    sRowCSV = sRowCSV.Replace("'", "");
                    //sRowCSV = sRowCSV.Replace("&", "AND");
                    //sCSV = sCSV + sRowCSV + "\n";

                    //Debug.WriteLine("SaveIRMaster->>ii=" + ii.ToString() + "-" + "sRowCSV=" + sRowCSV);

                    if (sRowCSV.Length > 0)
                    {
                        TempArrayDataCol.Add(sRowCSV);
                    }

                    //ucStatusImport.iState = 3;
                    //ucStatusImport.sMessage = "IMPORTING IR MASTER";
                    //ucStatusImport.iMin = ii;
                    //ucStatusImport.iMax = grdList.RowCount;
                    //ucStatusImport.AnimateStatus();
                }

                // ----------------------------------------------------------
                // Limit line# of csv created depends on iRecordMinLimit
                // ----------------------------------------------------------
                clsArray.sRepoData = TempArrayDataCol.ToArray();

                if (iRecordMinLimit <= clsArray.sRepoData.Length)
                {
                    i = 0;
                    do
                    {
                        if (i % iRecordMinLimit == 0 && i > 0)
                        {
                            iStartIndex = i - iRecordMinLimit;
                            iEndIndex = i;
                            columnbind.Clear();
                            //Debug.WriteLine("iStartIndex=" + iStartIndex.ToString() + "," + "iEndIndex=" + iEndIndex.ToString());
                            for (x = iStartIndex; x < iEndIndex; x++)
                            {
                                //Debug.WriteLine("x=" + x.ToString() + "," + "Data=" + clsArray.sRepoData[x].ToString());

                                columnbind.Append(clsArray.sRepoData[x].ToString());

                                if (x < iEndIndex - 1)
                                    columnbind.Append("\r\n");
                            }

                            // Write To File
                            iFileNameIndex++;
                            string sNewFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iIRImportDetail, iFileNameIndex);
                            
                            ucStatusDisplay.SetStatus($"Creating CSV file of {sNewFileName}", Enums.StatusType.Create);
                            Task.Delay(delay); // Asynchronously wait without blocking UI

                            dbFile.DeleteCSV(sNewFileName);
                            dbFile.WriteCSV(sNewFileName, columnbind.ToString());
                        }

                        i++;
                    } while (i < clsArray.sRepoData.Length);
                }
                else
                {
                    iEndIndex = 0;
                }

                // Get Last Part
                if (clsArray.sRepoData.Length > iEndIndex)
                {
                    int iTempStartIndex = iEndIndex;
                    int iTempEndIndex = clsArray.sRepoData.Length;
                    columnbind.Clear();
                    //Debug.WriteLine("iTempStartIndex=" + iTempStartIndex.ToString() + "," + "iTempEndIndex=" + iTempEndIndex.ToString());
                    for (x = iTempStartIndex; x < iTempEndIndex; x++)
                    {
                        //Debug.WriteLine("x=" + x.ToString() + "," + "Data=" + clsArray.sRepoData[x].ToString());

                        columnbind.Append(clsArray.sRepoData[x].ToString());

                        if (x < iTempEndIndex - 1)
                            columnbind.Append("\r\n");
                    }

                    // Write To File
                    iFileNameIndex++;
                    string sNewFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iIRImportDetail, iFileNameIndex);
                    
                    ucStatusDisplay.SetStatus($"Creating CSV file of {sNewFileName}", Enums.StatusType.Create);
                    Task.Delay(delay); // Asynchronously wait without blocking UI

                    dbFile.DeleteCSV(sNewFileName);
                    dbFile.WriteCSV(sNewFileName, columnbind.ToString());
                }

                // Import CSV
                for (i = 1; i <= iFileNameIndex; i++)
                {
                    string sImportFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iIRImportDetail, i);
                    
                    ucStatusDisplay.SetStatus($"Uploading CSV file of {sImportFileName}", Enums.StatusType.Upload);
                    Task.Delay(delay); // Asynchronously wait without blocking UI

                    // Upload File to FTP                                
                    Debug.WriteLine("=>>UploadFile=" + sImportFileName);
                    ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                    ftpClient.delete(clsGlobalVariables.strFTPUploadPath + sImportFileName);
                    ftpClient.upload(clsGlobalVariables.strFTPUploadPath + sImportFileName, @clsGlobalVariables.strFTPLocalPath + sImportFileName);
                    ftpClient.disconnect(); // ftp disconnect

                    Debug.WriteLine("=>>API Call ImportIRMaster=" + sImportFileName);
                    dbAPI.ExecuteAPI("POST", "Import", "CSV", sImportFileName, "Import IRMaster", "", "ImportIRMaster"); // Process CSV File

                }

                dbFunction.GetResponseTime("Import IRMaster");

                Cursor.Current = Cursors.Default; // Back to normal 
            }
        }

        private void SaveIRDetail()
        {
            Debug.WriteLine("--SaveIRDetail--");

            string sSQL = "";            
            int iRowCount = 0;
            int iColCount = 0;
            int i = 0;
            string sLastInsertID = clsSearch.ClassLastInsertedID.ToString();
            int ii = 0;

            List<string> TempArrayDataCol = new List<String>();
            int iRecordMinLimit = clsSystemSetting.ClassSystemRecordMinLimit;
            int iStartIndex = 0;
            int iEndIndex = 0;
            int iFileNameIndex = 0;
            int x;
            StringBuilder columnbind = new StringBuilder();

            iRowCount = grdList.RowCount;
            iColCount = grdList.ColumnCount; // exclude Result column

            int iIRNo = dbFunction.GetMapColumnIndex(clsDefines.IR_REQUEST_ID);
            int iDBAName = dbFunction.GetMapColumnIndex(clsDefines.IR_MERCHANT_NAME);
            int iPRIME_TID_01 = dbFunction.GetMapColumnIndex(clsDefines.IR_TID);
            int iPRIME_MID_01 = dbFunction.GetMapColumnIndex(clsDefines.IR_MID);
            int iRequestDate = dbFunction.GetMapColumnIndex(clsDefines.IR_REQUEST_DATE);
            int iInstDate = dbFunction.GetMapColumnIndex(clsDefines.IR_TARGET_INSTALLATION_DATE);

            int iSETUP = dbFunction.GetMapColumnIndex(clsDefines.IR_POS_SETUP);
            int iREQUEST_FOR = dbFunction.GetMapColumnIndex(clsDefines.IR_REQUEST_TYPE);
            int iREQUESTOR = dbFunction.GetMapColumnIndex(clsDefines.IR_REQUESTOR);
            int iALIPAY_TID_01 = dbFunction.GetMapColumnIndex(clsDefines.IR_ALIPAY_TID);
            int iALIPAY_MID_01 = dbFunction.GetMapColumnIndex(clsDefines.IR_ALIPAY_MID);
            int iWECHAT_TID_01 = dbFunction.GetMapColumnIndex(clsDefines.IR_WECHAT_TID);
            int iWECHAT_MID_01 = dbFunction.GetMapColumnIndex(clsDefines.IR_WECHAT_MID);
            int iOTHER_SPECIAL_INSTRUCTION = dbFunction.GetMapColumnIndex(clsDefines.IR_RM_INSTRUCTION);

            int iPOS_TYPE = dbFunction.GetMapColumnIndex(clsDefines.IR_POS_TYPE);
            int iREQUEST_TYPE = dbFunction.GetMapColumnIndex(clsDefines.IR_REQUEST_TYPE);

            bool isExist = false;

            
            dbFunction.GetCurrentDateTime();

            if (iRowCount > 0)
            {
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
                
                for (i = 0; i < iRowCount; i++)
                {
                    ii++;
                    string sIRNo = GetColumnValue(i, iIRNo, grdList);
                    string sDBAName = GetColumnValue(i, iDBAName, grdList);
                    string sPRIME_TID_01 = GetColumnValue(i, iPRIME_TID_01, grdList);
                    string sPRIME_MID_01 = GetColumnValue(i, iPRIME_MID_01, grdList);
                    string sRequestDate = GetColumnValue(i, iRequestDate, grdList);
                    string sInstDate = GetColumnValue(i, iInstDate, grdList);
                    
                    int iIRStatus = clsGlobalVariables.STATUS_AVAILABLE;
                    string sIRStatusDescription = clsGlobalVariables.STATUS_AVAILABLE_DESC;

                    string sSetUp = GetColumnValue(i, iSETUP, grdList).Trim();
                    string sRequestFor = GetColumnValue(i, iREQUEST_FOR, grdList).Trim();
                    string sRequestor = GetColumnValue(i, iREQUESTOR, grdList).Trim();
                    string sALIPAY_TID_01 = GetColumnValue(i, iALIPAY_TID_01, grdList).Trim();
                    string sALIPAY_MID_01 = GetColumnValue(i, iALIPAY_MID_01, grdList).Trim();
                    string sWECHAT_TID_01 = GetColumnValue(i, iWECHAT_TID_01, grdList).Trim();
                    string sWECHAT_MID_01 = GetColumnValue(i, iWECHAT_MID_01, grdList).Trim();
                    string sDCC_MERCHANT = "-";
                    string sOTHER_SPECIAL_INSTRUCTION = GetColumnValue(i, iOTHER_SPECIAL_INSTRUCTION, grdList).Trim();
                    string sPOS_TYPE = GetColumnValue(i, iPOS_TYPE, grdList).Trim();
                    string sREQUEST_TYPE = GetColumnValue(i, iREQUEST_TYPE, grdList).Trim();

                    // get ParticularID - getinfodetail
                    int iParticularID = 0;
                    dbAPI.ExecuteAPI("GET", "Search", clsDefines.GetID_Info, $"{clsDefines.SearchBy_Particular}{clsDefines.gPipe}{sDBAName}", "Get Info Detail", "", "GetInfoDetail");
                    if (dbAPI.isNoRecordFound() == false)
                    {
                        iParticularID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_ID));
                    }

                    // get RequestTypeID - getInfoDetail
                    int iRequestTypeID = 0;
                    dbFunction.GetIDFromFile("All Type", sREQUEST_TYPE);
                    iRequestTypeID = clsSearch.ClassOutFileID;

                    //dbAPI.ExecuteAPI("GET", "Search", clsDefines.GetID_Info, $"{clsDefines.SearchBy_Type}{clsDefines.gPipe}{sREQUEST_TYPE}", "Get Info Detail", "", "GetInfoDetail");
                    //if (dbAPI.isNoRecordFound() == false)
                    //{
                    //    iRequestTypeID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_ID));
                    //}

                    // Temp
                    string sTempTerminalSerialNo = ""; // GetColumnValue(i, iTempTerminalSerialNo, grdList);
                    string sTempBaseSerialNo = ""; // GetColumnValue(i, iTempBaseSerialNo, grdList);
                    string sTempTerminalSimSerialNo = ""; // GetColumnValue(i, iTempTerminalSimSerialNo, grdList);
                    string sTempStatus = ""; // GetColumnValue(i, iTempStatus, grdList);
                    string sTempStatusDate = ""; // GetColumnValue(i, iTempStatusDate, grdList);
                    string sTempAppVersion = ""; // GetColumnValue(i, iTempAppVersion, grdList);
                    string sTempAppCRC = ""; // GetColumnValue(i, iTempAppCRC, grdList);

                    isExist = dbAPI.isRecordExist("Search", "TID/MID", txtClientID.Text + clsDefines.gPipe + sPRIME_TID_01 + clsDefines.gPipe + sPRIME_MID_01 + clsDefines.gPipe + sIRNo);

                    string sExist = (isExist ? "Found" : "Not Found");

                    //Debug.WriteLine("ClientID="+ txtClientID.Text);
                    //Debug.WriteLine("iParticularID=" + iParticularID.ToString());
                    //Debug.WriteLine("sDBAName=" + sDBAName);
                    //Debug.WriteLine("IRNo=" + sIRNo);
                    //Debug.WriteLine("isExist=" + isExist);

                    Debug.WriteLine($"Checking IR Detail->Line#:{ii}[Merchant Name:{sDBAName}->ParticularID:{iParticularID}, ClientID:{txtClientID.Text}, TID:{sPRIME_TID_01}, MID:{sPRIME_MID_01}, RequestID:{sIRNo}, RequestType:{sREQUEST_TYPE}->RequestTypeID:{iRequestTypeID}, Exist:{sExist}]");

                    //ucStatusDisplay.SetStatus($"Checking TID/MID [{ii}/{iRowCount}] [{sPRIME_TID_01}][{sPRIME_MID_01}] [{sExist}]", Enums.StatusType.Processing);

                    if (dbFunction.isValidDescription(sIRNo) && dbFunction.isValidID(iParticularID.ToString()) && !isExist)
                    {
                        sSQL = "";
                        var data = new
                        {
                            LastInsertedID = dbFunction.CheckAndSetStringValue(sLastInsertID),
                            ParticularID = dbFunction.CheckAndSetNumericValue(iParticularID.ToString()),
                            MerchantID = dbFunction.CheckAndSetNumericValue(iParticularID.ToString()),
                            IRNo = dbFunction.CheckAndSetStringValue(sIRNo),
                            IRDate = dbFunction.CheckAndSetStringValue(sRequestDate),
                            InstallationDate = dbFunction.CheckAndSetStringValue(sInstDate),
                            MerchantName = dbFunction.CheckAndSetStringValue(sDBAName),
                            Name = dbFunction.CheckAndSetStringValue(sDBAName),
                            TID = dbFunction.CheckAndSetStringValue(sPRIME_TID_01),
                            MID = dbFunction.CheckAndSetStringValue(sPRIME_MID_01),
                            IRStatus = dbFunction.CheckAndSetNumericValue(iIRStatus.ToString()),
                            IRStatusDescription = dbFunction.CheckAndSetStringValue(sIRStatusDescription),
                            ClientID = dbFunction.CheckAndSetNumericValue(txtClientID.Text),
                            ClientName = dbFunction.CheckAndSetStringValue(cboSearchClient.Text),
                            ProcessType = dbFunction.CheckAndSetStringValue(clsGlobalVariables.PROCESS_TYPE_AUTO_DESC),
                            ProcessedDateTime = dbFunction.CheckAndSetStringValue(clsSearch.ClassCurrentDateTime),
                            ModifiedDateTime = dbFunction.CheckAndSetStringValue(clsSearch.ClassCurrentDateTime),
                            ProcessedBy = dbFunction.CheckAndSetStringValue(clsUser.ClassUserFullName),
                            ModifiedBy = dbFunction.CheckAndSetStringValue(clsUser.ClassUserFullName),
                            TempTerminalSerial = dbFunction.CheckAndSetStringValue(sTempTerminalSerialNo),
                            TempBaseSerial = dbFunction.CheckAndSetStringValue(sTempBaseSerialNo),
                            TempSimSerial = dbFunction.CheckAndSetStringValue(sTempTerminalSimSerialNo),
                            TempStatus = dbFunction.CheckAndSetStringValue(sTempStatus),
                            TempStatusDate = dbFunction.CheckAndSetStringValue(sTempStatusDate),
                            TempAppVersion = dbFunction.CheckAndSetStringValue(sTempAppVersion),
                            TempAppCRC = dbFunction.CheckAndSetStringValue(sTempAppCRC),
                            SET_UP = dbFunction.CheckAndSetStringValue(sSetUp),
                            REQUEST_FOR = dbFunction.CheckAndSetStringValue(sRequestFor),
                            ALIPAY_TID = dbFunction.CheckAndSetStringValue(sALIPAY_TID_01),
                            ALIPAY_MID = dbFunction.CheckAndSetStringValue(sALIPAY_MID_01),
                            WECHAT_TID = dbFunction.CheckAndSetStringValue(sWECHAT_TID_01),
                            WECHAT_MID = dbFunction.CheckAndSetStringValue(sWECHAT_MID_01),
                            DCC_MERCHANT = dbFunction.CheckAndSetStringValue(sDCC_MERCHANT),
                            OTHER_SPECIAL_INSTRUCTION = dbFunction.CheckAndSetStringValue(sOTHER_SPECIAL_INSTRUCTION),
                            POS_TYPE = dbFunction.CheckAndSetStringValue(sPOS_TYPE),
                            REQUESTOR = dbFunction.CheckAndSetStringValue(sRequestor),
                            RequestTypeID = dbFunction.CheckAndSetStringValue(iRequestTypeID.ToString())

                        };

                        sSQL = IFormat.Insert(data).Replace("'", ""); // remove ' since saving to csv file

                        Debug.WriteLine("SaveIRDetail" + sSQL);

                        dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 0);

                        if (sSQL.Length > 0)
                        {
                            TempArrayDataCol.Add(sSQL);
                        }
                    }
                    else
                    {
                        // do nothing if already exist.!

                        //string sMessage = "Failed to save merchant details.";
                        //string sTemp = "Line#" + ii + Environment.NewLine + "Request ID: " + sIRNo + Environment.NewLine + "Merchant Name: " + sDBAName + Environment.NewLine + Environment.NewLine;
                        //string sHint = "Merchant does not exist.";
                        //frmPromptMessage.sMenuHeader = sMessage;
                        //frmPromptMessage.sMessage = sTemp;

                        //frmPromptMessage.sHintMessage = "";
                        //frmPromptMessage.sHint = sHint + Environment.NewLine + Environment.NewLine + "Please check import file.";
                        //frmPromptMessage frm = new frmPromptMessage();
                        //frm.ShowDialog();

                        //return;

                        Debug.WriteLine($"Merchant not found -> Request ID [{sIRNo}], Merchant [{sDBAName}], TID [{sPRIME_TID_01}], MID [{sPRIME_MID_01}]");

                        dbFunction.SetMessageBox(
                            $"Merchant details not found.\n" +
                            $"Line #: {ii}\n" +
                            $"Client: {cboSearchClient.Text}\n" +
                            $"Request ID: {sIRNo}\n" +
                            $"Name: {sDBAName}\n" +
                            $"TID: {sPRIME_TID_01}\n" +
                            $"MID: {sPRIME_MID_01}\n\n" +
                            $"{clsDefines.CONTACT_ADMIN_MESSAGE}",
                            $"{clsDefines.FIELD_CHECK_MSG}-Import",
                            clsFunction.IconType.iWarning
                        );

                        return;
                    }

                    ucStatusDisplay.SetStatus($"Processing Installation Request ID [{ii}/{iRowCount}] [{sIRNo}]", Enums.StatusType.Processing);
                    Task.Delay(delay); // Asynchronously wait without blocking UI
                }

                // ----------------------------------------------------------
                // Limit line# of csv created depends on iRecordMinLimit
                // ----------------------------------------------------------
                clsArray.sRepoData = TempArrayDataCol.ToArray();

                if (iRecordMinLimit <= clsArray.sRepoData.Length)
                {
                    i = 0;
                    do
                    {
                        if (i % iRecordMinLimit == 0 && i > 0)
                        {
                            iStartIndex = i - iRecordMinLimit;
                            iEndIndex = i;
                            columnbind.Clear();
                            //Debug.WriteLine("iStartIndex=" + iStartIndex.ToString() + "," + "iEndIndex=" + iEndIndex.ToString());
                            for (x = iStartIndex; x < iEndIndex; x++)
                            {
                                //Debug.WriteLine("x=" + x.ToString() + "," + "Data=" + clsArray.sRepoData[x].ToString());

                                columnbind.Append(clsArray.sRepoData[x].ToString());

                                if (x < iEndIndex - 1)
                                    columnbind.Append("\r\n");
                            }

                            // Write To File
                            iFileNameIndex++;
                            string sNewFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iIRDetail, iFileNameIndex);
                            Debug.WriteLine("IR->sNewFileName=" + sNewFileName);
                            
                            ucStatusDisplay.SetStatus($"Creating CSV file of {sNewFileName}", Enums.StatusType.Create);
                            Task.Delay(delay); // Asynchronously wait without blocking UI

                            dbFile.DeleteCSV(sNewFileName);
                            dbFile.WriteCSV(sNewFileName, columnbind.ToString());
                        }

                        i++;
                    } while (i < clsArray.sRepoData.Length);
                }
                else
                {
                    iEndIndex = 0;
                }

                // Get Last Part
                if (clsArray.sRepoData.Length > iEndIndex)
                {
                    int iTempStartIndex = iEndIndex;
                    int iTempEndIndex = clsArray.sRepoData.Length;
                    columnbind.Clear();
                    //Debug.WriteLine("iTempStartIndex=" + iTempStartIndex.ToString() + "," + "iTempEndIndex=" + iTempEndIndex.ToString());
                    for (x = iTempStartIndex; x < iTempEndIndex; x++)
                    {
                        //Debug.WriteLine("x=" + x.ToString() + "," + "Data=" + clsArray.sRepoData[x].ToString());

                        columnbind.Append(clsArray.sRepoData[x].ToString());

                        dbFunction.parseDelimitedString(clsArray.sRepoData[x].ToString(), clsDefines.gComma, 0);

                        if (x < iTempEndIndex - 1)
                            columnbind.Append("\r\n");
                    }

                    // Write To File
                    iFileNameIndex++;
                    string sNewFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iIRDetail, iFileNameIndex);
                    Debug.WriteLine("IR->sNewFileName=" + sNewFileName);
                    
                    ucStatusDisplay.SetStatus($"Creating CSV file of {sNewFileName}", Enums.StatusType.Create);
                    Task.Delay(delay); // Asynchronously wait without blocking UI

                    dbFile.DeleteCSV(sNewFileName);
                    dbFile.WriteCSV(sNewFileName, columnbind.ToString());
                }

                // Import IR
                if (iFileNameIndex > 0)
                {
                    for (i = 1; i <= iFileNameIndex; i++)
                    {
                        string sImportFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iIRDetail, i);

                        ucStatusDisplay.SetStatus($"Uploading CSV file of {sImportFileName}", Enums.StatusType.Upload);
                        Task.Delay(delay); // Asynchronously wait without blocking UI

                        // Upload File to FTP                                
                        Debug.WriteLine("=>>UploadFile=" + sImportFileName);
                        ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                        ftpClient.delete(clsGlobalVariables.strFTPUploadPath + sImportFileName);
                        ftpClient.upload(clsGlobalVariables.strFTPUploadPath + sImportFileName, @clsGlobalVariables.strFTPLocalPath + sImportFileName);
                        ftpClient.disconnect(); // ftp disconnect

                        Debug.WriteLine("=>>API Call ImportIRDetail=" + sImportFileName);
                        dbAPI.ExecuteAPI("POST", "Import", "CSV", sImportFileName, "Import IRDetail", "", "ImportIRDetail"); // Process CSV File

                    }
                }
                
                dbFunction.GetResponseTime("Import IRDetail");

                Cursor.Current = Cursors.Default; // Back to normal
            }
        }

        private void SaveMIRDetail()
        {
            string sSQL = "";
            string sRowSQL = "";
            int iLastInsertID = 0;
            string sReqDate = dbFunction.GetDateFromParse(dteReqDate.Text, "MM-dd-yyyy H:mm:ss tt", "yyyy-MM-dd");
            string sInstDate = dbFunction.GetDateFromParse(dteInstDate.Text, "MM-dd-yyyy H:mm:ss tt", "yyyy-MM-dd");

            dbFunction.GetCurrentDateTime();

            sSQL = "";
            sRowSQL = "";
            sRowSQL = "('" + iLastInsertID + "'," +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtMerchantID.Text) + "'," +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtMerchantID.Text) + "'," +
            sRowSQL + sRowSQL + "'" + txtIRNo.Text + "'," +
            sRowSQL + sRowSQL + "'" + sReqDate + "'," +
            sRowSQL + sRowSQL + "'" + sInstDate + "'," +
            sRowSQL + sRowSQL + "'" + txtMerchantName.Text + "'," +
            sRowSQL + sRowSQL + "'" + txtMerchantName.Text + "'," +
            sRowSQL + sRowSQL + "'" + txtIRTID.Text + "'," +
            sRowSQL + sRowSQL + "'" + txtIRMID.Text + "'," +
            sRowSQL + sRowSQL + "'" + clsGlobalVariables.STATUS_AVAILABLE + "'," +
            sRowSQL + sRowSQL + "'" + clsGlobalVariables.STATUS_AVAILABLE_DESC + "'," +
            sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtClientID.Text) + "'," +
            sRowSQL + sRowSQL + "'" + txtClientName.Text + "'," +
            sRowSQL + sRowSQL + "'" + cboPOSType.Text + "'," +
            sRowSQL + sRowSQL + "'" + txtMRequestFor.Text + "'," +
            sRowSQL + sRowSQL + "'" + txtMSetup.Text + "'," +
            sRowSQL + sRowSQL + "'" + txtMRemarks.Text + "'," +
            sRowSQL + sRowSQL + "'" + txtMComments.Text + "'," +
            sRowSQL + sRowSQL + "'" + txtMInstruction.Text + "'," +
            sRowSQL + sRowSQL + "'" + clsGlobalVariables.PROCESS_TYPE_MANUAL_DESC + "'," +
            sRowSQL + sRowSQL + "'" + clsSearch.ClassCurrentDateTime + "'," +
            sRowSQL + sRowSQL + "'" + clsSearch.ClassCurrentDateTime + "'," +
            sRowSQL + sRowSQL + "'" + clsUser.ClassUserFullName + "'," +
            sRowSQL + sRowSQL + "'" + clsUser.ClassUserFullName + "') ";
            sSQL = sSQL + sRowSQL;

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "IR Import Detail", sSQL, "InsertCollectionDetail");

            txtIRIDNo.Text = clsLastID.ClassLastInsertedID.ToString();

        }

        private void InitImportDate()
        {
            DateTime ImportDateTime = DateTime.Now;
            string sImportDateTime = "";

            DateTime DateTimeModified = DateTime.Now;
            string sDateTimeModified = "";

            string FilePath = txtPathFileName.Text;
            string FileName = txtFileName.Text;
            string Remarks = txtRemarks.Text;

            sImportDateTime = ImportDateTime.ToString("yyyy-MM-dd H:mm:ss");
            sDateTimeModified = DateTimeModified.ToString("yyyy-MM-dd H:mm:ss");

            txtImportDate.Text = sImportDateTime;
        }

        private void InitProcessedBy()
        {
            txtProcessBy.Text = clsUser.ClassUserFullName;
        }
        private int GetHeaderColumnIndex(string sHeader)
        {
            int iColIndex = 0;

            for (int i = 0; i < grdList.ColumnCount; i++)
            {
                string cellParam = grdList.Columns[i].Name;

                if (sHeader.CompareTo(cellParam) == 0)
                {
                    iColIndex = i;
                    break;
                }
            }

            return iColIndex;
        }


        private void InsertMerchantParticular()
        {
            string sRowSQL = "";
            string sSQL = "";
            string sRowCSV = "";
            //string sCSV = "";
            int iRowCount = grdList.RowCount;
            int iColCount = grdList.ColumnCount; // exclude Result column
            int iRowIndex = 0;
            int ii = 0;

            List<string> TempArrayDataCol = new List<String>();
            int iRecordMinLimit = clsSystemSetting.ClassSystemRecordMinLimit;
            int iStartIndex = 0;
            int iEndIndex = 0;
            int iFileNameIndex = 0;
            int x;
            int i = 0;
            StringBuilder columnbind = new StringBuilder();

            int iRMColIndex = GetHeaderColumnIndex(clsDefines.IR_RM_INSTRUCTION);
            int iMerchNameColIndex = GetHeaderColumnIndex(clsDefines.IR_MERCHANT_NAME);
            int iMerchant_TID = GetHeaderColumnIndex(clsDefines.IR_TID);
            int iMerchant_MID = GetHeaderColumnIndex(clsDefines.IR_MID);
            int iAddressColIndex = GetHeaderColumnIndex(clsDefines.IR_ADDRESS);
            int iCityColIndex = GetHeaderColumnIndex(clsDefines.IR_CITY);
            int iProvinceColIndex = GetHeaderColumnIndex(clsDefines.IR_AREA1);
            int iContactPersonColIndex = GetHeaderColumnIndex(clsDefines.IR_CONTACT_PERSON);
            int iVoiceColIndex = GetHeaderColumnIndex(clsDefines.IR_CONTACT_NUMBER);
            int iSMSColIndex = GetHeaderColumnIndex(clsDefines.IR_CONTACT_NUMBER);
            int iAreaCodeColIndex = GetHeaderColumnIndex(clsDefines.IR_AREA2);

            dbAPI.GetCityList("View", "", "", "City"); // Load City
            dbAPI.GetProvinceList("View", "", "", "Province"); // Load Province
            dbAPI.GetRegionDetailList("View", "", "", "Region Detail"); // Load Region Detail

            if (iRowCount > 0)
            {
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                // Delete File
                //string sFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iMerchant, 0);                
                //dbFile.DeleteCSV(sFileName);

                for (i = iRowIndex; i < iRowCount; i++)
                {
                    ii++;
                    string sRM = GetColumnValue(i, iRMColIndex, grdList);
                    string sMerchName = GetColumnValue(i, iMerchNameColIndex, grdList);
                    string sTID = dbFunction.padLeftChar(GetColumnValue(i, iMerchant_TID, grdList).Trim(), clsFunction.sZero, clsFunction.TID_LENGTH);
                    string sMID = dbFunction.padLeftChar(GetColumnValue(i, iMerchant_MID, grdList).Trim(), clsFunction.sZero, clsFunction.MID_LENGTH);
                    string sAddress = GetColumnValue(i, iAddressColIndex, grdList);
                    string sAddress2 = "-";
                    string sAddress3 = "-";
                    string sAddress4 = "-";
                    string sCity = GetColumnValue(i, iCityColIndex, grdList);
                    string sProvince = GetColumnValue(i, iProvinceColIndex, grdList);
                    string sContactPerson = GetColumnValue(i, iContactPersonColIndex, grdList);
                    string sContactNumber = GetColumnValue(i, iSMSColIndex, grdList);
                    string sEmail = "-";
                    string sVoice = GetColumnValue(i, iVoiceColIndex, grdList);
                    string sSMS = GetColumnValue(i, iSMSColIndex, grdList);
                    string sAreaCode = GetColumnValue(i, iAreaCodeColIndex, grdList);

                    int iCityID = dbAPI.GetCityFromList(sCity);
                    int iProvinceID = dbAPI.GetProvinceFromList(sProvince);

                    dbAPI.GetRegionDetailFromList(sCity);

                    //bool fExist = dbAPI.isRecordExist("Search", "Merchant TID/MID", sTID + clsDefines.gPipe + sMID + clsDefines.gPipe + sMerchName);
                    bool fExist = dbAPI.isRecordExist("Search", "Merchant-Address", sMerchName + clsDefines.gPipe + sAddress + clsDefines.gPipe + sProvince + clsDefines.gPipe + sCity);

                    if (sMerchName.Length > 0 && sMerchName.CompareTo(clsFunction.sDash) != 0 && !fExist)
                    {
                        clsSearch.ClassParticularName = sMerchName;

                        clsParticular.ClassParticularName = StrClean(sMerchName.Length > 0 ? sMerchName : clsFunction.sDash);
                        clsParticular.ClassAddress = StrClean(sAddress.Length > 0 ? sAddress : clsFunction.sDash);
                        clsParticular.ClassAddress2 = StrClean(sAddress2.Length > 0 ? sAddress2 : clsFunction.sDash);
                        clsParticular.ClassAddress3 = StrClean(sAddress3.Length > 0 ? sAddress3 : clsFunction.sDash);
                        clsParticular.ClassAddress4 = StrClean(sAddress4.Length > 0 ? sAddress4 : clsFunction.sDash);
                        clsParticular.ClassContactPerson = StrClean(sContactPerson.Length > 0 ? sContactPerson : clsFunction.sDash);
                        clsParticular.ClassContactNumber = (sContactNumber.Length > 0 ? sContactNumber : clsFunction.sDash);
                        clsParticular.ClassTelNo = (sVoice.Length > 0 ? sVoice : clsFunction.sDash);
                        clsParticular.ClassMobile = (sSMS.Length > 0 ? sSMS : clsFunction.sDash);
                        clsParticular.ClassFax = clsFunction.sDash;
                        clsParticular.ClassEmail = (sEmail.Length > 0 ? sEmail : clsFunction.sDash);
                        clsParticular.ClassContractTerms = clsFunction.sDash;
                        clsParticular.ClassCityID = iCityID;
                        clsParticular.ClassProvinceID = iProvinceID;
                        clsParticular.ClassRegionID = clsSearch.ClassRegionID;
                        clsParticular.ClassRegionType = clsSearch.ClassRegionType;
                        clsParticular.ClassRegion = clsSearch.ClassRegion;
                        clsParticular.ClassProvince = clsSearch.ClassProvince;
                        clsParticular.ClassAreaCode = sAreaCode;

                        sSQL = "";
                        sRowSQL = "";
                        sRowSQL = "('" + clsParticular.ClassParticularName + "'," +
                        sRowSQL + sRowSQL + "'" + clsParticular.ClassAddress + "'," +
                        sRowSQL + sRowSQL + "'" + clsParticular.ClassAddress2 + "'," +
                        sRowSQL + sRowSQL + "'" + clsParticular.ClassAddress3 + "'," +
                        sRowSQL + sRowSQL + "'" + clsParticular.ClassAddress4 + "'," +
                        sRowSQL + sRowSQL + "" + clsParticular.ClassProvinceID + "," +
                        sRowSQL + sRowSQL + "" + clsParticular.ClassCityID + "," +
                        sRowSQL + sRowSQL + "" + clsGlobalVariables.iMerchant_Type + "," +
                        sRowSQL + sRowSQL + "'" + clsGlobalVariables.sMerchant_Type + "'," +
                        sRowSQL + sRowSQL + "'" + clsParticular.ClassContactPerson + "'," +
                        sRowSQL + sRowSQL + "'" + clsParticular.ClassTelNo + "'," +
                        sRowSQL + sRowSQL + "'" + clsParticular.ClassMobile + "'," +
                        sRowSQL + sRowSQL + "'" + clsParticular.ClassFax + "'," +
                        sRowSQL + sRowSQL + "'" + clsParticular.ClassEmail + "'," +
                        sRowSQL + sRowSQL + "'" + clsParticular.ClassContractTerms + "'," +
                        sRowSQL + sRowSQL + "'" + clsParticular.ClassRegionID + "'," +
                        sRowSQL + sRowSQL + "'" + clsParticular.ClassRegionType + "'," +
                        sRowSQL + sRowSQL + "'" + clsParticular.ClassRegion + "'," +
                        sRowSQL + sRowSQL + "'" + clsParticular.ClassProvince + "'," +
                        sRowSQL + sRowSQL + "'" + clsParticular.ClassAreaCode + "')";
                        sSQL = sSQL + sRowSQL;

                        dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 1);

                        sRowCSV = "";
                        sRowCSV = sSQL.Replace("(", "");
                        sRowCSV = sRowCSV.Replace(")", "");
                        sRowCSV = sRowCSV.Replace("'", "");
                        //sRowCSV = sRowCSV.Replace("&", "AND");
                        //sCSV = sCSV + sRowCSV + "\n";

                        

                        Debug.WriteLine("ii=" + ii.ToString() + "-" + "sRowCSV=" + sRowCSV);

                        if (sRowCSV.Length > 0)
                        {
                            TempArrayDataCol.Add(sRowCSV);
                        }
                    }
                    
                    ucStatusDisplay.SetStatus($"Processing merchant [{ii}/{iRowCount}] [{sMerchName}]", Enums.StatusType.Success);
                    Task.Delay(delay); // Asynchronously wait without blocking UI
                }

                //dbAPI.ExecuteAPI("POST", "Insert", "", "", "Particular", sSQL, "InsertMaintenanceMaster");

                // ----------------------------------------------------------
                // Limit line# of csv created depends on iRecordMinLimit
                // ----------------------------------------------------------
                clsArray.sRepoData = TempArrayDataCol.ToArray();

                if (iRecordMinLimit <= clsArray.sRepoData.Length)
                {
                    i = 0;
                    do
                    {
                        if (i % iRecordMinLimit == 0 && i > 0)
                        {
                            iStartIndex = i - iRecordMinLimit;
                            iEndIndex = i;
                            columnbind.Clear();
                            //Debug.WriteLine("iStartIndex=" + iStartIndex.ToString() + "," + "iEndIndex=" + iEndIndex.ToString());
                            for (x = iStartIndex; x < iEndIndex; x++)
                            {
                                //Debug.WriteLine("x=" + x.ToString() + "," + "Data=" + clsArray.sRepoData[x].ToString());

                                columnbind.Append(clsArray.sRepoData[x].ToString());

                                if (x < iEndIndex - 1)
                                    columnbind.Append("\r\n");
                            }

                            // Write To File
                            iFileNameIndex++;
                            string sNewFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iMerchant, iFileNameIndex);
                            
                            ucStatusDisplay.SetStatus($"Creating CSV file of {sNewFileName}", Enums.StatusType.Create);
                            Task.Delay(delay); // Asynchronously wait without blocking UI

                            dbFile.DeleteCSV(sNewFileName);
                            dbFile.WriteCSV(sNewFileName, columnbind.ToString());
                        }

                        i++;
                    } while (i < clsArray.sRepoData.Length);
                }
                else
                {
                    iEndIndex = 0;
                }

                // Get Last Part
                if (clsArray.sRepoData.Length > iEndIndex)
                {
                    int iTempStartIndex = iEndIndex;
                    int iTempEndIndex = clsArray.sRepoData.Length;
                    columnbind.Clear();
                    //Debug.WriteLine("iTempStartIndex=" + iTempStartIndex.ToString() + "," + "iTempEndIndex=" + iTempEndIndex.ToString());
                    for (x = iTempStartIndex; x < iTempEndIndex; x++)
                    {
                        //Debug.WriteLine("x=" + x.ToString() + "," + "Data=" + clsArray.sRepoData[x].ToString());

                        columnbind.Append(clsArray.sRepoData[x].ToString());

                        dbFunction.parseDelimitedString(clsArray.sRepoData[x].ToString(), clsDefines.gComma, 0);

                        if (x < iTempEndIndex - 1)
                            columnbind.Append("\r\n");
                    }

                    // Write To File
                    iFileNameIndex++;
                    string sNewFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iMerchant, iFileNameIndex);
                    
                    ucStatusDisplay.SetStatus($"Creating CSV file of {sNewFileName}", Enums.StatusType.Create);
                    Task.Delay(delay); // Asynchronously wait without blocking UI

                    dbFile.DeleteCSV(sNewFileName);
                    dbFile.WriteCSV(sNewFileName, columnbind.ToString());
                }

                // Import IR
                for (i = 1; i <= iFileNameIndex; i++)
                {
                    string sImportFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iMerchant, i);
                    
                    ucStatusDisplay.SetStatus($"Uploading CSV file of {sImportFileName}", Enums.StatusType.Upload);
                    Task.Delay(delay); // Asynchronously wait without blocking UI

                    // Upload File to FTP                                
                    Debug.WriteLine("=>>UploadFile=" + sImportFileName);
                    ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                    ftpClient.delete(clsGlobalVariables.strFTPUploadPath + sImportFileName);
                    ftpClient.upload(clsGlobalVariables.strFTPUploadPath + sImportFileName, @clsGlobalVariables.strFTPLocalPath + sImportFileName);
                    ftpClient.disconnect(); // ftp disconnect

                    Debug.WriteLine("=>>API Call ImportIRDetail=" + sImportFileName);
                    dbAPI.ExecuteAPI("POST", "Import", "CSV", sImportFileName, "Import Merchant Detail", "", "ImportMerchantDetail"); // Process CSV File

                }

                dbFunction.GetResponseTime("Import Merchant Detail");

                Cursor.Current = Cursors.Default; // Back to normal 

            }
        }

        private void InsertRegion()
        {
            int iRowCount = grdList.RowCount;
            int iColCount = grdList.ColumnCount; // exclude Result column
            int iRowIndex = 0;
            string sRowSQL = "";
            string sSQL = "";
            string sRowCSV = "";
            //string sCSV = "";
            int ii = 0;


            int iMerchRegionColIndex = GetHeaderColumnIndex("Area 1 (Metro Manila / Provincial)");
            
            if (iRowCount > 0)
            {
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                // Create File
                string sFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iRegion, 0);
                dbFile.DeleteCSV(sFileName);

                ucStatusDisplay.SetStatus($"Uploading physical file of {sFileName}", Enums.StatusType.Upload);
                Task.Delay(delay); // Asynchronously wait without blocking UI

                for (int i = iRowIndex; i < iRowCount; i++)
                {
                    ii++;

                    string sMerchRegion = StrClean(GetColumnValue(i, iMerchRegionColIndex, grdList).Trim());

                    Debug.WriteLine($"Region: [{sMerchRegion}]");

                    if (sMerchRegion.Length > 0 && sMerchRegion.CompareTo(clsFunction.sDash) != 0)
                    {
                        sSQL = "";
                        sRowSQL = "";
                        sRowSQL = "('" + sMerchRegion + "')";
                        sSQL = sSQL + sRowSQL;

                        sRowCSV = "";
                        sRowCSV = sSQL.Replace("(", "");
                        sRowCSV = sRowCSV.Replace(")", "");
                        sRowCSV = sRowCSV.Replace("'", "");
                        //sCSV = sCSV + sRowCSV + "\n";

                        //Debug.WriteLine("ii=" + ii.ToString() + "-" + "sRowCSV=" + sRowCSV);

                        if (sRowCSV.Length > 0)
                            dbFile.WriteCSV(sFileName, sRowCSV);
                    }

                    ucStatusDisplay.SetStatus($"Processing region [{ii}/{iRowCount}] [{sMerchRegion}]", Enums.StatusType.Processing);
                    Task.Delay(delay); // Asynchronously wait without blocking UI
                }
                
                ucStatusDisplay.SetStatus($"Uploading physical file of {sFileName}", Enums.StatusType.Upload);
                Task.Delay(delay); // Asynchronously wait without blocking UI

                // Upload File to FTP                
                ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                ftpClient.delete(clsGlobalVariables.strFTPUploadPath + sFileName);
                ftpClient.upload(clsGlobalVariables.strFTPUploadPath + sFileName, @clsGlobalVariables.strFTPLocalPath + sFileName);
                ftpClient.disconnect(); // ftp disconnect

                // Process CSV File
                dbAPI.ExecuteAPI("POST", "Import", "CSV", sFileName, "Region Detail", "", "ImportRegionDetail");

                Cursor.Current = Cursors.Default; // Back to normal 

            }
        }

        private void InsertCity()
        {
            int iRowCount = grdList.RowCount;
            int iColCount = grdList.ColumnCount; // exclude Result column
            int iRowIndex = 0;
            string sRowSQL = "";
            string sSQL = "";
            string sRowCSV = "";
            //string sCSV = "";
            int ii = 0;

            int iMerchCityColIndex = GetHeaderColumnIndex("City");

            if (iRowCount > 0)
            {
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                // Delete File            
                string sFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iCity, 0);
                dbFile.DeleteCSV(sFileName);
                
                ucStatusDisplay.SetStatus($"Creating CSV file of {sFileName}", Enums.StatusType.Create);
                Task.Delay(delay); // Asynchronously wait without blocking UI

                for (int i = iRowIndex; i < iRowCount; i++)
                {
                    ii++;
                    string sMerchCity = StrClean(GetColumnValue(i, iMerchCityColIndex, grdList).Trim());

                    Debug.WriteLine($"Province: [{sMerchCity}]");

                    if (sMerchCity.Length > 0 && sMerchCity.CompareTo(clsFunction.sDash) != 0)
                    {
                        sSQL = "";
                        sRowSQL = "";
                        sRowSQL = "('" + sMerchCity + "')";
                        sSQL = sSQL + sRowSQL;

                        sRowCSV = "";
                        sRowCSV = sSQL.Replace("(", "");
                        sRowCSV = sRowCSV.Replace(")", "");
                        sRowCSV = sRowCSV.Replace("'", "");
                        //sCSV = sCSV + sRowCSV + "\n";

                        //Debug.WriteLine("ii=" + ii.ToString() + "-" + "sRowCSV=" + sRowCSV);

                        if (sRowCSV.Length > 0)
                            dbFile.WriteCSV(sFileName, sRowCSV);
                    }
                    
                    ucStatusDisplay.SetStatus($"Processing city [{ii}/{iRowCount}] [{sMerchCity}]", Enums.StatusType.Processing);
                    Task.Delay(delay); // Asynchronously wait without blocking UI

                }
                
                ucStatusDisplay.SetStatus($"Uploading physical file of {sFileName}", Enums.StatusType.Upload);
                Task.Delay(delay); // Asynchronously wait without blocking UI

                // Upload File to FTP                
                ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                ftpClient.delete(clsGlobalVariables.strFTPUploadPath + sFileName);
                ftpClient.upload(clsGlobalVariables.strFTPUploadPath + sFileName, @clsGlobalVariables.strFTPLocalPath + sFileName);
                ftpClient.disconnect(); // ftp disconnect

                // Process CSV File
                dbAPI.ExecuteAPI("POST", "Import", "CSV", sFileName, "Import City Detail", "", "ImportCityDetail");

                Cursor.Current = Cursors.Default; // Back to normal 

            }
        }

        private void InsertProvince()
        {
            int iRowCount = grdList.RowCount;
            int iColCount = grdList.ColumnCount; // exclude Result column
            int iRowIndex = 0;
            string sRowSQL = "";
            string sSQL = "";
            string sRowCSV = "";
            //string sCSV = "";
            int ii = 0;


            int iMerchProvinceColIndex = GetHeaderColumnIndex("Area 1 (Metro Manila / Provincial)");
            
            if (iRowCount > 0)
            {
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                // Create File
                string sFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iProvince, 0);
                dbFile.DeleteCSV(sFileName);
                
                ucStatusDisplay.SetStatus($"Uploading physical file of {sFileName}", Enums.StatusType.Upload);
                Task.Delay(delay); // Asynchronously wait without blocking UI

                for (int i = iRowIndex; i < iRowCount; i++)
                {
                    ii++;
                    string sMerchProvince = StrClean(GetColumnValue(i, iMerchProvinceColIndex, grdList).Trim());

                    Debug.WriteLine($"Region: [{sMerchProvince}]");

                    if (sMerchProvince.Length > 0 && sMerchProvince.CompareTo(clsFunction.sDash) != 0)
                    {
                        sSQL = "";
                        sRowSQL = "";
                        sRowSQL = "('" + StrClean(sMerchProvince) + "')";
                        sSQL = sSQL + sRowSQL;

                        sRowCSV = "";
                        sRowCSV = sSQL.Replace("(", "");
                        sRowCSV = sRowCSV.Replace(")", "");
                        sRowCSV = sRowCSV.Replace("'", "");
                        //sCSV = sCSV + sRowCSV + "\n";

                        //Debug.WriteLine("ii=" + ii.ToString() + "-" + "sRowCSV=" + sRowCSV);

                        if (sRowCSV.Length > 0)
                            dbFile.WriteCSV(sFileName, sRowCSV);
                    }
                    
                    ucStatusDisplay.SetStatus($"Processing province [{ii}/{iRowCount}] [{sMerchProvince}]", Enums.StatusType.Processing);
                    Task.Delay(delay); // Asynchronously wait without blocking UI
                }
                
                ucStatusDisplay.SetStatus($"Uploading physical file of {sFileName}", Enums.StatusType.Upload);
                Task.Delay(delay); // Asynchronously wait without blocking UI

                // Upload File to FTP                
                ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                ftpClient.delete(clsGlobalVariables.strFTPUploadPath + sFileName);
                ftpClient.upload(clsGlobalVariables.strFTPUploadPath + sFileName, @clsGlobalVariables.strFTPLocalPath + sFileName);
                ftpClient.disconnect(); // ftp disconnect

                // Process CSV File
                dbAPI.ExecuteAPI("POST", "Import", "CSV", sFileName, "Province Import Detail", "", "ImportProvinceDetail");

                Cursor.Current = Cursors.Default; // Back to normal 

            }
        }

        private void InsertRegionDetail()
        {
            int iRowCount = grdList.RowCount;
            int iColCount = grdList.ColumnCount; // exclude Result column
            int iRowIndex = 0;
            string sRowSQL = "";
            string sSQL = "";
            string sRowCSV = "";
            //string sCSV = "";
            int ii = 0;


            int iMerchRegionColIndex = GetHeaderColumnIndex("Area 1 (Metro Manila / Provincial)");
            int iMerchProvinceColIndex = GetHeaderColumnIndex("City");

            if (iRowCount > 0)
            {
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                // Create File
                string sFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iRegionProvince, 0);
                dbFile.DeleteCSV(sFileName);

                ucStatusDisplay.SetStatus($"Uploading physical file of {sFileName}", Enums.StatusType.Upload);
                Task.Delay(delay); // Asynchronously wait without blocking UI

                for (int i = iRowIndex; i < iRowCount; i++)
                {
                    ii++;

                    string sMerchRegion = StrClean(GetColumnValue(i, iMerchRegionColIndex, grdList).Trim());
                    string sMerchProvince = StrClean(GetColumnValue(i, iMerchProvinceColIndex, grdList).Trim());

                    Debug.WriteLine($"Region: [{sMerchRegion}]");
                    Debug.WriteLine($"Province: [{sMerchProvince}]");

                    if ((sMerchRegion.Length > 0 && sMerchRegion.CompareTo(clsFunction.sDash) != 0) &&
                       (sMerchProvince.Length > 0 && sMerchProvince.CompareTo(clsFunction.sDash) != 0))
                    {
                        dbFunction.GetIDFromFile("Region", sMerchRegion);
                        int RegionType  = clsSearch.ClassOutFileID;
                        
                        sSQL = "";
                        sRowSQL = "";
                        sRowSQL = "('" +
                        sRowSQL + sRowSQL + "'" + RegionType.ToString() + "'," +                        
                        sRowSQL + sRowSQL + "'" + sMerchProvince + "')";

                        if (sSQL.Length > 0)
                            sSQL = sSQL + "," + sRowSQL;
                        else
                            sSQL = sSQL + sRowSQL;

                        sRowCSV = "";
                        sRowCSV = sSQL.Replace("(", "");
                        sRowCSV = sRowCSV.Replace(")", "");
                        sRowCSV = sRowCSV.Replace("'", "");
                        //sCSV = sCSV + sRowCSV + "\n";

                        Debug.WriteLine("ii=" + ii.ToString() + "-" + "sRowCSV=" + sRowCSV);

                        if (sRowCSV.Length > 0)
                            dbFile.WriteCSV(sFileName, sRowCSV);
                    }

                    ucStatusDisplay.SetStatus($"Processing region/province [{ii}/{iRowCount}] [{sMerchRegion}][{sMerchProvince}]", Enums.StatusType.Processing);
                    Task.Delay(delay); // Asynchronously wait without blocking UI
                }
                
                ucStatusDisplay.SetStatus($"Uploading physical file of {sFileName}", Enums.StatusType.Upload);
                Task.Delay(delay); // Asynchronously wait without blocking UI

                // Upload File to FTP                
                ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                ftpClient.delete(clsGlobalVariables.strFTPUploadPath + sFileName);
                ftpClient.upload(clsGlobalVariables.strFTPUploadPath + sFileName, @clsGlobalVariables.strFTPLocalPath + sFileName);
                ftpClient.disconnect(); // ftp disconnect

                // Process CSV File
                dbAPI.ExecuteAPI("POST", "Import", "CSV", sFileName, "Region Province Detail", "", "ImportRegionProvinceDetail");

                Cursor.Current = Cursors.Default; // Back to normal 

            }
        }

        private string GetColumnValue(int iRowIndex, int iColIndex, DataGridView obj)
        {
            string sColumnValue = clsFunction.sNull;

            try
            {
                sColumnValue = dbFunction.RemoveUnwantedChar(obj.Rows[iRowIndex].Cells[iColIndex].Value.ToString());
            }
            catch (Exception ex)
            {
                sColumnValue = clsFunction.sNull;
                Debug.WriteLine("Exceptional error " + ex.Message);
            }

            return sColumnValue;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            dbFunction.ClearTextBox(this);

            dbFunction.ClearListView(lvwSearch);
            dbFunction.ClearListView(lvwDetail);
            dbFunction.ClearDataGrid(grdDummy);
            dbFunction.ClearDataGrid(grdList);
            dbFunction.ClearDataGrid(dgvProfile);
            dbFunction.ClearDataGrid(dgvRaw);
            dbFunction.ClearDataGrid(grdBulk);
            
            btnLoadFile.Enabled = true;
            btnSave.Enabled = false;
            btnValidate.Enabled = false;
            
            ucStatusDisplay.SetStatus("", Enums.StatusType.Init);
            Task.Delay(delay); // Asynchronously wait without blocking UI

            InitTab();

            lblSelectedRow.Text = "";

            cboSearchClient.Text = clsFunction.sDefaultSelect;

            btnUpdateRawData.Enabled = false;
            btnUpdateListRawData.Enabled = false;
        }

        //private void ValidateDataGrid()
        //{
        //    int iRowCount = 0;
        //    int iColCount = 0;
        //    int i = 0;

        //    iRowCount = grdList.RowCount;
        //    iColCount = grdList.ColumnCount;

        //    int iRequestIDColIndex = GetHeaderColumnIndex("MCC INSTALLATION REQUEST ID");

        //    for (i = 0; i < iRowCount; i++)
        //    {
        //        if (iRowCount - 1 > i)
        //        {
        //            string sRequestID = GetColumnValue(i, iRequestIDColIndex, grdList);

        //            if (sRequestID.Trim().Length > 0)
        //            {
        //                if (dbAPI.isRequestID(sRequestID))
        //                {
        //                    DataGridViewBackColor(i);
        //                }
        //            }
        //        }
        //    }
        //}
        private void DataGridViewBackColor(int iRowIndex)
        {
            grdList.Rows[iRowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
        }

        private bool isValidHeader()
        {
            int i = 0;
            string sList = "";
            string sMapFrom = "";
            bool fExist = true;
            bool isMust = false;

            dbAPI.ExecuteAPI("GET", "View", "Type", "IR", "Mapping", "", "ViewMapping");

            for (i = 0; i < clsArray.MapFrom.Length; i++)
            {
                sMapFrom = clsArray.MapFrom[i];
                isMust = (int.Parse(clsArray.isMust[i]) > 0 ? true : false);
                if (isMust)
                {
                    if (!dbFunction.isHeaderExist(grdDummy, sMapFrom))
                    {
                        sList = sList + sMapFrom + "\n";
                        fExist = false;
                    }
                }
            }

            if (!fExist)
            {
                Debug.WriteLine("Mapping not found...");
                Debug.WriteLine(sList);

                MessageBox.Show("Mapping not found on import file\n\n" + "Header: " + grdDummy.ColumnCount + "\n" + "Mapping: " + clsMapping.ClassRecordCount + "\n\n" + "Mapping list: \n" + sList, "Note", MessageBoxButtons.OK, MessageBoxIcon.Error);

                dbFunction.ClearTextBox(this);
                //dbFunction.ClearComboBox(this);
                //dbFunction.ClearDataGrid(grdTempImport);
                //dbFunction.ClearDataGrid(grdList);
                btnLoadFile.Enabled = true;

                return false;
            }

            return true;
        }

        private void frmImportInstallation_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1: // New
                    if (tabTerminal.SelectedIndex == 1) // Manual entry
                        btnMAdd_Click(this, e);
                    break;
                case Keys.F2: // Sesrch     
                    if (tabTerminal.SelectedIndex == 1) // Manual entry
                        btnMerchantSearch_Click(this, e);
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }
        private void InitTab()
        {   
            switch (tabTerminal.SelectedIndex)
            {
                case 0:                    
                    lblHeader.Text = "INSTALLATION REQUEST" + " " + "[ " + "IMPORT" + " ]";                    
                    break;
                case 1:
                    lblSelectedRow.Text = "";
                    btnUpdateRawData.Enabled = false;
                    lblHeader.Text = "INSTALLATION REQUEST" + " " + "[ " + "MANUAL ENTRY" + " ]";                    
                    break;
            }
        }

        private void tabTerminal_SelectedIndexChanged(object sender, EventArgs e)
        {   
            InitTab();            
        }
        
        private void tabPage1_Click_1(object sender, EventArgs e)
        {

        }
        private void InitButton()
        {
            if (fEdit)
            {
                btnMAdd.Enabled = false;
                btnMSave.Enabled = true;
            }
            else
            {
                btnMAdd.Enabled = true;
                btnMSave.Enabled = false;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            fEdit = false;
            fAddTerminal = false;
            dbFunction.ClearTextBox(this);
          
            dbFunction.ClearListView(lvwSearch);
            dbFunction.ClearListView(lvwDetail);
            dbFunction.ClearListViewItems(lvwMerchant);

            dbFunction.ClearDataGrid(grdDummy);
            dbFunction.ClearDataGrid(grdList);
            dbFunction.ClearDataGrid(dgvProfile);
            dbFunction.ClearDataGrid(dgvRaw);

            InitButton();

            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);

            InitMessageCountLimit();

            SetMKTextBoxBackColor();
            SetPKTextBoxBackColor();
            PKTextBoxReadOnly(true);

            btnDeleteIR.Enabled = false;
            btnRefreshIR.Enabled = false;

            InitStatusTitle(true);

            lblSubHeader.Text = clsFunction.sDash;

            cboSearchClient.Text = cboRequestType.Text = cboPOSType.Text = clsFunction.sDefaultSelect;
            
            isUpdate = true;

            btnMerchantSearch.Enabled = true;
            btnMerchantListSearch.Enabled = false;
            btnClientSearch.Enabled = false;
            btnAddTerminal.Enabled = false;

            dbFunction.SetButtonIconImage(btnMerchantSearch);
            dbFunction.SetButtonIconImage(btnMerchantListSearch);
            dbFunction.SetButtonIconImage(btnClientSearch);

            lblSelectedRow.Text = "";
        }

        private void btnMAdd_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isAdd, clsUser.ClassUserID, 23)) return;

            dbFunction.ClearTextBox(this);
            //dbFunction.ClearComboBox(this);

            btnMAdd.Enabled = false;
            btnMSave.Enabled = true;

            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(true, this);

            //btnMerchantSearch_Click(this, e);

            InitCreatedDateTime();

            InitDate();

            SetMKTextBoxBackColor();

            SetPKTextBoxBackColor();

            txtMerchantName.BackColor = txtClientName.BackColor = clsFunction.EntryBackColor;

            //dbAPI.GenerateID(txtIRNo, txtIRIDNo, "IR Detail", "IR");
            //lblSubHeader.Text = txtIRNo.Text;

            InitStatusTitle(false);

            // ROCKY --  SET DEFAULT: FIX FOR RETRIEVAL OF NEWLY ADDED MERCHANTS
            isUpdate = false;

            //txtIRTID.Focus();

            btnMerchantSearch.Enabled = false;
            btnMerchantListSearch.Enabled = true;
            btnMerchantListSearch.Focus();

            dbFunction.SetButtonIconImage(btnMerchantSearch);
            dbFunction.SetButtonIconImage(btnMerchantListSearch);

        }

        private void btnMerchantSearch_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iMerchant;
            frmSearchField.sHeader = "MERCHANT";
            frmSearchField.isPreview = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                Cursor.Current = Cursors.WaitCursor;

                dbFunction.TextBoxUnLock(true, this);
                dbFunction.ComBoBoxUnLock(true, this);

                txtMerchantID.Text = dbFunction.CheckAndSetNumericValue(clsSearch.ClassParticularID.ToString());
                txtMerchantName.Text = txtMerchantName.Text = clsSearch.ClassParticularName;

                txtIRTID.Text = clsFunction.sNull;
                txtIRMID.Text = clsFunction.sNull;

                //txtIRTID.Text = clsSearch.ClassTID;
                //txtIRMID.Text = clsSearch.ClassMID;
                txtIRNo.Text = clsSearch.ClassIRNo;
                txtIRIDNo.Text = dbFunction.CheckAndSetNumericValue(clsSearch.ClassIRIDNo.ToString());
                txtClientID.Text = dbFunction.CheckAndSetNumericValue(clsSearch.ClassClientID.ToString());

                FillMerchantTextBox();

                // ROCKY --  CHECK STATUS: FIX FOR RETRIEVAL OF NEWLY ADDED MERCHANTS
                if (fEdit && isUpdate)
                {
                    InitStatusTitle(false);
                    btnMAdd.Enabled = false;
                    btnMSave.Enabled = true;
                    btnMerchantSearch.Enabled = false;

                }
                else
                {
                    //txtIRTID.Text = clsFunction.sNull;
                    //txtIRMID.Text = clsFunction.sNull;
                    txtIRNo.Text = clsFunction.sDash;
                    txtIRIDNo.Text = clsFunction.sZero;
                    txtClientID.Text = clsFunction.sZero;

                    //InitStatusTitle(false);
                    lblMainStatus.BackColor = Color.DarkBlue;
                    lblMainStatus.Text = "NEW";

                    //btnGenerate_Click(this, e);

                    lblSubHeader.Text = txtIRNo.Text;

                    //txtIRTID.Text = "";

                    btnMAdd.Enabled = false;
                    btnMSave.Enabled = true;
                    btnMerchantSearch.Enabled = false;

                }

                FillClientTextBox();

                InitCreatedDateTime();

                SetMKTextBoxBackColor();

                SetPKTextBoxBackColor();
                
                dbAPI.FillListViewMultiMerchantInfo(lvwMerchant, txtMerchantID.Text);

                FillProfile_Info();

                FillMerchantServiceInfo();

                cboRequestType_SelectedIndexChanged(this, e);

                cboPOSType_SelectedIndexChanged(this, e);
                
                btnMerchantSearch.Enabled = true;

                btnClientSearch.Enabled = true;
                dbFunction.SetButtonIconImage(btnClientSearch);

                btnAddTerminal.Enabled = true;

                txtClientName.Focus();

                Cursor.Current = Cursors.Default;
            }
        }

        private void FillMerchantServiceInfo()
        {   
            txtStatusID.Text = txtTerminalID.Text = txtSIMID.Text = txtTCount.Text = clsFunction.sZero;
            txtIRStatudDescription.Text = txtTerminalSN.Text = txtSIMSN.Text = clsFunction.sDash;

            if (dbFunction.isValidID(txtIRIDNo.Text))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Merchant Service Info", txtIRIDNo.Text, "Get Info Detail", "", "GetInfoDetail");

                if (dbAPI.isNoRecordFound() == false)
                {
                    txtTCount.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_ServiceCount);
                    txtIRStatudDescription.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_IRStatusDescription);
                    txtStatusID.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_IRStatus);
                    txtTerminalID.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_TerminalID);
                    txtTerminalSN.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_TerminalSN);
                    txtTerminalType.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_TerminalType);
                    txtTerminalModel.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_TerminalModel);
                    txtSIMID.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_SIMID);
                    txtSIMSN.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_SIMSN);
                    txtSIMCarrier.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_SIMCarrier);
                }
            }
            
        }
        
        private void btnAddClient_Click(object sender, EventArgs e)
        {
            frmParticular.iParticularType = clsGlobalVariables.iClient_Type;
            frmParticular frm = new frmParticular();
            frm.ShowDialog();

        }

        private void btnAddTerminal_Click(object sender, EventArgs e)
        {
            frmImportTerminal.iTab = 0;
            frmImportTerminal.iTabSub = 1;
            frmImportTerminal frm = new frmImportTerminal();
            frm.ShowDialog();
        }

        private void btnAddMerchant_Click(object sender, EventArgs e)
        {
            frmParticular.iParticularType = clsGlobalVariables.iMerchant_Type;
            frmParticular frm = new frmParticular();
            frm.ShowDialog();

        }

        private void btnAddSIM_Click(object sender, EventArgs e)
        {
            frmImportSIM frm = new frmImportSIM();
            frm.ShowDialog();
        }

        private void cboSearchClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassClientID = 0;
            if (!cboSearchClient.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Client List", cboSearchClient.Text);
                clsSearch.ClassClientID = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassClientID=" + clsSearch.ClassClientID);
            }

            txtClientID.Text = clsSearch.ClassClientID.ToString();

            if (dbFunction.isValidID(txtClientID.Text))
                FillClientTextBox();
        }

        private void txtIRTID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtIRMID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnMSave_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isUpdate, clsUser.ClassUserID, 23)) return;

            // pad
            txtIRTID.Text = dbFunction.padLeftChar(txtIRTID.Text, clsFunction.sZero, clsFunction.TID_LENGTH);
            txtIRMID.Text = dbFunction.padLeftChar(txtIRMID.Text, clsFunction.sZero, clsFunction.MID_LENGTH);

            if (!ValidateFields(1)) return;

            if (!dbFunction.fSavingConfirm(false)) return;

            dbFunction.GetModifiedByAndDateTime();

            if (!fEdit)
            {
                if (dbAPI.isRecordCount("Search", "IR Detail Check", txtMerchantID.Text + clsFunction.sPipe + txtClientID.Text + clsFunction.sPipe + txtIRTID.Text + clsFunction.sPipe + txtIRMID.Text))
                {
                    MessageBox.Show("Check the following field(s) listed below:" +
                                    "\n\n" +
                                    "*CLIENT NAME: " + txtClientName.Text +
                                     "\n" +
                                    "*MERCHANT NAME: " + txtMerchantName.Text +
                                     "\n" +
                                    "    >TID: " + txtIRTID.Text +
                                    "\n" +
                                    "    >MID: " + txtIRMID.Text +
                                    "\n\n" +
                                    "Already exist in installation request.", clsDefines.FIELD_CHECK_MSG, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                if (dbAPI.isRecordExist("Search", "IRNo Check", txtIRNo.Text))
                {
                    dbFunction.SetMessageBox("Request ID " + txtIRNo.Text + " already exist. Please create again.\n", "Already Exist", clsFunction.IconType.iExclamation);
                    return;
                }

                SaveMIRDetail();
                
            }
            else
            {
                clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtMerchantID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetDatePickerValueToDate(dteReqDate) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetDatePickerValueToDate(dteInstDate) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtIRTID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtIRMID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtClientID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtClientName.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtMerchantPrimaryNum.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtMerchantSecondaryNum.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtAppVersion.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtAppCRC.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtMSetup.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtMRequestFor.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtMInstruction.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtMComments.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtMRemarks.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(clsUser.ClassModifiedBy) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(clsUser.ClassModifiedDateTime) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(cboPOSType.Text);


                //clsSearch.ClassAdvanceSearchValue = clsSearch.ClassAdvanceSearchValue.Replace("&", "AND");

                Debug.WriteLine("clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                dbAPI.ExecuteAPI("PUT", "Update", "Update IR Info", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

            }

            saveProfileInfo();

            dbFunction.SetMessageBox("Manual installation request successfully " + (fEdit ? "updated." : "saved."), (fEdit ? "Updated" : "Saved"), clsFunction.IconType.iInformation);

            btnClear_Click(this, e);

        }

        private bool ValidateFields(int checkIndex)
        {
            Debug.WriteLine("--ValidateFields--");
            Debug.WriteLine("checkIndex="+ checkIndex);

            switch (checkIndex)
            {
                case 1:
                    if (!CheckDateFromTo(dteReqDate, dteInstDate)) return false;

                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iClientID, txtClientID.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantID, txtMerchantID.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iIRNo, txtIRNo.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iTID, txtIRTID.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iMID, txtIRMID.Text)) return false;
                    //if (!dbFunction.isValidEntry(clsFunction.CheckType.iSpecialInstruction, txtMInstruction.Text)) return false;

                    // profile_info
                    if (!dbFunction.isValidDescription(txtProfileDCC.Text) ||
                        !dbFunction.isValidDescription(txtProfileInstallment.Text) ||
                        !dbFunction.isValidDescription(txtProfileMKE.Text) ||
                        !dbFunction.isValidDescription(txtProfilePreAuth.Text) ||
                        !dbFunction.isValidDescription(txtProfileOffSale.Text) ||
                        !dbFunction.isValidDescription(txtProfileOffSaleLimit.Text) ||
                        !dbFunction.isValidDescription(txtProfileTipAdjust.Text) ||
                        !dbFunction.isValidDescription(txtProfileTipAdjustLimit.Text) ||
                        !dbFunction.isValidDescription(txtProfileAddOnDevice.Text) ||
                        !dbFunction.isValidDescription(txtProfileECRIntegration.Text) ||
                        !dbFunction.isValidDescription(txtProfileECRCableType.Text))
                    {
                        dbFunction.SetMessageBox("Please select valid profile value.", "Warning", clsFunction.IconType.iError);
                        return false;
                    }

                    // check tid/mid already exist
                    if (fAddTerminal && dbAPI.isRecordExist("Search", "Merchant TID/MID", txtIRTID.Text + clsDefines.gPipe + txtIRMID.Text + clsDefines.gPipe + txtMerchantName.Text))
                    {
                        dbFunction.SetMessageBox("Merchant details already exist." + "\n\n" +
                            " > Name: " + txtMerchantName.Text + "\n" +
                            " > TID: " + txtIRTID.Text + "\n" +
                            " > MID: " + txtIRMID.Text, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                        return false;
                    }

                    break;
                case 2:
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iClientID, txtClientID.Text)) return false;
                    break;
            }

            

            return true;
        }

        private void label25_Click(object sender, EventArgs e)
        {

        }

        private void InitDate()
        {
            dteReqDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteReqDate, clsFunction.sDateDefaultFormat);

            dteInstDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteInstDate, clsFunction.sDateDefaultFormat);
        }

        private void txtSheetName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtMerchantTelNo_TextChanged(object sender, EventArgs e)
        {

        }
        private void InitMessageCountLimit()
        {
            lblCountRemarks.Text = txtMInstruction.TextLength.ToString() + "/" + iLimit.ToString();
            lblCountComments.Text = txtMComments.TextLength.ToString() + "/" + iLimit.ToString();

            txtMInstruction.MaxLength = iLimit;
            txtMComments.MaxLength = iLimit;

            lblCountTID.Text = txtIRTID.TextLength.ToString() + "/" + clsFunction.TID_LENGTH.ToString();
            lblCountMID.Text = txtIRMID.TextLength.ToString() + "/" + clsFunction.MID_LENGTH.ToString();

            txtIRTID.MaxLength = clsFunction.TID_LENGTH;
            txtIRMID.MaxLength = clsFunction.MID_LENGTH;
        }

        private void txtMRemarks_TextChanged(object sender, EventArgs e)
        {
            lblCountRemarks.Text = txtMInstruction.Text.Length.ToString() + "/" + iLimit.ToString();
        }

        private void txtMComments_TextChanged(object sender, EventArgs e)
        {
            lblCountComments.Text = txtMComments.Text.Length.ToString() + "/" + iLimit.ToString();
        }
        
        private void txtIRNo_Click(object sender, EventArgs e)
        {
            btnMerchantSearch_Click(this, e);
        } 

        private void txtMerchantName_Click(object sender, EventArgs e)
        {
            btnMerchantSearch_Click(this, e);
        }

        private void LoadIR(string StatementType, string SearchBy, string SearchValue)
        {
            int i = 0;
            int ii = 0;
            int iLineNo = 0;

            btnDeleteIR.Enabled = false;
            btnRefreshIR.Enabled = false;
            lvwSearch.Items.Clear();

            dbAPI.ResetAdvanceSearch();
            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassServiceTypeID.ToString() + clsFunction.sPipe +
                                                txtMerchantID.Text + clsFunction.sPipe +
                                                clsSearch.ClassTerminalTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalModelID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalBrandID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalSN + clsFunction.sPipe +
                                                clsSearch.ClassIRStatus.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassIRNo + clsFunction.sPipe +
                                                clsSearch.ClassTID + clsFunction.sPipe +
                                                clsSearch.ClassMID + clsFunction.sPipe +
                                                clsSearch.ClassTerminalStatusType + clsFunction.sPipe +
                                                clsSearch.ClassReqDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassReqDateTo + clsFunction.sPipe +
                                                clsSearch.ClassInstDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassInstDateTo + clsFunction.sPipe +
                                                clsSearch.ClassTADateFrom + clsFunction.sPipe +
                                                clsSearch.ClassTADateTo + clsFunction.sPipe +
                                                clsSearch.ClassFSRDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassFSRDateTo + clsFunction.sPipe +
                                                clsSearch.ClassIRImportDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassIRImportDateTo + clsFunction.sPipe +
                                                clsSearch.ClassSIMSerialNo + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsSearch.ClassClientID + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero;

            Debug.WriteLine("LoadIR::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbFunction.GetRequestTime("Find IR");

            dbAPI.ExecuteAPI("GET", "View", "Advance IR", clsSearch.ClassAdvanceSearchValue, "IR", "", "ViewAdvanceIR");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.TAIDNo.Length > i)
                {
                    ii++;
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.TAIDNo[i].ToString());
                    item.SubItems.Add(clsArray.IRIDNo[i].ToString());
                    item.SubItems.Add(clsArray.IRNo[i].ToString());                    
                    item.SubItems.Add(clsArray.TID[i].ToString());
                    item.SubItems.Add(clsArray.MID[i].ToString());
                    item.SubItems.Add(clsArray.ClientID[i].ToString());
                    item.SubItems.Add(clsArray.ClientName[i].ToString());
                    item.SubItems.Add(clsArray.ServiceNo[i].ToString());
                    item.SubItems.Add(clsArray.RequestNo[i].ToString());
                    item.SubItems.Add(clsArray.JobTypeDescription[i].ToString());
                    item.SubItems.Add(clsArray.JobTypeStatusDescription[i].ToString());

                    lvwSearch.Items.Add(item);
                    
                    i++;
                    
                }

                dbFunction.ListViewAlternateBackColor(lvwSearch);
                btnDeleteIR.Enabled = true;
                btnRefreshIR.Enabled = true;
            }
            else
            {
                //dbFunction.SetMessageBox("No record found.", "Find IR", clsFunction.IconType.iExclamation);                
            }

            dbFunction.GetResponseTime("Find IR");            

            // Focus first item
            if (lvwSearch.Items.Count > 0)
            {
                lvwSearch.FocusedItem = lvwSearch.Items[0];
                lvwSearch.Items[0].Selected = true;
                lvwSearch.Select();
            }            
        }

        private void InitTextBoxLength()
        {
            txtIRTID.MaxLength = clsFunction.TID_LENGTH;
            txtIRMID.MaxLength = clsFunction.MID_LENGTH;
        }

        private void PKTextBoxBackColor(bool isLock)
        {
            if (isLock)
            {
                txtIRNo.BackColor = clsFunction.DisableBackColor;
                txtMerchantName.BackColor = clsFunction.PKBackColor;
                txtClientName.BackColor = clsFunction.PKBackColor;

            }
        }
        private void PKTextBoxReadOnly(bool fReadOnly)
        {
            //txtMSetup.ReadOnly = fReadOnly;
            //txtMRequestFor.ReadOnly = fReadOnly;
            //txtMRemarks.ReadOnly = fReadOnly;
            //txtMComments.ReadOnly = fReadOnly;
            //txtMInstruction.ReadOnly = fReadOnly;

            //if (!fReadOnly)
            //{  
            //    txtMSetup.BackColor = clsFunction.EntryBackColor;
            //    txtMRequestFor.BackColor = clsFunction.EntryBackColor;
            //    txtMRemarks.BackColor = clsFunction.EntryBackColor;
            //    txtMComments.BackColor = clsFunction.EntryBackColor;
            //    txtMInstruction.BackColor = clsFunction.EntryBackColor;
            //}

            txtStatusID.ReadOnly = fReadOnly;
            txtTerminalID.ReadOnly = txtSIMID.ReadOnly = fReadOnly;
            txtTerminalSN.ReadOnly = txtSIMSN.ReadOnly = fReadOnly;
            txtTCount.ReadOnly = fReadOnly;
        }

        private void btnClientSearch_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iClient;
            frmSearchField.sHeader = "CLIENT";
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                txtClientID.Text = clsSearch.ClassParticularID.ToString();
                txtClientName.Text = clsSearch.ClassParticularName;

                FillClientTextBox();
                
                //dbAPI.GenerateID(txtIRNo, "IR Detail", "IR");

                txtMSetup.Focus();
            }
        }

        

        private void txtIRTID_TextChanged(object sender, EventArgs e)
        {
            lblCountTID.Text = txtIRTID.Text.Length.ToString() + "/" + clsFunction.TID_LENGTH.ToString();
        }

        private void txtIRMID_TextChanged(object sender, EventArgs e)
        {
            lblCountMID.Text = txtIRMID.Text.Length.ToString() + "/" + clsFunction.MID_LENGTH.ToString();
        }

        private void txtIRTID_Leave(object sender, EventArgs e)
        {
            txtIRTID.Text = dbFunction.padLeftChar(txtIRTID.Text, clsFunction.sZero, clsFunction.TID_LENGTH);
        }

        private void txtIRMID_Leave(object sender, EventArgs e)
        {
            txtIRMID.Text = dbFunction.padLeftChar(txtIRMID.Text, clsFunction.sZero, clsFunction.MID_LENGTH);
        }
        
        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void btnRefreshIR_Click(object sender, EventArgs e)
        {
            if (dbFunction.isValidID(txtMerchantID.Text))
            {
                clsParticular.ClassParticularID = clsSearch.ClassParticularID;                
                LoadIR("View", "", "");                
            }
        }

        private void btnDeleteIR_Click(object sender, EventArgs e)
        {
            if (clsSearch.ClassIRIDNo > 0)
            {
                if (dbFunction.isValidDescription(clsSearch.ClassJobTypeStatusDescription) &&
                (clsSearch.ClassJobTypeStatusDescription.CompareTo(clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC) == 0 ||
                clsSearch.ClassJobTypeStatusDescription.CompareTo(clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING_DESC) == 0))
                {
                    dbFunction.SetMessageBox("Unable to delete installation request " + clsSearch.ClassIRNo + "." +
                                              "\n\n" +
                                              "Job Type: " + clsSearch.ClassJobTypeDescription + "\n" +
                                              "Status: " + clsSearch.ClassJobTypeStatusDescription
                                              , "Installation Request", clsFunction.IconType.iError);

                    return;
                }

                if (!fDetailConfirm()) return;

                if (dbFunction.isValidID(clsSearch.ClassIRIDNo.ToString()))
                    dbAPI.DeleteIRDetail(clsSearch.ClassIRIDNo.ToString(), clsSearch.ClassIRNo); // Delete from (tblirdetail)

                dbFunction.SetMessageBox("Installation request successfully deleted.", "Deleted", clsFunction.IconType.iInformation);

                btnClear_Click(this, e);
            }
            else
            {
                dbFunction.SetMessageBox("No installation request selected. Please chose item on the list.", "Installation Request", clsFunction.IconType.iError);
            }            
        }

        private void InitListView()
        {
            lvwSearch.View = View.Details;
            lvwSearch.Columns.Add("LINE#", 50, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("TAIDNO", dbFunction.ID_Width(), HorizontalAlignment.Left);
            lvwSearch.Columns.Add("IRIDNO", dbFunction.ID_Width(), HorizontalAlignment.Left);
            lvwSearch.Columns.Add("REQUEST ID", 140, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("TID", 90, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("MID", 130, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("CLIENTID", dbFunction.ID_Width(), HorizontalAlignment.Left);
            lvwSearch.Columns.Add("CLIENT NAME", 130, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("SERVICENO", dbFunction.ID_Width(), HorizontalAlignment.Left);
            lvwSearch.Columns.Add("SERVICE ID", 140, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("JOB TYPE DESC", 140, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("JOB TYPE STATUS", 140, HorizontalAlignment.Left);            
        }

        public bool fDetailConfirm()
        {
            bool fConfirm = true;                      

            string sTemp =
                           clsFunction.sLineSeparator + "\n" +                           
                           "Request ID: " + clsSearch.ClassIRNo + "\n" +
                           clsFunction.sLineSeparator + "\n" +
                           "Merchant Name: " + txtMerchantName.Text + "\n" +
                           "TID: " + clsSearch.ClassTID + "\n" +
                           "MID: " + clsSearch.ClassMID + "\n" +
                           "Client Name :" + clsSearch.ClassClientName + "\n" +
                           clsFunction.sLineSeparator + "\n";


            if (MessageBox.Show("Do you really want to delete selected record?\n" +
                               "\n\n" +
                                sTemp +
                                "\n\n" +
                               "Warning:\nData will permanently deleted." +
                               "\n",
                               "Confirm Delete Service Request?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fConfirm = false;
            }

            return fConfirm;
        }

        private void txtRemarks_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtMRemarks_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtMComments_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void btnAddMerchantImport_Click(object sender, EventArgs e)
        {            
            frmParticular.iParticularType = clsGlobalVariables.iClient_Type;
            frmParticular frm = new frmParticular();
            frm.ShowDialog();

            dbAPI.FillComboBoxClient(cboSearchClient);
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void InitCreatedDateTime()
        {
            DateTime ProcessDateTime = DateTime.Now;
            string sProcessDate = "";
            string sProcessTime = "";

            sProcessDate = ProcessDateTime.ToString("yyyy-MM-dd");
            sProcessTime = ProcessDateTime.ToString("HH:mm:ss");

            txtCreatedDate.Text = sProcessDate;
            txtCreatedTime.Text = sProcessTime;
        }

        private void SetMKTextBoxBackColor()
        {
            txtMerchantName.BackColor = txtClientName.BackColor = clsFunction.MKBackColor;            
        }

        private void SetPKTextBoxBackColor()
        {
            txtIRNo.BackColor = clsFunction.PKBackColor;
        }

        private void FillMerchantTextBox()
        {
            string profile_info;
            string rawdata_info;

            txtMerchantName.Text =          
            txtMerchantAddress.Text =
            txtMerchantProvince.Text =
            txtMerchantRegion.Text =       
            txtMerchantContactPerson.Text =
            txtMerchantTelNo.Text =
            txtMerchantMobile.Text =
            txtMerchantEmail.Text =
            txtIRTID.Text =
            txtIRMID.Text =
            txtMerchantPrimaryNum.Text =
            txtMerchantSecondaryNum.Text = clsFunction.sNull;

            if (dbFunction.isValidID(txtMerchantID.Text))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Merchant Info", dbFunction.CheckAndSetNumericValue(txtMerchantID.Text) + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text), "Get Info Detail", "", "GetInfoDetail");

                dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);

                if (dbAPI.isNoRecordFound() == false)
                {
                    // ROCKY -- MANUAL IR USSUE: FIX RETRIEVAL OF DETAILS OF NEWLY ADDED MERCHANTS
                    if (int.Parse(txtIRIDNo.Text) > 0)
                    {
                        profile_info = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 29);
                        rawdata_info = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 30);

                        txtProfile_Info.Text = profile_info;
                       
                        txtMerchantID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                        txtMerchantName.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                        txtMerchantAddress.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                        txtMerchantProvince.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                        txtMerchantRegion.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);

                        txtMerchantContactPerson.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                        txtMerchantTelNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                        txtMerchantMobile.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                        txtMerchantEmail.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                        txtIRTID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                        txtIRMID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 11);
                        txtMerchantPrimaryNum.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 12);
                        txtMerchantSecondaryNum.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);

                        txtAppVersion.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 14);
                        txtAppCRC.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 15);

                        txtIRIDNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 16);
                        txtIRNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 17);

                        dteReqDate.Value = DateTime.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 18));
                        dteInstDate.Value = DateTime.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 19));

                        txtMInstruction.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 21);
                        
                        cboRequestType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 35);

                        txtMSetup.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 25);
                        txtMRequestFor.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 26);
                        txtMComments.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 27);
                        txtMRemarks.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 28);

                        // POS Type
                        txtRentalFeeID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 22);
                        cboPOSType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 23); // -- Issue Retrieval of POS
                        txtRentalFee.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 24);
                        
                        // rawdata_info
                        if (dbFunction.isValidDescription(rawdata_info))
                            dbFunction.populateListViewFromJsonString(dgvRaw, rawdata_info, clsDefines.ROOTKEY_RAWDATA_INFO, clsDefines.NESTED_OBJECT_VALUES);

                        // profile_info
                        if (dbFunction.isValidDescription(profile_info))
                            dbFunction.populateListViewFromJsonString(dgvProfile, profile_info, clsDefines.ROOTKEY_PROFILE_INFO, clsDefines.gNull);
                        
                        fEdit = true;
                    }
                    else
                    {
                        // ROCKY -- MANUAL IR USSUE: FIX RETRIEVAL OF DETAILS OF NEWLY ADDED MERCHANTS
                        txtMerchantID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                        txtMerchantName.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                        txtMerchantAddress.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                        txtMerchantProvince.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                        txtMerchantRegion.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);

                        txtMerchantContactPerson.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                        txtMerchantTelNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                        txtMerchantMobile.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                        txtMerchantEmail.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);

                        cboRequestType.Text = cboPOSType.Text = clsFunction.sDefaultSelect;

                        fEdit = false;
                    }

                }
            }

        }

        private bool CheckDateFromTo(DateTimePicker objFrom, DateTimePicker objTo)
        {
            bool fValid = true;
            int iResult;

            iResult = DateTime.Compare(DateTime.Parse(objFrom.Value.ToShortDateString()), DateTime.Parse(objTo.Value.ToShortDateString()));

            if (iResult > 0)
                fValid = false;

            if (!fValid)
            {
                MessageBox.Show("[Date From] shouldn't be greater than [Date To]" +
                                        "\n\n" +
                                        "Request Date: " + objFrom.Value.ToString("MM-dd-yyyy") +
                                        Environment.NewLine +
                                        "Installation Date:      " + objTo.Value.ToString("MM-dd-yyyy"), "Date Range", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

            }

            return fValid;
        }

        private void InitStatusTitle(bool isClear)
        {
            if (isClear)
            {
                lblMainStatus.BackColor = Color.DarkBlue;
                lblMainStatus.Text = clsFunction.sDash;
            }
            else
            {
                if (dbFunction.isValidID(txtIRIDNo.Text))
                {
                    lblMainStatus.BackColor = Color.DarkGreen;
                    lblMainStatus.Text = "UPDATE";                    
                }
                else
                {
                    lblMainStatus.BackColor = Color.DarkBlue;
                    lblMainStatus.Text = "NEW";                   
                }
            }

        }

        private void btnAddTerminal_Click_1(object sender, EventArgs e)
        {
            if (!dbFunction.fPromptConfirmation($"Are you sure to add TID for merchant {txtMerchantName.Text}?")) return;

            fEdit = false;

            dbAPI.GenerateID(true, txtIRNo, txtIRIDNo, "IR Detail", clsDefines.CONTROLID_PREFIX_IR);

            fAddTerminal = true;
            txtIRTID.Text = clsFunction.sNull;
            txtIRTID.BackColor = txtIRMID.BackColor = clsFunction.EntryBackColor;        
            txtIRTID.ReadOnly = txtIRMID.ReadOnly = false;
            txtIRTID.Focus();
        }
        
        private void GetHeaderList()
        {
            Debug.WriteLine("--GetHeaderList--");
            string sHeader = clsFunction.sNull;
            string sRowSQL = clsFunction.sNull;
            string sInsert = clsFunction.sNull;
            string sInsertQuery = clsFunction.sNull;
            int iIndex = 0;

            for (int i = 0; i < grdDummy.ColumnCount; i++)
            {
                sHeader = grdDummy.Columns[i].Name.Replace("_", " ");

                //Debug.WriteLine("index="+i+ ",sHeader="+sHeader);
                //Debug.WriteLine("ADD `"+"Reserved"+i+"` VARCHAR(255)"+",");

                //sTemp += "Reserved" + i + ",";

                sInsert = clsFunction.sNull +
                          "INSERT INTO tblmapping (MapFrom, MapTo, Delimeter, ColumnIndex, `Type`)";

                iIndex++;

                sRowSQL = "";
                sRowSQL = " ('" + sHeader + "', " +
                sRowSQL + sRowSQL + " '" + sHeader + "', " +
                sRowSQL + sRowSQL + " '" + "0" + "', " +
                sRowSQL + sRowSQL + " '" + i + "', " +
                sRowSQL + sRowSQL + " '" + "IR" + "') ";

                sInsertQuery = clsFunction.sNull +
                               sInsert +
                               "VALUES " + sRowSQL + ";" + Environment.NewLine;

                Debug.WriteLine(sInsertQuery);

            }

            //Debug.WriteLine("sTemp=" + sTemp);
        }

        private void cboPOSType_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassRentalFeeID = 0;
            if (!cboPOSType.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Rental Fee List", cboPOSType.Text);
                clsSearch.ClassRentalFeeID = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassRentalFeeID=" + clsSearch.ClassRentalFeeID);

                // Get Rental Fee
                txtRentalFee.Text = clsFunction.sDefaultAmount;
                txtRentalFeeID.Text = clsSearch.ClassRentalFeeID.ToString();
                if (dbFunction.isValidID(txtRentalFeeID.Text))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "Rental Fee Info", txtRentalFeeID.Text, "Get Info Detail", "", "GetInfoDetail");

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtRentalFeeID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                        txtRentalFee.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                    }
                }

            }
            
        }

        private void grdList_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            dbFunction.DataGridViewAddLineNo(sender, e, this.Font); // add line#
        }

        private void TotalCount()
        {
            if (grdList.RowCount > 0)
            {
                grdList.Rows[0].Selected = false;
                
                ucStatusDisplay.SetStatus($"{grdList.RowCount} total record to import.", Enums.StatusType.Success);
                Task.Delay(delay); // Asynchronously wait without blocking UI
            }
        }

        private void grdDummy_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            dbFunction.DataGridViewAddLineNo(sender, e, this.Font); // add line#
        }

        private void btnHeaderList_Click(object sender, EventArgs e)
        {
            GetHeaderList();
        }
        
        private void FillProfile_Info()
        {
            Debug.WriteLine("--FillProfile_Info");

            string pProfile_Info = txtProfile_Info.Text;
            Debug.WriteLine("pProfile_info="+ pProfile_Info);

            txtProfileECRIntegration.Text =
            txtProfileInstallment.Text =
            txtProfileMKE.Text =
            txtProfilePreAuth.Text =
            txtProfileOffSale.Text =
            txtProfileOffSaleLimit.Text =
            txtProfileTipAdjust.Text =
            txtProfileTipAdjustLimit.Text =
            txtProfileAddOnDevice.Text =
            txtProfileECRIntegration.Text =
            txtProfileECRCableType.Text = clsFunction.sDefaultSelect;

            try
            {
                jsonObj obj = JsonConvert.DeserializeObject<jsonObj>(pProfile_Info);

                if (pProfile_Info.Length > 0 && obj.profile_info != null)
                {
                    string pDataToParse = dbAPI.GetValueFromJSONString(pProfile_Info, "profile_info"); ;
                    Debug.WriteLine("pDataToParse=" + pDataToParse);

                    txtProfileDCC.Text = dbAPI.GetValueFromJSONString(pDataToParse, clsDefines.IR_DCC_PROFILE_INFO);
                    txtProfileInstallment.Text = dbAPI.GetValueFromJSONString(pDataToParse, clsDefines.IR_INSTALLMENT_PROFILE_INFO);
                    txtProfileMKE.Text = dbAPI.GetValueFromJSONString(pDataToParse, clsDefines.IR_MKE_PROFILE_INFO);
                    txtProfilePreAuth.Text = dbAPI.GetValueFromJSONString(pDataToParse, clsDefines.IR_PREAUTH_PROFILE_INFO);
                    txtProfileOffSale.Text = dbAPI.GetValueFromJSONString(pDataToParse, clsDefines.IR_OFFLINESALE_PROFILE_INFO);
                    txtProfileOffSaleLimit.Text = dbAPI.GetValueFromJSONString(pDataToParse, clsDefines.IR_OFFLINESALELIMIT_PROFILE_INFO);
                    txtProfileTipAdjust.Text = dbAPI.GetValueFromJSONString(pDataToParse, clsDefines.IR_TIPADJUST_PROFILE_INFO);
                    txtProfileTipAdjustLimit.Text = dbAPI.GetValueFromJSONString(pDataToParse, clsDefines.IR_TIPADJUSTLIMIT_PROFILE_INFO);
                    txtProfileAddOnDevice.Text = dbAPI.GetValueFromJSONString(pDataToParse, clsDefines.IR_ADDONDEVICE_PROFILE_INFO);
                    txtProfileECRIntegration.Text = dbAPI.GetValueFromJSONString(pDataToParse, clsDefines.IR_ECRINTEGRATION_PROFILE_INFO);
                    txtProfileECRCableType.Text = dbAPI.GetValueFromJSONString(pDataToParse, clsDefines.IR_ECRCABLETYPE_PROFILE_INFO);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exceptional error " + ex.Message);
            }
            
        }
        
        private void saveProfileInfo()
        {
            Debug.WriteLine("--saveProfileInfo--");

            if (dbFunction.isValidID(txtIRIDNo.Text))
            {
                Dictionary<string, object> jsonData = new Dictionary<string, object>();

                jsonData.Add(clsDefines.IR_DCC_PROFILE_INFO, txtProfileDCC.Text);
                jsonData.Add(clsDefines.IR_INSTALLMENT_PROFILE_INFO, txtProfileInstallment.Text);
                jsonData.Add(clsDefines.IR_MKE_PROFILE_INFO, txtProfileMKE.Text);
                jsonData.Add(clsDefines.IR_PREAUTH_PROFILE_INFO, txtProfilePreAuth.Text);
                jsonData.Add(clsDefines.IR_OFFLINESALE_PROFILE_INFO, txtProfileOffSale.Text);
                jsonData.Add(clsDefines.IR_OFFLINESALELIMIT_PROFILE_INFO, txtProfileOffSaleLimit.Text);
                jsonData.Add(clsDefines.IR_TIPADJUST_PROFILE_INFO, txtProfileTipAdjust.Text);
                jsonData.Add(clsDefines.IR_TIPADJUSTLIMIT_PROFILE_INFO, txtProfileTipAdjustLimit.Text);
                jsonData.Add(clsDefines.IR_ADDONDEVICE_PROFILE_INFO, txtProfileAddOnDevice.Text);
                jsonData.Add(clsDefines.IR_ECRINTEGRATION_PROFILE_INFO, txtProfileECRIntegration.Text);
                jsonData.Add(clsDefines.IR_ECRCABLETYPE_PROFILE_INFO, txtProfileECRCableType.Text);

                string jsonString = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
                Debug.WriteLine("jsonString=" + jsonString);

                dbAPI.ExecuteAPI("PUT", "Update", "Update Profile Info", txtIRIDNo.Text + clsDefines.gPipe + jsonString, "", "", "UpdateCollectionDetail");
            }
            
        }

        private class DataGridViewRowData
        {
            public Dictionary<string, object> Values { get; set; }
        }
        
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (!dbFunction.fPromptConfirmation("Are you sure to let the system generate REQUEST ID?")) return;

            dbAPI.GenerateID(true, txtIRNo, txtIRIDNo, "IR Detail", clsDefines.CONTROLID_PREFIX_IR);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (dbFunction.isValidID(txtTCount.Text))
            {
                dbFunction.SetMessageBox("Unable to reset service due to has active service.", "IR", clsFunction.IconType.iError);
            }
            else
            {
                if (dbFunction.isValidID(txtIRIDNo.Text))
                {
                    if (!dbFunction.fSavingConfirm(true)) return;

                    dbAPI.ExecuteAPI("PUT", "Update", "Reset Merchant Service", txtIRIDNo.Text, "", "", "UpdateCollectionDetail");

                    dbFunction.SetMessageBox("Merchant " + txtMerchantName.Text + " reset service successfully.", "IR", clsFunction.IconType.iInformation);
                }
            }
            
        }
        
        private void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            if (!dbFunction.fPromptConfirmation("Are you sure to update merchant tenure?")) return;

            if (dbFunction.isValidID(txtIRIDNo.Text))
            {
                saveProfileInfo();

                // reload profile data
                dbAPI.ExecuteAPI("GET", "Search", "Merchant Info", dbFunction.CheckAndSetNumericValue(txtMerchantID.Text) + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text), "Get Info Detail", "", "GetInfoDetail");
                string profile_info = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 29);
                if (dbFunction.isValidDescription(profile_info))
                    dbFunction.populateListViewFromJsonString(dgvProfile, profile_info, clsDefines.ROOTKEY_PROFILE_INFO, clsDefines.gNull);

                dbFunction.SetMessageBox("Merchant tenure updated.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
            }
        }

        private void btnMerchantListSearch_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iIRMerchantList;
            frmSearchField.sHeader = "MERCHANT";
            frmSearchField.isPreview = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                Cursor.Current = Cursors.WaitCursor;

                dbFunction.ClearTextBox(this);
                dbFunction.TextBoxUnLock(true, this);
                
                txtMerchantID.Text = clsSearch.ClassParticularID.ToString();
                txtMerchantName.Text = clsSearch.ClassParticularName;
                
                modelParticular = modelParticular.setParticularInfo(int.Parse(txtMerchantID.Text));
                txtMerchantName.Text = modelParticular.Name;
                txtMerchantAddress.Text = modelParticular.Address;
                txtMerchantContactPerson.Text = modelParticular.ContactPerson;
                txtMerchantMobile.Text = modelParticular.ContactNumber;
                txtMerchantRegion.Text = modelParticular.Region;
                txtMerchantProvince.Text = modelParticular.Province;
                txtMerchantEmail.Text = modelParticular.Email;
                
                dbAPI.FillListViewMultiMerchantInfo(lvwMerchant, txtMerchantID.Text);

                btnClientSearch.Enabled = true;
                dbFunction.SetButtonIconImage(btnClientSearch);

                btnMAdd.Enabled = false;
                btnMSave.Enabled = true;

                btnAddTerminal.Enabled = true;
                
                Cursor.Current = Cursors.WaitCursor;
            }
        }

        private void processDataGridView(DataGridView dgv)
        {
            if (dbFunction.isValidCount(grdList.Rows.Count))
            {
                Cursor.Current = Cursors.WaitCursor;

                foreach (DataGridViewRow row in grdList.Rows)
                {
                    if (row.IsNewRow) continue;

                    int pLineNo = row.Index;

                    // Update label directly (no Invoke needed anymore)
                    lblSelectedRow.Text = $"Selected line# {pLineNo + 1}/{grdList.Rows.Count}";
                    Application.DoEvents(); // 🔥 allow UI refresh

                    string rawdata_info = dbFunction.genJSONFormat(
                        grdList, pLineNo, "", clsDefines.NESTED_OBJECT_VALUES);

                    string profile_info = getProfile_InfoFromJson(
                        rawdata_info, "", clsDefines.NESTED_OBJECT_VALUES);

                    // Direct UI update
                    dbFunction.populateListViewFromJsonString(
                        dgvRaw, rawdata_info, "", clsDefines.NESTED_OBJECT_VALUES);

                    dbFunction.populateListViewFromJsonString(
                        dgvProfile, profile_info, "", "");

                    clsSearch.ClassRawDataInfo = rawdata_info;
                    clsSearch.ClassProfileDataInfo = profile_info;

                    updateRawData(false);
                }

                Cursor.Current = Cursors.Default;
            }
        }

        private string getProfile_InfoFromJson(string rawdata_info, string token = "", string nestedObject = "")
        {
            string output = "";

            Debug.WriteLine("--getProfile_InfoFromJson--");
            Debug.WriteLine("token=" + token);
            Debug.WriteLine("nestedObject=" + nestedObject);
            Debug.WriteLine("rawdata_info="+ rawdata_info);
            
            var tagValueMap = new Dictionary<string, string>
            {
                { clsDefines.IR_DCC_PROFILE_INFO, dbFunction.getJSONTagValue(rawdata_info, clsDefines.IR_DCC_PROFILE_INFO, token, nestedObject) },
                { clsDefines.IR_INSTALLMENT_PROFILE_INFO, dbFunction.getJSONTagValue(rawdata_info, clsDefines.IR_INSTALLMENT_PROFILE_INFO, "", nestedObject) },
                { clsDefines.IR_MKE_PROFILE_INFO, dbFunction.getJSONTagValue(rawdata_info, clsDefines.IR_MKE_PROFILE_INFO, "", nestedObject) },
                { clsDefines.IR_PREAUTH_PROFILE_INFO, dbFunction.getJSONTagValue(rawdata_info, clsDefines.IR_PREAUTH_PROFILE_INFO, "", nestedObject) },
                { clsDefines.IR_OFFLINESALE_PROFILE_INFO, dbFunction.getJSONTagValue(rawdata_info, clsDefines.IR_OFFLINESALE_PROFILE_INFO, "", nestedObject) },
                { clsDefines.IR_OFFLINESALELIMIT_PROFILE_INFO, dbFunction.getJSONTagValue(rawdata_info, clsDefines.IR_OFFLINESALELIMIT_PROFILE_INFO, "", nestedObject) },
                { clsDefines.IR_TIPADJUST_PROFILE_INFO, dbFunction.getJSONTagValue(rawdata_info, clsDefines.IR_TIPADJUST_PROFILE_INFO, "", nestedObject) },
                { clsDefines.IR_TIPADJUSTLIMIT_PROFILE_INFO, dbFunction.getJSONTagValue(rawdata_info, clsDefines.IR_TIPADJUSTLIMIT_PROFILE_INFO, "", nestedObject) },
                { clsDefines.IR_ADDONDEVICE_PROFILE_INFO, dbFunction.getJSONTagValue(rawdata_info, clsDefines.IR_ADDONDEVICE_PROFILE_INFO, "", nestedObject) },
                { clsDefines.IR_ECRINTEGRATION_PROFILE_INFO, dbFunction.getJSONTagValue(rawdata_info, clsDefines.IR_ECRINTEGRATION_PROFILE_INFO, "", nestedObject) },
                { clsDefines.IR_ECRCABLETYPE_PROFILE_INFO, dbFunction.getJSONTagValue(rawdata_info, clsDefines.IR_ECRCABLETYPE_PROFILE_INFO, "", nestedObject) }
            };

            string jsonString = dbFunction.convertToJson(tagValueMap);
            Debug.WriteLine(jsonString);
            output = jsonString;

            return output;
        }
        
        private void btnUpdateRawData_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidDescriptionEntry(cboSearchClient.Text, "Client" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            FillClientTextBox();

            if (!dbFunction.fPromptConfirmation("Are you sure you want to update the raw data for the selected record?")) return;

            updateRawData(true);
        }

        private void updateRawData(bool isPrompt)
        {
            string pSearchValue = "";

            //if (!dbFunction.isValidDescriptionEntry(cboSearchClient.Text, "Client" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            //if (!dbFunction.isValidDescriptionEntry(txtProfileConfigInfo.Text, "Client" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            //Debug.WriteLine("--updateRawData--");
            //Debug.WriteLine($"isPrompt=[{isPrompt}]");
            //Debug.WriteLine($"isPrompt=[{isPrompt}]");
            //Debug.WriteLine($"clsSearch.ClassRawDataInfo=[{clsSearch.ClassRawDataInfo}]");
            //Debug.WriteLine($"clsSearch.ClassProfileDataInfo=[{clsSearch.ClassProfileDataInfo}]");

            try
            {
                if (dbFunction.isValidDescription(clsSearch.ClassRawDataInfo) && dbFunction.isValidDescription(clsSearch.ClassProfileDataInfo))
                {
                    clsSearch.ClassMerchantName = dbFunction.getJSONTagValue(clsSearch.ClassRawDataInfo, clsDefines.IR_MERCHANT_NAME, "", clsDefines.NESTED_OBJECT_VALUES);
                    clsSearch.ClassIRNo = dbFunction.getJSONTagValue(clsSearch.ClassRawDataInfo, clsDefines.IR_REQUEST_ID, "", clsDefines.NESTED_OBJECT_VALUES);
                    clsSearch.ClassTID = dbFunction.padLeftChar(dbFunction.getJSONTagValue(clsSearch.ClassRawDataInfo, clsDefines.IR_TID, "", clsDefines.NESTED_OBJECT_VALUES), clsFunction.sZero, clsFunction.TID_LENGTH);
                    clsSearch.ClassMID = dbFunction.padLeftChar(dbFunction.getJSONTagValue(clsSearch.ClassRawDataInfo, clsDefines.IR_MID, "", clsDefines.NESTED_OBJECT_VALUES), clsFunction.sZero, clsFunction.MID_LENGTH);

                    pSearchValue = $"{clsSearch.ClassTID}{clsDefines.gPipe}{clsSearch.ClassMID}{clsDefines.gPipe}{clsSearch.ClassIRNo}";

                    dbFunction.parseDelimitedString(pSearchValue, clsDefines.gPipe, 0);

                    string pJSONString = dbAPI.getInfoDetailJSON("Search", "Merchant Basic Info", pSearchValue);

                    dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                    //ucStatusDisplay.SetStatus($"Processing Request ID[{clsSearch.ClassIRNo}]", Enums.StatusType.Processing);

                    // update rawdata/profile
                    if (dbFunction.isValidDescription(pJSONString))
                    {
                        string pProfile_Info = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Profile_Info);
                        string pRawData_Info = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_RawData_Info);

                        clsSearch.ClassIRIDNo = int.Parse(dbFunction.CheckAndSetNumericValue(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_IRIDNo)));

                        //if (isPrompt)
                        //    if (!dbFunction.fPromptConfirmation("Are you sure to update JSON data?")) return;

                        pSearchValue = $"{clsSearch.ClassIRIDNo}{clsDefines.gPipe}{clsSearch.ClassIRNo}{clsFunction.sPipe}{clsSearch.ClassTID}{clsDefines.gPipe}{clsSearch.ClassMID}";
                        dbFunction.parseDelimitedString(pSearchValue, clsDefines.gPipe, 0);

                        if (!dbFunction.isValidDescription(pRawData_Info))
                        {
                            //Debug.WriteLine($"Inserting rawdata_info IRIDNo[{clsSearch.ClassIRIDNo}] IRNo:[{clsSearch.ClassIRNo}] TID:[{clsSearch.ClassTID}] MID:[{clsSearch.ClassMID}]");

                            ucStatusDisplay.SetStatus($"Inserting rawdata_info Request ID: [{clsSearch.ClassIRNo}]", Enums.StatusType.Processing);

                            dbAPI.ExecuteAPI("PUT", "Update", "Update Raw Data", $"{clsSearch.ClassIRIDNo}{clsDefines.gPipe}{clsSearch.ClassIRNo}{clsDefines.gPipe}{clsSearch.ClassRawDataInfo}", "", "", "UpdateCollectionDetail");
                        }
                        else
                        {
                            if (chkOverwrite.Checked)
                            {
                                //Debug.WriteLine($"Updating rawdata_info IRIDNo[{clsSearch.ClassIRIDNo}] IRNo:[{clsSearch.ClassIRNo}] TID:[{clsSearch.ClassTID}] MID:[{clsSearch.ClassMID}]");

                                ucStatusDisplay.SetStatus($"Updating rawdata_info Request ID: [{clsSearch.ClassIRNo}]", Enums.StatusType.Processing);

                                dbAPI.ExecuteAPI("PUT", "Update", "Update Raw Data 2", $"{clsSearch.ClassTID}{clsDefines.gPipe}{clsSearch.ClassMID}{clsDefines.gPipe}{clsSearch.ClassRawDataInfo}", "", "", "UpdateCollectionDetail");
                            }
                        }

                        if (!dbFunction.isValidDescription(pProfile_Info))
                        {
                            //Debug.WriteLine($"Inserting profile_info IRIDNo[{clsSearch.ClassIRIDNo}] IRNo:[{clsSearch.ClassIRNo}] TID:[{clsSearch.ClassTID}] MID:[{clsSearch.ClassMID}]");

                            ucStatusDisplay.SetStatus($"Inserting profile_info Request ID: [{clsSearch.ClassIRNo}]", Enums.StatusType.Processing);

                            dbAPI.ExecuteAPI("PUT", "Update", "Update Profile", $"{clsSearch.ClassIRIDNo}{clsDefines.gPipe}{clsSearch.ClassIRNo}{clsDefines.gPipe}{clsSearch.ClassProfileDataInfo}", "", "", "UpdateCollectionDetail");
                        }
                        else
                        {

                            if (chkOverwrite.Checked)
                            {
                                //Debug.WriteLine($"Updating profile_info IRIDNo[{clsSearch.ClassIRIDNo}] IRNo:[{clsSearch.ClassIRNo}] TID:[{clsSearch.ClassTID}] MID:[{clsSearch.ClassMID}]");

                                ucStatusDisplay.SetStatus($"Updating profile_info Request ID: [{clsSearch.ClassIRNo}]", Enums.StatusType.Processing);

                                dbAPI.ExecuteAPI("PUT", "Update", "Update Profile 2", $"{clsSearch.ClassTID}{clsDefines.gPipe}{clsSearch.ClassMID}{clsDefines.gPipe}{clsSearch.ClassProfileDataInfo}", "", "", "UpdateCollectionDetail");
                            }
                        }

                        // update profile_config_info (tblparticular)
                        if (dbFunction.isValidID(txtClientID.Text) && dbFunction.isValidDescription(txtProfileConfigInfo.Text))
                        {
                            clsSearch.ClassMerchantID = int.Parse(dbFunction.CheckAndSetNumericValue(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MerchantID)));

                            ucStatusDisplay.SetStatus($"Updating Particular profile_info Request ID: [{clsSearch.ClassIRNo}]", Enums.StatusType.Processing);

                            dbAPI.ExecuteAPI("PUT", "Update", "Update Profile Config Info", $"{clsSearch.ClassMerchantID}{clsDefines.gPipe}{txtProfileConfigInfo.Text}", "", "", "UpdateCollectionDetail");
                        }

                        if (isPrompt)
                            dbFunction.SetMessageBox("JSON data update complete.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);

                    }
                    else
                    {
                        if (isPrompt)
                            dbFunction.SetMessageBox($"Request ID {clsSearch.ClassIRNo} does not yet exist.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    }

                }
                else
                {
                    //dbFunction.SetMessageBox("Profile config and raw data must not be blank", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);

                    if (isPrompt)

                        Debug.WriteLine($"Failed to update Client:[{cboSearchClient.Text}], IRIDNo:[{clsSearch.ClassIRIDNo}], Request ID:[{clsSearch.ClassIRNo}], TID:[{clsSearch.ClassTID}], MID:[{clsSearch.ClassMID}], Merchant:[{clsSearch.ClassMerchantName}]");

                    dbFunction.SetMessageBox(
                                $"Profile config and raw data must not be blank.\n" +
                                $"Client: {cboSearchClient.Text}\n" +
                                $"IRIDNo: {clsSearch.ClassIRIDNo}\n" +
                                $"Request ID: {clsSearch.ClassIRNo}\n" +
                                $"Name: {clsSearch.ClassMerchantName}\n" +
                                $"TID: {clsSearch.ClassTID}\n" +
                                $"MID: {clsSearch.ClassMID}\n\n" +
                                $"{clsDefines.CONTACT_ADMIN_MESSAGE}",
                                $"{clsDefines.FIELD_CHECK_MSG}-Update",
                                clsFunction.IconType.iError
                            );
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"updateRawData, error={ex.Message}");
            }            
        }

        private void cboRequestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassRequestTypeID = 0;
            if (!cboRequestType.Text.Equals(clsFunction.sDefaultSelect))
            {
                clsSearch.ClassRequestTypeID = dbFunction.getFileID(cboRequestType, "All Type");
                clsSearch.ClassRequestTypeID = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassRequestTypeID=" + clsSearch.ClassRequestTypeID);
                
            }
        }

        public void ComputeSummary(DataGridView grid)
        {
            int totalInput = 0;
            int ready = 0;
            int notFound = 0;
            int restricted = 0;

            HashSet<string> uniqueSN = new HashSet<string>();

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;

                totalInput++;

                // ✅ get RequetID (column index 1)
                string unique = row.Cells[0].Value?.ToString(); // RequestId
                if (!string.IsNullOrWhiteSpace(unique))
                {
                    unique = Regex.Replace(unique, @"[\r\n,\s]", "");
                    uniqueSN.Add(unique);
                }

                // ✅ get RESULT column
                string result = row.Cells["RESULT"].Value?.ToString()?.ToUpper();

                switch (result)
                {
                    case "READY TO IMPORT":
                    case "READY TO DELETE":
                    case "NEW ITEM":
                        ready++;
                        break;

                    case "NOT FOUND":
                    case "NOT IN SYSTEM":
                        notFound++;
                        break;

                    case "RESTRICTED":
                    case "IN USE":
                        restricted++;
                        break;
                }
            }

            // ✅ assign to UI
            txtTotalInput.Text = totalInput.ToString();
            txtTotalUnique.Text = uniqueSN.Count.ToString();
            txtReady.Text = ready.ToString();
            txtNotFound.Text = notFound.ToString();
            txtRestricted.Text = restricted.ToString();
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            StringBuilder sbReuestId = new StringBuilder();
            StringBuilder sbTID = new StringBuilder();
            StringBuilder sbMID = new StringBuilder();
            string mode = "Import";
            string import_type = "Service Installation";

            if (!dbFunction.fPromptConfirmation($"Are you sure to validate the records on list?")) return;

            Cursor.Current = Cursors.WaitCursor;

            ucStatusDisplay.SetStatus($"Validating list...", Enums.StatusType.Processing);

            btnSave.Enabled = false;

            // ✅ build CSV
            foreach (DataGridViewRow row in grdList.Rows)
            {
                if (row.IsNewRow) continue;

                var requestId = row.Cells[0].Value?.ToString(); // Request ID
                var mid = dbFunction.formattedMID( row.Cells[12].Value?.ToString()); // MID
                var tid = dbFunction.formattedTID(row.Cells[13].Value?.ToString()); // TID                
                if (string.IsNullOrWhiteSpace(requestId)) continue;

                // Request ID
                requestId = Regex.Replace(requestId, @"[\r\n,\s]", "");
                sbReuestId.Append(requestId + ",");

                // MID
                mid = Regex.Replace(mid, @"[\r\n,\s]", "");
                sbMID.Append(mid + ",");

                // TID
                tid = Regex.Replace(tid, @"[\r\n,\s]", "");
                sbTID.Append(tid + ",");

            }

            string result_requestId = sbReuestId.ToString().TrimEnd(',');
            string result_MID = sbMID.ToString().TrimEnd(',');
            string result_TID = sbTID.ToString().TrimEnd(',');

            Debug.WriteLine($"result_requestId=[{result_requestId}]");
            Debug.WriteLine($"result_MID=[{result_MID}]");
            Debug.WriteLine($"result_TID=[{result_TID}]");

            // ✅ call API
            dbAPI.ExecuteAPI("GET", "View", "Service Bulk Cross-Check List",
                $"{mode}{clsFunction.sPipe}{import_type}{clsFunction.sPipe}{result_requestId}",
                "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;
            if (dbAPI.isNoRecordFound()) return;

            // ✅ 1. Add RESULT column if not exists
            if (!grdList.Columns.Contains("RESULT"))
            {
                grdList.Columns.Add("RESULT", "RESULT");
            }

            // ✅ 2. Build dictionary from API response (IRNo → Result)
            Dictionary<string, string> resultMap = new Dictionary<string, string>();

            for (int i = 0; i < clsArray.ID.Length; i++)
            {
                string json = clsArray.detail_info[i];

                string unique = dbAPI.GetValueFromJSONString(json, clsDefines.TAG_IRNO);
                string status = dbAPI.GetValueFromJSONString(json, clsDefines.TAG_Result);

                if (!string.IsNullOrEmpty(unique))
                {
                    resultMap[unique] = status;
                }
            }

            // ✅ 3. Apply result to grid
            foreach (DataGridViewRow row in grdList.Rows)
            {
                if (row.IsNewRow) continue;

                string requestId = row.Cells[0].Value?.ToString();
                if (string.IsNullOrWhiteSpace(requestId)) continue;

                requestId = Regex.Replace(requestId, @"[\r\n,\s]", "");

                if (resultMap.ContainsKey(requestId))
                {
                    string status = resultMap[requestId];

                    var cell = row.Cells["RESULT"];
                    cell.Value = status;

                    // ✅ color styling
                    switch (status?.ToUpper())
                    {
                        case "READY TO IMPORT":
                        case "READY TO DELETE":
                        case "NEW ITEM":
                            cell.Style.BackColor = Color.LightGreen;
                            cell.Style.ForeColor = Color.Black;
                            break;

                        case "RESTRICTED":
                        case "IN USE":
                            cell.Style.BackColor = Color.Red;
                            cell.Style.ForeColor = Color.White;
                            break;

                        case "NOT FOUND":
                        case "NOT IN SYSTEM":
                            cell.Style.BackColor = Color.LightBlue;
                            cell.Style.ForeColor = Color.Black;
                            break;

                        default:
                            cell.Style.BackColor = Color.White;
                            cell.Style.ForeColor = Color.Black;
                            break;
                    }
                }
            }

            // update count
            ComputeSummary(grdList);

            btnSave.Enabled = true;
            btnValidate.Enabled = false;

            Cursor.Current = Cursors.Default;

            ucStatusDisplay.SetStatus($"Validating list...completed", Enums.StatusType.Processing);

            dbFunction.SetMessageBox("Validate list complete. Check summary count.", lblHeader.Text, clsFunction.IconType.iInformation);
        }

        private void btnCopyClipboard_Click(object sender, EventArgs e)
        {
            dbFunction.CopyGridToClipboard(grdList);

            dbFunction.SetMessageBox($"Data list copied to clipboard!", lblHeader.Text, clsFunction.IconType.iInformation);
        }

        private async void btnUpdateListRawData_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidDescriptionEntry(cboSearchClient.Text, "Client" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            FillClientTextBox();

            if (!dbFunction.fPromptConfirmation("Are you sure you want to update the raw data for all records in the list?")) return;

            if (dbFunction.isValidCount(grdList.Rows.Count))
            {
                Cursor.Current = Cursors.WaitCursor;

                await Task.Run(() =>
                {
                    foreach (DataGridViewRow row in grdList.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            int pLineNo = row.Index;

                            this.Invoke(new Action(() =>
                            {
                                lblSelectedRow.Text = $"Selected line# {pLineNo + 1}/{grdList.Rows.Count}";
                            }));

                            string rawdata_info = dbFunction.genJSONFormat(grdList, pLineNo, "", clsDefines.NESTED_OBJECT_VALUES);
                            string profile_info = getProfile_InfoFromJson(rawdata_info, "", clsDefines.NESTED_OBJECT_VALUES);

                            this.Invoke(new Action(() =>
                            {
                                dbFunction.populateListViewFromJsonString(dgvRaw, rawdata_info, "", clsDefines.NESTED_OBJECT_VALUES);
                                dbFunction.populateListViewFromJsonString(dgvProfile, profile_info, "", "");

                                clsSearch.ClassRawDataInfo = rawdata_info;
                                clsSearch.ClassProfileDataInfo = profile_info;
                            }));

                            updateRawData(false);
                        }
                    }
                });

                Cursor.Current = Cursors.Default;

                dbFunction.SetMessageBox("Update raw data completed.", lblHeader.Text, clsFunction.IconType.iInformation);
            }
        }

        public void populateBulkDataGrid(DataGridView grdList, DataGridView grdBulk)
        {
            // Clear existing
            grdBulk.Rows.Clear();
            grdBulk.Columns.Clear();

            // Add checkbox first
            DataGridViewHelper.AddCheckBoxColumn(grdBulk, "chkSelect", 0);

            // Add Request ID column (from grdList)
            grdBulk.Columns.Add("Request ID", "Request ID");
            grdBulk.Columns["Request ID"].Width = 150;

            // Add new columns
            grdBulk.Columns.Add("ProfileInfo", "Profile Info");
            grdBulk.Columns.Add("RawDataInfo", "RawData Info");

            grdBulk.SuspendLayout();

            // Copy rows
            foreach (DataGridViewRow row in grdList.Rows)
            {
                if (row.IsNewRow) continue;

                int idx = grdBulk.Rows.Add();

                grdBulk.Rows[idx].Cells["Request ID"].Value =
                    row.Cells["Request ID"].Value;

                // 🔥 default empty (or map if exists)
                grdBulk.Rows[idx].Cells["ProfileInfo"].Value = "";
                grdBulk.Rows[idx].Cells["RawDataInfo"].Value = "";
            }

            grdBulk.ResumeLayout();
            grdBulk.Refresh();

        }

        private void btnCheckBulk_Click(object sender, EventArgs e)
        {
            grdBulk.SuspendLayout();

            foreach (DataGridViewColumn col in grdBulk.Columns)
            {
                Console.WriteLine(col.Name);
            }

            foreach (DataGridViewRow row in grdBulk.Rows)
            {
                if (row.IsNewRow) continue;

                bool isChecked = Convert.ToBoolean(row.Cells["chkSelect"].Value ?? false);

                if (!isChecked) continue;

                // Get values
                var requestId = row.Cells["Request ID"].Value?.ToString() ?? "";
                var profileInfo = row.Cells["ProfileInfo"].Value?.ToString() ?? "";
                var rawDataInfo = row.Cells["RawDataInfo"].Value?.ToString() ?? "";

                // 🔥 Do your logic here
                // Example:
                Console.WriteLine($"Request ID: {requestId}");
                Console.WriteLine($"ProfileInfo: {profileInfo}");
                Console.WriteLine($"RawDataInfo: {rawDataInfo}");
            }

            grdBulk.ResumeLayout();
            grdBulk.Refresh();
        }

        private void bthCheckAll_Click(object sender, EventArgs e)
        {
            if (!grdBulk.Columns.Contains("chkSelect"))
            {
                MessageBox.Show("Checkbox column not found.");
                return;
            }

            bool checkAll = bthCheckAll.Text == "CHECK ALL";

            grdBulk.EndEdit();
            grdBulk.SuspendLayout();

            foreach (DataGridViewRow row in grdBulk.Rows)
            {
                if (row.IsNewRow) continue;

                var cell = row.Cells["chkSelect"] as DataGridViewCheckBoxCell;

                if (cell != null)
                {
                    cell.Value = checkAll;
                }
            }

            grdBulk.ResumeLayout();
            grdBulk.Refresh();

            // 🔥 Toggle button text
            bthCheckAll.Text = checkAll ? "UNCHECK ALL" : "CHECK ALL";
        }

        private void btnUpdateSN_Click(object sender, EventArgs e)
        {
            if (!dbFunction.fPromptConfirmation($@"
                Are you sure to update merchant assigned SN's info?

                *Terminal
                 Current : {clsSearch.ClassHoldTerminalSN}
                 New     : {txtTerminalSN.Text}

                *SIM
                 Current : {clsSearch.ClassHoldSIMSN}
                 New     : {txtSIMSN.Text}
                ")) return;

            
            string pSearchValue = $"{txtIRIDNo.Text}{clsDefines.gPipe}{txtTerminalID.Text}{clsDefines.gPipe}{txtTerminalSN.Text}{clsDefines.gPipe}{txtSIMID.Text}{clsDefines.gPipe}{txtSIMSN.Text}";

            dbFunction.parseDelimitedString(pSearchValue, clsDefines.gPipe, 1);

            dbAPI.ExecuteAPI("PUT", "Update", "Update IR Detail Terminal/SIM", pSearchValue, "", "", "UpdateCollectionDetail");

            dbFunction.SetMessageBox("Merchant assigned SN's updated.", lblHeader.Text, clsFunction.IconType.iInformation);

        }

        private void btnSearchTerminal_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iTerminal;
            frmSearchField.iStatus = clsGlobalVariables.STATUS_AVAILABLE;
            frmSearchField.sTerminalType = "View Terminal";
            frmSearchField.sHeader = "TERMINAL";
            frmSearchField.isCheckBoxes = false;
            frmSearchField.iLocationID = clsFunction.iZero;
            frmSearchField.sLocation = clsFunction.sDefaultSelect;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsSearch.ClassHoldTerminalSN = clsSearch.ClassTerminalSN;

                txtTerminalID.Text = $"{clsSearch.ClassTerminalID}";
                txtTerminalSN.Text = $"{clsSearch.ClassTerminalSN}";
                txtTerminalType.Text = $"{clsSearch.ClassTerminalType}";
                txtTerminalModel.Text = $"{clsSearch.ClassTerminalModel}";
            }
        }

        private void btnRemoveTerminal_Click(object sender, EventArgs e)
        {
            txtTerminalID.Text =
            txtTerminalSN.Text = 
            txtTerminalType.Text =
            txtTerminalModel.Text =
            clsFunction.sNull;
        }

        private void btnSearchSIM_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iSIM;
            frmSearchField.iStatus = clsGlobalVariables.STATUS_AVAILABLE;
            frmSearchField.sHeader = "SIM";
            frmSearchField.isCheckBoxes = false;
            frmSearchField.iLocationID = clsFunction.iZero;
            frmSearchField.sLocation = clsFunction.sDefaultSelect;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                clsSearch.ClassHoldSIMSN = clsSearch.ClassSIMSerialNo;

                txtSIMID.Text = $"{clsSearch.ClassSIMID}";
                txtSIMSN.Text = $"{clsSearch.ClassSIMSerialNo}";
                txtSIMCarrier.Text = $"{clsSearch.ClassSIMCarrier}";
            }

        }

        private void btnRemoveSIM_Click(object sender, EventArgs e)
        {
            txtSIMID.Text =
            txtSIMSN.Text = 
            txtSIMCarrier.Text =
            clsFunction.sNull;
        }
    }
}
