using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using OfficeOpenXml;
using System.Data;
using OfficeOpenXml.Style;
using MIS.Function;
using System.Drawing.Drawing2D;
using System.Globalization;
using MIS.Model;
using static MIS.Function.AppUtilities;

namespace MIS
{
    public class clsFile
    {
        public string sApplicationMainPath = "C:\\CASTLESTECH_MIS";
        public string sLogFullPath  = "C:\\CASTLESTECH_MIS\\LOGS\\";
        public string sCSVPath = "C:\\CASTLESTECH_MIS\\CSV\\";
        public string sExportPath = "C:\\CASTLESTECH_MIS\\EXPORT\\";
        public string sRespFullPath = "C:\\CASTLESTECH_MIS\\RESPONSE\\";
        public string sImagePath = "C:\\CASTLESTECH_MIS\\IMAGE\\";
        public string sReportPath = "C:\\CASTLESTECH_MIS\\REPORTS\\";
        public string sJSONFullPath = "C:\\CASTLESTECH_MIS\\JSON\\";
        public string sDataFullPath = "C:\\CASTLESTECH_MIS\\DATA\\";
        public string sSupportFullPath = "C:\\CASTLESTECH_MIS\\SUPPORT\\";
        public string sScriptFullPath = "C:\\CASTLESTECH_MIS\\SCRIPTING\\";
        public string sDumpFullPath = "C:\\CASTLESTECH_MIS\\DUMP\\";
        public string sImportPath = "C:\\CASTLESTECH_MIS\\IMPORT\\";
        public string sDowloadPath = "C:\\CASTLESTECH_MIS\\DOWNLOAD\\";
        public string sDashboardPath = "C:\\CASTLESTECH_MIS\\DASHBOARD\\";
        public string sSettingPath = "C:\\CASTLESTECH_MIS\\SETTING\\";
        public string sTemplatePath = "C:\\CASTLESTECH_MIS\\TEMPLATE\\";
        public string sVendorSignaturePath = "C:\\CASTLESTECH_MIS\\IMAGE\\SIGNATURES\\";
        public string sSignatuPath = "C:\\CASTLESTECH_MIS\\DOWNLOAD\\IMAGE\\";

        private clsFunction dbFunction;
        private clsAPI dbAPI;

        public void WriteAPILog(int iLogType, string sLog)
        {
            DateTime LogtDateTime = DateTime.Now;
            string sLogDateTime = "";

            string FileName = "";
            string FullPath = "";
            string Log = "";
            string separator1 = "+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++";
            string separator2 = "#############################################################################################################################";

            sLogDateTime = LogtDateTime.ToString("yyyy-MM-dd H:mm:ss");

            string timePrefix = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ";
            string logEntry = $"{timePrefix} {clsSearch.ClassBankCode} : {sLog}";

            //Log = separator1 + "\n" + "[" + sLogDateTime + "]" +  "[" + sLog + "]" + "\n" + separator2 + "\n";
            switch (iLogType)
            {
                case 0:  // Request
                    FileName = "APIREQUEST_MIS.log";
                    break;
                case 1: // Response
                    FileName = "APIRESPONSE_MIS.log";
                    break;
                case 2: // Error
                    FileName = "APIERROR_MIS.log";
                    break;
                case 3: // 
                    FileName = "FTP_MIS.log";
                    break;
                case 4:
                    FileName = "Apps_MIS.log";
                    break;
            }
            
            FullPath = sLogFullPath + FileName;
           
            // Delete log file if size limit reached
            deleteLogFile(FullPath);

            // Ensure directory exists
            Directory.CreateDirectory(sLogFullPath);

            // Append line to the file.
            using (StreamWriter writer = new StreamWriter(FullPath, true))
            {
                writer.WriteLine(logEntry);
            }
        }

        public void WriteCSV(string sFileName, string sData)
        {
            string FullPath = "";            
            FullPath = sCSVPath + sFileName;

            // Append line to the file.
            using (StreamWriter writer = new StreamWriter(FullPath, true))
            {
                writer.WriteLine(sData);
            }
        }

