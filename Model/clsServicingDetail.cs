using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsServicingDetail
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int ServiceNo;

        public static int TerminalID;
        public static int SIMID;
        public static int DockID;

        public static string TerminalSN;
        public static string SIMSN;
        public static string DockSN;

        public static int CurTerminalSNStatus;
        public static int CurSIMSNStatus;
        public static int CurDockSNStatus;

        public static string CurTerminalSNStatusDescription;
        public static string CurSIMSNStatusDescription;
        public static string CurDockSNStatusDescription;

        public static int RepTerminalSNStatus;
        public static int RepSIMSNStatus;
        public static int RepDockSNStatus;

        public static string RepTerminalSNStatusDescription;
        public static string RepSIMSNStatusDescription;
        public static string RepDockSNStatusDescription;

        public static string CounterNo;
        public static string IRNo;
        public static string RequestNo;
        public static string ServiceDateTime;
        public static string ServiceDate;
        public static string ServiceTime;
        public static string CustomerName;
        public static string CustomerContactNo;
        public static string Remarks;
        public static string ServiceReqDate;
        public static string ServiceReqTime;
        public static string LastServiceRequest;
        public static string NewServiceRequest;
        public static string ReplaceTerminalSN;
        public static string ReplaceSIMSN;
        public static string ReplaceDockSN;
        public static int JobType;
        public static string JobTypeDescription;
        public static string JobTypeSubDescription;
        public static string JobTypeStatusDescription;        

        public static int ClassServiceNo
        {
            get { return ServiceNo; }
            set { ServiceNo = value; }
        }

        public static int IRIDNo;
        public static int ClassIRIDNo
        {
            get { return IRIDNo; }
            set { IRIDNo = value; }
        }

        public static int TAIDNo;
        public static int ClassTAIDNo
        {
            get { return TAIDNo; }
            set { TAIDNo = value; }
        }

        public static int ClientID;
        public static int ClassClientID
        {
            get { return ClientID; }
            set { ClientID = value; }
        }

        public static int MerchantID;
        public static int ClassMerchantID
        {
            get { return MerchantID; }
            set { MerchantID = value; }
        }

        public static int ClassTerminalID
        {
            get { return TerminalID; }
            set { TerminalID = value; }
        }
        public static int ClassSIMID
        {
            get { return SIMID; }
            set { SIMID = value; }
        }
        public static int ClassDockID
        {
            get { return DockID; }
            set { DockID = value; }
        }
        public static string ClassTerminalSN
        {
            get { return TerminalSN; }
            set { TerminalSN = value; }
        }
        public static string ClassSIMSN
        {
            get { return SIMSN; }
            set { SIMSN = value; }
        }
        public static string ClassDockSN
        {
            get { return DockSN; }
            set { DockSN = value; }
        }
        public static int ClassCurTerminalSNStatus
        {
            get { return CurTerminalSNStatus; }
            set { CurTerminalSNStatus = value; }
        }
        public static int ClassCurSIMSNStatus
        {
            get { return CurSIMSNStatus; }
            set { CurSIMSNStatus = value; }
        }
        public static int ClassCurDockSNStatus
        {
            get { return CurDockSNStatus; }
            set { CurDockSNStatus = value; }
        }
        public static string ClassCurTerminalSNStatusDescription
        {
            get { return CurTerminalSNStatusDescription; }
            set { CurTerminalSNStatusDescription = value; }
        }
        public static string ClassCurSIMSNStatusDescription
        {
            get { return CurSIMSNStatusDescription; }
            set { CurSIMSNStatusDescription = value; }
        }
        public static string ClassCurDockSNStatusDescription
        {
            get { return CurDockSNStatusDescription; }
            set { CurDockSNStatusDescription = value; }
        }
        public static int ClassRepTerminalSNStatus
        {
            get { return RepTerminalSNStatus; }
            set { RepTerminalSNStatus = value; }
        }
        public static int ClassRepSIMSNStatus
        {
            get { return RepSIMSNStatus; }
            set { RepSIMSNStatus = value; }
        }
        public static int ClassRepDockSNStatus
        {
            get { return RepDockSNStatus; }
            set { RepDockSNStatus = value; }
        }
        public static string ClassRepTerminalSNStatusDescription
        {
            get { return RepTerminalSNStatusDescription; }
            set { RepTerminalSNStatusDescription = value; }
        }
        public static string ClassRepSIMSNStatusDescription
        {
            get { return RepSIMSNStatusDescription; }
            set { RepSIMSNStatusDescription = value; }
        }
        public static string ClassRepDockSNStatusDescription
        {
            get { return RepDockSNStatusDescription; }
            set { RepDockSNStatusDescription = value; }
        }
        public static string ClassCounterNo
        {
            get { return CounterNo; }
            set { CounterNo = value; }
        }
        public static string ClassRequestNo
        {
            get { return RequestNo; }
            set { RequestNo = value; }
        }
        public static string ClassIRNo
        {
            get { return IRNo; }
            set { IRNo = value; }
        }
        public static string ClassServiceDateTime
        {
            get { return ServiceDateTime; }
            set { ServiceDateTime = value; }
        }
        public static string ClassServiceDate
        {
            get { return ServiceDate; }
            set { ServiceDate = value; }
        }
        public static string ClassServiceTime
        {
            get { return ServiceTime; }
            set { ServiceTime = value; }
        }

        public static string _ServiceReqDate;
        
        public static string ClassServiceReqDate
        {
            get { return _ServiceReqDate; }
            set { _ServiceReqDate = value; }
        }
        public static string _ServiceReqTime;
        public static string ClassServiceReqTime
        {
            get { return _ServiceReqTime; }
            set { _ServiceReqTime = value; }
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
        public static string ClassRemarks
        {
            get { return Remarks; }
            set { Remarks = value; }
        }
        public static int ClassJobType
        {
            get { return JobType; }
            set { JobType = value; }
        }
        public static string ClassJobTypeDescription
        {
            get { return JobTypeDescription; }
            set { JobTypeDescription = value; }
        }
        public static string ClassJobTypeSubDescription
        {
            get { return JobTypeSubDescription; }
            set { JobTypeSubDescription = value; }
        }
        public static string ClassJobTypeStatusDescription
        {
            get { return JobTypeStatusDescription; }
            set { JobTypeStatusDescription = value; }
        }

        public static string ReferenceNo;
        public static string ClassReferenceNo
        {
            get { return ReferenceNo; }
            set { ReferenceNo = value; }
        }

        public static string AppVersion;
        public static string ClassAppVersion
        {
            get { return AppVersion; }
            set { AppVersion = value; }
        }

        public static string AppCRC;
        public static string ClassAppCRC
        {
            get { return AppCRC; }
            set { AppCRC = value; }
        }
    }
}
