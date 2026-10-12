using iText.StyledXmlParser.Jsoup.Internal;
using MIS.Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;

namespace MIS.Function
{
    internal sealed class BankTemplateExcelExporter
    {
        private const string DefaultVendor = "CITAS";
        private const string TemplateFolderName = "Templates";
        private const string TemplateFileName = "MIS_BLANK_TEMPLATE.xlsx";
        private const string TemplateWorksheetName = "Sheet1";
        private const string BankDataWorksheetName = "__MIS_BANK_DATA";
        private const string BankDataSchemaVersion = "1";
        private const int ExcelCellCharacterLimit = 32767;
        private static readonly string[] TargetHeaders =
        {
            "Request ID",
            "Vendor",
            "Request Date",
            "Requestor",
            "Request Type",
            "Request Prioritization",
            "POS Setup",
            "POS Type",
            "POS Connection Type",
            "Target Installation date",
            "Pullout date \n(applicable only to exhibit POS Request type)",
            "RM Instruction / Remarks",
            "Merchant ID (MID)",
            "Terminal ID (TID)",
            "Merchant Location/DBA Name",
            "Address",
            "City",
            "Area 1 (Metro Manila / Provincial)",
            "Area 2 (Region/Zone/Area code)",
            "Merchant Group",
            "Contact Person",
            "Contact Number",
            "Bancnet TID",
            "Bancnet MID",
            "Alipay TID",
            "Alipay MID",
            "Wechat TID",
            "Wechat MID",
            "QR-Gcash (TID)",
            "QR-Gcash (mid)",
            "QR-Grab (TID)",
            "QR-Grab (MID)",
            "QR-UPI (TID)",
            "QR-UPI (MID)",
            "QR-P2M (TID)",
            "QR-P2M (MID)",
            "Multi-merchant 1 - Name",
            "Multi-merchant 1 - TID",
            "Multi-merchant 1 - MID",
            "Multi-merchant 2 - Name",
            "Multi-merchant 2 - TID",
            "Multi-merchant 2 - MID",
            "Multi-merchant 3 - Name",
            "Multi-merchant 3 - TID",
            "Multi-merchant 3 - MID",
            "Multi-merchant 4 - Name",
            "Multi-merchant 4 - TID",
            "Multi-merchant 4 - MID",
            "Multi-merchant 5 - Name",
            "Multi-merchant 5 - TID",
            "Multi-merchant 5 - MID",
            "DCC\n(Yes/No)",
            "Installment\n(Yes/No)",
            "Manual Key Entry (MKE)\n(Yes/No)",
            "Preauth\n(Yes/No)",
            "Offline Sale\n(Yes/No)",
            "Offline Sale Limit\n(To be specified)",
            "Tip Adjust\n(Yes/No)",
            "Tip Adjust Limit\n(To be specified)",
            "Add-on Device \n(To be specified)",
            "ECR Integration\n(Yes/No)",
            "ECR Cable Type"
        };

