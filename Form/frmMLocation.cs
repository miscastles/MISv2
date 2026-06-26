using Newtonsoft.Json.Bson;
using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MIS.Function.AppUtilities;

namespace MIS
{
    public partial class frmMLocation : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;

        public static string sHeader;
        bool fEdit = false;

        public frmMLocation()
        {
            InitializeComponent();
        }

        private void frmMLocation_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);

            LoadData();

            fEdit = false;
            InitButton();

            InitSearchTextBox(true);
            txtSearch.Focus();

            ComboBoxDefaultSelect();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool ValidateFields()
        {
            if (!dbFunction.isValidDescriptionEntry(txtDescription.Text, "Description" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            return true;
        }

        private void InitButton()
        {
            if (fEdit)
            {
                btnAdd.Enabled = false;
                btnSave.Enabled = true;
            }
            else
            {
                btnAdd.Enabled = true;
                btnSave.Enabled = false;
            }
        }

        private void ComboBoxDefaultSelect()
        {
            txtID.ResetText();
            txtDescription.ResetText();
        }

        private void InitSearchTextBox(bool isEnable)
        {
            txtSearch.Enabled = isEnable;
            if (isEnable)
            {
                txtSearch.BackColor = Color.White;
                txtSearch.ReadOnly = false;
            }
            else
            {
                txtSearch.BackColor = Color.WhiteSmoke;
                txtSearch.ReadOnly = true;
            }

        }

        private void LoadData()
        {
            int i = 0;
            int iLineNo = 0;

            Cursor.Current = Cursors.WaitCursor;

            lvwList.Items.Clear();

            dbAPI.ExecuteAPI("GET", "View", "Location List", "", "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                lvwList.Items.Clear();
                    
                while (clsArray.ID.Length > i)
                {
                    iLineNo++;

                    string pLocationID = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "LocationID");
                    string pCode = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "Code");
                    string pDescription = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "Description");
                    string pIsReleased  = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "isReleased");
                    string pIsTerminalReleased = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "isTerminalReleased");
                    string pIsSIMReleased = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "isSIMReleased");
                    string pIsDeploy = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "isDeploy");
                    string pIsReturn = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "isReturn");

                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                    item.SubItems.Add(pLocationID);
                    item.SubItems.Add(pCode);
                    item.SubItems.Add(pDescription);
                    item.SubItems.Add(pIsReleased);
                    item.SubItems.Add(pIsTerminalReleased);
                    item.SubItems.Add(pIsSIMReleased);
                    item.SubItems.Add(pIsDeploy);
                    item.SubItems.Add(pIsReturn);

                    lvwList.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(lvwList);
            }

            Cursor.Current = Cursors.Default;
        }

        private string GenerateLocationCode()
        {
            int maxCode = 0;

            foreach (ListViewItem item in lvwList.Items)
            {
                string pCode = item.SubItems[2].Text.Trim();

                int code;

                if (int.TryParse(pCode, out code))
                {
                    if (code > maxCode)
                    {
                        maxCode = code;
                    }
                }
            }

            return dbFunction.padLeftChar((maxCode + 1).ToString(), clsFunction.sZero, 4);
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.SelectedItems.Count == 0) return;

            ListViewItem selected = lvwList.SelectedItems[0];

            if (selected.SubItems[1].Text.Length > 0)
            {
                string pSelectedRow = dbFunction.GetListViewSelectedRow(lvwList, 0);
                Debug.WriteLine("pSelectedRow=\n" + pSelectedRow);

                dbFunction.TextBoxUnLock(true, this);
                dbFunction.ComBoBoxUnLock(true, this);

                txtID.Text = selected.SubItems[1].Text;
                txtDescription.Text = selected.SubItems[3].Text;

                chkboxReleased.Checked = selected.SubItems[4].Text == clsFunction.sOne;
                chkboxTerminalReleased.Checked = selected.SubItems[5].Text == clsFunction.sOne;
                chkboxSIMReleased.Checked = selected.SubItems[6].Text == clsFunction.sOne;
                chkboxDeployed.Checked = selected.SubItems[7].Text == clsFunction.sOne;
                chkboxReturn.Checked = selected.SubItems[8].Text == clsFunction.sOne;

                fEdit = true;

                InitButton();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);

            chkboxDeployed.Checked = false;
            chkboxReleased.Checked = false;
            chkboxReturn.Checked = false;
            chkboxSIMReleased.Checked = false;
            chkboxTerminalReleased.Checked = false;

            fEdit = false;
            InitButton();   

            ComboBoxDefaultSelect();

            InitSearchTextBox(true);
            txtSearch.Focus();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            fEdit = false;
            dbFunction.ClearTextBox(this);
            InitButton();

            btnSave.Enabled = true;
            btnAdd.Enabled = false;

            txtID.Enabled = false;

            dbFunction.TextBoxUnLock(true, this);
            dbFunction.ComBoBoxUnLock(true, this);

            InitSearchTextBox(false);
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    dbFunction.findAndSelectListViewItem(lvwList, txtSearch.Text);
                    break;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            dbFunction.findAndSelectListViewItem(lvwList, txtSearch.Text);
        }
        
        private void btnSave_Click(object sender, EventArgs e)
        {
            int ID = 0;
            string sRowSQL = "";
            string sSQL = "";

            if (!ValidateFields()) return;

            string pCode = GenerateLocationCode();
            string pDescription = StrClean(dbFunction.CheckAndSetStringValue(txtDescription.Text));
            string pIsReleased = chkboxReleased.Checked ? clsFunction.sOne : clsFunction.sZero;
            string pIsTerminalReleased = chkboxTerminalReleased.Checked ? clsFunction.sOne : clsFunction.sZero;
            string pIsSIMReleased = chkboxSIMReleased.Checked ? clsFunction.sOne : clsFunction.sZero;
            string pIsDeploy = chkboxDeployed.Checked ? clsFunction.sOne : clsFunction.sZero;
            string pIsReturn = chkboxReturn.Checked ? clsFunction.sOne : clsFunction.sZero;

            if (!fEdit)
            {
                if (!dbFunction.fSavingConfirm(false)) return;
            }
            else
            {
                if (!dbFunction.fSavingConfirm(true)) return;

                ID = int.Parse(txtID.Text);
            }

            if (!fEdit)
            {
                if (dbAPI.isRecordExist("Search", "Location", pDescription))
                {
                    dbFunction.SetMessageBox("Location details already exist.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return;
                }

                // Insert
                sRowSQL = "";
                sSQL = "";
                sRowSQL = " ('" + StrClean(dbFunction.CheckAndSetStringValue(pCode)) + "'," +
                sRowSQL + sRowSQL + "'" + StrClean(dbFunction.CheckAndSetStringValue(pDescription)) + "'," +
                sRowSQL + sRowSQL + "'" + StrClean(dbFunction.CheckAndSetStringValue(pIsReleased)) + "'," +
                sRowSQL + sRowSQL + "'" + StrClean(dbFunction.CheckAndSetStringValue(pIsTerminalReleased)) + "'," +
                sRowSQL + sRowSQL + "'" + StrClean(dbFunction.CheckAndSetStringValue(pIsSIMReleased)) + "'," +
                sRowSQL + sRowSQL + "'" + StrClean(dbFunction.CheckAndSetStringValue(pIsDeploy)) + "'," +
                sRowSQL + sRowSQL + "'" + StrClean(dbFunction.CheckAndSetStringValue(pIsReturn)) + "') ";
                sSQL = sSQL + sRowSQL;

                Debug.WriteLine("Update::" + "sSQL=" + sSQL);

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "Location", sSQL, "InsertMaintenanceMaster");

                MessageBox.Show("New Zoning successfully saved", "Saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }
            else
            {
                clsSearch.ClassAdvanceSearchValue =
                    ID.ToString() + clsFunction.sPipe +
                    pCode + clsFunction.sPipe +
                    pDescription + clsFunction.sPipe +
                    pIsReleased + clsFunction.sPipe +
                    pIsTerminalReleased + clsFunction.sPipe +
                    pIsSIMReleased + clsFunction.sPipe +
                    pIsDeploy + clsFunction.sPipe +
                    pIsReturn;

                Debug.WriteLine("Update::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                dbAPI.ExecuteAPI("PUT", "Update", "Location", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                MessageBox.Show("Location has been successfully modified", "Edited",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);
            }

            btnRefresh_Click(this, e);

            btnClear_Click(this, e);
        }
    }
}
