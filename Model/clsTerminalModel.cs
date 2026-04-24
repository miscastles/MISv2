using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsTerminalModel
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _TerminalModelID;
        public static string _Description;
        public static int _TerminalTypeID;
        public static string _TypeDescription;

        public static void ResetClass()
        {
            clsTerminalModel.ClassTerminalModelID = 0;
        }
        public static int ClassTerminalModelID
        {
            get { return _TerminalModelID; }
            set { _TerminalModelID = value; }
        }
        public static string ClassDescription
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public static int ClassTerminalTypeID
        {
            get { return _TerminalTypeID; }
            set { _TerminalTypeID = value; }
        }
        public static string ClassTypeDescription
        {
            get { return _TypeDescription; }
            set { _TypeDescription = value; }
        }
    }
}
