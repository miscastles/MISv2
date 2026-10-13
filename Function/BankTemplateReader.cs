using MIS.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;

namespace MIS.Function
{
    internal sealed class BankTemplateReadResult
    {
        public BankTemplateReadResult()
        {
            Rows = new List<BankTemplateRow>();
        }

        public IList<BankTemplateRow> Rows { get; set; }

        public bool IsAdditionalTerminal { get; set; }
    }

    internal sealed class BankTemplateReader
    {
        private const string PreviousInformationSectionPrefix = "Previous Merchant Information (";

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

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; using(ExcelPackage package = OpenPackage(filePath, workbookPassword))
            {
                ExcelWorksheet worksheet = FindTidIssuanceWorksheet(package.Workbook);

                if (worksheet == null)
                {
                    throw new InvalidDataException("The workbook does not contain a supported " +
                        "'TID Issuance Form' worksheet.");
                }
                if (worksheet.Dimension == null)
                {
                    throw new InvalidDataException("The 'TID Issuance Form' worksheet is empty.");
                }

                IList<BankTemplateRow> rows = ReadRows(worksheet);
                ExcelWorksheet previousInformationWorksheet = FindPreviousInformationWorksheet(package.Workbook, worksheet);

                bool isAdditionalTerminal = AppendPreviousMerchantInformation(previousInformationWorksheet,rows);

                return new BankTemplateReadResult
                {
                    Rows = rows, IsAdditionalTerminal = isAdditionalTerminal
                };
            }
        }

        private static bool ContainsTidIssuanceTitle(ExcelWorksheet worksheet)
        {
            int maximumRow = Math.Min(worksheet.Dimension.End.Row,10);

            int maximumColumn = Math.Min(worksheet.Dimension.End.Column,10);

            for (int row = 1; row <= maximumRow; row++)
            {
                for (int column = 1;
                     column <= maximumColumn;
                     column++)
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

        private static bool AppendPreviousMerchantInformation(ExcelWorksheet worksheet, IList<BankTemplateRow> rows)
        {
            if (worksheet == null || worksheet.Dimension == null) return false;

            int midColumn = FindHeaderColumn(worksheet, "MID");

            if (midColumn <= 0) return false;

            int dataRow = FindPreviousInformationDataRow(worksheet, midColumn);

            if (dataRow <= 0) return false;

            for (int column = 1; column <= worksheet.Dimension.End.Column; column++)
            {
                string header = GetCellText(worksheet, 1, column);
                string value = GetCellText(worksheet, dataRow, column);

                if (string.IsNullOrWhiteSpace(header) || string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                rows.Add(new BankTemplateRow
                {
                    ExcelRowNumber = dataRow,
                    Section = PreviousInformationSectionPrefix + worksheet.Name + ")",
                    Tag = worksheet.Name + " - " + header,
                    Value = value,
                    IsMandatory = false,
                    IsSelectable = true,
                    IsSelected = false
                });
            }
            return true;
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

        private static IList<BankTemplateRow> ReadRows(ExcelWorksheet worksheet)
        {
            List<BankTemplateRow> result = new List<BankTemplateRow>();

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

                result.Add(new BankTemplateRow
                {
                    ExcelRowNumber = row,
                    Section = currentSection,
                    SourceRowNumber = IsRowNumber(columnA) ? columnA : string.Empty,
                    Tag = displayedTag,
                    Value = columnC,
                    MccColumn = columnE,
                    IsMandatory = insideMandatorySection && hasTag &&hasValue,
                    IsSelectable = hasTag && hasValue,
                    IsSelected = false
                });
            }
            return result;
        }

        private static BankTemplateRow CreateDisplayRow(int excelRow, string section, string sourceRowNumber, string text)
        {
            return new BankTemplateRow
            {
                ExcelRowNumber = excelRow,
                Section = section,
                SourceRowNumber = sourceRowNumber,
                Tag = text,
                IsSelectable = false,
                IsSelected = false
            };
        }

        private static BankTemplateRow CreateValueRow(int excelRow, string section, string tag, string value, string mccColumn, bool isMandatory)
        {
            return new BankTemplateRow
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

            return (worksheet.Cells[row, column].Text ?? string.Empty).Trim();
        }
    }
}
