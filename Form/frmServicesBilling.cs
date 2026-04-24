using MIS.AppConnection;
using MIS.AppMainActivity;
using System;
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
            cmb_BillingType.SelectedIndex = 0;
            lb_Count.Text = "0";
            lb_Remove.Text = "0";
            RemovedRecords = new DataTable();
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

            // Check if the row index is valid
            if (rowIndex >= 0 && rowIndex < dgv_BillingRemove.Rows.Count)
            {
                DataTable currentTable = null;

                // Switch based on ComboBox index
                switch (id)
                {
                    case 1:
                        currentTable = LeasingBilling;
                        break;
                    case 2:
                        currentTable = ServicingBilling;
                        break;
                    case 3:
                        currentTable = TleBilling;
                        break;
                    case 4:
                        currentTable = WarehouseBilling;
                        break;
                    default:
                        return; // Exit if no valid selection
                }

                // Transfer record back
                currentTable = TransferRecord(RemovedRecords, currentTable, rowIndex);

                RefreshData(currentTable, RemovedRecords);

                // Update count label
                getGridCount(dgv_BillingRemove, lb_Remove);


                /*
                int rowIndex = e.RowIndex;

                // Check if the row index is valid
                if (rowIndex >= 0 && rowIndex < dgv_BillingRemove.Rows.Count)
                {
                    MccNetMatrix = TransferRecord(RemovedRecords, MccNetMatrix, rowIndex);
                    dgv_BillingRemove.DataSource = null; // Ensure refresh
                    dgv_BillingRemove.DataSource = RemovedRecords;
                    dgv_BillingRecords.DataSource = null; // Ensure refresh
                    dgv_BillingRecords.DataSource = MccNetMatrix;

                    getGridCount(dgv_BillingRemove, lb_Remove);
                }
                */
            }
        }

        private void RemoveData(DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            int id = Convert.ToInt32(cmb_BillingType.SelectedValue);

            // Check if the row index is valid
            if (rowIndex >= 0 && rowIndex < dgv_BillingRecords.Rows.Count)
            {
                DataTable currentTable = null;

                // Switch based on ComboBox ID
                switch (id)
                {
                    case 1:
                        currentTable = LeasingBilling;
                        break;
                    case 2:
                        currentTable = ServicingBilling;
                        break;
                    case 3:
                        currentTable = TleBilling;
                        break;
                    case 4:
                        currentTable = WarehouseBilling;
                        break;
                    default:
                        return;
                }

                // Transfer record
                RemovedRecords = TransferRecord(currentTable, RemovedRecords, rowIndex);

                RefreshData(currentTable, RemovedRecords);

                // Update count label
                getGridCount(dgv_BillingRecords, lb_Count);

                /*
                int rowIndex = e.RowIndex;

                // Check if the row index is valid
                if (rowIndex >= 0 && rowIndex < dgv_BillingRecords.Rows.Count)
                {
                    RemovedRecords = TransferRecord(MccNetMatrix, RemovedRecords, rowIndex);
                    dgv_BillingRemove.DataSource = null; // Ensure refresh
                    dgv_BillingRemove.DataSource = RemovedRecords;
                    dgv_BillingRecords.DataSource = null; // Ensure refresh
                    dgv_BillingRecords.DataSource = MccNetMatrix;

                    getGridCount(dgv_BillingRecords, lb_Count);
                }
                */
            }
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

            // Date Check
            var date_coverage = dgv_BillingHistory.CurrentRow.Cells[1].Value;

            if (date_coverage == null || date_coverage == DBNull.Value)
                return;

            var description = dgv_BillingHistory.CurrentRow.Cells[0].Value;
            var units = dgv_BillingHistory.CurrentRow.Cells[2].Value;
            var date_created = dgv_BillingHistory.CurrentRow.Cells[3].Value;

            var message =
            $"Are you sure you want to export the details?\n\n" +
            $"Description   : {description}\n" +
            $"Coverage      : {date_coverage}\n" +
            $"Units         : {units}\n\n" +    
            $"Note: This may take a few minutes.";

            if (!dbFunction.fPromptConfirmation(message))
                return;

            // Convert to Date
            DateTime dateValue = DateTime.ParseExact(
                 date_coverage.ToString(),
                 "MMMM yyyy",
                 CultureInfo.InvariantCulture
             ).AddDays(0);

            string Result = $"{dateValue:yyyy-MM-dd}";
            int id = Convert.ToInt32(cmb_BillingType.SelectedValue);

            DataTable Dt;

            // Switch based on ComboBox ID
            switch (id)
            {
                case 1: // LEASING
                    Dt = await GetBillingHistory("Leasing-Summary", Result);
                    break;

                case 2: // SERVICING
                    Dt = await GetBillingHistory("Service-Summary", Result);
                    break;

                case 3: // TLE
                    Dt = await GetBillingHistory("TLE-Summary", Result);
                    break;

                case 4: // WAREHOUSE
                    Dt = await GetBillingHistory("Warehouse-Summary", Result);
                    break;

                default:
                    ClearInputs();
                    return;
            }

            setExportData(Dt);
            
        }

        // ------------------------------------ Objects ------------------------------------------------------

        private async void cmb_BillingType_SelectedIndexChanged(object sender, EventArgs e)
        {
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
    }
}