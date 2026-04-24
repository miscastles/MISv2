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

namespace MIS
{
    public partial class frmImportDTR : Form
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
        private string sHeaderCSV = "";
        private int iImportLimit = 10;
        public static string sHeader;
        public frmImportDTR()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dbFunction.ClearTextBox(this);
            dbFunction.ClearListView(lvwSearch);

            
            btnPreview.Enabled = false;
        }

        private void frmDTR_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbSetting = new clsINI();
            dbFile = new clsFile();
            dbFunction = new clsFunction();
            dbInternet = new clsInternet();
            dbSetting.InitDatabaseSetting();
            
            dbFunction.ClearTextBox(this);
                        
            btnPreview.Enabled = false;
            btnSave.Enabled = false;
            
            lblHeader.Text = lblHeader.Text + " " + "[ " + sHeader + " ]";

            InitTime();
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            string sExtension = "";
            int iIndexCount = 0;

            OpenFileDialog ExcelDialog = new OpenFileDialog();
            ExcelDialog.Filter = "EXCEL Files|*.csv;*.xls;*.xlsx";
            ExcelDialog.InitialDirectory = @"C:\CASTLESTECH_MIS\IMPORT\";
            ExcelDialog.Title = "Select FSR File";

            if (ExcelDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dbFunction.ClearListView(lvwSearch);
                
                ExcelFilePath = ExcelDialog.FileName;
                txtPathFileName.Text = ExcelDialog.FileName;
                txtPathFileName.ReadOnly = true;
                btnLoadFile.Enabled = false;
                sExcelFileName = System.IO.Path.GetFileName(txtPathFileName.Text);
                txtFileName.Text = sExcelFileName;
                sExtension = Path.GetExtension(txtPathFileName.Text);
                sExtension = sExtension.Replace(clsFunction.sPeriod, clsFunction.sNull).ToUpper();

                try
                {
                    //dbFunction.SetMessageBox("Import and Processing Import. This will take a few minute(s).",  "Import", clsFunction.IconType.iInformation);


                    if (MessageBox.Show("This will take a few minute(s)." +
                                                           "\n" +
                                                           "\n" +
                                                           "Do want to continue import file?" +
                                                           "\n\n",
                                                           "Import DTR File", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                    {
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

                    switch (sExtension)
                    {
                        case "XLSX":
                        case "XLS":
                            sSheet = GetExcelWorkSheetName();
                            txtSheetName.Text = "`" + sSheet + "$" + "`";
                            txtFileName.Text = sExcelFileName;

                            sReqTime = dbFunction.GetRequestTime("DTR Import Detail");

                            // Create Temporary database            
                            Debug.WriteLine("Temporary Table=" + txtSheetName.Text);
                            dbAPI.ExecuteAPI("POST", "Create", "", "", txtSheetName.Text, "", "CreateTempTable");

                            Debug.WriteLine("=>>ImportToDummyDataGrid");
                            ImportToDummyDataGrid();
                           
                            Debug.WriteLine("=>>SetListViewData");
                            SetListViewData();
                            
                            dbFunction.GetResponseTime("DTR Import Detail");

                            // Process ERMTempDetail
                            //Debug.WriteLine("API Call ProcessERM");
                            //dbAPI.ProcessERMTempDetail(clsSearch.ClassParticularID.ToString(), clsSearch.ClassParticularName);

                            Cursor.Current = Cursors.Default; // Back to normal 

                            //dbFunction.SetMessageBox("Retrieving process data. This will take a few minute(s).", "Retrieve", clsFunction.IconType.iInformation);

                            // Start Import Here
                            Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                            dbAPI.ResetAdvanceSearch();
                            
                            sResTime = dbFunction.GetResponseTime("DTR Import Detail");

                            watch.Stop();
                            elapseMS = (watch.ElapsedMilliseconds / 1000).ToString() + " Second(s)";

                            Debug.WriteLine("=>>ImportDTRDetail=" + sFileName + " Complete!!!" + " RequestTime=" + sReqTime + "|ResponseTime=" + sResTime + "|ExecutionTime=" + elapseMS);
                            dbFunction.SetMessageBox("Import successfully completed." + "\n\nStart Time: " + sReqTime + "\n" + "End Time: " + sResTime + "\n" + "Executiion Time: " + elapseMS, "XLSX: Complete", clsFunction.IconType.iInformation);

                            break;
                        
                    }


                    Debug.WriteLine("Total execution time: " + elapseMS + " Seconds");

                    //clsFunction.WaitWindow(false, frmWait); // Close Wait Window                    

                    Cursor.Current = Cursors.Default; // Back to normal 

                    btnLoadFile.Enabled = true;

                    if (grdTempImport.RowCount > 0)
                    {
                        btnSave.Enabled = true;
                        btnPreview.Enabled = false;
                    }
                        

                }
                catch (Exception ex)
                {
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
                sWorkSheetName = "Timelogs";

            return sWorkSheetName;
        }

        private void UploadFile(string sFileName)
        {

            Debug.WriteLine("--UploadFile--");
            Debug.WriteLine("sFileName=" + sFileName);
            Debug.WriteLine("clsGlobalVariables.strFTPURL=" + clsGlobalVariables.strFTPURL);
            Debug.WriteLine("clsGlobalVariables.strFTPUserName=" + clsGlobalVariables.strFTPUserName);
            Debug.WriteLine("clsGlobalVariables.strFTPPassword=" + clsGlobalVariables.strFTPPassword);
            Debug.WriteLine("clsGlobalVariables.strFTPLocalPath=" + clsGlobalVariables.strFTPLocalPath);
            Debug.WriteLine("clsGlobalVariables.strFTPUploadPath=" + clsGlobalVariables.strFTPUploadPath);

            // Upload File to FTP                
            ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
            ftpClient.delete(clsGlobalVariables.strFTPUploadPath + sFileName);
            ftpClient.upload(clsGlobalVariables.strFTPUploadPath + sFileName, @clsGlobalVariables.strFTPLocalPath + sFileName);
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
                dbFunction.SetMessageBox("Office Version: " +
                                         clsSystemSetting.ClassSystemMSOffice + "\n\n" +
                                         ex.ToString(), ex.Message, clsFunction.IconType.iError);
            }
        }

        private void SetListViewData()
        {
            int iDataRowIndex = 5;
            int iRowCount = grdTempImport.RowCount;
            int iColCount = grdTempImport.ColumnCount;
            int iRowIndex = 0;
            int iLineNo = 0;
            int index = 0;
            List<string> TempArrayDataCol = new List<String>();

            Debug.WriteLine("iRowCount="+ iRowCount);
            Debug.WriteLine("iColCount=" + iColCount);

            // Get Start Date
            txtStartDate.Text = grdTempImport.Rows[0].Cells[1].Value.ToString(); 
            
            // Get End Data
            txtEndDate.Text = grdTempImport.Rows[1].Cells[1].Value.ToString();

            TempArrayDataCol.Clear();
            
            for (int i = iDataRowIndex; i < iRowCount; i++)
            {
                string sCellValue = grdTempImport.Rows[i].Cells[0].Value.ToString();
                
                if (sCellValue.Length > 0)
                {
                    string sTemp = sCellValue.Substring(0, 2);

                    Debug.WriteLine("iRow=" + i + ",sCellValue=" + sCellValue);

                    // Valid Row
                    if (sTemp.Length > 0)
                    {
                        dbFunction.ListViewChangeBackColorByRow(lvwSearch, iLineNo, Color.White);

                        iLineNo++;

                        if (!dbFunction.isNumeric(sTemp))
                        {
                            // Name
                            clsDTR.ClassParticularID = 0;
                            clsDTR.ClassName = sCellValue;
                            clsDTR.ClassDate = "-";
                            clsDTR.ClassCheckIn = "-";
                            clsDTR.ClassCheckOut = "-";
                            clsDTR.ClassTCheckHrs = "-";
                            clsDTR.ClassOvertimeIn = "-";
                            clsDTR.ClassOvertimeOut = "-";
                            clsDTR.ClassTOverTimeHrs = "-";
                            clsDTR.ClassFullName = clsDTR.ClassName;
                            clsDTR.ClassRemarks = "-";
                            clsDTR.ClassIsName = true;

                            int at = 0;
                            int length = clsDTR.ClassName.Length;
                            at = clsDTR.ClassName.IndexOf('(');

                            if (at <= 0)
                            {
                                dbFunction.SetMessageBox("Invalid IDNo. Name " + clsDTR.ClassName, "Invalid IDNo", clsFunction.IconType.iError);
                                return;
                            }

                            string sIDNo = clsDTR.ClassName.Substring(at);                            
                            clsDTR.ClassIDNo = sIDNo.Replace("(", "").Replace(")","");
                            Debug.WriteLine("clsDTR.ClassIDNo=" + clsDTR.ClassIDNo);

                            // Get Particular ID                            
                            int ParticularID = dbAPI.GetIDNo("IDNo", clsDTR.ClassIDNo);
                            clsDTR.ClassParticularID = ParticularID;
                            Debug.WriteLine("clsDTR.ClassParticularID=" + clsDTR.ClassParticularID);

                            if (clsDTR.ClassParticularID <= 0)
                            {
                                dbFunction.SetMessageBox("IDNo not found for name " + clsDTR.ClassName, "IDNo Not Found", clsFunction.IconType.iError);
                                return;
                            }

                        }
                        else
                        {                           
                            // Date / Time                             
                            clsDTR.ClassName = "-";
                            clsDTR.ClassDate = sCellValue;
                            clsDTR.ClassCheckIn = CheckTimeValue(grdTempImport.Rows[i].Cells[1].Value.ToString());
                            clsDTR.ClassCheckOut = CheckTimeValue(grdTempImport.Rows[i].Cells[2].Value.ToString());
                            clsDTR.ClassTCheckHrs = "-";
                            clsDTR.ClassOvertimeIn = CheckTimeValue(grdTempImport.Rows[i].Cells[3].Value.ToString());
                            clsDTR.ClassOvertimeOut = CheckTimeValue(grdTempImport.Rows[i].Cells[4].Value.ToString());
                            clsDTR.ClassTOverTimeHrs = "-";
                            clsDTR.ClassIsName = false;

                            if (isDateWeekEnd(clsDTR.ClassDate))
                            {
                                clsDTR.ClassRemarks = "It's weekend!";
                                dbFunction.ListViewChangeBackColorByRow(lvwSearch, iLineNo, Color.Red);
                            }
                            else
                            {
                                if (clsDTR.ClassCheckIn.CompareTo("00:00") == 0 && (clsDTR.ClassCheckOut.CompareTo("00:00") == 0))
                                {
                                    clsDTR.ClassRemarks = "No check-in/out";
                                }
                                else
                                {
                                    bool fError = false;
                                    if (clsDTR.ClassCheckIn.CompareTo("00:00") != 0 && clsDTR.ClassCheckOut.CompareTo("00:00") == 0)
                                    {
                                        clsDTR.ClassRemarks = "No check out";
                                        fError = true;
                                    }

                                    if (clsDTR.ClassCheckIn.CompareTo("00:00") == 0 && clsDTR.ClassCheckOut.CompareTo("00:00") != 0)
                                    {
                                        clsDTR.ClassRemarks = "No check in";
                                        fError = true;
                                    }

                                    // Compute CheckIn/Out Difference
                                    if (!fError)
                                    {
                                        DateTime dteCheckIn = DateTime.Parse(clsDTR.ClassCheckIn);
                                        DateTime dteCheckOut = DateTime.Parse(clsDTR.ClassCheckOut);

                                        TimeSpan span = (dteCheckOut - dteCheckIn);
                                        Debug.WriteLine("Check Span=" + span);

                                        clsDTR.ClassTCheckHrs = span.Hours + "h" + " " + span.Minutes.ToString() + "m";
                                        clsDTR.ClassRemarks = "-";

                                    }

                                }
                            }                            
                        }

                        AddItem(iLineNo);

                        dbFunction.AppDoEvents(true);
                    }
                }
                
            }
            
        }
        
        private void AddItem(int inLineNo)
        {
            // Add to List            
            ListViewItem item = new ListViewItem(inLineNo.ToString());
            item.SubItems.Add(clsDTR.ClassParticularID.ToString());

            item.BackColor = clsFunction.AlternateBackColor2;
            item.SubItems.Add(clsDTR.ClassName);
            item.BackColor = clsFunction.EntryBackColor;

            item.SubItems.Add(clsDTR.ClassDate);
            item.SubItems.Add(clsDTR.ClassCheckIn);
            item.SubItems.Add(clsDTR.ClassCheckOut);
            item.SubItems.Add(clsDTR.ClassTCheckHrs);
            item.SubItems.Add(clsDTR.ClassOvertimeIn);
            item.SubItems.Add(clsDTR.ClassOvertimeOut);
            item.SubItems.Add(clsDTR.ClassTOverTimeHrs);
            item.SubItems.Add(clsDTR.ClassFullName);
            item.SubItems.Add(clsDTR.ClassRemarks);
            item.SubItems.Add((clsDTR._isName ? 1 : 0).ToString());

            lvwSearch.Items.Add(item);
        }

        private string CheckTimeValue(string sTime)
        {
            string sTemp = "";

            if (sTime.Length > 0)
                sTemp = sTime;
            else
                sTemp = "00:00";

            return sTemp;
        }

        private void lvwSearch_DoubleClick(object sender, EventArgs e)
        {
            
        }

        private void lvwSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwSearch.SelectedItems.Count > 0)
            {
                string LineNo = lvwSearch.SelectedItems[0].Text;
                txtLineNo.Text = LineNo;

                if (LineNo.Length > 0)
                {
                    clsSearch.ClassParticularID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                    clsSearch.ClassName = lvwSearch.SelectedItems[0].SubItems[2].Text;
                    clsSearch.ClassDate = lvwSearch.SelectedItems[0].SubItems[3].Text;
                    clsSearch.ClassCheckIn = lvwSearch.SelectedItems[0].SubItems[4].Text;
                    clsSearch.ClassCheckOut = lvwSearch.SelectedItems[0].SubItems[5].Text;
                    clsSearch.ClassTCheckHrs = lvwSearch.SelectedItems[0].SubItems[6].Text;
                    clsSearch.ClassOvertimeIn = lvwSearch.SelectedItems[0].SubItems[7].Text;
                    clsSearch.ClassOvertimeOut = lvwSearch.SelectedItems[0].SubItems[8].Text;
                    clsSearch.ClassTOverTimeHrs = lvwSearch.SelectedItems[0].SubItems[9].Text;
                    clsSearch.ClassFullName = lvwSearch.SelectedItems[0].SubItems[10].Text;
                    clsSearch.ClassRemarks = lvwSearch.SelectedItems[0].SubItems[11].Text;
                    
                    int isName = int.Parse(lvwSearch.SelectedItems[0].SubItems[12].Text);
                    clsSearch.ClassIsName =  (isName > 0 ? true : false);

                    if (!clsSearch.ClassIsName)
                        ListDetails();

                }
            }
        }

        private void ListDetails()
        {
            DateTime dateTime = DateTime.Parse("00:00:00");

            txtSelLineNo.Text = txtLineNo.Text;
            txtName.Text = clsSearch.ClassFullName;
            txtDate.Text = clsSearch.ClassDate;
            txtCheckIn.Text = clsSearch.ClassCheckIn;
            txtCheckOut.Text = clsSearch.ClassCheckOut;
            txtOTIn.Text = clsSearch.ClassOvertimeIn;
            txtOTOut.Text = clsSearch.ClassOvertimeOut;

            // Set Time
            dateTime = DateTime.Parse(clsSearch.ClassCheckIn);
            dteCheckIn.Value = dateTime;

            dateTime = DateTime.Parse(clsSearch.ClassCheckOut);
            dteCheckOut.Value = dateTime;

            dateTime = DateTime.Parse(clsSearch.ClassOvertimeIn);
            dteOTIn.Value = dateTime;

            dateTime = DateTime.Parse(clsSearch.ClassOvertimeOut);
            dteOTOut.Value = dateTime;
        }

        private void InitTime()
        {
            DateTime dateTime = DateTime.Parse("00:00:00");

            dteCheckIn.Value = dateTime;
            dteCheckIn.CustomFormat = "hh:mm tt";
            dteCheckIn.Format = DateTimePickerFormat.Custom;

            dteCheckOut.Value = dateTime;
            dteCheckOut.CustomFormat = "hh:mm tt";
            dteCheckOut.Format = DateTimePickerFormat.Custom;

            dteOTIn.Value = dateTime;
            dteOTIn.CustomFormat = "hh:mm tt";
            dteOTIn.Format = DateTimePickerFormat.Custom;

            dteOTOut.Value = dateTime;
            dteOTOut.CustomFormat = "hh:mm tt";
            dteOTOut.Format = DateTimePickerFormat.Custom;
            
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            InitTime();
        }

        private void btnClearCheckIn_Click(object sender, EventArgs e)
        {
            ResetTime(1);
        }

        private void ResetTime(int index)
        {
            DateTime dateTime = DateTime.Parse("00:00:00");
            switch (index)
            {
                case 1:
                    dteCheckIn.Value = dateTime;
                    dteCheckIn.CustomFormat = "hh:mm tt";
                    dteCheckIn.Format = DateTimePickerFormat.Custom;
                    break;
                case 2:
                    dteCheckOut.Value = dateTime;
                    dteCheckOut.CustomFormat = "hh:mm tt";
                    dteCheckOut.Format = DateTimePickerFormat.Custom;
                    break;
                case 3:
                    dteOTIn.Value = dateTime;
                    dteOTIn.CustomFormat = "hh:mm tt";
                    dteOTIn.Format = DateTimePickerFormat.Custom;
                    break;
                case 4:
                    dteOTOut.Value = dateTime;
                    dteOTOut.CustomFormat = "hh:mm tt";
                    dteOTOut.Format = DateTimePickerFormat.Custom;
                    break;
            }

        }

        private void btnClearCheckOut_Click(object sender, EventArgs e)
        {
            ResetTime(2);
        }

        private void btnClearOTIn_Click(object sender, EventArgs e)
        {
            ResetTime(3);
        }

        private void btnClearOTOut_Click(object sender, EventArgs e)
        {
            ResetTime(4);
        }

        private bool isValidCheckInAndOutTime(string pDate, string pTimeIn, string pTimeOut, ref string outRemarks)
        {
            bool isValid = false;
            DateTime dteDate;

            Debug.WriteLine("--isValidCheckInAndOutTime--");
            Debug.WriteLine("pDate="+ pDate+ ",pTimeIn=" + pTimeIn + ",pTimeOut=" + pTimeOut);

            if (pDate.Length > 0)
            {
                string sDate = pDate.Substring(0, 8);
                dteDate = DateTime.Parse(sDate);
                Debug.WriteLine("dteDate=" + dteDate);

                // Check if weekend (Saturday/Sunday)
                if (dteDate.DayOfWeek == DayOfWeek.Saturday || dteDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    
                }


            }
            else
            {
                outRemarks = "Invalid date";
                return false;
            }

            return isValid;
        }

        private bool isDateWeekEnd(string pDate)
        {
            bool isWeekEnd = false;
            DateTime dteDate;

            string sDayOfWeek = pDate.Substring(9, 2).ToUpper();

            if (sDayOfWeek.CompareTo("SA") == 0 || (sDayOfWeek.CompareTo("SU") == 0))
                isWeekEnd = true;

            return isWeekEnd;
        }
        
        private bool isValidTime(string pTime)
        {
            bool isValid = false;
            DateTime dteTime;

            if (pTime.Length > 0)
            {                
                try
                {
                    dteTime = DateTime.Parse(pTime);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return isValid;
        }

        private bool isValidOverTimeInAndOutTime(string pDate, string pTimeIn, string pTimeOut, ref string outRemarks)
        {
            bool isValid = false;
            DateTime dteDate;

            Debug.WriteLine("--isValidOverTimeInAndOutTime--");
            Debug.WriteLine("pDate=" + pDate + ",pTimeIn=" + pTimeIn + ",pTimeOut=" + pTimeOut);

            if (pDate.Length > 0)
            {
                dteDate = DateTime.Parse(pDate);
                Debug.WriteLine("dteDate=" + dteDate);
            }

            return isValid;
        }

        public static bool isValidDate(string pDate)
        {
            bool isValid = false;
            DateTime tempDate;

            string sDate = pDate.Substring(0, 8);

            try
            {
                tempDate = DateTime.Parse(sDate);
                isValid = true;
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }

        private void WriteCSVFileFromListView(string sFileName, ref int iIndexCount)
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

            Debug.WriteLine("Writing as CSV File=" + sFileName);
            iDataRowIndex = iHeaderRowIndex + 1;
            iColumnMaxCount = 10;
            
            StringBuilder columnbind = new StringBuilder();
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
                            Debug.WriteLine("x=" + x.ToString() + "," + "Data=" + clsArray.sRepoData[x].ToString());

                            columnbind.Append(clsArray.sRepoData[x].ToString());

                            if (x < iEndIndex - 1)
                                columnbind.Append("\r\n");
                        }

                        // Write To File
                        iFileNameIndex++;
                        string sNewFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iDTR, iFileNameIndex);
                        Debug.WriteLine("sNewFileName="+ sNewFileName);
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
                    Debug.WriteLine("x=" + x.ToString() + "," + "Data=" + clsArray.sRepoData[x].ToString());

                    columnbind.Append(clsArray.sRepoData[x].ToString());

                    if (x < iTempEndIndex - 1)
                        columnbind.Append("\r\n");
                }

                // Write To File
                iFileNameIndex++;
                string sNewFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iDTR, iFileNameIndex);
                dbFile.DeleteCSV(sNewFileName);

                dbFile.WriteCSV(sNewFileName, columnbind.ToString());
            }

            iIndexCount = iFileNameIndex;
        }

        private void UpdateDTR(int pLineNo, string pCheckIn, string pCheckOut, string pOverTimeIn, string pOverTimeOut)
        {

            foreach (ListViewItem i in lvwSearch.Items)
            {
                int iLineNo = int.Parse(i.SubItems[0].Text);

                if (pLineNo == iLineNo)
                {
                    
                    i.SubItems[4].Text = pCheckIn; // CheckIn
                    i.SubItems[5].Text = pCheckOut; // CheckOut
                    i.SubItems[6].Text = "-"; // TCheckHrs

                    i.SubItems[7].Text = pOverTimeIn; // OverTimeIn
                    i.SubItems[8].Text = pOverTimeOut; // OverTimeOut
                    i.SubItems[9].Text = "-"; // TOverTimeHrs

                    break;
                }
            }
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            int iLineNo = int.Parse(txtSelLineNo.Text);
            string sCheckIn = CheckTime(dteCheckIn.Value.ToString("hh:mm tt"));
            string sCheckOut = CheckTime(dteCheckOut.Value.ToString("hh:mm tt"));
            string sOTIn = CheckTime(dteOTIn.Value.ToString("hh:mm tt"));
            string sOTOut = CheckTime(dteOTOut.Value.ToString("hh:mm tt"));

            // Check CheckIn/Out
            if (!ValidTime(sCheckIn, sCheckOut))
            {
                MessageBox.Show("[Check In] shouldn't be greater than [Check Out]" +
                                        "\n\n" +
                                        "Check-In:  " + sCheckIn +
                                        "\n" +
                                        "Check-Out: " + sCheckOut, "Check-In/Out time validation", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                return;
            }

            // Check OverTimeIn/Out
            if (!ValidTime(sOTIn, sOTOut))
            {
                MessageBox.Show("[OverTime In] shouldn't be greater than [OverTime Out]" +
                                        "\n\n" +
                                        "OverTime-In:  " + sOTIn +
                                        "\n" +
                                        "OverTime-Out: " + sOTOut, "OverTime-In/Out time validation", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                return;
            }


            UpdateDTR(iLineNo, sCheckIn, sCheckOut, sOTIn, sOTOut);

            // CheckIn/Out
            if (ValidDTR(sCheckIn, sCheckOut))
                UpdateTotalHour(iLineNo, 1, sCheckIn, sCheckOut);

            // OverTimeIn/Out
            if (ValidDTR(sOTIn, sOTOut))
                UpdateTotalHour(iLineNo, 2, sOTIn, sOTOut);
        }

        private string CheckTime(string pTime)
        {
            string sTemp = "";

            if (pTime.CompareTo("12:00 AM") == 0 || pTime.CompareTo("12:00 PM") == 0)
                sTemp = "00:00";
            else
                sTemp = pTime;

            return sTemp;
        }

        private bool ValidTime(string pTimeIn, string pTimeOut)
        {
            DateTime dTimeIn;
            DateTime dTimeOut;
            bool isValid = false;

            dTimeIn = DateTime.Parse(pTimeIn);
            dTimeOut = DateTime.Parse(pTimeOut);

            if (dTimeIn > dTimeOut)            
                isValid = false;            
            else            
                isValid = true;            

            return isValid;
        }

        private void UpdateTotalHour(int pLineNo, int pType, string pTimeIn, string pTimeOut)
        {
            DateTime dteCheckIn = DateTime.Parse(pTimeIn);
            DateTime dteCheckOut = DateTime.Parse(pTimeOut);

            TimeSpan span = (dteCheckOut - dteCheckIn);
            Debug.WriteLine("Check Span=" + span);

            string sTHour = span.Hours + "h" + " " + span.Minutes.ToString() + "m";
            string sRemarks = clsFunction.sDash;

            foreach (ListViewItem i in lvwSearch.Items)
            {
                int iLineNo = int.Parse(i.SubItems[0].Text);

                if (pLineNo == iLineNo)
                {
                    switch (pType)
                    {
                        case 1:
                            i.SubItems[6].Text = sTHour; // TCheckHrs
                            break;
                        case 2:
                            i.SubItems[9].Text = sTHour; // TOverTimeHrs
                            break;
                    }

                    i.SubItems[11].Text = sRemarks; // TOverTimeHrs
                    break;
                }
            }
        }

        private bool ValidDTR(string pTimeIn, string pTimeOut)
        {
            bool isValid = false;

            if (pTimeIn.CompareTo("00:00") != 0 && pTimeOut.CompareTo("00:00") != 0)
                isValid = true;

            return isValid;
        }

        private void PopulateTempArray()
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            string format = "MM/dd/yy";

            List<string> TempArrayDataCol = new List<String>();
            int index = 0;
            DateTime dteDate = DateTime.Now;
            
            foreach (ListViewItem i in lvwSearch.Items)
            {
                bool isName = (int.Parse(i.SubItems[12].Text) > 0 ? true : false);

                if (!isName)
                {
                    clsDTR.ClassParticularID = int.Parse(i.SubItems[1].Text);
                    clsDTR.ClassName = i.SubItems[2].Text;
                    clsDTR.ClassDate = i.SubItems[3].Text;
                    clsDTR.ClassDTRDate = i.SubItems[3].Text;

                    string sTempDate = clsDTR.ClassDate.Substring(0, 8);
                    Debug.WriteLine("sTempDate="+ sTempDate);
                    
                    try
                    {
                        dteDate = DateTime.ParseExact(sTempDate, format, provider);
                        Console.WriteLine("{0} converts to {1}.", sTempDate, dteDate.ToString());
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("{0} is not in the correct format.", sTempDate);
                    }

                    clsDTR.ClassDate = dteDate.ToString("yyyy-MM-dd");
                    clsDTR.ClassWeekDay = dteDate.DayOfWeek.ToString();

                    clsDTR.ClassCheckIn = i.SubItems[4].Text;
                    clsDTR.ClassCheckOut = i.SubItems[5].Text;
                    clsDTR.ClassTCheckHrs = i.SubItems[6].Text;
                    clsDTR.ClassOvertimeIn = i.SubItems[7].Text;
                    clsDTR.ClassOvertimeOut = i.SubItems[8].Text;
                    clsDTR.ClassTOverTimeHrs = i.SubItems[9].Text;
                    clsDTR.ClassFullName = i.SubItems[10].Text;
                    clsDTR.ClassRemarks = i.SubItems[11].Text;

                    string sRowNo = (index + 1).ToString();
                    string sCellValue = sRowNo + clsFunction.sComma +
                         clsDTR.ClassParticularID + clsFunction.sComma +
                         clsDTR.ClassIDNo + clsFunction.sComma +
                         clsDTR.ClassFullName.Replace(",", clsFunction.sNull) + clsFunction.sComma +
                         clsDTR.ClassDate + clsFunction.sComma +
                         clsDTR.ClassDTRDate + clsFunction.sComma +
                         clsDTR.ClassWeekDay + clsFunction.sComma +
                         clsDTR.ClassCheckIn + clsFunction.sComma +
                         clsDTR.ClassCheckOut + clsFunction.sComma +
                         clsDTR.ClassTCheckHrs.Replace("-", clsFunction.sZero) + clsFunction.sComma +
                         clsDTR.ClassOvertimeIn + clsFunction.sComma +
                         clsDTR.ClassOvertimeOut + clsFunction.sComma +
                         clsDTR.ClassTOverTimeHrs.Replace("-", clsFunction.sZero) + clsFunction.sComma +
                         clsDTR.ClassRemarks;
                    Debug.WriteLine("index=" + index + ",sCellValue=" + sCellValue);

                    if (sCellValue.Length > 0)
                    {
                        TempArrayDataCol.Add(sCellValue);
                        index++;
                    }
                }
            }

            // Save to clsArray
            clsArray.sRepoData = TempArrayDataCol.ToArray();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var watch = Stopwatch.StartNew();
            string sFileName = "";
            var elapseMS = "";

            PopulateTempArray();

            if (clsArray.sRepoData.Length > 0)
            {                
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                btnPreview.Enabled = true;
                
                // Write CSV file
                int outIndexCount = 0;
                WriteCSVFileFromListView(sFileName, ref outIndexCount);

                // Delete DTRTempDetail                            
                dbAPI.ExecuteAPI("DELETE", "Delete", "", "", "DTR Temp Detail", "", "DeleteCollectionDetail");

                string sReqTime = dbFunction.GetRequestTime("DTR Import Detail");

                // Import File
                for (int i = 1; i <= outIndexCount; i++)
                {
                    string sImportFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iDTR, i);

                    // Upload File
                    Debug.WriteLine("=>>UploadFile=" + sImportFileName);
                    UploadFile(sImportFileName);

                    //Debug.WriteLine("=>>API Call ImportDTRDetail=" + sImportFileName);
                    dbAPI.ExecuteAPI("POST", "Import", "CSV", sImportFileName, "DTR Import Detail", "", "ImportDTRDetail");

                }
                
                dbAPI.ResetAdvanceSearch();

                string sResTime = dbFunction.GetRequestTime("DTR Import Detail");

                watch.Stop();
                elapseMS = (watch.ElapsedMilliseconds / 1000).ToString() + " Second(s)";

                Cursor.Current = Cursors.Default; // Default

                dbFunction.SetMessageBox("DTR save successfully completed." + "\n\nStart Time: " + sReqTime + "\n" + "End Time: " + sResTime + "\n" + "Executiion Time: " + elapseMS, "XLSX: Complete", clsFunction.IconType.iInformation);


            }
        }
    }
}
