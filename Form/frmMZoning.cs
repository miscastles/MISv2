using MIS.Controller;
using MIS.Function;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MIS.Function.AppUtilities;

namespace MIS
{
    public partial class frmMZoning : Form
    {
        private clsAPI dbAPI;
        private clsINI dbSetting;
        private clsFile dbFile;
        private clsFunction dbFunction;

        public static string sHeader;
        bool fEdit = false;
        string tabIndex = clsFunction.sZero;

        protected override CreateParams CreateParams
        {
            // Override CreateParams to enable double-buffering for child controls
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED
                //cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        public frmMZoning()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwZoning, true);
            dbFunction.setDoubleBuffer(lvwZoningAlias, true);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmMZoning_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbSetting = new clsINI();
            dbFile = new clsFile();
            dbFunction = new clsFunction();

            dbFunction.ClearTextBox(this);            
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);

            dbAPI.FillComboBoxZonnigLookup(cboZone, "Zone");
            dbAPI.FillComboBoxZonnigLookup(cboCluster, "Cluster");
            dbAPI.FillComboBoxZonnigLookup(cboRegion, "Region");
            dbAPI.FillComboBoxZonnigLookup(cboArea, "Area");
            dbAPI.FillComboBoxZonnigLookup(cboCityMunicipal, "City");

            // register all combobox entry will be uppercase
            clsComboBox.RegisterUpperCase(cboCluster);
            clsComboBox.RegisterUpperCase(cboZone);
            clsComboBox.RegisterUpperCase(cboRegion);
            clsComboBox.RegisterUpperCase(cboArea);
            clsComboBox.RegisterUpperCase(cboCityMunicipal);

            loadData();

            fEdit = false;
            InitButton();

            ComboBoxDefaultSelect();

            initSearchTextBox(true);
            txtSearch.Focus();

            Cursor.Current = Cursors.Default;
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            fEdit = false;
            dbFunction.ClearTextBox(this);
            InitButton();
            btnSave.Enabled = true;
            btnAdd.Enabled = false;
           
            dbFunction.TextBoxUnLock(true, this);
            dbFunction.ComBoBoxUnLock(true, this);

            initSearchTextBox(false);

            switch (int.Parse(tabIndex))
            {
                case 0:
                    txtSLA.Text = "100";
                    cboCluster.Focus();
                    break;
                case 1:
                    chkIsWholeWord.Checked = chkIsActive.Checked = true;
                    txtAliasName.Focus();
                    break;
            }
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            int ID = 0;
            string sRowSQL = "";
            string sSQL = "";
            string sSearchValue = "";

            if (!ValidateFields()) return;

            if (!fEdit)
            {
                if (!dbFunction.fSavingConfirm(false)) return;

            }
            else
            {
                if (!dbFunction.fSavingConfirm(true)) return;

                switch (int.Parse(tabIndex))
                {
                    case 0:
                        ID = int.Parse(txtID.Text);
                        break;

                    case 1:
                        ID = int.Parse(txtAliasID.Text);
                        break;
                }
            }

