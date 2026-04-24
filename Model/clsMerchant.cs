using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsMerchant
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _MerchantID;
        public static int _CityID;
        public static int _ProvinceID;
        public static string _MerchantName;
        public static string _Address;
        public static string _City;
        public static string _Province;
        public static string _ContactPerson;
        public static string _ContactNo;

        public static void ResetClass()
        {
            clsMerchant.ClassMerchantID = 0;
            clsMerchant.ClassCityID = 0;
            clsMerchant.ClassProvinceID = 0;
            clsMerchant.ClassMerchantName = "";
            clsMerchant.ClassAddress = "";
            clsMerchant.ClassCity = "";
            clsMerchant.ClassProvince = "";
            clsMerchant.ClassContactPerson = "";
            clsMerchant.ClassContactNo = "";
        }
        public static int ClassMerchantID
        {
            get { return _MerchantID; }
            set { _MerchantID = value; }
        }
        public static int ClassCityID
        {
            get { return _CityID; }
            set { _CityID = value; }
        }
        public static int ClassProvinceID
        {
            get { return _ProvinceID; }
            set { _ProvinceID = value; }
        }

        public static string ClassMerchantName
        {
            get { return _MerchantName; }
            set { _MerchantName = value; }
        }
        public static string ClassAddress
        {
            get { return _Address; }
            set { _Address = value; }
        }
        public static string ClassCity
        {
            get { return _City; }
            set { _City = value; }
        }
        public static string ClassProvince
        {
            get { return _Province; }
            set { _Province = value; }
        }
        public static string ClassContactPerson
        {
            get { return _ContactPerson; }
            set { _ContactPerson = value; }
        }
        public static string ClassContactNo
        {
            get { return _ContactNo; }
            set { _ContactNo = value; }
        }
    }
}
