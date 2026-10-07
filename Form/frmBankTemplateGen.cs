using MIS.Function;
using MIS.Model;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;


namespace MIS
{
    public partial class frmBankTemplateGen : Form
    {
        private readonly BindingList<IrImportRow> _irRows = new BindingList<IrImportRow>();
        private readonly List<string> _sourceFiles = new List<string>();
        private clsAPI dbAPI;

        public frmBankTemplateGen()
        {
            InitializeComponent();
            ConfigureBankFieldsGrid();
            ApplyMccHeaderColors();

            grdBankFields.DataSource = _irRows;

            EnableFileDrop(this);

            btnBrowse.Click += btnBrowse_Click;
            btnGenerate.Click += btnGenerate_Click;
            btnReset.Click += btnReset_Click;

            grdBankFields.CurrentCellDirtyStateChanged += grdBankFields_CurrentCellDirtyStateChanged;

            grdBankFields.RowsAdded += grdBankFields_RowsAdded;
            grdBankFields.RowsRemoved += grdBankFields_RowsRemoved;
            grdBankFields.DataBindingComplete += grdBankFields_DataBindingComplete;
        }


        private void EnableFileDrop(Control parentControl)
        {
            parentControl.AllowDrop = true;
            parentControl.DragEnter += BankTemplate_DragEnter;
            parentControl.DragDrop += BankTemplate_DragDrop;

            foreach (Control childControl in parentControl.Controls)
            {
                EnableFileDrop(childControl);
            }
        }

        private void BankTemplate_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (files == null || files.Length == 0)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            bool allFilesAreValid = files.All(filePath => File.Exists(filePath)
               && string.Equals(Path.GetExtension(filePath), ".xlsx", StringComparison.OrdinalIgnoreCase));

            e.Effect = allFilesAreValid ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void BankTemplate_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (files == null || files.Length == 0) return;

            LoadTemplates(files);
        }

        private void ConfigureBankFieldsGrid()
        {
            grdBankFields.AutoGenerateColumns = false;
            grdBankFields.ColumnHeadersVisible = true;
            grdBankFields.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            grdBankFields.ColumnHeadersHeight = 36;
            grdBankFields.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grdBankFields.RowHeadersVisible = true;
            grdBankFields.RowHeadersWidth = 45;
            grdBankFields.AllowUserToAddRows = false;
            grdBankFields.AllowUserToDeleteRows = false;
            grdBankFields.AllowUserToResizeRows = false;
            grdBankFields.SelectionMode = DataGridViewSelectionMode.CellSelect;
            grdBankFields.EditMode = DataGridViewEditMode.EditOnEnter;
            grdBankFields.ScrollBars = ScrollBars.Both;
            grdBankFields.MultiSelect = false;
            grdBankFields.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            grdBankFields.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            grdBankFields.TopLeftHeaderCell.Value = "#";
            grdBankFields.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colIrRequestId.ReadOnly = true;

            ReplaceWithDatePickerColumn(ref colIrRequestDate);
            ReplaceWithDatePickerColumn(ref colIrTargetInstallationDate);

            DataGridViewTextBoxColumn bancnetTidColumn = new DataGridViewTextBoxColumn
                {
                    Name = "colIrBancnetTid",
                    HeaderText = "Bancnet TID",
                    DataPropertyName = "BancnetTid",
                    Width = 140
                };

            grdBankFields.Columns.Insert(colIrTid.Index + 1, bancnetTidColumn);
        }

        private void ApplyMccHeaderColors()
        {
            Color darkBlue = Color.FromArgb(0, 32, 96);
            Color red = Color.FromArgb(255, 0, 0);
            Color brightGreen = Color.FromArgb(0, 176, 80);

            // Dark blue headers
            SetHeaderColor(colIrRequestId, darkBlue);
            SetHeaderColor(colIrRemarks, darkBlue);
            SetHeaderColor(colIrMid, darkBlue);
            SetHeaderColor(colIrTid, darkBlue);
            SetHeaderColor(colIrMerchantLocation, darkBlue);
            SetHeaderColor(colIrAddress, darkBlue);

            // Red headers
            SetHeaderColor(colIrRequestDate, red);
            SetHeaderColor(colIrTargetInstallationDate, red);

            // Different shade of green
            SetHeaderColor(colIrRequestPrioritization, brightGreen);
        }

