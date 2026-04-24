using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Windows.Forms;
using MIS.AppConnection;
using MIS.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MIS.Function.AppUtilities;
using static MIS.AppActivity.BillingActivity;

namespace MIS.AppMainActivity
{
    class AppReports
    {
        private ServerDatabase Db;

        // Request Variables
        public string StoredProcedure { get; set; }
        public string StatementType { get; set; }
        public string SearchBy { get; set; }
        public string SearchValue { get; set; }
        public string Sql { get; set; }


        // Request
        public async Task<DataTable> GetsDetails()
        {
            // Initialize the database with default values
            Db = new ServerDatabase
            {
                Sprocedure = "spViewReport",
                Statement  = StatementType,
                SearchBy   = SearchBy,
                SearchVal  = SearchValue,
                Sql        = Sql,
            };

            // Call ExeAsyncProcedure and return the result
            return await Db.ExeAsyncProcedure();
        }

        public static void ReportPreview(int ReportID)
        {
            frmReportViewer.sReportID = ReportID;
            frmReportViewer frm = new frmReportViewer();
            frm.ShowDialog();
        }

        public static void setInvoiceByDataTable(int ReportID, DataTable Dt)
        {
            frmReportViewer.sReportID = ReportID;
            frmReportViewer frm = new frmReportViewer();
            frm.MccNetMatrixDetails = Dt;
            frm.ShowDialog();
        }


        // Text Objects
        private void SetHeaderText(ReportClass rptFile)
        {
            List<string> headerTexts = new List<string>
            {
                clsHeader.ClassName,
                clsHeader.ClassHeader1,
                clsHeader.ClassHeader2,
                clsHeader.ClassHeader3,
                clsHeader.ClassHeader4
            };

            setReportHeader(rptFile, "txtHeader", headerTexts);
        }

        private void SetDateFromTo(ReportClass rptFile)
        {
            // Set date range text for receipt
            setTextForObject(rptFile, "txtDateFromTo", $"TA Date: {parseStrDate(clsSearch.ClassDateFrom)} to {parseStrDate(clsSearch.ClassDateTo)}");
        }

        private void SetReceiptUser(ReportClass rptFile)
        {
            setTextForObject(rptFile, "txtUserFullName", clsUser.ClassUserFullName);
        }

        private void setReferenceInfo(ReportClass rptFile)
        {
            setTextForObject(rptFile, "txtInvoiceDate", RefNo);
            setTextForObject(rptFile, "txtRefDate", RefDate);
            setTextForObject(rptFile, "txtTerms", RefTerms);
            setTextForObject(rptFile, "TxtDueDate", DueDate);
            //setTextForObject(rptFile, "TxtDesc", Description);
            //setTextForObject(rptFile, "TxtDiscount", Discount.ToString());
        }

        private void SetDateRange(ReportClass rptFile)
        {
            string dateRangeText = $"{clsSearch.ClassDateFrom} To {clsSearch.ClassDateTo}";

            setTextForObject(rptFile, "txtDateRange", dateRangeText);           
        }

        // Initialized All Text Objects
        private void InitializedObjects(ReportClass rptFile)
        {
            // For Headers
            SetHeaderText(rptFile);

            // For User who generate  the report
            SetReceiptUser(rptFile);

            // For Date Range display
            SetDateRange(rptFile);

            // For Date From to Display
            SetDateFromTo(rptFile);

            // For Invoicing Ref Date. Display
            setReferenceInfo(rptFile);
        }

        // Reusable Text Object
        private void setReportHeader(ReportClass rpt, string ObjHeader, List<string> headerTexts)
        {
            for (int i = 1; i <= headerTexts.Count; i++)
            {
                if (isObjectExists(rpt, $"{ObjHeader}{i}"))
                {
                    setTextForObject(rpt, $"{ObjHeader}{i}", (i <= headerTexts.Count) ? headerTexts[i - 1] : string.Empty);
                }
            }
        }

        private void setTextForObject(ReportClass rpt, string objName, string text)
        {
            if (isObjectExists(rpt, objName))
            {
                if (rpt.ReportDefinition.ReportObjects[objName] is TextObject textObject && textObject != null)
                {
                    // Ensure that the Text property is not null
                    textObject.Text = text;
                }
            }
        }

        // Check Report Text Object
        private bool isObjectExists(ReportClass rpt, string objName)
        {
            // Check Object using Linq
            bool objectExists = rpt.ReportDefinition.ReportObjects.Cast<ReportObject>().Any(reportObject => reportObject.Name == objName);

            if (!objectExists)
            {
                Debug.WriteLine($"Warning: Object with name '{objName}' not found in the report.");
                
            }

            return objectExists;
        }

        // Date Parser
        private string parseStrDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return string.Empty;

            if (DateTime.TryParse(dateString, out DateTime dt))
                return dt.ToString("MM-dd-yyyy");

            return dateString;
        }

        // Report Process
        private async void LoadReport(CrystalReportViewer rptViewer, ReportClass rptFile, string rptfileName)
        {
            string reportFullPath = Path.Combine("C:\\CASTLESTECH_MIS\\REPORTS\\", rptfileName);
            Debug.WriteLine($"\nRerport Path: {reportFullPath}");

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show($"\nReport Path [{reportFullPath}] \nFile not found!");
                return;
            }

