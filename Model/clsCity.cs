using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsCity
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _CityID;
        public static string _City;

        public static int ClassCityID
        {
            get { return _CityID; }
            set { _CityID = value; }
        }
        public static string ClassCity
        {
            get { return _City; }
            set { _City = value; }
        }
    }
}
