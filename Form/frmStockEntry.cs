using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using MIS.Model;
using Spire.Xls;
using Newtonsoft.Json;
using static MIS.Function.AppUtilities;
using System.IO;

namespace MIS
{
    public partial class frmStockEntry : Form
    {
        public static bool fAutoLoadData = false;

        private clsAPI dbAPI;
        private clsFunction dbFunction;
        private clsFile dbFile;
        private clsINI dbINI;

        bool fEdit = false;
        private static string ExcelFilePath = @"";
        private string sExcelFileName = "";
        private string sSheetName = "";
        private string sSheet = "";

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED
                //cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        public frmStockEntry()
        {
            InitializeComponent();
        }

        private void frmStockEntry_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
                case Keys.F2:
                    btnSearchSN_Click(this, e);
                    break;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmStockEntry_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            dbFile = new clsFile();
            dbINI = new clsINI();

            dbINI.InitDatabaseSetting();
            dbINI.InitSystemSetting();

            dbFunction.ClearTextBox(this);
            dbFunction.ClearComboBox(this);

            dbAPI.FillComboBoxClient(cboClient);
            dbAPI.FillComboBoxClient(cboIClient);
            dbAPI.FillComboBoxFE(cboAllocation);
            dbAPI.FillComboBoxLocation(cboLocation);
            dbAPI.FillComboBoxAssetType(cboAssetType);
            dbAPI.FillComboBoxTerminalBase(cboType);            
            dbAPI.FillComboBoxTerminalBrand(cboBrand);
            dbAPI.FillComboBoxTerminalStatus(cboStatus);

            // Load Mapping
            dbAPI.ExecuteAPI("GET", "View", "Type", "TERMINAL", "Mapping", "", "ViewMapping");

            fEdit = false;
            InitButton();

            btnSearchSN.Enabled = true;
            btnClearTerminalSN.Enabled = true;

            dbFunction.initTabSelection(tabComponent, 1);

            if (fAutoLoadData)
            {
                btnSearchSN_Click(this, e);
                fAutoLoadData = false;
            }

            Cursor.Current = Cursors.Default;
        }

