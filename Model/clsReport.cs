using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsReport
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int ReportID;
        public static string ReportDesc;
        public static string ReportType;
        public static int ReportOrderDisplay;

        public clsReport()
        {
        }

        public static bool ClassRecordFound
        {
            get { return RecordFound; }
            set { RecordFound = value; }
        }
        public static int ClassReportID
        {
            get { return ReportID; }
            set { ReportID = value; }
        }
        public static string ClassReportDesc
        {
            get { return ReportDesc; }
            set { ReportDesc = value; }
        }
        public static string ClassReportType
        {
            get { return ReportType; }
            set { ReportType = value; }
        }
        public static int ClassReportOrderDisplay
        {
            get { return ReportOrderDisplay; }
            set { ReportOrderDisplay = value; }
        }
    }
}
