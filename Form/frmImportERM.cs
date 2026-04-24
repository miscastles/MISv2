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
using System.Globalization;
using System.Threading;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using static MIS.Function.AppUtilities;

namespace MIS
{
    public partial class frmImportERM : Form
    {
        private clsAPI dbAPI;
        private clsINI dbSetting;
        private clsFile dbFile;
        private clsFunction dbFunction;
        private clsInternet dbInternet;
        private static string ExcelFilePath = @"";
        private string sExcelFileName = "";        
        private string sDateFrom = "";
        private string sDateTo = "";
        private string sSheet = "";
        private int iHeaderRowIndex = 0;
        private int iColumnMaxCount = 0;
        private string sRowCSV = "";     
        
        public static string sHeader;

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

        public frmImportERM()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwList, true);
            dbFunction.setDoubleBuffer(lvwSearch, true);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dbFunction.ClearTextBox(this);
            dbFunction.ClearListView(lvwSearch);

            InitTextBox();
            InitPage(0, 0);
            btnPreview.Enabled = false;
            btnCheck.Enabled = false;
            lvwList.Enabled = false;

            btnCheck.Text = "UNCHECK ALL";
            btnCheck_Click(this, e);

        }

        private void frmERMTool_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbSetting = new clsINI();
            dbFile = new clsFile();
            dbFunction = new clsFunction();
            dbInternet = new clsInternet();
            dbSetting.InitDatabaseSetting();
            //InitListView();

            dbFunction.ClearTextBox(this);
            InitTextBox();
            
            //iCurrentPage = 0;
            InitPage(0, 0);
            btnPreview.Enabled = false;
            btnCheck.Enabled = false;
            lvwList.Enabled = false;

            LoadType();
            dbFunction.ClearComboBox(this);
            cboSearchMDRType.Enabled = false;
            dbAPI.FillComboBoxMDRType(cboSearchMDRType);

            //lblHeader.Text = lblHeader.Text + " " + "[ " + sHeader + " ]";

            // Load Mapping
            dbAPI.ExecuteAPI("GET", "View", "Type", "ERM", "Mapping", "", "ViewMapping");
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            string sExtension = "";
            int iIndexCount = 0;

            OpenFileDialog ExcelDialog = new OpenFileDialog();
            ExcelDialog.Filter = "EXCEL Files|*.csv;*.xls;*.xlsx";
            ExcelDialog.InitialDirectory = @"C:\CASTLESTECH_MIS\IMPORT\";
            ExcelDialog.Title = "Select FSR File";

            if (txtClient.Text.Length <= 0)
            {
                MessageBox.Show("No client selected for import.", "Client", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            
            if (ExcelDialog.ShowDialog() == DialogResult.OK)
            {
                dbFunction.ClearListView(lvwSearch);
                InitTextBox();

                ExcelFilePath = ExcelDialog.FileName;
                txtPathFileName.Text = ExcelDialog.FileName;
                txtPathFileName.ReadOnly = true;
                btnLoadFile.Enabled = false;
                sExcelFileName = Path.GetFileName(txtPathFileName.Text);
                txtFileName.Text = sExcelFileName;
                sExtension = Path.GetExtension(txtPathFileName.Text);
                sExtension = sExtension.Replace(clsFunction.sPeriod, clsFunction.sNull).ToUpper();

                try
                {
                    //dbFunction.SetMessageBox("Import and Processing Import. This will take a few minute(s).",  "Import", clsFunction.IconType.iInformation);

                    // Import
                    if (!fContinueConfirm())
                    {
                        btnLoadFile.Enabled = true;
                        return;
                    }

                    // Check execution time
                    var watch = Stopwatch.StartNew();
                    string sReqTime = "";
                    string sResTime = "";

                    //frmLoading frmWait = new frmLoading(); // Open Wait Window
                    //clsFunction.WaitWindow(true, frmWait);

                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                    // Get Format FileName
                    //string sFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iERM, 0);
                    //dbFile.DeleteCSV(sFileName);
                    string sFileName = "x";

                    var elapseMS = "";

                    // Init ListView
                    //InitListView();

                    switch (sExtension)
                    {
                        case "XLSX":
                        case "XLS":
                            sSheet = dbFunction.getSheetName(txtPathFileName.Text);
                            if (!dbFunction.isValidSheetName(sSheet)) return;

                            txtSheetName.Text = "`" + sSheet + "$" + "`";
                            txtFileName.Text = sExcelFileName;
                            
                            // Create Temporary database            
                            Debug.WriteLine("Temporary Table=" + txtSheetName.Text);
                            dbAPI.ExecuteAPI("POST", "Create", "", "", txtSheetName.Text, "", "CreateTempTable");

                            ucStatusImport.iState = 3;
                            ucStatusImport.sMessage = "IMPORTING:" + txtFileName.Text;
                            ucStatusImport.iMin = 0;
                            ucStatusImport.iMax = 0;
                            ucStatusImport.AnimateStatus();

                            Debug.WriteLine("=>>ImportToDummyDataGrid");
                            ImportToDummyDataGrid();

                            Debug.WriteLine("=>>SetListViewHeader");
                            SetListViewHeader(sFileName);

                            Debug.WriteLine("=>>SetListViewData");
                            SetListViewData(sFileName, ref iIndexCount);

                            InitHeader();

                            if (!isValidHeader()) return; // Check Header

                            // Delete ERMTempDetail
                            Debug.WriteLine("API Call DeleteERMTempDetail");
                            dbAPI.DeleteERMTempDetail();

                            sReqTime = dbFunction.GetRequestTime("ERM Import Detail");

                            // Import ERMTempDetail
                            for (int i = 1; i <= iIndexCount; i++)
                            {
                                string sImportFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iERM, i);

                                ucStatusImport.iState = 3;
                                ucStatusImport.sMessage = "UPLOADING:" + sImportFileName;
                                ucStatusImport.iMin = 0;
                                ucStatusImport.iMax = 0;
                                ucStatusImport.AnimateStatus();

                                // Upload File
                                Debug.WriteLine("=>>UploadFile=" + sImportFileName);
                                UploadFile(sImportFileName);

                                Debug.WriteLine("=>>API Call ImportERMDetail=" + sImportFileName);
                                dbAPI.ExecuteAPI("POST", "Import", "CSV", sImportFileName, "ERM Import Detail", "", "ImportERMDetail");

                            }

                            dbFunction.GetResponseTime("ERM Import Detail");

                            ucStatusImport.iState = 3;
                            ucStatusImport.sMessage = "FINALIZE " + clsSearch.ClassParticularName + " ERM BILLING" + " ON PROGRESS";
                            ucStatusImport.iMin = 0;
                            ucStatusImport.iMax = 0;
                            ucStatusImport.AnimateStatus();

                            // Process ERMTempDetail
                            Debug.WriteLine("API Call ProcessERM");
                            dbAPI.ProcessERMTempDetail(clsSearch.ClassParticularID.ToString(), clsSearch.ClassParticularName);

                            Cursor.Current = Cursors.Default; // Back to normal 

                            //dbFunction.SetMessageBox("Retrieving process data. This will take a few minute(s).", "Retrieve", clsFunction.IconType.iInformation);

                            // Start Import Here
                            Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                            // Drop table create from sheetname
                            if (txtSheetName.Text.Length > 0)
                                dbAPI.ExecuteAPI("DELETE", "Delete", "", txtSheetName.Text.Replace("`", ""), "Drop Temp Table", "", "DeleteCollectionDetail");

                            dbAPI.ResetAdvanceSearch();

                            InitPage(int.Parse(clsFunction.sOne), GetTotalPage());
                            
                            LoadERM();
                            
                            GetERMTotal();

                            FilterDate();
                            
                            sResTime = dbFunction.GetResponseTime("ERM Import Detail");

                            watch.Stop();
                            elapseMS = (watch.ElapsedMilliseconds / 1000).ToString() + " Second(s)";

                            ucStatusImport.iState = 3;
                            ucStatusImport.sMessage = "COMPLETED";
                            ucStatusImport.iMin = 0;
                            ucStatusImport.iMax = 0;
                            ucStatusImport.AnimateStatus();

                            Debug.WriteLine("=>>ImportERMDetail=" + sFileName + " Complete!!!" + " RequestTime=" + sReqTime + "|ResponseTime=" + sResTime + "|ExecutionTime=" + elapseMS);
                            dbFunction.SetMessageBox("Process successfully completed." + "\n\nStart Time: " + sReqTime + "\n" + "End Time: " + sResTime + "\n" + "Executiion Time: " + elapseMS, "XLSX: Complete", clsFunction.IconType.iInformation);

                            break;
                        case "CSV":
                            // Copy
                            dbFile.CopyFile(ExcelFilePath, clsGlobalVariables.strFTPLocalPath + sFileName);

                            // Upload File
                            UploadFile(sFileName);

                            dbFunction.GetRequestTime("ERM Import Detail");

                            // Process CSV File
                            dbAPI.ExecuteAPI("POST", "Import", "CSV", sFileName, "ERM Import Detail", "", "ImportERMDetail");

                            dbFunction.GetResponseTime("ERM Import Detail");

                            Cursor.Current = Cursors.Default; // Back to normal 

                            dbFunction.SetMessageBox(clsDefines.TAKE_FEW_MINUTE_MSG, "Retrieve", clsFunction.IconType.iInformation);

                            // Start Import Here
                            Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                            dbAPI.ResetAdvanceSearch();

                            InitPage(int.Parse(clsFunction.sOne), GetTotalPage());
                            LoadERM();

                            sResTime = dbFunction.GetResponseTime("ERM Import Detail");

                            watch.Stop();
                            elapseMS = (watch.ElapsedMilliseconds / 1000).ToString() + " Second(s)"; ;

                            Debug.WriteLine("=>>ImportERMDetail=" + sFileName + " Complete!!!" + " RequestTime=" + sReqTime + "|ResponseTime=" + sResTime + "|ExecutionTime=" + elapseMS);
                            dbFunction.SetMessageBox("Process successfully completed." + "\n\nStart Time: " + sReqTime + "\n" + "End Time: " + sResTime + "\n" + "Execution Time: " + elapseMS, "CSV: Complete", clsFunction.IconType.iInformation);

                            break;
                    }


                    Debug.WriteLine("Total execution time: " + elapseMS + " Seconds");

                    //clsFunction.WaitWindow(false, frmWait); // Close Wait Window     

                    // upload physical file
                    string pLocalPath = $"{txtPathFileName.Text.Replace(txtFileName.Text, "")}";
                    string pRemotePath = $"{clsGlobalVariables.strFTPRemoteErmPath}{clsGlobalVariables.strAPIBank}{clsFunction.sBackSlash}";
                    string pFileName = $"{txtFileName.Text}";

                    Debug.WriteLine("Uploading import erm...");
                    Debug.WriteLine($"pLocalPath=[{pLocalPath}]");
                    Debug.WriteLine($"pRemotePath=[{pRemotePath}]");
                    Debug.WriteLine($"pFileName=[{pFileName}]");

                    ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                    ftpClient.delete(pRemotePath + pFileName);
                    ftpClient.upload(pRemotePath + pFileName, pLocalPath + pFileName);
                    ftpClient.disconnect();

                    Debug.WriteLine("Uploading import erm...complete");

                    Cursor.Current = Cursors.Default; // Back to normal 
                   
                    btnLoadFile.Enabled = true;
                    btnPreview.Enabled = true;
                    btnCheck.Enabled = true;
                    lvwList.Enabled = true;

                    btnCheck.Text = "UNCHECK ALL";
                    btnCheck_Click(this, e);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception error " + ex.Message);
                    dbFunction.SetMessageBox(ex.Message, "Import FSR File", clsFunction.IconType.iError);
                    btnLoadFile.Enabled = true;                    
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
               
        private int GetRowCount(DataGridView obj)
        {
            int iCount = 0;

            iCount = obj.RowCount;

            return iCount;
        }

        private int GetColumnCount(DataGridView obj)
        {
            int iCount = 0;
            int i = 0;

            int iColCount = obj.ColumnCount;

            for (i = 0; i < iColCount; i++)
            {
                string cellParam = obj.Columns[i].Name;

                if (cellParam.Length > 0)
                {
                    iCount = i;
                }
            }

            iCount += 1;

            return iCount;
        }

        private string GetColumnValue(int iRowIndex, int iColIndex, DataGridView obj)
        {
            string sColumnValue = dbFunction.RemoveUnwantedChar(obj.Rows[iRowIndex].Cells[iColIndex].Value.ToString());

            return sColumnValue;
        }
        
        private void btnProcess_Click(object sender, EventArgs e)
        {
            // Waiting / Hour Glass
            Cursor.Current = Cursors.WaitCursor;            
            
            // Back to normal 
            Cursor.Current = Cursors.Default;

            dbFunction.SetMessageBox("Import ERM successfully saved.", "Saved", clsFunction.IconType.iInformation);

            btnClear_Click(this, e);

            btnLoadFile.Enabled = true;
        }        

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmERMTool_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }
        private void UploadFile(string sFileName)
        {
            
            Debug.WriteLine("--UploadFile--");
            Debug.WriteLine("sFileName=" + sFileName);
            Debug.WriteLine("clsGlobalVariables.strFTPURL="+ clsGlobalVariables.strFTPURL);
            Debug.WriteLine("clsGlobalVariables.strFTPUserName=" + clsGlobalVariables.strFTPUserName);
            Debug.WriteLine("clsGlobalVariables.strFTPPassword=" + clsGlobalVariables.strFTPPassword);
            Debug.WriteLine("clsGlobalVariables.strFTPLocalPath=" + clsGlobalVariables.strFTPLocalPath);
            Debug.WriteLine("clsGlobalVariables.strFTPUploadPath=" + clsGlobalVariables.strFTPUploadPath);

            // Upload File to FTP                
            ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
            ftpClient.delete(clsGlobalVariables.strFTPUploadPath + sFileName);
            ftpClient.upload(clsGlobalVariables.strFTPUploadPath + sFileName, @clsGlobalVariables.strFTPLocalPath + sFileName);
            ftpClient.disconnect(); // ftp disconnect
        }

        private void InitListView()
        {
            lvwSearch.Clear();
            lvwSearch.Items.Clear();
            lvwSearch.View = View.Details;
            lvwSearch.Columns.Add("LINE#", 50, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("FSRNo", dbFunction.ID_Width(), HorizontalAlignment.Left);
            lvwSearch.Columns.Add("No", dbFunction.ID_Width(), HorizontalAlignment.Left);
            lvwSearch.Columns.Add("MERCHANT", 320, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("MID", 180, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("TID", 140, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("INVOICE NO.", 90, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("BATCH NO.", 90, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("PAN", 90, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("DATE", 90, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("TIME", 90, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("TXN AMOUNT", 140, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("AUTH CODE", 140, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("REF. NO.", 140, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("PHONE NO.", 140, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("EMAIL", 140, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("NIRC", 140, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("ADDITIONAL INFORMATION", 140, HorizontalAlignment.Left);

        }

        private void ucStatusImport_Load(object sender, EventArgs e)
        {

        }

        private void LoadERM()
        {
            int i = 0;
            int iLineNo = 0;
            int iOffSetFrom = 0;
            int iOffSetTo = 0;

            ucStatusImport.iState = 3;
            ucStatusImport.sMessage = "LOADING DATA";
            ucStatusImport.iMin = 0;
            ucStatusImport.iMax = 0;
            ucStatusImport.AnimateStatus();


            if (clsSearch.ClassCurrentPage > int.Parse(clsFunction.sOne))
            {
                iOffSetFrom = (clsFunction.iOffSetTo * clsSearch.ClassCurrentPage - clsFunction.iOffSetTo) + int.Parse(clsFunction.sOne);
                iOffSetTo = iOffSetFrom + clsFunction.iOffSetTo;
            }
            else
            {
                iOffSetFrom = int.Parse(clsFunction.sOne);
                iOffSetTo = clsFunction.iOffSetTo;
            }
                
            
            Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

            
            lvwSearch.Items.Clear();

            dbFunction.GetRequestTime("LoadERM");

            clsSearch.ClassAdvanceSearchValue = iOffSetFrom.ToString() + clsFunction.sPipe + iOffSetTo;

            dbAPI.ExecuteAPI("GET", "View", "ERM Temp Detail", clsSearch.ClassAdvanceSearchValue, "ERM", "", "ViewERM");

            dbFunction.GetResponseTime("LoadERM");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.FSRNo.Length > i)
                {                    
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    //item.SubItems.Add(clsArray.FSRNo[i]);
                    //item.SubItems.Add(clsArray.No[i]);
                    item.SubItems.Add(clsArray.MerchantName[i]);
                    item.SubItems.Add(clsArray.MID[i]);
                    item.SubItems.Add(clsArray.TID[i]);
                    item.SubItems.Add(clsArray.InvoiceNo[i]);
                    item.SubItems.Add(clsArray.BatchNo[i]);
                    item.SubItems.Add(clsArray.FSR[i]);
                    item.SubItems.Add(clsArray.FSRDate[i]);
                    item.SubItems.Add(clsArray.FSRTime[i]);
                    item.SubItems.Add(clsFunction.sDash);
                    item.SubItems.Add(clsArray.TxnAmt[i]);
                    item.SubItems.Add(clsArray.AuthCode[i]);
                    item.SubItems.Add(clsArray.RefNo[i]);
                    item.SubItems.Add(clsArray.MerchantContactNo[i]);
                    item.SubItems.Add(clsArray.MerchantRepresentative[i]);
                    item.SubItems.Add(clsArray.NRIC[i]);
                    item.SubItems.Add(clsArray.AdditionalInformation[i]);

                    //Debug.WriteLine("iLineNo="+ iLineNo.ToString()+ "|clsArray.MerchantName[i]="+ clsArray.MerchantName[i]);

                    dbFunction.AppDoEvents(true);

                    lvwSearch.Items.Add(item);
                    
                    i++;

                }

                dbFunction.ListViewAlternateBackColor(lvwSearch);

                sDateFrom = clsArray.FSRDate[0];
                sDateTo = clsArray.FSRDate[i - 1];

                //ComputeTotals();
                //GetERMTotal();

                //FilterDate();


            }
            else
            {
                // Do noting.
            }

            Cursor.Current = Cursors.Default; // Normal
        }
            
        private void FilterDate()
        {
            
            clsSearch.ClassDateFrom = clsSearch.ClassDateTo = clsFunction.sDateFormat;

            dbAPI.ExecuteAPI("GET", "Search", "ERM Date Range", "", "Get Info Detail", "", "GetInfoDetail");

            if (dbAPI.isNoRecordFound() == false)
            {
                clsSearch.ClassDateFrom = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                clsSearch.ClassDateTo = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
            }

            txtFilterDate.Text = clsSearch.ClassDateFrom + " to " + clsSearch.ClassDateTo;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            // Check User Access Rights
            if (!dbAPI.isValidUserAccess(clsAPI.UserFunctionType.isView, clsUser.ClassUserID, 5)) return;

            if (lvwSearch.Items.Count > 0)
            {
                Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

                try
                {
                    dbAPI.ExecuteAPI("POST", "Create", "", "", "ERM Temp Detail Type", "", "CreateTempTable"); // Create temporary table

                    ProcessReport();

                    notifyEmail();
                }
                catch (Exception ex)
                {
                   MessageBox.Show(ex.Message);
                }

                Cursor.Current = Cursors.Default;  // Back to normal
            }
            else
            {
                dbFunction.SetMessageBox("No report to preview.", "Preview Report", clsFunction.IconType.iError);
            }
            
        }

        private void InitTextBox()
        {
            txtTTrxnAmt.Text = clsFunction.sDefaultAmount;
            txtTTrxnCount.Text = clsFunction.sZero;
            txtTRecurring.Text = clsFunction.sDefaultAmount;
            txtFilterDate.Text = clsFunction.sDateDefault + " to " + clsFunction.sDateDefault;
        }

        private void InitPage(int iCurrentPage, int iTotalPage)
        {
            if (iTotalPage > 0)
            {
                // do nothing...
            }
            else
            {
                iCurrentPage = 0;
            }
            
            clsSearch.ClassCurrentPage = iCurrentPage;
            clsSearch.ClassTotalPage = iTotalPage;

            txtPage.Text = iCurrentPage.ToString() + " / " + iTotalPage.ToString();
        }

        private int GetTotalPage()
        {
            int iCount = 0;
            int totalPage = 0;
            int iLimitSize = clsFunction.iOffSetTo;

            dbAPI.GetViewCount("Search", "ERM Temp Detail", clsFunction.sZero, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            if (iCount > 0)
            {
                totalPage = (int)Math.Ceiling((double)iCount / iLimitSize);
            }
            else
            {
                totalPage = 0;
            }
                
            return totalPage;
        }

        private void GetERMTotal()
        {
            int iCount = 0;
            double iTotal = 0;
            double iTRecurring = 0;
            
            ucStatusImport.iState = 3;
            ucStatusImport.sMessage = "COMPUTING SUMMARY COUNT";
            ucStatusImport.iMin = 0;
            ucStatusImport.iMax = 0;
            ucStatusImport.AnimateStatus();

            iCount = 0;
            clsTerminal.ClassTerminalCount = 0;
            dbAPI.GetViewCount("Search", "ERM Temp Detail Total Trxn Count", clsFunction.sZero, "Get Count");            
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            txtTTrxnCount.Text = (iCount > 0 ? iCount.ToString("N0") : "0");
            clsSearch.ClassTCount = (iCount > 0 ? iCount.ToString("N0") : "0");

            if (iCount > 0)
            {
                iTotal = 0;
                clsTerminal.ClassTerminalTotal = 0;
                clsSearch.ClassTAmount = clsFunction.sDefaultAmount;
                dbAPI.GetViewTotal("Search", "ERM Temp Detail Total Amt Count", clsFunction.sZero, "Get Total");
                if (dbAPI.isNoRecordFound() == false)
                    iTotal = clsTerminal.ClassTerminalTotal;
            }
                                    
            txtTTrxnAmt.Text = (iTotal > 0 ? iTotal.ToString("N") : "0.00");
            clsSearch.ClassTAmount = (iTotal > 0 ? iTotal.ToString("N") : "0.00");
           
            // Recurring
            iTRecurring = iTotal * 0.0005;
            txtTRecurring.Text = (iTRecurring > 0 ? iTRecurring.ToString("N") : "0.00");
            clsSearch.ClassTRecurring = txtTRecurring.Text;

            ucStatusImport.iState = 3;
            ucStatusImport.sMessage = "COMPLETED";
            ucStatusImport.iMin = 0;
            ucStatusImport.iMax = 0;
            ucStatusImport.AnimateStatus();
        }

        private void btnLastPage_Click(object sender, EventArgs e)
        {
            clsSearch.ClassCurrentPage = clsSearch.ClassTotalPage;
            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadERM();
        }

        private void btnFirstPage_Click(object sender, EventArgs e)
        {
            clsSearch.ClassCurrentPage = int.Parse(clsFunction.sOne);
            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadERM();
        }

        private void btnPreviousPage_Click(object sender, EventArgs e)
        {
            if (clsSearch.ClassCurrentPage > int.Parse(clsFunction.sOne))
                clsSearch.ClassCurrentPage--;

            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadERM();
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (clsSearch.ClassCurrentPage < clsSearch.ClassTotalPage)
                clsSearch.ClassCurrentPage++;

            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadERM();
        }

        /*
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
                string extension = file.Extension.Replace(clsFunction.sPeriod, clsFunction.sNull).ToUpper();
                switch (extension)
                {
                    case "XLS":
                        if (clsSystemSetting.ClassSystemMSOffice.CompareTo("2013") == 0)
                            strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathName + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
                        else
                            strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Jet OLEDB:Engine Type=5;Extended Properties=\"Excel 8.0;\"";
                        break;
                    case "XLSX":
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

                grdTempImport.DataSource = tbContainer;
                //MessageBox.Show("Excel file import complete.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exceptional error " + ex.Message);
                dbFunction.SetMessageBox("Office Version: " +
                                         clsSystemSetting.ClassSystemMSOffice + "\n\n" +
                                         ex.ToString(), ex.Message, clsFunction.IconType.iError);
            }
        }
        */

        private void ImportToDummyDataGrid()
        {
            string pathName = txtPathFileName.Text;
            DataTable tbContainer = new DataTable();
            string sheetName = sSheet;

            try
            {
                FileInfo file = new FileInfo(pathName);
                if (!file.Exists)
                    throw new Exception("Error, file doesn't exist!");

                if (file.Extension.ToUpper() != ".XLSX")
                    throw new Exception("Only .xlsx files are supported with EPPlus.");

                // Enable ExcelPackage to read without license prompt (non-commercial use)
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(file))
                {
                    var worksheet = package.Workbook.Worksheets[sheetName];
                    if (worksheet == null)
                        throw new Exception($"Sheet '{sheetName}' not found.");

                    int colCount = worksheet.Dimension.End.Column;
                    int rowCount = worksheet.Dimension.End.Row;

                    // Add columns from header row
                    for (int col = 1; col <= colCount; col++)
                    {
                        tbContainer.Columns.Add(worksheet.Cells[1, col].Text);
                    }

                    // Add rows (starting from row 2, skipping header)
                    for (int row = 2; row <= rowCount; row++)
                    {
                        DataRow dr = tbContainer.NewRow();
                        for (int col = 1; col <= colCount; col++)
                        {
                            dr[col - 1] = worksheet.Cells[row, col].Text;
                        }
                        tbContainer.Rows.Add(dr);
                    }
                }

                grdTempImport.DataSource = tbContainer;
                //MessageBox.Show("Excel file import complete.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exceptional error: " + ex.Message);
                dbFunction.SetMessageBox("EPPlus Import Error\n\n" +
                                         ex.ToString(), ex.Message, clsFunction.IconType.iError);
            }
        }


        private void SetListViewHeader(string sFileName)
        {
            int iRowCount;
            int iRowMax;            
            int i;          
            int iWidth = 50;

            Debug.WriteLine("--SetListViewHeader--");

            sRowCSV = clsFunction.sNull;
            iHeaderRowIndex = 14;
            iColumnMaxCount = 17;

            iRowCount = grdTempImport.RowCount;
            iRowMax = iRowCount - 1;

            lvwSearch.Items.Clear();
            lvwSearch.View = View.Details;
            for (i = 0; i < iColumnMaxCount; i++)
            {
                string cellParam = grdTempImport.Rows[iHeaderRowIndex].Cells[i].Value.ToString();
                cellParam = cellParam.Replace("\n", " ").Trim();

                Debug.WriteLine("cellParam="+ cellParam);

                // Set width size
                switch (cellParam)
                {
                    case "TID":
                    case "Invoice No":
                    case "Batch No":
                    case "PAN":
                    case "Date":
                    case "Time":
                    case "Txn Type":
                    case "Txn Amt":
                    case "Auth Code":                    
                    case "Phone No":
                        iWidth = 90;
                        break;
                    case "Ref. No":
                        iWidth = 110;
                        break;
                    case "Merchant":
                        iWidth = 350;
                        break;
                    case "MID":                    
                        iWidth = 130;
                        break;
                    case "Email":
                    case "NRIC":
                    case "Additional Information":
                        iWidth = 150;
                        break;
                    default:
                        iWidth = 50;
                        break;                        
                }

                InitListViewHeader(cellParam, iWidth);
                
                //sRowCSV += cellParam + clsFunction.sComma;

            }
            //sRowCSV = sRowCSV.Remove(sRowCSV.Length - 1, 1);
            //if (sRowCSV.Length > 0)
            //    dbFile.WriteCSV(sFileName, sRowCSV);
        }

        private void InitListViewHeader(string sHeader, int iWidth)
        {
            switch (sHeader)
            {
                case "Merchant":
                case "Email":
                case "NRIC":
                case "Additional Information":
                    lvwSearch.Columns.Add(sHeader.ToUpper(), iWidth, HorizontalAlignment.Left);
                    break;
                default:
                    lvwSearch.Columns.Add(sHeader.ToUpper(), iWidth, HorizontalAlignment.Right);
                    break;
            }
            
        }

        private void SetListViewData(string sFileName, ref int iIndexCount)
        {
            int iRowCount;            
            int iColCount;
            int iDataRowIndex;
            int i;
            int x;
            int iRowIndex = 1;
            int iRecordMinLimit = clsSystemSetting.ClassSystemRecordMinLimit;
            int iFileNameIndex = 0;
            int iRow = 0;
            int iStartIndex = 0;
            int iEndIndex = 0;            
            List<string> TempArrayDataCol = new List<String>();

            Debug.WriteLine("Writing as CSV File="+sFileName);
            iDataRowIndex = iHeaderRowIndex + 1;
            iColumnMaxCount = 17;

            iRowCount = grdTempImport.RowCount;            
            iColCount = grdTempImport.ColumnCount;

            Debug.WriteLine("iDataRowIndex="+ iDataRowIndex+"\n"+"RowCount=" +iRowCount+"\n"+"ColCount="+iColCount);
            
            StringBuilder columnbind = new StringBuilder();
            for (i = iDataRowIndex; i < iRowCount; i++)
            {
                iRow++;
                string sLineNo = grdTempImport.Rows[i].Cells[0].Value.ToString(); // Check is not blank                
                sRowCSV = "";
                if (sLineNo.Trim().Length > 0)
                {                    
                    sRowCSV = sLineNo + clsFunction.sComma;
                    for (x = 1; x < iColumnMaxCount; x++)
                    {
                        string sCellValue = StrClean(grdTempImport.Rows[i].Cells[x].Value.ToString().Trim().ToUpper());
                        sCellValue = sCellValue.Replace(",", clsFunction.sNull);
                        sRowCSV = sRowCSV + sCellValue + clsFunction.sComma;
                       
                    }
                    
                    // CleanUp character to accept during import
                    //sRowCSV = sRowCSV.Replace("(", clsFunction.sNull);
                    //sRowCSV = sRowCSV.Replace(")", clsFunction.sNull);
                    //sRowCSV = sRowCSV.Replace("'", clsFunction.sNull);
                    //sRowCSV = sRowCSV.Replace("&", clsFunction.sAndString);

                    sRowCSV = sRowCSV.Remove(sRowCSV.Length - 1, 1);
                    
                    if (sRowCSV.Length > 0)
                    {
                        TempArrayDataCol.Add(sRowCSV);
                    }                    
                }

                iRowIndex++;
            }

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
                        Debug.WriteLine("iStartIndex=" + iStartIndex.ToString() + "," + "iEndIndex=" + iEndIndex.ToString());
                        for (x = iStartIndex; x < iEndIndex; x++)
                        {
                           // Debug.WriteLine("x=" + x.ToString() + "," + "Data=" + clsArray.sRepoData[x].ToString());

                            columnbind.Append(clsArray.sRepoData[x].ToString());

                            if (x < iEndIndex - 1)
                                columnbind.Append("\r\n");
                        }

                        // Write To File
                        iFileNameIndex++;
                        string sNewFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iERM, iFileNameIndex);
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
                Debug.WriteLine("iTempStartIndex=" + iTempStartIndex.ToString() + "," + "iTempEndIndex=" + iTempEndIndex.ToString());
                for (x = iTempStartIndex; x < iTempEndIndex; x++)
                {
                    //Debug.WriteLine("x=" + x.ToString() + "," + "Data=" + clsArray.sRepoData[x].ToString());

                    columnbind.Append(clsArray.sRepoData[x].ToString());

                    if (x < iTempEndIndex - 1)
                        columnbind.Append("\r\n");
                }

                // Write To File
                iFileNameIndex++;
                string sNewFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iERM, iFileNameIndex);
                dbFile.DeleteCSV(sNewFileName);

                dbFile.WriteCSV(sNewFileName, columnbind.ToString());
            }

            iIndexCount = iFileNameIndex;
        }

        private void chkBillable_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            // Search Client
            dbAPI.ResetAdvanceSearch();
            frmSearchField.iSearchType = frmSearchField.SearchType.iClient;
            frmSearchField.sHeader = "CLIENT FOR ERM BILLING";
            frmSearchField.sSearchChar = clsFunction.sZero;
            frmSearchField.isCheckBoxes = false;
            frmSearchField.isPreview = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                clsSearch.ClassAdvanceSearchValue = clsSearch.ClassParticularID.ToString() + clsFunction.sPipe +
                                                    clsSearch.ClassParticularName;

                Debug.WriteLine("btnToolsERMBilling_Click::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                // Get Particular Detail
                dbAPI.ExecuteAPI("GET", "Search", "Particular Detail", clsSearch.ClassAdvanceSearchValue, "Particular Detail", "", "ViewParticularDetail");

                if (!clsGlobalVariables.isAPIResponseOK) return;

                if (dbAPI.isNoRecordFound() == false)
                    clsParticular.RecordFound = true;
                else
                    clsParticular.RecordFound = false;


                Cursor.Current = Cursors.Default; // Normal

                txtClient.Text = clsSearch.ClassClientName =  frmReportViewer.sClientName = clsSearch.ClassParticularName;                

                sHeader = clsSearch.ClassParticularName + " ERM BILLING";
                
                lblHeader.Text = "SEARCH " + "[ " + sHeader + " ]";
            }
        }

        private bool isValidHeader()
        {
            int i = 0;
            string sList = "";
            string sMapFrom = "";
            bool fExist = true;

            dbAPI.ExecuteAPI("GET", "View", "Type", "ERM", "Mapping", "", "ViewMapping");

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

                //dbFunction.ClearTextBox(this);
                //dbFunction.ClearComboBox(this);
                //dbFunction.ClearDataGrid(grdTempImport);                
                btnLoadFile.Enabled = true;

                return false;
            }

            return true;
        }

        private void InitHeader()
        {
            int iColCount = 0;
            int i = 0;
            int y = 0;

            iColCount = grdTempImport.ColumnCount;

            // Add Column and Set Header            
            grdList.ColumnCount = iColCount;
            for (i = 0; i < iColCount; i++)
            {
                string cellParam = grdTempImport.Rows[iHeaderRowIndex].Cells[i].Value.ToString();
                cellParam = cellParam.Replace("\n", " ").Trim();

                if (i > 0)
                    grdList.Columns[i].Width = 130;
                else
                    grdList.Columns[i].Width = 60;

                grdList.Columns[i].Name = cellParam;

                y = i + 1;
                Debug.WriteLine(y.ToString() + "," + cellParam + "," + cellParam + "," + "0" + "," + i.ToString() + "," + "ERM");

            }
        }

        private void LoadType()
        {
            int i = 0;

            lvwList.Items.Clear();

            clsSearch.ClassSearchBy = clsGlobalVariables.APPS_TYPE.ToString() + clsFunction.sPipe;
            dbAPI.ExecuteAPI("GET", "View", "Type", clsSearch.ClassSearchBy, "Type", "", "ViewType");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (clsType.RecordFound)
            {
                while (clsArray.TypeID.Length > i)
                {                    
                    clsType.TypeID = int.Parse(clsArray.TypeID[i].ToString());
                    clsType.ClassDescription = clsArray.TypeDescription[i];
                    clsType.ClassQueryString = clsArray.TypeQueryString[i];

                    i++;

                    AddItem(i);
                }
            }

            dbFunction.ListViewAlternateBackColor(lvwList);

        }
        private void AddItem(int inLineNo)
        {
            // Add to List            
            ListViewItem item = new ListViewItem(inLineNo.ToString());
            item.SubItems.Add(clsType.ClassTypeID.ToString());
            item.SubItems.Add(clsType.ClassDescription.ToString());
            item.SubItems.Add(clsType.ClassQueryString.ToString());
            lvwList.Items.Add(item);
        }

        private void ProcessReport()
        {
            clsSearch.ClassBillable = (chkBillable.Checked ? true : false);
            clsSearch.isMDRBreakdown = (chkMDRBreakdown.Checked ? true : false);
            clsSearch.ClassReportID = 6;
            clsSearch.ClassStatementType = "View";
            clsSearch.ClassStoredProcedureName = "spViewERM";
            clsSearch.ClassDateFrom = sDateFrom;
            clsSearch.ClassDateTo = sDateTo;
            clsSearch.ClassMDRType = cboSearchMDRType.Text;
            clsSearch.ClassDateFromTo = txtFilterDate.Text;

            if (chkMDRBreakdown.Checked)
            {
                if (!dbFunction.isValidComboBoxValue(cboSearchMDRType.Text))
                {
                    dbFunction.SetMessageBox("Please choose a value for MDR type.", "Check Field", clsFunction.IconType.iExclamation);
                    return;
                }
            }

            if (isCheckReportOption())
            {
                if (lvwList.Items.Count > 0)
                {

                    MessageBox.Show("Report type [Parent] will be generated.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

                    clsSearch.ClassTCount = clsFunction.sZero;
                    clsSearch.ClassTAmount = clsFunction.sZero;
                    clsSearch.ClassMDR0 = clsSearch.ClassMDR1 = clsSearch.ClassMDR2 = clsSearch.ClassMDR3 = clsSearch.ClassMDR4 = clsSearch.ClassMDR5 = clsFunction.sZero;               

                    GetERMTotal(); // Get Total Txn Count/Amount
                    ComputeMDR(); // Compute MDR

                    Debug.WriteLine("Report Type: Parent Count=" + clsSearch.ClassTCount);
                    if (int.Parse(clsSearch.ClassTCount.Replace(clsFunction.sComma, clsFunction.sNull)) > 0)
                    {
                        clsSearch.ClassStatementType = "View";
                        clsSearch.ClassSearchBy = "ERM Temp Detail Report";
                        clsSearch.ClassSearchValue = clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + (clsSearch.ClassBillable ? "1" : "0");
                        clsSearch.ClassReportType = clsReport.ClassReportDesc = "MAIN REPORT" + " " + txtFilterDate.Text.Substring(txtFilterDate.TextLength - 8);
                      
                        dbFunction.ProcessReport(clsSearch.ClassReportID);

                        foreach (ListViewItem i in lvwList.Items)
                        {
                            if (i.Checked)
                            {
                                string sID = i.SubItems[1].Text;
                                string sDescription = i.SubItems[2].Text;
                                string sQueryString = i.SubItems[3].Text;

                                Debug.WriteLine("i=" + i + ",sID=" + sID + ",sDescription=" + sDescription + ",sQueryString=" + sQueryString);

                                MessageBox.Show("Report type [" + sDescription + "] will be generated." + "\n\n" + "Query " + sQueryString, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

                                clsSearch.ClassTCount = clsFunction.sZero;
                                clsSearch.ClassTAmount = clsFunction.sZero;
                                clsSearch.ClassTRecurring = clsFunction.sDash;
                                GetERMTotalByType(sQueryString); // Get Total Txn Count/Amount

                                Debug.WriteLine("Report Type: " + sDescription + " Count =" + clsSearch.ClassTCount);
                                if (int.Parse(clsSearch.ClassTCount.Replace(clsFunction.sComma, clsFunction.sNull)) > 0)
                                {
                                    clsSearch.ClassStatementType = "View";
                                    clsSearch.ClassSearchBy = "ERM Temp Detail By Type";
                                    clsSearch.ClassSearchValue = clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + (clsSearch.ClassBillable ? "1" : "0") + clsFunction.sPipe + sQueryString;
                                    clsSearch.ClassReportType = clsReport.ClassReportDesc = sDescription + " " + txtFilterDate.Text.Substring(txtFilterDate.TextLength - 8);

                                    dbFunction.ProcessReport(clsSearch.ClassReportID);
                                }
                                else
                                {
                                    MessageBox.Show("No record found for report type [" + sDescription + "].", "Info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                                }                                
                            }
                        }

                    }
                    else
                    {
                        MessageBox.Show("No record found for report type [Parent].", "Info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    }                    
                }
                else
                {
                    MessageBox.Show("Please check report option to generate.", "Report option", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
            }
            else
            {
                if (MessageBox.Show("Report option is not check. \nNo filter of report to be generated." +
                                    "\n\n",
                                    "Continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    return;
                }
                            
                clsSearch.ClassMDR0 = clsSearch.ClassMDR1 = clsSearch.ClassMDR2 = clsSearch.ClassMDR3 = clsSearch.ClassMDR4 = clsSearch.ClassMDR5 = clsFunction.sZero;
                ComputeMDR();

                clsSearch.ClassReportType = clsReport.ClassReportDesc = "MAIN REPORT" + " " + txtFilterDate.Text.Substring(txtFilterDate.TextLength - 8);
                clsSearch.ClassSearchBy = "ERM Temp Detail Report";
                clsSearch.ClassSearchValue = clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + (clsSearch.ClassBillable ? "1" : "0");

                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
        }

        private void GetERMTotalByType(string pSearchQuery)
        {
            int iCount = 0;
            double iTotal = 0;

            iCount = 0;            
            dbAPI.GetViewCount("Search", "ERM Temp Detail Total Trxn Count By Type", pSearchQuery, "Get Count");
            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

            clsSearch.ClassTCount = (iCount > 0 ? iCount.ToString("N0") : "0");
            
            if (iCount > 0)
            {
                iTotal = 0;
                dbAPI.GetViewTotal("Search", "ERM Temp Detail Total Trxn Amt By Type", pSearchQuery, "Get Total");
                if (dbAPI.isNoRecordFound() == false)
                    iTotal = clsTerminal.ClassTerminalTotal;
            }
            
            clsSearch.ClassTAmount = (iTotal > 0 ? iTotal.ToString("N") : "0.00");
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {

            if (btnCheck.Text.Equals("CHECK ALL"))
            {
                btnCheck.Text = "UNCHECK ALL";

                // Check All
                foreach (ListViewItem i in lvwList.Items)
                {
                    i.Checked = true;
                }                    
            }
            else
            {
                btnCheck.Text = "CHECK ALL";

                // UnCheck All
                foreach (ListViewItem i in lvwList.Items)
                {
                    i.Checked = false;
                }
            }

        }

        private bool isCheckReportOption()
        {
            bool fCheck = false;

            foreach (ListViewItem i in lvwList.Items)
            {
                if (i.Checked)
                {
                    fCheck = true;
                    break;
                }
            }

            return fCheck;
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void chkMDRBreakdown_CheckedChanged(object sender, EventArgs e)
        {
            cboSearchMDRType.Enabled = false;
            if (chkMDRBreakdown.Checked)
            {
                cboSearchMDRType.Enabled = true;          
            }
            chkMDRBreakdown.Text = clsFunction.sDefaultSelect;
        }

        private void ComputeMDR()
        {       
            double iTotal = 0;          
            double iTMDR = 0;

            if (!cboSearchMDRType.Text.Equals(clsFunction.sDefaultSelect))
            {
                // -------------------------------------------------------------------
                // MDR Type A - 3.5% Computation
                // -------------------------------------------------------------------
                // MDR0 (TOTAL AMOUNT:)
                iTotal = double.Parse(clsSearch.ClassTAmount);
                clsSearch.ClassMDR0 = (iTotal > 0 ? iTotal.ToString("N") : "0.00");

                // MDR1 (MDR(3.5%):)
                if (cboSearchMDRType.Text.Equals(dbAPI.GetMDRType()[1]))
                    iTMDR = iTotal * 0.035;

                if (cboSearchMDRType.Text.Equals(dbAPI.GetMDRType()[2]))
                    iTMDR = iTotal * 0.040;

                clsSearch.ClassMDR1 = (iTMDR > 0 ? iTMDR.ToString("N") : "0.00");

                // MDR2 (MCC(2.2%):)
                iTMDR = iTotal * 0.022;
                clsSearch.ClassMDR2 = (iTMDR > 0 ? iTMDR.ToString("N") : "0.00");

                // MDR3 (WEEPAYxCITAS(1.3%):)
                iTMDR = double.Parse(clsSearch.ClassMDR1) - double.Parse(clsSearch.ClassMDR2);
                clsSearch.ClassMDR3 = (iTMDR > 0 ? iTMDR.ToString("N") : "0.00");

                // MDR4 (WEEPAY SHARE(0.75%):)
                iTMDR = double.Parse(clsSearch.ClassMDR3) * 0.75;
                clsSearch.ClassMDR4 = (iTMDR > 0 ? iTMDR.ToString("N") : "0.00");

                // MDR5 (CITAS SHARE(0.25%):)
                iTMDR = double.Parse(clsSearch.ClassMDR3) * 0.25;
                clsSearch.ClassMDR5 = (iTMDR > 0 ? iTMDR.ToString("N") : "0.00");
                // -------------------------------------------------------------------
                // MDR Computation
                // -------------------------------------------------------------------
            }
        }

        private void GetDateRange()
        {
            clsSearch.ClassSearchValue = clsUser.ClassParticularID + clsFunction.sPipe +
                                                clsUser.ClassModifiedDate;

            dbAPI.ExecuteAPI("GET", "Search", "ERM Date Range", clsSearch.ClassSearchValue, "Get Info Detail", "", "GetInfoDetail");

            if (dbAPI.isNoRecordFound() == false)
            {
                clsSearch.ClassDateFrom = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                clsSearch.ClassDateTo = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
            }
        }

        private void notifyEmail()
        {
            Cursor.Current = Cursors.WaitCursor;

            var tagValueMap = new Dictionary<string, string>
            {
                { clsDefines.TAG_DateRange, txtFilterDate.Text },
                { clsDefines.TAG_TCount, txtTTrxnCount.Text },
                { clsDefines.TAG_TAmount, txtTTrxnAmt.Text },
                { clsDefines.TAG_TRecurring, txtTRecurring.Text },
                { clsDefines.TAG_ProcessedDate, dbFunction.getCurrentDate() },
                { clsDefines.TAG_ProcessedTime, dbFunction.getCurrentTime() },
                { clsDefines.TAG_ProcessedBy, clsUser.ClassUserFullName }
            };

            string jsonString = dbFunction.convertToJson(tagValueMap);
            Debug.WriteLine(jsonString);

            dbFunction.parseDelimitedString(jsonString, clsDefines.gComma, 1);

            dbAPI.ExecuteAPI("POST", "Notify", "BDO ERM Billing", jsonString, "Email Notification", "", "EmailNotification");

            Cursor.Current = Cursors.Default;
        }

    }
}