            switch (int.Parse(tabIndex))
            {
                case 0:
                    if (!fEdit)
                    {
                        sSearchValue = $"{cboCluster.Text}{clsFunction.sPipe}" +
                                        $"{cboRegion.Text}{clsFunction.sPipe}" +
                                        $"{cboArea.Text}{clsFunction.sPipe}" +
                                        $"{cboCityMunicipal.Text}{clsFunction.sPipe}" +
                                        $"{cboZone.Text}";

                        if (dbAPI.isRecordExist("Search", "Zoning", sSearchValue))
                        {
                            dbFunction.SetMessageBox("Zoning details already exist.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                            return;
                        }

                        // Insert
                        sRowSQL = "";
                        sSQL = "";
                        sRowSQL = " ('" + StrClean(dbFunction.CheckAndSetStringValue(cboCluster.Text)) + "', " +
                        sRowSQL + sRowSQL + "'" + StrClean(dbFunction.CheckAndSetStringValue(cboRegion.Text)) + "'," +
                        sRowSQL + sRowSQL + "'" + StrClean(dbFunction.CheckAndSetStringValue(cboArea.Text)) + "'," +
                        sRowSQL + sRowSQL + "'" + StrClean(dbFunction.CheckAndSetStringValue(cboCityMunicipal.Text)) + "'," +
                        sRowSQL + sRowSQL + "'" + StrClean(dbFunction.CheckAndSetStringValue(cboZone.Text)) + "'," +
                        sRowSQL + sRowSQL + "'" + StrClean(dbFunction.CheckAndSetStringValue(txtSLA.Text)) + "') ";
                        sSQL = sSQL + sRowSQL;

                        Debug.WriteLine("Update::" + "sSQL=" + sSQL);

                        dbAPI.ExecuteAPI("POST", "Insert", "", "", "Zoning", sSQL, "InsertMaintenanceMaster");

                        MessageBox.Show("New Zoning successfully saved", "Saved",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1);
                    }
                    else
                    {
                        clsSearch.ClassAdvanceSearchValue = $"{txtID.Text}{clsFunction.sPipe}" +
                            $"{StrClean(dbFunction.CheckAndSetStringValue(cboCluster.Text))}{clsFunction.sPipe}" +
                            $"{StrClean(dbFunction.CheckAndSetStringValue(cboRegion.Text))}{clsFunction.sPipe}" +
                            $"{StrClean(dbFunction.CheckAndSetStringValue(cboArea.Text))}{clsFunction.sPipe}" +
                            $"{StrClean(dbFunction.CheckAndSetStringValue(cboCityMunicipal.Text))}{clsFunction.sPipe}" +
                            $"{StrClean(dbFunction.CheckAndSetStringValue(cboZone.Text))}{clsFunction.sPipe}" +
                            $"{StrClean(dbFunction.CheckAndSetStringValue(txtSLA.Text))}";

                        Debug.WriteLine("Insert::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                        dbAPI.ExecuteAPI("PUT", "Update", "Zoning", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                        MessageBox.Show("Zoning has been successfully modified", "Edited",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1);
                    }

                    break;
                case 1:
                    if (!fEdit)
                    {
                        sSearchValue =
                            $"{txtAliasName.Text}{clsFunction.sPipe}" +
                            $"{txtOfficialName.Text}";

                        if (dbAPI.isRecordExist("Search", "Zoning Alias", sSearchValue))
                        {
                            dbFunction.SetMessageBox("Zoning alias details already exist.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                            return;
                        }

                        // Insert
                        sRowSQL = "";
                        sSQL = "";
                        sRowSQL = sRowSQL +" ('" + StrClean(dbFunction.CheckAndSetStringValue(txtAliasName.Text)) + "', ";
                        sRowSQL = sRowSQL + "'" + StrClean(dbFunction.CheckAndSetStringValue(txtOfficialName.Text)) + "', ";
                        sRowSQL = sRowSQL + dbFunction.CheckAndSetNumericValue(txtPriority.Text) + ", ";
                        sRowSQL = sRowSQL + dbFunction.CheckAndSetBooleanValue(chkIsWholeWord.Checked) + ", ";
                        sRowSQL = sRowSQL + dbFunction.CheckAndSetBooleanValue(chkIsActive.Checked) + ") ";

                        sSQL = sSQL + sRowSQL;

                        Debug.WriteLine("Update::" + "sSQL=" + sSQL);

                        dbAPI.ExecuteAPI("POST", "Insert", "", "", "Zoning Alias", sSQL, "InsertMaintenanceMaster");

                        MessageBox.Show("New zoning alias successfully saved", "Saved",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1);
                    }
                    else
                    {
                        clsSearch.ClassAdvanceSearchValue =
                            $"{txtAliasID.Text}{clsFunction.sPipe}" +
                            $"{StrClean(dbFunction.CheckAndSetStringValue(txtAliasName.Text))}{clsFunction.sPipe}" +
                            $"{StrClean(dbFunction.CheckAndSetStringValue(txtOfficialName.Text))}{clsFunction.sPipe}" +
                            $"{dbFunction.CheckAndSetNumericValue(txtPriority.Text)}{clsFunction.sPipe}" +
                            $"{dbFunction.CheckAndSetBooleanValue(chkIsWholeWord.Checked)}{clsFunction.sPipe}" +
                            $"{dbFunction.CheckAndSetBooleanValue(chkIsActive.Checked)}";

                        Debug.WriteLine("Insert::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                        dbAPI.ExecuteAPI("PUT", "Update", "Zoning Alias", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                        MessageBox.Show("Zoning alias has been successfully modified", "Edited",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1);
                    }

                    break;
            }

            Cursor.Current = Cursors.Default;
            btnRefresh_Click(this, e);
            btnClear_Click(this, e);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbFunction.ClearTextBox(this);            
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);

            fEdit = false;            
            InitButton();

            ComboBoxDefaultSelect();

            initSearchTextBox(true);

            chkIsWholeWord.Checked = chkIsActive.Checked = false;

            txtSearch.Focus();

            Cursor.Current = Cursors.Default;
        }

        private void ComboBoxDefaultSelect()
        {
            cboZone.Text = cboCluster.Text = cboRegion.Text = cboArea.Text = cboCityMunicipal.Text = clsFunction.sDefaultSelect;
        }

        private void loadData()
        {
            try
            {
            Cursor.Current = Cursors.WaitCursor;

                int i = 0;
                int iLineNo = 0;

                switch (int.Parse(tabIndex))
                {
                    case 0:
                            lvwZoning.Items.Clear();
                            lvwZoning.Refresh();

                            dbAPI.ExecuteAPI("GET", "View", "Zoning List", "", "Advance Detail", "", "ViewAdvanceDetail");

                            if (!clsGlobalVariables.isAPIResponseOK)
                            {
                                return;
                            }
                            if (dbAPI.isNoRecordFound() == false)
                            {
                                while (clsArray.ID.Length > i)
                                {
                                    // Add to List
                                    iLineNo++;
                                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                                    string pZoneID = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ZoneID);
                                    string pCluster = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_Cluster);
                                    string pZone = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_Zone);
                                    string pRegion = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_Region);
                                    string pArea = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_Area);
                                    string pCityMunicipal = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_CityMunicipal);
                                    string pSLA = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_SLA);

