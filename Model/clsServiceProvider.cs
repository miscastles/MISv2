using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace MIS
{
    public class clsServiceProvider
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _ProviderID;
        public static string _Name;
        public static string _Address;
        public static string _TelNo;
        public static string _Mobile;
        public static string _Fax;
        public static string _Email;
        public static string _ContractTerms;        

        public static int ClassProviderID
        {
            get { return _ProviderID; }
            set { _ProviderID = value; }
        }

        public static string ClassName
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public static string ClassAddress
        {
            get { return _Address; }
            set { _Address = value; }
        }
        public static string ClassTelNo
        {
            get { return _TelNo; }
            set { _TelNo = value; }
        }
        public static string ClassMobile
        {
            get { return _Mobile; }
            set { _Mobile = value; }
        }
        public static string ClassFax
        {
            get { return _Fax; }
            set { _Fax = value; }
        }
        public static string ClassEmail
        {
            get { return _Email; }
            set { _Email = value; }
        }
        public static string ClassContactTerms
        {
            get { return _ContractTerms; }
            set { _ContractTerms = value; }
        }       
    }
}
