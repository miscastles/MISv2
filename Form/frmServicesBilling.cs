using MIS.AppConnection;
using MIS.AppMainActivity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MIS.AppActivity.BillingActivity;
using static MIS.Function.AppUtilities;

namespace MIS
{
    public partial class frmServicesBilling : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;

        DataTable LeasingBilling { get; set; }
        DataTable ServicingBilling { get; set; }
        DataTable TleBilling { get; set; }
        DataTable WarehouseBilling { get; set; }

        DataTable InvoiceDetails { get; set; }
        DataTable RemovedRecords { get; set; }

        class BillingMessageConfig
        {
            public string Title { get; set; }
            public List<(string Label, string Column)> Fields { get; set; }
        }

        public frmServicesBilling()
        {
            InitializeComponent();

            //MccNetMatrix   = new DataTable();
            //MccServicing   = new DataTable();
            //MccTLE         = new DataTable();
            InvoiceDetails = new DataTable();
            RemovedRecords = new DataTable();

            setDoubleBuffer(dgv_BillingRecords);
            setDoubleBuffer(dgv_BillingRemove);
        }

        private void DisabledCtrl()
        {
            Cursor = Cursors.WaitCursor;
            cmb_BillingType.Enabled = false;
            btn_GenInvoice.Enabled = false;
            btn_GenExcel.Enabled = false;
            //dtp_From.Enabled = false;
            //dtp_To.Enabled = false;
        }

        private void EnabledCtrl()
        {
            Cursor = Cursors.Default;
            cmb_BillingType.Enabled = true;
            btn_GenInvoice.Enabled = true;
            btn_GenExcel.Enabled = true;
            //dtp_From.Enabled = true;
            //dtp_To.Enabled = true;
        }

        private void ClearInputs()
        {
            dgv_BillingRecords.DataSource = null;
            dgv_BillingRemove.DataSource = null;
            dgv_BillingRemove.Columns.Clear();
            
            dgv_BillingHistory.Columns.Clear();
            
            cmb_BillingType.SelectedIndex = 0;
            lb_Count.Text = "0";
            lb_Remove.Text = "0";
            RemovedRecords = new DataTable();

            clsSearch.ClassBillngFileName = "";
        }

        private void ClearDisplay()
        {
            dgv_BillingRecords.DataSource = null;
            dgv_BillingRemove.DataSource = null;
            dgv_BillingRemove.Columns.Clear();
            lb_Count.Text = "0";
            lb_Remove.Text = "0";
            RemovedRecords = new DataTable();
        }

