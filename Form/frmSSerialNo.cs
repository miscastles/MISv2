using MIS.Model;
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

            dbFunction.FillSerialNoSearchTypes(cboSearchType);
            dbFunction.FillSerialNoSearchAlgorithim(cboSearchAlgorithim);

            colCheckBox.DisplayIndex = 0;

            FillGridBankSelection(dgvBankList);
            chkboxCheckAll.Checked = true;
            chkBoxUncheckAll.Checked = false;
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
                lvwReturnList.Items.Add(item);
            }

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

        private void frmTableEditor_KeyDown(object sender, KeyEventArgs e)
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