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
using System.Text.RegularExpressions;

namespace MIS
{
    public partial class frmImportTerminal : Form
    {
        public static bool fAutoLoadData = false;
        public static int iTab;
        public static int iTabSub;
        public static string sHeader;

        private clsAPI dbAPI;
        private clsINI dbSetting;
        private clsFile dbFile;
        private clsFunction dbFunction;
        private clsReportFunc dbReportFunc;

        private static string ExcelFilePath = @"";
        private string sExcelFileName = "";
        private string sSheet = "";
        private int iEndLimit = 10000;
        private int iColumnWidth = 100;

        private bool fEdit = false;
        // The column we are currently using for sorting.
        private ColumnHeader SortingColumn = null;

        int Counter = 1;
        int ReaderInterval;
        int ReaderTimeOut;
        int SerialNoMaxLength;
        int SerialNoMinLength = 5;
        bool fEnableScan = false;

        int delay = 5; // 500 default

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

        public frmImportTerminal()
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
                    if (dbAPI.isImportFileName("Search", "Terminal Import", sExcelFileName))
                    {                        
                        dbFunction.SetMessageBox("Import failure:" + "\n" +
                                                 sExcelFileName + " was already processed.", "Import Terminal", clsFunction.IconType.iWarning);

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

                    // Start Import Here
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

                    ucStatusDisplay.SetStatus("Validating import file data", Enums.StatusType.Processing);
                    Task.Delay(delay); // Asynchronously wait without blocking UI

                    // check for mandatory fields
                    if (!dbFunction.isValidDataGridValue(clsFunction.ImportType.iImportTerminal, dbAPI, grdDummy, txtFileName.Text, 0, false)) return;

                    // check field column is required
                    if (!dbFunction.isValidDataGridValue(clsFunction.ImportType.iImportTerminal, dbAPI, grdDummy, txtFileName.Text, 1, false)) return;

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
                                             , "Import Terminal", clsFunction.IconType.iInformation);
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
            int y = 1;
            int ii = 0;
            bool isValid = true;

            // Delivery Date            
            DateTime DDateTime = DateTime.Now;
         
            // Received Date        
            DateTime RDateTime = DateTime.Now;        
            
            dbFunction.InitDataGridView(grdList);

            iRowCount = grdDummy.RowCount;
            iColCount = 15;
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
                    if (cellParam.CompareTo("SERIAL NO") == 0)
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
                Debug.WriteLine(y.ToString() + "," + cellParam + "," + cellParam + "," + "0" + "," + y.ToString() + "," + "TERMINAL");
            }            

            for (i = 0; i < iRowCount; i++)
            {
                ii++;

                string sLineNo = grdDummy.Rows[i].Cells[0].Value.ToString(); // Check is not blank
                string sFieldCheck = grdDummy.Rows[i].Cells[1].Value.ToString(); // Check is not blank
                
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

            //dbFunction.DataGridViewAlternateBackColor(grdList);  

            grdList.ResumeLayout(); // Resume UI updates

            return isValid;
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

            if (grdInventory.RowCount > 0)
            {
                if (grdInventory.DataSource != null)
                {
                    grdInventory.DataSource = null;
                    grdInventory.Rows.Clear();
                }
                else
                {
                    grdInventory.Rows.Clear();
                }
                grdInventory.Refresh();
            }

            grdDummy.Rows.Clear();
            grdDummy.ColumnCount = 0;
            grdDummy.RowCount = 0;

            grdList.Rows.Clear();
            grdList.ColumnCount = 0;
            grdList.RowCount = 0;

            grdInventory.Rows.Clear();
            grdInventory.ColumnCount = 0;
            grdInventory.RowCount = 0;

            lvwDetail.Items.Clear();
            lvwCount.Items.Clear();            
        }
        
        private void ListDetails(int iRow)
        {
            int inLineNo = 0;
            int inGridColCount = grdList.ColumnCount;

            lvwDetail.Items.Clear();
            for (int i = 0; i < inGridColCount; i++)
            {
                inLineNo++;
                string cellParam = grdList.Columns[i].HeaderText; // Param
                string cellValue = grdList.Rows[iRow].Cells[i].Value.ToString(); // Value
                ListViewItem item = new ListViewItem(inLineNo.ToString());
                item.SubItems.Add(cellParam);
                item.SubItems.Add(cellValue);
                lvwDetail.Items.Add(item);

                int iLen = 50;
                Debug.WriteLine(dbFunction.padRightChar(cellParam, " ", iLen) + " -->> " + cellValue);
            }
        }
        
