using MIS.Controller;
using MIS.Enums;
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
    public partial class frmMType : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        public static string sHeader;
        bool fEdit = false;

        public static bool isGroupLock = false;
        public static int iGroupType = 0;
        public static bool isAmountEntry = false;

        private const int BANK_TEMPLATE_POS_SETUP_TYPE = 30;
        private const int EXISTING_POS_SETUP_TYPE = 6;
        private static readonly HashSet<int> EXISTING_POS_SETUP_TYPE_IDS = new HashSet<int> { 19, 22 };

        // Controller
        private TypeController _mTypeController;

#if ENABLE_COMPOSITED
                protected override CreateParams CreateParams
                {
                    get
                    {
                        CreateParams cp = base.CreateParams;
                        cp.ExStyle |= 0x02000000;
                        return cp;
                    }
                }
#endif

        public frmMType()
        {
            InitializeComponent();

            // Initialize the controller object
            _mTypeController = new TypeController();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void frmMType_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void frmMType_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            fEdit = false;
            InitButton();
            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);

            initAmountEntry(isAmountEntry);

            if (isGroupLock)
            {
                cboType.Enabled = false;
                // DropDownList ignores .Text unless the item exists in Items
                cboType.Items.Clear();
                cboType.Items.Add(GetGroupTypeName(iGroupType));
                cboType.SelectedIndex = 0;

                txtGroupType.Text = iGroupType.ToString();
            }
            else
            {
                dbAPI.FillComboBoxGroupType(cboType);

                cboType.Items.Add(GetGroupTypeName((int)BankColumnTemplateType.BankTemplateRequestPrioritizationType));
                cboType.Items.Add(GetGroupTypeName((int)BankColumnTemplateType.BankTemplatePosSetupType));
                cboType.Items.Add(GetGroupTypeName((int)BankColumnTemplateType.BankTemplatePosConnectionType));
            }

            loadData();

            lblHeader.Text = "ENROLLMENT - " + sHeader;

            Cursor.Current = Cursors.Default;
        }

        private void cboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cboType.Text.Equals(clsFunction.sDefaultSelect) && !isGroupLock)
            {
                txtGroupType.Text = GetGroupTypeID(cboType.SelectedIndex).ToString();
                loadData();
            }
        }

        private void loadData()
        {
            int lineno = 0;
            List<TypeController> mList = null;
            dbFunction.ClearListViewItems(lvwList);

            int pGroupType;
            if (!int.TryParse(txtGroupType.Text, out pGroupType) || pGroupType <= 0)
            {
                return;
            }

            mList = _mTypeController.getDetailList(clsFunction.sZero + clsFunction.sPipe + pGroupType);

            // CASHOUT SETUP and RETAIL SETUP are legacy POS setup records under
            // Type 6. Include those same records in the Bank Template POS Setup
            // maintenance view instead of duplicating them under Type 30.
            if (pGroupType == BANK_TEMPLATE_POS_SETUP_TYPE)
            {
                List<TypeController> existingPosSetupList = _mTypeController.getDetailList(
                    clsFunction.sZero + clsFunction.sPipe + EXISTING_POS_SETUP_TYPE);

                if (existingPosSetupList != null)
                {
                    mList.AddRange(existingPosSetupList.Where(item =>
                        EXISTING_POS_SETUP_TYPE_IDS.Contains(item.TypeID) &&
                        !mList.Any(existingItem => existingItem.TypeID == item.TypeID)));
                }
            }

            if (mList != null)
            {
                if (mList != null)
                {
                    foreach (var itemData in mList)
                    {
                        lineno++;
                        ListViewItem item = new ListViewItem(lineno.ToString());
                        item.SubItems.Add(itemData.TypeID.ToString());
                        item.SubItems.Add(itemData.Description);
                        item.SubItems.Add(GetGroupTypeName(itemData.Type));
                        item.SubItems.Add(itemData.SequenceDisplay.ToString());
                        item.SubItems.Add(itemData.RentFee.ToString());
                        item.SubItems.Add(itemData.Price.ToString());

                        lvwList.Items.Add(item);

                    }

                    dbFunction.ListViewAlternateBackColor(lvwList);

                }
            }
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.SelectedItems.Count > 0)
            {
                string LineNo = lvwList.SelectedItems[0].Text;
                txtLineNo.Text = LineNo;

                if (dbFunction.isValidID(txtLineNo.Text))
                {
                    string pSelectedRow = dbFunction.GetListViewSelectedRow(lvwList, 0);
                    Debug.WriteLine("pSelectedRow=\n" + pSelectedRow);

                    txtID.Text = dbFunction.GetSearchValue("ID");
                    txtDescription.Text = dbFunction.GetSearchValue("Description");
                    dbFunction.TextBoxUnLock(true, this);

                    fEdit = true;

                    btnAdd.Enabled = false;
                    btnSave.Enabled = true;

                    getInfo(int.Parse(txtID.Text));

                    txtDescription.Focus();
                }
            }
        }

        private void lvwList_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (lvwList.Items.Count > 0 && dbFunction.isValidID(txtLineNo.Text))
                    {
                        lvwList_DoubleClick(this, e);

                    }
                    break;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ClearListViewItems(lvwList);

            cboType_SelectedIndexChanged(this, e);

            fEdit = false;
            InitButton();

            initAmountEntry(isAmountEntry);

            if (isGroupLock)
            {
                cboType.Text = GetGroupTypeName(iGroupType);
                txtGroupType.Text = iGroupType.ToString();

                loadData();
            }

            Cursor.Current = Cursors.Default;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string sRowSQL = "";
            string sSQL = "";

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iDescription, txtDescription.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iType, cboType.Text)) return;

            if (!dbFunction.fSavingConfirm(fEdit)) return;

            if (!fEdit)
            {
                sSQL = "";
                sRowSQL = "";
                sRowSQL = " ('" + StrClean(txtDescription.Text) + "', " +
                sRowSQL + sRowSQL + " '" + txtGroupType.Text + "') ";
                sSQL = sSQL + sRowSQL;

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "Type", sSQL, "InsertMaintenanceMaster");

                dbFunction.SetMessageBox("Type has been successfully saved", "Saved", clsFunction.IconType.iInformation);
            }
            else
            {
                if (dbFunction.isValidID(txtID.Text))
                {
                    clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtID.Text) + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetStringValue(StrClean(txtDescription.Text)) + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetStringValue(StrClean(txtRemarks.Text)) + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtSeqDisplay.Text) + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtRentFee.Text) + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtPrice.Text);

                    dbAPI.ExecuteAPI("PUT", "Update", "Type Info", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                    dbFunction.SetMessageBox("Type has been successfully modified", "Edited", clsFunction.IconType.iInformation);

                }
                else
                {
                    dbFunction.SetMessageBox("No selected record from list", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                }
            }

            btnClear_Click(this, e);
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

            btnClear_Click(this, e);

            btnAdd.Enabled = false;
            btnSave.Enabled = true;

            dbFunction.TextBoxUnLock(true, this);

            if (!isGroupLock)
                cboType.Text = clsFunction.sDefaultSelect;

            //dbFunction.ClearListViewItems(lvwList);

            txtDescription.Focus();
        }

        private void getInfo(int pID)
        {
            TypeController model = new TypeController();
            TypeController data = model.getInfo(pID);

            if (data != null)
            {
                txtGroupType.Text = data.Type.ToString();
                txtDescription.Text = data.Description;
                txtRemarks.Text = data.Remarks;
                txtSeqDisplay.Text = data.SequenceDisplay.ToString();
                txtRentFee.Text = data.RentFee.ToString();
                txtPrice.Text = data.Price.ToString();
            }
        }

        private void initAmountEntry(bool isEnable)
        {
            txtRentFee.Text = txtPrice.Text = "0.00";
            txtSeqDisplay.Text = "0";

            txtRentFee.Enabled = txtPrice.Enabled = isAmountEntry;
            if (isEnable)
            {
                txtRentFee.BackColor = txtPrice.BackColor = clsFunction.EntryBackColor;
            }
            else
            {
                txtRentFee.BackColor = txtPrice.BackColor = clsFunction.DisableBackColor;
            }
        }

        public int GetGroupTypeID(int selectedIndex)
        {
            int existingGroupCount = dbAPI.GetGroupType().Length;

            switch (selectedIndex - existingGroupCount)
            {
                case 0:
                    return (int)BankColumnTemplateType.BankTemplateRequestPrioritizationType;   // 29
                case 1:
                    return (int)BankColumnTemplateType.BankTemplatePosSetupType;                // 30
                case 2:
                    return (int)BankColumnTemplateType.BankTemplatePosConnectionType;           // 31
            }

            if (selectedIndex >= 0 && selectedIndex < existingGroupCount)
            {
                return selectedIndex;
            }
            else
            {
                return 0;
            }
        }

        public string GetGroupTypeName(int groupType)
        {
            switch (groupType)
            {
                case (int)BankColumnTemplateType.BankTemplateRequestPrioritizationType:
                    return "IR-REQUEST PRIORITIZATION";
                case (int)BankColumnTemplateType.BankTemplatePosSetupType:
                    return "IR-POS SETUP TYPE";
                case (int)BankColumnTemplateType.BankTemplatePosConnectionType:
                    return "IR-POS CONNECTION TYPE";
            }

            string[] groupTypes = dbAPI.GetGroupType();

            if (groupType >= 0 && groupType < groupTypes.Length)
            {
                return groupTypes[groupType];
            }
            else
            {
                return groupType.ToString();
            }
        }
    }
}
