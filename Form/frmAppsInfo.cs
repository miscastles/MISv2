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
    public partial class frmAppsInfo : Form
    {
        private clsAPI dbAPI;
        private clsINI dbSystem;
        private clsInternet dbInternet;
        private clsFunction dbFunction;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED
                //cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        public frmAppsInfo()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwList, true);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmAppsInfo_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void frmAppsInfo_Load(object sender, EventArgs e)
        {

            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();                    
            dbFunction = new clsFunction();
            dbInternet = new clsInternet();
            dbSystem = new clsINI();
            
            dbSystem.InitAPISetting();

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ClearListViewItems(lvwList);
            
            loadData(lvwList);

            Cursor.Current = Cursors.Default;
        }

        private void loadData(ListView obj)
        {
            int i = 0;
            int iLineNo = 0;

            Cursor.Current = Cursors.WaitCursor;

            dbAPI.ExecuteAPI("GET", "View", "Application Info List", "", "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;
            
            if (dbAPI.isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.ID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TerminalTypeID));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TerminalModelID));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TerminalType));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TerminalModel));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_AppVersion));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_AppCRC));

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }

            Cursor.Current = Cursors.Default;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ClearListViewItems(lvwList);

            loadData(lvwList);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!dbFunction.fPromptConfirmation("Are you sure to update applcation information details?")) return;

            Cursor.Current = Cursors.WaitCursor;

            if (dbFunction.isValidID(lvwList.Items.Count.ToString()))
            {
                foreach (ListViewItem x in lvwList.Items)
                {
                    txtTerminalModelID.Text = x.SubItems[2].Text;
                    txtVersion.Text = x.SubItems[5].Text;
                    txtCRC.Text = x.SubItems[6].Text;

                    if (dbFunction.isValidID(txtTerminalModelID.Text) && dbFunction.isValidDescription(txtCRC.Text))
                    {
                        dbAPI.ExecuteAPI("PUT", "Update", "Update Application Info", txtTerminalModelID.Text + clsFunction.sPipe + txtVersion.Text + clsFunction.sPipe + txtCRC.Text, "", "", "UpdateCollectionDetail");
                    }                    
                }

                dbFunction.SetMessageBox("Update application information complete.", "Application information", clsFunction.IconType.iInformation);
            }

            Cursor.Current = Cursors.Default;

            btnRefresh_Click(this, e);

        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {   
            if (lvwList.Items.Count > 0)
            {
                string pSelectedRow = clsSearch.ClassRowSelected = dbFunction.GetListViewSelectedRow(lvwList, 0);
                Debug.WriteLine("pSelectedRow=\n" + pSelectedRow);

                if (int.Parse(dbFunction.GetSearchValue("TerminalTypeID")) > 0)
                {
                    dbFunction.ClearTextBox(this);
                    dbFunction.TextBoxUnLock(true, this);

                    txtLineNo.Text = dbFunction.GetSearchValue("Line#");
                    txtTerminalTypeID.Text = dbFunction.GetSearchValue("TerminalTypeID");
                    txtTerminalModelID.Text = dbFunction.GetSearchValue("TerminalModelID");
                    txtType.Text = dbFunction.GetSearchValue("Type");
                    txtModel.Text = dbFunction.GetSearchValue("Model");
                    txtVersion.Text = dbFunction.GetSearchValue("Version");
                    txtCRC.Text = dbFunction.GetSearchValue("CRC");
                    
                }
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (dbFunction.isValidID(txtLineNo.Text))
            {  
                bool isValid = dbFunction.fPromptConfirmation("Modify the following information below" + "\n\n" +
                        "> Type : " + txtType.Text + "\n" +
                        "> Model : " + txtModel.Text + "\n" +
                        "> Version : " + txtVersion.Text + "\n" +
                        "> CRC : " + txtCRC.Text);
                
                if (isValid)
                {
                    dbFunction.updateListViewByColRow(lvwList, 5, int.Parse(txtLineNo.Text), txtVersion.Text);
                    dbFunction.updateListViewByColRow(lvwList, 6, int.Parse(txtLineNo.Text), txtCRC.Text);
                }
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
