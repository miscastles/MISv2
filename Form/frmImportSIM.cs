using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
using System.Diagnostics;
using Spire.Xls;
using System.Threading;
using Newtonsoft.Json;
using static MIS.Function.AppUtilities;

namespace MIS
{
    public partial class frmImportSIM : Form
    {
        public static bool fAutoLoadData = false;
        public static int iTab;
        public static int iTabSub;

        private clsAPI dbAPI;
        private clsINI dbSetting;
        private clsFile dbFile;
        private clsFunction dbFunction;
        private clsReportFunc dbReportFunc;

        private static string ExcelFilePath = @"";
        private string sExcelFileName = "";
        private string sSheet = "";
        private int iEndLimit = 10000;
        bool fEdit = false;

        int Counter = 1;
        int ReaderInterval;
        int ReaderTimeOut;
        int SerialNoMaxLength;
        int SerialNoMinLength = 5;
        int iBatchNo = 0;
        bool fEnableScan = false;

        int delay = 5; // 500 default

        // The column we are currently using for sorting.
        private ColumnHeader SortingColumn = null;

        class jsonObj
        {
            public object outParamValue { get; set; }
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

        public frmImportSIM()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ExcelDialog = new OpenFileDialog();
            ExcelDialog.Filter = "Excel Files|*.xls;*.xlsx";
            ExcelDialog.InitialDirectory = @"C:\CASTLESTECH_MIS\IMPORT\";
            ExcelDialog.Title = "Select file to import...";

            if (ExcelDialog.ShowDialog() == DialogResult.OK)
            {
                ExcelFilePath = ExcelDialog.FileName;
                txtPathFileName.Text = ExcelDialog.FileName;
                txtPathFileName.ReadOnly = true;
                btnLoadFile.Enabled = false;
                sExcelFileName = Path.GetFileName(txtPathFileName.Text);

                if (clsSystemSetting.ClassSystemImportCheck > 0)
                {
                    if (dbAPI.isImportFileName("Search", "SIM Import", sExcelFileName))
                    {                        
                        dbFunction.SetMessageBox("Import failure:" + "\n" +
                                                 sExcelFileName + " was already processed.", "Import SIM", clsFunction.IconType.iWarning);

                        btnImportCancel_Click(this, e);
                        return;
                    }
                }

                try
                {
                    // Waiting / Hour Glass
                    Cursor.Current = Cursors.WaitCursor;

                    sSheet = dbFunction.getSheetName(txtPathFileName.Text);
                    if (!dbFunction.isValidSheetName(sSheet)) return;

                    txtSheetName.Text = "`" + sSheet + "$" + "`";
                    txtFileName.Text = sExcelFileName;

                    // Create Temporary database                
                    dbAPI.ExecuteAPI("POST", "Create", "", "", txtSheetName.Text, "", "CreateTempTable");

                    dbFunction.ClearDataGrid(grdDummy);
                    dbFunction.ImportToDummyDataGrid(grdDummy, sSheet, txtPathFileName.Text);

                    // Back to normal 
                    Cursor.Current = Cursors.Default;

                    btnImportSave.Enabled = false;
                    btnLoadFile.Enabled = false;

                    ucStatusDisplay.SetStatus("Preparing import file complete.", Enums.StatusType.Processing);
                    Task.Delay(delay); // Asynchronously wait without blocking UI

                    // Import Start Here
                    if (!fContinueConfirm())
                    {
                        btnImportCancel_Click(this, e);
                        return;
                    }     
                    
                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                    ucStatusDisplay.SetStatus("Processing import file", Enums.StatusType.Processing);
                    Task.Delay(delay); // Asynchronously wait without blocking UI

                    // Drop table create from sheetname
                    if (txtSheetName.Text.Length > 0)
                        dbAPI.ExecuteAPI("DELETE", "Delete", "", txtSheetName.Text.Replace("`", ""), "Drop Temp Table", "", "DeleteCollectionDetail");
                    
                    if (!ImportToDataGrid()) return;
                    
                    ucStatusDisplay.SetStatus("Validating import file header", Enums.StatusType.Processing);
                    Task.Delay(delay); // Asynchronously wait without blocking UI

                    if (!isValidHeader()) return; // Check Header                  
                    
                    // check for mandatory fields
                    if (!dbFunction.isValidDataGridValue(clsFunction.ImportType.iImportSIM, dbAPI, grdDummy, txtFileName.Text, 0, false)) return;

                    // check field column is required
                    if (!dbFunction.isValidDataGridValue(clsFunction.ImportType.iImportSIM, dbAPI, grdDummy, txtFileName.Text, 1, false)) return;
                    
                    TotalCount();

                    btnImportSave.Enabled = true;
                    btnValidate.Enabled = true;

                    cboIClient.Enabled = true;

                    Cursor.Current = Cursors.Default; // Back to normal 

                    dbFunction.SetMessageBox("Import of file " +
                                             "\n" +
                                             "[ " + txtFileName.Text + " ]" + " " + "is complete." +
                                             "\n\n" +
                                             "You may save import now."
                                             , "Import SIM", clsFunction.IconType.iInformation);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception error " + ex.Message);
                    MessageBox.Show(ex.Message);
                    btnLoadFile.Enabled = true;                    
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

        private void frmImportSIM_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbSetting = new clsINI();
            dbFile = new clsFile();
            dbFunction = new clsFunction();
            dbReportFunc = new clsReportFunc();

            dbSetting.InitDatabaseSetting();
            dbSetting.InitSystemSetting();

            ReaderInterval = clsSystemSetting.ClassSystemReaderInterval;
            ReaderTimeOut = clsSystemSetting.ClassSystemReaderTimeOut;
            SerialNoMaxLength = clsSystemSetting.ClassSystemSerialNoMaxLength;

            btnClearGenerateEntry_Click(this, e);

            fEdit = false;
            fEnableScan = false;
            InitButton();
            InitDate();
            InitTab();
            InitTimerSerialNo();
            PKTextBoxBackColor(true);

            InitComboBox();

            btnAddAutoGen.Focus();

            btnGenerate.Enabled = btnReset.Enabled = false;
            btnRelease.Enabled = false;
            
            // Load Mapping
            dbAPI.ExecuteAPI("GET", "View", "Type", "SIM", "Mapping", "", "ViewMapping");

            dbFunction.initTabSelection(tabSIM, 2);

            if (fAutoLoadData)
            {
                btnSearchSIMSN_Click(this, e);
                fAutoLoadData = false;
            }

            Cursor.Current = Cursors.Default;
        }

        private void ClearDataGrid()
        {
            if (grdDummy.RowCount > 0)
            {
                if (grdDummy.DataSource != null)
                {
                    grdDummy.DataSource = null;
                    grdDummy.Rows.Clear();
                }
                else
                {
                    grdDummy.Rows.Clear();
                }
                grdDummy.Refresh();
            }

            if (grdList.RowCount > 0)
            {
                if (grdList.DataSource != null)
                {
                    grdList.DataSource = null;
                    grdList.Rows.Clear();
                }
                else
                {
                    grdList.Rows.Clear();
                }
                grdList.Refresh();
            }

            grdDummy.Rows.Clear();
            grdDummy.ColumnCount = 0;
            grdDummy.RowCount = 0;

            grdList.Rows.Clear();
            grdList.ColumnCount = 0;
            grdList.RowCount = 0;

            lvwGenerateList.Items.Clear();
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

        bool fResetConfirm()
        {
            bool fContinue = true;

            if (MessageBox.Show("This will clear field(s)." +
                                    "\n\n",
                                    "Continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fContinue = false;
            }

            return fContinue;
        }

        private bool ImportToDataGrid()
        {
            int iRowCount = 0;
            int iColCount = 0;
            int i, x = 0;
            int iDataRowIndex = 0;
            int y = 1;
            int ii = 0;
            bool isValid = true;

            // Delivery Date            
            DateTime DDateTime = DateTime.Now;
            
            // Received Date            
            DateTime RDateTime = DateTime.Now;
            
            dbFunction.InitDataGridView(grdList);

            iRowCount = grdDummy.RowCount + 1;
            iColCount = 9;
            //iColCount = grdDummy.ColumnCount;

            DateTime dteTemp = DateTime.Now;            
            int iDeliveryDate = dbFunction.GetMapColumnIndex(clsDefines.HDR_TEMPLATE_DeliveryDate);            
            int iReceiveDate = dbFunction.GetMapColumnIndex(clsDefines.HDR_TEMPLATE_ReceivedDate);
            
            // Add Column and Set Header            
            grdList.ColumnCount = iColCount;
            for (i = 0; i < iColCount; i++)
            {
                string cellParam = grdDummy.Columns[i].HeaderText;
                cellParam = cellParam.Replace("\n", " ").Trim();

                if (i > 0)
                {
                    if (cellParam.CompareTo(clsDefines.HDR_TEMPLATE_SerialNo) == 0)
                        grdList.Columns[i].Width = 180;
                    else
                        grdList.Columns[i].Width = 130;
                }
                else
                {
                    grdList.Columns[i].Width = 60;
                }
                    
                grdList.Columns[i].Name = cellParam;

                y = i + 1;
                Debug.WriteLine(y.ToString() + "," + cellParam + "," + cellParam + "," + "0" + "," + y.ToString() + "," + "SIM");
            }

            int SerialNo = dbFunction.GetDataGridHeaderColumnIndex(grdDummy, "SERIAL NO");        

            for (i = iDataRowIndex; i < iRowCount; i++)
            {
                ii++;
                if (iRowCount - 1 > i)
                {
                    string sFieldCheck = grdDummy.Rows[i].Cells[1].Value.ToString(); // Check serial no is not blank

                    Debug.WriteLine("------------------------------------");
                    Debug.WriteLine($"Row:[{i}]->SN:[{sFieldCheck}]");
                    Debug.WriteLine("------------------------------------");

                    Invoke((MethodInvoker)(() =>
                    {
                        ucStatusDisplay.SetStatus($"Processing line# [{i}]/[{iRowCount}] [{sFieldCheck}]", Enums.StatusType.Processing);
                        Task.Delay(delay); // Asynchronously wait without blocking UI

                    }));

                    if (sFieldCheck.Trim().Length > 0)
                    {
                        int iIndex = grdList.Rows.Add();
                        for (x = 0; x < iColCount; x++)
                        {
                            string cellValue = grdDummy.Rows[i].Cells[x].Value.ToString().ToUpper();

                            /*
                            // Delivery date
                            if (iDeliveryDate == x && cellValue.Trim().Length > 0)
                            {
                                sTempDate = clsFunction.sNull + cellValue.Substring(0, 10).Trim();
                                sDeliveryDate = dbFunction.GetDateFromParse(sTempDate, "yyyy-MM-dd", "yyyy-MM-dd");
                                cellValue = sDeliveryDate;
                            }

                            // Receive date
                            if (iReceiveDate == x && cellValue.Trim().Length > 0)
                            {
                                sTempDate = clsFunction.sNull + cellValue.Substring(0, 10).Trim();
                                sReceiveDate = dbFunction.GetDateFromParse(sTempDate, "yyyy-MM-dd", "yyyy-MM-dd");
                                cellValue = sReceiveDate;
                            }
                            */

                            // special chraracter cleanup
                            //cellValue = cellValue.Replace("&", "AND");
                            cellValue = cellValue.Replace("|", clsFunction.sNull);
                            cellValue = cellValue.Replace("~", clsFunction.sNull);
                            cellValue = cellValue.Replace("^", clsFunction.sNull);
                            cellValue = cellValue.Replace("%", clsFunction.sNull);

                            if (cellValue.Length > 0)
                                grdList.Rows[iIndex].Cells[x].Value = StrClean(cellValue);
                            else
                                grdList.Rows[iIndex].Cells[x].Value = clsFunction.sDash;
                        }
                    }                    
                }                
            }

            //dbFunction.DataGridViewAlternateBackColor(grdList);

            grdList.ResumeLayout(); // Resume UI updates

            return isValid;
        }

        private void btnImportCancel_Click(object sender, EventArgs e)
        {
            dbFunction.ClearTextBox(this);
            //dbFunction.ClearComboBox(this);
            //ClearDataGrid();

            dbFunction.ClearDataGrid(grdDummy);
            dbFunction.ClearDataGrid(grdList);
            
            btnLoadFile.Enabled = true;
            btnImportSave.Enabled = false;

            ucStatusDisplay.SetStatus("", Enums.StatusType.Init);
        }
        
        private bool isValidHeader()
        {                     
            int i = 0;            
            string sList = "";
            string sMapFrom = "";
            bool fExist = true;

            dbAPI.ExecuteAPI("GET", "View", "Type", "SIM", "Mapping", "", "ViewMapping");

            Debug.WriteLine("grdList.ColumnCount=" + grdList.ColumnCount);
            Debug.WriteLine("clsMapping.ClassRecordCount=" + clsMapping.ClassRecordCount);
            Debug.WriteLine("clsArray.MapFrom.Length=" + clsArray.MapFrom.Length);

            for (i = 0; i < clsArray.MapFrom.Length; i++)
            {
                sMapFrom = clsArray.MapFrom[i];
                if (!dbFunction.isHeaderExist(grdList, sMapFrom))
                {
                    sList = sList + sMapFrom + "\n";
                    fExist = false;
                }
            }

            if (!fExist)
            {
                MessageBox.Show("Mapping not found on import file\n\n" + "Header: " + grdList.ColumnCount + "\n" + "Mapping: " + clsMapping.ClassRecordCount + "\n\n" + "Mapping list: \n" + sList, "Note", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ClearDataGrid();
                dbFunction.ClearTextBox(this);
                //dbFunction.ClearComboBox(this);
                btnLoadFile.Enabled = true;

                return false;
            }
            
            return true;
        }
        
        private void btnImportSave_Click(object sender, EventArgs e)
        {
            if (!dbAPI.isValidSystemVersion()) return;

            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isAdd, clsUser.ClassUserID, 34)) return;

            // check file in use
            if (!dbFunction.checkFileInUse(txtPathFileName.Text)) return;

            // check client
            //if (!dbFunction.isValidDescriptionEntry(cboIClient.Text, "Client" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            //// check ready
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

            if (!dbFunction.fPromptConfirmation($"Are you sure to save import records on list?")) return;

            Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

            try
            {
                SaveSIMMaster();

                SaveSIMImportDetail();

                // ------------------------------------------------------------------------------------------
                // Upload physical attach file
                // ------------------------------------------------------------------------------------------                
                ucStatusDisplay.SetStatus($"Uploading physial file of [{txtFileName.Text}]", Enums.StatusType.Upload);
                Task.Delay(delay); // Asynchronously wait without blocking UI

                string pLocalPath = $"{txtPathFileName.Text.Replace(txtFileName.Text, "")}";
                string pRemotePath = $"{clsGlobalVariables.strFTPRemoteSerialPath}{clsGlobalVariables.strAPIBank}{clsFunction.sBackSlash}";
                string pFileName = $"{txtFileName.Text}";

                Debug.WriteLine("Uploading import sim...");
                Debug.WriteLine($"pLocalPath=[{pLocalPath}]");
                Debug.WriteLine($"pRemotePath=[{pRemotePath}]");
                Debug.WriteLine($"pFileName=[{pFileName}]");

                ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                ftpClient.delete(pRemotePath + pFileName);
                ftpClient.upload(pRemotePath + pFileName, pLocalPath + pFileName);
                ftpClient.disconnect();

                Debug.WriteLine("Uploading import sim...complete");
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox("Message " + ex.Message, "SIM import failed", clsFunction.IconType.iError);
            }
            
            Cursor.Current = Cursors.Default; // Back to normal             

            dbFunction.SetMessageBox("Import SIM successfully saved.", "Saved", clsFunction.IconType.iInformation);

            // ROCKY - SIM IMPORT: ADD REPORT MESSAGE FOR SUCCESS AND DUPLICATE.

            if (dbFunction.isValidID(txtSIID.Text))
            {
                int pTSuccess = 0;
                int pTDuplicate = 0;

                dbAPI.ExecuteAPI("GET", "Search", "Total SIM Import Success", txtSIID.Text, "Get Info Detail", "", "GetInfoDetail");

                if (dbAPI.isNoRecordFound() == false && clsSearch.ClassOutParamValue.Length > 0)
                {
                    pTSuccess = int.Parse(dbFunction.CheckAndSetNumericValue(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "Total")));
                }

                dbAPI.ExecuteAPI("GET", "Search", "Total SIM Import Duplicate", txtSIID.Text, "Get Info Detail", "", "GetInfoDetail");

                if (dbAPI.isNoRecordFound() == false && clsSearch.ClassOutParamValue.Length > 0)
                {
                    pTDuplicate = int.Parse(dbFunction.CheckAndSetNumericValue(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "Total")));
                }

                if (MessageBox.Show("SIM import summary below:" +
                               "\n\n" +
                               " >Total SN import success: " + pTSuccess + "\n" +
                               " >Total SN import duplicate: " + pTDuplicate + "\n\n" +
                               "Do you want to generate report for the import summary?", "Import SIM",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    //dbAPI.ExecuteAPI("POST", "Insert", "", txtSIID.Text, "Import SIM", "", "InsertSelectCollectionDetail");
                    dbReportFunc.ViewSIMImportReport();

                }
            }


            btnImportCancel_Click(this, e);
        }

        private void SaveSIMMaster()
        {
            string sRowSQL = "";
            string sSQL = "";

            DateTime ImportDateTime = DateTime.Now;
            string sImportDateTime = "";

            DateTime DateTimeModified = DateTime.Now;
            string sDateTimeModified = "";

            //string FilePath = txtPathFileName.Text.Replace(@"\", clsFunction.sAsterisk); ;
            string FileName = txtFileName.Text;
            string Remarks = txtRemarks.Text;

            string ProcessedBy = clsUser.ClassUserFullName;
            string ModifiedBy = clsUser.ClassUserFullName;

            sImportDateTime = ImportDateTime.ToString("yyyy-MM-dd H:mm:ss");
            sDateTimeModified = DateTimeModified.ToString("yyyy-MM-dd H:mm:ss");

            sRowSQL = "";
            sRowSQL = " ('" + sImportDateTime + "', " +
            sRowSQL + sRowSQL + " '" + FileName + "', " +
            sRowSQL + sRowSQL + " '" + FileName + "', " +
            sRowSQL + sRowSQL + " '" + Remarks + "', " +
            sRowSQL + sRowSQL + " '" + sDateTimeModified + "', " +
            sRowSQL + sRowSQL + " '" + ProcessedBy + "', " +
            sRowSQL + sRowSQL + " '" + ModifiedBy + "') ";
            sSQL = sSQL + sRowSQL;

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "SIM Import Master", sSQL, "InsertCollectionMaster");

            txtSIID.Text = clsLastID.ClassLastInsertedID.ToString();

        }
        
