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
    public partial class ucMerchantRepInfo : UserControl
    {
        private clsFunction dbFunction;

        // Controller
        private ServicingDetailController _mServicingDetailController;

        public ucMerchantRepInfo()
        {
            InitializeComponent();

            dbFunction = new clsFunction();

            // Initialize the controller object
            _mServicingDetailController = new ServicingDetailController();
        }

        public void loadData(ServicingDetailController model)
        {
            dbFunction.ClearTextBox(this);

            txtMerchRepName.Text = model.MerchantRepresentative;
            txtMerchRepPosition.Text = model.MerchantRepresentativePosition;
            txtMerchRepEmail.Text = model.MerchantRepresentativeEmail;
        }
    }
}
