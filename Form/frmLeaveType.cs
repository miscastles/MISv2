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
using MIS.Model;

namespace MIS
{
    public partial class frmLeaveType : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        public static string sHeader;

        bool fEdit = false;

        public frmLeaveType()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmLeaveType_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            fEdit = false;
            dbFunction.ClearTextBox(this);
            dbFunction.ClearListViewItems(lvwList);
            dbFunction.TextBoxUnLock(false, this);
            InitButton();
            LoadData();
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

            if (txtDescription.TextLength > 0 && txtCreditLimit.TextLength > 0)
                fValid = true;

            if (!fValid)
            {
                MessageBox.Show("Check the following field(s) listed below:\n\n" +
                                "*Description\n" +
                                "*Credit Limit)\n" +
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

            txtDescription.BackColor = clsFunction.MKBackColor;
            txtDescription.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            fEdit = false;
            dbFunction.ClearTextBox(this);
            dbFunction.ClearListViewItems(lvwList);
            dbFunction.TextBoxUnLock(false, this);
            InitButton();
            LoadData();
        }
        private void LoadData()
        {
            int i = 0;

            dbFunction.ClearListViewItems(lvwList);

            clsSearch.ClassSearchValue = clsFunction.sZero + clsFunction.sPipe;
            dbAPI.ExecuteAPI("GET", "View", "LeaveType List", clsSearch.ClassSearchValue, "LeaveType", "", "ViewLeaveType");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.LeaveTypeID.Length > i)
                {
                    clsLeaveType.ClassLeaveTypeID = int.Parse(clsArray.LeaveTypeID[i]);
                    clsLeaveType.ClassCode = clsArray.LeaveTypeCode[i];
                    clsLeaveType.ClassDescription = clsArray.LeaveTypeDesc[i];
                    clsLeaveType.ClassCreditLimit = double.Parse(clsArray.LeaveTypeCreditLimit[i]);                    

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
            item.SubItems.Add(clsLeaveType.ClassLeaveTypeID.ToString());
            item.SubItems.Add(clsLeaveType.ClassCode.ToString());
            item.SubItems.Add(clsLeaveType.ClassDescription.ToString());
            item.SubItems.Add(clsLeaveType.ClassCreditLimit.ToString());
            lvwList.Items.Add(item);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }
    }
}
