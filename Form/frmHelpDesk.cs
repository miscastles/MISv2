using MIS.AppActivity;
using MIS.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.AppMainActivity;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using MIS.AppConnection;
using static MIS.Function.AppUtilities;
using Newtonsoft.Json;
using static MIS.frmImportIR;
using static MIS.clsDefines;
using MIS.Controller;

namespace MIS
{
    public partial class frmHelpDesk : Form
    {
        // Classes
        private clsAPI dbAPI;
        private clsFunction dbFunction;

        // Controller
        private HelpDeskController _mHelpDeskController;

        //Parameters
        private int MerchantID { get; set; }
        private int ClientID { get; set; }
        private int IRIDNo { get; set; }
        private int VendorHelpDeskID { get; set; }
        private int VendorTeamLeadID { get; set; }
        private int TerminalID { get; set; }
        private int SimID { get; set; }
        private int ReasonID { get; set; }
        private int ProblemReportedID { get; set; }
        private int AssistNo { get; set; }
        private int BillingTypeID { get; set; }
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

        public frmHelpDesk()
        {
            InitializeComponent();
            //InitStatusTitle();

            dbFunction = new clsFunction();
            dbFunction.setDoubleBuffer(lvwList, true);

            // Initialize the controller object            
            _mHelpDeskController = new HelpDeskController();
        }

        private void frmHelpDesk_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
        }


        // Fuctions
        private void ComboBoxDefaultSelect()
        {
            this.Controls.OfType<ComboBox>().ToList().ForEach(t => t.SelectedIndex = 0);
        }

        private void InitStatusTitle()
        {
            if (getFlag().Equals(0) || getFlag().Equals(2))
            {
                //lblMainStatus.ForeColor = Color.Yellow;
                lblMainStatus.Text = "NEW REQUEST";
                lblHeader.Text = "CREATE REQUEST " + cboSearchServiceType.Text;
            }
            else
            {
                //lblMainStatus.ForeColor = Color.Cyan;
                lblMainStatus.Text = "UPDATE REQUEST";
                lblHeader.Text = "UPDATE REQUEST " + cboSearchServiceType.Text;
            }

        }

        private void setFlag(int Flag)
        {
            iFlag = Flag;
        }

        private int getFlag()
        {
            return iFlag;
        }
        
        // Date Formatter
        private string FormatDate(string dateString)
        {
            if (DateTime.TryParse(dateString, out DateTime parsedDate))
            {
                return parsedDate.ToString("yyyy-MM-dd"); // Change format as needed (e.g., "MM/dd/yyyy")
            }
            return dateString; // Return original if parsing fails
        }

        public DateTime ParseDateTime(string dateStr)
        {
            if (DateTime.TryParse(dateStr, out DateTime dt) &&
                dateStr != "0000-00-00" && dateStr != "0000-00-00 00:00:00")
                return dt;

            return DateTime.Now;
        }


