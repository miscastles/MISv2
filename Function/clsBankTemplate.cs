using MIS.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MIS.Function
{
    #region BankTemplateFieldRules

    internal static class BankTemplateFieldRules
    {
        private static readonly Dictionary<string, string> SourceToMcc = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Nature of Request", "Request Type" },
            { "TID Issuance Type", "POS Type" },
            { "Requester's Name", "Requestor" },
            { "Merchant's DBA Name", "Merchant Location/DBA Name" },
            { "Merchant's DBA Address", "Address" },
            { "Merchant's DBA City", "City" },
            { "Merchant's DBA Province", "Area 1 (Metro Manila / Provincial)" },
            { "Store Representative", "Contact Person" },
            { "Contact Number (Outlet / Branch)", "Contact Number" },
            { "Required Date and Time of Installation", "Target Installation date" },
            { "Remarks / Special Instructions (Dispatch-related, etc.)", "RM Instruction / Remarks" }
        };

        private static readonly HashSet<string> JsonOnlyTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Prepared by",
            "Checked & Approved by",
            "Reference Credit TID",
            "Credit Straight MID-VMJACD"
        };

        public static void ApplyDefaults(IList<modelBankTemplate> rows)
        {
            ApplyDefaults(rows, false);
        }

        public static void ApplyDefaults(IList<modelBankTemplate> rows, bool isAdditionalTerminal)
        {
            if (rows == null) throw new ArgumentNullException("rows");

            foreach (modelBankTemplate row in rows)
            {
                string tag = CanonicalizeTag(row.Tag);
                string mccColumn;

                row.MccColumn = string.Empty;
                row.MccValue = string.Empty;

                if (TryGetIdentifierMccColumn(tag, isAdditionalTerminal, out mccColumn))
                {
                    row.MccColumn = mccColumn;
                    row.MccValue = IsAlipayIdentifierColumn(mccColumn)
                        ? (row.Value ?? string.Empty).Trim()
                        : NormalizeIdentifierList(row.Value);
                    row.IsSelected = row.IsSelectable;
                    continue;
                }

                if (SourceToMcc.TryGetValue(tag, out mccColumn))
                {
                    row.MccColumn = mccColumn;
                    row.MccValue = ConvertMccValue(tag, row.Value);
                    row.IsSelected = row.IsSelectable;
                    continue;
                }

                row.IsSelected = row.IsSelectable && (JsonOnlyTags.Contains(tag) || IsPreviousInformationRow(row));
            }
        }

        private static bool TryGetIdentifierMccColumn(string tag, bool isAdditionalTerminal, out string mccColumn)
        {
            mccColumn = string.Empty;

            if (string.Equals(tag, "VMJACD (catch-all) PHP", StringComparison.OrdinalIgnoreCase))
            {
                mccColumn = "Merchant ID (MID)";
                return true;
            }

            if (string.Equals(tag, "Debit TID", StringComparison.OrdinalIgnoreCase))
            {
                mccColumn = "Bancnet TID";
                return true;
            }

            if (string.Equals(tag, "Credit TID", StringComparison.OrdinalIgnoreCase))
            {
                mccColumn = "Terminal ID (TID)";
                return true;
            }

            if (IsAlipayTidTag(tag))
            {
                mccColumn = "Alipay TID";
                return true;
            }

            if (IsAlipayMidTag(tag))
            {
                mccColumn = "Alipay MID";
                return true;
            }

            if (isAdditionalTerminal && IsPreviousInformationIdentifier(tag, "MID"))
            {
                mccColumn = "Merchant ID (MID)";
                return true;
            }

            return false;
        }

        private static bool IsAlipayIdentifierColumn(string mccColumn)
        {
            return string.Equals(mccColumn, "Alipay TID", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(mccColumn, "Alipay MID", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsAlipayTidTag(string tag)
        {
            string normalizedTag = NormalizeMatchingTag(tag);

            return normalizedTag == "SWIFTPASSCHECKOUTSIGNKEY" ||
                   normalizedTag.EndsWith("ALIPAYTID", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsAlipayMidTag(string tag)
        {
            string normalizedTag = NormalizeMatchingTag(tag);

            return normalizedTag == "QRPHPLUSSWIFTPASSCHECKOUTMID" ||
                   normalizedTag.EndsWith("ALIPAYMID", StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeMatchingTag(string tag)
        {
            return Regex.Replace(tag ?? string.Empty, @"[^A-Za-z0-9]+", string.Empty).ToUpperInvariant();
        }

        private static bool IsPreviousInformationRow(modelBankTemplate row)
        {
            return row != null && (row.Section ?? string.Empty).StartsWith(
                BankTemplateReader.PreviousInformationSectionPrefix, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsPreviousInformationIdentifier(string tag, string identifier)
        {
            return (tag ?? string.Empty).EndsWith(" - " + identifier, StringComparison.OrdinalIgnoreCase);
        }

        public static string NormalizeIdentifierList(string sourceValue)
        {
            string value = (sourceValue ?? string.Empty).Trim();

            Match rangeMatch = Regex.Match(value, @"^(\d+)\s+to\s+(\d+)$", RegexOptions.IgnoreCase);

            if (rangeMatch.Success)
            {
                string startText = rangeMatch.Groups[1].Value;
                string endText = rangeMatch.Groups[2].Value;
                long start;
                long end;

                if (long.TryParse(startText, NumberStyles.None, CultureInfo.InvariantCulture, out start) &&
                    long.TryParse(endText, NumberStyles.None, CultureInfo.InvariantCulture, out end) &&
                    end >= start && end - start <= 1000)
                {
                    int width = Math.Max(startText.Length, endText.Length);
                    List<string> expanded = new List<string>();

                    for (long current = start; current <= end; current++)
                    {
                        expanded.Add(current.ToString(new string('0', width), CultureInfo.InvariantCulture));
                    }

                    return string.Join(", ", expanded);
                }
            }

            MatchCollection matches = Regex.Matches(value, @"\d+");

            if (matches.Count <= 1) return value;

            List<string> identifiers = new List<string>();
            HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (Match match in matches)
            {
                if (seen.Add(match.Value)) identifiers.Add(match.Value);
            }

            return string.Join(", ", identifiers);
        }

        private static string ConvertMccValue(string sourceTag, string sourceValue)
        {
            string tag = CanonicalizeTag(sourceTag);
            string value = (sourceValue ?? string.Empty).Trim();

            if (string.Equals(tag, "Nature of Request", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(value.Replace(" ", string.Empty), "TIDISSUANCE", StringComparison.OrdinalIgnoreCase))
            {
                return "New Installation";
            }

            if (string.Equals(tag, "TID Issuance Type", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(value.Replace(" ", string.Empty), "MINIPOS", StringComparison.OrdinalIgnoreCase))
            {
                return "Reprogramming";
            }

            return value;
        }

        public static bool TryGetMccColumn(string sourceTag, out string mccColumn)
        {
            return SourceToMcc.TryGetValue(CanonicalizeTag(sourceTag), out mccColumn);
        }

        public static string CanonicalizeTag(string tag)
        {
            string result = (tag ?? string.Empty).Trim();
            result = result.TrimEnd(':', '>');
            return Regex.Replace(result, @"\s+", " ");
        }

        public static string NormalizeJsonKey(string tag)
        {
            string result = CanonicalizeTag(tag);
            result = result.Replace("&", " and ");
            result = Regex.Replace(result, @"[^A-Za-z0-9]+", "_");
            return result.Trim('_');
        }
    }

    #endregion

    #region BankTemplateReader

    internal sealed class BankTemplateReadResult
    {
        public BankTemplateReadResult()
        {
            Rows = new List<modelBankTemplate>();
        }

        public IList<modelBankTemplate> Rows { get; set; }

        public bool IsAdditionalTerminal { get; set; }
    }

    internal sealed class BankTemplateReader
    {
        // internal (was private) so BankTemplateFieldRules can reuse the same prefix instead of a duplicate literal.
        internal const string PreviousInformationSectionPrefix = "Previous Merchant Information (";

        public BankTemplateReadResult Read(string filePath)
        {
            return Read(filePath, string.Empty);
        }

        public BankTemplateReadResult Read(string filePath, string workbookPassword)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("Please select a template.");

            if (!File.Exists(filePath)) throw new FileNotFoundException("The selected template was not found.", filePath);

            if (!string.Equals(Path.GetExtension(filePath), ".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                throw new NotSupportedException("Only .xlsx templates are supported.");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = OpenPackage(filePath, workbookPassword))
            {
                ExcelWorksheet worksheet = FindTidIssuanceWorksheet(package.Workbook);

                if (worksheet == null)
                {
                    throw new InvalidDataException("The workbook does not contain a supported 'TID Issuance Form' worksheet.");
                }

                if (worksheet.Dimension == null)
                {
                    throw new InvalidDataException("The 'TID Issuance Form' worksheet is empty.");
                }

                IList<modelBankTemplate> rows = ReadRows(worksheet);
                ExcelWorksheet previousInformationWorksheet = FindPreviousInformationWorksheet(package.Workbook, worksheet);

                bool isAdditionalTerminal = AppendPreviousMerchantInformation(previousInformationWorksheet, rows);

                return new BankTemplateReadResult
                {
                    Rows = rows,
                    IsAdditionalTerminal = isAdditionalTerminal
                };
            }
        }

        private static bool ContainsTidIssuanceTitle(ExcelWorksheet worksheet)
        {
            int maximumRow = Math.Min(worksheet.Dimension.End.Row, 10);
            int maximumColumn = Math.Min(worksheet.Dimension.End.Column, 10);

            for (int row = 1; row <= maximumRow; row++)
            {
                for (int column = 1; column <= maximumColumn; column++)
                {
                    string value = GetCellText(worksheet, row, column);

                    if (value.IndexOf("TID Issuance Form", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static ExcelWorksheet FindTidIssuanceWorksheet(ExcelWorkbook workbook)
        {
            foreach (ExcelWorksheet worksheet in workbook.Worksheets)
            {
                if (worksheet.Dimension != null && ContainsTidIssuanceTitle(worksheet))
                {
                    return worksheet;
                }
            }

            return null;
        }

        private static bool AppendPreviousMerchantInformation(ExcelWorksheet worksheet, IList<modelBankTemplate> rows)
        {
            if (worksheet == null || worksheet.Dimension == null) return false;

            int midColumn = FindHeaderColumn(worksheet, "MID");

            if (midColumn <= 0) return false;

            int dataRow = FindPreviousInformationDataRow(worksheet, midColumn);

            if (dataRow <= 0) return false;

            for (int column = 1; column <= worksheet.Dimension.End.Column; column++)
            {
                string header = GetCellText(worksheet, 1, column);
                string subHeader = dataRow > 2 ? GetCellText(worksheet, 2, column) : string.Empty;
                string value = GetCellText(worksheet, dataRow, column);

                if (string.IsNullOrWhiteSpace(header) || string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                rows.Add(new modelBankTemplate
                {
                    ExcelRowNumber = dataRow,
                    Section = PreviousInformationSectionPrefix + worksheet.Name + ")",
                    Tag = worksheet.Name + " - " + GetPreviousInformationTag(header, subHeader),
                    Value = value,
                    IsMandatory = false,
                    IsSelectable = true,
                    IsSelected = false
                });
            }

            return true;
        }

        private static string GetPreviousInformationTag(string header, string subHeader)
        {
            string combinedHeader = ((header ?? string.Empty) + " " + (subHeader ?? string.Empty)).Trim();

            if (combinedHeader.IndexOf("ALIPAY", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (combinedHeader.IndexOf("SWIFTPASS", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    combinedHeader.IndexOf("SIGNKEY", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "Alipay TID";
                }

                if (combinedHeader.IndexOf("MID", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "Alipay MID";
                }
            }

            return header;
        }

        private static ExcelWorksheet FindPreviousInformationWorksheet(ExcelWorkbook workbook, ExcelWorksheet posFormWorksheet)
        {
            foreach (ExcelWorksheet worksheet in workbook.Worksheets)
            {
                if (worksheet == posFormWorksheet || worksheet.Dimension == null)
                {
                    continue;
                }

                int midColumn = FindHeaderColumn(worksheet, "MID");

                if (midColumn > 0 && FindPreviousInformationDataRow(worksheet, midColumn) > 0)
                {
                    return worksheet;
                }
            }

            return null;
        }

        private static int FindHeaderColumn(ExcelWorksheet worksheet, string expectedHeader)
        {
            for (int column = 1; column <= worksheet.Dimension.End.Column; column++)
            {
                if (string.Equals(GetCellText(worksheet, 1, column), expectedHeader, StringComparison.OrdinalIgnoreCase))
                {
                    return column;
                }
            }

            return -1;
        }

        private static int FindPreviousInformationDataRow(ExcelWorksheet worksheet, int midColumn)
        {
            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                string mid = GetCellText(worksheet, row, midColumn);

                if (!string.IsNullOrWhiteSpace(mid))
                {
                    return row;
                }
            }

            return -1;
        }

        private static ExcelPackage OpenPackage(string filePath, string workbookPassword)
        {
            FileInfo file = new FileInfo(filePath);

            if (string.IsNullOrEmpty(workbookPassword)) return new ExcelPackage(file);
            return new ExcelPackage(file, workbookPassword);
        }

        private static IList<modelBankTemplate> ReadRows(ExcelWorksheet worksheet)
        {
            List<modelBankTemplate> result = new List<modelBankTemplate>();

            string currentSection = string.Empty;
            bool insideMandatorySection = false;

            for (int row = 1; row <= worksheet.Dimension.End.Row; row++)
            {
                string columnA = GetCellText(worksheet, row, 1);
                string columnB = GetCellText(worksheet, row, 2);
                string columnC = GetCellText(worksheet, row, 3);
                string columnE = GetCellText(worksheet, row, 5);

                if (IsSectionRow(columnA, columnB))
                {
                    currentSection = columnB;
                    insideMandatorySection = false;

                    result.Add(CreateDisplayRow(row, currentSection, string.Empty, columnB));
                    continue;
                }

                if (string.Equals(columnB, "MANDATORY FIELDS", StringComparison.OrdinalIgnoreCase))
                {
                    insideMandatorySection = true;
                    result.Add(CreateDisplayRow(row, currentSection, string.Empty, columnB));
                    continue;
                }

                if (IsApprovalHeader(columnB, columnC))
                {
                    string preparedValue = GetCellText(worksheet, row + 1, 2);
                    string approvedValue = GetCellText(worksheet, row + 1, 3);

                    result.Add(CreateValueRow(row, currentSection, "Prepared by:", preparedValue, string.Empty, true));
                    result.Add(CreateValueRow(row, currentSection, "Checked & Approved by>", approvedValue, string.Empty, true));
                    row++;
                    continue;
                }

                bool hasTag = !string.IsNullOrWhiteSpace(columnB);
                bool hasValue = !string.IsNullOrWhiteSpace(columnC);

                string displayedTag = columnB;

                // Captures titles such as "TID Issuance Form" from column A.
                if (string.IsNullOrWhiteSpace(displayedTag) && !string.IsNullOrWhiteSpace(columnA))
                {
                    displayedTag = columnA;
                }

                result.Add(new modelBankTemplate
                {
                    ExcelRowNumber = row,
                    Section = currentSection,
                    SourceRowNumber = IsRowNumber(columnA) ? columnA : string.Empty,
                    Tag = displayedTag,
                    Value = columnC,
                    MccColumn = columnE,
                    IsMandatory = insideMandatorySection && hasTag && hasValue,
                    IsSelectable = hasTag && hasValue,
                    IsSelected = false
                });
            }

            return result;
        }

        private static modelBankTemplate CreateDisplayRow(int excelRow, string section, string sourceRowNumber, string text)
        {
            return new modelBankTemplate
            {
                ExcelRowNumber = excelRow,
                Section = section,
                SourceRowNumber = sourceRowNumber,
                Tag = text,
                IsSelectable = false,
                IsSelected = false
            };
        }

        private static modelBankTemplate CreateValueRow(int excelRow, string section, string tag, string value, string mccColumn, bool isMandatory)
        {
            return new modelBankTemplate
            {
                ExcelRowNumber = excelRow,
                Section = section,
                Tag = tag,
                Value = value,
                MccColumn = mccColumn,
                IsMandatory = isMandatory,
                IsSelectable = !string.IsNullOrWhiteSpace(value),
                IsSelected = false
            };
        }

        private static bool IsSectionRow(string columnA, string columnB)
        {
            return string.Equals(columnA, "ROW", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(columnB);
        }

        private static bool IsApprovalHeader(string columnB, string columnC)
        {
            return columnB.IndexOf("Prepared by", StringComparison.OrdinalIgnoreCase) >= 0 &&
                   columnC.IndexOf("Approved by", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsRowNumber(string value)
        {
            int rowNumber;
            return int.TryParse(value, out rowNumber);
        }

        private static string GetCellText(ExcelWorksheet worksheet, int row, int column)
        {
            if (row > worksheet.Dimension.End.Row) return string.Empty;

            ExcelRange cell = worksheet.Cells[row, column];
            string displayedText = (cell.Text ?? string.Empty).Trim();

            // Excel's General format displays long numeric identifiers in scientific notation.
            // Preserve the complete whole-number value for MID/TID fields and similar IDs.
            if (displayedText.IndexOf('E') >= 0 && cell.Value != null)
            {
                double numericValue;

                if (double.TryParse(Convert.ToString(cell.Value, CultureInfo.InvariantCulture),
                        NumberStyles.Float, CultureInfo.InvariantCulture, out numericValue) &&
                    numericValue == Math.Truncate(numericValue))
                {
                    return numericValue.ToString("0", CultureInfo.InvariantCulture);
                }
            }

            return displayedText;
        }
    }

    #endregion

    #region BankTemplateJsonBuilder

    internal static class BankTemplateJsonBuilder
    {
        public static string Build(IList<modelBankTemplate> rows)
        {
            return Build(rows, string.Empty);
        }

        public static string Build(IList<modelBankTemplate> rows, string sourceFileName)
        {
            if (rows == null) throw new ArgumentNullException("rows");

            JObject values = new JObject();
            HashSet<string> usedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int selectedCount = 0;

            foreach (modelBankTemplate row in rows)
            {
                if (!row.IsSelectable || !row.IsSelected) continue;

                string baseKey = BankTemplateFieldRules.NormalizeJsonKey(row.Tag);

                if (string.IsNullOrWhiteSpace(baseKey)) continue;

                string uniqueKey = GetUniqueKey(baseKey, usedKeys);

                // Explicit string value.
                values.Add(uniqueKey, new JValue(row.Value ?? string.Empty));
                selectedCount++;
            }

            if (selectedCount == 0)
            {
                throw new InvalidOperationException("No fields are selected.");
            }

            JObject root = new JObject();

            if (!string.IsNullOrWhiteSpace(sourceFileName))
            {
                root["SourceBank"] = "BDO";
                root["SourceFileName"] = Path.GetFileName(sourceFileName);
            }

            root["Values"] = values;
            return root.ToString(Formatting.Indented);
        }

        private static string GetUniqueKey(string baseKey, ISet<string> usedKeys)
        {
            string key = baseKey;
            int suffix = 2;

            while (!usedKeys.Add(key))
            {
                key = baseKey + "_" + suffix;
                suffix++;
            }

            return key;
        }
    }

    #endregion

    #region BankTemplateExcelExporter

    internal sealed class BankTemplateExcelExporter
    {
        private const string DefaultVendor = "CITAS";
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

        // ---- Public API --------------------------------------------------------

        public IList<string> Export(string outputPath, IList<modelBankTemplate> rows, IDictionary<string, string> additionalValues)
        {
            if (string.IsNullOrWhiteSpace(outputPath)) throw new ArgumentException("Please select an output file.");
            if (rows == null) throw new ArgumentNullException("rows");

            Dictionary<string, string> values = BuildValues(rows);
            ApplyAdditionalValues(values, additionalValues);

            if (values.Count == 0)
            {
                throw new InvalidOperationException("No selected fields are mapped to the IR import template.");
            }

            EnsureOutputDirectory(outputPath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = OpenTemplatePackage())
            {
                ExcelWorksheet worksheet = GetTemplateWorksheet(package);
                WriteDataRow(worksheet, values);
                package.SaveAs(new FileInfo(outputPath));
            }

            return RequiredHeaders.Where(header => !values.ContainsKey(header) || string.IsNullOrWhiteSpace(values[header])).ToList();
        }

        public void Export(string outputPath, IList<modelBankTemplateIRRow> rows)
        {
            if (string.IsNullOrWhiteSpace(outputPath)) throw new ArgumentException("Please select an output file.");

            // null check must come before .Count, otherwise a null list throws NullReferenceException.
            if (rows == null || rows.Count == 0)
            {
                throw new InvalidOperationException("There are no Installation Request rows to export.");
            }

            EnsureOutputDirectory(outputPath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = OpenTemplatePackage())
            {
                ExcelWorksheet worksheet = GetTemplateWorksheet(package);
                WriteDataRows(worksheet, rows);
                WriteBankDataMetadata(package, rows);
                package.SaveAs(new FileInfo(outputPath));
            }
        }

        // ---- Template ----------------------------------------------------------

        private static ExcelPackage OpenTemplatePackage()
        {
            string templatePath = Path.Combine(new clsFile().sTemplatePath, TemplateFileName);

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

        private static void EnsureOutputDirectory(string outputPath)
        {
            string directory = Path.GetDirectoryName(outputPath);

            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        // ---- Single-row export (from modelBankTemplate rows) -------------------

        private static Dictionary<string, string> BuildValues(IEnumerable<modelBankTemplate> rows)
        {
            HashSet<string> validHeaders = new HashSet<string>(TargetHeaders, StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (modelBankTemplate row in rows)
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
            values["Bancnet MID"] = modelBankTemplateIRRow.DefaultBancnetMid;
            return values;
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

        private static void WriteDataRow(ExcelWorksheet worksheet, IDictionary<string, string> values)
        {
            for (int column = 1; column <= TargetHeaders.Length; column++)
            {
                string header = TargetHeaders[column - 1];
                string value;

                if (values.TryGetValue(header, out value))
                {
                    worksheet.Cells[2, column].Value = value;
                }

                worksheet.Cells[2, column].Style.Font.Name = "Calibri";
                worksheet.Cells[2, column].Style.Font.Size = 10;
                worksheet.Cells[2, column].Style.Numberformat.Format = "@";
            }

            ApplyThinBorder(worksheet.Cells[2, 1, 2, TargetHeaders.Length]);
        }

        // ---- Multi-row export (from IrImportRow list) --------------------------

        private static Dictionary<string, string> BuildValues(modelBankTemplateIRRow row)
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
                ["Bancnet MID"] = modelBankTemplateIRRow.DefaultBancnetMid,
                ["Alipay TID"] = Clean(row.AlipayTid),
                ["Alipay MID"] = Clean(row.AlipayMid),
                ["Merchant Location/DBA Name"] = Clean(row.MerchantLocation),
                ["Address"] = Clean(row.Address),
                ["City"] = Clean(row.City),
                ["Area 1 (Metro Manila / Provincial)"] = Clean(row.Area1),
                ["Area 2 (Region/Zone/Area code)"] = Clean(row.Area2),
                ["Contact Person"] = Clean(row.ContactPerson),
                ["Contact Number"] = Clean(row.ContactNumber)
            };
        }

        private static void WriteDataRows(ExcelWorksheet worksheet, IList<modelBankTemplateIRRow> rows)
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

        private static void WriteBankDataMetadata(ExcelPackage package, IList<modelBankTemplateIRRow> rows)
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
                modelBankTemplateIRRow row = rows[rowIndex];
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
                    throw new InvalidDataException("Bank data JSON exceeds the Excel cell limit for Request ID " + requestId + ".");
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
                modelBankTemplateIRRow row = rows[rowIndex];
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

        // ---- Helpers -----------------------------------------------------------

        private static string Clean(string value)
        {
            return (value ?? string.Empty).Trim();
        }

        private static void ApplyThinBorder(ExcelRange range)
        {
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }
    }

    #endregion
}
