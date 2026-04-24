using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bunifu.Framework.Lib;
using Bunifu.Framework.UI;
using System.Diagnostics;

namespace MIS
{
    public partial class frmFindFSR : Form
    {
        public static bool fAutoLoadData = false;
        public static bool fAllowSelect = false;
        public bool fSelected = false;
        public static string sHeader;
        public static bool isReport = false;

        private clsAPI dbAPI;
        private clsINI dbSetting;
        private clsFunction dbFunction;

        private static int iPanelSearchHeightMin = 35;
        private static int iPanelSearchHeightMax = 300;
        private static int iPanelListHeightMax = 519;

        public static string sDefaultIRStatus;
        public static string sDefaultTerminalStatus;

        public static CountType iCountType;

        // The column we are currently using for sorting.
        private ColumnHeader SortingColumn = null;

        public frmFindFSR()
        {
            InitializeComponent();
        }

        public enum CountType
        {
            iTInstallationReq, iTInstallationReqDaysPending, iTInstallationReqOverDue,
            iTInstallation, iTServicing, iTPullOut, iTReplacement, iTReprogramming, iTDispatch,
            iSVCReqServicing, iSVCPullOut, iSVCReplacement, iSVCReprogramming,
            iTotalInstalled, iTotalPullOut, iTotalReplacement, iTotalReprogramming, iTotalServicing, iTotalCancelled,
            iSearchIRInstalled,
            iSearchNegativeInstallation, iSearchNegativePullOut, iSearchNegativeReplacement, iSearchNegativeReprogramming, iSearchNegativeServicing,
            iSearchFSR
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }       

        private void frmFindFSR_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("--frmFindFSR_Load--");

            dbAPI = new clsAPI();
            dbSetting = new clsINI();
            dbFunction = new clsFunction();
            dbSetting.InitDatabaseSetting();

            lblSearchStatus.Text = "";
            ClearDataGrid();            
            fSelected = false;
            
            btnPreview.Enabled = false;
            InitPage(0, 0);

            iCountType = CountType.iSearchFSR;
            Debug.WriteLine("iCountType=" + iCountType);
            ProcessPage(iCountType);
            
            lblHeader.Text = lblHeader.Text + " " + "[ " + sHeader + " ]";

            //frmLoading frmWait = new frmLoading(); // Open Wait Window
            //clsFunction.WaitWindow(true, frmWait);

            if (fAutoLoadData)
            {
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
                LoadFSR("View", "", "");
                Cursor.Current = Cursors.Default; // Back to normal 
            }

            //clsFunction.WaitWindow(false, frmWait); // Close Wait Window
        }        
        
