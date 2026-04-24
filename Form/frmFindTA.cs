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
using System.Threading;

namespace MIS
{
    public partial class frmFindTA : Form
    {
        public static bool fAutoLoadData = false;
        public static bool fMultipleSelected = false;
        public static bool fAllowSelect = false;
        public static bool fConfirm = false;
        public bool fSelected = false;
        public static string sHeader;
        public static bool isReport = false;

        private clsAPI dbAPI;
        private clsINI dbSetting;
        private clsFunction dbFunction;
        private clsExport dbExport;
        private clsFile dbFile;

        private static int iPanelSearchHeightMin = 35;
        private static int iPanelSearchHeightMax = 300;
        private static int iPanelListHeightMax = 519;

        public static string sDefaultIRStatus;
        public static string sDefaultTerminalStatus;
        public static int isPrimary = clsFunction.iZero;

        public static SearchType iSearchType;

        // The column we are currently using for sorting.
        private ColumnHeader SortingColumn = null;

        List<string> TAIDNoCol = new List<String>();
        List<string> IRIDNoCol = new List<String>();
        List<string> IRNoCol = new List<String>();
        List<string> MerchantNameCol = new List<String>();
        List<string> TerminalIDCol = new List<String>();
        List<string> TerminalSNCol = new List<String>();
        List<string> IRStatusCol = new List<String>();
        List<string> IRStatusDescription = new List<String>();

        public frmFindTA()
        {
            InitializeComponent();
        }
        public enum SearchType
        {
            iFSR, iIR, iDispatch,  iAllocation, iServiceCall, iServicing
        }

        private void LoadTA(string StatementType, string SearchBy, string SearchValue)
        {
            int i = 0;
            int ii = 0;
            int iLineNo = 0;

            lblSearchStatus.Text = "";
            lvwSearch.Items.Clear();

            clsSearch.ClassCurrentPage = clsSearch.ClassCurrentPage;
            clsSearch.ClassTotalPage = dbFunction.GetPageLimit();

            ComposeSearchValue(); // Set clsSearch.ClassSearchValue

            dbFunction.GetRequestTime("Find TA");

            dbAPI.ExecuteAPI("GET", "View", "Advance TA", clsSearch.ClassSearchValue, "TA", "", "ViewAdvanceTA");

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
                    item.SubItems.Add(clsArray.ServiceTypeID[i].ToString());
                    item.SubItems.Add(clsArray.OtherServiceTypeID[i].ToString());
                    item.SubItems.Add(clsArray.IRNo[i]);
                    item.SubItems.Add(clsArray.SIMID[i]);
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
                    item.SubItems.Add(clsArray.IRDate[i]);
                    item.SubItems.Add(clsArray.InstallationDate[i]);
                    item.SubItems.Add(clsArray.TADateTime[i]);
                    item.SubItems.Add(clsArray.IRImportDateTime[i]);                    
                    item.SubItems.Add(clsArray.ServiceTypeDescription[i]);
                    item.SubItems.Add(clsArray.OtherServiceTypeDescription[i]);
                    item.SubItems.Add(clsArray.TerminalStatus[i]);
                    item.SubItems.Add(clsArray.TerminalStatusDescription[i]);
                    item.SubItems.Add(clsArray.RegionID[i]);
                    item.SubItems.Add(clsArray.Region[i]);
                    item.SubItems.Add(clsArray.TAProcessedBy[i]);
                    item.SubItems.Add(clsArray.TAModifiedBy[i]);
                    item.SubItems.Add(clsArray.TAProcessedDateTime[i]);
                    item.SubItems.Add(clsArray.TAModifiedDateTime[i]);
                    item.SubItems.Add((clsArray.TARemarks[i].Length > 0 ? clsArray.TARemarks[i] : clsFunction.sDash));
                    item.SubItems.Add((clsArray.TAComments[i].Length > 0 ? clsArray.TAComments[i] : clsFunction.sDash));
                    item.SubItems.Add(clsArray.ServiceTypeStatus[i]);
                    item.SubItems.Add(clsArray.ServiceTypeStatusDescription[i]);
                    item.SubItems.Add(clsArray.DockID[i]);
                    item.SubItems.Add(clsArray.DockSN[i]);
                    item.SubItems.Add(clsArray.JobType[i]);
                    item.SubItems.Add(clsArray.JobTypeDescription[i]);
                    item.SubItems.Add(clsArray.JobTypeSubDescription[i]);
                    item.SubItems.Add(clsArray.JobTypeStatusDescription[i]);
                    item.SubItems.Add(clsArray.ServiceNo[i]);
                    item.SubItems.Add(clsArray.RequestNo[i]);

                    i++;

                    dbFunction.UpdateProgressBar(pbLoading, lblSearchStatus, ii);

                    lvwSearch.Items.Add(item);

                    dbFunction.AppDoEvents(true);
                }

