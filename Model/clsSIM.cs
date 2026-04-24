using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsSIM
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _SIMID;        
        public static string _SIMSN;
        public static string _SIMCarrier;
        public static int _SIMStatus;
        public static string _SIMStatusDescription;

        public static int ClassSIMID
        {
            get { return _SIMID; }
            set { _SIMID = value; }
        }
        public static string ClassSIMSN
        {
            get { return _SIMSN; }
            set { _SIMSN = value; }
        }
        public static string ClassSIMCarrier
        {
            get { return _SIMCarrier; }
            set { _SIMCarrier = value; }
        }
        public static int ClassSIMStatus
        {
            get { return _SIMStatus; }
            set { _SIMStatus = value; }
        }
        public static string ClassSIMStatusDescription
        {
            get { return _SIMStatusDescription; }
            set { _SIMStatusDescription = value; }
        }
    }
}
