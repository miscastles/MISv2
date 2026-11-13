using MIS.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIS
{
    public partial class frmSSerialNo : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        private clsFile dbFile;
        private modelSerialSearch dbSerialSearch;

        public frmSSerialNo()
        {
            InitializeComponent();
        }

        private void frmSGlobalSearch_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            dbFile = new clsFile();
            dbSerialSearch = new modelSerialSearch();

            cboSearchType.Items.Clear();
            cboSearchType.Items.AddRange(modelSerialSearch.TypeNames);
            if (cboSearchType.Items.Count > 0) cboSearchType.SelectedIndex = 0;
            
            dbFunction.FillSerialNoSearchAlgorithim(cboSearchAlgorithim);

            colCheckBox.DisplayIndex = 0;

            FillGridBankSelection(dgvBankList);
            InitSNHistoryList();

            chkboxCheckAll.Checked = true;
            chkBoxUncheckAll.Checked = false;

            lvwReturnList.FullRowSelect = true;
        }

        private void FillGridBankSelection(DataGridView dgv)
        {
            string filepath = dbFile.sSettingPath + clsDefines.RESP_BANKLIST_FILENAME;

            if (!dbFile.FileExist(filepath))
            {
                return;
            }

            dgv.Rows.Clear();

            foreach (clsBank bank in dbFunction.loadBankList(filepath))
            {
                if (bank.Enable != 1 || bank.DisplayName == clsFunction.sDefaultSelect) continue;

                DataGridViewRow row = dgv.Rows[dgv.Rows.Add(bank.DisplayName, true)];
                row.Tag = bank.Code;
                row.Cells[colBank.Index].Tag = bank.DisplayName;
            }
        }

        private List<DataGridViewRow> getCheckedBanks()
        {
            List<DataGridViewRow> rows = new List<DataGridViewRow>();

            foreach (DataGridViewRow row in dgvBankList.Rows)
            {
                if (Convert.ToBoolean(row.Cells[colCheckBox.Index].Value))
                {
                    rows.Add(row);
                }
            }

            return rows;
        }

        private void SearchSerialNo()
        {
            dbSerialSearch.SerialType = cboSearchType.Text;
            dbSerialSearch.Algorithm = cboSearchAlgorithim.Text;
            dbSerialSearch.SearchValue = txtSearchValue.Text;

            dbSerialSearch.Banks.Clear();

            foreach (DataGridViewRow row in getCheckedBanks())
            {
                dbSerialSearch.Banks.Add(new modelSerialSearch.Bank
                {
                    Code = Convert.ToString(row.Tag),
                    Name = Convert.ToString(row.Cells[colBank.Index].Tag)
                });
            }

            Cursor.Current = Cursors.WaitCursor;
            dbSerialSearch.Search();
            Cursor.Current = Cursors.Default;

            lvwReturnList.BeginUpdate();
            lvwReturnList.Items.Clear();

            int iLine = 0;

            foreach (modelSerialSearch.Result result in dbSerialSearch.Results)
            {
                ListViewItem item = new ListViewItem((++iLine).ToString());
                item.SubItems.Add(result.Bank);
                item.SubItems.Add(result.SerialNo);
                item.SubItems.Add(result.Model);
                item.SubItems.Add(result.Status);
                item.SubItems.Add(result.Location);
                item.Tag = result;
                lvwReturnList.Items.Add(item);
            }

            lvwReturnList.EndUpdate();

            if (dbSerialSearch.Errors.Count > 0)
            {
                dbFunction.SetMessageBox(string.Join("\n", dbSerialSearch.Errors), "Search", clsFunction.IconType.iWarning);
            }
        }

        private void dgvBankList_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvBankList.IsCurrentCellDirty)
            {
                dgvBankList.CommitEdit(DataGridViewDataErrorContexts.Commit);

                int iChecked = getCheckedBanks().Count;

                chkboxCheckAll.Checked = iChecked == dgvBankList.Rows.Count;
                chkBoxUncheckAll.Checked = iChecked == 0;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchSerialNo();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSearchValue.Clear();
            lvwReturnList.Items.Clear();

            if (cboSearchType.Items.Count > 0) cboSearchType.SelectedIndex = 0;
            if (cboSearchAlgorithim.Items.Count > 0) cboSearchAlgorithim.SelectedIndex = 0;

            FillGridBankSelection(dgvBankList);

            dbSerialSearch = new modelSerialSearch();

            lvwSNHistory.Items.Clear();

            txtSearchValue.Focus();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void chkboxCheckAll_Click(object sender, EventArgs e)
        {
            dgvBankList.EndEdit();

            foreach (DataGridViewRow row in dgvBankList.Rows)
            {
                row.Cells[colCheckBox.Index].Value = true;
            }

            chkboxCheckAll.Checked = true;
            chkBoxUncheckAll.Checked = false;
        }

        private void chkBoxUncheckAll_Click(object sender, EventArgs e)
        {
            dgvBankList.EndEdit();

            foreach (DataGridViewRow row in dgvBankList.Rows)
            {
                row.Cells[colCheckBox.Index].Value = false;
            }

            chkboxCheckAll.Checked = false;
            chkBoxUncheckAll.Checked = true;
        }

        private void frmSSerialNo_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void ViewSNHistory(modelSerialSearch.Result result)
        {
            lvwSNHistory.Items.Clear();

            if (result == null) return;
            if (!dbFunction.isValidID(result.ID)) return;
            if (result.HistorySearchBy.Length == 0) return;

            string pBank = clsGlobalVariables.strAPIBank;

            Cursor.Current = Cursors.WaitCursor;
            lvwSNHistory.BeginUpdate();

            try
            {
                clsGlobalVariables.strAPIBank = result.BankCode;

                dbAPI.ExecuteAPI("GET", "View", result.HistorySearchBy, dbFunction.CheckAndSetNumericValue(result.ID), "Advance Detail", "", "ViewAdvanceDetail");

                if (!clsGlobalVariables.isAPIResponseOK) return;
                if (dbAPI.isNoRecordFound()) return;

                int iLineNo = 0;

                for (int i = 0; i < clsArray.ServiceNo.Length; i++)
                {
                    string sInfo = clsArray.detail_info[i];

                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                    item.ForeColor = dbFunction.GetColorByStatus(0, dbAPI.GetValueFromJSONString(sInfo, clsDefines.TAG_ActionMade));

                    item.SubItems.Add(dbAPI.GetValueFromJSONString(sInfo, clsDefines.TAG_SERVICENO));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(sInfo, clsDefines.TAG_FSRNO));

                    item.SubItems.Add(dbAPI.GetValueFromJSONString(sInfo, clsDefines.TAG_MERCHANTNAME));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(sInfo, clsDefines.TAG_TID));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(sInfo, clsDefines.TAG_MID));

                    item.SubItems.Add(dbAPI.GetValueFromJSONString(sInfo, clsDefines.TAG_ServiceJobTypeDescription));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(sInfo, clsDefines.TAG_IRNO));

                    // Servicing Date Info (one extra call per service, same as the import screens)
                    string pJSONString = dbAPI.getInfoDetailJSON("Search", "Servicing Date Info", dbAPI.GetValueFromJSONString(sInfo, clsDefines.TAG_SERVICENO));

                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_RequestDate));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_CreatedDate));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ScheduleDate));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_DispatchDate));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ServiceDate));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TicketDate));

                    item.SubItems.Add(dbAPI.GetValueFromJSONString(sInfo, clsDefines.TAG_ActionMade));

                    item.SubItems.Add(dbAPI.GetValueFromJSONString(sInfo, clsDefines.TAG_TerminalSN));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(sInfo, clsDefines.TAG_SIMSN));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(sInfo, clsDefines.TAG_ReplaceTerminalSN));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(sInfo, clsDefines.TAG_ReplaceSIMSN));

                    item.SubItems.Add(dbAPI.GetValueFromJSONString(sInfo, clsDefines.TAG_JobTypeStatusDescription));

                    lvwSNHistory.Items.Add(item);
                }

                dbFunction.ListViewAlternateBackColor(lvwSNHistory);
            }
            finally
            {
                clsGlobalVariables.strAPIBank = pBank;
                lvwSNHistory.EndUpdate();
                Cursor.Current = Cursors.Default;
            }
        }

        private void InitSNHistoryList()
        {
            lvwSNHistory.Clear();   // removes the copied columns and any rows

            lvwSNHistory.View = View.Details;
            lvwSNHistory.FullRowSelect = true;
            lvwSNHistory.HideSelection = false;
            lvwSNHistory.BackColor = Color.GhostWhite;
            lvwSNHistory.BorderStyle = BorderStyle.FixedSingle;

            lvwSNHistory.Columns.Add("LINE#", 50, HorizontalAlignment.Left);
            lvwSNHistory.Columns.Add("SVC NO.", 0, HorizontalAlignment.Left);
            lvwSNHistory.Columns.Add("FSR NO.", 0, HorizontalAlignment.Left);
            lvwSNHistory.Columns.Add("NAME", 180, HorizontalAlignment.Left);
            lvwSNHistory.Columns.Add("TID", 90, HorizontalAlignment.Left);
            lvwSNHistory.Columns.Add("MID", 130, HorizontalAlignment.Left);
            lvwSNHistory.Columns.Add("SERVICE TYPE", 110, HorizontalAlignment.Left);
            lvwSNHistory.Columns.Add("REQUEST ID", 130, HorizontalAlignment.Left);
            lvwSNHistory.Columns.Add("REQUEST DATE", 130, HorizontalAlignment.Left);
            lvwSNHistory.Columns.Add("CREATED DATE", 130, HorizontalAlignment.Left);
            lvwSNHistory.Columns.Add("SCHEDULE DATE", 130, HorizontalAlignment.Left);
            lvwSNHistory.Columns.Add("DISPATCH DATE", 130, HorizontalAlignment.Left);
            lvwSNHistory.Columns.Add("SERVICE DATE", 130, HorizontalAlignment.Left);
            lvwSNHistory.Columns.Add("CLOSE DATE", 130, HorizontalAlignment.Left);
            lvwSNHistory.Columns.Add("SERVICE RESULT", 110, HorizontalAlignment.Left);
            lvwSNHistory.Columns.Add("(C) TERMINAL SN", 120, HorizontalAlignment.Left);
            lvwSNHistory.Columns.Add("(C) SIM SN", 160, HorizontalAlignment.Left);
            lvwSNHistory.Columns.Add("(N) TERMINAL SN", 120, HorizontalAlignment.Left);
            lvwSNHistory.Columns.Add("(N) SIM SN", 160, HorizontalAlignment.Left);
            lvwSNHistory.Columns.Add("SERVICE STATUS", 110, HorizontalAlignment.Left);
        }

        private void lvwReturnList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lvwReturnList.SelectedItems.Count == 0) return;

            ViewSNHistory(lvwReturnList.SelectedItems[0].Tag as modelSerialSearch.Result);
        }
    }
}