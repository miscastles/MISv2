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
using MIS.Enums;

namespace MIS
{
    public partial class frmSearchField : Form
    {
        public static int iGroupType = 0;
        public static bool fSelected = false;
        public static string sHeader = "";        
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        public static SearchType iSearchType;        
        public static int iStatus;
        public static string sTerminalType;
        public static string sSearchChar;
        int iFormWidth = 0;
        int iFormHeight = 0;

        public static int iLocationID = 0;
        public static string sLocation = clsFunction.sDefaultSelect;
        public static bool isCheckBoxes = false;
        public static bool isPreview = false;

        // The column we are currently using for sorting.
        private ColumnHeader SortingColumn = null;

        // Controller
        private MSPMasterController _mMSPMasterController;
        private TypeController _mTypeController;
        private HelpDeskController _mHelpDeskController;

        public frmSearchField()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwSearch, true);

            // Initialize the controller object
            _mMSPMasterController = new MSPMasterController();
            _mTypeController = new TypeController();
            _mHelpDeskController = new HelpDeskController();
        }

        public enum SearchType
        {
            iTerminal, iSIM, iMerchant,
            iAllReason, iReason, iNegativeReason, iResolution, iFSRAttempt,
            iIR, iProvince, iRegion,
            iFE, iSP, iClient,
            iTerminalStatus, iSIMStatus, iDashboard,
            iTerminalType, iTerminalModel, iTerminalBrand,
            iEMP, iSupplier,
            iWorkArrangement, iLeaveDetail,
            iTimeSheetTerminal, iCoountry,
            iDispatch,
            iService, iFSR,
            iMerchantList,
            iSetup,
            iReleaseTerminal, iReleaseSIM,
            iInvoice, iSearchInvoice,
            iMobile,
            iDispatcher,
            iStockDetail,
            iIRMerchantList,
            iMSPMasterList,
            iPendingReason,
            iHelpDesk,
            iTypeList,
            iProblem,
            iHelpDeskProblem
        }

        private void lvwSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwSearch.SelectedItems.Count > 0)
            {
                string LineNo = lvwSearch.SelectedItems[0].Text;
                txtLineNo.Text = LineNo;

                if (LineNo.Length > 0)
                {   
                    switch (iSearchType)
                    {
                        case SearchType.iIRMerchantList:
                            clsSearch.ClassParticularID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassParticularName = lvwSearch.SelectedItems[0].SubItems[2].Text;
                            break;
                        case SearchType.iMerchant:
                        case SearchType.iFE:
                        case SearchType.iSP:
                        case SearchType.iClient:
                        case SearchType.iDashboard:
                        case SearchType.iDispatcher:
                            clsSearch.ClassParticularID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassParticularName = lvwSearch.SelectedItems[0].SubItems[2].Text;
                            clsSearch.ClassTID = lvwSearch.SelectedItems[0].SubItems[3].Text;
                            clsSearch.ClassMID = lvwSearch.SelectedItems[0].SubItems[4].Text;
                            clsSearch.ClassParticularAddress = lvwSearch.SelectedItems[0].SubItems[5].Text;
                            clsSearch.ClassParticularContactPerson = lvwSearch.SelectedItems[0].SubItems[6].Text; 
                            clsSearch.ClassParticularMobileNo = lvwSearch.SelectedItems[0].SubItems[7].Text;
                            clsSearch.ClassParticularTelNo = lvwSearch.SelectedItems[0].SubItems[8].Text;
                            clsSearch.ClassEmail = lvwSearch.SelectedItems[0].SubItems[9].Text;

                            if (iSearchType.Equals(SearchType.iMerchant))
                            {
                                clsSearch.ClassIRIDNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[10].Text);
                                clsSearch.ClassIRNo = lvwSearch.SelectedItems[0].SubItems[11].Text;
                                clsSearch.ClassServiceStatusDescription = lvwSearch.SelectedItems[0].SubItems[12].Text;
                                clsSearch.ClassClientID = int.Parse(lvwSearch.SelectedItems[0].SubItems[13].Text);
                                clsSearch.ClassRequestID = lvwSearch.SelectedItems[0].SubItems[11].Text;
                            }
                            else
                            {
                                clsSearch.ClassDepartment = lvwSearch.SelectedItems[0].SubItems[10].Text;
                                clsSearch.ClassPosition = lvwSearch.SelectedItems[0].SubItems[11].Text;
                            }
                            
                            clsSearch.ClassEmploymentStatus = lvwSearch.SelectedItems[0].SubItems[12].Text;
                            

                            break;

                        case SearchType.iMerchantList:
                            clsSearch.ClassParticularID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassParticularName = lvwSearch.SelectedItems[0].SubItems[2].Text;
                            
                            break;

                        case SearchType.iSIM:                            
                            clsSearch.ClassSIMID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassSIMSerialNo = lvwSearch.SelectedItems[0].SubItems[2].Text;
                            clsSearch.ClassSIMCarrier = lvwSearch.SelectedItems[0].SubItems[3].Text;
                            clsSearch.ClassSIMStatus = int.Parse(lvwSearch.SelectedItems[0].SubItems[4].Text); 
                            break;
                        case SearchType.iTerminal:
                        case SearchType.iStockDetail:
                            clsSearch.ClassTerminalID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassTerminalSN = lvwSearch.SelectedItems[0].SubItems[2].Text;
                            clsSearch.ClassTerminalType = lvwSearch.SelectedItems[0].SubItems[3].Text;
                            clsSearch.ClassTerminalModel = lvwSearch.SelectedItems[0].SubItems[4].Text;
                            clsSearch.ClassTerminalBrand = lvwSearch.SelectedItems[0].SubItems[5].Text;
                            clsSearch.ClassTerminalStatus = int.Parse(lvwSearch.SelectedItems[0].SubItems[6].Text);                            
                            break;
                        case SearchType.iAllReason:
                        case SearchType.iReason:
                        case SearchType.iNegativeReason:
                        case SearchType.iResolution:
                        case SearchType.iProblem:
                            clsSearch.ClassReasonID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassReasonDescription = lvwSearch.SelectedItems[0].SubItems[2].Text;
                            clsSearch.ClassReasonCode = lvwSearch.SelectedItems[0].SubItems[3].Text;
                            clsSearch.ClassReasonIsInput = int.Parse(lvwSearch.SelectedItems[0].SubItems[4].Text);
                            break;
                        case SearchType.iFSRAttempt:
                            clsSearch.ClassTAIDNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            break;
                        case SearchType.iIR:
                            clsSearch.ClassIRIDNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassIRNo = lvwSearch.SelectedItems[0].SubItems[2].Text;
                            break;
                        case SearchType.iRegion:                            
                            clsSearch.ClassRegionType = int.Parse(lvwSearch.SelectedItems[0].SubItems[2].Text);                            
                            clsSearch.ClassRegion = lvwSearch.SelectedItems[0].SubItems[4].Text;
                            break;
                        case SearchType.iProvince:
                            clsSearch.ClassRegionID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassRegionType = int.Parse(lvwSearch.SelectedItems[0].SubItems[2].Text);
                            clsSearch.ClassProvince  = lvwSearch.SelectedItems[0].SubItems[3].Text;
                            clsSearch.ClassRegion = lvwSearch.SelectedItems[0].SubItems[4].Text;
                            break;                        
                        case SearchType.iTerminalStatus:
                        case SearchType.iSIMStatus:
                            clsSearch.ClassTerminalStatusID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassTerminalStatusType = int.Parse(lvwSearch.SelectedItems[0].SubItems[2].Text);
                            clsSearch.ClassTerminalStatusDescription = lvwSearch.SelectedItems[0].SubItems[3].Text;
                            break;
                        case SearchType.iTerminalType:
                            clsSearch.ClassTerminalTypeID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);                            
                            clsSearch.ClassTerminalType = lvwSearch.SelectedItems[0].SubItems[2].Text;
                            break;
                        case SearchType.iTerminalModel:
                            clsSearch.ClassTerminalModelID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassTerminalModel = lvwSearch.SelectedItems[0].SubItems[2].Text;
                            break;
                        case SearchType.iTerminalBrand:
                            clsSearch.ClassTerminalBrandID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassTerminalBrand = lvwSearch.SelectedItems[0].SubItems[2].Text;
                            break;
                        case SearchType.iWorkArrangement:
                            clsSearch.ClassWorkArrangementID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassWorkTypeID = int.Parse(lvwSearch.SelectedItems[0].SubItems[2].Text);
                            clsSearch.ClassCode = lvwSearch.SelectedItems[0].SubItems[3].Text;
                            clsSearch.ClassDescription = lvwSearch.SelectedItems[0].SubItems[4].Text;
                            clsSearch.ClassDateFrom = lvwSearch.SelectedItems[0].SubItems[5].Text;
                            clsSearch.ClassDateTo = lvwSearch.SelectedItems[0].SubItems[6].Text;
                            clsSearch.ClassDuration = lvwSearch.SelectedItems[0].SubItems[7].Text;
                            clsSearch.ClassDateType = lvwSearch.SelectedItems[0].SubItems[8].Text;
                            clsSearch.ClassRemarks = lvwSearch.SelectedItems[0].SubItems[9].Text;
                            break;
                        case SearchType.iLeaveDetail:
                            clsSearch.ClassLeaveNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassLeaveTypeID = int.Parse(lvwSearch.SelectedItems[0].SubItems[2].Text);
                            clsSearch.ClassCode = lvwSearch.SelectedItems[0].SubItems[3].Text;
                            clsSearch.ClassDescription = lvwSearch.SelectedItems[0].SubItems[4].Text;
                            clsSearch.ClassDateFrom = lvwSearch.SelectedItems[0].SubItems[5].Text;
                            clsSearch.ClassDateTo = lvwSearch.SelectedItems[0].SubItems[6].Text;
                            clsSearch.ClassDuration = lvwSearch.SelectedItems[0].SubItems[7].Text;
                            clsSearch.ClassDateType = lvwSearch.SelectedItems[0].SubItems[8].Text;
                            clsSearch.ClassReasonID = int.Parse(lvwSearch.SelectedItems[0].SubItems[9].Text);
                            clsSearch.ClassReasonDescription = lvwSearch.SelectedItems[0].SubItems[10].Text;
                            clsSearch.ClassRemarks = lvwSearch.SelectedItems[0].SubItems[11].Text;
                            break;
                        case SearchType.iTimeSheetTerminal:
                            clsSearch.ClassTSID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassTSTerminalID = lvwSearch.SelectedItems[0].SubItems[2].Text;
                            clsSearch.ClassTSTerminalName = lvwSearch.SelectedItems[0].SubItems[3].Text;

                            break;
                        case SearchType.iCoountry:
                            clsSearch.ClassCountryID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassCountry = lvwSearch.SelectedItems[0].SubItems[2].Text;
                            break;
                        case SearchType.iDispatch:
                        case SearchType.iService:
                        case SearchType.iFSR:
                            clsSearch.ClassServiceNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassClientID = int.Parse(lvwSearch.SelectedItems[0].SubItems[2].Text);
                            clsSearch.ClassMerchantID = int.Parse(lvwSearch.SelectedItems[0].SubItems[3].Text);
                            clsSearch.ClassFEID = int.Parse(lvwSearch.SelectedItems[0].SubItems[4].Text);
                            clsSearch.ClassParticularName = lvwSearch.SelectedItems[0].SubItems[5].Text;
                            clsSearch.ClassTID = lvwSearch.SelectedItems[0].SubItems[6].Text;
                            clsSearch.ClassMID = lvwSearch.SelectedItems[0].SubItems[7].Text;

                            clsSearch.ClassIRIDNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[10].Text);
                            clsSearch.ClassIRNo = lvwSearch.SelectedItems[0].SubItems[11].Text;

                            clsSearch.ClassServiceStatusDescription = lvwSearch.SelectedItems[0].SubItems[18].Text;
                            clsSearch.ClassFSRNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[20].Text);
                            break;
                        case SearchType.iSetup:
                            clsSearch.ClassID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassDescription = lvwSearch.SelectedItems[0].SubItems[2].Text;
                            break;
                        case SearchType.iInvoice:
                            clsSearch.ClassInvoiceID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassInvoiceNo = lvwSearch.SelectedItems[0].SubItems[4].Text;
                            break;
                        case SearchType.iMobile:
                            clsSearch.ClassMobileID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            
                            break;
                        case SearchType.iMSPMasterList:
                            clsSearch.ClassMSPNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassClientID = int.Parse(lvwSearch.SelectedItems[0].SubItems[2].Text);
                            clsSearch.ClassMerchantID = int.Parse(lvwSearch.SelectedItems[0].SubItems[3].Text);
                            clsSearch.ClassSubmitID = int.Parse(lvwSearch.SelectedItems[0].SubItems[4].Text);
                            break;

                        case SearchType.iHelpDesk:
                            clsSearch.ClassIRIDNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.AssistNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[2].Text);
                            clsSearch.ClassMerchantID = int.Parse(lvwSearch.SelectedItems[0].SubItems[3].Text);
                            clsSearch.ClassClientID = int.Parse(lvwSearch.SelectedItems[0].SubItems[4].Text);
                            clsSearch.ClassRequestNo = lvwSearch.SelectedItems[0].SubItems[8].Text;
                            clsSearch.ClassRequestID = lvwSearch.SelectedItems[0].SubItems[9].Text;

                            break;
                        case SearchType.iTypeList:
                            clsSearch.ClassTypeID = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassDescription = lvwSearch.SelectedItems[0].SubItems[2].Text;
                            break;
                        case SearchType.iHelpDeskProblem:
                            clsSearch.AssistNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassJobType = int.Parse(lvwSearch.SelectedItems[0].SubItems[2].Text);
                            clsSearch.ProblemNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[3].Text);
                            //clsSearch.ClassClientID = int.Parse(lvwSearch.SelectedItems[0].SubItems[3].Text);
                            clsSearch.ClassMerchantID = int.Parse(lvwSearch.SelectedItems[0].SubItems[5].Text);
                            clsSearch.ClassIRIDNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[6].Text);
                            clsSearch.ClassParticularName = lvwSearch.SelectedItems[0].SubItems[7].Text;
                            clsSearch.ClassTID = lvwSearch.SelectedItems[0].SubItems[8].Text;
                            clsSearch.ClassMID = lvwSearch.SelectedItems[0].SubItems[9].Text;
                            clsSearch.ClassJobTypeDescription = lvwSearch.SelectedItems[0].SubItems[12].Text;

                            break;

                    }                             
                }
            }
        }

        private void frmSearchField_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("--frmSearchField_Load--");

            fSelected = false;  
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            lblHeader.Text = lblHeader.Text + " " + "[ " + sHeader + " ]";            
            lblSearchStatus.Text = "";
            lblSearchMessage.Text = "";
            
            InitPage(0, 0);
            clsSearch.ClassCurrentPage = int.Parse(clsFunction.sOne);

            //frmLoading frmWait = new frmLoading(); // Open Wait Window
            //clsFunction.WaitWindow(true, frmWait);

            Debug.WriteLine("lblHeader=" + lblHeader.Text);
            Debug.WriteLine("iSearchType="+ iSearchType);

            //ProcessPage(iSearchType);
            //LoadListView(iSearchType);
            
            //clsFunction.WaitWindow(false, frmWait); // Close Wait Window

            InitSearchStringHeader();

            InitListView();

            // Set from width and height
            iFormHeight = 550;
            iFormWidth += 50;
            SetDefaultWindowSize(iFormWidth, iFormHeight);
             
            chkShowAll_CheckedChanged(this, e);

            // Clear          
            clsSearch.ClassClientID =
            clsSearch.ClassFEID =
            clsSearch.ClassParticularID =
            clsSearch.ClassIRIDNo =
            clsSearch.ClassLastServiceNo =
            clsSearch.ClassTerminalID =
            clsSearch.ClassSIMID =
            clsSearch.ClassRepTerminalID =
            clsSearch.ClassRepSIMID = 
            clsSearch.ClassLocationID =
            clsSearch.ClassServiceStatus =
            clsSearch.ClassIsReleased =
            clsFunction.iZero;

            // Clear
            clsSearch.ClassParticularName =
            clsSearch.ClassTID =
            clsSearch.ClassMID =
            clsSearch.ClassIRNo = 
            clsSearch.ClassLocation =
            clsSearch.ClassServiceStatusDescription =
            clsFunction.sDash;

            // 

            //txtSearch.Focus();

            //ROCKY - SEARCH FIELD ISSUE: FIX LOCATION MISMATCH WHEN EXPANDED
            if (this.Width > 900)
            {
                StartPosition = FormStartPosition.Manual;
                Location = new Point(170, 250);
            }
            else
            {
                StartPosition = FormStartPosition.CenterScreen;
            }
        }

        private void LoadListView(SearchType iType)
        {
            int lineno = 0;
            List<MSPMasterController> mListMSPMaster = null;
            List<TypeController> mListType = null;
            List<HelpDeskController> mListHelpDesk = null;

            Debug.WriteLine("--LoadListView--");
            Debug.WriteLine("iType="+ iType);

            Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

            lblSearchStatus.Text = "";
            lblSearchMessage.Text = "";            
            lvwSearch.Items.Clear();

            txtSearch.Text = txtSearch.Text.Trim();

            if (txtSearch.Text.Length > 0)
            {
                InitPage(0, 0);
            }

            clsSearch.ClassCurrentPage = 1;

            switch (iType)
            {
                case SearchType.iTerminal:                    
                    clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
                                                        (chkShowAll.Checked ? clsFunction.iZero : iStatus) + clsFunction.sPipe +
                                                        clsFunction.sNull + clsFunction.sPipe +
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit() + clsFunction.sPipe +
                                                        iLocationID + clsFunction.sPipe +
                                                        sLocation;
                    
                    dbAPI.FillListViewTerminalSN(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iSIM:                    
                    clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
                                                        (chkShowAll.Checked ? clsFunction.iZero : iStatus) + clsFunction.sPipe +
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit() + clsFunction.sPipe +
                                                        iLocationID + clsFunction.sPipe +
                                                        sLocation;

                    dbAPI.FillListViewSIMSN(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iAllReason:
                    clsSearch.ClassAdvanceSearchValue = clsFunction.sPadZero + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe + clsFunction.sZero;
                    dbAPI.FillListViewReason(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iReason:
                    clsSearch.ClassAdvanceSearchValue = clsFunction.sPadZero + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe + clsGlobalVariables.REASON_TYPE;
                    dbAPI.FillListViewReason(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iNegativeReason:
                    clsSearch.ClassAdvanceSearchValue = clsFunction.sPadZero + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe + clsGlobalVariables.NEGATIVE_TYPE;
                    dbAPI.FillListViewReason(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iResolution:
                    clsSearch.ClassAdvanceSearchValue = clsFunction.sPadZero + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe + clsGlobalVariables.RESOLUTION_TYPE;
                    dbAPI.FillListViewReason(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iFSRAttempt:                    
                    dbAPI.FillListViewAttempt(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iIR:
                    clsSearch.ClassAdvanceSearchValue = txtSearch.Text + clsFunction.sPipe +
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit(); ;

                    dbAPI.FillListViewIRNo(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iRegion:                    
                    clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe + 
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();

                    dbAPI.FillListViewRegion(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iProvince:                    
                    clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe + 
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();

                    dbAPI.FillListViewProvince(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iClient:                    
                    clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.iClient_Type.ToString() + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe + 
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();

                    dbAPI.FillParticularListView(lvwSearch, clsGlobalVariables.sClient_Type, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iMerchant:                    
                    clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.iMerchant_Type.ToString() + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();

                    dbAPI.FillParticularListView(lvwSearch, clsGlobalVariables.sMerchant_Type, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iFE:
                case SearchType.iDispatcher:
                    clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.iFE_Type.ToString() + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();

                    dbAPI.FillParticularListView(lvwSearch, clsGlobalVariables.sFE_Type, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iSP:                    
                    clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.iSP_Type.ToString() + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();

                    dbAPI.FillParticularListView(lvwSearch, clsGlobalVariables.sSP_Type, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iTerminalStatus:                    
                    clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe + clsGlobalVariables.iTerminal_Type.ToString() + clsFunction.sPipe + txtSearch.Text;
                    dbAPI.FillListViewTerminalStatus(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iSIMStatus:                    
                    clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe + clsGlobalVariables.iTerminal_Type.ToString() + clsFunction.sPipe + txtSearch.Text;
                    dbAPI.FillListViewTerminalStatus(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iDashboard:
                    clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.iDashboard_Type.ToString() + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();

                    dbAPI.FillParticularListView(lvwSearch, clsFunction.sNull, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iTerminalType:
                    clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe;
                    dbAPI.FillListViewTerminalType(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iTerminalModel:
                    clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe;
                    dbAPI.FillListViewTerminalModel(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iTerminalBrand:
                    clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe;
                    dbAPI.FillListViewTerminalBrand(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iWorkArrangement:
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassParticularID.ToString() + clsFunction.sPipe + clsFunction.sZero;
                    dbAPI.FillListViewWorkArrangement(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iLeaveDetail:
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassParticularID.ToString() + clsFunction.sPipe + clsFunction.sZero;
                    dbAPI.FillListViewLeaveDetail(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iTimeSheetTerminal:
                    dbAPI.FillListViewAdvanceDetail(lvwSearch, "View", "TimeSheet Terminal", "");
                    break;
                case SearchType.iCoountry:
                    dbAPI.FillListViewAdvanceDetail(lvwSearch, "View", "Country", "");
                    break;
                case SearchType.iDispatch:
                    clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.STATUS_DISPATCH_DESC + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING_DESC+ clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();
                    dbAPI.FillListViewAServiceDispatch(lvwSearch, "View", "Dispatch Servicing 2", clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iService:             
                    clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        //clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();
                    dbAPI.FillListViewAServiceDispatch(lvwSearch, "View", "Dispatch Servicing 2", clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iFSR:
                    clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();
                    dbAPI.FillListViewAServiceDispatch(lvwSearch, "View", "Dispatch Servicing 2", clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iMerchantList:
                    clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.iMerchant_Type_List.ToString() + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();

                    dbAPI.FillParticularListView(lvwSearch, clsGlobalVariables.sMerchant_Type_List, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iSetup:
                    dbAPI.FillListViewAdvanceDetail(lvwSearch, "View", "Setup", "");
                    break;
                case SearchType.iReleaseTerminal:
                    clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
                                                        clsFunction.sDateFormat + clsFunction.sPipe +
                                                        clsFunction.sDateFormat + clsFunction.sPipe +
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();

                    dbAPI.FillListViewTerminalDetail(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iReleaseSIM:
                    clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
                                                        clsFunction.sDateFormat + clsFunction.sPipe +
                                                        clsFunction.sDateFormat + clsFunction.sPipe +
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();

                    dbAPI.FillListViewSIMDetail(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iInvoice:
                    clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtSearch.Text);

                    dbAPI.FillListViewInvoiceMaster(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iSearchInvoice:
                    clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe + 
                                                        dbFunction.CheckAndSetNumericValue(clsSearch.ClassParticularID.ToString()) + clsFunction.sPipe +
                                                        clsFunction.sZero;

                    dbAPI.FillListViewInvoiceMaster(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iMobile:
                    dbAPI.FillListViewMobileList(lvwSearch, "");
                    break;
                case SearchType.iStockDetail:
                    clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +                                                        
                                                        iStatus + clsFunction.sPipe +
                                                        clsDefines.SEARCH_COMPONENTS + clsFunction.sPipe +
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit() + clsFunction.sPipe +
                                                        iLocationID + clsFunction.sPipe +
                                                        sLocation;

                    dbAPI.FillListViewStockDetail(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iIRMerchantList:
                    clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.iMerchant_Type.ToString() + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +                                                        
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();

                    dbAPI.FillMerchantListView(lvwSearch, clsGlobalVariables.sMerchant_Type, clsSearch.ClassAdvanceSearchValue);
                    break;
                case SearchType.iMSPMasterList:

                    mListMSPMaster = _mMSPMasterController.getDetailList(clsFunction.sZero + clsFunction.sPipe + dbFunction.CheckAndSetStringValue(txtSearch.Text) + clsFunction.sPipe + clsSearch.ClassCurrentPage + clsFunction.sPipe + dbFunction.GetPageLimit());

                    if (mListMSPMaster != null)
                    {
                        foreach (var itemData in mListMSPMaster)
                        {
                            lineno++;
                            ListViewItem item = new ListViewItem(lineno.ToString());
                            item.SubItems.Add(itemData.MSPNo.ToString());
                            item.SubItems.Add(itemData.ClientID.ToString());
                            item.SubItems.Add(itemData.MerchantID.ToString());
                            item.SubItems.Add(itemData.SubmitID.ToString());
                            item.SubItems.Add(itemData.RegisteredName);
                            item.SubItems.Add(itemData.CreatedAt);
                            item.SubItems.Add(itemData.CreatedBy);
                            item.SubItems.Add(itemData.ReferenceNo);                            
                            item.SubItems.Add(itemData.SubmitAt);
                            item.SubItems.Add(itemData.SubmitBy);
                            item.SubItems.Add(itemData.Category);
                            item.SubItems.Add(itemData.NoBType);
                            item.SubItems.Add(itemData.BusType);
                            item.SubItems.Add(itemData.SchemeType);
                            item.SubItems.Add(itemData.ReferralType);
                            item.SubItems.Add(itemData.MSPStatusDesc);

                            lvwSearch.Items.Add(item);
                        }

                        dbFunction.ListViewAlternateBackColor(lvwSearch);

                    }

                    break;

                case SearchType.iHelpDesk:
                    LoadHelpdeskMaster(txtSearch.Text);
                    break;
                case SearchType.iTypeList:

                    mListType = _mTypeController.getDetailList(clsFunction.sZero + clsFunction.sPipe + iGroupType);

                    if (mListType != null)
                    {
                        foreach (var itemData in mListType)
                        {
                            lineno++;
                            ListViewItem item = new ListViewItem(lineno.ToString());
                            item.SubItems.Add(itemData.TypeID.ToString());
                            item.SubItems.Add(itemData.Description);

                            lvwSearch.Items.Add(item);
                        }

                        dbFunction.ListViewAlternateBackColor(lvwSearch);
                    }

                    break;

                case SearchType.iProblem:
                    clsSearch.ClassAdvanceSearchValue = clsFunction.sPadZero + clsFunction.sPipe + dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe + clsGlobalVariables.PROBLEM_TYPE;
                    dbAPI.FillListViewReason(lvwSearch, clsSearch.ClassAdvanceSearchValue);
                    break;

                case SearchType.iHelpDeskProblem:

                    mListHelpDesk = _mHelpDeskController.getDetailList($"{clsDefines.gZero}{clsDefines.gPipe}{dbFunction.CheckAndSetStringValue(txtSearch.Text)}");

                    if (mListHelpDesk != null)
                    {
                        foreach (var itemData in mListHelpDesk)
                        {
                            lineno++;
                            ListViewItem item = new ListViewItem(lineno.ToString());
                            item.SubItems.Add(itemData.AssistNo.ToString());
                            item.SubItems.Add(itemData.JobType.ToString());
                            item.SubItems.Add(itemData.ProblemNo.ToString());
                            item.SubItems.Add(itemData.ClientID.ToString());
                            item.SubItems.Add(itemData.MerchantID.ToString());
                            item.SubItems.Add(itemData.IRIDNo.ToString());                            
                            item.SubItems.Add(itemData.MerchantName);
                            item.SubItems.Add(itemData.TID);
                            item.SubItems.Add(itemData.MID);
                            item.SubItems.Add(itemData.CreatedDate);
                            item.SubItems.Add(itemData.RequestDate);
                            item.SubItems.Add(itemData.ServiceJobTypeDescription);
                            item.SubItems.Add(itemData.RequestID);
                            item.SubItems.Add(itemData.ProblemReported);
                            item.SubItems.Add(itemData.HelpDeskName);
                            item.SubItems.Add(itemData.TeamLeadName);
                            
                            lvwSearch.Items.Add(item);
                        }

                        dbFunction.ListViewAlternateBackColor(lvwSearch);

                    }

                    break;
            }
            
            lblSearchStatus.Text = lvwSearch.Items.Count.ToString() + " " + "record(s) found.";
            
            if (iType == SearchType.iTerminal || iType == SearchType.iSIM)
            {
                if (iStatus == clsGlobalVariables.STATUS_AVAILABLE)
                    lblSearchMessage.Text = "*ONLY AVAILABLE STATUS WILL BE LISTED";
                else
                    lblSearchMessage.Text = "*ALL RECORD(S) WILL BE LISTED";
            }
            else
            {
                lblSearchMessage.Text = "";
            }

            Cursor.Current = Cursors.Default; // Back to normal
            
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmSearchField_Activated(object sender, EventArgs e)
        {
            txtSearch.Focus();
        }

        private void frmSearchField_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    fSelected = false;
                    this.Close();
                    break;
                case Keys.Enter:
                    lvwSearch_DoubleClick(this, e);
                    break;
            }
        }
        
        private void lvwSearch_DoubleClick(object sender, EventArgs e)
        {
            List<string> IDCol = new List<String>();
            List<string> DescriptionCol = new List<String>();

            string pMessage = "";
            bool isConfrim = false;
            string pJobType = "";

            if (lvwSearch.Items.Count > 0 && dbFunction.isValidID(txtLineNo.Text))
            {

                string pSelectedRow = dbFunction.GetListViewSelectedRow(lvwSearch, 0);
                Debug.WriteLine("pSelectedRow=\n" + pSelectedRow);

                fSelected = true;

                switch (iSearchType)
                {   
                    case SearchType.iMerchant:
                        pJobType = dbFunction.GetSearchValue("Job Type");
                        pMessage = "Are you sure to select the following merchant details below:\n" +
                                   clsFunction.sLineSeparator + "\n" +
                                   (dbFunction.isValidDescription(pJobType) ? "JOB TYPE: " + pJobType + "\n\n" : "") +
                                   "> Merchant ID.: " + dbFunction.GetSearchValue("ID") + "\n" +                                   
                                   "> NAME: " + dbFunction.GetSearchValue("MERCHANT") + "\n" +
                                   "> TID: " + dbFunction.GetSearchValue("TID") + "\n" +
                                   "> MID: " + dbFunction.GetSearchValue("MID");
                        isConfrim = true;
                        break;
                    case SearchType.iService:
                    case SearchType.iFSR:
                    case SearchType.iDispatch:
                        pJobType = dbFunction.GetSearchValue("Job Type");
                        pMessage = "Are you sure to select the following merchant details below:\n" +
                                   clsFunction.sLineSeparator + "\n" +
                                   (dbFunction.isValidDescription(pJobType) ? "JOB TYPE: " + pJobType  + "\n\n" : "") +
                                   "> Client ID.: " + dbFunction.GetSearchValue("CLIENTID") + "\n" +
                                   "> Merchant ID.: " + dbFunction.GetSearchValue("MERCHANTID") + "\n" +
                                   "> Service No.: " + dbFunction.GetSearchValue("SERVICENO") + "\n" +
                                   "> FSR No.: " + dbFunction.GetSearchValue("FSRNO") + "\n" +
                                   "> NAME: " + dbFunction.GetSearchValue("MERCHANT") + "\n" +
                                   "> TID: " + dbFunction.GetSearchValue("TID") + "\n" +
                                   "> MID: " + dbFunction.GetSearchValue("MID");

                        isConfrim = true;
                        break;
                    case SearchType.iIRMerchantList:
                    case SearchType.iMerchantList:
                    case SearchType.iFE:
                    case SearchType.iSP:
                    case SearchType.iClient:
                    case SearchType.iDispatcher:
                        pMessage = "Are you sure to select the following particular details below:\n" +
                                   clsFunction.sLineSeparator + "\n" +                                 
                                   "> ID.: " + dbFunction.GetSearchValue("ID") + "\n" +
                                   "> NAME: " + dbFunction.GetSearchValue("MERCHANT");

                        isConfrim = true;
                        break;
                    case SearchType.iTerminal:

                        pMessage = "Are you sure to select the following terminal details below:\n" +
                                   clsFunction.sLineSeparator + "\n" +
                                   "> ID.: " + dbFunction.GetSearchValue("ID") + "\n" +
                                   "> SERIAL NO.: " + dbFunction.GetSearchValue("SERIAL NO.") + "\n" +
                                   "> TYPE: " + dbFunction.GetSearchValue("TYPE") + "\n" +
                                   "> MODEL: " + dbFunction.GetSearchValue("MODEL") + "\n" +
                                   "> BRAND: " + dbFunction.GetSearchValue("BRAND");


                        isConfrim = true;

                        break;
                    case SearchType.iSIM:

                        pMessage = "Are you sure to select the following sim details below:\n" +
                                   clsFunction.sLineSeparator + "\n" +
                                   "> ID.: " + dbFunction.GetSearchValue("ID") + "\n" +
                                   "> SERIAL NO.: " + dbFunction.GetSearchValue("SERIAL NO.");

                        isConfrim = true;

                        break;
                    case SearchType.iStockDetail:

                        pMessage = "Are you sure to select the following stock details below:\n" +
                                   clsFunction.sLineSeparator + "\n" +
                                   "> ID.: " + dbFunction.GetSearchValue("ID") + "\n" +
                                   "> SERIAL NO.: " + dbFunction.GetSearchValue("SERIAL NO.") + "\n" +
                                   "> TYPE: " + dbFunction.GetSearchValue("TYPE") + "\n" +
                                   "> MODEL: " + dbFunction.GetSearchValue("MODEL") + "\n" +
                                   "> BRAND: " + dbFunction.GetSearchValue("BRAND");


                        isConfrim = true;

                        break;

                    case SearchType.iMSPMasterList:
                        pMessage = "Are you sure to select the following MSP details below:\n" +
                                   clsFunction.sLineSeparator + "\n" +
                                   "> MSP No.: " + dbFunction.GetSearchValue("MSPNo") + "\n" +
                                   "> Client ID: " + dbFunction.GetSearchValue("ClientID") + "\n" +
                                   "> Merchant ID: " + dbFunction.GetSearchValue("MerchantID") + "\n" +
                                   "> Submit ID: " + dbFunction.GetSearchValue("SubmitID") + "\n" +
                                   "> Reference No: " + dbFunction.GetSearchValue("Reference No") + "\n" +
                                   "> Registered Name: " + dbFunction.GetSearchValue("Registered Name") + "\n" +
                                   "> Created By: " + dbFunction.GetSearchValue("Created By") + "\n" +
                                   "> Submit By: " + dbFunction.GetSearchValue("Submit By");

                        isConfrim = true;

                        break;
                    case SearchType.iHelpDesk:
                        pMessage = "Are you sure to select the following Request Details below:\n" +
                                    clsFunction.sLineSeparator + "\n" +
                                    "ASSIST NO.: " + dbFunction.GetSearchValue("ASSISTNO") + "\n" +
                                    "REQUEST ID.: " + dbFunction.GetSearchValue("REQUESTID") + "\n" +
                                    "REFERENCE NO.: " + dbFunction.GetSearchValue("REFERENCE NO") + "\n" +
                                    "TICKET STATUS.: " + dbFunction.GetSearchValue("TICKETSTATUS") + "\n" +
                                    //"MERCHANT ID: " + dbFunction.GetSearchValue("MERCHANT ID") + "\n" +
                                    "MERCHANT NAME: " + dbFunction.GetSearchValue("MERCHANT NAME") + "\n" +
                                    "TID: " + dbFunction.GetSearchValue("TID") + "\n" +
                                    "MID: " + dbFunction.GetSearchValue("MID") + "\n" +
                                    clsFunction.sLineSeparator + "\n";
                        isConfrim = true;

                        break;

                    case SearchType.iHelpDeskProblem:
                        pMessage = "Are you sure to select the following Helpdesk Ticket details below:\n" +
                                   clsFunction.sLineSeparator + "\n" +
                                   "> Assist No.: " + dbFunction.GetSearchValue("AssistNo") + "\n" +
                                   "> Job Type No.: " + dbFunction.GetSearchValue("JobType") + "\n" +
                                   "> Problem No.: " + dbFunction.GetSearchValue("ProblemNo") + "\n" +
                                   "> Service Job Type: " + dbFunction.GetSearchValue("Service Job Type") + "\n" +
                                   "> Request ID: " + dbFunction.GetSearchValue("Request ID") + "\n" +
                                   "> Merchant ID: " + dbFunction.GetSearchValue("MerchantID") + "\n" +
                                   "> Merchant: " + dbFunction.GetSearchValue("Merchant") + "\n" +
                                   "> TID: " + dbFunction.GetSearchValue("TID") + "\n" +
                                   "> MID: " + dbFunction.GetSearchValue("MID") + "\n" +
                                   "> Created Date: " + dbFunction.GetSearchValue("Created Date") + "\n" +
                                   "> Request Date: " + dbFunction.GetSearchValue("Request Date");

                        isConfrim = true;
                        break;
                }
                
                if (isConfrim)
                {
                    if (MessageBox.Show(pMessage, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                    {
                        fSelected = false;
                        return;
                    }
                    else
                    {
                        fSelected = true;

                        if (!isCheckBoxes)
                        {
                            IDCol.Add(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            DescriptionCol.Add(lvwSearch.SelectedItems[0].SubItems[2].Text);

                            clsArray.ID = IDCol.ToArray();
                            clsArray.Description = DescriptionCol.ToArray();
                        }

                        this.Close();
                    }
                }
                else
                {
                    this.Close();
                }


                
                
            }            
        }

        private void lvwSearch_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Get the new sorting column.
            ColumnHeader new_sorting_column = lvwSearch.Columns[e.Column];

            // Figure out the new sorting order.
            System.Windows.Forms.SortOrder sort_order;

            if (SortingColumn == null)
            {
                // New column. Sort ascending.
                sort_order = SortOrder.Ascending;
            }
            else
            {
                // See if this is the same column.
                if (new_sorting_column == SortingColumn)
                {
                    // Same column. Switch the sort order.
                    if (SortingColumn.Text.StartsWith("> "))
                    {
                        sort_order = SortOrder.Descending;
                    }
                    else
                    {
                        sort_order = SortOrder.Ascending;
                    }
                }
                else
                {
                    // New column. Sort ascending.
                    sort_order = SortOrder.Ascending;
                }

                // Remove the old sort indicator.
                SortingColumn.Text = SortingColumn.Text.Substring(2);
            }

            // Display the new sort order.
            SortingColumn = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                SortingColumn.Text = "> " + SortingColumn.Text;
            }
            else
            {
                SortingColumn.Text = "< " + SortingColumn.Text;
            }

            // Create a comparer.
            lvwSearch.ListViewItemSorter = new clsListView(e.Column, sort_order);

            // Sort.
            lvwSearch.Sort();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:   
                    
                    if (txtSearch.Text.Length > 0)
                        LoadListView(iSearchType);

                    //if (lvwSearch.Items.Count > 0)
                    //    lvwSearch.Focus();

                    break;
                case Keys.Down:

                    //if (lvwSearch.Items.Count > 0)
                    //    lvwSearch.Focus();

                    break;
            }
        }        

        private void InitListView()
        {
            string outField = "";
            int outWidth = 0;
            string outTitle = "";
            HorizontalAlignment outAlign = 0;
            bool outVisible = false;
            bool outAutoWidth = false;
            string outFormat = "";

            Debug.WriteLine("--InitListView--");
            Debug.WriteLine("iSearchType="+ iSearchType);


            // Enable object
            lvwSearch.CheckBoxes = isCheckBoxes;
            chkSelect.Enabled = isCheckBoxes;
            btnOK.Enabled = isCheckBoxes;

            switch (iSearchType)
            {
                case SearchType.iMerchant:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Name", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "TID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "MID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Address", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Contact Person", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Mobile No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Tel No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Email", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "IRIDNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Request ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    //outWidth = 0;
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", (!isPreview ? "Dummy" : "Status"), out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);     
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ClientID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Region", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Province", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;

                case SearchType.iMerchantList:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Name", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Address", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Contact Person", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Mobile No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Tel No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Email", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Region", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Province", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;

                case SearchType.iClient:
                case SearchType.iSP:
                case SearchType.iDashboard:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Name", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Address", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Contact Person", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Mobile No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Tel No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Email", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Region", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Province", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;
                
                case SearchType.iFE:
                case SearchType.iDispatcher:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Name", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Address", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Contact Person", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Mobile No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Tel No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Email", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Department", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Position", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Employment", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Region", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Province", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;
                case SearchType.iSIM:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Serial No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Carrier", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);                  
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "StatusID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Status", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Client", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    //dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    //dbFunction.GetListViewHeaderColumnFromFile("", "Merchant", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    //dbFunction.GetListViewHeaderColumnFromFile("", "TID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    //dbFunction.GetListViewHeaderColumnFromFile("", "MID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    //dbFunction.GetListViewHeaderColumnFromFile("", "Request No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Location", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Allocation", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Delivery Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Received Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Released Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;


                    break;
                case SearchType.iTerminal:
                case SearchType.iStockDetail:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Serial No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Type", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Model", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Brand", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "StatusID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Status", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Client", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    //dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    //dbFunction.GetListViewHeaderColumnFromFile("", "Merchant", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    //dbFunction.GetListViewHeaderColumnFromFile("", "TID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    //dbFunction.GetListViewHeaderColumnFromFile("", "MID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    //dbFunction.GetListViewHeaderColumnFromFile("", "Request ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Location", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Allocation", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Asset Type", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Delivery Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Received Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Released Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;
                case SearchType.iAllReason:
                case SearchType.iReason:
                case SearchType.iNegativeReason:
                case SearchType.iResolution:
                case SearchType.iProblem:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Description", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Code", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ReasonIsInput", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;                
                case SearchType.iFSRAttempt:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "FSRNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "FSR Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "FSR Time", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "StatusID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Status", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Remarks", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;
                case SearchType.iIR:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "IRIDNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Request ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Merchant", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;
                case SearchType.iRegion:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "RegionID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "RegionType", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "City", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Region", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;
                case SearchType.iProvince:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "RegionID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "RegionType", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "City", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Region", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;
                case SearchType.iTerminalStatus:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "StatusID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Terminal Status", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;
                case SearchType.iSIMStatus:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "StatusID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "SIM Status", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;

                case SearchType.iTerminalType:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Type Description", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;
                case SearchType.iTerminalBrand:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Brand Description", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;
                case SearchType.iTerminalModel:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ModelID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Model Description", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "TypeID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Type Description", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;

                case SearchType.iWorkArrangement:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "WorkTypeID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Code", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Work Type", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Start Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "End Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Duration", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Date Type", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Remarks", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;
                case SearchType.iLeaveDetail:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LeaveTypeID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Code", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Work Type", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Start Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "End Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Duration", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Date Type", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ReasonID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Reason", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Remarks", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;
                    break;

                case SearchType.iTimeSheetTerminal:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Terminal ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Terminal Name", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;
                    break;
                case SearchType.iCoountry:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Country", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;                  
                    break;
                case SearchType.iDispatch:
                case SearchType.iService:
                case SearchType.iFSR:
                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ServiceNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ClientID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "MerchantID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "FEID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Name", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "TID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "MID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Client Name", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "FE Name", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "IRIDNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "IR No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Service No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;
                  
                    dbFunction.GetListViewHeaderColumnFromFile("", "Request Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;
                    
                    dbFunction.GetListViewHeaderColumnFromFile("", "Job Type", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    //dbFunction.GetListViewHeaderColumnFromFile("",(iSearchType.Equals(SearchType.iDispatch) ? "Dummy" : "Service Result"), out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    dbFunction.GetListViewHeaderColumnFromFile("", (iSearchType.Equals(SearchType.iDispatch) ? "Dummy" : "Dummy"), out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Reference No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Status", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Service Result", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "FSRNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    outWidth = 0;
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ServiceNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    outWidth = 0;
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "TerminalSN", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);                    
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "SIMSN", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ReplaceTerminalSN", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ReplaceSIMSN", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Schedule Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Serviced Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;


                    break;
                case SearchType.iSetup:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Description", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;
                case SearchType.iReleaseTerminal:
                case SearchType.iReleaseSIM:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "TransNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Batch No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "FromLocationID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Location From", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ToLocationID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Location To", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Request No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Reference No.", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;
                    
                    dbFunction.GetListViewHeaderColumnFromFile("", "Remarks", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Processed By", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Processed Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;
                case SearchType.iInvoice:
                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ParticularID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "MerchantID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "InvoiceNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "AccountNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "CustomerNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Name", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "InvoiceDate", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ReferenceNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "DateCovered", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "DateDue", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "TAmtDue", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Processed By", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Processed Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;

                case SearchType.iSearchInvoice:
                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ParticularID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "MerchantID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "InvoiceNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "AccountNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "CustomerNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Name", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "InvoiceDate", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ReferenceNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "DateCovered", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "DateDue", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "TAmtDue", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "TAmtPaid", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Dummy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;
                    
                    break;

                case SearchType.iMobile:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "MobileID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "MobileTerminalID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "MobileTerminalName", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "AssignedTo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;

                case SearchType.iIRMerchantList:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Name", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;
                    
                    dbFunction.GetListViewHeaderColumnFromFile("", "Address", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Contact Person", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Contact Position", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Contact Number", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;
                    
                    dbFunction.GetListViewHeaderColumnFromFile("", "Email", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;
                    
                    dbFunction.GetListViewHeaderColumnFromFile("", "Region", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Province", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;
                case SearchType.iMSPMasterList:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "MSPNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ClientID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "MerchantID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "SubmitID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "RegisteredName", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "CreatedAt", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "CreatedBy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ReferenceNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "SubmitAt", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "SubmitBy", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;
                    
                    dbFunction.GetListViewHeaderColumnFromFile("", "Category", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "NoBType", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "BusType", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "SchemeType", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ReferralType", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "MSPStatusDesc", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;
                    break;

                case SearchType.iTypeList:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "TypeID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;
                    
                    dbFunction.GetListViewHeaderColumnFromFile("", "Description", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;
                    break;

                case SearchType.iHelpDeskProblem:
                    lvwSearch.View = View.Details;

                    dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "AssistNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "JobType", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ProblemNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ClientID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "MerchantID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "IRIDNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;
                    
                    dbFunction.GetListViewHeaderColumnFromFile("", "Name", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "TID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "MID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Created Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Request Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ServiceJobTypeDescription", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "Request ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "ProblemReported", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "HelpDeskName", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    dbFunction.GetListViewHeaderColumnFromFile("", "TeamLeadName", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
                    lvwSearch.Columns.Add(outTitle, outWidth, outAlign);
                    iFormWidth += outWidth;

                    break;

            }            
        }

        private void lvwSearch_KeyDown(object sender, KeyEventArgs e)
        {
            fSelected = false;
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (lvwSearch.Items.Count > 0 && dbFunction.isValidID(txtLineNo.Text))
                    {
                        fSelected = true;
                    }
                    break;                    
            }

            if (fSelected) this.Close();
        }
        
        private void InitSearchStringHeader()
        {
            lblSearchString.Text = " > ";

            switch (iSearchType)
            {
                case SearchType.iTerminal:
                case SearchType.iStockDetail:
                    lblSearchString.Text = lblSearchString.Text + " " + "SERIAL NO. / TYPE / MODEL / BRAND / ASSET TYPE / LOCATION / STATUS";                    
                    break;
                case SearchType.iSIM:
                    lblSearchString.Text = lblSearchString.Text + " " + "SERIAL NO. / CARRIER / LOCATION / STATUS";                    
                    break;
                case SearchType.iAllReason:
                case SearchType.iReason:
                    lblSearchString.Text = lblSearchString.Text + " " + "REASON DESCRIPTION";                   
                    break;
                case SearchType.iResolution:
                    lblSearchString.Text = lblSearchString.Text + " " + "RESOLUTION DESCRIPTION";
                    break;
                case SearchType.iFSRAttempt:
                    lblSearchString.Text = lblSearchString.Text + " " + "ATTEMPT";                    
                    break;
                case SearchType.iIR:
                    lblSearchString.Text = lblSearchString.Text + " " + "REQUEST ID";                    
                    break;
                case SearchType.iRegion:
                    lblSearchString.Text = lblSearchString.Text + " " + "REGION";
                    break;
                case SearchType.iProvince:
                    lblSearchString.Text = lblSearchString.Text + " " + "PROVINCE";                   
                    break;
                case SearchType.iClient:
                    lblSearchString.Text = lblSearchString.Text + " " + "CLIENT";                   
                    break;
                case SearchType.iMerchant:
                    lblSearchString.Text = lblSearchString.Text + " " + "MERCHANT / TID / MID / REQUEST ID. / REFERENCE NO. / STATUS";                   
                    break;
                case SearchType.iMerchantList:
                    lblSearchString.Text = lblSearchString.Text + " " + "MERCHANT";
                    break;
                case SearchType.iFE:
                case SearchType.iDispatcher:
                    lblSearchString.Text = lblSearchString.Text + " " + "EMPLOYEE/FE/DISPATCHER";                                                    
                    break;
                case SearchType.iSP:
                    lblSearchString.Text = lblSearchString.Text + " " + "SERVICE PROVIDER";                   
                    break;
                case SearchType.iTerminalStatus:
                    lblSearchString.Text = lblSearchString.Text + " " + "TERMINAL STATUS";
                    break;
                case SearchType.iSIMStatus:
                    lblSearchString.Text = lblSearchString.Text + " " + "SIM STATUS";                   
                    break;
                case SearchType.iTerminalType:
                    lblSearchString.Text = lblSearchString.Text + " " + "TERMINAL TYPE";
                    break;
                case SearchType.iTerminalModel:
                    lblSearchString.Text = lblSearchString.Text + " " + "TERMINAL MODEL";
                    break;
                case SearchType.iTerminalBrand:
                    lblSearchString.Text = lblSearchString.Text + " " + "TERMINAL BRAND";
                    break;
                case SearchType.iTimeSheetTerminal:
                    lblSearchString.Text = lblSearchString.Text + " " + "TERMINAL NAME";
                    break;
                case SearchType.iCoountry:
                    lblSearchString.Text = lblSearchString.Text + " " + "COUNTRY";
                    break;
                case SearchType.iDispatch:
                case SearchType.iService:
                    lblSearchString.Text = lblSearchString.Text + " " + "MERCHANT / TID / MID / REQUEST ID. / REFERENCE NO. / REQUEST NO. / SERVICE NO / FIELD ENGINEER / JOB TYPE / TERMINAL SN / SIM SN / STATUS";
                    break;
                case SearchType.iFSR:
                    lblSearchString.Text = lblSearchString.Text + " " + "MERCHANT / TID / MID / REQUEST ID. / REFERENCE NO. / REQUEST NO. / SERVICE NO / FSR REFERENCE NO. / FIELD ENGINEER / JOB TYPE / TERMINAL SN / SIM SN / STATUS";
                    break;
                case SearchType.iInvoice:
                    lblSearchString.Text = lblSearchString.Text + " " + "INVOICE NO. / MERCHANT / ACCOUNT NO. / CUSTOMER NO. / CONTACT PERSON / DUUE DATE / INVOICE DATE";
                    break;
                case SearchType.iMobile:
                    lblSearchString.Text = lblSearchString.Text + " " + "MOBILE ID / MOBILE NAME / FIELD ENGINEER";
                    break;
                case SearchType.iMSPMasterList:
                    lblSearchString.Text = lblSearchString.Text + " " + "MSP NO. / REFERENCE NO / REGISTERED NAME";
                    break;
                case SearchType.iHelpDesk:
                    lblSearchString.Text = lblSearchString.Text + " " + "MERCHANT NAME / TID / MID / REQUEST ID / REFERENCE NO / REQUEST ID";
                    break;
                case SearchType.iHelpDeskProblem:
                    lblSearchString.Text = lblSearchString.Text + " " + "MERCHANT NAME / TID / MID / REQUEST ID / REFERENCE NO / ASSIST NO / PROBLEM NO / PROBLEM REPORTED / HELPDESK / TEAM LEAD";
                    break;

                default:
                    lblSearchString.Text = lblSearchString.Text + " " + "DETAIL";
                    break;
            }
        }

        private void InitPage(int iCurrentPage, int iTotalPage)
        {
            iTotalPage = clsSystemSetting.ClassSystemPageLimit;

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

        //private int GetTotalPage(SearchType iType)
        //{
        //    Debug.WriteLine("--GetTotalPage--");
        //    Debug.WriteLine("iType="+ iType);

        //    int iCount = 0;
        //    int totalPage = 0;
        //    int iLimitSize = dbFunction.GetPageLimit();

        //    Debug.WriteLine("iLimitSize=" + iLimitSize);

        //    iCount = 0;
        //    switch (iType)
        //    {
        //        case SearchType.iTerminal:
        //            clsSearch.ClassHoldAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
        //                                                iStatus.ToString() + clsFunction.sPipe +
        //                                                sTerminalType;

        //            dbAPI.GetViewCount("Search", "TerminalSN List", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");
        //            break;

        //        case SearchType.iSIM:
        //            clsSearch.ClassHoldAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
        //                                                iStatus.ToString();

        //            dbAPI.GetViewCount("Search", "SIMSN List", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");
        //            break;
        //        case SearchType.iClient:
        //            clsSearch.ClassHoldAdvanceSearchValue = clsGlobalVariables.iClient_Type.ToString() + clsFunction.sPipe +
        //                                                dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
        //                                                clsFunction.sZero;

        //            dbAPI.GetViewCount("Search", "Particular List", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");
        //            break;
        //        case SearchType.iMerchant:
        //            clsSearch.ClassHoldAdvanceSearchValue = clsGlobalVariables.iMerchant_Type.ToString() + clsFunction.sPipe +
        //                                                dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
        //                                                clsFunction.sZero;

        //            dbAPI.GetViewCount("Search", "Particular List", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");
        //            break;
        //        case SearchType.iFE:
        //            clsSearch.ClassHoldAdvanceSearchValue = clsGlobalVariables.iFE_Type.ToString() + clsFunction.sPipe +
        //                                                dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
        //                                                clsFunction.sZero;

        //            dbAPI.GetViewCount("Search", "Particular List", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");
        //            break;
        //        case SearchType.iSP:
        //            clsSearch.ClassHoldAdvanceSearchValue = clsGlobalVariables.iSP_Type.ToString() + clsFunction.sPipe +
        //                                                dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
        //                                                clsFunction.sZero;

        //            dbAPI.GetViewCount("Search", "Particular List", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");
        //            break;
        //        case SearchType.iRegion:
        //            clsSearch.ClassHoldAdvanceSearchValue = clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe +
        //                                                clsFunction.sZero + clsFunction.sPipe +
        //                                                dbFunction.CheckAndSetNumericValue(txtSearch.Text);

        //            dbAPI.GetViewCount("Search", "Region List", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");
        //            break;
        //        case SearchType.iProvince:
        //            clsSearch.ClassHoldAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe +
        //                                                clsFunction.sZero + clsFunction.sPipe +
        //                                                dbFunction.CheckAndSetNumericValue(txtSearch.Text);

        //            dbAPI.GetViewCount("Search", "Province List", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");
        //            break;
        //        case SearchType.iTerminalType:
        //            clsSearch.ClassHoldAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
        //                                                iStatus.ToString() + clsFunction.sPipe +
        //                                                sTerminalType;

        //            dbAPI.GetViewCount("Search", "Terminal Type", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");
        //            break;
        //        case SearchType.iTerminalModel:
        //            clsSearch.ClassHoldAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
        //                                                iStatus.ToString() + clsFunction.sPipe +
        //                                                sTerminalType;

        //            dbAPI.GetViewCount("Search", "Terminal Model", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");
        //            break;
        //        case SearchType.iTerminalBrand:
        //            clsSearch.ClassHoldAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
        //                                                iStatus.ToString() + clsFunction.sPipe +
        //                                                sTerminalType;

        //            dbAPI.GetViewCount("Search", "Terminal Brand", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");
        //            break;
        //        case SearchType.iIR:
        //            clsSearch.ClassHoldAdvanceSearchValue = clsGlobalVariables.STATUS_ALLOCATED_DESC + clsFunction.sPipe +
        //                                                    clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;

        //            dbAPI.GetViewCount("Search", "IR Allocated List", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");
        //            break;
        //    }
            
        //    if (dbAPI.isNoRecordFound() == false)
        //        iCount = clsTerminal.ClassTerminalCount;

        //    if (iCount > 0)
        //    {
        //        totalPage = (int)Math.Ceiling((double)iCount / iLimitSize);
        //    }
        //    else
        //    {
        //        totalPage = 0;
        //    }

        //    return totalPage;
        //}

        private void ProcessPage(SearchType iType)
        {
            //InitPage(int.Parse(clsFunction.sOne), GetTotalPage(iType));
        }

        private void btnFirstPage_Click(object sender, EventArgs e)
        {
            //clsSearch.ClassCurrentPage = int.Parse(clsFunction.sOne);
            //InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            //LoadListView(iSearchType);
        }

        private void btnLastPage_Click(object sender, EventArgs e)
        {
            //clsSearch.ClassCurrentPage = clsSearch.ClassTotalPage;
            //InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            //LoadListView(iSearchType);
        }

        private void btnPreviousPage_Click(object sender, EventArgs e)
        {
            //if (clsSearch.ClassCurrentPage > int.Parse(clsFunction.sOne))
            //    clsSearch.ClassCurrentPage--;

            //InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            //LoadListView(iSearchType);
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            //if (clsSearch.ClassCurrentPage < clsSearch.ClassTotalPage)
            //    clsSearch.ClassCurrentPage++;

            //InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            //LoadListView(iSearchType);
        }

        private void chkShowAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowAll.Checked)
                lblSearchMessage.Text = "*ALL STATUS WILL BE LISTED";
            else
                lblSearchMessage.Text = "*ONLY AVAILABLE STATUS WILL BE LISTED";
            
            LoadListView(iSearchType);
        }

        private void SetDefaultWindowSize(int pWidth, int pHeight)
        {
            int default_width = 1390;
            Debug.WriteLine("--SetDefaultWindowSize--");
            Debug.WriteLine("pWidth="+ pWidth+ ",pHeight="+ pHeight);
            
            this.StartPosition = FormStartPosition.CenterScreen;
            
            if (pWidth > default_width)
                pWidth = default_width;

            Size formSize = new Size(pWidth, pHeight);
            this.Size = new Size(formSize.Width, formSize.Height);

            Debug.WriteLine("formSize.Width=" + formSize.Width + ",formSize.Height=" + formSize.Height);
        }

        private void chkSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSelect.Text.Equals("CHECK ALL?"))
            {
                chkSelect.Text = "UNCHECK ALL?";

                // Check All
                foreach (ListViewItem i in lvwSearch.Items)
                {
                    i.Checked = true;
                }
            }
            else
            {
                chkSelect.Text = "CHECK ALL?";

                // UnCheck All
                foreach (ListViewItem i in lvwSearch.Items)
                {
                    i.Checked = false;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bool isValid = false;
            List<string> IDCol = new List<String>();
            List<string> DescriptionCol = new List<String>();
            
            if (isCheckBoxes)
            {
                foreach (ListViewItem i in lvwSearch.Items)
                {
                    if (i.Checked)
                    {
                        IDCol.Add(i.SubItems[1].Text); // ID
                        DescriptionCol.Add(i.SubItems[2].Text); // Description

                        isValid = true;
                    }
                }

                if (!isValid)
                {
                    dbFunction.SetMessageBox("No selected item(s)", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    return;
                }
                else
                {
                    if (MessageBox.Show("Are you sure to add selected item(s)?" +
                                       "\n\n",
                                       "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                    {
                        return;
                    }

                    fSelected = true;
                    clsArray.ID = IDCol.ToArray();
                    clsArray.Description = DescriptionCol.ToArray();

                    this.Close();
                }
            }
            else
            {
                IDCol.Add(lvwSearch.SelectedItems[0].SubItems[0].Text);
                DescriptionCol.Add(lvwSearch.SelectedItems[0].SubItems[1].Text);

                fSelected = true;
                clsArray.ID = IDCol.ToArray();
                clsArray.Description = DescriptionCol.ToArray();

                this.Close();
            }            
        }

        private void LoadHelpdeskMaster(String Value)
        {
            dbAPI = new clsAPI();
            dbAPI.ExecuteAPI("GET", "View", "Helpdesk-Master", Value, "Advance Detail", "", "ViewAdvanceDetail");

            if (!dbAPI.isNoRecordFound())
            {
                var data = ParseResponseData(clsArray.ID, clsArray.detail_info);

                //string[] columnSequence = { "IRIDNO", "ASSIST NO", "MERCHANT ID", "CLIENT ID", "MERCHANT NAME", "TID", "MID", "REFERENCE NO", "REQUEST ID", "REQUEST DATE", "STATUS" };
                string[] columnSequence = { clsDefines.TAG_HD_IRIDNo,
                                            clsDefines.TAG_HD_AssistNo,
                                            clsDefines.TAG_HD_MerchantID,
                                            clsDefines.TAG_HD_ClientID,
                                            clsDefines.TAG_HD_MerchantName,
                                            clsDefines.TAG_HD_TID,
                                            clsDefines.TAG_HD_MID,
                                            clsDefines.TAG_HD_ReferenceNo,
                                            clsDefines.TAG_HD_RequestDate,
                                            clsDefines.TAG_HD_CreatedDate,
                                            clsDefines.TAG_HD_RequestID,
                                            clsDefines.TAG_HD_TicketStatus,
                                            clsDefines.TAG_HD_HelpdeskName,
                                            clsDefines.TAG_HD_TeamLeadName,
                                            clsDefines.TAG_ServiceJobTypeDescription
                };

                string[] hiddenColumns = { clsDefines.TAG_HD_IRIDNo, clsDefines.TAG_HD_MerchantID, clsDefines.TAG_HD_AssistNo, clsDefines.TAG_HD_ClientID };
                
                SetListViewData(lvwSearch, data, columnSequence, hiddenColumns);

                this.Size = new Size(1400, 500); // adjust as necessary depends on columnSequence
                this.StartPosition = FormStartPosition.CenterScreen;
                lvwSearch.Dock = DockStyle.Fill;

                dbFunction.ListViewAlternateBackColor(lvwSearch);

                // DEFINE HEADER NAME
                var headerRenameMap = new Dictionary<string, string>
                {
                    { clsDefines.TAG_HD_MerchantName, "MERCHANT NAME" },
                    { clsDefines.TAG_HD_TID, "TID" },
                    { clsDefines.TAG_HD_MID, "MID" },
                    { clsDefines.TAG_HD_ReferenceNo, "REFERENCE NO" },
                    { clsDefines.TAG_HD_RequestDate, "REQUEST DATE" },
                    { clsDefines.TAG_HD_CreatedDate, "CREATED DATE" },
                    { clsDefines.TAG_HD_RequestID, "REQUEST ID" },
                    { clsDefines.TAG_HD_TicketStatus, "TICKET STATUS" },
                    { clsDefines.TAG_HD_HelpdeskName, "HELPDESK" },
                    { clsDefines.TAG_HD_TeamLeadName, "TEAM LEAD" },
                    { clsDefines.TAG_ServiceJobTypeDescription, "JOB TYPE" }
                };

                SetListViewColumnNames(lvwSearch, headerRenameMap);

                /*
                // Define the status-to-color mapping
                var colorMap = new Dictionary<string, Color>
                {
                    { clsDefines.TAG_HD_Resolved, Color.Green },
                    { clsDefines.TAG_HD_Pending, Color.Red }
                };

                // Apply it to your ListView
                SetListViewTextColor(lvwSearch, clsDefines.TAG_HD_Status, colorMap);
                */
            }
        }
    }
}
