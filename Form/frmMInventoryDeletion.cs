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
    public partial class frmMInventoryDeletion : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;

        protected override CreateParams CreateParams
        {
            // Override CreateParams to enable double-buffering for child controls
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED
                //cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        public frmMInventoryDeletion()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefactor_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidDescriptionEntry(cboInventoryType.Text, "Inventory Type" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            dbFunction.ClearTextBox(this);
            
            rtbSNList.Text = FormatToCsv(rtbSNList.Text);
            rtbSNList.Focus();

            if (!string.IsNullOrWhiteSpace(rtbSNList.Text))
            {
                btnValidate.Enabled = true;
                btnProceedDeletion.Enabled = btnCopyClipboard.Enabled = false;

                UpdateCount();
            }
                
        }
        
        public static string FormatToCsv(string input)
        {
            return string.Join(",",
                input.Split(new[] { '\r', '\n', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                     .Select(x => x.Trim())
                     .Where(x => !string.IsNullOrWhiteSpace(x))
                     .Distinct()
            );
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            rtbSNList.Clear();

            dbFunction.ClearTextBox(this);
            dbFunction.ClearListViewItems(lvwList);

            tabDeletion.SelectedIndex = 0; // swith to tab (paste/load)

            cboInventoryType.Text = clsFunction.sDefaultSelect;

            btnValidate.Enabled = btnProceedDeletion.Enabled = btnCopyClipboard.Enabled = false;

            rtbSNList.Focus();
        }

        private void frmMInventoryDeletion_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ClearListViewItems(lvwList);
            cboInventoryType.Text = clsFunction.sDefaultSelect;

            lvwList.View = View.Details;
            lvwList.FullRowSelect = true;
            lvwList.GridLines = true;

            // Clear any existing columns first
            lvwList.Columns.Clear();

            // Add columns
            lvwList.Columns.Add("Line#", 60, HorizontalAlignment.Left);
            lvwList.Columns.Add("ID", 0, HorizontalAlignment.Left);
            lvwList.Columns.Add("Serial No.", 150, HorizontalAlignment.Left);
            lvwList.Columns.Add("Location", 100, HorizontalAlignment.Left);
            lvwList.Columns.Add("Client Name", 90, HorizontalAlignment.Left);
            lvwList.Columns.Add("StatusID", 0, HorizontalAlignment.Left);
            lvwList.Columns.Add("Status", 100, HorizontalAlignment.Left);
            lvwList.Columns.Add("Result", 120, HorizontalAlignment.Left);


            btnValidate.Enabled = btnProceedDeletion.Enabled = btnCopyClipboard.Enabled = false;

            rtbSNList.Focus();
        }

        public static int GetTotalInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            return input
                .Split(new[] { '\r', '\n', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Count();
        }

        public static int GetTotalUniqueInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            return input
                .Split(new[] { '\r', '\n', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .Count();
        }

        private void UpdateCount()
        {
            int total = GetTotalInput(rtbSNList.Text);
            int unique = GetTotalUniqueInput(rtbSNList.Text);

            txtTInput.Text = $"{total}";
            txtTUniqueInput.Text = $"{unique}";
            
        }

        private void loadData(ListView lvw)
        {
            int iLineNo = 0;
            int i = 0;

            int cntNotFound = 0;
            int cntRestricted = 0;
            int cntReady = 0;
            string mode = "Delete";

            dbFunction.ClearListViewItems(lvw);

            dbAPI.ExecuteAPI("GET", "View", "Inventory Bulk Cross-Check List", $"{mode}{clsFunction.sPipe}{cboInventoryType.Text}{clsFunction.sPipe}{rtbSNList.Text}","Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.ID.Length > i)
                {
                    iLineNo++;

                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                    // Data
                    string pJSONString = clsArray.detail_info[i];

                    string id = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ID);
                    string sn = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SerialNo);
                    string location = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Location);
                    string clientname = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ClientName);
                    string statusId = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_StatusID);
                    string status = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Status);
                    string result = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Result);

                    item.SubItems.Add(id);
                    item.SubItems.Add(sn);
                    item.SubItems.Add(location);
                    item.SubItems.Add(clientname);
                    item.SubItems.Add(statusId);
                    item.SubItems.Add(status);
                    item.SubItems.Add(result);

                    item.UseItemStyleForSubItems = false; 
                    ListViewItem.ListViewSubItem resultSubItem = item.SubItems[7];

                    switch (result?.ToUpper())
                    {
                        case "READY TO DELETE":
                            resultSubItem.BackColor = Color.LightGreen;
                            resultSubItem.ForeColor = Color.Black;
                            break;

                        case "RESTRICTED":
                            resultSubItem.BackColor = Color.Red;
                            resultSubItem.ForeColor = Color.White;
                            break;

                        case "NOT FOUND":
                            resultSubItem.BackColor = Color.LightBlue;
                            resultSubItem.ForeColor = Color.Black;
                            break;
                    }

                    // COUNTING LOGIC
                    switch (result?.ToUpper())
                    {
                        case "NOT FOUND":
                            cntNotFound++;
                            break;
                        case "RESTRICTED":
                            cntRestricted++;
                            break;
                        case "READY TO DELETE":
                            cntReady++;
                            break;
                    }

                    lvw.Items.Add(item);
                    i++;
                }

                //dbFunction.ListViewAlternateBackColor(lvw);
            }

            // update count
            txtTNotFound.Text = $"{cntNotFound}";
            txtRestricted.Text = $"{cntRestricted}";
            txtReady.Text = $"{cntReady}";
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidDescriptionEntry(cboInventoryType.Text, "Inventory Type" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            btnProceedDeletion.Enabled = btnCopyClipboard.Enabled = false;

            if (dbFunction.isValidID(txtTUniqueInput.Text))
            {
                Cursor.Current = Cursors.WaitCursor;

                tabDeletion.SelectedIndex = 1; // swith to tab (preview result)

                loadData(lvwList);

                if (dbFunction.isValidCount(lvwList.Items.Count))
                    btnProceedDeletion.Enabled = btnCopyClipboard.Enabled = true;

                Cursor.Current = Cursors.Default;
            }
            else
            {
                MessageBox.Show("No records to process.");
            }
                
        }

        private void frmMInventoryDeletion_Activated(object sender, EventArgs e)
        {
            rtbSNList.Focus();
        }

        private void btnProceedDeletion_Click(object sender, EventArgs e)
        {
            var snList = new List<string>();

            if (!dbFunction.isValidDescriptionEntry(cboInventoryType.Text, "Inventory Type" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            // check ready
            if (!dbFunction.isValidID(txtReady.Text))
            {
                dbFunction.SetMessageBox(
                    "No valid records found to import. Please validate your list.",
                    "Import Terminal",
                    clsFunction.IconType.iError
                );
                return;
            }

            // check restricted
            /*
            if (dbFunction.isValidID(txtRestricted.Text))
            {
                dbFunction.SetMessageBox(
                    "Import blocked: Some items are currently restricted or in use.",
                    "Import Terminal",
                    clsFunction.IconType.iError
                );
                return;
            }
            */

            if (!dbFunction.fPromptConfirmation("Proceed to delete all READY TO DELETE records?")) return;

            foreach (ListViewItem item in lvwList.Items)
            {
                string id = item.SubItems[1].Text;     // ID column
                string result = item.SubItems[7].Text; // Result column
                string serialNo = item.SubItems[2].Text; // SerialNo column

                if (result == "READY TO DELETE" && !string.IsNullOrWhiteSpace(serialNo))
                {
                    snList.Add(serialNo.Trim());
                }
            }

            // Convert to CSV
            string csvSNList = string.Join(",", snList);

            Debug.WriteLine("csvSNList="+csvSNList);

            if (!string.IsNullOrEmpty(csvSNList))
            {
                // call deleteCollectionDetail
                dbAPI.ExecuteAPI("DELETE", "Delete", "Inventory Deletion", $"{cboInventoryType.Text}{clsFunction.sPipe}{csvSNList}", "Inventory Deletion", "", "DeleteCollectionDetail");

                dbFunction.SetMessageBox($"[{cboInventoryType.Text}] deletion completed.", "Inventory Deletion", clsFunction.IconType.iInformation);

                btnProceedDeletion.Enabled = false;
            }
            
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnCopyClipboard_Click(object sender, EventArgs e)
        {
            if (dbFunction.isValidCount(lvwList.Items.Count))
            {
                dbFunction.CopyListViewToClipboard(lvwList, false);

                dbFunction.SetMessageBox("Copy to clipboard complete.", "Inventory Deletion", clsFunction.IconType.iNone);
            }
                
        }

        private void cboInventoryType_SelectedIndexChanged(object sender, EventArgs e)
        {
            rtbSNList.Focus();
        }
    }
}