        private void LoadFSR(string StatementType, string SearchBy, string SearchValue)
        {
            int i = 0;
            int ii = 0;
            
            lblSearchStatus.Text = "";
            lvwSearch.Items.Clear();

            clsSearch.ClassCurrentPage = clsSearch.ClassCurrentPage;
            clsSearch.ClassTotalPage = dbFunction.GetPageLimit();

            ComposeSearchValue(); // Set clsSearch.ClassSearchValue

            dbFunction.GetRequestTime("Find FSR");

            dbAPI.ExecuteAPI("GET", "View", "Advance FSR", clsSearch.ClassSearchValue, "FSR", "", "ViewAdvanceFSR");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                dbFunction.InitProgressBar(pbLoading, clsArray.FSRID.Length);

                while (clsArray.FSRID.Length > i)
                {
                    ii++;
                    //clsProductInfo.ResetProductInfo();
                    clsFSR.ClassFSRID = int.Parse(clsArray.FSRID[i].ToString());
                    clsFSR.ClassFSRDate = clsArray.FSRDate[i];
                    clsFSR.ClassFSRTime = clsArray.FSRTime[i];
                    clsFSR.ClassMerchant = clsArray.Merchant[i];                    
                    clsFSR.ClassTID = clsArray.TID[i];
                    clsFSR.ClassMID = clsArray.MID[i];
                    clsFSR.ClassInvoiceNo = clsArray.InvoiceNo[i];
                    clsFSR.ClassBatchNo = clsArray.BatchNo[i];
                    clsFSR.ClassTimeArrived = clsArray.TimeArrived[i];
                    clsFSR.ClassTimeStart = clsArray.TimeStart[i];
                    clsFSR.ClassTimeEnd = clsArray.TimeEnd[i];                    
                    clsFSR.ClassFSR = clsArray.FSR[i];
                    clsFSR.ClassServiceTypeDescription = clsArray.ServiceTypeDescription[i];
                    clsFSR.ClassServiceStatusDescription = clsArray.ServiceStatusDescription[i];
                    clsFSR.ClassFSRDate = clsArray.FSRDate[i];                    
                    clsFSR.ClassTxnAmt = clsArray.TxnAmt[i];                    
                    clsFSR.ClassTerminalSN = clsArray.TerminalSN[i];
                    clsFSR.ClassMerchantContactNo = clsArray.MerchantContactNo[i];
                    clsFSR.ClassMerchantRepresentative = clsArray.MerchantRepresentative[i];                    
                    clsFSR.ClassSerialNo = clsArray.SerialNo[i];
                    clsFSR.ClassFSRStatusDescription = clsArray.FSRStatusDescription[i];
                    clsFSR.ClassProcessType = clsArray.ProcessType[i];
                    clsFSR.ClassIRNo = clsArray.IRNo[i];
                    clsFSR.ClassClientName = clsArray.ClientName[i];
                    clsFSR.ClassFEName = clsArray.FEName[i];                    
                    clsFSR.ClassSIMSN = clsArray.SIMSerialNo[i];
                    clsFSR.ClassPowerSN = clsArray.PowerSN[i];
                    clsFSR.ClassDockSN = clsArray.DockSN[i];
                    clsFSR.ClassTypeDescription = clsArray.TypeDescription[i];
                    clsFSR.ClassModelDescription = clsArray.ModelDescription[i];
                    clsFSR.ClassServiceNo = int.Parse(clsArray.ServiceNo[i]);
                    clsFSR.ClassRequsetNo = clsArray.RequestNo[i];
                    clsFSR.ClassJobTypeDescription = clsArray.JobTypeDescription[i];
                    clsFSR.ClassServiceJobTypeDescription = clsArray.ServiceJobTypeDescription[i];

                    i++;

                    dbFunction.UpdateProgressBar(pbLoading, lblSearchStatus, ii);

                    AddItem(i);

                    dbFunction.AppDoEvents(true);
                }

                dbFunction.ListViewAlternateBackColor(lvwSearch);
                lblSearchStatus.Text = lvwSearch.Items.Count.ToString() + " " + "record(s) found.";
                
                btnPreview.Enabled = true;
            }
            else
            {
                //dbFunction.SetMessageBox("No record found.", "Find FSR", clsFunction.IconType.iExclamation);
                lblSearchStatus.Text = "No record found...";
            }

            dbFunction.GetResponseTime("Find FSR");            
            btnSearchOption.Enabled = true;

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
        private void AddItem(int inLineNo)
        {
            // Add to List            
            ListViewItem item = new ListViewItem(inLineNo.ToString());
            item.SubItems.Add(clsFSR.ClassFSRID.ToString());
            item.SubItems.Add(clsFSR.ClassFSRDate.ToString());
            item.SubItems.Add(clsFSR.ClassFSRTime.ToString());
            item.SubItems.Add(clsFSR.ClassMerchant.ToString());            
            item.SubItems.Add(clsFSR.ClassTID.ToString());
            item.SubItems.Add(clsFSR.ClassMID.ToString());
            item.SubItems.Add(clsFSR.ClassInvoiceNo.ToString());
            item.SubItems.Add(clsFSR.ClassBatchNo.ToString());
            item.SubItems.Add(clsFSR.ClassTimeArrived.ToString());
            item.SubItems.Add(clsFSR.ClassTimeStart.ToString());
            item.SubItems.Add(clsFSR.ClassTimeEnd.ToString());
            item.SubItems.Add(clsFSR.ClassFSR.ToString());
            item.SubItems.Add(clsFSR.ClassServiceTypeDescription.ToString());            
            item.SubItems.Add(clsFSR.ClassTxnAmt.ToString());
            item.SubItems.Add(clsFSR.ClassTerminalSN.ToString());
            item.SubItems.Add(clsFSR.ClassTypeDescription.ToString());
            item.SubItems.Add(clsFSR.ClassModelDescription.ToString());
            item.SubItems.Add(clsFSR.ClassMerchantContactNo.ToString());
            item.SubItems.Add(clsFSR.ClassMerchantRepresentative.ToString());            
            item.SubItems.Add(clsFSR.ClassSerialNo.ToString());
            item.SubItems.Add(clsFSR.ClassServiceStatusDescription.ToString());
            item.SubItems.Add(clsFSR.ClassFSRStatusDescription.ToString());
            item.SubItems.Add(clsFSR.ClassProcessType.ToString());
            item.SubItems.Add(clsFSR.ClassIRNo.ToString());
            item.SubItems.Add(clsFSR.ClassClientName.ToString());
            item.SubItems.Add(clsFSR.ClassFEName.ToString());            
            item.SubItems.Add(clsFSR.ClassSIMSN.ToString());
            item.SubItems.Add(clsFSR.ClassPowerSN.ToString());
            item.SubItems.Add(clsFSR.ClassDockSN.ToString());
            item.SubItems.Add(clsFSR.ClassServiceNo.ToString());
            item.SubItems.Add(clsFSR.ClassRequsetNo.ToString());
            item.SubItems.Add(clsFSR.ClassJobTypeDescription.ToString());
            item.SubItems.Add(clsFSR.ClassServiceJobTypeDescription.ToString());

            Debug.WriteLine("frmFindFSR->" + inLineNo.ToString() + "=" + clsFSR.ClassServiceTypeDescription + "," + clsFSR.ClassServiceStatusDescription + "," + clsFSR.ClassFSRStatusDescription);

            lvwSearch.Items.Add(item);
        }