        private void frmTerminalImportExport_Load(object sender, EventArgs e)
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

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);

            ClearDataGrid();
           
            InitComboBox();

            SetInitMode(iInitMode.iDisable);

            btnImportSave.Enabled = false;

            btnValidate.Enabled = false;

            btnGenerate.Enabled = false;
            btnSaveAutoGen.Enabled = false;
            
            fEdit = false;
            fEnableScan = false;
            InitDate();

            InitTab();
            InitTimerSerialNo();
            PKTextBoxBackColor(true);
            
            UpdateButton(true, 4);

            btnRelease.Enabled = false;

            btnGenerate.Enabled = btnReset.Enabled = false;

            // Load Mapping
            dbAPI.ExecuteAPI("GET", "View", "Type", "TERMINAL", "Mapping", "", "ViewMapping");

            chkRelease_CheckedChanged(this, e);
            
            btnClear_Click(this, e);

            dbFunction.initTabSelection(tabTerminal, 2);

            if (fAutoLoadData)
            {
                btnSearchTerminalSN_Click(this, e);
                fAutoLoadData = false;
            }

            ucStatusDisplay.SetStatus("", Enums.StatusType.Init);
            
            Cursor.Current = Cursors.Default;
        }

        private void btnImportCancel_Click(object sender, EventArgs e)
        {
            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);

            ClearDataGrid();
            ClearListView();
            
            fEdit = false;
            UpdateButton(true, tabTerminal.SelectedIndex);

            ucStatusDisplay.SetStatus("", Enums.StatusType.Init);
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

        private void btnImportSave_Click(object sender, EventArgs e)
        {
            if (!dbAPI.isValidSystemVersion()) return;

            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isAdd, clsUser.ClassUserID, 31)) return;

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
                SaveTerminalMaster();

                SaveTerminalImportDetail();

                // ------------------------------------------------------------------------------------------
                // Upload physical attach file
                // ------------------------------------------------------------------------------------------                                

                ucStatusDisplay.SetStatus($"Uploading physial file of [{txtFileName.Text}]", Enums.StatusType.Upload);
                Task.Delay(delay); // Asynchronously wait without blocking UI

                string pLocalPath = $"{txtPathFileName.Text.Replace(txtFileName.Text, "")}";
                string pRemotePath = $"{clsGlobalVariables.strFTPRemoteSerialPath}{clsGlobalVariables.strAPIBank}{clsFunction.sBackSlash}";
                string pFileName = $"{txtFileName.Text}";

                Debug.WriteLine("Uploading import terminal...");
                Debug.WriteLine($"pLocalPath=[{pLocalPath}]");
                Debug.WriteLine($"pRemotePath=[{pRemotePath}]");
                Debug.WriteLine($"pFileName=[{pFileName}]");

                ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                ftpClient.delete(pRemotePath + pFileName);
                ftpClient.upload(pRemotePath + pFileName, pLocalPath + pFileName);
                ftpClient.disconnect();

                Debug.WriteLine("Uploading import terminal...complete");
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox("Message " + ex.Message, "Terminal import failed", clsFunction.IconType.iError);
            }
                  
            Cursor.Current = Cursors.Default; // Back to normal 
            
            dbFunction.SetMessageBox("Import Terminal successfully saved.", "Saved", clsFunction.IconType.iInformation);

            // Show total
            if (dbFunction.isValidID(txtTIID.Text))
            {
                int pTSuccess = clsSearch.ClassSuccessCount = 0;
                int pTDuplicate = clsSearch.ClassFailedCount = 0;

                dbAPI.ExecuteAPI("GET", "Search", "Total Terminal Import Success", txtTIID.Text, "Get Info Detail", "", "GetInfoDetail");
                if (dbAPI.isNoRecordFound() == false && clsSearch.ClassOutParamValue.Length > 0)
                {
                    pTSuccess = clsSearch.ClassSuccessCount = int.Parse(dbFunction.CheckAndSetNumericValue(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "Total")));
                }
                dbAPI.ExecuteAPI("GET", "Search", "Total Terminal Import Duplicate", txtTIID.Text, "Get Info Detail", "", "GetInfoDetail");
                if (dbAPI.isNoRecordFound() == false && clsSearch.ClassOutParamValue.Length > 0)
                {
                    pTDuplicate = clsSearch.ClassFailedCount = int.Parse(dbFunction.CheckAndSetNumericValue(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "Total")));
                }
                
                if (MessageBox.Show("Terminal import summary below:" +
                               "\n\n" +
                               " >Total SN import success: " + pTSuccess + "\n" +
                               " >Total SN import duplicate: " + pTDuplicate + "\n\n" +
                               "Do you want to generate report for the import summary?", "Import terminal",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    //dbAPI.ExecuteAPI("POST", "Insert", "", txtTIID.Text, "Import Terminal", "", "InsertSelectCollectionDetail");
                    dbReportFunc.ViewTerminalImportReport();

                }
            }

            btnImportCancel_Click(this, e);
        }

        private void SaveTerminalMaster()
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

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "Terminal Import Master", sSQL, "InsertCollectionMaster");

            txtTIID.Text = txtTerminalID.Text = clsLastID.ClassLastInsertedID.ToString();

        }
        private void SaveTerminalImportDetail()
        {
            string sTempSQL = "";
            string sRowSQL = "";
            string sSQL = "";
            string sRowCSV = "";
            //string sCSV = "";
            int iRowCount = grdList.RowCount;
            int iColCount = grdList.ColumnCount;  // exclude Result column           
            int ii = 0;

            int i = 0;
            List<string> TempArrayDataCol = new List<String>();
            int iRecordMinLimit = clsSystemSetting.ClassSystemRecordMinLimit;
            int iStartIndex = 0;
            int iEndIndex = 0;
            int iFileNameIndex = 0;
            int x;
            StringBuilder columnbind = new StringBuilder();

            GetMTextBoxID();

            //dbAPI.GetClientList("View", "Client List", "", "Particular"); // Load Client

            int iSNColIndex = GetHeaderColumnIndex(grdList, clsDefines.HDR_TEMPLATE_SerialNo);
            int iTypeColIndex = GetHeaderColumnIndex(grdList, clsDefines.HDR_TEMPLATE_Type);
            int iModelColIndex = GetHeaderColumnIndex(grdList, clsDefines.HDR_TEMPLATE_Model);
            int iBrandColIndex = GetHeaderColumnIndex(grdList, clsDefines.HDR_TEMPLATE_Brand);
            int iStatusColIndex = GetHeaderColumnIndex(grdList, clsDefines.HDR_TEMPLATE_Status);
            int iClientColIndex = GetHeaderColumnIndex(grdList, clsDefines.HDR_TEMPLATE_Allocation);
            int iLocationColIndex = GetHeaderColumnIndex(grdList, clsDefines.HDR_TEMPLATE_Location);

            if (iRowCount > 0)
            {             
                // Delete File            
                //string sFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iSerialNo, 0);
                //dbFile.DeleteCSV(sFileName);

                for (i = 0; i < iRowCount; i++)
                {
                    ii++;
                    sRowSQL = "";
                    sSQL = "";
                    sTempSQL = "('" + dbFunction.CheckAndSetNumericValue(txtClientID.Text) + "'," + "'" + dbFunction.CheckAndSetNumericValue(txtTIID.Text) + "',";
                    
                    string pSN = grdList.Rows[i].Cells[1].Value.ToString(); // Check is not blank

                    for (x = 0; x < iColCount; x++)
                    {                        
                        string sCellValue = grdList.Rows[i].Cells[x].Value.ToString().Trim();

                        // ROCKY - TERMINAL IMPORT: ADD CLEAN UP FOR TERMINAL IMPORTS
                        sCellValue = (sCellValue.Length > 0 ? clsFunction.FormatCharAndDate(sCellValue).ToUpper() : clsFunction.sDash);

                        if (sCellValue.Length > 0)
                        {
                            if (iSNColIndex == x)
                            {                               
                                string sSN = "'" + (sCellValue.Length > 0 ? sCellValue : clsFunction.sDash) + "'";
                                sCellValue = sSN + "," + sSN;
                            }

                            if (iTypeColIndex == x)
                            {
                                clsSearch.ClassOutFileID = 0;
                                dbFunction.GetIDFromFile("Terminal Type", sCellValue);
                                int iTerminalType = clsSearch.ClassOutFileID;
                                string sType = "'" + (sCellValue.Length > 0 ? sCellValue : clsFunction.sDash) + "'";
                                sCellValue = sType + "," + iTerminalType.ToString();                                
                            }

                            if (iModelColIndex == x)
                            {
                                clsSearch.ClassOutFileID = 0;
                                dbFunction.GetIDFromFile("Terminal Model", sCellValue);
                                int iTerminalModel = clsSearch.ClassOutFileID;
                                string sModel = "'" + (sCellValue.Length > 0 ? sCellValue : clsFunction.sDash) + "'";
                                sCellValue = sModel + "," + iTerminalModel.ToString();                                
                            }

                            if (iBrandColIndex == x)
                            {
                                clsSearch.ClassOutFileID = 0;
                                dbFunction.GetIDFromFile("Terminal Brand", sCellValue);
                                int iTerminalBrand = clsSearch.ClassOutFileID;
                                string sBrand = "'" + (sCellValue.Length > 0 ? sCellValue : clsFunction.sDash) + "'";
                                sCellValue = sBrand + "," + iTerminalBrand.ToString();                                
                            }

                            if (iStatusColIndex == x)
                            {
                                clsSearch.ClassOutFileID = 0;
                                dbFunction.GetIDFromFile("Status List", sCellValue);
                                int iTerminalStatus = clsSearch.ClassOutFileID;
                                string sStatus = "'" + (sCellValue.Length > 0 ? sCellValue : clsFunction.sDash) + "'";
                                sCellValue = sStatus + "," + iTerminalStatus.ToString();                                
                            }

                            if (iLocationColIndex == x)
                            {
                                clsSearch.ClassOutFileID = 0;
                                dbFunction.GetIDFromFile("Location", sCellValue);
                                int iTerminalLocation = clsSearch.ClassOutFileID;
                                string sLocation = "'" + (sCellValue.Length > 0 ? sCellValue : clsFunction.sDash) + "'";
                                sCellValue = sLocation + "," + iTerminalLocation.ToString();
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
                            string sNewFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iSerialNo, iFileNameIndex);
                            Debug.WriteLine("Terminal->sNewFileName="+ sNewFileName);
                            
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
                    string sNewFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iSerialNo, iFileNameIndex);
                    Debug.WriteLine("Terminal->sNewFileName=" + sNewFileName);
                    
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
                        string sImportFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iSerialNo, i);
                        
                        ucStatusDisplay.SetStatus($"Uploading CSV File [{sImportFileName}]", Enums.StatusType.Create);
                        Task.Delay(delay); // Asynchronously wait without blocking UI

                        // Upload File to FTP                                
                        Debug.WriteLine("=>>UploadFile=" + sImportFileName);
                        ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                        ftpClient.delete(clsGlobalVariables.strFTPUploadPath + sImportFileName);
                        ftpClient.upload(clsGlobalVariables.strFTPUploadPath + sImportFileName, @clsGlobalVariables.strFTPLocalPath + sImportFileName);
                        ftpClient.disconnect(); // ftp disconnect

                        Debug.WriteLine("=>>API Call ImportTerminalDetail=" + sImportFileName);
                        dbAPI.ExecuteAPI("POST", "Import", "CSV", sImportFileName, "Import Terminal Detail", "", "ImportTerminalDetail"); // Process CSV File

                    }
                }
                
                dbFunction.GetResponseTime("Import Terminal Detail");

                Cursor.Current = Cursors.Default; // Back to normal             
            }

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
            
            sCurrentDateTime = CurrentDateTime.ToString("yyyy-MM-dd H:mm:ss");

            dbFunction.GetRequestTime("AutoGen Import Terminal Detail");

            dbFunction.GetIDFromFile("Client List", cboAClient.Text);
            clsSearch.ClassClientID = clsSearch.ClassOutFileID;

            dbFunction.GetIDFromFile("Location", cboALocation.Text);
            clsSearch.ClassLocationID = clsSearch.ClassOutFileID;

            dbFunction.GetIDFromFile("Terminal Type", cboAType.Text);
            clsSearch.ClassTerminalTypeID = clsSearch.ClassOutFileID;

            dbFunction.GetIDFromFile("Terminal Model", cboAModel.Text);
            clsSearch.ClassTerminalModelID = clsSearch.ClassOutFileID;

            dbFunction.GetIDFromFile("Terminal Brand", cboABrand.Text);
            clsSearch.ClassTerminalBrandID = clsSearch.ClassOutFileID;

            dbFunction.GetIDFromFile("Status List", cboAStatus.Text);
            clsSearch.ClassTerminalStatusID = clsSearch.ClassOutFileID;

            dbFunction.GetIDFromFile("FE List", cboAAllocation.Text);
            clsSearch.ClassParticularID = clsSearch.ClassOutFileID;

            if (iRowCount > 0)
            {
                // Delete File            
                string sFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iSerialNo, 0);
                dbFile.DeleteCSV(sFileName);
                
                foreach (ListViewItem i in lvwGenerateList.Items)
                {
                    ii++;
                    string sTIID = txtTIID.Text;
                    string sLineNo = i.SubItems[2].Text;
                    string sSerialNo = i.SubItems[3].Text;
                    string sPartNo = i.SubItems[4].Text;
                    string sTermminalTypeDesc = i.SubItems[5].Text;
                    string sTermminalModelDesc = i.SubItems[6].Text;
                    string sTermminalBrandDesc = i.SubItems[7].Text;
                    string sPONo = i.SubItems[8].Text;
                    string sInvNo = i.SubItems[9].Text;
                    string sDeliveryDate = i.SubItems[10].Text;
                    string sReceiveDate = i.SubItems[11].Text;
                    string sTypeID = i.SubItems[12].Text;
                    string sModelID = i.SubItems[13].Text;
                    string sBrandID = i.SubItems[14].Text;
                    string sStatus = i.SubItems[15].Text;
                    string sStatusID = i.SubItems[16].Text;

                    if (sSerialNo.Length > 0 && sTypeID.Length > 0 && sModelID.Length > 0)
                    {
                        sSQL = "";
                        sRowSQL = "";
                        sRowSQL = "('" + dbFunction.CheckAndSetNumericValue(clsSearch.ClassClientID.ToString()) + "'," +
                        sRowSQL + sRowSQL + "'" + sTIID + "'," +
                        sRowSQL + sRowSQL + "'" + ii + "'," +
                        sRowSQL + sRowSQL + "'" + sSerialNo + "'," +
                        sRowSQL + sRowSQL + "'" + sSerialNo + "'," +
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(txtPartNo.Text) + "'," +
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(cboAType.Text) + "'," +
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(clsSearch.ClassTerminalTypeID.ToString()) + "'," +
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(cboAModel.Text) + "'," +
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(clsSearch.ClassTerminalModelID.ToString()) + "'," +
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(cboABrand.Text) + "'," +
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(clsSearch.ClassTerminalBrandID.ToString()) + "'," +
                        sRowSQL + sRowSQL + "'" + sDeliveryDate + "'," +
                        sRowSQL + sRowSQL + "'" + sReceiveDate + "'," +                       
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(dbFunction.CheckDefaultSelectValue(cboALocation.Text)) + "'," +
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtLocationIDFrom.Text) + "'," +
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(txtPONo.Text) + "'," +
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(txtInvoNo.Text) + "'," +                                               
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(cboAStatus.Text) + "'," +
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(clsSearch.ClassTerminalStatusID.ToString()) + "'," +
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(dbFunction.CheckDefaultSelectValue(cboAAllocation.Text)) + "'," +
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(dbFunction.CheckDefaultSelectValue(cboAAssetType.Text)) + "'," +                       
                        sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(txtARemarks.Text) + "')"; 

                        if (sSQL.Length > 0)
                            sSQL = sSQL + "," + sRowSQL;
                        else
                            sSQL = sSQL + sRowSQL;
                        
                        sRowCSV = "";
                        sRowCSV = sSQL.Replace("(", "");
                        sRowCSV = sRowCSV.Replace(")", "");
                        sRowCSV = sRowCSV.Replace("'", "");
                        //sCSV = sCSV + sRowCSV + "\n";

                        Debug.WriteLine("ii="+ii.ToString()+"-"+ "sRowCSV=" + sRowCSV);

                        if (sRowCSV.Length > 0)
                            dbFile.WriteCSV(sFileName, sRowCSV);                     

                    }
                    
                }
                
                // Upload File to FTP                    
                ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                ftpClient.delete(clsGlobalVariables.strFTPUploadPath + sFileName);
                ftpClient.upload(clsGlobalVariables.strFTPUploadPath + sFileName, @clsGlobalVariables.strFTPLocalPath + sFileName);
                ftpClient.disconnect(); // ftp disconnect

                // Process CSV File
                dbAPI.ExecuteAPI("POST", "Import", "CSV", sFileName, "Import Terminal Detail", "", "ImportTerminalDetail");
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

            string sTIID = "";
            string sLineNo = "0";
            string sSerialNo = "";
            string sPartNo = "";
            string sTermminalTypeDesc = "";
            string sTermminalModelDesc = "";
            string sTermminalBrandDesc = "";
            string sPONo = "";
            string sInvNo = "";
            string sDeliveryDate = "";
            string sReceiveDate = "";
            string sTypeID = "";
            string sModelID = "";
            string sBrandID = "";
            string sStatus = "";
            string sStatusID = "";
            string sClientName = "";
            string sClientID = "";

            DateTime stDRDate = dteMDeliveryDate.Value;
            DateTime stRRDate = dteMReceiveDate.Value;

            sCurrentDateTime = CurrentDateTime.ToString("yyyy-MM-dd H:mm:ss");

            dbFunction.GetRequestTime("Manual Terminal Detail");

            if (!fEdit)
            {
                if (!fEnableScan)
                {
                    sTypeID = clsSearch.ClassTerminalTypeID.ToString();
                    sModelID = clsSearch.ClassTerminalModelID.ToString();
                    sBrandID = clsSearch.ClassTerminalBrandID.ToString();
                    sStatusID = clsSearch.ClassTerminalStatusID.ToString();

                    sSQL = "";
                    sRowSQL = "";
                    sRowSQL = "('" + (txtTIID.Text.Length > 0 ? txtTIID.Text : "0") + "'," +
                    sRowSQL + sRowSQL + "'" + sLineNo + "'," +
                    sRowSQL + sRowSQL + "'" + StrClean(dbFunction.CheckAndSetStringValue(txtTerminalSN.Text)) + "'," +
                    sRowSQL + sRowSQL + "'" + StrClean(dbFunction.CheckAndSetStringValue(txtTerminalSN.Text)) + "'," +
                    sRowSQL + sRowSQL + "'" + cboMType.Text + "'," +
                    sRowSQL + sRowSQL + "'" + cboMModel.Text + "'," +
                    sRowSQL + sRowSQL + "'" + cboMBrand.Text + "'," +
                    sRowSQL + sRowSQL + "'" + stDRDate.ToString("yyyy-MM-dd") + "'," +
                    sRowSQL + sRowSQL + "'" + stRRDate.ToString("yyyy-MM-dd") + "'," +
                    sRowSQL + sRowSQL + "'" + sTypeID + "'," +
                    sRowSQL + sRowSQL + "'" + sModelID + "'," +
                    sRowSQL + sRowSQL + "'" + sBrandID + "'," +
                    sRowSQL + sRowSQL + "'" + (txtMPartNo.Text.Length > 0 ? txtMPartNo.Text : "0") + "'," +                                         
                    sRowSQL + sRowSQL + "'" + (txtMPONo.Text.Length > 0 ? txtMPONo.Text : "0") + "'," +
                    sRowSQL + sRowSQL + "'" + (txtMInvNo.Text.Length > 0 ? txtMInvNo.Text : "0") + "'," +
                    sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtLocationIDFrom.Text) + "'," +
                    sRowSQL + sRowSQL + "'" + cboMLocation.Text + "'," +
                    sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtParticularID.Text) + "'," +
                    sRowSQL + sRowSQL + "'" + cboMAllocation.Text + "'," +
                    sRowSQL + sRowSQL + "'" + cboMAssetType.Text + "'," +
                    sRowSQL + sRowSQL + "'" + txtMRemarks.Text + "'," +
                    sRowSQL + sRowSQL + "'" + (sClientName.Length > 0 ? sClientName : "0") + "'," +
                    sRowSQL + sRowSQL + "'" + (sClientID.Length > 0 ? sClientID : "0") + "'," +
                    sRowSQL + sRowSQL + "'" + cboMStatus.Text + "'," +
                    sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtTerminalStatus.Text) + "')";

                    sSQL = sSQL + sRowSQL;

                    dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 1);

                    dbAPI.ExecuteAPI("POST", "Insert", "", "", "Terminal Import Detail", sSQL, "InsertCollectionDetail");
                }
                else
                {
                    if (iRowCount > 0)
                    {
                        // Delete File            
                        string sFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iSerialNo, 0);
                        dbFile.DeleteCSV(sFileName);

                        foreach (ListViewItem i in lvwHistoryList.Items)
                        {
                            ii++;
                            sTIID = i.SubItems[1].Text;
                            sLineNo = i.SubItems[2].Text;
                            sSerialNo = i.SubItems[3].Text;
                            sPartNo = i.SubItems[4].Text;
                            sTermminalTypeDesc = i.SubItems[5].Text;
                            sTermminalModelDesc = i.SubItems[6].Text;
                            sTermminalBrandDesc = i.SubItems[7].Text;
                            sPONo = i.SubItems[8].Text;
                            sInvNo = i.SubItems[9].Text;
                            sDeliveryDate = i.SubItems[10].Text;
                            sReceiveDate = i.SubItems[11].Text;
                            sTypeID = i.SubItems[12].Text;
                            sModelID = i.SubItems[13].Text;
                            sBrandID = i.SubItems[14].Text;
                            sStatus = i.SubItems[15].Text;
                            sStatusID = i.SubItems[16].Text;

                            if (sSerialNo.Length > 0 && sTypeID.Length > 0 && sModelID.Length > 0 && sBrandID.Length > 0)
                            {
                                sSQL = "";
                                sRowSQL = "";
                                sRowSQL = "('" + sTIID + "'," +
                                sRowSQL + sRowSQL + "'" + sLineNo + "'," +
                                sRowSQL + sRowSQL + "'" + sSerialNo + "'," +
                                sRowSQL + sRowSQL + "'" + sSerialNo + "'," +
                                sRowSQL + sRowSQL + "'" + sPartNo + "'," +
                                sRowSQL + sRowSQL + "'" + sTermminalTypeDesc + "'," +
                                sRowSQL + sRowSQL + "'" + sTypeID + "'," +
                                sRowSQL + sRowSQL + "'" + sTermminalModelDesc + "'," +
                                sRowSQL + sRowSQL + "'" + sModelID + "'," +
                                sRowSQL + sRowSQL + "'" + sTermminalBrandDesc + "'," +
                                sRowSQL + sRowSQL + "'" + sBrandID + "'," +
                                sRowSQL + sRowSQL + "'" + sDeliveryDate + "'," +
                                sRowSQL + sRowSQL + "'" + sReceiveDate + "'," +
                                sRowSQL + sRowSQL + "'" + sPONo + "'," +
                                sRowSQL + sRowSQL + "'" + sInvNo + "'," +
                                sRowSQL + sRowSQL + "'" + sStatus + "'," +
                                sRowSQL + sRowSQL + "'" + sStatusID + "')";

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

                        // Upload File to FTP                    
                        ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                        ftpClient.delete(clsGlobalVariables.strFTPUploadPath + sFileName);
                        ftpClient.upload(clsGlobalVariables.strFTPUploadPath + sFileName, @clsGlobalVariables.strFTPLocalPath + sFileName);
                        ftpClient.disconnect(); // ftp disconnect

                        // Process CSV File
                        dbAPI.ExecuteAPI("POST", "Import", "CSV", sFileName, "Import Terminal Detail", "", "ImportTerminalDetail");
                    }
                }
            }            

            dbFunction.GetResponseTime("Manual Terminal Detail");
        }

        private void tabTerminal_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitTab();
        }
        
        private void btnClearGenerateList_Click(object sender, EventArgs e)
        {
            lvwGenerateList.Items.Clear();
        }

        private void btnClearGenerateEntry_Click(object sender, EventArgs e)
        {                   
            dbFunction.ClearTextBox(this);          
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);
            dbFunction.ClearListViewItems(lvwGenerateList);

            fEdit = false;
            UpdateButton(true, 4);

            ClearListView();

            SetInitMode(iInitMode.iDisable);
            
            btnGenerate.Enabled = false;
            
            
            btnGenerate.Enabled = btnReset.Enabled = false;
            
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {            
            if (!ValidateFields()) return;

            // Check Date Range            
            if (!CheckDateFromTo(dteADeliveryDate, dteAReceivedDate)) return;

            if (fEndLimitExceed()) return;

            if (!fContinueConfirm()) return;
            
            // Waiting / Hour Glass
            Cursor.Current = Cursors.WaitCursor;
            
            GenerateSN();

            //btnGenerate.Enabled = btnReset.Enabled = false;
            
            // Back to normal 
            Cursor.Current = Cursors.Default;
        }

        private bool ValidateFields()
        {
            Debug.WriteLine("--ValidateFields--");
            Debug.WriteLine("tabTerminal.SelectedIndex="+ tabTerminal.SelectedIndex);

            switch (tabTerminal.SelectedIndex)
            {
                case 0: // import
                    break;
                case 1: // auto gen
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iLocation, txtLocationIDFrom.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iAssetType, cboAAssetType.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalType, txtTerminalTypeID.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalModel, txtTerminalModelID.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalBrand, txtTerminalBrandID.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalStatus, txtTerminalStatus.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iPrefix, txtPrefix.Text)) return false;
                    //if (!dbFunction.isValidEntry(clsFunction.CheckType.iPadLen, txtPadLength.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iStartIndex, txtIndexStart.Text)) return false;
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iEndIndex, txtIndexEnd.Text)) return false;
                    break;
                case 2: // manual entry
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalStatus, txtTerminalStatus.Text)) return false;

                    if (lvwManual.Items.Count > 0)
                    {
                        if (!dbFunction.isValidListViewChecked(lvwManual)) return false;
                    }
                    else
                    {
                        if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalSN, txtTerminalSN.Text)) return false;
                        if (!dbFunction.isValidEntry(clsFunction.CheckType.iAssetType, cboMAssetType.Text)) return false;
                        if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalType, txtTerminalTypeID.Text)) return false;
                        if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalModel, txtTerminalModelID.Text)) return false;
                        if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalBrand, txtTerminalBrandID.Text)) return false;
                    }
                        
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iLocation, txtLocationIDFrom.Text)) return false;                   
                    if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalStatus, cboMStatus.Text)) return false;

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

        //private bool MValidateFields()
        //{
        //    bool fValid = false;

        //    if (cboMType.Text.Length > 0 && cboMBrand.Text.Length > 0 && cboMBrand.Text.Length > 0 && cboMStatus.Text.Length > 0 && txtTerminalSN.Text.Length > 0)
        //        fValid = true;

        //    if (!fValid)
        //    {
        //        MessageBox.Show("Check the following field(s) listed below:\n\n" +
        //                        "*Serial No\n" +
        //                        "*Type\n" +
        //                        "*Model\n" +
        //                        "*Brand\n" +
        //                        "*Status\n" +                                
        //                        "\n" +
        //                        "Field(s) with asterisk(*) must not be blank.", "Incomplete Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    }

        //    return fValid;
        //}

        private void GenerateSN()
        {
            int iStart = 0;
            int iEnd = 0;
            int iPadLength = 0;
            int inLineNo = 0;
            string sSerialNo = "";
            string sPad = "";
            int ii = 0;
            int iStatusID = dbAPI.GetTerminalStatusFromList(cboAStatus.Text);
            int iTypeID = dbAPI.GetTerminalTypeFromList(cboAType.Text);
            int iModelID = dbAPI.GetTerminalModelFromList(cboAModel.Text);
            int iBrandID = dbAPI.GetTerminalBrandFromList(cboABrand.Text);

            Cursor.Current = Cursors.WaitCursor;

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

            txtTerminalStatus.Text = (iStatusID > 0 ? iStatusID : 0).ToString();
            txtTerminalTypeID.Text = (iTypeID > 0 ? iTypeID : 0).ToString();
            txtTerminalModelID.Text = (iModelID > 0 ? iModelID : 0).ToString();
            txtTerminalBrandID.Text = (iBrandID > 0 ? iBrandID : 0).ToString();

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
                bool fExist = dbAPI.CheckTerminalDetail("Search", "Terminal Detail Check", sSerialNo);
                if (fExist)
                    item.SubItems[0].ForeColor = Color.Red;
                
                item.SubItems.Add(txtTIID.Text);
                item.SubItems.Add(inLineNo.ToString());                
                
                item.SubItems.Add(sSerialNo);
                item.SubItems.Add(txtPartNo.Text);
                item.SubItems.Add(cboAType.Text);
                item.SubItems.Add(cboAModel.Text);
                item.SubItems.Add(cboABrand.Text);
                item.SubItems.Add(txtPONo.Text);
                item.SubItems.Add(txtInvoNo.Text);
                item.SubItems.Add(pDeliveryDate);
                item.SubItems.Add(pReceivedDate);
                
                item.SubItems.Add(txtTerminalTypeID.Text);
                item.SubItems.Add(txtTerminalModelID.Text);
                item.SubItems.Add(txtTerminalBrandID.Text);
                item.SubItems.Add(cboAStatus.Text);
                item.SubItems.Add(txtTerminalStatus.Text);

                lvwGenerateList.Items.Add(item);

                
            }

            dbFunction.ListViewAlternateBackColor(lvwGenerateList);          

            btnSaveAutoGen.Enabled = true;

            Cursor.Current = Cursors.Default;
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
        
        private string GetColumnValue(int iRowIndex, int iColIndex, DataGridView obj)
        {
            string sColumnValue = dbFunction.RemoveUnwantedChar(obj.Rows[iRowIndex].Cells[iColIndex].Value.ToString());

            return sColumnValue;
        }
        private void DataGridViewBackColor(int iRowIndex)
        {
            grdList.Rows[iRowIndex].DefaultCellStyle.BackColor = Color.PeachPuff;
        }
        
        private bool isValidTerminalList()
        {
            int i = 0;
            bool fValid = true;

            for (i = 0; i < grdList.Rows.Count; i++)
            {
                if (grdList.Rows[i].DefaultCellStyle.BackColor == Color.PeachPuff)
                {
                    fValid = false;
                    break;
                }
            }

            return fValid;
        }

        private void btnSaveGenerate_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isUpdate, clsUser.ClassUserID, 31)) return;

            if (!ValidateFields()) return;

            if (!dbFunction.fSavingConfirm(false)) return;

            // Waiting / Hour Glass
            Cursor.Current = Cursors.WaitCursor;
            
            SaveTerminalMaster();
            
            SaveGenerateSNDetail();
            
            // Back to normal 
            Cursor.Current = Cursors.Default;
            
            dbFunction.SetMessageBox("Auto Generated Terminal successfully saved.", "Saved", clsFunction.IconType.iInformation);

            btnImportCancel_Click(this, e);
            btnClearGenerateEntry_Click(this, e);
        }
        
        private void ClearListView()
        {
            lvwGenerateList.Items.Clear();
            lvwCount.Items.Clear();
            lvwDetail.Items.Clear();
            lvwRelease.Items.Clear();
        }
        
        private void btnAddGenerate_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isAdd, clsUser.ClassUserID, 31)) return;

            Cursor.Current = Cursors.WaitCursor;

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(true, this);
            dbFunction.ComBoBoxUnLock(true, this);
            
            SetInitMode(iInitMode.iEnable);

            UpdateButton(false, tabTerminal.SelectedIndex);
            
            btnGenerate.Enabled = true;
            
            txtPrefix.Focus();

            btnGenerate.Enabled = btnReset.Enabled = true;

            Cursor.Current = Cursors.Default;
        }

        private enum iInitMode
        {
            iEnable,
            iDisable
        }
        private void SetInitMode(iInitMode iMode)
        {
            switch (iMode)
            {
                case iInitMode.iEnable:
                    txtPrefix.Enabled = true;
                    txtPadLength.Enabled = true;
                    txtIndexStart.Enabled = true;
                    txtIndexEnd.Enabled = true;

                    txtPrefix.BackColor = Color.White;
                    txtPadLength.BackColor = Color.White;
                    txtIndexStart.BackColor = Color.White;
                    txtIndexEnd.BackColor = Color.White;                    
                    break;
                case iInitMode.iDisable:
                    txtPrefix.Enabled = false;
                    txtPadLength.Enabled = false;
                    txtIndexStart.Enabled = false;
                    txtIndexEnd.Enabled = false;

                    txtPrefix.BackColor = Color.LightGray;
                    txtPadLength.BackColor = Color.LightGray;
                    txtIndexStart.BackColor = Color.LightGray;
                    txtIndexEnd.BackColor = Color.LightGray;
                    break;
            }
        }

        private bool isValidHeader()
        {                       
            int i = 0;            
            string sList = "";
            string sMapFrom = "";
            bool fExist = true;

            dbAPI.ExecuteAPI("GET", "View", "Type", "TERMINAL", "Mapping", "", "ViewMapping");

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
                btnLoadFile.Enabled = true;

                return false;
            }

            return true;            
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

        private void bunifuCards6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (!fResetConfirm()) return;

            dbFunction.ClearTextBox(this);      
        }

        private void btnAddGenerate_Click_1(object sender, EventArgs e)
        {

        }

        private void cboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassTerminalTypeID = 0;
            if (!cboAType.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Terminal Type", cboAType.Text);
                clsSearch.ClassTerminalTypeID = clsSearch.ClassOutFileID;


                dbAPI.FillComboBoxTerminalModelByTerminalType(cboAModel, clsSearch.ClassTerminalTypeID.ToString());
            }

            txtTerminalTypeID.Text = clsSearch.ClassTerminalTypeID.ToString();
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

        private void frmImportTerminal_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
                case Keys.F2:
                    if (tabTerminal.SelectedIndex == 2) // Manual
                    {
                        btnSearchTerminalSN_Click(this, e);
                    }                    
                    break;
            }
        }

        private void lvwGenerateList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtPartNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void InitInventoryHeader()
        {
            int i = 0;
            string sHeaderID = "";
            string sHeader = "";
            string[] arrayHeader = new string[4];
            arrayHeader[0] = "LINE#";
            arrayHeader[1] = "TYPE";
            arrayHeader[2] = "MODEL";
            arrayHeader[3] = "TOTAL";

            dbAPI.GetTerminalStatusList("View", "", "", "Terminal Status");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound()) return;            
            
            int iColCount = clsArray.TerminalStatusID.Length + arrayHeader.Length;
            grdInventory.ColumnCount = iColCount;

            // Set ArrayHeader (Header Name)
            for (i = 0; i < arrayHeader.Length; i++)
            {
                sHeader = arrayHeader[i];

                if (i > 0)
                    grdInventory.Columns[i].Width = iColumnWidth;
                else
                    grdInventory.Columns[i].Width = 40;

                grdInventory.Columns[i].Name = sHeader;
            }

            // Set clsArray.TerminalStatusDescription (Header Name)
            for (i = 0; i < clsArray.TerminalStatusID.Length; i++)
            {
                sHeaderID = clsArray.TerminalStatusID[i].ToString();
                sHeader = clsArray.TerminalStatusDescription[i];

                int iColIndex = arrayHeader.Length + i;
                grdInventory.Columns[iColIndex].Width = iColumnWidth;
                grdInventory.Columns[iColIndex].Name = sHeader;
            }           
        }

        private void LoadInventoryTypeModel()
        {
            int i = 0;
            int x = 0;
            int y = 0;
            int iCount = 0;
            int iTypeDataRowIndex = 1;
            
            dbAPI.GetTerminalTypeList("View", "", "", "Terminal Type");
            dbAPI.GetTerminalModelList("View", "", "", "Terminal Model");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound()) return;

            int iLineNo = GetHeaderColumnIndex(grdInventory, "LINE#");
            int iType = GetHeaderColumnIndex(grdInventory, "TYPE");
            int iModel = GetHeaderColumnIndex(grdInventory, "MODEL");

            int iTotalColIndex = 0;
            int iAvailableColIndex = 0;
            int iAllocatedColIndex = 0;
            int iInstalledColIndex = 0;
            int iDefectiveColIndex = 0;
            int iLoanBorrowColIndex = 0;
            int iForRepairColIndex = 0;
            int iLossColIndex = 0;
            int iDispatchColIndex = 0;
            int iColIndex = 0;
            
            for (i = 0; i < clsArray.TerminalTypeID.Length; i++)
            {
                int iIndex = grdInventory.Rows.Add(); // Add Row

                string sTypeID = clsArray.TerminalTypeID[i];
                string sType = clsArray.TypeDescription[i];

                for (x = 0; x < grdInventory.ColumnCount; x++)
                {
                    if (x == iLineNo)
                        grdInventory.Rows[iIndex].Cells[iLineNo].Value = (i + 1).ToString();

                    if (x == iType)
                    {
                        //grdInventory.Rows[iIndex].Cells[iTypeDataRowIndex].Value = sType + "[" + sTypeID + "]";
                        grdInventory.Rows[iIndex].Cells[iTypeDataRowIndex].Value = sType;

                        for (y = 0; y < clsArray.TerminalModelID.Length; y++)
                        {
                            int iModelIndex = grdInventory.Rows.Add(); // Add Rows

                            string sModelID = clsArray.TerminalModelID[y];
                            string sModel = clsArray.ModelDescription[y];

                            //grdInventory.Rows[iModelIndex].Cells[x + 1].Value = sModel + "[" + sModelID + "]";
                            grdInventory.Rows[iModelIndex].Cells[x + 1].Value = sModel;

                            // Loop For Column (Status)
                            for (int z = 0; z < grdInventory.ColumnCount; z++)
                            {
                                iTotalColIndex = GetHeaderColumnIndex(grdInventory, "TOTAL");
                                iAvailableColIndex = GetHeaderColumnIndex(grdInventory, "AVAILABLE");
                                iAllocatedColIndex = GetHeaderColumnIndex(grdInventory, "ALLOCATED");
                                iInstalledColIndex = GetHeaderColumnIndex(grdInventory, "INSTALLED");
                                iDefectiveColIndex = GetHeaderColumnIndex(grdInventory, "DEFECTIVE");
                                iLoanBorrowColIndex = GetHeaderColumnIndex(grdInventory, "LOAN/BORROW");
                                iForRepairColIndex = GetHeaderColumnIndex(grdInventory, "FOR REPAIR");
                                iLossColIndex = GetHeaderColumnIndex(grdInventory, "LOSS");
                                iDispatchColIndex = GetHeaderColumnIndex(grdInventory, "DISPATCH");

                                if (z == iTotalColIndex)
                                {                                    
                                    clsSearch.ClassTerminalTypeID = int.Parse(sTypeID);
                                    clsSearch.ClassTerminalModelID = int.Parse(sModelID);
                                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassTerminalTypeID.ToString() + clsFunction.sPipe + clsSearch.ClassTerminalModelID;
                                    //Debug.WriteLine("LoadInventoryTypeModel[Grand Total]::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);
                                    dbAPI.GetViewCount("Search", "Grand Total", clsSearch.ClassAdvanceSearchValue, "Get Count");

                                    iCount = 0;
                                    if (dbAPI.isNoRecordFound() == false)
                                        iCount = clsTerminal.ClassTerminalCount;
                                    
                                    //Debug.WriteLine("sType="+sType+"|sModel="+sModel+"|iCount="+iCount.ToString());
                                    grdInventory.Rows[iModelIndex].Cells[iTotalColIndex].Value = iCount.ToString();
                                }

                                clsSearch.ClassTerminalTypeID = int.Parse(sTypeID);
                                clsSearch.ClassTerminalModelID = int.Parse(sModelID);

                                if (z == iAvailableColIndex)
                                {
                                    iColIndex = iAvailableColIndex;
                                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassTerminalTypeID.ToString() + clsFunction.sPipe + clsSearch.ClassTerminalModelID + clsFunction.sPipe + clsGlobalVariables.STATUS_AVAILABLE;                                                                       
                                }

                                if (z == iAllocatedColIndex)
                                {
                                    iColIndex = iAllocatedColIndex;
                                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassTerminalTypeID.ToString() + clsFunction.sPipe + clsSearch.ClassTerminalModelID + clsFunction.sPipe + clsGlobalVariables.STATUS_ALLOCATED;                                    
                                }

                                if (z == iInstalledColIndex)
                                {
                                    iColIndex = iInstalledColIndex;
                                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassTerminalTypeID.ToString() + clsFunction.sPipe + clsSearch.ClassTerminalModelID + clsFunction.sPipe + clsGlobalVariables.STATUS_INSTALLED;                                    
                                }

                                if (z == iDefectiveColIndex)
                                {
                                    iColIndex = iDefectiveColIndex;
                                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassTerminalTypeID.ToString() + clsFunction.sPipe + clsSearch.ClassTerminalModelID + clsFunction.sPipe + clsGlobalVariables.STATUS_DAMAGE;                                    
                                }

                                if (z == iLoanBorrowColIndex)
                                {
                                    iColIndex = iLoanBorrowColIndex;
                                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassTerminalTypeID.ToString() + clsFunction.sPipe + clsSearch.ClassTerminalModelID + clsFunction.sPipe + clsGlobalVariables.STATUS_BORROWED;                                    
                                }

                                if (z == iForRepairColIndex)
                                {
                                    iColIndex = iForRepairColIndex;
                                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassTerminalTypeID.ToString() + clsFunction.sPipe + clsSearch.ClassTerminalModelID + clsFunction.sPipe + clsGlobalVariables.STATUS_REPAIR;                                    
                                }

                                if (z == iLossColIndex)
                                {
                                    iColIndex = iLossColIndex;
                                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassTerminalTypeID.ToString() + clsFunction.sPipe + clsSearch.ClassTerminalModelID + clsFunction.sPipe + clsGlobalVariables.STATUS_LOSS;
                                }

                                if (z == iDispatchColIndex)
                                {
                                    iColIndex = iDispatchColIndex;
                                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassTerminalTypeID.ToString() + clsFunction.sPipe + clsSearch.ClassTerminalModelID + clsFunction.sPipe + clsGlobalVariables.STATUS_DISPATCH;
                                }

                                if (iColIndex > 0 && z != iTotalColIndex)
                                {
                                    dbAPI.GetViewCount("Search", "Total", clsSearch.ClassAdvanceSearchValue, "Get Count");

                                    iCount = 0;
                                    if (dbAPI.isNoRecordFound() == false)
                                        iCount = clsTerminal.ClassTerminalCount;

                                    grdInventory.Rows[iModelIndex].Cells[iColIndex].Value = iCount.ToString();
                                }                                
                            }
                            
                        }
                    }                    
                }                
            }

            //dbFunction.DataGridViewAlternateBackColor(grdInventory);

            grdInventory.ResumeLayout(); // Resume UI updates
        }

        private void LoadInventoryTotal()
        {            
            int iRowIndex = 0;
            int iColIndex = 0;
            
            // Add Row for Grand Total
            int iRowCount = grdInventory.RowCount;
            int iIndex = grdInventory.Rows.Add(); // Add Row

            for (int z = 0; z < grdInventory.ColumnCount; z++)
            {
                if (z > 0)
                    grdInventory.Rows[iIndex].Cells[z].Value = "=============";
                else
                    grdInventory.Rows[iIndex].Cells[z].Value = "====";
            }

            iRowIndex = grdInventory.Rows.Add(); // Add Row
            grdInventory.Rows[iRowIndex].Cells[1].Value = "GRAND TOTAL";

            iColIndex = GetHeaderColumnIndex(grdInventory, "TOTAL");
            ComputeColumnTotals(grdInventory, iColIndex, iRowIndex);

            iColIndex = GetHeaderColumnIndex(grdInventory, "AVAILABLE");
            ComputeColumnTotals(grdInventory, iColIndex, iRowIndex);

            iColIndex = GetHeaderColumnIndex(grdInventory, "ALLOCATED");
            ComputeColumnTotals(grdInventory, iColIndex, iRowIndex);

            iColIndex = GetHeaderColumnIndex(grdInventory, "INSTALLED");
            ComputeColumnTotals(grdInventory, iColIndex, iRowIndex);

            iColIndex = GetHeaderColumnIndex(grdInventory, "DEFECTIVE");
            ComputeColumnTotals(grdInventory, iColIndex, iRowIndex);

            iColIndex = GetHeaderColumnIndex(grdInventory, "LOAN/BORROW");
            ComputeColumnTotals(grdInventory, iColIndex, iRowIndex);

            iColIndex = GetHeaderColumnIndex(grdInventory, "FOR REPAIR");
            ComputeColumnTotals(grdInventory, iColIndex, iRowIndex);

            iColIndex = GetHeaderColumnIndex(grdInventory, "LOSS");
            ComputeColumnTotals(grdInventory, iColIndex, iRowIndex);

            iColIndex = GetHeaderColumnIndex(grdInventory, "DISPATCH");
            ComputeColumnTotals(grdInventory, iColIndex, iRowIndex);

        }

        private void ComputeColumnTotals(DataGridView obj, int iColIndex, int iRowIndex)
        {
            int iRowStart = 0;
            string sCellValue = "";
            bool isNumeric = false;
            int iTTotal = 0;
            int iValue = 0;

            for (int x = 0; x < obj.ColumnCount; x++)
            {
                if (x == iColIndex)
                {
                    for (int i = iRowStart; i < grdInventory.RowCount; i++)
                    {
                        if (grdInventory.Rows[i].Cells[x].Value != null)
                        {
                            sCellValue = grdInventory.Rows[i].Cells[x].Value.ToString();
                            isNumeric = int.TryParse(sCellValue, out iValue);
                            iTTotal += iValue;
                        }
                    }

                    grdInventory.Rows[iRowIndex].Cells[iColIndex].Value = iTTotal.ToString();
                }
            }
        }
        
        private void btnCheckInventory_Click(object sender, EventArgs e)
        {
            if (!fContinueConfirm()) return;
            Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
            dbFunction.ClearDataGrid(grdInventory);
            

            InitInventoryHeader();
            LoadInventoryTypeModel();
            LoadInventoryTotal();
            
            Cursor.Current = Cursors.Default; // Back to normal 
        }
        
        private void txtMSerialNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            if (fEnableScan)
            {
                if (txtTerminalSN.Text.Length > SerialNoMinLength)
                    tmrSerialNo.Enabled = true;
                else
                {
                    Counter = 0;
                    tmrSerialNo.Enabled = false;
                }
            }            
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dbFunction.ClearTextBox(this);           
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);
            dbFunction.ClearListViewItems(lvwHistoryList);
            dbFunction.ClearListViewItems(lvwManual);

            ClearListView();

            fEdit = false;
            
            InitDate();
            PKTextBoxBackColor(true);

            UpdateButton(true, 4);
            
            btnSearchTerminalSN.Enabled = true;
            btnRelease.Enabled = false;

            btnMAddItem.Enabled = btnMRemoveItem.Enabled = false;
            btnMApplyChanges.Enabled = false;
            btnSearchTerminalSN.Enabled = true;
            lblSelectedHeader.Text = clsFunction.sNull;
            rtbJSONFormat.Text = clsFunction.sNull;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isAdd, clsUser.ClassUserID, 31)) return;

            Cursor.Current = Cursors.WaitCursor;

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(true, this);
            dbFunction.ComBoBoxUnLock(true, this);

            fEdit = false;

            GetMTextBoxID();
            
            InitDate();
            
            int ID = dbAPI.GetControlID("Terminal Master");
            txtMBatchNo.Text = ID.ToString();
            
            Counter = 0;
            InitTimerSerialNo();
            PKTextBoxBackColor(false);

            btnSearchTerminalSN.Enabled = true;

            fEdit = false;

            UpdateButton(false, tabTerminal.SelectedIndex);

            btnMAddItem.Enabled = btnMRemoveItem.Enabled = true;
            btnMApplyChanges.Enabled = false;

            txtTerminalSN.Enabled = txtMBatchNo.Enabled = txtMPONo.Enabled = txtMPartNo.Enabled = true;

            btnSearchTerminalSN.Enabled = false;

            txtTerminalSN.Focus();

            Cursor.Current = Cursors.Default;
        }

        private void btnSearchTerminalSN_Click(object sender, EventArgs e)
        {
            if (fAutoLoadData)
            {
                frmSearchField.fSelected = true;
            }
            else
            {
                frmSearchField.iSearchType = frmSearchField.SearchType.iTerminal;
                frmSearchField.iStatus = clsFunction.iZero;
                frmSearchField.sTerminalType = "View Terminal";
                frmSearchField.sHeader = "TERMINAL";
                frmSearchField.isCheckBoxes = false;
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

                txtTerminalID.Text = clsSearch.ClassTerminalID.ToString();
                txtTerminalSN.Text = clsSearch.ClassTerminalSN;
                
                PopulateTerminalTextBox(txtTerminalID.Text, txtTerminalSN.Text);

                //if (dbFunction.isValidID(txtTerminalID.Text))
                //{
                //    if (!dbAPI.isDuplicateSN(int.Parse(txtTerminalID.Text), txtTerminalSN.Text, clsDefines.SEARCH_TERMINAL)) return;
                //}

                clsSearch.ClassTerminalTypeID = int.Parse(dbFunction.CheckAndSetNumericValue(txtTerminalTypeID.Text));

                // Load History
                clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtTerminalSN.Text) + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();

                
                PopulateMerchantTextBox();

                PopulateLastControlNoTextBox();

                ViewServiceHistory();

                viewSNMinMaxServiceInfo();

                PKTextBoxBackColor(false);

                UpdateButton(false, tabTerminal.SelectedIndex);

                btnRelease.Enabled = true;

                dteMReleaseDate.Enabled = true;

                dteMReceiveDate.Enabled = true;

                txtTerminalID.ReadOnly = true;

                setSelectedHeader();
                
                Cursor.Current = Cursors.Default;

            }
        }

        private void PopulateTerminalTextBox(string sTerminalID, string sTerminalSN)
        {
                cboMStatus.Text =
                txtTerminalSN.Text =
                txtTerminalTypeID.Text =
                cboMType.Text =
                txtTerminalModelID.Text =
                cboMModel.Text =
                txtTerminalBrandID.Text =
                cboMBrand.Text =
                cboMLocation.Text =
                txtParticularID.Text =
                cboMAllocation.Text =
                cboMAssetType.Text =
                txtMInvNo.Text =
                txtMPONo.Text =
                txtMPartNo.Text =
                txtMRemarks.Text =
                txtClientID.Text =
                clsFunction.sNull;

            if (dbFunction.isValidID(sTerminalID))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Terminal SN Info", sTerminalID, "Get Info Detail", "", "GetInfoDetail");
                
                if (dbAPI.isNoRecordFound() == false)
                {
                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                    txtTerminalID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                    txtTerminalTypeID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtTerminalModelID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                    txtTerminalBrandID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                    txtTerminalStatus.Text = txtHoldStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                    cboMStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5);
                    txtTerminalSN.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                    cboMType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                    cboMModel.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                    cboMBrand.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                    cboMLocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);
                    cboMAllocation.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 14);
                    cboMAssetType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 15);

                    txtMPONo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 16);
                    txtMInvNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 17);
                    
                    txtMPartNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 18);
                    txtMRemarks.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 19);

                    // -----------------------------------------------------------------------------
                    // Date 
                    // -----------------------------------------------------------------------------
                    string pTemp = "";
                    pTemp = dbFunction.GetDateFromParse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 11), clsFunction.sValueDateFormat, clsFunction.sDateDefaultFormat);
                    dteMDeliveryDate.Value = DateTime.Parse(pTemp);

                    pTemp = dbFunction.GetDateFromParse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 12), clsFunction.sValueDateFormat, clsFunction.sDateDefaultFormat);
                    dteMReceiveDate.Value = DateTime.Parse(pTemp);

                    pTemp = dbFunction.GetDateFromParse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 24), clsFunction.sValueDateFormat, clsFunction.sDateDefaultFormat);
                    dteMReleaseDate.Value = DateTime.Parse(pTemp);
                    // -----------------------------------------------------------------------------
                    // Date 
                    // -----------------------------------------------------------------------------

                    txtServiceNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 20);
                    txtIRIDNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 21);
                    
                    txtParticularID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 25);

                    txtClientID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 27);
                    cboMClient.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 28);

                    txtMUpdatedAt.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 31);
                    txtMUpdatedBy.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 32);
                }
            }
        }

        /*
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
                    txtTIID.Text = clsArray.TIID[i].ToString();
                    txtTerminalID.Text = clsArray.TerminalID[i].ToString();
                    txtTerminalTypeID.Text = clsArray.TerminalTypeID[i].ToString();
                    txtTerminalModelID.Text = clsArray.TerminalModelID[i].ToString();
                    txtTerminalBrandID.Text = clsArray.TerminalBrandID[i].ToString();
                    txtTerminalStatus.Text = clsArray.TerminalStatus[i].ToString();
                    txtTerminalSN.Text = clsArray.TerminalSN[i].ToString();
                    txtMPartNo.Text = clsArray.TerminalPartNo[i].ToString();
                    txtMPONo.Text = clsArray.TerminalPONo[i].ToString();
                    txtMInvNo.Text = clsArray.TerminalInvNo[i].ToString();
                    txtMRemarks.Text = clsArray.Remarks[i].ToString();                    

                    i++;
                }

            }
        }
        */


        //private void PopulateManualComboBox()
        //{
        //    if (dbFunction.isValidID(txtTerminalStatus.Text))
        //    {
        //        dbAPI.FillComboBoxTerminalStatus(cboMStatus);
        //        string sStatus = dbAPI.GetStatusDescription(int.Parse(txtTerminalStatus.Text));
        //        cboMStatus.Text = sStatus;
        //    }

        //    if (dbFunction.isValidID(txtTerminalID.Text))
        //    {
        //        dbAPI.GetTerminalInfo(txtTerminalID.Text, txtTerminalSN.Text);
        //        if (dbFunction.isValidID(txtTerminalTypeID.Text))
        //        {
        //            dbAPI.FillComboBoxTerminalType(cboMType);
        //            cboMType.Text = clsTerminal.ClassTerminalType;

        //        }

        //        if (dbFunction.isValidID(txtTerminalModelID.Text))
        //        {
        //            dbAPI.FillComboBoxTerminalModel(cboMModel);
        //            cboMModel.Text = clsTerminal.ClassTerminalModel;
        //        }

        //        if (dbFunction.isValidID(txtTerminalBrandID.Text))
        //        {
        //            dbAPI.FillComboBoxTerminalBrand(cboMBrand);
        //            cboMBrand.Text = clsTerminal.ClassTerminalBrand;
        //        }
        //    }
        //}

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool isProceed = false;

            if (!dbAPI.isValidSystemVersion()) return;

            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isUpdate, clsUser.ClassUserID, 31)) return;

            if (!ValidateFields()) return;

            DateTime stReceiveDate = dteMReceiveDate.Value;           
            string pReceiveDate = stReceiveDate.ToString("yyyy-MM-dd");

            DateTime stDeliveryDate = dteMDeliveryDate.Value;
            string pDeliveryDate = stDeliveryDate.ToString("yyyy-MM-dd");

            DateTime stReleaseDate = dteMReleaseDate.Value;
            string pReleseDate = stReleaseDate.ToString("yyyy-MM-dd");

            int iStatus = int.Parse(dbFunction.CheckAndSetNumericValue(txtTerminalStatus.Text));
            int iHoldStatus = int.Parse(dbFunction.CheckAndSetNumericValue(txtHoldStatus.Text));
            
            if (fEdit)
            {
                if (dbFunction.isValidID(txtTerminalID.Text))
                {
                    if ((txtLastServiceMade.Text.Equals(clsGlobalVariables.STATUS_PULLEDOUT_DESC) && txtLastServiceActionMade.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS)) ||
                        !dbFunction.isValidID(txtTService.Text))
                    {
                        isProceed = true;
                    }

                    if (dbFunction.isValidID(txtTerminalStatus.Text) &&
                        (iHoldStatus.Equals(clsGlobalVariables.STATUS_DAMAGE)) ||
                        (iHoldStatus.Equals(clsGlobalVariables.STATUS_LOSS)) ||
                        (iHoldStatus.Equals(clsGlobalVariables.STATUS_BORROWED)))
                    {
                        isProceed = true;
                    }

                    if (dbFunction.isValidID(txtTerminalStatus.Text) &&
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

                    if (!dbAPI.isRecordExist("Search", "TerminalSN From IRDetail", txtIRIDNo.Text + clsFunction.sPipe + txtTerminalID.Text))
                    {
                        isProceed = true;
                    }

                    if (dbAPI.isRecordExist("Search", "TerminalSN From IRDetail", txtIRIDNo.Text + clsFunction.sPipe + txtTerminalID.Text))
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
                dbFunction.SetMessageBox("Terminal SN " + dbFunction.AddBracketStartEnd(txtTerminalSN.Text) + "\n\n" + "Unable to update.", "Update failed", clsFunction.IconType.iError);
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
                        dbFunction.SetMessageBox("Location of this Terminal SN must be located in\n" + dbFunction.AddBracketStartEnd(clsSystemSetting.ClassSystemSNLocation), clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
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

            if (fEdit)
            {
                if (lvwManual.Items.Count > 0)
                {
                    // updating
                    BulkUpdateTerminalSNDetail(lvwManual, false);

                    // Back to normal 
                    Cursor.Current = Cursors.Default;

                    MessageBox.Show("Manual Multiple Terminal SN has been successfully updated.", "Updated",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);

                }
                else
                {
                    if (!ValidateFields()) return;

                    GetMTextBoxID();

                    clsSearch.ClassAdvanceSearchValue =
                                                    dbFunction.CheckAndSetNumericValue(txtTerminalID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtTIID.Text) + clsFunction.sPipe +
                                                    StrClean(txtTerminalSN.Text) + clsFunction.sPipe +
                                                    cboMType.Text + clsFunction.sPipe +
                                                    cboMModel.Text + clsFunction.sPipe +
                                                    cboMBrand.Text + clsFunction.sPipe +
                                                    pDeliveryDate + clsFunction.sPipe +
                                                    pReceiveDate + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtTerminalTypeID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtTerminalModelID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtTerminalBrandID.Text) + clsFunction.sPipe +
                                                    txtMPartNo.Text + clsFunction.sPipe +
                                                    txtMPONo.Text + clsFunction.sPipe +
                                                    txtMInvNo.Text + clsFunction.sPipe +
                                                    txtMRemarks.Text + clsFunction.sPipe +
                                                    (dbFunction.isValidDescription(cboMClient.Text) ? cboMClient.Text : clsFunction.sZero) + clsFunction.sPipe +
                                                    (dbFunction.CheckAndSetNumericValue(txtClientID.Text)) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtTerminalStatus.Text) + clsFunction.sPipe +
                                                    cboMStatus.Text + clsFunction.sPipe +
                                                    cboMLocation.Text + clsFunction.sPipe +
                                                    cboMAllocation.Text + clsFunction.sPipe +
                                                    cboMAssetType.Text + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtParticularID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtLocationIDFrom.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetBooleanValue(chkActive.Checked) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetNumericValue(txtClientID.Text) + clsFunction.sPipe +
                                                    pReleseDate + clsFunction.sPipe +
                                                    clsSearch.ClassCurrentParticularID + clsFunction.sPipe +
                                                    dbFunction.getCurrentDateTime();

                    Debug.WriteLine("SaveTerminalDetail::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                    dbFunction.parseDelimitedString(clsSearch.ClassAdvanceSearchValue, clsDefines.gPipe, 1);

                    dbAPI.ExecuteAPI("PUT", "Update", "Terminal Detail", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                    // Back to normal 
                    Cursor.Current = Cursors.Default;

                    MessageBox.Show("Manual Terminal has been successfully updated.", "Updated",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);
                }

            }
            else
            {
                SaveTerminalMaster();

                SaveMSNDetail();

                // Back to normal 
                Cursor.Current = Cursors.Default;
                
                dbFunction.SetMessageBox("Manual Terminal successfully saved.", "Saved", clsFunction.IconType.iInformation);

            }

            // Save Activity
            SaveActivity();
        
            dbFunction.ClearTextBox(this);
            dbFunction.ClearDataGrid(grdList);
            dbFunction.ClearDataGrid(grdInventory);
            //dbFunction.ClearListView(lvwGenerateList);
            //dbFunction.ClearListView(lvwHistoryList);
            
            btnClear_Click(this, e);
        }

        private void InitTab()
        {
            lblSelectedHeader.Text = "";
            switch (tabTerminal.SelectedIndex)
            {
                case 0:
                    lblHeader.Text = "TERMINAL" + " " + "[ " + "IMPORT" + " ]";
                    break;
                case 1:
                    lblHeader.Text = "TERMINAL" + " " + "[ " + "AUTO GENERATE SN" + " ]";
                    break;
                case 2:
                    lblHeader.Text = "TERMINAL" + " " + "[ " + "MANUAL ENTRY" + " ]";
                    txtTerminalSN.MaxLength = SerialNoMaxLength;                                     
                    break;
                case 3:
                    lblHeader.Text = "TERMINAL" + " " + "[ " + "RELEASE / TRANSFER" + " ]";
                    break;

            }
        }

        private void tmrSerialNo_Tick(object sender, EventArgs e)
        {
            if (Counter >= ReaderTimeOut) // 1 Seconds
            {
                Counter = 0;
                if (txtTerminalSN.Text.Length > SerialNoMinLength)
                {                    
                    tmrSerialNo.Enabled = false;
                    AddSerialNo(txtTerminalSN.Text);
                    txtTerminalSN.Text = "";
                }
            }
            
            //lblCounter.Text = "Counter: " + Counter.ToString() + " | " + "ReaderInterval: " + ReaderInterval.ToString() + " | " + "ReaderTimeOut: " + ReaderTimeOut.ToString();

            Counter++;
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

            txtTerminalSN.Focus();

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

        private void chkAutoScan_CheckedChanged(object sender, EventArgs e)
        {
            InitTimerSerialNo();
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
                        if (i.SubItems[3].Text == sSerialNo)
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
                    // Get ID
                    string sTypeID = dbAPI.GetTerminalTypeFromList(cboMType.Text).ToString();
                    string sModelID = dbAPI.GetTerminalModelFromList(cboMModel.Text).ToString();
                    string sBrandID = dbAPI.GetTerminalBrandFromList(cboMBrand.Text).ToString();
                    string sStatusID = dbAPI.GetTerminalStatusFromList(cboMStatus.Text).ToString();

                    ListViewItem item = new ListViewItem(LineNo.ToString());
                    item.SubItems.Add(txtMBatchNo.Text);
                    item.SubItems.Add(LineNo.ToString());
                    item.SubItems.Add(sSerialNo);
                    item.SubItems.Add(txtMPartNo.Text.Length > 0 ? txtMPartNo.Text : clsFunction.sDash);
                    item.SubItems.Add(cboMType.Text);
                    item.SubItems.Add(cboMModel.Text);
                    item.SubItems.Add(cboMBrand.Text);
                    item.SubItems.Add(txtMPONo.Text.Length > 0 ? txtMPONo.Text : clsFunction.sDash);
                    item.SubItems.Add(txtMInvNo.Text.Length > 0 ? txtMInvNo.Text : clsFunction.sDash);
                    item.SubItems.Add(dteMDeliveryDate.Text);
                    item.SubItems.Add(dteMReceiveDate.Text);
                    item.SubItems.Add(sTypeID);
                    item.SubItems.Add(sModelID);
                    item.SubItems.Add(sBrandID);
                    item.SubItems.Add(cboMStatus.Text);
                    item.SubItems.Add(sStatusID);
                    lvwHistoryList.Items.Add(item);
                }

                UpdateMLineNo();
            }

            txtTerminalSN.Text = "";
        }

        private bool ValidateMFields()
        {
            bool fValid = false;

            if ((cboMType.Text.Length > 0) && (cboMModel.Text.Length > 0) && (cboMBrand.Text.Length > 0) && (cboMStatus.Text.Length > 0))
                fValid = true;

            if (!fValid)
            {
                MessageBox.Show("Check the following field(s) listed below:\n\n" +
                                "*Type\n" +
                                "*Model\n" +
                                "*Brand\n" +
                                "*Status\n" +
                                "\n" +
                                "Field(s) with asterisk(*) must not be blank.", "Incomplete Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return fValid;
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
                        i.SubItems[2].Text = LineNo.ToString();

                        LineNo++;
                    }
                }
            }
        }

        private void txtMSerialNo_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    txtTerminalSN.Text = "";
                    break;
                case Keys.Enter:
                    if (fEnableScan)
                    {
                        if (!tmrSerialNo.Enabled)
                        {
                            AddSerialNo(txtTerminalSN.Text);
                            txtTerminalSN.Text = "";
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

        }

        private bool CheckTerminalDetail()
        {
            bool fExist = false;
            string sTerminalDetail = txtTerminalSN.Text;

            fExist = dbAPI.CheckTerminalDetail("Search", "Terminal Detail Check", sTerminalDetail);

            if (fExist)
            {
                dbFunction.SetMessageBox("Unable to save Terminal Detail." +
                            "\n\n" +
                            "Serial No: " + txtTerminalSN.Text +                            
                            "\n", "Already exist.", clsFunction.IconType.iWarning);


            }

            return fExist;
        }
        
        private void GetMTextBoxID()
        {
            // Fill List
            
            dbAPI.GetTerminalTypeList("View", "", "", "Terminal Type");
            dbAPI.GetTerminalModelList("View", "", "", "Terminal Model");
            dbAPI.GetTerminalBrandList("View", "", "", "Terminal Brand");

            txtTerminalTypeID.Text = dbAPI.GetTerminalTypeFromList(cboMType.Text).ToString();
            txtTerminalModelID.Text = dbAPI.GetTerminalModelFromList(cboMModel.Text).ToString();
            txtTerminalBrandID.Text = dbAPI.GetTerminalBrandFromList(cboMBrand.Text).ToString();
            
        }

        private void PKTextBoxBackColor(bool isLock)
        {
            if (isLock)
            {
                txtTerminalSN.BackColor = clsFunction.DisableBackColor;
                txtTerminalSN.ReadOnly = true;
            }
            else
            {
                if (fEdit)
                {
                    txtTerminalSN.BackColor = clsFunction.MKBackColor;
                    txtTerminalSN.ReadOnly = true;
                }
                else
                {
                    txtTerminalSN.BackColor = clsFunction.EntryBackColor;
                    txtTerminalSN.ReadOnly = false;
                }
                
            }
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

        private void txtInvoNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void txtPONo_KeyPress(object sender, KeyPressEventArgs e)
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

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void InitDate()
        {
            dteADeliveryDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteADeliveryDate, clsFunction.sDateDefaultFormat);

            dteAReceivedDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteAReceivedDate, clsFunction.sDateDefaultFormat);

            dteAReleaseDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteAReleaseDate, clsFunction.sDateDefaultFormat);

            dteMDeliveryDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteMDeliveryDate, clsFunction.sDateDefaultFormat);

            dteMReceiveDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteMReceiveDate, clsFunction.sDateDefaultFormat);

            dteMReleaseDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteMReleaseDate, clsFunction.sDateDefaultFormat);

            dteMShipmentDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteMShipmentDate, clsFunction.sDateDefaultFormat);
            
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

        private void bunifuCards2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cboAModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassTerminalModelID = 0;
            if (!cboAModel.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Terminal Model", cboAModel.Text);
                clsSearch.ClassTerminalModelID = clsSearch.ClassOutFileID;              
            }

            txtTerminalModelID.Text = clsSearch.ClassTerminalModelID.ToString();
        }

        private void cboABrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassTerminalBrandID = 0;
            if (!cboABrand.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Terminal Brand", cboABrand.Text);
                clsSearch.ClassTerminalBrandID = clsSearch.ClassOutFileID;
            }

            txtTerminalBrandID.Text = clsSearch.ClassTerminalBrandID.ToString();
        }

        private void cboAStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassTerminalStatusID = 0;
            if (!cboAStatus.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Status List", cboAStatus.Text);
                clsSearch.ClassTerminalStatusID = clsSearch.ClassOutFileID;
            }

            txtTerminalStatus.Text = clsSearch.ClassTerminalStatusID.ToString();
        }

        private void cboMType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //clsSearch.ClassTerminalTypeID = 0;
            if (!cboMType.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Terminal Type", cboMType.Text);
                clsSearch.ClassTerminalTypeID = clsSearch.ClassOutFileID;


                dbAPI.FillComboBoxTerminalModelByTerminalType(cboMModel, clsSearch.ClassTerminalTypeID.ToString());
            }

            txtTerminalTypeID.Text = clsSearch.ClassTerminalTypeID.ToString();
        }

        private void cboMModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassTerminalModelID = 0;
            if (!cboMModel.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Terminal Model", cboMModel.Text);
                clsSearch.ClassTerminalModelID = clsSearch.ClassOutFileID;
            }

            txtTerminalModelID.Text = clsSearch.ClassTerminalModelID.ToString();
        }

        private void cboMBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassTerminalBrandID = 0;
            if (!cboMBrand.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Terminal Brand", cboMBrand.Text);
                clsSearch.ClassTerminalBrandID = clsSearch.ClassOutFileID;
            }

            txtTerminalBrandID.Text = clsSearch.ClassTerminalBrandID.ToString();
        }

        private void cboMStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassTerminalStatus = 0;
            if (!cboMStatus.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Status List", cboMStatus.Text);
                clsSearch.ClassTerminalStatus = clsSearch.ClassOutFileID;
            }

            txtTerminalStatus.Text = clsSearch.ClassTerminalStatus.ToString();
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
        
        private void SaveActivity()
        {
            string sRowSQL = "";
            string sSQL = "";

            dbFunction.GetProcessedByAndDateTime(); // Get processedby and datetime

            dbFunction.GetModifiedByAndDateTime(); // Get modifiedby and datetime

            clsSearch.ClassStatus = int.Parse(dbFunction.CheckAndSetNumericValue(txtTerminalStatus.Text));
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
            sRowSQL = " ('" + dbFunction.CheckAndSetNumericValue(txtTerminalID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + txtTerminalSN.Text + "', " +
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

            Debug.WriteLine("sSQL="+ sSQL);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "Terminal Activity", sSQL, "InsertCollectionDetail");
            
        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void bunifuCards7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label26_Click(object sender, EventArgs e)
        {

        }
        
        private void UpdateButton(bool isClear, int index)
        {
            Debug.WriteLine("--UpdateButton--");
            Debug.WriteLine("isClear=" + isClear);
            Debug.WriteLine("index="+ index);
            Debug.WriteLine("fEdit=" + fEdit);

            if (isClear)
            {
                // Import
                btnLoadFile.Enabled = true;
                btnImportSave.Enabled = false;
                btnValidate.Enabled = false;

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
        
        private void btnRelease_Click(object sender, EventArgs e)
        {
            DateTime stReleaseDate = dteMReleaseDate.Value;
            string pReleseDate = stReleaseDate.ToString("yyyy-MM-dd");

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalSN, txtTerminalSN.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iLocation, cboMLocation.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalStatus, cboMStatus.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iAssetType, cboMAssetType.Text)) return;

            if (!cboMStatus.Text.Equals(clsGlobalVariables.STATUS_AVAILABLE_DESC))
            {
                dbFunction.SetMessageBox("Terminal status must be AVAILABLE to release.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iExclamation);
                return;
            }
            
            if (MessageBox.Show("Are you sure to release terminal SN " + txtTerminalSN.Text + "\n\n" + 
                                "to location " + cboMLocation.Text + "?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                return;
            }
            
            Cursor.Current = Cursors.WaitCursor;
            
            dbFunction.GetProcessedByAndDateTime(); // Get modifiedby and datetime

            clsSearch.ClassAdvanceSearchValue =
                                                dbFunction.CheckAndSetNumericValue(txtTerminalID.Text) + clsFunction.sPipe +
                                                cboMLocation.Text + clsFunction.sPipe +
                                                pReleseDate + clsFunction.sPipe +
                                                clsUser.ClassProcessedBy + clsFunction.sPipe +
                                                clsUser.ClassProcessedDateTime + clsFunction.sPipe +
                                                txtMRemarks.Text;

            Debug.WriteLine("ReleaseTerminal::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbAPI.ExecuteAPI("PUT", "Update", "Released Terminal Detail", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

            dbFunction.SetMessageBox("Terminal SN " + txtTerminalSN.Text + " has been successfully released." + "\n\n" +
                                     "Location " + cboMLocation.Text, "Released", clsFunction.IconType.iInformation);

            btnClear_Click(this, e);

            Cursor.Current = Cursors.Default;


        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label41_Click(object sender, EventArgs e)
        {

        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

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

            UpdateButton(false, tabTerminal.SelectedIndex);

            btnRSearch.Enabled = false;
            
            txtTransNo.Text = dbAPI.GetControlID("Terminal Movement Master").ToString();

            // Generate ControlID
            dbAPI.GenerateID(true, txtRRequestNo, txtTransNo, "Terminal Movement Master", clsDefines.CONTROLID_PREFIX_TERMINAL);

            // Generate ReferenceNo
            dbAPI.GenerateID(true, txtRReferenceNo, txtTransNo, "Terminal Movement Master", clsDefines.CONTROLID_PREFIX_REFNO);

            // Date/Time Created
            dbFunction.GetProcessedByAndDateTime(); // Get processedby and datetime
            txtRDate.Text = clsUser.ClassProcessedDate;

            chkRelease_CheckedChanged(this, e);

            dteRReleaseDate.Enabled = true;

            Cursor.Current = Cursors.Default;
        }

        private void InitComboBox()
        {
            Cursor.Current = Cursors.WaitCursor;

            dbFunction.ClearComboBox(this);

            // Auto Gen
            dbAPI.FillComboBoxLocation(cboALocation);
            dbAPI.FillComboBoxFE(cboAAllocation);
            dbAPI.FillComboBoxAssetType(cboAAssetType);
            dbAPI.FillComboBoxTerminalType(cboAType);
            dbAPI.FillComboBoxTerminalBrand(cboABrand);
            dbAPI.FillComboBoxTerminalStatus(cboAStatus);

            // Manual
            dbAPI.FillComboBoxLocation(cboMLocation);
            dbAPI.FillComboBoxFE(cboMAllocation);
            dbAPI.FillComboBoxAssetType(cboMAssetType);
            dbAPI.FillComboBoxTerminalType(cboMType);
            dbAPI.FillComboBoxTerminalBrand(cboMBrand);
            dbAPI.FillComboBoxTerminalStatus(cboMStatus);

            // Release/Transfer
            dbAPI.FillComboBoxLocation(cboRLocationFrom);
            dbAPI.FillComboBoxLocation(cboRLocationTo);
            dbAPI.FillComboBoxTerminalStatus(cboRStatus);

            dbAPI.FillComboBoxClient(cboIClient);
            dbAPI.FillComboBoxClient(cboAClient);
            dbAPI.FillComboBoxClient(cboMClient);

            // Lock
            dbFunction.ComBoBoxUnLock(false, this);

            Cursor.Current = Cursors.Default;
        }

        private void btnRClear_Click(object sender, EventArgs e)
        {         
            dbFunction.ClearTextBox(this);           
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);
            dbFunction.ClearListViewItems(lvwRelease);

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

        private void btnRAddItem_Click(object sender, EventArgs e)
        {
            if (chkRelease.Checked)
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iFromLocation, cboRLocationFrom.Text)) return;

            frmSearchField.iSearchType = frmSearchField.SearchType.iTerminal;
            frmSearchField.iStatus = clsGlobalVariables.STATUS_AVAILABLE;
            frmSearchField.sTerminalType = "View Terminal";
            frmSearchField.sHeader = "RELEASE - TERMINAL";
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
                    dbFunction.SetMessageBox("Error message " + ex.Message + "\n\n" + clsDefines.CONTACT_ADMIN_MESSAGE, "Release Terminal", clsFunction.IconType.iError);
                }
                
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
                if (chkRelease.Checked)
                {
                    SaveMovementMaster();
                    SaveMovementDetail();                    
                }
                else
                {
                    // Update here                
                }               
            }

            if (lvwRelease.Items.Count > 0)
            {
                BulkUpdateTerminalSNDetail(lvwRelease, true);
            }
            
            Cursor.Current = Cursors.Default;

            if (fEdit)
                dbFunction.SetMessageBox((chkRelease.Checked ? "Release/transfer" : "Terminal SN(s)") +  " has been successfully updated.", "Updated", clsFunction.IconType.iExclamation);
            else
                dbFunction.SetMessageBox((chkRelease.Checked ? "Release/transfer" : "Terminal SN(s)") + " has been successfully saved.", "Saved", clsFunction.IconType.iInformation);


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
                        dbAPI.ExecuteAPI("GET", "Search", "Terminal SN Info", clsArray.ID[i], "Get Info Detail", "", "GetInfoDetail");
                        
                        if (dbAPI.isNoRecordFound() == false)
                        {
                            dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                            string sTypeID = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                            string sModelID = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                            string sStatus = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5);
                            string sType = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                            string sModel = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                            string sBrand = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                            string sLocation = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);                           
                            string sAssetType = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 15);
                            string sPONo = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 16);
                            string sPartNo = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 18);

                            item.SubItems.Add(sTypeID);
                            item.SubItems.Add(sModelID);
                            item.SubItems.Add(clsArray.Description[i].ToString());
                            item.SubItems.Add(sType);
                            item.SubItems.Add(sModel);
                            item.SubItems.Add(sBrand);
                            item.SubItems.Add(sLocation);
                            item.SubItems.Add(sAssetType);
                            item.SubItems.Add(sPONo);
                            item.SubItems.Add(sPartNo);
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
        

        private bool isItemOnList(string pID, string pDescrption)
        {
            bool isListed = false;
            if (lvwRelease.Items.Count > 0)
            {
                foreach (ListViewItem i in lvwRelease.Items)
                {
                    Debug.WriteLine("i="+i+">>"+i.SubItems[1].Text+" is equal with "+ pID);
                    //Debug.WriteLine("i=" + i + ">>" + i.SubItems[3].Text + " is equal with " + pDescrption);

                    if ((i.SubItems[1].Text.Equals(pID)))
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

            Debug.WriteLine("sSQL="+ sSQL);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "Terminal Movement Master", sSQL, "InsertCollectionMaster");

            txtTIID.Text = txtTerminalID.Text = clsLastID.ClassLastInsertedID.ToString();
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
                int iTerminalID = int.Parse(x.SubItems[1].Text);
                int iTerminalTypeID = int.Parse(x.SubItems[2].Text);
                int iTerminalModelID = int.Parse(x.SubItems[3].Text);
                string sSerialNo = x.SubItems[4].Text;

                // Insert                
                sRowSQL = "";
                sRowSQL = " (" + dbFunction.CheckAndSetNumericValue(txtTIID.Text) + ", " +
                sRowSQL + sRowSQL + " " + iTerminalID + ", " +
                sRowSQL + sRowSQL + " " + iTerminalTypeID + ", " +
                sRowSQL + sRowSQL + " " + iTerminalModelID + ", " +              
                sRowSQL + sRowSQL + " '" + sSerialNo + "') ";

                if (sSQL.Length > 0)
                    sSQL = sSQL + ", " + sRowSQL;
                else
                    sSQL = sSQL + sRowSQL;

            }

            Debug.WriteLine("sSQL=" + sSQL);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "Terminal Movement Detail", sSQL, "InsertCollectionDetail");
        }

        private void BulkReleaseTerminalDetail()
        {
            string sRowSQL = "";
            string sSQL = "";          
            int iTerminalID = 0;
            string sSerialNo = "";
            string sLocation = "";
            DateTime stReleaseDate = dteMReleaseDate.Value;
            string pReleseDate = stReleaseDate.ToString("yyyy-MM-dd");

            Debug.WriteLine("--BulkReleaseTerminalDetail--");
            Debug.WriteLine("fEdit=" + fEdit);
            
            dbFunction.GetProcessedByAndDateTime(); // Get processedby and datetime

            foreach (ListViewItem i in lvwRelease.Items)
            {
                sSQL = "";

                sLocation = cboRLocationTo.Text;

                foreach (ListViewItem x in lvwRelease.Items)
                {
                    iTerminalID = int.Parse(x.SubItems[1].Text);
                    sSerialNo = x.SubItems[4].Text;

                    // Update
                    sRowSQL = "";
                    sRowSQL = iTerminalID + "~" +
                    sRowSQL + sRowSQL + "" +  dbFunction.CheckAndSetNumericValue(txtLocationIDTo.Text) + "~" +
                    sRowSQL + sRowSQL + "" + sLocation + "~" +
                    sRowSQL + sRowSQL + "" + pReleseDate + "~" +
                    sRowSQL + sRowSQL + "" + clsUser.ClassProcessedBy + "~" +
                    sRowSQL + sRowSQL + "" + clsUser.ClassProcessedDateTime + "~" +
                    sRowSQL + sRowSQL + "" + txtRRemarks.Text;

                    if (sSQL.Length > 0)
                        sSQL = sSQL + "|" + sRowSQL;
                    else
                        sSQL = sSQL + sRowSQL;

                }

                sSQL = lvwRelease.Items.Count.ToString() + "^" + sSQL;

                Debug.WriteLine("sSQL=" + sSQL);

                dbAPI.ExecuteAPI("PUT", "Update", "Released Terminal Detail", sSQL, "", "", "UpdateBulkCollectionDetail");
            }

        }

        private void lvwHistoryList_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void lvwRelease_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                
                case Keys.Delete:

                    if (lvwRelease.Items.Count > 0)
                    {
                        dbFunction.removeItemListView(lvwRelease, true);
                    }
                    break;
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
                                             lvwRelease.SelectedItems[0].SubItems[4].Text + "," +
                                             lvwRelease.SelectedItems[0].SubItems[5].Text + "," +
                                             lvwRelease.SelectedItems[0].SubItems[6].Text + "," +
                                             lvwRelease.SelectedItems[0].SubItems[7].Text + "," +
                                             lvwRelease.SelectedItems[0].SubItems[9].Text +
                                             " ]";
                }
            }
        }

        private void btnRRemoveItem_Click(object sender, EventArgs e)
        {
            dbFunction.removeItemListView(lvwManual, true);
        }

        private void chkSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSelect.Text.Equals("CHECK ALL?"))
            {
                chkSelect.Text = "UNCHECK ALL?";

                // Check All
                foreach (ListViewItem i in lvwRelease.Items)
                {
                    i.Checked = true;
                }
            }
            else
            {
                chkSelect.Text = "CHECK ALL?";

                // UnCheck All
                foreach (ListViewItem i in lvwRelease.Items)
                {
                    i.Checked = false;
                }
            }
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

        private void btnViewHistory_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalSN, txtTerminalSN.Text)) return;

            Cursor.Current = Cursors.WaitCursor;

            clsSearch.ClassClientID = clsSearch.ClassMerchantID = clsSearch.ClassJobType = clsSearch.ClassRegionType = clsSearch.ClassRegionID = clsSearch.ClassIRIDNo = clsSearch.ClassStatus = clsSearch.ClassIsPullOut = clsFunction.iZero;           
            clsSearch.ClassDateFrom = clsSearch.ClassDateTo = clsFunction.sDateFormat;
            clsSearch.ClassActionMade = clsFunction.sDefaultSelect;
            clsSearch.ClassTerminalSN = txtTerminalSN.Text;
            clsSearch.ClassSIMSerialNo = clsFunction.sZero;

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

            //if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalSN, txtTerminalSN.Text)) return;

            lvwHistoryList.Items.Clear();

            if (dbFunction.isValidID(txtTerminalID.Text))
            {
                dbAPI.ExecuteAPI("GET", "View", "Service TerminalSN History List", dbFunction.CheckAndSetNumericValue(txtTerminalID.Text), "Advance Detail", "", "ViewAdvanceDetail");

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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ViewServiceHistory();
        }

        private void btnRSearch_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iReleaseTerminal;
            frmSearchField.sTerminalType = clsFunction.sNull;
            frmSearchField.sHeader = "Terminal Release";
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
        

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            dbFunction.checkAllListView(lvwRelease, chkRAll);
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

        private void cboIClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassClientID = 0;
            if (!cboIClient.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Client List", cboIClient.Text);
                clsSearch.ClassClientID = clsSearch.ClassOutFileID;

            }
            txtClientID.Text = clsSearch.ClassClientID.ToString();
        }

        private void cboAClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassClientID = 0;
            if (!cboAClient.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Client List", cboAClient.Text);
                clsSearch.ClassClientID = clsSearch.ClassOutFileID;

            }
            txtClientID.Text = clsSearch.ClassClientID.ToString();
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
        
        private void chkRelease_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRelease.Checked)
            {
                //cboRLocationTo.Enabled = true;
                //cboRLocationFrom.Enabled = true;
                //dteRReleaseDate.Enabled = true;
                //btnRLocationApply.Enabled = btnRStatusApply.Enabled = false;
                //btnRApply.Enabled = false;
                
            }
            else
            {
                //cboRLocationTo.Enabled = true;
                //cboRLocationFrom.Enabled = false;
                //dteRReleaseDate.Enabled = false;
                //btnRLocationApply.Enabled = btnRStatusApply.Enabled = true;
                //btnRApply.Enabled = true;
            }

            lblMode.Text = "MODE: " + (chkRelease.Checked ? "RELEASE" : "TRANSFER");
        }

        private void BulkUpdateTerminalSNDetail(ListView lvw, bool isRelease)
        {
            Debug.WriteLine("--BulkUpdateTerminalDetail--");
            Debug.WriteLine("isRelease=" + isRelease);
            Debug.WriteLine("fEdit=" + fEdit);

            string sRowSQL = "";
            string sSQL = "";
            int iTerminalID = 0;
            string sSerialNo = "";           
            DateTime stReleaseDate =  (isRelease ? dteRReleaseDate.Value : dteMReleaseDate.Value);
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
                    iTerminalID = int.Parse(x.SubItems[1].Text);
                    sSerialNo = x.SubItems[4].Text;

                    // Update
                    sRowSQL = "";
                    sRowSQL = iTerminalID + "~" +
                    sRowSQL + sRowSQL + "" + dbFunction.CheckAndSetNumericValue(txtTerminalStatus.Text) + "~" +
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

                dbAPI.ExecuteAPI("PUT", "Update", "Multiple Terminal Detail", sSQL, "", "", "UpdateBulkCollectionDetail");

                //dbAPI.ExecuteAPI("PUT", "Update", (isRelease ? "Multiple Terminal Detail" : "Multiple Terminal SN Detail") , sSQL, "", "", "UpdateBulkCollectionDetail");
            }

        }

        private void cboRStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassTerminalStatusID = 0;
            if (!cboRStatus.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Status List", cboRStatus.Text);
                clsSearch.ClassTerminalStatusID = clsSearch.ClassOutFileID;
            }

            txtTerminalStatus.Text = clsSearch.ClassTerminalStatusID.ToString();
        }
        
        private void btnRLocationApply_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidListViewChecked(lvwRelease)) return;

            dbFunction.updateListView(lvwRelease, 8, cboRLocationTo.Text, true);
        }

        private void btnRStatusApply_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidListViewChecked(lvwRelease)) return;

            dbFunction.updateListView(lvwRelease, 12, cboRStatus.Text, true);
        }

        private void btnMAddItem_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iTerminal;
            frmSearchField.iStatus = 99; // ROCKY - TERMINAL IMPORT ISSUE: FIXED DISPLAYING INSTALLED STATUS
            frmSearchField.sTerminalType = "View Terminal";
            frmSearchField.sHeader = "MANUAL - TERMINAL";
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
                    txtTerminalSN.Enabled = false;
                    cboMType.Enabled = cboMModel.Enabled = cboMBrand.Enabled = cboMAssetType.Enabled = cboMAllocation.Enabled = cboMClient.Enabled = false;
                    txtMInvNo.Enabled = txtMPONo.Enabled = txtMPartNo.Enabled = false;
                    
                    dteMReceiveDate.Enabled = false;
                    btnMApplyChanges.Enabled = true;

                    LoadSelected(lvwManual);

                    Cursor.Current = Cursors.Default;
                }
                catch (Exception ex)
                {
                    dbFunction.SetMessageBox("Error message " + ex.Message + "\n\n" + clsDefines.CONTACT_ADMIN_MESSAGE, "Manual Terminal", clsFunction.IconType.iError);
                }
             
            }
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

            dbFunction.updateListView(lvwManual, 8, cboMLocation.Text, true); // location
            dbFunction.updateListView(lvwManual, 12, cboMStatus.Text, true); // status
        }

        private void chkMAll_CheckedChanged(object sender, EventArgs e)
        {
            dbFunction.checkAllListView(lvwManual, chkMAll);
        }

        private void PopulateMerchantTextBox()
        {
            Debug.WriteLine("--PopulateMerchantTextBox--");

            txtMerchName.Text =
            txtMerchTID.Text =
            txtMerchMID.Text =
            txtMerchRegion.Text = 
            txtAssignTerminalSN.Text =
            txtAssignSIMSN.Text = 
            txtAssignComponents.Text =
            txtIRStatusDescription.Text =
            clsFunction.sNull;

            try
            {
                if (dbFunction.isValidID(txtTerminalID.Text))
                {
                    clsSearch.ClassOutParamValue = dbAPI.getInfoDetailJSON("Search", "Merchant Assigned SN Info", txtTerminalID.Text + clsFunction.sPipe + clsDefines.SEARCH_TERMINAL);

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
                if (dbFunction.isValidID(txtTerminalID.Text))
                {
                    dbAPI.ExecuteAPI("GET", "Search", "Last ControlNo Info", clsDefines.TAG_Terminal + clsFunction.sPipe + txtTerminalID.Text, "Get Info Detail", "", "GetInfoDetail");

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
            lblSelectedHeader.Text = txtTerminalSN.Text + clsDefines.gPipe +
                                     cboMType.Text + clsDefines.gPipe +
                                     cboMModel.Text + clsDefines.gPipe +
                                     cboMLocation.Text + clsDefines.gPipe +
                                     cboMStatus.Text + clsDefines.gPipe +
                                     cboMClient.Text + clsDefines.gPipe +
                                     cboMAssetType.Text + clsDefines.gPipe +
                                     txtMerchName.Text;

        }

        private void btnClearTerminalSN_Click(object sender, EventArgs e)
        {
            btnClear_Click(this, e);
        }

        private void viewSNMinMaxServiceInfo()
        {
            txtFirstServiceMade.Text = txtLastServiceMade.Text = txtLastServiceActionMade.Text = clsFunction.sNull;

            if (dbFunction.isValidID(txtTerminalID.Text))
            {
                // Min
                dbAPI.ExecuteAPI("GET", "Search", "SN MIN/MAX Service Info", txtTerminalID.Text + clsFunction.sPipe + clsDefines.TAG_Terminal + clsFunction.sPipe + clsDefines.TAG_MIN, "Get Info Detail", "", "GetInfoDetail");

                Debug.WriteLine("viewSNMinMaxServiceInfo->MIN");
                dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gComma, 0);

                if (clsSearch.ClassOutParamValue.Length > 0)
                {
                    jsonObj obj = JsonConvert.DeserializeObject<jsonObj>(clsSearch.ClassOutParamValue);

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        txtFirstServiceMade.Text = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_ServiceJobTypeDescription);                    
                    }
                }

                // Max
                dbAPI.ExecuteAPI("GET", "Search", "SN MIN/MAX Service Info", txtTerminalID.Text + clsFunction.sPipe + clsDefines.TAG_Terminal + clsFunction.sPipe + clsDefines.TAG_MAX, "Get Info Detail", "", "GetInfoDetail");

                Debug.WriteLine("viewSNMinMaxServiceInfo->MAX");
                dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gComma, 0);

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
            
            dbFunction.updateListView(lvwRelease, 8, cboRLocationTo.Text, false); // LocationTo            
            dbFunction.updateListView(lvwRelease, 12, cboRStatus.Text, false); // Status

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

        private void btnValidate_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            string mode = "Import";
            string inventory_type = "Terminal";

            if (!dbFunction.fPromptConfirmation($"Are you sure to validate the records on list?")) return;

            Cursor.Current = Cursors.WaitCursor;
            
            ucStatusDisplay.SetStatus($"Validating list...", Enums.StatusType.Processing);

            btnImportSave.Enabled = false;

            // ✅ build CSV
            foreach (DataGridViewRow row in grdList.Rows)
            {
                if (row.IsNewRow) continue;

                var value = row.Cells[1].Value?.ToString(); // SN
                if (string.IsNullOrWhiteSpace(value)) continue;

                value = Regex.Replace(value, @"[\r\n,\s]", "");
                sb.Append(value + ",");
            }

            string result = sb.ToString().TrimEnd(',');

            Debug.WriteLine($"result=[{result}]");

            // ✅ call API
            dbAPI.ExecuteAPI("GET", "View", "Inventory Bulk Cross-Check List",
                $"{mode}{clsFunction.sPipe}{inventory_type}{clsFunction.sPipe}{result}",
                "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;
            if (dbAPI.isNoRecordFound()) return;

            // ✅ 1. Add RESULT column if not exists
            if (!grdList.Columns.Contains("RESULT"))
            {
                grdList.Columns.Add("RESULT", "RESULT");
            }

            // ✅ 2. Build dictionary from API response (SN → Result)
            Dictionary<string, string> resultMap = new Dictionary<string, string>();

            for (int i = 0; i < clsArray.ID.Length; i++)
            {
                string json = clsArray.detail_info[i];

                string sn = dbAPI.GetValueFromJSONString(json, clsDefines.TAG_SerialNo);
                string status = dbAPI.GetValueFromJSONString(json, clsDefines.TAG_Result);

                if (!string.IsNullOrEmpty(sn))
                {
                    resultMap[sn] = status;
                }
            }

            // ✅ 3. Apply result to grid
            foreach (DataGridViewRow row in grdList.Rows)
            {
                if (row.IsNewRow) continue;

                string sn = row.Cells[1].Value?.ToString();
                if (string.IsNullOrWhiteSpace(sn)) continue;

                sn = Regex.Replace(sn, @"[\r\n,\s]", "");

                if (resultMap.ContainsKey(sn))
                {
                    string status = resultMap[sn];

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

            btnImportSave.Enabled = true;

            Cursor.Current = Cursors.Default;
            
            ucStatusDisplay.SetStatus($"Validating list..completed.", Enums.StatusType.Success);

            dbFunction.SetMessageBox("Validate list complete. Check summary count.", lblHeader.Text, clsFunction.IconType.iInformation);
        }

        private void btnCopyClipboard_Click(object sender, EventArgs e)
        {
            dbFunction.CopyGridToClipboard(grdList);

            dbFunction.SetMessageBox($"Data list copied to clipboard!", lblHeader.Text, clsFunction.IconType.iInformation);
            
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

                // ✅ get SN (column index 1)
                string sn = row.Cells[1].Value?.ToString();
                if (!string.IsNullOrWhiteSpace(sn))
                {
                    sn = Regex.Replace(sn, @"[\r\n,\s]", "");
                    uniqueSN.Add(sn);
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

        private void btnRCopyClipboard_Click(object sender, EventArgs e)
        {
            dbFunction.CopyListViewToClipboard(lvwRelease);

            dbFunction.SetMessageBox($"Data list copied to clipboard!", lblHeader.Text, clsFunction.IconType.iInformation);
        }
    }
}