        public void DeleteCSV(string sFileName)
        {            
            string FullPath = "";
            DateTime WriteDateTime = DateTime.Now;            

            //sWriteDateTime = WriteDateTime.ToString("yyyyMMdd");

            //FileName = sFileName + "_" + sWriteDateTime + ".csv";
            FullPath = sCSVPath + sFileName;

            try
            {
                File.Delete(FullPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void CopyFile(string sFullPathSource, string sFullPathDestination)
        {
            // Delete Source
            DeleteFile(sFullPathDestination);

            try
            {
                File.Copy(sFullPathSource, sFullPathDestination);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void DeleteFile(string sFullPathSource)
        {
            try
            {
                File.Delete(sFullPathSource);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void WriteResponse(string pMethod, string pStatement, string pSearchBy, string pAction, JObject jObject)
        {
            Debug.WriteLine("--WriteResponse--");
            Debug.WriteLine("pMethod="+ pMethod);
            Debug.WriteLine("pStatement=" + pStatement);
            Debug.WriteLine("pSearchBy=" + pSearchBy);
            Debug.WriteLine("pAction=" + pAction);

            string FileName = clsSearch.ClassResponseFileName;
            string FullPath = sRespFullPath + FileName;

            Debug.WriteLine("FullPath="+ FullPath);

            string json = JsonConvert.SerializeObject(jObject, Formatting.Indented);
            Debug.WriteLine("-- Formatting Indented--");
            Debug.WriteLine(json);

            try
            {
                // Delete file when exist
                if (File.Exists(FullPath))
                {
                    File.Delete(FullPath);
                }

                using (StreamWriter writer = new StreamWriter(FullPath, true))
                {
                    writer.WriteLine(json);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error on writing response " + ex.Message + "\n\n" +
                               ">Method:" + pMethod +
                               "\n" +
                               ">Action:" + pAction +
                               "\n" +
                               "Statement Type:" + pStatement +
                               "\n" +
                               "Search By:" + pSearchBy + 
                               "\n" +
                               clsFunction.sLineSeparator + 
                               "\n" +
                               ">Response" +
                               clsFunction.sLineSeparator +
                               json, "File Write Response", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

           
        }

        public void CheckFolder(string sFolderPath)
        {
            try
            {
                if (!Directory.Exists(@sFolderPath))
                    Directory.CreateDirectory(@sFolderPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message +
                                "\n\n" +
                                "Folder: " + sFolderPath, "CheckFolder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        public void DeleteFolder(string sFolderPath)
        {
            try
            {
                // Delete Folder
                if (Directory.Exists(sFolderPath))
                {
                    Directory.Delete(sFolderPath, true);
                    Directory.CreateDirectory(sFolderPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message +
                                "\n\n" +
                                "Folder: " + sFolderPath, "DeleteFolder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
        }

        public void CreateAppRequiredFolder()
        {
            Debug.WriteLine("--CreateAppRequiredFolder--");

            CheckFolder(sLogFullPath);
            CheckFolder(sCSVPath);
            CheckFolder(sExportPath);
            CheckFolder(sRespFullPath);
            CheckFolder(sImagePath);
            CheckFolder(sReportPath);
            CheckFolder(sJSONFullPath);
            CheckFolder(sDataFullPath);
            CheckFolder(sSupportFullPath);
            CheckFolder(sScriptFullPath);
            CheckFolder(sDumpFullPath);
            CheckFolder(sDowloadPath);
            CheckFolder(sImportPath);
            CheckFolder(sDashboardPath);
        }

        public void attachImage(ref string outFileName, ref string outFullPathFileName, PictureBox obj, string pInitialDirectory)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Image Files|*.bmp;*.png;*.jpg";
            openFile.InitialDirectory = pInitialDirectory;
            openFile.Title = "Select image file";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                // selected image
                outFullPathFileName = openFile.FileName;
                outFileName = Path.GetFileName(outFullPathFileName);
                
                obj.Image = Bitmap.FromFile(@outFullPathFileName);

            }

            openFile.RestoreDirectory = true;
            openFile.Dispose();
            openFile = null;
        }

        public void saveImage(PictureBox obj, string pFileName, string pExtension)
        {
            clsFunction dbFunction = new clsFunction();
            string sFileName = pFileName;
            string sFullPath = sImportPath + sFileName;

            int targetWidth = 800;
            int targetHeight = 800;

            try
            {
                // delete file if exists
                DeleteFile(sFullPath);

                if (obj.Image == null)
                {
                    dbFunction.SetMessageBox("No image found in PictureBox.", "Save Error", clsFunction.IconType.iError);
                    return;
                }

                Image originalImage = obj.Image;

                // Create a new bitmap with target size
                using (Bitmap resizedImage = new Bitmap(targetWidth, targetHeight))
                {
                    using (Graphics g = Graphics.FromImage(resizedImage))
                    {
                        // Set high-quality settings
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        // Draw original image into resized target
                        g.DrawImage(originalImage, 0, 0, targetWidth, targetHeight);
                    }

                    // Determine format
                    ImageFormat format = ImageFormat.Bmp;
                    if (pExtension.Equals(clsDefines.FILE_EXT_JPG, StringComparison.OrdinalIgnoreCase))
                        format = ImageFormat.Jpeg;
                    else if (pExtension.Equals(clsDefines.FILE_EXT_PNG, StringComparison.OrdinalIgnoreCase))
                        format = ImageFormat.Png;

                    // Save image
                    resizedImage.Save(sFullPath, format);
                }
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox("Unable to create image " + pFileName + "\n\n" + "Error: " + ex.Message, "Failed to create", clsFunction.IconType.iError);
            }
        }
        
        public bool FileExist(string sFullPathSource)
        {
            // Check if the file exists
            if (File.Exists(sFullPathSource))
            {
                return true;

            }
            else
            {
                return false;

            }
        }

        public string openFolderDialog()
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderDialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    return folderDialog.SelectedPath;
                }
            }

            return null;
        }

        public bool isValidPath(string path)
        {
            bool isValid = false;

            isValid = Directory.Exists(path);

            if (!isValid)
            {
                MessageBox.Show(path + " is not valid path.", "Invalid path", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return isValid;
        }

        public bool RenameFile(string sourceFilePath, string destinationFilePath)
        {
           try
            {
                // Ensure the source file exists
                if (!File.Exists(sourceFilePath))
                {
                    Console.WriteLine("The source file does not exist.");
                    return false;
                }

                // Check if the destination file already exists
                if (File.Exists(destinationFilePath))
                {
                    // Delete the existing destination file
                    File.Delete(destinationFilePath);
                    Console.WriteLine("Existing destination file was deleted.");
                }

                // Rename (or move) the file
                File.Move(sourceFilePath, destinationFilePath);

                Console.WriteLine("File renamed/moved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public bool CopyAndRenameFile(string sourceFilePath, string destinationFilePath)
        {
            try
            {
                // Ensure the source file exists
                if (!File.Exists(sourceFilePath))
                {
                    Console.WriteLine("The source file does not exist.");
                    return false;
                }

                // Check if the destination file already exists
                if (File.Exists(destinationFilePath))
                {
                    // Delete the existing destination file
                    File.Delete(destinationFilePath);
                    Console.WriteLine("Existing destination file was deleted.");
                }

                // Copy the file to the destination path with the new name
                File.Copy(sourceFilePath, destinationFilePath);

                Console.WriteLine("File copied and renamed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public void openFile(string filePath)
        {
            try
            {
                // Ensure the file exists before trying to open it
                if (File.Exists(filePath))
                {
                    Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                    Console.WriteLine("File opened successfully.");
                }
                else
                {
                    Console.WriteLine("The file does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while opening the file: {ex.Message}");
            }
        }

        public long getFileSize(string filePath)
        {
            long fileSizeInBytes = 0;

            try
            {
                FileInfo fileInfo = new FileInfo(filePath);

                if (fileInfo.Exists)
                {
                    fileSizeInBytes = fileInfo.Length;
                    Console.WriteLine($"File size: {fileSizeInBytes} bytes");
                }
                else
                {
                    Console.WriteLine("File does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return fileSizeInBytes;
        }

        public double convertBytesToMB(long bytes)
        {
            return Math.Round((double)bytes / (1024 * 1024), 2); // 2 decimal places
        }

        public void ExportListViewToExcel(ListView listView, string filePath, DataSet dataSet)
        {
            
            Cursor.Current = Cursors.WaitCursor;
            
            try
            {
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    // Set the LicenseContext property to NonCommercial
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    string excludeFilePath = sRespFullPath + "respExcludeExportColumn.json";

                    // Load excluded columns
                    HashSet<string> excludedColumnsSet = loadExcludedColumns(excludeFilePath);

                    // Create a SaveFileDialog instance
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                        saveFileDialog.Title = "Save as Excel File";
                        saveFileDialog.InitialDirectory = sExportPath; // Set the default directory
                        saveFileDialog.FileName = filePath;
                        saveFileDialog.FilterIndex = 1;
                        saveFileDialog.RestoreDirectory = true;

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            // Copy the dataset to avoid modifying the original data
                            DataSet copyDataSet = dataSet.Copy();

                            using (ExcelPackage package = new ExcelPackage())
                            {
                                foreach (DataTable table in copyDataSet.Tables)
                                {
                                    // Create a new DataTable with only required columns
                                    DataTable filteredTable = new DataTable(table.TableName);

                                    // Add only non-excluded columns to the new DataTable
                                    foreach (DataColumn column in table.Columns)
                                    {
                                        if (!excludedColumnsSet.Contains(column.ColumnName))
                                        {
                                            filteredTable.Columns.Add(column.ColumnName, column.DataType);
                                        }
                                    }

                                    // Copy row data while maintaining the filtered columns
                                    foreach (DataRow row in table.Rows)
                                    {
                                        DataRow newRow = filteredTable.NewRow();
                                        foreach (DataColumn column in filteredTable.Columns)
                                        {
                                            newRow[column.ColumnName] = row[column.ColumnName];
                                        }
                                        filteredTable.Rows.Add(newRow);
                                    }

                                    // Create a worksheet for each DataTable
                                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(table.TableName);

                                    // Load filtered data into worksheet (starting from A1)
                                    worksheet.Cells["A1"].LoadFromDataTable(filteredTable, true);

                                    // Find the last used column dynamically
                                    int lastColumn = worksheet.Dimension.End.Column;
                                    string lastColumnLetter = GetExcelColumnName(lastColumn); // Convert to letter

                                    // Set header style: dark gray background with white text
                                    using (ExcelRange headerRange = worksheet.Cells[$"A1:{lastColumnLetter}1"])
                                    {
                                        headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        headerRange.Style.Fill.BackgroundColor.SetColor(Color.DarkGreen);
                                        headerRange.Style.Font.Color.SetColor(Color.White);
                                        headerRange.Style.Font.Bold = true;
                                    }

                                    // Auto-fit columns for better visibility
                                    worksheet.Cells.AutoFitColumns();
                                }

                                // Save the Excel package
                                Cursor.Current = Cursors.WaitCursor;

                                FileInfo excelFile = new FileInfo(saveFileDialog.FileName);
                                
                                Cursor.Current = Cursors.Default;

                                if (IsFileOpen(excelFile.FullName))
                                {
                                    MessageBox.Show("The file is already open. Please close it and try again.", "File in Use", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                else
                                {
                                    package.SaveAs(excelFile);
                                    MessageBox.Show($"Export complete. File saved to: {saveFileDialog.FileName}", "Exporting Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred in Export to Excel: \nError: {ex.Message}", "Excel Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            Cursor.Current = Cursors.Default;
        }

        public void ExportListViewToExcelWithTabSheet(ListView listView, string filePath, Dictionary<string, DataSet> dataSets, Dictionary<string, string> tabJsonTabMenu)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbFunction = new clsFunction();

            clsSearch.ClassExportStartTime = dbFunction.GetRequestTime(clsDefines.gNull);

            try
            {
                if (dataSets != null && dataSets.Count > 0)
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    string excludeFilePath = sRespFullPath + "respExcludeExportColumn.json";
                    HashSet<string> excludedColumnsSet = loadExcludedColumns(excludeFilePath);

                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                        saveFileDialog.Title = "Save as Excel File";
                        saveFileDialog.InitialDirectory = sExportPath;
                        saveFileDialog.FileName = filePath;
                        saveFileDialog.FilterIndex = 1;
                        saveFileDialog.RestoreDirectory = true;

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            using (ExcelPackage package = new ExcelPackage())
                            {
                                foreach (var tab in tabJsonTabMenu)
                                {
                                    string tabKey = tab.Key;  // Example: "tab1", "tab2", "tab3"
                                    string tabName = tab.Value; // Example: "Summary", "Detail", "Daily"

                                    Debug.WriteLine("tabName="+ tabName);

                                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(tabName);

                                    // Check if this tab has data
                                    if (dataSets.ContainsKey(tabKey) && dataSets[tabKey].Tables.Count > 0)
                                    {
                                        DataSet copyDataSet = dataSets[tabKey].Copy();

                                        foreach (DataTable table in copyDataSet.Tables)
                                        {
                                            DataTable filteredTable = new DataTable(table.TableName);

                                            // Add non-excluded columns
                                            foreach (DataColumn column in table.Columns)
                                            {
                                                if (!excludedColumnsSet.Contains(column.ColumnName))
                                                {
                                                    filteredTable.Columns.Add(column.ColumnName, column.DataType);
                                                }
                                            }

                                            // Copy row data
                                            foreach (DataRow row in table.Rows)
                                            {
                                                DataRow newRow = filteredTable.NewRow();
                                                foreach (DataColumn column in filteredTable.Columns)
                                                {
                                                    newRow[column.ColumnName] = row[column.ColumnName];
                                                }
                                                filteredTable.Rows.Add(newRow);
                                            }

                                            // Load data into worksheet
                                            worksheet.Cells["A1"].LoadFromDataTable(filteredTable, true); // true = include header
                                            
                                            // Hide Column A (column index 1)
                                            worksheet.Column(1).Hidden = false;

                                            // Set column P to bold font
                                            var columnP = worksheet.Column(16);
                                            columnP.Style.Font.Bold = true;

                                            if (worksheet.Dimension != null)
                                            {
                                                int lastColumn = worksheet.Dimension.End.Column;
                                                string lastColumnLetter = GetExcelColumnName(lastColumn); // Convert to letter
                                                int colAInt = 0;
                                                int[] highlightCols;

                                                switch (tabName)
                                                {
                                                    case "Summary":

                                                        // Hide Column A (column index 1)
                                                        worksheet.Column(1).Hidden = true;

                                                        // Remove header row if not needed
                                                        worksheet.DeleteRow(1);

                                                        using (ExcelRange headerRange = worksheet.Cells[$"A1:{lastColumnLetter}1"])
                                                        {
                                                            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                            headerRange.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#244062")); // dark blue
                                                            headerRange.Style.Font.Color.SetColor(Color.White);
                                                            headerRange.Style.Font.Bold = true;
                                                        }

                                                        // Add this block for conditional formatting
                                                        string previousValue = "";
                                                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                                                        {
                                                            var colAValue = worksheet.Cells[row, 1].Text;
                                                            var colBValue = worksheet.Cells[row, 2].Text;
                                                            var colCValue = worksheet.Cells[row, 3].Text;

                                                            if ((colBValue == "GRAND TOTAL") ||
                                                                (int.TryParse(colAValue, out colAInt) && colAInt == 7 && colBValue == "CURRENT SLA %") ||
                                                                    (int.TryParse(colAValue, out colAInt) && colAInt == 2 && colBValue == "OVERALL REQUESTS " + dbFunction.getCurrentYear() + "%" && clsSearch.ClassReportID == 53) ||
                                                                    (int.TryParse(colAValue, out colAInt) && colAInt == 3 && colBValue == "OVERALL REQUESTS") ||
                                                                    (int.TryParse(colAValue, out colAInt) && colAInt == 4 && colBValue == "REQUEST WITHIN SLA") ||
                                                                    (int.TryParse(colAValue, out colAInt) && colAInt == 6 && colBValue == "REQUEST PER TEAM LEAD") ||
                                                                    (int.TryParse(colAValue, out colAInt) && colAInt == 1 && colBValue == "NEGATIVE/UNSUCCESSFUL ACTIVITY"))
                                                            {
                                                                using (var cellRange = worksheet.Cells[row, 1, row, worksheet.Dimension.End.Column])
                                                                {
                                                                    cellRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                                    cellRange.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#244062")); // dark blue
                                                                    cellRange.Style.Font.Color.SetColor(Color.White);
                                                                    cellRange.Style.Font.Bold = true;
                                                                }
                                                                
                                                                if (int.TryParse(colAValue, out colAInt) && colAInt == 6)
                                                                {
                                                                    if (colBValue == "REQUEST PER TEAM LEAD")
                                                                    {
                                                                        highlightCols = new int[] { 4, 5, 6, 7, 8, 9 }; // D, E, F, G, H, I
                                                                        foreach (int col in highlightCols)
                                                                        {
                                                                            var cell = worksheet.Cells[row, col];
                                                                            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                                            cell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#008000")); // green
                                                                            cell.Style.Font.Color.SetColor(Color.White);
                                                                            cell.Style.Font.Bold = true;
                                                                        }

                                                                        highlightCols = new int[] { 10, 11 }; // J and K
                                                                        foreach (int col in highlightCols)
                                                                        {
                                                                            var cell = worksheet.Cells[row, col];
                                                                            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                                            cell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#FFCC00")); // orange
                                                                            cell.Style.Font.Color.SetColor(Color.White);
                                                                            cell.Style.Font.Bold = true;
                                                                        }

                                                                        highlightCols = new int[] { 12, 13, 14, 15 }; // L, M, N and O
                                                                        foreach (int col in highlightCols)
                                                                        {
                                                                            var cell = worksheet.Cells[row, col];
                                                                            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                                            cell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#FF0000")); // red
                                                                            cell.Style.Font.Color.SetColor(Color.White);
                                                                            cell.Style.Font.Bold = true;
                                                                        }
                                                                    }
                                                                    
                                                                    if (colBValue == "GRAND TOTAL")
                                                                    {
                                                                        highlightCols = new int[] { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }; // D, E, F, G, H, I, J, K, L, M, N, O
                                                                        foreach (int col in highlightCols)
                                                                        {
                                                                            var cell = worksheet.Cells[row, col];
                                                                            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                                            cell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#008000")); // green
                                                                            cell.Style.Font.Color.SetColor(Color.White);
                                                                            cell.Style.Font.Bold = true;
                                                                        }
                                                                    }
                                                                    
                                                                }
                                                                
                                                            }
                                                            else if (int.TryParse(colAValue, out colAInt) && colAInt == 0 && colBValue == "-")
                                                            {
                                                                using (var cellRange = worksheet.Cells[row, 1, row, worksheet.Dimension.End.Column])
                                                                {
                                                                    cellRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                                    cellRange.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#FFFFFF")); // white
                                                                    cellRange.Style.Font.Color.SetColor(Color.White);
                                                                    cellRange.Style.Font.Bold = true;
                                                                }
                                                            }
                                                            else if ((int.TryParse(colAValue, out colAInt) && (colAInt == 3 || colAInt == 4) && colCValue == "TOTAL") ||
                                                                    (int.TryParse(colAValue, out colAInt) && colAInt == 7 && (colBValue != "CURRENT SLA %" && colBValue != "GRAND TOTAL")))
                                                            {
                                                                using (var cellRange = worksheet.Cells[row, 1, row, worksheet.Dimension.End.Column])
                                                                {
                                                                    cellRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                                    cellRange.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#EDF7F7")); // light green
                                                                    cellRange.Style.Font.Color.SetColor(Color.Black);
                                                                    cellRange.Style.Font.Bold = true;
                                                                }
                                                            }
                                                            
                                                            // change value of B
                                                            var colBCell = worksheet.Cells[row, 2];
                                                            var currentValue = colBCell.Text;

                                                            if (currentValue == previousValue)
                                                            {
                                                                colBCell.Value = "-";
                                                            }
                                                            else
                                                            {
                                                                previousValue = currentValue;
                                                            }
                                                        }

                                                        break;                                                    
                                                    default:
                                                        using (ExcelRange headerRange = worksheet.Cells[$"A1:{lastColumnLetter}1"])
                                                        {
                                                            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                            headerRange.Style.Fill.BackgroundColor.SetColor(Color.DarkGreen);
                                                            headerRange.Style.Font.Color.SetColor(Color.White);
                                                            headerRange.Style.Font.Bold = true;
                                                        }
                                                        
                                                        break;
                                                }

                                                worksheet.Cells.AutoFitColumns();
                                            }                                            
                                        }
                                    }
                                }
                                
                                // Save the file
                                Cursor.Current = Cursors.WaitCursor;
                                FileInfo excelFile = new FileInfo(saveFileDialog.FileName);
                                Cursor.Current = Cursors.Default;

                                if (IsFileOpen(excelFile.FullName))
                                {
                                    MessageBox.Show("The file is already open. Please close it and try again.", "File in Use", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                {
                                    // update
                                    string pSearchValue = $"{clsSearch.ClassReportType}{clsDefines.gPipe}{(int)Enums.ReportProcessType.Idle}{clsDefines.gPipe}" +
                                        $"{clsDefines.REPORT_STATUS_IDLE}{clsDefines.gPipe}" +
                                        $"{dbFunction.getCurrentDateTime()}{clsDefines.gPipe}" +
                                        $"{clsUser.ClassUserFullName}";

                                    dbAPI = new clsAPI();
                                    dbAPI.ExecuteAPI("PUT", "Update", "Report Status", pSearchValue, "", "", "UpdateCollectionDetail");

                                    clsSearch.ClassExportEndTime = dbFunction.GetResponseTime(clsDefines.gNull);

                                    package.SaveAs(excelFile);
                                    
                                    // Prompt for process completion                                
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

                                                    $"Export complete.\nFile saved to:\n{saveFileDialog.FileName}";

                                    MessageBox.Show(message, "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred in Export to Excel: \nError: {ex.Message}");
                MessageBox.Show($"An error occurred in Export to Excel: \nError: {ex.Message}", "Excel Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Cursor.Current = Cursors.Default;
        }
        
        public HashSet<string> loadExcludedColumns(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) return new HashSet<string>();

                string jsonContent = File.ReadAllText(filePath);
                JObject jsonObj = JObject.Parse(jsonContent);

                if (jsonObj["data"] is JArray dataArray)
                {
                    return new HashSet<string>(
                        dataArray
                            .Select(item => item["Header"]?.ToString().Trim())
                            .Where(header => !string.IsNullOrEmpty(header)),
                        StringComparer.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading excluded columns file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return new HashSet<string>();
        }

        public bool IsFileOpen(string filePath)
        {
            if (!File.Exists(filePath))
                return false; // File does not exist, so it cannot be open

            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return false; // File is accessible, meaning it's not open elsewhere
                }
            }
            catch (IOException)
            {
                return true; // File is in use
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error checking file lock: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
        }

        // Helper function to convert column index to Excel letter
        string GetExcelColumnName(int columnNumber)
        {
            string columnName = "";
            while (columnNumber > 0)
            {
                int remainder = (columnNumber - 1) % 26;
                columnName = (char)(65 + remainder) + columnName;
                columnNumber = (columnNumber - 1) / 26;
            }
            return columnName;
        }

        public void emptyFolder(string folderPath)
        {
            Debug.WriteLine("--emptyFolder--");
            
            clsFunction dbFunction = new clsFunction();
            
            string protectedPath = @sApplicationMainPath;
            string normalizedFolderPath = folderPath.TrimEnd('\\');
            string normalizedProtectedPath = protectedPath.TrimEnd('\\');

            Debug.WriteLine("folderPath=" + folderPath);
            Debug.WriteLine("protectedPath=" + protectedPath);

            if (string.IsNullOrWhiteSpace(folderPath) ||
             normalizedFolderPath.Equals(normalizedProtectedPath, StringComparison.OrdinalIgnoreCase) ||
             normalizedFolderPath.StartsWith(normalizedProtectedPath + "\\", StringComparison.OrdinalIgnoreCase))
            {
                dbFunction.SetMessageBox("Access to this folder is not allowed. Operation cancelled.", "Folder", clsFunction.IconType.iWarning);
                return;
            }
            
            if (!Directory.Exists(folderPath))
            {
                Debug.WriteLine("Folder does not exist.");
                dbFunction.SetMessageBox("Folder does not exist.", "Folder", clsFunction.IconType.iError);
                return;
            }

            try
            {
                // Delete all files
                foreach (string file in Directory.GetFiles(folderPath))
                {
                    File.Delete(file);
                }

                // Delete all subdirectories
                foreach (string dir in Directory.GetDirectories(folderPath))
                {
                    Directory.Delete(dir, true); // 'true' means delete recursively
                }

                Debug.WriteLine("Folder emptied successfully.");
                dbFunction.SetMessageBox("Folder emptied successfully.", "Folder", clsFunction.IconType.iInformation);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while emptying folder: " + ex.Message);
                dbFunction.SetMessageBox("Error while emptying folder: " + ex.Message, "Folder", clsFunction.IconType.iError);
            }
        }

        public void deleteLogFile(string pLogFilePath)
        {
            long MaxFileSize = 10 * 1024 * 1024; // 10 MB

            if (File.Exists(pLogFilePath))
            {
                FileInfo fi = new FileInfo(pLogFilePath);
                if (fi.Length > MaxFileSize)
                {
                    File.Delete(pLogFilePath);
                }
            }
        }

        public void WriteSysytemLog(string message)
        {
            // Include both date and time: [yyyy-MM-dd HH:mm:ss]
            //string timePrefix = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ";
            //string logEntry = $"{timePrefix} {clsSearch.ClassBankCode} : {message}";

            // write log
            WriteAPILog(4, message);
        }

        public void ExportMultipleListViewsToExcel(ReportInfo[] reports, string defaultFileName, bool isExclude)
        {
            if (reports == null || reports.Length == 0)
            {
                MessageBox.Show("No reports to export.", "Export to Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                HashSet<string> excludedColumnsSet = new HashSet<string>();
                if (isExclude)
                {
                    string excludeFilePath = Path.Combine(sRespFullPath, "respExcludeExportColumn.json");
                    if (File.Exists(excludeFilePath))
                        excludedColumnsSet = loadExcludedColumns(excludeFilePath);
                }

                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                    saveFileDialog.Title = "Save as Excel File";
                    saveFileDialog.InitialDirectory = sExportPath;
                    saveFileDialog.FileName = defaultFileName;
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (ExcelPackage package = new ExcelPackage())
                        {
                            foreach (var report in reports)
                            {
                                ListView listView = report.ListView;
                                if (listView == null || listView.Items.Count == 0)
                                    continue;

                                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(report.SheetName);

                                // Set font
                                worksheet.Cells.Style.Font.Name = "Courier New";
                                worksheet.Cells.Style.Font.Size = 10;

                                int currentRow = 1;

                                // --- HEADER ---
                                worksheet.Cells[currentRow, 1].Value = report.Title;
                                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                                worksheet.Cells[currentRow, 1].Style.Font.Size = 14;
                                currentRow++;

                                worksheet.Cells[currentRow, 1].Value = $"Count: {report.Count}";
                                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                                currentRow++;

                                worksheet.Cells[currentRow, 1].Value = $"Total: {report.Total}";
                                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                                currentRow += 2;

                                // --- TABLE HEADER ---
                                int colIndex = 1;
                                foreach (ColumnHeader header in listView.Columns)
                                {
                                    if (!isExclude || !excludedColumnsSet.Contains(header.Text))
                                    {
                                        worksheet.Cells[currentRow, colIndex].Value = header.Text;
                                        colIndex++;
                                    }
                                }

                                using (ExcelRange headerRange = worksheet.Cells[currentRow, 1, currentRow, colIndex - 1])
                                {
                                    headerRange.Style.Font.Name = "Courier New";
                                    headerRange.Style.Font.Size = 10;
                                    headerRange.Style.Font.Bold = true;
                                    headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    headerRange.Style.Fill.BackgroundColor.SetColor(Color.DarkGreen);
                                    headerRange.Style.Font.Color.SetColor(Color.White);
                                    headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                }

                                currentRow++;

                                // --- TABLE DATA ---
                                foreach (ListViewItem item in listView.Items)
                                {
                                    colIndex = 1;
                                    for (int i = 0; i < listView.Columns.Count; i++)
                                    {
                                        string columnName = listView.Columns[i].Text.Trim();
                                        if (!isExclude || !excludedColumnsSet.Contains(columnName))
                                        {
                                            string value = item.SubItems[i].Text.Trim();
                                            double numericValue;

                                            bool isNonAmountColumn =
                                                (i == 0) ||
                                                columnName.Equals("Line#", StringComparison.OrdinalIgnoreCase) ||
                                                columnName.Equals("Line", StringComparison.OrdinalIgnoreCase) ||
                                                columnName.Equals("TID", StringComparison.OrdinalIgnoreCase);

                                            if (!isNonAmountColumn && double.TryParse(value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out numericValue))
                                            {
                                                worksheet.Cells[currentRow, colIndex].Value = numericValue;
                                                worksheet.Cells[currentRow, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                                worksheet.Cells[currentRow, colIndex].Style.Numberformat.Format = "#,##0.00";
                                            }
                                            else
                                            {
                                                worksheet.Cells[currentRow, colIndex].Value = value;
                                                worksheet.Cells[currentRow, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            }

                                            worksheet.Cells[currentRow, colIndex].Style.Font.Name = "Courier New";
                                            worksheet.Cells[currentRow, colIndex].Style.Font.Size = 10;

                                            colIndex++;
                                        }
                                    }
                                    currentRow++;
                                }

                                worksheet.Cells.AutoFitColumns();
                            }

                            FileInfo excelFile = new FileInfo(saveFileDialog.FileName);

                            if (IsFileOpen(excelFile.FullName))
                            {
                                MessageBox.Show("The file is already open. Please close it and try again.", "File in Use", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                package.SaveAs(excelFile);
                                MessageBox.Show($"Export complete.\nFile saved to:\n{saveFileDialog.FileName}", "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during export:\n{ex.Message}", "Excel Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        public void LoadExcelToListView(string filePath, ListView listView)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        MessageBox.Show($"No worksheet found in file: {filePath}");
                        return;
                    }

                    int startRow = worksheet.Dimension.Start.Row;
                    int endRow = worksheet.Dimension.End.Row;
                    int startCol = worksheet.Dimension.Start.Column;
                    int endCol = worksheet.Dimension.End.Column;

                    // Create columns (from header row)
                    listView.Columns.Clear();
                    listView.Columns.Add("Line#", 60);

                    for (int col = startCol; col <= endCol; col++)
                    {
                        string header = worksheet.Cells[startRow, col].Text.Trim();
                        if (string.IsNullOrEmpty(header)) header = $"Column {col}";
                        listView.Columns.Add(header, 150);
                    }

                    // Add rows
                    int lineNumber = 1;
                    for (int row = startRow + 1; row <= endRow; row++) // skip header
                    {
                        ListViewItem item = new ListViewItem(lineNumber.ToString());

                        for (int col = startCol; col <= endCol; col++)
                        {
                            string value = worksheet.Cells[row, col].Text;
                            item.SubItems.Add(value);
                        }

                        listView.Items.Add(item);
                        lineNumber++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Excel file:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Cursor.Current = Cursors.Default;
        }

        public void convertExcelToCsv(string xlsPath, string exportFolder, string fileName)
        {
            try
            {
                if (!File.Exists(xlsPath))
                {
                    Console.WriteLine($"Input file does not exist: {xlsPath}");
                    return;
                }

                // Ensure export folder exists
                if (!Directory.Exists(exportFolder))
                    Directory.CreateDirectory(exportFolder);

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(new FileInfo(xlsPath)))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        Console.WriteLine($"No worksheet found in: {xlsPath}");
                        return;
                    }

                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;

                    // Build full output file path
                    string safeFileName = Path.GetFileNameWithoutExtension(fileName) + clsDefines.FILE_EXT_CSV;
                    string csvFile = Path.Combine(exportFolder, safeFileName);

                    using (var writer = new StreamWriter(csvFile, false, System.Text.Encoding.UTF8))
                    {
                        for (int row = 1; row <= rowCount; row++)
                        {
                            var rowValues = new string[colCount];
                            for (int col = 1; col <= colCount; col++)
                            {
                                // Clean and escape text
                                string cellValue = worksheet.Cells[row, col].Text ?? "";
                                rowValues[col - 1] = StrClean(cellValue);
                            }

                            writer.WriteLine(string.Join(",", rowValues));
                        }
                    }

                    Console.WriteLine($"Excel file converted to CSV: {csvFile}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting file '{xlsPath}': {ex.Message}");
            }
        }

        public bool IsFileInUse(string filePath)
        {
            if (!File.Exists(filePath))
                return false; // or throw if you prefer

            try
            {
                using (FileStream stream = new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.ReadWrite,
                    FileShare.None))
                {
                    // file is NOT in use
                    return false;
                }
            }
            catch (IOException)
            {
                // file is in use (locked by another process)
                return true;
            }
        }
    }
}
