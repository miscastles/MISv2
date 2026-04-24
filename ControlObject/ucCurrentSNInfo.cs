using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MIS.Controller;

namespace MIS.ControlObject
{
    public partial class ucCurrentSNInfo : UserControl
    {
        private clsFunction dbFunction;

        // Controller
        private ServicingDetailController _mServicingDetailController;

        public ucCurrentSNInfo()
        {
            InitializeComponent();

            dbFunction = new clsFunction();

            // Initialize the controller object
            _mServicingDetailController = new ServicingDetailController();
        }

        public void loadData(ServicingDetailController model)
        {
            dbFunction.ClearTextBox(this);

            txtTerminalID.Text = model.TerminalID.ToString();
            txtSIMID.Text = model.SIMID.ToString();
            txtCurTerminalSN.Text = model.TerminalSN;
            txtCurTerminalType.Text = model.TerminalType;
            txtCurTerminalModel.Text = model.TerminalModel;
            txtCurSIMSN.Text = model.SIMSN;
            txtCurSIMCarrier.Text = model.SIMCarrier;
        }
    }
}