        private void SaveSIMImportDetail()
        {
            string sTempSQL = "";
            string sRowSQL = "";
            string sSQL = "";
            string sRowCSV = "";
            //string sCSV = "";
            int iRowCount = grdList.RowCount;
            int iColCount = grdList.ColumnCount; // exclude Result column
            int iRowIndex = 0;
            int ii = 0;

            int i = 0;
            string sLastInsertID = clsSearch.ClassLastInsertedID.ToString();           
            List<string> TempArrayDataCol = new List<String>();
            int iRecordMinLimit = clsSystemSetting.ClassSystemRecordMinLimit;
            int iStartIndex = 0;
            int iEndIndex = 0;
            int iFileNameIndex = 0;
            int x;
            StringBuilder columnbind = new StringBuilder();

            // Load Record to Array
            dbAPI.GetTerminalStatusList("View", "", "", "Terminal Status"); // Load Terminal Status
            dbAPI.GetClientList("View", "Client List", "", "Particular"); // Load Client
            dbAPI.GetFEList("View", "Field Engineer List", "", "Particular"); // Load Field Engineer

            int iSNColIndex = GetHeaderColumnIndex(grdList, clsDefines.HDR_TEMPLATE_SerialNo);
            //int iFEColIndex = GetHeaderColumnIndex(grdList, "ASSIGNED TO");
            int iStatusColIndex = GetHeaderColumnIndex(grdList, clsDefines.HDR_TEMPLATE_Status);
            //int iClientColIndex = GetHeaderColumnIndex(grdList, "ALLOCATION");

            int iCarrierColIndex = GetHeaderColumnIndex(grdList, clsDefines.HDR_TEMPLATE_Carrier);
            int iLocationColIndex = GetHeaderColumnIndex(grdList, clsDefines.HDR_TEMPLATE_Location);

            DateTime dteTemp = DateTime.Now;
            string sTempDate = "";
            int iDeliveryDate = dbFunction.GetMapColumnIndex(clsDefines.HDR_TEMPLATE_DeliveryDate);
            string sDeliveryDate = "";
            int iReceiveDate = dbFunction.GetMapColumnIndex(clsDefines.HDR_TEMPLATE_ReceivedDate);
            string sReceiveDate = "";

            if (iRowCount > 0)
            {                
                // Delete File            
                //string sFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iSIM, 0);
                //dbFile.DeleteCSV(sFileName);

                for (i = iRowIndex; i < iRowCount; i++)
                {
                    ii++;
                    sRowSQL = "";
                    sSQL = "";
                    sTempSQL = "('" + txtSIID.Text + "',";

                    string pSN = grdList.Rows[i].Cells[1].Value.ToString(); // Check is not blank

                    for (x = 0; x < iColCount; x++)
                    {
                        string sCellValue = grdList.Rows[i].Cells[x].Value.ToString().Trim();

                        // ROCKY - SIM IMPORT: ADD CLEAN UP FOR SIM IMPORTS 
                        sCellValue = (sCellValue.Length > 0 ? clsFunction.FormatCharAndDate(sCellValue).ToUpper() : clsFunction.sDash);

                        if (sCellValue.Length > 0)
                        {
                            if (iSNColIndex == x)
                            {
                                string sSN = "'" + (sCellValue.Length > 0 ? sCellValue : clsFunction.sDash) + "'";
                                sCellValue = sSN + "," + sSN;
                            }

                            // Delivery date
                            if (iDeliveryDate == x && sCellValue.Trim().Length > 0)
                            {
                                if (DateTime.TryParse(sDeliveryDate, out dteTemp))
                                {
                                    sTempDate = clsFunction.sNull + sCellValue.Substring(0, 10).Trim();
                                    sDeliveryDate = dbFunction.GetDateFromParse(sTempDate, "dd/MM/yyyy", "yyyy-MM-dd");
                                }
                                else
                                {
                                    try
                                    {
                                        sDeliveryDate = dbFunction.GetDateFromParse(sTempDate, "MM/dd/yyyy", "yyyy-MM-dd");
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine("Error " + ex.Message);
                                        sDeliveryDate = clsFunction.sDateFormat;
                                    }
                                }

                                sCellValue = (!dbFunction.isValidDescription(sDeliveryDate) ? sCellValue : sDeliveryDate);
                            }

                            // Receive date
                            if (iReceiveDate == x && sCellValue.Trim().Length > 0)
                            {
                                if (DateTime.TryParse(sReceiveDate, out dteTemp))
                                {
                                    sTempDate = clsFunction.sNull + sCellValue.Substring(0, 10).Trim();
                                    sReceiveDate = dbFunction.GetDateFromParse(sTempDate, "dd/MM/yyyy", "yyyy-MM-dd");
                                }
                                else
                                {
                                    try
                                    {
                                        sReceiveDate = dbFunction.GetDateFromParse(sTempDate, "MM/dd/yyyy", "yyyy-MM-dd");
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine("Error " + ex.Message);
                                        sReceiveDate = clsFunction.sDateFormat;
                                    }
                                }

                                sCellValue = (!dbFunction.isValidDescription(sReceiveDate) ? sCellValue : sReceiveDate);
                            }

                            if (iStatusColIndex == x)
                            {
                                clsSearch.ClassOutFileID = 0;
                                dbFunction.GetIDFromFile("Status List", sCellValue);
                                int iSIMStatus = clsSearch.ClassOutFileID;
                                string sStatus = "'" + (sCellValue.Length > 0 ? sCellValue : clsFunction.sDash) + "'";
                                sCellValue = sStatus + "," + iSIMStatus.ToString();
                            }

                            if (iCarrierColIndex == x)
                            {
                                clsSearch.ClassOutFileID = 0;
                                dbFunction.GetIDFromFile("Carrier", sCellValue);
                                int iSIMCarrier = clsSearch.ClassOutFileID;
                                string sCarrier = "'" + (sCellValue.Length > 0 ? sCellValue : clsFunction.sDash) + "'";
                                sCellValue = sCarrier + "," + iSIMCarrier.ToString();
                            }

                            if (iLocationColIndex == x)
                            {
                                clsSearch.ClassOutFileID = 0;
                                dbFunction.GetIDFromFile("Location", sCellValue);
                                int iSIMLocation = clsSearch.ClassOutFileID;
                                string sLocation = "'" + (sCellValue.Length > 0 ? sCellValue : clsFunction.sDash) + "'";
                                sCellValue = sLocation + "," + iSIMLocation.ToString();
                            }

                            /*
                            if (iClientColIndex == x)
                            {
                                int iClientID = dbAPI.GetClientFromList(sCellValue);
                                string sClient = "'" + (sCellValue.Length > 0 ? sCellValue : clsFunction.sDash) + "'";
                                sCellValue = sClient + "," + iClientID.ToString();
                            }
                            */

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
                    //sCSV = sCSV + sRowCSV + "\n";

                    Debug.WriteLine("ii=" + ii.ToString() + "-" + "sRowCSV=" + sRowCSV);

                    if (sRowCSV.Length > 0)
                    {
                        TempArrayDataCol.Add(sRowCSV);
                    }

                    ucStatusDisplay.SetStatus($"Generating CSV line# [{i}]/[{grdList.RowCount}] [{pSN}]", Enums.StatusType.Processing);
                    Task.Delay(delay); // Asynchronously wait without blocking UI
                }

                //dbAPI.ExecuteAPI("POST", "Insert", "", "", "Terminal Import Detail", sSQL, "InsertCollectionDetail");

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
                            string sNewFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iSIM, iFileNameIndex);
                            Debug.WriteLine("SIM->sNewFileName=" + sNewFileName);

                            ucStatusDisplay.SetStatus($"Creating CSV File [{sNewFileName}]", Enums.StatusType.Create);
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
                    string sNewFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iSIM, iFileNameIndex);
                    Debug.WriteLine("SIM->sNewFileName=" + sNewFileName);

                    ucStatusDisplay.SetStatus($"Creating CSV File [{sNewFileName}]", Enums.StatusType.Create);
                    Task.Delay(delay); // Asynchronously wait without blocking UI

                    dbFile.DeleteCSV(sNewFileName);
                    dbFile.WriteCSV(sNewFileName, columnbind.ToString());
                }

                // Import
                if (iFileNameIndex > 0)
                {
                    for (i = 1; i <= iFileNameIndex; i++)
                    {
                        string sImportFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iSIM, i);

                        ucStatusDisplay.SetStatus($"Uploading CSV File [{sImportFileName}]", Enums.StatusType.Create);
                        Task.Delay(delay); // Asynchronously wait without blocking UI

                        // Upload File to FTP                                
                        Debug.WriteLine("=>>UploadFile=" + sImportFileName);
                        ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                        ftpClient.delete(clsGlobalVariables.strFTPUploadPath + sImportFileName);
                        ftpClient.upload(clsGlobalVariables.strFTPUploadPath + sImportFileName, @clsGlobalVariables.strFTPLocalPath + sImportFileName);
                        ftpClient.disconnect(); // ftp disconnect

                        Debug.WriteLine("=>>API Call ImportSIMDetail=" + sImportFileName);
                        dbAPI.ExecuteAPI("POST", "Import", "CSV", sImportFileName, "Import SIM Detail", "", "ImportSIMDetail"); // Process CSV File

                    }
                }
                
                dbFunction.GetResponseTime("SIM Import Detail");

                Cursor.Current = Cursors.Default; // Back to normal                
            }

        }
        
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if(!ValidateFields()) return;

            if (!CheckDateFromTo(dteADeliveryDate, dteAReceivedDate)) return;

            if (fEndLimitExceed()) return;

            if (!fContinueConfirm()) return;

            // Waiting / Hour Glass
            Cursor.Current = Cursors.WaitCursor;

            GenerateSN();

            //btnGenerate.Enabled = btnReset.Enabled = pnlGenerate.Enabled = false;
            
