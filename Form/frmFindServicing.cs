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
    public partial class frmFindServicing : Form
    {
        public static bool fMultipleSelected = false;
        public static bool fAllowSelect = false;
        public static bool fConfirm = false;
        public bool fSelected = false;
        public static string sHeader = "";
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        public static SearchType iSearchType;
        public static ServiceType iServiceType;        
        public static int iStatus;
        public static string sTerminalType;
        public static string sSearchChar;

        public static string sDefaultIRStatus;
        public static string sDefaultTerminalStatus;

        // The column we are currently using for sorting.
        private ColumnHeader SortingColumn = null;

        List<string> TAIDNoCol = new List<String>();
        List<string> IRIDNoCol = new List<String>();
        List<string> IRNoCol = new List<String>();
        List<string> MerchantNameCol = new List<String>();
        List<string> TerminalIDCol = new List<String>();
        List<string> TerminalSNCol = new List<String>();
        List<string> ServiceTypeStatusCol = new List<String>();
        List<string> ServiceTypeStatusDescriptionCol = new List<String>();
        List<string> JobTypeCol = new List<String>();
        List<string> JobTypeDescriptionCol = new List<String>();
        List<string> JobTypeSubDescriptionCol = new List<String>();
        List<string> JobTypeStatusDescriptionCol = new List<String>();

        public frmFindServicing()
        {
            InitializeComponent();
        }

        public enum SearchType
        {
            iIR, iJobType, iCallOut
        }
        public enum ServiceType
        {
            iFSR, iIR, iDispatch, iAllocation, iServiceCall, iServicing
        }
        
        private void frmFindDispatch_Load(object sender, EventArgs e)
        {
            fSelected = false;
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            lblHeader.Text = lblHeader.Text + " " + "[ " + sHeader + " ]";
            lblSearchStatus.Text = "";            
            InitMouseRightClick();
            LoadTA("View", "", "");

            if (fMultipleSelected)
                lvwSearch.MultiSelect = true;
            else
                lvwSearch.MultiSelect = false;
        }

        private void lvwSearch_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Get the new sorting column.
            ColumnHeader new_sorting_column = lvwSearch.Columns[e.Column];

            // Figure out the new sorting order.
            System.Windows.Forms.SortOrder sort_order;

            if (SortingColumn == null)
            {
                // New column. Sort ascending.
                sort_order = SortOrder.Ascending;
            }
            else
            {
                // See if this is the same column.
                if (new_sorting_column == SortingColumn)
                {
                    // Same column. Switch the sort order.
                    if (SortingColumn.Text.StartsWith("> "))
                    {
                        sort_order = SortOrder.Descending;
                    }
                    else
                    {
                        sort_order = SortOrder.Ascending;
                    }
                }
                else
                {
                    // New column. Sort ascending.
                    sort_order = SortOrder.Ascending;
                }

                // Remove the old sort indicator.
                SortingColumn.Text = SortingColumn.Text.Substring(2);
            }

            // Display the new sort order.
            SortingColumn = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                SortingColumn.Text = "> " + SortingColumn.Text;
            }
            else
            {
                SortingColumn.Text = "< " + SortingColumn.Text;
            }

            // Create a comparer.
            lvwSearch.ListViewItemSorter = new clsListView(e.Column, sort_order);

            // Sort.
            lvwSearch.Sort();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmFindDispatch_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    fSelected = false;
                    this.Close();
                    break;
            }
        }

        private void LoadTA(string StatementType, string SearchBy, string SearchValue)
        {
            int i = 0;
            int ii = 0;
            int iLineNo = 0;

            lblSearchStatus.Text = "";
            lvwSearch.Items.Clear();

            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassIRStatus + clsFunction.sPipe +
                                                clsSearch.ClassIRStatusDescription;

            Debug.WriteLine("LoadTA::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbFunction.GetRequestTime("Find Servicing");

            dbAPI.ExecuteAPI("GET", "View", "Advance Servicing", clsSearch.ClassAdvanceSearchValue, "TA", "", "ViewAdvanceTA");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.TAID.Length > i)
                {
                    ii++;
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                    item.SubItems.Add(clsArray.TAIDNo[i].ToString());
                    item.SubItems.Add(clsArray.ClientID[i].ToString());
                    item.SubItems.Add(clsArray.ServiceProviderID[i].ToString());
                    item.SubItems.Add(clsArray.IRIDNo[i].ToString());
                    item.SubItems.Add(clsArray.MerchantID[i].ToString());
                    item.SubItems.Add(clsArray.FEID[i].ToString());
                    item.SubItems.Add(clsArray.TerminalID[i].ToString());
                    item.SubItems.Add(clsArray.TerminalTypeID[i].ToString());
                    item.SubItems.Add(clsArray.TerminalModelID[i].ToString());
                    item.SubItems.Add(clsArray.TerminalBrandID[i].ToString());
                    item.SubItems.Add(clsArray.SIMID[i]);
                    item.SubItems.Add(clsArray.IRNo[i]);                    
                    item.SubItems.Add(clsArray.TerminalSN[i]);
                    item.SubItems.Add(clsArray.SIMSerialNo[i]);
                    item.SubItems.Add(clsArray.SIMCarrier[i]);
                    item.SubItems.Add(clsArray.MerchantName[i]);
                    item.SubItems.Add(clsArray.TID[i]);
                    item.SubItems.Add(clsArray.MID[i]);
                    item.SubItems.Add(clsArray.ClientName[i]);
                    item.SubItems.Add(clsArray.FEName[i]);
                    item.SubItems.Add(clsArray.TypeDescription[i]);
                    item.SubItems.Add(clsArray.ModelDescription[i]);
                    item.SubItems.Add(clsArray.BrandDescription[i]);                    
                    item.SubItems.Add(clsArray.DockID[i]);
                    item.SubItems.Add(clsArray.DockSN[i]);
                    item.SubItems.Add(clsArray.ServiceTypeStatus[i]);
                    item.SubItems.Add(clsArray.ServiceTypeStatusDescription[i]);
                    item.SubItems.Add(clsArray.JobType[i]);
                    item.SubItems.Add(clsArray.JobTypeDescription[i]);
                    item.SubItems.Add(clsArray.JobTypeSubDescription[i]);
                    item.SubItems.Add(clsArray.JobTypeStatusDescription[i]);
                    item.SubItems.Add(clsArray.ServiceNo[i]);

                    i++;                    

                    lvwSearch.Items.Add(item);

                    dbFunction.AppDoEvents(true);
                }

                dbFunction.ListViewAlternateBackColor(lvwSearch);
                lblSearchStatus.Text = lvwSearch.Items.Count.ToString() + " " + "record(s) found.";                
            }
            else
            {
                //dbFunction.SetMessageBox("No record found.", "Find TA", clsFunction.IconType.iExclamation);
                lblSearchStatus.Text = "No record found...";
            }


            dbFunction.GetResponseTime("Find Servicing");
            
            /*
            // Focus first item
            if (lvwSearch.Items.Count > 0)
            {
                lvwSearch.FocusedItem = lvwSearch.Items[0];
                lvwSearch.Items[0].Selected = true;
                lvwSearch.Select();
            } 
            */
        }
        private void InitMouseRightClick()
        {
            switch (iServiceType)
            {
                case ServiceType.iDispatch:
                    cmsLvwSearch.Visible = true;
                    break;
                case ServiceType.iFSR:
                    cmsLvwSearch.Visible = false;
                    break;
                default:
                    cmsLvwSearch.Visible = false;
                    break;
            }
        }

        private void frmFindServicing_Activated(object sender, EventArgs e)
        {
            
        }
        private void tsmModify_Click(object sender, EventArgs e)
        {

        }

        private void tsmDispatch_Click(object sender, EventArgs e)
        {
            ProcessDispatch();
        }
        
        private void tsmPreview_Click(object sender, EventArgs e)
        {
            if (clsSearch.ClassIRNo.Length > 0 && clsSearch.ClassIRNo.CompareTo("0") != 0 && lvwSearch.Items.Count > 0)
            {
                fMultipleSelected = false;
                cmsLvwSearch.Visible = false;

                if (!fSelectedConfirm()) return;

                dbFunction.PreviewTA(clsSearch.ClassIRNo, clsSearch.ClassServiceNo, clsSearch.ClassFSRNo);
            }
            else
            {
                dbFunction.SetMessageBox("Select record from the list.", "TA(FSR) Preview", clsFunction.IconType.iError);
            }
        }

        private void ProcessDispatch()
        {
            bool fMultiple = false;
            int i = 0;

            TAIDNoCol.Clear();
            IRIDNoCol.Clear();
            IRNoCol.Clear();
            TerminalIDCol.Clear();
            TerminalSNCol.Clear();
            MerchantNameCol.Clear();
            ServiceTypeStatusCol.Clear();
            ServiceTypeStatusDescriptionCol.Clear();
            JobTypeCol.Clear();
            JobTypeDescriptionCol.Clear();
            JobTypeSubDescriptionCol.Clear();
            JobTypeSubDescriptionCol.Clear();

            if (!fMultipleSelected)
                fMultiple = false;

            foreach (ListViewItem item in lvwSearch.Items)
            {
                if (item.Selected)
                {
                    i++;
                }

                if (i > 1)
                {
                    fMultiple = true;
                    break;
                }
            }

            if (fMultiple)
            {
                fSelected = true;
                foreach (ListViewItem item in lvwSearch.Items)
                {
                    if (item.Selected)
                    {
                        TAIDNoCol.Add(item.SubItems[1].Text);
                        IRIDNoCol.Add(item.SubItems[4].Text);
                        TerminalIDCol.Add(item.SubItems[7].Text);
                        IRNoCol.Add(item.SubItems[12].Text);
                        TerminalSNCol.Add(item.SubItems[13].Text);
                        MerchantNameCol.Add(item.SubItems[16].Text);
                        ServiceTypeStatusCol.Add(item.SubItems[26].Text);
                        ServiceTypeStatusDescriptionCol.Add(item.SubItems[27].Text);
                        JobTypeCol.Add(item.SubItems[28].Text);
                        JobTypeDescriptionCol.Add(item.SubItems[29].Text);
                        JobTypeSubDescriptionCol.Add(item.SubItems[30].Text);
                        JobTypeStatusDescriptionCol.Add(item.SubItems[31].Text);
                    }
                }

            }
            else
            {
                fSelected = true;
                TAIDNoCol.Add(lvwSearch.SelectedItems[0].SubItems[1].Text);
                IRIDNoCol.Add(lvwSearch.SelectedItems[0].SubItems[4].Text);
                TerminalIDCol.Add(lvwSearch.SelectedItems[0].SubItems[7].Text);
                IRNoCol.Add(lvwSearch.SelectedItems[0].SubItems[12].Text);
                TerminalSNCol.Add(lvwSearch.SelectedItems[0].SubItems[13].Text);
                MerchantNameCol.Add(lvwSearch.SelectedItems[0].SubItems[16].Text);
                ServiceTypeStatusCol.Add(lvwSearch.SelectedItems[0].SubItems[26].Text);
                ServiceTypeStatusDescriptionCol.Add(lvwSearch.SelectedItems[0].SubItems[27].Text);
                JobTypeCol.Add(lvwSearch.SelectedItems[0].SubItems[28].Text);
                JobTypeDescriptionCol.Add(lvwSearch.SelectedItems[0].SubItems[29].Text);
                JobTypeSubDescriptionCol.Add(lvwSearch.SelectedItems[0].SubItems[30].Text);
                JobTypeStatusDescriptionCol.Add(lvwSearch.SelectedItems[0].SubItems[31].Text);
            }

            clsArray.TAIDNo = TAIDNoCol.ToArray();
            clsArray.IRIDNo = IRIDNoCol.ToArray();
            clsArray.TerminalID = TerminalIDCol.ToArray();
            clsArray.IRNo = IRNoCol.ToArray();
            clsArray.TerminalSN = TerminalSNCol.ToArray();
            clsArray.MerchantName = MerchantNameCol.ToArray();
            clsArray.ServiceTypeStatus = ServiceTypeStatusCol.ToArray();
            clsArray.ServiceTypeStatusDescription = ServiceTypeStatusDescriptionCol.ToArray();
            clsArray.JobType = JobTypeCol.ToArray();
            clsArray.JobTypeDescription = JobTypeDescriptionCol.ToArray();
            clsArray.JobTypeSubDescription = JobTypeSubDescriptionCol.ToArray();
            clsArray.JobTypeStatusDescription = JobTypeStatusDescriptionCol.ToArray();

            ProcessTerminalDispatch();

            // Reload Allocated            
            fMultipleSelected = true;
            fAllowSelect = false;
            fConfirm = false;
            lvwSearch.Items.Clear();            
            dbAPI.ResetAdvanceSearch();
            clsSearch.ClassIRStatus = clsGlobalVariables.STATUS_ALLOCATED;
            clsSearch.ClassIRStatusDescription = clsGlobalVariables.STATUS_ALLOCATED_DESC;
            Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
            LoadTA("View", "", "");
            Cursor.Current = Cursors.Default; // Back to normal  
        }

        private void ProcessTerminalDispatch()
        {
            int i = 0;
            int iCount = 0;
            string sItem = "";

            if (clsArray.TAIDNo.Length > 0)
            {
                // Display to be dispatch
                while (clsArray.TAIDNo.Length > i)
                {
                    string sTemp = clsArray.IRNo[i].ToString() + clsFunction.sComma + clsFunction.sPadSpace +                                   
                                   clsArray.JobTypeDescription[i].ToString() +
                                   "\n";
                    sItem = sItem + sTemp;

                    string sStatusDescription = clsArray.ServiceTypeStatusDescription[i].ToString();
                    if (sStatusDescription.CompareTo(clsGlobalVariables.STATUS_ALLOCATED_DESC) == 0)
                        iCount++;

                    i++;

                }

                if (MessageBox.Show("Are you sure to process selected on list." +
                               "\n\n" +
                               sItem +
                               "\n" +
                               "Terminal count to be dispatch: " + iCount.ToString() +
                               "\n\n" +
                               "Note: ALLOCATED terminal will be processed." +
                               "\n",
                               "Confirm?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    return;
                }

                i = 0;
                while (clsArray.TAIDNo.Length > i)
                {
                    string sStatusDescription = clsArray.ServiceTypeStatusDescription[i].ToString();
                    if (sStatusDescription.CompareTo(clsGlobalVariables.STATUS_ALLOCATED_DESC) == 0)
                    {
                        clsSearch.ClassStatus = clsGlobalVariables.STATUS_DISPATCH;
                        clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_DISPATCH_DESC;

                        // Update Status (tblterminaldetail)
                        string sTerminalID = clsArray.TerminalID[i].ToString();
                        int iTerminalID = int.Parse((sTerminalID.Length > 0 ? sTerminalID : "0"));
                        if (iTerminalID > 0)
                        {
                            clsSearch.ClassAdvanceSearchValue = sTerminalID + clsFunction.sPipe +
                                                                clsSearch.ClassStatus.ToString() + clsFunction.sPipe +
                                                                clsSearch.ClassStatusDescription;

                            Debug.WriteLine("Terminal::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                            dbAPI.ExecuteAPI("PUT", "Update", "Update Terminal Detail Status", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
                        }

                        // Update Status (tblirdetail)
                        string sIRIDNo = clsArray.IRIDNo[i].ToString();
                        if (sIRIDNo.Length > 0)
                        {
                            clsSearch.ClassAdvanceSearchValue = sIRIDNo + clsFunction.sPipe +
                                                                clsSearch.ClassStatus.ToString() + clsFunction.sPipe +
                                                                clsSearch.ClassStatusDescription;

                            Debug.WriteLine("IR::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                            dbAPI.ExecuteAPI("PUT", "Update", "Update IR Detail Status", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
                        }

                        // Update Status (tblterminalallocation)
                        string sTAIDNo = clsArray.TAIDNo[i].ToString();
                        int iTAIDNo = int.Parse((sTAIDNo.Length > 0 ? sTAIDNo : "0"));
                        if (iTAIDNo > 0)
                        {
                            
                            clsSearch.ClassAdvanceSearchValue = sTAIDNo + clsFunction.sPipe +                                    
                                                                clsSearch.ClassStatus.ToString() + clsFunction.sPipe +
                                                                clsSearch.ClassStatusDescription;

                            Debug.WriteLine("TA::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                            dbAPI.ExecuteAPI("PUT", "Update", "Update TA Detail Status", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                            // JobType Field
                            clsSearch.ClassAdvanceSearchValue = sTAIDNo + clsFunction.sPipe +
                                                                sIRIDNo + clsFunction.sPipe + 
                                                                clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING + clsFunction.sPipe +                                                                
                                                                clsGlobalVariables.TA_STATUS_DISPATCH_SUB_DESC + clsFunction.sPipe +
                                                                clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING_DESC;

                            Debug.WriteLine("JobType::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                            dbAPI.ExecuteAPI("PUT", "Update", "Job Type TA Detail", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
                        }

                        // Upddate Status (tblservicingdetail)
                        int iIRIDNo = int.Parse((sIRIDNo.Length > 0 ? sIRIDNo : "0"));
                        if (iTAIDNo > 0 && iIRIDNo > 0)
                        {
                            DateTime ProcessDateTime = DateTime.Now;
                            string sProcessDate = "";
                            string sProcessTime = "";
                            string sProcessedBy = clsUser.ClassUserFullName;

                            sProcessDate = ProcessDateTime.ToString("MM-dd-yyyy");
                            sProcessTime = ProcessDateTime.ToString("H:mm:ss");

                            clsSearch.ClassAdvanceSearchValue = sTAIDNo + clsFunction.sPipe +
                                                                sIRIDNo + clsFunction.sPipe +
                                                                sProcessDate + clsFunction.sPipe +
                                                                sProcessTime + clsFunction.sPipe +
                                                                sProcessedBy + clsFunction.sPipe +
                                                                clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING + clsFunction.sPipe +
                                                                clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING_DESC + clsFunction.sPipe +
                                                                clsGlobalVariables.TA_STATUS_DISPATCH + clsFunction.sPipe +
                                                                clsGlobalVariables.TA_STATUS_DISPATCH_DESC;                                                         
                                     
                            Debug.WriteLine("UpdateServicingDispatchToProcess::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                            dbAPI.ExecuteAPI("PUT", "Update", "Update Servicing Dispatch To Process", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                        }
                    }

                    i++;
                }

                if (iCount > 0)
                {
                    dbFunction.SetMessageBox("Terminal dispatch successfully.", "Dispatch Terminal", clsFunction.IconType.iInformation);                    
                }
                else
                    dbFunction.SetMessageBox("No terminal to be dispatch.", "Dispatch Terminal", clsFunction.IconType.iExclamation);
            }
        }
        private bool fSelectedConfirm()
        {
            bool fAddItem = true;

            if (fMultipleSelected)
            {
                if (MessageBox.Show("Are you sure to process multiple selected." +
                               "\n\n",
                               "Confirm?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    fAddItem = false;
                }
            }
            else
            {
                if (MessageBox.Show("Please confirm selected details below:" +
                                "\n\n" +
                                "[SERVICE DETAILS]" + "\n" +
                                "Service Type.: " + clsSearch.ClassServiceTypeDescription + "\n" +
                                "Other Service Type.: " + clsSearch.ClassOtherServiceTypeDescription + "\n" +
                                "Service Status.: " + clsSearch.ClassIRStatusDescription + "\n" +
                                "\n" +
                                "[INSTALLATION REQUEST DETAILS]" + "\n" +
                                "IR ID.: " + clsSearch.ClassIRNo + "\n" +
                                "Request Date: " + clsSearch.ClassIRDate + "\n" +
                                "Installation Date: " + clsSearch.ClassInstallationDate + "\n" +
                                "\n" +
                                "[TERMINAL DETAILS]" + "\n" +
                                "Serial No.: " + clsSearch.ClassTerminalSN + "\n" +
                                "Type/Model/Brand.: " + clsSearch.ClassTypeDescription + "/" + clsSearch.ClassModelDescription + "/" + clsSearch.ClassBrandDescription + "\n" +
                                "\n" +
                                "[TERMINAL ALLOCATION DETEILS]" + "\n" +
                                "Allocation Date Created: " + clsSearch.ClassTADateTime + "\n" +
                                "Processed By: " + clsSearch.ClassTAProcessedBy + " @ " + clsSearch.ClassTAProcessedDateTime + "\n" +
                                "Modified By: " + clsSearch.ClassTAModifiedBy + " @ " + clsSearch.ClassTAModifiedDateTime + "\n" +
                                "\n" +
                                "[ASSIGNMENT DETAILS]" + "\n" +
                                "Client: " + clsSearch.ClassClientName + "\n" +
                                "Merchant: " + clsSearch.ClassMerchantName + "\n" +
                                "Field Engineer: " + clsSearch.ClassFEName + "\n" +
                                "\n\n",
                                "Confirm?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    fAddItem = false;
                }
            }

            return fAddItem;

        }

        private void lvwSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwSearch.SelectedItems.Count > 0)
            {
                string LineNo = lvwSearch.SelectedItems[0].Text;
                txtLineNo.Text = LineNo;

                lblIRNo.Text = "";
                clsSearch.ClassIRIDNo = 0;
                clsSearch.ClassServiceNo = 0;
                if (LineNo.Length > 0)
                {
                    lblIRNo.Text = lvwSearch.SelectedItems[0].SubItems[12].Text;
                    clsSearch.ClassIRNo = lvwSearch.SelectedItems[0].SubItems[12].Text;
                    clsSearch.ClassServiceNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[32].Text);
                }
            }
        }
    }
}
