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
    public partial class frmFindCount : Form
    {
        public static bool fMultiSelect = false;
        public bool fSelected = false;
        public static string sHeader = "";
        public static CountType iCountType;
        public static PopupMenuType iPopupMenuType;
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        private int iHoldIRStatus = 0;
        public bool fModify = false;

        string sStatementType = clsFunction.sNull;
        string sSearchBy = clsFunction.sNull;
        string sSearchValue = clsFunction.sNull;
        string sReportDescription = clsFunction.sNull;

        public static int iJobType = clsFunction.iZero;
        public static string sJobTypeDescription = clsFunction.sZero;
        public static int iIRStatus = clsFunction.iZero;
        public static string sIRStatusDescription = clsFunction.sZero;
        public static int isPrimary = clsFunction.iZero;

        // The column we are currently using for sorting.
        private ColumnHeader SortingColumn = null;

        List<string> TAIDNoCol = new List<String>();
        List<string> IRIDNoCol = new List<String>();
        List<string> IRNoCol = new List<String>();
        List<string> MerchantIDCol = new List<String>();
        List<string> MerchantNameCol = new List<String>();
        List<string> ServiceProviderIDCol = new List<String>();
        List<string> ServiceProviderNameCol = new List<String>();
        List<string> FEIDCol = new List<String>();
        List<string> FENameCol = new List<String>();
        List<string> ClientIDCol = new List<String>();
        List<string> ClientNameCol = new List<String>();
        List<string> TerminalIDCol = new List<String>();
        List<string> TerminalSNCol = new List<String>();
        List<string> SIMIDCol = new List<String>();
        List<string> SIMSerialNoCol = new List<String>();
        List<string> DockIDCol = new List<String>();
        List<string> DockSNCol = new List<String>();
        List<string> IRStatusCol = new List<String>();
        List<string> IRStatusDescriptionCol = new List<String>();
        List<string> ServiceStatusCol = new List<String>();
        List<string> ServiceStatusDescriptionCol = new List<String>();
        List<string> ServiceNoCol = new List<String>();
        List<string> RequestNoCol = new List<String>();
        List<string> JobTypeDescriptionCol = new List<String>();
        List<string> ServiceDateTimeCol = new List<String>();
        List<string> ReasonIDCol = new List<String>();
        List<string> ReasonDescriptionCol = new List<String>();

        public frmFindCount()
        {
            InitializeComponent();
        }
        public enum CountType
        {
            iTInstallationReq, iTInstallationReqDaysPending, iTInstallationReqOverDue,
            iTInstallation, iTServicing, iTPullOut, iTReplacement, iTReprogramming, iTDispatch,
            iSVCInstallation, iSVCReqServicing, iSVCPullOut, iSVCReplacement, iSVCReprogramming,
            iTotalInstalled, iTotalPullOut, iTotalReplacement, iTotalReprogramming, iTotalServicing, iTotalCancelled,
            iSearchIRInstalled,
            iSearchNegativeInstallation, iSearchNegativePullOut, iSearchNegativeReplacement, iSearchNegativeReprogramming, iSearchNegativeServicing,
            iSearchFSR,
            iSearchServicing,
            iSearchTA
        }

        public enum PopupMenuType
        {
            iNone, iDispatch, iDelete, iModify, iPreview, iFSR, iExit
        }

        
        private void frmFindCount_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("--frmFindCount_Load--");

            Cursor.Current = Cursors.WaitCursor;

            fSelected = false;
            fModify = false;
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            lblHeader.Text = "SEARCH" + " " + "[ " + sHeader + " ]";
            lblSearchStatus.Text = "";
            lblSubHeader.Text = "";

            btnPreviewReport.Enabled = false;
            InitPage(0, 0);
            
            Debug.WriteLine("lblHeader="+ lblHeader.Text);
            Debug.WriteLine("iCountType="+ iCountType);

            ProcessPage(iCountType);
            InitListView(); // Load Data            

            dbFunction.ListViewRowFocus(lvwSearch, 0);
            InitMultiSelect();

            SetSearchHeader(iCountType);
            InitHeaderBackColor(iCountType);

            iHoldIRStatus = clsSearch.ClassIRStatus;
            iPopupMenuType = PopupMenuType.iNone;
            InitPopupMenu();
            txtSearch.Focus();

            Cursor.Current = Cursors.Default;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmFindCount_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }
        public void InitListView()
        {            
            switch (iCountType)
            {
                case CountType.iTInstallationReq:
                case CountType.iTInstallationReqDaysPending:
                case CountType.iTInstallationReqOverDue:
                    lvwSearch.View = View.Details;
                    lvwSearch.Columns.Add("LINE#", 50, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("TAIDNO", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("IRIDNO", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REQUEST ID", 140, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("MERCHANTID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("MERCHANT NAME", 230, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SPID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SP NAME", 140, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("FEID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("FE NAME", 140, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REQUEST DATE", 110, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("INST DATE", 90, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("TID", 90, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("MID", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("ADDRESS", 0, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("CLIENTID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("CLIENT NAME", 120, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("CREATED DATE/TIME", 180, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("CREATED BY", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("PROCESS TYPE", 100, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("IRSTATUS", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("IR STATUS", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SERVICESTATUS", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SERVICE STATUS", 130, HorizontalAlignment.Left);

                    // Load Data
                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
                    dbAPI.ResetAdvanceSearch();                    
                    clsSearch.ClassIRStatus = clsGlobalVariables.STATUS_AVAILABLE;                    
                    LoadIR("View", "", "");
                    Cursor.Current = Cursors.Default;  // Back to normal

                    break;
                case CountType.iTInstallation:
                case CountType.iTServicing:
                case CountType.iTPullOut:
                case CountType.iTReplacement:
                case CountType.iTReprogramming:
                case CountType.iTDispatch:

                // Negative
                case CountType.iSearchNegativeInstallation:
                case CountType.iSearchNegativePullOut:
                case CountType.iSearchNegativeReplacement:
                case CountType.iSearchNegativeReprogramming:
                case CountType.iSearchNegativeServicing:                               

                case CountType.iTotalInstalled:
                case CountType.iTotalPullOut:
                case CountType.iTotalServicing:
                case CountType.iTotalReplacement:
                case CountType.iTotalReprogramming:
                case CountType.iTotalCancelled:
                    lvwSearch.View = View.Details;                   
                    lvwSearch.Columns.Add("LINE#",50, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*TAIDNO", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*IRIDNO", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REQUEST ID", 140, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*MERCHANTID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("MERCHANT NAME", 230, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*SPID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SP NAME", 140, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*FEID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("FE NAME", 140, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REQUEST DATE", 110, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("INST DATE", 90, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("TID", 90, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("MID", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("ADDRESS", 0, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*CLIENTID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("CLIENT NAME", 120, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*TERMINALID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("TERMINAL SN", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*SIMID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SIM SN", 160, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*DOCKID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("DOCK SN", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*IRSTATUS", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("IR STATUS", dbFunction.ID_Width(), HorizontalAlignment.Left);                    
                    lvwSearch.Columns.Add("*SERVICESTATUS", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SERVICE STATUS", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*SERVICE NO.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REQUEST NO.", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("JOB TYPE DESC", 180, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SERVICE DATE/TIME", 180, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*REASONID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REASON", 150, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*FSRNO", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("FSR DATE/TIME", 160, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("PRIMARY NUM", 110, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SECONDARY NUM", 110, HorizontalAlignment.Left);

                    // Load Data
                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
                    dbAPI.ResetAdvanceSearch();

                    if (iCountType == CountType.iTDispatch)
                    {
                        clsSearch.ClassIRStatus = clsFunction.iZero;
                        clsSearch.ClassStatus = clsGlobalVariables.STATUS_DISPATCH;
                        clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_DISPATCH_DESC;
                        clsSearch.ClassJobType = clsFunction.iZero;
                        clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING_DESC;
                        clsSearch.ClassJobTypeDescription = clsFunction.sZero;
                        clsSearch.ClassActionMade = clsFunction.sZero;                        
                    }                  
                    else if (iCountType == CountType.iTPullOut)
                    {
                        clsSearch.ClassStatus = clsGlobalVariables.STATUS_PULLED_OUT;
                        clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_PULLED_OUT_DESC;
                        clsSearch.ClassJobType = clsFunction.iZero;
                        clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
                    }
                    else if (iCountType == CountType.iTReplacement)
                    {
                        clsSearch.ClassStatus = clsGlobalVariables.STATUS_REPLACEMENT;
                        clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_REPLACEMENT_DESC;
                        clsSearch.ClassJobType = clsFunction.iZero;
                        clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
                    }
                    else if (iCountType == CountType.iTServicing)
                    {
                        clsSearch.ClassStatus = clsGlobalVariables.STATUS_SERVICING;
                        clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_SERVICING_DESC;
                        clsSearch.ClassJobType = clsFunction.iZero;
                        clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
                    }
                    else if (iCountType == CountType.iTReprogramming)
                    {
                        clsSearch.ClassStatus = clsGlobalVariables.STATUS_REPROGRAMMED;
                        clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_REPROGRAMMED_DESC;
                        clsSearch.ClassJobType = clsFunction.iZero;
                        clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
                    }
                    else if (iCountType == CountType.iTotalInstalled)
                    {
                        clsSearch.ClassIRStatus = clsFunction.iZero;
                        clsSearch.ClassIRStatusDescription = clsFunction.sZero;
                        clsSearch.ClassStatus = clsFunction.iZero;
                        clsSearch.ClassStatusDescription = clsFunction.sZero;
                        clsSearch.ClassJobType = clsFunction.iZero;
                        clsSearch.ClassJobTypeStatusDescription = clsFunction.sZero;

                        clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC;
                        clsSearch.ClassActionMade = clsGlobalVariables.ACTION_MADE_SUCCESS;
                        clsSearch.ClassIsClose = clsGlobalVariables.SVC_REQ_CLOSE.ToString();
                    }
                    else if (iCountType == CountType.iTotalPullOut)
                    {
                        clsSearch.ClassIRStatus = clsFunction.iZero;
                        clsSearch.ClassIRStatusDescription = clsFunction.sZero;
                        clsSearch.ClassStatus = clsFunction.iZero;
                        clsSearch.ClassStatusDescription = clsFunction.sZero;
                        clsSearch.ClassJobType = clsFunction.iZero;
                        clsSearch.ClassJobTypeStatusDescription = clsFunction.sZero;

                        clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_PULLOUT_DESC;
                        clsSearch.ClassActionMade = clsGlobalVariables.ACTION_MADE_SUCCESS;
                        clsSearch.ClassIsClose = clsGlobalVariables.SVC_REQ_CLOSE.ToString();
                    }
                    else if (iCountType == CountType.iTotalReplacement)
                    {
                        clsSearch.ClassIRStatus = clsFunction.iZero;
                        clsSearch.ClassIRStatusDescription = clsFunction.sZero;
                        clsSearch.ClassStatus = clsFunction.iZero;
                        clsSearch.ClassStatusDescription = clsFunction.sZero;
                        clsSearch.ClassJobType = clsFunction.iZero;
                        clsSearch.ClassJobTypeStatusDescription = clsFunction.sZero;

                        clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC;
                        clsSearch.ClassActionMade = clsGlobalVariables.ACTION_MADE_SUCCESS;
                        clsSearch.ClassIsClose = clsGlobalVariables.SVC_REQ_CLOSE.ToString();


                    }
                    else if (iCountType == CountType.iTotalReprogramming)
                    {
                        clsSearch.ClassStatus = clsGlobalVariables.STATUS_REPROGRAMMED;
                        clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_REPROGRAMMED_DESC;
                        clsSearch.ClassJobType = clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED;
                        clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC;
                    }
                    else if (iCountType == CountType.iTotalServicing)
                    {
                        clsSearch.ClassStatus = clsGlobalVariables.STATUS_SERVICING;
                        clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_SERVICING_DESC;
                        clsSearch.ClassJobType = clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED;
                        clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC;
                    }
                    else if (iCountType == CountType.iTotalCancelled)
                    {
                        clsSearch.ClassIRStatus = clsGlobalVariables.STATUS_CANCELLED;
                        clsSearch.ClassStatus = clsFunction.iZero;
                        clsSearch.ClassStatusDescription = clsFunction.sZero;
                    }
                    // -------------------------------------------------------------------------------------------------------
                    // NEGATIVE
                    // -------------------------------------------------------------------------------------------------------
                    else if (iCountType == CountType.iSearchNegativeInstallation)
                    {
                        clsSearch.ClassIRStatus = clsFunction.iZero;
                        clsSearch.ClassStatus = clsFunction.iZero;
                        clsSearch.ClassStatusDescription = clsFunction.sZero;
                        clsSearch.ClassJobType = clsFunction.iZero;
                        clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
                        clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC;
                        clsSearch.ClassActionMade = clsGlobalVariables.ACTION_MADE_NEGATIVE;
                    }
                    else if (iCountType == CountType.iSearchNegativePullOut)
                    {
                        clsSearch.ClassIRStatus = clsFunction.iZero;
                        clsSearch.ClassStatus = clsFunction.iZero;
                        clsSearch.ClassStatusDescription = clsFunction.sZero;
                        clsSearch.ClassJobType = clsFunction.iZero;
                        clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
                        clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_PULLOUT_DESC;
                        clsSearch.ClassActionMade = clsGlobalVariables.ACTION_MADE_NEGATIVE;
                    }
                    else if (iCountType == CountType.iSearchNegativeReplacement)
                    {
                        clsSearch.ClassIRStatus = clsFunction.iZero;
                        clsSearch.ClassStatus = clsFunction.iZero;
                        clsSearch.ClassStatusDescription = clsFunction.sZero;
                        clsSearch.ClassJobType = clsFunction.iZero;
                        clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
                        clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC;
                        clsSearch.ClassActionMade = clsGlobalVariables.ACTION_MADE_NEGATIVE;
                    }
                    else if (iCountType == CountType.iSearchNegativeReprogramming)
                    {
                        clsSearch.ClassIRStatus = clsFunction.iZero;
                        clsSearch.ClassStatus = clsFunction.iZero;
                        clsSearch.ClassStatusDescription = clsFunction.sZero;
                        clsSearch.ClassJobType = clsFunction.iZero;
                        clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
                        clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC;
                        clsSearch.ClassActionMade = clsGlobalVariables.ACTION_MADE_NEGATIVE;
                    }
                    else if (iCountType == CountType.iSearchNegativeServicing)
                    {
                        clsSearch.ClassIRStatus = clsFunction.iZero;
                        clsSearch.ClassStatus = clsFunction.iZero;
                        clsSearch.ClassStatusDescription = clsFunction.sZero;
                        clsSearch.ClassJobType = clsFunction.iZero;
                        clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
                        clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_SERVICING_DESC;
                        clsSearch.ClassActionMade = clsGlobalVariables.ACTION_MADE_NEGATIVE;
                    }                                       
                    // -------------------------------------------------------------------------------------------------------
                    // NEGATIVE
                    // -------------------------------------------------------------------------------------------------------
                    else if (iCountType == CountType.iTInstallation)
                    {
                        clsSearch.ClassStatus = clsGlobalVariables.STATUS_ALLOCATED;
                        clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_ALLOCATED_DESC;
                    }
                    else
                        clsSearch.ClassIRStatus = clsGlobalVariables.STATUS_ALLOCATED;

                    LoadIR("View", "", "");
                    Cursor.Current = Cursors.Default;  // Back to normal

                    break;

                case CountType.iSVCReqServicing:
                case CountType.iSVCPullOut:
                case CountType.iSVCReplacement:
                case CountType.iSVCReprogramming:                    
                    lvwSearch.View = View.Details;
                    lvwSearch.Columns.Add("LINE#", 50, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*TAIDNO", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*IRIDNO", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REQUEST ID", 140, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*MERCHANTID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("MERCHANT NAME", 230, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*SPID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SP NAME", 140, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*FEID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("FE NAME", 140, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REQUEST DATE", 110, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("INST DATE", 90, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("TID", 90, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("MID", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("ADDRESS", 0, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*CLIENTID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("CLIENT NAME", 120, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*TERMINALID", 0, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("TERMINAL SN", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*SIMID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SIM SN", 160, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*DOCKID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("DOCK SN", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*IRSTATUS", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("IR STATUS", dbFunction.ID_Width(), HorizontalAlignment.Left);                    
                    lvwSearch.Columns.Add("*SERVICESTATUS", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SERVICE STATUS", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*SERVICE NO.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REQUEST NO.", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("JOB TYPE DESC", 180, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SERVICE DATE/TIME", 180, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*REASONID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REASON", 150, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*FSRNO", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("FSR DATE/TIME", 160, HorizontalAlignment.Left);

                    // Load Data
                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
                    dbAPI.ResetAdvanceSearch();
                    clsSearch.ClassIRStatus = clsGlobalVariables.STATUS_INSTALLED;
                    LoadIR("View", "", "");
                    Cursor.Current = Cursors.Default;  // Back to normal

                    break;
                case CountType.iSearchIRInstalled:                
                    lvwSearch.View = View.Details;
                    lvwSearch.Columns.Add("LINE#", 50, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*TAIDNO", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*IRIDNO", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REQUEST ID", 140, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*MERCHANTID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("MERCHANT NAME", 230, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*SPID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SP NAME", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*FEID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("FE NAME", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REQUEST DATE", 100, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("INST DATE", 90, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("TID", 90, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("MID", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("ADDRESS", 0, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*CLIENTID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("CLIENT NAME", 120, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*TERMINALID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("TERMINAL SN", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*SIMID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SIM SN", 160, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*DOCKID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("DOCK SN", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*IRSTATUS", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("IR STATUS", dbFunction.ID_Width(), HorizontalAlignment.Left);                    
                    lvwSearch.Columns.Add("*SERVICESTATUS", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SERVICE STATUS", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*SERVICE NO.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REQUEST NO.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("JOB TYPE DESC", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SERVICE DATE/TIME", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*REASONID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REASON", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*FSRNO", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("FSR DATE/TIME", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("PRIMARY NUM", 110, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SECONDARY NUM", 110, HorizontalAlignment.Left);

                    // Load Data
                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
                    dbAPI.ResetAdvanceSearch();

                    clsSearch.ClassStatus = clsGlobalVariables.STATUS_INSTALLED;
                    clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_INSTALLED_DESC;
                               
                    LoadIR("View", "", "");
                    Cursor.Current = Cursors.Default;  // Back to normal
                    break;

                case CountType.iSearchFSR:
                    lvwSearch.View = View.Details;
                    lvwSearch.Columns.Add("LINE#", 50, HorizontalAlignment.Left);

                    lvwSearch.Columns.Add("*FSRNO", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*SERVICE NO.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*IRIDNO.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*TAIDNo.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*ClientID.", dbFunction.ID_Width(), HorizontalAlignment.Left);                   
                    lvwSearch.Columns.Add("REQUEST ID", 140, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("DATE", 90, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("MERCHANT NAME", 230, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("TID", 90, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("MID", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("TERMINAL SN", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SIM SN", 160, HorizontalAlignment.Left);                    
                    lvwSearch.Columns.Add("REQUEST NO.", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("PRIMARY NUM", 110, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SECONDARY NUM", 110, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("JOB TYPE DESC", 180, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("FE NAME", 140, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REQUEST DATE", 110, HorizontalAlignment.Left);                    
                    lvwSearch.Columns.Add("SERVICE DATE", 110, HorizontalAlignment.Left);

                    // Load Data
                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
                    dbAPI.ResetAdvanceSearch();
                    
                    LoadIR("View", "", "");
                    Cursor.Current = Cursors.Default;  // Back to normal
                    break;

                case CountType.iSearchServicing:
                    lvwSearch.View = View.Details;
                    lvwSearch.Columns.Add("LINE#", 50, HorizontalAlignment.Left);
                    
                    lvwSearch.Columns.Add("*SERVICE NO.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*IRIDNO.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*TAIDNo.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*ClientID.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*FEID.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*MerchantID.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*RegionID.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*RegionType", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REQUEST ID", 140, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REQUEST DATE", 110, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REQUEST NO.", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SERVICE DATE", 110, HorizontalAlignment.Left);                    
                    lvwSearch.Columns.Add("MERCHANT NAME", 230, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("TID", 90, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("MID", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("TERMINAL SN(CUR)", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SIM SN(CUR)", 160, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("TERMINAL SN(REP)", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SIM SN(REP)", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("JOB TYPE DESC", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("JOB TYPE STATUS", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SERVICE STATUS*", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SERVICE STATUS DESC", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("PRIMARY NUM", 110, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SECONDARY NUM", 110, HorizontalAlignment.Left);

                    // Load Data
                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
                    dbAPI.ResetAdvanceSearch();

                    LoadIR("View", "", "");
                    Cursor.Current = Cursors.Default;  // Back to normal
                    break;

                case CountType.iSearchTA:
                    lvwSearch.View = View.Details;
                    lvwSearch.Columns.Add("LINE#", 50, HorizontalAlignment.Left);

                    lvwSearch.Columns.Add("*SERVICE NO.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*IRIDNO.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*TAIDNo.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*ClientID.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*FEID.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*ServiceProviderID.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*MerchantID.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*RegionID.", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*RegionType", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REQUEST ID", 140, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REQUEST DATE",110, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("REQUEST NO.", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SERVICE DATE", 110, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("MERCHANT NAME", 230, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("TID", 90, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("MID", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*TERMINALIID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("TERMINAL SN", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*SIMID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SIM SN", 160, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("*DOCKID", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("DOCK SN", 130, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("JOB TYPE DESC", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("JOB TYPE STATUS", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SERVICE STATUS*", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SERVICE STATUS DESC", dbFunction.ID_Width(), HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("PRIMARY NUM", 110, HorizontalAlignment.Left);
                    lvwSearch.Columns.Add("SECONDARY NUM", 110, HorizontalAlignment.Left);

                    // Load Data
                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
                    dbAPI.ResetAdvanceSearch();

                    LoadIR("View", "", "");
                    Cursor.Current = Cursors.Default;  // Back to normal
                    break;
            }
        }

        private void LoadIR(string StatementType, string SearchBy, string SearchValue)
        {
            Debug.WriteLine("--LoadIR--");
            Debug.WriteLine("iCountType=" + iCountType);

            int i = 0;
            int ii = 0;
            int iLineNo = 0;
            string sFSRTime = clsFunction.sNull;
            string sSearchString = txtSearch.Text;

            sSearchString = (txtSearch.TextLength > 0 ? txtSearch.Text.Trim() : clsFunction.sZero);

            Debug.WriteLine("LoadIR::iCountType=" + iCountType.ToString());

            lblSearchStatus.Text = "";
            lvwSearch.Items.Clear();
            btnPreviewReport.Enabled = false;

            Debug.WriteLine("Len="+ txtSearch.Text.Length+ ",txtSearch.Text="+ txtSearch.Text);
            Debug.WriteLine("Len=" + sSearchString.Length + ",sSearchString=" + sSearchString);

            if (sSearchString.Length > 0)
            {
                InitPage(0, 0);
            }
            else
            {
                ResetSearchField();

                switch (iCountType)
                {
                    case CountType.iTInstallationReq:
                    case CountType.iTInstallationReqDaysPending:
                    case CountType.iTInstallationReqOverDue:
                        clsSearch.ClassIRStatus = clsGlobalVariables.STATUS_AVAILABLE;
                        clsSearch.ClassIRStatusDescription = clsGlobalVariables.STATUS_AVAILABLE_DESC;
                        isPrimary = clsGlobalVariables.SERVICE_NON;
                        clsSearch.ClassJobTypeDescription = clsFunction.sNull;
                        break;
                }
            }

            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassServiceTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassParticularID + clsFunction.sPipe +
                                                clsSearch.ClassTerminalTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalModelID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalBrandID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalSN + clsFunction.sPipe +
                                                (iIRStatus > 0 ? clsSearch.ClassIRStatus : iIRStatus) + clsFunction.sPipe +
                                                clsSearch.ClassIRNo + clsFunction.sPipe +
                                                clsSearch.ClassTID + clsFunction.sPipe +
                                                clsSearch.ClassMID + clsFunction.sPipe +
                                                clsSearch.ClassTerminalStatusType + clsFunction.sPipe +
                                                clsSearch.ClassReqDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassReqDateTo + clsFunction.sPipe +
                                                clsSearch.ClassInstDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassInstDateTo + clsFunction.sPipe +
                                                clsSearch.ClassTADateFrom + clsFunction.sPipe +
                                                clsSearch.ClassTADateTo + clsFunction.sPipe +
                                                clsSearch.ClassFSRDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassFSRDateTo + clsFunction.sPipe +
                                                clsSearch.ClassIRImportDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassIRImportDateTo + clsFunction.sPipe +
                                                clsSearch.ClassSIMSerialNo + clsFunction.sPipe +
                                                dbFunction.CheckAndSetNumericValue(sSearchString) + clsFunction.sPipe +                                                
                                                (iIRStatus > 0 ? clsSearch.ClassStatus : iIRStatus) + clsFunction.sPipe +                                                
                                                (sIRStatusDescription.Equals(clsFunction.sZero) ? sIRStatusDescription : clsSearch.ClassStatusDescription) + clsFunction.sPipe +
                                                clsSearch.ClassJobType + clsFunction.sPipe +
                                                clsSearch.ClassJobTypeStatusDescription + clsFunction.sPipe +
                                                (sJobTypeDescription.Equals(clsFunction.sZero) ? sJobTypeDescription : clsSearch.ClassJobTypeDescription)    + clsFunction.sPipe +
                                                clsSearch.ClassActionMade + clsFunction.sPipe +
                                                clsSearch.ClassNoOfDayPending + clsFunction.sPipe +
                                                (iCountType == CountType.iTInstallationReqOverDue ? clsGlobalVariables.SVC_REQ_OVERDUE_TRUE.ToString() : clsGlobalVariables.SVC_REQ_OVERDUE_FALSE.ToString()) + clsFunction.sPipe +
                                                clsSearch.ClassIsClose + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsSearch.ClassClientID + clsFunction.sPipe +
                                                clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                dbFunction.GetPageLimit() + clsFunction.sPipe +
                                                clsSearch.ClassRegionID + clsFunction.sPipe +
                                                clsSearch.ClassProvinceID + clsFunction.sPipe +
                                                isPrimary.ToString();
            
            Debug.WriteLine("LoadIR::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            // For Preview Report
            sSearchValue = clsSearch.ClassServiceTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassParticularID + clsFunction.sPipe +
                                                clsSearch.ClassTerminalTypeID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalModelID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalBrandID.ToString() + clsFunction.sPipe +
                                                clsSearch.ClassTerminalSN + clsFunction.sPipe +                                                
                                                (iIRStatus > 0 ? clsSearch.ClassIRStatus : iIRStatus) + clsFunction.sPipe +
                                                clsSearch.ClassIRNo + clsFunction.sPipe +
                                                clsSearch.ClassTID + clsFunction.sPipe +
                                                clsSearch.ClassMID + clsFunction.sPipe +
                                                clsSearch.ClassTerminalStatusType + clsFunction.sPipe +
                                                clsSearch.ClassReqDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassReqDateTo + clsFunction.sPipe +
                                                clsSearch.ClassInstDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassInstDateTo + clsFunction.sPipe +
                                                clsSearch.ClassTADateFrom + clsFunction.sPipe +
                                                clsSearch.ClassTADateTo + clsFunction.sPipe +
                                                clsSearch.ClassFSRDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassFSRDateTo + clsFunction.sPipe +
                                                clsSearch.ClassIRImportDateFrom + clsFunction.sPipe +
                                                clsSearch.ClassIRImportDateTo + clsFunction.sPipe +
                                                clsSearch.ClassSIMSerialNo + clsFunction.sPipe +
                                                dbFunction.CheckAndSetNumericValue(sSearchString) + clsFunction.sPipe +
                                                (iIRStatus > 0 ? clsSearch.ClassStatus : iIRStatus) + clsFunction.sPipe +
                                                (sIRStatusDescription.Equals(clsFunction.sZero) ? sIRStatusDescription : clsSearch.ClassStatusDescription) + clsFunction.sPipe +
                                                clsSearch.ClassJobType + clsFunction.sPipe +
                                                clsSearch.ClassJobTypeStatusDescription + clsFunction.sPipe +
                                                (sJobTypeDescription.Equals(clsFunction.sZero) ? sJobTypeDescription : clsSearch.ClassJobTypeDescription) + clsFunction.sPipe +
                                                clsSearch.ClassActionMade + clsFunction.sPipe +
                                                clsSearch.ClassNoOfDayPending + clsFunction.sPipe +
                                                (iCountType == CountType.iTInstallationReqOverDue ? clsGlobalVariables.SVC_REQ_OVERDUE_TRUE.ToString() : clsGlobalVariables.SVC_REQ_OVERDUE_FALSE.ToString()) + clsFunction.sPipe +
                                                clsSearch.ClassIsClose + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                clsFunction.sZero + clsFunction.sPipe +
                                                isPrimary.ToString();
            
            Debug.WriteLine("LoadIR::sSearchValue=" + sSearchValue);

            dbFunction.GetRequestTime("Find IR");
            
            switch (iCountType)
            {
                case CountType.iSearchIRInstalled:
                case CountType.iTInstallationReq:
                case CountType.iTInstallationReqDaysPending:
                case CountType.iTInstallationReqOverDue:
                case CountType.iTotalInstalled:
                case CountType.iTotalPullOut:
                case CountType.iTotalReplacement:
                case CountType.iTotalReprogramming:
                case CountType.iTotalServicing:                
                case CountType.iTotalCancelled:

                    sSearchBy = "Advance IR";
                    dbAPI.ExecuteAPI("GET", "View", sSearchBy, clsSearch.ClassAdvanceSearchValue, "IR", "", "ViewAdvanceIR");
                    break;
                case CountType.iTDispatch:
                    GetSearchStringPerJobType();
                    
                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassStatusDescription + clsFunction.sPipe +
                                                        clsSearch.ClassJobTypeDescription + clsFunction.sPipe +
                                                        clsSearch.ClassJobTypeStatusDescription + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(sSearchString) + clsFunction.sPipe +
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();

                    // For View
                    sSearchValue = clsSearch.ClassStatusDescription + clsFunction.sPipe +
                                                        clsSearch.ClassJobTypeDescription + clsFunction.sPipe +
                                                        clsSearch.ClassJobTypeStatusDescription + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(sSearchString) + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsFunction.sZero;

                    Debug.WriteLine("LoadIR::sSearchValue=" + sSearchValue);

                    sSearchBy = "Dispatch Servicing";
                    dbAPI.ExecuteAPI("GET", "View", sSearchBy, clsSearch.ClassAdvanceSearchValue, "IR", "", "ViewAdvanceIR");
                    break;
                case CountType.iTInstallation:
                case CountType.iTServicing:
                case CountType.iTPullOut:
                case CountType.iTReplacement:
                case CountType.iTReprogramming:
                    GetSearchStringPerJobType();

                    clsSearch.ClassAdvanceSearchValue = clsSearch.ClassStatusDescription + clsFunction.sPipe +
                                                        clsSearch.ClassJobTypeDescription + clsFunction.sPipe +
                                                        clsSearch.ClassJobTypeStatusDescription + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(sSearchString) + clsFunction.sPipe +
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();

                    // For View
                    sSearchValue = clsSearch.ClassStatusDescription + clsFunction.sPipe +
                                                        clsSearch.ClassJobTypeDescription + clsFunction.sPipe +
                                                        clsSearch.ClassJobTypeStatusDescription + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(sSearchString) + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        clsFunction.sZero;

                    Debug.WriteLine("LoadIR::sSearchValue=" + sSearchValue);

                    sSearchBy = "Allocated Servicing";
                    dbAPI.ExecuteAPI("GET", "View", sSearchBy, clsSearch.ClassAdvanceSearchValue, "IR", "", "ViewAdvanceIR");
                    break;
                case CountType.iSearchNegativeInstallation:
                case CountType.iSearchNegativePullOut:
                case CountType.iSearchNegativeReplacement:
                case CountType.iSearchNegativeReprogramming:
                case CountType.iSearchNegativeServicing:
                        GetSearchStringPerJobType();

                        clsSearch.ClassAdvanceSearchValue = clsSearch.ClassJobTypeDescription + clsFunction.sPipe + 
                                                            clsSearch.ClassJobTypeStatusDescription + clsFunction.sPipe + 
                                                            clsGlobalVariables.ACTION_MADE_NEGATIVE + clsFunction.sPipe +
                                                            dbFunction.CheckAndSetNumericValue(sSearchString);

                        sSearchValue = clsSearch.ClassJobTypeDescription + clsFunction.sPipe +
                                                            clsSearch.ClassJobTypeStatusDescription + clsFunction.sPipe +
                                                            clsGlobalVariables.ACTION_MADE_NEGATIVE + clsFunction.sPipe +
                                                            dbFunction.CheckAndSetNumericValue(sSearchString);

                        Debug.WriteLine("LoadIR::sSearchValue=" + sSearchValue);

                    sSearchBy = "Negative Servicing";
                    dbAPI.ExecuteAPI("GET", "View", sSearchBy, clsSearch.ClassAdvanceSearchValue, "IR", "", "ViewAdvanceIR");
                    break;
                case CountType.iSearchFSR:
                    clsSearch.ClassSearchBy = "FSR";
                    clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(sSearchString) + clsFunction.sPipe+
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();

                    dbAPI.ExecuteAPI("GET", "View", clsSearch.ClassSearchBy, clsSearch.ClassAdvanceSearchValue, "FSR", "", "ViewFSR");
                    break;
                case CountType.iSearchServicing:
                    clsSearch.ClassSearchBy = "Servicing List";                    
                    clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(sSearchString) + clsFunction.sPipe +
                                                        clsFunction.sNull + clsFunction.sPipe +
                                                        isPrimary.ToString() + clsFunction.sPipe +
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();

                    Debug.WriteLine("LoadIR::ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                    dbAPI.ExecuteAPI("GET", "View", clsSearch.ClassSearchBy, clsSearch.ClassAdvanceSearchValue, "Servicing Detail", "", "ViewServicingDetail");
                    break;
                case CountType.iSearchTA:
                    clsSearch.ClassSearchBy = "TA List";                    
                    clsSearch.ClassAdvanceSearchValue = dbFunction.CheckAndSetNumericValue(sSearchString) + clsFunction.sPipe +
                                                        isPrimary.ToString() + clsFunction.sPipe +
                                                        clsSearch.ClassCurrentPage + clsFunction.sPipe +
                                                        dbFunction.GetPageLimit();

                    Debug.WriteLine("LoadIR::ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                    dbAPI.ExecuteAPI("GET", "View", clsSearch.ClassSearchBy, clsSearch.ClassAdvanceSearchValue, "TA", "", "ViewAdvanceTA");
                    break;
                default:

                    sSearchBy = "Advance Servicing";
                    dbAPI.ExecuteAPI("GET", "View", sSearchBy, clsSearch.ClassAdvanceSearchValue, "IR", "", "ViewAdvanceIR");
                    break;
            }
            
            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                while (clsArray.IRIDNo.Length > i)
                {
                    sFSRTime = clsFunction.sNull;
                    ii++;
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                    switch (iCountType)
                    {
                        case CountType.iSearchTA:
                            item.SubItems.Add(clsArray.ServiceNo[i].ToString());
                            item.SubItems.Add(clsArray.IRIDNo[i].ToString());
                            item.SubItems.Add(clsArray.TAIDNo[i].ToString());
                            item.SubItems.Add(clsArray.ClientID[i].ToString());
                            item.SubItems.Add(clsArray.FEID[i].ToString());
                            item.SubItems.Add(clsArray.ServiceProviderID[i].ToString());
                            item.SubItems.Add(clsArray.MerchantID[i].ToString());
                            item.SubItems.Add(clsArray.RegionID[i].ToString());
                            item.SubItems.Add(clsArray.RegionType[i].ToString());

                            item.SubItems.Add(clsArray.IRNo[i].ToString());
                            item.SubItems.Add(clsArray.ServiceReqDate[i].ToString());
                            item.SubItems.Add(clsArray.RequestNo[i].ToString());
                            item.SubItems.Add(clsArray.ServiceDate[i].ToString());
                            item.SubItems.Add(clsArray.MerchantName[i].ToString());
                            item.SubItems.Add(clsArray.TID[i].ToString());
                            item.SubItems.Add(clsArray.MID[i].ToString());
                            item.SubItems.Add(clsArray.TerminalID[i].ToString());
                            item.SubItems.Add(clsArray.TerminalSN[i].ToString());
                            item.SubItems.Add(clsArray.SIMID[i].ToString());
                            item.SubItems.Add(clsArray.SIMSerialNo[i].ToString());
                            item.SubItems.Add(clsArray.DockID[i].ToString());
                            item.SubItems.Add(clsArray.DockSN[i].ToString());
                            item.SubItems.Add(clsArray.JobTypeDescription[i].ToString());
                            item.SubItems.Add(clsArray.JobTypeStatusDescription[i].ToString());
                            item.SubItems.Add(clsArray.ServiceStatus[i].ToString());
                            item.SubItems.Add(clsArray.ServiceStatusDescription[i].ToString());
                            item.SubItems.Add(clsArray.PrimaryNum[i].ToString());
                            item.SubItems.Add(clsArray.SecondaryNum[i].ToString());

                            lvwSearch.Items.Add(item);
                            break;
                        case CountType.iSearchServicing:
                            item.SubItems.Add(clsArray.ServiceNo[i].ToString());                           
                            item.SubItems.Add(clsArray.IRIDNo[i].ToString());
                            item.SubItems.Add(clsArray.TAIDNo[i].ToString());
                            item.SubItems.Add(clsArray.ClientID[i].ToString());

                            item.SubItems.Add(clsArray.FEID[i].ToString());
                            item.SubItems.Add(clsArray.MerchantID[i].ToString());
                            item.SubItems.Add(clsArray.RegionID[i].ToString());
                            item.SubItems.Add(clsArray.RegionType[i].ToString());

                            item.SubItems.Add(clsArray.IRNo[i].ToString());
                            item.SubItems.Add(clsArray.ServiceReqDate[i].ToString());
                            item.SubItems.Add(clsArray.RequestNo[i].ToString());
                            item.SubItems.Add(clsArray.ServiceDate[i].ToString());
                            item.SubItems.Add(clsArray.MerchantName[i].ToString());
                            item.SubItems.Add(clsArray.TID[i].ToString());
                            item.SubItems.Add(clsArray.MID[i].ToString());
                            item.SubItems.Add(clsArray.TerminalSN[i].ToString());
                            item.SubItems.Add(clsArray.SIMSerialNo[i].ToString());
                            item.SubItems.Add(clsArray.ReplaceTerminalSN[i].ToString());
                            item.SubItems.Add(clsArray.ReplaceSIMSN[i].ToString());
                            item.SubItems.Add(clsArray.JobTypeDescription[i].ToString());
                            item.SubItems.Add(clsArray.JobTypeStatusDescription[i].ToString());
                            item.SubItems.Add(clsArray.ServiceStatus[i].ToString());
                            item.SubItems.Add(clsArray.ServiceStatusDescription[i].ToString());
                            item.SubItems.Add(clsArray.PrimaryNum[i].ToString());
                            item.SubItems.Add(clsArray.SecondaryNum[i].ToString());

                            lvwSearch.Items.Add(item);

                            break;
                        case CountType.iSearchFSR:
                            item.SubItems.Add(clsArray.FSRNo[i].ToString());
                            item.SubItems.Add(clsArray.ServiceNo[i].ToString());
                            item.SubItems.Add(clsArray.IRIDNo[i].ToString());
                            item.SubItems.Add(clsArray.TAIDNo[i].ToString());
                            item.SubItems.Add(clsArray.ClientID[i].ToString());
                            item.SubItems.Add(clsArray.IRNo[i].ToString());
                            item.SubItems.Add(clsArray.FSRDate[i].ToString());
                            item.SubItems.Add(clsArray.Merchant[i].ToString());
                            item.SubItems.Add(clsArray.TID[i].ToString());
                            item.SubItems.Add(clsArray.MID[i].ToString());
                            item.SubItems.Add(clsArray.TerminalSN[i].ToString());
                            item.SubItems.Add(clsArray.SIMSerialNo[i].ToString());
                            item.SubItems.Add(clsArray.RequestNo[i].ToString());
                            item.SubItems.Add(clsArray.PrimaryNum[i].ToString());
                            item.SubItems.Add(clsArray.SecondaryNum[i].ToString());
                            item.SubItems.Add(clsArray.JobTypeDescription[i].ToString());
                            item.SubItems.Add(clsArray.FEName[i].ToString());
                            item.SubItems.Add(clsArray.ServiceReqDate[i].ToString());
                            item.SubItems.Add(clsArray.ServiceDate[i].ToString());
                            lvwSearch.Items.Add(item);

                            break;
                        default:
                            item.SubItems.Add(clsArray.TAIDNo[i].ToString());
                            item.SubItems.Add(clsArray.IRIDNo[i].ToString());
                            item.SubItems.Add(clsArray.IRNo[i].ToString());
                            item.SubItems.Add(clsArray.MerchantID[i].ToString());
                            item.SubItems.Add(clsArray.ParticularName[i].ToString());
                            item.SubItems.Add(clsArray.ServiceProviderID[i].ToString());
                            item.SubItems.Add(clsArray.ServiceProviderName[i].ToString());
                            item.SubItems.Add(clsArray.FEID[i].ToString());
                            item.SubItems.Add(clsArray.FEName[i].ToString());
                            item.SubItems.Add(clsArray.IRDate[i].ToString());
                            item.SubItems.Add(clsArray.InstallationDate[i].ToString());
                            item.SubItems.Add(clsArray.TID[i].ToString());
                            item.SubItems.Add(clsArray.MID[i].ToString());
                            item.SubItems.Add(clsArray.Address[i].ToString());
                            item.SubItems.Add(clsArray.ClientID[i].ToString());
                            item.SubItems.Add(clsArray.ClientName[i].ToString());

                            switch (iCountType)
                            {
                                case CountType.iTInstallationReq:
                                case CountType.iTInstallationReqDaysPending:
                                case CountType.iTInstallationReqOverDue:
                                    item.SubItems.Add(clsArray.ProcessedDateTime[i].ToString());
                                    item.SubItems.Add(clsArray.ProcessedBy[i].ToString());
                                    item.SubItems.Add(clsArray.ProcessType[i].ToString());
                                    item.SubItems.Add(clsArray.IRStatus[i].ToString());
                                    item.SubItems.Add(clsArray.IRStatusDescription[i].ToString());
                                    item.SubItems.Add(clsArray.ServiceStatus[i].ToString());
                                    item.SubItems.Add(clsArray.ServiceStatusDescription[i].ToString());
                                    item.SubItems.Add(clsFunction.sDash);
                                    item.SubItems.Add(clsFunction.sDash);
                                    item.SubItems.Add(clsFunction.sDash);
                                    item.SubItems.Add(clsFunction.sDash);
                                    break;
                                case CountType.iSearchIRInstalled:
                                case CountType.iTInstallation:
                                case CountType.iTServicing:
                                case CountType.iTPullOut:
                                case CountType.iTReplacement:
                                case CountType.iTReprogramming:
                                case CountType.iTDispatch:
                                case CountType.iTotalInstalled:
                                case CountType.iTotalPullOut:
                                case CountType.iTotalReprogramming:
                                case CountType.iTotalReplacement:
                                case CountType.iTotalServicing:
                                case CountType.iTotalCancelled:
                                    item.SubItems.Add(clsArray.TerminalID[i].ToString());
                                    item.SubItems.Add(clsArray.TerminalSN[i].ToString());
                                    item.SubItems.Add(clsArray.SIMID[i].ToString());
                                    item.SubItems.Add(clsArray.SIMSerialNo[i].ToString());
                                    item.SubItems.Add(clsArray.DockID[i].ToString());
                                    item.SubItems.Add(clsArray.DockSN[i].ToString());
                                    item.SubItems.Add(clsArray.IRStatus[i].ToString());
                                    item.SubItems.Add(clsArray.IRStatusDescription[i].ToString());
                                    item.SubItems.Add(clsArray.ServiceStatus[i].ToString());
                                    item.SubItems.Add(clsArray.ServiceStatusDescription[i].ToString());
                                    item.SubItems.Add(clsArray.ServiceNo[i].ToString());
                                    item.SubItems.Add(clsArray.RequestNo[i].ToString());
                                    item.SubItems.Add(clsArray.JobTypeDescription[i].ToString());
                                    item.SubItems.Add(clsArray.ServiceDateTime[i].ToString());
                                    item.SubItems.Add(clsArray.ReasonID[i].ToString());
                                    item.SubItems.Add(clsArray.ReasonDescription[i].ToString());
                                    item.SubItems.Add(clsArray.FSRNo[i]);

                                    sFSRTime = dbFunction.GetDateFromParse(clsArray.FSRTime[i], "H:mm:ss", "hh:mm tt");
                                    item.SubItems.Add(clsArray.FSRDate[i] + clsFunction.sPadSpace + sFSRTime);

                                    item.SubItems.Add(clsArray.PrimaryNum[i]);
                                    item.SubItems.Add(clsArray.SecondaryNum[i]);
                                    break;
                                case CountType.iSVCReqServicing:
                                case CountType.iSVCPullOut:
                                case CountType.iSVCReplacement:
                                case CountType.iSVCReprogramming:
                                    item.SubItems.Add(clsArray.TerminalID[i].ToString());
                                    item.SubItems.Add(clsArray.TerminalSN[i].ToString());
                                    item.SubItems.Add(clsArray.SIMID[i].ToString());
                                    item.SubItems.Add(clsArray.SIMSerialNo[i].ToString());
                                    item.SubItems.Add(clsArray.DockID[i].ToString());
                                    item.SubItems.Add(clsArray.DockSN[i].ToString());
                                    item.SubItems.Add(clsArray.IRStatus[i].ToString());
                                    item.SubItems.Add(clsArray.IRStatusDescription[i].ToString());
                                    item.SubItems.Add(clsArray.ServiceStatus[i].ToString());
                                    item.SubItems.Add(clsArray.ServiceStatusDescription[i].ToString());
                                    item.SubItems.Add(clsArray.ServiceNo[i].ToString());
                                    item.SubItems.Add(clsArray.RequestNo[i].ToString());
                                    item.SubItems.Add(clsArray.JobTypeDescription[i].ToString());
                                    item.SubItems.Add(clsArray.ServiceDateTime[i].ToString());
                                    item.SubItems.Add(clsArray.ReasonID[i].ToString());
                                    item.SubItems.Add(clsArray.ReasonDescription[i].ToString());
                                    item.SubItems.Add(clsArray.FSRNo[i]);

                                    sFSRTime = dbFunction.GetDateFromParse(clsArray.FSRTime[i], "H:mm:ss", "hh:mm tt");
                                    item.SubItems.Add(clsArray.FSRDate[i] + clsFunction.sPadSpace + sFSRTime);
                                    break;
                                case CountType.iSearchNegativeInstallation:
                                case CountType.iSearchNegativePullOut:
                                case CountType.iSearchNegativeReplacement:
                                case CountType.iSearchNegativeReprogramming:
                                case CountType.iSearchNegativeServicing:
                                    item.SubItems.Add(clsArray.TerminalID[i].ToString());
                                    item.SubItems.Add(clsArray.TerminalSN[i].ToString());
                                    item.SubItems.Add(clsArray.SIMID[i].ToString());
                                    item.SubItems.Add(clsArray.SIMSerialNo[i].ToString());
                                    item.SubItems.Add(clsArray.DockID[i].ToString());
                                    item.SubItems.Add(clsArray.DockSN[i].ToString());
                                    item.SubItems.Add(clsArray.IRStatus[i].ToString());
                                    item.SubItems.Add(clsArray.IRStatusDescription[i].ToString());
                                    item.SubItems.Add(clsArray.ServiceStatus[i].ToString());
                                    item.SubItems.Add(clsArray.ServiceStatusDescription[i].ToString());
                                    item.SubItems.Add(clsArray.ServiceNo[i].ToString());
                                    item.SubItems.Add(clsArray.RequestNo[i].ToString());
                                    item.SubItems.Add(clsArray.JobTypeDescription[i].ToString());
                                    item.SubItems.Add(clsArray.ServiceDateTime[i].ToString());
                                    item.SubItems.Add(clsArray.ReasonID[i].ToString());
                                    item.SubItems.Add(clsArray.ReasonDescription[i].ToString());
                                    item.SubItems.Add(clsArray.FSRNo[i]);

                                    sFSRTime = dbFunction.GetDateFromParse(clsArray.FSRTime[i], "H:mm:ss", "hh:mm tt");
                                    item.SubItems.Add(clsArray.FSRDate[i] + clsFunction.sPadSpace + sFSRTime);
                                    break;

                            }

                            lvwSearch.Items.Add(item);

                            if (int.Parse(clsArray.IRStatus[i]) == clsGlobalVariables.STATUS_AVAILABLE)
                                clsIR.ClassIRStatusDescription = "-";

                            break;
                    }
                    

                    i++;

                    dbFunction.AppDoEvents(true);

                }

                dbFunction.ListViewAlternateBackColor(lvwSearch);                
                lblSearchStatus.Text = lvwSearch.Items.Count.ToString() + " " + "record(s) found.";

                sStatementType = "View";
                sReportDescription = clsSearch.ClassReportDescription;
                
                btnPreviewReport.Enabled = true;
            }
            else
            {
                //dbFunction.SetMessageBox("No record found.", "Find IR", clsFunction.IconType.iExclamation);
                lblSearchStatus.Text = "No record found...";

                sStatementType = clsFunction.sNull;
                sSearchBy = clsFunction.sNull;
                sSearchValue = clsFunction.sNull;
                sReportDescription = clsFunction.sNull;
            }

            dbFunction.GetResponseTime("Find IR");            

            /*
            // Focus first item
            if (lvwSearch.Items.Count > 0)
            {
                lvwSearch.FocusedItem = lvwSearch.Items[0];
                lvwSearch.Items[0].Selected = true;
                lvwSearch.Select();
            }
            */
            
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

        private void lvwSearch_SelectedIndexChanged(object sender, EventArgs e)
        {          
            if (lvwSearch.SelectedItems.Count > 0)
            {
                string LineNo = lvwSearch.SelectedItems[0].Text;
                txtLineNo.Text = LineNo;

                if (LineNo.Length > 0)
                {
                    switch (iCountType)
                    {
                        case CountType.iSearchTA:
                            clsSearch.ClassServiceNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassIRIDNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[2].Text);
                            clsSearch.ClassTAIDNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[3].Text);
                            clsSearch.ClassClientID = int.Parse(lvwSearch.SelectedItems[0].SubItems[4].Text);

                            clsSearch.ClassFEID = int.Parse(lvwSearch.SelectedItems[0].SubItems[5].Text);
                            clsSearch.ClassServiceProviderID = int.Parse(lvwSearch.SelectedItems[0].SubItems[6].Text);
                            clsSearch.ClassMerchantID = int.Parse(lvwSearch.SelectedItems[0].SubItems[7].Text);
                            clsSearch.ClassReasonID = int.Parse(lvwSearch.SelectedItems[0].SubItems[8].Text);
                            clsSearch.ClassRegionType = int.Parse(lvwSearch.SelectedItems[0].SubItems[9].Text);

                            clsSearch.ClassIRNo = lvwSearch.SelectedItems[0].SubItems[10].Text;
                            clsSearch.ClassIRRequestDate = lvwSearch.SelectedItems[0].SubItems[11].Text;

                            clsSearch.ClassRequestNo = lvwSearch.SelectedItems[0].SubItems[12].Text;
                            clsSearch.ClassServiceDate = lvwSearch.SelectedItems[0].SubItems[13].Text;

                            clsSearch.ClassMerchantName = lvwSearch.SelectedItems[0].SubItems[14].Text;
                            clsSearch.ClassTID = lvwSearch.SelectedItems[0].SubItems[15].Text;
                            clsSearch.ClassMID = lvwSearch.SelectedItems[0].SubItems[16].Text;

                            clsSearch.ClassTerminalID = int.Parse(lvwSearch.SelectedItems[0].SubItems[17].Text);
                            clsSearch.ClassTerminalSN = lvwSearch.SelectedItems[0].SubItems[18].Text;

                            clsSearch.ClassSIMID = int.Parse(lvwSearch.SelectedItems[0].SubItems[19].Text);
                            clsSearch.ClassSIMSerialNo = lvwSearch.SelectedItems[0].SubItems[20].Text;

                            clsSearch.ClassDockID = int.Parse(lvwSearch.SelectedItems[0].SubItems[21].Text);
                            clsSearch.ClassDockSN = lvwSearch.SelectedItems[0].SubItems[22].Text;

                            clsSearch.ClassJobTypeDescription = lvwSearch.SelectedItems[0].SubItems[23].Text;
                            clsSearch.ClassJobTypeStatusDescription = lvwSearch.SelectedItems[0].SubItems[24].Text;

                            clsSearch.ClassServiceStatus = int.Parse(lvwSearch.SelectedItems[0].SubItems[25].Text);
                            clsSearch.ClassServiceStatusDescription = lvwSearch.SelectedItems[0].SubItems[26].Text;
                            break;
                        case CountType.iSearchServicing:
                            clsSearch.ClassServiceNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassIRIDNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[2].Text);
                            clsSearch.ClassTAIDNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[3].Text);
                            clsSearch.ClassClientID = int.Parse(lvwSearch.SelectedItems[0].SubItems[4].Text);

                            clsSearch.ClassFEID = int.Parse(lvwSearch.SelectedItems[0].SubItems[5].Text);
                            clsSearch.ClassMerchantID = int.Parse(lvwSearch.SelectedItems[0].SubItems[6].Text);
                            clsSearch.ClassReasonID = int.Parse(lvwSearch.SelectedItems[0].SubItems[7].Text);
                            clsSearch.ClassRegionType = int.Parse(lvwSearch.SelectedItems[0].SubItems[8].Text);                            

                            clsSearch.ClassIRNo = lvwSearch.SelectedItems[0].SubItems[9].Text;
                            clsSearch.ClassIRRequestDate = lvwSearch.SelectedItems[0].SubItems[10].Text;

                            clsSearch.ClassRequestNo = lvwSearch.SelectedItems[0].SubItems[11].Text;
                            clsSearch.ClassServiceDate = lvwSearch.SelectedItems[0].SubItems[12].Text;

                            clsSearch.ClassMerchantName = lvwSearch.SelectedItems[0].SubItems[13].Text;
                            clsSearch.ClassTID = lvwSearch.SelectedItems[0].SubItems[14].Text;
                            clsSearch.ClassMID = lvwSearch.SelectedItems[0].SubItems[15].Text;

                            clsSearch.ClassTerminalSN = lvwSearch.SelectedItems[0].SubItems[16].Text;
                            clsSearch.ClassSIMSerialNo = lvwSearch.SelectedItems[0].SubItems[17].Text;
                            clsSearch.ClassRepTerminalSN = lvwSearch.SelectedItems[0].SubItems[18].Text;
                            clsSearch.ClassRepSIMSN = lvwSearch.SelectedItems[0].SubItems[19].Text;

                            clsSearch.ClassJobTypeDescription = lvwSearch.SelectedItems[0].SubItems[20].Text;
                            clsSearch.ClassJobTypeStatusDescription = lvwSearch.SelectedItems[0].SubItems[21].Text;

                            clsSearch.ClassServiceStatus = int.Parse(lvwSearch.SelectedItems[0].SubItems[22].Text);
                            clsSearch.ClassServiceStatusDescription = lvwSearch.SelectedItems[0].SubItems[22].Text;
                            break;
                        case CountType.iSearchFSR:
                            clsSearch.ClassFSRNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassServiceNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[2].Text);
                            clsSearch.ClassIRIDNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[3].Text);
                            clsSearch.ClassTAIDNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[4].Text);
                            clsSearch.ClassClientID = int.Parse(lvwSearch.SelectedItems[0].SubItems[5].Text);
                            clsSearch.ClassIRNo = lvwSearch.SelectedItems[0].SubItems[6].Text;
                            clsSearch.ClassFSRDate = lvwSearch.SelectedItems[0].SubItems[7].Text;
                            clsSearch.ClassMerchantName = lvwSearch.SelectedItems[0].SubItems[8].Text;
                            clsSearch.ClassTID = lvwSearch.SelectedItems[0].SubItems[9].Text;
                            clsSearch.ClassMID = lvwSearch.SelectedItems[0].SubItems[10].Text;
                            clsSearch.ClassTerminalSN = lvwSearch.SelectedItems[0].SubItems[11].Text;
                            clsSearch.ClassSIMSerialNo = lvwSearch.SelectedItems[0].SubItems[12].Text;
                            break;
                        default:
                            clsSearch.ClassTAIDNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[1].Text);
                            clsSearch.ClassIRIDNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[2].Text);
                            clsSearch.ClassIRNo = lvwSearch.SelectedItems[0].SubItems[3].Text;
                            clsSearch.ClassMerchantID = int.Parse(lvwSearch.SelectedItems[0].SubItems[4].Text);
                            clsSearch.ClassMerchantName = lvwSearch.SelectedItems[0].SubItems[5].Text;
                            clsSearch.ClassServiceProviderID = int.Parse(lvwSearch.SelectedItems[0].SubItems[6].Text);
                            clsSearch.ClassServiceProviderName = lvwSearch.SelectedItems[0].SubItems[7].Text;
                            clsSearch.ClassFEID = int.Parse(lvwSearch.SelectedItems[0].SubItems[8].Text);
                            clsSearch.ClassFEName = lvwSearch.SelectedItems[0].SubItems[9].Text;
                            clsSearch.ClassIRRequestDate = lvwSearch.SelectedItems[0].SubItems[10].Text;
                            clsSearch.ClassIRInstallationDate = lvwSearch.SelectedItems[0].SubItems[11].Text;
                            clsSearch.ClassTID = lvwSearch.SelectedItems[0].SubItems[12].Text;
                            clsSearch.ClassMID = lvwSearch.SelectedItems[0].SubItems[13].Text;
                            clsSearch.ClassClientID = int.Parse(lvwSearch.SelectedItems[0].SubItems[15].Text);
                            clsSearch.ClassClientName = lvwSearch.SelectedItems[0].SubItems[16].Text;

                            switch (iCountType)
                            {
                                case CountType.iTInstallationReq:
                                    clsSearch.ClassProcessedDateTime = lvwSearch.SelectedItems[0].SubItems[17].Text;
                                    clsSearch.ClassProcessedBy = lvwSearch.SelectedItems[0].SubItems[18].Text;
                                    clsSearch.ClassProcessType = lvwSearch.SelectedItems[0].SubItems[19].Text;

                                    lblSubHeader.Text = clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC;
                                    break;
                                case CountType.iTInstallation:
                                case CountType.iTServicing:
                                case CountType.iTPullOut:
                                case CountType.iTReplacement:
                                case CountType.iTReprogramming:
                                case CountType.iTDispatch:
                                case CountType.iSVCReqServicing:
                                case CountType.iSVCPullOut:
                                case CountType.iSVCReplacement:
                                case CountType.iSVCReprogramming:
                                case CountType.iSearchIRInstalled:

                                // NEGATIVE
                                case CountType.iSearchNegativeInstallation:
                                case CountType.iSearchNegativePullOut:
                                case CountType.iSearchNegativeReplacement:
                                case CountType.iSearchNegativeReprogramming:
                                case CountType.iSearchNegativeServicing:

                                // COMPLETED
                                case CountType.iTotalInstalled:
                                case CountType.iTotalPullOut:
                                case CountType.iTotalServicing:
                                case CountType.iTotalReplacement:
                                case CountType.iTotalReprogramming:
                                case CountType.iTotalCancelled:

                                    clsSearch.ClassTerminalID = int.Parse(lvwSearch.SelectedItems[0].SubItems[17].Text);
                                    clsSearch.ClassTerminalSN = lvwSearch.SelectedItems[0].SubItems[18].Text;
                                    clsSearch.ClassSIMID = int.Parse(lvwSearch.SelectedItems[0].SubItems[19].Text);
                                    clsSearch.ClassSIMSerialNo = lvwSearch.SelectedItems[0].SubItems[20].Text;
                                    clsSearch.ClassDockID = int.Parse(lvwSearch.SelectedItems[0].SubItems[21].Text);
                                    clsSearch.ClassDockSN = lvwSearch.SelectedItems[0].SubItems[22].Text;
                                    clsSearch.ClassIRStatus = int.Parse(lvwSearch.SelectedItems[0].SubItems[23].Text);
                                    clsSearch.ClassIRStatusDescription = lvwSearch.SelectedItems[0].SubItems[24].Text;
                                    clsSearch.ClassServiceStatus = int.Parse(lvwSearch.SelectedItems[0].SubItems[25].Text);
                                    clsSearch.ClassServiceStatusDescription = lvwSearch.SelectedItems[0].SubItems[26].Text;
                                    clsSearch.ClassServiceNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[27].Text);
                                    clsSearch.ClassRequestNo = lvwSearch.SelectedItems[0].SubItems[28].Text;
                                    clsSearch.ClassJobTypeDescription = lvwSearch.SelectedItems[0].SubItems[29].Text;
                                    clsSearch.ClassServiceDateTime = lvwSearch.SelectedItems[0].SubItems[30].Text;
                                    clsSearch.ClassReasonID = int.Parse(lvwSearch.SelectedItems[0].SubItems[31].Text);
                                    clsSearch.ClassReasonDescription = lvwSearch.SelectedItems[0].SubItems[32].Text;
                                    clsSearch.ClassFSRNo = int.Parse(lvwSearch.SelectedItems[0].SubItems[33].Text);
                                    clsSearch.ClassFSRDate = lvwSearch.SelectedItems[0].SubItems[34].Text;
                                    clsSearch.ClassPrimaryNum = lvwSearch.SelectedItems[0].SubItems[35].Text;
                                    clsSearch.ClassSecondaryNum = lvwSearch.SelectedItems[0].SubItems[36].Text;

                                    lblSubHeader.Text = clsSearch.ClassJobTypeDescription + (!dbFunction.isValidDescription(clsSearch.ClassServiceStatusDescription) ? "" : "-" + clsSearch.ClassServiceStatusDescription);
                                    break;
                            }

                            break;
                    }
                                
                }
            }
        }

        private void lvwSearch_DoubleClick(object sender, EventArgs e)
        {
            if (lvwSearch.Items.Count > 0)
            {
                Debug.WriteLine("--lvwSearch_DoubleClick--");
                Debug.WriteLine("clsSearch.ClassTAIDNo=" + clsSearch.ClassTAIDNo);
                Debug.WriteLine("clsSearch.ClassIRIDNo=" + clsSearch.ClassIRIDNo);
                Debug.WriteLine("=clsSearch.ClassServiceNo=" + clsSearch.ClassServiceNo);
                Debug.WriteLine("clsSearch.ClassClientID=" + clsSearch.ClassClientID);
                Debug.WriteLine("clsSearch.ClassFEID=" + clsSearch.ClassFEID);
                Debug.WriteLine("clsSearch.ClassMerchantID=" + clsSearch.ClassMerchantID);
                Debug.WriteLine("clsSearch.ClassReasonID=" + clsSearch.ClassReasonID);
                Debug.WriteLine("clsSearch.ClassRegionType=" + clsSearch.ClassRegionType);                
                Debug.WriteLine("clsSearch.ClassIRNo=" + clsSearch.ClassIRNo);
                Debug.WriteLine("clsSearch.ClassIRRequestDate=" + clsSearch.ClassIRRequestDate);
                Debug.WriteLine("clsSearch.ClassRequestNo=" + clsSearch.ClassRequestNo);
                Debug.WriteLine("clsSearch.ClassServiceDate=" + clsSearch.ClassServiceDate);
                Debug.WriteLine("clsSearch.ClassMerchantName=" + clsSearch.ClassMerchantName);
                Debug.WriteLine("clsSearch.ClassTID=" + clsSearch.ClassTID);
                Debug.WriteLine("clsSearch.ClassMID=" + clsSearch.ClassMID);
                Debug.WriteLine("clsSearch.ClassTerminalSN=" + clsSearch.ClassTerminalSN);
                Debug.WriteLine("clsSearch.ClassSIMSerialNo=" + clsSearch.ClassSIMSerialNo);
                Debug.WriteLine("clsSearch.ClassRepTerminalSN=" + clsSearch.ClassRepTerminalSN);
                Debug.WriteLine("clsSearch.ClassRepSIMSN=" + clsSearch.ClassRepSIMSN);
                Debug.WriteLine("clsSearch.ClassJobTypeDescription=" + clsSearch.ClassJobTypeDescription);
                Debug.WriteLine("clsSearch.ClassJobTypeStatusDescription=" + clsSearch.ClassJobTypeStatusDescription);


                switch (iCountType)
                {
                    case CountType.iTInstallationReq:
                    case CountType.iTServicing:
                    case CountType.iTPullOut:
                    case CountType.iTReplacement:
                    case CountType.iTReprogramming:
                    case CountType.iTDispatch:
                    case CountType.iSVCReqServicing:
                    case CountType.iSVCPullOut:
                    case CountType.iSVCReplacement:
                    case CountType.iSVCReprogramming:
                    case CountType.iSearchIRInstalled:
                    case CountType.iSearchFSR:
                    case CountType.iSearchServicing:
                    case CountType.iSearchTA:
                        fSelected = true;
                        this.Close();
                        break;

                    // Negative   
                    case CountType.iSearchNegativeInstallation:
                    case CountType.iSearchNegativePullOut:
                    case CountType.iSearchNegativeReplacement:
                    case CountType.iSearchNegativeReprogramming:
                    case CountType.iSearchNegativeServicing:

                        if (clsSearch.ClassIRIDNo > 0)
                        {
                            if (clsSearch.ClassIRStatus == clsGlobalVariables.STATUS_NEGATIVE || clsSearch.ClassServiceStatus == clsGlobalVariables.STATUS_NEGATIVE)
                            {
                                if (!fConfirmReOpenService())
                                {
                                    fSelected = false;
                                    return;
                                }
                            }

                            fSelected = true;
                            this.Close();
                        }                        
                        break;
                    default:
                        break;
                }                            
            }
        }

        private void lvwSearch_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    lvwSearch_DoubleClick(this, e);
                    break;
            }
        }

        private void SelectedMenu(CountType iCountType)
        {
            bool fMultiple = false;
            int i = 0;
            bool fValid = true;

            TAIDNoCol.Clear();
            IRIDNoCol.Clear();
            IRNoCol.Clear();
            TerminalIDCol.Clear();
            TerminalSNCol.Clear();
            SIMIDCol.Clear();
            SIMSerialNoCol.Clear();
            DockIDCol.Clear();
            DockSNCol.Clear();
            MerchantIDCol.Clear();
            MerchantNameCol.Clear();
            ServiceProviderIDCol.Clear();
            ServiceProviderNameCol.Clear();
            FEIDCol.Clear();
            FENameCol.Clear();
            ClientIDCol.Clear();
            ClientNameCol.Clear();
            IRStatusCol.Clear();
            IRStatusDescriptionCol.Clear();
            ServiceStatusCol.Clear();
            ServiceStatusDescriptionCol.Clear();
            ServiceNoCol.Clear();
            RequestNoCol.Clear();
            JobTypeDescriptionCol.Clear();
            ServiceDateTimeCol.Clear();
            ReasonIDCol.Clear();
            ReasonDescriptionCol.Clear();

            foreach (ListViewItem item in lvwSearch.Items)
            {
                if (item.Selected)
                {
                    i++;
                }

                if (i > 1)
                {
                    fMultiple = true;
                    break;
                }
            }
            
            if (fMultiple)
            {
                fSelected = true;
                foreach (ListViewItem item in lvwSearch.Items)
                {
                    if (item.Selected)
                    {
                        TAIDNoCol.Add(item.SubItems[1].Text);
                        IRIDNoCol.Add(item.SubItems[2].Text);
                        IRNoCol.Add(item.SubItems[3].Text);
                        MerchantIDCol.Add(item.SubItems[4].Text);
                        MerchantNameCol.Add(item.SubItems[5].Text);
                        ServiceProviderIDCol.Add(item.SubItems[6].Text);
                        ServiceProviderNameCol.Add(item.SubItems[7].Text);
                        FEIDCol.Add(item.SubItems[8].Text);
                        FENameCol.Add(item.SubItems[9].Text);
                        ClientIDCol.Add(item.SubItems[15].Text);
                        ClientNameCol.Add(item.SubItems[16].Text);

                        switch (iCountType)
                        {
                            case CountType.iTInstallationReq:
                                break;
                            case CountType.iTInstallation:
                            case CountType.iTServicing:
                            case CountType.iTPullOut:
                            case CountType.iTReplacement:
                            case CountType.iTReprogramming:
                            case CountType.iTDispatch:                                                        
                            case CountType.iSVCReqServicing:
                            case CountType.iSVCPullOut:
                            case CountType.iSVCReplacement:
                            case CountType.iSVCReprogramming:

                            // NEGATIVE
                            case CountType.iSearchNegativeInstallation:
                            case CountType.iSearchNegativePullOut:
                            case CountType.iSearchNegativeReplacement:
                            case CountType.iSearchNegativeReprogramming:
                            case CountType.iSearchNegativeServicing:

                            // COMPLETED
                            case CountType.iTotalInstalled:
                            case CountType.iTotalPullOut:
                            case CountType.iTotalServicing:
                            case CountType.iTotalReplacement:
                            case CountType.iTotalReprogramming:
                            case CountType.iTotalCancelled:

                                TerminalIDCol.Add(item.SubItems[17].Text);
                                SIMIDCol.Add(item.SubItems[19].Text);
                                DockIDCol.Add(item.SubItems[21].Text);
                                IRStatusCol.Add(item.SubItems[23].Text);
                                IRStatusDescriptionCol.Add(item.SubItems[24].Text);
                                ServiceStatusCol.Add(item.SubItems[25].Text);
                                ServiceStatusDescriptionCol.Add(item.SubItems[26].Text);
                                ServiceNoCol.Add(item.SubItems[27].Text);
                                RequestNoCol.Add(item.SubItems[28].Text);
                                JobTypeDescriptionCol.Add(item.SubItems[29].Text);
                                ServiceDateTimeCol.Add(item.SubItems[30].Text);
                                ReasonIDCol.Add(item.SubItems[31].Text);
                                ReasonDescriptionCol.Add(item.SubItems[32].Text);
                                break;
                        }    
                    }
                }
            }
            else
            {
                if (txtLineNo.Text.Length > 0 && txtLineNo.Text.CompareTo(clsFunction.sZero) != 0)
                {
                    fSelected = true;
                    TAIDNoCol.Add(lvwSearch.SelectedItems[0].SubItems[1].Text);
                    IRIDNoCol.Add(lvwSearch.SelectedItems[0].SubItems[2].Text);
                    IRNoCol.Add(lvwSearch.SelectedItems[0].SubItems[3].Text);
                    MerchantIDCol.Add(lvwSearch.SelectedItems[0].SubItems[4].Text);
                    MerchantNameCol.Add(lvwSearch.SelectedItems[0].SubItems[5].Text);
                    ServiceProviderIDCol.Add(lvwSearch.SelectedItems[0].SubItems[6].Text);
                    ServiceProviderNameCol.Add(lvwSearch.SelectedItems[0].SubItems[7].Text);
                    FEIDCol.Add(lvwSearch.SelectedItems[0].SubItems[8].Text);
                    FENameCol.Add(lvwSearch.SelectedItems[0].SubItems[9].Text);
                    ClientIDCol.Add(lvwSearch.SelectedItems[0].SubItems[15].Text);
                    ClientNameCol.Add(lvwSearch.SelectedItems[0].SubItems[16].Text);

                    switch (iCountType)
                    {
                        case CountType.iTInstallationReq:
                            break;
                        case CountType.iTInstallation:
                        case CountType.iTServicing:
                        case CountType.iTPullOut:
                        case CountType.iTReplacement:
                        case CountType.iTReprogramming:
                        case CountType.iTDispatch:                        
                        case CountType.iSVCReqServicing:
                        case CountType.iSVCPullOut:
                        case CountType.iSVCReplacement:
                        case CountType.iSVCReprogramming:

                        // NEGATIVE
                        case CountType.iSearchNegativeInstallation:
                        case CountType.iSearchNegativePullOut:
                        case CountType.iSearchNegativeReplacement:
                        case CountType.iSearchNegativeReprogramming:
                        case CountType.iSearchNegativeServicing:

                        // COMPLETED
                        case CountType.iTotalInstalled:
                        case CountType.iTotalPullOut:
                        case CountType.iTotalServicing:
                        case CountType.iTotalReplacement:
                        case CountType.iTotalReprogramming:
                        case CountType.iTotalCancelled:

                            TerminalIDCol.Add(lvwSearch.SelectedItems[0].SubItems[17].Text);
                            SIMIDCol.Add(lvwSearch.SelectedItems[0].SubItems[19].Text);
                            DockIDCol.Add(lvwSearch.SelectedItems[0].SubItems[21].Text);
                            IRStatusCol.Add(lvwSearch.SelectedItems[0].SubItems[23].Text);
                            IRStatusDescriptionCol.Add(lvwSearch.SelectedItems[0].SubItems[24].Text);
                            ServiceStatusCol.Add(lvwSearch.SelectedItems[0].SubItems[25].Text);
                            ServiceStatusDescriptionCol.Add(lvwSearch.SelectedItems[0].SubItems[26].Text);
                            ServiceNoCol.Add(lvwSearch.SelectedItems[0].SubItems[27].Text);
                            RequestNoCol.Add(lvwSearch.SelectedItems[0].SubItems[28].Text);
                            JobTypeDescriptionCol.Add(lvwSearch.SelectedItems[0].SubItems[29].Text);
                            ServiceDateTimeCol.Add(lvwSearch.SelectedItems[0].SubItems[30].Text);
                            ReasonIDCol.Add(lvwSearch.SelectedItems[0].SubItems[31].Text);
                            ReasonDescriptionCol.Add(lvwSearch.SelectedItems[0].SubItems[31].Text);
                            break;
                    }
                }
                else
                {
                    dbFunction.SetMessageBox("No merchant selected. Please chose item on the list.", "No selection.", clsFunction.IconType.iError);                    
                }
            }

            clsArray.TAIDNo = TAIDNoCol.ToArray();
            clsArray.IRIDNo = IRIDNoCol.ToArray();            
            clsArray.IRNo = IRNoCol.ToArray();
            clsArray.FEID = FEIDCol.ToArray();
            clsArray.ServiceProviderID = ServiceProviderIDCol.ToArray();
            clsArray.MerchantName = MerchantNameCol.ToArray();
            clsArray.TerminalID = TerminalIDCol.ToArray();
            clsArray.SIMID = SIMIDCol.ToArray();
            clsArray.DockID = DockIDCol.ToArray();            
            clsArray.IRStatus = IRStatusCol.ToArray();
            clsArray.IRStatusDescription = IRStatusDescriptionCol.ToArray();
            clsArray.ServiceStatus = ServiceStatusCol.ToArray();
            clsArray.ServiceStatusDescription = ServiceStatusDescriptionCol.ToArray();
            clsArray.ServiceNo= ServiceNoCol.ToArray();
            clsArray.RequestNo = RequestNoCol.ToArray();
            clsArray.JobTypeDescription = JobTypeDescriptionCol.ToArray();
            clsArray.ServiceDateTime = ServiceDateTimeCol.ToArray();
            clsArray.ReasonID = ReasonIDCol.ToArray();
            clsArray.ReasonDescription = ReasonDescriptionCol.ToArray();

            if (iPopupMenuType == PopupMenuType.iDispatch)
            {
                if (clsSearch.ClassTAIDNo > 0)
                {                    
                    // Check for Field Engineer
                    for (i = 0; i < clsArray.FEID.Length; i++)
                    {
                        if (!dbFunction.isValidID(clsArray.FEID[i]))
                        {
                            dbFunction.SetMessageBox("Field Engineer must not be blank." +
                                                     "\n\n" +
                                                     "Merchant Name: " + clsArray.MerchantName[i] + "\n" +
                                                     "Client Name: " + clsArray.ClientName[i] + "\n" +
                                                     "Request ID: " + clsArray.IRNo[i]  + "\n" +
                                                     "Service ID: " + clsArray.RequestNo[i] + "\n" +
                                                     "\n\n" +                                                     
                                                     "Modify service request " + clsArray.JobTypeDescription[i] + " to process.", "Dispatch", clsFunction.IconType.iExclamation);
                            fValid = false;
                            break;                            
                        }

                        if (!fValid) return;
                    }

                    // Check for Service Provider
                    for (i = 0; i < clsArray.ServiceProviderID.Length; i++)
                    {
                        if (!dbFunction.isValidID(clsArray.ServiceProviderID[i]))
                        {
                            dbFunction.SetMessageBox("Service Provider must not be blank." +
                                                     "\n\n" +
                                                     "Merchant Name: " + clsArray.MerchantName[i] + "\n" +
                                                     "Client Name: " + clsArray.ClientName[i] + "\n" +
                                                     "Request ID: " + clsArray.IRNo[i] + "\n" +
                                                     "Service ID: " + clsArray.RequestNo[i] + "\n" +
                                                     "\n\n" +
                                                     "Modify service request " + clsArray.JobTypeDescription[i] + " to process.", "Dispatch", clsFunction.IconType.iExclamation);
                            fValid = false;
                            break;
                        }                        
                    }

                    if (fValid)
                    {
                        ProcessDispatch();
                        fSelected = false;
                    }                    
                }
                else
                {
                    dbFunction.SetMessageBox("No merchant selected. Please chose item on the list.", "No selection.", clsFunction.IconType.iError);
                }
                
            }

            if (iPopupMenuType == PopupMenuType.iDelete)
            {
                ProcessIRDelete();
                fSelected = false;
            }

            if (iPopupMenuType == PopupMenuType.iModify)
            {
                if (clsSearch.ClassIRIDNo > 0)
                {
                    if (clsSearch.ClassIRStatus == clsGlobalVariables.STATUS_NEGATIVE || clsSearch.ClassServiceStatus == clsGlobalVariables.STATUS_NEGATIVE)
                    {
                        if (!fConfirmReOpenService())
                        {
                            fSelected = false;
                            return;
                        }
                    }

                    fSelected = true;
                    this.Close();
                }
                else
                {
                    dbFunction.SetMessageBox("No merchant selected. Please chose item on the list.", "No selection.", clsFunction.IconType.iError);
                }                
            }

            if (iPopupMenuType == PopupMenuType.iPreview)
            {
                if (clsSearch.ClassTAIDNo > 0)
                {
                    Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass
                    ProcessIRPreview();
                    Cursor.Current = Cursors.Default;  // Back to normal 
                    fSelected = false;
                }
                else
                {
                    dbFunction.SetMessageBox("No merchant selected. Please chose item on the list.", "No selection.", clsFunction.IconType.iError);
                }                
            }

            if (iPopupMenuType == PopupMenuType.iFSR)
            {
                if (clsSearch.ClassTAIDNo > 0)
                {
                    fSelected = true;
                    this.Close();
                }
                else
                {
                    dbFunction.SetMessageBox("No merchant selected. Please chose item on the list.", "No selection.", clsFunction.IconType.iError);
                }                
            }
            
        }

        private void ProcessDispatch()
        {
            int i = 0;
            int iCount = 0;
            string sItem = "";
            string sHeader = "";
            DateTime ProcessDateTime = DateTime.Now;
            string sProcessDate = "";
            string sProcessTime = "";
            string sProcessedBy = clsUser.ClassUserFullName;

            sProcessDate = ProcessDateTime.ToString("yyyy-MM-dd");
            sProcessTime = ProcessDateTime.ToString("H:mm:ss");

            if (clsArray.TAIDNo.Length > 0)
            {
                sHeader =
                         "-----------------------------------------------------------------------" + 
                         Environment.NewLine + 
                         "REQUEST NO | STATUS | TERMINAL SN | SIM SN | SERVICE" +
                         Environment.NewLine +
                         "-----------------------------------------------------------------------";

                // Display to be dispatch
                while (clsArray.TAIDNo.Length > i)
                {
                    string sTemp = clsArray.IRNo[i].ToString() + clsFunction.sPipe + clsFunction.sPadSpace +
                                   clsArray.IRStatusDescription[i].ToString() + clsFunction.sPipe + clsFunction.sPadSpace +
                                   clsArray.TerminalSN[i].ToString() + clsFunction.sPipe + clsFunction.sPadSpace +
                                   clsArray.SIMSerialNo[i].ToString() + clsFunction.sPipe + clsFunction.sPadSpace +                                   
                                   clsArray.ServiceStatusDescription[i].ToString() + clsFunction.sPipe + clsFunction.sPadSpace +
                                   Environment.NewLine;

                    sItem = sItem + sTemp;

                    string sStatusDescription = clsArray.ServiceStatusDescription[i].ToString();
                    string sIRStatusDescription = clsArray.IRStatusDescription[i].ToString();

                    if ((sStatusDescription.CompareTo(clsGlobalVariables.STATUS_ALLOCATED_DESC) == 0) ||
                        (sIRStatusDescription.CompareTo(clsGlobalVariables.STATUS_ALLOCATED_DESC) == 0) ||
                        (sStatusDescription.CompareTo(clsGlobalVariables.STATUS_SERVICING_DESC) == 0) ||
                        (sStatusDescription.CompareTo(clsGlobalVariables.STATUS_PULLED_OUT_DESC) == 0) ||
                        (sStatusDescription.CompareTo(clsGlobalVariables.STATUS_REPLACEMENT_DESC) == 0) ||
                        (sStatusDescription.CompareTo(clsGlobalVariables.STATUS_REPROGRAMMED_DESC) == 0))
                        iCount++;

                    i++;

                }

                if (MessageBox.Show("Are you sure to process selected row(s)?\n" +
                               "Total count: " + iCount.ToString() +
                               "\n\n" +
                               sHeader +
                               "\n" +
                               sItem +                               
                               "\n\n",
                               "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    return;
                }

                i = 0;
                while (clsArray.TAIDNo.Length > i)
                {
                    string sStatusDescription = clsArray.ServiceStatusDescription[i].ToString();
                    string sIRStatusDescription = clsArray.IRStatusDescription[i].ToString();

                    if ((sStatusDescription.CompareTo(clsGlobalVariables.STATUS_ALLOCATED_DESC) == 0) ||
                        (sIRStatusDescription.CompareTo(clsGlobalVariables.STATUS_ALLOCATED_DESC) == 0) ||
                        (sStatusDescription.CompareTo(clsGlobalVariables.STATUS_SERVICING_DESC) == 0) ||
                        (sStatusDescription.CompareTo(clsGlobalVariables.STATUS_PULLED_OUT_DESC) == 0) ||
                        (sStatusDescription.CompareTo(clsGlobalVariables.STATUS_REPLACEMENT_DESC) == 0) ||
                        (sStatusDescription.CompareTo(clsGlobalVariables.STATUS_REPROGRAMMED_DESC) == 0))
                    {
                        clsSearch.ClassStatus = clsGlobalVariables.STATUS_DISPATCH;
                        clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_DISPATCH_DESC;

                        string sServiceNo = clsArray.ServiceNo[i];
                        string sRequestNo = clsArray.RequestNo[i];
                        string sIRIDNo = clsArray.IRIDNo[i];
                        string sTAIDNo = clsArray.TAIDNo[i];
                        string sTerminalID = clsArray.TerminalID[i];
                        string sSIMID = clsArray.SIMID[i];
                        string sDockID = clsArray.DockID[i];

                        Debug.WriteLine("ProcessDispatch::"+"i=" +i.ToString()+"|"+
                                        "iCountType=" + iCountType.ToString() + "|" +
                                        "sServiceNo=" + sServiceNo + "|" +
                                        "sRequestNo=" + sRequestNo + "|" +
                                        "sIRIDNo=" + sIRIDNo + "|" +
                                        "sTAIDNo=" + sTAIDNo + "|" +
                                        "sTerminalID=" + sTerminalID + "|" +
                                        "sSIMID=" + sSIMID + "|" +
                                        "sDockID=" + sDockID + "\n");

                        switch (iCountType)
                        {
                            case CountType.iTPullOut:
                            case CountType.iTReplacement:
                            case CountType.iTReprogramming:
                            case CountType.iTServicing:
                                if (dbFunction.isValidID(sServiceNo))
                                    dbAPI.UpdateServicingDispatch(sServiceNo, sRequestNo, sProcessDate, sProcessTime, sProcessedBy);

                                if (dbFunction.isValidID(sServiceNo))
                                    dbAPI.UpdateServiceStatus(sServiceNo, sRequestNo, clsSearch.ClassStatus, clsSearch.ClassStatusDescription, clsSearch.ClassIRIDNo.ToString());
                                break;
                            case CountType.iTInstallation:

                                if (dbFunction.isValidID(sIRIDNo))
                                    dbAPI.UpdateIRDetailStatus(sIRIDNo, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                                if (dbFunction.isValidID(sTAIDNo))
                                    dbAPI.UpdateTADetailStatus(sTAIDNo, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                                if (dbFunction.isValidID(sServiceNo))
                                    dbAPI.UpdateServicingDispatch(sServiceNo, sRequestNo, sProcessDate, sProcessTime, sProcessedBy);

                                if (dbFunction.isValidID(sServiceNo))
                                    dbAPI.UpdateServiceStatus(sServiceNo, sRequestNo, clsSearch.ClassStatus, clsSearch.ClassStatusDescription, clsSearch.ClassIRIDNo.ToString());
                                break;                                                            
                            default:
                                if (dbFunction.isValidID(sTerminalID))
                                    dbAPI.UpdateTerminalDetailStatus(sTerminalID, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                                if (dbFunction.isValidID(sSIMID))
                                    dbAPI.UpdateSIMDetailStatus(sSIMID, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                                if (dbFunction.isValidID(sDockID))
                                    dbAPI.UpdateTerminalDetailStatus(sDockID, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                                if (dbFunction.isValidID(sIRIDNo))
                                    dbAPI.UpdateIRDetailStatus(sIRIDNo, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);

                                if (dbFunction.isValidID(sTAIDNo))
                                    dbAPI.UpdateTADetailStatus(sTAIDNo, clsSearch.ClassStatus, clsSearch.ClassStatusDescription);
                                break;
                        }

                        // Job Type
                        if (dbFunction.isValidID(sServiceNo))
                        {
                            switch (iCountType)
                            {
                                case CountType.iTInstallation:
                                case CountType.iTInstallationReq:
                                    clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC;
                                    clsSearch.ClassJobTypeSubDescription = clsGlobalVariables.TA_STATUS_INSTALLED_SUB_DESC;
                                    break;
                                case CountType.iTServicing:
                                case CountType.iSVCReqServicing:
                                    clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_SERVICING_DESC;
                                    clsSearch.ClassJobTypeSubDescription = clsGlobalVariables.TA_STATUS_SERVICING_SUB_DESC;
                                    break;
                                case CountType.iTPullOut:
                                case CountType.iSVCPullOut:
                                    clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_PULLOUT_DESC;
                                    clsSearch.ClassJobTypeSubDescription = clsGlobalVariables.TA_STATUS_PULLEDOUT_SUB_DESC;
                                    break;
                                case CountType.iTReplacement:
                                case CountType.iSVCReplacement:
                                    clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC;
                                    clsSearch.ClassJobTypeSubDescription = clsGlobalVariables.TA_STATUS_REPLACEMENT_SUB_DESC;
                                    break;
                                case CountType.iTReprogramming:
                                case CountType.iSVCReprogramming:
                                    clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC;
                                    clsSearch.ClassJobTypeSubDescription = clsGlobalVariables.TA_STATUS_REPROGRAMMED_SUB_DESC;
                                    break;
                            }

                            dbAPI.UpdateServiceJobType(sServiceNo, sRequestNo, clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING, clsSearch.ClassJobTypeDescription, clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING_DESC, clsSearch.ClassJobTypeSubDescription);
                        }
                            
                    }

                    i++;
                }

                if (iCount > 0)
                {
                    dbFunction.SetMessageBox("Terminal dispatch successfully.", "Dispatch Terminal", clsFunction.IconType.iInformation);

                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
                    dbAPI.ResetAdvanceSearch();
                    clsSearch.ClassIRStatus = clsGlobalVariables.STATUS_ALLOCATED;
                    LoadIR("View", "", "");
                    Cursor.Current = Cursors.Default;  // Back to normal
                }
                else
                    dbFunction.SetMessageBox("No terminal to be dispatch.", "Dispatch Terminal", clsFunction.IconType.iExclamation);
            }
        }

        private void ProcessIRDelete()
        {
            int i = 0;
            int iCount = 0;
            string sItem = "";

            if (clsArray.IRIDNo.Length > 0)
            {
                // Display to be dispatch
                while (clsArray.TAIDNo.Length > i)
                {
                    string sTemp = clsArray.IRNo[i].ToString() + clsFunction.sComma + clsFunction.sPadSpace +
                                   clsArray.MerchantName[i].ToString() + clsFunction.sComma + clsFunction.sPadSpace +
                                   clsArray.TerminalSN[i].ToString() + clsFunction.sComma + clsFunction.sPadSpace +
                                   clsArray.SIMSerialNo[i].ToString() + clsFunction.sComma + clsFunction.sPadSpace +
                                   clsArray.DockSN[i].ToString() + Environment.NewLine;
                    sItem = sItem + sTemp;
                    
                    iCount++;
                    i++;
                }

                if (MessageBox.Show("Do you really want to delete selected row(s)?\n" +
                               "Total count: " + iCount.ToString() +
                               "\n\n" +
                               sItem +                               
                               "\n\n" +
                               "Warning:\nData will permanently deleted and included serial number(s) used will be set to available." +                               
                               "\n",
                               "Confirm Dispatch Service Request?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    return;
                }

                i = 0;
                while (clsArray.IRIDNo.Length > i)
                {
                    
                    dbAPI.DeleteIRDetail(clsArray.IRIDNo[i].ToString(), clsArray.IRNo[i].ToString()); // Delete from (tblirdetail)
                    dbAPI.DeleteTADetail(clsArray.TAIDNo[i].ToString()); // Delete from (tblterminalallocation)
                    
                    clsSearch.ClassStatus = clsGlobalVariables.STATUS_AVAILABLE;
                    clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_AVAILABLE_DESC;

                    string sTAIDNo = clsArray.TAIDNo[i].ToString();
                    if (sTAIDNo.Length > 0 && sTAIDNo.CompareTo(clsFunction.sZero) != 0)
                    {
                        string sTerminalID = clsArray.TerminalID[i].ToString();
                        string sTerminalSN = clsArray.TerminalSN[i].ToString();

                        string sSIMID = clsArray.SIMID[i].ToString();
                        string sSIMSN = clsArray.SIMSerialNo[i].ToString();

                        string sDockID = clsArray.DockID[i].ToString();
                        string sDockSN = clsArray.DockSN[i].ToString();

                        if (sTerminalSN.Length > 0 && sTerminalSN.CompareTo(clsFunction.sDash) != 0)
                            dbAPI.UpdateTerminalDetailStatus(clsArray.TerminalID[i].ToString(), clsSearch.ClassStatus, clsSearch.ClassStatusDescription); // Update Terminal Detail

                        if (sSIMSN.Length > 0 && sSIMSN.CompareTo(clsFunction.sDash) != 0)
                            dbAPI.UpdateSIMDetailStatus(clsArray.SIMID[i].ToString(), clsSearch.ClassStatus, clsSearch.ClassStatusDescription);         // Update SIM Detail      

                        if (sDockSN.Length > 0 && sDockSN.CompareTo(clsFunction.sDash) != 0)
                            dbAPI.UpdateTerminalDetailStatus(clsArray.DockID[i].ToString(), clsSearch.ClassStatus, clsSearch.ClassStatusDescription);         // Update Dock Detail   
                    }                       

                    i++;
                }

                if (iCount > 0)
                {
                    dbFunction.SetMessageBox("Installation request successfully deleted.", "Deleted", clsFunction.IconType.iInformation);

                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
                    dbAPI.ResetAdvanceSearch();
                    clsSearch.ClassIRStatus = clsGlobalVariables.STATUS_ALLOCATED;
                    LoadIR("View", "", "");
                    Cursor.Current = Cursors.Default;  // Back to normal
                }
                else
                    dbFunction.SetMessageBox("No installation request to be deleted.", "Installation Request", clsFunction.IconType.iExclamation);
            }
        }

        private void ProcessServicingDelete()
        {
            int i = 0;
            int iCount = 0;
            string sItem = "";

            if (clsArray.IRIDNo.Length > 0)
            {
                // Display
                while (clsArray.TAIDNo.Length > i)
                {
                    string sTemp = clsArray.IRNo[i].ToString() + clsFunction.sComma + clsFunction.sPadSpace +
                                   clsArray.MerchantName[i].ToString() + clsFunction.sComma + clsFunction.sPadSpace +
                                   clsArray.TerminalSN[i].ToString() + Environment.NewLine;
                    sItem = sItem + sTemp;

                    iCount++;
                    i++;
                }

                if (MessageBox.Show("Do you really want to delete selected row(s)?\n" +
                               "Total count: " + iCount.ToString() +
                               "\n\n" +
                               sItem +
                               "\n\n" +
                               "Warning:\nData will permanently deleted and included serial number(s) used" +
                               "         will be set to available." +
                               "\n",
                               "Confirm Delete Service Request?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    return;
                }

                i = 0;
                while (clsArray.IRIDNo.Length > i)
                {

                    dbAPI.DeleteIRDetail(clsArray.IRIDNo[i].ToString(), clsArray.IRNo[i].ToString()); // Delete from (tblirdetail)
                    dbAPI.DeleteTADetail(clsArray.TAIDNo[i].ToString()); // Delete from (tblterminalallocation)

                    clsSearch.ClassStatus = clsGlobalVariables.STATUS_AVAILABLE;
                    clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_AVAILABLE_DESC;

                    dbAPI.UpdateTerminalDetailStatus(clsArray.TerminalID[i].ToString(), clsSearch.ClassStatus, clsSearch.ClassStatusDescription); // Update Terminal Detail
                    dbAPI.UpdateSIMDetailStatus(clsArray.SIMID[i].ToString(), clsSearch.ClassStatus, clsSearch.ClassStatusDescription);         // Update SIM Detail           

                    i++;
                }

                if (iCount > 0)
                {
                    dbFunction.SetMessageBox("Installation request successfully deleted.", "Deleted", clsFunction.IconType.iInformation);

                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
                    dbAPI.ResetAdvanceSearch();
                    clsSearch.ClassIRStatus = clsGlobalVariables.STATUS_ALLOCATED;
                    LoadIR("View", "", "");
                    Cursor.Current = Cursors.Default;  // Back to normal
                }
                else
                    dbFunction.SetMessageBox("No installation request to be deleted.", "Installation Request", clsFunction.IconType.iExclamation);
            }
        }

        private void ProcessIRPreview()
        {
            int i = 0;
            int iCount = 0;
            string sItem = "";

            if (clsArray.IRIDNo.Length > 0)
            {
                // Display to be dispatch
                while (clsArray.TAIDNo.Length > i)
                {
                    string sTemp = clsArray.IRNo[i].ToString() + clsFunction.sComma + clsFunction.sPadSpace +
                                   clsArray.MerchantName[i].ToString() + clsFunction.sComma + clsFunction.sPadSpace +
                                   clsArray.TerminalSN[i].ToString() + Environment.NewLine;
                    sItem = sItem + sTemp;

                    iCount++;
                    i++;
                }

                if (MessageBox.Show("Are you sure to preview selected row(s)?\n" +
                               "Total count: " + iCount.ToString() +
                               "\n\n" +
                               sItem +                               
                               "\n",
                               "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    return;
                }

                i = 0;
                while (clsArray.IRIDNo.Length > i)
                {
                    // Preview
                    string sIRIDNo = clsArray.IRIDNo[i].ToString();
                    string sIRNo = clsArray.IRNo[i].ToString();
                    if (sIRIDNo.Length > 0)
                    {
                        dbFunction.PreviewTA(sIRNo, clsSearch.ClassServiceNo, clsSearch.ClassFSRNo);
                    }

                    i++;
                }               
            }
        }

        private void InitMultiSelect()
        {
            if (fMultiSelect)
                lvwSearch.MultiSelect = true;
            else
                lvwSearch.MultiSelect = false;
        }

        private void tsmTADispatch_Click(object sender, EventArgs e)
        {
            if (lvwSearch.Items.Count > 0)
            {
                iPopupMenuType = PopupMenuType.iDispatch;
                SelectedMenu(iCountType);
            }                
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            //dbAPI.ResetAdvanceSearch();
            //clsSearch.ClassIRStatus = iHoldIRStatus;
            //LoadIR("View", "", "");
            //lblSubHeader.Text = "";
        }

        private void frmFindCount_Activated(object sender, EventArgs e)
        {
            txtSearch.Focus();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    Cursor.Current = Cursors.WaitCursor;
                    dbAPI.ResetAdvanceSearch();
                    clsSearch.ClassIRStatus = iHoldIRStatus;                    
                    ProcessPage(iCountType);
                    LoadIR("View", "", "");
                    Cursor.Current = Cursors.Default;
                    break;
                case Keys.Down:
                    if (lvwSearch.Items.Count > 0)
                    {
                        lvwSearch.Focus();
                    }
                    break;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnExit_Click(this, e);
        }

        private void deleteRequestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void InitPopupMenu()
        {            
            switch (iCountType)
            {
                case CountType.iTInstallationReq:
                    tsmPreviewFSR.Enabled = false;
                    tsmIRDelete.Enabled = false;
                    tsmTADispatch.Enabled = false;
                    tsmIRAllocate.Enabled = true;
                    tsmFSR.Enabled = false;
                    tsmTAModify.Enabled = false;
                    break;
                case CountType.iTInstallation:
                case CountType.iTPullOut:
                    tsmPreviewFSR.Enabled = true;
                    tsmIRDelete.Enabled = false;
                    tsmTADispatch.Enabled = true;
                    tsmIRAllocate.Enabled = false;
                    tsmFSR.Enabled = false;
                    tsmTAModify.Enabled = true;
                    break;
                case CountType.iTDispatch:
                    tsmPreviewFSR.Enabled = true;
                    tsmIRDelete.Enabled = false;
                    tsmTADispatch.Enabled = false;
                    tsmIRAllocate.Enabled = false;
                    tsmFSR.Enabled = true;
                    tsmTAModify.Enabled = false;
                    break;
                case CountType.iTServicing:                
                case CountType.iTReplacement:
                case CountType.iTReprogramming:
                    tsmPreviewFSR.Enabled = false;
                    tsmIRDelete.Enabled = false;
                    tsmTADispatch.Enabled = true;
                    tsmIRAllocate.Enabled = false;
                    tsmFSR.Enabled = false;
                    tsmTAModify.Enabled = false;
                    break;
                case CountType.iSearchIRInstalled:
                case CountType.iSearchNegativeInstallation:
                case CountType.iSearchNegativePullOut:
                case CountType.iSearchNegativeReplacement:
                case CountType.iSearchNegativeReprogramming:
                case CountType.iSearchNegativeServicing:
                    tsmPreviewFSR.Enabled = false;
                    tsmIRDelete.Enabled = false;
                    tsmTADispatch.Enabled = false;
                    tsmIRAllocate.Enabled = false;
                    tsmFSR.Enabled = false;
                    tsmTAModify.Enabled = true;
                    break;
                case CountType.iTotalInstalled:
                case CountType.iTotalPullOut:
                case CountType.iTotalReplacement:
                case CountType.iTotalReprogramming:
                case CountType.iTotalServicing:
                case CountType.iTotalCancelled:
                    tsmPreviewFSR.Enabled = false;
                    tsmIRDelete.Enabled = false;
                    tsmTADispatch.Enabled = false;
                    tsmIRAllocate.Enabled = false;
                    tsmFSR.Enabled = true;
                    tsmTAModify.Enabled = false;
                    tsmExit.Enabled = false;
                    break;
                case CountType.iSearchServicing:
                    tsmPreviewFSR.Enabled = true;
                    tsmIRDelete.Enabled = false;
                    tsmTADispatch.Enabled = false;
                    tsmIRAllocate.Enabled = false;
                    tsmTAModify.Enabled = false;
                    break;
                default:
                    tsmPreviewFSR.Enabled = false;
                    tsmIRDelete.Enabled = false;
                    tsmTADispatch.Enabled = true;
                    tsmIRAllocate.Enabled = false;
                    tsmTAModify.Enabled = false;
                    break;
            }

            // Enable Preview Field Service Report PopUp
            tsmPreviewFSR.Enabled = false;
            switch (iCountType)
            {
                case CountType.iTDispatch:
                case CountType.iTInstallation:
                case CountType.iTServicing:
                case CountType.iTPullOut:
                case CountType.iTReplacement:
                case CountType.iTReprogramming:
                    tsmPreviewFSR.Enabled = true;
                    break;
            }
        }

        private void tsmIRDelete_Click(object sender, EventArgs e)
        {
            if (lvwSearch.Items.Count > 0)
            {
                iPopupMenuType = PopupMenuType.iDelete;
                SelectedMenu(iCountType);
            }
        }

        private void tsmIRAllocate_Click(object sender, EventArgs e)
        {
            if (lvwSearch.Items.Count > 0)
            {                
                iPopupMenuType = PopupMenuType.iModify;
                SelectedMenu(iCountType);
            }
        }

        private void tsmPreviewFSR_Click(object sender, EventArgs e)
        {
            if (lvwSearch.Items.Count > 0)
            {
                iPopupMenuType = PopupMenuType.iPreview;
                SelectedMenu(iCountType);
            }
        }

        private void tsmFSR_Click(object sender, EventArgs e)
        {
            if (lvwSearch.Items.Count > 0)
            {
                iPopupMenuType = PopupMenuType.iFSR;
                SelectedMenu(iCountType);
            }
        }

        private void tsmTAModify_Click(object sender, EventArgs e)
        {
            if (lvwSearch.Items.Count > 0)
            {   
                iPopupMenuType = PopupMenuType.iModify;
                SelectedMenu(iCountType);
                fModify = true;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private bool fConfirmReOpenService()
        {
            bool fConfirm = true;
            string sClientDetail = "";
            string sMerchantDetail = "";
            string sIRDetail = "";
            string sServiceDetail = "";
            string sStatusDetail = "";
            string sReasonDetail = "";

            Debug.WriteLine("frmTerminalAllocation_Load::" + "\n" +
                        "iCountType=" + iCountType.ToString() + "|" + "\n" +
                        "ClassIRStatus=" + clsSearch.ClassIRStatus + "|" + "\n" +
                        "ClassIRStatusDescription=" + clsSearch.ClassIRStatusDescription + "|" + "\n" +
                        "ClassStatus=" + clsSearch.ClassStatus + "|" + "\n" +
                        "ClassStatusDescription=" + clsSearch.ClassStatusDescription + "|" + "\n" +
                        "ClassServiceNo=" + clsSearch.ClassServiceNo + "|" + "\n" +
                        "ClassRequestNo=" + clsSearch.ClassRequestNo + "|" + "\n" +
                        "ClassIRIDNo=" + clsSearch.ClassIRIDNo + "|" + "\n" +
                        "ClassIRNo=" + clsSearch.ClassIRNo + "|" + "\n" +
                        "ClassTAIDNo=" + clsSearch.ClassTAIDNo + "|" + "\n" +
                        "ClassFEID=" + clsSearch.ClassFEID + "|" + "\n" +
                        "ClassFEName=" + clsSearch.ClassFEName + "|" + "\n" +
                        "ClassClientID=" + clsSearch.ClassClientID + "|" + "\n" +
                        "ClassClientName=" + clsSearch.ClassClientName + "|" + "\n" +
                        "ClassMerchantID=" + clsSearch.ClassMerchantID + "|" + "\n" +
                        "ClassMerchantName=" + clsSearch.ClassMerchantName + "|" + "\n" +
                        "ClassServiceProviderID=" + clsSearch.ClassServiceProviderID + "|" + "\n" +
                        "ClassServiceProviderName=" + clsSearch.ClassServiceProviderName + "|" + "\n" +
                        "ClassTerminalID=" + clsSearch.ClassTerminalID + "|" + "\n" +
                        "ClassTerminalSN=" + clsSearch.ClassTerminalSN + "|" + "\n" +
                        "ClassSIMID=" + clsSearch.ClassSIMID + "|" + "\n" + "\n" +
                        "ClassSIMSerialNo=" + clsSearch.ClassSIMSerialNo + "|" + "\n" +
                        "ClassDockID=" + clsSearch.ClassDockID + "|" + "\n" +
                        "ClassDockSN=" + clsSearch.ClassDockSN + "|" + "\n" +
                        "ClassReasonID=" + clsSearch.ClassReasonID + "|" + "\n" +
                        "ClassReasonDescription=" + clsSearch.ClassReasonDescription + "|" + "\n" +
                        "\n");

            sClientDetail =
                           "Client Name :" + clsSearch.ClassClientName + "\n" +
                           clsFunction.sLineSeparator + "\n";

            sMerchantDetail =
                            "Merchant Name: " + clsSearch.ClassMerchantName + "\n" +
                            "TID: " + clsSearch.ClassTID + "\n" +
                            "MID: " + clsSearch.ClassMID + "\n" +
                            clsFunction.sLineSeparator + "\n";

            sIRDetail =
                            "Request Date: " + clsSearch.ClassIRRequestDate + "\n" +
                            "Installation Date: " + clsSearch.ClassIRInstallationDate + "\n" +
                            "Request ID: " + clsSearch.ClassIRNo + "\n" +
                            clsFunction.sLineSeparator + "\n";

            sServiceDetail =
                            "Previous Service Date: " + clsSearch.ClassServiceDateTime + "\n" +
                            "Previous Service ID: " + clsSearch.ClassRequestNo + "\n" +                            
                            clsFunction.sLineSeparator + "\n";

            sReasonDetail =
                          "Reason: " + clsSearch.ClassReasonDescription + "\n" +
                          clsFunction.sLineSeparator + "\n";

            sStatusDetail =
                           "Status: " + clsSearch.ClassIRStatusDescription + "\n" +
                           clsFunction.sLineSeparator + "\n";
            
            switch (iCountType)
            {
                case CountType.iSearchNegativeInstallation:
                    frmServiceJobOrder.iSearchJobType = clsAPI.JobType.iInstallation;
                    break;
                case CountType.iSearchNegativePullOut:
                    frmServiceJobOrder.iSearchJobType = clsAPI.JobType.iPullOut;
                    break;
                case CountType.iSearchNegativeReplacement:
                    frmServiceJobOrder.iSearchJobType = clsAPI.JobType.iReplacement;
                    break;
                case CountType.iSearchNegativeReprogramming:
                    frmServiceJobOrder.iSearchJobType = clsAPI.JobType.iReprogramming;
                    break;
                case CountType.iSearchNegativeServicing:
                    frmServiceJobOrder.iSearchJobType = clsAPI.JobType.iServicing;
                    break;
            }

            if (MessageBox.Show("Do you really want to re-open " + clsSearch.ClassJobTypeDescription + " Service Request?" +
                                           "\n\n" +
                                            sClientDetail +
                                            sMerchantDetail +
                                            sIRDetail +
                                            sServiceDetail +
                                            sReasonDetail +
                                            sStatusDetail +                                            
                                           "\n\n" +
                                           "\n",
                                           "Proceed?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fConfirm = false;
            }

            return fConfirm;
        }

        private void GetSearchStringPerJobType()
        {            
            switch (iCountType)
            {                
                case CountType.iSearchNegativeInstallation:
                    clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC;
                    clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
                    break;
                case CountType.iSearchNegativePullOut:
                    clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_PULLOUT_DESC;
                    clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
                    break;
                case CountType.iSearchNegativeReplacement:
                    clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC;
                    clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
                    break;
                case CountType.iSearchNegativeReprogramming:
                    clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC;
                    clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
                    break;
                case CountType.iSearchNegativeServicing:
                    clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_SERVICING_DESC;
                    clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
                    break;
                case CountType.iTDispatch:
                    clsSearch.ClassStatus = clsGlobalVariables.STATUS_DISPATCH;
                    clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_DISPATCH_DESC;
                    clsSearch.ClassJobTypeDescription = clsFunction.sZero;
                    clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING_DESC;
                    break;
                case CountType.iTInstallation:
                case CountType.iTServicing:
                case CountType.iTPullOut:
                case CountType.iTReplacement:
                case CountType.iTReprogramming:

                    if (iCountType == CountType.iTInstallation)
                    {
                        clsSearch.ClassStatus = clsGlobalVariables.STATUS_ALLOCATED;
                        clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_ALLOCATED_DESC;
                    }
                    else if (iCountType == CountType.iTServicing)
                    {
                        clsSearch.ClassStatus = clsGlobalVariables.STATUS_SERVICING;
                        clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_SERVICING_DESC;
                    }
                    else if (iCountType == CountType.iTPullOut)
                    {
                        clsSearch.ClassStatus = clsGlobalVariables.STATUS_PULLED_OUT;
                        clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_PULLED_OUT_DESC;
                    }
                    else if (iCountType == CountType.iTReplacement)
                    {
                        clsSearch.ClassStatus = clsGlobalVariables.STATUS_REPLACEMENT;
                        clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_REPLACEMENT_DESC;
                    }
                    else if (iCountType == CountType.iTReprogramming)
                    {
                        clsSearch.ClassStatus = clsGlobalVariables.STATUS_REPROGRAMMED;
                        clsSearch.ClassStatusDescription = clsGlobalVariables.STATUS_REPROGRAMMED_DESC;
                    }
                    
                    //clsSearch.ClassJobTypeDescription = clsFunction.sZero;
                    clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
                    break;
            }
        }
        
        private void lvwSearch_Click(object sender, EventArgs e)
        {
            if (lvwSearch.Items.Count > 0 && dbFunction.isValidID(txtLineNo.Text))
            {
                string sTitle = "Installation Request Details";
                /*
                string sMessage =
                                Environment.NewLine +
                                "Line#: " + txtLineNo.Text + Environment.NewLine +
                                "Request Date: " + clsSearch.ClassIRRequestDate + Environment.NewLine +
                                "Installation Date: " + clsSearch.ClassIRInstallationDate + Environment.NewLine +
                                "Request ID: " + clsSearch.ClassIRNo + Environment.NewLine +
                                clsFunction.sLineSeparator + Environment.NewLine +
                                "Merchant Name: " + clsSearch.ClassMerchantName + Environment.NewLine +
                                "TID: " + clsSearch.ClassTID + Environment.NewLine +
                                "MID: " + clsSearch.ClassMID + Environment.NewLine +
                                clsFunction.sLineSeparator + Environment.NewLine +
                                "Client Name: " + clsSearch.ClassClientName + Environment.NewLine +
                                "Service Provider: " + clsSearch.ClassServiceProviderName + Environment.NewLine +
                                "Field Engineer: " + clsSearch.ClassFEName + Environment.NewLine +
                                clsFunction.sLineSeparator + Environment.NewLine +
                                "Terminal Serial No: " + clsSearch.ClassTerminalSN + Environment.NewLine +
                                "SIM Serial No: " + clsSearch.ClassSIMSerialNo + Environment.NewLine +
                                "Dock Serial No: " + clsSearch.ClassDockSN + Environment.NewLine +
                                clsFunction.sLineSeparator + Environment.NewLine +
                                "Status: " + clsSearch.ClassServiceStatusDescription + Environment.NewLine +
                                "Job Type: " + clsSearch.ClassJobTypeDescription + Environment.NewLine +
                                clsFunction.sLineSeparator + Environment.NewLine +
                                "Service Date/Time: " + clsSearch.ClassServiceDateTime + Environment.NewLine +
                                "Service No: " + clsSearch.ClassRequestNo + Environment.NewLine +
                                "Reason: " + clsSearch.ClassReasonDescription + Environment.NewLine +
                                clsFunction.sLineSeparator;
                */

                /*
                if (dbFunction.isValidID(txtLineNo.Text))
                {
                    string sMessage = clsFunction.sLineSeparator + Environment.NewLine + 
                                      dbFunction.GetListViewSelectedRow(lvwSearch, int.Parse(txtLineNo.Text)) + 
                                      clsFunction.sLineSeparator;

                    dbFunction.ShowToolTip(lvwSearch, txtLineNo.Text, sTitle, sMessage);
                } 
                */
            }
        }

        private void lvwSearch_MouseLeave(object sender, EventArgs e)
        {

        }

        private void btnPreviewReport_Click(object sender, EventArgs e)
        {
            if (lvwSearch.Items.Count > 0)
            {
                Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

                try
                {
                    clsSearch.ClassReportID = 9;
                    clsSearch.ClassReportDescription = sReportDescription;
                    clsSearch.ClassStatementType = sStatementType;                    
                    clsSearch.ClassStoredProcedureName = "spViewAdvanceIR";
                    clsSearch.ClassSearchBy = sSearchBy;
                    clsSearch.ClassSearchValue = sSearchValue;
                    clsSearch.ClassDateFrom = clsFunction.sDateDefault;
                    clsSearch.ClassDateTo = clsFunction.sDateFormat;                    
                    dbFunction.ProcessReport(clsSearch.ClassReportID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

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
        private int GetTotalPage(CountType iType)
        {
            Debug.WriteLine("--GetTotalPage--");
            Debug.WriteLine("iType=" + iType);

            int iCount = 0;
            int totalPage = 0;
            int iLimitSize = dbFunction.GetPageLimit();

            lblTotalCount.Text = clsFunction.sNull;
            
            Debug.WriteLine("iLimitSize=" + iLimitSize);

            iCount = 0;
            switch (iType)
            {
                case CountType.iTInstallationReq:

                    clsSearch.ClassHoldAdvanceSearchValue = clsGlobalVariables.STATUS_AVAILABLE + clsFunction.sPipe + clsGlobalVariables.STATUS_AVAILABLE_DESC;
                    dbAPI.GetViewCount("Search", "IR Status Count", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");

                    Debug.WriteLine("GetTotalPage::clsSearch.ClassHoldAdvanceSearchValue=" + clsSearch.ClassHoldAdvanceSearchValue);
                    break;
                case CountType.iTInstallationReqDaysPending:

                    clsSearch.ClassHoldAdvanceSearchValue = clsGlobalVariables.STATUS_AVAILABLE + clsFunction.sPipe + clsGlobalVariables.STATUS_AVAILABLE_DESC + clsFunction.sPipe + clsSystemSetting.ClassSystemNoOfDayPending;
                    dbAPI.GetViewCount("Search", "IR Days Pending Count", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");

                    Debug.WriteLine("GetTotalPage::clsSearch.ClassHoldAdvanceSearchValue=" + clsSearch.ClassHoldAdvanceSearchValue);
                    break;
                case CountType.iTInstallationReqOverDue:

                    clsSearch.ClassHoldAdvanceSearchValue = clsGlobalVariables.STATUS_AVAILABLE + clsFunction.sPipe + clsGlobalVariables.STATUS_AVAILABLE_DESC;
                    dbAPI.GetViewCount("Search", "IR OverDue Count", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");

                    Debug.WriteLine("GetTotalPage::clsSearch.ClassHoldAdvanceSearchValue=" + clsSearch.ClassHoldAdvanceSearchValue);
                    break;
                case CountType.iSearchIRInstalled:                
                case CountType.iTotalInstalled:
                case CountType.iTotalPullOut:
                case CountType.iTotalReplacement:
                case CountType.iTotalReprogramming:
                case CountType.iTotalServicing:
                case CountType.iTotalCancelled:

                    clsSearch.ClassHoldAdvanceSearchValue = 
                                            clsSearch.ClassServiceTypeID.ToString() + clsFunction.sPipe +
                                            clsSearch.ClassParticularID + clsFunction.sPipe +
                                            clsSearch.ClassTerminalTypeID.ToString() + clsFunction.sPipe +
                                            clsSearch.ClassTerminalModelID.ToString() + clsFunction.sPipe +
                                            clsSearch.ClassTerminalBrandID.ToString() + clsFunction.sPipe +
                                            clsSearch.ClassTerminalSN + clsFunction.sPipe +                                            
                                            (iIRStatus > 0 ? clsSearch.ClassIRStatus : iIRStatus) + clsFunction.sPipe +
                                            clsSearch.ClassIRNo + clsFunction.sPipe +
                                            clsSearch.ClassTID + clsFunction.sPipe +
                                            clsSearch.ClassMID + clsFunction.sPipe +
                                            clsSearch.ClassTerminalStatusType + clsFunction.sPipe +
                                            clsSearch.ClassReqDateFrom + clsFunction.sPipe +
                                            clsSearch.ClassReqDateTo + clsFunction.sPipe +
                                            clsSearch.ClassInstDateFrom + clsFunction.sPipe +
                                            clsSearch.ClassInstDateTo + clsFunction.sPipe +
                                            clsSearch.ClassTADateFrom + clsFunction.sPipe +
                                            clsSearch.ClassTADateTo + clsFunction.sPipe +
                                            clsSearch.ClassFSRDateFrom + clsFunction.sPipe +
                                            clsSearch.ClassFSRDateTo + clsFunction.sPipe +
                                            clsSearch.ClassIRImportDateFrom + clsFunction.sPipe +
                                            clsSearch.ClassIRImportDateTo + clsFunction.sPipe +
                                            clsSearch.ClassSIMSerialNo + clsFunction.sPipe +
                                            dbFunction.CheckAndSetNumericValue(txtSearch.Text) + clsFunction.sPipe +
                                            clsSearch.ClassStatus + clsFunction.sPipe +
                                            clsSearch.ClassStatusDescription + clsFunction.sPipe +
                                            clsSearch.ClassJobType + clsFunction.sPipe +
                                            clsSearch.ClassJobTypeStatusDescription + clsFunction.sPipe +
                                            clsSearch.ClassJobTypeDescription + clsFunction.sPipe +
                                            clsSearch.ClassActionMade + clsFunction.sPipe +                                            
                                            clsSearch.ClassIsClose + clsFunction.sPipe +
                                            isPrimary.ToString();

                    Debug.WriteLine("GetTotalPage::clsSearch.ClassHoldAdvanceSearchValue=" + clsSearch.ClassHoldAdvanceSearchValue);

                    dbAPI.GetViewCount("Search", "Advance IR", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");
                    break;
                case CountType.iTDispatch:
                    GetSearchStringPerJobType();
                    clsSearch.ClassHoldAdvanceSearchValue = clsSearch.ClassStatusDescription + clsFunction.sPipe +
                                            clsSearch.ClassJobTypeDescription + clsFunction.sPipe +
                                            clsSearch.ClassJobTypeStatusDescription + clsFunction.sPipe +
                                            clsFunction.sZero + clsFunction.sPipe +
                                            dbFunction.CheckAndSetNumericValue(txtSearch.Text);

                    Debug.WriteLine("GetTotalPage::clsSearch.ClassHoldAdvanceSearchValue=" + clsSearch.ClassHoldAdvanceSearchValue);

                    dbAPI.GetViewCount("Search", "Dispatch Servicing", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");
                    break;
                case CountType.iTInstallation:
                case CountType.iTServicing:
                case CountType.iTPullOut:
                case CountType.iTReplacement:
                case CountType.iTReprogramming:
                    GetSearchStringPerJobType();
                    clsSearch.ClassHoldAdvanceSearchValue = clsSearch.ClassStatusDescription + clsFunction.sPipe +
                                                        clsSearch.ClassJobTypeDescription + clsFunction.sPipe +
                                                        clsSearch.ClassJobTypeStatusDescription + clsFunction.sPipe +
                                                        clsFunction.sZero + clsFunction.sPipe +
                                                        dbFunction.CheckAndSetNumericValue(txtSearch.Text);

                    Debug.WriteLine("GetTotalPage::clsSearch.ClassHoldAdvanceSearchValue=" + clsSearch.ClassHoldAdvanceSearchValue);

                    dbAPI.GetViewCount("Search", "Allocated Servicing", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");
                    break;
                case CountType.iSearchFSR:
                    clsSearch.ClassHoldAdvanceSearchValue = clsFunction.sNull;

                    dbAPI.GetViewCount("Search", "FSR", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");
                    break;

                case CountType.iSearchServicing:
                    clsSearch.ClassHoldAdvanceSearchValue = clsFunction.sNull;

                    dbAPI.GetViewCount("Search", "Servicing List", clsSearch.ClassHoldAdvanceSearchValue, "Get Count");
                    break;
            }

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
        private void ProcessPage(CountType iType)
        {
            InitPage(int.Parse(clsFunction.sOne), GetTotalPage(iType));
        }

        private void btnFirstPage_Click(object sender, EventArgs e)
        {
            clsSearch.ClassCurrentPage = int.Parse(clsFunction.sOne);
            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadIR("View", "", "");
        }

        private void btnLastPage_Click(object sender, EventArgs e)
        {
            clsSearch.ClassCurrentPage = clsSearch.ClassTotalPage;
            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadIR("View", "", "");
        }

        private void btnPreviousPage_Click(object sender, EventArgs e)
        {
            if (clsSearch.ClassCurrentPage > int.Parse(clsFunction.sOne))
                clsSearch.ClassCurrentPage--;

            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadIR("View", "", "");
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (clsSearch.ClassCurrentPage < clsSearch.ClassTotalPage)
                clsSearch.ClassCurrentPage++;

            InitPage(clsSearch.ClassCurrentPage, clsSearch.ClassTotalPage);

            LoadIR("View", "", "");
        }

        private void ResetSearchField()
        {            
            clsSearch.ClassTerminalSN = clsFunction.sZero;
            clsSearch.ClassIRStatus = clsFunction.iZero;
            clsSearch.ClassIRNo = clsFunction.sZero;
            clsSearch.ClassTID = clsFunction.sZero;
            clsSearch.ClassMID = clsFunction.sZero;         
            clsSearch.ClassSIMSerialNo = clsFunction.sZero;            
        }

        private void SetSearchHeader(CountType iType)
        {
            switch (iType)
            {
                case CountType.iSearchFSR:
                    lblSearchString.Text = "TYPE MERCHANT NAME, TID, MID, TERMINAL SERIAL NO., SIM SERIAL NO., REQUEST ID";
                    break;
                default:
                    lblSearchString.Text = "TYPE MERCHANT NAME, TID, MID, CLIENT NAME, TERMINAL SERIAL NO., SIM SERIAL NO., DOCK SERIAL NO., REQUEST ID";
                    break;
            }
        }

        private void InitHeaderBackColor(CountType iType)
        {
            switch (iType)
            {
                case CountType.iSearchFSR:
                case CountType.iSearchServicing:
                case CountType.iSearchTA:
                    pnlHeader.BackColor = Color.Crimson;
                    break;
                default:
                    pnlHeader.BackColor = Color.DarkGreen;
                    break;
            }
        }
    }
}
