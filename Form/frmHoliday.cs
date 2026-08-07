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

        public static class Holiday
        {
            public sealed class Month
            {
                public string Number { get; private set; }
                public string Name { get; private set; }
                public int MaxDay { get; private set; }

                public Month(string number, string name, int maxDay)
                {
                    Number = number;
                    Name = name;
                    MaxDay = maxDay;
                }
            }

            public static readonly Month[] Months =
            {
                new Month("01", "JANUARY",   31),
                new Month("02", "FEBRUARY",  29),
                new Month("03", "MARCH",     31),
                new Month("04", "APRIL",     30),
                new Month("05", "MAY",       31),
                new Month("06", "JUNE",      30),
                new Month("07", "JULY",      31),
                new Month("08", "AUGUST",    31),
                new Month("09", "SEPTEMBER", 30),
                new Month("10", "OCTOBER",   31),
                new Month("11", "NOVEMBER",  30),
                new Month("12", "DECEMBER",  31)
            };
        }

        private void frmHoliday_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            fEdit = false;
            dbFunction.ClearTextBox(this);
            dbFunction.ClearListViewItems(lvwList);
            dbFunction.TextBoxUnLock(false, this);

            InitHolidayMonth();
            InitHolidayMonthFilter();

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

            if (txtDescription.TextLength == 0)
            {
                fValid = false;
            }

            if (cboMM.SelectedIndex == 0)
            {
                fValid = true;
            }

            if (cboDD.SelectedIndex == 0)
            {
                fValid = true;
            }

            if (txtDescription.TextLength > 0 && (cboMM.SelectedIndex >= 0 && cboDD.SelectedIndex >= 0))
            {
                fValid = true;
            }

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

            cboMM.Enabled = true;
            cboDD.Enabled = true;

            cboMM.SelectedIndex = -1;

            cboDD.Items.Clear();
            cboDD.SelectedIndex = -1;
            cboDD.Text = "";

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

            cboMM.SelectedIndex = -1;
            cboMM.Enabled = false;

            cboDD.Items.Clear();
            cboDD.SelectedIndex = -1;
            cboDD.Text = "";
            cboDD.Enabled = false;

            cboMonthFilter.SelectedIndexChanged -= cboMonthFilter_SelectedIndexChanged;

            if (cboMonthFilter.Items.Count > 0)
            {
                cboMonthFilter.SelectedIndex = 0;
            }

            cboMonthFilter.SelectedIndexChanged += cboMonthFilter_SelectedIndexChanged;

            InitButton();
            LoadData();
        }

        private void LoadData()
        {
            int i = 0;
            string sFilter = "";

            dbFunction.ClearListViewItems(lvwList);

            if (cboMonthFilter.SelectedIndex > 0 &&
                cboMonthFilter.Text.Length >= 2)
            {
                sFilter = cboMonthFilter.Text.Substring(0, 2);
            }

            clsSearch.ClassSearchValue = clsFunction.sZero + clsFunction.sPipe + sFilter;
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
                if (dbAPI.isRecordExist("Search", "Holiday", $"{txtDescription.Text}{clsDefines.gPipe}{cboMM.SelectedValue}-{cboDD.Text}"))
                {
                    dbFunction.SetMessageBox($"Holiday {dbFunction.AddBracketStartEnd(txtDescription.Text)} with Date {dbFunction.AddBracketStartEnd($"{cboMM.SelectedValue}-{cboDD.Text}")} already exist.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
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
                sRowSQL = " ('" + dbFunction.CheckAndSetStringValue($"{cboMM.SelectedValue}-{cboDD.Text}") + "', " +
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
                clsSearch.ClassAdvanceSearchValue =
                    txtID.Text + clsFunction.sPipe +
                    $"{cboMM.SelectedValue}-{cboDD.Text}" + clsFunction.sPipe +
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
            if (lvwList.SelectedItems.Count == 0) return;
            ListViewItem selectedItem = lvwList.SelectedItems[0];

            if (selectedItem.SubItems[1].Text.Length == 0) return;

            fEdit = true;
            txtID.Text = selectedItem.SubItems[1].Text;

            InitButton();

            dbFunction.TextBoxUnLock(true, this);

            cboMM.Enabled = true;
            cboDD.Enabled = true;

            cboMM.SelectedIndex = -1;

            cboDD.Items.Clear();
            cboDD.SelectedIndex = -1;
            cboDD.Text = "";

            txtDescription.Text = selectedItem.SubItems[2].Text;
            string holidayDate = selectedItem.SubItems[3].Text;

            string[] dateParts = holidayDate.Split('-');

            if (dateParts.Length == 2)
            {
                cboMM.SelectedValue = dateParts[0];
                cboDD.SelectedItem = dateParts[1];
            }
        }
        private void InitHolidayMonth()
        {
            cboMM.DisplayMember = "Name";
            cboMM.ValueMember = "Number";
            cboMM.DataSource = Holiday.Months;

            cboMM.SelectedIndexChanged += cboMM_SelectedIndexChanged;
            LoadHolidayDays();
        }

        private void cboMM_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadHolidayDays();
        }

        private void LoadHolidayDays()
        {
            cboDD.Items.Clear();
            cboDD.SelectedIndex = -1;
            cboDD.Text = "";

            Holiday.Month selectedMonth = cboMM.SelectedItem as Holiday.Month;

            if (selectedMonth == null) return;

            for (int day = 1; day <= selectedMonth.MaxDay; day++) cboDD.Items.Add(day.ToString("00"));

            if (cboDD.Items.Count > 0) 
            {
                cboDD.SelectedIndex = 0;
            }
        }

        private void InitHolidayMonthFilter()
        {
            cboMonthFilter.Items.Clear();

            cboMonthFilter.Items.Add(clsFunction.sDefaultSelect);

            foreach (Holiday.Month month in Holiday.Months)
            {
                cboMonthFilter.Items.Add(
                    $"{month.Number} - {month.Name}");
            }

            cboMonthFilter.SelectedIndex = 0;

            cboMonthFilter.SelectedIndexChanged -= cboMonthFilter_SelectedIndexChanged;

            cboMonthFilter.SelectedIndexChanged += cboMonthFilter_SelectedIndexChanged;
        }

        private void cboMonthFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
