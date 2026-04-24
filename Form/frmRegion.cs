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
    public partial class frmRegion : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;

        bool fEdit = false;

        public frmRegion()
        {
            InitializeComponent();
        }
        private enum iInitMode
        {
            iEnable,
            iDisable
        }

        private void frmRegion_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            dbFunction.ClearTextBox(this);
            InitButton();
            SetInitMode(iInitMode.iDisable);
            LoadRegion();
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
        private bool ValidateFields()
        {
            bool fValid = false;

            if (txtName.TextLength > 0)
                fValid = true;

            if (!fValid)
            {
                MessageBox.Show("Check the following field(s) listed below:\n\n" +
                                "*Name\n" +
                                "\n" +
                                "Field(s) with asterisk(*) must not be blank.", "Incomplete Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return fValid;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            fEdit = false;
            dbFunction.ClearTextBox(this);
            InitButton();
            SetInitMode(iInitMode.iEnable);
            btnSave.Enabled = true;
            btnAdd.Enabled = false;
            txtName.Focus();
        }
        private void SetInitMode(iInitMode iMode)
        {
            switch (iMode)
            {
                case iInitMode.iEnable:
                    txtName.Enabled = true;
                    txtName.BackColor = Color.White;
                    break;
                case iInitMode.iDisable:
                    txtName.Enabled = false;
                    txtName.BackColor = Color.LightGray;
                    break;
            }
        }
        private void LoadRegion()
        {
            int i = 0;

            dbFunction.ClearListViewItems(lvwList);
            dbAPI.ExecuteAPI("GET", "View", "", "", "Region", "", "ViewRegion");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            while (clsArray.RegionID.Length > i)
            {
                clsRegion.ClassRegionID = int.Parse(clsArray.RegionID[i].ToString());
                clsRegion.ClassRegion = clsArray.Region[i];

                i++;

                AddItem(i);
            }

            dbFunction.ListViewAlternateBackColor(lvwList);

        }
        private void AddItem(int inLineNo)
        {
            // Add to List            
            ListViewItem item = new ListViewItem(inLineNo.ToString());
            item.SubItems.Add(clsRegion.ClassRegionID.ToString());
            item.SubItems.Add(clsRegion.ClassRegion.ToString());
            lvwList.Items.Add(item);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int ID = 0;
            string sRowSQL = "";
            string sSQL = "";

            if (!ValidateFields()) return;

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

                if (dbAPI.isRecordExist("Search", "Region", dbFunction.CheckAndSetStringValue(txtName.Text)))
                {
                    dbFunction.SetMessageBox("Region " + dbFunction.AddBracketStartEnd(txtName.Text) + " already exist.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return;
                }

                sRowSQL = "";
                sRowSQL = " ('" + txtName.Text + "') ";
                sSQL = sSQL + sRowSQL;

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "Region", sSQL, "InsertMaintenanceMaster");

                MessageBox.Show("New Region successfully saved", "Saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }
            else
            {
                clsSearch.ClassAdvanceSearchValue = txtID.Text + clsFunction.sPipe +
                                                    txtName.Text;

                Debug.WriteLine("UpdateRegionDetail::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                dbAPI.ExecuteAPI("PUT", "Update", "Region", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                MessageBox.Show("Region has been successfully modified", "Edited",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }

            btnClear_Click(this, e);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            fEdit = false;
            dbFunction.ClearTextBox(this);
            InitButton();
            SetInitMode(iInitMode.iDisable);
            LoadRegion();
        }

        private void frmRegion_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.SelectedItems[0].SubItems[1].Text.Length > 0)
            {
                string ID = lvwList.SelectedItems[0].SubItems[1].Text;
                string Name = lvwList.SelectedItems[0].SubItems[2].Text;

                txtID.Text = ID;
                txtName.Text = Name;

                if (txtID.Text.Length > 0 && txtName.Text.Length > 0)
                    fEdit = true;

                InitButton();
                SetInitMode(iInitMode.iEnable);
            }
        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void lvwList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