        private void cboModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassTerminalModelID = 0;
            if (!cboModel.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Terminal Model", cboModel.Text);
                clsSearch.ClassTerminalModelID = clsSearch.ClassOutFileID;
            }
            
        }

        private void cboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassTerminalTypeID = 0;
            if (!cboType.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Terminal Type", cboType.Text);
                clsSearch.ClassTerminalTypeID = clsSearch.ClassOutFileID;


                dbAPI.FillComboBoxTerminalModelByTerminalType(cboModel, clsSearch.ClassTerminalTypeID.ToString());
            }
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ExcelDialog = new OpenFileDialog();
            ExcelDialog.Filter = "Excel Files|*.xls;*.xlsx";
            ExcelDialog.InitialDirectory = @"C:\CASTLESTECH_MIS\IMPORT\";
            ExcelDialog.Title = "Select file to import...";

            if (ExcelDialog.ShowDialog() == DialogResult.OK)
            {
                ExcelFilePath = ExcelDialog.FileName;
                txtPathFileName.Text = ExcelDialog.FileName;
                txtPathFileName.ReadOnly = true;
                btnLoadFile.Enabled = false;
                sExcelFileName = Path.GetFileName(txtPathFileName.Text);
                
                try
                {
                    // Waiting / Hour Glass
                    Cursor.Current = Cursors.WaitCursor;

                    sSheet = dbFunction.getSheetName(txtPathFileName.Text);
                    if (!dbFunction.isValidSheetName(sSheet)) return;

                    sSheetName = "`" + sSheet + "$" + "`";
                    txtFileName.Text = sExcelFileName;

                    // Create Temporary database                
                    dbAPI.ExecuteAPI("POST", "Create", "", "", sSheetName, "", "CreateTempTable");

                    dbFunction.ClearDataGrid(grdDummy);
                    dbFunction.ClearDataGrid(grdList);

                    dbFunction.ImportToDummyDataGrid(grdDummy, sSheet, txtPathFileName.Text);

                    // Back to normal 
                    Cursor.Current = Cursors.Default;

                    btnLoadFile.Enabled = false;

                    // Start Import Here
                    if (!dbFunction.fPromptConfirmation("Are you sure to import?"))
                    {
                        btnImportCancel_Click(this, e);
                        return;
                    }

                    Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass

                    txtIStockID.Text = dbAPI.GetControlID("Stock Master").ToString(); // get control id
                    dbAPI.GenerateID(true, txtIRefNo, txtIStockID, "Stock Master", clsDefines.CONTROLID_PREFIX_IMPORT_BASE); // generate reference no

                    // Drop table create from sheetname
                    if (sSheetName.Length > 0)
                        dbAPI.ExecuteAPI("DELETE", "Delete", "", sSheetName.Replace("`", ""), "Drop Temp Table", "", "DeleteCollectionDetail");
                    
                    ImportToDataGrid();

                    if (!isValidHeader()) return; // Check Header

                    // check for mandatory fields
                    if (!dbFunction.isValidDataGridValue(clsFunction.ImportType.iImportStock, dbAPI, grdDummy, txtFileName.Text, 0, true))
                        return;

                    // check field column is required
                    if (!dbFunction.isValidDataGridValue(clsFunction.ImportType.iImportStock, dbAPI, grdDummy, txtFileName.Text, 1, true)) return;

                    TotalCount();

                    btnImportSave.Enabled = true;

                    cboIClient.Enabled = true;

                    Cursor.Current = Cursors.Default; // Back to normal 

                    dbFunction.SetMessageBox("Import of file " +
                                             "\n" +
                                             "[ " + txtFileName.Text + " ]" + " " + "is complete." +
                                             "\n\n" +
                                             "You may save import now."
                                             , "Import Terminal", clsFunction.IconType.iInformation);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception error " + ex.Message);
                    MessageBox.Show(ex.Message);
                    btnLoadFile.Enabled = true;
                    return;
                }

            }
        }

        private void btnImportCancel_Click(object sender, EventArgs e)
        {
            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);

            dbFunction.ClearDataGrid(grdDummy);
            dbFunction.ClearDataGrid(grdList);
            
            fEdit = false;
            
            ucStatusImport.iState = 4;
            ucStatusImport.sMessage = "";
            ucStatusImport.AnimateStatus();

            btnLoadFile.Enabled = true;
        }

        private void TotalCount()
        {
            if (grdList.RowCount > 0)
            {
                grdList.Rows[0].Selected = false;

                ucStatusImport.iState = 3;
                ucStatusImport.sMessage = "TOTAL LINE(S): " + grdList.RowCount.ToString();
                ucStatusImport.AnimateStatus();
            }
        }

        private void ImportToDataGrid()
        {
            int iRowCount = 0;
            int iColCount = 0;
            int i, x = 0;
            int y = 1;
            int ii = 0;

            Debug.WriteLine("--ImportToDataGrid--");

            // Delivery Date            
            DateTime DDateTime = DateTime.Now;

            // Received Date        
            DateTime RDateTime = DateTime.Now;

            dbFunction.InitDataGridView(grdList);

            iRowCount = grdDummy.RowCount;
            iColCount = 15;
          
            DateTime dteTemp = DateTime.Now;
            int iDeliveryDate = dbFunction.GetMapColumnIndex(clsDefines.HDR_TEMPLATE_DeliveryDate);
            int iReceiveDate = dbFunction.GetMapColumnIndex(clsDefines.HDR_TEMPLATE_ReceivedDate);

            // Add Column and Set Header            
            grdList.ColumnCount = iColCount;
            for (i = 0; i < iColCount; i++)
            {
                string cellParam = grdDummy.Columns[i].HeaderText;
                cellParam = cellParam.Replace("\n", " ").Trim();

                if (i > 0)
                {
                    if (cellParam.CompareTo("SERIAL NO") == 0)
                        grdList.Columns[i].Width = 180;
                    else
                        grdList.Columns[i].Width = 130;
                }
                else
                {
                    grdList.Columns[i].Width = 60;
                }

                grdList.Columns[i].Name = cellParam;

                y = i + 1;
                Debug.WriteLine(y.ToString() + "," + cellParam + "," + cellParam + "," + "0" + "," + y.ToString() + "," + "TERMINAL");
            }

            for (i = 0; i < iRowCount; i++)
            {
                ii++;

                string sLineNo = grdDummy.Rows[i].Cells[0].Value.ToString(); // Check is not blank
                string sFieldCheck = grdDummy.Rows[i].Cells[1].Value.ToString(); // Check is not blank

                Debug.WriteLine("ImportToDataGrid::i=" + i.ToString() + "," + "sLineNo=" + sLineNo + "," + "sFieldCheck=" + sFieldCheck);

                ucStatusImport.iState = 3;
                ucStatusImport.sMessage = "LINE#" + ii + " of " + iRowCount + " ->SN " + dbFunction.AddBracketStartEnd(sFieldCheck);
                ucStatusImport.AnimateStatus();

                if (sFieldCheck.Trim().Length > 0)
                {
                    int iIndex = grdList.Rows.Add();
                    for (x = 0; x < iColCount; x++)
                    {
                        string cellValue = grdDummy.Rows[i].Cells[x].Value.ToString().ToUpper();
                        
                        // special chraracter cleanup
                        //cellValue = cellValue.Replace("&", "AND");
                        cellValue = cellValue.Replace("|", clsFunction.sNull);
                        cellValue = cellValue.Replace("~", clsFunction.sNull);
                        cellValue = cellValue.Replace("^", clsFunction.sNull);
                        cellValue = cellValue.Replace("%", clsFunction.sNull);

                        if (cellValue.Length > 0)
                            grdList.Rows[iIndex].Cells[x].Value = cellValue.ToUpper();
                        else
                            grdList.Rows[iIndex].Cells[x].Value = clsFunction.sDash;

                    }
                }
            }

            dbFunction.DataGridViewAlternateBackColor(grdList);

        }

        private bool isValidHeader()
        {
            int i = 0;
            string sList = "";
            string sMapFrom = "";
            bool fExist = true;

            dbAPI.ExecuteAPI("GET", "View", "Type", "TERMINAL", "Mapping", "", "ViewMapping");

            Debug.WriteLine("grdList.ColumnCount=" + grdList.ColumnCount);
            Debug.WriteLine("clsMapping.ClassRecordCount=" + clsMapping.ClassRecordCount);
            Debug.WriteLine("clsArray.MapFrom.Length=" + clsArray.MapFrom.Length);

            for (i = 0; i < clsArray.MapFrom.Length; i++)
            {
                sMapFrom = clsArray.MapFrom[i];
                if (!dbFunction.isHeaderExist(grdList, sMapFrom))
                {
                    sList = sList + sMapFrom + "\n";
                    fExist = false;
                }
            }

            if (!fExist)
            {
                MessageBox.Show("Mapping not found on import file\n\n" + "Header: " + grdList.ColumnCount + "\n" + "Mapping: " + clsMapping.ClassRecordCount + "\n\n" + "Mapping list: \n" + sList, "Note", MessageBoxButtons.OK, MessageBoxIcon.Error);

                dbFunction.ClearDataGrid(grdDummy);
                dbFunction.ClearDataGrid(grdList);

                dbFunction.ClearTextBox(this);
                btnLoadFile.Enabled = true;

                return false;
            }

            return true;
        }

        private void btnImportSave_Click(object sender, EventArgs e)
        {
            if (!dbFunction.fPromptConfirmation("Are you sure to import" + "\n\n" +
                "File: " + dbFunction.AddBracketStartEnd(txtFileName.Text))) return;

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                saveStockMaster();

                saveStockDetail();

                // ------------------------------------------------------------------------------------------
                // Upload physical attach file
                // ------------------------------------------------------------------------------------------                
                ucStatusImport.iState = 3;
                ucStatusImport.sMessage = "UPLOADING PHYSICAL FILE :" + txtFileName.Text;                
                ucStatusImport.AnimateStatus();

                ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                ftpClient.delete(clsGlobalVariables.strFTPUploadPath + txtFileName.Text);
                ftpClient.upload(clsGlobalVariables.strFTPUploadPath + txtFileName.Text, txtPathFileName.Text.Replace(txtFileName.Text, "") + txtFileName.Text);
                ftpClient.disconnect(); // ftp disconnect

            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox("Message " + ex.Message, "Terminal import failed", clsFunction.IconType.iError);
            }

            Cursor.Current = Cursors.Default; // Back to normal 

            dbFunction.SetMessageBox("Import file " + dbFunction.AddBracketStartEnd(txtFileName.Text) + " successfully saved.", "Import complete", clsFunction.IconType.iInformation);

            btnImportCancel_Click(this, e);

            Cursor.Current = Cursors.Default;

        }

        private void saveStockMaster()
        {
            Debug.WriteLine("--saveStockMaster--");

            string sSQL = "";

            var data = new
            {
                ProcessDate = dbFunction.getCurrentDate(),
                ProcessTime = dbFunction.getCurrentTime(),
                UserID = clsSearch.ClassCurrentUserID,
                FileName = dbFunction.CheckAndSetStringValue(txtFileName.Text),
                FullPath = dbFunction.CheckAndSetStringValue(txtPathFileName.Text),
                ReferenceNo = dbFunction.CheckAndSetStringValue(txtIRefNo.Text),
                Remarks = dbFunction.CheckAndSetStringValue(txtIRemarks.Text)
            };

            sSQL = IFormat.Insert(data);

            Debug.WriteLine("saveStockMaster" + sSQL);

            dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 0);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "Stock Master", sSQL, "InsertCollectionMaster");

            clsSearch.ClassLastInsertedID = clsLastID.ClassLastInsertedID;
            txtIStockID.Text = clsSearch.ClassLastInsertedID.ToString();
        }

        private void saveStockDetail()
        {
            Debug.WriteLine("--saveStockDetail--");

            bool isValid = true;
            string sTempSQL = "";
            string sRowSQL = "";
            string sSQL = "";
            string sRowCSV = "";
            //string sCSV = "";
            int iRowCount = grdList.RowCount;
            int iColCount = grdList.ColumnCount;
            int ii = 0;

            int i = 0;
            List<string> TempArrayDataCol = new List<String>();
            int iRecordMinLimit = clsSystemSetting.ClassSystemRecordMinLimit;
            int iStartIndex = 0;
            int iEndIndex = 0;
            int iFileNameIndex = 0;
            int x;
            StringBuilder columnbind = new StringBuilder();

            
            string pData = "";

            int iSNColIndex = dbFunction.GetDataGridHeaderColumnIndex(grdList, clsDefines.HDR_TEMPLATE_SerialNo);
            int iTypeColIndex = dbFunction.GetDataGridHeaderColumnIndex(grdList, clsDefines.HDR_TEMPLATE_Type);
            int iModelColIndex = dbFunction.GetDataGridHeaderColumnIndex(grdList, clsDefines.HDR_TEMPLATE_Model);
            int iBrandColIndex = dbFunction.GetDataGridHeaderColumnIndex(grdList, clsDefines.HDR_TEMPLATE_Brand);
            int iStatusColIndex = dbFunction.GetDataGridHeaderColumnIndex(grdList, clsDefines.HDR_TEMPLATE_Status);
            int iClientColIndex = dbFunction.GetDataGridHeaderColumnIndex(grdList, clsDefines.HDR_TEMPLATE_Allocation);
            int iLocationColIndex = dbFunction.GetDataGridHeaderColumnIndex(grdList, clsDefines.HDR_TEMPLATE_Location);

            if (iRowCount > 0)
            {
                for (i = 0; i < iRowCount; i++)
                {
                    ii++;
                    sRowSQL = "";
                    sSQL = "";
                    
                    string pType = dbFunction.CheckAndSetStringValue(grdList.Rows[i].Cells[iTypeColIndex].Value.ToString().Trim().ToUpper());
                    dbFunction.GetIDFromFile("Terminal Type", pType);
                    int pTypeID = clsSearch.ClassOutFileID;

                    string pModel = dbFunction.CheckAndSetStringValue(grdList.Rows[i].Cells[iModelColIndex].Value.ToString().Trim().ToUpper());
                    dbFunction.GetIDFromFile("Terminal Model", pModel);
                    int pModelID = clsSearch.ClassOutFileID;

                    string pBrand = dbFunction.CheckAndSetStringValue(grdList.Rows[i].Cells[iBrandColIndex].Value.ToString().Trim().ToUpper());
                    dbFunction.GetIDFromFile("Terminal Brand", pBrand);
                    int pBrandID = clsSearch.ClassOutFileID;

                    string pLocation = dbFunction.CheckAndSetStringValue(grdList.Rows[i].Cells[iLocationColIndex].Value.ToString().Trim().ToUpper());
                    dbFunction.GetIDFromFile("Location", pLocation);
                    int pLocationID = clsSearch.ClassOutFileID;

                    string pStockStatus = dbFunction.CheckAndSetStringValue(grdList.Rows[i].Cells[iStatusColIndex].Value.ToString().Trim().ToUpper());
                    dbFunction.GetIDFromFile("Status List", pStockStatus);
                    int pStatusID = clsSearch.ClassOutFileID;

                    if (!dbFunction.isValidID(pTypeID.ToString()) || 
                        !dbFunction.isValidID(pModelID.ToString()) || 
                        !dbFunction.isValidID(pBrandID.ToString()) || 
                        !dbFunction.isValidID(pLocationID.ToString()) ||
                        !dbFunction.isValidID(pStatusID.ToString()))
                    {
                        isValid = false;
                        dbFunction.SetMessageBox("Invalid data found." + "\n\n" +
                            "Status:" + dbFunction.AddBracketStartEnd(pStockStatus) + dbFunction.AddBracketStartEnd(pStatusID.ToString()) + "\n" +
                            "Type:" + dbFunction.AddBracketStartEnd(pType) + dbFunction.AddBracketStartEnd(pTypeID.ToString()) + "\n" +
                            "Model: " + dbFunction.AddBracketStartEnd(pModel) + dbFunction.AddBracketStartEnd(pModelID.ToString()) + "\n" +
                            "Brand: " + dbFunction.AddBracketStartEnd(pBrand) + dbFunction.AddBracketStartEnd(pBrandID.ToString()) + "\n" +
                            "Location: " + dbFunction.AddBracketStartEnd(pLocation) + dbFunction.AddBracketStartEnd(pLocationID.ToString()) +
                            "\n\n" +
                            "Description above not exist on the server.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);

                        return;
                    }

                    var data = new
                    {
                        ClientID = dbFunction.CheckAndSetStringValue(txtIClientID.Text),
                        StockID = dbFunction.CheckAndSetStringValue(txtIStockID.Text),
                        LineNo = dbFunction.CheckAndSetStringValue(grdList.Rows[i].Cells[0].Value.ToString().Trim().ToUpper()),
                        SerialNo = dbFunction.CheckAndSetStringValue(grdList.Rows[i].Cells[1].Value.ToString().Trim().ToUpper()),
                        PartNo = dbFunction.CheckAndSetStringValue(grdList.Rows[i].Cells[2].Value.ToString().Trim().ToUpper()),
                        TypeID = pTypeID,
                        ModelID = pModelID,
                        BrandID = pBrandID,
                        DeliveryDate = dbFunction.CheckAndSetStringValue(clsFunction.FormatCharAndDate(grdList.Rows[i].Cells[6].Value.ToString().Trim().ToUpper())),
                        ReceivedDate = dbFunction.CheckAndSetStringValue(clsFunction.FormatCharAndDate(grdList.Rows[i].Cells[7].Value.ToString().Trim().ToUpper())),
                        LocationID = pLocationID,
                        PONo = dbFunction.CheckAndSetStringValue(grdList.Rows[i].Cells[9].Value.ToString().Trim().ToUpper()),
                        InvoiceNo = dbFunction.CheckAndSetStringValue(grdList.Rows[i].Cells[10].Value.ToString().Trim().ToUpper()),
                        StockStatatus = pStatusID,
                        Allocation = dbFunction.CheckAndSetStringValue(grdList.Rows[i].Cells[12].Value.ToString().Trim().ToUpper()),
                        AssetType = dbFunction.CheckAndSetStringValue(grdList.Rows[i].Cells[13].Value.ToString().Trim().ToUpper()),
                        Remarks = dbFunction.CheckAndSetStringValue(grdList.Rows[i].Cells[14].Value.ToString().Trim().ToUpper())                        
                    };

                    sSQL = IFormat.Insert(data);

                    // Concatenate rawdata_info
                    //sSQL = sSQL + "," + dbFunction.AddSingleQuote(dbFunction.getDataGridRowDataInJSON(grdList, i));

                    Debug.WriteLine("sSQL" + sSQL);     
                    dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 1);

                    sRowCSV = "";
                    sRowCSV = sSQL.Replace("(", "");
                    sRowCSV = sRowCSV.Replace(")", "");
                    sRowCSV = sRowCSV.Replace("'", "");
                    //sCSV = sCSV + sRowCSV + "\n";

                    Debug.WriteLine("i=" + i.ToString() + "-" + "sRowCSV=" + dbFunction.AddBracketStartEnd(sRowCSV));

                    if (sRowCSV.Length > 0)
                    {
                        TempArrayDataCol.Add(sRowCSV);
                    }

                    ucStatusImport.iState = 0;
                    ucStatusImport.sMessage = "PROCESSING TERMINAL";
                    ucStatusImport.iMin = ii;
                    ucStatusImport.iMax = grdList.RowCount;
                    ucStatusImport.AnimateStatus();

                }
                
                // ----------------------------------------------------------
                // Limit line# of csv created depends on iRecordMinLimit
                // ----------------------------------------------------------
                clsArray.sRepoData = TempArrayDataCol.ToArray();

                if (iRecordMinLimit <= clsArray.sRepoData.Length)
                {
                    i = 0;
                    do
                    {
                        if (i % iRecordMinLimit == 0 && i > 0)
                        {
                            iStartIndex = i - iRecordMinLimit;
                            iEndIndex = i;
                            columnbind.Clear();
                            //Debug.WriteLine("iStartIndex=" + iStartIndex.ToString() + "," + "iEndIndex=" + iEndIndex.ToString());
                            for (x = iStartIndex; x < iEndIndex; x++)
                            {
                                //Debug.WriteLine("x=" + x.ToString() + "," + "Data=" + clsArray.sRepoData[x].ToString());

                                columnbind.Append(clsArray.sRepoData[x].ToString());

                                if (x < iEndIndex - 1)
                                    columnbind.Append("\r\n");
                            }

                            // Write To File
                            iFileNameIndex++;
                            string sNewFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iImportStock, iFileNameIndex);
                            Debug.WriteLine("sNewFileName=" + sNewFileName);

                            ucStatusImport.iState = 3;
                            ucStatusImport.sMessage = "CREATING CSV FILE :" + sNewFileName;
                            ucStatusImport.iMin = 0;
                            ucStatusImport.iMax = 0;
                            ucStatusImport.AnimateStatus();

                            dbFile.DeleteCSV(sNewFileName);
                            dbFile.WriteCSV(sNewFileName, columnbind.ToString());
                        }

                        i++;
                    } while (i < clsArray.sRepoData.Length);
                }
                else
                {
                    iEndIndex = 0;
                }

                // Get Last Part
                if (clsArray.sRepoData.Length > iEndIndex)
                {
                    int iTempStartIndex = iEndIndex;
                    int iTempEndIndex = clsArray.sRepoData.Length;
                    columnbind.Clear();
                    //Debug.WriteLine("iTempStartIndex=" + iTempStartIndex.ToString() + "," + "iTempEndIndex=" + iTempEndIndex.ToString());
                    for (x = iTempStartIndex; x < iTempEndIndex; x++)
                    {
                        //Debug.WriteLine("x=" + x.ToString() + "," + "Data=" + clsArray.sRepoData[x].ToString());

                        columnbind.Append(clsArray.sRepoData[x].ToString());

                        if (x < iTempEndIndex - 1)
                            columnbind.Append("\r\n");
                    }

                    // Write To File
                    iFileNameIndex++;
                    string sNewFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iImportStock, iFileNameIndex);
                    Debug.WriteLine("sNewFileName=" + sNewFileName);

                    ucStatusImport.iState = 3;
                    ucStatusImport.sMessage = "CREATING CSV FILE :" + sNewFileName;
                    ucStatusImport.iMin = 0;
                    ucStatusImport.iMax = 0;
                    ucStatusImport.AnimateStatus();

                    dbFile.DeleteCSV(sNewFileName);
                    dbFile.WriteCSV(sNewFileName, columnbind.ToString());
                }

                // Import
                for (i = 1; i <= iFileNameIndex; i++)
                {
                    string sImportFileName = dbFunction.GetImportFileName(clsFunction.ImportType.iImportStock, i);

                    ucStatusImport.iState = 3;
                    ucStatusImport.sMessage = "UPLOADING CSV FILE :" + sImportFileName;
                    ucStatusImport.iMin = 0;
                    ucStatusImport.iMax = 0;
                    ucStatusImport.AnimateStatus();

                    // Upload File to FTP                                
                    Debug.WriteLine("=>>UploadFile=" + sImportFileName);
                    ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                    ftpClient.delete(clsGlobalVariables.strFTPUploadPath + sImportFileName);
                    ftpClient.upload(clsGlobalVariables.strFTPUploadPath + sImportFileName, @clsGlobalVariables.strFTPLocalPath + sImportFileName);
                    ftpClient.disconnect(); // ftp disconnect

                    Debug.WriteLine("=>>API Call ImportStockDetail=" + sImportFileName);
                    dbAPI.ExecuteAPI("POST", "Import", "CSV", sImportFileName, "Import Stock Detail", "", "ImportStockDetail"); // Process CSV File

                }

                dbFunction.GetResponseTime("Import Stock Detail");

                Cursor.Current = Cursors.Default; // Back to normal             
            }
        }

        private void cboIClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassClientID = 0;
            if (!cboIClient.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Client List", cboIClient.Text);
                clsSearch.ClassClientID = clsSearch.ClassOutFileID;

            }
            txtIClientID.Text = clsSearch.ClassClientID.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            fEdit = false;
            dbFunction.ClearTextBox(this);
            InitButton();
            btnSave.Enabled = true;
            btnAdd.Enabled = false;
            
            dbFunction.TextBoxUnLock(true, this);
            dbFunction.ComBoBoxUnLock(true, this);
            
            txtItemID.Text = dbAPI.GetControlID("Stock Detail").ToString();

            txtSerialNo.ReadOnly = false;

            btnSearchSN.Enabled = false;
            btnClearTerminalSN.Enabled = false;

            txtSerialNo.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {   
            string sSQL = "";

            // Init field
            clsSearch.ClassItemID = int.Parse(dbFunction.CheckAndSetNumericValue(txtItemID.Text));

            // ClientID
            dbFunction.GetIDFromFile("Client List", cboClient.Text);
            clsSearch.ClassClientID = clsSearch.ClassOutFileID;

            // LocationID
            dbFunction.GetIDFromFile("Location", cboLocation.Text);
            clsSearch.ClassLocationID = clsSearch.ClassOutFileID;

            // TypeID
            dbFunction.GetIDFromFile("Terminal Type", cboType.Text);
            clsSearch.ClassTerminalTypeID = clsSearch.ClassOutFileID;

            // ModelID
            dbFunction.GetIDFromFile("Terminal Model", cboModel.Text);
            clsSearch.ClassTerminalModelID = clsSearch.ClassOutFileID;

            // BrandID
            dbFunction.GetIDFromFile("Terminal Brand", cboBrand.Text);
            clsSearch.ClassTerminalBrandID = clsSearch.ClassOutFileID;

            // StatusID
            dbFunction.GetIDFromFile("Status List", cboStatus.Text);
            clsSearch.ClassTerminalStatus = clsSearch.ClassOutFileID;

            // check fields
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalSN, txtSerialNo.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iAssetType, cboAssetType.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalType, cboType.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalModel, cboModel.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalBrand, cboBrand.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iTerminalStatus, cboStatus.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iLocation, cboLocation.Text)) return;

            if (!dbFunction.fSavingConfirm(fEdit)) return;

            Cursor.Current = Cursors.WaitCursor;

            if (!fEdit)
            {
                var data = new
                {
                    StockID = clsDefines.gZero,
                    ClientID = dbFunction.CheckAndSetStringValue(clsSearch.ClassClientID.ToString()),
                    LocationID = dbFunction.CheckAndSetStringValue(clsSearch.ClassLocationID.ToString()),
                    TerminalTypeID = dbFunction.CheckAndSetStringValue(clsSearch.ClassTerminalTypeID.ToString()),
                    TerminalModelID = dbFunction.CheckAndSetStringValue(clsSearch.ClassTerminalModelID.ToString()),
                    TerminalBrandID = dbFunction.CheckAndSetStringValue(clsSearch.ClassTerminalBrandID.ToString()),
                    SerialNo = dbFunction.CheckAndSetStringValue(txtSerialNo.Text),
                    StockStatus = dbFunction.CheckAndSetStringValue(clsSearch.ClassTerminalStatus.ToString()),
                    DeliveryDate = dbFunction.CheckAndSetDatePickerValueToDate(dteMDeliveryDate),
                    ReceiveDate = dbFunction.CheckAndSetDatePickerValueToDate(dteMReceiveDate),
                    ReleaseDate = dbFunction.CheckAndSetDatePickerValueToDate(dteMReleaseDate),
                    Allocation = dbFunction.CheckAndSetStringValue(cboAllocation.Text),
                    AssetType = dbFunction.CheckAndSetStringValue(cboAssetType.Text),
                    PONo = dbFunction.CheckAndSetStringValue(txtPONo.Text),
                    InvNo = dbFunction.CheckAndSetStringValue(txtInvoiceNo.Text),
                    PartNo = dbFunction.CheckAndSetStringValue(txtPartNo.Text),
                    Remarks = dbFunction.CheckAndSetStringValue(txtRemarks.Text)

                };

                sSQL = IFormat.Insert(data);                
                Debug.WriteLine("sSQL" + sSQL);
                dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 1);

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "Stock Detail", sSQL, "InsertCollectionDetail");
            }
            else
            {
                sSQL = dbFunction.CheckAndSetStringValue(clsSearch.ClassItemID.ToString()) + clsDefines.gPipe +
                       dbFunction.CheckAndSetStringValue(clsSearch.ClassClientID.ToString()) + clsDefines.gPipe +
                       dbFunction.CheckAndSetStringValue(clsSearch.ClassLocationID.ToString()) + clsDefines.gPipe +
                       dbFunction.CheckAndSetStringValue(clsSearch.ClassTerminalTypeID.ToString()) + clsDefines.gPipe +
                       dbFunction.CheckAndSetStringValue(clsSearch.ClassTerminalModelID.ToString()) + clsDefines.gPipe +
                       dbFunction.CheckAndSetStringValue(clsSearch.ClassTerminalBrandID.ToString()) + clsDefines.gPipe +
                       dbFunction.CheckAndSetStringValue(txtSerialNo.Text) + clsDefines.gPipe +
                       dbFunction.CheckAndSetStringValue(clsSearch.ClassTerminalStatus.ToString()) + clsDefines.gPipe +
                       dbFunction.CheckAndSetDatePickerValueToDate(dteMDeliveryDate) + clsDefines.gPipe +
                       dbFunction.CheckAndSetDatePickerValueToDate(dteMReceiveDate) + clsDefines.gPipe +
                       dbFunction.CheckAndSetDatePickerValueToDate(dteMReleaseDate) + clsDefines.gPipe +
                       dbFunction.CheckAndSetStringValue(cboAllocation.Text) + clsDefines.gPipe +
                       dbFunction.CheckAndSetStringValue(cboAssetType.Text) + clsDefines.gPipe +
                       dbFunction.CheckAndSetStringValue(txtPONo.Text) + clsDefines.gPipe +
                       dbFunction.CheckAndSetStringValue(txtInvoiceNo.Text) + clsDefines.gPipe +
                       dbFunction.CheckAndSetStringValue(txtPartNo.Text) + clsDefines.gPipe +
                       dbFunction.CheckAndSetStringValue(txtRemarks.Text);
                
                Debug.WriteLine("sSQL" + sSQL);
                dbFunction.parseDelimitedString(sSQL, clsDefines.gPipe, 1);
                
                dbAPI.ExecuteAPI("PUT", "Update", "Stock Detail", sSQL, "", "", "UpdateCollectionDetail");

            }

            Cursor.Current = Cursors.Default;

            if (!fEdit)
                dbFunction.SetMessageBox("New stock detail has been successfully saved.", "Saved", clsFunction.IconType.iInformation);
            else
                dbFunction.SetMessageBox("Stock detail has been successfully updated.", "Updated", clsFunction.IconType.iWarning);

            btnClear_Click(this, e);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            fEdit = false;
            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);

            cboClient.Text = cboAllocation.Text = cboLocation.Text = cboAssetType.Text = cboType.Text = cboModel.Text = cboBrand.Text = cboStatus.Text = clsFunction.sDefaultSelect;
            
            InitButton();

            btnSearchSN.Enabled = true;
            btnClearTerminalSN.Enabled = true;

        }

        private void InitButton()
        {
            if (fEdit)
            {
                btnAdd.Enabled = false;
                btnSave.Enabled = true;
            }
            else
            {
                btnAdd.Enabled = true;
                btnSave.Enabled = false;
            }
        }

        private void cboClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassClientID = 0;
            if (!cboClient.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Client List", cboClient.Text);
                clsSearch.ClassClientID = clsSearch.ClassOutFileID;

            }
            
        }

        private void cboLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassLocationID = 0;
            if (!cboLocation.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Location", cboLocation.Text);
                clsSearch.ClassLocationID = clsSearch.ClassOutFileID;

            }            
        }

        private void cboBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassTerminalBrandID = 0;
            if (!cboBrand.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Terminal Brand", cboBrand.Text);
                clsSearch.ClassTerminalBrandID = clsSearch.ClassOutFileID;
            }
        }

        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsSearch.ClassTerminalStatus = 0;
            if (!cboStatus.Text.Equals(clsFunction.sDefaultSelect))
            {
                dbFunction.GetIDFromFile("Status List", cboStatus.Text);
                clsSearch.ClassTerminalStatus = clsSearch.ClassOutFileID;
            }
        }

        private void btnSearchSN_Click(object sender, EventArgs e)
        {
            if (fAutoLoadData)
            {
                frmSearchField.fSelected = true;
            }
            else
            {
                frmSearchField.iSearchType = frmSearchField.SearchType.iStockDetail;
                frmSearchField.iStatus = clsFunction.iZero;
                frmSearchField.sTerminalType = "View Stock Detail";
                frmSearchField.sHeader = "COMPONENTS";
                frmSearchField.isCheckBoxes = false;
                frmSearchField.iLocationID = clsFunction.iZero;
                frmSearchField.sLocation = clsFunction.sDefaultSelect;
                frmSearchField frm = new frmSearchField();
                frm.ShowDialog();

            }

            if (frmSearchField.fSelected)
            {
                Cursor.Current = Cursors.WaitCursor;
                
                dbFunction.ComBoBoxUnLock(true, this);
                dbFunction.TextBoxUnLock(true, this);

                fEdit = true;
                InitButton();

                txtItemID.Text = clsSearch.ClassTerminalID.ToString();
                txtSerialNo.Text = clsSearch.ClassTerminalSN;

                PopulateStockDetailTextBox();

                Cursor.Current = Cursors.Default;
            }
        }

        private void btnClearTerminalSN_Click(object sender, EventArgs e)
        {
            btnClear_Click(this, e);
        }

        private void PopulateStockDetailTextBox()
        {
            if (dbFunction.isValidID(txtItemID.Text))
            {
                string pJSONString = dbAPI.getInfoDetailJSON("Search", "Stock Detail Info", txtItemID.Text);
                dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                cboClient.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ClientName);
                cboAllocation.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Allocation);
                cboLocation.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Location);
                cboAssetType.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_AssetType);
                cboType.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalType);
                cboModel.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalModel);
                cboBrand.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalBrand);
                cboStatus.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_StockStatusDescription);

                string pTemp = "";
                pTemp = dbFunction.GetDateFromParse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_DeliveryDate), clsFunction.sValueDateFormat, clsFunction.sDateDefaultFormat);
                dteMDeliveryDate.Value = DateTime.Parse(dbFunction.CheckAndSetDate(pTemp));

                pTemp = dbFunction.GetDateFromParse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReceiveDate), clsFunction.sValueDateFormat, clsFunction.sDateDefaultFormat);
                dteMReceiveDate.Value = DateTime.Parse(dbFunction.CheckAndSetDate(pTemp));

                pTemp = dbFunction.GetDateFromParse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReleaseDate), clsFunction.sValueDateFormat, clsFunction.sDateDefaultFormat);
                dteMReleaseDate.Value =  DateTime.Parse(dbFunction.CheckAndSetDate(pTemp));

                txtInvoiceNo.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_InvNo);
                txtPONo.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_PONo);
                txtPartNo.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_PartNo);
                txtRemarks.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Remarks);

            }
        }
    }
}
