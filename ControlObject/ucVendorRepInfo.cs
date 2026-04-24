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
    public partial class ucVendorRepInfo : UserControl
    {
        private clsFunction dbFunction;

        // Controller
        private ServicingDetailController _mServicingDetailController;

        public ucVendorRepInfo()
        {
            InitializeComponent();

            dbFunction = new clsFunction();

            // Initialize the controller object
            _mServicingDetailController = new ServicingDetailController();
        }

        public void loadData(ServicingDetailController model)
        {
            dbFunction.ClearTextBox(this);

            txtVendorRepName.Text = model.FEName;
            txtVendorRepPosition.Text = model.Position;
            txtVendorRepEmail.Text = model.Email;
        }
    }
}
