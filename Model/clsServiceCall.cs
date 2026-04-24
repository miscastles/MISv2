using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsServiceCall
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int SCNo;
        public static string SCDateTime;
        public static string ReferralID;
        public static string CustomerName;
        public static string CustomerContactNo;
        public static string ReportedProblem;
        public static string ArrangementMade;
        public static string SCReqDate;
        public static string SCReqTime;
        public static string SCShipDate;
        public static string SCShipTime;
        public static string TrackingNo;
        public static string SCStatus;

        public static int ClassSCNo
        {
            get { return SCNo; }
            set { SCNo = value; }
        }
        public static string ClassSCDateTime
        {
            get { return SCDateTime; }
            set { SCDateTime = value; }
        }
        public static string ClassReferralID
        {
            get { return ReferralID; }
            set { ReferralID = value; }
        }
        public static string ClassCustomerName
        {
            get { return CustomerName; }
            set { CustomerName = value; }
        }
        public static string ClassCustomerContactNo
        {
            get { return CustomerContactNo; }
            set { CustomerContactNo = value; }
        }
        public static string ClassReportedProblem
        {
            get { return ReportedProblem; }
            set { ReportedProblem = value; }
        }
        public static string ClassArrangementMade
        {
            get { return ArrangementMade; }
            set { ArrangementMade = value; }
        }

        public static string ClassSCReqDate
        {
            get { return SCReqDate; }
            set { SCReqDate = value; }
        }
        public static string ClassSCReqTime
        {
            get { return SCReqTime; }
            set { SCReqTime = value; }
        }
        public static string ClassSCShipDate
        {
            get { return SCShipDate; }
            set { SCShipDate = value; }
        }
        public static string ClassSCShipTime
        {
            get { return SCShipTime; }
            set { SCShipTime = value; }
        }
        public static string ClassTrackingNo
        {
            get { return TrackingNo; }
            set { TrackingNo = value; }
        }
        public static string ClassSCStatus
        {
            get { return SCStatus; }
            set { SCStatus = value; }
        }

    }
}
