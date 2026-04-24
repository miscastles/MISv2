using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsTerminalBrand
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _TerminalBrandID;
        public static string _Description;

        public static void ResetClass()
        {
            clsTerminalBrand.ClassTerminalBrandID = 0;
        }
        public static int ClassTerminalBrandID
        {
            get { return _TerminalBrandID; }
            set { _TerminalBrandID = value; }
        }
        public static string ClassDescription
        {
            get { return _Description; }
            set { _Description = value; }
        }
    }
}
