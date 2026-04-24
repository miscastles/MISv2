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

namespace MIS
{
    public partial class frmImportFieldServiveReport : Form
    {
        private clsAPI dbAPI;
        private clsINI dbSetting;
        private clsFile dbFile;
        private clsFunction dbFunction;
        private static string ExcelFilePath = @"";
        private string sExcelFileName = "";        
        private string sDateFrom = "";
        private string sDateTo = "";

        List<string> ServiceNoCol = new List<String>();
        List<string> TAIDNoCol = new List<String>();
        List<string> IRIDNoCol = new List<String>();
        List<string> IRNoCol = new List<String>();

        private static clsAPI.JobType iSearchJobType;

        // The column we are currently using for sorting.
        private ColumnHeader SortingColumn = null;

        public frmImportFieldServiveReport()
        {
            InitializeComponent();
        }

        private void frmImportFieldServiveReport_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbSetting = new clsINI();
            dbFile = new clsFile();
            dbFunction = new clsFunction();
            dbSetting.InitDatabaseSetting();            
            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);
            dbFunction.ClearListView(lvwSearch);
            dbFunction.ClearListView(lvwServiceCode);
            dbFunction.ClearListView(lvwDetail);
            InitTextBox();

            btnPreviewSummary.Enabled = false;
            btnCheckServiceSummary.Enabled = false;
            btnPreviewSummary.Enabled = false;
            btnPreviewFSR.Enabled = false;
            btnSaveImport.Enabled = false;

            // Load Mapping
            dbAPI.ExecuteAPI("GET", "View", "Type", "FSR", "Mapping", "", "ViewMapping");
            InitListView();

            // Load Service Type
            dbAPI.ExecuteAPI("GET", "View", "Import FSR", "", "Service Type", "", "ViewServiceType");

