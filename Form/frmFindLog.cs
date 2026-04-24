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
    public partial class frmFindLog : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;

        // The column we are currently using for sorting.
        private ColumnHeader SortingColumn = null;

        public frmFindLog()
        {
            InitializeComponent();
        }

        private void LoadUserLog(string StatementType, string SearchBy, string SearchValue)
        {
            int i = 0;
            int ii = 0;
            int iLineNo = 0;
            
            lblSearchStatus.Text = "";
            lvwSearch.Items.Clear();

            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassUserID + clsFunction.sPipe +
                                                clsSearch.ClassUserType + clsFunction.sPipe +
                                                clsSearch.ClassLogDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassLogDateTo + clsFunction.sPipe +
                                                clsSearch.ClassLogSessionStatus.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                clsFunction.iLimit;

            Debug.WriteLine("LoadUserLog ::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbFunction.GetRequestTime("Find Log");

            dbAPI.ExecuteAPI("GET", "View", "User Log", clsSearch.ClassAdvanceSearchValue, "User", "", "ViewUser");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.LogID.Length > i)
                {
                    ii++;
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.LogID[i].ToString());
                    item.SubItems.Add(clsArray.UserID[i].ToString());
                    item.SubItems.Add(clsArray.UserName[i].ToString());
                    item.SubItems.Add(clsArray.FullName[i].ToString());
                    item.SubItems.Add(clsArray.UserType[i].ToString());
                    item.SubItems.Add(clsArray.ComputerIP[i].ToString());
                    item.SubItems.Add(clsArray.ComputerName[i].ToString());
                    item.SubItems.Add(clsArray.LogInDate[i].ToString());
                    item.SubItems.Add(clsArray.LogInTime[i].ToString());
                    item.SubItems.Add(clsArray.LogOutDate[i].ToString());
                    item.SubItems.Add(clsArray.LogOutTime[i].ToString());
                    item.SubItems.Add(clsArray.SessionStatus[i].ToString());
                    item.SubItems.Add(clsArray.SessionStatusDescription[i].ToString());
                    item.SubItems.Add(clsArray.LogPublishVersion[i].ToString());

                    lvwSearch.Items.Add(item);                    

                    i++;

                    dbFunction.AppDoEvents(true);

                }

                dbFunction.ListViewAlternateBackColor(lvwSearch);
                lblSearchStatus.Text = lvwSearch.Items.Count.ToString() + " " + "record(s) found.";
            }
            else
            {
                //dbFunction.SetMessageBox("No record found.", "Find IR", clsFunction.IconType.iExclamation);
                lblSearchStatus.Text = "No record found...";
            }

            dbFunction.GetResponseTime("Find Log");            
        }

        private void frmFindLog_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            //frmLoading frmWait = new frmLoading(); // Open Wait Window
            //clsFunction.WaitWindow(true, frmWait);

            InitPage(0, 0);

            ProcessPage();
            LoadUserLog("View", "", "");

            //clsFunction.WaitWindow(false, frmWait); // Close Wait Window
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmFindLog_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void btnSearchClear_Click(object sender, EventArgs e)
        {
            InitPage(0, 0);
            dbFunction.ClearListView(lvwSearch);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //frmLoading frmWait = new frmLoading(); // Open Wait Window
            //clsFunction.WaitWindow(true, frmWait);

            LoadUserLog("View", "", "");

            //clsFunction.WaitWindow(false, frmWait); // Close Wait Window
        }

        private void btnSearchOption_Click(object sender, EventArgs e)
        {

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
            int iLimitSize = clsFunction.iLimit;

            lblTotalCount.Text = clsFunction.sNull;
            Debug.WriteLine("GetTotalPage::iLimitSize=" + iLimitSize);

            dbAPI.GetViewCount("Search", "User Log", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
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

        private void btnFirstPage_Click(object sender, EventArgs e)
        {
            clsSearch.ClassCurrentPage = int.Parse(clsFunction.sOne);
            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadUserLog("View", "", "");
        }

        private void btnLastPage_Click(object sender, EventArgs e)
        {
            clsSearch.ClassCurrentPage = clsSearch.ClassTotalPage;
            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadUserLog("View", "", "");
        }

        private void btnPreviousPage_Click(object sender, EventArgs e)
        {
            if (clsSearch.ClassCurrentPage > int.Parse(clsFunction.sOne))
                clsSearch.ClassCurrentPage--;

            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadUserLog("View", "", "");
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (clsSearch.ClassCurrentPage < clsSearch.ClassTotalPage)
                clsSearch.ClassCurrentPage++;

            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadUserLog("View", "", "");
        }
        private void ProcessPage()
        {
            Debug.WriteLine("--ProcessPage--");

            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassUserID + clsFunction.sPipe +
                                                clsSearch.ClassUserType + clsFunction.sPipe +
                                                clsSearch.ClassLogDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassLogDateTo + clsFunction.sPipe +
                                                clsSearch.ClassLogSessionStatus.ToString();
            
            InitPage(int.Parse(clsFunction.sOne), GetTotalPage());
        }
    }
}