        private void btnSearchClear_Click(object sender, EventArgs e)
        {
            dbFunction.InitProgressBar(pbLoading, 0);            
            btnSearchOption.Enabled = true;
            lvwSearch.Items.Clear();            
            btnPreview.Enabled = false;
        }
        private void ClearDataGrid()
        {                        
            lvwSearch.Items.Clear();
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
        private void lvwSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isReport) return; //Do not select for report
        }

        private void cboSearchMerchant_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        
        private void btnSearchOption_Click(object sender, EventArgs e)
        {
            //if (!dbAPI.isCheckTable()) return;

            Cursor.Current = Cursors.WaitCursor;

            dbAPI.ResetAdvanceSearch();

            if (fAllowSelect)
            {
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
                    //dbFunction.SetMessageBox(ex.Message, "Find FSR", clsFunction.IconType.iError);
                }
            }

            frmPrintOptionCriteria.sHeader = "FIND FSR";
            frmPrintOptionCriteria.iSearchType = frmPrintOptionCriteria.SearchType.iFSR;
            frmPrintOptionCriteria frm = new frmPrintOptionCriteria();
            frm.ShowDialog();

            if (frmPrintOptionCriteria.fSelected)
            {
                lvwSearch.Items.Clear();                
                btnSearchOption.Enabled = false;
                lblSearchOption.Text = "(-)Search Options";
                lblSearchOption_Click(this, e);
                                
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass                
                ProcessPage(iCountType);
                Cursor.Current = Cursors.Default; // Back to normal 
            }
            else
            {
                btnSearchOption.Enabled = true;
            }

            Cursor.Current = Cursors.Default;
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

        private void frmFindFSR_Activated(object sender, EventArgs e)
        {
            btnSearchOption.Focus();
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

        private void frmFindFSR_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    fSelected = false;
                    this.Close();
                    break;
                case Keys.Enter:                    
                    break;
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {                                
                clsSearch.ClassReportID = 4;
                clsSearch.ClassSearchBy = "Advance FSR";
                clsSearch.ClassCurrentPage = int.Parse(clsFunction.sZero);
                clsSearch.ClassTotalPage = int.Parse(clsFunction.sZero);

                ComposeSearchValue(); // Set clsSearch.ClassSearchValue

                clsSearch.ClassStoredProcedureName = "spViewAdvanceFSR";
                dbFunction.ProcessReport(clsSearch.ClassReportID);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        private void lvwSearch_DoubleClick(object sender, EventArgs e)
        {

        }

        private void ComposeSearchValue()
        {
            Debug.WriteLine("--ComposeSearchValue--");

            clsSearch.ClassSearchValue = clsSearch.ClassServiceTypeID + clsFunction.sPipe +
                                             clsSearch.ClassParticularID + clsFunction.sPipe +
                                             clsSearch.ClassTerminalTypeID + clsFunction.sPipe +
                                             clsSearch.ClassTerminalModelID + clsFunction.sPipe +
                                             clsSearch.ClassTerminalBrandID + clsFunction.sPipe +
                                             clsSearch.ClassTerminalSN + clsFunction.sPipe +
                                             clsSearch.ClassIRNo + clsFunction.sPipe +
                                             clsSearch.ClassClientID + clsFunction.sPipe +
                                             clsSearch.ClassServiceProviderID + clsFunction.sPipe +
                                             clsSearch.ClassFEID + clsFunction.sPipe +
                                             clsSearch.ClassTID + clsFunction.sPipe +
                                             clsSearch.ClassMID + clsFunction.sPipe +
                                             clsFunction.sZero + clsFunction.sPipe +
                                             clsFunction.sZero + clsFunction.sPipe +
                                             clsSearch.ClassTerminalStatus + clsFunction.sPipe +
                                             clsSearch.ClassFSRStatus + clsFunction.sPipe +
                                             clsSearch.ClassServiceStatus + clsFunction.sPipe +
                                             clsSearch.ClassFSRDateFrom + clsFunction.sPipe +
                                             clsSearch.ClassFSRDateTo + clsFunction.sPipe +
                                             clsSearch.ClassActionMade + clsFunction.sPipe +
                                             clsSearch.ClassJobTypeDescription + clsFunction.sPipe +
                                             clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                             clsSearch.ClassTotalPage + clsFunction.sPipe +
                                             clsSearch.ClassRegionID + clsFunction.sPipe + // Region
                                             clsSearch.ClassProvinceID + clsFunction.sPipe;  // Location

            Debug.WriteLine("ComposeSearchValue, clsSearch.ClassSearchValue="+ clsSearch.ClassSearchValue);
        }

        // -------------------------------------------------------------------------------------
        // Pagination
        // -------------------------------------------------------------------------------------
        private void InitPage(int iCurrentPage, int iTotalPage)
        {
            if (iTotalPage > 0)
            {
                // do nothing...
            }
            else
            {
                iCurrentPage = 1;
            }

            clsSearch.ClassCurrentPage = iCurrentPage;
            clsSearch.ClassTotalPage = iTotalPage;

            txtPage.Text = iCurrentPage.ToString() + " / " + iTotalPage.ToString();
        }
        private int GetTotalPage(CountType iType)
        {
            int iCount = 0;
            int totalPage = 0;
            int iLimitSize = dbFunction.GetPageLimit();

            lblTotalCount.Text = clsFunction.sNull;
            Debug.WriteLine("GetTotalPage::iType=" + iType);
            Debug.WriteLine("GetTotalPage::iLimitSize=" + iLimitSize);

            iCount = 0;
            switch (iType)
            {                
                case CountType.iSearchFSR:
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassClientID + clsFunction.sPipe + clsGlobalVariables.STATUS_INSTALLED_DESC;
                    dbAPI.GetViewCount("Search", "Client Status Count", clsSearch.ClassAdvanceSearchValue, "Get Count");
                    break;
            }

            if (dbAPI.isNoRecordFound() == false)
            {
                iCount = clsTerminal.ClassTerminalCount;
                lblTotalCount.Text = iCount.ToString() + " " + "total record(s)";
            }

            if (iCount > 0)
            {
                totalPage = (int)Math.Ceiling((double)iCount / iLimitSize);
            }
            else
            {
                totalPage = 0;
            }

            return totalPage;
        }
        private void ProcessPage(CountType iType)
        {
            if (clsSearch.ClassClientID > 0)
            {
                InitPage(int.Parse(clsFunction.sOne), GetTotalPage(iType));
                LoadFSR("View", "", "");
            }
            else
            {
                InitPage(int.Parse(clsFunction.sOne), int.Parse(clsFunction.sOne));
            }                
        }

        private void btnFirstPage_Click(object sender, EventArgs e)
        {
            clsSearch.ClassCurrentPage = int.Parse(clsFunction.sOne);
            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadFSR("View", "", "");
        }

        private void btnLastPage_Click(object sender, EventArgs e)
        {
            clsSearch.ClassCurrentPage = clsSearch.ClassTotalPage;
            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadFSR("View", "", "");
        }

        private void btnPreviousPage_Click(object sender, EventArgs e)
        {
            if (clsSearch.ClassCurrentPage > int.Parse(clsFunction.sOne))
                clsSearch.ClassCurrentPage--;

            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadFSR("View", "", "");
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (clsSearch.ClassCurrentPage < clsSearch.ClassTotalPage)
                clsSearch.ClassCurrentPage++;

            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadFSR("View", "", "");
        }

        // -------------------------------------------------------------------------------------
        // Pagination
        // -------------------------------------------------------------------------------------

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
