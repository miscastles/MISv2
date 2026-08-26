using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIS
{
    public partial class frmDiagService : Form
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

        public frmDiagService()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwList, true);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void initKpiCard(string pStatus)
        {
            int i = 0;
            string pSearchValue = dbFunction.CheckAndSetNumericValue(pStatus);
            int TTotalJobs = 0;
            int TBeyondTAT = 0;
            int TWithinTAT = 0;
            int TOver5Days = 0;
            int TOver10Days = 0;
            int TOver30Days = 0;
            int TOver90Days = 0;
            int TOver180Days = 0;

            clearKpiCard();

            dbAPI.ExecuteAPI("GET", "View", "Service Diagnostic Summary", pSearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {            
                while (clsArray.ID.Length > i)
                {
                    int.TryParse(
                        dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "TotalJobs"),
                        out TTotalJobs
                    );

                    int.TryParse(
                        dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "BeyondTAT"),
                        out TBeyondTAT
                    );

                    int.TryParse(
                        dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "WithinTAT"),
                        out TWithinTAT
                    );

                    int.TryParse(
                        dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "Over5Days"),
                        out TOver5Days
                    );

                    int.TryParse(
                        dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "Over10Days"),
                        out TOver10Days
                    );

                    int.TryParse(
                        dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "Over30Days"),
                        out TOver30Days
                    );

                    int.TryParse(
                        dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "Over90Days"),
                        out TOver90Days
                    );

                    int.TryParse(
                        dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "Over180Days"),
                        out TOver180Days
                    );

                    i++;
                }                
            }

            ucKpiCardTotalJobs.Title = "TOTAL JOBS";
            ucKpiCardTotalJobs.Value = $"{TTotalJobs}";
            ucKpiCardTotalJobs.Description = "All Processing Jobs";
            
            // Beyond
            double beyondTATPercent = TTotalJobs > 0 ? (TBeyondTAT * 100.0) / TTotalJobs : 0;
            ucKpiCardBeyondTAT.Title = "BEYOND TAT";
            ucKpiCardBeyondTAT.Value = $"{TBeyondTAT}";
            ucKpiCardBeyondTAT.Description = $"{beyondTATPercent:F2}% of total";

            // Within
            double withinTATPercent = TTotalJobs > 0 ? (TWithinTAT * 100.0) / TTotalJobs : 0;
            ucKpiCardWithinTAT.Title = "WITHIN TAT";
            ucKpiCardWithinTAT.Value = $"{TWithinTAT}";
            ucKpiCardWithinTAT.Description = $"{withinTATPercent:F2}% of total";

            ucKpiCard5Days.Title = "> 5 DAYS";
            ucKpiCard5Days.Value = $"{TOver5Days}";
            ucKpiCard5Days.Description = "Needs follow-up";

            ucKpiCard10Days.Title = "> 10 DAYS";
            ucKpiCard10Days.Value = $"{TOver10Days}";
            ucKpiCard10Days.Description = "Requires attention";

            ucKpiCard30Days.Title = "> 30 DAYS";
            ucKpiCard30Days.Value = $"{TOver30Days}";
            ucKpiCard30Days.Description = "Need attention";

            ucKpiCard90Days.Title = "> 90 DAYS";
            ucKpiCard90Days.Value = $"{TOver90Days}";
            ucKpiCard90Days.Description = "High priority";

            ucKpiCard180Days.Title = "> 180 DAYS";
            ucKpiCard180Days.Value = $"{TOver180Days}";
            ucKpiCard180Days.Description = "Critical priority";

        }

        private void frmServiceDiagnostic_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            initServiceDiagnosticDeailListView(lvwList);

            cboServiceStatus.SelectedIndex = 0;

            lblHeader.Text = $"SERVICE DIAGNOSTIC [ {clsSearch.ClassBankDisplayName} | {clsSystemSetting.ClassSystemEnvironment} ]";

            // Fill combbbox
            dbAPI.FillComboBoxClient(cboClient);
            cboClient.SelectedIndex = 0;

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

            dbFunction.GetListViewHeaderColumnFromFile("", "ServiceNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "MerchantID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Merchant", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Region", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Zone", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "TID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "MID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ServiceJobTypeDescription", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ServiceStatusDescription", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "DaysOverDue", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "TATStatus", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Request Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Schedule Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "FieldEngineer", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Dispatcher", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "HelpDesk", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvw.Columns.Add(outTitle, outWidth, outAlign);

            //dbFunction.GetListViewHeaderColumnFromFile("", "JobCount", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            //lvw.Columns.Add(outTitle, outWidth, outAlign);

        }

        public void loadServiceDiagnosticDetail(ListView lvw, string pStatus)
        {
            int i = 0;
            int iLineNo = 0;

            Debug.WriteLine("--loadServiceDiagnosticDetail--");

            dbFunction = new clsFunction();

            lvw.Enabled = true;
            lvw.Items.Clear();

            string pSearchValue = dbFunction.CheckAndSetNumericValue(pStatus);

            Debug.WriteLine("pSearchValue=" + pSearchValue);

            dbAPI.ExecuteAPI("GET", "View", "Service Diagnostic Detail", pSearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            int tatStatusColumnIndex = dbFunction.GetListViewColumnIndex(lvwList, "TAT STATUS");

            if (dbAPI.isNoRecordFound() == false)
            {
                lvw.Items.Clear();
                while (clsArray.ID.Length > i)
                {   
                    string tatStatus = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "TATStatus");

                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ServiceNo"));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "MerchantID"));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "MerchantName"));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "Region"));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "Zone"));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "TID"));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "MID"));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ServiceJobTypeDescription"));                    
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ServiceStatusDescription"));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "DaysOverDue"));

                    item.UseItemStyleForSubItems = false;
                    
                    // TAT Status
                    item.SubItems.Add(tatStatus);

                    // Get the index AFTER adding TAT STATUS
                    tatStatusColumnIndex = item.SubItems.Count - 1;
                    
                    // TAT Status - ForeColor
                    item.SubItems[tatStatusColumnIndex].ForeColor = (tatStatus.Equals(clsDefines.WITHIN_TAT) ? Color.Black : Color.Red);

                    //item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "TCount"));

                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "RequestDate"));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "ScheduleDate"));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "FEName"));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "Dispatcher"));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "HelpDesk"));

                    lvw.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(lvw);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidDescriptionEntry(cboClient.Text, "Client" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            Cursor.Current = Cursors.WaitCursor;

            initKpiCard(cboServiceStatus.Text);

            loadServiceDiagnosticDetail(lvwList, cboServiceStatus.Text);

            Cursor.Current = Cursors.Default;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clearKpiCard();

            dbFunction.ClearListViewItems(lvwList);

            cboServiceStatus.SelectedIndex = 0;
            cboClient.SelectedIndex = 0;

        }

        private void clearKpiCard()
        {
            ucKpiCardTotalJobs.Clear();
            ucKpiCardBeyondTAT.Clear();
            ucKpiCardWithinTAT.Clear();
            ucKpiCard5Days.Clear();
            ucKpiCard10Days.Clear();
            ucKpiCard30Days.Clear();
            ucKpiCard90Days.Clear();
            ucKpiCard180Days.Clear();

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

        private void frmDiagService_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }
    }
}
