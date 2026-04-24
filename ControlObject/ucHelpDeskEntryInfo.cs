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
    public partial class ucHelpDeskEntryInfo : UserControl
    {
        // Classes
        private clsAPI dbAPI;
        private clsFunction dbFunction;

        private int MerchantID { get; set; }
        private int ClientID { get; set; }
        private int IRIDNo { get; set; }
        public int VendorHelpDeskID { get; set; }
        public int VendorTeamLeadID { get; set; }
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

        public ucHelpDeskEntryInfo()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbAPI = new clsAPI();

        }
        
        public void loadData(HelpDeskController model)
        {
            dbFunction.ClearTextBox(this);

            Form frm = this.FindForm();
            dbFunction.TextBoxUnLock(true, frm);
            
            txtProbReported.Text = model.ProblemReported;
            txtActualProblem.Text = model.ActualProblem;
            txtActionTaken.Text = model.ActionTaken;
            txtRemarks.Text = model.Remarks;

            txtCustomerName.Text = model.ContactPerson;
            txtRepresentative.Text = model.Representative;
            txtRequestBy.Text = model.RequestedBy;
            txtCustomerEmail.Text = model.ContactEmail;
            txtCustomerPosition.Text = model.ContactPosition;
            txtCustomerContactNo.Text = model.ContactNo;

            txtFUAppVersion.Text = model.AppVersion;
            txtFUAppCRC.Text = model.AppCRC;
        }
        
    }
}
