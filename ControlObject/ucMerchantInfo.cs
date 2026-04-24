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
    public partial class ucMerchantInfo : UserControl
    {
        private clsFunction dbFunction;

        // Controller
        private ServicingDetailController _mServicingDetailController;
        private IRDetailController _mIRDetailController;

        public ucMerchantInfo()
        {
            InitializeComponent();

            dbFunction = new clsFunction();

            // Initialize the controller object
            _mServicingDetailController = new ServicingDetailController();
            _mIRDetailController = new IRDetailController();

            dbFunction.ClearTextBox(this);
        }

        public void loadData(ServicingDetailController model)
        {
            txtIRIDNo.Text = model.IRIDNo.ToString();
            txtMerchantID.Text = model.MerchantID.ToString();
            txtMerchantName.Text = model.MerchantName;
            txtMerchantAddress.Text = model.Address;
            txtMerchantCity.Text = model.Province;
            txtMerchantRegion.Text = model.Region;
            txtAppVersion.Text = model.AppVersion;
            txtAppCRC.Text = model.AppCRC;
            txtIRTID.Text = model.TID;
            txtIRMID.Text = model.MID;
        }

        public void loadMerchant(IRDetailController model)
        {
            txtIRIDNo.Text = model.IRIDNo.ToString();
            txtMerchantID.Text = model.MerchantID.ToString();
            txtMerchantName.Text = model.MerchantName;
            txtMerchantAddress.Text = model.Address;
            txtMerchantCity.Text = model.Province;
            txtMerchantRegion.Text = model.Region;
            txtAppVersion.Text = model.AppVersion;
            txtAppCRC.Text = model.AppCRC;
            txtIRTID.Text = model.TID;
            txtIRMID.Text = model.MID;
        }
    }
}
