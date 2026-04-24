using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    class clsGlobalVariables
    {
        public static string strLocalIP = "";
        public static string strPublicIP = "";
        public static string strComputerName = "";

        public static string strIniSettingPath = "";
        public static string strSource = "";
        public static string strServer = "";
        public static string strDatabase = "";
        public static string strUserName = "";
        public static string strPassword = "";
        public static string strSecurity = "False";
        public static string strPort = "";
        public static string strTimeOut = "5000";       

        // API
        public static string strAPIURL = "";
        public static string strAPIPath = "";
        public static string strAPIKeys = "";
        public static string strAPIContentType = "";
        public static string strAPIAuthUser = "";
        public static string strAPIAuthPassword = "";
        public static string strAPIServerIP = "";
        public static string strAPIServerPort = "";
        public static string strAPIDashboardURL = "";
        public static string strAPIServerIPAddress = "";
        public static string strAPIFolder = "";
        public static string strAPIImagePath = "";
        public static string strAPIBank = "";
        public static int strAPISSLEnable = 0;

        // FTP
        public static string strFTPURL = "";
        public static string strFTPPORT = "";
        public static string strFTPUploadPath = "";
        public static string strFTPDownloadPath = "";              
        public static string strFTPUserName = "";
        public static string strFTPPassword = "";
        public static string strFTPLocalPath = "";
        public static string strFTPLocalImportPath = "";
        public static string strFTPLocalExportPath = "";
        public static string strFTPRemoteFSRPath = "";
        public static string strFTPRemoteImagesPath = "";
        public static string strLocalSignPath = "";
        public static string strFTPRemoteIRPath = "";
        public static string strFTPRemoteSerialPath = "";
        public static string strFTPRemoteErmPath = "";

        // System
        public static string strSplashInterval = "";
        public static string strSplashTimeOut = "";
        public static string strCheckNetLink = "";
        public static string strNetLink = "";
        public static string strCheckIPAddress = "";
        public static string strIPAddress = "";
        public static string strConnectionType = "";
        public static string strPromptAPIRequest = "";
        public static string strPromptAPIResponse = "";

        // Version
        public static string strCheckUpdate = "";
        public static string strPublishDate = "";
        public static string strPublishVersion = "";

        // JSON
        public static string strJSONRequest = "";
        public static string strJSONResponse = "";
        public static bool fJSONValidResponse = false;

        // Particular (Type)
        public static int iMerchant_Type = 1; // for merchant with IR
        public static int iClient_Type = 2;
        public static int iFE_Type = 3;
        public static int iSupplier_Type = 4;
        public static int iSP_Type = 5;
        public static int iEMP_Type = 6;
        public static int iDashboard_Type = 2;
        public static int iMerchant_Type_List = 7;  // for merchant only

        // Terminal isStatus 
        public static int iTerminal_Type = 1;
        public static int iSVC_Type = 2;

        // Particular (Description)
        public static string sMerchant_Type = "Merchant";
        public static string sMerchant_Type_List = "MerchantList";
        public static string sClient_Type = "Client";
        public static string sFE_Type = "Field Engineer";
        public static string sSupplier_Type = "Supplier";
        public static string sSP_Type = "Service Provider";
        public static string sEMP_Type = "Employee";

        // API Method
        public static string sAPIMethod_GET = "GET";
        public static string sAPIMethod_PUT = "PUT";
        public static string sAPIMethod_POST = "POST";
        public static string sAPIMethod_DELETE = "DELETE";

        // API Variable
        public static string ExceptionNumber = "";
        public static string ExceptionMessage = "";
        public static bool isAPIResponseOK = false;
        public static string sAPIResponseCode = "";
        public static int iAPIRecordCount = 0;
        public static string SUCCESS_RESPONSE = "200";
        public static string API_RESPONSE_ERROR = "300";
        public static string UNDEFINED_ERROR = "500";
        public static string REQUEST_METHOD_NOT_VALID = "100";
        public static string REQUEST_CONTENTTYPE_NOT_VALID = "101";
        public static string REQUEST_NOT_VALID = "102";
        public static string VALIDATE_PARAMETER_REQUIRED = "103";
        public static string VALIDATE_PARAMETER_DATATYPE = "104";
        public static string API_NAME_REQUIRED = "105";
        public static string API_PARAM_REQUIRED = "106";
        public static string API_DOST_NOT_EXIST = "107";
        public static string INVALID_USER_PASS = "108";
        public static string USER_NOT_ACTIVE = "109";
        public static string NO_RECORD_FOUND = "110";
        public static string RECORD_FOUND = "111";
        public static string API_KEY_REQUIRED = "112";
        public static string INVALID_AUTH_USER_PW = "113";
        public static string INVALID_METHOD = "114";

        public static int API_RESPONSE_SUCCESS = 0;
        public static int API_RESPONSE_FAILED = 1;

        // Service Type
        public static string SERVICE_TYPE_TRXN = "TRXN";

        // IR       
        public static int STATUS_AVAILABLE = 1;
        public static string STATUS_AVAILABLE_DESC = "AVAILABLE";

        public static int STATUS_ALLOCATED = 2;
        public static string STATUS_ALLOCATED_DESC = "ALLOCATED";        

        public static int STATUS_REPAIR = 3;
        public static string STATUS_REPAIR_DESC = "FOR REPAIR";

        public static int STATUS_DAMAGE = 4;
        public static string STATUS_DAMAGE_DESC = "DEFECTIVE";

        public static int STATUS_LOSS = 5;
        public static string STATUS_LOSS_DESC = "LOSS";

        public static int STATUS_BORROWED = 6;
        public static string STATUS_BORROWED_DESC = "LOAN/BORROW";

        public static int STATUS_INSTALLED = 7;
        public static string STATUS_INSTALLED_DESC = "INSTALLED";

        public static int STATUS_DISPATCH = 8;
        public static string STATUS_DISPATCH_DESC = "DISPATCH";

        public static int STATUS_NEGATIVE = 9;
        public static string STATUS_NEGATIVE_DESC = "NEGATIVE";

        public static int STATUS_REPROGRAMMED = 10;
        public static string STATUS_REPROGRAMMED_DESC = "REPROGRAMMING";

        public static int STATUS_PULLED_OUT = 11;
        public static string STATUS_PULLED_OUT_DESC = "PULL-OUT";

        public static int STATUS_DIAGNOSTIC = 12;
        public static string STATUS_DIAGNOSTIC_DESC = "DIAGNOSTIC";

        public static int STATUS_REPLACEMENT = 13;
        public static string STATUS_REPLACEMENT_DESC = "REPLACEMENT";

        public static int STATUS_SERVICING = 14;
        public static string STATUS_SERVICING_DESC = "SERVICING";

        public static int STATUS_INSTALLATION = 15;
        public static string STATUS_INSTALLATION_DESC = "INSTALLATION";

        public static int STATUS_CANCELLED = 16;
        public static string STATUS_CANCELLED_DESC = "CANCELLED";

        public static int STATUS_CREATED = 17;
        public static string STATUS_CREATED_DESC = "CREATE";

        public static int STATUS_UPDATED = 18;
        public static string STATUS_UPDATED_DESC = "UPDATE";

        public static int STATUS_DELETED = 19;
        public static string STATUS_DELETED_DESC = "DELETE";

        public static int STATUS_READ = 20;
        public static string STATUS_READ_DESC = "READ";

        public static int STATUS_REPLACED = 21;
        public static string STATUS_REPLACED_DESC = "REPLACED";

        public static int STATUS_PULLEDOUT = 22;
        public static string STATUS_PULLEDOUT_DESC = "PULLED-OUT";

        public static int STATUS_IMPORTED = 23;
        public static string STATUS_IMPORTED_DESC = "IMPORT";

        public static string STATUS_HOLD_DESC = "HOLD SERVICE";
        public static string STATUS_CANCEL_DESC = "CANCEL SERVICE";
        public static string STATUS_RESUME_DESC = "RESUME SERVICE";

        // User
        public static int LOGIN_STATUS = 1;
        public static string LOGIN_STATUS_DESC = "LOGIN";

        public static int LOGOUT_STATUS = 2;
        public static string LOGOUT_STATUS_DESC = "LOGOUT";

        public static string USER_LOGIN = "User LogIn";
        public static string USER_LOGOUT = "User LogOut";

        // FSR
        public static int FSR_VALID_STATUS = 1;
        public static string FSR_VALID_STATUS_DESC = "VALID FSR";

        public static int FSR_INVALID_STATUS = 2;
        public static string FSR_INVALID_STATUS_DESC = "INVALID FSR";

        // TA
        public static int TA_STATUS_INSTALLED = 1;
        public static string TA_STATUS_INSTALLED_DESC = "INSTALLED";
        public static string TA_STATUS_INSTALLED_SUB_DESC = "FOR INSTALLATION";
        public static string TA_STATUS_INSTALLED_CODE = "5555";

        public static int TA_STATUS_NEGATIVE = 2;
        public static string TA_STATUS_NEGATIVE_DESC = "NEGATIVE";
        public static string TA_STATUS_NEGATIVE_SUB_DESC = "FOR NEGATIVE";
        public static string TA_STATUS_NEGATIVE_CODE = "4444";

        public static int TA_STATUS_REPROGRAMMED = 3;
        public static string TA_STATUS_REPROGRAMMED_DESC = "REPROGRAMMED";
        public static string TA_STATUS_REPROGRAMMED_SUB_DESC = "FOR REPROGRAMMING";
        public static string TA_STATUS_REPROGRAMMED_CODE = "3333";

        public static int TA_STATUS_PULLEDOUT = 4;
        public static string TA_STATUS_PULLEDOUT_DESC = "PULLED-OUT";
        public static string TA_STATUS_PULLEDOUT_SUB_DESC = "FOR PULL-OUT";
        public static string TA_STATUS_PULLEDOUT_CODE = "2222";

        public static int TA_STATUS_DIAGNOSTIC = 5;
        public static string TA_STATUS_DIAGNOSTIC_DESC = "DIAGNOSTIC";
        public static string TA_STATUS_DIAGNOSTIC_SUB_DESC = "FOR DIAGNOSTIC";
        public static string TA_STATUS_DIAGNOSTIC_CODE = "7777";

        public static int TA_STATUS_SERVICING = 6;
        public static string TA_STATUS_SERVICING_DESC = "SERVICING";
        public static string TA_STATUS_SERVICING_SUB_DESC = "FOR SERVICING";
        public static string TA_STATUS_SERVICING_CODE = "9999";

        public static int TA_STATUS_REPLACEMENT = 7;
        public static string TA_STATUS_REPLACEMENT_DESC = "REPLACEMENT";
        public static string TA_STATUS_REPLACEMENT_SUB_DESC = "FOR REPLACEMENT";
        public static string TA_STATUS_REPLACEMENT_CODE = "8888";

        public static int TA_STATUS_DISPATCH = 8;
        public static string TA_STATUS_DISPATCH_DESC = "DISPATCH";
        public static string TA_STATUS_DISPATCH_SUB_DESC = "FOR DISPATCH";
        public static string TA_STATUS_DISPATCH_CODE = "1111";

        public static int TA_STATUS_HOLD = 9;
        public static string TA_STATUS_HOLD_DESC = "HOLD SERVICE";
        public static string TA_STATUS_HOLD_SUB_DESC = "FOR HOLD SERVICE";
        public static string TA_STATUS_HOLD_CODE = "1001";

        public static int TA_STATUS_CANCEL = 10;
        public static string TA_STATUS_CANCEL_DESC = "CANCEL SERVICE";
        public static string TA_STATUS_CANCEL_SUB_DESC = "FOR CANCEL SERVICE";
        public static string TA_STATUS_CANCEL_CODE = "1002";

        public static int PROCESS_TYPE_AUTO = 1;
        public static string PROCESS_TYPE_AUTO_DESC = "AUTO";

        public static int PROCESS_TYPE_MANUAL = 2;
        public static string PROCESS_TYPE_MANUAL_DESC = "MANUAL";
        
        public static string V3_TOUCH_DESC = "'V3 TOUCH'";
        public static string V5S_DESC = "'V5S'";
        public static string MP200_DESC = "'MP200'";
        public static string ANDROID_SE_DESC = "'ANDROID SE'";
        public static string V3_NT_DESC = "'V3 NT'";
        public static string V3_DOCK_DESC = "'V3 DOCK'";

        // Job Type
        public static int JOB_TYPE_STATUS_PREPARATION = 0;
        public static string JOB_TYPE_STATUS_PREPARATION_DESC = "PREPARATION";

        public static int JOB_TYPE_STATUS_PENDING = 1;
        public static string JOB_TYPE_STATUS_PENDING_DESC = "PENDING";

        public static int JOB_TYPE_STATUS_REPROCESSING = 2;
        public static string JOB_TYPE_STATUS_REPROCESSING_DESC = "REPROCESSING";

        public static int JOB_TYPE_STATUS_READY_TO_PROCESS = 3;
        public static string JOB_TYPE_STATUS_READY_TO_PROCESS_DESC = "READY TO PROCESS";

        public static int JOB_TYPE_STATUS_PROCESSING = 4;
        public static string JOB_TYPE_STATUS_PROCESSING_DESC = "PROCESSING";

        public static int JOB_TYPE_STATUS_COMPLETED = 5;
        public static string JOB_TYPE_STATUS_COMPLETED_DESC = "COMPLETED";

        public static int JOB_TYPE_STATUS_FAILED = 6;
        public static string JOB_TYPE_STATUS_FAILED_DESC = "FAILED";

        public static int JOB_TYPE_STATUS_CANCELLED= 7;
        public static string JOB_TYPE_STATUS_CANCELLED_DESC = "CANCELLED";

        public static int JOB_TYPE_INSTALLATION = 1;
        public static string JOB_TYPE_INSTALLATION_DESC = "SVC REQ INSTALLATION";

        public static int JOB_TYPE_SERVICING = 6;
        public static string JOB_TYPE_SERVICING_DESC = "SVC REQ SERVICING";

        public static int JOB_TYPE_PULLOUT = 4;
        public static string JOB_TYPE_PULLOUT_DESC = "SVC REQ PULL-OUT";

        public static int JOB_TYPE_REPLACEMENT = 7;
        public static string JOB_TYPE_REPLACEMENT_DESC = "SVC REQ REPLACEMENT";

        public static int JOB_TYPE_REPROGRAMMING = 3;
        public static string JOB_TYPE_REPROGRAMMING_DESC = "SVC REQ REPROGRAMMING";

        public static string JOB_TYPE_DIAGNOSTIC_DESC = "SVC REQ DIAGNOSTIC";
        public static string JOB_TYPE_DISPATCH_DESC = "SVC REQ DISPATCH";
        public static string JOB_TYPE_NEGATIVE_DESC = "SVC REQ NEGATIVE";

        public static int JOB_TYPE_HOLD = 9;
        public static string JOB_TYPE_HOLD_DESC = "SVC REQ HOLD";

        public static int JOB_TYPE_CANCEL = 10;
        public static string JOB_TYPE_CANCEL_DESC = "SVC REQ CANCEL";

        public static int IR_INACTIVE = 0;
        public static int IR_ACTIVE = 1;

        public static int SVC_REQ_OPEN = 0;
        public static int SVC_REQ_CLOSE = 1;

        public static int SVC_REQ_OVERDUE_FALSE = 0;
        public static int SVC_REQ_OVERDUE_TRUE = 1;

        public static string ACTION_MADE_SUCCESS = "SUCCESS";
        public static string ACTION_MADE_NEGATIVE = "NEGATIVE";

        public static int READY_FOR_IMPORT = 1;
        public static string READY_FOR_IMPORT_DESC = "READY FOR IMPORT";

        public static int SERVICE_ALREADY_PROCESSED = 2;
        public static string SERVICE_ALREADY_PROCESSED_DESC = "SERVICE ALREADY PROCESSED";

        public static int IMPORT_SERVICE_NOT_FOUND = 3;
        public static string IMPORT_SERVICE_NOT_FOUND_DESC = "IMPORT SERVICE NOT FOUND";

        public static string IMPORT_ACTION_DESC = "IMPORT";
        public static string IGNORE_ACTION_DESC = "IGNORE";

        public static string REASON_TYPE = "REASON";
        public static string NEGATIVE_TYPE = "NEGATIVE";
        public static string RESOLUTION_TYPE = "RESOLUTION";
        public static string PROBLEM_TYPE = "PROBLEM";

        public static int FSR_REPORT_TYPE = 1;
        public static string FSR_REPORT_TYPE_DESC = "FIELD SERVICE REPORT";

        public static int OVERDUE_REPORT_TYPE = 2;
        public static string OVERDUE_REPORT_TYPE_DESC = "OVERDUE REPORT";

        public static int EMAIL_NOTIF_TYPE = 1;
        public static string EMAIL_NOTIF_TYPE_DESC = "EMAIL NOTIF";

        public static int SMS_NOTIF_TYPE = 2;
        public static string SMS_NOTIF_TYPE_DESC = "SMS NOTIF";

        // tbltype
        public static int CARRIER_TYPE = 1;
        public static int REQUEST_TYPE = 2;
        public static int CONTRACT_TYPE = 3;
        public static int APPS_TYPE = 4;
        public static int ASSET_TYPE = 5;
        public static int SETUP_TYPE = 6;
        public static int RENTAL_TYPE = 7;
        public static int RENTAL_TERM_TYPE = 8;

        public static int SERVICE_NON = 0;
        public static int SERVICE_PRIMARY = 1;

        public static int TIMESHEET_OFF = 0;
        public static int TIMESHEET_ON = 1;

        public static string IMAGE_LOGO = "logo.png";

        public static string IMAGE_ALARM_ON = "alarm_on.png";
        public static string IMAGE_ALARM_OFF = "alarm_off.png";

        public static int REPORT_TYPE_ADMINISTRATIVE= 1;
        public static int REPORT_TYPE_OPERATION = 2;
        public static int REPORT_TYPE_FINANCE = 3;
        public static int REPORT_TYPE_INVENTORY = 4;

        // IR
        public static string IR_HEADER_REQUESTID = "Request ID";
        public static string IR_HEADER_REQUESTDATE = "Request Date";
        public static string IR_HEADER_REQUESTOR = "Requestor";
        public static string IR_HEADER_MID = "Merchant ID (MID)";
        public static string IR_HEADER_TID = "Terminal ID (TID)";
        public static string IR_HEADER_MERCHANT = "Merchant Location/DBA Name";
        public static string IR_HEADER_ADDRESS = "Address";
        public static string IR_HEADER_CITY = "City";
        public static string IR_HEADER_REGION = "Area 1 (Metro Manila / Provincial)";
        public static string IR_HEADER_CONTACT_PERSON = "Contact Person";
        public static string IR_HEADER_CONTACT_NUMBER  = "Contact Number";

        // Terminal / SIM
        public static string INV_HEADER_SN = "SERIAL NO";
        public static string INV_HEADER_TYPE = "TYPE";
        public static string INV_HEADER_MODEL = "MODEL";
        public static string INV_HEADER_BRAND = "BRAND";
        public static string INV_HEADER_DD = "DELIVERY DATE";
        public static string INV_HEADER_RD = "RECEIVED DATE";
        public static string INV_HEADER_LOCATION = "LOCATION";
        public static string INV_HEADER_ASSET_TYPE = "ASSET TYPE";
        public static string INV_HEADER_CARRIER = "CARRIER";


        public static DataSet globalDataSet= null;
        public static DataTable globalDataTable = null;

    }
}
