using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace MIS
{
    public class clsServiceType
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _ServiceTypeID;
        public static string _Description;
        public static string _Code;
        public static int _Count;
        public static int _ServiceStatus;
        public static string _ServiceStatusDescription;

        public static int ClassServiceTypeID
        {
            get { return _ServiceTypeID; }
            set { _ServiceTypeID = value; }
        }
        public static string ClassDescription
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public static string ClassCode
        {
            get { return _Code; }
            set { _Code = value; }
        }
        public static int ClassServiceCount
        {
            get { return _Count; }
            set { _Count = value; }
        }
        public static int ClassServiceStatus
        {
            get { return _ServiceStatus; }
            set { _ServiceStatus = value; }
        }
        public static string ClassStatusDescription
        {
            get { return _ServiceStatusDescription; }
            set { _ServiceStatusDescription = value; }
        }

        public static int _JobType;
        public static int ClassJobType
        {
            get { return _JobType; }
            set { _JobType = value; }
        }

        public static string _JobTypeDescription;
        public static string ClassJobTypeDescrition
        {
            get { return _JobTypeDescription; }
            set { _JobTypeDescription = value; }
        }

        public static string _ServiceJobTypeDescription;
        public static string ClassServiceJobTypeDescrition
        {
            get { return _ServiceJobTypeDescription; }
            set { _ServiceJobTypeDescription = value; }
        }
    }
}
