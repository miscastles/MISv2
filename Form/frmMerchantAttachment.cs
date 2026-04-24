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
using MIS.Controller;
using static MIS.Function.AppUtilities;
using MIS.Enums;

namespace MIS
{
    public partial class frmMerchantAttachment : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        private clsFile dbFile;
        private clsINI dbINI;
        public static string sHeader;

        // Controller
        private ImportMasterController _mImportMasterController;

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

        public frmMerchantAttachment()
        {
            InitializeComponent();

            // Initialize the controller object
            _mImportMasterController = new ImportMasterController();
        }

        private void frmMerchantAttachment_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFile = new clsFile();
            dbINI = new clsINI();
            dbFunction = new clsFunction();

            this.Text = lblHeader.Text = sHeader + " " + dbFunction.AddBracketStartEnd(clsSearch.ClassReferenceNo);

            initPath();
            loadMSP();
            loadMerchant();
            loadSales();
            loadImportMaster();

            Cursor.Current = Cursors.Default;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClearFile_Click(object sender, EventArgs e)
        {
            ucAttachDocument1.clearEntry();            
            ucAttachDocument1.clearList();
        }

        private void btnUploadFile_Click(object sender, EventArgs e)
        {
            if (!dbFunction.fPromptConfirmation("Are you sure to upload upload file(s)?")) return;

            Cursor.Current = Cursors.WaitCursor;

            List<Tuple<string, string, string, string>> fileDetails;
            ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);

            try
            {
                string FilePath;                
                string FileName;
                string Remarks;
                string DateReceived;
                string oldFileName;
                string Prefix;

                // ucAttachDocument1
                ucAttachDocument1.addToList();
                fileDetails = ucAttachDocument1.getFileDetails();
                if (fileDetails != null)
                {
                    foreach (var file in fileDetails)
                    {
                        FilePath = file.Item1;
                        oldFileName = FileName = file.Item2;
                        Remarks = file.Item3;
                        DateReceived = file.Item4;

                        // Rename                        
                        Prefix =  dbFunction.padLeftChar(dbFunction.CheckAndSetNumericValue(txtMSPNo.Text), clsFunction.sZero, 6);
                        FileName = Prefix + "_" + FileName;
                        dbFile.CopyAndRenameFile(FilePath, txtDownloadPath.Text + FileName);
                        
                        // upload
                        if (file != null)
                        {   
                            ftpClient.delete(txtRemotePath.Text + FileName);
                            ftpClient.upload(txtRemotePath.Text + FileName, txtDownloadPath.Text + FileName);

                            // save uploaded file
                            saveImportMaster(FilePath, oldFileName, FileName, DateReceived, Remarks, (int)FileType.MSP);
                        }
                    }
                }
                
                ftpClient.disconnect(); // ftp disconnect

                dbFunction.SetMessageBox("Upload file(s) complete.", "Support document", clsFunction.IconType.iInformation);

                btnClearFile_Click(this, e);

                loadImportMaster();

            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox("Exceptional error " + ex.Message, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }
            
            Cursor.Current = Cursors.Default;

        }

