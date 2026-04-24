using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MIS.Function
{
    public static class DataGridViewHelper
    {
        public static void AddCheckBoxColumn(
            DataGridView grid,
            string columnName = "chkSelect",
            int columnIndex = 0)
        {
            if (grid.Columns.Contains(columnName))
                return;

            // Add checkbox column
            DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn
            {
                Name = columnName,
                HeaderText = "",
                Width = 50,
                ReadOnly = false
            };

            // Insert or Add
            if (columnIndex >= 0 && columnIndex <= grid.Columns.Count)
                grid.Columns.Insert(columnIndex, chk);
            else
                grid.Columns.Add(chk);

            // Center align checkbox
            chk.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            chk.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Ensure immediate update when clicking checkbox
            grid.CurrentCellDirtyStateChanged -= Grid_CurrentCellDirtyStateChanged;
            grid.CurrentCellDirtyStateChanged += Grid_CurrentCellDirtyStateChanged;
        }

        // 🔄 Commit checkbox change immediately
        private static void Grid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            DataGridView grid = sender as DataGridView;

            if (grid.IsCurrentCellDirty)
            {
                grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        // ✅ Get checked rows
        public static List<DataGridViewRow> GetCheckedRows(
            DataGridView grid,
            string columnName = "chkSelect")
        {
            List<DataGridViewRow> list = new List<DataGridViewRow>();

            if (!grid.Columns.Contains(columnName))
                return list;

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;

                bool isChecked = Convert.ToBoolean(row.Cells[columnName].Value ?? false);
                if (isChecked)
                    list.Add(row);
            }

            return list;
        }

        // ✅ Optional: Check all rows (for your button)
        public static void SetAllCheckState(
            DataGridView grid,
            bool isChecked,
            string columnName = "chkSelect")
        {
            if (!grid.Columns.Contains(columnName))
                return;

            grid.EndEdit();
            grid.SuspendLayout();

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;

                var cell = row.Cells[columnName] as DataGridViewCheckBoxCell;
                if (cell != null)
                {
                    cell.Value = isChecked;
                }
            }

            grid.ResumeLayout();
            grid.Refresh();
        }
    }
}