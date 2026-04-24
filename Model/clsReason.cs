using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsReason
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _ReasonID;
        public static string _Code;
        public static string _Description;
        public static string _Type;
        public static int _ReasonIsInput;

        public static void ResetClass()
        {
            clsReason.ClassReasonID = 0;
        }
        public static bool ClassRecordFound
        {
            get { return RecordFound; }
            set { RecordFound = value; }
        }
        public static int ClassReasonID
        {
            get { return _ReasonID; }
            set { _ReasonID = value; }
        }
        public static string ClassReasonCode
        {
            get { return _Code; }
            set { _Code = value; }
        }
        public static string ClassReasonDescription
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public static string ClassReasonType
        {
            get { return _Type; }
            set { _Type = value; }
        }
        public static int ClassReasonIsInput
        {
            get { return _ReasonIsInput; }
            set { _ReasonIsInput = value; }
        }
    }
}
