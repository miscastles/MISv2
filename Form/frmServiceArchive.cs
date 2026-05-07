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
    public partial class frmServiceArchive : Form
    {

        private clsAPI dbAPI;        
        private clsFunction dbFunction;
        private clsFile dbFile;

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

        public frmServiceArchive()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwList, true);
        }

        private void frmServiceArchieving_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            dbFile = new clsFile();

            dbFunction.ClearComboBox(this);
            dbFunction.ClearListViewItems(lvwList);
            
            InitDateRange();

            dbAPI.FillComboBoxServiceType(cboSearchServiceType);

            Cursor.Current = Cursors.Default;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        
        private void btnClear_Click(object sender, EventArgs e)
        {
            dbFunction.ClearListViewItems(lvwList);
            InitDateRange();
        }
        
        private void loadData()
        {
            int i = 0;
            int iLineNo = 0;            
            string pFileName = "";

            Cursor.Current = Cursors.WaitCursor;

            Debug.WriteLine("--loadData--");
            
            lvwList.Items.Clear();

            clsSearch.ClassSearchValue = $"{clsSearch.ClassJobType}{clsDefines.gPipe}" +
                                        $"{clsDefines.gZero}{clsDefines.gPipe}" +
                                        $"{clsSearch.ClassDateFrom}{clsDefines.gPipe}" +
                                        $"{clsSearch.ClassDateTo}{clsDefines.gPipe}" +
                                        $"{clsDefines.gZero}";

            Debug.WriteLine("clsSearch.ClassSearchValue=" + clsSearch.ClassSearchValue);

            dbAPI.ExecuteAPI("GET", "View", "FSR Service Detail", clsSearch.ClassSearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.ID.Length > i)
                {
                    iLineNo++;

                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                    // Data
                    string pJSONString = clsArray.detail_info[i];

                    string serviceno = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SERVICENO);
                    
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SERVICENO));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_IRIDNO));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_JobTypeDescription));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MERCHANTNAME));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TID));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MID));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_IRNO));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FSRDate));

                    pFileName = serviceno + clsDefines.FSR_FILENAME_PREFIX + clsDefines.FILE_EXT_PDF;
                    item.SubItems.Add(dbAPI.isFileExist("Search", "Check Attach File", $"{pFileName}") ? clsDefines.MSG_FOUND : clsDefines.MSG_NOT_FOUND);

                    pFileName = serviceno + clsDefines.DIAGNOSTIC_FILENAME_PREFIX + clsDefines.FILE_EXT_PDF;
                    item.SubItems.Add(dbAPI.isFileExist("Search", "Check Attach File", $"{pFileName}") ? clsDefines.MSG_FOUND : clsDefines.MSG_NOT_FOUND);

                    string pJSONStringCount = dbAPI.checkFileInfo("View", "File Count", serviceno);
                    if (dbFunction.isValidDescription(pJSONStringCount))
                    {
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONStringCount, clsDefines.TAG_PngCount));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONStringCount, clsDefines.TAG_JpgCount));
                    }
                    else
                    {
                        item.SubItems.Add(clsFunction.sZero);
                        item.SubItems.Add(clsFunction.sZero);
                    }                    

                    lvwList.Items.Add(item);

                    i++;

                }

                dbFunction.ListViewAlternateBackColor(lvwList);
                
            }

            Cursor.Current = Cursors.Default;
        }

        private void InitDateRange()
        {
            dteDateFrom.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteDateFrom, clsFunction.sDateDefaultFormat);

            dteDateTo.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteDateTo, clsFunction.sDateDefaultFormat);
            
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            clsSearch.ClassServiceTypeID = 0;
            clsSearch.ClassJobType = 0;
            if (!cboSearchServiceType.Text.Equals(clsFunction.sDefaultSelect))
            {
                // Get Info
                dbAPI.ExecuteAPI("GET", "Search", "Service Type Info", cboSearchServiceType.Text, "Get Info Detail", "", "GetInfoDetail");

                if (dbAPI.isNoRecordFound() == false)
                {
                    clsSearch.ClassServiceTypeID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0));
                    clsSearch.ClassJobType = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5));

                }
            }

            clsSearch.ClassDateFrom = dteDateFrom.Value.ToString("yyyy-MM-dd");
            clsSearch.ClassDateTo = dteDateTo.Value.ToString("yyyy-MM-dd");

            dbFunction.ClearListViewItems(lvwList);

            loadData();
        }
    }
}
