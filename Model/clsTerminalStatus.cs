using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsTerminalStatus
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _TerminalStatusID;
        public static string _Description;
        public static int _TerminalStatusType;

        public static int ClassTerminalStatusID
        {
            get { return _TerminalStatusID; }
            set { _TerminalStatusID = value; }
        }

        public static string ClassDescription
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public static int ClassTerminalStatusType
        {
            get { return _TerminalStatusType; }
            set { _TerminalStatusType = value; }
        }
    }
}
