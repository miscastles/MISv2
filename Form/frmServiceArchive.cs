using MIS.Global;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

using static MIS.Function.AppUtilities;

namespace MIS
{
    public partial class frmServiceArchive : Form
    {

        private clsAPI dbAPI;
        private clsFunction dbFunction;
        private clsFile dbFile;

        private const string gReportListHeader = "FSR REPORT LIST";
        private const string gFileNamePrefix = "FSR_ARCHIVE";

#if ENABLE_COMPOSITED
                protected override CreateParams CreateParams
                {
                    get
                    {
                        CreateParams cp = base.CreateParams;
                        cp.ExStyle |= 0x02000000;
                        return cp;
                    }
                }
#endif

        public frmServiceArchive()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwList, true);
        }

        private void frmServiceArchieving_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            dbFile = new clsFile();

            dbFunction.ClearComboBox(this);
            dbFunction.ClearListViewItems(lvwList);

            btnClear_Click(this, e);

            InitDateRange();

            dbAPI.FillComboBoxServiceType(cboSearchServiceType);
            dbAPI.FillComboBoxFSRMode(cboFSRModeType);
            dbAPI.FillComboBoxPositionType(cboFEName, clsDefines.FIELD_ENGINEER_POSITION_TYPE);

            lblResultList.Text = gReportListHeader;

            initArchivePath();

            txtSearch.ReadOnly = false;

            Cursor.Current = Cursors.Default;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dbFunction.ClearListViewItems(lvwList);
            InitDateRange();
            lblResultList.Text = gReportListHeader;

            lblFSRFound.Text =
            lblFSRNotFound.Text =
            lblFSRTotal.Text =
            lblDiagFound.Text =
            lblDiagNotFound.Text =
            lblDiagTotal.Text = clsFunction.sZero;

            txtSearch.ReadOnly = false;

        }

        private void loadData()
        {
            int i = 0;
            int iLineNo = 0;

            int fsrFoundCount = 0;
            int fsrNotFoundCount = 0;

            int diagFoundCount = 0;
            int diagNotFoundCount = 0;

            int signCount = 0;
            int imageCount = 0;

            int fsrFileSize = 0;
            int diagnosticFileSize = 0;

            Cursor.Current = Cursors.WaitCursor;

            Debug.WriteLine("--loadData--");

            lvwList.Items.Clear();

            clsSearch.ClassFSRMode = cboFSRModeType.Text;

            clsSearch.ClassSearchValue = $"{clsSearch.ClassJobType}{clsDefines.gPipe}" +
                                        $"{clsDefines.gZero}{clsDefines.gPipe}" +
                                        $"{clsSearch.ClassDateFrom}{clsDefines.gPipe}" +
                                        $"{clsSearch.ClassDateTo}{clsDefines.gPipe}" +
                                        $"{clsSearch.ClassSearchString}{clsDefines.gPipe}" +
                                        $"{clsSearch.ClassFSRMode}{clsDefines.gPipe}" +
                                        $"{clsSearch.ClassParticularID}";

            Debug.WriteLine("clsSearch.ClassSearchValue=" + clsSearch.ClassSearchValue);

            dbAPI.ExecuteAPI("GET", "View", "FSR Service Detail", clsSearch.ClassSearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFoundMessage()) return;

            lvwList.BeginUpdate();

            while (clsArray.ID.Length > i)
            {
                iLineNo++;

                ListViewItem item = new ListViewItem(iLineNo.ToString());

                // Data
                string pJSONString = clsArray.detail_info[i];

                string serviceno = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SERVICENO);

                item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SERVICENO));
                item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_IRIDNO));
                item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_JobTypeDescription));
                item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MERCHANTNAME));
                item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TID));
                item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MID));
                item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_IRNO));
                item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FSRDate));

                string fsrFileName = $"{serviceno}{clsDefines.FSR_FILENAME_PREFIX}{clsDefines.FILE_EXT_PDF}";
                string diagnosticFileName = $"{serviceno}{clsDefines.DIAGNOSTIC_FILENAME_PREFIX}{clsDefines.FILE_EXT_PDF}";

                bool isFSRFound = dbAPI.isFileExist("Search", "Check Attach File", fsrFileName);
                bool isDiagnosticFound = dbAPI.isFileExist("Search", "Check Attach File", diagnosticFileName);

                int fsrColumnIndex = dbFunction.GetListViewColumnIndex(lvwList, "FSR FILE STATUS");
                int diagColumnIndex = dbFunction.GetListViewColumnIndex(lvwList, "DIAG FILE STATUS");

                int signColumnIndex = dbFunction.GetListViewColumnIndex(lvwList, "SIGN COUNT");
                int imageColumnIndex = dbFunction.GetListViewColumnIndex(lvwList, "IMAGE COUNT");

                item.UseItemStyleForSubItems = false;

                if (isFSRFound)
                {
                    dbAPI.checkFileInfo("FSR", "File Info", fsrFileName);

                    string fsrSizeValue = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "Size");

                    fsrFileSize = dbFunction.isValidDescription(fsrSizeValue)
                        ? int.Parse(fsrSizeValue)
                        : 0;

                    isFSRFound = fsrFileSize > 0;
                }

                if (isDiagnosticFound)
                {
                    dbAPI.checkFileInfo("FSR", "File Info", diagnosticFileName);

                    string diagnosticSizeValue = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "Size");

                    diagnosticFileSize = dbFunction.isValidDescription(diagnosticSizeValue)
                        ? int.Parse(diagnosticSizeValue)
                        : 0;

                    isDiagnosticFound = diagnosticFileSize > 0;
                }

                item.SubItems.Add(
                    isFSRFound
                    ? $"{clsIcons.FOUND} {clsDefines.MSG_FOUND}"
                    : $"{clsIcons.NOT_FOUND} {clsDefines.MSG_NOT_FOUND}"
                );

                if (fsrColumnIndex >= 0)
                {
                    item.SubItems[fsrColumnIndex].ForeColor =
                        isFSRFound ? Color.Green : Color.Red;
                }

                item.SubItems.Add(
                    isDiagnosticFound
                    ? $"{clsIcons.FOUND} {clsDefines.MSG_FOUND}"
                    : $"{clsIcons.NOT_FOUND} {clsDefines.MSG_NOT_FOUND}"
                );

                if (diagColumnIndex >= 0)
                {
                    item.SubItems[diagColumnIndex].ForeColor =
                        isDiagnosticFound ? Color.Green : Color.Red;
                }

                string pJSONStringCount = dbAPI.checkFileInfo("View", "File Count", serviceno);

                signCount = 0;
                imageCount = 0;

                if (dbFunction.isValidDescription(pJSONStringCount))
                {
                    signCount = int.Parse(dbAPI.GetValueFromJSONString(pJSONStringCount, clsDefines.TAG_PngCount));
                    imageCount = int.Parse(dbAPI.GetValueFromJSONString(pJSONStringCount, clsDefines.TAG_JpgCount));
                }

                item.SubItems.Add($"{signCount}");
                item.SubItems.Add($"{imageCount}");

                item.SubItems[signColumnIndex].ForeColor = (signCount > 0 ? Color.Black : Color.Red);
                item.SubItems[imageColumnIndex].ForeColor = (imageCount > 0 ? Color.Black : Color.Red);

                // FSR Mode
                item.SubItems.Add(dbFunction.isValidID(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MobileID)) ? clsDefines.DIGITAL_FSR : clsDefines.MANUAL_FSR);

                // summary count
                if (isFSRFound) fsrFoundCount++;
                else fsrNotFoundCount++;

                if (isDiagnosticFound) diagFoundCount++;
                else diagNotFoundCount++;

                string tatStatus = dbAPI.GetValueFromJSONString(pJSONString, "TATStatus");
                string actionMade = dbAPI.GetValueFromJSONString(pJSONString, "ActionMade");

                item.SubItems.Add(tatStatus);
                item.SubItems.Add(actionMade);

                lvwList.Items.Add(item);

                i++;

            }

            dbFunction.ListViewAlternateBackColor(lvwList);

            lvwList.EndUpdate();

            // display summary count
            lblFSRFound.Text = $"{fsrFoundCount}";
            lblFSRNotFound.Text = $"{fsrNotFoundCount}";
            lblFSRTotal.Text = $"{fsrFoundCount + fsrNotFoundCount}";

            lblDiagFound.Text = $"{diagFoundCount}";
            lblDiagNotFound.Text = $"{diagNotFoundCount}";
            lblDiagTotal.Text = $"{diagFoundCount + diagNotFoundCount}";

            Cursor.Current = Cursors.Default;
        }

        private void InitDateRange()
        {
            dteDateFrom.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteDateFrom, clsFunction.sStandardDateDefault);

            dteDateTo.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteDateTo, clsFunction.sStandardDateDefault);

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            clsSearch.ClassParticularID = 0;
            if (!cboFEName.Text.Equals(clsFunction.sDefaultSelect))
            {
                int rowIndex = cboFEName.SelectedIndex - 1;

                if (rowIndex >= 0 && clsArray.ID.Length > rowIndex)
                {
                    clsSearch.ClassParticularID = int.Parse(clsArray.ID[rowIndex]);
                }
            }

            Debug.WriteLine($"ClassParticularID=[{clsSearch.ClassParticularID}]");

            if (!dbFunction.fPromptConfirmation(
                "Are you sure you want to execute the filter criteria below?" + "\n\n" +
                " > Field Engineer : " + cboFEName.Text + "\n" +
                " > FSR Mode     :" + cboFSRModeType.Text + "\n" +
                " > Service Type : " + cboSearchServiceType.Text + "\n" +
                " > Date From    : " + dteDateFrom.Value.ToString("MMM-dd-yyyy") + "\n" +
                " > Date To      : " + dteDateTo.Value.ToString("MMM-dd-yyyy") + "\n\n" +
                "Do you want to continue?"
            )) return;

            dbFunction.ClearListViewItems(lvwList);

            clsSearch.ClassServiceTypeID = 0;
            clsSearch.ClassJobType = 0;
            if (!cboSearchServiceType.Text.Equals(clsFunction.sDefaultSelect))
            {
                // Get Info
                dbAPI.ExecuteAPI("GET", "Search", "Service Type Info", cboSearchServiceType.Text, "Get Info Detail", "", "GetInfoDetail");

                if (dbAPI.isNoRecordFound() == false)
                {
                    clsSearch.ClassServiceTypeID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0));
                    clsSearch.ClassJobType = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5));

                }
            }

            clsSearch.ClassDateFrom = dteDateFrom.Value.ToString("yyyy-MM-dd");
            clsSearch.ClassDateTo = dteDateTo.Value.ToString("yyyy-MM-dd");

            clsSearch.ClassSearchString = txtSearch.Text.Trim();

            loadData();

            lblResultList.Text = $"{gReportListHeader} ({lvwList.Items.Count})";
        }

        private bool validateCompression()
        {
            string basePath = $"{dbFile.sArchivePath}\\{clsSearch.ClassBankCode}";
            dbFile.CheckFolder(basePath);

            int zipFileEntries;

            string zipFileName =
                $"{clsSearch.ClassBankCode}_{gFileNamePrefix}_" +
                $"{dteDateFrom.Value:yyyy-MM-dd}_" +
                $"{dteDateTo.Value:yyyy-MM-dd}.zip";

            string zipFilePath = Path.Combine(basePath, zipFileName);

            string[] filesToZip = Directory.GetFiles(basePath, "*_fsr.pdf");

            if (File.Exists(zipFilePath))
            {
                long fileSize = new FileInfo(zipFilePath).Length;
                try
                {
                    using (ZipArchive zip = ZipFile.OpenRead(zipFilePath))
                    {
                        zipFileEntries = zip.Entries.Count;
                    }
                    if (zipFileEntries == 0)
                    {
                        DialogResult deleteEmptyCompressedFile = MessageBox.Show(
                            $"An empty compressed file with the filename of {zipFileName} already exists \n\n" +
                            $"Delete the file and continue?",
                            "File Already Exists",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                        );

                        if (deleteEmptyCompressedFile == DialogResult.No) return false;

                        try
                        {
                            File.Delete(zipFilePath);
                            return true;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(
                            $"Error deleting empty file: {ex.Message} please delete the file manually \n\n" +
                            $"{zipFilePath}",
                            "File deletion error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                            return false;
                        }

                    }
                    else
                    {
                        MessageBox.Show(
                        $"A compressed file with the filename of {zipFileName} already exists",
                        "File already exists",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                        );

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                    $"Error deleting empty file: {ex.Message} please delete the file manually to continue \n\n" +
                    $"{zipFilePath}",
                    "File deletion error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                    return false;
                }


            }
        
            return true;
        }

        private void downloadFile()
        {
            Cursor.Current = Cursors.WaitCursor;

            if (!validateCompression()) return;

            try
            {
                string localBasePath = $"{dbFile.sArchivePath}\\{clsSearch.ClassBankCode}";

                dbFile.CheckFolder(localBasePath); // create folder when not exist

                ftp ftpClient = new ftp(
                    $"{clsGlobalVariables.strFTPURL}/fsr/{clsSearch.ClassBankCode}",
                    clsGlobalVariables.strFTPUserName,
                    clsGlobalVariables.strFTPPassword
                );

                foreach (ListViewItem item in lvwList.Items)
                {
                    int serviceNoColumnIndex = dbFunction.GetListViewColumnIndex(lvwList, "SERVICE NO.");
                    int fsrColumnIndex = dbFunction.GetListViewColumnIndex(lvwList, "FSR FILE STATUS");
                    int diagColumnIndex = dbFunction.GetListViewColumnIndex(lvwList, "DIAG FILE STATUS");

                    string serviceNo = item.SubItems[serviceNoColumnIndex].Text;

                    string fsrStatusRaw = dbFunction.StripIcon(item.SubItems[fsrColumnIndex].Text);

                    Debug.WriteLine($"[ROW] {serviceNo} | RAW STATUS: {fsrStatusRaw}");

                    bool isFSRFound = fsrStatusRaw == clsDefines.MSG_FOUND;

                    if (isFSRFound)
                    {
                        string fsrFileName =
                            serviceNo + clsDefines.FSR_FILENAME_PREFIX + clsDefines.FILE_EXT_PDF;

                        string localFsrPath = Path.Combine(localBasePath, fsrFileName);

                        Debug.WriteLine($"[DOWNLOAD] {fsrFileName}");

                        ftpClient.download(fsrFileName, localFsrPath);
                    }
                    else
                    {
                        Debug.WriteLine($"[SKIP] {serviceNo}");
                    }
                }

                ftpClient.disconnect();
                compressFiles();

                Cursor.Current = Cursors.WaitCursor;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"FTP Error: {ex.Message}");
                MessageBox.Show(
                    $"Download failed: {ex.Message}",
                    "FTP Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void compressFiles()
        {
            Cursor.Current = Cursors.WaitCursor;


            try
            {
                string localBasePath = $"{dbFile.sArchivePath}\\{clsSearch.ClassBankCode}";

                string zipFileName =
                    $"{clsSearch.ClassBankCode}_{gFileNamePrefix}_" +
                    $"{dteDateFrom.Value:yyyy-MM-dd}_" +
                    $"{dteDateTo.Value:yyyy-MM-dd}.zip";

                string zipFilePath = Path.Combine(localBasePath, zipFileName);

                string[] filesToZip = Directory.GetFiles(localBasePath, "*_fsr.pdf");
                
                using (ZipArchive zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                {
                    foreach (string file in filesToZip)
                    {
                        ZipArchiveEntry entry = zip.CreateEntry(Path.GetFileName(file));

                        using (Stream entryStream = entry.Open())
                        using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                        {
                            fileStream.CopyTo(entryStream);
                        }

                        Debug.WriteLine($"Added to zip: {Path.GetFileName(file)}");
                    }
                }

                foreach (string file in filesToZip)
                {
                    try
                    {
                        if (File.Exists(file))
                        {
                            File.Delete(file);
                            Debug.WriteLine($"Deleted temp file: {Path.GetFileName(file)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Delete failed: {file} | {ex.Message}");
                    }
                }

                Debug.WriteLine($"Zip created: {zipFilePath}");

                MessageBox.Show(
                    $"Files compressed successfully.\n\n{zipFileName}",
                    "Archive Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Compress Error: {ex.Message}");

                MessageBox.Show(
                    $"Compression failed: {ex.Message}",
                    "Compression Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            Cursor.Current = Cursors.Default;
        }

        private void btnCompress_Click(object sender, EventArgs e)
        {
            if (lvwList.Items.Count == 0)
            {
                dbFunction.SetMessageBox("No record to process.", lblHeader.Text, clsFunction.IconType.iInformation);
                return;
            }

            if (!dbFunction.fPromptConfirmation(
                "Compress files to ZIP?\n\n" +
                " > Field Engineer : " + cboFEName.Text + "\n" +
                " > FSR Mode     :" + cboFSRModeType.Text + "\n" +
                " > Service Type : " + cboSearchServiceType.Text + "\n" +
                " > Date From    : " + dteDateFrom.Value.ToString("MMM-dd-yyyy") + "\n" +
                " > Date To      : " + dteDateTo.Value.ToString("MMM-dd-yyyy") + "\n\n" +
                "Do you want to continue?"
            )) return;

            downloadFile();
        }

        private void frmServiceArchive_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void initArchivePath()
        {
            txtArchivePath.Text = Path.Combine(dbFile.sArchivePath, clsSearch.ClassBankCode);
        }

        private void btnOpenFolderPath_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbFile.OpenFolder(txtArchivePath.Text);

            Cursor.Current = Cursors.Default;
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.Items.Count > 0)
            {
                string pSelectedRow = dbFunction.GetListViewSelectedRow(lvwList, 0);
                string jsonResult = dbFunction.genJSONFormat(lvwList, lvwList.SelectedIndices[0], "", "");

                // Pass JSON to popup window
                frmPopUpInfo frm = new frmPopUpInfo(jsonResult);
                frm.ShowDialog();
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (lvwList.Items.Count == 0)
            {
                dbFunction.SetMessageBox("No record to export.", lblHeader.Text, clsFunction.IconType.iInformation);
                return;
            }

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                DataTable dt = new DataTable();

                foreach (ColumnHeader col in lvwList.Columns)
                    dt.Columns.Add(col.Text);

                int fsrColIdx = dbFunction.GetListViewColumnIndex(lvwList, "FSR FILE STATUS");
                int diagColIdx = dbFunction.GetListViewColumnIndex(lvwList, "DIAG FILE STATUS");


                foreach (ListViewItem item in lvwList.Items)
                {
                    DataRow row = dt.NewRow();

                    for (int i = 0; i < lvwList.Columns.Count; i++)
                    {
                        string cellValue = item.SubItems.Count > i ? item.SubItems[i].Text : "";

                        if (i == fsrColIdx || i == diagColIdx)
                            cellValue = dbFunction.StripIcon(cellValue);

                        row[i] = cellValue;
                    }
                    dt.Rows.Add(row);
                }

                string pFileName = $"{clsSearch.ClassBankCode}_{gFileNamePrefix}_{dteDateFrom.Value:dd-MM-yyyy}_{dteDateTo.Value:dd-MM-yyyy}{(cboFEName.Text.Equals(clsFunction.sDefaultSelect) ? "" : $"_{cboFEName.Text}")}{(cboFSRModeType.Text.Equals(clsFunction.sDefaultSelect) ? "" : $"_{cboFSRModeType.Text}")}.xlsx";

                ExportCustomDataToExcel(
                    pFileName,
                    new[] { dt },
                    new[] { "FSR List" },
                    new[] { Color.ForestGreen }
                );
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox($"Export failed: {ex.Message}", lblHeader.Text, clsFunction.IconType.iError);
            }

            Cursor.Current = Cursors.Default;
        }
    }
}
