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
    public partial class frmFindSIM : Form
    {
        public static bool fAutoLoadData = false;
        public static bool fAllowSelect = false;
        public static bool fConfirm = false;
        public static string sFindType = "";
        public bool fSelected = false;
        public static string sHeader;

        private clsAPI dbAPI;
        private clsINI dbSetting;
        private clsFunction dbFunction;
        private clsExport dbExport;

        string sStatementType = clsFunction.sNull;
        string sSearchBy = clsFunction.sNull;
        string sSearchValue = clsFunction.sNull;

        private static int iPanelSearchHeightMin = 35;
        private static int iPanelSearchHeightMax = 300;
        private static int iPanelListHeightMax = 519;

        // The column we are currently using for sorting.
        private ColumnHeader SortingColumn = null;

        public frmFindSIM()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            fSelected = false;
            this.Close();
        }

        private void frmFindSIM_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("--frmFindSIM_Load--");

            dbAPI = new clsAPI();
            dbSetting = new clsINI();
            dbFunction = new clsFunction();
            dbExport = new clsExport();

            dbSetting.InitDatabaseSetting();

            lblSearchStatus.Text = "";
            ClearDataGrid();            
            btnSearchOption.Focus();            
            btnPreview.Enabled = false;
            lblHeader.Text = lblHeader.Text + " " + "[ " + sHeader + " ]";

            InitPage(0, 0);

            //frmLoading frmWait = new frmLoading(); // Open Wait Window
            //clsFunction.WaitWindow(true, frmWait);

            if (fAutoLoadData)
            {
                ProcessPage();
                LoadSIMWithAllocation("View", "", "");
            }

            //clsFunction.WaitWindow(false, frmWait); // Close Wait Window
        }
        private void ClearDataGrid()
        {
            lvwSearch.Items.Clear();
        }
        
        private void btnSearchClear_Click(object sender, EventArgs e)
        {
            //dbFunction.InitProgressBar(pbLoading, 0);            
            btnSearchOption.Enabled = true;
            lvwSearch.Items.Clear();            
            btnPreview.Enabled = false;
        }
        
        private void LoadSIMWithAllocation(string StatementType, string SearchBy, string SearchValue)
        {
            int i = 0;
            int ii = 0;

            lblSearchStatus.Text = "";
            lvwSearch.Items.Clear();

            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassServiceTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassParticularID + clsFunction.sPipe +
                                                clsSearch.ClassTerminalTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalModelID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalBrandID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalSN + clsFunction.sPipe +
                                                clsSearch.ClassTerminalStatusType + clsFunction.sPipe +
                                                clsSearch.ClassSIMID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassSIMSerialNo.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassSIMStatus + clsFunction.sPipe +
                                                clsSearch.ClassSIMCarrier + clsFunction.sPipe +
                                                clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                dbFunction.GetPageLimit();


            Debug.WriteLine("LoadSIMWithAllocation::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            // For View
            sSearchValue = clsSearch.ClassServiceTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassParticularID + clsFunction.sPipe +
                                                clsSearch.ClassTerminalTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalModelID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalBrandID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalSN + clsFunction.sPipe +
                                                clsSearch.ClassTerminalStatusType + clsFunction.sPipe +
                                                clsSearch.ClassSIMID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassSIMSerialNo.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassSIMStatus + clsFunction.sPipe +
                                                clsSearch.ClassSIMCarrier + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero;

            Debug.WriteLine("FindSIM::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbFunction.GetRequestTime("Find SIM With TA");

            dbAPI.ExecuteAPI("GET", "View", "Advance SIM", clsSearch.ClassAdvanceSearchValue, "TA", "", "ViewAdvanceTA");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                if (clsTA.RecordFound)
                {
                   // dbFunction.InitProgressBar(pbLoading, clsArray.SIMID.Length);

                    while (clsArray.SIMID.Length > i)
                    {
                        ii++;
                        
                        clsTA.ClassSIMID = int.Parse(clsArray.SIMID[i]);
                        clsTA.ClassFEID = int.Parse(clsArray.FEID[i]);
                        clsTA.ClassBatchNo = clsArray.BatchNo[i];
                        clsTA.ClassSIMSerialNo = clsArray.SerialNo[i];
                        clsTA.ClassTerminalSN = clsArray.TerminalSN[i];
                        clsTA.ClassIRNo = clsArray.IRNo[i];
                        clsTA.ClassIRDate = clsArray.IRDate[i];
                        clsTA.ClassInstallationDate = clsArray.InstallationDate[i];
                        clsTA.ClassClientName = clsArray.ClientName[i];
                        clsTA.ClassMerchantName = clsArray.MerchantName[i];
                        clsTA.ClassFEName = clsArray.FEName[i];
                        clsTA.ClassServiceProviderName = clsArray.ServiceProviderName[i];
                        clsTA.ClassTADateTime = clsArray.TADateTime[i];
                        clsTA.ClassCarrier = clsArray.SIMCarrier[i];
                        clsTA.ClassAssignedTo = clsArray.AssignedTo[i];
                        clsTA.ClassRemarks = clsArray.Remarks[i];
                        clsTA.ClassDeliveryDate = clsArray.DeliveryDate[i];
                        clsTA.ClassReceivedDate = clsArray.ReceiveDate[i];
                        clsTA.ClassSIMStatus = int.Parse(clsArray.SIMStatus[i]);
                        clsTA.ClassSIMStatusDescription = clsArray.SIMStatusDescription[i];
                        

                        i++;

                        //dbFunction.UpdateProgressBar(pbLoading, lblSearchStatus, ii);

                        AddItem(i);

                        dbFunction.AppDoEvents(true);
                    }
                }

                dbFunction.ListViewAlternateBackColor(lvwSearch);
                lblSearchStatus.Text = lvwSearch.Items.Count.ToString() + " " + "record(s) found.";
                
                btnPreview.Enabled = true;
            }
            else
            {                
                lblSearchStatus.Text = "No record found...";
            }

            dbFunction.GetResponseTime("Find SIM With TA");            
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

        private void AddItem(int inLineNo)
        {
            // Add to List            
            ListViewItem item = new ListViewItem(inLineNo.ToString());
            
            item.SubItems.Add(clsTA.ClassSIMID.ToString());
            item.SubItems.Add(clsTA.ClassFEID.ToString());
            item.SubItems.Add(clsTA.ClassSIMSerialNo);
            item.SubItems.Add(clsTA.ClassTerminalSN);
            item.SubItems.Add(clsTA.ClassIRNo);
            item.SubItems.Add(clsTA.ClassIRDate);
            item.SubItems.Add(clsTA.ClassInstallationDate);
            item.SubItems.Add(clsTA.ClassClientName);
            item.SubItems.Add(clsTA.ClassMerchantName);
            item.SubItems.Add(clsTA.ClassFEName);
            item.SubItems.Add(clsTA.ClassServiceProviderName);
            item.SubItems.Add(clsTA.ClassTADateTime);
            item.SubItems.Add(clsTA.ClassCarrier);
            item.SubItems.Add(clsTA.ClassAssignedTo);
            item.SubItems.Add(clsTA.ClassRemarks);
            item.SubItems.Add(clsTA.ClassDeliveryDate);
            item.SubItems.Add(clsTA.ClassReceivedDate);
            item.SubItems.Add(clsTA.ClassSIMStatus.ToString());
            item.SubItems.Add(clsTA.ClassSIMStatusDescription);

            lvwSearch.Items.Add(item);
        }

        private void btnSearchOption_Click(object sender, EventArgs e)
        {
            //if (!dbAPI.isCheckTable()) return;

            Cursor.Current = Cursors.WaitCursor;

            if (fAllowSelect)
            {
                frmPrintOptionCriteria.sDefaultIRStatus = "";
                frmPrintOptionCriteria.sDefaultServiceType = "";
                frmPrintOptionCriteria.sDefaultTerminalStatus = "";
                frmPrintOptionCriteria.sDefaultSIMStatus = clsGlobalVariables.STATUS_AVAILABLE_DESC;
            }
            
            frmPrintOptionCriteria.sHeader = "FIND SIM";
            frmPrintOptionCriteria frm = new frmPrintOptionCriteria();
            frm.ShowDialog();

            if (frmPrintOptionCriteria.fSelected)
            {
                lvwSearch.Items.Clear();                
                btnSearchOption.Enabled = false;
                lblSearchOption.Text = "(-)Search Options";
                lblSearchOption_Click(this, e);
                
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
                LoadSIMWithAllocation("View", "", "");                
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

        private void lvwSearch_DoubleClick(object sender, EventArgs e)
        {
            SelectedItem();
        }
        private bool fSelectedConfirm()
        {
            bool fAddItem = true;

            if (MessageBox.Show("Please confirm selected details below:" +
                                "\n\n" +
                                "SIM STATUS DETAILS" + "\n" +
                                "Status.: " + clsSearch.ClassSIMStatusDescription + "\n" +
                                "\n" +
                                "SIM DETAILS" + "\n" +
                                "Serial No.: " + clsSearch.ClassSIMSerialNo + "\n" +
                                "Carrier: " + clsSearch.ClassSIMCarrier + "\n" +                                
                                "\n\n",
                                "Confirm?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fAddItem = false;
            }

            return fAddItem;

        }

        private void lvwSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwSearch.SelectedItems.Count > 0)
            {
                string LineNo = lvwSearch.SelectedItems[0].Text;
                txtLineNo.Text = LineNo;

                if (LineNo.Length > 0)
                {                    
                    clsSearch.ClassSIMID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                    clsSearch.ClassFEID = int.Parse(lvwSearch.SelectedItems[0].SubItems[2].Text);
                    clsSearch.ClassSIMSerialNo = lvwSearch.SelectedItems[0].SubItems[3].Text;                    
                    clsSearch.ClassFEName = lvwSearch.SelectedItems[0].SubItems[5].Text;
                    clsSearch.ClassSIMRemarks = lvwSearch.SelectedItems[0].SubItems[6].Text;
                    clsSearch.ClassDeliveryDate = lvwSearch.SelectedItems[0].SubItems[7].Text;
                    clsSearch.ClassReceivedDate = lvwSearch.SelectedItems[0].SubItems[8].Text;
                    clsSearch.ClassSIMCarrier = lvwSearch.SelectedItems[0].SubItems[13].Text;
                    clsSearch.ClassSIMStatus = int.Parse(lvwSearch.SelectedItems[0].SubItems[18].Text);
                    clsSearch.ClassSIMStatusDescription = lvwSearch.SelectedItems[0].SubItems[19].Text;
                    
                }
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

        private void frmFindSIM_KeyDown(object sender, KeyEventArgs e)
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
                clsSearch.ClassReportID = 3;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Advance SIM";
                clsSearch.ClassSearchValue = sSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewAdvanceTA";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        private void SelectedItem()
        {
            if (!fAllowSelect) return;

            if (fConfirm)
            {
                if (!fSelectedConfirm()) return;
            }
            
            fSelected = true;

            this.Close();
        }

        private void lvwSearch_Click(object sender, EventArgs e)
        {
            
            
            /*
            if (dbFunction.isValidID(txtLineNo.Text))
            {
                string sMessage = clsFunction.sLineSeparator + Environment.NewLine +
                                    dbFunction.GetListViewSelectedRow(lvwSearch, int.Parse(txtLineNo.Text)) +
                                    clsFunction.sLineSeparator;

                dbFunction.ShowToolTip(lvwSearch, txtLineNo.Text, sTitle, sMessage);
            }
            */
        }

        private void InitPage(int iCurrentPage, int iTotalPage)
        {
            if (iTotalPage > 0)
            {
                // do nothing...
            }
            else
            {
                iCurrentPage = 0;
            }

            clsSearch.ClassCurrentPage = iCurrentPage;
            clsSearch.ClassTotalPage = iTotalPage;

            txtPage.Text = iCurrentPage.ToString() + " / " + iTotalPage.ToString();
        }
        private int GetTotalPage()
        {
            int iCount = 0;
            int totalPage = 0;
            int iLimitSize = dbFunction.GetPageLimit();

            Debug.WriteLine("GetTotalPage::iLimitSize=" + iLimitSize);

            iCount = 0;
            clsSearch.ClassHoldAdvanceSearchValue = clsSearch.ClassServiceTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassParticularID + clsFunction.sPipe +
                                                clsSearch.ClassTerminalTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalModelID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalBrandID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalSN + clsFunction.sPipe +
                                                clsSearch.ClassTerminalStatusType + clsFunction.sPipe +
                                                clsSearch.ClassSIMID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassSIMSerialNo.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassSIMStatus + clsFunction.sPipe +
                                                clsSearch.ClassSIMCarrier;

            Debug.WriteLine("GetTotalPage::clsSearch.ClassHoldAdvanceSearchValue=" + clsSearch.ClassHoldAdvanceSearchValue);

            dbAPI.GetViewCount("Search", "Advance SIM", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");

            if (dbAPI.isNoRecordFound() == false)
                iCount = clsTerminal.ClassTerminalCount;

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
        private void ProcessPage()
        {
            InitPage(int.Parse(clsFunction.sOne), GetTotalPage());
        }

        private void btnFirstPage_Click(object sender, EventArgs e)
        {
            ResetSearchField();
            clsSearch.ClassCurrentPage = int.Parse(clsFunction.sOne);
            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadSIMWithAllocation("View", "", "");
        }

        private void btnLastPage_Click(object sender, EventArgs e)
        {
            ResetSearchField();
            clsSearch.ClassCurrentPage = clsSearch.ClassTotalPage;
            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadSIMWithAllocation("View", "", "");
        }

        private void btnPreviousPage_Click(object sender, EventArgs e)
        {
            ResetSearchField();
            if (clsSearch.ClassCurrentPage > int.Parse(clsFunction.sOne))
                clsSearch.ClassCurrentPage--;

            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadSIMWithAllocation("View", "", "");
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            ResetSearchField();
            if (clsSearch.ClassCurrentPage < clsSearch.ClassTotalPage)
                clsSearch.ClassCurrentPage++;

            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadSIMWithAllocation("View", "", "");
        }
        private void ResetSearchField()
        {
            clsSearch.ClassTerminalSN = clsFunction.sZero;
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