        // Search
        private void SearchRequest()
        {
            dbAPI = new clsAPI();

            try
            {
                SearchEntity(frmSearchField.SearchType.iHelpDesk, "HELPDESK", () =>
                {
                    if (!isValid(clsSearch.AssistNo))
                    {
                        Prompt.Error("Invalid Service", $"Invalid AssistNo:{clsSearch.AssistNo} Please re-select the service again");
                        return;
                    }

                    Cursor.Current = Cursors.WaitCursor;

                    ClearAllFields();
                    setFlag(1);

                    MerchantID = clsSearch.ClassMerchantID;
                    IRIDNo = clsSearch.ClassIRIDNo;
                    ClientID = clsSearch.ClassClientID;
                    AssistNo = clsSearch.AssistNo;

                    // set ID's
                    txtAssistNo.Text = AssistNo.ToString();                    
                    txtIRIDNo.Text = IRIDNo.ToString();
                    txtMerchantID.Text = MerchantID.ToString();

                    LoadRequestStatusDetails(clsSearch.AssistNo);
                    LoadMerchantDetails(MerchantID, IRIDNo);
                    LoadClientDetails(ClientID);
                    LoadDeviceDetails(IRIDNo);
                    LoadMultiMerchant(IRIDNo);
                    ComboBoxDefaultSelect();

                    DisabledCtrl();
                    txtRequestNo.Text = clsSearch.ClassRequestNo;
                    btnSearchService.Enabled = true;
                    btnAdd.Enabled = true;
                    lvwList.Enabled = true;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceptional error message {ex.Message}");
                dbFunction.SetMessageBox($"Exceptional error message {ex.Message}", "Search: HelpDesk", clsFunction.IconType.iError);
            }            
        }

        private void SearchMerchant()
        {
            dbAPI = new clsAPI();

            try
            {
                SearchEntity(frmSearchField.SearchType.iMerchant, "MERCHANT", () =>
                {
                    if (!isValid(clsSearch.ClassTID) || !isValid(clsSearch.ClassMID))
                    {
                        Prompt.Error("Field Checking", $"Invalid merchant information selected for job order {cboSearchServiceType.Text}." +
                                                 $"\n\nMerchant information\n Name: {clsSearch.ClassParticularName}" +
                                                 $"\n TID: {clsSearch.ClassTID}" +
                                                 $"\n MID: {clsSearch.ClassMID}\n\n" +
                                                 "Kindly check the selected record.");

                        return;
                    }


                    Cursor.Current = Cursors.WaitCursor;

                    // Load Data
                    MerchantID = clsSearch.ClassParticularID;
                    IRIDNo = clsSearch.ClassIRIDNo;
                    ClientID = clsSearch.ClassClientID;

                    txtMerchantID.Text = MerchantID.ToString();
                    txtIRIDNo.Text = IRIDNo.ToString();
                    txtClientID.Text = ClientID.ToString();
                    /* 
                    if (isMerchantHasPending(IRIDNo))
                    {
                        Prompt.Info("Merchant Validation", "The Selected Merchant has a Pending Record");
                        ClearAllFields();
                        return;
                    }
                    */

                    LoadMerchantDetails(MerchantID, IRIDNo);
                    LoadClientDetails(ClientID);
                    LoadMultiMerchant(IRIDNo);
                    ComboBoxDefaultSelect();
                    FillServiceStatus();

                    // Auto Load Current Helpdesk
                    VendorHelpDeskID = clsSearch.ClassCurrentParticularID;
                    LoadVendorHelpDesk(VendorHelpDeskID);

                    // IR Request ID
                    txtRequestID1.Text = clsSearch.ClassRequestID;
                    GenerateID("REF", txtRequestNo);

                    LoadDeviceDetails(IRIDNo);

                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceptional error message {ex.Message}");
                dbFunction.SetMessageBox($"Exceptional error message {ex.Message}", "New: HelpDesk", clsFunction.IconType.iError);
            }
            
        }

        private void SearchVendorHelpDesk()
        {
            dbAPI = new clsAPI();

            SearchEntity(frmSearchField.SearchType.iFE, "VENDOR HELPDESK", () =>
            {
                VendorHelpDeskID = clsSearch.ClassParticularID;
                LoadVendorHelpDesk(VendorHelpDeskID);
            });
        }

        private void SearchTeamLead()
        {
            dbAPI = new clsAPI();

            SearchEntity(frmSearchField.SearchType.iFE, "VENDOR TEAM LEADER", () =>
            {
                VendorTeamLeadID = clsSearch.ClassParticularID;
                LoadVendorTeamLead(VendorTeamLeadID);
            });
        }

        private void SearchReason()
        {
            bool isSuccess = cboSearchServiceStatus.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS);

            SearchEntity(
                isSuccess ? frmSearchField.SearchType.iReason : frmSearchField.SearchType.iNegativeReason,
                (isSuccess ? "SUCCESS" : "NEGATIVE") + " REASON", () =>
                {
                    ReasonID = clsSearch.ClassReasonID;
                    txtReasonDesc.Text = clsSearch.ClassReasonDescription;
                    //txtReasonDesc.BackColor = clsFunction.MKBackColor;
                }
            );
        }

        private void SearchProblem()
        {
            bool isSuccess = cboSearchServiceStatus.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS);

            SearchEntity(frmSearchField.SearchType.iProblem,
                (isSuccess ? "SUCCESS" : "NEGATIVE") + " PROBLEM", () =>
                    {
                        ProblemReportedID = clsSearch.ClassReasonID;

                        if (clsSearch.ClassReasonIsInput > 0)
                        {   
                            txtProbReported.BackColor = clsFunction.EntryBackColor;
                            txtProbReported.ReadOnly = false;
                            txtProbReported.Focus();
                        }
                        else
                        {
                            txtProbReported.BackColor = clsFunction.DisableBackColor;
                            txtProbReported.ReadOnly = true;
                        }

                        txtProbReported.Text = clsSearch.ClassReasonDescription;

                    }
                );
        }

        private void SearchEntity(frmSearchField.SearchType searchType, string header, Action onSuccess)
        {
            dbFunction = new clsFunction();

            frmSearchField.iSearchType  = searchType;
            frmSearchField.sHeader      = header;
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

        // Getters
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

        private void getSourceCategoryID()
        {
            dbFunction.GetIDFromFile("All Type", cboBillingType.Text);
            BillingTypeID = clsSearch.ClassOutFileID;

            dbFunction.GetIDFromFile("All Type", cboSource.Text);
            SourceID= clsSearch.ClassOutFileID;

            dbFunction.GetIDFromFile("All Type", cboCategory.Text);
            CategoryID = clsSearch.ClassOutFileID;

            dbFunction.GetIDFromFile("All Type", cboSubCategory.Text);
            SubCategoryID = clsSearch.ClassOutFileID;

            DependencyID   = dbFunction.getFileID(cboDependency, "All Type");
            StatusReasonID = dbFunction.getFileID(cboStatusReason, "All Type");
        }

        private void getCurrentDevice(int iridNo, bool isTerminal)
        {
            string SearchBy = isTerminal ? "Merchant Current Terminal SN" : "Merchant Current SIM SN";
            string deviceID = isTerminal ? "TerminalID" : "SimID";

            TextBox deviceTextBox = isTerminal ? txtCurTerminalSN : txtCurSIMSN;

            dbAPI.ExecuteAPI("GET", "Search", SearchBy, iridNo.ToString(), "Get Info Detail", "", "GetInfoDetail");

            if (!dbAPI.isNoRecordFound())
            {
                int id = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1));
                var serialNumber = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);

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

        private string getDelimitedValue(string[] values, int index)
        {
            return values.Length > index ? values[index] : string.Empty;
        }

        // Data Placements
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

            string rawdata_info = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 30);
            txtVendor.Text = dbFunction.getJSONTagValue(rawdata_info, clsDefines.IR_VENDOR, clsDefines.ROOTKEY_RAWDATA_INFO, clsDefines.NESTED_OBJECT_VALUES);
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

        private void FillServiceStatus()
        {
            string[] actions = { clsFunction.sDefaultSelect, TAG_HD_Pending, TAG_HD_Success, TAG_HD_Negative, };

            cboSearchServiceStatus.Items.Clear();
            cboSearchServiceStatus.Items.AddRange(actions);

            if (cboSearchServiceStatus.Items.Count > 0) cboSearchServiceStatus.SelectedIndex = 0;
        }
        
        private void FillRequestDetails(string json)
        {
            AssistNo                = int.Parse(JFunc.getValue(json, TAG_HD_AssistNo));
            txtEntryRequestID.Text  = JFunc.getValue(json, TAG_HD_RequestID);
            MerchantID              = int.Parse(JFunc.getValue(json, TAG_HD_MerchantID));
            IRIDNo                  = int.Parse(JFunc.getValue(json, TAG_HD_IRIDNo));
            ClientID                = int.Parse(JFunc.getValue(json, TAG_HD_ClientID));
            VendorHelpDeskID        = int.Parse(JFunc.getValue(json, TAG_HD_HelpdeskID));
            VendorTeamLeadID        = int.Parse(JFunc.getValue(json, TAG_HD_TeamLeadID));
            ReasonID                = int.Parse(JFunc.getValue(json, TAG_HD_ReasonID));
            ProblemReportedID       = int.Parse(JFunc.getValue(json, TAG_HD_ProblemID));
            //txtProbReported.Text    = JFunc.getValue(json, TAG_HD_ProblemReported);
            txtCustomerName.Text    = JFunc.getValue(json, TAG_HD_ContactPerson);
            txtCustomerContactNo.Text = JFunc.getValue(json, TAG_HD_ContactNo);
            txtCustomerPosition.Text  = JFunc.getValue(json, TAG_HD_ContactPosition);
            txtCustomerEmail.Text   = JFunc.getValue(json, TAG_HD_ContactEmail);
            txtOptTerminalSN.Text   = JFunc.getValue(json, TAG_HD_TerminalSN);
            txtOptSimSN.Text        = JFunc.getValue(json, TAG_HD_SIMSN);
            txtOptDockerSN.Text     = JFunc.getValue(json, TAG_HD_DockerSN);
            txtRemarks.Text         = JFunc.getValue(json, TAG_HD_RemarksHelpDesk);
            txtActionTaken.Text     = JFunc.getValue(json, TAG_HD_RemarksService);
            txtRequestNo.Text       = JFunc.getValue(json, TAG_HD_ReferenceNo);
            txtRequestID1.Text      = JFunc.getValue(json, TAG_HD_RequestID);
            dteCreatedDate.Text     = JFunc.getValue(json, TAG_HD_CreatedDate);
            dteReqInstallationDate.Text = JFunc.getValue(json, TAG_HD_RequestDate);
            txtCreatedBy.Text       = JFunc.getValue(json, TAG_HD_CreatedBy);
            txtCreatedAt.Text       = JFunc.getValue(json, TAG_HD_CreatedAt);
            txtUpdatedBy.Text       = JFunc.getValue(json, TAG_HD_UpdatedBy);
            txtUpdatedAt.Text       = JFunc.getValue(json, TAG_HD_UpdatedAt);

            dteTimeAssisted.Value = ParseDateTime(JFunc.getValue(json, TAG_HD_TimeAssisted));
            dteTimeStart.Value = ParseDateTime(JFunc.getValue(json, TAG_HD_TimeStart));
            dteTimeEnd.Value = ParseDateTime(JFunc.getValue(json, TAG_HD_TimeEnd));

            TerminalID = int.Parse(JFunc.getValue(json, TAG_HD_TerminalID));
            SimID = int.Parse(JFunc.getValue(json, TAG_HD_SIMID));

            txtRepresentative.Text = JFunc.getValue(json, TAG_HD_Representative);
            txtRequestBy.Text = JFunc.getValue(json, TAG_HD_RequestedBy);
            txtActualProblem.Text = JFunc.getValue(json, TAG_HD_ActualProblem);
            AppVersion = JFunc.getValue(json, TAG_HD_AppVersion);
            AppCrc = JFunc.getValue(json, TAG_HD_AppCrc);

            // New Fields Here ...

            // get Categories
            FillServiceStatus();
            LoadCategories();

            // Select Categories
            cboSearchServiceType.SelectedValue = JFunc.getValue(json, TAG_HD_JobType);
            cboSearchServiceStatus.SelectedItem = JFunc.getValue(json, TAG_HD_Status);
            cboSource.SelectedItem = JFunc.getValue(json, TAG_HD_Source);
            cboCategory.SelectedItem = JFunc.getValue(json, TAG_HD_Category);
            cboSubCategory.SelectedItem = JFunc.getValue(json, TAG_HD_SubCategory);
            
        }

        // Disabled/Enabled Controls
        private void EnabledCtrl()
        {
            Control[] Ctrls = {
                btnAdd, btnSearchService, btnSave, cboSearchServiceType,
                cboSearchServiceStatus, btnSearchMerchant, cboSource,
                cboCategory, cboSubCategory, btnSearchFE, btnRemoveFE,
                btnSearchDispatcher, btnRemoveDispatcher, btnSearchReason,
                btnRemoveReason, btnNoReferenceNo, btnNoRequestID,
                txtEntryRequestID, dteCreatedDate, dteReqInstallationDate,
                txtProbReported, txtActionTaken, txtAnyComments, txtRemarks,
                txtOptTerminalSN, txtOptSimSN, txtOptDockerSN, txtRequestNo,
                txtCustomerName, txtCustomerPosition, txtCustomerEmail, txtCustomerContactNo,
                dteTimeAssisted, dteTimeStart, dteTimeEnd, txtActualProblem, lvwList, txtRepresentative, txtRequestBy,
                txtFUAppVersion, txtFUAppCRC, btnProblemReported

            };

            foreach (var control in Ctrls)
            {
                control.Enabled = true;
            }
        }

        private void DisabledCtrl()
        {
            Control[] Ctrls = {
                btnAdd, btnSave, cboSearchServiceType, cboSearchServiceStatus,
                btnSearchMerchant, btnSearchService, cboSource, cboCategory,
                cboSubCategory, btnSearchFE, btnRemoveFE, btnSearchDispatcher,
                btnRemoveDispatcher, btnSearchReason, btnRemoveReason,
                btnNoReferenceNo, btnNoRequestID, txtEntryRequestID,
                dteCreatedDate, dteReqInstallationDate, txtProbReported,
                txtActionTaken, txtAnyComments, txtRemarks,
                dteTimeAssisted, dteTimeStart, dteTimeEnd, txtRequestNo,
                txtActualProblem, txtCustomerName, txtCustomerPosition, txtCustomerEmail, txtCustomerContactNo, txtRepresentative, txtRequestBy,
                txtOptTerminalSN, txtOptSimSN, txtOptDockerSN, lvwList, txtRequestBy, txtRepresentative
            };

            foreach (var control in Ctrls)
            {
                control.Enabled = false;
            }
        }

        private void DisabledVendorInfoButton()
        {
            btnSearchFE.Enabled = false;
            btnRemoveFE.Enabled = false;
            btnSearchDispatcher.Enabled = false;
            btnRemoveDispatcher.Enabled = false;
            dteCreatedDate.Enabled = false;
            btnCancel.Enabled = true;
        }

        private void DisabledRequestButtons()
        {
            btnNoReferenceNo.Enabled = false;
            txtRequestNo.Enabled = false;
            dteCreatedDate.Enabled = false;
            txtEntryRequestID.Enabled = true;
            btnNoRequestID.Enabled = true;
            btnSearchMerchant.Enabled = false;
            lvwList.Enabled = false;
            lvwList.SelectedItems.Clear();
            btnAdd.Enabled = false;
            dteTimeStart.Enabled = false;
            dteTimeEnd.Enabled = false;
            btnCancel.Enabled = false;
            //txtEntryRequestID.Clear();
        }
        
        // Cleaners
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
                //txtCustomerName,
                //txtCustomerPosition,
                txtFUAppVersion,
                txtFUAppCRC,
                //txtCustomerContactNo,
                //txtCustomerEmail,
                txtClientRequestor,
                txtVendor
            });
        }

        private void ClearAllFields()
        {
            ClearAllTextBoxes(this);
            ClearAllComboBoxes(this);
            ClearAllParameters();
            DisabledCtrl();

            txtServiceType1.Text = string.Empty;
            lblSubHeader.Text    = string.Empty;
            lblMainStatus.Text   = string.Empty;
            dteCreatedDate.Value         = DateTime.Now;
            dteReqInstallationDate.Value = DateTime.Now;
            dteTimeAssisted.Value        = DateTime.Now;
            lvwList.Clear();

            btnAdd.Enabled = true;
            btnSearchService.Enabled = true;
            btnCancel.Enabled = false;
        }

        public void ClearCurrentFields()
        {
            ClearAllTextBoxes(this);
            ClearAllComboBoxes(this);
            DisabledCtrl();

            txtServiceType1.Clear();
            dteCreatedDate.Value         = DateTime.Now;
            dteReqInstallationDate.Value = DateTime.Now;
            lblMainStatus.Text           = string.Empty;
        }

        private void ClearHeldeskEntry()
        {
            ClearTextBoxes(new[]
{
                txtProbReported,
                txtActionTaken,
                txtAnyComments,
                txtRemarks,
                txtCustomerName,
                txtCustomerPosition,
                txtCustomerContactNo,
                txtCustomerEmail,
            });

            dteCreatedDate.Value = DateTime.Now;
            dteTimeAssisted.Value = DateTime.Now;
            dteTimeStart.Value = DateTime.Now;
            dteTimeEnd.Value = DateTime.Now;
        }

        private void ClearAllParameters()
        {
            MerchantID = 0;
            ClientID = 0;
            IRIDNo = 0;
            ProblemID = 0;
            VendorHelpDeskID = 0;
            VendorTeamLeadID = 0;
            AssistNo = 0;
            TerminalID = 0;
            SimID = 0;
            iFlag = 0;
            CurrentStatus = String.Empty;
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


        // Generate Random Request ID
        private void GenerateID(string idType, TextBox txt)
        {
            //string message = idType == "REQ" ? "Are you sure to let the system generate REQUEST ID?" : "Are you sure to let the system generate REFERENCE ID?";
            string prefix = idType == "REQ" ? CONTROLID_PREFIX_HELPDESK : CONTROLID_PREFIX_REFNO;

            //if (!new clsFunction().fPromptConfirmation(message)) return;

            dbFunction = new clsFunction();

            if(idType == "REQ") dbAPI.ExecuteAPI("GET", "Search", "Helpdesk-Master-ID", "", "CheckControlID", "", "CheckControlID");
            else dbAPI.ExecuteAPI("GET", "Search", "Helpdesk-Master-ID", "", "CheckControlID", "", "CheckControlID");

            int controlNo = clsGlobalVariables.isAPIResponseOK ? clsCheckControlID.ClassControlID : 0;

            string generatedID = dbFunction.GenerateControlNo(controlNo, prefix, true);

            if (idType == "REQ") txt.Text = generatedID;
            else txt.Text = generatedID;
        }

        private bool IsFilled()
        {
            // Validate Required Fields
            var validations = new(bool condition, string message)[]
            {
                (cboSearchServiceStatus.SelectedIndex.Equals(0), "Please select Service Status"),
                (ClientID.Equals(0), "Please select a Client"),
                (MerchantID.Equals(0), "Please select a Merchant"),
                (IRIDNo.Equals(0), "Please select a Merchant"),
                (VendorHelpDeskID.Equals(0), "Please select a Helpdesk"),
                (VendorTeamLeadID.Equals(0), "Please select a Team Leader"),
                (cboSource.SelectedIndex.Equals(0), "Please select Source Type"),
                (cboCategory.SelectedIndex.Equals(0), "Please select Category Type"),
                (cboSubCategory.SelectedIndex.Equals(0), "Please select Sub Category Type"),
                (txtRequestNo.Text.Length.Equals(0), "Request No is Required" ),
                (txtEntryRequestID.Text.Length.Equals(0), "Request ID is Required" ),
                (txtRepresentative.Text.Length.Equals(0), "Representative is Required" ),
                (txtRequestBy.Text.Length.Equals(0), "Requestor is Required" ),
                (ProblemReportedID.Equals(0), "Problem Reported is Required" )
            };

            // Display Error
            foreach (var (condition, message) in validations)
            {
                if (condition)
                {
                    Prompt.Error("Help Desk Request", message);
                    return false;
                }
            }

            return true;
        }

        // Checker
        private int getAttempts(int AssistNo, string RequestID)
        {
            dbAPI = new clsAPI();

            dbAPI.ExecuteAPI("GET", "Search", "Helpdesk-Attempt", $"{AssistNo.ToString()}|{RequestID}", "Get Info Detail", "", "GetInfoDetail");

            if (dbAPI.isNoRecordFound()) return 1;

            int Attempts = int.Parse(JFunc.getValue(clsSearch.ClassOutParamValue, "Result")) + 1;
            return Attempts;

        }

        private bool isMerchantHasPending(int IRIDNo)
        {
            dbAPI = new clsAPI();

            dbAPI.ExecuteAPI("GET", "Search", "Helpdesk-Master-Checker", IRIDNo.ToString(), "Get Info Detail", "", "GetInfoDetail");

            int ID = int.Parse(JFunc.getValue(clsSearch.ClassOutParamValue, "Result"));

            if (ID.Equals(1))
               return true;
            else
               return false;
        }

        private bool isRequestIDUsed(string RequestID)
        {
            dbAPI = new clsAPI();

            dbAPI.ExecuteAPI("GET", "Search", "Helpdesk-Attempt", $"|{RequestID}|", "Get Info Detail", "", "GetInfoDetail");

            int ID = int.Parse(JFunc.getValue(clsSearch.ClassOutParamValue, "Result"));

            if (ID.Equals(1))
                return true;
            else
                return false;
        }

        private bool isRequestIdAllowed(string RequestID, string Status)
        {
            int count = 0;

            dbAPI = new clsAPI();

            dbAPI.ExecuteAPI("GET", "Search", "Helpdesk-Attempt", $"{AssistNo.ToString()}|{RequestID}|{Status}", "Get Info Detail", "", "GetInfoDetail");

            count = int.Parse(JFunc.getValue(clsSearch.ClassOutParamValue, "Result"));

            if (Status == TAG_HD_Pending)
            { 
                if (count >= clsSystemSetting.ClassSystemJobOrderLimit)
                {
                    Prompt.Warning("Request ID Validation", $"The Request ID being used is Already at Limit\nTotal Pending Attempts: {count}");
                    return false;
                }
            }

            if (Status == TAG_HD_Negative)
            {
                if (count > 0)
                {
                    Prompt.Warning("Request ID Validation", $"The Request ID is Already been Negative");
                    return false;
                }
            }

            return true;
        }

        private bool IsNewTicketValid()
        {
            string status = cboSearchServiceStatus.Text;
            string requestId = txtEntryRequestID.Text;
            string jobtype = txtJobType.Text;
            string iridno = txtIRIDNo.Text;
            
            if (!IsFilled()) return false;

            // Check Limit
            if (status == TAG_HD_Pending)
            {
                if (!isRequestIdAllowed(requestId, TAG_HD_Pending))
                    return false;
            }

            // Check Availability
            if (isRequestIDUsed(requestId))
            {
                Prompt.Info("Request ID Validation", $"The Request ID [{requestId}] is already been used");
                return false;
            }

            // Check Current Negative
            if (!isRequestIdAllowed(requestId, TAG_HD_Negative))
                return false;

            // Check Request ID
            if (dbAPI.isRecordExist("Search", "Service RequestID", $"{iridno}{gPipe}{jobtype}{gPipe}{requestId}"))
            {
                Prompt.Info("Request ID Validation", $"The Request ID [{requestId}] is already been used");
                return false;                
            }

            return true;
        }

        private bool IsUpdateValid()
        {
            string status = cboSearchServiceStatus.Text;
            string requestId = txtEntryRequestID.Text;

            if (!IsFilled()) return false;

            if (status != CurrentStatus)
            {
                // Check Limit
                if (status == TAG_HD_Pending)
                {
                    if (!isRequestIdAllowed(requestId, TAG_HD_Pending))
                        return false;
                }

                if (isRequestIDUsed(requestId))
                {
                    Prompt.Info("Request ID Validation", "The Request No. is already been used");
                    return false;
                }

                if (!isRequestIdAllowed(requestId, TAG_HD_Negative))
                    return false;
            }

            return true;
        }

        private bool isValidToCancel()
        {
            string status = cboSearchServiceStatus.Text;
            string requestId = txtEntryRequestID.Text;

            if (isRequestIDUsed(requestId))
            {
                Prompt.Info("Request ID Validation", "The Request No. is already been used cannot be cancelled");
                return false;
            }

            if (!isRequestIdAllowed(requestId, TAG_HD_Negative))
                return false;

            return true;
        }
        
        // New | Update
        private void NewRequest()
        {
            if (!IsFilled()) return;
            
            if(!Prompt.YesNo("New Helpdesk Request", "Save New Request?").Equals(DialogResult.Yes)) return;

            //getSourceCategoryID();

            var Data = new
            {
                IRIDNo          = IRIDNo,
                Ref_No          = txtRequestNo.Text,
                //RequestID       = txtEntryRequestID.Text,
                RequestDate     = dteReqInstallationDate.Value.ToString("yyyy-MM-dd") + " " + dbFunction.getCurrentTime(),
                Requestor       = txtClientRequestor.Text,
                //SourceID        = SourceID,
                //CategoryID      = CategoryID,
                //SubCategoryID   = SubCategoryID,
                CreatedID       = clsSearch.ClassCurrentParticularID,
                CreatedAt       = dbFunction.getCurrentDateTime(),
                JobType         = cboSearchServiceType.SelectedValue,
                Status          = cboSearchServiceStatus.Text,
            };

            try
            {
                // Reset
                clsLastID.ClassLastInsertedID = 0;

                dbAPI = new clsAPI();

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "Helpdesk-Master", IFormat.Insert(Data), "InsertCollectionDetail");

                //if (dbAPI.isNoRecordFound()) return;
                if (clsLastID.ClassLastInsertedID > 0)
                {
                    txtAssistNo.Text = clsLastID.ClassLastInsertedID.ToString();

                    NewDetails(clsLastID.ClassLastInsertedID);
                }
                
            }
            catch (Exception ex)
            {
                Prompt.Info("New Request", $"Process Error: {ex.Message}");
            }
            
        }

        private void NewDetails(int AssistID)
        {
            if (AssistID.Equals(0)) return;

            getSourceCategoryID();

            var Data = new
            {
                AssistNo        = AssistID,
                RequestID       = txtEntryRequestID.Text,
                HelpDeskID      = VendorHelpDeskID,
                TeamLeadID      = VendorTeamLeadID,
                CreatedDate     = dteCreatedDate.Value.ToString("yyyy-MM-dd") + " " + dbFunction.getCurrentTime(),
                ReasonID        = ReasonID,
                ContactPerson   = txtCustomerName?.Text,
                ContactNo       = txtCustomerContactNo?.Text,
                ContactPosition = txtCustomerPosition?.Text,
                ContactEmail    = txtCustomerEmail?.Text,
                RequestedBy     = txtRequestBy?.Text,
                Representative  = txtRepresentative?.Text,
                TerminalID      = TerminalID,
                SIMID           = SimID,
                TerminalSN      = txtOptTerminalSN?.Text,
                SimSN           = txtOptSimSN?.Text,
                DockerSN        = txtOptDockerSN?.Text,
                ActualProblem   = txtActualProblem?.Text,
                ProblemReported = txtProbReported?.Text,
                RemarksHelpDesk = txtRemarks?.Text,
                RemarksService  = txtActionTaken?.Text,
                SourceID        = SourceID,
                CategoryID      = CategoryID,
                SubCategoryID   = SubCategoryID,
                CreatedID       = clsSearch.ClassCurrentParticularID,
                CreatedAt       = dbFunction.getCurrentDateTime(),

                CompletedAt     = cboSearchServiceStatus.Text != TAG_HD_Pending ? dbFunction.getCurrentDate() : "0000-00-00",
                Status          = cboSearchServiceStatus.Text,
                TimeAssisted    = dteTimeAssisted.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                TimeStart       = dteTimeStart.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                TimeEnd         = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),

                //CompletedAt     = cboSearchServiceStatus.Text != TAG_HD_Pending ? dbFunction.getCurrentDate() : "0000-00-00",
                //Status          = cboSearchServiceStatus.Text,
                //TimeAssisted    = cboSearchServiceStatus.Text != TAG_HD_Pending ? dteTimeAssisted.Value.ToString("yyyy-MM-dd HH:mm:ss") : "0000-00-00",
                //TimeStart       = cboSearchServiceStatus.Text != TAG_HD_Pending ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : "0000-00-00",
                //TimeEnd         = cboSearchServiceStatus.Text != TAG_HD_Pending ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : "0000-00-00",

                Attempts        = getAttempts(AssistID, txtEntryRequestID.Text),
                RequestDate     = dteReqInstallationDate.Value.ToString("yyyy-MM-dd"),

                AppVersion      = txtFUAppVersion.Text,
                AppCRC          = txtFUAppCRC.Text,
                ProblemReportedID       = ProblemReportedID,
                DependencyID = DependencyID,
                StatusReasonID = StatusReasonID
            };

            try
            {
                dbAPI = new clsAPI();

                dbAPI.ExecuteAPI("POST", "Insert", "", dteReqInstallationDate.Value.ToString("yyyy-MM-dd"), "Helpdesk-Details", IFormat.Insert(Data), "InsertCollectionDetail");

                //if (dbAPI.isNoRecordFound()) return;
                if (clsLastID.ClassLastInsertedID > 0)
                {
                    txtProblemNo.Text = clsLastID.ClassLastInsertedID.ToString();
                    txtAssistNo.Text = AssistID.ToString();
                    txtIRIDNo.Text = IRIDNo.ToString();

                    if (!dbAPI.isRecordExist("Search", "IRNo Check", dbFunction.CheckAndSetStringValue(txtEntryRequestID.Text)))
                        autoSaveServiceAndFSR();

                }

                //Prompt.Info("New Helpdesk Request", $"Request has been added");

                if (getFlag().Equals(0))
                    ClearAllFields();
                else
                    ClearCurrentFields();
                    btnSearchService.Enabled = true;
                    setFlag(0);

            }
            catch (Exception ex)
            {
                Prompt.Error("New Request Details", $"Process Error: {ex.Message}");
            }
        }

        private void UpdateDetails(int ProblemNo)
        {
            if (IsFilled() && !ProblemNo.Equals(0))
            {
                getSourceCategoryID();

                var Data = new
                {
                    // Details
                    ProblemID       = ProblemNo, 
                    //MerchantID    = MerchantID,
                    HelpDeskID      = clsSearch.ClassCurrentParticularID,
                    TeamLeadID      = VendorTeamLeadID,
                    //CreatedDate     = dteCreatedDate.Value.ToString("yyyy-MM-dd"),
                    ReasonID        = ReasonID,
                    ContactPerson   = txtCustomerName.Text,
                    ContactNo       = txtCustomerContactNo.Text,
                    ContactPosition = txtCustomerPosition.Text,
                    ContactEmail    = txtCustomerEmail.Text,
                    //TerminalID    = ,
                    //SIMID         = ,
                    TerminalSN      = txtOptTerminalSN.Text,
                    SimSN           = txtOptSimSN.Text,
                    DockerSN        = txtOptDockerSN.Text,
                    ProblemReported = txtProbReported.Text,
                    RemarksHelpDesk = txtRemarks.Text,
                    RemarksService  = txtActionTaken.Text,
                    Status          = cboSearchServiceStatus.Text,
                    //IRIDNo          = IRIDNo,
                    //ClientID        = ClientID,
                    CreatedAt       = dbFunction.getCurrentDateTime(),
                    //CreatedID       = clsSearch.ClassCurrentParticularID,
                    SourceID        = SourceID,
                    CategoryID      = CategoryID,
                    SubCategoryID   = SubCategoryID,
                    TimeAssisted    = dteTimeAssisted.Value.ToString("yyyy-MM-dd HH:mm:ss"),

                    /* 
                     *  Remove due to modification of Ticket
                     *  
                        TimeAssisted    = cboSearchServiceStatus.Text != "PENDING" ? dteTimeAssisted.Value.ToString("yyyy-MM-dd HH:mm:ss") : "0000-00-00",
                        TimeStart       = cboSearchServiceStatus.Text != "PENDING" ? TimeStartCaptured : "0000-00-00",
                        TimeEnd         = cboSearchServiceStatus.Text != "PENDING" ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : "0000-00-00",
                        CompletedAt     = dbFunction.getCurrentDate(),
                    */

                    RequestID       = txtEntryRequestID.Text,
                    RequestedBy     = txtRequestBy?.Text,
                    Representative  = txtRepresentative?.Text,
                    ActualProblem   = txtActualProblem?.Text,
                    RequestDate     = dteReqInstallationDate.Value.ToString("yyyy-MM-dd"),
                    CompletedAt     = dbFunction.getCurrentDate(),

                    AppVersion      = txtFUAppVersion.Text,
                    AppCRC          = txtFUAppCRC.Text,
                    ProblemReportedID = ProblemReportedID
                    // New Details here

                };

                if (!Prompt.YesNo("Request Details", $"Are you sure you want to Update?" +
                                 $"\n\nMERCHANT: {txtMerchantName.Text}" +
                                 $"\nREQUEST ID: {txtEntryRequestID.Text}")
                                 .Equals(DialogResult.Yes)) return;

                try
                {
                    dbAPI = new clsAPI();

                    dbAPI.ExecuteAPI("PUT", "Update", "Helpdesk-Details", IFormat.Update(Data), "", "", "UpdateCollectionDetail");

                    if (dbAPI.isNoRecordFound()) return;

                    Prompt.Info("Update Succesful","Succesfully Update" +
                                $"\n\nREQUEST ID: {txtRequestNo.Text}" +
                                $"\nMERCHANT: {txtMerchantName.Text}" +
                                $"\nPROBLEM ID: {ProblemNo}");

                    setFlag(0);
                    //LoadServiceStatusDetails(AssistNo);
                    ClearAllFields();
                    btnSearchService.Enabled = true;
                }

                catch (Exception ex)
                {
                    Prompt.Error("Updating Request Details", $"Process Error: {ex.Message}");
                }
            }
        }


        // Views
        private void LoadVendorHelpDesk(int VendorHelpDeskID)
        {
            Prompt.Debug("LoadVendorHelpDesk", "Fetching Data");

            ClearTextBoxes(new[]
            {
                txtFEName,
                txtFEMobileNo,
                txtFEEmail
            });

            if (!isValid(VendorHelpDeskID)) return;

            //dbAPI.ExecuteAPI("GET", "Search", "Helpdesk-Users", VendorHelpDeskID.ToString(), "Get Info Detail", "", "GetInfoDetail");

            dbAPI.ExecuteAPI("GET", "Search", "FE Info", VendorHelpDeskID.ToString(), "Get Info Detail", "", "GetInfoDetail");

            if (dbAPI.isNoRecordFound()) return;

            //txtFEName.Text      = JFunc.getValue(clsSearch.ClassOutParamValue, "Name");
            //txtFEMobileNo.Text  = JFunc.getValue(clsSearch.ClassOutParamValue, "Mobile");
            //txtFEEmail.Text     = JFunc.getValue(clsSearch.ClassOutParamValue, "Email");

            txtFEName.Text      = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
            txtFEMobileNo.Text  = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
            txtFEEmail.Text     = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
        }

        private void LoadVendorTeamLead(int VendorTeamLeadID)
        {
            Prompt.Debug("LoadVendorTeamLead", "Fetching Data");

            ClearTextBoxes(new[]
            {
                txtDispatcherMobileNo,
                txtDispatcherEmail
            });

            if (!isValid(VendorTeamLeadID)) return;

            //dbAPI.ExecuteAPI("GET", "Search", "Helpdesk-Users", VendorTeamLeadID.ToString(), "Get Info Detail", "", "GetInfoDetail");

            dbAPI.ExecuteAPI("GET", "Search", "FE Info", VendorTeamLeadID.ToString(), "Get Info Detail", "", "GetInfoDetail");

            if (dbAPI.isNoRecordFound()) return;

            //txtDispatcher.Text          = JFunc.getValue(clsSearch.ClassOutParamValue, "Name");
            //txtDispatcherMobileNo.Text  = JFunc.getValue(clsSearch.ClassOutParamValue, "Mobile");
            //txtDispatcherEmail.Text     = JFunc.getValue(clsSearch.ClassOutParamValue, "Email");

            txtDispatcher.Text            = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
            txtDispatcherMobileNo.Text    = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
            txtDispatcherEmail.Text       = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);

        }

        private void LoadMerchantDetails(int MerchantID, int IRIDNo)
        {
            Prompt.Debug("LoadMerchantDetails", "Fetching Data");

            if (!isValid(MerchantID)) return;

            dbAPI.ExecuteAPI("GET", "Search", "Merchant Info", $"{MerchantID}|{IRIDNo}", "Get Info Detail", "", "GetInfoDetail");

            if (dbAPI.isNoRecordFound()) return;

            dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

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
            
            txtClientName.Text          = clsSearch.ClassClientName = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
            txtClientAddress.Text       = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
            txtClientContactPerson.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
            txtClientMobileNo.Text      = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
            txtClientEmail.Text         = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);

        }

        private void LoadMultiMerchant(int IRIDNo)
        {
            Prompt.Debug("LoadMultiMerchant", "Fetching Data");

            dbAPI.loadMultiMerchantInfo(lvwMM, IRIDNo);
        }

        private void LoadRequestStatusDetails(int value)
        {
            dbAPI = new clsAPI();

            dbAPI.ExecuteAPI("GET", "View", "Helpdesk-Details", value.ToString(), "Advance Detail", "", "ViewAdvanceDetail");

            if (dbAPI.isNoRecordFound()) return;

            var data = ParseResponseData(clsArray.ID, clsArray.detail_info);

            string[] columnSequence = { TAG_HD_ID, TAG_ServiceJobTypeDescription, TAG_HD_Status, TAG_HD_RequestID, TAG_HD_CreatedAt, TAG_HD_RequestDate, TAG_HD_TimeAssistAt, TAG_HD_CreatedByName, TAG_ScheduleDate, TAG_FSRDate, TAG_ActionMade };
            //string[] hiddenColumns = { TAG_HD_ID };
            string[] hiddenColumns = { };

            SetListViewData(lvwList, data, columnSequence, hiddenColumns);

            // DEFINE HEADER NAME
            var headerRenameMap = new Dictionary<string, string>
            {
                { TAG_HD_Status, "STATUS" },
                { TAG_HD_CreatedAt, "DATE CREATED" },
                { TAG_HD_RequestID, "REQUEST ID" },               
                { TAG_HD_RequestDate, "DATE REQUEST" },
                { TAG_HD_TimeAssistAt, "TIME ASSISTED" },
                { TAG_HD_CreatedByName, "CREATED BY" },
                { TAG_ScheduleDate, "SCHEDULE DATE" },
                { TAG_FSRDate, "SERVICED DATE" },
                { TAG_ActionMade, "SERVICE RESULT" },
                { TAG_ServiceJobTypeDescription, "SERVICE TYPE" }
            };

            SetListViewColumnNames(lvwList, headerRenameMap);

            // DEFINE COLOR
            var colorMap = new Dictionary<string, Color>
                {
                    { TAG_HD_Resolved, Color.ForestGreen },
                    { TAG_HD_Pending, Color.Red },
                    { TAG_HD_Negative, Color.Orange }
                };

            // Apply to ListView
            SetListViewTextColor(lvwList, TAG_HD_Status, colorMap);
            SetListViewTextColor(lvwList, TAG_ActionMade, colorMap);

            // Apply auto width to ListView
            SetListViewAutoWidth(lvwList, hiddenColumns);

        }

        private void LoadReasonID(int ID)
        {
            if (ID.Equals(0))
            {
                txtReasonDesc.Clear();
                return;
            }

            dbAPI.ExecuteAPI("GET", "Search", "Helpdesk-Reason", ID.ToString(), "Get Info Detail", "", "GetInfoDetail");

            ReasonID            = int.Parse(JFunc.getValue(clsSearch.ClassOutParamValue, "ReasonID"));
            txtReasonDesc.Text  = JFunc.getValue(clsSearch.ClassOutParamValue, "Description");
        }

        private void LoadProblemID(int ID)
        {
            if (ID.Equals(0))
            {
                txtProbReported.Clear();
                return;
            }

            dbAPI.ExecuteAPI("GET", "Search", "Helpdesk-Problem", ID.ToString(), "Get Info Detail", "", "GetInfoDetail");

            ProblemReportedID = int.Parse(JFunc.getValue(clsSearch.ClassOutParamValue, "ReasonID"));
            txtProbReported.Text = JFunc.getValue(clsSearch.ClassOutParamValue, "Description");
        }

        private DataTable LoadJobType(string value)
        {
            dbAPI = new clsAPI();

            dbAPI.ExecuteAPI("GET", "View", "Helpdesk-JobType", value, "Advance Detail", "", "ViewAdvanceDetail");

            if (dbAPI.isNoRecordFound())
                return new DataTable();

            return ParseResponseData(clsArray.ID, clsArray.detail_info);
        }

        private void LoadCategories()
        {
            Prompt.Debug("LoadCategories", "Fetching Data");

            dbAPI = new clsAPI();

            //ClearText();

            //dbAPI.FillComboBoxServiceType(cboSearchServiceType);

            if (clsSearch.ClassIsBillType > 0)
            {
                cboBillingType.Enabled = true;
                dbAPI.FillComboBoxTypeByGroup(cboBillingType, (int)GroupType.BillingTypeID);
                
            }
            else
            {
                cboBillingType.Enabled = false;
                cboBillingType.Text = clsFunction.sDefaultSelect;                
            }

            dbAPI.FillComboBoxTypeByGroup(cboSource, (int)GroupType.SourceType);
            dbAPI.FillComboBoxTypeByGroup(cboCategory, (int)GroupType.CategoryType);
            dbAPI.FillComboBoxTypeByGroup(cboSubCategory, (int)GroupType.SubCategoryType);

            dbAPI.FillComboBoxDepedency(cboDependency);
            dbAPI.FillComboBoxStatusReason(cboStatusReason);

            cboDependency.SelectedIndex = 0;
            cboStatusReason.SelectedIndex = 0;

            BindComboBox(LoadJobType(""), cboSearchServiceType);
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
                Prompt.Error("Device Information","The current selected Merchant has no Active Terminal");
                ClearAllFields();
                return;
            }
        }

        private void LoadRequestDetails(int ProbID)
        {
            dbAPI = new clsAPI();

            //Hold Current ProblemID
            ProblemID = ProbID;

            dbAPI.ExecuteAPI("GET", "Search", "Helpdesk-Details", ProbID.ToString(), "Get Info Detail", "", "GetInfoDetail");

            if (dbAPI.isNoRecordFound()) return;

            string pJson = clsSearch.ClassOutParamValue;
            FillRequestDetails(pJson);
            LoadMerchantDetails(MerchantID, IRIDNo);
            //LoadDeviceDetails(IRIDNo);

            LoadDeviceInfo(TerminalID, SimID);
            LoadClientDetails(ClientID);
            LoadVendorHelpDesk(VendorHelpDeskID);
            LoadVendorTeamLead(VendorTeamLeadID);
            LoadReasonID(ReasonID);
            LoadProblemID(ProblemID);
            EnabledCtrl();

            btnNoReferenceNo.Enabled    = false;
            txtRequestNo.Enabled        = false;
            dteCreatedDate.Enabled      = false;
            btnAdd.Enabled              = true;
            btnNoRequestID.Enabled      = false;
            dteTimeStart.Enabled        = false;
            dteTimeEnd.Enabled          = false;
            btnSearchMerchant.Enabled   = false;
            btnSearchService.Enabled    = false;

            //Copy Original Status
            CurrentStatus = cboSearchServiceStatus.Text;
            txtFUAppCRC.Text = AppCrc;
            txtFUAppVersion.Text = AppVersion;

            /*if (isRequestIDUsed(txtEntryRequestID.Text))
            {
                // SUCCESS
                cboSearchServiceStatus.Enabled  = false;
                txtEntryRequestID.Enabled       = false;
            }
            */

            cboSearchServiceStatus.Enabled = true;

            // set ID's
            txtMerchantID.Text = JFunc.getValue(pJson, TAG_HD_MerchantID);
            txtIRIDNo.Text = JFunc.getValue(pJson, TAG_HD_IRIDNo);
            txtProblemNo.Text = JFunc.getValue(pJson, TAG_HD_ProblemNo);
            txtServiceNo.Text = JFunc.getValue(pJson, TAG_SERVICENO);
            txtFSRNo.Text = JFunc.getValue(pJson, TAG_FSRNO);

        }


        // Controls
        private void btnSearchFE_Click(object sender, EventArgs e)
        {
            SearchVendorHelpDesk();
        }

        private void btnSearchDispatcher_Click_1(object sender, EventArgs e)
        {
            SearchTeamLead();
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            dteTimeStart.Value = DateTime.Now;

            switch (getFlag())
            {
                case 1: //  New Details for New Ticket

                    if (!Prompt.YesNo("Request Ticket", "Create New Ticket?").Equals(DialogResult.Yes)) return;

                    setFlag(2);
                    EnabledCtrl();
                    InitStatusTitle();
                    FillServiceStatus();
                    //LoadCategories();
                    DisabledRequestButtons();
                    GenerateID("REF", txtRequestNo);
                    btnSearchService.Enabled = false;
                    
                    break;

                case 2: // 
                    //setFlag(2);
                    EnabledCtrl();
                    LoadCategories();
                    btnAdd.Enabled = false;
                    btnSave.Enabled = true;
                    btnCancel.Enabled = false;
                    InitStatusTitle();
                    
                    break;

                default: // New HD Master
                    ClearAllFields();
                    setFlag(0);
                    EnabledCtrl();
                    LoadCategories();
                    InitStatusTitle();
                    btnAdd.Enabled           = false;
                    btnSearchService.Enabled = false;
                    txtRequestNo.Enabled     = false;
                    btnNoReferenceNo.Enabled = false;
                    dteTimeStart.Enabled     = false;
                    dteTimeEnd.Enabled       = false;
                    dteCreatedDate.Enabled   = false;
                    btnCancel.Enabled        = false;
                    txtFUAppCRC.Enabled      = true;
                    txtFUAppVersion.Enabled  = true;
                    
                    break;
            }

            dbFunction.SetButtonIconImage(btnSearchService);

        }

        private void btnSearchMerchant_Click_1(object sender, EventArgs e)
        {
            if (!dbFunction.isValidDescriptionEntry(cboSearchServiceType.Text, "Service Type" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            Cursor.Current = Cursors.WaitCursor;

            SearchMerchant();

            Cursor.Current = Cursors.Default;
        }

        private void btnServiceSearch_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            SearchRequest();

            Cursor.Current = Cursors.Default;
        }

        private void btnClear_Click_1(object sender, EventArgs e)
        {
            ClearAllFields();
            btnAdd.Enabled = true;
            lvwList.Enabled = true;

            dbFunction.SetButtonIconImage(btnSearchService);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!dbAPI.isValidSystemVersion()) return;

            if (!ValidateFields()) return;

            if (string.IsNullOrEmpty(txtReasonDesc.Text)) {
                Prompt.Info("Reason Validation", "Please Select a Reason.");
                return;
            }

            Debug.WriteLine($"getFlag()=[{getFlag()}]");

            try
            {
                switch (getFlag())
                {
                    case 0: // Add New Merchant and Ticket
                        if (!IsNewTicketValid()) return;
                        NewRequest();

                        break;

                    case 1: // Update Request
                        if (!IsUpdateValid()) return;
                        UpdateDetails(ProblemID);
                        break;

                    case 2: // Add New Details From Existing
                        if (!IsNewTicketValid()) return;
                        if (!Prompt.YesNo("Helpdesk Ticket", $"Save New Ticket? \n" +
                                           $"\nSERVICE STATUS: {cboSearchServiceStatus.Text} " +
                                           $"\nREQUEST ID: {txtEntryRequestID.Text} " +
                                           $"\nMERCHANT: {txtMerchantName.Text}" +
                                           $"\nTID: {txtIRTID.Text}" +
                                           $"\nMID: {txtIRMID.Text}" +
                                           $"\n[ADDITIONAL TYPE]"+
                                           $"\nCSOURCE: {cboSource.Text}" +
                                           $"\nCATEGORY: {cboCategory.Text}"+
                                           $"\nSUB CATEGORY: {cboSubCategory.Text}")
                                           .Equals(DialogResult.Yes)) return;

                        if (!AssistNo.Equals(0))
                        {
                            NewDetails(AssistNo);
                            ClearAllFields();
                        }

                        break;

                    default:
                        return;
                }

                btnSearchService.Enabled = true;
                dbFunction.SetButtonIconImage(btnSearchService);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceptional error message {ex.Message}");
                dbFunction.SetMessageBox($"Exceptional error message {ex.Message}", "Save: HelpDesk", clsFunction.IconType.iError);
            }
            
        }

        private void cboSearchServiceType_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            txtServiceType1.Text = cboSearchServiceType.Text;
            lblSubHeader.Text = $"{cboSearchServiceType.Text} - {txtRequestID1.Text}";
            txtJobType.Text = $"{cboSearchServiceType.SelectedValue}";
        }

        private void btnNoRequestID_Click_1(object sender, EventArgs e)
        {
            GenerateID("REQ", txtEntryRequestID); // From: REQ - Change Due to Want to Use the ParentID
        }

        private void btnSearchReason_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidDescriptionEntry(cboSearchServiceStatus.Text, "Service Status" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return;

            SearchReason();
        }
        
        private void btnNoReferenceNo_Click_1(object sender, EventArgs e)
        {
            GenerateID("REF", txtRequestNo);        
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.SelectedItems.Count.Equals(0)) return;

            Cursor.Current = Cursors.WaitCursor;

            setFlag(1);
            InitStatusTitle();
            DisabledVendorInfoButton();
            LoadRequestDetails(int.Parse(lvwList.SelectedItems[0].SubItems[1].Text));
            dbFunction.SetButtonIconImage(btnSearchService);
            Cursor.Current = Cursors.Default;
        }
        
        private void btnRemoveFE_Click(object sender, EventArgs e)
        {
            ClearTextBoxes(new[]
            {
                txtFEName,
                txtFEMobileNo,
                txtFEEmail
            });

            VendorHelpDeskID = 0;
        }

        private void btnRemoveDispatcher_Click(object sender, EventArgs e)
        {
            ClearTextBoxes(new[]
            {
                txtDispatcher,
                txtDispatcherEmail,
                txtDispatcherMobileNo
            });

            VendorTeamLeadID = 0;
        }

        private void btnRemoveReason_Click(object sender, EventArgs e)
        {
            txtReasonDesc.Clear();
            ReasonID = 0;
        }
        
        private void txtReasonDesc_TextChanged(object sender, EventArgs e)
        {

        }

        private void frmHelpDesk_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

            //if (!isValidToCancel()) return;
            if (!dbFunction.isValidID(txtIRIDNo.Text) && !dbFunction.isValidID(txtAssistNo.Text) && !dbFunction.isValidID(txtProblemNo.Text))
            {
                dbFunction.SetMessageBox("Unable to cancel ticket.", "Cancel ticket", clsFunction.IconType.iExclamation);
                return;
            }

            if (dbFunction.isValidID(txtServiceNo.Text))
            {
                if (dbFunction.isValidID(txtFSRNo.Text))
                {  

                    if (!Prompt.YesNo("Helpdesk Ticket", $"This ticket has already been completed.\n\n" +
                                                         $"Do you still want to cancel this ticket?")
                                                           .Equals(DialogResult.Yes)) return;

                    if (!dbAPI.isPromptAdminLogIn()) return;
                }
                else
                {
                    if (!Prompt.YesNo("Helpdesk Ticket", $"This ticket is already scheduled for service.\n\n" +
                                                         $"Are you sure you want to cancel this ticket?")
                               .Equals(DialogResult.Yes)) return;
                }
            }

            if (!Prompt.YesNo("Helpdesk Ticket", $"Are you sure to cancel this ticket?\n" +
                               $"\nDATE CREATED: {dteCreatedDate.Text} " +
                               $"\nDATE REQUESTED: {dteReqInstallationDate.Text} " +
                               $"\nREFERENCE NO.: {txtRequestNo.Text} " +
                               $"\nREQUEST ID: {txtEntryRequestID.Text} " +                              
                               $"\n{clsFunction.sLineSeparator}" +
                               $"\nHELPDESK: {txtFEName.Text} " +
                               $"\nTEAM LEAD: {txtDispatcher.Text} " +
                               $"\n{clsFunction.sLineSeparator}" +
                               $"\nMERCHANT: {txtMerchantName.Text}" +
                               $"\nTID: {txtIRTID.Text}" +
                               $"\nMID: {txtIRMID.Text}" +
                               $"\n{clsFunction.sLineSeparator}" +
                               $"\nJOB TYPE: {cboSearchServiceType.Text}" +
                               $"\nTICKET STATUS: {cboSearchServiceStatus.Text} ")
                               .Equals(DialogResult.Yes)) return;

            // DELETE
            string pSearchValue = $"{txtIRIDNo.Text}{clsDefines.gPipe}" +
                                    $"{txtAssistNo.Text}{clsDefines.gPipe}" +
                                    $"{txtProblemNo.Text}{clsDefines.gPipe}" +
                                    $"{txtServiceNo.Text}{clsDefines.gPipe}" +
                                    $"{txtFSRNo.Text}";

            dbAPI.ExecuteAPI("DELETE", "Delete", "", pSearchValue, "Helpdesk-Ticket", "", "DeleteCollectionDetail");

            if (!dbAPI.isNoRecordFound())
            {
                Prompt.Info("Request Ticket", "Cancelled Successfuly");
                ClearAllFields();

                btnSearchService.Enabled = true;
                dbFunction.SetButtonIconImage(btnSearchService);
            }
        }

        private void btnProblemReported_Click(object sender, EventArgs e)
        {
            SearchProblem();
        }

        private void btnPreviewFSR_Click(object sender, EventArgs e)
        {
            _mHelpDeskController.HelpdeskServiceReport(ProblemID.ToString());
        }

        private void SaveServiceDetail()
        {
            Debug.WriteLine("--SaveServiceDetail--");

            string sSQL = "";

            int isBillable = (int)BillableType.NotBillable;
            bool isDispatch = DispatchType.NotDispatch > 0 ? true : false;

            // dteReqInstallationDate
            DateTime stReqInstallationDate = dteReqInstallationDate.Value;            
            string pReqInstallationDate = stReqInstallationDate.ToString("yyyy-MM-dd");
            string pReqInstallationTime = dbFunction.getCurrentTime(); ;

            // dteServiceReqDate
            DateTime stServiceReqDate = dteReqInstallationDate.Value;            
            string pServiceReqDate = stServiceReqDate.ToString("yyyy-MM-dd");
            string pServiceReqTime = dbFunction.getCurrentTime();
            
            dbFunction.GetProcessedByAndDateTime(); // Get processedby and datetime

            dbFunction.GetModifiedByAndDateTime(); // Get modifiedby and datetime
            
            ServiceTypeController _model = new ServiceTypeController();
            _model = _model.getServiceTypeInfo(cboSearchServiceType.Text);
            if (_model != null)
            {
                // Create Group Details - ROCKY BANTOLO
                var details = new
                {
                    TAIDNo = clsFunction.sZero,
                    ClientID = dbFunction.CheckAndSetNumericValue(ClientID.ToString()),
                    MerchantID = dbFunction.CheckAndSetNumericValue(MerchantID.ToString()),
                    FEID = dbFunction.CheckAndSetNumericValue(VendorHelpDeskID.ToString()),
                    ClientName = dbFunction.CheckAndSetStringValue(txtClientName.Text),
                    MerchantName = dbFunction.CheckAndSetStringValue(txtMerchantName.Text),
                    FEName = dbFunction.CheckAndSetStringValue(txtFEName.Text),
                    IRIDNo = dbFunction.CheckAndSetNumericValue(IRIDNo.ToString()),
                    SearchIRNo = dbFunction.CheckAndSetNumericValue(txtEntryRequestID.Text),
                    CurTerminalID = dbFunction.CheckAndSetNumericValue(TerminalID.ToString()),
                    CurTerminalSN = dbFunction.CheckAndSetStringValue(txtCurTerminalSN.Text),
                    CurSIMID = dbFunction.CheckAndSetNumericValue(SimID.ToString()),
                    CurSIMSN = dbFunction.CheckAndSetStringValue(txtCurSIMSN.Text),
                    CurDockID = clsFunction.sZero,
                    CurDockSN = clsFunction.sZero,
                    Counter = clsFunction.sZero,
                    RequestNo = dbFunction.CheckAndSetStringValue(txtRequestNo.Text),
                    ServiceDateTime = dbFunction.CheckAndSetStringValue(pReqInstallationDate + " " + pReqInstallationTime),
                    ServiceDate = dbFunction.CheckAndSetStringValue(pReqInstallationDate),
                    ServiceTime = dbFunction.CheckAndSetStringValue(pReqInstallationTime),
                    CustomerName = dbFunction.CheckAndSetStringValue(txtCustomerName.Text),
                    CustomerNo = dbFunction.CheckAndSetStringValue(txtCustomerContactNo.Text),
                    CustomerPosition = dbFunction.CheckAndSetStringValue(txtCustomerPosition.Text),
                    CustomerEmail = dbFunction.CheckAndSetStringValue(txtCustomerEmail.Text),
                    //Remarks = dbFunction.CheckAndSetStringValue(txtRemarks.Text),
                    Remarks = clsFunction.sDash,
                    ServiceReqDate = dbFunction.CheckAndSetStringValue(pServiceReqDate),
                    ServiceReqTime = dbFunction.CheckAndSetStringValue(pServiceReqTime),
                    LastServiceRequest = clsFunction.sZero,
                    NewServiceRequest = clsFunction.sDash,
                    RepTerminalID = clsFunction.sZero,
                    RepTerminalSN = clsFunction.sZero,
                    RepSIMID = clsFunction.sZero,
                    RepSIMSN = clsFunction.sZero,
                    RepDockID = clsFunction.sZero,
                    RepDockSN = clsFunction.sZero,
                    JobType = dbFunction.CheckAndSetStringValue(_model.JobType.ToString()),
                    JobTypeDescription = dbFunction.CheckAndSetStringValue(_model.JobTypeDescription),
                    JobTypeSubDescription = dbFunction.CheckAndSetStringValue(_model.Description),
                    Dispatch = clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC,
                    ReferenceNo = dbFunction.CheckAndSetStringValue(txtRequestNo.Text),
                    RegionID = dbFunction.CheckAndSetNumericValue(RegionID.ToString()),
                    RegionType = dbFunction.CheckAndSetNumericValue(RegionType.ToString()),
                    Billable = isBillable,
                    Primary = clsFunction.sZero,
                    AppVersion = dbFunction.CheckAndSetStringValue(txtFUAppVersion.Text),
                    AppCRC = dbFunction.CheckAndSetStringValue(txtFUAppCRC.Text),
                    DispatchID = dbFunction.CheckAndSetNumericValue(VendorTeamLeadID.ToString()),
                    DispatchBy = dbFunction.CheckAndSetStringValue(txtDispatcher.Text),
                    DispatchDateTime = clsFunction.sDash,
                    DispatchDate = clsFunction.sDash,
                    DispatchTime = clsFunction.sDash,
                    ProcessedBy = dbFunction.CheckAndSetStringValue(clsUser.ClassProcessedBy),
                    ModifiedBy = dbFunction.CheckAndSetStringValue(clsUser.ClassModifiedBy),
                    ProcessedDateTime = dbFunction.CheckAndSetStringValue(clsUser.ClassProcessedDateTime),
                    ModifiedDateTime = dbFunction.CheckAndSetStringValue(clsUser.ClassModifiedDateTime),
                    ReqInstallationDate = dbFunction.CheckAndSetStringValue(pReqInstallationDate),
                    ProblemReported = dbFunction.CheckAndSetStringValue(txtProbReported.Text),
                    RMInstruction = clsFunction.sDash,
                    SourceID = dbFunction.CheckAndSetNumericValue(SourceID.ToString()),
                    CategoryID = dbFunction.CheckAndSetNumericValue(CategoryID.ToString()),
                    SubCategoryID = dbFunction.CheckAndSetNumericValue(SubCategoryID.ToString()),
                    Vendor = dbFunction.CheckAndSetStringValue(txtVendor.Text),
                    Requestor = dbFunction.CheckAndSetStringValue(txtRequestBy.Text),
                    BillingTypeID = dbFunction.CheckAndSetStringValue(BillingTypeID.ToString())
                };

                sSQL = IFormat.Insert(details);

                Debug.WriteLine("SaveServiceDetail" + sSQL);

                dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 0);

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "Servicing Detail", sSQL, "InsertCollectionDetail");

                clsSearch.ClassLastInsertedID = clsLastID.ClassLastInsertedID;
                txtServiceNo.Text = clsSearch.ClassLastInsertedID.ToString();

                // Update AssistNo/ProblemNo
                if (dbFunction.isValidID(txtServiceNo.Text))
                {
                    dbAPI.ExecuteAPI("PUT", "Update", "Service Helpdesk", $"{txtServiceNo.Text}{clsDefines.gPipe}{txtAssistNo.Text}{clsDefines.gPipe}{txtProblemNo.Text}", "", "", "UpdateCollectionDetail");
                }
            }
        }

        private void SaveManulFSRMaster()
        {
            Debug.WriteLine("--SaveFSRMaster--");

            string sRowSQL = "";
            string sSQL = "";
            
            sRowSQL = "";
            sRowSQL = " ('" + dbFunction.getCurrentDateTime() + "', " +
            sRowSQL + sRowSQL + " '" + clsFunction.sDash + "', " +
            sRowSQL + sRowSQL + " '" + clsFunction.sDash + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtRemarks.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.getCurrentDateTime() + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassUserFullName + "', " +
            sRowSQL + sRowSQL + " '" + clsUser.ClassUserFullName + "') ";
            sSQL = sSQL + sRowSQL;

            Debug.WriteLine("SaveFSRMaster::" + "\n" + "sSQL=" + sSQL);

            dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 0);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "FSR Import Master", sSQL, "InsertCollectionMaster");

        }

        private void SaveManualFSRDetail()
        {
            Debug.WriteLine("--SaveManualFSRDetail--");

            string sSQL = "";
            int LineNo = 0;
            string sSerialNo = dbFunction.CheckAndSetNumericValue(txtCurSIMSN.Text) + clsFunction.sPipe +
                               dbFunction.CheckAndSetNumericValue(txtCurTerminalSN.Text) + clsFunction.sPipe +
                               dbFunction.CheckAndSetNumericValue(txtOptDockerSN.Text) + clsFunction.sPipe;

            DateTime stFSRDate = dteCreatedDate.Value;            
            string sMTimeStart = dbFunction.GetDateFromParse(dteTimeStart.Text, "hh:mm tt", "HH:mm:ss");
            string sMTimeEnd = DateTime.Now.ToString("HH:mm:ss");
            //string sMTimeEnd = dbFunction.GetDateFromParse(dteTimeEnd.Text, "hh:mm tt", "HH:mm:ss");            
            string sProcessType = clsGlobalVariables.PROCESS_TYPE_MANUAL_DESC;
            
            dbAPI.GenerateID(true, txtFSRRequestNo, txtFSRNo, "FSR Detail", clsDefines.CONTROLID_PREFIX_FSR);

            ServiceTypeController _model = new ServiceTypeController();
            _model = _model.getServiceTypeInfo(cboSearchServiceType.Text);
            if (_model != null)
            {
                // Create Group Details
                var details = new
                {
                    FSRID = clsLastID.ClassLastInsertedID.ToString(),
                    No = LineNo.ToString(),
                    MerchantID = dbFunction.CheckAndSetNumericValue(MerchantID.ToString()),
                    Merchant = (txtSearchMerchantName.Text.Length > 0 ? txtSearchMerchantName.Text : clsFunction.sDash),
                    MID = dbFunction.CheckAndSetStringValue(txtIRMID.Text),
                    TID = dbFunction.CheckAndSetStringValue(txtIRTID.Text),
                    MobileID = clsFunction.sZero,
                    MobileTerminalID = clsFunction.sDash,
                    MobileVersion = clsFunction.sZero,
                    TimeArrived = dteTimeAssisted.Value.ToString("HH:mm:ss"),
                    InvoiceNo = dteTimeAssisted.Value.ToString("HH:mm:ss"),
                    TimeStart = sMTimeStart,
                    BatchNo = sMTimeStart,
                    FSR = _model.Code,
                    FSRDate = stFSRDate.ToString("yyyy-MM-dd"),
                    FSRTime = dbFunction.getCurrentTime(),
                    TxnAmt = clsFunction.sDefaultAmount,
                    TimeEnd = sMTimeEnd,
                    TerminalID = dbFunction.CheckAndSetNumericValue(TerminalID.ToString()),
                    TerminalSN = dbFunction.CheckAndSetStringValue(txtCurTerminalSN.Text),
                    SIMID = dbFunction.CheckAndSetNumericValue(SimID.ToString()),
                    SIMSN = dbFunction.CheckAndSetStringValue(txtCurSIMSN.Text),
                    MerchantContactNo = (txtCustomerContactNo.Text.Length > 0 ? txtCustomerContactNo.Text : txtMerchantMobileNo.Text),
                    MerchantRepresentative = (txtRepresentative.Text.Length > 0 ? txtRepresentative.Text : txtMerchantContactPerson.Text),
                    MerchantRepresentativePosition = (txtCustomerPosition.Text.Length > 0 ? txtCustomerPosition.Text : txtMerchantPosition.Text),
                    FEID = dbFunction.CheckAndSetNumericValue(VendorHelpDeskID.ToString()),
                    FEName = (txtFEName.Text.Length > 0 ? txtFEName.Text : clsFunction.sDash),
                    //Remarks = dbFunction.CheckAndSetStringValue(txtRemarks.Text),
                    Remarks = clsFunction.sDash,
                    SerialNo = sSerialNo,
                    FSRStatus = clsGlobalVariables.FSR_VALID_STATUS,
                    FSRStatusDescription = clsGlobalVariables.FSR_VALID_STATUS_DESC,
                    ServiceStatus = clsGlobalVariables.STATUS_INSTALLED,
                    ServiceStatusDescription = clsGlobalVariables.STATUS_INSTALLED_DESC,
                    ReasonID = dbFunction.CheckAndSetNumericValue(ReasonID.ToString()),
                    Reason = dbFunction.CheckAndSetStringValue(txtReasonDesc.Text),
                    ClientID = dbFunction.CheckAndSetNumericValue(ClientID.ToString()),
                    ClientName = txtClientName.Text,
                    ProcessType = clsGlobalVariables.PROCESS_TYPE_MANUAL_DESC,
                    TAIDNo = clsFunction.sZero,
                    IRIDNo = dbFunction.CheckAndSetNumericValue(IRIDNo.ToString()),
                    IRNo = dbFunction.CheckAndSetNumericValue(txtEntryRequestID.Text),
                    ServiceNo = dbFunction.CheckAndSetNumericValue(txtServiceNo.Text),
                    ServiceCode = _model.Code,
                    FSRRefNo = txtFSRRequestNo.Text,
                    ActionMade = clsGlobalVariables.ACTION_MADE_SUCCESS,
                    JobType = dbFunction.CheckAndSetNumericValue(_model.JobType.ToString()),
                    JobTypeDescription = _model.JobTypeDescription,
                    ProblemReported = dbFunction.CheckAndSetStringValue(txtProbReported.Text),
                    ActualProblemReported = dbFunction.CheckAndSetStringValue(txtActualProblem.Text),
                    ActionTaken = dbFunction.CheckAndSetStringValue(txtActionTaken.Text),
                    AnyComments = dbFunction.CheckAndSetStringValue(txtAnyComments.Text),
                    ProcessedBy = clsUser.ClassProcessedBy,
                    ModifiedBy = clsUser.ClassModifiedBy,
                    ProcessedDateTime = clsUser.ClassProcessedDateTime,
                    ModifiedDateTime = clsUser.ClassModifiedDateTime,
                    geoLatitude = clsFunction.sNull,
                    geoLongitude = clsFunction.sNull,
                    geoCountry = clsFunction.sNull,
                    geoLocality = clsFunction.sNull,
                    geoAddress = clsFunction.sNull
                };
                
                sSQL = IFormat.Insert(details);

                Debug.WriteLine("SaveManualFSRDetail" + sSQL);

                dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 0);

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "FSR Manual Detail", sSQL, "InsertCollectionDetail");

                txtFSRNo.Text = clsLastID.ClassLastInsertedID.ToString(); // Get Last Save FSRNo
            }            
        }

        private bool ValidateFields()
        {
            if (!dbFunction.isValidDescriptionEntry(cboSearchServiceType.Text, "Service Type" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(cboSearchServiceType.SelectedValue.ToString(), "Job Type" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(cboSearchServiceStatus.Text, "Service Status" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(ClientID.ToString(), "Client Name" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(MerchantID.ToString(), "Merchant Name" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(IRIDNo.ToString(), "Merchant IRIDNo" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtRequestNo.Text, "Reference No." + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtEntryRequestID.Text, "Request ID" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(txtReasonDesc.Text, "Reason" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(cboSource.Text, "Source additional type" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(cboCategory.Text, "Category additional type" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
            if (!dbFunction.isValidDescriptionEntry(cboSubCategory.Text, "Sub category additional type" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;

            // check billing type
            if (clsSearch.ClassIsBillType > 0)
            {
                if (cboSearchServiceStatus.Text.Equals(clsGlobalVariables.ACTION_MADE_NEGATIVE) || cboSearchServiceStatus.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS))
                {
                    if (!dbFunction.isValidDescriptionEntry(cboBillingType.Text, "Billing Type" + clsDefines.MUST_NOT_BLANK_MESSAGE)) return false;
                }                
            }

            return true;
        }

        private void autoSaveServiceAndFSR()
        {
            string pSearchValue = "";
            //bool isSuccess = cboSearchServiceStatus.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS);
            if (cboSearchServiceStatus.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS) || cboSearchServiceStatus.Text.Equals(clsGlobalVariables.ACTION_MADE_NEGATIVE))
            {
                //if (!ValidateFields()) return;

                SaveServiceDetail();

                if (cboSearchServiceStatus.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS))
                {
                    SaveManulFSRMaster();
                    SaveManualFSRDetail();

                    ServiceTypeController _model = new ServiceTypeController();
                    _model = _model.getServiceTypeInfo(cboSearchServiceType.Text);
                    if (_model != null)
                    {
                        pSearchValue = $"{txtServiceNo.Text}{clsDefines.gPipe}{clsGlobalVariables.STATUS_INSTALLED}{clsDefines.gPipe}{clsGlobalVariables.STATUS_INSTALLED_DESC}{clsDefines.gPipe}{_model.Code}";
                        dbFunction.parseDelimitedString(pSearchValue, clsDefines.gPipe, 0);
                        dbAPI.ExecuteAPI("PUT", "Update", "Complete Helpdesk Service-FSR", pSearchValue, "", "", "UpdateCollectionDetail");
                    }
                }
                else
                {
                    pSearchValue = $"{txtServiceNo.Text}{clsDefines.gPipe}{txtRequestNo.Text}{clsDefines.gPipe}{clsGlobalVariables.STATUS_ALLOCATED}{clsDefines.gPipe}{clsGlobalVariables.STATUS_ALLOCATED_DESC}{clsDefines.gPipe}{clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC}";
                    dbFunction.parseDelimitedString(pSearchValue, clsDefines.gPipe, 0);
                    dbAPI.ExecuteAPI("PUT", "Update", "Update Servicing Status2", pSearchValue, "", "", "UpdateCollectionDetail");
                }
                
                // update dependency
                txtIRIDNo.Text = IRIDNo.ToString();
                pSearchValue = $"{txtIRIDNo.Text}{clsDefines.gPipe}" +
                                $"{txtServiceNo.Text}{clsDefines.gPipe}" +
                                $"{dbFunction.getFileID(cboDependency, "All Type")}{clsDefines.gPipe}" +
                                $"{dbFunction.getFileID(cboStatusReason, "All Type")}";
                
                dbFunction.parseDelimitedString(pSearchValue, clsDefines.gPipe, 1);

                dbAPI.ExecuteAPI("PUT", "Update", "Dependency", pSearchValue, "", "", "UpdateCollectionDetail");

                if (cboSearchServiceStatus.Text.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS))
                    dbFunction.SetMessageBox("Helpdesk ticket successfully save to JO and FSR.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
                else
                    dbFunction.SetMessageBox("Helpdesk ticket successfully save to JO.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
            }
        }

        private void LoadHelpdeskInfo()
        {

        }
    }
}
