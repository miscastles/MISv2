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
    public partial class frmTerminalModel : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;

        bool fEdit = false;

        public frmTerminalModel()
        {
            InitializeComponent();
        }
        private void ClearListView()
        {
            lvwList.Items.Clear();
        }
        private void ClearTextBox()
        {
            txtID.Text = clsFunction.sZero;
            txtDescription.Text = "";
            txtTypeID.Text = clsFunction.sZero;
            txtType.Text = "";
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

            if (txtDescription.TextLength > 0 && txtType.TextLength > 0)
                fValid = true;

            if (!fValid)
            {
                MessageBox.Show("Check the following field(s) listed below:\n\n" +
                                "*Description\n" +
                                "*Type\n" +
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
            LoadTerminalModel();
        }
        private void LoadTerminalModel()
        {
            int i = 0;

            ClearListView();
            dbAPI.ExecuteAPI("GET", "View", "", "", "Terminal Model", "", "ViewTerminalModel");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (clsTerminalModel.RecordFound)
            {
                while (clsArray.TerminalModelID.Length > i)
                {
                    //clsProductInfo.ResetProductInfo();
                    clsTerminalModel.ClassTerminalModelID = int.Parse(clsArray.TerminalModelID[i].ToString());
                    clsTerminalModel.ClassDescription = clsArray.Description[i];
                    clsTerminalModel.ClassTerminalTypeID = int.Parse(clsArray.TerminalTypeID[i].ToString());
                    clsTerminalModel.ClassTypeDescription = clsArray.TypeDescription[i];

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
            item.SubItems.Add(clsTerminalModel.ClassTerminalModelID.ToString());
            item.SubItems.Add(clsTerminalModel.ClassDescription.ToString());
            item.SubItems.Add(clsTerminalModel.ClassTerminalTypeID.ToString());
            item.SubItems.Add(clsTerminalModel.ClassTypeDescription.ToString());

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
                sSQL = "";
                sRowSQL = "";
                sRowSQL = "('" + txtDescription.Text + "'," +                
                sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetNumericValue(txtTypeID.Text) + "') ";
                sSQL = sSQL + sRowSQL;


                dbAPI.ExecuteAPI("POST", "Insert", "", "", "Terminal Model", sSQL, "InsertMaintenanceMaster");

                MessageBox.Show("New Terminal Model successfully saved", "Saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }
            else
            {
                clsSearch.ClassAdvanceSearchValue = txtID.Text + clsFunction.sPipe +
                                                    txtDescription.Text + clsFunction.sPipe + txtTypeID.Text;

                Debug.WriteLine("UpdateTerminalModel::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                dbAPI.ExecuteAPI("PUT", "Update", "Terminal Model", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                MessageBox.Show("Terminal Model has been successfully modified", "Edited",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }

            btnClear_Click(this, e);
        }

        private void frmTerminalModel_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            ClearTextBox();
            InitButton();
            SetInitMode(iInitMode.iDisable);
            LoadTerminalModel();
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.SelectedItems[0].SubItems[1].Text.Length > 0)
            {
                string ID = lvwList.SelectedItems[0].SubItems[1].Text;
                string Description = lvwList.SelectedItems[0].SubItems[2].Text;

                string TypeID = lvwList.SelectedItems[0].SubItems[3].Text;
                string TypeDescription = lvwList.SelectedItems[0].SubItems[4].Text;

                // Terminal Model
                txtID.Text = ID;
                txtDescription.Text = Description;

                // Terminal Type
                txtTypeID.Text = TypeID;
                txtType.Text = TypeDescription;

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

        private void frmTerminalModel_KeyDown(object sender, KeyEventArgs e)
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

        private void btnSearchTerminalType_Click(object sender, EventArgs e)
        {
            dbAPI.ResetAdvanceSearch();
            frmSearchField.iSearchType = frmSearchField.SearchType.iTerminalType;
            frmSearchField.iStatus = clsFunction.iZero;
            frmSearchField.sHeader = "VIEW TERMINAL TYPE";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {                
                txtTypeID.Text = clsSearch.ClassTerminalTypeID.ToString();
                txtType.Text = clsSearch.ClassTerminalType;
            }
        }

        private void lvwList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