        private static readonly string[] RequiredHeaders =
        {
            "Request ID",
            "Vendor",
            "Request Date",
            "Requestor",
            "Request Type",
            "Request Prioritization",
            "POS Setup",
            "POS Type",
            "POS Connection Type",
            "Target Installation date",
            "Merchant ID (MID)",
            "Terminal ID (TID)",
            "Merchant Location/DBA Name",
            "Address",
            "City",
            "Area 1 (Metro Manila / Provincial)",
            "Area 2 (Region/Zone/Area code)",
            "Contact Person",
            "Contact Number"
        };
        private static ExcelPackage OpenTemplatePackage()
        {
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TemplateFolderName, TemplateFileName);

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException("The MCC blank template was not found. Expected location:" + Environment.NewLine + templatePath, templatePath);
            }
            return new ExcelPackage(new FileInfo(templatePath));
        }

        private static ExcelWorksheet GetTemplateWorksheet(ExcelPackage package)
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[TemplateWorksheetName];
            if (worksheet == null)
            {
                throw new InvalidDataException("Worksheet '" + TemplateWorksheetName + "' was not found in the MCC blank template.");
            }
            ValidateTemplateHeaders(worksheet);
            return worksheet;
        }
        private static void ValidateTemplateHeaders(ExcelWorksheet worksheet)
        {
            for (int column = 1; column <= TargetHeaders.Length; column++)
            {
                string expectedHeader = NormalizeHeader(TargetHeaders[column - 1]);

                string actualHeader = NormalizeHeader(Convert.ToString(worksheet.Cells[1, column].Value));

                if (!string.Equals(expectedHeader, actualHeader, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidDataException("Invalid MCC Template header at Column " + column + ". Expected: '" + expectedHeader + "'. Actual: '" + actualHeader + "'.");
                }
            }
        }

        private static string NormalizeHeader(string value)
        {
            return (value ?? string.Empty).Replace("\r\n", "\n").Trim();
        }

        public IList<string> Export(string outputPath, IList<BankTemplateRow> rows, IDictionary<string, string> additionalValues)
        {
            if (string.IsNullOrWhiteSpace(outputPath)) throw new ArgumentException("Please select an output file.");

            if (rows == null) throw new ArgumentNullException("rows");

            Dictionary<string, string> values = BuildValues(rows);

            ApplyAdditionalValues(values, additionalValues);

            if (values.Count == 0)
            {
                throw new InvalidOperationException("No selected fields are mapped to the IR import template.");
            }

            string directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = OpenTemplatePackage())
            {
                ExcelWorksheet worksheet = GetTemplateWorksheet(package);
                WriteDataRow(worksheet, values);
                package.SaveAs(new FileInfo(outputPath));
            }
            return RequiredHeaders.Where(header => !values.ContainsKey(header) || string.IsNullOrWhiteSpace(values[header])).ToList();
        }

        public void Export(string outputPath, IList<IrImportRow> rows)
        {
            if (string.IsNullOrWhiteSpace(outputPath)) throw new ArgumentException("Please select an output file.");
            if (rows.Count == 0 || rows == null)
            {
                throw new InvalidOperationException("There are no Installation Request rows to export.");
            }

            string directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; using (ExcelPackage package = OpenTemplatePackage())
            {
                ExcelWorksheet worksheet = GetTemplateWorksheet(package);
                WriteDataRows(worksheet, rows);
                WriteBankDataMetadata(package, rows);
                package.SaveAs(new FileInfo(outputPath));
            }
        }

        private static void WriteBankDataMetadata(ExcelPackage package, IList<IrImportRow> rows)
        {
            if (package == null) throw new ArgumentNullException("package");
            if (rows == null) throw new ArgumentNullException("rows");
            ExcelWorksheet existingWorksheet = package.Workbook.Worksheets[BankDataWorksheetName];
            if (existingWorksheet != null)
            {
                throw new InvalidDataException("The MCC template contains the reserved worksheet '" + BankDataWorksheetName + "'.");
            }

            HashSet<string> requestIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                IrImportRow row = rows[rowIndex];

                int displayRowNumber = rowIndex + 1;

                if (row == null)
                {
                    throw new InvalidDataException("IR row " + displayRowNumber + " is invalid.");
                }

                string requestId = Clean(row.RequestId);

                if (string.IsNullOrWhiteSpace(requestId))
                {
                    throw new InvalidDataException("Request ID is required for IR row " + displayRowNumber + ".");
                }

                if (!requestIds.Add(requestId))
                {
                    throw new InvalidDataException("Duplicate Request ID detected: " + requestId);
                }

                if (string.IsNullOrWhiteSpace(row.SourceFileName))
                {
                    throw new InvalidDataException("Original filename is missing for Request ID " + requestId + ".");
                }

                if (string.IsNullOrWhiteSpace(row.BankDataInfo))
                {
                    throw new InvalidDataException("Bank data JSON is missing for Request ID " + requestId + ".");
                }

                if (row.BankDataInfo.Length > ExcelCellCharacterLimit)
                {
                    throw new InvalidDataException("Bank data JSON exceeds the Excel cell limit for " + "Request ID " + requestId + ".");
                }
            }

            ExcelWorksheet metadataWorksheet = package.Workbook.Worksheets.Add(BankDataWorksheetName);

            metadataWorksheet.Cells[1, 1].Value = "SchemaVersion";
            metadataWorksheet.Cells[1, 2].Value = "SourceBank";
            metadataWorksheet.Cells[1, 3].Value = "RequestID";
            metadataWorksheet.Cells[1, 4].Value = "SourceFileName";
            metadataWorksheet.Cells[1, 5].Value = "BankDataInfo";

            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                IrImportRow row = rows[rowIndex];

                int excelRow = rowIndex + 2;

                metadataWorksheet.Cells[excelRow, 1].Value = BankDataSchemaVersion;
                metadataWorksheet.Cells[excelRow, 2].Value = "BDO";
                metadataWorksheet.Cells[excelRow, 3].Value = Clean(row.RequestId);
                metadataWorksheet.Cells[excelRow, 4].Value = Path.GetFileName(row.SourceFileName);
                metadataWorksheet.Cells[excelRow, 5].Value = row.BankDataInfo;
            }
            metadataWorksheet.Hidden = eWorkSheetHidden.VeryHidden;
            package.Workbook.Worksheets.MoveBefore(BankDataWorksheetName, TemplateWorksheetName);
        }

        private static void WriteDataRows(ExcelWorksheet worksheet, IList<IrImportRow> rows)
        {
            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                int excelRow = rowIndex + 2;

                Dictionary<string, string> values = BuildValues(rows[rowIndex]);

                for (int column = 1; column <= TargetHeaders.Length; column++)
                {
                    string header = TargetHeaders[column - 1];
                    string value;

                    if (values.TryGetValue(header, out value))
                    {
                        worksheet.Cells[excelRow, column].Value = value;
                    }
                    worksheet.Cells[excelRow, column].Style.Font.Name = "Calibri";
                    worksheet.Cells[excelRow, column].Style.Font.Size = 10;
                    worksheet.Cells[excelRow, column].Style.Numberformat.Format = "@";
                }
            }
            ApplyThinBorder(worksheet.Cells[2, 1, rows.Count + 1, TargetHeaders.Length]);
        }

        private static Dictionary<string, string> BuildValues(IrImportRow row)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Request ID"] = Clean(row.RequestId),
                ["Vendor"] = string.IsNullOrWhiteSpace(row.Vendor) ? DefaultVendor : Clean(row.Vendor),
                ["Request Date"] = Clean(row.RequestDate),
                ["Requestor"] = Clean(row.Requestor),
                ["Request Type"] = Clean(row.RequestType),
                ["Request Prioritization"] = Clean(row.RequestPrioritization),
                ["POS Setup"] = Clean(row.PosSetup),
                ["POS Type"] = Clean(row.PosType),
                ["POS Connection Type"] = Clean(row.PosConnectionType),
                ["Target Installation date"] = Clean(row.TargetInstallationDate),
                ["RM Instruction / Remarks"] = Clean(row.Remarks),
                ["Merchant ID (MID)"] = Clean(row.Mid),
                ["Terminal ID (TID)"] = Clean(row.Tid),
                ["Bancnet TID"] = Clean(row.BancnetTid),
                ["Merchant Location/DBA Name"] = Clean(row.MerchantLocation),
                ["Address"] = Clean(row.Address),
                ["City"] = Clean(row.City),
                ["Area 1 (Metro Manila / Provincial)"] = Clean(row.Area1),
                ["Area 2 (Region/Zone/Area code)"] = Clean(row.Area2),
                ["Contact Person"] = Clean(row.ContactPerson),
                ["Contact Number"] = Clean(row.ContactNumber)
            };
        }
        private static string Clean(string value)
        {
            return (value ?? string.Empty).Trim();
        }

        private static void ApplyAdditionalValues(IDictionary<string, string> values, IDictionary<string, string> additionalValues)
        {
            if (additionalValues == null) return;

            HashSet<string> validHeaders = new HashSet<string>(TargetHeaders, StringComparer.OrdinalIgnoreCase);

            foreach (KeyValuePair<string, string> item in additionalValues)
            {
                string header = (item.Key ?? string.Empty).Trim();

                if (!validHeaders.Contains(header))
                {
                    throw new InvalidDataException("Unsupported IR import column: " + header);
                }
                values[header] = (item.Value ?? string.Empty).Trim();
            }
        }

        private static Dictionary<string, string> BuildValues(IEnumerable<BankTemplateRow> rows)
        {
            HashSet<string> validHeaders = new HashSet<string>(TargetHeaders, StringComparer.OrdinalIgnoreCase);

            Dictionary<string, string> values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (BankTemplateRow row in rows)
            {
                if (!row.IsSelectable || !row.IsSelected || string.IsNullOrWhiteSpace(row.MccColumn))
                {
                    continue;
                }

                string targetHeader = row.MccColumn.Trim();

                if (!validHeaders.Contains(targetHeader))
                {
                    throw new InvalidDataException("Unsupported IR import column: " + targetHeader);
                }

                string value = string.IsNullOrWhiteSpace(row.MccValue) ? row.Value : row.MccValue;

                values[targetHeader] = (value ?? string.Empty).Trim();
            }
            values["Vendor"] = DefaultVendor;
            return values;
        }

        private static void WriteDataRow(ExcelWorksheet worksheet, IDictionary<string, string> values)
        {
            for (int column = 1; column <= TargetHeaders.Length; column++)
            {
                string header = TargetHeaders[column - 1];
                string value;

                if (values.TryGetValue(header, out value))

                    worksheet.Cells[2, column].Value = value;
                worksheet.Cells[2, column].Style.Font.Name = "Calibri";
                worksheet.Cells[2, column].Style.Font.Size = 10;
                worksheet.Cells[2, column].Style.Numberformat.Format = "@";
            }
            ApplyThinBorder(worksheet.Cells[2, 1, 2, TargetHeaders.Length]);
        }

        private static void ApplyThinBorder(ExcelRange range)
        {
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }
    }
}