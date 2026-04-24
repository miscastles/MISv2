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
    public partial class ucFSRInfo : UserControl
    {
        private clsFunction dbFunction;

        // Controller
        private ServicingDetailController _mServicingDetailController;

        public ucFSRInfo()
        {
            InitializeComponent();

            dbFunction = new clsFunction();

            // Initialize the controller object
            _mServicingDetailController = new ServicingDetailController();
        }

        public void loadData(ServicingDetailController model)
        {
            dbFunction.ClearTextBox(this);

            txtFSRNo.Text = model.FSRNo.ToString();
            txtMFSRDate.Text = model.FSRDate;
            txtMReceiptTime.Text = model.FSRTime;
            txtMTimeArrived.Text = model.TimeArrived;
            txtMTimeStart.Text = model.TimeStart;
            txtMTimeEnd.Text = model.TimeEnd;
            txtFSRDateTime.Text = $"{model.FSRDate} {model.FSRTime}";

            txtServiceResult.Text = model.ServiceResult;
            txtReason.Text = model.Reason;
            txtFSRMode.Text = dbFunction.getFSRMode(model.MobileID, model.FSRNo, model.ServiceNo);

            txtIRNo.Text = model.IRNo_fsr;
        }
    }
}