                                item.SubItems.Add(pZoneID);
                                    item.SubItems.Add(pCluster);
                                    item.SubItems.Add(pZone);
                                    item.SubItems.Add(pRegion);
                                    item.SubItems.Add(pArea);
                                    item.SubItems.Add(pCityMunicipal);
                                    item.SubItems.Add(pSLA);

                                lvwZoning.Items.Add(item);

                                    i++;
                                }

                                dbFunction.ListViewAlternateBackColor(lvwZoning);
                            }

                    break;

                    case 1:
                        lvwZoningAlias.Items.Clear();
                        lvwZoningAlias.Refresh();

                        dbAPI.ExecuteAPI("GET", "View", "Zoning Alias List", "", "Advance Detail", "", "ViewAdvanceDetail");

                        if (!clsGlobalVariables.isAPIResponseOK)
                        {
                            return;
                        }

                        if (dbAPI.isNoRecordFound() == false)
                        {
                            while (clsArray.ID.Length > i)
                            {
                                iLineNo++;
                                ListViewItem item = new ListViewItem(iLineNo.ToString());

                                string pAliasID = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ZAliasID);
                                string pAliasName = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ZAliasName);
                                string pOfficialName = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ZOfficialName);
                                string pPriority = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ZPriority);
                                string isWholeWord = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "IsWholeWord");
                                string isActive = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], "IsActive");

                                item.SubItems.Add(pAliasID);
                                item.SubItems.Add(pAliasName);
                                item.SubItems.Add(pOfficialName);
                                item.SubItems.Add(pPriority);
                                item.SubItems.Add(isWholeWord);
                                item.SubItems.Add(isActive);

                                lvwZoningAlias.Items.Add(item);

                                i++;
                            }

                            dbFunction.ListViewAlternateBackColor(lvwZoningAlias);
                        }

                        break;
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Execeptional Error: \n{ex.Message}");
                return;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private bool ValidateFields()
        {
            switch (int.Parse(tabIndex))
            {
                case 0:
                    if (!dbFunction.isValidDescriptionEntry(cboCluster.Text, "Cluster" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
                    if (!dbFunction.isValidDescriptionEntry(cboZone.Text, "Zone" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
                    if (!dbFunction.isValidDescriptionEntry(cboRegion.Text, "Region" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
                    if (!dbFunction.isValidDescriptionEntry(cboArea.Text, "Area" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
                    if (!dbFunction.isValidDescriptionEntry(cboCityMunicipal.Text, "City/Municipal" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
                    if (!dbFunction.isValidDescriptionEntry(txtSLA.Text, "SLA" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
                    break;

                case 1:
                    if (!dbFunction.isValidDescriptionEntry(txtAliasName.Text, "AliasName" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
                    if (!dbFunction.isValidDescriptionEntry(txtOfficialName.Text, "OfficialName" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
                    if (!dbFunction.isNumeric(txtPriority.Text.Trim()))
                    {
                        dbFunction.SetMessageBox(
                            "Priority must contain numbers only.",
                            clsDefines.FIELD_CHECK_MSG,
                            clsFunction.IconType.iError);

                        txtPriority.Focus();
                        return false;
                    }
                    break;
            }


            return true;
        }

        private void lvwZoning_DoubleClick(object sender, EventArgs e)
        {
            if (lvwZoning.SelectedItems[0].SubItems[1].Text.Length > 0)
            {
                string pSelectedRow = dbFunction.GetListViewSelectedRow(lvwZoning, 0);
                Debug.WriteLine("pSelectedRow=\n" + pSelectedRow);

                dbFunction.TextBoxUnLock(true, this);
                dbFunction.ComBoBoxUnLock(true, this);

                txtID.Text = dbFunction.GetSearchValue("ZoneID");
                cboCluster.Text = dbFunction.GetSearchValue("Cluster");
                cboZone.Text = dbFunction.GetSearchValue("Zone");
                cboRegion.Text = dbFunction.GetSearchValue("Region");
                cboArea.Text = dbFunction.GetSearchValue("Area");
                cboCityMunicipal.Text = dbFunction.GetSearchValue("City/Municipal");
                txtSLA.Text = dbFunction.GetSearchValue("SLA");

                fEdit = true;                
                btnAdd.Enabled = false;
                btnSave.Enabled = true;
                txtID.ReadOnly = true;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            loadData();
        }

        private void initSearchTextBox(bool isEnable)
        {
            txtSearch.Enabled = isEnable;
            txtSearchAlias.Enabled = isEnable;

            txtSearch.ReadOnly = !isEnable;
            txtSearchAlias.ReadOnly = !isEnable;

            if (isEnable)
            {
                txtSearch.BackColor = Color.White;
                txtSearch.ReadOnly = false;
            }
            else
            {
                txtSearch.BackColor = Color.WhiteSmoke;
                txtSearch.ReadOnly = true;
            }

        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    dbFunction.findAndSelectListViewItem(lvwZoning, txtSearch.Text);   
                    break;
                
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            switch (int.Parse(tabIndex))
            {
                case 0:
                    dbFunction.findAndSelectListViewItem(lvwZoning, txtSearch.Text);
                    break;
                case 1:
                    dbFunction.findAndSelectListViewItem(lvwZoningAlias, txtSearchAlias.Text);
                    break;
            }
        }

        private void ZoningTabControl_SelectionChanged(object sender, EventArgs e)
        {
            tabIndex = ZoningTabControl.SelectedIndex.ToString();
            btnClear_Click(this, e);

            loadData();
        }

        private void lvwZoningAlias_DoubleClick(object sender, EventArgs e)
        {
            if (lvwZoningAlias.SelectedItems[0].SubItems[1].Text.Length > 0)
            {
                string pSelectedRow = dbFunction.GetListViewSelectedRow(lvwZoningAlias, 0);
                Debug.WriteLine("pSelectedRow=\n" + pSelectedRow);

                dbFunction.TextBoxUnLock(true, this);
                dbFunction.ComBoBoxUnLock(true, this);

                txtAliasID.Text = dbFunction.GetSearchValue("AliasID");
                txtAliasName.Text = dbFunction.GetSearchValue("AliasName");
                txtOfficialName.Text = dbFunction.GetSearchValue("OfficialName");
                txtPriority.Text = dbFunction.GetSearchValue("Priority");

                chkIsWholeWord.Checked = dbFunction.GetSearchValue(IsWholeWord.Text) == clsFunction.sOne;
                chkIsActive.Checked = dbFunction.GetSearchValue(isActive.Text) == clsFunction.sOne;

                fEdit = true;
                btnAdd.Enabled = false;
                btnSave.Enabled = true;
                txtAliasID.ReadOnly = true;
            }
        }
    }
}
