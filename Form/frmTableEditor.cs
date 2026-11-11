using FastColoredTextBoxNS;
using MIS.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIS
{
    public partial class frmTableEditor : Form
    {
        private clsFunction dbFunction;
        private clsAPI dbAPI;

        private modelTableEditor.Table dbTable = new modelTableEditor.Table();
        private modelTableEditor.Commands dbCommands = new modelTableEditor.Commands();
        private modelTableEditor.Function dbSqlFunction = new modelTableEditor.Function();
        private modelTableEditor.Routine dbRoutine = new modelTableEditor.Routine();

        private System.Diagnostics.Stopwatch swRuntime = new System.Diagnostics.Stopwatch();
        private System.Windows.Forms.Timer tmrRuntime = new System.Windows.Forms.Timer();

        private const int PAGE_SIZE = 100;

        private DataTable dtTableContents;

        private string sCurrentTable = "";
        private string sCurrentFunction = "";
        private string sCurrentRoutine = "";

        // default values
        private int iCurrentPage = 1;
        private int iTotalPage = 1;
        private int iTotalRows = 0;

        // precaution
        private bool runAccess = false;

        private bool fFormatting = false;
        private bool isFormClosed => IsDisposed;

        private static readonly HashSet<string> SQL_KEYWORDS = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "SELECT", "FROM", "WHERE", "AND", "OR", "NOT", "IN", "IS", "NULL", "LIKE", "BETWEEN", "EXISTS",
            "INSERT", "INTO", "VALUES", "UPDATE", "SET", "DELETE",
            "JOIN", "INNER", "LEFT", "RIGHT", "OUTER", "CROSS", "ON", "AS", "USING",
            "GROUP", "BY", "ORDER", "HAVING", "LIMIT", "OFFSET", "ASC", "DESC", "DISTINCT", "UNION", "ALL",
            "CASE", "WHEN", "THEN", "ELSE", "ELSEIF", "END", "IF",
            "SHOW", "DESCRIBE", "EXPLAIN", "CREATE", "ALTER", "DROP", "TRUNCATE", "TABLE", "PROCEDURE", "FUNCTION",
            "BEGIN", "DECLARE", "CALL", "RETURN", "RETURNS", "DEFAULT"
        };

        public frmTableEditor()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwTableList, true);
            dbFunction.setDoubleBuffer(dgvTableContents, true);

            tmrRuntime.Interval = 250;
            tmrRuntime.Tick += tmrRuntime_Tick;
        }

        private void frmTableEditor_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbFunction = new clsFunction();

            getTables();
            getFunctions();
            getRoutines();

            Cursor.Current = Cursors.Default;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void getTables()
        {
            int iLineNo = 0;

            lvwTableList.Items.Clear();

            List<string> tables = dbTable.Fetch();

            foreach (string sTable in tables)
            {
                iLineNo++;
                ListViewItem item = new ListViewItem(iLineNo.ToString());

                item.SubItems.Add(sTable);

                lvwTableList.Items.Add(item);
            }

            if (tables.Count > 0) dbFunction.ListViewAlternateBackColor(lvwTableList);
        }

        private void getFunctions()
        {
            int iLineNo = 0;

            lvwFunctionList.Items.Clear();

            List<string> functions = dbSqlFunction.Fetch();

            foreach (string sFunction in functions)
            {
                iLineNo++;
                ListViewItem item = new ListViewItem(iLineNo.ToString());

                item.SubItems.Add(sFunction);

                lvwFunctionList.Items.Add(item);
            }

            if (functions.Count > 0) dbFunction.ListViewAlternateBackColor(lvwFunctionList);
        }

        private void getRoutines()
        {
            int iLineNo = 0;

            lvwRoutineList.Items.Clear();

            List<string> routines = dbRoutine.Fetch();

            foreach (string sRoutine in routines)
            {
                iLineNo++;
                ListViewItem item = new ListViewItem(iLineNo.ToString());

                item.SubItems.Add(sRoutine);

                lvwRoutineList.Items.Add(item);
            }

            if (routines.Count > 0) dbFunction.ListViewAlternateBackColor(lvwRoutineList);
        }

        private void lvwTableList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwTableList.SelectedItems.Count == 0) return;

            sCurrentTable = lvwTableList.SelectedItems[0].SubItems[1].Text;
            iCurrentPage = 1;

            getTableContents();
        }

        private void lvwFunctionList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwFunctionList.SelectedItems.Count == 0) return;

            sCurrentFunction = lvwFunctionList.SelectedItems[0].SubItems[1].Text;

            getFunctionContents();
        }

        private void lvwRoutineList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwRoutineList.SelectedItems.Count == 0) return;

            sCurrentRoutine = lvwRoutineList.SelectedItems[0].SubItems[1].Text;

            getRoutineContents();
        }

        private void getFunctionContents()
        {
            Cursor.Current = Cursors.WaitCursor;

            string sCode = dbSqlFunction.Contents(sCurrentFunction);

            if (sCode.Length == 0)
            {
                Cursor.Current = Cursors.Default;
                return;
            }

            fFormatting = true;
            rteQuery.Text = sCode;
            fFormatting = false;

            lblHeader.Text = $"DEVELOPMENT-TABLE-EDITOR - [ {sCurrentFunction} ]";

            Cursor.Current = Cursors.Default;
        }

        private void getRoutineContents()
        {
            Cursor.Current = Cursors.WaitCursor;

            string sCode = dbRoutine.Contents(sCurrentRoutine);

            if (sCode.Length == 0)
            {
                Cursor.Current = Cursors.Default;
                return;
            }

            fFormatting = true;
            rteQuery.Text = sCode;
            fFormatting = false;

            lblHeader.Text = $"DEVELOPMENT-TABLE-EDITOR - [ {sCurrentRoutine} ]";

            Cursor.Current = Cursors.Default;
        }

        private void getTableContents()
        {
            Cursor.Current = Cursors.WaitCursor;

            dgvTableContents.DataSource = null;
            dtTableContents = null;
            lblSelectedRow.Text = "";

            List<string> rows = dbTable.Contents(sCurrentTable, iCurrentPage);

            if (rows.Count == 0)
            {
                Cursor.Current = Cursors.Default;
                return;
            }

            JObject header = JObject.Parse(rows[0]);

            iTotalRows = (int)header["TotalRows"];
            iTotalPage = Math.Max(1, (int)Math.Ceiling(iTotalRows / (double)PAGE_SIZE));

            DataTable dt = new DataTable();

            foreach (JToken col in header["Columns"])
            {
                dt.Columns.Add(col.ToString());
            }

            for (int i = 1; i < rows.Count; i++)
            {
                JArray values = JArray.Parse(rows[i]);
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

        private async void btnRun_Click(object sender, EventArgs e)
        {
            string sQuery = rteQuery.SelectionLength > 0 ? rteQuery.SelectedText : rteQuery.Text;
            string sRunText = btnRun.Text;

            bool commandReturn;
            DataTable dt = null;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                btnRun.Enabled = false;

                barLoading.Style = ProgressBarStyle.Marquee;
                barLoading.MarqueeAnimationSpeed = 10;

                swRuntime.Restart();
                showRuntime();
                tmrRuntime.Start();

                commandReturn = await Task.Run(() => dbCommands.Run(sQuery, out dt));
            }
            finally
            {
                tmrRuntime.Stop();
                swRuntime.Stop();

                if (!isFormClosed)
                {
                    showRuntime();

                    barLoading.Style = ProgressBarStyle.Blocks;
                    barLoading.Value = 0;

                    btnRun.Text = sRunText;
                    btnRun.Enabled = true;

                    lblRuntime.Text = "[ RUNTIME ]";

                    Cursor.Current = Cursors.Default;
                }
            }

            if (isFormClosed) return;

            rteRawReturn.Text = dbCommands.RawReturn;
            tabReturns.SelectedTab = (commandReturn && dt != null) ? tabDataReturn : tabRawReturn;

            if (!commandReturn || dt == null) return;

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

        private void btnReset_Click(object sender, EventArgs e)
        {
            rteQuery.Clear();
        }

        private void rteQuery_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            if (fFormatting)
            {
                return;
            }

            int iCaret = rteQuery.SelectionStart;
            string sText = rteQuery.Text;
            
            if (iCaret < 2 || iCaret > sText.Length)
            {
                return;
            }

            if (isSQLSyntax(sText[iCaret - 1]))
            {
                return;
            }

            int iEnd = iCaret - 1;
            int iStart = iEnd;

            while (iStart > 0 && isSQLSyntax(sText[iStart - 1])) iStart--;

            if (iStart == iEnd)
            {
                return;
            }

            string sWord = sText.Substring(iStart, iEnd - iStart);

            if (!SQL_KEYWORDS.Contains(sWord) || sWord == sWord.ToUpper())
            {
                return;
            }

            if (iStart > 0 && sText[iStart - 1] == '.')
            {
                return;
            }

            int iLineStart = iStart > 0 ? sText.LastIndexOf('\n', iStart - 1) + 1 : 0;

            if (sText.IndexOf("--", iLineStart, iStart - iLineStart) >= 0)
            {
                return;
            }

            bool inQuotes = false;
            char cQuote = '\0';

            for (int i = iLineStart; i < iStart; i++)
            {
                char c = sText[i];

                if (inQuotes)
                {
                    if (c == cQuote) inQuotes = false;
                }
                else if (c == '\'' || c == '"' || c == '`')
                {
                    inQuotes = true;
                    cQuote = c;
                }
            }

            if (inQuotes)
            {
                return;
            }

            fFormatting = true;

            try
            {
                rteQuery.SelectionStart = iStart;
                rteQuery.SelectionLength = sWord.Length;
                rteQuery.SelectedText = sWord.ToUpper();

                rteQuery.SelectionStart = iCaret;
                rteQuery.SelectionLength = 0;
            }
            finally
            {
                fFormatting = false;
            }
        }

        private bool isSQLSyntax(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_';
        }

        private void tmrRuntime_Tick(object sender, EventArgs e)
        {
            showRuntime();
        }

        private void showRuntime()
        {
            TimeSpan elapsed = swRuntime.Elapsed;
            lblRuntime.Text = $"[ {(int)elapsed.TotalMinutes:00}:{elapsed.Seconds:00} ]";
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
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