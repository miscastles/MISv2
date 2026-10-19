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
        private readonly clsFile dbFile = new clsFile();
        private int _lastSelectionRowIndex = -1;
        private bool _isUpdatingRowSelection;

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
            btnApplyToSelected.Click += btnApplyToSelected_Click;

            grdBankFields.CurrentCellDirtyStateChanged += grdBankFields_CurrentCellDirtyStateChanged;
            grdBankFields.CellValueChanged += grdBankFields_CellValueChanged;
            grdBankFields.ColumnHeaderMouseClick += grdBankFields_ColumnHeaderMouseClick;
            grdBankFields.EditingControlShowing += grdBankFields_EditingControlShowing;

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
                e.Effect = DragDropEffects.None; return;
            }

            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (files == null || files.Length == 0)
            {
                e.Effect = DragDropEffects.None; return;
            }

            bool allFilesAreValid = files.All(filePath => File.Exists(filePath) && string.Equals(Path.GetExtension(filePath), ".xlsx", StringComparison.OrdinalIgnoreCase));
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
            colIrPosSetup.ReadOnly = false;
            colIrPosSetup.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            colIrArea2.MaxInputLength = 6;
            colIrSelected.HeaderCell.ToolTipText = "Click to select or clear all rows. Shift-click a row checkbox to select a range.";
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

            DataGridViewTextBoxColumn bancnetMidColumn = new DataGridViewTextBoxColumn
            {
                Name = "colIrBancnetMid",
                HeaderText = "Bancnet MID",
                DataPropertyName = "BancnetMid",
                ReadOnly = true,
                Width = 150
            };

            grdBankFields.Columns.Insert(bancnetTidColumn.Index + 1, bancnetMidColumn);

            DataGridViewTextBoxColumn alipayTidColumn = new DataGridViewTextBoxColumn
            {
                Name = "colIrAlipayTid",
                HeaderText = "Alipay TID",
                DataPropertyName = "AlipayTid",
                Width = 180
            };

            grdBankFields.Columns.Insert(bancnetMidColumn.Index + 1, alipayTidColumn);

            DataGridViewTextBoxColumn alipayMidColumn = new DataGridViewTextBoxColumn
            {
                Name = "colIrAlipayMid",
                HeaderText = "Alipay MID",
                DataPropertyName = "AlipayMid",
                Width = 150
            };

            grdBankFields.Columns.Insert(alipayTidColumn.Index + 1, alipayMidColumn);
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

        private static void SetHeaderColor(DataGridViewColumn column, Color backColor)
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
                HeaderCell = (DataGridViewColumnHeaderCell)originalColumn.HeaderCell.Clone(),
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

            UpdateBulkEditControls();
        }

        private void UpdateBulkEditControls()
        {
            int selectedCount = _irRows.Count(row => row.IsSelected);
            lblSelectedRows.Text = selectedCount + " ROW(S) SELECTED";
            btnApplyToSelected.Enabled = selectedCount > 1;
        }

        private void grdBankFields_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_isUpdatingRowSelection || e.RowIndex < 0 || e.ColumnIndex != colIrSelected.Index)
            {
                return;
            }

            bool isSelected = Convert.ToBoolean(grdBankFields.Rows[e.RowIndex].Cells[e.ColumnIndex].Value ?? false);

            if ((ModifierKeys & Keys.Shift) == Keys.Shift && _lastSelectionRowIndex >= 0)
            {
                int firstRow = Math.Min(_lastSelectionRowIndex, e.RowIndex);
                int lastRow = Math.Max(_lastSelectionRowIndex, e.RowIndex);

                _isUpdatingRowSelection = true;
                try
                {
                    for (int rowIndex = firstRow; rowIndex <= lastRow; rowIndex++)
                    {
                        grdBankFields.Rows[rowIndex].Cells[colIrSelected.Index].Value = isSelected;
                    }
                }
                finally
                {
                    _isUpdatingRowSelection = false;
                }
            }

            _lastSelectionRowIndex = e.RowIndex;
            UpdateBulkEditControls();
        }

        private void grdBankFields_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != colIrSelected.Index || grdBankFields.Rows.Count == 0)
            {
                return;
            }

            bool selectAll = _irRows.Any(row => !row.IsSelected);
            _isUpdatingRowSelection = true;
            try
            {
                foreach (DataGridViewRow gridRow in grdBankFields.Rows)
                {
                    gridRow.Cells[colIrSelected.Index].Value = selectAll;
                }
            }
            finally
            {
                _isUpdatingRowSelection = false;
            }

            _lastSelectionRowIndex = -1;
            UpdateBulkEditControls();
        }

        private void btnApplyToSelected_Click(object sender, EventArgs e)
        {
            DataGridViewCell sourceCell = grdBankFields.CurrentCell;
            if (sourceCell == null || sourceCell.RowIndex < 0)
            {
                MessageBox.Show("Edit a cell in one of the selected rows first.",
                    "Bulk Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            grdBankFields.EndEdit();
            CurrencyManager currencyManager = BindingContext[_irRows] as CurrencyManager;
            if (currencyManager != null) currencyManager.EndCurrentEdit();

            DataGridViewColumn sourceColumn = grdBankFields.Columns[sourceCell.ColumnIndex];
            string dataPropertyName = sourceColumn.DataPropertyName;

            if (sourceCell.ReadOnly || sourceColumn.ReadOnly || sourceColumn == colIrSelected ||
                string.IsNullOrWhiteSpace(dataPropertyName) || IsUniqueIdentifierField(dataPropertyName))
            {
                MessageBox.Show("The " + sourceColumn.HeaderText + " column cannot be applied in bulk. " +
                    "Choose an editable, non-identifier field.", "Bulk Edit",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            IrImportRow sourceRow = grdBankFields.Rows[sourceCell.RowIndex].DataBoundItem as IrImportRow;
            List<DataGridViewRow> selectedRows = grdBankFields.Rows.Cast<DataGridViewRow>()
                .Where(row => row.DataBoundItem is IrImportRow && ((IrImportRow)row.DataBoundItem).IsSelected)
                .ToList();

            if (sourceRow == null || !sourceRow.IsSelected)
            {
                MessageBox.Show("The row containing the value to copy must also be checked.",
                    "Bulk Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectedRows.Count < 2)
            {
                MessageBox.Show("Select at least two rows before applying a bulk edit.",
                    "Bulk Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            object sourceValue = sourceCell.Value;
            string displayValue = Convert.ToString(sourceValue);
            if (string.IsNullOrEmpty(displayValue)) displayValue = "(blank)";

            DialogResult confirmation = MessageBox.Show("Apply " + sourceColumn.HeaderText + " = " + displayValue +
                " to " + selectedRows.Count + " selected rows?", "Confirm Bulk Edit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmation != DialogResult.Yes) return;

            foreach (DataGridViewRow targetRow in selectedRows)
            {
                targetRow.Cells[sourceCell.ColumnIndex].Value = sourceValue;
            }

            grdBankFields.EndEdit();
            if (currencyManager != null) currencyManager.EndCurrentEdit();
            _irRows.ResetBindings();
            lblStatus.Text = sourceColumn.HeaderText.ToUpperInvariant() + " APPLIED TO " + selectedRows.Count + " SELECTED ROW(S)";
        }

        private static bool IsUniqueIdentifierField(string dataPropertyName)
        {
            return string.Equals(dataPropertyName, "RequestId", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(dataPropertyName, "Mid", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(dataPropertyName, "Tid", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(dataPropertyName, "BancnetTid", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(dataPropertyName, "BancnetMid", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(dataPropertyName, "AlipayTid", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(dataPropertyName, "AlipayMid", StringComparison.OrdinalIgnoreCase);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Select one or more Issuance Templates";

                dialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";

                dialog.CheckFileExists = true;

                dialog.Multiselect = true; // allows multiple excel files. 

                dbFile.CheckFolder(dbFile.sImportPath);
                dialog.InitialDirectory = dbFile.sImportPath;
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() != DialogResult.OK) return;

                LoadTemplates(dialog.FileNames);
            }
        }

        private bool TryRequestBatchPassword(IList<string> encryptedFiles, out string workbookPassword)
        {
            workbookPassword = string.Empty;

            using (Form passwordDialog = new Form())
            using (Label instructionLabel = new Label())
            using (ToolTip instructionToolTip = new ToolTip())
            using (Label passwordLabel = new Label())
            using (TextBox passwordTextBox = new TextBox())
            using (CheckBox hidePasswordCheckBox = new CheckBox())
            using (Button unlockButton = new Button())
            using (Button cancelButton = new Button())
            {
                int encryptedFileCount = encryptedFiles == null ? 0 : encryptedFiles.Count;
                string instructionText = encryptedFileCount == 1
                    ? "'" + Path.GetFileName(encryptedFiles[0]) + "' is protected."
                    : encryptedFileCount + " encrypted Excel files are protected.";

                passwordDialog.Text = "Password";
                passwordDialog.StartPosition = FormStartPosition.CenterParent;
                passwordDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                passwordDialog.MinimizeBox = false;
                passwordDialog.MaximizeBox = false;
                passwordDialog.ShowInTaskbar = false;
                passwordDialog.ClientSize = new Size(300, 120);

                instructionLabel.AutoSize = false;
                instructionLabel.AutoEllipsis = true;
                instructionLabel.Location = new Point(8, 11);
                instructionLabel.Size = new Size(284, 18);
                instructionLabel.Text = instructionText;
                instructionToolTip.SetToolTip(instructionLabel, instructionText);

                passwordLabel.AutoSize = true;
                passwordLabel.Location = new Point(8, 43);
                passwordLabel.Text = "Password:";

                passwordTextBox.Location = new Point(72, 39);
                passwordTextBox.Size = new Size(220, 23);
                passwordTextBox.UseSystemPasswordChar = true;

                hidePasswordCheckBox.Text = "Show";
                hidePasswordCheckBox.AutoSize = true;
                hidePasswordCheckBox.Location = new Point(72, 76);
                hidePasswordCheckBox.Checked = false;
                hidePasswordCheckBox.CheckedChanged += delegate
                {
                    passwordTextBox.UseSystemPasswordChar = !hidePasswordCheckBox.Checked;
                    hidePasswordCheckBox.Text = hidePasswordCheckBox.Checked ? "Hide" : "Show";
                    passwordTextBox.Focus();
                    passwordTextBox.SelectionStart = passwordTextBox.TextLength;
                };

                unlockButton.Text = "OK";
                unlockButton.Location = new Point(136, 73);
                unlockButton.Size = new Size(75, 28);
                unlockButton.DialogResult = DialogResult.OK;

                cancelButton.Text = "Cancel";
                cancelButton.Location = new Point(217, 73);
                cancelButton.Size = new Size(75, 28);
                cancelButton.DialogResult = DialogResult.Cancel;

                passwordDialog.Controls.Add(instructionLabel);
                passwordDialog.Controls.Add(passwordLabel);
                passwordDialog.Controls.Add(passwordTextBox);
                passwordDialog.Controls.Add(hidePasswordCheckBox);
                passwordDialog.Controls.Add(unlockButton);
                passwordDialog.Controls.Add(cancelButton);
                passwordDialog.AcceptButton = unlockButton;
                passwordDialog.CancelButton = cancelButton;
                passwordDialog.Shown += delegate
                {
                    passwordTextBox.Focus();
                };

                while (true)
                {
                    if (passwordDialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return false;
                    }

                    if (!string.IsNullOrEmpty(passwordTextBox.Text))
                    {
                        workbookPassword = passwordTextBox.Text;
                        return true;
                    }

                    MessageBox.Show("Please enter the Excel file password.", "Excel Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    passwordTextBox.Clear();
                    passwordTextBox.Focus();
                }
            }
        }

        private void AddTemplate(string filePath, string workbookPassword, clsFunction requestIdFormatter, ref int nextIrControlNo)
        {
            IList<IrImportRow> irRows = ExpandIdentifierPairs(BuildIrImportRow(filePath, workbookPassword));

            foreach (IrImportRow irRow in irRows)
            {
                irRow.RequestId = requestIdFormatter.GenerateControlNo(nextIrControlNo, clsDefines.CONTROLID_PREFIX_IR, true);
                _irRows.Add(irRow);
                nextIrControlNo++;
            }

            _sourceFiles.Add(filePath);
        }

        private static IList<IrImportRow> ExpandIdentifierPairs(IrImportRow sourceRow)
        {
            if (sourceRow == null) throw new ArgumentNullException("sourceRow");

            List<string> terminalIds = SplitIdentifierValues(sourceRow.Tid);
            List<string> bancnetTerminalIds = SplitIdentifierValues(sourceRow.BancnetTid);

            if (terminalIds.Count != bancnetTerminalIds.Count)
            {
                throw new InvalidDataException(
                    "Terminal ID and Bancnet TID counts do not match. " +
                    "Terminal ID count: " + terminalIds.Count + "; " +
                    "Bancnet TID count: " + bancnetTerminalIds.Count + ".");
            }

            if (terminalIds.Count == 0)
            {
                return new List<IrImportRow> { sourceRow };
            }

            List<IrImportRow> expandedRows = new List<IrImportRow>(terminalIds.Count);

            for (int index = 0; index < terminalIds.Count; index++)
            {
                IrImportRow expandedRow = new IrImportRow(sourceRow)
                {
                    Tid = terminalIds[index],
                    BancnetTid = bancnetTerminalIds[index]
                };

                expandedRows.Add(expandedRow);
            }

            return expandedRows;
        }

        private static List<string> SplitIdentifierValues(string value)
        {
            return (value ?? string.Empty)
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(identifier => identifier.Trim())
                .Where(identifier => !string.IsNullOrWhiteSpace(identifier))
                .ToList();
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

        private static bool IsWorkbookOpen(string filePath)
        {
            try
            {
                string directory = Path.GetDirectoryName(filePath);
                string lockFilePath = Path.Combine(directory ?? string.Empty, "~$" + Path.GetFileName(filePath));

                if (File.Exists(lockFilePath))
                {
                    return true;
                }

                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
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

        private void PopulateExcelTemplateList(IList<string> filePaths)
        {
            lvwExcelTemplate.BeginUpdate();
            try
            {
                lvwExcelTemplate.Items.Clear();

                int count = 1;
                foreach (string filePath in filePaths)
                {
                    ListViewItem item = new ListViewItem(count.ToString());
                    item.SubItems.Add(Path.GetFileName(filePath));
                    item.SubItems.Add(filePath);
                    item.Tag = filePath;
                    item.ToolTipText = filePath;
                    lvwExcelTemplate.Items.Add(item);
                    count++;
                }
            }
            finally
            {
                lvwExcelTemplate.EndUpdate();
            }
        }

        private void lvwExcelTemplate_DoubleClick(object sender, EventArgs e)
        {
            if (lvwExcelTemplate.SelectedItems.Count == 0) return;

            string filePath = lvwExcelTemplate.SelectedItems[0].Tag as string;
            if (string.IsNullOrWhiteSpace(filePath)) return;

            txtSourcePath.Text = filePath;
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            string filePath = (txtSourcePath.Text ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                MessageBox.Show("Double-click a file in the list first to select its path.",
                    "Locate File", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Opens Explorer with the file highlighted
                System.Diagnostics.Process.Start("explorer.exe", "/select,\"" + filePath + "\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open the file location.\n\n" + ex.Message, "Locate File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTemplates(IEnumerable<string> filePaths)
        {
            List<string> selectedFiles = filePaths.Where(filePath => !string.IsNullOrWhiteSpace(filePath)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (selectedFiles.Count == 0) return;
            List<string> errors = new List<string>();

            PopulateExcelTemplateList(selectedFiles);

            try
            {
                Cursor = Cursors.WaitCursor;
                int nextIrControlNo;
                if (!TryGetFirstIrControlNo(out nextIrControlNo))
                {
                    MessageBox.Show("Unable to generate Installation Request IDs. " + "The MIS API did not return a valid control number.",
                        "Request ID Generation", MessageBoxButtons.OK, MessageBoxIcon.Error); return;
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
                            errors.Add(Path.GetFileName(filePath) + ": " + ex.GetBaseException().Message);
                        }
                    }
                }

                if (encryptedFiles.Count > 0)
                {
                    while (true)
                    {
                        string batchPassword;

                        if (!TryRequestBatchPassword(encryptedFiles, out batchPassword))
                        {
                            break;
                        }

                        int rowsBeforeAttempt = _irRows.Count;
                        int sourceFilesBeforeAttempt = _sourceFiles.Count;
                        int errorsBeforeAttempt = errors.Count;
                        int controlNoBeforeAttempt = nextIrControlNo;
                        string retryMessage = string.Empty;
                        string retryTitle = "Excel Password";

                        foreach (string filePath in encryptedFiles)
                        {
                            try
                            {
                                AddTemplate(filePath, batchPassword, requestIdFormatter, ref nextIrControlNo);
                            }
                            catch (Exception ex)
                            {
                                if (IsWorkbookOpen(filePath))
                                {
                                    retryTitle = "Excel File Is Open";
                                    retryMessage = "The Excel file \"" + Path.GetFileName(filePath) + "\" appears to be open in Excel. " +
                                        "Close the workbook first, then enter the password again. This does not necessarily mean that the password is incorrect.";
                                    break;
                                }

                                if (MayRequiredWorkbookPassword(ex))
                                {
                                    retryMessage = "Incorrect Excel file password. Please try again, or close the password window to cancel.";
                                    break;
                                }

                                errors.Add(Path.GetFileName(filePath) + ": Unable to read the file. " + ex.GetBaseException().Message);
                            }
                        }

                        // Do not retain the password after processing.
                        batchPassword = string.Empty;

                        if (string.IsNullOrEmpty(retryMessage))
                        {
                            break;
                        }

                        if (_irRows.Count > rowsBeforeAttempt)
                        {
                            while (_irRows.Count > rowsBeforeAttempt)
                            {
                                _irRows.RemoveAt(_irRows.Count - 1);
                            }
                        }

                        if (_sourceFiles.Count > sourceFilesBeforeAttempt)
                        {
                            _sourceFiles.RemoveRange(sourceFilesBeforeAttempt, _sourceFiles.Count - sourceFilesBeforeAttempt);
                        }

                        if (errors.Count > errorsBeforeAttempt)
                        {
                            errors.RemoveRange(errorsBeforeAttempt, errors.Count - errorsBeforeAttempt);
                        }

                        nextIrControlNo = controlNoBeforeAttempt;

                        MessageBox.Show(retryMessage, retryTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                txtSourcePath.Clear();
                lblTotal.Text = _irRows.Count.ToString();

                if (_irRows.Count > 0)
                {
                    lblValidation.Text = _irRows.Count + " VALID INSTALLATION REQUEST(S) LOADED";
                    lblValidation.ForeColor = Color.FromArgb(102, 0, 0);
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
                    MessageBox.Show("Some files could not be loaded: \n\n- " + string.Join("\n- ", errors), "File Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                Mid = GetMerchantMid(sourceRows),
                Tid = GetMappedValue(sourceRows, "Terminal ID (TID)"),
                BancnetTid = GetMappedValue(sourceRows, "Bancnet TID"),
                AlipayTid = GetPreferredMappedValue(sourceRows, "Alipay TID"),
                AlipayMid = GetPreferredMappedValue(sourceRows, "Alipay MID"),
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
            IEnumerable<BankTemplateRow> mappedRows = sourceRows.Where(row => row.IsSelectable && row.IsSelected && string.Equals(row.MccColumn, mccColumn, StringComparison.OrdinalIgnoreCase));
            IEnumerable<string> mappedValues = mappedRows.Select(row => !string.IsNullOrWhiteSpace(row.MccValue) ? row.MccValue.Trim() : (row.Value ?? string.Empty).Trim())
                .Where(value => !string.IsNullOrWhiteSpace(value));

            bool isIdentifierColumn =
                string.Equals(mccColumn, "Merchant ID (MID)", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(mccColumn, "Terminal ID (TID)", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(mccColumn, "Bancnet TID", StringComparison.OrdinalIgnoreCase);

            if (isIdentifierColumn)
            {
                mappedValues = mappedValues.SelectMany(value => value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)).Select(value => value.Trim());
            }
            List<string> values = mappedValues.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            return string.Join(", ", values);
        }

        private static string GetPreferredMappedValue(IEnumerable<BankTemplateRow> sourceRows, string mccColumn)
        {
            List<BankTemplateRow> rows = sourceRows.ToList();
            string primaryValue = GetMappedValue(rows.Where(row => !IsPreviousInformationRow(row)), mccColumn);

            if (!string.IsNullOrWhiteSpace(primaryValue))
            {
                return primaryValue;
            }

            return GetMappedValue(rows.Where(IsPreviousInformationRow), mccColumn);
        }

        private static bool IsPreviousInformationRow(BankTemplateRow row)
        {
            return row != null && (row.Section ?? string.Empty).StartsWith("Previous Merchant Information (", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetMerchantMid(IEnumerable<BankTemplateRow> sourceRows)
        {
            List<BankTemplateRow> rows = sourceRows.ToList();
            string primaryMid = GetMappedValue(rows.Where(row => !IsSecondaryIdentifierTag(row.Tag, "MID")), "Merchant ID (MID)");

            if (!string.IsNullOrWhiteSpace(primaryMid))
            {
                return primaryMid;
            }

            return GetMappedValue(rows.Where(row => IsSecondaryIdentifierTag(row.Tag, "MID")), "Merchant ID (MID)");
        }

        private static bool IsSecondaryIdentifierTag(string tag, string identifier)
        {
            return (tag ?? string.Empty).EndsWith(" - " + identifier, StringComparison.OrdinalIgnoreCase);
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
            _lastSelectionRowIndex = -1;
            UpdateBulkEditControls();

            txtSourcePath.Clear();
            lblValidation.Text = "NO FILES SELECTED";
            lblValidation.ForeColor = Color.DimGray;
            lblStatus.Text = "NO FILES SELECTED";

            btnGenerate.Enabled = false;
            btnBrowse.Focus();

            lvwExcelTemplate.Items.Clear();
            lblTotal.Text = "0";
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
                string validationMessage = "Unable to generate the IR Import Template because " + "the following mandatory fields are missing:\n\n" +
                string.Join("\n", missingFields) + "\n\nComplete all required fields in every row " + "before generating the template.";

                MessageBox.Show(validationMessage, "Incomplete Installation Request Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Save IR Import Template";
                dialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                dialog.DefaultExt = "xlsx";
                dialog.AddExtension = true;
                dialog.OverwritePrompt = true;

                dbFile.CheckFolder(dbFile.sImportPath);
                dialog.InitialDirectory = dbFile.sImportPath;
                dialog.FileName = Path.Combine(dbFile.sImportPath, BuildDefaultOutputFileName());
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() != DialogResult.OK) return;
                try
                {
                    Cursor = Cursors.WaitCursor;
                    btnGenerate.Enabled = false;
                    lblStatus.Text = "GENERATING IR IMPORT TEMPLATE...";

                    BankTemplateExcelExporter exporter = new BankTemplateExcelExporter();
                    exporter.Export(dialog.FileName, _irRows.ToList());
                    lblStatus.Text = "IR IMPORT TEMPLATE GENERATED";
                    string message = _irRows.Count + " Installation Request row(s) were exported successfully.\n\n" + "Saved to:\n" + dialog.FileName;
                    MessageBox.Show(message, "IR Import Template", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                AddMissingField(missing, "Target Installation Date", row.TargetInstallationDate);
                AddMissingField(missing, "Merchant ID (MID)", row.Mid);
                AddMissingField(missing, "Terminal ID (TID)", row.Tid);
                AddMissingField(missing, "Merchant Location/DBA Name", row.MerchantLocation);
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
               StringComparison.OrdinalIgnoreCase) || string.Equals(normalizedValue, "[NOT SPECIFIED]", StringComparison.OrdinalIgnoreCase))
            {
                missingFields.Add(fieldName);
            }
        }

        private string BuildDefaultOutputFileName()
        {
            string bankCode = (clsSearch.ClassBankCode ?? string.Empty).Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(bankCode)) bankCode = "bank";

            foreach (char invalidCharacter in Path.GetInvalidFileNameChars())
            {
                bankCode = bankCode.Replace(invalidCharacter, '_');
            }

            return "ir_" + bankCode + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmBankTemplateGen_Load(object sender, EventArgs e)
        {
            LoadPosTypes();
            LoadArea1Regions();
            LoadBankTemplateTypeDropdowns();
        }

        private const int REQUEST_PRIORITIZATION_TYPE = 29;
        private const int POS_SETUP_TYPE = 30;
        private const int POS_CONNECTION_TYPE = 31;
        private const int EXISTING_POS_SETUP_TYPE = 6;
        private const int CASHOUT_SETUP_TYPE_ID = 19;
        private const int RETAIL_SETUP_TYPE_ID = 22;

        private void LoadBankTemplateTypeDropdowns()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (dbAPI == null)
                {
                    dbAPI = new clsAPI();
                }

                colIrRequestPrioritization.Items.Clear();
                colIrPosSetup.Items.Clear();
                colIrPosConnectionType.Items.Clear();

                int prioritizationCount = AppendTypeGroupItems(colIrRequestPrioritization, REQUEST_PRIORITIZATION_TYPE);

                int posSetupCount = AppendTypeGroupItems(colIrPosSetup, POS_SETUP_TYPE);

                
                posSetupCount += AppendTypeGroupItems(colIrPosSetup, EXISTING_POS_SETUP_TYPE, CASHOUT_SETUP_TYPE_ID, RETAIL_SETUP_TYPE_ID);

                int connectionCount = AppendTypeGroupItems(colIrPosConnectionType, POS_CONNECTION_TYPE);

                List<string> emptyDropdowns = new List<string>();

                if (prioritizationCount == 0) emptyDropdowns.Add("Request Prioritization");

                if (posSetupCount == 0) emptyDropdowns.Add("POS Setup");

                if (connectionCount == 0) emptyDropdowns.Add("POS Connection Type");

                if (emptyDropdowns.Count > 0)
                {
                    MessageBox.Show("No active database records were returned for:\n\n" +string.Join("\n", emptyDropdowns), "Bank Template Dropdowns",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load Bank Template dropdown values.\n\n" + ex.Message, "Bank Template Dropdowns",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private int AppendTypeGroupItems(DataGridViewComboBoxColumn column, int groupType, params int[] includedTypeIds)
        {
            dbAPI.ExecuteAPI("GET", "View", "Type", groupType.ToString(),"Type", "", "ViewType");

            if (!clsGlobalVariables.isAPIResponseOK || dbAPI.isNoRecordFound())
            {
                return 0;
            }

            HashSet<int> typeIdFilter = includedTypeIds != null && includedTypeIds.Length > 0? new HashSet<int>(includedTypeIds): null;

            int addedCount = 0;
            int recordCount = Math.Min(clsArray.TypeID.Length, clsArray.TypeDescription.Length);

            for (int index = 0; index < recordCount; index++)
            {
                int typeId;

                if (!int.TryParse(clsArray.TypeID[index], out typeId))
                {
                    continue;
                }

                if (typeIdFilter != null && !typeIdFilter.Contains(typeId))
                {
                    continue;
                }

                string description = clsArray.TypeDescription[index];

                if (string.IsNullOrWhiteSpace(description))
                {
                    continue;
                }

                description = description.Trim();

                bool alreadyAdded = column.Items
                    .Cast<object>()
                    .Any(item => string.Equals(Convert.ToString(item), description, StringComparison.OrdinalIgnoreCase));

                if (!alreadyAdded)
                {
                    column.Items.Add(description);
                    addedCount++;
                }
            }
            return addedCount;
        }

        private void LoadPosTypes()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (dbAPI == null)
                {
                    dbAPI = new clsAPI();
                }

                dbAPI.ExecuteAPI("GET", "View", "Application Info List", "", "Advance Detail", "", "ViewAdvanceDetail");

                colIrPosType.Items.Clear();

                if (!clsGlobalVariables.isAPIResponseOK || dbAPI.isNoRecordFound())
                {
                    MessageBox.Show("No active POS Type records were returned by the database.", "POS Type",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                colIrPosType.Items.Add(clsFunction.sDefaultSelect);

                foreach (string detailInfo in clsArray.detail_info)
                {
                    string description = dbAPI.GetValueFromJSONString(
                        detailInfo,
                        clsDefines.TAG_TerminalModel);

                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        colIrPosType.Items.Add(description.Trim());
                    }
                }

                if (colIrPosType.Items.Count == 1)
                {
                    MessageBox.Show("No active POS Type records were returned by the database.", "POS Type",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load POS Types.\n\n" + ex.Message, "POS Type",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void grdBankFields_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (grdBankFields.IsCurrentCellDirty && (grdBankFields.CurrentCell is DataGridViewCheckBoxCell || grdBankFields.CurrentCell is DataGridViewComboBoxCell))
            {
                grdBankFields.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        private void grdBankFields_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            ComboBox comboBox = e.Control as ComboBox;

            if (comboBox != null)
            {
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBox.AutoCompleteMode = AutoCompleteMode.None;
                comboBox.AutoCompleteSource = AutoCompleteSource.None;
                return;
            }

            TextBox textBox = e.Control as TextBox;
            if (textBox == null)
            {
                return;
            }

            textBox.MaxLength = 32767;

            if (grdBankFields.CurrentCell != null && grdBankFields.CurrentCell.OwningColumn == colIrArea2)
            {
                textBox.MaxLength = 6;
            }
        }

        private void LoadArea1Regions()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (dbAPI == null)
                {
                    dbAPI = new clsAPI();
                }

                dbAPI.GetRegionList("View", "Region List", "", "Region");

                colIrArea1.Items.Clear();

                if (!clsGlobalVariables.isAPIResponseOK || dbAPI.isNoRecordFound())
                {
                    MessageBox.Show("No active Area 1 records were returned by the database.", "Area 1",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                foreach (string region in clsArray.Region
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .Select(value => value.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    colIrArea1.Items.Add(region);
                }

                if (colIrArea1.Items.Count == 0)
                {
                    MessageBox.Show("No active area 1 records were returned by the database.", "Area 1",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load Area 1 records. \n\n" + ex.Message, "Area 1",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
}
