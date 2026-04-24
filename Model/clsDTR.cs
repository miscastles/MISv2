using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsDTR
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _ParticularID;
        public static string _IDNo;
        public static string _Name;
        public static string _Date;
        public static string _DTRDate;
        public static string _WeekDay;
        public static string _CheckIn;
        public static string _CheckOut;
        public static string _TCheckHrs;
        public static string _OvertimeIn;
        public static string _OvertimeOut;
        public static string _TOverTimeHrs;
        public static string _FullName;
        public static string _Remarks;
        public static bool _isName;

        public static int ClassParticularID
        {
            get { return _ParticularID; }
            set { _ParticularID = value; }
        }
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
        public static string ClassDTRDate
        {
            get { return _DTRDate; }
            set { _DTRDate = value; }
        }
        public static string ClassWeekDay
        {
            get { return _WeekDay; }
            set { _WeekDay = value; }
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
    }
}
