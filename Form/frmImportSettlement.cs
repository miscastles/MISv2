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
using System.IO;
using OfficeOpenXml;
using System.Linq;
using MIS.Model;

namespace MIS
{
    public partial class frmImportSettlement : Form
    {
        private clsAPI dbAPI;
        private clsINI dbSetting;
        private clsFile dbFile;
        private clsFunction dbFunction;
        private clsReportFunc dbReportFunc;

        public frmImportSettlement()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwImport, true);
            dbFunction.setDoubleBuffer(lvwPreview, true);
            dbFunction.setDoubleBuffer(lvwPerTransType, true);
            dbFunction.setDoubleBuffer(lvwPerMonth, true);
            dbFunction.setDoubleBuffer(lvwPerTopSales, true);
            dbFunction.setDoubleBuffer(lvwPerQtr, true);
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmImportSettlement_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;                
            }
        }

        private void initListView(int index, ListView lvw)
        {
            lvw.View = View.Details;
            lvw.FullRowSelect = true;
            lvw.GridLines = true;
            lvw.Columns.Clear();
            
            switch (index)
            {
                case 0: // Import
                    lvw.Columns.Add("Line#", 50, HorizontalAlignment.Left);
                    lvw.Columns.Add("FileName", 290, HorizontalAlignment.Left);
                    lvw.Columns.Add("Path", 0, HorizontalAlignment.Left);
                    break;
                case 1: // Summary Per Trans Type
                    lvw.Columns.Add("Line#", 60, HorizontalAlignment.Left);
                    lvw.Columns.Add("TID", 90, HorizontalAlignment.Left);
                    lvw.Columns.Add("Merchant Name", 250, HorizontalAlignment.Left);
                    lvw.Columns.Add("Credit", 120, HorizontalAlignment.Right);
                    lvw.Columns.Add("Debit", 120, HorizontalAlignment.Right);
                    lvw.Columns.Add("QRPay", 120, HorizontalAlignment.Right);
                    lvw.Columns.Add("QRPh", 120, HorizontalAlignment.Right);
                    lvw.Columns.Add("Total", 140, HorizontalAlignment.Right);
                    break;
                case 2: // Summary Per Month
                    lvw.Columns.Add("Line#", 60, HorizontalAlignment.Left);
                    lvw.Columns.Add("TID", 90, HorizontalAlignment.Left);
                    lvw.Columns.Add("Merchant Name", 250, HorizontalAlignment.Left);
                    lvw.Columns.Add("January", 120, HorizontalAlignment.Right);
                    lvw.Columns.Add("February", 120, HorizontalAlignment.Right);
                    lvw.Columns.Add("March", 120, HorizontalAlignment.Right);
                    lvw.Columns.Add("April", 120, HorizontalAlignment.Right);
                    lvw.Columns.Add("May", 120, HorizontalAlignment.Right);
                    lvw.Columns.Add("June", 120, HorizontalAlignment.Right);
                    lvw.Columns.Add("July", 120, HorizontalAlignment.Right);
                    lvw.Columns.Add("August", 120, HorizontalAlignment.Right);
                    lvw.Columns.Add("September", 120, HorizontalAlignment.Right);
                    lvw.Columns.Add("October", 120, HorizontalAlignment.Right);
                    lvw.Columns.Add("November", 120, HorizontalAlignment.Right);
                    lvw.Columns.Add("December", 120, HorizontalAlignment.Right);
                    lvw.Columns.Add("Total", 140, HorizontalAlignment.Right);
                    break;
                case 3: // Top 10 Per Merchant Sales
                    lvw.Columns.Add("Line#", 60, HorizontalAlignment.Left);
                    lvw.Columns.Add("TID", 90, HorizontalAlignment.Left);
                    lvw.Columns.Add("Merchant Name", 250, HorizontalAlignment.Left);
                    lvw.Columns.Add("Amount", 150, HorizontalAlignment.Right);
                    break;
                case 4: // Summary Per Qtr
                    lvw.Columns.Add("Line#", 60, HorizontalAlignment.Left);
                    lvw.Columns.Add("Qtr", 150, HorizontalAlignment.Left);
                    lvw.Columns.Add("Amount", 150, HorizontalAlignment.Right);
                    break;
                case 5:
                    lvw.Columns.Add("Line#", 0, HorizontalAlignment.Left);
                    lvw.Columns.Add("TransType", 80, HorizontalAlignment.Left);
                    lvw.Columns.Add("Start Date", 90, HorizontalAlignment.Right);
                    lvw.Columns.Add("End Date", 90, HorizontalAlignment.Right);
                    lvw.Columns.Add("Count", 80, HorizontalAlignment.Right);
                    lvw.Columns.Add("Amount", 120, HorizontalAlignment.Right);

                    break;

            }
        }

        private void frmImportSettlement_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbSetting = new clsINI();
            dbFile = new clsFile();
            dbFunction = new clsFunction();
            dbReportFunc = new clsReportFunc();

            dbFunction.ClearTextBox(this);

            initListView(0, lvwImport);
            initListView(1, lvwPerTransType);
            initListView(2, lvwPerMonth);
            initListView(3, lvwPerTopSales);
            initListView(4, lvwPerQtr);
            initListView(5, lvwSummary);

            InitDateRange();

            chkDateRange_CheckedChanged(this, e);

            txtSearch.ReadOnly = false;
            txtSearch.BackColor = Color.White;

            tabFilter.SelectedIndex = 0;

            ucStatus.iState = 3;

            loadDataSummary(lvwSummary);

            Cursor.Current = Cursors.Default;
        }

        private void loadDataPerMonth(ListView lvw)
        {
            int iLineNo = 0;
            int i = 0;
            double dblTAmount = 0.00;

            ucStatus.sMessage = $"Processing summary per month...";
            ucStatus.AnimateStatus();

            dbFunction.ClearListViewItems(lvw);

            dbAPI.ExecuteAPI("GET", "View", "ERM Settlement Report-Per Month", $"{clsSearch.ClassDateFrom}{clsFunction.sPipe}{clsSearch.ClassDateTo}{clsFunction.sPipe}{txtSearch.Text.Trim()}", "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {   
                while (clsArray.ID.Length > i)
                {
                    iLineNo++;

                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    
                    // Data
                    string pJSONString = clsArray.detail_info[i];
                    
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TID));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MERCHANTNAME));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_January));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_February));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_March));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_April));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_May));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_June));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_July));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_August));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_September));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_October));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_November));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_December));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Total));

                    dblTAmount += double.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Total));

                    lvw.Items.Add(item);

                    i++;

                }

                dbFunction.ListViewAlternateBackColor(lvw);
                
            }

            // Total
            txtGTCntPerMonth.Text = $"{lvw.Items.Count}";
            txtGTPerMonth.Text = dblTAmount.ToString("N2");

            ucStatus.sMessage = $"Processing summary per month...complete";
            ucStatus.AnimateStatus();
        }

        private void loadDataPerTransType(ListView lvw)
        {
            int iLineNo = 0;
            int i = 0;
            double dblTAmount = 0.00;

            ucStatus.sMessage = $"Processing summary per trans type...";
            ucStatus.AnimateStatus();

            dbFunction.ClearListViewItems(lvw);

            dbAPI.ExecuteAPI("GET", "View", "ERM Settlement Report-Per Trans Type", $"{clsSearch.ClassDateFrom}{clsFunction.sPipe}{clsSearch.ClassDateTo}{clsFunction.sPipe}{txtSearch.Text.Trim()}", "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.ID.Length > i)
                {
                    iLineNo++;

                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                    // Data
                    string pJSONString = clsArray.detail_info[i];

                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TID));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MERCHANTNAME));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Credit));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Debit));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_QRPay));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_QRPh));                    
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Total));

                    dblTAmount += double.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Total));

                    lvw.Items.Add(item);

                    i++;

                }

                dbFunction.ListViewAlternateBackColor(lvw);

            }

            // Total
            txtGTCntPerTransType.Text = $"{lvw.Items.Count}";
            txtGTPerTransType.Text = dblTAmount.ToString("N2");

            ucStatus.sMessage = $"Processing summary per trans type...complete";
            ucStatus.AnimateStatus();
        }
        
        private void loadDataPerTopSales(ListView lvw)
        {
            int iLineNo = 0;
            int i = 0;
            double dblTAmount = 0.00;

            ucStatus.sMessage = $"Processing summary per top sales...";
            ucStatus.AnimateStatus();

            dbFunction.ClearListViewItems(lvw);

            dbAPI.ExecuteAPI("GET", "View", "ERM Settlement Report-Per Top Sales", $"{clsSearch.ClassDateFrom}{clsFunction.sPipe}{clsSearch.ClassDateTo}{clsFunction.sPipe}{txtSearch.Text.Trim()}", "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.ID.Length > i)
                {
                    iLineNo++;

                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                    // Data
                    string pJSONString = clsArray.detail_info[i];

                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TID));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MERCHANTNAME));                                   
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Total));

                    dblTAmount += double.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Total));

                    lvw.Items.Add(item);

                    i++;

                }

                dbFunction.ListViewAlternateBackColor(lvw);

            }

            // Total      
            txtGTCntPerTopSales.Text = $"{lvw.Items.Count}";
            txtGTPerTopSales.Text = dblTAmount.ToString("N2");

            ucStatus.sMessage = $"Processing summary per top sales...complete";
            ucStatus.AnimateStatus();
        }

        private void loadDataPerQtr(ListView lvw)
        {
            int iLineNo = 0;
            int i = 0;
            double dblTAmount = 0.00;

            ucStatus.sMessage = $"Processing summary per qtr...";
            ucStatus.AnimateStatus();

            dbFunction.ClearListViewItems(lvw);

            dbAPI.ExecuteAPI("GET", "View", "ERM Settlement Report-Per Qtr", $"{clsSearch.ClassDateFrom}{clsFunction.sPipe}{clsSearch.ClassDateTo}{clsFunction.sPipe}{txtSearch.Text.Trim()}", "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.ID.Length > i)
                {
                    iLineNo++;

                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                    // Data
                    string pJSONString = clsArray.detail_info[i];

                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Quarter));                    
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Total));

                    dblTAmount += double.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Total));

                    lvw.Items.Add(item);

                    i++;

                }

                dbFunction.ListViewAlternateBackColor(lvw);

            }

            // Total            
            txtGTCntPerQtr.Text = $"{lvw.Items.Count}";
            txtGTPerQtr.Text = dblTAmount.ToString("N2");

            ucStatus.sMessage = $"Processing summary per qtr...complete";
            ucStatus.AnimateStatus();
        }

        private void loadDataSummary(ListView lvw)
        {
            int iLineNo = 0;
            int i = 0;
            int TCount = 0;
            double dblTAmount = 0.00;

            ucStatus.sMessage = $"Processing summary...";
            ucStatus.AnimateStatus();

            dbFunction.ClearListViewItems(lvw);

            dbAPI.ExecuteAPI("GET", "View", "ERM Settlement Report-Summary", "", "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.ID.Length > i)
                {
                    iLineNo++;

                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                    // Data
                    string pJSONString = clsArray.detail_info[i];

                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Description));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_StartDate));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_EndDate));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TCount));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TAmount));

                    TCount += int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TCount));
                    dblTAmount += double.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TAmount));

                    lvw.Items.Add(item);

                    i++;

                }

                dbFunction.ListViewAlternateBackColor(lvw);

            }

            // Total            
            txtGTCntSummary.Text = $"{TCount}";
            txtGTSummary.Text = dblTAmount.ToString("N2");

            ucStatus.sMessage = $"Processing summary...complete";
            ucStatus.AnimateStatus();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            // Check if nothing is checked
            if (!chkPerTransType.Checked &&
                !chkPerMonth.Checked &&
                !chkPerTopSales.Checked &&
                !chkPerQtr.Checked)
            {
                dbFunction.SetMessageBox("Please select at least one report type before proceeding.",
                    clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);
                return;
            }

            if (!dbFunction.fPromptConfirmation("Are you sure to proceed search?")) return;
            
            dbFunction.SetMessageBox("This process may take a few minutes to complete.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);

            Cursor.Current = Cursors.WaitCursor;

            initTotal();

            if (chkDateRange.Checked)
            {
                clsSearch.ClassDateFrom = dbFunction.CheckAndSetDatePickerValueToDate(dteDateFrom);
                clsSearch.ClassDateTo = dbFunction.CheckAndSetDatePickerValueToDate(dteDateTo);
            }
            else
            {
                clsSearch.ClassDateFrom = clsSearch.ClassDateTo = clsFunction.sDateFormat;
            }

            // cleanup
            ucStatus.sMessage = $"Update ERM settlement report from server...";
            ucStatus.AnimateStatus();
            dbAPI.ExecuteAPI("PUT", "Update", "ERM Settlement Report", "x", "", "", "UpdateCollectionDetail");

            if (chkPerTransType.Checked)
                loadDataPerTransType(lvwPerTransType);

            if (chkPerMonth.Checked)
                loadDataPerMonth(lvwPerMonth);

            if (chkPerTopSales.Checked)
                loadDataPerTopSales(lvwPerTopSales);

            if (chkPerQtr.Checked)
                loadDataPerQtr(lvwPerQtr);

            Cursor.Current = Cursors.Default;

            tabMain.SelectedIndex = 1;

            ucStatus.iState = 3;
            ucStatus.sMessage = "Process complete";
            ucStatus.AnimateStatus();

            dbFunction.SetMessageBox("Load data complete", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dbFunction.ClearListViewItems(lvwImport);
            dbFunction.ClearListViewItems(lvwPreview);
            dbFunction.ClearListViewItems(lvwPerTransType);
            dbFunction.ClearListViewItems(lvwPerMonth);
            dbFunction.ClearListViewItems(lvwPerTopSales);
            dbFunction.ClearListViewItems(lvwPerQtr);

            dbFunction.ClearTextBox(this);

            dteDateFrom.Enabled = dteDateTo.Enabled = false;
            chkDateRange_CheckedChanged(this, e);

            initTotal();
            txtSearch.ReadOnly = false;
            txtSearch.BackColor = Color.White;

            tabFilter.SelectedIndex = 0;
            tabMain.SelectedIndex = 0;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            // Check if nothing is checked
            if (!chkPerTransType.Checked &&
                !chkPerMonth.Checked &&
                !chkPerTopSales.Checked &&
                !chkPerQtr.Checked)
            {
                dbFunction.SetMessageBox("Please select at least one report type before proceeding.",
                    clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);
                return;
            }

            if (!dbFunction.fPromptConfirmation("Are you sure to export?")) return;

            Cursor.Current = Cursors.WaitCursor;

            var reports = new ReportInfo[]
            {
                new ReportInfo
                {
                    ListView = lvwPerTransType,
                    SheetName = "Per Trans Type",
                    Title = "Summary per trans type",
                    Count = txtGTCntPerTransType.Text,
                    Total = txtGTPerTransType.Text
                },
                new ReportInfo
                {
                    ListView = lvwPerMonth,
                    SheetName = "Per Month",
                    Title = "Summary per month",
                    Count = txtGTCntPerMonth.Text,
                    Total = txtGTPerMonth.Text
                },
                new ReportInfo
                {
                    ListView = lvwPerTopSales,
                    SheetName = "Per Top Sales",
                    Title = "Summary per top sales",
                    Count = txtGTCntPerTopSales.Text,
                    Total = txtGTPerTopSales.Text
                },
                new ReportInfo
                {
                    ListView = lvwPerQtr,
                    SheetName = "Per Qtr",
                    Title = "Summary per qtr",
                    Count = txtGTCntPerQtr.Text,
                    Total = txtGTPerQtr.Text
                }
            };
            
            string filename = $"{clsSearch.ClassBankCode}_ERM-Settlement-Report_Date_Range_From_{clsSearch.ClassDateFrom}_To_{clsSearch.ClassDateTo}_{clsDefines.FILE_EXT_XLXS}";
            dbFile.ExportMultipleListViewsToExcel(reports, $"{filename}", false);

            Cursor.Current = Cursors.Default;

            dbFunction.SetMessageBox("Export complete", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select File(s)";
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.Multiselect = true;
                openFileDialog.InitialDirectory = dbFile.sImportPath;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Optional: clear previous list
                    // lvFiles.Items.Clear();

                    int lineNumber = lvwImport.Items.Count + 1;

                    foreach (string fullPath in openFileDialog.FileNames)
                    {
                        string fileName = Path.GetFileName(fullPath);

                        // Create new ListViewItem
                        ListViewItem item = new ListViewItem(lineNumber.ToString()); // Column 1: Line #
                        item.SubItems.Add(fileName); // Column 2: Filename
                        item.SubItems.Add(fullPath); // Column 3: Full path

                        lvwImport.Items.Add(item);
                        lineNumber++;
                    }
                    
                }
            }
        }

        private void lvwImport_DoubleClick(object sender, EventArgs e)
        {
            if (lvwImport.SelectedItems.Count == 0)
                return; // nothing selected

            tabMain.SelectedIndex = 0;
            dbFunction.ClearListViewItems(lvwPreview);

            // Get the selected item (from your earlier list of files)
            ListViewItem selectedItem = lvwImport.SelectedItems[0];

            // Full Path
            string filePath = selectedItem.SubItems[2].Text;

            if (!File.Exists(filePath))
            {
                MessageBox.Show("The file does not exist:\n" + filePath,
                    "File Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            // Create or clear preview ListView (example: lvwPreview)
            lvwPreview.Items.Clear();
            lvwPreview.Columns.Clear();
            lvwPreview.View = View.Details;
            lvwPreview.FullRowSelect = true;

            // Call your existing Excel load function
            dbFile.LoadExcelToListView(filePath, lvwPreview);

            txtGTCntPreview.Text = $"{lvwPreview.Items.Count}";

            // Optional: auto-resize for readability
            //lvwPreview.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            Cursor.Current = Cursors.Default;

            dbFunction.SetMessageBox("File import and data loading have been completed successfully.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);

        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            if (lvwImport.Items.Count == 0)
            {
                dbFunction.SetMessageBox("No files to process.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iWarning);
                return;
            }

            if (!dbFunction.fPromptConfirmation("Are you sure to process?")) return;

            Cursor.Current = Cursors.WaitCursor;

            string csvPath = dbFile.sExportPath;

            if (!Directory.Exists(csvPath))
                Directory.CreateDirectory(csvPath);

            int successCount = 0;
            int errorCount = 0;

            // -------------------------------------------------
            // convert xlsx to csv
            // -------------------------------------------------
            foreach (ListViewItem item in lvwImport.Items)
            {
                try
                { 
                    string fileName = item.SubItems[1].Text;
                    string xlsPath = item.SubItems[2].Text; 

                    if (File.Exists(xlsPath))
                    {
                        ucStatus.sMessage = $"Converting file {fileName} to csv...";
                        ucStatus.AnimateStatus();

                        dbFile.convertExcelToCsv(xlsPath, csvPath, fileName);

                        ucStatus.sMessage = $"Converting file {fileName} to csv...complete";
                        ucStatus.AnimateStatus();

                        successCount++;
                    }
                    else
                    {
                        errorCount++;
                        Console.WriteLine($"File not found: {xlsPath}");
                    }
                }
                catch (Exception ex)
                {
                    errorCount++;
                    Console.WriteLine($"Error processing file: {ex.Message}");
                }
            }
            
            // -------------------------------------------------
            // upload to ftp
            // -------------------------------------------------
            ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
            foreach (ListViewItem item in lvwImport.Items)
            {
                try
                {
                    string fileName = item.SubItems[1].Text.Replace(clsDefines.FILE_EXT_XLXS, clsDefines.FILE_EXT_CSV);
                    
                    string localPath = $"{dbFile.sExportPath}";
                    string remotePath = $"{clsGlobalVariables.strFTPRemoteErmPath}{clsGlobalVariables.strAPIBank}{clsFunction.sBackSlash}";
                    
                    Debug.WriteLine("Uploading import file...");
                    Debug.WriteLine($"localPath=[{localPath}]");
                    Debug.WriteLine($"remotePath=[{remotePath}]");
                    Debug.WriteLine($"fileName=[{fileName}]");

                    ucStatus.sMessage = $"Uploading file {fileName} to server...";
                    ucStatus.AnimateStatus();

                    ftpClient.delete(remotePath + fileName);
                    ftpClient.upload(remotePath + fileName, localPath + fileName);

                    ucStatus.sMessage = $"Converting file {fileName} to server...complete";
                    ucStatus.AnimateStatus();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error upload file: {ex.Message}");
                }
                
            }

            // call api truncate table
            ucStatus.sMessage = $"Truncate ERM settlement report from server...";
            ucStatus.AnimateStatus();
            dbAPI.ExecuteAPI("DELETE", "Delete", "ERM Settlement Report", "", "ERM Settlement Report", "", "DeleteCollectionDetail");

            // call api for import
            foreach (ListViewItem item in lvwImport.Items)
            {
                string fileName = item.SubItems[1].Text.Replace(clsDefines.FILE_EXT_XLXS, clsDefines.FILE_EXT_CSV);

                Debug.WriteLine("Executing import file...");                
                Debug.WriteLine($"fileName=[{fileName}]");

                ucStatus.sMessage = $"Import ERM settlement report [{fileName}] from server...";
                ucStatus.AnimateStatus();
                dbAPI.ExecuteAPI("POST", "Import", "CSV", fileName, "ERM Settlement Report", "", "ImportERMSettlementReport"); // Process CSV File

            }

            // call api updatecollectiondetail
            ucStatus.sMessage = $"Update ERM settlement report from server...";
            ucStatus.AnimateStatus();            
            dbAPI.ExecuteAPI("PUT", "Update", "ERM Settlement Report", "x", "", "", "UpdateCollectionDetail");

            // Summary
            btnRefresh_Click(this, e);

            Cursor.Current = Cursors.Default;

            dbFunction.SetMessageBox(
                $"Settlement report process completed.\n\nSuccessful: {successCount}\nFailed: {errorCount}",
                clsDefines.FIELD_CHECK_MSG,
                clsFunction.IconType.iInformation
            );

            ucStatus.sMessage = "";
            ucStatus.AnimateStatus();
        }

        private void InitDateRange()
        {
            dteDateFrom.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteDateFrom, clsFunction.sDateDefaultFormat);

            dteDateTo.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteDateTo, clsFunction.sDateDefaultFormat);
            
        }

        private void chkDateRange_CheckedChanged(object sender, EventArgs e)
        {
            dteDateFrom.Enabled = dteDateTo.Enabled = false;

            if (chkDateRange.Checked)
                dteDateFrom.Enabled = dteDateTo.Enabled = true;

        }

        private void initTotal()
        {
            txtGTCntPreview.Text =
                txtGTCntPerTransType.Text =
                txtGTPerTransType.Text =
                txtGTCntPerMonth.Text =
                txtGTPerMonth.Text =
                txtGTCntPerTopSales.Text =
                txtGTPerTopSales.Text =
                txtGTCntPerQtr.Text =
                txtGTPerQtr.Text = clsFunction.sZero;

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            loadDataSummary(lvwSummary);

            Cursor.Current = Cursors.WaitCursor;
        }
    }
}