            InitListViewServiceSummary();
        }
        
        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ExcelDialog = new OpenFileDialog();
            ExcelDialog.Filter = "CSV Files|*.csv";
            ExcelDialog.InitialDirectory = @"C:\CASTLESTECH_MIS\IMPORT\";
            ExcelDialog.Title = "Select FSR File";

            if (ExcelDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dbFunction.ClearListView(lvwSearch);
                dbFunction.ClearListView(lvwServiceSummary);
                dbFunction.ClearListView(lvwServiceCode);
                InitTextBox();

                btnPreviewSummary.Enabled = false;
                btnCheckServiceSummary.Enabled = false;
                btnPreviewSummary.Enabled = false;
                btnPreviewFSR.Enabled = false;
                btnSaveImport.Enabled = false;

                ExcelFilePath = ExcelDialog.FileName;
                txtPathFileName.Text = ExcelDialog.FileName;
                txtPathFileName.ReadOnly = true;
                btnLoadFile.Enabled = false;
                sExcelFileName = System.IO.Path.GetFileName(txtPathFileName.Text);
                txtFileName.Text = sExcelFileName;

                dbFunction.ClearComboBox(this);
                dbAPI.FillComboBoxClient(cboSearchClient);

                try
                {                    
                    dbFunction.SetMessageBox("Processing of FSR Import. This will take a few minute(s).", "Import", clsFunction.IconType.iInformation);

                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                    // Get Format FileName
                    string sFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iFSR, 0);

                    // Copy
                    dbFile.CopyFile(ExcelFilePath, clsGlobalVariables.strFTPLocalPath + sFileName);

                    // Upload File
                    UploadFile(sFileName);

                    // Process CSV File
                    dbAPI.ExecuteAPI("POST", "Import", "CSV", sFileName, "FSR Import Detail", "", "ImportFSRDetail");

                    Cursor.Current = Cursors.Default; // Back to normal 

                    dbFunction.SetMessageBox("Retrieving process data. This will take a few minute(s).", "Retrieve", clsFunction.IconType.iInformation);

                    // Start Import Here
                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                    LoadFSR();
                    InitImportDate();

                    Cursor.Current = Cursors.Default; // Back to normal 

                    //dbFunction.SetMessageBox("Retrieving process data. Complete!", "Import", clsFunction.IconType.iInformation);

                    btnLoadFile.Enabled = true;                    
                    btnCheckServiceSummary.Enabled = true;
                    btnPreviewFSR.Enabled = true;

                }
                catch (Exception ex)
                {
                    dbFunction.SetMessageBox(ex.Message, "Import FSR File", clsFunction.IconType.iError);
                    btnLoadFile.Enabled = true;
                }

            }
        }
        private void UploadFile(string sFileName)
        {
            // Upload File to FTP                
            ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
            ftpClient.delete(clsGlobalVariables.strFTPUploadPath + sFileName);
            ftpClient.upload(clsGlobalVariables.strFTPUploadPath + sFileName, @clsGlobalVariables.strFTPLocalPath + sFileName);
        }

        private void InitListView()
        {
            lvwSearch.Items.Clear();
            lvwSearch.View = View.Details;
            lvwSearch.Columns.Add(dbFunction.GetMapTo("No"), 50, HorizontalAlignment.Left); // Line#
            lvwSearch.Columns.Add("*FSRNo", dbFunction.ID_Width(), HorizontalAlignment.Left); // FSRNo
            lvwSearch.Columns.Add("*No", dbFunction.ID_Width(), HorizontalAlignment.Left); // No
            lvwSearch.Columns.Add(dbFunction.GetMapTo("Merchant"), 400, HorizontalAlignment.Left); // Merchant
            lvwSearch.Columns.Add(dbFunction.GetMapTo("MID"), 140, HorizontalAlignment.Left); // MID
            lvwSearch.Columns.Add(dbFunction.GetMapTo("TID"), 100, HorizontalAlignment.Left); // TID
            lvwSearch.Columns.Add(dbFunction.GetMapTo("Invoice No"), 0, HorizontalAlignment.Left); // Time Arrived (Military Time)
            lvwSearch.Columns.Add(dbFunction.GetMapTo("Invoice No"), 100, HorizontalAlignment.Left); // Time Arrived (Standard Time)
            lvwSearch.Columns.Add(dbFunction.GetMapTo("Batch No"), 0, HorizontalAlignment.Left); // Time Start (Military Time)
            lvwSearch.Columns.Add(dbFunction.GetMapTo("Batch No"), 100, HorizontalAlignment.Left); // Time Start (Standard Time)
            lvwSearch.Columns.Add(dbFunction.GetMapTo("PAN"), 100, HorizontalAlignment.Left); // Service Code
            lvwSearch.Columns.Add(dbFunction.GetMapTo("Date"), 100, HorizontalAlignment.Left); // Date            
            lvwSearch.Columns.Add(dbFunction.GetMapTo("Time"), 0, HorizontalAlignment.Left); // Receipt Time (Military Time)
            lvwSearch.Columns.Add(dbFunction.GetMapTo("Time"), 100, HorizontalAlignment.Left); // Receipt Time (Standard Time)
            lvwSearch.Columns.Add(dbFunction.GetMapTo("Txn Amt"), 0, HorizontalAlignment.Left); // Amount
            lvwSearch.Columns.Add(dbFunction.GetMapTo("Auth Code"), 0, HorizontalAlignment.Left); // Time End (Military Time)
            lvwSearch.Columns.Add(dbFunction.GetMapTo("Auth Code"), 100, HorizontalAlignment.Left); // Time End (Standard Time)
            lvwSearch.Columns.Add(dbFunction.GetMapTo("Ref. No"), 180, HorizontalAlignment.Left); // Terminal Serial No.
            lvwSearch.Columns.Add(dbFunction.GetMapTo("Phone No"), 180, HorizontalAlignment.Left); // Merchant Contact No
            lvwSearch.Columns.Add(dbFunction.GetMapTo("Email"), 180, HorizontalAlignment.Left); // Merchant Representative
            lvwSearch.Columns.Add(dbFunction.GetMapTo("NRIC"), 140, HorizontalAlignment.Left); // FE Name
            lvwSearch.Columns.Add(dbFunction.GetMapTo("Additional Information"), 220, HorizontalAlignment.Left); // SIM Serial|Power SN|Docsk SN

        }

        private void InitListViewServiceSummary()
        {
            lvwServiceSummary.Items.Clear();
            lvwServiceSummary.View = View.Details;
            lvwServiceSummary.Columns.Add("Line#", 0, HorizontalAlignment.Left); // Line#
            lvwServiceSummary.Columns.Add("Service Request", 210, HorizontalAlignment.Left);
            lvwServiceSummary.Columns.Add("Code", 60, HorizontalAlignment.Left);
            lvwServiceSummary.Columns.Add("Count", 130, HorizontalAlignment.Left);
            lvwServiceSummary.Columns.Add("Action", 110, HorizontalAlignment.Left);
            lvwServiceSummary.Columns.Add("ReasonID", 0, HorizontalAlignment.Left);
            lvwServiceSummary.Columns.Add("Reason", 300, HorizontalAlignment.Left);
            lvwServiceSummary.Columns.Add("Code", 100, HorizontalAlignment.Left);
            lvwServiceSummary.Columns.Add("ServiceNoList", 0, HorizontalAlignment.Left);
            lvwServiceSummary.Columns.Add("TAIDNoList", 0, HorizontalAlignment.Left);
            lvwServiceSummary.Columns.Add("IRIDNoList", 0, HorizontalAlignment.Left);
            lvwServiceSummary.Columns.Add("IRNoList", 0, HorizontalAlignment.Left);
            lvwServiceSummary.Columns.Add("RowIndexList", 0, HorizontalAlignment.Left);
            lvwServiceSummary.Columns.Add("RequestNoList", 0, HorizontalAlignment.Left);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);
            dbFunction.ClearListView(lvwSearch);
            dbFunction.ClearListView(lvwServiceCode);
            dbFunction.ClearListView(lvwDetail);
            dbFunction.ClearListView(lvwServiceSummary);

            InitTextBox();
            btnPreviewSummary.Enabled = false;
            btnCheckServiceSummary.Enabled = false;
            btnPreviewSummary.Enabled = false;
            btnPreviewFSR.Enabled = false;
            btnSaveImport.Enabled = false;

            // Load Mapping
            dbAPI.ExecuteAPI("GET", "View", "Type", "FSR", "Mapping", "", "ViewMapping");

            // Load Service Type
            dbAPI.ExecuteAPI("GET", "View", "Import FSR", "", "Service Type", "", "ViewServiceType");
        }
        private void InitTextBox()
        {            
            txtFilterDate.Text = clsFunction.sDateDefault + " to " + clsFunction.sDateDefault;
            txtTServiceCount.Text = clsFunction.sZero;
        }
        private void LoadFSR()
        {
            int i = 0;
            int iLineNo = 0;

            lvwSearch.Items.Clear();

            dbAPI.ExecuteAPI("GET", "View", "FSR Temp Detail", clsSearch.ClassAdvanceSearchValue, "FSR", "", "ViewFSR");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.FSRNo.Length > i)
                {
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.FSRNo[i]);
                    item.SubItems.Add(clsArray.No[i]);
                    item.SubItems.Add(clsArray.MerchantName[i]);
                    item.SubItems.Add(clsArray.MID[i]);
                    item.SubItems.Add(clsArray.TID[i]);

                    // Military Time
                    item.SubItems.Add(clsArray.InvoiceNo[i]);
                    
                    // Standard Time
                    string sTimeArrived = dbFunction.GetDateFromParse(clsArray.InvoiceNo[i], "H:mm:ss", "hh:mm tt");
                    item.SubItems.Add(sTimeArrived);

                    // Military Time
                    item.SubItems.Add(clsArray.BatchNo[i]);

                    // Standard Time
                    string sTimeStart = dbFunction.GetDateFromParse(clsArray.BatchNo[i], "H:mm:ss", "hh:mm tt");
                    item.SubItems.Add(sTimeStart);


                    item.SubItems.Add(clsArray.FSR[i]);
                    item.SubItems.Add(FormatDate(clsArray.FSRDate[i]));

                    // Military Time
                    string sFSRTime = dbFunction.GetDateFromParse(clsArray.FSRTime[i], "h:mm tt", "HH:mm:ss");
                    item.SubItems.Add(sFSRTime);

                    // Standard Time
                    string sReceiptTime = dbFunction.GetDateFromParse(sFSRTime, "H:mm:ss", "hh:mm tt");
                    item.SubItems.Add(sReceiptTime);

                    item.SubItems.Add(clsArray.TxnAmt[i]);

                    // Military Time
                    item.SubItems.Add(clsArray.TimeEnd[i]);

                    // Standard Time
                    string sTimeEnd = dbFunction.GetDateFromParse(clsArray.TimeEnd[i], "H:mm:ss", "hh:mm tt");
                    item.SubItems.Add(sTimeEnd);

                    item.SubItems.Add(clsArray.TerminalSN[i]);
                    item.SubItems.Add(clsArray.MerchantContactNo[i]);
                    item.SubItems.Add(clsArray.MerchantRepresentative[i]);
                    item.SubItems.Add(clsArray.FEName[i]);
                    item.SubItems.Add(clsArray.SerialNo[i]);

                    lvwSearch.Items.Add(item);

                    i++;

                }

                dbFunction.ListViewAlternateBackColor(lvwSearch);
                
                dbFunction.ListViewRowFocus(lvwSearch, 0); // Select first row

                sDateFrom = FormatDate(clsArray.FSRDate[0]);
                sDateTo = FormatDate(clsArray.FSRDate[i - 1]);
                
                FilterDate();
                ServiceTCount();
                ServiceCount();

            }
            else
            {
                // Do noting.
            }
        }
        private void FilterDate()
        {
            int i = 0;

            dbAPI.ExecuteAPI("GET", "View", "FSR Temp Detail Date Filter", "", "FSR", "", "ViewFSR");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.DateFrom.Length > i)
                {
                    sDateFrom = clsArray.DateFrom[i];
                    sDateTo = clsArray.DateTo[i];

                    i++;
                }
            }

            txtFilterDate.Text = sDateFrom + " to " + sDateTo;
        }

        private void ServiceTCount()
        {
            txtTServiceCount.Text = dbFunction.FormatCountWithComma(lvwSearch.Items.Count);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lvwSearch_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Get the new sorting column.
            ColumnHeader new_sorting_column = lvwSearch.Columns[e.Column];

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
            lvwSearch.ListViewItemSorter = new clsListView(e.Column, sort_order);

            // Sort.
            lvwSearch.Sort();
        }

        private void lvwSearch_Click(object sender, EventArgs e)
        {
            /*
            if (dbFunction.isValidID(txtLineNo.Text))
            {
                string sTitle = "Field Service Report(FSR) Details";
                string sMessage = clsFunction.sLineSeparator + Environment.NewLine +
                                  dbFunction.GetListViewSelectedRow(lvwSearch, int.Parse(txtLineNo.Text)) +
                                  clsFunction.sLineSeparator;

                dbFunction.ShowToolTip(lvwSearch, txtLineNo.Text, sTitle, sMessage);
            }
            */
        }

        private void lvwSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwSearch.SelectedItems.Count > 0)
            {
                string LineNo = lvwSearch.SelectedItems[0].Text;
                txtLineNo.Text = LineNo;
                
                clsSearch.ClassMerchantName = lvwSearch.SelectedItems[0].SubItems[3].Text;
                clsSearch.ClassMID = lvwSearch.SelectedItems[0].SubItems[4].Text;
                clsSearch.ClassTID = lvwSearch.SelectedItems[0].SubItems[5].Text;
                clsSearch.ClassServiceTypeCode = lvwSearch.SelectedItems[0].SubItems[10].Text;
                clsSearch.ClassTerminalSN = lvwSearch.SelectedItems[0].SubItems[17].Text;
                clsSearch.ClassSerialNoList = lvwSearch.SelectedItems[0].SubItems[21].Text;
            }
        }

        private void lvwSearch_DoubleClick(object sender, EventArgs e)
        {
            if (lvwSearch.Items.Count > 0)
            {
                if (dbFunction.isValidID(txtLineNo.Text))
                {
                    int iLineNo = int.Parse(txtLineNo.Text);

                    ListDetails(iLineNo, clsSearch.ClassServiceTypeCode);
                    dbFunction.ListViewAlternateBackColor(lvwDetail);
                }
            }
        }
        private void ListDetails(int iRow, string sServiceCode)
        {
                 
            // Display Service Type Description
            lblServiceType.Text = "";
            lblServiceType.Text = dbAPI.GetServiceDescriptionByCode(sServiceCode);

            // Display Delimeted SerialNo
            txtSIMSN.Text = "";
            txtPowerSN.Text = "";
            txtDockSN.Text = "";

            if (dbFunction.isValidDescription(clsSearch.ClassSerialNoList))
            {
                string sSIMSN = "";
                string sPowerSN = "";
                string sDockSN = "";
                ParseSerialNoList(clsSearch.ClassSerialNoList, ref sSIMSN, ref sPowerSN, ref sDockSN);
                txtSIMSN.Text = sSIMSN;
                txtPowerSN.Text = sPowerSN;
                txtDockSN.Text = sDockSN;
            }

            if (dbFunction.isValidID(iRow.ToString()))
            {
                lvwDetail.Items.Clear();
                dbFunction.DisplayListViewSelectedRow(lvwSearch, lvwDetail, iRow);
            }

        }
        private void ParseSerialNoList(string sSerialNoList, ref string sSIMSN, ref string sPowerSN, ref string sDockSN)
        {
            bool isValid = false;

            if (sSerialNoList.Length > 0)
            {
                if (sSerialNoList.IndexOf("|") > 0)
                {
                    isValid = true;
                }
            }

            if (isValid)
            {
                string[] SerialNo = sSerialNoList.Split('|');
                int iCount = SerialNo.Length;

                for (int x = 0; x < iCount; x++)
                {
                    switch (x)
                    {
                        case 0:
                            sSIMSN = SerialNo[0].ToString();
                            if (sSIMSN.Length <= 0)
                                sSIMSN = clsFunction.sDash;
                            break;
                        case 1:
                            sPowerSN = SerialNo[1].ToString();
                            if (sPowerSN.Length <= 0)
                                sPowerSN = clsFunction.sDash;
                            break;
                        case 2:
                            sDockSN = SerialNo[2].ToString();
                            if (sDockSN.Length <= 0)
                                sDockSN = clsFunction.sDash;
                            break;
                    }
                }
            }
            else
            {
                sSIMSN = clsFunction.sDash;
                sPowerSN = clsFunction.sDash;
                sDockSN = clsFunction.sDash;
            }
        }

        private void ServiceCount()
        {
            int i = 0;            
            string sServiceCode = "";
            int iCount = 0;

            dbFunction.ClearListView(lvwServiceCode);

            List<string> ServiceCountCol = new List<String>();

            if (clsArray.ServiceTypeID.Length > 0)
            {
                for (i = 0; i < clsArray.ServiceTypeID.Length; i++)
                {
                    iCount = 0;
                    sServiceCode = clsArray.Code[i].ToString();
                    
                    if (sServiceCode.Length > 0)
                    {
                        foreach (ListViewItem x in lvwSearch.Items)
                        {
                            string sPAN = x.SubItems[10].Text;

                            if (sPAN.CompareTo(sServiceCode) == 0)
                            {
                                iCount++;
                            }
                        }

                        ServiceCountCol.Add(iCount.ToString());
                    }
                }

                clsArray.ServiceCount = ServiceCountCol.ToArray();
            }

            // Display List of Service Type With Count
            if (clsArray.ServiceTypeID.Length > 0)
            {
                i = 0;
                lvwServiceCode.Items.Clear();
                while (clsArray.ServiceTypeID.Length > i)
                {
                    clsServiceType.ClassServiceTypeID = int.Parse(clsArray.ServiceTypeID[i].ToString());
                    clsServiceType.ClassDescription = clsArray.Description[i].ToString();
                    clsServiceType.ClassCode = clsArray.Code[i].ToString();
                    clsServiceType.ClassServiceCount = int.Parse(clsArray.ServiceCount[i].ToString());
                    clsServiceType.ClassStatusDescription = clsArray.ServiceStatusDescription[i].ToString();

                    i++;

                    ListViewItem item = new ListViewItem(i.ToString());
                    item.SubItems.Add(clsServiceType.ClassServiceTypeID.ToString());
                    item.SubItems.Add(clsServiceType.ClassDescription.ToString());
                    item.SubItems.Add(clsServiceType.ClassCode.ToString());
                    item.SubItems.Add(clsServiceType.ClassServiceCount.ToString());
                    item.SubItems.Add(clsServiceType.ClassStatusDescription);

                    lvwServiceCode.Items.Add(item);
                }

                dbFunction.ListViewAlternateBackColor(lvwServiceCode);
            }

        }

        private string FormatDate(string sDate)
        {
            string sFormmated = "";

            DateTime FSRDate = DateTime.Parse(sDate);
            sFormmated = FSRDate.ToString("yyyy-MM-dd");

            return sFormmated;
        }

        private string FormatTime(string sTime)
        {
            string sFormatted = "";

            sFormatted = dbFunction.GetDateFromParse(sTime, "hh:mm tt", "HHmmss");

            return sFormatted;
            
        }

        private void btnCheckServiceSummary_Click(object sender, EventArgs e)
        {
            
            dbFunction.SetMessageBox("Processing of Check Service Summary. This will take a few minute(s).", "Service Summary", clsFunction.IconType.iInformation);

            Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
            
            lvwServiceSummary.Items.Clear();

            if (lvwServiceCode.Items.Count > 0)
            {
                foreach (ListViewItem i in lvwServiceCode.Items)
                {
                    string sServiceRequest = i.SubItems[2].Text;
                    string sServiceCode = i.SubItems[3].Text;

                    if (dbFunction.isValidDescription(sServiceCode))
                    {
                        dbAPI.DownloadService(clsFunction.sDateFormat, clsFunction.sDateFormat, clsFunction.sZero, sServiceCode); // Download Service (Installation)
                        ProcessServiceSummary(sServiceCode, sServiceRequest);
                    }
                }
            }
            
            dbFunction.ListViewAlternateBackColor(lvwServiceSummary);

            dbFunction.ListViewRowFocus(lvwServiceSummary, 0);

            btnSaveImport.Enabled = true;

            Cursor.Current = Cursors.Default; // Back to normal

            //dbFunction.SetMessageBox("Check Service Summary... Complete", "Service Summary", clsFunction.IconType.iInformation);

        }

        private void ProcessServiceSummary(string sServiceTypeCode, string sServiceDescription)
        {            
            bool fFound = false;
            int iImportCount = 0;
            int iIgnoreCount = 0;
            int iNotFoundCount = 0;

            string sImportFSRNo;            
            string sIgnoreFSRNo;
            string sNotFoundFSRNo;

            string sImportServiceNo;
            string sImportTAIDNo;
            string sImportIRIDNo;
            string sImportIRNo;
            string sImportRowIndex;
            string sImportRequestNo;

            int iLineNo = 0;
            bool fCompleted = false;
            string sServiceNo = clsFunction.sZero;
            string sTAIDNo = clsFunction.sZero;
            string sIRIDNo = clsFunction.sZero;
            string sIRNo = clsFunction.sZero;
            string sRequestNo = clsFunction.sZero;

            if (lvwSearch.Items.Count > 0)
            {                                
                sImportFSRNo = clsFunction.sNull;
                sIgnoreFSRNo = clsFunction.sNull;
                sNotFoundFSRNo = clsFunction.sNull;

                sImportServiceNo = clsFunction.sNull;
                sImportTAIDNo = clsFunction.sNull;
                sImportIRIDNo = clsFunction.sNull;
                sImportIRNo = clsFunction.sNull;
                sImportRowIndex = clsFunction.sNull;
                sImportRequestNo = clsFunction.sNull;

                foreach (ListViewItem x in lvwSearch.Items)
                {
                    string sFSRNo = x.SubItems[1].Text;
                    string sMID = x.SubItems[4].Text;
                    string sTID = x.SubItems[5].Text;
                    string sServiceCode = x.SubItems[10].Text;
                    string sTerminalSN = x.SubItems[17].Text;
                        
                    if (sServiceCode.CompareTo(sServiceTypeCode) == 0)
                    {
                        fFound = CheckService(sTID, sMID, sServiceCode, sTerminalSN, ref fCompleted, ref sServiceNo, ref sTAIDNo, ref sIRIDNo, ref sIRNo, ref sRequestNo);

                        if (fFound)
                        {
                            if (fCompleted)
                            {
                                if (sIgnoreFSRNo.CompareTo(clsFunction.sZero) != 0)
                                    sIgnoreFSRNo = sIgnoreFSRNo + clsFunction.sNull + sFSRNo + clsFunction.sNull + clsFunction.sComma;

                                iIgnoreCount++;
                            }
                            else
                            {
                                if (sImportFSRNo.CompareTo(clsFunction.sZero) != 0)
                                {
                                    sImportFSRNo = sImportFSRNo + clsFunction.sNull + sFSRNo + clsFunction.sNull + clsFunction.sComma;

                                    sImportServiceNo = sImportServiceNo + clsFunction.sNull + sServiceNo + clsFunction.sNull + clsFunction.sComma;
                                    sImportTAIDNo = sImportTAIDNo + clsFunction.sNull + sTAIDNo + clsFunction.sNull + clsFunction.sComma;
                                    sImportIRIDNo = sImportIRIDNo + clsFunction.sNull + sIRIDNo + clsFunction.sNull + clsFunction.sComma;
                                    sImportIRNo = sImportIRNo + clsFunction.sNull + sIRNo + clsFunction.sNull + clsFunction.sComma;
                                    sImportRowIndex = sImportRowIndex + clsFunction.sNull + x.Index.ToString() + clsFunction.sNull + clsFunction.sComma;
                                    sImportRequestNo = sImportRequestNo + clsFunction.sNull + sRequestNo + clsFunction.sNull + clsFunction.sComma;
                                }
                                    

                                iImportCount++;
                            }
                        }
                        else
                        {
                            if (sNotFoundFSRNo.CompareTo(clsFunction.sZero) != 0)
                                sNotFoundFSRNo = sNotFoundFSRNo + clsFunction.sNull + sFSRNo + clsFunction.sNull + clsFunction.sComma;

                            iNotFoundCount++;
                        }
                    }
                }

                // Add To ListView
                Debug.WriteLine("sImportFSRNo=" + sImportFSRNo);
                Debug.WriteLine("sIgnoreFSRNo=" + sIgnoreFSRNo);
                Debug.WriteLine("sNotFoundFSRNo=" + sNotFoundFSRNo);

                Debug.WriteLine("sImportServiceNo=" + sImportServiceNo);
                Debug.WriteLine("sImportTAIDNo=" + sImportTAIDNo);
                Debug.WriteLine("sImportIRIDNo=" + sImportIRIDNo);
                Debug.WriteLine("sImportIRNo=" + sImportIRNo);
                Debug.WriteLine("sImportRowIndex=" + sImportRowIndex);
                Debug.WriteLine("sImportRequestNo=" + sImportRequestNo);

                //--------------------------------------------------------------------------------------------------------------------------------------
                // Update Here
                //--------------------------------------------------------------------------------------------------------------------------------------
                if (dbFunction.isValidDescription(sImportFSRNo))
                {
                    string sFSRNoList = sImportFSRNo.Remove(sImportFSRNo.Length - 1);
                    Debug.WriteLine("sFSRNoList=" + sFSRNoList);
                    clsSearch.ClassAdvanceSearchValue = sFSRNoList + clsFunction.sPipe + clsGlobalVariables.READY_FOR_IMPORT + clsFunction.sPipe + clsGlobalVariables.READY_FOR_IMPORT_DESC;
                    dbAPI.ExecuteAPI("PUT", "Update", "Update FSR Temp Detail Reason", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
                }

                if (dbFunction.isValidDescription(sIgnoreFSRNo))
                {
                    string sFSRNoList = sIgnoreFSRNo.Remove(sIgnoreFSRNo.Length - 1);
                    Debug.WriteLine("sFSRNoList=" + sFSRNoList);
                    clsSearch.ClassAdvanceSearchValue = sFSRNoList + clsFunction.sPipe + clsGlobalVariables.SERVICE_ALREADY_PROCESSED + clsFunction.sPipe + clsGlobalVariables.SERVICE_ALREADY_PROCESSED_DESC;
                    dbAPI.ExecuteAPI("PUT", "Update", "Update FSR Temp Detail Reason", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
                }

                if (dbFunction.isValidDescription(sNotFoundFSRNo))
                {
                    string sFSRNoList = sNotFoundFSRNo.Remove(sNotFoundFSRNo.Length - 1);
                    Debug.WriteLine("sFSRNoList=" + sFSRNoList);
                    clsSearch.ClassAdvanceSearchValue = sFSRNoList + clsFunction.sPipe + clsGlobalVariables.IMPORT_SERVICE_NOT_FOUND + clsFunction.sPipe + clsGlobalVariables.IMPORT_SERVICE_NOT_FOUND_DESC;
                    dbAPI.ExecuteAPI("PUT", "Update", "Update FSR Temp Detail Reason", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
                }


                iLineNo = 1;
                AddToServiceSummary(iLineNo, sServiceDescription, sServiceTypeCode, clsFunction.sNull, clsFunction.sNull, clsFunction.sNull, clsFunction.sNull, clsFunction.sNull, clsFunction.sNull, clsFunction.sNull, clsFunction.sNull, clsFunction.sNull, clsFunction.sNull, clsFunction.sNull);
                AddToServiceSummary(iLineNo, clsFunction.sNull, clsFunction.sNull, iImportCount.ToString(), clsGlobalVariables.IMPORT_ACTION_DESC, clsGlobalVariables.READY_FOR_IMPORT.ToString(), clsGlobalVariables.READY_FOR_IMPORT_DESC, sServiceTypeCode, sImportServiceNo, sImportTAIDNo, sImportIRIDNo, sImportIRNo, sImportRowIndex, sImportRequestNo);
                AddToServiceSummary(iLineNo, clsFunction.sNull, clsFunction.sNull, iIgnoreCount.ToString(), clsGlobalVariables.IGNORE_ACTION_DESC, clsGlobalVariables.SERVICE_ALREADY_PROCESSED.ToString(), clsGlobalVariables.SERVICE_ALREADY_PROCESSED_DESC, sServiceTypeCode, clsFunction.sNull, clsFunction.sNull, clsFunction.sNull, clsFunction.sNull, clsFunction.sNull, clsFunction.sNull);
                AddToServiceSummary(iLineNo, clsFunction.sNull, clsFunction.sNull, iNotFoundCount.ToString(), clsGlobalVariables.IGNORE_ACTION_DESC, clsGlobalVariables.IMPORT_SERVICE_NOT_FOUND.ToString(), clsGlobalVariables.IMPORT_SERVICE_NOT_FOUND_DESC, sServiceTypeCode, clsFunction.sNull, clsFunction.sNull, clsFunction.sNull, clsFunction.sNull, clsFunction.sNull, clsFunction.sNull);
                
                //--------------------------------------------------------------------------------------------------------------------------------------
                // Download Service
                //--------------------------------------------------------------------------------------------------------------------------------------
            }
        }

        private void AddToServiceSummary(int iLineNo, string sServiceRequest, string sServiceCode, string sCount, string sAction, string sReasonID, string sReason, string sTempServiceCode, 
                                         string sServiceNoList, string sTAIDNoList, 
                                         string sIRIDNoList, string sIRNoList, 
                                         string sRowIndexList, string sRequestNoList)
        {
            ListViewItem item = new ListViewItem(iLineNo.ToString());
            item.SubItems.Add(sServiceRequest);
            item.SubItems.Add(sServiceCode);
            item.SubItems.Add((sCount.Length > 0 ? sCount : clsFunction.sNull));
            item.SubItems.Add(sAction);
            item.SubItems.Add(sReasonID);
            item.SubItems.Add(sReason);
            item.SubItems.Add(sTempServiceCode);
            item.SubItems.Add(sServiceNoList);
            item.SubItems.Add(sTAIDNoList);
            item.SubItems.Add(sIRIDNoList);
            item.SubItems.Add(sIRNoList);
            item.SubItems.Add(sRowIndexList);
            item.SubItems.Add(sRequestNoList);

            lvwServiceSummary.Items.Add(item);            
        }
        
        private bool CheckService(string sTID, string sMID, string sServiceCode, string sTerminalSN, ref bool fCompleted, 
                                  ref string sServiceNo, ref string sTAIDNo, 
                                  ref string sIRIDNo, ref string sIRNo, 
                                  ref string sRequestNo)
        {
            bool fFound = false;
            int i = 0;

            if (!clsServicingDetail.RecordFound) return false;

            while (clsArray.ServiceNo.Length > i)
            {
                string arrServiceNo = clsArray.ServiceNo[i];
                string arrTAIDNo = clsArray.TAIDNo[i];
                string arrIRIDNo = clsArray.IRIDNo[i];
                string arrIRNo = clsArray.IRNo[i];
                string arrTID = clsArray.TID[i];
                string arrMID = clsArray.MID[i];
                string arrServiceCode = clsArray.ServiceCode[i];
                string arrTerminalSN = clsArray.TerminalSN[i];
                string arrJobTypeStatusDescription = clsArray.JobTypeStatusDescription[i];
                string arrRequestNo = clsArray.RequestNo[i];

                if ((sTID.CompareTo(arrTID) == 0) &&
                        (sMID.CompareTo(arrMID) == 0) &&
                        (sServiceCode.CompareTo(arrServiceCode) == 0) &&
                        (sTerminalSN.CompareTo(arrTerminalSN) == 0))
                {
                    fFound = true;

                    if (arrJobTypeStatusDescription.CompareTo(clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC) == 0)
                    {
                        fCompleted = true;
                    }
                    else
                    {
                        sServiceNo = arrServiceNo;
                        sTAIDNo = arrTAIDNo;
                        sIRIDNo = arrIRIDNo;
                        sIRNo = arrIRNo;
                        sRequestNo = arrRequestNo;
                        fCompleted = false;
                    }                    
                    break;
                }

                i++;
            }

            return fFound;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (lvwSearch.Items.Count > 0)
            {
                Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

                try
                {
                    if (clsSearch.ClassReasonID == clsGlobalVariables.READY_FOR_IMPORT)
                    {
                        clsSearch.ClassSearchBy = "FSR Ready For Import";                        
                    }

                    else if (clsSearch.ClassReasonID == clsGlobalVariables.SERVICE_ALREADY_PROCESSED)
                    {
                        clsSearch.ClassSearchBy = "FSR Already Processed";
                    }

                    else if (clsSearch.ClassReasonID == clsGlobalVariables.IMPORT_SERVICE_NOT_FOUND)
                    {
                        clsSearch.ClassSearchBy = "FSR Import Service Not Found";
                    }
                        
                    clsSearch.ClassReportID = 7;
                    clsSearch.ClassStatementType = "View";
                    clsSearch.ClassSearchValue = clsSearch.ClassReasonID + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsSearch.ClassServiceTypeCode;
                    clsSearch.ClassServiceTypeDescription = dbAPI.GetServiceDescriptionByCode(clsSearch.ClassServiceTypeCode);                    
                    clsSearch.ClassStoredProcedureName = "spViewFSR";
                    clsSearch.ClassDateFrom = sDateFrom;
                    clsSearch.ClassDateTo = sDateTo;
                    dbFunction.ProcessReport(clsSearch.ClassReportID);
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

        private void lvwServiceCode_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lvwServiceSummary_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnPreviewSummary.Enabled = false;
            if (lvwServiceSummary.SelectedItems.Count > 0)
            {
                string LineNo = lvwServiceSummary.SelectedItems[0].Text;
                txtLineNo.Text = LineNo;
                
                clsSearch.ClassTCount = lvwServiceSummary.SelectedItems[0].SubItems[3].Text;

                if (dbFunction.isValidID(clsSearch.ClassTCount))
                {
                    clsSearch.ClassReasonID = int.Parse(lvwServiceSummary.SelectedItems[0].SubItems[5].Text);
                    clsSearch.ClassReasonDescription = lvwServiceSummary.SelectedItems[0].SubItems[6].Text;
                    clsSearch.ClassServiceTypeCode = lvwServiceSummary.SelectedItems[0].SubItems[7].Text;
                    
                    btnPreviewSummary.Enabled = true;
                }
                
            }
        }

        private void frmImportFieldServiveReport_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void btnSaveImport_Click(object sender, EventArgs e)
        {
            FillClientTextBox();

            if (!ValidateFields()) return;

            if (!dbFunction.fSavingConfirm(false)) return;

            Cursor.Current = Cursors.WaitCursor;

            SaveFSRMaster();

            ProcessReadyForImport();

            Cursor.Current = Cursors.Default;

            dbFunction.SetMessageBox("Import Field Service Report successfully saved.", "Saved", clsFunction.IconType.iInformation);

            btnClear_Click(this, e);
        }

        private void SaveFSRMaster()
        {
            string sRowSQL = "";
            string sSQL = "";

            DateTime ImportDateTime = DateTime.Now;
            string sImportDateTime = "";

            DateTime DateTimeModified = DateTime.Now;
            string sDateTimeModified = "";

            string sFileName = clsFSR.ClassFileName;
            string sRemarks = clsFSR.ClassRemarks;

            string sProcessedBy = clsUser.ClassUserFullName;
            string sModifiedBy = clsUser.ClassUserFullName;

            sImportDateTime = ImportDateTime.ToString("yyyy-MM-dd H:mm:ss");
            sDateTimeModified = DateTimeModified.ToString("yyyy-MM-dd H:mm:ss");

            sRowSQL = "";
            sRowSQL = " ('" + sImportDateTime + "', " +
            sRowSQL + sRowSQL + " '" + sFileName + "', " +
            sRowSQL + sRowSQL + " '" + sFileName + "', " +
            sRowSQL + sRowSQL + " '" + sRemarks + "', " +
            sRowSQL + sRowSQL + " '" + sDateTimeModified + "', " +
            sRowSQL + sRowSQL + " '" + sProcessedBy + "', " +
            sRowSQL + sRowSQL + " '" + sModifiedBy + "') ";
            sSQL = sSQL + sRowSQL;

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "FSR Import Master", sSQL, "InsertCollectionMaster");

            clsSearch.ClassLastInsertedID = clsLastID.ClassLastInsertedID;
        }

        
        private void btnPreviewFSR_Click(object sender, EventArgs e)
        {
            if (lvwSearch.Items.Count > 0)
            {
                Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

                try
                {                    
                    clsSearch.ClassReportID = 8;
                    clsSearch.ClassStatementType = "View";
                    clsSearch.ClassSearchValue = clsFunction.sZero;
                    clsSearch.ClassServiceTypeDescription = dbAPI.GetServiceDescriptionByCode(clsSearch.ClassServiceTypeCode);
                    clsSearch.ClassStoredProcedureName = "spViewFSR";
                    clsSearch.ClassSearchBy = "FSR Temp Detail";
                    clsSearch.ClassSearchValue = clsFunction.sZero;
                    clsSearch.ClassDateFrom = sDateFrom;
                    clsSearch.ClassDateTo = sDateTo;
                    dbFunction.ProcessReport(clsSearch.ClassReportID);
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

        private void ProcessReadyForImport()
        {
            string sRowSQL = "";
            string sSQL = "";

            if (lvwServiceCode.Items.Count > 0)
            {
                foreach (ListViewItem z in lvwServiceCode.Items)
                {
                    string sImportServiceCode = z.SubItems[3].Text;

                    if (sImportServiceCode.CompareTo(clsGlobalVariables.TA_STATUS_INSTALLED_CODE) == 0)
                        iSearchJobType = clsAPI.JobType.iInstallation;
                    else if (sImportServiceCode.CompareTo(clsGlobalVariables.TA_STATUS_PULLEDOUT_CODE) == 0)
                        iSearchJobType = clsAPI.JobType.iPullOut;
                    else if (sImportServiceCode.CompareTo(clsGlobalVariables.TA_STATUS_REPLACEMENT_CODE) == 0)
                        iSearchJobType = clsAPI.JobType.iReplacement;

                    Debug.WriteLine("sImportServiceCode="+ sImportServiceCode);
                    Debug.WriteLine("iSearchJobType=" + iSearchJobType);

                    if (lvwServiceSummary.Items.Count > 0)
                    {
                        foreach (ListViewItem x in lvwServiceSummary.Items)
                        {
                            string sTempServiceCode = x.SubItems[7].Text;

                            if (sTempServiceCode.CompareTo(sImportServiceCode) == 0)
                            {
                                string sCount = x.SubItems[3].Text;
                                string sAction = x.SubItems[4].Text;

                                if (dbFunction.isValidID(sCount))
                                {
                                    string sJobTypeDescription = clsFunction.sNull;
                                    string sJobTypeSubDescription = clsFunction.sNull;

                                    dbAPI.GetJobTypeDescriptionByServiceCode(sTempServiceCode, ref sJobTypeDescription, ref sJobTypeSubDescription);

                                    int iCount = int.Parse(sCount);
                                    if (iCount > 0)
                                    {
                                        if (sAction.CompareTo(clsGlobalVariables.IMPORT_ACTION_DESC) == 0)
                                        {
                                            string sServiceNoList = x.SubItems[8].Text;
                                            string sTAIDNoList = x.SubItems[9].Text;
                                            string sIRIDNoList = x.SubItems[10].Text;
                                            string sIRNoList = x.SubItems[11].Text;
                                            string sRowIndexList = x.SubItems[12].Text;
                                            string sRequestNoList = x.SubItems[13].Text;

                                            if (sRowIndexList.Length > 0)
                                            {
                                                bool isValid = false;

                                                if (sRowIndexList.Length > 0)
                                                {
                                                    if (sRowIndexList.IndexOf(",") > 0)
                                                    {
                                                        isValid = true;
                                                    }
                                                }

                                                sRowIndexList = sRowIndexList.Remove(sRowIndexList.Length - 1); // Remove Extra Command at End

                                                if (isValid)
                                                {
                                                    string[] sTempRowIndex = sRowIndexList.Split(',');
                                                    int iDelimCount = sTempRowIndex.Length;

                                                    // ServiceNo
                                                    string[] sTempServiceNo = sServiceNoList.Split(',');

                                                    // TAIDNo
                                                    string[] sTempTAIDNo = sTAIDNoList.Split(',');

                                                    // IRNoID
                                                    string[] sTempIRIDNo = sIRIDNoList.Split(',');

                                                    // IRNo
                                                    string[] sTempIRNo = sIRNoList.Split(',');

                                                    // RequestNo
                                                    // IRNo
                                                    string[] sTempRequestNo = sRequestNoList.Split(',');

                                                    for (int y = 0; y < iDelimCount; y++)
                                                    {
                                                        string sRow = sTempRowIndex[y].ToString();
                                                        string sServiceNo = sTempServiceNo[y].ToString();
                                                        string sTAIDNo = sTempTAIDNo[y].ToString();
                                                        string sIRIDNo = sTempIRIDNo[y].ToString();
                                                        string sIRNo = sTempIRNo[y].ToString();
                                                        string sRequestNo = sTempRequestNo[y].ToString();

                                                        Debug.WriteLine("y=" + y + clsFunction.sPipe +
                                                                        "sRow=" + sRow + clsFunction.sPipe +
                                                                        "sServiceNo=" + sServiceNo + clsFunction.sPipe +
                                                                        "sTAIDNo=" + sTAIDNo + clsFunction.sPipe +
                                                                        "sIRIDNo=" + sIRIDNo + clsFunction.sPipe +
                                                                        "sIRNo=" + sIRNo + clsFunction.sPipe +
                                                                        "sRequestNo=" + sRequestNo);

                                                        // Save here...
                                                        if (dbFunction.isValidID(sServiceNo))
                                                        {
                                                            foreach (ListViewItem i in lvwSearch.Items)
                                                            {
                                                                int iRow = int.Parse(sRow);

                                                                string xTerminalID = clsFunction.sZero;
                                                                string xTerminalSN = clsFunction.sZero;
                                                                string xDockID = clsFunction.sZero;
                                                                string xDockSN = clsFunction.sZero;
                                                                string xSIMID = clsFunction.sZero;
                                                                string xSIMSN = clsFunction.sZero;
                                                                
                                                                if (i.Index == iRow)
                                                                {
                                                                    string sFSRNo = i.SubItems[1].Text;
                                                                    string sNo = i.SubItems[2].Text;
                                                                    string sMerchantName = i.SubItems[3].Text;
                                                                    string sMID = i.SubItems[4].Text;
                                                                    string sTID = i.SubItems[5].Text;
                                                                    string sTimeArrived = i.SubItems[6].Text;
                                                                    string sTimeStart = i.SubItems[8].Text;
                                                                    string sServiceCode = i.SubItems[10].Text;
                                                                    string sDate = i.SubItems[11].Text;
                                                                    string sReceiptTime = i.SubItems[12].Text;
                                                                    string sAmount = i.SubItems[14].Text;
                                                                    string sTimeEnd = i.SubItems[15].Text;
                                                                    string sTerminalSN = i.SubItems[17].Text;
                                                                    string sMerchContactNo = i.SubItems[18].Text;
                                                                    string sMerchRep = i.SubItems[19].Text;
                                                                    string sFEName = i.SubItems[20].Text;
                                                                    string sSerialNoList = i.SubItems[21].Text;

                                                                    dbAPI.GetFSRServiceTypeStatus(sJobTypeDescription); // Get FSR Service Type

                                                                    sMerchantName = sMerchantName.Replace("(", clsFunction.sNull);
                                                                    sMerchantName = sMerchantName.Replace(")", clsFunction.sNull);

                                                                    Debug.WriteLine("sMerchantName="+ sMerchantName);

                                                                    // Save to tblfsrdetail
                                                                    if (dbFunction.isValidID(sFSRNo))
                                                                    {
                                                                        switch (iSearchJobType)
                                                                        {
                                                                            case clsAPI.JobType.iInstallation:
                                                                            case clsAPI.JobType.iPullOut:
                                                                            case clsAPI.JobType.iReprogramming:
                                                                                sSQL = "";
                                                                                sRowSQL = "";
                                                                                sRowSQL = " ('" + clsSearch.ClassLastInsertedID.ToString() + "', " +
                                                                                sRowSQL + sRowSQL + " '" + sNo + "', " +
                                                                                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(sMerchantName) + "', " +
                                                                                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(sMID) + "', " +
                                                                                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(sTID) + "', " +
                                                                                sRowSQL + sRowSQL + " '" + sTimeArrived + "', " +
                                                                                sRowSQL + sRowSQL + " '" + sTimeArrived + "', " +
                                                                                sRowSQL + sRowSQL + " '" + sTimeStart + "', " +
                                                                                sRowSQL + sRowSQL + " '" + sTimeStart + "', " +
                                                                                sRowSQL + sRowSQL + " '" + sServiceCode + "', " +
                                                                                sRowSQL + sRowSQL + " '" + sDate + "', " +
                                                                                sRowSQL + sRowSQL + " '" + sReceiptTime + "', " +
                                                                                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(sAmount) + "', " +
                                                                                sRowSQL + sRowSQL + " '" + sTimeEnd + "', " +
                                                                                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(sTerminalSN) + "', " +
                                                                                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(sMerchContactNo) + "', " +
                                                                                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(sMerchRep) + "', " +
                                                                                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(sFEName) + "', " +
                                                                                sRowSQL + sRowSQL + " '" + clsFunction.sDash + "', " +
                                                                                sRowSQL + sRowSQL + " '" + sSerialNoList + "', " +
                                                                                sRowSQL + sRowSQL + " '" + clsGlobalVariables.FSR_VALID_STATUS + "', " +
                                                                                sRowSQL + sRowSQL + " '" + clsGlobalVariables.FSR_VALID_STATUS_DESC + "', " +
                                                                                sRowSQL + sRowSQL + " '" + clsSearch.ClassServiceTypeStatus + "', " +
                                                                                sRowSQL + sRowSQL + " '" + clsSearch.ClassServiceTypeStatusDescription + "', " +
                                                                                sRowSQL + sRowSQL + " '" + clsFunction.sZero + "', " +
                                                                                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtClientID.Text) + "', " +
                                                                                sRowSQL + sRowSQL + " '" + cboSearchClient.Text + "', " +
                                                                                sRowSQL + sRowSQL + " '" + clsGlobalVariables.PROCESS_TYPE_AUTO_DESC + "', " +
                                                                                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(sTAIDNo) + "', " +
                                                                                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(sIRIDNo) + "', " +
                                                                                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(sIRNo) + "', " +
                                                                                sRowSQL + sRowSQL + " '" + clsGlobalVariables.ACTION_MADE_SUCCESS + "', " +
                                                                                sRowSQL + sRowSQL + " '" + sJobTypeDescription + "', " +
                                                                                sRowSQL + sRowSQL + " '" + clsFunction.sDash + "', " +
                                                                                sRowSQL + sRowSQL + " '" + clsFunction.sDash + "', " +
                                                                                sRowSQL + sRowSQL + " '" + clsFunction.sDash + "') ";
                                                                                sSQL = sSQL + sRowSQL;

                                                                                Debug.WriteLine("SaveAutoFSRDetail::" + "\n" + "sSQL=" + sSQL);

                                                                                dbAPI.ExecuteAPI("POST", "Insert", "", "", "FSR Manual Detail", sSQL, "InsertCollectionDetail");

                                                                                string xFSRNo = clsLastID.ClassLastInsertedID.ToString(); // Last inserted ID in tblfsrdetail

                                                                                //dbAPI.ExecuteAPI("POST", "Insert", "", "", "FSR Manual Detail Dummy", sSQL, "InsertCollectionDetail");

                                                                                // Parse and Save SerialNo List
                                                                                string sSIMSN = clsFunction.sNull;
                                                                                string sPowerSN = clsFunction.sNull;
                                                                                string sDockSN = clsFunction.sNull;
                                                                                ParseSerialNoList(sSerialNoList, ref sSIMSN, ref sPowerSN, ref sDockSN);
                                                                                
                                                                                // Update FSR SerialNo
                                                                                if (dbFunction.isValidID(xFSRNo))
                                                                                    dbAPI.UpdateSerialNoList(xFSRNo, sSIMSN, sPowerSN, sDockSN);

                                                                                // Update FSR ServiceNo
                                                                                if (dbFunction.isValidID(xFSRNo))
                                                                                    dbAPI.UpdateFSRServiceNo(xFSRNo, sServiceNo);

                                                                                break;  
                                                                        }
                                                                    }
                                                                    
                                                                    // Update tblirdetail
                                                                    if (dbFunction.isValidID(sIRIDNo))
                                                                    {
                                                                        dbAPI.UpdateIRDetailStatus(sIRIDNo, clsSearch.ClassServiceTypeStatus, clsSearch.ClassServiceTypeStatusDescription);
                                                                    }

                                                                    // Update tblterminalallocation
                                                                    if (dbFunction.isValidID(sTAIDNo))
                                                                    {
                                                                        dbAPI.UpdateTADetailStatus(sTAIDNo, clsSearch.ClassServiceTypeStatus, clsSearch.ClassServiceTypeStatusDescription);
                                                                    }

                                                                    Debug.WriteLine("ProcessReadyForImport::iSearchJobType=" + iSearchJobType);
                                                                    Debug.WriteLine("ProcessReadyForImport::sServiceNo=" + sServiceNo);
                                                                    Debug.WriteLine("ProcessReadyForImport::sRequestNo=" + sRequestNo);
                                                                    Debug.WriteLine("ProcessReadyForImport::sIRIDNo=" + sIRIDNo);
                                                                    Debug.WriteLine("ProcessReadyForImport::sTAIDNo=" + sTAIDNo);

                                                                    // Update tblterminal/tblsimdetail
                                                                    if (dbFunction.isValidID(sServiceNo))
                                                                    {
                                                                        dbAPI.GetServicingCurrentTerminalInfo(sServiceNo, sRequestNo); // Get Serial Detail
                                                                        xTerminalID = clsServicingDetail.ClassTerminalID.ToString();
                                                                        xTerminalSN = clsServicingDetail.ClassTerminalSN;

                                                                        xDockID = clsServicingDetail.ClassDockID.ToString();
                                                                        xDockSN = clsServicingDetail.ClassDockSN;

                                                                        xSIMID = clsServicingDetail.ClassSIMID.ToString();
                                                                        xSIMSN = clsServicingDetail.ClassSIMSN;

                                                                        Debug.WriteLine("ProcessReadyForImport::xTerminalID=" + xTerminalID + "|xTerminalSN="+ xTerminalSN);
                                                                        Debug.WriteLine("ProcessReadyForImport::xDockID=" + xDockID + "|xDockSN=" + xDockSN);
                                                                        Debug.WriteLine("ProcessReadyForImport::xSIMID=" + xSIMID + "|xSIMSN=" + xSIMSN);

                                                                        switch (iSearchJobType)
                                                                        {
                                                                            case clsAPI.JobType.iInstallation:

                                                                                // Reset Value
                                                                                clsSearch.ClassCurTerminalSNStatus = clsSearch.ClassCurSIMSNStatus = clsSearch.ClassCurDockSNStatus = clsFunction.iZero;
                                                                                clsSearch.ClassCurTerminalSNStatusDescription = clsSearch.ClassCurSIMSNStatusDescription = clsSearch.ClassCurDockSNStatusDescription = clsFunction.sNull;
                                                                                
                                                                                if (dbFunction.isValidID(xTerminalID))
                                                                                {
                                                                                    dbAPI.UpdateTerminalDetailStatus(xTerminalID, clsGlobalVariables.STATUS_INSTALLED, clsGlobalVariables.STATUS_INSTALLED_DESC);
                                                                                    clsSearch.ClassCurTerminalSNStatus = clsGlobalVariables.STATUS_INSTALLED;
                                                                                    clsSearch.ClassCurTerminalSNStatusDescription = clsGlobalVariables.STATUS_INSTALLED_DESC;
                                                                                }

                                                                                if (dbFunction.isValidID(xDockID))
                                                                                {
                                                                                    dbAPI.UpdateTerminalDetailStatus(xDockID, clsGlobalVariables.STATUS_INSTALLED, clsGlobalVariables.STATUS_INSTALLED_DESC);
                                                                                    clsSearch.ClassCurDockSNStatus = clsGlobalVariables.STATUS_INSTALLED;
                                                                                    clsSearch.ClassCurDockSNStatusDescription = clsGlobalVariables.STATUS_INSTALLED_DESC;
                                                                                }

                                                                                if (dbFunction.isValidID(xSIMID))
                                                                                {
                                                                                    dbAPI.UpdateSIMDetailStatus(xSIMID, clsGlobalVariables.STATUS_INSTALLED, clsGlobalVariables.STATUS_INSTALLED_DESC);
                                                                                    clsSearch.ClassCurSIMSNStatus = clsGlobalVariables.STATUS_INSTALLED;
                                                                                    clsSearch.ClassCurSIMSNStatusDescription = clsGlobalVariables.STATUS_INSTALLED_DESC;
                                                                                }

                                                                                dbAPI.UpdateServicingCurrentTerminalStatus(sServiceNo, sRequestNo);
                                                                                dbAPI.UpdateServiceClose(sServiceNo, sTAIDNo, sRequestNo, sJobTypeDescription, clsGlobalVariables.ACTION_MADE_NEGATIVE, clsGlobalVariables.SVC_REQ_CLOSE);
                                                                                dbAPI.UpdateServiceJobType(sServiceNo, sRequestNo, clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED, sJobTypeDescription, clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC, sJobTypeSubDescription);
                                                                                dbAPI.UpdateServicingActionMade(sServiceNo, sRequestNo, clsGlobalVariables.ACTION_MADE_SUCCESS);
                                                                                dbAPI.UpdateIRClose(sIRIDNo, sIRNo, clsGlobalVariables.ACTION_MADE_SUCCESS);
                                                                                dbAPI.UpdateServicingDetailStatus(iSearchJobType, sServiceNo, sRequestNo, sIRIDNo);
                                                                                

                                                                                break;
                                                                            case clsAPI.JobType.iPullOut:
                                                                                
                                                                                // Reset Value
                                                                                clsSearch.ClassCurTerminalSNStatus = clsSearch.ClassCurSIMSNStatus = clsSearch.ClassCurDockSNStatus = clsFunction.iZero;
                                                                                clsSearch.ClassCurTerminalSNStatusDescription = clsSearch.ClassCurSIMSNStatusDescription = clsSearch.ClassCurDockSNStatusDescription = clsFunction.sNull;

                                                                                dbAPI.GetServicingCurrentTerminalInfo(sServiceNo, sRequestNo); // Get Serial Detail
                                                                                
                                                                                if (dbFunction.isValidID(xTerminalID))
                                                                                {
                                                                                    clsSearch.ClassStatus = dbAPI.GetStatus(clsServicingDetail.ClassCurTerminalSNStatusDescription);
                                                                                    clsSearch.ClassStatusDescription = dbAPI.GetStatusDescription(clsSearch.ClassStatus);

                                                                                    dbAPI.UpdateTerminalDetailStatus(xTerminalID, clsGlobalVariables.STATUS_AVAILABLE, clsGlobalVariables.STATUS_AVAILABLE_DESC);
                                                                                    clsSearch.ClassCurTerminalSNStatus = clsGlobalVariables.STATUS_AVAILABLE;
                                                                                    clsSearch.ClassCurTerminalSNStatusDescription = clsGlobalVariables.STATUS_AVAILABLE_DESC;
                                                                                }

                                                                                if (dbFunction.isValidID(xDockID))
                                                                                {
                                                                                    clsSearch.ClassStatus = dbAPI.GetStatus(clsServicingDetail.ClassCurDockSNStatusDescription);
                                                                                    clsSearch.ClassStatusDescription = dbAPI.GetStatusDescription(clsSearch.ClassStatus);

                                                                                    dbAPI.UpdateTerminalDetailStatus(xDockID, clsGlobalVariables.STATUS_AVAILABLE, clsGlobalVariables.STATUS_AVAILABLE_DESC);
                                                                                    clsSearch.ClassCurDockSNStatus = clsGlobalVariables.STATUS_AVAILABLE;
                                                                                    clsSearch.ClassCurDockSNStatusDescription = clsGlobalVariables.STATUS_AVAILABLE_DESC;
                                                                                }

                                                                                if (dbFunction.isValidID(xSIMID))
                                                                                {
                                                                                    clsSearch.ClassStatus = dbAPI.GetStatus(clsServicingDetail.ClassCurSIMSNStatusDescription);
                                                                                    clsSearch.ClassStatusDescription = dbAPI.GetStatusDescription(clsSearch.ClassStatus);

                                                                                    dbAPI.UpdateSIMDetailStatus(xSIMID, clsGlobalVariables.STATUS_AVAILABLE, clsGlobalVariables.STATUS_AVAILABLE_DESC);
                                                                                    clsSearch.ClassCurSIMSNStatus = clsGlobalVariables.STATUS_AVAILABLE;
                                                                                    clsSearch.ClassCurSIMSNStatusDescription = clsGlobalVariables.STATUS_AVAILABLE_DESC;
                                                                                }

                                                                                dbAPI.UpdateServicingCurrentTerminalStatus(sServiceNo, sRequestNo);
                                                                                dbAPI.UpdateServiceClose(sServiceNo, sTAIDNo, sRequestNo, sJobTypeDescription, clsGlobalVariables.ACTION_MADE_NEGATIVE, clsGlobalVariables.SVC_REQ_CLOSE);
                                                                                dbAPI.UpdateServiceJobType(sServiceNo, sRequestNo, clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED, sJobTypeDescription, clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC, sJobTypeSubDescription);
                                                                                dbAPI.UpdateServicingActionMade(sServiceNo, sRequestNo, clsGlobalVariables.ACTION_MADE_SUCCESS);
                                                                                dbAPI.UpdateIRClose(sIRIDNo, sIRNo, clsGlobalVariables.ACTION_MADE_SUCCESS);
                                                                                dbAPI.UpdateServicingDetailStatus(iSearchJobType, sServiceNo, sRequestNo, sIRIDNo);

                                                                                break;
                                                                            case clsAPI.JobType.iReplacement:                                                                                                                                                               
                                                                            case clsAPI.JobType.iReprogramming:                                                                                                                                                                
                                                                            case clsAPI.JobType.iServicing:                                                                               
                                                                            case clsAPI.JobType.iNegative:
                                                                                dbAPI.UpdateServiceClose(sServiceNo, sTAIDNo, sRequestNo, sJobTypeDescription, clsGlobalVariables.ACTION_MADE_NEGATIVE, clsGlobalVariables.SVC_REQ_CLOSE);
                                                                                dbAPI.UpdateServiceJobType(sServiceNo, sRequestNo, clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED, sJobTypeDescription, clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC, sJobTypeSubDescription);
                                                                                dbAPI.UpdateServicingActionMade(sServiceNo, sRequestNo, clsGlobalVariables.ACTION_MADE_SUCCESS);
                                                                                dbAPI.UpdateIRClose(sIRIDNo, sIRNo, clsGlobalVariables.ACTION_MADE_SUCCESS);
                                                                                dbAPI.UpdateServicingDetailStatus(iSearchJobType, sServiceNo, sRequestNo, sIRIDNo);
                                                                                break;
                                                                        }
                                                                    }
                                                                                                                                                                                                                                                                                                                                                                                                                                                      
                                                                    break;
                                                                }                                                                
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
        }

        void FillClientTextBox()
        {
            int i = 0;

            clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                cboSearchClient.Text;

            Debug.WriteLine("FillClientTextBox::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbAPI.ExecuteAPI("GET", "View", "Advance Particular", clsSearch.ClassAdvanceSearchValue, "Particular", "", "ViewAdvanceParticular");

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.ParticularID.Length > i)
                {
                    txtClientID.Text = clsArray.ParticularID[i].ToString();

                    i++;

                }
            }
        }
        private bool ValidateFields()
        {            

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iClientID, txtClientID.Text)) return false;
            
            return true;
        }
        private void InitImportDate()
        {
            DateTime ImportDateTime = DateTime.Now;
            string sImportDateTime = "";

            DateTime DateTimeModified = DateTime.Now;
            string sDateTimeModified = "";
            
            sImportDateTime = ImportDateTime.ToString("yyyy-MM-dd H:mm:ss");
            sDateTimeModified = DateTimeModified.ToString("yyyy-MM-dd H:mm:ss");

            txtImportDate.Text = sImportDateTime;
            txtProcessBy.Text = clsUser.ClassUserFullName;
        }

        private void GetAndParseSerialNoList(string sSerialNoList, ref string sSIMSN, ref string sPowerSN, ref string sDockSN)
        {
            bool isValid = false;

            if (sSerialNoList.Length > 0)
            {
                if (sSerialNoList.IndexOf("|") > 0)
                {
                    isValid = true;
                }
            }

            if (isValid)
            {
                string[] SerialNo = sSerialNoList.Split('|');
                int iCount = SerialNo.Length;

                for (int x = 0; x < iCount; x++)
                {
                    switch (x)
                    {
                        case 0:
                            sSIMSN = SerialNo[0].ToString();
                            if (sSIMSN.Length <= 0)
                                sSIMSN = clsFunction.sDash;
                            break;
                        case 1:
                            sPowerSN = SerialNo[1].ToString();
                            if (sPowerSN.Length <= 0)
                                sPowerSN = clsFunction.sDash;
                            break;
                        case 2:
                            sDockSN = SerialNo[2].ToString();
                            if (sDockSN.Length <= 0)
                                sDockSN = clsFunction.sDash;
                            break;
                    }
                }
            }
            else
            {
                sSIMSN = clsFunction.sDash;
                sPowerSN = clsFunction.sDash;
                sDockSN = clsFunction.sDash;
            }
        }

        private void txtRemarks_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }
    }
}
