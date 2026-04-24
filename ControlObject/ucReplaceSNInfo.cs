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
    public partial class ucReplaceSNInfo : UserControl
    {
        private clsFunction dbFunction;

        // Controller
        private ServicingDetailController _mServicingDetailController;

        public ucReplaceSNInfo()
        {
            InitializeComponent();

            dbFunction = new clsFunction();

            // Initialize the controller object
            _mServicingDetailController = new ServicingDetailController();
        }

        public void loadData(ServicingDetailController model)
        {
            dbFunction.ClearTextBox(this);

            txtTerminalID.Text = model.ReplaceTerminalID.ToString();
            txtSIMID.Text = model.ReplaceSIMID.ToString();
            txtRepTerminalSN.Text = model.ReplaceTerminalSN;
            txtRepTerminalType.Text = model.ReplaceTerminalType;
            txtRepTerminalModel.Text = model.ReplaceTerminalModel;
            txtRepSIMSN.Text = model.ReplaceSIMSN;
            txtRepSIMCarrier.Text = model.ReplaceSIMCarrier;
        }
    }
}
