using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsTerminal
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _TerminalID;
        public static int _SIMID;
        public static int _TerminalTypeID;
        public static int _TerminalModelID;
        public static int _TerminalBrandID;
        public static string _No;
        public static string _TerminalSN;
        public static string _SIMSerialNo;
        public static string _TerminalType;
        public static string _TerminalModel;
        public static string _TerminalBrand;
        public static string _DeliveryDate;
        public static string _ReceiveDate;

        public static int _TerminalStatus;
        public static string _TerminalStatusDescription;
        public static int _TerminalCount;
        public static double _TerminalTotal;

        public static int ClassTerminalID
        {
            get { return _TerminalID; }
            set { _TerminalID = value; }
        }
        public static int ClassSIMID
        {
            get { return _SIMID; }
            set { _SIMID = value; }
        }
        public static int ClassTerminalTypeID
        {
            get { return _TerminalTypeID; }
            set { _TerminalTypeID = value; }
        }
        public static int ClassTerminalModelID
        {
            get { return _TerminalModelID; }
            set { _TerminalModelID = value; }
        }
        public static int ClassTerminalBrandID
        {
            get { return _TerminalBrandID; }
            set { _TerminalBrandID = value; }
        }
        public static string ClassNo
        {
            get { return _No; }
            set { _No = value; }
        }
        public static string ClassTerminalSN
        {
            get { return _TerminalSN; }
            set { _TerminalSN = value; }
        }
        public static string ClassSIMSerialNo
        {
            get { return _SIMSerialNo; }
            set { _SIMSerialNo = value; }
        }
        public static string ClassTerminalType
        {
            get { return _TerminalType; }
            set { _TerminalType = value; }
        }
        public static string ClassTerminalModel
        {
            get { return _TerminalModel; }
            set { _TerminalModel = value; }
        }
        public static string ClassTerminalBrand
        {
            get { return _TerminalBrand; }
            set { _TerminalBrand = value; }
        }
        public static string ClassDeliveryDate
        {
            get { return _DeliveryDate; }
            set { _DeliveryDate = value; }
        }
        public static string ClassReceiveDate
        {
            get { return _ReceiveDate; }
            set { _ReceiveDate = value; }
        }
        public static int ClassTerminalStatus
        {
            get { return _TerminalStatus; }
            set { _TerminalStatus = value; }
        }
        public static string ClassTerminalStatusDescription
        {
            get { return _TerminalStatusDescription; }
            set { _TerminalStatusDescription = value; }
        }
        public static int ClassTerminalCount
        {
            get { return _TerminalCount; }
            set { _TerminalCount = value; }
        }
        public static double ClassTerminalTotal
        {
            get { return _TerminalTotal; }
            set { _TerminalTotal = value; }
        }
    }
}
