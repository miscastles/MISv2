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
        
        // Controller
        private TypeController _mTypeController;

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
                cboType.Text = dbAPI.GetGroupType()[iGroupType];

                txtGroupType.Text = iGroupType.ToString();
            }
            else
            {
                dbAPI.FillComboBoxGroupType(cboType);
            }

            loadData();

            lblHeader.Text = "ENROLLMENT - " + sHeader;
            
            Cursor.Current = Cursors.Default;
        }

        private void cboType_SelectedIndexChanged(object sender, EventArgs e)
        {   
            if (!cboType.Text.Equals(clsFunction.sDefaultSelect) && !isGroupLock)
            {
                txtGroupType.Text = cboType.SelectedIndex.ToString();
                loadData();
            }              
        }

        private void loadData()
        {
            int lineno = 0;
            int i = 0;
            List<TypeController> mList = null;
            int pGroupType = (dbFunction.isValidID(txtGroupType.Text) ? int.Parse(dbFunction.CheckAndSetNumericValue(txtGroupType.Text)) : 1); // default load sim type

            dbFunction.ClearListViewItems(lvwList);

            mList = _mTypeController.getDetailList(clsFunction.sZero + clsFunction.sPipe + pGroupType); 
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
                        item.SubItems.Add(dbAPI.GetGroupType()[itemData.Type]);
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
                cboType.Text = dbAPI.GetGroupType()[iGroupType];
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
    }
}