                dbFunction.ListViewAlternateBackColor(lvwSearch);
                lblSearchStatus.Text = lvwSearch.Items.Count.ToString() + " " + "record(s) found.";                
                btnPreview.Enabled = true;
            }
            else
            {
                //dbFunction.SetMessageBox("No record found.", "Find TA", clsFunction.IconType.iExclamation);
                lblSearchStatus.Text = "No record found...";
            }

            
            dbFunction.GetResponseTime("Find TA");            
            btnSearchOption.Enabled = true;

            /*
            // Focus first item
            if (lvwSearch.Items.Count > 0)
            {
                lvwSearch.FocusedItem = lvwSearch.Items[0];
                lvwSearch.Items[0].Selected = true;
                lvwSearch.Select();
            }

            if (clsFunction.iOneRecordOnly == lvwSearch.Items.Count)
            {
                SelectedItem();
            }
            */
        }    
        private void frmFindTA_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("--frmFindTA_Load--");

            dbAPI = new clsAPI();
            dbSetting = new clsINI();
            dbFunction = new clsFunction();
            dbExport = new clsExport();
            dbFile = new clsFile();

            dbSetting.InitDatabaseSetting();

            lblSearchStatus.Text = "";
            fSelected = false;                        
            btnPreview.Enabled = false;

            if (fMultipleSelected)
                lvwSearch.MultiSelect = true;
            else
                lvwSearch.MultiSelect = false;

            btnSearchOption.Focus();
            InitMouseRightClick();
            lblHeader.Text = lblHeader.Text + " " + "[ " + sHeader + " ]";

            //frmLoading frmWait = new frmLoading(); // Open Wait Window
            //clsFunction.WaitWindow(true, frmWait);

            if (fAutoLoadData)
            {
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
                LoadTA("View", "", "");
                Cursor.Current = Cursors.Default; // Back to normal 
            }

            //clsFunction.WaitWindow(false, frmWait); // Close Wait Window
        }
        
        private void btnExit_Click(object sender, EventArgs e)
        {
            fSelected = false;
            this.Close();
        }

        private void lvwSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isReport) return; //Do not select for report

            if (lvwSearch.SelectedItems.Count > 0)
            {
                string LineNo = lvwSearch.SelectedItems[0].Text;
                txtLineNo.Text = LineNo;

                if (LineNo.Length > 0)
                {
                    clsSearch.ClassTAIDNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                    clsSearch.ClassClientID = int.Parse(lvwSearch.SelectedItems[0].SubItems[2].Text);
                    clsSearch.ClassServiceProviderID = int.Parse(lvwSearch.SelectedItems[0].SubItems[3].Text);
                    clsSearch.ClassIRIDNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[4].Text);
                    clsSearch.ClassMerchantID = int.Parse(lvwSearch.SelectedItems[0].SubItems[5].Text);
                    clsSearch.ClassFEID = int.Parse(lvwSearch.SelectedItems[0].SubItems[6].Text);
                    clsSearch.ClassTerminalID = int.Parse(lvwSearch.SelectedItems[0].SubItems[7].Text);
                    clsSearch.ClassTerminalTypeID = int.Parse(lvwSearch.SelectedItems[0].SubItems[8].Text);
                    clsSearch.ClassTerminalModelID = int.Parse(lvwSearch.SelectedItems[0].SubItems[9].Text);
                    clsSearch.ClassTerminalBrandID = int.Parse(lvwSearch.SelectedItems[0].SubItems[10].Text);
                    clsSearch.ClassServiceTypeID = int.Parse(lvwSearch.SelectedItems[0].SubItems[11].Text);
                    clsSearch.ClassOtherServiceTypeID = int.Parse(lvwSearch.SelectedItems[0].SubItems[12].Text);                    
                    clsSearch.ClassIRNo = lvwSearch.SelectedItems[0].SubItems[13].Text;
                    clsSearch.ClassSIMID = int.Parse(lvwSearch.SelectedItems[0].SubItems[14].Text);                
                    clsSearch.ClassTerminalSN = lvwSearch.SelectedItems[0].SubItems[15].Text;
                    clsSearch.ClassSIMSerialNo = lvwSearch.SelectedItems[0].SubItems[16].Text;
                    clsSearch.ClassSIMCarrier = lvwSearch.SelectedItems[0].SubItems[17].Text;
                    clsSearch.ClassMerchantName = lvwSearch.SelectedItems[0].SubItems[18].Text;
                    clsSearch.ClassTID = lvwSearch.SelectedItems[0].SubItems[19].Text;
                    clsSearch.ClassMID = lvwSearch.SelectedItems[0].SubItems[20].Text;
                    clsSearch.ClassClientName = lvwSearch.SelectedItems[0].SubItems[21].Text;
                    clsSearch.ClassFEName = lvwSearch.SelectedItems[0].SubItems[22].Text;
                    clsSearch.ClassTypeDescription = lvwSearch.SelectedItems[0].SubItems[23].Text;
                    clsSearch.ClassModelDescription = lvwSearch.SelectedItems[0].SubItems[24].Text;
                    clsSearch.ClassBrandDescription = lvwSearch.SelectedItems[0].SubItems[25].Text;
                    clsSearch.ClassIRDate = lvwSearch.SelectedItems[0].SubItems[26].Text;
                    clsSearch.ClassInstallationDate = lvwSearch.SelectedItems[0].SubItems[27].Text;
                    clsSearch.ClassTADateTime = lvwSearch.SelectedItems[0].SubItems[28].Text;
                    clsSearch.ClassIRImportDateTime = lvwSearch.SelectedItems[0].SubItems[29].Text;                   
                    clsSearch.ClassServiceTypeDescription = lvwSearch.SelectedItems[0].SubItems[30].Text;
                    clsSearch.ClassOtherServiceTypeDescription = lvwSearch.SelectedItems[0].SubItems[31].Text;
                    clsSearch.ClassIRStatus = int.Parse(lvwSearch.SelectedItems[0].SubItems[32].Text);
                    clsSearch.ClassIRStatusDescription = lvwSearch.SelectedItems[0].SubItems[33].Text;
                    clsSearch.ClassRegionID = int.Parse(lvwSearch.SelectedItems[0].SubItems[34].Text);
                    clsSearch.ClassRegion = lvwSearch.SelectedItems[0].SubItems[35].Text;
                    clsSearch.ClassTAProcessedBy = lvwSearch.SelectedItems[0].SubItems[36].Text;
                    clsSearch.ClassTAModifiedBy = lvwSearch.SelectedItems[0].SubItems[37].Text;
                    clsSearch.ClassTAProcessedDateTime = lvwSearch.SelectedItems[0].SubItems[38].Text;
                    clsSearch.ClassTAModifiedDateTime = lvwSearch.SelectedItems[0].SubItems[39].Text;
                    clsSearch.ClassTARemarks = lvwSearch.SelectedItems[0].SubItems[40].Text;
                    clsSearch.ClassTAComments = lvwSearch.SelectedItems[0].SubItems[41].Text;
                    clsSearch.ClassServiceTypeStatus = int.Parse(lvwSearch.SelectedItems[0].SubItems[42].Text);
                    clsSearch.ClassServiceTypeStatusDescription = lvwSearch.SelectedItems[0].SubItems[43].Text;
                    clsSearch.ClassDockID = int.Parse(lvwSearch.SelectedItems[0].SubItems[44].Text);
                    clsSearch.ClassDockSN = lvwSearch.SelectedItems[0].SubItems[45].Text;
                    clsSearch.ClassJobType = int.Parse(lvwSearch.SelectedItems[0].SubItems[46].Text);
                    clsSearch.ClassJobTypeDescription = lvwSearch.SelectedItems[0].SubItems[47].Text;
                    clsSearch.ClassJobTypeSubDescription = lvwSearch.SelectedItems[0].SubItems[48].Text;
                    clsSearch.ClassJobTypeStatusDescription = lvwSearch.SelectedItems[0].SubItems[49].Text;
                    clsSearch.ClassServiceNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[50].Text);
                    clsSearch.ClassRequestNo = lvwSearch.SelectedItems[0].SubItems[51].Text;
                }
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

        private void lvwSearch_DoubleClick(object sender, EventArgs e)
        {            
            SelectedItem();
        }

        private void bunifuCards8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnSearchClear_Click(object sender, EventArgs e)
        {
            dbFunction.InitProgressBar(pbLoading, 0);                        
            btnPreview.Enabled = false;
            btnSearchOption.Enabled = true;
            lvwSearch.Items.Clear();            
        }
        private void ClearDataGrid()
        {
            lvwSearch.Items.Clear();
        }

        private void btnSearchOption_Click(object sender, EventArgs e)
        {
            //if (!dbAPI.isCheckTable()) return;

            Cursor.Current = Cursors.WaitCursor;

            dbAPI.ResetAdvanceSearch();

            frmPrintOptionCriteria.sDefaultIRStatus = "";
            frmPrintOptionCriteria.sDefaultSIMStatus = "";
            frmPrintOptionCriteria.sDefaultTerminalStatus = "";
            frmPrintOptionCriteria.sDefaultServiceType = "";

            try
            {
                if (sDefaultTerminalStatus.Length > 0)
                    frmPrintOptionCriteria.sDefaultTerminalStatus = sDefaultTerminalStatus;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                // Do nothing...                    
            }

            try
            {
                if (sDefaultIRStatus.Length > 0)
                    frmPrintOptionCriteria.sDefaultIRStatus = sDefaultIRStatus;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                // Do nothing...                    
            }

            frmPrintOptionCriteria.sHeader = "FIND TERMINAL ALLOCATION";
            frmPrintOptionCriteria.iSearchType = frmPrintOptionCriteria.SearchType.iTA;
            frmPrintOptionCriteria frm = new frmPrintOptionCriteria();
            frm.ShowDialog();

            if (frmPrintOptionCriteria.fSelected)
            {
                lvwSearch.Items.Clear();                
                btnSearchOption.Enabled = false;
                lblSearchOption.Text = "(-)Search Options";
                lblSearchOption_Click(this, e);
                
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
                LoadTA("View", "", "");
                Cursor.Current = Cursors.Default; // Back to normal 
            }
            else
            {
                btnSearchOption.Enabled = true;
            }

            Cursor.Current = Cursors.Default;
        }
        
        bool fContinueConfirm()
        {
            bool fContinue = true;

            if (MessageBox.Show("This process may take a few minute(s)." +
                                    "\n\n",
                                    "Continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fContinue = false;
            }

            return fContinue;
        }
       
        private void frmFindTA_Activated(object sender, EventArgs e)
        {
            btnSearchOption.Focus();            
        }
        private void PanelSearchResize()
        {
            if (lblSearchOption.Text.CompareTo("(+)Search Options") == 0)
            {
                pnlSearch.Height = iPanelSearchHeightMin;
            }
            else
            {
                pnlSearch.Height = iPanelSearchHeightMax;
            }
        }

        private void PanelListLocationAndHeight()
        {
            var curLoc = pnlSearch.Location;

            if (lblSearchOption.Text.CompareTo("(+)Search Options") == 0)
            {
                pnlList.Location = new Point(curLoc.X, pnlSearch.Height + iPanelSearchHeightMin);
            }
            else
            {
                pnlList.Location = new Point(curLoc.X, pnlSearch.Height + iPanelSearchHeightMin);
            }

            pnlList.Height = (iPanelListHeightMax - pnlSearch.Height) + iPanelSearchHeightMin;
        }

        private void lblSearchOption_Click(object sender, EventArgs e)
        {
            if (lblSearchOption.Text.CompareTo("(+)Search Options") == 0)
            {
                lblSearchOption.Text = "(-)Search Options";
                lblSearchOptionDetails.Visible = true;
                lblSearchOptionDetails.Text = frmPrintOptionCriteria.sSearchOptionDetails;
            }
            else
            {
                lblSearchOption.Text = "(+)Search Options";
                lblSearchOptionDetails.Visible = false;
                lblSearchOptionDetails.Text = "";
            }

            PanelSearchResize();
            PanelListLocationAndHeight();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {           
            // Open Dialog box for saving
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = dbFunction.GetExportFileName(clsFunction.ExportType.iTA);
            savefile.Filter = "Excel File (*.xls)|*.xls";
            savefile.InitialDirectory = @dbFile.sExportPath;
            string sFileName = System.IO.Path.GetFileName(savefile.FileName);

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                string sFilePath = @System.IO.Path.GetFullPath(savefile.FileName.Replace(sFileName, ""));
                dbExport.ExportToExcel(lvwSearch, sFilePath, sFileName, "Terminal Allocation");                
            }
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

        private void frmFindTA_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    fSelected = false;
                    this.Close();
                    break;
                case Keys.Enter:
                    lvwSearch_DoubleClick(this, e);
                    break;
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {           
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {                
                clsSearch.ClassReportID = 0;
                clsSearch.ClassSearchBy = "Advance TA";
                clsSearch.ClassCurrentPage = int.Parse(clsFunction.sZero);
                clsSearch.ClassTotalPage = int.Parse(clsFunction.sZero);

                ComposeSearchValue(); // Set clsSearch.ClassSearchValue

                clsSearch.ClassStoredProcedureName = "spViewAdvanceTA";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        private void tsmPreviewFSR_Click(object sender, EventArgs e)
        {
            if (clsSearch.ClassIRNo.Length > 0 && clsSearch.ClassIRNo.CompareTo("0") !=0 && lvwSearch.Items.Count > 0)
            {
                fMultipleSelected = false;
                cmsLvwSearch.Visible = false;

                //if (!fSelectedConfirm()) return;

                //dbFunction.PreviewTA(clsSearch.ClassIRNo);
            }
            else
            {
                dbFunction.SetMessageBox("Select record from the list.", "TA(FSR) Preview", clsFunction.IconType.iError);
            }
        }

        private void tsmTADispatch_Click(object sender, EventArgs e)
        {
            Dispatch();
        }

        private void Dispatch()
        {
            bool fMultiple = false;
            int i = 0;

            TAIDNoCol.Clear();
            IRIDNoCol.Clear();
            IRNoCol.Clear();
            TerminalIDCol.Clear();
            TerminalSNCol.Clear();
            MerchantNameCol.Clear();
            IRStatusCol.Clear();
            IRStatusDescription.Clear();

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
                        IRNoCol.Add(item.SubItems[13].Text);
                        TerminalSNCol.Add(item.SubItems[15].Text);
                        MerchantNameCol.Add(item.SubItems[18].Text);
                        IRStatusCol.Add(item.SubItems[32].Text);
                        IRStatusDescription.Add(item.SubItems[33].Text);
                    }
                }

            }
            else
            {
                fSelected = true;
                TAIDNoCol.Add(lvwSearch.SelectedItems[0].SubItems[1].Text);
                IRIDNoCol.Add(lvwSearch.SelectedItems[0].SubItems[4].Text);
                TerminalIDCol.Add(lvwSearch.SelectedItems[0].SubItems[7].Text);
                IRNoCol.Add(lvwSearch.SelectedItems[0].SubItems[13].Text);
                TerminalSNCol.Add(lvwSearch.SelectedItems[0].SubItems[15].Text);
                MerchantNameCol.Add(lvwSearch.SelectedItems[0].SubItems[18].Text);
                IRStatusCol.Add(lvwSearch.SelectedItems[0].SubItems[32].Text);
                IRStatusDescription.Add(lvwSearch.SelectedItems[0].SubItems[33].Text);
            }

            clsArray.TAIDNo = TAIDNoCol.ToArray();
            clsArray.IRIDNo = IRIDNoCol.ToArray();
            clsArray.TerminalID = TerminalIDCol.ToArray();
            clsArray.IRNo = IRNoCol.ToArray();
            clsArray.TerminalSN = TerminalSNCol.ToArray();
            clsArray.MerchantName = MerchantNameCol.ToArray();
            clsArray.IRStatus = IRStatusCol.ToArray();
            clsArray.IRStatusDescription = IRStatusDescription.ToArray();

            ProcessTerminalDispatch();

            // Reload Allocated            
            fMultipleSelected = true;
            fAllowSelect = false;
            fConfirm = false;
            lvwSearch.Items.Clear();
            btnSearchOption.Enabled = false;
            dbAPI.ResetAdvanceSearch();
            sDefaultTerminalStatus = clsGlobalVariables.STATUS_ALLOCATED_DESC;
            clsSearch.ClassTerminalStatusType = clsGlobalVariables.STATUS_ALLOCATED;
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
                    string sTemp = clsArray.IRIDNo[i].ToString() + clsFunction.sComma + clsFunction.sPadSpace + 
                                   //clsArray.TerminalSN[i] + clsFunction.sComma + 
                                   //clsArray.MerchantName[i] + clsFunction.sComma + clsFunction.sPadSpace +
                                   clsArray.IRStatusDescription[i].ToString() + "\n";
                    sItem = sItem + sTemp;

                    string sStatusDescription = clsArray.IRStatusDescription[i].ToString();
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
                    string sStatusDescription = clsArray.IRStatusDescription[i].ToString();
                    if (sStatusDescription.CompareTo(clsGlobalVariables.STATUS_ALLOCATED_DESC) == 0)
                    {
                        clsSearch.ClassStatus = clsGlobalVariables.STATUS_DISPATCH;
                        clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_DISPATCH_DESC;

                        // Update Status (tblterminaldetail)
                        string sTerminalID = clsArray.TerminalID[i].ToString();
                        int iTerminalID = int.Parse((sTerminalID.Length > 0 ? sTerminalID : "0"));
                        if (iTerminalID > 0)
                        {
                            clsSearch.ClassSearchValue = sTerminalID + clsFunction.sPipe +
                                                                clsSearch.ClassStatus.ToString() + clsFunction.sPipe +
                                                                clsSearch.ClassStatusDescription;

                            Debug.WriteLine("Terminal::clsSearch.ClassSearchValue=" + clsSearch.ClassSearchValue);

                            dbAPI.ExecuteAPI("PUT", "Update", "Update Terminal Detail Status", clsSearch.ClassSearchValue, "", "", "UpdateCollectionDetail");
                        }                            

                        // Update Status (tblirdetail)
                        string sIRIDNo = clsArray.IRIDNo[i].ToString();
                        if (sIRIDNo.Length > 0)
                        {
                            clsSearch.ClassSearchValue = sIRIDNo + clsFunction.sPipe +
                                                                clsSearch.ClassStatus.ToString() + clsFunction.sPipe +
                                                                clsSearch.ClassStatusDescription;

                            Debug.WriteLine("IR::clsSearch.ClassSearchValue=" + clsSearch.ClassSearchValue);

                            dbAPI.ExecuteAPI("PUT", "Update", "Update IR Detail Status", clsSearch.ClassSearchValue, "", "", "UpdateCollectionDetail");
                        }                            

                        // Update Status (tblterminalallocation)
                        string sTAIDNo = clsArray.TAIDNo[i].ToString();
                        int iTAIDNo = int.Parse((sTAIDNo.Length > 0 ? sTAIDNo : "0"));
                        if (iTAIDNo > 0)
                        {
                            clsSearch.ClassSearchValue = sTAIDNo + clsFunction.sPipe +
                                                                clsSearch.ClassStatus.ToString() + clsFunction.sPipe +
                                                                clsSearch.ClassStatusDescription;

                            Debug.WriteLine("TA::clsSearch.ClassSearchValue=" + clsSearch.ClassSearchValue);

                            dbAPI.ExecuteAPI("PUT", "Update", "Update TA Detail Status", clsSearch.ClassSearchValue, "", "", "UpdateCollectionDetail");
                        }
                                                                                                   
                    }

                    i++;
                }

                if (iCount > 0)
                {
                    dbFunction.SetMessageBox("Terminal dispatch successfully.", "Dispatch Terminal", clsFunction.IconType.iInformation);

                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
                    dbAPI.ResetAdvanceSearch();
                    LoadTA("View", "", "");
                    Cursor.Current = Cursors.Default; // Back to normal 
                }
                else
                    dbFunction.SetMessageBox("No terminal to be dispatch.", "Dispatch Terminal", clsFunction.IconType.iExclamation);
            }
        }

        private void cmsLvwSearch_Opening(object sender, CancelEventArgs e)
        {

        }

        private void SelectedItem()
        {                        
            if (!fAllowSelect) return;

            if (fConfirm)
            {
                if (!fSelectedConfirm()) return;
            }

            fSelected = true;

            switch (iSearchType)
            {
                case SearchType.iDispatch:
                    break;
                default:
                    this.Close();
                    break;  
            }            
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }

        private void InitMouseRightClick()
        {            
            switch (iSearchType)
            {
                case SearchType.iDispatch:
                    cmsLvwSearch.Enabled = true;
                    break;
                case SearchType.iFSR:
                    cmsLvwSearch.Enabled = false;
                    break;
                default:
                    cmsLvwSearch.Enabled = false;
                    break;
            }
        }

        private void ComposeSearchValue()
        {
            Debug.WriteLine("--ComposeSearchValue--");

            clsSearch.ClassSearchValue = clsSearch.ClassServiceTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassParticularID + clsFunction.sPipe +
                                                clsSearch.ClassTerminalTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalModelID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalBrandID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalSN + clsFunction.sPipe +
                                                clsSearch.ClassIRNo + clsFunction.sPipe +
                                                clsSearch.ClassClientID + clsFunction.sPipe +
                                                clsSearch.ClassServiceProviderID + clsFunction.sPipe +
                                                clsSearch.ClassFEID + clsFunction.sPipe +
                                                clsSearch.ClassTID + clsFunction.sPipe +
                                                clsSearch.ClassMID + clsFunction.sPipe +
                                                clsSearch.ClassTerminalStatusType + clsFunction.sPipe +
                                                clsSearch.ClassReqDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassReqDateTo + clsFunction.sPipe +
                                                clsSearch.ClassInstDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassInstDateTo + clsFunction.sPipe +
                                                clsSearch.ClassTADateFrom + clsFunction.sPipe +
                                                clsSearch.ClassTADateTo + clsFunction.sPipe +
                                                clsSearch.ClassFSRDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassFSRDateTo + clsFunction.sPipe +
                                                clsSearch.ClassIRStatus + clsFunction.sPipe +
                                                clsSearch.ClassSIMSerialNo + clsFunction.sPipe +
                                                clsSearch.ClassTAIDNo + clsFunction.sPipe +
                                                clsSearch.ClassIRIDNo + clsFunction.sPipe +                                                
                                                clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                clsSearch.ClassTotalPage + clsFunction.sPipe +
                                                clsSearch.ClassRegionID + clsFunction.sPipe + // Region
                                                clsSearch.ClassProvinceID +clsFunction.sPipe +  // Province 
                                                clsFunction.sZero + clsFunction.sPipe;   // ServiceNo

            Debug.WriteLine("ComposeSearchValue, clsSearch.ClassSearchValue=" + clsSearch.ClassSearchValue);
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
