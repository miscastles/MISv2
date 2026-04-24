using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.Model
{
    public class clsLeaveType
    {
        public static bool _RecordFound;
        public static bool RecordFound { get { return _RecordFound; } set { _RecordFound = value; } }

        public static int _ParticularID;
        public static int ClassParticularID { get { return _ParticularID; } set { _ParticularID = value; } }

        public static int _LeaveTypeID;
        public static int ClassLeaveTypeID { get { return _LeaveTypeID; } set { _LeaveTypeID = value; } }

        public static string _Code;
        public static string ClassCode { get { return _Code; } set { _Code = value; } }
        public static string _Description;
        public static string ClassDescription { get { return _Description; } set { _Description = value; } }

        public static double _CreditLimit;
        public static double ClassCreditLimit { get { return _CreditLimit; } set { _CreditLimit = value; } }

        public static double _LeaveCredit;
        public static double ClassLeaveCredit { get { return _LeaveCredit; } set { _LeaveCredit = value; } }

        public static int _isActive;
        public static int ClassisActive { get { return _isActive; } set { _isActive = value; } }

        public static int _LeaveNo;
        public static int ClassLeaveNo { get { return _LeaveNo; } set { _LeaveNo = value; } }

        public static string _DateFrom;
        public static string ClassDateFrom { get { return _DateFrom; } set { _DateFrom = value; } }

        public static string _DateTo;
        public static string ClassDateTo { get { return _DateTo; } set { _DateTo = value; } }

        public static double _Duration;
        public static double ClassDuration { get { return _Duration; } set { _Duration = value; } }

        public static string _DateType;
        public static string ClassDateType { get { return _DateType; } set { _DateType = value; } }

        public static int _ReasonID;
        public static int ClassReasonID { get { return _ReasonID; } set { _ReasonID = value; } }

        public static string _Reason;
        public static string ClassReason { get { return _Reason; } set { _Reason = value; } }

        public static string _Remarks;
        public static string ClassRemarks { get { return _Remarks; } set { _Remarks = value; } }


        public static string _ProcessedBy;
        public static string ClassProcessedBy { get { return _ProcessedBy; } set { _ProcessedBy = value; } }

        public static string _ProcessedDateTime;
        public static string ClassProcessedDateTime { get { return _ProcessedDateTime; } set { _ProcessedDateTime = value; } }

        public static string _ModifiedBy;
        public static string ClassModifiedBy { get { return _ModifiedBy; } set { _ModifiedBy = value; } }

        public static string _ModifiedDateTime;
        public static string ClassModifiedDateTime { get { return _ModifiedDateTime; } set { _ModifiedDateTime = value; } }

        public static string _CreditLimitString;
        public static string ClassCreditLimitString { get { return _CreditLimitString; } set { _CreditLimitString = value; } }

        public static string _LeaveCreditString;
        public static string ClassLeaveCreditString { get { return _LeaveCreditString; } set { _LeaveCreditString = value; } }


    }
}
