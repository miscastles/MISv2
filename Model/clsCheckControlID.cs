using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    class clsCheckControlID
    {
        public static bool RecordFound;
        public static int _ControlID;

        public static int ClassControlID
        {
            get { return _ControlID; }
            set { _ControlID = value; }
        }
    }
}
