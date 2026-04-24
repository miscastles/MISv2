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
    public partial class ucIRInfo : UserControl
    {
        private clsFunction dbFunction;

        // Controller
        private IRDetailController _mIRDetailController;

        public ucIRInfo()
        {
            InitializeComponent();

            dbFunction = new clsFunction();

            // Initialize the controller object
            _mIRDetailController = new IRDetailController();
        }

        public void loadData(IRDetailController model)
        {
            dbFunction.ClearTextBox(this);

            txtIRIDNo.Text = model.IRIDNo.ToString();
            txtClientName.Text = model.ClientName;
            txtIRNo.Text = model.IRNo;
            txtStatus.Text = model.IRStatusDescription;
            txtActive.Text = dbFunction.setIntegerToYesNoString(model.IRActive);
            txtRequestDate.Text = model.IRDate;
            txtInstallationDate.Text = model.InstallationDate;
            txtRequestType.Text = model.RequestType;
            txtRequestor.Text = model.Requestor;
            txtProcessedDate.Text = model.ProcessedDate;
            txtProcessedBy.Text = model.ProcessedBy;
            txtModifiedDate.Text = model.ModifiedDate;
            txtModifiedBy.Text = model.ModifiedBy;
            txtCreatedDate.Text = model.DateTimeStamp;

            // terminal
            txtTerminalSN.Text = model.TerminalSN;
            txtTerminalType.Text = model.TerminalType;
            txtTerminalModel.Text = model.TerminalModel;
            txtTerminalLocation.Text = model.TerminalLocation;
            txtTerminalStatus.Text = model.TerminalStatus;

            // sim
            txtSIMSN.Text = model.SIMSN;
            txtSIMCarrier.Text = model.SIMCarrier;
            txtSIMLocation.Text = model.SIMLocation;
            txtSIMStatus.Text = model.SIMStatus;

        }
    }
}