            try
            {
                //DataSet Ds = GetReportDetails();

                DataTable Dt = await GetsDetails();

                // Check Result
                if (Dt == null) {
                    MessageBox.Show("Invalid Report Details. Check parameters", "Report could not be created", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; }

                // Process Report
                rptFile.Load(reportFullPath);
                rptFile.SetDataSource(Dt);

                // Init Objects
                InitializedObjects(rptFile);

                // Load Report
                rptViewer.ReportSource = rptFile;
                rptViewer.ToolPanelView = ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n[{reportFullPath}]", "Report could not be created", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void getReportDetails(CrystalReportViewer rptViewer, ReportClass rptFile, string rptfileName, DataTable Dt)
        {
            string reportFullPath = Path.Combine("C:\\CASTLESTECH_MIS\\REPORTS\\", rptfileName);
            Debug.WriteLine($"\nRerport Path: {reportFullPath}");

            if (!File.Exists(reportFullPath))
            {
                MessageBox.Show($"\nReport Path [{reportFullPath}] \nFile not found!");
                return;
            }

            try
            {
                //DataSet Ds = GetReportDetails();

                // Check Result
                if (Dt == null)
                {
                    MessageBox.Show("Invalid Report Details. Check parameters", "Report could not be created", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Process Report
                rptFile.Load(reportFullPath);
                rptFile.SetDataSource(Dt);

                // Init Objects
                InitializedObjects(rptFile);

                // Load Report
                rptViewer.ReportSource = rptFile;
                rptViewer.ToolPanelView = ToolPanelViewType.None; // hide toggle tree view

            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n[{reportFullPath}]", "Report could not be created", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        // Set details and generate report
        private static void GenerateReport(string searchBy, string searchValue, string Sql, ReportClass reportDocs, CrystalReportViewer rptViewer)
        {
            var Apps = new AppReports()
            {
                StatementType = "View",
                SearchBy = searchBy,
                SearchValue = searchValue,
                Sql = Sql
            };

            Apps.LoadReport(rptViewer, reportDocs, $"{reportDocs.GetType().Name}.rpt");
        }

        private static async Task<DataTable> GenerateExcel(string pSearchBy, string pSearchValue)
        {
            var Apps = new AppReports()
            {
                StatementType = "View",
                SearchBy      = pSearchBy,
                SearchValue   = pSearchValue,
                Sql = ""
            };

            return await Apps.GetsDetails();
        }

        // REPORTS
        public static class Summary
        {
            public static void Leasing(CrystalReportViewer rptViewer)
            {
                GenerateReport("Leasing-Summary", clsSearch.ClassSearchValue, "", new rptBillingLeasingSummary(), rptViewer);
            }

            public static void Services(CrystalReportViewer rptViewer)
            {
                GenerateReport("Service-Summary", clsSearch.ClassSearchValue, "", new rptBillingServicingSummary(), rptViewer);
            }

            public static void WeePayInventory(CrystalReportViewer rptViewer)
            {
                GenerateReport("Weepay Inventory Report", clsSearch.ClassSearchValue, "", new rptBillingWeePayInventory(), rptViewer);
            }

            public static void WeePaySimDetails(CrystalReportViewer rptViewer)
            {
                GenerateReport("Weepay Sim Summary Report", clsSearch.ClassSearchValue, "", new rptBillingWeePaySimDetails(), rptViewer);
            }

            public static void HelpdeskServiceReport(CrystalReportViewer rptViewer)
            {
                GenerateReport("Helpdesk-Service-Report", clsSearch.ClassSearchValue, "", new rptHelpdeskFSR(), rptViewer);
            }

        }

        public static class Invoice
        {
            public static void Leasing(CrystalReportViewer rptViewer)
            {
                GenerateReport("Leasing-Invoice", clsSearch.ClassSearchValue, "", new rptBillingLeasingInvoice(), rptViewer);
            }

            public static void Services(CrystalReportViewer rptViewer)
            {
                GenerateReport("Service-Invoice", clsSearch.ClassSearchValue, "", new rptBillingServicingInvoice(), rptViewer);
            }

            public static void Tle(CrystalReportViewer rptViewer)
            {
                GenerateReport("TLE-Invoice", clsSearch.ClassSearchValue, "", new rptBillingTleInvoice(), rptViewer);
            }

            public static void WareHouse(CrystalReportViewer rptViewer)
            {
                GenerateReport("Warehouse-Invoice", clsSearch.ClassSearchValue, "", new rptBillingTleInvoice(), rptViewer);
            }

            public static void WeePaySimInvoice(CrystalReportViewer rptViewer)
            {
                GenerateReport("Weepay Sim Invoice", "", "", new rptBillingWeePaySimInvoice(), rptViewer);
            }

        }
 
        public static class ExportExcel
        {
            public static async void MccNetMatrixStr(string SearchValue)
            {
               ExportDtToExcel(await GenerateExcel("Leasing-Summary", SearchValue));
            }

            public static void ExportData(DataTable Dt)
            {
                ExportDtToExcel(Dt);
            }
        }

        // Add Onother Group here ...
    }
}
