using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIS
{
    public class clsTerminalType
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _TerminalTypeID;
        public static string _Description;        

        public static void ResetClass()
        {
            clsTerminalType.ClassTerminalTypeID = 0;
        }
        public static int ClassTerminalTypeID
        {
            get { return _TerminalTypeID; }
            set { _TerminalTypeID = value; }
        }    
        public static string ClassDescription
        {
            get { return _Description; }
            set { _Description = value; }
        }     
    }
}
