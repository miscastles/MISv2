using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsProvince
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _ProvinceID;
        public static string _Province;

        public static int ClassProvinceID
        {
            get { return _ProvinceID; }
            set { _ProvinceID = value; }
        }
        public static string ClassProvince
        {
            get { return _Province; }
            set { _Province = value; }
        }
    }
}
