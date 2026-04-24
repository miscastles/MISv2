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
    public partial class ucServiceInfo : UserControl
    {
        private clsFunction dbFunction;

        // Controller
        private ServicingDetailController _mServicingDetailController;

        public ucServiceInfo()
        {
            InitializeComponent();

            dbFunction = new clsFunction();

            // Initialize the controller object
            _mServicingDetailController = new ServicingDetailController();
        }

        public void loadData(ServicingDetailController model)
        {
            dbFunction.ClearTextBox(this);

            txtServiceNo.Text = model.ServiceNo.ToString();
            txtRequestNo.Text = model.RequestNo;
            txtIRNo.Text = model.IRNo;
            txtReferenceNo.Text = model.ReferenceNo;
            txtServiceStatus.Text = model.JobTypeStatusDescription;
            txtReqInstallationDate.Text = model.RequestDate;
            txtServiceReqDate.Text = model.ServiceReqDate;
            txtCreatedDate.Text = model.ServiceCreatedDate;
            txtCreatedTime.Text = model.ServiceCreatedTime;
            txtServiceStatusDescription.Text = model.ServiceStatusDescription;
            txtDispatchDate.Text = model.DispatchDate;
            txtDispatcher.Text = model.Dispatcher;
        }
    }
}
