using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MIS.clsDefines;
using static MIS.Function.AppUtilities;
using MIS.Controller;

namespace MIS.ControlObject
{
    public partial class ucVendorHelpDeskRepInfo : UserControl
    {
        // Classes
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        private ParticularController _mParticularController = new ParticularController();

        private int MerchantID { get; set; }
        private int ClientID { get; set; }
        private int IRIDNo { get; set; }
        private int VendorHelpDeskID { get; set; }
        private int VendorTeamLeadID { get; set; }
        private int TerminalID { get; set; }
        private int SimID { get; set; }
        private int ReasonID { get; set; }
        private int ProblemReportedID { get; set; }
        private int AssistNo { get; set; }
        private int SourceID { get; set; }
        private int CategoryID { get; set; }
        private int SubCategoryID { get; set; }
        private int iFlag { get; set; }
        private int ProblemID { get; set; }
        private bool fEdit { get; set; } = false;
        private string CurrentStatus { get; set; }
        private string AppVersion { get; set; }
        private string AppCrc { get; set; }

        public ucVendorHelpDeskRepInfo()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbAPI = new clsAPI();
        }

        // Public TextBox value via a property
        public string VendorName
        {
            get { return txtVendorRepName.Text; }
            set { txtVendorRepName.Text = value; }
        }

        public string VendorEmail
        {
            get { return txtVendorRepEmail.Text; }
            set { txtVendorRepEmail.Text = value; }
        }

        public string VendorPosition
        {
            get { return txtVendorRepPosition.Text; }
            set { txtVendorRepPosition.Text = value; }
        }

        public string VendorMobileNo
        {
            get { return txtVendorRepContactNo.Text; }
            set { txtVendorRepContactNo.Text = value; }
        }

        public void loadData(HelpDeskController model)
        {
            dbFunction.ClearTextBox(this);

            Form frm = this.FindForm();
            dbFunction.TextBoxUnLock(true, frm);

            ParticularController data = _mParticularController.getInfo(model.HelpDeskID);

            if (data != null)
            {
                txtVendorRepName.Text = data.Name;
                txtVendorRepEmail.Text = data.Email;
                txtVendorRepPosition.Text = data.Position;
                txtVendorRepContactNo.Text = data.MobileNo;
            }
            
        }

    }
}