        private void loadMSP()
        {
            txtMSPNo.Text = clsSearch.ClassMSPNo.ToString();
        }
        private void loadMerchant()
        {
            dbAPI.ExecuteAPI("GET", "Search", "Particular Info", clsSearch.ClassMerchantID.ToString(), "Get Info Detail", "", "GetInfoDetail");

            if (dbAPI.isNoRecordFound() == false)
            {
                txtMerchName.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                txtMerchAddress.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);                
                txtMerchContactPerson.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 11);
                txtMerchContactNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);
                
            }
        }

        private void loadSales()
        {
            dbAPI.ExecuteAPI("GET", "Search", "Particular Info", clsSearch.ClassSubmitID.ToString(), "Get Info Detail", "", "GetInfoDetail");

            if (dbAPI.isNoRecordFound() == false)
            {
                txtSalesName.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);                
                txtSalesContactNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);
                txtSalesEmail.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 14);

            }
        }

        private void btnDownloadFile_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidDescriptionEntry(txtRemotePath.Text, "Remote path" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtDownloadPath.Text, "Download path" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtFileName.Text, "Filename" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            if (!dbFunction.fPromptConfirmation("Are you sure to download " + dbFunction.AddBracketStartEnd(txtFileName.Text) + "?")) return;

            Cursor.Current = Cursors.WaitCursor;

            ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
            ftpClient.download(txtRemotePath.Text + txtFileName.Text, txtDownloadPath.Text + txtFileName.Text);
            ftpClient.disconnect();

            dbFunction.SetMessageBox("Download file complete." + 
                "\n\n" + 
                "Path: " + dbFunction.AddBracketStartEnd(txtDownloadPath.Text) + "\n" +
                "File:" + dbFunction.AddBracketStartEnd(txtFileName.Text), clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);

            if (chkOpenFile.Checked)
            {
                dbFunction.SetMessageBox("File " + dbFunction.AddBracketStartEnd(txtFileName.Text) + " will be open directly.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);

                dbFile.openFile(txtDownloadPath.Text + txtFileName.Text);
            }

            btnClear_Click(this, e);

            Cursor.Current = Cursors.Default;
        }

        private void saveImportMaster(string pFilePath, string pOrgFileName, string pFileName, string pReceivedDate, string pRemarks, int pFileType)
        {
            string sSQL = "";

            var data = new
            {
                TransDate = dbFunction.getCurrentDate(),
                TransTime = dbFunction.getCurrentTime(),
                ControlNo = dbFunction.CheckAndSetNumericValue(txtMSPNo.Text),
                UserID = clsSearch.ClassCurrentUserID,
                FilePath = pFilePath,
                OrgFileName = pOrgFileName,
                FileName = pFileName,
                FileSize = dbFile.getFileSize(pFilePath),
                ProcessedBy = clsUser.ClassUserFullName,
                ProcessedDateTime = dbFunction.getCurrentDateTime(),
                Remarks = pRemarks,
                ReceivedDate = pReceivedDate,
                FileType = pFileType
                
            };

            sSQL = IFormat.Insert(data);

            Debug.WriteLine("saveImportMaster" + sSQL);

            dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 0);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "Import Master", sSQL, "InsertMaintenanceMaster");
            
        }

        private void loadImportMaster()
        {
            int lineno = 0;
            int i = 0;
            List<ImportMasterController> mList = null;

            dbFunction.ClearListViewItems(lvwList);

            mList = _mImportMasterController.getDetailList(dbFunction.CheckAndSetNumericValue(txtMSPNo.Text) + clsDefines.gPipe + (int)FileType.MSP);
            if (mList != null)
            {
                if (mList != null)
                {
                    foreach (var itemData in mList)
                    {
                        lineno++;
                        ListViewItem item = new ListViewItem(lineno.ToString());
                        item.SubItems.Add(itemData.TransNo.ToString());
                        item.SubItems.Add(itemData.ReceivedDate);
                        item.SubItems.Add(itemData.FileName);
                        item.SubItems.Add(itemData.FileSize);
                        item.SubItems.Add(itemData.ProcessedBy);
                        item.SubItems.Add(itemData.ProcessedDate);
                        item.SubItems.Add(itemData.Remarks);

                        lvwList.Items.Add(item);

                    }

                    dbFunction.ListViewAlternateBackColor(lvwList);

                }
            }

        }

        private void btnRefreshFile_Click(object sender, EventArgs e)
        {
            loadImportMaster();
        }

        private void btnDeleteFile_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidDescriptionEntry(txtRemotePath.Text, "Remote path" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;
            if (!dbFunction.isValidDescriptionEntry(txtFileName.Text, "Filename" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            if (!dbFunction.fPromptConfirmation("Are you sure to delete uploaded file?")) return;

            ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                ftpClient.delete(txtRemotePath.Text + txtFileName.Text);

                dbAPI.ExecuteAPI("DELETE", "Delete", "Import Master", dbFunction.CheckAndSetNumericValue(txtControlNo.Text) + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtMSPNo.Text), "Import Master", "", "DeleteCollectionDetail");
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox("Exceptional error " + ex.Message, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }

            loadImportMaster();

            btnClear_Click(this, e);

            Cursor.Current = Cursors.Default;
            
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.Items.Count > 0)
            {
                string pSelectedRow = dbFunction.GetListViewSelectedRow(lvwList, 0);
                Debug.WriteLine("pSelectedRow=\n" + pSelectedRow);

                txtControlNo.Text = dbFunction.GetSearchValue("ID");
                
                txtFileName.Text = dbFunction.GetSearchValue("FileName");
                txtSize.Text = dbFunction.GetSearchValue("Size");
                txtProcessedBy.Text = dbFunction.GetSearchValue("ProcessedBy");
                txtProcessedDate.Text = dbFunction.GetSearchValue("ProcessedDate");
                txtReceivedDate.Text = dbFunction.GetSearchValue("Receive Date");
                txtRemarks.Text = dbFunction.GetSearchValue("Remarks");

                initPath();
            }
        }

        private void lvwList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwList.SelectedItems.Count > 0)
            {
                txtLineNo.Text = lvwList.SelectedItems[0].Text; ;
            }
        }

        private void initPath()
        {
            string sRemotePath = "\\upload\\msp\\";
            string sLocaPath = "C:\\CASTLESTECH_MIS\\DOWNLOAD\\";

            txtRemotePath.Text = sRemotePath;
            txtDownloadPath.Text = sLocaPath;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtLineNo.Text = txtControlNo.Text = txtFileName.Text = txtSize.Text = txtReceivedDate.Text = txtProcessedDate.Text = txtProcessedBy.Text = txtRemarks.Text = txtRemotePath.Text = txtDownloadPath.Text = clsFunction.sNull;
        }

        private void frmMerchantAttachment_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }
    }
}
