
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MIS.Function
{
    class AppUtilities
    {
        public static class Prompt
        {
            public static void Info(string Caption, string Message)
            {
                ShowMessage(Message, Caption, MessageBoxIcon.Information, MessageBoxButtons.OK);
            }

            public static void Error(string Caption, string Message)
            {
                ShowMessage(Message, Caption, MessageBoxIcon.Error, MessageBoxButtons.OK);
            }

            public static void Warning(string Caption, string Message)
            {
                ShowMessage(Message, Caption, MessageBoxIcon.Warning, MessageBoxButtons.OK);
            }

            public static DialogResult YesNo(string Caption, string Message)
            {
                return ShowMessage(Message, Caption, MessageBoxIcon.Question, MessageBoxButtons.YesNo);
            }

            // Debug WriteLine Ctrl
            public static void Debug(string Method, string Message)
            {
                // Calculate the methodWidth to ensure consistent spacing
                //int methodWidth = Math.Max(19, Method.Length + 2);

                // Pad Method with single quotes and align it to methodWidth
                //Method = $"'{Method}'".PadRight(methodWidth);

                // Output the debug message with the formatted date, Method, and Msg
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:hh:mm:ss tt}] [Method] {Method} : {Message}");
            }
        }

        public static class JFunc
        {
            // Get Specific Value from Jason using Key
            public static string GetJsonValue(string jsonString, string key, string OutParameter)
            {
                try
                {
                    JToken token = JToken.Parse(jsonString);

                    if (token is JArray jsonArray && jsonArray.Count > 0)
                    {
                        token = jsonArray[0];
                    }

                    if (token is JObject jsonObject)
                    {
                        if (jsonObject.TryGetValue(key, out JToken value))
                        {
                            return value.ToString();
                        }

                        // Get value of out Parameters
                        if (jsonObject.TryGetValue(OutParameter, out JToken outMsg) && outMsg is JObject innerObject)
                        {
                            if (innerObject.TryGetValue(key, out JToken innerValue))
                            {
                                return innerValue.ToString();
                            }
                        }
                    }
                }
                catch (JsonReaderException)
                {
                    // JSON parsing failed, return null or handle the error
                    return null;
                }

                return null;
            }

            // Using Objects
            public static T Deserialized<T>(string jsonString)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(jsonString))
                    {
                        return default(T);
                    }

                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
                catch (JsonException ex)
                {
                    // Get Specific Error in Json Response
                    string errorMessage = $"Deserialization Failed in {typeof(T).Name}: \n{ex.Message}";

                    if (ex is JsonSerializationException JSException)
                    {

                        JToken jToken = JToken.Parse(jsonString);
                        JToken property = jToken.SelectToken(JSException.Path);

                        if (property != null)
                        {
                            errorMessage += $"\n\nIncompatible Value. \n-------------------------\nKey    : {property.Path} \nValue : {property}";
                        }
                    }

                    Prompt.Error("Deserialized", errorMessage);
                    return default(T);
                }
            }

            // Using Objects
            public static string Serialized<T>(T obj)
            {
                try
                {
                    if (obj == null)
                    {
                        Prompt.Debug("Serialized", "Object is null. Cannot serialize.");
                        return null;
                    }

                    Prompt.Debug("Serialized", "Serialized Success");
                    return JsonConvert.SerializeObject(obj);
                }
                catch (JsonException ex)
                {
                    Prompt.Debug("Serialized", $"Serialization Error: {ex.Message}");
                    return null;
                }
            }

            public static string getValue(string jsonString, string key)
            {
                try
                {
                    JObject jsonObj = JObject.Parse(jsonString);

                    // Check if the key exists in the JSON
                    if (jsonObj.ContainsKey(key))
                    {
                        return jsonObj[key]?.ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    return $"Error: {ex.Message}";
                }
            }

        }

        public static class Window
        {
            // Sample Usage
            //Window.Hide(this); // Hide the current form
            //Window.Show<frm_Dashboard>(); // Show the new form
            //Window.Show<frm_Dashboard>(isModal: true); // For Modal Forms
            //Window.Show(() => new frm_Dashboard(Role, Name, UserID)); with paramters

            // Show Form
            public static void Show<T>(Func<T> formFactory = null, bool isModal = false) where T : Form, new()
            {
                // Use the provided factory or create a default instance if none
                T form = formFactory != null ? formFactory.Invoke() : new T();

                // Show the form
                if (isModal)
                {
                    form.ShowDialog();
                }
                else
                {
                    form.Show();
                }
            }

            // Hide Form
            public static void Hide(Form currentForm)
            {
                if (currentForm != null)
                {
                    currentForm.Hide();
                }
            }
        }


        public static class sDisplay
        {
            public static string Separator = LineSeparator(70);
        }

        public static class IFormat
        {
            // Format the Update Data Operation using Model or Object
            public static string Update<T>(T model)
            {
                // Use reflection to get the property values.
                var propertyValues = GetModelProperties(model);

                // Apply string cleanup and delimiter.
                var cleanedValues = propertyValues.Select(value => $"{StrClean(value)}");

                // Pipe Delimited
                return string.Join("|", cleanedValues);
            }

            // Format the Insert Data Operation using Model or Object
            public static string Insert<T>(T model)
            {
                // Use reflection to get the property values.
                var propertyValues = GetModelProperties(model);

                // Apply string cleanup and delimiter here.
                var cleanedValues = propertyValues.Select(value => $"'{StrClean(value)}'");

                // Comma Delimited
                return $"({string.Join(",", cleanedValues)})";
            }
        }

        public static void DebugTable(DataTable table)
        {
            Console.WriteLine("Columns: \n" + string.Join(", ", table.Columns.Cast<DataColumn>().Select(col => col.ColumnName)));

            foreach (DataRow row in table.Rows)
            {
                Console.WriteLine(string.Join(" | ", row.ItemArray));
            }
        }

        public static void DebugComboBox(ComboBox cbx)
        {
            // Use the selected ID for further processing (e.g., inserting data)
            Debug.WriteLine($"Selected ID: {cbx.SelectedValue}, Description: {cbx.Text}");
        }

        public static bool isValid(object input)
        {
            if (input is int number)
            {
                return number != 0;
            }

            else if (input is string text)
            {
                return !string.IsNullOrEmpty(text);
            }

            return false;
        }

        public static DataTable ParseResponseData(string[] ID, string[] Details)
        {
            DataTable dt = new DataTable();
            int i = 0;

            while (ID.Length > i)
            {
                string detail_info = Details[i];

                var dataDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(detail_info);

                if (dataDict != null)
                {
                    foreach (var key in dataDict.Keys)
                    {
                        if (!dt.Columns.Contains(key))
                        {
                            dt.Columns.Add(key, typeof(string));
                        }
                    }

                    DataRow row = dt.NewRow();
                    foreach (var key in dataDict.Keys)
                    {
                        row[key] = dataDict[key];
                    }

                    dt.Rows.Add(row);
                }

                i++;
            }

            return dt;
        }

        public static void SetListViewJson(ListView listView, string json, List<string> columnOrder)
        {
            try
            {
                JArray records = JArray.Parse(json);

                if (records.Count == 0)
                {
                    Prompt.Info("Populating List",$"No Record found");
                    return;
                }

                listView.Items.Clear();
                listView.Columns.Clear();

                foreach (string column in columnOrder)
                {
                    listView.Columns.Add(column, 120);
                }

                // Add rows to the ListView
                foreach (JObject record in records)
                {
                    List<string> rowValues = new List<string>();

                    foreach (string column in columnOrder)
                    {
                        JToken value;
                        if (record.TryGetValue(column, out value))
                        {
                            rowValues.Add(value.ToString());
                        }
                        else
                        {
                            rowValues.Add("");
                        }
                    }

                    // Add row to ListView
                    listView.Items.Add(new ListViewItem(rowValues.ToArray()));
                }

                // Auto-resize columns
                listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SetListViewData(ListView listView, DataTable dataTable, string[] columnSequence, string[] hiddenColumns)
        {
            listView.Clear();
            listView.View = View.Details;
            listView.Columns.Add("LINE#");

            if (columnSequence == null)
            {
                columnSequence = dataTable.Columns.Cast<DataColumn>()
                                    .Select(col => col.ColumnName)
                                    .ToArray();
            }

            foreach (string columnName in columnSequence)
            {
                listView.Columns.Add(columnName);
            }

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                ListViewItem item = new ListViewItem((i + 1).ToString());

                foreach (string columnName in columnSequence)
                {
                    item.SubItems.Add(dataTable.Rows[i][columnName]?.ToString());
                }

                listView.Items.Add(item);
            }

            foreach (ColumnHeader column in listView.Columns)
            {
                if (hiddenColumns != null && hiddenColumns.Contains(column.Text))
                {
                    column.Width = 1;
                }
                else
                {
                    column.Width = -2;
                }
            }
        }

        public static List<T> ConvertToList<T>(DataTable dt) where T : new()
        {
            List<T> list = new List<T>();

            // Get all properties of the model class
            var properties = typeof(T).GetProperties();

            foreach (DataRow row in dt.Rows)
            {
                T obj = new T();

                foreach (var prop in properties)
                {
                    if (dt.Columns.Contains(prop.Name) && row[prop.Name] != DBNull.Value)
                    {
                        // Convert and assign the value to the property
                        prop.SetValue(obj, Convert.ChangeType(row[prop.Name], prop.PropertyType));
                    }
                }

                list.Add(obj);
            }

            return list;
        }

        public static void MapDataToTxt(DataTable dt, Dictionary<string, TextBox> mappings, int rowIndex = 0)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("No data available.");
                return;
            }

            if (rowIndex < 0 || rowIndex >= dt.Rows.Count)
            {
                MessageBox.Show("Invalid row index.");
                return;
            }

            DataRow row = dt.Rows[rowIndex];

            foreach (var mapping in mappings)
            {
                string columnName = mapping.Key;
                TextBox textBox = mapping.Value;

                if (dt.Columns.Contains(columnName) && textBox != null)
                {
                    textBox.Text = row[columnName]?.ToString();
                }
            }
        }

        private static DialogResult ShowMessage(string message, string caption, MessageBoxIcon icon, MessageBoxButtons buttons)
        {
            return MessageBox.Show(message, caption, buttons, icon);
        }

        private static string LineSeparator(int size)
        {
            return new string('-', size);
        }

        // Get Model/Object Properties
        private static string[] GetModelProperties<T>(T model)
        {
            // Use reflection to get the property values.
            var propertyValues = typeof(T).GetProperties().Select(prop => prop.GetValue(model)?.ToString());

            return propertyValues.ToArray();
        }

        // Select Characters to Remove
        public static string StrClean(string str)
        {
            /*
            Dictionary<string, string> replacements = new Dictionary<string, string>
                {
                    { "ñ", "n" },
                    { "Ñ", "N" },
                    { "\t", "" },
                    { "\n", "" },
                    { "\r", "" },
                    { " ", "" },
                    { ",", "" },
                    { "'", "" },
                    { ";", "" },
                    { "  ", " " },
                    { "\"", "" },
                    {"Â€“", "" },
                    {"^", "" },
                    {"|", "" },
                    {"~", "" },
                    {"\\", "/" },
                    {"&", "AND" }
                };
            */

            // Normalize Unicode (helps with encoding issues)
            str = str.Normalize(NormalizationForm.FormKC);

            Dictionary<string, string> replacements = new Dictionary<string, string>
            {
                { "ñ", "n" },
                { "Ñ", "N" },

                // whitespace / control
                { "\t", "" },
                { "\n", "" },
                { "\r", "" },

                // fractions
                { "½", "1/2" },
                { "¼", "1/4" },
                { "¾", "3/4" },

                // special symbols
                { "|", "" },
                { "~", "" },
                { "^", "" },
                //{ "%", "" },
                { "'", "" },
                { "#", "" },
                //{ "&", "" },
                { "&", "AND" },
                { "+", "" },
                { "=", "" },
                { "?", "" },
                //{ "/", "" },
                { "\\", "/" },
                //{ ":", "" },
                { ";", "" },
                { "\"", "" },
                //{ "<", "" },
                //{ ">", "" },
                { ",", "" },
                //{ ".", "" },
                //{ "(", "" },
                //{ ")", "" },
                //{ "[", "" },
                //{ "]", "" },
                //{ "{", "" },
                //{ "}", "" },

                // dash normalization
                { "–", "-" },   // en dash
                { "—", "-" },   // em dash
                { "―", "-" },   // horizontal bar

                // encoding issues
                { "Â€“", "-" },
                { "â€“", "-" },
                { "â€”", "-" },

                // quotes normalization
                { "“", "" },
                { "”", "" },
                { "‘", "" },
                { "’", "" },

                // spacing
                { "  ", " " }  // double space → single
            };

            string Result = RemoveChars(str, replacements).ToUpper();

            if (!string.IsNullOrWhiteSpace(Result))
            {
                return Result;
            }
            else
            {
                return "-";
            }
        }

        // Remove Characters
        private static string RemoveChars(string input, Dictionary<string, string> replacements)
        {

            StringBuilder sb = new StringBuilder(input);

            foreach (var replacement in replacements)
            {
                sb.Replace(replacement.Key, replacement.Value);
            }

            return sb.ToString();
        }

        // Export DataTable to Excel
        public static void ExportDtToExcel(DataTable Dt)
        {
            try
            {

                if (Dt.Rows.Count != 0)
                {
                    // Set the LicenseContext property to NonCommercial
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    // Create a SaveFileDialog instance
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                    sfd.FilterIndex = 1;
                    sfd.RestoreDirectory = true;

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = sfd.FileName;

                        // Create a copy of the DataTable
                        DataTable copyDt = Dt.Copy();

                        // Remove first column (ServiceNo) from the copy
                        copyDt.Columns.RemoveAt(0);


                        using (ExcelPackage package = new ExcelPackage())
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                            // Load data into worksheet
                            worksheet.Cells["A1"].LoadFromDataTable(copyDt, true);

                            // Save the Excel package
                            FileInfo excelFile = new FileInfo(filePath);
                            package.SaveAs(excelFile);

                            MessageBox.Show($"Export complete. File saved to: {filePath}", "Exporting Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Prompt.Error("Data Export",$"An error occured: in ExportDtToExcel \nError:{ex.Message}");
            }
        }

        public static void ExportCustomDataToExcel(
            string defaultFilename = "ExportFile.xlsx",
            DataTable[] tables = null,
            string[] sheetNames = null,
            Color[] headerColors = null)
        {
            try
            {
                if (tables == null || tables.Length == 0 || tables.All(t => t == null || t.Rows.Count == 0))
                {
                    MessageBox.Show("No data available to export.", "Exporting Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                string filePath;

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                    sfd.FilterIndex = 1;
                    sfd.RestoreDirectory = true;
                    sfd.FileName = defaultFilename;

                    if (sfd.ShowDialog() != DialogResult.OK)
                        return;

                    filePath = sfd.FileName;
                }

                using (ExcelPackage package = new ExcelPackage())
                {
                    for (int i = 0; i < tables.Length; i++)
                    {
                        DataTable dt = tables[i];
                        if (dt == null || dt.Rows.Count == 0) continue;

                        string sheetName = (sheetNames != null && i < sheetNames.Length)
                            ? sheetNames[i]
                            : (tables.Length == 1 ? "Sheet1" : $"Sheet{i + 1}");

                        Color headerColor = (headerColors != null && i < headerColors.Length)
                            ? headerColors[i]
                            : Color.ForestGreen;

                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);
                        worksheet.Cells["A1"].LoadFromDataTable(dt.Copy(), true);

                        // Format date/time columns
                        for (int col = 0; col < dt.Columns.Count; col++)
                        {
                            if (dt.Columns[col].DataType == typeof(DateTime))
                            {
                                int excelCol = col + 1;
                                string colName = dt.Columns[col].ColumnName.ToLower();

                                worksheet.Column(excelCol).Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";
                                
                            }
                        }

                        // Header style
                        using (var range = worksheet.Cells[1, 1, 1, dt.Columns.Count])
                        {
                            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(headerColor);
                            range.Style.Font.Bold = true;
                            range.Style.Font.Color.SetColor(Color.White);
                        }

                        // Borders for all data
                        int totalRows = dt.Rows.Count + 1;
                        int totalCols = dt.Columns.Count;

                        var dataRange = worksheet.Cells[1, 1, totalRows, totalCols];
                        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    }

                    package.SaveAs(new FileInfo(filePath));

                    MessageBox.Show($"Export complete. File saved to: {filePath}", "Exporting Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Prompt.Error("Data Export", $"An error occurred in ExportMultiDtToExcel:\n{ex.Message}");
            }
        }

        public static DataTable ReplaceDuplicateData(DataTable dt, string columnName)
        {
            HashSet<string> seen = new HashSet<string>();

            foreach (DataRow row in dt.Rows)
            {
                string value = row[columnName]?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    if (seen.Contains(value))
                    {
                        row[columnName] = "";
                    }
                    else
                    {
                        seen.Add(value);
                    }
                }
            }

            return dt;
        }

        public static void Export_KPI_Report(DataTable dt1, DataTable dt2, DataTable dt3, DataTable dt4, DataTable dt5)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel Files|*.xlsx";
                    sfd.FileName = "HD_KPI_Report.xlsx";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (var package = new ExcelPackage())
                        {
                            var ws1 = package.Workbook.Worksheets.Add("Summary");

                            ws1.Cells[1, 1].LoadFromDataTable(dt1, true);
                            int dt1Rows = dt1.Rows.Count + 1;
                            var dt1Range = ws1.Cells[1, 1, dt1Rows, dt1.Columns.Count];
                            ApplyTableStyle(dt1Range, dt1.Columns.Count);

                            int currentRow = dt1Rows + 2;

                            ws1.Cells[currentRow, 1].LoadFromDataTable(dt2, true);
                            int dt2StartRow = currentRow;
                            int dt2EndRow = dt2StartRow + dt2.Rows.Count;
                            var dt2Range = ws1.Cells[dt2StartRow, 1, dt2EndRow, dt2.Columns.Count];
                            ApplyTableStyle(dt2Range, dt2.Columns.Count);
                            currentRow = dt2EndRow + 2;

                            int dt3StartRow = dt2StartRow;
                            int dt2ColCount = dt2.Columns.Count;
                            int dt3StartCol = dt2ColCount + 2;
                            ws1.Cells[dt3StartRow, dt3StartCol].LoadFromDataTable(dt3, true);
                            int dt3EndRow = dt3StartRow + dt3.Rows.Count;
                            var dt3Range = ws1.Cells[dt3StartRow, dt3StartCol, dt3EndRow, dt3StartCol + dt3.Columns.Count - 1];
                            ApplyTableStyle(dt3Range, dt3.Columns.Count);

                            int dt4StartCol = dt3StartCol + dt3.Columns.Count + 1;
                            ws1.Cells[dt3StartRow, dt4StartCol].LoadFromDataTable(dt4, true);
                            int dt4EndRow = dt3StartRow + dt4.Rows.Count;
                            var dt4Range = ws1.Cells[dt3StartRow, dt4StartCol, dt4EndRow, dt4StartCol + dt4.Columns.Count - 1];
                            ApplyTableStyle(dt4Range, dt4.Columns.Count);

                            int pieChartRow = dt2EndRow + 6;
                            var pieChart = ws1.Drawings.AddChart("PieChart", eChartType.Pie) as ExcelPieChart;
                            pieChart.Title.Text = "MONTHLY OVERALL RESOLUTION";
                            pieChart.SetPosition(pieChartRow - 1, 0, 0, 0);
                            pieChart.SetSize(600, 400);
                            pieChart.DataLabel.ShowPercent = true;
                            pieChart.DataLabel.ShowCategory = true;
                            pieChart.Legend.Position = eLegendPosition.Right;
                            pieChart.Series.Add(
                                ws1.Cells[dt2StartRow + 1, 2, dt2EndRow, 2],
                                ws1.Cells[dt2StartRow + 1, 1, dt2EndRow, 1]
                            );

                            var barChart = ws1.Drawings.AddChart("BarChart", eChartType.ColumnClustered) as ExcelBarChart;
                            barChart.Title.Text = "MONTHLY MONITORING GRAPH PERFORMANCE";
                            barChart.SetPosition(pieChartRow - 1, 0, 10, 0);
                            barChart.SetSize(1000, 400);

                            var barSeries1 = barChart.Series.Add(
                                ws1.Cells[dt3StartRow + 1, dt3StartCol + 1, dt3EndRow, dt3StartCol + 1],
                                ws1.Cells[dt3StartRow + 1, dt3StartCol, dt3EndRow, dt3StartCol]
                            );
                            barSeries1.Header = "PENDING";

                            var barSeries2 = barChart.Series.Add(
                                ws1.Cells[dt3StartRow + 1, dt3StartCol + 2, dt3EndRow, dt3StartCol + 2],
                                ws1.Cells[dt3StartRow + 1, dt3StartCol, dt3EndRow, dt3StartCol]
                            );
                            barSeries2.Header = "RESOLVED";

                            var barSeries3 = barChart.Series.Add(
                                ws1.Cells[dt3StartRow + 1, dt3StartCol + 3, dt3EndRow, dt3StartCol + 3],
                                ws1.Cells[dt3StartRow + 1, dt3StartCol, dt3EndRow, dt3StartCol]
                            );
                            barSeries3.Header = "NEGATIVE";

                            barChart.Legend.Position = eLegendPosition.Right;

                            var ws2 = package.Workbook.Worksheets.Add("Details");
                            ws2.Cells[1, 1].LoadFromDataTable(dt5, true);
                            var dt5Range = ws2.Cells[1, 1, dt5.Rows.Count + 1, dt5.Columns.Count];
                            ApplyTableStyle(dt5Range, dt5.Columns.Count);

                            File.WriteAllBytes(sfd.FileName, package.GetAsByteArray());
                            MessageBox.Show("Excel file exported successfully!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chart error: " + ex.Message);
            }
        }

        private static void ApplyTableStyle(ExcelRange range, int colCount)
        {
            // Apply thin borders
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            // Header fill color (light blue)
            var header = range.Offset(0, 0, 1, colCount);
            header.Style.Fill.PatternType = ExcelFillStyle.Solid;
            header.Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);
            header.Style.Font.Bold = true;
        }

        public static string FormatGridData(DataGridView dgv)
        {
            // build the parsed string
            StringBuilder parsedString = new StringBuilder();

            // Check if the DataGridView has any rows
            if (dgv.Rows.Count != 0)
            {
                // append the value surrounded by single quotes
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (!row.IsNewRow && row.Cells[0].Value != null)
                    {
                        string value = row.Cells[0].Value.ToString().Trim();
                        parsedString.Append($"'{value}', ");
                    }
                }

                // Remove the comma and space
                if (parsedString.Length > 0)
                {
                    parsedString.Length -= 2;
                }

                return parsedString.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public static string getParseValue(string Value)
        {
            return Value ?? string.Empty;
        }

        public static void PanelRadius(Control control, int Radius, Color BorderColor, int BorderWidth)
        {
            control.Paint += (sender, e) =>
            {
                Graphics graphics = e.Graphics;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                GraphicsPath path = new GraphicsPath();
                int diameter = Radius * 2;

                // Top left corner
                path.AddArc(0, 0, diameter, diameter, 180, 90);

                // Top right corner
                path.AddArc(control.Width - diameter, 0, diameter, diameter, 270, 90);

                // Bottom right corner
                path.AddArc(control.Width - diameter, control.Height - diameter, diameter, diameter, 0, 90);

                // Bottom left corner
                path.AddArc(0, control.Height - diameter, diameter, diameter, 90, 90);

                path.CloseFigure();

                // Set the region for the rounded corners
                control.Region = new Region(path);

                // Draw the border
                using (Pen borderPen = new Pen(BorderColor, BorderWidth))
                {
                    graphics.DrawPath(borderPen, path);
                }
            };

            // Invalidate the control on resize to force it to repaint
            control.Resize += (sender, e) =>
            {
                control.Invalidate(); // This will trigger the Paint event
            };
        }

        public static void SetListViewTextColor(ListView listView, string columnName, Dictionary<string, Color> valueColorMap)
        {
            // Get the index of the specified column
            int columnIndex = -1;
            for (int i = 0; i < listView.Columns.Count; i++)
            {
                if (listView.Columns[i].Text.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                {
                    columnIndex = i;
                    break;
                }
            }

            // Exit early if column not found
            if (columnIndex == -1)
                return;

            // Loop through all items
            foreach (ListViewItem item in listView.Items)
            {
                // Safety check for subitems count
                if (item.SubItems.Count <= columnIndex) continue;

                string cellValue = item.SubItems[columnIndex].Text.Trim();

                // Apply color based on value
                if (valueColorMap.TryGetValue(cellValue, out Color color))
                {
                    item.UseItemStyleForSubItems = false;
                    item.SubItems[columnIndex].ForeColor = color;
                }
            }
        }

        public static void SetListViewColumnNames(ListView listView, Dictionary<string, string> headerRenameMap)
        {
            foreach (ColumnHeader column in listView.Columns)
            {
                if (headerRenameMap.TryGetValue(column.Text, out string newHeader))
                {
                    column.Text = newHeader;
                }
            }
        }

        public static void SetListViewAutoWidth(ListView listView, string[] hiddenColumns = null)
        {
            if (listView == null || listView.Columns.Count == 0)
                return;

            for (int i = 0; i < listView.Columns.Count; i++)
            {
                ColumnHeader column = listView.Columns[i];

                if (hiddenColumns != null && hiddenColumns.Contains(column.Text))
                {
                    column.Width = 0; // hide completely
                }
                else
                {
                    // Auto-size to content
                    column.Width = -2;
                }
            }
        }

    }
}
