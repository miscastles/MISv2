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
    public partial class frmExpenses : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;

        bool fEdit = false;
        public frmExpenses()
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
            txtDescription.Text = "";
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

            if (dbFunction.isValidDescription(txtDescription.Text))
                fValid = true;

            if (!fValid)
            {
                MessageBox.Show("Check the following field(s) listed below:\n\n" +
                                "*Description\n" +
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
            btnAdd.Enabled = false;
            btnSave.Enabled = true;
            SetInitMode(iInitMode.iEnable);
            txtDescription.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            fEdit = false;
            ClearTextBox();
            InitButton();
            SetInitMode(iInitMode.iDisable);
            LoadExpenses();
        }
        private void LoadExpenses()
        {
            int i = 0;

            ClearListView();
            dbAPI.ExecuteAPI("GET", "View", "", "", "Expenses", "", "ViewExpenses");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (clsExpenses.RecordFound)
            {
                while (clsArray.ExpensesID.Length > i)
                {                    
                    clsExpenses.ClassExpensesID = int.Parse(clsArray.ExpensesID[i].ToString());
                    clsExpenses.ClassDescription = clsArray.ExpensesDescription[i];

                    i++;

                    AddItem(i);
                }
            }

            dbFunction.ListViewAlternateBackColor(lvwList);

        }
        private void AddItem(int inLineNo)
        {
            // Add to List            
            ListViewItem item = new ListViewItem(inLineNo.ToString());
            item.SubItems.Add(clsExpenses.ClassExpensesID.ToString());
            item.SubItems.Add(clsExpenses.ClassDescription.ToString());
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
                sRowSQL = " ('" + txtDescription.Text + "') ";
                sSQL = sSQL + sRowSQL;

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "Expenses", sSQL, "InsertMaintenanceMaster");

                MessageBox.Show("New Expenses successfully saved", "Saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }
            else
            {
                clsSearch.ClassAdvanceSearchValue = txtID.Text + clsFunction.sPipe +
                                                    txtDescription.Text;

                Debug.WriteLine("UpdateTerminalModel::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                dbAPI.ExecuteAPI("PUT", "Update", "Expenses", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                MessageBox.Show("Expenses has been successfully modified", "Edited",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }

            btnClear_Click(this, e);
        }

        private void frmExpenses_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            ClearTextBox();
            InitButton();
            SetInitMode(iInitMode.iDisable);
            LoadExpenses();
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.SelectedItems[0].SubItems[1].Text.Length > 0)
            {
                string ID = lvwList.SelectedItems[0].SubItems[1].Text;
                string Description = lvwList.SelectedItems[0].SubItems[2].Text;

                txtID.Text = ID;
                txtDescription.Text = Description;

                if (txtID.Text.Length > 0 && txtDescription.Text.Length > 0)
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
                    txtDescription.Enabled = true;
                    txtDescription.BackColor = Color.White;
                    break;
                case iInitMode.iDisable:
                    txtDescription.Enabled = false;
                    txtDescription.BackColor = Color.LightGray;
                    break;
            }
        }

        private void frmExpenses_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void txtDescription_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }
    }
}
