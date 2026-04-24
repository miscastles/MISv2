using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.Model
{
    public class clsDepartment
    {
        public static bool _RecordFound;
        public static bool RecordFound { get { return _RecordFound; } set { _RecordFound = value; } }

        public static int _DepartmentID;
        public static int ClassDepartmentID { get { return _DepartmentID; } set { _DepartmentID = value; } }

        public static string _Description;
        public static string ClassDescription { get { return _Description; } set { _Description = value; } }

        public static int _isActive;
        public static int ClassisActive { get { return _isActive; } set { _isActive = value; } }
    }
}
