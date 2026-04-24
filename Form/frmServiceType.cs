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
    public partial class frmServiceType : Form
    {        
        private clsAPI dbAPI;
        private clsFunction dbFunction;
               
        bool fEdit = false;

        public frmServiceType()
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
            txtCode.Text = "";         
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

            if (txtDescription.TextLength > 0 && txtCode.TextLength > 0)
                fValid = true;

            if (!fValid)
            {
                MessageBox.Show("Check the following field(s) listed below:\n\n" +
                                "*Description\n" +
                                "*Code\n" +
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
            btnSave.Enabled = true;
            btnAdd.Enabled = false;
            SetInitMode(iInitMode.iEnable);
            btnSave.Enabled = true;
            txtDescription.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            fEdit = false;
            ClearTextBox();
            InitButton();
            SetInitMode(iInitMode.iDisable);
            LoadServiceType();
        }
        private void LoadServiceType()
        {
            int i = 0;

            ClearListView();            
            dbAPI.ExecuteAPI("GET", "View", "Service Type Enrollment", "", "Service Type", "", "ViewServiceType");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (clsServiceType.RecordFound)
            {
                while (clsArray.ServiceTypeID.Length > i)
                {
                    //clsProductInfo.ResetProductInfo();
                    clsServiceType.ClassServiceTypeID = int.Parse(clsArray.ServiceTypeID[i].ToString());
                    clsServiceType.ClassDescription = clsArray.Description[i];
                    clsServiceType.ClassCode = clsArray.Code[i];

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
            item.SubItems.Add(clsServiceType.ClassServiceTypeID.ToString());
            item.SubItems.Add(clsServiceType.ClassDescription.ToString());
            item.SubItems.Add(clsServiceType.ClassCode.ToString());
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
                sRowSQL = " ('" + txtDescription.Text + "', " +                
                sRowSQL + sRowSQL + " '" + txtCode.Text + "') ";
                sSQL = sSQL + sRowSQL;

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "Service Type", sSQL, "InsertMaintenanceMaster");

                MessageBox.Show("New Service Type successfully saved", "Saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }
            else
            {                
                clsServiceType.ClassServiceTypeID = int.Parse(txtID.Text);
                clsServiceType.ClassDescription = txtDescription.Text;
                clsServiceType.ClassCode = txtCode.Text;

                dbAPI.ExecuteAPI("PUT", "Update", "", "", "Service Type", "", "UpdateServiceType");

                MessageBox.Show("Service Type has been successfully modified", "Edited",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }

            btnClear_Click(this, e);
        }

        private void frmServiceType_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            ClearTextBox();
            InitButton();
            SetInitMode(iInitMode.iDisable);
            LoadServiceType();
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {            

            if (lvwList.SelectedItems[0].SubItems[1].Text.Length > 0)
            {
                string ID = lvwList.SelectedItems[0].SubItems[1].Text;
                string Description = lvwList.SelectedItems[0].SubItems[2].Text;
                string Code = lvwList.SelectedItems[0].SubItems[3].Text;

                txtID.Text = ID;
                txtDescription.Text = Description;
                txtCode.Text = Code;

                if (txtID.Text.Length > 0)
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
                    txtCode.Enabled = true;
                    txtDescription.BackColor = Color.White;
                    txtCode.BackColor = Color.White;
                    break;
                case iInitMode.iDisable:
                    txtDescription.Enabled = false;
                    txtCode.Enabled = false;
                    txtDescription.BackColor = Color.LightGray;
                    txtCode.BackColor = Color.LightGray;
                    break;
            }
        }

        private void frmServiceType_KeyDown(object sender, KeyEventArgs e)
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

        private void txtCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }
    }
}