        private void KeyBinds(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Close();
                    break;

                case Keys.Enter:
                    btn_Search.PerformClick();
                    break;
            }
        }

        private DataTable LoadServiceType(string value = "")
        {
            dbAPI = new clsAPI();

            dbAPI.ExecuteAPI("GET", "View", "Billing-Type", value, "Advance Detail", "", "ViewAdvanceDetail");

            if (dbAPI.isNoRecordFound())
                return new DataTable();

            return ParseResponseData(clsArray.ID, clsArray.detail_info);
        }

        private void BindComboBox(DataTable dataTable, ComboBox comboBox)
        {
            DebugTable(dataTable);

            DataTable comboBoxData = new DataTable();

            comboBoxData.Columns.Add("ID", typeof(int));
            comboBoxData.Columns.Add("Description", typeof(string));
            comboBoxData.Rows.Add(-1, "[NOT SPECIFIED]");

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    comboBoxData.Rows.Add(row["ID"], row["Description"]);
                }
            }

            comboBox.DisplayMember = "Description";
            comboBox.ValueMember = "ID";
            comboBox.DataSource = comboBoxData;
            comboBox.SelectedIndex = 0;
        }

        private async Task<DataTable> setBilling(string Value)
        {
            DisabledCtrl();
            DataTable Dt = await setBillingPreviewByMonth(dgv_BillingRecords, Value);
            EnabledCtrl();

            return Dt;
        }

        private void getBillingInvoice()
        {
            string Result = getGridToJson(dgv_BillingRemove);
            int id = Convert.ToInt32(cmb_BillingType.SelectedValue);

            getBillRef();

            if(!string.IsNullOrEmpty(Discount))
            {
                Result = $"{Result}|{Description}|{Discount}";
            }

            switch (id)
            {
                case 1: // LEASING
                    
                    getInvoiceDetails(id, Result);

                    break;

                case 2: // SERVICING

                    getInvoiceDetails(id, Result);

                    break;

                case 3: // TLE

                    getInvoiceDetails(id, Result);

                    break;

                case 4: // WAREHOUSE
                    getInvoiceDetails(id, Result);
                    break;

                default:
                    Prompt.Info("Billing Invoice", "No Selected Billing Found..");
                    break;
            }
        }

        private void getBillRef()
        {
            // Invoice Reference
            RefNo = txtRefNo.Text;
            RefTerms = txtTerms.Text;
            RefDate = dtpRefDate.Value.ToString("dd-MMM-yy");
            DueDate = dtpDueDate.Value.ToString("dd-MMM-yy");
            Discount = txtDiscount.Text;
            Description = txtDescription.Text;
        }

        private void getRecords()
        {
            switch (cmb_BillingType.SelectedIndex)
            {
                case 0:
                    setSearch(txt_SearchRecord.Text, dgv_BillingRecords, LeasingBilling);
                    break;

                case 1:
                    setSearch(txt_SearchRecord.Text, dgv_BillingRecords, ServicingBilling);
                    break;

                case 2:
                    setSearch(txt_SearchRecord.Text, dgv_BillingRecords, TleBilling);
                    break;

                default:
                    // Do Nothing
                    break;
            }
        }

        private void setExportData(DataTable Dt)
        {
            //RemovedValues = getGridFormatData(dgv_BillingRemove);

            btn_GenExcel.Enabled = false;
            setExportToExcelByDataTable(Dt);
            //setExportToExcelByDate(dtp_From, dtp_To); 
            btn_GenExcel.Enabled = true;
        }

        private void getExcel()
        {
            int id = Convert.ToInt32(cmb_BillingType.SelectedValue);

            switch (id)
            {
                case 1: // Net Matrix
                    setExportData(LeasingBilling);
                    break;

                case 2: // Servicing Fee
                    setExportData(ServicingBilling);
                    break;

                case 3: // TLE
                    setExportData(TleBilling);
                    break;

                case 4: // Warehouse
                    setExportData(WarehouseBilling);
                    break;

                default:
                    Prompt.Info("Export Data", "No Selected Billing Found ..");
                    break;
            }
        }

        private void SaveBilling()
        {

        }

        private void ReturnData(DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            int id = Convert.ToInt32(cmb_BillingType.SelectedValue);

            if (rowIndex < 0 || rowIndex >= dgv_BillingRemove.Rows.Count)
                return;

            DataTable currentTable = null;

            switch (id)
            {
                case 1: currentTable = LeasingBilling; break;
                case 2: currentTable = ServicingBilling; break;
                case 3: currentTable = TleBilling; break;
                case 4: currentTable = WarehouseBilling; break;
                default: return;
            }

            if (!billingConfig.ContainsKey(id))
                return;

            var row = dgv_BillingRemove.Rows[rowIndex];

            // 🔥 Dynamic RETURN confirmation
            string message = BuildDynamicMessage(row, billingConfig[id], "INCLUDE");

            if (!dbFunction.fPromptConfirmation(message))
                return;

            // 🔹 Proceed with transfer back
            currentTable = TransferRecord(RemovedRecords, currentTable, rowIndex);

            RefreshData(currentTable, RemovedRecords);

            getGridCount(dgv_BillingRemove, lb_Remove);
        }

        private void RemoveData(DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            int id = Convert.ToInt32(cmb_BillingType.SelectedValue);

            if (rowIndex < 0 || rowIndex >= dgv_BillingRecords.Rows.Count)
                return;

            DataTable currentTable = null;

            switch (id)
            {
                case 1: currentTable = LeasingBilling; break;
                case 2: currentTable = ServicingBilling; break;
                case 3: currentTable = TleBilling; break;
                case 4: currentTable = WarehouseBilling; break;
                default: return;
            }

            if (!billingConfig.ContainsKey(id))
                return;

            var row = dgv_BillingRecords.Rows[rowIndex];

            // Dynamic message
            string message = BuildDynamicMessage(row, billingConfig[id], "EXCLUDE");

            if (!dbFunction.fPromptConfirmation(message))
                return;

            RemovedRecords = TransferRecord(currentTable, RemovedRecords, rowIndex);

            RefreshData(currentTable, RemovedRecords);

            getGridCount(dgv_BillingRecords, lb_Count);
        }

        private async Task getBilling()
        {
            ClearDisplay();
            int id = Convert.ToInt32(cmb_BillingType.SelectedValue);

            // Switch based on ComboBox ID
            switch (id)
            {
                case 1:
                    LeasingBilling = await setBilling("Leasing-Summary");
                    break;

                case 2:
                    ServicingBilling = await setBilling("Service-Summary");
                    break;

                case 3:
                    TleBilling = await setBilling("TLE-Summary");
                    break;

                case 4:
                    WarehouseBilling = await setBilling("Warehouse-Summary");
                    break;

                default:
                    ClearInputs();
                    break;
            }
            
        }

        private void RefreshData(DataTable currentTable, DataTable removedTable)
        {
            //dgv_BillingRecords.DataSource = null;
            dgv_BillingRecords.DataSource = currentTable;
            dgv_BillingRecords.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            dgv_BillingRemove.DataSource = removedTable;
            dgv_BillingRemove.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        }

        private async void getBillingHistory()
        {
            //await setBillingHistory(dgv_BillingHistory, "Billing_History", "1");

            int id = Convert.ToInt32(cmb_BillingType.SelectedValue);

            // Switch based on ComboBox ID
            switch (id)
            {
                case 1: // LEASING
                    await setBillingHistory(dgv_BillingHistory, "Billing_History", "1");
                    break;

                case 2: // SERVICING
                    await setBillingHistory(dgv_BillingHistory, "Billing_History", "2");
                    break;

                case 3: // TLE
                    await setBillingHistory(dgv_BillingHistory, "Billing_History", "3");
                    break;

                case 4: // WAREHOUSE
                    await setBillingHistory(dgv_BillingHistory, "Billing_History", "4");
                    break;

                default:
                    //Prompt.Info("Billing Invoice", "No Selected Billing Found..");
                    ClearInputs();
                    break;
            }

            dgv_BillingHistory.ClearSelection();
        }


        private async void GetBillingInfo()
        {
            // Null Check
            if (dgv_BillingHistory.CurrentRow == null)
                return;

            var row = dgv_BillingHistory.CurrentRow;

            // Date Check
            var date_coverage = row.Cells[1].Value;

            if (date_coverage == null || date_coverage == DBNull.Value)
                return;

            var description = row.Cells[0].Value;
            var unitsObj = row.Cells[2].Value;
            var date_created = row.Cells[3].Value;

            int units = 0;
            string rawUnits = unitsObj?.ToString();

            // Proper parsing
            if (!int.TryParse(rawUnits, NumberStyles.Number, CultureInfo.InvariantCulture, out units))
            {
                units = 0;
            }

            // Validate
            if (units <= 0)
            {
                dbFunction.SetMessageBox(
                    "No records found for the selected billing.",
                    clsDefines.FIELD_CHECK_MSG,
                    clsFunction.IconType.iInformation
                );
                return;
            }

            // Confirmation message
            var message =
                "Export Billing Details\n\n" +
                $"Description   : {description}\n" +
                $"Coverage      : {date_coverage}\n" +
                $"Units         : {units}\n\n" +
                "Note: This may take a few minutes.\n" +
                "Do you want to continue?";

            if (!dbFunction.fPromptConfirmation(message))
                return;

            // Convert to Date
            DateTime dateValue = DateTime.ParseExact(
                date_coverage.ToString(),
                "MMMM yyyy",
                CultureInfo.InvariantCulture
            );
            
            string Result = $"{dateValue:yyyy-MM-dd}";
            int id = Convert.ToInt32(cmb_BillingType.SelectedValue);

            DataTable Dt;

            // Switch based on ComboBox ID
            switch (id)
            {
                case 1:
                    Dt = await GetBillingHistory("Leasing-Summary", Result);
                    break;

                case 2:
                    Dt = await GetBillingHistory("Service-Summary", Result);
                    break;

                case 3:
                    Dt = await GetBillingHistory("TLE-Summary", Result);
                    break;

                case 4:
                    Dt = await GetBillingHistory("Warehouse-Summary", Result);
                    break;

                default:
                    ClearInputs();
                    return;
            }

            clsSearch.ClassBillngFileName = $"{clsSearch.ClassBankCode}_{description}" +
                                            $"_{date_coverage}" +                                            
                                            $".xlsx";
            clsSearch.ClassBillngFileName = dbFunction.CleanFileName(clsSearch.ClassBillngFileName);

            setExportData(Dt);
        }

        // ------------------------------------ Objects ------------------------------------------------------

        private async void cmb_BillingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblBillingHeader.Text = $"Billing History - {cmb_BillingType.Text}";
            await getBilling();
            getBillingHistory();

        }

        private void frmServicesBilling_Load(object sender, EventArgs e)
        {
            dbFunction = new clsFunction();

            txtRefNo.Text = $"{DateTime.Now.Year.ToString()}-";
            BindComboBox(LoadServiceType(), cmb_BillingType);
        }

        private void dgv_BillingRecords_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            RemoveData(e);
        }

        private void dgv_BillingRemove_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ReturnData(e);
        }

        private void btn_GenExcel_Click(object sender, EventArgs e)
        {
            clsSearch.ClassBillngFileName =
                                            $"{clsSearch.ClassBankCode}_{cmb_BillingType.Text}" +
                                            $"_{dtpRefDate.Value:yyyy-MM-dd}" +                                            
                                            $".xlsx";
            clsSearch.ClassBillngFileName = dbFunction.CleanFileName(clsSearch.ClassBillngFileName);
            getExcel();
        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void dgv_BillingRecords_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            EnabledCtrl();
            getGridCount(dgv_BillingRecords, lb_Count);
        }

        private void btn_GenInvoice_Click(object sender, EventArgs e)
        {
            getBillingInvoice();
        }

        private void btn_Search_Click(object sender, EventArgs e)
        {
            getRecords();
        }

        private void dtp_To_ValueChanged(object sender, EventArgs e)
        {
            cmb_BillingType.Enabled = true;
        }

        private void dtp_From_ValueChanged(object sender, EventArgs e)
        {
            cmb_BillingType.Enabled = true;
        }

        private void dgv_BillingRemove_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            getGridCount(dgv_BillingRemove, lb_Remove);
        }

        private void cmb_Month_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmb_BillingType.Enabled = true;
        }

        private void frmServicesBilling_KeyDown(object sender, KeyEventArgs e)
        {
            KeyBinds(e);
        }

        private void dgv_BillingHistory_Leave(object sender, EventArgs e)
        {
            dgv_BillingHistory.ClearSelection();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveBilling();
        }

        private void txtDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                // Prevent the character from being entered
                e.Handled = true;
            }
        }

        private void dgv_BillingHistory_DoubleClick(object sender, EventArgs e)
        {   
            GetBillingInfo();
        }

        private void dgv_BillingRecords_DoubleClick(object sender, EventArgs e)
        {

        }

        Dictionary<int, BillingMessageConfig> billingConfig = new Dictionary<int, BillingMessageConfig>
        {
            {
                1, new BillingMessageConfig
                {
                    Title = "Leasing",
                    Fields = new List<(string, string)>
                    {
                        ("Bill ID", "BILL_ID"),
                        ("Coverage", "MONTH_PERIOD"),
                        ("Request No.", "REQUEST_NO"),
                        ("Request Date", "REQUEST_DATE")
                    }
                }
            },
            {
                2, new BillingMessageConfig
                {
                    Title = "Servicing",
                    Fields = new List<(string, string)>
                    {
                        ("Service No.", "SERVICE_NO"),
                        ("Coverage", "MONTH_PERIOD"),
                        ("Request ID", "REQUEST_ID"),
                        ("Request Date", "REQUEST_DATE")
                    }
                }
            },
            {
                3, new BillingMessageConfig
                {
                    Title = "TLE",
                    Fields = new List<(string, string)>
                    {
                        ("Service No.", "SERVICE_NO"),
                        ("Coverage", "MONTH_PERIOD"),
                        ("Request ID", "REQUEST_ID"),
                        ("Request Date", "REQUEST_DATE")
                    }
                }
            },
            {
                4, new BillingMessageConfig
                {
                    Title = "Warehouse",
                    Fields = new List<(string, string)>
                    {
                        ("Record ID", "ID"),
                        ("Coverage", "MONTH_PERIOD"),
                        ("Model", "MODEL"),
                        ("Serial No.", "SERIAL_NO")
                    }
                }
            }
        };

        private string BuildDynamicMessage(DataGridViewRow row, BillingMessageConfig config, string action)
        {
            var sb = new System.Text.StringBuilder();

            sb.AppendLine($"{action} {config.Title} Record\n");
            sb.AppendLine($"You are about to {action.ToLower()} the following:\n");

            foreach (var field in config.Fields)
            {
                var value = row.Cells[field.Column]?.Value?.ToString();
                sb.AppendLine($"{field.Label.PadRight(15)}: {value}");
            }

            sb.AppendLine($"\nThis will move the record {(action == "EXCLUDE" ? "to the excluded list" : "back to the included list")}.");
            sb.Append("Do you want to continue?");

            return sb.ToString();
        }        
    }
}