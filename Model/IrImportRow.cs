using CrystalDecisions.CrystalReports.Engine;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.Office.Core;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Ocsp;

namespace MIS.Model
{
    // Generated Installation Request row used by the Bank Template Generator preview and export.
    internal sealed class IrImportRow
    {
       public IrImportRow()
        {
            SourceFilePath = string.Empty;
            SourceFileName = string.Empty;
            BankDataInfo = string.Empty;
            RequestId = string.Empty;
            Vendor = "CITAS";
            RequestDate = string.Empty;
            Requestor = string.Empty;
            RequestType = string.Empty;
            RequestPrioritization = string.Empty;
            PosSetup = string.Empty;
            PosType = string.Empty;
            PosConnectionType = string.Empty;
            TargetInstallationDate = string.Empty;
            Remarks = string.Empty;
            Mid = string.Empty;
            Tid = string.Empty;
            BancnetTid = string.Empty;
            MerchantLocation = string.Empty;
            Address = string.Empty;
            City = string.Empty;
            Area1 = string.Empty;
            Area2 = string.Empty;
            ContactPerson = string.Empty;
            ContactNumber = string.Empty;
        }

        public string SourceFilePath { get; set; }

        public string SourceFileName { get; set; }

        public string BankDataInfo { get; set; }

        public string RequestId { get; set; }

        public string Vendor { get; set; }

        public string RequestDate { get; set; }

        public string Requestor { get; set; }

        public string RequestType { get; set; }

        public string RequestPrioritization { get; set; }

        public string PosSetup { get; set; }

        public string PosType { get; set; }

        public string PosConnectionType { get; set; }

        public string TargetInstallationDate { get; set; }

        public string Remarks { get; set; }

        public string Mid { get; set; }

        public string Tid { get; set; }

        public string BancnetTid { get; set; }

        public string MerchantLocation { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Area1 { get; set; }

        public string Area2 { get; set; }

        public string ContactPerson { get; set; }

        public string ContactNumber { get; set; }
    }

}
