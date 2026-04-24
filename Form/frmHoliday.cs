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
using static MIS.Function.AppUtilities;

namespace MIS
{
    public partial class frmHoliday : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        public static string sHeader;

        bool fEdit = false;

        public frmHoliday()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmHoliday_Load(object sender, EventArgs e)
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

            if (txtDescription.TextLength > 0 && txtMothDay.TextLength > 0)
                fValid = true;

            if (!fValid)
            {
                MessageBox.Show("Check the following field(s) listed below:\n\n" +                                
                                "*Description\n" +
                                "*Date (MM-DD)\n" +
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
            dbAPI.ExecuteAPI("GET", "View", "Holiday List", clsSearch.ClassSearchValue, "Holiday", "", "ViewHoliday");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.HolidayID.Length > i)
                {
                    clsHoliday.ClassHolidayID = int.Parse(clsArray.HolidayID[i]);
                    clsHoliday.ClassDescription = clsArray.HolidayDesc[i];
                    clsHoliday.ClassHolidayDate = clsArray.HolidayDate[i];
                    clsHoliday.ClassisActive = int.Parse(clsArray.HolidayisActive[i]);

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
            item.SubItems.Add(clsHoliday.ClassHolidayID.ToString());
            item.SubItems.Add(clsHoliday.ClassDescription.ToString());
            item.SubItems.Add(clsHoliday.ClassHolidayDate.ToString());
            item.SubItems.Add(clsHoliday.ClassisActive.ToString());
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
                // check holiday exist
                if (dbAPI.isRecordExist("Search", "Holiday", $"{txtDescription.Text}{clsDefines.gPipe}{txtMothDay.Text}"))
                {
                    dbFunction.SetMessageBox($"Holiday {dbFunction.AddBracketStartEnd(txtDescription.Text)} with Date {dbFunction.AddBracketStartEnd(txtMothDay.Text)} already exist.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return;
                }

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
                sRowSQL = " ('" + dbFunction.CheckAndSetStringValue(txtMothDay.Text) + "', " +
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(StrClean(txtDescription.Text)) + "') ";
                sSQL = sSQL + sRowSQL;

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "Holiday", sSQL, "InsertMaintenanceMaster");

                MessageBox.Show("New Holiday successfully saved", "Saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }
            else
            {
                clsSearch.ClassAdvanceSearchValue = txtID.Text + clsFunction.sPipe +
                                                txtMothDay.Text + clsFunction.sPipe +
                                                dbFunction.CheckAndSetStringValue(StrClean(txtDescription.Text));
                
                dbAPI.ExecuteAPI("PUT", "Update", "Update Holiday", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                MessageBox.Show("Holiday has been successfully modified", "Edited",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }

            btnClear_Click(this, e);
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
                    
                }
            }
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.SelectedItems[0].SubItems[1].Text.Length > 0)
            {
                if (txtID.Text.Length > 0)
                    fEdit = true;

                InitButton();

                dbFunction.TextBoxUnLock(true, this);

                txtDescription.Text = lvwList.SelectedItems[0].SubItems[2].Text;
                txtMothDay.Text = lvwList.SelectedItems[0].SubItems[3].Text;

            }
        }
    }
}
