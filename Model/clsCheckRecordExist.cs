using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    class clsCheckRecordExist
    {
        public static bool RecordFound;
        public static int _RecordID;        

        public static int ClassRecordID
        {
            get { return _RecordID; }
            set { _RecordID = value; }
        }      
    }
}
