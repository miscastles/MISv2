using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.IO;
using System.Diagnostics;
using System.Threading;
using MIS.Report;
using MIS.AppMainActivity;
using static MIS.AppMainActivity.AppReports;
using static MIS.Function.AppUtilities;
using OfficeOpenXml;
using MIS.Function;

namespace MIS
{
    public partial class frmReportViewer : Form
    {
        public static string sReportStatementType;
        public static string sReportSearchBy;
        public static string sReportSearchValue;
        public static string sStoredProcedureName;
        public static bool sReportTemp;
        public static string sReportToView;
        public static int sReportID;
        public static string sClientName;
     

        private clsAPI dbAPI;
        private clsClientConnection dbConnect;
        private clsINI dbSetting;
        private clsFunction dbFunction;
        private clsFile dbFile;
        private clsExcelOpenXmlExporter dbExcelOpenXmlExporter;

        // Data Holder - Billing
        public DataTable MccNetMatrixDetails;
        public DataTable HelpdeskDt { get; set; }

        public frmReportViewer()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwList, true);
        }

        private void frmReportViewer_Load(object sender, EventArgs e)
        {
            bool fConnected = false;

            Debug.WriteLine("--frmReportViewer_Load--");

            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            dbSetting = new clsINI();
            dbSetting.InitDatabaseSetting();

            dbConnect = new clsClientConnection();
            fConnected = dbConnect.ConnectDBServerClient();
            
            dbFile = new clsFile();

            dbExcelOpenXmlExporter = new clsExcelOpenXmlExporter();

            Debug.WriteLine("sReportID="+ sReportID);
            Debug.WriteLine("clsSearch.isMDRBreakdown=" + clsSearch.isMDRBreakdown);

            if (!fConnected)
            {
                MessageBox.Show("Unable to connect to database. Contact administrator", "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Application.Exit();
            }

            ucStatusDisplay.SetStatus("", Enums.StatusType.Init);
            Task.Delay(500); // Asynchronously wait without blocking UI

            ucStatusDisplay.SetStatus($"Preparing report {clsSearch.ClassReportDescription}", Enums.StatusType.Processing);
            Task.Delay(500); // Asynchronously wait without blocking UI

            warmUpReport(); // warm-up report

            // Report Name
            this.Text = "Report Preview " + "[ " + clsReport.ClassReportDesc + " ]";
            
            try
            {                
                switch (sReportID)
                {
                    case 0:
                        TAReport();
                        break;

                    case 1:
                        InventoryTerminalDetailReport();
                        break;

                    case 2:
                        IRReport();
                        break;

                    case 3: // SIM DETAIL INVENTORY REPORT
                        InventorySIMDetailReport();
                        break;

                    case 4: // FSR REPORT (OPERATION)
                        FSRReport();
                        break;

                    case 5: // FSR REPORT (FIELD ENGINEER)
                        FieldServiceReport();                            
                        break;

                    case 6: // ERM BILLING REPORT
                        if (clsSearch.isMDRBreakdown)
                            ERMBillingWithMDR();
                        else
                            ERMBilling();
                        break;

                    case 7:
                        FSRService();
                        break;

                    case 8: // SERVICE REQUEST SUMMARY REPORT
                        InstallationSummaryReport();
                        break;

                    case 9: // SERVICE REQUEST DETAIL REPORT
                        InstallationReport();
                        break;

                    case 10: // SERVICING REQUEST REPORT
                        ServicingDetailTempReport();
                        break;

                    case 11: // SERVICE HISTORY REPORT                     
                        //ServiceHistoryReport();
                        ServiceHistoryDetailReport();
                        break;

                    case 12: // QRCODE REPORT
                        QRCodeReport();
                        break;

                    case 13: // TIMESHEET REPORT
                    case 21: // MISSING/INCOMPLETE TIMESHEET REPORT
                        TimeSheetReport();
                        break;

                    case 14: // WORK ARRANGEMENT REPORT
                        WorkArrangementReport();
                        break;

                    case 15: // HOLIDAY REPORT
                        HolidayReport();
                        break;

                    case 16: // LEAVE APPLICATION REPORT
                        LeaveApplicationReport();
                        break;

                    case 17: // LEAVE ASSIGNMENT REPORT
                        LeaveAssignmentReport();
                        break;

                    case 18: // CLIENT FSR REPORT
                        FSRClientReport();
                        break;

                    case 19: // TERMINAL SUMMARY(LOCATION) INVENTORY REPORT
                        InventoryTerminalSummaryReport();
                        break;

                    case 20: // TERMINAL ALLOCATION(SERVICING) REPORT
                        InventorySIMSummaryReport();
                        break;

                    case 23: // OPERATION FSR REPORT
                        FSROperationReport();
                        break;

                    case 25: // TERMINAL RELEASED SUMMARY REPORT
                        ReleaseTerminalSummaryReport();
                        break;

                    case 26: // TERMINAL RELEASED DETAIL REPORT
                        ReleaseTerminalDetailReport();
                        break;

                    case 27: // SIM RELEASED DETAIL REPORT
                        ReleaseSIMDetailReport();
                        break;

                    case 28: // SIM RELEASED SUMMARY REPORT
                        ReleaseSIMSummaryReport();
                        break;

                    case 29: // POS RENTAL REPORT
                        POSRentalReport();
                        break;

                    case 30: // TERMINAL IMPORT REPORT
                        TerminalImportReport();
                        break;

                    // ROCKY - SIM IMPORT: ADD SIM IMPORT REPORT
                    case 31: // SIM IMPORT REPORT
                        SIMImportReport();
                        break;

                    case 32: // TERMINAL SUMMARY(TYPE/MODEL) INVENTORY REPORT
                        InventoryTerminalTypeModelSummaryReport();
                        break;

                    case 33: // SIM SUMMARY(TELCO) INVENTORY REPORT
                        InventorySIMTelcoSummaryReport();
                        break;

                    // ROCKY - PARTICULAR: ADD PARTICULAR DETAILS REPORT 
                    case 34: // PARTICULAR DETAILS REPORT
                        ParticularDetailsReport();
                        break;

                    // ROCKY - PARTICULAR: ADD PARTICULAR REQUIREMENTS DETAIL REPORT 
                    case 35: // PARTICULAR DETAILS REPORT
                        ParticularRequirementsReport();
                        break;

                    // ROCKY - BILLING: ADD SECURITY BANK BILLING REPORT
                    case 36:
                        SecurityBankBillingReport();
                        break;

                    // ROCKY - BILLING: ADD METROBANK SERVICE REPORT
                    case 37:
                        // Servicing Summary
                        Summary.Services(myViewer);
                        break;

                    case 38:
                        // Net Matrix Summary
                        Summary.Leasing(myViewer);
                        break;

                    case 39:
                        // Weepay Inventory Summary
                        Summary.WeePayInventory(myViewer);
                        break;

                    case 40:
                        // Weepay Sim Summary
                        Summary.WeePaySimDetails(myViewer);
                        break;
                    
                    case 42: // eDiagnostic
                        eDiagnostic();
                        break;

                    case 43: // SERVICE SUMMARY REPORT(TYPE)
                        ServiceSummaryReport();
                        break;

                    case 44: // SERVICE DETAIL REPORT(TYPE)
                        ServiceDetailReport();
                        break;

                    case 45: // INSTALLATION SUMMARY REPORT
                        ServiceInstallationSummaryReport();
                        break;

                    case 46: // INSTALLATION DETAIL REPORT
                        ServiceInstallationDetailReport();
                        break;

                    case 47: // ACTIVE TERMINAL REPORT
                        ActiveTerminalReport();
                        break;

                    case 48: // ACTIVE POS SUMMARY REPORT
                        ActiveSIMReport();
                        break;

                    case 49: // ACTIVE POS DETAIL REPORT
                        ActiveTerminalDetailReport();
                        break;

                    case 52: // MONTHLY COMPLETED SERVICE REPORT
                        MonthlyServiceCountReport();
                        break;

                    case 53: // SERVICE INSTALLATION REPORT
                        ServiceInstallationReport();
                        break;

                    case 54: // SERVICE MAINTENANCE REPORT
                        ServiceMaintenanceReport();
                        break;

                    case 55: // SERVICE PULLOUT REPORT
                        ServicePullOutReport();
                        break;

                    case 56: // UNCLOSED TICKET REPORT
                        UnclosedServiceTicketReport();
                        break;

                    case 57: // HELPDESK DETAIL REPORT
                        HelpdeskSummaryReport();
                        break;

                    case 58:
                        HelpdeskKPIReport();
                        break;

                    // INVOICES
                    case 1001:
                        // Servicing Invoice
                            Invoice.Services(myViewer);
                        break;

                    case 1002:
                        // Net Matrix Invoice
                            Invoice.Leasing(myViewer);
                        break;

                    case 1003:
                        // TLE Invoice
                          Invoice.Tle(myViewer); 
                        break;

                    case 1004:
                        // Weepay Sim Invoice
                        Invoice.WeePaySimInvoice(myViewer);
                        break;

                    case 1005:
                        // Warehouse Invoice
                        Invoice.WareHouse(myViewer);
                        break;
      
                    default:
                        dbFunction.SetMessageBox("Report ID " + dbFunction.AddBracketStartEnd(sReportID.ToString()) + " does not defined.", "Report Preview", clsFunction.IconType.iError);
                        break;
                }
                
            }
            catch (Exception ex)
            {
                dbFile.WriteAPILog(2, "ReportPreview, ReportID=" + clsSearch.ClassReportID + "\n" + "Error " + ex.Message);
                MessageBox.Show(ex.Message, "Report ID " + dbFunction.AddBracketStartEnd(clsSearch.ClassReportID.ToString()) + ",Report " + dbFunction.AddBracketStartEnd(clsSearch.ClassReportDescription), MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }

            initButton();

            initTabButton(this, e);
            
            Cursor.Current = Cursors.Default;
        }

        private void TAReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptFindTA.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptFindTA rptViewer = new rptFindTA();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SeTAReceiptUser(rptViewer);
                SeTAReceiptDateFromTo(rptViewer);

                SetReceiptDateRange(rptViewer);
                SetReceiptClient(rptViewer);
                //SetReceiptServiceTypeDesc(rptViewer);
                SetReceiptServiceStatusDesc(rptViewer);
                //SetReceiptServiceResultDesc(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void SeTAReceiptDateFromTo(ReportClass rptViewer)
        {
            // Set report DateFromTo
            TextObject objDateFromTo;

            if (!fPrintDateFromTo()) return;

            if (rptViewer.ReportDefinition.ReportObjects["txtDateFromTo"] != null)
            {
                objDateFromTo = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtDateFromTo"];
                objDateFromTo.Text = "TA Date: " + DateTime.Parse(clsSearch.ClassTADateFrom).ToString("MM-dd-yyyy") + " to " + DateTime.Parse(clsSearch.ClassTADateTo).ToString("MM-dd-yyyy");
            }
        }

        private void SeTAReceiptUser(ReportClass rptViewer)
        {
            // Set report User
            TextObject objUserFullName;
            
            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtUserFullName"] != null)
            {
                objUserFullName = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtUserFullName"];
                objUserFullName.Text = "Printed By: " + clsUser.ClassUserFullName;
            }
        }

        private void TerminalWithTAReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptFindTerminalWithTA.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptFindTerminalWithTA rptViewer = new rptFindTerminalWithTA();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SeTerminalWithTAReceiptUser(rptViewer);

                // Check for valid date/time format
                if (clsSearch.ClassTADateTo.CompareTo(clsFunction.sDateFormat) != 0)
                {
                    SeTerminalWithTAReceiptDateFromTo(rptViewer);
                }
                    
                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }            
        }

        private void SeTerminalWithTAReceiptDateFromTo(ReportClass rptViewer)
        {
            // Set report DateFromTo
            TextObject objDateFromTo;

            if (!fPrintDateFromTo()) return;

            if (rptViewer.ReportDefinition.ReportObjects["txtDateFromTo"] != null)
            {
                objDateFromTo = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtDateFromTo"];
                objDateFromTo.Text = "TA Date: " + DateTime.Parse(clsSearch.ClassTADateFrom).ToString("MM-dd-yyyy") + " to " + DateTime.Parse(clsSearch.ClassTADateTo).ToString("MM-dd-yyyy");
            }
        }

        private void SeTerminalWithTAReceiptUser(ReportClass rptViewer)
        {
            // Set report User
            TextObject objUserFullName;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtUserFullName"] != null)
            {
                objUserFullName = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtUserFullName"];
                objUserFullName.Text = "Printed By: " + clsUser.ClassUserFullName;
            }
        }

        private void IRReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptFindIR.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptFindIR rptViewer = new rptFindIR();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SeIRReceiptUser(rptViewer);
                SeIRReceiptDateFromTo(rptViewer);

                SetReceiptDateRange(rptViewer);
                SetReceiptClient(rptViewer);
                SetReceiptServiceTypeDesc(rptViewer);
                SetReceiptServiceStatusDesc(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void SeIRReceiptDateFromTo(ReportClass rptViewer)
        {
            // Set report DateFromTo
            TextObject objDateFromTo;

            if (!fPrintDateFromTo()) return;

            if (rptViewer.ReportDefinition.ReportObjects["txtDateFromTo"] != null)
            {
                objDateFromTo = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtDateFromTo"];

                if (clsSearch.ClassReqDateFrom.CompareTo(clsFunction.sDateFormat) == 0 && clsSearch.ClassReqDateTo.CompareTo(clsFunction.sDateFormat) == 0)
                {
                    objDateFromTo.Text = "Installation Date: " + DateTime.Parse(clsSearch.ClassInstDateFrom).ToString("MM-dd-yyyy") + " to " + DateTime.Parse(clsSearch.ClassInstDateTo).ToString("MM-dd-yyyy");
                }

                if (clsSearch.ClassInstDateFrom.CompareTo(clsFunction.sDateFormat) == 0 && clsSearch.ClassInstDateTo.CompareTo(clsFunction.sDateFormat) == 0)
                {
                    objDateFromTo.Text = "Request Date: " + DateTime.Parse(clsSearch.ClassReqDateFrom).ToString("MM-dd-yyyy") + " to " + DateTime.Parse(clsSearch.ClassReqDateTo).ToString("MM-dd-yyyy");
                }

            }
        }

        private void SetReceiptDateRange(ReportClass rpt)
        {
            // Set report User
            TextObject obj;

            // Set User Name                
            if (rpt.ReportDefinition.ReportObjects["txtDateRange"] != null)
            {
                obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtDateRange"];

                if ((clsSearch.ClassDateFrom.Equals(clsFunction.sDateFormat)) && (clsSearch.ClassDateTo.Equals(clsFunction.sDateFormat)))
                    obj.Text = clsFunction.sDefaultSelect;
                else
                    obj.Text = clsSearch.ClassDateFrom + " To " + clsSearch.ClassDateTo;
            }

        }

        private void SetReceiptClient(ReportClass rpt)
        {
            // Set report User
            TextObject obj;

            // Set User Name                
            if (rpt.ReportDefinition.ReportObjects["txtClient"] != null)
            {
                obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtClient"];
                obj.Text = (clsSearch.ClassClientName != null ? clsSearch.ClassClientName : clsFunction.sDefaultSelect);
            }

        }

        private void SetReceiptServiceTypeDesc(ReportClass rpt)
        {
            // Set report User
            TextObject obj;

            // Set User Name                
            if (rpt.ReportDefinition.ReportObjects["txtServiceType"] != null)
            {
                obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtServiceType"];
                obj.Text = (clsSearch.ClassServiceTypeDescription != null ? clsSearch.ClassServiceTypeDescription : clsFunction.sDefaultSelect) ;
            }

        }

        private void SetReceiptServiceStatusDesc(ReportClass rpt)
        {
            // Set report User
            TextObject obj;

            // Set User Name                
            if (rpt.ReportDefinition.ReportObjects["txtServiceStatus"] != null)
            {
                obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtServiceStatus"];
                obj.Text = (clsSearch.ClassServiceStatusDescription != null ? clsSearch.ClassServiceStatusDescription : clsFunction.sDefaultSelect);
            }

        }

        private void SetReceiptServiceCount(ReportClass rpt)
        {
            // Set report User
            TextObject obj;

            // Set User Name                
            if (rpt.ReportDefinition.ReportObjects["txtServiceCount"] != null)
            {
                obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtServiceCount"];
                obj.Text = clsSearch.ClassServiceCount;
            }

        }

        private void SetReceiptServiceResultDesc(ReportClass rpt)
        {
            // Set report User
            TextObject obj;

            // Set User Name                
            if (rpt.ReportDefinition.ReportObjects["txtServiceResult"] != null)
            {
                obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtServiceResult"];
                obj.Text = clsSearch.ClassActionMade;
            }

        }

        private void SeIRReceiptUser(ReportClass rptViewer)
        {
            // Set report User
            TextObject objUserFullName;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtUserFullName"] != null)
            {
                objUserFullName = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtUserFullName"];
                objUserFullName.Text = "Printed By: " + clsUser.ClassUserFullName;
            }
        }

        private void SIMReport()
        {
            string ReportPath = "";
            string reportFullPath = "";
            bool fValid = false;

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptFindSIM.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptFindSIM rptViewer = new rptFindSIM();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);

                if (dsReport.Tables.Count > 0)
                {
                    if (dsReport.Tables[0].Rows.Count > 0)
                    {
                        fValid = true;                        
                    }                    
                }

                if (fValid)
                {
                    rptViewer.SetDataSource(dsReport.Tables[0]);
                }
                else
                {
                    MessageBox.Show("No record found.", "SIM Report", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                //SeSIMReceiptDateFromTo(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view*

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void SeSIMReceiptDateFromTo(ReportClass rptViewer)
        {
            // Set report DateFromTo
            TextObject objDateFromTo;

            if (!fPrintDateFromTo()) return;
            
            if (rptViewer.ReportDefinition.ReportObjects["txtDateFromTo"] != null)
            {
                objDateFromTo = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtDateFromTo"];
                objDateFromTo.Text = "Request Date: " + DateTime.Parse(clsSearch.ClassReqDateFrom).ToString("MM-dd-yyyy") + " to " + DateTime.Parse(clsSearch.ClassReqDateTo).ToString("MM-dd-yyyy");
            }
        }

        private void SeSIMReceiptUser(ReportClass rptViewer)
        {
            // Set report User
            TextObject objUserFullName;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtUserFullName"] != null)
            {
                objUserFullName = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtUserFullName"];
                objUserFullName.Text = "Printed By: " + clsUser.ClassUserFullName;
            }
        }

        private void FSRReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptFindFSR.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptFindFSR rptViewer = new rptFindFSR();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SeIRReceiptUser(rptViewer);
                //SeIRReceiptDateFromTo(rptViewer);

                SetReceiptDateRange(rptViewer);
                SetReceiptClient(rptViewer);
                SetReceiptServiceTypeDesc(rptViewer);               
                SetReceiptServiceStatusDesc(rptViewer);
                SetReceiptServiceResultDesc(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            
        }
        
        private void FieldServiceReport()
        {
            string ReportPath = "";
            string reportFullPath = "";
            
            ReportPath = GetReportPath();        
            reportFullPath = Path.Combine(ReportPath, "rptEFSRv2.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

           try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptEFSRv2 rptViewer = new rptEFSRv2();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;
                
                rptViewer.Load(reportFullPath);
                
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);

                // Components
                SetReceiptCurrentComponent(rptViewer);
                SetReceiptReplaceComponent(rptViewer);

                if (clsSearch.ClassIsExportToPDF)
                {
                    this.Close();
                    ReportExport(rptViewer, clsSearch.ClassServiceRequestID); // Export
                    clsSearch.ClassIsExportToPDF = false;
                }
                else
                {
                    myViewer.ReportSource = rptViewer;                    
                    myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view
                }
                
            }            
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
            
        }

        private void SetFSRWithTAReceiptUser(ReportClass rptViewer)
        {
            // Set report User
            TextObject obj;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtUserFullName"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtUserFullName"];
                obj.Text = clsUser.ClassUserFullName;
            }
        }

        private string GetReportPath()
        {
            string ReportPath = "";
            bool fDevelopment;
            
            fDevelopment = false;

            if (fDevelopment)
                ReportPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Report";
            else
                ReportPath = "C:\\CASTLESTECH_MIS\\REPORTS\\";

            return ReportPath;
        }

        private bool fPrintDateFromTo()
         {
            bool fPrint = true;

            if ((clsSearch.ClassReqDateFrom.CompareTo(clsFunction.sDateFormat) == 0) &&
               (clsSearch.ClassReqDateTo.CompareTo(clsFunction.sDateFormat) == 0) &&
               (clsSearch.ClassInstDateFrom.CompareTo(clsFunction.sDateFormat) == 0) &&
               (clsSearch.ClassInstDateTo.CompareTo(clsFunction.sDateFormat) == 0) &&
               (clsSearch.ClassTADateFrom.CompareTo(clsFunction.sDateFormat) == 0) &&
               (clsSearch.ClassTADateTo.CompareTo(clsFunction.sDateFormat) == 0) &&
               (clsSearch.ClassFSRDateFrom.CompareTo(clsFunction.sDateFormat) == 0) &&
               (clsSearch.ClassFSRDateTo.CompareTo(clsFunction.sDateFormat) == 0))
            {
                fPrint = false;
            }
            
            return fPrint;
        }

        private void ERMBilling()
        {
            string ReportPath = "";
            string reportFullPath = "";

            Cursor.Current = Cursors.WaitCursor;

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptERM.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptERM rptViewer = new rptERM();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SeERMReceiptUser(rptViewer);
                SetERMReceiptDateFromTo(rptViewer);

                SetERMReceiptClientName(rptViewer);
                SetERMReceiptTCount(rptViewer);
                SetERMReceiptTAmount(rptViewer);
                SetERMReceiptReportType(rptViewer);
                SetERMReceiptDateRange(rptViewer);
                SetReceiptRecurring(rptViewer);

                if (clsSearch.isMDRBreakdown)
                    SetReceiptMDRBreakDownTypeA(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            Cursor.Current = Cursors.Default;
        }

        private void ERMBillingWithMDR()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            
            reportFullPath = Path.Combine(ReportPath, "rptERMWithMDR.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptERMWithMDR rptViewer = new rptERMWithMDR();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SeERMReceiptUser(rptViewer);
                SetERMReceiptDateFromTo(rptViewer);

                SetERMReceiptClientName(rptViewer);
                SetERMReceiptTCount(rptViewer);
                SetERMReceiptTAmount(rptViewer);
                SetERMReceiptReportType(rptViewer);
                SetERMReceiptDateRange(rptViewer);
                SetReceiptRecurring(rptViewer);

                if (clsSearch.isMDRBreakdown)
                    SetReceiptMDRBreakDownTypeA(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void SetERMReceiptDateFromTo(ReportClass rptViewer)
        {
            // Set report DateFromTo
            TextObject obj;
            
            if (rptViewer.ReportDefinition.ReportObjects["txtDateFromTo"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtDateFromTo"];
                obj.Text = "Filter Date: " + clsSearch.ClassDateFrom + " to " + clsSearch.ClassDateTo;
            }
        }

        private void SeERMReceiptUser(ReportClass rptViewer)
        {
            // Set report User
            TextObject obj;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtUserFullName"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtUserFullName"];
                obj.Text = "Printed By: " + clsUser.ClassUserFullName;
            }
        }

        private void SetERMReceiptClientName(ReportClass rptViewer)
        {
            // Set report ClientName
            TextObject obj;

            // Set Client
            if (rptViewer.ReportDefinition.ReportObjects["txtClient"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtClient"];
                obj.Text = sClientName + " " + "(" + (clsSearch.ClassBillable ? "BILLABLE" : "NON-BILLABLE") + ")";
            }
        }

        private void SetERMReceiptTCount(ReportClass rptViewer)
        {
            // Set report TCount
            TextObject obj;
            string sTCount = (clsSearch.ClassBillable ? clsSearch.ClassTCount.ToString() : "0");

            // Set TCount Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtTCount"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtTCount"];
                obj.Text = sTCount;
            }
        }

        private void SetERMReceiptTAmount(ReportClass rptViewer)
        {
            // Set report TAmount
            TextObject obj;
            string sTAmount = (clsSearch.ClassBillable ? clsSearch.ClassTAmount.ToString() : "0.00");

            // Set TAmount Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtTAmount"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtTAmount"];
                obj.Text = sTAmount;
            }
        }

        private void SetERMReceiptReportType(ReportClass rptViewer)
        {
            // Set report ClientName
            TextObject obj;

            // Set Client
            if (rptViewer.ReportDefinition.ReportObjects["txtReportType"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtReportType"];
                obj.Text = clsSearch.ClassReportType.ToUpper();
            }
        }

        private void SetERMReceiptDateRange(ReportClass rptViewer)
        {
            // Set report ClientName
            TextObject obj;

            // Set Client
            if (rptViewer.ReportDefinition.ReportObjects["txtDateRange"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtDateRange"];
                //obj.Text = clsSearch.ClassDateFrom + " - " + clsSearch.ClassDateTo;
                obj.Text = clsSearch.ClassDateFromTo;
            }
        }
        
        private void FSRService()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptFSRTemp.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptFSRTemp rptViewer = new rptFSRTemp();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetFSRReceiptDateFromTo(rptViewer);
                SetFSRReceiptUser(rptViewer);
                SetFSRReceiptServiceType(rptViewer);
                SetFSRReceiptReason(rptViewer);
                SetFSRReceiptTCount(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        
        private void SetFSRReceiptUser(ReportClass rptViewer)
        {
            // Set report User
            TextObject obj;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtUserFullName"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtUserFullName"];
                obj.Text = "Printed By: " + clsUser.ClassUserFullName;
            }
        }

        private void SetFSRReceiptServiceType(ReportClass rptViewer)
        {
            // Set report TCount
            TextObject obj;

            // Set TCount Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtServiceType"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtServiceType"];
                obj.Text = clsSearch.ClassServiceTypeDescription + clsFunction.sPadSpace + "(" + clsSearch.ClassServiceTypeCode + ")";
            }
        }

        private void SetFSRReceiptReason(ReportClass rptViewer)
        {
            // Set report TCount
            TextObject obj;

            // Set TCount Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtReason"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtReason"];
                obj.Text = clsSearch.ClassReasonDescription;
            }
        }

        private void SetFSRReceiptTCount(ReportClass rptViewer)
        {
            // Set report TCount
            TextObject obj;

            // Set TCount Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtTCount"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtTCount"];
                obj.Text = clsSearch.ClassTCount;
            }
        }

        private void SetFSRReceiptDateFromTo(ReportClass rptViewer)
        {
            // Set report DateFromTo
            TextObject objDateFromTo;
            
            if (rptViewer.ReportDefinition.ReportObjects["txtDateFromTo"] != null)
            {
                objDateFromTo = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtDateFromTo"];
                objDateFromTo.Text = "Filter Date: " + clsSearch.ClassDateFrom + " to " + clsSearch.ClassDateTo;
            }
        }

        private void FSRTempDetail()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptFSRTempDetail.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptFSRTempDetail rptViewer = new rptFSRTempDetail();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetFSRTemDetailReceiptDateFromTo(rptViewer);
                SetFSRTempDetailReceiptUser(rptViewer);
                
                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void SetFSRTemDetailReceiptDateFromTo(ReportClass rptViewer)
        {
            // Set report DateFromTo
            TextObject objDateFromTo;

            if (rptViewer.ReportDefinition.ReportObjects["txtDateFromTo"] != null)
            {
                objDateFromTo = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtDateFromTo"];
                objDateFromTo.Text = "Filter Date: " + clsSearch.ClassDateFrom + " to " + clsSearch.ClassDateTo;
            }
        }

        private void SetFSRTempDetailReceiptUser(ReportClass rptViewer)
        {
            // Set report User
            TextObject obj;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtUserFullName"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtUserFullName"];
                obj.Text = "Printed By: " + clsUser.ClassUserFullName;
            }
        }

        private void IRDetailReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptFindCount.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptFindCount rptViewer = new rptFindCount();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetIRDetailReceiptUser(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void SetIRDetailReceiptUser(ReportClass rptViewer)
        {
            // Set report User
            TextObject obj;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtUserFullName"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtUserFullName"];
                obj.Text = "Printed By: " + clsUser.ClassUserFullName;
            }
        }

        private void ServicingDetailTempReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptFindServicingTemp.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptFindServicingTemp rptViewer = new rptFindServicingTemp();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetServicingDetailTempReceiptDateFromTo(rptViewer);
                SetServicingDetailTempReceiptUser(rptViewer);
                SetServicingDetailTempServiceType(rptViewer);
                SetServicingDetailTempRegionDetail(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void SetServicingDetailTempReceiptDateFromTo(ReportClass rptViewer)
        {
            // Set report DateFromTo
            TextObject objDateFromTo;

            if (rptViewer.ReportDefinition.ReportObjects["txtDateFromTo"] != null)
            {
                objDateFromTo = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtDateFromTo"];
                objDateFromTo.Text = "Filter Date: " + clsSearch.ClassDateFrom + " to " + clsSearch.ClassDateTo;
            }
        }

        private void SetServicingDetailTempReceiptUser(ReportClass rptViewer)
        {
            // Set report User
            TextObject obj;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtUserFullName"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtUserFullName"];
                obj.Text = "Printed By: " + clsUser.ClassUserFullName;
            }
        }

        private void SetServicingDetailTempServiceType(ReportClass rptViewer)
        {
            // Set report TCount
            TextObject obj;

            // Set TCount Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtServiceType"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtServiceType"];
                obj.Text = clsSearch.ClassJobTypeDescription;
            }
        }

        private void SetServicingDetailTempRegionDetail(ReportClass rptViewer)
        {
            // Set report TCount
            TextObject obj;

            // Set TCount Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtRegionDetail"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtRegionDetail"];
                obj.Text = clsSearch.ClassRegion + " / " + clsSearch.ClassProvince;
            }
        }

        private void SetReceiptRecurring(ReportClass rpt)
        {
            // Set report User
            TextObject obj;

            // Set User Name                
            if (rpt.ReportDefinition.ReportObjects["txtTRecurring"] != null)
            {
                obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtTRecurring"];
                obj.Text = clsSearch.ClassTRecurring;
            }

        }

        private void ServiceHistoryReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptServiceHistory.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptServiceHistory rptViewer = new rptServiceHistory();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                SetReceiptDateFromTo(rptViewer);
                
                SetReceiptClient(rptViewer);
                SetReceiptMerchant(rptViewer);
                SetReceiptContactPerson(rptViewer);
                SetReceiptContactNo(rptViewer);
                SetReceiptTID(rptViewer);
                SetReceiptMID(rptViewer);
                SetReceiptIRNo(rptViewer);
                SetReceiptAddress(rptViewer);
                SetReceiptRegion(rptViewer);
                SetReceiptPrimaryNum(rptViewer);
                SetReceiptSecondaryNum(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void SetReceiptUser(ReportClass rptViewer)
        {
            // Set report User
            TextObject obj;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtUserFullName"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtUserFullName"];
                obj.Text = "Printed By: " + clsUser.ClassUserFullName;
            }
        }

        private void SetReceiptUserNoPrinted(ReportClass rptViewer)
        {
            // Set report User
            TextObject obj;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtUserFullName"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtUserFullName"];
                obj.Text = clsUser.ClassUserFullName;
            }
        }

        private void SetReceiptDateFromTo(ReportClass rptViewer)
        {
            // Set report DateFromTo
            TextObject obj;

            if (rptViewer.ReportDefinition.ReportObjects["txtDateFromTo"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtDateFromTo"];
                obj.Text = "Filter Date: " + clsSearch.ClassDateFrom + " to " + clsSearch.ClassDateTo;
            }
        }

        private void SetReceiptMerchant(ReportClass rptViewer)
        {
            // Set report User
            TextObject obj;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtMerchant"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtMerchant"];
                obj.Text = clsSearch.ClassMerchantName;
            }
        }

        private void SetReceiptContactPerson(ReportClass rptViewer)
        {
            // Set report User
            TextObject obj;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtContactPerson"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtContactPerson"];
                obj.Text = clsParticular.ClassContactPerson;
            }
        }

        private void SetReceiptContactNo(ReportClass rptViewer)
        {
            // Set report User
            TextObject obj;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtContactNo"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtContactNo"];
                obj.Text = clsParticular.ClassTelNo + " / " + clsParticular.ClassMobile;
            }
        }

        private void SetReceiptTID(ReportClass rptViewer)
        {
            // Set report User
            TextObject obj;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtTID"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtTID"];
                obj.Text = clsIR.ClassTID;
            }
        }

        private void SetReceiptMID(ReportClass rptViewer)
        {
            // Set report User
            TextObject obj;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtMID"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtMID"];
                obj.Text = clsIR.ClassMID;
            }
        }

        private void SetReceiptIRNo(ReportClass rptViewer)
        {
            // Set report User
            TextObject obj;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtIRNo"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtIRNo"];
                obj.Text = clsIR.ClassIRNo;
            }
        }

        private void SetReceiptAddress(ReportClass rptViewer)
        {
            // Set report User
            TextObject obj;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtAddress"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtAddress"];
                obj.Text = clsParticular.ClassAddress;
            }
        }

        private void SetReceiptRegion(ReportClass rptViewer)
        {
            // Set report User
            TextObject obj;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtRegion"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtRegion"];
                obj.Text = clsParticular.ClassRegion + " / " + clsParticular.ClassProvince;
            }
        }

        private void SetReceiptPrimaryNum(ReportClass rptViewer)
        {
            // Set report User
            TextObject obj;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtPrimaryNum"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtPrimaryNum"];
                obj.Text = clsIR.ClassPrimaryNum;
            }
        }

        private void SetReceiptSecondaryNum(ReportClass rptViewer)
        {
            // Set report User
            TextObject obj;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtSecondaryNum"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtSecondaryNum"];
                obj.Text = clsIR.ClassSecondaryNum;
            }
        }

        private void SetReceiptMDRBreakDownTypeA(ReportClass rptViewer)
        {
            // Set report TAmount
            TextObject obj;
            string sMDRHeader = clsFunction.sNull;
            string sMDR1_Label = clsFunction.sNull;
            string sMDR3_Label = clsFunction.sNull;

            if (clsSearch.ClassMDRType.Equals(dbAPI.GetMDRType()[1])) 
            {
                sMDRHeader = "TYPE A - 3.5% (MDR BREAKDOWN)";
                sMDR1_Label = "MDR(3.5%):";
                sMDR3_Label = "WEEPAYxCITAS(1.3%):";
            }

            if (clsSearch.ClassMDRType.Equals(dbAPI.GetMDRType()[2]))
            {
                sMDRHeader = "TYPE B - 4.0% (MDR BREAKDOWN)";
                sMDR1_Label = "MDR(4.0%):";
                sMDR3_Label = "WEEPAYxCITAS(1.8%):";
            }

            // ----------------------------------------------------------------------------
            // Type A
            // ----------------------------------------------------------------------------
            // Set MDR
            if (rptViewer.ReportDefinition.ReportObjects["txtMDR"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtMDR"];
                obj.Text = sMDRHeader;
            }

            if (rptViewer.ReportDefinition.ReportObjects["txtMDR1_Label"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtMDR1_Label"];
                obj.Text = sMDR1_Label;
            }

            if (rptViewer.ReportDefinition.ReportObjects["txtMDR3_Label"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtMDR3_Label"];
                obj.Text = sMDR3_Label;
            }

            // Set MDR0
            if (rptViewer.ReportDefinition.ReportObjects["txtMDR0"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtMDR0"];
                obj.Text = clsSearch.ClassMDR0;
            }

            // Set MDR1
            if (rptViewer.ReportDefinition.ReportObjects["txtMDR1"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtMDR1"];
                obj.Text = clsSearch.ClassMDR1;
            }

            // Set MDR2
            if (rptViewer.ReportDefinition.ReportObjects["txtMDR2"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtMDR2"];
                obj.Text = clsSearch.ClassMDR2;
            }

            // Set MDR3
            if (rptViewer.ReportDefinition.ReportObjects["txtMDR3"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtMDR3"];
                obj.Text = clsSearch.ClassMDR3;
            }

            // Set MDR4
            if (rptViewer.ReportDefinition.ReportObjects["txtMDR4"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtMDR4"];
                obj.Text = clsSearch.ClassMDR4;
            }

            // Set MDR5
            if (rptViewer.ReportDefinition.ReportObjects["txtMDR5"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtMDR5"];
                obj.Text = clsSearch.ClassMDR5;
            }
            
        }

        private void SetReceiptDepartment(ReportClass rptViewer)
        {
            // Set report Department
            TextObject obj;

            if (rptViewer.ReportDefinition.ReportObjects["txtDepartment"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtDepartment"];
                obj.Text = clsSearch.ClassDepartment;
            }
        }

        private void QRCodeReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptQRCode.rpt");
            Debug.WriteLine("reportFullPath="+ reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptQRCode rptViewer = new rptQRCode();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportCompanyName(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void TimeSheetReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptTimeSheet.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptTimeSheet rptViewer = new rptTimeSheet();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                SetReceiptDateFromTo(rptViewer);
                SetReceiptDateRange(rptViewer);
                SetReceiptDepartment(rptViewer);
                SetReceiptReportTerminalName(rptViewer);
                SetReceiptReportType(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void WorkArrangementReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptWorkArrangement.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptWorkArrangement rptViewer = new rptWorkArrangement();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                //SetReceiptDateFromTo(rptViewer);
                //SetReceiptDateRange(rptViewer);
                SetReceiptDepartment(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void HolidayReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptHoliday.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptHoliday rptViewer = new rptHoliday();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                //SetReceiptDateFromTo(rptViewer);
                //SetReceiptDateRange(rptViewer);
                //SetReceiptDepartment(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void LeaveApplicationReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptLeaveApplication.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptLeaveApplication rptViewer = new rptLeaveApplication();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                //SetReceiptDateFromTo(rptViewer);
                //SetReceiptDateRange(rptViewer);
                SetReceiptDepartment(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void LeaveAssignmentReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptLeaveAssignment.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n"  + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptLeaveAssignment rptViewer = new rptLeaveAssignment();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                //SetReceiptDateFromTo(rptViewer);
                //SetReceiptDateRange(rptViewer);
                SetReceiptDepartment(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void SetReceiptReportType(ReportClass rptViewer)
        {
            // Set report ClientName
            TextObject obj;

            // Set Client
            if (rptViewer.ReportDefinition.ReportObjects["txtReportType"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtReportType"];
                obj.Text = (clsSearch.ClassReportType != null ? clsSearch.ClassReportType : clsFunction.sDefaultSelect);
            }
        }

        private void SetReceiptReportHeader(ReportClass rptViewer)
        {            
            TextObject obj1;
            TextObject obj2;
            TextObject obj3;
            TextObject obj4;
            TextObject obj5;

            // Set Header1
            if (rptViewer.ReportDefinition.ReportObjects["txtHeader1"] != null)
            {
                obj1 = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtHeader1"];
                obj1.Text = clsHeader.ClassName;
            }

            // Set Header2
            if (rptViewer.ReportDefinition.ReportObjects["txtHeader2"] != null)
            {
                obj2 = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtHeader2"];
                obj2.Text = clsHeader.ClassHeader1;
            }

            // Set Header3
            if (rptViewer.ReportDefinition.ReportObjects["txtHeader3"] != null)
            {
                obj3 = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtHeader3"];
                obj3.Text = clsHeader.ClassHeader2;
            }

            // Set Header4
            if (rptViewer.ReportDefinition.ReportObjects["txtHeader4"] != null)
            {
                obj4 = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtHeader4"];
                obj4.Text = clsHeader.ClassHeader3;
            }

            // Set Header5
            if (rptViewer.ReportDefinition.ReportObjects["txtHeader5"] != null)
            {
                obj5 = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtHeader5"];
                obj5.Text = clsHeader.ClassHeader4;
            }

        }

        private void SetReceiptReportCompanyName(ReportClass rptViewer)
        {
            TextObject obj;            

            // Set Name
            if (rptViewer.ReportDefinition.ReportObjects["txtCompanyName"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtCompanyName"];
                obj.Text = clsHeader.ClassName;
            }
            
        }

        private void SetReceiptReportTerminalName(ReportClass rptViewer)
        {
            TextObject obj;

            // Set Name
            if (rptViewer.ReportDefinition.ReportObjects["txtTerminalName"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtTerminalName"];
                obj.Text = clsSearch.ClassTSTerminalID + " - " + clsSearch.ClassTSTerminalName;
            }

        }

        private void SetReceiptReportCountry(ReportClass rptViewer)
        {
            TextObject obj;

            // Set Name
            if (rptViewer.ReportDefinition.ReportObjects["txtCountry"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtCountry"];
                obj.Text = clsSearch.ClassCountry;
            }

        }

        private void FSRClientReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptFindFSRv2.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptFindFSRv2 rptViewer = new rptFindFSRv2();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SeIRReceiptUser(rptViewer);
                
                SetReceiptDateRange(rptViewer);
                SetReceiptClient(rptViewer);
                SetReceiptServiceTypeDesc(rptViewer);
                SetReceiptServiceStatus(rptViewer);
                SetReceiptServiceResultDesc(rptViewer);
                FSRForClientSummary(rptViewer);
                SetReceiptBillable(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void InventoryTerminalDetailReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptInvTerminalDetail.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptInvTerminalDetail rptViewer = new rptInvTerminalDetail();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUserFullName(rptViewer);
                SetReceiptDateRange(rptViewer);
                SetReceiptClient(rptViewer);
                SetReceiptServiceStatusDesc(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void InventorySIMDetailReport()
        {
            string ReportPath = "";
            string reportFullPath = "";
           
            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptInvSIMDetail.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptInvSIMDetail rptViewer = new rptInvSIMDetail();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUserFullName(rptViewer);
                SetReceiptDateRange(rptViewer);
                SetReceiptClient(rptViewer);
                SetReceiptServiceStatusDesc(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view*

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void SetReceiptUserFullName(ReportClass rptViewer)
        {
            // Set report User
            TextObject objUserFullName;

            // Set User Full Name                
            if (rptViewer.ReportDefinition.ReportObjects["txtUserFullName"] != null)
            {
                objUserFullName = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtUserFullName"];
                objUserFullName.Text = "Printed By: " + clsUser.ClassUserFullName;
            }
        }

        private void InstallationReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptIR.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptIR rptViewer = new rptIR();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
               
                SetReceiptDateRange(rptViewer);
                SetReceiptClient(rptViewer);
                SetReceiptServiceStatusDesc(rptViewer);
                
                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void InstallationSummaryReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptIRSummary.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptIRSummary rptViewer = new rptIRSummary();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);

                SetReceiptDateRange(rptViewer);
                SetReceiptClient(rptViewer);
                SetReceiptServiceStatusDesc(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void FSRForClientSummary(ReportClass rpt)
        {      
            TextObject obj;
            string cntSuccess = clsFunction.sDash;
            string cntNegative = clsFunction.sDash;         

            // Installation
            cntSuccess = clsFunction.sDash;
            cntNegative = clsFunction.sDash;
            clsSearch.ClassSearchValue = clsGlobalVariables.TA_STATUS_INSTALLED.ToString();
            dbAPI.ExecuteAPI("GET", "Search", "FSR For Client", clsSearch.ClassSearchValue, "Get Info Detail", "", "GetInfoDetail");                    
            if (dbAPI.isNoRecordFound() == false)
            {
                cntSuccess = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                cntNegative = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);

                if (rpt.ReportDefinition.ReportObjects["txtCntSuccess1"] != null)
                {
                    obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtCntSuccess1"];
                    obj.Text = cntSuccess;
                }

                if (rpt.ReportDefinition.ReportObjects["txtCntNegative1"] != null)
                {
                    obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtCntNegative1"];
                    obj.Text = cntNegative;
                }

                if (rpt.ReportDefinition.ReportObjects["txtCntTotal1"] != null)
                {
                    obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtCntTotal1"];
                    obj.Text = (int.Parse(cntSuccess) + int.Parse(cntNegative)).ToString();
                }
            }

            // Servicing
            cntSuccess = clsFunction.sDash;
            cntNegative = clsFunction.sDash;
            clsSearch.ClassSearchValue = clsGlobalVariables.TA_STATUS_SERVICING.ToString();
            dbAPI.ExecuteAPI("GET", "Search", "FSR For Client", clsSearch.ClassSearchValue, "Get Info Detail", "", "GetInfoDetail");
            if (dbAPI.isNoRecordFound() == false)
            {
                cntSuccess = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                cntNegative = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
               
                if (rpt.ReportDefinition.ReportObjects["txtCntSuccess2"] != null)
                {
                    obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtCntSuccess2"];
                    obj.Text = cntSuccess;
                }

                if (rpt.ReportDefinition.ReportObjects["txtCntNegative2"] != null)
                {
                    obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtCntNegative2"];
                    obj.Text = cntNegative;
                }

                if (rpt.ReportDefinition.ReportObjects["txtCntTotal2"] != null)
                {
                    obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtCntTotal2"];
                    obj.Text = (int.Parse(cntSuccess) + int.Parse(cntNegative)).ToString();
                }
            }

            // Reprogramming
            cntSuccess = clsFunction.sDash;
            cntNegative = clsFunction.sDash;
            clsSearch.ClassSearchValue = clsGlobalVariables.TA_STATUS_REPROGRAMMED.ToString();
            dbAPI.ExecuteAPI("GET", "Search", "FSR For Client", clsSearch.ClassSearchValue, "Get Info Detail", "", "GetInfoDetail");
            if (dbAPI.isNoRecordFound() == false)
            {
                cntSuccess = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                cntNegative = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
              
                if (rpt.ReportDefinition.ReportObjects["txtCntSuccess3"] != null)
                {
                    obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtCntSuccess3"];
                    obj.Text = cntSuccess;
                }

                if (rpt.ReportDefinition.ReportObjects["txtCntNegative3"] != null)
                {
                    obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtCntNegative3"];
                    obj.Text = cntNegative;
                }

                if (rpt.ReportDefinition.ReportObjects["txtCntTotal3"] != null)
                {
                    obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtCntTotal3"];
                    obj.Text = (int.Parse(cntSuccess) + int.Parse(cntNegative)).ToString();
                }
            }

            // Replacement
            cntSuccess = clsFunction.sDash;
            cntNegative = clsFunction.sDash;
            clsSearch.ClassSearchValue = clsGlobalVariables.TA_STATUS_REPLACEMENT.ToString();
            dbAPI.ExecuteAPI("GET", "Search", "FSR For Client", clsSearch.ClassSearchValue, "Get Info Detail", "", "GetInfoDetail");
            if (dbAPI.isNoRecordFound() == false)
            {
                cntSuccess = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                cntNegative = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                
                if (rpt.ReportDefinition.ReportObjects["txtCntSuccess4"] != null)
                {
                    obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtCntSuccess4"];
                    obj.Text = cntSuccess;
                }

                if (rpt.ReportDefinition.ReportObjects["txtCntNegative4"] != null)
                {
                    obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtCntNegative4"];
                    obj.Text = cntNegative;
                }

                if (rpt.ReportDefinition.ReportObjects["txtCntTotal4"] != null)
                {
                    obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtCntTotal4"];
                    obj.Text = (int.Parse(cntSuccess) + int.Parse(cntNegative)).ToString();
                }
            }

            // Pullout
            cntSuccess = clsFunction.sDash;
            cntNegative = clsFunction.sDash;
            clsSearch.ClassSearchValue = clsGlobalVariables.TA_STATUS_PULLEDOUT.ToString();
            dbAPI.ExecuteAPI("GET", "Search", "FSR For Client", clsSearch.ClassSearchValue, "Get Info Detail", "", "GetInfoDetail");
            if (dbAPI.isNoRecordFound() == false)
            {
                cntSuccess = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                cntNegative = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                
                if (rpt.ReportDefinition.ReportObjects["txtCntSuccess5"] != null)
                {
                    obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtCntSuccess5"];
                    obj.Text = cntSuccess;
                }

                if (rpt.ReportDefinition.ReportObjects["txtCntNegative5"] != null)
                {
                    obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtCntNegative5"];
                    obj.Text = cntNegative;
                }

                if (rpt.ReportDefinition.ReportObjects["txtCntTotal5"] != null)
                {
                    obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtCntTotal5"];
                    obj.Text = (int.Parse(cntSuccess) + int.Parse(cntNegative)).ToString();
                }
            }
            
        }

        private void InventoryTerminalSummaryReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptInvTerminalSummary.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptInvTerminalSummary rptViewer = new rptInvTerminalSummary();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUserFullName(rptViewer);
                SetReceiptDateRange(rptViewer);
                SetReceiptClient(rptViewer);
                SetReceiptServiceStatusDesc(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void InventorySIMSummaryReport()
        {
            string ReportPath = "";
            string reportFullPath = "";
           
            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptInvSIMSummary.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptInvSIMSummary rptViewer = new rptInvSIMSummary();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUserFullName(rptViewer);
                SetReceiptDateRange(rptViewer);
                SetReceiptClient(rptViewer);
                SetReceiptServiceStatusDesc(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view*

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void ReleaseTerminalSummaryReport()
        {
            string ReportPath = "";
            string reportFullPath = "";
            
            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptReleaseTerminalSummary.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptReleaseTerminalSummary rptViewer = new rptReleaseTerminalSummary();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUserFullName(rptViewer);
                SetReceiptDateRange(rptViewer);
                //SetReceiptClient(rptViewer);
                //SetReceiptServiceStatusDesc(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view*

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void ReleaseTerminalDetailReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptReleaseTerminalDetail.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptReleaseTerminalDetail rptViewer = new rptReleaseTerminalDetail();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUserFullName(rptViewer);
                SetReceiptDateRange(rptViewer);
                //SetReceiptClient(rptViewer);
                //SetReceiptServiceStatusDesc(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view*

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void FSROperationReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptFindFSROperation.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptFindFSROperation rptViewer = new rptFindFSROperation();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SeIRReceiptUser(rptViewer);

                SetReceiptDateRange(rptViewer);
                SetReceiptClient(rptViewer);
                SetReceiptServiceTypeDesc(rptViewer);
                SetReceiptServiceStatus(rptViewer);
                SetReceiptServiceResultDesc(rptViewer);
                FSRForClientSummary(rptViewer);
                SetReceiptBillable(rptViewer);
                SetReceiptMobile(rptViewer);
                SetReceipFSRTpe(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void ExportToPDF()
        {
            ReportDocument cryRpt = new ReportDocument();

            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptFSR.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            cryRpt.Load(reportFullPath);

            myViewer.ReportSource = cryRpt;

            myViewer.Refresh();

            cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, @"C:\ASD.pdf");

            MessageBox.Show("Exported Successful");
        }

        private void SetReceiptBillable(ReportClass rpt)
        {
            // Set report User
            TextObject obj;

            // Set User Name                
            if (rpt.ReportDefinition.ReportObjects["txtBillable"] != null)
            {
                obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtBillable"];
                obj.Text = clsSearch.ClassIncludeBillable;
            }

        }

        private void SetReceiptMobile(ReportClass rpt)
        {
            // Set report User
            TextObject obj;

            // Set User Name                
            if (rpt.ReportDefinition.ReportObjects["txtMobile"] != null)
            {
                obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtMobile"];
                obj.Text = clsSearch.ClassMobileTerminalID + " - " + clsSearch.ClassMobileAssignedTo;
            }

        }

        private void SetReceipFSRTpe(ReportClass rpt)
        {
            // Set report User
            TextObject obj;

            // Set User Name                
            if (rpt.ReportDefinition.ReportObjects["txtFSRType"] != null)
            {
                obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtFSRType"];
                obj.Text = (clsSearch.ClassIsMobile > 0 ? "MOBILE eFSR" : "MANUAL FSR");
            }

        }

        private void ReleaseSIMSummaryReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptReleaseSIMSummary.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptReleaseSIMSummary rptViewer = new rptReleaseSIMSummary();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUserFullName(rptViewer);
                SetReceiptDateRange(rptViewer);
                //SetReceiptClient(rptViewer);
                //SetReceiptServiceStatusDesc(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view*

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void ReleaseSIMDetailReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptReleaseSIMDetail.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptReleaseSIMDetail rptViewer = new rptReleaseSIMDetail();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUserFullName(rptViewer);
                SetReceiptDateRange(rptViewer);
                //SetReceiptClient(rptViewer);
                //SetReceiptServiceStatusDesc(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view*

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void ReportExport(ReportClass rptViewer,  string pPDFExportFileName)
        {
            pPDFExportFileName = pPDFExportFileName + ".pdf";
            Debug.WriteLine("--ReportExport--");
            Debug.WriteLine("pReportFileName="+ pPDFExportFileName);

            dbFile.DeleteFile(dbFile.sExportPath + pPDFExportFileName);

            rptViewer.ExportToDisk(ExportFormatType.PortableDocFormat, dbFile.sExportPath + pPDFExportFileName);
            
        }

        void POSRentalReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptPOSRental.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptPOSRental rptViewer = new rptPOSRental();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                if (clsSearch.ClassIsExportToPDF)
                {
                    ReportExport(rptViewer, clsSearch.ClassInvoiceNo); // Export
                    clsSearch.ClassIsExportToPDF = false;
                    this.Close();
                }
                else
                {
                    myViewer.ReportSource = rptViewer;
                    myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        
        private void TerminalImportReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptImportTerminal.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptImportTerminal rptViewer = new rptImportTerminal();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                SetReceiptImportCount(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        // ROCKY - SIM IMPORT: ADD REPORT MESSAGE FOR SUCCESS AND DUPLICATE - REPORT VIEWER
        private void SIMImportReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptImportSIM.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptImportSIM rptViewer = new rptImportSIM();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                SetReceiptImportCount(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void SetReceiptImportCount(ReportClass rptViewer)
        {
            // Set report User
            TextObject obj;

            // Set Success Count                
            if (rptViewer.ReportDefinition.ReportObjects["txtSuccessCount"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtSuccessCount"];
                obj.Text = clsSearch.ClassSuccessCount.ToString();
            }

            // Set Failed Count                
            if (rptViewer.ReportDefinition.ReportObjects["txtFailedCount"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtFailedCount"];
                obj.Text = clsSearch.ClassFailedCount.ToString();
            }


        }

        private void InventoryTerminalTypeModelSummaryReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptInvTerminalTypeModelSummary.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptInvTerminalTypeModelSummary rptViewer = new rptInvTerminalTypeModelSummary();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUserFullName(rptViewer);
                SetReceiptDateRange(rptViewer);
                SetReceiptClient(rptViewer);
                SetReceiptServiceStatusDesc(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void InventorySIMTelcoSummaryReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptInvSIMTelcoSummary.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptInvSIMTelcoSummary rptViewer = new rptInvSIMTelcoSummary();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUserFullName(rptViewer);
                SetReceiptDateRange(rptViewer);
                SetReceiptClient(rptViewer);
                SetReceiptServiceStatusDesc(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view*

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        // ROCKY - PARTICULAR: ADD PARTICULAR DETAILS REPORT - REPORT VIEWER
        private void ParticularDetailsReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptParticularDetails.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptParticularDetails rptViewer = new rptParticularDetails();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        // ROCKY - PARTICULAR: ADD PARTICULAR REQUIREMENTS DETAIL REPORT - REPORT VIEWER
        private void ParticularRequirementsReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptParticularRequirements.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptParticularRequirements rptViewer = new rptParticularRequirements();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        // ROCKY - BILLING: ADD SECURITY BANK BILLING DETAILS - REPORT VIEWER
        private void SecurityBankBillingReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptBillingSecurityBankDetails.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptBillingSecurityBankDetails rptViewer = new rptBillingSecurityBankDetails();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        // ROCKY - BILLING: ADD METROBANK SERVICING DETAILS - REPORT VIEWER
        private void MccServicingReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptBillingMccServicing.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptBillingServicingSummary rptViewer = new rptBillingServicingSummary();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                SetReceiptDateRange(rptViewer);
                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        // ROCKY - BILLING: ADD METROBANK SERVICING INVOICE - REPORT VIEWER
        private void MccServicingInvoice()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptBillingMccServicingInvoice.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptBillingServicingInvoice rptViewer = new rptBillingServicingInvoice();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUserNoPrinted(rptViewer);
                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        // ROCKY - BILLING: ADD METROBANK NET MATRIX DETAILS - REPORT VIEWER
        private void MccNetMatrix()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptBillingMccNetMatrix.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptBillingLeasingSummary rptViewer = new rptBillingLeasingSummary();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUserNoPrinted(rptViewer);
                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void MccNetMatrixInvoice()
        {
            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptBillingMccNetMatrixInvoice.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {
                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptBillingLeasingInvoice rptViewer = new rptBillingLeasingInvoice();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUserNoPrinted(rptViewer);
                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void eDiagnostic()
        {
            Debug.WriteLine("--eDiagnostic--");

            clsSearch.ClassStatementType = "View";
            clsSearch.ClassSearchBy = "eFSR Diagnostic";
            clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
            clsSearch.ClassStoredProcedureName = "spViewAdvanceDetail";


            string ReportPath = "";
            string reportFullPath = "";

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptEFSRDiagnostic.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptEFSRDiagnostic rptViewer = new rptEFSRDiagnostic();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);                
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                
                if (clsSearch.ClassIsExportToPDF)
                {
                    ReportExport(rptViewer, clsSearch.ClassServiceNo.ToString() + "_diag"); // Export
                    clsSearch.ClassIsExportToPDF = false;
                    this.Close();
                }
                else
                {
                    myViewer.ReportSource = rptViewer;                    
                    myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

                Application.Exit();

            }

        }

        private void ServiceSummaryReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            Cursor.Current = Cursors.WaitCursor;

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptServiceTypeSLA.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptServiceTypeSLA rptViewer = new rptServiceTypeSLA();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);                
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);              
                SetReceiptDateRange(rptViewer);
                SetReceiptReportType(rptViewer);
                SetReceiptServiceType(rptViewer);
                SetReceiptServiceStatus(rptViewer);
                SetReceiptServiceResult(rptViewer);
                SetReceiptClient(rptViewer);
                SetReceiptBillingType(rptViewer);
                SetReceiptFSRMode(rptViewer);

                myViewer.ReportSource = rptViewer;                
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            Cursor.Current = Cursors.Default;

        }

        private void ServiceDetailReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            Cursor.Current = Cursors.WaitCursor;

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptServiceTypeDetail.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptServiceTypeDetail rptViewer = new rptServiceTypeDetail();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);                
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                SetReceiptDateRange(rptViewer);
                SetReceiptReportType(rptViewer);
                SetReceiptServiceType(rptViewer);
                SetReceiptServiceStatus(rptViewer);
                SetReceiptServiceResult(rptViewer);
                SetReceiptClient(rptViewer);
                SetReceiptBillingType(rptViewer);
                SetReceiptFSRMode(rptViewer);

                myViewer.ReportSource = rptViewer;                
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            Cursor.Current = Cursors.Default;

        }

        private void frmReportViewer_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;               
            }
        }

        private void SetReceiptServiceType(ReportClass rpt)
        {
            // Set report User
            TextObject obj;

            // Set User Name                
            if (rpt.ReportDefinition.ReportObjects["txtServiceType"] != null)
            {
                obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtServiceType"];
                obj.Text = (clsSearch.ClassServiceTypeDesc != null ? clsSearch.ClassServiceTypeDesc : clsFunction.sDefaultSelect);
            }

        }

        private void SetReceiptServiceStatus(ReportClass rpt)
        {
            // Set report User
            TextObject obj;

            // Set User Name                
            if (rpt.ReportDefinition.ReportObjects["txtServiceStatus"] != null)
            {
                obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtServiceStatus"];
                obj.Text = (clsSearch.ClassServiceStatusDesc != null ? clsSearch.ClassServiceStatusDesc : clsFunction.sDefaultSelect);
            }

        }

        private void SetReceiptServiceResult(ReportClass rpt)
        {
            // Set report User
            TextObject obj;

            // Set User Name                
            if (rpt.ReportDefinition.ReportObjects["txtServiceResult"] != null)
            {
                obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtServiceResult"];
                obj.Text = (clsSearch.ClassServiceResultDesc != null ? clsSearch.ClassServiceResultDesc : clsFunction.sDefaultSelect);
            }

        }

        private void ServiceInstallationSummaryReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            Cursor.Current = Cursors.WaitCursor;

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptServiceInstallationSummary.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptServiceInstallationSummary rptViewer = new rptServiceInstallationSummary();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                SetReceiptDateRange(rptViewer);
                SetReceiptReportType(rptViewer);
                SetReceiptClient(rptViewer);

                myViewer.ReportSource = rptViewer;                
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            Cursor.Current = Cursors.Default;

        }

        private void ServiceInstallationDetailReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            Cursor.Current = Cursors.WaitCursor;

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptServiceInstallationDetail.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptServiceInstallationDetail rptViewer = new rptServiceInstallationDetail();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);                
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                SetReceiptDateRange(rptViewer);
                SetReceiptReportType(rptViewer);
                SetReceiptClient(rptViewer);

                myViewer.ReportSource = rptViewer;                
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            Cursor.Current = Cursors.Default;

        }

        private void ServiceHistoryDetailReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            Cursor.Current = Cursors.WaitCursor;

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptServiceHistoryDetail.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptServiceHistoryDetail rptViewer = new rptServiceHistoryDetail();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);               
                SetReceiptReportType(rptViewer);               
                SetReceiptClient(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            Cursor.Current = Cursors.Default;

        }

        void SetReceiptBillingType(ReportClass rpt)
        {
            // Set report User
            TextObject obj;

            // Set User Name                
            if (rpt.ReportDefinition.ReportObjects["txtBillingType"] != null)
            {
                obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtBillingType"];
                obj.Text = (clsSearch.ClassIncludeBillable != null ? clsSearch.ClassIncludeBillable : clsFunction.sDefaultSelect);
            }

        }

        void SetReceiptFSRMode(ReportClass rpt)
        {
            // Set report User
            TextObject obj;

            // Set User Name                
            if (rpt.ReportDefinition.ReportObjects["txtFSRMode"] != null)
            {
                obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtFSRMode"];
                obj.Text = (clsSearch.ClassFSRMode != null ? clsSearch.ClassFSRMode : clsFunction.sDefaultSelect);
            }

        }

        private void ActiveTerminalReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            Cursor.Current = Cursors.WaitCursor;

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptActiveTerminal.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptActiveTerminal rptViewer = new rptActiveTerminal();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);                
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                SetReceiptReportType(rptViewer);
                SetReceiptClient(rptViewer);
                SetReceiptLocation(rptViewer);
                SetReceiptTerminalStatus(rptViewer);

                myViewer.ReportSource = rptViewer;                
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            Cursor.Current = Cursors.Default;

        }

        private void ActiveTerminalDetailReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            Cursor.Current = Cursors.WaitCursor;

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptActiveDetailTerminal.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptActiveDetailTerminal rptViewer = new rptActiveDetailTerminal();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                SetReceiptReportType(rptViewer);
                SetReceiptClient(rptViewer);
                SetReceiptLocation(rptViewer);
                SetReceiptTerminalStatus(rptViewer);
                SetReceiptTerminalType(rptViewer);
                SetReceiptTerminalModel(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            Cursor.Current = Cursors.Default;

        }

        private void ActiveSIMReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            Cursor.Current = Cursors.WaitCursor;

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptServiceHistoryDetail.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptServiceHistoryDetail rptViewer = new rptServiceHistoryDetail();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);                
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                SetReceiptReportType(rptViewer);
                SetReceiptClient(rptViewer);

                myViewer.ReportSource = rptViewer;                
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            Cursor.Current = Cursors.Default;

        }

        private void SetReceiptLocation(ReportClass rpt)
        {
            // Set report User
            TextObject obj;

            // Set User Name                
            if (rpt.ReportDefinition.ReportObjects["txtLocation"] != null)
            {
                obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtLocation"];
                obj.Text = (clsSearch.ClassLocation != null ? clsSearch.ClassLocation : clsFunction.sDefaultSelect);
            }

        }

        private void SetReceiptTerminalStatus(ReportClass rpt)
        {
            // Set report User
            TextObject obj;

            // Set User Name                
            if (rpt.ReportDefinition.ReportObjects["txtTerminalStatus"] != null)
            {
                obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtTerminalStatus"];
                obj.Text = (clsSearch.ClassTerminalStatusDescription != null ? clsSearch.ClassTerminalStatusDescription : clsFunction.sDefaultSelect);
            }

        }

        private void SetReceiptTerminalType(ReportClass rpt)
        {
            // Set report User
            TextObject obj;

            // Set User Name                
            if (rpt.ReportDefinition.ReportObjects["txtTerminalType"] != null)
            {
                obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtTerminalType"];
                obj.Text = (clsSearch.ClassTerminalType != null ? clsSearch.ClassTerminalType : clsFunction.sDefaultSelect);
            }

        }

        private void SetReceiptTerminalModel(ReportClass rpt)
        {
            // Set report User
            TextObject obj;

            // Set User Name                
            if (rpt.ReportDefinition.ReportObjects["txtTerminalModel"] != null)
            {
                obj = (TextObject)rpt.ReportDefinition.ReportObjects["txtTerminalModel"];
                obj.Text = (clsSearch.ClassTerminalModel!= null ? clsSearch.ClassTerminalModel : clsFunction.sDefaultSelect);
            }

        }

        private bool isValidReportDataSet(DataSet dsReport, string ReportPath)
        {
            bool isValid = false;

            if (dsReport == null || dsReport.Tables.Count == 0 || dsReport.Tables[0].Rows.Count == 0)
            {
                isValid = false;
            }
            else
            {
                isValid = true;
            }

            if (!isValid)
            {
                dbFunction.SetMessageBox("No records found." + "\n\n" + "Report: " + dbFunction.AddBracketStartEnd(ReportPath) + "\n\n" +
                                         "Please check your fields filtered.", "Report", clsFunction.IconType.iInformation);
                this.Close();
            }

            return isValid;
        }

        public void disposeDataSet(DataSet ds)
        {
            if (ds != null)
            {
                ds.Clear();
                ds.Dispose();
            }
        }

        public void disposeRepotDocument(ReportClass report)
        {
            if (report != null)
            {
                report.Close();
                report.Dispose();
            }
        }

        private void MonthlyServiceCountReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            Cursor.Current = Cursors.WaitCursor;

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptServiceTypeMonthly.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptServiceTypeMonthly rptViewer = new rptServiceTypeMonthly();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);                
                SetReceiptClient(rptViewer);
                SetReceiptServiceResultDesc(rptViewer);
                SetReceiptBillingType(rptViewer);
                SetReceiptFSRMode(rptViewer);
                SetReceiptReportType(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            Cursor.Current = Cursors.Default;

        }

        private void SetReceiptCurrentComponent(ReportClass rptViewer)
        {
            // Set report ClientName
            TextObject obj;
            string pValue = "";
            
            pValue = (clsSearch.ClassComponents == null || clsSearch.ClassComponents.Equals(clsDefines.gDash) || clsSearch.ClassComponents.Equals(clsFunction.sDefaultSelect) ? "-" : clsSearch.ClassComponents);

            // Set Client
            if (rptViewer.ReportDefinition.ReportObjects["txtCurComponent"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtCurComponent"];
                obj.Text = dbFunction.CheckAndSetStringValue(pValue);
            }
        }

        private void SetReceiptReplaceComponent(ReportClass rptViewer)
        {
            // Set report ClientName
            TextObject obj;
            string pValue = "";

            pValue = (clsSearch.ClassRepComponents == null || clsSearch.ClassRepComponents.Equals(clsDefines.gDash) || clsSearch.ClassRepComponents.Equals(clsFunction.sDefaultSelect) ? "-" : clsSearch.ClassRepComponents);

            // Set Client
            if (rptViewer.ReportDefinition.ReportObjects["txtRepComponent"] != null)
            {
                obj = (TextObject)rptViewer.ReportDefinition.ReportObjects["txtRepComponent"];
                obj.Text = dbFunction.CheckAndSetStringValue(pValue);
            }
        }

        private void initReport(bool isReportView)
        {
            Cursor.Current = Cursors.WaitCursor;

            myViewer.Visible = lvwList.Visible = false;
            btnExport.Enabled = false;

            if (isReportView)
            {
                ucStatusDisplay.SetStatus($"Preparing report view {clsSearch.ClassReportDescription}", Enums.StatusType.Processing);
                Task.Delay(500); // Asynchronously wait without blocking UI

                myViewer.Visible = true;
                myViewer.Dock = DockStyle.Fill;
            }
            else
            {
                ucStatusDisplay.SetStatus($"Exporting report list view {clsSearch.ClassReportDescription}", Enums.StatusType.Processing);
                Task.Delay(500); // Asynchronously wait without blocking UI

                lvwList.Visible = true;
                lvwList.Dock = DockStyle.Fill;
                btnExport.Enabled = true;
            }

            lblRecordCount.Text = "RECORD COUNT: " + (clsGlobalVariables.globalDataTable!=null ? clsGlobalVariables.globalDataTable.Rows.Count.ToString() : clsFunction.sZero);
            Cursor.Current = Cursors.Default;
        }

        private void btnReportView_Click(object sender, EventArgs e)
        {
            initReport(true);
        }

        private void btnListView_Click(object sender, EventArgs e)
        {
            initReport(false);
        }

        private void frmReportViewer_Activated(object sender, EventArgs e)
        {
            initReport(true);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> tabJsonTabMenu = new Dictionary<string, string>();
            Dictionary<string, DataSet> dataSets = new Dictionary<string, DataSet>();

            if (clsSearch.ClassIncludeDetailTab > 0)
            {
                if (!dbFunction.fPromptConfirmation("This detail contains a large number of records and may take longer to load or export.\n\nAre you sure you want to continue?")) return;
            }
            else
            {
                dbFunction.SetMessageBox($"Exporting to file.\n\n{clsDefines.TAKE_FEW_MINUTE_MSG}", "Export", clsFunction.IconType.iInformation);
            }

            Cursor.Current = Cursors.WaitCursor;

            //ucStatusDisplay.SetStatus("", Enums.StatusType.Init);
            //Task.Delay(500); // Asynchronously wait without blocking UI

            dbFile.WriteSysytemLog($"Export started...{clsReport.ClassReportDesc}"); // add log

            try
            {

                string filePath = clsSearch.ClassBankCode + "_" + "ExportedData_" + clsSearch.ClassReportType + "_" + dbFunction.getCurrentDate() + "_" + dbFunction.padLeftChar(clsSearch.ClassReportID.ToString(), clsFunction.sZero, 4); // Default file name
                filePath = $"{filePath}{clsDefines.FILE_EXT_XLXS}";

                clsSearch.ClassProcessStartTime = dbFunction.GetRequestTime(clsDefines.gNull);

                switch (clsSearch.ClassReportID)
                {
                    case 53: // SERVICE INSTALLATION REPORT
                    case 54: // SERVICE MAINTENANCE REPORT
                    case 55: // SERVICE PULLOUT REPORT

                        tabJsonTabMenu = new Dictionary<string, string>
                        {
                            { "tab1", "Summary" },
                            { "tab2", "Detail" },
                            { "tab3", "Daily" },
                            { "tab4", "Holiday List" }
                        };

                        // -------------------------------------------------------------------------
                        // Summary tab
                        // -------------------------------------------------------------------------
                        //ucStatusDisplay.SetStatus($"Processing summary tab sheet...", Enums.StatusType.Processing);
                        //Task.Delay(500); // Asynchronously wait without blocking UI
                        
                        DataSet summaryDataSet = new DataSet();
                        if (clsSearch.ClassIncludeSummaryTab > 0)
                        {
                            string pMonthHeader = "'January','February','March','April','May','June','July','August','September','October','November','December', '0'";
                            string pRequestHeader = "'-7 and Below','-6','-5','-4','-3','-2','-1','0','1','2','3','4 and Above', '0'";
                            string pSQL = "";
                            string pSearchBy = "Report Data Storage Header";

                            dbFile.WriteSysytemLog($"Delete Report Data Storage..."); // add log
                            dbAPI.ExecuteAPI("DELETE", "Delete", "", "", "Report Data Storage", "", "DeleteCollectionDetail");
                            dbFile.WriteSysytemLog($"Delete Report Data Storage...complete"); // add log
                            
                            pSQL = $"('7', 'CURRENT SLA %', '-',{pMonthHeader})";
                            dbFile.WriteSysytemLog($"Insert {pSearchBy}-{pSQL}..."); // add log
                            dbAPI.ExecuteAPI("POST", "Insert", "", "", pSearchBy, pSQL, "InsertCollectionMaster");
                            dbConnect.getStoredProcedureDateSet("View", "Overall Current SLA", $"{clsSearch.ClassClientID}{clsDefines.gPipe}{clsSearch.ClassJobTypeList}", "spProcessReportDataStorage");
                            dbFile.WriteSysytemLog($"Insert {pSearchBy}-{pSQL}...complete"); // add log

                            
                            pSQL = $"('6', 'REQUEST PER TEAM LEAD', '-',{pRequestHeader})";
                            dbFile.WriteSysytemLog($"Insert {pSearchBy}-{pSQL}..."); // add log
                            dbAPI.ExecuteAPI("POST", "Insert", "", "", pSearchBy, pSQL, "InsertCollectionMaster");
                            dbConnect.getStoredProcedureDateSet("View", "Overall Request Per Team Lead", $"{clsSearch.ClassClientID}{clsDefines.gPipe}{clsSearch.ClassJobTypeList}", "spProcessReportDataStorage");
                            dbFile.WriteSysytemLog($"Insert {pSearchBy}-{pSQL}...complete"); // add log

                            if (clsSearch.ClassReportID == 53)
                            {
                                pSQL = $"('2', 'OVERALL REQUESTS {dbFunction.getCurrentYear()}%', '-',{pMonthHeader})";
                                dbFile.WriteSysytemLog($"Insert {pSearchBy}-{pSQL}..."); // add log
                                dbAPI.ExecuteAPI("POST", "Insert", "", "", pSearchBy, pSQL, "InsertCollectionMaster");
                                dbConnect.getStoredProcedureDateSet("View", "Overall Request Summary", $"{clsSearch.ClassClientID}{ clsDefines.gPipe}{clsSearch.ClassJobTypeList}", "spProcessReportDataStorage");
                                dbFile.WriteSysytemLog($"Insert {pSearchBy}-{pSQL}...complete"); // add log
                            }

                            pSQL = $"('3', 'OVERALL REQUESTS', '-',{pMonthHeader})";
                            dbFile.WriteSysytemLog($"Insert {pSearchBy}-{pSQL}..."); // add log
                            dbAPI.ExecuteAPI("POST", "Insert", "", "", pSearchBy, pSQL, "InsertCollectionMaster");
                            dbConnect.getStoredProcedureDateSet("View", "Overall Status Summary", $"{clsSearch.ClassClientID}{ clsDefines.gPipe}{clsSearch.ClassJobTypeList}", "spProcessReportDataStorage");
                            dbFile.WriteSysytemLog($"Insert {pSearchBy}-{pSQL}...complete"); // add log

                            pSQL = $"('4', 'REQUEST WITHIN SLA', '-',{pMonthHeader})";
                            dbFile.WriteSysytemLog($"Insert {pSearchBy}-{pSQL}..."); // add log
                            dbAPI.ExecuteAPI("POST", "Insert", "", "", pSearchBy, pSQL, "InsertCollectionMaster");
                            dbConnect.getStoredProcedureDateSet("View", "Overall Request Within SLA Summary", $"{clsSearch.ClassClientID}{ clsDefines.gPipe}{clsSearch.ClassJobTypeList}", "spProcessReportDataStorage");
                            dbFile.WriteSysytemLog($"Insert {pSearchBy}-{pSQL}...complete"); // add log

                            pSQL = $"('1', 'NEGATIVE/UNSUCCESSFUL ACTIVITY', '-',{pMonthHeader})";
                            dbFile.WriteSysytemLog($"Insert {pSearchBy}-{pSQL}..."); // add log
                            dbAPI.ExecuteAPI("POST", "Insert", "", "", pSearchBy, pSQL, "InsertCollectionMaster");
                            dbConnect.getStoredProcedureDateSet("View", "Reason Summary", $"{clsSearch.ClassClientID}{clsDefines.gPipe}{clsSearch.ClassJobTypeList}", "spProcessReportDataStorage");
                            dbFile.WriteSysytemLog($"Insert {pSearchBy}-{pSQL}...complete"); // add log

                            //dbConnect.getStoredProcedureDateSet("View", "Overall Request Beyond SLA Summary", $"{clsSearch.ClassClientID}{ clsDefines.gPipe}{clsSearch.ClassJobTypeList}", "spProcessReportDataStorage");

                            dbFile.WriteSysytemLog($"View Report Data Storage..."); // add log
                            summaryDataSet = dbConnect.getStoredProcedureDateSet("View", "Report Data Storage", "", "spViewReport");
                            dbFile.WriteSysytemLog($"View Report Data Storage...complete"); // add log
                        }
                        
                        // -------------------------------------------------------------------------
                        // Detail tab
                        // -------------------------------------------------------------------------
                        //ucStatusDisplay.SetStatus($"Processing detail tab sheet...", Enums.StatusType.Processing);
                        //Task.Delay(500); // Asynchronously wait without blocking UI

                        DataSet detailDataSet = new DataSet();
                        if (clsSearch.ClassIncludeDetailTab > 0)
                        {
                            dbFile.WriteSysytemLog($"View Detail Service Attempt..."); // add log
                            detailDataSet = dbConnect.getStoredProcedureDateSet("View", "Detail Service Attempt", clsSearch.ClassClientID + clsFunction.sPipe +
                                                                                                        clsSearch.ClassJobTypeList + clsFunction.sPipe +
                                                                                                        clsSearch.ClassIRIDNo + clsFunction.sPipe +
                                                                                                        clsFunction.sZero + clsFunction.sPipe +
                                                                                                        clsSearch.ClassDetailDateFrom + clsFunction.sPipe +
                                                                                                        clsSearch.ClassDetailDateTo + clsFunction.sPipe +
                                                                                                        clsSearch.ClassIsExcludePending + clsFunction.sPipe +
                                                                                                        clsSearch.ClassReasonID, "spViewReport");
                            dbFile.WriteSysytemLog($"View Detail Service Attempt...complete"); // add log
                        }

                        // -------------------------------------------------------------------------
                        // Daily Tab
                        // -------------------------------------------------------------------------
                        //ucStatusDisplay.SetStatus($"Processing daily tab sheet...", Enums.StatusType.Processing);
                        //Task.Delay(500); // Asynchronously wait without blocking UI

                        dbFile.WriteSysytemLog($"DailyDataSet..."); // add log
                        DataSet dailyDataSet = new DataSet();
                        dailyDataSet = clsGlobalVariables.globalDataSet;
                        dbFile.WriteSysytemLog($"DailyDataSet...complete"); // add log

                        // -------------------------------------------------------------------------
                        // Holiday List Tab
                        // -------------------------------------------------------------------------
                        dbFile.WriteSysytemLog($"View Holiday List..."); // add log
                        DataSet holidayDataSet = new DataSet();
                        holidayDataSet = dbConnect.getStoredProcedureDateSet("View", "Holiday List", $"{clsFunction.sZero}{clsFunction.sPipe}{clsFunction.sZero}", "spViewHoliday");
                        dbFile.WriteSysytemLog($"View Holiday List...complete"); // add log

                        clsSearch.ClassProcessEndTime = dbFunction.GetResponseTime(clsDefines.gNull);

                        // Create the dictionary of datasets
                        dataSets = new Dictionary<string, DataSet>
                        {
                            { "tab1", summaryDataSet },
                            { "tab2", detailDataSet },
                            { "tab3", dailyDataSet },
                            { "tab4", holidayDataSet }
                        };

                        // debugging dataset
                        //dbFunction.debugDataSet(dailyDataSet, 0);

                        //ucStatusDisplay.SetStatus($"Exporting report {clsSearch.ClassReportDescription}", Enums.StatusType.Export);
                        //Task.Delay(500); // Asynchronously wait without blocking UI

                        dbFile.WriteSysytemLog($"ExportListViewToExcelWithTabSheet..."); // add log
                        //dbFile.ExportListViewToExcelWithTabSheet(lvwList, filePath, dataSets, tabJsonTabMenu);
                        dbExcelOpenXmlExporter.ExportListViewToExcelWithTabSheet(lvwList, filePath, dataSets, tabJsonTabMenu);
                        dbFile.WriteSysytemLog($"ExportListViewToExcelWithTabSheet...complete"); // add log

                        break;

                    case 57: // HELPDESK SUMMARY

                        string[] reportNames = {
                            "Helpdesk-Summary-Report",
                            "Helpdesk-Detailed-Report",
                            "Helpdesk-ActualProblem-Report"
                        };

                        DataTable[] tables = reportNames
                            .Select(name => dbConnect.getStoredProcedureDateSet("View", name, clsSearch.ClassSearchValue, "spViewReport").Tables[0])
                            .ToArray();

                        tables[0] = ReplaceDuplicateData(tables[0], "NAME");

                        ExportCustomDataToExcel(filePath,
                            tables,
                            new[] { "Summary", "Details", "Problems" }, // Sheetnames
                            new[] { Color.SteelBlue, Color.DarkGreen, Color.SteelBlue }); // Header Colors

                        break;

                    case 58: // HELPDESK KPI

                        string[] KpiTables = {
                            "Helpdesk-Summary-Report",
                            "Helpdesk-Resolution-Report",
                            "Helpdesk-Breakdown-Report",
                            "Helpdesk-ActualProblem-Report",
                            "Helpdesk-Detailed-Report",
                        };

                        DataTable[] Kpi = KpiTables
                            .Select(name => dbConnect.getStoredProcedureDateSet("View", name, clsSearch.ClassSearchValue, "spViewReport").Tables[0])
                            .ToArray();

                        // Summary
                        Kpi[0] = ReplaceDuplicateData(Kpi[0], "NAME");


                        Export_KPI_Report(Kpi[0], Kpi[1], Kpi[2], Kpi[3], Kpi[4]);

                        break;

                    default:
                        //ucStatusDisplay.SetStatus($"Exporting report {clsSearch.ClassReportDescription}", Enums.StatusType.Export);
                        //Task.Delay(500); // Asynchronously wait without blocking UI

                        tabJsonTabMenu = new Dictionary<string, string>
                        {
                            { "tab1", "Active POS" }
                        };

                        // Create the dictionary of datasets
                        dataSets = new Dictionary<string, DataSet>
                        {
                            { "tab1", clsGlobalVariables.globalDataSet }
                        };

                        clsSearch.ClassProcessEndTime = dbFunction.GetResponseTime(clsDefines.gNull);

                        dbFile.WriteSysytemLog($"ExportListViewToExcelWithTabSheet..."); // add log
                        //dbFile.ExportListViewToExcel(lvwList, filePath, clsGlobalVariables.globalDataSet);
                        dbExcelOpenXmlExporter.ExportListViewToExcelWithTabSheet(lvwList, filePath, dataSets, tabJsonTabMenu);
                        dbFile.WriteSysytemLog($"ExportListViewToExcelWithTabSheet...complete"); // add log

                        break;
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exceptional error " + ex.Message);
                dbFunction.SetMessageBox("Exceptional error " + ex.Message, "Preview Report", clsFunction.IconType.iError);
            }
            
            Cursor.Current = Cursors.Default;

            //ucStatusDisplay.SetStatus("", Enums.StatusType.Init);
            //Task.Delay(500); // Asynchronously wait without blocking UI

            initReport(false);

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchQuery = txtSearch.Text.Trim();

            // Call the method to search through the ListView based on the query
            if (dbFunction.isValidDescription(searchQuery))
                dbFunction.searchListView(lvwList, searchQuery);
        }

        private void initButton()
        {
            btnSearch.Enabled = false;

            dbFunction.SetButtonIconImage(btnSearch);
        }

        private void ServiceInstallationReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            Cursor.Current = Cursors.WaitCursor;

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptServiceInstallation.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptServiceInstallation rptViewer = new rptServiceInstallation();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                SetReceiptDateRange(rptViewer);
                SetReceiptClient(rptViewer);               
                SetReceiptReportType(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            Cursor.Current = Cursors.Default;

        }

        private void ServiceMaintenanceReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            Cursor.Current = Cursors.WaitCursor;

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptServiceMaintenance.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptServiceMaintenance rptViewer = new rptServiceMaintenance();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                SetReceiptDateRange(rptViewer);
                SetReceiptClient(rptViewer);
                SetReceiptReportType(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            Cursor.Current = Cursors.Default;

        }

        private void ServicePullOutReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            Cursor.Current = Cursors.WaitCursor;

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptServicePullOut.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptServicePullOut rptViewer = new rptServicePullOut();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                SetReceiptDateRange(rptViewer);
                SetReceiptClient(rptViewer);
                SetReceiptReportType(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            Cursor.Current = Cursors.Default;

        }

        private void UnclosedServiceTicketReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            Cursor.Current = Cursors.WaitCursor;

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptServiceUnclosedTicket.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptServiceUnclosedTicket rptViewer = new rptServiceUnclosedTicket();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                SetReceiptReportHeader(rptViewer);
                SetReceiptUser(rptViewer);
                SetReceiptDateRange(rptViewer);
                SetReceiptClient(rptViewer);
                SetReceiptReportType(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            Cursor.Current = Cursors.Default;

        }

        private void HelpdeskSummaryReport()
        {
            DataTable HDSummaryDs = dbConnect
                                     .getStoredProcedureDateSet("View",
                                      "Helpdesk-Summary-Report",
                                      clsSearch.ClassSearchValue,
                                      "spViewReport")
                                     .Tables[0];

            HDSummaryDs = ReplaceDuplicateData(HDSummaryDs, "NAME");

            SetListViewData(lvwList, HDSummaryDs, null, null);
        }

        private void HelpdeskServiceReport()
        {
            string ReportPath = "";
            string reportFullPath = "";

            Cursor.Current = Cursors.WaitCursor;

            ReportPath = GetReportPath();
            reportFullPath = Path.Combine(ReportPath, "rptHelpdeskFSR.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]" + "\n\n" + "File not found!");
                return;
            }

            try
            {

                DataSet dsReport = dbConnect.GetReportWithStoredProcedure(clsSearch.ClassReportID, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStoredProcedureName, lvwList);
                rptServiceUnclosedTicket rptViewer = new rptServiceUnclosedTicket();

                if (!isValidReportDataSet(dsReport, reportFullPath)) return;

                rptViewer.Load(reportFullPath);
                rptViewer.SetDataSource(dsReport.Tables[0]);

                //SetReceiptReportHeader(rptViewer);
                //SetReceiptUser(rptViewer);
                //SetReceiptDateRange(rptViewer);
                //SetReceiptClient(rptViewer);
                //SetReceiptReportType(rptViewer);

                myViewer.ReportSource = rptViewer;
                myViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "[" + reportFullPath + "]", "Report could not be created",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            Cursor.Current = Cursors.Default;

        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.SelectedItems.Count > 0)
            {
                int selectedRow = lvwList.SelectedItems[0].Index; // Get the first selected row index
                string jsonResult = dbFunction.genJSONFormat(lvwList, selectedRow, "", clsDefines.NESTED_OBJECT_VALUES);
                Debug.WriteLine($"Selected row={selectedRow}, jsonResult={jsonResult}");

                // Pass JSON to popup window
                frmPopUpInfo frm = new frmPopUpInfo(jsonResult);
                frm.ShowDialog();

                initReport(false);
            }

        }

        private void HelpdeskKPIReport()
        {
            DataTable HDSummaryDs = dbConnect
                                     .getStoredProcedureDateSet("View",
                                      "Helpdesk-Summary-Report",
                                      clsSearch.ClassSearchValue,
                                      "spViewReport")
                                     .Tables[0];

            HDSummaryDs = ReplaceDuplicateData(HDSummaryDs, "NAME");

            SetListViewData(lvwList, HDSummaryDs, null, null);
        }

        private void initTabButton(object sender, EventArgs e)
        {
            switch (clsSearch.ClassReportID)
            {
                case 57:
                case 58:
                    btnListView_Click(this, e);
                    break;
            }
        }

        private void warmUpReport()
        {
            string ReportPath = GetReportPath();
            string reportFullPath = Path.Combine(ReportPath, "rptBlank.rpt");
            Debug.WriteLine("reportFullPath=" + reportFullPath);

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show("Report Path [" + reportFullPath + "]\n\nFile not found!");
                return;
            }

            try
            {
                using (ReportDocument rptViewer = new ReportDocument())
                {
                    rptViewer.Load(reportFullPath);

                    myViewer.ReportSource = rptViewer;
                    myViewer.Refresh();
                    myViewer.ReportSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n[" + reportFullPath + "]",
                    "Report could not be created", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Cursor.Current = Cursors.Default;
        }

    }
}
