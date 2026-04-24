using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsFE
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _FEID;
        public static string _Name;
        public static string _Address;
        public static string _ContactNo;
        public static int ClassFEID
        {
            get { return _FEID; }
            set { _FEID = value; }
        }
        public static string ClassName
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public static string ClassAddress
        {
            get { return _Address; }
            set { _Address = value; }
        }
        public static string ClassContactNo
        {
            get { return _ContactNo; }
            set { _ContactNo = value; }
        }
    }
}
