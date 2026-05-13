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
using MIS.Global;
using System.IO;
using System.IO.Compression;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace MIS
{
    public partial class frmServiceArchive : Form
    {

        private clsAPI dbAPI;        
        private clsFunction dbFunction;
        private clsFile dbFile;

        private const string gReportListHeader = "FSR REPORT LIST";

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

            lblResultList.Text = gReportListHeader;

            initArchivePath();

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

        }
        
        private void loadData()
        {
            int i = 0;
            int iLineNo = 0;

            int fsrFoundCount = 0;
            int fsrNotFoundCount = 0;

            int diagFoundCount = 0;
            int diagNotFoundCount = 0;

            Cursor.Current = Cursors.WaitCursor;

            Debug.WriteLine("--loadData--");
            
            lvwList.Items.Clear();

            clsSearch.ClassSearchValue = $"{clsSearch.ClassJobType}{clsDefines.gPipe}" +
                                        $"{clsDefines.gZero}{clsDefines.gPipe}" +
                                        $"{clsSearch.ClassDateFrom}{clsDefines.gPipe}" +
                                        $"{clsSearch.ClassDateTo}{clsDefines.gPipe}" +
                                        $"{clsDefines.gZero}";

            Debug.WriteLine("clsSearch.ClassSearchValue=" + clsSearch.ClassSearchValue);

            dbAPI.ExecuteAPI("GET", "View", "FSR Service Detail", clsSearch.ClassSearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if(dbAPI.isNoRecordFound())
            {
                MessageBox.Show(
                    $"No record found for date range" +
                    "\n\n" +
                    $"{clsSearch.ClassDateFrom} to {clsSearch.ClassDateTo}",
                    "Archive search error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                ); return;
            }
            else
            {
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

                    item.UseItemStyleForSubItems = false;

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
                    if (dbFunction.isValidDescription(pJSONStringCount))
                    {
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONStringCount, clsDefines.TAG_PngCount));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONStringCount, clsDefines.TAG_JpgCount));
                    }
                    else
                    {
                        item.SubItems.Add(clsFunction.sZero);
                        item.SubItems.Add(clsFunction.sZero);
                    }

                    // FSR Mode
                    item.SubItems.Add(dbFunction.isValidID(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MobileID)) ? clsDefines.DIGITAL_FSR : clsDefines.MANUAL_FSR);

                    // summary count
                    if (isFSRFound) fsrFoundCount++;
                    else fsrNotFoundCount++;

                    if (isDiagnosticFound) diagFoundCount++;
                    else diagNotFoundCount++;

                    lvwList.Items.Add(item);

                    i++;

                }

                dbFunction.ListViewAlternateBackColor(lvwList);

                // display summary count
                lblFSRFound.Text = $"{fsrFoundCount}";
                lblFSRNotFound.Text = $"{fsrNotFoundCount}";
                lblFSRTotal.Text = $"{fsrFoundCount + fsrNotFoundCount}";

                lblDiagFound.Text = $"{diagFoundCount}";
                lblDiagNotFound.Text = $"{diagNotFoundCount}";
                lblDiagTotal.Text = $"{diagFoundCount + diagNotFoundCount}";

            }

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
            if (!dbFunction.fPromptConfirmation(
                "Are you sure you want to execute the filter criteria below?" + "\n\n" +
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
            
            loadData();

            lblResultList.Text = $"{gReportListHeader} ({lvwList.Items.Count})";
        }

        public void downloadFile()
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                string localBasePath = $"{dbFile.sArchivePath}\\{clsSearch.ClassBankCode}";

                dbFile.CheckFolder(localBasePath); // create folder when not exist

                ftp ftpClient = new ftp(
                    $"{clsGlobalVariables.strFTPURL}/fsr/{clsSearch.ClassBankDisplayName.ToLower()}",
                    clsGlobalVariables.strFTPUserName,
                    clsGlobalVariables.strFTPPassword
                );

                foreach (ListViewItem item in lvwList.Items)
                {
                    int serviceNoColumnIndex = dbFunction.GetListViewColumnIndex(lvwList, "SERVICE NO.");
                    int fsrColumnIndex = dbFunction.GetListViewColumnIndex(lvwList, "FSR FILE STATUS");
                    int diagColumnIndex = dbFunction.GetListViewColumnIndex(lvwList, "DIAG FILE STATUS");

                    string serviceNo = item.SubItems[serviceNoColumnIndex].Text;

                    string fsrStatusRaw =  dbFunction.StripIcon(item.SubItems[fsrColumnIndex].Text);

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

                dbFile.CheckFolder(localBasePath); // create folder when not exist

                string zipFileName =
                    $"FSR_ARCHIVE_" +
                    $"{dteDateFrom.Value:yyyyMMdd}_" +
                    $"{dteDateTo.Value:yyyyMMdd}.zip";

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
            finally
            {
                
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
            txtArchivePath.Text = Path.Combine(dbFile.sArchivePath,clsSearch.ClassBankCode);
        }

        private void btnOpenFolderPath_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbFile.OpenFolder(txtArchivePath.Text);

            Cursor.Current = Cursors.Default;
        }
    }
}
