using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace MIS.Function
{
    // Date editor used by the Bank Template Generator request-date column.
    internal sealed class DataGridViewDatePickerColumn : DataGridViewTextBoxColumn
    {
        public DataGridViewDatePickerColumn()
        {
            CellTemplate = new DataGridViewDatePickerCell();
            DefaultCellStyle.Format = "MM/dd/yyyy";
            SortMode = DataGridViewColumnSortMode.NotSortable;
        }
    }

    internal sealed class DataGridViewDatePickerCell : DataGridViewTextBoxCell
    {
        public override Type EditType
        {
            get { return typeof(DataGridViewDatePickerEditingControl); }
        }

        public override Type ValueType
        {
            get { return typeof(string); }
        }

        public override object DefaultNewRowValue
        {
            get { return string.Empty; }
        }

        public override void InitializeEditingControl(
            int rowIndex,
            object initialFormattedValue,
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(
                rowIndex,
                initialFormattedValue,
                dataGridViewCellStyle);

            DataGridViewDatePickerEditingControl control =
                DataGridView.EditingControl as
                    DataGridViewDatePickerEditingControl;

            if (control == null)
                return;

            string text = Convert.ToString(Value);
            DateTime parsedDate;

            if (DateTime.TryParseExact(
                    text,
                    "MM/dd/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out parsedDate) ||
                DateTime.TryParse(text, out parsedDate))
            {
                control.Value = parsedDate;
                control.Checked = true;
            }
            else
            {
                control.Value = DateTime.Today;
                control.Checked = false;
            }
        }
    }

    internal sealed class DataGridViewDatePickerEditingControl :
        DateTimePicker,
        IDataGridViewEditingControl
    {
        private DataGridView _dataGridView;
        private bool _valueChanged;
        private int _rowIndex;

        public DataGridViewDatePickerEditingControl()
        {
            Format = DateTimePickerFormat.Custom;
            CustomFormat = "MM/dd/yyyy";
            ShowCheckBox = true;
        }

        public object EditingControlFormattedValue
        {
            get
            {
                return Checked
                    ? Value.ToString("MM/dd/yyyy")
                    : string.Empty;
            }
            set
            {
                string text = Convert.ToString(value);
                DateTime parsedDate;

                if (DateTime.TryParse(text, out parsedDate))
                {
                    Value = parsedDate;
                    Checked = true;
                }
                else
                {
                    Value = DateTime.Today;
                    Checked = false;
                }
            }
        }

        public object GetEditingControlFormattedValue(
            DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            Font = dataGridViewCellStyle.Font;
            CalendarForeColor = dataGridViewCellStyle.ForeColor;
            CalendarMonthBackground = dataGridViewCellStyle.BackColor;
        }

        public int EditingControlRowIndex
        {
            get { return _rowIndex; }
            set { _rowIndex = value; }
        }

        public bool EditingControlWantsInputKey(
            Keys key,
            bool dataGridViewWantsInputKey)
        {
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
        }

        public bool RepositionEditingControlOnValueChange
        {
            get { return false; }
        }

        public DataGridView EditingControlDataGridView
        {
            get { return _dataGridView; }
            set { _dataGridView = value; }
        }

        public bool EditingControlValueChanged
        {
            get { return _valueChanged; }
            set { _valueChanged = value; }
        }

        public Cursor EditingPanelCursor
        {
            get { return base.Cursor; }
        }

        protected override void OnValueChanged(EventArgs eventArgs)
        {
            base.OnValueChanged(eventArgs);
            NotifyValueChanged();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                Checked = false;
                NotifyValueChanged();
                e.Handled = true;
                return;
            }

            base.OnKeyDown(e);
        }

        private void NotifyValueChanged()
        {
            _valueChanged = true;

            if (_dataGridView != null)
            {
                _dataGridView.NotifyCurrentCellDirty(true);
            }
        }
    }
}