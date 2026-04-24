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
    public partial class frmProvince : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;

        bool fEdit = false;
        public frmProvince()
        {
            InitializeComponent();
        }
        private void ClearListView()
        {
            lvwList.Items.Clear();
        }
        private void ClearTextBox()
        {
            txtID.Text = "0";
            txtName.Text = "";
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
            ClearTextBox();
            InitButton();
            SetInitMode(iInitMode.iEnable);
            btnSave.Enabled = true;
            btnAdd.Enabled = false;
            txtName.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            fEdit = false;
            ClearTextBox();
            InitButton();
            SetInitMode(iInitMode.iDisable);
            LoadProvince();
        }
        private void LoadProvince()
        {
            int i = 0;

            ClearListView();
            dbAPI.ExecuteAPI("GET", "View", "", "", "Province", "", "ViewProvince");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (clsProvince.RecordFound)
            {
                while (clsArray.ProvinceID.Length > i)
                {
                    //clsProductInfo.ResetProductInfo();
                    clsProvince.ClassProvinceID = int.Parse(clsArray.ProvinceID[i].ToString());
                    clsProvince.ClassProvince = clsArray.Province[i];

                    i++;

                    AddItem(i);
                }

                dbFunction.ListViewAlternateBackColor(lvwList);
            }

        }
        private void AddItem(int inLineNo)
        {
            // Add to List            
            ListViewItem item = new ListViewItem(inLineNo.ToString());
            item.SubItems.Add(clsProvince.ClassProvinceID.ToString());
            item.SubItems.Add(clsProvince.ClassProvince.ToString());
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
                sRowSQL = "";
                sRowSQL = " ('" + txtName.Text + "') ";
                sSQL = sSQL + sRowSQL;

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "Province", sSQL, "InsertMaintenanceMaster");

                MessageBox.Show("New Province successfully saved", "Saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }
            else
            {
                clsProvince.ClassProvinceID = int.Parse(txtID.Text);
                clsProvince.ClassProvince = txtName.Text;

                dbAPI.ExecuteAPI("PUT", "Update", "", "", "Province", "", "UpdateProvince");

                MessageBox.Show("Province has been successfully modified", "Edited",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }

            btnClear_Click(this, e);
        }

        private void frmProvince_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            ClearTextBox();
            InitButton();
            SetInitMode(iInitMode.iDisable);
            LoadProvince();
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
        private enum iInitMode
        {
            iEnable,
            iDisable
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

        private void frmProvince_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