            // Back to normal 
            Cursor.Current = Cursors.Default;
        }
        private bool ValidateFields()
        {
            Debug.WriteLine("--ValidateFields--");
            Debug.WriteLine("tabSIM.SelectedIndex=" + tabSIM.SelectedIndex);

            switch (tabSIM.SelectedIndex)
            {
                case 0: // import
                    break;
                case 1: // auto gen
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iCarrier, cboACarrier.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iLocation, txtLocationIDFrom.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iSIMStatus, txtSIMStatus.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iPrefix, txtPrefix.Text)) return false;
                    //if (!dbFunction.isValidEntry(clsFunction.CheckType.iPadLen, txtPadLength.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iStartIndex, txtIndexStart.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iEndIndex, txtIndexEnd.Text)) return false;
                    break;
                case 2: // manual entry      
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iSIMStatus, txtSIMStatus.Text)) return false;

                    if (lvwManual.Items.Count > 0)
                    {
                        if (!dbFunction.isValidListViewChecked(lvwManual)) return false;
                    }
                    else
                    {
                        if (!dbFunction.isValidEntry(clsFunction.CheckType.iSIMSN, txtSIMSN.Text)) return false;
                        if (!dbFunction.isValidEntry(clsFunction.CheckType.iCarrier, cboMCarrier.Text)) return false;                     
                    }

                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iLocation, txtLocationIDFrom.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iSIMStatus, cboMStatus.Text)) return false;

                    break;
                case 3: // released/transfer
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iItemList, lvwRelease.Items.Count.ToString())) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iBatchNo, txtTransNo.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iFromLocation, txtLocationIDFrom.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iToLocation, txtLocationIDTo.Text)) return false;
                    break;
            }

            return true;
        }
        private bool MValidateFields()
        {
            bool fValid = false;

            if (txtSIMSN.Text.Length > 0 &&
                dbFunction.isValidComboBoxValue(cboMStatus.Text) &&             
                dbFunction.isValidComboBoxValue(cboMCarrier.Text))
                fValid = true;

            if (!fValid)
            {
                MessageBox.Show("Check the following field(s) listed below:\n\n" +
                                "*Serial No.\n" +
                                "*Status\n" +
                                "*Carrier\n" +
                                "*AssignTo\n" +                                
                                "\n" +
                                "Field(s) with asterisk(*) must not be blank.", "Incomplete Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return fValid;
        }
        private bool ValidateMFields()
        {
            bool fValid = false;

            if ((cboMAllocation.Text.Length > 0) && (cboMCarrier.Text.Length > 0))
                fValid = true;

            if (!fValid)
            {
                MessageBox.Show("Check the following field(s) listed below:\n\n" +                                
                                "*Carrier\n" +
                                "*AssignTo\n" +
                                "\n" +
                                "Field(s) with asterisk(*) must not be blank.", "Incomplete Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return fValid;
        }
        private bool fEndLimitExceed()
        {
            bool isLimit = false;
            int iEnd = 0;

            iEnd = int.Parse((txtIndexEnd.Text.Length > 0 ? txtIndexEnd.Text : "0"));

            if (iEnd > iEndLimit)
                isLimit = true;

            if (isLimit)
            {
                MessageBox.Show("End index must be below " + iEndLimit.ToString() + "\n" +
                                "\n" +
                                "End index limmit exceed", "Import", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return isLimit;
        }
        private void GenerateSN()
        {
            int iStart = 0;
            int iEnd = 0;
            int iPadLength = 0;
            int inLineNo = 0;
            string sSerialNo = "";
            string sPad = "";
            int ii = 0;

            dbFunction.ClearListViewItems(lvwGenerateList);

            // Delivery Date
            DateTime stDeliveryDate = dteADeliveryDate.Value;
            string pDeliveryDate = stDeliveryDate.ToString("yyyy-MM-dd");

            // Received Date
            DateTime stReceivedDate = dteAReceivedDate.Value;
            string pReceivedDate = stReceivedDate.ToString("yyyy-MM-dd");

            if (int.Parse(txtIndexStart.Text) > int.Parse(txtIndexEnd.Text))
            {
                MessageBox.Show("Index End must be greater than Index Start", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            iStart = int.Parse(txtIndexStart.Text);
            iEnd = int.Parse(txtIndexEnd.Text);
            iPadLength = int.Parse(dbFunction.CheckAndSetNumericValue(txtPadLength.Text));

            btnSaveAutoGen.Enabled = false;
            lvwGenerateList.Items.Clear();

            if (iPadLength > 0)
                sPad = dbFunction.padLeftChar(clsFunction.sPadZero, clsFunction.sPadZero, iPadLength);
            else
                sPad = clsFunction.sNull;

            for (int i = iStart; i <= iEnd; i++)
            {
                ii++;
                inLineNo++;
                ListViewItem item = new ListViewItem(inLineNo.ToString());
                sSerialNo = txtPrefix.Text + sPad + i.ToString();
                item.SubItems[0].ForeColor = Color.Black;
                bool fExist = dbAPI.CheckTerminalDetail("Search", "SIM Detail Check", sSerialNo);
                if (fExist)
                    item.SubItems[0].ForeColor = Color.Red;

                item.SubItems.Add(txtABatchNo.Text);                              
                item.SubItems.Add(sSerialNo);
                item.SubItems.Add(cboACarrier.Text);
                item.SubItems.Add(cboAAllocation.Text);
                item.SubItems.Add(txtARemarks.Text);
                item.SubItems.Add(pDeliveryDate);
                item.SubItems.Add(pReceivedDate);
                
                lvwGenerateList.Items.Add(item);
            }

            dbFunction.ListViewAlternateBackColor(lvwGenerateList);

            if (lvwGenerateList.Items.Count > 0)
                btnSaveAutoGen.Enabled = true;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (!fResetConfirm()) return;

            dbFunction.ClearTextBox(this);
            //dbFunction.ClearComboBox(this);
        }
        
        private void btnAddGenerate_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isAdd, clsUser.ClassUserID, 34)) return;

            dbFunction.ClearTextBox(this);
            //dbFunction.ClearComboBox(this);   
            
            //dbAPI.FillComboBoxCarrier(cboACarrier);
            //dbAPI.FillComboBoxFE(cboAAllocation);
            //dbAPI.FillComboBoxTerminalStatus(cboAStatus);
            //dbAPI.FillComboBoxLocation(cboALocation);
            
            InitDate();
            btnAddAutoGen.Enabled = false;
            btnGenerate.Enabled = true;
            btnReset.Enabled = true;

            dbFunction.TextBoxUnLock(true, this);
            dbFunction.ComBoBoxUnLock(true, this);

            iBatchNo = dbAPI.GetControlID("SIM Master");
            txtABatchNo.Text = iBatchNo.ToString();

            cboACarrier.Text = cboAAllocation.Text = cboALocation.Text = cboAStatus.Text = clsFunction.sDefaultSelect;
            
            txtPrefix.Focus();

            btnGenerate.Enabled = btnReset.Enabled = pnlGenerate.Enabled = true;
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void btnClearGenerateEntry_Click(object sender, EventArgs e)
        {
            fEdit = false;
            ClearDataGrid();
            InitDate();
            btnGenerate.Enabled = false;
            btnReset.Enabled = false;
            btnSaveAutoGen.Enabled = false;
            btnAddAutoGen.Enabled = true;

            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);
            dbFunction.ClearListViewItems(lvwGenerateList);
            
            btnGenerate.Enabled = btnReset.Enabled = pnlGenerate.Enabled = false;

            cboAStatus.Text = cboAAllocation.Text = cboACarrier.Text = cboALocation.Text = clsFunction.sDefaultSelect;
        }

        private void txtPrefix_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtPadLength_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtIndexStart_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtIndexEnd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnSaveGenerate_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isUpdate, clsUser.ClassUserID, 34)) return;

            if (!ValidateFields()) return;

            if (!dbFunction.fSavingConfirm(false)) return;

            // Waiting / Hour Glass
            Cursor.Current = Cursors.WaitCursor;

            SaveSIMMaster();

            SaveGenerateSNDetail();

            // Back to normal 
            Cursor.Current = Cursors.Default;
            
            dbFunction.SetMessageBox("Auto Generated SIM successfully saved.", "Saved", clsFunction.IconType.iInformation);

            btnImportCancel_Click(this, e);
            btnClearGenerateEntry_Click(this, e);
        }
        private void SaveGenerateSNDetail()
        {
            int ii = 0;
            string sRowSQL = "";
            string sSQL = "";
            string sRowCSV = "";
            //string sCSV = "";
            int iRowCount = lvwGenerateList.Items.Count;
            
            DateTime CurrentDateTime = DateTime.Now;
            string sCurrentDateTime = "";
            string sCurrentUser = clsUser.ClassUserName;            
            
            dbAPI.GetFEList("View", "Field Engineer List", "", "Particular"); // Load Field Engineer
            int iFEID = dbAPI.GetFEFromList(cboAAllocation.Text);

            dbAPI.GetTerminalStatusList("View", "", "", "Terminal Status"); // Load Terminal Status
            int iStatus = dbAPI.GetTerminalStatusFromList(cboAStatus.Text);

            sCurrentDateTime = CurrentDateTime.ToString("MM-dd-yyyy");

            dbFunction.GetRequestTime("AutoGen Import SIM Detail");

            dbFunction.GetIDFromFile("Carrier", cboACarrier.Text);
            clsSearch.ClassCarrierID = clsSearch.ClassOutFileID;

            if (iRowCount > 0)
            {
                // Get location id
                dbFunction.GetIDFromFile("Location", cboALocation.Text);
                clsSearch.ClassLocationID = clsSearch.ClassOutFileID;

                // Delete File            
                string sFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iSIM, 0);
                dbFile.DeleteCSV(sFileName);
                
                foreach (ListViewItem i in lvwGenerateList.Items)
                {
                    ii++;
                    string sBatchNo = i.SubItems[1].Text;
                    string sSerialNo = i.SubItems[2].Text;
                    string sCarrier = i.SubItems[3].Text;
                    string sAssignedTo = i.SubItems[4].Text;
                    string sRemarks = i.SubItems[5].Text;
                    string sDeliveryDate = i.SubItems[6].Text;
                    string sReceiveDate = i.SubItems[7].Text;                                        

                    if (sSerialNo.Length > 0)
                    {
                        sSQL = "";
                        sRowSQL = "";
                        sRowSQL = "('" + ii + "'," +
                        sRowSQL + sRowSQL + "'" + txtSIID.Text + "'," +
                        sRowSQL + sRowSQL + "'" + sSerialNo + "'," +
                        sRowSQL + sRowSQL + "'" + sSerialNo + "'," +
                        sRowSQL + sRowSQL + "'" + cboACarrier.Text + "'," +
                        sRowSQL + sRowSQL + "'" + clsSearch.ClassCarrierID + "'," +                        
                        sRowSQL + sRowSQL + "'" + sDeliveryDate + "'," +
                        sRowSQL + sRowSQL + "'" + sReceiveDate + "'," +
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(dbFunction.CheckDefaultSelectValue(cboALocation.Text)) + "'," +
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtLocationIDFrom.Text) + "'," +
                        sRowSQL + sRowSQL + "'" + cboAStatus.Text + "'," +
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(iStatus.ToString()) + "'," +
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(dbFunction.CheckDefaultSelectValue(cboAAllocation.Text)) + "'," +
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(sRemarks) + "')";

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

                }                

                if (iRowCount > 0)
                {
                    // Upload File to FTP                    
                    ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                    ftpClient.delete(clsGlobalVariables.strFTPUploadPath + sFileName);
                    ftpClient.upload(clsGlobalVariables.strFTPUploadPath + sFileName, @clsGlobalVariables.strFTPLocalPath + sFileName);
                    ftpClient.disconnect(); // ftp disconnect

                    // Process CSV File
                    dbAPI.ExecuteAPI("POST", "Import", "CSV", sFileName, "Import SIM Detail", "", "ImportSIMDetail");
                }
            }

            dbFunction.GetResponseTime("AutoGen Import Terminal Detail");
        }
        private void SaveMSNDetail()
        {
            int ii = 0;
            string sRowSQL = "";
            string sSQL = "";
            string sRowCSV = "";            
            int iRowCount = lvwHistoryList.Items.Count;

            DateTime CurrentDateTime = DateTime.Now;
            string sCurrentDateTime = "";
            string sCurrentUser = clsUser.ClassUserName;
            int iFEID = dbAPI.GetFEFromList(cboMAllocation.Text);
            string sBatchNo = "";
            string sSerialNo = "";
            string sCarrier = "";
            string sAssignedTo = "";
            string sRemarks = "";
            string sDeliveryDate = "";
            string sReceiveDate = "";
            int iIRStatus = 0;
            string sIRStatusDescription = "";
            int iClientID = 0;
            string sClientName = "";

            DateTime stDRDate = dteMDeliveryDate.Value;
            DateTime stRRDate = dteMReceiveDate.Value;

            sCurrentDateTime = CurrentDateTime.ToString("MM-dd-yyyy");

            dbFunction.GetRequestTime("Manual SIM Detail");

            DateTime stReceiveDate = dteMReceiveDate.Value;
            string pReceiveDate = stReceiveDate.ToString("yyyy-MM-dd");

            DateTime stDeliveryDate = dteMDeliveryDate.Value;
            string pDeliveryDate = stDeliveryDate.ToString("yyyy-MM-dd");

            DateTime stReleaseDate = dteMReleaseDate.Value;
            string pReleseDate = stReleaseDate.ToString("yyyy-MM-dd");

            DateTime stShipmentDate = dteMShipmentDate.Value;
            string pShipmentDate = stShipmentDate.ToString("yyyy-MM-dd");

            if (!fEdit)
            {
                if (!fEnableScan)
                {
                    sSQL = "";
                    sRowSQL = "";
                    sRowSQL = "('" + (txtFEID.Text.Length > 0 ? txtFEID.Text : "0") + "'," +
                    sRowSQL + sRowSQL + "'" + (txtMBatchNo.Text.Length > 0 ? txtMBatchNo.Text : "0") + "'," +
                    sRowSQL + sRowSQL + "'" + StrClean(dbFunction.CheckAndSetStringValue(txtSIMSN.Text)) + "'," +
                    sRowSQL + sRowSQL + "'" + StrClean(dbFunction.CheckAndSetStringValue(txtSIMSN.Text)) + "'," +
                    sRowSQL + sRowSQL + "'" + clsSearch.ClassCarrierID + "'," +
                    sRowSQL + sRowSQL + "'" + cboMCarrier.Text + "'," +
                    sRowSQL + sRowSQL + "'" + cboMAllocation.Text + "'," +
                    sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtLocationIDFrom.Text) + "'," +
                    sRowSQL + sRowSQL + "'" + cboMLocation.Text + "'," +
                    sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtParticularID.Text) + "'," +
                    sRowSQL + sRowSQL + "'" + cboMAllocation.Text + "'," +
                    sRowSQL + sRowSQL + "'" + (txtMRemarks.Text.Length > 0 ? txtMRemarks.Text : clsFunction.sDash) + "'," +
                    sRowSQL + sRowSQL + "'" + stDRDate.ToString("yyyy-MM-dd") + "'," +
                    sRowSQL + sRowSQL + "'" + stRRDate.ToString("yyyy-MM-dd") + "'," +
                    sRowSQL + sRowSQL + "'" + clsFunction.sZero + "'," +
                    sRowSQL + sRowSQL + "'" + clsFunction.sDash + "'," +
                    sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtSIMStatus.Text) + "'," +                    
                    sRowSQL + sRowSQL + "'" + cboMStatus.Text + "')";
                    sSQL = sSQL + sRowSQL;

                    Debug.WriteLine("sSQL="+ sSQL);

                    dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 1);

                    dbAPI.ExecuteAPI("POST", "Insert", "", "", "SIM Import Detail", sSQL, "InsertCollectionDetail");
                }
                else
                {
                    if (iRowCount > 0)
                    {
                        // Delete File            
                        string sFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iSIM, 0);
                        dbFile.DeleteCSV(sFileName);

                        foreach (ListViewItem i in lvwHistoryList.Items)
                        {
                            ii++;
                            sBatchNo = i.SubItems[1].Text;
                            sSerialNo = i.SubItems[2].Text;
                            sCarrier = i.SubItems[3].Text;
                            sAssignedTo = i.SubItems[4].Text;
                            sRemarks = i.SubItems[5].Text;
                            sDeliveryDate = i.SubItems[6].Text;
                            sReceiveDate = i.SubItems[7].Text;
                            iIRStatus = clsGlobalVariables.STATUS_AVAILABLE;
                            sIRStatusDescription = clsGlobalVariables.STATUS_AVAILABLE_DESC;

                            if (sSerialNo.Length > 0)
                            {
                                sSQL = "";
                                sRowSQL = "";
                                sRowSQL = "('" + (sBatchNo.Length > 0 ? sBatchNo : clsFunction.sZero) + "'," +
                                sRowSQL + sRowSQL + "'" + (sSerialNo.Length > 0 ? sSerialNo : clsFunction.sZero) + "'," +
                                sRowSQL + sRowSQL + "'" + (sSerialNo.Length > 0 ? sSerialNo : clsFunction.sZero) + "'," +
                                sRowSQL + sRowSQL + "'" + (sCarrier.Length > 0 ? sCarrier : clsFunction.sDash) + "'," +
                                sRowSQL + sRowSQL + "'" + (sAssignedTo.Length > 0 ? sAssignedTo : clsFunction.sDash) + "'," +
                                sRowSQL + sRowSQL + "'" + (iFEID > 0 ? iFEID : 0) + "'," +                                
                                sRowSQL + sRowSQL + "'" + (sRemarks.Length > 0 ? sRemarks : clsFunction.sDash) + "'," +
                                sRowSQL + sRowSQL + "'" + sDeliveryDate + "'," +
                                sRowSQL + sRowSQL + "'" + sReceiveDate + "'," +
                                sRowSQL + sRowSQL + "'" + sIRStatusDescription + "'," +
                                sRowSQL + sRowSQL + "'" + (iIRStatus > 0 ? iIRStatus : 0) + "'," +
                                sRowSQL + sRowSQL + "'" + (sClientName.Length > 0 ? sClientName : clsFunction.sDash) + "'," +
                                sRowSQL + sRowSQL + "'" + (iClientID > 0 ? iClientID : 0) + "')";

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
                        }

                        if (iRowCount > 0)
                        {
                            // Upload File to FTP                    
                            ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                            ftpClient.delete(clsGlobalVariables.strFTPUploadPath + sFileName);
                            ftpClient.upload(clsGlobalVariables.strFTPUploadPath + sFileName, @clsGlobalVariables.strFTPLocalPath + sFileName);
                            ftpClient.disconnect(); // ftp disconnect

                            // Process CSV File
                            dbAPI.ExecuteAPI("POST", "Import", "CSV", sFileName, "Import SIM Detail", "", "ImportSIMDetail");
                        }
                    }
                }
            }
            
            dbFunction.GetResponseTime("Manual SIM Detail");
        }
        private void txtBatchNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void bunifuCards4_Paint(object sender, PaintEventArgs e)
        {

        }
        
        private void AddItem(int inLineNo)
        {
            // Add to List            
            ListViewItem item = new ListViewItem(inLineNo.ToString());

            item.SubItems.Add(clsTA.ClassSIMID.ToString());
            item.SubItems.Add(clsTA.ClassFEID.ToString());
            item.SubItems.Add(clsTA.ClassSIMSerialNo);
            item.SubItems.Add(clsTA.ClassCarrier);            
            item.SubItems.Add(clsTA.ClassAssignedTo);
            item.SubItems.Add(clsTA.ClassRemarks);
            item.SubItems.Add(clsTA.ClassDeliveryDate);
            item.SubItems.Add(clsTA.ClassReceivedDate);
            item.SubItems.Add(clsTA.ClassSIMStatus.ToString());
            item.SubItems.Add(clsTA.ClassSIMStatusDescription);

            lvwHistoryList.Items.Add(item);
        }

        private void cboAssignedTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void tabImport_SelectedIndexChanged(object sender, EventArgs e)
        {                        
            InitTab();
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
                        
        }

        private void lvwList_SelectedIndexChanged(object sender, EventArgs e)
        {
            
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isAdd, clsUser.ClassUserID, 34)) return;

            Cursor.Current = Cursors.WaitCursor;

            fEdit = false;

            dbFunction.ClearTextBox(this);
            //dbFunction.ClearComboBox(this);
            dbFunction.TextBoxUnLock(true, this);
            dbFunction.ComBoBoxUnLock(true, this);

            GetMTextBoxID();
            
            InitButton();            
            btnAdd.Enabled = false;
            btnSave.Enabled = true;

            iBatchNo = dbAPI.GetControlID("SIM Master");
            txtMBatchNo.Text = iBatchNo.ToString();

            cboMCarrier.Text = cboMAllocation.Text = cboMLocation.Text = cboMStatus.Text = clsFunction.sDefaultSelect;

            Counter = 0;
            InitTimerSerialNo();
            PKTextBoxBackColor(false);

            btnMAddItem.Enabled = btnMRemoveItem.Enabled = true;
            btnMApplyChanges.Enabled = false;

            txtSIMSN.Enabled = true;

            btnSearchSIMSN.Enabled = false;

            txtSIMSN.Focus();

            Cursor.Current = Cursors.Default;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            fEdit = false;
            dbFunction.ClearTextBox(this);
            //dbFunction.ClearComboBox(this);

            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);
            dbFunction.ClearListViewItems(lvwHistoryList);
            dbFunction.ClearListViewItems(lvwManual);
            
            InitButton();            
            PKTextBoxBackColor(true);

            btnSearchSIMSN.Enabled = true;

            btnRelease.Enabled = false;
            
            txtSIMSN.Focus();

            cboMStatus.Text = cboMAllocation.Text = cboMCarrier.Text = cboMLocation.Text = cboRStatus.Text = clsFunction.sDefaultSelect;

            btnMAddItem.Enabled = btnMRemoveItem.Enabled = false;
            btnMApplyChanges.Enabled = false;
            btnSearchSIMSN.Enabled = true;
            lblSelectedHeader.Text = clsFunction.sNull;
            rtbJSONFormat.Text = clsFunction.sNull;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool isProceed = false;

            if (!dbAPI.isValidSystemVersion()) return;

            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isUpdate, clsUser.ClassUserID, 34)) return;

            if (!ValidateFields()) return;

            DateTime stReceiveDate = dteMReceiveDate.Value;
            string pReceiveDate = stReceiveDate.ToString("yyyy-MM-dd");

            DateTime stDeliveryDate = dteMDeliveryDate.Value;
            string pDeliveryDate = stDeliveryDate.ToString("yyyy-MM-dd");

            DateTime stReleaseDate = dteMReleaseDate.Value;
            string pReleseDate = stReleaseDate.ToString("yyyy-MM-dd");

            int iStatus = int.Parse(dbFunction.CheckAndSetNumericValue(txtSIMStatus.Text));
            int iHoldStatus = int.Parse(dbFunction.CheckAndSetNumericValue(txtHoldStatus.Text));
            
            if (fEdit)
            {
                if (dbFunction.isValidID(txtSIMID.Text))
                {
                    if ((txtLastServiceMade.Text.Equals(clsGlobalVariables.STATUS_PULLEDOUT_DESC) && txtLastServiceActionMade.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS)) ||
                            !dbFunction.isValidID(txtTService.Text))
                    {
                        isProceed = true;
                    }

                    if (dbFunction.isValidID(txtSIMStatus.Text) &&
                        (iHoldStatus.Equals(clsGlobalVariables.STATUS_DAMAGE)) ||
                        (iHoldStatus.Equals(clsGlobalVariables.STATUS_LOSS)) ||
                        (iHoldStatus.Equals(clsGlobalVariables.STATUS_BORROWED)))
                    {
                        isProceed = true;
                    }

                    if (dbFunction.isValidID(txtSIMStatus.Text) &&
                        (iHoldStatus.Equals(clsGlobalVariables.STATUS_INSTALLED)) &&
                        (cboMLocation.Text.Equals(clsSystemSetting.ClassSystemSNLocation)))
                    {
                        isProceed = true;
                    }

                    if ((txtLastServiceMade.Text.Equals(clsGlobalVariables.STATUS_REPLACEMENT_DESC) && txtLastServiceActionMade.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS)))
                    {
                        isProceed = true;
                    }

                    if ((txtLastServiceMade.Text.Equals(clsGlobalVariables.STATUS_PULLED_OUT_DESC) && txtLastServiceActionMade.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS)))
                    {
                        isProceed = true;
                    }

                    if (!dbAPI.isRecordExist("Search", "SIMSN From IRDetail", txtIRIDNo.Text + clsFunction.sPipe + txtSIMID.Text))
                    {
                        isProceed = true;
                    }

                    if (dbAPI.isRecordExist("Search", "SIMSN From IRDetail", txtIRIDNo.Text + clsFunction.sPipe + txtSIMID.Text))
                    {
                        if (iHoldStatus.Equals(clsGlobalVariables.STATUS_AVAILABLE))
                            isProceed = true;
                    }
                }

            }
            else
            {
                isProceed = true;
            }

            if (!isProceed)
            {
                dbFunction.SetMessageBox("SIM SN " + dbFunction.AddBracketStartEnd(txtSIMSN.Text) + "\n\n" + "Unable to update.", "Update failed", clsFunction.IconType.iError);
                return;
            }

            if (fEdit)
            {
                if (iStatus.Equals(clsGlobalVariables.STATUS_INSTALLED))
                {
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iClientID, txtClientID.Text)) return;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iLocation, txtLocationIDFrom.Text)) return;

                    if (!cboMLocation.Text.Equals(clsSystemSetting.ClassSystemSNLocation))
                    {
                        dbFunction.SetMessageBox("Location of this SIM SN must be located in\n" + dbFunction.AddBracketStartEnd(clsSystemSetting.ClassSystemSNLocation), clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                        return;
                    }

                }
            }

            if (!dbFunction.fSavingConfirm(false)) return;

            // Waiting / Hour Glass
            Cursor.Current = Cursors.WaitCursor;

            // get client id
            dbFunction.GetIDFromFile("Client List", cboMClient.Text);
            txtClientID.Text = clsSearch.ClassOutFileID.ToString();

            dbFunction.GetIDFromFile("Carrier", cboMCarrier.Text);
            clsSearch.ClassCarrierID = clsSearch.ClassOutFileID;

            GetMTextBoxID();

            if (fEdit)
            {
                if (lvwManual.Items.Count > 0)
                {
                    // updating
                    BulkUpdateSIMSNDetail(lvwManual, false);

                    // Back to normal 
                    Cursor.Current = Cursors.Default;

                    MessageBox.Show("Manual Multiple SIM SN has been successfully updated.", "Updated",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);

                }
                else
                {
                    if (!ValidateFields()) return;

                    clsSearch.ClassAdvanceSearchValue =
                                                txtSIMID.Text + clsFunction.sPipe +
                                                txtFEID.Text + clsFunction.sPipe +
                                                txtMBatchNo.Text + clsFunction.sPipe +
                                                StrClean(dbFunction.CheckAndSetStringValue(txtSIMSN.Text)) + clsFunction.sPipe +
                                                cboMCarrier.Text + clsFunction.sPipe +
                                                cboMAllocation.Text + clsFunction.sPipe +
                                                txtMRemarks.Text + clsFunction.sPipe +
                                                pDeliveryDate + clsFunction.sPipe +
                                                pReceiveDate + clsFunction.sPipe +
                                                txtSIMStatus.Text + clsFunction.sPipe +
                                                cboMStatus.Text + clsFunction.sPipe +
                                                cboMLocation.Text + clsFunction.sPipe +
                                                cboMAllocation.Text + clsFunction.sPipe +
                                                dbFunction.CheckAndSetNumericValue(txtParticularID.Text) + clsFunction.sPipe +
                                                dbFunction.CheckAndSetNumericValue(txtLocationIDFrom.Text) + clsFunction.sPipe +
                                                dbFunction.CheckAndSetBooleanValue(chkActive.Checked) + clsFunction.sPipe +
                                                clsSearch.ClassCarrierID + clsFunction.sPipe +
                                                pReleseDate + clsFunction.sPipe +
                                                (dbFunction.isValidDescription(cboMClient.Text) ? cboMClient.Text : clsFunction.sZero) + clsFunction.sPipe +
                                                (dbFunction.CheckAndSetNumericValue(txtClientID.Text)) + clsFunction.sPipe +
                                                clsSearch.ClassCurrentParticularID + clsFunction.sPipe +
                                                dbFunction.getCurrentDateTime();

                    Debug.WriteLine("SaveSIMDetail::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                    dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                    dbAPI.ExecuteAPI("PUT", "Update", "SIM Detail", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                    // Back to normal 
                    Cursor.Current = Cursors.Default;

                    MessageBox.Show("Manual SIM has been successfully updated.", "Updated",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);
                }
                
            }
            else
            {
                SaveSIMMaster();

                SaveMSNDetail();

                // Back to normal 
                Cursor.Current = Cursors.Default;
                
                dbFunction.SetMessageBox("Manual SIM successfully saved.", "Saved", clsFunction.IconType.iInformation);
            }

            // Save Activity
            SaveActivity();

            //dbFunction.ClearComboBox(this);
            dbFunction.ClearTextBox(this);
            dbFunction.ClearDataGrid(grdList);
            dbFunction.ClearDataGrid(grdDummy);
            //dbFunction.ClearListView(lvwGenerateList);
            //dbFunction.ClearListView(lvwHistoryList);

            btnClear_Click(this, e);
        }

        private void lvwGenerateList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Get the new sorting column.
            ColumnHeader new_sorting_column = lvwGenerateList.Columns[e.Column];

            // Figure out the new sorting order.
            System.Windows.Forms.SortOrder sort_order;

            if (SortingColumn == null)
            {
                // New column. Sort ascending.
                sort_order = SortOrder.Ascending;
            }
            else
            {
                // See if this is the same column.
                if (new_sorting_column == SortingColumn)
                {
                    // Same column. Switch the sort order.
                    if (SortingColumn.Text.StartsWith("> "))
                    {
                        sort_order = SortOrder.Descending;
                    }
                    else
                    {
                        sort_order = SortOrder.Ascending;
                    }
                }
                else
                {
                    // New column. Sort ascending.
                    sort_order = SortOrder.Ascending;
                }

                // Remove the old sort indicator.
                SortingColumn.Text = SortingColumn.Text.Substring(2);
            }

            // Display the new sort order.
            SortingColumn = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                SortingColumn.Text = "> " + SortingColumn.Text;
            }
            else
            {
                SortingColumn.Text = "< " + SortingColumn.Text;
            }

            // Create a comparer.
            lvwGenerateList.ListViewItemSorter = new clsListView(e.Column, sort_order);

            // Sort.
            lvwGenerateList.Sort();
        }

        private void frmImportSIM_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
                case Keys.F2:
                    if (tabSIM.SelectedIndex == 2) // Manual
                    {
                        btnSearchSIMSN_Click(this, e);
                    }
                    break;
            }
        }
        
        private int GetHeaderColumnIndex(DataGridView obj, string sHeader)
        {
            int iColIndex = 0;

            for (int i = 0; i < obj.ColumnCount; i++)
            {
                string cellParam = obj.Columns[i].Name;

                if (sHeader.CompareTo(cellParam) == 0)
                {
                    iColIndex = i;
                    break;
                }
            }

            return iColIndex;
        }
        
        private void InitTab()
        {
            lblSelectedHeader.Text = "";
            switch (tabSIM.SelectedIndex)
            {
                case 0:
                    lblHeader.Text = "SIM" + " " + "[ " + "IMPORT" + " ]";
                    break;
                case 1:
                    lblHeader.Text = "SIM" + " " + "[ " + "AUTO GENERATE SN" + " ]";
                    break;
                case 2:
                    lblHeader.Text = "SIM" + " " + "[ " + "MANUAL ENTRY" + " ]";
                    txtSIMSN.MaxLength = SerialNoMaxLength;
                    break;
                case 3:
                    lblHeader.Text = "SIM" + " " + "[ " + "RELEASE / TRANSFER" + " ]";
                    break;
            }
        }

        private void btnSearchSIMSN_Click(object sender, EventArgs e)
        {
            if (fAutoLoadData)
            {
                frmSearchField.fSelected = true;
            }
            else
            {
                frmSearchField.iSearchType = frmSearchField.SearchType.iSIM;
                frmSearchField.iStatus = clsFunction.iZero;
                frmSearchField.sHeader = "SIM";
                frmSearchField.iLocationID = clsFunction.iZero;
                frmSearchField.sLocation = clsFunction.sDefaultSelect;
                frmSearchField frm = new frmSearchField();
                frm.ShowDialog();
            }
            
            if (frmSearchField.fSelected)
            {
                Cursor.Current = Cursors.WaitCursor;

                fEdit = true;
                dbFunction.ComBoBoxUnLock(true, this);
                dbFunction.TextBoxUnLock(true, this);

                txtSIMID.Text = clsSearch.ClassSIMID.ToString();
                txtSIMSN.Text = clsSearch.ClassSIMSerialNo;
                
                PopulateSIMTextBox(txtSIMID.Text, txtSIMSN.Text);

                //if (dbFunction.isValidID(txtSIMID.Text))
                //{
                //    if (!dbAPI.isDuplicateSN(int.Parse(txtSIMID.Text), txtSIMSN.Text, clsDefines.SEARCH_SIM)) return;
                //}

                // Load History
                clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtSIMSN.Text) + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();
                
                PopulateMerchantTextBox();

                PopulateLastControlNoTextBox();

                ViewServiceHistory();

                viewSNMinMaxServiceInfo();

                PKTextBoxBackColor(false);

                InitButton();

                btnRelease.Enabled = true;

                dteMReleaseDate.Enabled = true;

                dteMReceiveDate.Enabled = true;

                txtSIMID.ReadOnly = true;

                setSelectedHeader();
                
                Cursor.Current = Cursors.Default;

            }

        }
        private void PopulateSIMTextBox(string sSIMID, string sSIMSN)
        {
            txtSIMStatus.Text =
                txtSIMSN.Text =
                cboMCarrier.Text =
                cboMLocation.Text = 
                txtParticularID.Text =
                cboMAllocation.Text =
                cboMStatus.Text =
                txtMRemarks.Text =
                txtClientID.Text =
                clsFunction.sNull;

            if (dbFunction.isValidID(sSIMID))
            {
                dbAPI.ExecuteAPI("GET", "Search", "SIM SN Info", sSIMID, "Get Info Detail", "", "GetInfoDetail");
                
                if (dbAPI.isNoRecordFound() == false)
                {
                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                    txtSIMID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                    txtSIMStatus.Text = txtHoldStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    cboMStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                    txtSIMSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                    cboMCarrier.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                    cboMLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                    cboMAllocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                    txtMRemarks.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);

                    // -----------------------------------------------------------------------------
                    // Date 
                    // -----------------------------------------------------------------------------
                    string pTemp = "";
                    pTemp = dbFunction.GetDateFromParse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 12), clsFunction.sValueDateFormat, clsFunction.sDateDefaultFormat);
                    dteMDeliveryDate.Value = DateTime.Parse(pTemp);

                    pTemp = dbFunction.GetDateFromParse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13), clsFunction.sValueDateFormat, clsFunction.sDateDefaultFormat);
                    dteMReceiveDate.Value = DateTime.Parse(pTemp);

                    pTemp = dbFunction.GetDateFromParse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 14), clsFunction.sValueDateFormat, clsFunction.sDateDefaultFormat);
                    dteMReleaseDate.Value = DateTime.Parse(pTemp);
                    // -----------------------------------------------------------------------------
                    // Date 
                    // -----------------------------------------------------------------------------

                    txtServiceNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                    txtIRIDNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 11);

                    txtParticularID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 15);

                    txtClientID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 19);                    
                    cboMClient.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 20);

                    txtMUpdatedAt.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 21);
                    txtMUpdatedBy.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 22);
                }
            }
        }
        
        private void tmrSerialNo_Tick(object sender, EventArgs e)
        {
            if (Counter >= ReaderTimeOut) // 1 Seconds
            {
                Counter = 0;
                if (txtSIMSN.Text.Length > SerialNoMinLength)
                {                    
                    tmrSerialNo.Enabled = false;
                    AddSerialNo(txtSIMSN.Text);
                    txtSIMSN.Text = "";
                }
            }
            
            //lblCounter.Text = "Counter: " + Counter.ToString() + " | " + "ReaderInterval: " + ReaderInterval.ToString() + " | " + "ReaderTimeOut: " + ReaderTimeOut.ToString();

            Counter++;
        }

        private void AddSerialNo(string sSerialNo)
        {
            bool fListed = false;
            int LineNo = 1;

            if (!ValidateMFields()) return;

            if (sSerialNo.Length > SerialNoMinLength)
            {
                if (lvwHistoryList.Items.Count > 0)
                {
                    foreach (ListViewItem i in lvwHistoryList.Items)
                    {
                        if (i.SubItems[2].Text == sSerialNo)
                        {
                            fListed = true;
                            break;
                        }

                    }
                }

                if (fListed) // Already on the list
                {
                    dbFunction.SetMessageBox("Serial No " + sSerialNo + " is already on the list.", "SIM Check", clsFunction.IconType.iExclamation);
                    return;
                }
                else
                {
                    ListViewItem item = new ListViewItem(LineNo.ToString());
                    item.SubItems.Add(txtMBatchNo.Text);
                    item.SubItems.Add(sSerialNo);
                    item.SubItems.Add(cboMCarrier.Text);
                    item.SubItems.Add(cboMAllocation.Text);
                    item.SubItems.Add(txtMRemarks.Text);
                    item.SubItems.Add(dteMDeliveryDate.Text);
                    item.SubItems.Add(dteMReceiveDate.Text);
                    lvwHistoryList.Items.Add(item);
                }

                UpdateMLineNo();
            }

            txtSIMSN.Text = "";
        }

        private void txtMSerialNo_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    txtSIMSN.Text = "";
                    break;
                case Keys.Enter:
                    if (fEnableScan)
                    {
                        if (!tmrSerialNo.Enabled)
                        {
                            AddSerialNo(txtSIMSN.Text);
                            txtSIMSN.Text = "";
                        }
                    }                                        
                    break;
                case Keys.Down:
                case Keys.Up:
                    if (fEnableScan)
                    {
                        lvwHistoryList.Focus();
                    }                    
                    break;
            }
        }

        private void lvwList_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    if (lvwHistoryList.SelectedItems.Count > 0)
                    {
                        lvwHistoryList.SelectedItems[0].Remove();
                    }

                    UpdateMLineNo();
                    break;
            }
        }

        private void txtMSerialNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            if (fEnableScan)
            {
                if (txtSIMSN.Text.Length > SerialNoMinLength)
                    tmrSerialNo.Enabled = true;
                else
                {
                    Counter = 0;
                    tmrSerialNo.Enabled = false;
                }
            }            
        }
        private void InitTimerSerialNo()
        {
            bool fEnable = false;
            
            if (chkAutoScan.CheckState == System.Windows.Forms.CheckState.Checked)
            {                
                fEnable = true;
                fEnableScan = true;
                lvwHistoryList.BackColor = Color.GhostWhite;
            }
                
            else
            {                
                fEnable = false;
                fEnableScan = false;
                lvwHistoryList.BackColor = Color.LightGray;
            }
                
            tmrSerialNo.Enabled = fEnable;
            tmrSerialNo.Interval = ReaderInterval;
            
            txtSIMSN.Focus();

            if (fEnable)
            {
                tmrSerialNo.Start();
            }
            else
            {
                Counter = 0;
                tmrSerialNo.Stop();
            }
        }

        private void UpdateMLineNo()
        {
            int LineNo = 1;

            // Update LineNo
            if (lvwHistoryList.Items.Count > 0)
            {
                foreach (ListViewItem i in lvwHistoryList.Items)
                {
                    if (lvwHistoryList.Items.Count > 0)
                    {
                        i.SubItems[0].Text = LineNo.ToString();
                        LineNo++;
                    }
                }
            }
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void chkAutoScan_CheckedChanged(object sender, EventArgs e)
        {
            InitTimerSerialNo();
        }
        private bool CheckSIMDetail()
        {
            bool fExist = false;
            string sSIMDetail = txtSIMSN.Text;

            fExist = dbAPI.CheckSIMDetail("Search", "SIM Detail Check", sSIMDetail);

            if (fExist)
            {
                dbFunction.SetMessageBox("Unable to save SIM Detail." +
                            "\n\n" +
                            "Serial No: " + txtSIMSN.Text +
                            "\n", "Already exist.", clsFunction.IconType.iWarning);


            }

            return fExist;
        }
        private void PKTextBoxBackColor(bool isLock)
        {
            if (isLock)
            {
                txtSIMSN.BackColor = clsFunction.DisableBackColor;
                txtSIMSN.ReadOnly = true;
            }
            else
            {
                if (fEdit)
                {
                    txtSIMSN.BackColor = clsFunction.MKBackColor;
                    txtSIMSN.ReadOnly = true;
                }
                else
                {
                    txtSIMSN.BackColor = clsFunction.EntryBackColor;
                    txtSIMSN.ReadOnly = false;
                }
            }
        }
        private void GetMTextBoxID()
        {
            // Fill List
            dbAPI.GetCarrier();
            dbAPI.GetFEList("View", "Field Engineer List", "", "Particular");

            txtFEID.Text = dbAPI.GetFEFromList(cboMAllocation.Text).ToString();

        }

        private void txtPrefix_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (txtPrefix.Text.Length > 0)
                        txtPadLength.Focus();
                    break;
            }
        }

        private void txtPadLength_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (txtPadLength.Text.Length > 0)
                        txtIndexStart.Focus();
                    break;
            }
        }

        private void txtIndexStart_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (txtIndexStart.Text.Length > 0)
                        txtIndexEnd.Focus();
                    break;
            }
        }

        private void txtIndexEnd_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (txtIndexEnd.Text.Length > 0)
                        btnGenerate.Focus();
                    break;
            }
        }

        private void txtARemarks_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void InitDate()
        {
            dteMDeliveryDate.Value = dteADeliveryDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormatWithWeekDay(dteMDeliveryDate);
            dbFunction.SetDateFormatWithWeekDay(dteADeliveryDate);

            dteMReceiveDate.Value = dteAReceivedDate.Value = dteMReleaseDate.Value = dteMDeliveryDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormatWithWeekDay(dteMReceiveDate);
            dbFunction.SetDateFormatWithWeekDay(dteAReceivedDate);
            dbFunction.SetDateFormatWithWeekDay(dteMReleaseDate);
            dbFunction.SetDateFormatWithWeekDay(dteMDeliveryDate);
            dbFunction.SetDateFormatWithWeekDay(dteMShipmentDate);

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
                MessageBox.Show("[Delivery Date] shouldn't be greater than [Received Date]" +
                                        "\n\n" +
                                        "Delivery Date: " + objFrom.Value.ToString("MM-dd-yyyy") +
                                        Environment.NewLine +
                                        "Received Date:      " + objTo.Value.ToString("MM-dd-yyyy"), "Date Range", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

            }

            return fValid;
        }

        private void InitListView(ListView obj)
        {
            string outField = "";
            int outWidth = 0;
            string outTitle = "";
            HorizontalAlignment outAlign = 0;
            bool outVisible = false;
            bool outAutoWidth = false;
            string outFormat = "";

            obj.Clear();
            obj.View = View.Details;

            dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            obj.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ServiceNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            obj.Columns.Add(outTitle, outWidth, outAlign);


            dbFunction.GetListViewHeaderColumnFromFile("", "ClientID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            obj.Columns.Add(outTitle, outWidth, outAlign);


            dbFunction.GetListViewHeaderColumnFromFile("", "MerchantID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            obj.Columns.Add(outTitle, outWidth, outAlign);


            dbFunction.GetListViewHeaderColumnFromFile("", "FEID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            obj.Columns.Add(outTitle, outWidth, outAlign);


            dbFunction.GetListViewHeaderColumnFromFile("", "Name", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            obj.Columns.Add(outTitle, outWidth, outAlign);


            dbFunction.GetListViewHeaderColumnFromFile("", "TID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            obj.Columns.Add(outTitle, outWidth, outAlign);


            dbFunction.GetListViewHeaderColumnFromFile("", "MID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            obj.Columns.Add(outTitle, outWidth, outAlign);


            //dbFunction.GetListViewHeaderColumnFromFile("", "Client Name", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            obj.Columns.Add(outTitle, outWidth, outAlign);


            //dbFunction.GetListViewHeaderColumnFromFile("", "FE Name", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            obj.Columns.Add(outTitle, outWidth, outAlign);


            dbFunction.GetListViewHeaderColumnFromFile("", "IRIDNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            obj.Columns.Add(outTitle, outWidth, outAlign);


            dbFunction.GetListViewHeaderColumnFromFile("", "Requet ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            obj.Columns.Add(outTitle, outWidth, outAlign);


            //dbFunction.GetListViewHeaderColumnFromFile("", "Request No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            obj.Columns.Add(outTitle, outWidth, outAlign);


            dbFunction.GetListViewHeaderColumnFromFile("", "Request Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            obj.Columns.Add(outTitle, outWidth, outAlign);


            dbFunction.GetListViewHeaderColumnFromFile("", "Job Type", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            obj.Columns.Add(outTitle, outWidth, outAlign);


            dbFunction.GetListViewHeaderColumnFromFile("", "Service Result", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            obj.Columns.Add(outTitle, outWidth, outAlign);


            //dbFunction.GetListViewHeaderColumnFromFile("", "Reference No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            obj.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "FSR Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            obj.Columns.Add(outTitle, outWidth, outAlign);

        }

        private void InitComboBox()
        {

            Cursor.Current = Cursors.WaitCursor;
            
            // Auto Gen
            dbAPI.FillComboBoxTerminalStatus(cboAStatus);
            dbAPI.FillComboBoxFE(cboAAllocation);
            dbAPI.FillComboBoxCarrier(cboACarrier);
            dbAPI.FillComboBoxLocation(cboALocation);

            // Manual
            dbAPI.FillComboBoxTerminalStatus(cboMStatus);
            dbAPI.FillComboBoxFE(cboMAllocation);
            dbAPI.FillComboBoxCarrier(cboMCarrier);
            dbAPI.FillComboBoxLocation(cboMLocation);

            // Release/Transfer
            dbAPI.FillComboBoxTerminalStatus(cboRStatus);
            dbAPI.FillComboBoxLocation(cboRLocationFrom);
            dbAPI.FillComboBoxLocation(cboRLocationTo);

            // Client            
            dbAPI.FillComboBoxClient(cboIClient);
            dbAPI.FillComboBoxClient(cboMClient);
            
            // Lock
            dbFunction.ComBoBoxUnLock(false, this);

            Cursor.Current = Cursors.Default;
        }
        
        private void UpdateButton(bool isClear)
        {
            if (isClear)
            {
                btnAdd.Enabled = true;
                btnSave.Enabled = false;

            }
            else
            {
                btnAdd.Enabled = false;
                btnSave.Enabled = true;
            }
        }

        private void SaveActivity()
        {
            string sRowSQL = "";
            string sSQL = "";

            dbFunction.GetProcessedByAndDateTime(); // Get processedby and datetime

            dbFunction.GetModifiedByAndDateTime(); // Get modifiedby and datetime

            clsSearch.ClassStatus = int.Parse(dbFunction.CheckAndSetNumericValue(txtSIMStatus.Text));
            clsSearch.ClassStatusDescription = cboMStatus.Text;

            if (fEdit)
            {
                clsSearch.ClassOperation = clsGlobalVariables.STATUS_UPDATED;
                clsSearch.ClassOperationDescription = clsGlobalVariables.STATUS_UPDATED_DESC;
            }
            else
            {
                clsSearch.ClassOperation = clsGlobalVariables.STATUS_CREATED;
                clsSearch.ClassOperationDescription = clsGlobalVariables.STATUS_CREATED_DESC;
            }

            sSQL = "";
            sRowSQL = "";
            sRowSQL = " ('" + dbFunction.CheckAndSetNumericValue(txtSIMID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + txtSIMSN.Text + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtParticularID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + cboMAllocation.Text + "', " +
            sRowSQL + sRowSQL + " '" + clsSearch.ClassStatus + "', " +
            sRowSQL + sRowSQL + " '" + clsSearch.ClassStatusDescription + "', " +
            sRowSQL + sRowSQL + " '" + clsSearch.ClassOperation + "', " +
            sRowSQL + sRowSQL + " '" + clsSearch.ClassOperationDescription + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassProcessedDate + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassProcessedTime + "', " +
            sRowSQL + sRowSQL + " '" + txtMRemarks.Text + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassProcessedBy + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassProcessedDateTime + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassModifiedBy + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassModifiedDateTime + "', " +
            sRowSQL + sRowSQL + " '" + clsFunction.sZero + "', " +
            sRowSQL + sRowSQL + " '" + clsFunction.sDash + "') ";
            sSQL = sSQL + sRowSQL;

            Debug.WriteLine("sSQL=" + sSQL);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "SIM Activity", sSQL, "InsertCollectionDetail");

        }

        private void cboACarrier_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboAStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassSIMStatus = 0;
            if (!cboAStatus.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Status List", cboAStatus.Text);
                clsSearch.ClassSIMStatus = clsSearch.ClassOutFileID;
            }

            txtSIMStatus.Text = clsSearch.ClassSIMStatus.ToString();
        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void cboMStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassSIMStatus = 0;
            if (!cboMStatus.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Status List", cboMStatus.Text);
                clsSearch.ClassSIMStatus = clsSearch.ClassOutFileID;
            }

            txtSIMStatus.Text = clsSearch.ClassSIMStatus.ToString();
        }

        private void cboMAllocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassParticularID = 0;
            if (!cboMAllocation.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Particular", cboMAllocation.Text);
                clsSearch.ClassParticularID = clsSearch.ClassOutFileID;
            }

            txtParticularID.Text = clsSearch.ClassParticularID.ToString();
        }

        private void btnRelease_Click(object sender, EventArgs e)
        {
            DateTime stReleaseDate = dteMReleaseDate.Value;
            string pReleseDate = stReleaseDate.ToString("yyyy-MM-dd");

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iSIMSN, txtSIMSN.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iLocation, cboMLocation.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iSIMStatus, cboMStatus.Text)) return;
          
            if (!cboMStatus.Text.Equals(clsGlobalVariables.STATUS_AVAILABLE_DESC))
            {
                dbFunction.SetMessageBox("SIM status must be AVAILABLE to release.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iExclamation);
                return;
            }

            if (MessageBox.Show("Are you sure to release SIM SN " + txtSIMSN.Text + "\n\n" +
                                "to location " + cboMLocation.Text + "?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                return;
            }

            Cursor.Current = Cursors.WaitCursor;

            dbFunction.GetProcessedByAndDateTime(); // Get modifiedby and datetime

            clsSearch.ClassAdvanceSearchValue =
                                                dbFunction.CheckAndSetNumericValue(txtSIMID.Text) + clsFunction.sPipe +
                                                cboMLocation.Text + clsFunction.sPipe +
                                                pReleseDate + clsFunction.sPipe +
                                                clsUser.ClassProcessedBy + clsFunction.sPipe +
                                                clsUser.ClassProcessedDateTime + clsFunction.sPipe +
                                                txtMRemarks.Text;

            Debug.WriteLine("ReleaseTerminal::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbAPI.ExecuteAPI("PUT", "Update", "Released SIM Detail", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

            dbFunction.SetMessageBox("SIM SN " + txtSIMSN.Text + " has been successfully released." + "\n\n" +
                                     "Location " + cboMLocation.Text, "Released", clsFunction.IconType.iInformation);

            btnClear_Click(this, e);

            Cursor.Current = Cursors.Default;
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

        private void cboMLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassLocationIDFrom = 0;
            if (!cboMLocation.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Location", cboMLocation.Text);
                clsSearch.ClassLocationIDFrom = clsSearch.ClassOutFileID;

            }
            txtLocationIDFrom.Text = clsSearch.ClassLocationIDFrom.ToString();
        }

        private void cboMCarrier_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void btnViewHistory_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iSIMSN, txtSIMSN.Text)) return;

            Cursor.Current = Cursors.WaitCursor;

            clsSearch.ClassClientID = clsSearch.ClassMerchantID = clsSearch.ClassJobType = clsSearch.ClassRegionType = clsSearch.ClassRegionID = clsSearch.ClassIRIDNo = clsSearch.ClassStatus = clsSearch.ClassIsPullOut = clsFunction.iZero;
            clsSearch.ClassDateFrom = clsSearch.ClassDateTo = clsFunction.sDateFormat;
            clsSearch.ClassActionMade = clsFunction.sDefaultSelect;
            clsSearch.ClassTerminalSN = clsFunction.sZero;
            clsSearch.ClassSIMSerialNo = txtSIMSN.Text;

            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassClientID + clsFunction.sPipe +
                                                        clsSearch.ClassMerchantID + clsFunction.sPipe +
                                                        clsSearch.ClassJobType + clsFunction.sPipe +
                                                        clsSearch.ClassRegionType + clsFunction.sPipe +
                                                        clsSearch.ClassRegionID + clsFunction.sPipe +
                                                        clsSearch.ClassIRIDNo + clsFunction.sPipe +
                                                        clsSearch.ClassActionMade + clsFunction.sPipe +
                                                        clsSearch.ClassDateFrom + clsFunction.sPipe +
                                                        clsSearch.ClassDateTo + clsFunction.sPipe +
                                                        clsSearch.ClassStatus + clsFunction.sPipe +
                                                        clsSearch.ClassTerminalSN + clsFunction.sPipe +
                                                        clsSearch.ClassSIMSerialNo + clsFunction.sPipe +
                                                        clsSearch.ClassIsPullOut + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsFunction.sZero;

            dbReportFunc.ViewFSROperation();

            Cursor.Current = Cursors.Default;
        }

        private void ViewServiceHistory()
        {
            int i = 0;
            int iLineNo = 0;
            
            dbFunction = new clsFunction();

            //if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalSN, txtSIMSN.Text)) return;

            lvwHistoryList.Items.Clear();

            if (dbFunction.isValidID(txtSIMID.Text))
            {
                dbAPI.ExecuteAPI("GET", "View", "Service SIMSN History List", dbFunction.CheckAndSetNumericValue(txtSIMID.Text), "Advance Detail", "", "ViewAdvanceDetail");

                if (!clsGlobalVariables.isAPIResponseOK) return;

                if (dbAPI.isNoRecordFound() == false)
                {
                    //lvwHistoryList.Items.Clear();
                    while (clsArray.ServiceNo.Length > i)
                    {
                        // Add to List
                        iLineNo++;
                        ListViewItem item = new ListViewItem(iLineNo.ToString());

                        item.ForeColor = dbFunction.GetColorByStatus(0, dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ActionMade)); // set forecolor per actionMade

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_SERVICENO));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FSRNO));

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MERCHANTNAME));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TID));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MID));

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ServiceJobTypeDescription));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_IRNO));

                        // Servicing Date Info
                        string pJSONString = dbAPI.getInfoDetailJSON("Search", "Servicing Date Info", dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_SERVICENO));
                        dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_RequestDate));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_CreatedDate));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ScheduleDate));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_DispatchDate));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ServiceDate));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TicketDate));

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ActionMade));

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TerminalSN));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_SIMSN));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ReplaceTerminalSN));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ReplaceSIMSN));

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_JobTypeStatusDescription));

                        lvwHistoryList.Items.Add(item);

                        i++;
                    }

                    dbFunction.ListViewAlternateBackColor(lvwHistoryList);
                }
            }
            
        }
        
        private void btnRAdd_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isAdd, clsUser.ClassUserID, 31)) return;

            Cursor.Current = Cursors.WaitCursor;

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(true, this);
            dbFunction.ComBoBoxUnLock(true, this);

            fEdit = false;

            UpdateButton(false, tabSIM.SelectedIndex);

            btnRSearch.Enabled = false;

            txtTransNo.Text = dbAPI.GetControlID("SIM Movement Master").ToString();

            // Generate ControlID
            dbAPI.GenerateID(true, txtRRequestNo, txtTransNo, "SIM Movement Master", clsDefines.CONTROLID_PREFIX_SIM);

            // Generate ReferenceNo
            dbAPI.GenerateID(true, txtRReferenceNo, txtTransNo, "SIM Movement Master", clsDefines.CONTROLID_PREFIX_REFNO);

            // Date/Time Created
            dbFunction.GetProcessedByAndDateTime(); // Get processedby and datetime
            txtRDate.Text = clsUser.ClassProcessedDate;

            chkRelease_CheckedChanged(this, e);

            dteRReleaseDate.Enabled = true;

            Cursor.Current = Cursors.Default;
        }

        private void UpdateButton(bool isClear, int index)
        {
            Debug.WriteLine("--UpdateButton--");
            Debug.WriteLine("isClear=" + isClear);
            Debug.WriteLine("index=" + index);
            Debug.WriteLine("fEdit=" + fEdit);

            if (isClear)
            {
                // Import
                btnLoadFile.Enabled = true;
                btnImportSave.Enabled = false;

                // Auto Gen
                btnAddAutoGen.Enabled = true;
                btnSaveAutoGen.Enabled = false;

                // Manual
                btnAdd.Enabled = true;
                btnSave.Enabled = false;

                // Release
                btnRAdd.Enabled = true;
                btnRSave.Enabled = false;

            }
            else
            {
                btnImportSave.Enabled = btnAddAutoGen.Enabled = btnSaveAutoGen.Enabled = btnAdd.Enabled = btnSave.Enabled = btnRAdd.Enabled = btnRSave.Enabled = false;
                switch (index)
                {
                    case 0: // Import
                        btnImportSave.Enabled = true;
                        break;
                    case 1: // Auto Gen
                        btnSaveAutoGen.Enabled = true;
                        break;
                    case 2: // Manual
                        btnSave.Enabled = true;
                        break;
                    case 3: // Release
                        btnRSave.Enabled = true;
                        break;
                }

                btnAdd.Enabled = false;
                btnSave.Enabled = true;
            }
        }

        private void btnRSave_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isUpdate, clsUser.ClassUserID, 31)) return;

            Cursor.Current = Cursors.WaitCursor;

            if (!ValidateFields()) return;

            if (!isValidTransfer()) return;

            if (!dbFunction.isValidListViewChecked(lvwRelease)) return;

            if (!fConfirm(fEdit)) return;

            if (!fEdit)
            {
                SaveMovementMaster();
                SaveMovementDetail();                
            }
            else
            {
                // Update here...
            }

            if (lvwRelease.Items.Count > 0)
            {
                BulkUpdateSIMSNDetail(lvwRelease, true);
            }

            Cursor.Current = Cursors.Default;

            if (fEdit)
                dbFunction.SetMessageBox("Release/transfer has been successfully updated.", "Updated", clsFunction.IconType.iExclamation);
            else
                dbFunction.SetMessageBox("Release/transfer has been successfully added.", "Saved", clsFunction.IconType.iInformation);


            btnRClear_Click(this, e);
        }

        private bool isValidTransfer()
        {
            bool isValid = false;

            if (cboRLocationFrom.Text.Equals(cboRLocationTo.Text))
            {
                dbFunction.SetMessageBox("Location [From] and [To] must not be equal.", "Check Field", clsFunction.IconType.iError);
            }
            else
            {
                isValid = true;
            }

            return isValid;
        }

        private bool fConfirm(bool fUpdate)
        {
            bool fConfirm = true;
            string sMessage = clsFunction.sPipe;

            sMessage =
                        "Confirm details below:" + "\n" +
                        clsFunction.sLineSeparator + "\n" +
                       "Mode: " + (chkRelease.Checked ? "Releease" : "Transfer") + "\n" +
                        clsFunction.sLineSeparator + "\n" +
                        " >Trans No.: " + txtTransNo.Text + "\n" +
                        " >Request No.: " + txtRRequestNo.Text + "\n" +
                        " >Date: " + txtRDate.Text + "\n" +
                        " >Reference No: " + txtRReferenceNo.Text + "\n" +
                        " >Location From: " + cboRLocationFrom.Text + "\n" +
                        " >Location To: " + cboRLocationTo.Text + "\n" +
                        " >Status: " + cboRStatus.Text + "\n" +
                        " >Remarks: " + txtRRemarks.Text;

            if (MessageBox.Show("Are you sure you want to " + (fUpdate == true ? "update" : "save") + " " + "record?" +
                                    "\n\n" +
                                    sMessage,
                                    (fUpdate ? "Confirm update" : "Confirm save"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fConfirm = false;
            }

            return fConfirm;
        }

        private void btnRClear_Click(object sender, EventArgs e)
        {
            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);
            dbFunction.ClearListViewItems(lvwRelease);
            dbFunction.ClearListViewItems(lvwManual);

            fEdit = false;
            UpdateButton(true, 4);

            ClearListView();

            chkRelease.Checked = true;
            cboRLocationTo.Enabled = false;
            cboRLocationFrom.Enabled = false;
            cboRStatus.Enabled = false;
            dteRReleaseDate.Enabled = false;

            lblSelectedHeader.Text = lblMode.Text = clsFunction.sDash;

            chkRelease_CheckedChanged(this, e);
            
        }

        private void ClearListView()
        {
            lvwGenerateList.Items.Clear();           
            lvwRelease.Items.Clear();
        }

        private void btnRAddItem_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iFromLocation, cboRLocationFrom.Text)) return;

            frmSearchField.iSearchType = frmSearchField.SearchType.iSIM;
            frmSearchField.iStatus = clsGlobalVariables.STATUS_AVAILABLE;
            frmSearchField.sTerminalType = "View SIM";
            frmSearchField.sHeader = "SIM";
            frmSearchField.isCheckBoxes = true;
            frmSearchField.sLocation = cboRLocationFrom.Text;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    LoadSelected(lvwRelease);
                    
                    chkRAll.Checked = false;

                    Cursor.Current = Cursors.Default;
                }
                catch (Exception ex)
                {
                    dbFunction.SetMessageBox("Error message " + ex.Message + "\n\n" + clsDefines.CONTACT_ADMIN_MESSAGE, "Manual Terminal", clsFunction.IconType.iError);
                }
                
            }
        }

        private void LoadSelected(ListView lvw)
        {
            int i = 0;
            int iLineNo = 0;

            if (clsArray.ID.Length > 0)
            {
                while (clsArray.ID.Length > i)
                {

                    // Check if item already exist
                    if (!isItemOnList(clsArray.ID[i], clsArray.Description[i]))
                    {
                        // Add to List
                        iLineNo++;
                        ListViewItem item = new ListViewItem(iLineNo.ToString());

                        item.SubItems.Add(clsArray.ID[i].ToString());


                        // Get info
                        dbAPI.ExecuteAPI("GET", "Search", "SIM SN Info", clsArray.ID[i], "Get Info Detail", "", "GetInfoDetail");
                        
                        if (dbAPI.isNoRecordFound() == false)
                        {
                            dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                            string sStatus = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                            string sSerialNo = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                            string sCarrier = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                            string sLocation = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                            
                            item.SubItems.Add(sSerialNo);                         
                            item.SubItems.Add(sCarrier);
                            item.SubItems.Add(sLocation);
                            item.SubItems.Add(sStatus);

                        }

                        lvw.Items.Add(item);
                    }

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(lvw);

                dbFunction.UpdateListViewLineNo(lvw); // Update ListView LineNo

                // Focus first item
                if (lvw.Items.Count > 0)
                {
                    lvw.FocusedItem = lvw.Items[0];
                    lvw.Items[0].Selected = true;
                    lvw.Select();
                }
            }
        }

        private bool isItemOnList(string pProductID, string pDescrption)
        {
            bool isListed = false;
            if (lvwRelease.Items.Count > 0)
            {
                foreach (ListViewItem i in lvwRelease.Items)
                {
                    Debug.WriteLine("i=" + i + ">>" + i.SubItems[1].Text + " is equal with " + pProductID);
                    //Debug.WriteLine("i=" + i + ">>" + i.SubItems[3].Text + " is equal with " + pDescrption);

                    if ((i.SubItems[1].Text.Equals(pProductID)))
                    {
                        isListed = true;
                    }
                }
            }

            return isListed;
        }
        
        private void SaveMovementMaster()
        {
            string sRowSQL = "";
            string sSQL = "";
            int iFromLocationID = clsFunction.iZero;
            int iToLocationID = clsFunction.iZero;

            DateTime stReleaseDate = dteMReleaseDate.Value;
            string pReleseDate = stReleaseDate.ToString("yyyy-MM-dd");

            Debug.WriteLine("--SaveMovementMaster--");
            Debug.WriteLine("fEdit=" + fEdit);

            dbFunction.GetProcessedByAndDateTime(); // Get processedby and datetime

            dbFunction.GetModifiedByAndDateTime(); // Get modifiedby and datetime

            // Location From
            clsSearch.ClassOutFileID = 0;
            dbFunction.GetIDFromFile("Location", cboRLocationFrom.Text);
            iFromLocationID = clsSearch.ClassOutFileID;

            // Location To
            clsSearch.ClassOutFileID = 0;
            dbFunction.GetIDFromFile("Location", cboRLocationTo.Text);
            iToLocationID = clsSearch.ClassOutFileID;

            sRowSQL = "";
            sRowSQL = " ('" + clsUser.ClassProcessedDate + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassProcessedTime + "', " +
            sRowSQL + sRowSQL + " '" + pReleseDate + "', " +
            sRowSQL + sRowSQL + " " + iFromLocationID + ", " +
            sRowSQL + sRowSQL + " '" + cboRLocationFrom.Text + "', " +
            sRowSQL + sRowSQL + " " + iToLocationID + ", " +
            sRowSQL + sRowSQL + " '" + cboRLocationTo.Text + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassUserID + "', " +
            sRowSQL + sRowSQL + " '" + txtRRequestNo.Text + "', " +
            sRowSQL + sRowSQL + " '" + txtRReferenceNo.Text + "', " +
            sRowSQL + sRowSQL + " '" + txtRRemarks.Text + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassProcessedBy + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassProcessedDateTime + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassModifiedBy + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassModifiedDateTime + "') ";
            sSQL = sSQL + sRowSQL;

            Debug.WriteLine("sSQL=" + sSQL);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "SIM Movement Master", sSQL, "InsertCollectionMaster");

            txtTIID.Text = clsLastID.ClassLastInsertedID.ToString();
        }

        private void SaveMovementDetail()
        {
            string sRowSQL = "";
            string sSQL = "";

            Debug.WriteLine("--SaveMovementDetail--");
            Debug.WriteLine("fEdit=" + fEdit);

            sSQL = "";

            dbFunction.GetProcessedByAndDateTime();

            foreach (ListViewItem x in lvwRelease.Items)
            {
                int iSIMID = int.Parse(x.SubItems[1].Text);              
                string sSerialNo = x.SubItems[2].Text;
                string sCarrier = x.SubItems[3].Text;

                // Insert                
                sRowSQL = "";
                sRowSQL = " (" + dbFunction.CheckAndSetNumericValue(txtTIID.Text) + ", " +
                sRowSQL + sRowSQL + " " + iSIMID + ", " +
                sRowSQL + sRowSQL + " '" + sSerialNo + "', " +                
                sRowSQL + sRowSQL + " '" + sCarrier + "') ";

                if (sSQL.Length > 0)
                    sSQL = sSQL + ", " + sRowSQL;
                else
                    sSQL = sSQL + sRowSQL;

            }

            Debug.WriteLine("sSQL=" + sSQL);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "SIM Movement Detail", sSQL, "InsertCollectionDetail");
        }

        private void BulkUpdateSIMSNDetail(ListView lvw, bool isRelease)
        {
            Debug.WriteLine("--BulkUpdateSIMSNDetail--");
            Debug.WriteLine("isRelease=" + isRelease);
            Debug.WriteLine("fEdit=" + fEdit);

            string sRowSQL = "";
            string sSQL = "";
            int iSIMID = 0;
            string sSerialNo = "";
            string sLocation = "";
            DateTime stReleaseDate = (isRelease ? dteRReleaseDate.Value : dteMReleaseDate.Value);
            string pReleseDate = stReleaseDate.ToString("yyyy-MM-dd");

            string pLocationID = dbFunction.CheckAndSetNumericValue(isRelease ? txtLocationIDTo.Text : txtLocationIDFrom.Text);
            string pLocation = dbFunction.CheckAndSetNumericValue(isRelease ? cboRLocationTo.Text : cboRLocationFrom.Text);
            string pRemarks = dbFunction.CheckAndSetStringValue(isRelease ? txtRRemarks.Text : txtMRemarks.Text);
            string pMovementType = (chkRelease.Checked ? clsDefines.MOVEMENTTYPE_RELEASE : clsDefines.MOVEMENTTYPE_TRANSFER);
            
            dbFunction.GetProcessedByAndDateTime(); // Get processedby and datetime

            foreach (ListViewItem i in lvw.Items)
            {
                sSQL = "";

                sLocation = cboRLocationTo.Text;

                foreach (ListViewItem x in lvw.Items)
                {
                    iSIMID = int.Parse(x.SubItems[1].Text);
                    sSerialNo = x.SubItems[2].Text;

                    // Update
                    sRowSQL = "";
                    sRowSQL = iSIMID + "~" +
                    sRowSQL + sRowSQL + "" + dbFunction.CheckAndSetNumericValue(txtSIMStatus.Text) + "~" +
                    sRowSQL + sRowSQL + "" + cboRStatus.Text + "~" +
                    sRowSQL + sRowSQL + "" + pLocationID + "~" +
                    sRowSQL + sRowSQL + "" + pLocation + "~" +
                    sRowSQL + sRowSQL + "" + clsUser.ClassUserID + "~" +
                    sRowSQL + sRowSQL + "" + clsUser.ClassProcessedDateTime + "~" +
                    sRowSQL + sRowSQL + "" + pRemarks + "~" +
                    sRowSQL + sRowSQL + "" + pReleseDate + "~" +
                    sRowSQL + sRowSQL + "" + dbFunction.CheckAndSetNumericValue(txtTransNo.Text) + "~" +
                    sRowSQL + sRowSQL + "" + pMovementType;

                    Debug.WriteLine("sRowSQL=" + sRowSQL + "\n");

                    dbFunction.parseDelimitedString(sRowSQL, clsDefines.gTilde, 1);

                    if (sSQL.Length > 0)
                        sSQL = sSQL + "|" + sRowSQL;
                    else
                        sSQL = sSQL + sRowSQL;

                }

                sSQL = lvw.Items.Count.ToString() + "^" + sSQL;

                Debug.WriteLine("sSQL=" + sSQL);

                dbFunction.parseDelimitedString(sSQL, clsDefines.gCaret, 1);

                dbAPI.ExecuteAPI("PUT", "Update", "Multiple SIM Detail", sSQL, "", "", "UpdateBulkCollectionDetail");
            }

        }

        private void lvwRelease_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwRelease.SelectedItems.Count > 0)
            {
                string LineNo = lvwRelease.SelectedItems[0].Text;
                txtLineNo.Text = LineNo;

                if (LineNo.Length > 0)
                {
                    // Selected
                    lblSelectedHeader.Text = clsFunction.sNull;
                    lblSelectedHeader.Text = "ITEM LIST >" +
                                             "LINE#" + txtLineNo.Text +
                                             "." +
                                             "[ " +
                                             lvwRelease.SelectedItems[0].SubItems[2].Text + "," +
                                             lvwRelease.SelectedItems[0].SubItems[3].Text + "," +                                            
                                             lvwRelease.SelectedItems[0].SubItems[4].Text +
                                             " ]";
                }
            }
        }

        private void btnRSearch_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iReleaseSIM;
            frmSearchField.sTerminalType = clsFunction.sNull;
            frmSearchField.sHeader = "SIM Release";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {

            }
        }

        private void cboRLocationFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassLocationIDFrom = 0;
            if (!cboRLocationFrom.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Location", cboRLocationFrom.Text);
                clsSearch.ClassLocationIDFrom = clsSearch.ClassOutFileID;

            }
            txtLocationIDFrom.Text = clsSearch.ClassLocationIDFrom.ToString();
        }

        private void cboRLocationTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassLocationIDTo = 0;
            if (!cboRLocationTo.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Location", cboRLocationTo.Text);
                clsSearch.ClassLocationIDTo = clsSearch.ClassOutFileID;

            }
            txtLocationIDTo.Text = clsSearch.ClassLocationIDTo.ToString();
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

                ucStatusDisplay.SetStatus($"Total data import: [{grdList.RowCount}]", Enums.StatusType.Success);
            }
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            if (lvwRelease.Items.Count > 0)
            {
                if (chkRAll.Checked)
                {
                    chkRAll.Text = "UNCHECK ALL SERIAL NO.";

                    // Check All
                    foreach (ListViewItem i in lvwRelease.Items)
                    {
                        i.Checked = true;
                    }
                }
                else
                {
                    chkRAll.Text = "CHECK ALL SERIAL NO.";

                    // UnCheck All
                    foreach (ListViewItem i in lvwRelease.Items)
                    {
                        i.Checked = false;
                    }
                }
            }
        }

        private void cboALocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassLocationIDFrom = 0;
            if (!cboALocation.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Location", cboALocation.Text);
                clsSearch.ClassLocationIDFrom = clsSearch.ClassOutFileID;

            }
            txtLocationIDFrom.Text = clsSearch.ClassLocationIDFrom.ToString();
        }

        private void chkRelease_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRelease.Checked)
            {
                //cboRLocationTo.Enabled = true;
                //cboRLocationFrom.Enabled = true;
                //dteRReleaseDate.Enabled = true;
                //btnRApply.Enabled = false;                
            }
            else
            {
                //cboRLocationTo.Enabled = true;
                //cboRLocationFrom.Enabled = false;
                //dteRReleaseDate.Enabled = false;
                //btnRApply.Enabled = true;
            }

            lblMode.Text = "MODE: " + (chkRelease.Checked ? "RELEASE" : "TRANSFER");
        }

        private void chkMAll_CheckedChanged(object sender, EventArgs e)
        {
            dbFunction.checkAllListView(lvwManual, chkMAll);
        }

        private void btnMAddItem_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iSIM;
            frmSearchField.iStatus = 99; // ROCKY - IMPORT SIM ISSUE: FIXED DISPLAYING INSTALLED STATUS
            frmSearchField.sTerminalType = "View SIM";
            frmSearchField.sHeader = "MANUAL - SIM";
            frmSearchField.isCheckBoxes = true;
            frmSearchField.sLocation = cboRLocationFrom.Text;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    fEdit = true;

                    // disable manual object
                    txtSIMSN.Enabled = false;
                    cboMCarrier.Enabled = cboMAllocation.Enabled = false;

                    dteMReceiveDate.Enabled = false;
                    btnMApplyChanges.Enabled = true;

                    LoadSelected(lvwManual);
                    
                    Cursor.Current = Cursors.Default;
                }
                catch (Exception ex)
                {
                    dbFunction.SetMessageBox("Error message " + ex.Message + "\n\n" + clsDefines.CONTACT_ADMIN_MESSAGE, "Manual SIM", clsFunction.IconType.iError);
                }

            }
        }

        private void btnMRemoveItem_Click(object sender, EventArgs e)
        {
            dbFunction.removeItemListView(lvwManual, true);
        }

        private void btnRRemoveItem_Click(object sender, EventArgs e)
        {
            dbFunction.removeItemListView(lvwRelease, true);
        }

        private void btnMApplyChanges_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidListViewChecked(lvwManual)) return;

            if (MessageBox.Show("Are you sure you want to apply changes to list?" +
                                    "\n\n" +
                                    " >Location: " + cboMLocation.Text + "\n" +
                                    " >Status: " + cboMStatus.Text,
                                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                return;
            }

            dbFunction.updateListView(lvwManual, 4, cboMLocation.Text, true); // location
            dbFunction.updateListView(lvwManual, 5, cboMStatus.Text, true); // status
        }

        /*
        private void BulkUpdateSIMDetail(ListView lvw, bool isRelease)
        {
            Debug.WriteLine("--BulkUpdateSIMDetail--");
            Debug.WriteLine("isRelease=" + isRelease);
            Debug.WriteLine("fEdit=" + fEdit);

            string sRowSQL = "";
            string sSQL = "";
            int iSIMID = 0;
            string sSerialNo = "";
            DateTime stReleaseDate = dteMReleaseDate.Value;
            string pReleseDate = stReleaseDate.ToString("yyyy-MM-dd");

            string pLocationID = dbFunction.CheckAndSetNumericValue(isRelease ? txtLocationIDTo.Text : txtLocationIDFrom.Text);
            string pLocation = dbFunction.CheckAndSetNumericValue(isRelease ? cboRLocationTo.Text : cboRLocationFrom.Text);
            string pRemarks = dbFunction.CheckAndSetStringValue(isRelease ? txtRRemarks.Text : txtMRemarks.Text);            
            string pMovementType = (chkRelease.Checked ? clsDefines.MOVEMENTTYPE_RELEASE : clsDefines.MOVEMENTTYPE_TRANSFER);
            
            dbFunction.GetProcessedByAndDateTime(); // Get processedby and datetime

            foreach (ListViewItem i in lvw.Items)
            {
                sSQL = "";

                foreach (ListViewItem x in lvw.Items)
                {
                    iSIMID = int.Parse(x.SubItems[1].Text);
                    sSerialNo = x.SubItems[4].Text;

                    // Update
                    sRowSQL = "";
                    sRowSQL = iSIMID + "~" +
                    sRowSQL + sRowSQL + "" + dbFunction.CheckAndSetNumericValue(txtSIMStatus.Text) + "~" +
                    sRowSQL + sRowSQL + "" + cboRStatus.Text + "~" +
                    sRowSQL + sRowSQL + "" + pLocationID + "~" +
                    sRowSQL + sRowSQL + "" + pLocation + "~" +                    
                    sRowSQL + sRowSQL + "" + clsUser.ClassUserID + "~" +
                    sRowSQL + sRowSQL + "" + clsUser.ClassProcessedDateTime + "~" +
                    sRowSQL + sRowSQL + "" + pRemarks + "~" +
                    sRowSQL + sRowSQL + "" + dbFunction.CheckAndSetNumericValue(txtTransNo.Text) + "~" +
                    sRowSQL + sRowSQL + "" + pMovementType;

                    Debug.WriteLine("sRowSQL=" + sRowSQL + "\n");

                    dbFunction.parseDelimitedString(sRowSQL, clsDefines.gTilde, 1);

                    if (sSQL.Length > 0)
                        sSQL = sSQL + "|" + sRowSQL;
                    else
                        sSQL = sSQL + sRowSQL;

                }

                sSQL = lvw.Items.Count.ToString() + "^" + sSQL;

                Debug.WriteLine("sSQL=" + sSQL);

                dbAPI.ExecuteAPI("PUT", "Update", "Multiple SIM Detail", sSQL, "", "", "UpdateBulkCollectionDetail");
            }

        }
        */
        private void PopulateMerchantTextBox()
        {
            Debug.WriteLine("--PopulateMerchantTextBox--");

            txtMerchName.Text =
            txtMerchTID.Text =
            txtMerchMID.Text =
            txtAssignTerminalSN.Text =
            txtAssignSIMSN.Text =
            txtAssignComponents.Text =
            txtIRStatusDescription.Text =
            txtMerchRegion.Text = 
            clsFunction.sNull;

            try
            {
                if (dbFunction.isValidID(txtSIMID.Text))
                {
                    clsSearch.ClassOutParamValue = dbAPI.getInfoDetailJSON("Search", "Merchant Assigned SN Info", txtSIMID.Text + clsFunction.sPipe + clsDefines.SEARCH_SIM);

                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gComma, 0);

                    if (dbFunction.isValidLen(clsSearch.ClassOutParamValue))
                    {
                        txtIRIDNo.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_IRIDNO);
                        txtMerchName.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_MERCHANTNAME);
                        txtMerchTID.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_TID);
                        txtMerchMID.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_MID);
                        txtMerchRegion.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_REGION);
                        txtMerchProvince.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_Province);

                        txtAssignTerminalSN.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_TerminalSN);
                        txtAssignSIMSN.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_SIMSN);
                        txtIRStatusDescription.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_IRStatusDescription);
                    }
                }

                if (dbFunction.isValidID(txtServiceNo.Text))
                {
                    clsSearch.ClassOutParamValue = dbAPI.getInfoDetailJSON("Search", "Merchant SN Info", txtIRIDNo.Text + clsFunction.sPipe + txtServiceNo.Text);

                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gComma, 0);
                    
                    if (dbFunction.isValidLen(clsSearch.ClassOutParamValue))
                    {   
                        txtServiceNo.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_SERVICENO);                        
                        txtFSRNo.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_FSRNO);                        
                    }
                }
            }
            catch (Exception ex)
            {
                //dbFunction.SetMessageBox("Exceptional error " + ex.Message, "Exceptional error", clsFunction.IconType.iWarning);
                Debug.WriteLine("Exceptional error " + ex.Message);
            }
     
        }

        private void btnRefresh_Click_1(object sender, EventArgs e)
        {
            ViewServiceHistory();
        }

        private void PopulateLastControlNoTextBox()
        {
            Debug.WriteLine("--PopulateLastControlNoTextBox--");

            txtLastFSRNo.Text =
            txtLastServiceNo.Text =
            txtLastIRIDNo.Text =
            txtLastMerchantID.Text =
            txtLastClientID.Text =
            txtLastFEID.Text =
            txtTService.Text = 
            txtTFSR.Text = clsFunction.sNull;

            try
            {
                if (dbFunction.isValidID(txtSIMID.Text))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "Last ControlNo Info", clsDefines.TAG_SIM + clsFunction.sPipe + txtSIMID.Text, "Get Info Detail", "", "GetInfoDetail");

                    Debug.WriteLine("clsSearch.ClassOutParamValue=" + clsSearch.ClassOutParamValue);

                    if (clsSearch.ClassOutParamValue.Length > 0)
                    {
                        jsonObj obj = JsonConvert.DeserializeObject<jsonObj>(clsSearch.ClassOutParamValue);

                        if (dbAPI.isNoRecordFound() == false)
                        {
                            txtLastFSRNo.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_FSRNO);
                            txtLastServiceNo.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_SERVICENO);
                            txtLastIRIDNo.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_IRIDNO);
                            txtLastMerchantID.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_MerchantID);
                            txtLastClientID.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_ClientID);
                            txtLastFEID.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_FEID);
                            txtTService.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_ServiceCount);
                            txtTFSR.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_FSRCount);                          
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox("Exceptional error " + ex.Message, "Exceptional error", clsFunction.IconType.iWarning);
            }

        }

        private void setSelectedHeader()
        {
            lblSelectedHeader.Text = txtSIMSN.Text + clsDefines.gPipe +
                                     cboMLocation.Text + clsDefines.gPipe +
                                     cboMStatus.Text + clsDefines.gPipe +
                                     cboMClient.Text + clsDefines.gPipe +
                                     txtMerchName.Text;

        }

        private void btnClearTerminalSN_Click(object sender, EventArgs e)
        {
            btnClear_Click(this, e);
        }

        private void viewSNMinMaxServiceInfo()
        {
            txtFirstServiceMade.Text = txtLastServiceMade.Text = txtLastServiceActionMade.Text = clsFunction.sNull;

            if (dbFunction.isValidID(txtSIMID.Text))
            {
                // Min
                dbAPI.ExecuteAPI("GET", "Search", "SN MIN/MAX Service Info", txtSIMID.Text + clsFunction.sPipe + clsDefines.TAG_SIM + clsFunction.sPipe + clsDefines.TAG_MIN, "Get Info Detail", "", "GetInfoDetail");

                if (clsSearch.ClassOutParamValue.Length > 0)
                {
                    jsonObj obj = JsonConvert.DeserializeObject<jsonObj>(clsSearch.ClassOutParamValue);

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtFirstServiceMade.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_ServiceJobTypeDescription);
                    }
                }

                // Max
                dbAPI.ExecuteAPI("GET", "Search", "SN MIN/MAX Service Info", txtSIMID.Text + clsFunction.sPipe + clsDefines.TAG_SIM + clsFunction.sPipe + clsDefines.TAG_MAX, "Get Info Detail", "", "GetInfoDetail");

                if (clsSearch.ClassOutParamValue.Length > 0)
                {
                    jsonObj obj = JsonConvert.DeserializeObject<jsonObj>(clsSearch.ClassOutParamValue);

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtLastServiceMade.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_ServiceJobTypeDescription);
                        txtLastServiceActionMade.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_ActionMade);
                    }
                }

            }
        }

        private void lvwHistoryList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwHistoryList.Items.Count > 0)
            {
                string pSelectedRow = dbFunction.GetListViewSelectedRow(lvwHistoryList, 0);
                Debug.WriteLine("pSelectedRow=\n" + pSelectedRow);

                rtbJSONFormat.Text = clsFunction.sNull;
                rtbJSONFormat.Text = pSelectedRow;

                tabInfo.SelectedIndex = 1;
                tabService.SelectedIndex = 1;

                string jsonResult = dbFunction.genJSONFormat(lvwHistoryList, lvwHistoryList.SelectedIndices[0], "", "");
                Debug.WriteLine($"Selected row={pSelectedRow}, jsonResult={jsonResult}");

                // Pass JSON to popup window
                frmPopUpInfo frm = new frmPopUpInfo(jsonResult);
                frm.ShowDialog();
            }
        }

        private void cboMClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassClientID = 0;
            if (!cboMClient.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Client List", cboMClient.Text);
                clsSearch.ClassClientID = clsSearch.ClassOutFileID;

            }
            txtClientID.Text = clsSearch.ClassClientID.ToString();
        }

        private void grdList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (e.RowIndex >= 0)
            {
                var dataIndexNo = grdList.Rows[e.RowIndex].Index.ToString();
                string cellValue = grdList.Rows[e.RowIndex].Cells[1].Value.ToString();

                if (int.Parse(dataIndexNo) >= 0)
                {
                    int pLineNo = int.Parse(dataIndexNo);

                    string rawdata_info = dbFunction.genJSONFormat(grdList, pLineNo, "", "");
                    Debug.WriteLine("rawdata_info=" + rawdata_info);
                    dbFunction.populateListViewFromJsonString(dgvRaw, rawdata_info, "", "");
                }
            }

            Cursor.Current = Cursors.Default;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Your custom logic that should run during the Load event
            //MessageBox.Show("Form2 Load event triggered!");
        }

        private void btnRApply_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidListViewChecked(lvwRelease)) return;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalStatus, cboRStatus.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iBatchNo, txtTransNo.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iFromLocation, cboRLocationFrom.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iToLocation, cboRLocationTo.Text)) return;

            dbFunction.SetMessageBox($"This will {(chkRelease.Checked ? "release" : "transfer")} SN(s) to the selected [Location To] and update the [Status].",
                                    "Release/transfer",
                                    clsFunction.IconType.iInformation
                                    );

            dbFunction.updateListView(lvwRelease, 4, cboRLocationTo.Text, false); // LocationTo
            dbFunction.updateListView(lvwRelease, 5, cboRStatus.Text, false); // Status
        }

        private void lvwRelease_DoubleClick(object sender, EventArgs e)
        {
            if (lvwRelease.Items.Count > 0)
            {
                string pSelectedRow = dbFunction.GetListViewSelectedRow(lvwRelease, 0);
                Debug.WriteLine("pSelectedRow=\n" + pSelectedRow);

                string jsonResult = dbFunction.genJSONFormat(lvwRelease, lvwRelease.SelectedIndices[0], "", "");
                Debug.WriteLine($"Selected row={pSelectedRow}, jsonResult={jsonResult}");

                // Pass JSON to popup window
                frmPopUpInfo frm = new frmPopUpInfo(jsonResult);
                frm.ShowDialog();
            }
        }

        private void cboRStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassSIMStatus = 0;
            if (!cboRStatus.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Status List", cboRStatus.Text);
                clsSearch.ClassSIMStatus = clsSearch.ClassOutFileID;
            }

            txtSIMStatus.Text = clsSearch.ClassSIMStatus.ToString();
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {

        }

        private void btnCopyClipboard_Click(object sender, EventArgs e)
        {
            dbFunction.CopyGridToClipboard(grdList);

            dbFunction.SetMessageBox($"Data list copied to clipboard!", lblHeader.Text, clsFunction.IconType.iInformation);

        }

        private void btnRCopyClipboard_Click(object sender, EventArgs e)
        {
            dbFunction.CopyListViewToClipboard(lvwRelease);

            dbFunction.SetMessageBox($"Data list copied to clipboard!", lblHeader.Text, clsFunction.IconType.iInformation);
        }
    }
}
