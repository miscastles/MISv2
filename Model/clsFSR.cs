using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsFSR
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _FSRID;
        public static int _FSRNo;
        public static string _No;
        public static string _Merchant;
        public static string _MID;
        public static string _TID;
        public static string _InvoiceNo;
        public static string _BatchNo;
        public static string _TimeArrived;
        public static string _TimeStart;
        public static string _FSR;
        public static string _ServiceTypeDescription;
        public static string _ServiceStatusDescription;
        public static string _FSRDate;
        public static string _FSRTime;
        public static string _TxnAmt;
        public static string _TimeEnd;
        public static string _TerminalSN;
        public static string _MerchantContactNo;
        public static string _MerchantRepresentative;
        public static string _FEName;
        public static string _SerialNo;
        public static string _FSRStatusDescription;
        public static string _Remarks;
        public static string _FileName;
        public static string _ProcessType;
        public static string _IRNo;
        public static string _ClientName;
        public static string _SIMSN;
        public static string _PowerSN;
        public static string _DockSN;
        public static string _TypeDescription;
        public static string _ModelDescription;

        public static int ClassFSRID
        {
            get { return _FSRID; }
            set { _FSRID = value; }
        }
        public static int ClassFSRNo
        {
            get { return _FSRNo; }
            set { _FSRNo = value; }
        }
        public static string ClassNo
        {
            get { return _No; }
            set { _No = value; }
        }
        public static string ClassMerchant
        {
            get { return _Merchant; }
            set { _Merchant = value; }
        }
        public static string ClassMID
        {
            get { return _MID; }
            set { _MID = value; }
        }
        public static string ClassTID
        {
            get { return _TID; }
            set { _TID = value; }
        }
        public static string ClassInvoiceNo
        {
            get { return _InvoiceNo; }
            set { _InvoiceNo = value; }
        }
        public static string ClassBatchNo
        {
            get { return _BatchNo; }
            set { _BatchNo = value; }
        }
        public static string ClassTimeArrived
        {
            get { return _TimeArrived; }
            set { _TimeArrived = value; }
        }
        public static string ClassTimeStart
        {
            get { return _TimeStart; }
            set { _TimeStart = value; }
        }
        public static string ClassFSR
        {
            get { return _FSR; }
            set { _FSR = value; }
        }
        public static string ClassServiceTypeDescription
        {
            get { return _ServiceTypeDescription; }
            set { _ServiceTypeDescription = value; }
        }
        public static string ClassServiceStatusDescription
        {
            get { return _ServiceStatusDescription; }
            set { _ServiceStatusDescription = value; }
        }
        public static string ClassFSRDate
        {
            get { return _FSRDate; }
            set { _FSRDate = value; }
        }
        public static string ClassFSRTime
        {
            get { return _FSRTime; }
            set { _FSRTime = value; }
        }
        public static string ClassTxnAmt
        {
            get { return _TxnAmt; }
            set { _TxnAmt = value; }
        }
        public static string ClassTimeEnd
        {
            get { return _TimeEnd; }
            set { _TimeEnd = value; }
        }
        public static string ClassTerminalSN
        {
            get { return _TerminalSN; }
            set { _TerminalSN = value; }
        }
        public static string ClassMerchantContactNo
        {
            get { return _MerchantContactNo; }
            set { _MerchantContactNo = value; }
        }
        public static string ClassMerchantRepresentative
        {
            get { return _MerchantRepresentative; }
            set { _MerchantRepresentative = value; }
        }
        public static string ClassFEName
        {
            get { return _FEName; }
            set { _FEName = value; }
        }
        public static string ClassSerialNo
        {
            get { return _SerialNo; }
            set { _SerialNo = value; }
        }
        public static string ClassFSRStatusDescription
        {
            get { return _FSRStatusDescription; }
            set { _FSRStatusDescription = value; }
        }
        public static string ClassRemarks
        {
            get { return _Remarks; }
            set { _Remarks = value; }
        }
        public static string ClassFileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }
        public static string ClassProcessType
        {
            get { return _ProcessType; }
            set { _ProcessType = value; }
        }
        public static string ClassIRNo
        {
            get { return _IRNo; }
            set { _IRNo = value; }
        }
        public static string ClassClientName
        {
            get { return _ClientName; }
            set { _ClientName = value; }
        }
        public static string ClassSIMSN
        {
            get { return _SIMSN; }
            set { _SIMSN = value; }
        }
        public static string ClassPowerSN
        {
            get { return _PowerSN; }
            set { _PowerSN = value; }
        }
        public static string ClassDockSN
        {
            get { return _DockSN; }
            set { _DockSN = value; }
        }
        public static string ClassTypeDescription
        {
            get { return _TypeDescription; }
            set { _TypeDescription = value; }
        }
        public static string ClassModelDescription
        {
            get { return _ModelDescription; }
            set { _ModelDescription = value; }
        }

        public static string _ActionMade;
        public static string ClassActionMade
        {
            get { return _ActionMade; }
            set { _ActionMade = value; }
        }

        public static string _ProblemReported;
        public static string ClassProblemReported
        {
            get { return _ProblemReported; }
            set { _ProblemReported = value; }
        }

        public static string _ActualProblemReported;
        public static string ClassActualProblemReported
        {
            get { return _ActualProblemReported; }
            set { _ActualProblemReported = value; }
        }

        public static string _ActionTaken;
        public static string ClassActionTaken
        {
            get { return _ActionTaken; }
            set { _ActionTaken = value; }
        }

        public static int _ServiceNo;
        public static int ClassServiceNo
        {
            get { return _ServiceNo; }
            set { _ServiceNo = value; }
        }

        public static int _IRIDNo;
        public static int ClassIRIDNo
        {
            get { return _IRIDNo; }
            set { _IRIDNo = value; }
        }

        public static string _RequestNo;
        public static string ClassRequsetNo
        {
            get { return _RequestNo; }
            set { _RequestNo = value; }
        }

        public static int _TAIDNo;
        public static int ClassTAIDNo
        {
            get { return _TAIDNo; }
            set { _TAIDNo = value; }
        }

        public static string _JobTypeDescription;
        public static string ClassJobTypeDescription
        {
            get { return _JobTypeDescription; }
            set { _JobTypeDescription = value; }
        }

        public static string _ServiceJobTypeDescription;
        public static string ClassServiceJobTypeDescription
        {
            get { return _ServiceJobTypeDescription; }
            set { _ServiceJobTypeDescription = value; }
        }
    }
}
