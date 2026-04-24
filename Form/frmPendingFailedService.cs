using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace MIS
{
    public partial class frmPendingFailedService : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        private clsINI dbINI;
        private clsFile dbFile;
        private clsInternet dbInternet;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED
                //cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        public frmPendingFailedService()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwList, true);
        }

        private void frmPendingResetService_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbINI = new clsINI();
            dbFile = new clsFile();

            dbINI.InitAPISetting();
            dbINI.InitFTPSetting();

            dbINI = new clsINI();
            dbFunction = new clsFunction();

            dbInternet = new clsInternet();

            dbINI.InitSystemSetting();
            dbINI.InitVersionSetting();

            dbFunction.ClearListViewItems(lvwList);
            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            
            btnSearchNow_Click(this, e);

            btnOpenJO.Enabled = false;
            
            Cursor.Current = Cursors.Default;
        }

        private void btnSearchNow_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            dbFunction.ClearListViewItems(lvwList);

            dbAPI.FillListViewFailedServiceList(lvwList);

            string pHeader = "SERVICE INFORMATION ";
            if (dbFunction.isValidCount(lvwList.Items.Count))
                lblCountHeader.Text = pHeader + dbFunction.AddParenthesisStartEnd(lvwList.Items.Count.ToString());
            else
                lblCountHeader.Text = pHeader;

            Cursor.Current = Cursors.Default;
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            string pLineNo = "";            
            string pServiceNo = "";
            string pIRIDNo = "";
            
            if (lvwList.Items.Count > 0)
            {
                string pSelectedRow = dbFunction.GetListViewSelectedRow(lvwList, 0);
                Debug.WriteLine("pSelectedRow=\n" + pSelectedRow);
              
                pLineNo = lvwList.SelectedItems[0].Text;                
                pServiceNo = dbFunction.GetSearchValue("ServiceNo");
                pIRIDNo = dbFunction.GetSearchValue("IRIDNo");
                
                rtbJSONFormat.Text = clsFunction.sNull;
                string pJSONString = dbAPI.getInfoDetailJSON("Search", "Merchant Servicing Info", pServiceNo + clsDefines.gPipe + pIRIDNo);
                rtbJSONFormat.Text = dbFunction.BeautifyJson(pJSONString);

                // selected info
                txtLineNo.Text = pLineNo;
                txtClientID.Text = dbAPI.GetValueFromJSONString(rtbJSONFormat.Text, clsDefines.TAG_ClientID);
                txtFEID.Text = dbAPI.GetValueFromJSONString(rtbJSONFormat.Text, clsDefines.TAG_FEID);
                txtIRNo.Text = dbAPI.GetValueFromJSONString(rtbJSONFormat.Text, clsDefines.TAG_IRNO);
                txtMerchantName.Text = dbAPI.GetValueFromJSONString(rtbJSONFormat.Text, clsDefines.TAG_MERCHANTNAME);
                txtTID.Text = dbAPI.GetValueFromJSONString(rtbJSONFormat.Text, clsDefines.TAG_TID);
                txtMID.Text = dbAPI.GetValueFromJSONString(rtbJSONFormat.Text, clsDefines.TAG_MID);
                txtServiceNo.Text = dbAPI.GetValueFromJSONString(rtbJSONFormat.Text, clsDefines.TAG_SERVICENO);
                txtFSRNo.Text = dbAPI.GetValueFromJSONString(rtbJSONFormat.Text, clsDefines.TAG_FSRNO);
                txtMerchantID.Text = dbAPI.GetValueFromJSONString(rtbJSONFormat.Text, clsDefines.TAG_MerchantID);
                txtIRIDNo.Text = dbAPI.GetValueFromJSONString(rtbJSONFormat.Text, clsDefines.TAG_IRIDNO);
                txtRequestID.Text = dbAPI.GetValueFromJSONString(rtbJSONFormat.Text, clsDefines.TAG_IRNO);
                txtJobTypeDescription.Text = dbAPI.GetValueFromJSONString(rtbJSONFormat.Text, clsDefines.TAG_ServiceJobTypeDescription);                
                txtMerchRep.Text = dbAPI.GetValueFromJSONString(rtbJSONFormat.Text, clsDefines.TAG_CustomerName);
                txtVendorRep.Text = dbAPI.GetValueFromJSONString(rtbJSONFormat.Text, clsDefines.TAG_FEName);
                txtDispatcher.Text = dbAPI.GetValueFromJSONString(rtbJSONFormat.Text, clsDefines.TAG_DispatchBy);

                txtCreatedDate.Text = dbAPI.GetValueFromJSONString(rtbJSONFormat.Text, clsDefines.TAG_CreatedDate);
                txtRequestDate.Text = dbAPI.GetValueFromJSONString(rtbJSONFormat.Text, clsDefines.TAG_RequestDate);
                txtScheduleDate.Text = dbAPI.GetValueFromJSONString(rtbJSONFormat.Text, clsDefines.TAG_ScheduleDate);
                txtDispatchDate.Text = dbAPI.GetValueFromJSONString(rtbJSONFormat.Text, clsDefines.TAG_DispatchDate);

                btnOpenJO.Enabled = true;

            }
        }

        private void frmPendingResetService_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void btnClearJSON_Click(object sender, EventArgs e)
        {
            rtbJSONFormat.Text = clsFunction.sNull;
            btnOpenJO.Enabled = false;
            dbFunction.ClearTextBox(this);
        }

        private void btnOpenJO_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantName, txtMerchantName.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iClientID, txtClientID.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantID, txtMerchantID.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iFEID, txtFEID.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iFEName, txtVendorRep.Text)) return;

            if (!dbFunction.fPromptConfirmation("Selected merchant information to Job Order Window" + "\n\n" +
                    " > Line# :" + dbFunction.GetSearchValue("Line#") + "\n" +
                    " > Request ID :" + txtIRNo.Text + "\n" +
                    " > Created Date :" + txtCreatedDate.Text + "\n" +
                    " > Request Date :" + txtRequestDate.Text + "\n" +
                    " > Schedule Date :" + txtScheduleDate.Text + "\n" +
                    " > Dispatch Date :" + txtDispatchDate.Text + "\n" +
                    " > Name :" + txtMerchantName.Text + "\n" +
                    " > TID :" + txtTID.Text + "\n" +
                    " > MID :" + txtMID.Text + "\n" +
                    " > Job Type :" + txtJobTypeDescription.Text + "\n" +
                    " > CustomerName :" + txtMerchRep.Text + "\n" +
                    " > Field Engineer :" + txtVendorRep.Text + "\n" +
                    " > Dispatcher :" + txtDispatcher.Text + "\n" +
                    "\n\n" +
                    "Are you sure you want to continue?")) return;

            try
            {
                btnOpenJO.Enabled = false;

                // set fields
                clsSearch.ClassServiceNo = int.Parse(txtServiceNo.Text);                
                clsSearch.ClassParticularName = txtMerchantName.Text;
                clsSearch.ClassClientID = int.Parse(txtClientID.Text);
                clsSearch.ClassMerchantID = int.Parse(txtMerchantID.Text);
                clsSearch.ClassFEID = int.Parse(txtFEID.Text);
                clsSearch.ClassTID = txtTID.Text;
                clsSearch.ClassMID = txtMID.Text;
                clsSearch.ClassIRIDNo = int.Parse(txtIRIDNo.Text);
                clsSearch.ClassIRNo = txtIRNo.Text;

                //dbFunction.SetMessageBox("Opening Job Order window with:" + "\n\n" +
                //    " > Service No. :" + dbFunction.AddBracketStartEnd(clsSearch.ClassServiceNo.ToString()) + "\n" +
                //    " > Request ID :" + dbFunction.AddBracketStartEnd(clsSearch.ClassIRNo) + "\n" +
                //    " > Merchant Name :" + dbFunction.AddBracketStartEnd(clsSearch.ClassParticularName) + "\n" +
                //    " > TID :" + dbFunction.AddBracketStartEnd(clsSearch.ClassTID) + "\n" +
                //    " > MID :" + dbFunction.AddBracketStartEnd(clsSearch.ClassMID), "Open window", clsFunction.IconType.iInformation);

                frmServiceJobOrder.fAutoLoadData = true;
                frmServiceJobOrder frm = new frmServiceJobOrder();
                frm.Text = "JOB ORDER";
                dbFunction.handleForm(frm);

            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox("Exception error " + ex.Message, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }
        }
        
    }
}
