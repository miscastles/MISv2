using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.Model
{
    public class clsHoliday
    {
        public static bool _RecordFound;
        public static bool RecordFound { get { return _RecordFound; } set { _RecordFound = value; } }

        public static int _HolidayID;
        public static int ClassHolidayID { get { return _HolidayID; } set { _HolidayID = value; } }

        public static string _HolidayDate;
        public static string ClassHolidayDate { get { return _HolidayDate; } set { _HolidayDate = value; } }

        public static string _Description;
        public static string ClassDescription { get { return _Description; } set { _Description = value; } }

        public static int _isActive;
        public static int ClassisActive { get { return _isActive; } set { _isActive = value; } }


    }
}
