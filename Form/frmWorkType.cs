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
    public partial class frmWorkType : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        public static string sHeader;

        bool fEdit = false;

        public frmWorkType()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmWorkType_Load(object sender, EventArgs e)
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

        private void LoadData()
        {
            int inLineNo = 0;
            int i = 0;

            dbFunction.ClearListViewItems(lvwList);

            clsSearch.ClassSearchValue = clsFunction.isActive + clsFunction.sPipe;
            dbAPI.ExecuteAPI("GET", "View", "WorkType", clsSearch.ClassSearchValue, "Type", "", "ViewType");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.WorkTypeID.Length > i)
                {
                    // Add to List 
                    inLineNo++;

                    ListViewItem item = new ListViewItem(inLineNo.ToString());
                    item.SubItems.Add(clsArray.WorkTypeID[i]);
                    item.SubItems.Add(clsArray.Code[i]);
                    item.SubItems.Add(clsArray.Description[i]);                    
                    lvwList.Items.Add(item);

                    i++;                    
                }
            }

            dbFunction.ListViewAlternateBackColor(lvwList);

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

        private void btnSave_Click(object sender, EventArgs e)
        {

        }
    }
}
