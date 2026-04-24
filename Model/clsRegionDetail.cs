using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsRegionDetail
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _RegionID;
        public static int _RegionType;
        public static string _Province;
        public static string _Region;

        public static int ClassRegionID
        {
            get { return _RegionID; }
            set { _RegionID = value; }
        }
        public static int ClassRegionType
        {
            get { return _RegionType; }
            set { _RegionType = value; }
        }
        public static string ClassProvince
        {
            get { return _Province; }
            set { _Province = value; }
        }
        public static string ClassRegion
        {
            get { return _Region; }
            set { _Region = value; }
        }
    }
}
