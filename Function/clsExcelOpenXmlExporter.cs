using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace MIS.Function
{
    public class clsExcelOpenXmlExporter
    {
        private clsFile dbFile;
        private clsFunction dbFunction;
        private clsAPI dbAPI;

        public void ExportListViewToExcelWithTabSheet(ListView listView, string filePath, Dictionary<string, DataSet> dataSets, Dictionary<string, string> tabJsonTabMenu)
        {
            // Debugging...
            Debug.WriteLine("--ExportListViewToExcelWithTabSheet--");
            Debug.WriteLine($"Report Type:{clsReport.ClassReportDesc}");
            foreach (var kvp in tabJsonTabMenu)
            {
                Debug.WriteLine($"tabJsonTabMenu -> Key: {kvp.Key}, Value: {kvp.Value}");
            }
            Debug.WriteLine($"filePath:{filePath}");
            Debug.WriteLine($"dataSets count:{dataSets.Count}");
            // Debugging...

            Cursor.Current = Cursors.WaitCursor;

            dbFile = new clsFile();
            dbFunction = new clsFunction();
            clsSearch.ClassExportStartTime = dbFunction.GetRequestTime(clsDefines.gNull);

            try
            {
                if (dataSets == null || dataSets.Count == 0)
                    return;

                // Excluded columns from JSON
                string excludeFilePath = dbFile.sRespFullPath + "respExcludeExportColumn.json";
                HashSet<string> excludedColumnsSet = dbFile.loadExcludedColumns(excludeFilePath);

                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                    saveFileDialog.Title = "Save as Excel File";
                    saveFileDialog.InitialDirectory = dbFile.sExportPath;
                    saveFileDialog.FileName = filePath;
                    saveFileDialog.FilterIndex = 1;
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() != DialogResult.OK)
                        return;

                    string finalPath = saveFileDialog.FileName;
                    FileInfo excelFile = new FileInfo(finalPath);

                    if (dbFile.IsFileOpen(excelFile.FullName))
                    {
                        MessageBox.Show("The file is already open. Please close it and try again.",
                            "File in Use", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Create spreadsheet document (OpenXML)
                    using (SpreadsheetDocument document = SpreadsheetDocument.Create(finalPath, SpreadsheetDocumentType.Workbook))
                    {
                        WorkbookPart workbookPart = document.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();

                        // Add styles
                        WorkbookStylesPart stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                        stylesPart.Stylesheet = CreateStylesheet();
                        stylesPart.Stylesheet.Save();

                        Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());
                        uint sheetId = 1;

                        // For each tab
                        foreach (var tab in tabJsonTabMenu)
                        {
                            string tabKey = tab.Key;
                            string tabName = tab.Value;

                            if (!dataSets.ContainsKey(tabKey) || dataSets[tabKey].Tables.Count == 0)
                                continue;

                            DataSet ds = dataSets[tabKey];

                            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                            string relationshipId = workbookPart.GetIdOfPart(worksheetPart);

                            // Use OpenXmlWriter for rows
                            using (OpenXmlWriter writer = OpenXmlWriter.Create(worksheetPart))
                            {
                                writer.WriteStartElement(new Worksheet());

                                // Hide col A for Summary
                                Columns columns = new Columns();
                                if (tabName == "Summary")
                                    columns.Append(new Column() { Min = 1, Max = 1, Hidden = true });
                                writer.WriteElement(columns);

                                writer.WriteStartElement(new SheetData());

                                string previousBValue = "";
                                uint currentRowIndex = 1;

                                foreach (DataTable table in ds.Tables)
                                {
                                    var includedColumns = table.Columns.Cast<DataColumn>()
                                                            .Where(c => !excludedColumnsSet.Contains(c.ColumnName))
                                                            .ToList();

                                    bool writeHeader = tabName != "Summary";

                                    // Header
                                    if (writeHeader)
                                    {
                                        writer.WriteStartElement(new Row());
                                        foreach (var col in includedColumns)
                                            writer.WriteElement(CreateTextCell(col.ColumnName, 1)); // header style
                                        writer.WriteEndElement();
                                        currentRowIndex++;
                                    }
                                    
                                    // Data rows
                                    foreach (DataRow row in table.Rows)
                                    {
                                        writer.WriteStartElement(new Row());
                                        int excelColPos = 1;

                                        string colAValue = row[0]?.ToString() ?? "";
                                        string colBValue = row.Table.Columns.Count > 1 ? row[1]?.ToString() ?? "" : "";
                                        string colCValue = row.Table.Columns.Count > 2 ? row[2]?.ToString() ?? "" : "";

                                        uint rowStyle = 0;

                                        // ---------- Summary Sheet Styling (EPPlus logic) ----------
                                        if (tabName == "Summary")
                                        {
                                            if (colBValue == "GRAND TOTAL" ||
                                                (colAValue == "7" && colBValue == "CURRENT SLA %") ||
                                                (colAValue == "2" && colBValue.StartsWith("OVERALL REQUESTS") && clsSearch.ClassReportID == 53) ||
                                                (colAValue == "3" && colBValue == "OVERALL REQUESTS") ||
                                                (colAValue == "4" && colBValue == "REQUEST WITHIN SLA") ||
                                                (colAValue == "6" && colBValue == "REQUEST PER TEAM LEAD") ||
                                                (colAValue == "1" && colBValue == "NEGATIVE/UNSUCCESSFUL ACTIVITY"))
                                            {
                                                rowStyle = 2; // Dark Blue
                                            }
                                            else if (colAValue == "0" && colBValue == "-")
                                            {
                                                rowStyle = 4; // White row (hidden)
                                            }
                                            else if (((colAValue == "3" || colAValue == "4") && colCValue == "TOTAL") ||
                                                     (colAValue == "7" && (colBValue != "CURRENT SLA %" && colBValue != "GRAND TOTAL")))
                                            {
                                                rowStyle = 3; // Light Green
                                            }
                                        }

                                        foreach (DataColumn col in includedColumns)
                                        {
                                            //string cellRawValue = row[col]?.ToString() ?? "";
                                            string cellRawValue = CleanXmlString(row[col]?.ToString() ?? "");
                                            uint cellStyle = rowStyle;

                                            // --- Special highlight for "REQUEST PER TEAM LEAD" row ---
                                            if (tabName == "Summary" && colAValue == "6" && colBValue == "REQUEST PER TEAM LEAD")
                                            {
                                                if (excelColPos >= 4 && excelColPos <= 9) cellStyle = 5; // Green
                                                if (excelColPos == 10 || excelColPos == 11) cellStyle = 6; // Orange
                                                if (excelColPos >= 12 && excelColPos <= 15) cellStyle = 7; // Red
                                            }

                                            // --- Special highlight for "REQUEST PER TEAM LEAD GRAND TOTAL" ---
                                            if (tabName == "Summary" && colAValue == "6" && colBValue == "GRAND TOTAL" &&
                                                excelColPos >= 4 && excelColPos <= 15)
                                            {
                                                cellStyle = 5; // Green
                                            }

                                            // --- Deduplicate Column B (replace repeat values with "-") ---
                                            if (tabName == "Summary" && excelColPos == 2)
                                            {
                                                if (cellRawValue == previousBValue && cellRawValue != "")
                                                    cellRawValue = "-";
                                                else
                                                    previousBValue = cellRawValue;
                                            }

                                            // Create styled cell
                                            var cell = CreateTextCell(cellRawValue, cellStyle);
                                            writer.WriteElement(cell);

                                            excelColPos++;
                                        }

                                        writer.WriteEndElement(); // Row
                                        currentRowIndex++;
                                    }

                                }

                                writer.WriteEndElement(); // SheetData
                                writer.WriteEndElement(); // Worksheet
                                writer.Close();
                            }
                            
                            // Add sheet entry
                            sheets.Append(new Sheet()
                            {
                                Id = relationshipId,
                                SheetId = sheetId++,
                                Name = tabName.Length > 31 ? tabName.Substring(0, 31) : tabName
                            });
                        }

                        // ✅ Auto size every sheet at once
                        AutoSizeAllWorksheets(workbookPart, true);

                        workbookPart.Workbook.Save();
                    }

                    // Update API + show summary (kept same as original)
                    string pSearchValue = $"{clsSearch.ClassReportType}{clsDefines.gPipe}{(int)Enums.ReportProcessType.Idle}{clsDefines.gPipe}" +
                                          $"{clsDefines.REPORT_STATUS_IDLE}{clsDefines.gPipe}" +
                                          $"{dbFunction.getCurrentDateTime()}{clsDefines.gPipe}" +
                                          $"{clsUser.ClassUserFullName}";

                    dbAPI = new clsAPI();
                    dbAPI.ExecuteAPI("PUT", "Update", "Report Status", pSearchValue, "", "", "UpdateCollectionDetail");

                    clsSearch.ClassExportEndTime = dbFunction.GetResponseTime(clsDefines.gNull);

                    // Durations message (same as original)
                    DateTime processStartTime = DateTime.Parse(clsSearch.ClassProcessStartTime);
                    DateTime processEndTime = DateTime.Parse(clsSearch.ClassProcessEndTime);
                    DateTime exportStartTime = DateTime.Parse(clsSearch.ClassExportStartTime);
                    DateTime exportEndTime = DateTime.Parse(clsSearch.ClassExportEndTime);

                    TimeSpan processDuration = processEndTime - processStartTime;
                    TimeSpan exportDuration = exportEndTime - exportStartTime;

                    string message =
                        $"Data Process:\n" +
                        $"> Start Time: {dbFunction.AddBracketStartEnd(clsSearch.ClassProcessStartTime)}\n" +
                        $"> End Time:   {dbFunction.AddBracketStartEnd(clsSearch.ClassProcessEndTime)}\n" +
                        $"> Duration: {dbFunction.AddBracketStartEnd(dbFunction.formatDuration(processDuration))}\n\n" +
                        $"Export Process:\n" +
                        $"> Start Time: {dbFunction.AddBracketStartEnd(clsSearch.ClassExportStartTime)}\n" +
                        $"> End Time:   {dbFunction.AddBracketStartEnd(clsSearch.ClassExportEndTime)}\n" +
                        $"> Duration: {dbFunction.AddBracketStartEnd(dbFunction.formatDuration(exportDuration))}\n\n" +
                        $"Export complete.\nFile saved to:\n{finalPath}";

                    MessageBox.Show(message, "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred in Export to Excel: \nError: {ex.Message}");
                MessageBox.Show($"An error occurred in Export to Excel: \nError: {ex.Message}",
                    "Excel Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }


        private static Cell CreateTextCell(string text, uint styleIndex = 0)
        {
            // Always write as shared string alternative is possible, but writing inline string is simpler
            var cell = new Cell()
            {
                DataType = CellValues.String,
                CellValue = new CellValue(text ?? string.Empty)
            };
            if (styleIndex > 0)
                cell.StyleIndex = styleIndex;
            return cell;
        }

        private Stylesheet CreateStylesheet()
        {
            // General font settings
            string defaultFontName = "Courier New";
            double defaultFontSize = 10;

            // Fonts
            Fonts fonts = new Fonts(
                new Font( // 0 - Default
                    new FontName() { Val = defaultFontName },
                    new FontSize() { Val = defaultFontSize }
                ),
                new Font( // 1 - Header (white, bold)
                    new Bold(),
                    new Color() { Rgb = "FFFFFF" },
                    new FontName() { Val = defaultFontName },
                    new FontSize() { Val = defaultFontSize }
                ),
                new Font( // 2 - Dark Blue rows (white bold)
                    new Bold(),
                    new Color() { Rgb = "FFFFFF" },
                    new FontName() { Val = defaultFontName },
                    new FontSize() { Val = defaultFontSize }
                ),
                new Font( // 3 - Light Green rows (black)
                    new Color() { Rgb = "000000" },
                    new FontName() { Val = defaultFontName },
                    new FontSize() { Val = defaultFontSize }
                ),
                new Font( // 4 - White hidden rows
                    new Color() { Rgb = "FFFFFF" },
                    new FontName() { Val = defaultFontName },
                    new FontSize() { Val = defaultFontSize }
                ),
                new Font( // 5 - Green highlight (black, bold)
                    new Bold(),
                    new Color() { Rgb = "000000" },
                    new FontName() { Val = defaultFontName },
                    new FontSize() { Val = defaultFontSize }
                ),
                new Font( // 6 - Orange highlight (black, bold)
                    new Bold(),
                    new Color() { Rgb = "000000" },
                    new FontName() { Val = defaultFontName },
                    new FontSize() { Val = defaultFontSize }
                ),
                new Font( // 7 - Red highlight (white, bold)
                    new Bold(),
                    new Color() { Rgb = "FFFFFF" },
                    new FontName() { Val = defaultFontName },
                    new FontSize() { Val = defaultFontSize }
                ),
                new Font( // 8 - Bold only (for Column P)
                    new Bold(),
                    new Color() { Rgb = "000000" },
                    new FontName() { Val = defaultFontName },
                    new FontSize() { Val = defaultFontSize }
                )
            );

            // Fills
            Fills fills = new Fills(
                new Fill(new PatternFill() { PatternType = PatternValues.None }),   // 0
                new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }), // 1
                new Fill(new PatternFill( // 2 - Header background (Dark Blue)
                    new ForegroundColor { Rgb = "1F4E78" })
                { PatternType = PatternValues.Solid }),
                new Fill(new PatternFill( // 3 - Light Green row
                    new ForegroundColor { Rgb = "C6EFCE" })
                { PatternType = PatternValues.Solid }),
                new Fill(new PatternFill( // 4 - White row (hidden)
                    new ForegroundColor { Rgb = "FFFFFF" })
                { PatternType = PatternValues.Solid }),
                new Fill(new PatternFill( // 5 - Green highlight
                    new ForegroundColor { Rgb = "92D050" })
                { PatternType = PatternValues.Solid }),
                new Fill(new PatternFill( // 6 - Orange highlight
                    new ForegroundColor { Rgb = "F4B084" })
                { PatternType = PatternValues.Solid }),
                new Fill(new PatternFill( // 7 - Red highlight
                    new ForegroundColor { Rgb = "FF0000" })
                { PatternType = PatternValues.Solid })
            );

            // Borders
            Borders borders = new Borders(
                new Border(), // 0 - None
                new Border(   // 1 - Thin border all sides
                    new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new DiagonalBorder()
                )
            );

            // Cell formats (index = StyleIndex used in code)
            CellFormats cellFormats = new CellFormats(
                new CellFormat() { FontId = 0, FillId = 0, BorderId = 0 }, // 0 - default
                new CellFormat() { FontId = 1, FillId = 2, BorderId = 1, ApplyFill = true, ApplyFont = true }, // 1 - Header
                new CellFormat() { FontId = 2, FillId = 2, BorderId = 1, ApplyFill = true, ApplyFont = true }, // 2 - Dark Blue row
                new CellFormat() { FontId = 3, FillId = 3, BorderId = 1, ApplyFill = true, ApplyFont = true }, // 3 - Light Green row
                new CellFormat() { FontId = 4, FillId = 4, BorderId = 1, ApplyFill = true, ApplyFont = true }, // 4 - White row
                new CellFormat() { FontId = 5, FillId = 5, BorderId = 1, ApplyFill = true, ApplyFont = true }, // 5 - Green highlight
                new CellFormat() { FontId = 6, FillId = 6, BorderId = 1, ApplyFill = true, ApplyFont = true }, // 6 - Orange highlight
                new CellFormat() { FontId = 7, FillId = 7, BorderId = 1, ApplyFill = true, ApplyFont = true }, // 7 - Red highlight
                new CellFormat() { FontId = 8, FillId = 0, BorderId = 1, ApplyFont = true } // 8 - Bold only
            );

            return new Stylesheet(fonts, fills, borders, cellFormats);
        }


        private static void AutoSizeColumns(SheetData sheetData, Worksheet worksheet)
        {
            var columns = new Columns();

            int maxCol = sheetData.Elements<Row>().Max(r => r.Elements<Cell>().Count());

            for (int i = 0; i < maxCol; i++)
            {
                int columnIndex = i + 1;

                var columnCells = sheetData.Elements<Row>()
                    .Select(r => r.Elements<Cell>().ElementAtOrDefault(i));

                int maxLength = columnCells
                    .Where(c => c != null && c.CellValue != null)
                    .Select(c => c.CellValue.Text?.Length ?? 0)
                    .DefaultIfEmpty(10)
                    .Max();

                double width = Math.Min(maxLength + 2, 50);

                // ✅ Force minimum width (good for month names in Summary)
                if (width < 12)
                    width = 12;

                columns.Append(new Column()
                {
                    Min = (UInt32)columnIndex,
                    Max = (UInt32)columnIndex,
                    Width = width,
                    CustomWidth = true
                });
            }

            // Insert <Columns> before <SheetData>
            var sheetDataElement = worksheet.Elements<SheetData>().FirstOrDefault();
            if (sheetDataElement != null)
            {
                worksheet.InsertBefore(columns, sheetDataElement);
            }
            else
            {
                worksheet.Append(columns);
            }
        }


        private static void AutoSizeAllWorksheets(WorkbookPart workbookPart, bool isAutoSizeCol)
        {
            foreach (var worksheetPart in workbookPart.WorksheetParts)
            {
                var sheet = workbookPart.Workbook.Descendants<Sheet>()
                    .FirstOrDefault(s => s.Id == workbookPart.GetIdOfPart(worksheetPart));
                if (sheet == null) continue;

                var sheetData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();
                if (sheetData == null) continue;

                // Always remove old <Columns>
                worksheetPart.Worksheet.Elements<Columns>().ToList().ForEach(c => c.Remove());

                if (isAutoSizeCol)
                {
                    // ✅ Auto-size all sheets based on cell values
                    AutoSizeColumns(sheetData, worksheetPart.Worksheet);

                    // Special case: Summary sheet hides col A
                    if (sheet.Name == "Summary")
                    {
                        var cols = worksheetPart.Worksheet.Elements<Columns>().FirstOrDefault();
                        if (cols == null)
                        {
                            cols = new Columns();
                            worksheetPart.Worksheet.InsertAt(cols, 0);
                        }

                        var colA = cols.Elements<Column>().FirstOrDefault(c => c.Min == 1 && c.Max == 1);
                        if (colA == null)
                        {
                            colA = new Column()
                            {
                                Min = 1,
                                Max = 1,
                                Hidden = true,
                                CustomWidth = true
                            };
                            cols.Append(colA);
                        }
                        else
                        {
                            colA.Hidden = true;
                            colA.CustomWidth = true;
                        }
                    }
                }
                else
                {
                    // 🚀 Fixed-width columns (faster than auto-size)
                    var cols = new Columns();
                    for (uint i = 1; i <= 50; i++)
                    {
                        cols.Append(new Column()
                        {
                            Min = i,
                            Max = i,
                            Width = 20,
                            CustomWidth = true
                        });
                    }
                    worksheetPart.Worksheet.InsertAt(cols, 0);
                }

                // ✅ Save worksheet changes
                worksheetPart.Worksheet.Save();
            }
        }

        private static string CleanXmlString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            return new string(text.Where(ch => System.Xml.XmlConvert.IsXmlChar(ch)).ToArray());
        }
    }
}
