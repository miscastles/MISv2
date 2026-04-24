using MIS.AppMainActivity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MIS.Function.AppUtilities;

namespace MIS.AppActivity
{
    class BillingActivity : AppReports
    {
        // Mc Invoice Reference
        public static string RefNo { get; set; }
        public static string RefTerms { get; set; }
        public static string DueDate { get; set; }
        public static string RefDate { get; set; }
        public static string Discount { get; set; }
        public static string Description { get; set; }



        public static string RemovedValues { get; set; }

        private static async Task<DataTable> getData(string pSearchBy, string pSearchVal)
        {
            var Apps = new AppReports()
            {
                StatementType = "View",
                SearchBy = pSearchBy,
                SearchValue = pSearchVal,
                Sql = ""
            };

            return await Apps.GetsDetails();
        }

        public static void getSelectedRecord(DataGridView SelectedRecord, DataGridView RemoveRecord)
        {
            // Check if any row is selected in dgv_BillingRecords
            if (SelectedRecord.SelectedRows.Count > 0)
            {
                // Clone columns from dgv_BillingRecords to dgv_BillingRemove if not already cloned
                if (RemoveRecord.Columns.Count == 0)
                {
                    foreach (DataGridViewColumn column in SelectedRecord.Columns)
                    {
                        RemoveRecord.Columns.Add((DataGridViewColumn)column.Clone());
                    }
                }

                // Get the selected row
                DataGridViewRow selectedRow = SelectedRecord.SelectedRows[0];

                // Check if the record already exists in dgv_BillingRemove
                bool recordExists = false;
                foreach (DataGridViewRow row in RemoveRecord.Rows)
                {
                    bool rowEqual = true;
                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        if (!row.Cells[i].Value.Equals(selectedRow.Cells[i].Value))
                        {
                            rowEqual = false;
                            break;
                        }
                    }
                    if (rowEqual)
                    {
                        recordExists = true;
                        break;
                    }
                }

                // If the record doesn't exist, add it to dgv_BillingRemove
                if (!recordExists)
                {
                    RemoveRecord.Rows.Add(selectedRow.Cells.Cast<DataGridViewCell>().Select(cell => cell.Value).ToArray());
                }
                else
                {
                    MessageBox.Show("The selected data is already existed.", "Duplicate Row", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a row to transfer.", "No Row Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        public static DataTable TransferRecord(DataTable fromTable, DataTable toTable, int selectedRowIndex)
        {
            // Check if a valid row index is provided
            if (selectedRowIndex >= 0 && selectedRowIndex < fromTable.Rows.Count)
            {
                // Clone columns from fromTable to toTable if not already cloned
                if (toTable.Columns.Count == 0)
                {
                    foreach (DataColumn column in fromTable.Columns)
                    {
                        toTable.Columns.Add(column.ColumnName, column.DataType);
                    }
                }

                // Get the selected row
                DataRow selectedRow = fromTable.Rows[selectedRowIndex];

                // Check if the record already exists in toTable
                bool recordExists = toTable.AsEnumerable().Any(row =>
                {
                    return fromTable.Columns.Cast<DataColumn>().All(col => row[col.ColumnName].Equals(selectedRow[col.ColumnName]));
                });

                // If the record doesn't exist, add it to toTable
                if (!recordExists)
                {
                    DataRow newRow = toTable.NewRow();
                    newRow.ItemArray = selectedRow.ItemArray;
                    toTable.Rows.Add(newRow);
                    fromTable.Rows.RemoveAt(selectedRowIndex);
                }
                else
                {
                    Console.WriteLine("The selected data already exists."); 
                }
            }
            else
            {
                Console.WriteLine("Please select a valid row index.");
            }

            return toTable;
        }

        public async static Task<DataTable> setBillingPreviewByDate(DataGridView Dgv, DateTimePicker From, DateTimePicker To)
        {
            DataTable Dt = await getData("Mcc Net Matrix Report", $"{From.Value.ToString("yyyy-MM-dd")}|{To.Value.ToString("yyyy-MM-dd")}");

            Dgv.DataSource = Dt;

            return Dt;
        }

        public async static Task<DataTable> setBillingPreviewByMonth(DataGridView Dgv, string Source)
        {
            DataTable Dt = await getData(Source, "");

            Dgv.DataSource = Dt;

            return Dt;
        }

        public async static Task<DataTable> setBillingHistory(DataGridView Dgv, string Source = null, string filter = null)
        {
            DataTable Dt = await getData(Source, filter);

            foreach (DataRow row in Dt.Rows)
            {
                Console.WriteLine(string.Join(", ", row.ItemArray));
            }

            Dgv.DataSource = Dt;

            return Dt;
        }

        public async static Task<DataTable> GetBillingHistory(string source, string filter = null)
        {
            return await getData(source, filter);
        }


        public static void setExportToExcelByDate(DateTimePicker From, DateTimePicker To)
        {
            ExportExcel.MccNetMatrixStr($"" +
                $"{From.Value.ToString("yyyy-MM-dd")}|" +
                $"{To.Value.ToString("yyyy-MM-dd")}|" +
                $"{RemovedValues}");
        }

        public static void setExportToExcelByDataTable(DataTable Dt)
        {
            ExportExcel.ExportData(Dt);
        }

        public static string getGridFormatData(DataGridView dgv)
        {
            if (dgv.Rows.Count > 0)
            { 
                var values = dgv.Rows
                    .Cast<DataGridViewRow>()
                    .Where(row => row.Cells[0].Value != null)
                    .Select(row => row.Cells[0].Value.ToString())
                    .ToList();

                return string.Join(",", values.Select(value => $"'{value}'"));

            } else {

                return string.Empty;
            }
        }

        public static string getGridToJson(DataGridView dataGridView)
        {
            List<string> columnValues = new List<string>();

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    columnValues.Add(row.Cells[0].Value.ToString());
                }
            }

            return JsonConvert.SerializeObject(columnValues);
        }

        public static void setSearch(string Record, DataGridView Dgv, DataTable Dt)
        {
            string SearchRecord = $"[TID] LIKE '%{Record}%' OR [Merchant_Name] LIKE '%{Record}%' OR [Pos_Serial] LIKE '%{Record}%'";

            DataView dv = new DataView(Dt);
            dv.RowFilter = SearchRecord;
            Dgv.DataSource = dv;

        }
        
        public static void getInvoiceDetails(int ReportID, string Record)
        {
            switch (ReportID)
            {
                case 1: // LEASING
                    DisplayInvoice(1002, Record);
                    break;

                case 2: // SERVICING
                    DisplayInvoice(1001, Record);
                    break;

                case 3: // TLE
                    DisplayInvoice(1003, Record);
                    break;

                case 4: // WAREHOUSE
                    DisplayInvoice(1005, Record);
                    break;

                default:
                    Prompt.Info("Billing Invoice", "The selected Billing is not found ..");
                    break;
            }

        }

        private static void DisplayInvoice(int vID, string Value)
        {
            clsSearch.ClassSearchValue = Value;
            ReportPreview(vID);
        }
        
        public static void getGridCount(DataGridView Dgv, Label Count)
        {
            Count.Text = "0";
            Count.Text = $"{Dgv.Rows.Count.ToString("N0")}";
        }

        public static void setDoubleBuffer(Control ctrl)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, ctrl, new object[] { true });
        }
    }
}
