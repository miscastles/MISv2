using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIS
{
    public partial class frmPopUp : Form
    {
        public bool isProceed { get; private set; } = false;

        public frmPopUp()
        {
            InitializeComponent();
        }

        public frmPopUp(string pMessage) : this()
        {
            rtePopupInformation.Text = pMessage;
        }

        private void btnProceed_Click(object sender, EventArgs e)
        {
            isProceed = true;
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            isProceed = false;
            DialogResult = DialogResult.Cancel;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            isProceed = false;
            DialogResult = DialogResult.Cancel;
        }
    }
}