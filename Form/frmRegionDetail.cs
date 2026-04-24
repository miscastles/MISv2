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
    public partial class frmRegionDetail : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;

        bool fEdit = false;
        public frmRegionDetail()
        {
            InitializeComponent();
        }
        private void ClearListView()
        {
            lvwList.Items.Clear();
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
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iProvince, txtName.Text)) return false;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRegion, txtRegionType.Text)) return false;
            
            return true;
        }

        private bool CheckRegionDetail()
        {
            bool fExist = false;
            string sRegionDetail = cboRegion.Text + clsFunction.sPipe + txtName.Text;

            fExist = dbAPI.CheckRegionDetail("Search", "Region Detail Check", sRegionDetail);

            if (fExist)
            {
                dbFunction.SetMessageBox("Unable to save Region Detail." +
                            "\n\n" +
                            "Region: " + cboRegion.Text +
                            "\n" +
                            "Province: " + txtName.Text +
                            "\n", "Already exist.", clsFunction.IconType.iWarning);


            }

            return fExist;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            fEdit = false;

            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);

            dbFunction.TextBoxUnLock(true, this);
            dbFunction.ComBoBoxUnLock(true, this);

            dbAPI.FillComboBoxRegion(cboRegion, "View", "Region", "", "Region");           
            dbAPI.GetRegionList("View", "", "", "Region");

            InitButton();
            btnSave.Enabled = true;
            btnAdd.Enabled = false;
            txtName.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            fEdit = false;

            txtName.Text = "";
            cboRegion.Text = "";

            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);

            InitButton();
            //LoadCity();
            ClearListView();
            LoadRegionDetail();
        }
        private void LoadCity()
        {
            int i = 0;

            ClearListView();
            dbAPI.ExecuteAPI("GET", "View", "", "", "City", "", "ViewCity");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (clsCity.RecordFound)
            {
                while (clsArray.CityID.Length > i)
                {
                    //clsProductInfo.ResetProductInfo();
                    clsCity.ClassCityID = int.Parse(clsArray.CityID[i].ToString());
                    clsCity.ClassCity = clsArray.City[i];

                    i++;

                    AddItem(i);
                }
                dbFunction.ListViewAlternateBackColor(lvwList);

            }

        }

        private void LoadRegionDetail()
        {
            int i = 0;

            ClearListView();
            dbAPI.ExecuteAPI("GET", "View", "", "", "Region Detail", "", "ViewRegionDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            while (clsArray.RegionID.Length > i)
            {
                clsRegionDetail.ClassRegionID = int.Parse(clsArray.RegionID[i].ToString());
                clsRegionDetail.ClassRegionType = int.Parse(clsArray.RegionType[i].ToString());
                clsRegionDetail.ClassProvince = clsArray.RegionProvince[i];
                clsRegionDetail.ClassRegion = clsArray.Region[i];

                i++;

                AddItem(i);
            }

            dbFunction.ListViewAlternateBackColor(lvwList);
        }
        private void AddItem(int inLineNo)
        {
            // Add to List            
            ListViewItem item = new ListViewItem(inLineNo.ToString());
            item.SubItems.Add(clsRegionDetail.ClassRegionID.ToString());
            item.SubItems.Add(clsRegionDetail.ClassRegionType.ToString());
            item.SubItems.Add(clsRegionDetail.ClassProvince);
            item.SubItems.Add(clsRegionDetail.ClassRegion);
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

                ID = int.Parse(txtRegionID.Text);
            }

            if (!fEdit)
            {
                // Check Region Detail
                //if (CheckRegionDetail()) return;
                if (dbAPI.isRecordExist("Search", "Region Detail", dbFunction.CheckAndSetStringValue(txtName.Text)))
                {
                    dbFunction.SetMessageBox("Province " + dbFunction.AddBracketStartEnd(txtName.Text) + " already exist.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return;
                }

                // Save Region Detail
                sSQL = "";
                sRowSQL = "";
                sRowSQL = " ('" + dbFunction.CheckAndSetNumericValue(txtRegionType.Text) + "', " +                
                sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtName.Text) + "') ";
                sSQL = sSQL + sRowSQL;

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "Region Detail", sSQL, "InsertMaintenanceMaster");

                MessageBox.Show("New Province/Region successfully saved", "Saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }
            else
            {
                clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetStringValue(txtRegionID.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtRegionType.Text) + clsFunction.sPipe +
                                                    dbFunction.CheckAndSetStringValue(txtName.Text);

                Debug.WriteLine("UpdateRegionDetail::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                dbAPI.ExecuteAPI("PUT", "Update", "Region Detail", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                MessageBox.Show("Province/Region has been successfully modified", "Edited",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            }

            btnClear_Click(this, e);
        }

        private void frmCity_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);

            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);

            InitButton();
            //LoadCity();

            ClearListView();
            LoadRegionDetail();

            dbAPI.FillComboBoxRegion(cboRegion, "View", "Region", "", "Region");
            dbAPI.GetRegionList("View", "", "", "Region");
            
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.SelectedItems[0].SubItems[1].Text.Length > 0)
            {
                string RegionID = lvwList.SelectedItems[0].SubItems[1].Text;
                string RegionType = lvwList.SelectedItems[0].SubItems[2].Text;
                string Name = lvwList.SelectedItems[0].SubItems[3].Text;
                string Region = lvwList.SelectedItems[0].SubItems[4].Text;

                txtRegionID.Text = RegionID;
                txtRegionType.Text = RegionType;
                txtName.Text = Name;
                cboRegion.Text = Region;

                if (txtRegionID.Text.Length > 0 && txtName.Text.Length > 0 && txtRegionType.Text.Length > 0 && cboRegion.Text.Length > 0)
                    fEdit = true;
                
                InitButton();
                dbFunction.TextBoxUnLock(true, this);
                dbFunction.ComBoBoxUnLock(true, this);
            }
        }

        private void frmCity_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void cboRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtRegionType.Text = dbAPI.GetRegionFromList(cboRegion.Text).ToString();
        }

        private void lvwList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {            
            if (!dbFunction.isValidateEntry(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }
    }
}
