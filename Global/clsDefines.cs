using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsDefines
    {
        // Response FileName
        public const string RESP_REGIONLIST_FILENAME = "respRegionList.json";
        public const string RESP_PROVINCELIST_FILENAME = "respProvinceList.json";
        public const string RESP_REGIONDETAILLIST_FILENAME = "respRegionDetailList.json";
        public const string RESP_CLIENTLIST_FILENAME = "respClientList.json";
        public const string RESP_MERCHANTLIST_FILENAME = "respMerchantList.json";
        public const string RESP_SPLIST_FILENAME = "respSPList.json";
        public const string RESP_FELIST_FILENAME = "respFEList.json";
        public const string RESP_EMPLIST_FILENAME = "respEMPList.json";
        public const string RESP_TERMINALTYPELIST_FILENAME = "respTerminalTypeList.json";
        public const string RESP_TERMINALMODELLIST_FILENAME = "respTerminalModelList.json";
        public const string RESP_TERMINALBRANDLIST_FILENAME = "respTerminalBrandList.json";
        public const string RESP_TERMINALSTATUSLIST_FILENAME = "respTerminalStatusList.json";
        public const string RESP_STATUS_FILENAME = "respStatusList.json";
        public const string RESP_SERVICETYPELIST_FILENAME = "respServiceTypeList.json";
        public const string RESP_SERVICESTATUSLIST_FILENAME = "respServiceStatusList.json";
        public const string RESP_REASON_FILENAME = "respReasonList.json";
        public const string RESP_RESOLUTION_FILENAME = "respResolutionList.json";
        public const string RESP_DEPARTMENTLIST_FILENAME = "respDepartmentList.json";
        public const string RESP_POSITIONLIST_FILENAME = "respPositionList.json";
        public const string RESP_LEAVETYPELIST_FILENAME = "respLeaveTypeList.json";
        public const string RESP_LISTVIEWHEADERCOLUMN_FILENAME = "respListViewHeaderColumn.json";
        public const string RESP_WORKTYPELIST_FILENAME = "respWorkTypeList.json";
        public const string RESP_COUNTRYLIST_FILENAME = "respCountryList.json";
        public const string RESP_LOCATIONLIST_FILENAME = "respLocationList.json";
        public const string RESP_ASSETTYPELIST_FILENAME = "respAssetTypeList.json";
        public const string RESP_CARRIERLIST_FILENAME = "respCarrierList.json";
        public const string RESP_SETUPLIST_FILENAME = "respSetupList.json";
        public const string RESP_PARTICULARLIST_FILENAME = "respParticularList.json";
        public const string RESP_TYPELIST_FILENAME = "respTypeList.json";
        public const string RESP_RENTALFEELIST_FILENAME = "respRentalFeeList.json";
        public const string RESP_ALLTYPE_FILENAME = "respAllType.json";
        public const string RESP_BANKLIST_FILENAME = "respBankList.json";
        public const string RESP_DATABASELIST_FILENAME = "respDatabaseList.json";

        public const string CONTACT_ADMIN_MESSAGE = "Please contact system administrator.";
        public const string MUST_NOT_BLANK_MESSAGE = " must not be blank.";

        // IR Define
        public const string IR_REQUEST_ID = "Request ID";
        public const string IR_MERCHANT_NAME = "Merchant Location/DBA Name";
        public const string IR_TID = "Terminal ID (TID)";
        public const string IR_MID = "Merchant ID (MID)";
        public const string IR_REQUEST_DATE = "Request Date";
        public const string IR_REQUESTOR = "Requestor";
        public const string IR_VENDOR = "Vendor";
        public const string IR_TARGET_INSTALLATION_DATE = "Target Installation date";
        public const string IR_POS_TYPE = "POS Type";
        public const string IR_POS_SETUP = "POS Setup";      
        public const string IR_REQUEST_TYPE = "Request Type";
        public const string IR_REQUEST_PRIORITIZATION = "Request Prioritization";
        public const string IR_RM_INSTRUCTION = "RM Instruction / Remarks";
        public const string IR_ADDRESS = "Address";
        public const string IR_CITY = "City";
        public const string IR_AREA1 = "Area 1 (Metro Manila / Provincial)";
        public const string IR_AREA2 = "Area 2 (Region/Zone/Area code)";
        public const string IR_CONTACT_PERSON = "Contact Person";
        public const string IR_CONTACT_NUMBER = "Contact Number";
        public const string IR_BANCNET_TID = "Bancnet TID";
        public const string IR_BANCNET_MID = "Bancnet MID";
        public const string IR_ALIPAY_TID = "Alipay TID";
        public const string IR_ALIPAY_MID = "Alipay MID";
        public const string IR_WECHAT_TID = "Wechat TID";
        public const string IR_WECHAT_MID = "Wechat MID";
        public const string IR_DETAIL_INFO = "detail_info";

        // IR, profile_info
        public const string IR_DCC_PROFILE_INFO = "DCC (Yes/No)";
        public const string IR_INSTALLMENT_PROFILE_INFO = "Installment (Yes/No)";
        public const string IR_MKE_PROFILE_INFO = "Manual Key Entry (MKE) (Yes/No)";
        public const string IR_PREAUTH_PROFILE_INFO = "Preauth (Yes/No)";
        public const string IR_OFFLINESALE_PROFILE_INFO = "Offline Sale (Yes/No)";
        public const string IR_OFFLINESALELIMIT_PROFILE_INFO = "Offline Sale Limit (To be specified)";
        public const string IR_TIPADJUST_PROFILE_INFO = "Tip Adjust (Yes/No)";
        public const string IR_TIPADJUSTLIMIT_PROFILE_INFO = "Tip Adjust Limit (To be specified)";
        public const string IR_ADDONDEVICE_PROFILE_INFO = "Add-on Device  (To be specified)";
        public const string IR_ECRINTEGRATION_PROFILE_INFO = "ECR Integration (Yes/No)";
        public const string IR_ECRCABLETYPE_PROFILE_INFO = "ECR Cable Type";
        
        // TAGS
        public const string TAG_MERCHANTNAME = "MerchantName";
        public const string TAG_Address = "Address";
        public const string TAG_Province = "Province";
        public const string TAG_TID = "TID";
        public const string TAG_MID = "MID";
        public const string TAG_REGION = "Region";
        public const string TAG_IRIDNO = "IRIDNo";
        public const string TAG_IRNO = "IRNo";
        public const string TAG_IRNO_fsr = "IRNo_fsr";
        public const string TAG_SERVICENO = "ServiceNo";
        public const string TAG_RequestNo = "RequestNo";
        public const string TAG_REFERENCENO = "ReferenceNo";
        public const string TAG_FSRNO = "FSRNo";
        public const string TAG_FSRREFNO = "FSRRefNo";
        public const string TAG_DETAILINFO = "detail_info";
        public const string TAG_CreatedDate = "CreatedDate";
        public const string TAG_ReqInstallationDate = "ReqInstallationDate";
        public const string TAG_ServiceReqDate = "ServiceReqDate";
        public const string TAG_RequestDate = "RequestDate";
        public const string TAG_ScheduleDate = "ScheduleDate";
        public const string TAG_DispatchDate = "DispatchDate";
        public const string TAG_ServiceDate = "ServiceDate";
        public const string TAG_TicketDate = "TicketDate";
        public const string TAG_ServiceCreatedDate = "ServiceCreatedDate";
        public const string TAG_ServiceCreatedTime = "ServiceCreatedTime";
        public const string TAG_FSRDate = "FSRDate";
        public const string TAG_FSRTime = "FSRTime";
        public const string TAG_TimeArrived = "TimeArrived";
        public const string TAG_TimeStart = "TimeStart";
        public const string TAG_TimeEnd = "TimeEnd";

        public const string TAG_svcProcessedDate = "svcProcessedDate";
        public const string TAG_svcModifiedDate = "svcModifiedDate";
        public const string TAG_fsrProcessedDate = "fsrProcessedDate";
        public const string TAG_fsrModifiedDate = "fsrModifiedDate";

        public const string TAG_FEName = "FEName";
        public const string TAG_Position = "Position";
        public const string TAG_Email = "Email";

        public const string TAG_MerchantRepresentative = "MerchantRepresentative";
        public const string TAG_MerchantRepresentativePosition = "MerchantRepresentativePosition";
        public const string TAG_MerchantRepresentativeEmail = "MerchantRepresentativeEmail";

        public const string TAG_ActionMade = "ActionMade";
        public const string TAG_JobType = "JobType";
        public const string TAG_ServiceStatus = "ServiceStatus";
        public const string TAG_ServiceJobTypeDescription = "ServiceJobTypeDescription";
        public const string TAG_JobTypeStatusDescription = "JobTypeStatusDescription";
        public const string TAG_JobTypeDescription = "JobTypeDescription";
        public const string TAG_ServiceStatusDescription = "ServiceStatusDescription";
        public const string TAG_ReasonID = "ReasonID";
        public const string TAG_Reason = "Reason";

        public const string TAG_ContactPerson = "ContactPerson";
        public const string TAG_ContactNumber = "ContactNumber";
        public const string TAG_ContactPosition = "ContactPosition";
        public const string TAG_ContactEmail = "ContactEmail";
        public const string TAG_Mobile = "Mobile";
        public const string TAG_TelNo = "TelNo";
        

        // Mobile
        public const string TAG_MobileID = "MobileID";
        public const string TAG_MobileTerminalID = "MobileTerminalID";
        public const string TAG_MobileVersion = "MobileVersion";
        public const string TAG_MobileTerminalName = "MobileTerminalName";
        public const string TAG_Name = "Name";
        public const string TAG_FullName = "FullName";
        public const string TAG_UserID = "UserID";
        public const string TAG_ParticularID = "ParticularID";
        public const string TAG_UserType = "UserType";

        public const string TAG_IRIDNo = "IRIDNo";

        public const string TAG_TerminalID = "TerminalID";
        public const string TAG_TerminalSN = "TerminalSN";
        public const string TAG_SIMID = "SIMID";
        public const string TAG_SIMSN = "SIMSN";
        public const string TAG_SIMCarrier = "SIMCarrier";

        public const string TAG_ReplaceTerminalID = "ReplaceTerminalID";
        public const string TAG_ReplaceTerminalSN = "ReplaceTerminalSN";
        public const string TAG_ReplaceSIMID = "ReplaceSIMID";
        public const string TAG_ReplaceSIMSN = "ReplaceSIMSN";
        public const string TAG_ReplaceSIMCarrier = "ReplaceSIMCarrier";

        public const string TAG_IRActive = "IRActive";
        public const string TAG_IRStatus = "IRStatus";
        public const string TAG_IRStatusDescription = "IRStatusDescription";
        public const string TAG_ServiceCount = "ServiceCount";
        public const string TAG_FSRCount = "FSRCount";

        // Changes Mapping
        public const string TAG_MapID = "MapID";
        public const string TAG_Code = "Code";
        public const string TAG_Description = "Description";
        public const string TAG_MapFrom = "MapFrom";
        public const string TAG_MapTo = "MapTo";
        public const string TAG_TableName = "TableName";
        public const string TAG_FieldName = "FieldName";
        public const string TAG_FieldSearchKey = "FieldSearchKey";
        public const string TAG_FieldType = "FieldType";
        public const string TAG_MaxLimit = "MaxLimit";
        public const string TAG_ChangeFrom = "ChangeFrom";
        public const string TAG_ChangeTo = "ChangeTo";
        public const string TAG_FieldRootKey = "FieldRootKey";
        public const string TAG_FieldNestedObj = "FieldNestedObj";
        public const string TAG_FieldKeyValue = "FieldKeyValue";

        // Table
        public const string TAG_tblparticular = "tblparticular";
        public const string TAG_tblirdetail = "tblirdetail";
        public const string TAG_tblservicingdetail = "tblservicingdetail";
        public const string TAG_tblfsrdetail = "tblfsrdetail";

        // ControlNo
        public const string TAG_MerchantID = "MerchantID";
        public const string TAG_ClientID = "ClientID";
        public const string TAG_FEID = "FEID";

        public const string TAG_Terminal = "Terminal";
        public const string TAG_SIM = "SIM";
        public const string TAG_Components = "Components";

        // TAT
        public const string TAG_SLA = "SLA";
        public const string TAG_NetworkDays = "NetworkDays";
        public const string TAG_DaysOverDue = "DaysOverDue";
        public const string TAG_TATStatus = "TATStatus";

        public const string HEADER_Current_Terminal = "Current terminal";
        public const string HEADER_Current_SIM = "Current SIM";
        public const string HEADER_Replace_Terminal = "Replace terminal";
        public const string HEADER_Replace_SIM = "Replace SIM";

        // Stock Detail
        public const string TAG_StockID = "StockID";
        public const string TAG_ItemID = "ItemID";
        public const string TAG_ReplaceItemID = "ReplaceItemID";
        public const string TAG_LocationID = "LocationID";
        public const string TAG_Location = "Location";
        public const string TAG_StockStatus = "StockStatus";
        public const string TAG_StockStatusDescription = "StockStatusDescription";
        public const string TAG_SerialNo = "SerialNo";
        public const string TAG_DeliveryDate = "DeliveryDate";
        public const string TAG_ReceiveDate = "ReceiveDate";
        public const string TAG_ReleaseDate = "ReleaseDate";
        public const string TAG_Allocation = "Allocation";
        public const string TAG_AssetType = "AssetType";
        public const string TAG_PONo = "PONo";
        public const string TAG_InvNo = "InvNo";
        public const string TAG_PartNo = "PartNo";
        public const string TAG_Remarks = "Remarks";

        // Inventory
        public const string HDR_TEMPLATE_LineNo = "NO";
        public const string HDR_TEMPLATE_SerialNo = "SERIAL NO";
        public const string HDR_TEMPLATE_PartNumber = "PART NUMBER";
        public const string HDR_TEMPLATE_Type = "TYPE";
        public const string HDR_TEMPLATE_Model = "MODEL";
        public const string HDR_TEMPLATE_Brand = "BRAND";
        public const string HDR_TEMPLATE_PONumber = "PO NUMBER";
        public const string HDR_TEMPLATE_InvoiceNumber = "INVOICE NUMBER";
        public const string HDR_TEMPLATE_AssetType = "ASSET TYPE";
        public const string HDR_TEMPLATE_Carrier = "CARRIER";
        public const string HDR_TEMPLATE_DeliveryDate = "DELIVERY DATE";
        public const string HDR_TEMPLATE_ReceivedDate = "RECEIVED DATE";
        public const string HDR_TEMPLATE_Location = "LOCATION";
        public const string HDR_TEMPLATE_Status = "STATUS";
        public const string HDR_TEMPLATE_Allocation = "ALLOCATION";
        public const string HDR_TEMPLATE_Remarks = "REMARKS";

        public const string TAG_CustomerName = "CustomerName";
        public const string TAG_CustomerContactNo = "CustomerContactNo";
        public const string TAG_CustomerPosition = "CustomerPosition";
        public const string TAG_CustomerEmail = "CustomerEmail";

        public const string TAG_Dependency = "Dependency";
        public const string TAG_StatusReason = "StatusReason";

        public const string TAG_DateRange = "DateRange";
        public const string TAG_TAmount = "TAmount";
        public const string TAG_TRecurring = "TRecurring";

        // Character
        public const char gPipe = '|';
        public const char gCaret = '^';
        public const char gAsterisk = '#';
        public const char gSharp = '#';
        public const char gDollar = '$';
        public const char gAtSign = '@';
        public const char gPercent = '%';
        public const char gExlamationMark = '!';
        public const char gComma = ',';
        public const char gTilde = '~';
        public const char gDash = '-';
        public const char gZero = '0';
        public const string gYes = "YES";
        public const string gNo = "NO";
        public const string gSpace = " ";
        public const string gUnderScore = "_";
        public const char gNewLine = '\n';
        public const string gNull = "";

        public const string NOT_SELECTED = "[NOT SELECTED]";

        public const string HEADER_CURRENT_TERMINAL = "CURRENT TERMINAL DETAIL";
        public const string HEADER_CURRENT_SIM = "CURRENT SIM DETAIL";
        public const string HEADER_REPLACE_TERMINAL = "REPLACE WITH TERMINAL DETAIL";
        public const string HEADER_REPLACE_SIM = "REPLACE WITH SIM DETAIL";

        public const string CONTROLID_PREFIX_IR = "IR";
        public const string CONTROLID_PREFIX_SERVICE = "SR";
        public const string CONTROLID_PREFIX_FSR = "FR";
        public const string CONTROLID_PREFIX_TERMINAL = "RTR";
        public const string CONTROLID_PREFIX_SIM = "RSR";
        public const string CONTROLID_PREFIX_REFNO = "RF";
        public const string CONTROLID_PREFIX_NO_REQUESTID = "SD";
        public const string CONTROLID_PREFIX_HELPDESK = "HD";
        public const string CONTROLID_PREFIX_IMPORT_BASE = "IB";
        public const string CONTROLID_PREFIX_MSP = "MSP";

        public const string TAG_MIN = "MIN";
        public const string TAG_MAX = "MAX";

        // Terminal/SIM
        public const string TAG_TerminalTypeID = "TerminalTypeID";
        public const string TAG_TerminalModelID = "TerminalModelID";
        public const string TAG_TerminalBrandID = "TerminalBrandID";

        public const string TAG_TerminalType = "TerminalType";
        public const string TAG_TerminalModel = "TerminalModel";
        public const string TAG_TerminalBrand = "TerminalBrand";
        public const string TAG_TerminalLocation = "TerminalLocation";

        public const string TAG_ReplaceTerminalType = "ReplaceTerminalType";
        public const string TAG_ReplaceTerminalModel = "ReplaceTerminalModel";

        public const string TAG_AppVersion = "AppVersion";
        public const string TAG_AppCRC = "AppCRC";
        public const string TAG_TerminalStatus = "TerminalStatus";
        public const string TAG_TerminalStatusDescription = "TerminalStatusDescription";
        public const string TAG_SIMStatus = "SIMStatus";
        public const string TAG_SIMStatusDescription = "SIMStatusDescription";
        public const string TAG_SIMLocation = "SIMLocation";

        // Geometric Location
        public const string TAG_geoLatitude = "geoLatitude";
        public const string TAG_geoLongitude = "geoLongitude";
        public const string TAG_geoCountry = "geoCountry";
        public const string TAG_geoLocality = "geoLocality";
        public const string TAG_geoAddress = "geoAddress";

        public const string TAG_SelectionID = "SelectionID";
        public const string TAG_Type = "Type";
        public const string TAG_TypeID = "TypeID";
        public const string TAG_TypeValue = "TypeValue";
        public const string TAG_FormatType = "FormatType";

        public const string TAG_ProcessedAt = "ProcessedAt";
        public const string TAG_ProcessedBy = "ProcessedBy";
        public const string TAG_ProcessedDate = "ProcessedDate";
        public const string TAG_ProcessedTime = "ProcessedTime";
        public const string TAG_ModifiedAt = "ModifiedAt";
        public const string TAG_ModifiedBy = "ModifiedBy";
        public const string TAG_DispatchBy = "DispatchBy";

        public const string TAG_isDiagnostic = "isDiagnostic";
        public const string TAG_isBillable = "isBillable";
        public const string TAG_ClientName = "ClientName";
        public const string TAG_ClientEmail = "ClientEmail";

        public const string TAG_ParticularName = "ParticularName";

        public const string TAG_Price = "Price";
        public const string TAG_RentFee = "RentFee";
        public const string TAG_QueryString = "QueryString";
        public const string TAG_SequenceDisplay = "SequenceDisplay";


        // File Extension
        public const string FILE_EXT_PNG = ".png";
        public const string FILE_EXT_BMP = ".bmp";
        public const string FILE_EXT_JPG = ".jpg";
        public const string FILE_EXT_JSON = ".json";
        public const string FILE_EXT_PDF = ".pdf";
        public const string FILE_EXT_XLXS = ".xlsx";
        public const string FILE_EXT_CSV = ".csv";

        public const string VALID_SHEET_NAME = "Sheet1";

        public static string DEFAULT_GEO_LATITUDE = "0.00000000";
        public static string DEFAULT_GEO_LONGITUDE = "0.00000000";
        public static string DEFAULT_GEO_COUNTRY = "PHILIPPINES";

        // Service Status
        public static string SERVICE_STATUS_PREPARATION = "PREPARATION";
        public static string SERVICE_STATUS_OVERALL = "OVERALL";
        public static string SERVICE_STATUS_PENDING = "PENDING";
        public static string SERVICE_STATUS_PROCESSING = "PROCESSING";
        public static string SERVICE_STATUS_PENDINGANDPROCESSING = "PENDING/PROCESSING";
        public static string SERVICE_STATUS_COMPLETED = "COMPLETED";
        public static string SERVICE_STATUS_OVERALL_PENDING = "OVERALL PENDING";

        public const string TAG_InstallationServiceNo = "InstallationServiceNo";
        public const string TAG_ReplacementServiceNo = "ReplacementServiceNo";

        public const string TAG_SelectedValue = "SelectedValue";

        public const string TAG_Signature1 = "Signature1";
        public const string TAG_Signature2 = "Signature2";

        public const string NO_REQUEST_ID = "NO REQUEST ID";

        public const string NEW_RECORD = "NEW RECORD";
        public const string UPDATE_RECORD = "UPDATE RECORD";

        public const string MANUAL_FSR = "MANUAL FSR";
        public const string DIGITAL_FSR = "DIGITAL FSR";

        public const string FIELD_CHECK_MSG = "Field Check";
        public const string TAKE_FEW_MINUTE_MSG = "This process may take a few minute(s).";

        public const string OPEN_TICKET = "OPEN";
        public const string CLOSE_TICKET = "CLOSE";

        // TAT
        public const string WITHIN_TAT = "WITHIN TAT";
        public const string BEYOND_TAT = "BEYOND TAT";

        // Whos Online
        public const string TAG_LogID = "LogID";        
        public const string TAG_UserName = "UserName";        
        public const string TAG_ComputerName = "ComputerName";
        public const string TAG_PublishVersion = "PublishVersion";
        public const string TAG_LogDate = "LogDate";
        public const string TAG_LogTime = "LogTime";

        public const string TAG_isAdd = "isAdd";        
        public const string TAG_isView = "_isView";
        public const string TAG_isPrint = "isPrint";
        public const string TAG_isDelete = "isDelete";
        public const string TAG_isUpdate = "isUpdate";
        public const string TAG_PrivacyID = "PrivacyID";
        public const string TAG_isChecked = "isChecked";

        public const string TAG_SourceID = "SourceID";
        public const string TAG_Source = "Source";
        public const string TAG_CategoryID = "CategoryID";
        public const string TAG_Category = "Category";
        public const string TAG_SubCategoryID = "SubCategoryID";
        public const string TAG_SubCategory = "SubCategory";
        public const string TAG_BillingTypeID = "BillingTypeID";
        public const string TAG_BillType = "BillingType";

        public const string TAG_TInstallation = "TInstallation";
        public const string TAG_TReprogramming = "TReprogramming";
        public const string TAG_TServicing = "TServicing";
        public const string TAG_TReplacement = "TReplacement";
        public const string TAG_TPullOut = "TPullOut";

        public const string TAG_TransNo = "TransNo";
        public const string TAG_ControlNo = "ControlNo";
        public const string TAG_FileName = "FileName";
        public const string TAG_FileSize = "FileSize";
        public const string TAG_FullPath = "FullPath";
        public const string TAG_FileType = "FileType";

        public const string TAG_MSPNo = "MSPNo";
        public const string TAG_RegisteredName = "RegisteredName";
        public const string TAG_TradeName = "TradeName";
        public const string TAG_TINNo = "TINNo";

        public const string TAG_CreatedID = "CreatedID";
        public const string TAG_CreatedAt = "CreatedAt";
        public const string TAG_CreatedBy = "CreatedBy";

        public const string TAG_UpdatedID = "UpdatedID";
        public const string TAG_UpdatedAt = "UpdatedAt";
        public const string TAG_UpdatedBy = "UpdatedBy";

        public const string TAG_SubmitID = "SubmitID";
        public const string TAG_SubmitAt = "SubmitAt";
        public const string TAG_SubmitBy = "SubmitBy";

        public const string TAG_CheckedID = "CheckedID";
        public const string TAG_CheckedAt = "CheckedAt";
        public const string TAG_CheckedBy = "CheckedBy";

        public const string TAG_AcquirerType = "AcquirerType";
        public const string TAG_NoBType = "NoBType";
        public const string TAG_BusType = "BusType";
        public const string TAG_SchemeType = "SchemeType";
        public const string TAG_ReferralType = "ReferralType";

        public const string TAG_MDRCreditType = "MDRCreditType";
        public const string TAG_MDRDebitType = "MDRDebitType";
        public const string TAG_MDRInstType = "MDRInstType";

        public const string TAG_MSPStatusDesc = "MSPStatusDesc";

        public const string TAG_NoOfPOS = "NoOfPOS";
        public const string TAG_NoOfYear = "NoOfYear";
        public const string TAG_RentalAmt = "RentalAmt";
        public const string TAG_BankAcntName = "BankAcntName";
        public const string TAG_BankAcntNo = "BankAcntNo";
        public const string TAG_BankSettlement = "BankSettlement";
        public const string TAG_BankReferring = "BankReferring";

        public const string TAG_ResultStatus = "ResultStatus";
        public const string TAG_ResultStatusDesc = "ResultStatusDesc";

        public const string TAG_NoOfDocument = "NoOfDocument";
        
        public const string TAG_ResultCreatedAt = "ResultCreatedAt";
        public const string TAG_ResultSubmitAt = "ResultSubmitAt";

        public const string TAG_ResultRemarks = "ResultRemarks";

        public const string TAG_DispatchID = "DispatchID";
        public const string TAG_Dispatcher = "Dispatcher";
        public const string TAG_DispatcherEmail = "DispatcherEmail";
        public const string TAG_TCount = "TCount";

        public const string TAG_AutoSettlement = "AutoSettlement";
        public const string TAG_SettlementTime = "SettleTime";
        public const string TAG_Contactless = "Contactless";

        public const string TAG_IsPassword = "isPassword";

        public const string TAG_RegionType = "RegionType";
        public const string TAG_RegionID = "RegionID";

        public const string TAG_Password = "Password";

        public const string TAG_ID = "ID";

        // Helpdesk Details
        public const string TAG_HD_ProblemNo = "ProblemNo";
        public const string TAG_HD_AssistNo = "AssistNo";
        public const string TAG_HD_RequestID = "RequestID";
        public const string TAG_HD_HelpdeskID = "HelpdeskID";
        public const string TAG_HD_HelpdeskName = "HelpdeskName";
        public const string TAG_HD_TeamLeadID = "TeamLeadID";
        public const string TAG_HD_TeamLeadName = "TeamLeadName";
        public const string TAG_HD_CreatedDate = "CreatedDate";
        public const string TAG_HD_ReasonID = "ReasonID";
        public const string TAG_HD_Reason = "Reason";
        public const string TAG_HD_ContactPerson = "ContactPerson";
        public const string TAG_HD_ContactNo = "ContactNo";
        public const string TAG_HD_ContactPosition = "ContactPosition";
        public const string TAG_HD_ContactEmail = "ContactEmail";
        public const string TAG_HD_RequestedBy = "RequestedBy";
        public const string TAG_HD_Representative = "Representative";
        public const string TAG_HD_TerminalID = "TerminalID";
        public const string TAG_HD_SIMID = "SIMID";
        public const string TAG_HD_TerminalSN = "TerminalSN";
        public const string TAG_HD_SIMSN = "SIMSN";
        public const string TAG_HD_DockerSN = "DockerSN";
        public const string TAG_HD_ActualProblem = "ActualProblem";
        public const string TAG_HD_ProblemReported = "ProblemReported";
        public const string TAG_HD_RemarksHelpDesk = "RemarksHelpDesk";
        public const string TAG_HD_RemarksService = "RemarksService";
        public const string TAG_HD_SourceID = "SourceID";
        public const string TAG_HD_CategoryID = "CategoryID";
        public const string TAG_HD_SubCategoryID = "SubCategoryID";
        public const string TAG_HD_Source = "Source";
        public const string TAG_HD_Category = "Category";
        public const string TAG_HD_SubCategory = "SubCategory";
        public const string TAG_HD_CreatedID = "CreatedID";
        public const string TAG_HD_CreatedAt = "CreatedAt";
        public const string TAG_HD_CreatedBy = "CreatedBy";
        public const string TAG_HD_UpdatedAt = "UpdatedAt";
        public const string TAG_HD_UpdatedBy = "UpdatedBy";
        public const string TAG_HD_CompletedAt = "CompletedAt";
        public const string TAG_HD_Status = "Status";
        public const string TAG_HD_TimeAssisted = "TimeAssisted";
        public const string TAG_HD_TimeAssistAt = "TimeAssistAt";
        public const string TAG_HD_TimeStart = "TimeStart";
        public const string TAG_HD_TimeEnd = "TimeEnd";
        public const string TAG_HD_Attempts = "Attempts";
        public const string TAG_HD_isActive = "isActive";
        public const string TAG_HD_CreatedByName = "CreatedByName";
        public const string TAG_HD_AppVersion = "AppVersion";
        public const string TAG_HD_AppCrc = "AppCrc";
        public const string TAG_HD_ProblemID = "ProblemID";
        public const string TAG_HD_TicketStatus = "TicketStatus";

        // Helpdesk Master
        public const string TAG_HD_ID = "ID";
        public const string TAG_HD_IRIDNo = "IRIDNo";
        public const string TAG_HD_MerchantID = "MerchantID";
        public const string TAG_HD_TID = "TID";
        public const string TAG_HD_MID = "MID";
        public const string TAG_HD_MerchantName = "MerchantName";
        public const string TAG_HD_ClientID = "ClientID";
        public const string TAG_HD_ReferenceNo = "ReferenceNo";
        public const string TAG_HD_RequestDate = "RequestDate";
        public const string TAG_HD_Requestor = "Requestor";
        public const string TAG_HD_JobType = "JobType";

        // Helpdesk Status
        public const string TAG_HD_Resolved = "RESOLVED";
        public const string TAG_HD_Pending = "PENDING";
        public const string TAG_HD_Negative = "NEGATIVE";
        public const string TAG_HD_Contacted = "CONTACTED";
        public const string TAG_HD_Success = "SUCCESS";

        // Report Status
        public const string TAG_StatusID = "StatusID";
        public const string TAG_Status = "Status";
        public const string TAG_isReset = "isReset";

        public const string TAG_Requestor = "Requestor";
        public const string TAG_RequestTypeID = "RequestTypeID";
        public const string TAG_RequestType = "RequestType";

        public const string TAG_DateTimeStamp = "DateTimeStamp";

        // Position Type
        public const int FIELD_ENGINEER_POSITION_TYPE = 1; // Field Engineer
        public const int DISPATCHER_POSITION_TYPE = 2; // Dispatcher
        public const int HELPDESK_POSITION_TYPE = 3; // Helpdesk
        public const int TEAMLEAD_POSITION_TYPE = 4; // TeamLead
        public const int HEAD_POSITION_TYPE = 5; // Head
        public const int ADMIN_POSITION_TYPE = 6; // Admin

        public const string SEARCH_TERMINAL = "Terminal";
        public const string SEARCH_SIM = "SIM";
        public const string SEARCH_COMPONENTS = "Components";

        public const int GROUP_TERMINAL = 1;
        public const int GROUP_BASE = 2;

        // Signature / Image Index
        public const int ATTACHMENT_1_INDEX = 1;
        public const int ATTACHMENT_2_INDEX = 2;
        public const int ATTACHMENT_3_INDEX = 3;
        public const int ATTACHMENT_4_INDEX = 4;
        public const int MERCHANT_SIGNATURE_INDEX = 5;
        public const int VENDOR_SIGNATURE_INDEX = 6;

        public const string FSR_FILENAME_PREFIX = "_fsr";
        public const string DIAGNOSTIC_FILENAME_PREFIX = "_diag";

        public const string DEV_DATE = "1980-09-11";

        // UserType        
        public const string USERTYPE_ADMIN = "ADMIN"; // Full Access
        public const string USERTYPE_EDITOR = "EDITOR"; // Can create, modify, and delete eg. dispatcher
        public const string USERTYPE_USER = "USER"; // Has very limited access

        public const string ROOTKEY_RAWDATA_INFO = "rawdata_info";
        public const string ROOTKEY_PROFILE_INFO = "profile_info";
        public const string ROOTKEY_PROFILE_CONFIG_INFO = "profile_config_info";
        public const string NESTED_OBJECT_VALUES = "Values";

        // table
        public const string Table_ServiceChangesDetail = "tblservicechangesdetail";
        public const string Table_ServicingDetail = "tblservicingdetail";
        public const string Table_FSRDetail = "tblfsrdetail";

        // fieldname
        public const string FieldName_FSRNo = "FSRNo";
        public const string FieldName_ServiceNo = "ServiceNo";
        public const string FieldName_IRIDNo = "IRIDNo";
        public const string FieldName_ParticularID = "ParticularID";
        public const string FieldName_MerchantID = "MerchantID";


        public const string GetID_Info = "GetID Info";
        public const string GetDesc_Info = "GetDesc Info";

        public const string SearchBy_Particular = "Particular";
        public const string SearchBy_Type = "Type";

        // Report Status
        public static string REPORT_STATUS_IDLE = "IDLE";
        public static string REPORT_STATUS_PROCESSING = "PROCESSING";
        public static string REPORT_STATUS_FAILED = "FAILED";
        public static string REPORT_STATUS_COMPLETED = "COMPLETED";

        // Mode Type
        public static string Mode_Type_Deploy = "Deploy";
        public static string Mode_Type_Return = "Return";

        public static int FSR_GENERATE_TIMEOUT = 30;

        // Additional tags  
        public const string TAG_Path = "Path";
        public const string TAG_Size = "Size";
        public const string TAG_LastModified = "LastModified";
        public const string TAG_Prefix = "Prefix";
        public const string TAG_JpgCount = "JpgCount";
        public const string TAG_PngCount = "PngCount";

        // Tag Month
        public const string TAG_January = "January";
        public const string TAG_February = "February";
        public const string TAG_March = "March";
        public const string TAG_April = "April";
        public const string TAG_May = "May";
        public const string TAG_June = "June";
        public const string TAG_July = "July";
        public const string TAG_August = "August";
        public const string TAG_September = "September";
        public const string TAG_October = "October";
        public const string TAG_November = "November";
        public const string TAG_December = "December";
        public const string TAG_Total = "Total";

        // Trans Type
        public const string TAG_Credit = "Credit";
        public const string TAG_Debit = "Debit";
        public const string TAG_QRPay = "QRPay";
        public const string TAG_QRPh = "QRPh";

        // Qtr
        public const string TAG_Quarter = "Quarter";
        public const string TAG_StartDate = "StartDate";
        public const string TAG_EndDate = "EndDate";

        public const string TAG_Result = "Result";

        public const string TAG_Profile_Info = "profile_info";
        public const string TAG_RawData_Info = "rawdata_info";


        // MovementType
        public const string MOVEMENTTYPE_IMPORT = "IMPORT";
        public const string MOVEMENTTYPE_RELEASE = "RELEASE";
        public const string MOVEMENTTYPE_TRANSFER = "TRANSFER";

        public const string MSG_FOUND = "Found";
        public const string MSG_NOT_FOUND = "Not Found";

    }
}
