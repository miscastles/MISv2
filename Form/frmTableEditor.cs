using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Windows.Forms;

namespace MIS
{
    public partial class frmTableEditor : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;

        private const int PAGE_SIZE = 100;

        private DataTable dtTableContents;
        private string sCurrentTable = "";
        private int iCurrentPage = 1;
        private int iTotalPage = 1;
        private int iTotalRows = 0;

        public frmTableEditor()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwTableList, true);
            dbFunction.setDoubleBuffer(dgvTableContents, true);

            dgvTableContents.ReadOnly = true;
            dgvTableContents.AllowUserToAddRows = false;
            dgvTableContents.AllowUserToDeleteRows = false;
            dgvTableContents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTableContents.MultiSelect = false;
        }

        private void frmTableEditor_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            getTables();

            Cursor.Current = Cursors.Default;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void getTables()
        {
            int i = 0;
            int iLineNo = 0;

            lvwTableList.Items.Clear();

            dbAPI.ExecuteAPI("GET", "View", "Table List", "", "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.ID.Length > i)
                {
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                    item.SubItems.Add(clsArray.Description[i]);
                    item.Tag = clsArray.ID[i];

                    lvwTableList.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(lvwTableList);
            }
        }

        private void lvwTableList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwTableList.SelectedItems.Count == 0) return;

            sCurrentTable = lvwTableList.SelectedItems[0].SubItems[1].Text;
            iCurrentPage = 1;

            getTableContents();
        }

        private void getTableContents()
        {
            Cursor.Current = Cursors.WaitCursor;

            dgvTableContents.DataSource = null;
            dtTableContents = null;
            lblSelectedRow.Text = "";

            dbAPI.ExecuteAPI("GET", "View", "Table Rows", sCurrentTable + clsFunction.sPipe + iCurrentPage, "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK || dbAPI.isNoRecordFound())
            {
                Cursor.Current = Cursors.Default;
                return;
            }

            int iHeader = Array.IndexOf(clsArray.ID, "0");
            if (iHeader < 0)
            {
                Cursor.Current = Cursors.Default;
                return;
            }

            JObject header = JObject.Parse(clsArray.detail_info[iHeader]);

            iTotalRows = (int)header["TotalRows"];
            iTotalPage = Math.Max(1, (int)Math.Ceiling(iTotalRows / (double)PAGE_SIZE));

            DataTable dt = new DataTable();

            foreach (JToken col in header["Columns"])
            {
                dt.Columns.Add(col.ToString());
            }

            for (int i = 0; i < clsArray.ID.Length; i++)
            {
                if (i == iHeader) continue;

                JArray values = JArray.Parse(clsArray.detail_info[i]);
                DataRow row = dt.NewRow();

                for (int column = 0; column < values.Count; column++)
                {
                    row[column] = values[column].Type == JTokenType.Null ? (object)DBNull.Value : values[column].ToString();
                }

                dt.Rows.Add(row);
            }

            dtTableContents = dt;

            dgvTableContents.DataSource = dtTableContents;

            lblPage.Text = $"[ {iCurrentPage:N0} / {iTotalPage:N0} ]";
            lblHeader.Text = $"DEVELOPMENT-TABLE-EDITOR - [ {sCurrentTable} ]";
            btnBack.Enabled = iCurrentPage > 1;
            btnNext.Enabled = iCurrentPage < iTotalPage;

            Cursor.Current = Cursors.Default;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (dtTableContents == null || iCurrentPage <= 1) return;

            iCurrentPage--;
            getTableContents();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (dtTableContents == null || iCurrentPage >= iTotalPage) return;

            iCurrentPage++;
            getTableContents();
        }

        private void dgvTableContents_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTableContents.CurrentRow == null || dtTableContents == null)
            {
                lblSelectedRow.Text = "[ - ]";
                return;
            }

            int iRow = (iCurrentPage - 1) * PAGE_SIZE + dgvTableContents.CurrentRow.Index + 1;
            lblSelectedRow.Text = $"[ ROW {iRow:N0} / {iTotalRows:N0} ]";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dgvTableContents.DataSource = null;
            dtTableContents = null;

            sCurrentTable = "";
            iCurrentPage = 1;
            iTotalPage = 1;
            iTotalRows = 0;

            lvwTableList.SelectedItems.Clear();

            lblPage.Text = "[ PAGE ]";
            lblSelectedRow.Text = "[ SELECTED ROW ]";
            btnBack.Enabled = false;
            btnNext.Enabled = false;

            rteQuery.Clear();
        }

        private const int SELECT_LIMIT = 1000;

        private string getDevConnString()
        {
            return clsClientConnection.DbConnString() + "AllowBatch=false;";
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            string sQuery = (rteQuery.SelectionLength > 0 ? rteQuery.SelectedText : rteQuery.Text).Trim().TrimEnd(';');

            if (sQuery.Length == 0) return;

            string sKeyword = sQuery.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries)[0].ToUpper();

            switch (sKeyword)
            {
                case "SELECT":
                case "SHOW":
                case "DESCRIBE":
                case "EXPLAIN":
                    runSelect(sQuery);
                    break;
                case "UPDATE":
                case "INSERT":
                case "DELETE":
                    runModify(sQuery);
                    break;
                default:
                    dbFunction.SetMessageBox($"[{sKeyword}] is not allowed in the editor.", "Query Blocked", clsFunction.IconType.iError);
                    break;
            }
        }

        private void runSelect(string sQuery)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                using (var conn = new MySqlConnection(getDevConnString()))
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand($"SET SESSION sql_select_limit = {SELECT_LIMIT}", conn))
                        cmd.ExecuteNonQuery();

                    using (var adapter = new MySqlDataAdapter(sQuery, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.SelectCommand.CommandTimeout = 60;
                        adapter.Fill(dt);

                        // Unmount the table so Back/Next don't reload it over the result
                        sCurrentTable = "";
                        iCurrentPage = 1;
                        iTotalRows = dt.Rows.Count;

                        dtTableContents = dt;
                        dgvTableContents.DataSource = dtTableContents;

                        lblHeader.Text = "DEVELOPMENT-TABLE-EDITOR - [ QUERY ]";
                        lblPage.Text = $"[ {dt.Rows.Count:N0} ROW(S) ]";
                        btnBack.Enabled = false;
                        btnNext.Enabled = false;
                    }
                }
            }
            catch (MySqlException ex)
            {
                dbFunction.SetMessageBox(ex.Message, "Query Error", clsFunction.IconType.iError);
            }

            Cursor.Current = Cursors.Default;
        }

        private int executeModify(string sQuery, int iExpected)
        {
            using (var conn = new MySqlConnection(getDevConnString()))
            {
                conn.Open();

                using (var cmd = new MySqlCommand("SET SESSION sql_safe_updates = 1", conn))
                    cmd.ExecuteNonQuery();

                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    int iAffected;

                    using (var cmd = new MySqlCommand(sQuery, conn, trans))
                    {
                        cmd.CommandTimeout = 60;
                        iAffected = cmd.ExecuteNonQuery();
                    }

                    if (iExpected < 0 || iAffected != iExpected)
                    {
                        trans.Rollback();
                        return iExpected < 0 ? iAffected : -1;
                    }

                    using (var cmd = new MySqlCommand("INSERT INTO tbldevquerylog (UserID, QueryText, AffectedRows, ProcessedDateTime) " +
                                                      "VALUES (@UserID, @QueryText, @AffectedRows, NOW())", conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@UserID", clsUser.ClassUserID);
                        cmd.Parameters.AddWithValue("@QueryText", sQuery);
                        cmd.Parameters.AddWithValue("@AffectedRows", iAffected);
                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                    return iAffected;
                }
            }
        }

        private void runModify(string sQuery)
        {
            try
            {
                int iPreview = executeModify(sQuery, -1);

                if (!dbFunction.fPromptConfirmation($"{iPreview:N0} row(s) will be affected.\n\nCommit this change?")) return;

                int iAffected = executeModify(sQuery, iPreview);

                if (iAffected < 0)
                {
                    dbFunction.SetMessageBox("Row count changed since the preview. Nothing was saved, please run again.", "Query Cancelled", clsFunction.IconType.iWarning);
                    return;
                }

                dbFunction.SetMessageBox($"{iAffected:N0} row(s) saved.", "Saved", clsFunction.IconType.iInformation);
            }
            catch (MySqlException ex)
            {
                dbFunction.SetMessageBox(ex.Message, "Query Error", clsFunction.IconType.iError);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            rteQuery.Clear();
        }
    }
}