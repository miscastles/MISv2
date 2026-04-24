using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsArray
    {
        public static string[] UserID;
        public static string[] UserName;
        public static string[] Password;
        public static string[] MD5Password;
        public static string[] FullName;
        public static string[] UserType;
        public static string[] LogID;
        public static string[] ComputerIP;
        public static string[] ComputerName;
        public static string[] SessionStatus;
        public static string[] SessionStatusDescription;
        public static string[] LogInDate;
        public static string[] LogInTime;
        public static string[] LogOutDate;
        public static string[] LogOutTime;
        public static string[] LogPublishVersion;        
        public static string[] MobileTerminalID;
        public static string[] MobileTerminalName;

        public static string[] ProviderID;
        public static string[] Name;
        public static string[] Address;
        public static string[] Address2;
        public static string[] Address3;
        public static string[] Address4;
        public static string[] TelNo;
        public static string[] MobileNo;
        public static string[] ContactNo;
        public static string[] Fax;
        public static string[] Email;
        public static string[] ContractTerms;
        public static string[] Province;

        public static string[] ServiceTypeID;
        public static string[] Description;
        public static string[] Code;
        public static string[] ServiceCount;
        public static string[] ServiceStatus;
        public static string[] ServiceStatusDescription;
        public static string[] ReferenceNo;

        public static string[] ParticularDescription;
        public static string[] OtherServiceTypeID;
        public static string[] TerminalTypeID;
        public static string[] TerminalModelID;
        public static string[] TerminalBrandID;
        public static string[] TerminalStatusID;
        public static string[] TerminalStatusType;
        public static string[] FEID;
        public static string[] CityID;
        public static string[] City;
        public static string[] ProvinceID;
        public static string[] ParticularID;
        public static string[] ParticularTypeID;
        public static string[] RegionID;
        public static string[] Region;

        public static string[] MapID;
        public static string[] MapFrom;
        public static string[] MapTo;
        public static string[] MapDelimeter;
        public static string[] MapColumnIndex;
        public static string[] isMust;
        public static string[] Format;

        public static string[] LastInsertedID;
        public static string[] ID;

        // FSR
        public static string[] FSRNo;
        public static string[] FSRID;
        public static string[] No;
        public static string[] Merchant;
        public static string[] MID;
        public static string[] TID;
        public static string[] InvoiceNo;
        public static string[] BatchNo;
        public static string[] TimeArrived;
        public static string[] TimeStart;
        public static string[] FSR;
        public static string[] FSRDate;
        public static string[] FSRTime;
        public static string[] TxnType;
        public static string[] TxnAmt;
        public static string[] TimeEnd;
        public static string[] AuthCode;
        public static string[] RefNo;
        public static string[] TerminalSN;
        public static string[] MerchantContactNo;
        public static string[] MerchantRepresentative;
        public static string[] FEName;
        public static string[] FEEmail;
        public static string[] SerialNo;
        public static string[] SerialNoList;
        public static string[] NRIC;
        public static string[] AdditionalInformation;
        public static string[] FSRStatus;
        public static string[] FSRStatusDescription;
        public static string[] FSRServiceStatus;
        public static string[] FSRServiceStatusDescription;
        public static string[] FSRRemarks;
        public static string[] ProcessType;
        public static string[] PowerSN;
        public static string[] DockSN;
        public static string[] FSRStatusList;
        public static string[] DateFrom;
        public static string[] DateTo;
        public static string[] Date;
        public static string[] Time;

        // SIM
        public static string[] SIMID;
        public static string[] SIMSerialNo;
        public static string[] SIMCarrier;
        public static string[] AssignedTo;
        public static string[] Remarks;
        public static string[] SIMStatus;
        public static string[] SIMStatusDescription;

        // Terminal
        public static string[] TerminalID;
        public static string[] TIID;
        public static string[] TerminalType;
        public static string[] TerminalModel;
        public static string[] TerminalBrand;
        public static string[] DeliveryDate;
        public static string[] ReceiveDate;
        public static string[] TerminalStatus;
        public static string[] TerminalStatusDescription;
        public static string[] TerminalPartNo;
        public static string[] TerminalPONo;
        public static string[] TerminalInvNo;

        // Merchant
        public static string[] MerchantID;
        public static string[] MerchantName;
        public static string[] MerchantEmail;
        public static string[] ContactPerson;

        // IR
        public static string[] IRIDNo;
        public static string[] IRID;
        public static string[] IRNo;
        public static string[] IRDate;
        public static string[] InstallationDate;
        public static string[] ParticularName;
        public static string[] IRStatus;
        public static string[] IRStatusDescription;
        public static string[] IRImportDateTime;
        public static string[] ProcessedBy;
        public static string[] ModifiedBy;
        public static string[] ProcessedDateTime;
        public static string[] ModifiedDateTime;

        // TA
        public static string[] TAIDNo;
        public static string[] TAID;
        public static string[] ClientID;
        public static string[] ServiceProviderID;
        public static string[] TypeDescription;
        public static string[] ModelDescription;
        public static string[] BrandDescription;
        public static string[] ClientName;
        public static string[] ServiceProviderName;
        public static string[] TADateTime;
        public static string[] TAProcessedBy;
        public static string[] TAModifiedBy;
        public static string[] TAProcessedDateTime;
        public static string[] TAModifiedDateTime;
        public static string[] TARemarks;
        public static string[] TAComments;
        public static string[] ServiceTypeDescription;
        public static string[] OtherServiceTypeDescription;
        public static string[] ServiceTypeStatus;
        public static string[] ServiceTypeStatusDescription;
        public static string[] DockID;        

        // Report
        public static string[] ReportID;
        public static string[] ReportDesc;
        public static string[] ReportType;
        public static string[] ReportOrderDisplay;

        // Header
        public static string[] HeaderID;
        public static string[] Header1;
        public static string[] Header2;
        public static string[] Header3;
        public static string[] Header4;
        public static string[] Header5;

        // FSR Header
        public static string[] FSRHeader0;
        public static string[] FSRHeader1;
        public static string[] FSRHeader2;
        public static string[] FSRHeader3;
        public static string[] FSRHeader4;
        public static string[] FSRHeader5;
        public static string[] FSRHeader6;
        public static string[] FSRHeader7;
        public static string[] FSRHeader8;
        public static string[] FSRHeader9;
        public static string[] FSRHeader10;
        public static string[] FSRHeader11;
        public static string[] FSRHeader12;
        public static string[] FSRHeader13;
        public static string[] FSRHeader14;
        public static string[] FSRHeader15;

        // System
        public static string[] SysID;
        public static string[] PublishDate;
        public static string[] PublishVersion;

        // Reason
        public static string[] ReasonID;
        public static string[] ReasonCode;
        public static string[] ReasonDescription;
        public static string[] ReasonType;
        public static string[] ReasonIsInput;

        // Region Detail
        public static string[] RegionType;
        public static string[] RegionProvince;

        // Service Call
        public static string[] SCNo;
        public static string[] SCDateTime;
        public static string[] ReferralID;
        public static string[] CustomerName;
        public static string[] CustomerContactNo;
        public static string[] ReportedProblem;
        public static string[] ArrangementMade;
        public static string[] SCReqDate;
        public static string[] SCReqTime;
        public static string[] SCShipDate;
        public static string[] SCShipTime;
        public static string[] TrackingNo;
        public static string[] SCStatus;
        public static string[] SCListStatus;

        // Servicing Detail
        public static string[] ServiceNo;

        public static string[] CurTerminalSNStatus;
        public static string[] CurSIMSNStatus;
        public static string[] CurDockSNStatus;
        public static string[] CurTerminalSNStatusDescription;
        public static string[] CurSIMSNStatusDescription;
        public static string[] CurDockSNStatusDescription;

        public static string[] RepTerminalSNStatus;
        public static string[] RepSIMSNStatus;
        public static string[] RepDockSNStatus;
        public static string[] RepTerminalSNStatusDescription;
        public static string[] RepSIMSNStatusDescription;
        public static string[] RepDockSNStatusDescription;

        public static string[] CounterNo;
        public static string[] RequestNo;
        public static string[] ServiceDateTime;
        public static string[] ServiceCode;
        public static string[] ServiceDate;
        public static string[] ServiceTime;     
        public static string[] RequestTime;
        public static string[] ServiceReqDate;
        public static string[] ServiceReqTime;
        public static string[] LastServiceRequest;
        public static string[] NewServiceRequest;
        public static string[] ReplaceTerminalSN;
        public static string[] ReplaceSIMSN;
        public static string[] ReplaceDockSN;
        public static string[] JobType;
        public static string[] JobTypeDescription;
        public static string[] JobTypeSubDescription;
        public static string[] JobTypeStatusDescription;
        public static string[] ServiceJobTypeDescription;

        public static string[] ActionMade;

        // Expenses
        public static string[] ExpensesID;
        public static string[] ExpensesDescription;
        public static string[] ExpensesAmount;
        public static string[] TExpenses;

        public static string[] HoldExpensesID;
        public static string[] HoldExpensesDescription;
        public static string[] HoldExpensesAmount;

        public static string[,] arr3DImport;

        // FSR
        public static string[] ProblemReported;
        public static string[] ActualProblemReported;
        public static string[] ActionTaken;
        public static string[] AnyComments;

        public static string[] HoldServiceNo;
        public static string[] HoldTAIDNo;
        public static string[] HoldIRIDNo;
        public static string[] HoldTerminalID;

        public static string[] sRepoData;

        // Type
        public static string[] TypeID;        
        public static string[] TypeRemarks;
        public static string[] TypeQueryString;
        public static string[] TypeValue;

        // Holiday
        public static string[] HolidayID;
        public static string[] HolidayDate;
        public static string[] HolidayDesc;
        public static string[] HolidayisActive;

        // Leave Type
        public static string[] LeaveNo;
        public static string[] LeaveTypeID;
        public static string[] LeaveTypeCode;
        public static string[] LeaveTypeDesc;
        public static string[] LeaveTypeCreditLimit;
        public static string[] LeaveCredit;
        public static string[] LeaveTypeisActive;

        // Department
        public static string[] DepartmentID;
        public static string[] DepartmentDesc;
        public static string[] DepartmentisActive;

        // Position
        public static string[] PositionID;
        public static string[] PositionDesc;
        public static string[] PositionisActive;

        public static string[] EmploymentStatus;

        // Leave Movement
        public static string[] Duration;
        public static string[] DateType;
        public static string[] isActive;

        public static string[] AppVersion;
        public static string[] AppCRC;

        public static string[] PrimaryNum;
        public static string[] SecondaryNum;

        // WorkType
        public static string[] WorkTypeID;
        public static string[] WorkType;

        // TimeSheet
        public static string[] TimeSheetID;
        public static string[] TimeSheetDate;
        public static string[] TimeIn;
        public static string[] TimeOut;
        public static string[] TimeStatus;
        public static string[] THours;
        public static string[] TTime;
        public static string[] LocalIP;
        public static string[] RemoteIP;
        public static string[] TerminalName;

        // Work Arrangment
        public static string[] WorkArrangementID;

        // Privacy
        public static string[] PrivacyID;
        public static string[] Form;
        public static string[] isAdd;
        public static string[] isDelete;
        public static string[] isUpdate;
        public static string[] isView;
        public static string[] isPrint;
        public static string[] isChecked;

        // Country
        public static string[] CountryID;
        public static string[] Country;

        public static string[] Location;
        public static string[] Allocation;
        public static string[] AssetType;

        // Movement
        public static string[] TransNo;
        public static string[] TransDate;
        public static string[] TransTime;
        public static string[] ReleaseDate;
        public static string[] FromLocationID;
        public static string[] FromLocation;
        public static string[] ToLocationID;
        public static string[] ToLocation;

        // POS Rental / Invoice Master
        public static string[] InvoiceID;
        public static string[] AccountNo;
        public static string[] CustomerNo;
        public static string[] InvoiceDate;
        public static string[] DateCoveredFrom;
        public static string[] DateCoveredTo;
        public static string[] DateDue;
        public static string[] TAmtDue;
        public static string[] ModeOfPayment;
        public static string[] NoteToCustomer;
        public static string[] NoteToSelf;
        public static string[] RentalFee;

        public static string[] MobileID;
        public static string[] detail_info;

        // Selected
        public static string[] SearchKey;
        public static string[] SearchValue;


        public static string[] arrDTRHeader = new string[] { "CHECK-IN", "CHECK-OUT", "THRS", "OVERTIME-IN", "OVERTIME-OUT", "THRS", "REMARKS" };
    }
}
