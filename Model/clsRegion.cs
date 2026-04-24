using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsRegion
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _RegionID;
        public static string _Region;
        public static string _Province;

        public static int ClassRegionID
        {
            get { return _RegionID; }
            set { _RegionID = value; }
        }
        public static string ClassRegion
        {
            get { return _Region; }
            set { _Region = value; }
        }

        public static string ClassProvince
        {
            get { return _Province; }
            set { _Province = value; }
        }
    }
}
