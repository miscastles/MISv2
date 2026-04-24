using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MIS
{
    public class ParticularData
    {
        private int _ParticularID;
        [JsonProperty("ParticularID")]
        public int ParticularID
        {
            get { return _ParticularID; }
            set { _ParticularID = value; }
        }

        private int _ProvinceID;
        [JsonProperty("ProvinceID")]
        public int ProvinceID
        {
            get { return _ProvinceID; }
            set { _ProvinceID = value; }
        }

        private int _CityID;
        [JsonProperty("CityID")]
        public int CityID
        {
            get { return _CityID; }
            set { _CityID = value; }
        }

        private int _ParticularTypeID;
        [JsonProperty("ParticularTypeID")]
        public int ParticularTypeID
        {
            get { return _ParticularTypeID; }
            set { _ParticularTypeID = value; }
        }

        private int _RegionID;
        [JsonProperty("RegionID")]
        public int RegionID
        {
            get { return _RegionID; }
            set { _RegionID = value; }
        }

        private int _RegionType;
        [JsonProperty("RegionType")]
        public int RegionType
        {
            get { return _RegionType; }
            set { _RegionType = value; }
        }

        private string _ParticularDescription;
        [JsonProperty("ParticularDescription")]
        public string ParticularDescription
        {
            get { return _ParticularDescription; }
            set { _ParticularDescription = value; }
        }

        private string _ParticularName;
        [JsonProperty("Name")]
        public string ParticularName
        {
            get { return _ParticularName; }
            set { _ParticularName = value; }
        }

        private string _Address;
        [JsonProperty("Address")]
        public string Address
        {
            get { return _Address; }
            set { _Address = value; }
        }

        private string _Address2;
        [JsonProperty("Address2")]
        public string Address2
        {
            get { return _Address2; }
            set { _Address2 = value; }
        }

        private string _Address3;
        [JsonProperty("Address3")]
        public string Address3
        {
            get { return _Address3; }
            set { _Address3 = value; }
        }

        private string _Address4;
        [JsonProperty("Address4")]
        public string Address4
        {
            get { return _Address4; }
            set { _Address4 = value; }
        }

        private string _ContactPerson;
        [JsonProperty("ContactPerson")]
        public string ContactPerson
        {
            get { return _ContactPerson; }
            set { _ContactPerson = value; }
        }

        private string _TelNo;
        [JsonProperty("TelNo")]
        public string TelNo
        {
            get { return _TelNo; }
            set { _TelNo = value; }
        }

        private string _Mobile;
        [JsonProperty("Mobile")]
        public string Mobile
        {
            get { return _Mobile; }
            set { _Mobile = value; }
        }

        private string _Fax;
        [JsonProperty("Fax")]
        public string Fax
        {
            get { return _Fax; }
            set { _Fax = value; }
        }

        private string _Email;
        [JsonProperty("Email")]
        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }

        private string _ContractTerms;
        [JsonProperty("ContractTerms")]
        public string ContractTerms
        {
            get { return _ContractTerms; }
            set { _ContractTerms = value; }
        }

        private string _Province;
        [JsonProperty("Province")]
        public string Province
        {
            get { return _Province; }
            set { _Province = value; }
        }

        private string _City;
        [JsonProperty("City")]
        public string City
        {
            get { return _City; }
            set { _City = value; }
        }

        private string _Region;
        [JsonProperty("Region")]
        public string Region
        {
            get { return _Region; }
            set { _Region = value; }
        }

        private int _Count;
        [JsonProperty("Count")]
        public int Count
        {
            get { return _Count; }
            set { _Count = value; }
        }

        private string _ParticularUserName;
        [JsonProperty("ParticularUserName")]
        public string ParticularUserName
        {
            get { return _ParticularUserName; }
            set { _ParticularUserName = value; }
        }

        private string _ParticularUserKey;
        [JsonProperty("ParticularUserKey")]
        public string ParticularUserKey
        {
            get { return _ParticularUserKey; }
            set { _ParticularUserKey = value; }
        }

        // Leave Assignment
        private int _LeaveNo;
        [JsonProperty("LeaveNo")]
        public int LeaveNo
        {
            get { return _LeaveNo; }
            set { _LeaveNo = value; }
        }

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

        private double _LeaveCredit;
        [JsonProperty("LeaveCredit")]
        public double LeaveCredit
        {
            get { return _LeaveCredit; }
            set { _LeaveCredit = value; }
        }

        private string _Code;
        [JsonProperty("Code")]
        public string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }

        private string _Description;
        [JsonProperty("Description")]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        // Leave Movement
        private double _Duration;
        [JsonProperty("Duration")]
        public double Duration
        {
            get { return _Duration; }
            set { _Duration = value; }
        }

        private string _DateFrom;
        [JsonProperty("DateFrom")]
        public string DateFrom
        {
            get { return _DateFrom; }
            set { _DateFrom = value; }
        }

        private string _DateTo;
        [JsonProperty("DateTo")]
        public string DateTo
        {
            get { return _DateTo; }
            set { _DateTo = value; }
        }

        private string _DateType;
        [JsonProperty("DateType")]
        public string DateType
        {
            get { return _DateType; }
            set { _DateType = value; }
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

        private string _Remarks;
        [JsonProperty("Remarks")]
        public string Remarks
        {
            get { return _Remarks; }
            set { _Remarks = value; }
        }

        private int _isActive;
        [JsonProperty("isActive")]
        public int isActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        private string _LeaveCode;
        [JsonProperty("LeaveCode")]
        public string LeaveCode
        {
            get { return _LeaveCode; }
            set { _LeaveCode = value; }
        }

        private string _LeaveDesc;
        [JsonProperty("LeaveDesc")]
        public string LeaveDesc
        {
            get { return _LeaveDesc; }
            set { _LeaveDesc = value; }
        }

        private int _ReasonID;
        [JsonProperty("ReasonID")]
        public int ReasonID
        {
            get { return _ReasonID; }
            set { _ReasonID = value; }
        }

        private string _ReasonCode;
        [JsonProperty("ReasonCode")]
        public string ReasonCode
        {
            get { return _ReasonCode; }
            set { _ReasonCode = value; }
        }

        private string _ReasonDescription;
        [JsonProperty("ReasonDescription")]
        public string ReasonDescription
        {
            get { return _ReasonDescription; }
            set { _ReasonDescription = value; }
        }

        private string _ReasonDesc;
        [JsonProperty("ReasonDesc")]
        public string ReasonDesc
        {
            get { return _ReasonDesc; }
            set { _ReasonDesc = value; }
        }

        private string _EmploymentStatus;
        [JsonProperty("EmploymentStatus")]
        public string EmploymentStatus
        {
            get { return _EmploymentStatus; }
            set { _EmploymentStatus = value; }
        }

        private int _DepartmentID;
        [JsonProperty("DepartmentID")]
        public int DepartmentID
        {
            get { return _DepartmentID; }
            set { _DepartmentID = value; }
        }

        private string _Department;
        [JsonProperty("Department")]
        public string Department
        {
            get { return _Department; }
            set { _Department = value; }
        }

        private int _PositionID;
        [JsonProperty("PositionID")]
        public int PositionID
        {
            get { return _PositionID; }
            set { _PositionID = value; }
        }

        private string _Position;
        [JsonProperty("Position")]
        public string Position
        {
            get { return _Position; }
            set { _Position = value; }
        }

        private string _ComputerName;
        [JsonProperty("ComputerName")]
        public string ComputerName
        {
            get { return _ComputerName; }
            set { _ComputerName = value; }
        }

        private int _isWorkArrangement;
        [JsonProperty("isWorkArrangement")]
        public int isWorkArrangement
        {
            get { return _isWorkArrangement; }
            set { _isWorkArrangement = value; }
        }

        private int _isTimeSheet;
        [JsonProperty("isTimeSheet")]
        public int isTimeSheet
        {
            get { return _isTimeSheet; }
            set { _isTimeSheet = value; }
        }

        private int _isAppVersion;
        [JsonProperty("isAppVersion")]
        public int isAppVersion
        {
            get { return _isAppVersion; }
            set { _isAppVersion = value; }
        }

        // Work Arrangement
        private int _WorkArrangementID;
        [JsonProperty("WorkArrangementID")]
        public int WorkArrangementID
        {
            get { return _WorkArrangementID; }
            set { _WorkArrangementID = value; }
        }

        private int _WorkTypeID;
        [JsonProperty("WorkTypeID")]
        public int WorkTypeID
        {
            get { return _WorkTypeID; }
            set { _WorkTypeID = value; }
        }

        private string _WorkTypeCode;
        [JsonProperty("WorkTypeCode")]
        public string WorkTypeCode
        {
            get { return _WorkTypeCode; }
            set { _WorkTypeCode = value; }
        }

        private string _WorkTypeDesc;
        [JsonProperty("WorkTypeDesc")]
        public string WorkTypeDesc
        {
            get { return _WorkTypeDesc; }
            set { _WorkTypeDesc = value; }
        }

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

        private string _TID;
        [JsonProperty("TID")]
        public string TID
        {
            get { return _TID; }
            set { _TID = value; }
        }

        private string _MID;
        [JsonProperty("MID")]
        public string MID
        {
            get { return _MID; }
            set { _MID = value; }
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

        private int _IRStatus;
        [JsonProperty("IRStatus")]
        public int IRStatus
        {
            get { return _IRStatus; }
            set { _IRStatus = value; }
        }

        private string _IRStatusDescription;
        [JsonProperty("IRStatusDescription")]
        public string IRStatusDescription
        {
            get { return _IRStatusDescription; }
            set { _IRStatusDescription = value; }
        }

        private int _ClientID;
        [JsonProperty("ClientID")]
        public int ClientID
        {
            get { return _ClientID; }
            set { _ClientID = value; }
        }

        private int _RentalType;
        [JsonProperty("RentalType")]
        public int RentalType
        {
            get { return _RentalType; }
            set { _RentalType = value; }
        }

        private int _RentalTerms;
        [JsonProperty("RentalTerms")]
        public int RentalTerms
        {
            get { return _RentalTerms; }
            set { _RentalTerms = value; }
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

        private int _ID;
        [JsonProperty("ID")]
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private string _detail_info;
        [JsonProperty("detail_info")]
        public string detail_info
        {
            get { return _detail_info; }
            set { _detail_info = value; }
        }


    }

    public class ParticularDetailOnline
    {
        [JsonProperty("resp_code")]
        public string error_code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<ParticularData> data { get; set; }
    }
}
