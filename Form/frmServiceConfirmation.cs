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
    public partial class frmServiceConfirmation : Form
    {
        private clsFunction dbFunction;

        public static string gHeader;
        public static bool fSelected = false;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED
                //cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        public frmServiceConfirmation()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            fSelected = false;
            this.Close();
        }

        private void setServiceInfo()
        {
            txtServiceType.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassServiceTypeDesc);
            txtRequestID.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassRequestID);
            txtRequestDate.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassRequestDate);
            txtScheduleDate.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassScheduleDate);
            txtServicedDate.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassServicedDate);
            txtMerchant.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassMerchantName);
            txtTID.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassTID);
            txtMID.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassMID);
            txtVendorRep.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassFEName);
            txtVendorDispatcher.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassDispatcherName);
            txtRequestor.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassRequestor);
            txtDispatch.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassIsDispatch ? clsDefines.gYes : clsDefines.gNo);
            txtTerminalSN.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassTerminalSN);
            txtSIMSN.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassSIMSerialNo);
            txtRepTerminalSN.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassRepTerminalSN);
            txtRepSIMSN.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassRepSIMSN);
            txtComponents.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassComponents);
            txtRepComponents.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassRepComponents);
            txtBillable.Text = dbFunction.CheckAndSetStringValue(clsSearch.isBillable ? clsDefines.gYes : clsDefines.gNo);
            txtDependency.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassDependency);
            txtStatusReason.Text = dbFunction.CheckAndSetStringValue(clsSearch.ClassStatusReason);            
        }

        private void frmServiceConfirmation_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbFunction = new clsFunction();

            lblHeader.Text = gHeader;

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(true, this);

            setServiceInfo();

            Cursor.Current = Cursors.Default;
        }

        private void frmServiceConfirmation_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    fSelected = false;
                    this.Close();
                    break;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            fSelected = true;
            this.Close();
        }

        private void frmServiceConfirmation_Activated(object sender, EventArgs e)
        {
            btnOK.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            fSelected = false;
            this.Close();
        }
    }
}
