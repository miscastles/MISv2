using System;
using System.Linq;
using System.Windows.Forms;
using static MIS.Function.AppUtilities;
using static MIS.clsDefines;
using System.Data;
using System.Collections.Generic;
using MIS.Enums;

namespace MIS
{
    public partial class frmServiceOveride : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;

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

        //Parameters
        private int MerchantID { get; set; }
        private int ClientID { get; set; }
        private int IRIDNo { get; set; }
        private int FieldEngineerID { get; set; }
        private int DispatcherID { get; set; }
        private int TerminalID { get; set; }
        private int SimID { get; set; }
        private int repTerminalID { get; set; }
        private int repSimID { get; set; }
        private int ReasonID { get; set; }
        private int ProblemReportedID { get; set; }
        private int AssistNo { get; set; }
        private int SourceID { get; set; }
        private int CategoryID { get; set; }
        private int SubCategoryID { get; set; }
        private int DependencyID { get; set; }
        private int StatusReasonID { get; set; }
        private int iFlag { get; set; }
        private int ProblemID { get; set; }
        private bool fEdit { get; set; } = false;
        private string CurrentStatus { get; set; }
        private string AppVersion { get; set; }
        private string AppCrc { get; set; }
        private int ServiceNo { get; set; }
        private int FSRNo { get; set; }
        private int RegionID { get; set; }
        private int RegionType { get; set; }
        private int JobTypeID { get; set; }

        public frmServiceOveride()
        {
            InitializeComponent();
        }

        private void ComboBoxDefaultSelect()
        {
            this.Controls.OfType<ComboBox>().ToList().ForEach(t => t.SelectedIndex = 0);
        }

        private void GenerateReferenceNo()
        {
            bool isAutoGen = true;

            if (!dbFunction.fPromptConfirmation("Are you sure to let the system generate REFERENCE NO.?")) return;

            //if (!dbFunction.isValidID(txtSearchFSRNo.Text)) txtSearchServiceNo.Text = clsFunction.sZero;
            if (fEdit) isAutoGen = false;

            dbAPI.GenerateID(isAutoGen, txtEntryRequestID, txtSearchServiceNo, "Servicing Detail", clsDefines.CONTROLID_PREFIX_REFNO);
        }

        private void ClearAllFields()
        {
            ClearAllTextBoxes(this);
            ClearAllComboBoxes(this);
            ClearAllParameters();
            DisabledCtrl();

            txtServiceType1.Text = string.Empty;
            lblSubHeader.Text = string.Empty;
            lblMainStatus.Text = "-";
            dteCreatedDate.Value = DateTime.Now;
            dteReqInstallationDate.Value = DateTime.Now;
            btnSearchMerchant.Enabled = true;
        }

        private void ClearMerchantFields()
        {
            ClearTextBoxes(new[]
            {
                txtSearchMerchantName,
                txtMerchantName,
                txtMerchantAddress,
                txtMerchantRegion,
                txtMerchantCity,
                txtMerchantContactPerson,
                txtMerchantMobileNo,
                txtMerchantEmail,
                txtIRTID,
                txtIRMID,
                txtMerchantPrimaryNum,
                txtMerchantSecondaryNum,
                txtAppVersion,
                txtAppCRC,
                txtFUAppVersion,
                txtFUAppCRC,
                txtClientRequestor,
                txtVendor
            });
        }

        private void ClearAllParameters()
        {
            MerchantID = 0;
            ClientID = 0;
            IRIDNo = 0;
            ProblemID = 0;
            FieldEngineerID = 0;
            DispatcherID = 0;
            AssistNo = 0;
            TerminalID = 0;
            SimID = 0;
            iFlag = 0;
            CurrentStatus = String.Empty;
            repSimID = 0;
            repTerminalID = 0;
            ServiceNo = 0;
            JobTypeID = 0;
        }

        private void DisabledCtrl()
        {
            Control[] Ctrls = {
                btnSave, cboSearchServiceType, cboSearchServiceStatus,
                btnSearchMerchant, cboSource, cboCategory,
                cboSubCategory, btnSearchFE, btnRemoveFE, btnSearchDispatcher,
                btnRemoveDispatcher,
                btnNoReferenceNo, btnNoRequestID,
                dteCreatedDate, dteReqInstallationDate
            };

            foreach (var control in Ctrls)
            {
                control.Enabled = false;
            }
        }

        // Reusable Cleaners
        private void ClearTextBoxes(TextBox[] Tbx)
        {
            foreach (TextBox txt in Tbx)
            {
                txt.Clear();
            }
        }

        private void ClearComboBoxes(ComboBox[] Cmb)
        {
            foreach (ComboBox combo in Cmb)
            {
                combo.Items.Clear();
            }
        }