        private static void SetHeaderColor(DataGridViewColumn column,Color backColor)
        {
            column.HeaderCell.Style.BackColor = backColor;
            column.HeaderCell.Style.ForeColor = Color.White;
            column.HeaderCell.Style.SelectionBackColor = backColor;
            column.HeaderCell.Style.SelectionForeColor = Color.White;
        }

        private void ReplaceWithDatePickerColumn(ref DataGridViewTextBoxColumn originalColumn)
        {
            int columnIndex = originalColumn.Index;

            DataGridViewDatePickerColumn dateColumn = new DataGridViewDatePickerColumn
                {
                    Name = originalColumn.Name,
                    HeaderText = originalColumn.HeaderText,
                    DataPropertyName = originalColumn.DataPropertyName,
                    Width = originalColumn.Width,
                    MinimumWidth = originalColumn.MinimumWidth,
                    AutoSizeMode = originalColumn.AutoSizeMode,
                    FillWeight = originalColumn.FillWeight,
                    Frozen = originalColumn.Frozen,
                    ReadOnly = originalColumn.ReadOnly,
                    Visible = originalColumn.Visible,
                    HeaderCell = (DataGridViewColumnHeaderCell) originalColumn.HeaderCell.Clone(),
                    DefaultCellStyle = originalColumn.DefaultCellStyle.Clone()
                };

            grdBankFields.Columns.Remove(originalColumn);
            grdBankFields.Columns.Insert(columnIndex, dateColumn);

            originalColumn = dateColumn;
        }

