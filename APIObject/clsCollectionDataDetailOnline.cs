using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class CollectionData
    {
        private int _isActive;
        [JsonProperty("isActive")]
        public int isActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        private int _RegionID;
        [JsonProperty("RegionID")]
        public int RegionID
        {
            get { return _RegionID; }
            set { _RegionID = value; }
        }

        private int _TerminalTypeID;
        [JsonProperty("TerminalTypeID")]
        public int TerminalTypeID
        {
            get { return _TerminalTypeID; }
            set { _TerminalTypeID = value; }
        }

        private int _TerminalModelID;
        [JsonProperty("TerminalModelID")]
        public int TerminalModelID
        {
            get { return _TerminalModelID; }
            set { _TerminalModelID = value; }
        }

        private int _TerminalBrandID;
        [JsonProperty("TerminalBrandID")]
        public int TerminalBrandID
        {
            get { return _TerminalBrandID; }
            set { _TerminalBrandID = value; }
        }

        private int _TerminalStatusID;
        [JsonProperty("TerminalStatusID")]
        public int TerminalStatusID
        {
            get { return _TerminalStatusID; }
            set { _TerminalStatusID = value; }
        }

        private int _ServiceTypeID;
        [JsonProperty("ServiceTypeID")]
        public int ServiceTypeID
        {
            get { return _ServiceTypeID; }
            set { _ServiceTypeID = value; }
        }

        private int _ParticularID;
        [JsonProperty("ParticularID")]
        public int ParticularID
        {
            get { return _ParticularID; }
            set { _ParticularID = value; }
        }

        private int _MerchantID;
        [JsonProperty("MerchantID")]
        public int MerchantID
        {
            get { return _MerchantID; }
            set { _MerchantID = value; }
        }

        private int _ServiceStatusID;
        [JsonProperty("ServiceStatusID")]
        public int ServiceStatusID
        {
            get { return _ServiceStatusID; }
            set { _ServiceStatusID = value; }
        }

        private int _ServiceStatus;
        [JsonProperty("ServiceStatus")]
        public int ServiceStatus
        {
            get { return _ServiceStatus; }
            set { _ServiceStatus = value; }
        }

        private string _Region;
        [JsonProperty("Region")]
        public string Region
        {
            get { return _Region; }
            set { _Region = value; }
        }
        
        private string _Description;
        [JsonProperty("Description")]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private string _Name;
        [JsonProperty("Name")]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Address;
        [JsonProperty("Address")]
        public string Address
        {
            get { return _Address; }
            set { _Address = value; }
        }

        private string _Province;
        [JsonProperty("Province")]
        public string Province
        {
            get { return _Province; }
            set { _Province = value; }
        }

        private string _ServiceStatusDescription;
        [JsonProperty("ServiceStatusDescription")]
        public string ServiceStatusDescription
        {
            get { return _ServiceStatusDescription; }
            set { _ServiceStatusDescription = value; }
        }

        private string _Code;
        [JsonProperty("Code")]
        public string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }

        private int _JobType;
        [JsonProperty("JobType")]
        public int JobType
        {
            get { return _JobType; }
            set { _JobType = value; }
        }

        private string _JobTypeDescription;
        [JsonProperty("JobTypeDescription")]
        public string JobTypeDescription
        {
            get { return _JobTypeDescription; }
            set { _JobTypeDescription = value; }
        }

        private string _ServiceJobTypeDescription;
        [JsonProperty("ServiceJobTypeDescription")]
        public string ServiceJobTypeDescription
        {
            get { return _ServiceJobTypeDescription; }
            set { _ServiceJobTypeDescription = value; }
        }

        private int _TerminalStatusType;
        [JsonProperty("TerminalStatusType")]
        public int TerminalStatusType
        {
            get { return _TerminalStatusType; }
            set { _TerminalStatusType = value; }
        }

        // Holiday
        private int _HolidayID;
        [JsonProperty("HolidayID")]
        public int HolidayID
        {
            get { return _HolidayID; }
            set { _HolidayID = value; }
        }

        private string _HolidayDate;
        [JsonProperty("HolidayDate")]
        public string HolidayDate
        {
            get { return _HolidayDate; }
            set { _HolidayDate = value; }
        }


        // LeaveType
        private int _LeaveTypeID;
        [JsonProperty("LeaveTypeID")]
        public int LeaveTypeID
        {
            get { return _LeaveTypeID; }
            set { _LeaveTypeID = value; }
        }

        private double _CreditLimit;
        [JsonProperty("CreditLimit")]
        public double CreditLimit
        {
            get { return _CreditLimit; }
            set { _CreditLimit = value; }
        }

        // Department
        private int _DepartmentID;
        [JsonProperty("DepartmentID")]
        public int DepartmentID
        {
            get { return _DepartmentID; }
            set { _DepartmentID = value; }
        }

        // Position
        private int _PositionID;
        [JsonProperty("PositionID")]
        public int PositionID
        {
            get { return _PositionID; }
            set { _PositionID = value; }
        }

        // ListView Header Column
        private string _Field;
        [JsonProperty("Field")]
        public string Field
        {
            get { return _Field; }
            set { _Field = value; }
        }

        private string _Width;
        [JsonProperty("Width")]
        public string Width
        {
            get { return _Width; }
            set { _Width = value; }
        }

        private string _Title;
        [JsonProperty("Title")]
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        private string _Align;
        [JsonProperty("Align")]
        public string Align
        {
            get { return _Align; }
            set { _Align = value; }
        }

        private string _Visible;
        [JsonProperty("Visible")]
        public string Visible
        {
            get { return _Visible; }
            set { _Visible = value; }
        }

        private string _AutoWidth;
        [JsonProperty("AutoWidth")]
        public string AutoWidth
        {
            get { return _AutoWidth; }
            set { _AutoWidth = value; }
        }

        private string _Format;
        [JsonProperty("Format")]
        public string Format
        {
            get { return _Format; }
            set { _Format = value; }
        }

        // Report        
        private int _ReportID;
        [JsonProperty("ReportID")]
        public int ReportID
        {
            get { return _ReportID; }
            set { _ReportID = value; }
        }

        // WorkType        
        private int _WorkTypeID;
        [JsonProperty("WorkTypeID")]
        public int WorkTypeID
        {
            get { return _WorkTypeID; }
            set { _WorkTypeID = value; }
        }

        // TimeSheet
        private int _TimeSheetID;
        [JsonProperty("TimeSheetID")]
        public int TimeSheetID
        {
            get { return _TimeSheetID; }
            set { _TimeSheetID = value; }
        }

        private string _TSDate;
        [JsonProperty("TSDate")]
        public string TSDate
        {
            get { return _TSDate; }
            set { _TSDate = value; }
        }

        private string _TimeIn;
        [JsonProperty("TimeIn")]
        public string TimeIn
        {
            get { return _TimeIn; }
            set { _TimeIn = value; }
        }

        private string _TimeOut;
        [JsonProperty("TimeOut")]
        public string TimeOut
        {
            get { return _TimeOut; }
            set { _TimeOut = value; }
        }

        private string _THours;
        [JsonProperty("THours")]
        public string THours
        {
            get { return _THours; }
            set { _THours = value; }
        }

        private string _TTime;
        [JsonProperty("TTime")]
        public string TTime
        {
            get { return _TTime; }
            set { _TTime = value; }
        }

        private string _TimeStatus;
        [JsonProperty("TimeStatus")]
        public string TimeStatus
        {
            get { return _TimeStatus; }
            set { _TimeStatus = value; }
        }

        private string _Position;
        [JsonProperty("Position")]
        public string Position
        {
            get { return _Position; }
            set { _Position = value; }
        }

        private string _Department;
        [JsonProperty("Department")]
        public string Department
        {
            get { return _Department; }
            set { _Department = value; }
        }

        private string _WorkType;
        [JsonProperty("WorkType")]
        public string WorkType
        {
            get { return _WorkType; }
            set { _WorkType = value; }
        }

        private string _EmploymentStatus;
        [JsonProperty("EmploymentStatus")]
        public string EmploymentStatus
        {
            get { return _EmploymentStatus; }
            set { _EmploymentStatus = value; }
        }

        private string _TimeSheetDate;
        [JsonProperty("TimeSheetDate")]
        public string TimeSheetDate
        {
            get { return _TimeSheetDate; }
            set { _TimeSheetDate = value; }
        }

        private string _ComputerName;
        [JsonProperty("ComputerName")]
        public string ComputerName
        {
            get { return _ComputerName; }
            set { _ComputerName = value; }
        }

        private string _LocalIP;
        [JsonProperty("LocalIP")]
        public string LocalIP
        {
            get { return _LocalIP; }
            set { _LocalIP = value; }
        }

        private int _ID;
        [JsonProperty("ID")]
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private string _TerminalID;
        [JsonProperty("TerminalID")]
        public string TerminalID
        {
            get { return _TerminalID; }
            set { _TerminalID = value; }
        }

        private string _TerminalName;
        [JsonProperty("TerminalName")]
        public string TerminalName
        {
            get { return _TerminalName; }
            set { _TerminalName = value; }
        }

        // Privacy
        private int _PrivacyID;
        [JsonProperty("PrivacyID")]
        public int PrivacyID
        {
            get { return _PrivacyID; }
            set { _PrivacyID = value; }
        }

        private string _Form;
        [JsonProperty("Form")]
        public string Form
        {
            get { return _Form; }
            set { _Form = value; }
        }

        // Country
        private int _CountryID;
        [JsonProperty("CountryID")]
        public int CountryID
        {
            get { return _CountryID; }
            set { _CountryID = value; }
        }

        private string _Country;
        [JsonProperty("Country")]
        public string Country
        {
            get { return _Country; }
            set { _Country = value; }
        }

        private string _outParamValue;
        [JsonProperty("outParamValue")]
        public string outParamValue
        {
            get { return _outParamValue; }
            set { _outParamValue = value; }
        }

        // Pri
        private int _isAdd;
        [JsonProperty("isAdd")]
        public int isAdd
        {
            get { return _isAdd; }
            set { _isAdd = value; }
        }

        private int _isUpdate;
        [JsonProperty("isUpdate")]
        public int isUpdate
        {
            get { return _isUpdate; }
            set { _isUpdate = value; }
        }

        private int _isDelete;
        [JsonProperty("isDelete")]
        public int isDelete
        {
            get { return _isDelete; }
            set { _isDelete = value; }
        }

        private int _isView;
        [JsonProperty("isView")]
        public int isView
        {
            get { return _isView; }
            set { _isView = value; }
        }

        private int _isPrint;
        [JsonProperty("isPrint")]
        public int isPrint
        {
            get { return _isPrint; }
            set { _isPrint = value; }
        }

        private int _isChecked;
        [JsonProperty("isChecked")]
        public int isChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; }
        }

        private string _ParticularName;
        [JsonProperty("ParticularName")]
        public string ParticularName
        {
            get { return _ParticularName; }
            set { _ParticularName = value; }
        }

        // WorkType        
        private int _TypeID;
        [JsonProperty("TypeID")]
        public int TypeID
        {
            get { return _TypeID; }
            set { _TypeID = value; }
        }

        // POS Rental / Invoice Master
        private int _InvoiceID;
        [JsonProperty("InvoiceID")]
        public int InvoiceID
        {
            get { return _InvoiceID; }
            set { _InvoiceID = value; }
        }

        private string _InvoiceNo;
        [JsonProperty("InvoiceNo")]
        public string InvoiceNo
        {
            get { return _InvoiceNo; }
            set { _InvoiceNo = value; }
        }

        private string _AccountNo;
        [JsonProperty("AccountNo")]
        public string AccountNo
        {
            get { return _AccountNo; }
            set { _AccountNo = value; }
        }

        private string _CustomerNo;
        [JsonProperty("CustomerNo")]
        public string CustomerNo
        {
            get { return _CustomerNo; }
            set { _CustomerNo = value; }
        }

        private string _InvoiceDate;
        [JsonProperty("InvoiceDate")]
        public string InvoiceDate
        {
            get { return _InvoiceDate; }
            set { _InvoiceDate = value; }
        }

        private string _ReferenceNo;
        [JsonProperty("ReferenceNo")]
        public string ReferenceNo
        {
            get { return _ReferenceNo; }
            set { _ReferenceNo = value; }
        }

        private string _DateCoveredFrom;
        [JsonProperty("DateCoveredFrom")]
        public string DateCoveredFrom
        {
            get { return _DateCoveredFrom; }
            set { _DateCoveredFrom = value; }
        }

        private string _DateCoveredTo;
        [JsonProperty("DateCoveredTo")]
        public string DateCoveredTo
        {
            get { return _DateCoveredTo; }
            set { _DateCoveredTo = value; }
        }

        private string _DateDue;
        [JsonProperty("DateDue")]
        public string DateDue
        {
            get { return _DateDue; }
            set { _DateDue = value; }
        }

        private double _TAmtDue;
        [JsonProperty("TAmtDue")]
        public double TAmtDue
        {
            get { return _TAmtDue; }
            set { _TAmtDue = value; }
        }

        private int _ModeOfPayment;
        [JsonProperty("ModeOfPayment")]
        public int ModeOfPayment
        {
            get { return _ModeOfPayment; }
            set { _ModeOfPayment = value; }
        }

        private string _NoteToCustomer;
        [JsonProperty("NoteToCustomer")]
        public string NoteToCustomer
        {
            get { return _NoteToCustomer; }
            set { _NoteToCustomer = value; }
        }

        private string _NoteToSelf;
        [JsonProperty("NoteToSelf")]
        public string NoteToSelf
        {
            get { return _NoteToSelf; }
            set { _NoteToSelf = value; }
        }

        private string _ProcessedBy;
        [JsonProperty("ProcessedBy")]
        public string ProcessedBy
        {
            get { return _ProcessedBy; }
            set { _ProcessedBy = value; }
        }

        private string _ProcessedDateTime;
        [JsonProperty("ProcessedDateTime")]
        public string ProcessedDateTime
        {
            get { return _ProcessedDateTime; }
            set { _ProcessedDateTime = value; }
        }

        private string _ModifiedBy;
        [JsonProperty("ModifiedBy")]
        public string ModifiedBy
        {
            get { return _ModifiedBy; }
            set { _ModifiedBy = value; }
        }

        private string _ModifiedDateTime;
        [JsonProperty("ModifiedDateTime")]
        public string ModifiedDateTime
        {
            get { return _ModifiedDateTime; }
            set { _ModifiedDateTime = value; }
        }

        private int _MapID;
        [JsonProperty("MapID")]
        public int MapID
        {
            get { return _MapID; }
            set { _MapID = value; }
        }

        private string _detail_info;
        [JsonProperty("detail_info")]
        public string detail_info
        {
            get { return _detail_info; }
            set { _detail_info = value; }
        }

        private int _ServiceNo;
        [JsonProperty("ServiceNo")]
        public int ServiceNo
        {
            get { return _ServiceNo; }
            set { _ServiceNo = value; }
        }

        private int _MobileID;
        [JsonProperty("MobileID")]
        public int MobileID
        {
            get { return _MobileID; }
            set { _MobileID = value; }
        }

        private int _FSRNo;
        [JsonProperty("FSRNo")]
        public int FSRNo
        {
            get { return _FSRNo; }
            set { _FSRNo = value; }
        }

        private int _IRIDNo;
        [JsonProperty("IRIDNo")]
        public int IRIDNo
        {
            get { return _IRIDNo; }
            set { _IRIDNo = value; }
        }

        private string _IRNo;
        [JsonProperty("IRNo")]
        public string IRNo
        {
            get { return _IRNo; }
            set { _IRNo = value; }
        }

        private string _MerchantName;
        [JsonProperty("MerchantName")]
        public string MerchantName
        {
            get { return _MerchantName; }
            set { _MerchantName = value; }
        }

        private string _MerchantEmail;
        [JsonProperty("MerchantEmail")]
        public string MerchantEmail
        {
            get { return _MerchantEmail; }
            set { _MerchantEmail = value; }
        }

        private string _FEName;
        [JsonProperty("FEName")]
        public string FEName
        {
            get { return _FEName; }
            set { _FEName = value; }
        }

        private string _FEEmail;
        [JsonProperty("FEEmail")]
        public string FEEmail
        {
            get { return _FEEmail; }
            set { _FEEmail = value; }
        }

        private string _FSRDate;
        [JsonProperty("FSRDate")]
        public string FSRDate
        {
            get { return _FSRDate; }
            set { _FSRDate = value; }
        }

    }

    public class CollectionDataDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<CollectionData> data { get; set; }
    }
}