        private void ClearAllTextBoxes(Control obj)
        {
            foreach (Control Ctrl in obj.Controls)
            {
                if (Ctrl is TextBox textBox)
                {
                    textBox.Clear();
                }
                else if (Ctrl.HasChildren)
                {
                    ClearAllTextBoxes(Ctrl);
                }
            }
        }

        private void ClearAllComboBoxes(Control obj)
        {
            foreach (Control ctrl in obj.Controls)
            {
                if (ctrl is ComboBox cmb)
                {
                    // Clear the ComboBox
                    cmb.DataSource = null;
                    cmb.Items.Clear();
                    cmb.Text = string.Empty;
                    cmb.DisplayMember = string.Empty;
                    cmb.ValueMember = string.Empty;
                }
                else if (ctrl.HasChildren)
                {
                    // Recursively clear ComboBoxes in child controls
                    ClearAllComboBoxes(ctrl);
                }
            }
        }

        private void BindComboBox(DataTable dataTable, ComboBox comboBox)
        {
            DebugTable(dataTable);

            DataTable comboBoxData = new DataTable();

            comboBoxData.Columns.Add("ID", typeof(int));
            comboBoxData.Columns.Add("Description", typeof(string));
            comboBoxData.Rows.Add(-1, "[NOT SPECIFIED]");

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    comboBoxData.Rows.Add(row["ID"], row["Description"]);
                }
            }

            comboBox.DisplayMember = "Description";
            comboBox.ValueMember = "ID";
            comboBox.DataSource = comboBoxData;
            comboBox.SelectedIndex = 0;
        }

        private void SearchEntity(frmSearchField.SearchType searchType, string header, Action onSuccess)
        {
            dbFunction = new clsFunction();

            frmSearchField.iSearchType = searchType;
            frmSearchField.sHeader = header;
            frmSearchField.isCheckBoxes = false;

            using (frmSearchField frm = new frmSearchField())
            {
                frm.ShowDialog();
            }

            if (frmSearchField.fSelected)
            {
                onSuccess?.Invoke();
            }
        }

        private void LoadMerchantDetails(int MerchantID, int IRIDNo)
        {
            Prompt.Debug("LoadMerchantDetails", "Fetching Data");

            if (!isValid(MerchantID)) return;

            dbAPI.ExecuteAPI("GET", "Search", "Merchant Info", $"{MerchantID}|{IRIDNo}", "Get Info Detail", "", "GetInfoDetail");

            if (dbAPI.isNoRecordFound()) return;

            dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, gPipe, 0);

            ClearMerchantFields();
            FillMerchantFields(clsSearch.ClassOutParamValue);
        }

        private void LoadClientDetails(int ClientID)
        {
            Prompt.Debug("LoadClientDetails", "Fetching Data");

            ClearTextBoxes(new[]
            {
                txtClientName,
                txtClientAddress,
                txtClientContactPerson,
                txtClientMobileNo,
                txtClientEmail
            });

            if (!isValid(ClientID)) return;

            dbAPI.ExecuteAPI("GET", "Search", "Client Info", ClientID.ToString(), "Get Info Detail", "", "GetInfoDetail");

            if (dbAPI.isNoRecordFound()) return;

            dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

            txtClientName.Text          = clsSearch.ClassClientName = GetPipeValue(clsSearch.ClassOutParamValue, 1);
            txtClientAddress.Text       = GetPipeValue(clsSearch.ClassOutParamValue, 2);
            txtClientContactPerson.Text = GetPipeValue(clsSearch.ClassOutParamValue, 6);
            txtClientMobileNo.Text      = GetPipeValue(clsSearch.ClassOutParamValue, 7);
            txtClientEmail.Text         = GetPipeValue(clsSearch.ClassOutParamValue, 8);

        }

        private void LoadDispatcher(int VendorTeamLeadID)
        {
            Prompt.Debug("LoadVendorTeamLead", "Fetching Data");

            ClearTextBoxes(new[]
            {
                txtDispatcherMobileNo,
                txtDispatcherEmail
            });

            if (!isValid(VendorTeamLeadID)) return;

            dbAPI.ExecuteAPI("GET", "Search", "FE Info", VendorTeamLeadID.ToString(), "Get Info Detail", "", "GetInfoDetail");

            if (dbAPI.isNoRecordFound()) return;

            txtDispatcher.Text          = GetPipeValue(clsSearch.ClassOutParamValue, 1);
            txtDispatcherMobileNo.Text  = GetPipeValue(clsSearch.ClassOutParamValue, 8);
            txtDispatcherEmail.Text     = GetPipeValue(clsSearch.ClassOutParamValue, 9);
        }

        private void LoadMultiMerchant(int IRIDNo)
        {
            Prompt.Debug("LoadMultiMerchant", "Fetching Data");

            dbAPI.loadMultiMerchantInfo(lvwMM, IRIDNo);
        }

        private void LoadFieldEngineer(int Id)
        {
            Prompt.Debug("LoadVendorHelpDesk", "Fetching Data");

            ClearTextBoxes(new[]
            {
                txtFEName,
                txtFEMobileNo,
                txtFEEmail
            });

            if (!isValid(Id)) return;

            dbAPI.ExecuteAPI("GET", "Search", "FE Info", Id.ToString(), "Get Info Detail", "", "GetInfoDetail");

            if (dbAPI.isNoRecordFound()) return;

            txtFEName.Text      = GetPipeValue(clsSearch.ClassOutParamValue, 1);
            txtFEMobileNo.Text  = GetPipeValue(clsSearch.ClassOutParamValue, 8);
            txtFEEmail.Text     = GetPipeValue(clsSearch.ClassOutParamValue, 9);
        }

        private void LoadReplacementTerminal(int Id)
        {
            Prompt.Debug("Execute: LoadReplacementTerminal", "Fetching Data");

            ClearTextBoxes(new[]
            {
                txtRepTerminalSN,
                txtRepTerminalCode,
                txtRepTerminalType,
                txtRepTerminalModel,
                txtRepTerminalBrand,
                txtRepTerminalLocation,
                txtRepTerminalAssetType
            });

            if (!isValid(Id)) return;

            dbAPI.ExecuteAPI("GET", "Search", "Terminal SN Info", Id.ToString(), "Get Info Detail", "", "GetInfoDetail");

            if (dbAPI.isNoRecordFound()) return;

            //dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);
            //txtRepTerminalID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
            //txtRepTerminalStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
            txtRepTerminalSN.Text = GetPipeValue(clsSearch.ClassOutParamValue, 6);
            txtRepTerminalCode.Text = GetPipeValue(clsSearch.ClassOutParamValue, 7);
            txtRepTerminalType.Text = GetPipeValue(clsSearch.ClassOutParamValue, 8);
            txtRepTerminalModel.Text = GetPipeValue(clsSearch.ClassOutParamValue, 9);
            txtRepTerminalBrand.Text = GetPipeValue(clsSearch.ClassOutParamValue, 10);
            txtRepTerminalLocation.Text = GetPipeValue(clsSearch.ClassOutParamValue, 13);
            txtRepTerminalAssetType.Text = GetPipeValue(clsSearch.ClassOutParamValue, 15);
            //txtRepTerminalLocationID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 31);
            //txtRepTerminalStatusDesc.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 30);
            clsSearch.ClassIsReleased = int.Parse(dbFunction.CheckAndSetNumericValue(GetPipeValue(clsSearch.ClassOutParamValue, 26)));        
        }

        private void LoadReplacementSim(int Id)
        {
            Prompt.Debug("Execute: LoadReplacementSim", "Fetching Data");

            ClearTextBoxes(new[]
            {
                txtRepSIMSN,
                txtRepSIMCarrier,
                txtRepSIMLocation
            });

            if (!isValid(Id)) return;

            dbAPI.ExecuteAPI("GET", "Search", "SIM SN Info", Id.ToString(), "Get Info Detail", "", "GetInfoDetail");

            if (dbAPI.isNoRecordFound()) return;

            //dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);
            //txtRepSIMID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
            //txtRepSIMStatus.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
            txtRepSIMSN.Text        = GetPipeValue(clsSearch.ClassOutParamValue, 3);
            txtRepSIMCarrier.Text   = GetPipeValue(clsSearch.ClassOutParamValue, 4);
            txtRepSIMLocation.Text  = GetPipeValue(clsSearch.ClassOutParamValue, 7);
            //txtRepSIMLocationID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 19);
            //txtRepSIMStatusDesc.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 18);

            clsSearch.ClassIsReleased = int.Parse(dbFunction.CheckAndSetNumericValue(GetPipeValue(clsSearch.ClassOutParamValue, 16)));
  
        }

        private void FillServiceStatus()
        {
            string[] actions = { clsFunction.sDefaultSelect,
                                 SERVICE_STATUS_PENDING,
                                 SERVICE_STATUS_COMPLETED,
                                 SERVICE_STATUS_PROCESSING,
                                 SERVICE_STATUS_PREPARATION
            };

            cboSearchServiceStatus.Items.Clear();
            cboSearchServiceStatus.Items.AddRange(actions);

            if (cboSearchServiceStatus.Items.Count > 0) cboSearchServiceStatus.SelectedIndex = 0;
        }

        private void getCurrentDevice(int iridNo, bool isTerminal)
        {
            string SearchBy = isTerminal ? "Merchant Current Terminal SN" : "Merchant Current SIM SN";
            string deviceID = isTerminal ? "TerminalID" : "SimID";

            TextBox deviceTextBox = isTerminal ? txtCurTerminalSN : txtCurSIMSN;

            dbAPI.ExecuteAPI("GET", "Search", SearchBy, iridNo.ToString(), "Get Info Detail", "", "GetInfoDetail");

            if (!dbAPI.isNoRecordFound())
            {
                int id = int.Parse(GetPipeValue(clsSearch.ClassOutParamValue, 1));
                var serialNumber = GetPipeValue(clsSearch.ClassOutParamValue, 2);

                if (isTerminal)
                {
                    TerminalID = id;
                    deviceTextBox.Text = serialNumber;
                }
                else
                {
                    SimID = id;
                    deviceTextBox.Text = serialNumber;
                }
            }
        }

        private void LoadDeviceDetails(int iridNo)
        {
            Prompt.Debug("LoadTerminalAndSimDetails", "Fetching Data");

            if (!isValid(iridNo)) return;

            getCurrentDevice(iridNo, true); // Terminal
            getCurrentDevice(iridNo, false); // Sim

            LoadDeviceInfo(TerminalID, SimID);
        }

        private void LoadDeviceInfo(int TerminalNo, int SimNo)
        {
            if (!TerminalNo.Equals(0))
            {
                getDeviceDetails(TerminalNo, true);
                getDeviceDetails(SimNo, false);
            }
            else
            {
                Prompt.Error("Device Information", "The current selected Merchant has no Active Terminal");
                ClearAllFields();
                return;
            }
        }

        private void getDeviceDetails(int id, bool isTerminal)
        {
            if (!isValid(id)) return;

            if (isTerminal)
            {
                lblCurTerminalDetail.Text = HEADER_CURRENT_TERMINAL;

                //Clear
                ClearTextBoxes(new[]
                {
                    txtCurTerminalSN,
                    txtCurTerminalCode,
                    txtCurTerminalType,
                    txtCurTerminalModel,
                    txtCurTerminalBrand,
                    txtCurTerminalLocation,
                    txtCurTerminalAssetType
                });
            }
            else
            {
                lblCurSIMDetail.Text = HEADER_CURRENT_SIM;

                //Clear
                ClearTextBoxes(new[]
                {
                    txtCurSIMSN,
                    txtCurSIMCarrier,
                    txtCurSIMLocation
                });
            }

            string SearchBy = isTerminal ? "Terminal SN Info" : "SIM SN Info";

            dbAPI.ExecuteAPI("GET", "Search", SearchBy, id.ToString(), "Get Info Detail", "", "GetInfoDetail");

            if (dbAPI.isNoRecordFound()) return;

            // Extract values and populate fields
            string[] values = clsSearch.ClassOutParamValue.Split(clsFunction.cPipe);

            int[] valueIndexes = isTerminal ? new[] { 6, 7, 8, 9, 10, 13, 15 } : new[] { 3, 4, 7 }; // Map Values

            FillDeviceFields(isTerminal, values, valueIndexes);

            clsSearch.ClassIsReleased = int.Parse(dbFunction.CheckAndSetNumericValue(getDelimitedValue(values, isTerminal ? 26 : 16)));
        }

        private void FillDeviceFields(bool isTerminal, string[] values, int[] valueIndexes)
        {
            TextBox[] textBoxes = isTerminal ? new[]
            {
                txtCurTerminalSN,
                txtCurTerminalCode,
                txtCurTerminalType,
                txtCurTerminalModel,
                txtCurTerminalBrand,
                txtCurTerminalLocation,
                txtCurTerminalAssetType
            }
            : new[]
            {
                txtCurSIMSN,
                txtCurSIMCarrier,
                txtCurSIMLocation
            };

            for (int i = 0; i < valueIndexes.Length; i++)
            {
                textBoxes[i].Text = getDelimitedValue(values, valueIndexes[i]);
            }
        }

        private string getDelimitedValue(string[] values, int index)
        {
            return values.Length > index ? values[index] : string.Empty;
        }

        public static string GetPipeValue(string input, int position)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            string[] parts = input.Split('|');

            if (position < 0 || position >= parts.Length)
                return string.Empty; // Out of range

            return parts[position];
        }

        private void FillMerchantFields(string Details)
        {
            // Split Data
            string[] values = Details.Split(clsFunction.cPipe);

            // Mapping Text boxes
            Dictionary<TextBox, int> MapFields = new Dictionary<TextBox, int>
            {
                //{ txtMerchantID, 0 },
                { txtMerchantName, 1 },
                { txtSearchMerchantName, 1 },
                { txtMerchantAddress, 2 },
                //{ txtMerchantProvince, 3 },
                { txtMerchantRegion, 4 },
                { txtMerchantCity, 3 },
                { txtMerchantContactPerson, 6 },
                //{ txtCustomerName, 6 },
                //{ txtMerchantTelNo, 7 },
                { txtMerchantMobileNo, 8 },
                //{ txtCustomerContactNo, 8 },
                { txtMerchantEmail, 9 },
                //{ txtCustomerEmail, 9 },
                { txtIRTID, 10 },
                { txtIRMID, 11 },
                { txtMerchantPrimaryNum, 12 },
                { txtMerchantSecondaryNum, 13 },
                { txtAppVersion, 14 },
                { txtFUAppVersion, 14},
                { txtAppCRC, 15 },
                { txtFUAppCRC, 15 },
                //{ txtIRIDNo, 16 },
                //{ txtSearchIRNo, 17 },
                //{ txtIRRequestDate, 18 },
                //{ txtIRInstallationDate, 19 },
                { txtPOSType, 20 },
                //{ txtRMInstruction, 21 }
            };

            foreach (KeyValuePair<TextBox, int> mapping in MapFields)
            {
                mapping.Key.Text = values.Length > mapping.Value ? values[mapping.Value] : string.Empty;
            }

            //txtRemarks.Text = fEdit ? "" : GetDelimitedValue(values, 28);
            txtClientRequestor.Text = getDelimitedValue(values, fEdit ? 24 : 33);

            string rawdata_info = GetPipeValue(clsSearch.ClassOutParamValue, 30);
            txtVendor.Text = dbFunction.getJSONTagValue(rawdata_info, IR_VENDOR, ROOTKEY_RAWDATA_INFO, NESTED_OBJECT_VALUES);
        }

        private void LoadCategories()
        {
            Prompt.Debug("Execute: LoadCategories", "Fetching Data");

            dbAPI = new clsAPI();

            //ClearText();

            //dbAPI.FillComboBoxServiceType(cboSearchServiceType);
            dbAPI.FillComboBoxTypeByGroup(cboSource, (int)GroupType.SourceType);
            dbAPI.FillComboBoxTypeByGroup(cboCategory, (int)GroupType.CategoryType);
            dbAPI.FillComboBoxTypeByGroup(cboSubCategory, (int)GroupType.SubCategoryType);

            dbAPI.FillComboBoxDepedency(cboDependency);
            dbAPI.FillComboBoxStatusReason(cboStatusReason);

            cboDependency.SelectedIndex = 0;
            cboStatusReason.SelectedIndex = 0;

            BindComboBox(LoadJobType(""), cboSearchServiceType);
        }

        private void EnabledCtrl()
        {
            Control[] Ctrls = {
                btnSave, btnSearchRepTerminal,btnRemoveRepTerminal,
                btnSearchRepSIM, btnRemoveRepSIM
            };

            foreach (var control in Ctrls)
            {
                control.Enabled = true;
            }
        }

        private DataTable LoadJobType(string value)
        {
            dbAPI = new clsAPI();

            dbAPI.ExecuteAPI("GET", "View", "Service Reversal-JobType", value, "Advance Detail", "", "ViewAdvanceDetail");

            if (dbAPI.isNoRecordFound())
                return new DataTable();

            return ParseResponseData(clsArray.ID, clsArray.detail_info);
        }

        private void LoadReasonID(int ID)
        {
            if (ID.Equals(0))
            {
                //txtReasonDesc.Clear();
                return;
            }

            dbAPI.ExecuteAPI("GET", "Search", "Helpdesk-Reason", ID.ToString(), "Get Info Detail", "", "GetInfoDetail");

            ReasonID = int.Parse(JFunc.getValue(clsSearch.ClassOutParamValue, "ReasonID"));
            //txtReasonDesc.Text = JFunc.getValue(clsSearch.ClassOutParamValue, "Description");
        }

        private void LoadProblemID(int ID)
        {
            if (ID.Equals(0))
            {
                txtProbReported.Clear();
                return;
            }

            dbAPI.ExecuteAPI("GET", "Search", "Helpdesk-Reason", ID.ToString(), "Get Info Detail", "", "GetInfoDetail");

            ProblemReportedID = int.Parse(JFunc.getValue(clsSearch.ClassOutParamValue, "ReasonID"));
            txtProbReported.Text = JFunc.getValue(clsSearch.ClassOutParamValue, "Description");
        }

        private void getSourceCategoryID()
        {
            dbFunction.GetIDFromFile("All Type", cboSource.Text);
            SourceID = clsSearch.ClassOutFileID;

            dbFunction.GetIDFromFile("All Type", cboCategory.Text);
            CategoryID = clsSearch.ClassOutFileID;

            dbFunction.GetIDFromFile("All Type", cboSubCategory.Text);
            SubCategoryID = clsSearch.ClassOutFileID;

            DependencyID = dbFunction.getFileID(cboDependency, "All Type");
            StatusReasonID = dbFunction.getFileID(cboStatusReason, "All Type");
        }

        // Process
        private void SearchRequest()
        {
            dbAPI = new clsAPI();

            SearchEntity(frmSearchField.SearchType.iService, "SERVICING", () =>
            {
                if (!isValid(clsSearch.ClassServiceNo))
                {
                    Prompt.Error("Invalid Service", $"Invalid Service No:{clsSearch.ClassServiceNo} Please re-select the service again");
                    return;
                }

                Cursor.Current = Cursors.WaitCursor;

                ClearAllFields();

                MerchantID  = clsSearch.ClassMerchantID;
                IRIDNo      = clsSearch.ClassIRIDNo;
                ClientID    = clsSearch.ClassClientID;
                ServiceNo   = clsSearch.ClassServiceNo;
                FSRNo       = clsSearch.ClassFSRNo;

                LoadRequestDetails(clsSearch.ClassServiceNo, clsSearch.ClassIRIDNo);
                LoadMerchantDetails(MerchantID, IRIDNo);
                LoadClientDetails(ClientID);
                LoadDeviceDetails(IRIDNo);
                LoadMultiMerchant(IRIDNo);

                ComboBoxDefaultSelect();

                //DisabledCtrl();
                //txtRequestNo.Text           = clsSearch.ClassRequestNo;
                txtSearchServiceNo.Text     = clsSearch.ClassServiceNo.ToString();
                txtSearchFSRNo.Text         = clsSearch.ClassFSRNo.ToString();
                lblMainStatus.Text          = "OVERIDE";

                btnSearchMerchant.Enabled   = false;
                btnNoReferenceNo.Enabled    = false;
                dteCreatedDate.Enabled      = false;
                btnNoRequestID.Enabled      = false;
                btnSearchMerchant.Enabled   = false;
                cboSearchServiceType.Enabled = true;
                btnSave.Enabled = true;
            });
        }

        private void LoadRequestDetails(int pServiceNo, int pIRIDNo)
        {
            dbAPI = new clsAPI();

            //Hold Current ProblemID
            ServiceNo = pServiceNo;

            dbAPI.ExecuteAPI("GET", "Search", "Merchant Servicing Info", $"{pServiceNo}|{pIRIDNo}", "Get Info Detail", "", "GetInfoDetail");

            if (dbAPI.isNoRecordFound()) return;

            FillServiceDetails(clsSearch.ClassOutParamValue);
            LoadFieldEngineer(FieldEngineerID);
            LoadDispatcher(DispatcherID);
            //LoadProblemID(ProblemReportedID);
            EnabledCtrl();

            //Copy Original Status
            CurrentStatus = cboSearchServiceStatus.Text;

            txtFUAppCRC.Text = AppCrc;
            txtFUAppVersion.Text = AppVersion;
        }

        private void FillServiceDetails(string json)
        {
            ServiceNo               = int.Parse(JFunc.getValue(json, "ServiceNo"));
            MerchantID              = int.Parse(JFunc.getValue(json, "MerchantID"));
            IRIDNo                  = int.Parse(JFunc.getValue(json, "IRIDNo"));
            ClientID                = int.Parse(JFunc.getValue(json, "ClientID"));
            FieldEngineerID         = int.Parse(JFunc.getValue(json, "FEID"));
            DispatcherID            = int.Parse(JFunc.getValue(json, "DispatchID"));
            TerminalID              = int.Parse(JFunc.getValue(json, "TerminalID"));
            SimID                   = int.Parse(JFunc.getValue(json, "SIMID"));
            JobTypeID               = int.Parse(JFunc.getValue(json, "JobType"));
            FSRNo                   = int.Parse(JFunc.getValue(json, "FSRNo"));

            txtEntryRequestID.Text      = JFunc.getValue(json, "IRNo");
            txtEntryReferenceNo.Text    = JFunc.getValue(json, "ReferenceNo");
            txtRemarks.Text             = JFunc.getValue(json, "Remarks");
            txtActionTaken.Text         = JFunc.getValue(json, "ActionTaken");
            txtRequestNo.Text           = JFunc.getValue(json, "RequestNo");
            txtRequestID1.Text          = JFunc.getValue(json, "RequestNo");
            dteCreatedDate.Text         = JFunc.getValue(json, "CreatedDate");
            dteReqInstallationDate.Text = JFunc.getValue(json, "RequestDate");
            txtCreatedBy.Text           = JFunc.getValue(json, "ProcessedBy");
            txtCreatedAt.Text           = JFunc.getValue(json, "CreatedDate");
            txtUpdatedBy.Text           = JFunc.getValue(json, "ModifiedBy");
            txtUpdatedAt.Text           = JFunc.getValue(json, "ModifiedDateTime");
            txtActualProblem.Text       = JFunc.getValue(json, "ActualProblemReported");
            AppVersion                  = JFunc.getValue(json, "AppVersion");
            AppCrc                      = JFunc.getValue(json, "AppCRC");
            txtProbReported.Text        = JFunc.getValue(json, "ProblemReported");
            txtServiceType1.Text        = JFunc.getValue(json, "ServiceJobTypeDescription");
            txtRMInstruction.Text       = JFunc.getValue(json, "RMInstruction");
            txtSearchFSRDate.Text       = JFunc.getValue(json, "ServiceCreatedDate");

            // New Fields Here ...

            // get Categories
            FillServiceStatus();
            LoadCategories();

            // Select Categories

            cboSearchServiceStatus.SelectedItem = JFunc.getValue(json, "JobTypeStatusDescription");
            cboSource.SelectedItem              = JFunc.getValue(json, "Source");
            cboCategory.SelectedItem            = JFunc.getValue(json, "Category");
            cboSubCategory.SelectedItem         = JFunc.getValue(json, "SubCategory");
            cboDependency.SelectedItem          = JFunc.getValue(json, "Dependency");
            cboStatusReason.SelectedItem        = JFunc.getValue(json, "StatusReason");

            //cboSearchServiceType.SelectedValue  = JFunc.getValue(json, "JobTypeDescription");
            //txtProbReported.Text    = JFunc.getValue(json, TAG_HD_ProblemReported);
            //txtCustomerName.Text = JFunc.getValue(json, TAG_HD_ContactPerson);
            //txtCustomerContactNo.Text = JFunc.getValue(json, TAG_HD_ContactNo);
            //txtCustomerPosition.Text = JFunc.getValue(json, TAG_HD_ContactPosition);
            //txtCustomerEmail.Text = JFunc.getValue(json, TAG_HD_ContactEmail);
            //txtOptTerminalSN.Text = JFunc.getValue(json, TAG_HD_TerminalSN);
            //txtOptSimSN.Text = JFunc.getValue(json, TAG_HD_SIMSN);
            //txtOptDockerSN.Text = JFunc.getValue(json, TAG_HD_DockerSN);
            //ReasonID = int.Parse(JFunc.getValue(json, TAG_HD_ReasonID));
            //ProblemReportedID       = int.Parse(JFunc.getValue(json, "ServiceNo"));
        }

        private void UpdateJobOrder(int ServiceNo)
        {
            if (IsFilled() && !ServiceNo.Equals(0))
            {
                getSourceCategoryID();

                var Data = new
                {
                    // Details
                    JobType         = cboSearchServiceType.SelectedValue,
                    RepTerminalID   = repTerminalID,
                    RepSimID        = repSimID
   
                    // New Details here

                };

                if (!Prompt.YesNo("Request Details", $"Are you sure you want to Update?" +
                                 $"\n\nMERCHANT: {txtMerchantName.Text}" +
                                 $"\nREQUEST ID: {txtEntryRequestID.Text}" +
                                 $"\n\n[Service Type]" +
                                 $"\n >FROM: {txtServiceType1.Text}"+
                                 $"\n >TO: {cboSearchServiceType.Text}")
                                 .Equals(DialogResult.Yes)) return;

                try
                {
                    dbAPI = new clsAPI();

                    dbAPI.ExecuteAPI("PUT", "Update", "Job-Order-Overide", $"{ServiceNo}|{JFunc.Serialized(Data)}", "", "", "UpdateCollectionDetail");

                    if (dbAPI.isNoRecordFound()) return;

                    Prompt.Info("Update Succesful", "Succesfully updated the following:" +
                                $"\n\nREQUEST ID: {txtRequestNo.Text}" +
                                $"\nMERCHANT: {txtMerchantName.Text}" +
                                $"\n\n[Service Type]" +
                                $"\n >FROM: {txtServiceType1.Text}" +
                                $"\n >TO: {cboSearchServiceType.Text}");

                    ClearAllFields();
                    btnSearchMerchant.Enabled = true;
                }

                catch (Exception ex)
                {
                    Prompt.Error("Updating Job Oder Details", $"Process Error: {ex.Message}");
                    return;
                }
            }
        }

        private bool IsFilled()
        {
            // Validate Required Fields
            var validations = new(bool condition, string message)[]
            {
                (cboSearchServiceType.SelectedIndex == 0, "Please select Service Type"),
                (cboSearchServiceStatus.SelectedIndex == 0, "Please select Service Status"),
                (MerchantID.Equals(0), "Please select a Merchant"),
                //(FieldEngineerID.Equals(0), "Please select a Field Engineer"),
                //(DispatcherID.Equals(0), "Please select a Dispatcher"),
                //(cboSource.SelectedIndex == 0, "Please select Source Type"),
                //(cboCategory.SelectedIndex == 0, "Please select Category Type"),
                //(cboSubCategory.SelectedIndex == 0, "Please select Sub Category Type"),
                //(txtRequestNo.Text.Length.Equals(0), "Request No is Required" ),
                //(txtEntryRequestID.Text.Length.Equals(0), "Request ID is Required" ),
                //(ProblemReportedID.Equals(0), "Problem Reported is Required" )
            };

            // Display Error
            foreach (var (condition, message) in validations)
            {
                if (condition)
                {
                    Prompt.Error("Service Request", message);
                    return false;
                }
            }

            return true;
        }

        private void SearchVendorHelpDesk()
        {
            dbAPI = new clsAPI();

            SearchEntity(frmSearchField.SearchType.iFE, "FIELD ENGINEER", () =>
            {
                FieldEngineerID = clsSearch.ClassParticularID;
                LoadFieldEngineer(FieldEngineerID);
            });
        }

        private void SearchTeamLead()
        {
            dbAPI = new clsAPI();

            SearchEntity(frmSearchField.SearchType.iFE, "DISPATCHER", () =>
            {
                DispatcherID = clsSearch.ClassParticularID;
                LoadDispatcher(DispatcherID);
            });
        }

        private void SearchReplacementTerminal()
        {
            dbAPI = new clsAPI();

            SearchEntity(frmSearchField.SearchType.iTerminal, "REPLACE TERMINAL", () =>
            {
                repTerminalID = clsSearch.ClassTerminalID;
                LoadReplacementTerminal(repTerminalID);
            });
        }

        private void SearchReplacementSim()
        {
            dbAPI = new clsAPI();

            SearchEntity(frmSearchField.SearchType.iSIM, "REPLACE SIM", () =>
            {
                repSimID = clsSearch.ClassSIMID;
                LoadReplacementSim(repSimID);
            });
        }

        private void btnSearchMerchant_Click(object sender, EventArgs e)
        {
            SearchRequest();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (JobTypeID.Equals(1))
            {
                Prompt.Info("Servicing Update", "Installation is not allowed to be selected.");
                return;
            }

            if (cboSearchServiceStatus.SelectedItem?.ToString() == "COMPLETED")
            {
                Prompt.Info("Servicing Update", "Completed Job Order are not allowed");
                return;
            }

            if (cboSearchServiceType.Text.Equals(clsGlobalVariables.STATUS_INSTALLATION_DESC))
            {
                Prompt.Info("Servicing Update", "Installation is not allowed to be selected.");
                return;
            }

            if (cboSearchServiceType.Text.Equals(clsGlobalVariables.STATUS_REPLACEMENT_DESC))
            {
                if (repTerminalID.Equals(0) && repSimID.Equals(0))
                {
                    Prompt.Info("Servicing Update", "Please select a Device to be used");
                    return;
                }
            }

            UpdateJobOrder(ServiceNo);
        }

        private void btnSearchFE_Click(object sender, EventArgs e)
        {

        }

        private void btnSearchRepTerminal_Click(object sender, EventArgs e)
        {
            SearchReplacementTerminal();
        }

        private void btnSearchRepSIM_Click(object sender, EventArgs e)
        {
            SearchReplacementSim();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        private void btnRemoveRepTerminal_Click(object sender, EventArgs e)
        {
            ClearTextBoxes(new[]
            {
                txtRepTerminalAssetType,
                txtRepTerminalBrand,
                txtRepTerminalCode,
                txtRepTerminalLocation,
                txtRepTerminalModel,
                txtRepTerminalSN,
                txtRepTerminalType
            });

            repTerminalID = 0;
        }

        private void btnRemoveRepSIM_Click(object sender, EventArgs e)
        {
            ClearTextBoxes(new[]
            {
                txtRepSIMCarrier,
                txtRepSIMLocation,
                txtRepSIMSN
            });

            repSimID = 0;
        }

        private void cboSearchServiceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSearchServiceType.Text.Equals(clsGlobalVariables.STATUS_REPLACEMENT_DESC))
            {
                pnRepTerminal.Enabled = true;
                pnRepSim.Enabled = true;
            }
            else
            {
                pnRepTerminal.Enabled = false;
                pnRepSim.Enabled = false;
            }
        }
    }
}
