using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace MIS
{
    public class clsReportFunc
    {
        private clsFunction dbFunction = new clsFunction();

        public void ViewQRCode()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 12;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Employee QRCode";
                clsSearch.ClassSearchValue = clsGlobalVariables.IR_ACTIVE + clsFunction.sPipe + 
                                             clsGlobalVariables.iFE_Type + clsFunction.sPipe +
                                             clsSearch.ClassParticularID + clsFunction.sPipe +                                             
                                             clsSearch.ClassDepartmentID + clsFunction.sPipe +
                                             clsGlobalVariables.TIMESHEET_ON;

                clsSearch.ClassStoredProcedureName = "spViewAdvanceParticular";
                dbFunction.ProcessReport(clsSearch.ClassReportID);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewTimeSheet()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 13;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "TimeSheet";      
                clsSearch.ClassSearchValue = clsSearch.ClassParticularID + clsFunction.sPipe +
                                             clsSearch.ClassTSTerminalID + clsFunction.sPipe +
                                             clsSearch.ClassDepartmentID + clsFunction.sPipe +
                                             clsSearch.ClassDateFrom + clsFunction.sPipe +
                                             clsSearch.ClassDateTo + clsFunction.sPipe +
                                             clsSearch.ClassMissingTimeSheet ;

                clsSearch.ClassStoredProcedureName = "spViewTimeSheet";
                dbFunction.ProcessReport(clsSearch.ClassReportID);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewWorkArrangement()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 14;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Work Arrangement";
                clsSearch.ClassSearchValue = clsSearch.ClassParticularID + clsFunction.sPipe +                                             
                                             clsSearch.ClassDepartmentID;

                clsSearch.ClassStoredProcedureName = "spViewWorkArrangement";
                dbFunction.ProcessReport(clsSearch.ClassReportID);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewHoliday()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 15;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Holiday List";
                clsSearch.ClassSearchValue = clsGlobalVariables.IR_ACTIVE + clsFunction.sPipe;

                clsSearch.ClassStoredProcedureName = "spViewHoliday";
                dbFunction.ProcessReport(clsSearch.ClassReportID);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewLeaveApplication()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 16;
                clsSearch.ClassStatementType = "Search";
                clsSearch.ClassSearchBy = "Particular Leave Movement";
                clsSearch.ClassSearchValue = clsSearch.ClassParticularID + clsFunction.sPipe +                                             
                                             clsSearch.ClassDepartmentID + clsFunction.sPipe +
                                             clsSearch.ClassDateFrom + clsFunction.sPipe +
                                             clsSearch.ClassDateTo;

                clsSearch.ClassStoredProcedureName = "spViewLeaveDetail";
                dbFunction.ProcessReport(clsSearch.ClassReportID);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewLeaveAssignment()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 17;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Particular Leave Assignment";
                clsSearch.ClassSearchValue = clsSearch.ClassParticularID + clsFunction.sPipe +
                                             clsSearch.ClassDepartmentID + clsFunction.sPipe +
                                             clsSearch.ClassDateFrom + clsFunction.sPipe +
                                             clsSearch.ClassDateTo;

                clsSearch.ClassStoredProcedureName = "spViewLeaveDetail";
                dbFunction.ProcessReport(clsSearch.ClassReportID);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewFSRServicing()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 4;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Advance FSR";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassCurrentPage = int.Parse(clsFunction.sZero);
                clsSearch.ClassTotalPage = int.Parse(clsFunction.sZero);
                
                clsSearch.ClassStoredProcedureName = "spViewAdvanceFSR";
                dbFunction.ProcessReport(clsSearch.ClassReportID);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewSIMInventory()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 3;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Inventory Detail";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewSIMDetail";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewTerminalInventory()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 1;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Inventory Detail";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewTerminalDetail";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewFSR(int ReportID)
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            clsReport.ClassReportDesc = clsSearch.ClassReportType = clsSearch.ClassReportDescription = "FSR REPORT";
            
            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassIRNo + clsFunction.sPipe +
                                                clsSearch.ClassServiceNo + clsFunction.sPipe +
                                                clsSearch.ClassFSRNo + clsFunction.sPipe +
                                                clsSearch.ClassIRIDNo;

            try
            {
                clsSearch.ClassReportID = ReportID;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "FSR";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewAdvanceTA";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewFSRServicingV2()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsReport.ClassReportDesc = "CLIENT FSR REPORT";
                clsSearch.ClassReportID = 18;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "FSR For Client";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassCurrentPage = int.Parse(clsFunction.sZero);
                clsSearch.ClassTotalPage = int.Parse(clsFunction.sZero);

                clsSearch.ClassStoredProcedureName = "spViewAdvanceFSR";
                dbFunction.ProcessReport(clsSearch.ClassReportID);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewInstallationReport()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 9;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Installation Request";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewAdvanceIR";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewTerminalSummaryInventory()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 19;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Inventory Summary";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewTerminalDetail";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewSIMSummaryInventory()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 20;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Inventory Summary";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewSIMDetail";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewReleaseTerminalSummaryInventory()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 25;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Release Summary";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewTerminalDetail";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewReleaseTerminalDetailInventory()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 26;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Release Detail";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewTerminalDetail";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewFSROperation()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsReport.ClassReportDesc = "OPERATION FSR REPORT";
                clsSearch.ClassReportID = 23;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "FSR For Operation";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassCurrentPage = int.Parse(clsFunction.sZero);
                clsSearch.ClassTotalPage = int.Parse(clsFunction.sZero);

                clsSearch.ClassStoredProcedureName = "spViewAdvanceFSR";
                dbFunction.ProcessReport(clsSearch.ClassReportID);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewInstallationSummaryReport()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 8;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Installation Request Summary";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewAdvanceIR";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewReleaseSIMSummaryInventory()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 28;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Release Summary";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewSIMDetail";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewReleaseSIMDetailInventory()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 27;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Release Detail";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewSIMDetail";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewPOSRentalReport()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsReport.ClassReportDesc = clsSearch.ClassInvoiceNo + " - INVOICE REPORT";
                clsSearch.ClassReportID = 29;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Invoice Master/Detail";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewAdvanceDetail";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewTerminalImportReport()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 30;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Import Terminal";
                clsSearch.ClassSearchValue = clsFunction.sNull;
                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        // ROCKY - SIM IMPORT: ADD REPORT MESSAGE FOR SUCCESS AND DUPLICATE - REPORT SEARCH VALUE
        public void ViewSIMImportReport() {

            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try {

                clsSearch.ClassReportID = 31;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Import SIM";
                clsSearch.ClassSearchValue = clsFunction.sNull;
                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewTerminalTypeModelSummaryInventory()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 32;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Inventory Terminal Summary";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewTerminalDetail";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewSIMTelcoSummaryInventory()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 33;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Inventory Summary";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewSIMDetail";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        // ROCKY - PARTICULAR: ADD PARTICULAR DETAILS REPORT - REPORT SEARCH VALUE
        public void ViewParticularDetailReport()
        {

            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {

                clsSearch.ClassReportID = 34;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Particular Details";
                clsSearch.ClassSearchValue = clsFunction.sNull;
                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        // ROCKY - PARTICULAR: ADD PARTICULAR REQUIREMENTS DETAIL REPORT - REPORT SEARCH VALUE
        public void ViewParticularRequirementsReport()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {

                clsSearch.ClassReportID = 35;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Particular Details";
                clsSearch.ClassSearchValue = clsFunction.sNull;
                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        // ROCKY - BILLING: ADD SECURITY BANK BILLING DETAILS - REPORT SEARCH VALUE
        public void ViewBillingSecurityBankDetails()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {

                clsSearch.ClassReportID = 36;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Security Bank Billing Report";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;

                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        // ROCKY - BILLING: ADD METROBANK SERVICING DETAILS - REPORT SEARCH VALUE
        public void ViewMetrobankServicingDetails()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 37;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Mcc Service Report";
                clsSearch.ClassSearchValue = $"{clsSearch.ClassDateFrom}|{clsSearch.ClassDateTo}";

                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        // ROCKY - BILLING: ADD METROBANK SERVICING INVOICE - REPORT SEARCH VALUE
        public void ViewMetrobankServicingInvoice()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 1001;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Mcc Service Invoice";
                clsSearch.ClassSearchValue = $"{clsSearch.ClassDateFrom}|{clsSearch.ClassDateTo}";

                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        // ROCKY - BILLING: ADD METROBANK NET MATRIX - REPORT SEARCH VALUE
        public void ViewMetrobankNetMatrixDetails()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 38;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Mcc Net Matrix Report";
                clsSearch.ClassSearchValue = $"{clsSearch.ClassDateFrom}|{clsSearch.ClassDateTo}";

                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        // ROCKY - BILLING: ADD METROBANK NET INVOICE - REPORT SEARCH VALUE
        public void ViewMetrobankNetMatrixInvoice()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassReportID = 1002;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Mcc Net Matrix Invoice";
                clsSearch.ClassSearchValue = $"{clsSearch.ClassDateFrom}|{clsSearch.ClassDateTo}";

                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void WeePayInventoryReport()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassSearchValue ="";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void WeePaySimSummaryReport()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassSearchValue = "";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewServiceSummaryReport()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {   
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Service SLA Report";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewServiceDetailReport()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Service Detail Report";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewServiceInstallationSummaryReport()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Installation Summary Report";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewServiceInstallationDetailReport()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Installation Detail Report";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewServiceHistoryDetail(int ReportID)
        {
            Debug.WriteLine("--ViewServiceHistoryDetail--");

            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            clsReport.ClassReportDesc = clsSearch.ClassReportType = clsSearch.ClassReportDescription = "MERCHANT SERVICE HISTORY REPORT";

            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassClientID + clsFunction.sPipe + clsSearch.ClassIRIDNo;

            Debug.WriteLine("eFSRReport::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            try
            {
                clsSearch.ClassReportID = ReportID;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Service History Report";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewActiveTerminalReport()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Active Terminal";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewActiveTerminalDetailReport()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Active Detail Terminal";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewActiveSIMReport()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Active SIM";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewDiagnosticReport(int ReportID)
        {

            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            clsReport.ClassReportDesc = clsSearch.ClassReportType = clsSearch.ClassReportDescription = "DIAGNOSTIC REPORT";

            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassFSRNo + clsFunction.sPipe +
                                                clsSearch.ClassServiceNo;

            try
            {
                clsSearch.ClassReportID = ReportID;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "eFSR Diagnostic";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewAdvanceDetail";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewMonthlyServiceCountReport()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Monthly Service Count";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewServiceReport()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Service Attempt";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void ViewUnclosedServiceTicketReport()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Unclosed Service Ticket";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }
        
        public void ViewHelpdeskDetailReport()
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            try
            {
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "Helpdesk-Detail-Report";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewReport";
                dbFunction.ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }
    }
}
