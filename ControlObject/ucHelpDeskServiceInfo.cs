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
    public partial class ucHelpDeskServiceInfo : UserControl
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
        private string Dependency { get; set; }
        private string StatusReason { get; set; }

        public ucHelpDeskServiceInfo()
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

            txtAssistNo.Text = model.AssistNo.ToString();
            txtProblemNo.Text = model.ProblemNo.ToString();
            txtRequestNo.Text = model.Ref_No;
            txtEntryRequestID.Text = model.RequestID;
            txtCreatedDate.Text = model.CreatedDate;
            txtReqInstallationDate.Text = model.RequestDate;
            txtReasonDesc.Text = model.Reason;
            txtSource.Text = model.Source;
            txtCategory.Text = model.Category;
            txtSubCategory.Text = model.SubCategory;
            txtTimeAssisted.Text = model.TimeAssisted;
            txtTimeStart.Text = model.TimeStart;
            txtTimeEnd.Text = model.TimeEnd;
            txtDependency.Text = model.Dependency;
            txtStatusReason.Text = model.StatusReason;
        }
        
    }
}
