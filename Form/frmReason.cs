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
using MIS.Controller;

namespace MIS
{
    public partial class frmReason : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        public static string sHeader;
        bool fEdit = false;

        // Controller
        private ReasonController _mReasonController;

        public frmReason()
        {
            InitializeComponent();

            // Initialize the controller object
            _mReasonController = new ReasonController();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
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
                                "*Reason\n" +
                                "*Description\n" +
                                "\n" +
                                "Field(s) with asterisk(*) must not be blank.", "Incomplete Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return fValid;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            fEdit = false;
            dbFunction.ClearTextBox(this);            
            dbFunction.TextBoxUnLock(true, this);
            InitButton();
            btnSave.Enabled = true;
            btnAdd.Enabled = false;

            int iControlNo = dbAPI.GetControlID("Reason");
            string sControlNo = iControlNo.ToString();
            txtCode.Text = dbFunction.padLeftChar(sControlNo, clsFunction.sZero, 4);
            txtCode.BackColor = clsFunction.DisableBackColor;

            txtDescription.Focus();
        }

        private void frmReason_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            fEdit = false;
            dbFunction.ClearTextBox(this);
            dbFunction.ClearListViewItems(lvwList);            
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ClearComboBox(this);
            InitButton();
            
            dbAPI.FillComboBoxReasonType(cboType);
            cboType.Text = clsFunction.sDefaultSelect;

            lblHeader.Text = "ENROLLMENT - " + sHeader;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            fEdit = false;
            dbFunction.ClearTextBox(this);
            dbFunction.ClearListViewItems(lvwList);
            dbFunction.TextBoxUnLock(false, this);
            InitButton();
            
            cboType.Text = clsFunction.sDefaultSelect;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int ID = 0;
            string sRowSQL = "";
            string sSQL = "";            

            if (!ValidateFields()) return;
            
            if (!fEdit)
            {
                if (!CheckReasonCode(txtCode.Text)) return;

                if (!CheckReasonDescription(txtDescription.Text)) return;

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
                sRowSQL = " ('" + dbFunction.padLeftChar(txtCode.Text, clsFunction.sPadZero, 4) + "', " +
                sRowSQL + sRowSQL + " '" + txtDescription.Text + "', " +
                sRowSQL + sRowSQL + " '" + cboType.Text + "') ";
                sSQL = sSQL + sRowSQL;

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "Reason", sSQL, "InsertMaintenanceMaster");

                MessageBox.Show("New Reason with type "  + cboType.Text + " successfully saved", "Saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }
            else
            {
                clsSearch.ClassAdvanceSearchValue = txtID.Text + clsFunction.sPipe +
                                                txtCode.Text + clsFunction.sPipe +
                                                txtDescription.Text + clsFunction.sPipe +
                                                cboType.Text;

                Debug.WriteLine("SaveReasonDetail::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);
                
                dbAPI.ExecuteAPI("PUT", "Update", "Reason", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
                
                MessageBox.Show("Reason with type " + cboType.Text + " has been successfully modified", "Edited",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }

            btnClear_Click(this, e);
        }

        private void LoadReason()
        {
            int lineno = 0;
            int i = 0;
            List<ReasonController> mList = null;
            
            dbFunction.ClearListViewItems(lvwList);

            mList = _mReasonController.getDetailList(clsFunction.sZero + clsFunction.sPipe + cboType.Text);
            if (mList != null)
            {
                if (mList != null)
                {
                    foreach (var itemData in mList)
                    {
                        lineno++;
                        ListViewItem item = new ListViewItem(lineno.ToString());
                        item.SubItems.Add(itemData.ReasonID.ToString());
                        item.SubItems.Add(itemData.Code);
                        item.SubItems.Add(itemData.Description);
                        item.SubItems.Add(itemData.Type);

                        lvwList.Items.Add(item);

                    }

                    dbFunction.ListViewAlternateBackColor(lvwList);

                }
            }

        }
        private void AddItem(int inLineNo)
        {
            // Add to List            
            ListViewItem item = new ListViewItem(inLineNo.ToString());
            item.SubItems.Add(clsReason.ClassReasonID.ToString());
            item.SubItems.Add(clsReason.ClassReasonCode.ToString());
            item.SubItems.Add(clsReason.ClassReasonDescription.ToString());
            item.SubItems.Add(clsReason.ClassReasonType.ToString());
            lvwList.Items.Add(item);
        }

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    txtDescription.Focus();
                    break;
            }
        }

        private void txtCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private bool CheckReasonCode(string sCode)
        {
            bool fValid = true;

            foreach (ListViewItem i in lvwList.Items)
            {
                string sTemp = i.SubItems[2].Text;

                if (sTemp.CompareTo(sCode) == 0)
                {
                    fValid = false;
                    break;
                }

            }

            if (!fValid)
            {
                dbFunction.SetMessageBox("Reason code " + "[ " + txtCode.Text + " ]"+
                                          "\n\n" +
                                          "Already exist.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }

            return fValid;
        }

        private bool CheckReasonDescription(string sDescription)
        {
            bool fValid = true;

            foreach (ListViewItem i in lvwList.Items)
            {
                string sTemp = i.SubItems[3].Text;

                if (sTemp.CompareTo(sDescription) == 0)
                {
                    fValid = false;
                    break;
                }

            }

            if (!fValid)
            {
                dbFunction.SetMessageBox(clsSearch.ClassReasonType + " description " + "[ " + txtDescription.Text + " ]" +
                                          "\n\n" +
                                          "Already exist.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }

            return fValid;
        }

        private void lvwList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwList.SelectedItems.Count > 0)
            {
                string LineNo = lvwList.SelectedItems[0].Text;
                txtLineNo.Text = LineNo;

                if (LineNo.Length > 0)
                {
                    txtID.Text = lvwList.SelectedItems[0].SubItems[1].Text;
                    txtCode.Text = lvwList.SelectedItems[0].SubItems[2].Text;
                    txtDescription.Text = lvwList.SelectedItems[0].SubItems[3].Text;
                }
            }
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.SelectedItems[0].SubItems[1].Text.Length > 0)
            {
                if (txtID.Text.Length > 0 && txtCode.Text.Length > 0)
                    fEdit = true;

                InitButton();

                dbFunction.TextBoxUnLock(true, this);
                txtCode.BackColor = clsFunction.DisableBackColor;
            }
        }

        private void frmReason_KeyDown(object sender, KeyEventArgs e)
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

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void cboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cboType.Text.Equals(clsFunction.sDefaultSelect))
            {   
                LoadReason();
            }
        }
    }
}
