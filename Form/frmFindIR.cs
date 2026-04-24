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
    public partial class frmFindIR : Form
    {
        public static bool fAutoLoadData = false;
        public static bool fAllowSelect = false;
        public bool fSelected = false;
        public static bool fConfirm = false;
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

        // The column we are currently using for sorting.
        private ColumnHeader SortingColumn = null;

        public frmFindIR()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            fSelected = false;
            this.Close();
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

        private void LoadIR(string StatementType, string SearchBy, string SearchValue)
        {
            int i = 0;
            int ii = 0;
            int iLineNo = 0;

            lblSearchStatus.Text = "";
            lvwSearch.Items.Clear();

            clsSearch.ClassCurrentPage = clsSearch.ClassCurrentPage;
            clsSearch.ClassTotalPage = dbFunction.GetPageLimit();

            ComposeSearchValue(); // Set clsSearch.ClassSearchValue
            
            dbFunction.GetRequestTime("Find IR");

            dbAPI.ExecuteAPI("GET", "View", "Advance IR", clsSearch.ClassSearchValue, "IR", "", "ViewAdvanceIR");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.IRIDNo.Length > i)
                {
                    ii++;
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.IRIDNo[i].ToString());
                    item.SubItems.Add(clsArray.IRNo[i].ToString());
                    item.SubItems.Add(clsArray.IRDate[i].ToString());
                    item.SubItems.Add(clsArray.InstallationDate[i].ToString());
                    item.SubItems.Add(clsArray.IRImportDateTime[i].ToString());
                    item.SubItems.Add(clsArray.ParticularName[i].ToString());
                    item.SubItems.Add(clsArray.TID[i].ToString());
                    item.SubItems.Add(clsArray.MID[i].ToString());
                    item.SubItems.Add(clsArray.Address[i].ToString());
                    item.SubItems.Add(clsArray.City[i].ToString());
                    item.SubItems.Add(clsArray.Province[i].ToString());
                    item.SubItems.Add(clsArray.ContactPerson[i].ToString());
                    item.SubItems.Add(clsArray.TelNo[i].ToString());
                    item.SubItems.Add(clsArray.ServiceTypeDescription[i].ToString());
                    item.SubItems.Add(clsArray.IRStatusDescription[i].ToString());
                    item.SubItems.Add(clsArray.IRStatus[i].ToString());
                    item.SubItems.Add(clsArray.RegionID[i].ToString());
                    item.SubItems.Add(clsArray.TerminalSN[i].ToString());
                    item.SubItems.Add(clsArray.SIMSerialNo[i].ToString());
                    item.SubItems.Add(clsArray.DockSN[i].ToString());
                    item.SubItems.Add(clsArray.ClientID[i].ToString());
                    item.SubItems.Add(clsArray.ClientName[i].ToString());

                    lvwSearch.Items.Add(item);
                    
                    if (int.Parse(clsArray.IRStatus[i]) == clsGlobalVariables.STATUS_AVAILABLE)
                        clsIR.ClassIRStatusDescription = "-";

                    i++;

                    dbFunction.AppDoEvents(true);

                    dbFunction.UpdateProgressBar(pbLoading, lblSearchStatus, ii);                    
                }

                dbFunction.ListViewAlternateBackColor(lvwSearch);
                lblSearchStatus.Text = lvwSearch.Items.Count.ToString() + " " + "record(s) found.";
                
                btnPreview.Enabled = true;
            }
            else
            {
                //dbFunction.SetMessageBox("No record found.", "Find IR", clsFunction.IconType.iExclamation);
                lblSearchStatus.Text = "No record found...";
            }

            dbFunction.GetResponseTime("Find IR");            
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
       
        private void cboSearchMerchant_SelectedIndexChanged(object sender, EventArgs e)
        {

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
                    clsSearch.ClassIRIDNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                    clsSearch.ClassIRNo = lvwSearch.SelectedItems[0].SubItems[2].Text;
                    clsSearch.ClassIRRequestDate = lvwSearch.SelectedItems[0].SubItems[3].Text;
                    clsSearch.ClassIRInstallationDate = lvwSearch.SelectedItems[0].SubItems[4].Text;
                    clsSearch.ClassIRImportDateTime = lvwSearch.SelectedItems[0].SubItems[5].Text;
                    clsSearch.ClassParticularName = lvwSearch.SelectedItems[0].SubItems[6].Text;
                    clsSearch.ClassMerchantName = lvwSearch.SelectedItems[0].SubItems[6].Text;
                    clsSearch.ClassTID = lvwSearch.SelectedItems[0].SubItems[7].Text;
                    clsSearch.ClassMID = lvwSearch.SelectedItems[0].SubItems[8].Text;
                    clsSearch.ClassIRStatusDescription = lvwSearch.SelectedItems[0].SubItems[15].Text;
                    clsSearch.ClassIRStatus = int.Parse(lvwSearch.SelectedItems[0].SubItems[16].Text);
                    clsSearch.ClassRegionID = int.Parse(lvwSearch.SelectedItems[0].SubItems[17].Text);
                    clsSearch.ClassTerminalSN = lvwSearch.SelectedItems[0].SubItems[18].Text;
                    clsSearch.ClassSIMSerialNo = lvwSearch.SelectedItems[0].SubItems[19].Text;
                    clsSearch.ClassDockSN = lvwSearch.SelectedItems[0].SubItems[20].Text;
                    clsSearch.ClassClientID = int.Parse(lvwSearch.SelectedItems[0].SubItems[21].Text);
                    clsSearch.ClassClientName = lvwSearch.SelectedItems[0].SubItems[22].Text;
                }
            }
        }
        private bool fSelectedConfirm()
        {
            bool fAddItem = true;

            if (MessageBox.Show("Please confirm selected details below:" +
                                "\n\n" +
                                "[SERVICE DETAILS]" + "\n" +
                                "Status.: " + clsSearch.ClassIRStatusDescription + "\n" +
                                "\n" +
                                "[INSTALLATION REQUEST DETAILS]" + "\n" +
                                "IR ID.: " + clsSearch.ClassIRNo + "\n" +
                                "Request Date: " + clsSearch.ClassIRRequestDate + "\n" +
                                "Installation Date: " + clsSearch.ClassIRInstallationDate + "\n" +
                                "\n" +
                                "[MERCHANT DETAILS]" + "\n" +
                                "Merchant: " + clsSearch.ClassParticularName + "\n" +
                                "TID: " + clsSearch.ClassTID + "\n" +
                                "MID: " + clsSearch.ClassMID + "\n" +
                                "\n\n",
                                "Confirm?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fAddItem = false;
            }

            return fAddItem;

        }

        private void lvwSearch_DoubleClick(object sender, EventArgs e)
        {
            SelectedItem();
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

            }
            
            frmPrintOptionCriteria.sHeader = "FIND IR";
            frmPrintOptionCriteria.iSearchType = frmPrintOptionCriteria.SearchType.iIR;
            frmPrintOptionCriteria frm = new frmPrintOptionCriteria();
            frm.ShowDialog();

            if (frmPrintOptionCriteria.fSelected)
            {
                lvwSearch.Items.Clear();                
                btnSearchOption.Enabled = false;
                lblSearchOption.Text = "(-)Search Options";
                lblSearchOption_Click(this, e);
                
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
                LoadIR("Search", "IR", "");                
                Cursor.Current = Cursors.Default;  // Back to normal 

            }
            else
            {
                btnSearchOption.Enabled = true;
            }

            Cursor.Current = Cursors.Default;
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

        private void btnPreview_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {                
                clsSearch.ClassReportID = 2;
                clsSearch.ClassSearchBy = "Advance IR";
                clsSearch.ClassCurrentPage = int.Parse(clsFunction.sZero);
                clsSearch.ClassTotalPage = int.Parse(clsFunction.sZero);

                ComposeSearchValue(); // Set clsSearch.ClassSearchValue

                clsSearch.ClassStoredProcedureName = "spViewAdvanceIR";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        private void btnExport_Click(object sender, EventArgs e)
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

            this.Close();
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
                                                clsSearch.ClassIRStatus.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassIRNo + clsFunction.sPipe +
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
                                                clsSearch.ClassIRImportDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassIRImportDateTo + clsFunction.sPipe +
                                                clsSearch.ClassSIMSerialNo + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsSearch.ClassClientID + clsFunction.sPipe +
                                                clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                clsSearch.ClassTotalPage + clsFunction.sPipe +
                                                clsSearch.ClassRegionID + clsFunction.sPipe + // Region
                                                clsSearch.ClassProvinceID + clsFunction.sPipe +  // Province
                                                clsFunction.sZero;

            Debug.WriteLine("ComposeSearchValue, clsSearch.ClassSearchValue=" + clsSearch.ClassSearchValue);
        }

        private void frmFindIR_KeyDown(object sender, KeyEventArgs e)
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

        private void frmFindIR_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("--frmFindIR_Load--");

            dbAPI = new clsAPI();
            dbSetting = new clsINI();
            dbFunction = new clsFunction();
            dbSetting.InitDatabaseSetting();

            lblSearchStatus.Text = "";
            ClearDataGrid();
            btnSearchOption.Focus();
            btnPreview.Enabled = false;
            lblHeader.Text = lblHeader.Text + " " + "[ " + sHeader + " ]";
            
            if (fAutoLoadData)
            {
                LoadIR("View", "", "");
            }            
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
