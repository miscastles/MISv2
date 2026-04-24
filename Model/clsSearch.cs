using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsSearch
    {
        public static bool RecordFound;
        
        // ID
        public static int _SelectType;
        public static string _IRID;
        public static int _IRIDNo;
        public static int _TAIDNo;
        public static int _TAID;
        public static int _ParticularID;
        public static int _ParticularTypeID;
        public static int _TerminalID;
        public static int _MerchantID;
        public static int _ClientID;
        public static int _ServiceProviderID;
        public static int _FEID;
        public static int _ProvinceID;
        public static int _CityID;
        public static int _TerminalStatus;
        public static int _SIMStatus;
        public static int _TerminalTypeID;
        public static int _TerminalModelID;
        public static int _TerminalBrandID;
        public static int _ServiceTypeID;
        public static int _OtherServiceTypeID;
        public static int _ServiceStatus;
        public static int _ReportID;
        public static int _SIMID;
        public static int _FSRStatus;
        public static int _SCNo;
        public static int _ServiceNo;
        public static int _JobType;
        public static int _JobTypeStatus;
        public static int _IRActive;
        public static int _LastInsertedID;

        public static string _APIMethod;
        public static string _StatementType;
        public static string _MaintenanceType;
        public static string _SearchBy;
        public static string _SearchValue;
        public static string _AdvanceSearchValue;
        public static string _SQL;
        public static string _Action;
        public static string _APIData;
        public static string _ServiceTypeCode;
        public static string _MerchantName;
        public static string _ParticularTypeDescription;
        public static string _TID;
        public static string _MID;
        public static string _InvoiceNo;
        public static string _InvoiceNoFrom;
        public static string _InvoiceNoTo;
        public static string _BatchNo;
        public static string _TerminalStatusDescription;
        public static string _TerminalSN;
        public static string _SIMSerialNo;
        public static string _SIMCarrier;
        public static string _SIMStatusDescription;
        public static string _TerminalType;
        public static string _TerminalModel;
        public static string _TerminalBrand;
        public static int _TerminalStatusID;
        public static int _TerminalStatusType;        
        public static string _ParticularName;
        public static string _Address;
        public static string _ContactPerson;
        public static string _TelNo;
        public static string _MobileNo;
        public static string _IRNo;
        public static string _City;
        public static string _Province;
        public static string _ServiceProviderName;

        public static string _IRRequestDate;
        public static string _IRInstallationDate;
        public static int _IRStatus;
        public static string _IRStatusDescription;

        public static string _ClientName;
        public static string _FEName;
        public static string _TypeDescription;
        public static string _ModelDescription;
        public static string _BrandDescription;
        public static string _IRDate;
        public static string _InstallationDate;
        public static string _TADateTime;
        public static string _IRImportDateTime;
        public static string _TAProcessedBy;
        public static string _TAModifiedBy;
        public static string _TAProcessedDateTime;
        public static string _TAModifiedDateTime;
        public static string _TARemarks;
        public static string _TAComments;
        public static string _SIMRemarks;
        public static string _ReceivedDate;
        public static string _DeliveryDate;

        // Update Status
        public static int _StatusID;
        public static string _StatusDescription;
        public static string _StatusCode;

        // ServiceType
        public static string _ServiceTypeDescription;
        public static string _OtherServiceTypeDescription;
        public static string _LastServiceRequest;

        // Report
        public static string _StoredProcedureName;
        public static string _IRDateFrom;
        public static string _IRDateTo;
        public static string _ReportType;

        // Date
        public static string _ReqDateFrom;
        public static string _ReqDateTo;
        public static string _InstDateFrom;
        public static string _InstDateTo;
        public static string _TADateFrom;
        public static string _TADateTo;
        public static string _FSRDateFrom;
        public static string _FSRDateTo;
        public static string _IRImportDateFrom;
        public static string _IRImportDateTo;
        public static string _ServiceDateFrom;
        public static string _ServiceDateTo;
        public static string _CreatedDateFrom;
        public static string _CreatedDateTo;

        // Region
        public static int _RegionID;
        public static int _RegionType;
        public static string _Region;

        // Reason
        public static int _ReasonID;
        public static string _ReasonCode;
        public static string _ReasonDescription;
        public static int _ReasonIsInput;

        public static int _ServiceTypeStatus;
        public static string _ServiceTypeStatusDescription;
        public static string _ServiceTypeStatusSubDescription;

        // List
        public static string _TerminalStatusList;
        public static string _TerminalTypeList;
        public static string _TerminalModelList;
        public static string _TerminalBrandList;
        public static string _FENameList;
        public static string _ClientNameList;
        public static string _SPNameList;
        public static string _RegionList;
        public static string _CarrierList;
        public static string _CityList;
        public static string _ProvinceList;
        public static string _ServiceTypeList;
        public static string _ServiceStatusList;
        public static string _FSRStatusList;
        public static string _ActionMadeList;

        public static int _DockID;
        public static string _DockSN;

        public static string _IRDefaultStatusDescription;
        public static string _TADefaultStatusDescription;
        public static string _FSRDefaultStatusDescription;
        public static string _TerminalDefaultStatusDescription;

        public static string _JobTypeDescription;
        public static string _JobTypeSubDescription;
        public static string _JobTypeStatusDescription;

        public static string _ActionMade;
        public static string _SearchString;
        public static string _ProcessedDateTime;
        public static string _ProcessedBy;
        public static string _ProcessType;

        public static int _CurTerminalSNStatus;
        public static int _CurSIMSNStatus;
        public static int _CurDockSNStatus;
        public static int _RepTerminalSNStatus;
        public static int _RepSIMSNStatus;
        public static int _RepDockSNStatus;

        public static string _CurTerminalSNStatusDescription;
        public static string _CurSIMSNStatusDescription;
        public static string _CurDockSNStatusDescription;

        public static string _RepTerminalSN;
        public static string _RepTerminalSNStatusDescription;
        public static string _RepSIMSN;
        public static string _RepSIMSNStatusDescription;
        public static string _RepDockSN;
        public static string _RepDockSNStatusDescription;
        public static string _RequestNo;

        /* Job Order */
        public static string _JobOrderDate;
        public static string _ServiceRequestID;

        public static string _ServiceStatusDescription;
        public static string _ServiceDateTime;

        /* User */
        public static int _UserID;
        public static string _UserType;
        public static string _LogDateFrom;
        public static string _LogDateTo;
        public static int _LogSessionStatus;

        public static string _IsClose;
        public static string _IsOverDue;

        // DTR
        public static string _Name;
        public static string _Date;
        public static string _Time;
        public static string _IDNo;
        public static string _CheckIn;
        public static string _CheckOut;
        public static string _TCheckHrs;
        public static string _OvertimeIn;
        public static string _OvertimeOut;
        public static string _TOverTimeHrs;
        public static string _FullName;
        public static string _Remarks;
        public static bool _isName;

        public static string _RequestTime;
        public static string _ResponseTime;
        
        public static bool ClassRecordFound
        {
            get { return RecordFound; }
            set { RecordFound = value; }

        }
        public static int ClassSelectType
        {
            get { return _SelectType; }
            set { _SelectType = value; }
        }

        public static string ClassIRID
        {
            get { return _IRID; }
            set { _IRID = value; }

        }
        public static int ClassIRIDNo
        {
            get { return _IRIDNo; }
            set { _IRIDNo = value; }

        }
        public static int ClassTAIDNo
        {
            get { return _TAIDNo; }
            set { _TAIDNo = value; }

        }
        public static int ClassTAID
        {
            get { return _TAID; }
            set { _TAID = value; }

        }
        public static int ClassParticularID
        {
            get { return _ParticularID; }
            set { _ParticularID = value; }

        }
        public static int ClassParticularTypeID
        {
            get { return _ParticularTypeID; }
            set { _ParticularTypeID = value; }

        }
        public static int ClassTerminalID
        {
            get { return _TerminalID; }
            set { _TerminalID = value; }

        }
        public static int ClassMerchantID
        {
            get { return _MerchantID; }
            set { _MerchantID = value; }

        }
        public static int ClassClientID
        {
            get { return _ClientID; }
            set { _ClientID = value; }

        }
        public static int ClassServiceProviderID
        {
            get { return _ServiceProviderID; }
            set { _ServiceProviderID = value; }

        }
        public static int ClassFEID
        {
            get { return _FEID; }
            set { _FEID = value; }

        }
        public static int ClassProvinceID
        {
            get { return _ProvinceID; }
            set { _ProvinceID = value; }

        }
        public static int ClassCityID
        {
            get { return _CityID; }
            set { _CityID = value; }

        }
        public static int ClassTerminalStatus
        {
            get { return _TerminalStatus; }
            set { _TerminalStatus = value; }

        }
        public static int ClassTerminalTypeID
        {
            get { return _TerminalTypeID; }
            set { _TerminalTypeID = value; }

        }
        public static int ClassTerminalBrandID
        {
            get { return _TerminalBrandID; }
            set { _TerminalBrandID = value; }

        }
        public static int ClassTerminalModelID
        {
            get { return _TerminalModelID; }
            set { _TerminalModelID = value; }

        }
        public static int ClassServiceStatus
        {
            get { return _ServiceStatus; }
            set { _ServiceStatus = value; }

        }
        public static string ClassServiceStatusDescription
        {
            get { return _ServiceStatusDescription; }
            set { _ServiceStatusDescription = value; }

        }
        public static int ClassServiceTypeID
        {
            get { return _ServiceTypeID; }
            set { _ServiceTypeID = value; }

        }
        public static int ClassOtherServiceTypeID
        {
            get { return _OtherServiceTypeID; }
            set { _OtherServiceTypeID = value; }

        }
        public static int ClassOtherServiceStatus
        {
            get { return _ServiceStatus; }
            set { _ServiceStatus = value; }

        }
        public static int ClassReportID
        {
            get { return _ReportID; }
            set { _ReportID = value; }
        }
        public static string _ReportDescription;
        public static string ClassReportDescription
        {
            get { return _ReportDescription; }
            set { _ReportDescription = value; }
        }
        public static string ClassAPIMethod
        {
            get { return _APIMethod; }
            set { _APIMethod = value; }

        }

        public static string _APIURL;
        public static string ClassAPIURL
        {
            get { return _APIURL; }
            set { _APIURL = value; }

        }

        public static string _APIPath;
        public static string ClassAPIPath
        {
            get { return _APIPath; }
            set { _APIPath = value; }

        }

        public static string _APIKeys;
        public static string ClassAPIKeys
        {
            get { return _APIKeys; }
            set { _APIKeys = value; }

        }

        public static string _APIAuthUser;
        public static string ClassAPIAuthUser
        {
            get { return _APIAuthUser; }
            set { _APIAuthUser = value; }

        }

        public static string _APIAuthPassword;
        public static string ClassAPIAuthPassword
        {
            get { return _APIAuthPassword; }
            set { _APIAuthPassword = value; }

        }

        public static string _APIContentType;
        public static string ClassAPIContentType
        {
            get { return _APIContentType; }
            set { _APIContentType = value; }

        }

        public static string _APIDashboardURL;
        public static string ClassAPIDashboardURL
        {
            get { return _APIDashboardURL; }
            set { _APIDashboardURL = value; }

        }

        public static string _APIServerIPAddress;
        public static string ClassAPIServerIPAddress
        {
            get { return _APIServerIPAddress; }
            set { _APIServerIPAddress = value; }

        }

        public static string ClassAPIData
        {
            get { return _APIData; }
            set { _APIData = value; }

        }
        public static string ClassStatementType
        {
            get { return _StatementType; }
            set { _StatementType = value; }
        }
        public static string ClassMaintenanceType
        {
            get { return _MaintenanceType; }
            set { _MaintenanceType = value; }
        }
        public static string ClassSearchBy
        {
            get { return _SearchBy; }
            set { _SearchBy = value; }
        }
        public static string ClassSearchValue
        {
            get { return _SearchValue; }
            set { _SearchValue = value; }
        }
        public static string ClassAdvanceSearchValue
        {
            get { return _AdvanceSearchValue; }
            set { _AdvanceSearchValue = value; }
        }
        public static string ClassSQL
        {
            get { return _SQL; }
            set { _SQL = value; }
        }
        public static string ClassAction
        {
            get { return _Action; }
            set { _Action = value; }
        }
        public static string ClassServiceTypeCode
        {
            get { return _ServiceTypeCode; }
            set { _ServiceTypeCode = value; }
        }
        public static string ClassMerchantName
        {
            get { return _MerchantName; }
            set { _MerchantName = value; }
        }
        public static string ClassParticularTypeDescription
        {
            get { return _ParticularTypeDescription; }
            set { _ParticularTypeDescription = value; }
        }
        public static string ClassTID
        {
            get { return _TID; }
            set { _TID = value; }
        }
        public static string ClassMID
        {
            get { return _MID; }
            set { _MID = value; }
        }
        public static string ClassInvoiceNo
        {
            get { return _InvoiceNo; }
            set { _InvoiceNo = value; }
        }

        public static string ClassInvoiceNoFrom
        {
            get { return _InvoiceNoFrom; }
            set { _InvoiceNoFrom = value; }
        }

        public static string ClassInvoiceNoTo
        {
            get { return _InvoiceNoTo; }
            set { _InvoiceNoTo = value; }
        }

        public static string ClassBatchNo
        {
            get { return _BatchNo; }
            set { _BatchNo = value; }
        }
        public static string ClassTerminalStatusDescription
        {
            get { return _TerminalStatusDescription; }
            set { _TerminalStatusDescription = value; }
        }
        public static string ClassTerminalSN
        {
            get { return _TerminalSN; }
            set { _TerminalSN = value; }
        }
        public static string ClassTerminalType
        {
            get { return _TerminalType; }
            set { _TerminalType = value; }
        }
        public static string ClassTerminalModel
        {
            get { return _TerminalModel; }
            set { _TerminalModel = value; }
        }
        public static string ClassTerminalBrand
        {
            get { return _TerminalBrand; }
            set { _TerminalBrand = value; }
        }
        public static int ClassTerminalStatusID
        {
            get { return _TerminalStatusID; }
            set { _TerminalStatusID = value; }
        }
        public static int ClassTerminalStatusType
        {
            get { return _TerminalStatusType; }
            set { _TerminalStatusType = value; }
        }
        public static string ClassParticularName
        {
            get { return _ParticularName; }
            set { _ParticularName = value; }
        }
        public static string ClassParticularAddress
        {
            get { return _Address; }
            set { _Address = value; }
        }
        public static string ClassParticularContactPerson
        {
            get { return _ContactPerson; }
            set { _ContactPerson = value; }
        }
        public static string ClassParticularMobileNo
        {
            get { return _MobileNo; }
            set { _MobileNo = value; }
        }
        public static string ClassParticularTelNo
        {
            get { return _TelNo; }
            set { _TelNo = value; }
        }
        public static string ClassIRNo
        {
            get { return _IRNo; }
            set { _IRNo = value; }
        }
        public static string ClassCity
        {
            get { return _City; }
            set { _City = value; }
        }
        public static string ClassProvince
        {
            get { return _Province; }
            set { _Province = value; }
        }
        public static string ClassServiceProviderName
        {
            get { return _ServiceProviderName; }
            set { _ServiceProviderName = value; }
        }
        public static string ClassIRRequestDate
        {
            get { return _IRRequestDate; }
            set { _IRRequestDate = value; }
        }
        public static string ClassIRInstallationDate
        {
            get { return _IRInstallationDate; }
            set { _IRInstallationDate = value; }
        }
        public static int ClassIRStatus
        {
            get { return _IRStatus; }
            set { _IRStatus = value; }
        }
        public static string ClassIRStatusDescription
        {
            get { return _IRStatusDescription; }
            set { _IRStatusDescription = value; }
        }
        public static string ClassClientName
        {
            get { return _ClientName; }
            set { _ClientName = value; }

        }
        public static string ClassFEName
        {
            get { return _FEName; }
            set { _FEName = value; }

        }
        public static string ClassTypeDescription
        {
            get { return _TypeDescription; }
            set { _TypeDescription = value; }

        }
        public static string ClassModelDescription
        {
            get { return _ModelDescription; }
            set { _ModelDescription = value; }

        }
        public static string ClassBrandDescription
        {
            get { return _BrandDescription; }
            set { _BrandDescription = value; }

        }
        public static string ClassIRDate
        {
            get { return _IRDate; }
            set { _IRDate = value; }

        }
        public static string ClassInstallationDate
        {
            get { return _InstallationDate; }
            set { _InstallationDate = value; }

        }
        public static string ClassTADateTime
        {
            get { return _TADateTime; }
            set { _TADateTime = value; }

        }
        public static string ClassIRImportDateTime
        {
            get { return _IRImportDateTime; }
            set { _IRImportDateTime = value; }

        }
        public static string ClassTAProcessedBy
        {
            get { return _TAProcessedBy; }
            set { _TAProcessedBy = value; }

        }
        public static string ClassTAModifiedBy
        {
            get { return _TAModifiedBy; }
            set { _TAModifiedBy = value; }

        }
        public static string ClassTAProcessedDateTime
        {
            get { return _TAProcessedDateTime; }
            set { _TAProcessedDateTime = value; }

        }
        public static string ClassTAModifiedDateTime
        {
            get { return _TAModifiedDateTime; }
            set { _TAModifiedDateTime = value; }

        }
        public static string ClassTARemarks
        {
            get { return _TARemarks; }
            set { _TARemarks = value; }

        }
        public static string ClassTAComments
        {
            get { return _TAComments; }
            set { _TAComments = value; }

        }
        public static string ClassSIMRemarks
        {
            get { return _SIMRemarks; }
            set { _SIMRemarks = value; }

        }
        public static string ClassReceivedDate
        {
            get { return _ReceivedDate; }
            set { _ReceivedDate = value; }

        }
        public static string ClassDeliveryDate
        {
            get { return _DeliveryDate; }
            set { _DeliveryDate = value; }

        }
        public static int ClassStatus
        {
            get { return _StatusID; }
            set { _StatusID = value; }

        }
        public static string ClassStatusCode
        {
            get { return _StatusCode; }
            set { _StatusCode = value; }

        }

        public static string ClassStatusDescription
        {
            get { return _StatusDescription; }
            set { _StatusDescription = value; }

        }
        public static string ClassServiceTypeDescription
        {
            get { return _ServiceTypeDescription; }
            set { _ServiceTypeDescription = value; }

        }
        public static string ClassOtherServiceTypeDescription
        {
            get { return _OtherServiceTypeDescription; }
            set { _OtherServiceTypeDescription = value; }

        }
        public static string ClassStoredProcedureName
        {
            get { return _StoredProcedureName; }
            set { _StoredProcedureName = value; }
        }

        public static string ClassIRDateFrom
        {
            get { return _IRDateFrom; }
            set { _IRDateFrom = value; }
        }
        public static string ClassIRDateTo
        {
            get { return _IRDateTo; }
            set { _IRDateTo = value; }
        }

        public static int ClassSIMID
        {
            get { return _SIMID; }
            set { _SIMID = value; }

        }
        
        public static int ClassSIMStatus
        {
            get { return _SIMStatus; }
            set { _SIMStatus = value; }
        }
        public static string ClassSIMSerialNo
        {
            get { return _SIMSerialNo; }
            set { _SIMSerialNo = value; }
        }
        public static string ClassSIMCarrier
        {
            get { return _SIMCarrier; }
            set { _SIMCarrier = value; }
        }
        public static string ClassSIMStatusDescription
        {
            get { return _SIMStatusDescription; }
            set { _SIMStatusDescription = value; }
        }

        public static string ClassReqDateFrom
        {
            get { return _ReqDateFrom; }
            set { _ReqDateFrom = value; }
        }
        public static string ClassReqDateTo
        {
            get { return _ReqDateTo; }
            set { _ReqDateTo = value; }
        }

        public static string ClassInstDateFrom
        {
            get { return _InstDateFrom; }
            set { _InstDateFrom = value; }
        }
        public static string ClassInstDateTo
        {
            get { return _InstDateTo; }
            set { _InstDateTo = value; }
        }

        public static string ClassTADateFrom
        {
            get { return _TADateFrom; }
            set { _TADateFrom = value; }
        }
        public static string ClassTADateTo
        {
            get { return _TADateTo; }
            set { _TADateTo = value; }
        }

        public static string ClassFSRDateFrom
        {
            get { return _FSRDateFrom; }
            set { _FSRDateFrom = value; }
        }
        public static string ClassFSRDateTo
        {
            get { return _FSRDateTo; }
            set { _FSRDateTo = value; }
        }
        public static string ClassIRImportDateFrom
        {
            get { return _IRImportDateFrom; }
            set { _IRImportDateFrom = value; }
        }
        public static string ClassIRImportDateTo
        {
            get { return _IRImportDateTo; }
            set { _IRImportDateTo = value; }
        }
        public static string ClassServiceDateFrom
        {
            get { return _ServiceDateFrom; }
            set { _ServiceDateFrom = value; }
        }
        public static string ClassServiceDateTo
        {
            get { return _ServiceDateTo; }
            set { _ServiceDateTo = value; }
        }

        public static string ClassCreatedDateFrom
        {
            get { return _CreatedDateFrom; }
            set { _CreatedDateFrom = value; }
        }
        public static string ClassCreatedDateTo
        {
            get { return _CreatedDateTo; }
            set { _CreatedDateTo = value; }
        }

        public static int ClassRegionID
        {
            get { return _RegionID; }
            set { _RegionID = value; }
        }
        public static int ClassRegionType
        {
            get { return _RegionType; }
            set { _RegionType = value; }
        }
        public static string ClassRegion
        {
            get { return _Region; }
            set { _Region = value; }
        }

        public static int ClassServiceTypeStatus
        {
            get { return _ServiceTypeStatus; }
            set { _ServiceTypeStatus = value; }
        }

        public static string ClassServiceTypeStatusDescription
        {
            get { return _ServiceTypeStatusDescription; }
            set { _ServiceTypeStatusDescription = value; }
        }
        public static string ClassServiceTypeStatusSubDescription
        {
            get { return _ServiceTypeStatusSubDescription; }
            set { _ServiceTypeStatusSubDescription = value; }
        }
        public static int ClassReasonID
        {
            get { return _ReasonID; }
            set { _ReasonID = value; }
        }
        public static string ClassReasonCode
        {
            get { return _ReasonCode; }
            set { _ReasonCode = value; }
        }
        public static string ClassReasonDescription
        {
            get { return _ReasonDescription; }
            set { _ReasonDescription = value; }
        }
        public static string ClassTerminalStatusList
        {
            get { return _TerminalStatusList; }
            set { _TerminalStatusList = value; }
        }
        public static string ClassTerminalTypeList
        {
            get { return _TerminalTypeList; }
            set { _TerminalTypeList = value; }
        }
        public static string ClassTerminalModelList
        {
            get { return _TerminalModelList; }
            set { _TerminalModelList = value; }
        }
        public static string ClassTerminalBrandList
        {
            get { return _TerminalBrandList; }
            set { _TerminalBrandList = value; }
        }
        public static string ClassFENameList
        {
            get { return _FENameList; }
            set { _FENameList = value; }
        }
        public static string ClassClientNameList
        {
            get { return _ClientNameList; }
            set { _ClientNameList = value; }
        }
        public static string ClassSPNameList
        {
            get { return _SPNameList; }
            set { _SPNameList = value; }
        }
        public static string ClassRegionList
        {
            get { return _RegionList; }
            set { _RegionList = value; }
        }
        public static string ClassCarrierList
        {
            get { return _CarrierList; }
            set { _CarrierList = value; }
        }
        public static string ClassCityList
        {
            get { return _CityList; }
            set { _CityList = value; }
        }
        public static string ClassProvinceList
        {
            get { return _ProvinceList; }
            set { _ProvinceList = value; }
        }
        public static int ClassFSRStatus
        {
            get { return _FSRStatus; }
            set { _FSRStatus = value; }

        }

        public static int ClassSCNo
        {
            get { return _SCNo; }
            set { _SCNo = value; }
        }
        public static int ClassServiceNo
        {
            get { return _ServiceNo; }
            set { _ServiceNo = value; }
        }

        public static string ClassJobOrderDate
        {
            get { return _JobOrderDate; }
            set { _JobOrderDate = value; }
        }

        public static string ClassServiceRequestID
        {
            get { return _ServiceRequestID; }
            set { _ServiceRequestID = value; }
        }

        public static string ClassServiceTypeList
        {
            get { return _ServiceTypeList; }
            set { _ServiceTypeList = value; }
        }
        public static string ClassServiceStatusList
        {
            get { return _ServiceStatusList; }
            set { _ServiceStatusList = value; }
        }

        public static int ClassDockID
        {
            get { return _DockID; }
            set { _DockID = value; }
        }

        public static string ClassDockSN
        {
            get { return _DockSN; }
            set { _DockSN = value; }
        }

        public static string ClassDefaultIRStatusDescription
        {
            get { return _IRDefaultStatusDescription; }
            set { _IRDefaultStatusDescription = value; }
        }
        public static string ClassDefaultTAStatusDescription
        {
            get { return _TADefaultStatusDescription; }
            set { _TADefaultStatusDescription = value; }
        }
        public static string ClassDefaultFSRStatusDescription
        {
            get { return _FSRDefaultStatusDescription; }
            set { _FSRDefaultStatusDescription = value; }
        }
        public static string ClassDefaultTerminalStatusDescription
        {
            get { return _TerminalDefaultStatusDescription; }
            set { _TerminalDefaultStatusDescription = value; }
        }
        public static string ClassFSRStatusList
        {
            get { return _FSRStatusList; }
            set { _FSRStatusList = value; }
        }
        public static string ClassLastServiceRequest
        {
            get { return _LastServiceRequest; }
            set { _LastServiceRequest = value; }
        }
        public static int ClassJobType
        {
            get { return _JobType; }
            set { _JobType = value; }

        }
        public static string ClassJobTypeDescription
        {
            get { return _JobTypeDescription; }
            set { _JobTypeDescription = value; }

        }
        public static string ClassJobTypeSubDescription
        {
            get { return _JobTypeSubDescription; }
            set { _JobTypeSubDescription = value; }

        }
        public static int ClassJobTypeStatus
        {
            get { return _JobTypeStatus; }
            set { _JobTypeStatus = value; }

        }
        public static string ClassJobTypeStatusDescription
        {
            get { return _JobTypeStatusDescription; }
            set { _JobTypeStatusDescription = value; }

        }
        public static string ClassActionMadeList
        {
            get { return _ActionMadeList; }
            set { _ActionMadeList = value; }
        }
        public static string ClassActionMade
        {
            get { return _ActionMade; }
            set { _ActionMade = value; }
        }
        public static string ClassSearchString
        {
            get { return _SearchString; }
            set { _SearchString = value; }
        }
        public static int ClassIRActive
        {
            get { return _IRActive; }
            set { _IRActive = value; }

        }
        public static int ClassLastInsertedID
        {
            get { return _LastInsertedID; }
            set { _LastInsertedID = value; }

        }
        public static string ClassProcessedDateTime
        {
            get { return _ProcessedDateTime; }
            set { _ProcessedDateTime = value; }
        }
        public static string ClassProcessedBy
        {
            get { return _ProcessedBy; }
            set { _ProcessedBy = value; }
        }
        public static string ClassProcessType
        {
            get { return _ProcessType; }
            set { _ProcessType = value; }
        }
        public static int ClassCurTerminalSNStatus
        {
            get { return _CurTerminalSNStatus; }
            set { _CurTerminalSNStatus = value; }
        }
        public static int ClassCurSIMSNStatus
        {
            get { return _CurSIMSNStatus; }
            set { _CurSIMSNStatus = value; }
        }
        public static int ClassCurDockSNStatus
        {
            get { return _CurDockSNStatus; }
            set { _CurDockSNStatus = value; }
        }
        public static int ClassRepTerminalSNStatus
        {
            get { return _RepTerminalSNStatus; }
            set { _RepTerminalSNStatus = value; }
        }
        public static int ClassRepSIMSNStatus
        {
            get { return _RepSIMSNStatus; }
            set { _RepSIMSNStatus = value; }
        }
        public static int ClassRepDockSNStatus
        {
            get { return _RepDockSNStatus; }
            set { _RepDockSNStatus = value; }
        }
        public static string ClassCurTerminalSNStatusDescription
        {
            get { return _CurTerminalSNStatusDescription; }
            set { _CurTerminalSNStatusDescription = value; }
        }
        public static string ClassCurSIMSNStatusDescription
        {
            get { return _CurSIMSNStatusDescription; }
            set { _CurSIMSNStatusDescription = value; }
        }
        public static string ClassCurDockSNStatusDescription
        {
            get { return _CurDockSNStatusDescription; }
            set { _CurDockSNStatusDescription = value; }
        }
        public static string ClassRepTerminalSNStatusDescription
        {
            get { return _RepTerminalSNStatusDescription; }
            set { _RepTerminalSNStatusDescription = value; }
        }
        public static string ClassRepSIMSNStatusDescription
        {
            get { return _RepSIMSNStatusDescription; }
            set { _RepSIMSNStatusDescription = value; }
        }
        public static string ClassRepDockSNStatusDescription
        {
            get { return _RepDockSNStatusDescription; }
            set { _RepDockSNStatusDescription = value; }
        }
        public static string ClassRequestNo
        {
            get { return _RequestNo; }
            set { _RequestNo = value; }
        }
        public static string ClassServiceDateTime
        {
            get { return _ServiceDateTime; }
            set { _ServiceDateTime = value; }

        }
        public static int ClassUserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }
        public static string ClassUserType
        {
            get { return _UserType; }
            set { _UserType = value; }

        }
        public static string ClassLogDateFrom
        {
            get { return _LogDateFrom; }
            set { _LogDateFrom = value; }

        }
        public static string ClassLogDateTo
        {
            get { return _LogDateTo; }
            set { _LogDateTo = value; }

        }
        public static int ClassLogSessionStatus
        {
            get { return _LogSessionStatus; }
            set { _LogSessionStatus = value; }
        }

        public static int _ExpensesID;
        public static int ClassExpensesID
        {
            get { return _ExpensesID; }
            set { _ExpensesID = value; }
        }

        public static string _ExpensesDescription;
        public static string ClassExpensesDescription
        {
            get { return _ExpensesDescription; }
            set { _ExpensesDescription = value; }
        }

        public static double _ExpensesAmount;
        public static double ClassExpensesAmount
        {
            get { return _ExpensesAmount; }
            set { _ExpensesAmount = value; }
        }

        public static double _TExpensesAmount;
        public static double ClassTExpensesAmount
        {
            get { return _TExpensesAmount; }
            set { _TExpensesAmount = value; }
        }

        public static string _CurrentDateTime;
        public static string ClassCurrentDateTime
        {
            get { return _CurrentDateTime; }
            set { _CurrentDateTime = value; }
        }

        public static string _CurrentDate;
        public static string ClassCurrentDate
        {
            get { return _CurrentDate; }
            set { _CurrentDate = value; }
        }
        public static string _CurrentTime;
        public static string ClassCurrentTime
        {
            get { return _CurrentTime; }
            set { _CurrentTime = value; }
        }

        public static string _TCount;
        public static string ClassTCount
        {
            get { return _TCount; }
            set { _TCount = value; }
        }

        public static string _TAmount;
        public static string ClassTAmount
        {
            get { return _TAmount; }
            set { _TAmount = value; }
        }
        public static string _TRecurring;
        public static string ClassTRecurring
        {
            get { return _TRecurring; }
            set { _TRecurring = value; }
        }
        public static string _DateFrom;
        public static string ClassDateFrom
        {
            get { return _DateFrom; }
            set { _DateFrom = value; }
        }
        public static string _DateTo;
        public static string ClassDateTo
        {
            get { return _DateTo; }
            set { _DateTo = value; }
        }
        public static string _DateFromTo;
        public static string ClassDateFromTo
        {
            get { return _DateFromTo; }
            set { _DateFromTo = value; }
        }
        public static string _SerialNoList;
        public static string ClassSerialNoList
        {
            get { return _SerialNoList; }
            set { _SerialNoList = value; }
        }
        public static string ClassIsClose
        {
            get { return _IsClose; }
            set { _IsClose = value; }

        }
        public static string ClassIsOverDue
        {
            get { return _IsOverDue; }
            set { _IsOverDue = value; }

        }
        public static int _FSRNo;
        public static int ClassFSRNo
        {
            get { return _FSRNo; }
            set { _FSRNo = value; }
        }

        public static int _FSRID;
        public static int ClassFSRID
        {
            get { return _FSRID; }
            set { _FSRID = value; }
        }

        public static string _JobTypeDescriptionList;
        public static string ClassJobTypeDescriptionList
        {
            get { return _JobTypeDescriptionList; }
            set { _JobTypeDescriptionList = value; }
        }

        public static string _ParticularUserKey;
        public static string ClassParticularUserKey
        {
            get { return _ParticularUserKey; }
            set { _ParticularUserKey = value; }
        }

        public static int _CurrentPage;
        public static int ClassCurrentPage
        {
            get { return _CurrentPage; }
            set { _CurrentPage = value; }
        }

        public static int _TotalPage;
        public static int ClassTotalPage
        {
            get { return _TotalPage; }
            set { _TotalPage = value; }
        }
        public static string _HoldAdvanceSearchValue;
        public static string ClassHoldAdvanceSearchValue
        {
            get { return _HoldAdvanceSearchValue; }
            set { _HoldAdvanceSearchValue = value; }
        }
        public static string _JobTypeStatusDescriptionList;
        public static string ClassJobTypeStatusDescriptionList
        {
            get { return _JobTypeStatusDescriptionList; }
            set { _JobTypeStatusDescriptionList = value; }
        }

        public static string _JobTypeList;
        public static string ClassJobTypeList
        {
            get { return _JobTypeList; }
            set { _JobTypeList = value; }
        }

        public static string _NoOfDayPending;
        public static string ClassNoOfDayPending
        {
            get { return _NoOfDayPending; }
            set { _NoOfDayPending = value; }
        }

        public static string _ReasonType;
        public static string ClassReasonType
        {
            get { return _ReasonType; }
            set { _ReasonType = value; }
        }

        public static bool isBillable;
        public static bool ClassBillable
        {
            get { return isBillable; }
            set { isBillable = value; }

        }

        public static bool isMDRBreakdown;
        public static bool ClassMDRBreakdown
        {
            get { return isMDRBreakdown; }
            set { isMDRBreakdown = value; }

        }

        // DTR
        public static string ClassIDNo
        {
            get { return _IDNo; }
            set { _IDNo = value; }
        }
        public static string ClassName
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public static string ClassDate
        {
            get { return _Date; }
            set { _Date = value; }
        }

        public static string ClassTime
        {
            get { return _Time; }
            set { _Time = value; }
        }
        public static string ClassCheckIn
        {
            get { return _CheckIn; }
            set { _CheckIn = value; }
        }
        public static string ClassCheckOut
        {
            get { return _CheckOut; }
            set { _CheckOut = value; }
        }
        public static string ClassTCheckHrs
        {
            get { return _TCheckHrs; }
            set { _TCheckHrs = value; }
        }
        public static string ClassOvertimeIn
        {
            get { return _OvertimeIn; }
            set { _OvertimeIn = value; }
        }
        public static string ClassOvertimeOut
        {
            get { return _OvertimeOut; }
            set { _OvertimeOut = value; }
        }
        public static string ClassTOverTimeHrs
        {
            get { return _TOverTimeHrs; }
            set { _TOverTimeHrs = value; }
        }
        public static string ClassFullName
        {
            get { return _FullName; }
            set { _FullName = value; }
        }
        public static string ClassRemarks
        {
            get { return _Remarks; }
            set { _Remarks = value; }
        }
        public static bool ClassIsName
        {
            get { return _isName; }
            set { _isName = value; }
        }
        public static string ClassRequestTime
        {
            get { return _RequestTime; }
            set { _RequestTime = value; }
        }
        public static string ClassResponseTime
        {
            get { return _ResponseTime; }
            set { _ResponseTime = value; }
        }
        public static string ClassReportType
        {
            get { return _ReportType; }
            set { _ReportType = value; }
        }
        public static string _FSRDate;
        public static string ClassFSRDate
        {
            get { return _FSRDate; }
            set { _FSRDate = value; }
        }

        public static string _ServiceDate;
        public static string ClassServiceDate
        {
            get { return _ServiceDate; }
            set { _ServiceDate = value; }
        }

        public static string _ResponseFileName;
        public static string ClassResponseFileName
        {
            get { return _ResponseFileName; }
            set { _ResponseFileName = value; }
        }

        public static bool _isWriteResponse;
        public static bool ClassisWriteResponse
        {
            get { return _isWriteResponse; }
            set { _isWriteResponse = value; }

        }

        public static int _RepTerminalID;
        public static int ClassRepTerminalID
        {
            get { return _RepTerminalID; }
            set { _RepTerminalID = value; }
        }
        public static string ClassRepTerminalSN
        {
            get { return _RepTerminalSN; }
            set { _RepTerminalSN = value; }
        }

        public static int _RepSIMID;
        public static int ClassRepSIMID
        {
            get { return _RepSIMID; }
            set { _RepSIMID = value; }
        }
        public static string ClassRepSIMSN
        {
            get { return _RepSIMSN; }
            set { _RepSIMSN = value; }
        }
        public static string ClassRepDockSN
        {
            get { return _RepDockSN; }
            set { _RepDockSN = value; }
        }

        public static int _FIleOutID;
        public static int ClassOutFileID
        {
            get { return _FIleOutID; }
            set { _FIleOutID = value; }
        }

        public static string _FIleOutDescription;
        public static string ClassOutFileDescription
        {
            get { return _FIleOutDescription; }
            set { _FIleOutDescription = value; }
        }

        public static string _FIleOutDescription2;
        public static string ClassOutFileDescription2
        {
            get { return _FIleOutDescription2; }
            set { _FIleOutDescription2 = value; }
        }

        public static string _FIleOutCode;
        public static string ClassOutFileCode
        {
            get { return _FIleOutCode; }
            set { _FIleOutCode = value; }
        }

        public static string _FIleOutServiceStatus;
        public static string ClassOutFileServiceStatus
        {
            get { return _FIleOutServiceStatus; }
            set { _FIleOutServiceStatus = value; }
        }

        public static string _FIleOutServiceStatusDescription;
        public static string ClassOutFileServiceStatusDescription
        {
            get { return _FIleOutServiceStatusDescription; }
            set { _FIleOutServiceStatusDescription = value; }
        }

        public static string _FIleOutJobTypeDescription;
        public static string ClassOutFileJobTypeDescription
        {
            get { return _FIleOutJobTypeDescription; }
            set { _FIleOutJobTypeDescription = value; }
        }

        public static string _FIleOutTerminalStatusID;
        public static string ClassOutFileTerminalStatusID
        {
            get { return _FIleOutTerminalStatusID; }
            set { _FIleOutTerminalStatusID = value; }
        }

        public static string _FIleOutTerminalStatusType;
        public static string ClassOutFileTerminalStatusType
        {
            get { return _FIleOutTerminalStatusType; }
            set { _FIleOutTerminalStatusType = value; }
        }

        public static string _FIleOutTerminalStatus;
        public static string ClassOutFileTerminalStatus
        {
            get { return _FIleOutTerminalStatus; }
            set { _FIleOutTerminalStatus = value; }
        }

        // Department
        public static int _DepartmentID;
        public static int ClassDepartmentID
        {
            get { return _DepartmentID; }
            set { _DepartmentID = value; }
        }

        // Department
        public static string _Department;
        public static string ClassDepartment
        {
            get { return _Department; }
            set { _Department = value; }
        }

        // Position
        public static int _PositionID;
        public static int ClassPositionID
        {
            get { return _PositionID; }
            set { _PositionID = value; }
        }

        // Position
        public static string _Position;
        public static string ClassPosition
        {
            get { return _Position; }
            set { _Position = value; }
        }


        // LeaveType
        public static int _LeaveTypeID;
        public static int ClassLeaveTypeID
        {
            get { return _LeaveTypeID; }
            set { _LeaveTypeID = value; }
        }

        // EmploymentStatus
        public static string _EmploymentStatus;
        public static string ClassEmploymentStatus
        {
            get { return _EmploymentStatus; }
            set { _EmploymentStatus = value; }
        }

        public static string _AppVersion;
        public static string ClassAppVersion
        {
            get { return _AppVersion; }
            set { _AppVersion = value; }
        }

        public static string _AppCRC;
        public static string ClassAppCRC
        {
            get { return _AppCRC; }
            set { _AppCRC = value; }
        }

        // MDR A
        public static string _MDR0;
        public static string ClassMDR0
        {
            get { return _MDR0; }
            set { _MDR0 = value; }
        }

        public static string _MDR1;
        public static string ClassMDR1
        {
            get { return _MDR1; }
            set { _MDR1 = value; }
        }

        public static string _MDR2;
        public static string ClassMDR2
        {
            get { return _MDR2; }
            set { _MDR2 = value; }
        }

        public static string _MDR3;
        public static string ClassMDR3
        {
            get { return _MDR3; }
            set { _MDR3 = value; }
        }

        public static string _MDR4;
        public static string ClassMDR4
        {
            get { return _MDR4; }
            set { _MDR4 = value; }
        }

        public static string _MDR5;
        public static string ClassMDR5
        {
            get { return _MDR5; }
            set { _MDR5 = value; }
        }

        // MDR B
        public static string _MDR0_B;
        public static string ClassMDR0_B
        {
            get { return _MDR0_B; }
            set { _MDR0_B = value; }
        }

        public static string _MDR1_B;
        public static string ClassMDR1_B
        {
            get { return _MDR1_B; }
            set { _MDR1_B = value; }
        }

        public static string _MDR2_B;
        public static string ClassMDR2_B
        {
            get { return _MDR2_B; }
            set { _MDR2_B = value; }
        }

        public static string _MDR3_B;
        public static string ClassMDR3_B
        {
            get { return _MDR3_B; }
            set { _MDR3_B = value; }
        }

        public static string _MDR4_B;
        public static string ClassMDR4_B
        {
            get { return _MDR4_B; }
            set { _MDR4_B = value; }
        }

        public static string _MDR5_B;
        public static string ClassMDR5_B
        {
            get { return _MDR5_B; }
            set { _MDR5_B = value; }
        }
        public static string _PrimaryNum;
        public static string ClassPrimaryNum
        {
            get { return _PrimaryNum; }
            set { _PrimaryNum = value; }
        }

        public static string _SecondaryNum;
        public static string ClassSecondaryNum
        {
            get { return _SecondaryNum; }
            set { _SecondaryNum = value; }
        }

        public static string _MobileTerminalID;
        public static string ClassMobileTerminalID
        {
            get { return _MobileTerminalID; }
            set { _MobileTerminalID = value; }
        }

        public static string _MobileTerminalName;
        public static string ClassMobileTerminalName
        {
            get { return _MobileTerminalName; }
            set { _MobileTerminalName = value; }
        }

        // WorkType
        public static int _WorkTypeID;
        public static int ClassWorkTypeID
        {
            get { return _WorkTypeID; }
            set { _WorkTypeID = value; }
        }

        public static int _MissingTimeSheet;
        public static int ClassMissingTimeSheet
        {
            get { return _MissingTimeSheet; }
            set { _MissingTimeSheet = value; }
        }

        // Work Arrangement
        public static int _WorkArrangementID;
        public static int ClassWorkArrangementID
        {
            get { return _WorkArrangementID; }
            set { _WorkArrangementID = value; }
        }

        // Leave Movement
        public static int _LeaveNo;
        public static int ClassLeaveNo
        {
            get { return _LeaveNo; }
            set { _LeaveNo = value; }
        }

        public static string _Code;
        public static string ClassCode
        {
            get { return _Code; }
            set { _Code = value; }
        }

        public static string _Description;
        public static string ClassDescription
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public static string _Duration;
        public static string ClassDuration
        {
            get { return _Duration; }
            set { _Duration = value; }
        }

        public static string _DateType;
        public static string ClassDateType
        {
            get { return _DateType; }
            set { _DateType = value; }
        }

        // TimeSheet Terminal
        public static int _TSID;
        public static int ClassTSID
        {
            get { return _TSID; }
            set { _TSID = value; }
        }

        public static string _TSTerminalID;
        public static string ClassTSTerminalID
        {
            get { return _TSTerminalID; }
            set { _TSTerminalID = value; }
        }

        public static string _TSTerminalName;
        public static string ClassTSTerminalName
        {
            get { return _TSTerminalName; }
            set { _TSTerminalName = value; }
        }

        // Country
        public static int _CountryID;
        public static int ClassCountryID
        {
            get { return _CountryID; }
            set { _CountryID = value; }
        }

        public static string _Country;
        public static string ClassCountry
        {
            get { return _Country; }
            set { _Country = value; }
        }

        public static string _MDRType;
        public static string ClassMDRType
        {
            get { return _MDRType; }
            set { _MDRType = value; }
        }

        public static string _outParamValue;
        public static string ClassOutParamValue
        {
            get { return _outParamValue; }
            set { _outParamValue = value; }
        }

        public static string _Email;
        public static string ClassEmail
        {
            get { return _Email; }
            set { _Email = value; }
        }

        public static int _CurrentUserID;
        public static int ClassCurrentUserID
        {
            get { return _CurrentUserID; }
            set { _CurrentUserID = value; }
        }

        public static int _LastServiceNp;
        public static int ClassLastServiceNo
        {
            get { return _LastServiceNp; }
            set { _LastServiceNp = value; }
        }

        public static int _LocationID;
        public static int ClassLocationID
        {
            get { return _LocationID; }
            set { _LocationID = value; }
        }

        public static string _Location;
        public static string ClassLocation
        {
            get { return _Location; }
            set { _Location = value; }
        }

        public static string _AssetType;
        public static string ClassAssetType
        {
            get { return _AssetType; }
            set { _AssetType = value; }
        }

        public static string _ServiceCount;
        public static string ClassServiceCount
        {
            get { return _ServiceCount; }
            set { _ServiceCount = value; }
        }

        public static int _ID;
        public static int ClassID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public static int _OperationID;
        public static int ClassOperation
        {
            get { return _OperationID; }
            set { _OperationID = value; }
        }

        public static string _OperationDescription;
        public static string ClassOperationDescription
        {
            get { return _OperationDescription; }
            set { _OperationDescription = value; }

        }

        public static int _IsPullOut;
        public static int ClassIsPullOut
        {
            get { return _IsPullOut; }
            set { _IsPullOut = value; }
        }

        public static int _IsReleased;
        public static int ClassIsReleased
        {
            get { return _IsReleased; }
            set { _IsReleased = value; }
        }

        public static string _IncludeBillable;
        public static string ClassIncludeBillable
        {
            get { return _IncludeBillable; }
            set { _IncludeBillable = value; }
        }

        public static bool isExportToPDF;
        public static bool ClassIsExportToPDF
        {
            get { return isExportToPDF; }
            set { isExportToPDF = value; }

        }

        // WorkType
        public static int _TypeID;
        public static int ClassTypeID
        {
            get { return _TypeID; }
            set { _TypeID = value; }
        }

        public static double _RentalFee;
        public static double ClassRentalFee
        {
            get { return _RentalFee; }
            set { _RentalFee = value; }
        }

        // POS Rental / Invoice Master
        public static int _InvoiceID;
        public static int ClassInvoiceID
        {
            get { return _InvoiceID; }
            set { _InvoiceID = value; }
        }

        static int _RentalFeeID;
        public static int ClassRentalFeeID
        {
            get { return _RentalFeeID; }
            set { _RentalFeeID = value; }
        }

        public static int _LocationIDFrom;
        public static int ClassLocationIDFrom
        {
            get { return _LocationIDFrom; }
            set { _LocationIDFrom = value; }
        }

        public static int _LocationIDTo;
        public static int ClassLocationIDTo
        {
            get { return _LocationIDTo; }
            set { _LocationIDTo = value; }
        }

        // Import Count
        public static int _SuccessCount;
        public static int ClassSuccessCount
        {
            get { return _SuccessCount; }
            set { _SuccessCount = value; }
        }

        public static int _FailedCount;
        public static int ClassFailedCount
        {
            get { return _FailedCount; }
            set { _FailedCount = value; }
        }

        public static int _CarrierID;
        public static int ClassCarrierID
        {
            get { return _CarrierID; }
            set { _CarrierID = value; }
        }

        // Terminal/Mobile
        public static int _MobileID;
        public static int ClassMobileID
        {
            get { return _MobileID; }
            set { _MobileID = value; }
        }

        public static int _IsMobile;
        public static int ClassIsMobile
        {
            get { return _IsMobile; }
            set { _IsMobile = value; }
        }

        public static string _MobileAssignedTo;
        public static string ClassMobileAssignedTo
        {
            get { return _MobileAssignedTo; }
            set { _MobileAssignedTo = value; }
        }

        public static string _RowSelected;
        public static string ClassRowSelected
        {
            get { return _RowSelected; }
            set { _RowSelected = value; }
        }

        public static string _ServiceTypeDesc;
        public static string ClassServiceTypeDesc
        {
            get { return _ServiceTypeDesc; }
            set { _ServiceTypeDesc = value; }

        }

        public static string _ServiceStatusDesc;
        public static string ClassServiceStatusDesc
        {
            get { return _ServiceStatusDesc; }
            set { _ServiceStatusDesc = value; }

        }

        public static string _ServiceResultDesc;
        public static string ClassServiceResultDesc
        {
            get { return _ServiceResultDesc; }
            set { _ServiceResultDesc = value; }

        }

        public static int _DispatcherID;
        public static int ClassDispatcherID
        {
            get { return _DispatcherID; }
            set { _DispatcherID = value; }

        }

        public static int _CurrentParticularID;
        public static int ClassCurrentParticularID
        {
            get { return _CurrentParticularID; }
            set { _CurrentParticularID = value; }
        }

        public static string _DispatcherName;
        public static string ClassDispatcherName
        {
            get { return _DispatcherName; }
            set { _DispatcherName = value; }

        }

        public static string _CurrentParticularName;
        public static string ClassCurrentParticularName
        {
            get { return _CurrentParticularName; }
            set { _CurrentParticularName = value; }

        }

        public static string _CurrentUserType;
        public static string ClassCurrentUserType
        {
            get { return _CurrentUserType; }
            set { _CurrentUserType = value; }

        }

        public static string _FSRMode;
        public static string ClassFSRMode
        {
            get { return _FSRMode; }
            set { _FSRMode = value; }

        }

        // TAT
        public static string _SLA;
        public static string ClassSLA
        {
            get { return _SLA; }
            set { _SLA = value; }

        }

        public static string _NetworkDays;
        public static string ClassNetworkDays
        {
            get { return _NetworkDays; }
            set { _NetworkDays = value; }

        }

        public static string _DaysOverDue;
        public static string ClassDaysOverDue
        {
            get { return _DaysOverDue; }
            set { _DaysOverDue = value; }

        }

        public static string _TATStatus;
        public static string ClassTATStatus
        {
            get { return _TATStatus; }
            set { _TATStatus = value; }

        }

        public static int _ItemID;
        public static int ClassItemID
        {
            get { return _ItemID; }
            set { _ItemID = value; }
        }

        public static bool _IsDispatch;
        public static bool ClassIsDispatch
        {
            get { return _IsDispatch; }
            set { _IsDispatch = value; }
        }

        public static string _RequestID;
        public static string ClassRequestID
        {
            get { return _RequestID; }
            set { _RequestID = value; }

        }

        public static string _Components;
        public static string ClassComponents
        {
            get { return _Components; }
            set { _Components = value; }

        }

        public static string _RepComponents;
        public static string ClassRepComponents
        {
            get { return _RepComponents; }
            set { _RepComponents = value; }

        }
        public static int _GroupType;
        public static int ClassGroupType
        {
            get { return _GroupType; }
            set { _GroupType = value; }

        }

        public static int _BillingTypeID;
        public static int ClassBillingTypeID
        {
            get { return _BillingTypeID; }
            set { _BillingTypeID = value; }

        }

        public static int _SourceID;
        public static int ClassSourceID
        {
            get { return _SourceID; }
            set { _SourceID = value; }

        }

        public static int _CategoryID;
        public static int ClassCategoryID
        {
            get { return _CategoryID; }
            set { _CategoryID = value; }

        }

        public static int _SubCategoryID;
        public static int ClassSubCategoryID
        {
            get { return _SubCategoryID; }
            set { _SubCategoryID = value; }

        }

        public static int _MSPStatusID;
        public static int ClassMSPStatusID
        {
            get { return _MSPStatusID; }
            set { _MSPStatusID = value; }

        }

        public static int _SubmitID;
        public static int ClassSubmitID
        {
            get { return _SubmitID; }
            set { _SubmitID = value; }

        }

        public static int _CheckedID;
        public static int ClassCheckedID
        {
            get { return _CheckedID; }
            set { _CheckedID = value; }

        }

        public static int _MSPNo;
        public static int ClassMSPNo
        {
            get { return _MSPNo; }
            set { _MSPNo = value; }

        }

        public static string _ReferenceNo;
        public static string ClassReferenceNo
        {
            get { return _ReferenceNo; }
            set { _ReferenceNo = value; }

        }
        public static string _Requestor;
        public static string ClassRequestor
        {
            get { return _Requestor; }
            set { _Requestor = value; }

        }
        public static int _IsExcludePending;
        public static int ClassIsExcludePending
        {
            get { return _IsExcludePending; }
            set { _IsExcludePending = value; }
        }

        public static bool _isAppVersion;
        public static bool ClassisAppVersion
        {
            get { return _isAppVersion; }
            set { _isAppVersion = value; }
        }
        public static string _Dependency;
        public static string ClassDependency
        {
            get { return _Dependency; }
            set { _Dependency = value; }

        }
        public static string _StatusReason;
        public static string ClassStatusReason
        {
            get { return _StatusReason; }
            set { _StatusReason = value; }

        }
        public static int AssistNo { get; set; }
        public static int ProblemNo { get; set; }

        public static int _HoldServiceStatus;
        public static int ClassHoldServiceStatus
        {
            get { return _HoldServiceStatus; }
            set { _HoldServiceStatus = value; }

        }

        public static string _HoldServiceStatusDescription;
        public static string ClassHoldServiceStatusDescription
        {
            get { return _HoldServiceStatusDescription; }
            set { _HoldServiceStatusDescription = value; }

        }

        public static string _RawDataInfo;
        public static string ClassRawDataInfo
        {
            get { return _RawDataInfo; }
            set { _RawDataInfo = value; }

        }
        public static string _ProfileDataInfo;
        public static string ClassProfileDataInfo
        {
            get { return _ProfileDataInfo; }
            set { _ProfileDataInfo = value; }

        }
        public static int _RequestTypeID;
        public static int ClassRequestTypeID
        {
            get { return _RequestTypeID; }
            set { _RequestTypeID = value; }
        }

        public static int _IncludeSummaryTab;
        public static int ClassIncludeSummaryTab
        {
            get { return _IncludeSummaryTab; }
            set { _IncludeSummaryTab = value; }
        }
        public static int _IncludeDetailTab;
        public static int ClassIncludeDetailTab
        {
            get { return _IncludeDetailTab; }
            set { _IncludeDetailTab = value; }
        }
        public static string _DetailDateFrom;
        public static string ClassDetailDateFrom
        {
            get { return _DetailDateFrom; }
            set { _DetailDateFrom = value; }
        }
        public static string _DetailDateTo;
        public static string ClassDetailDateTo
        {
            get { return _DetailDateTo; }
            set { _DetailDateTo = value; }
        }

        // Bank variable
        public static int _BankID;
        public static int ClassBankID
        {
            get { return _BankID; }
            set { _BankID = value; }
        }

        public static int _IsBillType;
        public static int ClassIsBillType
        {
            get { return _IsBillType; }
            set { _IsBillType = value; }
        }

        public static string _BankCode;
        public static string ClassBankCode
        {
            get { return _BankCode; }
            set { _BankCode = value; }
        }

        public static string _BankName;
        public static string ClassBankName
        {
            get { return _BankName; }
            set { _BankName = value; }
        }

        public static string _BankDisplayName;
        public static string ClassBankDisplayName
        {
            get { return _BankDisplayName; }
            set { _BankDisplayName = value; }
        }

        public static string _BankMainColor;
        public static string ClassBankMainColor
        {
            get { return _BankMainColor; }
            set { _BankMainColor = value; }
        }

        public static string _BankPrimaryColor;
        public static string ClassBankPrimaryColor
        {
            get { return _BankPrimaryColor; }
            set { _BankPrimaryColor = value; }
        }
        public static string _BankSecondaryColor;
        public static string ClassBankSecondaryColor
        {
            get { return _BankSecondaryColor; }
            set { _BankSecondaryColor = value; }
        }

        // Process
        public static string _ProcessStartTime;
        public static string ClassProcessStartTime
        {
            get { return _ProcessStartTime; }
            set { _ProcessStartTime = value; }
        }

        public static string _ProcessEndTime;
        public static string ClassProcessEndTime
        {
            get { return _ProcessEndTime; }
            set { _ProcessEndTime = value; }
        }

        // Export
        public static string _ExportStartTime;
        public static string ClassExportStartTime
        {
            get { return _ExportStartTime; }
            set { _ExportStartTime = value; }
        }

        public static string _ExportEndTime;
        public static string ClassExportEndTime
        {
            get { return _ExportEndTime; }
            set { _ExportEndTime = value; }
        }

        public static string _RequestDate;
        public static string ClassRequestDate
        {
            get { return _RequestDate; }
            set { _RequestDate = value; }
        }

        public static string _ScheduleDate;
        public static string ClassScheduleDate
        {
            get { return _ScheduleDate; }
            set { _ScheduleDate = value; }
        }
        public static string _ServicedDate;
        public static string ClassServicedDate
        {
            get { return _ServicedDate; }
            set { _ServicedDate = value; }
        }

        public static int ClassReasonIsInput
        {
            get { return _ReasonIsInput; }
            set { _ReasonIsInput = value; }
        }
        public static string _ReportStatus;
        public static string ClassReportStatus
        {
            get { return _ReportStatus; }
            set { _ReportStatus = value; }
        }

        public static string _HoldTerminalSN;
        public static string ClassHoldTerminalSN
        {
            get { return _HoldTerminalSN; }
            set { _HoldTerminalSN = value; }
        }

        public static string _HoldSIMSN;
        public static string ClassHoldSIMSN
        {
            get { return _HoldSIMSN; }
            set { _HoldSIMSN = value; }
        }

    }
}
