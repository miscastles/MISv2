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
    public partial class frmFindME : Form
    {
        private clsAPI dbAPI;
        private clsINI dbSetting;
        private clsFile dbFile;
        private clsFunction dbFunction;
        public static string sHeader;

        private static int iPanelSearchHeightMin = 35;
        private static int iPanelSearchHeightMax = 300;
        private static int iPanelListHeightMax = 519;

        private int iRecordCount = 0;

        List<string> arrServiceNoCol = new List<String>();
        List<string> arrTAIDNoCol = new List<String>();
        List<string> arrIRIDNoCol = new List<String>();
        List<string> arrTerminalIDCol = new List<String>();

        public enum SearchType
        {
            iTerminal, iSIM, iIR, iTA, iFSR
        }

        public frmFindME()
        {
            InitializeComponent();
        }

        private void frmFindMasterEngine_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbSetting = new clsINI();
            dbFile = new clsFile();
            dbFunction = new clsFunction();
            dbSetting.InitDatabaseSetting();
            lblSearchStatus.Text = "";
            lblHeader.Text = lblHeader.Text + " " + "[ " + sHeader + " ]";
            lblTotalCount.Text = "";

            InitPage(0, 0);
            InitListView();
            
            btnPreview.Enabled = false;
        }

        private void InitListView()
        {            
            lvwSearch.View = View.Details;
            lvwSearch.Columns.Add("LINE#", 50, HorizontalAlignment.Left); // Line#            
            lvwSearch.Columns.Add("TERMINALID", dbFunction.ID_Width(), HorizontalAlignment.Left);
            lvwSearch.Columns.Add("TERMINAL SN", 100, HorizontalAlignment.Left);            
            lvwSearch.Columns.Add("TYPE", 90, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("MODEL", 90, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("BRAND", 90, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("TERM STATUS", 100, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("IRIDNO", dbFunction.ID_Width(), HorizontalAlignment.Left);
            lvwSearch.Columns.Add("IR NO.", 130, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("DATE REQUEST", 110, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("INST REQUEST", 110, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("MERCHANT NAME", 210, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("TID", 110, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("MID", 130, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("REGION", 110, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("CITY", 130, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("IR STATUS", 110, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("SERVICENO", dbFunction.ID_Width(), HorizontalAlignment.Left);
            lvwSearch.Columns.Add("SERVICE CREATED", 160, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("SERVICE REQ DATE", 150, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("SERVICE REQ TIME", 150, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("REQUEST NO.", 130, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("REFERENCE NO.", 130, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("CONTACT PERSON", 160, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("CONTACT NO.", 110, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("MERCH REPRESENTATIVE", 160, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("MERCH CONTACT NO.", 150, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("SERVICE REMARKS", 180, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("CLIENT NAME", 150, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("FE NAME", 150, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("TERMINAL SN", 130, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("SIM SN", 180, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("CARRIER", 90, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("DOCK SN", 110, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("REPLACED TERMINAL SN", 180, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("REPLACED SIM SN", 180, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("REPLACED DOCK SN", 130, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("RECEIPT DATE", 110, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("RECEIPT TIME", 110, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("TIME ARRIVED", 100, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("TIME START", 90, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("TIME END", 90, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("PROBLEM REPORTED", 180, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("ACTUAL PROBLEM FOUND", 180, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("ACTION TAKEN", 180, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("ANY COMMENTS", 180, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("JOB TYPE DESCRIPTION", 170, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("JOB TYPE STATUS", 150, HorizontalAlignment.Left);
            lvwSearch.Columns.Add("ACTION MADE", 130, HorizontalAlignment.Left);
            
        }

        private void ProcessServiceSearch()
        {
            lblSearchStatus.Text = "";
            
            GetServiceTerminalSNList(); // Service TerminalSN List (display to listview)
            
        }

        private void GetServiceTerminalSNList()
        {
            int i = 0;
            int iLineNo = 0;
            int iRecCount = 0;
            bool fHold = true;

            //clsSearch.ClassServiceDateFrom = "06-20-2019";
            //clsSearch.ClassServiceDateTo = "06-20-2019";

            // Clear Local Array
            arrServiceNoCol.Clear();
            arrTAIDNoCol.Clear();
            arrIRIDNoCol.Clear();

            lvwSearch.Items.Clear();
            btnPreview.Enabled = false;
            
            clsSearch.ClassAdvanceSearchValue = clsFunction.sNull;
            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassServiceDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassServiceDateTo + clsFunction.sPipe +
                                                clsSearch.ClassMerchantID + clsFunction.sPipe +
                                                clsSearch.ClassIRIDNo + clsFunction.sPipe +
                                                clsSearch.ClassTerminalID + clsFunction.sPipe +
                                                clsSearch.ClassSIMID + clsFunction.sPipe +
                                                clsSearch.ClassClientID + clsFunction.sPipe +
                                                clsSearch.ClassFEID + clsFunction.sPipe +
                                                clsSearch.ClassTID + clsFunction.sPipe +
                                                clsSearch.ClassMID + clsFunction.sPipe +
                                                clsSearch.ClassJobTypeDescriptionList + clsFunction.sPipe +
                                                clsSearch.ClassJobTypeStatusDescriptionList + clsFunction.sPipe +
                                                clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                clsFunction.iOffSetToSerialNo;

            Debug.WriteLine("GetServiceTerminalSNList::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);
            
            dbFunction.GetRequestTime("GetServiceTerminalSNList");

            dbAPI.ExecuteAPI("GET", "View", "Service TerminalSN List", clsSearch.ClassAdvanceSearchValue, "Servicing Detail", "", "ViewServicingDetail");

            dbFunction.GetResponseTime("GetServiceTerminalSNList");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                if (fHold)
                {
                    clsArray.HoldTAIDNo = clsArray.TAIDNo;
                    clsArray.HoldIRIDNo = clsArray.IRIDNo;
                    fHold = false;
                }
                

                while (clsArray.TerminalID.Length > i)
                {
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.TerminalID[i]);
                    item.SubItems.Add(clsArray.TerminalSN[i]);
                    item.SubItems.Add(clsArray.TypeDescription[i]);
                    item.SubItems.Add(clsArray.ModelDescription[i]);
                    item.SubItems.Add(clsArray.BrandDescription[i]);
                    item.SubItems.Add(clsArray.TerminalStatusDescription[i]);

                    lvwSearch.Items.Add(item);

                    GetServiceIRNoList(clsArray.TerminalID[i], clsArray.TerminalSN[i], clsArray.HoldIRIDNo[i]);

                    AddListViewLineSeparator();

                    i++;

                    //dbFunction.AppDoEvents(true);

                    ucStatus.iState = 3;
                    ucStatus.sMessage = "PLEASE WAIT. RETRIEVING DATA..." + "[ " + iLineNo.ToString() + " / " + clsArray.TerminalID.Length.ToString() + " ]";
                    ucStatus.AnimateStatus();

                }

                // Process TerminalSN ListView

                dbFunction.ListViewAlternateBackColor(lvwSearch);
                dbFunction.ListViewRowFocus(lvwSearch, 0);

                iRecCount = i;
                iRecordCount = i;
                lblSearchStatus.Text = iRecCount.ToString() + " " + "record(s) found.";
                btnPreview.Enabled = true;
            }
            else
            {
                btnPreview.Enabled = false;
                lblSearchStatus.Text = "No record found...";

                ucStatus.iState = 0;
                ucStatus.sMessage = clsFunction.sNull;
                ucStatus.AnimateStatus();
            }
        }

        private void GetServiceIRNoList(string sTerminalID, string sTerminalSN, string sIRIDNo)
        {
            int i = 0;
            int iLineNo = 0;

            clsSearch.ClassAdvanceSearchValue = sTerminalID;

            Debug.WriteLine("GetServiceIRNoList::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbFunction.GetRequestTime("GetServiceIRNoList");

            dbAPI.ExecuteAPI("GET", "View", "Service IRNo List", clsSearch.ClassAdvanceSearchValue, "Servicing Detail", "", "ViewServicingDetail");

            dbFunction.GetResponseTime("GetServiceIRNoList");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.IRIDNo.Length > i)
                {
                    iLineNo++;
                    ListViewItem item = new ListViewItem(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);

                    // IRNo List
                    item.SubItems.Add(clsArray.IRIDNo[i]);
                    item.SubItems.Add(clsArray.IRNo[i]);
                    item.SubItems.Add(clsArray.IRDate[i]);
                    item.SubItems.Add(clsArray.InstallationDate[i]);                    
                    item.SubItems.Add(clsArray.MerchantName[i]);
                    item.SubItems.Add(clsArray.TID[i]);
                    item.SubItems.Add(clsArray.MID[i]);
                    item.SubItems.Add(clsArray.Region[i]);
                    item.SubItems.Add(clsArray.Province[i]);
                    item.SubItems.Add(clsArray.IRStatusDescription[i]);

                    lvwSearch.Items.Add(item);

                    GetServiceNoList(sTerminalID, clsArray.IRIDNo[i], sTerminalSN);

                    Debug.WriteLine("GetServiceIRNoList::clsArray.IRIDNo[i]=" + clsArray.IRIDNo[i] + "|" + 
                                                        "clsArray.MerchantName[i]=" + clsArray.MerchantName[i] + "|" +
                                                        "clsArray.TID[i]=" + clsArray.TID[i] + "|" +
                                                        "clsArray.MID[i]=" + clsArray.MID[i]);

                    i++;

                }
            }
        }

        private void GetServiceNoList(string sTerminalID, string sIRIDNo, string sTerminalSN)
        {
            int i = 0;
            int iLineNo = 0;

            clsSearch.ClassAdvanceSearchValue = sTerminalID + clsFunction.sPipe +
                                                sIRIDNo + clsFunction.sPipe +
                                                clsSearch.ClassJobTypeDescriptionList + clsFunction.sPipe +
                                                clsSearch.ClassJobTypeStatusDescriptionList;

            Debug.WriteLine("GetServiceNoList::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbFunction.GetRequestTime("GetServiceNoList");

            dbAPI.ExecuteAPI("GET", "View", "Service ServiceNo List", clsSearch.ClassAdvanceSearchValue, "Servicing Detail", "", "ViewServicingDetail");

            dbFunction.GetResponseTime("GetServiceNoList");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.ServiceNo.Length > i)
                {
                    iLineNo++;
                    ListViewItem item = new ListViewItem(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);

                    // IRNo List
                    item.SubItems.Add(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);
                    item.SubItems.Add(clsFunction.sNull);

                    //ServiceNo List
                    item.SubItems.Add(clsArray.ServiceNo[i]);
                    item.SubItems.Add(clsArray.ServiceDateTime[i]);
                    item.SubItems.Add(clsArray.ServiceReqDate[i]);
                    item.SubItems.Add(clsArray.ServiceReqTime[i]);
                    item.SubItems.Add(clsArray.RequestNo[i]);
                    item.SubItems.Add(clsArray.ReferenceNo[i]);
                    item.SubItems.Add(clsArray.CustomerName[i]);
                    item.SubItems.Add(clsArray.CustomerContactNo[i]);
                    item.SubItems.Add(clsArray.MerchantRepresentative[i]);
                    item.SubItems.Add(clsArray.MerchantContactNo[i]);
                    item.SubItems.Add(clsArray.Remarks[i]);
                    item.SubItems.Add(clsArray.ClientName[i]);
                    item.SubItems.Add(clsArray.FEName[i]);
                    item.SubItems.Add(sTerminalSN);
                    item.SubItems.Add(clsArray.SIMSerialNo[i]);
                    item.SubItems.Add(clsArray.SIMCarrier[i]);
                    item.SubItems.Add(clsArray.DockSN[i]);
                    item.SubItems.Add(clsArray.ReplaceTerminalSN[i]);
                    item.SubItems.Add(clsArray.ReplaceSIMSN[i]);
                    item.SubItems.Add(clsArray.ReplaceDockSN[i]);
                    item.SubItems.Add(clsArray.FSRDate[i]);
                    item.SubItems.Add(clsArray.FSRTime[i]);
                    item.SubItems.Add(clsArray.TimeArrived[i]);
                    item.SubItems.Add(clsArray.TimeStart[i]);                    
                    item.SubItems.Add(clsArray.TimeEnd[i]);
                    item.SubItems.Add(clsArray.ProblemReported[i]);
                    item.SubItems.Add(clsArray.ActualProblemReported[i]);
                    item.SubItems.Add(clsArray.ActionTaken[i]);
                    item.SubItems.Add(clsArray.AnyComments[i]);
                    item.SubItems.Add(clsArray.JobTypeDescription[i]);
                    item.SubItems.Add(clsArray.JobTypeStatusDescription[i]);
                    item.SubItems.Add(clsArray.ActionMade[i]);

                    Debug.WriteLine("GetServiceNoList::clsArray.ServiceNo[i]=" + clsArray.ServiceNo[i]+"|"+ "clsArray.RequestNo[i]="+ clsArray.RequestNo[i]);

                    lvwSearch.Items.Add(item);

                    i++;

                }
            }
        }

        private void SaveServicingDetailTemp()
        {
            int i = 0;
            string sTerminalIDList = clsFunction.sZero;            
            
            if (lvwSearch.Items.Count > 0)
            {
                clsSearch.ClassAdvanceSearchValue = clsSearch.ClassServiceDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassServiceDateTo + clsFunction.sPipe +
                                                clsSearch.ClassMerchantID + clsFunction.sPipe +
                                                clsSearch.ClassIRIDNo + clsFunction.sPipe +
                                                clsSearch.ClassTerminalID + clsFunction.sPipe +
                                                clsSearch.ClassSIMID + clsFunction.sPipe +
                                                clsSearch.ClassClientID + clsFunction.sPipe +
                                                clsSearch.ClassFEID + clsFunction.sPipe +
                                                clsSearch.ClassTID + clsFunction.sPipe +
                                                clsSearch.ClassMID + clsFunction.sPipe +
                                                clsSearch.ClassJobTypeDescriptionList + clsFunction.sPipe +
                                                clsSearch.ClassJobTypeStatusDescriptionList;

                Debug.WriteLine("SaveServicingDetailTemp::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                dbFunction.GetRequestTime("SaveServicingDetailTemp");

                dbAPI.ExecuteAPI("GET", "View", "Service TerminalID List", clsSearch.ClassAdvanceSearchValue, "Servicing Detail", "", "ViewServicingDetail");

                dbFunction.GetResponseTime("SaveServicingDetailTemp");

                if (!clsGlobalVariables.isAPIResponseOK) return;

                if (clsArray.TerminalID.Length > 0)
                {

                    dbAPI.ExecuteAPI("DELETE", "Delete", "SVC", "", "Servicing Detail Temp", "", "DeleteCollectionDetail");

                    while (clsArray.TerminalID.Length > i)
                    {
                        sTerminalIDList = sTerminalIDList + clsArray.TerminalID[i] + clsFunction.sComma;

                        i++;

                        ucStatus.iState = 3;
                        ucStatus.sMessage = "PLEASE WAIT. EXPORTING DATA..." + "[ " + i.ToString() + " / " + clsArray.TerminalID.Length.ToString() + " ]";
                        ucStatus.AnimateStatus();
                    }

                    sTerminalIDList = sTerminalIDList.Substring(1, sTerminalIDList.Length - 2);

                    Debug.WriteLine("sTerminalIDList="+ sTerminalIDList);

                    clsSearch.ClassSearchValue = sTerminalIDList + clsFunction.sPipe +
                                                 clsSearch.ClassJobTypeDescriptionList + clsFunction.sPipe +
                                                 clsSearch.ClassJobTypeStatusDescriptionList;

                    Debug.WriteLine("SaveServicingDetailTemp::" + "clsSearch.ClassSearchValue=" + clsSearch.ClassSearchValue);

                    dbFunction.GetRequestTime("SaveServicingDetailTemp");

                    dbAPI.ExecuteAPI("POST", "Insert", "", clsSearch.ClassSearchValue, "Servicing Detail Temp", "", "InsertSelectCollectionDetail");

                    dbFunction.GetResponseTime("SaveServicingDetailTemp");
                }
            }
            else
            {
                ucStatus.iState = 0;
                ucStatus.sMessage = clsFunction.sNull;
                ucStatus.AnimateStatus();
            }
        }

        private void AddListViewLineSeparator()
        {
            ListViewItem item = new ListViewItem(clsFunction.sNull);
            for (int x = 0; x < lvwSearch.Columns.Count; x++)
            {                
                item.SubItems.Add(clsFunction.slvSeparator);                
            }
            lvwSearch.Items.Add(item);
        }
        
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void btnSearchOption_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            lblSearchStatus.Text = "";
            dbAPI.ResetAdvanceSearch();
            btnPreview.Enabled = false;

            ucStatus.iState = 0;
            ucStatus.sMessage = clsFunction.sNull;
            ucStatus.AnimateStatus();

            frmFindMECriteria frm = new frmFindMECriteria();
            frm.ShowDialog();

            if (frmFindMECriteria.fSelected)
            {
                lvwSearch.Items.Clear();                
                lblSearchOption.Text = "(-)Search Options";
                lblSearchOption_Click(this, e);

                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                //frmLoading frmWait = new frmLoading(); // Open Wait Window
                //clsFunction.WaitWindow(true, frmWait);

                ProcessPage();
                ProcessServiceSearch();
                
                //clsFunction.WaitWindow(false, frmWait); // Close Wait Window

                Cursor.Current = Cursors.Default;  // Back to normal
            }

            Cursor.Current = Cursors.Default;

        }

        private void frmFindME_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void btnSearchClear_Click(object sender, EventArgs e)
        {
            lvwSearch.Items.Clear();
            dbAPI.ResetAdvanceSearch();
            lblSearchStatus.Text = "";
            lblTotalCount.Text = "";

            InitPage(0, 0);

            btnPreview.Enabled = false;

            ucStatus.iState = 0;
            ucStatus.sMessage = clsFunction.sNull;
            ucStatus.AnimateStatus();
        }
        private void PanelSearchResize()
        {
            if (lblSearchOption.Text.CompareTo("(+)Search Options") == 0)
            {
                pnlSearch.Height = iPanelSearchHeightMin;
            }
            else
            {
                pnlSearch.Height = iPanelSearchHeightMax;
            }
        }
        private void PanelListLocationAndHeight()
        {
            var curLoc = pnlSearch.Location;

            if (lblSearchOption.Text.CompareTo("(+)Search Options") == 0)
            {
                pnlList.Location = new Point(curLoc.X, pnlSearch.Height + iPanelSearchHeightMin);
            }
            else
            {
                pnlList.Location = new Point(curLoc.X, pnlSearch.Height + iPanelSearchHeightMin);
            }

            pnlList.Height = (iPanelListHeightMax - pnlSearch.Height) + iPanelSearchHeightMin;
        }

        private void lblSearchOption_Click(object sender, EventArgs e)
        {
            if (lblSearchOption.Text.CompareTo("(+)Search Options") == 0)
            {
                lblSearchOption.Text = "(-)Search Options";
                lblSearchOptionDetails.Visible = true;
                lblSearchOptionDetails.Text = frmFindMECriteria.sSearchOptionDetails;
            }
            else
            {
                lblSearchOption.Text = "(+)Search Options";
                lblSearchOptionDetails.Visible = false;
                lblSearchOptionDetails.Text = "";
            }

            PanelSearchResize();
            PanelListLocationAndHeight();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (lvwSearch.Items.Count > 0)
            {
                Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

                //frmLoading frmWait = new frmLoading(); // Open Wait Window
                //clsFunction.WaitWindow(true, frmWait);

                SaveServicingDetailTemp(); // Save to tblservicingdetailtemp (report)

                try
                {
                    clsSearch.ClassReportID = 10;
                    clsSearch.ClassStatementType = "View";
                    clsSearch.ClassSearchValue = clsFunction.sZero;                    
                    clsSearch.ClassStoredProcedureName = "spViewServicingDetail";
                    clsSearch.ClassSearchBy = "Servicing Detail Temp";
                    clsSearch.ClassSearchValue = clsFunction.sZero;
                    clsSearch.ClassDateFrom = clsSearch.ClassServiceDateFrom;
                    clsSearch.ClassDateTo = clsSearch.ClassServiceDateTo;
                    clsSearch.ClassJobTypeDescription = clsSearch.ClassJobTypeList;
                    dbFunction.ProcessReport(clsSearch.ClassReportID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                //clsFunction.WaitWindow(false, frmWait); // Close Wait Window

                Cursor.Current = Cursors.Default;  // Back to normal
            }
            else
            {
                dbFunction.SetMessageBox("No report to preview.", "Preview Report", clsFunction.IconType.iError);
            }
        }
        private void InitPage(int iCurrentPage, int iTotalPage)
        {
            if (iTotalPage > 0)
            {
                // do nothing...
            }
            else
            {
                iCurrentPage = 0;
            }

            clsSearch.ClassCurrentPage = iCurrentPage;
            clsSearch.ClassTotalPage = iTotalPage;

            txtPage.Text = iCurrentPage.ToString() + " / " + iTotalPage.ToString();
        }

        private int GetTotalPage()
        {
            int iCount = 0;
            int totalPage = 0;
            int iLimitSize = clsFunction.iOffSetToSerialNo;

            lblTotalCount.Text = clsFunction.sNull;            
            Debug.WriteLine("GetTotalPage::iLimitSize=" + iLimitSize);

            dbAPI.GetViewCount("Search", "Service TerminalSN List", clsSearch.ClassAdvanceSearchValue, "Get Count");
            iCount = 0;
            if (dbAPI.isNoRecordFound() == false)
            {
                iCount = clsTerminal.ClassTerminalCount;
                lblTotalCount.Text = iCount.ToString() + " " + "total record(s)";
            }
                

            if (iCount > 0)
            {
                totalPage = (int)Math.Ceiling((double)iCount / iLimitSize);
            }
            else
            {
                totalPage = 0;
            }

            return totalPage;
        }

        private void btnFirstPage_Click(object sender, EventArgs e)
        {            
            clsSearch.ClassCurrentPage = int.Parse(clsFunction.sOne);
            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            ProcessServiceSearch();
        }

        private void btnLastPage_Click(object sender, EventArgs e)
        {
            clsSearch.ClassCurrentPage = clsSearch.ClassTotalPage;
            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            ProcessServiceSearch();
        }

        private void btnPreviousPage_Click(object sender, EventArgs e)
        {
            if (clsSearch.ClassCurrentPage > int.Parse(clsFunction.sOne))
                clsSearch.ClassCurrentPage--;

            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            ProcessServiceSearch();
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (clsSearch.ClassCurrentPage < clsSearch.ClassTotalPage)
                clsSearch.ClassCurrentPage++;

            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            ProcessServiceSearch();
        }

        private void ProcessPage()
        {
            Debug.WriteLine("--ProcessPage--");

            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassServiceDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassServiceDateTo + clsFunction.sPipe +
                                                clsSearch.ClassMerchantID + clsFunction.sPipe +
                                                clsSearch.ClassIRIDNo + clsFunction.sPipe +
                                                clsSearch.ClassTerminalID + clsFunction.sPipe +
                                                clsSearch.ClassSIMID + clsFunction.sPipe +
                                                clsSearch.ClassClientID + clsFunction.sPipe +
                                                clsSearch.ClassFEID + clsFunction.sPipe +
                                                clsSearch.ClassTID + clsFunction.sPipe +
                                                clsSearch.ClassMID + clsFunction.sPipe +
                                                clsSearch.ClassJobTypeDescriptionList + clsFunction.sPipe +
                                                clsSearch.ClassJobTypeStatusDescriptionList;

            Debug.WriteLine("clsSearch.ClassAdvanceSearchValue="+ clsSearch.ClassAdvanceSearchValue);

            InitPage(int.Parse(clsFunction.sOne), GetTotalPage());
        }
    }
}
