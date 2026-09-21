using System;
using System.Globalization;
using System.Windows.Forms;

namespace MIS
{
    public partial class frmGenericInput : Form
    {
        public DateTime DateValue { get; private set; }
        public decimal AmountValue { get; private set; }

        // Optional UI customization
        public string DialogTitle
        {
            get { return Text; }
            set { Text = value; }
        }

        public string DateLabel
        {
            get { return lblDate.Text; }
            set { lblDate.Text = value; }
        }

        public string AmountLabel
        {
            get { return lblAmount.Text; }
            set { lblAmount.Text = value; }
        }

        public frmGenericInput()
        {
            InitializeComponent();

            DateValue = DateTime.Today;
            AmountValue = 0m;
        }

        public frmGenericInput(DateTime dateValue, decimal amountValue)
            : this()
        {
            DateValue = dateValue;
            AmountValue = amountValue;
        }

        private void frmGenericInput_Load(object sender, EventArgs e)
        {
            dtpDate.Value = DateValue == DateTime.MinValue
                ? DateTime.Today
                : DateValue;

            txtAmount.Text = AmountValue.ToString("0.00");
            txtAmount.SelectAll();
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            decimal amount;

            string value = txtAmount.Text.Trim().Replace(",", "");

            if (!decimal.TryParse(
                    value,
                    NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture,
                    out amount))
            {
                MessageBox.Show(
                    "Please enter a valid amount.",
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtAmount.Focus();
                txtAmount.SelectAll();
                return;
            }

            if (amount < 0)
            {
                MessageBox.Show(
                    "Amount cannot be negative.",
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtAmount.Focus();
                txtAmount.SelectAll();
                return;
            }

            DateValue = dtpDate.Value.Date;
            AmountValue = amount;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow digits, decimal separator, backspace and control keys.
            if (char.IsControl(e.KeyChar) ||
                char.IsDigit(e.KeyChar) ||
                e.KeyChar == '.')
            {
                return;
            }

            e.Handled = true;
        }
    }
}
