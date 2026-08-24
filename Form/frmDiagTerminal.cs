using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIS
{
    public partial class frmDiagTerminal : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;

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
        public frmDiagTerminal()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwList, true);
            dbFunction.setDoubleBuffer(lvwListLocationIssues, true);
            dbFunction.setDoubleBuffer(lvwListStatusIssues, true);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void loadTerminalInventorySummaryKpiCardinitKpiCard(string pStatus)
        {
            int i = 0;
            string pSearchValue = dbFunction.CheckAndSetNumericValue(pStatus);
            int TTotalInventory = 0;
            int TStatusMismatch = 0;
            int TLocationMismatch = 0;
            int TStatusLocationMismatch = 0;
            
            clearKpiCard();

            dbAPI.ExecuteAPI("GET", "View", "Terminal Inventory Diagnostic Summary", pSearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.ID.Length > i)
                {
                    int.TryParse(
                        dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "TotalInventory"),
                        out TTotalInventory
                    );

                    int.TryParse(
                        dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "StatusMismatch"),
                        out TStatusMismatch
                    );

                    int.TryParse(
                        dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "LocationMismatch"),
                        out TLocationMismatch
                    );

                    int.TryParse(
                        dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "StatusLocationMismatch"),
                        out TStatusLocationMismatch
                    );

                    i++;
                }
            }

            ucKpiCardTotalInventory.Title = "TOTAL INVENTORY";
            ucKpiCardTotalInventory.Value = $"{TTotalInventory}";
            ucKpiCardTotalInventory.Description = "All active POS";

            double statusMismatchPercent =TTotalInventory > 0 ? (TStatusMismatch * 100.0) / TTotalInventory : 0;

            ucKpiCardTotalStatusMismatch.Title = "STATUS MISMATCH";
            ucKpiCardTotalStatusMismatch.Value = $"{TStatusMismatch:N0}";
            ucKpiCardTotalStatusMismatch.Description = $"{statusMismatchPercent:F2}% of inventory";


            double locationMismatchPercent = TTotalInventory > 0 ? (TLocationMismatch * 100.0) / TTotalInventory : 0;

            ucKpiCardTotalLocationMismatch.Title = "LOCATION MISMATCH";
            ucKpiCardTotalLocationMismatch.Value = $"{TLocationMismatch:N0}";
            ucKpiCardTotalLocationMismatch.Description = $"{locationMismatchPercent:F2}% of inventory";

            double statusLocationMismatchPercent = TTotalInventory > 0 ? (TStatusLocationMismatch * 100.0) / TTotalInventory: 0;

            ucKpiCardTotalStatusLocationMismatch.Title = "STATUS/LOCATION MISMATCH";
            ucKpiCardTotalStatusLocationMismatch.Value = $"{TStatusLocationMismatch:N0}";
            ucKpiCardTotalStatusLocationMismatch.Description = $"{statusLocationMismatchPercent:F2}% of inventory";

        }

        private void frmInventoryDiagnostic_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            lblHeader.Text = $"TERMINAL INVENTORY DIAGNOSTIC [ {clsSearch.ClassBankDisplayName} | {clsSystemSetting.ClassSystemEnvironment} ]";

            // Fill ComboBox
            dbAPI.FillComboBoxClient(cboClient);
            dbAPI.FillComboBoxTerminalType(cboType);            
            dbAPI.FillComboBoxLocation(cboLocation);

            initServiceDiagnosticDeailListView(lvwList);
            initServiceDiagnosticDeailListView(lvwListLocationIssues);
            initServiceDiagnosticDeailListView(lvwListStatusIssues);

            cboClient.SelectedIndex = 0;
            cboType.SelectedIndex = 0;
            cboItemCategory.SelectedIndex = 0;
            cboLocation.SelectedIndex = 0;

            Cursor.Current = Cursors.Default;
        }

        public void initServiceDiagnosticDeailListView(ListView lvw)
        {
            string outField = "";
            int outWidth = 0;
            string outTitle = "";
            HorizontalAlignment outAlign = 0;
            bool outVisible = false;
            bool outAutoWidth = false;
            string outFormat = "";

            dbFunction = new clsFunction();

            lvw.Clear();
            lvw.View = View.Details;

            dbFunction.GetListViewHeaderColumnFromFile("", "Line#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "TerminalID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Type", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Model", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "TerminalSN", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "IssueCategory", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Location", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Status", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "IssueDescription", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "RecommendedAction", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

        }

        public void loadTerminalInventoryDiagnosticDetail(ListView lvw, string pStatus)
        {
            int i = 0;
            int iLineNo = 0;

            Debug.WriteLine("--loadTerminalInventoryDiagnosticDetail--");

            dbFunction = new clsFunction();

            lvw.Enabled = true;
            lvw.Items.Clear();

            string pSearchValue = pStatus;

            Debug.WriteLine("pSearchValue=" + pSearchValue);

            dbAPI.ExecuteAPI("GET", "View", "Terminal Inventory Diagnostic Detail", pSearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            int issueCategoryColumnIndex = dbFunction.GetListViewColumnIndex(lvwList, "ISSUE CATEGORY");

            if (dbAPI.isNoRecordFound() == false)
            {
                lvw.Items.Clear();
                while (clsArray.ID.Length > i)
                {
                    string issueCategory = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "IssueCategory");

                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "TerminalID"));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "TerminalType"));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "TerminalModel"));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "TerminalSN"));

                    item.UseItemStyleForSubItems = false;

                    item.SubItems.Add(issueCategory);

                    // Get the index AFTER adding TAT STATUS
                    issueCategoryColumnIndex = item.SubItems.Count - 1;

                    // Issue Category - ForeColor
                    switch (issueCategory)
                    {
                        case "STATUS MISMATCH":
                            item.SubItems[issueCategoryColumnIndex].ForeColor = Color.Red;
                            break;

                        case "LOCATION MISMATCH":
                            item.SubItems[issueCategoryColumnIndex].ForeColor = Color.DarkOrange;
                            break;

                        case "STATUS & LOCATION MISMATCH":
                            item.SubItems[issueCategoryColumnIndex].ForeColor = Color.DarkRed;
                            break;

                        default:
                            item.SubItems[issueCategoryColumnIndex].ForeColor = Color.Black;
                            break;
                    }

                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "Location"));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "TerminalStatus"));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "IssueDescription"));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "RecommendedAction"));
                    
                    lvw.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(lvw);
            }
        }

        private void cboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cboType.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Terminal Type", cboType.Text);
                clsSearch.ClassTerminalTypeID = clsSearch.ClassOutFileID;

                dbAPI.FillComboBoxTerminalModelByTerminalType(cboModel, clsSearch.ClassTerminalTypeID.ToString());
            }
        }

        private void clearKpiCard()
        {
            ucKpiCardTotalInventory.Clear();
            ucKpiCardTotalStatusMismatch.Clear();
            ucKpiCardTotalLocationMismatch.Clear();
            ucKpiCardTotalStatusLocationMismatch.Clear();            

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            string clientID = $"{clsSearch.ClassClientID}";
            string itemCategory = cboItemCategory.Text;

            if (!dbFunction.isValidDescriptionEntry(cboClient.Text, "Client" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            loadTerminalInventorySummaryKpiCardinitKpiCard(clientID);

            if (itemCategory.Equals(clsDefines.NOT_SPECIFIED, StringComparison.OrdinalIgnoreCase))
            {
                // Main list - ALL
                string pSearchValue = $"{clientID}{clsDefines.gPipe}{clsDefines.NOT_SPECIFIED}";
                loadTerminalInventoryDiagnosticDetail(lvwList, pSearchValue);

                // Location Issues
                string pLocationSearchValue =
                    $"{clientID}{clsDefines.gPipe}LOCATION MISMATCH";

                loadTerminalInventoryDiagnosticDetail(
                    lvwListLocationIssues,
                    pLocationSearchValue
                );

                // Status Issues
                string pStatusSearchValue =
                    $"{clientID}{clsDefines.gPipe}STATUS MISMATCH";

                loadTerminalInventoryDiagnosticDetail(
                    lvwListStatusIssues,
                    pStatusSearchValue
                );
            }
            else
            {
                // Selected category
                string pSearchValue =
                    $"{clientID}{clsDefines.gPipe}{itemCategory}";

                loadTerminalInventoryDiagnosticDetail(
                    lvwList,
                    pSearchValue
                );

                // Optional: clear the dedicated lists
                lvwListLocationIssues.Items.Clear();
                lvwListStatusIssues.Items.Clear();
            }

            Cursor.Current = Cursors.Default;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dbFunction.ClearListViewItems(lvwList);
            dbFunction.ClearListViewItems(lvwListLocationIssues);
            dbFunction.ClearListViewItems(lvwListStatusIssues);

            clearKpiCard();

            cboClient.SelectedIndex = 0;
            cboType.SelectedIndex = 0;
            cboItemCategory.SelectedIndex = 0;
            cboLocation.SelectedIndex = 0;
        }

        private void cboClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassClientID = 0;
            if (!cboClient.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Client List", cboClient.Text);
                clsSearch.ClassClientID = clsSearch.ClassOutFileID;
                Debug.WriteLine("clsSearch.ClassClientID=" + clsSearch.ClassClientID);
            }
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.Items.Count > 0)
            {
                string pSelectedRow = dbFunction.GetListViewSelectedRow(lvwList, 0);
                string jsonResult = dbFunction.genJSONFormat(lvwList, lvwList.SelectedIndices[0], "", "");

                // Pass JSON to popup window
                frmPopUpInfo frm = new frmPopUpInfo(jsonResult);
                frm.ShowDialog();
            }
        }

        private void lvwListLocationIssues_DoubleClick(object sender, EventArgs e)
        {
            if (lvwListLocationIssues.Items.Count > 0)
            {
                string pSelectedRow = dbFunction.GetListViewSelectedRow(lvwListLocationIssues, 0);
                string jsonResult = dbFunction.genJSONFormat(lvwListLocationIssues, lvwListLocationIssues.SelectedIndices[0], "", "");

                // Pass JSON to popup window
                frmPopUpInfo frm = new frmPopUpInfo(jsonResult);
                frm.ShowDialog();
            }
        }

        private void lvwListStatusIssues_DoubleClick(object sender, EventArgs e)
        {
            if (lvwListStatusIssues.Items.Count > 0)
            {
                string pSelectedRow = dbFunction.GetListViewSelectedRow(lvwListStatusIssues, 0);
                string jsonResult = dbFunction.genJSONFormat(lvwListStatusIssues, lvwListStatusIssues.SelectedIndices[0], "", "");

                // Pass JSON to popup window
                frmPopUpInfo frm = new frmPopUpInfo(jsonResult);
                frm.ShowDialog();
            }
        }

        private void frmDiagTerminal_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void cboModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassTerminalModelID = 0;
            if (!cboType.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Terminal Model", cboModel.Text);
                clsSearch.ClassTerminalModelID = clsSearch.ClassOutFileID;                
            }
        }

        private void cboLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassLocationID = 0;
            if (!cboLocation.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Location", cboLocation.Text);
                clsSearch.ClassLocationID = clsSearch.ClassOutFileID;
            }
        }
    }
}