        private void grdBankFields_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            UpdateGridRowNumbers();
        }

        private void grdBankFields_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            UpdateGridRowNumbers();
        }

        private void grdBankFields_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            UpdateGridRowNumbers();
        }

        private void UpdateGridRowNumbers()
        {
            for (int rowIndex = 0; rowIndex < grdBankFields.Rows.Count; rowIndex++)
            {
                grdBankFields.Rows[rowIndex].HeaderCell.Value = (rowIndex + 1).ToString();
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Select one or more Issuance Templates";

                dialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";

                dialog.CheckFileExists = true;

                dialog.Multiselect = true; // allows multiple excel files. 

                if (dialog.ShowDialog() != DialogResult.OK) return;

                LoadTemplates(dialog.FileNames);
            }
        }

        private bool TryRequestBatchPassword(int encryptedFileCount, out string workbookPassword)
        {
            workbookPassword = string.Empty;

            using (Form passwordDialog = new Form())
            using (Label instructionLabel = new Label())
            using (TextBox passwordTextBox = new TextBox())
            using (Button unlockButton = new Button())
            using (Button cancelButton = new Button())
            {
                passwordDialog.Text = "Unlock Excel Files";
                passwordDialog.StartPosition = FormStartPosition.CenterParent;
                passwordDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                passwordDialog.MinimizeBox = false;
                passwordDialog.MaximizeBox = false;
                passwordDialog.ShowInTaskbar = false;
                passwordDialog.ClientSize = new Size(410, 145);

                instructionLabel.AutoSize = false;
                instructionLabel.Location = new Point(15, 15);
                instructionLabel.Text = encryptedFileCount + " encrypted Excel file(s) detected." + Environment.NewLine +
                    "Enter the batch password to unlock them.";

                passwordTextBox.Location = new Point(15, 60);
                passwordTextBox.Size = new Size(380, 23);
                passwordTextBox.UseSystemPasswordChar = true;

                unlockButton.Text = "UNLOCK";
                unlockButton.Location = new Point(215, 100);
                unlockButton.Size = new Size(85, 28);
                unlockButton.DialogResult = DialogResult.OK;

                cancelButton.Text = "CANCEL";
                cancelButton.Location = new Point(310, 100);
                cancelButton.Size = new Size(85, 28);
                cancelButton.DialogResult = DialogResult.Cancel;

                passwordDialog.Controls.Add(instructionLabel);
                passwordDialog.Controls.Add(passwordTextBox);
                passwordDialog.Controls.Add(unlockButton);
                passwordDialog.Controls.Add(cancelButton);

                passwordDialog.AcceptButton = unlockButton;
                passwordDialog.CancelButton = cancelButton;

                passwordDialog.Shown += delegate
                {
                    passwordTextBox.Focus();
                };

                if (passwordDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(passwordTextBox.Text))
                {
                    MessageBox.Show(
                        "Please enter the Excel file password.",
                        "Excel Password",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return false;
                }
                workbookPassword = passwordTextBox.Text;
                return true;
            }
        }

        private void AddTemplate(string filePath, string workbookPassword, clsFunction requestIdFormatter, ref int nextIrControlNo)
        {
            IrImportRow irRow = BuildIrImportRow(filePath, workbookPassword);

            irRow.RequestId = requestIdFormatter.GenerateControlNo(nextIrControlNo, clsDefines.CONTROLID_PREFIX_IR, true);

            _irRows.Add(irRow);
            _sourceFiles.Add(filePath);

            nextIrControlNo++;
        }

        private static bool MayRequiredWorkbookPassword(Exception exception)
        {
            if (exception == null) return false;

            Exception baseException = exception.GetBaseException();

            string message = baseException.Message ?? string.Empty; return
                message.IndexOf("encrypted", StringComparison.OrdinalIgnoreCase) >= 0 ||
                message.IndexOf("password", StringComparison.OrdinalIgnoreCase) >= 0 ||
                message.IndexOf("valid Package file", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool TryGetFirstIrControlNo(out int controlNo)
        {
            controlNo = 0;

            if (dbAPI == null)
            {
                dbAPI = new clsAPI();
            }

            controlNo = dbAPI.GetControlID("IR Detail");

            return clsGlobalVariables.isAPIResponseOK && controlNo > 0;
        }

        private void LoadTemplates(IEnumerable<string> filePaths)
        {
            List<string> selectedFiles = filePaths.Where(filePath => !string.IsNullOrWhiteSpace(filePath))
                .Distinct(StringComparer.OrdinalIgnoreCase) .ToList();

            if (selectedFiles.Count == 0) return;

            List<string> errors = new List<string>();

            try
            {
                Cursor = Cursors.WaitCursor;

                int nextIrControlNo;

                if (!TryGetFirstIrControlNo (out nextIrControlNo))
                {
                    MessageBox.Show("Unable to generate Installation Request IDs. " +
                        "The MIS API did not return a valid control number.",
                        "Request ID Generation",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                clsFunction requestIdFormatter = new clsFunction();

                _irRows.Clear();
                _sourceFiles.Clear();


                List<string> encryptedFiles = new List<string>();

                foreach (string filePath in selectedFiles)
                {
                    try
                    {
                        AddTemplate(filePath, string.Empty, requestIdFormatter, ref nextIrControlNo);
                    }
                    catch (Exception ex)
                    {
                        if (MayRequiredWorkbookPassword(ex))
                        {
                            encryptedFiles.Add(filePath);
                        }
                        else
                        {
                            errors.Add( Path.GetFileName(filePath) +": " + ex.GetBaseException().Message);
                        }
                    }
                }

                if (encryptedFiles.Count > 0)
                {
                    string batchPassword;

                    if (TryRequestBatchPassword(encryptedFiles.Count, out batchPassword))
                    {
                        foreach (string filePath in encryptedFiles)
                        {
                            try
                            {
                                AddTemplate(filePath, batchPassword, requestIdFormatter, ref nextIrControlNo);
                            }
                            catch (Exception ex)
                            {
                                errors.Add(Path.GetFileName(filePath) +
                                    ": Unable to unlock or read the file. " +
                                    ex.GetBaseException().Message);
                            }
                        }

                        // Do not retain the password after processing.
                        batchPassword = string.Empty;
                    }
                    else
                    {
                        foreach (string filePath in encryptedFiles)
                        {
                            errors.Add(
                                Path.GetFileName(filePath) + ": Password entry was cancelled.");
                        }
                    }
                }

                txtSourcePath.Text = _sourceFiles.Count + "FILE(S) SELECTED";

                if (_irRows.Count > 0)
                {
                    lblValidation.Text = _irRows.Count + " VALID INSTALLATION REQUEST(S) LOADED";

                    lblValidation.ForeColor = Color.FromArgb(74, 222, 128);

                    lblStatus.Text = _irRows.Count + " IR ROW(S) READY FOR EDITING";

                    btnGenerate.Enabled = true;
                }
                else
                {
                    txtSourcePath.Clear();

                    lblValidation.Text = "NO VALID FILES WERE LOADED";

                    lblValidation.ForeColor = Color.FromArgb(248, 113, 113);

                    lblStatus.Text = "NO INSTALLATION REQUEST LOADED";

                    btnGenerate.Enabled = false;
                }

                if (errors.Count > 0)
                {
                    MessageBox.Show("Some files could not be loaded: \n\n- " + string.Join("\n- ", errors),
                       "File Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private IrImportRow BuildIrImportRow(string filePath, string workbookPassword)
        {
            BankTemplateReader reader = new BankTemplateReader();

            BankTemplateReadResult readResult = reader.Read(filePath, workbookPassword);

            IList<BankTemplateRow> sourceRows = readResult.Rows;

            BankTemplateFieldRules.ApplyDefaults(sourceRows, readResult.IsAdditionalTerminal);

            string bankDataInfo = BankTemplateJsonBuilder.Build(sourceRows, Path.GetFileName(filePath));

            return new IrImportRow()
            {
                SourceFilePath = filePath,
                SourceFileName = Path.GetFileName(filePath),
                BankDataInfo = bankDataInfo,
                RequestId = string.Empty,
                Vendor = "CITAS",
                RequestDate = string.Empty,
                Requestor = GetMappedValue(sourceRows, "Requestor"),
                RequestType = GetMappedValue(sourceRows, "Request Type"),
                RequestPrioritization = string.Empty,
                PosSetup = string.Empty,
                PosType = string.Empty,
                PosConnectionType = string.Empty,
                TargetInstallationDate = string.Empty,
                Remarks = GetMappedValue(sourceRows, "RM Instruction / Remarks"),
                Mid = GetMappedValue(sourceRows, "Merchant ID (MID)"),
                Tid = GetMappedValue(sourceRows, "Terminal ID (TID)"),
                BancnetTid = GetMappedValue(sourceRows, "Bancnet TID"),
                MerchantLocation = GetMappedValue(sourceRows, "Merchant Location/DBA Name"),
                Address = GetMappedValue(sourceRows, "Address"),
                City = GetMappedValue(sourceRows, "City"),
                Area1 = string.Empty,
                Area2 = string.Empty,
                ContactPerson = GetMappedValue(sourceRows, "Contact Person"),
                ContactNumber = GetMappedValue(sourceRows, "Contact Number")
            };
        }

        private static string GetMappedValue(IEnumerable<BankTemplateRow> sourceRows, string mccColumn)
        {
            IEnumerable<BankTemplateRow> mappedRows = sourceRows
                .Where(row => row.IsSelectable && row.IsSelected && string.Equals(row.MccColumn, mccColumn, StringComparison.OrdinalIgnoreCase));

            if (string.Equals(mccColumn,"Terminal ID (TID)", StringComparison.OrdinalIgnoreCase))
            {
                mappedRows = mappedRows.OrderBy(row => row.Tag.StartsWith("Sheet2 - ", StringComparison.OrdinalIgnoreCase)? 0: 1);
            }

            IEnumerable<string> mappedValues = mappedRows.Select(row => !string.IsNullOrWhiteSpace(row.MccValue)
               ? row.MccValue.Trim(): (row.Value ?? string.Empty).Trim())
                .Where(value => !string.IsNullOrWhiteSpace(value));

            bool isIdentifierColumn =
                string.Equals(mccColumn,"Merchant ID (MID)",StringComparison.OrdinalIgnoreCase) ||
                string.Equals(mccColumn,"Terminal ID (TID)",StringComparison.OrdinalIgnoreCase) ||
                string.Equals(mccColumn,"Bancnet TID",StringComparison.OrdinalIgnoreCase);

            if (isIdentifierColumn)
            {
                mappedValues = mappedValues.SelectMany(value => value.Split(new[] { ',' },
                 StringSplitOptions.RemoveEmptyEntries)).Select(value => value.Trim());
            }

            List<string> values = mappedValues.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            return string.Join(", ", values);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            grdBankFields.EndEdit();

            _irRows.Clear();
            _sourceFiles.Clear();
            grdBankFields.ClearSelection();
            grdBankFields.CurrentCell = null;
            grdBankFields.HorizontalScrollingOffset = 0;

            txtSourcePath.Clear();
            lblValidation.Text = "NO FILES SELECTED";
            lblValidation.ForeColor = Color.DimGray;
            lblStatus.Text = "NO FILES SELECTED";

            btnGenerate.Enabled = false;

            btnBrowse.Focus();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            grdBankFields.EndEdit();

            CurrencyManager currencyManager = BindingContext[_irRows] as CurrencyManager;

            if (currencyManager != null) currencyManager.EndCurrentEdit();

            if (_irRows.Count == 0)
            {
                MessageBox.Show("Please load at least one valid Installation Request before exporting.",
                    "IR Import Template", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<string> missingFields = BuildMissingFieldSummary(_irRows);

            if (missingFields.Count > 0)
            {
                string validationMessage = "Unable to generate the IR Import Template because " +
                    "the following mandatory fields are missing:\n\n" +
                    string.Join("\n", missingFields) +
                    "\n\nComplete all required fields in every row " +
                    "before generating the template.";

                MessageBox.Show(validationMessage, "Incomplete Installation Request Data",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Save IR Import Template";
                dialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                dialog.DefaultExt = "xlsx";
                dialog.AddExtension = true;
                dialog.OverwritePrompt = true;
                dialog.FileName = BuildDefaultOutputFileName();

                string sourceDirectory = _sourceFiles.Count > 0 ? Path.GetDirectoryName(_sourceFiles[0]) : string.Empty;

                if (!string.IsNullOrWhiteSpace(sourceDirectory) && Directory.Exists(sourceDirectory))
                {
                    dialog.InitialDirectory = sourceDirectory;
                }

                if (dialog.ShowDialog() != DialogResult.OK) return;

                try
                {
                    Cursor = Cursors.WaitCursor;
                    btnGenerate.Enabled = false;
                    lblStatus.Text = "GENERATING IR IMPORT TEMPLATE...";

                    BankTemplateExcelExporter exporter = new BankTemplateExcelExporter();

                    exporter.Export(dialog.FileName, _irRows.ToList());

                    lblStatus.Text = "IR IMPORT TEMPLATE GENERATED";

                    string message = _irRows.Count +
                        " Installation Request row(s) were exported successfully.\n\n" +
                        "Saved to:\n" + dialog.FileName;

                    MessageBox.Show( message, "IR Import Template", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    lblStatus.Text = "UNABLE TO GENERATE IR IMPORT TEMPLATE";

                    MessageBox.Show(ex.Message, "IR Import Template", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                    btnGenerate.Enabled = _irRows.Count > 0;
                }
            }
        }

        private static List<string> BuildMissingFieldSummary(IEnumerable<IrImportRow> rows)
        {
            List<string> summary = new List<string>();
            int rowNumber = 1;

            foreach (IrImportRow row in rows)
            {
                List<string> missing = new List<string>();

                AddMissingField(missing, "Request ID", row.RequestId);
                AddMissingField(missing, "Vendor", row.Vendor);
                AddMissingField(missing, "Request Date", row.RequestDate);
                AddMissingField(missing, "Requestor", row.Requestor);
                AddMissingField(missing, "Request Type", row.RequestType);
                AddMissingField(missing, "Request Prioritization", row.RequestPrioritization);
                AddMissingField(missing, "POS Setup", row.PosSetup);
                AddMissingField(missing, "POS Type", row.PosType);
                AddMissingField(missing, "POS Connection Type", row.PosConnectionType);
                AddMissingField(missing, "Target Installation Date",row.TargetInstallationDate);
                AddMissingField(missing, "Merchant ID (MID)", row.Mid);
                AddMissingField(missing, "Terminal ID (TID)", row.Tid);
                AddMissingField(missing, "Merchant Location/DBA Name",row.MerchantLocation);
                AddMissingField(missing, "Address", row.Address);
                AddMissingField(missing, "City", row.City);
                AddMissingField(missing, "Area 1", row.Area1);
                AddMissingField(missing, "Area 2", row.Area2);
                AddMissingField(missing, "Contact Person", row.ContactPerson);
                AddMissingField(missing, "Contact Number", row.ContactNumber);

                if (missing.Count > 0)
                {
                    summary.Add("IR Row " + rowNumber + ": " + string.Join(", ", missing));
                }

                rowNumber++;
            }

            return summary;
        }

        private static void AddMissingField(ICollection<string> missingFields, string fieldName, string value)
        {
            string normalizedValue = (value ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(normalizedValue) || string.Equals(normalizedValue, clsFunction.sDefaultSelect,
                    StringComparison.OrdinalIgnoreCase) || string.Equals(
                    normalizedValue, "[NOT SPECIFIED]", StringComparison.OrdinalIgnoreCase))
            {
                missingFields.Add(fieldName);
            }
        }

        private string BuildDefaultOutputFileName()
        {
            string sourceName;

            if (_sourceFiles.Count == 1)
            {
                sourceName = Path.GetFileNameWithoutExtension(_sourceFiles[0]);
            }
            else if (_sourceFiles.Count > 1)
            {
                sourceName =_sourceFiles.Count + "_REQUESTS";
            }
            else
            {
                sourceName = "REQUEST";
            }

            if (string.IsNullOrWhiteSpace(sourceName))
                sourceName = "REQUEST";

            foreach (char invalidCharacter in
                Path.GetInvalidFileNameChars())
            {
                sourceName = sourceName.Replace(
                    invalidCharacter,
                    '_');
            }
            return "IR_IMPORT_" + sourceName + ".xlsx";
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmBankTemplateGen_Load( object sender, EventArgs e)
        {
            LoadPosTypes();
        }

        private void LoadPosTypes()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                dbAPI = new clsAPI();

                ComboBox temporaryComboBox = new ComboBox();

                dbAPI.FillComboBoxTerminalModel(temporaryComboBox);

                colIrPosType.Items.Clear();

                foreach (object item in temporaryComboBox.Items)
                {
                    string description = Convert.ToString(item);

                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        colIrPosType.Items.Add(description);
                    }
                }

                if (colIrPosType.Items.Count == 0)
                {
                    MessageBox.Show("No POS Type records were returned by the database.", "POS Type",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load POS Types.\n\n" + ex.Message,
                    "POS Type", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void grdBankFields_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (grdBankFields.IsCurrentCellDirty && (grdBankFields.CurrentCell is DataGridViewCheckBoxCell ||
                 grdBankFields.CurrentCell is DataGridViewComboBoxCell))
            {
                grdBankFields.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        // The on-screen JSON preview was part of the previous field-detail UI.
        // JSON generation remains active in BuildIrImportRow for exported metadata.
    }
}
