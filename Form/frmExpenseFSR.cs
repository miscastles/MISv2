using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIS
{
    public partial class frmExpenseFSR : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        private clsFile dbFile;
        private clsReceiptImageProcessor dbReceiptImageProcessor;

        bool fEdit;
        bool fNewExpense;

        string sSelectedImagePath = "";

        private ListViewItem pendingImageItem;
        private JObject pendingImageData;

        string pExpenseFTPHost = $"{clsGlobalVariables.strFTPURL}/{clsGlobalVariables.strFTPUploadPath}/expenses/{clsSearch.ClassBankCode}";

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

        private class ExpenseSummaryGrid : DataGridView
        {
            public ExpenseSummaryGrid()
            {
                this.AutoGenerateColumns = false;
                this.AllowUserToAddRows = false;
                this.AllowUserToDeleteRows = false;
                this.ReadOnly = true;
                this.RowHeadersVisible = false;

                InitializeColumns();
            }

            public static void Configure(DataGridView grid)
            {
                grid.AutoGenerateColumns = false;
                grid.AllowUserToAddRows = false;
                grid.AllowUserToDeleteRows = false;
                grid.ReadOnly = true;
                grid.RowHeadersVisible = false;
                // size fiill
                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (!grid.Columns.Contains("dgvcol_ExpensesType"))
                {
                    grid.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "dgvcol_ExpensesType",
                        HeaderText = "TYPE",
                        Width = 160,
                        ReadOnly = true
                    });
                }

                if (!grid.Columns.Contains("dgvcol_ExpensesAmount"))
                {
                    grid.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "dgvcol_ExpensesAmount",
                        HeaderText = "TOTAL AMOUNT",
                        Width = 85,
                        ReadOnly = true,
                        DefaultCellStyle = new DataGridViewCellStyle
                        {
                            Format = "N2",
                            Alignment = DataGridViewContentAlignment.MiddleRight
                        }
                    });
                }
            }

            private void InitializeColumns()
            {
                Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "dgvcol_ExpensesType",
                    HeaderText = "TYPE",
                    Width = 180,
                    ReadOnly = true
                });

                Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "dgvcol_ExpensesAmount",
                    HeaderText = "AMOUNT",
                    Width = 85,
                    ReadOnly = true,
                    DefaultCellStyle =

                    {
                        Format = "N2",
                        Alignment = DataGridViewContentAlignment.MiddleRight
                    }
                });
            }

            public static decimal Fill(DataGridView grid, ListView expenseList)
            {
                decimal dTotalExpenses = 0;

                grid.Rows.Clear();

                foreach (ListViewItem item in expenseList.Items)
                {
                    decimal dExpenseAmount;

                    if (!decimal.TryParse(item.SubItems[4].Text, out dExpenseAmount))
                    {
                        dExpenseAmount = 0;
                    }

                    int iIndex = grid.Rows.Add();

                    grid.Rows[iIndex].Cells["dgvcol_ExpensesType"].Value = item.SubItems[3].Text;
                    grid.Rows[iIndex].Cells["dgvcol_ExpensesAmount"].Value = dExpenseAmount;

                    dTotalExpenses += dExpenseAmount;
                }

                int iTotalIndex = grid.Rows.Add();

                grid.Rows[iTotalIndex].Cells["dgvcol_ExpensesType"].Value = "TOTAL";
                grid.Rows[iTotalIndex].Cells["dgvcol_ExpensesAmount"].Value = dTotalExpenses;

                grid.Rows[iTotalIndex].DefaultCellStyle.Font = new Font(grid.Font, FontStyle.Bold);
                grid.Rows[iTotalIndex].Cells["dgvcol_ExpensesAmount"].Style.ForeColor = Color.RoyalBlue;

                return dTotalExpenses;
            }
        }

        public frmExpenseFSR()
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            dbFile = new clsFile();

            InitializeComponent();

            dbFunction.setDoubleBuffer(lvwExpenseList, true);
            dbFunction.setDoubleBuffer(lvwExpenseImages, true);

            dbAPI.FillComboBoxExpenseType(cboExpenseType);

            dbReceiptImageProcessor = new clsReceiptImageProcessor();

            ExpenseSummaryGrid.Configure(dgvSummary);
        }

        private void frmExpenseFSR_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            fEdit = false;
            fNewExpense = false;
    
            lblHeader.Text = $"EXPENSES - FSR [ {clsSearch.ClassBankDisplayName} | {clsSystemSetting.ClassSystemEnvironment} ]";

            ResetImageFields();
            InitButtons();

            Cursor.Current = Cursors.Default;
        }

        private void InitButtons()
        {
            txtExpenseReferenceNo.ReadOnly = true;

            if (dbFunction.isValidID(txtServiceNo.Text))
            {
                btnNew.Enabled = true;
            }
            else
            {
                btnNew.Enabled = false;
            }

            if (fNewExpense)
            {
                btnAddExpense.Enabled = true;
                btnSave.Enabled = false;
                btnDeleteExpense.Enabled = false;
                btnClearExpense.Enabled = true;

                dtExpenseDate.Enabled = true;
                cboExpenseType.Enabled = true;
                txtExpenseAmount.Enabled = false;
                txtRemarks.ReadOnly = false;
                btnAddSelectImage.Enabled = true;
            }
            else if (fEdit)
            {
                btnAddExpense.Enabled = false;
                btnSave.Enabled = true;
                btnDeleteExpense.Enabled = true;
                btnClearExpense.Enabled = true;

                dtExpenseDate.Enabled = true;
                cboExpenseType.Enabled = false;
                txtExpenseAmount.Enabled = true;
                txtRemarks.ReadOnly = false;
                btnAddSelectImage.Enabled = true;
            }
            else
            {
                btnAddExpense.Enabled = false;
                btnSave.Enabled = false;
                btnDeleteExpense.Enabled = false;
                btnClearExpense.Enabled = false;

                dtExpenseDate.Enabled = false;
                cboExpenseType.Enabled = false;
                txtExpenseAmount.Enabled = false;
                txtRemarks.ReadOnly = true;
                btnAddSelectImage.Enabled = false;
            }
        }

        private void FillExpenseList()
        {
            int i = 0;
            int iLineNo = 0;

            lvwExpenseList.Items.Clear();
            txtTotalExpenses.Text = "0.00";

            dbAPI.ExecuteAPI("GET", "View", "Expenses Transaction Detail", txtServiceNo.Text + clsDefines.gPipe + txtIRIDNo.Text, "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound()) return;

            if (clsArray.ID == null || clsArray.detail_info == null) return;

            while (clsArray.ID.Length > i)
            {
                iLineNo++;

                string pExpenseID = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ExpensesID");
                string pExpenseReferenceNo =dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ExpensesReferenceNo");
                string pExpenseType = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ExpenseType");
                string pAmount = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ExpensesAmount");
                string pRemarks = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ExpensesDescription");
                string pExpenseDate = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ExpensesDate");

                decimal dExpenseAmount;

                if (!decimal.TryParse(pAmount, out dExpenseAmount))
                {
                    dExpenseAmount = 0;
                }

                ListViewItem item = new ListViewItem(pExpenseID);

                item.SubItems.Add(iLineNo.ToString());
                item.SubItems.Add(pExpenseReferenceNo);
                item.SubItems.Add(pExpenseType);
                item.SubItems.Add(dExpenseAmount.ToString("N2"));
                item.SubItems.Add(pRemarks);
                item.SubItems.Add(pExpenseDate);

                item.Tag = clsArray.detail_info[i];

                lvwExpenseList.Items.Add(item);

                i++;
            }

            dbFunction.ListViewAlternateBackColor(lvwExpenseList);

            ComputeTotalExpenses();
        }

        private async Task FillMerchantInfo()
        {
            string pOutParamValue = null;

            bool fSuccessful = await Task.Run(() =>
            {
                dbAPI.ExecuteAPI("GET", "Search", "Merchant Info", clsSearch.ClassMerchantID + clsFunction.sPipe + clsSearch.ClassIRIDNo, "Get Info Detail", "", "GetInfoDetail");

                if (!clsGlobalVariables.isAPIResponseOK) return false;

                if (dbAPI.isNoRecordFound()) return false;

                pOutParamValue = clsSearch.ClassOutParamValue;

                return true;
            });

            if (!fSuccessful) return;

            // MERCHANT DATA    
            txtMerchant.Text = dbFunction.getDelimitedString(pOutParamValue, clsFunction.cPipe, 1);
            txtMerchantAddress.Text = dbFunction.getDelimitedString(pOutParamValue, clsFunction.cPipe, 2);
            txtMerchantCity.Text = dbFunction.getDelimitedString(pOutParamValue, clsFunction.cPipe, 3);
            txtMerchantRegion.Text = dbFunction.getDelimitedString(pOutParamValue, clsFunction.cPipe, 4);
            txtIRIDNo.Text = dbFunction.getDelimitedString(pOutParamValue, clsFunction.cPipe, 16);
            // clientid
            // clientname
        }

        private async Task FillServicingInfo()
        {
            string pOutParamValue = null;

            bool fSuccessful = await Task.Run(() =>
            {
                dbAPI.ExecuteAPI("GET", "Search", "Servicing Info", clsSearch.ClassServiceNo.ToString(), "Get Info Detail", "", "GetInfoDetail");

                if (!clsGlobalVariables.isAPIResponseOK) return false;

                if (dbAPI.isNoRecordFound()) return false;

                pOutParamValue = clsSearch.ClassOutParamValue;

                return true;
            });

            if (!fSuccessful) return;

            txtServiceNo.Text = clsSearch.ClassServiceNo.ToString();
            txtServiceRequestNo.Text = dbFunction.getDelimitedString(pOutParamValue, clsFunction.cPipe, 2);
            txtServiceReferenceNo.Text = dbFunction.getDelimitedString(pOutParamValue, clsFunction.cPipe, 3);
            txtDispatchedBy.Text = dbFunction.getDelimitedString(pOutParamValue, clsFunction.cPipe, 10);
            txtServiceType.Text = dbFunction.getDelimitedString(pOutParamValue, clsFunction.cPipe, 18);
            txtRequestID.Text = dbFunction.getDelimitedString(pOutParamValue, clsFunction.cPipe, 33);
        }

        private async Task FillFSRInfo()
        {
            string pOutParamValue = null;

            bool fSuccessful = await Task.Run(() =>
            {
                dbAPI.ExecuteAPI("GET", "Search", "FSR Info", clsSearch.ClassServiceNo.ToString(), "Get Info Detail", "", "GetInfoDetail");

                if (!clsGlobalVariables.isAPIResponseOK) return false;

                if (dbAPI.isNoRecordFound()) return false;

                pOutParamValue = clsSearch.ClassOutParamValue;

                return true;
            });

            if (!fSuccessful) return;

            txtFSRDate.Text = dbFunction.getDelimitedString(pOutParamValue, clsFunction.cPipe, 2);
            txtFsrServicedBy.Text = dbFunction.getDelimitedString(pOutParamValue, clsFunction.cPipe, 9);
        }

        private async Task<bool> UpdateExpense()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                string pSearchValue =
                    dbFunction.CheckAndSetNumericValue(txtServiceNo.Text) + clsDefines.gPipe +
                    dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + clsDefines.gPipe +
                    dbFunction.CheckAndSetNumericValue(txtExpenseID.Text) + clsDefines.gPipe +
                    dbFunction.CheckAndSetNumericValue(txtExpenseAmount.Text) + clsDefines.gPipe +
                    dtExpenseDate.Value.ToString("yyyy-MM-dd");

                SetStatus("Updating expense record...");

                await Task.Run(() =>
                {
                    dbAPI.ExecuteAPI("PUT", "Update", "Update Service Expenses Detail", pSearchValue, "", "", "UpdateCollectionDetail"
                    );
                });

                if (!clsGlobalVariables.isAPIResponseOK) return false;

                if (!clsGlobalVariables.isAPIResponseOK) return false;

                return true;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private async Task<bool> DeleteExpenseImages(string pServiceNo, string pExpensesID)
        {
            if (!dbFunction.isValidID(pServiceNo) ||
                !dbFunction.isValidID(pExpensesID))
            {
                return false;
            }

            string pFilePrefix =
                dbFunction.CheckAndSetNumericValue(pServiceNo) + "_" +
                dbFunction.CheckAndSetNumericValue(pExpensesID) + "_";

            ftp ftpClient = new ftp(pExpenseFTPHost, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);

            try
            {
                string[] pFiles = await Task.Run(() => ftpClient.directoryListSimple(""));

                if (pFiles == null || pFiles.Length <= 0) return false;

                Array.Sort(pFiles, StringComparer.OrdinalIgnoreCase);

                if (pFiles == null || !Array.Exists(pFiles, pFile => !string.IsNullOrWhiteSpace(pFile)))
                {
                    return false;
                }

                Array.Sort(pFiles, StringComparer.OrdinalIgnoreCase);

                foreach (string pRemoteEntry in pFiles)
                {
                    string pFileName = Path.GetFileName(pRemoteEntry.TrimEnd('/', '\\'));

                    if (!dbFunction.isValidDescription(pFileName)) continue;

                    SetStatus("Deleting receipt image " + pFileName + "...");
                    await Task.Run(() => ftpClient.delete(pFileName));
                }

                return true;
            }
            finally
            {
                ftpClient.disconnect();
            }
        }

        private async Task FetchData()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                SetStatus("Loading merchant information...");
                await FillMerchantInfo();

                SetStatus("Loading servicing information...");
                await FillServicingInfo();

                SetStatus("Loading FSR information...");
                await FillFSRInfo();
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox(
                    "An error occurred while loading the expense information.\n\n" +
                    "Error: " + ex.Message,
                    "Loading failed",
                    clsFunction.IconType.iError
                );
            }
            finally
            {
                SetStatus(clsDefines.gNull);

                FillExpenseList();
                FillExpenseSummary();

                Cursor.Current = Cursors.Default;
            }
        }

        private async Task<bool> SynchronizeExpenseImages(string pExpensesID)
        {
            string pServiceNo = dbFunction.CheckAndSetNumericValue(txtServiceNo.Text);
            string pExpenseDate = dtExpenseDate.Value.ToString("yyyyMMdd");

            int iImageIndex = 1;

            foreach (ListViewItem item in lvwExpenseImages.Items)
            {
                if (item.Tag == null)
                {
                    dbFunction.SetMessageBox(
                        "An image in the attachment list contains no information.",
                        "Image synchronization failed",
                        clsFunction.IconType.iError
                    );

                    return false;
                }

                JObject imageData = JObject.Parse(item.Tag.ToString());
                
                string pCurrentFTPFileName = Convert.ToString(imageData["FTPFileName"]);
                string pImageSource = Convert.ToString(imageData["ImageSource"]);
                string pRequiredFTPFileName =
                    pServiceNo + "_" +
                    pExpensesID + "_" +
                    pExpenseDate + "_" +
                    iImageIndex.ToString("00") +
                    clsDefines.FILE_EXT_PNG;

                if (dbFunction.isValidDescription(pCurrentFTPFileName))
                {
                    if (!pCurrentFTPFileName.Equals(
                        pRequiredFTPFileName,
                        StringComparison.OrdinalIgnoreCase
                    ))
                    {
                        SetStatus(
                            "Renaming receipt image " +
                            pCurrentFTPFileName + " to " +
                            pRequiredFTPFileName + "..."
                        );

                        bool fRenameSuccessful = await Task.Run(() =>
                        {
                            ftp ftpClient = new ftp(
                                pExpenseFTPHost,
                                clsGlobalVariables.strFTPUserName,
                                clsGlobalVariables.strFTPPassword
                            );

                            try
                            {
                                long pTargetFileSize = ftpClient.getFileSize(pRequiredFTPFileName);

                                if (pTargetFileSize > 0)
                                {
                                    return false;
                                }

                                ftpClient.rename(pCurrentFTPFileName, pRequiredFTPFileName
                                );

                                long pRenamedFileSize = ftpClient.getFileSize(pRequiredFTPFileName);
                                long pOriginalFileSize = ftpClient.getFileSize(pCurrentFTPFileName);

                                return pRenamedFileSize > 0 && pOriginalFileSize <= 0;
                            }
                            finally
                            {
                                ftpClient.disconnect();
                            }
                        });

                        if (!fRenameSuccessful)
                        {
                            dbFunction.SetMessageBox(
                                "The receipt image could not be renamed on FTP.\n\n" +
                                "Current file: " + pCurrentFTPFileName + "\n" +
                                "Required file: " + pRequiredFTPFileName,
                                "Image synchronization failed",
                                clsFunction.IconType.iError
                            );

                            return false;
                        }
                    }
                }
                else
                {
                    if (!File.Exists(pImageSource))
                    {
                        dbFunction.SetMessageBox(
                            "The new receipt image could not be found.\n\n" +
                            "File: " + pImageSource,
                            "Image synchronization failed",
                            clsFunction.IconType.iError
                        );

                        return false;
                    }

                    SetStatus(
                        "Uploading receipt image " +
                        Path.GetFileName(pImageSource) + " as " +
                        pRequiredFTPFileName + "..."
                    );

                    bool fUploadSuccessful = await Task.Run(() =>
                    {
                        ftp ftpClient = new ftp(
                            pExpenseFTPHost,
                            clsGlobalVariables.strFTPUserName,
                            clsGlobalVariables.strFTPPassword
                        );

                        try
                        {
                            long pExistingFileSize = ftpClient.getFileSize(pRequiredFTPFileName);

                            if (pExistingFileSize > 0) return false;

                            ftpClient.upload(pRequiredFTPFileName, pImageSource);

                            long pUploadedFileSize = ftpClient.getFileSize(pRequiredFTPFileName);
                            return pUploadedFileSize > 0;
                        }
                        finally
                        {
                            ftpClient.disconnect();
                        }
                    });

                    if (!fUploadSuccessful)
                    {
                        dbFunction.SetMessageBox(
                            "The new receipt image could not be uploaded.\n\n" +
                            "File: " + Path.GetFileName(pImageSource) + "\n" +
                            "Required file: " + pRequiredFTPFileName,
                            "Image synchronization failed",
                            clsFunction.IconType.iError
                        );

                        return false;
                    }
                }

                imageData["FileName"] = pRequiredFTPFileName;
                imageData["FileLocation"] = "ftp://" + pExpenseFTPHost + "/";
                imageData["FTPFileName"] = pRequiredFTPFileName;
                imageData["IsUploaded"] = true;

                item.Tag = imageData.ToString();
                item.Text = iImageIndex.ToString();
                item.SubItems[1].Text = pRequiredFTPFileName;
                item.SubItems[2].Text = "ftp://" + pExpenseFTPHost + "/";

                iImageIndex++;
            }

            return true;
        }

        private void FillExpenseImage()
        {
            lvwExpenseImages.Items.Clear();
            txtImageCount.Text = "0";

            string pFilePrefix =
                txtServiceNo.Text + "_" +
                txtExpenseID.Text + "_" +
                dtExpenseDate.Value.ToString("yyyyMMdd") + "_";

            ftp ftpClient = new ftp(pExpenseFTPHost, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                string[] pFiles = ftpClient.directoryListSimple("");
                if (pFiles == null || pFiles.Length <= 0) return;

                Array.Sort(pFiles, StringComparer.OrdinalIgnoreCase);
                int iLineNo = 0;

                foreach (string pRemoteEntry in pFiles)
                {
                    if (!dbFunction.isValidDescription(pRemoteEntry)) continue;

                    string pFileName = Path.GetFileName(pRemoteEntry.TrimEnd('/', '\\'));

                    if (!dbFunction.isValidDescription(pFileName)) continue;

                    if (!pFileName.StartsWith(pFilePrefix, StringComparison.OrdinalIgnoreCase)) continue;

                    long pFileSize = ftpClient.getFileSize(pFileName);
                    string pLastModified = clsDefines.gNull;

                    try
                    {
                        DateTime dtLastModified = ftpClient.GetDateFTP(pFileName, "ftp://" + pExpenseFTPHost, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                        pLastModified = dtLastModified.ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Unable to obtain FTP modified date: " + ex.Message);
                    }

                    string pExtension = Path.GetExtension(pFileName);

                    iLineNo++;

                    // create image data 

                    JObject imageData = new JObject();

                    imageData["FileName"] = pFileName;
                    imageData["FileLocation"] = "ftp://" + pExpenseFTPHost + "/";
                    imageData["FileSize"] = (pFileSize / 1024D).ToString("N2") + " KB";
                    imageData["LastModified"] = pLastModified;
                    imageData["FileType"] = pExtension.TrimStart('.').ToUpper() + " Image";
                    imageData["Extension"] = pExtension;
                    imageData["FTPFileName"] = pFileName;
                    imageData["IsUploaded"] = true;

                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                    item.SubItems.Add(pFileName);
                    item.SubItems.Add("ftp://" + pExpenseFTPHost + "/");
                    item.SubItems.Add((pFileSize / 1024D).ToString("N2") + " KB");
                    item.SubItems.Add(pLastModified);
                    item.Tag = imageData.ToString();

                    lvwExpenseImages.Items.Add(item);
                }

                txtImageCount.Text = lvwExpenseImages.Items.Count.ToString();
            }
            finally
            {
                ftpClient.disconnect();
                Cursor.Current = Cursors.Default;
            }
        }

        private void FillExpenseSummary()
        {
            decimal dTotalExpenses = ExpenseSummaryGrid.Fill(dgvSummary, lvwExpenseList);

            txtTotalExpenses.Text = dTotalExpenses.ToString("N2");
        }

        private void ComputeTotalExpenses()
        {
            decimal totalExpenses = 0;

            foreach (ListViewItem item in lvwExpenseList.Items)
            {
                decimal expenseAmount;

                if (decimal.TryParse(item.SubItems[4].Text, out expenseAmount))
                {
                    totalExpenses += expenseAmount;
                }
            }

            txtTotalExpenses.Text = totalExpenses.ToString("N2");

        }

        private bool ValidateFields()
        {
            // Field Check
            if (!dbFunction.isValidID(txtServiceNo.Text) || !dbFunction.isValidDescription(txtIRIDNo.Text) || !dbFunction.isValidDescription(txtServiceReferenceNo.Text) || !dbFunction.isValidDescription(txtServiceRequestNo.Text))
            {
                dbFunction.SetMessageBox(
                    "Select a valid service.",
                    "Validation failed",
                    clsFunction.IconType.iError
                );

                return false;
            }

            // Combo box Expense Type check
            if (cboExpenseType.SelectedIndex <= 0)
            {
                dbFunction.SetMessageBox(
                    "Select a valid Expense Type",
                    "Add Expense Error",
                    clsFunction.IconType.iError
                );

                return false;
            }

            // Expense amount check
            decimal dExpenseAmount;

            if (!decimal.TryParse(
                txtExpenseAmount.Text.Trim(),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out dExpenseAmount))
            {
                dbFunction.SetMessageBox(
                    "Please enter a valid expense amount.",
                    "Expense Amount",
                    clsFunction.IconType.iWarning
                );

                txtExpenseAmount.Focus();
                return false;
            }

            if (dExpenseAmount <= 0)
            {
                dbFunction.SetMessageBox(
                        "Expense amount must be greater than zero.",
                    "Expense Amount",
                    clsFunction.IconType.iWarning
                );

                txtExpenseAmount.Focus();
                return false;
            }

            if (decimal.Round(dExpenseAmount, 2) != dExpenseAmount)
            {
                dbFunction.SetMessageBox(
                    "Expense amount can only have two decimal places.",
                    "Expense Amount",
                    clsFunction.IconType.iWarning
                );

                txtExpenseAmount.Focus();
                return false;
            }

            // Date check
            dbFunction.getCurrentDate();
            if (dtExpenseDate.Value.Date > DateTime.Today)
            {
                dbFunction.SetMessageBox(
                    "Expense date cannot be later than today's date.",
                    "Validation failed",
                    clsFunction.IconType.iError
                );

                dtExpenseDate.Focus();

                return false;
            }

            return true;
        }

        private void ResetExpenseFields()
        {
            txtExpenseID.Text = "";
            txtExpenseReferenceNo.Text = "";
            txtRemarks.Text = "";
            txtExpenseAmount.Text = "";
            cboExpenseType.SelectedIndex = 0;
        }

        private void ResetImageFields()
        {
            sSelectedImagePath = clsDefines.gNull;
            lblFileName.Text = clsDefines.gNull;
            pBoxPreview.Image = null;

            pendingImageItem = null;
            pendingImageData = null;

            txtImageAmount.Text = "";
            txtImageAmount.Enabled = false;
            btnAddAmount.Enabled = false;

            if (fNewExpense || fEdit)
            {
                btnAddSelectImage.Enabled = true;
            }
            else
            {
                btnAddSelectImage.Enabled = false;
            }
        }

        private bool ValidateExpenseImages()
        {
            if (lvwExpenseImages.Items.Count < 1)
            {
                dbFunction.SetMessageBox(
                    "A minimum of 1 receipt images is required.\n\n" +
                    "Current image count: " +
                    lvwExpenseImages.Items.Count,
                    "Insufficient images",
                    clsFunction.IconType.iError
                );

                return false;
            }

            return true;
        }

        private bool UploadExpenseImages(string pExpensesID)
        {
            string pServiceNo = dbFunction.CheckAndSetNumericValue(txtServiceNo.Text);
            string pExpenseDate = dtExpenseDate.Value.ToString("yyyyMMdd");

            ftp ftpClient = new ftp(
                pExpenseFTPHost,
                clsGlobalVariables.strFTPUserName,
                clsGlobalVariables.strFTPPassword
            );

            try
            {
                int ImageCount = 1;
                int iTotalImages = lvwExpenseImages.Items.Count;

                foreach (ListViewItem item in lvwExpenseImages.Items)
                {
                    if (item.Tag == null)
                    {
                        dbFunction.SetMessageBox(
                            "The selected receipt contains invalid image information.",
                            "Receipt upload failed",
                            clsFunction.IconType.iError
                        );

                        return false;
                    }

                    JObject imageData = JObject.Parse(item.Tag.ToString());

                    string pImageSource = Convert.ToString(imageData["ImageSource"]);

                    if (!File.Exists(pImageSource))
                    {
                        dbFunction.SetMessageBox(
                            "The receipt image could not be found.\n\n" +
                            "File: " + pImageSource,
                            "Receipt upload failed",
                            clsFunction.IconType.iError
                        );

                        return false;
                    }

                    string pExtension =
                        Path.GetExtension(pImageSource).ToLowerInvariant();

                    string pFileName =
                        pServiceNo + "_" +
                        pExpensesID + "_" +
                        pExpenseDate + "_" +
                        ImageCount.ToString("00") +
                        pExtension;

                    SetStatus(
                        "Uploading receipt image " +
                        Path.GetFileName(pImageSource) +
                        " (" + ImageCount + " of " + iTotalImages + ")..."
                    );

                    Debug.WriteLine("FTP host: " + pExpenseFTPHost);
                    Debug.WriteLine("FTP filename: " + pFileName);

                    ftpClient.upload(pFileName, pImageSource);

                    long pLocalFileSize = new FileInfo(pImageSource).Length;
                    long pUploadedFileSize = ftpClient.getFileSize(pFileName);

                    if (pUploadedFileSize <= 0 || pUploadedFileSize != pLocalFileSize)
                    {
                        dbFunction.SetMessageBox(
                            "The receipt image could not be verified on FTP.\n\n" +
                            "File: " + pFileName,
                            "Receipt upload failed",
                            clsFunction.IconType.iError
                        );

                        return false;
                    }

                    ImageCount++;
                }

                return true;
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox(
                    "The expense was saved, but its receipt images could not be uploaded.\n\n" +
                    ex.Message,
                    "Receipt upload failed",
                    clsFunction.IconType.iError
                );

                return false;
            }
            finally
            {
                ftpClient.disconnect();
            }
        }

        private void SetStatus(string pStatus)
        {
            lblStatus.Text = pStatus;
            lblStatus.Refresh();
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
            fEdit = false;

            dbFunction.ClearListViewItems(lvwExpenseList);
            dbFunction.ClearListViewItems(lvwExpenseImages);
            dbFunction.ClearTextBox(this);

            cboExpenseType.SelectedIndex = 0;

            dtExpenseDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dtExpenseDate, clsFunction.sStandardDateDefault);
            SetStatus(clsDefines.gNull);

            dgvSummary.Rows.Clear();

            InitButtons();
        }

        private async void btnSearchService_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType =
            frmSearchField.SearchType.iFSR;

            frmSearchField.sHeader = "FSR";
            frmSearchField.isCheckBoxes = false;

            frmSearchField frm = new frmSearchField();

            frm.ShowDialog(this);

            if (!frmSearchField.fSelected) return;

            if (clsSearch.ClassFSRNo <= 0)
            {
                frmSearchField.fSelected = false;

                dbFunction.SetMessageBox(
                    "This service cannot be selected because it is still in the Job Order or Dispatch stage.\n\n" +
                    "Expenses can only be added after the FSR has been completed.",
                    "FSR not completed",
                    clsFunction.IconType.iError
                );

                return;
            }

            try
            {
                Enabled = false;
                UseWaitCursor = true;
                Cursor.Current = Cursors.WaitCursor;

                dbFunction.ClearListViewItems(lvwExpenseList);

                await FetchData();

                btnSave.Enabled = true;
                cboExpenseType.Enabled = true;
                btnAddSelectImage.Enabled = true;
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox(
                    "An error occurred while loading the FSR information.\n\n" +
                    "Error: " + ex.Message,
                    "Loading failed",
                    clsFunction.IconType.iError
                );
            }
            finally
            {
                Enabled = true;
                UseWaitCursor = false;

                fEdit = false;
                InitButtons();
                Cursor.Current = Cursors.Default;
            }
        }

        private string GenerateExpensesReference()
        {
            dbAPI.ExecuteAPI("GET", "Search", "Expense Reference Sequence", clsDefines.gNull, "Get Info Detail", "", "GetInfoDetail");
            if (!clsGlobalVariables.isAPIResponseOK) return clsDefines.gNull;

            int lastSequence;

            if (!int.TryParse(clsSearch.ClassOutParamValue, out lastSequence)) return clsDefines.gNull;

            int nextSequence = lastSequence + 1;

            Debug.WriteLine("EXP-" + dtExpenseDate.Value.ToString("yyyyMMdd") + "-" + nextSequence.ToString("D6"));
            return "EXP-" + dtExpenseDate.Value.ToString("yyyyMMdd") + "-" + nextSequence.ToString("D6");
        }

        private string GenerateExpensesMasterReference()
        {
            return "EXP-" + dtExpenseDate.Value.ToString("yyyyMMdd") + "-" + txtServiceNo.Text;
        }

        private void btnAddExpense_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                string pSearchValue =
                dbFunction.CheckAndSetNumericValue(txtServiceNo.Text) + clsDefines.gPipe +
                dbFunction.CheckAndSetNumericValue(txtExpenseID.Text);

                if (dbAPI.isRecordExist("Search", "Service ExpenseID Check", pSearchValue))
                {
                    dbFunction.SetMessageBox(
                        "This expense type already exists for the selected service.",
                        "Duplicate expense",
                        clsFunction.IconType.iError
                    );

                    return;
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            try
            {   
                Cursor.Current = Cursors.WaitCursor;

                string sRowSQL = "";
                string sSQL = "";

                if (!ValidateFields()) return;
                if (!ValidateExpenseImages()) return;

                int rowIndex = cboExpenseType.SelectedIndex - 1;

                dbAPI.ExecuteAPI("GET", "View", "Expense List", clsDefines.gNull, "Advance Detail", "", "ViewAdvanceDetail");

                if (!clsGlobalVariables.isAPIResponseOK) return;
                if (dbAPI.isNoRecordFound()) return;

                int ExpenseTypeID = int.Parse(clsArray.ExpensesID[rowIndex]);

                Debug.WriteLine("SelectedIndex = " + cboExpenseType.SelectedIndex);
                Debug.WriteLine("rowIndex = " + rowIndex);
                Debug.WriteLine("ExpenseTypeID = " + ExpenseTypeID);

                string pExpensesReferenceNo = GenerateExpensesReference();

                if (!dbFunction.isValidDescription(pExpensesReferenceNo))
                {
                    dbFunction.SetMessageBox(
                        "Expenses ReferenceNo not generated",
                        "Add expense",
                        clsFunction.IconType.iInformation
                    );

                    return;
                }

                sRowSQL = "('" +
                    dbFunction.CheckAndSetStringValue(pExpensesReferenceNo) + "'," +
                    dbFunction.CheckAndSetNumericValue(txtServiceNo.Text) + "," +
                    dbFunction.CheckAndSetNumericValue(txtTAIDNo.Text) + "," +
                    dbFunction.CheckAndSetNumericValue(txtIRIDNo.Text) + "," +
                    dbFunction.CheckAndSetNumericValue(ExpenseTypeID.ToString()) + "," +
                    dbFunction.CheckAndSetNumericValue(txtExpenseAmount.Text) + "," +
                    "'" + dbFunction.CheckAndSetStringValue(txtRemarks.Text) + "'," +
                    "'" + dtExpenseDate.Value.ToString("yyyy-MM-dd") + "')";

                sSQL += sRowSQL;    

                Debug.WriteLine("SaveExpense::\n" + "sSQL=" + sSQL);

                dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 0);

                if (!dbFunction.fSavingConfirm(false)) return;

                dbAPI.ExecuteAPI("POST", "Insert", "", txtServiceNo.Text, "Service Expenses Detail", sRowSQL, "InsertCollectionDetail");
                if (!clsGlobalVariables.isAPIResponseOK) return;

                string pMasterReferenceNo = GenerateExpensesMasterReference();

                if (pMasterReferenceNo == clsDefines.gNull)
                {
                    dbFunction.SetMessageBox(
                        "Master Expenses ReferenceNo not generated",
                        "Add expense",
                        clsFunction.IconType.iInformation
                    );

                    return;
                }

                dbAPI.ExecuteAPI("POST", "Insert", "", txtServiceNo.Text, "Service Expenses Master", txtServiceNo.Text + clsDefines.gPipe + pMasterReferenceNo, "InsertCollectionMaster");
                if (!clsGlobalVariables.isAPIResponseOK) return;

                if (!UploadExpenseImages(ExpenseTypeID.ToString())) return;

                dbFunction.SetMessageBox(
                    "Expense and receipt images successfully saved.",
                    "Add expense",
                    clsFunction.IconType.iInformation
                );

                btnRefresh.PerformClick();
                SetStatus(clsDefines.gNull);

                fNewExpense = false;
                fEdit = false;

                lvwExpenseList.SelectedIndices.Clear();
                lvwExpenseImages.Items.Clear();

                ResetExpenseFields();
                ResetImageFields();

                txtImageCount.Text = "0";
                dtExpenseDate.Value = DateTime.Today;

                cboExpenseType.Enabled = true;
                btnAddSelectImage.Enabled = true;

                SetStatus(clsDefines.gNull);

                InitButtons();

                cboExpenseType.Focus();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.Default;

                if (txtServiceNo.Text.Equals(clsDefines.gNull) || txtIRIDNo.Text.Equals(clsDefines.gNull))
                {
                    dbFunction.SetMessageBox(
                        "No service selected.\n\n" +
                        "Please select a service before refreshing the expense list.",
                        "Refresh failed",
                        clsFunction.IconType.iError
                    );

                    return;
                }

                FillExpenseList();
                FillExpenseSummary();
                FillExpenseImage();
            }
            finally 
            {
                Cursor.Current = Cursors.WaitCursor;
            }


        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateFields()) return;
            if (!ValidateExpenseImages()) return;
            if (!dbFunction.fSavingConfirm(true)) return;

            if (!dbFunction.fPromptConfirmation(
                "Saving this expense may take some time.\n\n" +
                "Receipt images will be uploaded and synchronized with FTP.\n" +
                "Do you want to continue?"))
            {
                return;
            }

            try
            {
                Enabled = false;
                UseWaitCursor = true;
                Cursor.Current = Cursors.WaitCursor;

                if (!await UpdateExpense())
                {
                    dbFunction.SetMessageBox(
                        "The expense could not be updated.",
                        "Expense update failed",
                        clsFunction.IconType.iError
                    );

                    return;
                }

                if (!await SynchronizeExpenseImages(txtExpenseID.Text))
                {
                    return;
                }

                FillExpenseList();
                FillExpenseSummary();
                FillExpenseImage();

                dbFunction.SetMessageBox(
                    "Expense and receipt images successfully updated.",
                    "Expense update",
                    clsFunction.IconType.iInformation
                );

                ResetExpenseFields();
                ResetImageFields();

                lvwExpenseImages.Items.Clear();
                txtImageCount.Text = "0";

                dtExpenseDate.Value = DateTime.Today;
                cboExpenseType.Enabled = true;

                fNewExpense = false;
                fEdit = false;

                InitButtons();
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox(
                    "An error occurred while updating the expense.\n\n" +
                    "Error: " + ex.Message,
                    "Expense update failed",
                    clsFunction.IconType.iError
                );
            }
            finally
            {
                SetStatus(clsDefines.gNull);
                Enabled = true;
                UseWaitCursor = false;
                Cursor.Current = Cursors.Default;
            }
        }

        private async void btnDeleteExpense_Click(object sender, EventArgs e)
        {
            if (lvwExpenseList.SelectedItems.Count <= 0)
            {
                dbFunction.SetMessageBox(
                    "Please select an expense to delete.",
                    "No expense selected",
                    clsFunction.IconType.iError
                );

                return;
            }

            ListViewItem item = lvwExpenseList.SelectedItems[0];

            if (item.Tag == null) return;

            string pJSONString = item.Tag.ToString();

            string pDetailID = dbAPI.GetValueFromJSONString(pJSONString, "DetailID");
            string pExpensesID = dbAPI.GetValueFromJSONString(pJSONString, "ExpensesID");
            string pExpenseReferenceNo = dbAPI.GetValueFromJSONString(pJSONString, "ExpensesReferenceNo");
            string pExpenseType = dbAPI.GetValueFromJSONString(pJSONString, "ExpenseType");
            string pAmount = dbAPI.GetValueFromJSONString(pJSONString, "ExpensesAmount");
            string pRemarks = dbAPI.GetValueFromJSONString(pJSONString, "ExpensesDescription");
            string pServiceNo = txtServiceNo.Text;
            string pIRIDNo = txtIRIDNo.Text;

            if (!dbFunction.isValidID(pDetailID) || !dbFunction.isValidID(pServiceNo))
            {
                dbFunction.SetMessageBox(
                    "The selected expense contains invalid information.",
                    "Delete expense failed",
                    clsFunction.IconType.iError
                );

                return;
            }

            string pSearchValue = dbFunction.CheckAndSetNumericValue(pServiceNo) + clsDefines.gPipe + dbFunction.CheckAndSetNumericValue(pDetailID);

            if (!dbFunction.fPromptConfirmation(
                "Are you sure you want to permanently delete this expense?\n\n" +
                "[ THIS ACTION IS IRREVERSIBLE ]\n\n" +
                "Expense Type: " + pExpenseType + "\n" +
                "Amount: " + pAmount + "\n" +
                "Remarks: " + pRemarks + "\n" +
                "Request ID: " + txtServiceRequestNo.Text + "\n" +
                "IRID No: " + pIRIDNo + "\n\n" +
                "This action cannot be undone."))
            {
                return;
            }

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                SetStatus("Deleting expense record...");

                dbAPI.ExecuteAPI("DELETE", "Delete", "", pSearchValue, "Delete Service Expense", "", "DeleteCollectionDetail");

                if (!clsGlobalVariables.isAPIResponseOK) return;

                SetStatus("Deleting receipt images...");

                bool fImagesDeleted = await DeleteExpenseImages(pServiceNo, pExpensesID);

                FillExpenseList();
                FillExpenseSummary();

                lvwExpenseImages.Items.Clear();
                txtImageCount.Text = "0";

                ResetExpenseFields();
                ResetImageFields();

                dbFunction.SetMessageBox(
                    fImagesDeleted
                        ? "Expense and receipt images were successfully deleted."
                        : "Expense was successfully deleted.\n\nNo receipt images were found.",
                    "Delete expense",
                    clsFunction.IconType.iInformation
                );
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox(
                    "An error occurred while deleting the expense.\n\n" +
                    "Error: " + ex.Message,
                    "Delete expense failed",
                    clsFunction.IconType.iError
                );
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                SetStatus(clsDefines.gNull);
            }
        }

        private bool PromptManualReceiptAmount(out decimal pReceiptAmount)
        {
            pReceiptAmount = 0M;

            while (true)
            {
                InputBox.iInputType = clsFunction.Numeric_Input;
                InputBox.iInputLimitSize = 12;

                InputBoxResult amountInput = InputBox.Show(
                    "Enter the receipt amount.",
                    "Receipt Amount",
                    "0.00",
                    100,
                    0,
                    12,
                    (int)Enums.OptionType.Others
                );

                if (amountInput.ReturnCode != DialogResult.OK)
                {
                    return false;
                }

                if (decimal.TryParse(
                    amountInput.Text.Trim(),
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out pReceiptAmount) &&
                    pReceiptAmount > 0 &&
                    decimal.Round(pReceiptAmount, 2) == pReceiptAmount)
                {
                    return true;
                }

                dbFunction.SetMessageBox(
                    "Please enter a valid receipt amount greater than zero " +
                    "with no more than two decimal places.",
                    "Invalid Receipt Amount",
                    clsFunction.IconType.iWarning
                );
            }
        }

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
                openFile.Title = "Select receipt image";
                openFile.Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|PNG Images (*.png)|*.png|JPEG Images (*.jpg;*.jpeg)|*.jpg;*.jpeg";
                openFile.Multiselect = false;

                if (openFile.ShowDialog(this) != DialogResult.OK) return;

                FileInfo fileInfo = new FileInfo(openFile.FileName);

                foreach (ListViewItem existingItem in lvwExpenseImages.Items)
                {
                    if (existingItem.Tag == null) continue;

                    JObject existingImageData = JObject.Parse(existingItem.Tag.ToString());

                    string pExistingImageSource =
                        Convert.ToString(existingImageData["ImageSource"]);

                    if (pExistingImageSource.Equals(
                        fileInfo.FullName,
                        StringComparison.OrdinalIgnoreCase
                    ))
                    {
                        dbFunction.SetMessageBox(
                            "The selected image is already in the attachment list.",
                            "Duplicate image",
                            clsFunction.IconType.iError
                        );

                        return;
                    }
                }

                sSelectedImagePath = fileInfo.FullName;

                int iLineNo = lvwExpenseImages.Items.Count + 1;

                ListViewItem item = new ListViewItem(iLineNo.ToString());

                item.SubItems.Add(fileInfo.Name);
                item.SubItems.Add(fileInfo.DirectoryName);
                item.SubItems.Add((fileInfo.Length / 1024D).ToString("N2") + " KB");
                item.SubItems.Add(fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"));

                JObject imageData = new JObject();

                imageData["FileName"] = fileInfo.Name;
                imageData["FileLocation"] = fileInfo.DirectoryName;
                imageData["FileSize"] = (fileInfo.Length / 1024D).ToString("N2") + " KB";
                imageData["LastModified"] = fileInfo.LastWriteTime.ToString("yyyy-MM-dd h:mm tt");
                imageData["FileType"] = fileInfo.Extension.TrimStart('.').ToUpper() + " Image";
                imageData["Extension"] = fileInfo.Extension;
                imageData["ImageSource"] = fileInfo.FullName;

                if (pBoxPreview.Image != null)
                {
                    pBoxPreview.Image.Dispose();
                    pBoxPreview.Image = null;
                }

                pBoxPreview.Image = dbReceiptImageProcessor.CreatePreview(fileInfo.FullName, ReceiptPreviewMode.Original);
                pBoxPreview.SizeMode = PictureBoxSizeMode.Zoom;

                lblFileName.Text = fileInfo.Name;

                pBoxPreview.Refresh();
                lblFileName.Refresh();

                string pOCRText = dbReceiptImageProcessor.ExtractText(fileInfo.FullName);

                decimal? dDetectedReceiptTotal = dbReceiptImageProcessor.ExtractTransactionAmount(pOCRText);

                if (dDetectedReceiptTotal.HasValue)
                {
                    txtDetectedReceiptTotal.Text = dDetectedReceiptTotal.Value.ToString("0.00");
                    imageData["DetectedReceiptTotal"] = dDetectedReceiptTotal.Value.ToString("0.00");
                }
                else
                {
                    txtDetectedReceiptTotal.Text = "[ERROR]";
                    imageData["DetectedReceiptTotal"] = string.Empty;
                }

                bool fUseDetectedTotal = false;
                decimal dReceiptAmount = 0M;

                if (dDetectedReceiptTotal.HasValue)
                {
                    fUseDetectedTotal = dbFunction.fPromptConfirmation(
                        "We detected a receipt total of ₱" +
                        dDetectedReceiptTotal.Value.ToString("0.00") +
                        ".\n\nWould you like to add this to the expense amount?"
                    );

                    if (fUseDetectedTotal)
                    {
                        dReceiptAmount = dDetectedReceiptTotal.Value;
                    }
                    else
                    {
                        pendingImageItem = item;
                        pendingImageData = imageData;

                        txtImageAmount.Text = "";
                        txtImageAmount.Enabled = true;
                        btnAddAmount.Enabled = true;
                        btnAddSelectImage.Enabled = false;

                        txtImageAmount.Focus();
                        return;
                    }
                }
                else
                {
                    dbFunction.SetMessageBox(
                        "The receipt amount could not be detected.\n\n" +
                        "Please enter the amount manually.",
                        "Receipt OCR",
                        clsFunction.IconType.iWarning
                    );

                    pendingImageItem = item;
                    pendingImageData = imageData;

                    txtImageAmount.Text = "";
                    txtImageAmount.Enabled = true;
                    btnAddAmount.Enabled = true;
                    btnAddSelectImage.Enabled = false;

                    txtImageAmount.Focus();
                    return;
                }

                decimal dCurrentExpenseAmount = 0M;

                decimal.TryParse(
                    txtExpenseAmount.Text,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out dCurrentExpenseAmount
                );

                txtExpenseAmount.Text =
                    (
                        dCurrentExpenseAmount +
                        dReceiptAmount
                    ).ToString("0.00");

                if (dDetectedReceiptTotal.HasValue)
                {
                    imageData["DetectedReceiptTotal"] = dDetectedReceiptTotal.Value.ToString("0.00");
                }
                else
                {
                    imageData["DetectedReceiptTotal"] = string.Empty;
                }

                imageData["AppliedReceiptAmount"] = dReceiptAmount.ToString("0.00");
                imageData["OCRAmountAccepted"] = fUseDetectedTotal;

                item.Tag = imageData.ToString();

                lvwExpenseImages.Items.Add(item);
                txtImageCount.Text = lvwExpenseImages.Items.Count.ToString();

                ResetImageFields();
            }
        }

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(sSelectedImagePath) || !File.Exists(sSelectedImagePath))
            {
                dbFunction.SetMessageBox(
                    "Please select an image first.",
                    "No image selected",
                    clsFunction.IconType.iError
                );

                return;
            }

            foreach (ListViewItem existingItem in lvwExpenseImages.Items)
            {
                if (existingItem.Tag == null) continue;
                JObject existingImageData = JObject.Parse(existingItem.Tag.ToString());

                string existingImageSource = Convert.ToString(existingImageData["ImageSource"]);
                if (existingImageSource.Equals(sSelectedImagePath, StringComparison.OrdinalIgnoreCase))
                {
                    dbFunction.SetMessageBox(
                        "The selected image is already in the attachment list.",
                        "Duplicate image",
                        clsFunction.IconType.iError
                    );

                    return;
                }
            }

            FileInfo fileInfo = new FileInfo(sSelectedImagePath);

            int iLineNo = lvwExpenseImages.Items.Count + 1;

            ListViewItem item = new ListViewItem(iLineNo.ToString());

            item.SubItems.Add(fileInfo.Name);
            item.SubItems.Add(fileInfo.DirectoryName);

            item.SubItems.Add((fileInfo.Length / 1024D).ToString("N2") + " KB");
            item.SubItems.Add(fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"));

            JObject imageData = new JObject();

            imageData["FileName"] = fileInfo.Name;
            imageData["FileLocation"] = fileInfo.DirectoryName;

            imageData["FileSize"] = (fileInfo.Length / 1024D).ToString("N2") + " KB";
            imageData["LastModified"] = fileInfo.LastWriteTime.ToString("yyyy-MM-dd h:mm tt");

            imageData["FileType"] = fileInfo.Extension.TrimStart('.').ToUpper() + " Image"; imageData["Extension"] = fileInfo.Extension;
            imageData["ImageSource"] = fileInfo.FullName;

            item.Tag = imageData.ToString();

            lvwExpenseImages.Items.Add(item);
            txtImageCount.Text = lvwExpenseImages.Items.Count.ToString();

            ResetImageFields();
        }

        private void btnClearImage_Click(object sender, EventArgs e)
        {
            ResetImageFields();
        }

        private void lvwImageList_MouseDoubleClick(object sender, EventArgs e)
        {
            if (lvwExpenseImages.SelectedItems.Count <= 0) return;

            ListViewItem item = lvwExpenseImages.SelectedItems[0];

            if (item.Tag == null) return;

            JObject imageData = JObject.Parse(item.Tag.ToString());

            bool fUploaded = imageData["IsUploaded"] != null && Convert.ToBoolean(imageData["IsUploaded"]);

            if (!fUploaded)
            {
                dbFunction.SetMessageBox(
                    "This receipt has not been uploaded yet.",
                    "View receipt",
                    clsFunction.IconType.iError
                );

                return;
            }

            string pFTPFileName = Convert.ToString(imageData["FTPFileName"]);

            string pImageURL =
                dbAPI.getAPISSLEnable() +
                clsGlobalVariables.strAPIServerIPAddress.TrimEnd('/') + "/" +
                clsGlobalVariables.strAPIFolder.Trim('/', '\\') + "/" +
                clsGlobalVariables.strFTPUploadPath.Trim('/', '\\') + "/expenses/" +
                clsSearch.ClassBankCode.ToLowerInvariant() + "/" +
                Uri.EscapeDataString(pFTPFileName);

            using (frmImagePreview frm = new frmImagePreview(imageData.ToString(), pImageURL))
            {
                frm.ShowDialog(this);
            }
        }

        private void lvwImageList_Click(object sender, EventArgs e)
        {
            if (lvwExpenseImages.SelectedItems.Count <= 0) return;

            ListViewItem item = lvwExpenseImages.SelectedItems[0];

            if (item.Tag == null) return;

            JObject imageData = JObject.Parse(item.Tag.ToString());

            bool fUploaded = imageData["IsUploaded"] != null && Convert.ToBoolean(imageData["IsUploaded"]);

            try
            {
                SetStatus("Loading image preview...");

                lvwExpenseImages.Enabled = false;
                UseWaitCursor = true;
                Cursor.Current = Cursors.WaitCursor;

                if (pBoxPreview.Image != null)
                {
                    pBoxPreview.Image.Dispose();
                    pBoxPreview.Image = null;
                }

                if (fUploaded)
                {
                    string pFTPFileName = Convert.ToString(imageData["FTPFileName"]);

                    if (!dbFunction.isValidDescription(pFTPFileName)) return;

                    string pImageURL =
                        dbAPI.getAPISSLEnable() +
                        clsGlobalVariables.strAPIServerIPAddress.TrimEnd('/') + "/" +
                        clsGlobalVariables.strAPIFolder.Trim('/', '\\') + "/" +
                        clsGlobalVariables.strFTPUploadPath.Trim('/', '\\') + "/expenses/" +
                        clsSearch.ClassBankCode.ToLowerInvariant() + "/" +
                        Uri.EscapeDataString(pFTPFileName);

                    SetStatus("Loading preview: " + pFTPFileName + "...");

                    Debug.WriteLine("Expense image URL: " + pImageURL);

                    pBoxPreview.Load(pImageURL);
                    lblFileName.Text = pFTPFileName;
                }
                else
                {
                    {
                    string pImageSource = Convert.ToString(imageData["ImageSource"]);

                    pBoxPreview.Image = dbReceiptImageProcessor.CreatePreview(pImageSource, ReceiptPreviewMode.Original);
                    lblFileName.Text = Convert.ToString(imageData["FileName"]);

                    string pDetectedReceiptTotal = Convert.ToString(imageData["DetectedReceiptTotal"]);
                        if (string.IsNullOrWhiteSpace(pDetectedReceiptTotal))
                        {
                            txtDetectedReceiptTotal.Text = "[ERROR]";
                        }
                        else
                        {
                            txtDetectedReceiptTotal.Text = pDetectedReceiptTotal;
                        }
                    }
                }

                pBoxPreview.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox(
                    "Unable to load the image preview.\n\n" +
                    ex.Message,
                    "Preview failed",
                    clsFunction.IconType.iError
                );
            }
            finally
            {
                lvwExpenseImages.Enabled = true;
                UseWaitCursor = false;
                Cursor.Current = Cursors.Default;

                SetStatus(clsDefines.gNull);
            }
        }

        private async void btnRemoveImage_Click(object sender, EventArgs e)
        {
            if (lvwExpenseImages.SelectedItems.Count <= 0)
            {
                dbFunction.SetMessageBox(
                    "Please select an image to remove.",
                    "No image selected",
                    clsFunction.IconType.iError
                );

                return;
            }

            ListViewItem item = lvwExpenseImages.SelectedItems[0];

            if (item.Tag == null) return;

            JObject imageData = JObject.Parse(item.Tag.ToString());

            string pFileName = Convert.ToString(imageData["FileName"]);
            string pFTPFileName = Convert.ToString(imageData["FTPFileName"]);

            if (!dbFunction.fPromptConfirmation(
                "Are you sure you want to remove this image?\n\n" +
                "[ UPLOADED IMAGES ARE DELETED IMMEDIATELY ]\n\n" +
                "File name: " + pFileName))
            {
                return;
            }

            if (dbFunction.isValidDescription(pFTPFileName))
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    SetStatus("Deleting receipt image " + pFTPFileName + "...");

                    bool fDeleted = await Task.Run(() =>
                    {
                        ftp ftpClient = new ftp(
                            pExpenseFTPHost,
                            clsGlobalVariables.strFTPUserName,
                            clsGlobalVariables.strFTPPassword
                        );

                        try
                        {
                            ftpClient.delete(pFTPFileName);

                            long pRemainingFileSize = ftpClient.getFileSize(pFTPFileName);

                            return pRemainingFileSize <= 0;
                        }
                        finally
                        {
                            ftpClient.disconnect();
                        }
                    });

                    if (!fDeleted)
                    {
                        dbFunction.SetMessageBox(
                            "The receipt image could not be deleted from FTP.\n\n" +
                            "File: " + pFTPFileName,
                            "Delete image failed",
                            clsFunction.IconType.iError
                        );

                        return;
                    }
                }
                catch (Exception ex)
                {
                    dbFunction.SetMessageBox(
                        "An error occurred while deleting the receipt image.\n\n" +
                        "File: " + pFTPFileName + "\n" +
                        "Error: " + ex.Message,
                        "Delete image failed",
                        clsFunction.IconType.iError
                    );

                    return;
                }
                finally
                {
                    SetStatus(clsDefines.gNull);

                    Cursor.Current = Cursors.Default;
                }
            }

            lvwExpenseImages.Items.Remove(item);

            int iLineNo = 1;

            foreach (ListViewItem imageItem in lvwExpenseImages.Items)
            {
                imageItem.Text = iLineNo.ToString();
                iLineNo++;
            }

            txtImageCount.Text = lvwExpenseImages.Items.Count.ToString();

            ResetImageFields();
        }

        private void lvwExpenseList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                SetStatus("Loading images...");

                if (lvwExpenseList.SelectedItems.Count <= 0) return;

                ListViewItem item = lvwExpenseList.SelectedItems[0];

                if (item.Tag == null) return;

                string pJSONString = item.Tag.ToString();

                string pExpensesID = dbAPI.GetValueFromJSONString(pJSONString, "ExpensesID");
                string pExpenseReferenceNo = item.SubItems[2].Text;
                string pExpenseType = dbAPI.GetValueFromJSONString(pJSONString, "ExpenseType");
                string pRemarks = dbAPI.GetValueFromJSONString(pJSONString, "ExpensesDescription");
                string pAmount = dbAPI.GetValueFromJSONString(pJSONString, "ExpensesAmount");
                string pExpenseDate = dbAPI.GetValueFromJSONString(pJSONString, "ExpensesDate");

                txtExpenseID.Text = pExpensesID;
                txtExpenseReferenceNo.Text = pExpenseReferenceNo;
                txtRemarks.Text = pRemarks;
                txtExpenseAmount.Text = pAmount;

                int expenseTypeIndex = cboExpenseType.FindStringExact(pExpenseType);
                if (expenseTypeIndex >= 0) cboExpenseType.SelectedIndex = expenseTypeIndex;

                cboExpenseType.Enabled = false;
                DateTime expenseDate;

                if (DateTime.TryParse(pExpenseDate, out expenseDate))
                {
                    dtExpenseDate.Value = expenseDate;
                }

                FillExpenseImage();

                fNewExpense = false;
                fEdit = true;

                InitButtons();
            }
            finally
            {
                SetStatus(clsDefines.gNull);
            }
        }

        private void cboExpenseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtExpenseID.Text = "";

            if (cboExpenseType.SelectedIndex <= 0) return;
            int rowIndex = cboExpenseType.SelectedIndex - 1;

            if (clsArray.ExpensesID == null || rowIndex < 0 || rowIndex >= clsArray.ExpensesID.Length) return;

            txtExpenseID.Text = clsArray.ExpensesID[rowIndex];
        }

        private void btnClearExpense_Click(object sender, EventArgs e)
        {
            fEdit = false;

            lvwExpenseList.SelectedIndices.Clear();
            lvwExpenseImages.Items.Clear();

            ResetExpenseFields();
            ResetImageFields();

            txtImageCount.Text = "0";
            dtExpenseDate.Value = DateTime.Today;

            cboExpenseType.Enabled = true;

            SetStatus(clsDefines.gNull);

            fNewExpense = false;
            fEdit = false;
            InitButtons();

            cboExpenseType.Focus();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            txtExpenseID.Text = "";
            txtExpenseReferenceNo.Text = "";
            txtExpenseAmount.Text = "";
            txtRemarks.Text = "";

            cboExpenseType.SelectedIndex = 0;
            dtExpenseDate.Value = DateTime.Today;

            string pExpenseReferenceNo = GenerateExpensesReference();

            if (!dbFunction.isValidDescription(pExpenseReferenceNo))
            {
                dbFunction.SetMessageBox(
                    "Unable to generate the expense reference number.",
                    "Expense",
                    clsFunction.IconType.iError
                );

                return;
            }

            txtExpenseReferenceNo.Text = pExpenseReferenceNo;

            fNewExpense = true;
            fEdit = false;

            InitButtons();

            cboExpenseType.Focus();
        }

        private void btnAddAmount_Click(object sender, EventArgs e)
        {
            if (pendingImageItem == null || pendingImageData == null)
            {
                dbFunction.SetMessageBox(
                    "There is no pending receipt image.",
                    "Receipt Amount",
                    clsFunction.IconType.iWarning
                );

                return;
            }

            if (!dbFunction.isValidAmount(txtImageAmount.Text.Trim()))
            {
                dbFunction.SetMessageBox(
                    "Please enter the receipt amount.",
                    "Receipt Amount",
                    clsFunction.IconType.iWarning
                );

                txtImageAmount.Focus();
                return;
            }

            decimal dReceiptAmount;

            if (!decimal.TryParse(txtImageAmount.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out dReceiptAmount))
            {
                dbFunction.SetMessageBox(
                    "Please enter a valid receipt amount.",
                    "Receipt Amount",
                    clsFunction.IconType.iWarning
                );

                txtImageAmount.Focus();
                return;
            }

            if (dReceiptAmount <= 0)
            {
                dbFunction.SetMessageBox(
                    "Receipt amount must be greater than zero.",
                    "Receipt Amount",
                    clsFunction.IconType.iWarning
                );

                txtImageAmount.Focus();
                return;
            }

            if (decimal.Round(dReceiptAmount, 2) != dReceiptAmount)
            {
                dbFunction.SetMessageBox(
                    "Receipt amount can only have two decimal places.",
                    "Receipt Amount",
                    clsFunction.IconType.iWarning
                );

                txtImageAmount.Focus();
                return;
            }

            decimal dCurrentExpenseAmount = 0M;

            decimal.TryParse(txtExpenseAmount.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out dCurrentExpenseAmount);

            txtExpenseAmount.Text = (dCurrentExpenseAmount + dReceiptAmount).ToString("0.00");

            pendingImageData["AppliedReceiptAmount"] = dReceiptAmount.ToString("0.00");
            pendingImageData["OCRAmountAccepted"] = false;

            pendingImageItem.Tag = pendingImageData.ToString();

            lvwExpenseImages.Items.Add(pendingImageItem);
            txtImageCount.Text = lvwExpenseImages.Items.Count.ToString();

            ResetImageFields();
        }

        private async void btnSearchExpensesReferenceNo_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iExpense;
            frmSearchField.sHeader = "EXPENSE REFERENCE";
            frmSearchField.isCheckBoxes = false;

            frmSearchField frm = new frmSearchField();

            frm.ShowDialog(this);

            if (!frmSearchField.fSelected) return;

            if (clsSearch.ClassFSRNo <= 0)
            {
                frmSearchField.fSelected = false;

                dbFunction.SetMessageBox(
                    "This service cannot be selected because it is still in the Job Order or Dispatch stage.\n\n" +
                    "Expenses can only be viewed after the FSR has been completed.",
                    "FSR not completed",
                    clsFunction.IconType.iError
                );

                return;
            }

            try
            {
                Enabled = false;
                UseWaitCursor = true;
                Cursor.Current = Cursors.WaitCursor;

                fNewExpense = false;
                fEdit = false;

                dbFunction.ClearListViewItems(lvwExpenseList);
                dbFunction.ClearListViewItems(lvwExpenseImages);

                await FetchData();

                foreach (ListViewItem item in lvwExpenseList.Items)
                {
                    if (item.SubItems[2].Text.Equals(clsSearch.ClassExpenseReferenceNo, StringComparison.OrdinalIgnoreCase))
                    {
                        item.Selected = true;
                        item.Focused = true;
                        item.EnsureVisible();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox(
                    "An error occurred while loading the expense information.\n\n" +
                    "Error: " + ex.Message,
                    "Loading failed",
                    clsFunction.IconType.iError
                );
            }
            finally
            {
                Enabled = true;
                UseWaitCursor = false;

                InitButtons();

                Cursor.Current = Cursors.Default;
            }
        }
    }
}
