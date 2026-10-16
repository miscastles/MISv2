// Model/clsBankData.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;

namespace MIS
{
    public class clsExcel
    {
        public const string BANKDATA_SHEET_NAME = "__MIS_BANK_DATA";

        public bool RecordFound;

        public Dictionary<string, string> _BankDataList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        
        public Dictionary<string, string> ClassBankDataList
        {
            get { return _BankDataList; }
            set { _BankDataList = value; }
        }
        public bool ClassIsConverted
        {
            get { return _BankDataList.Count > 0; }
        }


        public void Reset()
        {
            RecordFound = false;
            _BankDataList.Clear();
        }

        public void getExcelMetadata(string filePath)
        {
            clsFunction dbFunction = new clsFunction();

            Reset();

            if (!dbFunction.CheckPathExtensionMatch(filePath, "xlsx"))
            {
                return;
            }

            try
            {
                if (!string.Equals(Path.GetExtension(filePath), ".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    RecordFound = true;
                    return;
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[BANKDATA_SHEET_NAME];

                    if (worksheet == null)
                    {
                        RecordFound = true;
                        return;
                    }

                    if (worksheet.Dimension == null || worksheet.Dimension.End.Row < 2)
                    {
                        dbFunction.SetMessageBox("The converted file contains an empty or invalid '" + BANKDATA_SHEET_NAME + "' worksheet.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                        return;
                    }

                    string[] expectedHeaders = { "SchemaVersion", "SourceBank", "RequestID", "SourceFileName", "BankDataInfo" };
                    string sList = "";

                    bool fValid = true;

                    for (int i = 0; i < expectedHeaders.Length; i++)
                    {
                        string header = Convert.ToString(worksheet.Cells[1, i + 1].Text).Trim();

                        if (header.ToUpper() != expectedHeaders[i].ToUpper())
                        {
                            sList = sList + expectedHeaders[i] + "\n";
                            fValid = false;
                        }
                    }

                    if (!fValid)
                    {
                        Debug.WriteLine("getExcelMetadata, header mismatch...");
                        Debug.WriteLine(sList);

                        dbFunction.SetMessageBox("Invalid converted metadata header.\n\nExpected header(s):\n" + sList, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                        return;
                    }

                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        string schemaversion = Convert.ToString(worksheet.Cells[row, 1].Text).Trim();
                        string sourcebank = Convert.ToString(worksheet.Cells[row, 2].Text).Trim();
                        string requestid = Convert.ToString(worksheet.Cells[row, 3].Text).Trim();
                        string sourcefilename = Convert.ToString(worksheet.Cells[row, 4].Text).Trim();
                        string bankdatainfo = Convert.ToString(worksheet.Cells[row, 5].Value).Trim();

                        if (string.IsNullOrWhiteSpace(schemaversion)
                            && string.IsNullOrWhiteSpace(sourcebank)
                            && string.IsNullOrWhiteSpace(requestid)
                            && string.IsNullOrWhiteSpace(sourcefilename)
                            && string.IsNullOrWhiteSpace(bankdatainfo))
                        {
                            continue;
                        }

                        if (schemaversion.CompareTo("1") != 0)
                        {
                            dbFunction.SetMessageBox("Line#: " + row + "\n\nUnsupported converted metadata schema version.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                            return;
                        }

                        if (sourcebank.ToUpper() != "BDO")
                        {
                            dbFunction.SetMessageBox("Line#: " + row + "\n\nInvalid source bank in converted metadata. Expected BDO.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(requestid))
                        {
                            dbFunction.SetMessageBox("Line#: " + row + "\n\nRequest ID is missing from converted metadata.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(sourcefilename))
                        {
                            dbFunction.SetMessageBox("Request ID: [" + requestid + "]\n\nSource filename is missing from converted metadata.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(bankdatainfo))
                        {
                            dbFunction.SetMessageBox("Request ID: [" + requestid + "]\n\nBank data JSON is missing from converted metadata.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                            return;
                        }

                        try
                        {
                            JToken.Parse(bankdatainfo);
                        }
                        catch (JsonReaderException ex)
                        {
                            Debug.WriteLine("getExcelMetadata, invalid JSON, error=" + ex.Message);

                            dbFunction.SetMessageBox("Request ID: [" + requestid + "]\n\nBank data JSON is invalid.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                            return;
                        }

                        if (_BankDataList.ContainsKey(requestid))
                        {
                            dbFunction.SetMessageBox("Duplicate Request ID found in converted metadata: [" + requestid + "].", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                            return;
                        }

                        _BankDataList.Add(requestid, bankdatainfo);
                    }

                    if (_BankDataList.Count == 0)
                    {
                        dbFunction.SetMessageBox("The converted file does not contain any bank metadata records.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                        return;
                    }
                }

                RecordFound = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("getExcelMetadata, error=" + ex.Message);
                dbFunction.SetMessageBox("Unable to read converted metadata.\n\nError: " + ex.Message, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }
        }
    }
}